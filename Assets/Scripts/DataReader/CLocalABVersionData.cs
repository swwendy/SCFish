using System.Collections.Generic;using System.IO;

/// <summary>/// 本地资源AB版本数据/// </summary>///  public class LocalABVersion
{
    public string abname; //ab名称
    public uint version; //版本号

    public LocalABVersion(string abnameIn, uint intVersionIn)
    {
        abname = abnameIn;
        version = intVersionIn;
    }
};

/// <summary>
/// 本地资源AB版本数据管理
/// </summary>public class CLocalABVerDataMgr{    private Dictionary<string, LocalABVersion> LocalABVersionDictionary;    //private List<string[]> allArrary;    public CLocalABVerDataMgr()    {        LocalABVersionDictionary = new Dictionary<string, LocalABVersion>();    }    /// <summary>    /// 读取本地ab版本数据    /// </summary>    public void LoadLocalABVerDataFile()    {
        string path = GameDefine.AssetBundleSavePath + GameDefine.LocalAssetbundleVersionDataFile;

        //检测文件是否存在
        if (!File.Exists(path))        {            return;        }

        List<string[]> strlist;
        CReadCsvBase.ReaderCsvDataFromFile(path, out strlist);        int columnCount = strlist.Count;        for (int i = 0; i < columnCount; i++)        {            string abname = strlist[i][0];            uint version;            uint.TryParse(strlist[i][1], out version);
            
            LocalABVersion tempData = new LocalABVersion(abname, version);

            LocalABVersionDictionary.Add(abname, tempData);        }            }    /// <summary>
    /// 获取本地ab的版本数据
    /// </summary>
    /// <param name="abname"></param>
    /// <returns></returns>    public LocalABVersion GetLocalABVersion(string abname)    {        if (LocalABVersionDictionary.ContainsKey(abname))            return LocalABVersionDictionary[abname];        return null;    }    /// <summary>
    /// 下载新的ab后修改版本值，没有则add
    /// </summary>
    /// <param name="abname"></param>
    /// <param name="ver"></param>
    /// <returns></returns>    public bool SetLocalABVersion(string abname,uint ver)
    {
        if (LocalABVersionDictionary.ContainsKey(abname))
        {
            LocalABVersionDictionary[abname].version = ver;
        }
        else
        {
            LocalABVersion tempData = new LocalABVersion(abname, ver);
            LocalABVersionDictionary.Add(abname, tempData);
        }
        WriteLocalABVersionDataToFile();
        return true;
    }    /// <summary>
    /// 把ab版本数据写入到文件
    /// </summary>    public void WriteLocalABVersionDataToFile()
    {
        if (LocalABVersionDictionary.Count == 0)
            return;
        string datatext = "";
        foreach(var data in LocalABVersionDictionary)
        {
            datatext += data.Value.abname +"," +data.Value.version.ToString() + "\r\n"; 
        }
        string path = GameDefine.AssetBundleSavePath + GameDefine.LocalAssetbundleVersionDataFile;
        StreamWriter sw = null;
        //检测文件是否存在
        if (!File.Exists(path))        {
            sw = File.CreateText(path);        }
        else
        {
            sw = new StreamWriter(path);
        }
        sw.Write(datatext);
        //File.WriteAllText(path, datatext);
        sw.Close();
        sw.Dispose();
    }}