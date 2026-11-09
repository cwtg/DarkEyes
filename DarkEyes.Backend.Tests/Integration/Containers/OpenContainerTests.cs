using System;
using DarkEyes.Backend.Containers;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Tests.Support;

namespace DarkEyes.Backend.Tests.Integration.Containers;

public sealed class OpenContainerTests
{
    [Fact]
    public void Should_OpenChest_When_HeroIsAdjacentAndChestIsUnlocked()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0))
            .Build();
        var mirror = TestMirror.Connect(game);

        // Act
        OpenContainerAction open = new(TestIds.Alrik, TestIds.CellarChest);
        DispatchResult result = game.Dispatch(open);

        // Assert: the state change, then the act's outcome as the last step
        CommitResult commit = Assert.IsType<Committed>(result).Commit;
        mirror.Apply(commit);
        Assert.Equal(2, commit.Steps.Length);
        Assert.IsType<ContainerOpened>(commit.Steps[0].Event);
        Assert.Equal(new ActionCompleted(open, ActOutcome.Succeeded), commit.Steps[1].Event);
        Assert.Empty(commit.Steps[1].Patches);
        Assert.True(mirror.Get(TestIds.CellarChest, ContainerState.Key).IsOpen);
        mirror.AssertMatches(game);
    }

    [Fact]
    public void Should_RejectWithTooFarAway_When_ChestIsNotAdjacent()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(3, 0, 0))
            .Build();
        var mirror = TestMirror.Connect(game);
        GameState before = game.State;

        // Act
        DispatchResult result = game.Dispatch(new OpenContainerAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Reason rejection = Assert.IsType<Rejected>(result).Reason;
        Assert.Equal(OpenContainerRejections.TooFarAway(distance: 3, maxDistance: 1), rejection);
        Assert.Same(before, game.State);
        mirror.AssertMatches(game);
    }

    [Fact]
    public void Should_RejectWithAlreadyOpen_When_ChestIsAlreadyOpen()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0), isOpen: true)
            .Build();
        var mirror = TestMirror.Connect(game);
        GameState before = game.State;

        // Act
        DispatchResult result = game.Dispatch(new OpenContainerAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Reason rejection = Assert.IsType<Rejected>(result).Reason;
        Assert.Equal(OpenContainerRejections.AlreadyOpen(), rejection);
        Assert.Same(before, game.State);
        mirror.AssertMatches(game);
    }

    [Fact]
    public void Should_RejectWithLocked_When_ChestIsLocked()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0), chestLock: ChestLock.Locked)
            .Build();
        var mirror = TestMirror.Connect(game);
        GameState before = game.State;

        // Act
        DispatchResult result = game.Dispatch(new OpenContainerAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Reason rejection = Assert.IsType<Rejected>(result).Reason;
        Assert.Equal(OpenContainerRejections.Locked(), rejection);
        Assert.Same(before, game.State);
        mirror.AssertMatches(game);
    }

    [Fact]
    public void Should_RejectWithTooFarAway_When_LockedChestIsNotAdjacent()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(3, 0, 0), chestLock: ChestLock.Locked)
            .Build();

        // Act
        DispatchResult result = game.Dispatch(new OpenContainerAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Reason rejection = Assert.IsType<Rejected>(result).Reason;
        Assert.Equal(OpenContainerRejections.TooFarAway(distance: 3, maxDistance: 1), rejection);
    }

    [Fact]
    public void Should_DoNothing_When_OpeningIsDecreedForAnOpenContainer()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithChest(TestIds.CellarChest, at: new GamePosition(0, 0, 0), isOpen: true)
            .Build();
        var mirror = TestMirror.Connect(game);

        // Act
        DecreeAction decree = new(new OpenContainerEffect(TestIds.CellarChest, TestIds.Alrik));
        DispatchResult result = game.Dispatch(decree);

        // Assert
        // Assert: the effect emitted nothing; only the decreeing test action reports its outcome, without patches
        Step step = Assert.Single(Assert.IsType<Committed>(result).Commit.Steps);
        Assert.Equal(new ActionCompleted(decree, ActOutcome.Succeeded), step.Event);
        Assert.Empty(step.Patches);
        mirror.AssertMatches(game);
    }

    [Fact]
    public void Should_Throw_When_TargetIsNotAContainer()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithHero(TestIds.Brin, at: new GamePosition(1, 0, 0))
            .Build();
        GameState before = game.State;

        // Act
        void Act() => game.Dispatch(new OpenContainerAction(TestIds.Alrik, TestIds.Brin));

        // Assert
        Assert.Throws<ArgumentException>(Act);
        Assert.Same(before, game.State);
    }

    [Fact]
    public void Should_KeepMirrorInSync_When_ARejectedActionIsFollowedByASuccessfulOne()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithHero(TestIds.Alrik, at: new GamePosition(0, 0, 0))
            .WithChest(TestIds.CellarChest, at: new GamePosition(1, 0, 0))
            .WithChest(TestIds.AtticChest, at: new GamePosition(3, 0, 0))
            .Build();
        var mirror = TestMirror.Connect(game);

        // Act
        DispatchResult rejected = game.Dispatch(new OpenContainerAction(TestIds.Alrik, TestIds.AtticChest));
        DispatchResult committed = game.Dispatch(new OpenContainerAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Assert.IsType<Rejected>(rejected);
        mirror.Apply(Assert.IsType<Committed>(committed).Commit);
        Assert.True(mirror.Get(TestIds.CellarChest, ContainerState.Key).IsOpen);
        Assert.False(mirror.Get(TestIds.AtticChest, ContainerState.Key).IsOpen);
        mirror.AssertMatches(game);
    }
}
