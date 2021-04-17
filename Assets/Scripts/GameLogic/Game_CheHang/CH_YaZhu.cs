
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using USocket.Messages;
using UnityEngine.EventSystems;
using XLua;

[Hotfix]
public class YaZhu
{
    enum chepai
    {
        freeari = 0,
        jaguar,
        bmw,
        porsche,
        //5倍

        bugatti,
        maybach,
        lamborghini,
        bingley,
        //20倍
    }

    public List<YazhuButtonStruct> buttons;

    public uint currentChouMa_ = 0;
    public float timeout;
    Image time1_;
    Image time2_;

    List<Sprite> sprites_;
    List<Sprite> IconSprites_;
    Texture2D m_Tex;

    public bool isTimeout;
    GameObject bosslistbtn;
    public GameObject bosslist;
    bool isWarm;

    
    Dictionary<int, uint> super_;


    GameObject gameUI_;
    public int level_;

    bool eventSetted_;
    GameObject pickPanel_;

    //float timeCount_;

    public YaZhu(GameObject gameUI, GameObject pickPanel)
    {
        gameUI_ = gameUI;
        pickPanel_ = pickPanel;
        bosslistbtn = gameUI_.transform.Find("MainBG").Find("Bet").
            Find("Bottom_Main").Find("ButtonDealerList").gameObject;
    }

    // Use this for initialization
    public void Start()
    {
        timeout = CH_DataCenter.Instance().state.leftTime;
        isTimeout = false;
        time1_ = GetTimeImageByName("ImageText_3");
        time2_ = GetTimeImageByName("ImageText_4");

        sprites_ = new List<Sprite>();
        IconSprites_ = new List<Sprite>();
        //for (int index = 0; index < 10; index++)
        //{
        //    LoadFromFile("Assets/Resources/chehang/number/", index.ToString() + ".png");
        //    Sprite tempSprite = new Sprite();
        //    tempSprite = Sprite.Create(m_Tex, new Rect(0, 0, m_Tex.width, m_Tex.height), new Vector2(0, 0));
        //    sprites_.Add(tempSprite);
        //}

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata != null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            if (bundle != null)
            {
                for (int index = 0; index < 10; index++)
                {
                    Sprite tempSprite = bundle.LoadAsset<Sprite>("Num_blue_" + index.ToString());
                    sprites_.Add(tempSprite);
                }

                for (int index = 0; index < 8; index++)
                {
                    Sprite tempSprite = bundle.LoadAsset<Sprite>("Icon_Car_0" + (index + 1).ToString());
                    IconSprites_.Add(tempSprite);
                }
            }
        }
        //AudioManager.Instance.player

        eventSetted_ = false;

        initButtons();
        initChips();
        InitExitButton();

        //generic_ = new Dictionary<int, uint>();
        //generic_.Add(1, 100);
        //generic_.Add(2, 1000);
        //generic_.Add(3, 5000);
        //generic_.Add(4, 10000);
        //generic_.Add(5, 50000);
        //generic_.Add(6, 100000);

        super_ = new Dictionary<int, uint>();
        super_.Add(1, 1000);
        super_.Add(2, 10000);
        super_.Add(3, 100000);
        super_.Add(4, 500000);
        super_.Add(5, 1000000);
        super_.Add(6, 5000000);

        InitBossButton();
        switchBossBtn(true);

