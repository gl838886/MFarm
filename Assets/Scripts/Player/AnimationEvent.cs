using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
     //�����������ܲ�����������¼�
     public void WalkSoundEvent()
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepSoft);
    }

    public void RunSoundEvent()
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepHard);
    }
}
