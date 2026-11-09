using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Randomness;

/// <summary>Identifies the PRNG stream of one random consumer.</summary>
public readonly record struct RandomStreamId(string Value)
{
    public static IComparer<RandomStreamId> Ordinal { get; } = Comparer<RandomStreamId>.Create((left, right) => string.CompareOrdinal(left.Value, right.Value));

    public string Value { get; } = string.IsNullOrWhiteSpace(Value)
        ? throw new ArgumentException("A random stream ID must not be empty.", nameof(Value))
        : Value;

    public override string ToString() => Value;
}
