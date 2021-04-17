﻿using UnityEngine;

public class LogFile : MonoBehaviour
#if !WINDOWS_GUEST
        outpath = GameDefine.AppPersistentDataPath + GameDefine.LocalRunLogFile;
        if (System.IO.File.Exists(outpath))
#else
        DateTime NowTime = DateTime.Now.ToLocalTime();
        outpath = "LogFile ["+ NowTime.ToString("yyyy.MM.dd-HH.mm.ss") + "] .txt";
        Debug.Log("=====================" + outpath);
#endif
        //在这里做一个Log的监听

        Application.logMessageReceived += HandleLog;
        DetaTimeString = "[" + DateTime.Now.ToLocalTime().ToString()+"] " + logString; 
        mWriteTxt.Add(DetaTimeString);
            {
                Log(stackTrace);
#if !UNITY_EDITOR
                GameCommon.SendMail(DetaTimeString +stackTrace);
#endif
                mWriteTxt.Add(stackTrace);
            }
        {
            return;