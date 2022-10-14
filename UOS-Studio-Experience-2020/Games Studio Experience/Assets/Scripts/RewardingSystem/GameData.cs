//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;



public class Data
{
    
    private Data()
    {
    }

    static private Data _instance;
    static public Data instance
    {
        get
        {
            if (_instance == null)
                _instance = new Data();
            return _instance;
        }
    }

    public int Gold;
    public int Gems;
    public int Score; //this needs to be reset each time the level resets

}