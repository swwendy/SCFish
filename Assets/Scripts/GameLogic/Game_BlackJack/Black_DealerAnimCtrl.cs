﻿using System.Collections;
                m_Animator.SetTrigger("EndIdle");
    bool m_bAsking = false;
    bool m_bDealPoker = false;
        m_bAsking = false;
        m_bDealPoker = false;
        if (clearPokerList)
            m_PokerDataList.Clear();
    }
        m_PokerDataList.AddRange(list);

        if (m_BjGameBase.m_eFocus == CGame_BlackJack.FocusType.eFT_Get)
            m_DealCardCoroutine = StartCoroutine(DealPlayerCards(m_DealCardCoroutine));
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
    }
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

        m_Animator.SetBool("PickCard", m_bAsking);
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

        if (m_BjGameBase.m_eFocus == CGame_BlackJack.FocusType.eFT_Get)
            m_AskCoroutine = StartCoroutine(DealAsk(m_AskCoroutine, sit, pos, bFirst));
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
            }
            ChangePokerMat(player.m_vecBlackPoker[0][0], 0);
            ChangePokerMat(player.m_vecBlackPoker[0][1], 1);
            player.ShowCountTip(false);
            StartCoroutine(DealChargeCard());
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
        }
        yield return new WaitWhile(() => m_bDealPoker);

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
    }