using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScheduleDataList_SO", menuName = "Schedule/ScheduleDataList_SO")]
public class ScheduleDataList_SO : ScriptableObject
{
   public List<ScheduleDetails> scheduleDetailsList;
}
