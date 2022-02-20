using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uv;

    public MeshData(int width, int height)
    {
        vertices = new Vector3[width * height];
        uv = new Vector2[width * height];

        // 6 because there are 2 triangles per quad, and each has 3 vertices
        triangles = new int[(width - 1) * (height - 1) * 6];
    }
}
