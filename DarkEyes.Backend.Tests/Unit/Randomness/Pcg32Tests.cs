using System;
using System.Collections.Generic;
using DarkEyes.Backend.Randomness;

namespace DarkEyes.Backend.Tests.Unit.Randomness;

public sealed class Pcg32Tests
{
    [Fact]
    public void Should_MatchReferenceOutput_When_SeededLikeTheOfficialDemo()
    {
        // Arrange: pcg32_srandom_r(&rng, 42u, 54u) from the PCG reference implementation (pcg-c-basic, pcg32-demo)
        Pcg32State state = Pcg32.Seed(initialState: 42, sequence: 54);
        List<uint> values = [];

        // Act
        for (int i = 0; i < 6; i++)
        {
            (uint value, Pcg32State next) = Pcg32.Next(state);
            values.Add(value);
            state = next;
        }

        // Assert
        Assert.Equal([0xa15c02b7u, 0x7b47f409u, 0xba1d3330u, 0x83d2f293u, 0xbfa4784bu, 0xcbed606eu], values);
    }

    [Fact]
    public void Should_StayWithinBounds_When_DrawingBoundedValues()
    {
        // Arrange
        Pcg32State state = Pcg32.Seed(initialState: 7, sequence: 1);

        // Act + Assert
        for (int i = 0; i < 1000; i++)
        {
            (uint value, Pcg32State next) = Pcg32.NextBounded(state, bound: 20);
            Assert.InRange(value, 0u, 19u);
            state = next;
        }
    }

    [Fact]
    public void Should_RejectValuesBelowTheThreshold_When_TheBoundDoesNotDivideTheRange()
    {
        // Arrange: for this bound the threshold is 2^31 - 1, so about half of all raw values must be rejected
        const uint Bound = 0x80000001u;
        const uint Threshold = 0x7FFFFFFFu;
        Pcg32State state = Pcg32.Seed(initialState: 3, sequence: 9);
        int rejections = 0;

        // Act + Assert: the bounded value is the first raw value at or above the threshold, modulo the bound
        for (int i = 0; i < 200; i++)
        {
            (uint raw, Pcg32State afterRaw) = Pcg32.Next(state);
            (uint bounded, Pcg32State afterBounded) = Pcg32.NextBounded(state, Bound);
            if (raw >= Threshold)
            {
                Assert.Equal(raw % Bound, bounded);
                Assert.Equal(afterRaw, afterBounded);
            }
            else
            {
                rejections++;
                (uint retry, Pcg32State afterRetry) = Pcg32.NextBounded(afterRaw, Bound);
                Assert.Equal(retry, bounded);
                Assert.Equal(afterRetry, afterBounded);
            }

            state = afterRaw;
        }

        Assert.InRange(rejections, 50, 150);
    }

    [Fact]
    public void Should_Throw_When_TheIncrementIsEven()
    {
        // Act
        static void Act() => _ = new Pcg32State(State: 1, Increment: 2);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}
