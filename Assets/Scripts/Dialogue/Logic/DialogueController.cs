using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Mfarm.dialogue
{
    //需要有NPC和collider
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(Collider2D))]
    public class DialogueController : MonoBehaviour
    {
        public NPCMovement npc => GetComponent<NPCMovement>();
        public List<DialoguePiece> dialoguePieceList = new List<DialoguePiece>();
        //把list里的内容压入stack，播放时直接循环
        private Stack<DialoguePiece> dialogueStack;

        public UnityEvent onFinishEvent;
        private GameObject UIsign;

        private bool canTalk; //npc是否可以对话
        private bool isTalking; //辅助协程判断
       

        private void Awake()
        {
            FillDialogueStack();
            UIsign = transform.GetChild(1).gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player")) //是不是玩家碰的
            {
                if(!npc.isMoving && npc.isInteractable)
                    canTalk = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) //是不是玩家碰的
            {
                canTalk = false;
            }
        }

        private void Update()
        {
            UIsign.SetActive(canTalk);
            if(canTalk && Input.GetKeyDown(KeyCode.Space) && !isTalking) //按空间键进行下一个对话
            {
                //Debug.Log("按空格键");
                StartCoroutine(DialogueRoutinue());
            }
        }

        /// <summary>
        /// 将List里的对话片段压入Stack中
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
                FillDialogueStack(); //stack里的全被pop掉了
                isTalking = false;
                //调用结束事件
                if(onFinishEvent!=null)
                {
                    onFinishEvent.Invoke();
                    isTalking = true;
                }
            }
        }
    }
}