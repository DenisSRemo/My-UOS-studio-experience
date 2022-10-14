using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BlackHolePlanetCollision : MonoBehaviour
{
    //Rhys' stuff for giving an extra shot to the player once they pot a planet
    PlayerMovement player;
    private Rewarding rewarding;

    public AudioClip PlanetToPlanetClip;
    public AudioClip PlanetPottedClip;
    public float WallRepellent = 0.6f;
    private AudioSource ThisSource;

    private void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        rewarding = GameObject.Find("GameManager").GetComponent<Rewarding>();

        ThisSource = this.GetComponent<AudioSource>();
    }


    //all planets will need to have this script attached to them
    void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.tag=="BlackHole")
        {

            if (gameObject.tag == "SmallPlanet")
                StartCoroutine(rewarding.AddGoldSmallPlanet());
            if (gameObject.tag == "BigPlanet")
                StartCoroutine(rewarding.AddGoldBigPlanet());

            ThisSource.PlayOneShot(PlanetPottedClip);
            Destroy(gameObject);

            //player.ShotCount++;
            player.AmountOfPlanetsPotted++;
        }

        else if(collision.gameObject.CompareTag("SmallPlanet") || collision.gameObject.CompareTag("BigPlanet") ||
            collision.gameObject.CompareTag("Player"))
        {
            if(ThisSource)
            {
                ThisSource.PlayOneShot(PlanetToPlanetClip);                
            }
        }

        if(collision.gameObject.CompareTag("Wall"))
        {
            if (TryGetComponent(out Rigidbody RB))
            {
                Debug.Log("Object: " + gameObject.name + " has bounced with: " + collision.gameObject.name);
                Vector3 ForceDirection = collision.contacts[0].normal;
                RB.AddForce(ForceDirection * WallRepellent, ForceMode.Impulse);
            }
        }
    }
}
