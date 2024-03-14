using Mfarm.Save;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static UnityEditor.Progress;

//���и�inventory��صģ�ȫ����ӵ�һ�������ռ�

namespace Mfarm.inventory
{
    public class InventoryManager : Singleton<InventoryManager>, ISaveable
    {
        public ItemDataList_SO itemDataList_SO;
        public BagItemList_SO bagItemList_SO;
        public BluePrintDataList_SO bluePrintDataList_SO;
        //��ǰ������Ʒ������
        public BagItemList_SO currentBoxList_SO;

        //�±���Prefab
        public BagItemList_SO startBag;

        //����
        public int playerMoney;

        //�洢�����е���Ʒ
        private Dictionary<string, List<BagItem>> boxItemDict = new Dictionary<string, List<BagItem>>();
        public int boxAmount => boxItemDict.Count;

        public string GUID => GetComponent<DataGUID>().GUID;

        private void OnEnable()
        {
            EventHandler.DropItemInBag += OnDropItemInBag;
            EventHandler.HarvestCropEvent += OnHarvestCropEvent;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.OpenBaseBagEvent += OnOpenBaseBagEvent;
            EventHandler.StartNewGame += OnStartNewGame;
        }

        private void OnDisable()
        {
            EventHandler.DropItemInBag -= OnDropItemInBag;
            EventHandler.HarvestCropEvent -= OnHarvestCropEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.OpenBaseBagEvent -= OnOpenBaseBagEvent;
            EventHandler.StartNewGame -= OnStartNewGame;
        }

        private void Start()
        {
            ISaveable saveable = this;
            SaveLoadManager.Instance.RegisterSaveable(saveable);
            //��ʼ��Ҫ�ȸ���һ�±���
            
        }

        private void OnOpenBaseBagEvent(SlotType slotType, BagItemList_SO boxItemList_SO)
        {
            //��ȡ��ǰ������Ʒ���ӵ�list_SO
            currentBoxList_SO = boxItemList_SO;
        }

        //������ɾ��ͼֽ��������Դ
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePosition)  
        {
            //ɾ��ͼֽ
            RemoveItemFromBag(ID, 1);
            //ɾ����Ӧ��Դ������
            var resourceItem = bluePrintDataList_SO.GetBluePrintDetails(ID).resourceItem;
            foreach(var item in resourceItem)
            {
                RemoveItemFromBag(item.itemID, item.itemCount);
            }
        }

