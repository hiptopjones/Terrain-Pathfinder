using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.habrador.com/tutorials/interpolation/1-catmull-rom-splines/
public static class SplineSmooth
{
    public static List<Vector3> Smooth(List<Vector3> path)
    {
        List<Vector3> smoothedPath = new List<Vector3>();

        // TODO: Could use direction from start to end to synthesize control points

        // The spline's resolution
        // Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
        float splineResolution = 0.2f;

        int numSteps = Mathf.RoundToInt(1 / splineResolution);

        for (int i = 1; i < path.Count - 3; i++)
        {
            Vector3 p0 = path[i - 1];
            Vector3 p1 = path[i];
            Vector3 p2 = path[i + 1];
            Vector3 p3 = path[i + 2];

            for (int j = 0; j < numSteps; j++)
            {
                float t = j * splineResolution;

                // Find the coordinate between the end points with a Catmull-Rom spline
                Vector3 splinePosition = GetCatmullRomPosition(t, p0, p1, p2, p3);
                smoothedPath.Add(splinePosition);
            }
        }

        return smoothedPath;
    }

    // Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
    // http://www.iquilezles.org/www/articles/minispline/minispline.htm
    private static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }
}
