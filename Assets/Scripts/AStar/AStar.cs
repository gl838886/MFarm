using JetBrains.Annotations;
using Mfarm.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Mfarm.AStar
{
    public class AStar : Singleton<AStar>
    {
        private GridNodes gridNodes; //每个场景存一个

        private Node startNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX; //左下角的原点
        private int originY;

        private List<Node> openNodeList; //用来存储当前点的周围八个点的信息

        //HashSet和List的区别，HashSet在查找时速度更快
        private HashSet<Node> closedNodeList; //已选择的所有点

        private bool isPathFound; 


        /// <summary>
        /// 构建最短路径
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="startPos">起始坐标</param>
        /// <param name="endPos">结束坐标</param>
        /// <param name="npcMovementStep">栈</param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos, Stack<MovementStep> npcMovementStep)
        {
            isPathFound= false;
            //如果生成了网格数组，那么开始找最短路径
            if(GenerateGridNodes(sceneName, startPos, endPos))
            {
                //找到最短路径
                if (FindShortestPath())
                {
                    //构建最短路径
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStep);
                }
            }
        }

        /// <summary>
        /// 构建网格节点信息
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="startPos">起点坐标</param>
        /// <param name="endPos">终点坐标</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            //如果在mapdata中找到该场景的场景名，那么
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDemensions, out Vector2Int gridOriginNode))
            {
                gridNodes = new GridNodes(gridDemensions.x, gridDemensions.y);
                gridWidth = gridDemensions.x;
                gridHeight = gridDemensions.y;
                originX = gridOriginNode.x;
                originY = gridOriginNode.y;
                //初始化两列表
                openNodeList = new List<Node>();
                closedNodeList = new HashSet<Node>();
            }
            else
                return false;

            //计算真正的起始点 数组中需要正数，所以全部改为从(0,0)开始
            //传进来的startPos很有可能是负数
            startNode = gridNodes.getGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.getGridNode(endPos.x - originX, endPos.y - originY);

            for(int x=0; x<gridWidth;x++)
            {
                for(int y=0;y<gridHeight;y++)
                {
                    string key = (x + originX) + "x" + (y + originY) + "y" + sceneName;
                    TileDetails tile = GridMapManager.Instance.GetTileDetails(key);
                    if(tile != null)
                    {
                        //Debug.Log("yes");
                        Node node = gridNodes.getGridNode(x, y);
                        //Debug.Log("no");
                        if (tile.isNpcObstacle)
                            node.isObstacle = true;
                    }
                }
            }
            return true;
        }

        private bool FindShortestPath()
        {
            openNodeList.Add(startNode); //加入起始点

            while(openNodeList.Count > 0)
            {
                openNodeList.Sort(); //因为我的Node类里已经做了比较的函数

                //取第一个
                Node closedNode = openNodeList[0];
                //移除第一个
                openNodeList.RemoveAt(0);
                //把拿出来的加入到我的已选列表当中
                closedNodeList.Add(closedNode);
                //如果我的当前循环的closedNode为目标节点，那么
                if(closedNode == targetNode)
                {
                    isPathFound= true;
                    break;
                }
                //接着弄周围8个点
                EvaluateNeighbourNodes(closedNode);
            }
            return isPathFound;
        }

        /// <summary>
        /// 评估当前走到的点周围的八个点
        /// </summary>
        /// <param name="currentNode">当前节点</param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPos;
            Node validNode; //临时变量，用来存储周围满足条件的点（即不超地图边界，没有障碍物）
            
            for(int x=-1;x<=1;x++)
            {
                for(int y=-1;y<=1;y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    validNode = GetValidNode(currentNodePos.x+x, currentNodePos.y+y);
                    if(validNode != null)
                    {
                        //if(!openNodeList.Contains(validNode))
                        //{
                            validNode.gCost = currentNode.gCost + GetDistanceBetweenNodes(currentNode, validNode);
                            validNode.hCost = GetDistanceBetweenNodes(targetNode, validNode);
                            validNode.parentNode = currentNode;
                            openNodeList.Add(validNode);
                        //}
                    }
                }
            }
        }

        /// <summary>
        /// 返回周围的可用点（即不超地图边界，没有障碍物）
        /// </summary>
        /// <param name="xPos">x坐标</param>
        /// <param name="yPos">y坐标</param>
        /// <returns></returns>
        private Node GetValidNode(int xPos, int yPos)
        {
            //超出边界(边界一定是四个）
            if(xPos>=gridWidth||yPos>=gridHeight||xPos<0||yPos<0) return null;
            
            Node validNode = gridNodes.getGridNode(xPos,yPos);
          
            //障碍或已选
            if (validNode.isObstacle || closedNodeList.Contains(validNode)) return null;
            else return validNode;
        }

        /// <summary>
        /// 计算两节点间的最短距离
        /// </summary>
        /// <param name="nodeA">节点A</param>
        /// <param name="nodeB">节点B</param>
        /// <returns></returns>
        private int GetDistanceBetweenNodes(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPos.x-nodeB.gridPos.x);
            int yDistance = Mathf.Abs(nodeA.gridPos.y-nodeB.gridPos.y);

            if (xDistance > yDistance)
            {
                return (xDistance - yDistance) * 10 + yDistance * 14;
            }
            else
                return xDistance * 14 + (yDistance - xDistance) * 10;
        }

        /// <summary>
        /// 更新NPC的每一步
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="stack"></param>
        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> stack)
        {
            //栈先入后出，从最后一个节点往后进行压栈
            Node nextNode = targetNode;
            while(nextNode != null) //startNode没有父节点，所以会跳出循环
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                //这里的坐标是真实坐标，即有负数
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPos.x+originX, nextNode.gridPos.y+originY);
                stack.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}
