using DG.Tweening;using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;

[Hotfix]public class Landlord_RoleLocal : Landlord_Role{    List<List<byte>> m_AblePokerArray = new List<List<byte>>();    byte m_TipPokerIndex = 0;    bool m_bAnswered = true;    bool m_bDrag = false;    Vector2 m_vStartPos;    Vector3 m_vCardLimit;    List<PlayCard> m_Cards = new List<PlayCard>();
    GameObject m_NoBigTextObj;
    HashSet<byte> m_setPreCards = new HashSet<byte>();    public Landlord_RoleLocal(CGame_LandLords game, byte index)        : base(game, index)    {        RectTransform tfm;        for (int i = 0; i < m_HaveCards.childCount; i++)        {            tfm = m_HaveCards.Find("Poker_Choupai_Icon_" + i) as RectTransform;            PlayCard card = tfm.gameObject.AddComponent<PlayCard>();            card.Init();            m_Cards.Add(card);            int temp = i;            XPointEvent.AutoAddListener(tfm.gameObject, OnClickCards, temp);        }        if(m_Cards.Count > 1)        {            Rect rect = GameFunction.GetSpaceRect(m_GameBase.GameCanvas, m_Cards[0].transform as RectTransform, m_GameBase.GameCanvas.worldCamera);            m_vCardLimit = new Vector3(rect.yMin, rect.yMax + rect.height * 0.1f, rect.width);        }        XPointEvent.AutoAddListener(m_GameBase.MainUITfm.Find("UiRootBG").gameObject, OnClickBackground, null);

        m_NoBigTextObj = m_InfoUI.parent.Find("PopUp_BG/Tip_BG/Tip_yaobuqi").gameObject;
        m_NoBigTextObj.SetActive(false);        InitMsgHandle();
    }

    void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_DEALPOKERFAILED, HandleNetMsg);
    }
    public override void Init()    {        base.Init();    }    public override void OnTick()    {        base.OnTick();        UpdatePreCard();

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
    public override void OnEnd()    {
        base.OnEnd();        foreach (PlayCard pc in m_Cards)        {            pc.Init();        }        m_setPreCards.Clear();        m_AblePokerArray.Clear();        m_TipPokerIndex = 0;        m_bAnswered = true;        m_NoBigTextObj.SetActive(false);
    }    void AddOrRemovePreCard(byte index, bool bAdd)    {        if (bAdd)            m_setPreCards.Add(index);        else        {            if (!m_setPreCards.Contains(index))                return;            m_setPreCards.Remove(index);        }        PlayCard card = m_Cards[index];        card.Masked = bAdd;        m_Cards[index] = card;    }    void OnEndDrag()    {        int selectNum = 0, unselectNum = 0;        foreach (byte id in m_setPreCards)        {            PlayCard card = m_Cards[id];            card.Selected(!card.Selected());            card.Masked = false;            m_Cards[id] = card;            if (card.Selected())                selectNum++;            else                unselectNum++;        }        m_setPreCards.Clear();        m_bDrag = false;        CanOutPoker(selectNum, unselectNum);
    }    void CanOutPoker(int selectNum = 0, int unselectNum = 0)
    {
        if (m_bAnswered)
            return;

        List<byte> list;
        GetPrePokers(out list);

        List<byte> validList = new List<byte>();        LandPokerType_Enum res = m_GameBase.GetPokersType(list, m_PlayPokerList, CurPokerType, ref validList);        if (res == LandPokerType_Enum.LandPokerType_Error)
        {            if (!m_GameBase.OutPokerBtn.interactable)//如果之前已经选好了就不再智能选取了
            {
                if (selectNum > unselectNum)
                {
                    if (SmartSelectCard(list, selectNum - unselectNum))
                    {
                        m_GameBase.OutPokerBtn.interactable = true;
                        return;
                    }                }
            }
            m_GameBase.OutPokerBtn.interactable = false;            return;
        }

        m_GameBase.OutPokerBtn.interactable = true;        if (validList.Count > 0)
        {            if (selectNum > validList.Count)                UpdateCards(false, validList);            else if(list.Count != validList.Count)                m_GameBase.OutPokerBtn.interactable = false;
        }    }    bool GetValidChildList(List<byte> list, ref List<byte> childList)
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
            List<List<byte>> ListCombination = PermutationAndCombination<byte>.GetCombination(list, i);            foreach(List<byte> child in ListCombination)
            {
                LandPokerType_Enum type = m_GameBase.GetPokersType(child, new List<byte>(new byte[i]), 
                    LandPokerType_Enum.LandPokerType_Error, ref childList);
                if (type != LandPokerType_Enum.LandPokerType_Error)
                    return true;
            }        }
        return false;
    }    bool SmartSelectCard(List<byte> list, int addNum)    {
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
                LandPokerType_Enum type = m_GameBase.GetPokersType(resList, new List<byte>(), LandPokerType_Enum.LandPokerType_Error, ref validList);                if (type != CurPokerType)
                    return false;

                UpdateCards(false, resList);
                return true;
            }
        }

        return false;    }    void UpdatePreCard()    {        if(m_bDrag)        {            if (Input.GetMouseButtonUp(0))            {                OnEndDrag();                return;            }            Vector3 mousePos = Input.mousePosition;            if (mousePos.y >= m_vCardLimit.x && mousePos.y <= m_vCardLimit.y)            {                PlayCard playCard;                Rect lastRect = Rect.zero;
                Rect rect, addRect;                bool hit;                for (int i = 0; i < m_Cards.Count; i++)                {                    playCard = m_Cards[i];                    if (!playCard.gameObject.activeSelf)                        continue;                    hit = false;                    rect = GameFunction.GetSpaceRect(m_GameBase.GameCanvas, playCard.transform as RectTransform, m_GameBase.GameCanvas.worldCamera);                    if (lastRect != Rect.zero)
                    {                        if (rect.yMin > lastRect.yMin)
                        {
                            addRect = Rect.MinMaxRect(lastRect.xMin, lastRect.yMax, rect.xMax, rect.yMax);
                            hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, addRect);                        }                        else if(rect.yMin < lastRect.yMin)
                        {
                            addRect = Rect.MinMaxRect(lastRect.xMin, rect.yMin, rect.xMax, lastRect.yMin);
                            hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, addRect);
                        }
                        rect = Rect.MinMaxRect(rect.xMin, rect.yMin, lastRect.xMin, rect.yMax);
                    }                    if(!hit)                        hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, rect);                    AddOrRemovePreCard((byte)i, hit);                    lastRect = rect;                }            }        }    }    void OnClickCards(EventTriggerType eventtype, object message, PointerEventData eventData)    {        int index = (int)message;        if (eventtype == EventTriggerType.PointerDown)        {            if(!m_bDrag)            {                m_bDrag = true;                m_vStartPos = eventData.position;            }        }    }    public override IEnumerator BeginPoker()    {        m_HaveCards.gameObject.SetActive(true);        m_HavePokerList.Sort(LL_PokerType.SortByIndex);        m_nCurPokerNum = (byte)m_HavePokerList.Count;

        yield return null;        m_HaveCards.GetComponent<GridLayoutGroup>().enabled = true;

        UpdateCardsSpace();        for (int i = m_nCurPokerNum - 1,j = 1; i >= 0 ; i--, j++)        {            m_Cards[i].SetCardSprite(m_GameBase.CommonAssetBundle, m_HavePokerList[i],                 false, 0, (m_GameBase.Bystander ? "_onlooker" : "_big"));            m_Cards[i].Selected(false);            CustomAudioDataManager.GetInstance().PlayAudio(1088);            yield return new WaitForSecondsRealtime(PokerInterval);        }        yield return null;        m_HaveCards.GetComponent<GridLayoutGroup>().enabled = false;    }    public override void OnAskBeLord(ushort curPro, float time)    {        base.OnAskBeLord(curPro, time);        int flag = 0;        GameKind.AddFlag(0, ref flag);        for (int i = curPro + 1; i < 4; i++)            GameKind.AddFlag(i, ref flag);        m_GameBase.ShowGameBtn(1, flag);    }    public void OnAnswerBeLord(byte value)    {        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_LANDLORDS_CM_ASKBELORDS);        msg.Add(value);        msg.Add(GameMain.hall_.GetPlayerId());        HallMain.SendMsgToRoomSer(msg);    }    public override void BackAskBeLord(byte askPro)    {        base.BackAskBeLord(askPro);        m_GameBase.ShowGameBtn(-1);    }    public override void OnPokerNumChanged(byte num)
    {
        if(m_GameBase.Bystander)
        {
            DebugLog.Log("bystander localPokerNum:" + num);
            m_HavePokerList.Clear();
            for (byte i = 0; i < num; i++)
                m_HavePokerList.Add(RoomInfo.NoSit);        }        m_HaveCards.gameObject.SetActive(true);
        OnDealPoker(-1);
        UpdateCards(true);
        ShowCountdown(false);
    }    public override void UpdateRecordCards()
    {
        OnPokerNumChanged(0);
    }    void UpdateCardsSpace()
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
        m_HaveCards.GetComponent<GridLayoutGroup>().spacing = new Vector2(spacing, 0f);    }    public void UpdateCards(bool modify = false, List<byte> selected = null, string posfix = "")    {        if(modify)        {            m_HavePokerList.Sort(LL_PokerType.SortByIndex);            m_nCurPokerNum = (byte)m_HavePokerList.Count;            int i = 0;            for (; i < m_HavePokerList.Count; i++)            {                m_Cards[i].SetCardSprite(m_GameBase.CommonAssetBundle, m_HavePokerList[i], m_LordIcon.activeSelf && i == 0,                    0, posfix.Length == 0 ? (m_GameBase.Bystander ? "_onlooker" : "_big") : posfix);                m_Cards[i].Selected((selected == null) ? false : selected.Contains(m_HavePokerList[i]));
            }            for (; i < m_Cards.Count; i++)                m_Cards[i].gameObject.SetActive(false);            m_HaveCards.GetComponent<GridLayoutGroup>().enabled = true;            UpdateCardsSpace();            GameMain.WaitForCall(-1f, () => m_HaveCards.GetComponent<GridLayoutGroup>().enabled = false);        }        else        {
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
                }            }        }    }    public override void AddCards(byte[] cards, bool show = true)    {        List<byte> list = null;
        if(m_GameBase.Bystander)
        {
            for (int i = 0; i < cards.Length; i++)
                m_HavePokerList.Add(RoomInfo.NoSit);
        }        else        {
            list = new List<byte>(cards);
            m_HavePokerList.AddRange(list);        }        if (show && list != null)        {
            CustomAudioDataManager.GetInstance().PlayAudio(1088);

            UpdateCards(true, list);
        }        else            UpdateCards(true);    }    public override void ShowLordIcon(bool bLord)
    {
        base.ShowLordIcon(bLord);

        UpdateCards();
    }    public override void RemoveCards(byte[] cards)    {        if (m_GameBase.Bystander)
        {
            m_HavePokerList.RemoveRange(0, cards.Length);
        }        else        {
            foreach (byte card in cards)
                m_HavePokerList.Remove(card);
        }        UpdateCards(true);    }    void ComputeAblePokerArray(List<byte> curPokerList, LandPokerType_Enum curType)    {        m_AblePokerArray.Clear();        m_TipPokerIndex = 0;        if (curPokerList == null || curPokerList.Count == 0)            return;        if (curType == LandPokerType_Enum.LandPokerType_Error             || curType == LandPokerType_Enum.LandPokerType_KingBlast)            return;        curPokerList.Sort(LL_PokerType.SortByIndex);        Dictionary<int, List<byte>> myValueList;        LL_PokerType.GetValueList(m_HavePokerList, out myValueList);        Dictionary<int, List<byte>> curValueList;        LL_PokerType.GetValueList(curPokerList, out curValueList);        List<byte> pokerList = new List<byte>();        List<List<byte>> splitPokerArray = new List<List<byte>>();        int typeCount = 0, typeNum = 0, curMinValue = 0;        if (curType == LandPokerType_Enum.LandPokerType_Four)        {            curMinValue = LL_PokerType.GetPokerLogicValue(curPokerList[0]);            foreach (int pokerValue in myValueList.Keys)            {                if (myValueList[pokerValue].Count == 4 && pokerValue > curMinValue)                {                    m_AblePokerArray.Add(new List<byte>(myValueList[pokerValue]));                }            }            if(myValueList.ContainsKey(16) && myValueList.ContainsKey(17))//王炸            {                pokerList.Clear();                pokerList.Add(myValueList[16][0]);                pokerList.Add(myValueList[17][0]);                m_AblePokerArray.Add(new List<byte>(pokerList));            }        }        else        {            if(m_HavePokerList.Count >= curPokerList.Count)            {
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
                                        }                                    }
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
                                                {                                                    bSplit1 = true;
                                                    break;
                                                }                                            }
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
            }            foreach (int pokerValue in myValueList.Keys)            {                if (myValueList[pokerValue].Count == 4) //4张是炸弹                {                    m_AblePokerArray.Add(new List<byte>(myValueList[pokerValue]));                }            }            if (myValueList.ContainsKey(16) && myValueList.ContainsKey(17))//王炸            {                pokerList.Clear();                pokerList.Add(myValueList[16][0]);                pokerList.Add(myValueList[17][0]);                m_AblePokerArray.Add(new List<byte>(pokerList));            }            m_AblePokerArray.AddRange(splitPokerArray);            splitPokerArray.Clear();        }    }    public override void OnAskDealPoker(bool bHaveRight, List<byte> curPokerList, LandPokerType_Enum pokerType, float askTime, float bankTime, bool pause)    {        base.OnAskDealPoker(bHaveRight, curPokerList, pokerType, askTime, bankTime, pause);        m_bAnswered = false;        int flag = 0;        if(bHaveRight || m_GameBase.Bystander)        {            GameKind.AddFlag(2, ref flag);//出牌

            m_PlayPokerList.Clear();            CurPokerType = LandPokerType_Enum.LandPokerType_Error;        }        else        {            ComputeAblePokerArray(curPokerList, pokerType);            if (m_AblePokerArray.Count == 0)
            {                GameKind.AddFlag(3, ref flag);//要不起
                m_NoBigTextObj.SetActive(true);
            }            else            {                GameKind.AddFlag(0, ref flag);//不出                GameKind.AddFlag(1, ref flag);//提示                GameKind.AddFlag(2, ref flag);//出牌            }            m_PlayPokerList = curPokerList;            CurPokerType = pokerType;        }        m_GameBase.ShowGameBtn(2, flag);        CanOutPoker();
    }    void GetPrePokers(out List<byte> preList)    {        preList = new List<byte>();        for(int i = 0; i < m_Cards.Count; i++)        {            if (m_Cards[i].Selected())                preList.Add(m_HavePokerList[i]);        }    }    public void OnAnswerPoker(byte index, Button btn)// 0:不出 1:提示 2:出牌 3:要不起    {        if(index == 1)        {            if(m_AblePokerArray.Count > 0)            {                UpdateCards(false, m_AblePokerArray[m_TipPokerIndex]);                m_TipPokerIndex++;                if (m_TipPokerIndex == m_AblePokerArray.Count)                    m_TipPokerIndex = 0;                if(btn != null)                {
                    btn.interactable = false;
                    GameMain.WaitForCall(0.5f, () => btn.interactable = true);                }

                m_GameBase.OutPokerBtn.interactable = true;            }        }        else if(!m_bAnswered)        {            if (index == 2)
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
        }    }    public override void OnDealPoker(short haveRight, byte[] cards = null, LandPokerType_Enum pokerType = LandPokerType_Enum.LandPokerType_Error)    {        base.OnDealPoker(haveRight, cards, pokerType);        m_GameBase.ShowGameBtn(-1);        m_AblePokerArray.Clear();        m_TipPokerIndex = 0;        m_NoBigTextObj.SetActive(false);
        if (haveRight >= 0 && (cards == null || cards.Length == 0))            UpdateCards();    }    public override void ShowEndPoker(bool show, byte[] cards)    {        if (show && m_GameBase.Bystander)
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

    void OnClickBackground(EventTriggerType eventtype, object message, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerDown)        {            if (!m_bDrag)            {                UpdateCards();                CanOutPoker();            }        }    }

    bool HandleNetMsg(uint _msgType, UMessage _ms)
    {
        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;
        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CCMsg_LANDLORDS_SM_DEALPOKERFAILED:
                {
                    byte nState = _ms.ReadByte();                    DebugLog.Log("Deal poker failed: " + nState);                    OnDealPokerFailed(nState);                }
                break;

            default:
                break;
        }

        return true;
    }

    void OnDealPokerFailed(byte nState = 0)//0:没有选牌 1：不在房间 2:不在出牌状态 3：没轮到 4：牌里有你没有的 5：牌型不对
    {        m_bAnswered = false;        if (nState == 0)            CRollTextUI.Instance.AddVerticalRollText(2306);        else if (nState < 4)
        {            CCustomDialog.OpenCustomConfirmUI(2309);
            UpdateCards();
        }        else        {            CRollTextUI.Instance.AddVerticalRollText(2304);            UpdateCards();        }        m_GameBase.ShowGameBtn(2);        m_GameBase.OutPokerBtn.interactable = false;    }}