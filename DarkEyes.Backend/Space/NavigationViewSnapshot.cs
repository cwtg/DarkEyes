using System.Collections.Generic;

namespace DarkEyes.Backend.Space;

internal class NavigationViewSnapshot(NavigationMap navigationMap, HashSet<GamePosition> occupied) : INavigationView
{
    private readonly NavigationMap _navigationMap = navigationMap;
    private readonly HashSet<GamePosition> _occupied = occupied;

    public CellContext ContextAt(GamePosition position)
    {
        return new();
    }

    public bool IsOccupied(GamePosition position) => _occupied.Contains(position);

    public bool IsWalkable(GamePosition position) => _navigationMap.IsWalkable(position);

    public int MovementCost(GamePosition position) => _navigationMap.BaseMovementCost(position);
}
