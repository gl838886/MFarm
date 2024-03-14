using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.cropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        private Transform playerTransform =>FindObjectOfType<Player>().transform;

        /// <summary>
        /// ���ũ����(������Ϣ)
        /// </summary>
        /// <param name="ID">��ƷID</param>
        public void InitCropDetails(int ID)
        {
            cropDetails = CropManager.Instance.GetCropDetails(ID);
        }

        /// <summary>
        /// �����������������
        /// </summary>
        public void SpawnCropItems()
        {
            for (int i = 0; i < cropDetails.produceItemID.Length; i++)
            {
                int spawnAmount = 0; //��������
                if (cropDetails.produceMaxAmount[i] == cropDetails.produceMinAmount[i])
                    spawnAmount = cropDetails.produceMaxAmount[i]; //����ָ������
                else
                    spawnAmount = Random.Range(cropDetails.produceMinAmount[i], cropDetails.produceMaxAmount[i] + 1);

                for (int j = 0; j < spawnAmount; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)
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
        }
    }
}


