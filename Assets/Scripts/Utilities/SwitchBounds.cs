using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    //改变场景时调用
    //private void Awake()
    //{
    //    switchConfinerShape();
    //}

    private void OnEnable()
    {
        EventHandler.AfterLoadSceneEvent += switchConfinerShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterLoadSceneEvent -= switchConfinerShape;
    }
    private void switchConfinerShape()
    {
        PolygonCollider2D confinerShape=GameObject.FindGameObjectWithTag("ConfinerBounds").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D= confinerShape;

        //Call this if the bounding shape's points change at runtime
        //清除缓存
        confiner.InvalidatePathCache();
    }
}
