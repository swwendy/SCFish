using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USocket.Messages;
using DG.Tweening;
using XLua;

#region "象棋游戏枚举"
/// <summary>
/// 象棋游戏房间状态
/// </summary>
[LuaCallCSharp]
public enum EChessRoomState
{
    ChessRoomState_Init = 0,
    ChessRoomState_WaitPlayer,         //等人
    ChessRoomState_WaitReady,          //等待准备
    ChessRoomState_TotalBegin,         //总的开始
    ChessRoomState_OnceBeginShow,      //每局开始前的显示
    ChessRoomState_Ask,                //提问
    ChessRoomState_OnceResult,         //
    ChessRoomState_OnceEnd,            //
    ChessRoomState_TotalResult,        //总的结果
    ChessRoomState_TotalEnd,           //
    ChessRoomState_MaxStateCount       //回收状态
};

//玩家状态
[LuaCallCSharp]
public enum EChessPlayerState
{
    ChessPlayerState_Init = 0,
    ChessPlayerState_Match,          //1匹配中
    ChessPlayerState_GameIn,         //2游戏中
    ChessPlayerState_OnGameButIsOver,//3游戏中但游戏已经结束
    ChessPlayerState_ReadyHall,      //玩家在大厅
    ChessPlayerState_OnDesk,         //玩家在桌子上

    ChessPlayerState_Max
};

/// <summary>
/// 棋子类型
/// </summary>
[LuaCallCSharp]
public enum EChessPiecesType
{
    CChessType_None,
    CChessType_General,         //将
    CChessType_Guard_1,         //士
    CChessType_Guard_2,         //士
    CChessType_Elephant_1,      //象
    CChessType_Elephant_2,      //象
    CChessType_Cannon_1,        //炮
    CChessType_Cannon_2,        //炮
    CChessType_Horse_1,         //马
    CChessType_Horse_2,         //马
    CChessType_Rook_1,          //车
    CChessType_Rook_2,          //车
    CChessType_Soldier_1,       //卒
    CChessType_Soldier_2,       //卒
    CChessType_Soldier_3,       //卒
    CChessType_Soldier_4,       //卒
    CChessType_Soldier_5,       //卒
    CChessType_Num
};

/// <summary>
/// 象棋棋盘状态类型
/// </summary>
[LuaCallCSharp]
public enum EGeneralType
{
    eCChessJS_None,
    eCChessJS_Jiang,            //将军
    eCChessJS_JiangSi,          //绝杀
    eCChessJS_BieSi,            //憋毙
    eCChessJS_He_NoAttack,      //无攻击棋子判和
    eCChessJS_He_StepOver,      //总步数超限判和
    eCChessJS_He_NoKillJiang,   //无吃子将军次数超限判和
    eCChessJS_He_NoKillStep,    //无吃子着数超限判和
    eCChessJS_He_ChangJiang,    //互相长将判和
    eCChessJS_He_ChangZhuo,     //互相长捉判和
    eCChessJS_He_NoAction,		//非将非捉着数重复判和
};
#endregion

/// <summary>
/// 中国象棋
/// </summary>
[Hotfix]
public class CGame_Chess : CGameBase
{
    /// <summary>
    /// 断线重连成功标志
    /// </summary>
    bool m_bReconnected = false;

    /// <summary>
    /// 房间是否免费
    /// </summary>
    public bool m_bIsFree = false;

    /// <summary>
    /// 延迟加载资源帧数
    /// </summary>
    sbyte m_nWaitingLoad = 0;

    /// <summary>
    /// 房间等级
    /// </summary>
    byte m_nCurRoomLevel = RoomInfo.NoSit;

    /// <summary>
    /// 棋盘棋子位置是否交换
    /// </summary>
    bool m_bPiecePostionSwapState = false;

    /// <summary>
    /// 当前提问标志
    /// </summary>
    uint m_CurAskQuestionSign = 0;

    /// <summary>
    /// 当前下棋玩家服务端座位号
    /// </summary>
    byte m_nCurAskPlayerSSit = 0;

    /// <summary>
    ///玩家总下棋时间
    /// </summary>
    private uint m_nTotalPlayChessTime = 0;

    /// <summary>
    ///玩家一步下棋时间
    /// </summary>
    private uint m_nPlayChessTime = 0;

    /// <summary>
    /// 求和次数
    /// </summary>
    int m_nPlayerDrawCount = 2;

    /// <summary>
    /// 棋盘上一步棋子起始点
    /// </summary>
    int m_nLastSourcePiecePostion = 0;

    /// <summary>
    /// 棋盘上一步棋子目标点
    /// </summary>
    int m_nLastTargetPiecePostion = 0;

    /// <summary>
    /// 游戏约据当前局数
    /// </summary>
    private int m_nCurGameRound = 0;

    /// <summary>
    /// 底分
    /// </summary>
    uint m_nDiFenValue = 0;

    /// <summary>
    /// 红方阵营服务端座位号
    /// </summary>
    byte m_nRedCamepPlayerSSit = 0;

    /// <summary>
    /// 移动棋子是否移动完成
    /// </summary>
    bool m_bPieceMoveComplete = true;

    /// <summary>
    /// 退出约据弹窗状态
    /// </summary>
    bool bExitAppointmentDialogState = false;

    /// <summary>
    /// 游戏AB资源包
    /// </summary>
    public AssetBundle m_ChessAssetBundle = null;

    /// <summary>
    /// 游戏公用资源包
    /// </summary>
    public AssetBundle m_ChessCommonAssetBundle = null;

    /// <summary>
    /// 吃或将动画
    /// </summary>
    private DragonBones.UnityArmatureComponent m_ChiJiangAnimatonComponent = null;

    /// <summary>
    /// 出牌先后手动画
    /// </summary>
    private DragonBones.UnityArmatureComponent m_XianHouShouAnimationComponent = null;

    /// <summary>
    /// 绝杀动画
    /// </summary>
    private DragonBones.UnityArmatureComponent m_JueShaAnimationComponent = null;

    /// <summary>
    /// 游戏结果动画
    /// </summary>
    private DragonBones.UnityArmatureComponent m_GameReslutAnimationComponent = null;


    /// <summary>
    /// 游戏画布对象
    /// </summary>
    public UnityEngine.Canvas GameCanvas = null;

    /// <summary>
    /// 游戏场景挂载对象
    /// </summary>
    Transform m_RootTransform = null;

    /// <summary>
    /// 游戏主场景UI对象
    /// </summary>
    Transform m_MainUITransform = null;

    /// <summary>
    /// 象棋棋盘对象
    /// </summary>
    Transform m_CheckerboardTranform = null;

    /// <summary>
    /// 左边玩家信息面板对象
    /// </summary>
    Transform m_PlayerLeftTranform = null;

    /// <summary>
    /// 右边玩家信息版本对象
    /// </summary>
    Transform m_PlayerRightTranform = null;

    /// <summary>
    /// 游戏结算对象
    /// </summary>
    Transform m_ResultMainTransform = null;

    /// <summary>
    /// 设置界面对象
    /// </summary>
    Transform m_SetMainTransform = null;

    /// <summary>
    /// 棋子移动对象
    /// </summary>
    Image m_PieceMoveObjectImage = null;

    /// <summary>
    /// 悔棋按钮对象
    /// </summary>
    Button m_RegretChessButton = null;

    /// <summary>
    /// 求和按钮对象
    /// </summary>
    Button m_DrawButton = null;

    /// <summary>
    /// 认输按钮对象
    /// </summary>
    Button m_GiveUpButton = null;

    /// <summary>
    /// 离开游戏
    /// </summary>
    Button m_LeaveGameButton = null;

    /// <summary>
    /// 再来一局
    /// </summary>
    Button m_AgainGameButton = null;

    /// <summary>
    /// 约据准备界面
    /// </summary>
    GameObject m_AppointmentReadyPanel = null;

    /// <summary>
    /// 约据准备阶段退出按钮对象
    /// </summary>
    Button m_AppointmentReadyExitButton = null;

    /// <summary>
    /// 约据游戏规则界面
    /// </summary>
    GameObject m_AppointmentRulePanel = null;

    /// <summary>
    /// 约据游戏总结算界面
    /// </summary>
    GameObject m_AppointmentResultPanel = null;

    /// <summary>
    /// 约据游戏中退出按钮对象
    /// </summary>
    Button m_AppointmentGameExitButton = null;

    /// <summary>
    /// 房间状态
    /// </summary>
    private EChessRoomState m_eChessRoomState = EChessRoomState.ChessRoomState_Init;

    /// <summary>
    /// 象棋玩家对象列表
    /// </summary>
    /// <param name="gameMode"></param>
    List<CChess_Role> m_PlayerList = new List<CChess_Role>();

    /// <summary>
    /// 象棋可移动位置标识对象
    /// </summary>
    List<GameObject> m_PieceMoveSignList = new List<GameObject>();

    /// <summary>
    /// 棋盘所有棋子对象
    /// </summary>
    Transform[,] m_ChessCheckerboardPieceList = new Transform[9,10];

    public bool PiecePostionSwapState { get { return m_bPiecePostionSwapState; } }

    public byte AskPlayerServerSit { get { return m_nCurAskPlayerSSit;} }

    public uint TotalPlayChessTime { get { return m_nTotalPlayChessTime; } }

    public uint PlayChessTime { get { return m_nPlayChessTime; } }

    public byte RedCampPlayerSSit { get { return m_nRedCamepPlayerSSit; } }

    public bool PieceMoveComplete { get { return m_bPieceMoveComplete; } }

    public CGame_Chess(GameTye_Enum gameMode) : base(GameKind_Enum.GameKind_Chess)
    {
        GameMode = gameMode;
        m_bReconnected = false;

        InitMsgHandle();
    }

