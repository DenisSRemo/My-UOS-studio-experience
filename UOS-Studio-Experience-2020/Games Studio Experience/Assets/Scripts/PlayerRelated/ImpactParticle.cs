using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactParticle : MonoBehaviour
{
    public GameObject ParticlePrefab;
    public string[] IgnoreGameObjectNames;
    public void OnCollisionEnter(Collision collision)
    {
        foreach(string s in IgnoreGameObjectNames)
        {
            if(collision.gameObject.name.Contains(s)) { return; }
        }

        GameObject g = Instantiate(ParticlePrefab, collision.contacts[0].point, Quaternion.identity);
    }
}
