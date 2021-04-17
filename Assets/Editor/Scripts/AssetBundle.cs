﻿using UnityEngine;



    [MenuItem("Tools/Build Windows AssetBundle")]

#if UNITY_EDITOR_WIN
#endif
#if UNITY_EDITOR_OSX
        BuildPipeline.BuildAssetBundles(savepath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSXIntel);
#endif
        AssetDatabase.SaveAssets();

    /// <summary>
    /// 实现复制srcPath文件夹中所有文件到aimPath文件夹的方法
    /// </summary>
    /// <param name="SourcePath"></param>
    /// <param name="TargetPath"></param>
    private static void CopyDirectory(string SourcePath, string TargetPath)
    {
        try
        {
            // 检查目标目录是否以目录分割字符结束如果不是则添加
            if (TargetPath[TargetPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                TargetPath += Path.DirectorySeparatorChar;
            }
            // 判断目标目录是否存在如果不存在则新建
            if (!Directory.Exists(TargetPath))
            {
                Directory.CreateDirectory(TargetPath);
            }
            // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
            // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
            // string[] fileList = Directory.GetFiles（srcPath）；
            string[] fileList = Directory.GetFileSystemEntries(SourcePath);
            // 遍历所有的文件和目录
            foreach (string file in fileList)
            {
                // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                if (Directory.Exists(file))
                {
                    CopyDirectory(file, TargetPath + Path.GetFileName(file));
                }
                // 否则直接Copy文件
                else
                {
                    File.Copy(file, TargetPath + Path.GetFileName(file), true);
                }
            }
        }
        catch (Exception exceptionMsg)
        {
            Debug.Log(exceptionMsg.Message);
        }
    }



    /// <summary>
    /// 获取编辑器打包设置的场景列表
    /// </summary>
    /// <returns></returns>
    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    [MenuItem("Tools/Build/WindowsPlayer")]
        Debug.Log(" 开始 BuildAllAssetBundles！");
        BuildPCAllAssetBundles();

        string defaultSpecified = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defaultSpecified +";WINDOWS_GUEST");

        Debug.Log(" 开始 BuildPlayer！" + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));
        BuildPipeline.BuildPlayer(GetScenePaths(), "Windows/来赛吧.exe", BuildTarget.StandaloneWindows,BuildOptions.None);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defaultSpecified);

        string AssetBundlesPath = Application.dataPath + "/../" + "Windows/来赛吧_Data/AssetBundles/Windows";
        Debug.Log(" 正在拷贝游戏资源: " + AssetBundlesPath);
        CopyDirectory(Application.dataPath + "/AssetBundles/Windows", AssetBundlesPath);

        string ConfigPath = Application.dataPath + "/../" + "Windows/来赛吧_Data/Config";
        Debug.Log(" 正在拷贝游戏配置: " + ConfigPath);
        CopyDirectory(Application.dataPath + "/Config", ConfigPath);
        
        Debug.Log("打包完成！");
    }


#if UNITY_IOS && UNITY_EDITOR

#if UKGAME_SDK
            proj.AddFrameworkToProject(target, "Frameworks/Plugins/iOS/UKGame_SDK/YKSDK.framework", false);
#endif

            // 设置签名  
            //          proj.SetBuildProperty (target, "CODE_SIGN_IDENTITY", "iPhone Distribution: _______________");  
            //          proj.SetBuildProperty (target, "PROVISIONING_PROFILE", "********-****-****-****-************");   

            // 保存工程  
            proj.WriteToFile(projPath);
            array.AddString("weixin");





#endif