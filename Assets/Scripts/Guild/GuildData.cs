using UnityEngine;using System.Collections;using System.Collections.Generic;using USocket.Messages;


public class GuildCsvData
{
    public byte ClubLv;
    public string ClubLevelName;
    public long ClubMinExp;
    public short MaxPlayer;
    public byte AttenuationCycle;
    public long AttenuationNum;
    public string Discount;
    public long GiveLimit;
}

public class GuildPlayerData
{
    public uint useid;
    public string name;
    public uint vipLevel;
    public long active;
    public bool online;
    public uint icon;
    public string url;
}
/// <summary>/// 公会数据 等级 最大人数 会长，成员等/// </summary>public class GuildData{    private static GuildData intance_;    GuildData()
    {
        GuildID = 0;        GuildName = "";        GuildCreatePlayerName = "";        GuildLevel = 1;        GuildMemberNum = 0;        GuildMaxMemberNum = 0;        GuildMoney = 0;        GuildContent = "";        GuildActive = 0;        GuildBreakTime = 0;

        m_GuildMemberList = new List<GuildPlayerData>();
        m_GuildCsvData = new List<GuildCsvData>();
        LoadGuildCsvData();
    }    private void LoadGuildCsvData()
    {
        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.ClubFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            GuildCsvData csvdata = new GuildCsvData();            byte.TryParse(strList[i][0], out csvdata.ClubLv);
            csvdata.ClubLevelName = strList[i][1];
            long.TryParse(strList[i][2], out csvdata.ClubMinExp);
            short.TryParse(strList[i][3], out csvdata.MaxPlayer);
            byte.TryParse(strList[i][4], out csvdata.AttenuationCycle);
            long.TryParse(strList[i][5], out csvdata.AttenuationNum);
            csvdata.Discount = strList[i][6];
            long.TryParse(strList[i][7], out csvdata.GiveLimit);

            m_GuildCsvData.Add(csvdata);        }
    }    public static GuildData Instance()
    {
        if (intance_ == null)
        {
            intance_ = new GuildData();
        }

        return intance_;
    }    //公会ID    public  uint GuildID;

    //公会名
    public string GuildName;

    //公会创建者名称
    public string GuildCreatePlayerName;

    //工会创建者id
    public uint GuildCreatePlayerID;

    //公会等级
    public uint GuildLevel;

    //公会当前人数
    public uint GuildMemberNum;

    //公会最大人数
    public uint GuildMaxMemberNum;

    //公会经济
    public float GuildMoney;

    //公会宣言
    public string GuildContent;

    //公会活跃度
    public long GuildActive;

    //公会解散时间（s）
    public uint GuildBreakTime;

    //加入公会失败原因
    public byte failedid;

    //公会成员列表
    public List<GuildPlayerData> m_GuildMemberList;
    public List<GuildCsvData> m_GuildCsvData;

    //填充公会数据
    public void InitData(UMessage msg)    {        GuildID = msg.ReadUInt();        if (GuildID == 0)
        {
            failedid = msg.ReadByte();
            return;
        }        GuildName = msg.ReadString();        GuildCreatePlayerID = msg.ReadUInt();        GuildCreatePlayerName = msg.ReadString();        GuildLevel = msg.ReadUInt();        GuildMemberNum = msg.ReadUInt();        GuildMaxMemberNum = msg.ReadUInt();        GuildMoney = msg.ReadSingle();        GuildContent = msg.ReadString();        GuildActive = msg.ReadLong();        GuildBreakTime = msg.ReadUInt();        m_GuildMemberList.Clear();        for (uint index = 0; index < GuildMemberNum; index++)
        {
            GuildPlayerData member = new GuildPlayerData();
            member.useid = msg.ReadUInt();
            member.name = msg.ReadString();
            member.vipLevel = msg.ReadUInt();
            member.active = msg.ReadLong();
            member.online = msg.ReadBool();
            member.icon = msg.ReadUInt();
            member.url = msg.ReadString();

            m_GuildMemberList.Add(member);
        }        failedid = 0;    }}