using System.Collections.Generic;using System.IO;

/// <summary>/// 服务器ab版本表字段/// </summary>public class CServerABVerData{    /// <summary>    /// assetbundle ID    /// </summary>    public uint AssetBundleId;          /// <summary>    /// 名称    /// </summary>             public string AssetBundleName;    /// <summary>    /// Assetbundle版本号    /// </summary>    public uint AssetbundleVersion;

    /// <summary>    /// AssetbundleMd5字符串    /// </summary>    public string AssetbundleMd5Str;


    /// <summary>    /// 说明    /// </summary>    public string DescriptionStr;}    /// <summary>/// 服务器资源AB版本表管理/// </summary>public class CServerABVerDataMgr{    private Dictionary<string, CServerABVerData> ABDataDictionary;    //private List<string[]> allArrary;    public CServerABVerDataMgr()    {        ABDataDictionary = new Dictionary<string, CServerABVerData>();    }    /// <summary>    /// 读取serverab版本表文件    /// </summary>    public void LoadServerABVerDataFile()    {       
        //CReadCsvBase.ReaderCsvData(GameDefine.CsvAssetbundleName, GameDefine.AssetbundleCsvFileName, out strList);

        string path = GameDefine.AppPersistentDataPath + GameDefine.ServerAssetbundleVersionDataFile;

        //检测文件是否存在
        if (!File.Exists(path))        {            HttpDownloadMgr.DownFile(GameDefine.LuancherURL, GameDefine.AppPersistentDataPath, GameDefine.ServerAssetbundleVersionDataFile);        }

        List<string[]> strList;
        CReadCsvBase.ReaderCsvDataFromFile(path, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            CServerABVerData tempData = new CServerABVerData();            uint.TryParse(strList[i][0], out tempData.AssetBundleId);            tempData.AssetBundleName = strList[i][1];            //tempData.DownloadUrlPath = strList[i][2];            uint.TryParse(strList[i][2], out tempData.AssetbundleVersion);            tempData.AssetbundleMd5Str = strList[i][3];            tempData.DescriptionStr = strList[i][4];            ABDataDictionary.Add(tempData.AssetBundleName,tempData);        }    }    /// <summary>    /// 获取ResABData    /// </summary>    /// <param name="AssetId"></param>    /// <returns></returns>    public CServerABVerData GetServerABVerData(string abname)    {        if (ABDataDictionary.ContainsKey(abname))            return ABDataDictionary[abname];        return null;    }}