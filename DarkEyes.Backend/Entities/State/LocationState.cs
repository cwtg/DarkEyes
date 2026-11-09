using System.Collections.Generic;
using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.Entities.State;

public class LocationState
{
    public EntityStore Entities { get; } = new();
    public NavigationMap NavigationMap { get; set; } = new();

    public INavigationView NavigationSnapshot()
    {
        HashSet<GamePosition> occupied = [];
        foreach (EntityId entityId in Entities.GetAll())
        {
            occupied.Add(Entities.Get(entityId).Position);
        }

        return new NavigationViewSnapshot(NavigationMap, occupied);
    }

    public void ChangePosition(EntityId entityId, GamePosition position)
    {
        Entities.MutateEntity(entityId, (entity) => { entity.Position = position; return entity; });
    }
}
