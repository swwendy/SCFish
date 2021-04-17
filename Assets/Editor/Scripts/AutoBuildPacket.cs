using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class MyEditorScript
{
    //得到工程中所有场景名称
    static string[] SCENES = FindEnabledEditorScenes();
    //一系列批量build的操作
    /*[MenuItem("Build/Android General")]
    static void PerformAndroidGeneralBuild()
    {
        BulidTarget("General", "Android");
    }

    [MenuItem("Build/Android NetBar")]
    static void PerformAndroidQQBuild()
    {
        BulidTarget("NetBar", "Android");
    }

    //[MenuItem("Build/Android ALL")]
    static void PerformAndroidALLBuild()
    {
        BulidTarget("General", "Android");
        BulidTarget("NetBar", "Android");
    }

    [MenuItem("Build/iPhone General")]
    static void PerformiPhoneQQBuild()
    {
        BulidTarget("General", "IOS");
    }


    //[MenuItem("Build/iPhone ALL")]
    static void PerformiPhoneALLBuild()
    {
        BulidTarget("General", "IOS");
    }*/

    //这里封装了一个简单的通用方法。
    static void BulidTarget(string name, string target)
    {
        string app_name = "JoyBigGamer";
        string target_dir = Application.dataPath + "/TargetAndroid";
        string target_name = app_name + ".apk";
        BuildTargetGroup targetGroup = BuildTargetGroup.Android;
        BuildTarget buildTarget = BuildTarget.Android;
        string applicationPath = Application.dataPath.Replace("/JoyBigGamer/Assets", "");

        if (target == "Android")
        {
            target_dir = applicationPath + "/Android_APK";
            target_name = app_name + ".apk";
            targetGroup = BuildTargetGroup.Android;
        }
        if (target == "IOS")
        {
            target_dir = applicationPath + "/IOS_Packet";
            target_name = app_name;
            targetGroup = BuildTargetGroup.iOS;
            buildTarget = BuildTarget.iOS;
        }

        //每次build删除之前的残留
        if (Directory.Exists(target_dir))
        {
            if (File.Exists(target_name))
            {
                File.Delete(target_name);
            }
        }
        else
        {
            Directory.CreateDirectory(target_dir);
        }

        //==================这里是比较重要的东西=======================
        switch (name)
        {
            case "General":
                {
                    //PlayerSettings.bundleIdentifier = "com.game.qq";
                    //PlayerSettings.bundleVersion = "v0.0.1";
                    string scriptdefinestr = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                    //scriptdefinestr += ";GENERAL";
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, scriptdefinestr);
                }
                break;
            case "NetBar":
                {
                    //PlayerSettings.bundleIdentifier = "com.game.qq";
                    //PlayerSettings.bundleVersion = "v0.0.1";
                    string scriptdefinestr = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                    //scriptdefinestr += ";NETBAR";
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, scriptdefinestr);
                }
                break;
        }

        //==================这里是比较重要的东西=======================

        //开始Build场景，等待吧～
        GenericBuild(SCENES, target_dir + "/" + target_name, buildTarget, BuildOptions.None);

    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        /*EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        string res = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);

        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }*/
    }

}

