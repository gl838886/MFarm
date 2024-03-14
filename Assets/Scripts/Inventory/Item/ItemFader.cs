using DG.Tweening;
using UnityEngine;

//该物体一定要有SpriteRenderer属性
[RequireComponent(typeof(SpriteRenderer))]  

public class ItemFader : MonoBehaviour
{
    //目标：拿到物体的sprite renderer属性，修改alpha（不透明度）

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //逐渐恢复颜色
    public void fadeIn()
    {
        Color targetColor = new Color(1, 1, 1, 1);
        spriteRenderer.DOColor(targetColor, Settings.durationTime);
    }

    public void fadeOut()
    {
        Color targetColor = new Color(1, 1, 1, Settings.targetAlpha);
        spriteRenderer.DOColor(targetColor, Settings.durationTime);
    }
}
