using UnityEngine;using System.IO;using System;using UnityEngine.Networking;using UnityEngine.UI;using System.Collections.Generic;using System.Collections;using ICSharpCode.SharpZipLib.Zip;

public class Luancher : MonoBehaviour{    private enum LUANCHERSTATE    {        LuancherState_None = 0,         //空状态        LuancherState_StartVerify,      //启动资源校验
        LuancherState_UnZip,            //解压缩资源包        LuancherState_VerifyInProcess,  //资源校验过程中        LuancherState_VerifyOver,       //资源校验过程结束        LuancherState_Downloading,      //资源下载更新中        LuancherState_InstallAPK,       //安卓更新安装APK        LuancherState_VerifyMD5,        //资源校验Md5        LuancherState_StartGame,        //校验通过启动游戏        LuancherState_Over,             //luancher流程结束    }    // 服务器资源版本号    private string m_SvrResVerStr;    private string m_SvrAppVerStr;    private string m_ServerIP;    private int m_ServerPort;    //luancher状态    private LUANCHERSTATE eLuancherState;    //安卓平台下APK是否需要更新    bool bApkNeedUpdate = false;    //程序是否第一次运行    bool bAppFirstRun;    //下载进度条对象    private Image DownloadProcessBarImg;    //下载进度百分比    private GameObject DownloadProcessPercentObj;    //下载进度总大小    private GameObject DownloadProcessTotalLengthObj;    //下载更新描述文本    private GameObject DownloadProcessUpdatetextObj;    //是否审核版本    public static bool IsReviewVersion = false;

    // 编辑器模式下是否需要走更新流程
#if UNITY_EDITOR || UNITY_STANDALONE_WIN    public static readonly bool UpdateWithLuncher = false;
#endif    //资源Md5校验开关
    public static readonly bool EnableResMD5CRC = false;

