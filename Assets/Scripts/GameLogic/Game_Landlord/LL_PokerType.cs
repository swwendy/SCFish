using DG.Tweening;
using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XLua;


[Hotfix]public class LL_PokerType{
    public static byte CurGrade = 2;//级牌（比A大的值）

    public static byte GetLaiziValue()
    {
        return (byte)(0x20 + CurGrade);
    }
    public static int GetPokerLogicIndex(byte poker, bool originValue = false)    {        int value = GameCommon.GetCardValue(poker);        if (value == 0)            return 54;        if (value < 0)            return 53;        int color = GameCommon.GetCardColor(poker);        if (!originValue && value == CurGrade)            return 49 + color;        if (value == 1)            return 45 + color;        return value * 4 - 11 + color;    }    public static int SortByOriginIndex(byte a, byte b)//小到大
    {        return GetPokerLogicIndex(a, true) - GetPokerLogicIndex(b, true);
    }    public static int SortByIndex(byte a, byte b)//小到大
    {        return GetPokerLogicIndex(a) - GetPokerLogicIndex(b);    }    public static int GetPokerLogicValue(byte poker, byte laizi = 0)    {        if (poker == laizi)            return -1;        int value = GameCommon.GetCardValue(poker);        if (value == 0)            return 17;        if (value < 0)            return 16;        if (value == CurGrade)            return 15;        if (value == 1)            return 14;        return value;    }    public static int SortByValue(byte a, byte b)//小到大
    {        return GetPokerLogicValue(a) - GetPokerLogicValue(b);    }    public static int GetPokerColorValue(byte poker)    {        int colorValue = poker;        int value = GameCommon.GetCardValue(poker);        if (value == CurGrade)
            colorValue = poker - value + 15;
        else if (value == 1)
            colorValue = poker + 13;
        return colorValue;    }    public static int SortByColor(byte a, byte b)//大到小
    {        return GetPokerColorValue(b) - GetPokerColorValue(a);    }
    public static void GetValueList(List<byte> srcList, out Dictionary<int, List<byte>> valueList, byte laizi = 0)    {        valueList = new Dictionary<int, List<byte>>();        List<byte> pokerList = new List<byte>();        List<byte> list = new List<byte>(srcList);        var groupNum = list.GroupBy(p => GetPokerLogicValue(p, laizi));
        foreach (var g in groupNum)
        {
            pokerList.Clear();
            foreach (var v in g)
            {
                pokerList.Add(v);
            }
            pokerList.Sort(SortByColor);
            valueList[g.Key] = new List<byte>(pokerList);
        }
        valueList = valueList.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
    }

    ////返回值 0：no -1:(no but have child flush) >0:yes
    //public delegate int IsDictType(Dictionary<int, List<byte>> valueLis, ref List<byte> childValidList, ref int biggest);

    //static int TestListType(IsDictType action, List<byte> list, ref List<byte> childValidList, ref int biggest)
    //{
    //    Dictionary<int, List<byte>> valueList;
    //    GetValueList(list, out valueList);
    //    return action(valueList, ref childValidList, ref biggest);
    //}

    //public static int TestListType(IsDictType action, Dictionary<int, List<byte>> valueList, ref int biggset)
    //{
    //    List<byte> child = null;
    //    return action(valueList, ref child, ref biggset);
    //}

    public static int IsPairs(Dictionary<int, List<byte>> vl, ref List<byte> childValidList, ref int biggest)
    {
        childValidList.Clear();

        Dictionary<int, List<byte>> valueList = new Dictionary<int, List<byte>>(vl);

        int laiziCount = 0;
        byte laizi = 0;
        if (valueList.ContainsKey(-1))
        {            laiziCount = valueList[-1].Count;
            laizi = valueList[-1][0];
            valueList.Remove(-1);
        }
        if (valueList.Count == 0)
        {            if (laiziCount != 2)
                return 0;

            for(int i = 0; i < laiziCount; i++)
                childValidList.Add(laizi);
            biggest = 15;
            return 1;
        }        else if (valueList.Count != 1)
            return 0;

        var first = valueList.First();
        if (first.Key > 15)
        {
            if (first.Value.Count != 2 || laiziCount != 0)
                return 0;
        }
        else if(first.Value.Count + laiziCount != 2)
            return 0;

        childValidList.AddRange(first.Value);
        for (int i = 0; i < laiziCount; i++)
            childValidList.Add(laizi);
        biggest = first.Key;
        return 1;
    }

    public static int IsThree(Dictionary<int, List<byte>> vl, ref List<byte> childValidList, ref int biggest)
    {
        childValidList.Clear();

        Dictionary<int, List<byte>> valueList = new Dictionary<int, List<byte>>(vl);

        int laiziCount = 0;
        byte laizi = 0;
        if (valueList.ContainsKey(-1))
        {            laiziCount = valueList[-1].Count;
            laizi = valueList[-1][0];
            valueList.Remove(-1);
        }
        if (valueList.Count == 0)
        {            if (laiziCount != 3)
                return 0;

            for (int i = 0; i < laiziCount; i++)
                childValidList.Add(laizi);
            biggest = 15;
            return 1;
        }        else if (valueList.Count != 1)
            return 0;

        var first = valueList.First();
        if (first.Key > 15)
        {
            if (first.Value.Count != 3 || laiziCount != 0)
                return 0;
        }
        else if (first.Value.Count + laiziCount != 3)
            return 0;

        childValidList.AddRange(first.Value);
        for (int i = 0; i < laiziCount; i++)
            childValidList.Add(laizi);
        biggest = first.Key;

        return 1;

    }

    public static int IsKingBlust(Dictionary<int, List<byte>> valueList, ref List<byte> childValidList, ref int biggest, int packNum = 1)
    {
        childValidList.Clear();

        if (valueList.Count != 2)
            return 0;

        foreach(var value in valueList)
        {
            if (value.Key < 16)
                return 0;

            if (value.Value.Count != packNum)
                return 0;

            childValidList.AddRange(value.Value);
        }

        biggest = 17;

        return 1;
    }

    public static int IsFour(Dictionary<int, List<byte>> valueList, ref List<byte> childValidList, ref int biggest)
    {
        childValidList.Clear();

        if (valueList.Count != 1)
            return 0;

        if (valueList.Values.First().Count != 4)
            return 0;

        biggest = valueList.Keys.First();
        childValidList.AddRange(valueList.Values.First());
        return 1;
    }

    public static int IsFourAndTwo(Dictionary<int, List<byte>> valueList, ref List<byte> childValidList, ref int biggest)
    {
        childValidList.Clear();

        if (valueList.Count < 2 || valueList.Count > 3)
            return 0;

        int count4 = 0, count2 = 0, count1 = 0;
        foreach (int i in valueList.Keys)        {
            if (valueList[i].Count == 1)
            {
                count1++;
                childValidList.AddRange(valueList[i]);
            }
            else if (valueList[i].Count == 2)
            {
                count2++;
                childValidList.AddRange(valueList[i]);
            }
            else if (valueList[i].Count == 4)
            {                biggest = i;                count4++;
                childValidList.InsertRange(0, valueList[i]);
            }            else
                return 0;
        }

        if (count4 != 1)
            return 0;

        if (count1 == 2 || count2 == 1)
            return 1;

        if (count2 == 2)
            return 2;

        return 0;
    }

    public static int IsFlush(Dictionary<int, List<byte>> vl, ref List<byte> validList, ref int biggest, bool bIncludeA2One = false, int maxNum = 0)
    {
        validList.Clear();

        Dictionary<int, List<byte>> valueList = new Dictionary<int, List<byte>>(vl);

        int laiziCount = 0;
        byte laizi = 0;
        if (valueList.ContainsKey(-1))
        {            laiziCount = valueList[-1].Count;
            laizi = valueList[-1][0];
            valueList.Remove(-1);
        }
        int len = valueList.Count + laiziCount;
        if (len < 5)
            return 0;

        if (maxNum > 0 && len > maxNum)
            return 0;

        if(bIncludeA2One)
        {
            if (valueList.ContainsKey(15))//改成用原始值
            {
                List<byte> temp = new List<byte>(valueList[15]);
                valueList.Remove(15);
                byte g = CurGrade;
                if (CurGrade == 1)
                    g = 14;
                valueList[g] = temp;
            }

            if (valueList.ContainsKey(14))
            {
                List<int> temp = new List<int>(valueList.Keys);
                temp.Remove(14);
                int find = temp.Find(s => s > len);
                if(find <= 0)//A变为1
                {
                    List<byte> temp1 = new List<byte>(valueList[14]);
                    valueList.Remove(14);
                    valueList[1] = temp1;
                }
            }
        }

        valueList = valueList.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);

        int value = 0;
        bool haveOther = false;
        int nowLaiziCount = laiziCount;
        foreach (int pokerValue in valueList.Keys)        {
            bool fail = false;
            if (pokerValue > 14)
                fail = true;
            else
            {
                if (value != 0)
                {
                    int offset = pokerValue - value - 1;
                    if (offset > nowLaiziCount)
                        fail = true;
                    else
                    {
                        nowLaiziCount -= offset;
                        for (int i = 0; i < offset; i++)
                            validList.Add(laizi);
                    }
                }
            }

            if (fail)
            {
                if (validList.Count < 5)
                {
                    validList.Clear();
                    biggest = 0;
                    return 0;
                }

                return -1;
            }

            value = pokerValue;
            validList.Add(valueList[value][0]);
            biggest = pokerValue;

            if (valueList[value].Count > 1)                haveOther = true;        }

        for (int i = 0; i < nowLaiziCount; i++)
        {            if (biggest < 14)            {
                validList.Add(laizi);
                biggest += 1;            }
            else
                validList.Insert(0, laizi);
        }
        if (haveOther)
            return -1;

        return 1;
    }

    public static int IsThreeAndOne(Dictionary<int, List<byte>> valueList, ref List<byte> childValidList, ref int biggest)
    {
        childValidList.Clear();

        if (valueList.Count != 2)
            return 0;

        int count3 = 0, count1 = 0;
        foreach (int i in valueList.Keys)        {
            if (valueList[i].Count == 3)
            {
                childValidList.InsertRange(0, valueList[i]);
                biggest = i;                count3++;
            }            else if (valueList[i].Count == 1)
            {
                count1++;
                childValidList.AddRange(valueList[i]);
            }
            else
                return 0;
        }

        if (count3 != 1 || count1 != 1)
            return 0;

        return 1;
    }

    public static int IsThreeAndTwo(Dictionary<int, List<byte>> vl, ref List<byte> childValidList, ref int biggest)
    {
        childValidList.Clear();
        int pokerNum = 0;        foreach (List<byte> poker in vl.Values)
            pokerNum += poker.Count;
        if (pokerNum != 5)            return 0;
        Dictionary<int, List<byte>> valueList = new Dictionary<int, List<byte>>(vl);

        int laiziCount = 0;
        byte laizi = 0;
        if (valueList.ContainsKey(-1))
        {            laiziCount = valueList[-1].Count;
            laizi = valueList[-1][0];
            valueList.Remove(-1);
        }        if (valueList.Count == 0 || valueList.Count > 2)            return 0;
        if(valueList.Count == 1)
        {
            var poker = valueList.First();
            if (poker.Key > 15 || poker.Value.Count > 3)
                return 0;

            if (poker.Value.Count == 3)
            {
                biggest = poker.Key;

                childValidList.AddRange(poker.Value);
                for (int i = 0; i < laiziCount; i++)
                    childValidList.Add(laizi);
            }
            else
            {
                biggest = 15;

                for (int i = 0; i < 3; i++)
                    childValidList.Add(laizi);
                laiziCount -= 3;
                childValidList.AddRange(poker.Value);
                for (int i = 0; i < laiziCount; i++)
                    childValidList.Add(laizi);
            }
        }        else        {
            if (valueList.ContainsKey(16) && valueList[16].Count < 2)
                return 0;
            if (valueList.ContainsKey(17) && valueList[17].Count < 2)
                return 0;

            valueList = valueList.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            List<List<byte>> listValue = valueList.Values.ToList();

            if (listValue[0].Count > 3 || listValue[1].Count > 3)
                return 0;

            List<int> listKey = valueList.Keys.ToList();

            if (listValue[1].Count == 3 || (listKey[1] < 16 && listValue[1].Count + laiziCount >= 3))
            {
                biggest = listKey[1];

                childValidList.AddRange(listValue[1]);
                for (int i = 0; i < 3 - listValue[1].Count; i++)
                {
                    childValidList.Add(laizi);
                    laiziCount -= 1;
                }
                childValidList.AddRange(listValue[0]);
                for (int i = 0; i < laiziCount; i++)
                    childValidList.Add(laizi);
            }
            else
            {
                biggest = listKey[0];

                childValidList.AddRange(listValue[0]);
                for (int i = 0; i < 3 - listValue[0].Count; i++)
                {
                    childValidList.Add(laizi);
                    laiziCount -= 1;
                }
                childValidList.AddRange(listValue[1]);
                for (int i = 0; i < laiziCount; i++)
                    childValidList.Add(laizi);
            }
        }
        return 1;
    }

    public static int IsPlaneWithOther(Dictionary<int, List<byte>> valueList, ref List<byte> childValidList, ref int biggest)
    {
        childValidList.Clear();

        if (valueList.Count < 2)
            return 0;

        int count1 = 0, count2 = 0, count3 = 0;
        int value = 0;
        List<List<int>> continueList = new List<List<int>>();
        List<int> temp = new List<int>();
        List<byte> allCards = new List<byte>();
        List<int> fourList = new List<int>();
        foreach (int i in valueList.Keys)        {
            allCards.AddRange(valueList[i]);

            if (valueList[i].Count >= 3)
            {                if (i < 15)//2以下
                {
                    if (value == 0 || i == (value + 1))
                    {
                        temp.Add(i);
                    }
                    else
                    {
                        if(temp.Count > 1)
                            continueList.Add(new List<int>(temp));
                        temp.Clear();
                        temp.Add(i);
                    }

                    value = i;
                }                if (valueList[i].Count == 3)                    count3++;
                else
                    fourList.Add(i);
            }            else if (valueList[i].Count == 1)
                count1++;
            else if (valueList[i].Count == 2)
                count2++;
            else
                return 0;
        }

        if(temp.Count > 1)
            continueList.Add(new List<int>(temp));

        foreach (List<int> list in continueList)
        {
            if (list.Count <= 1)
                continue;

            //单牌算
            if (list.Count == (count1 + count2 * 2 + count3 * 3 + fourList.Count * 4 - list.Count * 3))
            {
                biggest = list[list.Count - 1];

                byte card;
                foreach(int i in list)
                {
                    for(int j = 0; j < 3; j++)
                    {
                        card = valueList[i][j];
                        childValidList.Add(card);
                        allCards.Remove(card);
                    }
                }
                childValidList.AddRange(allCards);

                return 1;
            }

            if(fourList.Count > 0)//4个作单牌的情况
            {
                List<int> conList = new List<int>(list);
                for (int k = 0; k < fourList.Count; k++)
                {
                    if (fourList[k] == conList[0] || fourList[k] == conList[conList.Count -1])
                    {
                        conList.Remove(fourList[k]);
                        if (conList.Count < 2)
                            break;

                        if (conList.Count == (count1 + count2 * 2 + count3 * 3 + fourList.Count * 4 - conList.Count * 3))
                        {
                            biggest = conList[conList.Count - 1];

                            byte card;
                            foreach (int i in conList)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    card = valueList[i][j];
                                    childValidList.Add(card);
                                    allCards.Remove(card);
                                }
                            }
                            childValidList.AddRange(allCards);

                            return 1;
                        }

                    }
                }
            }

            //对牌算
            if (count1 != 0)
                return 0;

            if (count3 == list.Count && list.Count == (count2 + fourList.Count * 2))
            {
                biggest = list[list.Count - 1];

                byte card;
                foreach (int i in list)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        card = valueList[i][j];
                        childValidList.Add(card);
                        allCards.Remove(card);
                    }
                }
                childValidList.AddRange(allCards);

                return 2;
            }

            if (fourList.Count > 0)//4个作对牌的情况
            {
                List<int> conList = new List<int>(list);
                for (int k = 0; k < fourList.Count; k++)
                {
                    if (fourList[k] == conList[0] || fourList[k] == conList[conList.Count - 1])
                    {
                        conList.Remove(fourList[k]);
                        if (conList.Count < 2)
                            break;

                        if (count3 == conList.Count && conList.Count == (count2 + fourList.Count * 2))
                        {
                            biggest = conList[conList.Count - 1];

                            byte card;
                            foreach (int i in conList)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    card = valueList[i][j];
                                    childValidList.Add(card);
                                    allCards.Remove(card);
                                }
                            }
                            childValidList.AddRange(allCards);

                            return 2;
                        }
                    }
                }
            }
        }

        return 0;
    }

    public static int IsPlaneNoWith(Dictionary<int, List<byte>> vl, ref List<byte> childValidList, ref int biggest, bool bIncludeA2One = false, int maxNum = 0)
    {
        childValidList.Clear();

        if(maxNum > 0)
        {
            int pokerNum = 0;
            foreach (List<byte> poker in vl.Values)
                pokerNum += poker.Count;
            if (pokerNum != maxNum)
                return 0;        }
        Dictionary<int, List<byte>> valueList = new Dictionary<int, List<byte>>(vl);

        int laiziCount = 0;
        byte laizi = 0;
        if (valueList.ContainsKey(-1))
        {            laiziCount = valueList[-1].Count;
            laizi = valueList[-1][0];
            valueList.Remove(-1);
        }
        int len = valueList.Count + laiziCount;
        if (len < 2)
            return 0;

        if (bIncludeA2One)
        {
            if (valueList.ContainsKey(15))//改成用原始值
            {
                List<byte> temp = new List<byte>(valueList[15]);
                valueList.Remove(15);
                byte g = CurGrade;
                if (CurGrade == 1)
                    g = 14;
                valueList[g] = temp;
            }

            if (valueList.ContainsKey(14))
            {
                List<int> temp = new List<int>(valueList.Keys);
                temp.Remove(14);
                int find = temp.Find(s => s > len);
                if (find <= 0)//A变为1
                {
                    List<byte> temp1 = new List<byte>(valueList[14]);
                    valueList.Remove(14);
                    valueList[1] = temp1;
                }
            }
        }

        valueList = valueList.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);

        int value = 0;
        int nowLaiziCount = laiziCount;
        int num;
        foreach (int pokerValue in valueList.Keys)        {
            if (pokerValue > 14)
                return 0;

            if (valueList[pokerValue].Count + nowLaiziCount < 3 || valueList[pokerValue].Count > 3)
                return 0;

            if (value != 0)
            {
                int offset = pokerValue - value - 1;
                num = offset * 3;
                if (num > nowLaiziCount)
                    return 0;

                nowLaiziCount -= num;
                for (int i = 0; i < num; i++)
                    childValidList.Add(laizi);

                len += offset;
            }

            len++;
            value = pokerValue;
            biggest = pokerValue;

            num = 3 - valueList[pokerValue].Count;
            if (nowLaiziCount < num)
                return 0;

            childValidList.AddRange(valueList[pokerValue]);
            for (int i = 0; i < num; i++)
            {
                nowLaiziCount -= 1;
                childValidList.Add(laizi);
            }
        }

        if (nowLaiziCount % 3 != 0)
            return 0;

        for (int i = 0; i < nowLaiziCount; i++)
        {            if (biggest < 14)            {
                childValidList.Add(laizi);
                if (i % 2 == 2)
                    biggest += 1;
            }
            else
                childValidList.Insert(0, laizi);
        }
        return 1;
    }

    public static int IsSeriesPairs(Dictionary<int, List<byte>> vl, ref List<byte> childValidList, ref int biggest, bool bIncludeA2One = false, int maxNum = 0)
    {
        childValidList.Clear();

        if (maxNum > 0)
        {
            int pokerNum = 0;
            foreach (List<byte> poker in vl.Values)
                pokerNum += poker.Count;
            if (pokerNum != maxNum)
                return 0;        }
        Dictionary<int, List<byte>> valueList = new Dictionary<int, List<byte>>(vl);

        int laiziCount = 0;
        byte laizi = 0;
        if (valueList.ContainsKey(-1))
        {            laiziCount = valueList[-1].Count;
            laizi = valueList[-1][0];
            valueList.Remove(-1);
        }
        int len = valueList.Count + laiziCount;
        if (len < 3)
            return 0;

        if (bIncludeA2One)
        {
            if (valueList.ContainsKey(15))//改成用原始值
            {
                List<byte> temp = new List<byte>(valueList[15]);
                valueList.Remove(15);
                byte g = CurGrade;
                if (CurGrade == 1)
                    g = 14;
                valueList[g] = temp;
            }

            if (valueList.ContainsKey(14))
            {
                List<int> temp = new List<int>(valueList.Keys);
                temp.Remove(14);
                int find = temp.Find(s => s > len);
                if (find <= 0)//A变为1
                {
                    List<byte> temp1 = new List<byte>(valueList[14]);
                    valueList.Remove(14);
                    valueList[1] = temp1;
                }
            }
        }

        valueList = valueList.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);

        int value = 0;
        int nowLaiziCount = laiziCount;
        int num;
        foreach (int pokerValue in valueList.Keys)        {
            if (pokerValue > 14)
                return 0;

            if (valueList[pokerValue].Count + nowLaiziCount < 2 || valueList[pokerValue].Count > 2)
                return 0;

            if (value != 0)
            {
                int offset = pokerValue - value - 1;
                num = offset * 2;
                if (num > nowLaiziCount)
                    return 0;

                nowLaiziCount -= num;
                for (int i = 0; i < num; i++)
                    childValidList.Add(laizi);

                len += offset;
            }

            len++;
            value = pokerValue;
            biggest = pokerValue;

            num = 2 - valueList[pokerValue].Count;
            if (nowLaiziCount < num)
                return 0;

            childValidList.AddRange(valueList[pokerValue]);
            for (int i = 0; i < num; i++)
            {
                nowLaiziCount -= 1;
                childValidList.Add(laizi);
            }
        }

        if (nowLaiziCount % 2 != 0)
            return 0;

        for (int i = 0; i < nowLaiziCount; i++)
        {            if (biggest < 14)            {
                childValidList.Add(laizi);
                if (i % 2 == 1)
                    biggest += 1;
            }
            else
                childValidList.Insert(0, laizi);
        }
        return 1;
    }

    public static int IsNormalBomb(Dictionary<int, List<byte>> valueList, ref List<byte> childValidList, ref int biggest)
    {
        childValidList.Clear();

        int pokerNum = 0;
        foreach (List<byte> poker in valueList.Values)
            pokerNum += poker.Count;
        if (pokerNum < 4)
            return 0;
        if (valueList.Count == 1)
        {            var first = valueList.FirstOrDefault();            childValidList.AddRange(first.Value);            biggest = first.Key;            return 1;
        }
        if (valueList.Count == 2)
        {
            List<int> keys = new List<int>(valueList.Keys);
            if (keys[0] == 16 && keys[1] == 17)
            {                biggest = 17;                childValidList.AddRange(valueList[16]);                childValidList.AddRange(valueList[17]);                return 1;
            }
            if (keys[0] == -1 && keys[1] < 16)
            {                biggest = keys[1];                childValidList.AddRange(valueList[keys[0]]);                childValidList.AddRange(valueList[keys[1]]);                childValidList.Reverse();                return 1;
            }        }

        return 0;
    }

    public static int IsStraightFlush(Dictionary<int, List<byte>> valueList, ref List<byte> childValidList, ref int biggest, bool bIncludeA2One = false, int maxNum = 0)
    {
        int res = IsFlush(valueList, ref childValidList, ref biggest, bIncludeA2One, maxNum);
        if (res <= 0)
            return 0;

        int laiziCount = 0;
        byte laizi = 0;
        if (valueList.ContainsKey(-1))
        {            laiziCount = valueList[-1].Count;
            laizi = valueList[-1][0];
        }
        int color = -1;
        foreach(byte poker in childValidList)
        {
            if(poker != laizi)
            {
                if (color == -1)
                    color = GameCommon.GetCardColor(poker);
                else
                {
                    if (GameCommon.GetCardColor(poker) != color)
                        return 0;
                }
            }
        }

        return 1;
    }
}