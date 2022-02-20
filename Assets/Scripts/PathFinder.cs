using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField]
    private MeshPicker meshPicker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 startPosition = meshPicker.primarySelectedVertex;
        Vector3 endPosition = meshPicker.secondarySelectedVertex;

        Debug.DrawLine(startPosition, endPosition, Color.yellow);
    }
}
