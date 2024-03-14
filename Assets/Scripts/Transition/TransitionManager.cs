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
        SceneManager.LoadScene("UI", LoadSceneMode.Additive); //ֱ���ڵ�ǰ�����Ͻ��м���
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
    /// ת��������ж�ص�ǰ����������Ŀ�곡����
    /// </summary>
    /// <param name="sceneName">Ŀ�곡��</param>
    /// <param name="targetPosition">�����λ��</param>
    /// <returns></returns>
    private IEnumerator Transition(string sceneName, Vector3 targetPosition)
    {
        EventHandler.CallBeforeUnLoadSceneEvent(); //��������ǰ������õĺ���

        yield return Fade(1);

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()); //ж�ص�ǰ����

        yield return LoadSceneSetActive(sceneName); //������Ҫ�ĳ���

        EventHandler.CallMoveToPositionEvent(targetPosition); //�ƶ����λ��

        EventHandler.CallAfterLoadSceneEvent(); //�������غ�������õ��¼�

        yield return Fade(0);
    }

    /// <summary>
    /// ���ز�����Ŀ�곡��
    /// </summary>
    /// <param name="sceneName">Ŀ�곡��</param>
    /// <returns></returns>
    private IEnumerator LoadSceneSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        //Debug.Log(SceneManager.sceneCount); 3 
        SceneManager.SetActiveScene(newScene);
    }

    /// <summary>
    /// ���뵭������
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
            EventHandler.CallAfterLoadSceneEvent(); //��ʵ��unload
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
