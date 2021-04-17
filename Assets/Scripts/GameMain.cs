using System;using System.Collections;using System.IO;using UnityEngine;using XLua;using UnityEngine.EventSystems;using UnityEngine.UI;using UnityEngine.Events;

[Hotfix]public class GameMain : MonoBehaviour{    public static GameMain Instance;    /// <summary>    /// 当前程序版本号    /// </summary>    private string m_strAppVersion;    /// <summary>    /// 当前资源版本号    /// </summary>    private string m_strResVersion;    /// <summary>    /// 游戏服务器IP    /// </summary>    private string m_strServerIp;    private int m_iServerPort;    /// <summary>    /// lua    /// </summary>    private LuaEnv luaenv = null;    //大厅对象    public static HallMain hall_ = null;    static GameObject mainObj;    public static GameObject ddCanvas;    public static GameObject clubMemberList;    static GameObject ticketResultObj;    static GameMain()    {        mainObj = new GameObject("gamemain");        DontDestroyOnLoad(mainObj);        Instance = mainObj.AddComponent<GameMain>();        mainObj.AddComponent<LogFile>();        if(Luancher.IsPrintLogOnScreen)           mainObj.AddComponent<ShowFPS_OnGUI>();        ddCanvas = GameObject.Find("Canvas_1");        DontDestroyOnLoad(ddCanvas);        LoadClubMemberList();    }    static public void LoadTicketResult()
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
    }    static void LoadClubMemberList()
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
        {            ticketResultObj.SetActive(false);
            return;
        }        SC(DealTicketResult(num, showTime));    }

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
    void Start()    {        //Debug.Log("start");        InitManager();        LoadLua();        testmobilehotfix();        hall_ = new HallMain();        hall_.Start();    }    // Update is called once per frame    void Update()    {        //Debug.Log("update in gamemain");        if (luaenv != null)            luaenv.Tick();        ProcessKeyDown();          if (hall_ != null)            hall_.Update();
    }    void OnDestroy()    {#if HOTFIX_ENABLE        if (luaenv != null)        {            LuaFunction luafunc = luaenv.Global.Get<LuaFunction>("ClearHotfix");            if (luafunc != null)                luafunc.Call();            luaenv.Dispose();            luaenv = null;        }

#endif        if (hall_ != null)
            hall_.OnHallDeleteObj();        CCsvDataManager.Instance.LocalABVerDataMgr.WriteLocalABVersionDataToFile();    }    //程序暂停    private void OnApplicationPause(bool pause)    {        Debug.Log("OnApplicationPause:" + pause);        //程序从后台切回时检测是否与服务器断开连接        if(!pause)
        {
            if (hall_ != null)
                hall_.CheckSocketConnectedState();
        }
        if (hall_ != null)
        {
            hall_.NotifyAppBackStateToGameServer(pause);
        }

#if UNITY_ANDROID//         AndroidJavaObject androidactivity = AlipayWeChatPay.GetAndroidActivity();
                  //         if (pause)
                  //         {           
                  //             androidactivity.Call("acquireWakeLock");
                  //         }
                  //         else
                  //         {
                  //             androidactivity.Call("releaseWakeLock");
                  //         }
