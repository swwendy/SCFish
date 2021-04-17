//#define HAVE_LOBBY
#define MATCH_ROOM
using DG.Tweening;
using System.Collections;using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using USocket.Messages;
using UnityEngine.EventSystems;
using XLua;


//房间状态
[LuaCallCSharp]
public enum MahjongRoomState_Enum
{
    MjRoomState_Init = 0,

    MjRoomState_WaitPlayer,     //等人
    MjRoomState_WaitReady,          //等待准备

    MjRoomState_TotalBegin,         //总的开始

    MjRoomState_OnceBeginShow,      //每局开始前的显示
    MjRoomState_CountDownBegin,     //游戏开始倒计时

    MjRoomState_ThrowDice,          //掷骰子
    MjRoomState_DealMahjong,        //发牌
    MjRoomState_WaitChangeMj,       //换牌
    MjRoomState_ChangeMjCartoon,    //播放换牌动画
    MjRoomState_WaitMakeLack,       //定缺
    MjRoomState_MakeLackCartoon,    //播放定缺动画
    MjRoomState_BuHua,				//补花

    MjRoomState_WaitPlayerDeal,     //发一张牌给玩家并等待他出牌
    MjRoomState_CheckHuPengGang,    //玩家出牌后检测 别人能否胡 碰 杠
    MjRoomState_WaitHuPengGang,     //玩家出了一张牌 别人可以胡 碰 杠
    MjRoomState_WaitQiangGangHu,    //玩家杠了 别人可以抢杠胡
    MjRoomState_HuPokerShow,		//玩家胡牌之后  有一个show

    MjRoomState_OnceResult,         //一局结果
    MjRoomState_OnceEnd,            //

    MjRoomState_TotalResult,        //总的结果
    MjRoomState_TotalEnd,           //

    MjRoomState						//回收状态
}

[LuaCallCSharp]
public enum MjPlayerState_Enum
{
    MjPlayerState_Init = 0,
    MjPlayerState_Match,            //1匹配中
    MjPlayerState_GameIn,           //2游戏中
    MjPlayerState_OnGameButIsOver,  //3游戏中但游戏已经结束
    MjPlayerState_ReadyHall,        //玩家在大厅
    MjPlayerState_OnDesk,           //玩家在桌子上
    MjPlayerState_Ting,             //游戏中玩家准备听别人点炮的牌
    MjPlayerState_FeiTing,	        //游戏中玩家准备自摸

    MjPlayerState_Max
};


//换张的方向
[LuaCallCSharp]
public enum MjChangePokerDire_Enum
{
    MjChangePokerDire_Init = 0,

    MjChangePokerDire_Clockwise,    //顺时针
    MjChangePokerDire_AntiClockwise,    //逆时针
    MjChangePokerDire_Diagonal,         //对角线

    MjChangePokerDire
}

[LuaCallCSharp]
public enum MahjongPokerType_Enum
{
    MahjongPoker_Tiao = 0,      //条子
    MahjongPoker_Wan,           //万
    MahjongPoker_Bing,          //饼
    MahjongPoker_Gen,           // 风牌(东南西北中发白)

    MahjongPoker
}

[LuaCallCSharp]
public enum MjOtherDoing_Enum
{
    MjOtherDoing_Init = 0,  //过
    MjOtherDoing_Peng,      //碰
    MjOtherDoing_Gang,      //杠
    MjOtherDoing_Hu,        //和
    MjOtherDoing_Chi,       //吃
    MjOtherDoing_Ting,      //听
    MjOtherDoing_FeiTing,   //飞听
    MjOtherDoing,           
}

//麻将胡牌的牌型 按位
[LuaCallCSharp]
public enum MahjongHuType
{
    MjHu_Ping = 0,      //平胡
    MjHuHas_QiDui,      //是否七对
    MjHuHas_FourSame,   //手上是否有四张一样的牌（杠）
    MjHuHas_DuiDui,     //是否对对胡
    MjHuHas_QingYiSe,   //是否清一色
    MjHuHas_JiangDui,   //是否将对
    MjHuHas_JinGouDiao, //是否金钩钓
    MjHuHas_FourGang,   //是否有四个杠
    MjHuHas_HasYaoJiu,  //是否幺九
    MjHuHas_NoYaoJiu,   //是否是断幺九
    MjHuHas_MenQing,    //是否门清
    MjHuHas_DuanMen,    //是否断门
    MjHuHas_YiTiaoLong, //是否一条龙
    MjHuHas_DengZi,     //是否磴字（暗杠、花杠）
    MjHuHas_HeiZi,      //是否黑字（无花）
    MjHuHas_QiShouJiaoTing,//是否起手叫听
    MjHuHas_YaDang,       //是否压档
    MjHuHas_DuiDao,     //是否对倒
    MjHuHas_DaDiaoChe,  //是否大吊车
    MjHuHas_YaoJiuHua,	//幺九花

    //其他翻倍的
    MjHuOtherAdd_GangShangKaiHua = 20,
    MjHuOtherAdd_QiangGangHu,
    MjHuOtherAdd_GangHouPao,
    MjHuOtherAdd_TianHu,
    MjHuOtherAdd_DiHu,
    MjHuOtherAdd_DaFeiJiPro,
    MjHuOtherAdd_TingPaiDianPaoPro,
    MjHuOtherAdd_FeiTingZiMoPro,

    MjHuType_ZiMo = 30,
    MjHuType_DianPao,
    MahjongHu_End
}

//麻将中改变金钱的途径
[LuaCallCSharp]
public enum MjChangeCoinType_Enum
{
    MjChangeCoinType_Init,
    MjChangeCoinType_RoomCharge,        //房费
    MjChangeCoinType_Gang_Ming,
    MjChangeCoinType_Gang_Bu,			//补杠的
    MjChangeCoinType_Gang_An,
    MjChangeCoinType_DianPao,
    MjChangeCoinType_Zimo,
    MjChangeCoinType_BackTax,           //退税
    MjChangeCoinType_ChaDaJiao,         //查大叫
    MjChangeCoinType_ChaHuaZhu,         //查花猪
    MjChangeCoinType_HuJiaoZhuanYi,		//呼叫转移
    MjChangeCoinType_Hua,               //花
    MjChangeCoinType_QiDui,             //是否七对
    MjChangeCoinType_FourSame,          //手上是否有四张一样的牌（杠）
    MjChangeCoinType_DuiDui,            //是否对对胡
    MjChangeCoinType_QingYiSe,          //是否清一色
    MjChangeCoinType_JiangDui,          //是否将对
    MjChangeCoinType_JinGouDiao,        //是否金钩钓
    MjChangeCoinType_FourGang,          //是否有四个杠
    MjChangeCoinType_HasYaoJiu,         //是否幺九
    MjChangeCoinType_NoYaoJiu,          //是否是断幺九
    MjChangeCoinType_MenQing,           //是否门清
    MjChangeCoinType_DuanMen,           //是否断门
    MjChangeCoinType_YiTiaoLong,        //是否一条龙
    MjChangeCoinType_DengZi,            //是否磴字（暗杠、花杠）
    MjChangeCoinType_HeiZi,             //是否黑字（无花）
    MjChangeCoinType_QiShouJiaoTing,    //是否起手叫听
    MjChangeCoinType_YaDang,            //是否压档
    MjChangeCoinType_DuiDao,            //是否对倒
    MjChangeCoinType_DaDiaoChe,         //是否大吊车
    MjChangeCoinType_YaoJiuHua,         //是否幺九花
    MjChangeCoinType_GangShangKaiHua,
    MjChangeCoinType_QiangGangHu,
    MjChangeCoinType_GangHouPao,
    MjChangeCoinType_TianHu,
    MjChangeCoinType_DiHu,
    MjChangeCoinType_DaFeiJiPro,
    MjChangeCoinType_TingPaiDianPaoPro,
    MjChangeCoinType_FeiTingZiMoPro,
    MjChangeCoinType_JiaJiaJu,          //加价局翻倍
    MjChangeCoinType_QuanDaYuanPro ,    //全大元
    MjChangeCoinType_DaDaYuanPro,       //大大元
    MjChangeCoinType_HunYiSePro,        //混一色
    MjChangeCoinType_LenMenPro,         //冷门
    MjChangeCoinType_4YuanZiPro,        //四原子
    MjChangeCoinType_TaoHuaQiPro,       //套花齐
    MjChangeCoinType_CaBeiPro,          //擦背
    MjChangeCoinType_Bao3Kou,           //包三口
    MjChangeCoinType_Bao4Kou,           //包四口
    MjChangeCoinType_HuaKai,            //补花开
    MjChangeCoinType_Bao3Jia,		    //包三家
    MjChangeCoinType_Bird,              //抓鸟
    MjChangeCoinType
}

//起胡规则
[LuaCallCSharp]
public enum MinHuRule      
{
    eMHR_None,
    eMHR_1Fan,      //1番
    eMHR_Ying1Fan,  //硬一番
    eMHR_3Hua,      //3花
    eMHR_4Hua		//4花
};

[LuaCallCSharp]
public class MjProInfo
{
    public ushort m_iMaxPro;           //最大 倍数
    public Dictionary<MjChangeCoinType_Enum, byte> 
        m_CoinTypeProDict = new Dictionary<MjChangeCoinType_Enum, byte>();
};

[LuaCallCSharp]
public enum MjTingState_Enum
{
    MjTingState_Init,
    MjTingState_TingTypeNum = 2,
    MjTingState_WaitTing,
    MjTingState_WaitFeiTing,
    MjTingState_Ting,
    MjTingState_FeiTing,
}

//相对于自己座位的方向
[LuaCallCSharp]
public enum MjTileDirection_Enum
{
    TileDirection_Left,     //左边
    TileDirection_Right,    //右边
    TileDirection_Front,    //对面
    TileDirection_Count
}

//断线重连特殊数据处理标记
[LuaCallCSharp]
public enum MjTileMiddleEnterData_Enum
{
    MiddleEnterData_Normal,//通用处理
    MiddleEnterData_Cz,    //常州麻将处理
    MiddleEnterData_Hz,    //红中麻将处理
}

[Hotfix]
public class CGame_Mahjong : CGameBase
{
#if HAVE_LOBBY    Transform LobbyUITfm { get; set; }
#endif
    public Transform MainUITfm { get; set; }
    Transform ResultUITfm { get; set; }
    protected Transform LeftTileTfm { get; set; }
    protected Transform TingTfm { get; set; }
    protected Transform ChooseKongTfm { get; set; }
    Transform AnimationTfm;    Transform EffectTfm;
    protected Transform m_DiscardTfm;
    GameObject CurDiscardObj { get; set; }
    GameObject CurDiscardTipObj { get; set; }

    protected Dictionary<MjOtherDoing_Enum, Button> m_GameButton = new Dictionary<MjOtherDoing_Enum, Button>();
    Button m_ShowTingButton;
    protected Button m_TrustButton;

    public AssetBundle MahjongAssetBundle { get; private set; }
    public AssetBundle CommonAssetBundle { get; set; }
    public CustomCountdownImgMgr CCIMgr { get; private set; }

    public const int LocalHandTileLayer = 1 << 13;

    public MahjongRoomState_Enum GameState { get; private set; }
    public Canvas GameCanvas { get; private set; }

    byte m_nCurrenLevel = RoomInfo.NoSit;
    protected byte m_nTotalTilesNum = 108;
    byte m_nLeftTileNum;
    public bool IsFree = false;
    protected List<Mahjong_Role> m_PlayerList = new List<Mahjong_Role>();
    protected Dictionary<byte, byte> m_dictSitPlayer = new Dictionary<byte, byte>();
    protected Dictionary<byte, byte> m_dictTileValueNum = new Dictionary<byte, byte>();

    public MjProInfo m_ProInfo = new MjProInfo();

    public Mahjong_Desk Desk { get; set; }
    protected Text m_LeftNumText;
    public Transform ChangeTfm { get; set; }

    protected byte m_nMaxTileValue = 0xff;
    byte m_nOnTurnSit = RoomInfo.NoSit;
    byte m_nShowDiscardedValue = RoomInfo.NoSit;

    sbyte m_nWaitingLoad = 0;
    bool m_bReconnected = false;
    protected int m_nDealIndex = -1;//记录摸牌位置

    public byte m_AtomicCard = 0xff; //原子牌
    public int m_nBrightCardSit = -1;//明牌的方位
    public int m_nBrightCardIndex = -1;    //明牌的位置索引

    public int ChangeTileNum = 3;

    GameObject appointmentPanel_;    GameObject appointmentRulePanel_;    GameObject appointmentResultTotalPanel_;    List<Dictionary<byte, RoundResultData>> appointmentTotalResults_;    bool isOpenAppointmentFinalResults_;
    string appointmentGameRule_;
    IEnumerator appointmentGameRulePanelEnumerator = null;
    bool bExitAppointmentDialogState = false;
#if !MATCH_ROOM
    GameObject[] m_MatchBtns = new GameObject[2];
#endif

    public CGame_Mahjong(GameTye_Enum gameType, GameKind_Enum gameKind) : base(gameKind)
    {
        GameMode = gameType;        Bystander = false;        m_AtomicCard = 0xff;    }

    public override void Initialization()
    {
        base.Initialization();

        InitMsgHandle();
        CCIMgr = new CustomCountdownImgMgr();

        Camera.main.aspect = 16f / 9f;

        if (!m_bReconnected)
        {
            if (GameMode == GameTye_Enum.GameType_Appointment)
            {
                LoadAppintmentReadResource();
                m_nWaitingLoad = -1;
            }
            else if (GameMode == GameTye_Enum.GameType_Normal)
            {
#if MATCH_ROOM
                MatchRoom.GetInstance().ShowRoom((byte)GameType);
                m_nWaitingLoad = -1;
#endif

                OnClickLevelBtn(GameMain.hall_.CurRoomIndex);
            }        }
        else if (GameMode == GameTye_Enum.GameType_Normal)
        {
            MatchRoom.GetInstance().ShowRoom((byte)GameType);
            OnClickLevelBtn(GameMain.hall_.CurRoomIndex);
        }
        Load();
    }

    void LoadAppintmentReadResource()
    {
        //if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
        //    return;

        if (appointmentPanel_ != null)
            return;

        appointmentPanel_ = GameMain.hall_.gamerooms_.LoadAppintmentReadResource();
        if (appointmentPanel_ != null)
            InitAppointmentEvents();
    }

    void InitAppointmentEvents()
    {
        Button butn = appointmentPanel_.transform.Find("Top/Button_Return").GetComponent<Button>();        butn.onClick.AddListener(() => OnClickReturn(true));
    }

    bool Load()
    {
        if (m_PlayerList.Count == 0)
        {            if (m_nWaitingLoad > 0)                m_nWaitingLoad--;            if (m_nWaitingLoad != 0)                return false;
        }        else            return true;
        LoadResource();        InitPlayers();        m_nLeftTileNum = m_nTotalTilesNum;

        if (GameMode == GameTye_Enum.GameType_Contest || GameMode == GameTye_Enum.GameType_Record)
            EnterRoom(1, m_bReconnected);
        else if (GameMode == GameTye_Enum.GameType_Appointment)
            EnterAppointment();
        else
            EnterRoom(GameMain.hall_.CurRoomIndex, m_bReconnected);

#if MATCH_ROOM
        if (GameMode == GameTye_Enum.GameType_Normal)
        {
            MatchRoom.GetInstance().SetUIAsLast();
        }else if(GameMode == GameTye_Enum.GameType_Appointment)
        {
            GameMain.hall_.gamerooms_.SetUIAsLast();
        }
#endif
        return true;
    }

    void EnterAppointment()
    {
        MainUITfm.gameObject.SetActive(true);

        appointmentTotalResults_ = new List<Dictionary<byte, RoundResultData>>();
        isOpenAppointmentFinalResults_ = false;
        AppointmentDataManager.AppointmentDataInstance().interrupt = false;
        AppointmentDataManager.AppointmentDataInstance().interruptid = 0;

        Debug.Log("init results===============");

        LoadAppintmentReadResource();
        LoadAppointmentResultResource();
        LoadAppointmentRuleResource(1);
        //if (appointmentPanel_ != null)
        //    appointmentPanel_.transform.SetAsLastSibling();
    }

