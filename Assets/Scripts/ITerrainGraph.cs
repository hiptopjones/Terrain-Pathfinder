using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerrainGraph
{
    float GetCost(Vector3 a, Vector3 b);
    IEnumerable<Vector3> GetNeighborVertices(Vector3 position);
}
