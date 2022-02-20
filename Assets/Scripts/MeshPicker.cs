using UnityEngine;
using System.Collections;
using System;

// https://docs.unity3d.com/ScriptReference/RaycastHit-triangleIndex.html
// https://answers.unity.com/questions/1305031/pinpointing-one-vertice-with-raycasthit.html

public class MeshPicker : MonoBehaviour
{
    [SerializeField]
    private Camera pickingCamera;

    [HideInInspector]
    public Vector3 primarySelectedVertex;

    [HideInInspector]
    public Vector3 secondarySelectedVertex;

    [HideInInspector]
    public Vector3 hoveredVertex;

    [HideInInspector]
    public Vector3[] vertices;

    void Update()
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

        Mesh mesh = meshCollider.sharedMesh;

        HighlightTriangle(hit, mesh);
        HighlightClosestVertex(hit, mesh);
    }

    // Draw a debug line around the triangle containing the hit point
    public void HighlightTriangle(RaycastHit hit, Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        
        Debug.DrawLine(p0, p1, Color.red);
        Debug.DrawLine(p1, p2, Color.red);
        Debug.DrawLine(p2, p0, Color.red);
    }

    // Draws a sphere at the vertex closest to the hit point
    public void HighlightClosestVertex(RaycastHit hit, Mesh mesh)
    {
        var barycentricCoordinate = hit.barycentricCoordinate;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

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

        Transform hitTransform = hit.collider.transform;
        Vector3 vertex = hitTransform.TransformPoint(vertices[vertexIndex]);

        if (Input.GetMouseButtonDown(0))
        {
            primarySelectedVertex = vertex;

            RunHeapTest();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            secondarySelectedVertex = vertex;
        }
        else
        {
            hoveredVertex = vertex;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(primarySelectedVertex, 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(secondarySelectedVertex, 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hoveredVertex, 0.5f);
    }

    public class HeapItem : IComparable<HeapItem>
    {
        public int Value { get; set; }

        public int CompareTo(HeapItem other)
        {
            return Value.CompareTo(other.Value);
        }
    }

    private void RunHeapTest()
    {
        System.Random random = new System.Random((int)Time.time);

        int maxHeapSize = 10;
        MinHeap<HeapItem> heap = new MinHeap<HeapItem>(maxHeapSize);

        int heapSize = random.Next(maxHeapSize + 1); // boundary is exclusive
        for (int i = 0; i < heapSize; i++)
        {
            HeapItem item = new HeapItem
            {
                Value = random.Next(0, 100)
            };
            heap.Add(item);

            Debug.Log("Adding item: " + item.Value);
        }

        for (int i = 0; i < heapSize; i++)
        {
            HeapItem item = heap.RemoveMin();
            Debug.Log("Removing min item: " + item.Value);
        }
    }
}