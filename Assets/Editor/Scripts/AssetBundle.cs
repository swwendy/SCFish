using UnityEngine;using System.Collections;using UnityEditor;using System.IO;using System;#if UNITY_IOS && UNITY_EDITORusing UnityEditor.iOS.Xcode;#endifpublic class CreateAssetBundles : Editor{    [MenuItem("Tools/Build Android AssetBundle")]    static void BuildAndroidAllAssetBundles()    {        Caching.ClearCache();        string savepath = Application.dataPath + "/AssetBundles/Android";        if (!Directory.Exists(savepath))            Directory.CreateDirectory(savepath);        BuildPipeline.BuildAssetBundles(savepath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);        AssetDatabase.SaveAssets();        AssetDatabase.Refresh();        Debug.Log("Build Android AssetBundles Success");    }    [MenuItem("Tools/Build IOS AssetBundle")]    static void BuildIOSAllAssetBundles()    {        Caching.ClearCache();        string savepath = Application.dataPath + "/AssetBundles/IOS";        if (!Directory.Exists(savepath))            Directory.CreateDirectory(savepath);

        BuildPipeline.BuildAssetBundles(savepath, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.iOS);        AssetDatabase.SaveAssets();        AssetDatabase.Refresh();        Debug.Log("Build IOS AssetBundles Success");    }

    [MenuItem("Tools/Build Windows AssetBundle")]    static void BuildPCAllAssetBundles()    {        Caching.ClearCache();        string savepath = Application.dataPath + "/AssetBundles/Windows";        if (!Directory.Exists(savepath))            Directory.CreateDirectory(savepath);

#if UNITY_EDITOR_WIN        BuildPipeline.BuildAssetBundles(savepath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
#endif
#if UNITY_EDITOR_OSX
        BuildPipeline.BuildAssetBundles(savepath, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSXIntel);
#endif
        AssetDatabase.SaveAssets();        AssetDatabase.Refresh();        Debug.Log("Build Windows AssetBundles Success");    }

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

    [MenuItem("Tools/Build/WindowsPlayer")]    static void BuildWindowsPlayer()    {
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


#if UNITY_IOS && UNITY_EDITOR    // ios版本xcode工程维护代码      [UnityEditor.Callbacks.PostProcessBuild(999)]    public static void OnPostprocessBuild(BuildTarget BuildTarget, string path)    {        if (BuildTarget == BuildTarget.iOS)        {            string projPath = PBXProject.GetPBXProjectPath(path);            PBXProject proj = new PBXProject();            proj.ReadFromString(File.ReadAllText(projPath));            // 获取当前项目名字              string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());            // 对所有的编译配置设置选项              proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");            // 添加依赖库              // 语音sdk              proj.AddFrameworkToProject(target, "Security.framework", false);            proj.AddFrameworkToProject(target, "CoreTelephony.framework", false);            proj.AddFrameworkToProject(target, "libc++.tbd", false);            proj.AddFrameworkToProject(target, "libz.tbd", false);            proj.AddFrameworkToProject(target, "libsqlite3.0.tbd", false);            //proj.AddFrameworkToProject(target, "Frameworks/Plugins/iOS/AlipaySDK/AlipaySDK.framework", false);

#if UKGAME_SDK
            proj.AddFrameworkToProject(target, "Frameworks/Plugins/iOS/UKGame_SDK/YKSDK.framework", false);
#endif

            // 设置签名  
            //          proj.SetBuildProperty (target, "CODE_SIGN_IDENTITY", "iPhone Distribution: _______________");  
            //          proj.SetBuildProperty (target, "PROVISIONING_PROFILE", "********-****-****-****-************");   

            // 保存工程  
            proj.WriteToFile(projPath);            // 修改plist              string plistPath = path + "/Info.plist";            PlistDocument plist = new PlistDocument();            plist.ReadFromString(File.ReadAllText(plistPath));            PlistElementDict rootDict = plist.root;            //设置微信支付宝白名单            PlistElementArray array = rootDict.CreateArray("LSApplicationQueriesSchemes"); 
            array.AddString("weixin");            array.AddString("wechat");            //array.AddString("alipay");            // 语音所需要的声明，iOS10必须              rootDict.SetString("NSContactsUsageDescription", "是否允许此游戏使用麦克风？");            // 保存plist              plist.WriteToFile(plistPath);        }    }





#endif}