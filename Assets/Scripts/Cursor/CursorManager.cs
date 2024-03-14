using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mfarm.Map;
using Mfarm.cropPlant;
using Mfarm.inventory;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, seed, tool, commodity; //指代不同的鼠标图像

    public Sprite tempSprite; //中间变量

    private Image cursorImage;
    private RectTransform cursorCanvas;

    //鼠标建造图片
    private Image buildImage;

    //鼠标检测
    private Camera mainCamera; //屏幕坐标转为世界坐标
    private Grid currentGrid;

    //鼠标位置
    private Vector3 mouseWorldPosition; //鼠标世界坐标
    private Vector3Int gridMousePosition; //鼠标网格世界坐标
    
    //鼠标可用
    private bool cursorEnable; //找到grid后鼠标才可以动
    private bool cursorPositionValid;

    //物品信息
    private ItemDetails currentItemDetails;

    //玩家位置
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
        //我要的cursorImage是cursorCanvas的第一个孩子

        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);

        mainCamera = Camera.main;
    }

    private void OnAfterLoadSceneEvent()  
    {
        currentGrid = FindObjectOfType<Grid>(); //找到当前场景的grid
    }

    private void OnBeforeUnLoadSceneEvent()
    {
        cursorEnable = false;
    }

    private void Update()
    {
        CheckPlayerInput();
        if (cursorImage == null) return;

        cursorImage.transform.position =Input.mousePosition; //图片位置即鼠标位置

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
        //此处出现一个bug

    }


    #region 设置鼠标样式
    /// <summary>
    /// 设置鼠标图片及颜色
    /// </summary>
    /// <param name="sprite">鼠标图片</param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid= true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, (float)0.5);
    }
    /// <summary>
    /// 设置鼠标不可用
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
        //不同类型的物品，鼠标箭头的图片也不相同
        if (isSelected)
        {
            currentItemDetails = itemDetails; //拿到选中物品的信息
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
            if (itemDetails.itemType == ItemType.Furniture) //如果是图纸，那么显示建造的图片
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
            //Debug.Log("buildImage已关闭-2");
            buildImage.gameObject.SetActive(false);
        }
    }

    //检查玩家的鼠标操作
    void CheckPlayerInput()
    {
        if(Input.GetMouseButtonDown(0) && cursorPositionValid) //0代表左键被按下
        {
            EventHandler.CallMouseClickEvent(mouseWorldPosition, currentItemDetails);
        
        }
    }

    /// <summary>
    /// 检测鼠标是否可用
    /// </summary>
    /// 
    //1.在范围内
    //2.点击背包内的物品再瓦片地图的位置
    private void CheckCursorValid()
    {
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); //将屏幕空间转为世界空间
        mouseWorldPosition.z += 10;
        gridMousePosition = currentGrid.WorldToCell(mouseWorldPosition); //返回网格位置

        var playerGridPosition=currentGrid.WorldToCell(playerTransform.position);
        TileDetails currentTile =GridMapManager.Instance.GetTileDetailsOnCurrentMousePosition(gridMousePosition);

        buildImage.rectTransform.position = Input.mousePosition;

        //大于使用半径-鼠标不可用
        if (Mathf.Abs(gridMousePosition.x-playerGridPosition.x)>currentItemDetails.itemUseRadius || Mathf.Abs(gridMousePosition.y - playerGridPosition.y) > currentItemDetails.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }

        //如果当前的瓦片位置不为空
        if (currentTile != null)
        {
            //获取当前作物信息
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemId);
            //只针对树干
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
    /// 判读是否和UI互动
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
