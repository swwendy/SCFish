﻿using System;

[Hotfix]
    {
        if (ddCanvas == null)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj = (GameObject)bundle.LoadAsset("GameResult_Ticket");
        if(ticketResultObj != null)
            Destroy(ticketResultObj);

        ticketResultObj = (GameObject)GameMain.instantiate(obj);
        ticketResultObj.SetActive(false);
        ticketResultObj.transform.SetParent(ddCanvas.transform, false);
    }
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("PopUp_NameList");
        clubMemberList = (GameObject)GameMain.instantiate(obj0);
        clubMemberList.SetActive(false);
        clubMemberList.transform.SetParent(ddCanvas.transform, false);
    }

    public static void ShowTicketResult(long num, float showTime = 2f)
    {
        if (ticketResultObj == null)
            return;

        if (num == 0)
        {
            return;
        }

    static IEnumerator DealTicketResult(long num, float showTime = 3f)
    {
        ticketResultObj.SetActive(true);

        Transform tfm = ticketResultObj.transform.Find("ImageBG/Image_TicketFrame/Text_Ticket");
        tfm.GetComponent<Text>().text = "x" + num.ToString();


        tfm = ticketResultObj.transform.Find("ImageBG/Text/Anime_Ticket_1");
        tfm.gameObject.SetActive(true);

        Transform tfm1 = ticketResultObj.transform.Find("ImageBG/Text/Anime_Ticket_2");
        tfm1.gameObject.SetActive(false);

        DragonBones.Animation anim = tfm.GetComponent<DragonBones.UnityArmatureComponent>().animation;
        anim.Play();

        yield return new WaitUntil(() => anim.isCompleted);

        tfm.gameObject.SetActive(false);
        tfm1.gameObject.SetActive(true);

        anim = tfm1.GetComponent<DragonBones.UnityArmatureComponent>().animation;
        anim.Play();

        yield return new WaitForSecondsRealtime(showTime);

        ticketResultObj.SetActive(false);
    }

    // Use this for initialization
    void Start()
    }

#endif
            hall_.OnHallDeleteObj();
        {
            if (hall_ != null)
                hall_.CheckSocketConnectedState();
        }
        if (hall_ != null)
        {
            hall_.NotifyAppBackStateToGameServer(pause);
        }

#if UNITY_ANDROID
                  //         if (pause)
                  //         {           
                  //             androidactivity.Call("acquireWakeLock");
                  //         }
                  //         else
                  //         {
                  //             androidactivity.Call("releaseWakeLock");
                  //         }
#endif
        //Debug.Log("OnApplicationFocus:" + focus);
        if (hall_ != null && hall_.GameBaseObj != null)

    /// 截屏保存文件夹处理
    /// </summary>
    {
        //创建文件夹
        if (!Directory.Exists(GameDefine.ScreenshotSavePath))
        else
        //清理上次运行保存的截图,防止截图过多占用存储空间
        {
            DirectoryInfo fileDir = new DirectoryInfo(GameDefine.ScreenshotSavePath);
            FileInfo[] files = fileDir.GetFiles();
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i] != null)
                    {
                        //Debug.Log("删除文件:" + files[i].FullName + "__over");
                        files[i].Delete();
                        files[i] = null;
                    }
                }
                files = null;
            }
        }
    }


    /// <summary>
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else

    /// <summary>
        string appconfigfilepath = GameCommon.GetAppStreamingAssetPath() + "AppConfig.ini";
        INIParser ini = new INIParser();
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(appconfigfilepath);
            while (!www.isDone) { }
            ini.OpenFromString(www.text);
            www.Dispose();
        }
        else
        {
            ini.Open(appconfigfilepath);
        }
        m_strAppVersion = ini.ReadValue("Config", "AppVer", "1.0");


    /// <summary>

    public static void ST(IEnumerator routine)
    {
        if (Instance == null)
    }
    {
        if (time < 0f)
            yield return new WaitForEndOfFrame();
        else
            yield return new WaitForSecondsRealtime(time);
        call();
    }
    {
        Debug.Log(" Wechat login auth on Resp:" + retcode);
        //0(为0时返回用户换取access_token的code，仅在ErrCode为0时有效)
        //-4（用户拒绝授权）
        //-2（用户取消）
        int retstate;
        if(retstate == -4)
        {
            //用户拒绝授权
        }
        else if(retstate == -2)
        {
            //用户取消
        }
        else
        {
            //用户同意
            CWechatUserAuth.GetInstance().GetAuthUserInfo(retcode);
        }
    }

    //微信支付回调
    public void WXPayCallback(string retcode)
        //有支付结果返回值则结束购买过程
        //if (retstate == -1 || retstate == -2)
        //{

    //微信分享回调
    public void WXShareCallback(string retcode)
        //0   成功 
        //-1  错误,  
        //-2  用户取消, 
        //-4  拒绝 
        //其它：未知错误 
        int retstate;
        //分享成功逻辑处理
        if (retstate == 0)
        {
            //Player.FinishMission();
            if(hall_.crb_ != null)
            {
                if (hall_.crb_.root_ != null)
                    hall_.crb_.root_.transform.Find("Pop_up/ImageShare").gameObject.SetActive(false);
            }
        }
    }

    //支付宝回调
    public void MayunPayCallback(string retcode)

