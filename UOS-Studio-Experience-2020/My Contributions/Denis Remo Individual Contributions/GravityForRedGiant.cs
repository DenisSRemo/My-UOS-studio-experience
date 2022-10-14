using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GravityForRedGiant : MonoBehaviour
{


    public Rigidbody rb;


    private GameObject player;
    private Rigidbody playerRB;
    [SerializeField] private float distanceBetweeen;
    [SerializeField]private float lineWidth;
    private LineRenderer Line;
    private int Segments = 360;
    private bool Attacted;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerRB = player.GetComponent<Rigidbody>();

        Line = GetComponent<LineRenderer>();
       
        Line.useWorldSpace = false;
        Line.startWidth = lineWidth;
        Line.endWidth = lineWidth;
        Line.positionCount = Segments + 1;
        float Radius = distanceBetweeen/2;
        var pointCount = Segments + 1;
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / Segments);
            points[i] = new Vector3(Mathf.Sin(rad) * Radius, 0, Mathf.Cos(rad) * Radius);
        }

        Line.SetPositions(points);
        Attacted = false;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= distanceBetweeen)
        {
            Attract(playerRB);
        }
    }

    void Attract( Rigidbody rbToAttract)
    {
        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;

        float forceMagnitude = (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;

        
        rbToAttract.AddForce(force);
     
    }

    void Update()
    {
        transform.Rotate(0, 0.5f, 0);
    }

    
}
