namespace DarkEyes.Backend.Space
{
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

    public void Set(bool[,] walkabilityMap, int[,] movementCostMap)
    {
        if (walkabilityMap.GetLength(0) == movementCostMap.GetLength(0) && walkabilityMap.GetLength(1) == movementCostMap.GetLength(1))
        {
            for (int i = 0; i < walkabilityMap.GetLength(0); i++)
            {
                for (int j = 0; j < walkabilityMap.GetLength(1); j++)
                {
                    _map.Set(i, j, (walkabilityMap[i, j], movementCostMap[i, j]));
                }
            }
        }
    }
}
}
