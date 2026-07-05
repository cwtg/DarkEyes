using System;
using DarkEyes.Backend.Core.Modifier;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Resolution
{
    public sealed class SkillCheck(IRandomSource random) : IResolver
    {
        private readonly IRandomSource _random = random;
        private Skill? _relevantSkill;

        public ResolutionResult Result { get; private set; }

        public ResolutionResult Resolve(ResolutionRequest request)
        {
            if (request.RelevantStat is not Skill)
                throw new ArgumentException("Relevant stat must be a skill.", nameof(request));

            _relevantSkill = request.RelevantStat as Skill;

            ModifierPipeline.ApplyPreRoll(request);

            if (!AllowCheck()) throw new InvalidOperationException("Skill check cannot be performed due to invalid skill values.");

            Result = RollDice();
            ModifierPipeline.ApplyPostRoll(Result);
            return Result;
        }

        private bool AllowCheck()
        {
            return _relevantSkill is not null
                && _relevantSkill.Value >= 0
                && _relevantSkill.Attribute1.Value > 0
                && _relevantSkill.Attribute2.Value > 0
                && _relevantSkill.Attribute3.Value > 0;
        }

        private ResolutionResult RollDice()
        {
            int roll1 = DiceRoll.D20(_random);
            int roll2 = DiceRoll.D20(_random);
            int roll3 = DiceRoll.D20(_random);

            int remainingPool = _relevantSkill.Value;
            remainingPool -= Math.Max(0, _relevantSkill.Attribute1.Value - roll1);
            remainingPool -= Math.Max(0, _relevantSkill.Attribute2.Value - roll2);
            remainingPool -= Math.Max(0, _relevantSkill.Attribute3.Value - roll3);

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
}
