using System.Collections;using System.Collections.Generic;using UnityEngine;using System.IO;using System;using XLua;[Hotfix]public struct GameState{    public uint currentBossID;    public string bossName;    public uint faceID;    public string url;    public long BossMoney;    public long winMoney;    public char state;    public float leftTime;    public char carIcon;    public int roomLevel;    public List<CarInfo> infos;}public struct CarInfo{    public char carID;    public long totalMoney;    public long selfMoney;}public struct HistroyData{    public char historyIconNumber;    public char iconNumber;    public long winTotal;    public List<char> icons;             //历史记录（前几次）    public List<HistroyTimes> times;     //历史记录（总共出现次数）}public struct HistroyTimes{    public char iconID;    public long times;}public struct BossChangeInfo{    public uint changeBossID;    public string changeBossFaceid;    public string changeBossName;    public long changeBossMoney;}public struct GameResult{    public char carIndex;    public char two;}public class CH_DataCenter{    private static CH_DataCenter instance;    public GameState state;    public HistroyData histroy;    public BossChangeInfo boss;    public GameResult result;    public bool isStartChips;    public Dictionary<int, uint> generic_;    public bool isKickOut;    private CH_DataCenter()    {        state = new GameState();        state.roomLevel = 0;        state.infos = new List<global::CarInfo>();        histroy = new HistroyData();        histroy.icons = new List<char>();        histroy.times = new List<HistroyTimes>();        histroy.winTotal = 0;        boss = new BossChangeInfo();        result = new GameResult();        generic_ = new Dictionary<int, uint>();        isKickOut = false;        LoadGameDataFile();    }    public static CH_DataCenter Instance()    {        if (CH_DataCenter.instance == null)            CH_DataCenter.instance = new CH_DataCenter();        return CH_DataCenter.instance;    }

    public void LoadGameDataFile()    {        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.CarBetFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            ChipsLimitData limitdata = new ChipsLimitData();            uint level;            uint.TryParse(strList[i][0], out level);            uint one;            uint.TryParse(strList[i][1], out one);            generic_.Add(1, one);            uint two;            uint.TryParse(strList[i][2], out two);            generic_.Add(2, two);            uint three;            uint.TryParse(strList[i][3], out three);            generic_.Add(3, three);            uint four;            uint.TryParse(strList[i][4], out four);            generic_.Add(4, four);            uint five;            uint.TryParse(strList[i][5], out five);            generic_.Add(5, five);            uint six;            uint.TryParse(strList[i][6], out six);            generic_.Add(6, six);        }    }}public class CH_BossCondition
{
    public CH_BossCondition()
    {
        filename = "CarConfig";
        bossbases = new Dictionary<int, int>();
    }

    static public CH_BossCondition GetInstance()
    {
        if(instance_ == null)
        {
            instance_ = new CH_BossCondition();
            instance_.ReadIniConfig();
        }

        return instance_;
    }

    public void ReadIniConfig()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.CsvAssetbundleName);
        if (bundle == null)
            return;

        TextAsset datatxt = bundle.LoadAsset(filename) as TextAsset;        if (datatxt == null)            return;        MemoryStream stream = new MemoryStream(datatxt.bytes);        StreamReader filereader = new StreamReader(stream);

        string content = filereader.ReadToEnd();

        INIParser ip = new INIParser();
        ip.OpenFromString(content);

        bossbases.Add(1, ip.ReadValue("level_1", "bossmoneybase", 0));
        bossbases.Add(2, ip.ReadValue("level_2", "bossmoneybase", 0));

        ip.Close();
    }

    public string filename;
    public Dictionary<int, int> bossbases;
    private static CH_BossCondition instance_;
}