using Dcel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DcelTerrainGraph : ITerrainGraph
{
    private Dictionary<Vector3, Vertex> VertexMapping { get;  }

    public DcelTerrainGraph(Dictionary<Vector3, Vertex> vertexMapping)
    {
        VertexMapping = vertexMapping;
    }

    public float GetCost(Vector3 previousPosition, Vector3 currentPosition, Vector3 nextPosition)
    {
        // NOTE: Ignores previous position

        float rise = Mathf.Abs(currentPosition.y - nextPosition.y);
        float run = Mathf.Sqrt(
            (currentPosition.x - nextPosition.x) * (currentPosition.x - nextPosition.x) +
            (currentPosition.z - nextPosition.z) * (currentPosition.z - nextPosition.z));

        float slope = rise / run;
        float scaledSlope = slope * 10;

        // Bigger height differences lead to significantly bigger costs
        float slopeCost = scaledSlope * scaledSlope * scaledSlope;
        float distanceCost = run * run;

        return distanceCost + slopeCost;
    }

    public IEnumerable<Vector3> GetNeighborVertices(Vector3 position)
    {
        int recurseCount = 0; // Non-zero means it will also return neighbors of neighbors
        return GetNeighborVertices(position, recurseCount);
    }

    private IEnumerable<Vector3> GetNeighborVertices(Vector3 position, int recurseCount)
    {
        Vertex vertex = VertexMapping[position];
        HalfEdge currentEdge = vertex.IncidentEdge;

        HashSet<Vector3> neighborVertices = new HashSet<Vector3>();

        // NOTE: This may not enumerate all edges for vertices along the boundary
        while (neighborVertices.Add(currentEdge.Next.Origin.Coordinates))
        {
            Vector3 neighborVertex = currentEdge.Next.Origin.Coordinates;
            yield return neighborVertex;

            if (recurseCount > 0)
            {
                foreach (Vector3 extendedNeighborVertex in GetNeighborVertices(neighborVertex, recurseCount - 1))
                {
                    yield return extendedNeighborVertex;
                }
            }

            currentEdge = currentEdge.Twin.Next;
            if (currentEdge == null)
            {
                // Probably on a boundary of the mesh
                break;
            }
        }
    }
}
