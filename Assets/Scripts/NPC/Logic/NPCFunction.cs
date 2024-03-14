using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFunction : MonoBehaviour
{
    public BagItemList_SO shopBagData;
    private bool isOpen;

    private void Update()
    {
        if(isOpen && Input.GetKeyDown(KeyCode.Escape)) //����ESC�����ر��̵�
        {
            //�ر��̵�
            CloseShop();
        }
    }

    /// <summary>
    /// ���̵�
    /// </summary>
    public void OpenShop()
    {
        isOpen = true;
        EventHandler.CallOpenBaseBagEvent(SlotType.Shop, shopBagData); //���̵걳��
        //�ı���Ϸ״̬��������ͣ
        EventHandler.CallUpdateGameStateEvent(GameState.GamePause); 
    }

    /// <summary>
    /// �ر��̵�
    /// </summary>
    public void CloseShop() 
    { 
        isOpen = false;
        EventHandler.CallCloseBaseBagEvent(SlotType.Shop, shopBagData);
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
    }
}
