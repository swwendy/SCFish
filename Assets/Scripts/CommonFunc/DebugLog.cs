using UnityEngine;
using System.Collections;
/// <summary>
/// DEBUG输出模式 UNITY_EDITOR  
/// 打包手机运行时不会出现LOG
/// </summary>
public class DebugLog
{
    /// <summary>
    /// 不做限制都会打印
    /// </summary>
    /// <param name="_message"></param>
    public static void LogAny(string _message)
    {
        Debug.Log("debug====>" + _message);
    }

    public static void Log(object message)
    {
#if UNITY_EDITOR
        Debug.Log("debug====>"+message);
#endif
    }
    public static void Log(object message,Object context)
    {
#if UNITY_EDITOR
        Debug.Log("debug====> " + message, context);
#endif
    }
    public static void LogError(object message)
    {
#if UNITY_EDITOR
        Debug.LogError("debug error====> " + message);
#endif
    }
    public static void LogError(object message, Object context)
    {
#if UNITY_EDITOR
        Debug.LogError("debug error====> " + message, context);
#endif
    }
    public static void LogWarning(object message)
    {
#if UNITY_EDITOR
        Debug.LogWarning("debug warning====> " + message);
#endif
    }
    public static void LogWarning(object message, Object context)
    {
#if UNITY_EDITOR
        Debug.LogWarning("debug warning====> " + message, context);
#endif
    }

    private static float time;

    public static void LogTime(object message)
    {
#if UNITY_EDITOR
        Debug.Log("debug time====> " + message + "    tiem=>" + (Time.realtimeSinceStartup - time));
        time = Time.realtimeSinceStartup;
#endif
    }

}
