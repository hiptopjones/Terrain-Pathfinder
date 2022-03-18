using System;
using System.Collections.Generic;

// Credit to Paul Bourke (paul.bourke@uwa.edu.au) for the original Fortran 77 Program.
// Check out: http://local.wasp.uwa.edu.au/~pbourke/ 
// You can use this code however you like providing the above credits remain in tact.
// First converted to a standalone C# 2.0 library by Morten Nielsen (www.iter.dk)
// Performance enhanced C# 2.0 library by Christian Stelzl (www.ceometric.com) Sep. 2008

namespace ceometric.DelaunayTriangulator
{
    /// <summary>A 2d Delaunay triangulation class.</summary>
    /// <remarks>The triangulation doesn't support multiple points with identical x and y coordinates,
    /// nor does it support dublicate points.
    /// Vertex-lists with duplicate points may result in strange triangulations with intersecting edges or
    /// may cause the algorithm to fail. 
    /// Uses a simple O(n**2) algorithm based on Paul Bourke's "An Algorithm for Interpolating 
    /// Irregularly-Spaced Data with Applications in Terrain Modelling". 
    /// Uses an enhanced incircle-predicate for counterclockwise orientated triangles.</remarks>
    public class DelaunayTriangulation2d
    {
        /// <summary>Performs the 2d Delaunay triangulation on a set of n vertices in O(n**2) time.</summary>
        /// <param name="triangulationPoints">The points to triangulate.</param>
        /// <returns>A list of Delaunay-triangles.</returns>
        public List<Triangle> Triangulate(List<Point> triangulationPoints)
        {
            if (triangulationPoints.Count < 3) throw new ArgumentException("Can not triangulate less than three vertices!");

            // The triangle list
            List<Triangle> triangles = new List<Triangle>(); ;

            // The "supertriangle" which encompasses all triangulation points.
            // This triangle initializes the algorithm and will be removed later.
            Triangle superTriangle = this.SuperTriangle(triangulationPoints);
            triangles.Add(superTriangle);

            // Include each point one at a time into the existing triangulation
            for (int i = 0; i < triangulationPoints.Count; i++)
            {
                // Initialize the edge buffer.
                List<Edge> EdgeBuffer = new List<Edge>();

                // If the actual vertex lies inside the circumcircle, then the three edges of the 
                // triangle are added to the edge buffer and the triangle is removed from list.                             
                for (int j = triangles.Count - 1; j >= 0; j--)
                {
                    Triangle t = triangles[j];
                    if (t.ContainsInCircumcircle(triangulationPoints[i]) > 0)
                    {
                        EdgeBuffer.Add(new Edge(t.Vertex1, t.Vertex2));
                        EdgeBuffer.Add(new Edge(t.Vertex2, t.Vertex3));
                        EdgeBuffer.Add(new Edge(t.Vertex3, t.Vertex1));
                        triangles.RemoveAt(j);
                    }
                }

                // Remove duplicate edges. This leaves the convex hull of the edges.
                // The edges in this convex hull are oriented counterclockwise!
                for (int j = EdgeBuffer.Count - 2; j >= 0; j--)
                {
                    for (int k = EdgeBuffer.Count - 1; k >= j + 1; k--)
                    {
                        if (EdgeBuffer[j] == EdgeBuffer[k])
                        {
                            EdgeBuffer.RemoveAt(k);
                            EdgeBuffer.RemoveAt(j);
                            k--;
                            continue;
                        }
                    }
                }

                // Generate new counterclockwise oriented triangles filling the "hole" in
                // the existing triangulation. These triangles all share the actual vertex.
                for (int j = 0; j < EdgeBuffer.Count; j++)
                {
                    triangles.Add(new Triangle(EdgeBuffer[j].StartPoint, EdgeBuffer[j].EndPoint, triangulationPoints[i]));
                }
            }

            // We don't want the supertriangle in the triangulation, so
            // remove all triangles sharing a vertex with the supertriangle.
            for (int i = triangles.Count - 1; i >= 0; i--)
            {
                if (triangles[i].SharesVertexWith(superTriangle)) triangles.RemoveAt(i);
            }

            // Return the triangles
            return triangles;
        }

        /// <summary>Returns a triangle that encompasses all triangulation points.</summary>
        /// <param name="triangulationPoints">A list of triangulation points.</param>
        /// <returns>Returns a triangle that encompasses all triangulation points.</returns>
        private Triangle SuperTriangle(List<Point> triangulationPoints)
        {
            float M = triangulationPoints[0].X;

            // get the extremal x and y coordinates
            for (int i = 1; i < triangulationPoints.Count; i++)
            {
                float xAbs = Math.Abs(triangulationPoints[i].X);
                float yAbs = Math.Abs(triangulationPoints[i].Y);
                if (xAbs > M) M = xAbs;
                if (yAbs > M) M = yAbs;
            }

            // make a triangle
            Point sp1 = new Point(10 * M, 0, 0);
            Point sp2 = new Point(0, 10 * M, 0);
            Point sp3 = new Point(-10 * M, -10 * M, 0);

            return new Triangle(sp1, sp2, sp3);
        }
    }
}