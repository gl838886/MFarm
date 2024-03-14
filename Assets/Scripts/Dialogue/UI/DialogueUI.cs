using DG.Tweening;
using Mfarm.dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject textBox; //对话框
    public Text dialogueText; //文字
    public Image faceLeft, faceRight; //左右的图片
    public Text nameLeft, nameRight; //左右的名字
    public GameObject continueBox; //按空格继续的提示框

    private void Awake()
    {
        continueBox.SetActive(false);
    }

    private void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
    }

    private void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
    }

    private void OnShowDialogueEvent(DialoguePiece piece)
    {
         StartCoroutine(ShowDialogue(piece));
    }

    private IEnumerator ShowDialogue(DialoguePiece piece)
    {
        if(piece != null )
        {
            piece.isDone = false;

            textBox.SetActive(true);
            continueBox.SetActive(false); //等到显示完之后再true

            dialogueText.text = string.Empty; //先让文本框什么都不显示

            if (piece.name != string.Empty) //名字不为空
            {
                if (piece.onLeft) //如果是左边的在说话
                {
                    faceRight.gameObject.SetActive(false);
                    faceLeft.gameObject.SetActive(true);
                    faceLeft.sprite = piece.faceImage;
                    nameLeft.text = piece.name;
                }
                else
                {
                    faceRight.gameObject.SetActive(true);
                    faceLeft.gameObject.SetActive(false);
                    faceRight.sprite = piece.faceImage;
                    nameRight.text = piece.name;
                }
            }
            else
            {
                faceRight.gameObject.SetActive(false);
                faceLeft.gameObject.SetActive(false);
                //子物体也关了
            }
            
            yield return dialogueText.DOText(piece.dialogueText, 1f).WaitForCompletion();

            piece.isDone = true;

            //一条完了，是否继续
            if(piece.isDone && piece.hasToPause)
            {
                continueBox.SetActive(true);
            }
        }
        else
        {
            textBox.SetActive(false);
            yield break; //退出协程
        }
    }
}
