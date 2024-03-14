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

    [SerializeField] private GameObject bottom; //�������Ʒ�������ۣ�������bottom�������

    [Header("����")]
    public GameObject resourcePanel;
    [SerializeField] private Image[] resourceItem;


    public void SetUpItemToolTip(ItemDetails itemDetails, SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType); //itemType��enum�ͣ���Ҫһ��ǿ��ת��
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

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>()); //��ֹ�ӳٵĳ���
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
                //ͼƬ
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
            ItemType.Seed => "����",
            ItemType.Commodity => "��Ʒ",
            ItemType.Furniture => "�Ҿ�",
            ItemType.HoeTool => "����",
            ItemType.ChopTool => "����",
            ItemType.BreakTool => "����",
            ItemType.ReapTool => "����",
            ItemType.WaterTool => "����",
            ItemType.CollectTool => "����",
            _ => "��"

        };
    }
}
