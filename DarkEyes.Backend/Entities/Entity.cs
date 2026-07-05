using System.Collections.Generic;
using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.Entities
{
    public class Entity(EntityId id, GamePosition position, MovementProfile profile, IReadOnlyList<ComponentData>? componentData)
    {
        public readonly EntityId Id = id;
        public GamePosition Position = position;
        public MovementProfile MovementProfile = profile;
        public IReadOnlyList<ComponentData>? ComponentData = componentData;

        public Entity(Entity original) : this(original.Id, original.Position, original.MovementProfile, original.ComponentData)
        {
        }
    }
}
