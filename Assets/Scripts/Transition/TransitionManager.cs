using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Mfarm.Save;

public class TransitionManager : Singleton<TransitionManager>, ISaveable
{
    public string startSceneName = string.Empty;

    private CanvasGroup fadeCanvasGroup;

    private bool isFade;

    public string GUID => GetComponent<DataGUID>().GUID;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.LoadScene("UI", LoadSceneMode.Additive); //直接在当前场景上进行加载
    }

    private void OnEnable()
    {
        EventHandler.TransitionEvent += OnTransitionEvent;
        EventHandler.StartNewGame += OnStartNewGame;
        EventHandler.EndCurrentGame += OnEndCurrentGame;
    }

    private void OnDisable()
    {
        EventHandler.TransitionEvent -= OnTransitionEvent;
        EventHandler.StartNewGame -= OnStartNewGame;
        EventHandler.EndCurrentGame -= OnEndCurrentGame;
    }


    private void OnStartNewGame(int obj)
    {
        StartCoroutine(LoadGameSaveDataScene(startSceneName));
    }

    private void OnTransitionEvent(string sceneName, Vector3 targetPosition)
    {
        if (!isFade)
            StartCoroutine(Transition(sceneName, targetPosition));
    }


    private void OnEndCurrentGame()
    {
        StartCoroutine(ExitCurrentGame());
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
        fadeCanvasGroup = FindObjectOfType<CanvasGroup>();
    }

    /// <summary>
    /// 转换场景（卸载当前场景并加载目标场景）
    /// </summary>
    /// <param name="sceneName">目标场景</param>
    /// <param name="targetPosition">人物的位置</param>
    /// <returns></returns>
    private IEnumerator Transition(string sceneName, Vector3 targetPosition)
    {
        EventHandler.CallBeforeUnLoadSceneEvent(); //场景加载前所需调用的函数

        yield return Fade(1);

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()); //卸载当前场景

        yield return LoadSceneSetActive(sceneName); //加载想要的场景

        EventHandler.CallMoveToPositionEvent(targetPosition); //移动玩家位置

        EventHandler.CallAfterLoadSceneEvent(); //场景加载后所需调用的事件

        yield return Fade(0);
    }

    /// <summary>
    /// 加载并激活目标场景
    /// </summary>
    /// <param name="sceneName">目标场景</param>
    /// <returns></returns>
    private IEnumerator LoadSceneSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        //Debug.Log(SceneManager.sceneCount); 3 
        SceneManager.SetActiveScene(newScene);
    }

    /// <summary>
    /// 淡入淡出场景
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <returns></returns>
    private IEnumerator Fade(float targetAlpha)
    {
        isFade= true;

        fadeCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / Settings.fadeDurationTime;

        while(!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        fadeCanvasGroup.blocksRaycasts = false;

        isFade = false;
    }

    public IEnumerator LoadGameSaveDataScene(string sceneName)
    {
        yield return Fade(1f);

        if(SceneManager.GetActiveScene().name != "PersistentScene")
        {
            EventHandler.CallAfterLoadSceneEvent(); //其实是unload
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        }

        yield return LoadSceneSetActive(sceneName);
        EventHandler.CallAfterLoadSceneEvent();
        yield return Fade(0);
    }

    private IEnumerator ExitCurrentGame()
    {
        EventHandler.CallBeforeUnLoadSceneEvent();
        yield return Fade(1f);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        yield return Fade(0);
    }

    public GameSaveData GenerateGameData()
    {
        GameSaveData saveData  = new GameSaveData();
        saveData.sceneName = SceneManager.GetActiveScene().name;
        return saveData;
    }

    public void RestoreGameData(GameSaveData data)
    {
        StartCoroutine(LoadGameSaveDataScene(data.sceneName));
    }


}
