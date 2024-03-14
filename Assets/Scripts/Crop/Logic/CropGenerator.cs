using Mfarm.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.cropPlant
{
    //实现场景内物体的预生成
    //因为再场景加载前，我只是把物体拖到场景上，场景内的瓦片并不会存储我当前的植物信息
    //因此我需要在refreshMap之前，把我的植物信息传给瓦片
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