using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mfarm.Map;
using Mfarm.cropPlant;
using Mfarm.inventory;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, seed, tool, commodity; //ָ����ͬ�����ͼ��

    public Sprite tempSprite; //�м����

    private Image cursorImage;
    private RectTransform cursorCanvas;

    //��꽨��ͼƬ
    private Image buildImage;

    //�����
    private Camera mainCamera; //��Ļ����תΪ��������
    private Grid currentGrid;

    //���λ��
    private Vector3 mouseWorldPosition; //�����������
    private Vector3Int gridMousePosition; //���������������
    
    //������
    private bool cursorEnable; //�ҵ�grid�����ſ��Զ�
    private bool cursorPositionValid;

    //��Ʒ��Ϣ
    private ItemDetails currentItemDetails;

    //���λ��
    private Transform playerTransform => FindObjectOfType<Player>().transform;

    private bool Selected;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
    }



    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        //��Ҫ��cursorImage��cursorCanvas�ĵ�һ������

        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);

        mainCamera = Camera.main;
    }

    private void OnAfterLoadSceneEvent()  
    {
        currentGrid = FindObjectOfType<Grid>(); //�ҵ���ǰ������grid
    }

    private void OnBeforeUnLoadSceneEvent()
    {
        cursorEnable = false;
    }

    private void Update()
    {
        CheckPlayerInput();
        if (cursorImage == null) return;

        cursorImage.transform.position =Input.mousePosition; //ͼƬλ�ü����λ��

        //buildImage.rectTransform.position = Input.mousePosition;

        //Debug.Log("1"+cursorEnable);
        //Debug.Log("2" + InteractWithUI());
        if (cursorEnable && !InteractWithUI())
        {
            SetCursorImage(tempSprite);
            CheckCursorValid();
        }
        else
        {
            SetCursorImage(normal);
            //buildImage.gameObject.SetActive(false);
        }
        //if (Selected && currentItemDetails.itemType == ItemType.Furniture)
        //{
        //    buildImage.gameObject.SetActive(true);
        //    buildImage.sprite = currentItemDetails.itemOnWorldSprite;
        //}
        //else
        //{
        //    buildImage.gameObject.SetActive(false);
        //}
        //�˴�����һ��bug

    }


    #region ���������ʽ
    /// <summary>
    /// �������ͼƬ����ɫ
    /// </summary>
    /// <param name="sprite">���ͼƬ</param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// ����������
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid= true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, (float)0.5);
    }
    /// <summary>
    /// ������겻����
    /// </summary>
    private void SetCursorInValid()
    {
        cursorPositionValid= false;
        cursorImage.color = new Color(1, 0, 0, (float)0.5);
        buildImage.color = new Color(1, 0, 0, (float)0.5);
    }
    #endregion

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        Selected = isSelected;
        //��ͬ���͵���Ʒ������ͷ��ͼƬҲ����ͬ
        if (isSelected)
        {
            currentItemDetails = itemDetails; //�õ�ѡ����Ʒ����Ϣ
            tempSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.BreakTool => tool,
                ItemType.WaterTool => tool,
                ItemType.CollectTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Commodity => commodity,
                _ => normal
            };
            cursorEnable = true;
            if (itemDetails.itemType == ItemType.Furniture) //�����ͼֽ����ô��ʾ�����ͼƬ
            {
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
            }
            else
            {
                buildImage.gameObject.SetActive(false);
            }
        }
        else
        {
            currentItemDetails = null;
            tempSprite = normal;
            cursorEnable = false;
            //Debug.Log("buildImage�ѹر�-2");
            buildImage.gameObject.SetActive(false);
        }
    }

    //�����ҵ�������
    void CheckPlayerInput()
    {
        if(Input.GetMouseButtonDown(0) && cursorPositionValid) //0�������������
        {
            EventHandler.CallMouseClickEvent(mouseWorldPosition, currentItemDetails);
        
        }
    }

    /// <summary>
    /// �������Ƿ����
    /// </summary>
    /// 
    //1.�ڷ�Χ��
    //2.��������ڵ���Ʒ����Ƭ��ͼ��λ��
    private void CheckCursorValid()
    {
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); //����Ļ�ռ�תΪ����ռ�
        mouseWorldPosition.z += 10;
        gridMousePosition = currentGrid.WorldToCell(mouseWorldPosition); //��������λ��

        var playerGridPosition=currentGrid.WorldToCell(playerTransform.position);
        TileDetails currentTile =GridMapManager.Instance.GetTileDetailsOnCurrentMousePosition(gridMousePosition);

        buildImage.rectTransform.position = Input.mousePosition;

        //����ʹ�ð뾶-��겻����
        if (Mathf.Abs(gridMousePosition.x-playerGridPosition.x)>currentItemDetails.itemUseRadius || Mathf.Abs(gridMousePosition.y - playerGridPosition.y) > currentItemDetails.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }

        //�����ǰ����Ƭλ�ò�Ϊ��
        if (currentTile != null)
        {
            //��ȡ��ǰ������Ϣ
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemId);
            //ֻ�������
            Crop crop = GridMapManager.Instance.GetCrop(mouseWorldPosition);
            //WORKFLOW:
            switch (currentItemDetails.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daysSinceDug > -1 && currentTile.seedItemId == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Commodity:
                    if(currentTile.canDropItem&&currentItemDetails.canDrop) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDug>-1 && currentTile.daysSinceWatered ==-1 ) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.ChopTool:
                    if(crop!=null)
                    {
                        if (crop.canChop && crop.cropDetails.CheckToolAvailable(currentItemDetails.itemID))
                        {
                            SetCursorValid();
                        }
                        else SetCursorInValid();
                    }
                    break;
                case ItemType.CollectTool:
                    if (currentCrop != null)
                    { 
                        if(currentCrop.CheckToolAvailable(currentItemDetails.itemID))
                        {
                            if (currentTile.growthDays >= currentCrop.totalGrowthDays) SetCursorValid();
                        }
                        else SetCursorInValid();
                    }
                    else SetCursorInValid();
                    break;
                case ItemType.BreakTool:
                    if(crop!= null)
                    {
                        if (crop.cropDetails.CheckToolAvailable(currentItemDetails.itemID))
                        {
                            SetCursorValid();
                        }
                        else SetCursorInValid();
                    }
                    break;
                case ItemType.ReapTool:
                    if(GridMapManager.Instance.GetReapableItemInRadius(mouseWorldPosition, currentItemDetails)) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Furniture:
                    if(currentTile.canPlaceFurniture && InventoryManager.Instance.CheckBuild(currentItemDetails.itemID))
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
            }
        }
        else
        {
            SetCursorInValid();
        }
    }

    /// <summary>
    /// �ж��Ƿ��UI����
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else return false;
    }
}
