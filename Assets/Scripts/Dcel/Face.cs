using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dcel
{
    public class Face
    {
        public HalfEdge InnerComponent { get; set; }
        public HalfEdge OuterComponent { get; set; }
    }
}
