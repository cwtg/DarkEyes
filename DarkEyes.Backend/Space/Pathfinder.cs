using System.Collections.Generic;

namespace DarkEyes.Backend.Space
{
    public static class Pathfinder
    {
        public static IReadOnlyList<GamePosition> FindPath(GamePosition from, GamePosition to, INavigationView navigationView, MovementProfile profile)
        {
            PathNode start = new(from)
            {
                FromCost = 0,
                ToCost = from.DistanceTo(to),
                State = NodeState.Open,
            };
            PathNode target = new(to);

            PathNodeMinHeap openList = new(64);
            openList.Insert(start);

            BoundedMatrix<PathNode> grid = new(-1000, 1000, -1000, 1000);
            grid.Set(start.Position.Q, start.Position.R, start);

            while (openList.Count > 0)
            {
                PathNode? current = openList.ExtractMin();
                if (current is null) break;
                current.State = NodeState.Closed;

                if (current.Position == target.Position) return RetracePath(start, current);

                foreach (GamePosition neighbor in current.Position.Neighbors())
                {
                    ProcessNeighbor(neighbor, current, to, navigationView, grid, openList);
                }
            }

            return [];
        }

        private static void ProcessNeighbor(GamePosition neighbor, PathNode current, GamePosition target, INavigationView navigationView, BoundedMatrix<PathNode> grid, PathNodeMinHeap openList)
        {
            if (!navigationView.IsWalkable(neighbor) || navigationView.IsOccupied(neighbor))
                return;

            PathNode? neighborNode = grid.Get(neighbor.Q, neighbor.R);
            if (neighborNode is null)
            {
                neighborNode = new(neighbor);
                grid.Set(neighbor.Q, neighbor.R, neighborNode);
            }

            if (neighborNode.State == NodeState.Closed)
                return;

            int movementCost = navigationView.MovementCost(neighbor);
            TryUpdateNode(neighborNode, current, neighbor, movementCost, target, openList);
        }

        private static void TryUpdateNode(PathNode next, PathNode current, GamePosition neighbor, int neighborMovementCost, GamePosition target, PathNodeMinHeap openList)
        {
            int nextCost = current.FromCost + neighborMovementCost;
            if (next.State == NodeState.Unvisited || nextCost < next.FromCost)
            {
                bool alreadyInOpenList = next.State == NodeState.Open;

                next.FromCost = nextCost;
                next.ToCost = neighbor.DistanceTo(target);
                next.Parent = current;
                next.State = NodeState.Open;

                if (alreadyInOpenList)
                    openList.UpdateNode(next);
                else
                    openList.Insert(next);
            }
        }

        private static List<GamePosition> RetracePath(PathNode start, PathNode end)
        {
            List<GamePosition> path = [];
            PathNode current = end;

            while (current != start)
            {
                path.Add(current.Position);
                current = current.Parent!;
            }

            path.Reverse();
            return path;
        }
    }
}
