using System.Collections.Generic;
using System.Linq;

/// <summary>/// 提示数据/// </summary>
public class VipData{
    public uint VipLv;                    //Vip等级    
    public string VipPrice;               //充值数量
    public string Discount;               //充值返利
};

/// <summary>/// 提示管理/// </summary>
public class CVipDataMgr
{
    private Dictionary<uint, VipData> VipDataDictionary;

    public CVipDataMgr()
    {
        VipDataDictionary = new Dictionary<uint, VipData>();
    }

    /// <summary>    /// 读取提示表    /// </summary>
    public void LoadVipCsvFile()
    {
        List<string[]> strList;
        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.VipCsvFileName, out strList);

        int columnCount = strList.Count;
        for (int i = 2; i < columnCount; i++)
        {
            VipData vipdata = new VipData();
            uint.TryParse(strList[i][0], out vipdata.VipLv);

            vipdata.VipPrice = strList[i][1];
            vipdata.Discount = strList[i][2];

            VipDataDictionary.Add(vipdata.VipLv, vipdata);
        }
    }

    /// <summary>    /// 获取提示数据data    /// </summary>    /// <param name="VipLv"></param>    /// <returns></returns>
    public VipData GetVipData(uint vipLv)
    {
        if (VipDataDictionary.ContainsKey(vipLv))
            return VipDataDictionary[vipLv];
        return null;
    }

    public VipData GetMaxVipData()
    {
        if (VipDataDictionary.Count == 0)
        {
            return null;
        }
        //VipData vipdata = VipDataDictionary.Keys.Cast<uint>().Select(x => new { x, y = VipDataDictionary[x] }).OrderBy(x => x.y).Last().y;
        VipData vipdata = VipDataDictionary.Last().Value;
        return vipdata;
    }
}
