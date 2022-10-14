using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using System.IO;

public class tutorialModPowerups : MonoBehaviour
{
    private static float PowerMultiplier = 1f;//used by shoot ball to indicate if power shot has been used or not
    private bool AlreadyScaledBlackHolesThisLevel = false;//used to prevent scaling em up multiple times

    public float PowerShotPower = 1.5f, BiggerBlackHoleScaleModifier = 2f, BlackHoleScaleSpeed = 3f;
    public GameObject MovePositionPreviewPrefab;
    public Text Quantity_PowerShot, Quantity_Rewind, Quantity_BiggerBlackHole, Quantity_SelectPosition;

    public static int Count_PowerShots = 10, Count_Rewinds = 10, Count_BlackHoles = 10, Count_Repositions = 10;

    public int mCount_PowerShots;
    public int mCount_Rewinds;
    public int mCount_BlackHoles;
    public int mCount_Repositions;

    //Identified bug:
    //Bug clicking UI buttons counts as a shot, 
    //fix: check if pointer over UI element before shooting

    private void Start()
    {
        PowerMultiplier = 1f;
        AlreadyScaledBlackHolesThisLevel = false;
        FilePath = Application.dataPath + "/Resources/" + PowerUpFolder + "/" + PowerFileName + ".txt";
        LoadPowers();
    }

    //*****Power Shot*****
    public void Use_PowerShot()
    {
        
        if (Count_PowerShots == 0) { return; }
        Count_PowerShots--;
       

        SavePowers();
        PowerMultiplier = PowerShotPower;

        //play sound here
    }

    public static float GetPowerMultiplier()
    {
        float temp = PowerMultiplier;
        PowerMultiplier = 1f;
        return temp;
    }

    //*****Rewind*****
    private Dictionary<GameObject, Vector3> ObjectLastPosition = new Dictionary<GameObject, Vector3>();

    public void Use_Rewind()
    {
        if (Count_Rewinds == 0) { return; }
        Count_Rewinds--;
        SavePowers();

        foreach (KeyValuePair<GameObject,Vector3> KVP in ObjectLastPosition)
        {
            if(KVP.Key == null) { continue; }
            KVP.Key.transform.position = KVP.Value;
            if(KVP.Key.TryGetComponent(out Rigidbody RB))
            {
                RB.velocity = Vector3.zero;
                RB.angularVelocity = Vector3.zero;
            }
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().ShotCount++;
    }

    public void SetLastPositions()
    {
        Debug.Log("Set last positions");
        GameObject Player = GameObject.FindGameObjectWithTag("Player");

        if (ObjectLastPosition.ContainsKey(Player))
        {
            ObjectLastPosition[Player] = Player.transform.position;
        }
        else
        {
            ObjectLastPosition.Add(Player, Player.transform.position);
        }

        foreach(GameObject g in GameObject.FindGameObjectsWithTag("SmallPlanet"))
        {
            if (ObjectLastPosition.ContainsKey(g))
            {
                ObjectLastPosition[g] = g.transform.position;
            }
            else
            {
                ObjectLastPosition.Add(g, g.transform.position);
            }
        }

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("BigPlanet"))
        {
            if (ObjectLastPosition.ContainsKey(g))
            {
                ObjectLastPosition[g] = g.transform.position;
            }
            else
            {
                ObjectLastPosition.Add(g, g.transform.position);
            }
        }
    }

    //*****Bigger Black Holes*****
    public void Use_BiggerBlackHoles()
    {
        if (Count_BlackHoles == 0) { return; }
        Count_BlackHoles--;
        SavePowers();

        if (AlreadyScaledBlackHolesThisLevel) { return; }
        AlreadyScaledBlackHolesThisLevel = true;
        
        //play sound here

        StartCoroutine(ScaleUp());
    }

    private IEnumerator ScaleUp()
    {
        GameObject[] holes = GameObject.FindGameObjectsWithTag("BlackHole");
        if (holes.Length == 0) { yield break; }
        float scale = holes[0].transform.localScale.magnitude/3;

        while(scale<BiggerBlackHoleScaleModifier)
        {
            scale += BlackHoleScaleSpeed * Time.deltaTime;
            foreach (GameObject g in holes)
            {
                g.transform.localScale = new Vector3(scale,scale,scale);
            }
            yield return 0;
        }
    }

    //*****Choose Position*****
    bool SelectingPosition = false;
    public void Use_ChoosePosition()
    {
        if (Count_Repositions == 0) { return; }
        Count_Repositions--;
        SavePowers();

        if (!SelectingPosition)
        {
            Debug.Log("Start move");
            StartCoroutine(SelectPosition());
        }
        else
        {
            Debug.Log("Cancel Move");
            SelectingPosition = false;
        }
    }

