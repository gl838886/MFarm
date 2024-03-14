using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightControl : MonoBehaviour
{
    public LightListPattern_SO lightListPattern_SO;

    public Light2D currentLight2D;
    public LightDetails currentLightDetails;

    private void Awake()
    {
        currentLight2D= GetComponent<Light2D>();
    }

    /// <summary>
    /// 切换灯光效果
    /// </summary>
    /// <param name="season">季节</param>
    /// <param name="lightShift">早晚</param>
    /// <param name="timeDifference">时间差</param>
    public void ChangeLightShift(Season season, LightShift lightShift, float timeDifference)
    {
        currentLightDetails = lightListPattern_SO.GetLightDetails(season, lightShift);

        //还没有完全变亮或变暗
        if(timeDifference <Settings.lightChangeDuration)
        {
            //timeDifference在duration中的占比*颜色差值
            var colorOffset = (currentLightDetails.lightColor - currentLight2D.color) / Settings.lightChangeDuration * timeDifference;
            currentLight2D.color += colorOffset;
            DOTween.To(() => currentLight2D.color, c => currentLight2D.color = c, currentLightDetails.lightColor, Settings.lightChangeDuration - timeDifference);
            DOTween.To(() => currentLight2D.intensity, i => currentLight2D.intensity = i, currentLightDetails.lightStrength, Settings.lightChangeDuration - timeDifference);
        }
        if(timeDifference >=Settings.lightChangeDuration)
        {
            currentLight2D.color = currentLightDetails.lightColor;
            currentLight2D.intensity = currentLightDetails.lightStrength;
        }
    }
}
