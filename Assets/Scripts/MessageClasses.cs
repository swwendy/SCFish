using System.Collections;using System.Collections.Generic;using USocket.Messages;using System.IO;using System.Text;using System.Runtime.InteropServices;using System;using UnityEngine;class GameKind{    // en:枚举序号    public static int MyBIT(int en)    {        if (en < 0 || en >= 32)            return 0;        return (1 << en);    }    public static void AddFlag(int en, ref int flag)    {        flag |= MyBIT(en);    }    public static bool HasFlag(int en, int flag)    {        return (flag & (MyBIT(en))) > 0;    }    public static bool HasFlag(int en, uint flag)    {        return (flag & (MyBIT(en))) > 0;    }    public static void RemoveFlag<T>(int en, ref int flag)    {        flag &= (~MyBIT(en));    }}public class strCoinRank
{
    public uint nUseID;                  //useid
    public uint faceid;
    public string url;
    public long nCoin;
    public string sNickName;
}public class MessageLogin{    public void SetSendData(UMessage msg)    {        msg.Add(nUseID);        msg.Add(smachinecode);        msg.Add(sVersion);        msg.Add(nPlatform);
        msg.Add("");
        msg.Add("");        msg.Add("");    }    public uint nUseID;                  //useid
    public uint faceid;               //头像url
    public string name;                 //昵称
    public string smachinecode;          //机器码    public string sVersion;                //版本号
    public byte nPlatform;                  ////0:windows, 1苹果 2安卓
    public byte bLoadType;               // 0 默认 1 运营渠道 2 微信
}public class MobileLogin_Msg
{
    public void SetSendData(UMessage msg)    {        msg.Add(nState);        msg.Add(nMobileNum);    }

    public byte nState; // 1:首次手机登陆 2：绑定手机号
    public long nMobileNum; //手机号
}public class CheckCodeLogin_Msg
{
    public void SetSendData(UMessage msg)    {        msg.Add(nPlatform);        msg.Add(nBindOrLogin);        msg.Add(nCode);        msg.Add(smachinecode);    }

    public byte nPlatform;                  ////0:windows, 1苹果 2安卓
    public byte nBindOrLogin;               //1登陆 2绑定 3解绑
    public uint nCode;                  //useid
    public string smachinecode;          //机器码
}/*public class MessageLoginSuccess{    public void ReadData(UMessage msg)    {        nickname = msg.ReadString();        nUseid = msg.ReadUInt();        faceid = msg.ReadInt();        nCoin = msg.ReadLong();        nDiamond = msg.ReadUInt();        nLottery = msg.ReadUInt();        nVipLevel = msg.ReadChar();        nIsChangeName = msg.ReadBool();        gamelist = msg.ReadInt();    }    public string nickname;    public uint nUseid;    public int faceid;    public long nCoin;        //钱币    public uint nDiamond;  //钻石    public uint nLottery;  //奖券    public char nVipLevel;//VIP等级    public bool nIsChangeName;		//是否已经修改昵称    public int gamelist;                 //游戏列表}*/public class MessageChangeNickName{    public void SetSendData(UMessage msg)    {        msg.Add(uid);        msg.Add(faceid);        msg.Add(name);    }    public uint uid;    public int faceid;    public string url;    public string name;}public class MessageApplyGame{    public void SetSendData(UMessage msg)    {        msg.Add(nUseID);        msg.Add(nKind);    }    public uint nUseID;    public byte nKind;}public class MessageBackApplyGame{    public void ReadData(UMessage msg)    {        isin = msg.ReadBool();        if(isin)        {
            kind = msg.ReadByte();
            ip = msg.ReadString();
            port = msg.ReadInt();            nSerIndex = msg.ReadByte();        }    }    public bool isin;    public byte kind;    public string ip;    public int port;    public byte nSerIndex;  //房间服务器编号}

public class CarMessageBase
{
    public uint carportMsgType;
}

public class CarPortLogin : CarMessageBase
{
    public void Create()
    {
        carportMsgType = (uint)GameCity.CarportMsg_enum.CarportMsg_CM_LOGIN;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(carportMsgType);
        msg.Add(userID);
    }

    public uint userID;
}

public class CarPortChooseLevel : CarMessageBase
{
    public void Create()
    {
        carportMsgType = (uint)GameCity.CarportMsg_enum.CarportMsg_CM_CHOOSELEVEL;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(carportMsgType);
        msg.Add(level);
        msg.Add(userID);
    }

    public byte level;
    public uint userID;
}

public class ChipCar : CarMessageBase
{
    public void Create()
    {
        carportMsgType = (uint)GameCity.CarportMsg_enum.CarportMsg_CM_CHIPCAR;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(carportMsgType);
        msg.Add(nCarID);
        msg.Add(nUseid);
        msg.Add(nChipNum);
    }

    public char nCarID;
    public uint nUseid;
    public uint nChipNum;
}

public class BackChipCar : CarMessageBase
{
    public void ReadData(UMessage msg)
    {
        nState = msg.ReadChar();
        nCarid = msg.ReadChar();
        nUseid = msg.ReadUInt();
        nAllChipNum = msg.ReadLong();
        nSelfChipNum = msg.ReadLong();
        nCanChipNum = msg.ReadLong();
    }

    // 1:成功
    // 2: 此人不在该房间内
    // 3:车的id非法
    // 4:没人坐庄 或者 庄家id 非法
    // 5：庄家自己不能投注
    // 6: 投注金额要大于1
    // 7：投注的金额大于拥有的金额
    // 8:投注金额大于本局能投注的最大值
    public char nState;
    public char nCarid;
    public uint nUseid;
    public long nAllChipNum;
    public long nSelfChipNum;
    public long nCanChipNum;
}

public class CarApplyBoss : CarMessageBase
{
    public void Create(uint _id)
    {
        nUseid = _id;
        carportMsgType = (uint)GameCity.CarportMsg_enum.CarportMsg_CM_APPLYBOSS;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(carportMsgType);
        msg.Add(nUseid);
    }

    uint nUseid;
};

public class CarBackApplyBoss : CarMessageBase
{
    public void ReadData(UMessage msg)
    {
        //nstate = msg.ReadChar();
        nUseID = msg.ReadUInt();
        cRoleName = msg.ReadString();
        nCoin = msg.ReadLong();
        needcoin = msg.ReadLong();
    }

    public char nstate;//1:成功 2：不在房间里 3:钱不够 4:已经在申请列表中
    public uint nUseID;
    public string cRoleName;
    public long nCoin;
	public long needcoin;
}

public class CarCancleApplyBoss : CarMessageBase
{
    public void Create(uint _id)
    {
        nUseid = _id;
        carportMsgType = (uint)GameCity.CarportMsg_enum.CarportMsg_CM_CANCLEAPPLYBOSS;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(carportMsgType);
        msg.Add(nUseid);
    }

    public uint nUseid;
}

public class CarBackCancleBoss : CarMessageBase
{
    public void ReadData(UMessage msg)
    {
        nUseid = msg.ReadUInt();
        carportMsgType = (uint)GameCity.CarportMsg_enum.CarportMsg_SM_CANCLEAPPLYBOSS;
    }

    public uint nUseid;
}

public class CarApplyDownBoss : CarMessageBase
{
    public void Create(uint _id)
    {
        nUseid = _id;
        carportMsgType = (uint)GameCity.CarportMsg_enum.CarportMsg_CM_APPLYDOWNBOSS;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(carportMsgType);
        msg.Add(nUseid);
    }

    public uint nUseid;
}

public class CarCancleApplyDowng : CarMessageBase
{
    public void Create(uint useid)
    {
        nUseid = useid;
        carportMsgType = (uint)GameCity.CarportMsg_enum.CarportMsg_CM_CANCLEAPPLYDOWNBOSS;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(carportMsgType);
        msg.Add(nUseid);
    }

    public uint nUseid;
}

public class CarApplyBossList : CarMessageBase
{
    public void Create(uint _id)
    {
        nUseid = _id;
        carportMsgType = (uint)GameCity.CarportMsg_enum.CarportMsg_CM_APPLYBOSSLIST;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(carportMsgType);
        msg.Add(nUseid);
    }

    public uint nUseid;
};

public class BackBossList : CarMessageBase
{
    public BackBossList()
    {
        infos = new List<BossListInfo>();
    }

    public void ReadData(UMessage msg)
    {
        infos.Clear();
        peopleNumber = msg.ReadInt();
        for (int index = 0; index < peopleNumber; index++)
        {
            BossListInfo bli = new BossListInfo();

            bli.uid = msg.ReadUInt();
            bli.faceid = msg.ReadUInt();
            bli.url = msg.ReadString();
            bli.money = msg.ReadLong();
            bli.name = msg.ReadString();

            infos.Add(bli);
        }
    }

    public int peopleNumber;
    public List<BossListInfo> infos;
}

public class BossListInfo
{
    public uint uid;
    public uint faceid;
    public string url;
    public long money;
    public string name;
}

public class SortData
{
    public SortData()
    {
        si = new List<SortInfo>();
        god = new GameOverData();
    }

    public void ReadData(UMessage msg)
    {
        number = msg.ReadChar();
        si.Clear();
        for (int index = 0; index < (int)number; index++)
        {
            SortInfo tempsi = new SortInfo();
            tempsi.ReadData(msg);
            si.Add(tempsi);
        }

        god.ReadData(msg);
    }

    public char number;
    public List<SortInfo> si;
    public GameOverData god;
}

public class SortInfo
{
    public void ReadData(UMessage msg)
    {
        playerid = msg.ReadUInt();
        faceid = msg.ReadUInt();
        url = msg.ReadString();
        money = msg.ReadLong();
        name = msg.ReadString();
    }

    public uint playerid;
    public uint faceid;
    public string url;
    public long money;
    public string name;
}

public class GameOverData
{
    public void ReadData(UMessage msg)
    {
        nSelfRank = msg.ReadUInt();
        nSelfAdd = msg.ReadLong();
        nSelfCoin = msg.ReadLong();
        nBossAdd = msg.ReadLong();
        nBossCoin = msg.ReadLong();
    }

    public uint nSelfRank;
    public long nSelfAdd;
    public long nSelfCoin;
    public long nBossAdd;
    public long nBossCoin;
};

class BackRecharge
{
    public void ReadData(UMessage msg)
    {
        nUseid = msg.ReadUInt();
        nPath = msg.ReadChar();
        nAdd = msg.ReadLong();
        nCoin = msg.ReadLong();
        nDia = msg.ReadUInt();
        nItemID = msg.ReadString();
    }

    public uint nUseid;    //等于0表示失败
    public char nPath;     // 1:ipone 2:android
    public long nAdd;     //添加的数额
    public long nCoin;    //总共
    public uint nDia;	   //总共
    public string nItemID;
}

public class UClubBase_Msg      //所有俱乐部消息基类
{
    public UClubBase_Msg()
    {
        nMessageType = 0;
    }

    public uint nMessageType;
    public uint nClubMsg_Type;				//俱乐部的子消息
}

public class ClubInfo
{
    public uint clubid;
    public string clubName;
    public uint memberNumber;
    public uint maxMember;
    public uint clubLevel;
    public string content;
    public char condition;
}

public class UClubListInfo : UClubBase_Msg         //俱乐部列表
{
    public UClubListInfo()
    {
        clubs = new List<ClubInfo>();
    }

    public void ReadData(UMessage msg)
    {
        //nMessageType = msg.ReadUInt();
        clubNumber = msg.ReadUInt();
        clubs.Clear();
        for (uint index = 0; index < clubNumber; index++)
        {
            ClubInfo info = new ClubInfo();
            info.clubid = msg.ReadUInt();
            info.clubName = msg.ReadString();
            info.memberNumber = msg.ReadUInt();
            info.maxMember = msg.ReadUInt();
            info.clubLevel = msg.ReadUInt();
            info.content = msg.ReadString();
            info.condition = msg.ReadChar();

            clubs.Add(info);
        }
    }

    public uint clubNumber;
    public List<ClubInfo> clubs;
}

class UCreateClubMsg : UClubBase_Msg        //创建俱乐部
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.CM_ClubScondCreate;
        //msg.Add(nClubMsg_Type);
        msg.Add(playerid);
        msg.Add(clubName);
    }

    public uint playerid;
    public string clubName;
}

