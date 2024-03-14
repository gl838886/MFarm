using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO", menuName = "Map/MapData")]

//���ļ������洢��ǰsceneName������tile������

public class MapData_SO : ScriptableObject
{
    public string sceneName;  //�洢
    [Header("��ͼ��Ϣ")]
    public int gridWidth;
    public int gridHeight;

    [Header("ԭ��-���½�")]
    public int xOrigin;
    public int yOrigin;

    public List<TileProperty> tileProperties;
}

