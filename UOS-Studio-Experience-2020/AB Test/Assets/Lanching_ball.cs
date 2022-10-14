using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lanching_ball : MonoBehaviour
{

    public GameObject ball;
    public Button Lanch_button;
    public Slider Power;
    public Slider Angle;
    public float str;
    public float pr;
    public Rigidbody rb;
   private float xcomponent;
    private float ycomponent;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        str = Angle.value;
        pr = Power.value;
    }

    // Update is called once per frame
    void Update()
    {
        str = Angle.value*360;
        pr = Power.value*200;
        xcomponent = Mathf.Cos(str * Mathf.PI / 180) * pr;
         ycomponent = Mathf.Sin(str * Mathf.PI / 180) * pr;
    }

    public void Launch()
    {
        rb.AddForce(ycomponent, 0, xcomponent);
    }
}
