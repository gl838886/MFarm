using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public const float durationTime = 0.35f;
    public const float targetAlpha = 0.45f;

    //����Ҫ��const��ĵط����ܵ���
    //��Ϊ���ǳ����Ļ�������ط�ͬʱ�ı䣬���������

    public const  float timeThreshold = 0.01f; //��ֵԽС��ʱ��Խ�� ����ʵʱ��Ϊ1:500
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int dayHold = 23;
    public const int monthHold = 9; //һ���¶�Ϊ10�죬�����ٶȱȽϿ�
    public const int seasonHold = 3;

    public const float fadeDurationTime = 1.0f; //UI canvas�ĳ���ʱ��

    public const int maxReapCount = 2;

    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f; //1�� 20*20

    public const float animationBreakTime = 5f; //npc�ߵ�ָ��λ�ú󣬼��һ��ʱ�䲥�Ŷ���

    //�ƹ�
    public const float lightChangeDuration = 25f; //25s�л��ƹ�
    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);

    //��Ǯ
    public static int startMoney = 100;

}
