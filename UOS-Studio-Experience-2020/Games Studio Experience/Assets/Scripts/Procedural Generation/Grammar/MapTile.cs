using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MapTile : MonoBehaviour
{
    [SerializeField] private Transform Up;
    [SerializeField] private Transform Down;
    [SerializeField] private Transform Left;
    [SerializeField] private Transform Right;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform getRandomSide()
    {
        // gross approach, to be cleaned
        
        int randInt = (int)(Random.value * 4.0f - 0.01f); // -0.01f to avoid the slim possibility of getting a 4, as int conversion will round down
        Debug.Log(randInt);
        
        if (randInt == 0)
            return Up;
        if (randInt == 1)
            return Down;
        if (randInt == 2)
            return Left;
        
        return Right;
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Up.position, 1);
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Down.position, 1);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Left.position, 1);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Right.position, 1);
    }
}
