using System;

namespace ceometric.DelaunayTriangulator
{
    /// <summary>A class defining a triangle and some methods for the Delaunay algorithm.</summary>
    public class Triangle
    {
        /// <summary>The first vertex of the triangle.</summary>
        public Point Vertex1;
        /// <summary>The second vertex of the triangle.</summary>
        public Point Vertex2;
        /// <summary>The third vertex of the triangle.</summary>
        public Point Vertex3;

        #region Constructor

        /// <summary>Constructs a triangle from three points.</summary>
        /// <param name="vertex1">The first vertex of the triangle.</param>
        /// <param name="vertex2">The second vertex of the triangle.</param>
        /// <param name="vertex3">The third vertex of the triangle.</param>
        public Triangle(Point vertex1, Point vertex2, Point vertex3)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Vertex3 = vertex3;
        }

        #endregion

        #region Methods

        /// <summary>Tests if a point lies in the circumcircle of the triangle.</summary>
        /// <param name="point">A <see cref="Point"/>.</param>
        /// <returns>For a counterclockwise order of the vertices of the triangle, this test is 
        /// <list type ="bullet">
        /// <item>positive if <paramref name="point"/> lies inside the circumcircle.</item>
        /// <item>zero if <paramref name="point"/> lies on the circumference of the circumcircle.</item>
        /// <item>negative if <paramref name="point"/> lies outside the circumcircle.</item></list></returns>
        /// <remarks>The vertices of the triangle must be arranged in counterclockwise order or the result
        /// of this test will be reversed. This test ignores the z-coordinate of the vertices.</remarks>
        public float ContainsInCircumcircle(Point point)
        {
            float ax = this.Vertex1.X - point.X;
            float ay = this.Vertex1.Y - point.Y;
            float bx = this.Vertex2.X - point.X;
            float by = this.Vertex2.Y - point.Y;
            float cx = this.Vertex3.X - point.X;
            float cy = this.Vertex3.Y - point.Y;

            float det_ab = ax * by - bx * ay;
            float det_bc = bx * cy - cx * by;
            float det_ca = cx * ay - ax * cy;

            float a_squared = ax * ax + ay * ay;
            float b_squared = bx * bx + by * by;
            float c_squared = cx * cx + cy * cy;

            return a_squared * det_bc + b_squared * det_ca + c_squared * det_ab;
        }

        /// <summary>Tests if two triangles share at least one vertex.</summary>
        /// <param name="triangle">A <see cref="Triangle"/>.</param>
        /// <returns>Returns true if two triangles share at least one vertex, false otherwise.</returns>
        public bool SharesVertexWith(Triangle triangle)
        {
            if (this.Vertex1.X == triangle.Vertex1.X && this.Vertex1.Y == triangle.Vertex1.Y) return true;
            if (this.Vertex1.X == triangle.Vertex2.X && this.Vertex1.Y == triangle.Vertex2.Y) return true;
            if (this.Vertex1.X == triangle.Vertex3.X && this.Vertex1.Y == triangle.Vertex3.Y) return true;

            if (this.Vertex2.X == triangle.Vertex1.X && this.Vertex2.Y == triangle.Vertex1.Y) return true;
            if (this.Vertex2.X == triangle.Vertex2.X && this.Vertex2.Y == triangle.Vertex2.Y) return true;
            if (this.Vertex2.X == triangle.Vertex3.X && this.Vertex2.Y == triangle.Vertex3.Y) return true;

            if (this.Vertex3.X == triangle.Vertex1.X && this.Vertex3.Y == triangle.Vertex1.Y) return true;
            if (this.Vertex3.X == triangle.Vertex2.X && this.Vertex3.Y == triangle.Vertex2.Y) return true;
            if (this.Vertex3.X == triangle.Vertex3.X && this.Vertex3.Y == triangle.Vertex3.Y) return true;

            return false;
        }

        #endregion
    }
}
