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
        //每次变换场景都要改变灯光

        lightControl = FindObjectsOfType<LightControl>();

        foreach(LightControl light in lightControl)
        {
            //改变灯光
            light.ChangeLightShift(currentSeason, currentLightShift, this.timeDifference);
        }
    }


    private void OnChangeLightEvent(Season season, LightShift lightShift, float timeDifference)
    {
        currentSeason = season;
        this.timeDifference = timeDifference;
        if(lightShift != currentLightShift) //如果和当前不同
        {
            currentLightShift= lightShift;
            foreach(LightControl light in lightControl) //循环改变
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
