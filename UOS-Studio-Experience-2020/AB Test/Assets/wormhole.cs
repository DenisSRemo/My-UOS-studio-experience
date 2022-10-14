using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wormhole : MonoBehaviour
{
    public Transform exitPortal;
    static Transform thisPortal;
    private Vector3 playerVelocity;



    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            if (thisPortal == exitPortal)
                return;
            thisPortal = this.transform;
            playerVelocity = col.GetComponent<Rigidbody>().velocity;
            col.transform.position = exitPortal.transform.position;
        }
    }


    void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            if (exitPortal == thisPortal)
            {
                thisPortal = null;
            }
            col.GetComponent<Rigidbody>().velocity = playerVelocity;
        }
           
        
    }
}
