using UnityEngine;using System.Collections;using System.Collections.Generic;using System.IO;using System.Text;using System;

public class LogFile : MonoBehaviour{    static List<string> mLines = new List<string>();    static List<string> mWriteTxt = new List<string>();    private static string outpath;    private static string DetaTimeString;    public static void InitLog()    {
#if !WINDOWS_GUEST        //Application.persistentDataPath Unity中只有这个路径是既可以读也可以写的。
        outpath = GameDefine.AppPersistentDataPath + GameDefine.LocalRunLogFile;                //每次启动客户端删除之前保存的Log
        if (System.IO.File.Exists(outpath))        {            File.Delete(outpath);        }
#else
        DateTime NowTime = DateTime.Now.ToLocalTime();
        outpath = "LogFile ["+ NowTime.ToString("yyyy.MM.dd-HH.mm.ss") + "] .txt";
        Debug.Log("=====================" + outpath);
#endif
        //在这里做一个Log的监听

        Application.logMessageReceived += HandleLog;        Debug.Log("LaiSaiBa Debug Log");    }    void Update()    {        //因为写入文件的操作必须在主线程中完成，所以在Update中写入文件。        if (mWriteTxt.Count > 0)        {            string[] temp = mWriteTxt.ToArray();            foreach (string t in temp)            {                using (StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8))                {                    writer.WriteLine(t);                }                mWriteTxt.Remove(t);            }        }    }    static void HandleLog(string logString, string stackTrace, LogType type)    {            
        DetaTimeString = "[" + DateTime.Now.ToLocalTime().ToString()+"] " + logString; 
        mWriteTxt.Add(DetaTimeString);       // if (type == LogType.Error || type == LogType.Exception)        {            Log(logString);            if (type == LogType.Error || type == LogType.Exception)
            {
                Log(stackTrace);
#if !UNITY_EDITOR
                GameCommon.SendMail(DetaTimeString +stackTrace);
#endif
                mWriteTxt.Add(stackTrace);
            }        }    }    //这里我把错误的信息保存起来，用来输出在手机屏幕上    static public void Log(params object[] objs)    {        string text = "";        for (int i = 0; i < objs.Length; ++i)        {            if (i == 0)            {                text += objs[i].ToString();            }            else            {                text += ", " + objs[i].ToString();            }        }        if (Application.isPlaying && Luancher.IsPrintLogOnScreen)        {            if (mLines.Count > 10)            {                mLines.RemoveAt(0);            }            mLines.Add(text);        }    }    void OnGUI()    {        if(Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
        {
            return;        }        GUI.color = Color.yellow;        for (int i = 0, imax = mLines.Count; i < imax; ++i)        {            GUILayout.Label(mLines[i]);        }    }}