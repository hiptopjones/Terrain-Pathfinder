using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public int Width { get; }
    public int Height { get; }
    public Vector3[] Vertices { get; }
    public int[] Triangles { get; }
    public Vector2[] Uvs { get; set; }

    public MeshData(int width, int height)
    {
        Width = width;
        Height = height;

        Vertices = new Vector3[width * height];
        Uvs = new Vector2[width * height];

        // 6 because there are 2 triangles per quad, and each has 3 vertices
        Triangles = new int[(width - 1) * (height - 1) * 6];
    }
}
