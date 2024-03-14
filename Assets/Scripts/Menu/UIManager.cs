using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject menuCanvas;
    public GameObject menuCanvasPrefab;

    public Button settingButton; //���Ͻǵ����ð���
    public GameObject PausePanel; //��ͣ�˵�
    public Button MenuButton; //�������˵�����
    public Slider volumeSlider;


    private void Awake()
    {
        //��Ӽ����¼�-����ͣ���
        settingButton.onClick.AddListener(OpenPausePanel);
        //��Ӽ����¼�-����������
        volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetSliderVolume);
        //���ز˵�������ǰ��Ϸ
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
    /// ����ͣ���
    /// </summary>
    private void OpenPausePanel()
    {
        //�ж���ͣ����Ƿ��
        bool isOpen = PausePanel.activeInHierarchy;
        if(isOpen) //��
        {
            PausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else //���ڹر�״̬���ҵ��ϣ����
        {
            PausePanel.SetActive(true);
            Time.timeScale = 0f;
            //����ͣʱ��ϣ������ڴ�
            System.GC.Collect();
        }
    }

    public void ReturnMenuPanel()
    {
        //��Ϊ����ͣ�˵�ʱtimeScaleΪ0
        //�������˵�ʱ��Ҫ�õ�Э��
        //����Ҫ��timeScale����Ϊ1
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