    /// <summary>
    /// 象棋消息
    /// </summary>
    private void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_CHOOSElEVEL, HandChooseLevelNetMsg);             //匹配房间数据
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER, HandLeaveRoomNetMsg);        //离开房间消息回复
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_ROOMSTATE, HandRoomStateNetMsg);                 //同步房间状态到客户端
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_ENTERROOM, HandEnterRoomNetMsg);                 //进入房间成功
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_DEALBEGIN, HandCheckerboardNetMsg);              //初始化棋盘
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_ASKDEAL, HandPieceAskQuestionNetMsg);            //提问玩家
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_ANSWERDOING, HandPieceAnswerQuestionNetMsg);     //同步玩家回答
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_PUBLISHRESULT, HandGameResultNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_RequestDraw, HandRequestDrawNetMsg);                //求和
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_RequestDrawResult, HandRequestDrawResultNetMsg);    //求和结果
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_MIDDLEENTERROOM, HandMiddleEnterRoomNetMsg);     //中途加入
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_CChess_SM_AFTERONLOOKERENTER,HandBystanderEnterNetMsg);    //围观的玩家进入房间
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
            MatchRoom.GetInstance().AddDesk(deskNum, 2);
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
        byte state = _ms.ReadByte();
        OnStateChange((EChessRoomState)state, _ms);
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
        if (GameMode == GameTye_Enum.GameType_Contest)
        {
            MatchInGame.GetInstance().ResetGui();
        }

        uint roomId = _ms.ReadUInt();        byte roomLevel = _ms.ReadByte();        InitGameRoomInfo(_ms);        byte roomState = _ms.ReadByte();        m_PlayerList[0].m_nSSit = _ms.ReadByte();        m_PlayerList[0].m_nTotalCoin = _ms.ReadLong();
        InitRoleLocalInfo();        sbyte otherSit = _ms.ReadSByte();        if(otherSit >= 0)
        {
            m_PlayerList[1].m_nSSit = (byte)otherSit;
            m_PlayerList[1].m_nUseId = _ms.ReadUInt();
            m_PlayerList[1].m_nFaceId = _ms.ReadInt();
            m_PlayerList[1].m_sUrl = _ms.ReadString();
            m_PlayerList[1].m_nTotalCoin = _ms.ReadLong();
            m_PlayerList[1].m_sRoleName = _ms.ReadString();
            m_PlayerList[1].m_nDisconnectState = _ms.ReadByte();
            float MasterScore = _ms.ReadSingle();
            m_PlayerList[1].m_nSex = _ms.ReadByte();
            m_PlayerList[1].TotalPlayChessTime = TotalPlayChessTime;
            m_PlayerList[1].RefreshInfoUI();
        }
        uint contestID = _ms.ReadUInt();//比赛ID
        uint appointmentID = _ms.ReadUInt();//约据ID
        UpdateAppointentRuleInfoText();

        EnterGameRoom(roomLevel);
        DebugLog.Log("Start Chess game, otherNum:");
        return true;
    }

    /// <summary>
    /// 初始化象棋棋盘。
    /// </summary>
    /// <param name="_msgType">消息类型</param>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool HandCheckerboardNetMsg(uint _msgType, UMessage _ms)
    {
        ShowChessButtonActive(true);
        m_nRedCamepPlayerSSit = _ms.ReadByte();//红方人的座位号
        m_bPiecePostionSwapState = m_PlayerList[0].m_nSSit != m_nRedCamepPlayerSSit;
        InitChessCheckerboardData(_ms);
        if (m_XianHouShouAnimationComponent)
        {
            GameMain.SC(PlayAnimation(m_XianHouShouAnimationComponent, m_bPiecePostionSwapState ? "duifangxianshou" : "huodexianshou"));
        }

        DebugLog.Log("初始化棋盘 红方阵营座位号:" + m_nRedCamepPlayerSSit + " 自己座位号:" + m_PlayerList[0].m_nSSit);
        return true;
    }

    /// <summary>
    /// 提问玩家
    /// </summary>
    /// <param name="_msgType"></param>
    /// <param name="_ms"></param>
    /// <returns></returns>
    bool HandPieceAskQuestionNetMsg(uint _msgType, UMessage _ms)
    {
        m_nCurAskPlayerSSit =  _ms.ReadByte();
        m_CurAskQuestionSign = _ms.ReadUInt();
        InitPlayerAskQuestion(m_nPlayChessTime);
        DebugLog.Log("提问:m_CurAskQuestionSign = " + m_nCurAskPlayerSSit + " time =" + m_nPlayChessTime + " m_CurAskQuestionSign =" + m_CurAskQuestionSign);
        return true;
    }

    /// <summary>
    /// 同步玩家回答
    /// </summary>
    /// <param name="_msgType"></param>
    /// <param name="_ms"></param>
    /// <returns></returns>
    bool HandPieceAnswerQuestionNetMsg(uint _msgType, UMessage _ms)
    {
        //0:成功1:此答非所问2:异常状态 11:越界(位置非法)12:此棋数据异常
        //13:不该你下棋14:此处不能落子 15:送将不能走 16:没有应将17:长将需要变招18:长捉需要变招 
        //19：将捉交替次数超限需要变招//21:无吃子将军着数累计到20步将判和//22：无吃子着数达到120步将判和
        //23：双方互相长将6次将判和 //24：双方互相长捉6次将判和//25：非将非应将非捉非应捉局面重复5次将判和
        byte state =  _ms.ReadByte();
        if (state != 0)
        {
            if(state >= 12)
            {
                uint TipsId = 26000 + (uint)state;
                CRollTextUI.Instance.AddVerticalRollText(TipsId);
            }
            Debug.Log("答复失败: " + state);
            return false;
        }
        if(m_PlayerList.Count == 0)
        {
            Debug.Log("玩家数据为空");
            return false;
        }
        ResetPieceTransportableSigns();
        sbyte pieceType = _ms.ReadSByte();//棋子类型
        byte pieceX = _ms.ReadByte();//棋子X坐标
        byte pieceY = _ms.ReadByte();//棋子Y坐标
        sbyte deadPieceType = _ms.ReadSByte();//死亡棋子类型
        EGeneralType generalType = (EGeneralType)_ms.ReadSByte();//0:没有将军1:将军2:将死3:憋死对方
        uint gameTime = _ms.ReadUInt();//一局剩余时间

        GameMain.SC(MoveChessPieceEnumerator(pieceType, pieceX, pieceY, deadPieceType, generalType, gameTime));
        return true;
    }

    /// <summary>
    /// 游戏结算
    /// </summary>
    /// <param name="_msgType"></param>
    /// <param name="_ms"></param>
    /// <returns></returns>
    private bool HandGameResultNetMsg(uint _msgType, UMessage _ms)
    {
        sbyte winState = _ms.ReadSByte();//胜利玩家座位号，-1:平局
        Dictionary<byte, long> GameResultDataDictionary = new Dictionary<byte, long>();
        byte roleNum = _ms.ReadByte();
        for(byte index = 0; index < roleNum; ++index)
        {
            byte playerSit = _ms.ReadByte();//座位号
            uint playerId = _ms.ReadUInt(); //玩家ID
            long totalCoin = _ms.ReadLong();//剩余钱数
            long CoinValue = _ms.ReadLong();//本局改变值
            GameResultDataDictionary.Add(playerSit, CoinValue);
            CChess_Role ChessPlayer =  m_PlayerList.Find(player => player.m_nSSit == playerSit);
            if(ChessPlayer != null)
            {
                ChessPlayer.m_nTotalCoin = totalCoin;
                ChessPlayer.PlayChessTime = 0;
            }
        }
        m_nCurGameRound = _ms.ReadSByte();//第几局
        byte gameNum = _ms.ReadByte();//总局
        long vedioId = _ms.ReadLong();
        GameMain.SC(ShowGameResult(winState, m_nCurGameRound == gameNum, vedioId, GameResultDataDictionary));
        return true;
    }

    /// <summary>
    /// 求和
    /// </summary>
    /// <param name="_msgType"></param>
    /// <param name="_ms"></param>
    /// <returns></returns>
    bool HandRequestDrawNetMsg(uint _msgType, UMessage _ms)
    {
        float Time = _ms.ReadSingle();
        CCustomDialog.OpenCustomDialogWithTimer(26002, (uint)Time, (param) =>
        {
            int isagree = (int)param;
            ReqestDrawResultToServerMsg((byte)(isagree > 0 ? 1:0));
        });
        return true;
    }

    /// <summary>
    /// 求和结果
    /// </summary>
    /// <param name="_msgType"></param>
    /// <param name="_ms"></param>
    /// <returns></returns>
    bool HandRequestDrawResultNetMsg(uint _msgType, UMessage _ms)
    {
        byte State = _ms.ReadByte();//0次数不够1:拒绝2:成功（关闭窗口）

        if(State < 2)
        {
            uint Id = 26003 + (uint)State;
            CCustomDialog.OpenCustomConfirmUI(Id);
            if(m_DrawButton && State != 0 && m_nPlayerDrawCount > 0)
            {
                SetButtonInteractable(m_DrawButton);
            }
            SetButtonInteractable(m_GiveUpButton);
        }
        return true;
    }

    /// <summary>
    /// 中途加入
    /// </summary>
    /// <param name="type"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    private bool HandMiddleEnterRoomNetMsg(uint type, UMessage msg)
    {
        byte state = msg.ReadByte();//中途加入状态
        if(state == 0)
        {
            return false;
        }
        ResetGameData();
        EChessPlayerState playerState = (EChessPlayerState)msg.ReadByte();

        DebugLog.Log("Middle enter room playerState:" + playerState);

        if (playerState == EChessPlayerState.ChessPlayerState_OnGameButIsOver)
        {            CCustomDialog.OpenCustomConfirmUI(1621, (param) =>
            { 
                BackToGameRoom();
            });
            return false;
        }

        if (playerState == EChessPlayerState.ChessPlayerState_ReadyHall)
        {            MatchRoom.GetInstance().OnEnd();
            return false;
        }

        uint roomId = msg.ReadUInt();//房间ID
        byte roomLevel = msg.ReadByte();//房间等级

        //游戏房间规则
        InitGameRoomInfo(msg);

        GameMain.hall_.CurRoomIndex = roomLevel;
        Dictionary<byte, AppointmentRecordPlayer> recordPalyerDictionary = new Dictionary<byte, AppointmentRecordPlayer>();

        m_PlayerList[0].m_nSSit = msg.ReadByte();//自己座位号
        m_PlayerList[0].m_nTotalCoin = msg.ReadLong();
        m_PlayerList[0].m_nReady = msg.ReadByte();//是否准备1:准备0:否
        InitRoleLocalInfo();
        m_PlayerList[0].TotalPlayChessTime = msg.ReadUInt();
        m_nPlayerDrawCount = msg.ReadByte();
        m_nPlayerDrawCount = Mathf.Clamp(2 - m_nPlayerDrawCount, 0, 2);
        m_nRedCamepPlayerSSit = msg.ReadByte();//红方座位号
        m_bPiecePostionSwapState = m_PlayerList[0].m_nSSit != m_nRedCamepPlayerSSit;
        m_nCurAskPlayerSSit =msg.ReadByte();
        float time = msg.ReadSingle();
        RefreshPlayerDrawCount();
        ShowChessButtonActive(true);
        if (m_PlayerList[0].m_nSSit == m_nCurAskPlayerSSit)
        {
            SetButtonInteractable(m_DrawButton, true);
            SetButtonInteractable(m_GiveUpButton, true);
        }

        sbyte otherSit = msg.ReadSByte();
        if(otherSit >= 0)
        {
            m_PlayerList[1].m_nSSit = (byte)otherSit;
            m_PlayerList[1].m_nUseId = msg.ReadUInt();
            m_PlayerList[1].m_nFaceId = msg.ReadInt();
            m_PlayerList[1].m_sUrl = msg.ReadString();
            m_PlayerList[1].m_nTotalCoin = msg.ReadLong();
            m_PlayerList[1].m_sRoleName = msg.ReadString();
            m_PlayerList[1].m_nDisconnectState = msg.ReadByte();
            float MasterScore = msg.ReadSingle();
            m_PlayerList[1].m_nSex = msg.ReadByte();
            m_PlayerList[1].m_nReady = msg.ReadByte();//是否准备1:准备0:否
            m_PlayerList[1].TotalPlayChessTime = msg.ReadUInt();
            m_PlayerList[1].RefreshInfoUI();
        }

        m_nCurGameRound = msg.ReadByte();//当前第几场
        byte maxGameRound = msg.ReadByte();//这一轮有多少场
        uint contestId = msg.ReadUInt();//比赛ID
        uint appointId = msg.ReadUInt();//约据ID

        InitPlayerAskQuestion(time);
        m_eChessRoomState = (EChessRoomState)msg.ReadByte();//房间状态
        switch(m_eChessRoomState)
        {
            case EChessRoomState.ChessRoomState_Ask:
            case EChessRoomState.ChessRoomState_OnceBeginShow:
                m_CurAskQuestionSign = msg.ReadUInt();
                InitChessCheckerboardData(msg);
                InitChessPieceMovePostionSign(msg);
                break;
            case EChessRoomState.ChessRoomState_OnceEnd:
            case EChessRoomState.ChessRoomState_OnceResult:
                HandGameResultNetMsg(type, msg);
                break;
        }

        foreach(var player in m_PlayerList)
        {
            AppointmentRecordPlayer recordPalyerInfo = new AppointmentRecordPlayer();
            player.GetRecordPlyerInfo(ref recordPalyerInfo);
            recordPalyerDictionary.Add(player.m_nSSit, recordPalyerInfo);
        }

        if (GameMode == GameTye_Enum.GameType_Normal)
        {
            if (playerState == EChessPlayerState.ChessPlayerState_OnDesk)
                MatchRoom.GetInstance().ShowTable(true, recordPalyerDictionary, roomLevel, roomId);
            else
                MatchRoom.GetInstance().StartGame();
        }

        EnterGameRoom(roomLevel);

        if (appointId > 0)
        {
            ChessAppointmentData data = (ChessAppointmentData)GameFunction.CreateAppointmentData(GetGameType());
            data.roomid = appointId;
            data.playtimes = maxGameRound;
            data.maxpower = 250;
            data.ChessTime = m_nTotalPlayChessTime;
            AppointmentDataManager.AppointmentDataInstance().currentRoomID = appointId;
            AppointmentDataManager.AppointmentDataInstance().AddAppointmentData(appointId, data);

            GameMain.hall_.InitRoomsData();
            UpdateAppointentRuleInfoText();
            UpdateAppointmentRuleText(m_nCurGameRound);
        }
        return true;
    }

    /// <summary>
    /// 游戏旁观进入
    /// </summary>
    /// <param name="type"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    private bool HandBystanderEnterNetMsg(uint type, UMessage msg)
    {
        uint roomId = msg.ReadUInt();//房间ID
        byte roomLevel = msg.ReadByte();//房间等级
        ResetGameData();
        //游戏房间规则
        InitGameRoomInfo(msg);
        byte curRound = msg.ReadByte();
        byte maxRound = msg.ReadByte();
        uint contestId = msg.ReadUInt();//比赛ID
        m_nRedCamepPlayerSSit = msg.ReadByte();//红方座位号
        m_nCurAskPlayerSSit = msg.ReadByte();
        float time = msg.ReadSingle();
        byte roleNum = msg.ReadByte();
        for(byte index = 0; index < roleNum;++index)
        {
            byte roleServetSit = msg.ReadByte();
            m_PlayerList[roleServetSit].m_nCSit = roleServetSit;
            m_PlayerList[roleServetSit].m_nSSit = roleServetSit;
            m_PlayerList[roleServetSit].m_nUseId = msg.ReadUInt();
            m_PlayerList[roleServetSit].m_nFaceId = msg.ReadInt();
            m_PlayerList[roleServetSit].m_sUrl = msg.ReadString();
            m_PlayerList[roleServetSit].m_nTotalCoin = msg.ReadLong();
            m_PlayerList[roleServetSit].m_sRoleName = msg.ReadString();
            m_PlayerList[roleServetSit].m_nDisconnectState = msg.ReadByte();
            float MasterScore = msg.ReadSingle();
            m_PlayerList[roleServetSit].m_nSex = msg.ReadByte();
            m_PlayerList[roleServetSit].m_nReady = msg.ReadByte();//是否准备1:准备0:否
            m_PlayerList[roleServetSit].TotalPlayChessTime = msg.ReadUInt();
            m_PlayerList[roleServetSit].RefreshInfoUI();
        }

        m_bPiecePostionSwapState = m_PlayerList[0].m_nSSit != m_nRedCamepPlayerSSit;
        InitPlayerAskQuestion(time);
        m_eChessRoomState = (EChessRoomState)msg.ReadByte();//房间状态
        switch (m_eChessRoomState)
        {
            case EChessRoomState.ChessRoomState_Ask:
            case EChessRoomState.ChessRoomState_OnceBeginShow:
                InitChessCheckerboardData(msg);
                InitChessPieceMovePostionSign(msg);
                break;
            case EChessRoomState.ChessRoomState_OnceEnd:
            case EChessRoomState.ChessRoomState_OnceResult:
                HandGameResultNetMsg(type, msg);
                break;
        }
        EnterGameRoom(roomLevel, true);
        if (GameMode == GameTye_Enum.GameType_Contest)
        {
            MatchInGame.GetInstance().ShowBegin(Bystander, curRound, maxRound);
        }
        return true;
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
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_CChess_CM_CHOOSElEVEL);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(GameMain.hall_.CurRoomIndex);
        HallMain.SendMsgToRoomSer(msg);
    }

    /// <summary>
    /// 回答服务器提问
    /// </summary>
    public void AnswerServerAskQuestionToServerMsg(sbyte PieceType,int PiecePostion)
    {
        byte PieceX = 0, PieceY = 0;
        GetPieceServerPostion(PiecePostion, out PieceX, out PieceY);
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_CChess_CM_ANSWERDOING);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(m_CurAskQuestionSign);
        msg.Add(PieceType);
        msg.Add(PieceX);
        msg.Add(PieceY);
        HallMain.SendMsgToRoomSer(msg);
    }

    /// <summary>
    /// 申请认输
    /// </summary>
    void ReqestGiveUpToServerMsg()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_CChess_GiveUp);
        msg.Add(GameMain.hall_.GetPlayerId());
        HallMain.SendMsgToRoomSer(msg);
    }

    /// <summary>
    /// 申请求和
    /// </summary>
    void ReqestDrawToServerMsg()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_CChess_RequestDraw);
        msg.Add(GameMain.hall_.GetPlayerId());
        HallMain.SendMsgToRoomSer(msg);
    }


    /// <summary>
    /// 申请求和结果
    /// </summary>
    void ReqestDrawResultToServerMsg(byte Reslutt)
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_CChess_RequestDrawResult);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(Reslutt);
        HallMain.SendMsgToRoomSer(msg);
    }

    /// <summary>
    /// 离开旁观的游戏房间
    /// </summary>
    void OnLeaveBystanderGameRoom()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_CChess_CM_LEAVEONLOOKERROOM);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((uint)0);
        HallMain.SendMsgToRoomSer(msg);

    }

    /// <summary>
    /// 同步房间信息
    /// </summary>
    /// <param name="_ms">消息数据对象</param>
    /// <returns></returns>
    bool InitGameRoomInfo(UMessage _ms)
    {
        m_nTotalPlayChessTime = _ms.ReadUInt();
        m_nPlayChessTime = _ms.ReaduShort();
        m_nDiFenValue = _ms.ReadUInt();//底分
        UpdateRoomRuleData();
        return true;
    }

    /// <summary>
    /// 初始化主控角色基本信息
    /// </summary>
    void InitRoleLocalInfo()
    {
        PlayerData playerData = GameMain.hall_.GetPlayerData();        m_PlayerList[0].m_sRoleName = playerData.GetPlayerName();        m_PlayerList[0].m_nUseId = playerData.GetPlayerID();        m_PlayerList[0].m_nFaceId = (int)playerData.PlayerIconId;        m_PlayerList[0].m_sUrl = playerData.GetPlayerIconURL();        m_PlayerList[0].m_nSex = playerData.PlayerSexSign;        m_PlayerList[0].m_nDisconnectState = 1;
        m_PlayerList[0].TotalPlayChessTime = TotalPlayChessTime;
        m_PlayerList[0].RefreshInfoUI();    }

    /// <summary>
    /// 初始化棋盘数据
    /// </summary>
    /// <param name="_ms"></param>
    private void InitChessCheckerboardData(UMessage _ms)
    {
        byte chessNum = _ms.ReadByte();
        for (byte index = 0; index < chessNum; ++index)
        {
            sbyte chessPiecesType = _ms.ReadSByte();
            byte chessPiecesX = _ms.ReadByte();
            byte chessPiecesY = _ms.ReadByte();
            int playerClientSit = GetPlayerClientSitByPieceType(chessPiecesType);
            m_PlayerList[playerClientSit].AddChessPiecesData((EChessPiecesType)Mathf.Abs(chessPiecesType), chessPiecesX, chessPiecesY);
        }
        RefreshChessCheckerboard();
    }

    /// <summary>
    /// 初始化棋盘上一步棋子移动标识
    /// </summary>
    /// <param name="msg"></param>
    private void InitChessPieceMovePostionSign(UMessage msg)
    {
        sbyte pieceType = msg.ReadSByte();//棋子类型
        if (pieceType != 0)
        {
            byte pieceX = msg.ReadByte();//棋子X坐标
            byte pieceY = msg.ReadByte();//棋子Y坐标
            int playerClientSit = GetPlayerClientSitByPieceType(pieceType);
            EChessPiecesType CurPieceType = (EChessPiecesType)Mathf.Abs(pieceType);
            if (m_PlayerList[playerClientSit].GetPieceClientPostion(CurPieceType, out m_nLastTargetPiecePostion))
            {
                GetPieceClientPostion(pieceX, pieceY, out m_nLastSourcePiecePostion);
                RefreshMovePieceSigns(m_nLastSourcePiecePostion, m_nLastTargetPiecePostion);
            }
        }
    }

    /// <summary>
    /// 初始化玩家下棋步数据
    /// </summary>
    /// <param name="time"></param>
    private void InitPlayerAskQuestion(float time)
    {
        float playerChessTimer = 0;
        foreach(var Player in m_PlayerList)
        {
            playerChessTimer = 0;
            if (Player.m_nSSit == m_nCurAskPlayerSSit)
            {
                playerChessTimer = time;
                bool bInteractableState = Player.m_nCSit == 0;
                SetButtonInteractable(m_RegretChessButton, bInteractableState);
                SetButtonInteractable(m_DrawButton, bInteractableState && m_nPlayerDrawCount > 0);
                SetButtonInteractable(m_GiveUpButton, bInteractableState);
            }
            Player.PlayChessTime = playerChessTimer;
        }
    }

    /// <summary>
    /// 更新房间规则
    /// </summary>
    void UpdateRoomRuleData()
    {
        if (m_MainUITransform == null)
        {
            return;
        }
        Text ruleText = m_MainUITransform.Find("Top/ImageBetBG/TextBetNum").GetComponent<Text>();
        ruleText.text = m_nDiFenValue.ToString();
    }

    /// <summary>
    /// 游戏初始函数
    /// </summary>
    public override void Initialization()
    {
        base.Initialization();

        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameKind_Enum.GameKind_Chess, "GameChessAudioCsv");

        //请求进入房间
        ReqestChooseLevelToServerMsg();

        if (!m_bReconnected)
        {
            switch (GameMode)
            {
                case GameTye_Enum.GameType_Normal:
                    m_nWaitingLoad = -1;
                    MatchRoom.GetInstance().ShowRoom((byte)GameType);
                    break;
            }        }
        LoadChessResource();
    }

    /// <summary>
    /// 初始化游戏对象
    /// </summary>
    void InitPlayers()
    {
        CChess_Role role = new CCHess_RoleLocal(this, 0);
        m_PlayerList.Add(role);
        role = new CChess_RoleOther(this, 1);
        m_PlayerList.Add(role);
    }

    /// <summary>
    /// 进入游戏房间
    /// </summary>
    /// <param name="roomLevel">房间等级</param>
    /// <param name="bystander">旁观状态</param>
    void EnterGameRoom(byte roomLevel, bool bystander = false)
    {
        DebugLog.Log("进入房间：" + roomLevel + " " + bystander);
        CustomAudioDataManager.GetInstance().PlayAudio(1000, false);
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
            if (!m_bReconnected)
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
        }
        else
        {
            LoadAppointmentResource();
        }
    }

    /// <summary>
    /// 加载主场景资源
    /// </summary>
    private void LoadMainResource()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((int)GameKind_Enum.GameKind_Chess);
        if (gamedata == null)
            return;

        m_ChessAssetBundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
        if (m_ChessAssetBundle == null)
            return;

        m_ChessCommonAssetBundle = AssetBundleManager.GetAssetBundle("pokercommon.resource");        if (m_ChessCommonAssetBundle == null)            return;

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
            m_MainUITransform = ((GameObject)GameMain.instantiate(m_ChessAssetBundle.LoadAsset("Chess_MainUI"))).transform;
            m_MainUITransform.SetParent(m_RootTransform, false);
            m_MainUITransform.Find("Top/ButtonReturn").gameObject.SetActive(false);
            m_RegretChessButton = m_MainUITransform.Find("Buttons/ButtonTakeBack").GetComponent<Button>();
            XPointEvent.AutoAddListener(m_RegretChessButton.gameObject, OnRegretChessButtonClickEvent, null);
            m_DrawButton = m_MainUITransform.Find("Buttons/ButtonDraw").GetComponent<Button>();
            XPointEvent.AutoAddListener(m_DrawButton.gameObject, OnDrawButtonClickEvent, null);
            m_GiveUpButton = m_MainUITransform.Find("Buttons/ButtonGiveIn").GetComponent<Button>();
            XPointEvent.AutoAddListener(m_GiveUpButton.gameObject, OnGiveUpButtonClickEvent, null);
            SetButtonInteractable(m_RegretChessButton, false);
            SetButtonInteractable(m_DrawButton, false);
            SetButtonInteractable(m_GiveUpButton, false);
            ShowChessButtonActive(false);
            m_MainUITransform.Find("Top/ImageBetBG").gameObject.SetActive(GameMode == GameTye_Enum.GameType_Normal);
        }

        if (m_PieceMoveObjectImage == null)
        {
            m_PieceMoveObjectImage = ((GameObject)GameMain.instantiate(m_ChessAssetBundle.LoadAsset("Chessman_1"))).GetComponent<Image>();
            m_PieceMoveObjectImage.transform.SetParent(m_MainUITransform.Find("ImageCheckerboard"),false);
            m_PieceMoveObjectImage.gameObject.SetActive(false);
        }

        if (m_CheckerboardTranform == null)
        {
            m_CheckerboardTranform = m_MainUITransform.Find("ImageCheckerboard/ImageGrid");
        }

        if (m_PlayerLeftTranform == null)
        {
            m_PlayerLeftTranform = m_MainUITransform.Find("PlayerInfor_Left");
        }

        if (m_PlayerRightTranform == null)
        {
            m_PlayerRightTranform = m_MainUITransform.Find("PlayerInfor_Right");
        }

        if(m_ResultMainTransform == null)
        {
            m_ResultMainTransform = ((GameObject)GameMain.instantiate(m_ChessAssetBundle.LoadAsset("Chess_Result"))).transform;
            m_ResultMainTransform.SetParent(m_RootTransform, false);
            m_LeaveGameButton = m_ResultMainTransform.Find("ResultBG/Button_Exit").GetComponent<Button>();
            XPointEvent.AutoAddListener(m_LeaveGameButton.gameObject, OnLeaveGameButtonClickEvent, null);
            m_AgainGameButton = m_ResultMainTransform.Find("ResultBG/Button_Again").GetComponent<Button>();
            m_ResultMainTransform.gameObject.SetActive(false);
        }

        Transform animationTootTransform = m_MainUITransform.Find("Pop-up/anime/point_00");
        if (m_ChiJiangAnimatonComponent == null)
        {
            m_ChiJiangAnimatonComponent = LoadArmatureCoponent("anime_chi_jiang", animationTootTransform);
        }
       
        if(m_XianHouShouAnimationComponent == null)
        {
            m_XianHouShouAnimationComponent = LoadArmatureCoponent("anime_xian_hou", animationTootTransform);
        }

        if (m_JueShaAnimationComponent == null)
        {
            m_JueShaAnimationComponent = LoadArmatureCoponent("anime_sha", animationTootTransform);
        }

        if (m_GameReslutAnimationComponent == null && m_ResultMainTransform)
        {
            animationTootTransform = m_ResultMainTransform.Find("Animation");
            m_GameReslutAnimationComponent = LoadArmatureCoponent("anime_ying_shu_ping", animationTootTransform);
        }
       
        //设置界面
        if (m_SetMainTransform == null)
        {
            //设置按钮
            Transform expandTransform = m_MainUITransform.Find("Top/ButtonSet");
            XPointEvent.AutoAddListener(expandTransform.gameObject, OnClickSetButtonEvent, true);

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

        m_MainUITransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// 加载约据资源
    /// </summary>
    void LoadAppointmentResource()
    {
        AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (assetBundle == null)
            return;

        if (m_AppointmentRulePanel == null && m_RootTransform != null)
        {
            m_AppointmentRulePanel = (GameObject)GameMain.instantiate(assetBundle.LoadAsset("Room_process_Sp"));
            m_AppointmentRulePanel.transform.SetParent(m_RootTransform, false);
            m_AppointmentGameExitButton = m_AppointmentRulePanel.transform.Find("Top/ButtonReturn").GetComponent<Button>();
            m_AppointmentGameExitButton.interactable = true;
            m_AppointmentGameExitButton.onClick.AddListener(() => { m_AppointmentGameExitButton.interactable = false; OnLeaveAppointmentRoomEvent(); });
            XPointEvent.AutoAddListener(m_AppointmentRulePanel.transform.Find("ImageLeftBG").gameObject, OnClickPromptInofPanel, null);
        }

        if (m_AppointmentReadyPanel == null)
        {
            m_AppointmentReadyPanel = GameMain.hall_.gamerooms_.LoadAppintmentReadResource("Room_Ready_Sp");
            if (m_AppointmentReadyPanel != null)
            {
                m_AppointmentReadyExitButton = m_AppointmentReadyPanel.transform.Find("Top/Button_Return").GetComponent<Button>();
                m_AppointmentReadyExitButton.interactable = true;
                m_AppointmentReadyExitButton.onClick.AddListener(() => { m_AppointmentReadyExitButton.interactable = false; OnLeaveAppointmentRoomEvent(); });
                m_AppointmentReadyPanel.transform.SetAsLastSibling();
            }
        }

        if (m_AppointmentResultPanel == null && m_RootTransform != null)
        {
            m_AppointmentResultPanel = (GameObject)GameMain.instantiate(assetBundle.LoadAsset("Room_Result_Sp"));
            m_AppointmentResultPanel.transform.SetParent(m_RootTransform, false);
            m_AppointmentResultPanel.SetActive(false);
            XPointEvent.AutoAddListener(m_AppointmentResultPanel.transform.Find("ImageBG/Buttonclose").gameObject, OnCloseAppointmentResultPanel, null);
        }
    }

    /// <summary>
    /// 加载动画资源
    /// </summary>
    /// <param name="animationName"></param>
    /// <param name="AnimationRootTransform"></param>
    /// <returns></returns>
    private DragonBones.UnityArmatureComponent LoadArmatureCoponent(string animationName,Transform AnimationRootTransform)
    {
        if(m_ChessAssetBundle == null)
        {
            return null;
        }
        UnityEngine.Object AnimatonObject = m_ChessAssetBundle.LoadAsset(animationName);
        GameObject ArmatureComponent = (GameObject)GameMain.instantiate(AnimatonObject);
        ArmatureComponent.transform.SetParent(AnimationRootTransform, false);
        ArmatureComponent.SetActive(false);
        return ArmatureComponent.GetComponent<DragonBones.UnityArmatureComponent>();
    }

    /// <summary>
    /// 加载棋子资源
    /// </summary>
    private void LoadPieceResource()
    {
        if(m_CheckerboardTranform  == null || m_PlayerList.Count == 0)
        {
            return;
        }

        int PieceY = 0, PieceX = 0;
        CCHess_RoleLocal PlayerLocale = (CCHess_RoleLocal)m_PlayerList[0];
        UnityEngine.Object pieceObject = m_ChessAssetBundle.LoadAsset("Chessman");
        for (int index = 0; index < m_CheckerboardTranform.childCount; ++index)
        {
            PieceY = index / 9;
            PieceX = index - PieceY * 9;
            int PostionValue = PieceX * 100 + PieceY;
            Transform PieceTransform = ((GameObject)GameMain.instantiate(pieceObject)).transform;
            PieceTransform.SetParent(m_CheckerboardTranform.GetChild(index), false);
            m_ChessCheckerboardPieceList[PieceX, PieceY] = PieceTransform;
            XPointEvent.AutoAddListener(PieceTransform.gameObject, PlayerLocale.OnChessCheckerboardClickEvent, PostionValue);
        }
    }

    /// <summary>
    /// 加载象棋资源
    /// </summary>
    private bool LoadChessResource()
    {
        if (m_PlayerList.Count == 0)
        {            if (m_nWaitingLoad > 0)                m_nWaitingLoad--;            if (m_nWaitingLoad != 0)                return false;
        }        else            return true;

        LoadMainResource();
        InitPlayers();
        LoadPieceResource();
        EnterGameRoom(GameMode == GameTye_Enum.GameType_Normal ?  GameMain.hall_.CurRoomIndex:(byte)1);
        switch (GameMode)
        {
            case GameTye_Enum.GameType_Normal:
                MatchRoom.GetInstance().SetUIAsLast();
                break;
            case GameTye_Enum.GameType_Appointment:
                GameMain.hall_.gamerooms_.SetUIAsLast();
                break;
        }
        return true;
    }

    /// <summary>
    /// 游戏逻辑推进
    /// </summary>
    public override void ProcessTick()
    {
        base.ProcessTick();

        //游戏断线重连
        if (m_bReconnected)
        {
            GameMain.hall_.OnGameReconnect(GameType, GameMode);
            m_bReconnected = false;
        }

        if(!LoadChessResource())
        {
            return;
        }


        foreach (var Role in m_PlayerList)
        {
            Role.OnTick();
        }

        if (GameMode == GameTye_Enum.GameType_Appointment &&
            m_eChessRoomState == EChessRoomState.ChessRoomState_TotalEnd &&
            AppointmentDataManager.AppointmentDataInstance().interrupt)
        {
            ShowAppointmentTotalResult();
        }
    }

    /// <summary>
    /// 重置游戏UI
    /// </summary>
    public override void ResetGameUI()
    {
        base.ResetGameUI();
    }

    /// <summary>
    /// 与游戏服务器重连成功
    /// </summary>
    public override void ReconnectSuccess()
    {
        m_bReconnected = true;
    }

    public override void StartLoad()
    {
        m_nWaitingLoad = 0;
        ResetGameData();
    }

    /// <summary>
    /// 同步玩家离线状态
    /// </summary>
    /// <param name="disconnect">离线标志</param>
    /// <param name="userId">离线玩家ID</param>
    /// <param name="sit">离线玩家座位号</param>
    public override void OnPlayerDisOrReconnect(bool disconnect, uint userId, byte sit)
    {
        CChess_Role roleObject = m_PlayerList.Find(role => { return role.m_nUseId == userId; });
        if (roleObject != null)
        {
            roleObject.m_nDisconnectState = (byte)(disconnect ? 0 : 1);
            roleObject.RefreshRoleOfflineUI();
        }
        if (GameMode == GameTye_Enum.GameType_Normal)
        {
            MatchRoom.GetInstance().SetPlayerOffline(sit, disconnect);
        }
    }

    /// <summary>
    /// 与游戏服务器网络断开连接
    /// </summary>
    public override void OnDisconnect(bool over = true)
    {
        GameOver();
    }

    /// <summary>
    /// 退出游戏房间
    /// </summary>
    void BackToGameRoom()    {        switch (GameMode)
        {
            case GameTye_Enum.GameType_Normal:
                MatchRoom.GetInstance().GameOver();
                break;
            case GameTye_Enum.GameType_Appointment:
                GameMain.hall_.SwitchToHallScene(true, 0);
                break;
            default:
                GameMain.hall_.SwitchToHallScene();
                break;
        }    }

    /// <summary>
    /// 判断当前房间状态
    /// </summary>
    /// <param name="roomState">房间状态</param>
    /// <returns>true: 是 false: 不是</returns>
    public bool IsGameRoomState(EChessRoomState roomState)
    {
        return m_eChessRoomState == roomState;
    }

    /// <summary>
    /// 获得房间状态
    /// </summary>
    /// <returns></returns>
    public EChessRoomState GetRoomState()
    {
        return m_eChessRoomState;
    }

    /// <summary>
    /// 象棋按钮显示状态
    /// </summary>
    /// <param name="activeState"></param>
    private void ShowChessButtonActive(bool activeState)
    {
        if(m_DrawButton)
        {
            m_DrawButton.gameObject.SetActive(activeState);
        }
        if(m_GiveUpButton)
        {
            m_GiveUpButton.gameObject.SetActive(activeState);
        }
    }

    /// <summary>
    /// 结算界面按钮禁用状态
    /// </summary>
    /// <param name="interactableState">ture:按钮可用，false:不可用</param>
    void SetResultButtonInteractable(bool LeaveInteractable, bool AgainInteractable = false)
    {
        if (m_LeaveGameButton)
        {
            m_LeaveGameButton.interactable = LeaveInteractable;
        }

        if (m_AgainGameButton)
        {
            m_AgainGameButton.interactable = AgainInteractable;
        }
    }

    /// <summary>
    /// 房间状态切换
    /// </summary>
    /// <param name="state">新的房间状态</param>
    /// <param name="_ms"></param>
    /// <param name="mode"> 0:normal 1:reconnect 2:bystander</param>
    /// <param name="timeLeft"></param>
    void OnStateChange(EChessRoomState state, UMessage _ms, byte mode = 0,float timeLeft = 0f)    {        if (m_eChessRoomState !=EChessRoomState.ChessRoomState_WaitReady &&            m_eChessRoomState == state && mode == 0)            return;        DebugLog.Log(string.Format("room state change: ({0}->{1})", m_eChessRoomState, state));        OnQuitState(m_eChessRoomState);        m_eChessRoomState = state;        OnEnterState(m_eChessRoomState, _ms, mode, timeLeft);    }

    /// <summary>
    /// 退出房间状态
    /// </summary>
    /// <param name="state">当前房间状态</param>
    void OnQuitState(EChessRoomState state)    {        switch (state)        {
            case EChessRoomState.ChessRoomState_Init:
                break;
        }
    }

    /// <summary>
    /// 进入房间状态
    /// </summary>
    /// <param name="state">新的房间状态</param>
    /// <param name="_ms"></param>
    /// <param name="mode"> 0:normal 1:reconnect 2:bystander</param>
    /// <param name="timeLeft"></param>
    void OnEnterState(EChessRoomState state, UMessage _ms, byte mode,float timeLeft = 0f)    {
        switch (state)
        {
            case EChessRoomState.ChessRoomState_WaitPlayer://等人
                {
                    if (GameMode == GameTye_Enum.GameType_Normal)
                    {
                        MatchRoom.GetInstance().ShowKickTip(false);
                    }
                }
                break;
            case EChessRoomState.ChessRoomState_WaitReady://等待准备
                {
                    if (GameMode == GameTye_Enum.GameType_Normal)
                    {
                        float time = _ms.ReadSingle();
                        MatchRoom.GetInstance().ShowKickTip(true, time);//准备倒计时开启
                    }
                }
                break;
            case EChessRoomState.ChessRoomState_OnceBeginShow://每局开始前的显示
                {
                    m_nPlayerDrawCount = 2;
                    RefreshPlayerDrawCount();
                    ResetGameData();
                    UpdateAppointmentRuleText(m_nCurGameRound + 1);
                    if (GameMode == GameTye_Enum.GameType_Normal)
                    {
                        MatchRoom.GetInstance().StartGame();
                    }
                    else if (GameMode == GameTye_Enum.GameType_Contest)
                    {
                        MatchInGame.GetInstance().ShowBegin(Bystander);
                    }
                }
                break;

            case EChessRoomState.ChessRoomState_OnceEnd:
                {
                    SetResultButtonInteractable(true, false);
                }
                break;
            case EChessRoomState.ChessRoomState_TotalEnd:
                {
                    SetResultButtonInteractable(true,true);
                }
                break;
        }
    }

    /// <summary>
    /// 获得角色信息面板对象
    /// </summary>
    /// <param name="playerClientSit">客户端座位号</param>
    /// <returns></returns>
    public Transform GetPlayerInfoTranform(int playerClientSit)
    {
        return playerClientSit == 0 ? m_PlayerRightTranform : m_PlayerLeftTranform;
    }

    /// <summary>
    /// 获得角色棋盘面板对象
    /// </summary>
    /// <returns></returns>
    public Transform GetPlayerCheckerboardTranform()
    {
        return m_CheckerboardTranform;
    }

    /// <summary>
    /// 获得玩家客户端座位号
    /// </summary>
    /// <param name="PieceType"></param>
    /// <returns></returns>
    public int GetPlayerClientSitByPieceType(sbyte PieceType)
    {
       return PiecePostionSwapState ? (PieceType > 0 ? 1 : 0) : (PieceType > 0 ? 0 : 1);
    }

    /// <summary>
    /// 获取棋子资源
    /// </summary>
    /// <param name="ServerPlayerSit"></param>
    /// <param name="pieceType"></param>
    /// <param name="SelectState"></param>
    /// <returns></returns>
    public Sprite GetPieceSprite(byte ServerPlayerSit, EChessPiecesType PieceType,bool SelectState = false)
    {
        if(m_ChessAssetBundle == null)
        {
            return null;
        }
        StringBuilder ImageName = new StringBuilder(SelectState ? "w_0" : "q_0");
        ImageName.Append(ServerPlayerSit == RedCampPlayerSSit ? 1 : 2);
        switch (PieceType)
        {
            case EChessPiecesType.CChessType_Soldier_1:
            case EChessPiecesType.CChessType_Soldier_2:
            case EChessPiecesType.CChessType_Soldier_3:
            case EChessPiecesType.CChessType_Soldier_4:
            case EChessPiecesType.CChessType_Soldier_5:
                ImageName.Append("_01");
                break;
            case EChessPiecesType.CChessType_Cannon_1:
            case EChessPiecesType.CChessType_Cannon_2:
                ImageName.Append("_02");
                break;
            case EChessPiecesType.CChessType_Rook_1:
            case EChessPiecesType.CChessType_Rook_2:
                ImageName.Append("_03");
                break;
            case EChessPiecesType.CChessType_Horse_1:
            case EChessPiecesType.CChessType_Horse_2:
                ImageName.Append("_04");
                break;
            case EChessPiecesType.CChessType_Elephant_1:
            case EChessPiecesType.CChessType_Elephant_2:
                ImageName.Append("_05");
                break;
            case EChessPiecesType.CChessType_Guard_1:
            case EChessPiecesType.CChessType_Guard_2:
                ImageName.Append("_06");
                break;
            case EChessPiecesType.CChessType_General:
                ImageName.Append("_07");
                break;
        }
        return m_ChessAssetBundle.LoadAsset<Sprite>(ImageName.ToString());
    }

    /// <summary>
    /// 获得棋子服务端位置
    /// </summary>
    /// <param name="PieceType"></param>
    /// <param name="PieceX"></param>
    /// <param name="PieceY"></param>
    /// <returns></returns>
    public void GetPieceServerPostion(int PiecePostion, out byte PieceX, out byte PieceY)
    {
        PieceX = (byte)(PiecePostion / 100);
        PieceY = (byte)(PiecePostion % 100);
        if (PiecePostionSwapState)
        {
            PieceX = (byte)(8 - PieceX);
            PieceY = (byte)(9 - PieceY);
        }
    }

    /// <summary>
    /// 获得棋子客户端端位置
    /// </summary>
    /// <param name="PieceType"></param>
    /// <param name="PieceX"></param>
    /// <param name="PieceY"></param>
    /// <returns></returns>
    public void GetPieceClientPostion(byte PieceX,byte PieceY,out int PiecePostion)
    {
        if (PiecePostionSwapState)
        {
            PieceX = (byte)(8 - PieceX);
            PieceY = (byte)(9 - PieceY);
        }
        PiecePostion = PieceX * 100 + PieceY;
    }

    /// <summary>
    /// 获取棋盘中对应的棋子对象
    /// </summary>
    /// <param name="Postion"></param>
    /// <returns></returns>
    public Transform GetPieceTransform(int Postion)
    {
        int PieceX = Postion / 100;
        int PieceY = Postion % 100;
        if(PieceX < 0 || PieceX >= 9 || PieceY < 0 || PieceY >= 10)
        {
            return null;
        }
        return m_ChessCheckerboardPieceList[PieceX, PieceY];
    }

    /// <summary>
    /// 刷新象棋棋盘界面
    /// </summary>
    public void RefreshChessCheckerboard()
    {
        foreach (var player in m_PlayerList)
        {
            player.RefreshPlayerCheckerboard();
        }
    }

    /// <summary>
    /// 刷新上一步棋子行走标志
    /// </summary>
    /// <param name="SourcePiecePostion"></param>
    /// <param name="TargetPiecePostion"></param>
    /// <param name="Active"></param>
    public void RefreshMovePieceSigns(int SourcePiecePostion , int TargetPiecePostion, bool Active = true)
    {
        Transform pieceTransform = null;
        pieceTransform = GetPieceTransform(SourcePiecePostion);
        if (pieceTransform != null)
        {
            pieceTransform.Find("Image_point (2)").gameObject.SetActive(Active);
        }
        pieceTransform = GetPieceTransform(TargetPiecePostion);
        if (pieceTransform != null)
        {
            pieceTransform.Find("Image_outline").gameObject.SetActive(Active);
        }
    }

    /// <summary>
    /// 判断当前棋盘中此位置是否存在棋子
    /// </summary>
    /// <param name="PiecePostion"></param>
    /// <returns></returns>
    public bool IsCheckerboardPostionPieceType(int PiecePostion)
    {
        EChessPiecesType PieceType = EChessPiecesType.CChessType_None;
        foreach(var Player in m_PlayerList)
        {
            PieceType = Player.GetChessPieceTypeByPostion(PiecePostion);
            if(PieceType != EChessPiecesType.CChessType_None)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 判断是否在棋盘九宫格内
    /// </summary>
    /// <param name="PiecePostion"></param>
    /// <returns></returns>
    private bool IsCheckerboardNinthHouseState(int PieceX,int PieceY)
    {
        return PieceX >= 3 && PieceX <= 5 && PieceY >= 7 && PieceY <= 9;
    }

    /// <summary>
    /// 显示棋子可行走标记
    /// </summary>
    /// <param name="PiecePostion"></param>
    /// <returns></returns>
    private bool ShowPieceTransportableSigns(int PiecePostion)
    {
        Transform PieceTransform = GetPieceTransform(PiecePostion);
        if (PieceTransform && !IsCheckerboardPostionPieceType(PiecePostion))
        {
            GameObject  PieceSignObject = PieceTransform.Find("Image_point (1)").gameObject;
            PieceSignObject.SetActive(true);
            m_PieceMoveSignList.Add(PieceSignObject);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 重置已经显示棋子可行走标记
    /// </summary>
    private void ResetPieceTransportableSigns()
    {
        foreach (var SingObject in m_PieceMoveSignList)
        {
            SingObject.SetActive(false);
        }
        m_PieceMoveSignList.Clear();
    }

    /// <summary>
    /// 显示将(帅)或者仕(士)棋子可行走标记
    /// </summary>
    /// <param name="PieceX"></param>
    /// <param name="PieceY"></param>
    private void ShowGeneralPieceTransportableSigns(int PieceX,int PieceY)
    {
        if (IsCheckerboardNinthHouseState(PieceX, PieceY))
        {
            int PiecePostion = PieceX * 100 + PieceY;
            if (!IsCheckerboardPostionPieceType(PiecePostion))
            {
                ShowPieceTransportableSigns(PiecePostion);
            }
        }
    }

    /// <summary>
    /// 显示象(相)棋子可行走标记
    /// </summary>
    /// <param name="PieceX"></param>
    /// <param name="PieceY"></param>
    private void ShowElephantPieceTransportableSigns(int PieceX, int PieceY)
    {
        int NewPieceY = 0;
        int PiecePostion = (PieceX + 1) * 100 + (PieceY + 1);
        if (!IsCheckerboardPostionPieceType(PiecePostion))
        {
            PiecePostion = (PieceX + 2) * 100 + (PieceY + 2);
            ShowPieceTransportableSigns(PiecePostion);
        }
        PiecePostion = (PieceX + 1) * 100 + (PieceY - 1);
        if (!IsCheckerboardPostionPieceType(PiecePostion))
        {
            NewPieceY = PieceY - 2;
            if(NewPieceY >= 5)
            {
                PiecePostion = (PieceX + 2) * 100 + NewPieceY;
                ShowPieceTransportableSigns(PiecePostion);
            }
        }
        PiecePostion = (PieceX - 1) * 100 + (PieceY + 1);
        if (!IsCheckerboardPostionPieceType(PiecePostion))
        {
            PiecePostion = (PieceX - 2) * 100 + (PieceY + 2);
            ShowPieceTransportableSigns(PiecePostion);
        }
        PiecePostion = (PieceX - 1) * 100 + (PieceY - 1);
        if (!IsCheckerboardPostionPieceType(PiecePostion))
        {
            NewPieceY = PieceY - 2;
            if(NewPieceY >= 5)
            {
                PiecePostion = (PieceX - 2) * 100 + NewPieceY;
                ShowPieceTransportableSigns(PiecePostion);
            }
        }
    }

    /// <summary>
    /// 显示马(相)棋子可行走标记
    /// </summary>
    /// <param name="PieceX"></param>
    /// <param name="PieceY"></param>
    private void ShowHorsePieceTransportableSigns(int PieceX, int PieceY)
    {
        int PiecePostion = (PieceX + 1) * 100 + PieceY;
        if (!IsCheckerboardPostionPieceType(PiecePostion))
        {
            PiecePostion = (PieceX + 2) * 100 + (PieceY + 1);
            ShowPieceTransportableSigns(PiecePostion);
            PiecePostion = (PieceX + 2) * 100 + (PieceY - 1);
            ShowPieceTransportableSigns(PiecePostion);
        }

        PiecePostion = (PieceX - 1) * 100 + PieceY;
        if (!IsCheckerboardPostionPieceType(PiecePostion))
        {
            PiecePostion = (PieceX - 2) * 100 + (PieceY + 1);
            ShowPieceTransportableSigns(PiecePostion);
            PiecePostion = (PieceX - 2) * 100 + (PieceY - 1);
            ShowPieceTransportableSigns(PiecePostion);
        }

        PiecePostion = PieceX * 100 + (PieceY + 1);
        if (!IsCheckerboardPostionPieceType(PiecePostion))
        {
            PiecePostion = (PieceX + 1) * 100 + (PieceY + 2);
            ShowPieceTransportableSigns(PiecePostion);
            PiecePostion = (PieceX - 1) * 100 + (PieceY + 2);
            ShowPieceTransportableSigns(PiecePostion);
        }

        PiecePostion = PieceX * 100 + (PieceY - 1);
        if (!IsCheckerboardPostionPieceType(PiecePostion))
        {
            PiecePostion = (PieceX + 1) * 100 + (PieceY - 2);
            ShowPieceTransportableSigns(PiecePostion);
            PiecePostion = (PieceX - 1) * 100 + (PieceY - 2);
            ShowPieceTransportableSigns(PiecePostion);
        }
    }

    /// <summary>
    /// 显示炮或车棋子可行走标记
    /// </summary>
    /// <param name="PieceX"></param>
    /// <param name="PieceY"></param>
    private void ShowCannorOrRookPieceTransportableSigns(int PieceX, int PieceY)
    {
        int PiecePostion = 0;
        for (int X = PieceX + 1; X < 9; ++X)
        {
            PiecePostion = X * 100 + PieceY;
            if (!ShowPieceTransportableSigns(PiecePostion))
            {
                break;
            }
        }
        for (int X = PieceX - 1; X >= 0; --X)
        {
            PiecePostion = X * 100 + PieceY;
            if (!ShowPieceTransportableSigns(PiecePostion))
            {
                break;
            }
        }
        for (int Y = PieceY + 1; Y < 10; ++Y)
        {
            PiecePostion = PieceX * 100 + Y;
            if (!ShowPieceTransportableSigns(PiecePostion))
            {
                break;
            }
        }
        for (int Y = PieceY - 1; Y >= 0; --Y)
        {
            PiecePostion = PieceX * 100 + Y;
            if (!ShowPieceTransportableSigns(PiecePostion))
            {
                break;
            }
        }
    }

    public void RefreshPieceTransportableSigns(EChessPiecesType PieceType,int PiecePostion)
    {
        int PieceX = PiecePostion / 100;
        int PieceY = PiecePostion % 100;
        ResetPieceTransportableSigns();
        switch (PieceType)
        {
            case EChessPiecesType.CChessType_Soldier_1:
            case EChessPiecesType.CChessType_Soldier_2:
            case EChessPiecesType.CChessType_Soldier_3:
            case EChessPiecesType.CChessType_Soldier_4:
            case EChessPiecesType.CChessType_Soldier_5:
                {
                    PiecePostion = PieceX * 100 + (PieceY - 1);
                    ShowPieceTransportableSigns(PiecePostion);
                    if (PieceY <= 4)
                    {
                        PiecePostion = (PieceX - 1) * 100 + PieceY;
                        ShowPieceTransportableSigns(PiecePostion);
                        PiecePostion = (PieceX + 1) * 100 + PieceY;
                        ShowPieceTransportableSigns(PiecePostion);
                    }
                }
                break;
            case EChessPiecesType.CChessType_Cannon_1:
            case EChessPiecesType.CChessType_Cannon_2:
            case EChessPiecesType.CChessType_Rook_1:
            case EChessPiecesType.CChessType_Rook_2:
                {
                    ShowCannorOrRookPieceTransportableSigns(PieceX, PieceY);
                }
                break;
            case EChessPiecesType.CChessType_Horse_1:
            case EChessPiecesType.CChessType_Horse_2:
                {
                    ShowHorsePieceTransportableSigns(PieceX, PieceY);
                }
                break;
            case EChessPiecesType.CChessType_Elephant_1:
            case EChessPiecesType.CChessType_Elephant_2:
                {
                    ShowElephantPieceTransportableSigns(PieceX, PieceY);
                }
                break;
            case EChessPiecesType.CChessType_Guard_1:
            case EChessPiecesType.CChessType_Guard_2:
                {
                    ShowGeneralPieceTransportableSigns(PieceX + 1, PieceY + 1);
                    ShowGeneralPieceTransportableSigns(PieceX + 1, PieceY - 1);
                    ShowGeneralPieceTransportableSigns(PieceX - 1, PieceY + 1);
                    ShowGeneralPieceTransportableSigns(PieceX - 1, PieceY - 1);
                }
                break;
            case EChessPiecesType.CChessType_General:
                {
                    ShowGeneralPieceTransportableSigns(PieceX - 1, PieceY);
                    ShowGeneralPieceTransportableSigns(PieceX + 1, PieceY);
                    ShowGeneralPieceTransportableSigns(PieceX, PieceY + 1);
                    ShowGeneralPieceTransportableSigns(PieceX, PieceY - 1);
                }
                break;
        }
    }


    /// <summary>
    /// 刷新求和次数
    /// </summary>
    private void RefreshPlayerDrawCount()
    {
        if(m_DrawButton == null)
        {
            return;
        }
        m_DrawButton.transform.Find("Text").GetComponent<Text>().text = m_nPlayerDrawCount.ToString();
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="ArmatureComponent"></param>
    /// <param name="AnimationName"></param>
    /// <param name="AutoActive"></param>
    /// <param name="timeScale"></param>
    /// <returns></returns>
    private IEnumerator PlayAnimation(DragonBones.UnityArmatureComponent ArmatureComponent,string AnimationName = null,bool AutoActive = true,float timeScale = 1f)
    {
        if(ArmatureComponent == null)
        {
            yield break;
        }
        ArmatureComponent.animation.Play(AnimationName);
        ArmatureComponent.gameObject.SetActive(true);
        if(AutoActive)
        {
            yield return new WaitForSecondsRealtime(timeScale);
            ArmatureComponent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="onceEnd">true:一局结束 false: 整个游戏结束</param>
    public void GameOver(bool onceEnd = true,bool resetState = true)
    {
        ResetGameData(resetState);
        ResetGameUI();

        if (Bystander && onceEnd)
        {
            Bystander = false;
            Debug.Log("离开房间");
            OnLeaveBystanderGameRoom();
        }
    }
    /// <summary>
    /// 重置游戏数据
    /// </summary>
    public void ResetGameData(bool resetState = true)
    {
        if(resetState)
        {
            for (int columnIndex = 0; columnIndex < 9; ++columnIndex)
            {
                for (int rowIndex = 0; rowIndex < 10; ++rowIndex)
                {
                    if (m_ChessCheckerboardPieceList[columnIndex, rowIndex])
                    {
                        for (int index = 0; index < m_ChessCheckerboardPieceList[columnIndex, rowIndex].childCount; ++index)
                        {
                            m_ChessCheckerboardPieceList[columnIndex, rowIndex].GetChild(index).gameObject.SetActive(false);
                        }
                    }
                }
            }

            foreach (var role in m_PlayerList)
            {
                role.ResetRoleData();
            }
            m_PieceMoveSignList.Clear();
            m_nLastSourcePiecePostion = 0;
            m_nLastTargetPiecePostion = 0;
        }
        
        SetResultButtonInteractable(false);
        if (m_ResultMainTransform)
        {
            m_ResultMainTransform.gameObject.SetActive(false);
        }
        if(m_GameReslutAnimationComponent)
        {
            m_GameReslutAnimationComponent.gameObject.SetActive(false);
        }
        if(m_ChiJiangAnimatonComponent)
        {
            m_ChiJiangAnimatonComponent.gameObject.SetActive(false);
        }
        if(m_XianHouShouAnimationComponent)
        {
            m_XianHouShouAnimationComponent.gameObject.SetActive(false);
        }
        if(m_JueShaAnimationComponent)
        {
            m_JueShaAnimationComponent.gameObject.SetActive(false);
        }
        ShowChessButtonActive(false);
        if (m_PieceMoveObjectImage)
        {
            RefreshPlayerSelectPiece(false);
            DOTween.Kill(m_PieceMoveObjectImage.transform);
        }
        GameMain.Instance.StopAllCoroutines();
    }

    /// <summary>
    /// 设置按钮是否可以点击
    /// </summary>
    /// <param name="ButtonObject"></param>
    /// <param name="Interactable"></param>
    private void SetButtonInteractable(Button ButtonObject,bool Interactable = true)
    {
        if(ButtonObject)
        {
            ButtonObject.interactable = Interactable;
        }
    }

    /// <summary>
    /// 棋子移动效果
    /// </summary>
    /// <param name="pieceType"></param>
    /// <param name="pieceX"></param>
    /// <param name="pieceY"></param>
    /// <param name="deadPieceType"></param>
    /// <param name="generalType"></param>
    /// <param name="gameTime"></param>
    /// <param name="timeScale"></param>
    /// <param name="reverse"></param>
    /// <returns></returns>
    IEnumerator MoveChessPieceEnumerator(sbyte pieceType,byte pieceTX,byte pieceTY,sbyte deadPieceType,EGeneralType generalType,uint gameTime,
                                         float timeScale = 1.0f,bool reverse = false,byte pieceSX = 0,byte peiceSY = 0)
    {
        int playerClientSit = GetPlayerClientSitByPieceType(pieceType);
        int deadPlayerCSit = GetPlayerClientSitByPieceType(deadPieceType);
        m_PlayerList[playerClientSit].TotalPlayChessTime = gameTime;
        m_PlayerList[playerClientSit].PlayChessTime = 0;
        EChessPiecesType CurPieceType = (EChessPiecesType)Mathf.Abs(pieceType);
        RefreshMovePieceSigns(m_nLastSourcePiecePostion, m_nLastTargetPiecePostion, false);
        EChessPiecesType CurDeadPieceType = (EChessPiecesType)Mathf.Abs(deadPieceType);
        if (reverse && CurDeadPieceType != EChessPiecesType.CChessType_None)
        {
            m_PlayerList[deadPlayerCSit].AddChessPiecesData(CurDeadPieceType, pieceTX, pieceTY);
            m_PlayerList[deadPlayerCSit].RefreshPlayerPiece(CurDeadPieceType);
        }
        else
        {
            m_PlayerList[deadPlayerCSit].RemoveChessPiecesData((EChessPiecesType)Mathf.Abs(deadPieceType));
            m_PlayerList[playerClientSit].RefreshPlayerPiece(CurPieceType, false);
        }
        if(reverse)
        {
            pieceTX = pieceSX;
            pieceTY = peiceSY;
        }
        bool PiecePostionState = m_PlayerList[playerClientSit].GetPieceClientPostion(CurPieceType, out m_nLastSourcePiecePostion);
        if (m_PieceMoveObjectImage)
        {
            int TargetPostion = 0;
            m_bPieceMoveComplete = false;
            Transform SourcePieceTransform = GetPieceTransform(m_nLastSourcePiecePostion);
            GetPieceClientPostion(pieceTX, pieceTY, out TargetPostion);
            Transform TargetPieceTransform = GetPieceTransform(TargetPostion);
            Sprite PieceSprite = m_PlayerList[playerClientSit].GetPieceSprite(CurPieceType, true);
            RefreshPlayerSelectPiece(true, PieceSprite, SourcePieceTransform.position);
            bool distanceState = (TargetPieceTransform.position - SourcePieceTransform.position).sqrMagnitude > 4;
            m_PieceMoveObjectImage.transform.DOMove(TargetPieceTransform.position, timeScale * (distanceState ? 0.5f:0.15f)).OnComplete(() =>
            {
                m_PlayerList[playerClientSit].AddChessPiecesData(CurPieceType, pieceTX, pieceTY);
                m_PlayerList[playerClientSit].RefreshPlayerPiece(CurPieceType);
                if (PiecePostionState && !reverse &&
                    m_PlayerList[playerClientSit].GetPieceClientPostion(CurPieceType, out m_nLastTargetPiecePostion))
                {
                    RefreshMovePieceSigns(m_nLastSourcePiecePostion, m_nLastTargetPiecePostion);
                }
                RefreshPlayerSelectPiece(false);
                int AudioID = 0;
                string animationName = string.Empty;

                if ((generalType == EGeneralType.eCChessJS_JiangSi ||
                    generalType == EGeneralType.eCChessJS_BieSi) && m_JueShaAnimationComponent)
                {
                    GameMain.SC(PlayAnimation(m_JueShaAnimationComponent,null,true,timeScale));
                }
                else
                {
                    if (deadPieceType != 0)
                    {
                        AudioID = 1008;
                        animationName = "chi";
                    }
                    if (generalType == EGeneralType.eCChessJS_Jiang)
                    {
                        AudioID = 1009;
                        animationName = "jiang";
                    }
                    if (AudioID > 0)
                    {
                        CustomAudioDataManager.GetInstance().PlayAudio(AudioID);
                    }
                    if (m_ChiJiangAnimatonComponent && !string.IsNullOrEmpty(animationName))
                    {
                        GameMain.SC(PlayAnimation(m_ChiJiangAnimatonComponent, animationName, true, timeScale));
                    }
                }
                CustomAudioDataManager.GetInstance().PlayAudio(1003);
                m_bPieceMoveComplete = true;
            });
        }
        yield break;
    }

    /// <summary>
    /// 刷新玩家选中棋子对象
    /// </summary>
    /// <param name="PieceSprite"></param>
    /// <param name="PiecePostion"></param>
    /// <param name="Active"></param>
    public void RefreshPlayerSelectPiece(bool Active , Sprite PieceSprite = null, Vector3 PiecePostion = default(Vector3))
    {
        if(m_PieceMoveObjectImage == null)
        {
            return;
        }

        m_PieceMoveObjectImage.sprite = PieceSprite;
        m_PieceMoveObjectImage.transform.position = PiecePostion;
        m_PieceMoveObjectImage.gameObject.SetActive(Active);
    }

    /// <summary>
    /// 显示游戏结算
    /// </summary>
    /// <param name="winState"></param>
    /// <param name="TotalEnd"></param>
    /// <param name="GameResultDictionary"></param>
    /// <returns></returns>
    IEnumerator ShowGameResult(sbyte winState,bool TotalEnd,long VedioId, Dictionary<byte,long> GameResultDictionary)
    {
        if (m_ResultMainTransform == null)
        {
            Debug.Log("结算面板资源找不到!");
            yield break;
        }
        yield return new WaitForSecondsRealtime(1f);
        int SoundID = 1007;
        string resultName = string.Empty;
        if(winState == -1)
        {
            resultName = "heqi";
        }
        else
        {
            bool SelfWinState = m_PlayerList[0].m_nSSit == winState;
            SoundID = SelfWinState ? 1005 : 1004;
            resultName = SelfWinState ? "yingle":"shule";
        }
 
        CustomAudioDataManager.GetInstance().PlayAudio(SoundID);

        switch(GameMode)
        {
            case GameTye_Enum.GameType_Contest:
                {
                    //录像记录
                    AppointmentRecord recordData = null;
                    if (TotalEnd && !Bystander)
                    {
                        recordData = new AppointmentRecord();
                        recordData.gameID = (byte)GameKind_Enum.GameKind_Chess;
                        recordData.gamerule = "象棋比赛 第" + MatchInGame.GetInstance().m_nCurTurn.ToString() + "轮";
                        recordData.timeseconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);
                        recordData.isopen = false;
                        recordData.videoes = VedioId;
                        recordData.recordTimeSeconds = GameCommon.ConvertDataTimeToLong(System.DateTime.Now);
                    }

                    List<RoundResultData> roleRoundResultList = new List<RoundResultData>();
                    AppointmentRecordPlayer recordPlayerdata = null;
                    for (int index = 0; index < m_PlayerList.Count; index++)
                    {
                        if (!GameResultDictionary.ContainsKey(m_PlayerList[index].m_nSSit))
                        {
                            roleRoundResultList.Add(null);
                            continue;
                        }

                        RoundResultData resultData = new RoundResultData();
                        resultData.headImg = m_PlayerList[index].GetHeadImg();
                        resultData.coin = m_PlayerList[index].m_nTotalCoin;
                        resultData.name = m_PlayerList[index].GetRoleName();
                        resultData.addCoin = GameResultDictionary[m_PlayerList[index].m_nSSit];
                        roleRoundResultList.Add(resultData);

                        if (recordData != null)
                        {
                            recordPlayerdata = new AppointmentRecordPlayer();
                            m_PlayerList[index].GetRecordPlyerInfo(ref recordPlayerdata);
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

                    yield return MatchInGame.GetInstance().ShowRoundResult(m_PlayerList.Count, roleRoundResultList, () =>
                    {
                        GameOver(TotalEnd,false);
                    }, TotalEnd, Bystander);
                }
                break;
            default:
                {

                    if (m_GameReslutAnimationComponent)
                    {
                        GameMain.SC(PlayAnimation(m_GameReslutAnimationComponent, resultName, false));
                    }
                    m_ResultMainTransform.SetAsLastSibling();
                    if(m_AgainGameButton)
                    {
                        m_AgainGameButton.gameObject.SetActive(false);
                    }
                    if(m_LeaveGameButton)
                    {
                        m_LeaveGameButton.gameObject.SetActive(false);
                    }
                    m_ResultMainTransform.gameObject.SetActive(true);
                    yield return new WaitForSecondsRealtime(1f);

                    string UIName = string.Empty;
                    foreach (var Player in m_PlayerList)
                    {
                        UIName = Player.m_nCSit == 0 ? "ResultBG/Right/" : "ResultBG/Left/";

                        m_ResultMainTransform.Find(UIName + "HeadFram/Image_Mask/Image_Head").GetComponent<Image>().sprite = Player.GetHeadImg();
                        m_ResultMainTransform.Find(UIName + "TextName").GetComponent<Text>().text = Player.GetRoleName();
                        if (GameResultDictionary.ContainsKey(Player.m_nSSit))
                        {
                            m_ResultMainTransform.Find(UIName + "Image_coinframe/Text_Coin").GetComponent<Text>().text = GameResultDictionary[Player.m_nSSit].ToString();
                        }
                    }

                    bool chessRoomState = m_eChessRoomState == EChessRoomState.ChessRoomState_TotalEnd || m_eChessRoomState < EChessRoomState.ChessRoomState_OnceBeginShow;
                    bool LeaveInteractable = chessRoomState || m_eChessRoomState == EChessRoomState.ChessRoomState_OnceEnd;
                    SetResultButtonInteractable(LeaveInteractable, TotalEnd && chessRoomState);
                    if (m_AgainGameButton)
                    {
                        bool bShowTotalState = TotalEnd;
                        if (GameMode == GameTye_Enum.GameType_Appointment)
                        {
                            bShowTotalState = AppointmentDataManager.AppointmentDataInstance().interrupt || TotalEnd;
                        }
                        m_AgainGameButton.gameObject.SetActive(bShowTotalState);
                        XPointEvent.AutoAddListener(m_AgainGameButton.gameObject, OnAgainGameButtonClickEvent, bShowTotalState ? m_AgainGameButton : null);
                    }
                    if(m_LeaveGameButton)
                    {
                        m_LeaveGameButton.gameObject.SetActive(GameMode!= GameTye_Enum.GameType_Appointment);
                    }
                    m_ResultMainTransform.Find("ResultBG").gameObject.SetActive(true);
                }
                break;
        }
    }

    /// <summary>
    /// 显示约据总结算
    /// </summary>
    void ShowAppointmentTotalResult()
    {
        AppointmentDataManager.AppointmentDataInstance().interrupt = false;
        AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().isend = false;

        if (m_AppointmentResultPanel == null)
        {
            return;
        }

        int index = 1;
        CChess_Role curRole = null;
        Transform playerTransform = null, curJiFenOneTransform = null, curJiFenTwoTransform = null;
        Transform resultPlayerTransform = m_AppointmentResultPanel.transform.Find("ImageBG/Imageplayer");
        foreach (var roleData in AppointmentDataManager.AppointmentDataInstance().resultList)
        {
            playerTransform = resultPlayerTransform.Find((index).ToString());
            curRole = m_PlayerList.Find(role => role.m_nUseId == roleData.playerid);
            if (curRole != null)
            {
                playerTransform.Find("Head/HeadMask/ImageHead").GetComponent<Image>().sprite = curRole.GetHeadImg();
                playerTransform.Find("TextName").GetComponent<Text>().text = curRole.GetRoleName();
            }
            curJiFenOneTransform = playerTransform.Find("Text_jifen/TextNum_1");
            curJiFenTwoTransform = playerTransform.Find("Text_jifen/TextNum_2");
            curJiFenOneTransform.gameObject.SetActive(roleData.coin >= 0);
            curJiFenTwoTransform.gameObject.SetActive(roleData.coin < 0);
            if (roleData.coin >= 0)
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
        recordData.gamerule = "象棋 打" + AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment().playtimes.ToString() + "局";
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
                CurObject.transform.Find("Text_ju/Textnum").GetComponent<Text>().text = (index + 1).ToString();


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
    /// 更新约据游戏规则详情信息
    /// </summary>
    void UpdateAppointentRuleInfoText()
    {
        if (m_AppointmentRulePanel == null)
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
    /// 开始播放录像
    /// </summary>
    /// <param name="players"></param>
    public override void SetupVideo(List<AppointmentRecordPlayer> players)
    {
        if (players == null)
            return;

        AppointmentRecordPlayer info = null;
        for (byte sitIndex = 0; sitIndex < players.Count; ++sitIndex)        {            info = players[sitIndex];
            m_PlayerList[sitIndex].m_nUseId = info.playerid;
            m_PlayerList[sitIndex].m_nSex = info.sex;
            m_PlayerList[sitIndex].m_nSSit = sitIndex;
            m_PlayerList[sitIndex].m_nCSit = sitIndex;
            m_PlayerList[sitIndex].m_nFaceId = info.faceid;
            m_PlayerList[sitIndex].m_sRoleName = info.playerName;
            m_PlayerList[sitIndex].m_sUrl = info.url;
            m_PlayerList[sitIndex].m_nDisconnectState = 1;
            m_PlayerList[sitIndex].RefreshInfoUI();        }
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

        DebugLog.Log("Chess OnVideoStep:" + videoAction.vai + " rev:" + reverse);
        List<int> videoDataList = videoAction.list;
        int index = 0;
        m_eChessRoomState = EChessRoomState.ChessRoomState_Init;
        switch (videoAction.vai)
        {
            case VideoActionInfo_Enum.VideoActionInfo_401: //初始化棋盘
                RefreshMovePieceSigns(m_nLastSourcePiecePostion,m_nLastTargetPiecePostion,false);
                m_nTotalPlayChessTime = (uint)videoAction.list[index++];
                m_nPlayChessTime = (uint)videoAction.list[index++];
                m_nRedCamepPlayerSSit = (byte)videoAction.list[index++];//红方人的座位号
                m_bPiecePostionSwapState = m_PlayerList[0].m_nSSit != m_nRedCamepPlayerSSit;
                int chessPieceNum = videoAction.list[index++];
                for (int pieceIndex = 0; pieceIndex < chessPieceNum; ++pieceIndex)
                {
                    sbyte chessPiecesType = (sbyte)videoAction.list[index++];
                    byte chessPiecesX = (byte)videoAction.list[index++];
                    byte chessPiecesY = (byte)videoAction.list[index++];
                    int playerClientSit = GetPlayerClientSitByPieceType(chessPiecesType);
                    m_PlayerList[playerClientSit].AddChessPiecesData((EChessPiecesType)Mathf.Abs(chessPiecesType), chessPiecesX, chessPiecesY);
                }
                RefreshChessCheckerboard();
                if (m_XianHouShouAnimationComponent)
                {
                    float timeScale = GameVideo.GetInstance().m_bPause ? 0.0f : GameVideo.GetInstance().GetStepTime();
                    string animationName = m_bPiecePostionSwapState ? "duifangxianshou" : "huodexianshou";
                    GameMain.SC(PlayAnimation(m_XianHouShouAnimationComponent, animationName, true, timeScale));
                }
                break;
            case VideoActionInfo_Enum.VideoActionInfo_402: //棋子行走
                {
                    sbyte pieceType = (sbyte)videoAction.list[index++];//棋子类型
                    byte pieceSX = (byte)videoAction.list[index++];//棋子X坐标(起点)
                    byte pieceSY = (byte)videoAction.list[index++];//棋子Y坐标(起点)
                    byte pieceTX = (byte)videoAction.list[index++];//棋子X坐标(目标)
                    byte pieceTY = (byte)videoAction.list[index++];//棋子Y坐标(目标)
                    sbyte deadPieceType = (sbyte)videoAction.list[index++];//死亡棋子类型
                    EGeneralType generalType = (EGeneralType)videoAction.list[index++];//0:没有将军1:将军2:将死3:憋死对方
                    uint gameTime = (uint)videoAction.list[index++];//一局剩余时间
                    float timeScale = GameVideo.GetInstance().m_bPause ? 0.0f : GameVideo.GetInstance().GetStepTime();
                    GameMain.SC(MoveChessPieceEnumerator(pieceType, pieceTX, pieceTY, deadPieceType, generalType, gameTime, timeScale, reverse,pieceSX, pieceSY));
                    if(reverse)
                    {
                        --curStep;
                        if(curStep > 0)
                        {
                            videoAction = action[curStep];
                            index = 1;
                            pieceSX = (byte)videoAction.list[index++];//棋子X坐标(起点)
                            pieceSY = (byte)videoAction.list[index++];//棋子Y坐标(起点)
                            pieceTX = (byte)videoAction.list[index++];//棋子X坐标(目标)
                            pieceTY = (byte)videoAction.list[index++];//棋子Y坐标(目标)

                            GetPieceClientPostion(pieceSX, pieceSY, out m_nLastSourcePiecePostion);
                            GetPieceClientPostion(pieceTX, pieceTY, out m_nLastTargetPiecePostion);
                            RefreshMovePieceSigns(m_nLastSourcePiecePostion, m_nLastTargetPiecePostion);
                        }
                        
                    }
                }
                break;
        }
        return true;
    }


    /// <summary>
    /// 重新播放录像
    /// </summary>
    public override void OnVideoReplay()
    {
        GameOver();
    }

    /// <summary>
    /// 离开约据房间
    /// </summary>
    void OnLeaveAppointmentRoomEvent()
    {
        if (AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment() == null)
            return;

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
    }

    /// <summary>
    /// 认输按钮事件
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="message"></param>
    /// <param name="eventData"></param>
    private void OnGiveUpButtonClickEvent(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        if (m_GiveUpButton == null || eventtype != EventTriggerType.PointerClick)
        {
            return;
        }
        if (!m_GiveUpButton.interactable || m_PlayerList[0].m_nSSit != AskPlayerServerSit)
        {
            return;
        }
        CCustomDialog.OpenCustomDialogWithTipsID(26000,(param)=> 
        {
            int select = (int)param;

            if(select != 0)
            {
                SetButtonInteractable(m_GiveUpButton,false);
                SetButtonInteractable(m_DrawButton,false);
                ReqestGiveUpToServerMsg();
            }
        });
        CustomAudioDataManager.GetInstance().PlayAudio(1001);
    }

    /// <summary>
    /// 求和按钮事件
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="message"></param>
    /// <param name="eventData"></param>
    private void OnDrawButtonClickEvent(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        if (m_DrawButton == null || eventtype != EventTriggerType.PointerClick)
        {
            return;
        }
        if (!m_DrawButton.interactable || m_PlayerList[0].m_nSSit != AskPlayerServerSit ||
            m_nPlayerDrawCount <= 0)
        {
            return;
        }
        CCustomDialog.OpenCustomDialogWithTipsID(26001, (param) =>
        {
            int select = (int)param;

            if (select != 0)
            {
                SetButtonInteractable(m_GiveUpButton, false);
                SetButtonInteractable(m_DrawButton, false);
                ReqestDrawToServerMsg();
                --m_nPlayerDrawCount;
                RefreshPlayerDrawCount();
            }
        });
        CustomAudioDataManager.GetInstance().PlayAudio(1001);
    }

    /// <summary>
    /// 悔棋按钮事件
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="message"></param>
    /// <param name="eventData"></param>
    private void OnRegretChessButtonClickEvent(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
    }

    /// <summary>
    /// 结算界面再来一局按钮事件
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="message"></param>
    /// <param name="eventData"></param>
    private void OnAgainGameButtonClickEvent(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        if (m_AgainGameButton == null || eventtype != EventTriggerType.PointerClick)
        {
            return;
        }
        if (!m_AgainGameButton.interactable)
        {
            return;
        }

        if(GameMode == GameTye_Enum.GameType_Appointment)
        {
            ShowAppointmentTotalResult();
            ResetGameData(false);
            CustomAudioDataManager.GetInstance().PlayAudio(1001);
        }
        else
        {
            if (message != null)
            {
                MatchRoom.GetInstance().OnClickReturn(1);
            }
            else
            {
                GameOver(false);
                CustomAudioDataManager.GetInstance().PlayAudio(1001);
            }
        }
        m_AgainGameButton.interactable = false;
    }

    /// <summary>
    /// 结算界面离开游戏
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="message"></param>
    /// <param name="eventData"></param>
    private void OnLeaveGameButtonClickEvent(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        if(m_LeaveGameButton == null || eventtype != EventTriggerType.PointerClick)
        {
            return;
        }
        if(!m_LeaveGameButton.interactable)
        {
            return;
        }
        MatchRoom.GetInstance().OnClickReturn(0);
        m_LeaveGameButton.interactable = false;
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
                m_SetMainTransform.gameObject.SetActive(activeState);
            }            CustomAudioDataManager.GetInstance().PlayAudio(1001);        }    }

    /// <summary>
    /// 约据总结算界面事件
    /// </summary>
    private void OnCloseAppointmentResultPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            m_AppointmentResultPanel.SetActive(false);
            GameOver(false);            BackToGameRoom();
            AppointmentDataManager.AppointmentDataInstance().playerAlready = false;            CustomAudioDataManager.GetInstance().PlayAudio(1001);        }    }

    /// <summary>
    /// 游戏规则信息面板事件
    /// </summary>
    private void OnClickPromptInofPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {
        if (m_AppointmentRulePanel == null ||
            eventtype != EventTriggerType.PointerDown && eventtype != EventTriggerType.PointerUp)        {
            return;
        }
        CustomAudioDataManager.GetInstance().PlayAudio(1001);
        Transform promptInfoTransform = m_AppointmentRulePanel.transform.Find("ImageLeftBG/RuleInfo_BG/Image_info");
        float promptTextHeight = -promptInfoTransform.GetComponent<RectTransform>().sizeDelta.y;
        promptInfoTransform.DOLocalMoveY(eventtype == EventTriggerType.PointerDown ? promptTextHeight : 0, 0.6f);    }
}