/*---------------微信登陆授权用户信息-----------------
 *create by batcel at 2017-11-01
 */

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CWechatUserAuth
{

    private static CWechatUserAuth AuthInstance = null;

    //授权用户信息
    private Dictionary<string, string> AuthUserInfoMap;

    //缓存的微信用户头像(包含其他用户)
    private Dictionary<string, Sprite> UserHeadImgMap;

    CWechatUserAuth()
    {
        AuthUserInfoMap = new Dictionary<string, string>();
        UserHeadImgMap = new Dictionary<string, Sprite>();
    }

    public static CWechatUserAuth GetInstance()
    {
        if (AuthInstance == null)
            AuthInstance = new CWechatUserAuth();
        return AuthInstance;
    }

    /// <summary>
    /// 获取授权用户信息
    /// </summary>
    /// <param name="authcode"></param>
    public void GetAuthUserInfo(string authcode)
    {
        string authToken, authOpenId;
        GetAuthToken(authcode, out authToken, out authOpenId);
        if (!string.IsNullOrEmpty(authOpenId))
        {
            GetAuthUserInfoJosnMap(authToken, authOpenId);
            WriteWeChatAuthInfo();
        }
        CLoginUI.Instance.RequestLogin(CLoginUI.LoginType.LoginType_Wechat);
        //test
        //DownHeadImage(GetUserUnionId(), GetUserHeadImgUrl());
    }

    /// <summary>
    /// 获取微信用户统一标识
    /// </summary>
    /// <returns></returns>
    public string GetUserUnionId()
    {
        string userUnionId;
        if (AuthUserInfoMap.TryGetValue("unionid", out userUnionId))
        {
            return userUnionId;
        }
        return string.Empty;
    }

    /// <summary>
    /// 获取微信普通用户昵称
    /// </summary>
    /// <returns></returns>
    public string GetUserNickname()
    {
        string usernickname;
        if (AuthUserInfoMap.TryGetValue("nickname", out usernickname))
        {
            return usernickname;
        }
        return string.Empty;
    }

    /// <summary>
    /// 获取微信用户头像url
    /// </summary>
    /// <returns></returns>
    public string GetUserHeadImgUrl()
    {
        string userimgurl;
        if (AuthUserInfoMap.TryGetValue("headimgurl", out userimgurl))
        {
            return userimgurl;
        }
        return string.Empty;
    }

    /// <summary>
    /// 获取微信用户性别普通用户性别，1为男性，2为女性
    /// </summary>
    /// <returns></returns>
    public byte GetUserSex()
    {
        byte sex = 1;
        string usersex;
        if (AuthUserInfoMap.TryGetValue("sex", out usersex))
        {
            byte.TryParse(usersex, out sex);
            return sex;
        }
        return sex;
    }

    /// <summary>
    /// 写微信授权用户信息到本地
    /// </summary>
    public void ReadWeChatAuthInfo()
    {
        string AccountConfigPath = GameDefine.AppPersistentDataPath + GameDefine.AccountInfoFile;

        INIParser IniFile = new INIParser();
        IniFile.Open(AccountConfigPath);
        string namestr = IniFile.ReadValue("WXUser", "nickname", string.Empty);
        if (!string.IsNullOrEmpty(namestr))
        {
            string imgurlstr = IniFile.ReadValue("WXUser", "headimgurl", string.Empty);
            string unionidstr = IniFile.ReadValue("WXUser", "unionid", string.Empty);
            string sexstr = IniFile.ReadValue("WXUser", "sex", "1");
            AuthUserInfoMap.Clear();
            AuthUserInfoMap.Add("nickname", namestr);
            AuthUserInfoMap.Add("sex", sexstr);
            AuthUserInfoMap.Add("headimgurl", imgurlstr);
            AuthUserInfoMap.Add("unionid", unionidstr);

            Debug.Log("WxUserInfo name:" + namestr + ",sex:" + sexstr + ",img:" + imgurlstr + ",unionid:" + unionidstr);
        }
        else
        {
            Debug.Log("WxUserInfo null");
        }
        IniFile.Close();
    }

    /// <summary>
    /// 写微信授权用户信息到本地
    /// </summary>
    private void WriteWeChatAuthInfo()
    {
        string AccountConfigPath = GameDefine.AppPersistentDataPath + GameDefine.AccountInfoFile;

        INIParser IniFile = new INIParser();
        IniFile.Open(AccountConfigPath);
        IniFile.WriteValue("WXUser", "nickname", GetUserNickname());
        IniFile.WriteValue("WXUser", "sex", GetUserSex().ToString());
        IniFile.WriteValue("WXUser", "headimgurl", GetUserHeadImgUrl());
        IniFile.WriteValue("WXUser", "unionid", GetUserUnionId());
        IniFile.Close();
    }

    /// <summary>
    /// 获取微信登陆授权token
    /// </summary>
    /// <param name="authcode"></param>
    /// <param name="authtoken"></param>
    /// <param name="authopenid"></param>
    private void GetAuthToken(string authcode, out string authtoken, out string authopenid)
    {
        authtoken = string.Empty;
        authopenid = string.Empty;

        //通过code获取access_token的接口
        string urltoken = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + PayPlatformDefine.WXAPPID
                            + "&secret=" + PayPlatformDefine.WXAppSECRET
                            + "&code=" + authcode
                            + "&grant_type=authorization_code";

        UnityWebRequest wxtokenReq = UnityWebRequest.Get(urltoken);
        wxtokenReq.SetRequestHeader("Content-Type", "application/json");
        wxtokenReq.Send();
        while (!wxtokenReq.isDone) { }
        string TokenContent = wxtokenReq.downloadHandler.text.Trim();
        wxtokenReq.Dispose();


        var jsonobj = new JSONObject(TokenContent);
        Dictionary<string, string> tokenDic = jsonobj.ToDictionary();

        if (tokenDic.TryGetValue("access_token", out authtoken))
        {
            tokenDic.TryGetValue("openid", out authopenid);
            Debug.Log("Wechat Auth Access_token:" + authtoken + " Open:" + authopenid);
        }

    }

    /// <summary>
    /// 获取授权用户信息
    /// </summary>
    /// <param name="token"></param>
    /// <param name="openId"></param>
    private void GetAuthUserInfoJosnMap(string token, string openId)
    {
        //获取用户个人信息（UnionID机制）
        string userinfourl = "https://api.weixin.qq.com/sns/userinfo?access_token=" +
            token + "&openid=" + openId;

        UnityWebRequest wxuserinfoReq = UnityWebRequest.Get(userinfourl);
        wxuserinfoReq.SetRequestHeader("Content-Type", "application/json");
        wxuserinfoReq.Send();
        while (!wxuserinfoReq.isDone) { }
        string userInfoContent = wxuserinfoReq.downloadHandler.text.Trim();
        wxuserinfoReq.Dispose();

        var userInfojsonobj = new JSONObject(userInfoContent);
        AuthUserInfoMap.Clear();
        AuthUserInfoMap = userInfojsonobj.ToDictionary();

        //删除headimgurl中多余的反斜杠(\)
        string headurlstr;
        if (AuthUserInfoMap.TryGetValue("headimgurl", out headurlstr))
        {
            string headimgurl = headurlstr.Replace("\\", "");
            AuthUserInfoMap["headimgurl"] = headimgurl;
        }

    }

    /// <summary>
    /// 获取微信授权用户头像Image
    /// </summary>
    /// <param name="iconUrl"></param>
    /// <returns></returns>
    public Sprite GetUserHeadImg(string UnionID, string imgurl)
    {
        if (string.IsNullOrEmpty(imgurl))
            return GameMain.hall_.sprites_["101"];

        Sprite headImgSprite;
        //map中直接返回
        if (UserHeadImgMap.TryGetValue(UnionID, out headImgSprite))
            return headImgSprite;

        //查看本地文件夹是否有
        if (File.Exists(GameDefine.HeadImageSavePath + UnionID))
        {
            FileStream imgstream = File.OpenRead(GameDefine.HeadImageSavePath + UnionID);
            byte[] imgbytes = new byte[(int)imgstream.Length];
            imgstream.Read(imgbytes, 0, imgbytes.Length);
            Texture2D icontex = new Texture2D(80, 80);
            icontex.LoadImage(imgbytes);
            headImgSprite = Sprite.Create(icontex, new Rect(0, 0, icontex.width, icontex.height), new Vector2(0, 0));
            UserHeadImgMap.Add(UnionID, headImgSprite);
            return headImgSprite;
        }

        //本地和map都没有则下载玩家头像     
        headImgSprite = DownHeadImage(UnionID, imgurl);

        return headImgSprite;
    }


    private Sprite DownHeadImage(string unionId, string imgurl)
    {
        //本地和map都没有则下载玩家头像     
        //string trueiconurl = imgurl.Replace("\\", "");
        UnityWebRequest iconwebReq = UnityWebRequest.Get(imgurl);
        iconwebReq.Send();
        while (!iconwebReq.isDone) { }
        byte[] iconbytes = iconwebReq.downloadHandler.data;
        iconwebReq.Dispose();

        Texture2D icontex = new Texture2D(80, 80);

        icontex.LoadImage(iconbytes);

        Sprite headImgSpr = Sprite.Create(icontex, new Rect(0, 0, icontex.width, icontex.height), new Vector2(0, 0));

        //保存到本地文件夹中
        //SaveHeadImage(unionId, iconbytes);
        UserHeadImgMap.Add(unionId, headImgSpr);


        /*Image icon = GameObject.Find("Canvas/Root").transform.FindChild("Login").FindChild("Panel_Login").FindChild("Button_TouristLogin").gameObject
        .GetComponent<Image>();
        icon.sprite = headImgSpr;*/

        return headImgSpr;
    }



    /// <summary>
    /// 保存下载的微信用户头像到本地
    /// </summary>
    /// <param name="unionId"></param>
    /// <param name="imgdata"></param>
    private void SaveHeadImage(string unionId, byte[] imgdata)
    {
        if (!Directory.Exists(GameDefine.HeadImageSavePath))
        {
            Directory.CreateDirectory(GameDefine.HeadImageSavePath);
        }

        string path = GameDefine.HeadImageSavePath + unionId;
        File.WriteAllBytes(path, imgdata);
    }

    /// <summary>
    /// 申请微信登陆扫描二维码
    /// </summary>
    public string ReqBuildAuthQRcode(uint connId,uint GateLoginConnId)
    {

        //通过code获取access_token的接口
        string redirecturl = "http%3A%2F%2Flsb.jslaisai.com%2FWxWebQRCodeAuth%3F";
        string reqQRcodeurl = "https://open.weixin.qq.com/connect/qrconnect?appid=" + PayPlatformDefine.WXAPPID
                            + "&redirect_uri=" + redirecturl
                            + "&response_type=code&scope=snsapi_login"
                            + "&lang=zh_CN"
                            + "&state=" + connId.ToString() +"|" + GateLoginConnId.ToString() +"#wechat_redirect";
        return reqQRcodeurl;
//         UnityWebRequest wxqrcodeReq = UnityWebRequest.Get(reqQRcodeurl);
//         //wxqrcodeReq.SetRequestHeader("Content-Type", "application/json");
//         wxqrcodeReq.Send();
//         while (!wxqrcodeReq.isDone) { }
//         string TokenContent = wxqrcodeReq.downloadHandler.text.Trim();
//         wxqrcodeReq.Dispose();
//         //截取微信扫描登录二维码uuid
//         int uuidpos = TokenContent.IndexOf("uuid=");
//         return TokenContent.Substring(uuidpos + 5, 16);       
    }
}