using System;
using DarkEyes.Backend.Randomness;

namespace DarkEyes.Rules.Dsa5.Checks;

/// <summary>
/// The DSA5 skill check (3d20): each die is compared with its attribute value, raised or lowered by the modifier;
/// points above it are paid from the skill rating; what remains gives the quality level. Two or three 1s or 20s
/// override the sum.
/// </summary>
public static class SkillCheck
{
    private const int MaxQualityLevel = 6;

    /// <summary>DSA5: no check is possible once the modifier lowers one of its values to 0 or below. Returns the first such position, or null.</summary>
    public static CheckPosition? FirstTooLow(CheckValues values, int modifier)
    {
        if (values.First + modifier <= 0)
        {
            return CheckPosition.First;
        }

        if (values.Second + modifier <= 0)
        {
            return CheckPosition.Second;
        }

        if (values.Third + modifier <= 0)
        {
            return CheckPosition.Third;
        }

        return null;
    }

    /// <summary>Rolls the check. Only for possible checks (see <see cref="FirstTooLow"/>); callers check that first, without dice.</summary>
    public static SkillCheckOutcome Resolve(int skillRating, CheckValues values, int modifier, IRandomSource dice)
    {
        if (FirstTooLow(values, modifier) is { } position)
        {
            throw new ArgumentException($"The check is not possible: the modifier {modifier} lowers the {position} value to 0 or below.", nameof(modifier));
        }

        CheckRolls rolls = new(D20(dice), D20(dice), D20(dice));

        int remaining = skillRating
            - Overshoot(rolls.First, values.First + modifier)
            - Overshoot(rolls.Second, values.Second + modifier)
            - Overshoot(rolls.Third, values.Third + modifier);

        int ones = Count(rolls, 1);
        int twenties = Count(rolls, 20);

        SkillCheckResult result = (twenties, ones) switch
        {
            (3, _) => SkillCheckResult.SpectacularBlunder,
            (2, _) => SkillCheckResult.Blunder,
            (_, 3) => SkillCheckResult.SpectacularSuccess,
            (_, 2) => SkillCheckResult.CriticalSuccess,
            _ when remaining < 0 => SkillCheckResult.Failure,
            _ => SkillCheckResult.Success,
        };

        return new SkillCheckOutcome(rolls, result, SkillCheckOutcome.IsSuccess(result) ? QualityLevel(remaining) : 0);
    }

    private static int D20(IRandomSource dice) => dice.Next(1, 21);

    private static int Overshoot(int roll, int value) => Math.Max(0, roll - value);

    private static int Count(CheckRolls rolls, int face) =>
        (rolls.First == face ? 1 : 0) + (rolls.Second == face ? 1 : 0) + (rolls.Third == face ? 1 : 0);

    /// <summary>0–3 remaining points give 1, 4–6 give 2, and so on up to 6; at least 1 for any success.</summary>
    private static int QualityLevel(int remaining) => Math.Min(MaxQualityLevel, Math.Max(1, (remaining + 2) / 3));
}
