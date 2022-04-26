using ceometric.DelaunayTriangulator;
using Dcel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // Start position of the path, local to the terrain mesh
    private Vector3 StartVertex { get; set; }

    // End position of the path, local to the terrain mesh
    private Vector3 EndVertex { get; set; }

    // Assumes that the start and end are in the same terrain mesh
    // or at least that the terrain meshes have the same world transform
    private Transform WorldTransform { get; set; }
    private MeshData MeshData { get; set; }

    private bool IsPathStarted { get; set; }
    private bool IsPathEnded { get; set; }

    private List<Vector3> Path { get; set; }
    private List<Vector3> SmoothedPath { get; set; }
    private bool IsPathDirty { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        MeshPicker meshPicker = FindObjectOfType<MeshPicker>();
        meshPicker.OnPrimaryVertexSelected = OnPrimaryVertexSelected;
        meshPicker.OnSecondaryVertexSelected = OnSecondaryVertexSelected;
        meshPicker.OnSelectedVerticesCleared = OnSelectedVerticesCleared;
    }

    private void OnPrimaryVertexSelected(Vector3 vertexPosition, Transform worldTransform, MeshData meshData)
    {
        StartVertex = vertexPosition;
        WorldTransform = worldTransform;
        MeshData = meshData;

        IsPathStarted = true;
        IsPathDirty = true;
    }

    private void OnSecondaryVertexSelected(Vector3 vertexPosition, Transform worldTransform, MeshData meshData)
    {
        EndVertex = vertexPosition;
        WorldTransform = worldTransform;
        MeshData = meshData;

        IsPathEnded = true;
        IsPathDirty = true;
    }

    private void OnSelectedVerticesCleared()
    {
        IsPathStarted = false;
        IsPathEnded = false;
        Path = null;
        SmoothedPath = null;

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

        if (IsPathDirty)
        {
            Path = FindPath();
            SmoothedPath = SplineSmooth.Smooth(Path);
            IsPathDirty = false;
        }

        DrawPath(Path, Color.magenta);
        //DrawPath(SmoothedPath, Color.cyan);
    }

    private void DrawPath(List<Vector3> path, Color color)
    {
        Vector3 previousVertex = path[0];
        foreach (Vector3 currentVertex in path)
        {
            Debug.DrawLine(
                WorldTransform.TransformPoint(previousVertex),
                WorldTransform.TransformPoint(currentVertex),
                color);

            previousVertex = currentVertex;
        }
    }

    private void OnDrawGizmos()
    {
        if (IsPathStarted)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(WorldTransform.TransformPoint(StartVertex), 1f);
        }

        if (IsPathEnded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(WorldTransform.TransformPoint(EndVertex), 1f);
        }

        if (Path != null)
        {
            foreach (Vector3 currentVertex in Path)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(WorldTransform.TransformPoint(currentVertex), 0.2f);
            }
        }
    }

    private float GetHeuristic(Vector3 a, Vector3 b)
    {
        // Just uses a distance calculation
        return Mathf.Abs(Vector3.Distance(a, b));
    }

    private List<Vector3> FindPath()
    {
        //ITerrainGraph terrainGraph = new DcelTerrainGraph(DcelGenerator.CreateDcelFromDelaunay(MeshData.DelaunayTriangles));
        //ITerrainGraph terrainGraph = new GridTerrainGraph(MeshData.Width, MeshData.Height, MeshData.Vertices);
        ITerrainGraph terrainGraph = new AnisotropicTerrainGraph(MeshData.Width, MeshData.Height, MeshData.Vertices, 5);

        Dictionary<Vector3, AStarGraphNode> allNodes = new Dictionary<Vector3, AStarGraphNode>();
        MinHeap<AStarGraphNode> openNodes = new MinHeap<AStarGraphNode>(MeshData.Vertices.Length);
        Dictionary<Vector3, AStarGraphNode> closedNodes = new Dictionary<Vector3, AStarGraphNode>();
        Dictionary<Vector3, Vector3> parents = new Dictionary<Vector3, Vector3>();

        AStarGraphNode startNode = GetOrCreateNode(allNodes, StartVertex, out _);
        openNodes.Add(startNode);

        while (true)
        {
            AStarGraphNode currentNode = openNodes.RemoveMin();
            closedNodes[currentNode.Vertex] = currentNode;
            currentNode.IsClosed = true; // Used for drawing

            if (currentNode.Vertex == EndVertex)
            {
                // Path found!
                List<Vector3> path = new List<Vector3>();

                Vector3 currentVertex = currentNode.Vertex;
                while (true)
                {
                    path.Add(currentVertex);
                    
                    if (!parents.TryGetValue(currentVertex, out currentVertex))
                    {
                        // Make path go from start to end
                        path.Reverse();

                        return path;
                    }
                }
            }

            Vector3 previousNodeVertex;
            parents.TryGetValue(currentNode.Vertex, out previousNodeVertex);

            foreach (Vector3 vertex in terrainGraph.GetNeighborVertices(currentNode.Vertex))
            {
                bool isNewNode;
                AStarGraphNode nextNode = GetOrCreateNode(allNodes, vertex, out isNewNode);

                // Ensure node isn't already in the closed list
                if (closedNodes.ContainsKey(nextNode.Vertex))
                {
                    continue;
                }

                // Calculate cost from start to here
                float gCost = currentNode.GCost + terrainGraph.GetCost(previousNodeVertex, currentNode.Vertex, nextNode.Vertex);

                if (isNewNode)
                {
                    nextNode.GCost = gCost;
                    nextNode.HCost = GetHeuristic(nextNode.Vertex, EndVertex);

                    // Add this node for consideration
                    openNodes.Add(nextNode);

                    // Keep track of how we got here
                    parents[nextNode.Vertex] = currentNode.Vertex;
                }
                else if (nextNode.GCost > gCost)
                {
                    nextNode.GCost = gCost;

                    // Reorder this node in the open list
                    openNodes.Update(nextNode);

                    // Keep track of how we got here
                    parents[nextNode.Vertex] = currentNode.Vertex;
                }
            }
        }
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
}
