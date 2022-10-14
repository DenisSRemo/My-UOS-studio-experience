using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBounce : MonoBehaviour
{
    private Rigidbody ThisBody;

    private Vector3 LastVelocity;

    // Start is called before the first frame update
    void Start()
    {
        ThisBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        LastVelocity = ThisBody.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "NeutronStar")
        {
            Debug.Log("Neut star: " + collision.gameObject.name);
            float Speed = LastVelocity.magnitude;
            Vector3 Reflection = Vector3.Reflect(LastVelocity.normalized, collision.contacts[0].normal);

            ThisBody.velocity = Reflection * Mathf.Max(Speed, 0);
        }
    }
}
