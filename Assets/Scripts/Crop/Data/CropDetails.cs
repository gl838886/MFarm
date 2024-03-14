using UnityEngine;
[System.Serializable]

public class CropDetails
{
    [Header("���ӵ�ID")]
    public int seedItemID;
    [Header("��ͬ�׶���Ҫ������")]
    public int[] growthDays; //ÿ���׶ε���������
    public int totalGrowthDays;
    [Header("��ͬ�����׶���Ʒ��Prefab")]
    public GameObject[] growthPrefabs;
    [Header("��ͬ�����׶ε�ͼƬ")]
    public Sprite[] growthSprites;
    [Header("����ֲ�ļ���")]
    public Season[] seasons;

    [Space]
    [Header("�ո���Ϣ")]
    public int[] harvestToolItemID; //�����ж�����߿��Խ����ո�
    [Header("ÿ�ֹ���ʹ�ô���")]
    public int[] requireActionCount; //ÿ��������Ҫ���ٴΣ����翳����Ҫ��β�����
    [Header("ת������ƷID")]
    public int transferItemID;

    [Space]
    [Header("�ո��ʵ��Ϣ")]
    public int[] produceItemID;
    public int[] produceMinAmount; //��������
    public int[] produceMaxAmount; //�������
    public Vector2 spawnRadius; //���ɰ뾶
    [Header("�ٴ�����ʱ��")]
    public int daysToRegrow;
    public int regrowTimes;

    [Space]
    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticleEffect;
    public ParticleEffectType particalEffect; //��ЧЧ��
    public SoundName soundEffectName;
    public Vector3 effectPosition; //��Ч����λ�ã����crop��λ�ã�

    /// <summary>
    /// ��鹤���Ƿ����
    /// </summary>
    /// <param name="toolID">����ID</param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach(var tool in harvestToolItemID)
        {
            if (tool == toolID)
                return true;
        }
        return false;
    }

    /// <summary>
    /// ���ص�ǰ������Ҫ�Ĵ���
    /// </summary>
    /// <param name="toolID">����ID</param>
    /// <returns></returns>
    public int GetTotallRequireCount(int toolID)
    {
        for(int i=0;i<harvestToolItemID.Length;i++)
        {
            if (harvestToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }


}