class UExpelMsg : UClubBase_Msg //踢人
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondExpel;
        //msg.Add(nClubMsg_Type);
        msg.Add(clubid);
        msg.Add(expelID);
    }
    public uint clubid;
    public int expelID;
}

class UClubJoinMsg : UClubBase_Msg //加入
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondJoin;
        //msg.Add(nClubMsg_Type);
        msg.Add(clubID);
        msg.Add(playerID);
    }
    public uint clubID;
    public uint playerID;
}

class UClubLevelUp : UClubBase_Msg //俱乐部升级
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondLevelUp;
        //msg.Add(nClubMsg_Type);
        msg.Add(clubID);
    }

    public uint clubID =0 ;
}

class UClubGive : UClubBase_Msg // 赠送
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondGive;
        //msg.Add(nClubMsg_Type);
        msg.Add(giveID);
        msg.Add(accessID);
        msg.Add(money);
    }

    public uint giveID =0;
    public uint accessID =0;
    public long money =0;
};

class UClubBindingMsg : UClubBase_Msg //绑定手机
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondBindingPhone;
        //msg.Add(nClubMsg_Type);
        msg.Add(userID);
        msg.Add(phoneNumber);
    }

    public uint userID =0;
    public uint phoneNumber =0;
};

class UClubCheckPhone : UClubBase_Msg //验证手机
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondCheckPhone;
        //msg.Add(nClubMsg_Type);
        msg.Add(userID);
        msg.Add(phoneNumber);
    }
    public uint userID =0;
    public uint phoneNumber =0;
};

