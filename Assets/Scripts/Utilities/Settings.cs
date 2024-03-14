using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public const float durationTime = 0.35f;
    public const float targetAlpha = 0.45f;

    //这里要加const别的地方才能调用
    //因为不是常量的话，多个地方同时改变，会出现问题

    public const  float timeThreshold = 0.01f; //数值越小，时间越快 和现实时间为1:500
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int dayHold = 23;
    public const int monthHold = 9; //一个月定为10天，这样速度比较快
    public const int seasonHold = 3;

    public const float fadeDurationTime = 1.0f; //UI canvas的持续时间

    public const int maxReapCount = 2;

    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f; //1格， 20*20

    public const float animationBreakTime = 5f; //npc走到指定位置后，间隔一段时间播放动画

    //灯光
    public const float lightChangeDuration = 25f; //25s切换灯光
    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);

    //金钱
    public static int startMoney = 100;

}
