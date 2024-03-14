using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public BagItemList_SO shopBagData;
    private bool isOpen;

    private void Update()
    {
        if(isOpen && Input.GetKeyDown(KeyCode.Escape)) //按下ESC键，关闭商店
        {
            //关闭商店
            CloseShop();
        }
    }

    /// <summary>
    /// 打开商店
    /// </summary>
    public void OpenShop()
    {
        isOpen = true;
        EventHandler.CallOpenBaseBagEvent(SlotType.Shop, shopBagData); //打开商店背包
        //改变游戏状态，人物暂停
        EventHandler.CallUpdateGameStateEvent(GameState.GamePause); 
    }

    /// <summary>
    /// 关闭商店
    /// </summary>
    public void CloseShop() 
    { 
        isOpen = false;
        EventHandler.CallCloseBaseBagEvent(SlotType.Shop, shopBagData);
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    }
}
