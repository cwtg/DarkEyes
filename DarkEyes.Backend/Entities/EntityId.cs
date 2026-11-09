using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Entities;

public readonly struct EntityId(string value) : IEquatable<EntityId>
{
    public string Value { get; } = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("EntityId cannot be null or empty.", nameof(value))
            : value;

    public static bool operator ==(EntityId left, EntityId right) => left.Equals(right);
    public static bool operator !=(EntityId left, EntityId right) => !left.Equals(right);
    public bool Equals(EntityId other) => other.Value == Value;
    public override bool Equals(object obj) => obj is EntityId id && Equals(id);
    public override int GetHashCode() => Value != null ? EqualityComparer<string>.Default.GetHashCode(Value) : 0;
    public override string ToString() => Value;
}