class UClubChangeCondition : UClubBase_Msg //修改加入限制
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondChangeJoinCondition;
        //msg.Add(nClubMsg_Type);
        msg.Add(clubid);
        msg.Add(condition);
    }
    public uint clubid;
    public char condition;
};

class UClubExpelOneKey : UClubBase_Msg //一键踢出
{
    public UClubExpelOneKey()
    {
    }

    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondExpelOneKey;
        //msg.Add(nClubMsg_Type);
        msg.Add(clubid);
        msg.Add(Time2Expel);
    }

    public uint clubid;
    public byte Time2Expel;
};

class UClubJoinOneKey : UClubBase_Msg //一键加入
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondJoinOneKey;
        //msg.Add(nClubMsg_Type);
        msg.Add(userID);
        for (int index = 0; index < clubList.Count; index++)
        {
            msg.Add(clubList[index]);
        }
    }

    public List<uint> clubList = null;
    public uint userID =0;
}

class UClubInfo : UClubBase_Msg//俱乐部基本信息
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSecondInfo;
        //msg.Add(nClubMsg_Type);
        msg.Add(userID);
    }
    public uint userID;
}

class UClubBreak : UClubBase_Msg//解散俱乐部
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubBreak;
        //msg.Add(nClubMsg_Type);
        msg.Add(clubid);
    }

    public uint clubid;
}

