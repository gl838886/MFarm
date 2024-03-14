using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPos;
        public int gCost = 0; //离起点最近的点
        public int hCost = 0; //离终点最近的点
        public int fCost => gCost + hCost; //当前点的权值，即fCost=gCost+hCost

        public bool isObstacle = false; //当前格子是否是障碍物
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPos = pos;
            parentNode = null;
        }

        public int CompareTo(Node other) //IComparable的成员函数
        {
            int result = fCost.CompareTo(other.fCost);
            //fCost可能相等，相等比较hCost
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}
