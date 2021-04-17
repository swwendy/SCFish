using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using USocket.Messages;
using DragonBones;
using UnityEngine.EventSystems;
using XLua;

class Names
{
    public static string Canvas = "BackGround";

    public static string loadingpanel = "Car_Loading";
    public static string loginpanel = "Car_Lobby";
    public static string gamepanel = "Car_Main";

    public static string toplobby = "Top_Lobby";
    public static string midlobby = "Middle_Lobby";
    public static string leftblue = "Image (1)";
    public static string rightblue = "Image";
    public static string green = "Image";
    public static string rmbtn = "Button1";
    public static string cjbtn = "Button2";

    public static string lottery = "Lottery";
    public static string betnum = "BetNum";
    public static string over = "New_Result";
}

[Hotfix]
public class CGame_CheHang : CGameBase
{
    GameObject leftgreen_;
    GameObject rightgreen_;
    float currentflash_;
    bool direction_;
    public GameObject yazhupanel_;
    public GameObject gamepanel_;
    public GameObject pickpanel_;
    GameObject root_;
    GameObject gamingPanel_;

    bool isyazhu_ = false;
    bool isgaming_ = false;

    public YaZhu yazhu_;
    CH_StartGame game_;

    public CGame_CheHang() : base(GameKind_Enum.GameKind_CarPort)
    {
        leftgreen_ = null;
        rightgreen_ = null;
        currentflash_ = 0.0f;
        direction_ = false;
        yazhupanel_ = null;
        gamepanel_ = null;
        pickpanel_ = null;
        root_ = null;
        gamingPanel_ = null;
        isyazhu_ = false;
        isgaming_ = false;
        yazhu_ = null;
        game_ = null;
    }

    // Use this for initialization
    public override void Initialization()
    {
        base.Initialization();
        InitMsgFunc();
        DownBundle();
        //LoadPickUI();
        //init();

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata != null)
            AudioManager.Instance.PlayBGMusic(gamedata.ResourceABName, "BackMiusc");
        else
            Debug.Log("音效资源BackMiusc加载失败");

        CH_DataCenter.Instance().state.roomLevel = 0;

