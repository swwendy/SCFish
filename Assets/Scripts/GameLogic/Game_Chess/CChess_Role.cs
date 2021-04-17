
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XLua;

/// <summary>
/// 象棋玩家
/// </summary>
[Hotfix]
public class CChess_Role
{
    /// <summary>
    /// 玩家ID
    /// </summary>
    public uint m_nUseId;

    /// <summary>
    /// 玩家图标ID
    /// </summary>
    public int m_nFaceId;

    /// <summary>
    /// 玩家图标网络资源定位符
    /// </summary>    public string m_sUrl;

    /// <summary>
    /// 玩家钻石
    /// </summary>    public long m_nTotalCoin;

    /// <summary>
    /// 玩家名称
    /// </summary>    public string m_sRoleName;

    /// <summary>
    /// 玩家服务端座位号
    /// </summary>    public byte m_nSSit;

    /// <summary>
    /// 玩家客户端座位号
    /// </summary>    public byte m_nCSit;

    /// <summary>
    /// 玩家性别
    /// </summary>    public byte m_nSex = 0;

    /// <summary>
    /// 在线状态1:在线,0:离开
    /// </summary>
    public byte m_nDisconnectState = 0;

    /// <summary>
    /// 是否准备状态
    /// </summary>
    public byte m_nReady;

    /// <summary>
    /// 玩家每步下棋当前时间
    /// </summary>
    private float m_nCurPlayChessTime;

    /// <summary>
    ///玩家总下棋时间
    /// </summary>
    private float m_nTotalPlayChessTime;

    /// <summary>
    ///玩家每步下棋时间
    /// </summary>
    private float m_nPlayChessTime;

    /// <summary>
    /// 棋盘数据
    /// </summary>
    protected Dictionary<EChessPiecesType, int> ChessPiecesDictionary = new Dictionary<EChessPiecesType, int>();

    /// <summary>
    /// 象棋游戏对象
    /// </summary>
    public CGame_Chess m_GameBase = null;

    /// <summary>
    /// 角色信息面板
    /// </summary>
    public UnityEngine.Transform m_RoleInfoTranform = null;

    /// <summary>
    /// 角色棋盘面板
    /// </summary>
    public UnityEngine.Transform m_RoleCheckerboardTranform = null;

    /// <summary>
    /// /玩家每步下棋时间图片显示对象
    /// </summary>
    Image m_PlayChessTimeImage = null;

    /// <summary>
    /// /玩家每步下棋时间文本显示对象
    /// </summary>
    Text m_PlayChessTimeText = null;

    /// <summary>
    /// /玩家总下棋时间文本显示对象
    /// </summary>
    Text m_TotalPlayChessTimeText = null;

    /// <summary>
    /// 玩家步时警告次数
    /// </summary>
    protected int WarningAudioCount =  -1;
    public float PlayChessTime 
    { 
        get
        {
            return m_nCurPlayChessTime;
        }
        set 
        {
            m_nCurPlayChessTime = value;
            if (m_nCurPlayChessTime < 0)
            {
                m_nCurPlayChessTime = 0;
            }
            else
            {
                WarningAudioCount = Mathf.Clamp(Mathf.CeilToInt(m_nCurPlayChessTime),0,9);
            }

            if(m_nCurPlayChessTime == 0)
            {
                WarningAudioCount = -1;
            }

            if(m_nTotalPlayChessTime < m_nCurPlayChessTime)
            {
                m_nCurPlayChessTime = m_nTotalPlayChessTime;
            }
            m_nPlayChessTime = m_nCurPlayChessTime != 0 ? m_nCurPlayChessTime : 30;
            RefreshRoleTimeUI();
        } 
    }

    public float TotalPlayChessTime
    {
        set
        {
            m_nTotalPlayChessTime = value;
            RefreshRoleTimeUI();
        }
    }

    public CChess_Role(CGame_Chess gameBase, byte cSit)    {        m_GameBase = gameBase;
        m_nUseId = 0;
        m_nFaceId = 0;
        m_sUrl = string.Empty;
        m_nTotalCoin = 0;
        m_sRoleName = string.Empty;
        m_nSSit = RoomInfo.NoSit;
        m_nCSit = RoomInfo.NoSit;
        m_nDisconnectState = 0;        m_nCSit = cSit;        m_nReady = 0;        m_nTotalPlayChessTime = 0;        m_nCurPlayChessTime = 0;        m_nPlayChessTime = 1;        ChessPiecesDictionary.Clear();
        m_RoleInfoTranform = gameBase.GetPlayerInfoTranform(cSit);
        m_RoleCheckerboardTranform = gameBase.GetPlayerCheckerboardTranform();
        m_RoleInfoTranform.gameObject.SetActive(false);
    }

