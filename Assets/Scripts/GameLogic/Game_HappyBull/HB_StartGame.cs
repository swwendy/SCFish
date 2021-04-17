using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using USocket.Messages;
using DragonBones;
using XLua;

[Hotfix]
public class HB_StartGame
{
    GameObject pickroot_;
    GameObject gameroot_;
    GameObject resultwin_;
    GameObject resultlost_;

    Dictionary<byte, Sprite> pokers_;

    float currentLeftTime;
    bool iswaitting;
    bool isshowdealpoker;
    bool isStartCount;
    bool isUpdateScalePokers;
    string waitcontent;
    bool isopenpoker;
    bool isopenlastpoker;

    public HB_StartGame(GameObject pickroot)
    {
        LoadGameResource();
        InitSetButton();
        LoadResultResource();

        Start(pickroot);
        InitPokers();
        InitPlayerInfos();
        InitRule();
        InitHBMsg();

        clickpokers = new List<int>();
        pokers_ = new Dictionary<byte, Sprite>();
    }
	
    public void Start(GameObject pickroot)
    {
        pickroot_ = pickroot;
        currentLeftTime = 0.0f;
        
        isStartCount = false;
        isUpdateScalePokers = false;
        isopenpoker = false;
        isopenlastpoker = false;
        GameObject resultBG = gameroot_.transform.Find("Pop-up").Find("Anime").gameObject;
        resultBG.SetActive(false);
        waitcontent = "等待其他玩家...";
    }

