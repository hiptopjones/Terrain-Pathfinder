using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    [HideInInspector]
    public MeshData meshData;

    private void Awake()
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

        meshData = MeshGenerator.GenerateTerrainMesh(heightMap, heightMultiplier);
        UpdateMesh();
    }

    private void UpdateMesh()
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
        terrainManager.AddTerrainChunk(meshCollider, this);
    }
}
