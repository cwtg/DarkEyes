using System;

namespace DarkEyes.Backend.Core.Stats
{
    public readonly struct StatId(string value)
    {
        public string Value { get; } = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("StatId cannot be null or empty.", nameof(value))
            : value;

        public override string ToString() => Value;
    }
}