        Pickone();
    }

    bool StartYaZhu(uint _msgType, UMessage _ms) //消息处理函数
    {
        return true;
    }

    void init()
    {

        leftgreen_ = pickpanel_.transform.Find(Names.toplobby).Find(Names.leftblue).Find(Names.green).gameObject;
        rightgreen_ = pickpanel_.transform.Find(Names.toplobby).Find(Names.rightblue).Find(Names.green).gameObject;
        currentflash_ = 0.0f;
        direction_ = true;


        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata == null)
        {
            Debug.Log("游戏id:1不存在");
            return;
        }
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
        if (bundle == null)
            return;

        GameObject test = pickpanel_.transform.Find(Names.midlobby).Find(Names.rmbtn).gameObject;

        GameObject rmda = pickpanel_.transform.Find(Names.midlobby).Find("Anime_Car_Left").gameObject;
        UnityArmatureComponent rmanimate = rmda.GetComponent<UnityArmatureComponent>();
        rmanimate.animation.Play("jingchang");
        XPointEvent.AutoAddListener(test, rumenchang, rmda);

        GameObject chaoji = pickpanel_.transform.Find(Names.midlobby).Find(Names.cjbtn).gameObject;
        GameObject cjda = pickpanel_.transform.Find(Names.midlobby).Find("Anime_Car_Right").gameObject;
        UnityArmatureComponent cjanimate = cjda.GetComponent<UnityArmatureComponent>();
        cjanimate.animation.Play("jingchang");

        XPointEvent.AutoAddListener(chaoji, chaojichang, cjda);

        ResetGameUI();

        //Button backBtn = pickpanel_.transform.FindChild("Top_Lobby").FindChild("ButtonReturn").gameObject.GetComponent<Button>();
        //backBtn.onClick.AddListener(delegate () { ExitGame(); });
        GameObject backBtn = pickpanel_.transform.Find("Top_Lobby").Find("ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(backBtn, ExitGame, null);
    }

    public override void ResetGameUI()
    {
        GameObject iconobj = pickpanel_.transform.Find("Bottom_Lobby").Find("Image_HeadFram").Find("Image_Head").gameObject;
        Image icon = iconobj.GetComponent<Image>();
        icon.sprite = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(), 
                                                GameMain.hall_.GetPlayerId(),
                                                (int)GameMain.hall_.GetPlayerData().PlayerIconId);

        GameObject jinbi = pickpanel_.transform.Find("Bottom_Lobby").Find("JinBiBG").Find("TextNum").gameObject;
        Text jinbiTx = jinbi.GetComponent<Text>();
        jinbiTx.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();

        GameObject jiangjuan = pickpanel_.transform.Find("Bottom_Lobby").Find("JiangJuanBG").Find("TextNum").gameObject;
        Text jiangjuanTx = jiangjuan.GetComponent<Text>();
        //jiangjuanTx.text = GameMain.hall_.GetPlayerData().PlayerLottery.ToString();
        jiangjuanTx.text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();

        Image vipImg = pickpanel_.transform.Find("Bottom_Lobby").Find("Image_Vip").
            Find("Vip_Text").Find("Num").gameObject.GetComponent<Image>();
        Image vipTypeImg = pickpanel_.transform.Find("Bottom_Lobby").Find("Image_Vip").gameObject.GetComponent<Image>();
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (GameMain.hall_.GetPlayerData().GetVipLevel() == 0)
            vipTypeImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_hui");
        else
            vipTypeImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_jin");
        vipImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + GameMain.hall_.GetPlayerData().GetVipLevel().ToString());

        GameObject name = pickpanel_.transform.Find("Bottom_Lobby").Find("TextName").gameObject;
        Text nameTx = name.GetComponent<Text>();
        nameTx.text = GameMain.hall_.GetPlayerData().GetPlayerName();
    }

    public override void RefreshGamePlayerCoin(uint AddMoney)
    {
        base.RefreshGamePlayerCoin(AddMoney);

        GameObject jinbi = pickpanel_.transform.Find("Bottom_Lobby").Find("JinBiBG").Find("TextNum").gameObject;
        Text jinbiTx = jinbi.GetComponent<Text>();
        long coin;
        long.TryParse(jinbiTx.text, out coin);
        coin += AddMoney;
        jinbiTx.text = coin.ToString();

        GameObject playerInfo = root_.transform.Find("MainBG").Find("Bet").Find("Bottom_Main").Find("PlayerInfo").gameObject;
        Text playerMoney = playerInfo.transform.Find("Text_Gold_BG").Find("Text_Gold").gameObject.GetComponent<Text>();
        long.TryParse(playerMoney.text, out coin);
        coin += AddMoney;
        playerMoney.text = coin.ToString();
    }

    void PickedGame(byte gametype)
    {
        UMessage chooseLevel = new UMessage((uint)GameCity.CarportMsg_enum.CarportMsg_CM_CHOOSELEVEL);

        CarPortChooseLevel cpcl = new CarPortChooseLevel();
        cpcl.userID = GameMain.hall_.GetPlayerId();
        cpcl.Create();
        cpcl.level = gametype;

        cpcl.SetSendData(chooseLevel);
        //if (HallMain.gametcpclient.IsSocketConnected)
        //{
            HallMain.SendMsgToRoomSer(chooseLevel);
        //}
    }

    // Update is called once per frame
    public override void ProcessTick()
    {
        base.ProcessTick();

        Amount();

        if (isyazhu_)
            Yazhu();

        if (isgaming_)
            Gaming();

    }

    void Yazhu()
    {
        yazhu_.Update();

        if (isgaming_)
        {
            isyazhu_ = false;
            List<YazhuButtonStruct> buttons = yazhu_.buttons;
            CH_DataCenter.Instance().isStartChips = false;
            changeToGamePanel();
            game_.TargetPosition((int)CH_DataCenter.Instance().result.carIndex);
            game_.resetgame();
            game_.showYazhuInfo(buttons);

            GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
            if (gamedata != null)
            {
                AudioManager.Instance.PlaySound(gamedata.ResourceABName, "sgj_start");
                AudioManager.Instance.PlayBGMusic(gamedata.ResourceABName, "OpenBackMiusc");
            }
            else
                Debug.Log("音效资源sgj_start，OpenBackMiusc加载失败");
        }
    }

    void Gaming()
    {
        if (game_ != null)
        {
            game_.Update();

            if (yazhu_ == null)
                return;
        }
    }

    void Amount()
    {
        if (leftgreen_ == null || rightgreen_ == null)
            return;

        if (direction_)
            currentflash_ += 4.0f * Time.deltaTime;
        else
            currentflash_ -= 3.0f * Time.deltaTime;

        UnityEngine.UI.Image leftimage = leftgreen_.GetComponent<UnityEngine.UI.Image>();
        UnityEngine.UI.Image rightimage = rightgreen_.GetComponent<UnityEngine.UI.Image>();

        if (currentflash_ > Random.Range(0.5f, 1.0f))
            direction_ = false;
        else if (currentflash_ < 0.0f)
            direction_ = true;

        leftimage.fillAmount = currentflash_;
        rightimage.fillAmount = currentflash_;
    }


    void rumenchang(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            Pickone();
        }

        if (eventtype == EventTriggerType.PointerDown)
        {
            UnityArmatureComponent animate = ((GameObject)button).GetComponent<UnityArmatureComponent>();
            animate.animation.Play("idle");
        }
    }

    void Pickone()
    {
        if (CH_DataCenter.Instance().state.roomLevel != 1
                && CH_DataCenter.Instance().state.roomLevel != 0)
        {
            CCustomDialog.OpenCustomConfirmUI(2010);
            return;
        }
        root_.SetActive(true);
        //LoadUIByName(Names.gamepanel);
        if (yazhu_ == null)
        {
            if (pickpanel_)
            {
                //GameMain.safeDeleteObj(pickpanel_);
                pickpanel_.SetActive(false);
            }
            yazhu_ = new YaZhu(root_, pickpanel_);
            //if(yazhu_.currentChouMa_ == 0)
            yazhu_.currentChouMa_ = CH_DataCenter.Instance().generic_[1];
            yazhu_.level_ = 1;
            yazhu_.Start();
        }
        else
        {
            if (yazhu_.level_ != 1)
            {
                yazhu_.switchBossBtn(true);
                yazhu_.resetToggleBtn();
            }

            if (pickpanel_)
            {
                //GameMain.safeDeleteObj(pickpanel_);
                pickpanel_.SetActive(false);
            }
            //if (yazhu_.currentChouMa_ == 0)
            yazhu_.currentChouMa_ = CH_DataCenter.Instance().generic_[1];
            yazhu_.level_ = 1;
            yazhu_.changeToChipPanel();
        }
        yazhu_.bosslist = root_.transform.Find("MainBG").Find("DealerList").gameObject;
        //isyazhu_ = true;

        PickedGame((byte)1);
    }

    void chaojichang(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (CH_DataCenter.Instance().state.roomLevel != 2
            && CH_DataCenter.Instance().state.roomLevel != 0)
            {
                CCustomDialog.OpenCustomConfirmUI(2010);
                return;
            }
            //LoadUIByName(Names.gamepanel);
            root_.SetActive(true);
            if (yazhu_ == null)
            {
                if (pickpanel_)
                {
                    //GameMain.safeDeleteObj(pickpanel_);
                    pickpanel_.SetActive(false);
                }
                yazhu_ = new YaZhu(root_, pickpanel_);
                //if (yazhu_.currentChouMa_ == 0)
                yazhu_.currentChouMa_ = 1000;
                yazhu_.level_ = 2;
                yazhu_.Start();
            }
            else
            {
                if (yazhu_.level_ != 2)
                {
                    yazhu_.switchBossBtn(true);
                    yazhu_.resetToggleBtn();
                }
                if (pickpanel_)
                {
                    //GameMain.safeDeleteObj(pickpanel_);
                    pickpanel_.SetActive(false);
                }
                //if (yazhu_.currentChouMa_ == 0)
                yazhu_.currentChouMa_ = 1000;
                yazhu_.level_ = 2;
                yazhu_.changeToChipPanel();
            }

            yazhu_.bosslist = root_.transform.Find("MainBG").Find("DealerList").gameObject;
            //isyazhu_ = true;

            PickedGame((byte)2);
        }
        if (eventtype == EventTriggerType.PointerDown)
        {
            UnityArmatureComponent animate = ((GameObject)button).GetComponent<UnityArmatureComponent>();
            animate.animation.Play("idle");
        }
    }

    void changeToGamePanel()
    {
        gamingPanel_ = root_.transform.Find("MainBG").Find(Names.lottery).gameObject;
        gamingPanel_.SetActive(true);
    }

    void LoadPickUI()
    {
        //    Object objPrefab = Resources.Load(name);
        //    if (objPrefab)
        //    {
        //        objPrefab_ = (MonoBehaviour.Instantiate(objPrefab, new Vector3(480.0f, 270.0f, 0.0f), Quaternion.identity) as GameObject);
        //        GameObject background = GameObject.FindGameObjectWithTag("BackGround");
        //        //objPrefab_.transform.parent = background.transform;
        //        objPrefab_.transform.SetParent(background.transform);
        //    }
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata != null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            if (bundle == null)
                return;
            UnityEngine.Object obj0 = bundle.LoadAsset(Names.loginpanel);
            pickpanel_ = (GameObject)GameMain.instantiate(obj0);
            GameObject background = GameObject.Find("Canvas/Root");
            pickpanel_.transform.SetParent(background.transform, false);
            // objPrefab_.transform.Translate(new Vector3(480.0f, 270.0f, 0.0f));
        }
    }

    void DownBundle()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata == null)
        {
            Debug.Log("游戏id:1不存在");
            return;
        }
        //CResVersionCompareUpdate.CompareABVersionAndUpdate(gamedata.ResourceABName);
        //HttpDownload.DownFile(GameDefine.LuancherURL, GameDefine.AssetBundleSavePath, "car.resource");
        //AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, gamedata.ResourceABName);

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = bundle.LoadAsset("Car_Main");
        root_ = (GameObject)GameMain.instantiate(obj0);
        GameObject background = GameObject.Find("Canvas/Root");
        root_.transform.SetParent(background.transform, false);
        root_.SetActive(false);
        gamingPanel_ = root_.transform.Find("MainBG").Find(Names.lottery).gameObject;
        GameObject overlist = root_.transform.Find("MainBG").Find(Names.over).gameObject;
        game_ = new CH_StartGame(gamingPanel_, overlist);
        game_.Start();
    }

    void CloseGamePanel()
    {
        if (gamingPanel_ != null)
            gamingPanel_.SetActive(false);
    }

    void ShowGamePanel()
    {
        if (gamingPanel_ != null)
            gamingPanel_.SetActive(true);
    }

    bool BackCarNewJoin(uint _msgType, UMessage _ms)
    {
        CH_DataCenter.Instance().state.currentBossID = _ms.ReadUInt();
        if (CH_DataCenter.Instance().state.currentBossID > 0)
        {
            CH_DataCenter.Instance().state.bossName = _ms.ReadString();
            CH_DataCenter.Instance().state.faceID = _ms.ReadUInt();
            CH_DataCenter.Instance().state.url = _ms.ReadString();
            CH_DataCenter.Instance().state.BossMoney = _ms.ReadLong();
            CH_DataCenter.Instance().state.winMoney = _ms.ReadLong();
            CH_DataCenter.Instance().state.state = _ms.ReadChar();//根据状态CarPortGameState_enum
            CH_DataCenter.Instance().state.leftTime = _ms.ReadSingle();
            CH_DataCenter.Instance().state.carIcon = _ms.ReadChar();
            CH_DataCenter.Instance().state.infos.Clear();
            for (int index = 0; index < (int)CH_DataCenter.Instance().state.carIcon; index++)
            {
                CarInfo info = new CarInfo();
                info.carID = _ms.ReadChar();
                info.totalMoney = _ms.ReadLong();
                info.selfMoney = _ms.ReadLong();
                CH_DataCenter.Instance().state.infos.Add(info);
            }
        }
        else
        {
            yazhu_.ClearBossInfo();
            yazhu_.NoBoss();
        }
        //yazhu_.Start();
        updateUIData();
        changeState();
        return true;
    }

    bool BackCarChooseLevel(uint _msgType, UMessage _ms)
    {
        bool isInGame = _ms.ReadBool();
        byte level = _ms.ReadByte();
        if (isInGame)
        {
            root_.SetActive(true);
            isyazhu_ = true;
        }
        else
        {
            CCustomDialog.OpenCustomConfirmUI(2000);
            isyazhu_ = false;
        }
        return true;
    }

    bool BackCarHistroy(uint _msgType, UMessage _ms)
    {
        CH_DataCenter.Instance().histroy.historyIconNumber = _ms.ReadChar();
        CH_DataCenter.Instance().histroy.iconNumber = _ms.ReadChar();
        CH_DataCenter.Instance().histroy.icons.Clear();
        for (int index = 0; index < (int)CH_DataCenter.Instance().histroy.historyIconNumber; index++)
        {
            CH_DataCenter.Instance().histroy.icons.Add(_ms.ReadChar());
        }
        CH_DataCenter.Instance().histroy.times.Clear();
        for (int index = 0; index < (int)CH_DataCenter.Instance().histroy.iconNumber; index++)
        {
            HistroyTimes ht = new HistroyTimes();
            ht.iconID = _ms.ReadChar();
            ht.times = _ms.ReadLong();
            CH_DataCenter.Instance().histroy.times.Add(ht);
        }

        if (yazhu_ != null)
            yazhu_.initRecord();

        return true;
    }

    bool BackCarBossChange(uint _msgType, UMessage _ms)
    {
        if (CH_DataCenter.Instance().state.currentBossID == GameMain.hall_.GetPlayerData().GetPlayerID())
        {
            yazhu_.switchBossBtn(false);
            yazhu_.resetToggleBtn();
        }

        CH_DataCenter.Instance().state.currentBossID = _ms.ReadUInt();
        CH_DataCenter.Instance().state.faceID = _ms.ReadUInt();
        CH_DataCenter.Instance().state.url = _ms.ReadString();
        CH_DataCenter.Instance().state.bossName = _ms.ReadString();
        CH_DataCenter.Instance().state.BossMoney = _ms.ReadLong();
        CH_DataCenter.Instance().state.winMoney = 0;

        if (yazhu_ == null)
            return false;

        if (CH_DataCenter.Instance().state.currentBossID == GameMain.hall_.GetPlayerData().GetPlayerID())
        {
            //yazhu_.ShowTipsOneSecond("您已坐庄");
            CCustomDialog.OpenCustomConfirmUI(2012);
        }
        yazhu_.switchBossBtn(CH_DataCenter.Instance().state.currentBossID != GameMain.hall_.GetPlayerId());

        updateUIData();
        return true;
    }

    bool BackCarResult(uint _msgType, UMessage _ms)
    {
        CH_DataCenter.Instance().result.carIndex = _ms.ReadChar();
        CH_DataCenter.Instance().result.two = _ms.ReadChar();
        //Debug.Log("index is:" + (int)CH_DataCenter.Instance().result.carIndex);
        isgaming_ = true;
        return true;
    }

    bool BackCarBeginChip(uint _msgType, UMessage _ms)
    {
        CH_DataCenter.Instance().isStartChips = true;
        CH_DataCenter.Instance().state.state = (char)GameCity.CarPortGameState_enum.CarPortState_ChipIn;
        updateUIData();
        changeState(false);
        return true;
    }

    bool BackCarChip(uint _msgType, UMessage _ms)
    {
        if (yazhu_ == null)
            return false;
        return yazhu_.BackChip(_ms);
    }

    bool BackCarApplyBoss(uint _msgType, UMessage _ms)
    {
        return yazhu_.AskForBackOpen(_ms);
    }

    bool BackCarCancelApplyBoss(uint _msgType, UMessage _ms)
    {
        return yazhu_.AskForCancelBackOpen(_ms);
    }

    bool BackCarDownBoss(uint _msgType, UMessage _ms)
    {
        return yazhu_.BackDownBoss();
    }

    bool BackCarCancelDownBoss(uint _msgType, UMessage _ms)
    {
        return yazhu_.BackCancelDownBoss();
    }

    bool BackCarBossList(uint _msgType, UMessage _ms)
    {
        return yazhu_.BackBossList(_ms);
    }

    bool BackCarSortList(uint _msgType, UMessage _ms)
    {
        if (isgaming_)
        {
            if (yazhu_ == null || game_ == null)
                return false;

            bool result = game_.AccessResultData(_ms);

            if (game_.sd.god.nSelfAdd > 0)
                CH_DataCenter.Instance().histroy.winTotal += game_.sd.god.nSelfAdd;
            if (game_.sd.god.nBossAdd < 0)
                CH_DataCenter.Instance().state.winMoney += game_.sd.god.nBossAdd;
            if (CH_DataCenter.Instance().state.currentBossID == GameMain.hall_.GetPlayerData().GetPlayerID())
                yazhu_.SetPlayerMoney(game_.sd.god.nBossCoin, game_.sd.god.nBossAdd);
            else
                yazhu_.SetPlayerMoney(game_.sd.god.nSelfCoin, CH_DataCenter.Instance().histroy.winTotal);

            yazhu_.SetBossMoney(game_.sd.god.nBossCoin, game_.sd.god.nBossAdd);

            return result;
        }
        return true;
    }

    bool BackCarKickOut(uint _msgType, UMessage _ms)
    {
        //if (yazhu_ != null)
        //{
        //    CCustomDialog.OpenCustomConfirmUI(1017);
        //    yazhu_.changeToPickPanel();
        //    CCustomDialog.CloseCustomWaitUI();
        //}

        CH_DataCenter.Instance().isKickOut = true;
        Exit();

        return true;
    }

    bool BackCarLeaveRoom(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        switch (state)
        {
            case 1:
                //yazhu_.changeToPickPanel();
                Exit();
                CCustomDialog.CloseCustomWaitUI();
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                CCustomDialog.OpenCustomConfirmUI(1011);
                break;
            default:
                CCustomDialog.OpenCustomConfirmUI(2010);
                break;
        }

        return true;
    }

    public void InitMsgFunc()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_CURTABLEDATATONEWIN, BackCarNewJoin);         //车行加入游戏
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_CHOOSELEVEL, BackCarChooseLevel);             //选择关卡
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_HISTROYCARDATA, BackCarHistroy);              //历史记录
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_BOSSCHANGE, BackCarBossChange);                //轮换庄家
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_GAMESULET, BackCarResult);                     //结算
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_BEGINCHIP, BackCarBeginChip);                  //开始下注
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_CHIPCAR, BackCarChip);                         //押
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_APPLYBOSS, BackCarApplyBoss);                  //申请坐庄
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_CANCLEAPPLYBOSS, BackCarCancelApplyBoss);      //取消申请坐庄
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_APPLYDOWNBOSS, BackCarDownBoss);               //申请下庄
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_CANCLEAPPLYDOWNBOSS, BackCarCancelDownBoss);   //取消申请下庄
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_APPLYBOSSLIST, BackCarBossList);               //庄家列表
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_SORTDATA, BackCarSortList);                    //排名信息
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_KNCKOUTROOM, BackCarKickOut);                  //踢出房间
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.CarportMsg_enum.CarportMsg_SM_LEAVEROOM, BackCarLeaveRoom);                  //离开房间
    }

    void showmessage()
    {
        if (game_ != null)
        {
            CloseGamePanel();
            game_.CloseOverlist();
        }
        if (yazhu_ != null)
            yazhu_.RefreshPlayerMoney();
    }

    public void updateUIData()
    {
        GameObject Dealer_Info = root_.transform.Find("MainBG").Find("Top_Main").Find("Dealer_Info").gameObject;
        Text bossName = Dealer_Info.transform.Find("Text_Name").gameObject.GetComponent<Text>();
        bossName.text = CH_DataCenter.Instance().state.bossName;
        Text bossMoney = Dealer_Info.transform.Find("Text_Gold").gameObject.GetComponent<Text>();
        bossMoney.text = CH_DataCenter.Instance().state.BossMoney.ToString();
        Text bossWinMoney = Dealer_Info.transform.Find("Texe_ShuYing").gameObject.GetComponent<Text>();
        if (CH_DataCenter.Instance().state.winMoney < 0)
            CH_DataCenter.Instance().state.winMoney = 0;
        bossWinMoney.text = CH_DataCenter.Instance().state.winMoney.ToString();

        GameObject playerInfo = root_.transform.Find("MainBG").Find("Bet").Find("Bottom_Main").Find("PlayerInfo").gameObject;
        Image icon = playerInfo.transform.Find("Image_HeadFram").Find("Image_Head").gameObject.GetComponent<Image>();

        icon.sprite = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(), 
                GameMain.hall_.GetPlayerId(), (int)GameMain.hall_.GetPlayerData().PlayerIconId);

        Text playerMoney = playerInfo.transform.Find("Text_Gold_BG").Find("Text_Gold").gameObject.GetComponent<Text>();
        playerMoney.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
        Text playerWinMoney = playerInfo.transform.Find("Text_ShuYing_BG").Find("Texe_ShuYing").gameObject.GetComponent<Text>();
        playerWinMoney.text = CH_DataCenter.Instance().histroy.winTotal.ToString(); ;
        Text playerName = playerInfo.transform.Find("Text_Name_BG").Find("Text_Name").gameObject.GetComponent<Text>();
        playerName.text = GameMain.hall_.GetPlayerData().GetPlayerName();
    }

    void changeState(bool first = true)
    {
        switch (CH_DataCenter.Instance().state.state)
        {
            case (char)GameCity.CarPortGameState_enum.CarPortState_ChipIn: //下注中
                //Debug.Log("下注......");
                showmessage();
                if (yazhu_ == null)
                    break;

                if (first)
                    yazhu_.timeout = CH_DataCenter.Instance().state.leftTime;
                else
                    yazhu_.timeout = 29.0f;
                isyazhu_ = true;
                isgaming_ = false;
                yazhu_.isTimeout = false;
                yazhu_.reset();
                yazhu_.switchBossBtn(CH_DataCenter.Instance().state.currentBossID != GameMain.hall_.GetPlayerId());
                //yazhu_.DeleteWaitUI();
                CCustomDialog.CloseCustomWaitUI();
                yazhu_.RefreshRecord();
                break;
            case (char)GameCity.CarPortGameState_enum.CarPortState_Roulette: //游戏开始
                //Debug.Log("游戏中......");
                //yazhu_.reset();
                ClearChipData();
                showmessage();
                CCustomDialog.OpenCustomWaitUI(2002);
                break;
            case (char)GameCity.CarPortGameState_enum.CarPortState_WaitBoss: //轮换庄家
                //Debug.Log("轮换庄家......");
                //yazhu_.reset();
                if(yazhu_ != null)
                    yazhu_.ClearBossInfo();
                ClearChipData();
                showmessage();
                CCustomDialog.OpenCustomWaitUI(2013);
                break;
            case (char)GameCity.CarPortGameState_enum.CarPortState_OverWait: //游戏结束等待
                //Debug.Log("游戏结束......");
                //yazhu_.reset();
                ClearChipData();
                showmessage();
                CCustomDialog.OpenCustomWaitUI(2002);
                break;
            default:
                break;
        }
    }

    void ClearChipData()
    {
        CH_DataCenter.Instance().state.infos.Clear();
        for (int index = 0; index < (int)CH_DataCenter.Instance().state.infos.Count; index++)
        {
            CarInfo info = new CarInfo();
            info.carID = CH_DataCenter.Instance().state.infos[index].carID;
            info.totalMoney = 0;
            info.selfMoney = 0;
            CH_DataCenter.Instance().state.infos[index] = info;
        }
    }

    void ExitGame(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            Exit();
        }
    }

    void Exit()
    {
        CustomAudio.GetInstance().PlayCustomAudio(1002);

        isyazhu_ = false;
        isgaming_ = false;
        GameMain.hall_.SwitchToHallScene();

        game_ = null;
        yazhu_ = null;
    }

    public override void ReconnectSuccess()
    {
        base.ReconnectSuccess();

        CCustomDialog.OpenCustomConfirmUI(1018, (p) => Exit());
    }
}  