    // 是否打印日志在屏幕上
    public static readonly bool IsPrintLogOnScreen = false;    // 是否微信登录    public static  bool IsVChatLogin = false;    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]    static void Initialize()    {    }    // Use this for initialization    void Start()    {        InitApplicationSetting();        m_SvrResVerStr = string.Empty;        m_ServerIP = string.Empty;        m_ServerPort = 0;        bApkNeedUpdate = false;        LogFile.InitLog();        //this.transform.gameObject.AddComponent<LogFile>();        Transform Imagestriptf = GameObject.Find("Canvas/Root").transform.Find("Main_Loading").Find("ImageStripBG");        DownloadProcessBarImg = Imagestriptf.Find("ImageStrip").gameObject.GetComponent<Image>();        DownloadProcessPercentObj = Imagestriptf.Find("Text_percent").gameObject;        DownloadProcessTotalLengthObj = Imagestriptf.Find("Text_length").gameObject;        DownloadProcessUpdatetextObj = Imagestriptf.Find("Text_Info").gameObject;        DownloadProcessPercentObj.SetActive(false);        DownloadProcessTotalLengthObj.SetActive(false);        DownloadProcessUpdatetextObj.GetComponent<Text>().text = "校验游戏资源，请稍候...";        DownloadProcessUpdatetextObj.SetActive(true);        Imagestriptf.Find("Text_version").GetComponent<Text>().text = Application.version;        eLuancherState = LUANCHERSTATE.LuancherState_StartVerify;
        RefreshScreenResolution();    }    // Update is called once per frame    void Update()    {        //防止在start中初始化下载资源登陆场景还没有显示        if (eLuancherState == LUANCHERSTATE.LuancherState_StartVerify && Time.time > 0.5f)        {            Debug.Log("LuancherState_StartVerify process:");            if (CheckAppRunPermission() == true)            {                CheckVersionUpdate();                //eLuancherState = LUANCHERSTATE.LuancherState_Over;            }            else            {                DownloadProcessUpdatetextObj.GetComponent<Text>().text = "请授予程序运行必要的权限...";                DownloadProcessUpdatetextObj.SetActive(true);                eLuancherState = LUANCHERSTATE.LuancherState_Over;            }        }        //else if(eLuancherState == LUANCHERSTATE.LuancherState_UnZip)
        //{                    //    if (DownloadProcessBarImg.fillAmount == 0.01f)        //    {        //        FirstRunAppUnzipAssetbundle();        //    }        //    DownloadProcessBarImg.fillAmount += 0.005f;        //    Debug.Log("unzip process:" + DownloadProcessBarImg.fillAmount);        //}        //资源更新下载状态        else if (eLuancherState == LUANCHERSTATE.LuancherState_Downloading)        {            if (DownLoadProcessMgr.Instance.IsDownLoadOver)            {                 DownloadProcessUpdatetextObj.GetComponent<Text>().text = "正在校验资源...";                 DownloadProcessUpdatetextObj.SetActive(true);                 eLuancherState = LUANCHERSTATE.LuancherState_VerifyMD5;            }            else            {                if (HttpDownloadMgr.Instance.bDownloadError)                {                    DownloadProcessUpdatetextObj.GetComponent<Text>().text = "网络出现异常，正在重试...";                    DownloadProcessUpdatetextObj.SetActive(true);                }                else                {                    DownloadProcessBarImg.fillAmount = DownLoadProcessMgr.Instance.GetDownloadProcess();                    uint percent = DownLoadProcessMgr.Instance.GetDownloadPercent();                    //Debug.Log("percent:" + percent);                    DownloadProcessPercentObj.GetComponent<Text>().text = percent.ToString() + "%";                    //这里有优化空间的，不需要每帧去更新总大小                    string downlenstr = DownLoadProcessMgr.Instance.GetDownloadFileCurLength().ToString();                    DownloadProcessTotalLengthObj.GetComponent<Text>().text = downlenstr + "/" + DownLoadProcessMgr.Instance.GetDownloadFileTotalLength().ToString() + "KB";                    DownloadProcessPercentObj.SetActive(true);                    DownloadProcessTotalLengthObj.SetActive(true);                    DownloadProcessUpdatetextObj.GetComponent<Text>().text = "更新资源，请稍候...";                    DownloadProcessUpdatetextObj.SetActive(true);                }            }        }        //下载完成调起安装        else if (eLuancherState == LUANCHERSTATE.LuancherState_InstallAPK)        {#if UNITY_ANDROID#if UKGAME_SDK            CUKGameSDK.GetUKGameActivity().Call("InstallApk", GameDefine.DownloadApkSavePath + GameDefine.ApkFileName);#else             AlipayWeChatPay.GetAndroidActivity().Call("InstallApk",GameDefine.DownloadApkSavePath+GameDefine.ApkFileName);#endif#endif            eLuancherState = LUANCHERSTATE.LuancherState_Over;        }        //资源资源进行MD5校验(防止网络异常下载的资源文件内容有错)        else if( eLuancherState == LUANCHERSTATE.LuancherState_VerifyMD5 )        {
           if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
               eLuancherState = LUANCHERSTATE.LuancherState_StartGame;
          else              ResourceMD5Verify();        }        //启动游戏        else if (eLuancherState == LUANCHERSTATE.LuancherState_StartGame)        {            GameMain.Instance.InitApplication(m_SvrResVerStr, m_ServerIP, m_ServerPort);            eLuancherState = LUANCHERSTATE.LuancherState_Over;        }    }    /// <summary>    /// 初始程序设置    /// </summary>    private void InitApplicationSetting()    {        Screen.sleepTimeout = SleepTimeout.NeverSleep;        Application.targetFrameRate = 30;        Application.runInBackground = true;        Input.multiTouchEnabled = false;    }    /// <summary>    /// 检测安卓运行权限    /// </summary>    private bool CheckAppRunPermission()    {        bool bpermiss = true;#if UNITY_ANDROID#if UKGAME_SDK        if (Application.platform == RuntimePlatform.Android)            bpermiss = CUKGameSDK.GetUKGameActivity().Call<bool>("CheckAppRunPermission");#endif#endif        return bpermiss;    }    /// <summary>    /// 检查版本号，与服务器资源进行版本号比对    /// </summary>    public void CheckVersionUpdate()    {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN         if (!UpdateWithLuncher)        {            eLuancherState = LUANCHERSTATE.LuancherState_VerifyMD5;            return;        }#endif        //读取服务器资源及程序版本号        LoadServerResVersionConfig();        if (string.IsNullOrEmpty(m_SvrResVerStr))        {            DownloadProcessUpdatetextObj.GetComponent<Text>().text = "连接异常，请检查网络...";            DownloadProcessUpdatetextObj.SetActive(true);            return;        }        //IOS 检测下程序是否需要更新，需要 则提示跳转app store        if (Application.platform == RuntimePlatform.IPhonePlayer)        {            if (m_SvrAppVerStr.CompareTo(Application.version) != 0)            {                DownloadProcessUpdatetextObj.GetComponent<Text>().text = "请至AppStore更新APP...";                DownloadProcessUpdatetextObj.SetActive(true);                eLuancherState = LUANCHERSTATE.LuancherState_Over;

