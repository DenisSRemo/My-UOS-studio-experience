using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToGround : MonoBehaviour
{
    private float WorldFloorHeight = 0.1f;
    public float CalcHeight;
    private void Start()
    {
        SphereCollider MyCollider = GetComponent<SphereCollider>();
        CalcHeight = ((MyCollider.transform.lossyScale.y) * MyCollider.radius)+WorldFloorHeight;
    }
    private void Update()
    {        
        transform.position = new Vector3(transform.position.x, CalcHeight, transform.position.z);
    }

}
