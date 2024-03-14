using UnityEngine;


namespace Mfarm.dialogue
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("对话详情")]
        public Sprite faceImage;
        public bool onLeft;
        public string name;

        [TextArea]
        public string dialogueText;
        public bool hasToPause;
        public bool isDone; //对话是否已经结束
    }
}

