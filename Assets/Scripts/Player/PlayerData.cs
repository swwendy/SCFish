using System;
using System.Collections;using System.Collections.Generic;
using USocket.Messages;
using UnityEngine;

//玩家一辈子只改变一次的变量
public enum AllOneLifeSign_Enum
{
    AllOneLifeSign_ChangeName = 0,  //是否改变名字
    AllOneLifeSign_HasAddClub = 1,  //是否加入过俱乐部

    AllOneLifeSign
};

public class TotalInfo
{
    public TotalInfo()
    {
        matchtimes = 0;
        finaltimes = 0;
        totaltimes = 0;
        bestsort = 0;
        special1 = 0;
        special2 = 0;
        glory = new List<uint>();
        for (int gloryindex = 0; gloryindex < 3; gloryindex++)
            glory.Add(0);
    }

    public uint matchtimes;
    public uint finaltimes;
    public uint totaltimes;
    public ushort bestsort;
    public uint special1;
    public uint special2;
    public List<uint> glory;
}

public class GameInfo
{
    public byte gameId;
    public List<string> describeList = new List<string>();
}

/// <summary>/// 玩家数据 等级 金钱 钻石 拥有角色等等/// </summary>public class PlayerData{
    /// <summary>
    /// 玩家ID
    /// </summary>    private uint PlayerID;

    /// <summary>
    /// 玩家昵称
    /// </summary>        private string PlayerName;

    /// <summary>
    /// 玩家等级
    /// </summary>    public int PlayerLevel;

    /// <summary>
    /// 玩家头像ID
    /// </summary>
    public uint PlayerIconId;

    /// <summary>
    /// 玩家头像url
    /// </summary>
    private string PlayerIconUrl;

    /// <summary>
    /// 玩家性别
    /// </summary>
    public byte PlayerSexSign;

    /// <summary>
    /// 玩家vip等级
    /// </summary>    private byte PlayerVipLv;
    /// <summary>
    /// 玩家钱币
    /// </summary>
    private long PlayerCoin;

    /// <summary>
    /// 玩家钻石
    /// </summary>
    private uint PlayerDiamond;

    /// <summary>
    /// 玩家奖券
    /// </summary>
    private long PlayerLottery;

    /// <summary>
    /// 玩家俱乐部ID
    /// </summary>
    private uint GuildID;

    //标记值
    private byte FlagValue;

    /// <summary>
    /// 玩家是否已经修改昵称
    /// </summary>    public bool IsChangeName;

    /// <summary>
    /// 开启的游戏ID 标识
    /// </summary>
    public List<GameInfo> GameList;


    /// <summary>
    /// 充值总金额
    /// </summary>
    private uint RechargeTotal;


    /// <summary>
    /// 支付平台
    /// </summary>
    public uint nPayPlatform;

    /// <summary>
    /// 绑定手机号
    /// </summary>
    private long BindMobileNumber ;

    /// <summary>
    /// 之前所在的游戏的特殊号 列入 约局的约局ID  比赛的比赛ID
    /// </summary>
    public uint nSpecilID_Before;

    /// <summary>
    /// 之前所在的游戏的特殊号2 列入 比赛的dataid
    /// </summary>
    public uint nSpecilID2_Before;

    /// 上线之前所处的游戏模式（0 匹配，1 比赛，2约局）
    /// </summary>
    public short nGameMode_Before;
    /// <summary>
    /// 上线之前 所在的游戏kind
    /// </summary>
    public uint nGameKind_Before;

    /// <summary>
    /// 上线之前 所在的房间服务器的index
    /// </summary>
    public byte byGameSerIndex_Before;

    /// <summary>
    /// 上线之前 所在的房间服务器的特殊记号
    ///  比赛模式下  1是轮空进去的
    /// </summary>
    public byte nSpecilSign_Before;

    /// <summary>
    /// 俱乐部名称
    /// </summary>
    private string GuildName;

    /// <summary>
    /// 是否绑定邀请码
    /// </summary>
    public byte nIsBindInvite;

    public byte nIsBindAward = 0;
    public uint nTodayAward = 0;
    public uint nTomorrowAward = 0;

    /// <summary>
    /// 新手礼包购买情况
    /// </summary>
    public int newcomerGift;

    /// <summary>
    /// 特惠礼包购买情况
    /// </summary>
    public int discountsGift;

    /// <summary>
    /// 新手礼包倒计时
    /// </summary>
    public float newcomertime;

    //周签到情况
    public byte weekSign;

    //周登陆天数
    public byte weekAdd;

    //登陆奖励领取情况
    public byte addUpWard;

    //商城兑换开关
    public bool ShopExchangeTurnOff;

    /// <summary>
    /// 玩家人数
    /// </summary>
    public Dictionary<byte, ushort> peopleNumber;

    /// <summary>
    /// 当前报名的比赛
    /// </summary>
    public List<uint> signedContests;

    //信誉分
    public uint creditScore;
    //大师分
    public float[] MasterScoreKindArray;

    //未领取微信公众号现金红包
    public float UnreceivedRedBag;
    //已领取微信公众号现金红包
    public float ReceivedRedBag;

    //邮件数量
    public byte mailNumber;
    //物品格子数
    public byte itemNumber;
    //朋友圈id
    public uint momentsid;
    //朋友圈名字
    public string momentName;

    public byte NeedSign;
    public uint SignAward;
    //象棋未读比赛记录数量
    public byte ChessContestNumber;
    public Dictionary<byte, TotalInfo> totalinfo;

    public PlayerData()
    {
        GameList = new List<GameInfo>();
        totalinfo = new Dictionary<byte, TotalInfo>();
        peopleNumber = new Dictionary<byte, ushort>();
        signedContests = new List<uint>();
        MasterScoreKindArray = new float[(int)GameKind_Enum.GameKind_Max];
        for (int i = 0; i < MasterScoreKindArray.Length; i++)
            MasterScoreKindArray[i] = 0.0f;
    }

    public void ReadPlayerData(UMessage playerData, ref float nLeftTime)
    {
        peopleNumber.Clear();
        GameList.Clear();

        PlayerName = playerData.ReadString();        PlayerID = playerData.ReadUInt();        PlayerIconId = playerData.ReadUInt();        PlayerIconUrl = playerData.ReadString();        PlayerSexSign = playerData.ReadByte();        PlayerCoin = playerData.ReadLong();        PlayerDiamond = playerData.ReadUInt();        PlayerLottery = playerData.ReadLong();        PlayerVipLv = playerData.ReadByte();        FlagValue = playerData.ReadByte();        IsChangeName = (FlagValue & (0x1 << (int)AllOneLifeSign_Enum.AllOneLifeSign_ChangeName)) > 0 ;        GuildID = playerData.ReadUInt();        if (GuildID > 0)            GuildName = playerData.ReadString();                momentsid = playerData.ReadUInt();        if (momentsid > 0)            momentName = playerData.ReadString();        Debug.Log("moments id:" + momentsid.ToString() + "momentsName:" + momentName);        //GameIDFlag = playerData.ReadUInt();
        byte gameNum = playerData.ReadByte();
        GameInfo gameInfo;
        for(byte index = 0; index < gameNum; index++)
        {
            gameInfo = new GameInfo();
            gameInfo.gameId = playerData.ReadByte();
            peopleNumber.Add(gameInfo.gameId, playerData.ReaduShort());

            //if (gameInfo.gameId == 7 || gameInfo.gameId == 13) //GameKind_LandLords,GameKind_GuanDan
            {
                byte nNum = playerData.ReadByte();
                for (byte k=0; k<nNum; k++)
                {
                    gameInfo.describeList.Add(playerData.ReadString());
                }
            }
            GameList.Add(gameInfo);
        }

        RechargeTotal = (uint)playerData.ReadLong();
        nPayPlatform = playerData.ReadUInt();
        BindMobileNumber = playerData.ReadLong();

        nGameMode_Before = playerData.ReadShort();
        if(nGameMode_Before > -1)
        {
            if(nGameMode_Before == 0) 
            {
                //GameReason_Match
                nGameKind_Before = playerData.ReadUInt();
                byGameSerIndex_Before = playerData.ReadByte();
                HallMain.SetRoomSerIndex(byGameSerIndex_Before);
            }
            else
            {
                // GameReason_Contest  GameReason_Appoint
                nGameKind_Before = playerData.ReadByte();
                nSpecilID_Before = playerData.ReadUInt();
                byGameSerIndex_Before = playerData.ReadByte();
                HallMain.SetRoomSerIndex(byGameSerIndex_Before);
                nSpecilID2_Before = playerData.ReadUInt();
                nSpecilSign_Before = playerData.ReadByte();
            }
        }
        
        nLeftTime = playerData.ReadSingle();
        nIsBindInvite = playerData.ReadByte();
        if(nIsBindInvite > 0 && nIsBindInvite < 200)
        {
            nIsBindAward = playerData.ReadByte();
            nTodayAward = playerData.ReadUInt();
            nTomorrowAward = playerData.ReadUInt();
        }

        newcomerGift = playerData.ReadInt();
        discountsGift = playerData.ReadInt();
        newcomertime = playerData.ReadSingle();

        weekSign = playerData.ReadByte();
        weekAdd = playerData.ReadByte();
        addUpWard = playerData.ReadByte();
        ShopExchangeTurnOff = playerData.ReadBool();

        signedContests.Clear();
        byte length = playerData.ReadByte();
        for(int index = 0; index < length; index++)
        {
            signedContests.Add(playerData.ReadUInt());
        }

        creditScore = playerData.ReadUInt();
        UnreceivedRedBag = playerData.ReadSingle();
        ReceivedRedBag = playerData.ReadSingle();
        mailNumber = playerData.ReadByte();
        itemNumber = playerData.ReadByte();
  
        length = playerData.ReadByte();
        byte nKind = 0;       
        for (int index = 0; index < length; index++)
        {
            nKind = playerData.ReadByte();
            MasterScoreKindArray[nKind] = playerData.ReadSingle();
        }

        NeedSign = playerData.ReadByte();//0签到过 1未签到
        SignAward = playerData.ReadUInt();
        ChessContestNumber = playerData.ReadByte();

        return;
    }    public void SetPlayerID(uint playerid)
    {
        PlayerID = playerid;
    }    public uint GetPlayerID()
    {
        return PlayerID;
    }    public string GetPlayerName()
    {
        return PlayerName;
    }    public void SetPlayerName(string name)
    {
        if(!string.IsNullOrEmpty(name))
          PlayerName = name;
    }    public string GetPlayerIconURL()
    {
        return PlayerIconUrl;
    }    public long GetCoin()
    {
        return PlayerCoin;
    }    public void AddCoin(long coin)
    {
        PlayerCoin += coin;
    }    public void SetCoin(long coin)
    {
        PlayerCoin = coin;
    }    public uint GetDiamond()
    {
        return PlayerDiamond;
    }    public void SetDiamond(uint value)
    {
        PlayerDiamond = value;
    }    public long GetLottery()
    {
        return PlayerLottery;
    }    public void SetLottery(long value)
    {
        PlayerLottery = value;
    }

    public void SetMaster(byte bGameKind,float value)
    {
        MasterScoreKindArray[bGameKind] = value;
    }    public uint GetGuildID()
    {
        return GuildID;
    }    public void SetGuildID(uint id)
    {
        GuildID = id;
    }    public byte GetVipLevel()
    {
        return PlayerVipLv;
    }    public void SetVipLv(byte viplv)
    {
        PlayerVipLv = viplv;
    }    public uint GetRechargeTotal()
    {
        return RechargeTotal;
    }    public void SetRechargeTotal(uint value)
    {
        RechargeTotal = value;
    }    public string GetGuildName()
    {
        return GuildName;
    }    public void SetGuildName(string clubname)
    {
        if (!string.IsNullOrEmpty(clubname))
            GuildName = clubname;
    }    public long GetBindPhoneNumber()
    {
        return BindMobileNumber;
    }    public void SetBindPhoneNumber(long number)
    {
        BindMobileNumber = number;
    }}