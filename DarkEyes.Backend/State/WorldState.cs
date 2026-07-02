using DarkEyes.Backend.Entities;
using DarkEyes.Backend.Space;

namespace DarkEyes.Backend.State
{
    public class WorldState
    {
        public readonly EntityStore Entities = new();

        public void ChangePosition(EntityId entityId, GamePosition position)
        {
            Entities.MutateEntity(entityId, (entity) => entity.Position = position);
        }
    }
}
