using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Mfarm.AStar
{
    public class AStarTest : MonoBehaviour
    {

        private AStar aStar;
        [Header("用于测试")]
        public Vector2Int startPos;
        public Vector2Int endPos;
        public Tilemap displayMap;
        public TileBase displayTile;
        public bool displayStartAndEnd;
        public bool displayPath;

        private Stack<MovementStep> npcMovementStepStack;

        [Header("用于NPC测试")]
        public NPCMovement npcMovement;
        public bool moveNPC;
        [SceneName] public string targetScene;
        public Vector2Int targetPos;
        public AnimationClip stopClip;

        void Awake()
        {
            aStar = GetComponent<AStar>();
            npcMovementStepStack= new Stack<MovementStep>();
        }

         
        void Update()
        {
            ShowPathOnGridMap();
            if(moveNPC)
            {
                moveNPC= false;
                var schedule = new ScheduleDetails(0, 0, 0, 0, Season.春天, targetScene, targetPos, stopClip, true);
                npcMovement.BuildPath(schedule); //开始建路
            }
        }

        private void ShowPathOnGridMap()
        {
            if(displayMap!= null &&displayTile !=null)
            {
                if(displayStartAndEnd)
                {
                    displayMap.SetTile((Vector3Int)startPos, displayTile);
                    displayMap.SetTile((Vector3Int)endPos, displayTile);
                }
                else
                {
                    displayMap.SetTile((Vector3Int)startPos, null);
                    displayMap.SetTile((Vector3Int)endPos, null);
                }
                if (displayPath)
                {
                    string sceneName =SceneManager.GetActiveScene().name;
                    aStar.BuildPath(sceneName, startPos, endPos, npcMovementStepStack);

                    foreach(MovementStep step in npcMovementStepStack)
                    {
                        displayMap.SetTile((Vector3Int)step.gridCoordinate, displayTile);
                    }
                }
                else
                {
                    if(npcMovementStepStack.Count>0)
                    {
                        foreach (MovementStep step in npcMovementStepStack)
                        {
                            displayMap.SetTile((Vector3Int)step.gridCoordinate, null);
                        }
                    }
                }
            }
            

            
        }
    }
}

