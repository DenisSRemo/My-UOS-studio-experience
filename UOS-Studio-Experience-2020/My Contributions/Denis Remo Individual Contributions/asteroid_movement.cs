using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asteroid_movement : MonoBehaviour
{
    // for easy setting of the asteroid belt, the speed, delay time and life time can be edited in the instructor
    [SerializeField]private float Speed;
    [SerializeField] private float startTime;
    [SerializeField] private float delayTime;
    [SerializeField] private float lifeTime;
    [SerializeField] private Vector3 startingPosition;
    
    void Start()
    {
        startingPosition = gameObject.transform.position;
        startTime = Time.time + delayTime;// it sets the delay of the asteroid for the asteroid belt look more random
        
    }

    //movement and cycle of the asteroid
    void Update()
    {
        if (Time.time >= startTime)
        {
            transform.position += -transform.right * Speed;

            if (Time.time >= startTime + lifeTime)//when the asteroid's cycle (life time) ends, it returns to the original position
            {
                
                gameObject.SetActive(false);
                gameObject.transform.position = startingPosition;
                gameObject.SetActive(true);
                startTime = Time.time;
                
            }
        }

    }
}
