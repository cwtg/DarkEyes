using System;

namespace DarkEyes.Backend.Randomness;

/// <summary>The complete state of one PCG32 stream; stored in the game state and in saves. The increment must be odd (full period).</summary>
internal readonly record struct Pcg32State(ulong State, ulong Increment)
{
    public ulong Increment { get; } = (Increment & 1UL) == 0
        ? throw new ArgumentException("A PCG32 increment must be odd.", nameof(Increment))
        : Increment;
}
