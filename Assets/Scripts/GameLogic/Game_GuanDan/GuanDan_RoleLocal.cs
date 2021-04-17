using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USocket.Messages;
using XLua;

[Hotfix]
public class GuanDan_RoleLocal : GuanDan_Role
{
    Transform m_LipaiTfm, m_SendPokerTfm, m_VertLipaiTfm;
    Transform m_GongBtnsTfm, m_ChoosePokerTypeTfm;

    HashSet<PlayCard> m_setPreCards = new HashSet<PlayCard>();

    List<List<byte>> m_HaveLipaiList = new List<List<byte>>();
    List<List<byte>> m_VertHaveLipaiList = new List<List<byte>>();

    List<List<byte>> m_AblePokerArray = new List<List<byte>>();
    Dictionary<byte, List<List<byte>>> m_TongHuaArray = new Dictionary<byte, List<List<byte>>>();
    byte m_TipPokerIndex = 0;
    KeyValuePair<byte, byte> m_TipTongIndex = new KeyValuePair<byte, byte>(0, 0);
    sbyte m_nCurLipaiIndex = -1;
    bool m_bAnswered = true;    bool m_bDrag = false;
    Vector2 m_vStartPos;

    float m_fAlarmTimer = -1f;

    Vector2 m_vCardYLimit = Vector2.zero;
    float m_fVertCardWidth = 0.0f;

    Transform m_GameBtnsTfm;
    Transform m_ArrayBtnsTfm;

    Button m_OutPokerBtn;

    Button[] m_TongHuaBtn = new Button[5];
    Button[] m_LipaiBtns;

    GameObject m_NoBigTextObj;

    enum ePokerArrayType
    {
        ePAT_Bomb,
        ePAT_Color,
        ePAT_Index,

        ePAT_Num
    }
    ePokerArrayType m_eCurArrayType = ePokerArrayType.ePAT_Index;

    public GuanDan_RoleLocal(CGame_GuanDan game, byte index)
        : base(game, index)
    {
        InitUI();

        OnPSTChange();

        InitMsgHandle();
    }

    void InitUI()
    {
        XPointEvent.AutoAddListener(m_GameBase.MainUITfm.Find("UiRootBG").gameObject, OnClickBackground, null);

        m_SendPokerTfm = m_GameBase.MainUITfm.Find("Middle/fapai");
        m_PlayCards = m_GameBase.MainUITfm.Find("Middle/Poker_Chupai");
        m_CountdownObj = m_GameBase.MainUITfm.Find("Middle/OutlineCountdown/ImageBG/Player_1").gameObject;
        m_CountdownObj.SetActive(false);
        m_ChoosePokerTypeTfm = m_GameBase.MainUITfm.Find("Middle/choosePaixing");
        m_ChoosePokerTypeTfm.gameObject.SetActive(false);

        //horz
        Transform parent = m_GameBase.MainUITfm.Find("Middle/PlayerInfor_1_1");
        Transform tfm = parent.Find("Icon_PlayerInfor_1");

        Transform tfmChild = tfm.Find("Poker_Shoupai/Shoupai_1");
        foreach (Transform child in tfmChild)
        {
            PlayCard card = child.gameObject.AddComponent<PlayCard>();
            XPointEvent.AutoAddListener(child.gameObject, OnClickCards, card);
        }

        if (tfmChild.childCount > 1)
        {
            Rect rect = GameFunction.GetSpaceRect(m_GameBase.GameCanvas, tfmChild.GetChild(0) as RectTransform, m_GameBase.GameCanvas.worldCamera);
            m_vCardYLimit = new Vector2(rect.yMin, rect.yMax + rect.height * 0.1f);
        }

        tfm = parent.Find("ButtonBG");
        tfmChild = tfm.Find("ButtonBG_lipai");
        m_TongHuaBtn[0] = tfmChild.Find("Button_tonghua").GetComponent<Button>();
        m_TongHuaBtn[0].onClick.AddListener(() =>OnClickTongHua(0));

        Button[] buttons = tfmChild.Find("lipai_zidingyi").GetComponentsInChildren<Button>(true);
        byte id = 0;
        foreach (Button btn in buttons)
        {
            byte temp = id;
            btn.onClick.AddListener(() => OnClickLipaiBtn(temp));
            id++;
            btn.gameObject.SetActive(false);
        }

        buttons = tfmChild.Find("zu").GetComponentsInChildren<Button>(true);
        id = 0;
        foreach (Button btn in buttons)
        {
            byte temp = id;
            btn.onClick.AddListener(() => OnClickReArrayBtn((ePokerArrayType)temp));
            id++;
        }

        //vert
        parent = m_GameBase.MainUITfm.Find("Middle/PlayerInfor_1_2");
        m_VertLipaiTfm = parent.Find("Icon_PlayerInfor_1/Poker_Shoupai/Shoupai_1/Point_lipai_1");

        tfm = parent.Find("ButtonBG");
        tfmChild = tfm.Find("ButtonBG_lipai");
        buttons = tfmChild.Find("lipai_tonghua").GetComponentsInChildren<Button>(true);
        id = 1;
        foreach (Button btn in buttons)
        {
            byte temp = id;
            btn.onClick.AddListener(() => OnClickTongHua(temp));
            m_TongHuaBtn[temp] = btn;
            id++;
        }

        buttons = tfmChild.Find("lipai_zidingyi").GetComponentsInChildren<Button>(true);
        id = 0;
        foreach (Button btn in buttons)
        {
            byte temp = id;
            btn.onClick.AddListener(() => OnClickVertLipaiBtn(temp));
            id++;
            btn.gameObject.SetActive(false);
        }

        tfmChild = tfm.Find("Button_showdesk");
        XPointEvent.AutoAddListener(tfmChild.gameObject, OnClickShowDesk, null);

        //generic
        GameObject obj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("ButtonBG_xingpai");
        m_GameBtnsTfm = ((GameObject)GameMain.instantiate(obj)).transform;
        m_OutPokerBtn = m_GameBtnsTfm.Find("Button_chupai").GetComponent<Button>();
        buttons = m_GameBtnsTfm.GetComponentsInChildren<Button>(true);
        id = 0;
        foreach (Button btn in buttons)
        {
            byte temp = id;
            btn.onClick.AddListener(() => OnClickGameBtn(temp, btn));
            id++;
        }

        m_GongBtnsTfm = m_GameBase.MainUITfm.Find("Middle/ButtonBG_gong");
        buttons = m_GongBtnsTfm.GetComponentsInChildren<Button>(true);
        id = 0;
        foreach (Button btn in buttons)
        {
            byte temp = id;
            btn.onClick.AddListener(() => OnClickGongBtn(temp, btn));
            id++;
        }
    }

