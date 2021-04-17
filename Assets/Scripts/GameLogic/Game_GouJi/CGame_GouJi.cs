using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using USocket.Messages;
using DG.Tweening;
using XLua;
using UnityEngine.EventSystems;

#region "够级游戏枚举"
//房间状态
[LuaCallCSharp]
public enum RoomState_Enum
{
    RoomState_Init = 0,
    RoomState_WaitPlayer,         //等人
    RoomState_WaitReady,          //等待准备
    RoomState_TotalBegin,         //总的开始
    RoomState_OnceBeginShow,      //每局开始前的显示
    RoomState_CountDownBegin,     //游戏开始倒计时
    RoomState_DealPoker,          //发牌
    RoomState_Mai3,               //买3
    RoomState_Mai4,               //买4
    RoomState_QuanGong,           //圈贡
    RoomState_DianGong,           //点贡
    RoomState_ShaoGong,           //烧贡
    RoomState_MenGong,            //闷贡
    RoomState_LaGong,             //落贡
    RoomState_KangGong,           //抗贡
    RoomState_GeMing,             //革命
    RoomState_YaoTou,             //要头
    RoomState_XuanDian,           //宣点
    RoomState_WaitPlayerDeal,     //发一张牌给玩家并等待他出牌
    RoomState_AskShaoPai,         //烧牌提问
    RoomState_ShaoPai,            //烧牌
    RoomState_OnceResult,         //一局结果
    RoomState_OnceEnd,            //
    RoomState_TotalResult,        //总的结果
    RoomState_TotalEnd,           //
    RoomState_Count				  //回收状态
};

//玩家状态
[LuaCallCSharp]
public enum GouJiPlayerState_Enum
{
    GouJiPlayerState_Init = 0,
    GouJiPlayerState_Match,             //在匹配中
    GouJiPlayerState_GameIn,            //在游戏中
    GouJiPlayerState_OnGameButIsOver,   //3游戏中但游戏已经结束
    GouJiPlayerState_ReadyHall,         //玩家在大厅
    GouJiPlayerState_OnDesk,            //玩家在桌子上
    GouJiPlayerState_Max
};

//金钱改变类型
[LuaCallCSharp]
public enum ChangeCoinType_Enum
{
    ChangeCoinType_Init,
    ChangeCoinType_RoomCharge,        //房费
    ChangeCoinType_ZouKe,
    ChangeCoinType_KaiDian,
    ChangeCoinType_ShaoPai,
    ChangeCoinType_FanShao,
    ChangeCoinType_Men3,
    ChangeCoinType_LianBang,
    ChangeCoinType_XuanDian,
    ChangeCoinType_2K1,


    GjChangeCoinType
};

//玩家行为状态
[LuaCallCSharp]
public enum RoleDoing_Enum
{
    RoleDoing_Init = 0,
    RoleDoing_GeMing,           //革命
    DRoleDoing_YaoTou,          //要头
    RoleDoing_XuanDian,         //宣点
    RoleDoing_ChuPai,           //出牌
    RoleDoing_RangPai,          //让牌
    RoleDoing_ShaoPai,          //烧牌
    RoleDoing_FanShao,          //反烧
    RoleDoing_ShaoChuPai,       //烧牌第一次出牌  
    RoleDoing_Count
};

//宣点
[LuaCallCSharp]
public enum XuanDianType_Enum
{
    eXDT_No,
    eXDT_Yes,
    eXDT_Natural_No,
    eXDT_Natural_Yes,
};

//要头
[LuaCallCSharp]
public enum YaoTouType_Enum
{
    eYTT_None,
    eYTT_YaoTou,
    eYTT_WuTou,
    eYTT_GeMing,
};

//开点
[LuaCallCSharp]
public enum KaiDianType_Enum
{
    eKDT_None,
    eKDT_Success,
    eKDT_Fail,
    eKDT_Ready,
};

//玩法规则
[LuaCallCSharp]
public enum GouJiRule_Enum
{
    eGJR_BeiShan = 0,   //憋三
    eGJR_KaFaDianShi,   //开点发四
    eGJR_TwoShaYi,      //大王2杀1
    eGJR_KaiDian,       //开点
    eGJR_XuanDian,      //宣点
    eGJR_ShangGong,     //上贡
    eGJR_Max
}

//角色徽章
[LuaCallCSharp]
public enum RoleBadge_Enum
{
    eRB_XuanDain = 0,   //宣点
    eRB_XuanDain_Mo,    //宣点失败
    eRB_KaiDian,        //开点
    eRB_KaiDian_NO,     //开点失败
    eRB_ShaoPai,        //烧牌
    eRB_BeiShao,        //被烧
    eRB_MenRen,         //闷人
    eRB_BeiMen,         //被闷
    eRB_Max
}

//进贡和买三买四动画表现阶段类
[LuaCallCSharp]
public enum AnimationStage_Enum
{
    eAS_Ready,  //准备
    eAS_Show,   //显示
    eAS_Move,   //移动
    eAS_End,    //结束
    eAS_Count,  
};
#endregion

#region "够级游戏数据结构"
//游戏结算面板显示数据
[LuaCallCSharp]
public class RoleResultData
{
    public byte m_nRank;               //名次
    public byte m_nSit;                //座位号
    public uint m_nPlayerid = 0;       //玩家Id
    public long m_nAddCoin = 0;        //本局得分(钱)
    public long m_nTotalCoin = 0;      //总分(钱)--比赛使用
    public int[] m_nCoinTypeValue = new int[4] { 0,0,0,0};//点，烧，闷，落
}

//买三买四数据
[LuaCallCSharp]
public class RoleSwapPokerData
{
    public byte m_nMSit = RoomInfo.NoSit;   //买家座位号
    public byte m_nMPokerValue = 0;         //买家牌
    public byte m_nTSit = RoomInfo.NoSit;   //卖家座位号
    public byte m_nTPokerValue = 0;         //卖家牌
}

//进贡数据
[LuaCallCSharp]
public class RoleTributeData
{
    public byte m_nTributeOutSit = RoomInfo.NoSit;              //提供贡牌的人座位号
    public byte m_nTributeInSit = RoomInfo.NoSit;               //接收贡牌给的人座位号
    public sbyte m_nTributeState = sbyte.MaxValue;              //贡牌状态(0:无牌可供|-1:联邦抗贡|-2:个人抗贡)
    public List<byte> m_TributePokerList = new List<byte>();    //贡牌数据
}

/// <summary>
/// 卡牌数据
/// </summary>
[Hotfix]public class CardData
{
    public CardData()
    {
        m_bGongMask = false;
        m_bSelectState = false;
        m_nCardValue = 0;
        m_fCardYPostion = float.NaN;
        m_CartTransform = null;
    }

    public bool m_bGongMask;
    /// <summary>
    /// 牌选中状态
    /// </summary>
    public bool m_bSelectState;
    /// <summary>
    /// 牌值
    /// </summary>
    public byte m_nCardValue;
    /// <summary>
    /// 牌局部Y轴上的值
    /// </summary>
    public float m_fCardYPostion;
    /// <summary>
    /// 牌对象
    /// </summary>
    public UnityEngine.Transform m_CartTransform;
}

//当前房间状态协成数据(买三或买四、进贡)
[LuaCallCSharp]
public class RoomStateEnumeratorData
{
    public RoomStateEnumeratorData()
    {
        m_bReverseState = false;
        m_nAStageState = AnimationStage_Enum.eAS_Count;
        m_nRoomState = RoomState_Enum.RoomState_Count;
        m_CurRoleSwapPokerDataList = new List<RoleSwapPokerData>();
        m_CurRoleTributeDataList = new List<RoleTributeData>();
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetRoomStateData()
    {
        m_bReverseState = false;
        m_nRoomState = RoomState_Enum.RoomState_Count;
        m_nAStageState = AnimationStage_Enum.eAS_Ready;
        m_CurRoleSwapPokerDataList.Clear();
        m_CurRoleTributeDataList.Clear();
    }

    public void ReplicateRoomStateData(RoomStateEnumeratorData roomStateData)
    {
        m_nRoomState = roomStateData.m_nRoomState;
        m_nAStageState = roomStateData.m_nAStageState;
        m_CurRoleSwapPokerDataList.AddRange(roomStateData.m_CurRoleSwapPokerDataList);
        m_CurRoleTributeDataList.AddRange(roomStateData.m_CurRoleTributeDataList);
    }

    /// <summary>
    /// 房间状态
    /// </summary>
    public RoomState_Enum m_nRoomState;

    /// <summary>
    /// 当前进行阶段
    /// </summary>
    public AnimationStage_Enum m_nAStageState;

    /// <summary>
    /// 播放状态（true:倒播，false:正播）
    /// </summary>
    public bool m_bReverseState;

    /// <summary>
    /// 买三或买四数据
    /// </summary>
    public List<RoleSwapPokerData> m_CurRoleSwapPokerDataList;
    /// <summary>
    /// 进贡数据
    /// </summary>
    public List<RoleTributeData> m_CurRoleTributeDataList;
}

#endregion

/// <summary>
/// 够级游戏核心类
/// </summary>
[Hotfix]
public class CGame_GouJi : CGameBase
{
    /// <summary>
    /// 断线重连成功标志
    /// </summary>
    bool m_bReconnected = false;
    /// <summary>
    /// 当前房间等级
    /// </summary>
    byte m_nCurrenRoomLevel = RoomInfo.NoSit;
    /// <summary>
    /// 延迟加载资源帧数
    /// </summary>
    sbyte m_nWaitingLoad = 0;
    /// <summary>
    /// 房间状态
    /// </summary>
    RoomState_Enum m_eRoomState = RoomState_Enum.RoomState_Init;
    /// <summary>
    /// 房间等级
    /// </summary>
    byte m_nCurRoomLevel = RoomInfo.NoSit;
    /// <summary>
    /// 够级游戏规则
    /// </summary>
    int m_nGouJiRule = 0;
    /// <summary>
    /// 游戏场景挂载对象
    /// </summary>
    Transform m_RootTransform = null;
    /// <summary>
    /// 游戏主场景对象
    /// </summary>
    Transform m_MainUITransform = null;
    /// <summary>
    /// 游戏通用特效动画挂载点
    /// </summary>
    Transform m_AnimationTfm = null;
    /// <summary>
    /// 游戏结算对象
    /// </summary>
    Transform m_ResultMainTransform = null;
    /// <summary>
    /// 游戏设置界面对象
    /// </summary>
    Transform m_SetMainTransform = null;
    /// <summary>
    /// 进贡详情面板对象
    /// </summary>
    Transform m_TributeTransform = null;
    /// <summary>
    /// 聊天面板对象
    /// </summary>
    Transform m_ChatTransform = null;
    /// <summary>
    /// 进贡文本对象
    /// </summary>
    Text m_TributeText = null;
    /// <summary>
    /// 游戏继续按钮对象
    /// </summary>
    Button m_EnterButton = null;
    /// <summary>
    /// 游戏离开按钮对象
    /// </summary>
    Button m_LeaveButton = null;
    /// <summary>
    /// 托管按钮
    /// </summary>
    Button m_TrustButton = null;
    /// <summary>
    /// 房间是否免费
    /// </summary>
    public bool m_bIsFree = false;
    /// <summary>
    /// 围观玩家座位号
    /// </summary>
    public byte m_nLookAtRoleSit = RoomInfo.NoSit;
    /// <summary>
    /// 最近烧牌玩家座位号
    /// </summary>
    public byte m_nRecentlyShaoPaiSit = RoomInfo.NoSit;
    /// <summary>
    /// 最近被烧牌玩家座位号
    /// </summary>
    public byte m_nRecentlyBeiShaoPaiSit = RoomInfo.NoSit;
    /// <summary>
    /// 最近成功出牌玩家座位号
    /// </summary>
    public byte m_nRecentlyOutPokerSit = RoomInfo.NoSit;
    /// <summary>
    /// 最近玩家出牌数据
    /// </summary>
    public List<byte> m_RecentlyOutPokerList = new List<byte>();
    /// <summary>
    /// 当前显示定时器的玩家座位号
    /// </summary>
    public byte m_nCurTimePanelRoleSit = RoomInfo.NoSit;
    /// <summary>
    /// 游戏对象
    /// </summary>
    public List<GouJi_Role> m_PlayerList = new List<GouJi_Role>();
    /// <summary>
    /// 游戏AB资源包
    /// </summary>
    public AssetBundle m_GouJiAssetBundle = null;
    /// <summary>
    /// 游戏公用资源包
    /// </summary>
    public AssetBundle m_GouJiCommonAssetBundle = null;
    /// <summary>
    /// 游戏画布对象
    /// </summary>
    public UnityEngine.Canvas GameCanvas = null;
    /// <summary>
    /// 定时器
    /// </summary>
    public CustomCountdownImgMgr CCIMgr = null;
    /// <summary>
    /// 进贡动画对象
    /// </summary>
    GameObject m_AnimatonGongObject = null;
    /// <summary>
    /// 结算动画对象
    /// </summary>
    GameObject m_AnimatonResultObject = null;
    /// <summary>
    /// 约据准备界面
    /// </summary>
    GameObject m_AppointmentReadyPanel = null;
    /// <summary>
    /// 约据游戏规则界面
    /// </summary>
    GameObject m_AppointmentRulePanel = null;
    /// <summary>
    /// 约据游戏总结算界面
    /// </summary>
    GameObject m_AppointmentResultPanel = null;
    /// <summary>
    /// 约据准备阶段退出按钮对象
    /// </summary>
    Button m_AppointmentReadyExitButton = null;
    /// <summary>
    /// 约据游戏中退出按钮对象
    /// </summary>
    Button m_AppointmentGameExitButton = null;
    /// <summary>
    /// 约据规则协成对象
    /// </summary>
    IEnumerator m_AppointmentRuleEnumerator = null;
    /// <summary>
    /// 当前房间状态协成对象
    /// </summary>
    IEnumerator m_CurRoomStateEnumerator = null;
    /// <summary>
    /// 当前房间状态数据
    /// </summary>
    RoomStateEnumeratorData m_CurRoomStateStateData = new RoomStateEnumeratorData();
    /// <summary>
    /// 当前游戏协程完成数据
    /// </summary>
    public Dictionary<RoomState_Enum, byte> m_CurCoroutinesDictionary = new Dictionary<RoomState_Enum, byte>();
    /// <summary>
    /// 游戏待播放音效
    /// </summary>
    public List<string> m_GameAudioList = new List<string>();
    /// <summary>
    /// 当前阶段买三、买四、进贡创建的牌对象
    /// </summary>
    List<Transform> m_CurCoroutinesObjectList = new List<Transform>();
    /// <summary>
    /// 音效ID编码
    /// </summary>
    private static int m_nAudioIndex = 0;
    /// <summary>
    /// 游戏约据当前局数
    /// </summary>
    private int m_nCurGameRound = 0;
    /// <summary>
    /// 角色昵称
    /// </summary>
    string[] m_sRoleNickname = { "自己", "下家", "下联", "对家", "上联", "上家" };
    /// <summary>
    /// 进贡名称
    /// </summary>
    static string[] m_sTributeName = { "[买3]\n", "[买4]\n", "[圈贡]\n", "[点贡]\n", "[烧贡]\n", "[闷贡]\n", "[落贡]\n" };

    bool bExitAppointmentDialogState = false;

    #region 录像对象
    /// <summary>
    /// 录像界面对象
    /// </summary>
    public UnityEngine.Transform m_RecordTransform = null;

    /// <summary>
    /// 当前录像状态
    /// </summary>
    VideoActionInfo_Enum m_curVideoActionInfoState;

    /// <summary>
    /// 四户乱缠状态
    /// </summary>
    bool m_bSiHuLuanChanState = false;
    #endregion
    public CGame_GouJi(GameTye_Enum gameType) : base(GameKind_Enum.GameKind_GouJi)
    {
        GameMode = gameType;
        m_bIsFree = false;
        m_eRoomState = RoomState_Enum.RoomState_Init;
        m_nCurrenRoomLevel = RoomInfo.NoSit;
        InitMsgHandle();
    }

    /// <summary>
    /// 游戏初始函数
    /// </summary>
    public override void Initialization()
    {
        base.Initialization();

        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameKind_Enum.GameKind_GouJi, "GameGouJiAudioCsv");
        CCIMgr = new CustomCountdownImgMgr();
        //请求进入房间
        ReqestChooseLevelToServerMsg();

        if (!m_bReconnected)
        {
            switch (GameMode)
            {
                case GameTye_Enum.GameType_Normal:
                    MatchRoom.GetInstance().ShowRoom((byte)GameType);
                    m_nWaitingLoad = -1;
                    break;
                case GameTye_Enum.GameType_Appointment:
                    m_nWaitingLoad = -1;
                    LoadAppointmentResource();
                    break;
            }        }
        LoadResource();
    }

