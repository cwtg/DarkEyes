using System;
using System.Collections.Generic;
using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.Entities
{
    public class Entity(EntityId id, GamePosition position, MovementProfile profile, IReadOnlyList<ComponentData>? componentData = null, IReadOnlyDictionary<Type, object>? states = null)
    {
        public EntityId Id { get; } = id;
        public GamePosition Position { get; internal set; } = position;
        public MovementProfile MovementProfile { get; internal set; } = profile;
        public IReadOnlyList<ComponentData> ComponentData { get; internal set; } = componentData ?? [];
        public IReadOnlyDictionary<Type, object> States { get; } = states ?? new Dictionary<Type, object>();

        public Entity(Entity original) : this(original.Id, original.Position, original.MovementProfile, original.ComponentData, original.States)
        {
        }

        public Entity WithState<T>(T updatedState)
        {
            Type key = typeof(T);
            var updatedCopy = new Dictionary<Type, object>(States) { [key] = updatedState! };
            return new Entity(Id, Position, MovementProfile, ComponentData, updatedCopy);
        }
    }
}
