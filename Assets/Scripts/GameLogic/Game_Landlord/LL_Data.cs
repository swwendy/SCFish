﻿using System.Collections.Generic;
public class LL_RoomData
{
    public int m_nGameNum;
    public int m_nBaseCoin;
    public int m_nMinCoin;
    public int m_nRoomCharge;
}
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

    }