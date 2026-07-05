using System;

namespace DarkEyes.Backend.Entities
{
    public readonly struct EntityId(string value)
    {
        public string Value { get; } = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("EntityId cannot be null or empty.", nameof(value))
            : value;

        public override string ToString() => Value;
    }
}
