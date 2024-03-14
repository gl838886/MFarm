using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject[] panels;

    public void SwitchPanels(int index)
    {
        for(int i=0;i<panels.Length;i++)
        {
            if(i == index)
            {
                //将该panel移到当前列表最下面，但实际上渲染的话是最上面一层
                panels[i].transform.SetAsLastSibling(); 
            }
        }
    }
}
