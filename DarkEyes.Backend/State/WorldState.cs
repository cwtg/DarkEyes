using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Space;
using System.Collections.Generic;

namespace DarkEyes.Backend.State
{
    public class WorldState
    {
        public readonly EntityStore Entities = new();

        public NavigationMap NavigationMap = new();

        public void ChangePosition(EntityId entityId, GamePosition position)
        {
            Entities.MutateEntity(entityId, (entity) => entity.Position = position);
        }

        public INavigationView NavigationSnapshot()
        {
            HashSet<GamePosition> occupied = [];
            foreach (var entity in Entities.GetAll())
            {
                occupied.Add(entity.Position);
            }
            return new NavigationViewSnapshot(NavigationMap, occupied);
        }
    }
}
