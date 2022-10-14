using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public MapTile firstTile;

    private Random.State randState;

    public GameObject[] Tiles;

    public int MaxMapLength;
    // Start is called before the first frame update
    void Start()
    {
        // get random seed for analytics
        randState = Random.state;

        int success = GenerateNewTile(firstTile, 0);

        Debug.Log(success);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // WARNING: Recursive
    private int GenerateNewTile(MapTile lastTile, int level, int attemptNo = 1)
    {
        if (!(level >= MaxMapLength))
        {
            Transform newPos = lastTile.getRandomSide();

            // collision check with new tile
            RaycastHit uselessHitInfo;
            Collider[] hits = Physics.OverlapSphere(newPos.position, 1.0f);

            if (!(hits.Length > 0))
            {
                GameObject newTile = Instantiate(Tiles[Random.Range(0, Tiles.Length)], newPos.position,
                    Quaternion.identity);
                MapTile newMapTile = newTile.GetComponent<MapTile>();
                GenerateNewTile(newMapTile, level + 1);
            }
            else if(attemptNo < 4)
            {
                Debug.Log("Collision detected. Trying again");
                GenerateNewTile(lastTile, level, attemptNo + 1);
            }
            else
            {
                Debug.Log("Too many collisions occured. Backing out");
            }
        }

        return 0;
    }
}