    void LoadAppointmentRuleResource(uint lunshu, bool isShow = false)
    {
        if (GameMode != GameTye_Enum.GameType_Appointment)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (appointmentRulePanel_ == null)
        {
            UnityEngine.Object obj0 = bundle.LoadAsset("Room_process");
            appointmentRulePanel_ = (GameObject)GameMain.instantiate(obj0);
            appointmentRulePanel_.transform.SetParent(GameCanvas.transform.Find("Root"), false);
            appointmentRulePanel_.transform.Find("Top/ButtonReturn").GetComponent<Button>().onClick.AddListener(() => OnClickReturn(true));

            XPointEvent.AutoAddListener(appointmentRulePanel_.transform.Find("ImageLeftBG").gameObject, OnGamePromptInofPanel, null);
        }
   
        Text ruleTx = appointmentRulePanel_.transform.Find("ImageLeftBG").Find("Text_lunshu").gameObject.GetComponent<Text>();
        AppointmentData data = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();

        if (data == null)
            return;

        if (lunshu > data.playtimes)
            lunshu = data.playtimes;

        if(data.maxpower == 250 || data.maxpower == 0)
            appointmentGameRule_ = lunshu.ToString() + "/" + data.playtimes.ToString() + "局 不封顶";
        else
            appointmentGameRule_ = lunshu.ToString() + "/" + data.playtimes.ToString() + "局 最高" + data.maxpower + "倍";

        ruleTx.text = appointmentGameRule_;

        Text RuleText = appointmentRulePanel_.transform.Find("ImageLeftBG/RuleInfo_BG/Image_info/Text").GetComponent<Text>();
        if(string.IsNullOrEmpty(RuleText.text))
        {
            string gameRuleTextData = null;
            GameFunction.GetAppointmentRuleTextData(ref gameRuleTextData, GetGameType());
            RuleText.text = gameRuleTextData;
        }
        appointmentRulePanel_.SetActive(isShow);
    }

    private void OnGamePromptInofPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {
        if (appointmentRulePanel_ == null ||             eventtype != EventTriggerType.PointerDown && eventtype != EventTriggerType.PointerUp)        {
            return;
        }

        if(appointmentGameRulePanelEnumerator != null && eventtype == EventTriggerType.PointerUp)
        {
            GameMain.ST(appointmentGameRulePanelEnumerator);
        }

        Transform promptInfoTextTransform = appointmentRulePanel_.transform.Find("ImageLeftBG/RuleInfo_BG/Image_info");
        promptInfoTextTransform.DOLocalMoveY(eventtype == EventTriggerType.PointerDown  ?                                              -promptInfoTextTransform.GetComponent<RectTransform>().sizeDelta.y : 0, 0.6f);    }


    void LoadAppointmentResultResource()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((int)GameKind_Enum.GameKind_Mahjong);        if (gamedata == null)            return;        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj1 = (GameObject)bundle.LoadAsset("Room_Result_4");
        appointmentResultTotalPanel_ = (GameObject)GameMain.instantiate(obj1);
        appointmentResultTotalPanel_.transform.SetParent(GameCanvas.transform.Find("Root"), false);
        appointmentResultTotalPanel_.SetActive(false);

