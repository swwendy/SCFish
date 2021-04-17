﻿
using System.Collections;

#region "约据房间 - 游戏规则数据结构"
//斗地主(约据房间界面)
public class LandLordRule
    public byte maxPower;   //最大倍数
}

//掼蛋游戏玩法类型
public enum TePlayType
    Times,    //打几局(不带升级)
}

//掼蛋 --打几局(不带升级) -- 极牌类型
public enum CurrentPoker
    Two = 2,    //固定打2
}

//掼蛋(约据房间界面)
public class ThrowEggsRule

    //游戏玩法类型
    public TePlayType playType;
    //连续打(带升级)
    public byte vectory;
    //打几局 -- 局数
    public byte times;
    //打几局 -- 积分
    public byte score;
    //打几局 -- 极牌
    public CurrentPoker cp;

//血战麻将(约据房间界面)
public class MahjongRule
{
    public MahjongRule()
    {
        times = timesList[0];
        maxPower = maxPowerList[0];
        isAddBet = true;
        isOtherFour = true;
        isOneNine = true;
        isMiddle = true;
        isSkyGroud = true;
    }

    public static byte[] timesList = { 8, 16, 24, 40 };
    public static byte[] maxPowerList = { 4, 8, 16, 250 };

    public byte maxPower;       //最大倍数
    public byte times;          //对战局数
    public bool isAddBet; 		//自摸加底 自摸加翻
    public bool isOtherFour; 	//点杠花（点炮/自摸）
    public bool isOneNine;		//幺九将对
    public bool isMiddle;		//门清中张
    public bool isSkyGroud;		//天地胡
}

//盐城麻将(约据房间界面)
public class YcMahjongRule
{
    public YcMahjongRule()
    {
        times = timesList[0];
    }

    public static byte[] timesList = { 8, 16, 24, 40 };
    public byte times;          //对战局数
}

//常州麻将约据房间数据类型
public enum CzMahjongRuleDataType
    CzWanFa_Type,           //玩法
    CzQiHu_Type,            //起胡
    CzFengDing_Type,        //封顶
    CzDiHua_Type            //底花
}

//常州麻将(约据房间界面)
public class CzMahjongRule
{
    public CzMahjongRule()
    {
        times = timesList[0];
        qiHuNum = huNumList[0];
        fengDingNum = topNumList[0];
        wanFa = new byte[4] { 1, 1, 1, 1 };
        diHuaNum = BottomHuaNumList[0];
    }

    //默认对战局数
    public static byte[] timesList = { 8, 16, 24, 40 };
    ///默认起胡
    public static byte[] huNumList = { 1, 2, 3, 4 };
    //默认封顶 2的4,5,6次方
    public static byte[] topNumList = { 16, 32, 64, 250 };
    //默认底花
    public static byte[] BottomHuaNumList = { 3, 4, 5, 6 };

    //对战局数
    public byte times;
    //玩法 包三口/包四口/擦背/吃
    public byte[] wanFa;
    //起胡
    public byte qiHuNum;
    //封顶
    public byte fengDingNum;
    //底花
    public byte diHuaNum;
}

//够级游戏(够级约据房间)
public class GouJiRule
{
    public GouJiRule()
    {
        times = timesList[0];
        xuanDian = true;
        kaiFaDianShi = true;
        beiShan = true;
        twoShaYi = true;
        jinGong = true;
    }
    //默认对战局数
    public static byte[] timesList = { 8, 16, 24, 40 };

    //对战局数
    public byte times;
    //宣点
    public bool xuanDian;
    //开发点4
    public bool kaiFaDianShi;
    //憋三
    public bool beiShan;
    //大王二杀一
    public bool twoShaYi;
    //是否进贡
    public bool jinGong;
}


//红中麻将(约据房间界面)
public class HzMahjongRule
{
    public HzMahjongRule()
    {
        times = timesList[0];
        birdNum = birdNumList[0];
        dianPaoState = true;
        hongZhongState = true;
    }

    //默认对战局数
    public static byte[] timesList = { 8, 16, 24, 40 };
    ///抓鸟数
    public static byte[] birdNumList = { 2, 4, 6 };

    //对战局数
    public byte times;
    //抓鸟数
    public byte birdNum;
    //可以点炮
    public bool dianPaoState;
    //红中是赖子
    public bool hongZhongState;
}

/// <summary>
/// 象棋约据房间
/// </summary>
public class ChessRule
{
    public ChessRule()
    {
        ChessTimes = timesList[0];
        ChessTime = 5;
    }

    //默认对战局数
    public static byte[] timesList = { 1, 2, 4, 8 };
    //对战局数
    public byte ChessTimes;
    //局时间
    public uint ChessTime;
}
#endregion

