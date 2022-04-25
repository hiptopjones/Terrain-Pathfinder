using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerrainGraph
{
    float GetCost(Vector3 previousPosition, Vector3 currentPosition, Vector3 nextPosition);
    IEnumerable<Vector3> GetNeighborVertices(Vector3 position);
}