class UExitClub : UClubBase_Msg //退出俱乐部
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ExitClub;
        //msg.Add(nClubMsg_Type);
        msg.Add(clubid);
        msg.Add(userid);
    }

    public uint userid;
    public uint clubid;
}

class UClubSearch : UClubBase_Msg //搜索
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubSearch;
        //msg.Add(nClubMsg_Type);
        msg.Add(searchName);
    }
    public string searchName;
}

class UClubContent : UClubBase_Msg // 工会宣言
{
    public void SetSendData(UMessage msg)
    {
        //nClubMsg_Type = (uint)GameCity.ClubSecondMsg.ClubChangeContent;
        //msg.Add(nClubMsg_Type);
        msg.Add(clubid);
        msg.Add(content);
    }
    public uint clubid;
    public string content;
}

public class USlotMessageBase
{
    public USlotMessageBase()
    {
        nMessageType = 0;
    }

    public uint nMessageType;
}

class USlotLogin : USlotMessageBase
{
    public void SetSendData(UMessage msg)
    {
        //msg.Add((uint)GameCity.SlotSecondMsg.LabaMsg_CM_LOGIN);
        msg.Add(userID);
    }

    public uint userID;
}

public class USlotBackLogin : USlotMessageBase
{
    public USlotBackLogin()
    {
        ins = new List<int>();
        outs = new List<int>();
    }

    public void ReadData(UMessage msg)
    {
        levels = msg.ReadInt();
        for (int index = 0; index < levels; index++)
        {
            ins.Add(msg.ReadInt());
            outs.Add(msg.ReadInt());
        }
    }

    public int levels;
    public List<int> ins;
    public List<int> outs;
}

class USlotPickGame : USlotMessageBase
{
    public void SetSendData(UMessage msg)
    {
        //msg.Add((uint)GameCity.SlotSecondMsg.LabaMsg_CM_CHOOSElEVEL);
        msg.Add(userID);
        msg.Add(level);
    }

    public uint userID;
    public byte level;
}

class USlotGameState : USlotMessageBase
{
    public void SetSendData(UMessage msg)
    {
        //msg.Add((uint)GameCity.SlotSecondMsg.LabaMsg_CM_GAMESTATE);
        msg.Add(nline);
        msg.Add(isFree);
        msg.Add(userID);
        msg.Add(chipIn);
    }

    public byte nline;
    public bool isFree;
    public uint userID;
    public long chipIn;
}

public class USlotWinData
{
    public byte lineid;
    public byte lof;
    public byte number;
}

public class USlotCardResult : USlotMessageBase
{
    public void ReadData(UMessage msg)
    {
        addCoin = msg.ReadLong();
        coin = msg.ReadLong();
        times = msg.ReadByte();
    }

    public long addCoin;
    public long coin;
    public byte times;
}

public class USlotGameResult : USlotMessageBase
{
    public USlotGameResult()
    {
        icons = new List<byte>();
        windata = new List<USlotWinData>();
    }

    public void ReadData(UMessage msg)
    {
        isStart = msg.ReadBool();
        if (!isStart)
            return;
        getCoin = msg.ReadLong();
        playerCoin = msg.ReadLong();
        nLinenum = msg.ReadUInt();
        nWinCoin = msg.ReadLong();
        nFreeNum = msg.ReadByte();
        cardNum = msg.ReadByte();
        wLinenum = msg.ReadByte();
        for(int index = 0; index < 15; index++)
        {
            icons.Add(msg.ReadByte());
        }

        for(int index = 0; index < wLinenum; index++)
        {
            USlotWinData swd = new USlotWinData();
            swd.lineid = msg.ReadByte();
            swd.lof = msg.ReadByte();
            swd.number = msg.ReadByte();

            windata.Add(swd);
        }

        currentlottery = msg.ReadLong();
        lottery = msg.ReadLong();
    }

    public bool isStart;
    public long getCoin;
    public long playerCoin;
    public uint nLinenum;
    public long nWinCoin;
    public byte nFreeNum;
    public List<byte> icons;

    public byte wLinenum;
    public List<USlotWinData> windata;
    public byte cardNum;
    public long currentlottery;
    public long lottery;
}

public class ForestDanceMessageBase
{
    public uint forestMsgType;
}

public class ForestDanceLogin : ForestDanceMessageBase
{
    public void Create()
    {
        forestMsgType = (uint)GameCity.Forest_enum.ForestMsg_CM_LOGIN;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(forestMsgType);
        msg.Add(userID);
    }

    public uint userID;
}

public class ForestDanceBackLogin : ForestDanceMessageBase
{
    public void ReadData(UMessage msg)
    {
        byte length = msg.ReadByte();
        for(int index = 0; index < length; index++)
        {
            maxMoney = msg.ReadInt();
            inMoney = msg.ReadInt();
        }
    }

    public int maxMoney;
    public int inMoney;
}

public class ForestDanceChooseLevel : ForestDanceMessageBase
{
    public ForestDanceChooseLevel()
    {
        forestMsgType = (uint)GameCity.Forest_enum.ForestMsg_CM_CHOOSElEVEL;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(forestMsgType);
        msg.Add(userID);
        msg.Add(level);
    }

