using System.Collections.Immutable;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Visibility;

namespace DarkEyes.Backend.Pipeline;

/// <summary>Owns the committed state; the single, serialized entry point for changes.</summary>
public sealed class GameStore
{
    private readonly PipelineServices _services;
    private Projection _sent = Projection.Empty;

    internal GameStore(GameState state, PipelineServices services)
    {
        State = state;
        _services = services;
    }

    internal GameState State { get; private set; }

    /// <summary>Sends the full projection as patches (a diff against an empty projection), e.g. after start or load.</summary>
    public ImmutableArray<Patch> Connect()
    {
        (Projection projection, ImmutableArray<Patch> patches) = Projection.Update(Projection.Empty, State, State.Entities.Keys);
        _sent = projection;
        return patches;
    }

    /// <summary>The player entry point.</summary>
    public DispatchResult Dispatch(IAction action)
    {
        Transaction transaction = new(State, _sent);
        Reason? rejection = new Chain(transaction, _services).Run(action);
        if (rejection is not null)
        {
            return new Rejected(rejection);
        }

        CommitResult commit = transaction.Complete();
        State = transaction.State;
        _sent = transaction.Sent;
        return new Committed(commit);
    }
}