    void InitHBMsg()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_SM_OTHERENTERROOM, BackBullHappyOtherEnter);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_SM_TIMECOUNT, BackBullHappyTimeCount);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_SM_PUBLISHPOKERS, BackBullHappyPublishPokers);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_SM_DEALBOSS, BackBullHappyDealBoss);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCmsg_BULLHAPPY_SM_CHIPIN, BackBullHappyChipIn);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCmsg_BULLHAPPY_SM_OPENPOKERS, BackBullHappyOpenPokers);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCmsg_BULLHAPPY_SM_CACULATE, BackBullHappyCaculate);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_SM_RESULT, BackBullHappyResult);
    }


    bool BackBullHappyResult(uint _msgType, UMessage msg)
    {
        byte iswin = msg.ReadByte();
        byte pokerType = msg.ReadByte();
        long changeMoney = msg.ReadLong();

        GameObject resultBG = gameroot_.transform.Find("Pop-up").Find("Anime").gameObject;
        resultBG.SetActive(true);
        if (changeMoney > 0)
        {
            UnityArmatureComponent winanim = resultwin_.GetComponent<UnityArmatureComponent>();
            winanim.animation.Play("newAnimation");
        }
        else
        {
            UnityArmatureComponent lostanim = resultlost_.GetComponent<UnityArmatureComponent>();
            lostanim.animation.Play("newAnimation");
        }

        return true;
    }

    bool BackBullHappyCaculate(uint _msgType, UMessage msg)
    {
        CaculateBull(true);
        isUpdateScalePokers = true;
        isopenlastpoker = true;
        isopenpoker = true;
        return true;
    }

    bool BackBullHappyOpenPokers(uint _msgType, UMessage msg)
    {
        isUpdateScalePokers = true;
        isopenlastpoker = false;
        isopenpoker = false;
        return true;
    }

    bool BackBullHappyChipIn(uint _msgType, UMessage msg)
    {
        PickMultiple(true);

        return true;
    }

    bool BackBullHappyDealBoss(uint _msgType, UMessage msg)
    {
        //uint bossid = msg.ReadUInt();

        //for(int index = 0; index < HB_DataCenter.Instance().roomdata_.playersData.Count; index++)
        //{
        //    if(HB_DataCenter.Instance().roomdata_.playersData[index].playerid == bossid)
        //    {
        //        BecomeBoss(HB_DataCenter.Instance().roomdata_.playersData[index].sign);
        //    }
        //}
        CarryBoss(true);
        return true;
    }

    bool BackBullHappyTimeCount(uint _msgType, UMessage msg)
    {
        HB_DataCenter.Instance().counttime = msg.ReadSingle();
        isStartCount = true;
        waitcontent = "游戏即将开始...";
        return true;
    }

    void SetPokerTypeByIndex(int index, string type)
    {
        GameObject pokerBG = gameroot_.transform.Find("Middle").
            Find("Chip_Point").Find("Button_" + index.ToString()).gameObject;

        Text pokertype = pokerBG.transform.Find("poker").Find("Text_paixing").gameObject.GetComponent<Text>();
        pokertype.text = type;
    }

    bool BackBullHappyPublishPokers(uint _msgType, UMessage msg)
    {
        HB_PokerInfo pokerinfo = new HB_PokerInfo();

        byte pokersNumber = msg.ReadByte();
        for (byte index = 0; index < pokersNumber; index++)
        {
            byte pokerNo = msg.ReadByte();
            HB_DataCenter.Instance().pokerinfos[pokerNo].pokertype = msg.ReadByte();
            byte pokerSum = msg.ReadByte();
            HB_DataCenter.Instance().pokerinfos[pokerNo].pokers.Clear();
            for (int pokerindex = 0; pokerindex < pokerSum; pokerindex++)
            {
                HB_DataCenter.Instance().pokerinfos[pokerNo].pokers.Add(msg.ReadByte());
            }

            string pokertype = HB_DataCenter.Instance().pokertypes[HB_DataCenter.Instance().pokerinfos[pokerNo].pokertype].pokertype;
            pokertype = pokertype + "x" + HB_DataCenter.Instance().pokertypes[HB_DataCenter.Instance().pokerinfos[pokerNo].pokertype].rewardrate.ToString();

            SetPokerTypeByIndex(pokerNo, pokertype);
        }

        HB_DataCenter.Instance().roomstate = (byte)GameCity.BullHappyGameState_Enum.BullHappyRoomState_DealPokers;
        isUpdateScalePokers = true;
        isopenpoker = true;
        isopenlastpoker = false;

        return true;
    }
    
    void SetPoker(GameObject pokerBG,int pokersindex, int index)
    {
        string pokerName = "";
        if (index > 0)
            pokerName = " (" + index.ToString() + ")";

        if (HB_DataCenter.Instance().roomstate < (byte)GameCity.BullHappyGameState_Enum.BullHappyRoomState_DealPokers)
        {
            pokerBG.transform.Find("Image_poker" + pokerName).gameObject.SetActive(false);
            return;
        }
        else
        {
            pokerBG.transform.Find("Image_poker" + pokerName).gameObject.SetActive(true);
        }

        Image poker = pokerBG.transform.Find("Image_poker" + pokerName).gameObject.GetComponent<Image>();
        if (HB_DataCenter.Instance().pokerinfos[(byte)pokersindex].pokers.Count > index)
            poker.sprite = pokers_[HB_DataCenter.Instance().pokerinfos[(byte)pokersindex].pokers[index]];
    }

    void SetPokerByIndex(int pokersindex, bool isshow)
    {
        GameObject pokerBG = gameroot_.transform.Find("Middle").
            Find("Chip_Point").Find("Button_" + pokersindex.ToString()).Find("poker").gameObject;

        if (isshow)
        {
            for (int index = 0; index < 4; index++)
            {
                SetPoker(pokerBG, pokersindex, index);
            }
        }
        else
        {
            for (int index = 0; index < 5; index++)
            {
                SetPoker(pokerBG, pokersindex, 0);
                //string pokerName = "";
                //if (index > 0)
                //    pokerName = " (" + index.ToString() + ")";

                //if (HB_DataCenter.Instance().roomstate < (byte)GameCity.BullHappyGameState_Enum.BullHappyRoomState_DealPokers)
                //{
                //    pokerBG.transform.FindChild("Image_poker" + pokerName).gameObject.SetActive(false);
                //    continue;
                //}
                //else
                //{
                //    pokerBG.transform.FindChild("Image_poker" + pokerName).gameObject.SetActive(true);
                //}

                //Image poker = pokerBG.transform.FindChild("Image_poker" + pokerName).gameObject.GetComponent<Image>();
                //if (HB_DataCenter.Instance().pokerinfos[(byte)pokersindex].pokers.Count > index)
                //    poker.sprite = pokers_[0];
            }
        }
    }

    Vector3 scaleSpeed = new Vector3(-2.5f, 0.0f, 0.0f);
    int pokersIndex = 0;
    void UpdateScalePokers(float time)
    {
        if (!isUpdateScalePokers)
            return;

        int index = 0;
        GameObject pokerBG = gameroot_.transform.Find("Middle").Find("Chip_Point").
            Find("Button_" + index.ToString()).Find("poker").gameObject;

        int start = 0;
        int end = 0;

        if(isopenpoker)
        {
            start = 0;
            end = 1;
        }
        else
        {
            start = 1;
            end = 5;
        }

        for (int pokerindex = start; pokerindex < end; pokerindex++)
        {
            string pokerName = "";
            if (pokerindex > 0)
                pokerName = " (" + pokerindex.ToString() + ")";

            GameObject poker = pokerBG.transform.Find("Image_poker" + pokerName).gameObject;

            poker.transform.localScale += scaleSpeed * time;
            if (poker.transform.localScale.x <= 0.0f)
            {
                //RefreshPokers();
                SetPokerByIndex(index, true);
                scaleSpeed.x = 2.5f;
            }

            if (poker.transform.localScale.x >= 1.0f)
            {
                scaleSpeed.x = -2.5f;

                if (isopenlastpoker)
                {
                    pokersIndex = 4;
                    string tpokerName = pokersIndex.ToString();

                    GameObject tpoker = pokerBG.transform.Find("Image_poker" + tpokerName).gameObject;
                    tpoker.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                else
                    for (int tpokerindex = 0; tpokerindex < 5; tpokerindex++)
                    {
                        string tpokerName = "";
                        if (tpokerindex > 0)
                            tpokerName = " (" + tpokerindex.ToString() + ")";

                        GameObject tpoker = pokerBG.transform.Find("Image_poker" + tpokerName).gameObject;
                        tpoker.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    }

                PlayPokerTypeAudioByIndex(HB_DataCenter.Instance().pokerinfos[(byte)index].pokertype);
                ShowPokerTypeByIndex(true, index);

                pokersIndex++;
                if (pokersIndex >= 5)
                {
                    isUpdateScalePokers = false;
                }
            }
        }
    }

    void PlayPokerTypeAudioByIndex(int index)
    {
        int audiokey = 0;
        if (index <= 10)
            audiokey = 1008 + index;
        else
            audiokey = 1003 + index;

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullHappy);
        if (gamedata == null)
            return;
        AudioManager.Instance.PlaySound(gamedata.ResourceABName, BAK_DataCenter.Instance().audiodatas[audiokey].audioName);
    }

    void ShowPokerTypeByIndex(bool isshow, int index)
    {
        if (index == 0)
        {
            ShowBossPokerType(isshow);
            return;
        }

        GameObject pokerBG = gameroot_.transform.Find("Middle").
            Find("Chip_Point").Find("Button_" + index.ToString()).gameObject;
        GameObject pokertype = pokerBG.transform.Find("poker").Find("Text_paixing").gameObject;
        pokertype.SetActive(isshow);
    }

    void ShowBossPokerType(bool isshow)
    {
        GameObject pokerBG = gameroot_.transform.Find("Middle").
            Find("Chip_Point").Find("poker_Zhuangjia").gameObject;

        GameObject pokertype = pokerBG.transform.Find("Text_paixing").gameObject;
        pokertype.SetActive(isshow);
    }

    bool BackBullHappyOtherEnter(uint _msgType, UMessage msg)
    {
        //int index = HB_DataCenter.Instance().roomdata_.playersData.Count + 1;

        HB_PlayerData data = new HB_PlayerData();

        data.playerid = msg.ReadUInt();
        data.faceid = msg.ReadInt();
        string url = msg.ReadString();
        data.totalcoin = msg.ReadLong();
        data.name = msg.ReadString();
        data.sign = msg.ReadByte();

        HB_DataCenter.Instance().roomdata_.playersData.Add(data);

        UpdatePlayerInfoByIndex(data.sign);

        return true;
    }

    void InitSetButton()
    {
        GameObject setbtn = gameroot_.transform.Find("Top").Find("ButtonExpand").gameObject;
        XPointEvent.AutoAddListener(setbtn, OnClickSetBtn, null);
        GameObject closebtn = gameroot_.transform.Find("Pop-up").Find("Set").Find("ImageBG").Find("ButtonClose").gameObject;
        XPointEvent.AutoAddListener(closebtn, OnCloseSetPanel, null);
        GameObject confirmbtn = gameroot_.transform.Find("Pop-up").Find("Set").Find("ImageBG").Find("ButtonOk").gameObject;
        XPointEvent.AutoAddListener(confirmbtn, OnCloseSetPanel, null);

        Slider musicSlider = gameroot_.transform.Find("Pop-up").Find("Set").Find("ImageBG").Find("Slider_Music").gameObject.GetComponent<Slider>();
        musicSlider.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.MusicVolume = value; });
        Slider soundSlider = gameroot_.transform.Find("Pop-up").Find("Set").Find("ImageBG").Find("Slider_Sound").gameObject.GetComponent<Slider>();
        soundSlider.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.SoundVolume = value; });
    }

    void OnClickSetBtn(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            GameObject setpanel = gameroot_.transform.Find("Pop-up").Find("Set").gameObject;
            setpanel.SetActive(true);
        }
    }

    void OnCloseSetPanel(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            GameObject setpanel = gameroot_.transform.Find("Pop-up").Find("Set").gameObject;
            setpanel.SetActive(false);
        }
    }

    void InitInvitBtn()
    {

    }

    public void Update ()
    {
        float deltatime = Time.deltaTime;
        
        UpdateLeftTime(deltatime);
        WaitTips(HB_DataCenter.Instance().roomdata_.roleNumber == 0);
        UpdateScalePokers(deltatime);
    }

    void InitPlayerInfos()
    {
        for(int index = 1; index <= 5; index++)
        {
            UpdatePlayerInfoByIndex(index);
        }
    }

    void UpdatePlayerInfoByIndex(int index)
    {
        GameObject playerbg = gameroot_.transform.Find("Middle").Find("PlayerInfor").gameObject;

        GameObject infobg = playerbg.transform.Find("PlayerInfo_" + index.ToString()).Find("PlayerBG").gameObject;
        Image headimg = infobg.transform.Find("Head").Find("HeadMask").Find("ImageHead").gameObject.GetComponent<Image>();
        Text money = infobg.transform.Find("Image_coinframe").Find("Text_Coin").gameObject.GetComponent<Text>();
        Text name = infobg.transform.Find("TextName").gameObject.GetComponent<Text>();

        if (index == 1)
        {
            headimg.sprite = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(),
                GameMain.hall_.GetPlayerData().GetPlayerID(), (int)GameMain.hall_.GetPlayerData().PlayerIconId);
            name.text = GameMain.hall_.GetPlayerData().GetPlayerName();
            money.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
        }
        else
        {
            if(HB_DataCenter.Instance().roomdata_.playersData.Count - 1 < index)
            {
                playerbg.SetActive(false);
                return;
            }

            playerbg.SetActive(true);
            headimg.sprite = GameMain.hall_.GetIcon(HB_DataCenter.Instance().roomdata_.playersData[index - 1].url,
                HB_DataCenter.Instance().roomdata_.playersData[index - 1].playerid,HB_DataCenter.Instance().roomdata_.playersData[index - 1].faceid);
            name.text = HB_DataCenter.Instance().roomdata_.playersData[index - 1].name;
            money.text = HB_DataCenter.Instance().roomdata_.playersData[index - 1].totalcoin.ToString();
        }
    }

    void RefreshPlayerMoney()
    {
        GameObject playerbg = gameroot_.transform.Find("Middle").Find("PlayerInfor").gameObject;

        for (int index = 1; index <= 5; index++)
        {
            GameObject infobg = playerbg.transform.Find("PlayerInfo_" + index.ToString()).Find("PlayerBG").gameObject;
            Text money = infobg.transform.Find("Image_coinframe").Find("Text_Coin").gameObject.GetComponent<Text>();

            if (index == 1)
                money.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
            else
                money.text = "";
        }
    }

    void UpdateLeftTime(float time)
    {
        if (!iswaitting)
            return;

        GameObject timeObject = gameroot_.transform.Find("Pop-up").Find("Countdown").gameObject;
        Text timeTx = timeObject.transform.Find("Text").gameObject.GetComponent<Text>();
        currentLeftTime -= time;
        if (currentLeftTime > 0.0f)
            timeObject.SetActive(true);
        else
        {
            CloseLeftTime();
            return;
        }
        int time2text = (int)currentLeftTime;
        timeTx.text = waitcontent + time2text.ToString();
    }

    void ShowLeftTime(float time)
    {
        GameObject timeObject = gameroot_.transform.Find("Pop-up").Find("Countdown").gameObject;
        timeObject.SetActive(true);
        currentLeftTime = time;
        iswaitting = true;
    }

    void CloseLeftTime()
    {
        GameObject timeObject = gameroot_.transform.Find("Pop-up").Find("Countdown").gameObject;
        iswaitting = false;
        timeObject.SetActive(false);
        currentLeftTime = 0.0f;
    }

    void InitRule()
    {
        GameObject rulebtn = gameroot_.transform.Find("Middle").Find("Button_Rule").gameObject;
        XPointEvent.AutoAddListener(rulebtn, OnShowRule, null);

        GameObject rulebg = gameroot_.transform.Find("Pop-up").Find("Rule").Find("Rule_BG").gameObject;
        XPointEvent.AutoAddListener(rulebg, OnCloseRule, null); 
    }

    void OnShowRule(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            GameObject rulepanel = gameroot_.transform.Find("Pop-up").Find("Rule").gameObject;
            rulepanel.SetActive(true);
        }
    }

    void OnCloseRule(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            GameObject rulepanel = gameroot_.transform.Find("Pop-up").Find("Rule").gameObject;
            rulepanel.SetActive(false);
        }
    }

    void LoadGameResource()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullHappy);
        if (gamedata != null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            if (bundle == null)
                return;
            UnityEngine.Object obj0 = bundle.LoadAsset("Niu0_MainUI");
            gameroot_ = (GameObject)GameMain.instantiate(obj0);
            GameObject background = GameObject.Find("Canvas/Root");
            gameroot_.transform.SetParent(background.transform, false);
        }
    }

    float dealtime = 1.5f;
    float currentdealtime = 0.0f;
    void ShowDealPoker(float time, byte pokerindex)
    {
        if (!isshowdealpoker)
            return;

        GameObject pokerObj = gameroot_.transform.Find("Pop-up").Find("Deal").gameObject;
        currentdealtime += time;
        if (currentdealtime > 1.5f)
        {
            pokerObj.SetActive(false);
            isshowdealpoker = false;
        }
        else
        {
            Image poker = pokerObj.transform.Find("Image_poker").GetComponent<Image>();
            poker.sprite = pokers_[pokerindex];
            pokerObj.SetActive(true);
        }
    }

    int currentPokerPosition;
    int sendPokerRound;
    int pokertimes_ = 0;
    bool isSendingPoker = false;
    byte firstPoker;
    void SendingPokers()
    {
        //发牌
        if (!isSendingPoker)
            return;

        if (FlyPokers.Instance().isUpdate)
        {
            if (FlyPokers.Instance().flyObj == null)
                FlyPokers.Instance().LoadPokerResource();
            return;
        }

        ShowBackPokerByIndexes(currentPokerPosition, sendPokerRound);
        pokertimes_++;
        if (pokertimes_ >= 5)
        {
            sendPokerRound += 1;
            pokertimes_ = 0;
        }
        if (sendPokerRound >= 5)
        {
            currentPokerPosition = 0;
            sendPokerRound = 0;
            if (FlyPokers.Instance().flyObj == null)
                FlyPokers.Instance().LoadPokerResource();

            FlyPokers.Instance().flyObj.SetActive(false);
            isSendingPoker = false;
            CloseChargePokers();
            return;
        }

        currentPokerPosition += 1;
        if (currentPokerPosition >= 5 + firstPoker)
            currentPokerPosition = firstPoker;

        Vector3 targetPosition = new Vector3();
        int tempPosition = currentPokerPosition;
        if (tempPosition >= 5)
            tempPosition -= 5;
        string pokerName = "";
        if (sendPokerRound > 0)
            pokerName = " (" + sendPokerRound.ToString() + ")";
        if (tempPosition == 0)
        {
            targetPosition = gameroot_.transform.Find("Middle").
            Find("Chip_Point").Find("poker_Zhuangjia").Find("Image_poker" + pokerName).position;
        }
        else
        {
            targetPosition = gameroot_.transform.Find("Middle").
            Find("Chip_Point").Find("Button_" + tempPosition.ToString()).
            Find("poker").Find("Image_poker" + pokerName).position;
        }
        if (FlyPokers.Instance().flyObj == null)
            FlyPokers.Instance().LoadPokerResource();
        FlyPokers.Instance().flyObj.SetActive(true);
        ResetFlyPokers(targetPosition);
    }

    void ResetFlyPokers(Vector3 tposition)
    {
        Vector3 startPosition = gameroot_.transform.Find("Middle").
            Find("Chip_Point").Find("poker_paidui").position;

        FlyPokers.Instance().isUpdate = true;
        FlyPokers.Instance().SetChipStartPosition(startPosition);
        FlyPokers.Instance().targetPosition = tposition;
    }

    void ShowBackPokerByIndexes(int position, int index)
    {
        if (position >= 5)
            position -= 5;

        if (position == 0)
        {
            GameObject pokerBG = gameroot_.transform.Find("Middle").
                Find("Chip_Point").Find("poker_Zhuangjia").gameObject;

            string pokerName = "";
            if (index > 0)
                pokerName = " (" + index.ToString() + ")";
            GameObject poker = pokerBG.transform.Find("Image_poker" + pokerName).gameObject;
            Image pokerback = poker.GetComponent<Image>();
            poker.SetActive(true);
            pokerback.sprite = pokers_[0];
        }
        else
        {
            GameObject pokerBG = gameroot_.transform.Find("Middle").
                Find("Chip_Point").Find("Button_" + position.ToString()).Find("poker").gameObject;

            string pokerName = "";
            if (index > 0)
                pokerName = " (" + index.ToString() + ")";
            GameObject poker = pokerBG.transform.Find("Image_poker" + pokerName).gameObject;
            Image pokerback = poker.GetComponent<Image>();
            poker.SetActive(true);
            pokerback.sprite = pokers_[0];
        }

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullHappy);
        if (gamedata == null)
            return;
        AudioManager.Instance.PlaySound(gamedata.ResourceABName, BAK_DataCenter.Instance().audiodatas[1003].audioName);
    }

    void CloseChargePokers()
    {
        GameObject pokers = gameroot_.transform.Find("Middle").Find("Chip_Point").
            Find("poker_paidui").gameObject;
        pokers.SetActive(false);
    }

    byte[] mTexasCards = {
    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D,	//方块
	0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D,	//梅花
	0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D,	//红桃
	0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D,	//黑桃
};

    void InitPokers()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullHappy);
        if (gamedata != null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            if (bundle == null)
                return;

            for (byte round = 1; round <= 4; round++)
            {
                string cardName = "";
                switch (round)
                {
                    case 1:
                        cardName = "block";
                        break;
                    case 2:
                        cardName = "club";
                        break;
                    case 3:
                        cardName = "heart";
                        break;
                    case 4:
                        cardName = "spade";
                        break;
                }

                for (byte index = 1; index <= 13; index++)
                {
                    Sprite pokerSprite = bundle.LoadAsset<Sprite>(cardName + index.ToString());
                    pokers_.Add((byte)(index + (round - 1) * 16), pokerSprite);
                }
            }

            Sprite backSprite = bundle.LoadAsset<Sprite>("puke_back");
            pokers_.Add(0, backSprite);
        }
    }

    void InitCarryBoss()
    {
        GameObject carryboss = gameroot_.transform.Find("Middle").Find("PlayerInfor").
            Find("PlayerInfo_1").Find("BetBG_1").gameObject;
        GameObject btn0 = carryboss.transform.Find("Button").gameObject;
        XPointEvent.AutoAddListener(btn0, OnNotCarry, null);

        for(int index = 1; index <= 5; index++)
        {
            GameObject btn = carryboss.transform.Find("Button (" + index.ToString() + ")").gameObject;
            int tempindex = index;
            XPointEvent.AutoAddListener(btn, OnCarry, tempindex);
        }
    }

    void OnNotCarry(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            UMessage pickmsg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_CM_DEALBOSS);

            pickmsg.Add(GameMain.hall_.GetPlayerId());
            pickmsg.Add((byte)0);

            HallMain.SendMsgToRoomSer(pickmsg);
        }
    }

    void OnCarry(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            UMessage pickmsg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_CM_DEALBOSS);

            pickmsg.Add(GameMain.hall_.GetPlayerId());
            pickmsg.Add((byte)button);

            HallMain.SendMsgToRoomSer(pickmsg);
        }
    }

    void CarryBoss(bool isShow)
    {
        GameObject carryboss = gameroot_.transform.Find("Middle").Find("PlayerInfor").
            Find("PlayerInfo_1").Find("BetBG_1").gameObject;

        carryboss.SetActive(isShow);
    }

    void InitPickMultiple()
    {
        GameObject setbet = gameroot_.transform.Find("Middle").Find("PlayerInfor").
            Find("PlayerInfo_1").Find("BetBG_2").gameObject;

        for (int index = 0; index < 6; index++)
        {
            string btnName = "";
            if (index == 0)
                btnName = "Button";
            else
                btnName = "Button (" + index.ToString() + ")";

            GameObject btn = setbet.transform.Find(btnName).gameObject;
            int tempindex = index;
            XPointEvent.AutoAddListener(btn, OnClickMultiple, tempindex);
        }
    }

    void OnClickMultiple(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            UMessage pickmsg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_CM_CHIPIN);

            pickmsg.Add(GameMain.hall_.GetPlayerId());
            pickmsg.Add((byte)button);

            HallMain.SendMsgToRoomSer(pickmsg);
        }
    }

    void PickMultiple(bool isShow)
    {
        GameObject setbet = gameroot_.transform.Find("Middle").Find("PlayerInfor").
            Find("PlayerInfo_1").Find("BetBG_2").gameObject;

        setbet.SetActive(isShow);
    }

    void ShowPrivateCards(bool isShow)
    {
        GameObject pokers = gameroot_.transform.Find("Middle").
            Find("PlayerInfor").Find("PlayerInfo_1").Find("poker_group").gameObject;

        pokers.SetActive(isShow);
    }

    void SetPackPokers(bool isShow)
    {
        GameObject pokers = gameroot_.transform.Find("Middle").
            Find("PlayerInfor").Find("PlayerInfo_1").Find("poker_group").gameObject;

        pokers.SetActive(isShow);
        if (!isShow)
            return;

        for (int index = 0; index < 5; index++)
        {
            string pokerName = "";
            if (index == 0)
                pokerName = "Image_poker";
            else
                pokerName = "Image_poker (" + index.ToString() + ")";

            Image poker = pokers.transform.Find(pokerName).gameObject.GetComponent<Image>();
            poker.sprite = null;   
        }
    }

    void InitButtonGroup()
    {
        GameObject buttonGroup = gameroot_.transform.Find("Middle").
            Find("PlayerInfor").Find("PlayerInfo_1").Find("Buttongroup").gameObject;

        GameObject youniubtn = buttonGroup.transform.Find("Button_you").gameObject;
        XPointEvent.AutoAddListener(youniubtn, OnCaculateBull, true);
        GameObject wuniubtn = buttonGroup.transform.Find("Button_wu").gameObject;
        XPointEvent.AutoAddListener(youniubtn, OnCaculateBull, false);

        GameObject pokers = gameroot_.transform.Find("Middle").
            Find("PlayerInfor").Find("PlayerInfo_1").Find("poker_group").gameObject;

        for (int index = 0; index < 5; index++)
        {
            string pokerName = "";
            if (index == 0)
                pokerName = "Image_poker";
            else
                pokerName = "Image_poker (" + index.ToString() + ")";

            GameObject poker = pokers.transform.Find(pokerName).gameObject;
            int tempindex = index;
            XPointEvent.AutoAddListener(poker, OnClickPoker, tempindex);
        }
    }

    List<int> clickpokers;
    int sign;
    void OnClickPoker(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            int index = (int)button;
            int counter = HB_DataCenter.Instance().pokerinfos[sign].pokers[index];
            GameObject pokers = gameroot_.transform.Find("Middle").
                Find("PlayerInfor").Find("PlayerInfo_1").Find("poker_group").gameObject;
            string pokerName = "";
            if (index == 0)
                pokerName = "Image_poker";
            else
                pokerName = "Image_poker (" + index.ToString() + ")";

            GameObject poker = pokers.transform.Find(pokerName).gameObject;
            

            if(clickpokers.Contains(index))
            {
                poker.transform.Translate(new Vector3(0.0f, -10.0f, 0.0f));
                clickpokers.Remove(index);
                SetCounterTextByIndex(clickpokers.Count, 0);
            }
            else
            {
                clickpokers.Add(index);
                poker.transform.Translate(new Vector3(0.0f, 10.0f, 0.0f));
                SetCounterTextByIndex(clickpokers.Count, counter);
            }
        }
    }

    void SetCounterTextByIndex(int index, int counter)
    {
        GameObject counterBG = gameroot_.transform.Find("Middle").
            Find("PlayerInfor").Find("PlayerInfo_1").Find("counterBG").gameObject;
        string counterName = "Text_" + index.ToString();
        Text counterTx = counterBG.transform.Find(counterName).gameObject.GetComponent<Text>();
        counterTx.text = counter.ToString();
    }

    void OnCaculateBull(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            UMessage pickmsg = new UMessage((uint)GameCity.EMSG_ENUM.CCmsg_BULLHAPPY_CM_CACULATE);

            pickmsg.Add(GameMain.hall_.GetPlayerId());
            pickmsg.Add((byte)button);

            HallMain.SendMsgToRoomSer(pickmsg);
        }
    }

    void CaculateBull(bool isShow)
    {
        GameObject counterBG = gameroot_.transform.Find("Middle").
            Find("PlayerInfor").Find("PlayerInfo_1").Find("counterBG").gameObject;
        counterBG.SetActive(isShow);

        GameObject buttonGroup = gameroot_.transform.Find("Middle").
            Find("PlayerInfor").Find("PlayerInfo_1").Find("Buttongroup").gameObject;
        buttonGroup.SetActive(isShow);
    }

    void PublicCards(bool isShow)
    {
        GameObject publicpoker = gameroot_.transform.Find("Middle").
            Find("PlayerInfor").Find("PlayerInfo_1").Find("poker_Show").gameObject;
        publicpoker.SetActive(isShow);
    }

    void WaitTips(bool isShow, string content = "等待其他玩家...")
    {
        GameObject tips = gameroot_.transform.Find("Pop-up").Find("Tips").gameObject;
        tips.SetActive(isShow);

        Text tip = tips.transform.Find("Text").gameObject.GetComponent<Text>();
        tip.text = content;
    }

    void PlayerSitByIndex(int index)
    {
        GameObject seatBG = gameroot_.transform.Find("Middle").Find("PlayerInfor").gameObject;
        GameObject seat = seatBG.transform.Find("PlayerInfo_" + index.ToString()).gameObject;
    }

    void RefreshRoomInfo()
    {
        Text roombet = gameroot_.transform.Find("Middle").Find("RoomInfo_BG").
            Find("ImageMiniBet").Find("TextBetNum").gameObject.GetComponent<Text>();
        roombet.text = "";
    }

    void LoadResultResource()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullHappy);
        if (gamedata != null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            if (bundle == null)
                return;

            GameObject background = GameObject.Find("Canvas/Root");

            UnityEngine.Object obj0 = bundle.LoadAsset("Anime_ResultWin");
            resultwin_ = (GameObject)GameMain.instantiate(obj0);
            resultwin_.transform.SetParent(background.transform, false);
            resultwin_.SetActive(false);

            UnityEngine.Object obj1 = bundle.LoadAsset("Anime_ResultLost");
            resultlost_ = (GameObject)GameMain.instantiate(obj1);
            resultlost_.transform.SetParent(background.transform, false);
            resultlost_.SetActive(false);
        }
    }

    void ShowResult()
    {

    }
}