using DarkEyes.Backend.Containers;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
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
        DispatchResult result = game.Dispatch(new OpenContainerAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        CommitResult commit = Assert.IsType<Committed>(result).Commit;
        mirror.Apply(commit);
        Assert.IsType<ContainerOpened>(Assert.Single(commit.Steps).Event);
        Assert.True(mirror.Get(TestIds.CellarChest, ContainerState.Key).IsOpen);
    }
}
