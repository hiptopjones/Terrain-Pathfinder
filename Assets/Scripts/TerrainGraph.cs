using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGraph
{
    private int Width { get; }
    private int Height { get; }
    private Vector3[] Vertices { get; }

    private Vector3[] Directions { get; }

    public TerrainGraph(int width, int height, Vector3[] vertices)
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
        float heightDelta = Mathf.Abs(a.y - b.y);
        float scaledHeightDelta = heightDelta * 10f;

        // Bigger height differences lead to bigger costs
        float cost = 1 + scaledHeightDelta * scaledHeightDelta * scaledHeightDelta;

        // TODO: Avoid water if possible
        return cost;
    }

    public IEnumerable<Vector3> GetNeighbors(Vector3 position)
    {
        // X and Z are the coordinates of the height map, Y is the height value
        int index = Mathf.RoundToInt(position.z) * Width + Mathf.RoundToInt(position.x);

        // TODO: Check if passable before including

        // TODO: Given the origin, which is top vs. bottom ??

        // CARDINAL DIRECITONS

        if (position.x > 0)
        {
            int leftIndex = index - 1;
            yield return Vertices[leftIndex];
        }

        if (position.x < Width - 1)
        {
            int rightIndex = index + 1;
            yield return Vertices[rightIndex];
        }

        if (position.z > 0)
        {
            int topIndex = index - Width;
            yield return Vertices[topIndex];
        }

        if (position.z < Height - 1)
        {
            int bottomIndex = index + Width;
            yield return Vertices[bottomIndex];
        }

        // DIAGONAL DIRECTIONS

        if (position.x > 0 && position.z > 0)
        {
            int topleftIndex = index - Width - 1;
            yield return Vertices[topleftIndex];
        }

        if (position.x < Width - 1 && position.z > 0)
        {
            int topRightIndex = index - Width + 1;
            yield return Vertices[topRightIndex];
        }

        if (position.x > 0 && position.z < Height - 1)
        {
            int bottomLeftIndex = index + Width - 1;
            yield return Vertices[bottomLeftIndex];
        }

        if (position.x < Width - 1 && position.z < Height - 1)
        {
            int bottomIndex = index + Width + 1;
            yield return Vertices[bottomIndex];
        }

    }
}
