using System;

namespace DarkEyes.Backend.Pipeline;

/// <summary>
/// What an action handler returns: a rejection (validation failed, nothing changes) or the act's outcome, which the pipeline
/// reports as the commit's last event (<see cref="ActionCompleted"/>). Returning it, not emitting it, means no act can forget it.
/// </summary>
public sealed record ActResult
{
    private ActResult(Reason? rejection, ActOutcome? outcome)
    {
        Rejection = rejection;
        Outcome = outcome;
    }

    public static ActResult Succeeded { get; } = new(rejection: null, ActOutcome.Succeeded);

    public static ActResult Failed { get; } = new(rejection: null, ActOutcome.Failed);

    /// <summary>Why the action was rejected; null if it was carried out.</summary>
    public Reason? Rejection { get; }

    /// <summary>How the act went; null if it was rejected.</summary>
    public ActOutcome? Outcome { get; }

    /// <summary>Validation failed. Only valid before the first check, effect or event (architecture 8.9).</summary>
    public static ActResult Reject(Reason reason) => new(reason ?? throw new ArgumentNullException(nameof(reason)), outcome: null);

    public static ActResult NotPossible(Reason reason) => new(rejection: null, ActOutcome.NotPossible(reason ?? throw new ArgumentNullException(nameof(reason))));
}
