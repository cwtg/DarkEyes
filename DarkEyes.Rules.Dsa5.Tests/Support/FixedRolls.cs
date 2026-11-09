using System;
using System.Collections.Generic;
using DarkEyes.Backend.Randomness;

namespace DarkEyes.Rules.Dsa5.Tests.Support;

/// <summary>A scripted random source: returns exactly the given values, in order.</summary>
internal sealed class FixedRolls(params int[] values) : IRandomSource
{
    private readonly Queue<int> _values = new(values);

    public int Next(int minInclusive, int maxExclusive)
    {
        int value = _values.Dequeue();
        if (value < minInclusive || value >= maxExclusive)
        {
            throw new InvalidOperationException($"Scripted roll {value} is outside [{minInclusive}, {maxExclusive}).");
        }

        return value;
    }
}
