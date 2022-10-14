using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

public class RandomPlacement : MonoBehaviour
{
    [Header("Generation")]
    public Transform start;
    public Transform end;
    [SerializeReference]private bool showGizmo;
    
    private int width;
    private int height;

    // 0 = out of spawnable area
    // 1 = spawnable area
    // 2 = edge of spawnable area // this currently doesn't exist. Just marked as out of spawnable area
    // 3 = non spawnable area (object spawned or level geometry in the way)
    
    [SerializeField] private int ObstaclesToSpawn;
    [SerializeField] private int obstacleSpawnY;
    
    private int[,] obstaclePlacable;
    [SerializeField] private int timeToCheck = 10;

    [Space]
    [Header("Obstacles & Planets")]
    
    [SerializeField] private List<GameObject> obstacles;

    private int NumberOfWormholes = 1;
    private int NumberOfComets = 1;
    
    public List<GameObject> LargePlanets;
    public List<GameObject> SmallPlanets;
    public int NumberOfSmallPlanets;
    public int NumberOfLargePlanets;
    public List<GameObject> PlacedPlanets;

    public float NumberOfAstronauts;
    public GameObject AstronautPrefab;

    [SerializeField] private GameObject wormholePrefab;

    [SerializeField] private LayerMask spawnLayer;
    [SerializeField] private LayerMask noSpawnLayer;
    
    [Space]
    [Header("JSON Lookup")]

    [SerializeField] private GameObject JSONWormholePrefab;
    [SerializeField] private GameObject JSONBigPlanetPrefab;
    [SerializeField] private GameObject JSONSmallPlanetPrefab;
    [SerializeField] private GameObject JSONCometPrefab;
    [SerializeField] private GameObject JSONMoonPrefab;
    [SerializeField] private GameObject JSONNeutStarPrefab;
    [SerializeField] private GameObject JSONRedGiantPrefab;
    [SerializeField] private GameObject JSONBrokenSatPrefab;

    public string JSONToLoad;

    // moved to higher scope for mutation
    private List<GameObject> jsonObstacles=new List<GameObject>();

    private AnalyticsManager Analytics;

    // Start is called before the first frame update
    void Start()
    {
        width = (int) (end.position.x - start.position.x);
        height = (int) (end.position.z - start.position.z);

        
        // Get Obstacles from JSON
        List<JSONObstacle> obstaclesToSpawn = JSONReader.GetJsonObstacles(JSONToLoad);
        List<JSONPlanet> planetsToSpawn = JSONReader.GetJsonPlanets(JSONToLoad);
        Analytics = GameObject.FindGameObjectWithTag("Analytics").GetComponent<AnalyticsManager>();
        
        List<GameObject> jsonPlanets=new List<GameObject>();

        for (int i = 0; i < obstaclesToSpawn.Count;i++)
        {
            switch (obstaclesToSpawn[i].name)
            {
                case "Wormhole":
                    jsonObstacles.Add(JSONWormholePrefab);
                    break;
                case "Comet":
                    jsonObstacles.Add(JSONCometPrefab);
                    break;
                case "Moon":
                    jsonObstacles.Add(JSONMoonPrefab);
                    break;
                case "NeutStar":
                    jsonObstacles.Add(JSONNeutStarPrefab);
                    break;
                case "RedGiant":
                    jsonObstacles.Add(JSONRedGiantPrefab);
                    break;
                case "BrokenSat":
                    jsonObstacles.Add(JSONBrokenSatPrefab);
                    break;
                default:
                    Debug.LogError("ERROR reading from JSON file, have a look at the JSON file");
                    break;
            }

        }
        for (int j = 0; j < planetsToSpawn.Count; j++)
        {
            switch (planetsToSpawn[j].name)
            {
                case "BigPlanet":
                    for (int k = 0; k < planetsToSpawn[j].numberof; k++)
                    {
                        jsonPlanets.Add(JSONBigPlanetPrefab);
                    }
                    break;
                case "SmallPlanet":
                    for (int l = 0; l < planetsToSpawn[j].numberof; l++)
                    {
                        jsonPlanets.Add(JSONSmallPlanetPrefab);
                    }
                    break;
                default:
                    Debug.LogError("ERROR reading from JSON file, have a look at the JSON file");
                    break;
            }

        }



        // Init spawn grid
        Debug.Log("Width: " + width.ToString() + " Height: " + height.ToString());
        
        obstaclePlacable = new int[width,height];
        
        for (int i = 0; i < width; i++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 center;
                center.x = start.position.x + (i * 1) + 0.5f;
                center.y = start.position.y;
                center.z = start.position.z + (z * 1) + 0.5f;

                //default value not spawnable
                obstaclePlacable[i, z] = 0;
                
                // cachable
                if (Physics.OverlapSphere(center, 0.5f, spawnLayer).Length > 0)
                {
                    obstaclePlacable[i, z] = 1;
                }
                if (Physics.OverlapSphere(center, 0.5f, noSpawnLayer).Length > 0)
                {
                    obstaclePlacable[i, z] = 3;
                }
            }
        }

