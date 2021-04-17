using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using USocket.Messages;

/// <summary>
/// 自建赛类型
/// </summary>
public enum ESlefContestType
{
    PlayerNumberContest,//满人塞
    TimeContest,        //定时赛
    ContestTypeCount
}

public class SelfContest
{
    GameObject createPanel_;
    GameObject root_;
    GameObject rulepanel1_;
    GameObject rulepanel2_;
    byte playtimesindex_;
    SelfContestRule scr_;
    uint hourcall_;
    uint minutecall_;
    Dictionary<uint, GameObject> contestobjects_;
    Dictionary<string, Sprite> contestImages_;
    stContestData detail_;
    GameObject matchinfo_;
    Dictionary<uint, GameObject> tipsobjects_;
    public uint currentPassWord_;
    public uint selfcreateNumber_;

    /// <summary>
    /// 当前比赛游戏索引
    /// </summary>
    int CurContestGameIndex = 0;
    /// <summary>
    /// 定时赛
    /// </summary>
    GameKind_Enum[] TimeMatchGameIdData = { GameKind_Enum.GameKind_LandLords, 
                                            GameKind_Enum.GameKind_GuanDan, 
                                            GameKind_Enum.GameKind_GuanDan,
                                            GameKind_Enum.GameKind_Chess};
    /// <summary>
    /// 满人赛
    /// </summary>
    GameKind_Enum[] PlayerNumberMatchGameIdData = { GameKind_Enum.GameKind_LandLords,
                                                    GameKind_Enum.GameKind_GuanDan,
                                                    GameKind_Enum.GameKind_GuanDan};

    /// <summary>
    /// 当前赛事数据
    /// </summary>
    Dictionary<int, SelfContestRule> CurMatchDataDictionary;
    public SelfContest()
    {
        InitSelfContest();
        //LoadRulePanel();
        InitSelfContestMsg();
        scr_ = new SelfContestRule();
        hourcall_ = 1;
        minutecall_ = 0;
        selfcreateNumber_ = 0;
        contestobjects_ = new Dictionary<uint, GameObject>();
        tipsobjects_ = new Dictionary<uint, GameObject>();
        contestImages_ = new Dictionary<string, Sprite>();
        CurMatchDataDictionary = new Dictionary<int, SelfContestRule>();
        for(var TypeIndex = ESlefContestType.PlayerNumberContest;
            TypeIndex < ESlefContestType.ContestTypeCount; ++TypeIndex)
        {
            InitMatchData(TypeIndex);
        }
    }

    public void InitMatchData(ESlefContestType ContestType)
    {
        GameKind_Enum[] GameKindArray = null;
        switch(ContestType)
        {
            case ESlefContestType.TimeContest:
                GameKindArray = TimeMatchGameIdData;
                break;
            case ESlefContestType.PlayerNumberContest:
                GameKindArray = PlayerNumberMatchGameIdData;
                break;
        }
        int GameKey = 0;
        for(int index = 0; index < GameKindArray.Length; ++index)
        {
            GameKey = (int)ContestType * 10000 + (int)GameKindArray[index];
            if (!CurMatchDataDictionary.ContainsKey(GameKey))
            {
                SelfContestRule contestData = new SelfContestRule();
                CurMatchDataDictionary.Add(GameKey, contestData);
            }
            CurMatchDataDictionary[GameKey].playernumber = 0;
            switch (GameKindArray[index])
            {
                case GameKind_Enum.GameKind_LandLords:
                    {
                        if(ContestType == ESlefContestType.PlayerNumberContest)
                        {
                            CurMatchDataDictionary[GameKey].playernumber = SelfContestRule.llrplayerNumberList[0];
                        }
                        CurMatchDataDictionary[GameKey].precontest = SelfContestRule.llrprecontestList[0];
                        CurMatchDataDictionary[GameKey].finalcontest = SelfContestRule.llrprecontestList[0];
                    }
                    break;
                case GameKind_Enum.GameKind_GuanDan:
                    {
                        if (ContestType == ESlefContestType.PlayerNumberContest)
                        {
                            CurMatchDataDictionary[GameKey].playernumber = SelfContestRule.gdplayerNumberList[0];
                        }
                        CurMatchDataDictionary[GameKey].precontest = SelfContestRule.gdprecontestList[0];
                        CurMatchDataDictionary[GameKey].finalcontest = SelfContestRule.gdprecontestList[0];
                    }
                    break;
                case GameKind_Enum.GameKind_Chess:
                    {
                        CurMatchDataDictionary[GameKey].precontest = SelfContestRule.gdprecontestChessList[0];
                        CurMatchDataDictionary[GameKey].finalcontest = SelfContestRule.gdprecontestChessList[0];
                    }
                    break;
            }
            CurMatchDataDictionary[GameKey].cost = SelfContestRule.costList[0];
        }
    }

