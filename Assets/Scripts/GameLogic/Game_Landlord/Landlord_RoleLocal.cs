﻿using DG.Tweening;
using UnityEngine;

[Hotfix]
    GameObject m_NoBigTextObj;


        m_NoBigTextObj = m_InfoUI.parent.Find("PopUp_BG/Tip_BG/Tip_yaobuqi").gameObject;
        m_NoBigTextObj.SetActive(false);
    }

    void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_DEALPOKERFAILED, HandleNetMsg);
    }


        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    OnEnd();
        //    m_HavePokerList = new List<byte> { 0x03, 0x13, 0x23, 0x33, 0x04, 0x14,
        //        0x24, 0x34, 0x05, 0x15, 0x25, 0x35, 0x06, 0x16, 0x26, 0x36, 0x07,
        //        0x17, 0x27, 0x37};
        //    GameMain.SC(BeginPoker());
        //    OnAskDealPoker(true, new List<byte>(), LandPokerType_Enum.LandPokerType_Error, 10, 20, true);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    List<byte> list;
        //    GetPrePokers(out list);
        //    OnDealPoker(1, list.ToArray(), LandPokerType_Enum.LandPokerType_Error);
        //}

    }

        base.OnEnd();
    }
    }
    {
        if (m_bAnswered)
            return;

        List<byte> list;
        GetPrePokers(out list);

        List<byte> validList = new List<byte>();
        {
            {
                if (selectNum > unselectNum)
                {
                    if (SmartSelectCard(list, selectNum - unselectNum))
                    {
                        m_GameBase.OutPokerBtn.interactable = true;
                        return;
                    }
            }

        }

        m_GameBase.OutPokerBtn.interactable = true;
        {
        }
    {
        if(list == null || list.Count <= 5)
            return false;

        if (list.Count > 14)
        {
            Dictionary<int, List<byte>> valueList;
            LL_PokerType.GetValueList(list, out valueList);
            int biggest = 0;
            if (LL_PokerType.IsFlush(valueList, ref childList, ref biggest) != 0)
                return true;

            return false;
        }

        for (int i = list.Count - 1; i >= 5; i--)
        {
            List<List<byte>> ListCombination = PermutationAndCombination<byte>.GetCombination(list, i);
            {
                LandPokerType_Enum type = m_GameBase.GetPokersType(child, new List<byte>(new byte[i]), 
                    LandPokerType_Enum.LandPokerType_Error, ref childList);
                if (type != LandPokerType_Enum.LandPokerType_Error)
                    return true;
            }
        return false;
    }
        if (list.Count == 0 || CurPokerType == LandPokerType_Enum.LandPokerType_Single)
            return false;

        List<byte> childValidList = new List<byte>();

        if (CurPokerType == LandPokerType_Enum.LandPokerType_Error)//有牌权
        {
            if (addNum < list.Count)//已选出过牌了的情况不考虑
                return false;

            if (GetValidChildList(list, ref childValidList))
            {
                UpdateCards(false, childValidList);
                return true;
            }

            return false;
        }

        if (m_AblePokerArray.Count == 0)
            return false;

        Dictionary<int, List<byte>> valueList;
        LL_PokerType.GetValueList(list, out valueList);

        bool find;
        bool canMore;

        foreach (List<byte> ableList in m_AblePokerArray)
        {
            canMore = (CurPokerType == LandPokerType_Enum.LandPokerType_Flush);

            Dictionary<int, List<byte>> ableValueList;
            LL_PokerType.GetValueList(ableList, out ableValueList);

            List<byte> resList = new List<byte>(ableList);

            find = true;
            if (list.Count > ableList.Count)
            {
                if (!canMore)
                    continue;

                foreach (byte poker in ableList)
                {
                    if (!list.Contains(poker))
                    {
                        int value = LL_PokerType.GetPokerLogicValue(poker);
                        if (!valueList.ContainsKey(value))
                        {
                            find = false;
                            break;
                        }

                        resList.Remove(poker);
                        resList.Add(valueList[value][0]);
                    }
                }
            }
            else
            {
                List<byte> testList;
                int count;
                foreach (var array in valueList)
                {
                    if (!ableValueList.ContainsKey(array.Key) ||
                        array.Value.Count > ableValueList[array.Key].Count)
                    {
                        find = false;
                        break;
                    }

                    testList = new List<byte>(ableValueList[array.Key]);
                    foreach (byte poker in testList)
                        resList.Remove(poker);
                    foreach (byte poker in array.Value)
                    {
                        testList.Remove(poker);
                        resList.Add(poker);
                    }
                    count = ableValueList[array.Key].Count;
                    count -= array.Value.Count;
                    testList.Reverse();
                    for (int i = 0; i < count; i++)
                    {
                        resList.Add(testList[i]);
                    }
                }
            }

            if (find)
            {
                List<byte> validList = new List<byte>();
                LandPokerType_Enum type = m_GameBase.GetPokersType(resList, new List<byte>(), LandPokerType_Enum.LandPokerType_Error, ref validList);
                    return false;

                UpdateCards(false, resList);
                return true;
            }
        }

        return false;
                Rect rect, addRect;
                    {
                        {
                            addRect = Rect.MinMaxRect(lastRect.xMin, lastRect.yMax, rect.xMax, rect.yMax);
                            hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, addRect);
                        {
                            addRect = Rect.MinMaxRect(lastRect.xMin, rect.yMin, rect.xMax, lastRect.yMin);
                            hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, addRect);
                        }
                        rect = Rect.MinMaxRect(rect.xMin, rect.yMin, lastRect.xMin, rect.yMax);
                    }

        yield return null;

        UpdateCardsSpace();
    {
        if(m_GameBase.Bystander)
        {
            DebugLog.Log("bystander localPokerNum:" + num);
            m_HavePokerList.Clear();
            for (byte i = 0; i < num; i++)
                m_HavePokerList.Add(RoomInfo.NoSit);
        OnDealPoker(-1);
        UpdateCards(true);
        ShowCountdown(false);
    }
    {
        OnPokerNumChanged(0);
    }
    {
        RectTransform rootRtfm = m_GameBase.GameCanvas.transform.Find("Root") as RectTransform;
        float width = Screen.width + rootRtfm.sizeDelta.x;
        int count = m_HavePokerList.Count;
        float spacing = m_vCardLimit.z - width / count;
        spacing = spacing + spacing / count;
        if (spacing > 50f)
            spacing = -spacing;
        else
            spacing = -50f;
        spacing /= m_GameBase.GameCanvas.scaleFactor;
        m_HaveCards.GetComponent<GridLayoutGroup>().spacing = new Vector2(spacing, 0f);
            }
            if(selected == null)
            {
                for (int i = 0; i < m_HavePokerList.Count; i++)
                {
                    m_Cards[i].Selected(false);
                }
            }
            else
            {
                List<byte> clone = new List<byte>(selected);

                for (int i = 0; i < m_HavePokerList.Count; i++)
                {
                    m_Cards[i].Selected(clone.Contains(m_HavePokerList[i]));
                }
        if(m_GameBase.Bystander)
        {
            for (int i = 0; i < cards.Length; i++)
                m_HavePokerList.Add(RoomInfo.NoSit);
        }
            list = new List<byte>(cards);
            m_HavePokerList.AddRange(list);
            CustomAudioDataManager.GetInstance().PlayAudio(1088);

            UpdateCards(true, list);
        }
    {
        base.ShowLordIcon(bLord);

        UpdateCards();
    }
        {
            m_HavePokerList.RemoveRange(0, cards.Length);
        }
            foreach (byte card in cards)
                m_HavePokerList.Remove(card);
        }
                if (curType <= LandPokerType_Enum.LandPokerType_Three)
                {
                    curMinValue = LL_PokerType.GetPokerLogicValue(curPokerList[0]);
                    typeCount = (int)curType;
                    foreach (int pokerValue in myValueList.Keys)
                    {
                        if (pokerValue > curMinValue)
                        {
                            if (myValueList[pokerValue].Count >= typeCount && myValueList[pokerValue].Count != 4)
                            {
                                byte[] temp = new byte[typeCount];
                                myValueList[pokerValue].CopyTo(0, temp, 0, typeCount);

                                if (myValueList[pokerValue].Count == typeCount)
                                    m_AblePokerArray.Add(new List<byte>(temp));
                                else
                                    splitPokerArray.Add(new List<byte>(temp));
                            }
                        }
                    }
                }
                else if (curType == LandPokerType_Enum.LandPokerType_Flush || curType == LandPokerType_Enum.LandPokerType_SeriesPairs || curType == LandPokerType_Enum.LandPokerType_PlaneNoWith)
                {
                    curMinValue = LL_PokerType.GetPokerLogicValue(curPokerList[0]);
                    typeCount = 1;
                    if (curType == LandPokerType_Enum.LandPokerType_SeriesPairs)
                        typeCount = 2;
                    if (curType == LandPokerType_Enum.LandPokerType_PlaneNoWith)
                        typeCount = 3;

                    typeNum = curPokerList.Count / typeCount;
                    foreach (int pokerValue in myValueList.Keys)
                    {
                        if (pokerValue + typeNum > 15)//最大到A
                            break;

                        if (pokerValue > curMinValue)
                        {
                            pokerList.Clear();
                            bool find = true;
                            List<byte> andPokerList = new List<byte>();
                            for (int i = pokerValue; i < pokerValue + typeNum; i++)
                            {
                                if (!myValueList.ContainsKey(i)
                                    || myValueList[i].Count < typeCount)
                                {
                                    find = false;
                                    break;
                                }

                                byte[] temp = new byte[typeCount];
                                myValueList[i].CopyTo(0, temp, 0, typeCount);
                                if (myValueList[i].Count == typeCount)
                                    pokerList.AddRange(temp);
                                andPokerList.AddRange(temp);
                            }

                            if (find)
                            {
                                if (pokerList.Count == curPokerList.Count)
                                    m_AblePokerArray.Add(new List<byte>(pokerList));
                                else
                                    splitPokerArray.Add(andPokerList);
                            }
                        }
                    }
                }
                else if (curType == LandPokerType_Enum.LandPokerType_ThreeAndOne || curType == LandPokerType_Enum.LandPokerType_ThreeAndTwo
                    || curType == LandPokerType_Enum.LandPokerType_FourAndTwoSingle || curType == LandPokerType_Enum.LandPokerType_FourAndTwoPair)
                {
                    typeCount = 3;
                    if (curType == LandPokerType_Enum.LandPokerType_FourAndTwoSingle || curType == LandPokerType_Enum.LandPokerType_FourAndTwoPair)
                        typeCount = 4;
                    int andTypeCount = 1;
                    if (curType == LandPokerType_Enum.LandPokerType_ThreeAndTwo || curType == LandPokerType_Enum.LandPokerType_FourAndTwoPair)
                        andTypeCount = 2;
                    int andCount = 1;
                    if (curType == LandPokerType_Enum.LandPokerType_FourAndTwoSingle || curType == LandPokerType_Enum.LandPokerType_FourAndTwoPair)
                        andCount = 2;

                    foreach (int pokerValue in curValueList.Keys)
                    {
                        if (curValueList[pokerValue].Count == typeCount)
                        {
                            curMinValue = pokerValue;
                            break;
                        }
                    }

                    foreach (int pokerValue in myValueList.Keys)
                    {
                        if (pokerValue > curMinValue)
                        {
                            pokerList.Clear();
                            if (myValueList[pokerValue].Count == typeCount)
                            {
                                pokerList.AddRange(myValueList[pokerValue]);

                                Dictionary<int, List<byte>> testValueList = new Dictionary<int, List<byte>>();

                                foreach (int v in myValueList.Keys)
                                {
                                    if (v != pokerValue && myValueList[v].Count >= andTypeCount)
                                    {
                                        byte[] temp1 = new byte[andTypeCount];
                                        myValueList[v].CopyTo(0, temp1, 0, andTypeCount);
                                        List<byte> andPokerList = new List<byte>(temp1);
                                        testValueList.Add(v, andPokerList);
                                    }
                                }

                                if (testValueList.Count >= andCount)
                                {
                                    List<int> testList = new List<int>(testValueList.Keys);
                                    bool bSplit;
                                    for (int i = 0; i < testList.Count; i++)
                                    {
                                        bSplit = false;
                                        List<byte> temp = new List<byte>(pokerList);
                                        for (int j = i; j < i + andCount && j < testList.Count; j++)
                                        {
                                            if (myValueList[testList[j]].Count > andTypeCount)
                                                bSplit = true;
                                            temp.AddRange(testValueList[testList[j]]);
                                        }

                                        if (temp.Count == curPokerList.Count)
                                        {
                                            if (bSplit)
                                                splitPokerArray.Add(temp);
                                            else
                                                m_AblePokerArray.Add(temp);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                else if (curType == LandPokerType_Enum.LandPokerType_PlaneWithOherSingle || curType == LandPokerType_Enum.LandPokerType_PlaneWithOherPair)
                {
                    typeCount = 3;
                    int andTypeCount = 1;
                    if (curType == LandPokerType_Enum.LandPokerType_PlaneWithOherPair)
                        andTypeCount = 2;

                    foreach (int pokerValue in curValueList.Keys)
                    {
                        if (curValueList[pokerValue].Count == typeCount)
                        {
                            if (curMinValue == 0)
                                curMinValue = pokerValue;
                            typeNum++;
                        }
                    }

                    foreach (int pokerValue in myValueList.Keys)
                    {
                        if (pokerValue + typeNum > 15)//最大到A
                            break;

                        if (pokerValue > curMinValue)
                        {
                            pokerList.Clear();
                            bool find = true;
                            bool bSplit = false;
                            for (int i = pokerValue; i < pokerValue + typeNum; i++)
                            {
                                if (!myValueList.ContainsKey(i) || myValueList[i].Count < typeCount)
                                {
                                    find = false;
                                    break;
                                }

                                byte[] temp = new byte[typeCount];
                                myValueList[i].CopyTo(0, temp, 0, typeCount);
                                if (myValueList[i].Count != typeCount)
                                    bSplit = true;
                                pokerList.AddRange(temp);
                            }

                            if (find)
                            {
                                List<byte[]> testValueList = new List<byte[]>();
                                foreach (int v in myValueList.Keys)
                                {
                                    if ((v < pokerValue || v >= (pokerValue + typeNum)) && myValueList[v].Count >= andTypeCount)
                                    {
                                        byte[] temp= myValueList[v].ToArray();
                                        var result = temp.Select(x => new byte[] { x });
                                        for (int i = 0; i < andTypeCount - 1; i++)
                                        {
                                            result = result.SelectMany(x => temp.Where(y => y.CompareTo(x.First()) < 0).Select(y => new byte[] { y }.Concat(x).ToArray()));
                                        }
                                        foreach (var item in result)
                                        {
                                            testValueList.Add(item);
                                        }
                                }

                                byte[][] temp1 = testValueList.ToArray();
                                var result1 = temp1.Select(x => new byte[][] { x });
                                for (int i = 0; i < typeNum - 1; i++)
                                {
                                    result1 = result1.SelectMany(x => temp1.Where(y => y.First().CompareTo(x.First().First()) < 0).Select(y => new byte[][] { y }.Concat(x).ToArray()));
                                }

                                bool bSplit1;
                                foreach (var item in result1)
                                {
                                    List<byte> temp = new List<byte>(pokerList);
                                    bSplit1 = false;

                                    foreach (byte[] list in item)
                                    {
                                        temp.AddRange(list);

                                        if (!bSplit)
                                        {
                                            foreach(byte poker in list)
                                            {
                                                if (myValueList[LL_PokerType.GetPokerLogicValue(poker)].Count > andTypeCount)
                                                {
                                                    break;
                                                }
                                        }
                                    }

                                    if (bSplit1 || bSplit)
                                        splitPokerArray.Add(temp);
                                    else
                                        m_AblePokerArray.Add(temp);
                                }
                            }
                        }
                    }
                }
            }

            m_PlayPokerList.Clear();
            {
                m_NoBigTextObj.SetActive(true);
            }
    }
                    btn.interactable = false;
                    GameMain.WaitForCall(0.5f, () => btn.interactable = true);

                m_GameBase.OutPokerBtn.interactable = true;
            {
                List<byte> list;
                GetPrePokers(out list);
                if (list.Count == 0)
                    OnDealPokerFailed();
                else
                {
                    UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_CM_ASKDEALPOKER);
                    msg.Add(GameMain.hall_.GetPlayerId());
                    msg.Add((byte)list.Count);
                    foreach (byte card in list)
                        msg.Add(card);
                    HallMain.SendMsgToRoomSer(msg);

                    if (btn != null)
                        btn.interactable = false;

                    m_bAnswered = true;
                }
            }
            else
            {
                UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_CM_ASKDEALPOKER);
                msg.Add(GameMain.hall_.GetPlayerId());
                msg.Add((byte)0);
                HallMain.SendMsgToRoomSer(msg);

                if (btn != null)
                    btn.interactable = false;

                m_bAnswered = true;
            }
        }

        {
            m_HavePokerList.Clear();
            m_HavePokerList.AddRange(cards);
            OnDealPoker(-1);
            UpdateCards(true, null, "_big");
        }
    }

    protected override void OnPokerWarning()
    {
    }

    void OnClickBackground(EventTriggerType eventtype, object message, PointerEventData eventData)

    bool HandleNetMsg(uint _msgType, UMessage _ms)
    {
        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;
        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_DEALPOKERFAILED:
                {
                    byte nState = _ms.ReadByte();
                break;

            default:
                break;
        }

        return true;
    }

    void OnDealPokerFailed(byte nState = 0)//0:没有选牌 1：不在房间 2:不在出牌状态 3：没轮到 4：牌里有你没有的 5：牌型不对
    {
        {
            UpdateCards();
        }