    void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_OUTPOKERS, HandleNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_FRIENDPOKERS, HandleNetMsg);
    }

    public override void Init()
    {
        base.Init();
    }

    public override void SetupInfoUI(bool bContest, long coin, uint userId, string name, string url = "", int faceId = 0, float masterScore = 0f, byte male = 1)
    {
        base.SetupInfoUI(bContest, coin, userId, name, url, faceId, masterScore, male);

        int another = m_GameBase.CurPOT == PokerSortType.ePST_Horizontal ? 2 : 1;
        Transform parent = m_GameBase.MainUITfm.Find("Middle/PlayerInfor_1_" + another);
        Transform anotherInfo = parent.Find("Icon_PlayerInfor_1/Playerinfo");
        SetupInfoUI(anotherInfo, bContest, coin, userId, name, url, faceId, masterScore);
    }

    public override void CreateRole()
    {
        int cur = (int)m_GameBase.CurPOT + 1;
        Transform parent = m_GameBase.MainUITfm.Find("Middle/PlayerInfor_1_" + cur);
        CreateRole(false, parent.Find("Icon_PlayerInfor_1/point_ren"));
    }

    public override void OnTick()
    {
        base.OnTick();

        UpdatePreCard();

        UpdateAlarm();

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    LL_PokerType.CurGrade = 2;
        //    OnEnd();
        //    m_HavePokerList = new List<byte> { 0x22, 0x12, 0x02, 0x15, 0x3c, 0x2c,
        //        0x1c, 0x0c, 0x3b, 0x1b, 0x3a, 0x1a, 0x11, 0x14, 0x19, 0x37, 0x17
        //        };
        //    GameMain.SC(BeginPoker(new List<byte>(), 0x14));
        //    OnAskDealPoker(true, new List<byte> (), GuandanPokerType_Enum.GuanPokerType_Error, 10, 20, true);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    List<byte> list;
        //    GetPrePokers(out list);
        //    OnDealPoker(1, list.ToArray(), GuandanPokerType_Enum.GuanPokerType_Error, 0);
        //}
    }

    public override void OnEnd()
    {
        base.OnEnd();        RemoveAllLipai();
        m_HaveLipaiList.Clear();
        m_VertHaveLipaiList.Clear();

        if((m_GameBase.CurPOT == PokerSortType.ePST_Horizontal))
        {
            List<PlayCard> list = GetAllCards();
            foreach (PlayCard card in list)
                card.Init();
        }
        else
        {
            Transform parent = m_HaveCards.Find("Point_shoupai");
            foreach (Transform tfm in parent)
                GameObject.Destroy(tfm.gameObject);
        }
        m_setPreCards.Clear();
        m_AblePokerArray.Clear();
        m_TipPokerIndex = 0;
        m_TipTongIndex = new KeyValuePair<byte, byte>(0,0);
        m_eCurArrayType = ePokerArrayType.ePAT_Index;
        m_nCurLipaiIndex = -1;
        m_fAlarmTimer = -1f;
        m_SendPokerTfm.gameObject.SetActive(false);
        m_ChoosePokerTypeTfm.gameObject.SetActive(false);
        m_NoBigTextObj.SetActive(false);
        m_ArrayBtnsTfm.gameObject.SetActive(false);
        m_bAnswered = true;
        ShowGameBtn(false);        ShowGongBtn(false);
    }

    public override IEnumerator BeginPoker(List<byte> showGradeIndex, byte showPoker)
    {
        m_SendPokerTfm.GetComponent<GridLayoutGroup>().enabled = true;
        m_SendPokerTfm.gameObject.SetActive(true);
        m_HaveCards.gameObject.SetActive(false);

        int index = 0;
        byte laizi = LL_PokerType.GetLaiziValue();
        int showIndex = 0;
        if (showGradeIndex != null)
            showGradeIndex.Sort();

        bool bShowMask;
        foreach (Transform tfm in m_SendPokerTfm)
        {
            if(index < m_HavePokerList.Count)
            {
                bShowMask = LL_PokerType.GetPokerLogicValue(m_HavePokerList[index], laizi) == 15;
                PlayCard.SetCardSprite(tfm.gameObject, m_GameBase.CommonAssetBundle, m_HavePokerList[index], bShowMask, laizi, (m_GameBase.Bystander ? "_onlooker" : "_big"));

                if(showGradeIndex != null)
                {
                    if(showIndex < showGradeIndex.Count)
                    {
                        if(index == showGradeIndex[showIndex])
                        {
                            ShowTipPoker(showPoker, showIndex);
                            showIndex++;
                        }
                    }
                }

                CustomAudioDataManager.GetInstance().PlayAudio(1004);

                index++;

                yield return new WaitForSecondsRealtime(PokerInterval);
            }
        }

        yield return new WaitForSecondsRealtime(0.3f);

        m_SendPokerTfm.gameObject.SetActive(false);
        Refresh(true, true);

        yield return new WaitForSecondsRealtime(0.1f);

        m_ArrayBtnsTfm.gameObject.SetActive(true);
    }

    void ModifyHorizCards(string posfix = "")
    {
        byte laizi = LL_PokerType.GetLaiziValue();
        int index = 0;
        bool bShowMask;
        PlayCard pc;
        foreach (Transform tfm in m_HaveCards)
        {
            pc = tfm.GetComponent<PlayCard>();

            if (index < m_HavePokerList.Count)
            {
                tfm.gameObject.SetActive(true);
                bShowMask = LL_PokerType.GetPokerLogicValue(m_HavePokerList[index], laizi) == 15;
                pc.SetCardSprite(m_GameBase.CommonAssetBundle, m_HavePokerList[index], bShowMask, laizi, 
                    posfix.Length == 0 ? (m_GameBase.Bystander ? "_onlooker" : "_big") : posfix);
                pc.Selected(false);
                pc.Masked = (Rank > 0 && posfix.Length == 0);
            }
            else
                tfm.gameObject.SetActive(false);

            index++;
        }
    }

    IEnumerator ReArrayPoker(bool anim, UnityAction call = null, bool bVert = false)
    {
        m_HaveCards.GetComponent<HorizontalLayoutGroup>().enabled = true;

        float scaleTime = 0.05f;

        if(anim)
        {
            foreach (Transform tfm in m_SendPokerTfm)
            {
                tfm.DOScaleX(0.1f, scaleTime);
            }

            yield return new WaitForSecondsRealtime(scaleTime);

            foreach (Transform tfm in m_SendPokerTfm)
            {
                tfm.DOScaleX(1f, scaleTime);
                tfm.gameObject.SetActive(false);
            }
        }

        m_HaveCards.gameObject.SetActive(true);

        if (!bVert)
        {
            ModifyHorizCards();

            yield return new WaitForSecondsRealtime(scaleTime);
        }

        if (call != null)
            call.Invoke();

        yield return new WaitForEndOfFrame();

        m_HaveCards.GetComponent<HorizontalLayoutGroup>().enabled = false;

        if(bVert)
        {
            yield return new WaitForEndOfFrame();

            m_HaveCards.GetComponent<HorizontalLayoutGroup>().enabled = true;
        }
    }

    void OnClickCards(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        if (Rank > 0 || m_GameBase.GameMode == GameTye_Enum.GameType_Record)
            return;

        PlayCard card = (PlayCard)message;

        if (eventtype == EventTriggerType.PointerDown)
        {
            if (!m_bDrag)
            {
                m_bDrag = true;
                m_vStartPos = eventData.position;

                GetSelLipai(true);
            }
        }
    }

    void OnClickBackground(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        if (Rank > 0)
            return;

        if (eventtype == EventTriggerType.PointerDown)
        {
            if (!m_bDrag)
            {
                UpdateCards();
            }
        }
    }

    void AddOrRemovePreCard(PlayCard pc, bool bAdd)
    {
        bool suc = false;
        if (bAdd)
            suc = m_setPreCards.Add(pc);
        else
        {
            if (!m_setPreCards.Contains(pc))
                return;
            suc = m_setPreCards.Remove(pc);
        }

        if(suc)
        {
            if (m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
                pc.Masked = bAdd;
            else
                pc.Masked = !pc.Masked;
        }
    }

    GuandanPokerType_Enum CanLipai(List<byte> list, ref List<byte> childList, bool checkStraightFlush = false)
    {
        if (list.Count < 4)
            return GuandanPokerType_Enum.GuanPokerType_Error;

        byte laizi = LL_PokerType.GetLaiziValue();
        Dictionary<int, List<byte>> valueList;
        LL_PokerType.GetValueList(list, out valueList, laizi);

        int biggest = 0;

        if (LL_PokerType.IsNormalBomb(valueList, ref childList, ref biggest) > 0)
        {
            childList.Reverse();
            return GuandanPokerType_Enum.GuanPokerType_Blast;
        }

        if(list.Count == 5)
        {
            if (LL_PokerType.IsFlush(valueList, ref childList, ref biggest, true) > 0)
            {
                if(checkStraightFlush)
                {
                    if (LL_PokerType.IsStraightFlush(valueList, ref childList, ref biggest, true) > 0)
                    {
                        childList.Reverse();
                        return GuandanPokerType_Enum.GuanPokerType_SameColorFlush;
                    }
                }
                childList.Reverse();
                return GuandanPokerType_Enum.GuanPokerType_Flush;
            }

            if (LL_PokerType.IsThreeAndTwo(valueList, ref childList, ref biggest) > 0)
                return GuandanPokerType_Enum.GuanPokerType_ThreeAndTwo;
        }

        if(list.Count == 6)
        {
            if (LL_PokerType.IsPlaneNoWith(valueList, ref childList, ref biggest, true) > 0)
            {
                childList.Reverse();
                return GuandanPokerType_Enum.GuanPokerType_SeriesThree;
            }

            if (LL_PokerType.IsSeriesPairs(valueList, ref childList, ref biggest, true) > 0)
            {
                childList.Reverse();
                return GuandanPokerType_Enum.GuanPokerType_SeriesPairs;
            }
        }

        return GuandanPokerType_Enum.GuanPokerType_Error;
    }

    void OnVertSelectedChanged()
    {
        if (m_GameBase.CurPOT != PokerSortType.ePST_Vertical)
            return;

        List<byte> list;
        GetPrePokers(out list, true);
        float spacing = list.Count > 0 ? -85f : -100f;
        Transform parent = m_HaveCards.Find("Point_shoupai");
        foreach (Transform child in parent)
        {
            if (!child.gameObject.activeSelf)
                continue;
            child.GetComponent<VerticalLayoutGroup>().spacing = spacing;
        }

        RectTransform rtfm;
        foreach (Transform child in m_LipaiTfm)
        {
            rtfm = child.Find("ImagePoker") as RectTransform;
            rtfm.GetComponent<VerticalLayoutGroup>().spacing = spacing;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rtfm);
            (child as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtfm.sizeDelta.y);
        }
        foreach (Transform child in m_VertLipaiTfm)
        {
            rtfm = child.Find("ImagePoker") as RectTransform;
            rtfm.GetComponent<VerticalLayoutGroup>().spacing = spacing;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rtfm);
            (child as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtfm.sizeDelta.y);
        }
    }

    void OnEndDrag()
    {
        int selectNum = 0, unselectNum = 0;
        if(m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
        {
            foreach (PlayCard card in m_setPreCards)
            {
                card.Selected(!card.Selected());
                card.Masked = false;

                if (card.Selected())
                    selectNum++;
                else
                    unselectNum++;
            }
        }
        else
        {
            foreach (PlayCard card in m_setPreCards)
            {
                if (card.Masked)
                    selectNum++;
                else
                    unselectNum++;
            }
        }
        m_setPreCards.Clear();

        m_bDrag = false;

        UpdateGameBtn(null, selectNum, unselectNum);
    }

    void UpdatePreCard()
    {
        if (m_bDrag)
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnEndDrag();
                return;
            }

            Vector3 mousePos = Input.mousePosition;
            if (m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
            {
                if (mousePos.y >= m_vCardYLimit.x && mousePos.y <= m_vCardYLimit.y)
                {
                    Rect lastRect = Rect.zero;
                    Rect rect, addRect;
                    bool hit;
                    List<PlayCard> list = GetAllCards();
                    list.Reverse();

                    foreach (PlayCard pc in list)
                    {
                        hit = false;
                        rect = GameFunction.GetSpaceRect(m_GameBase.GameCanvas, pc.transform as RectTransform, m_GameBase.GameCanvas.worldCamera);
                        if (lastRect != Rect.zero)
                        {
                            if (rect.yMin > lastRect.yMin)
                            {
                                addRect = Rect.MinMaxRect(lastRect.xMin, lastRect.yMax, rect.xMax, rect.yMax);
                                hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, addRect);
                            }
                            else if (rect.yMin < lastRect.yMin)
                            {
                                addRect = Rect.MinMaxRect(lastRect.xMin, rect.yMin, rect.xMax, lastRect.yMin);
                                hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, addRect);
                            }

                            rect = Rect.MinMaxRect(rect.xMin, rect.yMin, lastRect.xMin, rect.yMax);
                        }

                        if (!hit)
                            hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, rect);

                        AddOrRemovePreCard(pc, hit);

                        lastRect = rect;
                    }
                }
            }
            else
            {
                Rect lastRect = Rect.zero, lastVRect = Rect.zero;
                Rect rect, addRect;
                bool hit;
                List<PlayCard> list = GetAllCards();
                list.Reverse();

                foreach (PlayCard pc in list)
                {
                    hit = false;
                    rect = GameFunction.GetSpaceRect(m_GameBase.GameCanvas, pc.transform as RectTransform, m_GameBase.GameCanvas.worldCamera);
                    if (lastRect != Rect.zero)
                    {
                        if(rect.xMin < lastRect.xMin)
                        {
                            lastVRect = lastRect;
                            if(rect.xMax > lastRect.xMin)
                                rect = Rect.MinMaxRect(rect.xMin, rect.yMin, lastRect.xMin, rect.yMax);
                        }
                        else
                        {
                            if(lastVRect == Rect.zero)
                                rect = Rect.MinMaxRect(rect.xMin, lastRect.yMax, rect.xMax, rect.yMax);
                            else
                            {
                                if(rect.xMax < lastVRect.xMin)
                                    rect = Rect.MinMaxRect(rect.xMin, lastRect.yMax, rect.xMax, rect.yMax);
                                else if(rect.yMax > lastVRect.yMax)
                                {
                                    if(rect.yMin >= lastVRect.yMax || lastRect.yMin >= lastVRect.yMax)
                                        rect = Rect.MinMaxRect(rect.xMin, lastRect.yMax, rect.xMax, rect.yMax);
                                    else
                                    {
                                        addRect = Rect.MinMaxRect(rect.xMin, lastRect.yMax, lastVRect.xMin, lastVRect.yMax);
                                        hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, addRect);

                                        rect = Rect.MinMaxRect(rect.xMin, lastVRect.yMax, rect.xMax, rect.yMax);
                                    }
                                }
                                else
                                    rect = Rect.MinMaxRect(rect.xMin, lastRect.yMax, lastVRect.xMin, rect.yMax);
                            }
                        }
                    }

                    if (!hit)
                        hit = GameFunction.IsLineCrossRectangle(m_vStartPos, Input.mousePosition, rect);

                    if(hit)
                        AddOrRemovePreCard(pc, hit);

                    lastRect = rect;
                }
            }
        }
    }

    List<PlayCard> GetAllCards(bool alldown = false)
    {
        List<PlayCard> list = new List<PlayCard>();
        PlayCard pc;

        if(m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
        {
            foreach (Transform tfm in m_HaveCards)
            {
                if (!tfm.gameObject.activeSelf)
                    continue;

                pc = tfm.GetComponent<PlayCard>();
                list.Add(pc);

                if (alldown)
                    pc.Selected(false);
            }
        }
        else
        {
            Transform parent = m_HaveCards.Find("Point_shoupai");
            foreach (Transform child in parent)
            {
                if (!child.gameObject.activeSelf)
                    continue;

                foreach (Transform tfm in child)
                {
                    if (!tfm.gameObject.activeSelf)
                        continue;

                    pc = tfm.GetComponent<PlayCard>();
                    list.Add(pc);

                    if (alldown)
                        pc.Masked = false;
                }
            }
        }

        if (alldown)
            UpdateGameBtn(new List<byte>());

        return list;
    }

    PlayCard GetSelLipai(bool allDown = false)
    {
        PlayCard pc, selPc = null;
        if(m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
        {
            foreach (Transform tfm in m_LipaiTfm)
            {
                pc = tfm.GetComponent<PlayCard>();
                if (pc == null)
                    continue;

                if (pc.Selected())
                    selPc = pc;

                if (allDown)
                {
                    pc.Selected(false);

                    foreach (Transform child in tfm.Find("ImagePoker"))
                    {
                        child.Find("image_Mask").gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            foreach (Transform tfm in m_LipaiTfm)
            {
                pc = tfm.GetComponent<PlayCard>();
                if (pc == null)
                    continue;

                if (pc.Masked)
                    selPc = pc;

                if (allDown)
                {
                    pc.Masked = false;

                    foreach (Transform child in tfm.Find("ImagePoker"))
                    {
                        child.Find("image_Mask").gameObject.SetActive(false);
                    }
                }
            }

            if(selPc == null)
            {
                foreach (Transform tfm in m_VertLipaiTfm)
                {
                    pc = tfm.GetComponent<PlayCard>();
                    if (pc == null)
                        continue;

                    if (pc.Masked)
                        selPc = pc;

                    if (allDown)
                    {
                        pc.Masked = false;

                        foreach (Transform child in tfm.Find("ImagePoker"))
                        {
                            child.Find("image_Mask").gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        if (allDown)
            m_LipaiBtns[1].gameObject.SetActive(false);

        return selPc;
    }

    //返回理牌的索引，-1表示不是理牌
    sbyte GetPrePokers(out List<byte> preList, bool bIncludeLipai = false)
    {
        preList = new List<byte>();

        PlayCard pc;
        if (bIncludeLipai)
        {
            pc = GetSelLipai();
            if (pc != null)
            {
                int si = pc.transform.GetSiblingIndex();
                if(pc.transform.parent == m_LipaiTfm)
                {
                    if (si < m_HaveLipaiList.Count)
                    {
                        preList.AddRange(new List<byte>(m_HaveLipaiList[si]));
                        return (sbyte)si;
                    }
                }
                else
                {
                    if(si < m_VertHaveLipaiList.Count)
                    {
                        preList.AddRange(new List<byte>(m_VertHaveLipaiList[si]));
                        si += m_HaveLipaiList.Count;
                        return (sbyte)si;
                    }
                }
            }
            else
            {
                Transform tfm, child;
                for (int i = 0; i < m_LipaiTfm.childCount; i++)
                {
                    tfm = m_LipaiTfm.GetChild(i).Find("ImagePoker");
                    for (int j = 0; j < tfm.childCount; j++)
                    {
                        child = tfm.GetChild(j);
                        if (!child.gameObject.activeSelf)
                            continue;

                        if (child.Find("image_Mask").gameObject.activeSelf)
                            preList.Add(m_HaveLipaiList[i][j]);
                    }
                }

                for (int i = 0; i < m_VertLipaiTfm.childCount; i++)
                {
                    tfm = m_VertLipaiTfm.GetChild(i).Find("ImagePoker");
                    for (int j = 0; j < tfm.childCount; j++)
                    {
                        child = tfm.GetChild(j);
                        if (!child.gameObject.activeSelf)
                            continue;

                        if (child.Find("image_Mask").gameObject.activeSelf)
                            preList.Add(m_VertHaveLipaiList[i][j]);
                    }
                }

            }
        }

        if(m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
        {
            foreach (Transform tfm in m_HaveCards)
            {
                if (!tfm.gameObject.activeSelf)
                    continue;

                pc = tfm.GetComponent<PlayCard>();
                if (pc.Selected())
                    preList.Add(pc.Value);
            }
        }
        else
        {
            Transform parent = m_HaveCards.Find("Point_shoupai");
            foreach (Transform child in parent)
            {
                if (!child.gameObject.activeSelf)
                    continue;

                foreach (Transform tfm in child)
                {
                    if (!tfm.gameObject.activeSelf)
                        continue;

                    pc = tfm.GetComponent<PlayCard>();
                    if (pc.Masked)
                        preList.Add(pc.Value);
                }
            }
        }
        return -1;
    }

    void OnClickGameBtn(byte index, Button btn = null)
    {
        if (btn != null)
            CustomAudioDataManager.GetInstance().PlayAudio(1005);

        OnAnswerPoker(index, btn);
    }

    void UpdateGameBtn(List<byte> outList = null, int selectNum = 0, int unselectNum = 0)
    {
        if(outList == null)
            GetPrePokers(out outList);

        List<byte> childList = new List<byte>();
        m_LipaiBtns[0].gameObject.SetActive(CanLipai(outList, ref childList) != GuandanPokerType_Enum.GuanPokerType_Error);
        Button btn = m_GongBtnsTfm.Find("Button_shanggong").GetComponent<Button>();
        if (btn.gameObject.activeSelf)
            btn.interactable = outList.Count == 1;
        btn = m_GongBtnsTfm.Find("Button_huangong").GetComponent<Button>();
        if (btn.gameObject.activeSelf)
            btn.interactable = outList.Count == 1;
        CanOutPoker(selectNum, unselectNum);

        OnVertSelectedChanged();
    }

    public void UpdateCards(List<byte> selected = null)
    {
        GetSelLipai(true);

        if (selected == null)
        {
            GetAllCards(true);
            m_ChoosePokerTypeTfm.gameObject.SetActive(false);
        }
        else
        {
            List<byte> clone = new List<byte>(selected);
            clone.Sort();

            //优先判断理牌里是否有吻合的
            List<byte> childLipai;
            for (int i = 0; i < m_HaveLipaiList.Count; i++)
            {
                childLipai = new List<byte>(m_HaveLipaiList[i]);
                childLipai.Sort();
                if(childLipai.SequenceEqual(clone))
                {
                    if (m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
                        OnClickLipai(EventTriggerType.PointerDown, m_LipaiTfm.GetChild(i).GetComponent<PlayCard>(), null);
                    else
                        OnClickVertLipai(EventTriggerType.PointerDown, m_LipaiTfm.GetChild(i).GetComponent<PlayCard>(), null);
                    return;
                }
            }

            for (int i = 0; i < m_VertHaveLipaiList.Count; i++)
            {
                childLipai = new List<byte>(m_VertHaveLipaiList[i]);
                childLipai.Sort();
                if (childLipai.SequenceEqual(clone))
                {
                    OnClickVertLipai(EventTriggerType.PointerDown, m_VertLipaiTfm.GetChild(i).GetComponent<PlayCard>(), null);
                    return;
                }
            }


            List<PlayCard> list = GetAllCards();

            if (m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
            {
                //去除已经提出来的牌
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Selected())
                    {
                        if (clone.Remove(list[i].Value))
                            list.RemoveAt(i);
                    }
                }

                foreach (PlayCard pc in list)
                {
                    pc.Selected(clone.Remove(pc.Value));
                }
            }
            else
            {
                //去除已经提出来的牌
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Masked)
                    {
                        if (clone.Remove(list[i].Value))
                            list.RemoveAt(i);
                    }
                }

                foreach (PlayCard pc in list)
                {
                    pc.Masked = clone.Remove(pc.Value);
                }
            }

            int index;
            GameObject go;
            bool find = false;
            foreach (byte poker in clone)
            {
                find = false;

                for (int i = 0; i < m_HaveLipaiList.Count; i++)
                {
                    find = false;
                    childLipai = new List<byte>(m_HaveLipaiList[i]);
                    index = childLipai.FindLastIndex(s => s == poker);
                    while (index != -1)
                    {
                        go = m_LipaiTfm.GetChild(i).Find("ImagePoker").GetChild(index)
                            .Find("image_Mask").gameObject;
                        if (!go.activeSelf)
                        {
                            go.SetActive(true);
                            find = true;
                            break;
                        }

                        childLipai.RemoveAt(index);
                        index = childLipai.FindLastIndex(s => s == poker);
                    }

                    if (find)
                        break;
                }

                if(find)
                    continue;

                for (int i = 0; i < m_VertHaveLipaiList.Count; i++)
                {
                    find = false;
                    childLipai = new List<byte>(m_VertHaveLipaiList[i]);
                    index = childLipai.FindLastIndex(s => s == poker);
                    while (index != -1)
                    {
                        go = m_VertLipaiTfm.GetChild(i).Find("ImagePoker").GetChild(index)
                            .Find("image_Mask").gameObject;
                        if (!go.activeSelf)
                        {
                            go.SetActive(true);
                            find = true;
                            break;
                        }

                        childLipai.RemoveAt(index);
                        index = childLipai.FindLastIndex(s => s == poker);
                    }

                    if (find)
                        break;
                }
            }
        }

        UpdateGameBtn();
    }

    void OnClickTongHua(byte index)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1005);

        if (m_TongHuaArray.Count == 0)
            return;

        if(index == 0)
        {
            List<byte> colors = new List<byte>(m_TongHuaArray.Keys);
            byte key = m_TipTongIndex.Key;
            byte color = colors[key];
            List<List<byte>> list = m_TongHuaArray[color];
            byte id = m_TipTongIndex.Value;
            UpdateCards(list[id]);

            id++;
            if (id >= list.Count)
            {
                id = 0;
                key++;
                if (key >= colors.Count)
                    key = 0;
            }
            m_TipTongIndex = new KeyValuePair<byte, byte>(key, id);

            m_TongHuaBtn[0].interactable = false;
            GameMain.WaitForCall(0.5f, () => m_TongHuaBtn[0].interactable = true);
        }
        else
        {
            byte color = index;
            color--;
            List<List<byte>> list = m_TongHuaArray[color];
            if (list.Count == 0)
                return;

            byte id = m_TipTongIndex.Value;
            if (m_TipTongIndex.Key != color)
                id = 0;
            UpdateCards(list[id]);

            id++;
            if (id >= list.Count)
                id = 0;
            m_TipTongIndex = new KeyValuePair<byte, byte>(color, id);
        }
    }

    void OnClickLipaiBtn(byte index)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1005);

        if (index == 0)//理牌
        {
            List<byte> list;
            GetPrePokers(out list);

            Debug.Assert(list.Count > 3, "no out poker!!");

            m_LipaiTfm.GetComponent<HorizontalLayoutGroup>().enabled = true;

            RectTransform rtfm;
            if (m_HaveLipaiList.Count > 0)
            {
                GetSelLipai(true);

                rtfm = m_LipaiTfm.GetChild(m_HaveLipaiList.Count - 1) as RectTransform;
                rtfm.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtfm.rect.width - 100);
            }

            List<byte> childList = new List<byte>();
            if(CanLipai(list, ref childList) == GuandanPokerType_Enum.GuanPokerType_Error)
                return;
            list = new List<byte>(childList);
            m_HaveLipaiList.Add(list);

            GameObject obj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("Shoupai_2_player1");
            obj = GameMain.Instantiate(obj);
            obj.transform.SetParent(m_LipaiTfm, false);

            Transform tfm = obj.transform.Find("ImagePoker");
            AssetBundle ab = m_GameBase.CommonAssetBundle;
            byte laizi = LL_PokerType.GetLaiziValue();
            int i = 0;
            bool bShowMask;
            foreach(Transform child in tfm)
            {
                if (i < list.Count)
                {
                    bShowMask = LL_PokerType.GetPokerLogicValue(list[i], laizi) == 15;
                    PlayCard.SetCardSprite(child.gameObject, ab, list[i], bShowMask, laizi, "_big");
                    m_HavePokerList.Remove(list[i]);
                }
                else
                    child.gameObject.SetActive(false);

                i++;
            }

            rtfm = obj.transform as RectTransform;
            rtfm.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 154 + 43 * (list.Count - 1));

            Refresh(false);

            GameMain.WaitForCall(-1f, () =>
            {
                m_LipaiTfm.GetComponent<HorizontalLayoutGroup>().enabled = false;

                PlayCard card = obj.AddComponent<PlayCard>();
                card.Masked = false;
                XPointEvent.AutoAddListener(obj, OnClickLipai, card);
            });
        }
        else//恢复
        {
            PlayCard pc = GetSelLipai();
            if (pc == null)
                return;

            int si = pc.transform.GetSiblingIndex();
            if(si < m_HaveLipaiList.Count)
            {
                RemoveLipai(si, true);

                Refresh(false);
            }
        }

        GetAllAblePokerArray();
    }

    void OnClickVertLipaiBtn(byte index)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1005);

        if (index == 0)//理牌
        {
            List<byte> list;
            GetPrePokers(out list);

            Debug.Assert(list.Count > 3, "no out poker!!");

            List<byte> childList = new List<byte>();
            GuandanPokerType_Enum lipaiType = CanLipai(list, ref childList, true);
            if (lipaiType == GuandanPokerType_Enum.GuanPokerType_Error)
                return;
            list = new List<byte>(childList);

            byte laizi = LL_PokerType.GetLaiziValue();
            GameObject obj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("Shoupai_vertical");
            obj = GameMain.Instantiate(obj);
            
            //bomb:LipaiTfm, normal:VertLipaiTfm
            if(lipaiType != GuandanPokerType_Enum.GuanPokerType_SameColorFlush && lipaiType != GuandanPokerType_Enum.GuanPokerType_Blast)
            {
                m_VertHaveLipaiList.Add(list);
                obj.transform.SetParent(m_VertLipaiTfm, false);
            }
            else
            {
                m_HaveLipaiList.Insert(0, list);
                obj.transform.SetParent(m_LipaiTfm, false);
                obj.transform.SetSiblingIndex(0);
            }

            Transform tfm = obj.transform.Find("ImagePoker");
            AssetBundle ab = m_GameBase.CommonAssetBundle;
            int i = 0;
            bool bShowMask;
            foreach (Transform child in tfm)
            {
                if (i < list.Count)
                {
                    bShowMask = LL_PokerType.GetPokerLogicValue(list[i], laizi) == 15;
                    PlayCard.SetCardSprite(child.gameObject, ab, list[i], bShowMask, laizi, "_wide");
                    m_HavePokerList.Remove(list[i]);
                }
                else
                    child.gameObject.SetActive(false);

                i++;
            }

            Refresh(false);

            GameMain.WaitForCall(-1f, () =>
            {
                PlayCard card = obj.AddComponent<PlayCard>();
                card.Masked = false;
                XPointEvent.AutoAddListener(obj, OnClickVertLipai, card);

                (card.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (tfm as RectTransform).sizeDelta.y);
            });
        }
        else//恢复
        {
            PlayCard pc = GetSelLipai();
            if (pc == null)
                return;

            int si = pc.transform.GetSiblingIndex();
            if (pc.transform.parent == m_LipaiTfm)
            {
                if (si < m_HaveLipaiList.Count)
                {
                    RemoveLipai(si, true);

                    Refresh(false);
                }
            }
            else
            {
                if (si < m_VertHaveLipaiList.Count)
                {
                    si += m_HaveLipaiList.Count;

                    RemoveLipai(si, true);

                    Refresh(false);
                }
            }
        }

        GetAllAblePokerArray();
    }

    void RemoveAllLipai()
    {
        if (m_LipaiTfm == null || m_VertLipaiTfm == null)
            return;

        foreach (Transform tfm in m_LipaiTfm)
            GameObject.Destroy(tfm.gameObject);
        foreach (Transform tfm in m_VertLipaiTfm)
            GameObject.Destroy(tfm.gameObject);
    }

    void RemoveLipai(int index, bool addToList)
    {
        if (m_HaveLipaiList.Count == 0 && m_VertHaveLipaiList.Count == 0)
            return;

        if(m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
        {
            m_LipaiTfm.GetComponent<HorizontalLayoutGroup>().enabled = true;

            int end = m_HaveLipaiList.Count - 1;
            bool bLast = (end != 0 && index == end);
            if (addToList)
                m_HavePokerList.AddRange(new List<byte>(m_HaveLipaiList[index]));
            m_HaveLipaiList.RemoveAt(index);
            if (bLast)
            {
                RectTransform rtfm = m_LipaiTfm.GetChild(end - 1) as RectTransform;
                rtfm.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtfm.rect.width + 100);
            }
            GameObject.DestroyImmediate(m_LipaiTfm.GetChild(index).gameObject);

            GameMain.WaitForCall(-1f, () =>
            {
                m_LipaiTfm.GetComponent<HorizontalLayoutGroup>().enabled = false;
            });
        }
        else
        {
            if(index < m_HaveLipaiList.Count)
            {
                if (addToList)
                    m_HavePokerList.AddRange(new List<byte>(m_HaveLipaiList[index]));
                m_HaveLipaiList.RemoveAt(index);
                GameObject.DestroyImmediate(m_LipaiTfm.GetChild(index).gameObject);
            }
            else
            {
                index -= m_HaveLipaiList.Count;
                if (addToList)
                    m_HavePokerList.AddRange(new List<byte>(m_VertHaveLipaiList[index]));
                m_VertHaveLipaiList.RemoveAt(index);
                GameObject.DestroyImmediate(m_VertLipaiTfm.GetChild(index).gameObject);
            }

            VertArrayPoker(false, false);
        }
    }

    void OnClickLipai(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        PlayCard card = (PlayCard)message;

        if (eventtype == EventTriggerType.PointerDown)
        {
            GetAllCards(true);

            PlayCard selPc = GetSelLipai();
            if (selPc != null && selPc != card)
                selPc.Selected(false);
            card.Selected(!card.Selected());

            m_LipaiBtns[1].gameObject.SetActive(card.Selected());

            UpdateGameBtn();
        }
    }

    void OnClickVertLipai(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        PlayCard card = (PlayCard)message;

        if (eventtype == EventTriggerType.PointerDown)
        {
            GetAllCards(true);

            PlayCard selPc = GetSelLipai();
            if (selPc != null && selPc != card)
                selPc.Masked = false;
            card.Masked = !card.Masked;

            m_LipaiBtns[1].gameObject.SetActive(card.Masked);

            UpdateGameBtn();
        }
    }

    void OnClickReArrayBtn(ePokerArrayType index, bool bAnim = true, bool audio = true)
    {
        if(audio)
            CustomAudioDataManager.GetInstance().PlayAudio(1005);

        UpdateCards();

        Transform tfm = m_ArrayBtnsTfm.Find("zu");
        ePokerArrayType next = index + 1;
        if (next >= ePokerArrayType.ePAT_Num)
            next = ePokerArrayType.ePAT_Bomb;
        tfm.GetChild((int)index).gameObject.SetActive(false);
        tfm = tfm.GetChild((int)next);
        tfm.gameObject.SetActive(true);
        Button btn = tfm.GetComponent<Button>();

        m_eCurArrayType = (ePokerArrayType)index;

        switch (m_eCurArrayType)
        {
            case ePokerArrayType.ePAT_Bomb://炸弹
                {
                    btn.interactable = false;

                    List<byte> haveList = new List<byte>(m_HavePokerList);
                    haveList.Sort(LL_PokerType.SortByIndex);
                    haveList.Reverse();

                    List<byte> result = new List<byte>();

                    //检测天炸
                    List<byte> check = haveList.FindAll(s => s >= 0x41);
                    if(check.Count == 4)
                    {
                        result.AddRange(check);
                        foreach (byte poker in check)
                            haveList.Remove(poker);
                    }

                    //检测5个（含）以上的炸
                    int num = 8;
                    for (; num > 5; num--)
                    {
                        var groupNum = haveList.GroupBy(p => (byte)GameCommon.GetCardValue(p)).Where(g => (g.Count() == num));
                        foreach (var g in groupNum)
                        {
                            foreach(var v in g)
                            {
                                result.Add(v);
                                haveList.Remove(v);
                            }
                        }
                    }

                    Dictionary<byte, List<List<byte>>> array = new Dictionary<byte, List<List<byte>>>();
                    ComputeTongHuaArray(haveList, ref array, true);
                    List<byte> temp;
                    while(array.Count > 0)
                    {
                        temp = array.FirstOrDefault().Value.FirstOrDefault();
                        result.AddRange(temp);
                        foreach (var v in temp)
                        {
                            haveList.Remove(v);
                        }

                        ComputeTongHuaArray(haveList, ref array, true);
                    }

                    byte laizi = LL_PokerType.GetLaiziValue();
                    List<byte> laiziList, noLaiziHaveList;

                    for(; num > 3; num--)
                    {
                        laiziList = haveList.FindAll(s => s == laizi);
                        noLaiziHaveList = new List<byte>(haveList);
                        noLaiziHaveList.RemoveAll(s => s == laizi);
                        var group = noLaiziHaveList.GroupBy(p => (byte)LL_PokerType.GetPokerLogicValue(p)).Where(r => ((r.Count() + laiziList.Count >= num) && r.Key != laizi));

                        while (group.Count() > 0)
                        {
                            var g = group.FirstOrDefault();
                            for (int k = 0; k < (num - g.Count()); k++)
                            {
                                result.Add(laizi);
                                haveList.Remove(laizi);
                            }

                            foreach (var v in g)
                            {
                                result.Add(v);
                                haveList.Remove(v);
                            }


                            laiziList = haveList.FindAll(s => s == laizi);
                            noLaiziHaveList = new List<byte>(haveList);
                            noLaiziHaveList.RemoveAll(s => s == laizi);
                            group = noLaiziHaveList.GroupBy(p => (byte)LL_PokerType.GetPokerLogicValue(p)).Where(r => ((r.Count() + laiziList.Count >= num) && r.Key != laizi));
                        }
                    }

                    result.AddRange(haveList);

                    m_HavePokerList = result;

                    GameMain.SC(ReArrayPoker(bAnim, () => btn.interactable = true));
                }
                break;
            case ePokerArrayType.ePAT_Color://花色
                {
                    btn.interactable = false;
                    m_HavePokerList.Sort(LL_PokerType.SortByColor);
                    GameMain.SC(ReArrayPoker(bAnim, () => btn.interactable = true));
                }
                break;
            case ePokerArrayType.ePAT_Index://大小
                {
                    btn.interactable = false;
                    m_HavePokerList.Sort(LL_PokerType.SortByIndex);
                    m_HavePokerList.Reverse();
                    GameMain.SC(ReArrayPoker(bAnim, () => btn.interactable = true));
                }
                break;
            default:
                break;
        }
    }

    List<byte> ComputeTongHuaArray(List<byte> havePokerList, ref Dictionary<byte, List<List<byte>>> tongHuaArray, bool bFindOne = false)
    {
        List<byte> biggestList = new List<byte>();
        tongHuaArray.Clear();

        //compute Laizi(heart run) num 
        byte laizi = LL_PokerType.GetLaiziValue();
        List<byte> laiziList = havePokerList.FindAll(s => s == laizi);
        int laiziCount = laiziList.Count;

        for (int i = 3; i >= 0; i--)//4种花色
        {
            List<byte> list = havePokerList.FindAll(s => (GameCommon.GetCardColor(s) == i && s != laizi));
            list = list.Distinct().ToList();
            if (list.Count + laiziCount < 5)
                continue;

            byte ace = (byte)((i << 4) + 1);
            byte last = (byte)(ace + 13);
            byte end = (byte)(ace + 4);
            byte biggest;
            for (byte j = last; j >= end; j--)
            {
                List<byte> flush = list.FindAll(s => ((s <= j && s >= (j - 4)) || (s == ace && j == last)));
                flush = flush.Distinct().ToList();
                if (flush.Count + laiziCount < 5)
                    continue;

                List<byte> temp = new List<byte>();
                int useLaiziCount = 0;
                bool find = true;
                biggest = (byte)GameCommon.GetCardValue(j);
                bool beginLaizi = true;
                byte offset = 0;
                for (byte k = j; k >= j - 4; k--)
                {
                    if (flush.Contains(k))
                    {
                        beginLaizi = false;
                        temp.Add(k);
                    }
                    else if(k == last && flush.Contains(ace))
                    {
                        beginLaizi = false;
                        temp.Add(ace);
                    }
                    else if (useLaiziCount < laiziCount)
                    {
                        temp.Add(laizi);
                        useLaiziCount++;

                        if (beginLaizi && !list.Contains((byte)(k - 5)))
                            offset++;
                    }
                    else
                        find = false;
                }

                if (!find)
                    continue;

                j -= offset;

                biggestList.Add(biggest);
                byte key = (byte)i;
                if(!tongHuaArray.ContainsKey(key))
                    tongHuaArray[key] = new List<List<byte>>();
                tongHuaArray[key].Add(temp);

                if (bFindOne)
                    return biggestList;
            }
        }

        return biggestList;
    }

    void Refresh(bool bAnim, bool begin = false)
    {
        if (m_SendPokerTfm.gameObject.activeSelf)//发牌过程中不刷新
            return;

        if (Rank == 0)
            ComputeTongHuaArray(m_HavePokerList, ref m_TongHuaArray);
        else
            m_TongHuaArray.Clear();
        m_TongHuaBtn[0].interactable = m_TongHuaArray.Count > 0;
        for (byte i = 0; i < 4; i++)
            m_TongHuaBtn[i + 1].interactable = m_TongHuaArray.ContainsKey(i);
        m_TipTongIndex = new KeyValuePair<byte, byte>(0,0);

        if (m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
            HorizArrayPoker(bAnim, m_eCurArrayType);
        else
            VertArrayPoker(bAnim, true, begin);
    }

    void HorizArrayPoker(bool bAnim, ePokerArrayType arrayType)
    {
        OnClickReArrayBtn(arrayType, bAnim, false);

        if (m_HaveLipaiList.Count > m_LipaiTfm.childCount)
        {
            m_LipaiTfm.GetComponent<HorizontalLayoutGroup>().enabled = true;

            RectTransform rtfm;
            if (m_LipaiTfm.childCount > 0)
            {
                GetSelLipai(true);

                rtfm = m_LipaiTfm.GetChild(m_LipaiTfm.childCount - 1) as RectTransform;
                rtfm.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rtfm.rect.width - 100);
            }

            byte laizi = LL_PokerType.GetLaiziValue();
            GameObject assetObj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("Shoupai_2_player1");
            GameObject obj;
            List<List<byte>> lipaiList = new List<List<byte>>(m_HaveLipaiList);
            lipaiList.RemoveRange(0, m_LipaiTfm.childCount);
            AssetBundle ab = m_GameBase.CommonAssetBundle;
            List<byte> list;
            int i = 0;
            float size = 0f;
            for (int j = 0; j < lipaiList.Count; j++)
            {
                list = lipaiList[j];
                obj = GameMain.Instantiate(assetObj);
                obj.transform.SetParent(m_LipaiTfm, false);

                Transform tfm = obj.transform.Find("ImagePoker");
                i = 0;
                bool bShowMask;
                foreach (Transform child in tfm)
                {
                    if (i < list.Count)
                    {
                        bShowMask = LL_PokerType.GetPokerLogicValue(list[i], laizi) == 15;
                        PlayCard.SetCardSprite(child.gameObject, ab, list[i], bShowMask, laizi, "_big");
                    }
                    else
                        child.gameObject.SetActive(false);

                    i++;
                }

                rtfm = obj.transform as RectTransform;
                size = 54 + 43 * (list.Count - 1);
                if (j == lipaiList.Count - 1)
                    size += 100;
                rtfm.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            }

            GameMain.WaitForCall(-1f, () =>
            {
                m_LipaiTfm.GetComponent<HorizontalLayoutGroup>().enabled = false;

                PlayCard card;
                foreach (Transform tfm in m_LipaiTfm)
                {
                    card = tfm.GetComponent<PlayCard>();
                    if (card == null)
                        card = tfm.gameObject.AddComponent<PlayCard>();
                    card.Masked = false;
                    XPointEvent.AutoAddListener(tfm.gameObject, OnClickLipai, card);
                }
            });
        }
    }

    void VertArrayPoker(bool bAnim, bool bModified, bool begin = false)
    {
        UpdateCards();

        GameMain.SC(ReArrayPoker(bAnim, () =>
        {
            byte laizi = LL_PokerType.GetLaiziValue();

            Dictionary<int, List<byte>> valueList = null;
            LL_PokerType.GetValueList(m_HavePokerList, out valueList);

            Transform tfm;
            GameObject child;
            bool bShowMask;
            PlayCard pc;
            int index;

            Transform handTfm = m_HaveCards.Find("Point_shoupai");

            if (bModified)
            {
                foreach (Transform childTfm in handTfm)
                    GameObject.Destroy(childTfm.gameObject);

                GameObject prefabObj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("Poker_shoupai_2");
                foreach (var pokerList in valueList.Values)
                {
                    tfm = GameMain.Instantiate(prefabObj).transform;
                    tfm.SetParent(handTfm, false);
                    tfm.SetSiblingIndex(0);

                    int i = 0;
                    for (; i < pokerList.Count; i++)
                    {
                        child = tfm.GetChild(i).gameObject;
                        child.SetActive(true);

                        pc = child.AddComponent<PlayCard>();
                        XPointEvent.AutoAddListener(child, OnClickCards, pc);

                        bShowMask = LL_PokerType.GetPokerLogicValue(pokerList[i], laizi) == 15;
                        pc.SetCardSprite(m_GameBase.CommonAssetBundle, pokerList[i], bShowMask, laizi, "_wide");
                        pc.Masked = (Rank > 0);

                        if (m_fVertCardWidth == 0.0f)
                        {
                            Rect rect = GameFunction.GetSpaceRect(m_GameBase.GameCanvas, child.transform as RectTransform, m_GameBase.GameCanvas.worldCamera);
                            m_fVertCardWidth = rect.width;
                        }
                    }

                    for (; i < tfm.childCount; i++)
                        tfm.GetChild(i).gameObject.SetActive(false);
                }
            }

            if (m_HaveLipaiList.Count > m_LipaiTfm.childCount)
            {
                GameObject assetObj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("Shoupai_vertical");
                List<List<byte>> lipaiList = new List<List<byte>>(m_HaveLipaiList);
                lipaiList.RemoveRange(m_LipaiTfm.childCount, m_LipaiTfm.childCount);
                foreach (var pokerList in lipaiList)
                {
                    tfm = GameMain.Instantiate(assetObj).transform;
                    tfm.SetParent(m_LipaiTfm, false);

                    tfm = tfm.Find("ImagePoker");
                    index = 0;
                    for (; index < pokerList.Count; index++)
                    {
                        child = tfm.GetChild(index).gameObject;
                        child.SetActive(true);
                        bShowMask = LL_PokerType.GetPokerLogicValue(pokerList[index], laizi) == 15;
                        PlayCard.SetCardSprite(child, m_GameBase.CommonAssetBundle, pokerList[index], bShowMask, laizi, "_wide");
                    }

                    for (; index < tfm.childCount; index++)
                        tfm.GetChild(index).gameObject.SetActive(false);
                }

                GameMain.WaitForCall(-1f, () =>
                {
                    PlayCard card;
                    RectTransform rtfm;
                    foreach (Transform childTfm in m_LipaiTfm)
                    {
                        card = childTfm.GetComponent<PlayCard>();
                        if (card == null)
                        {
                            card = childTfm.gameObject.AddComponent<PlayCard>();
                            XPointEvent.AutoAddListener(childTfm.gameObject, OnClickVertLipai, card);
                        }
                        card.Masked = Rank > 0;

                        rtfm = childTfm.Find("ImagePoker") as RectTransform;
                        (childTfm as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtfm.sizeDelta.y);
                    }
                });
            }

            if (m_VertHaveLipaiList.Count > m_VertLipaiTfm.childCount)
            {
                GameObject assetObj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("Shoupai_vertical");
                List<List<byte>> lipaiList = new List<List<byte>>(m_VertHaveLipaiList);
                lipaiList.RemoveRange(0, m_VertLipaiTfm.childCount);
                foreach (var pokerList in lipaiList)
                {
                    tfm = GameMain.Instantiate(assetObj).transform;
                    tfm.SetParent(m_VertLipaiTfm, false);

                    tfm = tfm.Find("ImagePoker");
                    index = 0;
                    for (; index < pokerList.Count; index++)
                    {
                        child = tfm.GetChild(index).gameObject;
                        child.SetActive(true);
                        bShowMask = LL_PokerType.GetPokerLogicValue(pokerList[index], laizi) == 15;
                        PlayCard.SetCardSprite(child, m_GameBase.CommonAssetBundle, pokerList[index], bShowMask, laizi, "_wide");
                    }

                    for (; index < tfm.childCount; index++)
                        tfm.GetChild(index).gameObject.SetActive(false);
                }

                GameMain.WaitForCall(-1f, () =>
                {
                    PlayCard card;
                    RectTransform rtfm;
                    foreach (Transform childTfm in m_LipaiTfm)
                    {
                        card = childTfm.GetComponent<PlayCard>();
                        if (card == null)
                        {
                            card = childTfm.gameObject.AddComponent<PlayCard>();
                            XPointEvent.AutoAddListener(childTfm.gameObject, OnClickVertLipai, card);
                        }
                        card.Masked = Rank > 0;

                        rtfm = childTfm.Find("ImagePoker") as RectTransform;
                        (childTfm as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtfm.sizeDelta.y);
                    }

                    foreach (Transform childTfm in m_VertLipaiTfm)
                    {
                        card = childTfm.GetComponent<PlayCard>();
                        if (card == null)
                        {
                            card = childTfm.gameObject.AddComponent<PlayCard>();
                            XPointEvent.AutoAddListener(childTfm.gameObject, OnClickVertLipai, card);
                        }
                        card.Masked = Rank > 0;

                        rtfm = childTfm.Find("ImagePoker") as RectTransform;
                        (childTfm as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rtfm.sizeDelta.y);
                    }

                });
            }

            RectTransform rootRtfm = m_GameBase.GameCanvas.transform.Find("Root") as RectTransform;
            float width = Screen.width + rootRtfm.sizeDelta.x;
            int count = valueList.Count + m_HaveLipaiList.Count + m_VertHaveLipaiList.Count;
            float spacing = m_fVertCardWidth - width / count;
            spacing = spacing + spacing / count;
            if (spacing > 0f)
                spacing = -spacing;
            else
                spacing = 0;
            //spacing -= 30f;
            m_HaveCards.GetComponent<HorizontalLayoutGroup>().spacing = spacing;
            handTfm.GetComponent<HorizontalLayoutGroup>().spacing = spacing;
            m_LipaiTfm.GetComponent<HorizontalLayoutGroup>().spacing = spacing;
            m_VertLipaiTfm.GetComponent<HorizontalLayoutGroup>().spacing = spacing;

            if (begin)
                OnVertSelectedChanged();
        }, true));
    }

    public override void RemoveCards(byte[] cards)
    {
        if(m_nCurLipaiIndex < 0)
        {
            List<byte> noInList = new List<byte>();

            if (m_GameBase.Bystander)
            {
                m_HavePokerList.RemoveRange(0, cards.Length);
            }
            else
            {
                foreach (byte card in cards)
                {
                    if (!m_HavePokerList.Remove(card))
                        noInList.Add(card);
                }
            }

            List<byte> clone = new List<byte>(noInList);
            for (int j = m_VertHaveLipaiList.Count - 1; j >= 0; j--)
            {
                foreach(byte card in clone)
                {
                    if (m_VertHaveLipaiList[j].Exists(s => s == card))
                    {
                        foreach (byte poker in m_VertHaveLipaiList[j])
                            clone.Remove(poker);

                        RemoveLipai(j + m_HaveLipaiList.Count, true);
                        break;
                    }
                }
            }

            for (int j = m_HaveLipaiList.Count - 1; j >= 0; j--)
            {
                foreach (byte card in clone)
                {
                    if (m_HaveLipaiList[j].Exists(s => s == card))
                    {
                        foreach (byte poker in m_HaveLipaiList[j])
                            clone.Remove(poker);
                        RemoveLipai(j, true);
                        break;
                    }
                }
            }

            foreach (byte card in noInList)
            {
                m_HavePokerList.Remove(card);
            }

            Refresh(false);
        }
        else
        {
            if(m_nCurLipaiIndex < m_HaveLipaiList.Count)
            {
                if (!m_HaveLipaiList[m_nCurLipaiIndex].SequenceEqual(cards))
                {
                    DebugLog.LogError("remove lipai error : No.(" + m_nCurLipaiIndex + ") poker value not match");

                    GetSelLipai(true);
                    m_nCurLipaiIndex = -1;

                    return;
                }
            }
            else
            {
                if (!m_VertHaveLipaiList[m_nCurLipaiIndex - m_HaveLipaiList.Count].SequenceEqual(cards))
                {
                    DebugLog.LogError("remove lipai error : No.(" + m_nCurLipaiIndex + ") poker value not match");

                    GetSelLipai(true);
                    m_nCurLipaiIndex = -1;

                    return;
                }
            }

            RemoveLipai(m_nCurLipaiIndex, false);
            m_nCurLipaiIndex = -1;
        }
    }

    public override void OnDealPoker(sbyte haveRight = -1, byte[] cards = null, GuandanPokerType_Enum pokerType = GuandanPokerType_Enum.GuanPokerType_Error, byte rank = 0)
    {
        base.OnDealPoker(haveRight, cards, pokerType, rank);

        m_AblePokerArray.Clear();
        m_TipPokerIndex = 0;
        m_LipaiBtns[1].gameObject.SetActive(false);
        m_nCurLipaiIndex = -1;
        m_fAlarmTimer = -1f;
        m_NoBigTextObj.SetActive(false);

        if (haveRight >= 0)
        {
            ShowGameBtn(false);
            UpdateCards();
        }
    }

    void ShowGameBtn(bool show, int flag = -1, bool activeOrEnableChild = true)
    {
        if (m_GameBtnsTfm == null)
            return;

        if(!show)
        {
            m_GameBtnsTfm.gameObject.SetActive(false);
            return;
        }

        m_GameBtnsTfm.gameObject.SetActive(true);

        Transform child;
        if (flag < 0)
        {
            for (int j = 0; j < m_GameBtnsTfm.childCount; j++)
            {
                child = m_GameBtnsTfm.GetChild(j);
                child.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            if (activeOrEnableChild)
            {
                for (int j = 0; j < m_GameBtnsTfm.childCount; j++)
                {
                    child = m_GameBtnsTfm.GetChild(j);
                    child.GetComponent<Button>().interactable = true;
                    child.gameObject.SetActive(GameKind.HasFlag(j, flag));
                }
            }
            else
            {
                for (int j = 0; j < m_GameBtnsTfm.childCount; j++)
                {
                    m_GameBtnsTfm.GetChild(j).GetComponent<Button>().interactable = GameKind.HasFlag(j, flag);
                }
            }
        }
    }

    void GetAllAblePokerArray()
    {
        m_AblePokerArray.Clear();
        m_TipPokerIndex = 0;

        List<List<byte>> haveBombArray = new List<List<byte>>();
        List<List<byte>> haveAbleArray = new List<List<byte>>();

        //1 看整理牌的组合是否可行
        List<byte> haveList = new List<byte>(m_HavePokerList);
        List<byte> childList = new List<byte>();
        List<List<byte>> lipaiList = new List<List<byte>>(m_HaveLipaiList);
        if (m_GameBase.CurPOT == PokerSortType.ePST_Vertical)
        {
            lipaiList.Reverse();
            lipaiList.InsertRange(0, m_VertHaveLipaiList);
        }
        foreach (List<byte> list in lipaiList)
        {
            haveList.AddRange(new List<byte>(list));

            GuandanPokerType_Enum type = m_GameBase.GetPokersType(list, m_PlayPokerList, CurPokerType, ref childList);
            if (type >= GuandanPokerType_Enum.GuanPokerType_Blast)
                haveBombArray.Add(new List<byte>(list));
            else if (type != GuandanPokerType_Enum.GuanPokerType_Error)
                haveAbleArray.Add(new List<byte>(list));
        }

        //2 找出手牌中的所有可行组合
        List<List<byte>> bombArray;
        List<List<byte>> AbleArray = ComputeAblePokerArray(m_HavePokerList, m_PlayPokerList, CurPokerType, out bombArray);
        haveAbleArray.AddRange(AbleArray);
        haveBombArray.AddRange(bombArray);

        //3 找出所有牌的可行组合
        List<List<byte>> allBombArray;
        List<List<byte>> allAbleArray = ComputeAblePokerArray(haveList, m_PlayPokerList, CurPokerType, out allBombArray);
        //4 剔除前面已找到的组合
        byte laizi = LL_PokerType.GetLaiziValue();
        foreach(List<byte> list1 in haveAbleArray)
        {
            foreach(List<byte> list2 in allAbleArray)
            {
                //如果牌值都一样，可以剔除
                if(list1.All(b => list2.Any(a => LL_PokerType.GetPokerLogicValue(a, laizi) == LL_PokerType.GetPokerLogicValue(b, laizi))))
                {
                    allAbleArray.Remove(list2);
                    break;
                }
            }
        }
        foreach (List<byte> list1 in haveBombArray)
        {
            foreach (List<byte> list2 in allBombArray)
            {
                //如果牌值都一样，可以剔除
                if (list1.All(b => list2.Any(a => LL_PokerType.GetPokerLogicValue(a, laizi) == LL_PokerType.GetPokerLogicValue(b, laizi))))
                {
                    allBombArray.Remove(list2);
                    break;
                }
            }
        }
        //5 按顺序存储以上组合
        m_AblePokerArray = haveAbleArray;
        m_AblePokerArray.AddRange(allAbleArray);
        m_AblePokerArray.AddRange(haveBombArray);
        m_AblePokerArray.AddRange(allBombArray);
    }

    public override void OnAskDealPoker(bool bHaveRight, List<byte> curPokerList, GuandanPokerType_Enum pokerType, float askTime, float bankTime, bool pause)
    {
        base.OnAskDealPoker(bHaveRight, curPokerList, pokerType, askTime, bankTime, pause);

        m_bAnswered = false;
        int flag = 0;
        if (bHaveRight || m_GameBase.Bystander)
        {
            GameKind.AddFlag(2, ref flag);//出牌

            m_PlayPokerList.Clear();
            CurPokerType = GuandanPokerType_Enum.GuanPokerType_Error;
        }
        else
        {
            m_PlayPokerList = curPokerList;
            CurPokerType = pokerType;
            GetAllAblePokerArray();

            if (m_AblePokerArray.Count == 0)
            {
                GameKind.AddFlag(3, ref flag);//要不起
                m_NoBigTextObj.SetActive(true);
            }
            else
            {
                GameKind.AddFlag(0, ref flag);//不出
                GameKind.AddFlag(1, ref flag);//提示
                GameKind.AddFlag(2, ref flag);//出牌
            }
        }

        ShowGameBtn(true, flag);

        UpdateGameBtn();

        m_fAlarmTimer = GuanDan_Data.GetInstance().m_RoomData.m_fOutPokerTime;
    }

    void CanOutPoker(int selectNum, int unselectNum)
    {
        if (m_bAnswered)
            return;

        List<byte> list;
        GetPrePokers(out list, true);

        List<byte> validList = new List<byte>();
        GuandanPokerType_Enum res = m_GameBase.GetPokersType(list, m_PlayPokerList, CurPokerType, ref validList);
        if (res == GuandanPokerType_Enum.GuanPokerType_Error)
        {
            if(!m_OutPokerBtn.interactable)//如果之前已经选好了就不再智能选取了
            {
                if (selectNum > unselectNum)
                {
                    if (SmartSelectCard(list))
                    {
                        m_OutPokerBtn.interactable = true;
                        return;
                    }
                }
            }

            m_OutPokerBtn.interactable = false;
            return;
        }

        m_OutPokerBtn.interactable = true;

        if (validList.Count > 0)
        {
            if (selectNum > validList.Count)
                UpdateCards(validList);
            else if (list.Count != validList.Count)
                m_OutPokerBtn.interactable = false;
        }
    }

    public void OnAnswerPoker(byte index, Button btn)// 0:不出 1:提示 2:出牌 3:要不起
    {
        if (index == 1)
        {
            if (m_AblePokerArray.Count > 0)
            {
                UpdateCards(m_AblePokerArray[m_TipPokerIndex]);

                m_TipPokerIndex++;
                if (m_TipPokerIndex == m_AblePokerArray.Count)
                    m_TipPokerIndex = 0;

                if (btn != null)
                {
                    btn.interactable = false;
                    GameMain.WaitForCall(0.5f, () => btn.interactable = true);
                }
            }
        }
        else if (index == 2)
        {
            List<byte> list;
            m_nCurLipaiIndex = GetPrePokers(out list, true);
            if (list.Count == 0)
                OnDealPokerFailed();
            else
            {
                List<List<byte>> childList = null;
                List<GuandanPokerType_Enum> typeList = null;
                if (CheckMultiplePokerType(list, ref childList, ref typeList))
                {
                    m_OutPokerBtn.interactable = false;
                    m_ChoosePokerTypeTfm.gameObject.SetActive(true);
                    Transform tfm = m_ChoosePokerTypeTfm.Find("ImageBG");
                    int i = 0, j = 0;
                    Button butn;
                    foreach(Transform child in tfm)
                    {
                        if (i < childList.Count)
                        {
                            j = 0;
                            foreach(Transform poker in child.Find("ImagePoker"))
                            {
                                if (j < childList[i].Count)
                                    PlayCard.SetCardSprite(poker.gameObject, m_GameBase.CommonAssetBundle, childList[i][j], false);
                                else
                                    poker.gameObject.SetActive(false);
                                j++;
                            }

                            butn = child.GetComponent<Button>();
                            butn.onClick.RemoveAllListeners();
                            List<byte> temp = childList[i];
                            GuandanPokerType_Enum type = typeList[i];
                            butn.onClick.AddListener(() =>
                            {
                                SendOutPoker(temp, type, btn);
                            });
                        }
                        else
                            child.gameObject.SetActive(false);
                        i++;
                    }
                }
                else
                    SendOutPoker(list, GuandanPokerType_Enum.GuanPokerType_Error, btn);
            }
        }
        else
        {
            SendOutPoker(new List<byte>(), GuandanPokerType_Enum.GuanPokerType_Error, btn);
        }
    }

    bool CheckMultiplePokerType(List<byte> list, ref List<List<byte>> childList, ref List<GuandanPokerType_Enum> typeList)
    {
        if(CurPokerType != GuandanPokerType_Enum.GuanPokerType_Error)
            return false;

        if(list.Count != 6)
            return false;

        byte laizi = LL_PokerType.GetLaiziValue();
        Dictionary<int, List<byte>> valueList;
        LL_PokerType.GetValueList(list, out valueList, laizi);

        int laiziCount = 0;
        if (valueList.ContainsKey(-1))
            laiziCount = valueList[-1].Count;

        if (laiziCount < 2)
            return false;

        int biggest = 0;

        List<byte> childList1 = new List<byte>();
        if (LL_PokerType.IsPlaneNoWith(valueList, ref childList1, ref biggest, true) == 0)
            return false;

        List<byte> childList2 = new List<byte>();
        if (LL_PokerType.IsSeriesPairs(valueList, ref childList2, ref biggest, true) == 0)
            return false;

        childList = new List<List<byte>>();
        childList.Add(childList1);
        childList.Add(childList2);
        typeList = new List<GuandanPokerType_Enum>();
        typeList.Add(GuandanPokerType_Enum.GuanPokerType_SeriesThree);
        typeList.Add(GuandanPokerType_Enum.GuanPokerType_SeriesPairs);
        return true;
    }

    void SendOutPoker(List<byte> list, GuandanPokerType_Enum pokerType, Button btn)
    {
        m_bAnswered = true;

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_CM_OUTPOKERS);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((byte)pokerType);
        msg.Add((byte)list.Count);
        foreach (byte card in list)
            msg.Add(card);
        HallMain.SendMsgToRoomSer(msg);

        if (btn != null)
            btn.interactable = false;
    }

    bool HandleNetMsg(uint _msgType, UMessage _ms)
    {
        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;
        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_OUTPOKERS:
                {
                    byte nState = _ms.ReadByte();

                    OnDealPokerFailed(nState);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_GUANDAN_SM_FRIENDPOKERS:
                {
                    m_HavePokerList.Clear();
                    byte friendSit = _ms.ReadByte();
                    byte num = _ms.ReadByte();
                    for(int i = 0; i < num; i++)
                        m_HavePokerList.Add(_ms.ReadByte());

                    if (m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
                        HorizArrayPoker(false, ePokerArrayType.ePAT_Index);
                    else
                        VertArrayPoker(false, true);
                }
                break;

            default:
                break;
        }

        return true;
    }

    //0:没有选牌 1：房间不是出牌的状态 2：不是轮到你出牌 3：出的牌手上没有 4：上次是你出的牌 没人要  你自己不能不要 5：牌型不对
    void OnDealPokerFailed(byte nState = 0)
    {
        DebugLog.Log("Deal poker failed: " + nState);
        m_bAnswered = false;

        bool show = true;
        if (nState == 0)
            CRollTextUI.Instance.AddVerticalRollText(2306);
        else if (nState < 4)
        {
            CRollTextUI.Instance.AddVerticalRollText(2309);
            UpdateCards();
            show = false;
        }
        else
        {
            CRollTextUI.Instance.AddVerticalRollText(2304);
            UpdateCards();
        }

        ShowGameBtn(show);
        m_OutPokerBtn.interactable = false;
    }

    void ShowGongBtn(bool show, bool submitOrReturn = true)
    {
        m_GongBtnsTfm.gameObject.SetActive(show);
        if (show)
        {
            m_GongBtnsTfm.Find("Button_shanggong").gameObject.SetActive(submitOrReturn);
            m_GongBtnsTfm.Find("Button_huangong").gameObject.SetActive(!submitOrReturn);
            UpdateGameBtn();
        }
    }

    void OnClickGongBtn(byte index, Button btn)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1005);

        btn.gameObject.SetActive(false);

        List<byte> list;
        GetPrePokers(out list);

        if (index == 0)//上贡
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_CM_SUBMITPOKER);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(list[0]);
            HallMain.SendMsgToRoomSer(msg);
        }
        else//还贡
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GUANDAN_CM_RETURNPOKER);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(list[0]);
            HallMain.SendMsgToRoomSer(msg);
        }
    }

    public override void AddCards(byte[] cards)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1004);

        if (m_GameBase.Bystander)
        {
            for (int i = 0; i < cards.Length; i++)
                m_HavePokerList.Add(RoomInfo.NoSit);
        }
        else
        {
            m_HavePokerList.AddRange(new List<byte>(cards));
        }

        Refresh(false);
        //UpdateCards(new List<byte>(cards));
    }

    public override void OnGong(byte state, bool submitOrReturn, byte poker, Transform srcTfm, bool bReverse)
    {
        base.OnGong(state, submitOrReturn, poker, srcTfm);

        ShowGongBtn(state == 0 && !bReverse, submitOrReturn);
    }

    bool SmartSelectCard(List<byte> list)
    {
        if (list.Count == 0 || CurPokerType == GuandanPokerType_Enum.GuanPokerType_Single)
            return false;

        byte laizi = LL_PokerType.GetLaiziValue();
        if (list.Count == 1 && list[0] == laizi)
            return false;

        Dictionary<int, List<byte>> valueList;
        LL_PokerType.GetValueList(list, out valueList, laizi);

        List<byte> childValidList = new List<byte>();
        int biggest = 0;

        if (CurPokerType == GuandanPokerType_Enum.GuanPokerType_Error)//有牌权
        {
            if(list.Count > 5 && LL_PokerType.IsFlush(valueList, ref childValidList, ref biggest, true, 5) != 0)
            {
                UpdateCards(childValidList);
                return true;
            }

            return false;
        }

        if (m_AblePokerArray.Count == 0)
            return false;

        bool find;
        bool canChange, canMore;

        foreach (List<byte> ableList in m_AblePokerArray)
        {
            canMore = (CurPokerType == GuandanPokerType_Enum.GuanPokerType_Flush);

            Dictionary<int, List<byte>> ableValueList;
            LL_PokerType.GetValueList(ableList, out ableValueList, laizi);

            canChange = true;
            if (LL_PokerType.IsStraightFlush(ableValueList, ref childValidList, ref biggest, true, 5) != 0)
                canChange = false;

            List<byte> resList = new List<byte>(ableList);

            find = true;
            if (list.Count > ableList.Count)
            {
                if (!canMore)
                    continue;

                foreach(byte poker in ableList)
                {
                    if(!list.Contains(poker))
                    {
                        if (!canChange)
                        {
                            find = false;
                            break;
                        }

                        int value = LL_PokerType.GetPokerLogicValue(poker, laizi);
                        if(!valueList.ContainsKey(value))
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
                        array.Value.Count > ableValueList[array.Key].Count ||
                        (!canChange && array.Value.SequenceEqual(ableValueList[array.Key])))
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
                    for (int i = 0; i < count; i++)
                    {
                        resList.Add(testList[i]);
                    }
                }
            }

            if (find)
            {
                List<byte> childList = new List<byte>();
                GuandanPokerType_Enum type = CanLipai(resList, ref childList, true);
                if (type != CurPokerType)
                    return false;

                UpdateCards(resList);
                return true;
            }
        }

        return false;
    }

    List<List<byte>> ComputeAblePokerArray(List<byte> haveList, List<byte> curPokerList, GuandanPokerType_Enum curType, out List<List<byte>> bombList)
    {
        List<List<byte>> AblePokerArray = new List<List<byte>>();
        bombList = new List<List<byte>>();

        if (curPokerList == null || curPokerList.Count == 0)            return AblePokerArray;
        if (curType == GuandanPokerType_Enum.GuanPokerType_Error
            || curType == GuandanPokerType_Enum.GuanPokerType_KingBlast)
            return AblePokerArray;

        byte laizi = LL_PokerType.GetLaiziValue();

        Dictionary<int, List<byte>> curValueList;
        LL_PokerType.GetValueList(curPokerList, out curValueList, laizi);

        Dictionary<int, List<byte>> myValueList;
        LL_PokerType.GetValueList(haveList, out myValueList, laizi);

        int laiziCount = 0;
        int nowLaiziCount = 0;

        if (myValueList.ContainsKey(-1))
        {
            laiziCount = myValueList[-1].Count;
            myValueList.Remove(-1);
        }

        int arrayNum = 7;
        List<List<byte>>[] splitPokerArray = new List<List<byte>>[arrayNum];
        for (int i = 0; i < arrayNum; i++)
            splitPokerArray[i] = new List<List<byte>>();

        int typeCount = 0, typeNum = 0;
        int curMinValue = 0;
        List<byte> pokerList = new List<byte>();
        List<byte> curChildValidList = new List<byte>();

        if (haveList.Count >= curPokerList.Count)
        {
            if (curType <= GuandanPokerType_Enum.GuanPokerType_Three)
            {
                curMinValue = curValueList.Keys.ToList().Find(s => s > 0);
                if (curMinValue == 0)//只有赖子的情况
                    curMinValue = 15;
                typeCount = (int)curType;
                foreach (var poker in myValueList)
                {
                    if (poker.Key > curMinValue)
                    {
                        if (poker.Value.Count == typeCount)
                            AblePokerArray.Add(new List<byte>(poker.Value));
                        else
                        {
                            if(poker.Value.Count > typeCount)
                            {
                                byte[] temp = new byte[typeCount];
                                poker.Value.CopyTo(0, temp, 0, typeCount);
                                splitPokerArray[0].Add(new List<byte>(temp));
                            }
                            else if (poker.Key < 16)
                            {
                                if(poker.Value.Count + laiziCount >= typeCount)
                                {
                                    pokerList = new List<byte>(poker.Value);
                                    for (int i = 0; i < typeCount - poker.Value.Count; i++)
                                        pokerList.Add(laizi);
                                    splitPokerArray[1].Add(pokerList);
                                }
                            }
                        }
                    }
                }

                if(curMinValue < 15 && laiziCount >= typeCount)
                {
                    pokerList.Clear();
                    for (int i = 0; i < typeCount; i++)
                        pokerList.Add(laizi);
                    AblePokerArray.Add(pokerList);
                }
            }
            else if(curType == GuandanPokerType_Enum.GuanPokerType_Flush
                || curType == GuandanPokerType_Enum.GuanPokerType_SeriesPairs
                || curType == GuandanPokerType_Enum.GuanPokerType_SeriesThree)
            {
                if (myValueList.ContainsKey(15))//改成用原始值
                {
                    List<byte> temp = new List<byte>(myValueList[15]);
                    myValueList.Remove(15);
                    myValueList[LL_PokerType.CurGrade] = temp;

                    myValueList = myValueList.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
                }
                
                if(curType == GuandanPokerType_Enum.GuanPokerType_Flush)
                {
                    typeCount = 1;
                    LL_PokerType.IsFlush(curValueList, ref curChildValidList, ref curMinValue, true, 5);
                    curMinValue -= 4;
                }
                else if (curType == GuandanPokerType_Enum.GuanPokerType_SeriesPairs)
                {
                    typeCount = 2;
                    LL_PokerType.IsSeriesPairs(curValueList, ref curChildValidList, ref curMinValue, true);
                    curMinValue -= 2;
                }
                else
                {
                    typeCount = 3;
                    LL_PokerType.IsPlaneNoWith(curValueList, ref curChildValidList, ref curMinValue, true);
                    curMinValue -= 1;
                }

                typeNum = curPokerList.Count / typeCount;
                int count;
                foreach (int pokerValue in myValueList.Keys)
                {
                    if (pokerValue + typeNum > 15)//最大到A
                        break;

                    if (pokerValue > curMinValue)
                    {
                        nowLaiziCount = laiziCount;
                        pokerList.Clear();
                        bool find = true;
                        List<byte> andPokerList = new List<byte>();
                        for (int i = pokerValue; i < pokerValue + typeNum; i++)
                        {
                            count = 0;
                            if (myValueList.ContainsKey(i))
                                count = myValueList[i].Count;

                            if (count + nowLaiziCount < typeCount)
                            {
                                find = false;
                                break;
                            }

                            byte[] temp = new byte[typeCount];
                            if(count >= typeCount)
                                myValueList[i].CopyTo(0, temp, 0, typeCount);
                            else
                            {
                                if(count > 0)
                                    myValueList[i].CopyTo(temp);
                                int num = typeCount - count;
                                nowLaiziCount -= num;
                                for (int j = 0; j < num; j++)
                                    temp[count + j] = laizi;                      
                            }

                            if (count == typeCount)
                                pokerList.AddRange(new List<byte>(temp));
                            andPokerList.AddRange(temp);
                        }

                        if (find)
                        {
                            if (pokerList.Count == curPokerList.Count)
                                AblePokerArray.Add(new List<byte>(pokerList));
                            else
                                splitPokerArray[0].Add(andPokerList);
                        }
                    }
                }
            }
            else if(curType == GuandanPokerType_Enum.GuanPokerType_ThreeAndTwo)
            {
                typeCount = 3;
                int andTypeCount = 2;
                int andCount = 1;

                LL_PokerType.IsThreeAndTwo(curValueList, ref curChildValidList, ref curMinValue);

                foreach (var poker in myValueList)
                {
                    if (poker.Key > curMinValue && poker.Key < 16)
                    {
                        pokerList.Clear();
                        nowLaiziCount = laiziCount;
                        List<byte> laiziPokerList = new List<byte>();
                        if (poker.Value.Count <= typeCount && poker.Value.Count + nowLaiziCount >= typeCount)
                        {
                            if(poker.Value.Count == typeCount)
                                pokerList.AddRange(new List<byte>(poker.Value));
                            else
                            {
                                laiziPokerList.AddRange(new List<byte>(poker.Value));
                                for (int i = 0; i < typeCount - poker.Value.Count; i++)
                                {
                                    laiziPokerList.Add(laizi);
                                    nowLaiziCount -= 1;
                                }
                            }

                            Dictionary<int, List<byte>> testValueList = new Dictionary<int, List<byte>>();
                            Dictionary<int, List<byte>> laiziTestValueList = new Dictionary<int, List<byte>>();

                            foreach (int v in myValueList.Keys)
                            {
                                if (v != poker.Key && myValueList[v].Count <= typeCount && myValueList[v].Count + nowLaiziCount >= andTypeCount)
                                {
                                    if(myValueList[v].Count >= andTypeCount)
                                    {
                                        byte[] temp1 = new byte[andTypeCount];
                                        myValueList[v].CopyTo(0, temp1, 0, andTypeCount);
                                        List<byte> andPokerList = new List<byte>(temp1);
                                        testValueList.Add(v, andPokerList);
                                    }
                                    else if(v < 16)
                                    {
                                        List<byte> andPokerList = new List<byte>(myValueList[v]);
                                        for (int i = 0; i < andTypeCount - myValueList[v].Count; i++)
                                        {
                                            andPokerList.Add(laizi);
                                            nowLaiziCount -= 1;
                                        }
                                        laiziTestValueList.Add(v, andPokerList);
                                    }
                                }
                            }

                            if(pokerList.Count == typeCount)
                            {
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

                                        //if (temp.Count == curPokerList.Count)
                                        {
                                            if (bSplit)
                                                splitPokerArray[0].Add(temp);
                                            else
                                                AblePokerArray.Add(temp);
                                        }
                                    }
                                }

                                if (laiziTestValueList.Count >= andCount)
                                {
                                    List<int> testList = new List<int>(laiziTestValueList.Keys);
                                    for (int i = 0; i < testList.Count; i++)
                                    {
                                        List<byte> temp = new List<byte>(pokerList);
                                        for (int j = i; j < i + andCount && j < testList.Count; j++)
                                        {
                                            temp.AddRange(laiziTestValueList[testList[j]]);
                                        }

                                        //if (temp.Count == curPokerList.Count)
                                        {
                                            splitPokerArray[1].Add(temp);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (testValueList.Count >= andCount)
                                {
                                    List<int> testList = new List<int>(testValueList.Keys);
                                    bool bSplit;
                                    for (int i = 0; i < testList.Count; i++)
                                    {
                                        bSplit = false;
                                        List<byte> temp = new List<byte>(laiziPokerList);
                                        for (int j = i; j < i + andCount && j < testList.Count; j++)
                                        {
                                            if (myValueList[testList[j]].Count > andTypeCount)
                                                bSplit = true;
                                            temp.AddRange(testValueList[testList[j]]);
                                        }

                                        //if (temp.Count == curPokerList.Count)
                                        {
                                            if (bSplit)
                                                splitPokerArray[3].Add(temp);
                                            else
                                                splitPokerArray[2].Add(temp);
                                        }
                                    }
                                }

                                if (laiziTestValueList.Count >= andCount)
                                {
                                    List<int> testList = new List<int>(laiziTestValueList.Keys);
                                    for (int i = 0; i < testList.Count; i++)
                                    {
                                        List<byte> temp = new List<byte>(laiziPokerList);
                                        for (int j = i; j < i + andCount && j < testList.Count; j++)
                                        {
                                            temp.AddRange(laiziTestValueList[testList[j]]);
                                        }

                                        //if (temp.Count == curPokerList.Count)
                                        {
                                            splitPokerArray[4].Add(temp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        int nCurBombValue = 0;
        bool bCurBomb = LL_PokerType.IsNormalBomb(curValueList, ref curChildValidList, ref nCurBombValue) != 0;

        int nCurStraightValue = 0;
        bool bCurStraight = LL_PokerType.IsStraightFlush(curValueList, ref curChildValidList, ref nCurStraightValue, true, 5) != 0;
 
        int pokerNum = 4;

        if(!bCurStraight)
        {
            //查找4, 5张牌的炸弹
            for (; pokerNum < 6; pokerNum++)
            {
                foreach (var poker in myValueList)
                {
                    if (!bCurBomb || curPokerList.Count < pokerNum || (curPokerList.Count == pokerNum && poker.Key > nCurBombValue))
                    {
                        if (poker.Value.Count == pokerNum)
                        {
                            splitPokerArray[5].Add(new List<byte>(poker.Value));
                        }
                        else if (poker.Key < 16 && poker.Value.Count < pokerNum && poker.Value.Count + laiziCount >= pokerNum)
                        {
                            List<byte> list = new List<byte>(poker.Value);
                            for (int i = 0; i < pokerNum - poker.Value.Count; i++)
                                list.Add(laizi);
                            splitPokerArray[6].Add(list);
                        }
                    }
                }

                splitPokerArray[5].AddRange(splitPokerArray[6]);
                splitPokerArray[6].Clear();
            }
        }

        if(!bCurBomb || curPokerList.Count < 6)
        {
            Dictionary<byte, List<List<byte>>> tongArray = new Dictionary<byte, List<List<byte>>>();
            List<byte> biggestList = ComputeTongHuaArray(haveList, ref tongArray);
            int index = 0;
            foreach (List<List<byte>> list in tongArray.Values)
            {
                foreach (List<byte> childList in list)
                {
                    if (!bCurStraight || biggestList[index] > nCurStraightValue)
                    {
                        splitPokerArray[5].Add(childList);
                    }

                    index++;
                }
            }
        }

        //查找6张牌以上的炸弹
        pokerNum = 6;
        for (; pokerNum <= 10; pokerNum++)
        {
            foreach (var poker in myValueList)
            {
                if (!bCurBomb || curPokerList.Count < pokerNum || (curPokerList.Count == pokerNum && poker.Key > nCurBombValue))
                {
                    if (poker.Value.Count == pokerNum)
                    {
                        splitPokerArray[5].Add(new List<byte>(poker.Value));
                    }
                    else if (poker.Key < 16 && poker.Value.Count < pokerNum && poker.Value.Count + laiziCount >= pokerNum)
                    {
                        List<byte> list = new List<byte>(poker.Value);
                        for (int i = 0; i < pokerNum - poker.Value.Count; i++)
                            list.Add(laizi);
                        splitPokerArray[6].Add(list);
                    }
                }
            }

            splitPokerArray[5].AddRange(splitPokerArray[6]);
            splitPokerArray[6].Clear();
        }

        for(int i = 0; i < 5; i++)
        {
            AblePokerArray.AddRange(splitPokerArray[i]);
        }

        //bomb
        for (int i = 5; i < 7; i++)
        {
            bombList.AddRange(splitPokerArray[i]);
        }

        //查找天炸
        List<byte> check = haveList.FindAll(s => s >= 0x41);
        if (check.Count == 4)
        {
            bombList.Add(check);
        }

        return AblePokerArray;
    }

    void UpdateAlarm()
    {
        if (m_fAlarmTimer > 0f)
        {
            m_fAlarmTimer -= Time.deltaTime;
            if (m_fAlarmTimer < 5f)
            {
                CustomAudioDataManager.GetInstance().PlayAudio(1007);
                m_fAlarmTimer = -1f;
            }
        }
    }

    public void OnPSTChange()
    {
        int cur = (int)m_GameBase.CurPOT + 1;
        int other = cur + 1;
        if (other > 2)
            other = 1;
        Transform parent = m_GameBase.MainUITfm.Find("Middle/PlayerInfor_1_" + other);
        parent.gameObject.SetActive(false);
        parent = m_GameBase.MainUITfm.Find("Middle/PlayerInfor_1_" + cur);
        parent.gameObject.SetActive(true);
        Transform tfm = parent.Find("Icon_PlayerInfor_1");
        m_InfoUI = tfm.Find("Playerinfo");
        m_SitUI = tfm.Find("Button_Sitdown");
        m_HaveCards = tfm.Find("Poker_Shoupai/Shoupai_1");
        m_HaveCards.gameObject.SetActive(true);
        UpdateInfoUI();

        RemoveAllLipai();
        if(m_GameBase.CurPOT == PokerSortType.ePST_Horizontal)
        {
            m_LipaiTfm = tfm.Find("Poker_Shoupai/Shoupai_2");

            m_HaveLipaiList.Reverse();
            m_HaveLipaiList.InsertRange(0, m_VertHaveLipaiList);
            m_VertHaveLipaiList.Clear();
        }
        else
        {
            m_LipaiTfm = m_HaveCards.Find("Point_lipai");

            List<List<byte>> haveLipaiList = new List<List<byte>>(m_HaveLipaiList);
            m_HaveLipaiList.Clear();
            m_VertHaveLipaiList.Clear();
            List<byte> childList = new List<byte>();
            GuandanPokerType_Enum type;
            List<byte> temp;
            foreach(List<byte> list in haveLipaiList)
            {
                type = CanLipai(list, ref childList, true);
                temp = new List<byte>(childList);
                if (type == GuandanPokerType_Enum.GuanPokerType_Blast || type == GuandanPokerType_Enum.GuanPokerType_SameColorFlush)
                    m_HaveLipaiList.Insert(0, temp);
                else
                    m_VertHaveLipaiList.Add(temp);
            }
        }

        ShowPlayerInfo();

        bool showRole = m_RoleAnim == null ? false : m_RoleAnim.gameObject.activeSelf;
        if(showRole)
            CreateRole(false, tfm.Find("point_ren"));
        ShowRole(showRole);

        tfm = tfm.Find("Tip_BG");
        m_TipText = tfm.Find("Image_zhuangtai/Text").GetComponent<Text>();
        m_RankTfm = tfm.Find("Image_ranking");
        m_ChatText = tfm.Find("Tip_talk/Text").GetComponent<Text>();
        m_NoBigTextObj = tfm.Find("Tip_yaobuqi").gameObject;
        m_NoBigTextObj.SetActive(false);

        tfm = parent.Find("ButtonBG");
        m_GameBtnsTfm.SetParent(tfm.Find("ButtonBG_xingpai"), false);

        bool showBtn = false;
        if (m_ArrayBtnsTfm != null)
            showBtn = m_ArrayBtnsTfm.gameObject.activeSelf;
        m_ArrayBtnsTfm = tfm.Find("ButtonBG_lipai");
        m_LipaiBtns = m_ArrayBtnsTfm.Find("lipai_zidingyi").GetComponentsInChildren<Button>(true);
        m_ArrayBtnsTfm.gameObject.SetActive(showBtn);

        Refresh(false);
    }

    void OnClickShowDesk(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerEnter)
            m_HaveCards.gameObject.SetActive(false);

        if (eventtype == EventTriggerType.PointerExit)
            m_HaveCards.gameObject.SetActive(true);
    }

    public override void OnPokerNumChanged(byte num)
    {
        base.OnPokerNumChanged(num);

        if (m_GameBase.Bystander)
        {
            m_HavePokerList.Clear();
            for (byte i = 0; i < num; i++)
                m_HavePokerList.Add(RoomInfo.NoSit);
        }

        m_ArrayBtnsTfm.gameObject.SetActive(true);
        Refresh(false);
    }

    public override void UpdateRecordCards(bool begin)
    {
        VertArrayPoker(false, true, begin);
    }

    public override void ShowEndPoker(byte[] cards)    {
        if (m_GameBase.Bystander)
        {
            m_HavePokerList.Clear();
            m_HavePokerList.AddRange(cards);
            OnDealPoker();
            ModifyHorizCards("_big");
        }
    }
}