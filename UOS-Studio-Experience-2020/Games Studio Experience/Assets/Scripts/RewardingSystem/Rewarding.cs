using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewarding : MonoBehaviour
{
    public  int SmallPlanets;
    public  int BigPlanets;
    public  int Planets;
    public bool Stop;//just a simple flag
    public int GoldEarnedThisScene;
   [SerializeField]private GameObject VictoryPanel;


    public InGameUI UI;
    void Start()
    {
        //these need to be overwritten by the save system when loading the game
        //Data.instance.Gold = 0;
        //Data.instance.Gems = 0;
       
        
        SmallPlanets = GameObject.FindGameObjectsWithTag("SmallPlanet").Length;
        BigPlanets= GameObject.FindGameObjectsWithTag("BigPlanet").Length;

        Planets = SmallPlanets + BigPlanets;
        UI=GameObject.Find("InGameUI").GetComponent<InGameUI>();

        Stop = false;

    }

    void Update()
    {
        //for dubugging 
        //if (Input.GetKeyDown("space"))
        //{
        //    StartCoroutine(AddGoldBigPlanet());
        //}
        if (Planets == 0 && Stop == false) 
        {
            StartCoroutine(RewardGems());
            VictoryPanel.SetActive(true);
        }
     
    }

    //each planet will need to use one of this functions in their "collision" script 
    public  IEnumerator AddGoldSmallPlanet()
    {
        int ammount = Random.Range(5,10);
        Data.instance.Gold  += ammount;
        GoldEarnedThisScene += ammount;
        Planets--;
        UI.UpdateCoinsUI();

        yield return new WaitForSeconds(0.5f);
    }

    public   IEnumerator AddGoldBigPlanet()
    {
        int ammount = Random.Range(15, 25);
        Data.instance.Gold  += ammount;
        GoldEarnedThisScene += ammount;
        Planets--;
        UI.UpdateCoinsUI();

        yield return new WaitForSeconds(0.5f);
    }

    public   IEnumerator RewardGems( ) //the coins ui will update only when the level is finished
    {
        int gemsreward = 0;
        gemsreward += SmallPlanets+BigPlanets*3;
        Data.instance.Gems += gemsreward;
        
        UI.UpdateGemsUI();
        Stop = true;
        Data.instance.Score += GoldEarnedThisScene + gemsreward * 20;
        yield return new WaitForSeconds(0.5f);
    }
  
}
