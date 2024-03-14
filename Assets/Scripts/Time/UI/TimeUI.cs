using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNight;
    public RectTransform clockParent;

    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeDate;

    public Image seasonImage;

    public Sprite[] seasonSprites;

    private List<GameObject> clockBlocks = new List<GameObject>();

    private void Awake()
    {
        for(int i=0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
        }
    }

    private void OnEnable()
    {
        EventHandler.UpdateTimeEvent += OnUpdateTimeEvent;
        EventHandler.UpdateDateEvent += OnUpdateDateEvent;
    }

    private void OnDisable()
    {
        EventHandler.UpdateTimeEvent -= OnUpdateTimeEvent;
        EventHandler.UpdateDateEvent -= OnUpdateDateEvent;
    }

    private void OnUpdateDateEvent(int hour, int day, int month, int year, Season season)
    {
        dateText.text = year.ToString() + "��" + month.ToString() + "��" + day.ToString() + "��";
        //���¼���
        seasonImage.sprite = seasonSprites[(int)season];
        //����switchBlocks
        SwitchClockBlocks(hour);
        //��תdayNight
        RotateDayNight(hour);
    }

    private void OnUpdateTimeEvent(int minute, int hour, int day, Season season)
    {
        timeDate.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    private void SwitchClockBlocks(int hour)
    {
        int index = hour / 4;
        if(index == 0)
        {
            foreach(var item in clockBlocks)
            {
                item.SetActive(false);
            }
        }
        else
        {
            for(int i = 0; i < index; i++)
            {
                clockBlocks[i].SetActive(true);
            }
        }
    }

    private void  RotateDayNight(int hour)
    {
        var target = new Vector3(0, 0, hour * 15);
        dayNight.DORotate(target, 1f, RotateMode.Fast);
    }
}
