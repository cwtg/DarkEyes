using DarkEyes.Backend.Locks;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.Tests.Support;

namespace DarkEyes.Backend.Tests.Integration.Locks;

public sealed class UnlockTests
{
    [Fact]
    public void Should_DoNothing_When_UnlockIsDecreedForAnUnlockedLock()
    {
        // Arrange
        GameStore game = new ScenarioBuilder()
            .WithChest(TestIds.CellarChest, at: new GamePosition(0, 0, 0), chestLock: ChestLock.Unlocked)
            .Build();
        var mirror = TestMirror.Connect(game);

        // Act
        DispatchResult result = game.Dispatch(new DecreeAction(new UnlockEffect(TestIds.CellarChest, TestIds.Alrik)));

        // Assert
        Assert.Empty(Assert.IsType<Committed>(result).Commit.Steps);
        mirror.AssertMatches(game);
    }
}