public class GameRoom
    //UnityArmatureComponent starteffect;
    byte currentNeedUpdateGameID;

    //开房扩展 私密/公开
    GameObject AppointmentRoomPanel_;
    //游戏房间游戏列表数据
    Dictionary<GameKind_Enum, Toggle> lefttoggles_;
    //游戏约据房间游戏列表数据
    Dictionary<GameKind_Enum, Toggle> rulePanelLeftToggles_;
    //进入约据房间方式
    bool isRuleEnterWay;
        lefttoggles_ = new Dictionary<GameKind_Enum, Toggle>();
        rulePanelLeftToggles_ = new Dictionary<GameKind_Enum, Toggle>();

        playtimesindex_ = 1;
    {
        if (rulepanel_ == null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bundle == null)
                return;

            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("friend_Room_rule");
            rulepanel_ = (GameObject)GameMain.instantiate(obj0);
            GameObject CanvasObj = GameObject.Find("Canvas/Root");
            rulepanel_.transform.SetParent(CanvasObj.transform, false);

            rulepanel_.SetActive(false);
        }
    }
        isopen_ = false;
        LoadRulePanel();
        InitRulePanel();
        RefreshMoney();
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Appointment_SM_Join_1, BackJoinAppointmentGameIDOnly);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Appointment_SM_ClearReady, BackClearReady);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Appointment_SM_PublicRooms, BackPublicRoomData);

    {
        AppointmentDataManager.AppointmentDataInstance().matchrooms_.Clear();
        byte gameid = msg.ReadByte();
        byte length = msg.ReadByte();
        for (int index = 0; index < length; index++)
        {
            MatchRooms mr = new MatchRooms();

            mr.descript_ = msg.ReadString();
            mr.incoin_ = msg.ReadInt();

            AppointmentDataManager.AppointmentDataInstance().matchrooms_.Add(mr);
        }

        List<uint> appointmentRoomIDList = new List<uint>();
        while (true)
        {
            int iscontinue = msg.ReadInt();
            if (iscontinue < 0)
                break;

            AppointmentData publicAppointmentData = GameFunction.CreateAppointmentData((GameKind_Enum)gameid);
            publicAppointmentData.gameid = gameid;
            publicAppointmentData.ReadAppointmentDataMessage(msg,true);
            publicAppointmentData.SetIsOpenRoom(true);
            appointmentRoomIDList.Add(publicAppointmentData.roomid);
            AppointmentDataManager.AppointmentDataInstance().AddAppointmentData(publicAppointmentData.roomid, publicAppointmentData);
        }

        ClearAppointments();

        InitAppointmentRooms(appointmentRoomIDList);

        return true;
    }
    {
        byte state = msg.ReadByte();
        if (state == 0)
            CCustomDialog.OpenCustomConfirmUI(1654);
        if (state == 1)
            CCustomDialog.OpenCustomConfirmUI(1655);

        AppointmentData currentroom = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();

        foreach (byte key in currentroom.seats_.Keys)

        InitAppointmentData();

        return true;
    }

        GameMain.hall_.SetGameRecordMsgState(GameRecordMsgState.RecordMsg_End);
                    else
                        powerText = " 打" + data.gameTimes + "局 打" + data.maxpower.ToString();
                         data.gameID == (byte)GameKind_Enum.GameKind_CzMahjong || data.gameID == (byte)GameKind_Enum.GameKind_GouJi ||
                         data.gameID == (byte)GameKind_Enum.GameKind_HongZhong)
                {
                    powerText = " 打" + data.gameTimes + "局";

                    data.gamerule = CCsvDataManager.Instance.GameDataMgr.GetGameData(data.gameID).GameName + powerText;
                }
        else
        {
            recordpanel_.SetActive(true);
            RefreshRecordData();
        }
            AppointmentRecord data = recordlist_[index];

            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Lobby_Roomrecord_info");
            GameObject recorditem = (GameObject)GameMain.instantiate(obj0);
            GameObject recorditemBG = recordpanel_.transform.Find("middle").
                Find("Viewport_record").Find("Content_record").gameObject;
            recorditem.transform.SetParent(recorditemBG.transform, false);

            XPointEvent.AutoAddListener(recorditem, OnOOCRecordItem, index);

            Image gameicon = recorditem.transform.Find("Image_gaikuang").Find("TextGame").gameObject.GetComponent<Image>();
            gameicon.sprite = bundle.LoadAsset<Sprite>(CCsvDataManager.Instance.GameDataMgr.GetGameData(data.gameID).GameTextIcon);
            Text appointmentTime = recorditem.transform.Find("Image_gaikuang").Find("TextTime").gameObject.GetComponent<Text>();
            //Debug.Log("data seconds :" + data.timeseconds.ToString());
            System.DateTime sdt = GameCommon.ConvertLongToDateTime(data.timeseconds);
            appointmentTime.text = sdt.ToString("yyyy年MM月dd日HH:mm");
            Text appointmentRule = recorditem.transform.Find("Image_gaikuang").Find("TextRule").gameObject.GetComponent<Text>();
            //string powerText = "";
            //if (data.maxpower == 250)
            //    powerText = "不封顶";
            //else
            //    powerText = "最大" + data.maxpower + "倍";
            //appointmentRule.text = CCsvDataManager.Instance.GameDataMgr.GetGameData(data.gameID).GameName + " " + data.gameTimes + "局 " + powerText;
            appointmentRule.text = data.gamerule;

            Text selfcoinTx = recorditem.transform.Find("Image_gaikuang").Find("TextJifen").gameObject.GetComponent<Text>();
            if (data.result.ContainsKey(GameMain.hall_.GetPlayerId()))
            {
                if (data.result[GameMain.hall_.GetPlayerId()].coin >= 0)
                {
                    selfcoinTx.text = "+" + data.result[GameMain.hall_.GetPlayerId()].coin.ToString();
                    selfcoinTx.color = new Color(217.0f / 255.0f, 59.0f / 255.0f, 42.0f / 255.0f);
                }
                else
                {
                    selfcoinTx.text = data.result[GameMain.hall_.GetPlayerId()].coin.ToString();
                    selfcoinTx.color = new Color(89.0f / 255.0f, 130.0f / 255.0f, 188.0f / 255.0f);
                }
            }
            else
            {
                selfcoinTx.text = "";
            }

            int playerIndex = 0;
            UnityEngine.Transform recordPlayerTransform = null;
            UnityEngine.Transform detailBGTransform = recorditem.transform.Find("Image_xiangqing");
            foreach(var recordData in data.result)
            {
                recordPlayerTransform = detailBGTransform.Find("playerinfo_" + (playerIndex +1));
                if(recordPlayerTransform == null)
                {
                    Debug.Log("录像玩家人数超过资源上限!");
                    break;
                }
                Image playerIcon = recordPlayerTransform.Find("Image_HeadBG/Image_HeadMask/Image_HeadImage").GetComponent<Image>();
                playerIcon.sprite = GameMain.hall_.GetIcon(recordData.Value.url, recordData.Value.playerid, recordData.Value.faceid);
                recordPlayerTransform.transform.Find("TextName").GetComponent<Text>().text = recordData.Value.playerName;

                Text playerCoin = recordPlayerTransform.Find("TextJifen").gameObject.GetComponent<Text>();
                if (recordData.Value.coin >= 0)
                {
                    playerCoin.text = "+" + recordData.Value.coin.ToString();
                    playerCoin.color = new Color(217.0f / 255.0f, 59.0f / 255.0f, 42.0f / 255.0f);
                }
                else
                {
                    playerCoin.text = recordData.Value.coin.ToString();
                    playerCoin.color = new Color(89.0f / 255.0f, 130.0f / 255.0f, 188.0f / 255.0f);
                }
                detailBGTransform.GetChild(playerIndex).gameObject.SetActive(true);
                ++playerIndex;
            }

            UnityEngine.Transform childTransform = null;
            for(; playerIndex < detailBGTransform.childCount; ++playerIndex)
            {
                childTransform = detailBGTransform.Find("playerinfo_" + (playerIndex + 1));
                if (childTransform == null)
                {
                    break;
                }
                childTransform.gameObject.SetActive(false);
            }

            GameObject videoBtn = recorditem.transform.Find("Image_xiangqing/Button_video").gameObject;
            XPointEvent.AutoAddListener(videoBtn, OnClickWatchVideo, data);

            recorditems_.Add(recorditem);
        }

    //观看录像
    private void OnClickWatchVideo(EventTriggerType eventtype, object button, PointerEventData eventData)

            //按下录像按钮后要做的事情......
            GameVideo.GetInstance().OnClickWatch(record);
            AppointmentDataManager.AppointmentDataInstance().interruptid > 0;
        {
            if (AppointmentDataManager.AppointmentDataInstance().interruptid == value.playerid)
            {
                AppointmentDataManager.AppointmentDataInstance().interruptName = value.playerName;
                break;
            }
        }


        byte round = msg.ReadByte();
        //if (round <= 1)
        //{
        //    Back2Hall();
        //}

        {
            Dictionary<byte, AppointmentResult> perresult = new Dictionary<byte, AppointmentResult>();
            for (int playerindex = 0; playerindex < playernumber; playerindex++)
            {
                AppointmentResult result = new AppointmentResult();

                result.playerid = msg.ReadUInt();
                result.coin = msg.ReadInt();
                result.sitNo = msg.ReadByte();

                perresult.Add(result.sitNo, result);
            }

            AppointmentDataManager.AppointmentDataInstance().perResultList_.Add(perresult);
        }
        AppointmentDataManager.AppointmentDataInstance().playerAlready = false;

        //与游戏服断开连接
        //UMessage exitmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_PLAYERLEAVEROOMSER);
        //exitmsg.Add(GameMain.hall_.GetPlayerId());
        //HallMain.SendMsgToRoomSer(exitmsg);

        return true;


        //GameObject cancelbtn = alreadyPanel.transform.FindChild("Text_yizhunbei").gameObject;
        //cancelbtn.GetComponent<Button>().interactable = true;

        InitAppointmentData();


        //byte[] uiname = { 0, 1, 3, 2 };
        //targetseatid = uiname[targetseatid];

        if (alreadyPanel == null)
        foreach (byte key in currentroom.seats_.Keys)
        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
        {
            Back2Hall();
            return;
        }

        //AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        //if (bundle == null)
        //    return;

        AppointmentData data = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();


        string gameRuleTextData = null;
        Text tableTx = alreadyPanel.transform.Find("table/Text_Rule").gameObject.GetComponent<Text>();
        tableTx.text = gameRuleTextData;
        }
                    //CCustomDialog.OpenCustomConfirmUI(1623);
                    break;

        GameMain.hall_.GameBaseObj.StartLoad();
        appointmentID_ = 0;
        return true;
            {
                CCustomDialog.OpenCustomConfirmUI(2106, Back2Hall);
            }else if (state == 251)
            {
                if (GameMain.hall_.GetCurrentGameState() == GameState_Enum.GameState_Appointment)
                else
                    CCustomDialog.OpenCustomConfirmUI(1619);
            }
            AppointmentData publicAppointmentData = GameFunction.CreateAppointmentData(gamekind);
            publicAppointmentData.gameid = AppointmentDataManager.AppointmentDataInstance().gameid;
            publicAppointmentData.ReadAppointmentDataMessage(msg);
            publicAppointmentData.SetIsOpenRoom(false);

            publicAppointmentData.roomid = appointmentid;
            publicAppointmentData.hostid = hostid;

            publicAppointmentData.seats_[state].playerid = playerid;
            publicAppointmentData.seats_[state].playerName = GameMain.hall_.GetPlayerData().GetPlayerName();
            publicAppointmentData.seats_[state].icon = (int)GameMain.hall_.GetPlayerData().PlayerIconId;
            publicAppointmentData.seats_[state].url = GameMain.hall_.GetPlayerData().GetPlayerIconURL();

            AppointmentDataManager.AppointmentDataInstance().AddAppointmentData(appointmentid, publicAppointmentData);
            AppointmentDataManager.AppointmentDataInstance().currentRoomID = appointmentid;
            AppointmentDataManager.AppointmentDataInstance().currentsource = state;
            AppointmentDataManager.AppointmentDataInstance().playerSitNo = state;
                if (userSitNo == AppointmentDataManager.AppointmentDataInstance().playerSitNo)
                    AppointmentDataManager.AppointmentDataInstance().playerAlready = already;

            RefreshJoin(appointmentid, playerid, playerName, faceid, state, url, already);
        appointmentData.seats_[sitNo].playerName = playerName;
        publicAppointmentData.gameid = AppointmentDataManager.AppointmentDataInstance().gameid;
        publicAppointmentData.ReadAppointmentDataMessage(msg);

        publicAppointmentData.roomid = roomid;
        publicAppointmentData.hostid = GameMain.hall_.GetPlayerId();
        publicAppointmentData.seats_[0].playerid = GameMain.hall_.GetPlayerId();
        publicAppointmentData.seats_[0].playerName = GameMain.hall_.GetPlayerData().GetPlayerName();
        publicAppointmentData.seats_[0].icon = (int)GameMain.hall_.GetPlayerData().PlayerIconId;
        publicAppointmentData.seats_[0].url = GameMain.hall_.GetPlayerData().GetPlayerIconURL();
        AppointmentDataManager.AppointmentDataInstance().currentsource = 0;

        #region "废弃代码"
        //Text coin = rulepanel_.transform.FindChild("Top").FindChild("Image_coinframe").FindChild("Text_Coin").gameObject.GetComponent<Text>();
        //coin.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
        //Text diamond = rulepanel_.transform.FindChild("Top").FindChild("Image_DiamondFrame").FindChild("Text_Diamond").gameObject.GetComponent<Text>();
        //diamond.text = (GameMain.hall_.GetPlayerData().GetDiamond() + GameMain.hall_.GetPlayerData().GetCoin()).ToString();

        //GameObject diamondBtn = rulepanel_.transform.FindChild("Top").FindChild("Image_DiamondFrame").gameObject;
        //XPointEvent.AutoAddListener(diamondBtn, GameMain.hall_.Charge, Shop.SHOPTYPE.SHOPTYPE_DIAMOND);
        //GameObject coinBtn = rulepanel_.transform.FindChild("Top").FindChild("Image_coinframe").gameObject;
        //XPointEvent.AutoAddListener(coinBtn, GameMain.hall_.Charge, Shop.SHOPTYPE.SHOPTYPE_COIN);
        #endregion

        rulePanelLeftToggles_.Clear();
        bool initCurGameKindState = false;
        UnityEngine.Transform GameKindTransform = null;
        for (GameKind_Enum gameKind = GameKind_Enum.GameKind_CarPort; gameKind < GameKind_Enum.GameKind_Max; ++gameKind)
        {
            GameKind_Enum curGameKind = gameKind;
            string toggleName = "Left/Imagebg/Toggle_" + (byte)curGameKind;
            GameKindTransform = rulepanel_.transform.Find(toggleName);
            if (GameKindTransform == null)
            {
                continue;
            }

            bool activeState = GameMain.hall_.GetGameIcon_ImageIcon((byte)curGameKind) != null;
            Toggle leftToggle = GameKindTransform.GetComponent<Toggle>();
            leftToggle.isOn = false;
            leftToggle.onValueChanged.AddListener(delegate (bool value) { ChangeRuleInfo(value, curGameKind); });
            rulePanelLeftToggles_.Add(curGameKind, leftToggle);
            GameKindTransform.gameObject.SetActive(activeState);
            if(activeState && !initCurGameKindState)
            {
                currentpickrooms_ = gameKind;
                initCurGameKindState = true;
            }
        }
        {
            rulePanelLeftToggles_[currentpickrooms_].isOn = true;
        }
        rulepanel_.transform.Find("Left").gameObject.SetActive(initCurGameKindState);
        rulepanel_.transform.Find("Open").gameObject.SetActive(initCurGameKindState);
        rulepanel_.transform.Find("Imagekong").gameObject.SetActive(!initCurGameKindState);

    void OpenRoomType()
    {
        GameObject openbg = rulepanel_.transform.Find("Open/ImageBG").gameObject;

        Toggle opentoggle = openbg.transform.GetChild(0).gameObject.GetComponent<Toggle>();
        opentoggle.isOn = isopen_;
        opentoggle.onValueChanged.AddListener(delegate (bool call) { CustomAudio.GetInstance().PlayCustomAudio(1004);  isopen_ = call; });

        Toggle closetoggle = openbg.transform.GetChild(1).gameObject.GetComponent<Toggle>();
        closetoggle.isOn = !isopen_;
        closetoggle.onValueChanged.AddListener(delegate (bool call) { CustomAudio.GetInstance().PlayCustomAudio(1004); isopen_ = !call; });
    }
    {
        if(null == rulepanel_)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_7");
        UnityEngine.Transform llfirstGroupTransform = ruleTransform.Find("jushu/ImageBG");
                {
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                llr_.maxPower = LandLordRule.maxPowerList[temp];
            });
    }
    {
        if(rulepanel_ == null)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_13");
        UnityEngine.Transform tefirstGroupTransform = ruleTransform.Find("wanfa/ImageBG");
            {
            });
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                ter_.vectory = ThrowEggsRule.victoryList[temp];
            });
                {
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                ter_.score = ThrowEggsRule.scoreList[temp];
            });

        UnityEngine.Transform tefifthGroupTransform = ruleTransform.Find("jiju/jipai/ImageBG");
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                ter_.cp = (CurrentPoker)temp;
            });

    //初始化血战麻将约据房间规则事件
    void InitRightRuleMahjongEvents()
    {
        if (rulepanel_ == null)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_12");
        UnityEngine.Transform mjfirstGroupTransform = ruleTransform.Find("jushu/ImageBG");
        {
            Toggle toggle = mjfirstGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                mjr_.times = MahjongRule.timesList[temp];
            });
        }

        UnityEngine.Transform mjsecondGroupTransform = ruleTransform.Find("beishu/ImageBG");
        {
            Toggle toggle = mjsecondGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                mjr_.maxPower = MahjongRule.maxPowerList[temp];
            });
        }

        UnityEngine.Transform mjthirdGroupTransform = ruleTransform.Find("wanfa_1/ImageBG");
        {
            Toggle toggle = mjthirdGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;
            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                if (temp == 0)
                    mjr_.isAddBet = value;
                if (temp == 1)
                    mjr_.isAddBet = !value;
            });
        }
        {
            Toggle toggle = mjforthGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;
            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                if (temp == 0)
                    mjr_.isOtherFour = value;
                if (temp == 1)
                    mjr_.isOtherFour = !value;
            });
        }

        UnityEngine.Transform mjfifthGroupTransform = ruleTransform.Find("wanfa_3/ImageBG");
        toggle0.onValueChanged.AddListener(delegate (bool value)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1004);
            mjr_.isOneNine = value;
        });

        Toggle toggle1 = mjfifthGroupTransform.GetChild(1).gameObject.GetComponent<Toggle>();
        toggle1.onValueChanged.AddListener(delegate (bool value)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1004);
            mjr_.isMiddle = value;
        });

        Toggle toggle2 = mjfifthGroupTransform.GetChild(2).gameObject.GetComponent<Toggle>();
        toggle2.onValueChanged.AddListener(delegate (bool value)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1004);
            mjr_.isSkyGroud = value;
        });
    }
    {
        if (rulepanel_ == null)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_14");
        UnityEngine.Transform ycmjfirstGropTransform = ruleTransform.Find("jushu/ImageBG");
        {
            Toggle toggle = ycmjfirstGropTransform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                if (!value)
                    return;
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                ycmjr_.times = YcMahjongRule.timesList[temp];
            });
        }
    }

    //常州麻将约据房间Toggle组件事件
    void OnRuleCzMahjongRoomToggleEvent(CzMahjongRuleDataType dataType,bool isOnValue,int index)
    {
        CustomAudio.GetInstance().PlayCustomAudio(1004);
        switch(dataType)
        {
            case CzMahjongRuleDataType.CzJuShu_Type:
                czmjr_.times = CzMahjongRule.timesList[index];
                break;
            case CzMahjongRuleDataType.CzWanFa_Type:
                czmjr_.wanFa[index] = (byte)(isOnValue ? 1 : 0);
                break;
            case CzMahjongRuleDataType.CzQiHu_Type:
                czmjr_.qiHuNum = CzMahjongRule.huNumList[index];
                break;
            case CzMahjongRuleDataType.CzFengDing_Type:
                czmjr_.fengDingNum = CzMahjongRule.topNumList[index];
                break;
            case CzMahjongRuleDataType.CzDiHua_Type:
                czmjr_.diHuaNum = CzMahjongRule.BottomHuaNumList[index];
                break;
        }
    }
    {
        UnityEngine.Transform czGropTransform = ruleTransform.Find(pathName);
        {
            Toggle toggle = czGropTransform.GetChild(index).gameObject.GetComponent<Toggle>();
            int tempIndex = index;
            toggle.onValueChanged.AddListener(delegate(bool isOnValue)
            {
                if (dataType != CzMahjongRuleDataType.CzWanFa_Type && !isOnValue)
                    return;
                OnRuleCzMahjongRoomToggleEvent(dataType, isOnValue, tempIndex);
            });
        }
    }
    {
        if (rulepanel_ == null)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_15");
        //对战局数
        RuleCzMahjongRoomEvent(ruleTransform, "jushu/ImageBG", CzMahjongRuleDataType.CzJuShu_Type);

        //玩法
        RuleCzMahjongRoomEvent(ruleTransform, "wanfa_0/ImageBG", CzMahjongRuleDataType.CzWanFa_Type);

        //起胡
        RuleCzMahjongRoomEvent(ruleTransform, "qihu/ImageBG", CzMahjongRuleDataType.CzQiHu_Type);

        //封顶
        RuleCzMahjongRoomEvent(ruleTransform, "fengding/ImageBG", CzMahjongRuleDataType.CzFengDing_Type);

        //底花
        RuleCzMahjongRoomEvent(ruleTransform, "dihua/ImageBG", CzMahjongRuleDataType.CzDiHua_Type);
    }

    //够级
    {
        if (rulepanel_ == null)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_17");
        //局数
        UnityEngine.Transform juShuTransform = ruleTransform.Find("jushu/ImageBG");
        for(int index = 0; index < juShuTransform.childCount; ++index)
        {
            int juShuIndex = index;
            Toggle toggle = juShuTransform.GetChild(index).GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((state) => 
            {
                if(state)
                {
                    gouji_.times = GouJiRule.timesList[juShuIndex];
                }
            });
        }
        //玩法
        UnityEngine.Transform wanFaTransform = ruleTransform.Find("wanfa_0/ImageBG");
        wanFaTransform.GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener((state)=> { gouji_.xuanDian = state;});
        wanFaTransform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((state) => { gouji_.kaiFaDianShi = state; });
        wanFaTransform.GetChild(2).GetComponent<Toggle>().onValueChanged.AddListener((state) => { gouji_.beiShan = state; });
        wanFaTransform = ruleTransform.Find("wanfa_1/ImageBG");
        wanFaTransform.GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener((state) => { gouji_.twoShaYi = state; });
        wanFaTransform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((state) => { gouji_.jinGong = state; });
    }
    {
        if (rulepanel_ == null)
        {
            return;
        }
        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_18");

        //局数
        UnityEngine.Transform juShuTransform = ruleTransform.Find("jushu/ImageBG");
        for (int index = 0; index < juShuTransform.childCount; ++index)
        {
            int juShuIndex = index;
            Toggle toggle = juShuTransform.GetChild(index).GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((state) =>
            {
                if (state)
                {
                    hongZhong_.times = HzMahjongRule.timesList[juShuIndex];
                }
            });
        }

        //抓鸟
        UnityEngine.Transform zhuaNiaoTransform = ruleTransform.Find("niaoshu/ImageBG");
        for (int index = 0; index < zhuaNiaoTransform.childCount; ++index)
        {
            int zhuaNiaoIndex = index;
            Toggle toggle = zhuaNiaoTransform.GetChild(index).GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((state) =>
            {
                if (state)
                {
                    hongZhong_.birdNum = HzMahjongRule.birdNumList[zhuaNiaoIndex];
                }
            });
        }

        //玩法
        UnityEngine.Transform wanFaTransform = ruleTransform.Find("wanfa_0/ImageBG");
        wanFaTransform.GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener((state) => { hongZhong_.dianPaoState = state; });
        wanFaTransform.GetChild(1).GetComponent<Toggle>().onValueChanged.AddListener((state) => { hongZhong_.hongZhongState = state; });
    }
    /// 象棋
    /// </summary>
    {
        if (rulepanel_ == null)
        {
            return;
        }
        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_20");

        //局数
        UnityEngine.Transform juShuTransform = ruleTransform.Find("jushu/ImageBG");
        for (int index = 0; index < juShuTransform.childCount; ++index)
        {
            int juShuIndex = index;
            Toggle toggle = juShuTransform.GetChild(index).GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((state) =>
            {
                if (state)
                {
                    chessRule_.ChessTimes = ChessRule.timesList[juShuIndex];
                }
            });
        }

        //局时
        UnityEngine.Transform juShiTransform = ruleTransform.Find("beishu/ImageBG/InputField");
        InputField juShiInputField = juShiTransform.GetComponent<InputField>();
        {
            uint.TryParse(times, out chessRule_.ChessTime);
            chessRule_.ChessTime = (uint)Mathf.Clamp(chessRule_.ChessTime, 1, 1440);
            juShiInputField.text = chessRule_.ChessTime.ToString();
        });
        juShiInputField.onValueChanged.Invoke(chessRule_.ChessTime.ToString());
    }
        {
            return;
        }
        createBtn0.onClick.RemoveAllListeners();
        //斗地主
        InitRightRuleLandLordsEvents();
        //掼蛋
        InitRightRuleGuanDanEvents();
        //盐城麻将
        InitRightRuleYcMahjongEvents();
        //常州麻将
        InitRightRuleCzMahjongEvents();
        //够级游戏
        InitRightRuleGouJiEvents();
        //红中游戏
        InitRightRuleHzMahjongEvents();
        //象棋
        InitRightRuleChessEvent();
    }
        if (!ison)
        }
        CustomAudio.GetInstance().PlayCustomAudio(1003);
            {
                gameRuleTransform.gameObject.SetActive(selectGameKind == gameKind);
            }

        AppointmentDataManager.AppointmentDataInstance().gameid = (byte)selectGameKind;

        if (!AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_.Keys.Contains(gameid))
        {
            return;
        }


        playtimesindex_ = (byte)levelindex;

            NetWorkClient.GetInstance().SendMsg(tickmsg);

    /// <summary>
    /// 获取局数和最大倍数
    /// </summary>
    /// <param name="type"> ture : 局数 false :倍数</param>
    /// <param name="gameID">游戏ID</param>
    /// <returns>局数或最大倍数</returns>
    byte GetJuShuORPower(bool type, GameKind_Enum gameID)
    {
        byte returnValue = 0;
        switch(gameID)
        {
            case GameKind_Enum.GameKind_LandLords:
                returnValue = type? llr_.playTimes : llr_.maxPower;
                break;
            case GameKind_Enum.GameKind_GuanDan:
                break;
            case GameKind_Enum.GameKind_Mahjong:
                returnValue = type ? mjr_.times:mjr_.maxPower;
                break;
            case GameKind_Enum.GameKind_YcMahjong:
                returnValue = type ? ycmjr_.times : returnValue;
                break;
            case GameKind_Enum.GameKind_CzMahjong:
                returnValue = type ? czmjr_.times : czmjr_.fengDingNum;
                break;
            case GameKind_Enum.GameKind_GouJi:
                returnValue = type ? gouji_.times : (byte)250;
                break;
            case GameKind_Enum.GameKind_HongZhong:
                returnValue = type ? hongZhong_.times : (byte)250;
                break;
            case GameKind_Enum.GameKind_Chess:
                returnValue = type ? chessRule_.ChessTimes : (byte)250;
                break;
        }
        return returnValue;
    }

    //创建房间
    public void OnCreateGameRoom()

        byte gameid = AppointmentDataManager.AppointmentDataInstance().gameid;

        if(!AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_.Keys.Contains(gameid))
        {
            return;
        }

        int power = AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_[gameid].datas[0];
        int level = AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_[gameid].datas[playtimesindex_];

        int pay4appointment = power * level;
        {
            CCustomDialog.OpenCustomConfirmUI(1617);
            return;
        }

        createmsg.Add((byte)0);
        createmsg.Add(playtimesindex_);
        createmsg.Add(GetJuShuORPower(true, (GameKind_Enum)gameid));
        createmsg.Add(GetJuShuORPower(false, (GameKind_Enum)gameid));
        int wanFa = 0;
        {
            case GameKind_Enum.GameKind_GuanDan:
                createmsg.Add((byte)ter_.playType);
                if (ter_.playType == TePlayType.Goon)
                {
                    createmsg.Add(ter_.vectory);
                }
                if (ter_.playType == TePlayType.Times)
                {
                    createmsg.Add(ter_.times);
                    createmsg.Add(ter_.score);
                    createmsg.Add((byte)ter_.cp);
                }
                break;
            case GameKind_Enum.GameKind_Mahjong:
                if (mjr_.isAddBet)
                    GameKind.AddFlag(0, ref wanFa);
                if (mjr_.isOtherFour)
                    GameKind.AddFlag(1, ref wanFa);
                if (mjr_.isOneNine)
                    GameKind.AddFlag(2, ref wanFa);
                if (mjr_.isMiddle)
                    GameKind.AddFlag(3, ref wanFa);
                if (mjr_.isSkyGroud)
                    GameKind.AddFlag(4, ref wanFa);
                createmsg.Add((ushort)wanFa);
                break;
            case GameKind_Enum.GameKind_CzMahjong:
                //玩法 包三口/包四口/擦背/吃
                for (int index = 0; index < czmjr_.wanFa.Count();++index)
                {
                    if(czmjr_.wanFa[index] == 0)
                    {
                        continue;
                    }
                    GameKind.AddFlag(index, ref wanFa);
                }
                createmsg.Add((ushort)wanFa);
                //起胡
                createmsg.Add(czmjr_.qiHuNum);

                //底花
                createmsg.Add(czmjr_.diHuaNum);

                break;
            case GameKind_Enum.GameKind_GouJi:
                if(gouji_.xuanDian)
                    GameKind.AddFlag(0, ref wanFa);
                if (gouji_.kaiFaDianShi)
                    GameKind.AddFlag(1, ref wanFa);
                if (gouji_.beiShan)
                    GameKind.AddFlag(2, ref wanFa);
                if (gouji_.twoShaYi)
                    GameKind.AddFlag(3, ref wanFa);
                if (gouji_.jinGong)
                    GameKind.AddFlag(4, ref wanFa);

                createmsg.Add((byte)wanFa);
                break;

            case GameKind_Enum.GameKind_HongZhong:
                if (hongZhong_.hongZhongState)
                    GameKind.AddFlag(0, ref wanFa);
                if (hongZhong_.dianPaoState)
                    GameKind.AddFlag(1, ref wanFa);
                createmsg.Add((ushort)wanFa);
                createmsg.Add(hongZhong_.birdNum);
                break;
            case GameKind_Enum.GameKind_Chess:
                createmsg.Add(chessRule_.ChessTime * 60);
                break;
        }
        groupTransform.Find("Button_chuangjian").GetComponent<Button>().interactable = false;
        groupTransform.Find("Button_chuangjian_0").GetComponent<Button>().interactable = false;


    public void OnShowNumberPanel(EventTriggerType eventtype, object button, PointerEventData eventData)

    public void OnShowRulePanel(EventTriggerType eventtype, object button, PointerEventData eventData)
            if (!isRuleEnterWay)
            {
                AppointmentRoomPanel_.SetActive(true);
        }

    {
        AppointmentDataManager.AppointmentDataInstance().gameid = msg.ReadByte();
        if (AppointmentDataManager.AppointmentDataInstance().gameid  == 251)
        {
            AppointmentDataManager.AppointmentDataInstance().gameid = (byte)GameKind_Enum.GameKind_LandLords;
            CCustomDialog.OpenCustomConfirmUI(1619);
            appointmentID_ = 0;
        }else if(AppointmentDataManager.AppointmentDataInstance().gameid == 250)
        {
            CCustomDialog.OpenCustomConfirmUI(2106);
        }
        else
        {
            byte state = UpdateGameResource();
            if (state == 0)
            {
                //Join2Msg();
                GameKind_Enum GameKindEnum = (GameKind_Enum)AppointmentDataManager.AppointmentDataInstance().gameid;
                if (GameKindEnum <= GameKind_Enum.GameKind_None || GameKindEnum >= GameKind_Enum.GameKind_Max)
                {
                    Debug.Log("约据模式加入房间游戏模式非法！");
                    return false;
                }
                GameMain.hall_.EnterGameScene(GameKindEnum, GameTye_Enum.GameType_Appointment, 0, () => { Join2Msg(); });
            }
            else
                isUpdateJoin_ = state != 0;
        }
      
        return true;
    }
    /// 快速加入比赛或约据
    /// </summary>
    /// <param name="id"></param>
        GameMain.ST(FastSendJoinMsgEnumerator);
        FastSendJoinMsgEnumerator = SendJoinMsgEnumerator(id);
        GameMain.SC(FastSendJoinMsgEnumerator);
    {
        NumberPanel.GetInstance().SetNumberPanelActive(false);
        CCustomDialog.OpenCustomWaitUI("正在加入中...");
        yield return new WaitForSecondsRealtime(0.3f);
        int SendCount = 12;
        if (id >= 50000)
        {
            if(AppointmentRoomPanel_)
            {
                if(AppointmentRoomPanel_.activeSelf)
                {
                    AppointmentRoomPanel_.SetActive(false);
                }
            }

            GameMain.hall_.Go2ContestHall(2);
            CCustomDialog.OpenCustomWaitUI("正在加入中...");
            if (GameMain.hall_.GetPlayerData().signedContests.Contains(id))
            {
                CCustomDialog.OpenCustomConfirmUI(1666);
                CCustomDialog.CloseCustomWaitUI();
                yield break;
            }
            GameMain.hall_.contest.root_.transform.Find("Right").gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(1f);
            while(SendCount > 0)
            {
                if (GameMain.hall_.selfcontest_.CheckSendJoinMsg(id, false))
                {
                    break;
                }
                --SendCount;
                yield return new WaitForSecondsRealtime(1);
            }
            GameMain.hall_.selfcontest_.SendJoinMsg(id);
            GameMain.hall_.contest.root_.transform.Find("Right").gameObject.SetActive(true);
        }
        else
        {
            while (SendCount > 0)
            {
                if (SendAppintmentJoinMsg(id))
                {
                    break;
                }
                --SendCount;
                yield return new WaitForSecondsRealtime(1);
            }
        }
        CCustomDialog.CloseCustomWaitUI();
        yield break;
    }
    {
        if (appointmentID_ != 0)
        {
            return false;
        }

        AppointmentData appointmentData = AppointmentDataManager.AppointmentDataInstance().GetAppointmentData(appointmentID);
        if (appointmentData != null)
        {
            if (appointmentData.currentPeople >= appointmentData.maxPlayer)
            {
                if(tipState)
                {
                    CCustomDialog.OpenCustomConfirmUI(2106);
                }
                return false;
            }
        }

        AppointmentDataManager.AppointmentDataInstance().kind = AppointmentKind.From_Appointment;

        UMessage joinmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Join_1);
        joinmsg.Add(GameMain.hall_.GetPlayerId());
        joinmsg.Add(appointmentID);
        NetWorkClient.GetInstance().SendMsg(joinmsg);
        return true;
    }
        UnityEngine.Transform TableTransform = alreadyPanel.transform.Find("table");
        if(TableTransform)
        {
            UnityEngine.Transform Button_sitTransform = null;
            for (int childIndex = 0; childIndex < TableTransform.childCount;++childIndex)
            {
                Button_sitTransform = TableTransform.GetChild(childIndex).Find("Button_sit");
                if(Button_sitTransform)
                {
                    Button_sitTransform.gameObject.SetActive(false);
                }
            }
        }

        Button readyBtn = alreadyPanel.transform.Find("Button_Ready").GetComponent<Button>();
        readyBtn.interactable = false;
        readyBtn.onClick.RemoveAllListeners();
        readyBtn.onClick.AddListener(OnReady);

        //LoadStartEffect();

        return alreadyPanel;
        {
            Back2Hall();
            return;
        }

        //XPointEvent.AutoAddListener(invatebtn, OnInvateVX, null);
        invatebtn.SetActive(false);

        //GameObject cancelbtn = appointmentPanel_.transform.FindChild("Text_yizhunbei").gameObject;
        //XPointEvent.AutoAddListener(cancelbtn, OnCancel, null);
    }
                        + AppointmentDataManager.AppointmentDataInstance().gameid
                        + "&type=" + (byte)GameTye_Enum.GameType_Appointment
                        + "&room=" + roomid;
            Player.ShareURLToWechat(url,
                CCsvDataManager.Instance.GameDataMgr.GetGameData(AppointmentDataManager.AppointmentDataInstance().gameid).GameName +

            //readymsg.Add(GameMain.hall_.GetPlayerId());
            readymsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);

            //GameObject cancelbtn = appointmentPanel_.transform.FindChild("Text_yizhunbei").gameObject;
            //cancelbtn.GetComponent<Button>().interactable = false;
        }

        //readymsg.Add(GameMain.hall_.GetPlayerId());
        readymsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);
    {
        alreadyPanel.transform.SetAsLastSibling();
        GameObject readybtn = alreadyPanel.transform.Find("Button_Ready").gameObject;

            NetWorkClient.GetInstance().SendMsg(tickmsg);
            //for (uint index = 1;
            //    index < AppointmentDataManager.AppointmentDataInstance().
            //    GetCurrentAppointment().maxPlayer + 1;
            //    index++)
            //{
            //    GameObject seatBG = alreadyPanel.transform.FindChild("table").gameObject;
            //    GameObject switchbtn = seatBG.transform.FindChild("Player_" + index.ToString()).FindChild("Button_change").gameObject;
            //    switchbtn.SetActive(//tindex == index - 1 && 
            //        GameMain.hall_.GetPlayerId() != AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)(index - 1)].playerid);
            //}
        }
    {
        if (alreadyPanel == null)
            return;

        for (uint index = 1;
            index < AppointmentDataManager.AppointmentDataInstance().
            GetCurrentAppointment().maxPlayer + 1;
            index++)
        {
            GameObject seatBG = alreadyPanel.transform.Find("table").gameObject;
            GameObject switchbtn = seatBG.transform.Find("Player_" + index.ToString()).Find("Button_change").gameObject;
            switchbtn.SetActive(GameMain.hall_.GetPlayerId() != AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)(index - 1)].playerid);
        }
    }

            //uint[] uiname = { 0, 1, 3, 2 };
            //target = uiname[target];

            UMessage sitmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Switch);

    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (AppointmentRoomPanel_ == null)
        {
            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Room_GameList");
            AppointmentRoomPanel_ = (GameObject)GameMain.instantiate(obj0);
            GameObject CanvasObj = GameObject.Find("Canvas/Root");
            AppointmentRoomPanel_.transform.SetParent(CanvasObj.transform, false);

            InitRoomsEvents();
        }

        AppointmentRoomPanel_.SetActive(true);
        currentpickrooms_ = (GameKind_Enum)gameid;
        SendMatchRoomMsg();
        lefttoggles_[currentpickrooms_].isOn = true;
        RefreshAppointmentDiamond();
    }
    {
        if(AppointmentRoomPanel_ == null)
        {
            return;
        }

        if(!AppointmentRoomPanel_.activeSelf)
        {
            return;
        }

        fResfreshAppointmentPanelTime -= Time.unscaledDeltaTime;
        if(fResfreshAppointmentPanelTime < 0)
        {
            SendMatchRoomMsg();
        }
    }
    {
        fResfreshAppointmentPanelTime = 10;
        UMessage publicmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_PublicRooms);

        publicmsg.Add((byte)currentpickrooms_);

        NetWorkClient.GetInstance().SendMsg(publicmsg);
    }
    {
        GameObject returnbtn = AppointmentRoomPanel_.transform.Find("Top/ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(returnbtn, OnCloseAllAppointments, null);

        lefttoggles_.Clear();
        UnityEngine.Transform GameKindTransform = null;
        for (GameKind_Enum gameKind = GameKind_Enum.GameKind_CarPort; gameKind < GameKind_Enum.GameKind_Max; ++gameKind)
        {
            GameKind_Enum curGameKind = gameKind;
            string toggleName = "Left/Imagebg/Toggle_" + (byte)curGameKind;
            GameKindTransform = AppointmentRoomPanel_.transform.Find(toggleName);
            if(GameKindTransform == null)
            {
                continue;
            }
            bool activeState = GameMain.hall_.GetGameIcon_ImageIcon((byte)curGameKind) != null;
            Toggle leftToggle = AppointmentRoomPanel_.transform.Find(toggleName).gameObject.GetComponent<Toggle>();
            leftToggle.onValueChanged.AddListener(delegate (bool call) { if (call) SwitchAppointments(curGameKind); });
            lefttoggles_.Add(curGameKind, leftToggle);
            GameKindTransform.gameObject.SetActive(activeState);
        }

        //GameObject returnBtn = rulepanel_.transform.FindChild("Top").FindChild("ButtonReturn").gameObject;
        //XPointEvent.AutoAddListener(returnBtn, OnCloseRulePanel, null);

        GameObject createBtn = AppointmentRoomPanel_.transform.Find("Top").Find("Button_chuangjian").gameObject;
        XPointEvent.AutoAddListener(createBtn, OnCreateRooms, null);

        GameObject joinBtn = AppointmentRoomPanel_.transform.Find("Top").Find("Button_jiaru").gameObject;
        XPointEvent.AutoAddListener(joinBtn, OnShowNumberPanel, null);

        GameObject shopBtn = AppointmentRoomPanel_.transform.Find("Top/Image_DiamondFrame").gameObject;
        XPointEvent.AutoAddListener(shopBtn, GameMain.hall_.Charge, Shop.SHOPTYPE.SHOPTYPE_DIAMOND);
    }

    private void OnCreateRooms(EventTriggerType eventtype, object button, PointerEventData eventData)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if (rulepanel_ == null)
            {
                LoadRulePanel();
                InitRulePanel();
            }

            //GameObject createPanel = rulepanel_.transform.FindChild("Pop-up").FindChild("Room_rule").gameObject;
           foreach(var key in rulePanelLeftToggles_.Keys)
            {
                if(key == currentpickrooms_)
                {
                    rulePanelLeftToggles_[key].isOn = true;
                }else
                {
                    rulePanelLeftToggles_[key].isOn = false;
                }
            }

            rulepanel_.transform.SetAsLastSibling();
            isRuleEnterWay = false;
            AppointmentRoomPanel_.SetActive(false);
           
            //ResetCostContents(currentpickrooms_);
        }

    void SwitchAppointments(GameKind_Enum callrooms)
    {
        currentpickrooms_ = callrooms;
        CustomAudio.GetInstance().PlayCustomAudio(1003);
        SendMatchRoomMsg();
        //ResetCostContents(currentpickrooms_);
    }

    /// <summary>
    /// 初始化公开约据房间界面列表
    /// </summary>
    /// <param name="publicAppointmentRoomIDList">公开约据房间ID队列</param>
    void InitAppointmentRooms(List<uint> publicAppointmentRoomIDList)
    {
        for (int index = 0; index < AppointmentDataManager.AppointmentDataInstance().matchrooms_.Count; index++)
        {
            byte temp = (byte)(index + 1);
            LoadMatchRoomResource(temp);
        }

        foreach (var appointmentRoomID in publicAppointmentRoomIDList)
        {
            LoadPublicRoomResource(appointmentRoomID);
        }

        RefreshAppointmentDiamond();
    }

    void RefreshAppointmentDiamond()
    {
        if(null == AppointmentRoomPanel_)
        {
            return;
        }

        Text diamondTx = AppointmentRoomPanel_.transform.Find("Top/Image_DiamondFrame/Text_Diamond").gameObject.GetComponent<Text>();
        diamondTx.text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();
    }

    void LoadMatchRoomResource(byte index)
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Room_icon_matching");
        GameObject match = (GameObject)GameMain.instantiate(obj0);
        match.transform.SetParent(AppointmentRoomPanel_.transform.Find("Right/Room_matching/Content"), false);
        XPointEvent.AutoAddListener(match, OnClickMatch, index);

        Text bet = match.transform.Find("Text_dizhu").gameObject.GetComponent<Text>();
        bet.text = AppointmentDataManager.AppointmentDataInstance().matchrooms_[index - 1].descript_;
        Text incoin = match.transform.Find("Text_jinchang").gameObject.GetComponent<Text>();
        incoin.text = AppointmentDataManager.AppointmentDataInstance().matchrooms_[index - 1].incoin_.ToString();
    }

    void LoadPublicRoomResource(uint key)
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        AppointmentData appointmentData =  AppointmentDataManager.AppointmentDataInstance().GetAppointmentData(key);
        if(appointmentData == null)
        {
            return;
        }

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Room_icon_creation");
        GameObject publicroom = (GameObject)GameMain.instantiate(obj0);
        publicroom.transform.SetParent(AppointmentRoomPanel_.transform.Find("Right/Room_creation/Content"), false);

        GameObject buttonin = publicroom.transform.Find("Button_in").gameObject;
        XPointEvent.AutoAddListener(buttonin, OnClickPublicRoom, key);
        Text bet = publicroom.transform.Find("Text_least").gameObject.GetComponent<Text>();
        bet.text = appointmentData.bet.ToString();
        Text incoin = publicroom.transform.Find("Text_in").gameObject.GetComponent<Text>();
        incoin.text = appointmentData.incoin.ToString();
        Text outcoin = publicroom.transform.Find("Text_out").gameObject.GetComponent<Text>();
        outcoin.text = appointmentData.outcoin.ToString();
        Text roomNo = publicroom.transform.Find("Text_roonnum").gameObject.GetComponent<Text>();
        roomNo.text = key.ToString();
        Text rule = publicroom.transform.Find("Text_rule").gameObject.GetComponent<Text>();

        int maxplayer = appointmentData.gameid == (byte)GameKind_Enum.GameKind_LandLords ? 3 : 4;

        string ruletext = "";
        if (appointmentData.gameid == (byte)GameKind_Enum.GameKind_LandLords)
                        + GuanDanAppointmentData.terData_.score.ToString() + "分 打2";

            else
                  appointmentData.gameid == (byte)GameKind_Enum.GameKind_YcMahjong ||
                  appointmentData.gameid == (byte)GameKind_Enum.GameKind_CzMahjong ||
                  appointmentData.gameid == (byte)GameKind_Enum.GameKind_HongZhong)
        {
            if (appointmentData.maxpower == 250 || appointmentData.maxpower == 0)
                ruletext = "打" + appointmentData.playtimes.ToString() + "局 不封顶";
            else
                ruletext = "打" + appointmentData.playtimes.ToString()
                    + "局 最高" + appointmentData.maxpower + "倍";
        }else if(appointmentData.gameid == (byte)GameKind_Enum.GameKind_GouJi)
        {
            maxplayer = 6;
            ruletext = "打" + appointmentData.playtimes.ToString() + "局";
        }else if(appointmentData.gameid == (byte)GameKind_Enum.GameKind_Chess)
        {
            maxplayer = 2;
            ruletext = "打" + appointmentData.playtimes.ToString() + "局";
        }

        rule.text = ruletext;

        //Text peopleNumber = publicroom.transform.FindChild("Text_people").gameObject.GetComponent<Text>();
      
        GameObject peoplebg = publicroom.transform.Find("Text_people/IconBG").gameObject;

        for (int index = 0; index < peoplebg.transform.childCount; index++)
            peoplebg.transform.GetChild(index).gameObject.SetActive(true);

        for (int index = 0; index < appointmentData.currentPeople; index++)
            peoplebg.transform.GetChild(index).Find("Icon").gameObject.SetActive(true);
    }

    private void OnClickMatch(EventTriggerType eventtype, object button, PointerEventData eventData)
            byte index = (byte)button;
            if (GameMain.hall_.GetPlayerData().GetDiamond() < AppointmentDataManager.AppointmentDataInstance().matchrooms_[index - 1].incoin_)
            {
                CCustomDialog.OpenCustomConfirmUI(2100);
                return;
            }

            //进入游戏
            GameMain.hall_.OnClickRoomIconBtn((byte)currentpickrooms_, index);

    private void OnClickPublicRoom(EventTriggerType eventtype, object button, PointerEventData eventData)
            //进入公开房间
            uint appointmentid = (uint)button;
            SendAppintmentJoinMsg(appointmentid);
        }

    void ClearAppointments()
    {
        if(AppointmentRoomPanel_ == null)
        {
            return;
        }

        GameObject matchbg = AppointmentRoomPanel_.transform.Find("Right/Room_matching/Content").gameObject;
        GameObject createbg = AppointmentRoomPanel_.transform.Find("Right/Room_creation/Content").gameObject;

        GameMain.hall_.ClearChilds(matchbg);
        GameMain.hall_.ClearChilds(createbg);
    }

    private void OnCloseAllAppointments(EventTriggerType eventtype, object button, PointerEventData eventData)

    //GameObject startAnimate;
    //void LoadStartEffect()
    //{
    //    AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
    //    if (bundle == null)
    //        return;

    //    Object startAnimateObj = (GameObject)bundle.LoadAsset("Anime_startgame");
    //    startAnimate = (GameObject)GameMain.instantiate(startAnimateObj);
    //    startAnimate.SetActive(false);
    //    starteffect = startAnimate.GetComponent<UnityArmatureComponent>();
    //    starteffect.AddEventListener(EventObject.COMPLETE, AnimationComplete);
    //}

    //public void PlayAppointmentStartEffect(GameObject rulepanel)
    //{
    //    if (rulepanel == null)
    //        return;

    //    rulepanel.SetActive(true);

    //    if (startAnimate == null)
    //        return;

    //    GameObject startobj = rulepanel.transform.FindChild("Pop-up/Animation/point_startgame").gameObject;

    //    startAnimate.transform.SetParent(startobj.transform, false);
    //    startAnimate.SetActive(true);
    //    starteffect.animation.Play("newAnimation");
    //}

    //void AnimationComplete(string _type, EventObject eventObject)
    //{
    //    switch (_type)
    //    {
    //        case EventObject.COMPLETE:
    //            starteffect.gameObject.SetActive(false);
    //            break;
    //    }
    //}

    public void Update()
        {
            UpdateDownLoad();
        }
    {
        uint percent = DownLoadProcessMgr.Instance.GetDownloadPercent();
                    isDownload_ = false;
                    //GameMain.hall_.EnterGameScene(joinGameKind_, GameTye_Enum.GameType_Appointment);
                    //Join2Msg();
                    GameKind_Enum GameKindEnum = (GameKind_Enum)AppointmentDataManager.AppointmentDataInstance().gameid;
                    if (GameKindEnum > GameKind_Enum.GameKind_None && GameKindEnum < GameKind_Enum.GameKind_Max)
                    {
                        GameMain.hall_.EnterGameScene(GameKindEnum, GameTye_Enum.GameType_Appointment,0,()=> { Join2Msg(); });
                    }
                }
    }

    //第二次加入消息
    void Join2Msg()
    {
        AppointmentDataManager.AppointmentDataInstance().kind = AppointmentKind.From_Appointment;

        UMessage joinmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Join_2);
        joinmsg.Add(GameMain.hall_.GetPlayerId());
        joinmsg.Add(appointmentID_);
        NetWorkClient.GetInstance().SendMsg(joinmsg);
    }