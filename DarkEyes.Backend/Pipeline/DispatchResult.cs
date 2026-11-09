namespace DarkEyes.Backend.Pipeline;

public abstract record DispatchResult
{
    private protected DispatchResult()
    {
    }
}

public sealed record Committed(CommitResult Commit) : DispatchResult;

/// <summary>The action was refused; nothing changed.</summary>
public sealed record Rejected(Rejection Reason) : DispatchResult;