    public uint userID;
    public byte level;
}

public class ForestDanceRoomData : ForestDanceMessageBase
{
    public ForestDanceRoomData()
    {
        chipsinfo = new Dictionary<int, ChipsData>();
        histroyinfo = new List<ushort>();
        colorsort = new List<byte>();
    }

    public void ReadData(UMessage msg)
    {
        level = msg.ReadByte();
        incoin = msg.ReadLong();
        handle = msg.ReadLong();
        colorid = msg.ReadInt();
        roomState = msg.ReadInt();
        lefttime = msg.ReadSingle();

        byte chipslength = msg.ReadByte();
        byte histroylength = msg.ReadByte();
        byte colorlength = msg.ReadByte();

        for(int index = 0; index < chipslength; index++)
        {
            byte chipIndex = msg.ReadByte();
            if(chipsinfo.ContainsKey(chipIndex))
            {
                chipsinfo[chipIndex].selfchip = msg.ReadLong();
                chipsinfo[chipIndex].totalchip = msg.ReadLong();
            }
            else
            {
                ChipsData data = new ChipsData();
                data.chipIndex = chipIndex;
                data.selfchip = msg.ReadLong();
                data.totalchip = msg.ReadLong();

                chipsinfo.Add(data.chipIndex, data);
            }
        }

        for(int index = 0; index < histroylength; index++)
        {
            histroyinfo.Add(msg.ReaduShort());
        }

        for(int index = 0; index < colorlength; index++)
        {
            colorsort.Add(msg.ReadByte());
        }
    }

    public byte level;
    public long incoin;
    public long handle;
    public int colorid;
    public int roomState;
    public float lefttime;
    public Dictionary<int, ChipsData> chipsinfo;
    public List<ushort> histroyinfo;
    public List<byte> colorsort;
}

public class ForestDanceStartGame : ForestDanceMessageBase
{
    public ForestDanceStartGame()
    {
        colorsort = new List<byte>();
    }

    public void ReadData(UMessage msg)
    {
        colorid = msg.ReadInt();
        byte length = msg.ReadByte();
        for(int index = 0; index < length; index++)
        {
            if (colorsort.Count > index)
                colorsort[index] = msg.ReadByte();
            else
                colorsort.Add(msg.ReadByte());
        }
    }

    public int colorid;
    public List<byte> colorsort;
}

public class ForestDanceResult : ForestDanceMessageBase
{
    public ForestDanceResult()
    {
        signs = new List<byte>();
    }

    public void ReadData(UMessage msg)
    {
        model = msg.ReadByte();
        //model = (byte)GameCity.ForestMode_Enum.ForestMode_Handsel;
        //model = (byte)GameCity.ForestMode_Enum.ForestMode_GiveGun;
        //model = (byte)GameCity.ForestMode_Enum.ForestMode_Four;
        //model = (byte)GameCity.ForestMode_Enum.ForestMode_Three;
        sign = msg.ReadByte();
        //sign = 2;
        osign = msg.ReadByte();
        handle = msg.ReadLong();
        signs.Clear();
        if (model == (byte)GameCity.ForestMode_Enum.ForestMode_Flash)
            power = msg.ReadByte();
        else if (model == (byte)GameCity.ForestMode_Enum.ForestMode_GiveGun)
        {
            for (int index = 0; index < sign; index++)
            {
                if (signs.Count > index)
                    signs[index] = msg.ReadByte();
                else
                    signs.Add(msg.ReadByte());
            }
        }
        //signs.Add(11);
        //signs.Add(12);
    }

    public byte model;       //模式
    public byte sign;        //押注索引
    public byte osign;       //庄和闲
    public long handle;      //彩金
    public byte power;       //闪电模式倍率
    public List<byte> signs; //送枪模式索引
}

public class ForestDanceChip : ForestDanceMessageBase
{
    public ForestDanceChip()
    {
        forestMsgType = (uint)GameCity.Forest_enum.ForestMsg_CM_CHIPIN;
    }

    public void SetSendData(UMessage msg)
    {
        //msg.Add(forestMsgType);
        msg.Add(sign);
        msg.Add(userID);
        msg.Add(chipCoin);
    }

    public byte sign;
    public uint userID;
    public long chipCoin;
}

public class ForestDanceSortData
{
    public uint playerid;
    public uint faceID;
    public string url;
    public long addNormalCoin;
    public string userName;
}

public class ForestDanceShowResult : ForestDanceMessageBase
{
    public ForestDanceShowResult()
    {
        forestMsgType = (uint)GameCity.Forest_enum.ForestMsg_SM_GAMECOUNT;
        sortData = new List<ForestDanceSortData>();
    }

