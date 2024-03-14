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
    private int index => transform.GetSiblingIndex(); //��ȡ��Ʒ�ڸ�Ŀ¼�µ����

    //��ǰSlotData
    private DataSlot currentData;


    private void Awake()
    {
        currentButton = GetComponent<Button>();
        //��������ӷ���
        currentButton.onClick.AddListener(LoadGameData);
        //�Ȼ�ȡ��Slot���Ƿ�������
        currentData = SaveLoadManager.Instance.dataSlotList[index];
    }

    private void OnEnable()
    {
        LoadSlotUIText(); //�ڿ�ʼ��Ϸʱ
    }

    private void LoadSlotUIText()
    {
        if(currentData != null )
        {
            //��ʾ���ں͵ص�
            gameTime.text = currentData.slotDate;
            gameScene.text = currentData.slotScene;
        }
        else
        {
            gameTime.text = "�������";
            gameScene.text = "����ʼδ֪��ð�հɣ�";
        }
    }

    private void LoadGameData()
    {
        if(currentData != null) //��ǰSlot��������
        {
            //��ԭ�е����ݼ��س���
            SaveLoadManager.Instance.Load(index);
        }
        else //��ʼһ������Ϸ
        {
            Debug.Log("����Ϸ");
            Debug.Log(index);
            EventHandler.CallStartNewGame(index);
        }
    }
}