#if UNITY_IOS                WechatPlatfrom_IOS.ShowAppUpdateTips("itms-apps://itunes.apple.com/cn/app/qiu-qiu-dou-di-zhu/id1319175293?mt=8");
#endif                return;            }        }        //bool bNeedUpdate = false;        //读取本地资源版本        string localResVerStr = ReadLocalResConfig();        //首次运行解压包里的资源,更新资源        if (bAppFirstRun)        {            eLuancherState = LUANCHERSTATE.LuancherState_UnZip;            DownloadProcessUpdatetextObj.GetComponent<Text>().text = "正在解压游戏资源(此过程不消耗流量)";
            Debug.Log("unzip processs start-------:" );            bAppFirstRun = false;            FirstRunAppUnzipAssetbundle();            //bNeedUpdate = true;               }        else        {            if (m_SvrResVerStr.CompareTo(localResVerStr) != 0)            {                //bNeedUpdate = true;
                WriteLocalResConfig();
                DownHotfixAndSerABCsvFile();            }            ResourceUpdateVerify();        }           }    /// <summary>    /// 读取服务器资源和程序版本号    /// </summary>    /// <param name="path"></param>    /// <returns></returns>    private void LoadServerResVersionConfig()    {        UnityWebRequest www = UnityWebRequest.Get(GameDefine.LuancherURL + GameDefine.ServerResVersionFile);        www.Send();        while (!www.isDone) { }        // Show results as text        string verstr = www.downloadHandler.text;        www.Dispose();        INIParser ini = new INIParser();        ini.OpenFromString(verstr);        string IosReviewAppVer = ini.ReadValue("SvrResVer", "ReviewAppVer", "");        string luancherurl = ini.ReadValue("SvrResVer", "FormalLuancher", "");                        //当前是否审核版本        if(!string.IsNullOrEmpty(IosReviewAppVer) && IosReviewAppVer.CompareTo(Application.version) == 0 )        {            IsReviewVersion = true;        }        //如果不是审核版本且有指向则需连接正式luancher服上获取更新字段        if ( !IsReviewVersion && !string.IsNullOrEmpty(luancherurl))        {            UnityWebRequest www1 = UnityWebRequest.Get(luancherurl + "SerResVersion.data");            www1.Send();            while (!www1.isDone) { }            verstr = www1.downloadHandler.text;            www1.Dispose();            ini.OpenFromString(verstr);            GameDefine.LuancherURL = luancherurl;        }        m_SvrResVerStr = ini.ReadValue("SvrResVer", "ResVer", "");        m_SvrAppVerStr = ini.ReadValue("SvrResVer", "AppVer", "");        m_ServerIP = ini.ReadValue("SvrResVer", "SvrIP", "");        m_ServerPort = ini.ReadValue("SvrResVer", "SvrPort", 16201);        ini.Close();        Debug.Log("Res:" + m_SvrResVerStr + ",ApkVer:" + m_SvrAppVerStr + " SerIP:" + m_ServerIP);    }    /// <summary>    /// 读取本地资源版本号    /// </summary>    /// <param name="name"></param>    /// <returns></returns>    string ReadLocalResConfig()    {        string localResVerPath = GameDefine.AppPersistentDataPath + GameDefine.LocalResVersionFile;        INIParser IniFile = new INIParser();        IniFile.Open(localResVerPath);        string localResVerstr= IniFile.ReadValue("AppConfig", "LocalResVer", string.Empty);        bAppFirstRun = IniFile.ReadValue("AppConfig", "AppFirstRun", true);        IniFile.Close();        //检测文件是否存在        /*if (!File.Exists(localResVerPath))        {            Debug.Log("ReadLocalResourceVersion no file ");            return null;        }        //使用流的形式读取        StreamReader sr = null;        try        {            sr = File.OpenText(localResVerPath);        }        catch (Exception)        {            //路径与名称未找到文件则直接返回            return null;        }        string localResVerstr = sr.ReadLine();        Debug.Log("ReadLocalResourceVersion:" + localResVerstr);        sr.Close();        sr.Dispose();*/        return localResVerstr;    }    /// <summary>    /// 写入当前服务器资源版本号到本地    /// </summary>    /// <returns></returns>    bool WriteLocalResConfig()    {        string localResVerPath = GameDefine.AppPersistentDataPath + GameDefine.LocalResVersionFile;        INIParser IniFile = new INIParser();        IniFile.Open(localResVerPath);        IniFile.WriteValue("AppConfig", "LocalResVer", m_SvrResVerStr);        if(!bAppFirstRun)           IniFile.WriteValue("AppConfig", "AppFirstRun", false);        IniFile.Close();        return true;    }    /// <summary>    /// 下载更新的assetbundle    /// </summary>    private void DownHotfixAndSerABCsvFile()    {        if (!Directory.Exists(GameDefine.AssetBundleSavePath))            Directory.CreateDirectory(GameDefine.AssetBundleSavePath);        //有更新的话，热更新脚本文件与服务器资源版本文件是必需更新的        HttpDownloadMgr.DownFile(GameDefine.LuancherURL, GameDefine.AppPersistentDataPath, GameDefine.HotFixLuaFile);        HttpDownloadMgr.DownFile(GameDefine.LuancherURL, GameDefine.AppPersistentDataPath, GameDefine.ServerAssetbundleVersionDataFile);        //HttpDownload.DownFile(GameDefine.LuancherURL, GameDefine.AssetBundleSavePath, "hall");        //         AssetBundleManager.LoadAssetBundleFromLocal(GameDefine.AssetBundleSavePath, "resverab");        //         AssetBundle bundle = AssetBundleManager.GetAssetBundle("resverab");        //         //         TextAsset datatxt = bundle.LoadAsset("AssetbundleConfig.txt", typeof(TextAsset)) as TextAsset;        //         Debug.Log(datatxt.text);    }    /// <summary>    /// 启动游戏所需要资源包校验，有需要则下载更新    /// </summary>    private void ResourceUpdateVerify()    {        CCsvDataManager.Instance.LocalABVerDataMgr.LoadLocalABVerDataFile();        CCsvDataManager.Instance.SerABVerDataMgr.LoadServerABVerDataFile();        //校验csv和大厅资源 assetbundle        bool ResourceNeedUpdate = CResVersionCompareUpdate.CompareABVersionAndUpdate(GameDefine.HallAssetbundleName, true);        ResourceNeedUpdate |= CResVersionCompareUpdate.CompareABVersionAndUpdate(GameDefine.DependenciesAssetBundleName, true);        ResourceNeedUpdate |= CResVersionCompareUpdate.CompareABVersionAndUpdate(GameDefine.HallConstAssetBundleName, true);        ResourceNeedUpdate |= CResVersionCompareUpdate.CompareABVersionAndUpdate(GameDefine.CsvAssetbundleName, true);        ResourceNeedUpdate |= CResVersionCompareUpdate.CompareABVersionAndUpdate(GameDefine.PokerAssetBundleName, true);        ResourceNeedUpdate |= CResVersionCompareUpdate.CompareABVersionAndUpdate(GameDefine.HallBagIconAssetBundleName, true);
        ResourceNeedUpdate |= CResVersionCompareUpdate.CompareABVersionAndUpdate(GameDefine.HallAnimeAssetBundleName, true);

        //android平台下检测apk是否需要更新
        if (Application.platform == RuntimePlatform.Android)        {            bApkNeedUpdate = CResVersionCompareUpdate.CheckApkVerAndUpdate(m_SvrAppVerStr);        }        if (ResourceNeedUpdate || bApkNeedUpdate)            eLuancherState = LUANCHERSTATE.LuancherState_Downloading;        else        {            eLuancherState = LUANCHERSTATE.LuancherState_VerifyMD5;        }           }    /// <summary>    /// 资源MD5检验    /// </summary>    private void ResourceMD5Verify()    {        //string hallAssetMd5str = GameCommon.GenerateFileMd5(GameDefine.AssetBundleSavePath + GameDefine.HallAssetbundleName);        List<string> filelist = DownLoadProcessMgr.Instance.DownloadOverFileNameList;        bool md5crcSuccessed = true;        if (EnableResMD5CRC)
        {
            for (int i = 0; i < filelist.Count; i++)
            {

                CServerABVerData filesvrdata = CCsvDataManager.Instance.SerABVerDataMgr.GetServerABVerData(filelist[i]);

                if (filesvrdata == null)
                    continue;

                string filemd5 = GameCommon.GenerateFileMd5(GameDefine.AssetBundleSavePath + filelist[i]);
                if (filemd5.CompareTo(filesvrdata.AssetbundleMd5Str) != 0)
                {
                    File.Delete(GameDefine.AssetBundleSavePath + filelist[i]);
                    CResVersionCompareUpdate.CompareABVersionAndUpdate(filelist[i], true);
                    Debug.Log(filelist[i] + " md5 CRC fialed");
                    md5crcSuccessed = false;
                }
            }        }        DownLoadProcessMgr.Instance.DownloadOverFileNameList.Clear();        if (md5crcSuccessed)        {            //apk更新下载完成执行安装            if (bApkNeedUpdate)                eLuancherState = LUANCHERSTATE.LuancherState_InstallAPK;            else              eLuancherState = LUANCHERSTATE.LuancherState_StartGame;        }        else            eLuancherState = LUANCHERSTATE.LuancherState_Downloading;    }    /// <summary>    /// 首次运行APP,资源解压拷贝处理    /// </summary>    private void FirstRunAppUnzipAssetbundle()    {        if (!Directory.Exists(GameDefine.AssetBundleSavePath))            Directory.CreateDirectory(GameDefine.AssetBundleSavePath);        //if(Application.platform == RuntimePlatform.Android)        //{        //    if (!Directory.Exists(GameCommon.GetAppStreamingAssetPath()))        //        return;        //}        //DirectoryInfo fileDir = new DirectoryInfo(GameCommon.GetAppStreamingAssetPath());        //FileInfo[] files = fileDir.GetFiles();        //if (files != null)        //{        //    for (int i = 0; i < files.Length; i++)        //    {        //        //ab版本文件放到AppPersistentDataPath        //        if (files[i].Name.CompareTo(GameDefine.LocalAssetbundleVersionDataFile) == 0)        //            CopyFileToDesPath(files[i].FullName, files[i].Name, GameDefine.AppPersistentDataPath);        //        else        //            CopyFileToDesPath(files[i].FullName, files[i].Name, GameDefine.AssetBundleSavePath);        //    }        //}        //UnzipAssetFile();        StartCoroutine(UnzipAssetFile());    }    IEnumerator UnzipAssetFile()    {        string src = GameCommon.GetAppStreamingAssetPath() + GameDefine.AssetsZipFile;        string des = GameDefine.AssetBundleSavePath + GameDefine.AssetsZipFile;        Debug.Log("load zip:" + src + "->" + des);

        float percent = 0.5f;

        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(src);

            while (!www.isDone)
            {
                DownloadProcessBarImg.fillAmount = www.progress * percent;

                yield return null;
            }

            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log("www.error:" + www.error);
            }
            else
            {
                try
                {
                    if (File.Exists(des))
                    {
                        File.Delete(des);
                    }
                    FileStream fsDes = File.Create(des);
                    fsDes.Write(www.bytes, 0, www.bytes.Length);
                    fsDes.Flush();
                    fsDes.Close();
                    fsDes.Dispose();
                }
                catch (Exception ex)
                {
                    DownloadProcessUpdatetextObj.GetComponent<Text>().text = ex.ToString();
                    eLuancherState = LUANCHERSTATE.LuancherState_Over;
                }
            }
            www.Dispose();
        }
        else if (File.Exists(src))
        {
            yield return CopyFileToDesPath(src, GameDefine.AssetsZipFile, GameDefine.AssetBundleSavePath, percent);
        }

        percent = 0.4f;
        yield return UnZip(des, GameDefine.AssetBundleSavePath, null, percent, true);

        DownloadProcessBarImg.fillAmount = 0.9f;        yield return null;        WriteLocalResConfig();

        DownloadProcessBarImg.fillAmount = 0.95f;

        yield return null;

        DownHotfixAndSerABCsvFile();

        DownloadProcessBarImg.fillAmount = 1f;

        yield return null;

        ResourceUpdateVerify();
    }

    IEnumerator UnZip(string fileToUnZip, string zipedFolder, string password, float percent, bool delZipOnEnd)
    {
        FileStream fs = null;
        ZipInputStream zipStream = null;
        ZipEntry ent = null;
        string fileName, directoryName;

        if (!File.Exists(fileToUnZip))
        {            Debug.Log("zip file not exist!!!!!");
            yield break;
        }
        if (!Directory.Exists(zipedFolder))
            Directory.CreateDirectory(zipedFolder);

        yield return null;

        float process = 0;
        FileStream srcfs = File.OpenRead(fileToUnZip);        int filelen = (int)srcfs.Length;        zipStream = new ZipInputStream(srcfs);
        if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
        while ((ent = zipStream.GetNextEntry()) != null)
        {
            if (!string.IsNullOrEmpty(ent.Name))
            {
                directoryName = Path.GetDirectoryName(ent.Name);
                if (directoryName != String.Empty)
                    Directory.CreateDirectory(zipedFolder + directoryName);

                fileName = Path.Combine(zipedFolder, ent.Name);

                if (!string.IsNullOrEmpty(fileName))
                {
                    fs = File.Create(fileName);
                    int size = 2048;
                    byte[] data = new byte[size];
                    while (true)
                    {
                        size = zipStream.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            try
                            {
                                fs.Write(data, 0, size);                            }                            catch (Exception ex)                            {
                                DownloadProcessUpdatetextObj.GetComponent<Text>().text = "存储空间不足，请释放足够空间后重新运行";
                                eLuancherState = LUANCHERSTATE.LuancherState_Over;
                                Debug.Log("unzip asset zip file failed!!!!!");
                                yield break;
                            }                        }
                        else
                            break;
                    }
                }
            }

            
            process = (float)ent.CompressedSize / filelen;
            DownloadProcessBarImg.fillAmount += process * percent;
            DebugLog.Log("unzip process:" + DownloadProcessBarImg.fillAmount + " name:" + ent.Name);

            yield return null;
        }

        zipStream.Close();

        if (fs != null)
        {
            fs.Close();
            fs.Dispose();
        }
        if (zipStream != null)
        {
            zipStream.Close();
            zipStream.Dispose();
        }
        if (ent != null)
        {
            ent = null;
        }
        GC.Collect();
        GC.Collect(1);

        if (delZipOnEnd)
            File.Delete(fileToUnZip);
    }

    /*void UnzipAssetFile()    {        string src = GameCommon.GetAppStreamingAssetPath() + GameDefine.AssetsZipFile;        string des = GameDefine.AssetBundleSavePath + GameDefine.AssetsZipFile;        if (Application.platform == RuntimePlatform.Android)        {            Debug.Log("load zip:" + src);            WWW www = new WWW(src);            while (!www.isDone) { }            if (!string.IsNullOrEmpty(www.error))            {                Debug.Log("www.error:" + www.error);            }            else            {                try                {                    if (File.Exists(des))                    {                        File.Delete(des);                    }                    FileStream fsDes = File.Create(des);                    fsDes.Write(www.bytes, 0, www.bytes.Length);                    fsDes.Flush();                    fsDes.Close();                    fsDes.Dispose();                }                catch(Exception ex)                {                    DownloadProcessUpdatetextObj.GetComponent<Text>().text = "存储空间不足，请释放足够空间后重新运行";                    eLuancherState = LUANCHERSTATE.LuancherState_Over;                }            }            www.Dispose();        }        else        {            CopyFileToDesPath(src, GameDefine.AssetsZipFile, GameDefine.AssetBundleSavePath);        }        bool result = ZipCompress.UnZip(des, GameDefine.AssetBundleSavePath);        if(!result)        {            DownloadProcessUpdatetextObj.GetComponent<Text>().text = "存储空间不足，请释放足够空间后重新运行";            eLuancherState = LUANCHERSTATE.LuancherState_Over;            Debug.Log("unzip asset zip file failed!!!!!");        }        File.Delete(des);    }*/

    /// <summary>    /// 从源path拷贝文件到目标path    /// </summary>    /// <param name="srcpath"></param>    /// <param name="name">文件名</param>    /// <param name="despath"></param>    private IEnumerator CopyFileToDesPath(string srcpath, string fileName, string despath, float percent = 1f)    {        if (!File.Exists(srcpath))
            yield break;
        string des = despath + fileName;        int readmaxlength = 5 * 1024 * 1024;                FileStream srcfs = File.OpenRead(srcpath);        int filelen = (int)srcfs.Length;        int srcLen = filelen;        float process = 0;        int readlen = 0;        FileStream fsDes = File.Create(des);        while (filelen > 0)        {            if(filelen > readmaxlength)            {                readlen = readmaxlength;            }            else            {                readlen = filelen;            }            byte[] array = new byte[readlen];            //将文件读到byte数组中            srcfs.Read(array, 0, readlen);            fsDes.Write(array, 0, readlen);            fsDes.Flush();            filelen -= readlen;            process = (float)readlen / srcLen;
            DownloadProcessBarImg.fillAmount += process * percent;
            DebugLog.Log("copy process:" + DownloadProcessBarImg.fillAmount + " len:"+readlen);

            yield return null;        }        srcfs.Close();        fsDes.Close();    }    /*string perstr;    public void UKGamePermissionResp(string retstr)    {        perstr = perstr + "-" + retstr;        Debug.Log("UKGamePermissionResp Resp:" + retstr);        DownloadProcessUpdatetextObj.GetComponent<Text>().text = perstr;        DownloadProcessUpdatetextObj.SetActive(true);    }*/


    void RefreshScreenResolution()
    {
#if WINDOWS_GUEST
        Resolution[] resolutions = Screen.resolutions;
        foreach (var value in resolutions)
        {
            Debug.Log("显示器支持的分辨率:" + value.ToString());
        }
        string[] CommandLine = System.Environment.CommandLine.Split('-');        if (CommandLine.Length >= 4)
        {
            int separatorIndex = 0;
            int[] resolutionsValue = { -1, -1 };
            string [] resolutionsNameValue = {"Width =", "Height =" };
            for (int index = 0; index < 2; ++index)
            {
                separatorIndex = CommandLine[index + 2].IndexOf(resolutionsNameValue[index]) + resolutionsNameValue[index].Length + 1;
                string ddd = CommandLine[index + 2].Substring(separatorIndex, CommandLine[index + 2].Length - separatorIndex);
                int.TryParse(ddd, out resolutionsValue[index]);
            }

            if (resolutionsValue[0] > 0 && resolutionsValue[1] > 0)
            {
                Screen.SetResolution(resolutionsValue[0], resolutionsValue[1], false);
            }
        }
#endif    }
}