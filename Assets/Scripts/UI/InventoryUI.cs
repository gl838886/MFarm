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

        [Header("拖拽图片")]
        public Image dragItemImage;

        [Header("背包UI")]
        [SerializeField] private GameObject bagUI;
        private bool isBagOpen;

        [Header("通用背包")]
        [SerializeField] private GameObject baseBag; //商店的基础背包
        public GameObject shopSlotPrefab; //每个格子

        [Header("箱子")]
        public GameObject boxSlotPrefab;

        [Header("交易UI")]
        public TradeUI tradeUI;
        public TextMeshProUGUI moneyAmountUI; //金钱数

        [SerializeField] private List<SlotUI> baseBagSlots;

        private void Start()
        {
            //给每个格子一个序号
            for(int i=0;i<playerSlots.Length;i++)
            {
                playerSlots[i].slotIndex = i;
            }

            //判断背包开始是激活还是被激活状态
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

            //生成背包UI
            baseBag.SetActive(true);
            baseBagSlots = new List<SlotUI>();

            for(int i=0;i< bagData.bagItemList.Count;i++)
            {
                var slot =Instantiate(prefab, baseBag.transform.GetChild(1)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            //强制更新布局
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)baseBag.transform);

            //打开背包
            if(slotType== SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
                bagUI.SetActive(true);
                isBagOpen= true;
            }

            //更新UI显示
            OnUpdataInventoryUI(InventoryLocation.Box, bagData.bagItemList);
        }

        //关闭背包事件函数
        private void OnCloseBaseBagEvent(SlotType slotType, BagItemList_SO bagData)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);
            UpdateSlotHightlight(-1); //序号为-1代表一个都亮不了

            foreach(var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            //关闭背包
            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                bagUI.SetActive(false);
                isBagOpen = false;
            }
        }

        private void OnBeforeUnLoadSceneEvent()
        {
            UpdateSlotHightlight(-1); //序号为-1代表一个都亮不了
        }

        /// <summary>
        /// 更新指定位置的SlotUI事件函数
        /// </summary>
        /// <param name="location">位置</param>
        /// <param name="list">列表</param>
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
            //如果打开为真，进行显示
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

