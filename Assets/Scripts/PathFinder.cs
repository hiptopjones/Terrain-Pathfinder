using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private MinHeap<AStarGraphNode> frontier;

    private Vector3 StartVertex { get; set; }
    private Vector3 EndVertex { get; set; }

    private Transform WorldTransform { get; set; }
    private MeshData MeshData { get; set; }

    private bool IsPathStarted { get; set; }
    private bool IsPathEnded { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        MeshPicker meshPicker = FindObjectOfType<MeshPicker>();
        meshPicker.OnPrimaryVertexSelected = OnPrimaryVertexSelected;
        meshPicker.OnSecondaryVertexSelected = OnSecondaryVertexSelected;
    }

    private void OnPrimaryVertexSelected(Vector3 vertexPosition, Transform worldTransform, MeshData meshData)
    {
        StartVertex = vertexPosition;
        WorldTransform = worldTransform;
        MeshData = meshData;

        IsPathStarted = true;
    }

    private void OnSecondaryVertexSelected(Vector3 vertexPosition, Transform worldTransform, MeshData meshData)
    {
        EndVertex = vertexPosition;
        WorldTransform = worldTransform;
        MeshData = meshData;

        IsPathEnded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPathStarted || !IsPathEnded)
        {
            // No path endpoints selected
            return;
        }

        Debug.DrawLine(
            WorldTransform.TransformPoint(StartVertex),
            WorldTransform.TransformPoint(EndVertex),
            Color.yellow);

        List<Vector3> path = FindPath();

        Vector3 previousVertex = StartVertex;
        foreach (Vector3 currentVertex in path)
        {
            Debug.DrawLine(
                WorldTransform.TransformPoint(previousVertex),
                WorldTransform.TransformPoint(currentVertex),
                Color.magenta);

            previousVertex = currentVertex;
        }
    }

    private void OnDrawGizmos()
    {
        if (IsPathStarted)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(WorldTransform.TransformPoint(StartVertex), 0.5f);
        }

        if (IsPathEnded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(WorldTransform.TransformPoint(EndVertex), 0.5f);
        }
    }

    private int GetHeuristic(Vector3 a, Vector3 b)
    {
        // Just uses a Manhattan distance calculation
        return Mathf.RoundToInt(Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z));
    }

    private List<Vector3> FindPath()
    {
        TerrainGraph terrainGraph = new TerrainGraph(MeshData.Width, MeshData.Height, MeshData.Vertices);

        MinHeap<AStarGraphNode> OpenNodes = new MinHeap<AStarGraphNode>(MeshData.Vertices.Length);
        Dictionary<Vector3, AStarGraphNode> ClosedNodes = new Dictionary<Vector3, AStarGraphNode>();
        Dictionary<Vector3, Vector3> Parents = new Dictionary<Vector3, Vector3>();
        
        OpenNodes.Add(GetNode(StartVertex));

        while (true)
        {
            AStarGraphNode currentNode = OpenNodes.RemoveMin();
            ClosedNodes[currentNode.Vertex] = currentNode;

            if (currentNode.Vertex == EndVertex)
            {
                // Path found!
                List<Vector3> path = new List<Vector3>();

                Vector3 currentVertex = currentNode.Vertex;
                while (true)
                {
                    path.Add(currentVertex);
                    
                    if (!Parents.TryGetValue(currentVertex, out currentVertex))
                    {
                        // Make path go from start to end
                        path.Reverse();

                        return path;
                    }
                }
            }

            foreach (Vector3 vertex in terrainGraph.GetNeighbors(currentNode.Vertex))
            {
                // This will create duplicate objects
                AStarGraphNode nextNode = GetNode(vertex);

                // Ensure node isn't already in the closed list
                if (ClosedNodes.ContainsKey(nextNode.Vertex))
                {
                    continue;
                }

                // Update costs
                nextNode.GCost = currentNode.GCost + terrainGraph.GetCost(currentNode.Vertex, nextNode.Vertex);
                nextNode.HCost = GetHeuristic(nextNode.Vertex, EndVertex);

                // Add this node for consideration
                // TODO: Should check if it's already present, and update it instead
                OpenNodes.Add(nextNode);

                // Keep track of how we got here
                Parents[nextNode.Vertex] = currentNode.Vertex;
            }
        }
    }

    private AStarGraphNode GetNode(Vector3 vertex)
    {
        return new AStarGraphNode
        {
            Vertex = vertex,
        };
    }

    public class AStarGraphNode : IComparable<AStarGraphNode>
    {
        public Vector3 Vertex { get; set; }
        public int HCost { get; set; } // Heuristic - estimate from here to end
        public int GCost { get; set; } // Cost So Far - cost from start to here
        public int FCost // HCost + GCost - most efficient path from start to end
        {
            get
            {
                return GCost + HCost;
            }
        }

        public int CompareTo(AStarGraphNode other)
        {
            return FCost.CompareTo(other.FCost);
        }
    }
}
