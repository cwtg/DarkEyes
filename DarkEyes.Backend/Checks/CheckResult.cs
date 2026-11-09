using System;

namespace DarkEyes.Backend.Checks;

/// <summary>
/// The generic result of a check. Closed, and handled through <see cref="Match"/>: a new case changes its signature, so every
/// caller must decide what it means. Callers that don't distinguish "not possible" from "failed" leave out <c>notPossible</c>.
/// </summary>
public abstract record CheckResult
{
    private protected CheckResult()
    {
    }

    public abstract T Match<T>(Func<int, T> succeeded, Func<T> failed, Func<Reason, T>? notPossible = null);

    public void Match(Action<int> succeeded, Action failed, Action<Reason>? notPossible = null)
    {
        Match<bool>(
            degree => { succeeded(degree); return true; },
            () => { failed(); return true; },
            notPossible is null ? null : reason => { notPossible(reason); return true; });
    }
}

/// <summary>The check succeeded; the degree says how well (at least 1).</summary>
public sealed record CheckSucceeded : CheckResult
{
    public CheckSucceeded(int degree)
    {
        Degree = degree >= 1 ? degree : throw new ArgumentOutOfRangeException(nameof(degree), degree, "A success has a degree of at least 1.");
    }

    public int Degree { get; }

    public override T Match<T>(Func<int, T> succeeded, Func<T> failed, Func<Reason, T>? notPossible = null) => succeeded(Degree);
}

public sealed record CheckFailed : CheckResult
{
    public override T Match<T>(Func<int, T> succeeded, Func<T> failed, Func<Reason, T>? notPossible = null) => failed();
}

/// <summary>The rules don't allow the check at all; no dice were rolled. Treated as failed unless the caller handles it.</summary>
public sealed record CheckNotPossible(Reason Reason) : CheckResult
{
    public override T Match<T>(Func<int, T> succeeded, Func<T> failed, Func<Reason, T>? notPossible = null) =>
        notPossible is null ? failed() : notPossible(Reason);
}
