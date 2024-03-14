using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mfarm.dialogue
{
    //��Ҫ��NPC��collider
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(Collider2D))]
    public class DialogueController : MonoBehaviour
    {
        public NPCMovement npc => GetComponent<NPCMovement>();
        public List<DialoguePiece> dialoguePieceList = new List<DialoguePiece>();
        //��list�������ѹ��stack������ʱֱ��ѭ��
        private Stack<DialoguePiece> dialogueStack;

        public UnityEvent onFinishEvent;
        private GameObject UIsign;

        private bool canTalk; //npc�Ƿ���ԶԻ�
        private bool isTalking; //����Э���ж�
       

        private void Awake()
        {
            FillDialogueStack();
            UIsign = transform.GetChild(1).gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player")) //�ǲ����������
            {
                if(!npc.isMoving && npc.isInteractable)
                    canTalk = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) //�ǲ����������
            {
                canTalk = false;
            }
        }

        private void Update()
        {
            UIsign.SetActive(canTalk);
            if(canTalk && Input.GetKeyDown(KeyCode.Space) && !isTalking) //���ռ��������һ���Ի�
            {
                //Debug.Log("���ո��");
                StartCoroutine(DialogueRoutinue());
            }
        }

        /// <summary>
        /// ��List��ĶԻ�Ƭ��ѹ��Stack��
        /// </summary>
        private void FillDialogueStack()
        {
            dialogueStack= new Stack<DialoguePiece>();
            for(int i=dialoguePieceList.Count - 1; i>=0; i--)
            {
                dialoguePieceList[i].isDone= false;
                dialogueStack.Push(dialoguePieceList[i]);
            }
        }

        private IEnumerator DialogueRoutinue()
        {
            isTalking= true;
            if(dialogueStack.TryPop(out DialoguePiece piece))
            {
                EventHandler.CallUpdateGameStateEvent(GameState.GamePause);
                EventHandler.CallShowDialogueEvent(piece);
                yield return new WaitUntil(() => piece.isDone);
                isTalking= false;
            }
            else
            {
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
                EventHandler.CallShowDialogueEvent(null);
                FillDialogueStack(); //stack���ȫ��pop����
                isTalking = false;
                //���ý����¼�
                if(onFinishEvent!=null)
                {
                    onFinishEvent.Invoke();
                    isTalking = true;
                }
            }
        }
    }
}