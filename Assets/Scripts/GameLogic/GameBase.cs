//create by Batcel on 2017-07-20

using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;
using XLua;

/// <summary>
/// 游戏基类
/// </summary>
[LuaCallCSharp]
public class CGameBase
{
    public GameKind_Enum GameType;
    public GameTye_Enum GameMode { get; set; }
    public bool Bystander { get; set; }
    public CGameBase()
    {
        GameType = GameKind_Enum.GameKind_None;
        GameMode = GameTye_Enum.GameType_Normal;
        Bystander = false;
    }

    public CGameBase(GameKind_Enum gametype)
    {
        GameType = gametype;
        GameMode = GameTye_Enum.GameType_Normal;
        Bystander = false;
    }

    /// <summary>
    /// 游戏初始函数
    /// </summary>
    public virtual void Initialization()
    {
        HallMain.AdaptiveUI();
    }

    /// <summary>
    /// 游戏逻辑推进
    /// </summary>
    public virtual void ProcessTick()
    {
        //Debug.Log("base class update....");
    }

    /// <summary>
    /// 处理游戏网络消息包
    /// </summary>
    public virtual bool HandleGameNetMsg(uint _msgType, UMessage _ms)
    {
        return true;
    }


    /// <summary>
    /// 重置游戏UI
    /// </summary>
    public virtual void ResetGameUI()
    {

    }

    /// <summary>
    /// 刷新游戏中玩家新获取的金币数量
    /// </summary>
    public virtual void RefreshGamePlayerCoin(uint AddMoney)
    {

    }

    /// <summary>
    /// 刷新游戏中玩家新获取的金币数量
    /// </summary>
    public virtual void RefreshGamePlayerCoin()
    {

    }

    //初始通用UI
    public void InitialCommonUI()
    {
        string TrumpetBtnPath = "";
        switch(GameType)
        {
            case GameKind_Enum.GameKind_BlackJack:
                TrumpetBtnPath = "Blackjack_MainUI(Clone)/Middle/Button_horn";
                break;
            case GameKind_Enum.GameKind_BullAllKill:
                TrumpetBtnPath = "Niu5_MainUI(Clone)/Middle/Button_horn";
                break;
            case GameKind_Enum.GameKind_BullHundred:
                TrumpetBtnPath = "Niu2_MainUI(Clone)/Middle/Button_horn";
                break;
            case GameKind_Enum.GameKind_CarPort:
                TrumpetBtnPath = "Car_Main(Clone)/MainBG/Button_horn";
                break;
            case GameKind_Enum.GameKind_DiuDiuLe:
                TrumpetBtnPath = "DouDiZhu_Game(Clone)/Middle/Button_horn";
                break;
            case GameKind_Enum.GameKind_Fishing:
                TrumpetBtnPath = "Fishing_Game(Clone)/Middle/Button_horn";
                break;
            case GameKind_Enum.GameKind_FiveInRow:
                TrumpetBtnPath = "Five_MainUI(Clone)/Button_horn";
                break;
            case GameKind_Enum.GameKind_ForestDance:
                TrumpetBtnPath = "Forest_MainUI(Clone)/Button_horn";
                break;
            case GameKind_Enum.GameKind_LaBa:
                TrumpetBtnPath = "Slots_Game(Clone)/Middle/Button_horn";
                break;
            case GameKind_Enum.GameKind_LandLords:
                TrumpetBtnPath = "DouDiZhu_Game(Clone)/Middle/Button_horn";
                break;
            case GameKind_Enum.GameKind_Mahjong:
                TrumpetBtnPath = "Mahjong_Game(Clone)/Middle/Button_horn";
                break;
            case GameKind_Enum.GameKind_GuanDan:
                TrumpetBtnPath = "GuanDan_Game(Clone)/Middle/Button_horn";
                break;
        }

        CTrumpetUI.Instance.InitTrumpetButtenEvent(TrumpetBtnPath);
    }

   

    /// <summary>
    /// 获取游戏类型
    /// </summary>
    /// <returns></returns>
    public GameKind_Enum GetGameType()
    {
        return GameType;
    }

    /// <summary>
    /// 设置游戏类型
    /// </summary>
    /// <returns></returns>
    public void SetGameType(GameKind_Enum gametype)
    {
        if(gametype > GameKind_Enum.GameKind_None && gametype < GameKind_Enum.GameKind_Max)
          GameType = gametype;
    }

    public virtual void OnApplicationFocus(bool focus)
    {

    }

    /// <summary>
    /// 与游戏服务器网络断开连接
    /// </summary>
    public virtual void OnDisconnect(bool over = true)
    {

    }

    /// <summary>
    /// 与游戏服务器重连成功
    /// </summary>
    public virtual void ReconnectSuccess()
    {

    }

    public virtual void SetupVideo(List<AppointmentRecordPlayer> players)
    {

    }

    public virtual bool OnVideoStep(List<VideoAction> action, int curStep, bool reverse = false)
    {
        return false;
    }

    public virtual void OnVideoReplay()
    {
    }

    public virtual void OnPlayerDisOrReconnect(bool disconnect, uint userId, byte sit)
    {

    }

    public virtual void StartLoad()
    {

    }
}