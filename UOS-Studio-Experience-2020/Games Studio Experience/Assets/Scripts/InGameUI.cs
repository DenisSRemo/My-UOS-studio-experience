using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public  TextMeshProUGUI gold;
    public  TextMeshProUGUI gems;
    // Start is called before the first frame update
    void Start()
    {

        gold.text = ""+Data.instance.Gold.ToString();
        gems.text = ""+Data.instance.Gems.ToString();
     
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public  void UpdateGemsUI()
    {
        gems.text = "" + Data.instance.Gems.ToString();
    }

    public  void UpdateCoinsUI()
    {
        gold.text = "" + Data.instance.Gold.ToString();
    }



    
}
