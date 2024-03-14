using UnityEngine;

namespace Mfarm.inventory
{
    public class Box : MonoBehaviour
    {
        public BagItemList_SO boxBagTemplate;
        public BagItemList_SO boxBagData;

        private bool canOpen = false; 
        private bool isOpen;

        public int index;

        public GameObject openSign; //点击鼠标右键打开的标识

        private void OnEnable()
        {
            if(boxBagData == null)
            {
                boxBagData = Instantiate(boxBagTemplate);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "Player")
            {
                canOpen = true;
                openSign.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                canOpen = false;  //里开箱子范围就不能再打开箱子了
                openSign.SetActive(false);
            }
        }

        private void Update()
        {
            if(canOpen && !isOpen && Input.GetMouseButtonDown(1)) //鼠标右键
            {
                EventHandler.CallOpenBaseBagEvent(SlotType.Box, boxBagData);
                isOpen = true;
            }
            else if((!canOpen || Input.GetKeyDown(KeyCode.Escape)) && isOpen) //打开状态下离开或摁esc都可关闭箱子
            {
                EventHandler.CallCloseBaseBagEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
        }

        public void InitBoxDict(int boxIndex)
        {
            index = boxIndex;
            string key = this.name + index;
            //如果字典里存储该箱子
            if(InventoryManager.Instance.GetBagItemListInDict(key)!=null)
            {
                boxBagData.bagItemList = InventoryManager.Instance.GetBagItemListInDict(key);
            }
            else
            {
                InventoryManager.Instance.AddBoxItemInDict(this);
            }
        }
    }
}
