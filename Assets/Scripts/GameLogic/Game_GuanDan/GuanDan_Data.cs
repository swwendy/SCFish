﻿using System.Collections.Generic;
public class GuanDan_RoomData
{
    public byte m_iDeskMaxPeople;       //一桌需要多少人
    public byte m_iNeedReady;           //是否需要玩家点击准备
    public float m_fGameBeginShowTime;  //开始前的动画显示多少时间
    public float m_fCountDownBeginTime; //开始倒计时几秒
    public float m_fDealPokersTime;     //开始发牌时间
    public float m_fSubmitPokerTime;    //上贡时间
    public float m_fReturnPokerTime;    //还贡时间
    public float m_fMatchSitTime;       //换座位的时间
    public float m_fOutPokerTime;       //出牌时间
}
    {
        m_RoomData.m_iDeskMaxPeople = msg.ReadByte(); 
        m_RoomData.m_iNeedReady = msg.ReadByte();
        m_RoomData.m_fGameBeginShowTime = msg.ReadSingle();
        m_RoomData.m_fCountDownBeginTime = msg.ReadSingle();
        m_RoomData.m_fDealPokersTime = msg.ReadSingle();
        m_RoomData.m_fSubmitPokerTime = msg.ReadSingle();
        m_RoomData.m_fReturnPokerTime = msg.ReadSingle();
        m_RoomData.m_fMatchSitTime = msg.ReadSingle();
        m_RoomData.m_fOutPokerTime = msg.ReadSingle();
    }