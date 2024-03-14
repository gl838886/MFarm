using Mfarm.inventory;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using UnityEditor.Animations;
using UnityEngine;

public class SwitchAnimatorOverride : MonoBehaviour
{
    private Animator[] animators;

    public SpriteRenderer holdItem;

    [Header("动画列表")]
    public List<AnimatorType> animTypes;

    private Dictionary<string,Animator> animNameDict= new Dictionary<string,Animator>();

    private void Awake()
    {
        animators= GetComponentsInChildren<Animator>();
        foreach(Animator anim in animators)
        {
            animNameDict.Add(anim.name, anim);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
        EventHandler.HarvestCropEvent += OnHarvestCropEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
        EventHandler.HarvestCropEvent -= OnHarvestCropEvent;
    }

    //生成图片
    private void OnHarvestCropEvent(int ID)
    {
        //获取图片
        Sprite cropSprite = InventoryManager.Instance.getItemDetails(ID).itemOnWorldSprite;
        if(holdItem.enabled == false)
        {
            StartCoroutine(ShowItem(cropSprite));
        }
    }

    private IEnumerator ShowItem(Sprite sprite)
    {
        holdItem.sprite = sprite;
        holdItem.enabled = true;
        yield return new WaitForSeconds(0.2f);
        holdItem.enabled = false;
    }

    private void OnBeforeUnLoadSceneEvent()
    {
        holdItem.enabled= false;
        SwitchAnimator(PartType.None);
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        PartType currentType = itemDetails.itemType switch
        {
            //WORKFLOW:
            ItemType.Seed => PartType.Carry,
            ItemType.Commodity=> PartType.Carry,
            ItemType.HoeTool=> PartType.Hoe,
            ItemType.WaterTool => PartType.Water,
            ItemType.ChopTool => PartType.Chop,
            ItemType.CollectTool => PartType.Collect,
            ItemType.BreakTool=> PartType.Break,
            ItemType.ReapTool=>PartType.Reap,
            _ => PartType.None
        };
        if(!isSelected)
        {
            currentType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if(currentType== PartType.Carry)
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite;
                holdItem.enabled = true;
            }
            else
            {
                holdItem.enabled = false;
                SwitchAnimator(PartType.None);
            }
            
        }
        SwitchAnimator(currentType);
    }

    private void SwitchAnimator(PartType currentType)
    {
        foreach(var item in animTypes)
        {
            if(item.partType == currentType)
            {
                animNameDict[item.partName.ToString()].runtimeAnimatorController = item.animColl;
            }
        }
    }
}
