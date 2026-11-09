namespace DarkEyes.Backend.Pipeline;

/// <summary>The last event of every committed action: which action it was and how it went (architecture 8.2).</summary>
public sealed record ActionCompleted(IAction Action, ActOutcome Outcome) : IEvent;