#if WINDOWS_GUEST
    //游客登陆ID
    private string m_strUserLoginID = string.Empty;
    private bool   m_bShowUserLoginState = false;
    private float  m_fLoginUIPosY = 0;
    private bool   m_bMoveState = false;
    public void OnGUI()
    {
        if(hall_ == null)
        {   
            return;
        }
        GUI.SetNextControlName("UserLoginID");
        m_strUserLoginID = GUI.TextField(new Rect(UnityEngine.Screen.width * 0.5f - 50, m_fLoginUIPosY - 20, 100, 20), m_strUserLoginID, 15);
        GUI.SetNextControlName("UserLogin");
        if (GUI.Button(new Rect(UnityEngine.Screen.width * 0.5f - 40, m_fLoginUIPosY - 1, 80, 20), "自定义登陆"))
        {
            if (hall_.GetCurrentGameState() == GameState_Enum.GameState_Login)
            {
                GuestRequestLogin();
            }
        }

        float TargetPos = 0.0f;
        if (m_bShowUserLoginState)
        {
            GUI.FocusControl("UserLoginID");

            TargetPos = UnityEngine.Screen.height * 0.5f - 10;
            m_fLoginUIPosY = Mathf.Lerp(m_fLoginUIPosY, TargetPos, 0.6f);
            if(TargetPos.Equals(m_fLoginUIPosY))
            {
                m_bMoveState = true;
            }
        }
        else
        {
            m_bMoveState = false;
            m_strUserLoginID = string.Empty;
            TargetPos = -20.0f;
            m_fLoginUIPosY = Mathf.Lerp(m_fLoginUIPosY, TargetPos,0.6f);
        }

        Event CurEent = Event.current;
        if (hall_.GetCurrentGameState() == GameState_Enum.GameState_Login)
        {
            if(CurEent.isKey && CurEent.type == EventType.KeyDown)
            {
                if (CurEent.keyCode == KeyCode.BackQuote)
                {
                    m_bShowUserLoginState = !m_bMoveState;
                }
                else if (CurEent.keyCode == KeyCode.None)
                {
                    GuestRequestLogin();
                }
            }
            else
            {
                if( -1 != m_strUserLoginID.LastIndexOf('`'))
                {
                    m_strUserLoginID = string.Empty;
                    m_bShowUserLoginState = !m_bMoveState;
                    GUI.FocusControl(null);
                }
            }
        }else
        {
            m_bShowUserLoginState = false;
        }
    }

    /// <summary>
    /// 游客自定义登陆
    /// </summary>
    private void GuestRequestLogin()
    {
        if(string.IsNullOrEmpty(m_strUserLoginID))
        {
            return;
        }
        CLoginUI.Instance.SetVisitorAccountId(uint.Parse(m_strUserLoginID));
        CLoginUI.Instance.RequestLogin();
        Debug.Log("登陆ID:" + m_strUserLoginID);
        m_bShowUserLoginState = false;
        m_strUserLoginID = string.Empty;
    }
#endif

#if UKGAME_SDK
    {
      Debug.Log(" UKGameSDK on Resp:" + retstr);
      int retstate;
      if(retstate != 0)
      {
            CLoginUI.Instance.UnionIdLogin(retstr);
      }
    }

    public void UKGameSDKPayCallback(string retstr)
    {
        Debug.Log(" UKGameSDK Pay on Resp:" + retstr);
        int retstate;
        int.TryParse(retstr, out retstate);
        //if (retstate != 0)
        {
            Player.BuyEnd();
            //CLoginUI.Instance.UnionIdLogin(retstr);
        }
    }
#endif
    public void EnterRoom(string code)
    {
        Debug.Log("EnterRoom:" + code);

        string[] strs = code.Split('|');
        if (strs.Length < 4)
        {
            Debug.LogWarning("param count wrong:" + strs.Length);
            return;
        }

        hall_.ReqEnterGameRoom(byte.Parse(strs[0]), byte.Parse(strs[1]), byte.Parse(strs[2]), uint.Parse(strs[3]));
    }