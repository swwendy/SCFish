using System;
using System.Collections.Generic;
using System.Linq;
using XLua;

/// <summary>/// 游戏数据/// </summary>[LuaCallCSharp] public class GameData{    public byte   GameID;                    //GameID        public string GameName;                  //游戏名称    public string GameIcon;                  //游戏ICON
    public string GameSceneName;             //游戏场景名
    public string SceneABName;               //场景资源包名称
    public string ResourceABName;            //游戏资源包名称
    public byte   BundleTotalSize;           //场景和资源asset包的总大小
    public string GameTextIcon;              //游戏文字图标};/// <summary>/// 商城商品数据管理/// </summary>[LuaCallCSharp]public class CGameDataMgr{    private Dictionary<byte, GameData> GameDataDictionary;
    private Dictionary<float, string> MasterLvDictionary;


    //private List<string[]> allArrary;

    public CGameDataMgr()    {
        GameDataDictionary = new Dictionary<byte, GameData>();        MasterLvDictionary = new Dictionary<float, string>();    }    /// <summary>    /// 读取游戏数据表    /// </summary>    public void LoadGameDataFile()    {        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.GameCsvFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            GameData gamedata = new GameData();            byte.TryParse(strList[i][0], out gamedata.GameID);

            gamedata.GameName = strList[i][1];
            gamedata.GameIcon = strList[i][2];
            gamedata.GameSceneName = strList[i][3];
            gamedata.SceneABName = strList[i][4];
            gamedata.ResourceABName = strList[i][5];
            byte.TryParse(strList[i][7], out gamedata.BundleTotalSize);
            gamedata.GameTextIcon = strList[i][9];

            GameDataDictionary.Add(gamedata.GameID, gamedata);        }

        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, "MasterLevCsv", out strList);        columnCount = strList.Count;        float min;        for (int i = 2; i < columnCount; i++)        {            float.TryParse(strList[i][0], out min);            MasterLvDictionary.Add(min, strList[i][2]);        }        MasterLvDictionary = MasterLvDictionary.OrderByDescending(o => o.Key).ToDictionary(o => o.Key, p => p.Value);    }

    /// <summary>    /// 获取商品数据data    /// </summary>    /// <param name="itemId"></param>    /// <returns></returns>    public GameData GetGameData(byte  gameId)    {        if (GameDataDictionary.ContainsKey(gameId))            return GameDataDictionary[gameId];        return null;    }    public string GetMasterLv(float score)
    {
        foreach(var m in MasterLvDictionary)
        {
            if (score >= m.Key)
                return m.Value;
        }

        return "";
    }}