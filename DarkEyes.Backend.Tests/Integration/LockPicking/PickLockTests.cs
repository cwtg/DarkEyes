using System;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.Containers;
using DarkEyes.Backend.LockPicking;
using DarkEyes.Backend.Locks;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Tests.Support;

namespace DarkEyes.Backend.Tests.Integration.LockPicking;

public sealed class PickLockTests
{
    [Fact]
    public void Should_UnlockButNotOpenChest_When_LockPickingSucceeds()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0), chestLock: ChestLock.Locked)
            .WithChecks(new ScriptedChecks(succeed: true))
            .Build();
        var mirror = TestMirror.Connect(game);

        // Act
        DispatchResult result = game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        CommitResult commit = Assert.IsType<Committed>(result).Commit;
        mirror.Apply(commit);
        Assert.Equal(new LockUnlocked(TestIds.CellarChest, TestIds.Alrik), Assert.Single(commit.Steps).Event);
        Assert.False(mirror.Get(TestIds.CellarChest, Lock.Key).IsLocked);
        Assert.False(mirror.Get(TestIds.CellarChest, ContainerState.Key).IsOpen);
        mirror.AssertMatches(game);
    }

    [Fact]
    public void Should_RequestAPickLockCheckForActorAndTarget_When_PickingALock()
    {
        // Arrange
        ScriptedChecks checks = new(succeed: true);
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0), chestLock: ChestLock.Locked)
            .WithChecks(checks)
            .Build();

        // Act
        game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Assert.Equal(new CheckRequest(CheckPurposes.PickLock, TestIds.Alrik, TestIds.CellarChest), Assert.Single(checks.Requests));
    }

    [Fact]
    public void Should_RejectWithNotLocked_When_LockIsUnlocked()
    {
        // Arrange
        ScriptedChecks checks = new(succeed: true);
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0), chestLock: ChestLock.Unlocked)
            .WithChecks(checks)
            .Build();
        GameState before = game.State;

        // Act
        DispatchResult result = game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Assert.Equal(PickLockRejections.NotLocked(), Assert.IsType<Rejected>(result).Reason);
        Assert.Same(before, game.State);
        Assert.Empty(checks.Requests);
    }

    [Fact]
    public void Should_RejectWithTooFarAway_When_UnlockedLockIsNotAdjacent()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(3, 0, 0), chestLock: ChestLock.Unlocked)
            .WithChecks(new ScriptedChecks(succeed: true))
            .Build();

        // Act
        DispatchResult result = game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Assert.Equal(PickLockRejections.TooFarAway(distance: 3, maxDistance: 1), Assert.IsType<Rejected>(result).Reason);
    }

    [Fact]
    public void Should_Throw_When_NoCheckServiceIsInstalled()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0), chestLock: ChestLock.Locked)
            .Build();

        // Act
        void Act() => game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Should_RejectWithTooFarAway_When_LockIsNotAdjacent()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(3, 0, 0), chestLock: ChestLock.Locked)
            .WithChecks(new ScriptedChecks(succeed: true))
            .Build();
        GameState before = game.State;

        // Act
        DispatchResult result = game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Assert.Equal(PickLockRejections.TooFarAway(distance: 3, maxDistance: 1), Assert.IsType<Rejected>(result).Reason);
        Assert.Same(before, game.State);
    }

    [Fact]
    public void Should_Throw_When_TargetHasNoLock()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0))
            .WithChecks(new ScriptedChecks(succeed: true))
            .Build();

        // Act
        void Act() => game.Dispatch(new PickLockAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}
