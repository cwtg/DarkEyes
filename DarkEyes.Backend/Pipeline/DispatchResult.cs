namespace DarkEyes.Backend.Pipeline;

public abstract record DispatchResult
{
    private protected DispatchResult()
    {
    }
}

public sealed record Committed(CommitResult Commit) : DispatchResult;
