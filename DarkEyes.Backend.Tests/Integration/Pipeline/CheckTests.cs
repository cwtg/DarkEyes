using System;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.State;
using DarkEyes.Backend.Tests.Support;

namespace DarkEyes.Backend.Tests.Integration.Pipeline;

public sealed class CheckTests
{
    private static readonly CheckPurpose s_trap = new("test.trap");

    [Fact]
    public void Should_RunCheck_When_AnEffectRequestsIt()
    {
        // Arrange
        ScriptedChecks checks = new(succeed: true);
        GameStore game = new GameStoreBuilder()
            .UseChecks(checks)
            .AddActionHandler(new DecreeHandler())
            .AddEffectHandler(new CheckingEffectHandler())
            .Build(GameState.Empty);

        // Act
        game.Dispatch(new DecreeAction(new CheckingEffect(TestIds.Alrik)));

        // Assert
        Assert.Equal(new CheckRequest(s_trap, TestIds.Alrik, Target: null), Assert.Single(checks.Requests));
    }

    [Fact]
    public void Should_Throw_When_ChecksAreInstalledTwice()
    {
        // Arrange
        GameStoreBuilder builder = new GameStoreBuilder().UseChecks(new ScriptedChecks(succeed: true));

        // Act
        void Act() => builder.UseChecks(new ScriptedChecks(succeed: true));

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Should_Throw_When_ChecksAreInstalledAfterBuild()
    {
        // Arrange
        GameStoreBuilder builder = new();
        builder.Build(GameState.Empty);

        // Act
        void Act() => builder.UseChecks(new ScriptedChecks(succeed: true));

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Should_Throw_When_ARequestHasNoPurpose()
    {
        // Act
        static void Act() => _ = new CheckRequest(default, TestIds.Alrik, Target: null);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    private sealed record CheckingEffect(EntityId Victim) : IEffect;

    private sealed class CheckingEffectHandler : IEffectHandler<CheckingEffect>
    {
        public void Handle(CheckingEffect effect, EffectContext context)
        {
            context.Check(new CheckRequest(s_trap, effect.Victim, Target: null));
        }
    }
}
