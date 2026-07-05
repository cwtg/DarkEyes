using System.Collections.Generic;

namespace DarkEyes.Backend.Space.Pathfinding
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
                PathNode current = openList.ExtractMin();
                current.State = NodeState.Closed;

                if (current.Position == target.Position) return RetracePath(start, current);

                foreach (GamePosition neighbor in current.Position.Neighbors())
                {
                    if (!navigationView.IsWalkable(neighbor) || navigationView.IsOccupied(neighbor)) continue;

                    PathNode next = grid.Get(neighbor.Q, neighbor.R);
                    if (next is null)
                    {
                        next = new(neighbor);
                        grid.Set(neighbor.Q, neighbor.R, next);
                    }

                    if (next.State == NodeState.Closed) continue;

                    int nextCost = current.FromCost + navigationView.MovementCost(neighbor);
                    if (next.State == NodeState.Unvisited || nextCost < next.FromCost)
                    {
                        bool alreadyInOpenList = next.State == NodeState.Open;

                        next.FromCost = nextCost;
                        next.ToCost = neighbor.DistanceTo(to);
                        next.Parent = current;
                        next.State = NodeState.Open;

                        if (alreadyInOpenList)
                            openList.UpdateNode(next);
                        else
                            openList.Insert(next);
                    }
                }
            }

            return[];
        }

        private static List<GamePosition> RetracePath(PathNode start, PathNode end)
        {
            List<GamePosition> path =[];
            PathNode current = end;

            while (current != start)
            {
                path.Add(current.Position);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}
