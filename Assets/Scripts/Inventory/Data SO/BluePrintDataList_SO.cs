using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BluePrintDataList_SO", menuName = "Inventory/BluePrintDataList_SO")]

public class BluePrintDataList_SO : ScriptableObject
{
    public List<BluePrintDetails> bluePrintDataList;

    public BluePrintDetails GetBluePrintDetails(int itemID)
    {
        return bluePrintDataList.Find(b => b.ID == itemID);
    }
}


[System.Serializable]
public class BluePrintDetails
{
    public int ID;
    public BagItem[] resourceItem = new BagItem[4];
    public GameObject buildPrefab;
}


