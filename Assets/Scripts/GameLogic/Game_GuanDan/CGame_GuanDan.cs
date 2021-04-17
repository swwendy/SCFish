﻿using DG.Tweening;
using UnityEngine;
[LuaCallCSharp]
public enum GuandanPokerType_Enum
    //四王是最大的牌>六张和六张以上炸弹>同花顺>五张炸弹>四张炸弹>其它牌型。
    GuanPokerType_Error = 0,
    GuanPokerType_Single,           //单牌
    GuanPokerType_Pairs,            //一对对子
    GuanPokerType_Three,            //三张一样点数
    GuanPokerType_ThreeAndTwo,      //三张带两张
    GuanPokerType_SeriesPairs,      //连续的3对牌
    GuanPokerType_SeriesThree,      //连续的两副三张
    GuanPokerType_Flush,            //连续的5张牌，花色不同
    GuanPokerType_Blast,            //大于或者等于4张一样的牌
    GuanPokerType_SameColorFlush,   //同花顺
    GuanPokerType_KingBlast,        //4个鬼牌组成的炸弹

    GuanPokerType_Max
};

//房间状态
[LuaCallCSharp]
public enum GuanDanRoomState_Enum
    GuanDanRoomState_Init = 0,
    GuanDanRoomState_WaitPlayer,            //等人
    GuanDanRoomState_WaitReady,         //等待准备
    GuanDanRoomState_TotalBegin,            //总的开始

    GuanDanRoomState_OnceBeginShow,     //每局开始前的显示
    GuanDanRoomState_CountDownBegin,        //游戏开始倒计时
    GuanDanRoomState_DealPoker,             //发牌
    GuanDanRoomState_SubmitPoker,           //第二局之后的上贡（升级）
    GuanDanRoomState_ReturnPoker,           //第二局之后的还贡（升级）
    GuanDanRoomState_MatchSit,              //第二局之后的配桌（不是升级）
    GuanDanRoomState_StoodUp,               //抗贡（升级）
    GuanDanRoomState_OpenMatchPoker,        //翻牌的状态（不是升级）
    GuanDanRoomState_TurnOutPoker,          //轮流出牌
    GuanDanRoomState_OnceResult,            //一局结果
    GuanDanRoomState_OnceEnd,               //

    GuanDanRoomState_TotalResult,           //总的结果
    GuanDanRoomState_TotalEnd,              //

    GuanDanRoomState_Max
};
[LuaCallCSharp]
public enum GuandanPlayerState_Enum
{
    GuandanPlayerState_Init = 0,
    GuandanPlayerState_Match,
    GuandanPlayerState_GameIn,
    GuandanPlayerState_OnGameButIsOver,//3游戏中但游戏已经结束
    GuandanPlayerState_ReadyHall,   //玩家在大厅
    GuandanPlayerState_OnDesk,      //玩家在桌子上
    GuandanPlayerState
};

