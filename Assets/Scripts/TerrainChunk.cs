using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TerrainChunk : MonoBehaviour
{
    [SerializeField]
    private Texture2D noiseMap;

    [SerializeField]
    private float heightMultiplier;

    [HideInInspector]
    public MeshData meshData;

    private void Awake()
    {
        float[,] heightMap = new float[noiseMap.width, noiseMap.height];

        Color[] colorMap = noiseMap.GetPixels();
        for (int y = 0; y < noiseMap.height; y++)
        {
            for (int x = 0; x < noiseMap.width; x++)
            {
                // R, G, B change uniformly when lerping from black to white, so just pick one to inverse lerp
                heightMap[x, y] = Mathf.Clamp01(Mathf.InverseLerp(Color.black.r, Color.white.r, colorMap[y * noiseMap.width + x].r));
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
