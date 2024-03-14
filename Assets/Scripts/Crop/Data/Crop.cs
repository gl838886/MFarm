using System.Collections;
using UnityEngine;


public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    private int actionCount; //记录收割（操作次数）
    public TileDetails currentTileDetails;

    private Animator animator; //大树的动画
    private Transform playerTransform => FindObjectOfType<Player>().transform;

    public bool canChop => currentTileDetails.growthDays >= cropDetails.totalGrowthDays; //大树是否能砍

    /// <summary>
    /// 使用工具
    /// </summary>
    /// <param name="toolDetails">工具</param>
    public void ProcessToolAction(ItemDetails toolDetails, TileDetails tileDetails)
    {
        int requireCount = cropDetails.GetTotallRequireCount(toolDetails.itemID);
        animator=GetComponentInChildren<Animator>(); //在子物体身上

        if (requireCount == -1) return;

        if(actionCount < requireCount)  
        {
            actionCount++;
            if(animator !=null && cropDetails.hasAnimation)
            {
                if(playerTransform.position.x>transform.position.x)
                {
                    animator.SetTrigger("rotateLeft");
                }
                else
                {
                    animator.SetTrigger("rotateRight");             
                }       
            }
            if (cropDetails.hasParticleEffect) //是否有粒子效果
            {
                EventHandler.CallParticleEffectEvent(cropDetails.particalEffect, transform.position + cropDetails.effectPosition);
            }
            if (cropDetails.soundEffectName != SoundName.none)
            {
                var audioDetails = AudioManager.Instance.audioDataList.GetAudioDetails(cropDetails.soundEffectName);
                EventHandler.CallInitSound(audioDetails);
            }
        }

        if(actionCount >=requireCount )
        {
            if(cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation) //在人物的头顶处生成
            {
                SpawnCropItems();
            }
            else //砍树&砸石头
            {
                if (playerTransform.position.x > transform.position.x)
                {
                    animator.SetTrigger("fallLeft");
                }
                else
                {
                    animator.SetTrigger("fallRight");
                }
                StartCoroutine(HarvestAfterAnimation());
            }
            if (cropDetails.transferItemID != 0)
                GenerateTransferItem();
        }
    }

    private IEnumerator HarvestAfterAnimation()
    {
        while(animator.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            yield return null;
        }
        SpawnCropItems();
        
    }

    /// <summary>
    /// 生成（树桩）
    /// </summary>
    private void GenerateTransferItem()
    {
        currentTileDetails.seedItemId = cropDetails.transferItemID;
        currentTileDetails.growthDays = 0;
        currentTileDetails.daysSinceLastHarvest = -1;

        EventHandler.CallRefreshMapEvent();
    }

    /// <summary>
    /// 生成随机数量的作物
    /// </summary>
    private void SpawnCropItems()
    {
        for(int i=0;i<cropDetails.produceItemID.Length;i++)
        {
            int spawnAmount = 0; //生成数量
            if (cropDetails.produceMaxAmount[i] == cropDetails.produceMinAmount[i])
                spawnAmount = cropDetails.produceMaxAmount[i]; //生成指定数量
            else
                spawnAmount = Random.Range(cropDetails.produceMinAmount[i], cropDetails.produceMaxAmount[i] + 1);

            for(int j=0;j<spawnAmount;j++)
            {
               if(cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestCropEvent(cropDetails.produceItemID[i]);
                }
                else //在世界上生成
                {
                    //方向
                    var dirX = transform.position.x > playerTransform.position.x ? 1 : -1;
                    //生成范围
                    var spawnPosition = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                        transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);
                    EventHandler.CallInstantiateItemInScene(cropDetails.produceItemID[i], spawnPosition);
                }
            }
        }

        //是否可以重复生长
        if(currentTileDetails != null)
        {
            currentTileDetails.daysSinceLastHarvest++; //收割了几次

            //可以重复生长
            if (cropDetails.daysToRegrow > 0 && currentTileDetails.daysSinceLastHarvest < cropDetails.regrowTimes)
            {
                currentTileDetails.growthDays = cropDetails.totalGrowthDays - cropDetails.daysToRegrow;
                EventHandler.CallRefreshMapEvent();
            }
            //不可以重复生长
            else
            {
                currentTileDetails.seedItemId = -1;
                currentTileDetails.growthDays = -1;
                currentTileDetails.daysSinceLastHarvest = -1;
            }

            Destroy(gameObject);
        }
    }
}



