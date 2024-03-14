using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //为了让unity在inspector窗口里识别
 public class ItemDetails
{
    public int itemID;
    public string itemName;
    public ItemType itemType; //创建的枚举类型

    public Sprite itemIcon; //图标
    public Sprite itemOnWorldSprite; //地图上的图标

    public string itemDescription; 
    public int itemUseRadius;
    public bool canPickUp;
    public bool canDrop;
    public bool canCarry;

    public int itemPrice;

    [Range(0, 1)] public float sellPercentage;
}


[System.Serializable]
public struct BagItem
{
    public int itemID;
    public int itemCount;
}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController animColl;
}

[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;
    public SerializableVector3(Vector3 pos) //传入坐标将其序列化
    {
        x=pos.x; y=pos.y; z=pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x,y,z);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

/// <summary>
/// 场景物品
/// </summary>
[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 itemPosition;
}

/// <summary>
/// 场景内建造物品
/// </summary>
[System.Serializable]
public class FurnitureItem
{
    public int itemID;  
    public SerializableVector3 itemPosition;
    public int boxIndex;
}

[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate; //每一块瓦片的坐标
    public GridType gridType; //每个瓦片的种类，可种植
    public bool boolType;
}

[System.Serializable]
public class TileDetails
{
    public int gridX, gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNpcObstacle;
    public int daysSinceDug = -1; //这块地挖了几天
    public int daysSinceWatered = -1;
    public int seedItemId = -1;
    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;
}

[System.Serializable]
public class NPCPosition
{
    public Transform npcTransform;
    public string startScene;
    public Vector3 startPosition;
}

//跨场景路径
[System.Serializable]
public class SceneRoute
{
    public string fromScene;
    public string goToScene;
    public List<ScenePath> pathList; //连接两个场景，有一去一回两个
}

//场景路径
[System.Serializable]
public class ScenePath
{
    public string sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int goToGridCell;
}

