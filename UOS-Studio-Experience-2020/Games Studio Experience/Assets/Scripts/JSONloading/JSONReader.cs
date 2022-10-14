using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    

    void Start()
    {
       // ReadFile("D1");
        
    }

    static public List<JSONObstacle> GetJsonObstacles(string name)
    {
        List<JSONObstacle> obstacles= new List<JSONObstacle>();
        var jsonTextFile = Resources.Load<TextAsset>("Difficulties/"+name);
        JSONObstacles obstaclesInJSON = JsonUtility.FromJson<JSONObstacles>(jsonTextFile.text);
        foreach (JSONObstacle obstacle in obstaclesInJSON.Obstacles)
        {
            obstacles.Add(obstacle);
        }
        return obstacles;
    }
    static public List<JSONPlanet> GetJsonPlanets(string name)
    {
        List<JSONPlanet> planets = new List<JSONPlanet>();
        var jsonTextFile = Resources.Load<TextAsset>("Difficulties/" + name);
        JSONPlanets planetsinJSON = JsonUtility.FromJson<JSONPlanets>(jsonTextFile.text);

      
        foreach (JSONPlanet planet in planetsinJSON.Planets)
        {
            planets.Add(planet);

        }

        return planets;
    }



}