using System.Collections.Generic;

namespace DarkEyes.Backend.Space.Pathfinding
{
    internal class PathNodeComparer : IComparer<PathNode>
    {
        public int Compare(PathNode a, PathNode b)
        {
            if (ReferenceEquals(a, b)) return 0;
            if (a is null) return -1;
            if (b is null) return 1;

            int compare = a.TotalCost.CompareTo(b.TotalCost);
            if (compare == 0) compare = a.ToCost.CompareTo(b.ToCost);
            if (compare == 0) compare = a.Position.Q.CompareTo(b.Position.Q);
            if (compare == 0) compare = a.Position.R.CompareTo(b.Position.R);

            return compare;
        }
    }
}
