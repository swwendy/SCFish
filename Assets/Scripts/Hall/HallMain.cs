using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.SceneManagement;using UnityEngine.UI;using USocket.Messages;using UnityEngine.EventSystems;using DragonBones;using XLua;using System.IO;
using System;
using UnityEngine.Events;

//游戏请求录像数据消息进度
[LuaCallCSharp]
public enum GameRecordMsgState
{
    RecordMsg_Default, //默认值
    RecordMsg_Begin,   //发送
    RecordMsg_End,     //收到回复消息
}

[Hotfix]public class HallMain{    GameObject mainui_;    GameObject roomLobbyUI_;    GameObject roomui_;    GameObject createroomui_;    GameObject joinroomui_;    bool isDownloadGameAssetBundle;

    GameObject CanvasObj;

    //游戏状态
    private GameState_Enum enGameState;

    MessageBackApplyGame mbag_;

    // 玩家对象
    Player PlayerObj;

    //游戏类对象
    public CGameBase GameBaseObj;    //网络类型
    public NetworkReachability IntelentType;
    //是否断线重连过程中
    public static bool bDisconnectReconnection;
    //重连次数
    private byte ReconnectTimes;    public static NetWorkClient gametcpclient;
    public static NetWorkClient videotcpclient;

    GameObject m_objRelief;         //救济金
    public uint m_iAddReliefCoin;   //增加的救济金金额
    public byte m_iLeftReliefNum;//剩余救济金次数今天
    public bool m_bIsAfterLogin;//标志是否进入 afterlogin
    public bool isGetRelief;    public static byte m_iRoomSerIndex;    //房间服务器的编号    public Club club_;    public Shop shop;    public Dictionary<string, Sprite> sprites_;    //异步加载场景
    private AsyncOperation aAsyncSceneOperation;    private bool isAsyncLoadScene;
    bool isOtherLoadScene;
    //横屏场景加载界面
    private GameObject gSceneLoadProgressLandscapeUI;
    //竖屏场景加载界面
    private GameObject gSceneLoadProgressPortraitUI;    private GameObject gCurDisplayLoadSceneProgressUI;    private float fAsyncLoadProgress;    private GameKind_Enum CurGameId;
    public byte CurRoomIndex = 0;
    private bool AutoEnterGameMode;

    private Button m_RecordButton;
    //public Gift gift;
    Dictionary<byte, GameObject> gameicons_;

    //网络断线重连计时器
    private CTimerCirculateCall ReconnectTimer;

    //public Activity m_Activity;
    public Bulletin m_Bulletin;

    GameObject servicePanel_;
    GameObject redbag_;
    ActiveInfo ai_;
    public Contest contest;
    public GameObject contestui_;
    public GameRoom gamerooms_;
    public GameObject roomsui_;
    public SelfContest selfcontest_;

    public GameRecordMsgState m_eGameRecordMsgState;

    public CrashRedBag crb_;
    private GameDocumentPanel DocumentPanel;
    FriendsMoments moments_;
    //当前已经下载游戏资源数量
    int m_nCurGRDCount; 
    //当前下载游戏最大资源数量
    int m_nCurTGRDCount;

    // Use this for initialization
    public void Start()    {        initIcons();        mbag_ = new MessageBackApplyGame();        PlayerObj = new Player();        enGameState = GameState_Enum.GameState_Luancher;        isDownloadGameAssetBundle = false;        aAsyncSceneOperation = null;        isAsyncLoadScene = false;        isOtherLoadScene = false;        CurGameId = GameKind_Enum.GameKind_Max;        bDisconnectReconnection = false;        ReconnectTimes = 0;        m_iRoomSerIndex = 0;        m_nCurGRDCount = 0;        m_nCurTGRDCount = 0;        m_objRelief = null;        m_iAddReliefCoin = 0;        m_iLeftReliefNum = 0;        m_bIsAfterLogin = false;        AutoEnterGameMode = false;        IntelentType = NetworkReachability.NotReachable;        RegitserMsgHandle();

        isGetRelief = false;        m_eGameRecordMsgState = GameRecordMsgState.RecordMsg_Default;        AudioManager.Instance.PlayBGMusic(GameDefine.HallAssetbundleName, "hall");        gameicons_ = new Dictionary<byte, GameObject>();

        m_RecordButton = null;
        ReconnectTimer = null;
        DocumentPanel = null;
        //EmailDataManager.GetNewsInstance();
        AudioManager.Instance.MusicVolume = StateStorage.HasKey("PE_MusicVolume") ? StateStorage.LoadData<float>("PE_MusicVolume") : 1f;
        AudioManager.Instance.SoundVolume = StateStorage.HasKey("PE_SoundVolume") ? StateStorage.LoadData<float>("PE_SoundVolume") : 1f;
    }    void initIcons()    {        sprites_ = new Dictionary<string, Sprite>();        List<string[]> result = new List<string[]>();        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, "HeadIconCsv", out result);        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)            return;        for (int index = 2; index < result.Count; index++)        {            Sprite tempSprite = bundle.LoadAsset<Sprite>(result[index][1]);            sprites_.Add(result[index][0], tempSprite);        }        CRankUI.Instance.InitIcons(bundle);    }

    //检测网络环境是否发生变化
    private void CheckIntelentType()    {        if (IntelentType != NetworkReachability.NotReachable)        {            if (Application.internetReachability == NetworkReachability.NotReachable)            {                IntelentType = NetworkReachability.NotReachable;                CCustomDialog.OpenCustomConfirmUI(1018, IntelentTypeChangeCallBack);            }        }    }    private void IntelentTypeChangeCallBack(object param)    {        AnyWhereBackToLoginUI();    }

    
    //检测连接服务器状态
    public void CheckConnectSeverState()    {        if (bDisconnectReconnection)            return;
        if (enGameState >= GameState_Enum.GameState_Hall)        {
            if (!NetWorkClient.GetInstance().IsSocketConnected)
            {                bDisconnectReconnection = true;
                NetWorkClient.GetInstance().CloseNetwork();
                CCustomDialog.OpenCustomWaitUI("网络正在尝试第1次重连...");
                Debug.Log("socket disconnect server");
            }
        }        if(bDisconnectReconnection)
        {
            ReconnectTimes = 1;
            ReconnectTimer = new CTimerCirculateCall(5.0f, ReconnectTimerCallBack);
            xTimeManger.Instance.RegisterTimer(ReconnectTimer);
        }    }

    //断线重连计时回调
    private void ReconnectTimerCallBack(object param)
    {
        ReconnectTimes++;        //if(ReconnectTimes > 10)
        //{
        //    ReconnectTimer.SetDeleteFlag(true);
        //    CCustomDialog.CloseCustomWaitUI();

        //    AnyWhereBackToLoginUI();
        //    return;
        //}
        if(!NetWorkClient.GetInstance().IsSocketConnected)
        {
            CCustomDialog.OpenCustomWaitUI(string.Format("网络正在尝试第{0}次重连...", ReconnectTimes));
            Debug.Log(" Reconnecting GateServer ....");
            if (NetWorkClient.GetInstance().Reconnect())
            {
                bDisconnectReconnection = false;
                ReconnectTimer.SetDeleteFlag(true);
                CCustomDialog.CloseCustomWaitUI();

                //发送请求最新玩家数据
                CLoginUI.Instance.RequestLogin(CLoginUI.LoginType.LoginType_LastOne);
            }
        }
      
    }

    //游戏尚未结束，与游戏服重连
    public void ReconnectGameServer()
    {
        if (enGameState < GameState_Enum.GameState_Game )            return;
        bDisconnectReconnection = false;        if(AutoEnterGameMode)
        {        
            AutoEnterGameMode = false;
        }
        if (GameBaseObj != null)
            GameBaseObj.ReconnectSuccess();
    }



    public void ReconnectLoadGame(GameKind_Enum gameId, GameTye_Enum modetype)
    {
        //if(CurGameId == GameKind_Enum.GameKind_Max)
        {
            if (enGameState < GameState_Enum.GameState_Game )
            {
                AutoEnterGameMode = true;
                Debug.Log("重登陆,进入自动进入游戏模式 GameId:" + gameId + ",GameMode:"+ modetype);
                EnterGameScene(gameId, modetype);               
            }           
        }
    }

    //主动通知服务器我切后台了
    public void NotifyAppBackStateToGameServer(bool bInBack)    {        if(bInBack)
            SaveSoundState();
        if (bDisconnectReconnection)            return;        if (enGameState >=  GameState_Enum.GameState_Game )        {
            //主动通知服务器我切后台了
            if (NetWorkClient.GetInstance().IsSocketConnected)
            {
                UMessage appbackmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_APPBACKSTATENOTIFYGAMESERVER);
                appbackmsg.Add(GetPlayerId());
                appbackmsg.Add(bInBack);
                SendMsgToRoomSer(appbackmsg);
            }        }

    }

    //检测连接服务器状态
    public void CheckSocketConnectedState()    {        if (bDisconnectReconnection)            return;        if (enGameState == GameState_Enum.GameState_Game || enGameState == GameState_Enum.GameState_Hall             || enGameState == GameState_Enum.GameState_Contest || enGameState == GameState_Enum.GameState_Appointment)        {            if (!NetWorkClient.GetInstance().IsSocketConnected)            {                //发送一个探测消息
                UMessage checkmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_BEGIN);
                checkmsg.Add(GetPlayerId());
                NetWorkClient.GetInstance().SendMsg(checkmsg);
            }        }            }    public static bool ConnectLoginServer()    {        string ip = GameMain.Instance.GetServerIP();        int port = GameMain.Instance.GetServerPort();        if (ip == string.Empty)
            ip = "192.166.0.129";
        if (port == 0)
            port = 16701;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN        if (!Luancher.UpdateWithLuncher)        {            string serverInipath = Application.dataPath + "/Config/ServerConfig.ini";            INIParser IniFile = new INIParser();            IniFile.Open(serverInipath);            ip = IniFile.ReadValue("Server", "SvrIp", "127.0.0.1");            port = IniFile.ReadValue("Server", "SvrPort", 16201);            IniFile.Close();            Debug.Log("ServerConfig Ip:" + ip + ",Port:" + port);        }
#endif        bool success = NetWorkClient.GetInstance().InitNetWork(ip, port);        bDisconnectReconnection = !success;        if(success)
        {
            UMessage  ApplyMsg = new UMessage((uint)GameCity.EMSG_ENUM.CCGateMsg_PlayerApplyLoginGame);
            NetWorkClient.GetInstance().SendMsg(ApplyMsg);
        }        return success;    }



    private void RegitserMsgHandle()    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_LOGINSERDISCONNECT, LoginSerDisConnect);                     
        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKAPPLYGAME, ApplyGameSuccess);                     //请求进入游戏
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FIVEINROW_SM_BACKINVATECLUBMEMBER, InvateGame);         //收到邀请
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FIVEINROW_SM_BACKISAGREEINVATE, InvateGameSuccess);          //收到邀请进入游戏

        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_LOGIN, BackCarPortLogin);         //车行游戏消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Diudiule_enum.DiudiuleMsg_SM_LOGIN, BackDiuDiuLeLogin);       //丢丢乐游戏消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Diudiule_enum.DiudiuleMsg_SM_GAMESCENE, BackDiuDiuLeGameScene);       //丢丢乐重连消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Diudiule_enum.DiudiuleMsg_SM_NOGAMESCENE, BackDiuDiuLeNoGameScene);

        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.SlotSecondMsg.LabaMsg_SM_LOGIN, BackSlotLogin);               //拉霸遊戲消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Forest_enum.ForestMsg_SM_LOGIN, BackForestDanceLogin); //森林舞会遊戲消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FIVEINROW_SM_LOGIN, BackFiveLogin); //五子棋遊戲消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_TEXASPOKER_SM_LOGIN, BackTexasLogin); //德州遊戲消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_BULLKILL_SM_LOGIN, BackBullAllKillLogin); //通杀牛牛遊戲消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_BLACKJACK_SM_LOGIN, BackCustomLogin); //21点遊戲消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
             (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_LOGIN, BackCustomContestLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
             (uint)GameCity.EMSG_ENUM.CCMsg_BULLHUNDRED_SM_LOGIN, BackCustomLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
             (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_LOGIN, BackCustomLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_SM_LOGIN, BackBullHappyLogin); //抢庄牛牛游戏消息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
             (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_LOGIN, BackCustomContestLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
             (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_LOGIN, BackCustomContestLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
             (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_LOGIN, BackCustomContestLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
             (uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_LOGIN, BackCustomContestLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
             (uint)GameCity.EMSG_ENUM.CCMsg_TURNTABLE_SM_LOGIN, BackCustomLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKRECHARGE, BackCharge);                //充值
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_LOGIN, BackCustomContestLogin);         //够级
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_LOGIN, BackCustomContestLogin);        //象棋
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_LOGIN, BackCustomContestLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_ANSWER_SM_LOGIN, BackCustomContestLogin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKGMTOOLADDITEM, BackGmTool);           //GM工具信息

        //CMsgDispatcher.GetInstance().RegMsgDictionary(
        //    (uint)GameCity.EMSG_ENUM.CrazyCityMsg_UPDATECOINRANKDATATOCLINT, BackUpdateCoinRank); //更新玩家金钱榜
        //CMsgDispatcher.GetInstance().RegMsgDictionary(
        //    (uint)GameCity.EMSG_ENUM.CrazyCityMsg_NEEDUPDATECOINRANK, BackNeedUpdateCoinRank); //更新玩家金钱榜
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKCHANGERELIEF, BackAddRelief); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BEFOREGAMEOVER, BackBeforeGameOver); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_RUNHORSELIGHTDATA, RunHorseLightData); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_GMCLOSETALLSERVER, GmCloseAllConnect); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_UPDATERANDOMGAMEROLENUM, BackPeopleNumber); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SENDADDLOTTERYTOCLINT, BackLottery); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_REDBAGBEGIN, BackRedBagBegin); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_REDBAGEND, BackRedBagEnd); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKPLAYERREDBAG, BackOpenRedBag); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKTODAYREDBAGINFO, BackActiveInfo); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERDISORRECONNECT, PlayerDisOrReconnect); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYER_QUERYSTATE_REPLY, QueryStateReply); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKVIDEOSERIPPORT, BackVideoServerIp);
    }

    // Update is called once per frame
    public void Update()    {
        if (enGameState == GameState_Enum.GameState_Luancher)
            SwithGameStateToLogin();        if (GameBaseObj != null)            GameBaseObj.ProcessTick();        if (club_ != null)            club_.Update();

        if (isDownloadGameAssetBundle)
            UpdateGameResDownloadProcess();        if (isAsyncLoadScene)
            UpdateAsyncLoadSceneProcess();        NetWorkClient.GetInstance().Update();        //if (gametcpclient != null)        //    gametcpclient.Update();        if (null != videotcpclient)            videotcpclient.Update();        xTimeManger.Instance.Update();        CRollTextUI.Instance.ProcessTickRollText();        CRankUI.Instance.UpdateRankUI();        //if (gift != null)        //    gift.Update();

        //CheckIntelentType();
        CheckConnectSeverState();

        UpdateRedBag(Time.deltaTime);        if (contest != null)            contest.Update();        if (gamerooms_ != null)            gamerooms_.Update();        if (moments_ != null)            moments_.Update();        if (isOtherLoadScene && !isAsyncLoadScene)
        {
            SwitchToHallScene(true, 0);
            //isOtherLoadScene = false;
        }        //WeffectSwitch();    }
    //float unittime = 0.7f;    //float currentunittime = 0.0f;    //void WeffectSwitch()
    //{
    //    if (contestui_ == null)
    //        return;
    //    Image w1 = contestui_.transform.FindChild("UiRootBG/lobby_bg/ImageW/ImageW_1").gameObject.GetComponent<Image>();
    //    Image w2 = contestui_.transform.FindChild("UiRootBG/lobby_bg/ImageW/ImageW_2").gameObject.GetComponent<Image>();
    //    Color color1 = w1.color;
    //    Color color2 = w2.color;

    //    if(currentunittime <= unittime)
    //    {
    //        color1.a = currentunittime / unittime;
    //        color2.a = 1.0f - currentunittime / unittime;
    //    }
    //    else
    //    {
    //        color1.a = 1.0f - (currentunittime - unittime) / unittime;
    //        color2.a = (currentunittime - unittime) / unittime;
    //    }

    //    w1.color = color1;
    //    w2.color = color2;

    //    currentunittime += Time.deltaTime;

    //    if (currentunittime > unittime * 2)
    //        currentunittime = 0.0f;
    //}    public void OnHallDeleteObj()    {        if (gametcpclient != null)        {            gametcpclient.CloseNetwork();            gametcpclient = null;        }        if(videotcpclient != null)
        {
            videotcpclient.CloseNetwork();
            videotcpclient = null;
        }        NetWorkClient.GetInstance().CloseNetwork();
        CGameContestRankingTifings.GetChessRankingInstance(false).SaveContestRankingData();
        SaveSoundState();
    }

    void SaveSoundState()
    {
        StateStorage.SaveData("PE_MusicVolume", AudioManager.Instance.MusicVolume);
        StateStorage.SaveData("PE_SoundVolume", AudioManager.Instance.SoundVolume);
    }

    //情况房间服务器编号
    public static void ClearRoomSerIndex()
    {
        m_iRoomSerIndex = 0;
    }

    //设置房间服务器编号
    public static void SetRoomSerIndex(byte nIndex)
    {
        m_iRoomSerIndex = nIndex;
        Debug.Log("Set roomser success! index:" + m_iRoomSerIndex.ToString());
    }

    //发送消息到 roomser
    public static void SendMsgToRoomSer(UMessage msg)
    {
        if (m_iRoomSerIndex > 1)
        {
            msg.BaseMsgType = m_iRoomSerIndex;
            NetWorkClient.GetInstance().SendMsg(msg);
        }
        else
            Debug.Log("m_iRoomSerIndex is not valid" + m_iRoomSerIndex.ToString());
    }

    //发送消息loginser
    public static void SendMsgToLoginSer(UMessage msg)
    {
        msg.BaseMsgType = 1;
        NetWorkClient.GetInstance().SendMsg(msg);
    }    UnityArmatureComponent redbagbuttonAnimate;    void InitHallUIBtnListener()    {
        //GameObject returnBtn = contestui_.transform.FindChild("PanelHead_/Button_Return").gameObject;
        //XPointEvent.AutoAddListener(returnBtn, OnRetruen2Main, null);

        //CTrumpetUI.Instance.InitTrumpetButtenEvent("MainUI_New(Clone)/Panelbottom/Button_horn");
        //club_ = new Club(CanvasObj);
        //club_.Start();

        //GameObject shopBtn = mainui_.transform.FindChild("PanelHead_").FindChild("Button_Shop").gameObject;
        //XPointEvent.AutoAddListener(shopBtn, onClickShopBtn, null);
        //shop = new Shop(CanvasObj);
        //shop.InitShopUI();

        //GameObject servicebtn = mainui_.transform.FindChild("Panelbottom").FindChild("Left").FindChild("Button_service").gameObject;
        //XPointEvent.AutoAddListener(servicebtn, OnShowService, null);

        //GameObject redbagbtn = mainui_.transform.FindChild("Panelbottom").FindChild("Left").FindChild("Button_Redpackets").gameObject;
        //XPointEvent.AutoAddListener(redbagbtn, OnShowRedBag, null);

        //redbagbuttonAnimate = mainui_.transform.FindChild("Panelbottom").FindChild("Left").
        //    FindChild("Button_Redpackets").FindChild("Anime_Actred_button").gameObject.GetComponent<UnityArmatureComponent>();
        //redbagbuttonAnimate.animation.Stop();
        //CRankUI.Instance.InitRankUIBtnListener(mainui_);

        if (gamerooms_ == null)
            gamerooms_ = new GameRoom();
        else
            gamerooms_.InitGameRoom();

        GameObject joinroombtn = contestui_.transform.Find("Panelbottom/Bottom_Button/Button_join").gameObject;
        XPointEvent.AutoAddListener(joinroombtn, gamerooms_.OnShowNumberPanel, null);

        GameObject createroombtn = contestui_.transform.Find("Panelbottom/Bottom_Button/Button_table").gameObject;
        XPointEvent.AutoAddListener(createroombtn, gamerooms_.OnShowRulePanel, null);

        GameObject contestButton = contestui_.transform.Find("Panelbottom/Bottom_Button/Button_match").gameObject;
        XPointEvent.AutoAddListener(contestButton, OnClickTournament, 2);
    }

    void AskForNewsData()
    {

    }

    void onClickNews(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            //AudioManager.Instance.PlaySound(GameDefine.HallAssetbundleName, "UIbutton02");
            //AskForNewsData();
            Email.GetEmailInstance().Ask4MailData();
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (m_Bulletin != null)
                m_Bulletin.gameObject.SetActive(true);
        }    }

    void AskForBagData()
    {

    }

    void onClickBag(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            AskForBagData();            Bag.GetBagInstance().ShowBag();        }    }    public void AskForClubData()    {
        //if (club_ == null)
        //{
        //    UMessage clubMsg = new UMessage((uint)GameCity.ClubSecondMsg.CM_ClubSecondInfo);

        //    UClubInfo ci = new UClubInfo();
        //    ci.userID = PlayerObj.GetPlayerId();
        //    ci.SetSendData(clubMsg);

        //    NetWorkClient.GetInstance().SendMsg(clubMsg);

        //    return;
        //}

        if (moments_ == null)
        {
            UMessage momentsMsg = new UMessage((uint)GameCity.EMSG_ENUM.Friends_Moments_CM_Info);

            momentsMsg.Add(GetPlayerId());

            NetWorkClient.GetInstance().SendMsg(momentsMsg);
        }    }    void onClickClub(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            AskForClubData();
            //if (club_ == null)
            //{
            //    club_ = new Club(CanvasObj);
            //    club_.Start();
            //}
            //club_.ShowClub();

            //改成朋友进入逻辑             if(moments_ == null)
            {
                moments_ = new FriendsMoments();
            }            moments_.ShowFriendsMoments();

            if (contestui_ != null)
                contestui_.SetActive(false);        }    }    void onClickShopBtn(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            if (shop == null)            {                shop = new Shop(CanvasObj);                shop.InitShopUI();            }            shop.OpenOrCloseShopMainUI(true);        }    }    bool BackCharge(uint _msgType, UMessage _ms)    {        BackRecharge brc = new BackRecharge();        brc.ReadData(_ms);        Debug.Log("charge!!!!!!!!!!!!!!!!!!!");        if (brc.nCoin > 0)        {            //Text coin = mainui_.transform.FindChild("PanelHead_").            //    FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();            //coin.text = brc.nCoin.ToString();        }        if (brc.nDia > 0)        {            Text diamond = mainui_.transform.Find("PanelHead_").                Find("Image_DiamondFrame").Find("Text_Diamond").gameObject.GetComponent<Text>();            diamond.text = (brc.nDia + brc.nCoin).ToString();        }        return false;    }    bool BackGmTool(uint _msgType, UMessage _ms)    {        BackGmToolAddItem btai = new BackGmToolAddItem();        btai.ReadData(_ms);        PlayerObj.GetPlayerData().SetCoin (btai.coin);        PlayerObj.GetPlayerData().SetPlayerID(btai.accountID);        PlayerObj.GetPlayerData().SetVipLv(btai.level);        PlayerObj.GetPlayerData().SetDiamond(btai.diamond);        RefreshPlayerCurrency();        return false;    }    public Player GetPlayer()    {        return PlayerObj;    }    public PlayerData GetPlayerData()    {        return PlayerObj.GetPlayerData();    }    public uint GetPlayerId()    {
        return GetPlayerData().GetPlayerID();    }    bool LoginSerDisConnect(uint _msgType, UMessage _ms)
    {
        Debug.Log("Login server is disconnect, Please try again later!");
        return true;
    }    bool ApplyGameSuccess(uint _msgType, UMessage _ms)    {        CCustomDialog.CloseCustomWaitUI();        mbag_.ReadData(_ms);

        //请求进入游戏失败
        if (!mbag_.isin)        {            CurRoomIndex = 0;            CCustomDialog.OpenCustomConfirmUI(1020);            PlayerObj.ChangeRequestEnterGameState(false);            return false;        }        else
        {
            //m_iRoomSerIndex = mbag_.nSerIndex;
            SetRoomSerIndex(mbag_.nSerIndex);
        }        if (mbag_.kind == (byte)GameKind_Enum.GameKind_CarPort)            connect2CHserver();        else if (mbag_.kind == (byte)GameKind_Enum.GameKind_DiuDiuLe)            connect2DDLserver();        else if (mbag_.kind == (byte)GameKind_Enum.GameKind_LaBa)            connect2SlotServer();        else if (mbag_.kind == (byte)GameKind_Enum.GameKind_ForestDance)            connect2ForestDanceServer();        else if (mbag_.kind == (byte)GameKind_Enum.GameKind_FiveInRow)            connect2FiveInRowServer();        else if (mbag_.kind == (byte)GameKind_Enum.GameKind_BullHappy)            connect2BullHappyServer();        else            connect2CustomServer(mbag_.kind, 0);        return true;    }

    public void ClearChilds(GameObject obj)    {        int count = obj.transform.childCount;        for (int index = 0; index < count; index++)        {            GameMain.safeDeleteObj(obj.transform.GetChild(0).gameObject);        }    }    public void ResetInvatePanel(InvateMsg im)
    {
        GameObject friendList = GameMain.clubMemberList;
        friendList.SetActive(true);
        GameObject closeBtn = friendList.transform.Find("ImageBG").Find("ButtonClose").gameObject;
        XPointEvent.AutoAddListener(closeBtn, OnCloseFriendList, null);
        ResetFriendList(im);
    }

    void ResetFriendList(InvateMsg im)
    {
        GameObject friendList = GameMain.clubMemberList;
        GameObject namelist = friendList.transform.Find("ImageBG").Find("Viewport_ClubName").Find("Content_ClubName").gameObject;
        AssetBundle bundle = AssetBundleManager.GetAssetBundle("hall");        ClearChilds(namelist);        if (bundle != null)        {            for (int index = 0; index < GuildData.Instance().GuildMemberNum; index++)            {                if (GuildData.Instance().m_GuildMemberList[index].useid == GameMain.hall_.GetPlayerId())                    continue;                if (!GuildData.Instance().m_GuildMemberList[index].online)                    continue;                UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Club_NameList");                GameObject friendBtn = (GameObject)GameMain.instantiate(obj0);
                Text nameText = friendBtn.transform.Find("Text").gameObject.GetComponent<Text>();
                nameText.text = GuildData.Instance().m_GuildMemberList[index].name;
                Image icon = friendBtn.transform.Find("Image_HeadBG").Find("Image_HeadMask").Find("Image_HeadImage").gameObject.GetComponent<Image>();
                icon.sprite = GetIcon(GuildData.Instance().m_GuildMemberList[index].url,
                    GuildData.Instance().m_GuildMemberList[index].useid,
                    (int)GuildData.Instance().m_GuildMemberList[index].icon);
                friendBtn.transform.SetParent(namelist.transform, false);
                im.memberid = (uint)index;
                XPointEvent.AutoAddListener(friendBtn, SelectFriend, im);
            }
        }
    }

    void SelectFriend(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            FIR_AudioDataManager.GetInstance().PlayAudio(1001);
            InvateMsg msg = (InvateMsg)button;
            UMessage invateMsg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FIVEINROW_CM_INVATECLUBMEMBER);

            msg.memberid = GuildData.Instance().m_GuildMemberList[(int)msg.memberid].useid;
            msg.SetSendData(invateMsg);

            NetWorkClient.GetInstance().SendMsg(invateMsg);

            CloseFriendList();
        }
    }

    void OnCloseFriendList(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            FIR_AudioDataManager.GetInstance().PlayAudio(1001);
            CloseFriendList();
        }
    }

    void CloseFriendList()
    {
        GameMain.clubMemberList.SetActive(false);
    }    public InvateInfo invate_;    bool InvateGame(uint _msgType, UMessage _ms)    {        if (invate_ == null)            invate_ = new InvateInfo();        invate_.ReadData(_ms);        byte state = CResVersionCompareUpdate.CheckGameResNeedDownORUpdate(invate_.gameid);        if (state != 0)        {            CCustomDialog.OpenCustomConfirmUI(2010);            return false;        }        string[] param = { invate_.name, invate_.levelname, invate_.roomid.ToString() };        CCustomDialog.OpenCustomDialogWithFormatParams(2105, SendAgree2FiveInRow, param);        return true;    }    void SendAgree2FiveInRow(object isagree)    {        bool agree = (int)isagree == 1;        UMessage agreeMsg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FIVEINROW_CM_ISAGREEINVATE);        agreeMsg.Add(GetPlayerId());        agreeMsg.Add(agree);        agreeMsg.Add(invate_.gameid);        agreeMsg.Add(invate_.level);        agreeMsg.Add(invate_.roomid);        NetWorkClient.GetInstance().SendMsg(agreeMsg);    }    bool InvateGameSuccess(uint _msgType, UMessage _ms)    {        PickGameByID(invate_.gameid);        return true;    }

    //从任何地方返回登陆界面
    public void AnyWhereBackToLoginUI()    {        if (enGameState >= GameState_Enum.GameState_Game)        {            NetWorkClient.GetInstance().CloseNetwork();                       SwitchToHallScene(true, 2);        }               else        {            HallBackToLoginUI();        }    }

    //大厅界面返回登陆界面
    public void HallBackToLoginUI()    {        enGameState = GameState_Enum.GameState_Login;        NetWorkClient.GetInstance().CloseNetwork();        if(mainui_ != null)            mainui_.SetActive(false);        if (contestui_ != null)
            contestui_.SetActive(false);        CLoginUI.Instance.LoadChooseLoginType();    }    void connect2DDLserver()    {        if (!mbag_.isin)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 102);            PlayerObj.ChangeRequestEnterGameState(false);            return;        }        if (gametcpclient == null)            gametcpclient = new NetWorkClient();        bool isconn = gametcpclient.InitNetWork(mbag_.ip, mbag_.port);        if (!isconn)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 202);            PlayerObj.ChangeRequestEnterGameState(false);            return;        }        UMessage loginGame = new UMessage((uint)GameCity.Diudiule_enum.DiudiuleMsg_CM_LOGIN);        Ex_CMLogin ex_CMLogin = new Ex_CMLogin();        ex_CMLogin.diuMsgType = (uint)GameCity.Diudiule_enum.DiudiuleMsg_CM_LOGIN;        ex_CMLogin.userid = PlayerObj.GetPlayerId();        ex_CMLogin.SetSendData(loginGame);        SendMsgToRoomSer(loginGame);    }    void connect2CHserver()    {        if (!mbag_.isin)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 101);            PlayerObj.ChangeRequestEnterGameState(false);            return;        }        if (gametcpclient == null)            gametcpclient = new NetWorkClient();        //Debug.Log(mbag_.ip + " " + mbag_.port);        //bool isconn = gametcpclient.InitNetWork(mbag_.ip, mbag_.port);        //if (!isconn)        //{        //    CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 201);        //    PlayerObj.ChangeRequestEnterGameState(false);        //    return;        //}        UMessage loginGame = new UMessage((uint)GameCity.CarportMsg_enum.CarportMsg_CM_LOGIN);        CarPortLogin cpl = new CarPortLogin();        cpl.Create();        cpl.userID = PlayerObj.GetPlayerId();        cpl.SetSendData(loginGame);        SendMsgToRoomSer(loginGame);    }    void connect2ForestDanceServer()    {        if (!mbag_.isin)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 103);            PlayerObj.ChangeRequestEnterGameState(false);            return;        }        if (gametcpclient == null)            gametcpclient = new NetWorkClient();        //Debug.Log(mbag_.ip + " " + mbag_.port);        //bool isconn = gametcpclient.InitNetWork(mbag_.ip, mbag_.port);        //if (!isconn)        //{        //    CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 203);        //    PlayerObj.ChangeRequestEnterGameState(false);        //    return;        //}        UMessage loginGame = new UMessage((uint)GameCity.Forest_enum.ForestMsg_CM_LOGIN);        ForestDanceLogin fdl = new ForestDanceLogin();        fdl.Create();        fdl.userID = PlayerObj.GetPlayerId();        fdl.SetSendData(loginGame);        SendMsgToRoomSer(loginGame);    }    void connect2SlotServer()    {        if (!mbag_.isin)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 104);
            PlayerObj.ChangeRequestEnterGameState(false);
            return;        }        if (gametcpclient == null)            gametcpclient = new NetWorkClient();        //Debug.Log(mbag_.ip + " " + mbag_.port);        //bool isconn = gametcpclient.InitNetWork(mbag_.ip, mbag_.port);        //if (!isconn)        //{        //    CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 204);        //    PlayerObj.ChangeRequestEnterGameState(false);        //    return;        //}        UMessage loginGame = new UMessage((uint)GameCity.SlotSecondMsg.LabaMsg_CM_LOGIN);        USlotLogin sl = new USlotLogin();        sl.userID = PlayerObj.GetPlayerId();        sl.SetSendData(loginGame);        SendMsgToRoomSer(loginGame);    }    void connect2BullHappyServer()
    {
        if (!mbag_.isin)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 111);            PlayerObj.ChangeRequestEnterGameState(false);            return;        }        if (gametcpclient == null)            gametcpclient = new NetWorkClient();        Debug.Log(mbag_.ip + " " + mbag_.port);        bool isconn = gametcpclient.InitNetWork(mbag_.ip, mbag_.port);        if (!isconn)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 211);            PlayerObj.ChangeRequestEnterGameState(false);            return;        }        UMessage loginGame = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_CM_LOGIN);        loginGame.Add(PlayerObj.GetPlayerId());        SendMsgToRoomSer(loginGame);
    }    void connect2FiveInRowServer()    {        if (!mbag_.isin)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 105);            PlayerObj.ChangeRequestEnterGameState(false);            return;        }        if (gametcpclient == null)            gametcpclient = new NetWorkClient();        //Debug.Log(mbag_.ip + " " + mbag_.port);        //bool isconn = gametcpclient.InitNetWork(mbag_.ip, mbag_.port);        //if (!isconn)        //{        //    CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 205);        //    PlayerObj.ChangeRequestEnterGameState(false);        //    return;        //}        UMessage loginGame = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FIVEINROW_CM_LOGIN);        loginGame.Add(PlayerObj.GetPlayerId());        SendMsgToRoomSer(loginGame);    }    void connect2TexasServer()
    {
        if (!mbag_.isin)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 106);            PlayerObj.ChangeRequestEnterGameState(false);            return;        }        if (gametcpclient == null)            gametcpclient = new NetWorkClient();        Debug.Log(mbag_.ip + " " + mbag_.port);        bool isconn = gametcpclient.InitNetWork(mbag_.ip, mbag_.port);        if (!isconn)        {            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2016, 206);            PlayerObj.ChangeRequestEnterGameState(false);            return;        }        UMessage loginGame = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_TEXASPOKER_CM_LOGIN);        loginGame.Add(PlayerObj.GetPlayerId());        SendMsgToRoomSer(loginGame);
    }    public void Connect2AppointmentCommonServer(byte gameid, byte middleIn, uint appointmentId)
    {
        UMessage loginGame = new UMessage((uint)(100001 + gameid * 100));

        PlayerData pd = PlayerObj.GetPlayerData();
        loginGame.Add(pd.GetPlayerID());
        loginGame.Add((byte)2);// 0 匹配登陆 1比赛登陆  2约局登陆
        loginGame.Add(middleIn);
        loginGame.Add(appointmentId);

        byte[] uiname = { 0, 1, 3, 2 };
        GameKind_Enum GameKindID = (GameKind_Enum)gameid;
        switch (GameKindID)
        {
            case GameKind_Enum.GameKind_GuanDan:
            case GameKind_Enum.GameKind_Mahjong:
            case GameKind_Enum.GameKind_YcMahjong:
                loginGame.Add(uiname[AppointmentDataManager.AppointmentDataInstance().playerSitNo]);
                break;
            default:
                loginGame.Add(AppointmentDataManager.AppointmentDataInstance().playerSitNo);
                break;
        }
        //loginGame.Add(AppointmentDataManager.AppointmentDataInstance().playerSitNo);

        loginGame.Add(pd.GetPlayerName());
        loginGame.Add(pd.GetPlayerIconURL());
        loginGame.Add(pd.MasterScoreKindArray[gameid]);
        loginGame.Add(pd.PlayerSexSign);

        SendMsgToRoomSer(loginGame);

        Debug.Log("Connect 2 gameserver....");
    }    public void Connect2ContestCommonServer(byte gameid, byte middleIn,uint contestId, ushort roundPerTurn = 0, int roleNum = 0, byte nPromotion = 0)
    {
        if (gameid == 0)
            return;

        Debug.Log("连接游戏服务器......room sever index:" + m_iRoomSerIndex);

        MatchInGame.GetInstance().SetContestData(roundPerTurn, roleNum);

        UMessage loginGame = new UMessage((uint)(100001 + gameid * 100));
        PlayerData pd = PlayerObj.GetPlayerData();
        loginGame.Add(pd.GetPlayerID());
        loginGame.Add((byte)1);// 0 匹配登陆 1比赛登陆  2约局登陆
        loginGame.Add(middleIn);
        loginGame.Add(pd.GetPlayerName());
        loginGame.Add(pd.GetPlayerIconURL());
        loginGame.Add(contestId);
        loginGame.Add(nPromotion);
        SendMsgToRoomSer(loginGame);
    }    public void connect2CustomServer(byte gameId, byte middleIn)//middleIn:0正常登陆 1中途加入
    {        UMessage loginGame = new UMessage((uint)(100001 + gameId * 100));        loginGame.Add(PlayerObj.GetPlayerId());        loginGame.Add((byte)0);// 0 匹配登陆 1比赛登陆  2约局登陆
        loginGame.Add(middleIn);
        //SendMsgToRoomSer(loginGame);
        SendMsgToRoomSer(loginGame);    }    public void CutConnect(object pragma)    {        int title = (int)pragma;        if (title == 0)        {
            GameMain.Instance.ExitApplication(1);        }        else        {            bDisconnectReconnection = false;            AnyWhereBackToLoginUI();        }    }    public void OnGameReconnect(GameKind_Enum gameId, GameTye_Enum gameMode)
    {
        PlayerData pd = PlayerObj.GetPlayerData();
        byte gameKind = (byte)gameId;
        DebugLog.Log("OnGameReconnect:" + gameKind + " type:" + gameMode);

        if(m_iRoomSerIndex == 0)
        {
            if (gameMode == GameTye_Enum.GameType_Contest)
            {                if (contest == null)
                {                    contest = new Contest();
                    contest.SetCurrentContest(pd.nSpecilID_Before, pd.nSpecilID2_Before);
                }
                uint SpecilID = ContestDataManager.Instance().currentContestID;
                UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYER_QUERYSTATE);
                msg.Add(GetPlayerId());
                msg.Add((byte)gameMode);
                msg.Add(SpecilID);
                NetWorkClient.GetInstance().SendMsg(msg);

                DebugLog.Log("send request state:" + SpecilID);
            }
            else if (gameMode == GameTye_Enum.GameType_Appointment)
            {
                UMessage joinmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Join_2);
                joinmsg.Add(GameMain.hall_.GetPlayerId());
                joinmsg.Add(AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().roomid);
                NetWorkClient.GetInstance().SendMsg(joinmsg);
            }        }
        else
        {
            byte middleIn = 1;
            if (gameMode == GameTye_Enum.GameType_Normal)
                connect2CustomServer(gameKind, middleIn);
            else if (gameMode == GameTye_Enum.GameType_Contest)
            {
                if (contest == null)
                {                    contest = new Contest();
                    contest.SetCurrentContest(pd.nSpecilID_Before, pd.nSpecilID2_Before);
                }
                byte nState = 0;
                if (pd.nSpecilSign_Before == 1)
                    nState = 1;

                Connect2ContestCommonServer(gameKind, middleIn, pd.nSpecilID_Before, 0, 0, nState);
            }
            else if (gameMode == GameTye_Enum.GameType_Appointment)
            {
                Connect2AppointmentCommonServer(gameKind, middleIn, pd.nSpecilID_Before);

                if (gamerooms_ != null)
                {
                    if (gamerooms_.alreadyPanel != null)
                        gamerooms_.alreadyPanel.SetActive(false);
                }

            }
        }    }    bool BackForestDanceLogin(uint _msgType, UMessage _ms)    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_ForestDance);
        if (gamedata != null)
            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_ForestDance);
        else
        {
            Debug.Log("游戏id:0不存在");
            return false;
        }

        FD_DataCenter.GetInstance().condition.ReadData(_ms);

        return true;    }    public void EnterGameScene(GameKind_Enum gameId, GameTye_Enum gameType, uint timeLeft = 0, UnityAction callback = null)
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)gameId);
        if (gamedata != null)
        {            if (GameBaseObj == null)                SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, gameId, gameType, callback);
            //else
            //    GameBaseObj.GameMode = gameType;
            if(gameType == GameTye_Enum.GameType_Contest)
                MatchInGame.GetInstance().contestTimeLeft = timeLeft;
        }        else
            Debug.Log("游戏id:" + gameId + "不存在");
    }    bool BackCustomLogin(uint _msgType, UMessage _ms)    {        GameKind_Enum gameId = (GameKind_Enum)(_msgType / 100 % 1000);        ReadCustomData(gameId, _ms);

        EnterGameScene(gameId, GameTye_Enum.GameType_Normal);
        return true;    }    bool BackCustomContestLogin(uint _msgType, UMessage _ms)    {        GameKind_Enum gameId = (GameKind_Enum)(_msgType / 100 % 1000);        ReadCustomData(gameId, _ms);

        GameTye_Enum gameType = (GameTye_Enum)_ms.ReadByte();
        if (gameType == GameTye_Enum.GameType_Normal)
            EnterGameScene(gameId, gameType);        return true;    }    void ReadCustomData(GameKind_Enum gameId, UMessage _ms)
    {
        if (gameId == GameKind_Enum.GameKind_BlackJack)
            BlackJack_Data.GetInstance().ReadData(_ms);
        else if (gameId == GameKind_Enum.GameKind_BullHundred)
            BullHundred_Data.GetInstance().ReadData(_ms);
        else if (gameId == GameKind_Enum.GameKind_LandLords)
            LandLords_Data.GetInstance().ReadData(_ms);
        else if (gameId == GameKind_Enum.GameKind_Fishing)
            Fishing_Data.GetInstance().ReadData(_ms);
        else if (gameId == GameKind_Enum.GameKind_Mahjong || gameId == GameKind_Enum.GameKind_YcMahjong ||
                 gameId == GameKind_Enum.GameKind_CzMahjong || gameId == GameKind_Enum.GameKind_HongZhong)
            Mahjong_Data.GetInstance().ReadData(_ms);
        else if (gameId == GameKind_Enum.GameKind_GuanDan)
            GuanDan_Data.GetInstance().ReadData(_ms);
        else if(gameId == GameKind_Enum.GameKind_Answer)
            Answer_Data.GetInstance().ReadData(_ms);
    }    bool BackCarPortLogin(uint _msgType, UMessage _ms)    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata != null)
            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_CarPort);
        else
            Debug.Log("游戏id:0不存在");

        return true;    }    bool BackSlotLogin(uint _msgType, UMessage _ms)    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_LaBa);
        if (gamedata != null)
            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_LaBa);
        else
            Debug.Log("游戏id:2不存在");

        LB_DataCenter.Instance().LevelJoinCoinLimit.ReadData(_ms);

        return true;    }    bool BackFiveLogin(uint _msgType, UMessage _ms)    {        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_FiveInRow);        if (gamedata != null)            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_FiveInRow);        else            Debug.Log("游戏id:5不存在");        FIR_DataCenter.Instance().ReadRoomDataFromServer(_ms);        return true;    }    bool BackBullAllKillLogin(uint _msgType, UMessage _ms)
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullAllKill);        if (gamedata != null)            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_BullAllKill);        else            Debug.Log("游戏id:8不存在");        BAK_DataCenter.Instance().ReadLevelConfigFromServer(_ms);        return true;
    }    bool BackBullHappyLogin(uint _msgType, UMessage _ms)
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullHappy);        if (gamedata != null)            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_BullHappy);        else            Debug.Log("游戏id:11不存在");        HB_DataCenter.Instance().ReadLevelConfigFromServer(_ms);        return true;
    }    bool BackTexasLogin(uint _msgType, UMessage _ms)
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_TexasPoker);        if (gamedata != null)            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_TexasPoker);        else            Debug.Log("游戏id:6不存在");        TP_DataCenter.Instance().ReadRoomDataFromServer(_ms);        return true;
    }    bool BackDiuDiuLeGameScene(uint _msgType, UMessage _ms)
    {
        Ex_GameData.isReconect = true;
        Ex_SMGameScene ex_SMGameScene = new Ex_SMGameScene();
        ex_SMGameScene.usercardList = new List<byte>();
        ex_SMGameScene.outCardList = new List<byte>();
        uint myUid = _ms.ReadUInt();
        long myCarryInCoin = _ms.ReadLong();
        uint myStandardScore = _ms.ReadUInt();
        byte num = _ms.ReadByte();
        for (int i = 0; i < num; i++)
        {
            byte v = _ms.ReadByte();
            ex_SMGameScene.usercardList.Add(v);
        }
        uint otherUid = _ms.ReadUInt();
        string otherName = _ms.ReadString();
        uint otherFaceId = _ms.ReadUInt();
        long otherCarryInCoin = _ms.ReadLong();
        byte otherIsTruste = _ms.ReadByte();
        uint otherStandardScore = _ms.ReadUInt();
        byte otherNum = _ms.ReadByte();
        uint roomId = _ms.ReadUInt();
        byte roomLevel = _ms.ReadByte();
        byte roomType = _ms.ReadByte();
        uint curActionUid = _ms.ReadUInt();
        uint playCount = _ms.ReadUInt();
        uint scorePro = _ms.ReadUInt();
        uint standardScore = _ms.ReadUInt();
        uint rewardCoin = _ms.ReadUInt();
        uint maxNum = _ms.ReadUInt();
        uint addMultiple = _ms.ReadUInt();
        byte outNum = _ms.ReadByte();
        for (int i = 0; i < outNum; i++)
        {
            byte v = _ms.ReadByte();
            ex_SMGameScene.outCardList.Add(v);
        }
        ex_SMGameScene.myUid = myUid;
        ex_SMGameScene.myCarryInCoin = myCarryInCoin;
        ex_SMGameScene.myStandardScore = myStandardScore;
        ex_SMGameScene.otherUid = otherUid;
        ex_SMGameScene.otherName = otherName;
        ex_SMGameScene.otherFaceId = otherFaceId;
        ex_SMGameScene.otherCarryInCoin = otherCarryInCoin;
        ex_SMGameScene.otherIsTruste = otherIsTruste;
        ex_SMGameScene.otherStandardScore = otherStandardScore;
        ex_SMGameScene.otherNum = otherNum;
        ex_SMGameScene.areaId = roomType;
        ex_SMGameScene.regionId = roomLevel;
        ex_SMGameScene.curActionCID = curActionUid;
        ex_SMGameScene.playcount = playCount;
        ex_SMGameScene.scorePro = scorePro;
        ex_SMGameScene.standardScore = standardScore;
        ex_SMGameScene.rewardCoin = rewardCoin;
        ex_SMGameScene.maxNum = maxNum;
        ex_SMGameScene.addMultiple = addMultiple;
        Ex_GameData.SMGameScene = ex_SMGameScene;

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_DiuDiuLe);
        if (gamedata != null)
        {
            if (mainui_ == null)
            {
                CGame_DiuDiuLe ex_main = new CGame_DiuDiuLe();
                ex_main.Initialization();
                GameBaseObj = (CGameBase)ex_main;
            }
            else
            {
                SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_DiuDiuLe);
            }
        }

        else
            Debug.Log("游戏id:1不存在");

        return true;
    }    bool BackDiuDiuLeNoGameScene(uint _msgType, UMessage _ms)
    {
        Ex_SMNoGameScene ex_SMNoGameScene = new Ex_SMNoGameScene();
        long coin = _ms.ReadLong();
        ex_SMNoGameScene.coin = coin;
        GetPlayerData().SetCoin(coin);
        CGame_DiuDiuLe ex_main = new CGame_DiuDiuLe();
        ex_main.Initialization();
        GameBaseObj = (CGameBase)ex_main;

        return true;
    }    bool BackDiuDiuLeLogin(uint _msgType, UMessage _ms)    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_DiuDiuLe);
        if (gamedata != null)
            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_DiuDiuLe);
        else
            Debug.Log("游戏id:1不存在");

        return true;    }


    /// <summary>    /// 游戏图标按钮点击处理    /// </summary>    /// <param name="eventtype"></param>    /// <param name="parma"></param>    /// <param name="eventData"></param>    private void OnClickGameIconBtn(byte gameId, byte index)    {        AudioManager.Instance.PlaySound(GameDefine.HallAssetbundleName, "UIbutton01");        if (PickGameByID(gameId))            CurRoomIndex = index;    }

    private void OnClickGameIconBtnEvents(EventTriggerType eventtype, object parma, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            AudioManager.Instance.PlaySound(GameDefine.HallAssetbundleName, "UIbutton01");            byte gameId = (byte)parma;
            switch((GameKind_Enum)gameId)
            {
                case GameKind_Enum.GameKind_LandLords:
                case GameKind_Enum.GameKind_GuanDan:
                case GameKind_Enum.GameKind_Mahjong:
                case GameKind_Enum.GameKind_YcMahjong:
                case GameKind_Enum.GameKind_CzMahjong:
                case GameKind_Enum.GameKind_GouJi:
                case GameKind_Enum.GameKind_HongZhong:
                case GameKind_Enum.GameKind_Answer:
                case GameKind_Enum.GameKind_Chess:
                    {
                        if (gamerooms_ == null)
                            gamerooms_ = new GameRoom();
                        if (PickGameByID((byte)gameId, false))
                        {
                            gamerooms_.ShowAllAppointments((byte)gameId);
                        }
                    }
                    break;
                default:
                        OnClickRoomIconBtn(gameId, 1);
                        break;
            }        }    }

    public void OnClickRoomIconBtn(byte gameId, byte index)
    {
        if (PickGameByID(gameId))
            CurRoomIndex = index;
    }    private bool PickGameByID(byte gameid, bool request = true)    {
        //如果资源正在下载中
        if (CGameResDownloadMgr.Instance.isGameResDownloading(gameid))        {
            //Debug.Log("游戏资源还在下载中,游戏ID:" + gameid);
            CCustomDialog.OpenCustomConfirmUI(1016);            return false;        }

        //如果当前正在请求进入游戏状态，那么nothing to do
        if (request && PlayerObj.IsRequestEnterGameState())        {            return false;        }        bool needupdate = CheckGameResourceUpdate(gameid);
        //不需要更新直接请求进入游戏
        if (!needupdate)        {
            //有其它游戏资源正在下载，为避免网络受影响，不让进游戏
            if (!CGameResDownloadMgr.Instance.isAllGameResHaveDownOver())            {                CCustomDialog.OpenCustomConfirmUI(1016);                return false;            }            else if(request)            {                SendRequestEnterGame(gameid);                PlayerObj.ChangeRequestEnterGameState(true);            }        }        else        {            Debug.Log("游戏资源需要更新，游戏ID:" + gameid);            ShowGameResDownloadProcess(gameid);            return false;        }        return true;    }


    /// <summary>    /// 检查游戏资源是否需要更新,有更新刚进入到下载队列    /// </summary>    /// <param name="gameId">游戏ID</param>    /// <returns></returns>    public bool CheckGameResourceUpdate(byte gameId)    {        bool bneedUpdate = true;
        //先检测资源是否需要更新
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(gameId);        if (gamedata != null)        {            bneedUpdate = CResVersionCompareUpdate.CompareABVersionAndUpdate(gamedata.ResourceABName, true, gameId);            if (bneedUpdate)                CGameResDownloadMgr.Instance.AddGameResDownloadDic(gameId, gamedata.ResourceABName);            bool resupdate = CResVersionCompareUpdate.CompareABVersionAndUpdate(gamedata.SceneABName, true, gameId);            if (resupdate)                CGameResDownloadMgr.Instance.AddGameResDownloadDic(gameId, gamedata.SceneABName);            if (!bneedUpdate)                bneedUpdate = resupdate;        }        return bneedUpdate;
    }

    /// <summary>
    /// 下载完成的游戏资源进行Md5校验
    /// </summary>
    /// <returns></returns>
    private bool GameAssetBundleMd5CRC()
    {
        bool md5crcSuccssed = true;
        if(Luancher.EnableResMD5CRC)
        {
            List<DownloadEndGameAssetbundleInfo> bundlelist = CGameResDownloadMgr.Instance.DownloadOverbundleInfoList;
            for (int i = 0; i < bundlelist.Count; i++)
            {

                string filemd5str = GameCommon.GenerateFileMd5(GameDefine.AssetBundleSavePath + bundlelist[i].bundlename);
                CServerABVerData filesvrdata = CCsvDataManager.Instance.SerABVerDataMgr.GetServerABVerData(bundlelist[i].bundlename);

                if (filesvrdata == null)
                    continue;

                if (filemd5str.CompareTo(filesvrdata.AssetbundleMd5Str) != 0)
                {

                    Debug.Log(bundlelist[i].bundlename + " md5 CRC fialed");
                    File.Delete(GameDefine.AssetBundleSavePath + bundlelist[i].bundlename);
                    CResVersionCompareUpdate.CompareABVersionAndUpdate(bundlelist[i].bundlename, true, bundlelist[i].BelongGameId);

                    CGameResDownloadMgr.Instance.AddGameResDownloadDic(bundlelist[i].BelongGameId, bundlelist[i].bundlename);

                    ShowGameResDownloadProcess(bundlelist[i].BelongGameId);

                    md5crcSuccssed = false;
                }

            }
        }
        CGameResDownloadMgr.Instance.DownloadOverbundleInfoList.Clear();
        return md5crcSuccssed;
    }


    /// <summary>    /// 打开游戏ICON上的游戏资源下载进度条    /// </summary>    /// <param name="gameid">游戏id</param>    private void ShowGameResDownloadProcess(byte gameid)    {
        isDownloadGameAssetBundle = true;
        m_nCurGRDCount = 1;
        m_nCurTGRDCount =  CGameResDownloadMgr.Instance.GetDownloadGameResCount(gameid);
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(gameid);        if (gamedata != null)
            CCustomDialog.OpenCustomWaitUI("下载"+ gamedata.GameName + "("+ m_nCurGRDCount+"/"+ m_nCurTGRDCount+ ")" + "... 0%");    }

    /// <summary>    /// 更新游戏资源下载进度条    /// </summary>    private void UpdateGameResDownloadProcess()    {        byte curGameId = DownLoadProcessMgr.Instance.CurDownloadGameId;
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(curGameId);        if (gamedata == null)
            return;

        uint percent = DownLoadProcessMgr.Instance.GetDownloadPercent();
        int curGameResCount = m_nCurGRDCount + m_nCurTGRDCount - CGameResDownloadMgr.Instance.GetDownloadGameResCount(curGameId);
        CCustomDialog.OpenCustomWaitUI("下载" + gamedata.GameName +"(" + curGameResCount + "/" + m_nCurTGRDCount + ")"+ "... " + percent + "%");
        //List<byte> gameidlist = new List<byte>();
        //CGameResDownloadMgr.Instance.GetDownloadResOverGameIdList(ref gameidlist);
        //foreach (byte gameid in gameidlist)
        //{
        //    //Debug.Log("资源下载完成,GameID:" + gameid);
        //    HideGameResDownloadOverBar(gameid);
        //}

        if (CGameResDownloadMgr.Instance.isAllGameResHaveDownOver())
        {
            CGameResDownloadMgr.Instance.DownloadOverGameResIdList.Clear();
            CGameResDownloadMgr.Instance.GameResDownloadDic.Clear();

            //游戏assetbundle进行MD5校验            
            if (GameAssetBundleMd5CRC())
            {
                //把所有的下载显示都关掉
                HideGameResDownloadOverBar(0, true);
                isDownloadGameAssetBundle = false;

                GetGameIcon_ImageIcon(curGameId).Find("ImageBG").gameObject.SetActive(false);
            }

        }
    }


    /// <summary>    /// 该游戏资源下载完毕    /// </summary>    /// <param name="gameid"></param>    public void HideGameResDownloadOverBar(byte gameid, bool hidealldownBar = false)    {        if (hidealldownBar)            CCustomDialog.CloseCustomWaitUI();    }


    /// <summary>    /// 发送进入游戏请求    /// </summary>    /// <param name="gameid">游戏ID</param>    void SendRequestEnterGame(byte gameid)    {        UMessage app = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERAPPLYGAME);        MessageApplyGame mag_ = new MessageApplyGame();        mag_.nUseID = PlayerObj.GetPlayerId();        mag_.nKind = gameid;        mag_.SetSendData(app);        NetWorkClient.GetInstance().SendMsg(app);        CCustomDialog.OpenCustomWaitUI("正在进入房间...");    }    private IEnumerator LoadGameScene(string bundleName, string scenename, GameKind_Enum gameid, GameTye_Enum gameType, UnityAction callback = null)
    {
        PickGameByID((byte)gameid, false);        yield return new WaitWhile(() => isDownloadGameAssetBundle);        CurGameId = gameid;        CRollTextUI.Instance.SetRollTextTickPause(true);
        GameMain.ShowTicketResult(0);
        //CResVersionCompareUpdate.CompareABVersionAndUpdate(bundleName);
        //HttpDownload.DownFile(GameDefine.LuancherURL, GameDefine.AssetBundleSavePath, "car.scene");        
        AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, bundleName);        //进入异步加载场景
        isAsyncLoadScene = true;        fAsyncLoadProgress = 0f;        aAsyncSceneOperation = SceneManager.LoadSceneAsync(scenename);        aAsyncSceneOperation.allowSceneActivation = false;

        SceneActivationSetScreneOrientation();        if (gCurDisplayLoadSceneProgressUI != null)        {
            gCurDisplayLoadSceneProgressUI.SetActive(true);
            gCurDisplayLoadSceneProgressUI.transform.SetAsLastSibling();
        }        yield return aAsyncSceneOperation;
        //yield return new WaitForEndOfFrame();
        //Debug.Log("AsyncSceneOperation.progress:" + AsyncSceneOperation.progress);
        GameSceneLoadFinishCallBack(gameType, callback);    }


    /// <summary>    /// 更新异步加载场景进度条    /// </summary>    private void UpdateAsyncLoadSceneProcess()    {        if (aAsyncSceneOperation == null)
            return;        if (aAsyncSceneOperation.progress < 0.9f)            fAsyncLoadProgress = aAsyncSceneOperation.progress;        else            fAsyncLoadProgress += 0.11f;

        if (gCurDisplayLoadSceneProgressUI != null)        {            Image processbar = gCurDisplayLoadSceneProgressUI.transform.Find("ImageStripBG").Find("ImageStrip").gameObject.GetComponent<Image>();            processbar.fillAmount = fAsyncLoadProgress;        }        if (fAsyncLoadProgress > 1f)        {            isAsyncLoadScene = false;
            //Debug.Log("AsyncSceneOperation.progress:" + fAsyncLoadProgress);           
            //ScreneLoadFinishCallBack(sAsyncLoadSceneName);
            aAsyncSceneOperation.allowSceneActivation = true;
            //SceneActivationSetScreneOrientation();
        }    }

    /// <summary>
    /// 设置加载过程随机提示文本
    /// </summary>
    /// <param name="parenttf"></param>    private void SetRandomLoadingTipsText(UnityEngine.Transform parenttf)
    {
        //加个小提示文本
        AssetBundle hallbundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (hallbundle == null)
            return;

        System.Random rand = new System.Random();
        int tipsId = rand.Next(4001, 4020);

        TipsData tipsdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData((uint)tipsId);
        if (tipsdata == null)
            return;

        UnityEngine.Object Obj = hallbundle.LoadAsset<UnityEngine.Object>("Tips_lodingText");
        if (Obj)
        {
            GameObject tipsObj = GameMain.instantiate(Obj) as GameObject;
            Text tipstext = tipsObj.GetComponent<Text>();
            tipstext.text = tipsdata.TipsText;
            tipsObj.transform.SetParent(parenttf, false);
        }

    }    private void SceneActivationSetScreneOrientation()    {        if(gSceneLoadProgressLandscapeUI == null)
        {
            return;
        }        if (CurGameId == GameKind_Enum.GameKind_DiuDiuLe)        {            //gCurDisplayLoadSceneProgressUI = gSceneLoadProgressPortraitUI;
            gCurDisplayLoadSceneProgressUI = gSceneLoadProgressLandscapeUI;
            //设置屏幕自动旋转， 并置支持的方向
            //             if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            //                 Screen.orientation = ScreenOrientation.Portrait;
            //             Screen.autorotateToLandscapeLeft = false;
            //             Screen.autorotateToLandscapeRight = false;
            //             Screen.autorotateToPortrait = true;
            //             Screen.autorotateToPortraitUpsideDown = true;
            //             CanvasScaler cans = CanvasObj.GetComponent<CanvasScaler>();
            //             cans.referenceResolution = new Vector2(540, 960);
        }
        else        {
            //随机换张加载图
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallConstAssetBundleName);            if (bundle)            {                System.Random rand = new System.Random();                int randint = rand.Next(2);                Sprite loadUI = bundle.LoadAsset<Sprite>("cc_loadingBg" + randint.ToString());                gSceneLoadProgressLandscapeUI.GetComponent<Image>().sprite = loadUI;            }            bool bGameChessState = CurGameId == GameKind_Enum.GameKind_Chess;            gCurDisplayLoadSceneProgressUI = bGameChessState ? gSceneLoadProgressPortraitUI : gSceneLoadProgressLandscapeUI;            SetRandomLoadingTipsText(gCurDisplayLoadSceneProgressUI.transform);
            if(bGameChessState)
            {
                Screen.orientation = ScreenOrientation.Portrait;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = false;
                RefreshCanvasScaler(new Vector3(750,1334));
            }
        }    }

    void RefreshCanvasScaler(Vector2 ReferenceResolution)
    {
        CanvasScaler canvasScaler = GameMain.ddCanvas.GetComponent<CanvasScaler>();
        canvasScaler.referenceResolution = ReferenceResolution;
    }

    //游戏场景加载完成回调
    private bool GameSceneLoadFinishCallBack(GameTye_Enum gameType, UnityAction callback = null)    {        if (gameType == GameTye_Enum.GameType_Contest)            enGameState = GameState_Enum.GameState_Contest;
        else if(gameType == GameTye_Enum.GameType_Appointment)
            enGameState = GameState_Enum.GameState_Appointment;
        else
            enGameState = GameState_Enum.GameState_Game;        PlayerObj.ChangeRequestEnterGameState(false);        CRollTextUI.Instance.SetRollTextTickPause(false);        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)CurGameId);        if (gamedata == null)            return false;
        switch (CurGameId)        {            case GameKind_Enum.GameKind_CarPort:
                {
                    GameBaseObj = new CGame_CheHang();
                }
                break;
            case GameKind_Enum.GameKind_DiuDiuLe:
                {
                    GameBaseObj = new CGame_DiuDiuLe();
                }
                break;
            case GameKind_Enum.GameKind_LaBa:
                {
                    GameBaseObj = new CGame_LaBa();
                }
                break;
            case GameKind_Enum.GameKind_ForestDance:
                {
                    GameBaseObj = new CGame_SheLinWuHui();
                }
                break;
            case GameKind_Enum.GameKind_FiveInRow:
                {
                    GameBaseObj = new CGame_FiveInRow();
                }
                break;
            case GameKind_Enum.GameKind_BlackJack:
                {
                    GameBaseObj = new CGame_BlackJack();
                }
                break;
            case GameKind_Enum.GameKind_TexasPoker:
                {
                    GameBaseObj = new CGame_TexasPoker();
                }
                break;
            case GameKind_Enum.GameKind_LandLords:
                {
                    GameBaseObj = new CGame_LandLords(gameType);
                }
                break;
            case GameKind_Enum.GameKind_BullHundred:
                {
                    GameBaseObj = new CGame_BullHundred();
                }
                break;
            case GameKind_Enum.GameKind_BullAllKill:
                {
                    GameBaseObj = new CGame_BullAllKill();
                }
                break;
            case GameKind_Enum.GameKind_Fishing:
                {
                    GameBaseObj = new CGame_Fishing();
                }
                break;
            case GameKind_Enum.GameKind_BullHappy:
                {
                    GameBaseObj = new CGame_HappyBull();
                }
                break;            case GameKind_Enum.GameKind_Mahjong:
                {
                    GameBaseObj = new CGame_XzMahjong(gameType, CurGameId);
                }
                break;            case GameKind_Enum.GameKind_YcMahjong:
                {
                    GameBaseObj = new CGame_YcMahjong(gameType, CurGameId);
                }
                break;            case GameKind_Enum.GameKind_GuanDan:
                {
                    GameBaseObj = new CGame_GuanDan(gameType);
                }
                break;
            case GameKind_Enum.GameKind_CzMahjong:                {
                    GameBaseObj = new CGame_CzMahjong(gameType, CurGameId);                }                break;
            case GameKind_Enum.GameKind_LuckyTurntable:                {
                    GameBaseObj = new CGame_LuckyTurntable();
                }                break;
            case GameKind_Enum.GameKind_GouJi:                {
                    GameBaseObj = new CGame_GouJi(gameType);
                }                break;
            case GameKind_Enum.GameKind_HongZhong:                {
                    GameBaseObj = new CGame_HzMahjong(gameType, CurGameId);
                }
                break;
            case GameKind_Enum.GameKind_Answer:                {
                    GameBaseObj = new CGame_Answer(gameType);
                }                break;
            case GameKind_Enum.GameKind_Chess:                {
                    GameBaseObj = new CGame_Chess(gameType);
                }                break;        }
        if (AutoEnterGameMode)
        {
            ReconnectGameServer();
        }        if (GameBaseObj != null)
        {
            GameBaseObj.Initialization();
            //GameBaseObj.InitialCommonUI();
        }        if(gameType == GameTye_Enum.GameType_Contest)            GameMain.hall_.GetPlayerData().signedContests.Remove(ContestDataManager.Instance().currentContestID);        if (callback != null)            callback.Invoke();        return true;    }

    //返回大厅设置横屏或竖屏加载进度界面
    private void BackHallSetLandscapePortraitUI(bool isgameBack)    {        if (enGameState == GameState_Enum.GameState_Game || enGameState == GameState_Enum.GameState_Contest || enGameState == GameState_Enum.GameState_Appointment)        {            UnityEngine.Object obj = Resources.Load("Prefabs/Main_Loading");            gCurDisplayLoadSceneProgressUI = GameMain.instantiate(obj) as GameObject;            gCurDisplayLoadSceneProgressUI.transform.SetParent(GameObject.Find("Canvas/Root").transform, false);

            //             Screen.orientation = ScreenOrientation.LandscapeLeft;
            //             Screen.autorotateToLandscapeLeft = true;
            //             Screen.autorotateToLandscapeRight = true;
            //             Screen.autorotateToPortrait = false;
            //             Screen.autorotateToPortraitUpsideDown = false;
            //             CanvasScaler cans = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
            //             cans.referenceResolution = new Vector2(960, 540);

        }        else        {            gCurDisplayLoadSceneProgressUI = GameObject.Find("Canvas/Root").transform.Find("Main_Loading").gameObject;
        }
        if (Screen.orientation == ScreenOrientation.Portrait)
        {
            //设置屏幕自动旋转， 并置支持的方向
            Screen.orientation = ScreenOrientation.Landscape;//如果屏幕是竖屏,则立刻旋转至横屏
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
            RefreshCanvasScaler(new Vector3(1134, 750));
        }        if (isgameBack)
            SetRandomLoadingTipsText(gCurDisplayLoadSceneProgressUI.transform);    }

    //加载大厅场景
    private IEnumerator LoadHallScene(bool isGameBack, byte backState)    {        BackHallSetLandscapePortraitUI(isGameBack);        isAsyncLoadScene = true;        aAsyncSceneOperation = SceneManager.LoadSceneAsync(1);        aAsyncSceneOperation.allowSceneActivation = false;        yield return aAsyncSceneOperation;        if (isGameBack)        {            ClearRoomSerIndex();            CurRoomIndex = 0;            CCustomDialog.CloseCustomWaitUI();
            //UnityFactory.factory.Clear();
            //LoadHallResource();
            enGameState = GameState_Enum.GameState_Hall;            AfterLogin();
            UnloadGameAssetBundle();            GetPlayer().SetGameBackToHallState(false);            GameBaseObj = null;            CRollTextUI.Instance.SetRollTextTickPause(false);            if(backState == 0)
            {
                Go2MatchHall();
                if (AppointmentDataManager.AppointmentDataInstance().interruptid > 0)
                {
                    object[] param = { "<color=#bf442d>" + AppointmentDataManager.AppointmentDataInstance().interruptName  + "</color>" };
                    CCustomDialog.OpenCustomConfirmUIWithFormatParam(1653, param);
                    AppointmentDataManager.AppointmentDataInstance().interruptid = 0;
                }

                if(LB_DataCenter.Instance().isKickOut)
                {
                    CCustomDialog.OpenCustomConfirmUI(2004);
                    LB_DataCenter.Instance().isKickOut = false;
                }

                if(CH_DataCenter.Instance().isKickOut)
                {
                    CCustomDialog.OpenCustomConfirmUI(1017);
                    CH_DataCenter.Instance().isKickOut = false;
                }
            }            if (backState == 1)
            {
                enGameState = GameState_Enum.GameState_Login;
                CLoginUI.Instance.IntoLoginProcess();
            }            else if (backState == 2)
                HallBackToLoginUI();            else if (backState == 4)                Go2AppointmentHall();            else if (backState == 5)                Go2VideoPanel();            else if (backState == 6)                Go2Moments();        }        else
        {            CLoginUI.Instance.IntoLoginProcess();        }
    }    void Go2Moments()
    {
        AskForClubData();
        if (moments_ == null)
            moments_ = new FriendsMoments();
        moments_.ShowFriendsMoments();

        if (contestui_ != null)
            contestui_.SetActive(false);
        if (mainui_ != null)
            mainui_.SetActive(false);
        if (roomsui_ != null)
            roomsui_.SetActive(false);
    }    void Go2VideoPanel()
    {
        if (mainui_ != null)
            mainui_.SetActive(false);
        if (contestui_ != null)
            contestui_.SetActive(true);
        InitRoomsData();
        if(roomsui_ != null)
            roomsui_.SetActive(false);
        ShowGameRecord();
        //显示录像界面
        GameVideo.GetInstance().ShowRoundScore(true);
    }    void Go2AppointmentHall()
    {
        if(contestui_ != null)
            contestui_.SetActive(false);
        if(mainui_ != null)
            mainui_.SetActive(true);
        //InitRoomsData();
        //roomsui_.SetActive(true);
    }    public void LoadHallResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (roomLobbyUI_ != null)
        {
            roomLobbyUI_.SetActive(true);
            return;
        }

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("UI_Main_Lobby");
        roomLobbyUI_ = (GameObject)GameMain.instantiate(obj0);

        CanvasObj = GameObject.Find("Canvas/Root");
        roomLobbyUI_.transform.SetParent(CanvasObj.transform, false);

        GameObject createRoomBtn = roomLobbyUI_.transform.Find("Middle").Find("Button_CreateRoom").gameObject;
        XPointEvent.AutoAddListener(createRoomBtn, OnCreateRoom, null);
        GameObject joinRoomBtn = roomLobbyUI_.transform.Find("Middle").Find("Button_JoinRoom").gameObject;
        XPointEvent.AutoAddListener(joinRoomBtn, OnJoinRoom, null);
        GameObject gamecityBtn = roomLobbyUI_.transform.Find("Middle").Find("Button_PlayGame").gameObject;
        XPointEvent.AutoAddListener(gamecityBtn, OnGameCity, null);

        SetMainUIData(roomLobbyUI_);
    }

    void LoadRoomResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (roomui_ != null)
        {
            roomui_.SetActive(true);
            return;
        }

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("UI_Main_RoomSet");
        roomui_ = (GameObject)GameMain.instantiate(obj0);

        CanvasObj = GameObject.Find("Canvas/Root");
        roomui_.transform.SetParent(CanvasObj.transform, false);

        createroomui_ = roomui_.transform.Find("Middle").Find("CreateRoom").gameObject;
        joinroomui_ = roomui_.transform.Find("Middle").Find("JoinRoom").gameObject;

        GameObject returnBtn = roomui_.transform.Find("Top").Find("ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(returnBtn, OnReturn2Hall, null);

        InitCreateRoomEvents();
        InitRoomIdButtonsEvents();
        SetMainUIData(roomui_);
    }

    void InitCreateRoomEvents()
    {
        GameObject confirm = createroomui_.transform.Find("RuleBG").
            Find("UI_RoomSet_Rule7").Find("ButtonOK").gameObject;
        XPointEvent.AutoAddListener(confirm, CreateRoom, null);
    }

    //设置创建房间时消耗房卡数量
    void SetRoomCardNumber(int number)
    {
        GameObject confirm = createroomui_.transform.Find("RuleBG").
            Find("UI_RoomSet_Rule7").Find("ButtonOK").gameObject;

        Text cardNumber = confirm.transform.Find("Text_Num").gameObject.GetComponent<Text>();
        cardNumber.text = number.ToString();
    }

    void CreateRoom(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            SendCreateRoomMsg();
        }
    }

    void SendCreateRoomMsg()
    {
        //发送创建房间消息
    }

    void InitRoomIdButtonsEvents()
    {
        GameObject keyboard = joinroomui_.transform.Find("ImageBG").Find("ButtonGroup").gameObject;
        for (int index = 0; index < 12; index++)
        {
            GameObject button = null;
            if (index < 10)
                button = keyboard.transform.Find("Button_" + index.ToString()).gameObject;
            else
            {
                if (index == 10)
                    button = keyboard.transform.Find("Button_ChongShu").gameObject;
                if (index == 11)
                    button = keyboard.transform.Find("Button_ShanChu").gameObject;
            }
            int btnnum = index;
            XPointEvent.AutoAddListener(button, OnClickKeyBoard, btnnum);
        }
    }

    void OnClickKeyBoard(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            int btnnum = (int)button;
            InputField roomNo = joinroomui_.transform.Find("ImageBG").
                Find("InputField").gameObject.GetComponent<InputField>();
            if (btnnum < 10)
            {
                if (roomNo.text.Length < 6)
                    roomNo.text += btnnum.ToString();
                else
                    SendJoinRoomMsg();
            }
            else
            {
                if (btnnum == 10)
                    roomNo.text = "";
                if (btnnum == 11)
                    roomNo.text = roomNo.text.Substring(0, roomNo.text.Length - 1);
            }
        }
    }

    void SendJoinRoomMsg()
    {
        //发送加入房间消息
    }

    void OnReturn2Hall(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            Return2Hall();
        }
    }

    void Return2Hall()
    {
        if (roomui_ != null && roomui_.activeSelf)
            roomui_.SetActive(false);
        if (mainui_ != null && mainui_.activeSelf)
            mainui_.SetActive(true);
        //roomLobbyUI_.SetActive(true);
    }

    void OnCreateRoom(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //roomLobbyUI_.SetActive(false);
            LoadRoomResource();

            createroomui_.SetActive(true);
            joinroomui_.SetActive(false);
        }
    }

    void OnJoinRoom(EventTriggerType eventtype, object button, PointerEventData eventData)
    {        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //roomLobbyUI_.SetActive(false);
            LoadRoomResource();

            createroomui_.SetActive(false);
            joinroomui_.SetActive(true);
        }
    }

    void OnGameCity(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            roomLobbyUI_.SetActive(false);
            AfterLogin();
        }
    }
    //返回到大厅场景后卸载游戏资源
    private void UnloadGameAssetBundle()    {        if (CurGameId == GameKind_Enum.GameKind_Max)            return;        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)CurGameId);        if (gamedata != null)        {            AssetBundleManager.UnloadAssetBundle(gamedata.SceneABName);            AssetBundleManager.UnloadAssetBundle(gamedata.ResourceABName);
            //Debug.Log("返回到大厅场景后卸载游戏资源");
        }

    }

    /// <summary>    /// 切换大厅场景    /// </summary>    /// <param name="isGameBack"></param>    /// <param name="backState">0->正常返回大厅 1->掉线重连 2->顶号返回登陆界面</param>    public void SwitchToHallScene(bool isGameBack = true, byte backState = 0)    {        if (isAsyncLoadScene)            return;        if (isGameBack)        {            UnityFactory.factory.Clear();            if (GetPlayer().IsInGameBackToHall())                return;                      AudioManager.Instance.PlayBGMusic(GameDefine.HallAssetbundleName, "hall");            GetPlayer().SetGameBackToHallState(true);            PlayerInfoUI.Instance.ResetInitIconFlag();            CRollTextUI.Instance.SetRollTextTickPause(true);
        }        GameMain.SC(LoadHallScene(isGameBack, backState));    }

    //切换游戏场景
    void SwitchGameScene(string abname, string scenename, GameKind_Enum gameid, GameTye_Enum gameType = GameTye_Enum.GameType_Normal, UnityAction callback = null)
    {        if (isAsyncLoadScene)            return;        GameMain.SC(LoadGameScene(abname, scenename, gameid, gameType, callback));
    }    void SwithGameStateToLogin()    {        if (enGameState == GameState_Enum.GameState_Luancher)        {            enGameState = GameState_Enum.GameState_Login;            SwitchToHallScene(false);
        }    }    void SetMainUIData(GameObject root, bool needicon = true)    {        if(needicon)
        {
            GameObject IconBtn = root.transform.Find("PanelHead_").Find("Image_HeadBG").Find("Image_HeadFrame").gameObject;
            XPointEvent.AutoAddListener(IconBtn, ClickPlayerIconBtn, null);

            Image icon = root.transform.Find("PanelHead_").Find("Image_HeadBG").Find("Image_HeadMask").Find("Image_HeadImage").gameObject.GetComponent<Image>();
            icon.sprite = GetIcon(PlayerObj.GetPlayerData().GetPlayerIconURL(), 
                GameMain.hall_.GetPlayerId(), (int)PlayerObj.GetPlayerData().PlayerIconId);
            Text name = root.transform.Find("PanelHead_").Find("Image_NameBG").Find("Text_Name").gameObject.GetComponent<Text>();
            name.text = GetPlayerData().GetPlayerName();

            Text id = root.transform.Find("PanelHead_/Image_NameBG/Text_ID").gameObject.GetComponent<Text>();
            id.text = GetPlayerData().GetPlayerID().ToString();

            Text diamond = root.transform.Find("PanelHead_").Find("Image_DiamondFrame").Find("Text_Diamond").gameObject.GetComponent<Text>();
            diamond.text = (GetPlayerData().GetDiamond() + GetPlayerData().GetCoin()).ToString();

            RefreshPlayerVipText();
        }        else
        {
            Text diamond = root.transform.Find("PanelHead_").Find("Image_DiamondFrame").Find("Text_Diamond").gameObject.GetComponent<Text>();
            diamond.text = GetPlayerData().GetDiamond().ToString();
        }        //Text coin = root.transform.FindChild("PanelHead_").FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();        //coin.text = GetPlayerData().GetCoin().ToString();

        //Text Lotterytext = root.transform.FindChild("PanelHead_/Image_TicketFrame/Text_Ticket").gameObject.GetComponent<Text>();
        //Lotterytext.text = GetPlayerData().GetLottery().ToString();

        GameObject diamondBtn = root.transform.Find("PanelHead_").Find("Image_DiamondFrame").gameObject;        XPointEvent.AutoAddListener(diamondBtn, Charge, Shop.SHOPTYPE.SHOPTYPE_DIAMOND);        //GameObject coinBtn = root.transform.FindChild("PanelHead_").FindChild("Image_coinframe").gameObject;        //XPointEvent.AutoAddListener(coinBtn, Charge, Shop.SHOPTYPE.SHOPTYPE_COIN);

        //GameObject lotteryBtn = root.transform.FindChild("PanelHead_").FindChild("Image_TicketFrame").gameObject;        //XPointEvent.AutoAddListener(lotteryBtn, Charge, Shop.SHOPTYPE.SHOPTYPE_EXCHANGE);

        /*if (GirlAnimationTimer == null)        {
           GirlAnimationTimer = new CTimerCirculateCall(3.0f, PlayGirlAnimation);
           xTimeManger.Instance.RegisterTimer(GirlAnimationTimer);
        }*/
    }

    

    private void OnRetruen2Main(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (roomsui_ != null)
                roomsui_.SetActive(false);
            if(mainui_ != null)
                mainui_.SetActive(false);
            if(contestui_ != null)
                contestui_.SetActive(true);
        }    }

    private void OnClickMatch(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            Go2MatchHall();        }    }

    //加载房间资源
    //public void LoadRoomsResource()
    //{
    //    AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
    //    if (bundle == null)
    //        return;

    //    if (roomsui_ != null)
    //    {
    //        roomsui_.SetActive(true);

    //        if (contestui_ != null)
    //            contestui_.SetActive(false);
    //        return;
    //    }

    //    UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Lobby_Room");
    //    roomsui_ = (GameObject)GameMain.instantiate(obj0);

    //    CanvasObj = GameObject.Find("Canvas/Root");
    //    roomsui_.transform.SetParent(CanvasObj.transform, false);
    //    roomsui_.SetActive(false);
    //    if (contestui_ != null)    //        contestui_.SetActive(false);

    //    InitRoomsEvents();
    //    InitRoomsData();
    //}

    public void InitRoomsData()
    {
        if (roomsui_ == null)
        {
            if (gamerooms_ == null)
                gamerooms_ = new GameRoom();
            else
                gamerooms_.InitGameRoom();
        }

        //Text coin = roomsui_.transform.FindChild("Top").FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();        //coin.text = GetPlayerData().GetCoin().ToString();        //Text diamond = roomsui_.transform.FindChild("Top").FindChild("Image_DiamondFrame").FindChild("Text_Diamond").gameObject.GetComponent<Text>();        //diamond.text = (GetPlayerData().GetDiamond() + GetPlayerData().GetCoin()).ToString();
    }

    void InitRoomsEvents()
    {
        GameObject returnBtn = roomsui_.transform.Find("Top").Find("ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(returnBtn, OnRetruen2Main, null);

        GameObject createBtn = roomsui_.transform.Find("middle").Find("Button_chuangjian").gameObject;
        XPointEvent.AutoAddListener(createBtn, OnCreateRooms, null);

        GameObject joinBtn = roomsui_.transform.Find("middle").Find("Button_jiaru").gameObject;
        XPointEvent.AutoAddListener(joinBtn, OnJoinRooms, null);

        GameObject diamondBtn = roomsui_.transform.Find("Top").Find("Image_DiamondFrame").gameObject;        XPointEvent.AutoAddListener(diamondBtn, Charge, Shop.SHOPTYPE.SHOPTYPE_DIAMOND);        //GameObject coinBtn = roomsui_.transform.FindChild("Top").FindChild("Image_coinframe").gameObject;        //XPointEvent.AutoAddListener(coinBtn, Charge, Shop.SHOPTYPE.SHOPTYPE_COIN);

        //GameObject recordBtn = roomsui_.transform.FindChild("middle").FindChild("Button_record").gameObject;
        //XPointEvent.AutoAddListener(recordBtn, OnClickAppointmentRecord, null);
    }

    public void OnClickAppointmentRecord(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            if(m_RecordButton == null)
            {
                return;
            }

            if(!m_RecordButton.interactable)
            {
                return;
            }
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            ShowGameRecord();            m_RecordButton.interactable = false;        }    }

    void ShowGameRecord()
    {
        if (gamerooms_ == null)
            gamerooms_ = new GameRoom();

        UMessage ask4videoip = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_APPLYVIDEOSERIPPORT);
        NetWorkClient.GetInstance().SendMsg(ask4videoip);

        if (m_eGameRecordMsgState == GameRecordMsgState.RecordMsg_End)
        {
            gamerooms_.ShowAppointmentRecord();
        }
    }

    /// <summary>
    /// 设置录像消息进度状态
    /// </summary>
    /// <param name="recordMsgState">录像数据消息进度类型</param>
    public void SetGameRecordMsgState(GameRecordMsgState recordMsgState)
    {
        m_eGameRecordMsgState = recordMsgState;
        if(m_eGameRecordMsgState == GameRecordMsgState.RecordMsg_End && m_RecordButton)
        {
            m_RecordButton.interactable = true;
        }
    }

    bool BackVideoServerIp(uint msgType, UMessage msg)    {        string ip = msg.ReadString();        int port = msg.ReadInt();        if (videotcpclient == null)            videotcpclient = new NetWorkClient();        else if (!videotcpclient.IsSameIpAndPort(ip, port))        {            videotcpclient.CloseNetwork();            videotcpclient = new NetWorkClient();        }

        if (m_RecordButton && m_eGameRecordMsgState == GameRecordMsgState.RecordMsg_End)
        {
            m_RecordButton.interactable = true;
        }

        if (m_eGameRecordMsgState == GameRecordMsgState.RecordMsg_Default)
        {
            UMessage ask4AppointmentRecord = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Record);
            ask4AppointmentRecord.Add(GameMain.hall_.GetPlayerId());
            NetWorkClient.GetInstance().SendMsg(ask4AppointmentRecord);
            m_eGameRecordMsgState = GameRecordMsgState.RecordMsg_Begin;
        }
        bool isconn = videotcpclient.InitNetWork(ip, port);        if (!isconn)        {            CCustomDialog.OpenCustomConfirmUI(2007);            return false;        }        return true;    }

    private void OnCreateRooms(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            GameObject createPanel = roomsui_.transform.Find("Pop-up").Find("Room_rule").gameObject;            createPanel.SetActive(true);        }    }

    private void OnJoinRooms(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            GameObject joinPanel = roomsui_.transform.Find("Pop-up").Find("Room_number").gameObject;            joinPanel.SetActive(true);        }    }

    //自建比赛
    private void OnClickSelfMatch(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if (selfcontest_ == null)
                selfcontest_ = new SelfContest();

            selfcontest_.Ask4SelfContestList();
        }
    }

    private void OnClickRomm(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //AppointmentDataManager.AppointmentDataInstance().kind = AppointmentKind.From_Appointment;
            //LoadRoomsResource();
            //contestui_.SetActive(false);
            //roomsui_.SetActive(true);

            //if (gamerooms_ == null)
            //    gamerooms_ = new GameRoom(roomsui_);
            //else
            //    gamerooms_.InitGameRoom(roomsui_);
        }    }

    //进入匹配厅
    void Go2MatchHall()
    {
        //if (contestui_ != null)
        //    contestui_.SetActive(false);
        if(roomsui_ != null)
            roomsui_.SetActive(false);
        //InitGamePanel();
        //mainui_.SetActive(true);
    }

    //进入比赛厅
    public void Go2ContestHall(byte ContestType = 1)
    {
        if (contest == null)
            contest = new Contest();

        //UMessage ask4ContestDataMsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_RequestContestInfo);

        //ask4ContestDataMsg.Add((byte)1);

        //NetWorkClient.GetInstance().SendMsg(ask4ContestDataMsg);
        contest.Go2ContestHall(ContestType);
    }

    private void OnClickTournament(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            int ContestType = (int)button;
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            Go2ContestHall((byte)ContestType);        }    }

    //播放美女动作
    void PlayGirlAnimation(object[] args)
    {
        if (enGameState == GameState_Enum.GameState_Hall)
        {
            GameObject girlAnimateObj = mainui_.transform.Find("Image_Show").Find("Anime_Lobbt_meinv").gameObject;
            UnityArmatureComponent girlAnimate = girlAnimateObj.GetComponent<UnityArmatureComponent>();
            girlAnimate.animation.Play("newAnimation");
        }
    }    private void ClickPlayerIconBtn(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            PlayerInfoUI.Instance.OpenOrClosePlayerInfoUI(true);        }    }    public void Charge(EventTriggerType eventtype, object param, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            GameObject root = roomLobbyUI_;
            if (mainui_ != null && mainui_.activeSelf)
                root = mainui_;
            if (roomui_ != null && roomui_.activeSelf)
                root = roomui_;

            Shop.SHOPTYPE _type = (Shop.SHOPTYPE)param;            if (shop == null)            {                shop = new Shop(CanvasObj);                           }            shop.InitShopUI();            shop.ShowKindsShopUI(_type);            //shop.ChangeToggle(_type);            shop.OpenOrCloseShopMainUI(true);        }    }    public static void AdaptiveUI(GameObject canvasObj = null)
    {
#if UNITY_IPHONE || UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Screen.width == 2436 && Screen.height == 1125)
        {
            if (canvasObj == null)
                canvasObj = GameObject.Find("Canvas/Root");

            RectTransform rectTransform = (canvasObj.transform as RectTransform);
            rectTransform.offsetMin = new Vector2(44f, 0f);
            rectTransform.offsetMax = new Vector2(-44f, 0f);
        }
#endif
    }    void SetModelSex(byte male)//1为男性，2为女性
    {
        GameObject ModelObj = GameObject.Find("3d_point/Model");        if (ModelObj == null)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle("pokercommon.resource");        if (bundle == null)            return;

        foreach (UnityEngine.Transform child in ModelObj.transform)
            GameObject.Destroy(child.gameObject);
        UnityEngine.Object asset = bundle.LoadAsset(male == 1 ? "3Dmodel_nan_1" : "3Dmodel_nv_1");
        GameObject obj = (GameObject)GameMain.instantiate(asset);        obj.transform.SetParent(ModelObj.transform, false);    }    public void AfterLogin()
    {
        if (enGameState == GameState_Enum.GameState_Game || enGameState == GameState_Enum.GameState_Appointment
            || enGameState == GameState_Enum.GameState_Contest)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle != null)        {            if (contestui_ == null)            {                UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("UI_MainLobby");                contestui_ = (GameObject)GameMain.instantiate(obj0);                CanvasObj = GameObject.Find("Canvas/Root");                contestui_.transform.SetParent(CanvasObj.transform, false);

                GameObject tournamentObj = contestui_.transform.Find("PanelGame_").Find("Scroll_Game").Find("Button_tournament").gameObject;
                XPointEvent.AutoAddListener(tournamentObj, OnClickTournament,1);

                GameObject matchObj = contestui_.transform.Find("PanelGame_").Find("Scroll_Game").Find("Button_match").gameObject;
                XPointEvent.AutoAddListener(matchObj, OnClickMatch, null);
                matchObj.SetActive(false);

                GameObject roomObj = contestui_.transform.Find("PanelGame_").Find("Scroll_Game").Find("Button_room").gameObject;
                //XPointEvent.AutoAddListener(roomObj, OnClickRomm, null);
                XPointEvent.AutoAddListener(roomObj, onClickClub, null);

                GameObject selfMatchObj = contestui_.transform.Find("PanelGame_").Find("Scroll_Game").Find("Button_Create").gameObject;
                XPointEvent.AutoAddListener(selfMatchObj, OnClickSelfMatch, null);

                UnityEngine.Transform tfm = contestui_.transform.Find("Pop-up/Set/ImageBG");
                Slider music = tfm.Find("Slider_Music").GetComponent<Slider>();
                Slider sound = tfm.Find("Slider_Sound").GetComponent<Slider>();
                music.value = AudioManager.Instance.MusicVolume;
                sound.value = AudioManager.Instance.SoundVolume;
                music.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.MusicVolume = value; });
                sound.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.SoundVolume = value; });

                GameObject obj = (GameObject)bundle.LoadAsset("Lobby_News");
                obj = (GameObject)GameMain.instantiate(obj);
                obj.transform.SetParent(CanvasObj.transform, false);
                m_Bulletin = obj.AddComponent<Bulletin>();
                m_Bulletin.gameObject.SetActive(false);

                AdaptiveUI(CanvasObj);
                InitContestUIEvents();
                SetModelSex(GetPlayerData().PlayerSexSign);
            }

            if (gSceneLoadProgressPortraitUI == null)
            {
                UnityEngine.Object obj0 = Resources.Load("Prefabs/Main_Loading_shuping");                gSceneLoadProgressPortraitUI = (GameObject)GameMain.instantiate(obj0);
                gSceneLoadProgressPortraitUI.transform.SetParent(CanvasObj.transform, false);
                gSceneLoadProgressPortraitUI.transform.SetAsFirstSibling();
            }
        }
        enGameState = GameState_Enum.GameState_Hall;
        //SetMainUIData(contestui_);
        contestui_.SetActive(true);
        //GameObject openrolltextbtn = contestui_.transform.FindChild("Panelbottom").FindChild("Button_horn").gameObject;
        //XPointEvent.AutoAddListener(openrolltextbtn, )
        //CTrumpetUI.Instance.InitTrumpetButtenEvent("UI_MainLobby(Clone)/Panelbottom/Button_horn");
        gSceneLoadProgressLandscapeUI = CanvasObj.transform.Find("Main_Loading").gameObject;
        m_bIsAfterLogin = true;

        contestui_.transform.Find("Panelbottom/Bottom/Button_Bonus/TextNum").GetComponent<Text>().text = GetPlayerData().UnreceivedRedBag.ToString();
        contestui_.transform.Find("Panelbottom/Bottom/Button_News/ImageSpot").gameObject.SetActive(GetPlayerData().ChessContestNumber > 0);

        InitGamePanel();
        InitGameDocumentPanel();
    }

    void InitGameDocumentPanel()
    {
        if(DocumentPanel == null)
        {
            DocumentPanel = new GameDocumentPanel();
        }
        DocumentPanel.InitGameDocumentPanelResource();
    }


    void InitContestUIEvents()
    {
        GameObject clubBtn = contestui_.transform.Find("Panelbottom/Bottom/Button_Club").gameObject;        XPointEvent.AutoAddListener(clubBtn, onClickClub, null);

        GameObject bagBtn = contestui_.transform.Find("Panelbottom/Bottom/Button_Goods").gameObject;
        XPointEvent.AutoAddListener(bagBtn, onClickBag, null);

        GameObject newsBtn = contestui_.transform.Find("Panelbottom/Bottom/Button_News").gameObject;
        XPointEvent.AutoAddListener(newsBtn, onClickNews, null);

        GameObject recordBtn = contestui_.transform.Find("Panelbottom/Bottom/Button_record").gameObject;
        m_RecordButton = recordBtn.GetComponent<Button>();
        XPointEvent.AutoAddListener(recordBtn, OnClickAppointmentRecord,null);

        GameObject boundsBtn = contestui_.transform.Find("Panelbottom/Bottom/Button_Bonus").gameObject;
        XPointEvent.AutoAddListener(boundsBtn, OnOpenBounds, null);

        GameObject rankBtn = contestui_.transform.Find("Panelbottom/Bottom_Button/Button_ranking").gameObject;
        XPointEvent.AutoAddListener(rankBtn, OnShowMasterRankPanel, null);

        GameObject ruleBtn = contestui_.transform.Find("Panelbottom/Bottom/Button_rule").gameObject;
        XPointEvent.AutoAddListener(ruleBtn, OnShowGameDocumentPanel, null);
    }

    void OnShowGameDocumentPanel(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            DocumentPanel.SetGameDocumentActivePanel(true,gameicons_);
        }
    }

    private void OnShowMasterRankPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            MasterRank.GetMasterRankInstance().ShowMasterRankPanel();        }    }

    void OnOpenBounds(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //打开红包界面
            if (crb_ == null)                crb_ = new CrashRedBag();            crb_.ShowCrashPanel();        }    }

    //void InitMainPanelUIData()
    //{
    //    Image icon = contestui_.transform.FindChild("PanelHead_").FindChild("Image_HeadBG").
    //        FindChild("Image_HeadMask").FindChild("Image_HeadImage").gameObject.GetComponent<Image>();
    //    icon.sprite = GetHostIconByID(PlayerObj.GetPlayerData().PlayerIconId.ToString());
    //    GameObject iconbtn = contestui_.transform.FindChild("PanelHead_").FindChild("Image_HeadBG").
    //        FindChild("Image_HeadFrame").gameObject;
    //    XPointEvent.AutoAddListener(iconbtn, ClickPlayerIconBtn, null);
    //    Text name = contestui_.transform.FindChild("PanelHead_").FindChild("Image_NameBG").
    //        FindChild("Text_Name").gameObject.GetComponent<Text>();
    //    name.text = PlayerObj.GetPlayerData().GetPlayerName();
    //    RefreshPlayerVipText();
    //    Text coin = contestui_.transform.FindChild("PanelHead_").FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();
    //    coin.text = GetPlayerData().GetCoin().ToString();
    //    Text diamond = contestui_.transform.FindChild("PanelHead_").FindChild("Image_DiamondFrame").FindChild("Text_Diamond").gameObject.GetComponent<Text>();
    //    diamond.text = GetPlayerData().GetDiamond().ToString();
    //}

    void InitGamePanel()    {        PlayerObj.ChangeRequestEnterGameState(false);        //gameicons_.Clear();

        //GameMain.WaitForCall(-1f, () =>
        //{
            //foreach (byte key in gameicons_.Keys)
                //GameMain.safeDeleteObj(gameicons_[key]);
           // gameicons_.Clear();

            PlayerData pd = GetPlayerData();
            for (int i = 0; i < pd.GameList.Count; i++)
            {
                InitGameIcon(pd.GameList[i]);
            }

            GameMain.WaitForCall(-1f, () => InitHallUIBtnListener());
        //});        SetMainUIData(contestui_);
        GameMain.LoadTicketResult();    }    public void ShowRelief()    {        ShowReliefInterface(m_iAddReliefCoin, m_iLeftReliefNum);        isGetRelief = false;    }    public bool isGameRelief = false;


    /// <summary>    /// 服务器通知客户端增加救济金了    /// </summary>    bool BackAddRelief(uint _msgType, UMessage _ms)    {        m_iLeftReliefNum = _ms.ReadByte();        m_iAddReliefCoin = _ms.ReadUInt();        GetPlayerData().AddCoin(m_iAddReliefCoin);        if (m_bIsAfterLogin)        {            isGetRelief = true;            if (gametcpclient == null)            {                ShowRelief();            }            else            {                isGameRelief = true;            }        }        return true;    }

    /// <summary>    /// 服务器通知客户端之前所在的游戏结束了    /// </summary>    bool BackBeforeGameOver(uint _msgType, UMessage _ms)    {        GetPlayerData().nGameKind_Before = 0;        CCustomDialog.CloseCustomWaitUI();        return true;    }    bool BackLottery(uint _msgType, UMessage _ms)    {        long addLottery = _ms.ReadLong();        GameMain.ShowTicketResult(addLottery);        return true;    }

    /// <summary>    /// 服务器通知客户端跑马灯    /// </summary>    bool RunHorseLightData(uint _msgType, UMessage _ms)    {        byte nGameKind = _ms.ReadByte();        byte nGameLevel = _ms.ReadByte();        byte nGameMode = _ms.ReadByte();        long nGet = _ms.ReadLong();        string sName = _ms.ReadString();        if (nGameKind == (byte)GameKind_Enum.GameKind_ForestDance)        {            if (nGameMode == (byte)GameCity.ForestMode_Enum.ForestMode_Handsel)            {                CRollTextUI.Instance.AddHorizontalRollText(3001, sName, nGet);            }            else if (nGameMode == (byte)GameCity.ForestMode_Enum.ForestMode_Three)            {                CRollTextUI.Instance.AddHorizontalRollText(3002, sName, nGet);            }            else if (nGameMode == (byte)GameCity.ForestMode_Enum.ForestMode_Four)            {                CRollTextUI.Instance.AddHorizontalRollText(3003, sName, nGet);            }            else if (nGameMode == (byte)GameCity.ForestMode_Enum.ForestMode_GiveGun)            {                CRollTextUI.Instance.AddHorizontalRollText(3004, sName, nGet);            }        }        else if (nGameKind == (byte)GameKind_Enum.GameKind_LaBa)        {            CRollTextUI.Instance.AddHorizontalRollText(3005, sName, nGet);        }        else if (nGameKind == (byte)GameKind_Enum.GameKind_BullHundred)        {            CRollTextUI.Instance.AddHorizontalRollText(3008, sName, nGet);        }        else if (nGameKind == (byte)GameKind_Enum.GameKind_BullAllKill)        {            CRollTextUI.Instance.AddHorizontalRollText(3007, sName, nGet);        }        else if (nGameKind == (byte)GameKind_Enum.GameKind_Fishing)        {            if (nGameMode == (byte)FishType_Enum.FishType_ScreenBomb)            {                CRollTextUI.Instance.AddHorizontalRollText(3009, sName, nGet);            }            else if (nGameMode == (byte)FishType_Enum.FishType_Lottery)            {                CRollTextUI.Instance.AddHorizontalRollText(3010, sName, nGet);            }        }        Debug.Log("Get server run horse light! gamekind:" + nGameKind.ToString() +            "mode:" + nGameMode.ToString() +            "name:" + sName +            "getcoin:" + nGet.ToString());        return true;    }







    /// <summary>    /// 服务器通知客户端关闭服务器    /// </summary>    bool GmCloseAllConnect(uint _msgType, UMessage _ms)    {        Debug.Log("Gm close all connect!");        CCustomDialog.OpenCustomConfirmUI(1013, ServerCloseByGmOrder);        return true;    }



    //大厅状态断线处理
    public void ServerCloseByGmOrder(object param)    {        GameMain.Instance.ExitApplication(1);    }

    /// <summary>    /// 展示救济金界面    /// </summary>    void ShowReliefInterface(uint nAddCoin, int nLeftNum)    {        if (enGameState == GameState_Enum.GameState_Login)            return;        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)            return;        if (CanvasObj == null)            CanvasObj = GameObject.Find("Canvas/Root");        if (null == m_objRelief)        {            UnityEngine.Object tipsobj = bundle.LoadAsset("Tips_Jiujijin");            m_objRelief = (GameObject)GameMain.instantiate(tipsobj);            XPointEvent.AutoAddListener(m_objRelief.transform.Find("ImageBG").Find("ButtonOk").gameObject, OnClickReliefOk, null);        }

        //GameObject background = GameObject.Find("Canvas/Root");
        m_objRelief.transform.SetParent(CanvasObj.transform, false);        m_objRelief.SetActive(true);        Text strLeft = m_objRelief.transform.Find("ImageBG").Find("Text").Find("TextTime").Find("Num").gameObject.GetComponent<Text>();        strLeft.text = nLeftNum.ToString();        Text strAdd = m_objRelief.transform.Find("ImageBG").Find("Text").Find("TextNum").gameObject.GetComponent<Text>();        strAdd.text = nAddCoin.ToString();        m_iAddReliefCoin = 0;        m_iLeftReliefNum = 0;    }    void OnClickReliefOk(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype != EventTriggerType.PointerUp)            return;        AudioManager.Instance.PlaySound(GameDefine.HallAssetbundleName, "UIbutton02");        m_objRelief.SetActive(false);    }

    /// <summary>    /// 发送消息到login 获取金钱排行榜    /// </summary>    public void SendGetCoinRankData(short nBeginSign = 0)    {        UMessage LoginMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERGETCOINRANK);        LoginMsg.Add(nBeginSign);        NetWorkClient.GetInstance().SendMsg(LoginMsg);    }    bool BackPeopleNumber(uint _msgType, UMessage _ms)
    {
        if (HallMain.gametcpclient != null)
            return false;

        PlayerData pd = GetPlayerData();
        byte length = _ms.ReadByte();
        //for (GameKind_Enum index = GameKind_Enum.GameKind_CarPort; index < GameKind_Enum.GameKind_Max; index++)
        for (byte index = 0; index < length; index++)
        {
            if (pd.GameList.Find(s => s.gameId ==index) == null)
            {
                _ms.ReaduShort();
                continue;
            }

            ushort number = _ms.ReaduShort();
            pd.peopleNumber[index] = number;
            RefreshPeopleNumberByGameKind(index);
        }

        return true;
    }

    void RefreshPeopleNumberByGameKind(byte gameid)
    {
        UnityEngine.Transform ImageIconTransform = GetGameIcon_ImageIcon(gameid);
        Text peopleOnLine = ImageIconTransform.Find("playerNum").Find("Text_Num").gameObject.GetComponent<Text>();        peopleOnLine.text = GetPlayerData().peopleNumber[gameid].ToString();
    }    /// <summary>
    /// 获得游戏图标
    /// </summary>
    /// <param name="gameid">游戏ID</param>
    /// <returns></returns>    public UnityEngine.Transform GetGameIcon_ImageIcon(byte gameid)
    {
        if (gameicons_.Count == 0)
            return null;

        if (!gameicons_.ContainsKey(gameid))
            return null;

        if (gameicons_[gameid] == null)
            return null;

        return gameicons_[gameid].transform.Find("Image_Icon");
    }    void InitGameIcon(GameInfo gameInfo)    {        byte GameId = gameInfo.gameId;

        GameObject GameIcon = null;
        if (gameicons_.TryGetValue(GameId, out GameIcon))
        {
            if(GameIcon)
            {
                DebugLog.Log("当前游戏Id : " + GameId.ToString() + " 已经初始化过了! ");
                return;
            }
            gameicons_.Remove(GameId);
        }        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(GameId);        if (gamedata == null)        {            Debug.Log("初始化游戏失败，id：" + GameId.ToString());            return;        }        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)            return;

        UnityEngine.Object gameIconBG = (GameObject)bundle.LoadAsset("Game_Icon");
        GameObject go_gameIconBG = (GameObject)GameMain.instantiate(gameIconBG);
        go_gameIconBG.transform.SetParent(contestui_.transform.Find("Game_icon").Find("Scroll_Game"), false);
        go_gameIconBG.name = GameId.ToString();        UnityEngine.Object objanimator = (GameObject)bundle.LoadAsset(gamedata.GameIcon);        GameObject gameanimator = (GameObject)GameMain.instantiate(objanimator);
        UnityEngine.Transform ImageIconTransform = go_gameIconBG.transform.Find("Image_Icon");

        gameanimator.transform.SetParent(ImageIconTransform.Find("Anime"), false);

        ImageIconTransform.Find("playerNum").gameObject.SetActive(false);

        Text peopleOnLine = ImageIconTransform.Find("playerNum").Find("Text_Num").gameObject.GetComponent<Text>();        peopleOnLine.text = GetPlayerData().peopleNumber[GameId].ToString();
        gameicons_.Add(GameId, go_gameIconBG);

        UnityEngine.Transform gameiconbtnTransform = go_gameIconBG.transform.Find("Image_Icon");

        //检查游戏资源是否需要下载或更新
        byte state = CResVersionCompareUpdate.CheckGameResNeedDownORUpdate(GameId);        if (state != 0)        {            UnityEngine.Transform IconImageTransform = gameiconbtnTransform.Find("ImageBG");            IconImageTransform.gameObject.SetActive(true);            if (state == 1)            {                IconImageTransform.Find("Text_Size").gameObject.GetComponent<Text>().text = gamedata.BundleTotalSize.ToString() + "M";                IconImageTransform.Find("Text_tishi").gameObject.GetComponent<Text>().text = "下载";            }            else            {                IconImageTransform.Find("Text_Size").gameObject.SetActive(false);                IconImageTransform.Find("Text_tishi").gameObject.GetComponent<Text>().text = "更新";            }        }        else
        {
            gameiconbtnTransform.transform.Find("ImageBG").gameObject.SetActive(false);        }        XPointEvent.AutoAddListener(gameiconbtnTransform.gameObject, OnClickGameIconBtnEvents, GameId);
    }    public Sprite GetPlayerIcon()
    {
        if (Luancher.IsVChatLogin)            return GetUrlIconByID(GetPlayerData().GetPlayerIconURL(), 0);        else            return GetHostIconByID(GetPlayerData().PlayerIconId.ToString());
    }    public Sprite GetIcon(string url, uint playerid, int faceid = 0)
    {
        if (Luancher.IsVChatLogin)
            return CWechatUserAuth.GetInstance().GetUserHeadImg(playerid.ToString(), url);
        else
            return GetHostIconByID(faceid.ToString());
    }    public Sprite GetUrlIconByID(string url, uint playerid)    {

        if (playerid == 0)
            playerid = GetPlayerId();
        if(Luancher.IsVChatLogin)
          return CWechatUserAuth.GetInstance().GetUserHeadImg(playerid.ToString(), url);
        return sprites_["101"];
    }    public Sprite GetHostIconByID(string id)
    {
        if (sprites_.ContainsKey(id))
            return sprites_[id];
        else
            return sprites_["101"];
    }    public bool isShowActive()    {        if (mainui_ != null)            return mainui_.activeSelf;        return false;    }    public void RefreshPlayerCurrency()    {        //if (isShowActive() == false)        //    return;        RefreshShopPlayerCoinText();        RefreshShopPlayerDiamondText();        RefreshShopPlayerLotteryText();        RefreshCrashRedBag();        PlayerInfoUI.Instance.InitPlayerInfoPanel();

        //刷新游戏界面中的金钱数量
        if (GameBaseObj != null && enGameState == GameState_Enum.GameState_Game)
        {
            GameBaseObj.RefreshGamePlayerCoin();
        }    }

    //刷新红包显示
    public void RefreshCrashRedBag()
    {
        if (contestui_ == null)
            return;

        contestui_.transform.
            Find("Panelbottom/Bottom/Button_Bonus/TextNum").
            GetComponent<Text>().text = GetPlayerData().UnreceivedRedBag.ToString();

        if (crb_ == null)
            crb_ = new CrashRedBag();

        crb_.RefreshMoney();
    }

    //刷新金币显示
    public void RefreshShopPlayerCoinText()    {        //if (isShowActive() == false)        //    return;        //if(mainui_ != null)
        //{
        //    Text coin = mainui_.transform.FindChild("PanelHead_").
        //            FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();
        //    coin.text = GetPlayerData().GetCoin().ToString();
        //}        //if(contestui_ != null)
        //{
        //    Text contestcoin = contestui_.transform.FindChild("PanelHead_").
        //            FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();
        //    contestcoin.text = GetPlayerData().GetCoin().ToString();
        //}    }

    //刷新钻石显示
    public void RefreshShopPlayerDiamondText()    {
        //if (isShowActive() == false)
        //    return;
        if (mainui_ != null)
        {
            Text diamond = mainui_.transform.Find("PanelHead_").
                Find("Image_DiamondFrame").Find("Text_Diamond").gameObject.GetComponent<Text>();
            diamond.text = GetPlayerData().GetDiamond().ToString();
        }
        if (contestui_ != null)
        {
            Text diamond = contestui_.transform.Find("PanelHead_").
                Find("Image_DiamondFrame").Find("Text_Diamond").gameObject.GetComponent<Text>();
            diamond.text = (GetPlayerData().GetDiamond() + GetPlayerData().GetCoin()).ToString();
        }        if(gamerooms_ != null)            gamerooms_.RefreshMoney();        if (contest != null)            contest.RefreshMoney();        if (moments_ != null)            moments_.RefreshMoney();    }

    //刷新奖券显示
    public void RefreshShopPlayerLotteryText()    {
        //if (isShowActive() == false)
        //    return;
        if (mainui_ != null)
        {
            //Text ticket = mainui_.transform.FindChild("PanelHead_").
            //    FindChild("Image_TicketFrame").FindChild("Text_Ticket").gameObject.GetComponent<Text>();
            //ticket.text = GetPlayerData().GetLottery().ToString();
        }
        if (contestui_ != null)
        {
            //Text ticket = contestui_.transform.FindChild("PanelHead_").
            //    FindChild("Image_TicketFrame").FindChild("Text_Ticket").gameObject.GetComponent<Text>();
            //ticket.text = GetPlayerData().GetLottery().ToString();
        }    }

    //刷新玩家昵称或头像
    public void RefreshPlayerNameIcon()    {        Image icon = mainui_.transform.Find("PanelHead_").Find("Image_HeadBG").Find("Image_HeadMask").Find("Image_HeadImage").gameObject.GetComponent<Image>();        icon.sprite = GetIcon(PlayerObj.GetPlayerData().GetPlayerIconURL(),                 GameMain.hall_.GetPlayerId(), (int)PlayerObj.GetPlayerData().PlayerIconId);        Text name = mainui_.transform.Find("PanelHead_").Find("Image_NameBG").Find("Text_Name").gameObject.GetComponent<Text>();        name.text = GetPlayerData().GetPlayerName();    }

    //刷新vip显示
    public void RefreshPlayerVipText()    {        GameObject root = contestui_;
        //if (mainui_ != null)
        //{
        //    if (root != null)
        //    {
        //        if (mainui_.activeSelf)
        //            root = mainui_;
        //    }
        //    else
        //        root = mainui_;
        //}
        //if (roomui_ != null && roomui_.activeSelf)
        //    root = roomui_;

        if (root == null)            return;        Image vip = root.transform.Find("PanelHead_").Find("Image_NameBG").Find("Image_Vip").gameObject.GetComponent<Image>();        GameObject vip_text1 = root.transform.Find("PanelHead_").Find("Image_NameBG").Find("Image_Vip").Find("Vip_Text").Find("Num").gameObject;        GameObject vip_text2 = root.transform.Find("PanelHead_").Find("Image_NameBG").Find("Image_Vip").Find("Vip_Text").Find("Num (1)").gameObject;        int vip_lv = GetPlayerData().GetVipLevel();        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle != null)        {            if (vip_lv.ToString().Length == 1)            {                if (vip_text2.activeSelf)                {                    vip_text2.SetActive(false);                }                if (vip_lv == 0)                {                    vip.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_hui");                }                else if (vip_lv > 0)                {                    vip.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_jin");                }                vip_text1.GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + vip_lv);            }            else if (vip_lv.ToString().Length == 2)            {                if (vip.sprite.name != "zjm_word_vip_jin")                {                    vip.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_jin");                }                if (!vip_text2.activeSelf)                {                    vip_text2.SetActive(true);                }                vip_text1.GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + vip_lv.ToString().Substring(0, 1));                vip_text2.GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + vip_lv.ToString().Substring(1, 1));            }        }    }    void LoadServiceResource()
    {
        if (servicePanel_ == null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bundle == null)
                return;

            UnityEngine.Object obj = (GameObject)bundle.LoadAsset("Lobby_service");
            servicePanel_ = (GameObject)GameMain.instantiate(obj);
            servicePanel_.transform.SetParent(CanvasObj.transform, false);
            GameObject confirmbtn = servicePanel_.transform.Find("ImageBG").Find("ButtonOk").gameObject;
            XPointEvent.AutoAddListener(confirmbtn, OnCloseService, null);
        }
    }

    void OnShowService(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if (servicePanel_ == null)                LoadServiceResource();            servicePanel_.SetActive(true);        }    }

    void OnCloseService(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if (servicePanel_ == null)                LoadServiceResource();            servicePanel_.SetActive(false);        }    }    bool BackActiveInfo(uint _msgType, UMessage _ms)
    {
        if (ai_ == null)
            ai_ = new ActiveInfo();
        ai_.ReadData(_ms);
        //ShowRedBag();
        return true;
    }    bool BackRedBagBegin(uint _msgType, UMessage _ms)
    {
        uint number = _ms.ReadUInt();
        uint money = _ms.ReadUInt();
        uint activeid = _ms.ReadUInt();
        short hour = _ms.ReadShort();
        short minute = _ms.ReadShort();

        if(ai_ == null)
        {
            UMessage activemsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_GETTODAYREDBAGINFO);
            activemsg.Add(GameMain.hall_.GetPlayerId());
            NetWorkClient.GetInstance().SendMsg(activemsg);

        }
        else
        {
            ai_.activeid = activeid;
            ai_.state = (byte)GameCity.ActivityState_Enum.ActivityState_In;
            ai_.hour = hour;
            ai_.minute = minute;
            ai_.isget = false;
        }
        if(enGameState == GameState_Enum.GameState_Hall && redbagbuttonAnimate != null)
           redbagbuttonAnimate.animation.Play("newAnimation");

        ShowRedBag();

        return true;
    }    bool BackRedBagEnd(uint _msgType, UMessage _ms)
    {
        uint acitveid = _ms.ReadUInt();
        if (enGameState == GameState_Enum.GameState_Hall)
        {
            if (redbagbuttonAnimate != null)
                redbagbuttonAnimate.animation.Stop();            
        }
        if(ai_ != null)
            ai_.activeid = 0;
        UMessage activemsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_GETTODAYREDBAGINFO);
        activemsg.Add(GameMain.hall_.GetPlayerId());
        NetWorkClient.GetInstance().SendMsg(activemsg);
        //GameObject timeobj = redbag_.transform.FindChild("ImageBG").FindChild("TextTimeInfo").gameObject;
        //timeobj.SetActive(true);
        return true;
    }    bool BackOpenRedBag(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        uint getmoney = 0;
        if (enGameState == GameState_Enum.GameState_Hall)
        {
            if (redbagbuttonAnimate != null)
                redbagbuttonAnimate.animation.Stop();
        }
        switch (state)
        {
            case 0:
                byte contenttype = _ms.ReadByte();
                getmoney = _ms.ReadUInt();
                GetPlayerData().SetCoin(_ms.ReadLong());
                RefreshShopPlayerCoinText();
                ShowRedBagMoney(getmoney, contenttype);

                openredbag = redbag_.transform.Find("ImageBG").
                    Find("ImageIcon").Find("Anime").Find("Anime_Actred_2").
                    gameObject.GetComponent<UnityArmatureComponent>();
                startredbag = redbag_.transform.Find("ImageBG").
                    Find("ImageIcon").Find("Anime").Find("Anime_Actred_1").
                    gameObject.GetComponent<UnityArmatureComponent>();
                Text money = redbag_.transform.Find("ImageBG").Find("ImageIcon")
                    .Find("Text_bonus").gameObject.GetComponent<Text>();
                startredbag.gameObject.SetActive(false);
                openredbag.gameObject.SetActive(true);
                money.gameObject.SetActive(true);
                openredbag.animation.Play("newAnimation");
                ai_.isget = true;

                //刷新游戏界面中的金钱数量
                if(GameBaseObj != null && (enGameState == GameState_Enum.GameState_Game || enGameState == GameState_Enum.GameState_Contest))
                {
                    GameBaseObj.RefreshGamePlayerCoin(getmoney);
                }

                break;
            case 1:
                ai_.isget = true;
                break;
            case 2:
                CCustomDialog.OpenCustomConfirmUI(1505);
                break;
            case 3:
                CCustomDialog.OpenCustomConfirmUI(1504);
                break;
            default:
                break;
        }

        return true;
    }    void ShowRedBagMoney(uint number, byte redbagtype)
    {
        Text money = redbag_.transform.Find("ImageBG").Find("ImageIcon").
            Find("Text_bonus").gameObject.GetComponent<Text>();
        money.gameObject.SetActive(true);

        string typestr = "";
        switch (redbagtype)
        {
            case 1:
                typestr = "金币";
                break;
            case 2:
                typestr = "钻石";
                break;
            case 3:
                typestr = "奖券";
                break;
        }
        money.text = number.ToString() + typestr;
    }    void OnShowRedBag(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            ShowRedBag();        }
    }    void ShowRedBag()
    {
        if (redbag_ == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);            if (bd == null)                return;            redbag_ = GameMain.instantiate(bd.LoadAsset("Activity_Redpackets") as UnityEngine.Object) as GameObject;
            redbag_.transform.SetParent(GameObject.Find("Canvas/Root").transform, false);
            GameObject openbtn = redbag_.transform.Find("ImageBG").Find("ImageIcon").Find("Button_kai").gameObject;
            XPointEvent.AutoAddListener(openbtn, OnOpenRedBag, null);
            GameObject closebtn = redbag_.transform.Find("ImageBG").Find("Buttonclose").gameObject;
            XPointEvent.AutoAddListener(closebtn, OnCloseRedBag, null);
        }

        if (redbag_.activeSelf)
            return;

        redbag_.SetActive(true);
        Text money = redbag_.transform.Find("ImageBG").Find("ImageIcon")
            .Find("Text_bonus").gameObject.GetComponent<Text>();
        money.gameObject.SetActive(false);
        startredbag = redbag_.transform.Find("ImageBG").
                Find("ImageIcon").Find("Anime").Find("Anime_Actred_1").
                gameObject.GetComponent<UnityArmatureComponent>();
        startredbag.gameObject.SetActive(true);

        if (openredbag == null)
            openredbag = redbag_.transform.Find("ImageBG").
                Find("ImageIcon").Find("Anime").Find("Anime_Actred_2").
                gameObject.GetComponent<UnityArmatureComponent>();

        openredbag.gameObject.SetActive(false);
        money.gameObject.SetActive(false);
        startredbag.animation.Play("hongbaochu");
        startredbag.AddEventListener(EventObject.COMPLETE, RedbagStartComplete);
    }

    void UpdateRedBag(float time)
    {
        if (redbag_ == null)
            return;

        if (ai_ == null)
            return;

        Text nexttimeobj = redbag_.transform.Find("ImageBG").Find("TextNext").gameObject.GetComponent<Text>();
        Text currentText = redbag_.transform.Find("ImageBG").Find("ImageIcon").Find("TextCountdown").gameObject.GetComponent<Text>();

        if (ai_.activeid == 0 || ai_.state == (byte)GameCity.ActivityState_Enum.ActivityState_Over)
        {
            currentText.gameObject.SetActive(false);
            nexttimeobj.gameObject.SetActive(true);
            nexttimeobj.text = "活动已结束";
            return;
        }

        int distanceMinutes = (ai_.hour * 60 + ai_.minute) - (DateTime.Now.Hour * 60 + DateTime.Now.Minute);
        int hour = distanceMinutes / 60;
        int minute = distanceMinutes % 60;

        if (ai_.state == (byte)GameCity.ActivityState_Enum.ActivityState_In)
        {
            currentText.gameObject.SetActive(true);
            nexttimeobj.gameObject.SetActive(false);
            if(ai_.isget)
            {
                currentText.text = "已领取";
            }
            else if (hour <= 0)
            {
                hour = 0;
                currentText.text = "剩余时间：" + minute.ToString() + "分钟";
            }
            else
            {
                if (minute <= 0)
                    minute = 0;
                currentText.text = "剩余时间：" + hour.ToString() + "小时" + minute.ToString() + "分钟";
            }
            if (distanceMinutes < 0)
                ai_.state = (byte)GameCity.ActivityState_Enum.ActivityState_Over;
        }

        if (ai_.state == (byte)GameCity.ActivityState_Enum.ActivityState_Wait)
        {
            currentText.gameObject.SetActive(false);
            nexttimeobj.gameObject.SetActive(true);
            if (hour <= 0)
            {
                hour = 0;
                nexttimeobj.text = "下场红包开启：" + minute.ToString() + "分钟";
            }
            else
            {
                if (minute <= 0)
                    minute = 0;
                nexttimeobj.text = "下场红包开启：" + hour.ToString() + "小时" + minute.ToString() + "分钟";
            }
            if (distanceMinutes < 0)
                ai_.state = (byte)GameCity.ActivityState_Enum.ActivityState_In;
        }
    }

    UnityArmatureComponent startredbag;
    UnityArmatureComponent openredbag;
    void OnOpenRedBag(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            if (ai_.activeid == 0 || ai_.state == (byte)GameCity.ActivityState_Enum.ActivityState_Over)
                return;
            UMessage openredbagmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERGETREDBAG);                        openredbagmsg.Add(GetPlayerId());            openredbagmsg.Add(ai_.activeid);            NetWorkClient.GetInstance().SendMsg(openredbagmsg);        }
    }

    void RedbagStartComplete(string _type, EventObject eventObject)    {        switch (_type)        {            case EventObject.COMPLETE:                if (eventObject.animationState.name == "hongbaochu")                {                    startredbag.RemoveEventListener(EventObject.COMPLETE, RedbagStartComplete);                    startredbag.animation.Play("xingxingshan");                }                break;
        }    }

    void OnCloseRedBag(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            redbag_.SetActive(false);        }
    }    bool PlayerDisOrReconnect(uint _msgType, UMessage _ms)
    {
        byte connect = _ms.ReadByte();//0:上线 1:离线
        byte gameKind = _ms.ReadByte();
        uint userId = _ms.ReadUInt();
        byte sit = _ms.ReadByte();

        if(GameBaseObj != null)
        {
            Debug.Assert(gameKind == (byte)GameBaseObj.GameType);
            GameBaseObj.OnPlayerDisOrReconnect(connect == 1, userId, sit);
        }

        return true;
    }    bool QueryStateReply(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();//0:结束 1:等待比赛
        DebugLog.Log("QueryStateReply:" + state);
        if (state == 0)
            MatchInGame.GetInstance().OnAlreadyEnd();
        else
            MatchInGame.GetInstance().ShowWait(ContestDataManager.Instance().GetCurrentContestData().nEnrollStartTime);

        return true;
    }    public GameState_Enum GetCurrentGameState()
    {
        return enGameState;
    }

    public void ReqEnterGameRoom(byte mode, byte game, byte type, uint code)
    {
        if (enGameState == GameState_Enum.GameState_Login)
            return;

        if (HallMain.m_iRoomSerIndex != 0 || GameBaseObj != null)//in game
        {
            CCustomDialog.OpenCustomConfirmUI(2011);
            return;
        }

        if (!PickGameByID(game, false))
            return;

        if (type == (byte)GameTye_Enum.GameType_Appointment)
        {
            if (gamerooms_ == null)
                gamerooms_ = new GameRoom();
            else
                gamerooms_.InitGameRoom();
            gamerooms_.SendJoinMsg(code);
        }
        else if (type == (byte)GameTye_Enum.GameType_Contest)
        {
            OnClickSelfMatch(EventTriggerType.PointerClick, null, null);

            UMessage joinmsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_PlayerEnroll);
            joinmsg.Add(GameMain.hall_.GetPlayerId());
            joinmsg.Add(code);
            NetWorkClient.GetInstance().SendMsg(joinmsg);
        }
    }
}