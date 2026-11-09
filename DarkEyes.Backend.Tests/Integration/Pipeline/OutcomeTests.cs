using System;
using DarkEyes.Backend.Containers;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Tests.Support;

namespace DarkEyes.Backend.Tests.Integration.Pipeline;

public sealed class OutcomeTests
{
    [Fact]
    public void Should_Throw_When_AnEffectChangesStateWithoutAnEvent()
    {
        // Arrange: the outcome event must not silently take over an unexplained change
        GameStore game = new GameStoreBuilder()
            .AddActionHandler(new DecreeHandler())
            .AddEffectHandler(new SilentOpenHandler())
            .Build(ChestState());

        // Act
        void Act() => game.Dispatch(new DecreeAction(new SilentOpen(TestIds.CellarChest)));

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Should_ReportTheOutcomeLast_When_TheHandlerEmitsAfterItsEffects()
    {
        // Arrange
        GameStore game = new GameStoreBuilder()
            .AddContainers()
            .AddActionHandler(new OpenThenEmitHandler())
            .Build(ChestState());
        OpenThenEmit action = new(TestIds.Alrik, TestIds.CellarChest);

        // Act
        CommitResult commit = Assert.IsType<Committed>(game.Dispatch(action)).Commit;

        // Assert
        Assert.Equal(3, commit.Steps.Length);
        Assert.IsType<Noted>(commit.Steps[1].Event);
        Assert.Equal(new ActionCompleted(action, ActOutcome.Succeeded), commit.Steps[2].Event);
        Assert.Empty(commit.Steps[2].Patches);
    }

    [Fact]
    public void Should_Throw_When_AHandlerReturnsNoResult()
    {
        // Arrange
        GameStore game = new GameStoreBuilder()
            .AddActionHandler(new NullHandler())
            .Build(GameState.Empty);

        // Act
        void Act() => game.Dispatch(new NullAction(TestIds.Alrik));

        // Assert
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(Act);
        Assert.Contains(nameof(NullAction), exception.Message, StringComparison.Ordinal);
    }

    private static GameState ChestState() => GameState.Empty.WithEntity(Entity.Create(TestIds.CellarChest)
        .With(Placement.Key, new Placement(new GamePosition(0, 0, 0)))
        .With(ContainerState.Key, new ContainerState(IsOpen: false)));

    private sealed record SilentOpen(EntityId Container) : IEffect;

    private sealed class SilentOpenHandler : IEffectHandler<SilentOpen>
    {
        public void Handle(SilentOpen effect, EffectContext context)
        {
            context.SetComponent(effect.Container, ContainerState.Key, new ContainerState(IsOpen: true));
        }
    }

    private sealed record OpenThenEmit(EntityId Actor, EntityId Container) : IAction;

    private sealed record Noted(EntityId Container) : IEvent;

    private sealed class OpenThenEmitHandler : IActionHandler<OpenThenEmit>
    {
        public ActResult Handle(OpenThenEmit action, ActionContext context)
        {
            context.Submit(new OpenContainerEffect(action.Container, action.Actor));
            context.Emit(new Noted(action.Container));
            return ActResult.Succeeded;
        }
    }

    private sealed record NullAction(EntityId Actor) : IAction;

    private sealed class NullHandler : IActionHandler<NullAction>
    {
        public ActResult Handle(NullAction action, ActionContext context) => null!;
    }
}
