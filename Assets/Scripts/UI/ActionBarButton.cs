using UnityEngine;

namespace Mfarm.inventory
{
    //快捷键控制物品栏
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slotUI;

        private bool canUse;

        private void Awake()
        {
            slotUI= GetComponent<SlotUI>();
        }

        private void OnEnable()
        {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        }

        private void OnUpdateGameStateEvent(GameState gameState)
        {
            canUse = gameState == GameState.GamePlay;
        }

        private void Update()
        {
            if(Input.GetKeyDown(key) && canUse)
            {
                if(slotUI.itemDetails!=null)
                {
                    slotUI.isSelected = !slotUI.isSelected;
                }
                if(slotUI.isSelected)
                {
                    slotUI.inventoryUI.UpdateSlotHightlight(slotUI.slotIndex);
                }
                else
                {
                    slotUI.inventoryUI.UpdateSlotHightlight(-1);
                }
                EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);
            }
        }
    }
}

