using System;
using DarkEyes.Backend.Checks;
using DarkEyes.Backend.Containers;
using DarkEyes.Backend.LockPicking;
using DarkEyes.Backend.Locks;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Tests.Support;

/// <summary>Builds a game state for a test scenario and a store with all gameplay registered.</summary>
internal sealed class ScenarioBuilder
{
    private GameState _state = GameState.Empty;
    private IChecks? _checks;

    public ScenarioBuilder WithHero(EntityId id, GamePosition at)
    {
        return Add(Entity.Create(id)
            .With(Placement.Key, new Placement(at)));
    }

    public ScenarioBuilder WithChest(EntityId id, GamePosition at, bool isOpen = false, ChestLock chestLock = ChestLock.None)
    {
        Entity chest = Entity.Create(id)
            .With(Placement.Key, new Placement(at))
            .With(ContainerState.Key, new ContainerState(isOpen));
        return Add(chestLock == ChestLock.None ? chest : chest.With(Lock.Key, new Lock(IsLocked: chestLock == ChestLock.Locked)));
    }

    /// <summary>The check service standing in for the rule module; without it, any check fails the test.</summary>
    public ScenarioBuilder WithChecks(IChecks checks)
    {
        _checks = checks;
        return this;
    }

    public GameStore Build()
    {
        GameStoreBuilder builder = new GameStoreBuilder().AddContainers().AddLocks().AddLockPicking()
            .AddActionHandler(new DecreeHandler());
        if (_checks is not null)
        {
            builder.UseChecks(_checks);
        }

        return builder.Build(_state);
    }

    private ScenarioBuilder Add(Entity entity)
    {
        if (_state.Entities.ContainsKey(entity.Id))
        {
            throw new InvalidOperationException($"The scenario already contains {entity.Id}.");
        }

        _state = _state.WithEntity(entity);
        return this;
    }
}
