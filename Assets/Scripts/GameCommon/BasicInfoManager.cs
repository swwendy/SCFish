using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 手机基本信息管理类
/// </summary>
public class BasicInfoManager : MonoBehaviour
{
    //时间
    Text TimeText = null;
    //电池图标
    Image BatteryIcon = null;
    //时间格式(是否显示秒)
    public bool ShowSecond = false;
    // Use this for initialization
    void Start () {
        TimeText = transform.Find("TextTime").GetComponent<Text>();
        BatteryIcon = transform.Find("ImageElectricity/Image").GetComponent<Image>();
        StartCoroutine(UpdataBasicInfoData());
    }

    //更新手机基本信息
    IEnumerator UpdataBasicInfoData()
    {
        while (true)
        {
            float batteryLevel = 0.0f;
#if UNITY_ANDROID && !UNITY_EDITOR
            batteryLevel =  AlipayWeChatPay.GetAndroidActivity().Call<int>("GetBatteryLevel");
            batteryLevel /= 100.0f;
#endif

#if UNITY_IOS  && !UNITY_EDITOR
            batteryLevel = WechatPlatfrom_IOS.GetBatteryLevel();
#endif
            //电池电量
            if (BatteryIcon)
            {
                BatteryIcon.fillAmount = batteryLevel;
            }

            //时间
            if(TimeText)
            {
                TimeText.text = System.DateTime.Now.Hour.ToString("D2")+ ":" 
                                + System.DateTime.Now.Minute.ToString("D2")
                                + (ShowSecond ? (":" + System.DateTime.Now.Second.ToString("D2")) : "");
            }
            yield return new WaitForSeconds(ShowSecond ? 1f : 5f);
        }
    }
}
