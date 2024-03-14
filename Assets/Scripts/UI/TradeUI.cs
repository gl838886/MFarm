using Mfarm.inventory;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TradeUI : MonoBehaviour
{
    public Image itemIcon; //图标
    public Text itemName; //物品名称
    public InputField tradeAmount; //交易数量
    public Button yesButton; //确认
    public Button noButton;

    private ItemDetails itemDetails;
    private bool isSellTrade;

    private void Awake()
    {
        noButton.onClick.AddListener(CancelTrade); //添加监听函数
        yesButton.onClick.AddListener(SubmitTrade);
    }

    public void SetUpTradeUI(ItemDetails itemDetails, bool isSell)
    {
        this.itemDetails = itemDetails;
        itemIcon.sprite = itemDetails.itemIcon;
        itemName.text = itemDetails.itemName;
        //打开交易框时清零，防止存留上一次的数据
        tradeAmount.text = string.Empty;
        isSellTrade= isSell;
    }

    private void SubmitTrade()
    {
        int amount = Convert.ToInt32(tradeAmount.text);
        InventoryManager.Instance.TradeItem(itemDetails, amount, isSellTrade);
        CancelTrade(); //交易完成后关闭UI界面
    }

    private void CancelTrade()
    {
        this.gameObject.SetActive(false);   
    }
}