    public void ReadData(UMessage msg)
    {
        handsel = msg.ReadLong();
        addRewardCoin = msg.ReadLong();
        addNormalCoin = msg.ReadLong();
        carryMoney = msg.ReadLong();
        int length = msg.ReadInt();
        for(int index = 0; index < length; index++)
        {
            if (sortData.Count > index)
            {
                sortData[index].playerid = msg.ReadUInt();
                sortData[index].faceID = msg.ReadUInt();
                sortData[index].url = msg.ReadString();
                sortData[index].addNormalCoin = msg.ReadLong();
                sortData[index].userName = msg.ReadString();
            }
            else
            {
                ForestDanceSortData data = new ForestDanceSortData();
                data.playerid = msg.ReadUInt();
                data.faceID = msg.ReadUInt();
                data.url = msg.ReadString();
                data.addNormalCoin = msg.ReadLong();
                data.userName = msg.ReadString();
                sortData.Add(data);
            }
        }
    }

    public long handsel;                        //彩金
    public long addRewardCoin;                  //从彩金中增加的钱
    public long addNormalCoin;                  //本局获得钱
    public long carryMoney;                     //本局钱
    public List<ForestDanceSortData> sortData;  //排名信息
}

public class BackGmToolAddItem
{
    public BackGmToolAddItem()
    {

    }

    public void ReadData(UMessage msg)
    {
        level = msg.ReadByte();
        coin = msg.ReadLong();
        diamond = msg.ReadUInt();
        accountID = msg.ReadUInt();
    }

    public byte level;
    public long coin;
    public uint diamond;
    public uint accountID;
}

public class InvateInfo
{
    public InvateInfo()
    {

    }

    public void ReadData(UMessage msg)
    {
        playerid = msg.ReadUInt();
        name = msg.ReadString();
        gameid = msg.ReadByte();
        level = msg.ReadInt();
        switch (level)
        {
            case 1:
                levelname = "新手场";
                break;
            case 2:
                levelname = "高手场";
                break;
            case 3:
                levelname = "专家场";
                break;
        }
        roomid = msg.ReadUInt();
    }

    public uint playerid;
    public string name;
    public byte gameid;
    public int level;
    public string levelname;
    public uint roomid;
}

public class InvateMsg
{
    public void SetSendData(UMessage msg)
    {
        msg.Add(playerid);
        msg.Add(memberid);
        msg.Add(gameid);
        msg.Add(level);
        msg.Add(roomid);
        msg.Add(roomNum);
    }

    public uint memberid;
    public uint playerid;
    public byte gameid;
    public int level;
    public uint roomid;
    public int roomNum;
}

public class TexasPokerTypeInfo
{
    public byte sign;
    public uint maxCarryIn;
    public uint blinds;
    public int currentPlayerNum;
}

public class TexasPokerLevelInfo
{
    public TexasPokerLevelInfo()
    {
        typeinfos = new List<TexasPokerTypeInfo>();
    }

    public byte level;
    public int types;
    public List<TexasPokerTypeInfo> typeinfos;
}

public class TexasPokerLogin
{
    public TexasPokerLogin()
    {
        levelinfos = new List<TexasPokerLevelInfo>();
    }

    public void ReadMessage(UMessage msg)
    {
        countTime = msg.ReadSingle();
        cutBlindsTime = msg.ReadSingle();
        onePokerTime = msg.ReadSingle();
        gameoverTime = msg.ReadSingle();
        askChipTime = msg.ReadSingle();
        maxPlayers = msg.ReadInt();
        levelNum = msg.ReadByte();

        for(byte index = 0; index < levelNum; index++)
        {
            TexasPokerLevelInfo levelinfo = new TexasPokerLevelInfo();
            levelinfo.level = msg.ReadByte();
            levelinfo.types = msg.ReadInt();
            for(int typeindex = 0; typeindex < levelinfo.types; typeindex++)
            {
                TexasPokerTypeInfo typeinfo = new TexasPokerTypeInfo();

                typeinfo.sign = msg.ReadByte();
                typeinfo.maxCarryIn = msg.ReadUInt();
                typeinfo.blinds = msg.ReadUInt();
                typeinfo.currentPlayerNum = msg.ReadInt();

                levelinfo.typeinfos.Add(typeinfo);
            }

            levelinfos.Add(levelinfo);
        }
    }

    public float countTime;
    public float cutBlindsTime;
    public float onePokerTime;
    public float gameoverTime;
    public float askChipTime;
    public int maxPlayers;
    public byte levelNum;

    public List<TexasPokerLevelInfo> levelinfos;
}

public class TexasPokerPlayerInfo
{
    public byte sitNo;
    public uint userid;
    public uint faceid;
    public string url;
    public long carryMoney;
    public string playerName;
    public byte currentCardNum;
    public long preChipNum;
    public long currChipNum;
}

public class TexasPokerEnterRoomMSG
{
    public TexasPokerEnterRoomMSG()
    {
        publiccards = new List<byte>();
        playerinfos = new Dictionary<byte, TexasPokerPlayerInfo>();
    }

