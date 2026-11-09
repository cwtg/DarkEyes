using System;

namespace DarkEyes.Backend.Checks;

/// <summary>A dice-free assessment of a check: may it be attempted, and if not, the rule module's reason.</summary>
public sealed record CheckAssessment
{
    private CheckAssessment(Reason? reason)
    {
        Reason = reason;
    }

    public static CheckAssessment Possible { get; } = new(reason: null);

    public bool IsPossible => Reason is null;

    /// <summary>Why the check can't be attempted (a rule-module code); null if it can.</summary>
    public Reason? Reason { get; }

    public static CheckAssessment Impossible(Reason reason) => new(reason ?? throw new ArgumentNullException(nameof(reason)));
}
