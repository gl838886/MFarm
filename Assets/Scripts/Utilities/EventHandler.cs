using Mfarm.dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler  
{
    public static event Action<InventoryLocation, List<BagItem>> UpdateInventoryUI; //注册一个事件

    public static void CallUpdateInventoryUI(InventoryLocation location, List<BagItem> list) //实例化
    {
        if (UpdateInventoryUI != null)
        {
            UpdateInventoryUI.Invoke(location, list);
        }
        //UpdateInventoryUI?.Invoke(location, list);
    }

    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int id, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(id, pos);
    }
    /// <summary>
    /// 扔掉背包内的物品
    /// </summary>
    public static event Action<int, Vector3, ItemType> DropItemInBag;
    public static void CallDropItemInBag(int itemId, Vector3 pos, ItemType itemType)
    {
        DropItemInBag?.Invoke(itemId, pos, itemType);
    }

    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }

    public static event Action<int, int, int , Season> UpdateTimeEvent;
    public static void CallUpdateTimeEvent(int minute, int hour, int day, Season season)
    {
        UpdateTimeEvent?.Invoke(minute, hour, day, season);
    }

    public static event Action<int, Season> UpdateGameDate;
    public static void CallUpdateGameDate(int day, Season season)
    {
        UpdateGameDate?.Invoke(day,season);
    }

    public static event Action<int, int, int, int, Season> UpdateDateEvent;
    public static void CallUpdateDateEvent(int hour, int day, int month, int year, Season season)
    {
        UpdateDateEvent?.Invoke(hour, day, month, year, season);
    }

    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string transition, Vector3 pos)
    {
        TransitionEvent?.Invoke(transition, pos);
    }

    //场景卸载之前
    public static event Action BeforeUnLoadSceneEvent;
    public static void CallBeforeUnLoadSceneEvent()
    {
        BeforeUnLoadSceneEvent?.Invoke();
    }

    //场景加载之后
    public static event Action AfterLoadSceneEvent;
    public static void CallAfterLoadSceneEvent()
    {
        AfterLoadSceneEvent?.Invoke();
    }
    
    //场景切换后移动玩家坐标
    public static event Action<Vector3> MoveToPositionEvent;
    public static void CallMoveToPositionEvent(Vector3 pos)
    {
        MoveToPositionEvent?.Invoke(pos);
    }

    //玩家点击事件
    public static event Action<Vector3, ItemDetails> MouseClickEvent;
    public static void CallMouseClickEvent(Vector3 mousePos, ItemDetails itemDetails)
    {
        MouseClickEvent?.Invoke(mousePos, itemDetails);
    }

    //执行动画事件
    public static event Action<Vector3, ItemDetails> ExecuteActionAftreAnimation;
    public static void CallExecuteActionAftreAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAftreAnimation?.Invoke(pos, itemDetails);
    }

    //种植种子
    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID, TileDetails itemDetails)
    {
        PlantSeedEvent?.Invoke(ID, itemDetails);
    }

    //收获作物-
    public static event Action<int> HarvestCropEvent;
    public static void CallHarvestCropEvent(int ID)
    {
        HarvestCropEvent?.Invoke(ID);
    }

    public static event Action RefreshMapEvent;
    public static void CallRefreshMapEvent()
    {
        RefreshMapEvent?.Invoke();
    }

    
    public static event Action<ParticleEffectType, Vector3> ParticleEffectEvent;
    /// <summary>
    /// //执行特效
    /// </summary>
    /// <param name="effectType">特效的种类</param>
    /// <param name="effectPosition">生成特效位置</param>
    public static void CallParticleEffectEvent(ParticleEffectType effectType, Vector3 effectPosition)
    {
        ParticleEffectEvent?.Invoke(effectType, effectPosition);
    }

    //预生成场景内物品
    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    //显示与NPC的对话
    public static event Action<DialoguePiece> ShowDialogueEvent;
    public static void CallShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        ShowDialogueEvent?.Invoke(dialoguePiece);
    }

    //打开商店
    public static event Action<SlotType, BagItemList_SO> OpenBaseBagEvent;
    public static void CallOpenBaseBagEvent(SlotType slotType, BagItemList_SO bagItemList_SO)
    {
        OpenBaseBagEvent?.Invoke(slotType, bagItemList_SO);
    }

    //关闭商店
    public static event Action<SlotType, BagItemList_SO> CloseBaseBagEvent;
    public static void CallCloseBaseBagEvent(SlotType slotType, BagItemList_SO bagItemList_SO)
    {
        CloseBaseBagEvent?.Invoke(slotType, bagItemList_SO);
    }

    //设置游戏状态
    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }

    //买卖物品
    public static event Action<ItemDetails, bool> ShowTradeUIEvent;
    public static void CallShowTadeUIEvent(ItemDetails itemDetails, bool isSell)
    {
        ShowTradeUIEvent?.Invoke(itemDetails, isSell);
    }

    //切换灯光
    public static event Action<Season, LightShift, float> ChangeLightEvent;
    public static void CallChangeLightEvent(Season season, LightShift lightShift, float timeDifference)
    {
        ChangeLightEvent?.Invoke(season, lightShift, timeDifference);
    }

    //建造家具
    public static event Action<int, Vector3> BuildFurnitureEvent;
    public static void CallBuildFurnitureEvent(int ID, Vector3 mousePosition)
    {
        BuildFurnitureEvent?.Invoke(ID, mousePosition);
    }

    //音效
    public static event Action<AudioDetails> InitSound;
    public static void CallInitSound(AudioDetails audioDetails)
    {
        InitSound?.Invoke(audioDetails);
    }

    //播放音效
    public static event Action<SoundName> PlaySoundEvent;
    public static void CallPlaySoundEvent(SoundName soundName)
    {
        PlaySoundEvent?.Invoke(soundName);
    }

    //点击slot后，开始新游戏
    public static event Action<int> StartNewGame;
    public static void CallStartNewGame(int index)
    {
        StartNewGame?.Invoke(index);
    }

    //结束游戏事件
    public static event Action EndCurrentGame;
    public static void CallEndCurrentGame()
    {
        EndCurrentGame?.Invoke();
    }
}
