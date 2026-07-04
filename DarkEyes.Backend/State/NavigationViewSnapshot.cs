using DarkEyes.Backend.Space;
using System.Collections.Generic;

namespace DarkEyes.Backend.State
{
    internal class NavigationViewSnapshot(NavigationMap navigationMap, HashSet<GamePosition> occupied) : INavigationView
    {
        private readonly NavigationMap _navigationMap = navigationMap;
        private readonly HashSet<GamePosition> _occupied = occupied;

        public CellContext ContextAt(GamePosition position)
        {
            return new();
        }

        public bool IsOccupied(GamePosition position)
        {
            return _occupied.Contains(position);
        }

        public bool IsWalkable(GamePosition position)
        {
            return _navigationMap.IsWalkable(position);
        }

        public int MovementCost(GamePosition position)
        {
            return _navigationMap.BaseMovementCost(position);
        }
    }
}
