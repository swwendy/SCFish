using DG.Tweening;using System;using System.Collections;using System.Collections.Generic;using System.Linq;using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;//牌型
[LuaCallCSharp]
public enum LandPokerType_Enum{    LandPokerType_Error = 0,    LandPokerType_Single,           //单牌
    LandPokerType_Pairs,            //一对对子
    LandPokerType_Three,            //三张一样点数
    LandPokerType_SeriesPairs,      //连对
    LandPokerType_PlaneNoWith,      //不带其他

    LandPokerType_PlaneWithOherPair,    //带对

    LandPokerType_PlaneWithOherSingle,  //带单

    LandPokerType_ThreeAndOne,      //三带1
    LandPokerType_ThreeAndTwo,      //三带2
    LandPokerType_Flush,            //N顺子
    LandPokerType_FourAndTwoPair,       //4带2对子
    LandPokerType_FourAndTwoSingle,     //4带2单牌
    LandPokerType_Four,             //
    LandPokerType_KingBlast,        //王炸

    LandPokerType_Max};
//房间状态
[LuaCallCSharp]
public enum LandRoomState_Enum{    LandRoomState_Init = 0,    LandRoomState_WaitPlayer,       //等人
    LandRoomState_WaitReady,        //等待准备
    LandRoomState_CountDownBegin,       //游戏开始倒计时    LandRoomState_DealPoker,        //发牌    LandRoomState_AskLandlord,      //询问是否叫地主    LandRoomState_GetLordsPoker,    //坐庄的人获得最后三张牌    LandRoomState_TurnOutPoker,     //轮流出牌    LandRoomState_OverOnce,         //一局结束    LandRoomState_Result,    LandRoomState_End,    LandRoomState_Max				//回收状态};//玩家状态
[LuaCallCSharp]
public enum LandPlayerState_Enum
{
    LandPlayerState_Init = 0,
    LandPlayerState_Match,          //1匹配中
    LandPlayerState_GameIn,         //2游戏中
    LandPlayerState_OnGameButIsOver,//3游戏中但游戏已经结束
    LandPlayerState_ReadyHall,  //玩家在大厅
    LandPlayerState_OnDesk,     //玩家在桌子上

    LandPlayerState_Max
};
[Hotfix]public class CGame_LandLords : CGameBase{    public Transform MainUITfm { get; set; }    Transform ResultUITfm { get; set; }    Transform ChatUITfm { get; set; }    public Transform RecordUITfm { get; set; }    public Transform AnimationTfm { get; set; }    public Button OutPokerBtn { get; private set; }    public AssetBundle LandlordAssetBundle { get; set; }    public AssetBundle CommonAssetBundle { get; set; }    public CustomCountdownImgMgr CCIMgr { get; set; }    public Canvas GameCanvas { get; private set; }    Landlord_RoleLocal m_LocalPlayer;    Landlord_Role m_LeftPlayer;    Landlord_Role m_RightPlayer;    Dictionary<byte, Landlord_Role> m_PlayerList = new Dictionary<byte, Landlord_Role>();    Transform m_LastCard;    Transform m_GameInfo, m_GameButton;    byte m_nCurrenLevel = RoomInfo.NoSit;    public bool IsFree = false;    LandRoomState_Enum m_eRoomState;    GameObject appointmentPanel_;    GameObject appointmentResultPerPanel_;    GameObject appointmentResultTotalPanel_;    List<List<ResultData>> appointmentTotalResults_;    bool isOpenAppointmentFinalResults_;    GameObject appointmentRulePanel_;    string appointmentGameRule_;    sbyte m_nWaitingLoad = 0;    bool m_bReconnected = false;    bool bExitAppointmentDialogState = false;    public CGame_LandLords(GameTye_Enum gameType) : base(GameKind_Enum.GameKind_LandLords)    {        GameMode = gameType;        InitMsgHandle();    }    public override void Initialization()    {        base.Initialization();        CCIMgr = new CustomCountdownImgMgr();        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameKind_Enum.GameKind_LandLords, "GameDouDiZhuAudioCsv");
        GameCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if(!m_bReconnected)
        {
            if (GameMode == GameTye_Enum.GameType_Appointment)
            {
                AudioManager.Instance.PlayBGMusic(GameDefine.HallAssetbundleName, "hall");
                LoadAppintmentReadResource();
                m_nWaitingLoad = -1;
            }
            else if (GameMode == GameTye_Enum.GameType_Normal)
            {
                MatchRoom.GetInstance().ShowRoom((byte)GameType);
                OnClickLevelBtn(GameMain.hall_.CurRoomIndex);
                //CustomAudioDataManager.GetInstance().PlayAudio(1001, false);
                m_nWaitingLoad = -1;
            }        }else if (GameMode == GameTye_Enum.GameType_Normal)
        {
            MatchRoom.GetInstance().ShowRoom((byte)GameType);
            OnClickLevelBtn(GameMain.hall_.CurRoomIndex);        }
        Load();    }    bool Load()
    {
        if (m_LocalPlayer == null)
        {            if (m_nWaitingLoad > 0)                m_nWaitingLoad--;            if (m_nWaitingLoad != 0)                return false;
        }        else            return true;
        LoadResource();        InitPlayers();        if (GameMode == GameTye_Enum.GameType_Contest)            EnterRoom(1, m_bReconnected);        else if (GameMode == GameTye_Enum.GameType_Appointment)            EnterAppointment();        else            EnterRoom(GameMain.hall_.CurRoomIndex, m_bReconnected);

        if (GameMode == GameTye_Enum.GameType_Normal)
        {
            MatchRoom.GetInstance().SetUIAsLast();
        }
        else if(GameMode == GameTye_Enum.GameType_Appointment)
        {
            GameMain.hall_.gamerooms_.SetUIAsLast();
        }
        return true;
    }    void EnterAppointment()
    {
        MainUITfm.gameObject.SetActive(true);        isOpenAppointmentFinalResults_ = false;
        AppointmentDataManager.AppointmentDataInstance().interrupt = false;
        appointmentTotalResults_ = new List<List<ResultData>>();

        LoadAppintmentReadResource();
        LoadAppointmentResultResource();
        LoadAppointmentRuleResource();
        //if(appointmentPanel_ != null)
        //    appointmentPanel_.transform.SetAsLastSibling();
    }    void LoadAppointmentRuleResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Room_process");
        appointmentRulePanel_ = (GameObject)GameMain.instantiate(obj0);
        appointmentRulePanel_.transform.SetParent(GameCanvas.transform.Find("Root"), false);

        appointmentRulePanel_.transform.Find("Top/ButtonReturn").
            GetComponent<Button>().onClick.AddListener(() => OnClickReturn(true));