        XPointEvent.AutoAddListener(appointmentResultTotalPanel_.transform.Find("ImageBG/Buttonclose").gameObject, OnCloseTotalResultPanel, null);
    }

    private void OnCloseTotalResultPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            appointmentResultTotalPanel_.SetActive(false);
            //GameMain.hall_.AnyWhereBackToLoginUI();
            BackToChooseLevel(false);
            AppointmentDataManager.AppointmentDataInstance().playerAlready = false;        }    }

    protected virtual string GetMahJiongGameRule()
    {
        return "";
    }

    void ShowAppointmentTotalResult()
    {
        if (appointmentResultTotalPanel_ == null)
            LoadAppointmentResultResource();

        if (appointmentRulePanel_ != null)
            appointmentRulePanel_.SetActive(false);

        OnEnd();

        GameObject playerBG = appointmentResultTotalPanel_.transform.Find("ImageBG").Find("Imageplayer").gameObject;
        //playerBG.transform.FindChild("4").gameObject.SetActive(false);

        string rule = GetMahJiongGameRule();

        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().maxpower == 0
            || AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().maxpower == 250)
            rule += "打" + AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().playtimes + "局 不封顶";
        else
            rule += "打" + AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().playtimes + "局 最高"
                + AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().maxpower + "倍";

        AppointmentRecord data = new AppointmentRecord();
        data.gameID = (byte)GameType;
        data.gamerule = rule;
        data.timeseconds = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().createtimeseconds;
        data.isopen = false;
        data.videoes = FriendsMomentsDataMamager.GetFriendsInstance().currentvideoid;
        data.recordTimeSeconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);

        Transform playerTransform = null;
        if (appointmentTotalResults_.Count == 0)
        {
            for (int index = 0; index < 4; index++)
            {
                Mahjong_Role role = m_PlayerList[index];
                playerTransform = playerBG.transform.Find((index + 1).ToString());
                Image headimg = playerTransform.Find("Head/HeadMask/ImageHead").gameObject.GetComponent<Image>();
                headimg.sprite = role.GetHeadImg();
                Text playerNameTx = playerTransform.Find("TextName").gameObject.GetComponent<Text>();
                playerNameTx.text = role.GetRoleName();

                playerTransform.Find("Text_jifen/TextNum_1").gameObject.SetActive(true);
                playerTransform.Find("Text_jifen/TextNum_2").gameObject.SetActive(false);

                Text totalcoinTx = playerTransform.Find("Text_jifen/TextNum_1").gameObject.GetComponent<Text>();
                totalcoinTx.text = "0";

                AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();

                playerdata.playerid = GameMain.hall_.GetPlayerId();
                playerdata.faceid = role.m_faceid;
                playerdata.playerName = role.GetRoleName();
                playerdata.url = role.m_url;
                playerdata.coin = 0;

                if (!data.result.ContainsKey(playerdata.playerid))
                    data.result.Add(playerdata.playerid, playerdata);
                playerTransform.gameObject.SetActive(true);
            }

            GameMain.hall_.gamerooms_.recordlist_.Insert(0, data);
            appointmentResultTotalPanel_.SetActive(true);
            return;
        }

        //Dictionary<byte, AppointmentSeat> seats = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_;
        Dictionary<byte,RoundResultData> resultDatas = appointmentTotalResults_[appointmentTotalResults_.Count - 1];

        foreach (var rdV in resultDatas)
        {
            //RoundResultData rd = resultDatas.Find(s => s.playerid == seat.Value.playerid);
            //if (rd == null)
            //{
            //    Debug.Log("error result key !" + seat.Value.playerid);
            //    continue;
            //}

            Mahjong_Role role = m_PlayerList[m_dictSitPlayer[rdV.Key]];
            var rd = rdV.Value;
            long coin = rd.coin;

            playerTransform = playerBG.transform.Find((rdV.Key + 1).ToString());
            Image headimg = playerTransform.Find("Head/HeadMask/ImageHead").gameObject.GetComponent<Image>();
            headimg.sprite = rd.headImg;
            Text playerNameTx = playerTransform.Find("TextName").gameObject.GetComponent<Text>();
            playerNameTx.text = rd.name;

            bool iswin = coin > 0;
            string totalcoinNode = "TextNum_1";
            if (!iswin)
                totalcoinNode = "TextNum_2";

            playerTransform.Find("Text_jifen/TextNum_1").gameObject.SetActive(iswin);
            playerTransform.Find("Text_jifen/TextNum_2").gameObject.SetActive(!iswin);

            Text totalcoinTx = playerTransform.Find("Text_jifen").Find(totalcoinNode).gameObject.GetComponent<Text>();
            string addorminus = "+";
            if (!iswin)
                addorminus = "";
            totalcoinTx.text = addorminus + coin.ToString();

            AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();

            playerdata.playerid = rd.playerid;
            playerdata.faceid = role.m_faceid;
            playerdata.playerName = rd.name;
            playerdata.url = role.m_url;
            playerdata.coin = rd.coin;

            if (!data.result.ContainsKey(playerdata.playerid))
                data.result.Add(playerdata.playerid, playerdata);
            playerTransform.gameObject.SetActive(true);
        }

        GameMain.hall_.gamerooms_.recordlist_.Insert(0, data);

        for (int index = 0; index < AppointmentDataManager.AppointmentDataInstance().perResultList_.Count; index++)
        {
            LoadAppointmentTotalResultItem(index);
        }

        appointmentResultTotalPanel_.SetActive(true);
        AppointmentDataManager.AppointmentDataInstance().perResultList_.Clear();
    }

    void LoadAppointmentTotalResultItem(int index)
    {
        if (appointmentResultTotalPanel_ == null)
            LoadAppointmentResultResource();

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        GameObject itemBG = appointmentResultTotalPanel_.transform.Find("ImageBG").
            Find("Viewport").Find("Content").gameObject;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Room_Result_xiangqing");
        GameObject item = (GameObject)GameMain.instantiate(obj0);
        item.transform.SetParent(itemBG.transform, false);

        Text roundTx = item.transform.Find("Text_ju").Find("Textnum").gameObject.GetComponent<Text>();
        roundTx.text = (index + 1).ToString();

        Transform roleRankingTransform = null;
        //Dictionary<byte, AppointmentSeat> seats = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().seats_;
        List<AppointmentResult> resultDatas = AppointmentDataManager.AppointmentDataInstance().perResultList_[index].Values.ToList();
        foreach (var rd in resultDatas)
        {
            //AppointmentResult rd = resultDatas.Find(s => s.playerid == seat.Value.playerid);
            //if (rd == null)
            //{
            //    Debug.Log("error result key !" + seat.Value.playerid);
            //    continue;
            //}
            Mahjong_Role role = m_PlayerList[m_dictSitPlayer[rd.sitNo]];
            roleRankingTransform = item.transform.Find("ImageBG/Text_ranking_" + (rd.sitNo + 1).ToString());
            Text percoin = roleRankingTransform.GetComponent<Text>();
            string addorminus = "+";
            if (rd.coin < 0)
                addorminus = "";

            percoin.text = addorminus + rd.coin.ToString();
            roleRankingTransform.gameObject.SetActive(true);
        }

        if (AppointmentDataManager.AppointmentDataInstance().perResultList_[index].Count < 4)
        {
            item.transform.Find("ImageBG").
                Find("Text_ranking_4").gameObject.SetActive(false);
        }
    }

    private void OnClosePerResultPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            bool isback2hall = (bool)button;
            if (!isback2hall)
            {
                BackToChooseLevel(false);
                AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = false;
            }
            else
            {
                OnEnd();
                //GameMain.hall_.AnyWhereBackToLoginUI();
                //AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = true;
                isOpenAppointmentFinalResults_ = true;
            }        }    }

    /// <summary>
    /// 显示约据游戏规则界面
    /// </summary>
    /// <returns>协程对象</returns>
    IEnumerator OpenAppointmentGameRulePanel()
    {
        yield return new WaitForEndOfFrame();
        OnGamePromptInofPanel(EventTriggerType.PointerDown,null,null);
        yield return new WaitForSeconds(5.0f);
        OnGamePromptInofPanel(EventTriggerType.PointerUp, null, null);
        appointmentGameRulePanelEnumerator = null;
    }


    void EnterRoom(byte level, bool bReconnect, bool bystander = false)    {
        Bystander = bystander;        if (m_nCurrenLevel == level)
            return;

        m_nCurrenLevel = level;        MainUITfm.gameObject.SetActive(true);
#if HAVE_LOBBY        LobbyUITfm.gameObject.SetActive(false);
#endif

        PlayerData pd = GameMain.hall_.GetPlayerData();        long coin = 0;        if (GameMode == GameTye_Enum.GameType_Normal)
        {#if MATCH_ROOM
            OnStartEndMatch(false, false);
#else
            OnStartEndMatch(false);
            MainUITfm.FindChild("Top/ButtonReturn").gameObject.SetActive(true);
#endif
            coin = pd.GetDiamond();
            //CustomAudioDataManager.GetInstance().PlayAudio(1001, false);
        }        else
        {            OnStartEndMatch(false, false);
            coin = 0;

            if (!bReconnect)
            {
                if (GameMode == GameTye_Enum.GameType_Contest)
                    MatchInGame.GetInstance().ShowWait();
                else if (GameMode == GameTye_Enum.GameType_Record)
                    GameVideo.GetInstance().ShowBegin();            }        }        m_PlayerList[0].SetupInfoUI(pd.GetPlayerID(), coin,            pd.GetPlayerName(), pd.GetPlayerIconURL(), (int)pd.PlayerIconId, pd.MasterScoreKindArray[(int)GameType], pd.PlayerSexSign);    }

    public override void ProcessTick()
    {
        base.ProcessTick();

        if (m_bReconnected)
        {
            GameMain.hall_.OnGameReconnect(GameType, GameMode);
            m_bReconnected = false;
        }


        if (!Load())
            return;

        CCIMgr.UpdateTimeImage();

        foreach (Mahjong_Role role in m_PlayerList)
        {
            role.OnTick();
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    m_ProInfo.m_iMaxPro = 250;
        //    m_ProInfo.m_iDuiDuiPro = 2;
        //    m_ProInfo.m_iQiDuiPro = 4;
        //    m_ProInfo.m_iQingYiSePro = 4;
        //    m_ProInfo.m_iJinGouDiaoPro = 2;
        //    m_ProInfo.m_iFourSamePro = 2;
        //    m_ProInfo.m_iOneGangPro = 2;
        //    m_ProInfo.m_iJiangDuiPro = 4;
        //    m_ProInfo.m_iHasYaoJiuPro = 4;
        //    m_ProInfo.m_iNoYaoJiuPro = 2;
        //    m_ProInfo.m_iMenQingPro = 2;

        //    JudgeTile.GetTileWinPro(new List<byte> { 1, 2, 3, 7, 8, 9, 0x21, 0x21, 0x22, 0x22, 0x33, 0x33, 0x39, 0x39 }, null, m_ProInfo);

        //    Dictionary<byte, HashSet<byte>> tingDict;
        //    //if(JudgeTile.CheckPrepareTing(new List<byte> { 6, 6, 6, 8, 8, 11, 12, 13, 13, 14, 15, 18, 19, 21 }, 2, out tingDict))
        //    //if (JudgeTile.CheckPrepareTing(new List<byte> { 6, 6, 6, 21, 26 }, 1, out tingDict))
        //    if (JudgeTile.CheckPrepareTing(new List<byte> { 1, 1, 2, 2, 3, 3, 26, 26, 26, 25, 25, 8, 8, 28 }, 1, out tingDict))
        //    {
        //        foreach (var tile in tingDict)
        //        {
        //            DebugLog.Log("ting+++++++++++++++remove:" + tile.Key);
        //            foreach (byte ting in tile.Value)
        //                DebugLog.Log("ting:" + ting);
        //            DebugLog.Log("End-----------------------");
        //        }
        //    }
        //}
        //if(Input.GetKeyDown(KeyCode.K))
        //{
        //    m_dictSitPlayer[0] = 0;
        //    m_dictSitPlayer[1] = 1;
        //    m_dictSitPlayer[2] = 2;
        //    m_dictSitPlayer[3] = 3;
        //    m_PlayerList[0].m_nSit = 0;
        //    m_PlayerList[1].m_nSit = 1;
        //    m_PlayerList[2].m_nSit = 2;
        //    m_PlayerList[3].m_nSit = 3;
        //}

        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
            return;

        if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            if (AppointmentDataManager.AppointmentDataInstance().interrupt)
            {
                AppointmentDataManager.AppointmentDataInstance().interrupt = false;
                isOpenAppointmentFinalResults_ = false;
                AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = false;
                ShowAppointmentTotalResult();
            }
            else if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend)
            {
                if (isOpenAppointmentFinalResults_)
                {
                    AppointmentDataManager.AppointmentDataInstance().interrupt = false;
                    isOpenAppointmentFinalResults_ = false;
                    AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = false;

                    ShowAppointmentTotalResult();
                }
            }
        }
    }

    public virtual void InitMsgHandle()
    {
    }

    public override bool HandleGameNetMsg(uint _msgType, UMessage _ms)
    {
        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;
        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_CHOOSElEVEL:
                {
                    byte nState = _ms.ReadByte();//1:id非法或Level非法 2：钱不够
                    byte level = _ms.ReadByte();                    ushort deskNum = _ms.ReaduShort();                    IsFree = (_ms.ReadByte() == 1);#if !MATCH_ROOM
                    int minCoin = _ms.ReadInt();#endif                    if (nState == 0)                    {#if MATCH_ROOM
                        MatchRoom.GetInstance().AddDesk(deskNum, 4);
#endif
                    }                    else                    {                        DebugLog.Log("Choose level failed: " + nState);#if !MATCH_ROOM
                        if (nState == 2)
                            CCustomDialog.OpenCustomConfirmUIWithFormatParamFunc(2307, (param) => OnClickReturn(false), minCoin);
                        else
#endif
                            CCustomDialog.OpenCustomConfirmUI(2301, (param) => OnClickReturn(false));
                        m_nCurrenLevel = RoomInfo.NoSit;                    }
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ROOMSTATE:
                {
                    OnStateChange((MahjongRoomState_Enum)_ms.ReadByte(), _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_OTHERENTER:
                {
                    byte sit = _ms.ReadByte();
                    uint userId = _ms.ReadUInt();
                    int face = _ms.ReadInt();
                    string url = _ms.ReadString();
                    long coin = _ms.ReadLong();
                    string name = _ms.ReadString();
                    byte clientSit = GetClientSit(sit, m_PlayerList[0].m_nSit);
                    m_dictSitPlayer[sit] = clientSit;
                    m_PlayerList[clientSit].m_nSit = sit;
                    m_PlayerList[clientSit].OnDisOrReconnect(_ms.ReadByte() == 0);
                    m_PlayerList[clientSit].SetupInfoUI(userId, coin, name, url, face, _ms.ReadSingle(), _ms.ReadByte());
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_FIRSTASKBANKERDEAL:
                {
                    byte sit = _ms.ReadByte();
                    byte doing = _ms.ReadByte();
                    byte num = _ms.ReadByte();
                    List<byte> list = new List<byte>();
                    for (byte i = 0; i < num; i++)
                        list.Add(_ms.ReadByte());
                    float time = _ms.ReadSingle();
                    uint sign = _ms.ReadUInt();
                    byte cSit = m_dictSitPlayer[sit];
                    OnChangeTurn(cSit, doing, sign, list);
                    Desk.StartCountdown(time, false, cSit == 0);
                    if (cSit == 0)
                        ((Mahjong_RoleLocal)m_PlayerList[0]).CheckTing();
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_GETPOKERASKDEAL:
                {
                    byte sit = _ms.ReadByte();
                    byte tile = _ms.ReadByte();
                    byte doing = _ms.ReadByte();
                    byte num = _ms.ReadByte();

                    List<byte> list = new List<byte>();
                    for (byte i = 0; i < num; i++)
                        list.Add(_ms.ReadByte());

                    float time = _ms.ReadSingle();
                    uint sign = _ms.ReadUInt();
                    HandleAskDeal(sit, tile, doing, sign, time, list);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_BACKLEAVE:
                {
                    byte state = _ms.ReadByte();
                    if (state == 0)
                    {
                        byte sit = _ms.ReadByte();
                        uint userid = _ms.ReadUInt();
                        string name = _ms.ReadString();

                        if (userid == GameMain.hall_.GetPlayerId())
                            BackToChooseLevel(false);
                        else if (m_dictSitPlayer.ContainsKey(sit))
                            m_PlayerList[m_dictSitPlayer[sit]].OnQuit();
                    }
                    else
                    {
                        //failed
                        CCustomDialog.OpenCustomConfirmUI(2305);
                    }
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ENTERTRUSTSTATE:
                {
                    SetTrustButtonActive(true);
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER:                {
                    byte state = _ms.ReadByte();
                    if (state == 0)
                        BackToChooseLevel(false);
                    else
                        CCustomDialog.OpenCustomConfirmUI(2305);
                }                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_AFTERQIANGGANGHUCUTCOIN:                {
                    byte kongSit = _ms.ReadByte();
                    HandleShowReward(m_dictSitPlayer[kongSit], PongKongType.ePKT_Pong2Kong, _ms);
                }                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_LIUJUINFO:
                {
                    byte num = _ms.ReadByte();
                    for(byte i = 0; i < num; i++)
                    {
                        int flag = _ms.ReadInt();
                        int coin = _ms.ReadInt();
                        m_PlayerList[m_dictSitPlayer[i]].ShowReward(coin, 1, flag);
                    }
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_HUJIAOZHUANYI:
                {
                    int coin = _ms.ReadInt();
                    byte sit = _ms.ReadByte();
                    int flag = 0;
                    GameKind.AddFlag((int)MjChangeCoinType_Enum.MjChangeCoinType_HuJiaoZhuanYi, ref flag);
                    m_PlayerList[m_dictSitPlayer[sit]].ShowReward(-coin, 1, flag);
                    sit = _ms.ReadByte();
                    m_PlayerList[m_dictSitPlayer[sit]].ShowReward(coin);
                }
                break;

            default:
                return false;
        }
        return true;
    }

    void BackToChooseLevel(bool kick)
    {
        GameOver();
        if (GameMode == GameTye_Enum.GameType_Appointment)            GameMain.hall_.SwitchToHallScene(true, 0);
#if MATCH_ROOM
        else if (GameMode == GameTye_Enum.GameType_Normal)
            MatchRoom.GetInstance().GameOver();
#endif
        else
            GameMain.hall_.SwitchToHallScene();
        if (kick)
            CCustomDialog.OpenCustomConfirmUI(2507);
    }

    public override void ResetGameUI()
    {
        base.ResetGameUI();

#if HAVE_LOBBY        Transform tfm = LobbyUITfm.FindChild("Bottom");
        tfm.FindChild("Image_coinframe").GetComponentInChildren<Text>().text = pd.GetCoin().ToString();
        tfm.FindChild("Image_DiamondFrame").GetComponentInChildren<Text>().text = pd.GetDiamond().ToString();
#endif
        ClearUIAnimation();
        ResetLeftTile();

        SetTrustButtonActive(false);

        ChooseKongTfm.gameObject.SetActive(false);
        ResultUITfm.gameObject.SetActive(false);
        ShowTingUI(false);
        ShowTingBtn(false);
        ShowGameButton(0);
    }

    public virtual void LoadMainUI(Transform root)
    {
    }

    void LoadResource()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((int)GameKind_Enum.GameKind_Mahjong);
        if (gamedata == null)
            return;

        MahjongAssetBundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
        if (MahjongAssetBundle == null)
            return;

        CommonAssetBundle = AssetBundleManager.GetAssetBundle("pokercommon.resource");        if (CommonAssetBundle == null)            return;
        Transform tfm = GameObject.Find("Game_Model/zhuomian").transform;
        m_DiscardTfm = tfm.Find("feipai");
        EffectTfm = tfm.Find("EffectPoint");
        GameCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        Transform root = GameCanvas.transform.Find("Root");
        GameObject obj;
        Button butn;
        Sprite sprite;

        PlayerData pd = GameMain.hall_.GetPlayerData();
        sprite = GameMain.hall_.GetIcon(pd.GetPlayerIconURL(), pd.GetPlayerID(), (int)pd.PlayerIconId);

#if HAVE_LOBBY        byte index = 0;
        Button[] buttons;

        //load lobby ui---------------------------------------------
        obj = (GameObject)MahjongAssetBundle.LoadAsset("Mahjong_Lobby");
        LobbyUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        LobbyUITfm.SetParent(root, false);

        butn = LobbyUITfm.FindChild("Top/ButtonReturn").GetComponent<Button>();
        butn.onClick.AddListener(() => OnClickReturn(true));

        tfm = LobbyUITfm.FindChild("Middle/Middle_Button");
        buttons = tfm.GetComponentsInChildren<Button>();
        index = 1;
        foreach(Button btn in buttons)
        {
            byte temp = index;
            btn.onClick.AddListener(() => OnClickLevelBtn(temp));
            index++;
        }

        tfm = LobbyUITfm.FindChild("Bottom/PlayerInfoBG");

        tfm.FindChild("TextName").GetComponent<Text>().text = pd.GetPlayerName();
        Image vipImg = tfm.FindChild("Image_Vip/Vip_Text/Num").gameObject.GetComponent<Image>();
        Image vipTypeImg = tfm.FindChild("Image_Vip").gameObject.GetComponent<Image>();
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle != null)
        {
            if (pd.GetVipLevel() == 0)
                vipTypeImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_hui");
            else
                vipTypeImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_jin");
            vipImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + pd.GetVipLevel().ToString());
        }

        Image icon = tfm.FindChild("Image_HeadFram/Image_Mask/Image_Head").GetComponent<Image>();
        icon.sprite = sprite;

        tfm = tfm.parent.FindChild("Image_coinframe");
        tfm.GetComponentInChildren<Text>().text = pd.GetCoin().ToString();

        tfm = tfm.parent.FindChild("Image_DiamondFrame");
        tfm.GetComponentInChildren<Text>().text = pd.GetDiamond().ToString();
        ////////////////////////////////////////////////////////////////////////
#endif
        obj = GameObject.Find("Game_Model/Zhuozi/mesh_Zhuo");
        Desk = obj.AddComponent<Mahjong_Desk>();
        Desk.GameBase = this;

        LoadMainUI(root);

        butn = MainUITfm.Find("Top/ButtonExpand").GetComponent<Button>();
        butn.onClick.AddListener(OnClickSet);

        butn = MainUITfm.Find("Top/ButtonReturn").GetComponent<Button>();
        butn.onClick.AddListener(() => OnClickReturn(false));

        tfm = MainUITfm.Find("Pop-up");
        Slider music = tfm.Find("Set/ImageBG/Slider_Music").gameObject.GetComponent<Slider>();
        Slider sound = tfm.Find("Set/ImageBG/Slider_Sound").gameObject.GetComponent<Slider>();
        music.value = AudioManager.Instance.MusicVolume;
        sound.value = AudioManager.Instance.SoundVolume;
        music.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.MusicVolume = value; });
        sound.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.SoundVolume = value; });

        AnimationTfm = MainUITfm.Find("Pop-up/Animation/Effectpoint");

        TingTfm = tfm.Find("ting");
        TingTfm.GetComponent<Button>().onClick.AddListener(() => ShowTingUI(false));

        LeftTileTfm = tfm.Find("shengyu");
        LeftTileTfm.GetComponent<Button>().onClick.AddListener(() => OnClickNum(false));

        m_GameButton[MjOtherDoing_Enum.MjOtherDoing_Gang].gameObject.AddComponent<CommonUserData>();
        m_GameButton[MjOtherDoing_Enum.MjOtherDoing_Init].gameObject.AddComponent<CommonUserData>();

        tfm = MainUITfm.Find("Top/Num");
        tfm.GetComponent<Button>().onClick.AddListener(() => OnClickNum(true));
        m_LeftNumText = tfm.GetComponentInChildren<Text>();
        m_LeftNumText.text = "";
        tfm.gameObject.SetActive(false);

        tfm = MainUITfm.Find("Top/ButtonTing");
        m_ShowTingButton = tfm.GetComponent<Button>();
        m_ShowTingButton.onClick.AddListener(() => ShowTingUI(true));
        m_ShowTingButton.gameObject.SetActive(false);

#if !MATCH_ROOM
        tfm = MainUITfm.FindChild("Middle/Button_zhunbeiBG");
        m_MatchBtns[0] = tfm.FindChild("Button_zhunbei").gameObject;
        m_MatchBtns[0].GetComponent<Button>().onClick.AddListener(() => OnClickMatchButton(0, true));
        m_MatchBtns[1] = tfm.FindChild("Button_quxiao").gameObject;
        m_MatchBtns[1].GetComponent<Button>().onClick.AddListener(() => OnClickMatchButton(1, true));
#endif

        ResetLeftTile();

        //load result ui
        obj = (GameObject)MahjongAssetBundle.LoadAsset("Mahjong_Result");
        ResultUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        ResultUITfm.SetParent(root, false);

        butn = ResultUITfm.Find("BottomButton/ButtonOk").GetComponent<Button>();
        butn.onClick.AddListener(() => OnClickResultButton(0));
        ResultUITfm.gameObject.SetActive(false);

        //load curDiscardTipObj
        obj = (GameObject)MahjongAssetBundle.LoadAsset("Effect_sign");
        CurDiscardTipObj = (GameObject)GameMain.instantiate(obj);

        GameFunction.PreloadPrefab(CommonAssetBundle, "Anime_startgame");

        if(GameMode == GameTye_Enum.GameType_Appointment)
        {
            butn.gameObject.SetActive(false);

            if (appointmentRulePanel_ != null)
                appointmentRulePanel_.SetActive(false);
        }
    }

    public virtual void InitPlayers()
    {
    }

    void OnClickReturn(bool playsound)
    {
        if (playsound)
            CustomAudioDataManager.GetInstance().PlayAudio(3006);

        if (GameMode == GameTye_Enum.GameType_Normal)        {
            //TryToLeaveRoom
#if MATCH_ROOM
            MatchRoom.GetInstance().OnClickReturn(0);
#else
#if !HAVE_LOBBY
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_PLAYERLEAVEROOMSER);
#else
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_APPLYLEAVE);
#endif
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);
#endif
        }
        else if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            CCustomDialog.OpenCustomDialogWithTipsID(1625, OnExitAppointmentGame);
            bExitAppointmentDialogState = true;
        }        else            MatchInGame.GetInstance().OnClickReturn();    }

    void OnExitAppointmentGame(object call)
    {
        if ((int)call == 0)            return;

        bExitAppointmentDialogState = false;
        byte sitNo = AppointmentDataManager.AppointmentDataInstance().playerSitNo;
        uint playerid = GameMain.hall_.GetPlayerId();
        UMessage tickmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Exit);

        tickmsg.Add(playerid);
        tickmsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);
        tickmsg.Add((byte)sitNo);
        tickmsg.Add((byte)2);

        NetWorkClient.GetInstance().SendMsg(tickmsg);
    }

    void OnClickLevelBtn(byte index)
    {
        if (m_nCurrenLevel == RoomInfo.NoSit)
        {
            m_nCurrenLevel = 255;

            //CustomAudioDataManager.GetInstance().PlayAudio(3006);
            uint type = 100000 + (uint)GameType * 100 + 3;
            UMessage msg = new UMessage(type);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(index);
            HallMain.SendMsgToRoomSer(msg);
        }
    }

    void OnClickSet()
    {
        CustomAudioDataManager.GetInstance().PlayAudio(3006);
        MainUITfm.Find("Pop-up/Set").gameObject.SetActive(true);
    }

    protected byte GetClientSit(byte sit, byte localSit)
    {
        return (byte)((4 + sit - localSit) % 4);
    }

    protected bool HandleEnterRoom(uint _msgType, UMessage _ms)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(3008, false);

        if (GameMode == GameTye_Enum.GameType_Contest)            MatchInGame.GetInstance().ResetGui();
        uint roomId = _ms.ReadUInt();
        byte level = _ms.ReadByte();

        InitChangeCoinTypeData(_ms);

        OnStateChange((MahjongRoomState_Enum)_ms.ReadByte(), null);

        byte localSit = _ms.ReadByte();
        m_dictSitPlayer[localSit] = 0;
        m_PlayerList[0].m_nSit = localSit;
        if (GameMode == GameTye_Enum.GameType_Normal)
            EnterRoom(level, false);

        Desk.OnRotDir(localSit);
        long coin = _ms.ReadLong();
        PlayerData pd = GameMain.hall_.GetPlayerData();        m_PlayerList[0].SetupInfoUI(pd.GetPlayerID(), coin, pd.GetPlayerName(), pd.GetPlayerIconURL(), (int)pd.PlayerIconId,             pd.MasterScoreKindArray[(int)GameType], pd.PlayerSexSign);
        byte otherNum = _ms.ReadByte();
        for (byte i = 0; i < otherNum; i++)
        {
            byte sit = _ms.ReadByte();
            byte clientSit = GetClientSit(sit, localSit);
            m_dictSitPlayer[sit] = clientSit;
            m_PlayerList[clientSit].m_nSit = sit;
            uint userId = _ms.ReadUInt();
            int face = _ms.ReadInt();
            string url = _ms.ReadString();
            coin = _ms.ReadLong();
            string name = _ms.ReadString();
            m_PlayerList[clientSit].OnDisOrReconnect(_ms.ReadByte() == 0);            m_PlayerList[clientSit].SetupInfoUI(userId, coin, name, url, face, _ms.ReadSingle(), _ms.ReadByte());
        }

        OnStartEndMatch(false, false);

        return true;
    }

    //初始化胡牌倍数
    protected virtual void InitChangeCoinTypeData(UMessage _ms, bool midEnt = false)
    {
        m_ProInfo.m_iMaxPro = _ms.ReaduShort();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuiDui] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QiDui] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QingYiSe] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_JinGouDiao] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_FourSame] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_Gang_Ming] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DaFeiJiPro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_TingPaiDianPaoPro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_FeiTingZiMoPro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_TianHu] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_JiaJiaJu] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuanMen] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_YiTiaoLong] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DengZi] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_HeiZi] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QiShouJiaoTing] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_YaDang] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuiDao] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DaDiaoChe] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_JiangDui] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_HasYaoJiu] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_NoYaoJiu] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_MenQing] = _ms.ReadByte();
    }

    //mode: 0:normal 1:reconnect 2:bystander
    void OnStateChange(MahjongRoomState_Enum state, UMessage _ms, byte mode = 0, float timeLeft = 0f)
    {
        if (GameState != MahjongRoomState_Enum.MjRoomState_WaitReady &&
            GameState == state && mode == 0)
            return;

        DebugLog.Log(string.Format("room state change: ({0}->{1})", GameState, state));

        OnQuitState(GameState);

        GameState = state;

        OnEnterState(GameState, _ms, mode, timeLeft);
    }

    virtual protected void OnQuitState(MahjongRoomState_Enum state)
    {
        if(state == MahjongRoomState_Enum.MjRoomState_OnceEnd)
            CustomAudioDataManager.GetInstance().PlayAudio(3008, false);
    }

    public virtual void OnEnterState(MahjongRoomState_Enum state, UMessage _ms, byte mode, float timeLeft)
    {
        switch (state)
        {
#if MATCH_ROOM
            case MahjongRoomState_Enum.MjRoomState_WaitPlayer:
                {
                    if (GameMode == GameTye_Enum.GameType_Normal)
                        MatchRoom.GetInstance().ShowKickTip(false);                }                break;

            case MahjongRoomState_Enum.MjRoomState_WaitReady:                {
                    if (GameMode == GameTye_Enum.GameType_Normal)
                    {                        float time = _ms.ReadSingle();
                        MatchRoom.GetInstance().ShowKickTip(true, time);
                    }                }                break;#endif

            case MahjongRoomState_Enum.MjRoomState_OnceBeginShow:
                {
                    OnEnd();

                    if (GameMode == GameTye_Enum.GameType_Contest)
                        MatchInGame.GetInstance().ShowBegin(Bystander);
#if MATCH_ROOM
                    else if (GameMode == GameTye_Enum.GameType_Normal)
                        MatchRoom.GetInstance().StartGame();
#endif
                    if (appointmentRulePanel_ != null)
                        appointmentRulePanel_.SetActive(true);

                    appointmentGameRulePanelEnumerator = OpenAppointmentGameRulePanel();
                    GameMain.SC(appointmentGameRulePanelEnumerator);
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_CountDownBegin:
                {
                    PlayEffect(true, "Anime_startgame", 1f, 0, null, CommonAssetBundle);
                    CustomAudioDataManager.GetInstance().PlayAudio(3004);
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_ThrowDice:
                {
                    if(mode == 0)
                    {
                        byte DealerSit = _ms.ReadByte();
                        byte point1 = _ms.ReadByte();
                        byte point2 = _ms.ReadByte();
                        DebugLog.Log("Zhuang:" + DealerSit + " ThrowDice:" + point1 + " " + point2);

                        OnStartDie(DealerSit, point1, point2);
                    }
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_WaitPlayerDeal:
                {
                    if (mode == 1)
                    {
                        byte tile = _ms.ReadByte();
                        byte doing = _ms.ReadByte();
                        byte num = _ms.ReadByte();

                        List<byte> list = new List<byte>();
                        for (byte i = 0; i < num; i++)
                            list.Add(_ms.ReadByte());

                        uint sign = _ms.ReadUInt();
                        if(m_nOnTurnSit == 0)
                            ShowGameButton(doing, sign, list);
                        Desk.StartCountdown(timeLeft, false, m_nOnTurnSit == 0);
                    }
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_WaitHuPengGang:
            case MahjongRoomState_Enum.MjRoomState_WaitQiangGangHu:
                {
                    if (mode > 0)
                        HandleBackOtherDiscard(0, _ms);
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_OnceResult:
                {
                    if (mode == 1)
                        HandleResult(0, _ms);
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_OnceEnd:
                {
                    if (mode == 1)
                        HandleResult(0, _ms);

                    if (ResultUITfm != null)
                    {
                        Button againButton = ResultUITfm.Find("BottomButton/ButtonAgain").GetComponent<Button>();
                        againButton.interactable = true;

                        Button okButton = ResultUITfm.Find("BottomButton/ButtonOk").GetComponent<Button>();
                        okButton.interactable = true;
                    }
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_TotalEnd:
                {
                    if (ResultUITfm == null)
                        break;

                    if (ResultUITfm.gameObject.activeSelf)
                    {
                        Button againButton = ResultUITfm.Find("BottomButton/ButtonAgain").GetComponent<Button>();
                        againButton.interactable = true;
                        againButton.onClick.RemoveAllListeners();
                        againButton.onClick.AddListener(() => OnClickResultButton(1, true));

                        Button okButton = ResultUITfm.Find("BottomButton/ButtonOk").GetComponent<Button>();
                        okButton.interactable = true;
                    }                }
                break;

            default:
                break;
        }
    }

    protected void OnChangeTurn(byte sit, byte doing = 0, uint sign = 0, List<byte> repeatDoingList = null)
    {
        //DebugLog.Log(string.Format("turn change: ({0}->{1}) doing:{2} sign:{3}", m_nOnTurnSit, sit, doing, sign));

        if (m_nOnTurnSit != RoomInfo.NoSit && m_nOnTurnSit != sit)
        {
            m_PlayerList[m_nOnTurnSit].OnTurn = false;
            if (m_nOnTurnSit == 0)
                ShowGameButton(0);
        }
        if (sit != RoomInfo.NoSit)
        {
            m_PlayerList[sit].OnTurn = true;
            Desk.ShowTurn(m_PlayerList[sit].m_nSit);

            if (sit == 0)
                ShowGameButton(doing, sign, repeatDoingList);
        }
        m_nOnTurnSit = sit;
    }

    public void ShowGameButton(byte doing, uint sign = 0, List<byte> repeatDoingList = null)
    {
        if (GameMode == GameTye_Enum.GameType_Record)
            return;

        if (doing == 0)
        {
            foreach (Button btn in m_GameButton.Values)
            {
                btn.gameObject.SetActive(false);
                btn.interactable = true;
            }
            return;
        }

        foreach(var btn in m_GameButton)
        {
            if(GameKind.HasFlag((int)btn.Key, doing))
                btn.Value.gameObject.SetActive(true);
        }

        if (GameKind.HasFlag((int)MjOtherDoing_Enum.MjOtherDoing_Gang, doing))//杠
        {
            CommonUserData cud = m_GameButton[MjOtherDoing_Enum.MjOtherDoing_Gang].gameObject.GetComponent<CommonUserData>();
            cud.userData = repeatDoingList;
        }

        m_GameButton[MjOtherDoing_Enum.MjOtherDoing_Init].gameObject.SetActive(m_PlayerList[0].TingState < MjTingState_Enum.MjTingState_Ting);
        if(sign > 0)
        {
            CommonUserData cudGuo = m_GameButton[MjOtherDoing_Enum.MjOtherDoing_Init].gameObject.GetComponent<CommonUserData>();
            cudGuo.userData = sign;
        }
    }

    public virtual void OnEnd()
    {
        if (m_PlayerList.Count == 0)            return;
        GameMain.Instance.StopAllCoroutines();

        SetCurDiscardTile(null);
        Desk.OnEnd();
        ShowTips(false);

        foreach (Mahjong_Role role in m_PlayerList)
            role.OnEnd();

        m_nOnTurnSit = RoomInfo.NoSit;
        m_nShowDiscardedValue = RoomInfo.NoSit;
        m_nDealIndex = -1;
        m_nLeftTileNum = m_nTotalTilesNum;
        appointmentGameRulePanelEnumerator = null;
        m_nBrightCardSit = -1;      //明牌的方位
        m_nBrightCardIndex = -1;    //明牌的位置索引
        m_AtomicCard = 0xff;           //原子牌
        for (int i = m_DiscardTfm.childCount - 1; i >= 0; i--)
            GameObject.DestroyImmediate(m_DiscardTfm.GetChild(i).gameObject);

        ResetGameUI();
    }

    void GameOver(bool showMatch = false)    {        if (m_PlayerList.Count == 0)            return;

        //if (GameMain.hall_.isGetRelief)
        //    GameMain.hall_.ShowRelief();

        OnEnd();

        if (bExitAppointmentDialogState)
        {
            CCustomDialog.CloseCustomDialogUI();
        }        Desk.ShowTurn(RoomInfo.NoSit);
        m_dictSitPlayer.Clear();
        for (int i = 1; i < m_PlayerList.Count; i++)
            m_PlayerList[i].Init();
        m_nWaitingLoad = 0;

        IsFree = false;
        m_nCurrenLevel = RoomInfo.NoSit;

        OnStartEndMatch(false, showMatch);

        if (Bystander)
        {
            Bystander = false;
            OnLeaveLookRoom(GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_LEAVEONLOOKERROOM);
        }
    }

    public override void OnDisconnect(bool over)
    {
        if (over)
            GameOver();
        else
            OnEnd();
    }

    protected void OnClickGameBtn(MjOtherDoing_Enum doing, byte chooseIndex, GameCity.EMSG_ENUM msgType)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(3006);

        byte value = 0;
        if(!GetOtherDoingValue(ref value,doing,chooseIndex))
        {
            return;
        }

        CommonUserData cudGuo = m_GameButton[MjOtherDoing_Enum.MjOtherDoing_Init].gameObject.GetComponent<CommonUserData>();
        uint sign = cudGuo.userData == null ? 0 : (uint)cudGuo.userData;

        UMessage msg = new UMessage((uint)msgType);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((byte)doing);
        msg.Add(value);
        msg.Add(sign);
        HallMain.SendMsgToRoomSer(msg);

        foreach (Button btn in m_GameButton.Values)
        {
            btn.interactable = false;
        }

        DebugLog.Log("OnClickGameButn :" + doing + " sign:" + sign);
    }

    protected virtual bool GetOtherDoingValue(ref byte dongValue,MjOtherDoing_Enum doing, byte chooseIndex)
    {
        if (doing == MjOtherDoing_Enum.MjOtherDoing_Gang)
        {
            CommonUserData cud = m_GameButton[doing].gameObject.GetComponent<CommonUserData>();
            if (cud == null || cud.userData == null)
                return false;

            List<byte> userDataList = (List<byte>)cud.userData;
            if (userDataList.Count == 0)
                return false;

            if (chooseIndex != RoomInfo.NoSit)
            {
                dongValue = userDataList[chooseIndex];
                ChooseKongTfm.gameObject.SetActive(false);
            }
            else if (userDataList.Count == 1)
                dongValue = userDataList[0];
            else
            {
                ChooseKongTfm.gameObject.SetActive(true);
                int i = 0;
                Transform tfm;
                for (; i < userDataList.Count; i++)
                {
                    tfm = ChooseKongTfm.GetChild(i);
                    tfm.gameObject.SetActive(true);
                    foreach (Transform child in tfm)
                    {
                        child.GetComponent<Image>().sprite =
                            MahjongAssetBundle.LoadAsset<Sprite>("mahjong_" + userDataList[i].ToString("X2"));
                    }
                }
                for (; i < ChooseKongTfm.childCount; i++)
                    ChooseKongTfm.GetChild(i).gameObject.SetActive(false);

                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 表情消息处理
    /// </summary>
    /// <param name="_msgType"></param>
    /// <param name="_ms"></param>
    protected bool HandEmotionNetMsg(uint _msgType, UMessage _ms)
    {
        byte sign = _ms.ReadByte();
        byte sit = _ms.ReadByte();
        string name = _ms.ReadString();
        m_PlayerList[m_dictSitPlayer.ContainsKey(sit) ? m_dictSitPlayer[sit] : 0].OnEmotion(sign);
        return true;
    }

    public void SetTileMat(Transform tfm, byte value, bool bBlock = false)
    {
        if (value == 0 && !bBlock)
            return;

        MeshRenderer mr = tfm.GetComponent<MeshRenderer>();
        if (mr == null)
            return;

        string mat = "Material_Ma_" + (bBlock ? "00" : value.ToString("X2"));
        mr.material = MahjongAssetBundle.LoadAsset<Material>(mat);
    }

    public GameObject CreateTile(byte value, Vector3 pos, Quaternion rot, byte sit)
    {
        string name = value.ToString("X2");
        Transform parent = m_DiscardTfm.Find(name);
        if (parent == null)
        {
            parent = new GameObject(name).transform;
            parent.SetParent(m_DiscardTfm, false);
        }
        GameObject obj = (GameObject)MahjongAssetBundle.LoadAsset("mesh_majiang");
        obj = GameMain.Instantiate(obj, pos, rot);
        SetTileMat(obj.transform, value);
        obj.name = sit.ToString();
        obj.transform.SetParent(parent, true);
        return obj;
    }

    public GameObject FindLastDiscardTile(byte sit, byte value)
    {
        string name = value.ToString("X2");
        Transform parent = m_DiscardTfm.Find(name);
        if (parent == null)
            return null;

        name = sit.ToString();
        for(int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform tfm = parent.GetChild(i);
            if (tfm.name == name)
                return tfm.gameObject;
        }

        return null;
    }

    public void SetCurDiscardTile(GameObject tileObj, bool attachTip = true)
    {
        if (CurDiscardObj != null && CurDiscardTipObj != null)
            CurDiscardTipObj.transform.SetParent(null, false);

        if (tileObj == null)
            GameObject.Destroy(CurDiscardObj);
        CurDiscardObj = tileObj;

        if (attachTip)
            SetDiscardTip();
    }

    public void SetDiscardTip(GameObject obj = null)
    {
        if (CurDiscardObj == null)
            return;

        if (obj != null && CurDiscardObj != obj)
            return;

        if(CurDiscardTipObj != null)
            CurDiscardTipObj.transform.SetParent(CurDiscardObj.transform.Find("Effectpoint"), false);
    }

    public virtual Transform CreatePongOrKong(byte value, PongKongType type, Transform target, MjTileDirection_Enum direction = MjTileDirection_Enum.TileDirection_Front, byte firstValue = 0)
    {
        string name = value.ToString("X2");
        Transform parent = m_DiscardTfm.Find(name);
        if (parent == null)
        {
            parent = new GameObject(name).transform;
            parent.SetParent(m_DiscardTfm, false);
        }

        GameObject obj = (GameObject)MahjongAssetBundle.LoadAsset(type == PongKongType.ePKT_Kong_Concealed ? "majiang_4an" : "majiang_4ming");
        obj = GameMain.Instantiate(obj, target.position, target.rotation);
        Transform tfm = obj.transform;
        tfm.SetParent(parent, true);
        tfm.name = "pong";

        int num = (type == PongKongType.ePKT_Pong ? 3 : 4);
        int i = 0;
        for (; i < num; i++)
            SetTileMat(tfm.GetChild(i), value);
        for (; i < tfm.childCount; i++)
            tfm.GetChild(i).gameObject.SetActive(false);

        return tfm;
    }

    protected void ShowTips(bool show, string tips = "")
    {
        Transform tfm = MainUITfm.Find("Pop-up/Tips");
        tfm.gameObject.SetActive(show);
        if (show)
        {
            tfm.GetComponentInChildren<Text>().text = tips;
        }
    }

    public void OnTileNumChanged(int sub)
    {
        if (sub == 0)
            m_LeftNumText.transform.parent.gameObject.SetActive(true);

        m_nLeftTileNum = (byte)(m_nLeftTileNum - sub);
        m_LeftNumText.text = m_nLeftTileNum.ToString();
    }

    protected virtual bool HandleSendTile(uint _msgType, UMessage _ms)
    {
        byte zhuangSit = _ms.ReadByte();
        zhuangSit = m_dictSitPlayer[zhuangSit];
        List<byte> list = new List<byte>();
        byte num = _ms.ReadByte();
        for (int i = 0; i < num; i++)
        {
            list.Add(_ms.ReadByte());
        }

        m_PlayerList[0].m_HaveTiles = new List<byte>(list);
        for(int i = 1; i < 4; i++)
            m_PlayerList[i].m_HaveTiles = new List<byte>(new byte[i == zhuangSit ? 14 : 13]);

        List<byte> players = new List<byte>(m_dictSitPlayer.Values);
        int id = players.FindIndex(s => s == zhuangSit);
        List<byte> temp = new List<byte>(players.GetRange(0, id));
        players.RemoveRange(0, id);
        players.AddRange(temp);
        GameMain.SC(ShowSendTile(m_nDealIndex, list, zhuangSit, players));

        int tileNum = 13 * players.Count + 1;
        int tileBeginSit = m_nDealIndex / 100;
        int index = m_nDealIndex % 100;
        while (tileNum > 0)
        {
            int remain = m_PlayerList[tileBeginSit].GetWallTileCount() - index;
            if (tileNum >= remain)
            {
                tileNum -= remain;
                index = 0;
                tileBeginSit = (tileBeginSit + 3) % 4;
            }
            else
            {
                index += (tileNum - 1);
                tileNum = 0;
            }
        }
        
        m_nDealIndex = tileBeginSit * 100 + index + 1;
        return true;
    }

    protected virtual IEnumerator ShowSendTile(int dealIndex, List<byte> list, byte dealerSit, List<byte> players)
    {
        int tileBeginSit = dealIndex / 100;
        int index = dealIndex % 100;
        int showedNum = 0;

        while (list.Count > 0)
        {
            List<byte> childList;
            if (list.Count > 4)
            {
                childList = list.GetRange(0, 4);
                list.RemoveRange(0, 4);
            }
            else
            {
                childList = new List<byte>(list);
                list.Clear();
            }

            int count = childList.Count;
            int extra;
            foreach (byte sit in players)
            {
                List<byte> child = childList;
                if (count < 4)
                {
                    if (sit == dealerSit)//庄家跳牌
                    {
                        extra = m_PlayerList[tileBeginSit].HideWallTile(index + 4, 1, GetBrightCardIndex(tileBeginSit));
                        if (extra != 0)
                        {
                            int wallCount = m_PlayerList[tileBeginSit].GetWallTileCount();
                            int next = (tileBeginSit + 3) % 4;
                            int nextId = index + 4 - wallCount;
                            extra = m_PlayerList[next].HideWallTile(nextId, extra, GetBrightCardIndex(next));
                            //if (players.Count == 4)
                            //    dealIndex = next * 100 + nextId + 1;
                            //else if (4 - nextId > players.Count)
                            //    dealIndex = tileBeginSit * 100 + index + players.Count;
                            //else
                            //    dealIndex = next * 100 + nextId + players.Count - 4;
                        }
                        //else
                        //    dealIndex = tileBeginSit * 100 + index + players.Count;

                        if (child.Count == 1)
                            child.Add(0);
                    }
                    else
                        child = childList.GetRange(0, 1);

                    count = 1;
                }

                extra = m_PlayerList[tileBeginSit].HideWallTile(index, count, GetBrightCardIndex(tileBeginSit));
                if (extra != 0)
                {
                    tileBeginSit = (tileBeginSit + 3) % 4;
                    index = extra;
                    extra = m_PlayerList[tileBeginSit].HideWallTile(0, extra, GetBrightCardIndex(tileBeginSit));
                    if (extra != 0)
                        DebugLog.LogError("no tiles can send!!!");
                }
                else
                    index += count;

                m_PlayerList[sit].SendTiles(child, showedNum);

                yield return new WaitForSecondsRealtime(0.1f);
            }

            showedNum += count;
        }

        //游戏暂停时其他玩家已经出牌(碰，吃等)导致的牌数不对
        foreach (byte sit in players)
        {
            m_PlayerList[sit].ReArrayTiles();
        }
        
        ((Mahjong_RoleLocal)m_PlayerList[0]).SortTiles(0.5f);

        yield return new WaitForSecondsRealtime(1f);
    }

    protected virtual bool HandleBackOtherDiscard(uint _msgType, UMessage _ms)
    {
        byte sit = _ms.ReadByte();
        byte tile = _ms.ReadByte();
        byte doing = _ms.ReadByte();
        uint sign = _ms.ReadUInt();

        if (!m_dictSitPlayer.ContainsKey(sit))
            return false;

        if (_msgType == 0)
            m_PlayerList[m_dictSitPlayer[sit]].UpdateTiles(1);
        else
            m_PlayerList[m_dictSitPlayer[sit]].OnDiscardTile(tile);

        ShowGameButton(doing, sign, new List<byte> { tile });

        //DebugLog.Log("discard other Sit:" + m_dictSitPlayer[sit] + " tile:" + tile + " doing:" + doing);

        return true;
    }

    protected void HandleAskDeal(byte sit, byte tile, byte doing, uint sign, float time, List<byte> listKong = null, bool bPause = false)
    {
        if (!m_dictSitPlayer.ContainsKey(sit))
            return;

        bool reverse = (time < 0f);
        byte cSit = m_dictSitPlayer[sit];

        OnChangeTurn(cSit, doing, sign, listKong);
        if (reverse)
            Desk.StartCountdown(0);
        else
            Desk.StartCountdown(time, bPause, cSit == 0);

        //wall tile
        HideWallTile(1, reverse);

        m_PlayerList[cSit].DealTile(tile, cSit == 0 || GameMode == GameTye_Enum.GameType_Record, reverse);
        //DebugLog.Log("Handle deal Sit:" + cSit + " tile:" + tile + " doing:" + doing + " index:"+m_nDealIndex);
    }

    public virtual void CreateWallTiles()
    {
    }

    public void ShowWallTiles()
    {
        foreach (Mahjong_Role role in m_PlayerList)
            role.ShowWallTiles();
    }

    protected void HideWallTile(byte num, bool reverse)
    {
        int tileBeginSit = m_nDealIndex / 100;
        int index = m_nDealIndex % 100;
        if (!reverse)
        {
            int extra = m_PlayerList[tileBeginSit].HideWallTile(index, num, GetBrightCardIndex(tileBeginSit));
            if (extra != 0)
            {
                extra = m_PlayerList[tileBeginSit].HideWallTile(index + (num - extra) + 1, extra, GetBrightCardIndex(tileBeginSit));
                if (extra != 0)
                {
                    tileBeginSit = (tileBeginSit + 3) % 4;
                    index = extra;
                    extra = m_PlayerList[tileBeginSit].HideWallTile(0, extra, GetBrightCardIndex(tileBeginSit));
                    if (extra != 0)
                    {
                        extra = m_PlayerList[tileBeginSit].HideWallTile(1, extra, GetBrightCardIndex(tileBeginSit));
                        if (extra != 0)
                            DebugLog.LogError("no tiles can deal!!! " + m_nDealIndex + " left:" + m_nLeftTileNum);
                        else
                            index++;
                    }
                }
                else
                    index += num + 1;
            }
            else
                index += num;
        }
        else
        {
            int extra = m_PlayerList[tileBeginSit].HideWallTile(index - 1, -num, GetBrightCardIndex(tileBeginSit));
            if (extra != 0)
            {
                tileBeginSit = (tileBeginSit + 1) % 4;
                index = m_PlayerList[tileBeginSit].GetWallTileCount() - 1;
                extra = m_PlayerList[tileBeginSit].HideWallTile(index, -extra, GetBrightCardIndex(tileBeginSit));
                if (extra != 0)
                    DebugLog.LogError("no tiles can add!!! " + m_nDealIndex + " left:" + m_nLeftTileNum);
            }
            else
                index -= num;

        }
        m_nDealIndex = tileBeginSit * 100 + index;

        //DebugLog.Log("HideWall===index:"+m_nDealIndex);
    }

    protected bool HandleAnswer(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        if (state != 0)//fail 错误码 11 房间状态不对    12 已经定过听跟非听  13 未符合听牌条件
        {
            foreach (Button btn in m_GameButton.Values)
            {
                btn.interactable = true;
            }

            return false;
        }

        byte dealSit = _ms.ReadByte();
        MjOtherDoing_Enum doing = (MjOtherDoing_Enum)_ms.ReadByte();
        DebugLog.Log("=============doing=============" + doing);
        if (doing != MjOtherDoing_Enum.MjOtherDoing_Init)
        {
            if(doing >= MjOtherDoing_Enum.MjOtherDoing_Ting)
            {
                m_PlayerList[0].OnTing(doing);
            }
            else
            {
                byte tile = _ms.ReadByte();
                PongKongType type = PongKongType.ePKT_None;
                byte exposed = _ms.ReadByte();//杠(1:明 2：暗 3:补) 吃(起头的牌)
                if (doing == MjOtherDoing_Enum.MjOtherDoing_Chi)
                {
                    type = m_PlayerList[0].ChiTile(tile,exposed, false, 1.0f, GetOtherTileDirection(m_PlayerList[0].m_nSit,dealSit,0));
                }
                else
                {
                    type = m_PlayerList[0].PongKongTile(tile, exposed, false, 1.0f, GetOtherTileDirection(m_PlayerList[0].m_nSit, dealSit, exposed));
                }
                byte cSit = m_dictSitPlayer[dealSit];
                if (cSit != 0)
                    m_PlayerList[cSit].OnOtherDoing(tile, type);

                HandleShowReward(0, type, _ms);
            }
        }

        OnChangeTurn(0);

        m_PlayerList[0].PlayWeiPaiShowExpression(_ms);

        return true;
    }

    //获得其他人碰和杠排版样式类型
    public MjTileDirection_Enum GetOtherTileDirection(byte selfSit,byte dealSit,byte dongType)
    {
        if (dongType == 1)//明杠
        {
            return MjTileDirection_Enum.TileDirection_Front;
        }
        else if (dongType == 2)//暗杠
        {
            return MjTileDirection_Enum.TileDirection_Count;
        }
        else
        {
            int RightSit = selfSit + 1;
            if (RightSit > 3)
                RightSit = 0;

            int LeftSit = selfSit - 1;
            if (LeftSit < 0)
                LeftSit = 3;

            if (dealSit == LeftSit)
            {
                return MjTileDirection_Enum.TileDirection_Left;
            }
            else if (dealSit == RightSit)
            {
                return MjTileDirection_Enum.TileDirection_Right;
            }
        }

        return MjTileDirection_Enum.TileDirection_Front;
    }

    protected bool HandleOtherAnswer(uint _msgType, UMessage _ms)
    {
        byte sit = _ms.ReadByte();
        byte cSit = m_dictSitPlayer[sit];
        OnChangeTurn(cSit);

        byte dealSit = _ms.ReadByte();
        MjOtherDoing_Enum doing = (MjOtherDoing_Enum)_ms.ReadByte();
        if (doing >= MjOtherDoing_Enum.MjOtherDoing_Ting)
        {
            m_PlayerList[cSit].OnTing(doing);
        }
        else
        {
            byte tile = _ms.ReadByte();
            PongKongType type = PongKongType.ePKT_None;
            byte exposed = _ms.ReadByte();
            if (doing == MjOtherDoing_Enum.MjOtherDoing_Chi)
            {
                type = m_PlayerList[cSit].ChiTile(tile, exposed, false, 1.0f, GetOtherTileDirection(sit, dealSit,0));
            }
            else
            {
                type = m_PlayerList[cSit].PongKongTile(tile, exposed, false, 1.0f, GetOtherTileDirection(sit, dealSit, exposed));
            }
            if (sit != dealSit)
                m_PlayerList[m_dictSitPlayer[dealSit]].OnOtherDoing(tile, type);

            byte selfHu = _ms.ReadByte();//1:可以抢杠胡
            HandleShowReward(cSit, type, _ms);
            uint sign = _ms.ReadUInt();

            if (selfHu == 1)
            {
                int selfDoing = 0;
                GameKind.AddFlag((int)MjOtherDoing_Enum.MjOtherDoing_Hu, ref selfDoing);
                ShowGameButton((byte)selfDoing, sign);
            }
        }

        m_PlayerList[cSit].PlayWeiPaiShowExpression(_ms);
        return true;
    }

    void HandleShowReward(byte cSit, PongKongType type, UMessage _ms)
    {
        uint coinPerPlayer = _ms.ReadUInt();
        byte num = _ms.ReadByte();
        m_PlayerList[cSit].ShowReward(coinPerPlayer * num);

        byte sit;
        for (int i = 0; i < num; i++)
        {
            sit = _ms.ReadByte();
            cSit = m_dictSitPlayer[sit];
            m_PlayerList[cSit].ShowReward(-coinPerPlayer);
        }

        if (coinPerPlayer > 0)
        {
            if (type == PongKongType.ePKT_Pong2Kong || type == PongKongType.ePKT_Kong_Exposed)
                PlayEffect(false, "Effect_wind", 1.5f);
            else if(type == PongKongType.ePKT_Kong_Concealed)
                PlayEffect(false, "Effect_rain", 1.5f);
        }
    }

    protected bool HandlePlayerWin(uint _msgType, UMessage _ms)
    {
        byte sit = _ms.ReadByte();
        byte cSit = m_dictSitPlayer[sit];

        byte dealSit = _ms.ReadByte();
        byte tile = _ms.ReadByte();
        uint result = _ms.ReadUInt();
        uint addCoin = _ms.ReadUInt();

        bool end = true;
        m_PlayerList[cSit].OnWin(tile, result, addCoin, end);
        if (dealSit != sit)
            m_PlayerList[m_dictSitPlayer[dealSit]].OnOtherDoing(tile,
                GameKind.HasFlag((int)MahjongHuType.MjHuOtherAdd_QiangGangHu, (int)result) ?
                PongKongType.ePKT_CancelKong : PongKongType.ePKT_Win);
        else
        {
            for(int i = 0; i < 4; i++)
            {
                if (i == cSit || m_PlayerList[i].IsWin())
                    continue;
                m_PlayerList[i].PlayRoleAnim(false);
            }
        }

        if (cSit == 0)
        {
            ShowGameButton(0);

            if (end)
                m_ShowTingButton.gameObject.SetActive(false);
        }

        OnChangeTurn(sit);
        return true;
    }

    public struct ChangeCoinData
    {
        public MjChangeCoinType_Enum type;
        public int coin;
    }
    protected bool HandleResult(uint _msgType, UMessage _ms)
    {
        ShowGameButton(0);

        int result = (int)_ms.ReadUInt();
        byte kongNum = _ms.ReadByte();
        byte winNum = _ms.ReadByte();
        byte i = 0;
        List<ChangeCoinData> changeList = new List<ChangeCoinData>();
        for (; i < winNum; i++)
        {
            ChangeCoinData ccd = new ChangeCoinData();
            ccd.type = (MjChangeCoinType_Enum)_ms.ReadByte();
            ccd.coin = _ms.ReadInt();
            byte changeSit = _ms.ReadByte();
            changeList.Add(ccd);
        }

        bool bLiuJu = true;
        byte roleNum = _ms.ReadByte();
        Dictionary<byte, RoundResultData> resultdata = new Dictionary<byte, RoundResultData>();
        RoundResultData data;
        i = 0;
        for (; i < roleNum; i++)
        {
            byte sit = _ms.ReadByte();
            uint playerid = _ms.ReadUInt();
            byte cSit = m_dictSitPlayer[sit];
            long coin = _ms.ReadLong();
            long add = _ms.ReadLong();
            if (add != 0)
                bLiuJu = false;
            m_PlayerList[cSit].UpdateInfoUI(coin);

            data = new RoundResultData();
            data.headImg = m_PlayerList[cSit].GetHeadImg();
            data.coin = coin;
            data.name = m_PlayerList[cSit].GetRoleName();
            data.addCoin = add;
            data.playerid = playerid;

            byte num = _ms.ReadByte();
            List<byte> list = new List<byte>();
            for (byte j = 0; j < num; j++)
            {
                list.Add(_ms.ReadByte());
            }
            m_PlayerList[cSit].ShowTiles(list);

            resultdata.Add(sit, data);
        }

        if(bLiuJu)
            PlayEffect(true, "Anime_liuju", 1.5f);

        byte curRound = _ms.ReadByte();        byte totalRound = _ms.ReadByte();
        long vedioId = _ms.ReadLong();

        if (appointmentTotalResults_ != null)
        {
            appointmentTotalResults_.Add(resultdata);
            Debug.Log("add results===============");
        }

        if (GameMode == GameTye_Enum.GameType_Contest)            GameMain.SC(ShowContestResult(resultdata, curRound, totalRound, vedioId, result, changeList, kongNum));
        else if(GameMode == GameTye_Enum.GameType_Appointment)
        {
            LoadAppointmentRuleResource((uint)(curRound + 1), true);
            GameMain.SC(ShowResult(resultdata, curRound, totalRound, result, changeList, kongNum));
        }
        else
            GameMain.SC(ShowResult(resultdata, curRound, totalRound, result, changeList, kongNum));

        return true;
    }

    IEnumerator ShowResult(Dictionary<byte, RoundResultData> resultDataList, byte curRound, byte totalRound, int result, List<ChangeCoinData> changeList, byte kongNum)
    {
        yield return new WaitForSecondsRealtime(2f);

        CustomAudioDataManager.GetInstance().StopAudio();
        Desk.StartCountdown(-1);

        ResultUITfm.gameObject.SetActive(true); 
        Button butn = ResultUITfm.Find("BottomButton/ButtonAgain").GetComponent<Button>();
        butn.onClick.RemoveAllListeners();

        RoundResultData SelfResultData;
        Transform tfm = ResultUITfm.Find("Result_Own/Text_jifen_win");
        if (resultDataList.TryGetValue(m_PlayerList[0].m_nSit, out SelfResultData))
        {
            if (SelfResultData.addCoin < 0)
            {
                tfm.gameObject.SetActive(false);
                tfm = ResultUITfm.Find("Result_Own/Text_jifen_lost");
                tfm.gameObject.SetActive(true);
            }
            else
            {
                ResultUITfm.Find("Result_Own/Text_jifen_lost").gameObject.SetActive(false);
                tfm.gameObject.SetActive(true);
            }
            tfm.GetComponent<Text>().text = GameFunction.FormatCoinText(SelfResultData.addCoin, true, false);
        }

        tfm = ResultUITfm.Find("Result_Own/Viewport_beishu/Content");
        foreach (Transform t in tfm)
        {
            GameObject.Destroy(t.gameObject);
        }

        UnityEngine.Object obj = MahjongAssetBundle.LoadAsset("Result_beishu");
        Transform child;
        string str = "";
        Dictionary<int, string> OutDifferenceTextData = new Dictionary<int, string>();
        Dictionary<int, string> dict = Mahjong_Data.GetInstance().m_TextData[GameKind_Enum.GameKind_Mahjong];

        foreach(ChangeCoinData ccd in changeList)
        {
            DebugLog.Log("result:" + result + " type:" + ccd.type + " coin:" + ccd.coin);

            child = (GameMain.instantiate(obj) as GameObject).transform;
            child.SetParent(tfm, false);

            int textID = 100 + (int)ccd.type;

            str = dict[textID];
            if (Mahjong_Data.GetInstance().m_TextData.TryGetValue(GameType, out OutDifferenceTextData))
            {
                if(!string.IsNullOrEmpty(OutDifferenceTextData[textID]))
                {
                    str = OutDifferenceTextData[textID];
                }
            }
           
            if (ccd.coin > 0 && (ccd.type == MjChangeCoinType_Enum.MjChangeCoinType_DianPao || ccd.type == MjChangeCoinType_Enum.MjChangeCoinType_Zimo))
            {
                foreach (MahjongHuType item in System.Enum.GetValues(typeof(MahjongHuType)))
                {
                    if (item >= MahjongHuType.MjHuType_ZiMo)
                        continue;

                    int j = (int)item;
                    if (GameKind.HasFlag(j, result))
                    {
                        DebugLog.Log("MahjongHuType:" + j);
                        str = dict[j] + "·" + str;
                    }
                }

                if (kongNum > 0)
                    str = str + "·" + kongNum + dict[(int)MahjongHuType.MahjongHu_End];
            }

            child.GetComponent<Text>().text = str;
            if(ccd.coin != 0)
                child.Find("TextNum").GetComponent<Text>().text = GameFunction.FormatCoinText(ccd.coin, true, false);
            else
                child.Find("TextNum").GetComponent<Text>().text = "×" + m_ProInfo.m_CoinTypeProDict[ccd.type];
        }

        Color WSColor = new Color(1f, 0.8627f, 0.3216f);        Color WEColor = new Color(0.9686f, 0.4667f, 0.1294f);        Color LSColor = new Color(0.7176f, 0.9569f, 0.96478f);        Color LEColor = new Color(0.1294f, 0.643f, 0.9686f);        Gradient grad;        RoundResultData ResultData;        for (int index = 1; index < 4; index++)
        {
            tfm = ResultUITfm.Find("OtherPlayerBG/Player_" + index);
            if (!resultDataList.TryGetValue(m_PlayerList[index].m_nSit, out ResultData))
            {
                tfm.gameObject.SetActive(false);
                continue;
            }
            tfm.gameObject.SetActive(true);
            tfm.Find("Name_Text").GetComponent<Text>().text = m_PlayerList[index].GetRoleName();
            tfm.Find("Head/HeadMask/ImageHead").GetComponent<Image>().sprite = m_PlayerList[index].GetHeadImg();

            tfm = tfm.Find("Text_zongfen");
            grad = tfm.GetComponent<Gradient>();            if (ResultData.addCoin > 0)            {                grad.StartColor = WSColor;                grad.EndColor = WEColor;            }            else            {                grad.StartColor = LSColor;                grad.EndColor = LEColor;            }            tfm.GetComponent<Text>().text = GameFunction.FormatCoinText(ResultData.addCoin, true, false);
        }

        bool allEnd = (curRound == totalRound);
        if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            bool isshowtotal = AppointmentDataManager.AppointmentDataInstance().interrupt || allEnd;
            XPointEvent.AutoAddListener(butn.gameObject, OnClosePerResultPanel, isshowtotal);

            butn.gameObject.SetActive(allEnd);
        }
        else
        {
            butn.gameObject.SetActive(true);
            butn.onClick.AddListener(() => OnClickResultButton(1, allEnd));
            butn.interactable = GameState == MahjongRoomState_Enum.MjRoomState_TotalEnd || GameState == MahjongRoomState_Enum.MjRoomState_OnceEnd ||
                                GameState < MahjongRoomState_Enum.MjRoomState_TotalBegin;

            Button okButton = ResultUITfm.Find("BottomButton/ButtonOk").GetComponent<Button>();
            okButton.interactable = butn.interactable;
        }    }

    IEnumerator ShowContestResult(Dictionary<byte, RoundResultData> resultDataDictionary, byte curRound, byte totalRound, long vedioId, int result, List<ChangeCoinData> changeList, byte kongNum)
    {
        bool allEnd = (curRound == totalRound);
        yield return new WaitForSecondsRealtime(2f);
        AppointmentRecord record = null;
        if (allEnd && !Bystander)
        {
            //组装比赛记录数据
            record = new AppointmentRecord();
            record.gameID = (byte)GameType;
            appointmentGameRule_ = GetMahJiongGameRule() + "比赛第" + MatchInGame.GetInstance().m_nCurTurn.ToString() + "轮";
            record.gamerule = appointmentGameRule_;
            record.timeseconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);
            record.isopen = false;
            record.videoes = vedioId;
        }

        int index = 0;
        RoundResultData roundResultData = null;
        List<RoundResultData> list = new List<RoundResultData>();
        foreach (Mahjong_Role role in m_PlayerList)
        {
            if(!resultDataDictionary.TryGetValue(role.m_nSit, out roundResultData))
            {
                continue;
            }

            list.Add(roundResultData);            if (record != null)
            {
                AppointmentRecordPlayer playerdata = new AppointmentRecordPlayer();
                playerdata.playerid = roundResultData.playerid;
                playerdata.faceid = role.m_faceid;
                playerdata.playerName = role.GetRoleName();
                playerdata.url = role.m_url;
                playerdata.coin = role.m_nTotalCoin;

                if (!record.result.ContainsKey(playerdata.playerid))
                    record.result.Add(playerdata.playerid, playerdata);
            }            index++;        }
        if (record != null)
        {
            if (GameMain.hall_.gamerooms_ == null)
                GameMain.hall_.InitRoomsData();

            GameMain.hall_.gamerooms_.recordlist_.Insert(0, record);
        }

        CustomAudioDataManager.GetInstance().StopAudio();
        Desk.StartCountdown(-1);

        string str = "", detail = "";
        Dictionary<int, string> OutDifferenceTextData = new Dictionary<int, string>();
        Dictionary<int, string> dict = Mahjong_Data.GetInstance().m_TextData[GameKind_Enum.GameKind_Mahjong];

        foreach (ChangeCoinData ccd in changeList)
        {
            DebugLog.Log("result:" + result + " type:" + ccd.type + " coin:" + ccd.coin);

            int textID = 100 + (int)ccd.type;

            str = dict[textID];
            if (Mahjong_Data.GetInstance().m_TextData.TryGetValue(GameType, out OutDifferenceTextData))
            {
                if (!string.IsNullOrEmpty(OutDifferenceTextData[textID]))
                {
                    str = OutDifferenceTextData[textID];
                }
            }

            if (ccd.coin > 0 && (ccd.type == MjChangeCoinType_Enum.MjChangeCoinType_DianPao || ccd.type == MjChangeCoinType_Enum.MjChangeCoinType_Zimo))
            {
                foreach (MahjongHuType item in System.Enum.GetValues(typeof(MahjongHuType)))
                {
                    if (item >= MahjongHuType.MjHuType_ZiMo)
                        continue;

                    int j = (int)item;
                    if (GameKind.HasFlag(j, result))
                        str = dict[j] + "·" + str;
                }

                if (kongNum > 0)
                    str = str + "·" + kongNum + dict[(int)MahjongHuType.MahjongHu_End];
            }

            if(ccd.coin != 0)
                str = str + " " + GameFunction.FormatCoinText(ccd.coin, true, false);
            else
                str = str + " ×" + m_ProInfo.m_CoinTypeProDict[ccd.type];

            if (detail.Length > 0)
                detail += "\n";
            detail += str; 
        }

        yield return MatchInGame.GetInstance().ShowRoundResult(4, list, () =>        {
            if (allEnd)
                GameOver();
            else
                OnEnd();
        }, allEnd, Bystander, detail);
    }

    void OnClickResultButton(int index, bool allEnd = false)
    {
        if (GameMode != GameTye_Enum.GameType_Normal)
            return;

        CustomAudioDataManager.GetInstance().PlayAudio(3006);

#if MATCH_ROOM
        if (index == 0)//离开
        {
            MatchRoom.GetInstance().OnClickReturn(index);
        }
        else if (index == 1)//继续
        {
            if (allEnd)
                MatchRoom.GetInstance().OnClickReturn(index);
            else
                OnEnd();
        }
        else if (index == 2)
        {
            Player.ShareImageToWechat(false);
        }
#else
        if (index == 0)//离开
        {
            GameOver(true);
        }
        else if (index == 1)//继续
        {
            if (!allEnd)
                OnEnd();
            else
            {
                GameOver(true);
                OnClickMatchButton(0, false);
            }
        }
#endif
    }

    void OnClickNum(bool show)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(3006);
        LeftTileTfm.gameObject.SetActive(show);
    }

    public virtual void ResetLeftTile()
    {
        m_LeftNumText.text = "";
        m_LeftNumText.transform.parent.gameObject.SetActive(false);

        m_dictTileValueNum.Clear();
        Transform tfm;
        foreach (byte value in JudgeTile.MahjongTiles)
        {
            if (value > m_nMaxTileValue)
                break;

            m_dictTileValueNum[value] = (byte)(value < 0x40 ? 4 : 1);
            tfm = LeftTileTfm.Find("Mask/" + JudgeTile.GetTileSuit(value));
            tfm.gameObject.SetActive(true);
            tfm = tfm.Find("majiang_" + JudgeTile.GetTileValue(value));
            tfm.Find("ImageIcon").GetComponent<Image>().color = Color.white;
            //tfm.FindChild("Text").GetComponent<Text>().text = m_dictTileValueNum[value].ToString();
        }
    }

    public void OnLeftTileChanged(byte value, byte num, bool reduce = true)
    {
        if (!m_dictTileValueNum.ContainsKey(value))
            return;

        if(reduce)
            m_dictTileValueNum[value] -= num;
        else
            m_dictTileValueNum[value] += num;

        Transform tfm = LeftTileTfm.Find("Mask/" + JudgeTile.GetTileSuit(value) + "/majiang_" + JudgeTile.GetTileValue(value));
        tfm.Find("ImageIcon").GetComponent<Image>().color = m_dictTileValueNum[value] == 0 ? Color.gray : Color.white;
        //tfm.FindChild("Text").GetComponent<Text>().text = m_dictTileValueNum[value].ToString();
    }

    public void ShowTingBtn(bool show)
    {
        m_ShowTingButton.gameObject.SetActive(show);
    }

    public virtual string GetBeiShuStr()
    {
        return "";
    }

    //获得查听胡牌类型显示界面路径
    protected virtual string GetTingUIPath()
    {
        return "Mask/ImageBG";
    }

    /// <summary>
    /// 获取明牌的索引
    /// </summary>
    /// <param name="clientSit">客户端座位号</param>
    /// <returns></returns>
    public int GetBrightCardIndex(int clientSit)
    {
        return clientSit == m_nBrightCardSit ? m_nBrightCardIndex : -1;
    }


    /// <summary>
    /// 初始化查听的牌对象数据
    /// </summary>
    /// <param name="tileObject">查听的牌对象</param>
    /// <param name="tingTile">查听的牌对象数据</param>
    /// <returns>剩余牌的数量</returns>
    ushort InitTingTileGameObejectData(GameObject tileObject, KeyValuePair<byte, ushort> tingTile)
    {
        tileObject.name = "ting_" + tingTile;
        byte tileNum = m_dictTileValueNum[tingTile.Key];
        Image tileImg = tileObject.transform.Find("ImageIcon").GetComponent<Image>();
        tileImg.sprite = MahjongAssetBundle.LoadAsset<Sprite>("mahjong_" + tingTile.Key.ToString("X2"));
        tileImg.color = (tileNum == 0 ? Color.gray : (m_AtomicCard == tingTile.Key ? Color.yellow : Color.white));
        tileImg.transform.Find("TextNUM").GetComponent<Text>().text = tileNum.ToString();
        tileObject.transform.Find("Textbeishu").GetComponent<Text>().text = tingTile.Value.ToString() + GetBeiShuStr();
        return tileNum;
    }

    public virtual void ShowTingUI(bool bShow, Dictionary<byte, ushort> tileHu = null)
    {
        TingTfm.gameObject.SetActive(bShow);
        if (tileHu != null)
        {
            Transform parent = TingTfm.Find(GetTingUIPath());
            foreach (Transform tfm in parent)
            {
                if (tfm.name.Contains("ting"))
                {
                    GameObject.Destroy(tfm.gameObject);
                }
            }

            if (tileHu.Count > 0)
            {
                ushort atomicCardNum = 0, totalNum = 0;
                Object obj = MahjongAssetBundle.LoadAsset("ting_majiang");
                if (tileHu.TryGetValue(m_AtomicCard,out atomicCardNum))
                {
                    GameObject go = GameMain.Instantiate(obj) as GameObject;
                    totalNum += InitTingTileGameObejectData(go, new KeyValuePair<byte, ushort>(m_AtomicCard, atomicCardNum));
                    go.transform.SetParent(parent, false);
                }

                foreach (var ting in tileHu)
                {
                    if(ting.Key == m_AtomicCard)
                    {
                        continue;
                    }
                    GameObject go = GameMain.Instantiate(obj) as GameObject;
                    totalNum += InitTingTileGameObejectData(go, ting);
                    go.transform.SetParent(parent, false);
                }
                TingTfm.GetComponent<Button>().interactable = false;
                TingTfm.Find("Mask/ImageBG/Imagenum/TextNum").GetComponent<Text>().text = totalNum.ToString();
            }
        }
        else if (bShow)//update data
        {
            Transform parent = TingTfm.Find("Mask/ImageBG");
            foreach (Transform tfm in parent)
            {
                if (tfm.name.Contains("ting"))
                {
                    byte ting;
                    if (byte.TryParse(tfm.name, out ting))
                    {
                        byte num = m_dictTileValueNum[ting];
                        Image tileImg = tfm.Find("ImageIcon").GetComponent<Image>();
                        tileImg.color = (num == 0 ? Color.gray :(m_AtomicCard == ting ? Color.yellow : Color.white));
                        tileImg.transform.Find("TextNUM").GetComponent<Text>().text = num.ToString();
                    }
                }
            }
            TingTfm.GetComponent<Button>().interactable = true;
        }
    }

    private void ShowTileCole(byte value,Color color)
    {
        string name = value.ToString("X2");
        Transform parent = m_DiscardTfm.Find(name);
        if (parent != null)
        {
            foreach (Transform tfm in parent)
            {
                if (tfm.name == "pong")
                {
                    foreach (Transform child in tfm)
                    {
                        ShowTileColor(child, color);
                    }
                }
                else
                {
                    ShowTileColor(tfm, color);
                }
            }
        }

        parent = m_DiscardTfm.Find("chi");
        if (parent != null)
        {
            Transform childChiTransform = null;
            foreach (Transform childTransform in parent)
            {
                childChiTransform = childTransform.Find(name);
                if (childChiTransform)
                {
                    ShowTileColor(childChiTransform, color);
                }
            }
        }
    }   

    public void ShowDiscardedTile(byte value)
    {
        if (m_nShowDiscardedValue == value)
            return;

        //上一个选中的麻将颜色复原
        if (m_nShowDiscardedValue != RoomInfo.NoSit)
        {
            ShowTileCole(m_nShowDiscardedValue, Color.white);
        }

        m_nShowDiscardedValue = value;
        //当前选中的麻将颜色修改
        if (m_nShowDiscardedValue != RoomInfo.NoSit)
        {
            Color color = new Color(0.5f, 0.5f, 1f);
            ShowTileCole(m_nShowDiscardedValue, color);
        }
    }

    public void ShowTileColor(Transform tfm, Color color)
    {
        foreach (Material mat in tfm.GetComponent<MeshRenderer>().materials)
        {
            mat.color = color;
        }
    }

    public byte GetTileRemainNum(Dictionary<byte, ushort> tileHu, out ushort maxPro)
    {
        byte total = 0;
        maxPro = 0;
        foreach (var tile in tileHu)
        {
            total += m_dictTileValueNum[tile.Key];
            if (tile.Value > maxPro)
                maxPro = tile.Value;
        }
        return total;
    }
    
    //获得单个麻将剩余的个数
    public byte GetTileRemainNum(byte tile)
    {
        byte tileCount = 0;
        m_dictTileValueNum.TryGetValue(tile,out tileCount);
        return tileCount;
    }

    protected void OnClickTrustBtn(GameCity.EMSG_ENUM msgType)
    {
        UMessage msg = new UMessage((uint)msgType);
        msg.Add(GameMain.hall_.GetPlayerId());
        HallMain.SendMsgToRoomSer(msg);
        SetTrustButtonActive(false);
    }

    public void SetTrustButtonActive(bool activeState)
    {
        if(m_TrustButton.gameObject.activeSelf != activeState)
        {
            m_TrustButton.gameObject.SetActive(activeState);
        }
    }

    public override void ReconnectSuccess()
    {
        m_bReconnected = true;
    }

    protected virtual void OnLeaveLookRoom(GameCity.EMSG_ENUM GameCityMsg)
    {
        UMessage msg = new UMessage((uint)GameCityMsg);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((uint)0);
        HallMain.SendMsgToRoomSer(msg);
    }

    public override void StartLoad()
    {
        m_nWaitingLoad = 0;
    }

    protected virtual void OnStartDie(byte dealerSit, byte point1, byte point2, bool midEnt = false)
    {
        if (point1 == 0 || point2 == 0)
            return;
        int beginSit = (m_dictSitPlayer[dealerSit] + point1 + point2 + 3) % 4;
        int index = Mathf.Min(point1, point2) * 2;
        m_nDealIndex = beginSit * 100 + index;
        Desk.OnPlayDie(new byte[] { point1, point2 }, midEnt);
        ThrowDice(dealerSit, midEnt);
    }

    //掷骰子
    protected void ThrowDice(byte dealerSit, bool midEnt = false)
    {
        m_PlayerList[m_dictSitPlayer[dealerSit]].ShowZhuang();
        OnChangeTurn(m_dictSitPlayer[dealerSit]);
        CreateWallTiles();

        if (!midEnt)
        {
            Desk.BeginGame();
            Desk.ShowTurn(dealerSit);
        }
        else
        {
            OnTileNumChanged(0);
            ShowWallTiles();
        }
    }

    //断线重连同步数据
    protected virtual MjTileMiddleEnterData_Enum MiddleEnterRoomRoleLocal(UMessage _ms, ref List<byte> listPong, ref List<byte> listKong, ref List<byte> listKongSelf,
                                                                          ref List<byte> listDiscard, ref List<byte> listHua,ref List<byte> listChi, ref List<byte> listKong2)
    {
        byte num = _ms.ReadByte();
        listPong.Clear();
        for (byte j = 0; j < num; j++)
            listPong.Add(_ms.ReadByte());
        num = _ms.ReadByte();
        listKong.Clear();
        for (byte j = 0; j < num; j++)
            listKong.Add(_ms.ReadByte());
        num = _ms.ReadByte();
        listKongSelf.Clear();
        for (byte j = 0; j < num; j++)
            listKongSelf.Add(_ms.ReadByte());
        num = _ms.ReadByte();
        listDiscard.Clear();
        for (byte j = 0; j < num; j++)
            listDiscard.Add(_ms.ReadByte());
        num = _ms.ReadByte();
        listHua.Clear();
        for (byte j = 0; j < num; j++)
            listHua.Add(_ms.ReadByte());
        return MjTileMiddleEnterData_Enum.MiddleEnterData_Normal;
    }

    /// <summary>
    /// 更新约据房间信息
    /// </summary>
    /// <param name="Data"></param>
    protected virtual void UpdateAppointmentData(AppointmentData Data, UMessage _ms)
    {

    }

    protected virtual bool HandleMiddleEnterRoom(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        if (state == 0)//失败
        {
            return false;
        }

        MjPlayerState_Enum playerState = (MjPlayerState_Enum)_ms.ReadByte();

        DebugLog.Log("Middle enter room playerState:" + playerState);

        if (playerState == MjPlayerState_Enum.MjPlayerState_OnGameButIsOver)
        {
            CCustomDialog.OpenCustomConfirmUI(1621, (param) => BackToChooseLevel(false));            return false;        }#if MATCH_ROOM
        if (playerState == MjPlayerState_Enum.MjPlayerState_ReadyHall)
        {
            MatchRoom.GetInstance().OnEnd();
            return false;
        }
#endif
        uint roomId = _ms.ReadUInt();
        byte level = _ms.ReadByte();
        EnterRoom(level, true);
        OnStartEndMatch(false, false);
        GameMain.hall_.CurRoomIndex = level;
        OnEnd();

        InitChangeCoinTypeData(_ms,true);

        Dictionary<byte, AppointmentRecordPlayer> players = new Dictionary<byte, AppointmentRecordPlayer>();
        AppointmentRecordPlayer player;

        long coin = _ms.ReadLong();
        m_PlayerList[0].UpdateInfoUI(coin);

        byte localSit = _ms.ReadByte();
        m_dictSitPlayer[localSit] = 0;
        m_PlayerList[0].m_nSit = localSit;
        Desk.OnRotDir(localSit);
        byte ready = _ms.ReadByte();

        PlayerData pd = GameMain.hall_.GetPlayerData();
        player = new AppointmentRecordPlayer();
        player.playerid = pd.GetPlayerID();
        player.faceid = (int)pd.PlayerIconId;
        player.url = pd.GetPlayerIconURL();
        player.coin = coin;
        player.playerName = pd.GetPlayerName();
        player.master = pd.MasterScoreKindArray[(int)GameType];
        player.sex = pd.PlayerSexSign;
        player.ready = ready;
        players[localSit] = player;

        sbyte lastSit = _ms.ReadSByte();
        sbyte dealSit = _ms.ReadSByte();
        byte isLastPong = _ms.ReadByte();

        float timeLeft = _ms.ReadSingle();
        byte beginSit = _ms.ReadByte();
        byte point1 = _ms.ReadByte();
        byte point2 = _ms.ReadByte();

        byte otherNum = _ms.ReadByte();
        byte sit, clientSit, sex, dis, num, hu;
        sbyte lackType;
        uint userId;
        int face;
        string url, name;
        float master;
        List<byte> list = new List<byte>();
        List<byte> listPong = new List<byte>();
        List<byte> listKong = new List<byte>();
        List<byte> listKongSelf = new List<byte>();
        List<byte> listDiscard = new List<byte>();
        List<byte> listHua = new List<byte>();
        List<byte> listChi = new List<byte>();
        List<byte> listKong2 = new List<byte>();
        MjPlayerState_Enum ps;
        Mahjong_Role role;
        MjTileMiddleEnterData_Enum middleEnterDataEnum = MjTileMiddleEnterData_Enum.MiddleEnterData_Normal;
        for (byte i = 0; i < otherNum; i++)
        {
            sit = _ms.ReadByte();
            clientSit = GetClientSit(sit, localSit);
            m_dictSitPlayer[sit] = clientSit;
            role = m_PlayerList[clientSit];
            role.m_nSit = sit;
            userId = _ms.ReadUInt();
            face = _ms.ReadInt();
            url = _ms.ReadString();
            coin = _ms.ReadLong();
            name = _ms.ReadString();
            m_PlayerList[clientSit].OnEnd();
            hu = _ms.ReadByte();//0:不胡 1：点炮胡 2：自摸
            num = _ms.ReadByte();
            list = new List<byte> (new byte[num]);
            if(hu > 0 && num > 0)
                list[num - 1] = _ms.ReadByte();
            dis = _ms.ReadByte();
            master = _ms.ReadSingle();
            sex = _ms.ReadByte();
            ready = _ms.ReadByte();
            lackType = _ms.ReadSByte();
            ps = (MjPlayerState_Enum)_ms.ReadByte();
            middleEnterDataEnum = MiddleEnterRoomRoleLocal(_ms,ref listPong, ref listKong, ref listKongSelf, ref listDiscard, ref listHua,ref listChi,ref listKong2);
            role.OnTurn = (sit == dealSit);
            role.OnDisOrReconnect(dis == 0);
            role.SetupInfoUI(userId, coin, name, url, face, master, sex);
            role.OnMidEnt(sit == lastSit && isLastPong == 0, hu, list, lackType, listPong, listKong, listKongSelf, listDiscard, listHua, listChi, listKong2,ps,middleEnterDataEnum);
   
            player = new AppointmentRecordPlayer();
            player.playerid = userId;
            player.faceid = face;
            player.url = url;
            player.coin = coin;
            player.playerName = name;
            player.master = master;
            player.sex = sex;
            player.ready = ready;
            players[sit] = player;
        }

        byte curRound = _ms.ReadByte();
        byte maxRound = _ms.ReadByte();

        uint contestId = _ms.ReadUInt();
        uint appointId = _ms.ReadUInt();

        if(appointId > 0)
        {
            AppointmentData data = GameFunction.CreateAppointmentData(GetGameType());
            data.roomid = appointId;
            data.playtimes = maxRound;
            data.maxpower = (byte)m_ProInfo.m_iMaxPro;
            UpdateAppointmentData(data, _ms);
            AppointmentDataManager.AppointmentDataInstance().currentRoomID = appointId;
            AppointmentDataManager.AppointmentDataInstance().AddAppointmentData(appointId, data);

            GameMain.hall_.InitRoomsData();

            LoadAppointmentRuleResource(curRound, true);
        }

        lackType = _ms.ReadSByte();
        ps = (MjPlayerState_Enum)_ms.ReadByte();
        hu = _ms.ReadByte();
        list.Clear();
        num = _ms.ReadByte();
        for (byte j = 0; j < num; j++)
            list.Add(_ms.ReadByte());
        middleEnterDataEnum = MiddleEnterRoomRoleLocal(_ms, ref listPong, ref listKong, ref listKongSelf, ref listDiscard, ref listHua,ref listChi,ref listKong2);
        m_PlayerList[0].OnTurn = (localSit == dealSit);
        m_PlayerList[0].OnMidEnt(localSit == lastSit && isLastPong == 0, hu, list, lackType,listPong, listKong, listKongSelf, listDiscard, listHua, listChi, listKong2, ps, middleEnterDataEnum);
        OnStartDie(beginSit, point1, point2, true);
        SyncWallTiles();

        if (dealSit >= 0 && beginSit != dealSit)
            OnChangeTurn(m_dictSitPlayer[(byte)dealSit]);

        MahjongRoomState_Enum gameState = (MahjongRoomState_Enum)_ms.ReadByte();
        OnStateChange(gameState, _ms, 1, timeLeft);

#if MATCH_ROOM
        if (GameMode == GameTye_Enum.GameType_Normal)
        {
            if (playerState == MjPlayerState_Enum.MjPlayerState_OnDesk)
                MatchRoom.GetInstance().ShowTable(true, players, level, roomId);
            else
                MatchRoom.GetInstance().StartGame();
        }
#endif

        CustomAudioDataManager.GetInstance().PlayAudio(3008, false);

        return true;
    }

    protected void SyncWallTiles()
    {
        if (m_nDealIndex < 0)
            return;

        int CircleCount = 0;//循环隐藏牌堆第几圈（防止进入死循环）
        int beginSit = m_nDealIndex / 100;
        int index = m_nDealIndex % 100;
        int extra;
        int num = m_nTotalTilesNum - m_nLeftTileNum;
        int oldSit = beginSit;
        while (num > 0)
        {
            extra = m_PlayerList[beginSit].HideWallTile(index, num, GetBrightCardIndex(beginSit));
            if (extra != 0)
            {
                extra = m_PlayerList[beginSit].HideWallTile((index + num - extra) + 1, extra, GetBrightCardIndex(beginSit));
                if(extra != 0)
                {
                    beginSit = (beginSit + 3) % 4;
                    if (beginSit == oldSit)
                    {
                        if(CircleCount == 3)
                        {
                            Debug.LogError("Hide wall tile failed!!");
                            break;
                        }
                        CircleCount++;
                    }
                    index = 0;
                }else
                {
                   index += num + 1;
                }
            }
            else
                index += num;
            num = extra;
        }

        m_nDealIndex = beginSit * 100 + index;
    }

    public override void OnPlayerDisOrReconnect(bool disconnect, uint userId, byte sit)
    {
        if (m_dictSitPlayer.ContainsKey(sit))
            m_PlayerList[m_dictSitPlayer[sit]].OnDisOrReconnect(disconnect);
#if MATCH_ROOM
        else if (GameMode == GameTye_Enum.GameType_Normal)
            MatchRoom.GetInstance().SetPlayerOffline(sit, disconnect);
#endif
    }

    protected virtual bool HandleBystanderEnter(uint _msgType, UMessage _ms)
    {
        uint roomId = _ms.ReadUInt();
        byte level = _ms.ReadByte();

        InitChangeCoinTypeData(_ms, true);


        uint contestId = _ms.ReadUInt();

        EnterRoom(level, true, true);
        OnStartEndMatch(false, false);

#if MATCH_ROOM
        if (GameMode == GameTye_Enum.GameType_Normal)
            MatchRoom.GetInstance().StartGame();
#endif

        sbyte lastSit = _ms.ReadSByte();
        sbyte dealSit = _ms.ReadSByte();
        byte isLastPong = _ms.ReadByte();

        float timeLeft = _ms.ReadSingle();
        byte beginSit = _ms.ReadByte();
        byte point1 = _ms.ReadByte();
        byte point2 = _ms.ReadByte();

        byte localSit = 0;
        Desk.OnRotDir(localSit);

        byte otherNum = _ms.ReadByte();
        long coin;
        byte sit, clientSit, sex, dis, num, hu, ready;
        sbyte lackType;
        uint userId;
        int face;
        string url, name;
        float master;
        List<byte> list = new List<byte>();
        List<byte> listPong = new List<byte>();
        List<byte> listKong = new List<byte>();
        List<byte> listKongSelf = new List<byte>();
        List<byte> listDiscard = new List<byte>();
        List<byte> listHua = new List<byte>();
        List<byte> listChi = new List<byte>();
        List<byte> listKong2 = new List<byte>();
        MjPlayerState_Enum ps;

        for (byte i = 0; i < otherNum; i++)
        {
            sit = _ms.ReadByte();
            clientSit = GetClientSit(sit, localSit);
            m_dictSitPlayer[sit] = clientSit;
            m_PlayerList[clientSit].m_nSit = sit;
            userId = _ms.ReadUInt();
            face = _ms.ReadInt();
            url = _ms.ReadString();
            coin = _ms.ReadLong();
            name = _ms.ReadString();
            m_PlayerList[clientSit].OnEnd();
            hu = _ms.ReadByte();
            num = _ms.ReadByte();
            list = new List<byte>(new byte[num]);
            if (hu > 0 && num > 0)
                list[num - 1] = _ms.ReadByte();
            dis = _ms.ReadByte();
            master = _ms.ReadSingle();
            sex = _ms.ReadByte();
            ready = _ms.ReadByte();
            lackType = _ms.ReadSByte();
            ps = (MjPlayerState_Enum)_ms.ReadByte();
            MjTileMiddleEnterData_Enum middleEnterDataEnum =  MiddleEnterRoomRoleLocal(_ms, ref listPong, ref listKong, ref listKongSelf, ref listDiscard, ref listHua, ref listChi, ref listKong2);
            m_PlayerList[clientSit].OnTurn = (sit == dealSit);
            m_PlayerList[clientSit].OnDisOrReconnect(dis == 0);
            m_PlayerList[clientSit].SetupInfoUI(userId, coin, name, url, face, master, sex);
            m_PlayerList[clientSit].OnMidEnt(sit == lastSit && isLastPong == 0, hu, list, lackType,listPong,listKong, listKongSelf, listDiscard, listHua, listChi, listKong2, ps, middleEnterDataEnum);
        }

        byte curRound = _ms.ReadByte();
        byte maxRound = _ms.ReadByte();

        if (GameMode == GameTye_Enum.GameType_Contest)
            MatchInGame.GetInstance().ShowBegin(true, curRound, maxRound);

        OnStartDie(beginSit, point1, point2, true);
        SyncWallTiles();

        if (dealSit >= 0 && beginSit != dealSit)
            OnChangeTurn(m_dictSitPlayer[(byte)dealSit]);

        MahjongRoomState_Enum gameState = (MahjongRoomState_Enum)_ms.ReadByte();
        OnStateChange(gameState, _ms, 2, timeLeft);

        CustomAudioDataManager.GetInstance().PlayAudio(3008, false);

        return true;
    }

    public override void SetupVideo(List<AppointmentRecordPlayer> list)
    {
        if (list == null)
            return;

        uint localId = GameMain.hall_.GetPlayerId();
        byte localSit = (byte)list.FindIndex(s => s.playerid == localId);
        Desk.OnRotDir(localSit);
        byte sit;
        for (byte i = 0; i < 4; i++)
        {            sit = (byte)((localSit + i) % 4);            m_PlayerList[i].m_nSit = sit;
            m_dictSitPlayer[sit] = i;
        }

        AppointmentRecordPlayer info;
        for (byte i = 0; i < list.Count; i++)        {            info = list[i];            sit = m_dictSitPlayer[i];            m_PlayerList[sit].SetupInfoUI(info.playerid, 0, info.playerName, info.url, info.faceid, info.master, info.sex);
        }
    }

    public override bool OnVideoStep(List<VideoAction> actionList, int curStep, bool reverse)
    {
        if (curStep > actionList.Count || curStep < 0)
            return true;

        VideoAction action = actionList[curStep];

        DebugLog.Log("Mahjong OnVideoStep:" + action.vai + " rev:" + reverse);
        List<int> list = action.list;
        int j = 0;
        bool res = true;

        if (!reverse)
        {
            switch (action.vai)
            {
                case VideoActionInfo_Enum.VideoActionInfo_201:
                    {
                        OnEnd();

                        byte beginSit = (byte)list[j++];
                        byte point1 = (byte)list[j++];
                        byte point2 = (byte)list[j++];
                        OnStartDie(beginSit, point1, point2, true);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_202:
                    {
                        byte dealSit = (byte)list[j++];
                        int num = list[j++];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte clientSit = m_dictSitPlayer[sit];
                            byte tileNum = (byte)list[j++];
                            List<byte> tileList = new List<byte>();
                            for (int k = 0; k < tileNum; k++)
                            {
                                byte tile = (byte)list[j++];
                                tileList.Add(tile);
                            }
                            m_PlayerList[clientSit].OnTurn = (sit == dealSit);
                            m_PlayerList[clientSit].OnMidEnt(false, 0, tileList, -1);
                        }

                        SyncWallTiles();
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_207:
                    {
                        byte sit = (byte)list[j++];
                        byte tile = (byte)list[j++];
                        byte doing = (byte)list[j++];
                        HandleAskDeal(sit, tile, doing, 0, 15f, null, GameVideo.GetInstance().m_bPause);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_208:
                    {
                        byte sit = (byte)list[j++];
                        byte tile = (byte)list[j++];
                        Mahjong_Role role = m_PlayerList[m_dictSitPlayer[sit]];
                        byte index = (byte)role.m_HaveTiles.FindIndex(s => s == tile);
                        role.OnDiscardTile(tile, index, GameVideo.GetInstance().GetStepTime());
                        Desk.StartCountdown(0);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_209:
                    {
                        byte sit = (byte)list[j++];
                        byte cSit = m_dictSitPlayer[sit];
                        OnChangeTurn(cSit);

                        float time = GameVideo.GetInstance().GetStepTime();
                        byte dealSit = (byte)list[j++];
                        MjOtherDoing_Enum doing = (MjOtherDoing_Enum)list[j++];
                        if (doing >= MjOtherDoing_Enum.MjOtherDoing_Ting)
                        {
                            m_PlayerList[cSit].OnTing(doing);
                            m_PlayerList[cSit].OnTingEnd();
                        }
                        else
                        {
                            byte tile = (byte)list[j++];
                            PongKongType type = PongKongType.ePKT_None;
                            byte exposed = (byte)list[j++];
                            if(doing == MjOtherDoing_Enum.MjOtherDoing_Chi)
                            {
                                type = m_PlayerList[cSit].ChiTile(tile,exposed,false, time, GetOtherTileDirection(sit, dealSit, 0));
                            }
                            else
                            {
                                type = m_PlayerList[cSit].PongKongTile(tile, exposed, false, time, GetOtherTileDirection(sit, dealSit, exposed));
                            }
                            if (sit != dealSit)
                                m_PlayerList[m_dictSitPlayer[dealSit]].OnOtherDoing(tile, type, time);

                            byte selfHu = (byte)list[j++];//1:可以抢杠胡

                            int coinPerPlayer = list[j++];
                            byte num = (byte)list[j++];
                            m_PlayerList[cSit].ShowReward(coinPerPlayer * num, time);
                            for (int i = 0; i < num; i++)
                            {
                                sit = (byte)list[j++];
                                cSit = m_dictSitPlayer[sit];
                                m_PlayerList[cSit].ShowReward(-coinPerPlayer, time);
                            }

                            if (coinPerPlayer > 0)
                            {
                                if (type == PongKongType.ePKT_Pong2Kong || type == PongKongType.ePKT_Kong_Exposed)
                                    PlayEffect(false, "Effect_wind", time);
                                else if (type == PongKongType.ePKT_Kong_Concealed)
                                    PlayEffect(false, "Effect_rain", time);
                            }

                            //吃两口
                            m_PlayerList[cSit].PlayWeiPaiShowExpression(null, list,j);
                        }
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_210:
                    {
                        float time = GameVideo.GetInstance().GetStepTime();
                        byte sit = (byte)list[j++];
                        byte cSit = m_dictSitPlayer[sit];
                        int coinPerPlayer = list[j++];
                        byte num = (byte)list[j++];
                        m_PlayerList[cSit].ShowReward(coinPerPlayer * num, time);
                        for (int i = 0; i < num; i++)
                        {
                            sit = (byte)list[j++];
                            cSit = m_dictSitPlayer[sit];
                            m_PlayerList[cSit].ShowReward(-coinPerPlayer, time);
                        }
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_211:
                    {
                        byte sit = (byte)list[j++];
                        byte cSit = m_dictSitPlayer[sit];

                        byte dealSit = (byte)list[j++];
                        byte tile = (byte)list[j++];
                        uint result = (uint)list[j++];
                        uint addCoin = (uint)list[j++];

                        float time = GameVideo.GetInstance().GetStepTime();
                        bool end = true;
                        m_PlayerList[cSit].OnWin(tile, result, addCoin, end, time);
                        if (dealSit != sit)
                            m_PlayerList[m_dictSitPlayer[dealSit]].OnOtherDoing(tile,
                                GameKind.HasFlag((int)MahjongHuType.MjHuOtherAdd_QiangGangHu, (int)result) ?
                                PongKongType.ePKT_CancelKong : PongKongType.ePKT_Win, time);
                        else
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (i == cSit || m_PlayerList[i].IsWin())
                                    continue;
                                m_PlayerList[i].PlayRoleAnim(false);
                            }
                        }
                    }
                    break;

                default:
                    res = false;
                    break;
            }        }        else
        {
            switch (action.vai)
            {
                case VideoActionInfo_Enum.VideoActionInfo_201:
                    {
                        OnEnd();
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_202:
                    {
                        OnVideoStep(actionList, curStep - 1, false);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_207:
                    {
                        byte sit = (byte)list[j++];
                        byte tile = (byte)list[j++];
                        byte doing = (byte)list[j++];
                        byte lastSit = (byte)list[j++];

                        HandleAskDeal(sit, tile, 0, 0, -15f);

                        if (lastSit >= 0)
                            OnChangeTurn(m_dictSitPlayer[lastSit]);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_208:
                    {
                        byte sit = (byte)list[j++];
                        byte tile = (byte)list[j++];
                        byte lastSit = (byte)list[j++];
                        byte lastTile = (byte)list[j++];

                        m_PlayerList[m_dictSitPlayer[sit]].OnDiscardTile(tile, 0, -1f);
                        if (lastSit < 4)
                            SetCurDiscardTile(FindLastDiscardTile(lastSit, lastTile));
                        Desk.StartCountdown(15, GameVideo.GetInstance().m_bPause);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_209:
                    {
                        byte sit = (byte)list[j++];
                        byte cSit = m_dictSitPlayer[sit];

                        byte dealSit = (byte)list[j++];
                        byte cDealSit = m_dictSitPlayer[dealSit];
                        OnChangeTurn(cDealSit);

                        float time = -GameVideo.GetInstance().GetStepTime();
                        MjOtherDoing_Enum doing = (MjOtherDoing_Enum)list[j++];
                        if (doing >= MjOtherDoing_Enum.MjOtherDoing_Ting)
                        {
                            m_PlayerList[cSit].OnTingEnd(true);
                        }
                        else
                        {
                            byte tile = (byte)list[j++];
                            PongKongType type = PongKongType.ePKT_None;
                            byte exposed = (byte)list[j++];
                            if (doing == MjOtherDoing_Enum.MjOtherDoing_Chi)
                            {
                                type = m_PlayerList[cSit].ChiTile(tile, exposed, false, time, GetOtherTileDirection(sit, dealSit, 0));
                            }
                            else
                            {
                                type = m_PlayerList[cSit].PongKongTile(tile, exposed, false, time, GetOtherTileDirection(sit, dealSit, exposed));
                            }
                            if (sit != dealSit)
                                m_PlayerList[cDealSit].OnOtherDoing(tile, type, time);
                        }

                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_211:
                    {
                        byte sit = (byte)list[j++];
                        byte cSit = m_dictSitPlayer[sit];

                        byte dealSit = (byte)list[j++];
                        byte tile = (byte)list[j++];
                        uint result = (uint)list[j++];
                        uint addCoin = (uint)list[j++];

                        bool end = true;
                        float time = -GameVideo.GetInstance().GetStepTime();
                        m_PlayerList[cSit].OnWin(tile, result, addCoin, end, time);
                        if (dealSit != sit)
                            m_PlayerList[m_dictSitPlayer[dealSit]].OnOtherDoing(tile,
                                GameKind.HasFlag((int)MahjongHuType.MjHuOtherAdd_QiangGangHu, (int)result) ?
                                PongKongType.ePKT_CancelKong : PongKongType.ePKT_Win, time);
                    }
                    break;

                default:
                    res = false;
                    break;
            }        }        return res;    }

    public override void OnVideoReplay()
    {
        base.OnVideoReplay();

        OnEnd();
    }

    public void PlayEffect(bool ui, string animation, float lifeTime, int audioID = 0, Transform parentTfm = null, AssetBundle bundle = null)    {        if (bundle == null)            bundle = MahjongAssetBundle;        UnityEngine.Object obj = (GameObject)bundle.LoadAsset(animation);        GameObject gameObj = (GameObject)GameMain.instantiate(obj);        gameObj.transform.SetParent(parentTfm == null ? (ui ? AnimationTfm : EffectTfm) : parentTfm, false);        if(ui)        {
            DragonBones.UnityArmatureComponent animate = gameObj.GetComponentInChildren<DragonBones.UnityArmatureComponent>();
            animate.animation.Play("newAnimation");
        }
        if (lifeTime > 0f)            GameObject.Destroy(gameObj, lifeTime);        if (audioID != 0)            CustomAudioDataManager.GetInstance().PlayAudio(audioID);    }

    void ClearUIAnimation()
    {
        foreach (Transform tfm in AnimationTfm)            GameObject.Destroy(tfm.gameObject);        foreach (Transform tfm in EffectTfm)            GameObject.Destroy(tfm.gameObject);    }

    void OnStartEndMatch(bool bStart, bool showBtn = true)
    {
#if !MATCH_ROOM
        bool[] bShow = new bool[m_MatchBtns.Length];
        bShow[0] = showBtn && !bStart;
        bShow[1] = showBtn && bStart;
        for (int i = 0; i < m_MatchBtns.Length; i++)
        {
            m_MatchBtns[i].SetActive(bShow[i]);
        }

        if (bStart)
            CCustomDialog.OpenCustomWaitUI(2302, false);
        else
            CCustomDialog.CloseCustomWaitUI();
#endif
    }

#if !MATCH_ROOM
    void OnClickMatchButton(int index, bool playSound)
    {
        if (playSound)
            CustomAudioDataManager.GetInstance().PlayAudio(3006);

        if (index == 0)//开始匹配
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_ENTERMATCH);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(m_nCurrenLevel);
            HallMain.SendMsgToRoomSer(msg);
        }
        else//取消匹配
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_CANCLEMATCH);
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);
        }
    }
#endif
}