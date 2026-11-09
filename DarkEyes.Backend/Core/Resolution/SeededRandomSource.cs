#pragma warning disable RS0030 // Legacy code, replaced or moved to the rule module in story 01 (docs/implementation/01-open-chest.md).

using System;

namespace DarkEyes.Backend.Core.Resolution;

public sealed class SeededRandomSource(int? seed) : IRandomSource
{
    private readonly Random _random = seed.HasValue ? new(seed.Value) : new();

    public int Next()
    {
        return _random.Next();
    }

    public int Next(int max)
    {
        return _random.Next(max);
    }

    public int Next(int min, int max)
    {
        return _random.Next(min, max);
    }
}
