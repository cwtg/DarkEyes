using System;
using DarkEyes.Backend.Containers;
using DarkEyes.Backend.Pipeline;
using DarkEyes.Backend.Space;
using DarkEyes.Backend.State;

namespace DarkEyes.Backend.Tests.Support;

/// <summary>Builds a game state for a test scenario and a store with all gameplay registered.</summary>
internal sealed class ScenarioBuilder
{
    private GameState _state = GameState.Empty;

    public ScenarioBuilder WithHero(EntityId id, GamePosition at)
    {
        return Add(Entity.Create(id)
            .With(Placement.Key, new Placement(at)));
    }

    public ScenarioBuilder WithChest(EntityId id, GamePosition at, bool isOpen = false)
    {
        return Add(Entity.Create(id)
            .With(Placement.Key, new Placement(at))
            .With(ContainerState.Key, new ContainerState(isOpen)));
    }

    public GameStore Build() => new GameStoreBuilder().AddContainers().Build(_state);

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
