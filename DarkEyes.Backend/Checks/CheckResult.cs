namespace DarkEyes.Backend.Checks;

/// <summary>The generic result of a check; the act decides what it means for its outcome.</summary>
public sealed record CheckResult(bool Succeeded, int Degree);