        // Trim edges of space
        // for, for: if they are next to a 0 then trim it to a 3
        // 3 because if it was made 0 then they would potentially just all get trimmed to a 0
        int[,] obstaclePlacableCopy = (int[,])obstaclePlacable.Clone();
        for(int w = 0; w < width; w++)
        {
            for(int h = 0; h < height; h++)
            {
                if(w + 1 > 0 && w + 1 < width)
                {
                    if (obstaclePlacableCopy[w + 1, h] == 0 || obstaclePlacableCopy[w + 1, h] == 3)
                    {
                        obstaclePlacable[w, h] = 0;
                    }
                }

                if (w - 1 > 0 && w -1 < width)
                {
                    if (obstaclePlacableCopy[w - 1, h] == 0 || obstaclePlacableCopy[w - 1, h] == 3)
                    {
                        obstaclePlacable[w, h] = 0;
                    }
                }

                if (h + 1 > 0 && h + 1 < height)
                {
                    if (obstaclePlacableCopy[w, h + 1] == 0 || obstaclePlacableCopy[w, h + 1] == 3)
                    {
                        obstaclePlacable[w, h] = 0;
                    }
                }

                if (h - 1 > 0 && h - 1 < height)
                {
                    if (obstaclePlacableCopy[w, h - 1] == 0 || obstaclePlacableCopy[w, h - 1] == 3)
                    {
                        obstaclePlacable[w, h] = 0;
                    }
                }
            }
        }

        //Spawn Large Planets
        for (int i = 0; i < NumberOfLargePlanets; i++)
        {
            SpawnPlanet(LargePlanets[Random.Range(0, LargePlanets.Count - 1)]);
        }

        //Spawn Small Planets
        for (int i = 0; i < NumberOfSmallPlanets; i++)
        {
            SpawnPlanet(SmallPlanets[Random.Range(0, SmallPlanets.Count - 1)]);
        }

        // for i < Number of Obstacle
        for (int i = 0; i < ObstaclesToSpawn; i++)
        {
            SpawnObstacle(jsonObstacles[Random.Range(0, jsonObstacles.Count)]);
        }

