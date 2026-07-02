using DarkEyes.Backend.Core;
using DarkEyes.Backend.Space;
using System;
using System.Collections.Generic;

namespace DarkEyes.Backend.State
{
    public class WorldState
    {
        public Dictionary<EntityId, GamePosition> EntityPositions = [];

        public void ChangePosition(EntityId entityId, GamePosition position)
        {
            if (EntityPositions.ContainsKey(entityId))
                EntityPositions[entityId] = position;
            else
                throw new InvalidOperationException($"Entity {entityId} not found");
        }
    }
}
