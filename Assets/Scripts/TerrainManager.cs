using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    private Dictionary<MeshCollider, MeshData> ColliderTerrainChunkMapping { get; set; } = new Dictionary<MeshCollider, MeshData>();

    public void AddMeshData(MeshCollider meshCollider, MeshData meshData)
    {
        ColliderTerrainChunkMapping[meshCollider] = meshData;
    }

    public MeshData GetMeshData(MeshCollider meshCollider)
    {
        return ColliderTerrainChunkMapping[meshCollider];
    }
}
