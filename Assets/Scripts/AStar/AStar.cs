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
        private GridNodes gridNodes; //ÿ��������һ��

        private Node startNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX; //���½ǵ�ԭ��
        private int originY;

        private List<Node> openNodeList; //�����洢��ǰ�����Χ�˸������Ϣ

        //HashSet��List������HashSet�ڲ���ʱ�ٶȸ���
        private HashSet<Node> closedNodeList; //��ѡ������е�

        private bool isPathFound; 


        /// <summary>
        /// �������·��
        /// </summary>
        /// <param name="sceneName">������</param>
        /// <param name="startPos">��ʼ����</param>
        /// <param name="endPos">��������</param>
        /// <param name="npcMovementStep">ջ</param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int endPos, Stack<MovementStep> npcMovementStep)
        {
            isPathFound= false;
            //����������������飬��ô��ʼ�����·��
            if(GenerateGridNodes(sceneName, startPos, endPos))
            {
                //�ҵ����·��
                if (FindShortestPath())
                {
                    //�������·��
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStep);
                }
            }
        }

        /// <summary>
        /// ��������ڵ���Ϣ
        /// </summary>
        /// <param name="sceneName">������</param>
        /// <param name="startPos">�������</param>
        /// <param name="endPos">�յ�����</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int endPos)
        {
            //�����mapdata���ҵ��ó����ĳ���������ô
            if (GridMapManager.Instance.GetGridDimensions(sceneName, out Vector2Int gridDemensions, out Vector2Int gridOriginNode))
            {
                gridNodes = new GridNodes(gridDemensions.x, gridDemensions.y);
                gridWidth = gridDemensions.x;
                gridHeight = gridDemensions.y;
                originX = gridOriginNode.x;
                originY = gridOriginNode.y;
                //��ʼ�����б�
                openNodeList = new List<Node>();
                closedNodeList = new HashSet<Node>();
            }
            else
                return false;

            //������������ʼ�� ��������Ҫ����������ȫ����Ϊ��(0,0)��ʼ
            //��������startPos���п����Ǹ���
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
            openNodeList.Add(startNode); //������ʼ��

            while(openNodeList.Count > 0)
            {
                openNodeList.Sort(); //��Ϊ�ҵ�Node�����Ѿ����˱Ƚϵĺ���

                //ȡ��һ��
                Node closedNode = openNodeList[0];
                //�Ƴ���һ��
                openNodeList.RemoveAt(0);
                //���ó����ļ��뵽�ҵ���ѡ�б���
                closedNodeList.Add(closedNode);
                //����ҵĵ�ǰѭ����closedNodeΪĿ��ڵ㣬��ô
                if(closedNode == targetNode)
                {
                    isPathFound= true;
                    break;
                }
                //����Ū��Χ8����
                EvaluateNeighbourNodes(closedNode);
            }
            return isPathFound;
        }

        /// <summary>
        /// ������ǰ�ߵ��ĵ���Χ�İ˸���
        /// </summary>
        /// <param name="currentNode">��ǰ�ڵ�</param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPos;
            Node validNode; //��ʱ�����������洢��Χ���������ĵ㣨��������ͼ�߽磬û���ϰ��
            
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
        /// ������Χ�Ŀ��õ㣨��������ͼ�߽磬û���ϰ��
        /// </summary>
        /// <param name="xPos">x����</param>
        /// <param name="yPos">y����</param>
        /// <returns></returns>
        private Node GetValidNode(int xPos, int yPos)
        {
            //�����߽�(�߽�һ�����ĸ���
            if(xPos>=gridWidth||yPos>=gridHeight||xPos<0||yPos<0) return null;
            
            Node validNode = gridNodes.getGridNode(xPos,yPos);
          
            //�ϰ�����ѡ
            if (validNode.isObstacle || closedNodeList.Contains(validNode)) return null;
            else return validNode;
        }

        /// <summary>
        /// �������ڵ�����̾���
        /// </summary>
        /// <param name="nodeA">�ڵ�A</param>
        /// <param name="nodeB">�ڵ�B</param>
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
        /// ����NPC��ÿһ��
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="stack"></param>
        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> stack)
        {
            //ջ�������������һ���ڵ��������ѹջ
            Node nextNode = targetNode;
            while(nextNode != null) //startNodeû�и��ڵ㣬���Ի�����ѭ��
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                //�������������ʵ���꣬���и���
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPos.x+originX, nextNode.gridPos.y+originY);
                stack.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}
