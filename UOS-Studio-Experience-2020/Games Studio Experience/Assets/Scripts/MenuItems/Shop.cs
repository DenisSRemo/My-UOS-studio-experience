using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public void GiveCurrency() 
    {
        Data.instance.Gold += 100;
        Data.instance.Gems += 100;
    }
    public void RecordShop() 
    {
        GameObject.FindGameObjectWithTag("Analytics").GetComponent<AnalyticsManager>().ShopUsed = true;
    }
    public void RecordPurchase() 
    {
        GameObject.FindGameObjectWithTag("Analytics").GetComponent<AnalyticsManager>().SomethingBought = true;
    }
}
