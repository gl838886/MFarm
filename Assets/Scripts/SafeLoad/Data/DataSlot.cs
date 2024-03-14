using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.Save
{
    public class DataSlot
    {
        //key��GUID
        public Dictionary<string, GameSaveData> slotDataDict= new Dictionary<string, GameSaveData>();

        public string slotDate
        {
            get
            {
                var key= TimeManager.Instance.GUID;
                if(slotDataDict.ContainsKey(key))
                {
                    string date = slotDataDict[key].timeDict["gameYear"].ToString() + "��/" +
                        (Season) slotDataDict[key].timeDict["seasonNum"] + "/" +
                        slotDataDict[key].timeDict["gameMonth"].ToString() + "��/" +
                        slotDataDict[key].timeDict["gameDay"].ToString() + "��";
                    return date;
                }
                return string.Empty;
            }
        }

        public string slotScene
        {
            get
            {
                var key = TransitionManager.Instance.GUID;
                if(slotDataDict.ContainsKey(key))
                {
                    string sceneName = slotDataDict[key].sceneName;
                    return sceneName switch
                    {
                        "00_Start" => "����",
                        "01_Field" => "ũ��",
                        "02_House" => "Сľ��",
                        "03_Stall" => "�㳡",
                        _ => string.Empty,
                    };
                }
                return string.Empty;
            }
        }
    }
}
