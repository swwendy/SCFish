﻿#if UNITY_ANDROID

using System;
/// <summary>

    
#if !UNITY_EDITOR
#endif


    private static IEnumerator CaptureScreenRectShot(Canvas cans, RectTransform rect,bool bTimeLine)
    {

        Rect targetRect = GameFunction.GetSpaceRect(cans, rect, cans.worldCamera);

        Int32 timestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D((int)targetRect.width, (int)targetRect.height, TextureFormat.RGBA32, false);

        tex.ReadPixels(targetRect, 0, 0);
        tex.Apply();
        File.WriteAllBytes(screenshotname, tex.EncodeToPNG());

        while (!File.Exists(screenshotname))
            yield return new WaitForSeconds(0.05f);
        Debug.Log("ShareImageRectToWechat:" + screenshotname);
        PayApiActivity.Call("WeChat_ShareImage", screenshotname, bTimeLine);
        
    }

    //微信分享屏幕区域截图图片(true朋友圈，false好友)
    public static bool WeChat_ShareRectImage(Canvas cans,RectTransform rect,bool bTimeLine)
        GameMain.SC(CaptureScreenRectShot(cans,rect,bTimeLine));
        return true;

    //微信分享文本(true朋友圈，false好友)
    public static bool WeChat_ShareText(string sharetxt,bool bTimeLine)
            InitPayPlatformApi();
        PayApiActivity.Call("WeChat_ShareText", sharetxt, bTimeLine);
        return true;
    public static bool WeChat_ShareURL(string urlTxt,string desctxt, bool bTimeLine)
            InitPayPlatformApi();
        PayApiActivity.Call("WeChat_ShareURL", urlTxt, desctxt, bTimeLine);
        return true;
#endif