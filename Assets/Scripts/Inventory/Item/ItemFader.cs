using DG.Tweening;
using UnityEngine;

//������һ��Ҫ��SpriteRenderer����
[RequireComponent(typeof(SpriteRenderer))]  

public class ItemFader : MonoBehaviour
{
    //Ŀ�꣺�õ������sprite renderer���ԣ��޸�alpha����͸���ȣ�

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //�𽥻ָ���ɫ
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
