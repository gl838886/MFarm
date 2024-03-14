using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //Ϊ����unity��inspector������ʶ��
 public class ItemDetails
{
    public int itemID;
    public string itemName;
    public ItemType itemType; //������ö������

    public Sprite itemIcon; //ͼ��
    public Sprite itemOnWorldSprite; //��ͼ�ϵ�ͼ��

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
    public SerializableVector3(Vector3 pos) //�������꽫�����л�
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
/// ������Ʒ
/// </summary>
[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 itemPosition;
}

/// <summary>
/// �����ڽ�����Ʒ
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
    public Vector2Int tileCoordinate; //ÿһ����Ƭ������
    public GridType gridType; //ÿ����Ƭ�����࣬����ֲ
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
    public int daysSinceDug = -1; //�������˼���
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

//�糡��·��
[System.Serializable]
public class SceneRoute
{
    public string fromScene;
    public string goToScene;
    public List<ScenePath> pathList; //����������������һȥһ������
}

//����·��
[System.Serializable]
public class ScenePath
{
    public string sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int goToGridCell;
}

