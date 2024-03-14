using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPos;
        public int gCost = 0; //���������ĵ�
        public int hCost = 0; //���յ�����ĵ�
        public int fCost => gCost + hCost; //��ǰ���Ȩֵ����fCost=gCost+hCost

        public bool isObstacle = false; //��ǰ�����Ƿ����ϰ���
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPos = pos;
            parentNode = null;
        }

        public int CompareTo(Node other) //IComparable�ĳ�Ա����
        {
            int result = fCost.CompareTo(other.fCost);
            //fCost������ȣ���ȱȽ�hCost
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}
