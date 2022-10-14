using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstronautScript : MonoBehaviour
{
    private float SpinSpeed;

    private void Start()
    {
        SpinSpeed = 0.05f;
    }

    private void Update()
    {
        transform.Rotate(0, 0, SpinSpeed);
    }
}
