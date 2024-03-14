using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.AStar
{
    public class GridNodes : MonoBehaviour
    {
        public int width;
        public int height;

        private Node[,] gridNode; //二维数组，每个xy存一个node

        /// <summary>
        /// 构造函数初始化节点范围
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
        /// 获取二维数组中的节点
        /// </summary>
        /// <param name="xPos">x坐标</param>
        /// <param name="yPos">y坐标</param>
        /// <returns></returns>
        public Node getGridNode(int xPos, int yPos)
        {
            if (xPos < width && yPos < height)
            {
                return gridNode[xPos, yPos];
            }
            Debug.Log("超出界面范围");
            return null;
        }
    }
}
