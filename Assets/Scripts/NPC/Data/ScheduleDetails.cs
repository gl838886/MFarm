using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScheduleDetails :IComparable<ScheduleDetails>
{
    public int hour, minute, day;
    public int priority;
    public Season season;
    public string targetScene;
    public Vector2Int targetGridPosition;
    public AnimationClip animationClipAtStop; //����������ִ�еĶ���
    public bool isInteractable; //���н������в��ɽ���

    public ScheduleDetails(int hour, int minute, int day, int priority, Season season, string targetScene, Vector2Int targetGridPosition, AnimationClip animationClipAtStop, bool isInteractable)
    {
        this.hour = hour;
        this.minute = minute;
        this.day = day;
        this.priority = priority;
        this.season = season;
        this.targetScene = targetScene;
        this.targetGridPosition = targetGridPosition;
        this.animationClipAtStop = animationClipAtStop;
        this.isInteractable = isInteractable;
    }

    public int Time => hour * 100 + minute; //��4λ�����бȽ�
    public int CompareTo(ScheduleDetails other)
    {
        if (Time == other.Time)
        {
            if (priority > other.priority) return 1;
            else return -1;
        }
        else if (Time > other.Time) return 1;
        else if (Time < other.Time) return -1;
        return 0;
    }
}
