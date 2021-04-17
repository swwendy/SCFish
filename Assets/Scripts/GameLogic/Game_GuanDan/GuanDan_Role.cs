﻿using DG.Tweening;
    {
        get
        {
            return m_nSex;
        }
        set
        {
            if(value != m_nSex)
            {
                if(m_RoleAnim != null)
                {
                    GameObject.Destroy(m_RoleAnim.gameObject);
                    m_RoleAnim = null;
                }
            }
            m_nSex = value;
        }
    }
    {

    }
    {
        if(m_RoleAnim != null)
        {
            m_RoleAnim.transform.SetParent(parent, false);
            return;
        }

        UnityEngine.Object obj = (GameObject)m_GameBase.CommonAssetBundle.LoadAsset("Anime_ren_" + Sex);
        if(obj != null)
        {
            GameObject gameObj = (GameObject)GameMain.instantiate(obj);
            gameObj.transform.SetParent(parent, false);

            m_RoleAnim = gameObj.GetComponent<DragonBones.UnityArmatureComponent>();
            m_RoleAnim.armature.flipX = bFlip;
            m_RoleAnim.flipX = bFlip;
            m_RoleAnim.AddEventListener(DragonBones.EventObject.COMPLETE, AnimationEventHandler);
            m_RoleAnim.gameObject.SetActive(false);

            m_strNextAnim = "idle";

    void AnimationEventHandler(string _type, DragonBones.EventObject eventObject)
    {
        if (_type == DragonBones.EventObject.COMPLETE)
        {
            PlayRoleAnim(m_strNextAnim);
        }
    }

    {
        if (m_RoleAnim == null)
            return;

        m_RoleAnim.animation.Play(anim);
        if (nextAnim.Length > 0)
            m_strNextAnim = nextAnim;
    }
        {
            Image img = m_CountdownObj.transform.Find("shine").GetComponent<Image>();

            float time = 0.5f;
            float lerp = Mathf.PingPong(Time.time, time)/time;

            Color c = img.color;
            c.a = lerp;
            img.color = c;
        }
        m_InfoUI.Find("Head").gameObject.SetActive(true);
        m_RankTfm.gameObject.SetActive(false);
        Rank = 0;
        m_HavePokerList.Clear();
        CurPokerType = GuandanPokerType_Enum.GuanPokerType_Error;
        OnDealPoker();
        ShowRole(false);

        foreach (Transform childTfm in m_PlayCards)
    {
        Transform tfm = infoUI.Find("Head/HeadMask/ImageHead");
            tfm.gameObject.SetActive(true);
            tfm.Find("Text_Jifen").GetComponent<Text>().text = m_nTotalCoin.ToString();
            tfm.Find("Image_Jinbi").gameObject.SetActive(!bContest);
            tfm.Find("Image_Jifen").gameObject.SetActive(bContest);
        }
        tfm = infoUI.Find("Text_dashifen");
        m_nTotalCoin = coin;
    }

    public void UpdateInfoUI()

    public virtual IEnumerator BeginPoker(List<byte> showGradeIndex = null, byte showPoker = 0)
    {
        yield return null;
    }
    
    public void OnGameStart()
    {
        OnDealPoker();
        ShowRole();
        ShowCountdown(false);
    }

    public virtual void RemoveCards(byte[] cards)

    public virtual void AddCards(byte[] cards)

    public void ShowTipPoker(byte poker, int index, float showTime = -1f)
    {
        int num = index * 4 + 1;
        byte[] pokers = new byte[num];
        for (int i = 0; i <= index; i++)
            pokers[i * 4] = poker;
        OnDealPoker(-1, pokers);

        if(showTime > 0)
            GameMain.WaitForCall(showTime, () => OnDealPoker());
    }

    public void ShowRank(byte rank)
    {
        if (rank == 0)
        {
            return;
            return;

        Rank = rank;
        m_RankTfm.gameObject.SetActive(true);
        m_RankTfm.Find("TextNum").GetComponent<Text>().text = rank.ToString();

        //string[] strs = new string[] { "", "头游", "二游", "三游", "末游" };
        //m_RankTfm.FindChild("TextName").GetComponent<Text>().text = strs[rank];

        m_RankTfm.GetComponent<Image>().sprite = m_GameBase.GuanDanAssetBundle.LoadAsset<Sprite>("gd_icon_" + rank);
    }


    //haveRight -1:not server 0:no right 1:have right
    public virtual void OnDealPoker(sbyte haveRight = -1, byte[] cards = null, GuandanPokerType_Enum pokerType = GuandanPokerType_Enum.GuanPokerType_Error, byte rank = 0)
        {
            int value = 0;

            if (cards != null && cards.Length > 0)
            {
                RemoveCards(cards);

                m_PlayPokerList = new List<byte>(cards);
                m_PlayPokerList.Sort(LL_PokerType.SortByIndex);
                CurPokerType = pokerType;

                //FormatPokersByType
                List<byte> childList = new List<byte>();
                GuandanPokerType_Enum type = m_GameBase.GetPokersType(m_PlayPokerList, new List<byte>(new byte[m_PlayPokerList.Count]), GuandanPokerType_Enum.GuanPokerType_Error, ref childList);
                if (type != GuandanPokerType_Enum.GuanPokerType_Error)
                        m_PlayPokerList = childList;

                byte laizi = LL_PokerType.GetLaiziValue();
                bool showMask = false;
                GameObject prefabObj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("Poker_Chupai_1");
                GameObject obj;
                for (int i = 0; i < m_PlayPokerList.Count; i++)
                {
                    obj = GameMain.Instantiate(prefabObj);
                    obj.transform.SetParent(m_PlayCards, false);

                    showMask = LL_PokerType.GetPokerLogicValue(m_PlayPokerList[i], laizi) == 15;
                    PlayCard.SetCardSprite(obj, m_GameBase.CommonAssetBundle, m_PlayPokerList[i], showMask, laizi);
                }

                CustomAudioDataManager.GetInstance().PlayAudio(Sex == 1 ? 1006 : 2006);

                GameMain.SC(PlayPokerEffect(pokerType));

                PlayRoleAnim("fapai");
                m_TipText.text = "";
            }
            else
            {
                m_PlayPokerList.Clear();
                CurPokerType = GuandanPokerType_Enum.GuanPokerType_Error;
                m_TipText.text = (cards == null) ? "" : "不出";
                PlayRoleAnim("idle");
            }

            ShowCountdown(false);

            if(cards != null)
                GameMain.WaitForCall(0.3f, () => PlayPokerSound(haveRight, CurPokerType, value));

            ShowRank(rank);
        {
            m_TipText.text = "";

            if (cards != null)
            {
                if(cards.Length > 0)
                {
                    m_PlayPokerList = new List<byte>(cards);
                    CurPokerType = pokerType;

                    byte laizi = LL_PokerType.GetLaiziValue();
                    GameObject prefabObj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("Poker_Chupai_1");
                    GameObject obj;
                    for (int i = 0; i < m_PlayPokerList.Count; i++)
                    {
                        obj = GameMain.Instantiate(prefabObj);
                        obj.transform.SetParent(m_PlayCards, false);

                        PlayCard.SetCardSprite(obj, m_GameBase.CommonAssetBundle, m_PlayPokerList[i], false, laizi);
                    }
                {
                    m_PlayPokerList.Clear();
                    CurPokerType = GuandanPokerType_Enum.GuanPokerType_Error;
                    m_TipText.text = "不出";
                }
        }
    {
        Transform tfm = m_InfoUI.Find("Head/HeadMask/ImageHead");
        return tfm.GetComponent<Image>();
    }
    {
        return m_InfoUI.Find("BG_nameJifen/Name_Text").GetComponent<Text>().text;
    }
    {
        return m_InfoUI.Find("Text_dashifen").GetComponent<Text>().text;
    }
    {
        OnDealPoker();
    }
    {
        if(show)
        {
                CreateRole();
        }
            if (m_RoleAnim != null)
                GameObject.Destroy(m_RoleAnim.gameObject);
        if (m_RoleAnim != null)
        {
            m_RoleAnim.gameObject.SetActive(show);
            PlayRoleAnim("idle");

    public virtual void OnAskDealPoker(bool bHaveRight, List<byte> curPokerList, GuandanPokerType_Enum pokerType, float askTime, float bankTime, bool pause = false)
        PlayRoleAnim("sikao");
            if (bankTime >= 0f)
            {
                return false;
            }

    protected void ShowCountdown(bool show, float time = 0f, CustomCountdownImgMgr.CallBackFunc fun = null, bool pause = false)
        Image img = m_CountdownObj.GetComponent<Image>();
        m_CountdownObj.SetActive(show && (time > 0f || fun == null));

    public virtual void ShowEndPoker(byte[] cards)

    //state: 0:显示按钮 1：显示贡牌 2:收贡 submitOrReturn:上贡还是还贡
    public virtual void OnGong(byte state, bool submitOrReturn, byte poker = 0, Transform srcTfm = null, bool bReverse = false)
    {
        if (state == 1)
        {
            if (!bReverse)
            {
                ShowTipPoker(submitOrReturn ? poker : RoomInfo.NoSit, 0);
                RemoveCards(new byte[] { poker });
            {
                OnDealPoker();
                AddCards(new byte[] { poker });
            }
        {
            byte laizi = LL_PokerType.GetLaiziValue();
            GameObject flyObj = new GameObject();
            flyObj.AddComponent<Image>();
            bool bShowMask = LL_PokerType.GetPokerLogicValue(poker, laizi) == 15;
            PlayCard.SetCardSprite(flyObj, m_GameBase.CommonAssetBundle, submitOrReturn ? poker : RoomInfo.NoSit, bShowMask, laizi);
            flyObj.transform.SetParent(m_GameBase.GameCanvas.transform, false);
            flyObj.transform.position = srcTfm.position;
            (flyObj.transform as RectTransform).sizeDelta = (srcTfm.GetChild(0) as RectTransform).sizeDelta;
            Vector3 targetPos = m_PlayCards.position;
            flyObj.transform.DOMove(targetPos, 1f).OnComplete(() =>
            {
                if (!submitOrReturn)
                    PlayCard.SetCardSprite(flyObj, m_GameBase.CommonAssetBundle, poker, bShowMask, laizi);

                GameObject.Destroy(flyObj, 1f);
                AddCards(new byte[] { poker });
            });

    public void OnChat(byte index)

    void PlayPokerSound(sbyte haveRight, GuandanPokerType_Enum type, int value)
        int begin = (Sex == 1) ? 1000 : 2000;
        if (type == GuandanPokerType_Enum.GuanPokerType_Error)
            CustomAudioDataManager.GetInstance().PlayAudio(begin + 410 + Random.Range(0, 3));
        else if (type == GuandanPokerType_Enum.GuanPokerType_KingBlast || type == GuandanPokerType_Enum.GuanPokerType_Blast)
        else
        {
            //float ratio = Random.Range(0f, 1f);
            //if (haveRight > 0 || ratio < 0.5f)
            {
                if(type <= GuandanPokerType_Enum.GuanPokerType_Three)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + (int)type * 100 + value);
                else if(type == GuandanPokerType_Enum.GuanPokerType_SeriesThree)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 401);
                else if(type == GuandanPokerType_Enum.GuanPokerType_SeriesPairs)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 402 + Random.Range(0, 2));
                else if(type == GuandanPokerType_Enum.GuanPokerType_SameColorFlush)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 404);
                else if(type == GuandanPokerType_Enum.GuanPokerType_Flush)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 408);
                else if(type == GuandanPokerType_Enum.GuanPokerType_ThreeAndTwo)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 500 + value);
            }
            //else
            //{
            //    CustomAudioDataManager.GetInstance().PlayAudio(begin + 413 + Random.Range(0, 4));
            //}
        }
    }

    IEnumerator PlayPokerEffect(GuandanPokerType_Enum pokerType)
    {
        if(pokerType == GuandanPokerType_Enum.GuanPokerType_SeriesThree)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_gangban", 0.73f, 0, m_AnimTfm);
        }
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_tonghuashun", 0.97f, 0, m_AnimTfm);
        }
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_GDzhadan", 2f, 1009, m_AnimTfm);
        }
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_GDwangzha1", 1f, 0, m_AnimTfm);

            yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_GDwangzha2", 1f, 0);
        }
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_GDliandui", 2f, 0, m_AnimTfm);
        }
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_GDshunzi", 2f, 1008, m_AnimTfm);
        }
    }

    public virtual void OnPokerNumChanged(byte num)
    {
        m_HaveCards.gameObject.SetActive(true);
        OnGameStart();
    }

    public virtual void UpdateRecordCards(bool begin = false)
    {
    }

    public virtual void OnDisOrReconnect(bool bDis)
    {

    }

    public virtual bool Disconnected()
    {
        return true;
    }