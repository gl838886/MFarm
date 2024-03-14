using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mfarm.inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("组件获取")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] public Image slotHighlight;
        [SerializeField] private Button slotButton;

        [Header("格子种类")]
        public SlotType slotType;

        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        public bool isSelected;

        public ItemDetails itemDetails;
        public int itemAmount;

        public int slotIndex; //给格子一个序号

        private void Start()
        {
            isSelected = false;
            if (itemDetails==null)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// 更新盒子
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            itemAmount = amount;
            slotImage.sprite = item.itemIcon;
            slotImage.enabled = true;
            amountText.text = amount.ToString();
            slotButton.interactable = true;
        }

        /// <summary>
        /// 清空盒子
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateSlotHightlight(-1);
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails= null;
            slotImage.enabled = false;
            amountText.text = "";
            slotButton.interactable = false; //格子为空时不能被点暗
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null) return;
            isSelected = !isSelected;
            //这样写会出现问题，我可以同时选择很多个slot
            //slotHighlight.gameObject.SetActive(isSelected);

            inventoryUI.UpdateSlotHightlight(slotIndex);

            if(slotType ==SlotType.Bag)
            {
                //只有在背包里，才能点击呼叫举起
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(itemAmount != 0)
            {
                inventoryUI.dragItemImage.enabled = true;
                inventoryUI.dragItemImage.sprite = slotImage.sprite; //只能传sprite
                //inventoryUI.dragItemImage.SetNativeSize();
                
                isSelected = true;
                inventoryUI.UpdateSlotHightlight(slotIndex);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItemImage.gameObject.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItemImage.enabled = false;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
            if(eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() != null)
                {
                    SlotUI targetSlotUI = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                    int targetSlotIndex = targetSlotUI.slotIndex;

                    //在这里需要注意，我们不仅仅只在背包内交换
                    if (targetSlotUI.slotType == SlotType.Bag && slotType == SlotType.Bag) //beibao
                    {
                        InventoryManager.Instance.SwapItem(slotIndex, targetSlotIndex);
                    }
                    else if(targetSlotUI.slotType == SlotType.Bag && slotType == SlotType.Shop) //买
                    {
                        EventHandler.CallShowTadeUIEvent(itemDetails, false);
                    }
                    else if (targetSlotUI.slotType == SlotType.Shop && slotType == SlotType.Bag) //卖
                    {
                        EventHandler.CallShowTadeUIEvent(itemDetails, true);
                    }
                    //人物和箱子交换物品
                    else if(targetSlotUI.slotType != SlotType.Shop && slotType != SlotType.Shop && slotType != targetSlotUI.slotType)
                    {
                        //交换物品，重载SwapItem函数
                        InventoryManager.Instance.SwapItem(slotType, slotIndex, targetSlotUI.slotType, targetSlotIndex);
                    }
                    isSelected = false;
                    inventoryUI.UpdateSlotHightlight(slotIndex);
                }
            }
            else //如果物品被扔到了地上
            {
                Debug.Log("Slot UI");
                //鼠标对应的地面坐标
                var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            }
           
        }
    }

}
