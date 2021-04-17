using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;

//约据房间座位上玩家数据
public class AppointmentSeat
{
    public AppointmentSeat()
    {
        Clear();
    }

    public void Clear()
    {
        playerid = 0;
        playerName = "";
        icon = 0;
        //seatid = 0;
        already = false;
    }

    public uint playerid;
    public string playerName;
    public int icon;
    //public uint seatid;
    public bool already;
    public string url;
}

public class AppointmentData
{
    public uint roomid;
    public byte playtimes;
    public byte maxpower;
    public int bet;
    public int incoin;
    public int outcoin;
    public byte gameid;
    public uint currentPeople;
    public uint maxPlayer;
    public uint hostid;
    public bool isend;
    public long createtimeseconds;
    public bool isopen;
    public Dictionary<byte, AppointmentSeat> seats_;

    public AppointmentData()
    {

    }

    public AppointmentData(uint max)
    {
        maxPlayer = max;
        seats_ = new Dictionary<byte, AppointmentSeat>();
        for (byte index = 0; index < maxPlayer; index++)
        {
            AppointmentSeat seat = new AppointmentSeat();
            seats_.Add(index, seat);
            //seat.seatid = index;
        }

        isend = false;
    }

    //解析公开约据房间数据
    public void ReadPublicAppointmentData(UMessage msg)
    {
        bet = msg.ReadInt();
        incoin = msg.ReadInt();
        outcoin = msg.ReadInt();
        roomid = msg.ReadUInt();
        currentPeople = msg.ReadUInt();
    }

    //解析游戏规则数据
    public virtual void ReadAppointmentDataMessage(UMessage msg,bool isPublicRoom = false)
    {
        playtimes = msg.ReadByte();
        maxpower = msg.ReadByte();
        if (isPublicRoom)
        {
            ReadPublicAppointmentData(msg);
        }
    }

    /// <summary>
    /// 设置约据房间权限
    /// </summary>
    /// <param name="isOpenState">true: 公开的 false : 私密的</param>
    public void SetIsOpenRoom(bool isOpenState)
    {
        isopen = isOpenState;
    }

    public void ClearSeats()
    {
        foreach (byte key in seats_.Keys)
        {
            seats_[key].playerid = 0;
        }
    }

    public bool IsFull()
    {
        bool result = true;

        foreach (byte key in seats_.Keys)
        {
            if (seats_[key].playerid == 0)
            {
                result = false;
                break;
            }
        }

        return result;
    }

}

//掼蛋(约据房间)
public class  GuanDanAppointmentData: AppointmentData
{
    public GuanDanAppointmentData()
    {
        terData_ = new ThrowEggsRule();
    }

    public GuanDanAppointmentData(uint max): base(max)
    {
        terData_ = new ThrowEggsRule();
    }

    public override void ReadAppointmentDataMessage(UMessage msg, bool isPublicRoom = false)
    {
        base.ReadAppointmentDataMessage(msg);
        if(isPublicRoom)
        {
            base.ReadPublicAppointmentData(msg);
        }
   
        terData_.playType = (TePlayType)msg.ReadByte();
        if (terData_.playType == TePlayType.Goon)
            terData_.vectory = msg.ReadByte();
        else
        {
            terData_.times = msg.ReadByte();
            terData_.score = msg.ReadByte();
            terData_.cp = (CurrentPoker)msg.ReadByte();
        }
    }
    public ThrowEggsRule terData_;
}

//血战(约据房间)
public class MahjongAppointmentData : AppointmentData
{
    public MahjongAppointmentData()
    {

    }

    public MahjongAppointmentData(uint max): base(max)
    {

    }

    public override void ReadAppointmentDataMessage(UMessage msg, bool isPublicRoom = false)
    {
        base.ReadAppointmentDataMessage(msg);
        if (isPublicRoom)
        {
            base.ReadPublicAppointmentData(msg);
        }

        palyType = msg.ReadShort();
    }
    public short palyType;//
}


//盐城麻将(约据房间)
public class YcMahjongAppointmentData : AppointmentData
{
    public YcMahjongAppointmentData()
    {

    }

    public YcMahjongAppointmentData(uint max) : base(max)
    {

    }

    public override void ReadAppointmentDataMessage(UMessage msg, bool isPublicRoom = false)
    {
        base.ReadAppointmentDataMessage(msg);
        if (isPublicRoom)
        {
            base.ReadPublicAppointmentData(msg);
        }
    }
}

//常州麻将(约据房间)
public class CzMahjongAppointmentData : AppointmentData
{
    public CzMahjongAppointmentData()
    {
    }

    public CzMahjongAppointmentData(uint max): base(max)
    {

    }

    public override void ReadAppointmentDataMessage(UMessage msg, bool isPublicRoom = false)
    {
        base.ReadAppointmentDataMessage(msg);
        if (isPublicRoom)
        {
            base.ReadPublicAppointmentData(msg);
        }

        wanFa = msg.ReadShort();
        qiHu  = msg.ReadByte();
        diHua = msg.ReadByte();
    }

    //玩法 包三口/包四口/擦背/吃
    public short wanFa;
    //起胡
    public byte qiHu;
    //底花
    public byte diHua;
}


//够级(约据房间)
public class GoujiAppointmentData : AppointmentData
{
    public GoujiAppointmentData()
    {
    }

    public GoujiAppointmentData(uint max) : base(max)
    {

    }

    public override void ReadAppointmentDataMessage(UMessage msg, bool isPublicRoom = false)
    {
        base.ReadAppointmentDataMessage(msg);
        if (isPublicRoom)
        {
            base.ReadPublicAppointmentData(msg);
        }

        wanFa = msg.ReadByte();
    }

    //玩法 
    public byte wanFa;
}

