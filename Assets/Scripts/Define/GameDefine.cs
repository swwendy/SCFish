﻿using UnityEngine;

    /// <summary>
#if UNITY_IOS
#elif UNITY_ANDROID
    //public static string LuancherURL = "http://lsbcdn.jslaisai.com/Lsb_Res_Beta/Android/   
    public static string LuancherURL = "http://47.110.128.120/Game_Res/QP_Lsb_Android/";
#else

    /// <summary>
#if UKGAME_SDK
#else
#endif
    /// <summary>


    #region Assetbundle文件名
    /// <summary>
    /// Assetbundle依赖项信息包文件名
    /// </summary>
#if UNITY_IOS
#elif UNITY_ANDROID
#else
    public static readonly string DependenciesAssetBundleName = "Windows";
#endif
    /// <summary>

    /// <summary>


    /// <summary>

    /// <summary>

    /// <summary>
    /// 背包图标资源
    /// </summary>
    public static readonly string HallBagIconAssetBundleName = "hallbagicon.resource";

    /// <summary>

    #endregion
    #region CSV表文件名
    /// <summary>
    /// 游戏横屏和竖屏资源文件名
    /// </summary>
    /// <summary>
    /// <summary>

    /// <summary>

    #endregion