        Toggle music = gameUI_.transform.Find("MainBG").Find("Top_Main").Find("ToggleVoice").gameObject.GetComponent<Toggle>();
        music.onValueChanged.AddListener(delegate (bool sound)
        {
            SwitchMusic(sound);
        });
    }

    public void resetToggleBtn()
    {
        GameObject bobtn = GetBossBotton("ToggleOpen");
        Toggle bossOpenBtn = bobtn.GetComponent<Toggle>();
        bossOpenBtn.isOn = false;

        GameObject bcbtn = GetBossBotton("ToggleClose");
        Toggle bossCloseBtn = bcbtn.GetComponent<Toggle>();
        bossCloseBtn.isOn = false;
    }

    void InitBossButton()
    {
        GameObject bobtn = GetBossBotton("ToggleOpen");
        Toggle bossOpenBtn = bobtn.GetComponent<Toggle>();
        bossOpenBtn.onValueChanged.AddListener(delegate (bool call) { AskForOpen(call); });

        GameObject bcbtn = GetBossBotton("ToggleClose");
        Toggle bossCloseBtn = bcbtn.GetComponent<Toggle>();
        bossCloseBtn.onValueChanged.AddListener(delegate (bool call) { AskForClose(call); });
    }

    public void switchBossBtn(bool isBoss)
    {
        GameObject bobtn = GetBossBotton("ToggleOpen");
        GameObject bcbtn = GetBossBotton("ToggleClose");

        bobtn.SetActive(isBoss);
        bcbtn.SetActive(!isBoss);
    }

    GameObject GetBossBotton(string name)
    {
        return gameUI_.transform.Find("MainBG").Find("Bet").Find("Bottom_Main").Find(name).gameObject;
    }

    void AskForOpen(bool call)
    {
        if (call)
        {
            UMessage afo = new UMessage((uint)GameCity.CarportMsg_enum.CarportMsg_CM_APPLYBOSS);
            CarApplyBoss cab = new CarApplyBoss();
            cab.Create(GameMain.hall_.GetPlayerId());
            cab.SetSendData(afo);
            HallMain.SendMsgToRoomSer(afo);
        }
        else
        {
            UMessage afo = new UMessage((uint)GameCity.CarportMsg_enum.CarportMsg_CM_CANCLEAPPLYBOSS);
            CarCancleApplyBoss ccab = new CarCancleApplyBoss();
            ccab.Create(GameMain.hall_.GetPlayerId());
            ccab.SetSendData(afo);
            HallMain.SendMsgToRoomSer(afo);
        }
    }

    public bool AskForBackOpen(UMessage msg)
    {
        CarBackApplyBoss cbab = new CarBackApplyBoss();
        char state = msg.ReadChar();

        if (state == (char)1)
        {
            cbab.ReadData(msg);
            cbab.nstate = state;

            //Debug.Log("become boss success");
        }
        else
        {
            //Debug.Log("become boss failed");
            if (state == (char)2)
            {
                //不在房间
            }
            if(state == (char)3)
            {
                //钱不够
                long needcoin = msg.ReadLong();
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(2018, (int)needcoin);
            }
            if(state == (char)4)
            {
                //已经在申请列表中
            }
            resetToggleBtn();
        }

        return true;
    }

    public bool AskForCancelBackOpen(UMessage msg)
    {
        CarBackCancleBoss cbcb = new CarBackCancleBoss();
        cbcb.ReadData(msg);
        //Debug.Log(cbcb.nUseid);

        return true;
    }

    void AskForClose(bool call)
    {
        if (call)
        {
            UMessage afc = new UMessage((uint)GameCity.CarportMsg_enum.CarportMsg_CM_APPLYDOWNBOSS);
            CarApplyDownBoss cadb = new CarApplyDownBoss();
            cadb.Create(GameMain.hall_.GetPlayerId());
            cadb.SetSendData(afc);
            HallMain.SendMsgToRoomSer(afc);
        }
        else
        {
            UMessage afc = new UMessage((uint)GameCity.CarportMsg_enum.CarportMsg_CM_CANCLEAPPLYDOWNBOSS);
            CarCancleApplyDowng ccad = new CarCancleApplyDowng();
            ccad.Create(GameMain.hall_.GetPlayerId());
            ccad.SetSendData(afc);
            HallMain.SendMsgToRoomSer(afc);
        }
    }

    public bool BackDownBoss()
    {
        //Debug.Log("downboss!");
        return true;
    }

    public bool BackCancelDownBoss()
    {
        //Debug.Log("cancel downboss!");
        return true;
    }

    public bool BackChip(UMessage _ms)
    {
        BackChipCar bcc = new BackChipCar();
        bcc.ReadData(_ms);
        switch (bcc.nState)
        {
            case (char)2:
                CCustomDialog.OpenCustomConfirmUI(2005);
                //ShowTipsOneSecond("您已退出");
                return false;
            case (char)3:
                CCustomDialog.OpenCustomConfirmUI(2005);
                //ShowTipsOneSecond("下注id非法");
                return false;
            case (char)4:
                CCustomDialog.OpenCustomConfirmUI(2005);
                //ShowTipsOneSecond("庄家id非法");
                return false;
            case (char)5:
                CCustomDialog.OpenCustomConfirmUI(2008);
                //ShowTipsOneSecond("庄家不可下注");
                return false;
            case (char)6:
                CCustomDialog.OpenCustomConfirmUI(2005);
                //ShowTipsOneSecond("投注金额不可为0");
                return false;
            case (char)7:
                CCustomDialog.OpenCustomConfirmUI(2000);
                //ShowTipsOneSecond("金币不足");
                return false;
            case (char)8:
                CCustomDialog.OpenCustomConfirmUI(2009);
                //ShowTipsOneSecond("已达到投注上限");
                return false;
            default:
                break;
        }

        if (buttons.Count == 0)
            return false;

        if (bcc.nUseid == GameMain.hall_.GetPlayerId())
        {
            buttons[bcc.nCarid - 1].selfMoney = bcc.nSelfChipNum;
        }
        buttons[bcc.nCarid - 1].totalMoney = bcc.nAllChipNum;
        GameObject root = GetRootIconByName(buttons[bcc.nCarid - 1].buttonName);

        Text selfTx = GetIconTextByName(root, "ImageBG_Own");
        Text allTx = GetIconTextByName(root, "ImageBG_All");

        allTx.text = buttons[bcc.nCarid - 1].totalMoney.ToString();

        if (bcc.nUseid == GameMain.hall_.GetPlayerId())
        {
            selfTx.text = buttons[bcc.nCarid - 1].selfMoney.ToString();
        }
        CanChipsNumber(root, bcc.nCanChipNum.ToString());

        GameObject playerInfo = gameUI_.transform.Find("MainBG").Find("Bet").Find("Bottom_Main").Find("PlayerInfo").gameObject;
        Image icon = playerInfo.transform.Find("Image_HeadFram").Find("Image_Head").gameObject.GetComponent<Image>();
        icon.sprite = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(), GameMain.hall_.GetPlayerId(), (int)GameMain.hall_.GetPlayerData().PlayerIconId);
        Text playerMoney = playerInfo.transform.Find("Text_Gold_BG").Find("Text_Gold").gameObject.GetComponent<Text>();
        if (bcc.nUseid == GameMain.hall_.GetPlayerId())
        {
            GameMain.hall_.GetPlayerData().SetDiamond(GameMain.hall_.GetPlayerData().GetDiamond() - currentChouMa_);
            //playerMoney.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
            playerMoney.text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();

            GameObject choose = root.transform.Find("ImageChoose").gameObject;
            choose.SetActive(true);
        }

        if(GameMain.hall_.GetPlayerId() == bcc.nUseid)
            CH_DataCenter.Instance().state.roomLevel = level_;
        return true;
    }

    public void SetPlayerMoney(long money, long addMoney)
    {
        GameObject playerInfo = gameUI_.transform.Find("MainBG").Find("Bet").Find("Bottom_Main").Find("PlayerInfo").gameObject;
        Text playerMoney = playerInfo.transform.Find("Text_Gold_BG").Find("Text_Gold").gameObject.GetComponent<Text>();
        if (money <= 0)
            money = 0;

        if (!GameMain.hall_.isGameRelief)
        {
           // GameMain.hall_.GetPlayerData().SetCoin(money);
        }
        else
        {
            GameMain.hall_.isGameRelief = false;
        }

        GameMain.hall_.GetPlayerData().SetDiamond((uint)money);

        Text playerWinMoney = playerInfo.transform.Find("Text_ShuYing_BG").Find("Texe_ShuYing").gameObject.GetComponent<Text>();
        playerWinMoney.text = addMoney.ToString();
        playerMoney.text = money.ToString();
    }

    public void RefreshPlayerMoney()
    {
        GameObject playerInfo = gameUI_.transform.Find("MainBG").Find("Bet").Find("Bottom_Main").Find("PlayerInfo").gameObject;
        Text playerMoney = playerInfo.transform.Find("Text_Gold_BG").Find("Text_Gold").gameObject.GetComponent<Text>();
        //Text playerWinMoney = playerInfo.transform.FindChild("Text_ShuYing_BG").FindChild("Texe_ShuYing").gameObject.GetComponent<Text>();

        //playerMoney.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
        playerMoney.text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();
    }

    public void SetBossMoney(long money, long addMoney)
    {
        GameObject Dealer_Info = gameUI_.transform.Find("MainBG").Find("Top_Main").Find("Dealer_Info").gameObject;
        //Text bossName = Dealer_Info.transform.FindChild("Text_Name").gameObject.GetComponent<Text>();
        //bossName.text = CH_DataCenter.Instance().state.bossName;
        Text bossMoney = Dealer_Info.transform.Find("Text_Gold").gameObject.GetComponent<Text>();
        bossMoney.text = money.ToString();
        CH_DataCenter.Instance().state.BossMoney = money;
        Text bossWinMoney = Dealer_Info.transform.Find("Texe_ShuYing").gameObject.GetComponent<Text>();
        bossWinMoney.text = addMoney.ToString();
        CH_DataCenter.Instance().state.winMoney = addMoney;
    }


    private void LoadFromFile(string path, string _name)
    {
        m_Tex = new Texture2D(1, 1);
        m_Tex.LoadImage(ReadPNG(path + _name));
    }

    private byte[] ReadPNG(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read);

        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] binary = new byte[fileStream.Length];
        fileStream.Read(binary, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        return binary;
    }

    // Update is called once per frame
    public void Update()
    {
        SetTime();
    }

    void SetTime()
    {
        timeout -= Time.deltaTime;
        if (timeout <= 0.0f)
        {
            isTimeout = true;
            return;
        }

        if (sprites_.Count != 10)
            return;

        if (timeout <= 5.0f && !isWarm)
        {
            GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
            if (gamedata != null)
                AudioManager.Instance.PlaySound(gamedata.ResourceABName, "Timer_Warm");
            isWarm = true;
        }
        if (time1_ != null && time2_ != null)
        {
            time1_.sprite = sprites_[((int)timeout) / 10];
            time2_.sprite = sprites_[((int)timeout) % 10];
        }

    }

    void initButtons()
    {
        buttons = new List<YazhuButtonStruct>();

        reset();

        Button bosslistButton = bosslistbtn.GetComponent<Button>();
        bosslistButton.onClick.AddListener(
            delegate ()
            {
                //ShowBossList();
                SendBossListMsg();
            });
    }

    void SendBossListMsg()
    {
        UMessage sblm = new UMessage((uint)GameCity.CarportMsg_enum.CarportMsg_CM_APPLYBOSSLIST);
        CarApplyBossList cabl = new CarApplyBossList();
        cabl.Create(GameMain.hall_.GetPlayerId());
        cabl.SetSendData(sblm);
        HallMain.SendMsgToRoomSer(sblm);
    }

    public bool BackBossList(UMessage msg)
    {
        //Debug.Log("access bosslist!");

        BackBossList bbl = new BackBossList();
        bbl.ReadData(msg);
        if (bbl.peopleNumber >= 0)
            initBossList(bbl);
        ShowBossList();
        return true;
    }

    void initBossList(BackBossList bbl)
    {
        GameObject bosslistScroll = bosslist.transform.Find("Image_BG").
            Find("Viewport_DealerList").Find("Content_DealerList").
            gameObject;

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata != null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            if (bundle)
            {
                ClearChilds(bosslistScroll);
                for (int index = 0; index < bbl.peopleNumber; index++)
                {
                    UnityEngine.Object obj = (GameObject)bundle.LoadAsset("DealerInfo");
                    GameObject infoUI = (GameObject)GameMain.instantiate(obj);
                    infoUI.transform.SetParent(bosslistScroll.transform, false);

                    Text name = infoUI.transform.Find("touxiangkuang").Find("name").gameObject.GetComponent<Text>();
                    name.text = bbl.infos[index].name;
                    Text money = infoUI.transform.Find("jinbi").Find("jinbishu").gameObject.GetComponent<Text>();
                    money.text = bbl.infos[index].money.ToString();

                    Image icon = infoUI.transform.Find("touxiangkuang").Find("tongxiang").gameObject.GetComponent<Image>();
                    icon.sprite = GameMain.hall_.GetIcon(bbl.infos[index].url, bbl.infos[index].uid, (int)bbl.infos[index].faceid);
                }
            }
        }
    }

    public void reset()
    {
        //resetChips();
        buttons.Clear();
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata != null)
            AudioManager.Instance.PlaySound(gamedata.ResourceABName, "pepol");
        if (CH_DataCenter.Instance().state.infos.Count == 0)
        {
            NoBoss();
        }

        for (int index = 1; index < 11; index++)
        {
            string nameNumber;
            if (index < 10)
                nameNumber = "0" + index.ToString();
            else
                nameNumber = "10";

            GameObject root = GetRootIconByName("Button_YaZhu_" + nameNumber);
            GameObject choose = root.transform.Find("ImageChoose").gameObject;
            choose.SetActive(false);
            Text selfTx = GetIconTextByName(root, "ImageBG_Own");
            selfTx.text = CH_DataCenter.Instance().state.infos[index - 1].selfMoney.ToString();
            Text allTx = GetIconTextByName(root, "ImageBG_All");
            allTx.text = CH_DataCenter.Instance().state.infos[index - 1].totalMoney.ToString();

            initButton("Button_YaZhu_" + nameNumber, index,
                CH_DataCenter.Instance().state.infos[index - 1].selfMoney,
                CH_DataCenter.Instance().state.infos[index - 1].totalMoney);

            CanChipsNumber(root, CH_DataCenter.Instance().state.infos[index - 1].selfMoney.ToString());
        }
        //timeCount_ = 0.0f;
        eventSetted_ = true;
        isWarm = false;
    }

    GameObject GetRootIconByName(string name)
    {
        return gameUI_.transform.Find("MainBG").Find("Bet").Find("Middle_Main").Find(name).gameObject;
    }

    void resetChips()
    {
        GameObject chips1 = FindChipsByLevel(1);
        GameObject chips2 = FindChipsByLevel(2);
        Toggle t1 = chips1.transform.Find("Toggle").gameObject.GetComponent<Toggle>();
        t1.isOn = true;
        Toggle t2 = chips2.transform.Find("Toggle").gameObject.GetComponent<Toggle>();
        t2.isOn = true;
    }

    void initChips()
    {
        GameObject chips1 = FindChipsByLevel(1);
        GameObject chips2 = FindChipsByLevel(2);

        if (level_ == 1)
        {
            chips1.SetActive(true);
            chips2.SetActive(false);

            //ToggleGroup group = chips1.GetComponent<ToggleGroup>();
            for (int index = 1; index < 7; index++)
            {
                string toggleName;
                int temp = index;
                if (index == 1)
                    toggleName = "Toggle";
                else
                    toggleName = "Toggle (" + (index - 1).ToString() + ")";

                Toggle t = chips1.transform.Find(toggleName).gameObject.GetComponent<Toggle>();
                t.onValueChanged.AddListener(
                    delegate (bool check)
                    {
                        onClickChips(temp);
                    });

                Text chipNumber = chips1.transform.Find(toggleName + "/TextNum").gameObject.GetComponent<Text>();
                chipNumber.text = CH_DataCenter.Instance().generic_[temp].ToString();
            }
        }
        if (level_ == 2)
        {
            chips1.SetActive(false);
            chips2.SetActive(true);

            //ToggleGroup group = chips2.GetComponent<ToggleGroup>();
            for (int index = 1; index < 7; index++)
            {
                string toggleName;
                int temp = index;
                if (index == 1)
                    toggleName = "Toggle";
                else
                    toggleName = "Toggle (" + (index - 1).ToString() + ")";

                Toggle t = chips2.transform.Find(toggleName).gameObject.GetComponent<Toggle>();
                t.onValueChanged.AddListener(
                    delegate (bool check)
                    {
                        onClickSuperChips(temp);
                    });
            }
        }
    }

    GameObject FindChipsByLevel(int level)
    {
        return gameUI_.transform.
            Find("MainBG").Find("Bet").Find("Bottom_Main").
            Find("Chips").Find("ChouMa" + level.ToString()).gameObject;
    }

    void onClickChips(int index)
    {
        currentChouMa_ = CH_DataCenter.Instance().generic_[index];
    }

    void onClickSuperChips(int index)
    {
        currentChouMa_ = super_[index];
    }

    void initButton(string name, int index, long selfMoney, long totalMoney)
    {
        YazhuButtonStruct button = new YazhuButtonStruct();
        button.buttonName = name;
        button.number = index;
        button.selfMoney = selfMoney;
        button.totalMoney = totalMoney;
        if (!eventSetted_)
            initButtonEvent(button);

        buttons.Add(button);
    }

    void initButtonEvent(YazhuButtonStruct button)
    {
        GameObject tempobj = GameObject.Find(button.buttonName);
        //Button tempbtn = tempobj.GetComponent<Button>();
        //SetImgAlpha(button, 0.0f);
        XPointEvent.AutoAddListener(tempobj, onClick, button);
    }

    void SetImgAlpha(YazhuButtonStruct button, float alpha)
    {
        GameObject tempobj = GameObject.Find(button.buttonName);
        Image tempimg = tempobj.GetComponent<Image>();
        tempimg.color = new Color(tempimg.color.r, tempimg.color.g, tempimg.color.b, alpha);
    }

    void onClick(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            UMessage msg = new UMessage((uint)GameCity.CarportMsg_enum.CarportMsg_CM_CHIPCAR);
            ChipCar cc = new ChipCar();
            cc.Create();
            cc.nUseid = GameMain.hall_.GetPlayerId();
            cc.nCarID = (char)((YazhuButtonStruct)button).number;
            cc.nChipNum = currentChouMa_;
            cc.SetSendData(msg);

            HallMain.SendMsgToRoomSer(msg);

            //GameObject root = GetRootIconByName(((YazhuButtonStruct)button).buttonName);

            ////if (lastbutton_ != null)
            ////    lastbutton_.SetActive(false);

            //GameObject choose = root.transform.FindChild("ImageChoose").gameObject;
            //choose.SetActive(true);

            //lastbutton_ = choose;
            GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
            if (gamedata != null)
                AudioManager.Instance.PlaySound(gamedata.ResourceABName, "jetton01");
        }
    }

    Image GetTimeImageByName(string name)
    {
        GameObject obj = gameUI_.transform.Find("MainBG").Find("Top_Main").Find("TimeBG").Find(name).gameObject;
        Image img = obj.GetComponent<Image>();
        return img;
    }

    Text GetIconTextByName(GameObject root, string name)
    {
        GameObject obj = root.transform.Find(name).Find("Text_Num").gameObject;
        Text text = obj.GetComponent<Text>();
        return text;
    }

    void ShowBossList()
    {
        bosslist.SetActive(true);

        Button backbtn = bosslist.GetComponent<Button>();
        backbtn.onClick.AddListener(delegate () { CloseBossList(); });
    }

    void CloseBossList()
    {
        bosslist.SetActive(false);
    }

    void CanChipsNumber(GameObject root, string canchipNumber)
    {
        Text canchips = root.transform.Find("ImageBG_Max").Find("Text_Num").gameObject.GetComponent<Text>();
        canchips.text = "可下注:" + canchipNumber;
    }

    void InitExitButton()
    {
        Button exitbtn = gameUI_.transform.Find("MainBG").Find("Top_Main").Find("ButtonReturn").gameObject.GetComponent<Button>();
        exitbtn.onClick.AddListener(delegate () { Exit(); });
    }

    public void Exit()
    {
        //HallMain.gametcpclient.CloseNetwork();
        if (CH_DataCenter.Instance().state.currentBossID == GameMain.hall_.GetPlayerData().GetPlayerID())
        {
            //ShowTipsOneSecond("庄家不能中途退出");
            CCustomDialog.OpenCustomConfirmUI(2011);
            return;
        }
        else
        {
            UMessage msg = new UMessage((uint)GameCity.CarportMsg_enum.CarportMsg_CM_LEAVEROOM);
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);
        }
        //changeToPickPanel();
    }

    public void changeToPickPanel()
    {
        gameUI_.SetActive(false);
        if(pickPanel_ != null)
            pickPanel_.SetActive(true);
        GameMain.hall_.GameBaseObj.ResetGameUI();
        AudioManager.Instance.StopSound();
    }

    public void changeToChipPanel()
    {
        initChips();

        gameUI_.SetActive(true);
        pickPanel_.SetActive(false);
    }

    void ClearChilds(GameObject obj)
    {
        int count = obj.transform.childCount;
        for (int index = 0; index < count; index++)
        {
            GameMain.safeDeleteObj(obj.transform.GetChild(0).gameObject);
        }
    }

    public void RefreshRecord()
    {
        GameObject recordList = gameUI_.transform.Find("MainBG").
            Find("Bet").Find("Middle_Main").Find("Record_xiao").gameObject;

        GameObject xiaolist = recordList.transform.Find("ScrollView_Record").
            Find("Viewport_Record").Find("Content_Record").gameObject;

        ClearChilds(xiaolist);

        int length = 0;
        if (CH_DataCenter.Instance().histroy.icons.Count > 5)
            length = 5;
        else
            length = CH_DataCenter.Instance().histroy.icons.Count;

        for (int index = CH_DataCenter.Instance().histroy.icons.Count - length;
            index < CH_DataCenter.Instance().histroy.icons.Count;
            index++)
        {
            GameObject img = new GameObject(index.ToString());
            img.AddComponent<Image>();
            Image icon = img.GetComponent<Image>();
            icon.raycastTarget = false;
            icon.sprite = IconSprites_[CH_DataCenter.Instance().histroy.icons[index] - 1];
            icon.transform.SetParent(xiaolist.transform, false);
        }
    }

    public void initRecord()
    {
        GameObject recordList = gameUI_.transform.Find("MainBG").
            Find("Bet").Find("Middle_Main").Find("Record_xiao").gameObject;

        GameObject list = recordList.transform.Find("ScrollView_Record").
            Find("Viewport_Record").gameObject;

        Button left = recordList.transform.Find("ButtonLeft").gameObject.GetComponent<Button>();
        left.onClick.AddListener(delegate () { leftClick(list); });
        Button right = recordList.transform.Find("ButtonRight").gameObject.GetComponent<Button>();
        right.onClick.AddListener(delegate () { rightClick(list); });
        Button record = recordList.transform.Find("ImageBG").gameObject.GetComponent<Button>();
        record.onClick.AddListener(delegate () { ShowHistroyPanel(); });

        RefreshRecord();
    }

    void leftClick(GameObject list)
    {
        ScrollRect hlg = list.GetComponent<ScrollRect>();
        hlg.content.Translate(new Vector3(-20.0f, 0.0f, 0.0f));
    }

    void rightClick(GameObject list)
    {
        ScrollRect hlg = list.GetComponent<ScrollRect>();
        hlg.content.Translate(new Vector3(20.0f, 0.0f, 0.0f));
    }

    void ShowHistroyPanel()
    {
        initHistroyPanel();
    }

    void RefreshHistroyPanel()
    {
        GameObject histroy = gameUI_.transform.Find("MainBG").Find("Record").gameObject;

        GameObject Total = histroy.transform.Find("ImageBG").Find("Total").gameObject;
        GameObject Details = histroy.transform.Find("ImageBG").Find("Details").
            Find("Viewport_Details").Find("Content_Details").gameObject;

        for (int index = 0; index < 8; index++)
        {
            string name;
            if (index == 0)
                name = "ImageIcon";
            else
                name = "ImageIcon (" + index.ToString() + ")";

            Text number = Total.transform.Find(name).Find("TextNum").gameObject.GetComponent<Text>();
            number.text = CH_DataCenter.Instance().histroy.times[index].times.ToString();
        }
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
        if (gamedata != null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            if (bundle)
            {
                int length = 0;
                if (CH_DataCenter.Instance().histroy.icons.Count > 27)
                    length = 27;
                else
                    length = CH_DataCenter.Instance().histroy.icons.Count;

                ClearChilds(Details);

                for (int index = CH_DataCenter.Instance().histroy.icons.Count - 1;
                    index >= CH_DataCenter.Instance().histroy.icons.Count - length;
                    index--)
                {
                    //GameObject img = new GameObject(index.ToString());
                    //img.AddComponent<Image>();
                    Object obj;
                    GameObject gobj;

                    obj = (GameObject)bundle.LoadAsset("Record_IconBG");
                    gobj = (GameObject)GameMain.instantiate(obj);
                    gobj.transform.SetParent(Details.transform, false);
                    //else
                    //{
                    //    gobj = Details.transform.GetChild(index).gameObject;
                    //}
                    Image img = gobj.transform.Find("ImageIcon").GetComponent<Image>();
                    img.GetComponent<Image>().sprite = IconSprites_[CH_DataCenter.Instance().histroy.icons[index] - 1];
                }
            }
        }
        histroy.SetActive(true);
    }

    void initHistroyPanel()
    {
        RefreshHistroyPanel();

        GameObject histroy = gameUI_.transform.Find("MainBG").Find("Record").gameObject;
        Button hisbtn = histroy.GetComponent<Button>();
        hisbtn.onClick.AddListener(delegate () { CloseHistroyPanel(); });
    }

    void CloseHistroyPanel()
    {
        GameObject histroy = gameUI_.transform.Find("MainBG").Find("Record").gameObject;
        histroy.SetActive(false);
    }

    public void ClearBossInfo()
    {
        CH_DataCenter.Instance().state.bossName = "";
        CH_DataCenter.Instance().state.faceID = 0;
        CH_DataCenter.Instance().state.url = "";
        CH_DataCenter.Instance().state.BossMoney = 0;
        CH_DataCenter.Instance().state.winMoney = 0;
        CH_DataCenter.Instance().state.state = (char)GameCity.CarPortGameState_enum.CarPortState_WaitBoss;//根据状态CarPortGameState_enum
        CH_DataCenter.Instance().state.leftTime = 0.0f;
        CH_DataCenter.Instance().state.carIcon = (char)10;
    }

    public void NoBoss()
    {
        CH_DataCenter.Instance().state.infos.Clear();
        for (int index = 0; index < 10; index++)
        {
            CarInfo info = new CarInfo();
            info.carID = (char)(index + 1);
            info.totalMoney = 0;
            info.selfMoney = 0;
            CH_DataCenter.Instance().state.infos.Add(info);
        }
    }

    void SwitchMusic(bool sound)
    {
        if (sound)
        {
            AudioManager.Instance.MusicVolume = 1.0f;
            AudioManager.Instance.SoundVolume = 1.0f;
        }
        else
        {
            AudioManager.Instance.MusicVolume = 0.0f;
            AudioManager.Instance.SoundVolume = 0.0f;
        }
    }
}