//红中(约据房间)
public class HzMahjongAppointmentData : AppointmentData
{
    public HzMahjongAppointmentData()
    {
    }

    public HzMahjongAppointmentData(uint max) : base(max)
    {

    }

    public override void ReadAppointmentDataMessage(UMessage msg, bool isPublicRoom = false)
    {
        base.ReadAppointmentDataMessage(msg);
        if (isPublicRoom)
        {
            base.ReadPublicAppointmentData(msg);
        }
        wanFa = msg.ReaduShort();
        birdNum = msg.ReadByte();

    }

    //抓鸟数
    public byte birdNum;
    //玩法 
    public ushort wanFa;
}

//象棋(约据房间)
public class ChessAppointmentData : AppointmentData
{
    public ChessAppointmentData()
    {
    }

    public ChessAppointmentData(uint max) : base(max)
    {

    }

    public override void ReadAppointmentDataMessage(UMessage msg, bool isPublicRoom = false)
    {
        base.ReadAppointmentDataMessage(msg);
        if (isPublicRoom)
        {
            base.ReadPublicAppointmentData(msg);
        }
        ChessTime = msg.ReadUInt()/60;
    }

    //对战局数
    public byte ChessTimes;
    //局时间
    public uint ChessTime;
}

public class AppointmentResult
{
    public byte sitNo;
    public uint playerid;
    public long coin;
}

public class AppointmentCSVData
{
    public AppointmentCSVData()
    {
        datas = new List<int>();
    }

    public byte gameid;
    public List<int> datas;
    //public int incoin;
    //public int outcoin;
    //public int bet;
}

public enum AppointmentKind
{
    From_Appointment = 0,
    From_Moments,
}

public class AppointmentDataManager
{
    AppointmentDataManager()
    {
        rooms_ = new Dictionary<uint, AppointmentData>();
        playerSitNo = 0;
        playerAlready = false;
        resultList = new List<AppointmentResult>();
        perResultList_ = new List<Dictionary<byte, AppointmentResult>>();
        gameid = (byte)GameKind_Enum.GameKind_LandLords;
        appointmentcsvs_ = new Dictionary<byte, AppointmentCSVData>();
        kind = AppointmentKind.From_Appointment;

        interruptid = 0;
        interrupt = false;
        interruptName = "";

        matchrooms_ = new List<MatchRooms>();

        ReadAppointmentCSVData();
    }

    public static AppointmentDataManager AppointmentDataInstance()
    {
        if (instance_ == null)
            instance_ = new AppointmentDataManager();

        return instance_;
    }

    public void Clear()
    {
        if (rooms_ == null)
            rooms_ = new Dictionary<uint, AppointmentData>();
        else
            rooms_.Clear();

        playerSitNo = 0;
        playerAlready = false;
        resultList = new List<AppointmentResult>();
        perResultList_ = new List<Dictionary<byte, AppointmentResult>>();
        gameid = (byte)GameKind_Enum.GameKind_LandLords;
        currentRoomID = 0;
        currentsource = 0;
    }

    public AppointmentData GetCurrentAppointment()
    {
        return GetAppointmentData(currentRoomID);
    }

    void ReadAppointmentCSVData()
    {
        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.AppointmentFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            AppointmentCSVData data = new AppointmentCSVData();            byte.TryParse(strList[i][0], out data.gameid);            for (int index = 1; index < 7; index++)
            {
                int temp = 0;
                int.TryParse(strList[i][index], out temp);
                data.datas.Add(temp);
            }            //int.TryParse(strList[i][8], out data.incoin);            //int.TryParse(strList[i][9], out data.outcoin);            //int.TryParse(strList[i][10], out data.bet);            appointmentcsvs_.Add(data.gameid, data);        }
    }

    /// <summary>
    /// 添加约据房间数据(包括公开和私密)
    /// </summary>
    /// <param name="id">约据ID</param>
    /// <param name="data">约据房间数据</param>
    public void AddAppointmentData(uint id,AppointmentData data)
    {
        if(!rooms_.ContainsKey(id))
        {
            rooms_.Add(id,data);
        }else
        {
            rooms_[id] = data;
        }
    }

    /// <summary>
    /// 获得约据房间数据
    /// </summary>
    /// <param name="id">约据ID</param>
    /// <returns>约据房间数据</returns>
    public AppointmentData GetAppointmentData(uint id)
    {
        if (!rooms_.ContainsKey(id))
            return null;

        return rooms_[id];
    }

    static AppointmentDataManager instance_;

    Dictionary<uint, AppointmentData> rooms_;
    public uint currentRoomID;
    public byte currentsource;
    public byte gameid;
    public byte playerSitNo;
    public bool playerAlready;
    public List<AppointmentResult> resultList;
    public List<Dictionary<byte, AppointmentResult>> perResultList_;

    public Dictionary<byte, AppointmentCSVData> appointmentcsvs_;
    public AppointmentKind kind;

    public bool interrupt;
    public uint interruptid;
    public string interruptName;

    public List<MatchRooms> matchrooms_;
}

public class MatchRooms
{
    public string descript_;
    public int incoin_;
}

//房卡记录
public class AppointmentRecordPlayer
{
    public uint playerid;
    public int faceid;
    public string playerName;
    public long coin;
    public string url;
    public float master;
    public byte sex;
    public byte ready;
}

public class AppointmentRecord
{
    public AppointmentRecord()
    {
        result = new Dictionary<uint, AppointmentRecordPlayer>();
        isopen = false;
    }

    public byte gameID;
    public long recordTimeSeconds;
    public Dictionary<uint, AppointmentRecordPlayer> result;
    public bool isopen;
    public long timeseconds;
    public byte maxpower;
    public byte gameTimes;
    public string gamerule;
    public long videoes;
}
