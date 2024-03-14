using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]  //�༭������ִ��
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    public Tilemap currentTilemap;

    private void OnEnable()
    {
        if(!Application.IsPlaying(this)) //���û������Ϸ
        {
            currentTilemap = GetComponent<Tilemap>();
            if(mapData != null)
            {
                mapData.tileProperties.Clear(); //��ʼʱ���
            }
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(this)) //���û������Ϸ
        {
            currentTilemap = GetComponent<Tilemap>();
            UpdateTileMap();
#if UNITY_EDITOR //���ڱ༭����ͨ��
            if(mapData!=null)
            {
                EditorUtility.SetDirty(mapData); //���¶�������ݲ�����ʧ
            }
#endif
        }
        
    }

    private void UpdateTileMap()
    {
        currentTilemap.CompressBounds();
        if (!Application.IsPlaying(this)) //���û������Ϸ
        {
            if (mapData != null)
            {
                //���Ͻ���Ϊ��ʼ��
                Vector3Int startPos = currentTilemap.cellBounds.min;
                //���Ͻ���Ϊ�յ�
                Vector3Int endPos = currentTilemap.cellBounds.max;
                //Debug.Log(startPos);
                //Debug.Log(endPos);
                //ѭ��������Χ�ڵ�tileȫ����ȡ
                for (int i = startPos.x; i < endPos.x; i++)
                {
                    for (int j = startPos.y; j < endPos.y; j++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(i, j, 0));
                        if (tile != null) //����д��tile
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
