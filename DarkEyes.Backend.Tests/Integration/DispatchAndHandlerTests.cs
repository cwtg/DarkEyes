using System;
using System.Collections.Generic;
using DarkEyes.Backend.Commands;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Rx;

namespace DarkEyes.Backend.Tests.Integration
{
    public sealed class DispatchAndHandlerTests
    {
        [Fact]
        public void Should_EmitEvent_When_DispatchCommand()
        {
            // Arrange
            CommandRouter router = new();
            router.Register<TestCommand>((cmd, ctx) => ctx.Emit(new TestEvent()));
            GameOrchestrator orchestrator = new(router, new(), new(), new NoopLogger());

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

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
            GameOrchestrator orchestrator = new(router, new(), new(), new NoopLogger());

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

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
            GameOrchestrator orchestrator = new(router, new(), new(), new NoopLogger());

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            Assert.Throws<InvalidOperationException>(() => orchestrator.Dispatch(new TestCommand(orchestrator)));

            // Assert
            Assert.Empty(emittedEvents);
        }
    }

    sealed file class TestCommand(IGameOrchestrator? dispatcher = null) : ICommand
    {
        public IGameOrchestrator Dispatcher = dispatcher;
    }

    sealed file class TestEvent : IGameEvent { }
}
