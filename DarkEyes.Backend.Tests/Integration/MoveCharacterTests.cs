using System;
using System.Collections.Generic;
using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Handlers;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Orchestration;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.Store;

namespace DarkEyes.Backend.Tests.Integration
{
    public sealed class MoveCharacterTests
    {
        [Fact]
        public void Should_EmitCharacterStepped_When_DispatchMoveCharacter()
        {
            // Arrange
            EntityId id = new("mover");
            GamePosition startPosition = new(0, 0, 0);
            GamePosition endPosition = new(1, 0, 0);

            CommandRouter router = new();
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(id, startPosition, new(), null));
            store.ActiveLocation.Entities.Add(new Entity(new("entity2"), new(2, 2, 0), new(), null));
            store.ActiveLocation.Entities.Add(new Entity(new("other"), new(-1, 0, 1), new(), null));
            IReadOnlyList<IReadOnlyList<bool>> walkability =
            [
                [ true, true ],
                [ true, true ]
            ];
            IReadOnlyList<IReadOnlyList<int>> cost =
            [
                [ 1, 1 ],
                [ 1, 1 ]
            ];
            NavigationMap map = new();
            map.Set(walkability, cost);
            store.ActiveLocation.NavigationMap = map;
            GameOrchestrator orchestrator = new(router, store, (new Random() as IRandomSource)!, new NullLogger());

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new MoveCharacterCommand(id, endPosition, null));

            // Assert
            Assert.Single(emittedEvents);
            Assert.IsType<CharacterSteppedEvent>(emittedEvents[0]);
            var emitted = (CharacterSteppedEvent)emittedEvents[0];
            Assert.Equal(id, emitted.Id);
            Assert.Equal(startPosition, emitted.From);
            Assert.Equal(endPosition, emitted.To);
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
            GameOrchestrator orchestrator = new(router, new(), (new Random() as IRandomSource)!, new NullLogger());

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new MoveCharacterCommand(id, endPosition, null));

            // Assert
            Assert.Single(emittedEvents);
            Assert.IsType<ErrorRaisedEvent>(emittedEvents[0]);
        }
    }
}
