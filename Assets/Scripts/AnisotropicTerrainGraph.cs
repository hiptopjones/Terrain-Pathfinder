using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnisotropicTerrainGraph : ITerrainGraph
{
    private int Width { get; }
    private int Height { get; }
    private Vector3[] Vertices { get; }

    private Vector3[] NeighborOffsets { get; }

    public AnisotropicTerrainGraph(int width, int height, Vector3[] vertices, int neighborDistance)
    {
        Width = width;
        Height = height;
        Vertices = vertices;

        NeighborOffsets = CalculateNeighborOffsets(neighborDistance);
    }

    private Vector3[] CalculateNeighborOffsets(int neighborDistance)
    {
        List<Vector3> neighborsOffsets = new List<Vector3>();

        for (int x = 0; x < neighborDistance; x++)
        {
            for (int y = 0; y < neighborDistance; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int gcd = CalculateGcd(x, y);
                if (gcd == 1)
                {
                    neighborsOffsets.Add(new Vector3(x, 0, y));
                    neighborsOffsets.Add(new Vector3(-x, 0, y));
                    neighborsOffsets.Add(new Vector3(x, 0, -y));
                    neighborsOffsets.Add(new Vector3(-x, 0, -y));
                }
            }
        }

        return neighborsOffsets.ToArray();
    }

    // Using Euclid's algorithm
    private int CalculateGcd(int a, int b)
    {
        if (a == 0)
        {
            return b;
        }

        if (b == 0)
        {
            return a;
        }

        a = Mathf.Abs(a);
        b = Mathf.Abs(b);

        if (a < b)
        {
            int t = a;
            a = b;
            b = t;
        }

        int r = a % b;
        return CalculateGcd(b, r);
    }

    public float GetCost(Vector3 previousPosition, Vector3 currentPosition, Vector3 nextPosition)
    {
        // Basic movement cost formula from "Extensions to least cost path algorithms for roadway planning"
        // movement cost = surface distance * (average cell cost + slope angle * slope weight)
        // surface distance = sqrt(gridX*gridX + gridY*gridY + height*height)
        // average cell cost = isotropic, intrinsic cost assigned to a cell based on land type / passability
        // slope angle = arctan(height distance / grid distance)
        // slope weight = steeper slopes are significantly more costly, and slopes over a threshold are impassable.

        float surfaceDistance = Mathf.Sqrt(
            (currentPosition.x - nextPosition.x) * (currentPosition.x - nextPosition.x) +
            (currentPosition.y - nextPosition.y) * (currentPosition.y - nextPosition.y) + 
            (currentPosition.z - nextPosition.z) * (currentPosition.z - nextPosition.z));

        float averageCellCost = 1;

        float rise = Mathf.Abs(currentPosition.y - nextPosition.y);
        float run = Mathf.Sqrt(
            (currentPosition.x - nextPosition.x) * (currentPosition.x - nextPosition.x) +
            (currentPosition.z - nextPosition.z) * (currentPosition.z - nextPosition.z));
        float slopeAngle = Mathf.Abs(Mathf.Atan(rise / run)) * Mathf.Rad2Deg;

        float slopeWeight;
        if (slopeAngle > 16)
        {
            slopeWeight = float.MaxValue;
        }
        else if (slopeAngle > 12)
        {
            slopeWeight = 4;
        }
        else if (slopeAngle > 6)
        {
            slopeWeight = 1;
        }
        else
        {
            slopeWeight = 0;
        }

        Vector3 currentDirection = currentPosition - previousPosition;
        Vector3 nextDirection = nextPosition - currentPosition;
        float turnAngle = Vector3.Angle(currentDirection, nextDirection);

        float turnWeight;
        if (turnAngle > 90)
        {
            turnWeight = 8;
        }
        else if (turnAngle > 45)
        {
            turnWeight = 4;
        }
        else if (turnAngle > 30)
        {
            turnWeight = 2;
        }
        else if (turnAngle > 15)
        {
            turnWeight = 1;
        }
        else
        {
            turnWeight = 0;
        }

        float movementCost = surfaceDistance * (averageCellCost + slopeAngle * slopeWeight) + turnWeight;
        return movementCost;
    }

    public IEnumerable<Vector3> GetNeighborVertices(Vector3 position)
    {
        foreach (Vector3 neighborOffset in NeighborOffsets)
        {
            // X and Z are the coordinates of the height map, Y is the height value
            int neighborX = Mathf.RoundToInt(position.x) + Mathf.RoundToInt(neighborOffset.x);
            int neighborZ = Mathf.RoundToInt(position.z) + Mathf.RoundToInt(neighborOffset.z);

            // Test whether the direction is valid
            if (neighborX >= 0 && neighborX <= Width - 1 &&
                neighborZ >= 0 && neighborZ <= Height - 1)
            {
                int neighborIndex = neighborZ * Width + neighborX;
                Vector3 neighborVertex = Vertices[neighborIndex];
                yield return neighborVertex;
            }
        }
    }
}