#endif    }    //程序失去焦点    private void OnApplicationFocus(bool focus)    {
        //Debug.Log("OnApplicationFocus:" + focus);
        if (hall_ != null && hall_.GameBaseObj != null)            hall_.GameBaseObj.OnApplicationFocus(focus);    }    public void testmobilehotfix()    {        Debug.Log("dsfasdfasd in C#");    }    /// <summary>    /// 初始程序设置    /// </summary>    private void InitApplicationSetting()    {        Screen.sleepTimeout = SleepTimeout.NeverSleep;        Application.targetFrameRate = 30;        //Application.runInBackground = true;        Input.multiTouchEnabled = false;    }    /// <summary>    /// 初始化应用程序    /// </summary>    /// <param name="ResVer">资源版本号</param>    public void InitApplication(string InResVer,string SerIp,int SerPort)    {        Debug.Log("GameMain InitApplication");
        m_strResVersion = InResVer;        m_strServerIp = SerIp;        m_iServerPort = SerPort;        //LogFile.InitLog();        ReadAppVersion();        LoadGameAssetBundle();        CCsvDataManager.Instance.LoadAllCsv();        ScreenshotFilesDirectory();    }    /// <summary>    /// 初始各种管理器    /// </summary>    private void InitManager()    {        AudioManager.Instance.InitAudioMgr(mainObj);    }    /// <summary>
    /// 截屏保存文件夹处理
    /// </summary>    private void ScreenshotFilesDirectory()
    {
        //创建文件夹
        if (!Directory.Exists(GameDefine.ScreenshotSavePath))            Directory.CreateDirectory(GameDefine.ScreenshotSavePath);
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


    /// <summary>    /// 加载lua脚本    /// </summary>    private void LoadLua()    {#if HOTFIX_ENABLE        string LuaFilepath = GameDefine.AppPersistentDataPath + GameDefine.HotFixLuaFile;        //检测lua文件是否存在        if (!File.Exists(LuaFilepath))        {            Debug.Log("lua 文件不存在");            return;        }        luaenv = new LuaEnv();        //使用流的形式读取        StreamReader sr = null;        try        {            sr = File.OpenText(LuaFilepath);        }        catch (Exception)        {            return;        }        string luatxt = sr.ReadToEnd();        luaenv.DoString(luatxt);        LuaFunction luafunc = luaenv.Global.Get<LuaFunction>("AddHotfix");        if (luafunc != null)            luafunc.Call();        sr.Close();        sr.Dispose();#endif    }    /// <summary>    /// 处理游戏按键响应处理    /// </summary>    private void ProcessKeyDown()    {        if (Input.GetKeyUp(KeyCode.Escape) && hall_ != null)        {            CCustomDialog.OpenCustomDialogUI("确定退出游戏吗", ExitApplication,"系 统");                  }    }    public void ExitApplication(object pragma)    {        int exitSelectState = (int)pragma;        //确定退出        if(exitSelectState == 1)        {            OnDestroy();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else            Application.Quit();#endif        }    }    /// <summary>    /// 获取程序版本号    /// </summary>    /// <returns></returns>    public string GetAppVersion()    {        return m_strAppVersion;    }    /// <summary>    /// 获取资源版本号    /// </summary>    /// <returns></returns>    public string GetResVersion()    {        return m_strResVersion;    }    //获取Server IP    public string GetServerIP()    {        return m_strServerIp;    }    //获取server Port    public int GetServerPort()    {        return m_iServerPort;    }

    /// <summary>    /// 读取应用程序版本号    /// </summary>    private void ReadAppVersion()    {
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
        m_strAppVersion = ini.ReadValue("Config", "AppVer", "1.0");        ini.Close();    }


    /// <summary>    /// 加载游戏所需的AssetBundle    /// </summary>    private void LoadGameAssetBundle()    {        AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, GameDefine.CsvAssetbundleName);        AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, GameDefine.HallAssetbundleName);        //AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, "resverab");        /*AssetBundle bundle = AssetBundleManager.GetAssetBundle("csv");        if(bundle)        {            TextAsset datatxt = bundle.LoadAsset("AssetbundleCSV.txt", typeof(TextAsset)) as TextAsset;            if(datatxt)              Debug.Log(datatxt.text);        }*/    }    public static bool safeDeleteObj(UnityEngine.Object obj)    {        if (obj == null)            return false;        DestroyImmediate(obj);        return true;    }    public static Coroutine SC(IEnumerator routine)    {        if (Instance == null)            return null;        return Instance.StartCoroutine(routine);    }

    public static void ST(IEnumerator routine)    {        if (Instance == null || null == routine)            return;        Instance.StopCoroutine(routine);    }    public static void WaitForCall(float time, UnityAction call)//time <0:nextframe
    {
        if (Instance == null)            return;        Instance.StartCoroutine(WaitForCallEnumerator(time, call));
    }    static IEnumerator WaitForCallEnumerator(float time, UnityAction call)
    {
        if (time < 0f)
            yield return new WaitForEndOfFrame();
        else
            yield return new WaitForSecondsRealtime(time);
        call();
    }    public static UnityEngine.Object instantiate(UnityEngine.Object obj)    {        return Instantiate(obj);    }    //微信登陆授权回调    public void WXAuthCallback(string retcode)
    {
        Debug.Log(" Wechat login auth on Resp:" + retcode);
        //0(为0时返回用户换取access_token的code，仅在ErrCode为0时有效)
        //-4（用户拒绝授权）
        //-2（用户取消）
        int retstate;        int.TryParse(retcode, out retstate);
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
    public void WXPayCallback(string retcode)    {        Debug.Log(" WxchatPay sdk on Resp:" + retcode);        //0   成功 展示成功页面        //-1  错误 可能的原因：签名错误、未注册APPID、项目设置APPID不正确、注册的APPID与设置的不匹配、其他异常等。        //-2  用户取消 无需处理。发生场景：用户不支付了，点击取消，返回APP。        int retstate;        int.TryParse(retcode, out retstate);
        //有支付结果返回值则结束购买过程
        //if (retstate == -1 || retstate == -2)
        //{        //    Player.BuyEnd(); ;        //}    }

    //微信分享回调
    public void WXShareCallback(string retcode)    {        Debug.Log(" Wxchat Share sdk on Resp:" + retcode);
        //0   成功 
        //-1  错误,  
        //-2  用户取消, 
        //-4  拒绝 
        //其它：未知错误 
        int retstate;        int.TryParse(retcode, out retstate);
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
    public void MayunPayCallback(string retcode)    {        Debug.Log(" Mayun sdk on Resp:" + retcode);        /*9000    订单支付成功        8000    正在处理中，支付结果未知（有可能已经支付成功），请查询商户订单列表中订单的支付状态        4000    订单支付失败        5000    重复请求        6001    用户中途取消        6002    网络连接出错        6004    支付结果未知（有可能已经支付成功），请查询商户订单列表中订单的支付状态        其它  其它支付错误*/        int retstate;        int.TryParse(retcode, out retstate);                //有支付结果返回值则结束购买过程        //if (retstate == 6001 || retstate == 6002 || retstate == 4000)        //{        //    Player.BuyEnd(); ;        //}    }

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

#if UKGAME_SDK    public void UKGameSDKLoginCallback(string retstr)
    {
      Debug.Log(" UKGameSDK on Resp:" + retstr);
      int retstate;      int.TryParse(retstr, out retstate);
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
    }}