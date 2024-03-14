using UnityEngine;
[System.Serializable]

public class CropDetails
{
    [Header("种子的ID")]
    public int seedItemID;
    [Header("不同阶段需要的天数")]
    public int[] growthDays; //每个阶段的生长天数
    public int totalGrowthDays;
    [Header("不同生长阶段物品的Prefab")]
    public GameObject[] growthPrefabs;
    [Header("不同生长阶段的图片")]
    public Sprite[] growthSprites;
    [Header("可种植的季节")]
    public Season[] seasons;

    [Space]
    [Header("收割信息")]
    public int[] harvestToolItemID; //可能有多个工具可以进行收割
    [Header("每种工具使用次数")]
    public int[] requireActionCount; //每个工具需要多少次（例如砍树需要多次操作）
    [Header("转换新物品ID")]
    public int transferItemID;

    [Space]
    [Header("收割果实信息")]
    public int[] produceItemID;
    public int[] produceMinAmount; //最少数量
    public int[] produceMaxAmount; //最多数量
    public Vector2 spawnRadius; //生成半径
    [Header("再次生长时间")]
    public int daysToRegrow;
    public int regrowTimes;

    [Space]
    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticleEffect;
    public ParticleEffectType particalEffect; //特效效果
    public SoundName soundEffectName;
    public Vector3 effectPosition; //特效生成位置（相对crop的位置）

    /// <summary>
    /// 检查工具是否可用
    /// </summary>
    /// <param name="toolID">工具ID</param>
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
    /// 返回当前工具需要的次数
    /// </summary>
    /// <param name="toolID">工具ID</param>
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
