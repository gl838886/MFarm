using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject menuCanvas;
    public GameObject menuCanvasPrefab;

    public Button settingButton; //右上角的设置按键
    public GameObject PausePanel; //暂停菜单
    public Button MenuButton; //返回主菜单按键
    public Slider volumeSlider;


    private void Awake()
    {
        //添加监听事件-打开暂停面板
        settingButton.onClick.AddListener(OpenPausePanel);
        //添加监听事件-调整环境音
        volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetSliderVolume);
        //返回菜单结束当前游戏
        //MenuButton.onClick.AddListener(EventHandler.CallEndCurrentGame);
    }

    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");
        Instantiate(menuCanvasPrefab, menuCanvas.transform);
    }

    private void OnEnable()
    {
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
    }

    private void OnAfterLoadSceneEvent()
    {
        DestoryMenuCanvas();
    }

    private void DestoryMenuCanvas()
    {
        if(menuCanvas.transform.childCount > 0)
        {
            Destroy(menuCanvas.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// 打开暂停面板
    /// </summary>
    private void OpenPausePanel()
    {
        //判断暂停面板是否打开
        bool isOpen = PausePanel.activeInHierarchy;
        if(isOpen) //打开
        {
            PausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else //处于关闭状态，我点击希望打开
        {
            PausePanel.SetActive(true);
            Time.timeScale = 0f;
            //在暂停时我希望清空内存
            System.GC.Collect();
        }
    }

    public void ReturnMenuPanel()
    {
        //因为打开暂停菜单时timeScale为0
        //返回主菜单时需要用到协程
        //故需要将timeScale设置为1
        Time.timeScale = 1f;
        StartCoroutine(BackToMenu());
    }

    private IEnumerator BackToMenu()
    {
        PausePanel.SetActive(false);
        EventHandler.CallEndCurrentGame();
        yield return new WaitForSeconds(1f);
        Instantiate(menuCanvasPrefab, menuCanvas.transform);
    }
}
