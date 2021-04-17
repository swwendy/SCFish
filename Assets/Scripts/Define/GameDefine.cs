using UnityEngine;using XLua;/// <summary>/// 游戏通用定义/// </summary>[LuaCallCSharp]public class GameDefine{        /// <summary>    /// 应用程序PersistentDataPath    /// </summary>    public static readonly string AppPersistentDataPath = Application.persistentDataPath + "/";        /// <summary>    /// 下载的assetbundle保存目录    /// </summary>    public static readonly string AssetBundleSavePath = AppPersistentDataPath + "AssetBundle/";    /// <summary>    /// 屏幕截图保存目录    /// </summary>    public static readonly string ScreenshotSavePath = AppPersistentDataPath + "Screenshot/";    /// <summary>    /// 下载APK保存路径    /// </summary>    public static readonly string DownloadApkSavePath = AppPersistentDataPath + "ApkFile/";    /// <summary>    /// 微信用户头像保存路径    /// </summary>    public static readonly string HeadImageSavePath = AppPersistentDataPath + "HeadImage/";

    /// <summary>    /// 更新URL    /// </summary>
#if UNITY_IOS    //审核服luancher地址    //public static string LuancherURL = "http://lsbcdn.jslaisai.com/Lsb_Res_Beta/Ios/";    public static string LuancherURL = "http://lsbcdn.jslaisai.com/Lsb_Res/Ios/";
#elif UNITY_ANDROID    //测试服luancher地址
    //public static string LuancherURL = "http://lsbcdn.jslaisai.com/Lsb_Res_Beta/Android/   
    public static string LuancherURL = "http://47.110.128.120/Game_Res/QP_Lsb_Android/";
#else    public static string LuancherURL = "http://lsbcdn.jslaisai.com/Lsb_Res/Windows/";#endif#if UKGAME_SDK    public static readonly string ApkFileName = "Mnttddz_UK.apk";#else    public static readonly string ApkFileName = "LaiSaiBa_udt.apk";#endif    /// <summary>    /// 热更新lua脚本文件    /// </summary>    public static readonly string HotFixLuaFile = "HotFixLua.lua.data";    /// <summary>    /// 服务器资源版本号文件    /// </summary>    public static readonly string ServerResVersionFile = "SerResVersion.data";    /// <summary>    /// 本地资源版本号文件    /// </summary>    public static readonly string LocalResVersionFile = "LocalResVersion.data";

    /// <summary>    /// 游戏运行日志    /// </summary>
#if UKGAME_SDK    public static readonly string LocalRunLogFile = "MnttddzLog.data";
#else    public static readonly string LocalRunLogFile = "LaiSaiBaLog.data";
#endif
    /// <summary>    /// 服务器ab版本表文件    /// </summary>    public static readonly string ServerAssetbundleVersionDataFile = "ServerAssetbundleCsv.data";    /// <summary>    /// 本地ab资源的版本号，即已经下载到本地AB的版本，为后面使用是否需要更新提供依据（与服务器的ab版本号比较）    /// </summary>    public static readonly string LocalAssetbundleVersionDataFile = "LocalAssetBundleVer.data";    /// <summary>    /// 本地保存的账号信息    /// </summary>    public static readonly string AccountInfoFile = "AccountConfig.data";    public static readonly string AssetsZipFile = "Res.zip";


    #region Assetbundle文件名
    /// <summary>
    /// Assetbundle依赖项信息包文件名
    /// </summary>
#if UNITY_IOS    public static readonly string DependenciesAssetBundleName = "IOS";
#elif UNITY_ANDROID    public static readonly string DependenciesAssetBundleName = "Android";
#else
    public static readonly string DependenciesAssetBundleName = "Windows";
