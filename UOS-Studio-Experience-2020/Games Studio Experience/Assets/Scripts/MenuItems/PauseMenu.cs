using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{


    public void Pause()
    {
        Time.timeScale = 0;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isPaused = true;
    }

    public void Unpause() 
    {
        Time.timeScale = 1;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().isPaused = false;
    }

    public void RecordPause() 
    {
        GameObject.FindGameObjectWithTag("Analytics").GetComponent<AnalyticsManager>().GamePaused = true;
    }
}
