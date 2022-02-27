using UnityEngine;
using System.Collections;
using System;

// https://docs.unity3d.com/ScriptReference/RaycastHit-triangleIndex.html
// https://answers.unity.com/questions/1305031/pinpointing-one-vertice-with-raycasthit.html

public class MeshPicker : MonoBehaviour
{
    [SerializeField]
    private Camera pickingCamera;

    public Action<Vector3, Transform, MeshData> OnPrimaryVertexSelected;
    public Action<Vector3, Transform, MeshData> OnSecondaryVertexSelected;
    public Action<Vector3, Transform, MeshData> OnVertexHovered;

    private TerrainManager terrainManager;

    private Vector3 hitVertex;
    private Transform hitTransform;

    private void Start()
    {
        terrainManager = FindObjectOfType<TerrainManager>();
    }

    private void Update()
    {
        RaycastHit hit;
        if (!Physics.Raycast(pickingCamera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            return;
        }

        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return;
        }

        // Get the original mesh data
        TerrainChunk terrainChunk = terrainManager.GetTerrainChunk(meshCollider);
        MeshData meshData = terrainChunk.meshData;

        HighlightTriangle(hit, meshData);

        // Vertex in local coordinates
        hitVertex = GetClosestVertex(hit, meshData);
        hitTransform = hit.collider.transform;

        NotifyVertex(hitVertex, hitTransform, meshData);
    }

    private void NotifyVertex(Vector3 hitVertex, Transform hitTransform, MeshData meshData)
    {
        if (Input.GetMouseButtonDown(0)) // Left button
        {
            if (OnPrimaryVertexSelected != null)
            {
                OnPrimaryVertexSelected(hitVertex, hitTransform, meshData);
            }
        }
        else if (Input.GetMouseButtonDown(1)) // Right Button
        {
            if (OnSecondaryVertexSelected != null)
            {
                OnSecondaryVertexSelected(hitVertex, hitTransform, meshData);
            }
        }

        if (OnVertexHovered != null)
        {
            OnVertexHovered(hitVertex, hitTransform, meshData);
        }
    }

    // Draw a debug line around the triangle containing the hit point
    public void HighlightTriangle(RaycastHit hit, MeshData meshData)
    {
        Vector3[] vertices = meshData.Vertices;
        int[] triangles = meshData.Triangles;

        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];

        hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        
        Debug.DrawLine(p0, p1, Color.red);
        Debug.DrawLine(p1, p2, Color.red);
        Debug.DrawLine(p2, p0, Color.red);
    }

    // Draws a sphere at the vertex closest to the hit point
    public Vector3 GetClosestVertex(RaycastHit hit, MeshData meshData)
    {
        var barycentricCoordinate = hit.barycentricCoordinate;

        Vector3[] vertices = meshData.Vertices;
        int[] triangles = meshData.Triangles;

        int vertexIndex;
        
        // The vertex index is determined by which barycentric coordinate component is largest
        if (barycentricCoordinate.x > barycentricCoordinate.y && barycentricCoordinate.x > barycentricCoordinate.z)
        {
            vertexIndex = triangles[hit.triangleIndex * 3]; // x
        }
        else if (barycentricCoordinate.y > barycentricCoordinate.z)
        {
            vertexIndex = triangles[hit.triangleIndex * 3 + 1]; // y
        }
        else
        {
            vertexIndex = triangles[hit.triangleIndex * 3 + 2]; // z
        }

        return vertices[vertexIndex];
    }

    private void OnDrawGizmos()
    {
        // No vertex picking happened yet
        if (hitTransform == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hitTransform.TransformPoint(hitVertex), 1f);
    }
}