    public void ReadMessage(UMessage msg)
    {
        roomid = msg.ReadUInt();
        carryMoney = msg.ReadLong();
        level = msg.ReadByte();
        state = msg.ReadByte();
        lefttime = msg.ReadSingle();
        whoisasked = msg.ReadByte();
        bosssitNo = msg.ReadByte();
        publiccardNum = msg.ReadByte();

        for (byte index = 0; index < publiccardNum; index++)
        {
            publiccards.Add(msg.ReadByte());
        }

        isOnPlayerNum = msg.ReadByte();

        for(byte index = 0; index < isOnPlayerNum; index++)
        {
            TexasPokerPlayerInfo tpp = new TexasPokerPlayerInfo();

            tpp.sitNo = msg.ReadByte();
            tpp.userid = msg.ReadUInt();
            tpp.faceid = msg.ReadUInt();
            tpp.url = msg.ReadString();
            tpp.carryMoney = msg.ReadLong();
            tpp.playerName = msg.ReadString();
            tpp.currentCardNum = msg.ReadByte();
            tpp.preChipNum = msg.ReadLong();
            tpp.currChipNum = msg.ReadLong();

            playerinfos.Add(tpp.sitNo, tpp);
        }
    }

    public uint roomid;
    public long carryMoney;
    public byte level;
    public byte state;
    public float lefttime;
    public byte whoisasked;
    public byte bosssitNo;
    public byte publiccardNum;
    public byte isOnPlayerNum;

    public Dictionary<byte, TexasPokerPlayerInfo> playerinfos;
    public List<byte> publiccards;
}

//5牛牛
public class BAKLevelConfig
{
    public int bossMoney;
    public int bossDownMoney;
    public byte bossTimes;
    public int vipSeatMoney;
}

public class BAKLoginMsg
{
    public BAKLoginMsg()
    {
        levelconfig = new Dictionary<byte, BAKLevelConfig>();
    }

    public void ReadConfig(UMessage msg)
    {
        chipTime = msg.ReadSingle();
        playerTime = msg.ReadSingle();
        overWaitTime = msg.ReadSingle();
        waitRobotTime = msg.ReadSingle();
        showPokerTime = msg.ReadSingle();
        levelNumber = msg.ReadByte();
        levelconfig.Clear();
        for (byte index = 1; index <= levelNumber; index++)
        {
            BAKLevelConfig config = new BAKLevelConfig();
            config.bossMoney = msg.ReadInt();
            config.bossDownMoney = msg.ReadInt();
            config.bossTimes = msg.ReadByte();
            config.vipSeatMoney = msg.ReadInt();
            byte level = index;
            levelconfig.Add(level, config);
        }
    }

    public float chipTime;
    public float playerTime;
    public float overWaitTime;
    public float waitRobotTime;
    public float showPokerTime;
    public byte levelNumber;
    public Dictionary<byte, BAKLevelConfig> levelconfig;
} 

public class BAKVipSeatInfo
{
    public BAKVipSeatInfo()
    {
        seatNo = 0;
        playerid = 0;
        faceid = 0;
        url = "";
        money = 0;
        name = "";
    }

    public byte seatNo;
    public uint playerid;
    public uint faceid;
    public string url;
    public long money;
    public string name;
}

public class BAKPokerInfo
{
    public BAKPokerInfo()
    {
        pokers = new List<byte>();
    }

    public long chipMoney;
    public byte pokerNumber;
    public List<byte> pokers;
    public byte pokertype;
    public int rewardRate;
}

public class BAKEnterRoomMsg
{
    public BAKEnterRoomMsg()
    {
        vipSeatInfos = new Dictionary<byte, BAKVipSeatInfo>();
        pokerinfos = new Dictionary<byte, BAKPokerInfo>();
        records = new List<byte>();

        vipSeatInfos.Clear();
        for (byte index = 1; index <= 4; index++)
        {
            vipSeatInfos.Add(index, new BAKVipSeatInfo());
        }
    }

    public void ReadEnterRommMsg(UMessage msg)
    {
        roomid = msg.ReadUInt();
        carryMoney = msg.ReadLong();
        roomlevel = msg.ReadByte();
        roomstate = msg.ReadByte();
        stateLastTime = msg.ReadSingle();
        bossid = msg.ReadUInt();
        if(bossid != 0)
        {
            bossFaceId = msg.ReadUInt();
            bossurl = msg.ReadString();
            bossCarryMoney = msg.ReadLong();
            bossName = msg.ReadString();
        }

        for (byte index = 1; index <= 4; index++)
        {
            vipSeatInfos[index].playerid = 0;
            vipSeatInfos[index].money = 0;
            vipSeatInfos[index].faceid = 0;
            vipSeatInfos[index].name = "";
        }

        vipPlayerNumber = msg.ReadByte();
        for(byte index = 0; index < vipPlayerNumber; index++)
        {
            BAKVipSeatInfo bvsi = new BAKVipSeatInfo();

            bvsi.seatNo = msg.ReadByte();
            bvsi.playerid = msg.ReadUInt();
            bvsi.faceid = msg.ReadUInt();
            bvsi.url = msg.ReadString();
            bvsi.money = msg.ReadLong();
            bvsi.name = msg.ReadString();

            vipSeatInfos[bvsi.seatNo] = bvsi;
        }

        pokersNumber = msg.ReadByte();
        pokerinfos.Clear();
        for (byte index = 0; index < pokersNumber; index++)
        {
            BAKPokerInfo bpi = new BAKPokerInfo();
            bpi.chipMoney = msg.ReadLong();
            bpi.pokerNumber = msg.ReadByte();
            for(byte pokerindex = 0; pokerindex < bpi.pokerNumber; pokerindex++)
            {
                bpi.pokers.Add(msg.ReadByte());
            }
            byte pokerIndex = index;
            pokerinfos.Add( pokerIndex, bpi );
        }

        byte recordLength = msg.ReadByte();
        records.Clear();
        for (byte index = 0; index < recordLength; index++)
        {
            records.Add(msg.ReadByte());
        }

        playerNumber = msg.ReadInt();
    }

