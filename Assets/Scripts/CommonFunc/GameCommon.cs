﻿/**





using UnityEngine;
using System.Net;
using System.Linq;

[LuaCallCSharp]

    // long --> DateTime
    public static DateTime ConvertLongToDateTime(double d)
        //long lTime = long.Parse(d.ToString());
        //TimeSpan toNow = new TimeSpan(lTime);
        DateTime dtResult = dtStart.AddSeconds(d);


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
        #region 扑克牌通用




        public static int GetCardColor(byte card)
        //收件人地址
        mail.To.Add(new MailAddress("joybiggamer@sina.com"));
        //发件人地址
        mail.From = new MailAddress("joybiggamer@sina.com");
        //这个可以不指定
        //mail.Sender = new System.Net.Mail.MailAddress("xxx@sina.com", "SenderName");

        mail.Subject = "PhoenixEgg dump strack log";
        //mm.IsBodyHtml = true;
        mail.Priority = MailPriority.High; // 设置发送邮件的优先级
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
        {

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
    public static void  GetAppointmentRuleTextData(ref string RuleTextData,GameKind_Enum GameID)
            return;
                RuleTextData = "打" + data.playtimes.ToString() + "局 不封顶";
        else if (GameID == GameKind_Enum.GameKind_GuanDan)

            else
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
        }

    /// <summary>
    /// 创建约据房间数据
    /// </summary>
    /// <param name="GameID"></param>
    /// <returns></returns>
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
    public static GameObject PlayUIAnim(string animationName, float lifeTime, Transform parentTfm, AssetBundle assetBundle,int audioID = 0)
        {
            return null;
        }
        {
            Debug.Log("动画挂载点不存在!!!");
            return null;
        }
        {
            Debug.Log("动画资源加载失败!!!");
            return null;
        }
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