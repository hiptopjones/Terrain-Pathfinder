using Dcel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DcelTerrainGraph
{
    private Dictionary<Vector3, Vertex> VertexMapping { get;  }

    public DcelTerrainGraph(Dictionary<Vector3, Vertex> vertexMapping)
    {
        VertexMapping = vertexMapping;
    }

    public float GetCost(Vector3 a, Vector3 b)
    {
        float heightDelta = Mathf.Abs(a.y - b.y);
        float scaledHeightDelta = heightDelta * 10f;

        float distanceCost = Mathf.Abs(Vector3.Distance(a, b));

        // Bigger height differences lead to significantly bigger costs
        float heightCost = scaledHeightDelta * scaledHeightDelta * scaledHeightDelta;

        return distanceCost + heightCost;
    }

    public IEnumerable<Vector3> GetNeighbors(Vector3 position)
    {
        Vertex vertex = VertexMapping[position];
        HalfEdge currentEdge = vertex.IncidentEdge;

        HashSet<Vector3> neighbors = new HashSet<Vector3>();

        // NOTE: This may not enumerate all edges for vertices along the boundary
        while (neighbors.Add(currentEdge.Next.Origin.Coordinates))
        {
            yield return currentEdge.Next.Origin.Coordinates;

            currentEdge = currentEdge.Twin.Next;
            if (currentEdge == null)
            {
                // Probably on a boundary of the mesh
                break;
            }
        }
    }
}
