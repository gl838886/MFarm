using Mfarm.inventory;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TradeUI : MonoBehaviour
{
    public Image itemIcon; //ͼ��
    public Text itemName; //��Ʒ����
    public InputField tradeAmount; //��������
    public Button yesButton; //ȷ��
    public Button noButton;

    private ItemDetails itemDetails;
    private bool isSellTrade;

    private void Awake()
    {
        noButton.onClick.AddListener(CancelTrade); //��Ӽ�������
        yesButton.onClick.AddListener(SubmitTrade);
    }

    public void SetUpTradeUI(ItemDetails itemDetails, bool isSell)
    {
        this.itemDetails = itemDetails;
        itemIcon.sprite = itemDetails.itemIcon;
        itemName.text = itemDetails.itemName;
        //�򿪽��׿�ʱ���㣬��ֹ������һ�ε�����
        tradeAmount.text = string.Empty;
        isSellTrade= isSell;
    }

    private void SubmitTrade()
    {
        int amount = Convert.ToInt32(tradeAmount.text);
        InventoryManager.Instance.TradeItem(itemDetails, amount, isSellTrade);
        CancelTrade(); //������ɺ�ر�UI����
    }

    private void CancelTrade()
    {
        this.gameObject.SetActive(false);   
    }
}
