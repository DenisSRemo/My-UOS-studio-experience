using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotCounterUI : MonoBehaviour
{
  [HideInInspector] public Text ShotCounterText;
 
   [SerializeField] public GameObject GameoverPanel;
    GameObject playerObject;
  PlayerMovement player;

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<PlayerMovement>();
        ShotCounterText = GameObject.Find("ShotCounter").GetComponent<Text>();
       // GameoverPanel = GameObject.FindGameObjectWithTag("GameOverPanel");
    }
    void Update()
    {
        ShotCounterText.text = player.ShotCount + "";

        if(player.ShotCount <= 0)
        {
            GameoverPanel.SetActive(true);
        }
    }
}
