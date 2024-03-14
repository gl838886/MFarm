using Mfarm.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.cropPlant
{
    //ʵ�ֳ����������Ԥ����
    //��Ϊ�ٳ�������ǰ����ֻ�ǰ������ϵ������ϣ������ڵ���Ƭ������洢�ҵ�ǰ��ֲ����Ϣ
    //�������Ҫ��refreshMap֮ǰ�����ҵ�ֲ����Ϣ������Ƭ
    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;
        public int seedItemID;
        public int growthDay;

        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += OnGenerateCropEvent;
        }

        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= OnGenerateCropEvent;
        }

        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }

        public void OnGenerateCropEvent()
        {
            
            Vector3Int currentCropGridPosition =currentGrid.WorldToCell(transform.position);
            if(seedItemID != 0)
            {
                TileDetails currentTileDetails = GridMapManager.Instance.GetTileDetailsOnCurrentMousePosition(currentCropGridPosition);
                if(currentTileDetails == null)
                {
                    currentTileDetails = new TileDetails();
                }
                currentTileDetails.daysSinceWatered = -1;
                currentTileDetails.seedItemId = seedItemID;
                currentTileDetails.growthDays = growthDay;
                
                GridMapManager.Instance.UpdateTileDetails(currentTileDetails);
            }
        }
    }
}