using ceometric.DelaunayTriangulator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TerrainChunk : MonoBehaviour
{
    [SerializeField]
    private int noiseWidth;

    [SerializeField]
    private int noiseHeight;

    [SerializeField]
    private TextAsset heightTable;

    [SerializeField]
    private float heightMultiplier;

    public MeshData TerrainMeshData { get; set; }
    public MeshData NavigationMeshData { get; set; }

    private void Awake()
    {
        float[,] heightMap = LoadHeightMap();

        GenerateTerrainMesh(heightMap);
        GenerateNavigationMesh(TerrainMeshData);
    }

    private float[,] LoadHeightMap()
    {
        int width = noiseWidth;
        int height = noiseHeight;

        float[,] heightMap = new float[width, height];

        string[] lines = heightTable.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length != width * height)
        {
            throw new System.Exception($"Wrong number of lines in height table file - expected: {width * height} actual: {lines.Length}");
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                heightMap[x, y] = float.Parse(lines[y * width + x]);
            }
        }

        return heightMap;
    }

    private void GenerateTerrainMesh(float[,] heightMap)
    {
        // Create the structures for the mesh
        TerrainMeshData = MeshGenerator.GenerateTerrainMesh(heightMap, heightMultiplier);

        // Convert to a mesh and display
        //UpdateMesh(TerrainMeshData);
    }

    private void GenerateNavigationMesh(MeshData meshData)
    {
        // Contour the terrain
        ContourGenerator contourGenerator = new ContourGenerator(meshData, 20);
        List<Vector3> contourVertices = contourGenerator.GenerateContours();

        // Triangulate the resulting points
        DelaunayTriangulation2d triangulation = new DelaunayTriangulation2d();
        List<Triangle> triangles = triangulation.Triangulate(
            contourVertices.Select(p => new Point(p.x, p.z, p.y)).ToList()); // Swaps y and z for the call to Triangulate

        // Create the structures for the mesh
        NavigationMeshData = MeshGenerator.GenerateNavigationMesh(triangles);

        // Convert to a mesh and display
        DisplayMesh(NavigationMeshData);
    }

    private void DisplayMesh(MeshData meshData)
    {
        Mesh mesh = new Mesh
        {
            vertices = meshData.Vertices,
            triangles = meshData.Triangles,
            uv = meshData.Uvs
        };
        mesh.RecalculateNormals();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        TerrainManager terrainManager = FindObjectOfType<TerrainManager>();
        terrainManager.AddMeshData(meshCollider, meshData);
    }
}
