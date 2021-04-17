using System.Collections.Generic;


/// <summary>/// 提示数据/// </summary>
public class TipsData{    public uint TipsID;                    //TipsID    
    public string TipsTitle;               //提示标题
    public string TipsText;                //提示内容

};


/// <summary>/// 提示管理/// </summary>public class CTipsDataMgr{    private Dictionary<uint, TipsData> TipsDataDictionary;

    public CTipsDataMgr()    {
        TipsDataDictionary = new Dictionary<uint, TipsData>();    }

    /// <summary>    /// 读取提示表    /// </summary>    public void LoadTipsCsvFile()    {        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.TipsCsvFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            TipsData tipsdata = new TipsData();            uint.TryParse(strList[i][0], out tipsdata.TipsID);

            tipsdata.TipsTitle = strList[i][1];
            tipsdata.TipsText = strList[i][2];


            TipsDataDictionary.Add(tipsdata.TipsID, tipsdata);        }

    }

    /// <summary>    /// 获取提示数据data    /// </summary>    /// <param name="tipsId"></param>    /// <returns></returns>    public TipsData GetTipsData(uint tipsId)    {        if (TipsDataDictionary.ContainsKey(tipsId))            return TipsDataDictionary[tipsId];        return null;    }    public string GetTipsText(uint tipsId, params object[] param)
    {
        if(TipsDataDictionary.ContainsKey(tipsId))
        {
            return string.Format(TipsDataDictionary[tipsId].TipsText, param);
        }
        return null;
    }}