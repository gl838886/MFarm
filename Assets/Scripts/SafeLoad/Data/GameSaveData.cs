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
        /// 玩家位置
        /// </summary>
        public Dictionary<string, SerializableVector3> characterPosDict;
        /// <summary>
        /// 场景内物品
        /// </summary>
        public Dictionary<string, List<SceneItem>> sceneItemDict;
        /// <summary>
        /// 场景内建造的物品
        /// </summary>
        public Dictionary<string, List<FurnitureItem>> furnitureItemDict;
        /// <summary>
        /// 场景内瓦片信息
        /// </summary>
        public Dictionary<string, TileDetails> tileDetailsDict;
        /// <summary>
        /// 是否是原始物品
        /// </summary>
        public Dictionary<string, bool> firstGenerateDict;
        /// <summary>
        /// 箱子和背包
        /// </summary>
        public Dictionary<string, List<BagItem>> boxItemDict;
        /// <summary>
        /// 时间
        /// </summary>
        public Dictionary<string, int> timeDict;

        public int playerMoney;

        //NPC
        public string targetScene;
        public bool interactable;
        public int animationInstanceID;
    }
}
