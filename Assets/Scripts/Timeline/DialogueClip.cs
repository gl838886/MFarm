using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    public ClipCaps clipCaps => ClipCaps.None;
    public DialogueBehaviour dialogueBehaviour = new DialogueBehaviour();

    //����Ҫʵ�ּ̳��Ի��� PlayableAsset �ĳ����Ա CreatePlayable ����������������ڴ����� PlayableAsset ��ص� Playable ʵ����
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, dialogueBehaviour);
        return playable;
    }
}
