using Mfarm.Save;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static UnityEditor.Progress;

//所有跟inventory相关的，全部添加到一个命名空间

namespace Mfarm.inventory
{
    public class InventoryManager : Singleton<InventoryManager>, ISaveable
    {
        public ItemDataList_SO itemDataList_SO;
        public BagItemList_SO bagItemList_SO;
        public BluePrintDataList_SO bluePrintDataList_SO;
        //当前交换物品的箱子
        public BagItemList_SO currentBoxList_SO;

        //新背包Prefab
        public BagItemList_SO startBag;

        //交易
        public int playerMoney;

        //存储箱子中的物品
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
            //开始就要先更新一下背包
            
        }

        private void OnOpenBaseBagEvent(SlotType slotType, BagItemList_SO boxItemList_SO)
        {
            //获取当前交换物品箱子的list_SO
            currentBoxList_SO = boxItemList_SO;
        }

        //建造完删除图纸和所需资源
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePosition)  
        {
            //删除图纸
            RemoveItemFromBag(ID, 1);
            //删除对应资源的数量
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
            //开始时给玩家一个新背包（装有必备的工具）
            bagItemList_SO = Instantiate(startBag);
            boxItemDict.Clear();
            playerMoney = Settings.startMoney;
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        /// <summary>
        /// 通过ID返回物品的itemDatails
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails getItemDetails(int ID)
        {
            //i是我要返回的ItemDetails，i goes to要itemID和ID相同    
            //List<T>.Find 后面用了简捷lambda表达式，详见CSDN的收藏
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID.Equals(ID));
        }

        /// <summary>
        /// 添加物品到player背包里
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestroy">是否要被销毁</param>
        public void AddItem(Item item, bool toDestroy)
        {
            //背包是否有当前的物品
            //判断背包是否为空，哪里为空
            
            var index = GetIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);

            if(toDestroy)
            {
                Destroy(item.gameObject);
            }

            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        /// <summary>
        /// 查看背包是否未满
        /// </summary>
        /// <returns>未满返回true</returns>
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
        /// 通过物品ID来找到物品在背包的位置
        /// </summary>
        /// <param name="ID">物品的ID</param>
        /// <returns>未找到则返回-1</returns>
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
        /// 通过index将物品添加进背包
        /// </summary>
        /// <param name="id">物品的ID</param>
        /// <param name="index">要添加物品在列表中的序号</param>
        /// <param name="amount">要添加物品的个数</param>
        private void AddItemAtIndex(int id, int index, int amount)
        {
            
            if (index == -1 && CheckBagCapacity()) //如果背包内无当前物品&&背包不满
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
            else //背包内有当前物品
            {
                int currentAmount = bagItemList_SO.bagItemList[index].itemCount + amount;
                BagItem bagItem = new BagItem { itemID = id, itemCount = currentAmount };
                bagItemList_SO.bagItemList[index] = bagItem;
            }
        }

        /// <summary>
        /// 交换物品（背包内）
        /// </summary>
        /// <param name="currentIndex">当前序号</param>
        /// <param name="targetIndex">目标序号</param>
        public void SwapItem(int currentIndex, int targetIndex)
        {
            //通过currentIndex和targetIndex来获得背包内相应物品
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
        /// 交换物品（玩家和箱子）
        /// </summary>
        /// <param name="currentLocation">当前位置</param>
        /// <param name="currentIndex">当前序号</param>
        /// <param name="targetLocation">目标位置</param>
        /// <param name="targetIndex">目标序号</param>
        public void SwapItem(SlotType currentType, int currentIndex, SlotType targetTYpe, int targetIndex)
        {
            //1.交换物品首先明确玩家和哪个箱子进行交换
            //2.明确目标是人还是箱子

            BagItem currentItem = new BagItem();
            BagItem targetItem =new BagItem();
            List<BagItem> currentList = new List<BagItem>();
            List<BagItem> targetList = new List<BagItem>(); 

            if(currentType == SlotType.Bag) //从玩家到背包
            {
                currentItem = bagItemList_SO.bagItemList[currentIndex];
                targetItem = currentBoxList_SO.bagItemList[targetIndex];
                currentList = bagItemList_SO.bagItemList;
                targetList = currentBoxList_SO.bagItemList;
            }
            else //从背包到玩家
            {
                currentItem = currentBoxList_SO.bagItemList[currentIndex];
                targetItem = bagItemList_SO.bagItemList[targetIndex];
                currentList = currentBoxList_SO.bagItemList;
                targetList = bagItemList_SO.bagItemList;
            }

            //如果交换物品相同
            if(currentItem.itemID == targetItem.itemID)
            {
                targetItem.itemCount += currentItem.itemCount;
                targetList[targetIndex] = targetItem;
                currentList[currentIndex] = new BagItem();
            }
            //交换物品不同且目标格子不为空
            else if(currentItem.itemID != targetItem.itemID && targetItem.itemCount != 0)
            {
                targetList[targetIndex] = currentItem;
                currentList[currentIndex] = targetItem;
            }
            else //交换物品不同且目标格子不为空
            {
                targetList[targetIndex] = currentItem;
                currentList[currentIndex] = new BagItem();
            }

            if (currentType == SlotType.Bag) //从玩家到背包
            {
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, currentList);
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Box, targetList);
            }
            else //从背包到玩家
            {
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Box, currentList);
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, targetList);
            }
        }

        private void RemoveItemFromBag(int itemId, int amount)
        {
            int index = GetIndexInBag(itemId);
            if (bagItemList_SO.bagItemList[index].itemCount > amount) //如果大于移除数量
            {
                int newAmount = bagItemList_SO.bagItemList[index].itemCount - amount;
                var item = new BagItem { itemID=itemId, itemCount=newAmount };
                bagItemList_SO.bagItemList[index] = item;
            }
            else if (bagItemList_SO.bagItemList[index].itemCount == amount) //如果等于移除数量
            {
                var item = new BagItem();
                bagItemList_SO.bagItemList[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            //计算价格
            int cost = itemDetails.itemPrice * amount;
            //获得在背包中的序号
            int index = GetIndexInBag(itemDetails.itemID);

            if(isSellTrade) //卖
            {
                if (bagItemList_SO.bagItemList[index].itemCount >= amount) //有足够的数量卖
                {
                    RemoveItemFromBag(itemDetails.itemID, amount); //移除
                    cost = (int)(cost * itemDetails.sellPercentage); //卖的时候价格便宜，乘系数
                    playerMoney += cost;
                }
            }
            else //买
            {
                if(playerMoney - cost >=0) //有足够钱来买
                {
                    if(CheckBagCapacity()) //有足够空间
                    {
                        AddItemAtIndex(itemDetails.itemID, index, amount); //加入
                    }
                    playerMoney -= cost;
                }
            }
            //刷新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bagItemList_SO.bagItemList);
        }

        //检查背包内物品是否可以建造
        public bool CheckBuild(int ID)
        {
            //通过id拿到物品的蓝图
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
        /// 获取箱子字典中列表
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
        /// 在字典中添加每个盒子
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxItemInDict(Box box)
        {
            string key = box.name + box.index;
            if (!boxItemDict.ContainsKey(key)) //如果字典中不包含，则加入
                boxItemDict.Add(key, box.boxBagData.bagItemList);
        }

        public GameSaveData GenerateGameData()
        {
            GameSaveData saveData  = new GameSaveData();
            saveData.playerMoney = playerMoney;

            saveData.boxItemDict = new Dictionary<string, List<BagItem>>();
            //先将背包内的物品添加进去
            saveData.boxItemDict.Add(bagItemList_SO.name, bagItemList_SO.bagItemList);
            //通过循环，将场景内箱子的物品也添加进去
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



