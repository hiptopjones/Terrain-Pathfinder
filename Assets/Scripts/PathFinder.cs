using ceometric.DelaunayTriangulator;
using Dcel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // Start position of the path, local to the terrain mesh
    private Vector3 StartVertex { get; set; }

    // End position of the path, local to the terrain mesh
    private Vector3 EndVertex { get; set; }

    // Assumes that the start and end are in the same terrain mesh
    // or at least that the terrain meshes have the same world transform
    private Transform WorldTransform { get; set; }
    private MeshData MeshData { get; set; }

    private bool IsPathStarted { get; set; }
    private bool IsPathEnded { get; set; }
    private AStarAlgorithm Algorithm { get; set; }

    private List<Vector3> Path { get; set; }
    private List<Vector3> SmoothedPath { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        MeshPicker meshPicker = FindObjectOfType<MeshPicker>();
        meshPicker.OnPrimaryVertexSelected = OnPrimaryVertexSelected;
        meshPicker.OnSecondaryVertexSelected = OnSecondaryVertexSelected;
        meshPicker.OnSelectedVerticesCleared = OnSelectedVerticesCleared;
    }

    private void OnPrimaryVertexSelected(Vector3 vertexPosition, Transform worldTransform, MeshData meshData)
    {
        StartVertex = vertexPosition;
        WorldTransform = worldTransform;
        MeshData = meshData;

        IsPathStarted = true;
    }

    private void OnSecondaryVertexSelected(Vector3 vertexPosition, Transform worldTransform, MeshData meshData)
    {
        EndVertex = vertexPosition;
        WorldTransform = worldTransform;
        MeshData = meshData;

        IsPathEnded = true;
    }

    private void OnSelectedVerticesCleared()
    {
        IsPathStarted = false;
        IsPathEnded = false;
        Path = null;
        SmoothedPath = null;

        if (Algorithm != null)
        {
            Destroy(Algorithm);
            Algorithm = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPathStarted || !IsPathEnded)
        {
            // No path endpoints selected
            return;
        }

        if (Algorithm == null)
        {
            Algorithm = gameObject.AddComponent<AStarAlgorithm>();
            Algorithm.StartVertex = StartVertex;
            Algorithm.EndVertex = EndVertex;
            Algorithm.WorldTransform = WorldTransform;
            Algorithm.MeshData = MeshData;
        }

        if (Algorithm.IsPathValid && Path == null)
        {
            Path = Algorithm.Path;
            SmoothedPath = SplineSmooth.Smooth(Path);
        }
    }

    private void OnDrawGizmos()
    {
        if (IsPathStarted)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(WorldTransform.TransformPoint(StartVertex), 1f);
        }

        if (IsPathEnded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(WorldTransform.TransformPoint(EndVertex), 1f);
        }

        if (IsPathStarted && IsPathEnded)
        {
            Debug.DrawLine(
                WorldTransform.TransformPoint(StartVertex),
                WorldTransform.TransformPoint(EndVertex),
                Color.yellow);
        }

        if (SmoothedPath != null)
        {
            Gizmos.color = Color.cyan;

            Vector3 previousVertex = SmoothedPath.First();
            foreach (Vector3 currentVertex in SmoothedPath.Skip(1))
            {
                Gizmos.DrawLine(
                    WorldTransform.TransformPoint(previousVertex),
                    WorldTransform.TransformPoint(currentVertex));

                previousVertex = currentVertex;
            }
        }
    }
}
