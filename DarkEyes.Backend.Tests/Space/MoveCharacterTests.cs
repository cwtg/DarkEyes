using DarkEyes.Backend.Abstractions;
using DarkEyes.Backend.Core;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Rx;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.Space.Commands;
using DarkEyes.Backend.Space.Events;
using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Tests.Space
{
    public sealed class MoveCharacterTests
    {
        [Fact]
        public void Should_EmitCharacterStepped_When_DispatchMoveCharacter()
        {
            // Arrange
            EntityId id = new("mover");
            GamePosition startPosition = new(0, 0, 0);
            GamePosition endPosition = new(1, 1, 1);

            CommandRouter router = new();
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
            GameStore store = new();
            store.World.EntityPositions = new()
            {
                { id, startPosition }
            };
            GameOrchestrator orchestrator = new(router, store, new NoopLogger());

            List<IGameEvent> emittedEvents = [];
            using var subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new MoveCharacterCommand(id, endPosition));

            // Assert
            Assert.Single(emittedEvents);
            Assert.IsType<CharacterSteppedEvent>(emittedEvents[0]);
            var emitted = (CharacterSteppedEvent) emittedEvents[0];
            Assert.Equal(emitted.Id, id);
            Assert.Equal(emitted.From, startPosition);
            Assert.Equal(emitted.To, endPosition);
        }

        [Fact]
        public void Should_EmitErrorRaise_When_DispatchMoveCharacterWithNoMoverRegistered()
        {
            // Arrange
            EntityId id = new("mover");
            GamePosition startPosition = new(0, 0, 0);
            GamePosition endPosition = new(1, 1, 1);

            CommandRouter router = new();
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
            GameOrchestrator orchestrator = new(router, new(), new NoopLogger());

            List<IGameEvent> emittedEvents = [];
            using var subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new MoveCharacterCommand(id, endPosition));

            // Assert
            Assert.Single(emittedEvents);
            Assert.IsType<ErrorRaisedEvent>(emittedEvents[0]);
        }
    }
}
