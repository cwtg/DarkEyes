using System;

namespace DarkEyes.Backend.Space;

internal class PathNodeMinHeap(int capacity)
{
    private PathNode[] _nodes = new PathNode[capacity];
    private readonly PathNodeComparer _comparer = new();

    public int Count { get; private set; }

    public void Insert(PathNode node)
    {
        if (Count == _nodes.Length)
        {
            Array.Resize(ref _nodes, _nodes.Length * 2);
        }

        node.HeapIndex = Count;
        _nodes[Count] = node;
        Count++;
        SortUp(node);
    }

    public PathNode? ExtractMin()
    {
        if (Count == 0)
        {
            return null;
        }

        PathNode minNode = _nodes[0];
        Count--;

        if (Count > 0)
        {
            _nodes[0] = _nodes[Count];
            _nodes[0].HeapIndex = 0;
            SortDown(_nodes[0]);
        }

        minNode.HeapIndex = -1;
        return minNode;
    }

    public void UpdateNode(PathNode node) => SortUp(node);

    private void SortUp(PathNode node)
    {
        int parentIndex = (node.HeapIndex - 1) / 2;

        while (node.HeapIndex > 0)
        {
            PathNode parentNode = _nodes[parentIndex];
            if (_comparer.Compare(node, parentNode) < 0)
            {
                Swap(node, parentNode);
            }
            else
            {
                break;
            }

            parentIndex = (node.HeapIndex - 1) / 2;
        }
    }

    private void SortDown(PathNode node)
    {
        while (true)
        {
            int childLeftIndex = (node.HeapIndex * 2) + 1;
            int childRightIndex = (node.HeapIndex * 2) + 2;
            if (childLeftIndex < Count)
            {
                int swapIndex = childLeftIndex;
                if (childRightIndex < Count && _comparer.Compare(_nodes[childRightIndex], _nodes[childLeftIndex]) < 0)
                {
                    swapIndex = childRightIndex;
                }

                if (_comparer.Compare(_nodes[swapIndex], node) < 0)
                {
                    Swap(node, _nodes[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void Swap(PathNode nodeA, PathNode nodeB)
    {
        _nodes[nodeA.HeapIndex] = nodeB;
        _nodes[nodeB.HeapIndex] = nodeA;

        (nodeB.HeapIndex, nodeA.HeapIndex) = (nodeA.HeapIndex, nodeB.HeapIndex);
    }
}
