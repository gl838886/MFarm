using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData_SO", menuName = "Map/MapData")]

//该文件用来存储当前sceneName的所有tile的数据

public class MapData_SO : ScriptableObject
{
    public string sceneName;  //存储
    [Header("地图信息")]
    public int gridWidth;
    public int gridHeight;

    [Header("原点-左下角")]
    public int xOrigin;
    public int yOrigin;

    public List<TileProperty> tileProperties;
}

