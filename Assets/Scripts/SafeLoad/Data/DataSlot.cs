using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.Save
{
    public class DataSlot
    {
        //key是GUID
        public Dictionary<string, GameSaveData> slotDataDict= new Dictionary<string, GameSaveData>();

        public string slotDate
        {
            get
            {
                var key= TimeManager.Instance.GUID;
                if(slotDataDict.ContainsKey(key))
                {
                    string date = slotDataDict[key].timeDict["gameYear"].ToString() + "年/" +
                        (Season) slotDataDict[key].timeDict["seasonNum"] + "/" +
                        slotDataDict[key].timeDict["gameMonth"].ToString() + "月/" +
                        slotDataDict[key].timeDict["gameDay"].ToString() + "天";
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
                        "00_Start" => "海边",
                        "01_Field" => "农场",
                        "02_House" => "小木屋",
                        "03_Stall" => "广场",
                        _ => string.Empty,
                    };
                }
                return string.Empty;
            }
        }
    }
}
