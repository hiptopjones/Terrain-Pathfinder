using ceometric.DelaunayTriangulator;
using Dcel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarAlgorithm : MonoBehaviour
{
    // Start position of the path, local to the terrain mesh
    public Vector3 StartVertex { get; set; }

    // End position of the path, local to the terrain mesh
    public Vector3 EndVertex { get; set; }

    // Assumes that the start and end are in the same terrain mesh
    // or at least that the terrain meshes have the same world transform
    public Transform WorldTransform { get; set; }
    public MeshData MeshData { get; set; }

    // Algorithm locals
    private ITerrainGraph TerrainGraph { get; set; }
    private Dictionary<Vector3, AStarGraphNode> AllNodes { get; set; }
    private MinHeap<AStarGraphNode> OpenNodes { get; set; }
    private HashSet<AStarGraphNode> ClosedNodes { get; set; }
    private Dictionary<AStarGraphNode, AStarGraphNode> Parents { get; set; }
    private AStarGraphNode CurrentNode { get; set; }
    private AStarGraphNode NeighborNode { get; set; }
    private AStarGraphNode PreviousNode { get; set; }

    // Return variables
    public List<Vector3> Path { get; set; }
    public bool IsPathValid { get; set; }

    // Coroutine advancement variables
    private bool IsKeyPressed { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoExecuteAlgorithm());
    }

    // Update is called once per frame
    void Update()
    {
        IsKeyPressed = false;

        if (Input.GetKey(KeyCode.Space))
        {
            IsKeyPressed = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (CurrentNode == null)
        {
            return;
        }

        List<AStarGraphNode> nodePath = GetNodePath(CurrentNode);
        HashSet<AStarGraphNode> nodePathSet = new HashSet<AStarGraphNode>(nodePath);

        Vector3 cubeSize = Vector3.one * 0.25f;

        foreach (AStarGraphNode node in AllNodes.Values)
        {
            if (node == CurrentNode)
            {
                Gizmos.color = Color.black;
            }
            else if (node == NeighborNode)
            {
                Gizmos.color = Color.gray;
            }
            else if (nodePathSet.Contains(node))
            {
                Gizmos.color = Color.green;
            }
            else if (node.IsClosed)
            {
                Gizmos.color = Color.red;
            }
            else // node is open
            {
                Gizmos.color = Color.yellow;
            }

            Gizmos.DrawCube(WorldTransform.TransformPoint(node.Vertex), cubeSize);
        }

        AStarGraphNode previousNode = null;
        foreach (AStarGraphNode currentNode in nodePath)
        {
            if (previousNode != null)
            {
                Gizmos.color = IsPathValid ? Color.magenta : Color.green;
                Gizmos.DrawLine(
                    WorldTransform.TransformPoint(previousNode.Vertex),
                    WorldTransform.TransformPoint(currentNode.Vertex));
            }

            previousNode = currentNode;
        }

        if (!IsPathValid && CurrentNode != null && NeighborNode != null)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(
                WorldTransform.TransformPoint(CurrentNode.Vertex),
                WorldTransform.TransformPoint(NeighborNode.Vertex));
        }
    }

    private IEnumerator CoExecuteAlgorithm()
    {
        yield return new WaitUntil(CanStepForward);

        //TerrainGraph = new DcelTerrainGraph(DcelGenerator.CreateDcelFromDelaunay(MeshData.DelaunayTriangles));
        //TerrainGraph = new GridTerrainGraph(MeshData.Width, MeshData.Height, MeshData.Vertices);
        TerrainGraph = new AnisotropicTerrainGraph(MeshData.Width, MeshData.Height, MeshData.Vertices, 5);

        AllNodes = new Dictionary<Vector3, AStarGraphNode>();
        OpenNodes = new MinHeap<AStarGraphNode>(MeshData.Vertices.Length);
        ClosedNodes = new HashSet<AStarGraphNode>();
        Parents = new Dictionary<AStarGraphNode, AStarGraphNode>();

        AStarGraphNode startNode = GetOrCreateNode(AllNodes, StartVertex, out _);
        OpenNodes.Add(startNode);

        yield return new WaitUntil(CanStepForward);

        while (true)
        {
            CurrentNode = OpenNodes.RemoveMin();
            ClosedNodes.Add(CurrentNode);
            CurrentNode.IsClosed = true; // Used for drawing

            if (CurrentNode.Vertex == EndVertex)
            {
                // Path found!
                Path = GetVertexPath(CurrentNode);
                IsPathValid = true;
                yield break;
            }

            if (Parents.ContainsKey(CurrentNode))
            {
                PreviousNode = Parents[CurrentNode];
            }

            foreach (Vector3 vertex in TerrainGraph.GetNeighborVertices(CurrentNode.Vertex))
            {
                bool isNewNode;
                NeighborNode = GetOrCreateNode(AllNodes, vertex, out isNewNode);

                // Ensure node isn't already in the closed list
                if (ClosedNodes.Contains(NeighborNode))
                {
                    continue;
                }

                // Calculate cost from start to here
                float movementCost = TerrainGraph.GetCost(
                    PreviousNode != null ? PreviousNode.Vertex : Vector3.zero,
                    CurrentNode.Vertex,
                    NeighborNode.Vertex);
                float gCost = CurrentNode.GCost + movementCost;

                if (isNewNode)
                {
                    NeighborNode.GCost = gCost;
                    NeighborNode.HCost = GetHeuristic(NeighborNode.Vertex, EndVertex);

                    // Add this node for consideration
                    OpenNodes.Add(NeighborNode);

                    // Keep track of how we got here
                    Parents[NeighborNode] = CurrentNode;
                }
                else if (NeighborNode.GCost > gCost)
                {
                    NeighborNode.GCost = gCost;

                    // Reorder this node in the open list
                    OpenNodes.Update(NeighborNode);

                    // Keep track of how we got here
                    Parents[NeighborNode] = CurrentNode;
                }
            }

            yield return new WaitUntil(CanStepForward);
        }
    }

    private List<AStarGraphNode> GetNodePath(AStarGraphNode currentNode)
    {
        List<AStarGraphNode> path = new List<AStarGraphNode>();

        while (true)
        {
            path.Add(currentNode);

            if (!Parents.TryGetValue(currentNode, out currentNode))
            {
                // Make path go from start to end
                path.Reverse();

                return path;
            }
        }
    }

    private List<Vector3> GetVertexPath(AStarGraphNode currentNode)
    {
        List<AStarGraphNode> path = GetNodePath(currentNode);
        return path.Select(x => x.Vertex).ToList();
    }

    private bool CanStepForward()
    {
        return IsKeyPressed;
    }

    private AStarGraphNode GetOrCreateNode(Dictionary<Vector3, AStarGraphNode> nodes, Vector3 vertex, out bool isNewNode)
    {
        isNewNode = false;

        AStarGraphNode node;

        if (!nodes.TryGetValue(vertex, out node))
        {
            node = new AStarGraphNode
            {
                Vertex = vertex
            };

            nodes[vertex] = node;
            isNewNode = true;
        }

        Debug.Log("Nodes: " + nodes.Count);

        return node;
    }

    private float GetHeuristic(Vector3 a, Vector3 b)
    {
        // Just uses a distance calculation
        return Mathf.Abs(Vector3.Distance(a, b));
    }
}
