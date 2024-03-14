using Mfarm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Text gameTime;
    public Text gameScene;

    private Button currentButton;
    private int index => transform.GetSiblingIndex(); //获取物品在该目录下的序号

    //当前SlotData
    private DataSlot currentData;


    private void Awake()
    {
        currentButton = GetComponent<Button>();
        //给按键添加方法
        currentButton.onClick.AddListener(LoadGameData);
        //先获取该Slot上是否有数据
        currentData = SaveLoadManager.Instance.dataSlotList[index];
    }

    private void OnEnable()
    {
        LoadSlotUIText(); //在开始游戏时
    }

    private void LoadSlotUIText()
    {
        if(currentData != null )
        {
            //显示日期和地点
            gameTime.text = currentData.slotDate;
            gameScene.text = currentData.slotScene;
        }
        else
        {
            gameTime.text = "点击按键";
            gameScene.text = "来开始未知的冒险吧！";
        }
    }

    private void LoadGameData()
    {
        if(currentData != null) //当前Slot上有数据
        {
            //将原有的数据加载出来
            SaveLoadManager.Instance.Load(index);
        }
        else //开始一个新游戏
        {
            Debug.Log("新游戏");
            Debug.Log(index);
            EventHandler.CallStartNewGame(index);
        }
    }
}
