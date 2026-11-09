using System;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.Containers;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Tests.Support;

namespace DarkEyes.Backend.Tests.Integration.Pipeline;

public sealed class RejectionTests
{
    [Fact]
    public void Should_Throw_When_HandlerRejectsAfterSubmittingAnEffect()
    {
        // Arrange
        GameState state = GameState.Empty.WithEntity(Entity.Create(TestIds.CellarChest)
            .With(Placement.Key, new Placement(new GamePosition(0, 0, 0)))
            .With(ContainerState.Key, new ContainerState(IsOpen: false)));
        GameStore game = new GameStoreBuilder()
            .AddContainers()
            .AddActionHandler(new SubmitThenRejectHandler())
            .Build(state);

        // Act
        void Act() => game.Dispatch(new SubmitThenRejectAction(TestIds.Alrik, TestIds.CellarChest));

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        Assert.Same(state, game.State);
    }

    [Fact]
    public void Should_Throw_When_HandlerRejectsAfterACheck()
    {
        // Arrange
        GameStore game = new GameStoreBuilder()
            .UseChecks(new ScriptedChecks(succeed: false))
            .AddActionHandler(new CheckThenRejectHandler())
            .Build(GameState.Empty);

        // Act
        void Act() => game.Dispatch(new CheckThenRejectAction(TestIds.Alrik));

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Should_Throw_When_ARejectionArgumentIsNotASimpleValue()
    {
        // Act
        static void Act() => Reason.Create("test.float_argument", ("distance", 1.5));

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    private sealed record CheckThenRejectAction(EntityId Actor) : IAction;

    private sealed class CheckThenRejectHandler : IActionHandler<CheckThenRejectAction>
    {
        public Reason? Handle(CheckThenRejectAction action, ActionContext context)
        {
            context.Check(new CheckRequest(new CheckPurpose("test.check"), action.Actor, Target: null));
            return Reason.Create("test.too_late");
        }
    }

    private sealed record SubmitThenRejectAction(EntityId Actor, EntityId Container) : IAction;

    private sealed class SubmitThenRejectHandler : IActionHandler<SubmitThenRejectAction>
    {
        public Reason? Handle(SubmitThenRejectAction action, ActionContext context)
        {
            context.Submit(new OpenContainerEffect(action.Container, action.Actor));
            return Reason.Create("test.too_late");
        }
    }
}
