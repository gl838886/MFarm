using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueClip : PlayableAsset, ITimelineClipAsset
{
    public ClipCaps clipCaps => ClipCaps.None;
    public DialogueBehaviour dialogueBehaviour = new DialogueBehaviour();

    //您需要实现继承自基类 PlayableAsset 的抽象成员 CreatePlayable 方法。这个方法用于创建与 PlayableAsset 相关的 Playable 实例。
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, dialogueBehaviour);
        return playable;
    }
}
