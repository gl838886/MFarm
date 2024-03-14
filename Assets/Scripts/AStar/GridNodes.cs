using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.AStar
{
    public class GridNodes : MonoBehaviour
    {
        public int width;
        public int height;

        private Node[,] gridNode; //��ά���飬ÿ��xy��һ��node

        /// <summary>
        /// ���캯����ʼ���ڵ㷶Χ
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public GridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;
            gridNode = new Node[width, height];
            for(int x=0;x<width; x++)
            {
                for(int y=0;y<height;y++)
                {
                    gridNode[x, y] = new Node(new Vector2Int(x, y));
                }
            }
        }

        /// <summary>
        /// ��ȡ��ά�����еĽڵ�
        /// </summary>
        /// <param name="xPos">x����</param>
        /// <param name="yPos">y����</param>
        /// <returns></returns>
        public Node getGridNode(int xPos, int yPos)
        {
            if (xPos < width && yPos < height)
            {
                return gridNode[xPos, yPos];
            }
            Debug.Log("�������淶Χ");
            return null;
        }
    }
}
