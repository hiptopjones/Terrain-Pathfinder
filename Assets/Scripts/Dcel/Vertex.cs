using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dcel
{
    public class Vertex
    {
        public Vector3 Coordinates { get; set; }
        public HalfEdge IncidentEdge { get; set; }
    }

}
