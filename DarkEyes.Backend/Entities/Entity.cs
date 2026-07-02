using DarkEyes.Backend.Space;
using System.Collections.Generic;

namespace DarkEyes.Backend.Entities
{
    public class Entity(EntityId id, GamePosition position, IReadOnlyList<ComponentData>? componentData)
    {
        public readonly EntityId Id = id;
        public GamePosition Position = position;
        public IReadOnlyList<ComponentData>? ComponentData = componentData;

        public Entity(Entity original) : this(original.Id, original.Position, original.ComponentData) { }
    }
}
