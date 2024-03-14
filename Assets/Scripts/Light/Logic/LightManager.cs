using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] lightControl;

    private Season currentSeason;
    private LightShift currentLightShift;
    private float timeDifference;

    private void OnEnable()
    {
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        EventHandler.ChangeLightEvent += OnChangeLightEvent;
        EventHandler.StartNewGame += OnStartNewGame;
    }

    

    private void OnDisable()
    {
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        EventHandler.ChangeLightEvent -= OnChangeLightEvent;
        EventHandler.StartNewGame -= OnStartNewGame;
    }



    private void OnAfterLoadSceneEvent()
    {
        //ÿ�α任������Ҫ�ı�ƹ�

        lightControl = FindObjectsOfType<LightControl>();

        foreach(LightControl light in lightControl)
        {
            //�ı�ƹ�
            light.ChangeLightShift(currentSeason, currentLightShift, this.timeDifference);
        }
    }


    private void OnChangeLightEvent(Season season, LightShift lightShift, float timeDifference)
    {
        currentSeason = season;
        this.timeDifference = timeDifference;
        if(lightShift != currentLightShift) //����͵�ǰ��ͬ
        {
            currentLightShift= lightShift;
            foreach(LightControl light in lightControl) //ѭ���ı�
            {
                light.ChangeLightShift(currentSeason, currentLightShift, this.timeDifference);
            }
        }
    }

    private void OnStartNewGame(int obj)
    {
        currentLightShift = LightShift.Morning;
    }
}