    public uint roomid;
    public long carryMoney;
    public byte roomlevel;
    public byte roomstate;
    public float stateLastTime;
    public uint bossid;
    public uint bossFaceId;
    public string bossurl;
    public long bossCarryMoney;
    public string bossName;
    public byte vipPlayerNumber;
    public Dictionary<byte, BAKVipSeatInfo> vipSeatInfos;
    public byte pokersNumber;
    public Dictionary<byte, BAKPokerInfo> pokerinfos;
    public List<byte> records;
    public int playerNumber;
}

public class BAKBossInfo
{
    public BAKBossInfo()
    {

    }

    public uint bossid;
    public uint bossfaceid;
    public string bossurl;
    public long bossmoney;
    public string bossname;
}

public class BAKBossList
{
    public BAKBossList()
    {
        bossinfos = new Dictionary<uint, BAKBossInfo>();
    }

    public void ReadBossListData(UMessage msg)
    {
        length = msg.ReadInt();
        bossinfos.Clear();
        for (int index = 0; index < length; index++)
        {
            BAKBossInfo bbi = new BAKBossInfo();
            bbi.bossid = msg.ReadUInt();
            bbi.bossfaceid = msg.ReadUInt();
            bbi.bossurl = msg.ReadString();
            bbi.bossmoney = msg.ReadLong();
            bbi.bossname = msg.ReadString();

            bossinfos.Add(bbi.bossid, bbi);
        }
    }

    public int length;
    public Dictionary<uint, BAKBossInfo> bossinfos;
}

public class BAKSortResult
{
    public BAKSortResult()
    {

    }

    public uint playerid;
    public uint faceid;
    public string url;
    public long money;
    public string name;
}

public class BAKResultMsg
{
    public BAKResultMsg()
    {
        gameresults = new List<byte>();
        bsr = new List<BAKSortResult>();
        gameresulttypes = new List<byte>();
    }

    public void ReadResultMsg(UMessage msg)
    {
        bossName = msg.ReadString();
        bossmoney = msg.ReadLong();
        bossChangeMoney = msg.ReadLong();
        long playermoney = msg.ReadLong();
        //GameMain.hall_.GetPlayerData().SetCoin(msg.ReadLong());
        selfChangemoney = msg.ReadLong();
        byte length = msg.ReadByte();
        resultlength = length;
        gameresults.Clear();
        for (byte index = 0; index < length; index++)
        {
            gameresults.Add(msg.ReadByte());
        }

        byte vipNum = msg.ReadByte();
        for(byte index = 0; index < vipNum; index++)
        {
            byte sitNo = msg.ReadByte();
            BAK_DataCenter.Instance().ber.vipSeatInfos[sitNo].playerid = msg.ReadUInt();
            BAK_DataCenter.Instance().ber.vipSeatInfos[sitNo].money = msg.ReadLong();
        }

        byte sortLen = msg.ReadByte();
        bsr.Clear();
        for (byte index = 0; index < sortLen; index++)
        {
            BAKSortResult data = new BAKSortResult();
            data.playerid = msg.ReadUInt();
            data.faceid = msg.ReadUInt();
            data.url = msg.ReadString();
            data.money = msg.ReadLong();
            data.name = msg.ReadString();
            bsr.Add(data);
        }

        byte typelen = msg.ReadByte();
        for(int index = 0; index < typelen; index++)
        {
            byte pokersNo = msg.ReadByte();
            gameresulttypes.Add(msg.ReadByte());
        }
    }

    public string bossName;
    public long bossmoney;
    public long bossChangeMoney;
    public long selfChangemoney;
    public int resultlength;
    public List<byte> gameresults; //除以100得输赢
    public List<BAKSortResult> bsr;
    public List<byte> gameresulttypes;
}

public class ActiveInfo
{
    public ActiveInfo()
    {
        year = 0;
        month = 0;
        weekday = 0;
        day = 0;
        hour = 0;
        minute = 0;
    }

    public void ReadData(UMessage msg)
    {
        activeid = msg.ReadUInt();
        if (activeid == 0)
            return;
        activetype = msg.ReadByte();
        state = msg.ReadByte();
        year = msg.ReadShort();
        month = msg.ReadShort();
        weekday = msg.ReadShort();
        day = msg.ReadShort();
        hour = msg.ReadShort();
        minute = msg.ReadShort();
    }

    public uint activeid;
    public byte activetype;
    public byte state;
    public short year;
    public short month;
    public short weekday;
    public short day;
    public short hour;
    public short minute;
    public bool isget;
}