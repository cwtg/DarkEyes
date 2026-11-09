namespace DarkEyes.Rules.Dsa5.Checks;

/// <summary>A resolved skill check: the rolls, the result and the quality level (0 unless successful).</summary>
public sealed record SkillCheckOutcome(CheckRolls Rolls, SkillCheckResult Result, int QualityLevel)
{
    public bool Succeeded => IsSuccess(Result);

    internal static bool IsSuccess(SkillCheckResult result) =>
        result is SkillCheckResult.Success or SkillCheckResult.CriticalSuccess or SkillCheckResult.SpectacularSuccess;
}