    /// <summary>
    /// 重置角色数据
    /// </summary>
    public virtual void ResetRoleData()
    {
        PlayChessTime = 0;
        m_nPlayChessTime = 1;
        ChessPiecesDictionary.Clear();    }

    /// <summary>
    /// 刷新角色信息面板
    /// </summary>
    public void RefreshInfoUI()
    {
        if (m_RoleInfoTranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }

        if (m_GameBase == null)
        {
            Debug.Log("游戏对象为空!<m_GameBase>");
            return;
        }

        m_RoleInfoTranform.Find("TextName").GetComponent<Text>().text = m_sRoleName;

        Transform tfm = m_RoleInfoTranform.Find("Head/HeadMask/ImageHead");        tfm.GetComponent<Image>().sprite = GameMain.hall_.GetIcon(m_sUrl, m_nUseId, m_nFaceId);

        RefreshRoleMoneyUI();
        RefreshRoleOfflineUI();
        RefreshRoleTimeUI();

        m_RoleInfoTranform.gameObject.SetActive(true);
    }

    /// <summary>
    /// 刷新玩家货币(钻石或积分)
    /// </summary>
    public void RefreshRoleMoneyUI()
    {
        if (m_RoleInfoTranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }

        Transform transfrom = m_RoleInfoTranform.Find("Image_coinframe");        if (!m_GameBase.m_bIsFree)        {
            transfrom.gameObject.SetActive(true);
            transfrom.Find("Text_Coin").GetComponent<Text>().text = m_nTotalCoin.ToString();
        }        else
        {
            transfrom.gameObject.SetActive(false);        }
    }

    /// <summary>
    /// 刷新角色离线标志
    /// </summary>
    public void RefreshRoleOfflineUI()
    {
        if (m_RoleInfoTranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }
        m_RoleInfoTranform.Find("Text_offline").gameObject.SetActive(m_nDisconnectState == 0);
    }

    /// <summary>
    /// 刷新角色步时和局时
    /// </summary>
    public void RefreshRoleTimeUI()
    {
        if (m_RoleInfoTranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }
        if(m_PlayChessTimeImage == null)
        {
            m_PlayChessTimeImage = m_RoleInfoTranform.Find("Head/HeadOutline/OutlineCountdown").GetComponent<Image>();
        }

        if(m_PlayChessTimeText == null)
        {
            m_PlayChessTimeText = m_PlayChessTimeImage.transform.Find("Time_BG/Text").GetComponent<Text>();
        }
        
        if(m_TotalPlayChessTimeText == null)
        {
            m_TotalPlayChessTimeText = m_PlayChessTimeImage.transform.Find("Time_BG (1)/Text").GetComponent<Text>();
        }

        m_PlayChessTimeImage.fillAmount = m_nCurPlayChessTime / m_nPlayChessTime;
     
        m_PlayChessTimeText.text = GameCommon.SecondsToDateTime(Mathf.CeilToInt(m_nCurPlayChessTime));
        m_TotalPlayChessTimeText.text = GameCommon.SecondsToDateTime(Mathf.CeilToInt(m_nTotalPlayChessTime)); 
    }

    /// <summary>
    /// 玩家游戏逻辑推进
    /// </summary>
    public virtual void OnTick()    {
        UpdateRoleChessTime();
    }

    /// <summary>
    /// 更新下棋时间
    /// </summary>
    private void UpdateRoleChessTime()
    {
        if (m_nCurPlayChessTime > 0)
        {
            m_nCurPlayChessTime -= Time.deltaTime;
            m_nTotalPlayChessTime -= Time.deltaTime;
            RefreshRoleTimeUI();
        }
    }

    /// <summary>
    /// 获得头像图标
    /// </summary>
    /// <returns></returns>
    public Sprite GetHeadImg()
    {
        return GameMain.hall_.GetIcon(m_sUrl, m_nUseId, m_nFaceId);
    }
    /// <summary>
    /// 获得玩家名称
    /// </summary>
    /// <returns></returns>
    public string GetRoleName()
    {
        return m_sRoleName;
    }

