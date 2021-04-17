

#if UNITY_IOS
using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;

/// <summary>
/// IOS下各支付平台功能接口
/// </summary>
public class WechatPlatfrom_IOS
{
    //IOS支付宝jiezhang接口
    //[DllImport("__Internal")]
    //public static extern void MayunJieZhang_IOS(string paystr);
    public static void MayunJieZhang_IOS(string paystr)
    {

    }

     //微信分享接口URL
    [DllImport("__Internal")]
    public static extern void ShowAppUpdateTips(string AppStoreUrl);

    //微信IOS支付接口
    [DllImport("__Internal")]
    private static extern void WeChatLgoinJieZhang_IOS(string prepayId,string noncestr,string timestr,string signstr);

    //微信是否安装
    [DllImport("__Internal")]
    public static extern bool WeChat_IsWXAppInstalled();

    //微信登陆授权接口
    [DllImport("__Internal")]
    public static extern void  WeChat_AuthLogin();

    //微信分享接口图片
    [DllImport("__Internal")]
    public static extern void WeChat_ShareImage(bool isTimeline);

    //微信分享接口图片
    [DllImport("__Internal")]
    public static extern void WeChat_ShareImageByPath(string ImgPath,bool isTimeline);

    //微信分享接口文本
    [DllImport("__Internal")]
    public static extern void WeChat_ShareText(string sharetxt,bool isTimeline);
   
    //微信分享接口URL
    [DllImport("__Internal")]
    public static extern void WeChat_ShareURL(string URLtxt,string descpxt,bool isTimeline);

    //手机电池电量
    [DllImport("__Internal")]
    public static extern float GetBatteryLevel();

    public static string MD5Encrypt(string input)    {        MD5 md5 = new MD5CryptoServiceProvider();        byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(input));        StringBuilder sb = new StringBuilder(32);        for (int i = 0; i < t.Length; i++)            sb.Append(t[i].ToString("x").PadLeft(2, '0'));        return sb.ToString();    }    /// <summary>    /// 生成微信支付签名    /// </summary>    /// <param name="paras"></param>    /// <returns></returns>    private static string GenerateSignString(ref Dictionary<string, string> paramsMap)    {
        //Dictionary<string, string> sortDic = paramsMap.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);

        var sortDic = (from objDic in paramsMap orderby objDic.Key ascending select objDic);
        List<string> tmp = new List<string>();        foreach (var kv in sortDic)        {            tmp.Add(string.Format("{0}={1}", kv.Key, kv.Value));        }        tmp.Add("key=" + PayPlatformDefine.WxApiKey);        //string[] sortKeyArray = tmp.ToArray();        var signTmp = string.Join("&", tmp.ToArray());        var result = MD5Encrypt(signTmp).ToUpper();        return result;    }

    /// <summary>    /// 请求微信支付    /// </summary>
    public static void WXLgoinJieZhang_IOS(string prepayId,string noncestr)
    {
        Dictionary<string, string> paras = new Dictionary<string, string>();        paras.Add("appid", PayPlatformDefine.WXAPPID);
        paras.Add("noncestr", noncestr); //随机字符串
        paras.Add("partnerid", PayPlatformDefine.WXMCH_ID); //商户号        paras.Add("prepayid", prepayId); //预支付订单ID 这个是后台跟微信服务器交互后，微信服务器传给支付服务器，服务器传给客户端        paras.Add("package", "Sign=WXPay"); //暂填写固定值Sign=WXPay             Int32 timestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;        paras.Add("timestamp", timestamp.ToString());

        string signStr = GenerateSignString(ref paras);//应用签名

        WeChatLgoinJieZhang_IOS(prepayId,noncestr,timestamp.ToString(),signStr);
    }

    private static IEnumerator CaptureScreenRectShot(Canvas cans, RectTransform rect,bool bTimeLine)
    {

        Rect targetRect = GameFunction.GetSpaceRect(cans, rect, cans.worldCamera);

        Int32 timestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;        string screenshotname = GameDefine.ScreenshotSavePath + timestamp.ToString() + ".png";
        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D((int)targetRect.width, (int)targetRect.height, TextureFormat.RGBA32, false);

        tex.ReadPixels(targetRect, 0, 0);
        tex.Apply();
        File.WriteAllBytes(screenshotname, tex.EncodeToPNG());

        while (!File.Exists(screenshotname))        {
            yield return new WaitForSeconds(0.05f);        }
        
        WeChat_ShareImageByPath(screenshotname,bTimeLine);
    } 


   //微信分享屏幕区域截图图片(true朋友圈，false好友)
    public static bool WeChat_ShareRectImage(Canvas cans,RectTransform rect,bool bTimeLine)    {
        GameMain.SC(CaptureScreenRectShot(cans,rect,bTimeLine));
        return true;    }
}

#endif