using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfContestRule
{
    public SelfContestRule()
    {
        playernumber = llrplayerNumberList[0];
        precontest = llrprecontestList[0];
        finalcontest = llrprecontestList[0];
        cost = costList[0];
        timeseconds = (uint)GameCommon.ConvertDataTimeToLong(System.DateTime.Now.Date) + 3600;
    }

    public static byte[] llrplayerNumberList = { 6, 12, 24, 48, 96 };    public static byte[] llrprecontestList = { 3, 6, 9 };
    public static byte[] gdplayerNumberList = { 8, 16, 32, 64, 128 };
    public static byte[] gdprecontestList = { 3, 5, 10, 20 };
    public static byte[] costList = { 0, 10, 50, 100 };
    public static byte[] gdprecontestChessList = { 1, 2, 3, 4 };

    public ushort playernumber;
    public byte precontest;
    public byte finalcontest;
    public ushort cost;
    public uint timeseconds;
}

public class SelfContestCSVData
{
    public SelfContestCSVData()
    {
        datas = new List<int>();
    }

    public byte gameid;
    public List<int> datas;
}

public class SelfContestDataManager
{
    SelfContestDataManager()
    {
        selfcontestcsvs = new Dictionary<byte, SelfContestCSVData>();
        ReadSelfContestCSVData();
    }

    void ReadSelfContestCSVData()
    {
        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.SelfContestFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            SelfContestCSVData data = new SelfContestCSVData();            byte.TryParse(strList[i][0], out data.gameid);            for (int index = 1; index < 4; index++)
            {
                int temp = 0;
                int.TryParse(strList[i][index], out temp);
                data.datas.Add(temp);
            }            selfcontestcsvs.Add(data.gameid, data);        }
    }

    public static SelfContestDataManager instance()
    {
        if (instance_ == null)
            instance_ = new SelfContestDataManager();

        return instance_;
    }

    public void SetGameDataID(int index)
    {
        gamedataid = contestdataids[index];
    }

    public byte gameid;
    public Dictionary<byte, SelfContestCSVData> selfcontestcsvs;
    static SelfContestDataManager instance_;
    public uint gamedataid;

    static uint[] contestdataids = { 2001, 2002, 2000, 2011, 2012, 2010, 2013 };
}
