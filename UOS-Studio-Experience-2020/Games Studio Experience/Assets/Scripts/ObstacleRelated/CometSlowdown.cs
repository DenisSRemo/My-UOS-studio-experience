using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometSlowdown : MonoBehaviour
{
    public Rigidbody ObjectBody;
    private void Start()
    {
        ObjectBody = this.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Get objects rigidbody
        ObjectBody = other.gameObject.GetComponent<Rigidbody>();
        //If object has body, increase drag
        if (ObjectBody != null)
        {
            ObjectBody.drag += 6f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Get rigidbody
        ObjectBody = other.gameObject.GetComponent<Rigidbody>();
        //If object has body, decrease drag
        if (ObjectBody != null)
        {
            ObjectBody.drag -= 6f;
        }
    }
}
