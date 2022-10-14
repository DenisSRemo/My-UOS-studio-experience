using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    LineRenderer line;

    public float BasePower;
    public Slider powerSlider;
    public int ShotCount, ShotsTaken;
    public GameObject PowerUpUI,PauseButton;

    float minimumPower = 0;
    float maximumPower = 10f;

    float TimeMoving = 0f, MaxTimeMoving = 5f, MaxTimeSmallMovement =3f, MaximumVelocityForMove = 1.25f;

    //Here for testing for now
    public int AmountOfPlanetsPotted;
    public int AmountOfPlanets;
    bool Recorded = false;

    public bool CanShoot = true, Moving = false, isPaused = false;
    private bool Shooting = false;

    private AudioSource ThisSource;
    private AnalyticsManager dataRecorder;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
        ThisSource = GetComponent<AudioSource>();
        AmountOfPlanets += GameObject.FindGameObjectsWithTag("SmallPlanet").Length;
        AmountOfPlanets += GameObject.FindGameObjectsWithTag("BigPlanet").Length;
        PauseButton = GameObject.FindGameObjectWithTag("Pause");
        dataRecorder = GameObject.FindGameObjectWithTag("Analytics").GetComponent<AnalyticsManager>();
    }
    
    void Update()
    {
		//Section for analytics recording how many shots taken.
        if (AmountOfPlanetsPotted >= AmountOfPlanets && !Recorded)
        {
            Debug.Log("Recorded");
            int AmountOfShotsNeeded = ShotsTaken;
            int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            dataRecorder.RecordData(CurrentSceneIndex, AmountOfShotsNeeded, AmountOfPlanets);

            /*
             * All of this was unity analytics however since it wasn't giving us the information we needed its been made redudant.
              Analytics.CustomEvent("Amount_Shots", new Dictionary<string, object>
            {
                {"Amount of shots needed", AmountOfShotsNeeded},
                {"Level :",CurrentScene.buildIndex }
            });
            AnalyticsResult ar = Analytics.CustomEvent("Amount_Shots");
            Debug.Log("Result is " + ar.ToString());
             */

            Recorded = true;
        }

        if (isPaused)
            return;

        if (ShotCount == 0)
        {
            line.enabled = false;
            return;
        }
        if (rb.velocity.magnitude > MaximumVelocityForMove)
        {
            TimeMoving += Time.deltaTime;
            if (TimeMoving >= MaxTimeMoving)
            {
                ResetingBall();
            }
            return;
        }
        else if(Moving)
        {
            TimeMoving += Time.deltaTime;
            if (TimeMoving >= MaxTimeSmallMovement) 
            {
                ResetingBall();
            }
            return;
        }
        TimeMoving = 0f;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Set up mouse position
        Vector3 mousePosition = ray.GetPoint(Camera.main.transform.position.y);
        mousePosition = new Vector3(mousePosition.x, 0.1f, mousePosition.z);

        //Set up ball position
        Vector3 ballPosition = new Vector3(transform.position.x, 0.1f,transform.position.z);
        line.SetPosition(0, transform.position);

        //Set up line directions
        Vector3 BallToMouseDirection = (mousePosition - ballPosition).normalized * Options.OptionsValue_DragSensitivity;
        //distance from mouse to ball clamped by power
        float Dist = Mathf.Clamp(Vector3.Distance(ballPosition, mousePosition), minimumPower, maximumPower);
        //Invert the direction
        if (Options.OptionsValue_PushControls)
        {
            BallToMouseDirection = new Vector3(BallToMouseDirection.x, 0.0f, BallToMouseDirection.z) * Dist;
        }
        else
        {
            BallToMouseDirection = new Vector3(-BallToMouseDirection.x, 0.0f, -BallToMouseDirection.z) * Dist;
        }
        
        line.SetPosition(1, ballPosition+(new Vector3(BallToMouseDirection.x,0.1f,BallToMouseDirection.z)));

        Debug.DrawLine(transform.position, mousePosition,Color.red);

        if (CanShoot) 
        {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                line.enabled = true;
                Shooting = true;
                return;
            }

            else if (Input.GetMouseButtonUp(0) && Shooting)
            {
                Shooting = false;
                PowerUpUI.SetActive(false);
                PauseButton.SetActive(false);
                line.enabled = false;
                rb.isKinematic = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                Shoot(BallToMouseDirection * (1 + rb.drag));
                CanShoot = false;
                Moving = true;

                ThisSource.PlayOneShot(ThisSource.clip);
                ShotCount--;
                ShotsTaken++;
                Debug.Log("released");
                return;
            }

            //line.enabled = false;
        }
    }

    void Shoot(Vector3 Force) 
    {
        if (PowerUps.PUS)
        {
            PowerUps.PUS.SetLastPositions();
        }
        
        
        rb.AddForce(Force * PowerUps.GetPowerMultiplier(),ForceMode.Impulse);
    }

    void ResetingBall() 
    {
        rb.velocity = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;
        Moving = false;
        CanShoot = true;
        PowerUpUI.SetActive(true);
        PauseButton.SetActive(true);
    }


}
