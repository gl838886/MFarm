using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPatternList_SO", menuName ="Light/LightPattern")]
public class LightListPattern_SO : ScriptableObject
{
    public List<LightDetails> lightDetailsList;

    /// <summary>
    /// ͨ�����ں��������ض�Ӧ��lightDetails
    /// </summary>
    /// <param name="season">����</param>
    /// <param name="lightShift">����</param>
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
    public LightShift lightShift; //����仯
    public Color lightColor; //�ƹ����ɫ
    public float lightStrength; //�ƹ��ǿ��
}
