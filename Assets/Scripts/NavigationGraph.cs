using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationGraph
{
    public MeshData MeshData { get; }

    public int ContourCount { get; }

    Dictionary<int, HashSet<Vector3>> ContourBuckets { get; set; }

    public NavigationGraph(MeshData meshData, int contourCount)
    {
        MeshData = meshData;
        ContourCount = contourCount;
    }

    public List<Vector3> GenerateContours()
    {
        RunConrec();

        List<Vector3> navigationVertices = new List<Vector3>();

        foreach (int contourBucketId in ContourBuckets.Keys)
        {
            navigationVertices.AddRange(ContourBuckets[contourBucketId]);
        }

        return navigationVertices;
    }

    private void RunConrec()
    {
        // Create contour buckets
        ContourBuckets = new Dictionary<int, HashSet<Vector3>>();
        for (int i = 0; i < ContourCount; i++)
        {
            ContourBuckets[i] = new HashSet<Vector3>();
        }

        // Run CONREC implementation
        float[,] d = MeshData.HeightMap;
        float[] x = Enumerable.Range(0, 100).Select(x => (float)x).ToArray();
        float[] y = Enumerable.Range(0, 100).Select(x => (float)x).ToArray();
        float[] z = Enumerable.Range(0, ContourCount).Select(x => (float)x / ContourCount).ToArray();

        Conrec.Contour(d, x, y, z, OnContourSegment);
    }

    private void OnContourSegment(float x1, float y1, float x2, float y2, float z)
    {
        // Add returned vertices to the correct bucket
        int bucketId = Mathf.RoundToInt(z * ContourCount);
        ContourBuckets[bucketId].Add(new Vector3((x1 + x2) / 2, 100, (y1 + y2) / 2));
    }
}
