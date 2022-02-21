using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGraph
{
    private int Width { get; }
    private int Height { get; }
    private Vector3[] Vertices { get; }

    public TerrainGraph(int width, int height, Vector3[] vertices)
    {
        Width = width;
        Height = height;
        Vertices = vertices;
    }

    public int GetCost(Vector3 a, Vector3 b)
    {
        float heightDelta = Mathf.Abs(a.y - b.y);

        // Bigger height distances lead to bigger costs
        int cost = 1 + Mathf.RoundToInt(heightDelta * 10);

        //// Avoid water if possible
        //if (b.y < 0.3)
        //{
        //    cost *= 10;
        //}

        return cost;
    }

    public IEnumerable<Vector3> GetNeighbors(Vector3 position)
    {
        // X and Z are the coordinates of the height map, Y is the height value
        int index = Mathf.RoundToInt(position.z) * Width + Mathf.RoundToInt(position.x);

        // TODO: Check if passable before including

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

        // Given the origin, which is top vs. bottom ??
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
    }
}
