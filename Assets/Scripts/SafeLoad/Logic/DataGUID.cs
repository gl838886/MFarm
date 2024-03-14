using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DataGUID : MonoBehaviour
{
    public string GUID;

    public void Awake()
    {
        if(GUID == string.Empty)
        {
            GUID = System.Guid.NewGuid().ToString();
        }
    }
}
