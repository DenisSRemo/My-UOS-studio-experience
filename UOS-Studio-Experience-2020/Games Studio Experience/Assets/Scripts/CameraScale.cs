using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour
{
    public MeshCollider ObjectToScaleTo;


    private void Awake()
    {
        float TargetSize = ObjectToScaleTo.bounds.size.x / ObjectToScaleTo.bounds.size.y;

        Camera.main.orthographicSize = ObjectToScaleTo.bounds.size.y / 2 * TargetSize;
    }
}
