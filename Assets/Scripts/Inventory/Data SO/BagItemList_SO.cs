using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BagItemList_SO", menuName = "Inventory/BagItemList_SO")]

public class BagItemList_SO : ScriptableObject
{
    public List<BagItem> bagItemList;

    public BagItem GetBagItem(int ID)
    {
        return bagItemList.Find(b => b.itemID == ID);
    }
}
