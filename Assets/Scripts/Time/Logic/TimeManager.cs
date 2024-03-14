using Mfarm.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>, ISaveable
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;

    private int monthInSeason = 3;
    private Season seasonNum = Season.����;

    private bool gameClockPause;
    private float tikTime;

    public TimeSpan gameTime => new TimeSpan(gameHour, gameMinute, gameSecond);

    public string GUID => GetComponent<DataGUID>().GUID;

    public float timeDifference; //ʱ����5��01���ݺ�5��03���ݹ���Ӧ�ò�һ����

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
        gameClockPause = true;
        //EventHandler.CallUpdateTimeEvent(gameMinute, gameHour, gameDay, seasonNum);
        //EventHandler.CallUpdateDateEvent(gameHour, gameDay, gameMonth, gameYear, seasonNum);
        ////�л��ƹ�-lightManager������¼�
        //EventHandler.CallChangeLightEvent(seasonNum, GetCurrentLightShift(), timeDifference);
    }

    private void OnEnable()
    {
        EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGame += OnStartNewGame;
        EventHandler.EndCurrentGame += OnEndCurrentGame;
    }


    private void OnDisable()
    {
        EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGame -= OnStartNewGame;
        EventHandler.EndCurrentGame -= OnEndCurrentGame;
    }


    private void OnUpdateGameStateEvent(GameState gameState)
    {
        if (gameState == GameState.GamePause)
            gameClockPause = true;
        else
            gameClockPause = false;
        //Debug.Log(2);
    }

    private void OnAfterLoadSceneEvent()
    {
        EventHandler.CallUpdateTimeEvent(gameMinute, gameHour, gameDay, seasonNum);
        EventHandler.CallUpdateDateEvent(gameHour, gameDay, gameMonth, gameYear, seasonNum);
        //�л��ƹ�-lightManager������¼�
        EventHandler.CallChangeLightEvent(seasonNum, GetCurrentLightShift(), timeDifference);
        //Debug.Log(1);
        gameClockPause = false;
    }

    private void OnBeforeUnLoadSceneEvent()
    {
        gameClockPause = true;
    }

    private void OnStartNewGame(int obj)
    {
        NewGameTime();
        gameClockPause= false;
    }

    private void OnEndCurrentGame()
    {
        gameClockPause= true;
    }

    //����Ҫ�ˣ���OnStartNewGame�е��ü���
    //protected override void Awake() //��Ϊ����������д
    //{
    //    base.Awake();
    //    NewGameTime();
    //}

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay = 0;
        gameMonth = 0;
        gameYear = 0;
        seasonNum = Season.����;
        EventHandler.CallUpdateDateEvent(gameHour, gameDay, gameMonth, gameYear, seasonNum);
    }

    private void Update()
    {
        //updateÿһ֡����һ��
        //Time.deltaTime����һ֡����һ֡��ʱ����
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if(tikTime > Settings.timeThreshold )
            {
                UpdateGameTime(); //Ҳ����˵����ÿ��һ��timeThreshold����һ�μ�����Ϸ��+1s
                //Debug.Log(gameSecond + " " + gameMinute+ " " + gameHour);
                tikTime= 0;
            }
        }
        if(Input.GetKeyDown(KeyCode.G)) //����
        {
            gameDay++;
            EventHandler.CallUpdateGameDate(gameDay, seasonNum);
            EventHandler.CallUpdateDateEvent(gameHour,gameDay,gameMonth, gameYear, seasonNum);
        }
        if (Input.GetKeyDown(KeyCode.H)) //����
        {
            if (gameHour < Settings.dayHold-5)
                gameHour+=5;
            EventHandler.CallUpdateGameDate(gameDay, seasonNum);
            EventHandler.CallUpdateDateEvent(gameHour, gameDay, gameMonth, gameYear, seasonNum);
        }
    }
    private void UpdateGameTime()
    {
        gameSecond++;
        if(gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;
            if(gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;
                if(gameHour > Settings.dayHold)
                {
                    gameDay++;
                    gameHour = 0;
                    if(gameDay >Settings.monthHold)
                    {
                        gameMonth++;
                        gameDay = 1;
                        
                        if (gameMonth > 12)
                        {
                            gameMonth = 1;
                            gameYear++;
                        }
                        monthInSeason--;
                        if(monthInSeason == 0)
                        {
                            monthInSeason = 3;

                            int temp = (int)seasonNum;
                            temp++;
                            seasonNum = (Season)temp;
                        }
                    }
                    EventHandler.CallUpdateGameDate(gameDay, seasonNum);
                }
                EventHandler.CallUpdateDateEvent(gameHour, gameDay, gameMonth, gameYear, seasonNum);
            }
            EventHandler.CallUpdateTimeEvent(gameMinute, gameHour, gameDay, seasonNum);
            //�л��ƹ�-lightManager������¼�
            EventHandler.CallChangeLightEvent(seasonNum, GetCurrentLightShift(), timeDifference);
        }
    }

    /// <summary>
    /// ���ص�ǰ��ʱ��״�������죬ҹ��
    /// </summary>
    /// <returns></returns>
    private LightShift GetCurrentLightShift()
    {
        if(gameTime>Settings.morningTime&&gameTime<Settings.nightTime)
        {
            timeDifference = (float)(gameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }
        else if (gameTime < Settings.morningTime || gameTime > Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(gameTime - Settings.nightTime).TotalMinutes);
            return LightShift.Night;
        }

        return LightShift.Morning;
    }

    public GameSaveData GenerateGameData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>
        {
            { "gameYear", gameYear },
            { "gameMonth", gameMonth },
            { "gameDay", gameDay },
            { "gameHour", gameHour },
            { "gameMinute", gameMinute },
            { "gameSecond", gameSecond },
            { "seasonNum", (int)seasonNum }
        };
        return saveData;
    }

    public void RestoreGameData(GameSaveData data)
    {
        gameYear = data.timeDict["gameYear"];
        gameMonth = data.timeDict["gameMonth"];
        gameDay = data.timeDict["gameDay"];
        gameHour = data.timeDict["gameHour"];
        gameMinute = data.timeDict["gameMinute"];
        gameSecond = data.timeDict["gameSecond"];
        seasonNum = (Season)data.timeDict["seasonNum"];
    }
}
