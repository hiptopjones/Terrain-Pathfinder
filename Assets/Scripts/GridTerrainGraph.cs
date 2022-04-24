using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTerrainGraph : ITerrainGraph
{
    private int Width { get; }
    private int Height { get; }
    private Vector3[] Vertices { get; }

    private Vector3[] Directions { get; }

    public GridTerrainGraph(int width, int height, Vector3[] vertices)
    {
        Width = width;
        Height = height;
        Vertices = vertices;

        Directions = new Vector3[]
        {
            new Vector3(1, 0, 0), // right
            new Vector3(0, 0, width), // up
            new Vector3(-1, 0, 0), // left
            new Vector3(0, 0, -width), // down
            new Vector3(1, 0, width), // up-right
            new Vector3(-1, 0, width), // up-left
            new Vector3(1, 0, -width), // down-right
            new Vector3(-1, 0, -width), // down-left
        };
    }

    public float GetCost(Vector3 a, Vector3 b)
    {
        float rise = Mathf.Abs(a.y - b.y);
        float run = Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));

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
        // X and Z are the coordinates of the height map, Y is the height value
        int index = Mathf.RoundToInt(position.z) * Width + Mathf.RoundToInt(position.x);

        // TODO: Check if passable before including

        // TODO: Given the origin, which is top vs. bottom ??

        // CARDINAL DIRECITONS

        if (position.x > 0)
        {
            int leftIndex = index - 1;
            Vector3 neighbor = Vertices[leftIndex];
            yield return neighbor;

            if (recurseCount > 0)
            {
                foreach (Vector3 extendedNeighbor in GetNeighborVertices(neighbor, recurseCount -1))
                {
                    yield return extendedNeighbor;
                }
            }
        }

        if (position.x < Width - 1)
        {
            int rightIndex = index + 1;
            Vector3 neighbor = Vertices[rightIndex];
            yield return neighbor;

            if (recurseCount > 0)
            {
                foreach (Vector3 extendedNeighbor in GetNeighborVertices(neighbor, recurseCount - 1))
                {
                    yield return extendedNeighbor;
                }
            }
        }

        if (position.z > 0)
        {
            int topIndex = index - Width;
            Vector3 neighbor = Vertices[topIndex];
            yield return neighbor;

            if (recurseCount > 0)
            {
                foreach (Vector3 extendedNeighbor in GetNeighborVertices(neighbor, recurseCount - 1))
                {
                    yield return extendedNeighbor;
                }
            }
        }

        if (position.z < Height - 1)
        {
            int bottomIndex = index + Width;
            Vector3 neighbor = Vertices[bottomIndex];
            yield return neighbor;

            if (recurseCount > 0)
            {
                foreach (Vector3 extendedNeighbor in GetNeighborVertices(neighbor, recurseCount - 1))
                {
                    yield return extendedNeighbor;
                }
            }

        }

        // DIAGONAL DIRECTIONS

        if (position.x > 0 && position.z > 0)
        {
            int topleftIndex = index - Width - 1;
            Vector3 neighbor = Vertices[topleftIndex];
            yield return neighbor;

            if (recurseCount > 0)
            {
                foreach (Vector3 extendedNeighbor in GetNeighborVertices(neighbor, recurseCount - 1))
                {
                    yield return extendedNeighbor;
                }
            }
        }

        if (position.x < Width - 1 && position.z > 0)
        {
            int topRightIndex = index - Width + 1;
            Vector3 neighbor = Vertices[topRightIndex];
            yield return neighbor;

            if (recurseCount > 0)
            {
                foreach (Vector3 extendedNeighbor in GetNeighborVertices(neighbor, recurseCount - 1))
                {
                    yield return extendedNeighbor;
                }
            }
        }

        if (position.x > 0 && position.z < Height - 1)
        {
            int bottomLeftIndex = index + Width - 1;
            Vector3 neighbor = Vertices[bottomLeftIndex];
            yield return neighbor;

            if (recurseCount > 0)
            {
                foreach (Vector3 extendedNeighbor in GetNeighborVertices(neighbor, recurseCount - 1))
                {
                    yield return extendedNeighbor;
                }
            }
        }

        if (position.x < Width - 1 && position.z < Height - 1)
        {
            int bottomIndex = index + Width + 1;
            Vector3 neighbor = Vertices[bottomIndex];
            yield return neighbor;

            if (recurseCount > 0)
            {
                foreach (Vector3 extendedNeighbor in GetNeighborVertices(neighbor, recurseCount - 1))
                {
                    yield return extendedNeighbor;
                }
            }
        }
    }
}
