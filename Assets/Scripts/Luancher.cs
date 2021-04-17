﻿using UnityEngine;

public class Luancher : MonoBehaviour
        LuancherState_UnZip,            //解压缩资源包

    // 编辑器模式下是否需要走更新流程
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#endif
    public static readonly bool EnableResMD5CRC = false;

    // 是否打印日志在屏幕上
    public static readonly bool IsPrintLogOnScreen = false;
        RefreshScreenResolution();
        //{            
           if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
               eLuancherState = LUANCHERSTATE.LuancherState_StartGame;
          else 

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN 

#if UNITY_IOS
#endif
            Debug.Log("unzip processs start-------:" );
                WriteLocalResConfig();
                DownHotfixAndSerABCsvFile();
        ResourceNeedUpdate |= CResVersionCompareUpdate.CompareABVersionAndUpdate(GameDefine.HallAnimeAssetBundleName, true);

        //android平台下检测apk是否需要更新
        if (Application.platform == RuntimePlatform.Android)
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
            }

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

        DownloadProcessBarImg.fillAmount = 0.9f;

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
        {
            yield break;
        }
        if (!Directory.Exists(zipedFolder))
            Directory.CreateDirectory(zipedFolder);

        yield return null;

        float process = 0;
        FileStream srcfs = File.OpenRead(fileToUnZip);
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
                                fs.Write(data, 0, size);
                                DownloadProcessUpdatetextObj.GetComponent<Text>().text = "存储空间不足，请释放足够空间后重新运行";
                                eLuancherState = LUANCHERSTATE.LuancherState_Over;
                                Debug.Log("unzip asset zip file failed!!!!!");
                                yield break;
                            }
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

    /*void UnzipAssetFile()

    /// <summary>
            yield break;

            DownloadProcessBarImg.fillAmount += process * percent;
            DebugLog.Log("copy process:" + DownloadProcessBarImg.fillAmount + " len:"+readlen);

            yield return null;


    void RefreshScreenResolution()
    {
#if WINDOWS_GUEST
        Resolution[] resolutions = Screen.resolutions;
        foreach (var value in resolutions)
        {
            Debug.Log("显示器支持的分辨率:" + value.ToString());
        }
        string[] CommandLine = System.Environment.CommandLine.Split('-');
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
#endif
}