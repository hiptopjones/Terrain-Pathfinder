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

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap, heightMultiplier);

        Mesh mesh = new Mesh();
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.triangles;
        mesh.uv = meshData.uv;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