    private IEnumerator SelectPosition()
    {
        SelectingPosition = true;

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        GameObject Preview = Instantiate(MovePositionPreviewPrefab);
        Material InstanceMat = Preview.GetComponent<MeshRenderer>().material;
        EventSystem ES = EventSystem.current;

        if (!Player) { Debug.LogError("Player not found!"); SelectingPosition = false;yield break; }

        //Comet colliders are blocking everything lol, layermask everything other than comets and ignore rays
        LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Comet") | 1 << LayerMask.NameToLayer("Ignore Raycast"));
        int FloorHeight = 0;
        float PlayerHeight = Player.transform.position.y;

        while (SelectingPosition)
        {
            //Raycast users mouse position for selection
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.SphereCastAll(ray, 0.4f,100f, (LayerMask)layerMask);            

            if (hits.Length>0)
            {
                bool valid = true;
                foreach(RaycastHit rh in hits)
                {
                    if (rh.point.y < FloorHeight) { continue; }
                    if (rh.transform.gameObject.layer != LayerMask.NameToLayer("Floor"))
                    {
                        valid = false;
                        Debug.Log("Hit: " + rh.transform.name);
                        Debug.DrawLine(ray.origin, rh.point,Color.red);
                        break;
                    }
                }

                //Get the center of our hit
                Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask);

                //not valid (is blocked)
                if (!valid)
                {
                    InstanceMat.SetColor("_Color", new Color(1f,0,0,0.5f));
                }
                //valid
                else
                {
                    InstanceMat.SetColor("_Color", new Color(0, 1f, 0, 0.5f));

                    //Move the player if they click, and current event system says not over a UI object
                    if (Input.GetButtonDown("Fire1") && !ES.IsPointerOverGameObject())
                    {
                        Player.transform.position = new Vector3(hit.point.x,PlayerHeight,hit.point.z);
                        Destroy(Preview);
                        SelectingPosition = false;
                        yield break;
                    }
                }

                //Move preview to location
                Preview.transform.position  = new Vector3(hit.point.x, FloorHeight, hit.point.z);

            }
            //not valid didn't hit anything
            else
            {
                InstanceMat.SetColor("_Color", new Color(0, 0, 0, 0.0f));
            }

            //Wait for next frame
            yield return 0;
        }

        //User has clicked the button again canceling it
        Destroy(Preview);
    }

    private string PowerUpFolder = "Powers", FilePath = "",PowerFileName = "PowerInfo";
    private void LoadPowers()
    {
        CheckDirectory();

        if (!File.Exists(FilePath)) { return; }

        StreamReader sr = File.OpenText(FilePath);

        Count_PowerShots = mCount_PowerShots;
        Count_Rewinds = mCount_Rewinds;
        Count_BlackHoles = mCount_BlackHoles;
        Count_Repositions = mCount_Repositions;

        sr.Close();

        UpdateTextCounts();
    }

    private void SavePowers()
    {
        //CheckDirectory();

        //if (File.Exists(FilePath))
        //{
        //    File.Delete(FilePath);
        //}

        //StreamWriter SW = File.CreateText(FilePath);

        //SW.WriteLine(Count_PowerShots);
        //SW.WriteLine(Count_Rewinds);
        //SW.WriteLine(Count_BlackHoles);
        //SW.WriteLine(Count_Repositions);

        //SW.Close();


       

        UpdateTextCounts();
  

    }

    private void UpdateTextCounts()
    {
      
        Quantity_PowerShot.text = Count_PowerShots.ToString();
        Quantity_Rewind.text = Count_Rewinds.ToString();
        Quantity_BiggerBlackHole.text = Count_BlackHoles.ToString();
        Quantity_SelectPosition.text = Count_Repositions.ToString();
}

    private void CheckDirectory()
    {
        if (!Directory.Exists(Application.dataPath + "/Resources/" + PowerUpFolder))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/" + PowerUpFolder);
            Debug.Log("Created power up data directory.");
        }
    }

    // Functions for the shop for purchasing powerups
    public void PurchasePowershot() 
    {
        if (Data.instance.Gold >= 5)
        {
            Data.instance.Gold -= 5;
            Count_PowerShots++;
            SavePowers();
        }
    }

    public void PurchaseRewind() 
    {
        if (Data.instance.Gold >= 7)
        {
            Data.instance.Gold -= 7;
            Count_Rewinds++;
            SavePowers();
        }
    }

    public void PurchaseBlackhole() 
    {
        if (Data.instance.Gems >= 1)
        {
            Data.instance.Gems--;
            Count_BlackHoles++;
            SavePowers();
        }
    }

    public void PurchaseSelectPosition() 
    {
        if (Data.instance.Gems >= 3)
        {
            Data.instance.Gems -= 3;
            Count_Repositions++;
            SavePowers();
        }
    }
}
