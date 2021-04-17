using UnityEngine;using XLua;


/// <summary>/// 平台相关信息定义/// </summary>[LuaCallCSharp]public class PayPlatformDefine
{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    //微信网页应用
    public static readonly string WXAPPID = "wxf39b6dff0f6cfa3c";                    //微信平台的生成网页应用APPID
    public static readonly string WXAppSECRET = "11c3fc63708b065201824f0959dd0924";  //微信平台生成的网页应用APP SECRET
#else
    public static readonly string WXAPPID = "wx043094aaa5449b38";                    //微信支付平台的生成应用APPID
    public static readonly string WXAppSECRET = "f892d5043cc07087c1f992b837692218";  //微信平台生成的APP SECRET  
#endif
    public static readonly string WXMCH_ID = "1515219211";                           //微信支付商户ID 
    public static readonly string WxApiKey = "DjukmAw87yhgwi2SnagvHqsfzmstrc6x";     //微信API密钥
}