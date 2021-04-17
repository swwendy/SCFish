using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using USocket.Messages;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class CGame_HappyBull : CGameBase
{
    GameObject pickroot_;
    HB_StartGame game_;

    public CGame_HappyBull() : base(GameKind_Enum.GameKind_BullHappy)
    {
		
	}

    public override void Initialization()
    {
        InitHappyBullMsg();
        LoadPickResource();
        InitHappyBullMsg();
    }

    void InitHappyBullMsg()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_SM_CHOOSElEVEL, BackBullHappyChooselevel);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_SM_ENTERROOM, BackBullHappyEnterRoom);
    }

    public override void ProcessTick()
    {
        if (game_ != null)
            game_.Update();
    }

    void LoadPickResource()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullHappy);
        if (gamedata != null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            if (bundle == null)
                return;
            UnityEngine.Object obj0 = bundle.LoadAsset("Niu0_Lobby");
            pickroot_ = (GameObject)GameMain.instantiate(obj0);
            GameObject background = GameObject.Find("Canvas/Root");
            pickroot_.transform.SetParent(background.transform, false);

            GameObject returnBtn = pickroot_.transform.Find("Top").Find("ButtonReturn").gameObject;
            XPointEvent.AutoAddListener(returnBtn, Back2Hall, null);

            int length = HB_DataCenter.Instance().pickdatas.Count;
            LoadPickItem(bundle, length);
            LoadPlayerResource();
        }
    }

    void LoadPickItem(AssetBundle bundle, int length)
    {
        for(int index = 0; index < length; index++)
        {
            UnityEngine.Object itemobj = bundle.LoadAsset("Room_Info");
            GameObject item = (GameObject)GameMain.instantiate(itemobj);
            GameObject background = pickroot_.transform.Find("Middle").
                Find("Middle_Room").Find("Room_Viewport").Find("Room_Content").gameObject; ;
            item.transform.SetParent(background.transform, false);

            int pick = index;
            XPointEvent.AutoAddListener(item, OnPickItem, pick);

            Text minTx = item.transform.Find("ImageMinimum").Find("TextBetNum").GetComponent<Text>();
            minTx.text = HB_DataCenter.Instance().pickdatas[index].minMoney.ToString();
            Text betTx = item.transform.Find("ImageMiniBet").Find("TextBetNum").GetComponent<Text>();
            betTx.text = HB_DataCenter.Instance().pickdatas[index].baseBet.ToString();
        }
    }

    void OnPickItem(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            int index = (int)button;

            UMessage pickmsg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_BULLHAPPY_CM_CHOOSElEVEL);

            pickmsg.Add(GameMain.hall_.GetPlayerId());
            pickmsg.Add((byte)(index + 1));

            HallMain.SendMsgToRoomSer(pickmsg);
        }
    }

    bool BackBullHappyChooselevel(uint _msgType, UMessage msg)
    {
        byte state = msg.ReadByte();

        switch(state)
        {
            case 1:
                break;
            case 2:
                CCustomDialog.OpenCustomConfirmUI(2010);
                break;
            case 3:
                CCustomDialog.OpenCustomConfirmUI(2010);
                break;
        }

        return true;
    }

    bool BackBullHappyEnterRoom(uint _msgType, UMessage msg)
    {
        HB_DataCenter.Instance().roomdata_.roomid = msg.ReadUInt();
        HB_DataCenter.Instance().roomdata_.getMoney = msg.ReadLong();
        HB_DataCenter.Instance().roomdata_.roomLevel = msg.ReadByte();
        HB_DataCenter.Instance().roomdata_.roomState = msg.ReadByte();
        HB_DataCenter.Instance().roomdata_.leftTime = msg.ReadSingle();

        byte length = msg.ReadByte();
        for(int index = 0; index < length; index++)
        {
            HB_PlayerData data = new HB_PlayerData();

            data.playerid = msg.ReadUInt();
            data.faceid = msg.ReadInt();
            string url = msg.ReadString();
            data.totalcoin = msg.ReadLong();
            data.name = msg.ReadString();
            data.sign = msg.ReadByte();

            HB_DataCenter.Instance().roomdata_.playersData.Add(data);
        }

        Go2GamePanel(HB_DataCenter.Instance().roomdata_.roomLevel);

        return true;
    }

    void Go2GamePanel(int index)
    {
        if (game_ == null)
            game_ = new HB_StartGame(pickroot_);
        else
        {
            game_.Start(pickroot_);
        }

        pickroot_.SetActive(false);
    }

    void LoadPlayerResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (GameMain.hall_.GetPlayerData().GetVipLevel() > 0)
        {
            GameObject vip0Img = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").
                    Find("Image_Vip").Find("Vip_Text").Find("Num").gameObject;
            GameObject vipImg = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").
                    Find("Image_Vip").Find("Vip_Text").Find("Num (1)").gameObject;

            Image vipicon = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").
                    Find("Image_Vip").gameObject.GetComponent<Image>();

            vip0Img.SetActive(false);
            vipImg.SetActive(true);
            vipImg.GetComponent<Image>().sprite =
                bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + GameMain.hall_.GetPlayerData().GetVipLevel());

            vipicon.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_jin");
        }

        RefreshPlayerInfo();
    }

    void RefreshPlayerInfo()
    {
        Image icon = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").
            Find("Image_HeadFram").Find("Image_Mask").Find("Image_Head").
            gameObject.GetComponent<Image>();

        icon.sprite = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(), 
            GameMain.hall_.GetPlayerId(), (int)GameMain.hall_.GetPlayerData().PlayerIconId);


        Text playername = pickroot_.transform.Find("Bottom").Find("PlayerInfoBG").
            Find("TextName").gameObject.GetComponent<Text>();
        playername.text = GameMain.hall_.GetPlayerData().GetPlayerName();

        Text diamond = pickroot_.transform.Find("Bottom").Find("Image_DiamondFrame").
            Find("Text_Diamond").gameObject.GetComponent<Text>();
        diamond.text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();

        SetPlayerMoney();
    }

    void SetPlayerMoney()
    {
        Text coin = pickroot_.transform.Find("Bottom").Find("Image_coinframe").
            Find("Text_Coin").gameObject.GetComponent<Text>();
        coin.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
    }

    void Back2Hall(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            GameMain.hall_.SwitchToHallScene();
        }
    }
}
