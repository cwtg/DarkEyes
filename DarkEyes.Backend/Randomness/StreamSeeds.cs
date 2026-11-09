using System;
using System.Text;

namespace DarkEyes.Backend.Randomness;

/// <summary>
/// Derives a stream's initial state from the master seed and a stable hash of its ID, so adding a random consumer
/// never shifts another's rolls.
/// </summary>
internal static class StreamSeeds
{
    public static Pcg32State Derive(ulong masterSeed, RandomStreamId stream)
    {
        if (stream == default)
        {
            throw new ArgumentException("A stream needs an ID.", nameof(stream));
        }

        ulong hash = Fnv1a64(stream.Value);
        return Pcg32.Seed(initialState: SplitMix64(masterSeed ^ hash), sequence: SplitMix64(hash));
    }

    internal static ulong Fnv1a64(string text)
    {
        unchecked
        {
            ulong hash = 14695981039346656037UL;
            foreach (byte b in Encoding.UTF8.GetBytes(text))
            {
                hash = (hash ^ b) * 1099511628211UL;
            }

            return hash;
        }
    }

    private static ulong SplitMix64(ulong value)
    {
        unchecked
        {
            value += 0x9E3779B97F4A7C15UL;
            value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
            value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
            return value ^ (value >> 31);
        }
    }
}
