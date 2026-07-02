using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Rx;
using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Tests
{
    public class GameOrchestratorTests
    {
        [Fact]
        public void Should_EmitEvent_When_DispatchCommand()
        {
            // Arrange
            CommandRouter router = new();
            router.Register<TestCommand>((cmd, ctx) => ctx.Emit(new TestEvent()));
            GameOrchestrator orchestrator = new(router, new NoopLogger());

            List<IGameEvent> emittedEvents = [];
            using var subscription = orchestrator.Events.Subscribe(e => emittedEvents.Add(e));

            // Act
            orchestrator.Dispatch(new TestCommand());

            // Assert
            Assert.Single(emittedEvents);
            Assert.IsType<TestEvent>(emittedEvents[0]);
        }

        [Fact]
        public void Should_EmitErrorRaised_When_DispatchCommandWithoutHandler()
        {
            // Arrange
            CommandRouter router = new();
            GameOrchestrator orchestrator = new(router, new NoopLogger());

            List<IGameEvent> emittedEvents = [];
            using var subscription = orchestrator.Events.Subscribe(e => emittedEvents.Add(e));

            // Act
            orchestrator.Dispatch(new TestCommand());

            // Assert
            Assert.Single(emittedEvents);
            Assert.IsType<ErrorRaisedEvent>(emittedEvents[0]);
        }

        [Fact]
        public void Should_ThrowException_When_DispatchReentrantCommand()
        {
            // Arrange
            CommandRouter router = new();
            router.Register<TestCommand>((cmd, ctx) => cmd.Dispatcher.Dispatch(cmd));
            GameOrchestrator orchestrator = new(router, new NoopLogger());

            List<IGameEvent> emittedEvents = [];
            using var subscription = orchestrator.Events.Subscribe(e => emittedEvents.Add(e));

            // Act
            Assert.Throws<InvalidOperationException>(() => orchestrator.Dispatch(new TestCommand(orchestrator)));

            // Assert
            Assert.Empty(emittedEvents);
        }
    }
}

file sealed class TestCommand(IGameOrchestrator? dispatcher = null) : ICommand {
    public IGameOrchestrator Dispatcher = dispatcher;
}

file sealed class TestEvent : IGameEvent { }
