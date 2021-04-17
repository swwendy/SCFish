using System.Collections;using System.Collections.Generic;using UnityEngine;using System;public struct DownloadEndGameAssetbundleInfo
{
    public string bundlename;

    public byte BelongGameId;

    public DownloadEndGameAssetbundleInfo(byte gameid, string name)
    {
        bundlename = name;
        BelongGameId = gameid;
    }
}public class CGameResDownloadMgr{
    

    public Dictionary<byte, List<string>> GameResDownloadDic;    public static readonly CGameResDownloadMgr Instance = new CGameResDownloadMgr();    public List<byte> DownloadOverGameResIdList;

    public List<DownloadEndGameAssetbundleInfo> DownloadOverbundleInfoList;

    private CGameResDownloadMgr()    {
        DownloadOverGameResIdList = new List<byte>();
        GameResDownloadDic = new Dictionary<byte, List<string>>();
        DownloadOverbundleInfoList = new List<DownloadEndGameAssetbundleInfo>();
    }    /// <summary>
    /// 下成完成一个资源回调处理
    /// </summary>
    /// <param name="resbelonggameid"></param>
    /// <param name="resname"></param>	public void DownloadOverGameResCallBack(byte resbelonggameid,string resname)
    {
        Debug.Log("游戏资源下载完成callback，GameID:" + resbelonggameid);
        if (GameResDownloadDic.ContainsKey(resbelonggameid))
        {
            foreach(string str in GameResDownloadDic[resbelonggameid])
            {
                if (str.CompareTo(resname) == 0)
                {
                    DownloadEndGameAssetbundleInfo info = new DownloadEndGameAssetbundleInfo(resbelonggameid, resname);
                    DownloadOverbundleInfoList.Add(info);
                    GameResDownloadDic[resbelonggameid].Remove(str);
                    break;
                }
            }
            //该游戏所有资源都下载完成
            if(GameResDownloadDic[resbelonggameid].Count ==0)
            {
                //GameMain.hall_.GameResDownloadOver(resbelonggameid);
                GameResDownloadDic.Remove(resbelonggameid);
                DownloadOverGameResIdList.Add(resbelonggameid);
                Debug.Log("下载队列资源下载完成，GameID:"+resbelonggameid);
            }
            
        }
    }    /// <summary>
    /// 所有游戏资源是否下载完成
    /// </summary>
    /// <returns></returns>    public bool isAllGameResHaveDownOver()
    {
        if (GameResDownloadDic.Count !=0)
            return false;
        return true;
    }

    /// <summary>
    /// 该游戏资源是否下载完成
    /// </summary>
    /// <returns></returns>    public bool isGameResHaveDownOver(byte gameid)
    {
        foreach (byte id in DownloadOverGameResIdList)
        {
            if (id == gameid)
                return true;
        }         
        return false;
    }


    /// <summary>
    /// 该游戏资源是否正在下载
    /// </summary>
    /// <returns></returns>    public bool isGameResDownloading(byte gameid)
    {
        if (GameResDownloadDic.ContainsKey(gameid))
            return true;
        return false;
    }

    /// <summary>
    /// 添加游戏所需下载的资源
    /// </summary>
    /// <param name="gameid"></param>
    /// <param name="resname"></param>    public void AddGameResDownloadDic(byte gameid,string resname)
    {
        if (!GameResDownloadDic.ContainsKey(gameid))
            GameResDownloadDic.Add(gameid,new List<string>());
        GameResDownloadDic[gameid].Add(resname);
        Debug.Log("游戏资源添加下载，GameID:" + gameid);
    }    /// <summary>
    /// 获取需下载资源 的游戏id
    /// </summary>
    /// <returns></returns>    public  List<byte>  GetDownloadResGameIdList()
    {
        List<byte> gamelist = new List<byte>();
        foreach(var game in GameResDownloadDic)
        {
            gamelist.Add(game.Key);
        }
        return gamelist;
    }    /// <summary>
    /// 获取资源已经下载完成的游戏id列表
    /// </summary>
    /// <returns></returns>    public  void GetDownloadResOverGameIdList(ref List<byte> gamelist)
    {
        gamelist.Clear();
        gamelist.AddRange(DownloadOverGameResIdList);
    }

    /// <summary>
    /// 获取游戏剩余资源的个数
    /// </summary>
    /// <returns></returns>    public int GetDownloadGameResCount(byte gameid)
    {
        if (GameResDownloadDic.ContainsKey(gameid))
        {
            return GameResDownloadDic[gameid].Count;
        }
        return 0;
    }}