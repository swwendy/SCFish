﻿using System;
using System.Collections.Generic;
using System.Linq;
using XLua;

/// <summary>
    public string GameSceneName;             //游戏场景名
    public string SceneABName;               //场景资源包名称
    public string ResourceABName;            //游戏资源包名称
    public byte   BundleTotalSize;           //场景和资源asset包的总大小
    public string GameTextIcon;              //游戏文字图标
    private Dictionary<float, string> MasterLvDictionary;


    //private List<string[]> allArrary;

    public CGameDataMgr()
        GameDataDictionary = new Dictionary<byte, GameData>();

            gamedata.GameName = strList[i][1];
            gamedata.GameIcon = strList[i][2];
            gamedata.GameSceneName = strList[i][3];
            gamedata.SceneABName = strList[i][4];
            gamedata.ResourceABName = strList[i][5];
            byte.TryParse(strList[i][7], out gamedata.BundleTotalSize);
            gamedata.GameTextIcon = strList[i][9];

            GameDataDictionary.Add(gamedata.GameID, gamedata);

        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, "MasterLevCsv", out strList);

    /// <summary>
    {
        foreach(var m in MasterLvDictionary)
        {
            if (score >= m.Key)
                return m.Value;
        }

        return "";
    }