public enum PokerSortType
    ePST_Horizontal,
    ePST_Vertical,
    ePST_Max
};
    Transform AnimationTfm;
    Dictionary<byte, byte> m_dictSitPlayer = new Dictionary<byte, byte>();

    bool m_bReconnected = false;
    bool bExitAppointmentDialogState = false;

        GameCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        if (!m_bReconnected)
        {
            if (GameMode == GameTye_Enum.GameType_Appointment)
            {
                LoadAppintmentReadResource();
                m_nWaitingLoad = -1;
            }
            else if (GameMode == GameTye_Enum.GameType_Normal)
            {
                MatchRoom.GetInstance().ShowRoom((byte)GameType);
                OnClickLevelBtn(GameMain.hall_.CurRoomIndex);

                m_nWaitingLoad = -1;
            }
        else if(GameMode == GameTye_Enum.GameType_Normal)
        {
        }

    bool Load()
    {
        if (m_PlayerList.Count == 0)
        {
        }
        LoadResource();
        if (GameMode == GameTye_Enum.GameType_Contest || GameMode == GameTye_Enum.GameType_Record)
            EnterRoom(1, m_bReconnected);
        else if (GameMode == GameTye_Enum.GameType_Appointment)
            EnterAppointment();
        else
            EnterRoom(GameMain.hall_.CurRoomIndex, m_bReconnected);

        if (GameMode == GameTye_Enum.GameType_Normal)
        {
            MatchRoom.GetInstance().SetUIAsLast();
        }
        else if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            GameMain.hall_.gamerooms_.SetUIAsLast();
        }
        return true;
    }

    void EnterAppointment()
    {
        MainUITfm.gameObject.SetActive(true);

        appointmentTotalResults_ = new List<Dictionary<byte, RoundResultData>>();
        isOpenAppointmentFinalResults_ = false;
        AppointmentDataManager.AppointmentDataInstance().interrupt = false;
        AppointmentDataManager.AppointmentDataInstance().interruptid = 0;

        Debug.Log("init results===============");

        LoadAppintmentReadResource();
        LoadAppointmentResultResource();
        LoadAppointmentRuleResource(1);
        //if(appointmentPanel_ != null)
        //    appointmentPanel_.transform.SetAsLastSibling();
    }

    void LoadAppintmentReadResource()
    {
        //if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
        //    return;

        if (appointmentPanel_ != null)
            return;

        appointmentPanel_ = GameMain.hall_.gamerooms_.LoadAppintmentReadResource();
        if (appointmentPanel_ != null)
            InitAppointmentEvents();
    }

    void InitAppointmentEvents()
    {
        Button butn = appointmentPanel_.transform.Find("Top/Button_Return").GetComponent<Button>();
    }
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((int)GameKind_Enum.GameKind_GuanDan);
        if (bundle == null)
            return;

        UnityEngine.Object obj1 = (GameObject)bundle.LoadAsset("Room_Result_4");
        appointmentResultTotalPanel_ = (GameObject)GameMain.instantiate(obj1);
        appointmentResultTotalPanel_.transform.SetParent(GameCanvas.transform.Find("Root"), false);
        appointmentResultTotalPanel_.SetActive(false);

        XPointEvent.AutoAddListener(appointmentResultTotalPanel_.transform.Find("ImageBG/Buttonclose").gameObject, OnCloseTotalResultPanel, null);
    }

    private void OnClosePerResultPanel(EventTriggerType eventtype, object button, PointerEventData eventData)
            bool isback2hall = (bool)button;
            if(isback2hall)
            {
                BackToChooseLevel();
                AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = false;
            }
            else
            {
                OnEnd();
                //GameMain.hall_.AnyWhereBackToLoginUI();

                isOpenAppointmentFinalResults_ = true;
            }

    private void OnCloseTotalResultPanel(EventTriggerType eventtype, object button, PointerEventData eventData)
            appointmentResultTotalPanel_.SetActive(false);
            //GameMain.hall_.AnyWhereBackToLoginUI();
            BackToChooseLevel();

    {
        if (GameMode != GameTye_Enum.GameType_Appointment)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if(appointmentRulePanel_ == null)
        {
            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Room_process");
            appointmentRulePanel_ = (GameObject)GameMain.instantiate(obj0);
            appointmentRulePanel_.transform.SetParent(GameCanvas.transform.Find("Root"), false);
            appointmentRulePanel_.transform.Find("Top/ButtonReturn").
                GetComponent<Button>().onClick.AddListener(() => OnClickReturn(true));
        }

        Text ruleTx = appointmentRulePanel_.transform.Find("ImageLeftBG").Find("Text_lunshu").gameObject.GetComponent<Text>();
        GuanDanAppointmentData data = (GuanDanAppointmentData)AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();

        if (data == null)
            return;

        if (lunshu > data.terData_.times)
            lunshu = data.terData_.times;

        if (data.terData_.playType == TePlayType.Times)
        {
            if (data.terData_.cp == CurrentPoker.Two)
                appointmentGameRule_ = lunshu.ToString() + "/" + data.terData_.times.ToString() + "局 双下" + data.terData_.score.ToString() + "分 打2";
            else
                appointmentGameRule_ = lunshu.ToString() + "/" + data.terData_.times.ToString() + "局 双下" + data.terData_.score.ToString() + "分 随机打";
        }
        else
        {
            if (data.terData_.vectory == 1)
                appointmentGameRule_ = "连续打 过A胜利";
            else
                appointmentGameRule_ = "连续打 过" + data.terData_.vectory.ToString() + "胜利";
        }

        ruleTx.text = appointmentGameRule_;

        appointmentRulePanel_.SetActive(isShow);
    }

    void ShowAppointmentTotalResult()
    {
        if (appointmentResultTotalPanel_ == null)
            LoadAppointmentResultResource();

        if(appointmentRulePanel_ != null)
            appointmentRulePanel_.SetActive(false);

        GameObject playerBG = appointmentResultTotalPanel_.transform.Find("ImageBG").Find("Imageplayer").gameObject;
        //playerBG.transform.FindChild("4").gameObject.SetActive(false);

        GuanDanAppointmentData appointmentData = (GuanDanAppointmentData)AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();
        if(appointmentData == null)
        {
            return;
        }

        string rule = "掼蛋 ";
        if (appointmentData.terData_.playType == TePlayType.Times)
        {
            if (appointmentData.terData_.cp == CurrentPoker.Two)
                rule += "打" + appointmentData.terData_.times.ToString() + "局 双下" + appointmentData.terData_.score.ToString() + "分 打2";
            else
                rule += "打" + appointmentData.terData_.times.ToString() + "局 双下" + appointmentData.terData_.score.ToString() + "分 随机打";
        }
        else
        {
            if (appointmentData.terData_.vectory == 1)
                rule = "连续打 过A胜利";
            else
                rule = "连续打 过" + appointmentData.terData_.vectory.ToString() + "胜利";
        }

        AppointmentRecord recordData = new AppointmentRecord();
        recordData.gameID = (byte)GameKind_Enum.GameKind_GuanDan;
        recordData.gamerule = rule;
        recordData.timeseconds = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().createtimeseconds;
        recordData.isopen = false;
        recordData.videoes = FriendsMomentsDataMamager.GetFriendsInstance().currentvideoid;
        recordData.recordTimeSeconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);
        Transform playerTransform = null;
        if (appointmentTotalResults_.Count == 0)
        {
            for (int index = 0; index < 4; index++)
            {
                GuanDan_Role role = m_PlayerList[index];
                playerTransform = playerBG.transform.Find((index + 1).ToString());
                Image headimg = playerTransform.Find("Head/HeadMask/ImageHead").gameObject.GetComponent<Image>();
                headimg.sprite = role.GetHeadImg().sprite;
                Text playerNameTx = playerBG.transform.Find((index + 1).ToString()).Find("TextName").gameObject.GetComponent<Text>();
                playerNameTx.text = role.GetName();

                playerTransform.Find("Text_jifen/TextNum_1").gameObject.SetActive(true);
                playerTransform.Find("Text_jifen/TextNum_2").gameObject.SetActive(false);

                Text totalcoinTx = playerTransform.Find("Text_jifen/TextNum_1").gameObject.GetComponent<Text>();
                totalcoinTx.text = "0";

                AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();

                playerdata.playerid = GameMain.hall_.GetPlayerId();
                playerdata.faceid = role.m_nFaceId;
                playerdata.playerName = role.GetName();
                playerdata.url = role.m_szUrl;
                playerdata.coin = 0;

                if (!recordData.result.ContainsKey(playerdata.playerid))
                    recordData.result.Add(playerdata.playerid, playerdata);
                playerTransform.gameObject.SetActive(true);
            }

            GameMain.hall_.gamerooms_.recordlist_.Insert(0, recordData);
            appointmentResultTotalPanel_.SetActive(true);
            return;
        }

        //Dictionary<byte, AppointmentSeat> seats = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_;
        Dictionary<byte, RoundResultData> resultDatas = appointmentTotalResults_[appointmentTotalResults_.Count - 1];


        foreach (var v in resultDatas)
        {
            //RoundResultData rd = resultDatas.Find(s => s.playerid == seat.Value.playerid);
            //if (rd == null)
            //{
            //    Debug.Log("error result key !" + seat.Value.playerid);
            //    continue;
            //}

            RoundResultData rd = v.Value;
            GuanDan_Role role = m_PlayerList[m_dictSitPlayer[v.Key]];
            long coin = rd.coin;

            playerTransform = playerBG.transform.Find((v.Key + 1).ToString());
            Image headimg = playerTransform.Find("Head/HeadMask/ImageHead").GetComponent<Image>();
            headimg.sprite = role.GetHeadImg().sprite;
            Text playerNameTx = playerTransform.Find("TextName").gameObject.GetComponent<Text>();
            playerNameTx.text = role.GetName();

            bool iswin = coin > 0;
            string totalcoinNode = "TextNum_1";
            if (!iswin)
                totalcoinNode = "TextNum_2";

            playerTransform.Find("Text_jifen/TextNum_1").gameObject.SetActive(iswin);
            playerTransform.Find("Text_jifen/TextNum_2").gameObject.SetActive(!iswin);

            Text totalcoinTx = playerTransform.Find("Text_jifen").Find(totalcoinNode).gameObject.GetComponent<Text>();
            string addorminus = "+";
            if (!iswin)
                addorminus = "";
            totalcoinTx.text = addorminus + coin.ToString();

            AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();

            playerdata.playerid = rd.playerid;
            playerdata.faceid = role.m_nFaceId;
            playerdata.playerName = role.GetName();
            playerdata.url = role.m_szUrl;
            playerdata.coin = rd.coin;

            if (!recordData.result.ContainsKey(playerdata.playerid))
                recordData.result.Add(playerdata.playerid, playerdata);
            playerTransform.gameObject.SetActive(true);
        }

        //for (int index = 0; index < AppointmentDataManager.AppointmentDataInstance().resultList.Count; index++)
        //{
        //    byte switchSitNo = AppointmentDataManager.AppointmentDataInstance().playerSitNo;

        //    int playerindex = index + switchSitNo;
        //    if (playerindex >= 4)
        //        playerindex -= 4;

        //    Image headimg = playerBG.transform.FindChild((index + 1).ToString()).FindChild("Head").
        //        FindChild("HeadMask").FindChild("ImageHead").gameObject.GetComponent<Image>();
        //    //byte sitNo = AppointmentDataManager.AppointmentDataInstance().resultList[index].sitNo;
        //    headimg.sprite = GameMain.hall_.GetHostIconByID(AppointmentDataManager.AppointmentDataInstance().
        //        GetCurrentAppointment().seats_[(byte)playerindex].icon.ToString());
        //    Text playerNameTx = playerBG.transform.FindChild((index + 1).ToString()).FindChild("TextName").gameObject.GetComponent<Text>();
        //    playerNameTx.text = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].playerName;

        //    int coinindex = 0;
        //    for (; coinindex < AppointmentDataManager.AppointmentDataInstance().resultList.Count; coinindex++)
        //    {
        //        if (AppointmentDataManager.AppointmentDataInstance().resultList[coinindex].playerid
        //            == AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].playerid)
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

        //   playerdata.playerid = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].playerid;
        //    playerdata.faceid = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].icon;
        //    playerdata.playerName = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].playerName;
        //    playerdata.url = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)playerindex].url;
        //    playerdata.coin = AppointmentDataManager.AppointmentDataInstance().resultList[coinindex].coin;

        //    if (!data.result.ContainsKey(playerdata.playerid))
        //        data.result.Add(playerdata.playerid, playerdata);
        //}

        GameMain.hall_.gamerooms_.recordlist_.Insert(0, recordData);

        for (int index = 0; index < AppointmentDataManager.AppointmentDataInstance().perResultList_.Count; index++)
        {
            LoadAppointmentTotalResultItem(index);
        }

        appointmentResultTotalPanel_.SetActive(true);
        AppointmentDataManager.AppointmentDataInstance().perResultList_.Clear();
    }
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
        //Dictionary<byte, AppointmentSeat> seats = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_;
        List<AppointmentResult> resultDatas = AppointmentDataManager.AppointmentDataInstance().perResultList_[index].Values.ToList();
        foreach(var rd in resultDatas)
        {
            //AppointmentResult rd = resultDatas.Find(s => s.playerid == seat.Value.playerid);
            //if (rd == null)
            //{
            //    Debug.Log("error result key !" + seat.Value.playerid);
            //    continue;
            //}
            GuanDan_Role role = m_PlayerList[m_dictSitPlayer[rd.sitNo]];
            roleRankingTransform = item.transform.Find("ImageBG/Text_ranking_" + (rd.sitNo + 1).ToString());
            Text percoin = roleRankingTransform.GetComponent<Text>();
            string addorminus = "+";
            if (rd.coin < 0)
                addorminus = "";

            percoin.text = addorminus + rd.coin.ToString();
            roleRankingTransform.gameObject.SetActive(true);
        }

        //GuanDan_Role role;
        ////Text text;
        //for (int i = 0; i < 2; i++)
        //{
        //    for (int j = 0; j < 2; j++)
        //    {
        //        Text percoin = item.transform.FindChild("ImageBG").
        //            FindChild("Text_ranking_" + (i * 2 + j + 1).ToString()).gameObject.GetComponent<Text>();

        //        //byte switchSitNo = AppointmentDataManager.AppointmentDataInstance().playerSitNo;

        //        //int playerindex = i + 2 * j + switchSitNo;
        //        //if (playerindex >= 4)
        //        //    playerindex -= 4;

        //        //role = m_PlayerList[playerindex];
        //        role = m_PlayerList[i + 2 * j];

        //        //Debug.Log("((((((length:" + appointmentTotalResults_[index][i * 2 + j].rankSitInfo.Count.ToString() + "(((((key:" + role.m_nSrvSit.ToString() + "keys====");
        //        //foreach (byte key in appointmentTotalResults_[index][i * 2 + j].rankSitInfo.Keys)
        //        //    Debug.Log(key + ",");
        //        //

        //        string addorminus = "+";
        //        if (appointmentTotalResults_[index][role.m_nSrvSit].coin < 0)
        //            addorminus = "";

        //        percoin.text = addorminus + appointmentTotalResults_[index][role.m_nSrvSit].coin.ToString();

        //for (int perindex = 0; perindex < appointmentTotalResults_[index].Count; perindex++)
        //{
        //    Text percoin = item.transform.FindChild("ImageBG").
        //        FindChild("Text_ranking_" + (perindex + 1).ToString()).gameObject.GetComponent<Text>();

        //    int playerindex = perindex + AppointmentDataManager.AppointmentDataInstance().playerSitNo;
        //    if (playerindex >= 4)
        //        playerindex -= 4;

        //    int coinindex = 0;
        //    for (int tcindex = 0; tcindex < appointmentTotalResults_[index].Count; tcindex++)
        //    {
        //        if (appointmentTotalResults_[index][tcindex].sitNo == playerindex)
        //        {
        //            coinindex = tcindex;
        //            break;
        //        }
        //    }

        //    percoin.text = appointmentTotalResults_[index][coinindex].coin.ToString();
        //}

        if (AppointmentDataManager.AppointmentDataInstance().perResultList_[index].Count < 4)
        {
            item.transform.Find("ImageBG").
                Find("Text_ranking_4").gameObject.SetActive(false);
        }
    }


        if (m_bReconnected)
        {
            GameMain.hall_.OnGameReconnect(GameType, GameMode);
            m_bReconnected = false;
        }
            return;
        {
            role.OnTick();
        }
        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
            return;

        if (GameMode == GameTye_Enum.GameType_Appointment)
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
        }
    }
        {
            role = new GuanDan_RoleOther(this, i);
            role.Init();
            m_PlayerList.Add(role);
        }
    }
                (uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_SUBMITPOKER, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
                (uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_RETURNPOKER, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
                    byte state = _ms.ReadByte();
                    if (state == 0)
                        BackToChooseLevel();
                    else
                        CCustomDialog.OpenCustomConfirmUI(2305);
                }
                    byte level = _ms.ReadByte();
                    }
                    }

            case GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_ENTERMATCH:

                    if (nState == 0)
                    if (nState == 0)
                        OnStartEndMatch(false);

            case GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_ROOMSTATE:
            case GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_LETOUTPOKER:
                    {
                    }

            case GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_SUBMITPOKER:
                {
                    byte state = _ms.ReadByte();//1:上贡过了 2:没有这张牌
                    byte sit = _ms.ReadByte();
                    byte poker = _ms.ReadByte();
                    if (state == 0)
                    {
                        ShowGong(true, 1, true, new byte[] { sit }, poker);
                    }
                    {
                        m_PlayerList[m_dictSitPlayer[sit]].OnGong(0, true);
                        if (state == 3)
                            CRollTextUI.Instance.AddVerticalRollText(2703);
                        else
                            CRollTextUI.Instance.AddVerticalRollText(2700);
                break;

            case GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_RETURNPOKER:
                {
                    byte state = _ms.ReadByte();//1:还贡过了 2:没有这张牌 3：不能大于10
                    byte sit = _ms.ReadByte();
                    byte poker = _ms.ReadByte();
                    if (state == 0)
                    {
                    }
                    {
                        m_PlayerList[m_dictSitPlayer[sit]].OnGong(0, false);
                        if (state == 3)
                            CRollTextUI.Instance.AddVerticalRollText(2702);
                        else
                            CRollTextUI.Instance.AddVerticalRollText(2701);
                break;
                        uint userid = _ms.ReadUInt();
                        string name = _ms.ReadString();
                        //failed
                        CCustomDialog.OpenCustomConfirmUI(2305);

            case GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_AFTERALLSUBMIT:
                    byte doubleDown = _ms.ReadByte();
                    byte poker = _ms.ReadByte();
                    byte sit = _ms.ReadByte();
                    m_PlayerList[m_dictSitPlayer[sit]].OnDealPoker();
                    byte revSit = _ms.ReadByte();
                    m_PlayerList[m_dictSitPlayer[revSit]].OnGong(2, eMsg == GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_AFTERALLSUBMIT, poker, m_PlayerList[m_dictSitPlayer[sit]].m_PlayCards.transform);

                    if(doubleDown == 1)
                    {
                        poker = _ms.ReadByte();
                        sit = _ms.ReadByte();
                        m_PlayerList[m_dictSitPlayer[sit]].OnDealPoker();
                        revSit = _ms.ReadByte();
                        m_PlayerList[m_dictSitPlayer[revSit]].OnGong(2, eMsg == GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_AFTERALLSUBMIT, poker, m_PlayerList[m_dictSitPlayer[sit]].m_PlayCards.transform);
                    }
                }

            case GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION:

    byte GetClientSit(byte sit, byte localSit)
    {
        return (byte)((4 + sit - localSit) % 4);
    }


       if(GameMode == GameTye_Enum.GameType_Appointment)
        else if (GameMode == GameTye_Enum.GameType_Normal)
            MatchRoom.GetInstance().GameOver();
        else
            GameMain.hall_.SwitchToHallScene();

        PlayerData pd = GameMain.hall_.GetPlayerData();
        m_PlayerList[0].UpdateInfoUI();

        for (int i = 0; i < 2; i++)
        {
            m_RoundTipTfm.GetChild(i).
                Find("Texttime").GetComponent<Text>().text = "";
        }
    }

        GameObject obj;
        Button butn;
        Transform tfm;
        Sprite sprite;
        byte index = 0;

        PlayerData pd = GameMain.hall_.GetPlayerData();

        sprite = GameMain.hall_.GetIcon(pd.GetPlayerIconURL(), pd.GetPlayerID(), (int)pd.PlayerIconId);

        //load game ui-----------------------------------------------
        MainUITfm = root.Find("GuanDan_Game");
        if(MainUITfm == null)
        {
            obj = (GameObject)GuanDanAssetBundle.LoadAsset("GuanDan_Game");
            MainUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
            MainUITfm.SetParent(root, false);
        }
        //menu
        butn = MainUITfm.Find("Top/ButtonExpand").GetComponent<Button>();

        butn = MainUITfm.Find("Top/ButtonReturn").GetComponent<Button>();

        tfm = MainUITfm.Find("Pop-up/Set/ImageBG");
        Slider music = tfm.Find("Slider_Music").GetComponent<Slider>();
        Slider sound = tfm.Find("Slider_Sound").GetComponent<Slider>();
        music.value = AudioManager.Instance.MusicVolume;
        sound.value = AudioManager.Instance.SoundVolume;
        music.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.MusicVolume = value; });
        sound.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.SoundVolume = value; });
        if (GameMode == GameTye_Enum.GameType_Record)
            CurPOT = PokerSortType.ePST_Vertical;
        else
        {
            tfm = MainUITfm.Find("Pop-up/Set/ImageBG");
            Toggle t = tfm.Find("paixu/Toggle_1").GetComponent<Toggle>();
            t.onValueChanged.AddListener((isOn) => { if (isOn) OnPokerSortTypeChanged(PokerSortType.ePST_Horizontal); });
            if (t.isOn)
                CurPOT = PokerSortType.ePST_Horizontal;
            t = tfm.Find("paixu/Toggle_2").GetComponent<Toggle>();
            t.onValueChanged.AddListener((isOn) => { if (isOn) OnPokerSortTypeChanged(PokerSortType.ePST_Vertical); });
            if (t.isOn)
                CurPOT = PokerSortType.ePST_Vertical;
        }
        AnimationTfm = MainUITfm.Find("Pop-up/Animation/Imagepoint");
        m_RoundTipTfm = MainUITfm.Find("Top/ImageLV");
        ShowRoundTip(false);

        //logic buttons
        tfm = MainUITfm.Find("Middle/PlayerInfor_1_1/ButtonBG/Button_chat");
        tfm.GetComponent<Button>().onClick.AddListener(OnClickChat);
        tfm.GetComponent<Button>().onClick.AddListener(OnClickChat);

        //load result ui-----------------------------------------------
        obj = (GameObject)GuanDanAssetBundle.LoadAsset("GuanDan_Result");
        ResultUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        ResultUITfm.SetParent(root, false);
        ResultUITfm.gameObject.SetActive(false);

        butn = ResultUITfm.Find("ImageBG/ImageButtonBG/Button_likai").GetComponent<Button>();
        butn = ResultUITfm.Find("ImageBG/Button_xiangqing").GetComponent<Button>();

        //load chat ui-------------
        obj = (GameObject)GuanDanAssetBundle.LoadAsset("GuanDan_Chat");

        //load record UI
        obj = (GameObject)GuanDanAssetBundle.LoadAsset("Lobby_video_card");
        RecordUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        RecordUITfm.SetParent(root, false);
        RecordUITfm.gameObject.SetActive(GameMode == GameTye_Enum.GameType_Record);
        GameFunction.PreloadPrefab(GuanDanAssetBundle, "anime_lost");

        GameFunction.PreloadPrefab(CommonAssetBundle, "Anime_startgame");
    }

    void EnterRoom(byte level, bool bReconnect, bool bystander = false)
        Bystander = bystander;
            return;

        m_nCurrenLevel = level;

        PlayerData pd = GameMain.hall_.GetPlayerData();
        {
            loclRole.m_nTotalCoin = pd.GetCoin();
            //CustomAudioDataManager.GetInstance().PlayAudio(1001, false);
        {
            loclRole.m_nTotalCoin = 0;

            if(!bReconnect)
            {
                if (GameMode == GameTye_Enum.GameType_Contest)
                    MatchInGame.GetInstance().ShowWait();
                else if (GameMode == GameTye_Enum.GameType_Record)
                    GameVideo.GetInstance().ShowBegin();
            //TryToLeaveRoom
            MatchRoom.GetInstance().OnClickReturn(0);
        }
        {
            if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
                return;

            if(AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().hostid == GameMain.hall_.GetPlayerId())
                CCustomDialog.OpenCustomDialogWithTipsID(1624, OnExitAppointmentGame);
            else
                CCustomDialog.OpenCustomDialogWithTipsID(1625, OnExitAppointmentGame);
            bExitAppointmentDialogState = true;
            //CCustomDialog.OpenCustomConfirmUI(2305);
        }
    {
        if ((int)call == 0)

        bExitAppointmentDialogState = false;
        byte sitNo = AppointmentDataManager.AppointmentDataInstance().playerSitNo;
        uint playerid = GameMain.hall_.GetPlayerId();
        UMessage tickmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Exit);

        tickmsg.Add(playerid);
        tickmsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);
        tickmsg.Add((byte)sitNo);
        tickmsg.Add((byte)2);

        NetWorkClient.GetInstance().SendMsg(tickmsg);
    }
            //CustomAudioDataManager.GetInstance().PlayAudio(1005);

            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_CM_CHOOSELEVEL);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(level);
            HallMain.SendMsgToRoomSer(msg);

            m_nCurrenLevel = 255;
        }

        if (btn == 1)//设置
        {
    void OnClickChat()
            ResultUITfm.gameObject.SetActive(false);
        
            role.OnEnd();

        if (appointmentResultTotalPanel_ != null)

    //mode: 0:normal 1:reconnect 2:bystander
    void OnStateChange(GuanDanRoomState_Enum state, UMessage _ms, byte mode = 0, float timeLeft = 0f)
                    ClearUIAnimation();
                }
            case GuanDanRoomState_Enum.GuanDanRoomState_OnceBeginShow:
                {
                    CustomAudioDataManager.GetInstance().PlayAudio(1001, false);
                }
                break;
            default:
                break;
        }
    }
                {
                    if (GameMode == GameTye_Enum.GameType_Normal)
                        MatchRoom.GetInstance().ShowKickTip(false);

            case GuanDanRoomState_Enum.GuanDanRoomState_WaitReady:
                    if (GameMode == GameTye_Enum.GameType_Normal)
                    {
                        MatchRoom.GetInstance().ShowKickTip(true, time);
                    }
                {
                    if (GameMode == GameTye_Enum.GameType_Contest)
                        MatchInGame.GetInstance().ShowBegin(Bystander);
                    else if (GameMode == GameTye_Enum.GameType_Normal)
                        MatchRoom.GetInstance().StartGame();
                }
                break;

            case GuanDanRoomState_Enum.GuanDanRoomState_SubmitPoker:
                    byte doubleDown = _ms.ReadByte();

                    byte gongSit = _ms.ReadByte();

                    byte poker0 = 0, poker1 = 0;
                    byte[] sits = new byte[doubleDown == 1 ? 2 : 1];
                    sits[0] = gongSit;
                    if (mode > 0)
                        poker0 = _ms.ReadByte();

                    if(doubleDown == 1)
                    {
                        gongSit = _ms.ReadByte();
                        sits[1] = gongSit;
                        if (mode > 0)
                            poker1 = _ms.ReadByte();
                    }

                    ShowGong(true, 0, true, sits, 0, false, mode > 0 ? timeLeft : -1f);

                    if (poker0 != 0)
                        ShowGong(true, 1, true, new byte[] { sits[0] }, poker0);
                    if (poker1 != 0)
                        ShowGong(true, 1, true, new byte[] { sits[1] }, poker1);

                    PlayUIAnim("anime_gong", -1f);
                }

            case GuanDanRoomState_Enum.GuanDanRoomState_ReturnPoker:
                    byte doubleDown = _ms.ReadByte();
                    byte gongSit = _ms.ReadByte();

                    byte poker0 = 0, poker1 = 0;
                    byte[] sits = new byte[doubleDown == 1 ? 2 : 1];
                    sits[0] = gongSit;
                    if (mode > 0)
                        poker0 = _ms.ReadByte();

                    if (doubleDown == 1)
                    {
                        gongSit = _ms.ReadByte();
                        sits[1] = gongSit;
                        if (mode > 0)
                            poker1 = _ms.ReadByte();
                    }

                    ShowGong(true, 0, false, sits);

                    if (poker0 != 0)
                        ShowGong(true, 1, false, new byte[] { sits[0] }, poker0);
                    if (poker1 != 0)
                        ShowGong(true, 1, false, new byte[] { sits[1] }, poker1);

                    PlayUIAnim("anime_gong", -1f);
                }
                break;

            case GuanDanRoomState_Enum.GuanDanRoomState_StoodUp:
                {
                    PlayUIAnim("anime_kanggong", 2f);

                    byte num = _ms.ReadByte();
                    for(int i = 0; i < num; i++)
                    {
                        byte sit = _ms.ReadByte();
                        byte jokerNum = _ms.ReadByte();

                        m_PlayerList[m_dictSitPlayer[sit]].ShowTipPoker(0x42, jokerNum - 1);
                    }
                }
                break;

            case GuanDanRoomState_Enum.GuanDanRoomState_OpenMatchPoker:
                {
                    foreach (GuanDan_Role role in m_PlayerList)
                        role.OnGameStart();

                    byte firstSit = _ms.ReadByte();
                    ShowRoundTip(true, 4, firstSit);
                }
                break;

            case GuanDanRoomState_Enum.GuanDanRoomState_TurnOutPoker:
                {
                    ShowGong(false);
                    {
                        byte sit = _ms.ReadByte();
                        float bankTime = _ms.ReadSingle();
                        byte beforeSit = _ms.ReadByte();
                        byte beforePokerNum = _ms.ReadByte();
                        byte[] cards = new byte[beforePokerNum];
                        for (int i = 0; i < beforePokerNum; i++)
                            cards[i] = _ms.ReadByte();
                        {
                            GuanDan_Role role = m_PlayerList[m_dictSitPlayer[beforeSit]];
                            if(jieFeng > 0)
                            {
                                if (role.Rank > 0)
                                    role.OnDealPoker();
                            }
                            {

                                byte start = (byte)((beforeSit + 1) % 4);
                                if(start != sit)//中间隔人
                                {
                                    byte end = (byte)((sit + 3)%4);
                                    byte[] card = new byte[] { };
                                    if (end < start)
                                    {
                                        byte temp = start;
                                        start = end;
                                        end = start;
                                    }
                                    for (byte s = start; s <= end; s++)
                                        m_PlayerList[m_dictSitPlayer[s]].OnDealPoker(-1, card);

            case GuanDanRoomState_Enum.GuanDanRoomState_OnceEnd:
                {
                    if (mode == 1)
                        HandleResult((uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_PUBLISHRESULT, _ms);

                    if (ResultUITfm)
                    {
                        Button btn = ResultUITfm.Find("ImageBG/ImageButtonBG/Button_jixu").GetComponent<Button>();
                        btn.interactable = true;
                        Button liKaiButton = ResultUITfm.Find("ImageBG/ImageButtonBG/Button_likai").GetComponent<Button>();
                        liKaiButton.interactable = true;
                    }
                }
                break;

            case GuanDanRoomState_Enum.GuanDanRoomState_TotalEnd:
                {
                    if (ResultUITfm == null)
                        break;

                    if (ResultUITfm.gameObject.activeSelf)
                    {
                        Button btn = ResultUITfm.Find("ImageBG/ImageButtonBG/Button_jixu").GetComponent<Button>();
                        btn.interactable = true;
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => OnClickResultButton(1, true));

                        Button liKaiButton = ResultUITfm.Find("ImageBG/ImageButtonBG/Button_likai").GetComponent<Button>();
                        liKaiButton.interactable = true;
                    }
                break;

    void ChangeSit(byte changeSit1, byte changeSit2, float flyTime = 1.5f)
    {
        byte localSrvSit = m_PlayerList[0].m_nSrvSit;
        DebugLog.Log("ChangeSit local:" + localSrvSit + " change1:" + changeSit1 + " change2:" + changeSit2);

        byte num = 4;
        int[] cSits = new int[num];
        for (byte i = 0; i < num; i++)
        {
            if (i == m_dictSitPlayer[changeSit1])
                cSits[i] = m_dictSitPlayer[changeSit2];
            else if (i == m_dictSitPlayer[changeSit2])
                cSits[i] = m_dictSitPlayer[changeSit1];
            else
                cSits[i] = i;
        }

        int offsetSit = cSits[0];
        localSrvSit = (byte)((localSrvSit + offsetSit) % 4);
        Dictionary<byte, byte> clientChangeDict = new Dictionary<byte, byte>();
        byte cSit, tCSit;
        for (byte i = 0; i < num; i++)
        {
            cSits[i] = (cSits[i] - offsetSit + 4) % 4;
            tCSit = (byte)cSits[i];
            clientChangeDict[tCSit] = i;

            cSit = GetClientSit(i, localSrvSit);
            m_dictSitPlayer[i] = cSit;
            m_PlayerList[cSit].m_nSrvSit = i;
        }

        foreach (var change in clientChangeDict)
        {
            tCSit = change.Key;
            cSit = change.Value;
            m_PlayerList[cSit].ChangeSit(cSit != tCSit, flyTime,
                m_PlayerList[tCSit].GetHeadImg().transform.position, m_PlayerList[clientChangeDict[cSit]]);
        }
    }

    bool HandleStartGame(uint _msgType, UMessage _ms)
        OnStateChange(state, null);

        m_dictSitPlayer[localSit] = 0;
        m_PlayerList[0].m_nSrvSit = localSit;
        PlayerData pd = GameMain.hall_.GetPlayerData();
        byte otherNum = _ms.ReadByte();
        for (byte i = 0; i < otherNum; i++)
        {
            byte sit = _ms.ReadByte();
            byte clientSit = GetClientSit(sit, localSit);
            m_dictSitPlayer[sit] = clientSit;
            m_PlayerList[clientSit].m_nSrvSit = sit;
            uint userId = _ms.ReadUInt();
            int faceid = _ms.ReadInt();
            string url = _ms.ReadString();
            long coin = _ms.ReadLong();
            string name = _ms.ReadString();
            m_PlayerList[clientSit].OnDisOrReconnect(_ms.ReadByte() == 0);
                coin, userId, name, url, faceid, _ms.ReadSingle(), _ms.ReadByte());
        }

            appointmentRulePanel_.SetActive(true);
    {
        byte state = _ms.ReadByte();
        if (state == 0)//失败
        {
            return false;
        }

        GuandanPlayerState_Enum playerState = (GuandanPlayerState_Enum)_ms.ReadByte();

        DebugLog.Log("Middle enter room playerState:" + playerState);

        if (playerState == GuandanPlayerState_Enum.GuandanPlayerState_OnGameButIsOver)
        {
            CCustomDialog.OpenCustomConfirmUI(1621, (param) => BackToChooseLevel());
        {
            MatchRoom.GetInstance().OnEnd();
            return false;
        }

        Dictionary<byte, AppointmentRecordPlayer> players = new Dictionary<byte, AppointmentRecordPlayer>();
        AppointmentRecordPlayer player;

        uint roomId = _ms.ReadUInt();
        byte level = _ms.ReadByte();
        EnterRoom(level, true);
        OnStartEndMatch(false, false);
        m_PlayerList[0].m_nTotalCoin = _ms.ReadLong();
        m_dictSitPlayer[localSit] = 0;
        m_PlayerList[0].m_nSrvSit = localSit;
        byte ready = _ms.ReadByte();

        PlayerData pd = GameMain.hall_.GetPlayerData();
        player = new AppointmentRecordPlayer();
        player.playerid = pd.GetPlayerID();
        player.faceid = (int)pd.PlayerIconId;
        player.url = pd.GetPlayerIconURL();
        player.coin = m_PlayerList[0].m_nTotalCoin;
        player.playerName = pd.GetPlayerName();
        player.master = pd.MasterScoreKindArray[(int)GameType];
        player.sex = pd.PlayerSexSign;
        player.ready = ready;
        players[localSit] = player;

        byte otherNum = _ms.ReadByte();
        byte sit, clientSit, pokernum, sex, dis;
        uint userId;
        int face;
        string url, name;
        float master;
        long coin;
        for (byte i = 0; i < otherNum; i++)
        {
            sit = _ms.ReadByte();
            clientSit = GetClientSit(sit, localSit);
            m_dictSitPlayer[sit] = clientSit;
            m_PlayerList[clientSit].m_nSrvSit = sit;
            userId = _ms.ReadUInt();
            face = _ms.ReadInt();
            url = _ms.ReadString();
            coin = _ms.ReadLong();
            name = _ms.ReadString();
            m_PlayerList[clientSit].OnEnd();
            pokernum = _ms.ReadByte();
            dis = _ms.ReadByte();
            master = _ms.ReadSingle();
            sex = _ms.ReadByte();
            ready = _ms.ReadByte();
            m_PlayerList[clientSit].OnDisOrReconnect(dis == 0);
                coin, userId, name, url, face, master, sex);
            m_PlayerList[clientSit].OnPokerNumChanged(pokernum);

            player = new AppointmentRecordPlayer();
            player.playerid = userId;
            player.faceid = face;
            player.url = url;
            player.coin = coin;
            player.playerName = name;
            player.master = master;
            player.sex = sex;
            player.ready = ready;
            players[sit] = player;
        }

        byte curGrade = _ms.ReadByte();
        LL_PokerType.CurGrade = curGrade;

        byte roundState = 2;//对方打
        byte num = _ms.ReadByte();
        for(byte i = 0; i < num; i++)
        {
            if (localSit == dealerSit)
                roundState = 1;//我方打
        }

        byte bUpgrade = _ms.ReadByte();
        {
            byte otherGrade = _ms.ReadByte();
            ShowRoundTip(true, roundState, curGrade, otherGrade);
        }
        {
            ShowRoundTip(true, 3, curGrade);
            curRound = _ms.ReadByte();
            byte maxRound = _ms.ReadByte();
        }

        uint contestId = _ms.ReadUInt();
        uint appointId = _ms.ReadUInt();

        if(appointId > 0)
        {
            GuanDanAppointmentData data = new GuanDanAppointmentData(4);
            data.roomid = appointId;

            if (bUpgrade == 1)
            {
                data.terData_.playType = TePlayType.Goon;
                data.terData_.vectory = _ms.ReadByte();
            }
            else
            {
                data.terData_.playType = TePlayType.Times;
                data.terData_.times = _ms.ReadByte();
                data.terData_.score = _ms.ReadByte();
                data.terData_.cp = (CurrentPoker)_ms.ReadByte();
            }

            AppointmentDataManager.AppointmentDataInstance().currentRoomID = appointId;
            AppointmentDataManager.AppointmentDataInstance().AddAppointmentData(appointId, data);

            GameMain.hall_.InitRoomsData();

            LoadAppointmentRuleResource(curRound, true);
        }

        float timeLeft = _ms.ReadSingle();

        m_PlayerList[0].OnEnd();
        byte pokerNum = _ms.ReadByte();
        if (pokerNum == 0)//本人结束，同步队友牌
        {
            pokerNum = _ms.ReadByte();
        }
            m_PlayerList[0].m_HavePokerList.Add(_ms.ReadByte());

        byte finishRoleNum = _ms.ReadByte();
        for (byte i = 1; i <= finishRoleNum; i++)
        {
            sit = _ms.ReadByte();
            m_PlayerList[m_dictSitPlayer[sit]].ShowRank(i);
        }
        m_PlayerList[0].OnPokerNumChanged(pokerNum);//确定名次后再显示牌（有可能是队友的牌）

        GuanDanRoomState_Enum gameState = (GuanDanRoomState_Enum)_ms.ReadByte();
        OnStateChange(gameState, _ms, 1, timeLeft);

        if (GameMode == GameTye_Enum.GameType_Normal)
        {
            if (playerState == GuandanPlayerState_Enum.GuandanPlayerState_OnDesk)
                MatchRoom.GetInstance().ShowTable(true, players, level, roomId);
            else
                MatchRoom.GetInstance().StartGame();
        }

        CustomAudioDataManager.GetInstance().PlayAudio(1001, false);
    }
        OnEnd();
        PlayUIAnim("Anime_startgame", 1f, 0, null, CommonAssetBundle);

        byte pokerNum = _ms.ReadByte();
        for (int i = 0; i < pokerNum; i++)
        {
            ((GuanDan_RoleLocal)m_PlayerList[0]).m_HavePokerList.Add(_ms.ReadByte());
        }

        byte bUpgrade = _ms.ReadByte();
        {
            byte curGrade = _ms.ReadByte();
            byte dealerSit1 = _ms.ReadByte();
            byte dealerSit2 = _ms.ReadByte();
            byte otherGrade = _ms.ReadByte();
            byte localSit = m_PlayerList[0].m_nSrvSit;
            byte roundState = 1;
            if (localSit != dealerSit1 && localSit != dealerSit2)
                roundState = 2;
            ShowRoundTip(true, roundState, curGrade, otherGrade);

            LL_PokerType.CurGrade = curGrade;
            foreach (GuanDan_Role role in m_PlayerList)
            {
                role.ShowRole();
                GameMain.SC(role.BeginPoker());
            }
        }
        {
            byte curGrade = _ms.ReadByte();
            byte sit1 = _ms.ReadByte();
            byte index1 = _ms.ReadByte();
            byte sit2 = _ms.ReadByte();
            byte index2 = _ms.ReadByte();
            byte show = _ms.ReadByte();
            ShowRoundTip(true, 3, curGrade, show);

            DebugLog.Log(string.Format("begin poker local:{0} {1}-{2} {3}-{4} ", m_PlayerList[0].m_nSrvSit, sit1, index1, sit2, index2));

            LL_PokerType.CurGrade = curGrade;
            foreach (GuanDan_Role role in m_PlayerList)
            {
                List<byte> list = new List<byte>();
                if (role.m_nSrvSit == sit1)
                    list.Add(index1);
                if (role.m_nSrvSit == sit2)
                    list.Add(index2);
                GameMain.SC(role.BeginPoker(list, show));
            }
        }
        return true;

        if (bExitAppointmentDialogState)
        {
            CCustomDialog.CloseCustomDialogUI();
        }
            m_PlayerList[i].Init();
        m_nWaitingLoad = 0;

        if (quit)
        {
        }
        OnStartEndMatch(false, showMatch);
        {
            Bystander = false;
            OnLeaveLookRoom();
        }
    {
        if (over)
            GameOver();
        else
            OnEnd();
    }

    {
        m_bReconnected = true;
    }

    //roundState 1：我方打 2：对方打 3：不升级 4：显示赖子
    void ShowRoundTip(bool show, byte roundState = 1, byte curPoint = 2, byte otherPointOrShowPoker = 2, float showTime = 2f)
    {
        if(show)
        {
            Transform tfm;
            if(roundState == 4)
            {
                GuanDan_Role role = m_PlayerList[m_dictSitPlayer[curPoint]];
                tfm = MainUITfm.Find("Pop-up/BeforeGameInfo");
            else
            {
                string[] pokerStrs = new string[] {"","A", "2", "3", "4", "5", "6",
                if (roundState != 3)
                {
                    bool[] side;
                    byte[] point;
                    if (roundState == 1)
                    {
                        side = new bool[] { true, false };
                        point = new byte[] { curPoint, otherPointOrShowPoker };
                    }
                    else
                    {
                        side = new bool[] { false, true };
                        point = new byte[] { otherPointOrShowPoker, curPoint };
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        tfm = m_RoundTipTfm.GetChild(i);
                        tfm.Find("Imageoutline").gameObject.SetActive(side[i]);
                        tfm.Find("Textnum").GetComponent<Text>().text = pokerStrs[point[i]];
                }
                else
                {
                    m_RoundTipTfm.GetChild(0).gameObject.SetActive(false);
                    m_RoundTipTfm.GetChild(1).gameObject.SetActive(false);
                    tfm = m_RoundTipTfm.GetChild(2);
                    tfm.gameObject.SetActive(true);
                    tfm.Find("Textnum").GetComponent<Text>().text = pokerStrs[curPoint];
                }
                m_RoundTipTfm.gameObject.SetActive(true);
            }
        }
        else
        {
            m_RoundTipTfm.gameObject.SetActive(false);
            MainUITfm.Find("Pop-up/BeforeGameInfo").gameObject.SetActive(false);
        }
    }
            if (beforeSit < 4 && sit != beforeSit)
            {
                GuanDan_Role role = m_PlayerList[m_dictSitPlayer[beforeSit]];
                if (role.Rank > 0)
                {
                    DebugLog.Log("hide finish deal poker! sit:" + m_dictSitPlayer[beforeSit] + " dealSit:" + m_dictSitPlayer[sit]);

                    role.OnDealPoker();
                }
            }
        {
        }

        if(isUpgrade == 1)
            ourGrade = _ms.ReadByte();
            otherGrade = _ms.ReadByte();
            byte dealerSit1 = _ms.ReadByte();
            byte dealerSit2 = _ms.ReadByte();
            levelChange = _ms.ReadByte();
            nextGrade = _ms.ReadByte();
            endGrade = _ms.ReadByte();
        }
        {
            ourGrade = _ms.ReadByte();
            otherGrade = ourGrade;
            byte dealerSit1 = _ms.ReadByte();
            byte dealerSit2 = _ms.ReadByte();
            levelChange = 0;
            nextGrade = 0;
            endGrade = 0;
        }
        for (int i = 0; i < num; i++)
        {
            long addCoin = _ms.ReadLong();
            role.m_nTotalCoin = _ms.ReadLong();
            role.UpdateInfoUI();

            RoundResultData data = new RoundResultData();
            data.headImg = role.GetHeadImg().sprite;
            data.name = role.GetName();
            data.addCoin = addCoin;
            data.coin = role.m_nTotalCoin;
            data.playerid = playerid;
            result.Add(sit, data);
        }
        {
            appointmentTotalResults_.Add(result);
            Debug.Log("add results===============");
        }
        bool bWin = m_PlayerList[0].m_nSrvSit == winSit || m_PlayerList[2].m_nSrvSit == winSit;

        {
            byte sit = _ms.ReadByte();
            byte pokerNum = _ms.ReadByte();
            byte[] list = new byte[pokerNum];
                list[j] = _ms.ReadByte();
            m_PlayerList[m_dictSitPlayer[sit]].ShowEndPoker(list);
        }

        byte curRound = _ms.ReadByte();
        long vedioId = _ms.ReadLong();

        bool allEnd = (isUpgrade == 1) ? (nextGrade == 0 ): (curRound == totalRound);
        if (GameMode == GameTye_Enum.GameType_Contest)
        else if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            LoadAppointmentRuleResource((uint)(curRound + 1), true);
            GameMain.SC(ShowResult(isUpgrade == 1, bWin, ourGrade, otherGrade, levelChange, nextGrade, result, allEnd, curRound));
        }
        else
        {
            Transform tfm;
            string[] pokerStrs = new string[] {"","A", "2", "3", "4", "5", "6",
            for (int i = 0; i < 2; i++)
            {
                tfm = m_RoundTipTfm.GetChild(i);
                if(tfm.Find("Imageoutline").gameObject.activeSelf 
                    && tfm.Find("Textnum").GetComponent<Text>().text == pokerStrs[endGrade])
                {
                    Text text = tfm.Find("Texttime").GetComponent<Text>();
                    int times = 0;
                    int.TryParse(text.text, out times);
                    text.text = (times + 1).ToString();

                    DebugLog.Log(i + ":Add end times---nextGrade:" + nextGrade + " endGrade:" + endGrade);
                }
            }
        }
        return true;
    }

    IEnumerator ShowResult(bool isUpgrade, bool bWin, byte ourGrade, byte otherGrade, byte levelChange, 
        byte nextGrade, Dictionary<byte, RoundResultData> rankSitInfo, bool allEnd, byte curRound = 0)
    {
        yield return new WaitForSecondsRealtime(2f);

        CustomAudioDataManager.GetInstance().StopAudio();

        ResultUITfm.gameObject.SetActive(true);

        Transform parent = ResultUITfm.Find("ImageBG");
        Button butn = parent.Find("ImageButtonBG/Button_jixu").GetComponent<Button>();

        CustomAudioDataManager.GetInstance().PlayAudio(bWin ? 1003 : 1002);
        Transform tfm, tfm1;
        tfm = parent.Find("animation");
        foreach (Transform child in tfm)
            GameObject.Destroy(child.gameObject);
        GameObject obj = (GameObject)GuanDanAssetBundle.LoadAsset(bWin ? "anime_win" : "anime_lost");
        obj = GameMain.Instantiate(obj);
        obj.transform.SetParent(tfm, false);
        parent.Find("Image_difen").gameObject.SetActive(false);

        int rank;
        GuanDan_Role role;
        string[] strs = new string[] { "头游", "二游", "三游", "末游" };
        string[] childName = new string[] { "1", "2" };
        byte[] grade = new byte[] { ourGrade, otherGrade };
        bool bBigWin = false;
        RoundResultData rd = null;
        //Text text;
        for (int i = 0; i < 2; i++)
        {
            tfm = parent.Find("playerBG_" + (i+1));
            bBigWin = isUpgrade && nextGrade == 0 && ((i == 1) ^ bWin);
            tfm.Find("ImageWin").gameObject.SetActive(bBigWin);
            tfm.Find("ImageLv/ImageIcon").gameObject.SetActive(bBigWin);
            //text = tfm.FindChild("ImageLv/Text").GetComponent<Text>();
            //if(bBigWin)
            //{
            //    text.text = "过";
            //}
            //{
            //    text.color = Color.black;
            //    text.text = "打";
            //}

            for(int j = 0; j < 2; j++)
            {
                tfm1 = tfm.Find(childName[j]);
                role = m_PlayerList[i + 2 * j];
                rd = rankSitInfo[role.m_nSrvSit];
                rank = rankSitInfo.Keys.ToList().FindIndex(s => s == role.m_nSrvSit);
                DebugLog.Log(role.GetName() + " Sit:" + role.m_nSrvSit + " rank:" + rank 
                    + " cur:" + grade[i] + " next:" + nextGrade
                    + " change:" + levelChange);
                tfm1.Find("Image_ranking").GetComponent<Image>().sprite = GuanDanAssetBundle.LoadAsset<Sprite>("js_widget_" + (rank + 1));
                tfm1.Find("Image_ranking/TextNum").GetComponent<Text>().text = (rank + 1).ToString();
                tfm1.Find("Image_ranking/TextName").GetComponent<Text>().text = strs[rank];
                tfm1.Find("TextName").GetComponent<Text>().text = role.GetName();
                tfm1.Find("Text_jifen").GetComponent<Text>().text =
                    GameFunction.FormatCoinText(rd.addCoin, true);

        if (nextGrade == 0)
            tfm.gameObject.SetActive(false);
        {
            tfm.Find("Text_shengji/Text_1").GetComponent<Text>().text = bWin ? "己方" : "对方";
            tfm.Find("Text_shengji/Text_2").GetComponent<Text>().text = levelChange.ToString();
            string[] pokerStrs = new string[] {"","A", "2", "3", "4", "5", "6",
            tfm.Find("Text_xiaju/Text_1").GetComponent<Text>().text = pokerStrs[nextGrade]; 
        }

        if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            parent.transform.Find("Image_difen").gameObject.SetActive(false);
            parent.transform.Find("ImageButtonBG/Button_likai").gameObject.SetActive(false);

            if (isUpgrade)
            {
                bool isshowtotal = nextGrade == 0
                    || AppointmentDataManager.AppointmentDataInstance().interrupt;

                XPointEvent.AutoAddListener(butn.gameObject, OnClosePerResultPanel, isshowtotal);

                butn.interactable = !isshowtotal || m_eRoomState == GuanDanRoomState_Enum.GuanDanRoomState_TotalEnd;

                isOpenAppointmentFinalResults_ = false;
                AppointmentDataManager.AppointmentDataInstance().playerAlready = false;
            }
            else
            {
                bool isshowtotal = AppointmentDataManager.AppointmentDataInstance().interrupt && curRound == 1;
                XPointEvent.AutoAddListener(butn.gameObject, OnClosePerResultPanel, isshowtotal);
            }

            butn.gameObject.SetActive(allEnd);
        }
        else
        {
            butn.gameObject.SetActive(true);
            butn.onClick.AddListener(() => OnClickResultButton(1, allEnd));
            butn.interactable = m_eRoomState== GuanDanRoomState_Enum.GuanDanRoomState_OnceEnd || m_eRoomState == GuanDanRoomState_Enum.GuanDanRoomState_TotalEnd ||
                                m_eRoomState < GuanDanRoomState_Enum.GuanDanRoomState_TotalBegin;

            GameObject liKaiGameObject = parent.Find("ImageButtonBG/Button_likai").gameObject;
            liKaiGameObject.SetActive(true);
            liKaiGameObject.GetComponent<Button>().interactable = butn.interactable;
        }

    IEnumerator ShowContestResult(bool bWin, byte curGrade, Dictionary<byte, RoundResultData> rankSitInfo, bool allEnd, long vedioId)
    {
        AppointmentRecord record = null;
        if (allEnd && !Bystander)
        {
            //组装比赛记录数据
            record = new AppointmentRecord();
            record.gameID = (byte)GameKind_Enum.GameKind_GuanDan;
            appointmentGameRule_ = "掼蛋比赛第" + MatchInGame.GetInstance().m_nCurTurn.ToString() + "轮";
            record.gamerule = appointmentGameRule_;
            record.timeseconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);
            record.isopen = false;
            record.videoes = vedioId;
        }
        int index = 0;
        RoundResultData rd = null;
        foreach (GuanDan_Role role in m_PlayerList)
        {
            rd = rankSitInfo[role.m_nSrvSit];
            list.Add(rd);
            {
                AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();

                playerdata.playerid = rd.playerid;
                playerdata.faceid = role.m_nFaceId;
                playerdata.playerName = role.GetName();
                playerdata.url = role.m_szUrl;
                playerdata.coin = role.m_nTotalCoin;

                if (!record.result.ContainsKey(playerdata.playerid))
                    record.result.Add(playerdata.playerid, playerdata);
            }
        if (record != null)
        {
            if (GameMain.hall_.gamerooms_ == null)
                GameMain.hall_.InitRoomsData();

            GameMain.hall_.gamerooms_.recordlist_.Insert(0, record);
        }

        yield return new WaitForSecondsRealtime(2f);

        CustomAudioDataManager.GetInstance().StopAudio();

        CustomAudioDataManager.GetInstance().PlayAudio(bWin ? 1003 : 1002);

        yield return MatchInGame.GetInstance().ShowRoundResult(4, list, () =>
            if (allEnd)
                GameOver(false, true);
            else
                OnEnd();
        }, allEnd, Bystander);
    void OnClickResultButton(int index, bool allEnd = false)
    {
        if (GameMode != GameTye_Enum.GameType_Normal)
            return;

        DebugLog.Log("click continue:" + index + "--end:" + allEnd);

        {
            MatchRoom.GetInstance().OnClickReturn(index);
        }
        {
            if(allEnd)
                MatchRoom.GetInstance().OnClickReturn(index);
            else
        {
        }
    }

    public GuandanPokerType_Enum GetPokersType(List<byte> list, List<byte> curPokerList, GuandanPokerType_Enum curType, ref List<byte> childValidList)
    {
        if (list.Count == 0 || curType == GuandanPokerType_Enum.GuanPokerType_KingBlast)
            return GuandanPokerType_Enum.GuanPokerType_Error;

        if (list.Count == 1)
        {
            {
            }
            if (curPokerList.Count == 1 && LL_PokerType.GetPokerLogicValue(list[0]) > LL_PokerType.GetPokerLogicValue(curPokerList[0]))
            {
            }
            return GuandanPokerType_Enum.GuanPokerType_Error;
        }

        byte laizi = LL_PokerType.GetLaiziValue();
        Dictionary<int, List<byte>> valueList;

        int curBiggest = 0;
        int biggest = 0;
        List<byte> curChildValidList = new List<byte>();

        if (LL_PokerType.IsKingBlust(valueList, ref childValidList, ref biggest, 2) != 0)
            return GuandanPokerType_Enum.GuanPokerType_KingBlast;

        Dictionary<int, List<byte>> curValueList;

        //同花顺
        if (curType == GuandanPokerType_Enum.GuanPokerType_SameColorFlush)
        {
                return GuandanPokerType_Enum.GuanPokerType_Error;

            if (list.Count == 5)
            {
                if (LL_PokerType.IsStraightFlush(valueList, ref childValidList, ref biggest, true, 5) != 0)
                {
                    LL_PokerType.IsStraightFlush(curValueList, ref curChildValidList, ref curBiggest, true, 5);

            if (list.Count > 5)
            {
                if (LL_PokerType.IsNormalBomb(valueList, ref childValidList, ref biggest) != 0)
                    return GuandanPokerType_Enum.GuanPokerType_Blast;
            return GuandanPokerType_Enum.GuanPokerType_Error;
        }

        //普通炸弹
        if (curType == GuandanPokerType_Enum.GuanPokerType_Blast)
        {
            if (list.Count < 4)
                return GuandanPokerType_Enum.GuanPokerType_Error;

            if (list.Count == 5)
            {
                if (LL_PokerType.IsStraightFlush(valueList, ref childValidList, ref biggest, true, 5) != 0)
                    if (curPokerList.Count <= 5)
                        return GuandanPokerType_Enum.GuanPokerType_SameColorFlush;
            }

            if (LL_PokerType.IsNormalBomb(valueList, ref childValidList, ref biggest) != 0)
            {
                if(LL_PokerType.IsNormalBomb(curValueList, ref curChildValidList, ref curBiggest) != 0)
            return GuandanPokerType_Enum.GuanPokerType_Error;
        }

        //这时都可以炸了
        if (LL_PokerType.IsStraightFlush(valueList, ref childValidList, ref biggest, true, 5) != 0)
            return GuandanPokerType_Enum.GuanPokerType_SameColorFlush;

        if (LL_PokerType.IsNormalBomb(valueList, ref childValidList, ref biggest) != 0)
            return GuandanPokerType_Enum.GuanPokerType_Blast;

        //此后比大小
        if (curType != GuandanPokerType_Enum.GuanPokerType_Error && list.Count != curPokerList.Count)
            return GuandanPokerType_Enum.GuanPokerType_Error;

        if (list.Count == 2)
        {
                if (LL_PokerType.IsPairs(valueList, ref childValidList, ref biggest) != 0)
                {
                    LL_PokerType.IsPairs(curValueList, ref curChildValidList, ref curBiggest);
        {
            if (curType == GuandanPokerType_Enum.GuanPokerType_Error || curType == GuandanPokerType_Enum.GuanPokerType_Three)
                if (LL_PokerType.IsThree(valueList, ref childValidList, ref biggest) != 0)
                {
                    LL_PokerType.IsThree(curValueList, ref curChildValidList, ref curBiggest);
        {
            //4个只能是炸，之前判断过，这里直接返回
            return GuandanPokerType_Enum.GuanPokerType_Error;
        }
        else if (list.Count == 5)
        {
            if (curType == GuandanPokerType_Enum.GuanPokerType_Error || curType == GuandanPokerType_Enum.GuanPokerType_Flush)
            {
                {
                    LL_PokerType.IsFlush(curValueList, ref curChildValidList, ref curBiggest, true, 5);
                }
            if (curType == GuandanPokerType_Enum.GuanPokerType_Error || curType == GuandanPokerType_Enum.GuanPokerType_ThreeAndTwo)
            {
                if (LL_PokerType.IsThreeAndTwo(valueList, ref childValidList, ref biggest) != 0)
                {
                    curBiggest = 0;
                    LL_PokerType.IsThreeAndTwo(curValueList, ref curChildValidList, ref curBiggest);
                }
        else
        {
            if (list.Count == 6)
            {
                if (curType == GuandanPokerType_Enum.GuanPokerType_Error || curType == GuandanPokerType_Enum.GuanPokerType_SeriesThree)
                {
                    if (LL_PokerType.IsPlaneNoWith(valueList, ref childValidList, ref biggest, true, 6) != 0)
                    {
                        LL_PokerType.IsPlaneNoWith(curValueList, ref curChildValidList, ref curBiggest, true, 6);
                        if (biggest > curBiggest)
                            return GuandanPokerType_Enum.GuanPokerType_SeriesThree;
                    }
                }

                if (curType == GuandanPokerType_Enum.GuanPokerType_Error || curType == GuandanPokerType_Enum.GuanPokerType_SeriesPairs)
                {
                    if (LL_PokerType.IsSeriesPairs(valueList, ref childValidList, ref biggest, true, 6) != 0)
                    {
                        curBiggest = 0;
                        LL_PokerType.IsSeriesPairs(curValueList, ref curChildValidList, ref curBiggest, true, 6);
                        if (biggest > curBiggest)
                            return GuandanPokerType_Enum.GuanPokerType_SeriesPairs;
                    }
                }
            }

            if (curType == GuandanPokerType_Enum.GuanPokerType_Error || curType == GuandanPokerType_Enum.GuanPokerType_Flush)
            {
                {
                    LL_PokerType.IsFlush(curValueList, ref curChildValidList, ref curBiggest, true, 5);
                }
    }

    void OnPokerSortTypeChanged(PokerSortType type)
    {
        if(type != CurPOT)
        {
            CurPOT = type;
            ((GuanDan_RoleLocal)m_PlayerList[0]).OnPSTChange();

    public void PlayUIAnim(string animation, float lifeTime, int audioID = 0, Transform parentTfm = null, AssetBundle bundle = null)

    void ClearUIAnimation()
    {
        foreach (Transform tfm in AnimationTfm)

    //state: 0:显示按钮 1：显示贡牌 2:收贡 submitOrReturn:上贡还是还贡
    void ShowGong(bool show, byte state = 0, bool submitOrReturn = true, byte[] sits = null, byte poker = 0, bool bReverse = false, float time = -1f)
    {
        GongUITfm.gameObject.SetActive(show);

        Transform tfm;
        string[] prefix = new string[] {"playerBG_1/", "playerBG_2/" };

        if (show)
        {
            if(state == 0)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        tfm = GongUITfm.Find(prefix[j] + (i + 1));
                        tfm.gameObject.SetActive(sits.Length > i);
                        if (tfm.gameObject.activeSelf && (submitOrReturn ^ (j == 1)))
                        {
                            GuanDan_Role role = m_PlayerList[m_dictSitPlayer[sits[i]]];
                            role.OnGong(state, submitOrReturn);
                            tfm.Find("Name_Text").GetComponent<Text>().text = bReverse ? "" : role.GetName();
                            Image img = tfm.Find("Head/HeadMask/ImageHead").GetComponent<Image>();
                            img.enabled = !bReverse;
                            img.sprite = role.GetHeadImg().sprite;
                        }
                    }
                }
                                : GuanDan_Data.GetInstance().m_RoomData.m_fReturnPokerTime;
                ShowCountdown(true, null, time);
            }
            {
                for(int i  = 0; i < sits.Length; i++)
                {
                    GuanDan_Role role = m_PlayerList[m_dictSitPlayer[sits[i]]];
                    role.OnGong(state, submitOrReturn, poker, null, bReverse);

                    tfm = GongUITfm.Find(prefix[submitOrReturn ? 0 : 1] + "1");
                    string name = tfm.Find("Name_Text").GetComponent<Text>().text;
                    if (name != role.GetName())
                        tfm = tfm.parent.Find("2");
                    PlayCard.SetCardSprite(tfm.Find("Imagecard").gameObject, 
        {
            for(int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    tfm = GongUITfm.Find(prefix[j] + (i + 1));
                    tfm.Find("Name_Text").GetComponent<Text>().text = "";
                    tfm.Find("Head/HeadMask/ImageHead").GetComponent<Image>().enabled = false;
                    PlayCard.SetCardSprite(tfm.Find("Imagecard").gameObject, CommonAssetBundle, RoomInfo.NoSit, false);
                    tfm.gameObject.SetActive(false);
                }


    public override void SetupVideo(List<AppointmentRecordPlayer> list)
    {
        if (list == null)
            return;

        uint localId = GameMain.hall_.GetPlayerId();
        byte localSit = (byte)list.FindIndex(s => s.playerid == localId);
        byte srvSit;
        for (byte i = 0; i < 4; i++)
        {
            m_dictSitPlayer[srvSit] = i;
        }

        AppointmentRecordPlayer info;
        for (byte i = 0; i < list.Count; i++)
                0, info.playerid, info.playerName, info.url, info.faceid, info.master, info.sex);
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
                //case VideoActionInfo_Enum.VideoActionInfo_101:
                //    OnStateChange((GuanDanRoomState_Enum)list[0], null);
                //    break;

                case VideoActionInfo_Enum.VideoActionInfo_102:
                    {
                        OnEnd();

                        int upgrade = list[j++];
                        if (upgrade == 1)
                        {
                            byte curGrade = (byte)list[j++];
                            byte dealerSit1 = (byte)list[j++];
                            byte dealerSit2 = (byte)list[j++];
                            byte otherGrade = (byte)list[j++];
                            byte localSit = m_PlayerList[0].m_nSrvSit;
                            byte roundState = 1;
                            if (localSit != dealerSit1 && localSit != dealerSit2)
                                roundState = 2;
                            ShowRoundTip(true, roundState, curGrade, otherGrade);

                            LL_PokerType.CurGrade = curGrade;
                            foreach (GuanDan_Role role in m_PlayerList)
                                role.ShowRole();
                        }
                        else
                        {
                            byte curGrade = (byte)list[j++];
                            byte sit1 = (byte)list[j++];
                            byte index1 = (byte)list[j++];
                            byte sit2 = (byte)list[j++];
                            byte index2 = (byte)list[j++];
                            byte show = (byte)list[j++];
                            ShowRoundTip(true, 3, curGrade, show);

                            LL_PokerType.CurGrade = curGrade;
                            int showIndex;
                            foreach (GuanDan_Role role in m_PlayerList)
                            {
                                showIndex = 0;
                                if (role.m_nSrvSit == sit1)
                                {
                                    role.ShowTipPoker(show, showIndex);
                                    showIndex++;
                                }
                                if (role.m_nSrvSit == sit2)
                                {
                                    role.ShowTipPoker(show, showIndex);
                                    showIndex++;
                                }
                            }
                        }

                        int pokerNum = list[j++];
                        int playerNum = list[j++];
                        for(int i = 0; i < playerNum; i++)
                        {
                            byte sit = (byte)list[j++];
                            sit = m_dictSitPlayer[sit];
                            uint userid = (uint)list[j++];
                            for(int k = 0; k < pokerNum; k++)
                            {
                                byte poker = (byte)list[j++];
                                m_PlayerList[sit].m_HavePokerList.Add(poker);
                            }
                            m_PlayerList[sit].UpdateRecordCards(true);
                        }
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_103:
                case VideoActionInfo_Enum.VideoActionInfo_105:
                    {
                        int doubleDown = list[j++];
                        byte[] sits = new byte[doubleDown == 1 ? 2 : 1];
                        sits[0] = (byte)list[j++];

                        if (doubleDown == 1)
                            sits[1] = (byte)list[j++]; 

                        ShowGong(true, 0, action.vai == VideoActionInfo_Enum.VideoActionInfo_103, sits);
                        PlayUIAnim("anime_gong", -1f);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_104:
                case VideoActionInfo_Enum.VideoActionInfo_106:
                    {
                        int doubleDown = list[j++];
                        byte poker = (byte)list[j++];
                        byte sit = (byte)list[j++];
                        byte revSit = (byte)list[j++];
                        ShowGong(true, 1, action.vai == VideoActionInfo_Enum.VideoActionInfo_104, new byte[] { sit }, poker);
                        //m_PlayerList[m_dictSitPlayer[sit]].OnDealPoker();
                        //m_PlayerList[m_dictSitPlayer[revSit]].OnGong(2, true, poker, m_PlayerList[m_dictSitPlayer[sit]].m_PlayCards.transform);

                        if (doubleDown == 1)
                        {
                            revSit = (byte)list[j++];
                            ShowGong(true, 1, action.vai == VideoActionInfo_Enum.VideoActionInfo_104, new byte[] { sit }, poker);
                            //m_PlayerList[m_dictSitPlayer[sit]].OnDealPoker();
                            //m_PlayerList[m_dictSitPlayer[revSit]].OnGong(2, true, poker, m_PlayerList[m_dictSitPlayer[sit]].m_PlayCards.transform);
                        }

                        ClearUIAnimation();
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_107:
                    {
                        byte changeSit1 = (byte)list[j++];
                        byte changeSit2 = (byte)list[j++];

                        ChangeSit(changeSit1, changeSit2, GameVideo.GetInstance().GetStepTime());
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_108:
                    {
                        PlayUIAnim("anime_kanggong", 2f);

                        byte num = (byte)list[j++];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte jokerNum = (byte)list[j++];

                            m_PlayerList[m_dictSitPlayer[sit]].ShowTipPoker(0x42, jokerNum - 1);
                        }
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_109:
                    {
                        GongUITfm.gameObject.SetActive(false);

                        foreach (GuanDan_Role role in m_PlayerList)
                            role.OnGameStart();

                        byte firstSit = (byte)list[0];
                        ShowRoundTip(true, 4, firstSit, 2, GameVideo.GetInstance().GetStepTime());
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_110:
                    {
                        byte sit = (byte)list[j++];
                        byte beforeSit = (byte)list[j++];
                        uint userId = (uint)list[j++];
                        float bankTime = list[j++];
                        float askTime = list[j++];
                        if (beforeSit < 4)
                        {
                            GuanDan_Role beforeRole = m_PlayerList[m_dictSitPlayer[beforeSit]];
                            m_PlayerList[m_dictSitPlayer[sit]].OnAskDealPoker(sit == beforeSit, beforeRole.m_PlayPokerList, 
                                beforeRole.CurPokerType, askTime, bankTime, GameVideo.GetInstance().m_bPause);
                        }
                        else
                            m_PlayerList[m_dictSitPlayer[sit]].OnAskDealPoker(true, null, 
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_113:
                    {
                        byte sit = (byte)list[j++];
                        uint userid = (uint)list[j++];
                        byte beforeSit = (byte)list[j++];
                        GuandanPokerType_Enum pokerType = (GuandanPokerType_Enum)list[j++];
                        byte haveRight = (byte)list[j++];
                        byte num = (byte)list[j++];
                        byte[] cards = new byte[num];
                        for (byte i = 0; i < num; i++)
                        {
                            cards[i] = (byte)list[j++];
                        }

                        byte rank = (byte)list[j++];

                        m_PlayerList[m_dictSitPlayer[sit]].OnDealPoker((sbyte)haveRight, cards, pokerType, rank);

                        if (cards != null && cards.Length > 0)
                        {
                            if (beforeSit < 4 && sit != beforeSit)
                            {
                                GuanDan_Role role = m_PlayerList[m_dictSitPlayer[beforeSit]];
                                if (role.Rank > 0)
                                {
                                    DebugLog.Log("hide finish deal poker! sit:" + m_dictSitPlayer[beforeSit] + " dealSit:" + m_dictSitPlayer[sit]);

                                    role.OnDealPoker();
                                }
                            }
                        }
                    }
                    break;

                default:
                    res = false;
                    break;
            }
        {
            switch (action.vai)
            {
                case VideoActionInfo_Enum.VideoActionInfo_102:
                    {
                        OnEnd();
                    }

                case VideoActionInfo_Enum.VideoActionInfo_103:
                    {
                        ShowGong(false);
                        ClearUIAnimation();
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_104:
                case VideoActionInfo_Enum.VideoActionInfo_106:
                    {
                        int doubleDown = list[j++];
                        byte[] sits = new byte[doubleDown == 1 ? 2 : 1];
                        byte[] poker = new byte[sits.Length];
                        byte[] recSits = new byte[sits.Length];
                        poker[0] = (byte)list[j++];
                        sits[0] = (byte)list[j++];
                        recSits[0] = (byte)list[j++];

                        if (doubleDown == 1)
                        {
                            sits[1] = (byte)list[j++];
                            recSits[1] = (byte)list[j++];
                        }

                        ShowGong(true, 1, action.vai == VideoActionInfo_Enum.VideoActionInfo_104, sits, 0, true);
                        PlayUIAnim("anime_gong", -1f);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_105:
                    {
                        int doubleDown = list[j++];
                        byte[] sits = new byte[doubleDown == 1 ? 2 : 1];
                        sits[0] = (byte)list[j++];

                        if (doubleDown == 1)
                            sits[1] = (byte)list[j++];

                        ShowGong(true, 0, false, sits, 0, true);
                        PlayUIAnim("anime_gong", -1f);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_107:
                    {
                        byte changeSit1 = (byte)list[j++];
                        byte changeSit2 = (byte)list[j++];

                        ChangeSit(changeSit1, changeSit2, GameVideo.GetInstance().GetStepTime());
                        foreach (GuanDan_Role role in m_PlayerList)
                            role.ShowRole(false);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_108:
                    {
                        ClearUIAnimation();
                        byte num = (byte)list[j++];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte jokerNum = (byte)list[j++];

                            m_PlayerList[m_dictSitPlayer[sit]].OnDealPoker();
                        }
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_109:
                    {
                        MainUITfm.Find("Pop-up/BeforeGameInfo").gameObject.SetActive(false);
                        VideoAction lastAction = actionList[curStep - 1];
                        if (lastAction.vai == VideoActionInfo_Enum.VideoActionInfo_106 || 
                            lastAction.vai == VideoActionInfo_Enum.VideoActionInfo_108)
                            OnVideoStep(actionList, curStep - 1, false);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_110:
                    {
                        byte sit = (byte)list[j++];
                        m_PlayerList[m_dictSitPlayer[sit]].OnDealPoker(0);
                        OnVideoStep(actionList, curStep - 1, false);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_113:
                    {
                        byte sit = (byte)list[j++];
                        uint userid = (uint)list[j++];
                        byte beforeSit = (byte)list[j++];
                        GuandanPokerType_Enum pokerType = (GuandanPokerType_Enum)list[j++];
                        byte haveRight = (byte)list[j++];
                        byte num = (byte)list[j++];
                        GuanDan_Role role = m_PlayerList[m_dictSitPlayer[sit]];
                        byte[] cards = new byte[num];
                        for (byte i = 0; i < num; i++)
                        {
                            cards[i] = (byte)list[j++];
                        }
                        role.AddCards(cards);
                        role.ShowRank(0);

                        foreach (Transform tfm in AnimationTfm)
                            GameObject.Destroy(tfm.gameObject);

                        OnVideoStep(actionList, curStep - 1, false);

                        while (haveRight == 0 && curStep > 0)
                        {
                            curStep -= 1;
                            VideoAction lastAction = actionList[curStep];
                            if (lastAction.vai == VideoActionInfo_Enum.VideoActionInfo_113)
                            {
                                byte lastSit = (byte)lastAction.list[0];
                                if (lastSit == sit)
                                    break;

                                j = 1;
                                userid = (uint)lastAction.list[j++];
                                beforeSit = (byte)lastAction.list[j++];
                                pokerType = (GuandanPokerType_Enum)lastAction.list[j++];
                                haveRight = (byte)lastAction.list[j++];
                                num = (byte)lastAction.list[j++];
                                role = m_PlayerList[m_dictSitPlayer[lastSit]];
                                cards = new byte[num];
                                for (byte i = 0; i < num; i++)
                                {
                                    cards[i] = (byte)lastAction.list[j++];
                                }
                                role.OnDealPoker(-1, cards, pokerType);
                            }
                        }

                    }
                    break;

                default:
                    res = false;
                    break;
            }

    public override void OnVideoReplay()
    {
        base.OnVideoReplay();

        OnEnd();
    }

    public override void OnPlayerDisOrReconnect(bool disconnect, uint userId, byte sit)
    {
        if (m_dictSitPlayer.ContainsKey(sit))
            m_PlayerList[m_dictSitPlayer[sit]].OnDisOrReconnect(disconnect);
        else if (GameMode == GameTye_Enum.GameType_Normal)
            MatchRoom.GetInstance().SetPlayerOffline(sit, disconnect);
    }

    void OnStartEndMatch(bool bStart, bool showBtn = true)
        {
            m_MatchBtns[i].SetActive(bShow[i]);
        }

    void OnClickMatchButton(int index, bool playSound)
    {
        if (playSound)

        if (index == 0)//开始匹配
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_CM_ENTERMATCH);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(m_nCurrenLevel);
            HallMain.SendMsgToRoomSer(msg);
        }
        else//取消匹配
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_CM_CANCLEMATCH);
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);
        }

    bool HandleBystanderEnter(uint _msgType, UMessage _ms)
    {
        OnPokerSortTypeChanged(PokerSortType.ePST_Horizontal);

        uint roomId = _ms.ReadUInt();
        byte level = _ms.ReadByte();
        uint contestId = _ms.ReadUInt();

        EnterRoom(level, true, true);
        OnStartEndMatch(false, false);
        byte otherNum = _ms.ReadByte();
        for (byte i = 0; i < otherNum; i++)
        {
            byte sit = _ms.ReadByte();
            byte clientSit = GetClientSit(sit, localSit);
            m_dictSitPlayer[sit] = clientSit;
            uint userId = _ms.ReadUInt();
            int face = _ms.ReadInt();
            string url = _ms.ReadString();
            long coin = _ms.ReadLong();
            string name = _ms.ReadString();
            m_PlayerList[clientSit].m_nSrvSit = sit;
            m_PlayerList[clientSit].OnEnd();
            byte pokerNum = _ms.ReadByte();
            float master = _ms.ReadSingle();
            byte sex = _ms.ReadByte();
            m_PlayerList[clientSit].SetupInfoUI(GameMode != GameTye_Enum.GameType_Normal, 
                coin, userId, name, url, face, master, sex);
            m_PlayerList[clientSit].OnPokerNumChanged(pokerNum);
        }

        byte curGrade = _ms.ReadByte();
        LL_PokerType.CurGrade = curGrade;

        byte roundState = 2;//对方打
        byte num = _ms.ReadByte();
        for (byte i = 0; i < num; i++)
        {
            if (localSit == dealerSit)
                roundState = 1;//我方打
        }

        byte bUpgrade = _ms.ReadByte();
        {
            byte otherGrade = _ms.ReadByte();
            ShowRoundTip(true, roundState, curGrade, otherGrade);
        }
        {
            ShowRoundTip(true, 3, curGrade);
        }

        byte curRound = _ms.ReadByte();
        byte maxRound = _ms.ReadByte();
        if (GameMode == GameTye_Enum.GameType_Contest)
            MatchInGame.GetInstance().ShowBegin(true, curRound, maxRound);

        byte finishRoleNum = _ms.ReadByte();
        for (byte i = 1; i <= finishRoleNum; i++)
        {
            byte sit = _ms.ReadByte();
            m_PlayerList[m_dictSitPlayer[sit]].ShowRank(i);
        }

        float timeLeft = _ms.ReadSingle();
        GuanDanRoomState_Enum gameState = (GuanDanRoomState_Enum)_ms.ReadByte();
        OnStateChange(gameState, _ms, 2, timeLeft);
        return true;
    }

    void OnLeaveLookRoom()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_CM_LEAVEONLOOKERROOM);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((uint)0);
        HallMain.SendMsgToRoomSer(msg);

        Transform tfm = MainUITfm.Find("Pop-up/Set/ImageBG");
        Toggle t = tfm.Find("paixu/Toggle_1").GetComponent<Toggle>();
        PokerSortType type = t.isOn ? PokerSortType.ePST_Horizontal : PokerSortType.ePST_Vertical;
        OnPokerSortTypeChanged(type);
    }

    public override void StartLoad()
    {
        m_nWaitingLoad = 0;
    }

    public void ShowCountdown(bool show, Image img, float time = 0f, CustomCountdownImgMgr.CallBackFunc fun = null, bool pause = false)
    {
        GameObject parent = MainUITfm.Find("Middle/OutlineCountdown").gameObject;
                parent.SetActive(true);
                Text text = parent.transform.Find("TextCountdown").GetComponent<Text>();
                if (!pause)
                    CCIMgr.AddTimeImage(img, time, 1f, fun, text);
                {
                }
            {
                parent.SetActive(false);
            }
        {
    }
}