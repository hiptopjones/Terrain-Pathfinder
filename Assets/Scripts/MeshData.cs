using ceometric.DelaunayTriangulator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public int Width { get; }
    public int Height { get; }

    public float[,] HeightMap { get; }
    public float HeightMultiplier { get; }

    public List<Triangle> DelaunayTriangles { get; }

    public Vector3[] Vertices { get; }
    public int[] Triangles { get; }
    public Vector2[] Uvs { get; set; }

    public MeshData(List<Triangle> delaunayTriangles, Vector3[] vertices, int[] triangles, Vector2[] uvs)
    {
        DelaunayTriangles = delaunayTriangles;
        Vertices = vertices;
        Triangles = triangles;
        Uvs = uvs;
    }

    public MeshData(float[,] heightMap, float heightMultiplier)
    {
        Width = heightMap.GetLength(0);
        Height = heightMap.GetLength(1);

        HeightMap = heightMap;
        HeightMultiplier = heightMultiplier;

        Vertices = new Vector3[Width * Height];
        Uvs = new Vector2[Width * Height];

        // 6 because there are 2 triangles per quad, and each has 3 vertices
        Triangles = new int[(Width - 1) * (Height - 1) * 6];
    }
}