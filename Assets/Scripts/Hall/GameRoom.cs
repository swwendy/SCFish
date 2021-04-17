using System.Collections.Generic;using UnityEngine;using UnityEngine.UI;using UnityEngine.EventSystems;using USocket.Messages;using DragonBones;using System.Linq;
using System.Collections;

#region "约据房间 - 游戏规则数据结构"
//斗地主(约据房间界面)
public class LandLordRule{    public LandLordRule()    {        playTimes = playTimesList[0];        maxPower = maxPowerList[0];    }    public static byte[] playTimesList = { 6, 9, 12, 15, 18 };    public static byte[] maxPowerList = { 12, 24, 250 };    public byte playTimes;  //对战局数
    public byte maxPower;   //最大倍数
}

//掼蛋游戏玩法类型
public enum TePlayType{    Goon = 0, //连续打(带升级)
    Times,    //打几局(不带升级)
}

//掼蛋 --打几局(不带升级) -- 极牌类型
public enum CurrentPoker{    Random = 0, //随机
    Two = 2,    //固定打2
}

//掼蛋(约据房间界面)
public class ThrowEggsRule{    public ThrowEggsRule()    {        vectory = victoryList[0];        times = timesList[0];        score = scoreList[0];        cp = CurrentPoker.Two;    }    public static byte[] victoryList = { 5, 8, 10, 1 };    public static byte[] timesList = { 4, 8, 12, 16 };    public static byte[] scoreList = { 3, 4 };

    //游戏玩法类型
    public TePlayType playType;
    //连续打(带升级)
    public byte vectory;
    //打几局 -- 局数
    public byte times;
    //打几局 -- 积分
    public byte score;
    //打几局 -- 极牌
    public CurrentPoker cp;}

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
public enum CzMahjongRuleDataType{    CzJuShu_Type,           //对战局数
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

public class GameRoom{    public GameObject root_;    GameObject rulepanel_;    LandLordRule llr_;    ThrowEggsRule ter_;    MahjongRule mjr_;    YcMahjongRule ycmjr_;    CzMahjongRule czmjr_;    GouJiRule gouji_;    HzMahjongRule hongZhong_;    ChessRule chessRule_;    int currentPickCost_;    public GameObject alreadyPanel;    public GameObject recordpanel_;    public List<AppointmentRecord> recordlist_;    List<GameObject> recorditems_;    byte playtimesindex_;
    //UnityArmatureComponent starteffect;
    byte currentNeedUpdateGameID;    GameObject waitingTips_;    bool isUpdateJoin_;    GameKind_Enum joinGameKind_;    GameObject mrPanel_;    bool isDownload_;    GameObject costpanel_;    InputField costinputfield_;    bool isopen_;

