using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mfarm.inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemToolTip itemToolTip;

        [SerializeField] private SlotUI[] playerSlots;

        [Header("��קͼƬ")]
        public Image dragItemImage;

        [Header("����UI")]
        [SerializeField] private GameObject bagUI;
        private bool isBagOpen;

        [Header("ͨ�ñ���")]
        [SerializeField] private GameObject baseBag; //�̵�Ļ�������
        public GameObject shopSlotPrefab; //ÿ������

        [Header("����")]
        public GameObject boxSlotPrefab;

        [Header("����UI")]
        public TradeUI tradeUI;
        public TextMeshProUGUI moneyAmountUI; //��Ǯ��

        [SerializeField] private List<SlotUI> baseBagSlots;

        private void Start()
        {
            //��ÿ������һ�����
            for(int i=0;i<playerSlots.Length;i++)
            {
                playerSlots[i].slotIndex = i;
            }

            //�жϱ�����ʼ�Ǽ���Ǳ�����״̬
            isBagOpen = bagUI.activeInHierarchy; 

            moneyAmountUI.text = InventoryManager.Instance.playerMoney.ToString();
        }

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdataInventoryUI;
            EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
            EventHandler.OpenBaseBagEvent += OnOpenBaseBagEvent;
            EventHandler.CloseBaseBagEvent += OnCloseBaseBagEvent;
            EventHandler.ShowTradeUIEvent += OnShowTradeUIEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdataInventoryUI;
            EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
            EventHandler.OpenBaseBagEvent -= OnOpenBaseBagEvent;
            EventHandler.CloseBaseBagEvent -= OnCloseBaseBagEvent;
            EventHandler.ShowTradeUIEvent -= OnShowTradeUIEvent;
        }

        private void OnShowTradeUIEvent(ItemDetails itemDetails, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetUpTradeUI(itemDetails, isSell);
        }

        private void OnOpenBaseBagEvent(SlotType slotType, BagItemList_SO bagData)
        {
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,
                SlotType.Box => boxSlotPrefab,
                _=>null
            } ;

            //���ɱ���UI
            baseBag.SetActive(true);
            baseBagSlots = new List<SlotUI>();

            for(int i=0;i< bagData.bagItemList.Count;i++)
            {
                var slot =Instantiate(prefab, baseBag.transform.GetChild(1)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            //ǿ�Ƹ��²���
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)baseBag.transform);

            //�򿪱���
            if(slotType== SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
                bagUI.SetActive(true);
                isBagOpen= true;
            }

            //����UI��ʾ
            OnUpdataInventoryUI(InventoryLocation.Box, bagData.bagItemList);
        }

        //�رձ����¼�����
        private void OnCloseBaseBagEvent(SlotType slotType, BagItemList_SO bagData)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);
            UpdateSlotHightlight(-1); //���Ϊ-1����һ����������

            foreach(var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            //�رձ���
            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                bagUI.SetActive(false);
                isBagOpen = false;
            }
        }

        private void OnBeforeUnLoadSceneEvent()
        {
            UpdateSlotHightlight(-1); //���Ϊ-1����һ����������
        }

        /// <summary>
        /// ����ָ��λ�õ�SlotUI�¼�����
        /// </summary>
        /// <param name="location">λ��</param>
        /// <param name="list">�б�</param>
        private void OnUpdataInventoryUI(InventoryLocation location, List<BagItem> list)
        {
            switch(location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemCount > 0)
                        {
                            var item = InventoryManager.Instance.getItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemCount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemCount > 0)
                        {
                            var item = InventoryManager.Instance.getItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].itemCount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
            moneyAmountUI.text = InventoryManager.Instance.playerMoney.ToString();
        }

        public void OpenBagUI()
        {
            isBagOpen = !isBagOpen;
            //�����Ϊ�棬������ʾ
            bagUI.SetActive(isBagOpen);
        }

        public void UpdateSlotHightlight(int index)
        {
            foreach(var slot in playerSlots)
            {
                if(slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHighlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected= false;
                    slot.slotHighlight.gameObject.SetActive(false);
                }
            }
        }
    }
}

