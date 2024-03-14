using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mfarm.inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("�����ȡ")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] public Image slotHighlight;
        [SerializeField] private Button slotButton;

        [Header("��������")]
        public SlotType slotType;

        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        public bool isSelected;

        public ItemDetails itemDetails;
        public int itemAmount;

        public int slotIndex; //������һ�����

        private void Start()
        {
            isSelected = false;
            if (itemDetails==null)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// ���º���
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
        /// ��պ���
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
            slotButton.interactable = false; //����Ϊ��ʱ���ܱ��㰵
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null) return;
            isSelected = !isSelected;
            //����д��������⣬�ҿ���ͬʱѡ��ܶ��slot
            //slotHighlight.gameObject.SetActive(isSelected);

            inventoryUI.UpdateSlotHightlight(slotIndex);

            if(slotType ==SlotType.Bag)
            {
                //ֻ���ڱ�������ܵ�����о���
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(itemAmount != 0)
            {
                inventoryUI.dragItemImage.enabled = true;
                inventoryUI.dragItemImage.sprite = slotImage.sprite; //ֻ�ܴ�sprite
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

                    //��������Ҫע�⣬���ǲ�����ֻ�ڱ����ڽ���
                    if (targetSlotUI.slotType == SlotType.Bag && slotType == SlotType.Bag) //beibao
                    {
                        InventoryManager.Instance.SwapItem(slotIndex, targetSlotIndex);
                    }
                    else if(targetSlotUI.slotType == SlotType.Bag && slotType == SlotType.Shop) //��
                    {
                        EventHandler.CallShowTadeUIEvent(itemDetails, false);
                    }
                    else if (targetSlotUI.slotType == SlotType.Shop && slotType == SlotType.Bag) //��
                    {
                        EventHandler.CallShowTadeUIEvent(itemDetails, true);
                    }
                    //��������ӽ�����Ʒ
                    else if(targetSlotUI.slotType != SlotType.Shop && slotType != SlotType.Shop && slotType != targetSlotUI.slotType)
                    {
                        //������Ʒ������SwapItem����
                        InventoryManager.Instance.SwapItem(slotType, slotIndex, targetSlotUI.slotType, targetSlotIndex);
                    }
                    isSelected = false;
                    inventoryUI.UpdateSlotHightlight(slotIndex);
                }
            }
            else //�����Ʒ���ӵ��˵���
            {
                Debug.Log("Slot UI");
                //����Ӧ�ĵ�������
                var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
                EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            }
           
        }
    }

}