    //开房扩展 私密/公开
    GameObject AppointmentRoomPanel_;    GameKind_Enum currentpickrooms_;
    //游戏房间游戏列表数据
    Dictionary<GameKind_Enum, Toggle> lefttoggles_;
    //游戏约据房间游戏列表数据
    Dictionary<GameKind_Enum, Toggle> rulePanelLeftToggles_;
    //进入约据房间方式
    bool isRuleEnterWay;    float fResfreshAppointmentPanelTime = 10;    IEnumerator FastSendJoinMsgEnumerator = null;    public GameRoom()    {
        lefttoggles_ = new Dictionary<GameKind_Enum, Toggle>();
        rulePanelLeftToggles_ = new Dictionary<GameKind_Enum, Toggle>();        InitGameRoom();        recordlist_ = new List<AppointmentRecord>();        recorditems_ = new List<GameObject>();

        playtimesindex_ = 1;        isUpdateJoin_ = false;        isDownload_ = false;    }    void LoadRulePanel()
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
    }    public void InitGameRoom()    {
        isopen_ = false;        chessRule_ = new ChessRule();
        LoadRulePanel();
        InitRulePanel();
        RefreshMoney();        NumberPanel.GetInstance().LoadNumberPanelResources();        currentPickCost_ = 0;        llr_ = new LandLordRule();        ter_ = new ThrowEggsRule();        mjr_ = new MahjongRule();        ycmjr_ = new YcMahjongRule();        czmjr_ = new CzMahjongRule();        gouji_ = new GouJiRule();        hongZhong_ = new HzMahjongRule();        currentpickrooms_ = GameKind_Enum.GameKind_LandLords;        InitGameRoomMsg();    }    void InitGameRoomMsg()    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Appointment_SM_Join_1, BackJoinAppointmentGameIDOnly);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_Join_2, BackJoinAppointment);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_Create, BackCreateAppointment);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_Exit, BackExitAppointment);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_Switch, BackSwitchAppointment);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_AgreeSwitch, BackAgreeSwitchAppointment);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_Ready, BackReadyAppointment);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_StartGame, BackStartGameAppointment);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_GameResult, BackStartGameResult);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_Record, BackAppointmentRecord);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.Appointment_SM_ConnectGameServer, BackAppointmentConnectGameServer);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Appointment_SM_ClearReady, BackClearReady);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Appointment_SM_PublicRooms, BackPublicRoomData);    }
    bool BackPublicRoomData(uint msgType, UMessage msg)
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
    }    bool BackClearReady(uint msgType, UMessage msg)
    {
        byte state = msg.ReadByte();
        if (state == 0)
            CCustomDialog.OpenCustomConfirmUI(1654);
        if (state == 1)
            CCustomDialog.OpenCustomConfirmUI(1655);

        AppointmentData currentroom = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();        if (currentroom == null)            return false;

        foreach (byte key in currentroom.seats_.Keys)            currentroom.seats_[key].already = false;        AppointmentDataManager.AppointmentDataInstance().playerAlready = false;

        InitAppointmentData();        RefreshSwitchBtn();

        return true;
    }    bool BackReInitAppointmentData(uint msgType, UMessage msg)    {        byte playtimes = 0;        byte maxpower = 0;        GameKind_Enum gamekind = (GameKind_Enum)msg.ReadByte();        AppointmentDataManager.AppointmentDataInstance().gameid = (byte)gamekind;        if (gamekind == GameKind_Enum.GameKind_LandLords)        {            playtimes = msg.ReadByte();            maxpower = msg.ReadByte();        }        if (gamekind == GameKind_Enum.GameKind_GuanDan)        {            ter_.playType = (TePlayType)msg.ReadByte();            if (ter_.playType == TePlayType.Goon)                ter_.vectory = msg.ReadByte();            else            {                ter_.times = msg.ReadByte();                ter_.score = msg.ReadByte();                ter_.cp = (CurrentPoker)msg.ReadByte();            }        }        return true;    }    bool BackAppointmentConnectGameServer(uint msgType, UMessage msg)    {        CCustomDialog.OpenCustomConfirmUI(1020);        return true;    }    public void AppointmentAddRecord(AppointmentRecord data)    {        recordlist_.Add(data);        RefreshRecordData();    }    bool BackAppointmentRecord(uint msgType, UMessage msg)    {        byte state = msg.ReadByte();        if (state == 0)        {            CCustomDialog.OpenCustomConfirmUI(2005);            return false;        }

        GameMain.hall_.SetGameRecordMsgState(GameRecordMsgState.RecordMsg_End);        recordlist_.Clear();        byte length = msg.ReadByte();        for (int index = 0; index < length; index++)        {            AppointmentRecord data = new AppointmentRecord();            data.gameID = msg.ReadByte();            data.gameTimes = msg.ReadByte();            data.maxpower = msg.ReadByte();            string powerText = "";            if (data.maxpower != 251)            {                if (data.gameID == (byte)GameKind_Enum.GameKind_LandLords)                {                    if (data.maxpower == 250)                        powerText = "不封顶";                    else                        powerText = "最大" + data.maxpower + "倍";                    data.gamerule = CCsvDataManager.Instance.GameDataMgr.GetGameData(data.gameID).GameName + " 打" + data.gameTimes + "局 " + powerText;                }                else if (data.gameID == (byte)GameKind_Enum.GameKind_GuanDan)                {                    if (data.gameTimes == (byte)TePlayType.Goon)                        powerText = " 连续打";                    else if (data.maxpower == 0)                        powerText = " 打" + data.gameTimes + "局 随机打";
                    else
                        powerText = " 打" + data.gameTimes + "局 打" + data.maxpower.ToString();                    data.gamerule = CCsvDataManager.Instance.GameDataMgr.GetGameData(data.gameID).GameName + powerText;                }                else if (data.gameID == (byte)GameKind_Enum.GameKind_Mahjong || data.gameID == (byte)GameKind_Enum.GameKind_YcMahjong || 
                         data.gameID == (byte)GameKind_Enum.GameKind_CzMahjong || data.gameID == (byte)GameKind_Enum.GameKind_GouJi ||
                         data.gameID == (byte)GameKind_Enum.GameKind_HongZhong)
                {
                    powerText = " 打" + data.gameTimes + "局";

                    data.gamerule = CCsvDataManager.Instance.GameDataMgr.GetGameData(data.gameID).GameName + powerText;
                }            }            else            {                powerText = "比赛 第" + data.gameTimes.ToString() + "轮";                data.gamerule = CCsvDataManager.Instance.GameDataMgr.GetGameData(data.gameID).GameName + powerText;            }            data.timeseconds = msg.ReadLong();            data.videoes = msg.ReadLong();            data.isopen = false;            byte playerLength = msg.ReadByte();            for (int playerindex = 0; playerindex < playerLength; playerindex++)            {                AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();                playerdata.playerid = msg.ReadUInt();                playerdata.faceid = msg.ReadInt();                playerdata.playerName = msg.ReadString();                playerdata.coin = msg.ReadLong();                playerdata.url = msg.ReadString();                playerdata.master = msg.ReadSingle();                playerdata.sex = msg.ReadByte();                data.result.Add(playerdata.playerid, playerdata);            }            recordlist_.Add(data);        }        recordlist_ = recordlist_.OrderByDescending(u => u.timeseconds).ToList();        ShowAppointmentRecord();        return true;    }    public void ShowAppointmentRecord()    {        if (recordpanel_ == null)            LoadAppointmentRecordResource();
        else
        {
            recordpanel_.SetActive(true);
            RefreshRecordData();
        }    }    void RefreshRecordData()    {        if (recordpanel_ == null)            return;        GameObject recorditemBG = recordpanel_.transform.Find("middle").            Find("Viewport_record").Find("Content_record").gameObject;        GameMain.hall_.ClearChilds(recorditemBG);        for (int index = 0; index < recorditems_.Count; index++)            GameMain.safeDeleteObj(recorditems_[index]);        recorditems_.Clear();        for (int index = 0; index < recordlist_.Count; index++)        {            int temp = index;            GameMain.SC(LoadRecordItem(temp));        }    }    IEnumerator<WaitForSecondsRealtime> LoadRecordItem(int index)    {        yield return new WaitForSecondsRealtime(0.1f * index);        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle != null && recordpanel_ != null)        {
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
        }    }

    //观看录像
    private void OnClickWatchVideo(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            AppointmentRecord record = (AppointmentRecord)button;

            //按下录像按钮后要做的事情......
            GameVideo.GetInstance().OnClickWatch(record);        }    }    private void OnOOCRecordItem(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            int index = (int)button;            AppointmentRecord data = recordlist_[index];            if (index >= recorditems_.Count)                return;            Rect temp = recorditems_[index].transform.gameObject.GetComponent<Image>().rectTransform.rect;            if (temp.height == 80.0f)            {                recorditems_[index].transform.gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(temp.width, 150.0f);                recorditems_[index].transform.Find("Image_gaikuang").Find("ImageIcon_1").gameObject.SetActive(false);                recorditems_[index].transform.Find("Image_gaikuang").Find("ImageIcon_2").gameObject.SetActive(true);            }            else            {                recorditems_[index].transform.gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(temp.width, 80.0f);                recorditems_[index].transform.Find("Image_gaikuang").Find("ImageIcon_1").gameObject.SetActive(true);                recorditems_[index].transform.Find("Image_gaikuang").Find("ImageIcon_2").gameObject.SetActive(false);            }            for (int objindex = 0; objindex < recorditems_.Count; objindex++)            {                if (objindex == index)                    continue;                recorditems_[objindex].transform.gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(temp.width, 80.0f);                recorditems_[objindex].transform.Find("Image_gaikuang").Find("ImageIcon_1").gameObject.SetActive(true);                recorditems_[objindex].transform.Find("Image_gaikuang").Find("ImageIcon_2").gameObject.SetActive(false);            }        }    }    void InitRecordEvents()    {        GameObject closebtn = recordpanel_.transform.Find("Top").Find("ButtonReturn").gameObject;        XPointEvent.AutoAddListener(closebtn, OnCloseRecordPanel, null);    }    private void OnCloseRecordPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            if (GameMain.hall_.contestui_ != null)                GameMain.hall_.contestui_.SetActive(true);            recordpanel_.SetActive(false);            if (HallMain.videotcpclient != null)            {                HallMain.videotcpclient.CloseNetwork();                HallMain.videotcpclient = null;            }        }    }    void LoadAppointmentRecordResource()    {        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)            return;        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Lobby_Roomrecord");        recordpanel_ = (GameObject)GameMain.instantiate(obj0);        GameObject GameCanvas = GameObject.Find("Canvas");        recordpanel_.transform.SetParent(GameCanvas.transform.Find("Root"), false);        RefreshRecordData();        InitRecordEvents();    }    bool BackStartGameResult(uint msgType, UMessage msg)    {        AppointmentDataManager.AppointmentDataInstance().interrupt = false;        byte rtype = msg.ReadByte();        FriendsMomentsDataMamager.GetFriendsInstance().currentvideoid = msg.ReadLong();        AppointmentDataManager.AppointmentDataInstance().resultList.Clear();        int roleNum = msg.ReadInt();        for (int index = 0; index < roleNum; index++)        {            AppointmentResult resultData = new AppointmentResult();            resultData.sitNo = msg.ReadByte();            resultData.playerid = msg.ReadUInt();            resultData.coin = msg.ReadLong();            AppointmentDataManager.AppointmentDataInstance().resultList.Add(resultData);        }        if (rtype == (byte)GameReason_enum.GameReason_Contest)            return false;        AppointmentDataManager.AppointmentDataInstance().interruptid = msg.ReadUInt();        AppointmentDataManager.AppointmentDataInstance().interrupt =
            AppointmentDataManager.AppointmentDataInstance().interruptid > 0;        foreach (AppointmentSeat value in AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_.Values)
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
        AppointmentDataManager.AppointmentDataInstance().perResultList_.Clear();        byte totalround = msg.ReadByte();        byte playernumber = msg.ReadByte();        for (int index = 0; index < totalround; index++)
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
        }        AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = true;
        AppointmentDataManager.AppointmentDataInstance().playerAlready = false;

        //与游戏服断开连接
        //UMessage exitmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_PLAYERLEAVEROOMSER);
        //exitmsg.Add(GameMain.hall_.GetPlayerId());
        //HallMain.SendMsgToRoomSer(exitmsg);

        return true;    }    bool BackStartGameAppointment(uint msgType, UMessage msg)    {        byte nSerIndex = msg.ReadByte();        HallMain.SetRoomSerIndex(nSerIndex);        uint appointmentid = msg.ReadUInt();        uint gameroomid = msg.ReadUInt();        AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().createtimeseconds = msg.ReadLong();        byte gameid = AppointmentDataManager.AppointmentDataInstance().gameid;        GameMain.hall_.Connect2AppointmentCommonServer(gameid, 0, gameroomid);        if (alreadyPanel != null)            alreadyPanel.SetActive(false);
        return true;    }    bool BackReadyAppointment(uint msgType, UMessage msg)    {        if (alreadyPanel == null)            return false;        byte seatid = msg.ReadByte();        bool isready = msg.ReadByte() != 0;        AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[seatid].already = isready;        if (seatid == AppointmentDataManager.AppointmentDataInstance().playerSitNo)            AppointmentDataManager.AppointmentDataInstance().playerAlready = isready;        GameObject readybtn = alreadyPanel.transform.Find("Button_Ready").gameObject;        readybtn.GetComponent<Button>().interactable = true;

        //GameObject cancelbtn = alreadyPanel.transform.FindChild("Text_yizhunbei").gameObject;
        //cancelbtn.GetComponent<Button>().interactable = true;

        InitAppointmentData();
        return true;    }    bool BackAgreeSwitchAppointment(uint msgType, UMessage msg)    {        uint playerid = msg.ReadUInt();        string playername = msg.ReadString();        agreeappointmentid = msg.ReadUInt();        agreetargetseatid = msg.ReadByte();        agreesourceseatid = msg.ReadByte();        string[] param = { playername };        CCustomDialog.OpenCustomDialogWithFormatParams(1622, IsAgree, param);        return true;    }    uint agreeappointmentid = 0;    byte agreesourceseatid = 0;    byte agreetargetseatid = 0;    void IsAgree(object isagree)    {        bool agree = (int)isagree == 1;        UMessage agreeMsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_AgreeSwitch);        agreeMsg.Add(agree);        agreeMsg.Add(GameMain.hall_.GetPlayerId());        agreeMsg.Add(agreeappointmentid);        agreeMsg.Add(agreesourceseatid);        agreeMsg.Add(agreetargetseatid);        NetWorkClient.GetInstance().SendMsg(agreeMsg);    }    bool BackSwitchAppointment(uint msgType, UMessage msg)    {        uint playerid = msg.ReadUInt();        byte sourceseatid = msg.ReadByte();        byte targetseatid = msg.ReadByte();

        //byte[] uiname = { 0, 1, 3, 2 };
        //targetseatid = uiname[targetseatid];

        if (alreadyPanel == null)            return false;        AppointmentData currentroom = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();        if (currentroom == null)            return false;        int temp = currentroom.seats_[sourceseatid].icon;        currentroom.seats_[sourceseatid].icon = currentroom.seats_[targetseatid].icon;        currentroom.seats_[targetseatid].icon = temp;        uint tempu = currentroom.seats_[sourceseatid].playerid;        currentroom.seats_[sourceseatid].playerid = currentroom.seats_[targetseatid].playerid;        currentroom.seats_[targetseatid].playerid = tempu;        string temps = currentroom.seats_[sourceseatid].playerName;        currentroom.seats_[sourceseatid].playerName = currentroom.seats_[targetseatid].playerName;        currentroom.seats_[targetseatid].playerName = temps;        string tempr = currentroom.seats_[sourceseatid].url;        currentroom.seats_[sourceseatid].url = currentroom.seats_[targetseatid].url;        currentroom.seats_[targetseatid].url = tempr;
        foreach (byte key in currentroom.seats_.Keys)            currentroom.seats_[key].already = false;        AppointmentDataManager.AppointmentDataInstance().playerAlready = false;        if (AppointmentDataManager.AppointmentDataInstance().playerSitNo == sourceseatid)        {            AppointmentDataManager.AppointmentDataInstance().currentsource = targetseatid;            AppointmentDataManager.AppointmentDataInstance().playerSitNo = targetseatid;        }        else if (AppointmentDataManager.AppointmentDataInstance().playerSitNo == targetseatid)        {            AppointmentDataManager.AppointmentDataInstance().playerSitNo = sourceseatid;            AppointmentDataManager.AppointmentDataInstance().currentsource = sourceseatid;        }        InitAppointmentData();        RefreshSwitchBtn();        return true;    }    public void InitAppointmentData()    {
        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
        {
            Back2Hall();
            return;
        }        if (alreadyPanel == null)            return;        Text appointmentNo = alreadyPanel.transform.Find("Top/ImageBG/Text_Room/Text_Num").gameObject.GetComponent<Text>();        appointmentNo.text = AppointmentDataManager.AppointmentDataInstance().currentRoomID.ToString();

        //AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        //if (bundle == null)
        //    return;

        AppointmentData data = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();        GameObject seatBG = alreadyPanel.transform.Find("table").gameObject;        for (byte index = 0; index < data.maxPlayer; index++)        {            bool isEmpty = data.seats_[index].playerid == 0;            GameObject nameBG = seatBG.transform.Find("Player_" + (index + 1).ToString()).                Find("ImageBG").gameObject;            GameObject iconBG = seatBG.transform.Find("Player_" + (index + 1).ToString()).                Find("Head").gameObject;            GameObject already = seatBG.transform.Find("Player_" + (index + 1).ToString()).                Find("already").gameObject;            GameObject tickout = seatBG.transform.Find("Player_" + (index + 1).ToString()).                Find("Button_tichu").gameObject;            nameBG.SetActive(!isEmpty);            iconBG.SetActive(!isEmpty);            already.SetActive(data.seats_[index].already);            tickout.SetActive(data.hostid == GameMain.hall_.GetPlayerId() && !isEmpty && data.seats_[index].playerid != GameMain.hall_.GetPlayerId());            if (isEmpty)                continue;            Text name = nameBG.transform.Find("Name_Text").gameObject.GetComponent<Text>();            name.text = data.seats_[index].playerName;            Image icon = iconBG.transform.Find("HeadMask").Find("ImageHead").gameObject.GetComponent<Image>();            icon.sprite = GameMain.hall_.GetIcon(data.seats_[index].url, data.seats_[index].playerid, data.seats_[index].icon);        }        int totalChildCount = seatBG.transform.childCount - 1;        for (uint index = data.maxPlayer + 1; index < totalChildCount; index++)        {            GameObject closeseat = seatBG.transform.Find("Player_" + index.ToString()).gameObject;            closeseat.SetActive(false);        }


        string gameRuleTextData = null;        GameFunction.GetAppointmentRuleTextData(ref gameRuleTextData, (GameKind_Enum)AppointmentDataManager.AppointmentDataInstance().gameid);
        Text tableTx = alreadyPanel.transform.Find("table/Text_Rule").gameObject.GetComponent<Text>();
        tableTx.text = gameRuleTextData;        GameObject invatebtn = alreadyPanel.transform.Find("Button_Invitation").gameObject;        GameObject readybtn = alreadyPanel.transform.Find("Button_Ready").gameObject;        GameObject cancelbtn = alreadyPanel.transform.Find("Text_yizhunbei").gameObject;        invatebtn.SetActive(false);        //invatebtn.SetActive(!AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().IsFull());        readybtn.SetActive(AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().IsFull());        cancelbtn.SetActive(AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().IsFull());        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().IsFull())        {            if (AppointmentDataManager.AppointmentDataInstance().playerAlready)            {                cancelbtn.SetActive(true);                readybtn.SetActive(false);            }            else            {                cancelbtn.SetActive(false);                readybtn.SetActive(true);            }
        }    }    bool BackExitAppointment(uint msgType, UMessage msg)    {        byte state = msg.ReadByte();        uint playerid = msg.ReadUInt();        byte reason = msg.ReadByte();        if (playerid == GameMain.hall_.GetPlayerId())        {            AppointmentDataManager.AppointmentDataInstance().Clear();            Debug.Log("appointment back 2 hall : 4");            Back2Hall();            switch (reason)            {                case 0:
                    //CCustomDialog.OpenCustomConfirmUI(1623);
                    break;                case 1:                    CCustomDialog.OpenCustomConfirmUI(1623, Back2Hall);                    break;                case 2:                    break;                case 3:                    CCustomDialog.OpenCustomConfirmUI(2107, Back2Hall);                    break;            }        }        else        {            byte seatid = msg.ReadByte();            AppointmentData data = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();            if (!data.seats_.ContainsKey(seatid))                return false;            data.seats_[seatid].Clear();            InitAppointmentData();        }        return true;    }    public void Back2Hall(object value = null)    {        if (GameMain.hall_.GameBaseObj != null)            GameMain.hall_.GameBaseObj.OnDisconnect();        if (AppointmentDataManager.AppointmentDataInstance().kind == AppointmentKind.From_Appointment)            GameMain.hall_.SwitchToHallScene(true, 0);        else            GameMain.hall_.SwitchToHallScene(true, 6);        AppointmentDataManager.AppointmentDataInstance().playerAlready = false;        AppointmentDataManager.AppointmentDataInstance().playerSitNo = 0;    }    bool BackJoinAppointment(uint msgType, UMessage msg)    {        joinGameKind_ = ReadJoinMsg(msg);

        GameMain.hall_.GameBaseObj.StartLoad();        
        appointmentID_ = 0;
        return true;    }    byte UpdateGameResource()    {        byte gameid = AppointmentDataManager.AppointmentDataInstance().gameid;        byte updatestate = CResVersionCompareUpdate.CheckGameResNeedDownORUpdate(gameid);        if (updatestate != 0)        {            uint tips = 1024;            currentNeedUpdateGameID = gameid;            CCustomDialog.OpenCustomDialogWithTipsID(tips, DownLoadGameResource);        }        return updatestate;    }    GameKind_Enum ReadJoinMsg(UMessage msg)    {        byte state = msg.ReadByte();        uint playerid = msg.ReadUInt();        if (state == 250 || state == 251)        {            if (playerid != GameMain.hall_.GetPlayerId())                return GameKind_Enum.GameKind_None;            if (state == 250)
            {
                CCustomDialog.OpenCustomConfirmUI(2106, Back2Hall);
            }else if (state == 251)
            {
                if (GameMain.hall_.GetCurrentGameState() == GameState_Enum.GameState_Appointment)                    CCustomDialog.OpenCustomConfirmUI(1619, Back2Hall);
                else
                    CCustomDialog.OpenCustomConfirmUI(1619);
            }            NumberPanel.GetInstance().ResetNumberInputTextData();            return GameKind_Enum.GameKind_None;        }        if (playerid == GameMain.hall_.GetPlayerId())        {            uint appointmentid = msg.ReadUInt();            uint hostid = msg.ReadUInt();            byte playtimes = 0;            byte maxpower = 0;            GameKind_Enum gamekind = (GameKind_Enum)msg.ReadByte();
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
            AppointmentDataManager.AppointmentDataInstance().playerSitNo = state;            AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().ClearSeats();            byte length = msg.ReadByte();            for (int index = 0; index < length; index++)            {                uint userid = msg.ReadUInt();                byte userSitNo = msg.ReadByte();                string userName = msg.ReadString();                int userFaceID = msg.ReadInt();                long userMoney = msg.ReadLong();                string url = msg.ReadString();                bool already = msg.ReadByte() != 0;//准备
                if (userSitNo == AppointmentDataManager.AppointmentDataInstance().playerSitNo)
                    AppointmentDataManager.AppointmentDataInstance().playerAlready = already;                RefreshJoin(appointmentid, userid, userName, userFaceID, userSitNo, url, already);            }            AppointmentDataManager.AppointmentDataInstance().gameid = (byte)gamekind;            InitAppointmentData();            InitAppointmentEvents();            return gamekind;        }        else        {            uint appointmentid = msg.ReadUInt();            string playerName = msg.ReadString();            int faceid = msg.ReadInt();            long coin = msg.ReadLong();            string url = msg.ReadString();            bool already = msg.ReadByte()!=0;//准备

            RefreshJoin(appointmentid, playerid, playerName, faceid, state, url, already);            InitAppointmentData();            InitAppointmentEvents();            return GameKind_Enum.GameKind_None;        }    }    void RefreshJoin(uint appointmentid, uint playerid, string playerName, int icon, byte sitNo, string url,bool bAlready)    {        AppointmentData appointmentData = AppointmentDataManager.AppointmentDataInstance().GetAppointmentData(appointmentid);        if (appointmentData == null)            return;        Debug.Log("refresh sitNo:" + sitNo);        if (!appointmentData.seats_.ContainsKey(sitNo))            return;        appointmentData.seats_[sitNo].playerid = playerid;
        appointmentData.seats_[sitNo].playerName = playerName;        appointmentData.seats_[sitNo].icon = icon;        appointmentData.seats_[sitNo].url = url;        appointmentData.seats_[sitNo].already = bAlready;        AppointmentDataManager.AppointmentDataInstance().currentRoomID = appointmentid;    }    bool BackCreateAppointment(uint msgType, UMessage msg)    {        byte state = msg.ReadByte();        if (state == 0 || state == 2)        {            UnityEngine.Transform groupTransform = rulepanel_.transform.Find("Right");            groupTransform.Find("Button_chuangjian").GetComponent<Button>().interactable = true;            groupTransform.Find("Button_chuangjian_0").GetComponent<Button>().interactable = true;            CCustomDialog.OpenCustomConfirmUI((uint)(state == 0 ? 1618 : 1617));            return false;        }        uint roomid = msg.ReadUInt();        AppointmentDataManager.AppointmentDataInstance().gameid = msg.ReadByte();        byte isopen = msg.ReadByte();        AppointmentDataManager.AppointmentDataInstance().playerSitNo = 0;        AppointmentData publicAppointmentData = GameFunction.CreateAppointmentData((GameKind_Enum)AppointmentDataManager.AppointmentDataInstance().gameid);
        publicAppointmentData.gameid = AppointmentDataManager.AppointmentDataInstance().gameid;
        publicAppointmentData.ReadAppointmentDataMessage(msg);        publicAppointmentData.SetIsOpenRoom(isopen != 0);

        publicAppointmentData.roomid = roomid;
        publicAppointmentData.hostid = GameMain.hall_.GetPlayerId();
        publicAppointmentData.seats_[0].playerid = GameMain.hall_.GetPlayerId();
        publicAppointmentData.seats_[0].playerName = GameMain.hall_.GetPlayerData().GetPlayerName();
        publicAppointmentData.seats_[0].icon = (int)GameMain.hall_.GetPlayerData().PlayerIconId;
        publicAppointmentData.seats_[0].url = GameMain.hall_.GetPlayerData().GetPlayerIconURL();        AppointmentDataManager.AppointmentDataInstance().currentRoomID = roomid;
        AppointmentDataManager.AppointmentDataInstance().currentsource = 0;        AppointmentDataManager.AppointmentDataInstance().AddAppointmentData(roomid, publicAppointmentData);        GameMain.hall_.EnterGameScene((GameKind_Enum)AppointmentDataManager.AppointmentDataInstance().gameid, GameTye_Enum.GameType_Appointment,0,()=> { InitAppointmentData();InitAppointmentEvents(); });        return true;    }    void RefreshTime()    {        if (root_ == null)            return;        Text time = root_.transform.Find("Top").Find("Text_time").gameObject.GetComponent<Text>();        time.text = System.DateTime.Now.ToString("HH:mm");        Text ruletime = rulepanel_.transform.Find("Top").Find("Text_time").gameObject.GetComponent<Text>();        ruletime.text = System.DateTime.Now.ToString("HH:mm");        if (recordpanel_ == null)            return;        Text recordtime = recordpanel_.transform.Find("Top").Find("Text_time").gameObject.GetComponent<Text>();        recordtime.text = System.DateTime.Now.ToString("HH:mm");    }    void InitRulePanel()    {        GameObject returnBtn = rulepanel_.transform.Find("Top").Find("ButtonReturn").gameObject;        XPointEvent.AutoAddListener(returnBtn, OnCloseRulePanel, null);

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
        }        if(rulePanelLeftToggles_.ContainsKey(currentpickrooms_))
        {
            rulePanelLeftToggles_[currentpickrooms_].isOn = true;
        }        rulepanel_.transform.Find("Right").gameObject.SetActive(initCurGameKindState);
        rulepanel_.transform.Find("Left").gameObject.SetActive(initCurGameKindState);
        rulepanel_.transform.Find("Open").gameObject.SetActive(initCurGameKindState);
        rulepanel_.transform.Find("Imagekong").gameObject.SetActive(!initCurGameKindState);        AppointmentDataManager.AppointmentDataInstance().gameid = (byte)currentpickrooms_;        InitRightRuleEvents();        RefreshPayInfo(1);        OpenRoomType();    }

    void OpenRoomType()
    {
        GameObject openbg = rulepanel_.transform.Find("Open/ImageBG").gameObject;

        Toggle opentoggle = openbg.transform.GetChild(0).gameObject.GetComponent<Toggle>();
        opentoggle.isOn = isopen_;
        opentoggle.onValueChanged.AddListener(delegate (bool call) { CustomAudio.GetInstance().PlayCustomAudio(1004);  isopen_ = call; });

        Toggle closetoggle = openbg.transform.GetChild(1).gameObject.GetComponent<Toggle>();
        closetoggle.isOn = !isopen_;
        closetoggle.onValueChanged.AddListener(delegate (bool call) { CustomAudio.GetInstance().PlayCustomAudio(1004); isopen_ = !call; });
    }    public void RefreshMoney()    {        RefreshAppointmentDiamond();        if (root_ == null)            return;        Text diamond = root_.transform.Find("Top").Find("Image_DiamondFrame").Find("Text_Diamond").gameObject.GetComponent<Text>();        diamond.text = (GameMain.hall_.GetPlayerData().GetDiamond() + GameMain.hall_.GetPlayerData().GetCoin()).ToString();    }    //初始化斗地主约据房间规则事件    void InitRightRuleLandLordsEvents()
    {
        if(null == rulepanel_)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_7");
        UnityEngine.Transform llfirstGroupTransform = ruleTransform.Find("jushu/ImageBG");        for (int index = 0; index < llfirstGroupTransform.childCount; index++)        {            Toggle toggle = llfirstGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            toggle.onValueChanged.AddListener(                delegate (bool value)
                {                    CustomAudio.GetInstance().PlayCustomAudio(1004);                    llr_.playTimes = LandLordRule.playTimesList[temp];                    RefreshPayInfo(temp + 1);                });        }        UnityEngine.Transform llsecondGroupTransform = ruleTransform.Find("beishu/ImageBG");        for (int index = 0; index < llsecondGroupTransform.childCount; index++)        {            Toggle toggle = llsecondGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            toggle.onValueChanged.AddListener(delegate (bool vale)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                llr_.maxPower = LandLordRule.maxPowerList[temp];
            });        }
    }    //初始化掼蛋约据房间规则事件    void InitRightRuleGuanDanEvents()
    {
        if(rulepanel_ == null)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_13");
        UnityEngine.Transform tefirstGroupTransform = ruleTransform.Find("wanfa/ImageBG");        GameObject goonPanel = ruleTransform.Find("lianxu").gameObject;        GameObject timesPanel = ruleTransform.Find("jiju").gameObject;        for (int index = 0; index < tefirstGroupTransform.childCount; index++)        {            Toggle toggle = tefirstGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            toggle.onValueChanged.AddListener(delegate (bool value)
            {                CustomAudio.GetInstance().PlayCustomAudio(1004);                ter_.playType = (TePlayType)temp;                goonPanel.SetActive(temp == 0);                timesPanel.SetActive(temp == 1);
            });        }        UnityEngine.Transform tesecondGroupTransform = ruleTransform.Find("lianxu/guo/ImageBG");        for (int index = 0; index < tesecondGroupTransform.childCount; index++)        {            Toggle toggle = tesecondGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                ter_.vectory = ThrowEggsRule.victoryList[temp];
            });        }        UnityEngine.Transform tethirdGroupTransform = ruleTransform.Find("jiju/jushu/ImageBG");        for (int index = 0; index < tethirdGroupTransform.childCount; index++)        {            Toggle toggle = tethirdGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            toggle.onValueChanged.AddListener(                delegate (bool value)
                {                    CustomAudio.GetInstance().PlayCustomAudio(1004);                    ter_.times = ThrowEggsRule.timesList[temp];                    RefreshPayInfo(temp + 1);                });        }        UnityEngine.Transform teforthGroupTransform = ruleTransform.Find("jiju/jifen/ImageBG");        for (int index = 0; index < teforthGroupTransform.childCount; index++)        {            Toggle toggle = teforthGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                ter_.score = ThrowEggsRule.scoreList[temp];
            });        }

        UnityEngine.Transform tefifthGroupTransform = ruleTransform.Find("jiju/jipai/ImageBG");        for (int index = 0; index < tefifthGroupTransform.childCount; index++)        {            Toggle toggle = tefifthGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();            int temp = index;            if (temp == 0)                temp = 2;            else                temp = 0;            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                ter_.cp = (CurrentPoker)temp;
            });        }    }

    //初始化血战麻将约据房间规则事件
    void InitRightRuleMahjongEvents()
    {
        if (rulepanel_ == null)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_12");
        UnityEngine.Transform mjfirstGroupTransform = ruleTransform.Find("jushu/ImageBG");        for (int index = 0; index < mjfirstGroupTransform.childCount; index++)
        {
            Toggle toggle = mjfirstGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                mjr_.times = MahjongRule.timesList[temp];
            });
        }

        UnityEngine.Transform mjsecondGroupTransform = ruleTransform.Find("beishu/ImageBG");        for (int index = 0; index < mjsecondGroupTransform.childCount; index++)
        {
            Toggle toggle = mjsecondGroupTransform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                mjr_.maxPower = MahjongRule.maxPowerList[temp];
            });
        }

        UnityEngine.Transform mjthirdGroupTransform = ruleTransform.Find("wanfa_1/ImageBG");        for (int index = 0; index < mjthirdGroupTransform.childCount; index++)
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
        }        UnityEngine.Transform mjforthGroupTransform = ruleTransform.Find("wanfa_2/ImageBG");        for (int index = 0; index < mjforthGroupTransform.childCount; index++)
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

        UnityEngine.Transform mjfifthGroupTransform = ruleTransform.Find("wanfa_3/ImageBG");        Toggle toggle0 = mjfifthGroupTransform.GetChild(0).gameObject.GetComponent<Toggle>();
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
    }    //盐城麻将    void InitRightRuleYcMahjongEvents()
    {
        if (rulepanel_ == null)
        {
            return;
        }

        UnityEngine.Transform ruleTransform = rulepanel_.transform.Find("Right/rule_14");
        UnityEngine.Transform ycmjfirstGropTransform = ruleTransform.Find("jushu/ImageBG");        for (int index = 0; index < ycmjfirstGropTransform.childCount; index++)
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
    }    //常州麻将约据房间事件    void RuleCzMahjongRoomEvent(UnityEngine.Transform ruleTransform,string pathName,CzMahjongRuleDataType dataType)
    {
        UnityEngine.Transform czGropTransform = ruleTransform.Find(pathName);        for (int index = 0; index < czGropTransform.childCount; index++)
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
    }    //常州麻将    void InitRightRuleCzMahjongEvents()
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

    //够级    void InitRightRuleGouJiEvents()
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
    }    //红中    void InitRightRuleHzMahjongEvents()
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
    }    /// <summary>
    /// 象棋
    /// </summary>    void InitRightRuleChessEvent()
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
        InputField juShiInputField = juShiTransform.GetComponent<InputField>();        juShiInputField.onValueChanged.RemoveAllListeners();        juShiInputField.onValueChanged.AddListener(delegate (string times)
        {
            uint.TryParse(times, out chessRule_.ChessTime);
            chessRule_.ChessTime = (uint)Mathf.Clamp(chessRule_.ChessTime, 1, 1440);
            juShiInputField.text = chessRule_.ChessTime.ToString();
        });
        juShiInputField.onValueChanged.Invoke(chessRule_.ChessTime.ToString());
    }    void InitRightRuleEvents()    {        if(rulepanel_ == null)
        {
            return;
        }        UnityEngine.Transform rightTransform = rulepanel_.transform.Find("Right");        Button createBtn = rightTransform.Find("Button_chuangjian").GetComponent<Button>();        createBtn.onClick.RemoveAllListeners();        createBtn.onClick.AddListener(() => { OnCreateGameRoom(); });        Button createBtn0 = rightTransform.Find("Button_chuangjian_0").GetComponent<Button>();
        createBtn0.onClick.RemoveAllListeners();        createBtn0.onClick.AddListener(() => { OnCreateGameRoom(); });
        //斗地主
        InitRightRuleLandLordsEvents();
        //掼蛋
        InitRightRuleGuanDanEvents();        //血战麻将        InitRightRuleMahjongEvents();
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
    }    void ChangeRuleInfo(bool ison, GameKind_Enum selectGameKind)    {
        if (!ison)        {            return;
        }
        CustomAudio.GetInstance().PlayCustomAudio(1003);        string ruleName = string.Empty;        UnityEngine.Transform gameRuleTransform = null;        UnityEngine.Transform groupTransform = rulepanel_.transform.Find("Right");        for (GameKind_Enum gameKind = GameKind_Enum.GameKind_CarPort; gameKind < GameKind_Enum.GameKind_Max; ++gameKind)        {            ruleName = "rule_" + (byte)gameKind;            gameRuleTransform = groupTransform.Find(ruleName);            if(gameRuleTransform)
            {
                gameRuleTransform.gameObject.SetActive(selectGameKind == gameKind);
            }        }

        AppointmentDataManager.AppointmentDataInstance().gameid = (byte)selectGameKind;        RefreshPayInfo(1);    }    void RefreshPayInfo(int levelindex)    {        GameObject group = rulepanel_.transform.Find("Right").gameObject;        byte gameid = AppointmentDataManager.AppointmentDataInstance().gameid;

        if (!AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_.Keys.Contains(gameid))
        {
            return;
        }
        int power = AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_[gameid].datas[0];        int level = AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_[gameid].datas[levelindex];        int pay4appointment = power * level;        group.transform.Find("Button_chuangjian").gameObject.SetActive(pay4appointment > 0);        group.transform.Find("Button_chuangjian_0").gameObject.SetActive(pay4appointment <= 0);        Text pay = group.transform.Find("ImageBGcost").Find("Textnum").gameObject.GetComponent<Text>();        pay.text = pay4appointment.ToString();        group.transform.Find("ImageBGcost/ImageIcon_jlb").gameObject.SetActive(false);        group.transform.Find("ImageBGcost/ImageIcon_yj").gameObject.SetActive(true);

        playtimesindex_ = (byte)levelindex;    }    void LoadWaitingTipsResource()    {        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)            return;        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Tips_Waiting");        waitingTips_ = (GameObject)GameMain.instantiate(obj0);        GameObject GameCanvas = GameObject.Find("Canvas/Root");        waitingTips_.transform.SetParent(GameCanvas.transform, false);    }    void DownLoadGameResource(object param)    {        int select = (int)param;        if (select == 1)        {            if (waitingTips_ == null)                LoadWaitingTipsResource();            waitingTips_.SetActive(true);            GameMain.hall_.CheckGameResourceUpdate((byte)currentNeedUpdateGameID);            isDownload_ = true;        }        else        {            byte sitNo = AppointmentDataManager.AppointmentDataInstance().playerSitNo;            uint playerid = GameMain.hall_.GetPlayerId();            UMessage tickmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Exit);            tickmsg.Add(playerid);            tickmsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);            tickmsg.Add((byte)sitNo);            tickmsg.Add((byte)2);//0：游戏中有玩家退出，解散房间 1：房主退出房间，解散房间 2：玩家正常退出，不解散房间 3：被踢，不解散房间

            NetWorkClient.GetInstance().SendMsg(tickmsg);        }    }

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
    public void OnCreateGameRoom()    {        CustomAudio.GetInstance().PlayCustomAudio(1002);

        byte gameid = AppointmentDataManager.AppointmentDataInstance().gameid;

        if(!AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_.Keys.Contains(gameid))
        {
            return;
        }

        int power = AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_[gameid].datas[0];
        int level = AppointmentDataManager.AppointmentDataInstance().appointmentcsvs_[gameid].datas[playtimesindex_];

        int pay4appointment = power * level;        if (GameMain.hall_.GetPlayerData().GetDiamond() < pay4appointment)
        {
            CCustomDialog.OpenCustomConfirmUI(1617);
            return;
        }        byte state = UpdateGameResource();        if (state != 0)            return;        UMessage createmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Create);

        createmsg.Add((byte)0);        createmsg.Add(GameMain.hall_.GetPlayerId());        createmsg.Add(gameid);
        createmsg.Add(playtimesindex_);        createmsg.Add(isopen_);
        createmsg.Add(GetJuShuORPower(true, (GameKind_Enum)gameid));
        createmsg.Add(GetJuShuORPower(false, (GameKind_Enum)gameid));
        int wanFa = 0;        switch ((GameKind_Enum)gameid)
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
        }        NetWorkClient.GetInstance().SendMsg(createmsg);        UnityEngine.Transform groupTransform = rulepanel_.transform.Find("Right");
        groupTransform.Find("Button_chuangjian").GetComponent<Button>().interactable = false;
        groupTransform.Find("Button_chuangjian_0").GetComponent<Button>().interactable = false;
        AppointmentDataManager.AppointmentDataInstance().kind = AppointmentKind.From_Appointment;    }

    public void OnShowNumberPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            NumberPanel.GetInstance().SetNumberPanelActive(true, SendJoinMsg);        }    }

    public void OnShowRulePanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            rulepanel_.SetActive(true);            isRuleEnterWay = true;            rulepanel_.transform.SetSiblingIndex(rulepanel_.transform.parent.childCount);        }    }    private void OnCloseRulePanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if (!isRuleEnterWay)
            {
                AppointmentRoomPanel_.SetActive(true);            }            rulepanel_.SetActive(false);
        }    }
    bool BackJoinAppointmentGameIDOnly(uint msgType, UMessage msg)
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
    }    /// <summary>
    /// 快速加入比赛或约据
    /// </summary>
    /// <param name="id"></param>    public void SendJoinMsg(uint id)    {
        GameMain.ST(FastSendJoinMsgEnumerator);
        FastSendJoinMsgEnumerator = SendJoinMsgEnumerator(id);
        GameMain.SC(FastSendJoinMsgEnumerator);    }    IEnumerator SendJoinMsgEnumerator(uint id)
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
    }    uint appointmentID_ = 0;    private bool SendAppintmentJoinMsg(uint appointmentID,bool tipState = true)
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
        NetWorkClient.GetInstance().SendMsg(joinmsg);        appointmentID_ = appointmentID;
        return true;
    }    public GameObject LoadAppintmentReadResource(string resourceName = "Room_Ready")    {        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)            return null;        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset(resourceName);        alreadyPanel = (GameObject)GameMain.instantiate(obj0);        GameObject GameCanvas = GameObject.Find("Canvas");        alreadyPanel.transform.SetParent(GameCanvas.transform.Find("Root"), false);
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

        return alreadyPanel;    }    void InitAppointmentEvents()    {        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
        {
            Back2Hall();
            return;
        }        GameObject seatBG = alreadyPanel.transform.Find("table").gameObject;        for (uint index = 1;            index < AppointmentDataManager.AppointmentDataInstance().            GetCurrentAppointment().maxPlayer + 1;            index++)        {            GameObject seatbtn = seatBG.transform.Find("Player_" + index.ToString()).Find("Button_sit").gameObject;            seatbtn.SetActive(true);            uint temp = index - 1;            XPointEvent.AutoAddListener(seatbtn, OnSit, temp);
            GameObject tickout = seatBG.transform.Find("Player_" + index.ToString()).Find("Button_tichu").gameObject;            XPointEvent.AutoAddListener(tickout, OnTickOut, (byte)temp);            GameObject switchbtn = seatBG.transform.Find("Player_" + index.ToString()).Find("Button_change").gameObject;            XPointEvent.AutoAddListener(switchbtn, OnSendSwitchMsg, temp);            switchbtn.SetActive(GameMain.hall_.GetPlayerId() != AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[(byte)(index - 1)].playerid);        }        GameObject invatebtn = alreadyPanel.transform.Find("Button_Invitation").gameObject;
        //XPointEvent.AutoAddListener(invatebtn, OnInvateVX, null);
        invatebtn.SetActive(false);        //GameObject readybtn = alreadyPanel.transform.FindChild("Button_Ready").gameObject;        //readybtn.GetComponent<Button>().interactable = false;        //XPointEvent.AutoAddListener(readybtn, OnReady, null);

        //GameObject cancelbtn = appointmentPanel_.transform.FindChild("Text_yizhunbei").gameObject;
        //XPointEvent.AutoAddListener(cancelbtn, OnCancel, null);
    }    private void OnInvateVX(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            Text tableTx = alreadyPanel.transform.Find("table/Text_Rule").gameObject.GetComponent<Text>();            uint roomid = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().roomid;            string url = "http://gdcdn.qpdashi.com/PhoenixEgg/open_yc.html?game="
                        + AppointmentDataManager.AppointmentDataInstance().gameid
                        + "&type=" + (byte)GameTye_Enum.GameType_Appointment
                        + "&room=" + roomid;
            Player.ShareURLToWechat(url,
                CCsvDataManager.Instance.GameDataMgr.GetGameData(AppointmentDataManager.AppointmentDataInstance().gameid).GameName +                " 房间密码:" + roomid.ToString() + " " + tableTx.text, false);        }    }    private void OnCancel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            UMessage readymsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Ready);

            //readymsg.Add(GameMain.hall_.GetPlayerId());
            readymsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);            readymsg.Add((byte)AppointmentDataManager.AppointmentDataInstance().playerSitNo);            readymsg.Add(false);            NetWorkClient.GetInstance().SendMsg(readymsg);

            //GameObject cancelbtn = appointmentPanel_.transform.FindChild("Text_yizhunbei").gameObject;
            //cancelbtn.GetComponent<Button>().interactable = false;
        }    }    private void OnReady()    {        GameObject readybtn = alreadyPanel.transform.Find("Button_Ready").gameObject;        readybtn.GetComponent<Button>().interactable = false;        CustomAudio.GetInstance().PlayCustomAudio(1002);        UMessage readymsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Ready);

        //readymsg.Add(GameMain.hall_.GetPlayerId());
        readymsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);        readymsg.Add((byte)AppointmentDataManager.AppointmentDataInstance().playerSitNo);        readymsg.Add(true);        NetWorkClient.GetInstance().SendMsg(readymsg);    }    public void SetUIAsLast()
    {
        alreadyPanel.transform.SetAsLastSibling();
        GameObject readybtn = alreadyPanel.transform.Find("Button_Ready").gameObject;        readybtn.GetComponent<Button>().interactable = true;    }    private void OnTickOut(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            byte sitNo = (byte)button;            uint playerid = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_[sitNo].playerid;            UMessage tickmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Exit);            tickmsg.Add(playerid);            tickmsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);            tickmsg.Add((byte)sitNo);            tickmsg.Add((byte)3);//0：游戏中有玩家退出，解散房间 1：房主退出房间，解散房间 2：玩家正常退出，不解散房间 3：被踢，不解散房间

            NetWorkClient.GetInstance().SendMsg(tickmsg);        }    }    private void OnSit(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            uint tindex = (uint)button;
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
        }    }    void RefreshSwitchBtn()
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
    }    private void OnSendSwitchMsg(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            uint target = (uint)button;

            //uint[] uiname = { 0, 1, 3, 2 };
            //target = uiname[target];

            UMessage sitmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Switch);            sitmsg.Add(GameMain.hall_.GetPlayerId());            sitmsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);            sitmsg.Add(AppointmentDataManager.AppointmentDataInstance().currentsource);            sitmsg.Add((byte)target);            NetWorkClient.GetInstance().SendMsg(sitmsg);            GameObject seatBG = alreadyPanel.transform.Find("table").gameObject;            GameObject switchbtn = seatBG.transform.Find("Player_" + (target + 1).ToString()).Find("Button_change").gameObject;            switchbtn.SetActive(false);        }    }
    public void ShowAllAppointments(byte gameid)
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
    }    private void ResfreshAppointmentPanel()
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
    }    void SendMatchRoomMsg()
    {
        fResfreshAppointmentPanelTime = 10;
        UMessage publicmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_PublicRooms);

        publicmsg.Add((byte)currentpickrooms_);

        NetWorkClient.GetInstance().SendMsg(publicmsg);
    }    void InitRoomsEvents()
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

    private void OnCreateRooms(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
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

            rulepanel_.transform.SetAsLastSibling();            rulepanel_.SetActive(true);
            isRuleEnterWay = false;
            AppointmentRoomPanel_.SetActive(false);
           
            //ResetCostContents(currentpickrooms_);
        }    }

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
        if (appointmentData.gameid == (byte)GameKind_Enum.GameKind_LandLords)        {            if (appointmentData.maxpower == 250)                ruletext = "打" + appointmentData.playtimes.ToString() + "局 不封顶";            else                ruletext = "打" + appointmentData.playtimes.ToString() + "局 最高" + appointmentData.maxpower + "倍";        }else if (appointmentData.gameid == (byte)GameKind_Enum.GameKind_GuanDan)        {            GuanDanAppointmentData GuanDanAppointmentData = (GuanDanAppointmentData)appointmentData;            if (GuanDanAppointmentData.terData_.playType == TePlayType.Times)            {                if (GuanDanAppointmentData.terData_.cp == CurrentPoker.Two)                    ruletext = "打" + GuanDanAppointmentData.terData_.times.ToString() + "局 双下"
                        + GuanDanAppointmentData.terData_.score.ToString() + "分 打2";                else                    ruletext = "打" + GuanDanAppointmentData.terData_.times.ToString() + "局 双下"                        + GuanDanAppointmentData.terData_.score.ToString() + "分 随机打";            }

            else            {                if (GuanDanAppointmentData.terData_.vectory == 1)                    ruletext = "连续打 过A胜利";                else                    ruletext = "连续打 过" + GuanDanAppointmentData.terData_.vectory.ToString() + "胜利";            }        }else if (appointmentData.gameid == (byte)GameKind_Enum.GameKind_Mahjong ||
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

        for (int index = 0; index < peoplebg.transform.childCount; index++)            peoplebg.transform.GetChild(index).gameObject.SetActive(false);        for (int index = 0; index < maxplayer; index++)
            peoplebg.transform.GetChild(index).gameObject.SetActive(true);

        for (int index = 0; index < appointmentData.currentPeople; index++)
            peoplebg.transform.GetChild(index).Find("Icon").gameObject.SetActive(true);
    }

    private void OnClickMatch(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);
            byte index = (byte)button;
            if (GameMain.hall_.GetPlayerData().GetDiamond() < AppointmentDataManager.AppointmentDataInstance().matchrooms_[index - 1].incoin_)
            {
                CCustomDialog.OpenCustomConfirmUI(2100);
                return;
            }

            //进入游戏
            GameMain.hall_.OnClickRoomIconBtn((byte)currentpickrooms_, index);        }    }

    private void OnClickPublicRoom(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //进入公开房间
            uint appointmentid = (uint)button;
            SendAppintmentJoinMsg(appointmentid);
        }    }

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

    private void OnCloseAllAppointments(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            AppointmentRoomPanel_.SetActive(false);        }    }

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

    public void Update()    {        RefreshTime();        if (isDownload_)
        {
            UpdateDownLoad();
        }        ResfreshAppointmentPanel();    }    void UpdateDownLoad()
    {
        uint percent = DownLoadProcessMgr.Instance.GetDownloadPercent();        if (waitingTips_ != null)        {            Text percentTx = waitingTips_.transform.Find("Text").gameObject.GetComponent<Text>();            percentTx.text = percent.ToString() + "%";        }        if (CGameResDownloadMgr.Instance.isAllGameResHaveDownOver())        {            if (waitingTips_ != null)                waitingTips_.SetActive(false);            if (isUpdateJoin_)            {                if (percent >= 100)                {                    isUpdateJoin_ = false;
                    isDownload_ = false;
                    //GameMain.hall_.EnterGameScene(joinGameKind_, GameTye_Enum.GameType_Appointment);
                    //Join2Msg();
                    GameKind_Enum GameKindEnum = (GameKind_Enum)AppointmentDataManager.AppointmentDataInstance().gameid;
                    if (GameKindEnum > GameKind_Enum.GameKind_None && GameKindEnum < GameKind_Enum.GameKind_Max)
                    {
                        GameMain.hall_.EnterGameScene(GameKindEnum, GameTye_Enum.GameType_Appointment,0,()=> { Join2Msg(); });
                    }
                }            }        }
    }

    //第二次加入消息
    void Join2Msg()
    {
        AppointmentDataManager.AppointmentDataInstance().kind = AppointmentKind.From_Appointment;

        UMessage joinmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Join_2);
        joinmsg.Add(GameMain.hall_.GetPlayerId());
        joinmsg.Add(appointmentID_);
        NetWorkClient.GetInstance().SendMsg(joinmsg);
    }}