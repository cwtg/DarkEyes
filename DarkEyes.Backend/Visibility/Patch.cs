using System.Collections.Immutable;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Visibility;

/// <summary>A change to the observer's mirror, derived by diffing projections. Closed: the frontend's patch applier must be exhaustive.</summary>
public abstract record Patch
{
    private protected Patch(EntityId entity)
    {
        Entity = entity;
    }

    public EntityId Entity { get; }
}

public sealed record EntityAppeared(EntityId Entity, ImmutableSortedDictionary<string, object> Components) : Patch(Entity);

public sealed record ComponentReplaced(EntityId Entity, string Component, object Value) : Patch(Entity);
