﻿using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.UI;using UnityEngine.EventSystems;using USocket.Messages;public class CGame_TexasPoker : CGameBase{    GameObject gameroot_;    GameObject pickroot_;    GameObject roomroot_;    GameObject buttonroot_;    Dictionary<uint, GameObject> roomObjects;    public TP_StartGame game_;    public byte currentLevel_ = 1;    List<Sprite> levelTitles;    public CGame_TexasPoker()    {        roomObjects = new Dictionary<uint, GameObject>();                GameMain.hall_.AskForClubData();        LoadResource();        InitPlayerInfo();        SwitchPickButton(true);        InitLevelTitles();        Reset();        InitBackMsg();        if (game_ == null)            game_ = new TP_StartGame(gameroot_, pickroot_);        if (GameMain.hall_.invate_ != null)        {            SwitchPickButton(false);        }    }    void InitLevelTitles()    {        levelTitles = new List<Sprite>();        levelTitles.Add(LoadTitleResource("DZ_word_title_xsc"));        levelTitles.Add(LoadTitleResource("DZ_word_title_gsc"));        levelTitles.Add(LoadTitleResource("DZ_word_title_zjc"));    }    Sprite LoadTitleResource(string spriteName)    {        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_TexasPoker);        if (gamedata == null)            return null;        AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);        if (bundle == null)            return null;        Sprite result = bundle.LoadAsset<Sprite>(spriteName);        return result;    }    void Reset()    {    }    void InitBackMsg()    {        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_TEXASPOKER_SM_CHOOSElEVEL, BackChooseLevel);        CMsgDispatcher.GetInstance().RegMsgDictionary(                (uint)GameCity.EMSG_ENUM.CCMsg_TEXASPOKER_SM_ENTERROOM, BackEnterRoom);    }    bool BackChooseLevel(uint _msgType, UMessage msg)    {        byte state = msg.ReadByte();        byte level = msg.ReadByte();        byte sigh = msg.ReadByte();        CCustomDialog.OpenCustomConfirmUI(2106);        return true;    }    bool BackEnterRoom(uint _msgType, UMessage msg)    {        TP_DataCenter.Instance().tper.ReadMessage(msg);        pickroot_.SetActive(false);        gameroot_.SetActive(true);        return true;    }    void LoadResource()    {        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_TexasPoker);        if (gamedata != null)        {            //AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, gamedata.ResourceABName);            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);            if (bundle == null)                return;            GameObject background = GameObject.Find("Canvas/Root");            UnityEngine.Object obj0 = bundle.LoadAsset("Dezhou_MainUI");            gameroot_ = (GameObject)GameMain.instantiate(obj0);            gameroot_.transform.SetParent(background.transform, false);            gameroot_.SetActive(false);            UnityEngine.Object obj1 = bundle.LoadAsset("Dezhou_Lobby");            pickroot_ = (GameObject)GameMain.instantiate(obj1);            pickroot_.transform.SetParent(background.transform, false);            pickroot_.SetActive(true);            roomroot_ = pickroot_.transform.Find("Middle").Find("Middle_Room").gameObject;            buttonroot_ = pickroot_.transform.Find("Middle").Find("Middle_Button").gameObject;            InitChooseLevelEvents(bundle);        }    }    void InitChooseLevelEvents(AssetBundle bundle)    {        GameObject BsqjBtn = pickroot_.transform.Find("Middle").Find("Middle_Button").Find("Button_1Xinshou").gameObject;        GameObject XycjBtn = pickroot_.transform.Find("Middle").Find("Middle_Button").Find("Button_2Gaoshou").gameObject;        GameObject DzdhBtn = pickroot_.transform.Find("Middle").Find("Middle_Button").Find("Button_3Zhuangjia").gameObject;        XPointEvent.AutoAddListener(BsqjBtn, OnClickRmc, null);        XPointEvent.AutoAddListener(XycjBtn, OnClickGsc, null);        XPointEvent.AutoAddListener(DzdhBtn, OnClickZjc, null);        GameObject returnBtn = pickroot_.transform.Find("Top").Find("ButtonReturn").gameObject;        XPointEvent.AutoAddListener(returnBtn, OnReturn, null);    }    TexasPokerLevelInfo GetLevelInfo(int levelindex)    {        return TP_DataCenter.Instance().tpl.levelinfos[levelindex];    }    void LoadRoomResource(TexasPokerTypeInfo typeinfo)    {        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_TexasPoker);        if (gamedata != null)        {            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);            if (bundle == null)                return;            GameObject roomBG = pickroot_.transform.Find("Middle").Find("Middle_Room").                Find("Room_Viewport").Find("Room_Content").gameObject;            UnityEngine.Object obj = bundle.LoadAsset("Room_Lobby");            GameObject room = (GameObject)GameMain.instantiate(obj);            Text playerNum = room.transform.Find("players_Num").Find("TextNum").gameObject.GetComponent<Text>();            playerNum.text = typeinfo.currentPlayerNum.ToString();            Text carryIn = room.transform.Find("Text_MaxNum").gameObject.GetComponent<Text>();            carryIn.text = typeinfo.maxCarryIn.ToString();            Text blinds = room.transform.Find("Text_ChipNum").gameObject.GetComponent<Text>();            blinds.text = typeinfo.blinds.ToString();            XPointEvent.AutoAddListener(room, OnClicRoom, typeinfo);            room.transform.SetParent(roomBG.transform, false);        }    }    void OnClicRoom(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerUp)        {            TexasPokerTypeInfo typeinfo = (TexasPokerTypeInfo)button;            UMessage pickroom = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_TEXASPOKER_CM_CHOOSElEVEL);            pickroom.Add(GameMain.hall_.GetPlayerId());            pickroom.Add(currentLevel_);            pickroom.Add(typeinfo.sign);            if (HallMain.gametcpclient.IsSocketConnected)            {                HallMain.SendMsgToRoomSer(pickroom);            }        }    }    void OnClickRmc(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerUp)        {            buttonroot_.SetActive(false);            roomroot_.SetActive(true);            currentLevel_ = 1;            TexasPokerLevelInfo levelinfo = GetLevelInfo(0);            for (int index = 0; index < levelinfo.types; index++)            {                LoadRoomResource(levelinfo.typeinfos[index]);            }        }        if (eventtype == EventTriggerType.PointerDown)        {        }    }    void OnClickGsc(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerUp)        {            buttonroot_.SetActive(false);            roomroot_.SetActive(true);            currentLevel_ = 2;            TexasPokerLevelInfo levelinfo = GetLevelInfo(1);            for (int index = 0; index < levelinfo.types; index++)            {                LoadRoomResource(levelinfo.typeinfos[index]);            }        }        if (eventtype == EventTriggerType.PointerDown)        {        }    }    void OnClickZjc(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerUp)        {            buttonroot_.SetActive(false);            roomroot_.SetActive(true);            currentLevel_ = 3;            TexasPokerLevelInfo levelinfo = GetLevelInfo(2);            for (int index = 0; index < levelinfo.types; index++)            {                LoadRoomResource(levelinfo.typeinfos[index]);            }        }        if (eventtype == EventTriggerType.PointerDown)        {        }    }    void OnReturn(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerUp)        {            FIR_AudioDataManager.GetInstance().PlayAudio(1001);            if (roomroot_.activeSelf)                SwitchPickButton(true);            else                GameMain.hall_.SwitchToHallScene();        }    }    void SwitchPickButton(bool ischooselevel)    {        GameObject choosebtn = pickroot_.transform.Find("Middle").Find("Middle_Button").gameObject;        GameObject roombtn = pickroot_.transform.Find("Middle").Find("Middle_Room").gameObject;        choosebtn.SetActive(ischooselevel);        roombtn.SetActive(!ischooselevel);    }    void InitPlayerInfo()    {        //AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, gamedata.ResourceABName);        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)            return;        if (GameMain.hall_.GetPlayerData().GetVipLevel() > 0)        {            GameObject vip0Img = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").                    Find("Image_Vip").Find("Vip_Text").Find("Num").gameObject;            GameObject vipImg = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").                    Find("Image_Vip").Find("Vip_Text").Find("Num (1)").gameObject;            Image vipicon = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").                    Find("Image_Vip").gameObject.GetComponent<Image>();            vip0Img.SetActive(false);            vipImg.SetActive(true);            vipImg.GetComponent<Image>().sprite =                bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + GameMain.hall_.GetPlayerData().GetVipLevel());            vipicon.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_jin");        }        RefreshPlayerInfo();    }    void RefreshPlayerInfo()    {        Image icon = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").            Find("Image_HeadFram").Find("Image_Mask").Find("Image_Head").            gameObject.GetComponent<Image>();        if(Luancher.IsVChatLogin)            icon.sprite = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(),                GameMain.hall_.GetPlayerId(), (int)GameMain.hall_.GetPlayerData().PlayerIconId);        Text playername = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").            Find("TextName").gameObject.GetComponent<Text>();        playername.text = GameMain.hall_.GetPlayerData().GetPlayerName();        Text coin = pickroot_.transform.Find("Bottom").Find("Image_coinframe").            Find("Text_Coin").gameObject.GetComponent<Text>();        coin.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();        Text diamond = pickroot_.transform.Find("Bottom").Find("Image_DiamondFrame").            Find("Text_Diamond").gameObject.GetComponent<Text>();        diamond.text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();    }}