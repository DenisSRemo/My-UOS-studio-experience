using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawCircle 
{
    // public float theta_scale = 0.01f;        
    //public  int size; 
    // public float radius = 3f;
    //public  LineRenderer lineRenderer;

    //  void Awake()
    //  {
    //      float sizeValue = (2.0f * Mathf.PI) / theta_scale;
    //      size = (int)sizeValue;
    //      size++;
    //      //lineRenderer = gameObject.AddComponent<LineRenderer>();
    //     // lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
    //      lineRenderer.startWidth = 0.1f;
    //      lineRenderer.endWidth = 0.1f;
    //      lineRenderer.positionCount = size;
    //  }

    //  void Update()
    //  {
    //      Vector3 pos;
    //      float theta = 0f;
    //      for (int i = 0; i < size; i++)
    //      {
    //          theta += (2.0f * Mathf.PI * theta_scale);
    //          float x = radius * Mathf.Cos(theta);
    //          float y = radius * Mathf.Sin(theta);
    //          x += gameObject.transform.position.x;
    //          y += gameObject.transform.position.y;
    //          pos = new Vector3(x, y, 0);
    //          lineRenderer.SetPosition(i, pos);
    //      }
    //  }


    public static void drawCircle(this GameObject container, float radius, float lineWidth)
    {
        var segments = 360;
        var line = container.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; 
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }
       
        line.SetPositions(points);
    }
}
