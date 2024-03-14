using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEditor.Overlays;
using Mfarm.inventory;

public class ItemToolTip : Singleton<ItemToolTip>
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Text priceText;

    [SerializeField] private GameObject bottom; //如果该物品不可销售，则隐藏bottom这个物体

    [Header("建造")]
    public GameObject resourcePanel;
    [SerializeField] private Image[] resourceItem;


    public void SetUpItemToolTip(ItemDetails itemDetails, SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType); //itemType是enum型，需要一个强制转换
        descriptionText.text = itemDetails.itemDescription;
        var price = itemDetails.itemPrice;

        if(itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            bottom.SetActive(true);
            if (slotType == SlotType.Bag)
            {
                price = (int)(price * itemDetails.sellPercentage);
            }
            //Debug.Log(itemDetails.itemName + " " + itemDetails.itemType.ToString() + " " + itemDetails.itemDescription);
            priceText.text = price.ToString();
        }
        else
        {
            bottom.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>()); //防止延迟的出现
    }

    public void SetUpResourceItem(int ID)
    {
        var bluePrintItemDetails = InventoryManager.Instance.bluePrintDataList_SO.GetBluePrintDetails(ID);
        for(int i = 0; i < resourceItem.Length; i++)
        {
            
            if (i < bluePrintItemDetails.resourceItem.Length)
            {
                var item = bluePrintItemDetails.resourceItem[i];
                resourceItem[i].gameObject.SetActive(true);
                //图片
                resourceItem[i].sprite = InventoryManager.Instance.getItemDetails(item.itemID).itemIcon;
                resourceItem[i].transform.GetChild(0).GetComponent<Text>().text = item.itemCount.ToString();
            }
            else
            {
                resourceItem[i].gameObject.SetActive(false);
            }
        }
    }
    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed => "种子",
            ItemType.Commodity => "商品",
            ItemType.Furniture => "家具",
            ItemType.HoeTool => "工具",
            ItemType.ChopTool => "工具",
            ItemType.BreakTool => "工具",
            ItemType.ReapTool => "工具",
            ItemType.WaterTool => "工具",
            ItemType.CollectTool => "工具",
            _ => "无"

        };
    }
}
