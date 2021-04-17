using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USocket.Messages;
using System.Linq;
using System.Globalization;

//public enum ContestState
//{
//    NotSign = 0,
//    Special,
//    HasSign,
//}

//public enum Time_Enum
//{
//    StartTime = 0,
//    HasStop,
//    Full2Start,
//}

////概况
//public class BriefInfo
//{
//    public ContestRuleType rule;
//    public string time;
//    public int game;
//    public int condition;
//    public bool reset;
//    public int bestsort;
//}

////赛况
//public class ContestInfo
//{
//    public int sortNo;
//    public string name;
//    public int score;
//}

////奖励
//public class RewardBySort
//{
//    public string sortNo;
//    public int reward;
//}


////本场赛况
//public class ContestDetial
//{
//    public uint id;
//    public BriefInfo bi;
//    public List<ContestInfo> ci;
//    public List<RewardBySort> reward;
//    public string detailrule;
//    public int maxtable;
//    public int lefttable;
//    public string lefttime;
//}

//public class ContestData
//{
//    public uint id;
//    public Sprite icon;
//    public string name;
//    public int number;
//    public Time_Enum timetype;
//    public string date;
//    public string time;
//    public int reward;
//    public int sign;
//    public int ticket;
//    public int signlen;
//    public ContestState state;
//}

public class Contest
{
    public GameObject root_;
    GameObject CanvasObj;
    public GameObject matchinfo_;
    int currentContestIndex_;
    Dictionary<uint, GameObject> contestobjects_;
    stContestData detail_;
    GameObject contestTips_;
    int currentPage_;
    GameObject ticketpanel_;
    List<Sprite> sortImages_;
    Dictionary<string, Sprite> contestImages_;
    GameObject waitingTips_;
    byte contesttype_;
    Button ButtonGF, ButtonZJ;
    Text ContestIconNameText = null;
    GameObject CreateGameBG = null;
    bool bContestInfoListRefreshState = false;//比赛信息列表
    public Contest()
    {
        LoadContestResource();
        InitContestMsg();
        ContestDataManager.Instance();
        InitSortImages();
        contestobjects_ = new Dictionary<uint, GameObject>();
        tipsobjects_ = new Dictionary<uint, GameObject>();
        
        isRefreshContestList = false;
        currentPage_ = 0;
        isDownload_ = false;
    }

    Sprite GetContestImages(string resourceName)
    {
        Sprite result = null;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return result;

        if (contestImages_ == null)
            contestImages_ = new Dictionary<string, Sprite>();

        if (contestImages_.ContainsKey(resourceName))
            return contestImages_[resourceName];

        result = bundle.LoadAsset<Sprite>(resourceName);
        contestImages_.Add(resourceName, result);
        return result;
    }

