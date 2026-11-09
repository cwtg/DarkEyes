namespace DarkEyes.Backend.Space;

internal class PathNode(GamePosition position)
{
    public GamePosition Position { get; set; } = position;

    public int FromCost { get; set; }

    public int ToCost { get; set; }

    public int TotalCost => FromCost + ToCost;

    public PathNode? Parent { get; set; }

    public NodeState State { get; set; } = NodeState.Unvisited;

    internal int HeapIndex { get; set; } = -1;
}
