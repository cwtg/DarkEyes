using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using DarkEyes.Backend.Core.Resolution;
using DarkEyes.Backend.Core.Stats;
using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Entities.State;
using DarkEyes.Backend.Events;
using DarkEyes.Backend.Handlers;
using DarkEyes.Backend.Logging;
using DarkEyes.Backend.Orchestration;
using DarkEyes.Backend.Store;
using Xunit.Abstractions;
using Attribute = DarkEyes.Backend.Core.Stats.Attribute;

namespace DarkEyes.Backend.Tests.Integration
{
    public sealed class OpenContainerTests(ITestOutputHelper output)
    {
        [Fact]
        public void Should_UnlockContainer_When_SkillCheckSuccessful()
        {
            // Arrange
            var intuition = new Attribute(new("intuition"), 14);
            var dexterity1 = new Attribute(new("dexterity"), 12);
            var dexterity2 = new Attribute(new("dexterity"), 12);
            var skill = new Skill(new("lockpicking"), 5, intuition, dexterity1, dexterity2);

            EntityId actor = new("actor");
            CharacterStats stats = new("Stats", [intuition, dexterity1], [skill]);
            EntityId container = new("container");
            LockedContainerState lockedContainerState = new("chest", 5, [], true);

            CommandRouter router = new();
            router.Register<OpenContainerCommand>(new OpenContainerHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(actor, new(1, 0, 0), new(), null, new Dictionary<Type, object> { { typeof(CharacterStats), stats } }));
            store.ActiveLocation.Entities.Add(new Entity(container, new(0, 0, 0), new(), null, new Dictionary<Type, object> { { typeof(LockedContainerState), lockedContainerState } }));

            GameOrchestrator orchestrator = new(router, store, new SeededRandomSource(0), new NullLogger());

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new OpenContainerCommand(container, actor));
            store.ActiveLocation.Entities.Get(container).States.TryGetValue(typeof(LockedContainerState), out object? state);

            // Assert
            Assert.Equal(2, emittedEvents.Count);
            Assert.IsType<ResolutionRolledEvent>(emittedEvents[0]);
            Assert.IsType<ContainerUnlockEvent>(emittedEvents[1]);
            Assert.False((state as LockedContainerState)!.Locked);
        }

        [Fact]
        public void Should_NotUnlockContainer_When_SkillCheckFailed()
        {
            // Arrange
            var intuition = new Attribute(new("intuition"), 8);
            var dexterity1 = new Attribute(new("dexterity"), 10);
            var dexterity2 = new Attribute(new("dexterity"), 10);
            var skill = new Skill(new("lockpicking"), 5, intuition, dexterity1, dexterity2);

            EntityId actor = new("actor");
            CharacterStats stats = new("Stats", [intuition, dexterity1], [skill]);
            EntityId container = new("container");
            LockedContainerState lockedContainerState = new("chest", 5, [], true);

            CommandRouter router = new();
            router.Register<OpenContainerCommand>(new OpenContainerHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(actor, new(1, 0, 0), new(), null, new Dictionary<Type, object> { { typeof(CharacterStats), stats } }));
            store.ActiveLocation.Entities.Add(new Entity(container, new(0, 0, 0), new(), null, new Dictionary<Type, object> { { typeof(LockedContainerState), lockedContainerState } }));

            GameOrchestrator orchestrator = new(router, store, new SeededRandomSource(10), new NullLogger());

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new OpenContainerCommand(container, actor));
            store.ActiveLocation.Entities.Get(container).States.TryGetValue(typeof(LockedContainerState), out object? state);

            // Assert
            Assert.Equal(2, emittedEvents.Count);
            Assert.IsType<ResolutionRolledEvent>(emittedEvents[0]);
            Assert.IsType<ContainerOpenFailedEvent>(emittedEvents[1]);
            Assert.True((state as LockedContainerState)!.Locked);
        }

        [Fact]
        public void Should_NotUnlockContainer_When_ActorIsTooFarAway()
        {
            // Arrange
            var intuition = new Attribute(new("intuition"), 12);
            var dexterity1 = new Attribute(new("dexterity"), 10);
            var dexterity2 = new Attribute(new("dexterity"), 10);
            var skill = new Skill(new("lockpicking"), 5, intuition, dexterity1, dexterity2);

            EntityId actor = new("actor");
            CharacterStats stats = new("Stats", [intuition, dexterity1], [skill]);
            EntityId container = new("container");
            LockedContainerState lockedContainerState = new("chest", 5, [], true);

            CommandRouter router = new();
            router.Register<OpenContainerCommand>(new OpenContainerHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(actor, new(2, 0, 0), new(), null, new Dictionary<Type, object> { { typeof(CharacterStats), stats } }));
            store.ActiveLocation.Entities.Add(new Entity(container, new(0, 0, 0), new(), null, new Dictionary<Type, object> { { typeof(LockedContainerState), lockedContainerState } }));

            GameOrchestrator orchestrator = new(router, store, new SeededRandomSource(10), new NullLogger());

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new OpenContainerCommand(container, actor));
            store.ActiveLocation.Entities.Get(container).States.TryGetValue(typeof(LockedContainerState), out object? state);

            // Assert
            Assert.Single(emittedEvents);
            Assert.IsType<ErrorRaisedEvent>(emittedEvents[0]);
            Assert.True((state as LockedContainerState)!.Locked);
        }

        [Fact]
        public void Should_NotUnlockContainer_When_ContainerIsAlreadyOpen()
        {
            // Arrange
            var intuition = new Attribute(new("intuition"), 14);
            var dexterity1 = new Attribute(new("dexterity"), 12);
            var dexterity2 = new Attribute(new("dexterity"), 12);
            var skill = new Skill(new("lockpicking"), 5, intuition, dexterity1, dexterity2);

            EntityId actor = new("actor");
            CharacterStats stats = new("Stats", [intuition, dexterity1], [skill]);
            EntityId container = new("container");
            LockedContainerState lockedContainerState = new("chest", 5, [], false);

            CommandRouter router = new();
            router.Register<OpenContainerCommand>(new OpenContainerHandler().Handle);
            GameStore store = new();
            store.ActiveLocation.Entities.Add(new Entity(actor, new(1, 0, 0), new(), null, new Dictionary<Type, object> { { typeof(CharacterStats), stats } }));
            store.ActiveLocation.Entities.Add(new Entity(container, new(0, 0, 0), new(), null, new Dictionary<Type, object> { { typeof(LockedContainerState), lockedContainerState } }));

            GameOrchestrator orchestrator = new(router, store, new SeededRandomSource(0), new NullLogger());

            List<IGameEvent> emittedEvents = [];
            using IDisposable subscription = orchestrator.Events.Subscribe(emittedEvents.Add);

            // Act
            orchestrator.Dispatch(new OpenContainerCommand(container, actor));
            store.ActiveLocation.Entities.Get(container).States.TryGetValue(typeof(LockedContainerState), out object? state);

            // Assert
            Assert.Single(emittedEvents);
            Assert.IsType<ErrorRaisedEvent>(emittedEvents[0]);
            Assert.False((state as LockedContainerState)!.Locked);
        }
    }
}
