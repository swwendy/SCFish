using System.Collections;using System.Collections.Generic;using UnityEngine;using XLua;[Hotfix]public class Black_DealerAnimCtrl : MonoBehaviour{    public CGame_BlackJack m_BjGameBase;    //FBIKController m_IKCtrl;    Animator m_Animator;    SkinnedMeshRenderer m_LeftCardMesh, m_RightCardMesh;    bool m_bPickCard = false;    bool m_bAddCard = false;    bool m_bDealEnd = false;    bool m_bOpenCard = false;    bool m_bBeginPart = false;    bool m_bPart = false;    bool m_bBuySafe = false;    bool m_bChargeCard = false;    public bool BuySafe    {        get        {            return m_bBuySafe;         }        set        {            if(value)
                m_Animator.SetTrigger("EndIdle");            m_Animator.SetBool("NeedSafe", value);            m_bBuySafe = value;        }    }    bool m_bLookCard = false;    Coroutine m_DealCardCoroutine = null;    Coroutine m_AskCoroutine = null;
    bool m_bAsking = false;
    bool m_bDealPoker = false;    List<PokerData> m_PokerDataList = new List<PokerData>();    void Awake()    {        Transform tfm = transform.Find("Dealer");        m_LeftCardMesh = tfm.Find("Card_Left").GetComponent<SkinnedMeshRenderer>();        m_RightCardMesh = tfm.Find("Card_Right").GetComponent<SkinnedMeshRenderer>();    }    // Use this for initialization    void Start ()    {        //m_IKCtrl = GetComponent<FBIKController>();        m_Animator = GetComponent<Animator>();        BuySafe = false;    }    // Update is called once per frame    void Update ()    {    }    public void Init(bool clearPokerList = true)    {        m_bPickCard = false;        m_bAddCard = false;        m_bDealEnd = false;        m_bOpenCard = false;        m_bBeginPart = false;        m_bPart = false;        m_bChargeCard = false;        BuySafe = false;        m_bLookCard = false;        m_Animator.Rebind();        StopAllCoroutines();        m_DealCardCoroutine = null;        m_AskCoroutine = null;
        m_bAsking = false;
        m_bDealPoker = false;
        if (clearPokerList)
            m_PokerDataList.Clear();
    }    public void CardsToPlayer(byte sit, byte points, byte other, byte pos = 0, byte card = 0)    {        PokerData data = new PokerData();        data.Sit = sit;        data.Pos = pos;        data.Card = card;        data.Points = points;        data.OtherPoints = other;        List<PokerData> list = new List<PokerData> {data};        CardsToPlayer(list);    }    public void CardsToPlayer(List<PokerData> list)    {
        m_PokerDataList.AddRange(list);

        if (m_BjGameBase.m_eFocus == CGame_BlackJack.FocusType.eFT_Get)
            m_DealCardCoroutine = StartCoroutine(DealPlayerCards(m_DealCardCoroutine));        //else        //    HandlePokerDirect();    }    void HandlePokerDirect()
    {
        while (m_PokerDataList.Count > 0)
        {
            PokerData data = m_PokerDataList[0];

            if (data.Sit < RoomInfo.PlayerNum)
            {
                m_BjGameBase.m_RoomInfo.m_PlayerList[data.Sit].HandleAddPoker(data.Pos, data.Card, data.Points, data.OtherPoints);
            }
            else
            {
                int count = m_BjGameBase.m_RoomInfo.m_Dealer.m_vecBlackPoker[0].Count;

                if (count == 2 && !m_bOpenCard && m_BjGameBase.m_RoomInfo.m_Dealer.m_nOpenPoke > 0)//等待翻牌
                {
                    m_BjGameBase.m_RoomInfo.m_Dealer.Update2ndPoker(true, data.Points, data.OtherPoints);
                }
                else
                {
                    m_BjGameBase.m_RoomInfo.m_Dealer.HandleAddPoker(0, data.Card, data.Points, data.OtherPoints);
                }
            }

            m_PokerDataList.RemoveAt(0);
        }

        m_LeftCardMesh.gameObject.SetActive(true);
        m_RightCardMesh.gameObject.SetActive(true);
    }    IEnumerator DealPlayerCards(Coroutine cor)    {        yield return cor;        yield return new WaitWhile(() => (m_bLookCard || m_Animator.GetBool("Ask")));        m_bDealPoker = true;        m_Animator.SetTrigger("EndIdle");        while (m_PokerDataList.Count > 0)
        {
            PokerData data = m_PokerDataList[0];

            if (data.Sit < RoomInfo.PlayerNum)
            {
                ChangePokerMat(data.Card);

                m_Animator.SetBool("PickCard", true);

                yield return new WaitUntil(() => (m_bPickCard && !m_bDealEnd));

                m_Animator.SetBool("DealCardPlayer", true);
                m_Animator.SetFloat("PlayerID", data.Sit);
                //Transform tfm = m_BjGameBase.m_RoomInfo.m_PlayerList[data.Sit].GetPokerTfm(data.Pos);
                //m_IKCtrl.target = tfm.GetChild(0);
                m_Animator.SetFloat("PileID", data.Pos);
                int pos = data.Pos;
                if (pos > 0)
                    pos--;
                m_Animator.SetFloat("PlayerCardID", m_BjGameBase.m_RoomInfo.m_PlayerList[data.Sit].m_vecBlackPoker[pos].Count);

                yield return new WaitUntil(() => (m_bAddCard));

                m_bAddCard = false;
                m_LeftCardMesh.gameObject.SetActive(false);
                m_RightCardMesh.gameObject.SetActive(false);
                m_BjGameBase.m_RoomInfo.m_PlayerList[data.Sit].HandleAddPoker(data.Pos, data.Card, data.Points, data.OtherPoints);
                m_PokerDataList.RemoveAt(0);

                yield return new WaitUntil(() => (m_bDealEnd));

                //m_Animator.SetBool("Ask", m_bAsking);
                m_Animator.SetBool("DealCardPlayer", false);
                //m_IKCtrl.target = null;
            }
            else
            {
                int count = m_BjGameBase.m_RoomInfo.m_Dealer.m_vecBlackPoker[0].Count;

                if (count == 2 && !m_bOpenCard && m_BjGameBase.m_RoomInfo.m_Dealer.m_nOpenPoke > 0)//等待翻牌
                {
                    ChangePokerMat(m_BjGameBase.m_RoomInfo.m_Dealer.m_vecBlackPoker[0][1]);

                    m_Animator.SetBool("PickCard", false);
                    m_Animator.SetBool("Ask", false);

                    m_Animator.SetTrigger("OpenCard");

                    yield return new WaitUntil(() => (m_bAddCard));

                    m_BjGameBase.m_RoomInfo.m_Dealer.Update2ndPoker(true, data.Points, data.OtherPoints);
                    m_bAddCard = false;
                    m_LeftCardMesh.gameObject.SetActive(false);
                    m_RightCardMesh.gameObject.SetActive(false);
                    m_PokerDataList.RemoveAt(0);

                    yield return new WaitUntil(() => (m_bOpenCard));

                }
                else
                {

                    m_Animator.SetBool("PickCard", true);

                    yield return new WaitUntil(() => (m_bPickCard && !m_bDealEnd));

                    string param;
                    if (count == 0)
                        param = "DealCardDealer";
                    else if (count == 1)
                    {
                        ChangePokerMat(m_BjGameBase.m_RoomInfo.m_Dealer.m_vecBlackPoker[0][0]);
                        param = "DealCardDealerScend";
                    }
                    else
                    {
                        ChangePokerMat(data.Card);
                        m_Animator.SetFloat("DealerCardID", count + 1);
                        param = "DealCardDealer3rd";
                    }
                    m_Animator.SetBool(param, true);

                    yield return new WaitUntil(() => (m_bAddCard));

                    m_bAddCard = false;
                    m_LeftCardMesh.gameObject.SetActive(false);
                    m_RightCardMesh.gameObject.SetActive(false);
                    m_BjGameBase.m_RoomInfo.m_Dealer.HandleAddPoker(0, data.Card, data.Points, data.OtherPoints);
                    m_PokerDataList.RemoveAt(0);

                    yield return new WaitUntil(() => (m_bDealEnd));

                    m_Animator.SetBool(param, false);
                }
            }

            m_LeftCardMesh.gameObject.SetActive(true);
            m_RightCardMesh.gameObject.SetActive(true);
        }

        m_Animator.SetBool("PickCard", m_bAsking);        m_bDealPoker = false;
    }

    IEnumerator DealAsk(Coroutine cor, byte sit, byte pos, bool bFirst)
    {
        yield return cor;

        yield return new WaitWhile(() => m_bDealPoker);

        DebugLog.Log("StartAskPileID:" + m_Animator.GetFloat("PileID") + "->" + m_BjGameBase.m_RoomInfo.m_iCurPokerPos);

        m_Animator.SetTrigger("EndIdle");
        m_Animator.SetFloat("PlayerID", sit);
        m_Animator.SetFloat("PileID", pos);
        m_Animator.SetBool("Ask", true);
        Black_Player player = m_BjGameBase.m_RoomInfo.m_PlayerList[sit];
        player.OnTurn(1.0f, bFirst);

        yield return new WaitUntil(() => (!player.Asking || m_bPart || !m_bAsking));

        bool part = m_bPart;

        if (m_bPart)
        {
            player.OnSplit();
            yield return new WaitWhile(() => (m_bPart));
            player.OnSplitEnd();
            m_LeftCardMesh.gameObject.SetActive(false);
            m_RightCardMesh.gameObject.SetActive(false);
        }

        m_Animator.SetBool("Ask", false);
        m_Animator.SetBool("PickCard", m_bAsking);

        yield return new WaitWhile(() => m_bDealEnd);

        if (part)
        {
            m_LeftCardMesh.gameObject.SetActive(true);
            m_RightCardMesh.gameObject.SetActive(true);
        }
        m_bPart = false;
    }
    void PickCardEnd()    {        m_bPickCard = !m_bPickCard;    }    void AddCard()    {        m_bAddCard = true;    }    void DealEnd()    {        m_bDealEnd = true;    }    void DealCardDel()    {        m_BjGameBase.m_RoomInfo.m_Dealer.HidePoker();    }    void CardDel()    {        m_BjGameBase.m_RoomInfo.m_Dealer.HidePoker(1);    }    void OpenCardEnd()    {        m_bOpenCard = true;    }    void DealIdle()    {        m_bPickCard = true;        m_bDealEnd = false;    }    void Idle()    {        m_bDealEnd = false;    }    void ChargeCardDel(int pos)    {        int sit = pos / 10;        int cardPos = pos % 10;        if (cardPos == 1)            m_BjGameBase.m_RoomInfo.m_PlayerList[sit].OnEnd();        else            m_BjGameBase.m_RoomInfo.m_PlayerList[sit].ClearPoker(cardPos);    }    void ChargeDealerDel()    {        m_BjGameBase.m_RoomInfo.m_Dealer.OnEnd();    }    public void AskPlayer(byte sit, byte pos, bool bFirst)    {        m_bAsking = true;
        if (m_BjGameBase.m_eFocus == CGame_BlackJack.FocusType.eFT_Get)
            m_AskCoroutine = StartCoroutine(DealAsk(m_AskCoroutine, sit, pos, bFirst));        else            HandleAskPlayerDirect();    }    void HandleAskPlayerDirect()
    {
        if (!m_bAsking)
            return;

        if(m_BjGameBase.m_RoomInfo.m_iTurnAskDoubleSign < RoomInfo.PlayerNum)
        {
            Black_Player player = m_BjGameBase.m_RoomInfo.m_PlayerList[m_BjGameBase.m_RoomInfo.m_iTurnAskDoubleSign];
            player.OnTurn(1f, true);
            player.OnAnswer(0, 1, 0);

            if (m_bBeginPart)
            {
                player.OnSplit();
                player.OnSplitEnd();
            }        }    }    public void StopAsk()    {        m_bAsking = false;    }    void Part()    {        m_bPart = true;    }    void PartEnd()    {        m_bPart = false;    }    public void BuySafeTimeUpWarn(float time)    {        Invoke("TimeUpWarn", time);    }    void TimeUpWarn()    {        CustomAudioDataManager.GetInstance().PlayAudio(1009);    }    public void BuySafeResult(bool someOneBuy)    {        BuySafe = false;        CancelInvoke();        //if(someOneBuy)        {            m_Animator.SetTrigger("EndIdle");            m_Animator.SetTrigger("LookCard");            m_bLookCard = true;        }    }    void LookCardEnd()    {        m_bLookCard = false;        m_BjGameBase.OnBuySafeEnd();    }    public void PlaySplit(byte sit)    {        m_bBeginPart = true;        m_Animator.SetFloat("PlayerID", sit);        Black_Player player = m_BjGameBase.m_RoomInfo.m_PlayerList[sit];        if(player != null && player.m_vecBlackPoker[0].Count > 1)        {
            ChangePokerMat(player.m_vecBlackPoker[0][0], 0);
            ChangePokerMat(player.m_vecBlackPoker[0][1], 1);
            player.ShowCountTip(false);        }        m_Animator.SetTrigger("Part");    }    public void PlayChargeCard()    {        StopAsk();        m_bChargeCard = true;        if (m_BjGameBase.m_eFocus == CGame_BlackJack.FocusType.eFT_Get)
            StartCoroutine(DealChargeCard());        else            HandleChargeCardDirect();    }    void ChargeEnd()    {        m_bChargeCard = false;    }    void HandleChargeCardDirect()
    {
        if (m_bChargeCard)
        {
            foreach (Black_Player player in m_BjGameBase.m_RoomInfo.m_PlayerList)
            {
                player.OnEnd();
            }
            m_BjGameBase.m_RoomInfo.m_Dealer.OnEnd();

            m_BjGameBase.OnEnd();

            m_PokerDataList.Clear();
        }        Init(false);    }    IEnumerator DealChargeCard()    {
        yield return new WaitWhile(() => m_bDealPoker);
        int playerNum = 0;        int startPos = 0;        byte card = 0;        Black_Role role;        for(int i = RoomInfo.PlayerNum - 1; i >= 0 ; i--)        {            role = m_BjGameBase.m_RoomInfo.m_PlayerList[i];            if (role.m_vecBlackPoker[0].Count > 0)            {                if(playerNum == 0)                {                    startPos = i;                    card = role.m_vecBlackPoker[1].Count > 0 ? role.m_vecBlackPoker[1][0] : role.m_vecBlackPoker[0][0];                }                if (i == 0)                    m_Animator.SetBool("HavePlayerRight", true);                playerNum++;            }        }        if(playerNum != 0)        {            CustomAudioDataManager.GetInstance().PlayAudio(1006);            m_Animator.SetTrigger("EndIdle");            m_Animator.SetBool("Charge", true);            m_Animator.SetFloat("PlayerNum", playerNum);            m_Animator.SetInteger("StartChargeNum", startPos);            ChangePokerMat(card);            yield return new WaitWhile(() => m_bChargeCard);            m_bChargeCard = false;            m_BjGameBase.OnEnd();        }        Init();    }    void ChangePokerMat(byte card, int leftOrRight = -1)//leftOrRight: -1(all) 1(left) 0(right)     {        string mat = GameCommon.GetPokerMat(card);        Material material = m_BjGameBase.m_AssetBundle.LoadAsset<Material>(mat);        if (leftOrRight < 0)            m_LeftCardMesh.material = m_RightCardMesh.material = material;        else if (leftOrRight == 0)            m_RightCardMesh.material = material;        else            m_LeftCardMesh.material = material;    }
    public void OnFocusChange()
    {
        if(m_BjGameBase.m_eFocus == CGame_BlackJack.FocusType.eFT_Lost)
        {
            HandleAskPlayerDirect();
        }
        else if(m_BjGameBase.m_eFocus == CGame_BlackJack.FocusType.eFT_Get)
        {
            HandlePokerDirect();
        }

        HandleChargeCardDirect();
    }}