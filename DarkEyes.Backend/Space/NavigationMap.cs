using System.Collections.Generic;

namespace DarkEyes.Backend.Space;

public class NavigationMap()
{
    private readonly BoundedMatrix<(bool, int)> _map = new(-1000, 1000, -1000, 1000);

    public bool IsWalkable(GamePosition position)
    {
        (bool walkable, int _) = _map.Get(position.Q, position.R);
        return walkable;
    }

    public int BaseMovementCost(GamePosition position)
    {
        (bool _, int movementCost) = _map.Get(position.Q, position.R);
        return movementCost;
    }

    public void Set(IReadOnlyList<IReadOnlyList<bool>> walkabilityMap, IReadOnlyList<IReadOnlyList<int>> movementCostMap)
    {
        if (walkabilityMap.Count == movementCostMap.Count)
        {
            for (int i = 0; i < walkabilityMap.Count; i++)
            {
                IReadOnlyList<bool> walkRow = walkabilityMap[i];
                IReadOnlyList<int> costRow = movementCostMap[i];

                if (walkRow.Count != costRow.Count)
                {
                    return;
                }

                for (int j = 0; j < walkRow.Count; j++)
                {
                    _map.Set(i, j, (walkRow[j], costRow[j]));
                }
            }
        }
    }
}
