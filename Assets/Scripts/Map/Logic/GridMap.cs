using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]  //编辑条件下执行
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    public Tilemap currentTilemap;

    private void OnEnable()
    {
        if(!Application.IsPlaying(this)) //如果没有在游戏
        {
            currentTilemap = GetComponent<Tilemap>();
            if(mapData != null)
            {
                mapData.tileProperties.Clear(); //开始时清空
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(this)) //如果没有在游戏
        {
            currentTilemap = GetComponent<Tilemap>();
            UpdateTileMap();
#if UNITY_EDITOR //仅在编辑器下通过
            if(mapData!=null)
            {
                EditorUtility.SetDirty(mapData); //重新读入后数据不会消失
            }
#endif
        }
        
    }

    private void UpdateTileMap()
    {
        currentTilemap.CompressBounds();
        if (!Application.IsPlaying(this)) //如果没有在游戏
        {
            if (mapData != null)
            {
                //左上角作为起始点
                Vector3Int startPos = currentTilemap.cellBounds.min;
                //右上角作为终点
                Vector3Int endPos = currentTilemap.cellBounds.max;
                //Debug.Log(startPos);
                //Debug.Log(endPos);
                //循环，将范围内的tile全部存取
                for (int i = startPos.x; i < endPos.x; i++)
                {
                    for (int j = startPos.y; j < endPos.y; j++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(i, j, 0));
                        if (tile != null) //不能写！tile
                        {
                            TileProperty newTile = new TileProperty
                            {
                                tileCoordinate = new Vector2Int(i, j),
                                gridType = this.gridType,
                                boolType = true
                            };
                            mapData.tileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}