    /// <summary>
    /// 够级消息注册
    /// </summary>
    void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_CHOOSElEVEL, HandChooseLevelNetMsg);          //匹配房间数据
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER, HandLeaveRoomNetMsg);    //离开房间消息回复
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_ROOMSTATE, HandRoomStateNetMsg);              //同步房间状态到客户端
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_ENTERROOM, HandEnterRoomNetMsg);              //进入房间成功
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_DEALMJBEGIN, HandStartDealPokerMsg);          //开始发牌
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_ASKDEAL, HandAskdelMsg);                      //提问
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_PUBLISHASKDEAL, HandUpdateAskdelMsg);         //同步提问
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_ANSWERDOING, HandAnswerDoingMsg);             //回复此人提问答复
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_UPDATEDOING, HandOtherAnswerDoingMsg);        //同步其他人提问答复
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_YAOTOUINFO, HandYaoTouInfoMsg);               //通知要头相关
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_XUANDIANINFO, HandXuanDianInfoMsg);           //通知宣点相关
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_UPDATEDEALPOKER, HandUpdateEalmjPokerMsg);    //同步出牌
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_BACKDEALMJPOKER, HandBackEalmjPokerMsg);      //回复出牌玩家
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_PUBLISHRESULT, HandGameResultMsg);            //同步游戏结束
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_CLEARRANGPAI, HandClearRanPaiMsg);            //通知取消让牌
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_KAIDIAN, HandKaiDianMsg);                     //同步开点情况
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_4HULUANCHAN, HandFourHuLuaChanMsg);           //通知进入四户乱缠
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_BIE3, HandBieThreeMsg);                       //通知憋3
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_FRIENDPOKERS, HandFriendPokersMsg);           //同步队友的牌
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION, HandEmotionMsg);                       //聊天
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_MIDDLEENTERROOM, HandMiddleEnterRoomMsg);     //中途加入
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_AFTERONLOOKERENTER, HandleBystanderEnter);    //中途加入
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_BACKLEAVE, HandleBackLevel);                  //离开房间的人
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_SM_ENTERTRUSTSTATE, HandleEnterTrustStateMsg);   //进入托管状态
    }

    /// <summary>
    /// 初始化游戏数据
    /// </summary>
    void ResetGameData(bool bDestroryState = true)
    {
        if (m_ResultMainTransform)
        {
            m_ResultMainTransform.gameObject.SetActive(false);
        }

        if (m_AnimatonResultObject)
        {
            GameObject.Destroy(m_AnimatonResultObject);
        }

        m_RecentlyOutPokerList.Clear();

        foreach (var role in m_PlayerList)
        {
            role.ResetRoleData(bDestroryState);
        }

        if (m_AnimatonGongObject)
        {
            GameObject.Destroy(m_AnimatonGongObject);
        }
        if (m_TributeText)
        {
            m_TributeText.text = string.Empty;
        }
        if (m_ChatTransform)
        {
            m_ChatTransform.gameObject.SetActive(false);
        }
        if(m_SetMainTransform)
        {
            m_SetMainTransform.gameObject.SetActive(false);
        }
        if (bExitAppointmentDialogState)
        {
            CCustomDialog.CloseCustomDialogUI();
        }
        SetTributeInfoPanelActive(false);
        m_nLookAtRoleSit = RoomInfo.NoSit;
        m_CurCoroutinesDictionary.Clear();
        m_GameAudioList.Clear();
        m_nAudioIndex = 0;
        m_bSiHuLuanChanState = false;
        m_CurRoomStateEnumerator = null;
        m_curVideoActionInfoState = VideoActionInfo_Enum.VideoActionInfo_Gouji;
        GameMain.Instance.StopAllCoroutines();
    }

    /// <summary>
    /// 删除所有的子组件
    /// </summary>
    /// <param name="parentTransform"></param>
    public void DestoryAllChildObject(Transform parentTransform)
    {
        if (parentTransform == null)
        {
            return;
        }
        for (int index = 0; index < parentTransform.childCount; ++index)
        {
            GameObject.Destroy(parentTransform.GetChild(index).gameObject);
        }
    }

    /// <summary>
    /// 清除两个玩家之间其他玩家的行牌数据
    /// </summary>
    /// <param name="startSit">开始座位号</param>
    /// <param name="endSit">结束座位号</param>
    void RemoveLinePromptPokerData(int startSit, int endSit)
    {
        if (endSit > 5)
        {
            endSit = 0;
        }
        startSit += 1;
        if (startSit > 5)
        {
            startSit = 0;
        }

        DebugLog.Log("开始的位置：" + startSit + "结束的位置：" + endSit);
        while (startSit != endSit)
        {
            DebugLog.Log("清理的位置：" + startSit);
            if (m_nRecentlyOutPokerSit != startSit)
            {
                DebugLog.Log("成功清理的位置：" + startSit);
                m_PlayerList[startSit].DestoryAllOutPokerObject();
                m_PlayerList[startSit].UpdatePromptText(string.Empty);
                m_PlayerList[startSit].RefreshOutPokerPromptUI(0);
            }
            startSit += 1;
            if (startSit > 5)
            {
                startSit = 0;
            }
        }
    }

    /// <summary>
    /// 刷新协成完成标记数据
    /// </summary>
    /// <param name="roomState">协成类型</param>
    public void RefreshCoroutinesDictionary(RoomState_Enum roomState)
    {
        if (m_CurCoroutinesDictionary.ContainsKey(roomState))
        {
            --m_CurCoroutinesDictionary[roomState];
        }
    }

    /// <summary>
    /// 同步房间信息
    /// </summary>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool InitGameRoomInfo(UMessage _ms)
    {
        m_nGouJiRule = 0;

        for (GouJiRule_Enum index = GouJiRule_Enum.eGJR_BeiShan; index < GouJiRule_Enum.eGJR_Max; ++index)
        {
            if (_ms.ReadByte() == 1)
            {
                GameKind.AddFlag((int)index, ref m_nGouJiRule);
            }
        }

        UpdateRoomRuleTextData();
        return true;
    }

    /// <summary>
    /// 初始化主控角色基本信息
    /// </summary>
    void InitRoleLocalInfo()
    {
        PlayerData playerData = GameMain.hall_.GetPlayerData();        m_PlayerList[0].m_sRoleName = playerData.GetPlayerName();        m_PlayerList[0].m_nUseId = playerData.GetPlayerID();        m_PlayerList[0].m_nFaceId = (int)playerData.PlayerIconId;        m_PlayerList[0].m_sUrl = playerData.GetPlayerIconURL();        m_PlayerList[0].m_nSex = playerData.PlayerSexSign;        m_PlayerList[0].m_fMasterScore = playerData.MasterScoreKindArray[(int)GameType];
        m_PlayerList[0].m_nDisconnectState = 1;
        m_PlayerList[0].RefreshInfoUI();    }

    /// <summary>
    /// 判断当前游戏里面规则
    /// </summary>
    /// <param name="rule">游戏规则</param>
    /// <returns></returns>
    public bool IsGameRule(GouJiRule_Enum rule)
    {
        return GameKind.HasFlag((int)rule, m_nGouJiRule);
    }

    /// <summary>
    /// 匹配房间数据
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandChooseLevelNetMsg(uint _msgType, UMessage _ms)
    {
        byte nState = _ms.ReadByte();//1:id非法或Level非法 2：钱不够
        byte level = _ms.ReadByte();
        ushort deskNum = _ms.ReaduShort();
        m_bIsFree = (_ms.ReadByte() == 1);

        if (nState == 0)
        {
            MatchRoom.GetInstance().AddDesk(deskNum, 6);
        }
        else
        {
            DebugLog.Log("Choose level failed: " + nState);

            if (nState == 2)
            {
                LL_RoomData rd = LandLords_Data.GetInstance().m_RoomData[level];
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(2307, rd.m_nMinCoin);
            }
            else if (nState == 3)
            {
                LL_RoomData rd = LandLords_Data.GetInstance().m_RoomData[level];
                CCustomDialog.OpenCustomConfirmUIWithFormatParam(2310, rd.m_nMinCoin);
            }
            else
                CCustomDialog.OpenCustomConfirmUI(2301);
        }
        return true;
    }

    /// <summary>
    /// 离开房间
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandLeaveRoomNetMsg(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        if (state == 0)
            BackToGameRoom();
        else
            CCustomDialog.OpenCustomConfirmUI(2305);
        return true;
    }

    /// <summary>
    /// 同步房间状态到客户端
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandRoomStateNetMsg(uint _msgType, UMessage _ms)
    {
        float time = _ms.ReadSingle();
        byte state = _ms.ReadByte();
        OnStateChange((RoomState_Enum)state, _ms,0,time);
        return true;
    }

    /// <summary>
    /// 进入房间成功。
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandEnterRoomNetMsg(uint _msgType, UMessage _ms)
    {
        //播放背景音乐
        CustomAudioDataManager.GetInstance().PlayAudio(GetGameAudioID(0,1), false);

        if (GameMode == GameTye_Enum.GameType_Contest)
        {
            MatchInGame.GetInstance().ResetGui();
        }
        uint roomId = _ms.ReadUInt();        byte roomLevel = _ms.ReadByte();        InitGameRoomInfo(_ms);        byte roomState = _ms.ReadByte();        m_PlayerList[0].m_nSSit = _ms.ReadByte();        m_PlayerList[0].m_nTotalCoin = _ms.ReadLong();        InitRoleLocalInfo();        byte sitNum = _ms.ReadByte();
        for (byte sit = 0; sit < sitNum; ++sit)
        {
            byte roleServetSit = _ms.ReadByte();
            byte roleClientSit = GetClientRoleSit(roleServetSit, m_PlayerList[0].m_nSSit);
            m_PlayerList[roleClientSit].m_nSSit = roleServetSit;
            m_PlayerList[roleClientSit].m_nUseId = _ms.ReadUInt();
            m_PlayerList[roleClientSit].m_nFaceId = _ms.ReadInt();
            m_PlayerList[roleClientSit].m_sUrl = _ms.ReadString();
            m_PlayerList[roleClientSit].m_nTotalCoin = _ms.ReadLong();
            m_PlayerList[roleClientSit].m_sRoleName = _ms.ReadString();
            m_PlayerList[roleClientSit].m_nDisconnectState = _ms.ReadByte();
            m_PlayerList[roleClientSit].m_fMasterScore = _ms.ReadSingle();
            m_PlayerList[roleClientSit].m_nSex = _ms.ReadByte();
            m_PlayerList[roleClientSit].RefreshInfoUI();
        }
        uint contestID = _ms.ReadUInt();//比赛ID
        uint appointmentID = _ms.ReadUInt();//约据ID
        DebugLog.Log("Start landlord game, otherNum:" + sitNum);
        return true;
    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandStartDealPokerMsg(uint _msgType, UMessage _ms)
    {
        byte bankerSit = _ms.ReadByte();//庄家座位号
        float time = _ms.ReadSingle();     //发牌阶段时间
        byte pokerNum = _ms.ReadByte(); //手牌数量
        for (int index = 0; index < pokerNum; ++index)
        {
            CardData cardData = new CardData();
            cardData.m_nCardValue = _ms.ReadByte();
            if(Bystander)
            {
                cardData.m_nCardValue = RoomInfo.NoSit;
            }
            m_PlayerList[0].m_HavePokerList.Add(cardData);
        }
        m_CurCoroutinesDictionary.Add(RoomState_Enum.RoomState_DealPoker, 1);
        GameMain.SC(m_PlayerList[0].DealPoker(Mathf.Max(time - 1.0f, 0) / Mathf.Max(pokerNum, 1)));
        return true;
    }

    /// <summary>
    /// 提问
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandAskdelMsg(uint _msgType, UMessage _ms)
    {
        sbyte lastChuPaiRoleSit = _ms.ReadSByte();//是否有牌权(座位号)
        ushort askBit = _ms.ReaduShort();//行为标识
        uint askSign = _ms.ReadUInt();//提问ID
        float time = _ms.ReadSingle();//提问时间

        AskdelEvent(askBit, askSign, time, lastChuPaiRoleSit);
        return true;
    }

    /// <summary>
    /// 同步提问
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandUpdateAskdelMsg(uint _msgType, UMessage _ms)
    {
        byte roleSit = _ms.ReadByte();//出牌玩家座位号
        byte beforeOutPokerSit = _ms.ReadByte();//是否有牌权(座位号)
        float time = _ms.ReadSingle();//出牌时间

        GouJi_Role roleObject = m_PlayerList.Find(playerPoker => playerPoker.m_nSSit == roleSit);
        if (roleObject != null)
        {
            if (m_PlayerList.Count > m_nCurTimePanelRoleSit && m_nCurTimePanelRoleSit >= 0)
            {
                m_PlayerList[m_nCurTimePanelRoleSit].ShowCountdownPanel(false);
            }

            //清除已经有名次玩家的出牌对象
            if (beforeOutPokerSit == roleSit)//有牌权
            {
                m_RecentlyOutPokerList.Clear();
                foreach (var role in m_PlayerList)
                {
                    role.UpdatePromptText(string.Empty);
                    role.DestoryAllOutPokerObject();
                    role.RefreshOutPokerPromptUI(0);
                }
            }
            else
            {
                roleObject.UpdatePromptText(string.Empty);
                roleObject.DestoryAllOutPokerObject();
                roleObject.RefreshOutPokerPromptUI(0);
                //RemoveLinePromptPokerData(m_nCurTimePanelRoleSit != m_nRecentlyOutPokerSit ? m_nCurTimePanelRoleSit : m_nRecentlyOutPokerSit, roleObject.m_nCSit + 1);
            }

            m_nCurTimePanelRoleSit = roleObject.m_nCSit;
            if (!roleObject.m_bBeiThreeState)
            {
                roleObject.ShowCountdownPanel(true, time);
            }
        }

        DebugLog.Log("同步提问:出牌座位号: " + roleSit + " beforeOutPokerSit: " + beforeOutPokerSit + " 时间: " + time);
        return true;
    }

    /// <summary>
    /// 回复自己的答复
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandAnswerDoingMsg(uint _msgType, UMessage _ms)
    {
        sbyte state = _ms.ReadSByte(); //0:成功1:此答非所问2:没有此问题或已经过时
        if (state != 0)
        {
            DebugLog.Log("答复失败: " + state);
            ((GouJi_RoleLocal)m_PlayerList[0]).ResetAskdelPanelInteractable(true);
            return false;
        }
        RoleDoing_Enum answerDoing = (RoleDoing_Enum)_ms.ReadByte();
        m_PlayerList[0].AnswerDoingEvent((RoleDoing_Enum)answerDoing,answerDoing != RoleDoing_Enum.RoleDoing_XuanDian && answerDoing != RoleDoing_Enum.RoleDoing_GeMing);
        DebugLog.Log("答复成功: " + answerDoing + " 枚举: " + answerDoing);

        return true;
    }

    /// <summary>
    /// 同步其他人答复
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandOtherAnswerDoingMsg(uint _msgType, UMessage _ms)
    {
        byte sit = _ms.ReadByte();//答复人座位号
        byte pokerSit = _ms.ReadByte();//出牌人的座位号
        RoleDoing_Enum answerDoing = (RoleDoing_Enum)_ms.ReadByte();

        GouJi_Role roleObject = m_PlayerList.Find(role => { return role.m_nSSit == sit; });
        if (roleObject != null)
        {
            roleObject.AnswerDoingEvent((RoleDoing_Enum)answerDoing, answerDoing != RoleDoing_Enum.RoleDoing_XuanDian && answerDoing != RoleDoing_Enum.RoleDoing_GeMing);
        }

        DebugLog.Log("其他人答复成功: " + sit + " : " + pokerSit + " : " + answerDoing);
        return true;
    }

    /// <summary>
    /// 通知宣点相关
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandXuanDianInfoMsg(uint _msgType, UMessage _ms)
    {
        byte roleNum = _ms.ReadByte();
        for (int sit = 0; sit < roleNum; ++sit)
        {
            byte roleSit = _ms.ReadByte();
            byte xuanDianType = _ms.ReadByte();
            GouJi_Role roleObject = m_PlayerList.Find(role => { return role.m_nSSit == roleSit; });
            if (roleObject != null)
            {
                roleObject.AnswerDoingEvent(RoleDoing_Enum.RoleDoing_XuanDian,true, xuanDianType);
            }
        }
        return true;
    }

    /// <summary>
    /// 通知要头相关
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandYaoTouInfoMsg(uint _msgType, UMessage _ms)
    {
        byte geMingFrontSit = _ms.ReadByte();//要头座位号
        YaoTouType_Enum doingType = (YaoTouType_Enum)_ms.ReadByte();//对家的选择

        GouJi_Role roleObject = m_PlayerList.Find(role => { return role.m_nSSit == geMingFrontSit; });
        if (roleObject != null)
        {
            switch (doingType)
            {
                case YaoTouType_Enum.eYTT_YaoTou:
                    roleObject.AnswerDoingEvent(RoleDoing_Enum.DRoleDoing_YaoTou,true, 0);
                    break;
                case YaoTouType_Enum.eYTT_WuTou:
                    roleObject.ShowGeMingYaotouInfoUI(true, doingType);
                    break;
            }
        }
        return true;
    }

    /// <summary>
    /// 同步出牌数据
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandUpdateEalmjPokerMsg(uint _msgType, UMessage _ms)
    {
        byte roleSit = _ms.ReadByte();          //出牌玩家座位号

        uint roleID = _ms.ReadUInt();           //角色ID
        byte lastRoleSit = _ms.ReadByte();      //上一次成功出牌的玩家座位号
        byte bIsFirst = _ms.ReadByte();         //是否是第一次出牌1:是0:不是
        byte rolePokerNum = _ms.ReadByte();     //剩余牌张数    
        byte gouJiState = _ms.ReadByte();       //是否是够极牌 1:是 0:不是   

        GouJi_Role roleObject = m_PlayerList.Find(playerPoker => playerPoker.m_nSSit == roleSit);
        if (roleObject == null)
        {
            DebugLog.Log("找不到当前出牌玩家座位号: " + roleSit);
        } else
        {
            roleObject.ShowCountdownPanel(false);
            roleObject.ResetOutPokerPanel();
            roleObject.RefreshRemainPokerPromptUI(rolePokerNum < 10 && rolePokerNum > 0);
        }

        byte outPokerCount = _ms.ReadByte();         //出牌数
        if (outPokerCount > 0)
        {
            if (roleObject != null)
            {
                m_nRecentlyOutPokerSit = roleObject.m_nCSit;
            }
            m_RecentlyOutPokerList.Clear();
        }

        DebugLog.Log( "出牌座位号: "+ roleSit + " ################# " + outPokerCount + " ##################### ");
        for (byte index = 0; index < outPokerCount; ++index)
        {
            byte pokerValue = _ms.ReadByte();
            if (roleSit != m_PlayerList[0].m_nSSit)
            {
                m_RecentlyOutPokerList.Add(pokerValue);
            }
            if (roleObject != null)
            {
                CardData cardData = new CardData();
                cardData.m_nCardValue = pokerValue;
                roleObject.m_OutPokerList.Add(cardData);
            }
        }

        sbyte ranking = _ms.ReadSByte(); //角色排名(0,1,2...),-1:没有名次
        byte friendPokerState = _ms.ReadByte();//1:队友0:自己(玩家手牌已经出完，需要同步显示队友的牌)
        byte specialState = _ms.ReadByte();//1:二杀一的牌 0:正常牌

        if(specialState == 1)
        {
            GameFunction.PlayUIAnim("anime_GJ_2sha1", 2f, m_AnimationTfm, m_GouJiAssetBundle);
            PlayerGameAudio(roleObject.m_nCSit,15);
        }

        if (roleObject != null)
        {
            if (ranking >= 0)
            {
                roleObject.RefreshRoleRankUI(ranking);
            }

            if (outPokerCount > 0)
            {
                roleObject.PlayerOutPokerPromptAnimation(gouJiState == 1);

                PlayerGameAudio(roleObject.m_nCSit, gouJiState == 1 ? 9 : 8);
            }

            if (m_PlayerList[0] != null)
            {
                if (m_PlayerList[0].m_bLookAtRoleState && m_nLookAtRoleSit == roleSit)
                {
                    m_PlayerList[0].m_OutPokerList.Clear();
                    m_PlayerList[0].m_OutPokerList.AddRange(roleObject.m_OutPokerList);
                    m_PlayerList[0].RefreshHavePokerPanel();
                }
            }
            roleObject.UpdatePromptText(outPokerCount == 0 ? "过牌" : string.Empty);
            roleObject.RefreshOutPokerPanel();
            roleObject.RefreshHavePokerPanel();
            roleObject.RefreshOutPokerPromptUI(outPokerCount);
            roleObject.RefreshDiscardPokerPromptUI(false);
            roleObject.ShowRoleDoingPanel(RoleDoing_Enum.RoleDoing_ChuPai, false);
        }
        return true;
    }

    /// <summary>
    /// 回复出牌人消息
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandBackEalmjPokerMsg(uint _msgType, UMessage _ms)
    {
        sbyte state = _ms.ReadSByte();//0:出牌成功1:不在出牌状态2:不该你出牌3:出牌数为空
                                      //4:没有这些牌5:牌型不符或大不过6:憋三玩法不能出三
                                      //7:烧牌中都得带王出牌 8:烧牌过程中王后不够
        if (state > 0)
        {
            CRollTextUI.Instance.AddVerticalRollText((uint)(2800+ state));
            ((GouJi_RoleLocal)m_PlayerList[0]).ResetOutPanelInteractable(true);
            DebugLog.Log("出牌错误! -- 错误码: " + state);
        }
        return true;
    }

    /// <summary>
    /// 游戏结算
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandGameResultMsg(uint _msgType, UMessage _ms)
    {
        byte mulitple = _ms.ReadByte();//倍数 0: 流局 其他倍数
        if (mulitple == 0)
        {
            GameFunction.PlayUIAnim("anime_GJ_LiuJu", 3f, m_AnimationTfm, m_GouJiAssetBundle);
        }
        byte roleNum = _ms.ReadByte();
        List<RoleResultData> RoleResultDataList = new List<RoleResultData>();
        for (byte roleIndex = 0; roleIndex < roleNum; ++roleIndex)
        {
            RoleResultData roleResultData = new RoleResultData();
            int value = 0, valueIndex = -1;
            ChangeCoinType_Enum type = ChangeCoinType_Enum.ChangeCoinType_Init;
            byte coinChangeNum = _ms.ReadByte();//自己钱币改变途径
            for (byte index = 0; index < coinChangeNum; ++index)
            {
                type = (ChangeCoinType_Enum)_ms.ReadByte();//改变类型
                value = _ms.ReadInt();//改变数值
                valueIndex = -1;
                switch (type)
                {
                    case ChangeCoinType_Enum.ChangeCoinType_KaiDian:
                        valueIndex = 0;
                        break;
                    case ChangeCoinType_Enum.ChangeCoinType_ShaoPai:
                        valueIndex = 1;
                        break;
                    case ChangeCoinType_Enum.ChangeCoinType_Men3:
                        valueIndex = 2;
                        break;
                    case ChangeCoinType_Enum.ChangeCoinType_LianBang:
                        valueIndex = 3;
                        break;
                }
                if (valueIndex != -1)
                {
                    roleResultData.m_nCoinTypeValue[valueIndex] = value;
                }
            }
            roleResultData.m_nSit = _ms.ReadByte();//座位号
            roleResultData.m_nRank = _ms.ReadByte();//名次
            roleResultData.m_nPlayerid = _ms.ReadUInt();//useId
            roleResultData.m_nTotalCoin = _ms.ReadLong();//剩余的钱币
            roleResultData.m_nAddCoin = _ms.ReadLong();//本局此人钱币额改变值

            byte pokerNum = _ms.ReadByte();//剩余手上的牌
            List<byte> havePokerList = new List<byte>();
            for (byte pokerIndex = 0; pokerIndex < pokerNum; ++pokerIndex)
            {
                havePokerList.Add(_ms.ReadByte());//牌
            }

            GouJi_Role Role = m_PlayerList.Find(roleData => roleData.m_nSSit == roleResultData.m_nSit);
            if (Role != null)
            {
                Role.RefreshLayPokerPanel(havePokerList);
                Role.RefreshRoleRankUI((sbyte)roleResultData.m_nRank);
            }

            RoleResultDataList.Add(roleResultData);
        }
        m_nCurGameRound = _ms.ReadByte();//第几轮
        byte totalNum = _ms.ReadByte();//总局
        sbyte quanSanHuState = _ms.ReadSByte();//圈三户(圈3户 -1：没有 0：024圈 1：135圈)
        uint baseCoin = _ms.ReadUInt();//底分
        uint charge = _ms.ReadUInt();//房费
        _ms.ReadLong();//录像ID

        if (quanSanHuState >= 0)
        {
            GameFunction.PlayUIAnim("anime_GJ_shengyu3", 3f, m_AnimationTfm, m_GouJiAssetBundle);
        }
        if(mulitple == 0)
        {
            return true;
        }
        GameMain.SC(ShowResult(m_nCurGameRound == totalNum, baseCoin, charge, mulitple, RoleResultDataList));
        return true;
    }

    /// <summary>
    /// 通知取消让牌
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandClearRanPaiMsg(uint _msgType, UMessage _ms)
    {
        sbyte roleSit = _ms.ReadSByte();//让牌玩家座位号
        GouJi_Role role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == roleSit);
        if (role != null)
        {
            if (m_nRecentlyOutPokerSit != role.m_nCSit)
            {
                role.UpdatePromptText("过牌");
                DebugLog.Log("=============让牌玩家座位号:" + roleSit);
            }
        }
        DebugLog.Log("让牌玩家座位号:" + roleSit);
        return true;
    }

    /// <summary>
    /// 同步开点情况
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandKaiDianMsg(uint _msgType, UMessage _ms)
    {
        byte roleSit = _ms.ReadByte();//开点玩家座位号
        KaiDianType_Enum kaiDianType = (KaiDianType_Enum)_ms.ReadByte();//开点状态类型
        XuanDianType_Enum xuanDianType = (XuanDianType_Enum)_ms.ReadByte();//宣点状态类型
        KaiDianType_Enum dJKaiDianType = (KaiDianType_Enum)_ms.ReadByte();//对家开点状态类型
        XuanDianType_Enum dJXuanDianType = (XuanDianType_Enum)_ms.ReadByte();//对家宣点状态类型

        GouJi_Role roleObject = m_PlayerList.Find(role => { return role.m_nSSit == roleSit; });
        if (roleObject != null)
        {
            switch (kaiDianType)
            {
                case KaiDianType_Enum.eKDT_Success:
                    {
                        roleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_KaiDian);
                        if (roleObject.m_eKaiDianType != kaiDianType)
                        {
                            roleObject.AddRoleAnimation("anime_GJ_dian1");
                            PlayerGameAudio(roleObject.m_nCSit, 11);
                        }

                        if ((xuanDianType == XuanDianType_Enum.eXDT_Yes || xuanDianType == XuanDianType_Enum.eXDT_Natural_Yes) &&
                            dJKaiDianType == KaiDianType_Enum.eKDT_Fail)
                        {
                            roleObject.AddRoleAnimation("anime_GJ_dianXuan1");
                        }

                        byte frontSit = (byte)((roleSit + 3) % 6);
                        GouJi_Role frontRoleObject = m_PlayerList.Find(role => { return role.m_nSSit == frontSit; });
                        if (frontRoleObject != null && !frontRoleObject.m_bXuanDianAnimationState &&
                            (dJXuanDianType == XuanDianType_Enum.eXDT_Yes || dJXuanDianType == XuanDianType_Enum.eXDT_Natural_Yes))
                        {
                            frontRoleObject.AddRoleAnimation("anime_GJ_dianXuan0");
                            frontRoleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain, false);
                            frontRoleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain_Mo);
                            frontRoleObject.m_bXuanDianAnimationState = true;
                        }
                    }
                    break;
                case KaiDianType_Enum.eKDT_Fail:
                    {
                        if (kaiDianType == KaiDianType_Enum.eKDT_Fail)
                        {
                            if (roleObject.m_eKaiDianType != kaiDianType)
                            {
                                roleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_KaiDian_NO);
                                roleObject.AddRoleAnimation("anime_GJ_dian0");

                                byte frontSit = (byte)((roleSit + 3) % 6);
                                GouJi_Role frontRoleObject = m_PlayerList.Find(role => { return role.m_nSSit == frontSit; });
                                if ((dJXuanDianType == XuanDianType_Enum.eXDT_Yes || dJXuanDianType == XuanDianType_Enum.eXDT_Natural_Yes) &&
                                    dJKaiDianType == KaiDianType_Enum.eKDT_Success && !frontRoleObject.m_bXuanDianAnimationState && frontRoleObject != null)
                                {
                                    frontRoleObject.AddRoleAnimation("anime_GJ_dianXuan1");
                                    frontRoleObject.m_bXuanDianAnimationState = true;
                                }
                            }

                            if (!roleObject.m_bXuanDianAnimationState &&
                                (xuanDianType == XuanDianType_Enum.eXDT_Yes || xuanDianType == XuanDianType_Enum.eXDT_Natural_Yes))
                            {
                                roleObject.AddRoleAnimation("anime_GJ_dianXuan0");
                                roleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain, false);
                                roleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain_Mo);
                                roleObject.m_bXuanDianAnimationState = true;
                            }
                        }
                    }
                    break;
            }
            roleObject.m_eKaiDianType = kaiDianType;
        }
        DebugLog.Log("开点玩家座位号:" + roleSit + "开点状态类型:" + kaiDianType + "宣点状态类型:" + xuanDianType);
        return true;
    }

    /// <summary>
    /// 通知进入四户乱缠
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandFourHuLuaChanMsg(uint _msgType, UMessage _ms)
    {
        DebugLog.Log("进入四户乱缠！！！！");

        GameFunction.PlayUIAnim("anime_GJ_shengyu4", 3f, m_AnimationTfm, m_GouJiAssetBundle);

        foreach(var player in m_PlayerList)
        {
            if(player.m_eYaoTouType == YaoTouType_Enum.eYTT_WuTou || player.m_eYaoTouType == YaoTouType_Enum.eYTT_YaoTou)
            {
                player.ShowGeMingYaotouInfoUI(false);
            }
        }
        return true;
    }

    /// <summary>
    /// 通知憋3
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandBieThreeMsg(uint _msgType, UMessage _ms)
    {
        byte roleBeiBieSit = _ms.ReadByte();//被憋玩家座位号
        byte roleBieSit = _ms.ReadByte();//憋人玩家座位号
        sbyte roleBeiBieRank = _ms.ReadSByte();//被憋玩家名次
        UpdateBieThreeBadge(roleBieSit, roleBeiBieSit, roleBeiBieRank);

        DebugLog.Log("被憋玩家座位号:" + roleBeiBieSit + "憋人玩家座位号:" + roleBieSit);
        return true;
    }

    /// <summary>
    /// 同步队友的牌
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandFriendPokersMsg(uint _msgType, UMessage _ms)
    {
        byte roleFriendNum = _ms.ReadByte();//剩余未结束队友人数
        byte roleLookAtMark = _ms.ReadByte();//1(上联玩家)2(下联玩家)
        byte roleLookAtSit = _ms.ReadByte();//队友座位号

        List<byte> pokerList = null;
        byte pokerNum = _ms.ReadByte();//牌数
        if (pokerNum > 0)
        {
            pokerList = new List<byte>();
        }

        for (int index = 0; index < pokerNum; ++index)
        {
            pokerList.Add(_ms.ReadByte());
        }

        UpdateFriendPoker(roleFriendNum, roleLookAtMark, roleLookAtSit, pokerList);
        DebugLog.Log("剩余未结束队友人数:" + roleFriendNum + "队友座位号:" + m_nLookAtRoleSit);
        return true;
    }

    /// <summary>
    /// 聊天
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandEmotionMsg(uint _msgType, UMessage _ms)
    {
        byte sign = _ms.ReadByte(); //聊天内容索引
        byte sit = _ms.ReadByte();  //发送聊天玩家座位号
        string name = _ms.ReadString();//发送聊天玩家名称

        GouJi_Role player = m_PlayerList.Find(role => role.m_nSSit == sit);
        if (player != null)
        {
            player.RefreshEmotionPanel(sign);
        }
        return true;
    }

    /// <summary>
    /// 离开房间的人
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandleBackLevel(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        if (state > 0)
        {
            byte sit = _ms.ReadByte();
            uint userid = _ms.ReadUInt();
            string name = _ms.ReadString();

            if (userid == GameMain.hall_.GetPlayerId())
            {
                BackToGameRoom();
            }
        }
        else
        {
            CCustomDialog.OpenCustomConfirmUI(2305);
        }
        return true;
    }

    /// <summary>
    /// 比赛旁观
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandleBystanderEnter(uint _msgType, UMessage _ms)
    {
        uint curRoomId = _ms.ReadUInt();//房间ID
        byte curRoomLevel = _ms.ReadByte();//房间等级

        ResetGameData(false);

        EnterGameRoom(curRoomLevel,true);
        InitGameRoomInfo(_ms);//游戏房间规则

        uint curContestId = _ms.ReadUInt();//比赛ID

        sbyte recentlyOutPokerSit = _ms.ReadSByte();//上一次成功出牌的人座位号
        sbyte curTimePanelRoleSit = _ms.ReadSByte();//当前出牌人座位号
        byte curBankerSit = _ms.ReadByte();//庄家座位号
        sbyte curGeMingSit = _ms.ReadSByte();//革命人的座位号-1:没有
        sbyte curQuan3Hu = _ms.ReadSByte();//圈三户 -1:没有 0:024圈 1:135圈
        byte b4HuLuanChanState = _ms.ReadByte();//是否是四户乱缠 0:不是 1:是
        byte rangPaiSit = _ms.ReadByte();//让牌人座位号
        float curRoomTime = _ms.ReadSingle();//此房间状态下剩余时间

        byte pokerNum = _ms.ReadByte(); //最近出牌人的数据
        for (int index = 0; index < pokerNum; ++index)
        {
            m_RecentlyOutPokerList.Add(_ms.ReadByte());
        }

        byte bystanderPokerNum = 0;
        List<int> men3RoleList = new List<int>();
        XuanDianType_Enum[] xuanDianList = { 0, 0, 0, 0, 0, 0 };
        byte roleNum = _ms.ReadByte();//房间人数
        for(int index =0; index < roleNum; ++index)
        {
            byte roleServetSit = _ms.ReadByte();
            byte rolPokerNum = InitRoleBasicData(roleServetSit, roleServetSit,_ms,ref xuanDianList,ref men3RoleList);
            if(roleServetSit == 0)
            {
                bystanderPokerNum = rolPokerNum;
            }
        }

        if(bystanderPokerNum > 0)
        {
            List<byte> hvaePokerList = new List<byte>();
            for(int index = 0; index < bystanderPokerNum; ++index)
            {
                hvaePokerList.Add(RoomInfo.NoSit);
            }
            m_PlayerList[0].AddHavePokerObject(hvaePokerList);
        }

        m_nCurGameRound = _ms.ReadByte();//当前第几场
        byte maxRound = _ms.ReadByte();//这一轮有多少场

        if (GameMode == GameTye_Enum.GameType_Contest)
            MatchInGame.GetInstance().ShowBegin(true, (byte)m_nCurGameRound, maxRound);

        RefreshShoPaiStateInfoPanel(_ms);

        RefreshAllRoleInfoPanel(recentlyOutPokerSit, curTimePanelRoleSit, curGeMingSit, xuanDianList, men3RoleList);

        m_eRoomState = (RoomState_Enum)_ms.ReadByte();//房间状态
        switch(m_eRoomState)
        {
            case RoomState_Enum.RoomState_DealPoker:
            case RoomState_Enum.RoomState_Mai3:
            case RoomState_Enum.RoomState_Mai4:
            case RoomState_Enum.RoomState_QuanGong:
            case RoomState_Enum.RoomState_DianGong:
            case RoomState_Enum.RoomState_ShaoGong:
            case RoomState_Enum.RoomState_MenGong:
            case RoomState_Enum.RoomState_LaGong:
            case RoomState_Enum.RoomState_KangGong:
                m_CurCoroutinesDictionary.Add(m_eRoomState, 0);
                break;
            case RoomState_Enum.RoomState_ShaoPai:
                UpdateShaoPaiStateData(_ms, true);
                break;
        }
        return true;
    }

    /// <summary>
    /// 断线重连
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandMiddleEnterRoomMsg(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();//中途加入成功或失败
        if(state != 1)
        {
            return false;
        }
        //玩家状态
        GouJiPlayerState_Enum playerState = (GouJiPlayerState_Enum)_ms.ReadByte();

        if (playerState == GouJiPlayerState_Enum.GouJiPlayerState_OnGameButIsOver)
        {            CCustomDialog.OpenCustomConfirmUI(1621, (param) => { GameOver(true); BackToGameRoom(); });
            return false;
        }

        if (playerState == GouJiPlayerState_Enum.GouJiPlayerState_ReadyHall)
        {            GameOver(true);
            return false;
        }

        ResetGameData(false);

        uint roomId = _ms.ReadUInt();//房间ID
        byte roomLevel = _ms.ReadByte();//房间等级
        
        //游戏房间规则
        InitGameRoomInfo(_ms);
        GameMain.hall_.CurRoomIndex = roomLevel;
        Dictionary<byte, AppointmentRecordPlayer> recordPalyerDictionary = new Dictionary<byte, AppointmentRecordPlayer>();

        PlayerData playerData = GameMain.hall_.GetPlayerData();        m_PlayerList[0].m_nTotalCoin = _ms.ReadLong();//带入的钻石
        m_PlayerList[0].m_nSSit = _ms.ReadByte();//座位号
        m_PlayerList[0].m_sRoleName = playerData.GetPlayerName();        m_PlayerList[0].m_nUseId = playerData.GetPlayerID();        m_PlayerList[0].m_nFaceId = (int)playerData.PlayerIconId;        m_PlayerList[0].m_sUrl = playerData.GetPlayerIconURL();        m_PlayerList[0].m_nSex = playerData.PlayerSexSign;        m_PlayerList[0].m_fMasterScore = playerData.MasterScoreKindArray[(int)GameType];
        m_PlayerList[0].m_nDisconnectState = 1;
        m_PlayerList[0].RefreshInfoUI();

        m_PlayerList[0].m_nReady = _ms.ReadByte();//是否准备1:准备0:否
        byte truste = _ms.ReadByte();//托管标记
        SetTrustButtonActiveState(truste == 1);
        m_PlayerList[0].m_nRank = _ms.ReadSByte();//名次
        m_PlayerList[0].RefreshRoleRankUI(m_PlayerList[0].m_nRank);

        AppointmentRecordPlayer recordPalyerInfo = new AppointmentRecordPlayer();
        m_PlayerList[0].GetRecordPlyerInfo(ref recordPalyerInfo);
        recordPalyerDictionary.Add(m_PlayerList[0].m_nSSit, recordPalyerInfo);

         XuanDianType_Enum[] xuanDianList = { 0,0,0,0,0,0};
        YaoTouType_Enum yaoTouType = (YaoTouType_Enum)_ms.ReadByte();
        xuanDianList[0] = ((XuanDianType_Enum)_ms.ReadByte());
        m_PlayerList[0].m_eKaiDianType = (KaiDianType_Enum)_ms.ReadByte();

        List<int> men3RoleList = new List<int>();
        sbyte men3RoleSit = _ms.ReadSByte();//闷三状态 -1:没有被闷
        if(men3RoleSit >= 0)
        {
            men3RoleList.Add(men3RoleSit * 100 + m_PlayerList[0].m_nSSit);
        }
        //要头图标/无头图标
        if (yaoTouType != YaoTouType_Enum.eYTT_None)
        {
            m_PlayerList[0].ShowGeMingYaotouInfoUI(true, yaoTouType);
        }

        sbyte recentlyOutPokerSit = _ms.ReadSByte();//上一次成功出牌的人座位号
        sbyte curTimePanelRoleSit = _ms.ReadSByte();//当前出牌人座位号
        byte curBankerSit = _ms.ReadByte();//庄家座位号
        sbyte curGeMingSit = _ms.ReadSByte();//革命人的座位号-1:没有
        sbyte curQuan3Hu = _ms.ReadSByte();//圈三户 -1:没有 0:024圈 1:135圈
        byte b4HuLuanChanState = _ms.ReadByte();//是否是四户乱缠 0:不是 1:是
        byte rangPaiSit = _ms.ReadByte();//让牌人座位号


        byte pokerNum = _ms.ReadByte(); //最近出牌人的数据
        for(int index = 0; index < pokerNum; ++index)
        {
            m_RecentlyOutPokerList.Add(_ms.ReadByte());
        }

        byte otherRoleNum = _ms.ReadByte(); // 其他人
        for(int index = 0; index < otherRoleNum;++index)
        {
            byte roleServetSit = _ms.ReadByte();
            byte roleClientSit = GetClientRoleSit(roleServetSit, m_PlayerList[0].m_nSSit);
            InitRoleBasicData(roleClientSit, roleServetSit, _ms, ref xuanDianList, ref men3RoleList);

            recordPalyerInfo = new AppointmentRecordPlayer();
            m_PlayerList[roleClientSit].GetRecordPlyerInfo(ref recordPalyerInfo);
            recordPalyerDictionary.Add(m_PlayerList[roleClientSit].m_nSSit, recordPalyerInfo);
        }

        RefreshAllRoleInfoPanel(recentlyOutPokerSit, curTimePanelRoleSit, curGeMingSit, xuanDianList, men3RoleList);

        m_nCurGameRound = _ms.ReadByte();//当前第几场
        byte maxRound = _ms.ReadByte();//这一轮有多少场
        _ms.ReadUInt();//比赛序号
        uint appointId = _ms.ReadUInt();//约据序号

        if (appointId > 0)
        {
            GoujiAppointmentData data = (GoujiAppointmentData)GameFunction.CreateAppointmentData(GetGameType());
            data.roomid = appointId;
            data.playtimes = maxRound;
            data.maxpower = 250;
            data.wanFa = (byte)m_nGouJiRule;
            AppointmentDataManager.AppointmentDataInstance().currentRoomID = appointId;
            AppointmentDataManager.AppointmentDataInstance().AddAppointmentData(appointId, data);

            GameMain.hall_.InitRoomsData();
            UpdateAppointentRuleInfoText();
            UpdateAppointmentRuleText(m_nCurGameRound);
        }

        List<byte> havePokerList =  new List<byte>();
        float time = _ms.ReadSingle();//此房间状态下剩余时间
        byte selfPokerNum = _ms.ReadByte();//自己手上牌数量
        if (selfPokerNum > 0)
        {
            for(int index = 0; index< selfPokerNum; ++index)
            {
                havePokerList.Add(_ms.ReadByte());
            }
            m_PlayerList[0].RefreshRemainPokerPromptUI(selfPokerNum < 10);
            m_PlayerList[0].AddHavePokerObject(havePokerList);
        }
        else
        {
            sbyte lookFriendRoleSit = _ms.ReadSByte();//正在观看队友位置(-1:表示自己没打完)
            if(lookFriendRoleSit >= 0)
            {
                byte roleNum = _ms.ReadByte();//剩余队友数
                byte roleMark = _ms.ReadByte();//1:上联2:下联
                byte rolePokerNum = _ms.ReadByte();//队友牌数
                for(int index = 0; index < rolePokerNum; ++index)
                {
                    havePokerList.Add(_ms.ReadByte());
                }
                UpdateFriendPoker(roleNum, roleMark, (byte)lookFriendRoleSit, havePokerList);
            }
            else
            {
                _ms.ReadByte();
            }
        }
        RefreshShoPaiStateInfoPanel(_ms);
        m_eRoomState = (RoomState_Enum)_ms.ReadByte();//房间状态
        switch(m_eRoomState)
        {
            case RoomState_Enum.RoomState_DealPoker:
            case RoomState_Enum.RoomState_Mai3:
            case RoomState_Enum.RoomState_Mai4:
            case RoomState_Enum.RoomState_QuanGong:
            case RoomState_Enum.RoomState_DianGong:
            case RoomState_Enum.RoomState_ShaoGong:
            case RoomState_Enum.RoomState_MenGong:
            case RoomState_Enum.RoomState_LaGong:
            case RoomState_Enum.RoomState_KangGong:
                m_CurCoroutinesDictionary.Add(m_eRoomState,0);
                break;
            case RoomState_Enum.RoomState_GeMing:
            case RoomState_Enum.RoomState_YaoTou:
            case RoomState_Enum.RoomState_XuanDian:
            case RoomState_Enum.RoomState_AskShaoPai:
            case RoomState_Enum.RoomState_WaitPlayerDeal:
                MiddleEnterAskdelEvent(_ms,time,recentlyOutPokerSit);
                break;
            case RoomState_Enum.RoomState_ShaoPai:
                UpdateShaoPaiStateData(_ms,true);
                MiddleEnterAskdelEvent(_ms, time, recentlyOutPokerSit);
                break;
            case RoomState_Enum.RoomState_OnceEnd:
                HandGameResultMsg(_msgType,_ms);
                break;
        }

        if (GameMode == GameTye_Enum.GameType_Normal)
        {
            if (playerState == GouJiPlayerState_Enum.GouJiPlayerState_OnDesk)
            {
                MatchRoom.GetInstance().ShowTable(true, recordPalyerDictionary, roomLevel, roomId);
            }
            else
            {
                MatchRoom.GetInstance().StartGame();
            }
        }

        return true;
    }

    /// <summary>
    /// 进入托管状态
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandleEnterTrustStateMsg(uint _msgType, UMessage _ms)
    {
        SetTrustButtonActiveState(true);
        return true;
    }

    /// <summary>
    /// 断线重连提问事件
    /// </summary>
    /// <param name="_ms">消息对象</param>
    /// <param name="time">提问时间</param>
    /// <param name="roleSit"> 牌权座位号</param>
    void MiddleEnterAskdelEvent(UMessage _ms,float time,sbyte roleSit)
    {
        ushort askBit = _ms.ReaduShort();
        if (askBit != 0)
        {
            uint askSignId = _ms.ReadUInt();
            AskdelEvent(askBit, askSignId, time, roleSit);
        }
        else
        {
            if (m_nCurTimePanelRoleSit < m_PlayerList.Count && m_nCurTimePanelRoleSit >= 0)
            {
                if (!m_PlayerList[m_nCurTimePanelRoleSit].m_bBeiThreeState)
                {
                    m_PlayerList[m_nCurTimePanelRoleSit].ShowCountdownPanel(true, time);
                }
            }
        }
    }

    /// <summary>
    /// 更新闷人徽章图标
    /// </summary>
    /// <param name="menRoleSit">闷人的座位号</param>
    /// <param name="beiMenRoleSit">被闷的座位号</param>
    /// <param name="beiMenRank">被闷的名次</param>
    /// <param name="reconnected">是否断线重连</param>
    /// <param name="reverseState">false(正播)true(倒播)</param>
    void UpdateBieThreeBadge(byte menRoleSit, byte beiMenRoleSit, sbyte beiMenRank = -2, bool reconnected = false,bool reverseState = false)
    {
        GouJi_Role player = m_PlayerList.Find(role => role.m_nSSit == beiMenRoleSit);
        if (player != null)
        {
            if (!reconnected && !reverseState)
            {
                player.AddRoleAnimation("anime_men0");
                PlayerGameAudio(player.m_nCSit, 10);
            }
            if(beiMenRank != -2)
            {
                player.RefreshRoleRankUI(beiMenRank);
            }
            player.RefreshBadgeObject(RoleBadge_Enum.eRB_BeiMen, !reverseState);
            player.m_bBeiThreeState = !reverseState;

        }

        player = m_PlayerList.Find(role => role.m_nSSit == menRoleSit);
        if (player != null)
        {
            if (!reconnected && reverseState)
            {
                player.AddRoleAnimation("anime_men1");
            }
            player.RefreshBadgeObject(RoleBadge_Enum.eRB_MenRen, !reverseState);
        }
    }

    /// <summary>
    /// 更新队友的手牌
    /// </summary>
    /// <param name="roleNum">队友数量</param>
    /// <param name="lookAtMark">队友相对于玩家的名称(1(上联玩家)2(下联玩家))</param>
    /// <param name="roleSit">队友座位号</param>
    /// <param name="pokerList">队友手牌</param>
    void UpdateFriendPoker(byte roleNum, byte lookAtMark, byte roleSit, List<byte> pokerList)
    {
        if (m_PlayerList[0] != null)
        {
            m_PlayerList[0].m_bLookAtRoleState = true;
            m_PlayerList[0].m_HavePokerList.Clear();
            m_PlayerList[0].AddHavePokerObject(pokerList, true);
            ((GouJi_RoleLocal)m_PlayerList[0]).RefreshWatchingFriendUI(roleNum > 1 ? true : false, lookAtMark == 1 ? false : true);
        }
        m_nLookAtRoleSit = roleSit;
    }

    /// <summary>
    /// 更新烧牌状态下数据
    /// </summary>
    /// <param name="_ms">消息对象</param>
    /// <param name="reconnected">是否断线重连</param>
    void UpdateShaoPaiStateData(UMessage _ms, bool reconnected = false)
    {
        RefreshRoleShaoPaiBadgePanel();
        byte fanShaoState = _ms.ReadByte();//是否反烧 0:非反烧1:反烧
        m_nRecentlyBeiShaoPaiSit = _ms.ReadByte(); //被烧牌玩家的座位号
        m_nRecentlyShaoPaiSit = _ms.ReadByte(); //烧牌玩家的座位号

        GouJi_Role role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == m_nRecentlyShaoPaiSit);
        if (role != null)
        {
            if (fanShaoState == 1 && !reconnected)
            {
                role.AddRoleAnimation("anime_GJ_shao_2");
                PlayerGameAudio(role.m_nCSit, 14);
            }

            if (fanShaoState == 0)
            {
                if (!reconnected)
                {
                    role.AddRoleAnimation("anime_GJ_shao_1");
                }
            }
            role.m_bShaoPaiSucceedState = true;
            role.PlayShaoPaiAnimation(true);
        }

        role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == m_nRecentlyBeiShaoPaiSit);
        if (role != null)
        {
            if (!reconnected)
            {
                role.AddRoleAnimation("anime_GJ_shao_0");
            }
            role.PlayShaoPaiAnimation(true);
        }
        DebugLog.Log("否反烧:" + fanShaoState + "被烧玩家:" + m_nRecentlyBeiShaoPaiSit + "烧牌玩家:" + m_nRecentlyShaoPaiSit);
    }

    /// <summary>
    /// 提问事件
    /// </summary>
    /// <param name="askBit">提问标识</param>
    /// <param name="askSign">提问ID</param>
    /// <param name="time">时间</param>
    /// <param name="roleSit">有牌权座位号</param>
    void AskdelEvent(ushort askBit, uint askSign, float time, sbyte roleSit)
    {
        DebugLog.Log("===== 提问:==== " + askBit + " ID: " + askSign + " 时间: " + time + "lastChuPaiRoleSit: " + roleSit);
        if (m_PlayerList.Count > m_nCurTimePanelRoleSit && m_nCurTimePanelRoleSit >= 0)
        {
            m_PlayerList[m_nCurTimePanelRoleSit].ShowCountdownPanel(false);
        }
  
        GouJi_RoleLocal RoleLocal = ((GouJi_RoleLocal)m_PlayerList[0]);
        for (RoleDoing_Enum doing = RoleDoing_Enum.RoleDoing_Init; doing < RoleDoing_Enum.RoleDoing_Count; ++doing)
        {
            RoleLocal.ShowRoleDoingPanel(doing, false);
        }

        for (RoleDoing_Enum doing = RoleDoing_Enum.RoleDoing_Init; doing < RoleDoing_Enum.RoleDoing_Count; ++doing)
        {
            if (GameKind.HasFlag((int)doing, askBit))
            {
                if (doing == RoleDoing_Enum.RoleDoing_ChuPai || doing == RoleDoing_Enum.RoleDoing_ShaoChuPai)
                {
                    if (roleSit == m_PlayerList[0].m_nSSit)
                    {
                        m_RecentlyOutPokerList.Clear();
                        foreach (var role in m_PlayerList)
                        {
                            role.UpdatePromptText(string.Empty);
                            role.DestoryAllOutPokerObject();
                            role.RefreshOutPokerPromptUI(0);
                        }
                    }
                    else
                    {
                        m_PlayerList[0].DestoryAllOutPokerObject();
                        m_PlayerList[0].UpdatePromptText(string.Empty);
                        m_PlayerList[0].RefreshOutPokerPromptUI(0);
                        //RemoveLinePromptPokerData(m_nCurTimePanelRoleSit != m_nRecentlyOutPokerSit ? m_nCurTimePanelRoleSit : m_nRecentlyOutPokerSit, m_PlayerList[0].m_nCSit + 1);
                    }
                }
                if(doing != RoleDoing_Enum.RoleDoing_ShaoPai && doing != RoleDoing_Enum.RoleDoing_FanShao)
                {
                    m_nCurTimePanelRoleSit = m_PlayerList[0].m_nCSit;
                }
                RoleLocal.AskdelEvent(doing, askSign, time, roleSit == RoleLocal.m_nSSit);
                DebugLog.Log("提问: " + askBit + " 枚举: " + doing + " ID: " + askSign + " 时间: " + time + "lastChuPaiRoleSit: " + roleSit);
            }
        }
    }

    /// <summary>
    /// 初始化断线重连和旁观状态下玩家基本信息
    /// </summary>
    /// <param name="cSit">客户端座位号</param>
    /// <param name="sSit">服务端座位号</param>
    /// <param name="_ms">消息对象</param>
    /// <param name="xuanDianData">宣点储数据存对象</param>
    /// <param name="men3Data">闷三数据储存对象</param>
    byte InitRoleBasicData(byte cSit, byte sSit,UMessage _ms, ref XuanDianType_Enum[] xuanDianData, ref List<int> men3Data)
    {
        if (m_PlayerList.Count <= cSit || men3Data == null)
        {
            return 0;
        }

        m_PlayerList[cSit].m_nSSit = sSit;
        m_PlayerList[cSit].m_nUseId = _ms.ReadUInt();
        m_PlayerList[cSit].m_nFaceId = _ms.ReadInt();
        m_PlayerList[cSit].m_sUrl = _ms.ReadString();
        m_PlayerList[cSit].m_nTotalCoin = _ms.ReadLong();
        m_PlayerList[cSit].m_sRoleName = _ms.ReadString();
        byte pokerNum = _ms.ReadByte();
        m_PlayerList[cSit].RefreshRemainPokerPromptUI(pokerNum > 0 && pokerNum < 10);//手上还剩余多少张牌
        m_PlayerList[cSit].m_nDisconnectState = _ms.ReadByte();
        m_PlayerList[cSit].m_fMasterScore = _ms.ReadSingle();
        m_PlayerList[cSit].m_nSex = _ms.ReadByte();
        m_PlayerList[cSit].m_nReady = _ms.ReadByte();//是否准备1:准备0:否
        _ms.ReadByte();//托管标记
        m_PlayerList[cSit].m_nRank = _ms.ReadSByte();//名次

        YaoTouType_Enum yaoTouType = (YaoTouType_Enum)_ms.ReadByte();
        if (yaoTouType != YaoTouType_Enum.eYTT_None)
        {
            m_PlayerList[cSit].ShowGeMingYaotouInfoUI(true, yaoTouType);
        }
        xuanDianData[cSit] = (XuanDianType_Enum)_ms.ReadByte();//宣点类型
        m_PlayerList[cSit].m_eKaiDianType = (KaiDianType_Enum)_ms.ReadByte();//开点类型
        sbyte menRoleSit = _ms.ReadSByte();//闷三状态 -1:没有被闷
        if (menRoleSit >= 0)
        {
            men3Data.Add(menRoleSit * 100 + m_PlayerList[cSit].m_nSSit);
        }
        m_PlayerList[cSit].RefreshRoleRankUI(m_PlayerList[cSit].m_nRank);
        m_PlayerList[cSit].RefreshInfoUI();
        return pokerNum;
    }

    /// <summary>
    /// 刷新断线重连和旁观状态下玩家基本表现
    /// </summary>
    /// <param name="recentlySit">最近一次出牌的座位号</param>
    /// <param name="curSit">当前出牌的座位号</param>
    /// <param name="curGeMingSit">革命的座位号</param>
    /// <param name="xuanDianData">宣点数据</param>
    /// <param name="men3RoleList">闷三数据</param>
    void RefreshAllRoleInfoPanel(sbyte recentlySit, sbyte curSit, sbyte curGeMingSit, XuanDianType_Enum[] xuanDianData, List<int> men3RoleList)
    {
        foreach (var men3Data in men3RoleList)
        {
            UpdateBieThreeBadge((byte)(men3Data / 100), (byte)(men3Data % 100),-2,true);
        }

        //最近出牌玩家的牌
        GouJi_Role player = m_PlayerList.Find(role => role.m_nSSit == recentlySit);
        if (player != null)
        {
            if (m_RecentlyOutPokerList.Count > 0)
            {
                foreach (var cardValue in m_RecentlyOutPokerList)
                {
                    CardData cardData = new CardData();
                    cardData.m_nCardValue = cardValue;
                    player.m_OutPokerList.Add(cardData);
                }
                player.RefreshOutPokerPanel();
                player.RefreshOutPokerPromptUI(m_RecentlyOutPokerList.Count);
                player.m_OutPokerList.Clear();
            }
            m_nRecentlyOutPokerSit = player.m_nCSit;
        }
        player = m_PlayerList.Find(role => role.m_nSSit == curSit);
        if (player != null)
        {
            m_nCurTimePanelRoleSit = player.m_nCSit;
        }

        //革命图标
        player = m_PlayerList.Find(role => role.m_nSSit == curGeMingSit);
        if (player != null)
        {
            player.ShowGeMingYaotouInfoUI(true);
        }

        //徽章图标
        RoleBadge_Enum roleBadge = RoleBadge_Enum.eRB_Max;
        foreach (var role in m_PlayerList)
        {
            roleBadge = RoleBadge_Enum.eRB_Max;
            if (role.m_eKaiDianType == KaiDianType_Enum.eKDT_Fail)
            {
                roleBadge = RoleBadge_Enum.eRB_KaiDian_NO;
            }
            else if (role.m_eKaiDianType == KaiDianType_Enum.eKDT_Success)
            {
                roleBadge = RoleBadge_Enum.eRB_KaiDian;
            }
            role.RefreshBadgeObject(roleBadge);

            if (xuanDianData[role.m_nCSit] == XuanDianType_Enum.eXDT_Natural_Yes || xuanDianData[role.m_nCSit] == XuanDianType_Enum.eXDT_Yes)
            {
                roleBadge = RoleBadge_Enum.eRB_XuanDain;
                byte frontSit = (byte)((role.m_nCSit + 3) % 6);
                if (m_PlayerList.Count > frontSit && frontSit >= 0)
                {
                    if (m_PlayerList[frontSit].m_eKaiDianType == KaiDianType_Enum.eKDT_Success || 
                        role.m_eKaiDianType == KaiDianType_Enum.eKDT_Fail)
                    {
                        roleBadge = RoleBadge_Enum.eRB_XuanDain_Mo;
                    }
                }
                role.RefreshBadgeObject(roleBadge);
            }
        }

        if (m_nCurTimePanelRoleSit != RoomInfo.NoSit && false)
        {
            byte roleSit = m_nRecentlyOutPokerSit;
            while (roleSit != m_nCurTimePanelRoleSit)
            {
                roleSit += 1;

                if (roleSit > 5)
                {
                    roleSit = 0;
                }

                if (roleSit == m_nCurTimePanelRoleSit)
                {
                    break;
                }

                player = m_PlayerList.Find(role => role.m_nSSit == curGeMingSit);
                if (player != null && player.m_nCSit == roleSit)
                {
                    continue;
                }

                if (m_PlayerList.Count > roleSit && roleSit >= 0)
                {
                    m_PlayerList[roleSit].UpdatePromptText("过牌");
                }
            }
        }
    }

    /// <summary>
    /// 刷新断线重连和旁观状态下烧牌徽章
    /// </summary>
    /// <param name="_ms">消息对象</param>
    void RefreshShoPaiStateInfoPanel(UMessage _ms)
    {
        if (_ms == null)
        {
            return;
        }
        GouJi_Role player = null;
        byte shaoPaiNum = _ms.ReadByte();//烧牌记录数
        for (int index = 0; index < shaoPaiNum; ++index)
        {
            byte beiShaoSit = _ms.ReadByte();//被烧的人
            byte shaoSit = _ms.ReadByte();//烧牌的人
            player = m_PlayerList.Find(role => role.m_nSSit == beiShaoSit);
            if (player != null)
            {
                player.RefreshBadgeObject(RoleBadge_Enum.eRB_BeiShao);
            }

            player = m_PlayerList.Find(role => role.m_nSSit == shaoSit);
            if (player != null)
            {
                player.m_bShaoPaiSucceedState = true;
                player.RefreshBadgeObject(RoleBadge_Enum.eRB_ShaoPai);
            }
        }
    }

    /// <summary>
    /// 录像-发牌逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionDealPokerLogic(VideoAction videoAction, bool reverseState)
    {
        if(videoAction == null)
        {
            return;
        }
        int index = 0;
        if (reverseState)
        {
            GameOver(false);
        }
        else
        {
            int outSit = videoAction.list[index++]; //第一个出牌的玩家的座位号
            int roleNum = videoAction.list[index++]; //玩家数量
            GouJi_Role player = null;
            List<byte> pokerList = null;
            for (int sitIndex = 0; sitIndex < roleNum; ++sitIndex)
            {
                int sit = videoAction.list[index++];
                int pokerNum = videoAction.list[index++];
                pokerList = new List<byte>();
                for (int pokerIndex = 0; pokerIndex < pokerNum; ++pokerIndex)
                {
                    pokerList.Add((byte)videoAction.list[index++]);
                }
                player = m_PlayerList.Find(role => role.m_nSSit == sit);
                if (player != null)
                {
                    player.AddHavePokerObject(pokerList);
                }
            }
            UpdateRoomRuleTextData();
            m_CurCoroutinesDictionary.Add(RoomState_Enum.RoomState_DealPoker, 0);
        }
    }

    /// <summary>
    /// 录像-买三买四相关逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionMaiThreeOrFourLogic(VideoAction videoAction, bool reverseState)
    {
        if(videoAction == null)
        {
            return;
        }
        RefreshRoomStateEnumeratorLogic();
        m_CurRoomStateStateData.ResetRoomStateData();
        int index = 0;
        int num = videoAction.list[index++];//交易数
        for (int swapIndex = 0; swapIndex < num; ++swapIndex)
        {
            RoleSwapPokerData roleSwapPokerData = new RoleSwapPokerData();
            roleSwapPokerData.m_nMSit = (byte)videoAction.list[index++];//买的人座位号
            roleSwapPokerData.m_nMPokerValue = (byte)videoAction.list[index++];//买的牌 0:无效牌
            roleSwapPokerData.m_nTSit = (byte)videoAction.list[index++];//卖的人座位号
            roleSwapPokerData.m_nTPokerValue = (byte)videoAction.list[index++];//卖的牌
            if(reverseState)
            {
                byte pokerValue = roleSwapPokerData.m_nMPokerValue;
                roleSwapPokerData.m_nMPokerValue = roleSwapPokerData.m_nTPokerValue;
                roleSwapPokerData.m_nTPokerValue = pokerValue;
            }
            m_CurRoomStateStateData.m_CurRoleSwapPokerDataList.Add(roleSwapPokerData);
        }

        RoomState_Enum roomState = videoAction.vai == VideoActionInfo_Enum.VideoActionInfo_303 ? RoomState_Enum.RoomState_Mai3 : RoomState_Enum.RoomState_Mai4;
        m_eRoomState = roomState;
        m_CurRoomStateStateData.m_nRoomState = roomState;
        if (!reverseState)
        {
            m_CurCoroutinesDictionary.Add(roomState, 1);
        }
        else
        {
            m_CurCoroutinesDictionary.Remove(roomState);
        }
        m_CurRoomStateStateData.m_bReverseState = reverseState;
        float time = GameVideo.GetInstance().m_bPause ? 0.0f : GameVideo.GetInstance().GetStepTime();
        m_CurRoomStateEnumerator = ShowSwapPoker(m_CurRoomStateStateData, time);
        GameMain.SC(m_CurRoomStateEnumerator);
    }

    /// <summary>
    /// 录像-进贡相关逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionTributeLogic(VideoAction videoAction, bool reverseState)
    {
        if (videoAction == null)
        {
            return;
        }
        RefreshRoomStateEnumeratorLogic();
        m_CurRoomStateStateData.ResetRoomStateData();
        int index = 0;
        byte gongNum = (byte)videoAction.list[index++];//上贡的数量
        for (int tributeIndex = 0; tributeIndex < gongNum; ++tributeIndex)
        {
            RoleTributeData tributeData = new RoleTributeData();
            tributeData.m_nTributeOutSit = (byte)videoAction.list[index++]; //提供贡牌的人座位号
            tributeData.m_nTributeInSit = (byte)videoAction.list[index++]; //接收贡牌给的人座位号
            byte pokerNum = (byte)videoAction.list[index++];//贡牌数量
            for (int pokerIndex = 0; pokerIndex < pokerNum; ++pokerIndex)
            {
                sbyte pokerValue = (sbyte)videoAction.list[index++];//贡牌(0:无牌可供|-1:联邦抗贡|-2:个人抗贡)
                if (pokerValue > 0)
                {
                    tributeData.m_TributePokerList.Add((byte)pokerValue);
                }else
                {
                    tributeData.m_nTributeState = pokerValue;
                }
                DebugLog.Log("贡牌值:" + pokerValue);
            }
            if(reverseState)
            {
                byte roleSit = tributeData.m_nTributeOutSit;
                tributeData.m_nTributeOutSit = tributeData.m_nTributeInSit;
                tributeData.m_nTributeInSit = roleSit;
            }
            m_CurRoomStateStateData.m_CurRoleTributeDataList.Add(tributeData);
        }
        m_CurRoomStateStateData.m_bReverseState = reverseState;
        RoomState_Enum roomState = RoomState_Enum.RoomState_Count;
        switch(videoAction.vai)
        {
            case VideoActionInfo_Enum.VideoActionInfo_305://圈贡
                roomState = RoomState_Enum.RoomState_QuanGong;
                break;
            case VideoActionInfo_Enum.VideoActionInfo_306://点贡
                roomState = RoomState_Enum.RoomState_DianGong;
                break;
            case VideoActionInfo_Enum.VideoActionInfo_307://烧贡
                roomState = RoomState_Enum.RoomState_ShaoGong;
                break;
            case VideoActionInfo_Enum.VideoActionInfo_308://闷贡
                roomState = RoomState_Enum.RoomState_MenGong;
                break;
            case VideoActionInfo_Enum.VideoActionInfo_309://落贡
                roomState = RoomState_Enum.RoomState_LaGong;
                break;
        }
        m_eRoomState = roomState;
        m_CurRoomStateStateData.m_nRoomState = roomState;
        m_CurCoroutinesDictionary.Add(roomState, 1);
        float time = GameVideo.GetInstance().m_bPause ? 0.0f : GameVideo.GetInstance().GetStepTime();
        m_CurRoomStateEnumerator = ShowTribute(m_CurRoomStateStateData, time);
        GameMain.SC(m_CurRoomStateEnumerator);
    }

    /// <summary>
    /// 录像-抗贡相关逻辑
    /// </summary>
    void UpdateVideoActionKangGongLogic()
    {
        float time = GameVideo.GetInstance().m_bPause ? 0.01f : GameVideo.GetInstance().GetStepTime();
        GameFunction.PlayUIAnim("anime_GJ_kanggong3", time, m_AnimationTfm, m_GouJiAssetBundle);
    }

    /// <summary>
    /// 录像-革命逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionGeMingLogic(VideoAction videoAction, bool reverseState)
    {
        if (videoAction == null)
        {
            return;
        }
        int index = 0;
        byte roleSit = (byte)videoAction.list[index++];//革命人的座位号
        GouJi_Role roleObject = m_PlayerList.Find(role => { return role.m_nSSit == roleSit;});
        if (roleObject != null)
        {
            if (!reverseState)
            {
                float time = GameVideo.GetInstance().m_bPause ? 0.0f : GameVideo.GetInstance().GetStepTime();
                roleObject.AnswerDoingEvent(RoleDoing_Enum.RoleDoing_GeMing,true,230, time);
                roleObject.RefreshRoleRankUI((sbyte)videoAction.list[index++]);
                roleObject.m_eYaoTouType = YaoTouType_Enum.eYTT_GeMing;
            }
            else
            {
                roleObject.ShowGeMingYaotouInfoUI(false);
                roleObject.m_nRank = -1;
                roleObject.RefreshRoleRankUI(roleObject.m_nRank);
                roleObject.m_eYaoTouType = YaoTouType_Enum.eYTT_None;
            }
        }
    }

    /// <summary>
    /// 录像-要头逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionYaoTouLogic(VideoAction videoAction, bool reverseState)
    {
        if (videoAction == null)
        {
            return;
        }
        int index = 0;
        byte roleSit = (byte)videoAction.list[index++];//要头座位号
        YaoTouType_Enum doingType = (YaoTouType_Enum)videoAction.list[index++];//要头选择

        GouJi_Role roleObject = m_PlayerList.Find(role => { return role.m_nSSit == roleSit; });
        if (roleObject != null)
        {
            float time = GameVideo.GetInstance().m_bPause ? 0.0f : GameVideo.GetInstance().GetStepTime();
            if (reverseState)
            {
                if(doingType == YaoTouType_Enum.eYTT_WuTou)
                {
                    byte frontSit = (byte)((roleSit + 3) % 6);
                    GouJi_Role frontRoleObject = m_PlayerList.Find(role => { return role.m_nSSit == frontSit; });
                    if(frontRoleObject.m_eYaoTouType == YaoTouType_Enum.eYTT_GeMing)
                    {
                        doingType = YaoTouType_Enum.eYTT_YaoTou;
                    }
                    else{
                        doingType = YaoTouType_Enum.eYTT_None;
                    }
                }
            }

            switch (doingType)
            {
                case YaoTouType_Enum.eYTT_YaoTou:
                    roleObject.AnswerDoingEvent(RoleDoing_Enum.DRoleDoing_YaoTou, true, 0, time);
                    break;
                case YaoTouType_Enum.eYTT_WuTou:
                    roleObject.ShowGeMingYaotouInfoUI(true,YaoTouType_Enum.eYTT_WuTou);
                    break;
                case YaoTouType_Enum.eYTT_None:
                    roleObject.ShowGeMingYaotouInfoUI(false);
                    break;
            }
            roleObject.m_eYaoTouType = doingType;
        }
    }

    /// <summary>
    /// 录像-宣点逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionXuanDianLogic(VideoAction videoAction, bool reverseState)
    {
        if (videoAction == null)
        {
            return;
        }
        int index = 0;
        byte roleNum = (byte)videoAction.list[index++];
        for (int roleIndex = 0; roleIndex < roleNum; ++roleIndex)
        {
            byte roleSit = (byte)videoAction.list[index++];
            byte xuanDianType = (byte)videoAction.list[index++];
            GouJi_Role roleObject = m_PlayerList.Find(role => { return role.m_nSSit == roleSit; });
            if (roleObject != null)
            {
                if(!reverseState)
                {
                    float time = GameVideo.GetInstance().m_bPause ? 0.001f : GameVideo.GetInstance().GetStepTime()*0.5f;
                    GameFunction.PlayUIAnim("anime_GJ_dianXuanDeng", time, m_AnimationTfm, m_GouJiAssetBundle);
                    roleObject.AnswerDoingEvent(RoleDoing_Enum.RoleDoing_XuanDian, true, xuanDianType, time);
                }
                else
                {
                    roleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain,false);
                }
            }
        }
    }

    /// <summary>
    /// 录像-系统出牌逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActonPlayerDealLogic(VideoAction videoAction, bool reverseState)
    {
        if(videoAction == null)
        {
            return;
        }

        if (reverseState && m_PlayerList.Count > m_nCurTimePanelRoleSit && m_nCurTimePanelRoleSit >= 0)
        {
            m_PlayerList[m_nCurTimePanelRoleSit].ShowCountdownPanel(false);
            m_PlayerList[m_nCurTimePanelRoleSit].ShowRoleDoingPanel(false);
        }

        int index = 0;
        byte shaoPaiState = (byte)videoAction.list[index++]; //烧牌状态 0:无 1：烧牌成功 2：解烧成功
        byte shaoPaiSit = 250, beiJieShaoSit = 250;
        if (shaoPaiState > 0)
        {
            beiJieShaoSit = (byte)videoAction.list[index++];//被烧牌的玩家
            shaoPaiSit = (byte)videoAction.list[index++];//烧牌的玩家|(解烧成功的玩家)
            GouJi_Role role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == shaoPaiSit);
            switch(shaoPaiState)
            {
                case 1:
                    {
                        if (role != null)
                        {
                            role.PlayShaoPaiAnimation(reverseState);
                            role.RefreshBadgeObject(RoleBadge_Enum.eRB_ShaoPai, !reverseState);
                        }
                        role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == beiJieShaoSit);
                        if (role != null)
                        {
                            role.PlayShaoPaiAnimation(reverseState);
                            role.RefreshBadgeObject(RoleBadge_Enum.eRB_BeiShao, !reverseState);
                        }
                    }
                    break;
                case 2:
                    {
                        if (role != null && !reverseState)
                        {
                            role.AddRoleAnimation("anime_GJ_shao_3");
                        }

                        RefreshRoleShaoPaiBadgePanel(reverseState);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 录像-烧牌逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActonShaoPaiLogic(VideoAction videoAction, bool reverseState)
    {
        if(videoAction == null)
        {
            return;
        }
        int index = 0;
        RefreshRoleShaoPaiBadgePanel();
        byte fanShaoState = (byte)videoAction.list[index++];//是否反烧 0:非反烧1:反烧
        m_nRecentlyBeiShaoPaiSit = (byte)videoAction.list[index++]; //被烧牌玩家的座位号
        m_nRecentlyShaoPaiSit = (byte)videoAction.list[index++]; //烧牌玩家的座位号

        GouJi_Role role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == m_nRecentlyShaoPaiSit);
        if (role != null)
        {
            if(!reverseState)
            {
                if (fanShaoState == 1)
                {
                    role.AddRoleAnimation("anime_GJ_shao_2");
                    PlayerGameAudio(role.m_nCSit, 14);
                }

                if (fanShaoState == 0)
                {
                    role.AddRoleAnimation("anime_GJ_shao_1");
                }
            }

            role.PlayShaoPaiAnimation(!reverseState);
        }

        role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == m_nRecentlyBeiShaoPaiSit);
        if (role != null)
        {
            if (!reverseState)
            {
                role.AddRoleAnimation("anime_GJ_shao_0");
            }
            role.PlayShaoPaiAnimation(!reverseState);
        }
    }

    /// <summary>
    /// 录像-出牌提问逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionAskdelLogic(VideoAction videoAction, bool reverseState)
    {
        if(videoAction == null)
        {
            return;
        }
        int index = 0;
        byte roleSit = (byte)videoAction.list[index++];//出牌玩家座位号
        byte beforeOutPokerSit = (byte)videoAction.list[index++];//是否有牌权(座位号)

        GouJi_Role roleObject = m_PlayerList.Find(playerPoker => playerPoker.m_nSSit == roleSit);
        if (roleObject != null)
        {
            //清除已经有名次玩家的出牌对象
            if (beforeOutPokerSit == roleSit)//有牌权
            {
                m_RecentlyOutPokerList.Clear();
                foreach (var role in m_PlayerList)
                {
                    role.UpdatePromptText(string.Empty);
                    role.DestoryAllOutPokerObject();
                    role.RefreshOutPokerPromptUI(0);
                }
            }
            else
            {
                if(!reverseState)
                {
                    roleObject.UpdatePromptText(string.Empty);
                    roleObject.DestoryAllOutPokerObject();
                    roleObject.RefreshOutPokerPromptUI(0);
                }
                //RemoveLinePromptPokerData(m_nCurTimePanelRoleSit != m_nRecentlyOutPokerSit ? m_nCurTimePanelRoleSit : m_nRecentlyOutPokerSit, roleObject.m_nCSit + 1);
            }

            m_nCurTimePanelRoleSit = roleObject.m_nCSit;
            if (!roleObject.m_bBeiThreeState && m_curVideoActionInfoState != videoAction.vai)
            {
                roleObject.ShowCountdownPanel(true, 15);
                roleObject.ShowRoleDoingPanel(true);
            }
        }
    }

    /// <summary>
    /// 录像-出牌逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionOutPokerLogic(VideoAction videoAction, bool reverseState)
    {
        if(videoAction == null)
        {
            return;
        }
        int index = 0;

        byte roleSit = (byte)videoAction.list[index++]; //出牌玩家座位号
        byte lastRoleSit = (byte)videoAction.list[index++];      //上一次成功出牌的玩家座位号
        byte bIsFirst = (byte)videoAction.list[index++];         //是否是第一次出牌1:是0:不是
        byte rolePokerNum = (byte)videoAction.list[index++];     //剩余牌张数    
        byte gouJiState = (byte)videoAction.list[index++];       //是否是够极牌 1:是 0:不是   

        if (reverseState && m_PlayerList.Count > m_nCurTimePanelRoleSit && m_nCurTimePanelRoleSit >= 0)
        {
            m_PlayerList[m_nCurTimePanelRoleSit].ShowCountdownPanel(false);
            m_PlayerList[m_nCurTimePanelRoleSit].ShowRoleDoingPanel(false);
        }

        GouJi_Role roleObject = m_PlayerList.Find(playerPoker => playerPoker.m_nSSit == roleSit);
        if (roleObject != null)
        {
            if (reverseState)
            {
                m_nCurTimePanelRoleSit = roleObject.m_nCSit;
            }
            roleObject.ResetOutPokerPanel();
            if(!reverseState)
            {
                roleObject.RefreshRemainPokerPromptUI(rolePokerNum < 10);
            }
        }

        byte outPokerCount = (byte)videoAction.list[index++];         //出牌数
        if (outPokerCount > 0)
        {
            if (roleObject != null)
            {
                m_nRecentlyOutPokerSit = roleObject.m_nCSit;
            }
            m_RecentlyOutPokerList.Clear();
        }

        for (byte pokerIndex = 0; pokerIndex < outPokerCount; ++pokerIndex)
        {
            byte pokerValue = (byte)videoAction.list[index++];
            if (roleSit != m_PlayerList[0].m_nSSit || reverseState)
            {
                m_RecentlyOutPokerList.Add(pokerValue);
            }
            if (roleObject != null)
            {
                CardData cardData = new CardData();
                cardData.m_nCardValue = pokerValue;
                roleObject.m_OutPokerList.Add(cardData);
            }
        }

        sbyte ranking = (sbyte)videoAction.list[index++]; //角色排名(0,1,2...),-1:没有名次
        byte specialState = (byte)videoAction.list[index++];//1:二杀一的牌 0:正常牌
        bool siHuLuanChanState = videoAction.list[index++] >= 0 ? true : false;//大于等于0表示四户乱缠

        if(!reverseState)
        {
            float time = GameVideo.GetInstance().m_bPause ? 0.001f : GameVideo.GetInstance().GetStepTime();
            if (m_bSiHuLuanChanState != siHuLuanChanState)
            {
                m_bSiHuLuanChanState = siHuLuanChanState;
                GameFunction.PlayUIAnim("anime_GJ_shengyu4", time, m_AnimationTfm, m_GouJiAssetBundle);
            }
            if (specialState == 1)
            {
                GameFunction.PlayUIAnim("anime_GJ_2sha1", time, m_AnimationTfm, m_GouJiAssetBundle);
                PlayerGameAudio(roleObject.m_nCSit, 15);
            }
        }

        byte menThreeState = (byte)videoAction.list[index++];//大于0表示闷三 自己闷上次出牌的玩家
        if(menThreeState > 0)
        {
            sbyte menThreeRank = (sbyte)videoAction.list[index++];//被闷的人的名次
            UpdateBieThreeBadge(roleSit, lastRoleSit, reverseState ? (sbyte)-1: menThreeRank, false,reverseState);
        }

        if (roleObject != null)
        {
            if (outPokerCount > 0 && !reverseState)
            {
                roleObject.PlayerOutPokerPromptAnimation(gouJiState == 1);

                PlayerGameAudio(roleObject.m_nCSit, gouJiState == 1 ? 9 : 8);
            }

            roleObject.UpdatePromptText(outPokerCount == 0 || outPokerCount == 0 && reverseState && m_curVideoActionInfoState != videoAction.vai ? "过牌" : string.Empty);
            roleObject.RefreshOutPokerPanel();
            if(reverseState)
            {
                roleObject.m_OutPokerList.Clear();
            }
            roleObject.RefreshHavePokerPanel();
            roleObject.RefreshOutPokerPromptUI(outPokerCount);
            roleObject.RefreshDiscardPokerPromptUI(false);
            roleObject.ShowCountdownPanel(false);
            roleObject.ShowRoleDoingPanel(false);
        }

        if (reverseState && m_PlayerList.Count > m_nCurTimePanelRoleSit && m_nCurTimePanelRoleSit >= 0)
        {
            m_PlayerList[m_nCurTimePanelRoleSit].ShowCountdownPanel(false);
            m_PlayerList[m_nCurTimePanelRoleSit].ShowRoleDoingPanel(false);
            m_PlayerList[m_nCurTimePanelRoleSit].RefreshOutPokerPromptUI(0);
            m_PlayerList[m_nCurTimePanelRoleSit].AddHavePokerObject(m_RecentlyOutPokerList);
            m_PlayerList[m_nCurTimePanelRoleSit].RefreshRemainPokerPromptUI(m_PlayerList[m_nCurTimePanelRoleSit].GetHavePokerNum() < 10);
            m_RecentlyOutPokerList.Clear();
        }

        if (ranking >= 0)
        {
            roleObject.RefreshRoleRankUI(reverseState && roleObject.GetHavePokerNum() > 0 ? (sbyte)-1 : ranking);
        }
    }

    /// <summary>
    /// 录像-让牌逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionRangPaiLogic(VideoAction videoAction, bool reverseState)
    {
        if(videoAction == null)
        {
            return;
        }
        int index = 0;
        byte roleSit = (byte)videoAction.list[index++];//让牌玩家座位号
        byte rangPaiState = (byte)videoAction.list[index++];//让牌玩家状态(0:过==1:让牌

        if (reverseState && m_PlayerList.Count > m_nCurTimePanelRoleSit && m_nCurTimePanelRoleSit >= 0)
        {
            m_PlayerList[m_nCurTimePanelRoleSit].ShowCountdownPanel(false);
            m_PlayerList[m_nCurTimePanelRoleSit].ShowRoleDoingPanel(false);
        }

        GouJi_Role role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == roleSit);
        if (role != null)
        {
            if (!reverseState && (rangPaiState == 0 && m_nRecentlyOutPokerSit != role.m_nCSit)|| rangPaiState == 1)
            {
                role.UpdatePromptText(rangPaiState == 1 ? "让牌" : "过牌");
            }
            m_nCurTimePanelRoleSit = role.m_nCSit;
            role.ShowCountdownPanel(false);
            role.ShowRoleDoingPanel(false);
        }

    }

    /// <summary>
    /// 录像-开点逻辑
    /// </summary>
    /// <param name="videoAction">录像数据</param>
    /// <param name="reverseState">false(正序播放)true(倒序播放)</param>
    void UpdateVideoActionKaiDianLogic(VideoAction videoAction, bool reverseState)
    {
        if(videoAction == null)
        {
            return;
        }
        int index = 0;
        byte roleSit = (byte)videoAction.list[index++];//开点玩家座位号
        KaiDianType_Enum kaiDianType = (KaiDianType_Enum)(byte)videoAction.list[index++];//开点状态类型
        XuanDianType_Enum xuanDianType = (XuanDianType_Enum)(byte)videoAction.list[index++];//宣点状态类型
        KaiDianType_Enum dJKaiDianType = (KaiDianType_Enum)(byte)videoAction.list[index++];//对家开点状态类型
        XuanDianType_Enum dJXuanDianType = (XuanDianType_Enum)(byte)videoAction.list[index++];//对家宣点状态类型

        GouJi_Role roleObject = m_PlayerList.Find(role => { return role.m_nSSit == roleSit; });
        if (roleObject != null)
        {
            switch (kaiDianType)
            {
                case KaiDianType_Enum.eKDT_Success:
                    {
                        roleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_KaiDian, !reverseState);
                        if (!reverseState)
                        {
                            if (roleObject.m_eKaiDianType != kaiDianType)
                            {
                                roleObject.AddRoleAnimation("anime_GJ_dian1");
                                PlayerGameAudio(roleObject.m_nCSit, 11);
                            }

                            if ((xuanDianType == XuanDianType_Enum.eXDT_Yes || xuanDianType == XuanDianType_Enum.eXDT_Natural_Yes) &&
                                dJKaiDianType == KaiDianType_Enum.eKDT_Fail)
                            {
                                roleObject.AddRoleAnimation("anime_GJ_dianXuan1");
                            }
                        }
                        else
                        {
                            if (roleObject.m_eKaiDianType == KaiDianType_Enum.eKDT_Success && kaiDianType == KaiDianType_Enum.eKDT_Success)
                            {
                                kaiDianType = KaiDianType_Enum.eKDT_None;
                            }
                        }
                        byte frontSit = (byte)((roleSit + 3) % 6);
                        GouJi_Role frontRoleObject = m_PlayerList.Find(role => { return role.m_nSSit == frontSit; });
                        if (frontRoleObject != null &&(dJXuanDianType == XuanDianType_Enum.eXDT_Yes || dJXuanDianType == XuanDianType_Enum.eXDT_Natural_Yes))
                        {
                            if(reverseState)
                            {
                                frontRoleObject.m_bXuanDianAnimationState = false;
                            }else
                            {
                                if (!frontRoleObject.m_bXuanDianAnimationState)
                                {
                                    frontRoleObject.AddRoleAnimation("anime_GJ_dianXuan0");
                                    frontRoleObject.m_bXuanDianAnimationState = true;
                                }
                            }
                            
                            frontRoleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain, reverseState);
                            frontRoleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain_Mo, !reverseState);
                        }
                    }
                    break;
                case KaiDianType_Enum.eKDT_Fail:
                    {
                        if(reverseState && roleObject.m_eKaiDianType == KaiDianType_Enum.eKDT_Fail && kaiDianType == KaiDianType_Enum.eKDT_Fail)
                        {
                            kaiDianType = KaiDianType_Enum.eKDT_None;
                        }

                        if (roleObject.m_eKaiDianType != kaiDianType)
                        {
                            roleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_KaiDian_NO, !reverseState);
                            if (!reverseState)
                            {
                                roleObject.AddRoleAnimation("anime_GJ_dian0");
                            }

                            byte frontSit = (byte)((roleSit + 3) % 6);
                            GouJi_Role frontRoleObject = m_PlayerList.Find(role => { return role.m_nSSit == frontSit; });
                            if ((dJXuanDianType == XuanDianType_Enum.eXDT_Yes || dJXuanDianType == XuanDianType_Enum.eXDT_Natural_Yes) && frontRoleObject != null)
                            {
                                if(dJKaiDianType == KaiDianType_Enum.eKDT_Success)
                                {
                                    if (!frontRoleObject.m_bXuanDianAnimationState)
                                    {
                                        if (!reverseState)
                                        {
                                            frontRoleObject.AddRoleAnimation("anime_GJ_dianXuan1");
                                        }
                                        frontRoleObject.m_bXuanDianAnimationState = true;
                                    }
                                }
                                else
                                {
                                    if (reverseState)
                                    {
                                        frontRoleObject.m_bXuanDianAnimationState = false;
                                    }
                                }
                            }
                        }

                        if (xuanDianType == XuanDianType_Enum.eXDT_Yes || xuanDianType == XuanDianType_Enum.eXDT_Natural_Yes)
                        {
                            if (!reverseState)
                            {
                                if(!roleObject.m_bXuanDianAnimationState)
                                {
                                    roleObject.AddRoleAnimation("anime_GJ_dianXuan0");
                                    roleObject.m_bXuanDianAnimationState = true;
                                }
                            }else
                            {
                                roleObject.m_bXuanDianAnimationState = false;
                            }
                            roleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain, reverseState);
                            roleObject.RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain_Mo, !reverseState);
                        }
                    }
                    break;
            }
            roleObject.m_eKaiDianType = kaiDianType;
        }
    }

    /// <summary>
    /// 开始播放录像
    /// </summary>
    /// <param name="players"></param>
    public override void SetupVideo(List<AppointmentRecordPlayer> players)
    {
        if (players == null)
            return;

        uint localId = GameMain.hall_.GetPlayerId();
        byte localSit = (byte)players.FindIndex(s => s.playerid == localId);

        byte curSit = 0;
        AppointmentRecordPlayer info = null;
        for (byte sitIndex = 0; sitIndex < players.Count; ++sitIndex)        {            info = players[sitIndex];
            curSit = GetClientRoleSit(sitIndex, localSit);
            m_PlayerList[curSit].m_nUseId = info.playerid;
            m_PlayerList[curSit].m_nSex = info.sex;
            m_PlayerList[curSit].m_nSSit = sitIndex;
            m_PlayerList[curSit].m_nCSit = curSit;
            m_PlayerList[curSit].m_nFaceId = info.faceid;
            m_PlayerList[curSit].m_sRoleName = info.playerName;
            m_PlayerList[curSit].m_sUrl = info.url;
            m_PlayerList[curSit].m_fMasterScore = info.master;
            m_PlayerList[curSit].m_nDisconnectState = 1;
            m_PlayerList[curSit].RefreshInfoUI();        }
    }

    /// <summary>
    /// 录像数据
    /// </summary>
    /// <param name="action">录像数据</param>
    /// <param name="curStep">当前帧</param>
    /// <param name="reverse">false(正序播放)true(倒序播放)</param>
    /// <returns></returns>
    public override bool OnVideoStep(List<VideoAction> action, int curStep, bool reverse = false)
    {
        if (curStep > action.Count || curStep < 0)
            return true;

        VideoAction videoAction = action[curStep];

        DebugLog.Log("GouJi OnVideoStep:" + videoAction.vai + " rev:" + reverse);
        List<int> videoDataList = videoAction.list;
        bool resultState = true;

        m_eRoomState = RoomState_Enum.RoomState_Init;
        switch (videoAction.vai)
        {
            case VideoActionInfo_Enum.VideoActionInfo_301://发牌
                UpdateVideoActionDealPokerLogic(videoAction,reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_302://抗贡
                UpdateVideoActionKangGongLogic();
                break;
            case VideoActionInfo_Enum.VideoActionInfo_303://买3
            case VideoActionInfo_Enum.VideoActionInfo_304://买4
                UpdateVideoActionMaiThreeOrFourLogic(videoAction, reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_305://圈贡
            case VideoActionInfo_Enum.VideoActionInfo_306://点贡
            case VideoActionInfo_Enum.VideoActionInfo_307://烧贡
            case VideoActionInfo_Enum.VideoActionInfo_308://闷贡
            case VideoActionInfo_Enum.VideoActionInfo_309://落贡
                UpdateVideoActionTributeLogic(videoAction, reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_310://革命
                UpdateVideoActionGeMingLogic(videoAction, reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_311://要头
                UpdateVideoActionYaoTouLogic(videoAction, reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_312://宣点
                UpdateVideoActionXuanDianLogic(videoAction,reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_313://系统通知出牌(烧牌结束)
                UpdateVideoActonPlayerDealLogic(videoAction, reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_314://开始烧(反烧)牌
                UpdateVideoActonShaoPaiLogic(videoAction, reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_315://转换出牌人
                UpdateVideoActionAskdelLogic(videoAction, reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_316://玩家出牌
                UpdateVideoActionOutPokerLogic(videoAction,reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_317://让牌
                UpdateVideoActionRangPaiLogic(videoAction,reverse);
                break;
            case VideoActionInfo_Enum.VideoActionInfo_318://开点（宣点成功或失败）
                UpdateVideoActionKaiDianLogic(videoAction, reverse);
                break;
            default:
                resultState = false;
                break;
        }
        m_curVideoActionInfoState = videoAction.vai;
        return resultState;
    }

    /// <summary>
    /// 重新播放录像
    /// </summary>
    public override void OnVideoReplay()
    {
        GameOver(false);
    }

    /// <summary>
    /// 与游戏服务器网络断开连接
    /// </summary>
    public override void OnDisconnect(bool over = true)
    {
        GameOver(!over);
    }

    /// <summary>
    /// 与游戏服务器重连成功
    /// </summary>
    public override void ReconnectSuccess()
    {
        m_bReconnected = true;
    }

    /// <summary>
    /// 同步玩家离线状态
    /// </summary>
    /// <param name="disconnect">离线标志</param>
    /// <param name="userId">离线玩家ID</param>
    /// <param name="sit">离线玩家座位号</param>
    public override void OnPlayerDisOrReconnect(bool disconnect, uint userId, byte sit)
    {
        GouJi_Role roleObject = m_PlayerList.Find(role => { return role.m_nUseId == userId; });
        if (roleObject != null)
        {
            roleObject.m_nDisconnectState = (byte)(disconnect ? 0 : 1);
            roleObject.RefreshRoleOfflineUI();
        }
        else if (GameMode == GameTye_Enum.GameType_Normal)
        {
            MatchRoom.GetInstance().SetPlayerOffline(sit, disconnect);
        }
    }

    /// <summary>
    /// 申请进入游戏
    /// </summary>
    void ReqestChooseLevelToServerMsg()
    {
        if (GameMode == GameTye_Enum.GameType_Appointment)
        {
            return;
        }
        MatchRoom.GetInstance().ShowRoom((byte)GameType);

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_CM_CHOOSElEVEL);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(GameMain.hall_.CurRoomIndex);
        HallMain.SendMsgToRoomSer(msg);
    }

    /// <summary>
    /// 进入游戏房间
    /// </summary>
    /// <param name="roomLevel">房间等级</param>
    /// <param name="bystander">旁观状态</param>
    void EnterGameRoom(byte roomLevel, bool bystander = false)
    {
        DebugLog.Log("进入房间：" + roomLevel  +" " + bystander);
        if (m_MainUITransform)
        {
            m_MainUITransform.gameObject.SetActive(true);
        }

        if (GameMode != GameTye_Enum.GameType_Appointment)
        {
            Bystander = bystander;
            if (m_nCurRoomLevel == roomLevel)
                return;
            m_nCurRoomLevel = roomLevel;
            if(!m_bReconnected)
            {
                if (GameMode == GameTye_Enum.GameType_Contest)
                {
                    MatchInGame.GetInstance().ShowWait();
                }
                else if (GameMode == GameTye_Enum.GameType_Record)
                {
                    GameVideo.GetInstance().ShowBegin();
                }
            }
        } else
        {
            LoadAppointmentResource();
        }

        InitRoleLocalInfo();
    }

    /// <summary>
    /// 退出游戏房间
    /// </summary>
    void BackToGameRoom()    {        if (GameMode == GameTye_Enum.GameType_Appointment)            GameMain.hall_.SwitchToHallScene(true, 0);
        else if (GameMode == GameTye_Enum.GameType_Normal)            MatchRoom.GetInstance().GameOver();
        else
            GameMain.hall_.SwitchToHallScene();
    }

    /// <summary>
    /// 加载游戏资源
    /// </summary>
    bool LoadResource()
    {
        if (m_PlayerList.Count == 0)
        {            if (m_nWaitingLoad > 0)                m_nWaitingLoad--;            if (m_nWaitingLoad != 0)                return false;
        }        else            return true;

        LoadSceneResource();
        InitPlayers();
        EnterGameRoom(GameMain.hall_.CurRoomIndex);
        if (GameMode == GameTye_Enum.GameType_Normal)
        {
            MatchRoom.GetInstance().SetUIAsLast();
        }else if(GameMode == GameTye_Enum.GameType_Appointment)
        {
            GameMain.hall_.gamerooms_.SetUIAsLast();
        }
        return true;
    }

    /// <summary>
    /// 加载游戏场景资源
    /// </summary>
    void LoadSceneResource()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((int)GameKind_Enum.GameKind_GouJi);
        if (gamedata == null)
            return;

        m_GouJiAssetBundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
        if (m_GouJiAssetBundle == null)
            return;

        m_GouJiCommonAssetBundle = AssetBundleManager.GetAssetBundle("pokercommon.resource");        if (m_GouJiCommonAssetBundle == null)            return;

        if (GameCanvas == null)
        {
            GameCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }

        if (m_RootTransform == null)
        {
            m_RootTransform = GameObject.Find("Canvas/Root").transform;
        }

        if (m_MainUITransform == null)
        {
            m_MainUITransform = ((GameObject)GameMain.instantiate(m_GouJiAssetBundle.LoadAsset("GouJi_Game"))).transform;
            m_MainUITransform.SetParent(m_RootTransform, false);
        }
        m_MainUITransform.gameObject.SetActive(false);

        UpdateRoomRuleTextData();

        m_AnimationTfm = m_MainUITransform.Find("Pop-up/Animation/Imagepoint");
        m_TributeTransform = m_MainUITransform.Find("Pop-up/gongInfo/imageBG");
        m_TributeText = m_TributeTransform.Find("Content_record/Text").GetComponent<Text>();
        m_TributeText.text = string.Empty;
        m_TrustButton = m_MainUITransform.Find("Top/Button_Tuoguan").GetComponent<Button>();
        m_TrustButton.onClick.AddListener(() => {
            if(m_TrustButton.interactable)
            {
                UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_CM_CANCLETRUSTSTATE);
                msg.Add(GameMain.hall_.GetPlayerId());
                HallMain.SendMsgToRoomSer(msg);
                SetTrustButtonActiveState(false);
            }
        });
        SetTrustButtonActiveState(false);

        //设置按钮
        Transform expandTransform = m_MainUITransform.Find("Top/ButtonExpand");
        XPointEvent.AutoAddListener(expandTransform.gameObject, OnClickSetButtonEvent, true);

        Transform backgroundTransform = GetGameBackgroundMainUITransform();
        XPointEvent.AutoAddListener(backgroundTransform.gameObject, (EventTriggerType eventtype, object message, PointerEventData eventData) =>
        {
            if (eventtype == EventTriggerType.PointerClick)
            {
                m_PlayerList[0].ResetOutPokerPanel();
                SetTributeInfoPanelActive();
            }
        }, null);

        //设置界面
        if (m_SetMainTransform == null)
        {
            m_SetMainTransform = m_MainUITransform.Find("Pop-up/Set");

            Slider music = m_SetMainTransform.Find("ImageBG/Slider_Music").GetComponent<Slider>();
            Slider sound = m_SetMainTransform.Find("ImageBG/Slider_Sound").GetComponent<Slider>();
            music.value = AudioManager.Instance.MusicVolume;
            sound.value = AudioManager.Instance.SoundVolume;
            music.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.MusicVolume = value; });
            sound.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.SoundVolume = value; });
        }

        XPointEvent.AutoAddListener(m_SetMainTransform.Find("ImageBG/ButtonClose").gameObject, OnClickSetButtonEvent, false);
        m_SetMainTransform.gameObject.SetActive(false);


        //聊天
        if (m_ChatTransform == null)
        {
            m_ChatTransform = ((GameObject)GameMain.instantiate(m_GouJiAssetBundle.LoadAsset("GouJi_Chat"))).transform;
            m_ChatTransform.SetParent(m_RootTransform, false);

            int index = 1;
            Button[] chatButtons = m_ChatTransform.Find("Chat_Viewport/Chat_Content").GetComponentsInChildren<Button>();
            foreach (var chatButton in chatButtons)
            {
                int chatIndex = index;
                chatButton.onClick.AddListener(() => OnClickGouJiChatEvent(chatIndex));
                index++;
            }
        }
        m_ChatTransform.gameObject.SetActive(false);

        //结算
        if (m_ResultMainTransform == null)
        {
            m_ResultMainTransform = ((GameObject)GameMain.instantiate(m_GouJiAssetBundle.LoadAsset("Gouji_Result"))).transform;
            m_ResultMainTransform.SetParent(m_RootTransform, false);
            m_LeaveButton = m_ResultMainTransform.Find("ImageBG/ImageButtonBG/Button_likai").GetComponent<Button>();
            m_LeaveButton.onClick.AddListener(() =>
            {
                MatchRoom.GetInstance().OnClickReturn(0);
            });
            m_EnterButton = m_ResultMainTransform.Find("ImageBG/ImageButtonBG/Button_jixu").GetComponent<Button>();
            m_EnterButton.onClick.AddListener(() =>
            {
                if (GameMode == GameTye_Enum.GameType_Appointment)
                {
                    ShowAppointmentTotalResult();
                }
                else
                {
                    ResetGameData();
                }
                PlayerGameAudio(0, 6);
            });
        }

        //录像
        if(m_RecordTransform == null)
        {
            m_RecordTransform = ((GameObject)GameMain.instantiate(m_GouJiAssetBundle.LoadAsset("Lobby_video_card_GJ"))).transform;
            m_RecordTransform.SetParent(m_RootTransform, false);
        }
        m_RecordTransform.gameObject.SetActive(GameMode == GameTye_Enum.GameType_Record);

        m_ResultMainTransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// 加载约据资源
    /// </summary>
    void LoadAppointmentResource()
    {
        AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (assetBundle == null)
            return;

        if (m_AppointmentReadyPanel == null)
        {
            m_AppointmentReadyPanel = GameMain.hall_.gamerooms_.LoadAppintmentReadResource("Room_Ready_6");
            if (m_AppointmentReadyPanel != null)
            {
                m_AppointmentReadyExitButton = m_AppointmentReadyPanel.transform.Find("Top/Button_Return").GetComponent<Button>();
                m_AppointmentReadyExitButton.interactable = true;
                m_AppointmentReadyExitButton.onClick.AddListener(() => { m_AppointmentReadyExitButton.interactable = false; OnLeaveAppointmentRoomEvent(); });
                m_AppointmentReadyPanel.transform.SetAsLastSibling();
            }
        }

        if (m_AppointmentRulePanel == null && m_RootTransform != null)
        {
            m_AppointmentRulePanel = (GameObject)GameMain.instantiate(assetBundle.LoadAsset("Room_process"));
            m_AppointmentRulePanel.transform.SetParent(m_RootTransform, false);
            m_AppointmentGameExitButton = m_AppointmentRulePanel.transform.Find("Top/ButtonReturn").GetComponent<Button>();
            m_AppointmentGameExitButton.interactable = true;
            m_AppointmentGameExitButton.onClick.AddListener(() => { m_AppointmentGameExitButton.interactable = false; OnLeaveAppointmentRoomEvent(); });

            UpdateAppointentRuleInfoText();
            XPointEvent.AutoAddListener(m_AppointmentRulePanel.transform.Find("ImageLeftBG").gameObject, OnClickPromptInofPanel, null);
        }

        if (m_AppointmentResultPanel == null && m_RootTransform != null)
        {
            m_AppointmentResultPanel = (GameObject)GameMain.instantiate(assetBundle.LoadAsset("Room_Result_4"));
            m_AppointmentResultPanel.transform.SetParent(m_RootTransform, false);
            m_AppointmentResultPanel.SetActive(false);

            XPointEvent.AutoAddListener(m_AppointmentResultPanel.transform.Find("ImageBG/Buttonclose").gameObject, OnCloseAppointmentResultPanel, null);
        }

        //if(m_AppointmentReadyPanel != null)
        //{
        //    m_AppointmentReadyPanel.transform.SetAsLastSibling();
        //}
    }

    /// <summary>
    /// 更新约据游戏规则详情信息
    /// </summary>
    void UpdateAppointentRuleInfoText()
    {
        if(m_AppointmentRulePanel == null)
        {
            return;
        }
        Text RuleText = m_AppointmentRulePanel.transform.Find("ImageLeftBG/RuleInfo_BG/Image_info/Text").GetComponent<Text>();
        if (string.IsNullOrEmpty(RuleText.text))
        {
            string gameRuleTextData = null;
            GameFunction.GetAppointmentRuleTextData(ref gameRuleTextData, GetGameType());
            RuleText.text = gameRuleTextData;
        }
    }

    /// <summary>
    /// 离开约据房间
    /// </summary>
    void OnLeaveAppointmentRoomEvent()
    {
        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
            return;

        PlayerGameAudio(0, 6);
        bExitAppointmentDialogState = true;
        bool selftState = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().hostid == GameMain.hall_.GetPlayerId();
        CCustomDialog.OpenCustomDialogWithTipsID((uint)(selftState ? 1624 : 1625), OnExitAppointmentGame);
    }

    /// <summary>
    /// 退出约据游戏
    /// </summary>
    /// <param name="call"></param>
    void OnExitAppointmentGame(object call)
    {
        if ((int)call == 0)
        {
            if (m_AppointmentReadyExitButton)
            {
                m_AppointmentReadyExitButton.interactable = true;
            }
            if (m_AppointmentGameExitButton)
            {
                m_AppointmentGameExitButton.interactable = true;
            }
            return;
        }
        bExitAppointmentDialogState = false;
        byte sitNo = AppointmentDataManager.AppointmentDataInstance().playerSitNo;
        uint playerid = GameMain.hall_.GetPlayerId();
        UMessage tickmsg = new UMessage((uint)GameCity.EMSG_ENUM.Appointment_CM_Exit);

        tickmsg.Add(playerid);
        tickmsg.Add(AppointmentDataManager.AppointmentDataInstance().currentRoomID);
        tickmsg.Add((byte)sitNo);
        tickmsg.Add((byte)2);

        NetWorkClient.GetInstance().SendMsg(tickmsg);

        PlayerGameAudio(0, 6);
    }

    /// <summary>
    /// 更新约据房间规则文本数据
    /// </summary>
    /// <param name="curJuShu">当前第几局</param>
    void UpdateAppointmentRuleText(int curJuShu)
    {
        if (GameMode != GameTye_Enum.GameType_Appointment ||
            m_AppointmentRulePanel == null)
        {
            return;
        }

        Text ruleTx = m_AppointmentRulePanel.transform.Find("ImageLeftBG/Text_lunshu").GetComponent<Text>();
        AppointmentData appointmentData = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();
        if (appointmentData == null)
        {
            return;
        }

        if (curJuShu > appointmentData.playtimes)
            curJuShu = appointmentData.playtimes;

        ruleTx.text = curJuShu.ToString() + "/" + appointmentData.playtimes.ToString() + "局";
    }

    /// <summary>
    /// 更新房间规则文本数据
    /// </summary>
    void UpdateRoomRuleTextData()
    {
        if (m_MainUITransform == null)
        {
            return;
        }
        Text ruleText = m_MainUITransform.Find("Middle/PlayerInfor_BG/Text_rule").GetComponent<Text>();
        ruleText.text = GameMode != GameTye_Enum.GameType_Appointment ? GetRoomRuleTextInfo() : string.Empty;
    }

    /// <summary>
    /// 开始加载资源
    /// </summary>
    public override void StartLoad()
    {
        m_nWaitingLoad = 0;
    }

    /// <summary>
    /// 游戏逻辑推进
    /// </summary>
    public override void ProcessTick()    {        base.ProcessTick();

        //游戏断线重连
        if (m_bReconnected)
        {
            GameMain.hall_.OnGameReconnect(GameType, GameMode);
            m_bReconnected = false;
        }        if (!LoadResource())            return;

        CCIMgr.UpdateTimeImage();

        foreach (GouJi_Role Role in m_PlayerList)
        {
            Role.OnTick();
        }

        if (GameMode == GameTye_Enum.GameType_Appointment &&
           IsGameRoomState(RoomState_Enum.RoomState_TotalEnd) &&
           AppointmentDataManager.AppointmentDataInstance().interrupt)
        {
            ShowAppointmentTotalResult();
        }

        UpdateGameAudio();
    }

    /// <summary>
    /// 判断当前房间状态
    /// </summary>
    /// <param name="roomState">房间状态</param>
    /// <returns>true: 是 false: 不是</returns>
    public bool IsGameRoomState(RoomState_Enum roomState)
    {
        return m_eRoomState == roomState;
    }
    
    /// <summary>
    /// 获得房间状态
    /// </summary>
    /// <returns></returns>
    public RoomState_Enum GetRoomState()
    {
        return m_eRoomState;
    }

    /// <summary>
    /// 房间状态切换
    /// </summary>
    /// <param name="state">新的房间状态</param>
    /// <param name="_ms"></param>
    /// <param name="mode"> 0:normal 1:reconnect 2:bystander</param>
    /// <param name="timeLeft"></param>
    void OnStateChange(RoomState_Enum state, UMessage _ms, byte mode = 0, float timeLeft = 0f)    {        if (m_eRoomState != RoomState_Enum.RoomState_WaitReady &&            m_eRoomState == state && mode == 0)            return;        DebugLog.Log(string.Format("room state change: ({0}->{1})", m_eRoomState, state));        OnQuitState(m_eRoomState);        m_eRoomState = state;        OnEnterState(m_eRoomState, _ms, mode, timeLeft);    }

    /// <summary>
    /// 退出房间状态
    /// </summary>
    /// <param name="state">当前房间状态</param>
    void OnQuitState(RoomState_Enum state)    {        switch (state)        {
            case RoomState_Enum.RoomState_Mai3:
            case RoomState_Enum.RoomState_Mai4:
            case RoomState_Enum.RoomState_QuanGong:
            case RoomState_Enum.RoomState_DianGong:
            case RoomState_Enum.RoomState_ShaoGong:
            case RoomState_Enum.RoomState_MenGong:
            case RoomState_Enum.RoomState_LaGong:
                if (m_AnimatonGongObject)
                {
                    GameObject.Destroy(m_AnimatonGongObject);
                }
                break;
            case RoomState_Enum.RoomState_GeMing:
            case RoomState_Enum.RoomState_YaoTou:
                m_PlayerList[0].ShowRoleDoingPanel(state == RoomState_Enum.RoomState_GeMing ? RoleDoing_Enum.RoleDoing_GeMing: RoleDoing_Enum.DRoleDoing_YaoTou, false);
                break;
            case RoomState_Enum.RoomState_AskShaoPai:
                m_PlayerList[0].ShowRoleDoingPanel(RoleDoing_Enum.RoleDoing_ShaoPai, false);
                m_PlayerList[0].ShowRoleDoingPanel(RoleDoing_Enum.RoleDoing_FanShao, false);
                break;
            case RoomState_Enum.RoomState_OnceEnd:
                CustomAudioDataManager.GetInstance().PlayAudio(GetGameAudioID(0,1), false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 刷新烧牌徽章
    /// </summary>
    /// <param name="activeState">true(烧牌表现激活)false(烧牌表现关闭)</param>
    void RefreshRoleShaoPaiBadgePanel(bool activeState = false)
    {
        GouJi_Role role = null;
        if (m_nRecentlyShaoPaiSit != RoomInfo.NoSit)
        {
            role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == m_nRecentlyShaoPaiSit);
            if (role != null)
            {
                role.m_bShaoPaiSucceedState = false;
                role.RefreshBadgeObject(RoleBadge_Enum.eRB_ShaoPai, activeState);
                role.PlayShaoPaiAnimation(activeState);
            }
        }

        if (m_nRecentlyBeiShaoPaiSit != RoomInfo.NoSit)
        {
            role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == m_nRecentlyBeiShaoPaiSit);
            if (role != null)
            {
                role.RefreshBadgeObject(RoleBadge_Enum.eRB_BeiShao, activeState);
                role.PlayShaoPaiAnimation(activeState);
            }
        }
    }

    /// <summary>
    /// 进入房间状态
    /// </summary>
    /// <param name="state">新的房间状态</param>
    /// <param name="_ms"></param>
    /// <param name="mode"> 0:normal 1:reconnect 2:bystander</param>
    /// <param name="timeLeft"></param>    void OnEnterState(RoomState_Enum state, UMessage _ms, byte mode, float timeLeft)    {
        switch (state)
        {
            case RoomState_Enum.RoomState_WaitPlayer://等人
                {
                    if (GameMode == GameTye_Enum.GameType_Normal)
                    {
                        MatchRoom.GetInstance().ShowKickTip(false);
                    }
                }
                break;
            case RoomState_Enum.RoomState_WaitReady://等待准备
                {
                    if (GameMode == GameTye_Enum.GameType_Normal)
                    {
                        float time = _ms.ReadSingle();
                        MatchRoom.GetInstance().ShowKickTip(true, timeLeft);//准备倒计时开启
                    }
                }
                break;
            case RoomState_Enum.RoomState_OnceBeginShow://每局开始前的显示
                {
                    ResetGameData();
                    UpdateAppointmentRuleText(m_nCurGameRound+1);
                    if(GameMode == GameTye_Enum.GameType_Normal)
                    {
                        MatchRoom.GetInstance().StartGame();
                    }else if(GameMode == GameTye_Enum.GameType_Contest)
                    {
                        MatchInGame.GetInstance().ShowBegin(Bystander);
                    }
                }
                break;
            case RoomState_Enum.RoomState_CountDownBegin://游戏开始倒计时
                {
                    GameFunction.PlayUIAnim("Anime_startgame", timeLeft - 1, m_AnimationTfm, m_GouJiCommonAssetBundle);
                    if (GameMode == GameTye_Enum.GameType_Appointment)
                    {
                        m_AppointmentRuleEnumerator = ShowAppointmentRule();
                        GameMain.SC(m_AppointmentRuleEnumerator);
                    }
                }
                break;

            case RoomState_Enum.RoomState_Mai3:
            case RoomState_Enum.RoomState_Mai4:
                {
                    RefreshRoomStateEnumeratorLogic();
                    m_CurRoomStateStateData.ResetRoomStateData();
                    m_CurRoomStateStateData.m_nRoomState = state;
                    byte num = _ms.ReadByte();//交易数
                    for (byte index = 0; index < num; ++index)
                    {
                        RoleSwapPokerData roleSwapPokerData = new RoleSwapPokerData();
                        roleSwapPokerData.m_nMSit = _ms.ReadByte();//买的人座位号
                        roleSwapPokerData.m_nMPokerValue = _ms.ReadByte();//买的牌 0:无效牌
                        roleSwapPokerData.m_nTSit = _ms.ReadByte();//卖的人座位号
                        roleSwapPokerData.m_nTPokerValue = _ms.ReadByte();//卖的牌
                        m_CurRoomStateStateData.m_CurRoleSwapPokerDataList.Add(roleSwapPokerData);

                        DebugLog.Log("买:" + roleSwapPokerData.m_nMSit + " 买牌 " + roleSwapPokerData.m_nMPokerValue + " 卖 " + roleSwapPokerData.m_nTSit + " 卖牌 " + roleSwapPokerData.m_nTPokerValue);
                    }
                    m_CurCoroutinesDictionary.Add(state, 1);
                    m_CurRoomStateEnumerator = ShowSwapPoker(m_CurRoomStateStateData, timeLeft - 1);
                    GameMain.SC(m_CurRoomStateEnumerator);
                }
                break;
            case RoomState_Enum.RoomState_QuanGong:
            case RoomState_Enum.RoomState_DianGong:
            case RoomState_Enum.RoomState_ShaoGong:
            case RoomState_Enum.RoomState_MenGong:
            case RoomState_Enum.RoomState_LaGong:
                {
                    RefreshRoomStateEnumeratorLogic();
                    m_CurRoomStateStateData.ResetRoomStateData();
                    m_CurRoomStateStateData.m_nRoomState = state;
                    byte gongNum = _ms.ReadByte();//上贡的数量
                    for (int index = 0; index < gongNum; ++index)
                    {
                        RoleTributeData tributeData = new RoleTributeData();
                        tributeData.m_nTributeOutSit = _ms.ReadByte(); //提供贡牌的人座位号
                        tributeData.m_nTributeInSit = _ms.ReadByte(); //接收贡牌给的人座位号
                        byte pokerNum = _ms.ReadByte();//贡牌数量
                        for (int pokerIndex = 0; pokerIndex < pokerNum; ++pokerIndex)
                        {
                            sbyte pokerValue = _ms.ReadSByte();//贡牌(0:无牌可供|-1:联邦抗贡|-2:个人抗贡)
                            if (pokerValue > 0)
                            {
                                tributeData.m_TributePokerList.Add((byte)pokerValue);
                            }else 
                            {
                                tributeData.m_nTributeState = pokerValue;
                            }
                            DebugLog.Log("贡牌值:" + pokerValue);
                        }
                        m_CurRoomStateStateData.m_CurRoleTributeDataList.Add(tributeData);

                        DebugLog.Log("提供贡牌:" + tributeData.m_nTributeOutSit + " 接收贡牌 " + tributeData.m_nTributeInSit + " 贡牌数量 " + pokerNum);
                    }
                    m_CurCoroutinesDictionary.Add(state, 1);
                    m_CurRoomStateEnumerator = ShowTribute(m_CurRoomStateStateData, timeLeft - 1);
                    GameMain.SC(m_CurRoomStateEnumerator);
                }
                break;
            case RoomState_Enum.RoomState_KangGong:
                {
                    GameFunction.PlayUIAnim("anime_GJ_kanggong3", timeLeft - 1, m_AnimationTfm, m_GouJiAssetBundle);
                }
                break;
            case RoomState_Enum.RoomState_XuanDian:
                {
                    GameFunction.PlayUIAnim("anime_GJ_dianXuanDeng", timeLeft - 1, m_AnimationTfm, m_GouJiAssetBundle);
                }
                break;
            case RoomState_Enum.RoomState_YaoTou:
                {
                    byte roleSit = _ms.ReadByte();//革命玩家座位号
                    sbyte roleRank = _ms.ReadSByte();//革命玩家名次

                    GouJi_Role role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == roleSit);
                    if (role != null)
                    {
                        role.RefreshRoleRankUI(roleRank);
                        role.AnswerDoingEvent(RoleDoing_Enum.RoleDoing_GeMing,true);
                    }
                }
                break;
            case RoomState_Enum.RoomState_WaitPlayerDeal://发一张牌给玩家并等待他出牌
                {
                    byte shaoPaiState = _ms.ReadByte(); //烧牌状态 0:无 1：烧牌成功 2：解烧成功
                    byte shaoPaiSit = 250, beiJieShaoSit = 250;
                    if (shaoPaiState > 0)
                    {
                        beiJieShaoSit = _ms.ReadByte();//被烧牌的玩家
                        shaoPaiSit = _ms.ReadByte();//烧牌的玩家|(解烧成功的玩家)
                        GouJi_Role role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == shaoPaiSit);
                        if (shaoPaiState == 2)
                        {
                            if (role != null)
                            {
                                role.AddRoleAnimation("anime_GJ_shao_3");
                            }
                            RefreshRoleShaoPaiBadgePanel();
                        } else if (shaoPaiState == 1)
                        {
                            if (role != null)
                            {
                                role.PlayShaoPaiAnimation(false);
                                role.RefreshBadgeObject(RoleBadge_Enum.eRB_ShaoPai);
                            }
                            role = m_PlayerList.Find(roleValue => roleValue.m_nSSit == beiJieShaoSit);
                            if (role != null)
                            {
                                role.PlayShaoPaiAnimation(false);
                                role.RefreshBadgeObject(RoleBadge_Enum.eRB_BeiShao);
                            }
                        }
                    }

                    DebugLog.Log("烧牌状态:" + shaoPaiState + "被烧玩家:" + beiJieShaoSit + "烧牌玩家:" + shaoPaiSit);
                }
                break;
            case RoomState_Enum.RoomState_ShaoPai:
                {
                    UpdateShaoPaiStateData(_ms);
                }
                break;
            case RoomState_Enum.RoomState_OnceEnd://一局结束
                {
                    SetResultButtonInteractable(true);
                }
                break;
            case RoomState_Enum.RoomState_TotalEnd://游戏结束
                {
                    if (GameMode == GameTye_Enum.GameType_Normal)
                    {
                        if(m_ResultMainTransform.gameObject.activeSelf)
                        {
                            m_EnterButton.onClick.RemoveAllListeners();
                            m_EnterButton.onClick.AddListener(() =>
                            {
                                MatchRoom.GetInstance().OnClickReturn(1);
                            });
                        }
                    }
                }
                break;
            default:
                break;
        }

        if(state > RoomState_Enum.RoomState_KangGong)
        {
            RefreshRoomStateEnumeratorLogic();
        }
    }

    /// <summary>
    /// 初始化游戏对象
    /// </summary>
    void InitPlayers()
    {
        GouJi_Role role;
        role = new GouJi_RoleLocal(this, 0);
        m_PlayerList.Add(role);

        for (byte sit = 1; sit < 6; sit++)
        {
            role = new GouJi_RoleOther(this, sit);
            m_PlayerList.Add(role);
        }
    }

    /// <summary>
    /// 获取客户端的座位号
    /// </summary>
    /// <param name="sit">服务端座位号</param>
    /// <param name="localSit">参考座位号</param>
    /// <returns></returns>
    byte GetClientRoleSit(byte sit, byte localSit)
    {
        return (byte)((6 + sit - localSit) % 6);
    }

    /// <summary>
    /// 获得房间玩法规则
    /// </summary>
    /// <returns></returns>
    string GetRoomRuleTextInfo()
    {
        string textInfo = string.Empty;
        string[] wanFaName = { "憋三\n", "开点发4\n", "大王二杀一\n", "", "宣点\n", "进贡\n" };
        for (GouJiRule_Enum index = GouJiRule_Enum.eGJR_BeiShan; index < GouJiRule_Enum.eGJR_Max; ++index)
        {
            if (GameKind.HasFlag((int)index, m_nGouJiRule))
            {
                textInfo += wanFaName[(int)index];
            }
        }
        return textInfo;
    }

    /// <summary>
    /// 获得角色面板对象
    /// </summary>
    /// <param name="roleClientSit">角色客户端座位号</param>
    /// <returns></returns>
    public UnityEngine.Transform GetGameRoleMainUITransform(int roleClientSit)
    {
        if (m_MainUITransform == null)
        {
            return null;
        }
        return m_MainUITransform.Find("Middle/PlayerInfor_BG/Player_" + roleClientSit);
    }

    /// <summary>
    /// 获得角色动画挂接对象
    /// </summary>
    /// <param name="roleClientSit">角色客户端座位号</param>
    /// <returns></returns>
    public UnityEngine.Transform GetGameRoleAnimationTransform(int roleClientSit)
    {
        if (m_MainUITransform == null)
        {
            return null;
        }
        return m_MainUITransform.Find("Pop-up/Animation/Playerpoint_" + roleClientSit);
    }

    /// <summary>
    /// 获得游戏背景面板对象
    /// </summary>
    /// <returns></returns>
    public UnityEngine.Transform GetGameBackgroundMainUITransform()
    {
        if (m_MainUITransform == null)
        {
            return null;
        }
        return m_MainUITransform.Find("UiRootBG");
    }

    /// <summary>
    /// 离开旁观的游戏房间
    /// </summary>
    void OnLeaveBystanderGameRoom()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_CM_LEAVEONLOOKERROOM);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((uint)0);
        HallMain.SendMsgToRoomSer(msg);
    }

    /// <summary>
    /// 聊天事件
    /// </summary>
    /// <param name="index">聊天内容索引</param>
    void OnClickGouJiChatEvent(int index)
    {
        if (m_ChatTransform == null)
        {
            return;
        }
        if (!m_ChatTransform.gameObject.activeSelf)
        {
            return;
        }

        m_ChatTransform.gameObject.SetActive(false);

        PlayerGameAudio(0, 6);

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION);        msg.Add(GameMain.hall_.GetPlayerId());        msg.Add((byte)index);        HallMain.SendMsgToRoomSer(msg);    }

    /// <summary>
    /// 设置托管状态
    /// </summary>
    /// <param name="activeState">true(进入托管状态)false(退出托管状态)</param>
    public void SetTrustButtonActiveState(bool activeState)
    {
        if(m_TrustButton == null)
        {
            return;
        }

        if(m_TrustButton.interactable != activeState)
        {
            m_TrustButton.interactable = activeState;
            m_TrustButton.gameObject.SetActive(activeState);
        }
    }

    /// <summary>
    /// 约据总结算界面事件
    /// </summary>
    private void OnCloseAppointmentResultPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            m_AppointmentResultPanel.SetActive(false);
            GameOver(false);            BackToGameRoom();
            AppointmentDataManager.AppointmentDataInstance().playerAlready = false;            PlayerGameAudio(0, 6);        }    }

    /// <summary>
    /// 游戏规则信息面板事件
    /// </summary>
    private void OnClickPromptInofPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {
        if (m_AppointmentRulePanel == null ||
            eventtype != EventTriggerType.PointerDown && eventtype != EventTriggerType.PointerUp)        {
            return;
        }

        if (m_AppointmentRuleEnumerator != null && eventtype == EventTriggerType.PointerUp)
        {
            GameMain.ST(m_AppointmentRuleEnumerator);
        }

        PlayerGameAudio(0, 6);

        Transform promptInfoTransform = m_AppointmentRulePanel.transform.Find("ImageLeftBG/RuleInfo_BG/Image_info");
        float promptTextHeight = -promptInfoTransform.GetComponent<RectTransform>().sizeDelta.y;
        promptInfoTransform.DOLocalMoveY(eventtype == EventTriggerType.PointerDown ? promptTextHeight : 0, 0.6f);    }

    /// <summary>
    /// 进贡详情
    /// </summary>
    /// <param name="eventtype">事件触发类型</param>
    /// <param name="message">附属参数</param>
    /// <param name="eventData">事件点击属性</param>
    public void OnClickTribute(EventTriggerType eventtype, object message, PointerEventData eventData)    {        if (m_TributeTransform == null)
        {
            return;
        }        switch (eventtype)
        {
            case EventTriggerType.PointerClick:
                {
                    PlayerGameAudio(0, 6);
                    SetTributeInfoPanelActive(!m_TributeTransform.gameObject.activeSelf);
                }
                break;
            default:
                break;

        }
    }

    /// <summary>
    /// 聊天详情
    /// </summary>
    /// <param name="eventtype">事件触发类型</param>
    /// <param name="message">附属参数</param>
    /// <param name="eventData">事件点击属性</param>
    public void OnClickEmotion(EventTriggerType eventtype, object message, PointerEventData eventData)    {        if (m_ChatTransform == null)
        {
            return;
        }

        if (eventtype == EventTriggerType.PointerClick)
        {
            PlayerGameAudio(0, 6);
            m_ChatTransform.gameObject.SetActive(!m_ChatTransform.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// 游戏设置界面按钮事件
    /// </summary>
    private void OnClickSetButtonEvent(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            if (m_SetMainTransform == null)
            {
                return;
            }
            bool activeState = (bool)(button);
            if (m_SetMainTransform.gameObject.activeSelf != activeState)
            {
                PlayerGameAudio(0, 6);
                m_SetMainTransform.gameObject.SetActive(activeState);
            }        }    }

    /// <summary>
    /// 添加买3买4信息
    /// </summary>
    /// <param name="tributeInfo"></param>
    void AddTributeInfo(RoleSwapPokerData tributeInfo)
    {
        if (m_TributeText == null)
        {
            return;
        }

        string tributInfoText = string.Empty;
        //买家
        byte tSitIndex = GetClientRoleSit(tributeInfo.m_nMSit, m_PlayerList[0].m_nSSit);
        tributInfoText += m_sRoleNickname[tSitIndex];

        if (tributeInfo.m_nMPokerValue != 0)
        {
            tributInfoText += "用";

            int cardValue = GameCommon.GetCardValue(tributeInfo.m_nMPokerValue);
            if (cardValue == 0)
            {
                tributInfoText += "大王";
            }
            else if (cardValue == -1)
            {
                tributInfoText += "小王";
            }
            else
            {
                tributInfoText += cardValue;
            }

        }
        tributInfoText += "向";

        //卖家
        byte mSitIndex = GetClientRoleSit(tributeInfo.m_nTSit, m_PlayerList[0].m_nSSit);
        tributInfoText += m_sRoleNickname[mSitIndex];

        if (tributeInfo.m_nMPokerValue != 0)
        {
            tributInfoText += "买";
        } else
        {
            tributInfoText += "要";
        }
        tributInfoText += GameCommon.GetCardValue(tributeInfo.m_nTPokerValue) + "\n";

        if (!string.IsNullOrEmpty(tributInfoText))
        {
            m_TributeText.text += tributInfoText;
        }
    }

    /// <summary>
    /// 添加进贡信息
    /// </summary>
    /// <param name="tributeInfo"></param>
    void AddTributeInfo(RoleTributeData tributeInfo)
    {
        if (m_TributeText == null || tributeInfo.m_nTributeState <= 0)
        {
            return;
        }

        string tributInfoText = string.Empty;

        //上贡玩家
        byte tSitIndex = GetClientRoleSit(tributeInfo.m_nTributeOutSit, m_PlayerList[0].m_nSSit);
        tributInfoText += m_sRoleNickname[tSitIndex];

        tributInfoText += "向";

        //接受玩家
        byte mSitIndex = GetClientRoleSit(tributeInfo.m_nTributeInSit, m_PlayerList[0].m_nSSit);
        tributInfoText += m_sRoleNickname[mSitIndex];

        tributInfoText += "进贡";

        int cardValue = 0,countIndex = tributeInfo.m_TributePokerList.Count;
        foreach (var pokerValue in tributeInfo.m_TributePokerList)
        {
            cardValue = GameCommon.GetCardValue(pokerValue);
            if (cardValue == 0)
            {
                tributInfoText += "大王";
            }
            else if (cardValue == -1)
            {
                tributInfoText += "小王";
            }
            else
            {
                tributInfoText += cardValue;
            }
            --countIndex;
            if(countIndex >= 1)
            {
                tributInfoText += ",";
            }
        }
        tributInfoText += "\n";

        if (!string.IsNullOrEmpty(tributInfoText))
        {
            m_TributeText.text += tributInfoText;
        }
    }

    /// <summary>
    /// 进贡详情界面是否激活
    /// </summary>
    /// <param name="activeState">true:激活false:失败</param>
    void SetTributeInfoPanelActive(bool activeState = false)
    {
        if (m_TributeTransform == null)
        {
            return;
        }
        m_TributeTransform.gameObject.SetActive(activeState);
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="onceEnd">true:一局结束 false: 整个游戏结束</param>
    public void GameOver(bool onceEnd)
    {
        ResetGameData();

        if (Bystander && onceEnd)
        {
            Bystander = false;
            OnLeaveBystanderGameRoom();
        }
    }

    /// <summary>
    /// 根据名次进行排名
    /// </summary>
    /// <param name="roleResletDataA"></param>
    /// <param name="roleResletDataB"></param>
    /// <returns></returns>
    public static int SortByRoleRank(RoleResultData roleResletDataA, RoleResultData roleResletDataB)//从小到大
    {        return roleResletDataA.m_nRank - roleResletDataB.m_nRank;    }

    /// <summary>
    /// 结算界面按钮禁用状态
    /// </summary>
    /// <param name="interactableState">ture:按钮可用，false:不可用</param>
    void SetResultButtonInteractable(bool interactableState)
    {
        if (m_LeaveButton)
        {
            m_LeaveButton.interactable = interactableState;
        }

        if (m_EnterButton)
        {
            m_EnterButton.interactable = interactableState;
        }
    }

    /// <summary>
    /// 计算联邦积分
    /// </summary>
    /// <param name="roleReslutDataList">联邦角色结算数据</param>
    /// <returns>联邦积分</returns>
    long CalculateFederationTotalScore(List<RoleResultData> roleReslutDataList)
    {
        long totalScore = 0;
        foreach (var roleReslutData in roleReslutDataList)
        {
            totalScore += roleReslutData.m_nAddCoin;
        }
        return totalScore;
    }

    /// <summary>
    /// 获得协程数据执行状态
    /// </summary>
    /// <param name="roomState">房间状态</param>
    /// <returns></returns>
    bool GetCoroutinesDataState(RoomState_Enum roomState)
    {
        RoomState_Enum curRoomState = RoomState_Enum.RoomState_DealPoker;
        for (RoomState_Enum roomStateIndex = roomState - 1; roomStateIndex >= RoomState_Enum.RoomState_Mai3; --roomStateIndex)
        {
            if (m_CurCoroutinesDictionary.ContainsKey(roomStateIndex))
            {
                curRoomState = roomStateIndex;
                break;
            }
        }

        if (m_CurCoroutinesDictionary.ContainsKey(curRoomState))
        {
            return m_CurCoroutinesDictionary[curRoomState] == 0;
        }
        return false;
    }

    /// <summary>
    /// 获得音效ID
    /// </summary>
    /// <param name="roleSit">玩家客户端座位号</param>
    /// <param name="id">音效ID</param>
    /// <param name="param_male">男性音效参数值</param>
    /// <param name="param_female">女性音效参数值</param>
    /// <returns></returns>
    public int GetGameAudioID(byte roleSit, int id ,int param_male = 1000,int param_female = 2000)
    {
       if (m_PlayerList.Count <= roleSit || roleSit < 0)
        {
            return 0;
        }
        return (m_PlayerList[roleSit].m_nSex != 1 ? param_female : param_male) + id;
    }

    /// <summary>
    /// 播放音效(只能适用很短的音效)
    /// </summary>
    /// <param name="roleSit">玩家客户端座位号</param>
    /// <param name="id">音效ID</param>
    /// <returns></returns>
    public void PlayerGameAudio(byte roleSit, int id)
    {
        string audioData = roleSit + "|" + m_nAudioIndex + "|" + id ;
        m_GameAudioList.Add(audioData);
        ++m_nAudioIndex;
    }

    /// <summary>
    /// 执行音效播放
    /// </summary>
    private void UpdateGameAudio()
    {
        if(m_GameAudioList.Count == 0 ||
           !AudioManager.Instance.GetIdleSoundAudioSourceState())
        {
            return;
        }
        byte roleSit = byte.Parse(m_GameAudioList[0].Substring(0, m_GameAudioList[0].IndexOf('|')));
        int audioID = int.Parse(m_GameAudioList[0].Substring(m_GameAudioList[0].LastIndexOf('|')+1));
        CustomAudioDataManager.GetInstance().PlayAudio(GetGameAudioID(roleSit, audioID));
        m_GameAudioList.RemoveAt(0);
    }

    /// <summary>
    /// 显示约据总结算
    /// </summary>
    void ShowAppointmentTotalResult()
    {
        AppointmentDataManager.AppointmentDataInstance().interrupt = false;
        AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = false;

        if(m_AppointmentResultPanel == null)
        {
            return;
        }

        int index = 1;
        GouJi_Role curRole = null;
        Transform playerTransform = null,curJiFenOneTransform = null, curJiFenTwoTransform = null;
        Transform resultPlayerTransform = m_AppointmentResultPanel.transform.Find("ImageBG/Imageplayer");
        foreach (var roleData  in AppointmentDataManager.AppointmentDataInstance().resultList)
        {
            playerTransform = resultPlayerTransform.Find((index).ToString());
            curRole = m_PlayerList.Find(role => role.m_nUseId == roleData.playerid);
            if(curRole != null)
            {
                playerTransform.Find("Head/HeadMask/ImageHead").GetComponent<Image>().sprite = curRole.GetHeadImg();
                playerTransform.Find("TextName").GetComponent<Text>().text = curRole.GetRoleName();
            }
            curJiFenOneTransform = playerTransform.Find("Text_jifen/TextNum_1");
            curJiFenTwoTransform = playerTransform.Find("Text_jifen/TextNum_2");
            curJiFenOneTransform.gameObject.SetActive(roleData.coin >= 0);
            curJiFenTwoTransform.gameObject.SetActive(roleData.coin < 0);
            if(roleData.coin >= 0)
            {
                curJiFenOneTransform.GetComponent<Text>().text = roleData.coin.ToString();
            }
            else
            {
                curJiFenTwoTransform.GetComponent<Text>().text = roleData.coin.ToString();
            }
            ++index;
            playerTransform.gameObject.SetActive(true);
        }

        //录像记录
        AppointmentRecord recordData = new AppointmentRecord();
        recordData.gameID = (byte)GameKind_Enum.GameKind_GouJi;
        recordData.gamerule = "够级 打" + AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().playtimes.ToString() + "局";
        recordData.timeseconds = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().createtimeseconds;
        recordData.isopen = false;
        recordData.videoes = FriendsMomentsDataMamager.GetFriendsInstance().currentvideoid;
        recordData.recordTimeSeconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);

        AssetBundle assetbundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (assetbundle != null)
        {
            Text coinText = null;
            GameObject CurObject = null;
            Transform curTransform = null;
            AppointmentRecordPlayer recordPlayerdata = null;
            Transform viewportTransform = m_AppointmentResultPanel.transform.Find("ImageBG/Viewport/Content");
            UnityEngine.Object xiangQingObject = assetbundle.LoadAsset("Room_Result_xiangqing");
            for (index = 0; index < AppointmentDataManager.AppointmentDataInstance().perResultList_.Count; index++)
            {
                CurObject = (GameObject)GameMain.instantiate(xiangQingObject);
                CurObject.transform.SetParent(viewportTransform, false);
                CurObject.transform.Find("Text_ju/Textnum").GetComponent<Text>().text = (index+1).ToString();


                foreach (var roleData in AppointmentDataManager.AppointmentDataInstance().perResultList_[index])
                {
                    curTransform = CurObject.transform.Find("ImageBG/Text_ranking_" + (roleData.Key + 1).ToString());
                    coinText = curTransform.GetComponent<Text>();
                    coinText.text = roleData.Value.coin.ToString();
                    curTransform.gameObject.SetActive(true);
                    
                    curRole = m_PlayerList.Find(role => role.m_nUseId == roleData.Value.playerid);
                    if (curRole != null)
                    {
                        recordPlayerdata = new AppointmentRecordPlayer();
                        curRole.GetRecordPlyerInfo(ref recordPlayerdata);
                        recordPlayerdata.coin = roleData.Value.coin;
                        if (!recordData.result.ContainsKey(recordPlayerdata.playerid))
                            recordData.result.Add(recordPlayerdata.playerid, recordPlayerdata);
                    }
                }
                CurObject.SetActive(true);
            }
        }

        GameMain.hall_.gamerooms_.recordlist_.Insert(0, recordData);

        m_AppointmentResultPanel.SetActive(true);
    }

    /// <summary>
    /// 刷新当前房间状态协成逻辑
    /// </summary>
    void RefreshRoomStateEnumeratorLogic()
    {
        if (m_CurRoomStateEnumerator != null)
        {
            GameMain.ST(m_CurRoomStateEnumerator);
            RoomStateEnumeratorData roomStateData = new RoomStateEnumeratorData();
            roomStateData.ReplicateRoomStateData(m_CurRoomStateStateData);
            if (roomStateData.m_CurRoleTributeDataList.Count > 0)
            {
                GameMain.SC(ShowTribute(roomStateData, 0));
            }
            else
            {
                GameMain.SC(ShowSwapPoker(roomStateData, 0));
            }
            m_CurRoomStateEnumerator = null;
        }
    }

    /// <summary>
    /// 显示约据游戏规则界面
    /// </summary>
    /// <returns>协程对象</returns>
    IEnumerator ShowAppointmentRule()
    {
        yield return new WaitForEndOfFrame();
        OnClickPromptInofPanel(EventTriggerType.PointerDown, null, null);
        yield return new WaitForSeconds(5.0f);
        OnClickPromptInofPanel(EventTriggerType.PointerUp, null, null);
        m_AppointmentRuleEnumerator = null;
        yield break;
    }

    /// <summary>
    /// 显示游戏结算界面
    /// </summary>
    /// <param name="totalEnd">总结束</param>
    /// <param name="baseCoin">底分</param>
    /// <param name="charge">房费</param>
    /// <param name="mulitple">倍数</param>
    /// <param name="RoleResultDataList">结算数据</param>
    /// <returns></returns>
    IEnumerator ShowResult(bool totalEnd ,uint baseCoin, uint charge,byte mulitple,List<RoleResultData> RoleResultDataList)
    {
        List<RoleResultData> RedCampResultDataList = RoleResultDataList.FindAll(resultData => resultData.m_nSit == 0 || resultData.m_nSit == 2 || resultData.m_nSit == 4);
        List<RoleResultData> BlueCampResultDataList = RoleResultDataList.FindAll(resultData => resultData.m_nSit == 1 || resultData.m_nSit == 3 || resultData.m_nSit == 5);
        RedCampResultDataList.Sort(SortByRoleRank);
        BlueCampResultDataList.Sort(SortByRoleRank);

        long RedTotalScore =  CalculateFederationTotalScore(RedCampResultDataList);
        long BlueTotalScore = CalculateFederationTotalScore(BlueCampResultDataList);
        yield return new WaitForSecondsRealtime(5f);
        if(m_ResultMainTransform == null)
        {
            Debug.Log("结算面板资源找不到!");
            yield break;
        }

        List<RoleResultData> roleResultDataList = new List<RoleResultData>();
        RoleResultData selfResultData = RedCampResultDataList.Find(resultData => resultData.m_nSit == m_PlayerList[0].m_nSSit);

        int winStateIndex = -1; // 0:平局 1: 胜利 2:失败
        int campCount = 0;
        if(selfResultData == null)
        {
            campCount = BlueCampResultDataList.Count;
            selfResultData = BlueCampResultDataList.Find(resultData => resultData.m_nSit == m_PlayerList[0].m_nSSit);
            if(selfResultData != null)
            {
                roleResultDataList.Add(selfResultData);
                BlueCampResultDataList.Remove(selfResultData);
                roleResultDataList.AddRange(BlueCampResultDataList);
                roleResultDataList.AddRange(RedCampResultDataList);
            }
            winStateIndex = RedTotalScore == BlueTotalScore ? 0 : (RedTotalScore < BlueTotalScore ? 1 : 2);
        }
        else
        {
            campCount = RedCampResultDataList.Count;
            roleResultDataList.Add(selfResultData);
            RedCampResultDataList.Remove(selfResultData);
            roleResultDataList.AddRange(RedCampResultDataList);
            roleResultDataList.AddRange(BlueCampResultDataList);
            winStateIndex = RedTotalScore == BlueTotalScore ? 0 : (RedTotalScore > BlueTotalScore ? 1 : 2);
        }

        //胜利音效
        int[] AudioIDs = { 4, 2, 3};
        PlayerGameAudio(0, AudioIDs[winStateIndex]);

        //胜利动画
        string [] animationName = {"anime_result_ping", "anime_result_sheng", "anime_result_bai" };
        if(m_AnimatonResultObject)
        {
            GameObject.Destroy(m_AnimatonResultObject);
        }

        float lifeTime = GameMode == GameTye_Enum.GameType_Contest ? 3.4f : 0.0f;
        Transform animatinoSocketTransform = GameMode == GameTye_Enum.GameType_Contest ? m_AnimationTfm : m_ResultMainTransform.Find("ImageBG/animation");
        m_AnimatonResultObject = GameFunction.PlayUIAnim(animationName[winStateIndex],lifeTime, animatinoSocketTransform, m_GouJiAssetBundle);

        switch(GameMode)
        {
            case GameTye_Enum.GameType_Contest:
                {
                    yield return new WaitForSecondsRealtime(3.5f);

                    //录像记录
                    AppointmentRecord recordData = null;
                    if (totalEnd && !Bystander)
                    {
                        recordData = new AppointmentRecord();
                        recordData.gameID = (byte)GameKind_Enum.GameKind_GouJi;
                        recordData.gamerule = "够级比赛 第" + MatchInGame.GetInstance().m_nCurTurn.ToString() + "轮";
                        recordData.timeseconds = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().createtimeseconds;
                        recordData.isopen = false;
                        recordData.videoes = FriendsMomentsDataMamager.GetFriendsInstance().currentvideoid;
                        recordData.recordTimeSeconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);
                    }

                    List<RoundResultData> roleRoundResultList = new List<RoundResultData>();
                    RoleResultData curRoleResultData = null;
                    AppointmentRecordPlayer recordPlayerdata = null;
                    for (int index = 0; index < 6; index++)
                    {
                        curRoleResultData = RoleResultDataList.Find(roleData => roleData.m_nSit == m_PlayerList[index].m_nSSit);
                        if (curRoleResultData == null)
                        {
                            roleRoundResultList.Add(null);
                            continue;
                        }

                        RoundResultData resultData = new RoundResultData();
                        resultData.headImg = m_PlayerList[index].GetHeadImg();
                        resultData.coin = curRoleResultData.m_nTotalCoin;
                        resultData.name = m_PlayerList[index].GetRoleName();
                        resultData.addCoin = curRoleResultData.m_nAddCoin;
                        roleRoundResultList.Add(resultData);

                        if(recordData != null)
                        {
                            recordPlayerdata = new AppointmentRecordPlayer();
                            m_PlayerList[index].GetRecordPlyerInfo(ref recordPlayerdata);
                            recordPlayerdata.coin = curRoleResultData.m_nTotalCoin;
                            if (!recordData.result.ContainsKey(recordPlayerdata.playerid))
                                recordData.result.Add(recordPlayerdata.playerid, recordPlayerdata);
                        }
                    }

                    if (recordData != null)
                    {
                        if (GameMain.hall_.gamerooms_ == null)
                            GameMain.hall_.InitRoomsData();

                        GameMain.hall_.gamerooms_.recordlist_.Insert(0, recordData);
                    }

                    yield return MatchInGame.GetInstance().ShowRoundResult(6, roleRoundResultList, () =>
                    {
                        GameOver(totalEnd);
                    }, totalEnd, Bystander);
                }
                break;
            default:
                {
                    Transform CurRoleTransform = null;
                    int curRoleIndex = 1; long totalScore = 0;
                    string[] textName = { "Text_dian", "Text_shao", "Text_men", "Text_luo" };
                    Transform RoleMainTransform = m_ResultMainTransform.Find("ImageBG/playerBG_1");
                    for (int childIndex = 0; childIndex < RoleMainTransform.childCount; ++childIndex)
                    {
                        RoleMainTransform.GetChild(childIndex).gameObject.SetActive(false);
                    }

                    foreach (var resultData in roleResultDataList)
                    {
                        if (curRoleIndex == campCount + 1)
                        {
                            curRoleIndex = 1;
                            RoleMainTransform = m_ResultMainTransform.Find("ImageBG/playerBG_2");
                            for (int childIndex = 0; childIndex < RoleMainTransform.childCount; ++childIndex)
                            {
                                RoleMainTransform.GetChild(childIndex).gameObject.SetActive(false);
                            }
                        }

                        CurRoleTransform = RoleMainTransform.Find(curRoleIndex.ToString());
                        if (CurRoleTransform == null)
                        {
                            DebugLog.Log("结算索引: " + curRoleIndex);
                            continue;
                        }

                        if (m_GouJiAssetBundle)
                        {
                            CurRoleTransform.Find("Image_ranking").GetComponent<Image>().sprite = m_GouJiAssetBundle.LoadAsset<Sprite>("Gj_PaiMing_" + (resultData.m_nRank + 1));
                        }

                        DebugLog.Log("======坐标：" + resultData.m_nSit + "名次:" + resultData.m_nRank + "序号:" + curRoleIndex);
                        GouJi_Role Role = m_PlayerList.Find(roleData => roleData.m_nSSit == resultData.m_nSit);
                        if (Role != null)
                        {
                            //更新角色金钱
                            CurRoleTransform.Find("Text_Name").GetComponent<Text>().text = Role != null ? Role.m_sRoleName : "";
                            Role.m_nTotalCoin = resultData.m_nTotalCoin;
                            Role.RefreshRoleMoneyUI();

                            //总积分
                            if (Role.m_nCSit == 0)
                            {
                                totalScore = resultData.m_nTotalCoin;
                            }
                        }

                        for (int coinIndex = 0; coinIndex < 4; ++coinIndex)
                        {
                            CurRoleTransform.Find(textName[coinIndex]).GetComponent<Text>().text = resultData.m_nCoinTypeValue[coinIndex].ToString();
                        }
                        CurRoleTransform.Find("Text_jifen").GetComponent<Text>().text = resultData.m_nAddCoin.ToString();
                        CurRoleTransform.gameObject.SetActive(true);
                        ++curRoleIndex;
                    }
                    m_ResultMainTransform.Find("ImageBG/Image_top/Text_difen/Text_num").GetComponent<Text>().text = baseCoin.ToString();
                    m_ResultMainTransform.Find("ImageBG/Image_top/Text_beishu/Text_num").GetComponent<Text>().text = mulitple.ToString();
                    m_ResultMainTransform.Find("ImageBG/Image_bottom/Text_fangfei/Text_2").GetComponent<Text>().text = charge.ToString();

                    if (m_GouJiAssetBundle)
                    {
                        string imageName = m_PlayerList[0].m_nSSit % 2 > 0 ? "gj_frame_js1" : "gj_frame_js";
                        m_ResultMainTransform.Find("ImageBG/BG1").GetComponent<Image>().sprite = m_GouJiAssetBundle.LoadAsset<Sprite>(imageName);
                    }

                    Transform totalScoreTransform = m_ResultMainTransform.Find("ImageBG/Image_bottom/Text_zongfen");
                    totalScoreTransform.Find("Text_2").GetComponent<Text>().text = totalScore.ToString();
                    totalScoreTransform.gameObject.SetActive(GameMode == GameTye_Enum.GameType_Appointment);

                    if (GameMode == GameTye_Enum.GameType_Appointment)
                    {
                        bool isshowtotal = AppointmentDataManager.AppointmentDataInstance().interrupt || totalEnd;
                        m_LeaveButton.gameObject.SetActive(false);
                        m_EnterButton.gameObject.SetActive(isshowtotal);
                    }

                    SetResultButtonInteractable(m_eRoomState == RoomState_Enum.RoomState_OnceEnd || m_eRoomState == RoomState_Enum.RoomState_TotalEnd ||
                                                m_eRoomState < RoomState_Enum.RoomState_CountDownBegin);

                    m_ResultMainTransform.gameObject.SetActive(true);
                }
                break;
        }
        yield break;
    }

    /// <summary>
    /// 显示角色交换牌流程(买三买四流程)
    /// </summary>
    /// <param name="roomStateData">买三或买四数据</param>
    /// <param name="roomTime">当前状态时间</param>
    /// <returns></returns>
    IEnumerator ShowSwapPoker(RoomStateEnumeratorData roomStateData,float roomTime)
    {
        if(roomStateData.m_nAStageState == AnimationStage_Enum.eAS_Ready)
        {
            #region 当前流程执行前提条件
            if (roomStateData.m_CurRoleSwapPokerDataList.Count == 0 || m_GouJiAssetBundle == null)
            {
                yield break;
            }
            yield return new WaitUntil(() =>
            {
                return GetCoroutinesDataState(roomStateData.m_nRoomState);
            });

            Transform ChuPaiTransform = null;
            foreach (var player in m_PlayerList)
            {
                ChuPaiTransform = player.m_RoleMainUITranform.Find("Poker_Chupai");
                player.m_PokerSwapTransform.SetParent(ChuPaiTransform, false);
            }
            #endregion
            #region 买三买四动画
            string animName = roomStateData.m_nRoomState == RoomState_Enum.RoomState_Mai3 ? "anime_GJ_mai3" : "anime_GJ_mai4";
            if (m_AnimatonGongObject)
            {
                GameObject.Destroy(m_AnimatonGongObject);
            }
            m_AnimatonGongObject = GameFunction.PlayUIAnim(animName, 0, m_AnimationTfm, m_GouJiAssetBundle);
            #endregion
            roomStateData.m_nAStageState = AnimationStage_Enum.eAS_Show;
            yield return new WaitForSecondsRealtime(roomTime * 0.2f);
        }
        if (roomStateData.m_nAStageState == AnimationStage_Enum.eAS_Show)
        {
            #region 买家卖家显示牌
            List<byte> pokerDataList = null;
            List<RoleSwapPokerData> rolePokerDataList = null;
            foreach (var player in m_PlayerList)
            {
                player.m_PokerSwapTransform.SetParent(player.m_RoleMainUITranform, true);

                rolePokerDataList = roomStateData.m_CurRoleSwapPokerDataList.FindAll(value => value.m_nTSit == player.m_nSSit);
                if (rolePokerDataList != null && rolePokerDataList.Count > 0)
                {
                    pokerDataList = new List<byte>();
                    foreach (var pokerValue in rolePokerDataList)
                    {
                        if (pokerValue.m_nTPokerValue == 0)
                        {
                            continue;
                        }
                        pokerDataList.Add(pokerValue.m_nTPokerValue);
                    }
                    player.AddSwapPokerObject(pokerDataList);
                }
                else
                {
                    rolePokerDataList = roomStateData.m_CurRoleSwapPokerDataList.FindAll(value => value.m_nMSit == player.m_nSSit);
                    if (rolePokerDataList != null && rolePokerDataList.Count > 0)
                    {
                        pokerDataList = new List<byte>();
                        foreach (var pokerValue in rolePokerDataList)
                        {
                            if (pokerValue.m_nMPokerValue == 0)
                            {
                                continue;
                            }
                            pokerDataList.Add(pokerValue.m_nMPokerValue);
                        }
                        player.AddSwapPokerObject(pokerDataList);
                    }
                }
            }
            #endregion
            roomStateData.m_nAStageState = AnimationStage_Enum.eAS_Move;
            yield return new WaitForSecondsRealtime(roomTime * 0.2f);
        }

        if(roomStateData.m_nAStageState == AnimationStage_Enum.eAS_Move)
        {
            #region 从卖家移动到买家
            GouJi_Role roleMObject = null, roleTObject = null;
            //Transform mChuPaiTransform = null, tChuPaiTransform = null;
            foreach (var roleSwapValue in roomStateData.m_CurRoleSwapPokerDataList)
            {
                roleMObject = m_PlayerList.Find(roleValue => roleValue.m_nSSit == roleSwapValue.m_nTSit);
                roleTObject = m_PlayerList.Find(roleValue => roleValue.m_nSSit == roleSwapValue.m_nMSit);
                if (roleMObject != null && roleTObject != null)
                {
                    Transform mChuPaiTransform = roleMObject.m_RoleMainUITranform.Find("Poker_Chupai");
                    Transform tChuPaiTransform = roleTObject.m_RoleMainUITranform.Find("Poker_Chupai");
                    Vector3 postionT = roleTObject.m_PokerSwapTransform ? roleTObject.m_PokerSwapTransform.position : tChuPaiTransform.position;
                    Vector3 postionM = roleMObject.m_PokerSwapTransform ? roleMObject.m_PokerSwapTransform.position : mChuPaiTransform.position;

                    if (mChuPaiTransform.childCount > 0)
                    {
                        Transform childTransform = mChuPaiTransform.GetChild(0);
                        childTransform.SetParent(roleMObject.m_RoleMainUITranform, true);
                        m_CurCoroutinesObjectList.Add(childTransform);
                        childTransform.DOMove(postionT, roomTime * 0.58f).OnComplete(() =>
                        {
                            if (m_CurCoroutinesDictionary.ContainsKey(roomStateData.m_nRoomState))
                            {
                                if (m_CurCoroutinesDictionary[roomStateData.m_nRoomState] != 0)
                                {
                                    childTransform.SetParent(tChuPaiTransform, false);
                                    return;
                                }
                            }
                            GameObject.Destroy(childTransform.gameObject);
                        });
                    }

                    if (tChuPaiTransform.childCount > 0)
                    {
                        Transform childTransform = tChuPaiTransform.GetChild(0);
                        childTransform.SetParent(roleTObject.m_RoleMainUITranform, true);
                        m_CurCoroutinesObjectList.Add(childTransform);
                        childTransform.DOMove(postionM, roomTime * 0.58f).OnComplete(() =>
                        {
                            if (m_CurCoroutinesDictionary.ContainsKey(roomStateData.m_nRoomState))
                            {
                                if (m_CurCoroutinesDictionary[roomStateData.m_nRoomState] != 0)
                                {
                                    childTransform.SetParent(mChuPaiTransform, false);
                                    return;
                                }
                            }
                            GameObject.Destroy(childTransform.gameObject);
                        });
                    }
                }
            }
            #endregion
            roomStateData.m_nAStageState = AnimationStage_Enum.eAS_End;
            yield return new WaitForSecondsRealtime(roomTime * 0.6f);
        }

        if(roomStateData.m_nAStageState == AnimationStage_Enum.eAS_End)
        {
            #region 更新卖家和买家手牌
            foreach(var transform in m_CurCoroutinesObjectList)
            {
                transform.DOKill(true);
            }
            m_CurCoroutinesObjectList.Clear();

            if (m_TributeText)
            {
                m_TributeText.text += m_sTributeName[(byte)(roomStateData.m_nRoomState - RoomState_Enum.RoomState_Mai3)];
            }
            GouJi_Role role = null;
            List<byte> pokerDataList = null;
            foreach (var rolePokerData in roomStateData.m_CurRoleSwapPokerDataList)
            {
                role = m_PlayerList.Find(player => player.m_nSSit == rolePokerData.m_nMSit);
                if (role != null)
                {
                    role.DestoryAllOutPokerObject();
                    pokerDataList = new List<byte>();
                    if (rolePokerData.m_nTPokerValue != 0)
                    {
                        pokerDataList.Add(rolePokerData.m_nTPokerValue);
                    }
                    role.AddHavePokerObject(pokerDataList);
                }

                role = m_PlayerList.Find(player => player.m_nSSit == rolePokerData.m_nTSit);
                if (role != null)
                {
                    role.DestoryAllOutPokerObject();
                    pokerDataList = new List<byte>();
                    if (rolePokerData.m_nMPokerValue != 0)
                    {
                        pokerDataList.Add(rolePokerData.m_nMPokerValue);
                    }
                    role.AddHavePokerObject(pokerDataList, false, GameMode == GameTye_Enum.GameType_Record ? false : true);
                }
                AddTributeInfo(rolePokerData);
            }
            #endregion
            #region 当前流程执行完毕
            RefreshCoroutinesDictionary(roomStateData.m_nRoomState);
            #endregion
            #region 删除动画
            if (m_AnimatonGongObject)
            {
                GameObject.Destroy(m_AnimatonGongObject);
            }
            #endregion
            roomStateData.m_nAStageState = AnimationStage_Enum.eAS_Count;
        }
    }

    /// <summary>
    /// 进贡流程
    /// </summary>
    /// <param name="roomStateData">进贡数据</param>
    /// <param name="roomTime">进贡房间阶段时间</param>
    /// <returns></returns>
    IEnumerator ShowTribute(RoomStateEnumeratorData roomStateData, float roomTime)
    {
        if (roomStateData.m_nAStageState == AnimationStage_Enum.eAS_Ready)
        {
            #region 上贡前提条件
            if (roomStateData.m_CurRoleTributeDataList.Count == 0)
            {
                yield break;
            }
            yield return new WaitUntil(() =>
            {
                return GetCoroutinesDataState(roomStateData.m_nRoomState);
            });
            Transform ChuPaiTransform = null;
            foreach (var player in m_PlayerList)
            {
                ChuPaiTransform = player.m_RoleMainUITranform.Find("Poker_Chupai");
                player.m_PokerSwapTransform.SetParent(ChuPaiTransform, false);
            }
            #endregion
            #region 进贡动画
            string[] animationName = { "anime_GJ_jingong_5quan", "anime_GJ_jingong_1dian", "anime_GJ_jingong_2shao" ,
                                    "anime_GJ_jingong_3men","anime_GJ_jingong_4luo"};
            if (m_AnimatonGongObject)
            {
                GameObject.Destroy(m_AnimatonGongObject);
            }
            m_AnimatonGongObject = GameFunction.PlayUIAnim(animationName[(int)roomStateData.m_nRoomState - 9], 0, m_AnimationTfm, m_GouJiAssetBundle);
            #endregion
            roomStateData.m_nAStageState = AnimationStage_Enum.eAS_Show;
            yield return new WaitForSecondsRealtime(roomTime * 0.2f);
        }
        if (roomStateData.m_nAStageState == AnimationStage_Enum.eAS_Show)
        {
            #region 上贡之前
            foreach (var player in m_PlayerList)
            {
                player.m_PokerSwapTransform.SetParent(player.m_RoleMainUITranform, true);
            }
            GouJi_Role playerTOut = null;
            foreach (var tributeData in roomStateData.m_CurRoleTributeDataList)
            {
                playerTOut = m_PlayerList.Find(role => role.m_nSSit == tributeData.m_nTributeOutSit);
                if (playerTOut != null)
                {
                    if (tributeData.m_nTributeState == -2)
                    {
                        playerTOut.AddRoleAnimation("anime_GJ_kanggong");
                    }
                    else if (tributeData.m_nTributeState == -1)
                    {
                        GameFunction.PlayUIAnim("anime_GJ_kanggong3", roomTime < 0.1f ? 0.01f : roomTime * 0.3f, m_AnimationTfm, m_GouJiAssetBundle);
                    }

                    playerTOut.DestoryAllOutPokerObject();
                    playerTOut.AddSwapPokerObject(tributeData.m_TributePokerList);
                }
            }
            #endregion
            roomStateData.m_nAStageState = AnimationStage_Enum.eAS_Move;
            yield return new WaitForSecondsRealtime(roomTime * 0.2f);
        }

        if (roomStateData.m_nAStageState == AnimationStage_Enum.eAS_Move)
        {
            #region 上贡开始
            Vector3 tInPostion = Vector3.zero;
            Transform tOutTransform = null;
            GouJi_Role playerTOut = null, playerTIn = null;
            foreach (var tributeData in roomStateData.m_CurRoleTributeDataList)
            {
                playerTOut = m_PlayerList.Find(role => role.m_nSSit == tributeData.m_nTributeOutSit);
                playerTIn = m_PlayerList.Find(role => role.m_nSSit == tributeData.m_nTributeInSit);
                if (playerTOut != null && playerTIn != null)
                {
                    tOutTransform = playerTOut.m_RoleMainUITranform.Find("Poker_Chupai");
                    Transform tInTransform = playerTIn.m_RoleMainUITranform.Find("Poker_Chupai");
                    tInPostion = playerTIn.m_PokerSwapTransform ? playerTIn.m_PokerSwapTransform.position : tInTransform.position;

                    while (tOutTransform.childCount > 0)
                    {
                        Transform childTransform = tOutTransform.GetChild(0);
                        childTransform.SetParent(playerTOut.m_RoleMainUITranform, true);
                        m_CurCoroutinesObjectList.Add(childTransform);
                        childTransform.DOMove(tInPostion, roomTime * 0.58f).OnComplete(() =>
                        {
                            if (m_CurCoroutinesDictionary.ContainsKey(roomStateData.m_nRoomState))
                            {
                                if (m_CurCoroutinesDictionary[roomStateData.m_nRoomState] != 0)
                                {
                                    childTransform.SetParent(tInTransform, false);
                                    return;
                                }
                            }
                            GameObject.Destroy(childTransform.gameObject);
                        });
                    }
                }
            }
            #endregion
            roomStateData.m_nAStageState = AnimationStage_Enum.eAS_End;
            yield return new WaitForSecondsRealtime(roomTime * 0.6f);
        }

        if (roomStateData.m_nAStageState == AnimationStage_Enum.eAS_End)
        {
            #region 上贡结束
            foreach (var transform in m_CurCoroutinesObjectList)
            {
                transform.DOKill(true);
            }
            m_CurCoroutinesObjectList.Clear();

            if (m_TributeText)
            {
                m_TributeText.text += m_sTributeName[(byte)(roomStateData.m_nRoomState - RoomState_Enum.RoomState_Mai3)];
            }
            GouJi_Role playerTOut = null, playerTIn = null;
            foreach (var tributeData in roomStateData.m_CurRoleTributeDataList)
            {
                playerTOut = m_PlayerList.Find(role => role.m_nSSit == tributeData.m_nTributeOutSit);
                playerTIn = m_PlayerList.Find(role => role.m_nSSit == tributeData.m_nTributeInSit);
                if (playerTOut != null)
                {
                    playerTOut.DestoryAllOutPokerObject();
                }
                if (playerTIn != null)
                {
                    playerTIn.DestoryAllOutPokerObject();
                    playerTIn.AddHavePokerObject(tributeData.m_TributePokerList, false, GameMode == GameTye_Enum.GameType_Record ? false : true);
                }
                AddTributeInfo(tributeData);
            }
            #endregion
            #region 更新进贡前提条件
            RefreshCoroutinesDictionary(roomStateData.m_nRoomState);
            #endregion
            #region 删除动画
            if (m_AnimatonGongObject)
            {
                GameObject.Destroy(m_AnimatonGongObject);
            }
            #endregion
            roomStateData.m_nAStageState = AnimationStage_Enum.eAS_Count;
        }
    }
}