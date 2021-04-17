/** * 游戏常用函数 **/





using UnityEngine;using System.IO;using System.Collections.Generic;using System.Text.RegularExpressions;using System.Text;using XLua;using System;using System.Security.Cryptography;using System.Net.Mail;
using System.Net;
using System.Linq;

[LuaCallCSharp]public class GameCommon{    #region 平台资源读取    public static string STREAMING_ASSETS_ANDROI_PATH = Application.persistentDataPath + "/";    public static string STREAMING_ASSETS_ANDROID_JAR_PATH = "jar:file://" + Application.dataPath + "!/assets/";    public static string STREAMING_ASSETS_IPHONE_PATH = Application.dataPath + "/Raw/";    public static string STREAMING_ASSETS_PATH = Application.dataPath + "/StreamingAssets/";    public static string GetStreamingAssetsPath(string path)    {        if (Application.platform == RuntimePlatform.Android)        {            return STREAMING_ASSETS_ANDROI_PATH + path;        }        else if (Application.platform == RuntimePlatform.IPhonePlayer)        {            return STREAMING_ASSETS_IPHONE_PATH + path;        }        return STREAMING_ASSETS_PATH + path;    }    /// <summary>    /// 获取应用在各平台下streamingAsset对应的路径    /// </summary>    /// <returns></returns>    public static string GetAppStreamingAssetPath()    {        string DataAssetPath = string.Empty;        if (Application.platform == RuntimePlatform.Android)        {            DataAssetPath = "jar:file://" + Application.dataPath + "!/assets/";        }        else if (Application.platform == RuntimePlatform.IPhonePlayer)        {            DataAssetPath = Application.dataPath + "/Raw/";        }        else        {            DataAssetPath = Application.dataPath + "/StreamingAssets/";        }        return DataAssetPath;    }    public static bool HasStreamingAssetsFile(string path, string fileName)    {        if (Application.platform == RuntimePlatform.Android)        {            string assetPath = GetStreamingAssetsPath(path);            if (!Directory.Exists(assetPath)) Directory.CreateDirectory(assetPath);            string filePath = assetPath + "/" + fileName;            if (!File.Exists(filePath))            {                string jarFilePath = STREAMING_ASSETS_ANDROID_JAR_PATH + path + "/" + fileName;                WWW www = new WWW(jarFilePath);                while (!www.isDone) { }                if (www.error == null)                {                    FileStream fs1 = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);                    byte[] tBytes = www.bytes;                    fs1.Write(tBytes, 0, tBytes.Length);                    fs1.Flush();                    fs1.Close();                }                www.Dispose();                www = null;            }            return File.Exists(filePath);        }        else if (Application.platform == RuntimePlatform.IPhonePlayer) { }        return true;    }#endregion#region 聊天关键字屏蔽    /// <summary>    /// 单个屏蔽字节点    /// </summary>    //class ForbiddenWord    //{    //    public char character;    //    public bool isEnd = false;    //    public Dictionary<char, ForbiddenWord> children = new Dictionary<char, ForbiddenWord>();    //}    /// <summary>    /// 缓存所有屏蔽字    /// </summary>    private static Dictionary<char,List<string> > mForbiddenWord = new Dictionary<char, List<string> >();        /// <summary>    /// 聊天关键字屏蔽,true是有敏感字，false是没有    /// </summary>    public static bool CheckForbiddenWord(ref string InCheckStr, bool bReplace = false)    {        //空的直接返回false        if (string.IsNullOrEmpty(InCheckStr))        {            return false;        }        //是否已经读取屏蔽字表        if (mForbiddenWord.Count == 0)        {            ReadForbiddenFile();        }        if (mForbiddenWord.Count == 0)            return false;        bool isForbidden = false;         StringBuilder Replacesb = new StringBuilder(InCheckStr.Length);        int checkstrLen = InCheckStr.Length;         for (int i = 0; i < checkstrLen; i++)        {            char word = InCheckStr[i];            if (mForbiddenWord.ContainsKey(word))//如果在字典表中存在这个key            {                                List<string> ForbiddenStrlist = mForbiddenWord[word];                //把该key的字典集合按 字符数排序(方便下面从少往多截取字符串查找)                foreach (string ForbiddenStr in ForbiddenStrlist)                {                    if (i + ForbiddenStr.Length <= checkstrLen)                    //如果需截取的字符串的索引小于总长度 则执行截取                    {                        string result = InCheckStr.Substring(i, ForbiddenStr.Length);                        //根据关键字长度往后截取相同的字符数进行比较                        if (result == ForbiddenStr)                        {                            isForbidden = true;                            Replacesb.Append(GetReplaceCharString(result));                            i = i + ForbiddenStr.Length - 1;                            //比较成功 同时改变i的索引                            break;                        }                    }                }                if (!isForbidden)                    Replacesb.Append(word);            }            else                Replacesb.Append(word);        }        if (bReplace)            InCheckStr = Replacesb.ToString();        return isForbidden;    }    /// 替换星号    private static string GetReplaceCharString(string value)    {        string starNum = string.Empty;        for (int i = 0; i < value.Length; i++)        {            starNum += "*";        }        return starNum;    }    /// <summary>    /// 读取屏蔽字文件    /// </summary>    static void ReadForbiddenFile()    {        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.CsvAssetbundleName);        if (bundle == null)            return;        TextAsset datatxt = bundle.LoadAsset("forbidden.txt", typeof(TextAsset)) as TextAsset;        if (datatxt == null)            return;        string ForbiddenStr = datatxt.ToString();        string[] Forbiddenlist = ForbiddenStr.Split('|');        foreach (var str in Forbiddenlist)        {        char key = str[0];            if (mForbiddenWord.ContainsKey(key))                mForbiddenWord[key].Add(str);            else                mForbiddenWord.Add(key, new List<string>() { str });        }    }    #endregion    #region 检测手机号码是否合法    public static bool CheckPhoneIsAble(string input)    {        if (input.Length < 11)        {            return false;        }        //电信手机号码正则        string dianxin = @"^1[3578][01379]\d{8}$";        Regex regexDX = new Regex(dianxin);        //联通手机号码正则        string liantong = @"^1[34578][01256]\d{8}";        Regex regexLT = new Regex(liantong);        //移动手机号码正则        string yidong = @"^(1[012345678]\d{8}|1[345678][012356789]\d{8})$";        Regex regexYD = new Regex(yidong);        if (regexDX.IsMatch(input) || regexLT.IsMatch(input) || regexYD.IsMatch(input))        {            return true;        }        else        {            return false;        }    }    #endregion    #region 时间格式转换    // DateTime --> long    public static long ConvertDataTimeToLong(DateTime dt)    {        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));        TimeSpan toNow = dt.Subtract(dtStart);        return (long)toNow.TotalSeconds;        //timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));    }    // long --> DateTime    public static DateTime ConvertLongToDateTime(long d)    {        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));        //long lTime = long.Parse(d.ToString());        //TimeSpan toNow = new TimeSpan(lTime);        DateTime dtResult = dtStart.AddSeconds(d);        return dtResult;    }

    // long --> DateTime
    public static DateTime ConvertLongToDateTime(double d)    {        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //long lTime = long.Parse(d.ToString());
        //TimeSpan toNow = new TimeSpan(lTime);
        DateTime dtResult = dtStart.AddSeconds(d);        return dtResult;    }


    //将秒数转化为时分秒
    public static string SecondsToDateTime(long duration)
    {
        TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(duration));
        string str = string.Empty;
        if (ts.Days > 0)
        {
            str = String.Format("{0:00} : {1:00} : {2:00} : {3:00}", ts.Days,ts.Hours, ts.Minutes, ts.Seconds);
        }
        if (ts.Hours > 0)
        {
            str = String.Format("{0:00} : {1:00} : {2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }
        if (ts.Hours == 0 && ts.Minutes > 0)
        {
            str = String.Format("{0:00} : {1:00}", ts.Minutes, ts.Seconds);
        }
        if (ts.Hours == 0 && ts.Minutes == 0)
        {
            str = String.Format("{0:00}", ts.Seconds);
        }
        return str;
    }
    #endregion
        #region 扑克牌通用    //获取花色




        public static int GetCardColor(byte card)    {        int type = card >> 4;        return type;    }    //获取牌值    public static int GetCardValue(byte card)    {        if (card == 0x41 || card == 0x4E)//小王        {            return -1;        }        if (card == 0x42 || card == 0x4F) //大王        {            return 0;        }        int value = card & 0x0F;        return value;    }    static readonly string[] PokerTypes = { "_diamond", "_club", "_heart", "_spade", "_clown" };    public static string GetPokerMat(byte card, byte laizi = 0)    {        //if (card == laizi)        //    return GetCardValue(card).ToString() + "_star";        return GetCardValue(card).ToString() + PokerTypes[GetCardColor(card)];    }    #endregion    #region 生成md5校验码    public static string GenerateFileMd5(string filename)    {        string filemd5str = null;        try        {            using (var fileStream = File.OpenRead(filename))            {                MD5 md5 = MD5.Create();                //计算指定Stream 对象的哈希值                  byte[] fileMD5Bytes = md5.ComputeHash(fileStream);                //将byte[]装换成字符串(fileMD5Bytes);                                                    filemd5str = System.BitConverter.ToString(fileMD5Bytes).Replace("-", "").ToLower();            }        }        catch (Exception ex)        {            Debug.Log(ex.Message);        }        return filemd5str;    }    #endregion    /// <summary>    /// 发送邮件    /// </summary>    /// <param name="content"></param>    public static void SendMail(string content)    {        MailMessage mail = new MailMessage();
        //收件人地址
        mail.To.Add(new MailAddress("joybiggamer@sina.com"));
        //发件人地址
        mail.From = new MailAddress("joybiggamer@sina.com");
        //这个可以不指定
        //mail.Sender = new System.Net.Mail.MailAddress("xxx@sina.com", "SenderName");

        mail.Subject = "PhoenixEgg dump strack log";        mail.BodyEncoding = Encoding.UTF8;        string appver = "Runplatform:"+ Application.platform+",AppVer:" + GameMain.Instance.GetAppVersion()             + ",ResVer:" + GameMain.Instance.GetResVersion() + "\r\n";        mail.Body = appver + content;
        //mm.IsBodyHtml = true;
        mail.Priority = MailPriority.High; // 设置发送邮件的优先级        SmtpClient smtClient = new SmtpClient();
        //指定邮件服务器
        smtClient.Host = "smtp.sina.com";
        //smtp邮件服务器的端口号  
        smtClient.Port = 25;
        //启用ssl,也就是安全发送
        //smtCliend.EnableSsl = false;
        //smtCliend.UseDefaultCredentials = false;
        //指定邮件的发送方式
        smtClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //设置发件人邮箱的用户名和地址，使用公共邮件服务器一般需要提供，不然发送不会成功
        smtClient.Credentials = new NetworkCredential("joybiggamer@sina.com", "963.8520") as ICredentialsByHost;
        try
        {            smtClient.SendAsync(mail, null);        }        catch (System.Net.Mail.SmtpException ex)        {            Debug.Log("mail send error:" + ex.ToString());        }        Debug.Log("mail send end");    }}[LuaCallCSharp]public static class GameFunction{    /// <summary>    /// 世界物体转换为UI坐标    /// </summary>    /// <param name="_worldGo">世界物体</param>    /// <param name="_worldCamera">世界摄像机</param>    /// <param name="_uiCanvas">UI画布</param>    /// <param name="_uiCamera">UI摄像机</param>    /// <returns>UI坐标</returns>    public static Vector2 WorldToLocalPointInRectangle(this Transform _worldGo, Camera _worldCamera, Canvas _uiCanvas, Camera _uiCamera)    {        Vector2 mUIPoint = RectTransformUtility.WorldToScreenPoint(_worldCamera, _worldGo.position);        RectTransformUtility.ScreenPointToLocalPointInRectangle(            (RectTransform)_uiCanvas.transform, mUIPoint, _uiCamera, out mUIPoint);        return mUIPoint;    }    /// <summary>    /// 世界物体转换为UI坐标    /// </summary>    /// <param name="_worldPosition">世界坐标</param>    /// <param name="_worldCamera">世界摄像机</param>    /// <param name="_uiCanvas">UI画布</param>    /// <param name="_uiCamera">UI摄像机</param>    /// <returns>UI坐标</returns>    public static Vector2 WorldToLocalPointInRectangle(this Vector3 _worldPosition, Camera _worldCamera, Canvas _uiCanvas, Camera _uiCamera)    {        Vector2 mUIPoint = RectTransformUtility.WorldToScreenPoint(_worldCamera, _worldPosition);        RectTransformUtility.ScreenPointToLocalPointInRectangle(            (RectTransform)_uiCanvas.transform, mUIPoint, _uiCamera, out mUIPoint);        return mUIPoint;    }    //This function finds out on which side of a line segment the point is located.    //The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on    //the line segment, project it on the line using ProjectPointOnLine() first.    //Returns 0 if point is on the line segment.    //Returns 1 if point is outside of the line segment and located on the side of linePoint1.    //Returns 2 if point is outside of the line segment and located on the side of linePoint2.    public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)    {        Vector3 lineVec = linePoint2 - linePoint1;        Vector3 pointVec = point - linePoint1;        float dot = Vector3.Dot(pointVec, lineVec);        //point is on side of linePoint2, compared to linePoint1        if (dot > 0)        {            //point is on the line segment            if (pointVec.magnitude <= lineVec.magnitude)            {                return 0;            }            //point is not on the line segment and it is on the side of linePoint2            else            {                return 2;            }        }        //Point is not on side of linePoint2, compared to linePoint1.        //Point is not on the line segment and it is on the side of linePoint1.        else        {            return 1;        }    }    //Two non-parallel lines which may or may not touch each other have a point on each line which are closest    //to each other. This function finds those two points. If the lines are not parallel, the function     //outputs true, otherwise false.    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)    {        closestPointLine1 = Vector3.zero;        closestPointLine2 = Vector3.zero;        float a = Vector3.Dot(lineVec1, lineVec1);        float b = Vector3.Dot(lineVec1, lineVec2);        float e = Vector3.Dot(lineVec2, lineVec2);        float d = a * e - b * b;        //lines are not parallel        if (d != 0.0f)        {            Vector3 r = linePoint1 - linePoint2;            float c = Vector3.Dot(lineVec1, r);            float f = Vector3.Dot(lineVec2, r);            float s = (b * f - c * e) / d;            float t = (a * f - c * b) / d;            closestPointLine1 = linePoint1 + lineVec1 * s;            closestPointLine2 = linePoint2 + lineVec2 * t;            return true;        }        else        {            return false;        }    }    //Returns true if line segment made up of pointA1 and pointA2 is crossing line segment made up of    //pointB1 and pointB2. The two lines are assumed to be in the same plane.    public static bool AreLineSegmentsCrossing(Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)    {        Vector3 closestPointA;        Vector3 closestPointB;        int sideA;        int sideB;        Vector3 lineVecA = pointA2 - pointA1;        Vector3 lineVecB = pointB2 - pointB1;        bool valid = ClosestPointsOnTwoLines(out closestPointA, out closestPointB, pointA1, lineVecA.normalized, pointB1, lineVecB.normalized);        //lines are not parallel        if (valid)        {            sideA = PointOnWhichSideOfLineSegment(pointA1, pointA2, closestPointA);            sideB = PointOnWhichSideOfLineSegment(pointB1, pointB2, closestPointB);            if ((sideA == 0) && (sideB == 0))            {                return true;            }            else            {                return false;            }        }        //lines are parallel        else        {            return false;        }    }    public static bool IsLineCrossRectangle(Vector2 linePoint1, Vector2 linePoint2, Rect rect)    {        bool pointAInside = false;        bool pointBInside = false;        pointAInside = rect.Contains(linePoint1);        if (!pointAInside)        {            pointBInside = rect.Contains(linePoint2);        }        //none of the points are inside, so check if a line is crossing        if (!pointAInside && !pointBInside)        {            Vector2 rectA = new Vector2(rect.xMin, rect.yMin);            Vector2 rectB = new Vector2(rect.xMax, rect.yMin);            Vector2 rectC = new Vector2(rect.xMax, rect.yMax);            Vector2 rectD = new Vector2(rect.xMin, rect.yMax);            bool lineACrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectA, rectB);            bool lineBCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectB, rectC);            bool lineCCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectC, rectD);            bool lineDCrossing = AreLineSegmentsCrossing(linePoint1, linePoint2, rectD, rectA);            if (lineACrossing || lineBCrossing || lineCCrossing || lineDCrossing)            {                return true;            }            else            {                return false;            }        }        else        {            return true;        }    }    public static Rect GetSpaceRect(Canvas canvas, RectTransform rect, Camera camera)    {        Rect spaceRect = rect.rect;        Vector3 spacePos = camera.WorldToScreenPoint(rect.position);        spaceRect.x = spaceRect.x * rect.localScale.x * canvas.scaleFactor + spacePos.x;        spaceRect.y = spaceRect.y * rect.localScale.y * canvas.scaleFactor + spacePos.y;        spaceRect.width = spaceRect.width * rect.localScale.x * canvas.scaleFactor;        spaceRect.height = spaceRect.height * rect.localScale.y * canvas.scaleFactor;        return spaceRect;    }

    public static bool IsPointerOnUI()
    {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
#endif
            return true;
        return false;
    }

    public static void PreloadPrefab(AssetBundle ab, string prefabName)
    {
        GameObject prefab = (GameObject)ab.LoadAsset(prefabName);
        if (prefab == null)
        {
            DebugLog.LogWarning("Fail to preload prefab:" + prefabName);
            return;
        }

        DebugLog.Log("preload prefab:" + prefabName + " assetbundle:" + ab.name);
        GameObject go = (GameObject)GameMain.instantiate(prefab);
        GameObject.Destroy(go);
    }
    public static string FormatCoinText(long grade, bool bShowAdd = false, bool bReduce = true)
    {
        string text = "";

        if (grade == 0)
            text = "0";
        else
        {
            long coin = grade;
            if (coin < 0)
                coin = -coin;

            if (bReduce)
            {
                long wNum = coin / 10000;
                coin %= 10000;
                long kNum = coin / 1000;
                coin %= 1000;
                long hNum = coin / 100;

                string[] str = new string[] { "万", "千" };
                if (wNum > 0)
                    text = string.Format(kNum > 0 ? "{0}.{1}{2}" : "{0}{2}", wNum, kNum, str[0]);
                else if (kNum > 0)
                    text = string.Format(hNum > 0 ? "{0}.{1}{2}" : "{0}{2}", kNum, hNum, str[1]);
                else
                    text = coin.ToString();
            }
            else
                text = coin.ToString();

            if (grade < 0)
                text = "-" + text;
            else if (bShowAdd)
                text = "+" + text;
        }

        return text;
    }

    /// <summary>
    /// 获得约据游戏自定义规则。
    /// </summary>
    /// <param name="RuleTextData">游戏规则</param>
    /// <param name="GameID">游戏ID</param>
    public static void  GetAppointmentRuleTextData(ref string RuleTextData,GameKind_Enum GameID)    {        AppointmentData data = AppointmentDataManager.AppointmentDataInstance().GetCurrentAppointment();        RuleTextData = "";        if (data == null)        {
            return;        }        if (GameID == GameKind_Enum.GameKind_LandLords)        {            if (data.maxpower == 250)
                RuleTextData = "打" + data.playtimes.ToString() + "局 不封顶";            else                RuleTextData = "打" + data.playtimes.ToString() + "局 最高" + data.maxpower + "倍";        }
        else if (GameID == GameKind_Enum.GameKind_GuanDan)        {            GuanDanAppointmentData appointmentData = (GuanDanAppointmentData)data;            if (appointmentData.terData_.playType == TePlayType.Times)            {                if (appointmentData.terData_.cp == CurrentPoker.Two)                    RuleTextData = "打" + appointmentData.terData_.times.ToString() + "局 双下" + appointmentData.terData_.score.ToString() + "分 打2";                else                    RuleTextData = "打" + appointmentData.terData_.times.ToString() + "局 双下" + appointmentData.terData_.score.ToString() + "分 随机打";            }

            else            {                if (appointmentData.terData_.vectory == 1)                    RuleTextData = "连续打 过A胜利";                else                    RuleTextData = "连续打 过" + appointmentData.terData_.vectory.ToString() + "胜利";            }        }
        else if (GameID == GameKind_Enum.GameKind_Mahjong ||
                 GameID == GameKind_Enum.GameKind_YcMahjong ||
                 GameID == GameKind_Enum.GameKind_CzMahjong)
        {
            if (data.maxpower == 250 || data.maxpower == 0)
                RuleTextData = "打" + data.playtimes.ToString() + "局 不封顶";
            else
                RuleTextData = "打" + data.playtimes.ToString() + "局 最高" + data.maxpower + "倍";

            if (GameID == GameKind_Enum.GameKind_Mahjong)
            {
                MahjongAppointmentData mahjongData = (MahjongAppointmentData)data;
                RuleTextData += GameKind.HasFlag(0, mahjongData.palyType) ? "\n自摸加底" : "\n自摸加翻";
                RuleTextData += GameKind.HasFlag(1, mahjongData.palyType) ? "\n点杠花（点炮）\n" : "\n点杠花（自摸）\n";
                RuleTextData += GameKind.HasFlag(2, mahjongData.palyType) ? " 幺九将对" : "";
                RuleTextData += GameKind.HasFlag(3, mahjongData.palyType) ? " 门清中张" : "";
                RuleTextData += GameKind.HasFlag(4, mahjongData.palyType) ? " 天地胡" : "";

            }
            else if (GameID == GameKind_Enum.GameKind_CzMahjong)
            {
                CzMahjongAppointmentData mahjongData = (CzMahjongAppointmentData)data;

                RuleTextData += "\n";
                string[] wanFaName = { "包三口", " 包四口", " 擦背", " 吃" };
                for (int index = 0; index <4; ++index)
                {
                    if (GameKind.HasFlag(index, mahjongData.wanFa))
                    {
                        RuleTextData += wanFaName[index];
                    }
                }

                string[] qiHuName = { "", "\n一番起胡", "\n硬一番起胡", "\n3花起胡", "\n4花起胡" };

                RuleTextData += qiHuName.Count() > 5 ? "" : qiHuName[mahjongData.qiHu];
                RuleTextData += "\n" + mahjongData.diHua + "底花";
            }
        }else if (GameID == GameKind_Enum.GameKind_GouJi)
        {
            GoujiAppointmentData gouJiData = (GoujiAppointmentData)(data);
            RuleTextData = "打" + gouJiData.playtimes.ToString() + "局 \n";
            string[] wanFaName = { "宣点\n", "开点发4\n", "憋三\n", "大王二杀一\n", "进贡\n" };
            for (int index = 0; index < 5; ++index)
            {
                if (GameKind.HasFlag(index, gouJiData.wanFa))
                {
                    RuleTextData += wanFaName[index];
                }
            }
        }else if(GameID == GameKind_Enum.GameKind_HongZhong)
        {
            HzMahjongAppointmentData mahjongData = (HzMahjongAppointmentData)(data);
            RuleTextData = "打" + mahjongData.playtimes.ToString() + "局 \n" + "抓鸟" + mahjongData.birdNum+"张 \n";
            string[] wanFaName = { "红中是赖子\n", "可以点炮\n",};
            for (int index = 0; index < 2; ++index)
            {
                if (GameKind.HasFlag(index, mahjongData.wanFa))
                {
                    RuleTextData += wanFaName[index];
                }
            }
        }else if(GameID == GameKind_Enum.GameKind_Chess)
        {
            ChessAppointmentData chessData = (ChessAppointmentData)(data);
            RuleTextData = "打" + chessData.playtimes.ToString() + "局 局时 "  +chessData.ChessTime + "分";
        }    }

    /// <summary>
    /// 创建约据房间数据
    /// </summary>
    /// <param name="GameID"></param>
    /// <returns></returns>    public static AppointmentData CreateAppointmentData(GameKind_Enum GameID)
    {
        AppointmentData publicAppointmentData = null;
        switch (GameID)
        {
            case GameKind_Enum.GameKind_GuanDan:
                publicAppointmentData = new GuanDanAppointmentData(4);
                break;
            case GameKind_Enum.GameKind_Mahjong:
                publicAppointmentData = new MahjongAppointmentData(4);
                break;
            case GameKind_Enum.GameKind_CzMahjong:
                publicAppointmentData = new CzMahjongAppointmentData(4);
                break;
            case GameKind_Enum.GameKind_YcMahjong:
                publicAppointmentData = new YcMahjongAppointmentData(4);
                break;
            case GameKind_Enum.GameKind_GouJi:
                publicAppointmentData = new GoujiAppointmentData(6);
                break;
            case GameKind_Enum.GameKind_HongZhong:
                publicAppointmentData = new HzMahjongAppointmentData(4);
                break;
            case GameKind_Enum.GameKind_Chess:
                publicAppointmentData = new ChessAppointmentData(2);
                break;
            default:
                publicAppointmentData = new AppointmentData(3);
                break;
        }
        return publicAppointmentData;
    }

    /// <summary>
    /// 播放UI动画
    /// </summary>
    /// <param name="animationName">动画名称</param>
    /// <param name="lifeTime">动画时间</param>
    /// <param name="parentTfm">动画挂载点</param>
    /// <param name="assetBundle">动画资源包</param>
    /// <param name="audioID">音效ID</param>
    public static GameObject PlayUIAnim(string animationName, float lifeTime, Transform parentTfm, AssetBundle assetBundle,int audioID = 0)    {        if(string.IsNullOrEmpty(animationName))
        {
            return null;
        }        if (parentTfm == null)
        {
            Debug.Log("动画挂载点不存在!!!");
            return null;
        }        if (assetBundle == null)
        {
            Debug.Log("动画资源加载失败!!!");
            return null;
        }        UnityEngine.Object obj = assetBundle.LoadAsset(animationName);        GameObject gameObj = (GameObject)GameMain.instantiate(obj);        gameObj.transform.SetParent(parentTfm, false);        DragonBones.UnityArmatureComponent animate = gameObj.GetComponentInChildren<DragonBones.UnityArmatureComponent>();        animate.animation.Play("newAnimation");        if (lifeTime > 0f)            GameObject.Destroy(gameObj, lifeTime);        if (audioID != 0)            CustomAudioDataManager.GetInstance().PlayAudio(audioID);        return gameObj;    }}[LuaCallCSharp]public class PermutationAndCombination<T>
{
    /// <summary>
    /// 交换两个变量
    /// </summary>
    /// <param name="a">变量1</param>
    /// <param name="b">变量2</param>
    public static void Swap(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }
    /// <summary>
    /// 递归算法求数组的组合(私有成员)
    /// </summary>
    /// <param name="list">返回的范型</param>
    /// <param name="t">所求数组</param>
    /// <param name="n">辅助变量</param>
    /// <param name="m">辅助变量</param>
    /// <param name="b">辅助数组</param>
    /// <param name="M">辅助变量M</param>
    private static void GetCombination(ref List<List<T>> list, List<T> t, int n, int m, int[] b, int M)
    {
        for (int i = n; i >= m; i--)
        {
            b[m - 1] = i - 1;
            if (m > 1)
            {
                GetCombination(ref list, t, i - 1, m - 1, b, M);
            }
            else
            {
                if (list == null)
                {
                    list = new List<List<T>>();
                }
                T[] temp = new T[M];
                for (int j = 0; j < b.Length; j++)
                {
                    temp[j] = t[b[j]];
                }
                list.Add(new List<T>(temp));
            }
        }
    }
    /// <summary>
    /// 递归算法求排列(私有成员)
    /// </summary>
    /// <param name="list">返回的列表</param>
    /// <param name="t">所求数组</param>
    /// <param name="startIndex">起始标号</param>
    /// <param name="endIndex">结束标号</param>
    private static void GetPermutation(ref List<List<T>> list, T[] t, int startIndex, int endIndex)
    {
        if (startIndex == endIndex)
        {
            if (list == null)
            {
                list = new List<List<T>>();
            }
            List<T> temp = new List<T>(t);
            list.Add(temp);
        }
        else
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                Swap(ref t[startIndex], ref t[i]);
                GetPermutation(ref list, t, startIndex + 1, endIndex);
                Swap(ref t[startIndex], ref t[i]);
            }
        }
    }
    /// <summary>
    /// 求从起始标号到结束标号的排列，其余元素不变
    /// </summary>
    /// <param name="t">所求数组</param>
    /// <param name="startIndex">起始标号</param>
    /// <param name="endIndex">结束标号</param>
    /// <returns>从起始标号到结束标号排列的范型</returns>
    public static List<List<T>> GetPermutation(T[] t, int startIndex, int endIndex)
    {
        if (startIndex < 0 || endIndex > t.Length - 1)
        {
            return null;
        }
        List<List<T>> list = new List<List<T>>();
        GetPermutation(ref list, t, startIndex, endIndex);
        return list;
    }
    /// <summary>
    /// 返回数组所有元素的全排列
    /// </summary>
    /// <param name="t">所求数组</param>
    /// <returns>全排列的范型</returns>
    public static List<List<T>> GetPermutation(List<T> t)
    {
        return GetPermutation(t.ToArray(), 0, t.Count - 1);
    }
    /// <summary>
    /// 求数组中n个元素的排列
    /// </summary>
    /// <param name="t">所求数组</param>
    /// <param name="n">元素个数</param>
    /// <returns>数组中n个元素的排列</returns>
    public static List<List<T>> GetPermutation(List<T> t, int n)
    {
        if (n > t.Count)
        {
            return null;
        }
        List<List<T>> list = new List<List<T>>();
        List<List<T>> c = GetCombination(t, n);
        for (int i = 0; i < c.Count; i++)
        {
            List<List<T>> l = new List<List<T>>();
            GetPermutation(ref l, c[i].ToArray(), 0, n - 1);
            list.AddRange(l);
        }
        return list;
    }
    /// <summary>
    /// 求数组中n个元素的组合
    /// </summary>
    /// <param name="t">所求数组</param>
    /// <param name="n">元素个数</param>
    /// <returns>数组中n个元素的组合的范型</returns>
    public static List<List<T>> GetCombination(List<T> t, int n)
    {
        if (t.Count < n)
        {
            return null;
        }
        int[] temp = new int[n];
        List<List<T>> list = new List<List<T>>();
        GetCombination(ref list, t, t.Count, n, temp, n);
        return list;
    }
}