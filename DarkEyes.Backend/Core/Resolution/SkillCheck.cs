using System;
using System.Collections.Generic;
using DarkEyes.Backend.Core.Modifiers;
using DarkEyes.Backend.Core.Stats;

namespace DarkEyes.Backend.Core.Resolution;

public sealed class SkillCheck : IResolver
{
    public ResolutionResult Resolve(ResolutionRequest request, IRandomSource random)
    {
        if (request.Subject is not Skill skill)
        {
            throw new ArgumentException("Relevant stat must be a skill.", nameof(request));
        }

        IReadOnlyList<IModifier> modifiers = request.Modifiers ?? [];

        var valSet = new SkillWorkingSet
        {
            Skill = ModifierEvaluator.ApplyModifiers(skill.Value, modifiers, new(skill.Id)),
            Attribute1 = ModifierEvaluator.ApplyModifiers(skill.Attribute1.Value, modifiers, new(skill.Attribute1.Id)),
            Attribute2 = ModifierEvaluator.ApplyModifiers(skill.Attribute2.Value, modifiers, new(skill.Attribute2.Id)),
            Attribute3 = ModifierEvaluator.ApplyModifiers(skill.Attribute3.Value, modifiers, new(skill.Attribute3.Id))
        };

        if (!AllowCheck(valSet))
        {
            throw new InvalidOperationException("Skill check cannot be performed due to invalid skill values.");
        }

        return RollDice(valSet, random);
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

        if (count20 == 3)
        {
            return new(ResultType.CriticalBlunder, 0);
        }

        if (count20 == 2)
        {
            return new(ResultType.Blunder, 0);
        }

        if (count1 == 2)
        {
            return new(ResultType.CriticalSuccess, qualityLevel);
        }

        if (count1 == 3)
        {
            return new(ResultType.SuperCriticalSuccess, qualityLevel);
        }

        if (remainingPool < 0)
        {
            return new(ResultType.Failed, 0);
        }

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
