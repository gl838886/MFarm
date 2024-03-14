using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPatternList_SO", menuName ="Light/LightPattern")]
public class LightListPattern_SO : ScriptableObject
{
    public List<LightDetails> lightDetailsList;

    /// <summary>
    /// 通过季节和早晚，返回对应的lightDetails
    /// </summary>
    /// <param name="season">季节</param>
    /// <param name="lightShift">早晚</param>
    /// <returns></returns>
    public LightDetails GetLightDetails(Season season, LightShift lightShift)
    {
        return lightDetailsList.Find(l => l.season == season&& l.lightShift == lightShift);
    }
}

[System.Serializable]
public class LightDetails
{
    public Season season;
    public LightShift lightShift; //早晚变化
    public Color lightColor; //灯光的颜色
    public float lightStrength; //灯光的强度
}