        InitRuleData(1);
        appointmentRulePanel_.SetActive(false);
    }    void InitRuleData(uint lunshu)
    {
        if (GameMode != GameTye_Enum.GameType_Appointment)
            return;

        Text ruleTx = appointmentRulePanel_.transform.Find("ImageLeftBG").Find("Text_lunshu").gameObject.GetComponent<Text>();
        AppointmentData data = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();

        if (data == null)
            return;

        if (lunshu > data.playtimes)
            lunshu = data.playtimes;

        if (data.maxpower == 250)
            appointmentGameRule_ = lunshu.ToString() + "/" + data.playtimes.ToString() + "局 不封顶";
        else
            appointmentGameRule_ = lunshu.ToString() + "/" + data.playtimes.ToString() + "局 最高" + data.maxpower + "倍";

        ruleTx.text = appointmentGameRule_;

        appointmentRulePanel_.SetActive(true);
    }    void LoadAppointmentResultResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Match_resultRound_4");
        appointmentResultPerPanel_ = (GameObject)GameMain.instantiate(obj0);
        appointmentResultPerPanel_.transform.SetParent(GameCanvas.transform.Find("Root"), false);
        appointmentResultPerPanel_.SetActive(false);

        UnityEngine.Object obj1 = (GameObject)bundle.LoadAsset("Room_Result_4");
        appointmentResultTotalPanel_ = (GameObject)GameMain.instantiate(obj1);
        appointmentResultTotalPanel_.transform.SetParent(GameCanvas.transform.Find("Root"), false);
        appointmentResultTotalPanel_.SetActive(false);

        XPointEvent.AutoAddListener(appointmentResultTotalPanel_.transform.Find("ImageBG/Buttonclose").gameObject, OnCloseTotalResultPanel, null);
    }

    private void OnCloseTotalResultPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            appointmentResultTotalPanel_.SetActive(false);
            //GameMain.hall_.AnyWhereBackToLoginUI();
            BackToChooseLevel();

            AppointmentDataManager.AppointmentDataInstance().playerAlready = false;        }    }    private void OnClosePerResultPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            bool isback2hall = (bool)button;
            if(isback2hall)
            {
                BackToChooseLevel();
                AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = false;
            }
            else
            {
                appointmentResultPerPanel_.SetActive(false);
                OnEnd();
                //GameMain.hall_.AnyWhereBackToLoginUI();

                isOpenAppointmentFinalResults_ = true;
            }        }    }    void ShowAppointmentTotalResult()
    {
        if (appointmentResultTotalPanel_ == null)
            LoadAppointmentResultResource();

        if (appointmentRulePanel_ != null)
            appointmentRulePanel_.SetActive(false);

        GameObject playerBG = appointmentResultTotalPanel_.transform.Find("ImageBG").Find("Imageplayer").gameObject;
        playerBG.transform.Find("4").gameObject.SetActive(false);

        string rule = "斗地主 ";
        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().maxpower == 250)
            rule += "打" + AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().playtimes.ToString() + "局 不封顶";
        else
            rule += "打" + AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().playtimes.ToString() + 
                "局 最高" + AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().maxpower + "倍";

        AppointmentRecord data = new AppointmentRecord();
        data.gameID = (byte)GameKind_Enum.GameKind_LandLords;
        data.gamerule = rule;
        data.timeseconds = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().createtimeseconds;
        data.isopen = false;
        data.videoes = FriendsMomentsDataMamager.GetFriendsInstance().currentvideoid;
        data.recordTimeSeconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);

        List<Landlord_Role> roles = new List<Landlord_Role> { m_LocalPlayer, m_RightPlayer, m_LeftPlayer };

        Transform playerTransform = null;
        if (appointmentTotalResults_.Count == 0)
        {
            for (int index = 0; index < 3; index++)
            {
                Landlord_Role role = roles[index];

                playerTransform = playerBG.transform.Find((index + 1).ToString());
                Image headimg = playerTransform.Find("Head/HeadMask/ImageHead").GetComponent<Image>();
                headimg.sprite = role.GetHeadImage();
                Text playerNameTx = playerTransform.Find("TextName").gameObject.GetComponent<Text>();
                playerNameTx.text = role.m_cRoleName;

                playerTransform.Find("Text_jifen/TextNum_1").gameObject.SetActive(true);
                playerTransform.Find("Text_jifen/TextNum_2").gameObject.SetActive(false);

                Text totalcoinTx = playerTransform.Find("Text_jifen/TextNum_1").gameObject.GetComponent<Text>();
                totalcoinTx.text = "0";

                AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();
                
                playerdata.playerid = GameMain.hall_.GetPlayerId();
                playerdata.faceid = (int)role.m_nFaceId;
                playerdata.playerName = role.m_cRoleName;
                playerdata.url = role.m_nUrl;
                playerdata.coin = 0;

                if (!data.result.ContainsKey(playerdata.playerid))
                    data.result.Add(playerdata.playerid, playerdata);
                playerTransform.gameObject.SetActive(true);
            }

            GameMain.hall_.gamerooms_.recordlist_.Insert(0, data);
            appointmentResultTotalPanel_.SetActive(true);
            return;
        }


        for (int index = 0; index < roles.Count; index++)
        {
            ResultData rd = appointmentTotalResults_[appointmentTotalResults_.Count - 1].Find(s => s.sit == roles[index].m_nSit);
            Landlord_Role role = roles[index];

            playerTransform = playerBG.transform.Find((index + 1).ToString());
            Image headimg = playerTransform.Find("Head/HeadMask/ImageHead").gameObject.GetComponent<Image>();
            headimg.sprite = role.GetHeadImage();
            Text playerNameTx = playerTransform.Find("TextName").gameObject.GetComponent<Text>();
            playerNameTx.text = role.m_cRoleName;

            bool iswin = role.m_nTotalCoin > 0;
            string totalcoinNode = "TextNum_1";
            if (!iswin)
                totalcoinNode = "TextNum_2";

            playerTransform.Find("Text_jifen/TextNum_1").gameObject.SetActive(iswin);
            playerTransform.Find("Text_jifen/TextNum_2").gameObject.SetActive(!iswin);

            Text totalcoinTx = playerTransform.Find("Text_jifen").Find(totalcoinNode).gameObject.GetComponent<Text>();
            string addorminus = "+";
            if (!iswin)
                addorminus = "";
            totalcoinTx.text = addorminus + role.m_nTotalCoin.ToString();

            AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();

            playerdata.playerid = rd.userId;
            playerdata.faceid = (int)role.m_nFaceId;
            playerdata.playerName = role.m_cRoleName;
            playerdata.url = role.m_nUrl;
            playerdata.coin = role.m_nTotalCoin;

            if (!data.result.ContainsKey(playerdata.playerid))
                data.result.Add(playerdata.playerid, playerdata);
            playerTransform.gameObject.SetActive(true);
        }

        //for (int index = 0; index < AppointmentDataManager.AppointmentDataInstance().resultList.Count; index++)
        //{
        //    int playerindex = index + AppointmentDataManager.AppointmentDataInstance().playerSitNo;
        //    if (playerindex >= 3)
        //        playerindex -= 3;

        //    Image headimg = playerBG.transform.FindChild((index + 1).ToString()).FindChild("Head").
        //        FindChild("HeadMask").FindChild("ImageHead").gameObject.GetComponent<Image>();
        //    //byte sitNo = AppointmentDataManager.AppointmentDataInstance().resultList[index].sitNo;
        //    headimg.sprite = GameMain.hall_.GetHostIconByID(AppointmentDataManager.AppointmentDataInstance().
        //        GetCurrentAppointment().seats_[(byte)playerindex].icon.ToString());
        //    Text playerNameTx = playerBG.transform.FindChild((index + 1).ToString()).FindChild("TextName").gameObject.GetComponent<Text>();
        //    playerNameTx.text = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].playerName;

        //    int coinindex = 0;
        //    for(; coinindex < AppointmentDataManager.AppointmentDataInstance().resultList.Count; coinindex++)
        //    {
        //        if (AppointmentDataManager.AppointmentDataInstance().resultList[coinindex].sitNo == playerindex)
        //            break;
        //    }

        //    bool iswin = AppointmentDataManager.AppointmentDataInstance().resultList[coinindex].coin > 0;
        //    string totalcoinNode = "TextNum_1";
        //    if (!iswin)
        //        totalcoinNode = "TextNum_2";

        //    playerBG.transform.FindChild((index + 1).ToString()).FindChild("Text_jifen").FindChild("TextNum_1").gameObject.SetActive(iswin);
        //    playerBG.transform.FindChild((index + 1).ToString()).FindChild("Text_jifen").FindChild("TextNum_2").gameObject.SetActive(!iswin);

        //    Text totalcoinTx = playerBG.transform.FindChild((index + 1).ToString()).FindChild("Text_jifen").FindChild(totalcoinNode).gameObject.GetComponent<Text>();
        //    string addorminus = "+";
        //    if (!iswin)
        //        addorminus = "";
        //    totalcoinTx.text = addorminus + AppointmentDataManager.AppointmentDataInstance().resultList[coinindex].coin.ToString();

        //    AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();

        //    playerdata.playerid = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].playerid;
        //    playerdata.faceid = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].icon;
        //    playerdata.playerName = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].playerName;
        //    playerdata.url = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].url;

        //    if (!data.result.ContainsKey(playerdata.playerid))
        //        data.result.Add(playerdata.playerid, playerdata);
        //}

        GameMain.hall_.gamerooms_.recordlist_.Insert(0, data);

        for (int index = 0; index < AppointmentDataManager.AppointmentDataInstance().perResultList_.Count; index++)
        {
            LoadAppointmentTotalResultItem(index);
        }

        appointmentResultTotalPanel_.SetActive(true);
        AppointmentDataManager.AppointmentDataInstance().perResultList_.Clear();
    }    void LoadAppointmentTotalResultItem(int index)
    {
        if (appointmentResultTotalPanel_ == null)
            LoadAppointmentResultResource();

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        GameObject itemBG = appointmentResultTotalPanel_.transform.Find("ImageBG").
            Find("Viewport").Find("Content").gameObject;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Room_Result_xiangqing");
        GameObject item = (GameObject)GameMain.instantiate(obj0);
        item.transform.SetParent(itemBG.transform, false);

        Text roundTx = item.transform.Find("Text_ju").Find("Textnum").gameObject.GetComponent<Text>();
        roundTx.text = (index + 1).ToString();

        Transform roleRankingTransform = null;
        List<Landlord_Role> roles = new List<Landlord_Role> { m_LocalPlayer, m_RightPlayer, m_LeftPlayer };
        List<AppointmentResult> resultDatas = AppointmentDataManager.AppointmentDataInstance().perResultList_[index].Values.ToList();
        for (int perindex = 0; perindex < roles.Count; perindex++)
        {
            AppointmentResult rd = resultDatas.Find(s => s.sitNo == roles[perindex].m_nSit);
            if (rd == null)
            {
                Debug.Log("error result key !" + roles[perindex].m_nSit);
                continue;
            }
                
            Landlord_Role role = roles[perindex];
            roleRankingTransform = item.transform.Find("ImageBG/Text_ranking_" + (perindex + 1).ToString());
            Text percoin = roleRankingTransform.GetComponent<Text>();

            percoin.text = rd.coin.ToString();
            roleRankingTransform.gameObject.SetActive(true);
        }

        //for (int perindex = 0; perindex < appointmentTotalResults_[index].Count; perindex++)
        //{
        //    Text percoin = item.transform.FindChild("ImageBG").
        //        FindChild("Text_ranking_" + (perindex + 1).ToString()).gameObject.GetComponent<Text>();

        //    int playerindex = perindex + AppointmentDataManager.AppointmentDataInstance().playerSitNo;
        //    if (playerindex >= 3)
        //        playerindex -= 3;

        //    int coinindex = 0;
        //    for (int tcindex = 0; tcindex < appointmentTotalResults_[index].Count; tcindex++)
        //    {
        //        if (appointmentTotalResults_[index][tcindex].sit == playerindex)
        //        {
        //            coinindex = tcindex;
        //            break;
        //        }
        //    }

        //    percoin.text = appointmentTotalResults_[index][coinindex].coin.ToString();
        //}

        if(AppointmentDataManager.AppointmentDataInstance().perResultList_[index].Count < 4)
        {
            item.transform.Find("ImageBG").
                Find("Text_ranking_4").gameObject.SetActive(false);
        }
    }    IEnumerator ShowAppointmentPerResult(int win, byte springPro, int currentIndex, int totalRound)
    {
        if (springPro > 0)        {            PlayUIAnim("Anime_chuntian", 1.5f, 1085);            yield return new WaitForSecondsRealtime(1.5f);        }

        if (win == 2 || win == -1)            PlayUIAnim("Anime_win1", 1.5f);        else            PlayUIAnim("Anime_win2", 1.5f);
        yield return new WaitForSecondsRealtime(1.5f);
        if (appointmentResultPerPanel_ == null)
            LoadAppointmentResultResource();

        bool isshowtotal = AppointmentDataManager.AppointmentDataInstance().interrupt;
        GameObject confirmBtn = appointmentResultPerPanel_.transform.Find("Button_Confirm").gameObject;
        XPointEvent.AutoAddListener(confirmBtn, OnClosePerResultPanel, isshowtotal);
        confirmBtn.SetActive(currentIndex == totalRound);

        appointmentResultPerPanel_.transform.Find("player_3").gameObject.SetActive(false);

        List<Landlord_Role> roles = new List<Landlord_Role> { m_LocalPlayer, m_RightPlayer, m_LeftPlayer };

        for (int perindex = 0; perindex < roles.Count; perindex++)
        {
            if (currentIndex - 1 >= appointmentTotalResults_.Count)
                currentIndex = appointmentTotalResults_.Count;

            if (currentIndex <= 0)
                continue;

            ResultData rd = appointmentTotalResults_[currentIndex - 1].Find(s => s.sit == roles[perindex].m_nSit);

            GameObject playerResult = appointmentResultPerPanel_.transform.Find("player_" + (perindex + 1).ToString()).gameObject;

            if (perindex == 2)
                playerResult = appointmentResultPerPanel_.transform.Find("player_" + (perindex + 2).ToString()).gameObject;

            Landlord_Role role = roles[perindex];
            bool isadd = rd.coin > 0;
            
            playerResult.transform.Find("Image_add").gameObject.SetActive(true);
            playerResult.transform.Find("Image_subtract").gameObject.SetActive(!isadd);

            string panelName = "Image_add";
            if (!isadd)
            {
                panelName = "Image_subtract";
                playerResult.transform.Find("Image_add").Find("Text_jifen").gameObject.SetActive(false);
                if (perindex == 0)
                {
                    playerResult.transform.Find("Image_add").Find("TextName").gameObject.SetActive(false);
                    playerResult.transform.Find("Image_add").Find("Text_zongfen").gameObject.SetActive(false);
                }
            }
            else
            {
                panelName = "Image_add";
                playerResult.transform.Find("Image_add").Find("Text_jifen").gameObject.SetActive(true);
                if (perindex == 0)
                {
                    playerResult.transform.Find("Image_add").Find("TextName").gameObject.SetActive(true);
                    playerResult.transform.Find("Image_add").Find("Text_zongfen").gameObject.SetActive(true);
                }
            }

            Text playerNameTx = playerResult.transform.Find("Image_add").Find("TextName").gameObject.GetComponent<Text>();

            if (perindex == 0)
                playerNameTx = playerResult.transform.Find(panelName).Find("TextName").gameObject.GetComponent<Text>();

            playerNameTx.text = role.m_cRoleName;
            Text curcoinTx = playerResult.transform.Find(panelName).Find("Text_jifen").Find("TextNum").gameObject.GetComponent<Text>();

            string addorminus = "+";
            if (!isadd)
                addorminus = "";

            curcoinTx.text = addorminus + rd.coin.ToString();

            Text totalcoinTx = playerResult.transform.Find("Image_add").Find("Text_zongfen").Find("TextNum").gameObject.GetComponent<Text>();
            if (perindex == 0)
                totalcoinTx = playerResult.transform.Find(panelName).Find("Text_zongfen").Find("TextNum").gameObject.GetComponent<Text>();
            totalcoinTx.text = role.m_nTotalCoin.ToString();

            Image headimg = playerResult.transform.Find("Image_HeadBG").Find("Image_HeadMask").
                Find("Image_HeadImage").gameObject.GetComponent<Image>();
            headimg.sprite = role.GetHeadImage();
        }

        //for (int perindex = 0; perindex < appointmentTotalResults_[currentIndex - 1].Count; perindex++)
        //{

        //    GameObject playerResult = appointmentResultPerPanel_.transform.FindChild("player_" + (perindex + 1).ToString()).gameObject;

        //    if (perindex == 2)
        //        playerResult = appointmentResultPerPanel_.transform.FindChild("player_" + (perindex + 2).ToString()).gameObject;

        //    int playerindex = perindex + AppointmentDataManager.AppointmentDataInstance().playerSitNo;
        //    if (playerindex >= 3)
        //        playerindex -= 3;

        //    int coinindex = 0;
        //    for (int index = 0; index < appointmentTotalResults_[currentIndex - 1].Count; index++)
        //    {
        //        if (appointmentTotalResults_[currentIndex - 1][index].sit == playerindex)
        //        {
        //            coinindex = index;
        //            break;
        //        }
        //    }

        //    bool isadd = appointmentTotalResults_[currentIndex - 1][coinindex].coin > 0;
        //    playerResult.transform.FindChild("Image_add").gameObject.SetActive(true);
        //    playerResult.transform.FindChild("Image_subtract").gameObject.SetActive(!isadd);

        //    string panelName = "Image_add";
        //    if (!isadd)
        //    {
        //        panelName = "Image_subtract";
        //        playerResult.transform.FindChild("Image_add").FindChild("Text_jifen").gameObject.SetActive(false);
        //        if (perindex == 0)
        //        {
        //            playerResult.transform.FindChild("Image_add").FindChild("TextName").gameObject.SetActive(false);
        //            playerResult.transform.FindChild("Image_add").FindChild("Text_zongfen").gameObject.SetActive(false);
        //        }
        //    }
        //    else
        //    {
        //        panelName = "Image_add";
        //        playerResult.transform.FindChild("Image_add").FindChild("Text_jifen").gameObject.SetActive(true);
        //        if (perindex == 0)
        //        {
        //            playerResult.transform.FindChild("Image_add").FindChild("TextName").gameObject.SetActive(true);
        //            playerResult.transform.FindChild("Image_add").FindChild("Text_zongfen").gameObject.SetActive(true);
        //        }
        //    }

        //    Text playerNameTx = playerResult.transform.FindChild("Image_add").FindChild("TextName").gameObject.GetComponent<Text>();

        //    if (perindex == 0)
        //        playerNameTx = playerResult.transform.FindChild(panelName).FindChild("TextName").gameObject.GetComponent<Text>();

        //    //byte sitNo = appointmentTotalResults_[currentIndex - 1][playerindex].sit;
        //    byte sitNo = (byte)playerindex;

        //    playerNameTx.text = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[sitNo].playerName;
        //    Text curcoinTx = playerResult.transform.FindChild(panelName).FindChild("Text_jifen").FindChild("TextNum").gameObject.GetComponent<Text>();

        //    string addorminus = "+";
        //    if (!isadd)
        //        addorminus = "";

        //    curcoinTx.text = addorminus + appointmentTotalResults_[currentIndex - 1][coinindex].coin.ToString();

        //    Text totalcoinTx = playerResult.transform.FindChild("Image_add").FindChild("Text_zongfen").FindChild("TextNum").gameObject.GetComponent<Text>();
        //    if (perindex == 0)
        //        totalcoinTx = playerResult.transform.FindChild(panelName).FindChild("Text_zongfen").FindChild("TextNum").gameObject.GetComponent<Text>();
        //    totalcoinTx.text = appointmentTotalResults_[currentIndex - 1][coinindex].totalcoin.ToString();

        //    Image headimg = playerResult.transform.FindChild("Image_HeadBG").FindChild("Image_HeadMask").
        //        FindChild("Image_HeadImage").gameObject.GetComponent<Image>();
        //    headimg.sprite = GameMain.hall_.GetHostIconByID(AppointmentDataManager.AppointmentDataInstance().
        //        GetCurrentAppointment().seats_[sitNo].icon.ToString());
        //}

        appointmentResultPerPanel_.SetActive(true);
        isOpenAppointmentFinalResults_ = false;
        AppointmentDataManager.AppointmentDataInstance().playerAlready = false;
    }    void LoadAppintmentReadResource()
    {
        if (appointmentPanel_ != null)
            return;

        appointmentPanel_ = GameMain.hall_.gamerooms_.LoadAppintmentReadResource();
        if(appointmentPanel_ != null)
            InitAppointmentEvents();
    }    void InitAppointmentEvents()
    {
        Button butn = appointmentPanel_.transform.Find("Top/Button_Return").GetComponent<Button>();        butn.onClick.AddListener(() => OnClickReturn(true));
    }    public override void ProcessTick()    {        base.ProcessTick();

        if (m_bReconnected)
        {
            GameMain.hall_.OnGameReconnect(GameType, GameMode);
            m_bReconnected = false;
        }        if (!Load())            return;        CCIMgr.UpdateTimeImage();        m_LeftPlayer.OnTick();        m_RightPlayer.OnTick();        m_LocalPlayer.OnTick();
        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)            return;        if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            if (AppointmentDataManager.AppointmentDataInstance().interrupt)
            {
                AppointmentDataManager.AppointmentDataInstance().interrupt = false;
                isOpenAppointmentFinalResults_ = false;
                AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = false;
                ShowAppointmentTotalResult();
            }
            else if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend)
            {
                if (isOpenAppointmentFinalResults_)
                {
                    AppointmentDataManager.AppointmentDataInstance().interrupt = false;
                    isOpenAppointmentFinalResults_ = false;
                    AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = false;

                    ShowAppointmentTotalResult();
                }
            }
        }    }    void InitPlayers()    {        m_LeftPlayer = new Landlord_Role(this, 3);        m_LeftPlayer.Init();        m_RightPlayer = new Landlord_Role(this, 2);        m_RightPlayer.Init();        m_LocalPlayer = new Landlord_RoleLocal(this, 1);        m_LocalPlayer.Init();        LL_PokerType.CurGrade = 2;
    }    void InitMsgHandle()    {        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_CHOOSElEVEL, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_ENTERMATCH, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_CANCLEMATCH, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_ENTERROOM, HandleStartGame);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_ENTERGAMESTATE, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_BEGINDEALPOKER, HandleBeginPoker);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_ASKBELORDS, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_BELORDSFAILED, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_PUBLISHBELORDS, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_PUBLISHLORDSPOKER, HandleLordPoker);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_ASKDEALPOKER, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_PUBLISHDEALPOKER, HandleDealPoker);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_RESTART, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_PUBLISHRESULT, HandleResult);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_APPLYLEAVEROOM, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_MIDDLEENTERROOM, HandleMiddleEnterRoom);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_AFTERONLOOKERENTER, HandleBystanderEnter);    }    public override bool HandleGameNetMsg(uint _msgType, UMessage _ms)    {        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;        switch (eMsg)        {            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER:                {
                    byte state = _ms.ReadByte();
                    if(state == 0)
                        BackToChooseLevel();
                    else
                        CCustomDialog.OpenCustomConfirmUI(2305);
                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_CHOOSElEVEL:                {                    byte nState = _ms.ReadByte();//1:id非法或Level非法 2：钱不够                    byte level = _ms.ReadByte();                    ushort deskNum = _ms.ReaduShort();                    IsFree = (_ms.ReadByte() == 1);                    if (nState == 0)                    {
                        MatchRoom.GetInstance().AddDesk(deskNum, 3);                    }                    else                    {                        DebugLog.Log("Choose level failed: " + nState);                        if (nState == 2)                        {                            LL_RoomData rd = LandLords_Data.GetInstance().m_RoomData[level];                            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2307, rd.m_nMinCoin);                        }                        else if(nState == 3)
                        {
                            LL_RoomData rd = LandLords_Data.GetInstance().m_RoomData[level];                            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2310, rd.m_nMinCoin);                        }                        else                            CCustomDialog.OpenCustomConfirmUI(2301);                        OnClickReturn(false);                        m_nCurrenLevel = RoomInfo.NoSit;                    }                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_ENTERMATCH:                {                    byte nState = _ms.ReadByte();//1:id非法或Level非法 2：钱不够 3：已经在队列里                    if (nState == 0)                    {                        OnStartEndMatch(true);                    }                    else                    {                        DebugLog.Log("Enter match failed: " + nState);                        if (nState == 2)                        {                            LL_RoomData rd = LandLords_Data.GetInstance().m_RoomData[m_nCurrenLevel - 1];                            CCustomDialog.OpenCustomConfirmUIWithFormatParam(2308, rd.m_nMinCoin);                        }                        else if(nState != 3)                            CCustomDialog.OpenCustomConfirmUI(2301);                    }                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_CANCLEMATCH:                {                    byte nState = _ms.ReadByte();//0：取消成功 1:id非法或Level非法 2：玩家不在队列
                    if(nState == 0)
                        OnStartEndMatch(false);                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_ENTERGAMESTATE:                {                    byte state = _ms.ReadByte();                    OnStateChange((LandRoomState_Enum)state, _ms);                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_ASKBELORDS:                {                    byte sit = _ms.ReadByte();                    uint userid = _ms.ReadUInt();                    ushort curPro = _ms.ReaduShort();                    m_PlayerList[sit].OnAskBeLord(curPro, LandLords_Data.GetInstance().m_nAskLordTime);                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_BELORDSFAILED:                {                    byte nState = _ms.ReadByte();//1:不在房间 2：没轮到 3：房间不在叫地主 4：倍数不对                    DebugLog.Log("be lord failed: " + nState);                    CCustomDialog.OpenCustomConfirmUI(2301);                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_PUBLISHBELORDS:                {                    byte sit = _ms.ReadByte();                    uint userid = _ms.ReadUInt();                    byte askPro = _ms.ReadByte();                    m_PlayerList[sit].BackAskBeLord(askPro);                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_ASKDEALPOKER:                {                    byte sit = _ms.ReadByte();                    uint userid = _ms.ReadUInt();                    uint beforeUserId = _ms.ReadUInt();                    byte beforeSit = _ms.ReadByte();                    float bankTime = _ms.ReadSingle();                    float askTime = _ms.ReadSingle();                    m_PlayerList[sit].OnAskDealPoker(sit == beforeSit, m_PlayerList[beforeSit].m_PlayPokerList, m_PlayerList[beforeSit].CurPokerType, askTime, bankTime);                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_RESTART:                {                    OnEnd();                }                break;            case GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION:                {                    byte sign = _ms.ReadByte();                    byte sit = _ms.ReadByte();                    string name = _ms.ReadString();                    m_PlayerList[sit].OnChat(sign);                }                break;            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_APPLYLEAVEROOM:                {                    byte state = _ms.ReadByte();                    if (state > 0)                    {                        byte sit = _ms.ReadByte();
                        uint userid = _ms.ReadUInt();
                        string name = _ms.ReadString();                        if(userid == GameMain.hall_.GetPlayerId())                            BackToChooseLevel();                    }                    else                    {                        //failed                        CCustomDialog.OpenCustomConfirmUI(2305);                    }                }                break;            default:                break;        }        return true;    }    void BackToChooseLevel()    {        GameOver(false, true);
       if (GameMode == GameTye_Enum.GameType_Appointment)            GameMain.hall_.SwitchToHallScene(true, 0);
        else if (GameMode == GameTye_Enum.GameType_Normal)            MatchRoom.GetInstance().GameOver();
        else
            GameMain.hall_.SwitchToHallScene();
    }    public override void ResetGameUI()    {        base.ResetGameUI();
    }    public override void RefreshGamePlayerCoin(uint AddMoney)
    {
        base.RefreshGamePlayerCoin(AddMoney);

        m_LocalPlayer.m_nTotalCoin += AddMoney;
        m_LocalPlayer.UpdateInfoUI();    }    void LoadResource()    {        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((int)GameKind_Enum.GameKind_LandLords);        if (gamedata == null)            return;        LandlordAssetBundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);        if (LandlordAssetBundle == null)            return;        CommonAssetBundle = AssetBundleManager.GetAssetBundle("pokercommon.resource");        if (CommonAssetBundle == null)            return;        Transform root = GameCanvas.transform.Find("Root");

        //load game ui-----------------------------------------------
        UnityEngine.Object obj = (GameObject)LandlordAssetBundle.LoadAsset("DouDiZhu_Game");        MainUITfm = ((GameObject)GameMain.instantiate(obj)).transform;        MainUITfm.SetParent(root, false);        MainUITfm.gameObject.SetActive(false);        m_LastCard = MainUITfm.Find("Top/Poker_Dipai");        m_LastCard.gameObject.SetActive(false);        Button butn = MainUITfm.Find("Top/ButtonExpand").GetComponent<Button>();        butn.onClick.AddListener(() => OnClickMenu(1));

        butn = MainUITfm.Find("Top/ButtonReturn").GetComponent<Button>();        butn.onClick.AddListener(() => OnClickMenu(2));

        m_GameInfo = MainUITfm.Find("Top/Image_gameinfo");        m_GameInfo.gameObject.SetActive(false);        m_GameButton = MainUITfm.Find("Middle/PlayerInfor/Icon_PlayerInfor_1/PopUp_BG/ButtonGroup");        OutPokerBtn = m_GameButton.Find("Button_xingpaiBG/Button_chupai").GetComponent<Button>();        Button[] buttons;        int index = 0;        foreach (Transform t in m_GameButton)        {            buttons = t.GetComponentsInChildren<Button>(true);            foreach (Button btn in buttons)            {                int temp = index;                btn.onClick.AddListener(() => OnClickGameButton(temp, btn));                index++;            }            index = index / 10 * 10;            index += 10;        }        //load result ui-----------------------------------------------        obj = (GameObject)LandlordAssetBundle.LoadAsset("DouDiZhu_Result");        ResultUITfm = ((GameObject)GameMain.instantiate(obj)).transform;        ResultUITfm.SetParent(root, false);        butn = ResultUITfm.Find("Result_Game/ButtonOk").GetComponent<Button>();
        butn.onClick.AddListener(() => OnClickResultButton(0));
        ResultUITfm.gameObject.SetActive(false);
        //load setup ui-------------        Transform tfm = MainUITfm.Find("Pop-up");        Slider music = tfm.Find("Set/ImageBG/Slider_Music").gameObject.GetComponent<Slider>();        Slider sound = tfm.Find("Set/ImageBG/Slider_Sound").gameObject.GetComponent<Slider>();        music.value = AudioManager.Instance.MusicVolume;        sound.value = AudioManager.Instance.SoundVolume;        music.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.MusicVolume = value; });        sound.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.SoundVolume = value; });        AnimationTfm = tfm.Find("Animation/Imagepoint");        //load chat ui-------------        obj = (GameObject)LandlordAssetBundle.LoadAsset("DouDiZhu_Chat");        ChatUITfm = ((GameObject)GameMain.instantiate(obj)).transform;        ChatUITfm.SetParent(root, false);        buttons = ChatUITfm.Find("Chat_Viewport/Chat_Content").GetComponentsInChildren<Button>();        index = 0;        foreach (Button btn in buttons)        {            int temp = index;            btn.onClick.AddListener(() => OnClickChatButton(temp));            index++;        }        ChatUITfm.gameObject.SetActive(false);        tfm = MainUITfm.Find("Middle/PlayerInfor/Icon_PlayerInfor_1/Button_chat");        tfm.GetComponent<Button>().onClick.AddListener(OnClickChat);

        //load record UI
        obj = (GameObject)LandlordAssetBundle.LoadAsset("Lobby_video_card");        RecordUITfm = ((GameObject)GameMain.instantiate(obj)).transform;        RecordUITfm.SetParent(root, false);        RecordUITfm.gameObject.SetActive(GameMode == GameTye_Enum.GameType_Record);

        GameFunction.PreloadPrefab(CommonAssetBundle, "Anime_startgame");
    }    void EnterRoom(byte level, bool bReconnect, bool bystander = false)    {        Bystander = bystander;        if (m_nCurrenLevel == level)            return;        m_nCurrenLevel = level;        MainUITfm.gameObject.SetActive(true);        PlayerData pd = GameMain.hall_.GetPlayerData();        m_LocalPlayer.m_cRoleName = pd.GetPlayerName();        m_LocalPlayer.m_nFaceId = pd.PlayerIconId;        m_LocalPlayer.m_nUrl = pd.GetPlayerIconURL();        if (GameMode == GameTye_Enum.GameType_Normal)
        {            OnStartEndMatch(false, false);            m_LocalPlayer.m_nTotalCoin = pd.GetCoin();            //CustomAudioDataManager.GetInstance().PlayAudio(1002, false);        }        else
        {            OnStartEndMatch(false, false);
            m_LocalPlayer.m_nTotalCoin = 0;

            if(!bReconnect)
            {
                if (GameMode == GameTye_Enum.GameType_Contest)
                    MatchInGame.GetInstance().ShowWait();
                else if (GameMode == GameTye_Enum.GameType_Record)
                    GameVideo.GetInstance().ShowBegin();            }        }        m_LocalPlayer.SetupInfoUI(GameMode != GameTye_Enum.GameType_Normal, pd.GetPlayerID(), pd.MasterScoreKindArray[(int)GameType], pd.PlayerSexSign);    }    void OnClickReturn(bool playsound)    {        if(playsound)            CustomAudioDataManager.GetInstance().PlayAudio(1089);        if (GameMode == GameTye_Enum.GameType_Normal)        {
            MatchRoom.GetInstance().OnClickReturn(0);        }        else if (GameMode == GameTye_Enum.GameType_Contest)            MatchInGame.GetInstance().OnClickReturn();        else
        {
            if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
                return;

            if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().hostid == GameMain.hall_.GetPlayerId())
                CCustomDialog.OpenCustomDialogWithTipsID(1624, OnExitAppointmentGame);
            else
                CCustomDialog.OpenCustomDialogWithTipsID(1625, OnExitAppointmentGame);
            bExitAppointmentDialogState = true;
            //GameMain.hall_.gamerooms_.Back2Hall();
        }    }

    void OnExitAppointmentGame(object call)
    {
        if ((int)call == 0)            return;

        bExitAppointmentDialogState = false;
        byte sitNo = AppointmentDataManager.AppointmentDataInstance().playerSitNo;
        uint playerid = GameMain.hall_.GetPlayerId();
        UMessage tickmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Exit);

        tickmsg.Add(playerid);
        tickmsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);
        tickmsg.Add((byte)sitNo);
        tickmsg.Add((byte)2);

        NetWorkClient.GetInstance().SendMsg(tickmsg);
    }    void OnClickLevelBtn(byte level)    {        if(m_nCurrenLevel == RoomInfo.NoSit)        {
            //CustomAudioDataManager.GetInstance().PlayAudio(1089);

            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_CM_CHOOSElEVEL);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(level);
            HallMain.SendMsgToRoomSer(msg);            m_nCurrenLevel = 255;        }    }    void OnClickMenu(int btn)    {        CustomAudioDataManager.GetInstance().PlayAudio(1089);

        if (btn == 1)//设置
        {            MainUITfm.Find("Pop-up/Set").gameObject.SetActive(true);        }        else if (btn == 2)        {            OnClickReturn(false);        }    }    void OnClickGameButton(int index, Button btn = null)    {        if(btn != null)            CustomAudioDataManager.GetInstance().PlayAudio(1089);        switch (index)        {            case 0://开始匹配                {                    UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_CM_ENTERMATCH);                    msg.Add(GameMain.hall_.GetPlayerId());                    msg.Add(m_nCurrenLevel);                    HallMain.SendMsgToRoomSer(msg);                }                break;            case 1://取消匹配                {                    UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_CM_CANCLEMATCH);                    msg.Add(GameMain.hall_.GetPlayerId());                    HallMain.SendMsgToRoomSer(msg);                }                break;            case 10://不要            case 11://叫一分            case 12://叫二分            case 13://叫三分                m_LocalPlayer.OnAnswerBeLord((byte)(index - 10));                break;            case 20://不出            case 21://提示            case 22://出牌            case 23://要不起                m_LocalPlayer.OnAnswerPoker((byte)(index - 20), btn);                break;            default:                break;        }    }    void OnEnd()    {        if (m_LocalPlayer == null)            return;        foreach (Transform tfm in AnimationTfm)            GameObject.Destroy(tfm.gameObject);

        if (ResultUITfm.gameObject.activeSelf)        {
            CustomAudioDataManager.GetInstance().PlayAudio(1002, false);
            ResultUITfm.gameObject.SetActive(false);        }        m_GameInfo.gameObject.SetActive(false);        m_LastCard.gameObject.SetActive(false);        if(appointmentResultPerPanel_ != null)            appointmentResultPerPanel_.SetActive(false);        if (appointmentResultTotalPanel_ != null)            appointmentResultTotalPanel_.SetActive(false);        m_LeftPlayer.OnEnd();        m_RightPlayer.OnEnd();        m_LocalPlayer.OnEnd();        GameMain.Instance.StopAllCoroutines();    }    void OnStartEndMatch(bool bStart, bool showBtn = true)    {        if (showBtn)        {            int flag = 0;            if (bStart)                GameKind.AddFlag(1, ref flag);            else                GameKind.AddFlag(0, ref flag);            ShowGameBtn(0, flag);        }        else            ShowGameBtn(-1);        if (bStart)            CCustomDialog.OpenCustomWaitUI(2302, false);        else            CCustomDialog.CloseCustomWaitUI();    }    bool HandleStartGame(uint _msgType, UMessage _ms)    {
        CustomAudioDataManager.GetInstance().PlayAudio(1002, false);
        if (GameMode == GameTye_Enum.GameType_Contest)            MatchInGame.GetInstance().ResetGui();        uint userid = 0;        uint roomId = _ms.ReadUInt();        m_LocalPlayer.m_nTotalCoin = _ms.ReadLong();        PlayerData pd = GameMain.hall_.GetPlayerData();        m_LocalPlayer.m_cRoleName = pd.GetPlayerName();        m_LocalPlayer.m_nFaceId = pd.PlayerIconId;        m_LocalPlayer.m_nUrl = pd.GetPlayerIconURL();        m_LocalPlayer.SetupInfoUI(GameMode != GameTye_Enum.GameType_Normal, pd.GetPlayerID(), pd.MasterScoreKindArray[(int)GameType], pd.PlayerSexSign);        byte sit = _ms.ReadByte();        m_PlayerList[sit] = m_LocalPlayer;        m_LocalPlayer.m_nSit = sit;        Landlord_Role[] roles_ = new Landlord_Role[] { m_LeftPlayer, m_RightPlayer };        if (sit != 1)            Array.Reverse(roles_);        byte num = _ms.ReadByte();        for (byte i = 0; i < num; i++)        {            sit = _ms.ReadByte();            userid = _ms.ReadUInt();            roles_[i].m_nFaceId = _ms.ReadUInt();            roles_[i].m_nUrl = _ms.ReadString();            roles_[i].m_nTotalCoin = _ms.ReadLong();            roles_[i].m_cRoleName = _ms.ReadString();            m_PlayerList[sit] = roles_[i];            roles_[i].m_nSit = sit;            roles_[i].OnDisOrReconnect(_ms.ReadByte() == 0);            roles_[i].SetupInfoUI(GameMode != GameTye_Enum.GameType_Normal, userid, _ms.ReadSingle(), _ms.ReadByte());
        }        OnStartEndMatch(false, false);        if (appointmentRulePanel_ != null)
            appointmentRulePanel_.SetActive(true);        DebugLog.Log("Start landlord game, otherNum:" + num);        return true;    }

    bool HandleMiddleEnterRoom(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        if(state == 0)//失败
        {
            return false;
        }

        LandPlayerState_Enum playerState = (LandPlayerState_Enum)_ms.ReadByte();

        DebugLog.Log("Middle enter room playerState:" + playerState);

        if(playerState == LandPlayerState_Enum.LandPlayerState_OnGameButIsOver)
        {            CCustomDialog.OpenCustomConfirmUI(1621, (param) => BackToChooseLevel());
            return false;
        }

        if (playerState == LandPlayerState_Enum.LandPlayerState_ReadyHall)
        {            MatchRoom.GetInstance().OnEnd();
            return false;
        }
        uint userId = 0;        uint roomId = _ms.ReadUInt();
        byte level = _ms.ReadByte();
        EnterRoom(level, true);
        OnStartEndMatch(false, false);
        GameMain.hall_.CurRoomIndex = level;
        Dictionary<byte, AppointmentRecordPlayer> players = new Dictionary<byte, AppointmentRecordPlayer>();
        AppointmentRecordPlayer player;

        m_LocalPlayer.m_nTotalCoin = _ms.ReadLong();        m_LocalPlayer.UpdateInfoUI();        byte sit = _ms.ReadByte();        byte ready = _ms.ReadByte();        m_PlayerList[sit] = m_LocalPlayer;        m_LocalPlayer.m_nSit = sit;

        PlayerData pd = GameMain.hall_.GetPlayerData();
        player = new AppointmentRecordPlayer();
        player.playerid = pd.GetPlayerID();
        player.faceid = (int)pd.PlayerIconId;
        player.url = pd.GetPlayerIconURL();
        player.coin = m_LocalPlayer.m_nTotalCoin;
        player.playerName = pd.GetPlayerName();
        player.master = pd.MasterScoreKindArray[(int)GameType];
        player.sex = pd.PlayerSexSign;
        player.ready = ready;
        players[sit] = player;


        Landlord_Role[] roles_ = new Landlord_Role[] { m_LeftPlayer, m_RightPlayer };        if (sit != 1)            Array.Reverse(roles_);        byte num = _ms.ReadByte();        float master;        byte sex;        for (byte i = 0; i < num; i++)        {            sit = _ms.ReadByte();            userId = _ms.ReadUInt();            roles_[i].m_nFaceId = _ms.ReadUInt();            roles_[i].m_nUrl = _ms.ReadString();            roles_[i].m_nTotalCoin = _ms.ReadLong();            roles_[i].m_cRoleName = _ms.ReadString();            m_PlayerList[sit] = roles_[i];            roles_[i].m_nSit = sit;            roles_[i].OnPokerNumChanged(_ms.ReadByte());            roles_[i].OnDisOrReconnect(_ms.ReadByte() == 0);            master = _ms.ReadSingle();            sex = _ms.ReadByte();            ready = _ms.ReadByte();            roles_[i].SetupInfoUI(GameMode != GameTye_Enum.GameType_Normal, userId, master, sex);

            player = new AppointmentRecordPlayer();
            player.playerid = userId;
            player.faceid = (int)roles_[i].m_nFaceId;
            player.url = roles_[i].m_nUrl;
            player.coin = roles_[i].m_nTotalCoin;
            player.playerName = roles_[i].m_cRoleName;
            player.master = master;
            player.sex = sex;
            player.ready = ready;
            players[sit] = player;
        }

        uint contestId = _ms.ReadUInt();
        uint appointId = _ms.ReadUInt();

        if(appointId > 0)
        {
            AppointmentData data = new AppointmentData(3);
            data.roomid = appointId;

            byte currentRound = _ms.ReadByte();
            data.playtimes = _ms.ReadByte();
            data.maxpower = (byte)_ms.ReadShort();

            AppointmentDataManager.AppointmentDataInstance().currentRoomID = appointId;
            AppointmentDataManager.AppointmentDataInstance().AddAppointmentData(appointId, data);

            GameMain.hall_.InitRoomsData();

            InitRuleData((uint)(currentRound));
        }

        float timeLeft = _ms.ReadSingle();

        byte pokerNum = _ms.ReadByte();
        m_LocalPlayer.m_HavePokerList.Clear();
        for(byte i = 0; i < pokerNum; i++)
            m_LocalPlayer.m_HavePokerList.Add(_ms.ReadByte());
        m_LocalPlayer.OnPokerNumChanged(pokerNum);

        m_LastCard.gameObject.SetActive(true);
        Sprite spriteBack = CommonAssetBundle.LoadAsset<Sprite>("puke_back");
        foreach (Transform t in m_LastCard)
            t.GetComponent<Image>().sprite = spriteBack;

        LandRoomState_Enum gameState = (LandRoomState_Enum)_ms.ReadByte();
        OnStateChange(gameState, _ms, 1, timeLeft);        if(GameMode == GameTye_Enum.GameType_Normal)
        {
            if (playerState == LandPlayerState_Enum.LandPlayerState_OnDesk)
                MatchRoom.GetInstance().ShowTable(true, players, level, roomId);
            else
                MatchRoom.GetInstance().StartGame();
        }
        return true;
    }

    //mode: 0:normal 1:reconnect 2:bystander
    void OnStateChange(LandRoomState_Enum state, UMessage _ms, byte mode = 0, float timeLeft = 0f)    {        if (m_eRoomState != LandRoomState_Enum.LandRoomState_WaitReady &&            m_eRoomState == state && mode == 0)            return;        DebugLog.Log(string.Format("room state change: ({0}->{1})", m_eRoomState, state));        OnQuitState(m_eRoomState);        m_eRoomState = state;        OnEnterState(m_eRoomState, _ms, mode, timeLeft);    }    void OnQuitState(LandRoomState_Enum state)    {    }    void OnEnterState(LandRoomState_Enum state, UMessage _ms, byte mode, float timeLeft)    {        if (mode > 0)
        {
            switch (state)
            {
                case LandRoomState_Enum.LandRoomState_AskLandlord:
                    {
                        byte curAskSit = _ms.ReadByte();
                        ushort curPro = _ms.ReaduShort();
                        m_PlayerList[curAskSit].OnAskBeLord(curPro, timeLeft);
                    }
                    break;

                case LandRoomState_Enum.LandRoomState_GetLordsPoker:
                    {
                        byte sit = _ms.ReadByte();
                        ushort curPro = _ms.ReaduShort();
                        byte num = _ms.ReadByte();
                        byte[] cards = new byte[num];
                        for (byte i = 0; i < num; i++)
                            cards[i] = _ms.ReadByte();

                        GameMain.SC(ShowLastCards(sit, cards, false));

                        m_GameInfo.gameObject.SetActive(true);
                        m_GameInfo.Find("game_beishu/TextInfo").GetComponent<Text>().text = curPro.ToString();
                    }
                    break;

                case LandRoomState_Enum.LandRoomState_TurnOutPoker:
                    {
                        byte sit = _ms.ReadByte();
                        float bankTime = _ms.ReadSingle();
                        byte beforeSit = _ms.ReadByte();
                        byte beforePokerNum = _ms.ReadByte();
                        byte[] cards = new byte[beforePokerNum];
                        for (int i = 0; i < beforePokerNum; i++)
                            cards[i] = _ms.ReadByte();                        LandPokerType_Enum pokerType = (LandPokerType_Enum)_ms.ReadInt();                        if(beforePokerNum > 0)
                        {                            if(sit != beforeSit)                            {
                                m_PlayerList[beforeSit].OnDealPoker(-1, cards, pokerType);

                                byte start = (byte)((beforeSit + 1) % 3);
                                if (start != sit)//中间隔人(最多隔1人)
                                    m_PlayerList[start].OnDealPoker(-1, new byte[] { });
                            }                        }                        if (!m_GameInfo.gameObject.activeSelf)                            OnEnterState(LandRoomState_Enum.LandRoomState_GetLordsPoker, _ms, mode, 0);                        m_PlayerList[sit].OnAskDealPoker(sit == beforeSit, new List<byte>(cards), pokerType, timeLeft, bankTime);                    }
                    break;

                case LandRoomState_Enum.LandRoomState_OverOnce:
                case LandRoomState_Enum.LandRoomState_Result:
                    {
                        if(mode == 1)
                            HandleResult((uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_PUBLISHRESULT, _ms);
                    }
                    break;
                default:
                    break;
            }        }        else
        {
            if (GameMode == GameTye_Enum.GameType_Normal)
            {
                switch (state)
                {
                    case LandRoomState_Enum.LandRoomState_WaitPlayer:
                        MatchRoom.GetInstance().ShowKickTip(false);
                        break;

                    case LandRoomState_Enum.LandRoomState_WaitReady:
                        {
                            float time = _ms.ReadSingle();
                            MatchRoom.GetInstance().ShowKickTip(true, time);
                        }
                        break;

                    case LandRoomState_Enum.LandRoomState_CountDownBegin:
                        {
                            MatchRoom.GetInstance().StartGame();
                            OnEnd();
                        }
                        break;

                    case LandRoomState_Enum.LandRoomState_End:
                        {
                            if (ResultUITfm == null)
                                break;

                            if (ResultUITfm.gameObject.activeSelf)
                            {
                                Button btn = ResultUITfm.Find("Result_Game/ButtonAgain").GetComponent<Button>();
                                btn.interactable = true;
                                btn.onClick.RemoveAllListeners();
                                btn.onClick.AddListener(() => OnClickResultButton(1, true));
                            }
                        }
                        break;

                    default:
                        break;
                }
            }        }    }    bool HandleBeginPoker(uint _msgType, UMessage _ms)    {
        if (GameMode == GameTye_Enum.GameType_Contest)
        {
            OnEnd();
            MatchInGame.GetInstance().ShowBegin(Bystander);
        }
        else if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            OnEnd();
        }
        PlayUIAnim("Anime_startgame", 1f, 0, null, CommonAssetBundle);

        byte firstAskSit = _ms.ReadByte();        byte num = _ms.ReadByte();        for (int i = 0; i < num; i++)        {            m_LocalPlayer.m_HavePokerList.Add(_ms.ReadByte());        }        GameMain.SC(m_LocalPlayer.BeginPoker());        GameMain.SC(m_LeftPlayer.BeginPoker());        GameMain.SC(m_RightPlayer.BeginPoker());        m_LastCard.gameObject.SetActive(true);        Sprite spriteBack = CommonAssetBundle.LoadAsset<Sprite>("puke_back");        foreach (Transform t in m_LastCard)            t.GetComponent<Image>().sprite = spriteBack;        return true;    }    public void ShowGameBtn(int layer, int flag = -1, bool activeOrEnableChild = true)    {        if (layer < 0)        {            m_GameButton.gameObject.SetActive(false);            return;        }        m_GameButton.gameObject.SetActive(true);        Transform tfm;        Transform child;        for (int i = 0; i < m_GameButton.childCount; i++)        {            tfm = m_GameButton.GetChild(i);            if (i == layer)            {                tfm.gameObject.SetActive(true);                if (flag < 0)                {                    for (int j = 0; j < tfm.childCount; j++)                    {                        child = tfm.GetChild(j);                        child.GetComponent<Button>().interactable = true;                    }                }                else                {                    if (activeOrEnableChild)                    {                        for (int j = 0; j < tfm.childCount; j++)                        {
                            child = tfm.GetChild(j);
                            child.GetComponent<Button>().interactable = true;                            child.gameObject.SetActive(GameKind.HasFlag(j, flag));                        }                    }                    else                    {                        for (int j = 0; j < tfm.childCount; j++)                        {                            tfm.GetChild(j).GetComponent<Button>().interactable = GameKind.HasFlag(j, flag);                        }                    }                }            }            else                tfm.gameObject.SetActive(false);        }    }    bool HandleLordPoker(uint _msgType, UMessage _ms)    {        byte sit = _ms.ReadByte();        uint userid = _ms.ReadUInt();        ushort curPro = _ms.ReaduShort();        byte num = _ms.ReadByte();        byte[] cards = new byte[num];        for (byte i = 0; i < num; i++)        {            cards[i] = _ms.ReadByte();        }        GameMain.SC(ShowLastCards(sit, cards));        m_GameInfo.gameObject.SetActive(true);        m_GameInfo.Find("game_beishu/TextInfo").GetComponent<Text>().text = curPro.ToString();        return true;    }    public IEnumerator ShowLastCards(byte sit, byte[] cards, bool bAdd = true)    {        Sprite sprite;        GameObject go;        float scaleTime = 0.05f;        for (int j = 0; j < cards.Length; j++)        {            go = m_LastCard.GetChild(j).gameObject;            go.transform.DOScaleX(0.1f, scaleTime);            yield return new WaitForSecondsRealtime(scaleTime);            sprite = CommonAssetBundle.LoadAsset<Sprite>(GameCommon.GetPokerMat(cards[j]));            go.GetComponent<Image>().sprite = sprite;            go.transform.DOScaleX(1f, scaleTime);            yield return new WaitForSecondsRealtime(scaleTime);        }        if(bAdd)            m_PlayerList[sit].AddCards(cards);

        yield return new WaitForSecondsRealtime(1f);        foreach (byte i in m_PlayerList.Keys)        {            if (bAdd)
                m_PlayerList[i].OnDealPoker();            m_PlayerList[i].ShowLordIcon(i == sit);        }
    }    bool HandleDealPoker(uint _msgType, UMessage _ms)    {        byte sit = _ms.ReadByte();        uint userid = _ms.ReadUInt();        byte haveRight = _ms.ReadByte();        byte num = _ms.ReadByte();        byte[] cards = new byte[num];        for (byte i = 0; i < num; i++)        {            cards[i] = _ms.ReadByte();        }        ushort curPro = _ms.ReaduShort();        m_GameInfo.Find("game_beishu/TextInfo").GetComponent<Text>().text = curPro.ToString();        LandPokerType_Enum pokerType = (LandPokerType_Enum)_ms.ReadInt();        m_PlayerList[sit].OnDealPoker(haveRight, cards, pokerType);        return true;    }    class ResultData    {        public byte sit;        public byte loseState;        public long coin;        public long totalcoin;        public uint userId;    }    bool HandleResult(uint _msgType, UMessage _ms)    {        uint winUser = _ms.ReadUInt();        uint lordUser = _ms.ReadUInt();        int basePro = _ms.ReadInt();        byte askPro = _ms.ReadByte();        int blustPro = _ms.ReadInt();        byte springPro = _ms.ReadByte();        byte num = _ms.ReadByte();        uint userid;        byte pokerNum;        List<ResultData> resultDataList = new List<ResultData>();        int nWin = 0;        for (int i = 0; i < num; i++)        {            ResultData rd = new ResultData();            rd.sit = _ms.ReadByte();            rd.loseState = _ms.ReadByte();            userid = _ms.ReadUInt();            rd.userId = userid;            m_PlayerList[rd.sit].m_nTotalCoin = _ms.ReadLong();            rd.totalcoin = m_PlayerList[rd.sit].m_nTotalCoin;            rd.coin = _ms.ReadLong();            m_PlayerList[rd.sit].UpdateInfoUI();            pokerNum = _ms.ReadByte();            if (pokerNum > 0)            {                byte[] list = new byte[pokerNum];                for (int j = 0; j < pokerNum; j++)                {                    list[j] = _ms.ReadByte();                }                m_PlayerList[rd.sit].ShowEndPoker(true, list);            }

            if (GameMode == GameTye_Enum.GameType_Normal)
            {                if (userid == lordUser)
                    resultDataList.Insert(0, rd);
                else
                    resultDataList.Add(rd);
            }            else
                resultDataList.Add(rd);

            if (m_PlayerList[rd.sit] == m_LocalPlayer)
            {
                if (userid != lordUser)
                    nWin = rd.coin > 0 ? 1 : -1;
                else
                    nWin = rd.coin > 0 ? 2 : -2;
            }

            if (rd.coin > 0)
                m_PlayerList[rd.sit].PlayRoleAnim("shengli");
            else
                m_PlayerList[rd.sit].PlayRoleAnim("shule", "shule_idle");
        }        byte curRound = _ms.ReadByte();        byte totalRound = _ms.ReadByte();        long vedioId = _ms.ReadLong();        InitRuleData((uint)(curRound + 1));        if (GameMode == GameTye_Enum.GameType_Normal)            GameMain.SC(ShowResult(nWin, basePro, askPro, blustPro, springPro, resultDataList, curRound == totalRound));        else if (GameMode == GameTye_Enum.GameType_Contest)            GameMain.SC(ShowContestResult(nWin, springPro, resultDataList, curRound, totalRound, vedioId));
        else if (GameMode == GameTye_Enum.GameType_Appointment)
        {            appointmentTotalResults_.Add(resultDataList);            GameMain.SC(ShowAppointmentPerResult(nWin, springPro, curRound, totalRound));
        }
        return true;    }    IEnumerator ShowContestResult(int win, byte springPro, List<ResultData> resultDataList, byte curRound, byte totalRound, long vedioId)
    {
        AppointmentRecord record = null;
        if (curRound == totalRound && !Bystander)
        {
            //组装比赛记录数据
            record = new AppointmentRecord();
            record.gameID = (byte)GameKind_Enum.GameKind_LandLords;
            appointmentGameRule_ = "斗地主比赛 第" + MatchInGame.GetInstance().m_nCurTurn.ToString() +"轮";
            record.gamerule = appointmentGameRule_;
            record.timeseconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);
            record.isopen = false;
            record.videoes = vedioId;
        }

        Landlord_Role[] roles = new Landlord_Role[] { m_LocalPlayer, m_LeftPlayer, m_RightPlayer };
        List<RoundResultData> list = new List<RoundResultData>();
        for (int i = 0; i < 3; i++)
        {
            ResultData rd = resultDataList.Find(s => s.sit == roles[i].m_nSit);
            if (rd == null)
            {                list.Add(null);                continue;
            }
            RoundResultData data = new RoundResultData();
            data.headImg = roles[i].GetHeadImage();
            data.coin = roles[i].m_nTotalCoin;
            data.name = roles[i].m_cRoleName;
            data.addCoin = rd.coin;
            list.Add(data);

            if (record != null)
            {
                AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();

                playerdata.playerid = rd.userId;
                playerdata.faceid = (int)roles[i].m_nFaceId;
                playerdata.playerName = roles[i].m_cRoleName;
                playerdata.url = roles[i].m_nUrl;
                playerdata.coin = roles[i].m_nTotalCoin;

                if (!record.result.ContainsKey(playerdata.playerid))
                    record.result.Add(playerdata.playerid, playerdata);
            }        }
        if (record != null)
        {
            if (GameMain.hall_.gamerooms_ == null)
                GameMain.hall_.InitRoomsData();

            GameMain.hall_.gamerooms_.recordlist_.Insert(0, record);
        }

        if (springPro > 0)        {            PlayUIAnim("Anime_chuntian", 1.5f, 1085);            yield return new WaitForSecondsRealtime(1.5f);        }

        if (win == 2 || win == -1)            PlayUIAnim("Anime_win1", 1.5f);        else            PlayUIAnim("Anime_win2", 1.5f);
        yield return new WaitForSecondsRealtime(1.5f);
        CustomAudioDataManager.GetInstance().StopAudio();

        yield return MatchInGame.GetInstance().ShowRoundResult(3, list, () =>        {
            if (curRound == totalRound)
                GameOver(false, true);
            else
                OnEnd();
        }, curRound == totalRound, Bystander);    }

    IEnumerator ShowResult(int win, int basePro, byte askPro, int blustPro, byte springPro, List<ResultData> resultDataList, bool allEnd)    {        yield return new WaitForSecondsRealtime(2f);        if (springPro > 0)        {            PlayUIAnim("Anime_chuntian", 3f, 1085);            yield return new WaitForSecondsRealtime(3f);        }        if(win == 2 || win == -1)            PlayUIAnim("Anime_win1", 3f);        else            PlayUIAnim("Anime_win2", 3f);
        yield return new WaitForSecondsRealtime(3f);        CustomAudioDataManager.GetInstance().StopAudio();        ResultUITfm.gameObject.SetActive(true);        ResultUITfm.Find("Result_Game").gameObject.SetActive(true);        Button butn = ResultUITfm.Find("Result_Game/ButtonAgain").GetComponent<Button>();
        butn.onClick.RemoveAllListeners();
        string str;        Transform tfm = ResultUITfm.Find("Result_Game/animation");        foreach (Transform child in tfm)
            GameObject.Destroy(child.gameObject);
        Color proColor;        if(win > 0)        {            CustomAudioDataManager.GetInstance().PlayAudio(1006);

            GameObject obj = (GameObject)LandlordAssetBundle.LoadAsset("Anime_jiesuan_1");
            obj = GameMain.Instantiate(obj);
            obj.transform.SetParent(tfm, false);
            proColor = new Color(1f, 0.933f, 0.698f);        }        else        {            CustomAudioDataManager.GetInstance().PlayAudio(1005);            GameObject obj = (GameObject)LandlordAssetBundle.LoadAsset("Anime_jiesuan_2");
            obj = GameMain.Instantiate(obj);
            obj.transform.SetParent(tfm, false);
            proColor = new Color(0.8196f, 0.933f, 0.94f);        }        ResultUITfm.Find("Result_Game/Result_BG/TopText/Text_beishu_0/TextNum").GetComponent<Text>().text = basePro.ToString();        ResultUITfm.Find("Result_Game/Result_BG/TopText/Text_beishu_1/TextNum").GetComponent<Text>().text = askPro.ToString();        ResultUITfm.Find("Result_Game/Result_BG/TopText/Text_beishu_2/TextNum").GetComponent<Text>().text = blustPro.ToString();        ResultUITfm.Find("Result_Game/Result_BG/TopText/Text_beishu_3/TextNum").GetComponent<Text>().text = springPro.ToString();        tfm = ResultUITfm.Find("Result_Game/Result_BG/TopText");        Text[] texts = tfm.GetComponentsInChildren<Text>();        foreach(Text t in texts)            t.color = proColor;        Color WSColor = new Color(1f, 0.8627f, 0.3216f);        Color WEColor = new Color(0.9686f, 0.4667f, 0.1294f);        Color LSColor = new Color(0.7176f, 0.9569f, 0.96478f);        Color LEColor = new Color(0.1294f, 0.643f, 0.9686f);        Gradient grad;        for(int i = 0; i < resultDataList.Count; i++)        {            tfm = ResultUITfm.Find("Result_Game/Result_BG/Text_" + (i + 1).ToString());            tfm.Find("Head/HeadMask/ImageHead").GetComponent<Image>().sprite = m_PlayerList[resultDataList[i].sit].GetHeadImage();            tfm.Find("Text_name").GetComponent<Text>().text = m_PlayerList[resultDataList[i].sit].m_cRoleName;            str = resultDataList[i].coin.ToString();            tfm.Find("ImagebrokeIcon").gameObject.SetActive(resultDataList[i].loseState > 0);            tfm = tfm.Find("Text_jifen");            grad = tfm.GetComponent<Gradient>();            if (resultDataList[i].coin > 0)            {                str = "+" + str;                grad.StartColor = WSColor;                grad.EndColor = WEColor;            }            else            {                grad.StartColor = LSColor;                grad.EndColor = LEColor;            }            tfm.GetComponent<Text>().text = str;        }        LL_RoomData rd = LandLords_Data.GetInstance().m_RoomData[m_nCurrenLevel - 1];        tfm = ResultUITfm.Find("Result_Game/Result_BG/Text_tickets");        if (rd.m_nRoomCharge > 0)        {
            tfm.gameObject.SetActive(true);
            tfm.Find("TextNum").GetComponent<Text>().text
                = rd.m_nRoomCharge.ToString();        }        else            tfm.gameObject.SetActive(false);

        if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            bool isshowtotal = AppointmentDataManager.AppointmentDataInstance().interrupt || allEnd;
            XPointEvent.AutoAddListener(butn.gameObject, OnClosePerResultPanel, isshowtotal);

            butn.gameObject.SetActive(allEnd);
        }
        else
        {
            butn.gameObject.SetActive(true);
            butn.onClick.AddListener(() => OnClickResultButton(1, allEnd));
            butn.interactable = !allEnd || m_eRoomState == LandRoomState_Enum.LandRoomState_End || 
                                m_eRoomState < LandRoomState_Enum.LandRoomState_CountDownBegin;
        }
    }    void GameOver(bool showMatch = false, bool quit = false)    {        if (m_LocalPlayer == null)            return;        OnEnd();        m_LeftPlayer.Init();        m_RightPlayer.Init();        m_PlayerList.Clear();        m_nWaitingLoad = 0;

        if (quit)
        {            IsFree = false;            m_nCurrenLevel = RoomInfo.NoSit;
        }        if(bExitAppointmentDialogState)
        {
            CCustomDialog.CloseCustomDialogUI();
        }        OnStartEndMatch(false, showMatch);        ResetGameUI();

        if (Bystander)
        {
            Bystander = false;
            OnLeaveLookRoom();
        }    }    void OnClickResultButton(int index, bool allEnd = false)    {        if (GameMode != GameTye_Enum.GameType_Normal)
            return;

        CustomAudioDataManager.GetInstance().PlayAudio(1089);

        if (index == 0)//离开
        {
            MatchRoom.GetInstance().OnClickReturn(index);
        }
        else if (index == 1)//继续
        {
            if (allEnd)
                MatchRoom.GetInstance().OnClickReturn(index);
            else
                OnEnd();
        }
        else if (index == 2)
        {
            Player.ShareImageToWechat(false);
        }
    }    void OnClickChat()    {        CustomAudioDataManager.GetInstance().PlayAudio(1089);        ChatUITfm.gameObject.SetActive(true);    }    void OnClickChatButton(int index)    {        CustomAudioDataManager.GetInstance().PlayAudio(1089);        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION);        msg.Add(GameMain.hall_.GetPlayerId());        msg.Add((byte)index);        HallMain.SendMsgToRoomSer(msg);        ChatUITfm.gameObject.SetActive(false);    }    public void PlayUIAnim(string animation, float lifeTime, int audioID = 0, Transform parentTfm = null, AssetBundle bundle = null)    {        if(bundle == null)            bundle = LandlordAssetBundle;        UnityEngine.Object obj = (GameObject)bundle.LoadAsset(animation);        GameObject gameObj = (GameObject)GameMain.instantiate(obj);        gameObj.transform.SetParent(parentTfm == null ? AnimationTfm : parentTfm, false);        DragonBones.UnityArmatureComponent animate = gameObj.GetComponentInChildren<DragonBones.UnityArmatureComponent>();        animate.animation.Play("newAnimation");        if(lifeTime > 0f)            GameObject.Destroy(gameObj, lifeTime);        if(audioID != 0)            CustomAudioDataManager.GetInstance().PlayAudio(audioID);    }
    public override void OnDisconnect(bool over)
    {
        if (over)
            GameOver();
        else
            OnEnd();
    }    public override void ReconnectSuccess()
    {
        m_bReconnected = true;
    }

    public override void SetupVideo(List<AppointmentRecordPlayer> players)
    {
        if (players == null)
            return;

        uint localId = GameMain.hall_.GetPlayerId();
        byte localSit = (byte)players.FindIndex(s => s.playerid == localId);
        List<Landlord_Role> roles;
        if (localSit == 0)
            roles = new List<Landlord_Role> { m_LocalPlayer, m_RightPlayer, m_LeftPlayer };
        else if (localSit == 1)
            roles = new List<Landlord_Role> { m_LeftPlayer, m_LocalPlayer, m_RightPlayer };
        else
            roles = new List<Landlord_Role> { m_RightPlayer, m_LeftPlayer, m_LocalPlayer };

        AppointmentRecordPlayer info;
        for (byte i = 0; i < players.Count; i++)        {            info = players[i];            roles[i].m_nUrl = info.url;            roles[i].m_cRoleName = info.playerName;            roles[i].m_nTotalCoin = 0;            m_PlayerList[i] = roles[i];            roles[i].m_nSit = i;
            roles[i].SetupInfoUI(GameMode != GameTye_Enum.GameType_Normal, info.playerid, info.master, info.sex);
        }

    }

    public override bool OnVideoStep(List<VideoAction> actionList, int curStep, bool reverse)
    {
        if (curStep > actionList.Count || curStep < 0)
            return true;

        VideoAction action = actionList[curStep];

        DebugLog.Log("Landlord OnVideoStep:" + action.vai + " rev:" + reverse);
        List<int> list = action.list;
        int j = 0;
        bool res = true;

        if (!reverse)
        {
            switch (action.vai)
            {
                //case VideoActionInfo_Enum.VideoActionInfo_1:
                //    OnStateChange((LandRoomState_Enum)list[0], null);
                //    break;

                case VideoActionInfo_Enum.VideoActionInfo_2:
                    {
                        OnEnd();

                        //list[0] 第一个出牌的玩家的座位号
                        //list[1] 每个玩家多少张牌的数量
                        //list[1] = 每桌多少人
                        j = 3;
                        for (int i = 0; i < list[2]; i++)
                        {
                            byte sit = (byte)list[j++];// 玩家的座位号
                            int userId = list[j++];//玩家的useid
                            for (int k = 0; k < list[1]; k++)
                            {
                                byte poker = (byte)list[j++];
                                m_PlayerList[sit].m_HavePokerList.Add(poker);
                            }
                            m_PlayerList[sit].UpdateRecordCards();
                        }

                        m_LastCard.gameObject.SetActive(true);
                        Sprite spriteBack = CommonAssetBundle.LoadAsset<Sprite>("puke_back");
                        foreach (Transform t in m_LastCard)
                            t.GetComponent<Image>().sprite = spriteBack;
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_3:
                    {
                        byte sit = (byte)list[0];
                        float time = (float)list[1];
                        byte curPro = (byte)list[2];
                        m_PlayerList[sit].OnAskBeLord(curPro, time);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_4:
                    {
                        byte sit = (byte)list[0];
                        byte curPro = (byte)list[1];
                        int num = list[2];
                        j = 3;
                        byte[] cards = new byte[num];
                        for (byte i = 0; i < num; i++)
                            cards[i] = (byte)list[j++];

                        GameMain.SC(ShowLastCards(sit, cards, false));

                        m_PlayerList[sit].AddCards(cards, false);

                        m_GameInfo.gameObject.SetActive(true);
                        m_GameInfo.Find("game_beishu/TextInfo").GetComponent<Text>().text = curPro.ToString();
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_5:
                    {
                        byte sit = (byte)list[j++];
                        uint userId = (uint)list[j++];
                        byte beforeSit = (byte)list[j++];
                        float bankTime = list[j++];
                        float askTime = list[j++];
                        m_PlayerList[sit].OnAskDealPoker(sit == beforeSit, m_PlayerList[beforeSit].m_PlayPokerList,                             m_PlayerList[beforeSit].CurPokerType, askTime, bankTime, GameVideo.GetInstance().m_bPause);                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_6:
                    {
                        byte sit = (byte)list[0];
                        byte askPro = (byte)list[1];
                        m_PlayerList[sit].BackAskBeLord(askPro);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_7:
                    {
                        byte sit = (byte)list[0];
                        short haveRight = (short)list[1];
                        int num = list[2];
                        j = 3;
                        byte[] cards = new byte[num];
                        for (int i = 0; i < num; i++)
                            cards[i] = (byte)list[j++];                        int curPro = list[j++];                        LandPokerType_Enum pokerType = (LandPokerType_Enum)list[j++];                        m_PlayerList[sit].OnDealPoker(haveRight, cards, pokerType);                    }
                    break;

                default:
                    res = false;
                    break;
            }        }        else
        {
            switch (action.vai)
            {
                case VideoActionInfo_Enum.VideoActionInfo_2:
                    {
                        OnEnd();
                    }                    break;

                case VideoActionInfo_Enum.VideoActionInfo_4:
                    {
                        byte sit = (byte)list[0];
                        int num = list[2];
                        j = 3;
                        byte[] cards = new byte[num];
                        for (int i = 0; i < num; i++)
                            cards[i] = (byte)list[j++];                        m_PlayerList[sit].RemoveCards(cards);
                        m_PlayerList[sit].ShowLordIcon(false);

                        m_GameInfo.gameObject.SetActive(false);
                        Sprite spriteBack = CommonAssetBundle.LoadAsset<Sprite>("puke_back");
                        foreach (Transform t in m_LastCard)
                            t.GetComponent<Image>().sprite = spriteBack;
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_3:
                case VideoActionInfo_Enum.VideoActionInfo_5:
                    {
                        byte sit = (byte)list[j++];
                        m_PlayerList[sit].OnDealPoker(0);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_6:
                    {
                        byte sit = (byte)list[0];
                        byte askPro = (byte)list[1];
                        m_PlayerList[sit].ShowLordIcon(false);
                        m_PlayerList[sit].BackAskBeLord(askPro);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_7:
                    {
                        byte sit = (byte)list[0];
                        short haveRight = (short)list[1];
                        int num = list[2];
                        j = 3;
                        byte[] cards = new byte[num];
                        for (int i = 0; i < num; i++)
                            cards[i] = (byte)list[j++];                        m_PlayerList[sit].AddCards(cards, false);                        m_PlayerList[sit].OnDealPoker();

                        foreach (Transform tfm in AnimationTfm)
                            GameObject.Destroy(tfm.gameObject);
                        OnVideoStep(actionList, curStep - 1, false);

                        while (haveRight == 0 && curStep > 0)
                        {
                            curStep -= 1;
                            VideoAction lastAction = actionList[curStep];
                            if(lastAction.vai == VideoActionInfo_Enum.VideoActionInfo_7)
                            {
                                byte lastSit = (byte)lastAction.list[0];
                                if (lastSit == sit)
                                    break;

                                haveRight = (short)lastAction.list[1];

                                num = lastAction.list[2];
                                j = 3;
                                cards = new byte[num];
                                for (int i = 0; i < num; i++)
                                    cards[i] = (byte)lastAction.list[j++];
                                LandPokerType_Enum pokerType = (LandPokerType_Enum)lastAction.list[j++];
                                m_PlayerList[lastSit].OnDealPoker(-1, cards, pokerType);
                            }
                        }
                    }
                    break;

                default:
                    res = false;
                    break;
            }        }        return res;    }

    public override void OnVideoReplay()
    {
        base.OnVideoReplay();

        OnEnd();
    }

    public override void OnPlayerDisOrReconnect(bool disconnect, uint userId, byte sit)
    {
        if (m_PlayerList.ContainsKey(sit))
            m_PlayerList[sit].OnDisOrReconnect(disconnect);
        else if (GameMode == GameTye_Enum.GameType_Normal)
            MatchRoom.GetInstance().SetPlayerOffline(sit, disconnect);
    }


    bool HandleBystanderEnter(uint _msgType, UMessage _ms)
    {
        uint roomId = _ms.ReadUInt();
        byte level = _ms.ReadByte();
        uint contestId = _ms.ReadUInt();
        byte curRound = _ms.ReadByte();
        byte maxRound = _ms.ReadByte();

        if (GameMode == GameTye_Enum.GameType_Contest)
            MatchInGame.GetInstance().ShowBegin(true, curRound, maxRound);
        EnterRoom(level, true, true);
        OnStartEndMatch(false, false);
        byte sit;        uint userid = 0;        Landlord_Role[] roles_ = new Landlord_Role[] { m_LocalPlayer, m_RightPlayer, m_LeftPlayer };        byte num = _ms.ReadByte();        for (byte i = 0; i < num; i++)        {            sit = _ms.ReadByte();            userid = _ms.ReadUInt();            roles_[i].m_nFaceId = _ms.ReadUInt();            roles_[i].m_nUrl = _ms.ReadString();            roles_[i].m_nTotalCoin = _ms.ReadLong();            roles_[i].m_cRoleName = _ms.ReadString();            m_PlayerList[sit] = roles_[i];            roles_[i].m_nSit = sit;            roles_[i].OnPokerNumChanged(_ms.ReadByte());            roles_[i].SetupInfoUI(GameMode != GameTye_Enum.GameType_Normal, userid, _ms.ReadSingle(), _ms.ReadByte());
        }

        m_LastCard.gameObject.SetActive(true);
        Sprite spriteBack = CommonAssetBundle.LoadAsset<Sprite>("puke_back");
        foreach (Transform t in m_LastCard)
            t.GetComponent<Image>().sprite = spriteBack;

        float timeLeft = _ms.ReadSingle();
        LandRoomState_Enum gameState = (LandRoomState_Enum)_ms.ReadByte();
        OnStateChange(gameState, _ms, 2, timeLeft);

        CustomAudioDataManager.GetInstance().PlayAudio(1002, false);
        return true;
    }

    void OnLeaveLookRoom()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_CM_LEAVEONLOOKERROOM);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((uint)0);
        HallMain.SendMsgToRoomSer(msg);
    }

    public override void StartLoad()
    {
        m_nWaitingLoad = 0;
    }

    public LandPokerType_Enum GetPokersType(List<byte> list, List<byte> curPokerList, LandPokerType_Enum curType, ref List<byte> childValidList)
    {
        if (list.Count == 0)
            return LandPokerType_Enum.LandPokerType_Error;

        if (list.Count == 1)
        {            if (curPokerList.Count == 0)
            {                childValidList = new List<byte>(list);                return LandPokerType_Enum.LandPokerType_Single;
            }
            if (curPokerList.Count == 1 && LL_PokerType.GetPokerLogicValue(list[0]) > LL_PokerType.GetPokerLogicValue(curPokerList[0]))
            {                childValidList = new List<byte>(list);                return LandPokerType_Enum.LandPokerType_Single;
            }
            return LandPokerType_Enum.LandPokerType_Error;
        }

        Dictionary<int, List<byte>> curValueList;        LL_PokerType.GetValueList(curPokerList, out curValueList);

        Dictionary<int, List<byte>> valueList;        LL_PokerType.GetValueList(list, out valueList);

        int curBiggest = 0;
        int biggest = 0;
        List<byte> curChildValidList = new List<byte>();

        if (list.Count == 2)
        {            if (LL_PokerType.IsKingBlust(valueList, ref childValidList, ref biggest) != 0)                return LandPokerType_Enum.LandPokerType_KingBlast;            if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPairs(curValueList, ref curChildValidList, ref curBiggest) != 0)            {
                if (LL_PokerType.IsPairs(valueList, ref childValidList, ref biggest) != 0)
                    if (biggest > curBiggest)
                        return LandPokerType_Enum.LandPokerType_Pairs;            }        }        else if (list.Count == 3)
        {
            if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsThree(curValueList, ref curChildValidList, ref curBiggest) != 0)            {
                if (LL_PokerType.IsThree(valueList, ref childValidList, ref biggest) != 0)
                    if (biggest > curBiggest)
                        return LandPokerType_Enum.LandPokerType_Three;
            }        }        else if (list.Count == 4)
        {            LL_PokerType.IsFour(curValueList, ref curChildValidList, ref curBiggest);            if (LL_PokerType.IsFour(valueList, ref childValidList, ref biggest) != 0)
                if (biggest > curBiggest)
                    return LandPokerType_Enum.LandPokerType_Four;
            curBiggest = 0;            if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsThreeAndOne(curValueList, ref curChildValidList, ref curBiggest) != 0)
            {                if (LL_PokerType.IsThreeAndOne(valueList, ref childValidList, ref biggest) != 0)
                    if (biggest > curBiggest)
                        return LandPokerType_Enum.LandPokerType_ThreeAndOne;
            }        }
        else if (list.Count >= 5)
        {
            if (list.Count % 2 == 0)
            {
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsSeriesPairs(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {
                    if (LL_PokerType.IsSeriesPairs(valueList, ref childValidList, ref biggest) != 0)
                        if (biggest > curBiggest && (curPokerList.Count == 0 || curPokerList.Count == list.Count))
                            return LandPokerType_Enum.LandPokerType_SeriesPairs;
                }            }            curBiggest = 0;            if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsFlush(curValueList, ref curChildValidList, ref curBiggest) != 0)
            {                if (LL_PokerType.IsFlush(valueList, ref childValidList, ref biggest) > 0)
                    if (biggest > curBiggest && (curPokerList.Count == 0 || childValidList.Count == curPokerList.Count))
                        return LandPokerType_Enum.LandPokerType_Flush;
            }            if (curPokerList.Count != 0 && list.Count != curPokerList.Count)
                return LandPokerType_Enum.LandPokerType_Error;

            if (list.Count == 5)
            {
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsThreeAndTwo(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsThreeAndTwo(valueList, ref childValidList, ref biggest) != 0)
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_ThreeAndTwo;
                }            }
            else if (list.Count == 6)
            {
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsFourAndTwo(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsFourAndTwo(valueList, ref childValidList, ref biggest) == 1)
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_FourAndTwoSingle;
                }
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneNoWith(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneNoWith(valueList, ref childValidList, ref biggest) != 0)
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_PlaneNoWith;
                }            }
            else if (list.Count == 8)
            {
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneWithOther(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneWithOther(valueList, ref childValidList, ref biggest) == 1)    //飞机带两个单牌 2*3+2
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_PlaneWithOherSingle;
                }
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsFourAndTwo(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsFourAndTwo(valueList, ref childValidList, ref biggest) == 2)
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_FourAndTwoPair;
                }            }
            else if (list.Count == 9)
            {
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneNoWith(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneNoWith(valueList, ref childValidList, ref biggest) != 0)     //飞机不带
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_PlaneNoWith;
                }            }
            else if (list.Count == 10)
            {
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneWithOther(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneWithOther(valueList, ref childValidList, ref biggest) == 2)    //飞机带两个对子 2*3+4
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_PlaneWithOherPair;
                }            }
            else if (list.Count == 12)
            {
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneNoWith(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneNoWith(valueList, ref childValidList, ref biggest) != 0)    //4个的飞机
                        return LandPokerType_Enum.LandPokerType_PlaneNoWith;
                }
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneWithOther(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneWithOther(valueList, ref childValidList, ref biggest) == 1)    //3个的飞机带三个单牌 3*3+3
                        return LandPokerType_Enum.LandPokerType_PlaneWithOherSingle;
                }            }
            else if (list.Count == 15)
            {
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneWithOther(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneWithOther(valueList, ref childValidList, ref biggest) == 2)   //3个的飞机  带3个对子 3*3+6
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_PlaneWithOherPair;
                }
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneNoWith(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneNoWith(valueList, ref childValidList, ref biggest) != 0)    //5个的飞机
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_PlaneNoWith;
                }            }
            else if (list.Count == 16)
            {
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneWithOther(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneWithOther(valueList, ref childValidList, ref biggest) == 1)   //4个的飞机  带4个单牌 4*3+4
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_PlaneWithOherSingle;
                }            }
            else if (list.Count == 18)
            {
                curBiggest = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || LL_PokerType.IsPlaneNoWith(curValueList, ref curChildValidList, ref curBiggest) != 0)
                {                    if (LL_PokerType.IsPlaneNoWith(valueList, ref childValidList, ref biggest) != 0)    //6个的飞机
                        if (biggest > curBiggest)
                            return LandPokerType_Enum.LandPokerType_PlaneNoWith;
                }            }
            else if (list.Count == 20)
            {
                curBiggest = 0;
                int curNum = 0;
                if (curType == LandPokerType_Enum.LandPokerType_Error || (curNum = LL_PokerType.IsPlaneWithOther(curValueList, ref curChildValidList, ref curBiggest)) != 0)
                {                    int nNum = LL_PokerType.IsPlaneWithOther(valueList, ref childValidList, ref biggest);
                    if (biggest > curBiggest && (curNum == 0 || curNum == nNum))
                    {
                        if (nNum == 1)  // 5*3+5
                            return LandPokerType_Enum.LandPokerType_PlaneWithOherSingle;
                        else if (nNum == 2) //4*3+8
                            return LandPokerType_Enum.LandPokerType_PlaneWithOherPair;
                    }
                }            }
        }
        return LandPokerType_Enum.LandPokerType_Error;
    }}