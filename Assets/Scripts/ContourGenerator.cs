using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Contour
{
    // First point we saw in the contour
    public Vector3 Start { get; set; }
    // Most recent point we added to the thinned contour
    public Vector3 Anchor { get; set; }
    // Most recent point we saw in the contour
    public Vector3 Head { get; set; }

    public List<Vector3> Points { get; set; } = new List<Vector3>();
}

public class ContourGenerator
{
    public MeshData MeshData { get; }

    public int ContourCount { get; }

    public HashSet<Vector3> ContourPoints { get; set; } = new HashSet<Vector3>();

    public List<Contour> Contours { get; set; } = new List<Contour>();

    public ContourGenerator(MeshData meshData, int contourCount)
    {
        MeshData = meshData;
        ContourCount = contourCount;
    }

    public List<Vector3> GenerateContours()
    {
        RunConrec();

        //return ContourPoints.ToList();
        return Contours.SelectMany(x => x.Points).ToList();
    }

    private void RunConrec()
    {
        // Run CONREC implementation
        float[,] d = MeshData.HeightMap;
        float[] x = Enumerable.Range(0, 100).Select(x => (float)x).ToArray();
        float[] y = Enumerable.Range(0, 100).Select(x => (float)x).ToArray();
        float[] z = Enumerable.Range(0, ContourCount).Select(x => (float)x / ContourCount).ToArray();
        Conrec.Contour(d, x, y, z, OnContourSegment);

        DetectContours();
    }

    private void OnContourSegment(float x1, float y1, float x2, float y2, float z)
    {
        ContourPoints.Add(new Vector3((x1 + x2) / 2, z * MeshData.HeightMultiplier, (y1 + y2) / 2));
    }

    // Sweeps the contour points from left to right, spacing the points evenly throughout the contour
    // This method keeps track of individual contour lines and assigns a point to a line based on proximity
    // There are cases where this breaks down, but it largely works fine
    private void DetectContours()
    {
        List<Contour> contours = new List<Contour>();

        // Sweep across all points by increasing X
        foreach (Vector3 point in ContourPoints.OrderBy(p => p.x))
        {
            bool isExistingContour = false;

            // If the number of contours is too big, time complexity suffers
            foreach (Contour contour in contours)
            {
                // Is the current point attached to this contour?
                if (Mathf.Abs(Vector3.Distance(contour.Head, point)) < 1)
                {
                    isExistingContour = true;
                    contour.Head = point;

                    // Is the current point far enough from the anchor to create a new anchor?
                    if (Mathf.Abs(Vector3.Distance(contour.Anchor, point)) > 2)
                    {
                        contour.Points.Add(point);
                        contour.Anchor = point;
                    }

                    break;
                }
            }

            // Point doesn't appear to be part any existing contour, so start a new one
            if (!isExistingContour)
            {
                Contour contour = new Contour
                {
                    Start = point,
                    Anchor = point,
                    Head = point,
                };
                contour.Points.Add(point);

                contours.Add(contour);
            }
        }

        // TODO: Check if any contours should be joined
        // TODO: What about contours that fork?

        Contours = contours;
    }
}
