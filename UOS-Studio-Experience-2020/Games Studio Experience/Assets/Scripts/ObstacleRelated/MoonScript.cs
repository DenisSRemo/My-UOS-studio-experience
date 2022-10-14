using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonScript : MonoBehaviour
{
    public float OrbitDistance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = this.transform.root.position + (transform.position - this.transform.root.position).normalized * OrbitDistance;

        this.transform.RotateAround(this.transform.root.position, Vector3.up, 20 * Time.deltaTime);
    }
}
