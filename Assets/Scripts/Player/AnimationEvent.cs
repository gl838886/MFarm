using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
     //用于在行走跑步动画中添加事件
     public void WalkSoundEvent()
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepSoft);
    }

    public void RunSoundEvent()
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepHard);
    }
}
