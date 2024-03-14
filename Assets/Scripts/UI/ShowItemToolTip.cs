using Mfarm.inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mfarm.inventory
{
    public class ShowItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SlotUI slotUI;
        private InventoryUI inventoryUI;
         

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
            inventoryUI = GetComponentInParent<InventoryUI>();
             
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null)
            {
                inventoryUI.itemToolTip.gameObject.SetActive(true);
                inventoryUI.itemToolTip.SetUpItemToolTip(slotUI.itemDetails, slotUI.slotType);
                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * 60;
                if(slotUI.itemDetails.itemType == ItemType.Furniture)
                {
                    inventoryUI.itemToolTip.resourcePanel.SetActive(true);
                    inventoryUI.itemToolTip.SetUpResourceItem(slotUI.itemDetails.itemID);
                }
                else
                {
                    inventoryUI.itemToolTip.resourcePanel.SetActive(false);
                }
            }
            else
            {
                inventoryUI.itemToolTip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);
        }


    }
}


