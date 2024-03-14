using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteDataList_SO;
    public List<NPCPosition> npcPostionList= new List<NPCPosition>();
    public Dictionary<string , SceneRoute> sceneRouteDict= new Dictionary<string , SceneRoute>();

    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }

    private void OnEnable()
    {
        EventHandler.StartNewGame += OnStartNewGame;
    }

    private void OnDisable()
    {
        EventHandler.StartNewGame -= OnStartNewGame;
    }

    private void OnStartNewGame(int index)
    {
        foreach(var character in npcPostionList)
        {
            //将npc的位置和场景初始化
            character.npcTransform.position = character.startPosition;
            character.npcTransform.GetComponent<NPCMovement>().currentScene = character.startScene;
        }
    }

    /// <summary>
    /// 初始化字典
    /// </summary>
    private void InitSceneRouteDict()
    {
        if(sceneRouteDataList_SO.sceneRouteList.Count > 0)
        {
            foreach(var sceneRoute in sceneRouteDataList_SO.sceneRouteList)
            {
                string name = sceneRoute.fromScene + sceneRoute.goToScene;
                if (sceneRouteDict.ContainsKey(name))
                    continue;
                else
                {
                    sceneRouteDict.Add(name, sceneRoute);
                }
            }
        }
    }

    /// <summary>
    /// 获取路径信息
    /// </summary>
    /// <param name="fromScene">起始场景名</param>
    /// <param name="goToScene">目的场景名</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromScene, string goToScene)
    {
        return sceneRouteDict[fromScene + goToScene];
    }
}
