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
                //����panel�Ƶ���ǰ�б������棬��ʵ������Ⱦ�Ļ���������һ��
                panels[i].transform.SetAsLastSibling(); 
            }
        }
    }
}
