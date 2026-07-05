using System.Collections.Generic;
using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.Entities
{
    public class Entity(EntityId id, GamePosition position, MovementProfile profile, IReadOnlyList<ComponentData> componentData)
    {
        public EntityId Id { get; } = id;
        public GamePosition Position { get; internal set; } = position;
        public MovementProfile MovementProfile { get; internal set; } = profile;
        public IReadOnlyList<ComponentData> ComponentData { get; internal set; } = componentData;

        public Entity(Entity original) : this(original.Id, original.Position, original.MovementProfile, original.ComponentData)
        {
        }
    }
}
