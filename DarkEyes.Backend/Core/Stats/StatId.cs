using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Core.Stats
{
    public readonly struct StatId(string value) : IEquatable<StatId>
    {
        public string Value { get; } = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("StatId cannot be null or empty.", nameof(value))
            : value;

        public static bool operator ==(StatId left, StatId right) => left.Equals(right);
        public static bool operator !=(StatId left, StatId right) => !left.Equals(right);
        public bool Equals(StatId other) => other.Value == Value;
        public override bool Equals(object obj) => obj is StatId id && Equals(id);
        public override int GetHashCode() => Value != null ? EqualityComparer<string>.Default.GetHashCode(Value) : 0;
        public override string ToString() => Value;
    }
}
