using System;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.LockPicking;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Randomness;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Tests.Support;

namespace DarkEyes.Backend.Tests.Integration.Pipeline;

public sealed class RandomTests
{
    [Fact]
    public void Should_RollTheSameNumbers_When_SeedAndInputsAreTheSame()
    {
        // Arrange
        RollingChecks first = new(rollsPerCheck: 4);
        RollingChecks second = new(rollsPerCheck: 4);

        // Act
        PickCellarChest(seed: 1234, first);
        PickCellarChest(seed: 1234, second);

        // Assert
        Assert.Equal(first.Rolls, second.Rolls);
    }

    [Fact]
    public void Should_RollDifferentNumbers_When_SeedsDiffer()
    {
        // Arrange
        RollingChecks first = new(rollsPerCheck: 4);
        RollingChecks second = new(rollsPerCheck: 4);

        // Act
        PickCellarChest(seed: 1234, first);
        PickCellarChest(seed: 4321, second);

        // Assert
        Assert.NotEqual(first.Rolls, second.Rolls);
    }

    [Fact]
    public void Should_ContinueTheStream_When_TheNextCommitRollsAgain()
    {
        // Arrange
        RollingChecks checks = new();
        GameStore game = new ScenarioBuilder()
            .WithSeed(99)
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0), chestLock: ChestLock.Locked)
            .WithChest(TestIds.AtticChest, at: new GamePosition(-1, 0, 0), chestLock: ChestLock.Locked)
            .WithChecks(checks)
            .Build();

        // Act
        game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.CellarChest));
        game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.AtticChest));

        // Assert: the two d20s are the first two values of the rules stream, in order
        Pcg32State stream = GameState.Empty.WithMasterSeed(99).GetRandomStream(RandomStreamIds.Rules);
        (uint first, Pcg32State afterFirst) = Pcg32.NextBounded(stream, bound: 20);
        (uint second, _) = Pcg32.NextBounded(afterFirst, bound: 20);
        Assert.Equal([(int)first + 1, (int)second + 1], checks.Rolls);
    }

    [Fact]
    public void Should_LeaveTheStreamUnchanged_When_AChainIsDiscarded()
    {
        // Arrange
        RollingChecks discarded = new();
        RollingChecks fresh = new();
        GameStore game = new GameStoreBuilder()
            .UseChecks(discarded)
            .AddActionHandler(new RollThenFailHandler())
            .Build(GameState.Empty.WithMasterSeed(5));

        // Act
        Assert.Throws<InvalidOperationException>(() => game.Dispatch(new RollThenFailAction(TestIds.Alrik)));
        Assert.Throws<InvalidOperationException>(() => game.Dispatch(new RollThenFailAction(TestIds.Alrik)));
        PickCellarChest(seed: 5, fresh);

        // Assert
        Assert.Equal(discarded.Rolls[0], discarded.Rolls[1]);
        Assert.Equal(fresh.Rolls[0], discarded.Rolls[0]);
    }

    [Fact]
    public void Should_NotShiftOtherStreams_When_TheRulesStreamIsUsed()
    {
        // Arrange
        RandomStreamId other = new("test.other");
        RollingChecks checks = new(rollsPerCheck: 3);
        GameStore game = PickCellarChest(seed: 77, checks);

        // Act
        Pcg32State afterRules = game.State.GetRandomStream(other);
        Pcg32State untouched = GameState.Empty.WithMasterSeed(77).GetRandomStream(other);

        // Assert
        Assert.Equal(untouched, afterRules);
        Assert.NotEqual(GameState.Empty.WithMasterSeed(77).GetRandomStream(RandomStreamIds.Rules), game.State.GetRandomStream(RandomStreamIds.Rules));
    }

    [Fact]
    public void Should_Throw_When_TheMasterSeedChangesAfterAStreamWasUsed()
    {
        // Arrange
        GameState used = PickCellarChest(seed: 1, new RollingChecks()).State;

        // Act
        void Act() => used.WithMasterSeed(2);

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Should_Throw_When_ACheckRollsWithoutAMasterSeed()
    {
        // Arrange
        GameStore game = new GameStoreBuilder()
            .UseChecks(new RollingChecks())
            .AddActionHandler(new RollThenFailHandler())
            .Build(GameState.Empty);

        // Act
        void Act() => game.Dispatch(new RollThenFailAction(TestIds.Alrik));

        // Assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(Act);
        Assert.Contains("master seed", exception.Message, StringComparison.Ordinal);
    }

    private static GameStore PickCellarChest(ulong seed, IChecks checks)
    {
        GameStore game = new ScenarioBuilder()
            .WithSeed(seed)
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0), chestLock: ChestLock.Locked)
            .WithChecks(checks)
            .Build();
        game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.CellarChest));
        return game;
    }

    private sealed record RollThenFailAction(EntityId Actor) : IAction;

    private sealed class RollThenFailHandler : IActionHandler<RollThenFailAction>
    {
        public Rejection? Handle(RollThenFailAction action, ActionContext context)
        {
            context.Check(new CheckRequest(CheckPurposes.PickLock, action.Actor, Target: null));
            throw new InvalidOperationException("A bug after the roll: the chain is discarded.");
        }
    }
}
