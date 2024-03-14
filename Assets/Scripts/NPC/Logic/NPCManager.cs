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
            //��npc��λ�úͳ�����ʼ��
            character.npcTransform.position = character.startPosition;
            character.npcTransform.GetComponent<NPCMovement>().currentScene = character.startScene;
        }
    }

    /// <summary>
    /// ��ʼ���ֵ�
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
    /// ��ȡ·����Ϣ
    /// </summary>
    /// <param name="fromScene">��ʼ������</param>
    /// <param name="goToScene">Ŀ�ĳ�����</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromScene, string goToScene)
    {
        return sceneRouteDict[fromScene + goToScene];
    }
}
