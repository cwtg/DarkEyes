using System;
using DarkEyes.Backend.Randomness;

namespace DarkEyes.Backend.Tests.Unit.Randomness;

public sealed class StreamSeedsTests
{
    [Fact]
    public void Should_MatchTheFnv1aReferenceVectors()
    {
        // Assert: published FNV-1a 64-bit test vectors
        Assert.Equal(0xcbf29ce484222325UL, StreamSeeds.Fnv1a64(string.Empty));
        Assert.Equal(0xaf63dc4c8601ec8cUL, StreamSeeds.Fnv1a64("a"));
    }

    [Fact]
    public void Should_DeriveTheSameStreamAsAlways_When_SeedAndIdAreGiven()
    {
        // Arrange: pinned output; if this changes, every new game and every stream derived later from old saves changes too
        Pcg32State state = StreamSeeds.Derive(masterSeed: 42, RandomStreamIds.Rules);

        // Act
        (uint first, Pcg32State next) = Pcg32.Next(state);
        (uint second, _) = Pcg32.Next(next);

        // Assert
        Assert.Equal((0xdd40559au, 0x12d2a743u), (first, second));
    }

    [Fact]
    public void Should_DeriveDifferentStreams_When_IdsDiffer()
    {
        // Act
        Pcg32State rules = StreamSeeds.Derive(masterSeed: 42, RandomStreamIds.Rules);
        Pcg32State other = StreamSeeds.Derive(masterSeed: 42, new RandomStreamId("test.other"));

        // Assert
        Assert.NotEqual(rules.State, other.State);
        Assert.NotEqual(rules.Increment, other.Increment);
    }

    [Fact]
    public void Should_Throw_When_TheStreamHasNoId()
    {
        // Act
        static void Act() => StreamSeeds.Derive(masterSeed: 42, default);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}
