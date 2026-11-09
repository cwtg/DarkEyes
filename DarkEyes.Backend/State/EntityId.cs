using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.State;

/// <summary>Stable identity of an entity: a content ID for authored entities, a generated ID for spawns.</summary>
public readonly record struct EntityId(string Value)
{
    /// <summary>Ordinal order; the only order used for entity IDs (determinism).</summary>
    public static IComparer<EntityId> Ordinal { get; } = Comparer<EntityId>.Create((left, right) => string.CompareOrdinal(left.Value, right.Value));

    public string Value { get; } = string.IsNullOrWhiteSpace(Value)
        ? throw new ArgumentException("An entity ID must not be empty.", nameof(Value))
        : Value;

    public override string ToString() => Value;
}
