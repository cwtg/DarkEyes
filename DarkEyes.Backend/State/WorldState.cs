using System.Collections.Generic;
using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.State
{
    public class WorldState
    {
        public readonly EntityStore Entities = new();

        public NavigationMap NavigationMap { get; set; } = new();

        public void ChangePosition(EntityId entityId, GamePosition position)
        {
            Entities.MutateEntity(entityId, (entity) => entity.Position = position);
        }

        public INavigationView NavigationSnapshot()
        {
            HashSet<GamePosition> occupied =[];
            foreach (EntityId entityId in Entities.GetAll())
            {
                occupied.Add(Entities.Get(entityId).Position);
            }

            return new NavigationViewSnapshot(NavigationMap, occupied);
        }
    }
}
