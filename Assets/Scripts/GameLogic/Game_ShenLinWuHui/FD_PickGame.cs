using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DragonBones;
using XLua;

[Hotfix]
public class CGame_SheLinWuHui : CGameBase
{
    FD_StartGame game_;
    GameObject root_;

    public CGame_SheLinWuHui() : base(GameKind_Enum.GameKind_ForestDance)
    {
        game_ = null;
        root_ = null;

        InitForestDanceMsg();
        PickOne();
    }

    public override void Initialization()
    {
        base.Initialization();

        //LoadResource();
        //InitUI();
      
    }

    public override void ProcessTick()
    {
        base.ProcessTick();

        if (game_ != null)
            game_.Update();
	}

    bool BackForestDanceChooseLevel(uint msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        switch (state)
        {
            case 1:
                if(root_ != null)
                    root_.SetActive(false);
                byte level = _ms.ReadByte();
                if (game_ == null)
                    game_ = new FD_StartGame();
                game_.level = level;
                game_.resetData();
                if (level == 1)
                    game_.currentChouMa_ = FD_AudioDataManager.GetInstance().chipList[0].gradeone;
                else if (level == 2)
                    game_.currentChouMa_ = FD_AudioDataManager.GetInstance().chipList[1].gradeone;
                else if (level == 3)
                    game_.currentChouMa_ = FD_AudioDataManager.GetInstance().chipList[2].gradeone;

                break;
            case 2:
                CCustomDialog.OpenCustomConfirmUI(2010);
                break;
            case 3:
                CCustomDialog.OpenCustomConfirmUI(2010);
                break;
            case 4:
                CCustomDialog.OpenCustomConfirmUI(2010);
                break;
            case 5:
                List<int> coinlimitlist = FD_DataCenter.GetInstance().condition.minMoneys;
                CCustomDialog.OpenCustomConfirmUIWithFormatParamFunc(2003, Back2Hall, coinlimitlist[0]);
                break;
            default:
                CCustomDialog.OpenCustomConfirmUI(2010);
                break;
        }
        InitialCommonUI();
        return true;
    }

