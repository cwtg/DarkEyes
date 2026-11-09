using System.Collections.Immutable;

namespace DarkEyes.Backend.State;

/// <summary>The root of the immutable state tree. Only created and changed through its own members.</summary>
public sealed record GameState
{
    private GameState(ImmutableSortedDictionary<EntityId, Entity> entities)
    {
        Entities = entities;
    }

    public static GameState Empty { get; } = new(ImmutableSortedDictionary.Create<EntityId, Entity>(EntityId.Ordinal));

    public ImmutableSortedDictionary<EntityId, Entity> Entities { get; private init; }

    public Entity GetEntity(EntityId id) => Entities[id];

    public GameState WithEntity(Entity entity) => this with { Entities = Entities.SetItem(entity.Id, entity) };
}
