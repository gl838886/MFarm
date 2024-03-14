using System.Collections;
using UnityEngine;


public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    private int actionCount; //��¼�ո����������
    public TileDetails currentTileDetails;

    private Animator animator; //�����Ķ���
    private Transform playerTransform => FindObjectOfType<Player>().transform;

    public bool canChop => currentTileDetails.growthDays >= cropDetails.totalGrowthDays; //�����Ƿ��ܿ�

    /// <summary>
    /// ʹ�ù���
    /// </summary>
    /// <param name="toolDetails">����</param>
    public void ProcessToolAction(ItemDetails toolDetails, TileDetails tileDetails)
    {
        int requireCount = cropDetails.GetTotallRequireCount(toolDetails.itemID);
        animator=GetComponentInChildren<Animator>(); //������������

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
            if (cropDetails.hasParticleEffect) //�Ƿ�������Ч��
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
            if(cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation) //�������ͷ��������
            {
                SpawnCropItems();
            }
            else //����&��ʯͷ
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
    /// ���ɣ���׮��
    /// </summary>
    private void GenerateTransferItem()
    {
        currentTileDetails.seedItemId = cropDetails.transferItemID;
        currentTileDetails.growthDays = 0;
        currentTileDetails.daysSinceLastHarvest = -1;

        EventHandler.CallRefreshMapEvent();
    }

    /// <summary>
    /// �����������������
    /// </summary>
    private void SpawnCropItems()
    {
        for(int i=0;i<cropDetails.produceItemID.Length;i++)
        {
            int spawnAmount = 0; //��������
            if (cropDetails.produceMaxAmount[i] == cropDetails.produceMinAmount[i])
                spawnAmount = cropDetails.produceMaxAmount[i]; //����ָ������
            else
                spawnAmount = Random.Range(cropDetails.produceMinAmount[i], cropDetails.produceMaxAmount[i] + 1);

            for(int j=0;j<spawnAmount;j++)
            {
               if(cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestCropEvent(cropDetails.produceItemID[i]);
                }
                else //������������
                {
                    //����
                    var dirX = transform.position.x > playerTransform.position.x ? 1 : -1;
                    //���ɷ�Χ
                    var spawnPosition = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                        transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);
                    EventHandler.CallInstantiateItemInScene(cropDetails.produceItemID[i], spawnPosition);
                }
            }
        }

        //�Ƿ�����ظ�����
        if(currentTileDetails != null)
        {
            currentTileDetails.daysSinceLastHarvest++; //�ո��˼���

            //�����ظ�����
            if (cropDetails.daysToRegrow > 0 && currentTileDetails.daysSinceLastHarvest < cropDetails.regrowTimes)
            {
                currentTileDetails.growthDays = cropDetails.totalGrowthDays - cropDetails.daysToRegrow;
                EventHandler.CallRefreshMapEvent();
            }
            //�������ظ�����
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



