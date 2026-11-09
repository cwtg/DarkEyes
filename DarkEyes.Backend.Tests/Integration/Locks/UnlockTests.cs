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
        DecreeAction decree = new(new UnlockEffect(TestIds.CellarChest, TestIds.Alrik));
        DispatchResult result = game.Dispatch(decree);

        // Assert
        // Assert: the effect emitted nothing; only the decreeing test action reports its outcome, without patches
        Step step = Assert.Single(Assert.IsType<Committed>(result).Commit.Steps);
        Assert.Equal(new ActionCompleted(decree, ActOutcome.Succeeded), step.Event);
        Assert.Empty(step.Patches);
        mirror.AssertMatches(game);
    }
}
