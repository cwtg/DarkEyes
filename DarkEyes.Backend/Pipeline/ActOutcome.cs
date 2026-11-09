namespace DarkEyes.Backend.Pipeline;

/// <summary>How a committed act went. Closed: the frontend presents every case.</summary>
public abstract record ActOutcome
{
    private protected ActOutcome()
    {
    }

    public static ActOutcome Succeeded { get; } = new ActSucceeded();

    public static ActOutcome Failed { get; } = new ActFailed();

    public static ActOutcome NotPossible(Reason reason) => new ActNotPossible(reason);
}

public sealed record ActSucceeded : ActOutcome;

public sealed record ActFailed : ActOutcome;

/// <summary>The rules didn't allow the act's check at all.</summary>
public sealed record ActNotPossible(Reason Reason) : ActOutcome;