    bool BackForestDanceLeaveRoom(uint msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        switch (state)
        {
            case 1:
                game_.LeaveRoom();
                resetMoney();
                game_ = null;
                GameMain.hall_.SwitchToHallScene();
                CCustomDialog.CloseCustomWaitUI();
                break;
            case 2:
                CCustomDialog.OpenCustomConfirmUI(1206);
                break;
            case 3:
                CCustomDialog.OpenCustomConfirmUI(1206);
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

    bool BackForestDanceNewJoin(uint msgType, UMessage _ms)
    {
        //ForestDanceRoomData fdrd = new ForestDanceRoomData();
        if (game_ == null)
            game_ = new FD_StartGame();
        game_.fdrd_.ReadData(_ms);
        game_.level = game_.fdrd_.level;
        if (game_.fdrd_.roomState != 1)
        {
            //game_.ShowTips("游戏已开始，请稍后...");
            CCustomDialog.OpenCustomWaitUI(2002);
        }

        game_.chipTime = game_.fdrd_.lefttime;
        game_.CurrentChips_.Clear();
        foreach (int key in game_.fdrd_.chipsinfo.Keys)
        {
            game_.CurrentChips_.Add(key, game_.fdrd_.chipsinfo[key]);
        }

        game_.colorList_.Clear();
        for (int index = 0; index < game_.fdrd_.colorsort.Count; index++)
        {
            game_.colorList_.Add(game_.fdrd_.colorsort[index]);
        }
        game_.Start(root_);
        game_.InitChipObjects();
        game_.CloseResult();
        game_.refreshColor_ = true;
        game_.InitSmallHistroy();
        game_.NewJoinHandle(game_.fdrd_.handle.ToString());
        return true;
    }

    bool BackForestDanceBegin(uint msgType, UMessage _ms)
    {
        Debug.Log("game begin--------------");
        if (game_ == null)
            return false;

        //game_.CloseTips();
        CCustomDialog.CloseCustomWaitUI();
        //ForestDanceStartGame fdsg = new ForestDanceStartGame();
        game_.fdsg_.ReadData(_ms);
        game_.colorList_.Clear();
        for (int index = 0; index < game_.fdsg_.colorsort.Count; index++)
        {
            game_.colorList_.Add(game_.fdsg_.colorsort[index]);
        }
        game_.chipTime = 30.0f;
        game_.colorid = game_.fdsg_.colorid;
        game_.RestChipObjects(game_.fdsg_.colorid);
        game_.refreshColor_ = true;
        game_.CloseResult();
        game_.resetData();
        return true;
    }

    bool BackForestDanceResult(uint msgType, UMessage _ms)
    {
        Debug.Log("game result--------------");
        //ForestDanceResult fdr = new ForestDanceResult();
        game_.fdr_.ReadData(_ms);
        if (game_ == null)
            return false;
        if (game_.fdr_.model == (byte)GameCity.ForestMode_Enum.ForestMode_GiveGun)
            game_.ParseResult(game_.fdr_.signs[0]);
        else
            game_.ParseResult(game_.fdr_.sign);
        game_.begin_ = true;
        //game_.fdr_ = fdr;
        game_.chipTime = 0.0f;
        return true;
    }

    bool BackForestDanceChipIn(uint msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        if (state == 2)
        {
            //game_.ShowTips("玩家不存在");
            CCustomDialog.OpenCustomConfirmUI(2005);
            return false;
        }
        if (state == 3)
        {
            //game_.ShowTips("玩家不在房间");
            CCustomDialog.OpenCustomConfirmUI(2005);
            return false;
        }
        if (state == 4)
        {
            //game_.ShowTips("游戏已开始,无法下注");
            CCustomDialog.OpenCustomConfirmUI(2006);
            return false;
        }
        if (state == 5)
        {
            //game_.ShowTips("无效的下注类型");
            CCustomDialog.OpenCustomConfirmUI(2005);
            return false;
        }
        if (state == 6)
        {
            //game_.ShowTips("金币不足");
            CCustomDialog.OpenCustomConfirmUI(2000);
            return false;
        }
        if (state == 7)
        {
            CCustomDialog.OpenCustomConfirmUI(2017);
            return false;
        }
        game_.BackChip(_ms);
        return true;
    }

    bool BackForestDanceCount(uint msgType, UMessage _ms)
    {
        Debug.Log("game count--------------");
        //ForestDanceShowResult fdsr = new ForestDanceShowResult();
        game_.fdsr_.ReadData(_ms);
        if (game_ == null)
            return false;
        //game_.fdsr_ = fdsr;
        if (game_.fdr_.sign == 0)
            return false;
        game_.ShowResult();
        game_.NewJoinHandle(game_.fdsr_.handsel.ToString());
        return true;
    }

    public void InitForestDanceMsg()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Forest_enum.ForestMsg_SM_CHOOSElEVEL, BackForestDanceChooseLevel);           //选场
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Forest_enum.ForestMsg_SM_LEAVEROOM, BackForestDanceLeaveRoom);               //离开
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Forest_enum.ForestMsg_SM_ROOMDATATONEWJOIN, BackForestDanceNewJoin);         //进入
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Forest_enum.ForestMsg_SM_GAMEBEGIN, BackForestDanceBegin);                   //开始游戏
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Forest_enum.ForestMsg_SM_GAMERESULT, BackForestDanceResult);                 //结算
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Forest_enum.ForestMsg_SM_CHIPIN, BackForestDanceChipIn);                     //押注
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Forest_enum.ForestMsg_SM_GAMECOUNT, BackForestDanceCount);                   //结算结果
    }


    public override void RefreshGamePlayerCoin(uint AddMoney)
    {
        base.RefreshGamePlayerCoin(AddMoney);

        Text cointext = root_.transform.Find("Bottom").Find("PlayerInfoBG").
          Find("Image_coinframe").Find("Text_Coin").gameObject.GetComponent<Text>();
        long coin;
        long.TryParse(cointext.text, out coin);
        coin += AddMoney;

        cointext.text = coin.ToString();

        if (game_ != null)
            game_.RefreshPlayerAddMoney(AddMoney);
    }

    void resetMoney()
    {
        if (root_ == null)
            return;

        Text coin = root_.transform.Find("Bottom").Find("PlayerInfoBG").
            Find("Image_coinframe").Find("Text_Coin").gameObject.GetComponent<Text>();
        coin.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();

        Text diamond = root_.transform.Find("Bottom").Find("Image_DiamondFrame").
            Find("Text_Diamond").gameObject.GetComponent<Text>();
        diamond.text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();
    }

    void InitUI()
    {
        GameObject returnBtn = root_.transform.Find("Top").Find("ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(returnBtn, OnBack2Hall, null);

        Image icon = root_.transform.Find("Bottom").Find("PlayerInfoBG").
            Find("Image_HeadFram").Find("Image_Mask").Find("Image_Head").
            gameObject.GetComponent<Image>();

        icon.sprite = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(), 
            GameMain.hall_.GetPlayerId(), (int)GameMain.hall_.GetPlayerData().PlayerIconId);

        resetMoney();

        Text name = root_.transform.Find("Bottom").Find("PlayerInfoBG").
            Find("TextName").gameObject.GetComponent<Text>();
        name.text = GameMain.hall_.GetPlayerData().GetPlayerName();

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        Image vipImg = root_.transform.Find("Bottom").Find("PlayerInfoBG").
            Find("Image_Vip").Find("Vip_Text").Find("Num").gameObject.GetComponent<Image>();
        vipImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + GameMain.hall_.GetPlayerData().GetVipLevel().ToString());

        Image vipTypeImg = root_.transform.Find("Bottom").Find("PlayerInfoBG").
            Find("Image_Vip").gameObject.GetComponent<Image>();
        if (GameMain.hall_.GetPlayerData().GetVipLevel() == 0)
            vipTypeImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_hui");
        else
            vipTypeImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_jin");

        GameObject setBtn = root_.transform.Find("Bottom").Find("ButtonSet").gameObject;
        XPointEvent.AutoAddListener(setBtn, OnSet, null);
    }

    void PickOne()
    {
        //先进入该场条件是否达到
        //List<int> coinlimitlist = FD_DataCenter.GetInstance().condition.minMoneys;
        //if (coinlimitlist.Count > 0 && GameMain.hall_.GetPlayerData().GetDiamond() < coinlimitlist[0])
        //{
        //    CCustomDialog.OpenCustomConfirmUIWithFormatParamFunc(2003, Back2Hall, coinlimitlist[0]);
        //    return;
        //}
        ////进场金币超过最高限制(上限为0即为无上限制)
        //List<int> Maxcoinlimitlist = FD_DataCenter.GetInstance().condition.maxMoneys;
        //if (Maxcoinlimitlist.Count > 0 && Maxcoinlimitlist[0] != 0 && GameMain.hall_.GetPlayerData().GetDiamond() > Maxcoinlimitlist[0])
        //{
        //    CCustomDialog.OpenCustomConfirmUIWithFormatParamFunc(2015, Back2Hall, Maxcoinlimitlist[0]);
        //    return;
        //}
        UMessage forestChooseLevel = new UMessage((uint)GameCity.Forest_enum.ForestMsg_CM_CHOOSElEVEL);
        ForestDanceChooseLevel fdcl = new ForestDanceChooseLevel();
        fdcl.userID = GameMain.hall_.GetPlayerId();
        fdcl.level = 1;
        fdcl.SetSendData(forestChooseLevel);
        HallMain.SendMsgToRoomSer(forestChooseLevel);
    }

    //新手
    void OnClickNewbie(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            PickOne();
        }

        if (eventtype == EventTriggerType.PointerDown)
        {
            UnityArmatureComponent animate = ((GameObject)button).GetComponent<UnityArmatureComponent>();
            animate.animation.Play("newAnimation");
        }
    }
    //高手
    void OnClickSuper(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            //先进入该场条件是否达到
            List<int> coinlimitlist = FD_DataCenter.GetInstance().condition.minMoneys;
            if (coinlimitlist.Count > 1  && GameMain.hall_.GetPlayerData().GetCoin() < coinlimitlist[1])
            {
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(2003, coinlimitlist[1]);
                return;
            }
            //进场金币超过最高限制(上限为0即为无上限制)
            List<int> Maxcoinlimitlist = FD_DataCenter.GetInstance().condition.maxMoneys;
            if (Maxcoinlimitlist.Count > 1 && Maxcoinlimitlist[1] != 0 && GameMain.hall_.GetPlayerData().GetCoin() > Maxcoinlimitlist[1])
            {
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(2015, Maxcoinlimitlist[1]);
                return;
            }
            UMessage forestChooseLevel = new UMessage((uint)GameCity.Forest_enum.ForestMsg_CM_CHOOSElEVEL);
            ForestDanceChooseLevel fdcl = new ForestDanceChooseLevel();
            fdcl.userID = GameMain.hall_.GetPlayerId();
            fdcl.level = 2;
            fdcl.SetSendData(forestChooseLevel);
            HallMain.SendMsgToRoomSer(forestChooseLevel);
        }
        if (eventtype == EventTriggerType.PointerDown)
        {
            UnityArmatureComponent animate = ((GameObject)button).GetComponent<UnityArmatureComponent>();
            animate.animation.Play("newAnimation");
        }
    }
    //专家
    void OnClickProfasion(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {

            //先进入该场条件是否达到
            List<int> coinlimitlist = FD_DataCenter.GetInstance().condition.minMoneys;
            if (coinlimitlist.Count > 2 && GameMain.hall_.GetPlayerData().GetCoin() < coinlimitlist[2])
            {
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(2003, coinlimitlist[2]);
                return;
            }

            //进场金币超过最高限制(上限为0即为无上限制)
            List<int> Maxcoinlimitlist = FD_DataCenter.GetInstance().condition.maxMoneys;
            if(Maxcoinlimitlist[2] != -1)
            {
                if (Maxcoinlimitlist.Count > 2 && Maxcoinlimitlist[2] != 0 && GameMain.hall_.GetPlayerData().GetCoin() > Maxcoinlimitlist[2])
                {
                    CCustomDialog.OpenCustomConfirmUIWithFormatParam(2015, Maxcoinlimitlist[2]);
                    return;
                }
            }

            UMessage forestChooseLevel = new UMessage((uint)GameCity.Forest_enum.ForestMsg_CM_CHOOSElEVEL);
            ForestDanceChooseLevel fdcl = new ForestDanceChooseLevel();
            fdcl.userID = GameMain.hall_.GetPlayerId();
            fdcl.level = 3;
            fdcl.SetSendData(forestChooseLevel);
            HallMain.SendMsgToRoomSer(forestChooseLevel);
        }
        if (eventtype == EventTriggerType.PointerDown)
        {
            UnityArmatureComponent animate = ((GameObject)button).GetComponent<UnityArmatureComponent>();
            animate.animation.Play("newAnimation");
        }
    }

    void OnSet(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {

        }
    }

    void OnBack2Hall(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            Back2Hall(null);
        }
    }

    public void Back2Hall(object value)
    {
        game_ = null;
        GameMain.hall_.SwitchToHallScene();

        //UMessage leaveMsg = new UMessage((uint)GameCity.Forest_enum.ForestMsg_CM_LEAVEROOM);
        //leaveMsg.Add(GameMain.hall_.GetPlayerId());
        //HallMain.SendMsgToRoomSer(leaveMsg);
    }

    void LoadResource()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((int)GameKind_Enum.GameKind_ForestDance);
        if (gamedata == null)
            return;

        //AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, gamedata.ResourceABName);
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Forest_Lobby");
        root_ = (GameObject)GameMain.instantiate(obj0);

        GameObject CanvasObj = GameObject.Find("Canvas/Root");
        root_.transform.SetParent(CanvasObj.transform, false);

        Object monkeyObj = (GameObject)bundle.LoadAsset("Anime_Icon_Monkey");
        GameObject monkey = (GameObject)GameMain.instantiate(monkeyObj);
        monkey.transform.SetParent(root_.transform.Find("Middle").Find("Button_1Xinshou"), false);
        Object PandaObj = (GameObject)bundle.LoadAsset("Anime_Icon_Pande");
        GameObject panda = (GameObject)GameMain.instantiate(PandaObj);
        panda.transform.SetParent(root_.transform.Find("Middle").Find("Button_2Gaoshou"), false);
        Object lionObj = (GameObject)bundle.LoadAsset("Anime_Icon_Lion");
        GameObject lion = (GameObject)GameMain.instantiate(lionObj);
        lion.transform.SetParent(root_.transform.Find("Middle").Find("Button_3Zhuangjia"), false);

        GameObject newbieBtn = root_.transform.Find("Middle").Find("Button_1Xinshou").gameObject;
        XPointEvent.AutoAddListener(newbieBtn, OnClickNewbie, monkey);
        GameObject superBtn = root_.transform.Find("Middle").Find("Button_2Gaoshou").gameObject;
        XPointEvent.AutoAddListener(superBtn, OnClickSuper, panda);
        GameObject profasionBtn = root_.transform.Find("Middle").Find("Button_3Zhuangjia").gameObject;
        XPointEvent.AutoAddListener(profasionBtn, OnClickProfasion, lion);
    }
}
