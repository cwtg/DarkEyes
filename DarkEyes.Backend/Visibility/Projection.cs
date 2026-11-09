using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Visibility;

/// <summary>What the observer sees of the state. No redaction or perception yet: every entity is visible as stored.</summary>
public sealed class Projection
{
    private Projection(ImmutableSortedDictionary<EntityId, ImmutableSortedDictionary<string, object>> entities)
    {
        Entities = entities;
    }

    public static Projection Empty { get; } = new(ImmutableSortedDictionary.Create<EntityId, ImmutableSortedDictionary<string, object>>(EntityId.Ordinal));

    public ImmutableSortedDictionary<EntityId, ImmutableSortedDictionary<string, object>> Entities { get; }

    /// <summary>The full projection of a state.</summary>
    internal static Projection Of(GameState state) => Update(Empty, state, state.Entities.Keys).Projection;

    /// <summary>Re-projects the given entities and diffs them against the last projection sent.</summary>
    internal static (Projection Projection, ImmutableArray<Patch> Patches) Update(Projection sent, GameState state, IEnumerable<EntityId> dirty)
    {
        ImmutableSortedDictionary<EntityId, ImmutableSortedDictionary<string, object>> entities = sent.Entities;
        ImmutableArray<Patch>.Builder patches = ImmutableArray.CreateBuilder<Patch>();

        foreach (EntityId id in dirty)
        {
            ImmutableSortedDictionary<string, object> after = state.GetEntity(id).Components;
            if (entities.TryGetValue(id, out ImmutableSortedDictionary<string, object>? before))
            {
                AddComponentPatches(id, before, after, patches);
            }
            else
            {
                patches.Add(new EntityAppeared(id, after));
            }

            entities = entities.SetItem(id, after);
        }

        return (new Projection(entities), patches.ToImmutable());
    }

    private static void AddComponentPatches(
        EntityId id,
        ImmutableSortedDictionary<string, object> before,
        ImmutableSortedDictionary<string, object> after,
        ImmutableArray<Patch>.Builder patches)
    {
        foreach (KeyValuePair<string, object> component in after)
        {
            if (!before.TryGetValue(component.Key, out object? previous))
            {
                throw new NotSupportedException("Adding a component to a visible entity is not supported yet.");
            }

            if (!ReferenceEquals(previous, component.Value) && !Equals(previous, component.Value))
            {
                patches.Add(new ComponentReplaced(id, component.Key, component.Value));
            }
        }

        if (before.Keys.Any(name => !after.ContainsKey(name)))
        {
            throw new NotSupportedException("Removing a component from a visible entity is not supported yet.");
        }
    }
}
