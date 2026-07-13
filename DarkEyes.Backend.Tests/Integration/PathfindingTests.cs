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
    public sealed class PathfindingTests
    {
        [Fact]
        public void Should_MoveCharacterAlongPath()
        {
            // Arrange
            EntityId id = new("mover");
            GamePosition startPosition = new(0, 0, 0);
            GamePosition endPosition = new(4, 0, 0);

            CommandRouter router = new();
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(id, startPosition, new(), null));

            IReadOnlyList<IReadOnlyList<bool>> walkability =
            [
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ]
            ];
            IReadOnlyList<IReadOnlyList<int>> cost =
            [
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
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
            Assert.Equal(4, emittedEvents.Count);
            GamePosition from = startPosition;
            foreach (IGameEvent ev in emittedEvents)
            {
                Assert.IsType<CharacterSteppedEvent>(ev);
                var emitted = (CharacterSteppedEvent)ev;
                Assert.Equal(id, emitted.Id);
                Assert.Equal(from, emitted.From);
                from = emitted.To;
            }
            Assert.Equal(endPosition, from);
        }

        [Fact]
        public void Should_RaiseError_When_PathIsBlocked()
        {
            // Arrange
            EntityId id = new("mover");
            GamePosition startPosition = new(0, 0, 0);
            GamePosition endPosition = new(4, 0, 0);

            CommandRouter router = new();
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(id, startPosition, new(), null));

            IReadOnlyList<IReadOnlyList<bool>> walkability =
            [
                [ true, true, false, true, true ],
                [ true, true, false, true, true ],
                [ false, false, false, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ]
            ];
            IReadOnlyList<IReadOnlyList<int>> cost =
            [
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
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
            Assert.IsType<ErrorRaisedEvent>(emittedEvents[0]);
        }

        [Fact]
        public void Should_DoNothing_When_TargetIsStartingPosition()
        {
            // Arrange
            EntityId id = new("mover");
            GamePosition startPosition = new(0, 0, 0);
            GamePosition endPosition = new(0, 0, 0);

            CommandRouter router = new();
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(id, startPosition, new(), null));

            IReadOnlyList<IReadOnlyList<bool>> walkability =
            [
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ]
            ];
            IReadOnlyList<IReadOnlyList<int>> cost =
            [
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
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
            Assert.Empty(emittedEvents);
            Assert.Equal(endPosition, store.ActiveLocation.Entities.Get(id).Position);
        }

        [Fact]
        public void Should_FollowPreferredPath()
        {
            // Arrange
            EntityId id = new("mover");
            GamePosition startPosition = new(0, 0, 0);
            GamePosition endPosition = new(4, 0, 0);

            CommandRouter router = new();
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(id, startPosition, new(), null));

            IReadOnlyList<IReadOnlyList<bool>> walkability =
            [
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ]
            ];
            IReadOnlyList<IReadOnlyList<int>> cost =
            [
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
            ];
            NavigationMap map = new();
            map.Set(walkability, cost);
            store.ActiveLocation.NavigationMap = map;
            GameOrchestrator orchestrator = new(router, store, (new Random() as IRandomSource)!, new NullLogger());

            List<GamePosition> preferredPath =
            [
                new(0, 1, 0),
                new(1, 1, 0),
                new(2, 1, 0),
                new(3, 0, 0),
                new(4, 0, 0),
            ];

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new MoveCharacterCommand(id, endPosition, preferredPath));

            // Assert
            Assert.Equal(5, emittedEvents.Count);
            GamePosition from = startPosition;
            for (int i = 0; i < 5; i++)
            {
                Assert.IsType<CharacterSteppedEvent>(emittedEvents[i]);
                var emitted = (CharacterSteppedEvent)emittedEvents[i];
                Assert.Equal(id, emitted.Id);
                Assert.Equal(from, emitted.From);
                Assert.Equal(preferredPath[i], emitted.To);
                from = emitted.To;
            }
            Assert.Equal(endPosition, from);
        }

        [Fact]
        public void Should_MoveCharacterAlongFallbackPath_When_PreferredPathIsBroken()
        {
            // Arrange
            EntityId id = new("mover");
            GamePosition startPosition = new(0, 0, 0);
            GamePosition endPosition = new(4, 0, 0);

            CommandRouter router = new();
            router.Register<MoveCharacterCommand>(new MoveCharacterHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(id, startPosition, new(), null));

            IReadOnlyList<IReadOnlyList<bool>> walkability =
            [
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ],
                [ true, true, true, true, true ]
            ];
            IReadOnlyList<IReadOnlyList<int>> cost =
            [
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
                [ 1, 1, 1, 1, 1 ],
            ];
            NavigationMap map = new();
            map.Set(walkability, cost);
            store.ActiveLocation.NavigationMap = map;
            GameOrchestrator orchestrator = new(router, store, (new Random() as IRandomSource)!, new NullLogger());

            List<GamePosition> preferredPath =
            [
                new(0, 1, 0),
                new(1, 1, 0),
                new(2, 1, 0)
            ];

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new MoveCharacterCommand(id, endPosition, preferredPath));

            // Assert
            Assert.Equal(4, emittedEvents.Count);
            GamePosition from = startPosition;
            foreach (IGameEvent ev in emittedEvents)
            {
                Assert.IsType<CharacterSteppedEvent>(ev);
                var emitted = (CharacterSteppedEvent)ev;
                Assert.Equal(id, emitted.Id);
                Assert.Equal(from, emitted.From);
                from = emitted.To;
            }
            Assert.Equal(endPosition, from);
        }
    }
}
