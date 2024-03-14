using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.Save
{
    [System.Serializable]
    public class GameSaveData
    {
        public string sceneName;
        /// <summary>
        /// ���λ��
        /// </summary>
        public Dictionary<string, SerializableVector3> characterPosDict;
        /// <summary>
        /// ��������Ʒ
        /// </summary>
        public Dictionary<string, List<SceneItem>> sceneItemDict;
        /// <summary>
        /// �����ڽ������Ʒ
        /// </summary>
        public Dictionary<string, List<FurnitureItem>> furnitureItemDict;
        /// <summary>
        /// ��������Ƭ��Ϣ
        /// </summary>
        public Dictionary<string, TileDetails> tileDetailsDict;
        /// <summary>
        /// �Ƿ���ԭʼ��Ʒ
        /// </summary>
        public Dictionary<string, bool> firstGenerateDict;
        /// <summary>
        /// ���Ӻͱ���
        /// </summary>
        public Dictionary<string, List<BagItem>> boxItemDict;
        /// <summary>
        /// ʱ��
        /// </summary>
        public Dictionary<string, int> timeDict;

        public int playerMoney;

        //NPC
        public string targetScene;
        public bool interactable;
        public int animationInstanceID;
    }
}