    /// <summary>
    /// 获得房间角色信息
    /// </summary>
    /// <param name="recordPalyerInfo">角色信息</param>
    public void GetRecordPlyerInfo(ref AppointmentRecordPlayer  recordPalyerInfo)
    {
        recordPalyerInfo.playerid = m_nUseId;
        recordPalyerInfo.faceid = m_nFaceId;
        recordPalyerInfo.url = m_sUrl;
        recordPalyerInfo.coin = m_nTotalCoin;
        recordPalyerInfo.playerName = GetRoleName();
        recordPalyerInfo.sex = m_nSex;
        recordPalyerInfo.ready = m_nReady;
    }

    /// <summary>
    /// 添加棋子数据
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="PiecesX"></param>
    /// <param name="PiecesY"></param>

    public void AddChessPiecesData(EChessPiecesType Type,byte PiecesX, byte PiecesY)
    {
        int Postion = 0;
        if (m_GameBase != null)
        {
            m_GameBase.GetPieceClientPostion(PiecesX, PiecesY, out Postion);
        }
         
        if (ChessPiecesDictionary.ContainsKey(Type))
        {
            ChessPiecesDictionary[Type] = Postion;
        }
        else
        {
            ChessPiecesDictionary.Add(Type, Postion);
        }
    }

    /// <summary>
    /// 删除棋子数据
    /// </summary>
    /// <param name="Type"></param>
    public void RemoveChessPiecesData(EChessPiecesType Type)
    {
        ChessPiecesDictionary.Remove(Type);
    }

    /// <summary>
    /// 刷新当前角色棋盘界面
    /// </summary>
    public void RefreshPlayerCheckerboard()
    {
        if(m_GameBase == null)
        {
            return;
        }

        foreach(var Pieces in ChessPiecesDictionary)
        {
            RefreshPlayerPiece(Pieces.Key);
        }
    }

    /// <summary>
    /// 刷新当前角色棋子界面
    /// </summary>
    /// <param name="PieceType"></param>
    /// <param name="Active"></param>
    /// <param name="PieceState"></param>
    /// <param name="SelectRefreshState"></param>
    public bool RefreshPlayerPiece(EChessPiecesType PieceType,bool Active= true,bool PieceState = false,bool SelectRefreshState = false)
    {
        if (m_GameBase == null)
        {
            return false;
        }
        Transform PieceObjectTransform = null;
        if (ChessPiecesDictionary.ContainsKey(PieceType))
        {
            PieceObjectTransform = m_GameBase.GetPieceTransform(ChessPiecesDictionary[PieceType]);
            if (PieceObjectTransform == null)
            {
                return false;
            }
            Sprite PieceSprite = GetPieceSprite(PieceType, PieceState);
            if (!SelectRefreshState)
            {
                Transform PieceTransform = PieceObjectTransform.Find("Image_qz");
                PieceTransform.GetComponent<Image>().sprite = PieceSprite;
                PieceTransform.gameObject.SetActive(Active);
            }
            else
            {
                m_GameBase.RefreshPlayerSelectPiece(Active, PieceSprite, PieceObjectTransform.position);
            }
            
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获得棋子图集对象
    /// </summary>
    /// <param name="PieceType"></param>
    /// <param name="SelectState"></param>
    /// <returns></returns>
    public Sprite GetPieceSprite(EChessPiecesType PieceType, bool SelectState = false)
    {
        if(m_GameBase == null)
        {
            return null;
        }
       return m_GameBase.GetPieceSprite(m_nSSit, PieceType, SelectState);
    }

    /// <summary>
    /// 获取棋子客户端位置
    /// </summary>
    /// <param name="PieceType"></param>
    /// <param name="PiecePostion"></param>
    /// <returns></returns>
    public bool GetPieceClientPostion(EChessPiecesType PieceType, out int PiecePostion)
    {
        PiecePostion = 0;
        if (ChessPiecesDictionary.ContainsKey(PieceType))
        {
            PiecePostion = ChessPiecesDictionary[PieceType];
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取棋子类型(根据棋子坐标)
    /// </summary>
    /// <param name="PiecePostion"></param>
    /// <returns></returns>
    public EChessPiecesType GetChessPieceTypeByPostion(int PiecePostion)
    {
        foreach (var Piece in ChessPiecesDictionary)
        {
            if (Piece.Value == PiecePostion)
            {
                return Piece.Key;
            }
        }
        return EChessPiecesType.CChessType_None;
    }
}