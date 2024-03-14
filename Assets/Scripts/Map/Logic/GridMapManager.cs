using Mfarm.cropPlant;
using Mfarm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Mfarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>, ISaveable
    {
        [Header("��ͼ��Ϣ")]
        public List<MapData_SO> mapDataList; //�洢ÿ��������so�ļ�

        [Header("�ɸ��ĵ���Ƭ��ͼ��Ϣ")]
        public RuleTile digTile;
        public RuleTile waterTile;
        private Tilemap digTileMap;
        private Tilemap waterTileMap;

        //�洢���е���Ƭ��Ϣ-������+��Ƭ��Ϣ
        private Dictionary<string, TileDetails> tileDetailsDictionary= new Dictionary<string, TileDetails>();

        //�洢��������Ʒ�Ƿ��һ�μ���
        private Dictionary<string, bool> firstGenerateDict= new Dictionary<string, bool>();

        private Grid currentGrid;
        private Season currentSeason;

        //�Ӳ��б�
        private List<ReapItem> reapItemList;

        public string GUID => GetComponent<DataGUID>().GUID;

        private void OnEnable()
        {
            EventHandler.ExecuteActionAftreAnimation += OnExecuteActionAftreAnimation;
            EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
            EventHandler.UpdateGameDate += OnUpdateGameDateEvent;
            EventHandler.RefreshMapEvent += RefreshMap;
        }

        private void OnDisable()
        {
            EventHandler.ExecuteActionAftreAnimation -= OnExecuteActionAftreAnimation;
            EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
            EventHandler.UpdateGameDate -= OnUpdateGameDateEvent;
            EventHandler.RefreshMapEvent -= RefreshMap;
        }

        void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
            foreach (var mapData in mapDataList)
            {
                //Debug.Log(mapData.sceneName);
                firstGenerateDict.Add(mapData.sceneName, true);
                InitTileDetailsDictionary(mapData);
            }
        }

        /// <summary>
        /// �����л����ҵ���ǰ������
        /// </summary>
        private void OnAfterLoadSceneEvent()
        {
            currentGrid=FindObjectOfType<Grid>();
            digTileMap = GameObject.FindGameObjectWithTag("Dig").GetComponent<Tilemap>();
            waterTileMap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();

            if (firstGenerateDict[SceneManager.GetActiveScene().name])
            {
                EventHandler.CallGenerateCropEvent();
                firstGenerateDict[SceneManager.GetActiveScene().name] = false;
            }
            RefreshMap();
        }

        /// <summary>
        /// ÿ�����һ��
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnUpdateGameDateEvent(int day, Season season)
        {
            currentSeason= season;
            foreach(var tile in tileDetailsDictionary)
            {
                if(tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug++;
                }
                if(tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered = -1;
                }
                //�������5��
                if(tile.Value.daysSinceDug > 5 && tile.Value.seedItemId == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig= true;
                    tile.Value.growthDays = -1;
                }
                if(tile.Value.seedItemId != -1)
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }


        /// <summary>
        /// �����е�so�ļ��е���Ƭ��Ϣȫ��¼�뵽�ֵ���(һ��������so��������ѭ����
        /// </summary>
        /// <param name="mapData"></param>
        void InitTileDetailsDictionary(MapData_SO mapData)
        {
            foreach(var tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y,
                };
                //ÿ����Ƭ������keyΪx+y+��ǰ������
                string k = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;

                if(GetTileDetails(k)!=null) //�����ڿգ��ͰѾɵ��Ѷ��ȸ�ֵ��
                {
                    tileDetails= GetTileDetails(k);
                }
                //�ٿ������ػ��ܸ�ɶ
                switch (tileProperty.gridType) //��Ϊһ����Ƭ��ͼ���ܾ��ж��ֹ���
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolType;
                        break;
                    case GridType.CanDropItem:
                        tileDetails.canDropItem = tileProperty.boolType;
                        break;
                    case GridType.CanPlaceFurniture:
                        tileDetails.canPlaceFurniture= tileProperty.boolType;
                        break;
                    case GridType.NPC_Obstacles:
                        tileDetails.isNpcObstacle = tileProperty.boolType;
                        break;
                }
                //���ֵ
                if(GetTileDetails(k)!=null)
                {
                    tileDetailsDictionary[k] = tileDetails;
                }
                else
                    tileDetailsDictionary.Add(k, tileDetails);

            }
        }
        /// <summary>
        /// �����ֵ䣬����tileDetails
        /// </summary>
        /// <param name="key">tileDetails��Ӧ��key</param>
        /// <returns></returns>
        public TileDetails GetTileDetails(string key)
        {
            if(tileDetailsDictionary.ContainsKey(key))
            {
                //Debug.Log(tileDetailsDictionary[key].ToString());
                return tileDetailsDictionary[key];
            }
            else return null;
        }

        /// <summary>
        /// ������굱ǰλ�õ���Ƭ��Ϣ
        /// </summary>
        /// <param name="currentMousePosition">��굱ǰ������λ��</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnCurrentMousePosition(Vector3Int currentMousePosition)
        {
            string key = currentMousePosition.x + "x" + currentMousePosition.y + "y" + SceneManager.GetActiveScene().name;
            
            return GetTileDetails(key);
        }

        /// <summary>
        /// ����ִ�����Ĳ���
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="itemDetails"></param>
        private void OnExecuteActionAftreAnimation(Vector3 mousePosition, ItemDetails itemDetails)
        {
            var mouseGridPosition = currentGrid.WorldToCell(mousePosition);
            //��ȡ��ǰ�����λ�õ���Ƭ��ͼ��Ϣ
            TileDetails currentTileDetails = GetTileDetailsOnCurrentMousePosition(mouseGridPosition);
            if(currentTileDetails != null)
            {
                Crop currentCrop = GetCrop(mousePosition);
                switch (itemDetails.itemType)
                {
                    //WORLKFLOW:����
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID, currentTileDetails);
                        EventHandler.CallDropItemInBag(itemDetails.itemID, mouseGridPosition, itemDetails.itemType);
                        EventHandler.CallPlaySoundEvent(SoundName.Plant);
                        break;
                    case ItemType.Commodity:
                        EventHandler.CallDropItemInBag(itemDetails.itemID,mouseGridPosition, itemDetails.itemType);
                        break;
                    case ItemType.HoeTool: 
                        SetDigTile(currentTileDetails);
                        currentTileDetails.canDig = false;
                        currentTileDetails.canDropItem = false;
                        currentTileDetails.daysSinceDug = 0;
                        EventHandler.CallPlaySoundEvent(SoundName.Hoe);
                        //��Ч
                        break;
                    case ItemType.WaterTool:
                        SetWaterTile(currentTileDetails);
                        currentTileDetails.daysSinceWatered = 0;
                        EventHandler.CallPlaySoundEvent(SoundName.Water);
                        //��Ч
                        break;
                    case ItemType.ChopTool:
                        if(currentCrop!=null)
                        {
                            currentCrop.ProcessToolAction(itemDetails, currentCrop.currentTileDetails);
                            //EventHandler.CallPlaySoundEvent(SoundName.Chop);
                        }
                        break;
                    case ItemType.CollectTool:
                        currentCrop.ProcessToolAction(itemDetails, currentTileDetails);
                        EventHandler.CallPlaySoundEvent(SoundName.Reap);
                        break;
                    case ItemType.BreakTool:
                        if (currentCrop != null)
                        {
                            currentCrop.ProcessToolAction(itemDetails, currentCrop.currentTileDetails);
                            EventHandler.CallPlaySoundEvent(SoundName.Pickaxe);
                        }
                            break;
                    case ItemType.ReapTool:
                        int reapCount = 0;
                        for(int i=0;i<reapItemList.Count;i++)
                        {
                            EventHandler.CallParticleEffectEvent(ParticleEffectType.GrassParticle, reapItemList[i].transform.position + Vector3.up);
                            reapItemList[i].SpawnCropItems();
                            Destroy(reapItemList[i].gameObject);
                            reapCount++;
                            if(reapCount>=Settings.maxReapCount)
                                break;
                        }
                        break;
                    case ItemType.Furniture:
                        //��ItemManager�н��н���
                        //��InventoryManager��ɾ����Ӧ����Դ
                        //Debug.Log("hello");
                        EventHandler.CallBuildFurnitureEvent(itemDetails.itemID, mousePosition);
                        break;
                    default: break;
                 
                }
                UpdateTileDetails(currentTileDetails);
            }
        }

        /// <summary>
        /// �������ɵ�Cropbase�ϵ�crop
        /// </summary>
        /// <param name="mouseWorldPosition">���λ��</param>
        /// <returns></returns>
        public Crop GetCrop(Vector3 mouseWorldPosition)
        {
            //������괦��������ײ��-��������-�ɲ��ĵ�
            Collider2D[] cropColl = Physics2D.OverlapPointAll(mouseWorldPosition);
            Crop currentCrop = null;
            for(int i=0;i<cropColl.Length;i++)
            {
                if (cropColl[i].GetComponent<Crop>())
                {
                    currentCrop = cropColl[i].GetComponent<Crop>();
                }
            }
            return currentCrop;
        }

        public bool GetReapableItemInRadius(Vector3 mouseWorldPosition,ItemDetails itemDetails)
        {
            reapItemList = new List<ReapItem>();
            Collider2D[] colls =new Collider2D[20];
            Physics2D.OverlapCircleNonAlloc(mouseWorldPosition, itemDetails.itemUseRadius, colls);
            if(colls.Length > 0 )
            {
                for(int i= 0;i<colls.Length;i++)
                {
                    if (colls[i] != null)
                    {
                        if (colls[i].GetComponent<ReapItem>() != null)
                        {
                            ReapItem reapItem = colls[i].GetComponent<ReapItem>();
                            reapItemList.Add(reapItem);
                        }
                    }
                }
            }
            return reapItemList.Count > 0;
        }

        /// <summary>
        /// �����ڿ���Ƭ
        /// </summary>
        /// <param name="tileDetails">��Ƭ��Ϣ</param>
        private void SetDigTile(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.gridX, tileDetails.gridY, 0);
            digTileMap.SetTile(pos, digTile); //��ȥ�ĵ��鿴setTile
        }

        /// <summary>
        /// ���ý�ˮ��Ƭ
        /// </summary>
        /// <param name="tileDetails">��Ƭ��Ϣ</param>
        private void SetWaterTile(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.gridX, tileDetails.gridY, 0);
            waterTileMap.SetTile(pos, waterTile);
        }

        /// <summary>
        /// �����ֵ�����Ƭ����
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if(tileDetailsDictionary.ContainsKey(key))
            {
                tileDetailsDictionary[key] = tileDetails;
            }
            else //���û�У��������
            {
                tileDetailsDictionary.Add(key, tileDetails);
            }
        }

        /// <summary>
        /// ��ʾ��ͼ��Ƭ
        /// </summary>
        /// <param name="sceneName">��������</param>
        private void DisplayMap(string sceneName)
        {
            foreach(var tile in tileDetailsDictionary)
            {
                string key = tile.Key;
                TileDetails tileDetails = tile.Value;
                if(key.Contains(sceneName))
                {
                    if(tileDetails.daysSinceDug>-1)
                        SetDigTile(tileDetails);
                    if (tileDetails.daysSinceWatered > -1)
                        SetWaterTile(tileDetails);
                    if(tileDetails.seedItemId>-1)
                    {
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemId, tileDetails);
                    }
                }
            }
        }

        //���¼��س�������ɾ����ǰ������������Ƭ���ٰ���֮ǰ�洢���µ����¼���
        private void RefreshMap()
        {
            //�����
            if(digTileMap!=null)
            {
                digTileMap.ClearAllTiles();
            }
            if(waterTileMap!=null)
            {
                waterTileMap.ClearAllTiles();
            }
            foreach(var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }
            //����ʾ
            DisplayMap(SceneManager.GetActiveScene().name); 
        }

        /// <summary>
        /// ͨ�����������ص�ǰ�����ķ�Χ��ԭ��
        /// </summary>
        /// <param name="sceneName">������</param>
        /// <param name="gridDimensions">������Χ</param>
        /// <param name="gridOrginNode">����ԭ��(���½�)</param>
        /// <returns></returns>
        public bool GetGridDimensions(string sceneName, out Vector2Int gridDimensions, out Vector2Int gridOrginNode)
        {
            gridDimensions = Vector2Int.zero;
            gridOrginNode= Vector2Int.zero;

            foreach(var mapData in mapDataList)
            {
                if(mapData.sceneName==sceneName)
                {
                    gridDimensions.x = mapData.gridWidth;
                    gridDimensions.y = mapData.gridHeight;
                    gridOrginNode.x = mapData.xOrigin;
                    gridOrginNode.y = mapData.yOrigin;

                    return true;
                }
            }

            return false; //û���ҵ�
        }

        public GameSaveData GenerateGameData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.tileDetailsDict = tileDetailsDictionary;
            saveData.firstGenerateDict = firstGenerateDict;
            return saveData;
        }

        public void RestoreGameData(GameSaveData data)
        {
            tileDetailsDictionary = data.tileDetailsDict;
            firstGenerateDict = data.firstGenerateDict;
            RefreshMap();
        }
    }
}

