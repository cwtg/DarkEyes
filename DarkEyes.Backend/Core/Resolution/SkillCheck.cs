using System;
using DarkEyes.Backend.Core.Modifier;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Resolution
{
    public sealed class SkillCheck : IResolver
    {
        public ResolutionResult Resolve(ResolutionRequest request, IRandomSource random)
        {
            if (request.Subject is not Skill) throw new ArgumentException("Relevant stat must be a skill.", nameof(request));

            var skill = request.Subject as Skill;

            var valSet = new SkillWorkingSet
            {
                Skill = ModifierEvaluator.ApplyPreRoll(skill.Value, request.PreRoll, new(skill.Id, skill.Value)),
                Attribute1 = ModifierEvaluator.ApplyPreRoll(skill.Attribute1.Value, request.PreRoll, new(skill.Attribute1.Id, skill.Attribute1.Value)) + request.ChallengeRating,
                Attribute2 = ModifierEvaluator.ApplyPreRoll(skill.Attribute2.Value, request.PreRoll, new(skill.Attribute2.Id, skill.Attribute2.Value)) + request.ChallengeRating,
                Attribute3 = ModifierEvaluator.ApplyPreRoll(skill.Attribute3.Value, request.PreRoll, new(skill.Attribute3.Id, skill.Attribute3.Value)) + request.ChallengeRating
            };

            if (!AllowCheck(valSet)) throw new InvalidOperationException("Skill check cannot be performed due to invalid skill values.");

            ResolutionResult result = RollDice(valSet, random);
            ModifierEvaluator.ApplyPostRoll(result, request.PostRoll, new(result.Type, result.QualityLevel));
            return result;
        }

        private static bool AllowCheck(SkillWorkingSet set) => set.Skill >= 0 && set.Attribute1 > 0 && set.Attribute2 > 0 && set.Attribute3 > 0;

        private static ResolutionResult RollDice(SkillWorkingSet valSet, IRandomSource random)
        {
            int roll1 = DiceRoll.D20(random);
            int roll2 = DiceRoll.D20(random);
            int roll3 = DiceRoll.D20(random);

            int remainingPool = valSet.Skill;
            remainingPool -= Math.Max(0, roll1 - valSet.Attribute1);
            remainingPool -= Math.Max(0, roll2 - valSet.Attribute2);
            remainingPool -= Math.Max(0, roll3 - valSet.Attribute3);

            int qualityLevel = Math.Max(1, Math.Min((remainingPool + 2) / 3, 6));

            int count1 = NumberCount(1, roll1, roll2, roll3);
            int count20 = NumberCount(20, roll1, roll2, roll3);

            if (count20 == 3) return new(ResultType.CriticalBlunder, 0);
            if (count20 == 2) return new(ResultType.Blunder, 0);
            if (count1 == 2) return new(ResultType.CriticalSuccess, qualityLevel);
            if (count1 == 3) return new(ResultType.SuperCriticalSuccess, qualityLevel);
            if (remainingPool < 0) return new(ResultType.Failed, 0);

            return new(ResultType.Success, qualityLevel);
        }

        private static int NumberCount(int number, int val1, int val2, int val3)
            => (val1 == number ? 1 : 0) + (val2 == number ? 1 : 0) + (val3 == number ? 1 : 0);
    }

    internal ref struct SkillWorkingSet
    {
        public int Skill;
        public int Attribute1;
        public int Attribute2;
        public int Attribute3;
    }
}
