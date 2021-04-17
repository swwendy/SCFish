﻿using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 游戏界面资源数据
/// </summary>
public class CGameUIData
    public string HorizontalPrefab;         //横屏界面资源
    public string VerticalPrefab;           //竖屏界面资源
};

/// <summary>
public class CGameUIDataMgr
{
    private Dictionary<string, CGameUIData> GameUIDataDictionary;

    public CGameUIDataMgr()
    {
        GameUIDataDictionary = new Dictionary<string, CGameUIData>();
    }

    /// <summary>
    public void LoadGameUICsvFile()
    {
        List<string[]> strList;
        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.GameUIFileName, out strList);

        int columnCount = strList.Count;
        for (int i = 2; i < columnCount; i++)
        {
            CGameUIData GameUIData = new CGameUIData();

            GameUIData.HorizontalPrefab = strList[i][1];
            GameUIData.VerticalPrefab = strList[i][2];

            GameUIDataDictionary.Add(strList[i][0], GameUIData);
        }
    }

    /// <summary>
    public CGameUIData GetGameUIData(string GameUIName)
    {
        if (GameUIDataDictionary.ContainsKey(GameUIName))
            return GameUIDataDictionary[GameUIName];
        return null;
    }

    /// <summary>
    /// 获得界面资源名称
    /// </summary>
    /// <param name="GameUIName"></param>
    /// <param name="GameKind"></param>
    /// <returns></returns>
    public string GetGameUIPrefabName(string GameUIName,GameKind_Enum GameKind)
    {
        if (GameUIDataDictionary.ContainsKey(GameUIName))
        {
            if(GameKind == GameKind_Enum.GameKind_Chess)
            {
                return GameUIDataDictionary[GameUIName].VerticalPrefab;
            }
            return GameUIDataDictionary[GameUIName].HorizontalPrefab;
        }
        return string.Empty;
    }
}