        for(int i = 0; i < NumberOfAstronauts; i++)
        {
            FriendsList.UpdateFriends();
            // check if we have friends. error caused otherwise with count 0
            if (FriendsList.TrackedFriends.Count > 0)
            {
                string Name = FriendsList.TrackedFriends[Random.Range(0, FriendsList.TrackedFriends.Count - 1)];
                SpawnAstronaut(Name);
            }
        }

    }

    private void SpawnAstronaut(string AstronautName)
    {
        //Find Suitable Location
        int timesCheckedForPlacement = 0;
        int spawnFound = 0;
        Vector2 spawnLocation = new Vector2();
        int AstroWidth = (int)AstronautPrefab.GetComponentInChildren<MeshRenderer>().bounds.size.x;
        int AstroHeight = (int)AstronautPrefab.GetComponentInChildren<MeshRenderer>().bounds.size.y;

        //Check 10 times for placement
        while (timesCheckedForPlacement < timeToCheck && spawnFound != 1)
        {
            spawnLocation = FindSuitableLocation(AstroWidth, AstroHeight, out spawnFound);

            timesCheckedForPlacement++;
        }

        if(spawnFound == 1)
        {
            Vector3 spawnPosition = new Vector3();
            spawnPosition.x = start.position.x + spawnLocation.x;
            spawnPosition.y = 0.75f;
            spawnPosition.z = start.position.z + spawnLocation.y;

            //Spawn Astronaut
            GameObject SpawnedAstro = Instantiate(AstronautPrefab, spawnPosition, Quaternion.Euler(90, -90, 0));
            //Set Name Text
            SpawnedAstro.transform.GetChild(0).GetComponent<TextMesh>().text = AstronautName;

        }
    }

    private void SpawnPlanet(GameObject SelectedPlanet)
    {
        int timesCheckedForPlacement = 0;
        int spawnFound = 0;
        Vector2 spawnLocation = new Vector2();
        int PlanetWidth = (int)SelectedPlanet.GetComponentInChildren<MeshRenderer>().bounds.size.x;
        int PlanetHeight = (int)SelectedPlanet.GetComponentInChildren<MeshRenderer>().bounds.size.y;

        //Check 10 times for placement
        while (timesCheckedForPlacement < timeToCheck && spawnFound != 1)
        {
            spawnLocation = FindSuitableLocation(PlanetWidth, PlanetHeight, out spawnFound);

            timesCheckedForPlacement++;
        }

        if (spawnFound == 1)
        {
            Vector3 spawnPosition = new Vector3();
            spawnPosition.x = start.position.x + spawnLocation.x;
            spawnPosition.y = 0.75f;
            spawnPosition.z = start.position.z + spawnLocation.y;

            GameObject spawnedObject = Instantiate(SelectedPlanet, spawnPosition, Quaternion.identity);
            PlacedPlanets.Add(spawnedObject);
        }

        for (int i = 0; i < PlanetWidth; i++)
        {
            for (int j = 0; j < PlanetHeight; j++)
            {
                Debug.Log("Marking position: " + (spawnLocation.x - i).ToString() + "," + (spawnLocation.y - j).ToString() + " as taken.");
                obstaclePlacable[(int)spawnLocation.x - i, (int)spawnLocation.y - j] = 3;
            }
        }
    }

    // needs refactoring into a different function for DRY principles
    // also needs variable name change to remove the _obstacleToPlace malarkey
    private bool PlacingWormhole = false;
    private wormhole FirstHole;
    private void SpawnObstacle(GameObject _obstacleToPlace)
    {
        // pick an obstacle from list
        Debug.Log(_obstacleToPlace.name);
        Analytics.ObstaclesSpawned += _obstacleToPlace.name+"/";
        
        // get data component from obstacle
        Obstacle obstacleData = _obstacleToPlace.GetComponentInChildren<Obstacle>();
        
        // try 10 times to find a suitable location for an obstacle
        int timesCheckedForPlacement = 0;
        int spawnFound = 0;
        Vector2 spawnLocation = new Vector2();
        
        while (timesCheckedForPlacement < timeToCheck && spawnFound != 1)
        {
            try
            {
                spawnLocation = FindSuitableLocation(obstacleData.obstacleWidth, obstacleData.obstacleHeight, out spawnFound);

                timesCheckedForPlacement++;
            }
            catch (Exception e)
            {
                Debug.LogError("Error getting obstacle data on" + _obstacleToPlace.name);
                throw;
            }
            
        }

        if (spawnFound == 1)
        {
            Vector3 spawnPosition = new Vector3();
            spawnPosition.x = start.position.x + spawnLocation.x;
            spawnPosition.y = obstacleSpawnY;
            spawnPosition.z = start.position.z + spawnLocation.y;
            
            if(_obstacleToPlace.name != "Moon")
            {
                GameObject spawnedObject = Instantiate(_obstacleToPlace, spawnPosition, Quaternion.identity);
                
                for (int i = 0; i < obstacleData.obstacleWidth; i++)
                {
                    for (int j = 0; j < obstacleData.obstacleHeight; j++)
                    {
                        Debug.Log("Marking position: " + (spawnLocation.x - i).ToString() + "," + (spawnLocation.y - j).ToString() + " as taken.");
                        obstaclePlacable[(int)spawnLocation.x - i, (int)spawnLocation.y - j] = 3;
                    }
                }

                wormhole newWorm = spawnedObject.GetComponentInChildren<wormhole>();
                if (newWorm)
                {
                    //if we are already placing a wormhole then this must be the second one
                    if (PlacingWormhole)
                    {
                        Debug.Log("Placed second wormhole");
                        //Link em
                        newWorm.Link = FirstHole;
                        FirstHole.Link = newWorm;

                        PlacingWormhole = false;

                        // remove wormhole from spawn rotation once the 2nd one is spawned & they're linked
                        jsonObstacles.Remove(_obstacleToPlace);
                    }
                    //if its a wormhole and not placing one it must be the first   
                    else
                    {
                        Debug.Log("Placed first wormhole");
                        //Set the first hole so they can be linked
                        FirstHole = newWorm;

                        PlacingWormhole = true;

                        //we just placed a wormhole so this must be the wormhole prefab, spawn it again for link
                        SpawnObstacle(_obstacleToPlace);
                    }
                }
            }

            else
            {
                Debug.Log("Moon");
                //Select Planet without Moon
                foreach(GameObject Planet in PlacedPlanets)
                {
                    if(Planet.transform.childCount == 0)
                    {
                        //Create Offset Vector
                        Vector3 Offset = new Vector3(Planet.transform.position.x + 1.5f, Planet.transform.position.y, Planet.transform.position.z);

                        //Spawn Moon and set Parent
                        GameObject spawnedObject = Instantiate(_obstacleToPlace, Offset, Quaternion.identity);
                        spawnedObject.transform.SetParent(Planet.transform);

                        break;
                    }
                }
            }
            
            // Only spawn 1 comet
            if(_obstacleToPlace.name == "CometRoot")
                jsonObstacles.Remove(_obstacleToPlace);
        }
        //oh no we can't spawn the second wormhole, better delete the first one then :(
        else if (PlacingWormhole)
        {
            Debug.Log("Failed to place second wormhole, deleting the first");
            //todo mark the first hole position as no longer taken

            Destroy(FirstHole.gameObject);
            PlacingWormhole = false;
        }
    }   

    private Vector2 FindSuitableLocation(int obstacleWidth, int obstacleHeight, out int spawnFound)
    {
        Debug.Log("**** Finding a location to spawn ****");
        
        int x = Random.Range(0, width);
        int z = Random.Range(0, height);
        
        Debug.Log("X: " + x.ToString() + " Z: " + z.ToString() + " " + obstaclePlacable[x, z].ToString());

        // check that there is enough space for object to spawn
        for (int i = 0; i < obstacleWidth; i++)
        {
            for (int j = 0; j < obstacleHeight; j++)
            {
                if (obstaclePlacable[x - i, z - j] == 0)
                {
                    Debug.Log(obstaclePlacable[x - i, z - j].ToString());
                    spawnFound = 0;
                    return Vector2.zero;
                }
            }
        }

        spawnFound = 1;
        return new Vector2(x, z);
    }

    // Draw the tool grid
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            
            // can probably be sped up by caching some values

            width = (int) (end.position.x - start.position.x);
            height = (int) (end.position.z - start.position.z);

            for (int i = 0; i < width; i++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 center;
                    center.x = start.position.x + (i * 1) + 0.5f;
                    center.y = start.position.y;
                    center.z = start.position.z + (z * 1) + 0.5f;

                    Gizmos.color = Color.yellow;

                    if (!Application.isPlaying)
                    {
                        // cachable
                        if (Physics.OverlapSphere(center, 0.5f, spawnLayer).Length > 0)
                        {
                            Gizmos.color = Color.green;
                        }

                        if (Physics.OverlapSphere(center, 0.5f, noSpawnLayer).Length > 0)
                        {
                            Gizmos.color = Color.red;
                        }
                    }

                    else
                    {
                        if (obstaclePlacable[i, z] == 1)
                        {
                            Gizmos.color = Color.green;
                        }
                        else if (obstaclePlacable[i, z] == 2)
                        {
                            Gizmos.color = Color.cyan;
                        }
                        if (obstaclePlacable[i, z] == 3)
                        {
                            Gizmos.color = Color.red;
                        }
                    }
                    
                    // could cache all the centers in a 2D list? but may incur more overhead when using the tool
                    Gizmos.DrawWireCube(center, Vector3.one);
                }
            }
        }
    }
}
