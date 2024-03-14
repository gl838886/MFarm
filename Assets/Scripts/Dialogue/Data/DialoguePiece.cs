using UnityEngine;


namespace Mfarm.dialogue
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("�Ի�����")]
        public Sprite faceImage;
        public bool onLeft;
        public string name;

        [TextArea]
        public string dialogueText;
        public bool hasToPause;
        public bool isDone; //�Ի��Ƿ��Ѿ�����
    }
}

