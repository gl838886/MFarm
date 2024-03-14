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
        [Header("地图信息")]
        public List<MapData_SO> mapDataList; //存储每个场景的so文件

        [Header("可更改的瓦片地图信息")]
        public RuleTile digTile;
        public RuleTile waterTile;
        private Tilemap digTileMap;
        private Tilemap waterTileMap;

        //存储所有的瓦片信息-场景名+瓦片信息
        private Dictionary<string, TileDetails> tileDetailsDictionary= new Dictionary<string, TileDetails>();

        //存储场景内物品是否第一次加载
        private Dictionary<string, bool> firstGenerateDict= new Dictionary<string, bool>();

        private Grid currentGrid;
        private Season currentSeason;

        //杂草列表
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
        /// 场景切换后找到当前的网格
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
        /// 每天更新一次
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
                //如果超过5天
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
        /// 将所有的so文件中的瓦片信息全部录入到字典中(一个场景的so，后面再循环）
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
                //每个瓦片的名字key为x+y+当前场景名
                string k = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;

                if(GetTileDetails(k)!=null) //不等于空，就把旧的已读先赋值上
                {
                    tileDetails= GetTileDetails(k);
                }
                //再看看这块地还能干啥
                switch (tileProperty.gridType) //因为一个瓦片地图可能具有多种功能
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
                //最后赋值
                if(GetTileDetails(k)!=null)
                {
                    tileDetailsDictionary[k] = tileDetails;
                }
                else
                    tileDetailsDictionary.Add(k, tileDetails);

            }
        }
        /// <summary>
        /// 查找字典，返回tileDetails
        /// </summary>
        /// <param name="key">tileDetails对应的key</param>
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
        /// 返回鼠标当前位置的瓦片信息
        /// </summary>
        /// <param name="currentMousePosition">鼠标当前的网格位置</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnCurrentMousePosition(Vector3Int currentMousePosition)
        {
            string key = currentMousePosition.x + "x" + currentMousePosition.y + "y" + SceneManager.GetActiveScene().name;
            
            return GetTileDetails(key);
        }

        /// <summary>
        /// 动画执行完后的操作
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="itemDetails"></param>
        private void OnExecuteActionAftreAnimation(Vector3 mousePosition, ItemDetails itemDetails)
        {
            var mouseGridPosition = currentGrid.WorldToCell(mousePosition);
            //获取当前鼠标点击位置的瓦片地图信息
            TileDetails currentTileDetails = GetTileDetailsOnCurrentMousePosition(mouseGridPosition);
            if(currentTileDetails != null)
            {
                Crop currentCrop = GetCrop(mousePosition);
                switch (itemDetails.itemType)
                {
                    //WORLKFLOW:代办
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
                        //音效
                        break;
                    case ItemType.WaterTool:
                        SetWaterTile(currentTileDetails);
                        currentTileDetails.daysSinceWatered = 0;
                        EventHandler.CallPlaySoundEvent(SoundName.Water);
                        //音效
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
                        //在ItemManager中进行建造
                        //在InventoryManager中删除对应的资源
                        //Debug.Log("hello");
                        EventHandler.CallBuildFurnitureEvent(itemDetails.itemID, mousePosition);
                        break;
                    default: break;
                 
                }
                UpdateTileDetails(currentTileDetails);
            }
        }

        /// <summary>
        /// 返回生成的Cropbase上的crop
        /// </summary>
        /// <param name="mouseWorldPosition">鼠标位置</param>
        /// <returns></returns>
        public Crop GetCrop(Vector3 mouseWorldPosition)
        {
            //返回鼠标处的所有碰撞体-返回数组-可查文档
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
        /// 设置挖坑瓦片
        /// </summary>
        /// <param name="tileDetails">瓦片信息</param>
        private void SetDigTile(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.gridX, tileDetails.gridY, 0);
            digTileMap.SetTile(pos, digTile); //可去文档查看setTile
        }

        /// <summary>
        /// 设置浇水瓦片
        /// </summary>
        /// <param name="tileDetails">瓦片信息</param>
        private void SetWaterTile(TileDetails tileDetails)
        {
            Vector3Int pos = new Vector3Int(tileDetails.gridX, tileDetails.gridY, 0);
            waterTileMap.SetTile(pos, waterTile);
        }

        /// <summary>
        /// 更新字典内瓦片数据
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if(tileDetailsDictionary.ContainsKey(key))
            {
                tileDetailsDictionary[key] = tileDetails;
            }
            else //如果没有，则新添加
            {
                tileDetailsDictionary.Add(key, tileDetails);
            }
        }

        /// <summary>
        /// 显示地图瓦片
        /// </summary>
        /// <param name="sceneName">场景名字</param>
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

        //重新加载场景后，先删除当前场景的所有瓦片，再按照之前存储更新的重新加载
        private void RefreshMap()
        {
            //先清除
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
            //再显示
            DisplayMap(SceneManager.GetActiveScene().name); 
        }

        /// <summary>
        /// 通过场景名返回当前场景的范围和原点
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="gridDimensions">场景范围</param>
        /// <param name="gridOrginNode">场景原点(左下角)</param>
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

            return false; //没有找到
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

