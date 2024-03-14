using JetBrains.Annotations;
using UnityEngine;

namespace Mfarm.cropPlant
{
    public class CropManager : Singleton<CropManager>
    {
        public CropDataList_SO cropDataList_SO;
        public Transform cropParent; //所有生成的作物都在此父物体下面
        public Grid currentGrid;

        private Season currentSeason;

        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
            EventHandler.UpdateGameDate += OnUpdateGameDate;
        }

        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
            EventHandler.UpdateGameDate -= OnUpdateGameDate;
        }

        private void OnUpdateGameDate(int arg1, Season season)
        {
            currentSeason = season; //获取当前季节
        }
        private void OnAfterLoadSceneEvent()
        {
            cropParent = GameObject.FindGameObjectWithTag("CropParent").transform;
            currentGrid = FindObjectOfType<Grid>();
        }

        /// <summary>
        /// 种植作物
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="tileDetails"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnPlantSeedEvent(int ID, TileDetails tileDetails)
        {
            CropDetails currentCropDetails = GetCropDetails(ID);
            //Debug.Log(currentCropDetails.seedItemID);
            if(currentCropDetails != null && SeasonAvailale(currentCropDetails) && tileDetails.seedItemId == -1)
            {
                tileDetails.seedItemId = currentCropDetails.seedItemID;
                tileDetails.growthDays = 0;
                //显示农作物
                DisPlayCropPlant(tileDetails, currentCropDetails);
            }
            else //已经种过
            {
                //显示农作物
                DisPlayCropPlant(tileDetails, currentCropDetails);
            }
        }

        /// <summary>
        /// 显示农作物
        /// </summary>
        /// <param name="tileDetails">地图信息</param>
        /// <param name="cropDetails">农作物信息</param>
        private void DisPlayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            int cropStages = cropDetails.growthDays.Length; //一共多少个生长阶段
            int currentStage = 0; //当前阶段
            int cropDaysCounter = cropDetails.totalGrowthDays; //总的生长时间

            //判断当前处于哪个生长阶段（匹配相应的图片）
            for (int i=cropStages - 1; i>=0;i--) 
            {
                if (tileDetails.growthDays >= cropDaysCounter) 
                {
                    currentStage = i;
                    break;
                }
                cropDaysCounter -= cropDetails.growthDays[currentStage];
            }

            //显示农作物
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];
            Vector3 cropPosition = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);
            GameObject cropInstance = Instantiate(cropPrefab, cropPosition, Quaternion.identity, cropParent);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;
            cropInstance.GetComponent<Crop>().cropDetails= cropDetails;
            cropInstance.GetComponent<Crop>().currentTileDetails = tileDetails;  //拿到该作物所处土地的土地信息
        }

        /// <summary>
        /// 返回农作物详细信息
        /// </summary>
        /// <param name="ID">种子ID</param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int ID)
        {
            return cropDataList_SO.cropDetailsList.Find(c =>c.seedItemID== ID);
        }

        /// <summary>
        /// 判断当前季节是否可以种植
        /// </summary>
        /// <param name="cropDetails">种子信息</param>
        /// <returns></returns>
        private bool SeasonAvailale(CropDetails cropDetails)
        {
            for(int i=0;i<cropDetails.seasons.Length;i++)
            {
                if (currentSeason == cropDetails.seasons[i])
                    return true;
            }
            return false;
        }
    }
}
