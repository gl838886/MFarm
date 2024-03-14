using Mfarm.dialogue;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    //将用户自定义的行为加到playable graph里
    public PlayableDirector director;
    public DialoguePiece dialoguePiece ;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        if(Application.isPlaying)
        {
            if(dialoguePiece.hasToPause)
            {
                //暂停timeLine
                TimelineManager.Instance.PauseTimeline(director);
            }
            else
            {
                EventHandler.CallShowDialogueEvent(null);
            }
        }
    }

    //timeLine在执行时实时检测，类似与update
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(Application.isPlaying)
        {
            TimelineManager.Instance.IsDone = dialoguePiece.isDone;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        EventHandler.CallShowDialogueEvent(null);
    }

    public override void OnGraphStart(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.GamePause);
        //Debug.Log("Start");
    }

    public override void OnGraphStop(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
        //Debug.Log("Stop");
    }
}
