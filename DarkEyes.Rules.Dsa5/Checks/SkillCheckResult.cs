namespace DarkEyes.Rules.Dsa5.Checks;

/// <summary>The DSA5 outcomes of a skill check.</summary>
public enum SkillCheckResult
{
    /// <summary>Three 20s.</summary>
    SpectacularBlunder,

    /// <summary>Two 20s.</summary>
    Blunder,

    Failure,

    Success,

    /// <summary>Two 1s.</summary>
    CriticalSuccess,

    /// <summary>Three 1s.</summary>
    SpectacularSuccess,
}
