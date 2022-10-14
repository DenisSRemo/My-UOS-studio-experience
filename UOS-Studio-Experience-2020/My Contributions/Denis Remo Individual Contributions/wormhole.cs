using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class wormhole : MonoBehaviour
{
    [HideInInspector] public wormhole Link;

    private AudioSource ThisSource;
    public AudioClip EnterSound;
    public AudioClip ExitSound;

    private void Start()
    {
        ThisSource = GetComponent<AudioSource>();
    }
    
    public SphereCollider MyCollider;
    private List<GameObject> CurrentlyInZone = new List<GameObject>();
    void OnTriggerEnter(Collider col)
    {
        //Return if the object is already on the list of visitors
        if (CurrentlyInZone.Contains(col.gameObject) || Link.CurrentlyInZone.Contains(col.gameObject)) 
        {
            return; 
        }

        //Return if the object is not a player, small planet or big planet
        if (!(col.CompareTag("Player") || col.CompareTag("SmallPlanet") || col.CompareTag("BigPlanet")))
        {
            return; 
        }

        PlaySound(EnterSound);

        //Add to our current visitor list            
        Link.CurrentlyInZone.Add(col.gameObject);

        PlaySound(ExitSound);

        //move it
        Rigidbody TargetRigidbody = col.gameObject.GetComponent<Rigidbody>();
        Vector3 TargetVeloctiy = TargetRigidbody.velocity;
        //Keep the old Y position
        TargetRigidbody.transform.position = new Vector3(Link.transform.position.x, TargetRigidbody.position.y, Link.transform.position.z);
        //Set the old velocity
        TargetRigidbody.velocity = TargetVeloctiy;
    }

    //Check our current visitors to see if they leave,
    //not doing if on trigger exit as we are teleporting them which often doesnt call that function
    private void FixedUpdate()
    {
        float ColliderBuffer = 0.25f;
        int count = 0;
        while(count<CurrentlyInZone.Count)
        {
            if(Vector3.Distance(CurrentlyInZone[count].transform.position,transform.position)> (MyCollider.radius*MyCollider.transform.localScale.x)+ ColliderBuffer) 
            {
                CurrentlyInZone.Remove(CurrentlyInZone[count]); 
            }
            else
            {
                count++;
            }            
        }
    }

    void PlaySound(AudioClip Clip)
    {
        ThisSource.PlayOneShot(Clip);
    }
}
