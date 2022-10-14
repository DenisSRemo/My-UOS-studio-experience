using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometMovement : MonoBehaviour
{
    public float TargetX;
    public float TargetY;

    private Rigidbody ThisBody;
    private Vector3 TargetPos;

    // Start is called before the first frame update
    void Start()
    {
        //Get Rigidbody
        ThisBody = this.transform.GetComponent<Rigidbody>();
        //Point to target
        TargetPos = new Vector3(TargetX, TargetY, -0.6f);
        this.transform.LookAt(TargetPos);
        //Move towards
        ThisBody.AddForce(this.transform.root.forward * 5, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.y <= -1f)
        {
            Debug.Log("Comet Passed");
            //Stop Comet moving
            ThisBody.drag = 1000;
            //Rotate so collider is straight up
            this.transform.rotation = new Quaternion(0, 0, 0, 1);
            this.transform.GetChild(0).rotation = Quaternion.Euler(-90,0,0);
            //Change emmision rate on trail
            this.transform.GetChild(0).GetComponent<ParticleSystem>().emissionRate = 7;
            //Disable script
            this.enabled = false;
        }
    }
}