    void InitSelfContestMsg()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Contestmsg_PlayerCreateContestReply, BackSelfCreateContest);       //创建比赛回复
    }

    bool BackSelfCreateContest(uint msgType, UMessage msg)
    {
        stContestData data = new stContestData();

        byte state = msg.ReadByte();
        if(state == 0)
        {
            CCustomDialog.OpenCustomConfirmUI(1106);
            return false;
        }
        else
            CRollTextUI.Instance.AddVerticalRollText(1202);

        data.createid = msg.ReadUInt();

        data.nContestID = msg.ReadUInt();
        data.nContestDataID = msg.ReadUInt();
        data.sContestName = ContestDataManager.Instance().contestData[data.nContestDataID].sContestName;
        data.byGameID = ContestDataManager.Instance().contestData[data.nContestDataID].byGameID;
        data.nGamePlayerNum = msg.ReaduShort();
        data.tStartTimeSeconds = msg.ReadUInt();
        ContestDataManager.Instance().contestData[data.nContestDataID].tStartTimeSeconds = data.tStartTimeSeconds;
        data.contestState = (CONTEST_STATE)(msg.ReadByte());
        ushort temp = 0;
        temp = msg.ReaduShort();
        data.nMaxEnrollPlayerNum = temp;
        data.nEnrollRechargeCoin = msg.ReaduShort();
        data.createName = GameMain.hall_.GetPlayerData().GetPlayerName();


        data.nRewardDataID = ContestDataManager.Instance().contestData[data.nContestDataID].nRewardDataID;
        data.enContestType = ContestDataManager.Instance().contestData[data.nContestDataID].enContestType;
        if (data.enContestType == ContestType.ContestType_AnyTime)
            data.contestState = CONTEST_STATE.CONTEST_STATE_ENROLL;
        data.enContestKind = ContestDataManager.Instance().contestData[data.nContestDataID].enContestKind;
        data.enOrganisersType = ContestDataManager.Instance().contestData[data.nContestDataID].enOrganisersType;
        data.enContestOpenCycle = ContestDataManager.Instance().contestData[data.nContestDataID].enContestOpenCycle;
        data.vecContestDate = ContestDataManager.Instance().contestData[data.nContestDataID].vecContestDate;
        data.vecAdmissionHour = ContestDataManager.Instance().contestData[data.nContestDataID].vecAdmissionHour;
        data.vecContestHour = ContestDataManager.Instance().contestData[data.nContestDataID].vecContestHour;
        data.nExhibitionTime = ContestDataManager.Instance().contestData[data.nContestDataID].nExhibitionTime;
        data.nEnrollStartTime = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollStartTime;
        //data.nMaxEnrollPlayerNum = ContestDataManager.Instance().contestData[data.nContestDataID].nMaxEnrollPlayerNum;
        data.nMinEnrollPlayerNum = ContestDataManager.Instance().contestData[data.nContestDataID].nMinEnrollPlayerNum;
        data.nEnrollReputationMiniNum = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollReputationMiniNum;
        data.nEnrollMasterMiniNum = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollMasterMiniNum;
        data.nEnrollNamelistID = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollNamelistID;
        data.nEnrollItemID = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollItemID;
        data.nEnrollItemNum = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollItemNum;
        //data.nEnrollRechargeCoin = 0;// ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollRechargeCoin;
        data.nEnrollMoneyNum = ContestDataManager.Instance().contestData[data.nContestDataID].nEnrollMoneyNum;
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
        data.resetDetail = ContestDataManager.Instance().contestData[data.nContestDataID].resetDetail;

        ContestDataManager.Instance().selfcontestdatas_.Add(data.nContestID, data);
        GameMain.hall_.contest.onClickContestType();
        GameMain.hall_.contest.ResetRefreshContestTime();
        selfcreateNumber_ += 1;

        if(rulepanel1_ != null)
            rulepanel1_.SetActive(false);
        if(rulepanel2_ != null)
            rulepanel2_.SetActive(false);

        return true;
    }

    public void Ask4SelfContestList()
    {
        UMessage ask4ContestDataMsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_RequestContestInfo);

        ask4ContestDataMsg.Add((byte)2);

        NetWorkClient.GetInstance().SendMsg(ask4ContestDataMsg);
        if (GameMain.hall_.contest != null)
        {
            GameMain.hall_.contest.UpdateContestButtonState(2);
            GameMain.hall_.contest.ResetRefreshContestTime();
        }
    }

    public void InitSelfContest()
    {
        if (root_ == null)
        {
            if (GameMain.hall_.contest == null)
                GameMain.hall_.contest = new Contest();

            if (GameMain.hall_.contest.root_ == null)
                GameMain.hall_.contest.LoadContestResource();
            root_ = GameMain.hall_.contest.root_;
        }
        root_.SetActive(true);
        if(GameMain.hall_.contestui_ != null)
            GameMain.hall_.contestui_.SetActive(false);

        InitSelfContestDE();
    }

    void InitSelfContestDE()
    {
        root_.transform.Find("Top/CreateGame_BG").gameObject.SetActive(true);

        GameObject returnBtn = root_.transform.Find("Top/ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(returnBtn, OnBack2Hall, null);

        GameObject createBtn = root_.transform.Find("Top/CreateGame_BG/Button_Create").gameObject;
        XPointEvent.AutoAddListener(createBtn, OnOpenCreatePanel, null);

        GameObject joinBtn = root_.transform.Find("Top/CreateGame_BG/Button_Join").gameObject;
        XPointEvent.AutoAddListener(joinBtn, OnJoin, null);
    }

    private void OnJoin(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            NumberPanel.GetInstance().SetNumberPanelActive(true, SendJoinMsg);
        }
    }

    /// <summary>
    /// 检测是否可以发送加入比赛消息
    /// </summary>
    /// <param name="contestid"></param>
    /// <param name="tipShowState"></param>
    /// <returns></returns>
    public bool CheckSendJoinMsg(uint contestid,bool tipShowState = true)
    {
        if (currentPassWord_ != 0)
        {
            if (contestid != currentPassWord_)
            {
                if(tipShowState)
                {
                    CCustomDialog.OpenCustomConfirmUI(1302);
                }
                return false;
            }
        }

        if(GameMain.hall_.contest != null)
        {
            if (GameMain.hall_.contest.CheckContestInfoListRefreshState() &&
                ContestDataManager.Instance().selfcontestdatas_.Count == 0)
            {
                if (!tipShowState)
                {
                    return true;
                }
                CCustomDialog.OpenCustomConfirmUI(1610);
                return false;
            }
        }

        if (!ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestid))
        {
            if (!tipShowState)
            {
                return true;
            }
            CCustomDialog.OpenCustomConfirmUI(1610);
            return false;
        }
        return true;
    }

    //发送加入比赛消息
    public void SendJoinMsg(uint contestid)    {        NumberPanel.GetInstance().SetNumberPanelActive(false);

        bool CheckSendState = CheckSendJoinMsg(contestid);
        if(!CheckSendState)
        {
            return;
        }

        if (ContestDataManager.Instance().selfcontestdatas_[contestid].enContestType == ContestType.ContestType_AnyTime)
        {
            UMessage admissionMsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_PlayerRequestAdmission);

            admissionMsg.Add(GameMain.hall_.GetPlayerId());
            admissionMsg.Add(contestid);

            NetWorkClient.GetInstance().SendMsg(admissionMsg);
        }
        else
        {
            UMessage joinmsg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_PlayerEnroll);
            joinmsg.Add(GameMain.hall_.GetPlayerId());
            joinmsg.Add(contestid);
            NetWorkClient.GetInstance().SendMsg(joinmsg);
        }    }

    void LoadCreatePanel()
    {
        if (createPanel_ != null)
        {
            createPanel_.SetActive(true);
            InitCreateData();
            return;
        }

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("CreateGame_choose");
        createPanel_ = (GameObject)GameMain.instantiate(obj0);

        GameObject CanvasObj = GameObject.Find("Canvas/Root");
        createPanel_.transform.SetParent(CanvasObj.transform, false);
        createPanel_.SetActive(true);

        InitCreateData();
        InitCreateEvents();
    }

    void InitCreateData()
    {
        Text diamond = createPanel_.transform.Find("Top/Image_DiamondFrame/Text_Diamond").gameObject.GetComponent<Text>();
        diamond.text = (GameMain.hall_.GetPlayerData().GetDiamond() + GameMain.hall_.GetPlayerData().GetCoin()).ToString();
    }

    void InitCreateEvents()
    {
        GameObject returnbtn = createPanel_.transform.Find("Top/ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(returnbtn, OnCloseCreatePanel, null);

        GameObject diamondBtn = createPanel_.transform.Find("Top").Find("Image_DiamondFrame").gameObject;        XPointEvent.AutoAddListener(diamondBtn, GameMain.hall_.Charge, Shop.SHOPTYPE.SHOPTYPE_DIAMOND);

        GameObject fullstart = createPanel_.transform.Find("middle/Button_1").gameObject;
        XPointEvent.AutoAddListener(fullstart, OnPickContestType, ESlefContestType.PlayerNumberContest);

        GameObject timestart = createPanel_.transform.Find("middle/Button_2").gameObject;
        XPointEvent.AutoAddListener(timestart, OnPickContestType, ESlefContestType.TimeContest);
    }

    private void OnCloseCreatePanel(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            createPanel_.SetActive(false);
        }
    }

    private void OnOpenCreatePanel(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            LoadCreatePanel();
        }
    }

    private void OnPickContestType(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            ESlefContestType createtype = (ESlefContestType)button;

            LoadRulePanel(createtype);

            if(rulepanel1_ != null)
                rulepanel1_.SetActive(false);
            if(rulepanel2_ != null)
                rulepanel2_.SetActive(false);

            createPanel_.SetActive(false);

            int SelfGameDataIndex = ((int)createtype) * 3 + CurContestGameIndex;
            SelfContestDataManager.instance().SetGameDataID(SelfGameDataIndex);
            switch(createtype)
            {
                case ESlefContestType.TimeContest:
                    {
                        rulepanel2_.SetActive(true);
                        GameObject BG = rulepanel2_.transform.Find("Right").gameObject;
                        for (int index = 0; index < 3; index++)
                        {
                            Text texttips = BG.transform.GetChild(index).Find("shijian/Text_tips").gameObject.GetComponent<Text>();
                            RefreshContestTimeTipsText(texttips);
                        }
                    }
                    break;
                case ESlefContestType.PlayerNumberContest:
                    {
                        rulepanel1_.SetActive(true);
                    }
                    break;
            }

            UnityEngine.Transform groupTransform = rulepanel1_ ? rulepanel1_.transform : rulepanel2_.transform;
            groupTransform.Find("Right/ImageBG/Button_chuangjian").GetComponent<Button>().interactable = true;
            groupTransform.Find("Right/ImageBG/Button_chuangjian_0").GetComponent<Button>().interactable = true;
            RefreshContestTime(groupTransform);
        }
    }

    void LoadRulePanel(ESlefContestType createtype)
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;
        GameObject CanvasObj = GameObject.Find("Canvas/Root");

        switch(createtype)
        {
            case ESlefContestType.TimeContest:
                {
                    if (rulepanel2_ == null)
                    {
                        UnityEngine.Object obj1 = (GameObject)bundle.LoadAsset("CreateGame_rule_2");
                        rulepanel2_ = (GameObject)GameMain.instantiate(obj1);

                        rulepanel2_.transform.SetParent(CanvasObj.transform, false);
                        rulepanel2_.SetActive(false);
                        InitRulePanel(createtype);
                        GameMain.safeDeleteObj(rulepanel1_);
                    }
                }
                break;
            case ESlefContestType.PlayerNumberContest:
                {
                    if (rulepanel1_ == null)
                    {
                        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("CreateGame_rule_1");
                        rulepanel1_ = (GameObject)GameMain.instantiate(obj0);
                        rulepanel1_.transform.SetParent(CanvasObj.transform, false);
                        rulepanel1_.SetActive(false);
                        InitRulePanel(createtype);
                        GameMain.safeDeleteObj(rulepanel2_);
                    }
                }
                break;
        }
    }

    private void OnBack2SelfContest(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (rulepanel1_ != null)
                rulepanel1_.SetActive(false);
            if(rulepanel2_ != null)
                rulepanel2_.SetActive(false);

            createPanel_.SetActive(true);
            InitCreateData();
        }    }

    void InitRulePanel(ESlefContestType createtype)
    {
        switch (createtype)
        {
            case ESlefContestType.PlayerNumberContest:
                {
                    GameObject returnBtn1 = rulepanel1_.transform.Find("Top/ButtonReturn").gameObject;
                    XPointEvent.AutoAddListener(returnBtn1, OnBack2SelfContest, null);

                    Text diamond1 = rulepanel1_.transform.Find("Top/Image_DiamondFrame/Text_Diamond").gameObject.GetComponent<Text>();
                    diamond1.text = (GameMain.hall_.GetPlayerData().GetDiamond() + GameMain.hall_.GetPlayerData().GetCoin()).ToString();

                    GameObject togglebg1 = rulepanel1_.transform.Find("Left").gameObject;
                    for (int index = 0; index < togglebg1.transform.childCount; index++)
                    {
                        Toggle toggle = togglebg1.transform.GetChild(index).gameObject.GetComponent<Toggle>();
                        int temp = index;
                        toggle.onValueChanged.AddListener(delegate (bool value) {
                            CustomAudio.GetInstance().PlayCustomAudio(1003);
                            ChangeRuleInfo(temp, value, createtype);
                        });
                    }
                    InitRightRuleEvents(rulepanel1_, createtype);
                }
                break;
            case ESlefContestType.TimeContest:
                {
                    GameObject returnBtn2 = rulepanel2_.transform.Find("Top/ButtonReturn").gameObject;
                    XPointEvent.AutoAddListener(returnBtn2, OnBack2SelfContest, null);

                    Text diamond2 = rulepanel2_.transform.Find("Top/Image_DiamondFrame/Text_Diamond").gameObject.GetComponent<Text>();
                    diamond2.text = (GameMain.hall_.GetPlayerData().GetDiamond() + GameMain.hall_.GetPlayerData().GetCoin()).ToString();

                    GameObject togglebg2 = rulepanel2_.transform.Find("Left").gameObject;
                    for (int index = 0; index < togglebg2.transform.childCount; index++)
                    {
                        Toggle toggle = togglebg2.transform.GetChild(index).gameObject.GetComponent<Toggle>();
                        int temp = index;
                        toggle.onValueChanged.AddListener(delegate (bool value) {
                            CustomAudio.GetInstance().PlayCustomAudio(1003);
                            ChangeRuleInfo(temp, value, createtype);
                        });
                    }

                    InitRightRuleEvents(rulepanel2_, createtype);
                }
                break;
        }
        InitMatchData(createtype);
        SelfContestDataManager.instance().gameid = (byte)GameKind_Enum.GameKind_LandLords;
        RefreshPayInfo(1, createtype);
        RefreshPlayerNumber(SelfContestDataManager.instance().gameid, createtype);
        CurContestGameIndex = 0;
        SelfContestDataManager.instance().SetGameDataID((int)createtype * 3);
    }

    void InitRightRuleEvents(GameObject rulepanel, ESlefContestType createtype)
    {
        UnityEngine.Transform rightTransform = rulepanel.transform.Find("Right");

        Button createBtn = rightTransform.Find("ImageBG/Button_chuangjian").GetComponent<Button>();        createBtn.onClick.AddListener(() => { OnCreateContest(); });        Button createBtn0 = rightTransform.Find("ImageBG/Button_chuangjian_0").GetComponent<Button>();        createBtn0.onClick.AddListener(() => { OnCreateContest(); });

        if (createtype == ESlefContestType.TimeContest)
        {
            GameObject closetimesetbtn = rulepanel.transform.Find("Pop-up/shijian_set/UiRootBG").gameObject;
            XPointEvent.AutoAddListener(closetimesetbtn, OnCloseTimeSet, rulepanel);
            RefreshContestTime(rulepanel.transform);
        }

        InitRuleEvent(rightTransform.Find("rule_7"), createtype,GameKind_Enum.GameKind_LandLords);
        InitRuleEvent(rightTransform.Find("rule_13"), createtype,GameKind_Enum.GameKind_GuanDan);
        InitRuleEvent(rightTransform.Find("rule_13_1"), createtype, GameKind_Enum.GameKind_GuanDan);
        InitRuleEvent(rightTransform.Find("rule_20"), createtype, GameKind_Enum.GameKind_Chess);
    }

    private void OnOpenTimeSet(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);
            Transform rulebg = (Transform)button;

            GameObject timesetpanel = rulebg.parent.parent.Find("Pop-up/shijian_set").gameObject;
            timesetpanel.SetActive(true);
        }    }

    private void OnCloseTimeSet(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);

            GameObject rulepanel = (GameObject)button;

            GameObject timesetpanel = rulepanel.transform.Find("Pop-up/shijian_set").gameObject;
            timesetpanel.SetActive(false);
            RefreshContestTime(rulepanel.transform,DateControl._hour, DateControl._minute);
        }    }
    
    void RefreshContestTime(Transform rulePanelTransform,int HourValue = -1,int MinuteValue = -1)
    {
        if (HourValue >= 0 && MinuteValue >= 0)
        {
            hourcall_ = (uint)HourValue;
            minutecall_ = (uint)MinuteValue;
        }
        else
        {
            hourcall_ = (uint)System.DateTime.Now.Hour;
            minutecall_ = (uint)System.DateTime.Now.Minute + 3;
            if (minutecall_ >= 60)
            {
                hourcall_ += 1;
                minutecall_ -= 60;
            }
        }

        scr_.timeseconds = (uint)GameCommon.ConvertDataTimeToLong(System.DateTime.Now.Date.AddSeconds(hourcall_ * 3600 + minutecall_ * 60));

        Transform ChildTransform = null;
        Transform right = rulePanelTransform.Find("Right");
        for (int index = 0; index < right.childCount; ++index)
        {
            ChildTransform = right.GetChild(index);
            if (!ChildTransform.gameObject.activeSelf)
            {
                continue;
            }
            ChildTransform = ChildTransform.Find("shijian");
            if (ChildTransform == null)
            {
                continue;
            }
            Text texttips = ChildTransform.Find("Text_tips").GetComponent<Text>();
            RefreshContestTimeTipsText(texttips);
            Text hourtx = ChildTransform.Find("Dropdown_h/Label").GetComponent<Text>();
            hourtx.text = string.Format("{0:00}", hourcall_);
            Text minutetx = ChildTransform.Find("Dropdown_m/Label").GetComponent<Text>();
            minutetx.text = string.Format("{0:00}",minutecall_);
        }
    }


    void RefreshContestTimeTipsText(Text texttips)
    {
        long timeseconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);
        if (scr_.timeseconds <= timeseconds)
        {
            scr_.timeseconds = (uint)GameCommon.ConvertDataTimeToLong(System.DateTime.Now.AddDays(1).Date.AddSeconds(hourcall_ * 3600 + minutecall_ * 60));
            texttips.text = string.Format("比赛开始时间为明天的{0:00}时{1:00}分", hourcall_, minutecall_);
        }
        else
        {
            texttips.text = string.Format("比赛开始时间为今天的{0:00}时{1:00}分", hourcall_, minutecall_);
        }
    }

    void InitRuleEvent(Transform rulebg, ESlefContestType createtype,GameKind_Enum gameKind)
    {
        if(rulebg == null)
        {
            return;
        }
        switch(createtype)
        {
            case ESlefContestType.TimeContest:
                {
                    GameObject timebtn = rulebg.Find("shijian/Button_TimeSet").gameObject;
                    XPointEvent.AutoAddListener(timebtn, OnOpenTimeSet, rulebg);
                }
                break;
            case ESlefContestType.PlayerNumberContest:
                {
                    GameObject llfirstGroup = rulebg.Find("renshu").Find("ImageBG").gameObject;
                    for (int index = 0; index < llfirstGroup.transform.childCount; index++)
                    {
                        Toggle toggle = llfirstGroup.transform.GetChild(index).gameObject.GetComponent<Toggle>();
                        int temp = index;
                        toggle.onValueChanged.AddListener(delegate (bool value)
                        {
                            CustomAudio.GetInstance().PlayCustomAudio(1004);
                            switch (gameKind)
                            {
                                case GameKind_Enum.GameKind_LandLords:
                                    scr_.playernumber = SelfContestRule.llrplayerNumberList[temp];
                                    break;
                                case GameKind_Enum.GameKind_GuanDan:
                                    scr_.playernumber = SelfContestRule.gdplayerNumberList[temp];
                                    break;
                            }
                            RefreshPlayerNumber((int)gameKind, createtype, true);
                        });
                    }
                }
                break;
        }
        GameObject llsecondGroup = rulebg.Find("yusai").Find("ImageBG").gameObject;        for (int index = 0; index < llsecondGroup.transform.childCount; index++)        {            Toggle toggle = llsecondGroup.transform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            toggle.onValueChanged.AddListener(delegate (bool vale) { 
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                switch(gameKind)
                {
                    case GameKind_Enum.GameKind_LandLords:
                        scr_.precontest = SelfContestRule.llrprecontestList[temp];
                        break;
                    case GameKind_Enum.GameKind_GuanDan:
                        scr_.precontest = SelfContestRule.gdprecontestList[temp];
                        break;
                    case GameKind_Enum.GameKind_Chess:
                        scr_.precontest = SelfContestRule.gdprecontestChessList[temp];
                        scr_.finalcontest = scr_.precontest;
                        break;
                }
                RefreshPlayerNumber((int)gameKind, createtype, true);
            });        }

        GameObject llthirdGroup = rulebg.Find("juesai").Find("ImageBG").gameObject;        for (int index = 0; index < llthirdGroup.transform.childCount; index++)        {            Toggle toggle = llthirdGroup.transform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            toggle.onValueChanged.AddListener(delegate (bool vale) { 
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                switch (gameKind)
                {
                    case GameKind_Enum.GameKind_LandLords:
                        scr_.finalcontest = SelfContestRule.llrprecontestList[temp];
                        break;
                    case GameKind_Enum.GameKind_GuanDan:
                        scr_.finalcontest = SelfContestRule.gdprecontestList[temp];
                        break;
                    case GameKind_Enum.GameKind_Chess:
                        scr_.finalcontest = SelfContestRule.gdprecontestChessList[temp];
                        break;
                }
                RefreshPlayerNumber((int)gameKind, createtype, true);
            });        }

        GameObject llforthGroup = rulebg.Find("feiyong").Find("ImageBG").gameObject;        for (int index = 0; index < llforthGroup.transform.childCount; index++)        {            Toggle toggle = llforthGroup.transform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            toggle.onValueChanged.AddListener(delegate (bool vale) { 
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                scr_.cost = SelfContestRule.costList[temp];
                RefreshPlayerNumber((int)gameKind, createtype, true);
            });        }
    }

    //创建比赛
    private void OnCreateContest()    {        CustomAudio.GetInstance().PlayCustomAudio(1002);

        if (selfcreateNumber_ >= 10)
        {
            CCustomDialog.OpenCustomConfirmUI(1651);
            return;
        }

        byte gameid = SelfContestDataManager.instance().gameid;
        int power = SelfContestDataManager.instance().selfcontestcsvs[gameid].datas[0];
        int level = SelfContestDataManager.instance().selfcontestcsvs[gameid].datas[playtimesindex_];

        int pay4appointment = power * level;

        if (GameMain.hall_.GetPlayerData().GetDiamond() + GameMain.hall_.GetPlayerData().GetCoin() < pay4appointment)
        {
            CCustomDialog.OpenCustomConfirmUI(1501);
            return;
        }

        UMessage createContestDataMsg = new UMessage((uint)GameCity.EMSG_ENUM.Contestmsg_PlayerCreateContestRequest);
        createContestDataMsg.Add(GameMain.hall_.GetPlayerId());
        createContestDataMsg.Add(SelfContestDataManager.instance().gamedataid);
        createContestDataMsg.Add(scr_.playernumber);
        createContestDataMsg.Add(scr_.cost);
        createContestDataMsg.Add(scr_.precontest);
        createContestDataMsg.Add(scr_.finalcontest);

        Debug.Log(" 比赛游戏:"+ SelfContestDataManager.instance().gamedataid + " 比赛人数:" + scr_.playernumber + " 报名费:" + scr_.cost + "预赛场数:" + scr_.precontest + " 决赛场数" + scr_.finalcontest);

        if (rulepanel2_ == null)
        {
            createContestDataMsg.Add((uint)0);
        }
        else
        {
            if (!rulepanel2_.activeSelf)
                createContestDataMsg.Add((uint)0);
            else
                createContestDataMsg.Add(scr_.timeseconds);
        }

        NetWorkClient.GetInstance().SendMsg(createContestDataMsg);

        UnityEngine.Transform groupTransform = rulepanel1_ ? rulepanel1_.transform : rulepanel2_.transform;
        groupTransform.Find("Right/ImageBG/Button_chuangjian").GetComponent<Button>().interactable = false;
        groupTransform.Find("Right/ImageBG/Button_chuangjian_0").GetComponent<Button>().interactable = false;
    }

    void ChangeRuleInfo(int pick, bool ison, ESlefContestType constType)    {        if (ison)        {            byte CurGameId = 0;            Transform groupTransform = null;
            switch(constType)
            {
                case ESlefContestType.TimeContest:
                    CurGameId = (byte)TimeMatchGameIdData[pick];
                    groupTransform = rulepanel2_.transform.Find("Right");
                    break;
                case ESlefContestType.PlayerNumberContest:
                    CurGameId = (byte)PlayerNumberMatchGameIdData[pick];
                    groupTransform = rulepanel1_.transform.Find("Right");
                    break;
            }
            int lastIndex = groupTransform.childCount - 1;            for (int index = 0; index < groupTransform.childCount; index++)            {                GameObject child = groupTransform.GetChild(index).gameObject;                child.SetActive(lastIndex == index || pick == index);            }
            SelfContestDataManager.instance().gameid = CurGameId;            CurContestGameIndex = pick;            SelfContestDataManager.instance().SetGameDataID((int)constType * 3 + pick);            RefreshPayInfo(1, constType);
            RefreshPlayerNumber(CurGameId, constType);
            RefreshContestTime(groupTransform.parent);
        }    }

    void RefreshPlayerNumber(int pick, ESlefContestType constType,bool updateState = false)
    {
        int GameKey = (int)constType * 10000 + pick;
        if (CurMatchDataDictionary.ContainsKey(GameKey))
        {
            if(updateState)
            {
                CurMatchDataDictionary[GameKey] = scr_;
            }
            else
            {
                scr_ = CurMatchDataDictionary[GameKey];
            }
        }
    }

    void RefreshPayInfo(int levelindex, ESlefContestType contestType)    {
        Transform groupTransform = null;
        switch(contestType)
        {
            case ESlefContestType.TimeContest:
                groupTransform = rulepanel2_.transform.Find("Right");
                break;
            case ESlefContestType.PlayerNumberContest:
                groupTransform = rulepanel1_.transform.Find("Right");
                break;
        }        byte gameid = SelfContestDataManager.instance().gameid;        int power = SelfContestDataManager.instance().selfcontestcsvs[gameid].datas[0];        int level = SelfContestDataManager.instance().selfcontestcsvs[gameid].datas[levelindex];        int pay4appointment = power * level;        groupTransform.Find("ImageBG/Button_chuangjian").gameObject.SetActive(pay4appointment > 0);        groupTransform.Find("ImageBG/Button_chuangjian_0").gameObject.SetActive(pay4appointment <= 0);        Text pay = groupTransform.Find("ImageBG/ImageBGcost").Find("Textnum").gameObject.GetComponent<Text>();        pay.text = pay4appointment.ToString();        playtimesindex_ = (byte)levelindex;    }

    private void OnBack2Hall(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (root_ != null)
            {
                root_.transform.Find("Top/CreateGame_BG").gameObject.SetActive(false);
                root_.SetActive(false);
            }
            if (GameMain.hall_.contestui_ != null)
                GameMain.hall_.contestui_.SetActive(true);
        }
    }
}