        private void OnHarvestCropEvent(int ID)
        {
            var index = GetIndexInBag(ID);
            AddItemAtIndex(ID, index, 1);

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        private void OnDropItemInBag(int itemId, Vector3 pos, ItemType itemType)
        {
            RemoveItemFromBag(itemId, 1);
        }

        private void OnStartNewGame(int index)
        {
            //��ʼʱ�����һ���±�����װ�бر��Ĺ��ߣ�
            bagItemList_SO = Instantiate(startBag);
            boxItemDict.Clear();
            playerMoney = Settings.startMoney;
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        /// <summary>
        /// ͨ��ID������Ʒ��itemDatails
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails getItemDetails(int ID)
        {
            //i����Ҫ���ص�ItemDetails��i goes toҪitemID��ID��ͬ    
            //List<T>.Find �������˼��lambda���ʽ�����CSDN���ղ�
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID.Equals(ID));
        }

        /// <summary>
        /// �����Ʒ��player������
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestroy">�Ƿ�Ҫ������</param>
        public void AddItem(Item item, bool toDestroy)
        {
            //�����Ƿ��е�ǰ����Ʒ
            //�жϱ����Ƿ�Ϊ�գ�����Ϊ��
            
            var index = GetIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);

            if(toDestroy)
            {
                Destroy(item.gameObject);
            }

            //����UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        /// <summary>
        /// �鿴�����Ƿ�δ��
        /// </summary>
        /// <returns>δ������true</returns>
        private bool CheckBagCapacity()
        {
            for(int i = 0; i < bagItemList_SO.bagItemList.Count; i++)
            {
                if (bagItemList_SO.bagItemList[i].itemID == 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// ͨ����ƷID���ҵ���Ʒ�ڱ�����λ��
        /// </summary>
        /// <param name="ID">��Ʒ��ID</param>
        /// <returns>δ�ҵ��򷵻�-1</returns>
        private int GetIndexInBag(int ID)
        {
            for(int i = 0; i< bagItemList_SO.bagItemList.Count; i++)
            {
                if (bagItemList_SO.bagItemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// ͨ��index����Ʒ��ӽ�����
        /// </summary>
        /// <param name="id">��Ʒ��ID</param>
        /// <param name="index">Ҫ�����Ʒ���б��е����</param>
        /// <param name="amount">Ҫ�����Ʒ�ĸ���</param>
        private void AddItemAtIndex(int id, int index, int amount)
        {
            
            if (index == -1 && CheckBagCapacity()) //����������޵�ǰ��Ʒ&&��������
            {
                BagItem bagItem = new BagItem { itemID = id, itemCount = amount };
                for (int i = 0; i < bagItemList_SO.bagItemList.Count; i++)
                {
                    if (bagItemList_SO.bagItemList[i].itemID == 0)
                    {
                        bagItemList_SO.bagItemList[i] = bagItem;
                        break;
                    }
                }
            }
            else //�������е�ǰ��Ʒ
            {
                int currentAmount = bagItemList_SO.bagItemList[index].itemCount + amount;
                BagItem bagItem = new BagItem { itemID = id, itemCount = currentAmount };
                bagItemList_SO.bagItemList[index] = bagItem;
            }
        }

        /// <summary>
        /// ������Ʒ�������ڣ�
        /// </summary>
        /// <param name="currentIndex">��ǰ���</param>
        /// <param name="targetIndex">Ŀ�����</param>
        public void SwapItem(int currentIndex, int targetIndex)
        {
            //ͨ��currentIndex��targetIndex����ñ�������Ӧ��Ʒ
            BagItem currentItem = bagItemList_SO.bagItemList[currentIndex];
            BagItem targetItem = bagItemList_SO.bagItemList[targetIndex];

            if(targetItem.itemCount !=0)
            {
                bagItemList_SO.bagItemList[currentIndex] = targetItem;
                bagItemList_SO.bagItemList[targetIndex] = currentItem;
            }
            else
            {
                bagItemList_SO.bagItemList[targetIndex] = currentItem;
                bagItemList_SO.bagItemList[currentIndex] = new BagItem();
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        /// <summary>
        /// ������Ʒ����Һ����ӣ�
        /// </summary>
        /// <param name="currentLocation">��ǰλ��</param>
        /// <param name="currentIndex">��ǰ���</param>
        /// <param name="targetLocation">Ŀ��λ��</param>
        /// <param name="targetIndex">Ŀ�����</param>
        public void SwapItem(SlotType currentType, int currentIndex, SlotType targetTYpe, int targetIndex)
        {
            //1.������Ʒ������ȷ��Һ��ĸ����ӽ��н���
            //2.��ȷĿ�����˻�������

            BagItem currentItem = new BagItem();
            BagItem targetItem =new BagItem();
            List<BagItem> currentList = new List<BagItem>();
            List<BagItem> targetList = new List<BagItem>(); 

            if(currentType == SlotType.Bag) //����ҵ�����
            {
                currentItem = bagItemList_SO.bagItemList[currentIndex];
                targetItem = currentBoxList_SO.bagItemList[targetIndex];
                currentList = bagItemList_SO.bagItemList;
                targetList = currentBoxList_SO.bagItemList;
            }
            else //�ӱ��������
            {
                currentItem = currentBoxList_SO.bagItemList[currentIndex];
                targetItem = bagItemList_SO.bagItemList[targetIndex];
                currentList = currentBoxList_SO.bagItemList;
                targetList = bagItemList_SO.bagItemList;
            }

            //���������Ʒ��ͬ
            if(currentItem.itemID == targetItem.itemID)
            {
                targetItem.itemCount += currentItem.itemCount;
                targetList[targetIndex] = targetItem;
                currentList[currentIndex] = new BagItem();
            }
            //������Ʒ��ͬ��Ŀ����Ӳ�Ϊ��
            else if(currentItem.itemID != targetItem.itemID && targetItem.itemCount != 0)
            {
                targetList[targetIndex] = currentItem;
                currentList[currentIndex] = targetItem;
            }
            else //������Ʒ��ͬ��Ŀ����Ӳ�Ϊ��
            {
                targetList[targetIndex] = currentItem;
                currentList[currentIndex] = new BagItem();
            }

            if (currentType == SlotType.Bag) //����ҵ�����
            {
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, currentList);
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Box, targetList);
            }
            else //�ӱ��������
            {
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Box, currentList);
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, targetList);
            }
        }

        private void RemoveItemFromBag(int itemId, int amount)
        {
            int index = GetIndexInBag(itemId);
            if (bagItemList_SO.bagItemList[index].itemCount > amount) //��������Ƴ�����
            {
                int newAmount = bagItemList_SO.bagItemList[index].itemCount - amount;
                var item = new BagItem { itemID=itemId, itemCount=newAmount };
                bagItemList_SO.bagItemList[index] = item;
            }
            else if (bagItemList_SO.bagItemList[index].itemCount == amount) //��������Ƴ�����
            {
                var item = new BagItem();
                bagItemList_SO.bagItemList[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            //����۸�
            int cost = itemDetails.itemPrice * amount;
            //����ڱ����е����
            int index = GetIndexInBag(itemDetails.itemID);

            if(isSellTrade) //��
            {
                if (bagItemList_SO.bagItemList[index].itemCount >= amount) //���㹻��������
                {
                    RemoveItemFromBag(itemDetails.itemID, amount); //�Ƴ�
                    cost = (int)(cost * itemDetails.sellPercentage); //����ʱ��۸���ˣ���ϵ��
                    playerMoney += cost;
                }
            }
            else //��
            {
                if(playerMoney - cost >=0) //���㹻Ǯ����
                {
                    if(CheckBagCapacity()) //���㹻�ռ�
                    {
                        AddItemAtIndex(itemDetails.itemID, index, amount); //����
                    }
                    playerMoney -= cost;
                }
            }
            //ˢ��UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        //��鱳������Ʒ�Ƿ���Խ���
        public bool CheckBuild(int ID)
        {
            //ͨ��id�õ���Ʒ����ͼ
            var bluePrintDetails = bluePrintDataList_SO.GetBluePrintDetails(ID);
            foreach(var resourceItem in bluePrintDetails.resourceItem)
            {
                BagItem bagItem = bagItemList_SO.GetBagItem(resourceItem.itemID);
                if(bagItem.itemCount >= resourceItem.itemCount)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// ��ȡ�����ֵ����б�
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<BagItem> GetBagItemListInDict(string key)
        {
            if (boxItemDict.ContainsKey(key))
            {
                return boxItemDict[key];
            }
            else return null;
        }

        /// <summary>
        /// ���ֵ������ÿ������
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxItemInDict(Box box)
        {
            string key = box.name + box.index;
            if (!boxItemDict.ContainsKey(key)) //����ֵ��в������������
                boxItemDict.Add(key, box.boxBagData.bagItemList);
        }

        public GameSaveData GenerateGameData()
        {
            GameSaveData saveData  = new GameSaveData();
            saveData.playerMoney = playerMoney;

            saveData.boxItemDict = new Dictionary<string, List<BagItem>>();
            //�Ƚ������ڵ���Ʒ��ӽ�ȥ
            saveData.boxItemDict.Add(bagItemList_SO.name, bagItemList_SO.bagItemList);
            //ͨ��ѭ���������������ӵ���ƷҲ��ӽ�ȥ
            foreach(var box in boxItemDict)
            {
                saveData.boxItemDict.Add(box.Key, box.Value);
            }
            return saveData;
        }

        public void RestoreGameData(GameSaveData data)
        {
            this.playerMoney = data.playerMoney;
            bagItemList_SO = Instantiate(startBag);
            bagItemList_SO.bagItemList = data.boxItemDict[bagItemList_SO.name];

            foreach(var box in data.boxItemDict)
            {
                if(boxItemDict.ContainsKey(box.Key))
                {
                    boxItemDict[box.Key] = box.Value;
                }
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }
    }
}



