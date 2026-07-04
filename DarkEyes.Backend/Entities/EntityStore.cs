using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.Entities
{
    public class EntityStore
    {
        private readonly Dictionary<EntityId, Entity> _entities = [];

        public Entity Get(EntityId id)
        {
            return new(GetRef(id));
        }

        private Entity GetRef(EntityId id)
        {
            if (_entities.TryGetValue(id, out var entity))
                return entity;
            else
                throw new KeyNotFoundException($"There is no entity {id}");
        }

        public void Add(Entity entity)
        {
            _entities.Add(entity.Id, entity);
        }

        public void MutateEntity(EntityId id, Action<Entity> mutation)
        {
            var entity = GetRef(id);
            mutation(entity);
        }

        public IReadOnlyCollection<Entity> GetAll()
        {
            return _entities.Values;
        }
    }
}
