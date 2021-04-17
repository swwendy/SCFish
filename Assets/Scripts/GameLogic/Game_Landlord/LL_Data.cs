using System.Collections.Generic;using UnityEngine;using USocket.Messages;using System.IO;using XLua;[LuaCallCSharp]
public class LL_RoomData
{
    public int m_nGameNum;
    public int m_nBaseCoin;
    public int m_nMinCoin;
    public int m_nRoomCharge;
}[Hotfix]public class LandLords_Data{    public byte m_nCountdownTime;    public byte m_nDealPokerTime;    public byte m_nAskLordTime;    public byte m_nOutPokerTime;    public byte m_nGameEndTime;    public List<LL_RoomData> m_RoomData = new List<LL_RoomData>();    LandLords_Data()    {    }    public static LandLords_Data GetInstance()    {        if (instance == null)            instance = new LandLords_Data();        return instance;    }    static LandLords_Data instance;    public void ReadData(UMessage msg)
    {
        m_nCountdownTime = msg.ReadByte();
        m_nDealPokerTime = msg.ReadByte();
        m_nAskLordTime = msg.ReadByte();
        m_nOutPokerTime = msg.ReadByte();
        m_nGameEndTime = msg.ReadByte();

        m_RoomData.Clear();
        byte levelNum = msg.ReadByte();
        for (int i = 0; i < levelNum; i++)
        {
            LL_RoomData data = new LL_RoomData();
            data.m_nGameNum = msg.ReadInt();
            data.m_nBaseCoin = msg.ReadInt();
            data.m_nMinCoin = msg.ReadInt();
            data.m_nRoomCharge = msg.ReadInt();
            m_RoomData.Add(data);
        }

    }}