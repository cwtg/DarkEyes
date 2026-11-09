using System.Collections.Immutable;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Visibility;

namespace DarkEyes.Backend.Pipeline;

/// <summary>Owns the committed state; the single, serialized entry point for changes.</summary>
public sealed class GameStore
{
    private readonly ActionRouter _actions;
    private readonly EffectRouter _effects;
    private Projection _sent = Projection.Empty;

    internal GameStore(GameState state, ActionRouter actions, EffectRouter effects)
    {
        State = state;
        _actions = actions;
        _effects = effects;
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
        CommitResult commit = new Chain(transaction, _actions, _effects).Run(action);

        State = transaction.State;
        _sent = transaction.Sent;
        return new Committed(commit);
    }
}
