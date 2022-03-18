using ceometric.DelaunayTriangulator;
using Dcel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DcelGenerator
{
    public static Dictionary<Vector3, Vertex> CreateDcelFromDelaunay(List<Triangle> triangles)
    {
        Dictionary<Vector3, Vertex> vertexMapping = new Dictionary<Vector3, Vertex>();
        Dictionary<string, HalfEdge> edgeMapping = new Dictionary<string, HalfEdge>();

        // https://cs.stackexchange.com/questions/2450/how-do-i-construct-a-doubly-connected-edge-list-given-a-set-of-line-segments
        foreach (Triangle triangle in triangles)
        {
            Vector3 vertexCoordinates1 = new Vector3(triangle.Vertex1.X, triangle.Vertex1.Z, triangle.Vertex1.Y); // Swap Y and Z back after triangulation
            Vertex vertex1;
            if (!vertexMapping.TryGetValue(vertexCoordinates1, out vertex1))
            {
                vertex1 = new Vertex
                {
                    Coordinates = vertexCoordinates1
                };

                vertexMapping[vertexCoordinates1] = vertex1;
            }

            Vector3 vertexCoordinates2 = new Vector3(triangle.Vertex2.X, triangle.Vertex2.Z, triangle.Vertex2.Y); // Swap Y and Z back after triangulation
            Vertex vertex2;
            if (!vertexMapping.TryGetValue(vertexCoordinates2, out vertex2))
            {
                vertex2 = new Vertex
                {
                    Coordinates = vertexCoordinates2
                };

                vertexMapping[vertexCoordinates2] = vertex2;
            }

            Vector3 vertexCoordinates3 = new Vector3(triangle.Vertex3.X, triangle.Vertex3.Z, triangle.Vertex3.Y); // Swap Y and Z back after triangulation
            Vertex vertex3;
            if (!vertexMapping.TryGetValue(vertexCoordinates3, out vertex3))
            {
                vertex3 = new Vertex
                {
                    Coordinates = vertexCoordinates3
                };

                vertexMapping[vertexCoordinates3] = vertex3;
            }

            string edgeKey12 = vertexCoordinates1.ToString() + vertexCoordinates2.ToString();
            string edgeKey21 = vertexCoordinates2.ToString() + vertexCoordinates1.ToString();

            string edgeKey23 = vertexCoordinates2.ToString() + vertexCoordinates3.ToString();
            string edgeKey32 = vertexCoordinates3.ToString() + vertexCoordinates2.ToString();

            string edgeKey31 = vertexCoordinates3.ToString() + vertexCoordinates1.ToString();
            string edgeKey13 = vertexCoordinates1.ToString() + vertexCoordinates3.ToString();

            HalfEdge halfEdge1;
            if (!edgeMapping.TryGetValue(edgeKey12, out halfEdge1))
            {
                // If edge was not found, create both directions and connect them
                HalfEdge halfEdge12 = new HalfEdge
                {
                    Origin = vertex1
                };

                HalfEdge halfEdge21 = new HalfEdge
                {
                    Origin = vertex2
                };

                edgeMapping[edgeKey12] = halfEdge12;
                edgeMapping[edgeKey21] = halfEdge21;

                halfEdge12.Twin = halfEdge21;
                halfEdge21.Twin = halfEdge12;

                halfEdge1 = halfEdge12;
            }

            HalfEdge halfEdge2;
            if (!edgeMapping.TryGetValue(edgeKey23, out halfEdge2))
            {
                // If edge was not found, create both directions and connect them
                HalfEdge halfEdge23 = new HalfEdge
                {
                    Origin = vertex2
                };

                HalfEdge halfEdge32 = new HalfEdge
                {
                    Origin = vertex3
                };

                edgeMapping[edgeKey23] = halfEdge23;
                edgeMapping[edgeKey32] = halfEdge32;

                halfEdge23.Twin = halfEdge32;
                halfEdge32.Twin = halfEdge23;

                halfEdge2 = halfEdge23;
            }

            HalfEdge halfEdge3;
            if (!edgeMapping.TryGetValue(edgeKey31, out halfEdge3))
            {
                // If edge was not found, create both directions and connect them
                HalfEdge halfEdge31 = new HalfEdge
                {
                    Origin = vertex3
                };

                HalfEdge halfEdge13 = new HalfEdge
                {
                    Origin = vertex1
                };

                edgeMapping[edgeKey31] = halfEdge31;
                edgeMapping[edgeKey13] = halfEdge13;

                halfEdge31.Twin = halfEdge13;
                halfEdge13.Twin = halfEdge31;

                halfEdge3 = halfEdge31;
            }

            if (vertex1.IncidentEdge == null)
            {
                vertex1.IncidentEdge = halfEdge1;
            }

            if (vertex2.IncidentEdge == null)
            {
                vertex2.IncidentEdge = halfEdge2;
            }

            if (vertex3.IncidentEdge == null)
            {
                vertex3.IncidentEdge = halfEdge3;
            }

            halfEdge1.Next = halfEdge2;
            halfEdge2.Next = halfEdge3;
            halfEdge3.Next = halfEdge1;

            halfEdge1.Previous = halfEdge3;
            halfEdge2.Previous = halfEdge1;
            halfEdge3.Previous = halfEdge2;

            Face face = new Face
            {
                OuterComponent = halfEdge1
            };

            halfEdge1.IncidentFace = face;
            halfEdge2.IncidentFace = face;
            halfEdge3.IncidentFace = face;
        }

        return vertexMapping;
    }
}
