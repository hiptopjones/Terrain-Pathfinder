using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTerrainGraph : ITerrainGraph
{
    private int Width { get; }
    private int Height { get; }
    private Vector3[] Vertices { get; }

    private Vector3[] NeighborOffsets { get; }

    public GridTerrainGraph(int width, int height, Vector3[] vertices)
    {
        Width = width;
        Height = height;
        Vertices = vertices;

        NeighborOffsets = new Vector3[]
        {
            new Vector3(1, 0, 0), // right
            new Vector3(0, 0, 1), // up
            new Vector3(-1, 0, 0), // left
            new Vector3(0, 0, -1), // down
            new Vector3(1, 0, 1), // up-right
            new Vector3(-1, 0, 1), // up-left
            new Vector3(1, 0, -1), // down-right
            new Vector3(-1, 0, -1), // down-left
        };
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
        foreach (Vector3 neighborOffset in NeighborOffsets)
        {
            // X and Z are the coordinates of the height map, Y is the height value
            int neighborX = Mathf.RoundToInt(position.x) + Mathf.RoundToInt(neighborOffset.x);
            int neighborZ = Mathf.RoundToInt(position.z) + Mathf.RoundToInt(neighborOffset.z);

            // Test whether the direction is valid
            if (neighborX >= 0 && neighborX <= Width - 1 &&
                neighborZ >= 0 && neighborZ <= Height - 1)
            {
                int neighborIndex = neighborZ * Width + neighborX;
                Vector3 neighborVertex = Vertices[neighborIndex];
                yield return neighborVertex;

                if (recurseCount > 0)
                {
                    foreach (Vector3 extendedNeighborVertex in GetNeighborVertices(neighborVertex, recurseCount - 1))
                    {
                        yield return extendedNeighborVertex;
                    }
                }
            }
        }
    }
}
