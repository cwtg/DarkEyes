using DarkEyes.Backend.Abstractions;

namespace DarkEyes.Backend.Tests
{
    public sealed class GameBackendBuilderTests
    {
        [Fact]
        public void Should_WireUpGameOrchestrator()
        {
            // Arrange
            bool handlerWasExecuted = false;
            GameBackendBuilder builder = new();

            // Act
            builder.RegisterHandler<TestCommand>((cmd, ctx) =>
            {
                Assert.NotNull(ctx.Store);
                handlerWasExecuted = true;
            });
            var orchestrator = builder.Build();
            orchestrator.Dispatch(new TestCommand());

            // Assert
            Assert.True(handlerWasExecuted);
        }
    }
}

file sealed class TestCommand : ICommand { }

file sealed class TestEvent : IGameEvent { }