#endif
    /// <summary>    /// CSV assetbundle文件名    /// </summary>    public static readonly string CsvAssetbundleName = "csv.resource";    /// <summary>    /// 大厅 assetbundle文件名    /// </summary>    public static readonly string HallAssetbundleName = "hall.resource";

    /// <summary>    /// 大厅动画资源    /// </summary>    public static readonly string HallAnimeAssetBundleName = "hallanime.resource";


    /// <summary>    /// 更新频率低的大厅公共资源    /// </summary>    public static readonly string HallConstAssetBundleName = "hallconst.resource";

    /// <summary>    /// 通用扑克牌资源    /// </summary>    public static readonly string PokerAssetBundleName = "pokercommon.resource";

    /// <summary>
    /// 背包图标资源
    /// </summary>
    public static readonly string HallBagIconAssetBundleName = "hallbagicon.resource";

    /// <summary>    /// 通用麻将牌资源    /// </summary>    public static readonly string MahjongAssetBundleName = "mahjongcommon.resource";

    #endregion
    #region CSV表文件名
    /// <summary>    /// assetbundle管理表文件名    /// </summary>    public static readonly string AssetbundleCsvFileName = "AssetbundleCsv.txt";    /// <summary>    /// 游戏管理表文件名    /// </summary>    public static readonly string GameManageCsvFileName = "GameMangerCsv.txt";    /// <summary>    /// 游戏设置表文件名    /// </summary>    public static readonly string GameSettingCsvFileName = "GameSettingCsv.txt";    /// <summary>    /// 商店表文件名    /// </summary>    public static readonly string ShopCsvFileName = "ShopCsv.txt";    /// <summary>    /// 游戏数据文件名    /// </summary>    public static readonly string GameCsvFileName = "GameMangerCsv.txt";    /// <summary>    /// tips文件名    /// </summary>    public static readonly string TipsCsvFileName = "TipsCsv.txt";    /// <summary>    /// vip文件名    /// </summary>    public static readonly string VipCsvFileName = "VipCsv.txt";    /// <summary>
    /// 游戏横屏和竖屏资源文件名
    /// </summary>    public static readonly string GameUIFileName = "GameUI.txt";    /// <summary>    /// 拉霸概率文件名    /// </summary>    public static readonly string SlotCsvFileName = "GameSlotsRateCsv";    /// <summary>    /// 拉霸押注限制文件名    /// </summary>    public static readonly string SlotChipFileName = "GameSlotsBetCsv";    /// <summary>    /// 五子棋押注限制文件名    /// </summary>    public static readonly string FiveChipFileName = "GameFiveBetCsv";    /// <summary>    /// 俱乐部配置文件    /// </summary>    public static readonly string ClubFileName = "ClubCsv";    /// <summary>    /// 森林舞会配置文件    /// </summary>    public static readonly string ForestDanceFileName = "GameAnimMultipleCsv";    /// <summary>    /// 森林舞会音频文件    /// </summary>    public static readonly string ForestDanceAudioFileName = "GameAnimAudioCsv";    /// <summary>    /// 森林舞会押注文件    /// </summary>    public static readonly string ForestDanceBetFileName = "GameAnimBetCsv";    /// <summary>    /// 五子棋音频文件    /// </summary>    public static readonly string FiveInRowAudioFileName = "GameFiveAudioCsv";    /// <summary>    /// 5堆牛牛押注配置    /// </summary>    public static readonly string BullAllKillBetFileName = "GameNiu5BetCsv";    /// <summary>    /// 5堆牛牛牌型配置    /// </summary>    public static readonly string BullAllKillPokerTypeFileName = "GameNiu5awardsCsv";    /// <summary>    /// 5堆牛牛音频配置    /// </summary>    public static readonly string BullAllKillAudioFileName = "GameNiu5AudioCsv";
    /// <summary>    /// 房卡配置    /// </summary>    public static readonly string AppointmentFileName = "RoomPriceCsv";
    /// <summary>    /// 自建比赛配置    /// </summary>    public static readonly string SelfContestFileName = "ContestPriceCSV";

    /// <summary>    /// 车行押注限制文件名    /// </summary>    public static readonly string CarBetFileName = "GameCarBetCsv";

    #endregion}