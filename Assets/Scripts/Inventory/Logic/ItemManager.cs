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

        //�ֵ䣬�����洢���������Ʒ,string�ǳ�������List�б������洢���������Ʒ
        public Dictionary<string,List<SceneItem>> sceneItemDict = new Dictionary<string,List<SceneItem>>();
        //�½�һ���ֵ䣬�����洢������ ���� ����Ʒ������ͬ��
        //string-��������ÿ����������Ӧһ���б�
        public Dictionary<string, List<FurnitureItem>> furnitureItemDict = new Dictionary<string, List<FurnitureItem>>();

        public Transform itemParentTransform; //Instantiate �����һ��������ϣ������һ��parent��
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
            //Debug.Log(mousePosition); z���������
           
            //mousePosition.z += 10;
            var buildItem = Instantiate(bluePrintDetails.buildPrefab, mousePosition, Quaternion.identity, itemParentTransform);

            //ÿ����һ�����ӣ���Ҫ������һ�����صı�ʶindex
            if (buildItem.GetComponent<Box>())  //���������������
            {
                //���Ϊ��������
                buildItem.GetComponent<Box>().index = InventoryManager.Instance.boxAmount;
                //�����ֵ�
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

            //Item��init��Ҫid
            item.itemID = id;
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
;        }

        /// <summary>
        /// �ӱ������Ӷ���
        /// </summary>
        /// <param name="itemId">��ƷID</param>
        /// <param name="mousePos">λ��</param>
        /// <param name="itemType">����</param>
        private void OnDropItemInBag(int itemId, Vector3 mousePos, ItemType itemType) //�Ӷ���ʱ����������ӣ��Ͳ���������
        {
            if (ItemType.Seed == itemType) return; //�����Ӳ���Ҫ����
            var item = Instantiate(itemBouncePrefab, playerTransform.position, Quaternion.identity, itemParentTransform);
            item.itemID = itemId;
            var direction = (mousePos - playerTransform.position ).normalized; //������
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, direction);
        }


        private void OnStartNewGame(int obj)
        {
            //�����������ֵ����
            sceneItemDict.Clear();
            furnitureItemDict.Clear();
        }

        /// <summary>
        /// ��ó�������������壬�����뵽�ֵ���
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItem = new List<SceneItem>();
            foreach(var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem();
                //��ID��λ�ø�ֵ
                sceneItem.itemID = item.itemID;
                sceneItem.itemPosition = new SerializableVector3(item.transform.position);
                currentSceneItem.Add(sceneItem); //��ӵ��½����б���
            }
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name)) //����ֵ��������ǰ����������
            {
                //��ô���и���
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItem;
            }
            else //����ֵ䲻������ǰ�������ʼʱ���ֵ�Ϊ�գ�
            {
                //�ͽ������
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItem);
            }
        }

        /// <summary>
        /// ��ȡ���������н������Ʒ
        /// </summary>
        private void GetAllFurnituresInScene()
        {
            //��ʱ����һ��List
            List<FurnitureItem> furnitureItemList = new List<FurnitureItem>();
            //�����д���Furniture�ű���ȫ���ҵ�
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
                //�����ǰ�ֵ�����ó������б�-�Ǿ͸���
                furnitureItemDict[SceneManager.GetActiveScene().name] = furnitureItemList;
            }
            else //���������-����һ�ζ���-��ô���¼���
            {
                furnitureItemDict.Add(SceneManager.GetActiveScene().name, furnitureItemList);
            }
        }

        /// <summary>
        /// �ؽ��������������Ʒ
        /// </summary>
        private void RecreatAllItemsInScene()
        {
            List<SceneItem> currentSceneItem = new List<SceneItem>();
            //���û�У��򷵻�false������У����ֵ�������ݷ��ظ�currentSceneItem
            if(sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItem))
            {
                if(currentSceneItem.Count > 0)
                {
                    //����ǰ�����������ȫ�����
                    foreach(var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }
                    //���������µ���Ʒ
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
        /// �ؽ�������Ʒ
        /// </summary>
        private void RebuildAllFurnitures()
        {
            List<FurnitureItem> furnitureItemList = new List<FurnitureItem>();
            if(furnitureItemDict.TryGetValue(SceneManager.GetActiveScene().name, out furnitureItemList))
            {
                if(furnitureItemList.Count > 0) //�����Ϊ��
                {
                    //��Ϊ�������Ʒ���ܱ�����
                    //�����������ؽ�ʱ�������ڵ�furniture����
                    //��ͬһ�����겻�ܽ�������Ʒ
                    foreach(var item in furnitureItemList)
                    {
                        //�����һ�ȡ���嵥�ؽ�
                        //OnBuildFurnitureEvent(item.itemID, item.itemPosition.ToVector3());
                        //��ϸ�۲�����Ĵ��룬ʵ�ʺ�����ĺ�����ִ�е����ݻ�����ͬ������Ϊʲô���ǵ���������أ�
                        //��Ϊrebuild��ʱ�������ٸ���ǰ���õ������µ�index
                        //��������ô֪����ǰ��index�أ�
                        //��ItemManager��GetAllFurnituresInScene()ʱ��Ҳ���ǳ����ؽ�ǰ����box���index��ֵ��
                        //FurnitureItem��boxIndex�����������ؽ�ʱ��InitBoxDictʱ������ͬ��key�Ͳ����ڴ����µ�key���б���

                        BluePrintDetails bluePrintDetails = InventoryManager.Instance.bluePrintDataList_SO.GetBluePrintDetails(item.itemID);
                        //mousePosition.z += 10;
                        var buildItem = Instantiate(bluePrintDetails.buildPrefab, item.itemPosition.ToVector3(), Quaternion.identity, itemParentTransform);

                        //ÿ����һ�����ӣ���Ҫ������һ�����صı�ʶindex
                        if (buildItem.GetComponent<Box>())  //���������������
                        { 
                            //�����ֵ�
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
