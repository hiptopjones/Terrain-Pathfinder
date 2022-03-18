using ceometric.DelaunayTriangulator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateNavigationMesh(List<Triangle> delauneyTriangles)
    {
        // Take the 2-D points from the triangle vertices and return them to Vector3 points with height values
        //  - The height data is lost in preparation for triangulation
        //  - Can they be passed through in the Z?  I think the triangulation library ignores Z
        // Generate a list of the unique Vector3 points for vertices
        // Create a triangles array that indexes the Vector3 points
        // Don't need any UV mapping for this, since it won't be textured

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Used to keep track of the index of vertices in case of duplicates
        Dictionary<Vector3, int> vertexIndexes = new Dictionary<Vector3, int>();

        foreach (Triangle triangle in delauneyTriangles)
        {
            Vector3 vertex1 = new Vector3(triangle.Vertex1.X, triangle.Vertex1.Z, triangle.Vertex1.Y); // Swap Y and Z back after triangulation
            int index1;
            if (!vertexIndexes.TryGetValue(vertex1, out index1))
            {
                index1 = vertices.Count;
                vertices.Add(vertex1);
            }
            triangles.Add(index1);

            Vector3 vertex2 = new Vector3(triangle.Vertex2.X, triangle.Vertex2.Z, triangle.Vertex2.Y); // Swap Y and Z back after triangulation
            int index2;
            if (!vertexIndexes.TryGetValue(vertex2, out index2))
            {
                index2 = vertices.Count;
                vertices.Add(vertex2);
            }
            triangles.Add(index2);

            Vector3 vertex3 = new Vector3(triangle.Vertex3.X, triangle.Vertex3.Z, triangle.Vertex3.Y); // Swap Y and Z back after triangulation
            int index3;
            if (!vertexIndexes.TryGetValue(vertex3, out index3))
            {
                index3 = vertices.Count;
                vertices.Add(vertex3);
            }
            triangles.Add(index3);
        }

        MeshData meshData = new MeshData(vertices.ToArray(), triangles.ToArray());
        return meshData;
    }

    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier)
    {
        MeshData meshData = new MeshData(heightMap, heightMultiplier);
        int width = meshData.Width;
        int height = meshData.Height;

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float heightValue = heightMap[x, y] * heightMultiplier;

                meshData.Vertices[vertexIndex] = new Vector3(x, heightValue, y);

                // No triangles on the last row/column of vertices
                if (x < width - 1 && y < height -1)
                {
                    // Consistent triangle winding is important for consistent face culling

                    // Left triangle of quad
                    meshData.Triangles[triangleIndex++] = vertexIndex;
                    meshData.Triangles[triangleIndex++] = vertexIndex + width;
                    meshData.Triangles[triangleIndex++] = vertexIndex + width + 1;

                    // Right triangle of quad
                    meshData.Triangles[triangleIndex++] = vertexIndex;
                    meshData.Triangles[triangleIndex++] = vertexIndex + width + 1;
                    meshData.Triangles[triangleIndex++] = vertexIndex + 1;
                }

                meshData.Uvs[vertexIndex] = new Vector2((float)x / width, (float)y / height);

                vertexIndex++;
            }
        }

        return meshData;
    }
}
