﻿using System.Collections.Generic;

/// <summary>
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
/// </summary>
        string path = GameDefine.AssetBundleSavePath + GameDefine.LocalAssetbundleVersionDataFile;

        //检测文件是否存在
        if (!File.Exists(path))

        List<string[]> strlist;
        CReadCsvBase.ReaderCsvDataFromFile(path, out strlist);
            
            LocalABVersion tempData = new LocalABVersion(abname, version);

            LocalABVersionDictionary.Add(abname, tempData);
    /// 获取本地ab的版本数据
    /// </summary>
    /// <param name="abname"></param>
    /// <returns></returns>
    /// 下载新的ab后修改版本值，没有则add
    /// </summary>
    /// <param name="abname"></param>
    /// <param name="ver"></param>
    /// <returns></returns>
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
    }
    /// 把ab版本数据写入到文件
    /// </summary>
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
        if (!File.Exists(path))
            sw = File.CreateText(path);
        else
        {
            sw = new StreamWriter(path);
        }
        sw.Write(datatext);
        //File.WriteAllText(path, datatext);
        sw.Close();
        sw.Dispose();
    }