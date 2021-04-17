﻿using System.Collections;
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

[Hotfix]

    GameObject CanvasObj;

    //游戏状态
    private GameState_Enum enGameState;

    MessageBackApplyGame mbag_;

    // 玩家对象
    Player PlayerObj;

    //游戏类对象
    public CGameBase GameBaseObj;
    public NetworkReachability IntelentType;
    //是否断线重连过程中
    public static bool bDisconnectReconnection;
    //重连次数
    private byte ReconnectTimes;
    public static NetWorkClient videotcpclient;

    GameObject m_objRelief;         //救济金
    public uint m_iAddReliefCoin;   //增加的救济金金额
    public byte m_iLeftReliefNum;//剩余救济金次数今天
    public bool m_bIsAfterLogin;//标志是否进入 afterlogin
    public bool isGetRelief;
    private AsyncOperation aAsyncSceneOperation;
    bool isOtherLoadScene;
    //横屏场景加载界面
    private GameObject gSceneLoadProgressLandscapeUI;
    //竖屏场景加载界面
    private GameObject gSceneLoadProgressPortraitUI;
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
    public void Start()

        isGetRelief = false;

        m_RecordButton = null;
        ReconnectTimer = null;
        DocumentPanel = null;
        //EmailDataManager.GetNewsInstance();
        AudioManager.Instance.MusicVolume = StateStorage.HasKey("PE_MusicVolume") ? StateStorage.LoadData<float>("PE_MusicVolume") : 1f;
        AudioManager.Instance.SoundVolume = StateStorage.HasKey("PE_SoundVolume") ? StateStorage.LoadData<float>("PE_SoundVolume") : 1f;
    }

    //检测网络环境是否发生变化
    private void CheckIntelentType()

    
    //检测连接服务器状态
    public void CheckConnectSeverState()
        if (enGameState >= GameState_Enum.GameState_Hall)
            if (!NetWorkClient.GetInstance().IsSocketConnected)
            {
                NetWorkClient.GetInstance().CloseNetwork();
                CCustomDialog.OpenCustomWaitUI("网络正在尝试第1次重连...");
                Debug.Log("socket disconnect server");
            }
        }
        {
            ReconnectTimes = 1;
            ReconnectTimer = new CTimerCirculateCall(5.0f, ReconnectTimerCallBack);
            xTimeManger.Instance.RegisterTimer(ReconnectTimer);
        }

    //断线重连计时回调
    private void ReconnectTimerCallBack(object param)
    {
        ReconnectTimes++;
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
        if (enGameState < GameState_Enum.GameState_Game )
        bDisconnectReconnection = false;
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
    public void NotifyAppBackStateToGameServer(bool bInBack)
            SaveSoundState();

            //主动通知服务器我切后台了
            if (NetWorkClient.GetInstance().IsSocketConnected)
            {
                UMessage appbackmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_APPBACKSTATENOTIFYGAMESERVER);
                appbackmsg.Add(GetPlayerId());
                appbackmsg.Add(bInBack);
                SendMsgToRoomSer(appbackmsg);
            }

    }

    //检测连接服务器状态
    public void CheckSocketConnectedState()
                UMessage checkmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_BEGIN);
                checkmsg.Add(GetPlayerId());
                NetWorkClient.GetInstance().SendMsg(checkmsg);
            }
            ip = "192.166.0.129";
        if (port == 0)
            port = 16701;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#endif
        {
            UMessage  ApplyMsg = new UMessage((uint)GameCity.EMSG_ENUM.CCGateMsg_PlayerApplyLoginGame);
            NetWorkClient.GetInstance().SendMsg(ApplyMsg);
        }



    private void RegitserMsgHandle()
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_LOGINSERDISCONNECT, LoginSerDisConnect);                     
        CMsgDispatcher.GetInstance().RegMsgDictionary(
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
        CMsgDispatcher.GetInstance().RegMsgDictionary(
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
        CMsgDispatcher.GetInstance().RegMsgDictionary(
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
    public void Update()
        if (enGameState == GameState_Enum.GameState_Luancher)
            SwithGameStateToLogin();

        if (isDownloadGameAssetBundle)
            UpdateGameResDownloadProcess();
            UpdateAsyncLoadSceneProcess();

        //CheckIntelentType();
        CheckConnectSeverState();

        UpdateRedBag(Time.deltaTime);
        {
            SwitchToHallScene(true, 0);
            //isOtherLoadScene = false;
        }

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
    //}
        {
            videotcpclient.CloseNetwork();
            videotcpclient = null;
        }
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
    }
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

    void onClickNews(EventTriggerType eventtype, object button, PointerEventData eventData)
            //AudioManager.Instance.PlaySound(GameDefine.HallAssetbundleName, "UIbutton02");
            //AskForNewsData();
            Email.GetEmailInstance().Ask4MailData();
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (m_Bulletin != null)
                m_Bulletin.gameObject.SetActive(true);
        }

    void AskForBagData()
    {

    }

    void onClickBag(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            AskForBagData();
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
        }
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            AskForClubData();
            //if (club_ == null)
            //{
            //    club_ = new Club(CanvasObj);
            //    club_.Start();
            //}
            //club_.ShowClub();

            //改成朋友进入逻辑 
            {
                moments_ = new FriendsMoments();
            }

            if (contestui_ != null)
                contestui_.SetActive(false);
        return GetPlayerData().GetPlayerID();
    {
        Debug.Log("Login server is disconnect, Please try again later!");
        return true;
    }

        //请求进入游戏失败
        if (!mbag_.isin)
        {
            //m_iRoomSerIndex = mbag_.nSerIndex;
            SetRoomSerIndex(mbag_.nSerIndex);
        }

    public void ClearChilds(GameObject obj)
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
        AssetBundle bundle = AssetBundleManager.GetAssetBundle("hall");
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
    }

    //从任何地方返回登陆界面
    public void AnyWhereBackToLoginUI()

    //大厅界面返回登陆界面
    public void HallBackToLoginUI()
            contestui_.SetActive(false);
            PlayerObj.ChangeRequestEnterGameState(false);
            return;
    {
        if (!mbag_.isin)
    }
    {
        if (!mbag_.isin)
    }
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
    }
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
    }
    {
        loginGame.Add(middleIn);
        //SendMsgToRoomSer(loginGame);
        SendMsgToRoomSer(loginGame);
            GameMain.Instance.ExitApplication(1);
    {
        PlayerData pd = PlayerObj.GetPlayerData();
        byte gameKind = (byte)gameId;
        DebugLog.Log("OnGameReconnect:" + gameKind + " type:" + gameMode);

        if(m_iRoomSerIndex == 0)
        {
            if (gameMode == GameTye_Enum.GameType_Contest)
            {
                {
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
            }
        else
        {
            byte middleIn = 1;
            if (gameMode == GameTye_Enum.GameType_Normal)
                connect2CustomServer(gameKind, middleIn);
            else if (gameMode == GameTye_Enum.GameType_Contest)
            {
                if (contest == null)
                {
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
        }
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_ForestDance);
        if (gamedata != null)
            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_ForestDance);
        else
        {
            Debug.Log("游戏id:0不存在");
            return false;
        }

        FD_DataCenter.GetInstance().condition.ReadData(_ms);

        return true;
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)gameId);
        if (gamedata != null)
        {
            //else
            //    GameBaseObj.GameMode = gameType;
            if(gameType == GameTye_Enum.GameType_Contest)
                MatchInGame.GetInstance().contestTimeLeft = timeLeft;
        }
            Debug.Log("游戏id:" + gameId + "不存在");
    }

        EnterGameScene(gameId, GameTye_Enum.GameType_Normal);


        GameTye_Enum gameType = (GameTye_Enum)_ms.ReadByte();
        if (gameType == GameTye_Enum.GameType_Normal)
            EnterGameScene(gameId, gameType);
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
    }
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata != null)
            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_CarPort);
        else
            Debug.Log("游戏id:0不存在");

        return true;
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_LaBa);
        if (gamedata != null)
            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_LaBa);
        else
            Debug.Log("游戏id:2不存在");

        LB_DataCenter.Instance().LevelJoinCoinLimit.ReadData(_ms);

        return true;
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullAllKill);
    }
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullHappy);
    }
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_TexasPoker);
    }
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
    }
    {
        Ex_SMNoGameScene ex_SMNoGameScene = new Ex_SMNoGameScene();
        long coin = _ms.ReadLong();
        ex_SMNoGameScene.coin = coin;
        GetPlayerData().SetCoin(coin);
        CGame_DiuDiuLe ex_main = new CGame_DiuDiuLe();
        ex_main.Initialization();
        GameBaseObj = (CGameBase)ex_main;

        return true;
    }
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_DiuDiuLe);
        if (gamedata != null)
            SwitchGameScene(gamedata.SceneABName, gamedata.GameSceneName, GameKind_Enum.GameKind_DiuDiuLe);
        else
            Debug.Log("游戏id:1不存在");

        return true;


    /// <summary>

    private void OnClickGameIconBtnEvents(EventTriggerType eventtype, object parma, PointerEventData eventData)
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
            }

    public void OnClickRoomIconBtn(byte gameId, byte index)
    {
        if (PickGameByID(gameId))
            CurRoomIndex = index;
    }
        //如果资源正在下载中
        if (CGameResDownloadMgr.Instance.isGameResDownloading(gameid))
            //Debug.Log("游戏资源还在下载中,游戏ID:" + gameid);
            CCustomDialog.OpenCustomConfirmUI(1016);

        //如果当前正在请求进入游戏状态，那么nothing to do
        if (request && PlayerObj.IsRequestEnterGameState())
        //不需要更新直接请求进入游戏
        if (!needupdate)
            //有其它游戏资源正在下载，为避免网络受影响，不让进游戏
            if (!CGameResDownloadMgr.Instance.isAllGameResHaveDownOver())


    /// <summary>
        //先检测资源是否需要更新
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(gameId);
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


    /// <summary>
        isDownloadGameAssetBundle = true;
        m_nCurGRDCount = 1;
        m_nCurTGRDCount =  CGameResDownloadMgr.Instance.GetDownloadGameResCount(gameid);
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(gameid);
            CCustomDialog.OpenCustomWaitUI("下载"+ gamedata.GameName + "("+ m_nCurGRDCount+"/"+ m_nCurTGRDCount+ ")" + "... 0%");

    /// <summary>
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(curGameId);
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


    /// <summary>


    /// <summary>
    {
        PickGameByID((byte)gameid, false);
        GameMain.ShowTicketResult(0);
        //CResVersionCompareUpdate.CompareABVersionAndUpdate(bundleName);
        //HttpDownload.DownFile(GameDefine.LuancherURL, GameDefine.AssetBundleSavePath, "car.scene");        
        AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, bundleName);
        isAsyncLoadScene = true;

        SceneActivationSetScreneOrientation();
            gCurDisplayLoadSceneProgressUI.SetActive(true);
            gCurDisplayLoadSceneProgressUI.transform.SetAsLastSibling();
        }
        //yield return new WaitForEndOfFrame();
        //Debug.Log("AsyncSceneOperation.progress:" + AsyncSceneOperation.progress);
        GameSceneLoadFinishCallBack(gameType, callback);


    /// <summary>
            return;

        if (gCurDisplayLoadSceneProgressUI != null)
            //Debug.Log("AsyncSceneOperation.progress:" + fAsyncLoadProgress);           
            //ScreneLoadFinishCallBack(sAsyncLoadSceneName);
            aAsyncSceneOperation.allowSceneActivation = true;
            //SceneActivationSetScreneOrientation();
        }

    /// <summary>
    /// 设置加载过程随机提示文本
    /// </summary>
    /// <param name="parenttf"></param>
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

    }
        {
            return;
        }
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
        else
            //随机换张加载图
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallConstAssetBundleName);
            if(bGameChessState)
            {
                Screen.orientation = ScreenOrientation.Portrait;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = false;
                RefreshCanvasScaler(new Vector3(750,1334));
            }
        }

    void RefreshCanvasScaler(Vector2 ReferenceResolution)
    {
        CanvasScaler canvasScaler = GameMain.ddCanvas.GetComponent<CanvasScaler>();
        canvasScaler.referenceResolution = ReferenceResolution;
    }

    //游戏场景加载完成回调
    private bool GameSceneLoadFinishCallBack(GameTye_Enum gameType, UnityAction callback = null)
        else if(gameType == GameTye_Enum.GameType_Appointment)
            enGameState = GameState_Enum.GameState_Appointment;
        else
            enGameState = GameState_Enum.GameState_Game;

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
                break;
                {
                    GameBaseObj = new CGame_XzMahjong(gameType, CurGameId);
                }
                break;
                {
                    GameBaseObj = new CGame_YcMahjong(gameType, CurGameId);
                }
                break;
                {
                    GameBaseObj = new CGame_GuanDan(gameType);
                }
                break;
            case GameKind_Enum.GameKind_CzMahjong:
                    GameBaseObj = new CGame_CzMahjong(gameType, CurGameId);
            case GameKind_Enum.GameKind_LuckyTurntable:
                    GameBaseObj = new CGame_LuckyTurntable();
                }
            case GameKind_Enum.GameKind_GouJi:
                    GameBaseObj = new CGame_GouJi(gameType);
                }
            case GameKind_Enum.GameKind_HongZhong:
                    GameBaseObj = new CGame_HzMahjong(gameType, CurGameId);
                }
                break;
            case GameKind_Enum.GameKind_Answer:
                    GameBaseObj = new CGame_Answer(gameType);
                }
            case GameKind_Enum.GameKind_Chess:
                    GameBaseObj = new CGame_Chess(gameType);
                }
        if (AutoEnterGameMode)
        {
            ReconnectGameServer();
        }
        {
            GameBaseObj.Initialization();
            //GameBaseObj.InitialCommonUI();
        }

    //返回大厅设置横屏或竖屏加载进度界面
    private void BackHallSetLandscapePortraitUI(bool isgameBack)

            //             Screen.orientation = ScreenOrientation.LandscapeLeft;
            //             Screen.autorotateToLandscapeLeft = true;
            //             Screen.autorotateToLandscapeRight = true;
            //             Screen.autorotateToPortrait = false;
            //             Screen.autorotateToPortraitUpsideDown = false;
            //             CanvasScaler cans = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
            //             cans.referenceResolution = new Vector2(960, 540);

        }
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
        }
            SetRandomLoadingTipsText(gCurDisplayLoadSceneProgressUI.transform);

    //加载大厅场景
    private IEnumerator LoadHallScene(bool isGameBack, byte backState)
            //UnityFactory.factory.Clear();
            //LoadHallResource();
            enGameState = GameState_Enum.GameState_Hall;
            UnloadGameAssetBundle();
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
            }
            {
                enGameState = GameState_Enum.GameState_Login;
                CLoginUI.Instance.IntoLoginProcess();
            }
                HallBackToLoginUI();
        {
    }
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
    }
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
    }
    {
        if(contestui_ != null)
            contestui_.SetActive(false);
        if(mainui_ != null)
            mainui_.SetActive(true);
        //InitRoomsData();
        //roomsui_.SetActive(true);
    }
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

    void OnCreateRoom(EventTriggerType eventtype, object button, PointerEventData eventData)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //roomLobbyUI_.SetActive(false);
            LoadRoomResource();

            createroomui_.SetActive(true);
            joinroomui_.SetActive(false);
        }
    }

    void OnJoinRoom(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //roomLobbyUI_.SetActive(false);
            LoadRoomResource();

            createroomui_.SetActive(false);
            joinroomui_.SetActive(true);
        }
    }

    void OnGameCity(EventTriggerType eventtype, object button, PointerEventData eventData)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            roomLobbyUI_.SetActive(false);
            AfterLogin();
        }
    }
    //返回到大厅场景后卸载游戏资源
    private void UnloadGameAssetBundle()
            //Debug.Log("返回到大厅场景后卸载游戏资源");
        }

    }

    /// <summary>
        }

    //切换游戏场景
    void SwitchGameScene(string abname, string scenename, GameKind_Enum gameid, GameTye_Enum gameType = GameTye_Enum.GameType_Normal, UnityAction callback = null)
    {
    }
        }
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
        }
        {
            Text diamond = root.transform.Find("PanelHead_").Find("Image_DiamondFrame").Find("Text_Diamond").gameObject.GetComponent<Text>();
            diamond.text = GetPlayerData().GetDiamond().ToString();
        }

        //Text Lotterytext = root.transform.FindChild("PanelHead_/Image_TicketFrame/Text_Ticket").gameObject.GetComponent<Text>();
        //Lotterytext.text = GetPlayerData().GetLottery().ToString();

        GameObject diamondBtn = root.transform.Find("PanelHead_").Find("Image_DiamondFrame").gameObject;

        //GameObject lotteryBtn = root.transform.FindChild("PanelHead_").FindChild("Image_TicketFrame").gameObject;

        /*if (GirlAnimationTimer == null)
           GirlAnimationTimer = new CTimerCirculateCall(3.0f, PlayGirlAnimation);
           xTimeManger.Instance.RegisterTimer(GirlAnimationTimer);
        }*/
    }

    

    private void OnRetruen2Main(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (roomsui_ != null)
                roomsui_.SetActive(false);
            if(mainui_ != null)
                mainui_.SetActive(false);
            if(contestui_ != null)
                contestui_.SetActive(true);
        }

    private void OnClickMatch(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            Go2MatchHall();

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
    //    if (contestui_ != null)

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

        //Text coin = roomsui_.transform.FindChild("Top").FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();
    }

    void InitRoomsEvents()
    {
        GameObject returnBtn = roomsui_.transform.Find("Top").Find("ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(returnBtn, OnRetruen2Main, null);

        GameObject createBtn = roomsui_.transform.Find("middle").Find("Button_chuangjian").gameObject;
        XPointEvent.AutoAddListener(createBtn, OnCreateRooms, null);

        GameObject joinBtn = roomsui_.transform.Find("middle").Find("Button_jiaru").gameObject;
        XPointEvent.AutoAddListener(joinBtn, OnJoinRooms, null);

        GameObject diamondBtn = roomsui_.transform.Find("Top").Find("Image_DiamondFrame").gameObject;

        //GameObject recordBtn = roomsui_.transform.FindChild("middle").FindChild("Button_record").gameObject;
        //XPointEvent.AutoAddListener(recordBtn, OnClickAppointmentRecord, null);
    }

    public void OnClickAppointmentRecord(EventTriggerType eventtype, object button, PointerEventData eventData)
            if(m_RecordButton == null)
            {
                return;
            }

            if(!m_RecordButton.interactable)
            {
                return;
            }
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            ShowGameRecord();

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

    bool BackVideoServerIp(uint msgType, UMessage msg)

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


    private void OnCreateRooms(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            GameObject createPanel = roomsui_.transform.Find("Pop-up").Find("Room_rule").gameObject;

    private void OnJoinRooms(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            GameObject joinPanel = roomsui_.transform.Find("Pop-up").Find("Room_number").gameObject;

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

    private void OnClickRomm(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //AppointmentDataManager.AppointmentDataInstance().kind = AppointmentKind.From_Appointment;
            //LoadRoomsResource();
            //contestui_.SetActive(false);
            //roomsui_.SetActive(true);

            //if (gamerooms_ == null)
            //    gamerooms_ = new GameRoom(roomsui_);
            //else
            //    gamerooms_.InitGameRoom(roomsui_);
        }

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

    private void OnClickTournament(EventTriggerType eventtype, object button, PointerEventData eventData)
            int ContestType = (int)button;
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            Go2ContestHall((byte)ContestType);

    //播放美女动作
    void PlayGirlAnimation(object[] args)
    {
        if (enGameState == GameState_Enum.GameState_Hall)
        {
            GameObject girlAnimateObj = mainui_.transform.Find("Image_Show").Find("Anime_Lobbt_meinv").gameObject;
            UnityArmatureComponent girlAnimate = girlAnimateObj.GetComponent<UnityArmatureComponent>();
            girlAnimate.animation.Play("newAnimation");
        }
    }
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            PlayerInfoUI.Instance.OpenOrClosePlayerInfoUI(true);
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            GameObject root = roomLobbyUI_;
            if (mainui_ != null && mainui_.activeSelf)
                root = mainui_;
            if (roomui_ != null && roomui_.activeSelf)
                root = roomui_;

            Shop.SHOPTYPE _type = (Shop.SHOPTYPE)param;
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
    }
    {
        GameObject ModelObj = GameObject.Find("3d_point/Model");
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle("pokercommon.resource");

        foreach (UnityEngine.Transform child in ModelObj.transform)
            GameObject.Destroy(child.gameObject);
        UnityEngine.Object asset = bundle.LoadAsset(male == 1 ? "3Dmodel_nan_1" : "3Dmodel_nv_1");
        GameObject obj = (GameObject)GameMain.instantiate(asset);
    {
        if (enGameState == GameState_Enum.GameState_Game || enGameState == GameState_Enum.GameState_Appointment
            || enGameState == GameState_Enum.GameState_Contest)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);

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
                UnityEngine.Object obj0 = Resources.Load("Prefabs/Main_Loading_shuping");
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
        GameObject clubBtn = contestui_.transform.Find("Panelbottom/Bottom/Button_Club").gameObject;

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
        if (eventtype == EventTriggerType.PointerClick)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            DocumentPanel.SetGameDocumentActivePanel(true,gameicons_);
        }
    }

    private void OnShowMasterRankPanel(EventTriggerType eventtype, object button, PointerEventData eventData)

    void OnOpenBounds(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //打开红包界面
            if (crb_ == null)

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

    void InitGamePanel()

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
        //});



    /// <summary>

    /// <summary>

    /// <summary>







    /// <summary>



    //大厅状态断线处理
    public void ServerCloseByGmOrder(object param)

    /// <summary>

        //GameObject background = GameObject.Find("Canvas/Root");
        m_objRelief.transform.SetParent(CanvasObj.transform, false);

    /// <summary>
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
        Text peopleOnLine = ImageIconTransform.Find("playerNum").Find("Text_Num").gameObject.GetComponent<Text>();
    }
    /// 获得游戏图标
    /// </summary>
    /// <param name="gameid">游戏ID</param>
    /// <returns></returns>
    {
        if (gameicons_.Count == 0)
            return null;

        if (!gameicons_.ContainsKey(gameid))
            return null;

        if (gameicons_[gameid] == null)
            return null;

        return gameicons_[gameid].transform.Find("Image_Icon");
    }

        GameObject GameIcon = null;
        if (gameicons_.TryGetValue(GameId, out GameIcon))
        {
            if(GameIcon)
            {
                DebugLog.Log("当前游戏Id : " + GameId.ToString() + " 已经初始化过了! ");
                return;
            }
            gameicons_.Remove(GameId);
        }

        UnityEngine.Object gameIconBG = (GameObject)bundle.LoadAsset("Game_Icon");
        GameObject go_gameIconBG = (GameObject)GameMain.instantiate(gameIconBG);
        go_gameIconBG.transform.SetParent(contestui_.transform.Find("Game_icon").Find("Scroll_Game"), false);
        go_gameIconBG.name = GameId.ToString();
        UnityEngine.Transform ImageIconTransform = go_gameIconBG.transform.Find("Image_Icon");

        gameanimator.transform.SetParent(ImageIconTransform.Find("Anime"), false);

        ImageIconTransform.Find("playerNum").gameObject.SetActive(false);

        Text peopleOnLine = ImageIconTransform.Find("playerNum").Find("Text_Num").gameObject.GetComponent<Text>();
        gameicons_.Add(GameId, go_gameIconBG);

        UnityEngine.Transform gameiconbtnTransform = go_gameIconBG.transform.Find("Image_Icon");

        //检查游戏资源是否需要下载或更新
        byte state = CResVersionCompareUpdate.CheckGameResNeedDownORUpdate(GameId);
        {
            gameiconbtnTransform.transform.Find("ImageBG").gameObject.SetActive(false);
    }
    {
        if (Luancher.IsVChatLogin)
    }
    {
        if (Luancher.IsVChatLogin)
            return CWechatUserAuth.GetInstance().GetUserHeadImg(playerid.ToString(), url);
        else
            return GetHostIconByID(faceid.ToString());
    }

        if (playerid == 0)
            playerid = GetPlayerId();
        if(Luancher.IsVChatLogin)
          return CWechatUserAuth.GetInstance().GetUserHeadImg(playerid.ToString(), url);
        return sprites_["101"];
    }
    {
        if (sprites_.ContainsKey(id))
            return sprites_[id];
        else
            return sprites_["101"];
    }

        //刷新游戏界面中的金钱数量
        if (GameBaseObj != null && enGameState == GameState_Enum.GameState_Game)
        {
            GameBaseObj.RefreshGamePlayerCoin();
        }

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
    public void RefreshShopPlayerCoinText()
        //{
        //    Text coin = mainui_.transform.FindChild("PanelHead_").
        //            FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();
        //    coin.text = GetPlayerData().GetCoin().ToString();
        //}
        //{
        //    Text contestcoin = contestui_.transform.FindChild("PanelHead_").
        //            FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();
        //    contestcoin.text = GetPlayerData().GetCoin().ToString();
        //}

    //刷新钻石显示
    public void RefreshShopPlayerDiamondText()
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
        }

    //刷新奖券显示
    public void RefreshShopPlayerLotteryText()
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
        }

    //刷新玩家昵称或头像
    public void RefreshPlayerNameIcon()

    //刷新vip显示
    public void RefreshPlayerVipText()
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

        if (root == null)
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

    void OnShowService(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if (servicePanel_ == null)

    void OnCloseService(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if (servicePanel_ == null)
    {
        if (ai_ == null)
            ai_ = new ActiveInfo();
        ai_.ReadData(_ms);
        //ShowRedBag();
        return true;
    }
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
    }
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
    }
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
    }
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
    }
    {
        if (eventtype == EventTriggerType.PointerClick)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            ShowRedBag();
    }
    {
        if (redbag_ == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
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
        if (eventtype == EventTriggerType.PointerClick)
                return;
            UMessage openredbagmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERGETREDBAG);
    }

    void RedbagStartComplete(string _type, EventObject eventObject)
        }

    void OnCloseRedBag(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
    }
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
    }
    {
        byte state = _ms.ReadByte();//0:结束 1:等待比赛
        DebugLog.Log("QueryStateReply:" + state);
        if (state == 0)
            MatchInGame.GetInstance().OnAlreadyEnd();
        else
            MatchInGame.GetInstance().ShowWait(ContestDataManager.Instance().GetCurrentContestData().nEnrollStartTime);

        return true;
    }
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