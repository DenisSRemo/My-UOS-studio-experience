using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class AnalyticsManager : MonoBehaviour
{
    public List<string[]> LevelInfo = new List<string[]>();
    public List<string[]> DataToBeExported = new List<string[]>();

    static AnalyticsManager instance;


    //Variables for store
    public int AmountPowerupsUsed;
    public bool ShopUsed, GamePaused, SomethingBought;
    public string PowerupsUsed,ObstaclesSpawned;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject); 
    }

    private void OnApplicationQuit()
    {
        SaveToFile();
        Debug.Log("This is saved");
    }

    public void RecordData(int SceneIndex,int ShotsTaken,int AmountOfPlanets) 
    {
        string[] TempDateRow = new string[9];
        TempDateRow[0] = SceneIndex + "";
        TempDateRow[1] = ShotsTaken + "";
        TempDateRow[2] = AmountPowerupsUsed + "";
        TempDateRow[3] = PowerupsUsed;
        TempDateRow[4] = ShopUsed + "";
        TempDateRow[5] = GamePaused + "";
        TempDateRow[6] = SomethingBought + "";
        TempDateRow[7] = AmountOfPlanets + "";
        TempDateRow[8] = ObstaclesSpawned;
        LevelInfo.Add(TempDateRow);
        ResetVariables();
    }

    void SaveToFile() 
    {
        string[] TempDateRow = new string[9];
        TempDateRow[0] = "Level (IndexNum):";
        TempDateRow[1] = "Amount of shots taken:";
        TempDateRow[2] = "Amount of powerups used:";
        TempDateRow[3] = "What powerups used:";
        TempDateRow[4] = "Was shop used:";
        TempDateRow[5] = "Was game paused:";
        TempDateRow[6] = "Was something bought:";
        TempDateRow[7] = "Amount of planets spawned:";
        TempDateRow[8] = "What obstacles spawned:";
        DataToBeExported.Add(TempDateRow);

        for (int i = 0; i < LevelInfo.Count; i++) 
        {
            DataToBeExported.Add(LevelInfo[i]);
        }

        string[][] Output = new string[DataToBeExported.Count][];

        for (int i = 0; i < Output.Length; i++) 
        {
            Output[i] = DataToBeExported[i];
        }

        int length = Output.GetLength(0);
        string breakline = ",";

        StringBuilder sb = new StringBuilder();

        for (int j = 0; j < length; j++) 
        {
            sb.AppendLine(string.Join(breakline, Output[j]));
        }

        string filePath = System.IO.Directory.GetCurrentDirectory() + "/" + "Analytics.csv";

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    void ResetVariables() 
    {
        AmountPowerupsUsed = 0;
        PowerupsUsed = "";
        ShopUsed = false;
        GamePaused = false;
        SomethingBought = false;
        ObstaclesSpawned = "";
    }
}
