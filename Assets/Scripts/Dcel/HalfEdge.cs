using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dcel
{
    public class HalfEdge
    {
        public Vertex Origin { get; set; }
        public HalfEdge Twin { get; set; }
        public Face IncidentFace { get; set; }
        public HalfEdge Next { get; set; }
        public HalfEdge Previous { get; set; }
    }
}