    void InitSortImages()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle != null)
        {
            if(sortImages_ == null)
                sortImages_ = new List<Sprite>();

            for (int index = 1; index < 4; index++)
            {
                Sprite tempSprite = bundle.LoadAsset<Sprite>("bs_icon_mc" + index.ToString());
                sortImages_.Add(tempSprite);
            }
        }
    }

    void InitContestMsg()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_ContestInfoList, BackContestInfoList);           //所有的比赛信息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_PlayerEnrollReply, BackSignResult);              //报名结果
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_NotifiyPlayerAdmission, BackAdmission);           //通知入场
        CMsgDispatcher.GetInstance().RegMsgDictionary(
           (uint)GameCity.EMSG_ENUM.ContestMsg_PlayerAdmissionReply, BackAdmissionReply);           //入场回复
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_PlayerCancelEnrollReply, BackCancelEnroll);       //取消比赛结果
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_NotifyPlayerStartContest, BackConnectGameServer);       //链接游戏服务器
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Contestmsg_PlayerDisbandCreateContestReply, BackBreakContestMsg);       //解散比赛回复
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_EnrollDisband_NotifyPlayer, BackDisband);                           //条件不足解散比赛
    }

    bool BackDisband(uint msgType, UMessage msg)
    {
        uint contestId = msg.ReadUInt();

        if (GameMain.hall_.GetPlayerData().signedContests.Contains(contestId))
            GameMain.hall_.GetPlayerData().signedContests.Remove(contestId);

        if (GameMain.hall_.GetCurrentGameState() != GameState_Enum.GameState_Hall)
            return true;

        if (contesttype_ == 1)
        {
            if (ContestDataManager.Instance().contestdatas_.ContainsKey(contestId))
                if (ContestDataManager.Instance().contestdatas_[contestId].contestState >= CONTEST_STATE.CONTEST_STATE_ENROLL)
                {
                    System.DateTime sdt = GameCommon.ConvertLongToDateTime(ContestDataManager.Instance().contestdatas_[contestId].tStartTimeSeconds);


                    object[] param = { "您报名的" + sdt.ToString("yyyy年MM月dd日HH:mm") + "开始的" +
                        CCsvDataManager.Instance.GameDataMgr.GetGameData(ContestDataManager.Instance().contestdatas_[contestId].byGameID).GameName
                        + "比赛 由于" };
                    CCustomDialog.OpenCustomConfirmUIWithFormatParam(1604, param);
                }
        }
        else
        {
            if (ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestId))
                if (ContestDataManager.Instance().selfcontestdatas_[contestId].contestState >= CONTEST_STATE.CONTEST_STATE_ENROLL)
                {
                    System.DateTime sdt = GameCommon.ConvertLongToDateTime(ContestDataManager.Instance().selfcontestdatas_[contestId].tStartTimeSeconds);


                    object[] param = { "您报名的" + sdt.ToString("yyyy年MM月dd日HH:mm") + "开始的" +
                        CCsvDataManager.Instance.GameDataMgr.GetGameData(ContestDataManager.Instance().selfcontestdatas_[contestId].byGameID).GameName
                        + "自建赛 由于" };
                    CCustomDialog.OpenCustomConfirmUIWithFormatParam(1604, param);
                }
        }

        return true;
    }

    bool BackConnectGameServer(uint msgType, UMessage msg)
    {
        uint m_nContestId = msg.ReadUInt();
        byte nSerIndex = msg.ReadByte();
        HallMain.SetRoomSerIndex(nSerIndex);
        byte roundPerTurn = msg.ReadByte();
        ushort roleNum = msg.ReaduShort();
        uint m_nDeskIndex = msg.ReadUInt();

        if(contesttype_ == 1)
        {
            if (ContestDataManager.Instance().contestdatas_.ContainsKey(m_nContestId))
                GameMain.hall_.Connect2ContestCommonServer(ContestDataManager.Instance().contestdatas_[m_nContestId].byGameID,
                    0, m_nContestId, roundPerTurn, roleNum);
        }
        else
        {
            if (ContestDataManager.Instance().selfcontestdatas_.ContainsKey(m_nContestId))
                GameMain.hall_.Connect2ContestCommonServer(ContestDataManager.Instance().selfcontestdatas_[m_nContestId].byGameID,
                    0, m_nContestId, roundPerTurn, roleNum);
        }

        return true;
    }

    bool BackCancelEnroll(uint msgType, UMessage msg)
    {
        uint playerid = msg.ReadUInt();
        uint contestid = msg.ReadUInt();
        byte state = msg.ReadByte();

        if(contesttype_ == 1)
        {
            if (!ContestDataManager.Instance().contestdatas_.ContainsKey(contestid))
            {
                CCustomDialog.OpenCustomConfirmUI(1610);
                return false;
            }

            if (state == 1)
            {
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(1602, ContestDataManager.Instance().contestdatas_[contestid].sContestName);
                if (GameMain.hall_.GetPlayerData().signedContests.Contains(contestid))
                {
                    GameMain.hall_.GetPlayerData().signedContests.Remove(contestid);

                    if (contestobjects_.ContainsKey(contestid) && ContestDataManager.Instance().contestdatas_.ContainsKey(contestid))
                    {
                        ContestDataManager.Instance().contestdatas_[contestid].contestState = CONTEST_STATE.CONTEST_STATE_ENROLL;
                        SetSignButtonUI(contestobjects_[contestid], ContestDataManager.Instance().contestdatas_[contestid]);
                        if (matchinfo_ != null)
                            SetInfoSignButtonUI(ContestDataManager.Instance().contestdatas_[contestid]);
                        RefreshSignedPlayerNumber(contestid, false);
                    }
                }
            }
            else
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(1601, ContestDataManager.Instance().contestdatas_[contestid].sContestName);
        }
        else
        {
            if (!ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestid))
            {
                CCustomDialog.OpenCustomConfirmUI(1610);
                return false;
            }

            if (state == 1)
            {
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(1602, ContestDataManager.Instance().selfcontestdatas_[contestid].sContestName);
                if (GameMain.hall_.GetPlayerData().signedContests.Contains(contestid))
                {
                    GameMain.hall_.GetPlayerData().signedContests.Remove(contestid);

                    if (contestobjects_.ContainsKey(contestid) && ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestid))
                    {
                        ContestDataManager.Instance().selfcontestdatas_[contestid].contestState = CONTEST_STATE.CONTEST_STATE_ENROLL;
                        SetSignButtonUI(contestobjects_[contestid], ContestDataManager.Instance().selfcontestdatas_[contestid]);
                        if (matchinfo_ != null)
                            SetInfoSignButtonUI(ContestDataManager.Instance().selfcontestdatas_[contestid]);
                        RefreshSignedPlayerNumber(contestid, false);
                    }
                }
            }
            else
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(1601, ContestDataManager.Instance().selfcontestdatas_[contestid].sContestName);
        }

        return true;
    }

    bool BackAdmission(uint msgType, UMessage msg)
    {
        if (root_ == null)
            return false;

        uint contestid = msg.ReadUInt();

        if(ContestDataManager.Instance().contestdatas_.ContainsKey(contestid))
        {
            if (ContestDataManager.Instance().contestdatas_[contestid].contestState >= CONTEST_STATE.CONTEST_STATE_ADMISSION)
                return false;

            if (GameMain.hall_.GetPlayerData().signedContests.Contains(contestid))
            {
                ContestDataManager.Instance().contestdatas_[contestid].contestState = CONTEST_STATE.CONTEST_STATE_ADMISSION;
                if (contestobjects_[contestid] != null)
                    SetSignButtonUI(contestobjects_[contestid], ContestDataManager.Instance().contestdatas_[contestid]);

                if (contestTips_ == null)
                    LoadTipsBackGroundResource();
                contestTips_.SetActive(true);
                LoadTipsResource(ContestDataManager.Instance().contestdatas_[contestid]);
                //GameMain.hall_.GetPlayerData().signedContests.Remove(contestid);
            }
        }
        else if(ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestid))
        {
            if (ContestDataManager.Instance().selfcontestdatas_[contestid].contestState >= CONTEST_STATE.CONTEST_STATE_ADMISSION)
                return false;

            if (GameMain.hall_.GetPlayerData().signedContests.Contains(contestid))
            {
                ContestDataManager.Instance().selfcontestdatas_[contestid].contestState = CONTEST_STATE.CONTEST_STATE_ADMISSION;
                if (contestobjects_[contestid] != null)
                    SetSignButtonUI(contestobjects_[contestid], ContestDataManager.Instance().selfcontestdatas_[contestid]);

                if (contestTips_ == null)
                    LoadTipsBackGroundResource();
                contestTips_.SetActive(true);
                LoadTipsResource(ContestDataManager.Instance().selfcontestdatas_[contestid]);
                //GameMain.hall_.GetPlayerData().signedContests.Remove(contestid);
            }
        }
        else
        {
            CCustomDialog.OpenCustomConfirmUI(1610);
            Debug.Log("error contest id:" + contestid.ToString() +
                "data length:" + ContestDataManager.Instance().contestdatas_.Count.ToString());
            return false;
        }
        GameMain.SC(DelayTipsObject(contestid));

        return true;
    }

    IEnumerator DelayTipsObject(uint contestid)
    {
        yield return new WaitForSecondsRealtime(30.0f);

        if (tipsobjects_.ContainsKey(contestid))
        {
            GameMain.safeDeleteObj(tipsobjects_[contestid]);
            tipsobjects_.Remove(contestid);
            if (tipsobjects_.Count <= 0)
                if (contestTips_ != null)
                    contestTips_.SetActive(false);
        }
    }

    bool BackAdmissionReply(uint msgType, UMessage msg)
    {
        uint playerid = msg.ReadUInt();
        uint contestid = msg.ReadUInt();
        byte state = msg.ReadByte();
        ushort timeLeft = msg.ReaduShort();
        bool backAdmissionState = false;
        contestTips_.transform.Find("TipsBG").gameObject.SetActive(false);
        if (ContestDataManager.Instance().contestdatas_.ContainsKey(contestid))
        {
            if (state == 1)
            {
                contesttype_ = (byte)ContestOrganisersType.ContestOrganisersType_Official;
                ContestDataManager.Instance().currentContestID = contestid;
                currentPage_ = 0;
                backAdmissionState = true;
                GameMain.hall_.EnterGameScene((GameKind_Enum)ContestDataManager.Instance().contestdatas_[contestid].byGameID, GameTye_Enum.GameType_Contest, timeLeft);
            }
            else
            {
                CCustomDialog.OpenCustomConfirmUI(1606);
            }
        }
        else if(ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestid))
        {
            if (state == 1)
            {
                contesttype_ = (byte)ContestOrganisersType.ContestOrganisersType_Building;
                ContestDataManager.Instance().currentContestID = contestid;
                currentPage_ = 0;
                backAdmissionState = true;
                GameMain.hall_.EnterGameScene((GameKind_Enum)ContestDataManager.Instance().selfcontestdatas_[contestid].byGameID, GameTye_Enum.GameType_Contest, timeLeft);
            }
            else
            {
                CCustomDialog.OpenCustomConfirmUI(1606);
            }
        }
        else
        {
            CCustomDialog.OpenCustomConfirmUI(1606);
        }

        if(!backAdmissionState)
        {
            RequestAdmissionButtonInteractable(contestid, true);
        }
        return backAdmissionState;
    }

    void RefreshSignedPlayerNumber(uint contestid, bool add = true)
    {
        if (!contestobjects_.ContainsKey(contestid))
            return;
        if (ContestDataManager.Instance().contestdatas_.ContainsKey(contestid))
        {
            Text number = contestobjects_[contestid].transform.Find("NumPopulation").gameObject.GetComponent<Text>();
            if (add)
                ContestDataManager.Instance().contestdatas_[contestid].nGamePlayerNum += 1;
            else
                ContestDataManager.Instance().contestdatas_[contestid].nGamePlayerNum -= 1;
            number.text = ContestDataManager.Instance().contestdatas_[contestid].nGamePlayerNum.ToString();
        }
        else if(ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestid))
        {
            Text number = contestobjects_[contestid].transform.Find("NumPopulation").gameObject.GetComponent<Text>();
            if (add)
                ContestDataManager.Instance().selfcontestdatas_[contestid].nGamePlayerNum += 1;
            else
                ContestDataManager.Instance().selfcontestdatas_[contestid].nGamePlayerNum -= 1;
            number.text = ContestDataManager.Instance().selfcontestdatas_[contestid].nGamePlayerNum.ToString();
        }
    }

    bool BackSignResult(uint msgType, UMessage msg)
    {
        uint playerid = msg.ReadUInt();
        uint contestid = msg.ReadUInt();
        byte state = msg.ReadByte();

        switch(state)
        {
            case 1:
                GameMain.hall_.GetPlayerData().signedContests.Add(contestid);
                if(ContestDataManager.Instance().contestdatas_.ContainsKey(contestid) && contestobjects_.ContainsKey(contestid))
                {
                    if(ContestDataManager.Instance().contestdatas_[contestid].contestState != CONTEST_STATE.CONTEST_STATE_ADMISSION)
                        ContestDataManager.Instance().contestdatas_[contestid].contestState = CONTEST_STATE.CONTEST_STATE_ENROLL;
                    SetSignButtonUI(contestobjects_[contestid], ContestDataManager.Instance().contestdatas_[contestid]);
                    if(matchinfo_ != null)
                        SetInfoSignButtonUI(ContestDataManager.Instance().contestdatas_[contestid]);

                    RefreshSignedPlayerNumber(contestid);
                }
                else if(ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestid) && contestobjects_.ContainsKey(contestid))
                {
                    if (ContestDataManager.Instance().selfcontestdatas_[contestid].contestState != CONTEST_STATE.CONTEST_STATE_ADMISSION)
                        ContestDataManager.Instance().selfcontestdatas_[contestid].contestState = CONTEST_STATE.CONTEST_STATE_ENROLL;
                    SetSignButtonUI(contestobjects_[contestid], ContestDataManager.Instance().selfcontestdatas_[contestid]);
                    if (matchinfo_ != null)
                        SetInfoSignButtonUI(ContestDataManager.Instance().selfcontestdatas_[contestid]);

                    RefreshSignedPlayerNumber(contestid);
                }
                break;
            case 2:
                CCustomDialog.OpenCustomConfirmUI(1610);
                ContestDataManager.Instance().contestdatas_.Remove(contestid);
                ContestDataManager.Instance().selfcontestdatas_.Remove(contestid);
                if (contestobjects_.ContainsKey(contestid))
                {
                    GameMain.safeDeleteObj(contestobjects_[contestid]);
                    contestobjects_.Remove(contestid);
                }
                break;
            case 3:

                uint tipsId = 0;
                if (state.CompareTo(3) == 0)
                    tipsId = 1607;
                else if (state.CompareTo(4) == 0)
                    tipsId = 1608;
                else
                    tipsId = 1609;

                CCustomDialog.OpenCustomConfirmUI(tipsId);

                if (contestobjects_.ContainsKey(contestid))
                {
                    contestobjects_[contestid].transform.Find("ApplyBG/Button_Apply").GetComponent<Button>().interactable = true;
                    contestobjects_[contestid].transform.Find("ApplyBG/Button_mianfei").GetComponent<Button>().interactable = true;
                }
                break;
            case 6:
                CCustomDialog.OpenCustomConfirmUI(1611);
                ContestDataManager.Instance().contestdatas_.Remove(contestid);
                ContestDataManager.Instance().selfcontestdatas_.Remove(contestid);
                if (contestobjects_.ContainsKey(contestid))
                {
                    GameMain.safeDeleteObj(contestobjects_[contestid]);
                    contestobjects_.Remove(contestid);
                }
                break;
        }

        return true;
    }

    /// <summary>
    /// 比赛信息列表刷新状态
    /// </summary>
    /// <returns></returns>
    public bool CheckContestInfoListRefreshState()
    {
        return bContestInfoListRefreshState;
    }

    bool BackContestInfoList(uint msgType, UMessage msg)
    {
        bContestInfoListRefreshState = true;
        if (GameMain.hall_.GetCurrentGameState() == GameState_Enum.GameState_Game  || HallMain.videotcpclient != null)
            return false;

        if (GameMain.hall_.contestui_ == null)
            return false;

        contesttype_ = msg.ReadByte();

        if(contesttype_ == 1)
            ContestDataManager.Instance().contestdatas_.Clear();
        else
            ContestDataManager.Instance().selfcontestdatas_.Clear();

        byte length = msg.ReadByte();
        for (byte index = 0; index < length; index++)
        {
            stContestData data = new stContestData();

            data.nContestID = msg.ReadUInt();
            data.nContestDataID = msg.ReadUInt();
            data.sContestName = ContestDataManager.Instance().contestData[data.nContestDataID].sContestName;
            data.byGameID = ContestDataManager.Instance().contestData[data.nContestDataID].byGameID;
            if (GameMain.hall_.GetPlayerData().totalinfo.ContainsKey(data.byGameID))
                data.bestsort = GameMain.hall_.GetPlayerData().totalinfo[data.byGameID].bestsort;
            else
                data.bestsort = 0;
            data.nGamePlayerNum = msg.ReaduShort();
            data.tStartTimeSeconds = msg.ReadUInt();
            ContestDataManager.Instance().contestData[data.nContestDataID].tStartTimeSeconds = data.tStartTimeSeconds;
            data.contestState = (CONTEST_STATE)msg.ReadByte();
            data.createid = msg.ReadUInt();
            if (contesttype_ == 2)
            {
                data.nMaxEnrollPlayerNum = msg.ReaduShort();
                data.nEnrollRechargeCoin = msg.ReaduShort();
                data.createName = msg.ReadString();
            }
            
            //data.contestState = CONTEST_STATE.CONTEST_STATE_ENROLL;
            if (contesttype_ == 1)
            {
                data.nMaxEnrollPlayerNum = ContestDataManager.Instance().contestData[data.nContestDataID].nMaxEnrollPlayerNum;
                data.nEnrollRechargeCoin = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollRechargeCoin;
                data.nEnrollMoneyNum = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollMoneyNum;
            }
            data.nRewardDataID = ContestDataManager.Instance().contestData[data.nContestDataID].nRewardDataID;
            data.enContestType = ContestDataManager.Instance().contestData[data.nContestDataID].enContestType;
            data.enContestKind = ContestDataManager.Instance().contestData[data.nContestDataID].enContestKind;
            data.enOrganisersType = ContestDataManager.Instance().contestData[data.nContestDataID].enOrganisersType;
            data.enContestOpenCycle = ContestDataManager.Instance().contestData[data.nContestDataID].enContestOpenCycle;
            data.vecContestDate = ContestDataManager.Instance().contestData[data.nContestDataID].vecContestDate;
            data.vecAdmissionHour = ContestDataManager.Instance().contestData[data.nContestDataID].vecAdmissionHour;
            data.vecContestHour = ContestDataManager.Instance().contestData[data.nContestDataID].vecContestHour;
            data.nExhibitionTime = ContestDataManager.Instance().contestData[data.nContestDataID].nExhibitionTime;
            data.nEnrollStartTime = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollStartTime;
            data.nMinEnrollPlayerNum = ContestDataManager.Instance().contestData[data.nContestDataID].nMinEnrollPlayerNum;
            data.nEnrollReputationMiniNum = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollReputationMiniNum;
            data.nEnrollMasterMiniNum = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollMasterMiniNum;
            data.nEnrollNamelistID = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollNamelistID;
            data.nEnrollItemID = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollItemID;
            data.nEnrollItemNum = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollItemNum;
            data.shPreliminaryRuleID = ContestDataManager.Instance().contestData[data.nContestDataID].shPreliminaryRuleID;
            data.shFinalsRuleID = ContestDataManager.Instance().contestData[data.nContestDataID].shFinalsRuleID;
            data.nContestBeginBroadcastID = ContestDataManager.Instance().contestData[data.nContestDataID].nContestBeginBroadcastID;
            data.nChampionBroadcastID = ContestDataManager.Instance().contestData[data.nContestDataID].nChampionBroadcastID;
            data.nRewardDataID = ContestDataManager.Instance().contestData[data.nContestDataID].nRewardDataID;
            data.nContestQualificationBuyID = ContestDataManager.Instance().contestData[data.nContestDataID].nContestQualificationBuyID;
            data.iconname = ContestDataManager.Instance().contestData[data.nContestDataID].iconname;
            data.nContestRule = ContestDataManager.Instance().contestData[data.nContestDataID].nContestRule;
            data.nContestRuleDetail = ContestDataManager.Instance().contestData[data.nContestDataID].nContestRuleDetail;
            data.playmode = ContestDataManager.Instance().contestData[data.nContestDataID].playmode;
            data.playmodeicon = ContestDataManager.Instance().contestData[data.nContestDataID].playmodeicon;
            data.ticketIcon = ContestDataManager.Instance().contestData[data.nContestDataID].ticketIcon;
            data.reward = ContestDataManager.Instance().contestData[data.nContestDataID].reward;
            data.resetDetail = ContestDataManager.Instance().contestData[data.nContestDataID].resetDetail;

            if (contesttype_ == 1)
                ContestDataManager.Instance().contestdatas_.Add(data.nContestID, data);
            else
                ContestDataManager.Instance().selfcontestdatas_.Add(data.nContestID, data);
        }

        if (contesttype_ == 2)
            GameMain.hall_.selfcontest_.InitSelfContest();
        else
            SortContestByTime();

        LoadContestResource();
        onClickContestType(currentPage_);
        ShowContestPanel();

        if (BagDataManager.GetBagDataInstance().isUseItem)
        {
            BagDataManager.GetBagDataInstance().isUseItem = false;
            if(contesttype_ == 1)
            {
                if (ContestDataManager.Instance().contestdatas_.ContainsKey(BagDataManager.GetBagDataInstance().scriptid))
                {
                    detail_ = ContestDataManager.Instance().contestdatas_[BagDataManager.GetBagDataInstance().scriptid];
                    ShowMatchInfo();
                }
                else
                    CCustomDialog.OpenCustomConfirmUI(1610);
            }
            else
            {
                if (ContestDataManager.Instance().selfcontestdatas_.ContainsKey(BagDataManager.GetBagDataInstance().scriptid))
                {
                    detail_ = ContestDataManager.Instance().selfcontestdatas_[BagDataManager.GetBagDataInstance().scriptid];
                    ShowMatchInfo();
                }
                else
                    CCustomDialog.OpenCustomConfirmUI(1610);
            }
        }
        UpdateContestButtonState(contesttype_);
        return true;
    }

    public void UpdateContestButtonState(byte ContestType)
    {
        bContestInfoListRefreshState = false;
        if (ButtonGF == null || ButtonZJ ==null || root_ == null)
        {
            return;
        }
        bool bGFType = ContestType == 1;
        if (ContestIconNameText == null)
        {
            ContestIconNameText = root_.transform.Find("Top/ImageIcon/Text").gameObject.GetComponent<Text>();
        }
        if(CreateGameBG == null)
        {
            CreateGameBG = root_.transform.Find("Top/CreateGame_BG").gameObject;
        }
        ButtonZJ.interactable = bGFType;
        ButtonGF.interactable = !bGFType;
        ContestIconNameText.text = bGFType ?  "官方比赛" : "自建比赛";
        ButtonGF.gameObject.SetActive(!bGFType);
        ButtonZJ.gameObject.SetActive(bGFType);
        CreateGameBG.SetActive(!bGFType);
    }

    void SortContestByTime()
    {
        //ContestDataManager.Instance().contestdatas_ = ContestDataManager.Instance().contestdatas_.OrderByDescending(o => o.Value.tStartTimeSeconds).ToDictionary(p => p.Key, o => o.Value);
        ContestDataManager.Instance().contestdatas_ = ContestDataManager.Instance().contestdatas_.OrderByDescending(o => o.Value.tStartTimeSeconds).ToDictionary(p => p.Key, o => o.Value);
    }

    void LoadContestBGResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle != null)
        {
            if (root_ == null)
            {
                UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Lobby_tournament");
                root_ = (GameObject)GameMain.instantiate(obj0);

                CanvasObj = GameObject.Find("Canvas/Root");
                root_.transform.SetParent(CanvasObj.transform, false);
                root_.SetActive(false);

                InitContestEvents();
                LoadTipsBackGroundResource();
            }
        }
    }

    public void LoadContestResource()
    {
        LoadContestBGResource();

        InitContestData();

        //ticketpanel_ = root_.transform.FindChild("Pop-up").FindChild("ticket").gameObject;
    }

    void InitTickPanelEvents()
    {
        GameObject closebtn = ticketpanel_.transform.Find("ImageBG").Find("ButtonClose").gameObject;
        XPointEvent.AutoAddListener(closebtn, OnCloseTicketPanel, null);
    }

    void InitContestEvents()
    {
        GameObject returnBtn = root_.transform.Find("Top").Find("ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(returnBtn, OnReturn2Hall, null);

        GameObject toggleGroup = root_.transform.Find("Left").gameObject;
        for (int index = 0; index < toggleGroup.transform.childCount; index++)
        {
            int temp = index;
            Toggle toggle = toggleGroup.transform.GetChild(index).gameObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(
                delegate (bool check)
                {
                    AudioManager.Instance.PlaySound(GameDefine.HallAssetbundleName, "UIbutton02");
                    onClickContestType(temp);
                });
        }

        ButtonGF = root_.transform.Find("Top/ChangeGame_BG/Button_GF").GetComponent<Button>();
        XPointEvent.AutoAddListener(ButtonGF.gameObject, OnClickTournament, 1);

        ButtonZJ = root_.transform.Find("Top/ChangeGame_BG/Button_ZJ").GetComponent<Button>();
        ButtonZJ.interactable = false;
        XPointEvent.AutoAddListener(ButtonZJ.gameObject, OnClickSelfMatch, null);
    }

    //官方比赛
    private void OnClickTournament(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick && ButtonGF)        {
            if(!ButtonGF.interactable)
            {
                return;
            }

            CustomAudio.GetInstance().PlayCustomAudio(1002);

            ButtonGF.interactable = false;

            Go2ContestHall();        }    }

    //进入比赛厅
    public void Go2ContestHall(byte ContestType = 1)
    {
        if (GameMain.hall_.contest == null)
            GameMain.hall_.contest = new Contest();

        if (GameMain.hall_.selfcontest_ == null && ContestType == 2)
            GameMain.hall_.selfcontest_ = new SelfContest();

        UMessage ask4ContestDataMsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_RequestContestInfo);

        ask4ContestDataMsg.Add(ContestType);

        NetWorkClient.GetInstance().SendMsg(ask4ContestDataMsg);

        LoadContestBGResource();
        root_.SetActive(true);
        UpdateContestButtonState(ContestType);
        ResetRefreshContestTime();
    }

    //自建比赛
    private void OnClickSelfMatch(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick && ButtonZJ)
        {
            if(!ButtonZJ.interactable)
            {
                return;
            }
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            ButtonZJ.interactable = false;

            if (GameMain.hall_.selfcontest_ == null)
                GameMain.hall_.selfcontest_ = new SelfContest();

            GameMain.hall_.selfcontest_.Ask4SelfContestList();
        }
    }

    public void onClickContestType(int contestindex = 0)
    {
        if(GameMain.hall_.selfcontest_ != null)
            GameMain.hall_.selfcontest_.selfcreateNumber_ = 0;

        LoadContestResource();
        root_.SetActive(true);
        InitContestMsg();

        GameObject background = root_.transform.Find("Right").
            Find("Viewport_tournament").Find("Content_tournament").gameObject;

        //GameMain.hall_.ClearChilds(background);
        //contestobjects_.Clear();
        currentPage_ = contestindex;

        GameObject nonecontest = root_.transform.Find("Right/Viewport_tournament/Imagekong").gameObject;
        if(contesttype_ == 1)
        {
            nonecontest.SetActive(ContestDataManager.Instance().contestdatas_.Count == 0);

            int index = 0;
            foreach (uint key in ContestDataManager.Instance().contestdatas_.Keys)
            {
                int temp = index;
                LoadContestItemResource(ContestDataManager.Instance().contestdatas_[key], temp);
                index++;
            }

            foreach (uint key in contestobjects_.Keys)
            {
                if (contestobjects_[key] != null)
                    contestobjects_[key].SetActive(ContestDataManager.Instance().contestdatas_.ContainsKey(key));
            }
        }
        else
        {
            nonecontest.SetActive(ContestDataManager.Instance().selfcontestdatas_.Count == 0);

            List<uint> topresult = new List<uint>();
            List<uint> desckeys = new List<uint>(ContestDataManager.Instance().selfcontestdatas_.Keys);

            for (int index = desckeys.Count - 1; index >= 0; index--)
                if (ContestDataManager.Instance().selfcontestdatas_[desckeys[index]].createid != GameMain.hall_.GetPlayerId())
                    topresult.Add(desckeys[index]);

            for (int index = 0; index < topresult.Count; index++)
                desckeys.Remove(topresult[index]);

            for (int index = desckeys.Count - 1; index >= 0; index--)
                topresult.Add(desckeys[index]);

            for (int index = 0; index < topresult.Count; index++)
            {
                int temp = index;
                LoadContestItemResource(ContestDataManager.Instance().selfcontestdatas_[topresult[index]], temp);
            }

            foreach (uint key in contestobjects_.Keys)
            {
                if (contestobjects_[key] != null)
                    contestobjects_[key].SetActive(ContestDataManager.Instance().selfcontestdatas_.ContainsKey(key));
            }
        }
    }

    void LoadContestItemResource(stContestData data, int index)
    {
        if(root_ != null)
        {
            GameObject background = root_.transform.Find("Right").
                Find("Viewport_tournament").Find("Content_tournament").gameObject;

            AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            AssetBundle bagbundle = AssetBundleManager.GetAssetBundle(GameDefine.HallBagIconAssetBundleName);

            if (bundle != null)
            {
                if (root_ != null)
                {
                    GameObject item = null;
                    if (contestobjects_.ContainsKey(data.nContestID))
                    {
                        item = contestobjects_[data.nContestID];
                        if(item == null)
                        {
                            contestobjects_.Remove(data.nContestID);

                            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Tournament_Info");
                            item = (GameObject)GameMain.instantiate(obj0);
                            item.transform.SetParent(background.transform, false);
                        }
                    }
                    else
                    {
                        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Tournament_Info");
                        item = (GameObject)GameMain.instantiate(obj0);
                        item.transform.SetParent(background.transform, false);
                        //item.transform.SetAsFirstSibling();
                    }

                    item.transform.Find("ImageIcon_zijian").gameObject.SetActive(data.createid == GameMain.hall_.GetPlayerId());
                    if (data.createid == GameMain.hall_.GetPlayerId())
                        GameMain.hall_.selfcontest_.selfcreateNumber_ += 1;
                    Image icon = item.transform.Find("ImageIcon").gameObject.GetComponent<Image>();
                    icon.sprite = GetContestImages(data.iconname);
                    Image name = item.transform.Find("TextName").gameObject.GetComponent<Image>();
                    name.sprite = bundle.LoadAsset<Sprite>(data.playmodeicon);
                    Text number = item.transform.Find("NumPopulation").gameObject.GetComponent<Text>();
                    number.text = data.nGamePlayerNum.ToString();
                    SetTimeType(item, data);
                    Text reward = item.transform.Find("TextBonus").gameObject.GetComponent<Text>();
                    //Debug.Log("load resource contestid :" + data.nContestID);
                    if (contesttype_ == 1)
                        //reward.text = ContestDataManager.Instance().GetCurrentRewardData(1, data.nContestID).nRewardContent;
                        reward.text = data.reward;
                    else
                    {
                        string contestName = "";
                        if (data.enContestType == ContestType.ContestType_AnyTime)
                            contestName = "的即开赛";
                        else
                            contestName = "的定时赛";

                        reward.text = "<color=#bf442d>" + data.createName + "</color>" + contestName;
                    }

                    if (bagbundle != null)
                    {
                        Image ticketIcon = item.transform.Find("ImageTicket").Find("ImageIcon").gameObject.GetComponent<Image>();
                        ticketIcon.sprite = bagbundle.LoadAsset<Sprite>(data.ticketIcon);
                    }
                    Text ticketNumber = item.transform.Find("ImageTicket").Find("Textnum").gameObject.GetComponent<Text>();
                    if (data.nEnrollRechargeCoin != 0)
                        ticketNumber.text = data.nEnrollRechargeCoin.ToString();
                    //if (data.nEnrollMoneyNum != 0)
                        ticketNumber.text = data.nEnrollRechargeCoin.ToString();
                    //else
                    //{
                    //    //ticketNumber.text = "免费";
                    //    //item.transform.FindChild("ImageTicket").FindChild("ImageIcon").gameObject.SetActive(false);
                    //}
                    //SetTicketType(item, data);
                    SetSignButtonUI(item, data);

                    XPointEvent.AutoAddListener(item, OnClickContestDetial, data);
                    XPointEvent.AutoAddListener(item.transform.Find("ApplyBG").Find("Button_Apply").gameObject, OnSignContestMsg, data.nContestID);
                    XPointEvent.AutoAddListener(item.transform.Find("ApplyBG").Find("Button_ApplyDelay").gameObject, OnSignContestMsg, data.nContestID);
                    XPointEvent.AutoAddListener(item.transform.Find("ApplyBG").Find("Button_mianfei").gameObject, OnSignContestMsg, data.nContestID);
                    XPointEvent.AutoAddListener(item.transform.Find("ApplyBG").Find("Button_jinru").gameObject, OnGoinContest, data.nContestID);

                    if (!contestobjects_.ContainsKey(data.nContestID))
                        contestobjects_.Add(data.nContestID, item);

                    item.transform.SetAsFirstSibling();
                }
            }
        }
    }

    public void OnClickContestDetial(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            if(matchinfo_ != null && matchinfo_.activeSelf)
            {
                return;
            }
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            detail_ = (stContestData)button;
            ShowMatchInfo();
        }
    }

    void ShowMatchInfo()
    {
        LoadContestInfoResource(detail_);
        GameObject togglebg = matchinfo_.transform.Find("ImageBG").Find("ToggleGroup").gameObject;

        bool isShowBtn = HallMain.gametcpclient == null;
        if (isShowBtn)
        {
            Toggle first = togglebg.transform.Find("Toggle_gaikuang").gameObject.GetComponent<Toggle>();
            first.isOn = true;
        }
        else
        {
            Toggle second = togglebg.transform.Find("Toggle_saikuang").gameObject.GetComponent<Toggle>();
            second.isOn = true;
        }
        for (int index = 1; index < togglebg.transform.childCount; index++)
        {
            int temp = index;
            Toggle child = togglebg.transform.GetChild(index).gameObject.GetComponent<Toggle>();
            child.isOn = false;
        }
        if (isShowBtn)
            OnSwitchMatchInfo(0);
        else
            OnSwitchMatchInfo(1);
        matchinfo_.SetActive(true);
    }

    public void LoadContestInfoResource(stContestData detail)
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle != null)
        {
            if (matchinfo_ == null)
            {
                UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("MatchInfo");
                matchinfo_ = (GameObject)GameMain.instantiate(obj0);

                CanvasObj = GameObject.Find("Canvas/Root");
                matchinfo_.transform.SetParent(CanvasObj.transform, false);
            }

            InitMatchInfoEvents();
        }

        InitMatchInfoUI(detail);

        matchinfo_.SetActive(true);
    }

    //比赛概况页面事件
    void InitMatchInfoEvents()
    {
        GameObject closebtn = matchinfo_.transform.Find("ImageBG").Find("ButtonClose").gameObject;
        XPointEvent.AutoAddListener(closebtn, OnCloseMatchInfo, null);

        GameObject togglebg = matchinfo_.transform.Find("ImageBG").Find("ToggleGroup").gameObject;
        for(int index = 0; index < togglebg.transform.childCount; index++)
        {
            int temp = index;
            Toggle child = togglebg.transform.GetChild(index).gameObject.GetComponent<Toggle>();
            child.onValueChanged.AddListener(
                delegate (bool check)
                {
                    CustomAudio.GetInstance().PlayCustomAudio(1002);
                    OnSwitchMatchInfo(temp);
                });
        }

        bool isShowBtn = GameMain.hall_.GetCurrentGameState() != GameState_Enum.GameState_Contest;
        if (!isShowBtn)
            return;

        GameObject goinBtn = matchinfo_.transform.Find("ImageBG/InfoBG/Image_gaikuang/ImageBG/Button_jinru").gameObject;
        XPointEvent.AutoAddListener(goinBtn, OnGoinContest, detail_.nContestID);

        GameObject signBtn = matchinfo_.transform.Find("ImageBG/InfoBG/Image_gaikuang/ImageBG/Button_baoming").gameObject;
        XPointEvent.AutoAddListener(signBtn, OnSignContestMsg, detail_.nContestID);

        GameObject tuisaiBtn = matchinfo_.transform.Find("ImageBG/InfoBG/Image_gaikuang/ImageBG/Button_tuisai").gameObject;
        XPointEvent.AutoAddListener(tuisaiBtn, OnTuiSai, detail_.nContestID);

        GameObject breakBtn = matchinfo_.transform.Find("ImageBG/InfoBG/Image_gaikuang/ImageBG/Button_jiesan").gameObject;
        XPointEvent.AutoAddListener(breakBtn, OnBreakContest, detail_.nContestID);

        GameObject sharebtn = matchinfo_.transform.Find("ImageBG/InfoBG/Image_gaikuang/Button_share").gameObject;
        //XPointEvent.AutoAddListener(sharebtn, OnShareSelfContest, detail_.nContestID);
        sharebtn.SetActive(false);
    }

    //分享自建比赛
    public void OnShareSelfContest(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            uint shareContestid = (uint)button;

            stContestData sharedata = ContestDataManager.Instance().selfcontestdatas_[shareContestid];
            string content = "";

            if (sharedata.enContestType == ContestType.ContestType_Timing)
            {
                content = "自建比赛 " + "报名密码:" + shareContestid.ToString() +
                    CCsvDataManager.Instance.GameDataMgr.GetGameData(ContestDataManager.Instance().selfcontestdatas_[shareContestid].byGameID).GameName + "定时赛" +
                    GameCommon.ConvertLongToDateTime(sharedata.tStartTimeSeconds).ToString("yyyy年MM月dd日HH:mm")
                    + "开赛，输入报名密码即可参赛，快来报名吧。";
            }
            else
            {
                content = "自建比赛 " + "报名密码:" + shareContestid.ToString() +
                    CCsvDataManager.Instance.GameDataMgr.GetGameData(ContestDataManager.Instance().selfcontestdatas_[shareContestid].byGameID).GameName + "即开赛" +
                    sharedata.nMaxEnrollPlayerNum.ToString() + "人坐满即开，输入报名密码即可参赛，快来报名吧。";
            }

            Player.ShareURLToWechat("http://gdcdn.qpdashi.com",content, false);
        }
    }

    //解散比赛
    private void OnBreakContest(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            cancelContestid = (uint)button;

            string[] param = { cancelContestid.ToString() };
            CCustomDialog.OpenCustomDialogWithFormatParams(1603, SendBreakContestMsg, param);
        }
    }

    bool BackBreakContestMsg(uint msgType, UMessage msg)
    {
        byte state = msg.ReadByte();
        if (state == 0)
        {
            CCustomDialog.OpenCustomConfirmUI(2506);
            return false;
        }

        GameMain.hall_.selfcontest_.selfcreateNumber_ -= 1;

        uint contestid = msg.ReadUInt();
        if(ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestid))
            ContestDataManager.Instance().selfcontestdatas_.Remove(contestid);
        if(contestobjects_.ContainsKey(contestid))
        {
            GameMain.safeDeleteObj(contestobjects_[contestid]);
            contestobjects_.Remove(contestid);
        }

        if (matchinfo_ != null)
            matchinfo_.SetActive(false);

        return true;
    }

    //解散比赛
    void SendBreakContestMsg(object isagree)
    {
        bool agree = (int)isagree == 1;

        if (!agree)
            return;

        if (cancelContestid == 0)
            return;

        if (contestobjects_.ContainsKey(cancelContestid))
        {
            contestobjects_[cancelContestid].transform.Find("ApplyBG/Button_Apply").GetComponent<Button>().interactable = true;
            contestobjects_[cancelContestid].transform.Find("ApplyBG/Button_mianfei").GetComponent<Button>().interactable = true;
        }

        //发送解散比赛消息
        UMessage breakMsg = new UMessage((uint)GameCity.EMSG_ENUM.Contestmsg_PlayerDisbandCreateContestReq);

        breakMsg.Add(GameMain.hall_.GetPlayerId());
        breakMsg.Add(cancelContestid);

        NetWorkClient.GetInstance().SendMsg(breakMsg);
    }

    //退赛
    private void OnTuiSai(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            cancelContestid = (uint)button;

            string[] param = { cancelContestid.ToString() };
            CCustomDialog.OpenCustomDialogWithFormatParams(1603, SendTuiSaiMsg, param);
        }
    }

    uint cancelContestid = 0;

    void SendTuiSaiMsg(object isagree)
    {
        bool agree = (int)isagree == 1;

        if (!agree)
            return;

        if (cancelContestid == 0)
            return;

        if(contestobjects_.ContainsKey(cancelContestid))
        {
            contestobjects_[cancelContestid].transform.Find("ApplyBG/Button_Apply").GetComponent<Button>().interactable = true;
            contestobjects_[cancelContestid].transform.Find("ApplyBG/Button_mianfei").GetComponent<Button>().interactable = true;
        }

        UMessage cancelMsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_PlayerCancelEnroll);

        cancelMsg.Add(GameMain.hall_.GetPlayerId());
        cancelMsg.Add(cancelContestid);

        NetWorkClient.GetInstance().SendMsg(cancelMsg);
    }

    //进入比赛
    private void OnGoinContest(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            uint contestid = (uint)button;

            if (contestobjects_.ContainsKey(contestid))
            {
                if(!contestobjects_[contestid].transform.Find("ApplyBG/Button_jinru").GetComponent<Button>().interactable)
                {
                    return;
                }
            }

            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (tipsobjects_.ContainsKey(contestid))
            {
                GameMain.safeDeleteObj(tipsobjects_[contestid]);
                tipsobjects_.Remove(contestid);
                if (tipsobjects_.Count <= 0)
                    contestTips_.transform.Find("Button_Begintips").gameObject.SetActive(false);
            }

           byte gameid = 0;
           if (ContestDataManager.Instance().contestdatas_.ContainsKey(contestid))
            {
                gameid = ContestDataManager.Instance().contestdatas_[contestid].byGameID;
                if (!GameMain.hall_.GetPlayerData().signedContests.Contains(contestid)&& 
                    ContestDataManager.Instance().contestdatas_[contestid].enContestType != ContestType.ContestType_AnyTime)
                {
                    CCustomDialog.OpenCustomConfirmUI(1606);
                    return;
                }

            }
            else if(ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestid))
            {
                gameid = ContestDataManager.Instance().selfcontestdatas_[contestid].byGameID;
                if (!GameMain.hall_.GetPlayerData().signedContests.Contains(contestid)&& 
                    ContestDataManager.Instance().selfcontestdatas_[contestid].enContestType != ContestType.ContestType_AnyTime)
                {
                    CCustomDialog.OpenCustomConfirmUI(1606);
                    return;
                }
            }

            if (gameid == 0)
                return;

            byte state = CResVersionCompareUpdate.CheckGameResNeedDownORUpdate(gameid);
            if (state != 0)
            {
                uint tips = 1024;

                currentNeedUpdateGameID = gameid;
                CCustomDialog.OpenCustomDialogWithTipsID(tips, DownLoadGameResource);
                return;
            }

            UMessage admissionMsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_PlayerRequestAdmission);
            admissionMsg.Add(GameMain.hall_.GetPlayerId());
            admissionMsg.Add(contestid);
            NetWorkClient.GetInstance().SendMsg(admissionMsg);
            RequestAdmissionButtonInteractable(contestid,false);
        }
    }

    void RequestAdmissionButtonInteractable(uint contestID,bool bInteractable)
    {
        if (matchinfo_)
        {
            matchinfo_.transform.Find("ImageBG/InfoBG/Image_gaikuang/ImageBG/Button_jinru").GetComponent<Button>().interactable = bInteractable;
        }
        if (contestobjects_.ContainsKey(contestID))
        {
            contestobjects_[contestID].transform.Find("ApplyBG/Button_jinru").GetComponent<Button>().interactable = bInteractable;
        }
    }


    //比赛概况
    void OnSwitchMatchInfo(int toggleindex)
    {
        GameObject info = matchinfo_.transform.Find("ImageBG").Find("InfoBG").gameObject;
        for (int index = 1; index < info.transform.childCount; index++)
        {
            info.transform.GetChild(index).gameObject.SetActive(index == toggleindex + 1);
        }
    }

    //关闭概况
    private void OnCloseMatchInfo(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            matchinfo_.SetActive(false);
        }
    }

    string GetGameNameByID(uint gameid)
    {
        return "";
    }

    string GetItemNameByID(uint itemid)
    {
        return "";
    }

    string GetSignPayByID(stContestData detail)
    {
        string result = "";

        if (detail.nEnrollMoneyNum == 0 && detail.nEnrollRechargeCoin == 0 && detail.nEnrollItemNum == 0)
            return "免费";

        if (detail.nEnrollMoneyNum > 0)
            result += detail.nEnrollMoneyNum.ToString() + "金币";
        if (detail.nEnrollRechargeCoin > 0)
            //if (detail.nEnrollMasterMiniNum > 0)
            //    result += " ";
            //else
                result += detail.nEnrollRechargeCoin.ToString() + "钻石";
        if (detail.nEnrollItemNum > 0)
            result += " " + GetItemNameByID(detail.nEnrollItemID) + "x" + detail.nEnrollItemNum.ToString();
        return result;
    }

    //初始化概况信息
    void InitMatchInfoUI(stContestData detail)
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        //第一页
        GameObject BG = matchinfo_.transform.Find("ImageBG").Find("InfoBG").
            Find("Image_gaikuang").Find("Viewport").Find("Content").gameObject;
        Text contestName = matchinfo_.transform.Find("ImageBG").Find("InfoBG").
            Find("Image_gaikuang").Find("ImageBgTop").Find("Text").gameObject.GetComponent<Text>();
        //GameObject breakBtn = matchinfo_.transform.FindChild("ImageBG/InfoBG/Image_gaikuang/ImageBG/Button_jiesan").gameObject;
        //breakBtn.SetActive(detail.createid == GameMain.hall_.GetPlayerId());
        contestName.text = detail.sContestName;
        Text rule = BG.transform.Find("Text_rule").Find("TextContent").gameObject.GetComponent<Text>();
        switch(detail.shPreliminaryRuleID)
        {
            case 0:
                rule.text = "定局积分";
                break;
        }
        Text time = BG.transform.Find("Text_time").Find("TextContent").gameObject.GetComponent<Text>();

        //System.DateTime sdt = new System.DateTime(1970, 1, 1);
        //sdt.AddSeconds(detail_.tStartTimeSeconds);

        time.text = "";
        if (detail.enContestType == ContestType.ContestType_Timing)
        {
            if (detail.tStartTimeSeconds == 0)
                time.text = "";
            else
            {
                System.DateTime sdt = GameCommon.ConvertLongToDateTime(detail.tStartTimeSeconds);
                string timeTx = sdt.ToString("yyyy年MM月dd日HH:mm");
                time.text = timeTx;
            }
        }
        else
        {
            time.text = "即开";
        }
        Text population = BG.transform.Find("Text_population").Find("TextContent").gameObject.GetComponent<Text>();
        string TextValue = string.Empty;
        if(detail.nGamePlayerNum != 0 || GameMain.hall_.GetCurrentGameState() == GameState_Enum.GameState_Hall)
        {
            TextValue = "<color=#66CD00>";
            if (contesttype_ == 1)
                TextValue += ContestDataManager.Instance().contestdatas_[detail.nContestID].nGamePlayerNum.ToString() + "</color>(最小开赛人数" +
                             ContestDataManager.Instance().contestdatas_[detail.nContestID].nMinEnrollPlayerNum.ToString();
            else if(detail.enContestType == ContestType.ContestType_AnyTime)
                TextValue += ContestDataManager.Instance().selfcontestdatas_[detail.nContestID].nGamePlayerNum.ToString() + "</color>(最小开赛人数" +
                             detail.nMaxEnrollPlayerNum.ToString();
            else
                TextValue += ContestDataManager.Instance().selfcontestdatas_[detail.nContestID].nGamePlayerNum.ToString() + "</color>(最小开赛人数" +
                             ContestDataManager.Instance().selfcontestdatas_[detail.nContestID].nMinEnrollPlayerNum.ToString();
            TextValue += ")";
        }
        population.text = TextValue;
        Text game = BG.transform.Find("Text_game").Find("TextContent").gameObject.GetComponent<Text>();
        game.text = detail.playmode;
        Text condition = BG.transform.Find("Text_condition").Find("TextContent").gameObject.GetComponent<Text>();

        string request = "";
        if (detail.nEnrollReputationMiniNum == 0 && detail.nEnrollMasterMiniNum == 0)
            request += "不限";
        else 
        {
            if (detail.nEnrollReputationMiniNum > 0)
                request += "信誉分≥" + detail.nEnrollReputationMiniNum.ToString();
            if (detail.nEnrollMasterMiniNum > 0)
                request += " 大师分≥" + detail.nEnrollMasterMiniNum.ToString();
        }

        condition.text = request;

        Text money = BG.transform.Find("Text_money").Find("TextContent").gameObject.GetComponent<Text>();
        money.text = GetSignPayByID(detail);
        Text reset = BG.transform.Find("Text_chonggou").Find("TextContent").gameObject.GetComponent<Text>();
        //if (detail.nContestQualificationBuyID > 0)
        //    reset.text = "是";
        //else
        //    reset.text = "否";
        reset.text = detail.resetDetail;
        Text bestsort = matchinfo_.transform.Find("ImageBG").Find("InfoBG").
            Find("Image_gaikuang").Find("ownrankingBG").Find("TextNum").
            gameObject.GetComponent<Text>();
        if (detail.bestsort == 0)
            bestsort.text = "暂无";
        else
            bestsort.text = detail.bestsort.ToString(); //历史最高排名
        bool isShowBtn = GameMain.hall_.GetCurrentGameState() != GameState_Enum.GameState_Contest;
        if (isShowBtn)
            SetInfoSignButtonUI(detail);
        else
            matchinfo_.transform.Find("ImageBG").Find("InfoBG").
                Find("Image_gaikuang").Find("ImageBG").gameObject.SetActive(false);

        Text contestidTx = matchinfo_.transform.Find("ImageBG/InfoBG/Image_gaikuang/Text_ID/TextContent").gameObject.GetComponent<Text>();
        if (contesttype_ == 2)
        {
            if (detail.createid == GameMain.hall_.GetPlayerId())
            {
                contestidTx.text = detail.nContestID.ToString();
                contestidTx.gameObject.transform.parent.gameObject.SetActive(true);
            }
            else
                contestidTx.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
            contestidTx.gameObject.transform.parent.gameObject.SetActive(false);

        GameObject sharebtn = matchinfo_.transform.Find("ImageBG/InfoBG/Image_gaikuang/Button_share").gameObject;
        //sharebtn.SetActive(contesttype_ == 2);
        sharebtn.SetActive(false);

        //第二页
        GameObject noneinfo = matchinfo_.transform.Find("ImageBG").Find("InfoBG").
            Find("Image_saikuang").Find("Viewport").Find("Content").Find("none").gameObject;
        
        noneinfo.SetActive(isShowBtn);
        GameObject selfinfo = matchinfo_.transform.Find("ImageBG").Find("InfoBG").
            Find("Image_saikuang").Find("MatchInfo_saikuang_ziji").gameObject;
        selfinfo.SetActive(false);

        GameObject togglebg = matchinfo_.transform.Find("ImageBG").Find("ToggleGroup").gameObject;

        for (int index = 0; index < togglebg.transform.childCount; index++)
        {
            int temp = index;
            Toggle child = togglebg.transform.GetChild(index).gameObject.GetComponent<Toggle>();
            child.isOn = false;
        }
        
        if (isShowBtn)
        {
            Toggle first = togglebg.transform.Find("Toggle_gaikuang").gameObject.GetComponent<Toggle>();
            first.isOn = true;
        }
        else
        {
            Toggle second = togglebg.transform.Find("Toggle_saikuang").gameObject.GetComponent<Toggle>();
            second.isOn = true;
        }

        if (isShowBtn)
            OnSwitchMatchInfo(0);
        else
            OnSwitchMatchInfo(1);

        if (!isShowBtn)
        {
            //for (int index = 0; index < detail_.ci.Count; index++)
            //    LoadSortInfoResource(index);

            //Text tableNum = matchinfo_.transform.FindChild("ImageBG").FindChild("InfoBG").
            //    FindChild("Image_saikuang").FindChild("Imagebottom").FindChild("TextNum").gameObject.GetComponent<Text>();
            //Text lefttime = matchinfo_.transform.FindChild("ImageBG").FindChild("InfoBG").
            //    FindChild("Image_saikuang").FindChild("Imagebottom").FindChild("TextTime").gameObject.GetComponent<Text>();
            //SetTableState(tableNum, lefttime);
        }

        //第三页
        List<stRankingRewardData> rewardlist = ContestDataManager.Instance().rewardData_[detail.nRewardDataID].vecRankingReward;

        GameObject BGReward = matchinfo_.transform.Find("ImageBG").Find("InfoBG").
            Find("Image_jiangli").Find("Viewport").Find("Content").gameObject;
        GameMain.hall_.ClearChilds(BGReward);
        for (uint index = 0; index < rewardlist.Count; index++)
            LoadSortRewardInfo(index, detail);
        //Text tableNum2 = matchinfo_.transform.FindChild("ImageBG").FindChild("InfoBG").
        //    FindChild("Image_jiangli").FindChild("Imagebottom").FindChild("TextNum").gameObject.GetComponent<Text>();
        //Text lefttime2 = matchinfo_.transform.FindChild("ImageBG").FindChild("InfoBG").
        //    FindChild("Image_jiangli").FindChild("Imagebottom").FindChild("TextTime").gameObject.GetComponent<Text>();
        //SetTableState(tableNum2, lefttime2);

        //第四页
        Text detailrule = matchinfo_.transform.Find("ImageBG").Find("InfoBG").
            Find("Image_guize").Find("Viewport").Find("Content").Find("Text").gameObject.GetComponent<Text>();
        
        detailrule.text = detail.nContestRuleDetail.Replace("\\n", "\n");
    }

    //初始化桌子情况
    void SetTableState(Text tableNum, Text lefttime)
    {
        //tableNum.text = detail_.lefttable.ToString() + "/" + detail_.maxtable.ToString();
        //lefttime.text = detail_.lefttime;
    }

    //加载排名奖励资源
    void LoadSortRewardInfo(uint index, stContestData detail)
    {
        List<stRankingRewardData> rewardlist = ContestDataManager.Instance().rewardData_[detail.nRewardDataID].vecRankingReward;

        GameObject BG = matchinfo_.transform.Find("ImageBG").Find("InfoBG").
            Find("Image_jiangli").Find("Viewport").Find("Content").gameObject;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle != null)
        {
            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("MatchInfo_jiangli");
            GameObject sortreward = (GameObject)GameMain.instantiate(obj0);
            sortreward.transform.SetParent(BG.transform, false);

            Text rank = sortreward.transform.Find("TextRanking").gameObject.GetComponent<Text>();
            if(rewardlist[(int)index].nRankingBegin == rewardlist[(int)index].nRankingEnd)
            {
                rank.text = rewardlist[(int)index].nRankingBegin.ToString();
                if(rewardlist[(int)index].nRankingBegin < 4)
                {
                    Image sortimg = sortreward.transform.Find("ImageIcon").gameObject.GetComponent<Image>();
                    sortimg.sprite = sortImages_[(int)rewardlist[(int)index].nRankingBegin - 1];
                    sortimg.gameObject.SetActive(true);
                }
            }
            else
                rank.text = rewardlist[(int)index].nRankingBegin.ToString() + "-"  + rewardlist[(int)index].nRankingEnd.ToString();
            Text reward = sortreward.transform.Find("Textjiangli").gameObject.GetComponent<Text>();
            reward.text = rewardlist[(int)index].nRewardContent;
        }
    }

    //加载排名信息资源
    void LoadSortInfoResource(int index)
    {
        GameObject BG2 = matchinfo_.transform.Find("ImageBG").Find("InfoBG").
            Find("Image_saikuang").Find("Viewport").Find("Content").gameObject;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle != null)
        {
            //UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("MatchInfo_saikuang");
            //GameObject sortinfo = (GameObject)GameMain.instantiate(obj0);
            //sortinfo.transform.SetParent(BG2.transform, false);

            //Text rank = sortinfo.transform.FindChild("TextRanking").gameObject.GetComponent<Text>();
            //rank.text = detail_.ci[index].sortNo.ToString();
            //Text name = sortinfo.transform.FindChild("TextName").gameObject.GetComponent<Text>();
            //name.text = detail_.ci[index].name;
            //Text score = sortinfo.transform.FindChild("TextGrade").gameObject.GetComponent<Text>();
            //score.text = detail_.ci[index].score.ToString();
        }
    }

    bool ismoney = true;
    void LoadTicketResource(int index, uint money, uint diamond, uint itemid, uint itemnum, uint contestid)
    {
        if (money == diamond && diamond == itemid  && itemid == 0)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle != null)
        {
            GameObject ticketback = root_.transform.Find("Pop-up").Find("ticket").
                Find("ImageBG").Find("Viewport").Find("Content").gameObject;

            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Tournament_Ticket");
            GameObject ticket = (GameObject)GameMain.instantiate(obj0);
            ticket.transform.SetParent(ticketback.transform, false);

            Toggle item = ticketback.transform.Find("Toggle").gameObject.GetComponent<Toggle>();
            item.group = ticketback.GetComponent<ToggleGroup>();

            Text payTx = item.transform.Find("ImageBG").Find("ImageTicketBG").Find("TextMoney").gameObject.GetComponent<Text>();
            Text ticketTx = item.transform.Find("ImageBG").Find("ImageTicketBG").Find("TextTicket").gameObject.GetComponent<Text>();

            item.onValueChanged.AddListener(delegate (bool ispick) 
            {
                AudioManager.Instance.PlaySound(GameDefine.HallAssetbundleName, "UIbutton02");
                ismoney = money != 0 && index == 0;
                if (ismoney)
                    payTx.text = money.ToString() + "金币";
                else if (diamond == 0)
                    payTx.gameObject.SetActive(false);
                else
                    payTx.text = diamond.ToString() + "钻石";

                if (itemid != 0 && itemnum != 0)
                {
                    ticketTx.text = itemnum.ToString();
                    ticketTx.gameObject.SetActive(true);
                }
                else
                    ticketTx.gameObject.SetActive(false);
            });

            GameObject signbtn = ticketpanel_.transform.Find("ImageBG").Find("Button_Apply").gameObject;
            XPointEvent.AutoAddListener(signbtn, OnSignContestMsg, contestid);
            GameObject signbtnfree = ticketpanel_.transform.Find("ImageBG").Find("Button_mianfei").gameObject;
            XPointEvent.AutoAddListener(signbtnfree, OnSignContestMsg, contestid);
        }
    }

    private void OnCloseTicketPanel(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            ticketpanel_.SetActive(false);
        }
    }

    uint currentNeedUpdateGameID;
    private void OnSignContestMsg(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            uint key = (uint)button;
            if(contesttype_ == 1)
            {
                if (!ContestDataManager.Instance().contestdatas_.ContainsKey(key))
                {
                    CCustomDialog.OpenCustomConfirmUI(1610);
                    return;
                }

                byte gameid = ContestDataManager.Instance().contestdatas_[key].byGameID;
                byte state = CResVersionCompareUpdate.CheckGameResNeedDownORUpdate(gameid);
                if (state != 0)
                {
                    uint tips = 1024;

                    currentNeedUpdateGameID = gameid;
                    CCustomDialog.OpenCustomDialogWithTipsID(tips, DownLoadGameResource);

                    return;
                }
            }
            else
            {
                if (!ContestDataManager.Instance().selfcontestdatas_.ContainsKey(key))
                {
                    CCustomDialog.OpenCustomConfirmUI(1610);
                    return;
                }

                byte gameid = ContestDataManager.Instance().selfcontestdatas_[key].byGameID;
                byte state = CResVersionCompareUpdate.CheckGameResNeedDownORUpdate(gameid);
                if (state != 0)
                {
                    uint tips = 1024;

                    currentNeedUpdateGameID = gameid;
                    CCustomDialog.OpenCustomDialogWithTipsID(tips, DownLoadGameResource);

                    return;
                }

                if (ContestDataManager.Instance().selfcontestdatas_[key].createid != GameMain.hall_.GetPlayerId())
                {
                    NumberPanel.GetInstance().SetNumberPanelActive(true,SendSignMsg);
                    return;
                }
            }

            SendSignMsg(key);
        }
    }

    void DownLoadGameResource(object param)
    {
        int select = (int)param;
        if(select == 1)
        {
            if (waitingTips_ == null)
                LoadWaitingTipsResource();
            waitingTips_.SetActive(true);
            GameMain.hall_.CheckGameResourceUpdate((byte)currentNeedUpdateGameID);

            isDownload_ = true;

            if (contestTips_ != null)
                contestTips_.SetActive(false);
        }
    }

    void LoadWaitingTipsResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Tips_Waiting");
        waitingTips_ = (GameObject)GameMain.instantiate(obj0);
        waitingTips_.transform.SetParent(CanvasObj.transform, false);

    }

    void SendSignMsg(uint key)
    {
        if (!contestobjects_.ContainsKey(key))
        {
            CCustomDialog.OpenCustomConfirmUI(1665);
            return;
        }

        Button BaoMingButton = contestobjects_[key].transform.Find("ApplyBG/Button_Apply").GetComponent<Button>();
        Button MianFeiBaoMingButton = contestobjects_[key].transform.Find("ApplyBG/Button_mianfei").GetComponent<Button>();
        if (!BaoMingButton.interactable && !MianFeiBaoMingButton.interactable)
        {
            return;
        }

        BaoMingButton.interactable = false;
        MianFeiBaoMingButton.interactable = false;

        ContestType contestType = contesttype_ == 1 ? ContestDataManager.Instance().contestdatas_[key].enContestType : ContestDataManager.Instance().selfcontestdatas_[key].enContestType;
       
        if (contestType == ContestType.ContestType_AnyTime)
        {
            UMessage admissionMsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_PlayerRequestAdmission);

            admissionMsg.Add(GameMain.hall_.GetPlayerId());
            admissionMsg.Add(key);

            NetWorkClient.GetInstance().SendMsg(admissionMsg);
        }
        else
        {
            UMessage enrollMsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_PlayerEnroll);

            enrollMsg.Add(GameMain.hall_.GetPlayerId());
            enrollMsg.Add(key);

            NetWorkClient.GetInstance().SendMsg(enrollMsg);
        }
        NumberPanel.GetInstance().SetNumberPanelActive(false);
    }

    void InitTicketsPickPanel(stContestData data)
    {
        LoadTicketResource(0, data.nEnrollMoneyNum, data.nEnrollRechargeCoin, data.nEnrollItemID, data.nEnrollItemNum, data.nContestID);
        if (data.nEnrollMoneyNum != 0 && data.nEnrollRechargeCoin != 0)
            LoadTicketResource(1, data.nEnrollMoneyNum, data.nEnrollRechargeCoin, data.nEnrollItemID, data.nEnrollItemNum, data.nContestID);

        ticketpanel_.SetActive(true);
    }

    void SetTicketType(GameObject item, stContestData data)
    {
        GameObject ticketbg = item.transform.Find("TicketBG").gameObject;

        if(data.nEnrollMoneyNum != 0)
        {
            GameObject tinfo = ticketbg.transform.Find("ImageTicketBG").gameObject;
            Text money = tinfo.transform.Find("TextMoney").gameObject.GetComponent<Text>();
            money.text = data.nEnrollMoneyNum.ToString() + "金币";
            tinfo.SetActive(true);

            Text itemTx = tinfo.transform.Find("ImageBG").Find("TextTicket").gameObject.GetComponent<Text>();
            if (data.nEnrollItemID != 0)
            {
                itemTx.text = data.nEnrollItemNum.ToString();
            } 
            else
            {
                tinfo.transform.Find("ImageBG").gameObject.SetActive(false);
                tinfo.transform.Find("Textbg").gameObject.SetActive(false);
            }
        }
        else
        {
            GameObject tinfo = ticketbg.transform.Find("ImageTicketBG").gameObject;
            tinfo.SetActive(false);
        }

        if(data.nEnrollRechargeCoin != 0)
        {
            GameObject tinfo = ticketbg.transform.Find("ImageTicketBG (1)").gameObject;
            Text money = tinfo.transform.Find("TextMoney").gameObject.GetComponent<Text>();
            money.text = data.nEnrollRechargeCoin.ToString() + "钻石";

            tinfo.transform.Find("Textbghuo").gameObject.SetActive(data.nEnrollMoneyNum != 0);
            tinfo.SetActive(true);

            Text itemTx = tinfo.transform.Find("ImageBG").Find("TextTicket").gameObject.GetComponent<Text>();
            if (data.nEnrollItemID != 0)
            {
                itemTx.text = data.nEnrollItemNum.ToString();
            }
            else
            {
                tinfo.transform.Find("ImageBG").gameObject.SetActive(false);
                tinfo.transform.Find("Textbg").gameObject.SetActive(false);
            }
        }
    }

    void SetTimeType(GameObject item, stContestData data)
    {
        switch (data.enContestType)
        {
            case ContestType.ContestType_Timing:
                System.DateTime sdt = GameCommon.ConvertLongToDateTime(data.tStartTimeSeconds);
                string timeTx = sdt.ToString("MM月dd日HH:mm");
                Text date = item.transform.Find("TimeBG").Find("ImageBG_1").
                    Find("Text_date").gameObject.GetComponent<Text>();
                date.text = sdt.ToString("MM月dd日");
                Text time = item.transform.Find("TimeBG").Find("ImageBG_1").
                    Find("Text_time").gameObject.GetComponent<Text>();
                time.text = sdt.ToString("HH:mm");
                break;
            //case Time_Enum.HasStop:
            //    Text hour = item.transform.FindChild("TimeBG").FindChild("ImageBG_2").FindChild("Countdownbg").
            //        FindChild("Text_Countdown_1").gameObject.GetComponent<Text>();
            //    hour.text = data.date;
            //    Text minute = item.transform.FindChild("TimeBG").FindChild("ImageBG_2").FindChild("Countdownbg").
            //        FindChild("Text_Countdown_2").gameObject.GetComponent<Text>();
            //    minute.text = data.time;
            //    break;
            case ContestType.ContestType_AnyTime:
                item.transform.Find("TimeBG").Find("ImageBG_1").gameObject.SetActive(false);
                item.transform.Find("TimeBG").Find("ImageBG_3").gameObject.SetActive(true);
                Text signNumber = item.transform.Find("TimeBG").Find("ImageBG_3").
                    Find("Text_Num").gameObject.GetComponent<Text>();
                signNumber.text = data.nMaxEnrollPlayerNum.ToString() + "人";
                break;
        }
    }

    void SetSignButtonUI(GameObject item, stContestData data)
    {
        GameObject BG1 = item.transform.Find("ApplyBG").gameObject;
        GameObject ticket = item.transform.Find("ImageTicket").gameObject;
        ticket.SetActive(false);
        for (int index = 1; index < BG1.transform.childCount; index++)
        {
            BG1.transform.GetChild(index).gameObject.SetActive(false);
            Text hassign = BG1.transform.Find("ImageBG").Find("Text_state").gameObject.GetComponent<Text>();
            if (data.contestState == CONTEST_STATE.CONTEST_STATE_EXHIBITION)
            {
                uint signtime = data.tStartTimeSeconds - (uint)data.nEnrollStartTime * 60;
                System.DateTime sdt = GameCommon.ConvertLongToDateTime(signtime);

                BG1.transform.GetChild(index).gameObject.SetActive(index == 5);
                hassign.text = sdt.ToString("yyyy年MM月dd日HH:mm") + " 开始报名";
                hassign.gameObject.SetActive(true);
            }
            else if (data.contestState == CONTEST_STATE.CONTEST_STATE_ENROLL)
            {
                if (GameMain.hall_.GetPlayerData().signedContests.Contains(data.nContestID))
                {
                    hassign.text = "已报名";
                    BG1.transform.GetChild(index).gameObject.SetActive(index == 5);
                    hassign.gameObject.SetActive(true);
                }
                else
                {
                    BG1.transform.GetChild(index).gameObject.SetActive(index == 1);
                    Transform ApplyTransform = item.transform.Find("ApplyBG/Button_Apply");
                    ApplyTransform.gameObject.SetActive(data.nEnrollRechargeCoin != 0);

                    ApplyTransform = item.transform.Find("ApplyBG/Button_mianfei");
                    ApplyTransform.gameObject.SetActive(data.nEnrollRechargeCoin == 0);

                    ticket.SetActive(data.nEnrollRechargeCoin != 0);
                }  
            }
            else if(data.contestState == CONTEST_STATE.CONTEST_STATE_ADMISSION)
            {
                if(contesttype_ == 1 || data.enContestType == ContestType.ContestType_Timing)
                {
                    if (GameMain.hall_.GetPlayerData().signedContests.Contains(data.nContestID))
                        BG1.transform.Find("Button_jinru").gameObject.SetActive(true);
                    else
                    {
                        BG1.transform.GetChild(index).gameObject.SetActive(index == 1);
                        ticket.SetActive(true);
                    }
                }
                else
                {
                    if (data.enContestType == ContestType.ContestType_AnyTime)
                    {
                        BG1.transform.GetChild(index).gameObject.SetActive(index == 1);

                        Transform ApplyTransform = item.transform.Find("ApplyBG/Button_Apply");
                        ApplyTransform.gameObject.SetActive(data.nEnrollRechargeCoin != 0);

                        ApplyTransform = item.transform.Find("ApplyBG/Button_mianfei");
                        ApplyTransform.gameObject.SetActive(data.nEnrollRechargeCoin == 0);

                        ticket.SetActive(data.nEnrollRechargeCoin != 0);
                    }
                }
            }
        }
            
        //if (matchinfo_ != null)
        //{
        //    SetInfoSignButtonUI(data);
        //}
    }

    void SetInfoSignButtonUI(stContestData data)
    {
        GameObject BG2 = matchinfo_.transform.Find("ImageBG").Find("InfoBG").Find("Image_gaikuang").Find("ImageBG").gameObject;
        BG2.transform.gameObject.SetActive(true);
        for (int index = 0; index < BG2.transform.childCount; index++)
        {
            BG2.transform.GetChild(index).gameObject.SetActive(false);
            Text hassign = BG2.transform.Find("Button_yibaoming").Find("Text").gameObject.GetComponent<Text>();
            if (data.contestState == CONTEST_STATE.CONTEST_STATE_EXHIBITION)
            {
                uint signtime = data.tStartTimeSeconds - (uint)data.nEnrollStartTime * 60;
                System.DateTime sdt = GameCommon.ConvertLongToDateTime(signtime);

                hassign.text = sdt.ToString("yyyy年MM月dd日HH:mm") + " 开始报名";
                hassign.gameObject.SetActive(true);
                BG2.transform.Find("Button_yibaoming").gameObject.SetActive(true);
            }
            else if (data.contestState == CONTEST_STATE.CONTEST_STATE_ENROLL)
            {
                if (GameMain.hall_.GetPlayerData().signedContests.Contains(data.nContestID))
                {
                    GameObject tuisai = BG2.transform.Find("Button_tuisai").gameObject;
                    //if (GameMain.hall_.GetPlayerData().signedContests.Contains(data.nContestID))
                    BG2.transform.Find("Button_yibaoming").gameObject.SetActive(false);
                    tuisai.SetActive(true);
                }
                else
                    BG2.transform.Find("Button_baoming").gameObject.SetActive(true);
            }
            else if (data.contestState == CONTEST_STATE.CONTEST_STATE_ADMISSION)
            {
                if (GameMain.hall_.GetPlayerData().signedContests.Contains(data.nContestID))
                {
                    Transform jinRuTransfrom = BG2.transform.Find("Button_jinru");
                    if(contestobjects_.ContainsKey(data.nContestID))
                    {
                        bool bInteractable = contestobjects_[data.nContestID].transform.Find("ApplyBG/Button_jinru").GetComponent<Button>().interactable;
                        jinRuTransfrom.GetComponent<Button>().interactable = bInteractable;
                    }
                    jinRuTransfrom.gameObject.SetActive(true);
                }
                else
                    BG2.transform.Find("Button_baoming").gameObject.SetActive(true);
            }
                
        }

        GameObject breakBtn = matchinfo_.transform.Find("ImageBG/InfoBG/Image_gaikuang/ImageBG/Button_jiesan").gameObject;
        breakBtn.SetActive(data.createid == GameMain.hall_.GetPlayerId());
    }

    private void OnSignContest(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            //发送报名信息
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            stContestData data = (stContestData)button;
            //contestdatas_[key].state = ContestState.HasSign;
            //SetSignButtonUI(contestobjects_[key], contestdatas_[key]);

            InitTicketsPickPanel(data);
        }

    }

    void InitContestData()
    {
        //Text money = root_.transform.FindChild("Top").FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();
        //money.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();

        Text diamond = root_.transform.Find("Top").Find("Image_DiamondFrame").Find("Text_Diamond").gameObject.GetComponent<Text>();
        diamond.text = (GameMain.hall_.GetPlayerData().GetDiamond() + GameMain.hall_.GetPlayerData().GetCoin()).ToString();

        GameObject diamondBtn = root_.transform.Find("Top").Find("Image_DiamondFrame").gameObject;
        XPointEvent.AutoAddListener(diamondBtn, GameMain.hall_.Charge, Shop.SHOPTYPE.SHOPTYPE_DIAMOND);
    }

    private void OnReturn2Hall(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (GameMain.hall_.contestui_ != null)
                GameMain.hall_.contestui_.SetActive(true);

            root_.SetActive(false);
        }
    }

    public void ShowContestPanel()
    {
        if (GameMain.hall_.contestui_ != null)            GameMain.hall_.contestui_.SetActive(false);

        root_.SetActive(true);
    }

    void TipsPanel(bool isShow)
    {
        GameObject tips = root_.transform.Find("Pop-up").Find("Tips").gameObject;
        tips.SetActive(isShow);
    }

    //void RefreshTips(stContestData data)
    //{
    //    GameObject tips = root_.transform.FindChild("Pop-up").FindChild("Tips").gameObject;
    //    Text gamename = tips.transform.FindChild("ImageBG").FindChild("Text").gameObject.GetComponent<Text>();
    //    gamename.text = "您报名的" + data.sContestName + "即将开始，是否现在进入比赛？";

    //    GameObject tipsBtn = root_.transform.FindChild("Pop-up").FindChild("Tips").FindChild("ImageBG").FindChild("Buttonstart").gameObject;
    //    XPointEvent.AutoAddListener(tipsBtn, OnGoinContest, data.nContestID);
    //}

    void LoadTipsBackGroundResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (contestTips_ != null)
            return;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Match_Begintips");
        contestTips_ = (GameObject)GameMain.instantiate(obj0);
        contestTips_.transform.SetParent(GameObject.Find("Canvas/Root").transform, false);
        contestTips_.SetActive(false);

        GameObject showtipsbtn = contestTips_.transform.Find("Button_Begintips").gameObject;
        XPointEvent.AutoAddListener(showtipsbtn, OnShowContestTips, null);

        GameObject closetipsbtn = contestTips_.transform.Find("TipsBG/UiRootBG_Button").gameObject;
        XPointEvent.AutoAddListener(closetipsbtn, OnCloseContestTips, null);
    }

    private void OnCloseContestTips(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            contestTips_.transform.Find("TipsBG").gameObject.SetActive(false);
        }
    }

    private void OnShowContestTips(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            contestTips_.transform.Find("TipsBG").gameObject.SetActive(true);
        }
    }

    Dictionary<uint, GameObject> tipsobjects_;
    void LoadTipsResource(stContestData data)
    {
        GameObject BG = contestTips_.transform.Find("TipsBG").Find("Viewport").Find("Content").gameObject;
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("MatchBegin_tips");
        GameObject tips = (GameObject)GameMain.instantiate(obj0);
        tips.transform.SetParent(BG.transform, false);

        Text gamename = tips.transform.Find("Text").gameObject.GetComponent<Text>();
        gamename.text = "您报名的" + data.sContestName + "即将开始，是否现在进入比赛？";

        GameObject tipsBtn = tips.transform.Find("Buttonstart").gameObject;
        XPointEvent.AutoAddListener(tipsBtn, OnGoinContest, data.nContestID);

        contestTips_.SetActive(true);
        contestTips_.transform.Find("Button_Begintips").gameObject.SetActive(true);
        GameObject closetipsbtn = contestTips_.transform.Find("TipsBG").gameObject;
        closetipsbtn.SetActive(true);

        if (!tipsobjects_.ContainsKey(data.nContestID))
            tipsobjects_.Add(data.nContestID, tips);
    }

    //float tipstime = 15.0f;
    //bool istips = false;
    //int gameindex = 0;
    //void UpdateTips(float deltatime)
    //{
    //    if (!istips)
    //        return;

    //    tipstime -= deltatime;
    //    if (tipstime > 0.0f)
    //        RefreshTips((int)tipstime);
    //    else
    //        TipsPanel(false);
    //}

    bool isDownload_;
    public void Update ()
    {
        float deltatime = Time.deltaTime;
        //UpdateTips(deltatime);
        UpdateContestList(deltatime);

        if(isDownload_)
        {
            uint percent = DownLoadProcessMgr.Instance.GetDownloadPercent();
            if (waitingTips_ != null)
            {
                Text percentTx = waitingTips_.transform.Find("Text").gameObject.GetComponent<Text>();
                percentTx.text = percent.ToString() + "%";
            }
            if (CGameResDownloadMgr.Instance.isAllGameResHaveDownOver())
            {
                if (waitingTips_ != null)
                    waitingTips_.SetActive(false);

                isDownload_ = false;
            }
        }
    }

    public void RefreshMoney()
    {
        if (root_ == null)
            return;

        Text diamond = root_.transform.Find("Top").Find("Image_DiamondFrame").Find("Text_Diamond").gameObject.GetComponent<Text>();
        diamond.text = (GameMain.hall_.GetPlayerData().GetDiamond() + GameMain.hall_.GetPlayerData().GetCoin()).ToString();
    }

    bool isRefreshContestList;
    float currentRefreshTime = 0.0f;
    float  refreshTime = 10.0f;
    void UpdateContestList(float deltatime)
    {
        if (root_ == null)
            return;

        if (!root_.activeSelf)
            return;

        currentRefreshTime += deltatime;
        if (currentRefreshTime >= refreshTime)
        {
            currentRefreshTime = 0.0f;
            isRefreshContestList = true;
        }
        else
            isRefreshContestList = false;

        if (!isRefreshContestList)
            return;

        UMessage ask4ContestDataMsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_RequestContestInfo);
        ask4ContestDataMsg.Add(contesttype_);
        NetWorkClient.GetInstance().SendMsg(ask4ContestDataMsg);
        bContestInfoListRefreshState = false;
    }

    /// <summary>
    /// 重置比赛信息请求时间
    /// 原因:自建比赛创建成功界面显示成功,立刻请求比赛信息消息会导致已经创建成功界面关闭。
    /// </summary>
    public void ResetRefreshContestTime()
    {
        currentRefreshTime = 0;
    }

    public void SetCurrentContest(uint contestId, uint contestDataId)
    {
        Debug.Log("]]]]]]]]]]]]]]]]]contestid:" + contestId + " dataId:" + contestDataId);

        stContestData srcData = ContestDataManager.Instance().GetContestDataByDataId(contestDataId);
        if (srcData == null)
        {
            Debug.Log("Can't find contest data!! id:" + contestDataId);
            return;
        }

        ContestDataManager.Instance().currentContestID = contestId;
        Dictionary<uint, stContestData> cd;
        if (srcData.enOrganisersType == ContestOrganisersType.ContestOrganisersType_Official)
        {
            contesttype_ = 1;
            cd = ContestDataManager.Instance().contestdatas_;
        }
        else
        {
            contesttype_ = 2;
            cd = ContestDataManager.Instance().selfcontestdatas_;
        }

        if (!cd.ContainsKey(contestId))
        {
            stContestData data = new stContestData();
            data.nContestID = contestId;
            data.nContestDataID = contestDataId;
            data.tStartTimeSeconds = 0;
            data.nGamePlayerNum = 0;
            data.sContestName = srcData.sContestName;
            data.byGameID = srcData.byGameID;
            data.nRewardDataID = srcData.nRewardDataID;
            data.enContestType = srcData.enContestType;
            data.enContestKind = srcData.enContestKind;
            data.enOrganisersType = srcData.enOrganisersType;
            data.enContestOpenCycle = srcData.enContestOpenCycle;
            data.vecContestDate = srcData.vecContestDate;
            data.vecAdmissionHour = srcData.vecAdmissionHour;
            data.vecContestHour = srcData.vecContestHour;
            data.nExhibitionTime = srcData.nExhibitionTime;
            data.nEnrollStartTime = srcData.nEnrollStartTime;
            data.nMaxEnrollPlayerNum = srcData.nMaxEnrollPlayerNum;
            data.nMinEnrollPlayerNum = srcData.nMinEnrollPlayerNum;
            data.nEnrollReputationMiniNum = srcData.nEnrollReputationMiniNum;
            data.nEnrollMasterMiniNum = srcData.nEnrollMasterMiniNum;
            data.nEnrollNamelistID = srcData.nEnrollNamelistID;
            data.nEnrollItemID = srcData.nEnrollItemID;
            data.nEnrollItemNum = srcData.nEnrollItemNum;
            data.nEnrollRechargeCoin = srcData.nEnrollRechargeCoin;
            data.nEnrollMoneyNum = srcData.nEnrollMoneyNum;
            data.shPreliminaryRuleID = srcData.shPreliminaryRuleID;
            data.shFinalsRuleID = srcData.shFinalsRuleID;
            data.nContestBeginBroadcastID = srcData.nContestBeginBroadcastID;
            data.nChampionBroadcastID = srcData.nChampionBroadcastID;
            data.nRewardDataID = srcData.nRewardDataID;
            data.nContestQualificationBuyID = srcData.nContestQualificationBuyID;
            data.iconname = srcData.iconname;
            data.nContestRule = srcData.nContestRule;
            data.nContestRuleDetail = srcData.nContestRuleDetail;
            data.playmode = srcData.playmode;
            data.playmodeicon = srcData.playmodeicon;
            data.ticketIcon = srcData.ticketIcon;

            cd.Add(data.nContestID, data);
        }
    }
}
