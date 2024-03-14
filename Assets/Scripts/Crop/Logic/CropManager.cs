using JetBrains.Annotations;
using UnityEngine;

namespace Mfarm.cropPlant
{
    public class CropManager : Singleton<CropManager>
    {
        public CropDataList_SO cropDataList_SO;
        public Transform cropParent; //�������ɵ����ﶼ�ڴ˸���������
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
            currentSeason = season; //��ȡ��ǰ����
        }
        private void OnAfterLoadSceneEvent()
        {
            cropParent = GameObject.FindGameObjectWithTag("CropParent").transform;
            currentGrid = FindObjectOfType<Grid>();
        }

        /// <summary>
        /// ��ֲ����
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
                //��ʾũ����
                DisPlayCropPlant(tileDetails, currentCropDetails);
            }
            else //�Ѿ��ֹ�
            {
                //��ʾũ����
                DisPlayCropPlant(tileDetails, currentCropDetails);
            }
        }

        /// <summary>
        /// ��ʾũ����
        /// </summary>
        /// <param name="tileDetails">��ͼ��Ϣ</param>
        /// <param name="cropDetails">ũ������Ϣ</param>
        private void DisPlayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            int cropStages = cropDetails.growthDays.Length; //һ�����ٸ������׶�
            int currentStage = 0; //��ǰ�׶�
            int cropDaysCounter = cropDetails.totalGrowthDays; //�ܵ�����ʱ��

            //�жϵ�ǰ�����ĸ������׶Σ�ƥ����Ӧ��ͼƬ��
            for (int i=cropStages - 1; i>=0;i--) 
            {
                if (tileDetails.growthDays >= cropDaysCounter) 
                {
                    currentStage = i;
                    break;
                }
                cropDaysCounter -= cropDetails.growthDays[currentStage];
            }

            //��ʾũ����
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];
            Vector3 cropPosition = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);
            GameObject cropInstance = Instantiate(cropPrefab, cropPosition, Quaternion.identity, cropParent);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;
            cropInstance.GetComponent<Crop>().cropDetails= cropDetails;
            cropInstance.GetComponent<Crop>().currentTileDetails = tileDetails;  //�õ��������������ص�������Ϣ
        }

        /// <summary>
        /// ����ũ������ϸ��Ϣ
        /// </summary>
        /// <param name="ID">����ID</param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int ID)
        {
            return cropDataList_SO.cropDetailsList.Find(c =>c.seedItemID== ID);
        }

        /// <summary>
        /// �жϵ�ǰ�����Ƿ������ֲ
        /// </summary>
        /// <param name="cropDetails">������Ϣ</param>
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
