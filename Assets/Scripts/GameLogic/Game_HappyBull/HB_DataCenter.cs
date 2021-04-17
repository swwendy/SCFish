using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;using XLua;

public class HB_PokerInfo
{
    public HB_PokerInfo()
    {
        pokers = new List<byte>();
    }

    public long chipMoney;
    public byte pokerNumber;
    public List<byte> pokers;
    public byte pokertype;
    public int rewardRate;
}

public class HB_PickData
{
    public int maxMoney;
    public int minMoney;
    public int baseBet;
}

public class HB_PlayerData
{
    public uint playerid;
    public string url;
    public int faceid;
    public long totalcoin;
    public string name;
    public byte sign;
}

public class HB_RoomData
{
    public HB_RoomData()
    {
        playersData = new List<HB_PlayerData>();
    }

    public uint roomid;
    public byte roomLevel;
    public int roleNumber;
    public long getMoney;
    public byte roomState;
    public float leftTime;

    public List<HB_PlayerData> playersData;
}

public class HB_PokerType{    public int pokertypeNO;    public string pokertype;    public int rewardrate;}

public class HB_DataCenter
{
    public List<HB_PickData> pickdatas;
    static HB_DataCenter instance_;
    public HB_RoomData roomdata_;
    public float counttime;
    public List<HB_PokerInfo> pokerinfos;
    public Dictionary<byte, HB_PokerType> pokertypes;
    public byte roomstate;

    public HB_DataCenter()
    {
        pickdatas = new List<HB_PickData>();
        roomdata_ = new HB_RoomData();
        pokerinfos = new List<HB_PokerInfo>();
        pokertypes = new Dictionary<byte, HB_PokerType>();

        ReadPokerTypeConfig();
    }

    public static HB_DataCenter Instance()
    {
        if (instance_ == null)
            instance_ = new HB_DataCenter();

        return instance_;
    }

    public void ReadLevelConfigFromServer(UMessage msg)
    {
        byte length = msg.ReadByte();

        for(int index = 0; index < length; index++)
        {
            HB_PickData data = new HB_PickData();

            data.maxMoney = msg.ReadInt();
            data.minMoney = msg.ReadInt();
            data.baseBet = msg.ReadInt();

            pickdatas.Add(data);
        }
    }

    void ReadPokerTypeConfig()    {        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.BullAllKillPokerTypeFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            HB_PokerType pokertype = new HB_PokerType();            byte key = 0;            byte.TryParse(strList[i][0], out key);            pokertype.pokertypeNO = key;            pokertype.pokertype = strList[i][1];            int.TryParse(strList[i][2], out pokertype.rewardrate);            pokertypes.Add(key, pokertype);        }    }
}
