using Mfarm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;

namespace Mfarm.inventory
{
    public class ItemManager : MonoBehaviour, ISaveable
    {
        public Item itemPrefab;
        public Item cropGrass01;
        public Item cropGrass02;
        public Item cropGrass03;
        public Item itemBouncePrefab;
        public Transform playerTransform=>FindObjectOfType<Player>().transform;

        public string GUID => GetComponent<DataGUID>().GUID;

        //字典，用来存储场景里的物品,string是场景名，List列表用来存储场景里的物品
        public Dictionary<string,List<SceneItem>> sceneItemDict = new Dictionary<string,List<SceneItem>>();
        //新建一个字典，用来存储场景中 建造 的物品，方法同上
        //string-场景名，每个场景名对应一个列表
        public Dictionary<string, List<FurnitureItem>> furnitureItemDict = new Dictionary<string, List<FurnitureItem>>();

        public Transform itemParentTransform; //Instantiate 的最后一个参量，希望都在一个parent下
        public Transform cropParentTransform;

        private void Start()
        {
            ISaveable saveable = this;
            SaveLoadManager.Instance.RegisterSaveable(saveable);
        }

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
            EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
            EventHandler.DropItemInBag += OnDropItemInBag;
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.StartNewGame += OnStartNewGame;
        }


        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
            EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
            EventHandler.DropItemInBag -= OnDropItemInBag;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.StartNewGame -= OnStartNewGame;
        }


        private void OnBuildFurnitureEvent(int ID, Vector3 mousePosition)
        {
            BluePrintDetails bluePrintDetails = InventoryManager.Instance.bluePrintDataList_SO.GetBluePrintDetails(ID);
            //Debug.Log(mousePosition); z轴出现问题
           
            //mousePosition.z += 10;
            var buildItem = Instantiate(bluePrintDetails.buildPrefab, mousePosition, Quaternion.identity, itemParentTransform);

            //每建造一个箱子，都要给箱子一个独特的标识index
            if (buildItem.GetComponent<Box>())  //如果建造物是箱子
            {
                //序号为箱子数量
                buildItem.GetComponent<Box>().index = InventoryManager.Instance.boxAmount;
                //更新字典
                buildItem.GetComponent<Box>().InitBoxDict(buildItem.GetComponent<Box>().index);
            }
        }

        private void OnBeforeUnLoadSceneEvent()
        {
             GetAllSceneItems();
             GetAllFurnituresInScene();
        }

        private void OnAfterLoadSceneEvent()
        {
            itemParentTransform = GameObject.FindWithTag("ItemParentTransform").transform;
            cropParentTransform = GameObject.FindWithTag("CropParent").transform;
            RecreatAllItemsInScene();
            RebuildAllFurnitures();
        }

        private void OnInstantiateItemInScene(int id, Vector3 pos)
        {
            var item = Instantiate(itemBouncePrefab, pos, Quaternion.identity, itemParentTransform);

            //Item里init需要id
            item.itemID = id;
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
;        }

        /// <summary>
        /// 从背包内扔东西
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="mousePos">位置</param>
        /// <param name="itemType">种类</param>
        private void OnDropItemInBag(int itemId, Vector3 mousePos, ItemType itemType) //扔东西时，如果是种子，就不用生成了
        {
            if (ItemType.Seed == itemType) return; //扔种子不需要生成
            var item = Instantiate(itemBouncePrefab, playerTransform.position, Quaternion.identity, itemParentTransform);
            item.itemID = itemId;
            var direction = (mousePos - playerTransform.position ).normalized; //向量化
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, direction);
        }


        private void OnStartNewGame(int obj)
        {
            //将场景内两字典清空
            sceneItemDict.Clear();
            furnitureItemDict.Clear();
        }

        /// <summary>
        /// 获得场景里的所有物体，并加入到字典中
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItem = new List<SceneItem>();
            foreach(var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem();
                //将ID和位置赋值
                sceneItem.itemID = item.itemID;
                sceneItem.itemPosition = new SerializableVector3(item.transform.position);
                currentSceneItem.Add(sceneItem); //添加到新建的列表中
            }
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name)) //如果字典里包含当前场景的名字
            {
                //那么进行更新
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItem;
            }
            else //如果字典不包含当前场景（最开始时，字典为空）
            {
                //就进行添加
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItem);
            }
        }

        /// <summary>
        /// 获取场景内所有建造的物品
        /// </summary>
        private void GetAllFurnituresInScene()
        {
            //临时变量一个List
            List<FurnitureItem> furnitureItemList = new List<FurnitureItem>();
            //将所有带有Furniture脚本的全部找到
            foreach(var item in FindObjectsOfType<Furniture>()) 
            {
                FurnitureItem furnitureItem = new FurnitureItem();
                furnitureItem.itemID = item.itemID;
                furnitureItem.itemPosition = new SerializableVector3(item.transform.position);
                
                if(item.GetComponent<Box>())
                {
                    furnitureItem.boxIndex = item.GetComponent<Box>().index;
                }
                furnitureItemList.Add(furnitureItem);
            }
            if (furnitureItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //如果当前字典包含该场景的列表-那就更新
                furnitureItemDict[SceneManager.GetActiveScene().name] = furnitureItemList;
            }
            else //如果不包含-即第一次读入-那么就新加入
            {
                furnitureItemDict.Add(SceneManager.GetActiveScene().name, furnitureItemList);
            }
        }

        /// <summary>
        /// 重建场景里的所有物品
        /// </summary>
        private void RecreatAllItemsInScene()
        {
            List<SceneItem> currentSceneItem = new List<SceneItem>();
            //如果没有，则返回false；如果有，则将字典里的数据返回给currentSceneItem
            if(sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItem))
            {
                if(currentSceneItem.Count > 0)
                {
                    //将当前场景里的数据全部清除
                    foreach(var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }
                    //清除后加载新的物品
                    foreach(var item in currentSceneItem)
                    {
                        if(item.itemID == 1038)
                        {
                            Item newItem = Instantiate(cropGrass01, item.itemPosition.ToVector3(), Quaternion.identity, cropParentTransform);
                            newItem.Init(item.itemID);
                        }
                        else if(item.itemID == 1039)
                        {
                            Item newItem = Instantiate(cropGrass02, item.itemPosition.ToVector3(), Quaternion.identity, cropParentTransform);
                            newItem.Init(item.itemID);
                        }
                        else if(item.itemID == 1040)
                        {
                            Item newItem = Instantiate(cropGrass03, item.itemPosition.ToVector3(), Quaternion.identity, cropParentTransform);
                            newItem.Init(item.itemID);
                        }
                        else
                        {
                            Item newItem = Instantiate(itemPrefab, item.itemPosition.ToVector3(), Quaternion.identity, itemParentTransform);
                            newItem.Init(item.itemID);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 重建建造物品
        /// </summary>
        private void RebuildAllFurnitures()
        {
            List<FurnitureItem> furnitureItemList = new List<FurnitureItem>();
            if(furnitureItemDict.TryGetValue(SceneManager.GetActiveScene().name, out furnitureItemList))
            {
                if(furnitureItemList.Count > 0) //如果不为空
                {
                    //因为建造的物品不能被销毁
                    //所以无需在重建时将场景内的furniture消除
                    //且同一个坐标不能建造多个物品
                    foreach(var item in furnitureItemList)
                    {
                        //根据我获取的清单重建
                        //OnBuildFurnitureEvent(item.itemID, item.itemPosition.ToVector3());
                        //仔细观察下面的代码，实际和上面的函数所执行的内容基本相同，但我为什么还是调用下面的呢？
                        //因为rebuild的时候无需再给以前建好的箱子新的index
                        //但是我怎么知道以前的index呢？
                        //在ItemManager里GetAllFurnituresInScene()时，也就是场景重建前，将box类的index赋值给
                        //FurnitureItem的boxIndex，这样我在重建时，InitBoxDict时发现相同的key就不会在创建新的key和列表了

                        BluePrintDetails bluePrintDetails = InventoryManager.Instance.bluePrintDataList_SO.GetBluePrintDetails(item.itemID);
                        //mousePosition.z += 10;
                        var buildItem = Instantiate(bluePrintDetails.buildPrefab, item.itemPosition.ToVector3(), Quaternion.identity, itemParentTransform);

                        //每建造一个箱子，都要给箱子一个独特的标识index
                        if (buildItem.GetComponent<Box>())  //如果建造物是箱子
                        { 
                            //更新字典
                            buildItem.GetComponent<Box>().InitBoxDict(item.boxIndex);
                        }
                    }
                }
            }
        }

        public GameSaveData GenerateGameData()
        {
            GetAllSceneItems();
            GetAllFurnituresInScene();
            GameSaveData saveData = new GameSaveData();
            saveData.sceneItemDict = sceneItemDict;
            saveData.furnitureItemDict= furnitureItemDict;
            return saveData;

        }

        public void RestoreGameData(GameSaveData data)
        {
            sceneItemDict=data.sceneItemDict;
            furnitureItemDict= data.furnitureItemDict;

            RecreatAllItemsInScene();
            RebuildAllFurnitures();
        }
    }

}
