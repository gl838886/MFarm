using DG.Tweening;
using Mfarm.dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject textBox; //�Ի���
    public Text dialogueText; //����
    public Image faceLeft, faceRight; //���ҵ�ͼƬ
    public Text nameLeft, nameRight; //���ҵ�����
    public GameObject continueBox; //���ո��������ʾ��

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
            continueBox.SetActive(false); //�ȵ���ʾ��֮����true

            dialogueText.text = string.Empty; //�����ı���ʲô������ʾ

            if (piece.name != string.Empty) //���ֲ�Ϊ��
            {
                if (piece.onLeft) //�������ߵ���˵��
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
                //������Ҳ����
            }
            
            yield return dialogueText.DOText(piece.dialogueText, 1f).WaitForCompletion();

            piece.isDone = true;

            //һ�����ˣ��Ƿ����
            if(piece.isDone && piece.hasToPause)
            {
                continueBox.SetActive(true);
            }
        }
        else
        {
            textBox.SetActive(false);
            yield break; //�˳�Э��
        }
    }
}
