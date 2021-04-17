using System.IO;using System.Net;using UnityEngine;

/// <summary>/// 资源版本管理更新系统/// </summary>public class CResVersionCompareUpdate{

    /// <summary>
    /// 检测apk是否需要更新
    /// </summary>
    /// <param name="appVerStr"></param>
    /// <returns></returns>
    public static bool CheckApkVerAndUpdate(string appVerStr)
    {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN        if (!Luancher.UpdateWithLuncher)
        {
            return false;
        }
#endif
        bool apkupdate = false;
        if (appVerStr.CompareTo(Application.version) != 0)
        {
            apkupdate = true;
            if (!Directory.Exists(GameDefine.DownloadApkSavePath))
                Directory.CreateDirectory(GameDefine.DownloadApkSavePath);
            HttpDownloadMgr.Instance.RequestDownload(GameDefine.LuancherURL, GameDefine.DownloadApkSavePath, GameDefine.ApkFileName, 1, 255);
        }
        if (apkupdate == false)
        {
            //把上次更新的apk删除
            string apkfile = GameDefine.DownloadApkSavePath + GameDefine.ApkFileName;
            if (File.Exists(apkfile))
                File.Delete(apkfile);
        }

        return apkupdate;
    }

    public static bool CompareABVersionAndUpdate(string abname,bool bAsyncDown = false,byte gameid = 255)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN        if (!Luancher.UpdateWithLuncher)
        {
            return false;
        }
#endif
        LocalABVersion lcABVerdata = CCsvDataManager.Instance.LocalABVerDataMgr.GetLocalABVersion(abname);
        CServerABVerData sevABVerdata = CCsvDataManager.Instance.SerABVerDataMgr.GetServerABVerData(abname);
        bool needDownloadPpdate = false;
        if (sevABVerdata == null)
        {
            Debug.Log("服务器资源表上无此资源，请检查：" + abname);
            return false;
        }
        if (lcABVerdata == null)
        {
            needDownloadPpdate = true;
        }
        else
        {
            if (lcABVerdata.version == sevABVerdata.AssetbundleVersion)
                needDownloadPpdate = false;
            else
            {
                needDownloadPpdate = true;
                //本地存在可能是旧的版本文件或者是上次未下载完成的的损坏文件先删除
                if (File.Exists(GameDefine.AssetBundleSavePath + abname))
                    File.Delete(GameDefine.AssetBundleSavePath + abname);
            }
        }

        //如果本地没有此资源则更新
        if (!File.Exists(GameDefine.AssetBundleSavePath + abname))
            needDownloadPpdate = true;

        if (needDownloadPpdate)
        {
            if(bAsyncDown)
                HttpDownloadMgr.Instance.RequestDownload(GameDefine.LuancherURL, GameDefine.AssetBundleSavePath, abname, sevABVerdata.AssetbundleVersion, gameid);
            else
            {
                HttpDownloadMgr.DownFile(GameDefine.LuancherURL, GameDefine.AssetBundleSavePath, abname);
                CCsvDataManager.Instance.LocalABVerDataMgr.SetLocalABVersion(abname, sevABVerdata.AssetbundleVersion);
            }

        }
        return needDownloadPpdate;
    }


    /// <summary>
    /// 检查游戏资源是否需要下载或更新
    /// </summary>
    /// <param name="gameId"></param>
    /// <returns>0正常 ,1需下载, 2有更新</returns>
    public static byte CheckGameResNeedDownORUpdate(byte gameId)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN        if (!Luancher.UpdateWithLuncher)
        {
            return 0;
        }
#endif
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(gameId);
        if(gamedata == null)
        {
            return 0;
        }
        byte checkstate = 0;
        //如果本地没有场景assetbundle
        if (!File.Exists(GameDefine.AssetBundleSavePath + gamedata.SceneABName))
        {
            checkstate = 1;
            return checkstate;
        }
        //如果本地没有资源assetbundle
        if (!File.Exists(GameDefine.AssetBundleSavePath + gamedata.ResourceABName))
        {
            checkstate = 1;
            return checkstate;
        }
        
        LocalABVersion lcABVerdata = CCsvDataManager.Instance.LocalABVerDataMgr.GetLocalABVersion(gamedata.SceneABName);
        //本地资源管理表中没有数据说明上次下载出错了，那么重新下
        if (lcABVerdata == null)
        {
            checkstate = 1;
            return checkstate;
        }

        CServerABVerData sevABVerdata = CCsvDataManager.Instance.SerABVerDataMgr.GetServerABVerData(gamedata.SceneABName);
        if (sevABVerdata == null)
        {
            Debug.Log("服务器资源表上无此资源，请检查：" + gamedata.SceneABName);
            checkstate = 1;
            return checkstate;
        }

        //版本不一致需要更新状态
        if (lcABVerdata.version != sevABVerdata.AssetbundleVersion)
            checkstate = 2;

        //场景版本没有更新，检测资源
        if (checkstate == 0)
        {
            LocalABVersion lcResABVerdata = CCsvDataManager.Instance.LocalABVerDataMgr.GetLocalABVersion(gamedata.ResourceABName);
            //本地资源管理表中没有数据说明上次下载出错了，那么重新下
            if (lcResABVerdata == null)
            {
                checkstate = 1;
                return checkstate;
            }

            CServerABVerData SerResABVerdata = CCsvDataManager.Instance.SerABVerDataMgr.GetServerABVerData(gamedata.ResourceABName);
            //服务器资源管理表中没有数据
            if (SerResABVerdata == null)
            {
                checkstate = 2;
                return checkstate;
            }

            //版本不一致需要更新状态
            if (lcResABVerdata.version != SerResABVerdata.AssetbundleVersion)
                checkstate = 2;
        }
        
       
        return checkstate;
    }}