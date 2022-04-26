using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGraphNode : IHeapItem<AStarGraphNode>
{
    // IHeapItem
    public int HeapIndex { get; set; }

    public Vector3 Vertex { get; set; }
    public float HCost { get; set; } // Heuristic - estimate from here to end
    public float GCost { get; set; } // Cost So Far - cost from start to here
    public float FCost // HCost + GCost - most efficient path from start to end
    {
        get
        {
            return GCost + HCost;
        }
    }

    public bool IsClosed { get; set; }

    // IComparable
    public int CompareTo(AStarGraphNode other)
    {
        return FCost.CompareTo(other.FCost);
    }
}