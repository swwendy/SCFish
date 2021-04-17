using System.Collections.Generic;using UnityEngine;using USocket.Messages;using System.IO;using XLua;using System.Linq;

[Hotfix]public class Mahjong_Data{
    public List<Dictionary<int, byte>>[] m_JudgeTable = new List<Dictionary<int, byte>>[2];    public List<Dictionary<int, byte>>[] m_JudgeTable_Eye = new List<Dictionary<int, byte>>[2];    public Dictionary<GameKind_Enum, Dictionary<int, string>> m_TextData = new Dictionary<GameKind_Enum, Dictionary<int, string>>();    Mahjong_Data()    {        m_TextData[GameKind_Enum.GameKind_Mahjong] = new Dictionary<int, string>();
        m_TextData[GameKind_Enum.GameKind_HongZhong] = new Dictionary<int, string>();        ReadMahjongData();    }    public static Mahjong_Data GetInstance()    {        if (instance == null)            instance = new Mahjong_Data();        return instance;    }    static Mahjong_Data instance;    public void ReadData(UMessage msg)
    {
    }    void ReadMahjongData()
    {
        List<string[]> strList;        int j;        int index;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, "Mahjong_textCsv", out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            j = 0;
            int.TryParse(strList[i][j++], out index);            m_TextData[GameKind_Enum.GameKind_Mahjong][index] = strList[i][j++];            m_TextData[GameKind_Enum.GameKind_HongZhong][index] = strList[i][j++];
        }

        for(int k = 0; k < m_JudgeTable.Length; k++)
            m_JudgeTable[k] = new List<Dictionary<int, byte>>();
        for (int k = 0; k < m_JudgeTable.Length; k++)
            m_JudgeTable_Eye[k] = new List<Dictionary<int, byte>>();

        for (int k = 0; k < 6; k++)
        {
            CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, k + "_table", out strList);
            columnCount = strList.Count;
            if (columnCount == 0)
                break;

            Dictionary<int, byte> dict = new Dictionary<int, byte>();
            for (int i = 0; i < columnCount; i++)
            {
                int.TryParse(strList[i][0], out index);                dict[index] = 0;
            }
            m_JudgeTable[0].Add(dict);

            CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, k + "_table_eye", out strList);
            columnCount = strList.Count;
            if (columnCount == 0)
                continue;

            dict = new Dictionary<int, byte>();
            for (int i = 0; i < columnCount; i++)
            {
                int.TryParse(strList[i][0], out index);                dict[index] = 0;
            }
            m_JudgeTable_Eye[0].Add(dict);

            CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, k + "_table_feng", out strList);
            columnCount = strList.Count;
            if (columnCount == 0)
                continue;

            dict = new Dictionary<int, byte>();
            for (int i = 0; i < columnCount; i++)
            {
                int.TryParse(strList[i][0], out index);                dict[index] = 0;
            }
            m_JudgeTable[1].Add(dict);

            CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, k + "_table_feng_eye", out strList);
            columnCount = strList.Count;
            if (columnCount == 0)
                continue;

            dict = new Dictionary<int, byte>();
            for (int i = 0; i < columnCount; i++)
            {
                int.TryParse(strList[i][0], out index);                dict[index] = 0;
            }
            m_JudgeTable_Eye[1].Add(dict);
        }    }}[Hotfix]public class JudgeTile
{
    public static readonly byte[] MahjongTiles = {
    0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09,   // 条
	0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19,   // 万
	0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29,   // 饼
	0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,			    // 风牌(东南西北中发白)
	0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48,			// 花牌(春夏秋冬梅兰竹菊)
    };

    public static int GetTileSuit(byte tile)
    {
        int type = tile >> 4;        return type;    }

    public static int GetTileValue(byte tile)
    {
        int value = tile & 0x0F;        return value;    }

    static int AnalyseTiles(List<byte> tiles, byte suit)
    {
        int res = 0;
        List<byte> list;
        List<byte> checkList = new List<byte>(MahjongTiles);
        checkList = checkList.FindAll(s => GetTileSuit(s) == suit);

        foreach (byte i in checkList)
        {            list = tiles.FindAll(s => s == i);
            res = res * 10 + list.Count;
        }        //去掉后面0        if(res != 0)        {
            while (res % 10 == 0)
                res /= 10;        }
        return res;
    }

    public static int CheckSevenPair(List<byte> handTiles, bool tingOrHu, out HashSet<byte> tingTile, byte hun = 0)
    {
        tingTile = null;

        if (handTiles.Count < 13)
            return -1;

        List<byte> tiles = new List<byte>(handTiles);
        tiles.RemoveAll(s => s == hun);
        int hunNum = handTiles.Count - tiles.Count;

        int[] count = new int[5];
        for (int i = 1; i < 5; i++)
        {
            count[i] = tiles.GroupBy(p => p).Where(g => g.Count() == i).Count();
        }
        int pairCount = count[2] + count[3] + 2 * count[4];
        if(count[1] > 0 && 0 < hunNum)
        {            if(count[1] <= hunNum)
            {
                pairCount += count[1];
                hunNum -= count[1];
            }else
            {
                pairCount += hunNum;
                hunNum = 0;
            }        }

        if (count[3] > 0 && 0 <= hunNum)
        {            if (count[3] <= hunNum)
            {
                pairCount += count[3];
                count[4] += count[3];
                hunNum -= count[3];
            }else
            {
                pairCount += hunNum;
                count[4] += hunNum;
                hunNum = 0;
            }        }        int duiCount = (hunNum / 2);        pairCount += duiCount;
        count[4] += duiCount;
        if (count[4] > 3)//最多3个
            count[4] = 3;

        if (tingOrHu)//查听
        {
            if (pairCount >= 6)
            {
                if (pairCount == 7)
                    tingTile = new HashSet<byte>(tiles);
                else
                {
                    var group = tiles.GroupBy(p => p).Where(g => (g.Count() == 1 || g.Count() == 3));
                    tingTile = new HashSet<byte>();
                    foreach (var g in group)
                    {
                        tingTile.Add(g.Key);
                    }                }
                if (hun > 0)
                    tingTile.Add(hun);

                return count[4];
            }
        }
        else if (pairCount == 7)
            return count[4];


        return -1;
    }

    public static bool CheckQingYiSe(List<byte> tiles, List<PongKongData> pkd, byte hun = 0)
    {
        List<byte> handTiles = new List<byte>(tiles);
        handTiles.RemoveAll(s => s == hun);

        int suit = GetTileSuit(handTiles[0]);
        foreach(byte tile in handTiles)
        {
            if (suit != GetTileSuit(tile))
                return false;
        }

        if (pkd == null)
            return true;

        foreach(PongKongData d in pkd)
        {
            if (suit != GetTileSuit(d.value))
                return false;
        }

        return true;
    }

    public static bool CheckDuiDuiHu(List<byte> tiles, List<PongKongData> pkd, byte hun = 0)
    {
        if (tiles.Count % 3 != 2)
            return false;

        PongKongData data =  pkd.Find(pkdData => pkdData.pkt == PongKongType.ePKT_Chi);
        if(data != null)
            return false;

        List<byte> handTiles = new List<byte>(tiles);
        int hunNum = handTiles.RemoveAll(s => s == hun);

        int[] count = new int[5];
        for (int i = 1; i < 5; i++)
        {
            count[i] = handTiles.GroupBy(p => p).Where(g => g.Count() == i).Count();
        }
        int hunLeft = hunNum , need = 0;
        bool havePair = false;
        if (count[2] > 0)
        {
            need = count[2] - 1;
            if (hunLeft < need)
                return false;

            hunLeft -= need;
            havePair = true;
        }

        if (havePair)
        {
            need = (count[1] + count[4]) * 2;
            if (hunLeft < need)
                return false;
        }
        else
        {
            if (count[1] > 0)
            {
                need = count[1] * 2 - 1;
                if (hunLeft < need)
                    return false;

                havePair = true;
            }

            if (count[4] > 0)
            {
                need = count[4] * 2;
                if (!havePair)
                    need -= 1;

                if (hunLeft < need)
                    return false;

                havePair = true;
            }

            if (!havePair)
            {
                if (hunLeft < 2)
                    return false;
            }
        }

        return true;
    }

    public static bool CheckJinGouDiao(List<byte> handTiles, List<PongKongData> pkd)
    {
        return handTiles.Count == 2;
    }

    public static int GetGangNum(List<PongKongData> pkd)
    {
        if (pkd == null)
            return 0;

        List<PongKongData> list = pkd.FindAll(s => s.pkt == PongKongType.ePKT_Kong_Concealed
                                                || s.pkt == PongKongType.ePKT_Kong_Exposed
                                                || s.pkt == PongKongType.ePKT_Pong2Kong); 
        return list.Count;
    }

    public static bool CheckPrepareTing(List<byte> handTiles, byte lack, out Dictionary<byte, HashSet<byte>> tingDict, bool check7pair = true, byte suitCount = 3, byte hun = 0)
    {
        tingDict = null;

        if (handTiles.Count % 3 != 2)
            return false;

        List<byte> suitList = handTiles.FindAll(s => GetTileSuit(s) >= suitCount);        if (suitList.Count > 0)            return false; 
       List<byte> list;

        bool ting = false;

        var group = handTiles.GroupBy(p => p).Where(g => g.Count() >= 1);
        foreach (var g in group)
        {
            byte tile = g.Key;

            list = new List<byte>(handTiles);
            list.Remove(tile);

            HashSet<byte> tileHu;
            if(CheckTing(list, lack, out tileHu, check7pair, suitCount, hun))
            {
                ting = true;

                if (tingDict == null)
                    tingDict = new Dictionary<byte, HashSet<byte>>();
                tingDict[tile] = tileHu;
            }
        }

        return ting;
    }

    public static bool CheckTing(List<byte> list, byte lack, out HashSet<byte> tileHu, bool check7pair, byte suitCount, byte hun)
    {
        tileHu = null;

        if (list.Count % 3 != 1)
            return false;

        if (lack != RoomInfo.NoSit)
        {            List<byte> lackList = list.FindAll(s => GetTileSuit(s) == lack);            if (lackList.Count > 0)
                return false;        }

        bool ting = false;

        if(check7pair)
        {
            if (CheckSevenPair(list, true, out tileHu, hun) >= 0)//检测可以听"七对"的情况
                ting = true;        }
        HashSet<byte> nortileHu;
        if (CheckNormalTing(list, lack, out nortileHu, suitCount, hun))
        {
            ting = true;

            if(nortileHu != null)
            {
                if (tileHu == null)
                    tileHu = new HashSet<byte>();
                tileHu.UnionWith(nortileHu);            }        }

        return ting;
    }

    public static bool CheckNormalHu(List<byte> tiles, List<byte> suits, int hunNum = 0, bool bHasPair = true)
    {
        int num = tiles.Count + hunNum + (bHasPair ? 0 : 2);
        if (num % 3 != 2)
            return false;

        int need, leftHun;

        if(bHasPair)
        {
            foreach (byte suit in suits)
            {
                need = IsHuPairSuit(tiles, suit, hunNum);
                if (need < 0)
                    continue;

                leftHun = hunNum - need;

                bool suc = true;
                List<byte> otherSuit = new List<byte>(suits);
                otherSuit.Remove(suit);
                foreach (byte ot in otherSuit)
                {
                    need = IsHuNoPairSuit(tiles, ot, leftHun);
                    if (need < 0)
                    {
                        suc = false;
                        break;
                    }

                    leftHun -= need;
                }

                if (suc)
                    return true;
            }
        }
        else
        {
            leftHun = hunNum;
            foreach (byte suit in suits)
            {
                need = IsHuNoPairSuit(tiles, suit, leftHun);
                if (need < 0)
                    return false;

                leftHun -= need;
            }

            return true;
        }

        return false;
    }

    //判断是不是可以胡并含将对的门，返回值 -1:不是 >=0:需要用几个混
    static int IsHuPairSuit(List<byte> tiles, byte suit, int hunNum)
    {
        int res = -1;
        List<byte> suitList = tiles.FindAll(s => GetTileSuit(s) == suit);
        int yu = suitList.Count % 3;
        int need = 2 - yu;
        int analy = 0; 

        while (hunNum >= need)
        {
            if(analy == 0)
                analy = AnalyseTiles(suitList, suit);

            if(analy == 0)
            {
                res = need;
                break;
            }

            if (Mahjong_Data.GetInstance().m_JudgeTable_Eye[suit / 3][need].ContainsKey(analy))
            {
                res = need;
                break;
            }

            need += 3;
        }

        return res;
    }

    //判断是不是可以胡并不含将对的门，返回值 -1:不是 >=0:需要用几个混
    static int IsHuNoPairSuit(List<byte> tiles, byte suit, int hunNum)
    {
        int res = -1;

        List<byte> suitList = tiles.FindAll(s => GetTileSuit(s) == suit);
        int yu = suitList.Count % 3;
        int need = (3 - yu) % 3;
        int analy = 0;

        while (hunNum >= need)
        {
            if (analy == 0)
                analy = AnalyseTiles(suitList, suit);

            if(analy == 0)
            {
                res = need;
                break;
            }

            if (Mahjong_Data.GetInstance().m_JudgeTable[suit / 3][need].ContainsKey(analy))
            {
                res = need;
                break;
            }

            need += 3;
        }

        return res;
    }

    public static bool CheckNormalTing(List<byte> list, byte lack, out HashSet<byte> tileHu, byte suitCount, byte hun)
    {
        tileHu = null;

        if (list.FindAll(s =>  s == 0).Count > 0)
            return false;

        if (list.Count % 3 != 1)
            return false;

        if (lack != RoomInfo.NoSit)
        {            List<byte> lackList = list.FindAll(s => GetTileSuit(s) == lack);            if (lackList.Count > 0)
                return false;        }

        List<byte> checkList = new List<byte>(MahjongTiles);
        checkList.RemoveAll(s => GetTileSuit(s) >= suitCount);

        List<byte> suits = new List<byte>();
        for (byte i = 0; i < suitCount; i++)
            suits.Add(i);

        if (lack != RoomInfo.NoSit)
        {            checkList.RemoveAll(s => GetTileSuit(s) == lack);
            suits.Remove(lack);
        }

        List<byte> tiles = new List<byte>(list);
        tiles.RemoveAll(s => s == hun);        checkList.Remove(hun);        int hunNum = list.Count - tiles.Count;

        bool ting = false;
        int need, leftHun;

        foreach (byte suit in suits)
        {
            need = IsHuPairSuit(tiles, suit, hunNum + 1);//多加1个混测试是否听
            if (need < 0)
                continue;

            leftHun = hunNum + 1 - need;

            bool suc = true;
            List<byte> otherSuit = new List<byte>(suits);
            otherSuit.Remove(suit);
            foreach (byte ot in otherSuit)
            {
                need = IsHuNoPairSuit(tiles, ot, leftHun);
                if(need < 0)
                {
                    suc = false;
                    break;
                }

                leftHun -= need;
            }

            if (suc)
            {
                ting = true;

                foreach (byte value in checkList)
                {
                    suc = true;
                    tiles.Add(value);
                    if (CheckNormalHu(tiles, suits, hunNum))
                    {
                        if (tileHu == null)
                            tileHu = new HashSet<byte>();

                        tileHu.Add(value);
                    }
                    tiles.Remove(value);
                }
            }
        }

        if (ting && hun > 0)
        {
            if (tileHu == null)
                tileHu = new HashSet<byte>();
            tileHu.Add(hun);
        }                   return ting;
    }
}