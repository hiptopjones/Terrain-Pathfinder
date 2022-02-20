using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        MeshData meshData = new MeshData(width, height);

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float heightValue = heightMap[x, y] * heightMultiplier;

                meshData.vertices[vertexIndex] = new Vector3(x, heightValue, y);

                // No triangles on the last row/column of vertices
                if (x < width - 1 && y < height -1)
                {
                    // Consistent triangle winding is important for consistent face culling

                    // Left triangle of quad
                    meshData.triangles[triangleIndex++] = vertexIndex;
                    meshData.triangles[triangleIndex++] = vertexIndex + width;
                    meshData.triangles[triangleIndex++] = vertexIndex + width + 1;

                    // Right triangle of quad
                    meshData.triangles[triangleIndex++] = vertexIndex;
                    meshData.triangles[triangleIndex++] = vertexIndex + width + 1;
                    meshData.triangles[triangleIndex++] = vertexIndex + 1;
                }

                meshData.uv[vertexIndex] = new Vector2((float)x / width, (float)y / height);

                vertexIndex++;
            }
        }

        return meshData;
    }
}
