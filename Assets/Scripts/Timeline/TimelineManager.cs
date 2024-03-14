using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : Singleton<TimelineManager>
{
    public PlayableDirector startDirector;
    private PlayableDirector currentDirector;

    private bool isPause;
    private bool isDone;
    private bool hasStarted; 

    public bool IsDone { set => isDone = value; }

    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
        
    }

    private void OnEnable()
    {
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
    }

    private void OnAfterLoadSceneEvent()
    {
        //playableDirector组件上有play on awake，如果勾选上，意味我将在awake后立刻播放
        //根据在TimeManager中写的判断来看，加载场景前后会更新gameClockPause
        //当然我希望在场景加载后再进行CallUpdateGameStateEvent
        //但是我勾选play on awake后，则会先CallUpdateGameStateEvent后加载场景，这会导致我的gameState失效
        //所以我要取消勾选啊Play on awake，选择手动让currentDirector.Play()
        //但是此方法是每一次加载场景都会实现，所以加入一个bool的判断即hasStarted
        currentDirector = FindObjectOfType<PlayableDirector>();
        if (currentDirector != null && !hasStarted)
        {
            currentDirector.Play();
            hasStarted = true;
        }
    }

    private void Update()
    {
        //如果暂停并且按下空格并且播放完成
        if(isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }

    public void PauseTimeline(PlayableDirector Director)
    {
        currentDirector = Director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause= true;
    }
}
