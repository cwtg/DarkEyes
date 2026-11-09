using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Visibility;

namespace DarkEyes.Backend.Pipeline;

/// <summary>The working copy of one dispatch: state, last projection sent and the steps built so far. Applied atomically or discarded.</summary>
internal sealed class Transaction(GameState state, Projection sent)
{
    private readonly SortedSet<EntityId> _dirty = new(EntityId.Ordinal);
    private readonly ImmutableArray<Step>.Builder _steps = ImmutableArray.CreateBuilder<Step>();

    public GameState State { get; private set; } = state;

    public Projection Sent { get; private set; } = sent;

    public void Apply(Op op)
    {
        switch (op)
        {
            case SetComponentOp set:
                State = State.WithEntity(State.GetEntity(set.Entity).With(set.Key, set.Component));
                _dirty.Add(set.Entity);
                break;
            case SetRandomStreamOp stream:
                // Hidden non-entity state, not part of any projection, so nothing becomes dirty.
                State = State.WithRandomStream(stream.Stream, stream.State);
                break;
            default:
                throw new InvalidOperationException($"Unknown op {op.GetType().Name}.");
        }
    }

    /// <summary>Closes the current step: the event plus the patches caused by the ops since the previous event.</summary>
    public void Emit(IEvent gameEvent)
    {
        (Projection projection, ImmutableArray<Patch> patches) = Projection.Update(Sent, State, _dirty);
        Sent = projection;
        _dirty.Clear();
        _steps.Add(new Step(gameEvent, patches));
    }

    /// <summary>
    /// Ends the transaction. Changes to entities must be explained by an event; changes to hidden state
    /// (e.g. a check that only advanced a random stream) need none.
    /// </summary>
    public CommitResult Complete()
    {
        if (_dirty.Count > 0)
        {
            throw new InvalidOperationException($"Ops after the last event on {string.Join(", ", _dirty)}. Every change must be followed by an event.");
        }

        return new CommitResult(_steps.ToImmutable());
    }
}
