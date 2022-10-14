using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityForRedGiant : MonoBehaviour
{


    public Rigidbody rb;
 




    void FixedUpdate()
    {
       Lanching_ball player= FindObjectOfType<Lanching_ball>();

        Attract(player);
    }

    void Attract( Lanching_ball obj)
    {
        Rigidbody rbtoAttract = obj.rb;
        Vector3 direction = rb.position - rbtoAttract.position;
        float distance = direction.magnitude;

        float forceMagnitude = (rb.mass * rbtoAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;


        rbtoAttract.AddForce(force);
    }























































    void Start()
    {
        
    }

    
    void Update()
    {
        transform.Rotate(0, 0.5f, 0);
    }


}
