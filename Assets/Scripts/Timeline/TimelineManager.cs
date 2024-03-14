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
        //playableDirector�������play on awake�������ѡ�ϣ���ζ�ҽ���awake�����̲���
        //������TimeManager��д���ж����������س���ǰ������gameClockPause
        //��Ȼ��ϣ���ڳ������غ��ٽ���CallUpdateGameStateEvent
        //�����ҹ�ѡplay on awake�������CallUpdateGameStateEvent����س�������ᵼ���ҵ�gameStateʧЧ
        //������Ҫȡ����ѡ��Play on awake��ѡ���ֶ���currentDirector.Play()
        //���Ǵ˷�����ÿһ�μ��س�������ʵ�֣����Լ���һ��bool���жϼ�hasStarted
        currentDirector = FindObjectOfType<PlayableDirector>();
        if (currentDirector != null && !hasStarted)
        {
            currentDirector.Play();
            hasStarted = true;
        }
    }

    private void Update()
    {
        //�����ͣ���Ұ��¿ո��Ҳ������
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
