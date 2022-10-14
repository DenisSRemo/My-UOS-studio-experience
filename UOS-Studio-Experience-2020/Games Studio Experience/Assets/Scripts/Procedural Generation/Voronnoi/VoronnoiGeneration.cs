using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoronnoiGeneration : MonoBehaviour
{
    
    // bounds for generation
    public Vector2 Corner_TL;
    public Vector2 Corner_BR;

    public int NumberPoints;

    [SerializeField] private GameObject voronoiPoint;
    
    // VORONOI TESSELATION
    // 1) Create random set of points
    // 2) Create areas of equidistance from all points
    // 3) Generate meshes of these areas
    //    a) Calculate vertices
    //    b) Calculate edges
    //    c) Construct mesh
    // 4) Pick x meshes in a row to form a map 

    // Start is called before the first frame update
    void Start()
    {
       GeneratePoints(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GeneratePoints()
    {
        for (int i = 0; i < NumberPoints; i++)
        {
            Vector3 position = new Vector3(Random.Range(Corner_TL.x, Corner_BR.x), 0.0f, Random.Range(Corner_TL.y, Corner_BR.y));
            
            Instantiate(voronoiPoint, position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Corner_TL + (Corner_BR - Corner_TL) / 2.0f;
        center.z = center.y;
        center.y = 0.0f;
        
        Vector3 size = new Vector3(Corner_BR.x - Corner_TL.x, 0, Corner_BR.y - Corner_TL.y);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
}
