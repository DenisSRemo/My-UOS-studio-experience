using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackholepot : MonoBehaviour
{
    private AudioSource pottedsound;
    // Start is called before the first frame update
    void Start()
    {
        pottedsound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("SmallPlanet") || collision.transform.CompareTag("BigPlanet"))
        {
            pottedsound.PlayOneShot(pottedsound.clip);
            transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }
    }
}
