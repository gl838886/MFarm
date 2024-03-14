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

        public GameObject openSign; //�������Ҽ��򿪵ı�ʶ

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
                canOpen = false;  //�￪���ӷ�Χ�Ͳ����ٴ�������
                openSign.SetActive(false);
            }
        }

        private void Update()
        {
            if(canOpen && !isOpen && Input.GetMouseButtonDown(1)) //����Ҽ�
            {
                EventHandler.CallOpenBaseBagEvent(SlotType.Box, boxBagData);
                isOpen = true;
            }
            else if((!canOpen || Input.GetKeyDown(KeyCode.Escape)) && isOpen) //��״̬���뿪����esc���ɹر�����
            {
                EventHandler.CallCloseBaseBagEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
        }

        public void InitBoxDict(int boxIndex)
        {
            index = boxIndex;
            string key = this.name + index;
            //����ֵ���洢������
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
