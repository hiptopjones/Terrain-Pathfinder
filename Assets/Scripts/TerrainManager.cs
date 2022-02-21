using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private Dictionary<MeshCollider, TerrainChunk> ColliderTerrainChunkMapping { get; set; } = new Dictionary<MeshCollider, TerrainChunk>();

    public void AddTerrainChunk(MeshCollider meshCollider, TerrainChunk terrainChunk)
    {
        ColliderTerrainChunkMapping[meshCollider] = terrainChunk;
    }

    public TerrainChunk GetTerrainChunk(MeshCollider meshCollider)
    {
        return ColliderTerrainChunkMapping[meshCollider];
    }
}
