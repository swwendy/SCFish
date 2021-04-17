﻿using DG.Tweening;
    {
        get
        {
            return m_nSex;
        }
        set
        {
            if (value != m_nSex)
            {
                if (m_RoleAnim != null)
                {
                    GameObject.Destroy(m_RoleAnim.gameObject);
                    m_RoleAnim = null;
                }
            }
            m_nSex = value;
        }
    }
        tfm = tfm.Find("Text_offline");
        if(tfm != null)
            m_DisconnectObj = tfm.gameObject;

        m_AnimTfm = m_GameBase.AnimationTfm.parent.Find("Playerpoint_" + index);
    {
        if (m_RoleAnim != null)
            return;

        UnityEngine.Object obj = (GameObject)m_GameBase.CommonAssetBundle.LoadAsset("Anime_ren_" + Sex);
            GameObject gameObj = (GameObject)GameMain.instantiate(obj);
            gameObj.transform.SetParent(m_InfoUI.parent.Find("point_ren"), false);

            bool bFlip = m_nIndex == 2;
            m_RoleAnim = gameObj.GetComponent<DragonBones.UnityArmatureComponent>();
            m_RoleAnim.armature.flipX = bFlip;
            m_RoleAnim.flipX = bFlip;
            m_RoleAnim.AddEventListener(DragonBones.EventObject.COMPLETE, AnimationEventHandler);
            m_strNextAnim = "idle";

            gameObj.SetActive(false);

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

        if (m_RoleAnim != null)
        OnDisOrReconnect(false);
    {
        foreach(Transform child in m_AnimTfm)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in m_AlarmTfm)
        {
            GameObject.Destroy(child.gameObject);
        }

        PlayRoleAnim("idle");
        m_strNextAnim = "idle";
        ShowEndPoker(false);

        CurPokerType = LandPokerType_Enum.LandPokerType_Error;

        if(m_RecordPokerTfm)
            m_RecordPokerTfm.gameObject.SetActive(false);
    }
            tfm.gameObject.SetActive(true);
            tfm.Find("Text_Jifen").GetComponent<Text>().text = m_nTotalCoin.ToString();
            tfm.Find("Image_Jinbi").gameObject.SetActive(!bContest);
            tfm.Find("Image_Jifen").gameObject.SetActive(bContest);
        }
    {
        Transform tfm = m_InfoUI.Find("Head/HeadMask/ImageHead");
        return tfm.GetComponent<Image>().sprite;
    }
        m_HaveCards.gameObject.SetActive(true);
                m_CountdownObj.SetActive(true);
                Text text = m_CountdownObj.GetComponentInChildren<Text>();
                if (!pause)
                    m_GameBase.CCIMgr.AddTimeImage(img, time, 1f, fun, text, false);
                {
                }
            {
                m_CountdownObj.SetActive(false);

                fun(0, false, img, "");
            }
        {
        }
    {
        m_nCurPokerNum = num;

        tfm.gameObject.SetActive(m_nCurPokerNum != 0);
        if (m_nCurPokerNum != 0)
        ShowCountdown(false);
        {
            m_HavePokerList.AddRange(cards);
            UpdateRecordCards();
        }
            m_nCurPokerNum += (byte)cards.Length;
            OnPokerNumChanged(m_nCurPokerNum);
        {
            foreach(byte card in cards)
                m_HavePokerList.Remove(card);
            UpdateRecordCards();
        }
            m_nCurPokerNum -= (byte)cards.Length;
            OnPokerNumChanged(m_nCurPokerNum);
        }
        PlayRoleAnim("sikao");
            if (bankTime >= 0f)
            {
                return false;
            }
        int value = 0;
        {
            if (cards != null && cards.Length > 0)
            {
                RemoveCards(cards);

                m_PlayPokerList = new List<byte>(cards);
                m_PlayPokerList.Sort(LL_PokerType.SortByIndex);
                CurPokerType = pokerType;
                //FormatPokersByType
                List<byte> childList = new List<byte>();
                LandPokerType_Enum type = m_GameBase.GetPokersType(m_PlayPokerList, new List<byte>(new byte[m_PlayPokerList.Count]), LandPokerType_Enum.LandPokerType_Error, ref childList);
                if (type != LandPokerType_Enum.LandPokerType_Error)
                    m_PlayPokerList = childList;

                int end = m_PlayPokerList.Count - 1;
                for (; i < m_PlayPokerList.Count; i++)
                {
                    PlayCard.SetCardSprite(m_PlayCards.GetChild(i).gameObject, 
                        m_GameBase.CommonAssetBundle, m_PlayPokerList[i], m_LordIcon.activeSelf && i == end);
                }

                CustomAudioDataManager.GetInstance().PlayAudio(1090);

                GameMain.SC(PlayPokerEffect(pokerType));

                PlayRoleAnim("fapai");
                m_TipText.text = "";
            }
            else
            {
                m_PlayPokerList.Clear();
                CurPokerType = LandPokerType_Enum.LandPokerType_Error;
                m_TipText.text = (cards == null) ? "" : "不出";
                PlayRoleAnim("idle");
            }

            ShowCountdown(false);
            if (cards != null)
                GameMain.WaitForCall(0.3f, () => PlayPokerSound(haveRight, CurPokerType, value));
        {

            if (cards != null && cards.Length > 0)
            {
                m_PlayPokerList = new List<byte>(cards);
                m_PlayPokerList.Sort(LL_PokerType.SortByIndex);
                CurPokerType = pokerType;

                List<byte> list = new List<byte>(m_PlayPokerList);
                list.Reverse();
                for (; i < list.Count; i++)
                {
                    PlayCard.SetCardSprite(m_PlayCards.GetChild(i).gameObject, m_GameBase.CommonAssetBundle, list[i], false);
                }
            }
        }
    }
    {
        if (pokerType == LandPokerType_Enum.LandPokerType_PlaneNoWith
        || pokerType == LandPokerType_Enum.LandPokerType_PlaneWithOherPair
        || pokerType == LandPokerType_Enum.LandPokerType_PlaneWithOherSingle)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_feiji", 2f, 1087);
        }
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_zhadan", 2f, 1084);
        }
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_huojian_1", 1f, 1086, m_AnimTfm);

            yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_huojian_2", 1f, 1084);
        }
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_liandui", 2f, 1082, m_AnimTfm);
        }
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_shunzi", 2f, 1082, m_AnimTfm);
        }

        yield return new WaitForSecondsRealtime(1f);

        if (m_nCurPokerNum > 0 && m_nCurPokerNum < 3)
        {
            CustomAudioDataManager.GetInstance().PlayAudio((Sex == 1 ? 1079 : 2079) + m_nCurPokerNum);
            CustomAudioDataManager.GetInstance().PlayAudio(1003, false);

            OnPokerWarning();
        }
    {
        if (m_AlarmTfm.childCount == 0)
        {
            m_GameBase.PlayUIAnim("Anime_baojing", -1f, 0, m_AlarmTfm);
            CustomAudioDataManager.GetInstance().PlayAudio(1083);

    void PlayPokerSound(short haveRight, LandPokerType_Enum type, int value)
        int begin = (Sex == 1) ? 1000 : 2000;
        if (type == LandPokerType_Enum.LandPokerType_Error)
            CustomAudioDataManager.GetInstance().PlayAudio(begin + 62 + Random.Range(0, 4));
        else if (type == LandPokerType_Enum.LandPokerType_KingBlast || type == LandPokerType_Enum.LandPokerType_Four)
            float ratio = Random.Range(0f, 1f);
            if (haveRight > 0 || ratio < 0.5f)
            {
                if (type <= LandPokerType_Enum.LandPokerType_Three)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 13 + ((int)type - 1) * 13 + value);
                else if (type == LandPokerType_Enum.LandPokerType_PlaneNoWith
                    || type == LandPokerType_Enum.LandPokerType_PlaneWithOherPair
                    || type == LandPokerType_Enum.LandPokerType_PlaneWithOherSingle)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 55);
                else if (type == LandPokerType_Enum.LandPokerType_Flush)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 56);
                else if (type == LandPokerType_Enum.LandPokerType_SeriesPairs)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 57);
                else if (type == LandPokerType_Enum.LandPokerType_ThreeAndOne)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 58);
                else if (type == LandPokerType_Enum.LandPokerType_ThreeAndTwo)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 59);
                else if (type == LandPokerType_Enum.LandPokerType_FourAndTwoSingle)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 60);
                else if (type == LandPokerType_Enum.LandPokerType_FourAndTwoPair)
                    CustomAudioDataManager.GetInstance().PlayAudio(begin + 61);
            }
            else
            {
                CustomAudioDataManager.GetInstance().PlayAudio(begin + 66 + Random.Range(0, 3));
            }
        }
    {
        m_RecordPokerTfm.gameObject.SetActive(true);

        m_HavePokerList.Sort(LL_PokerType.SortByIndex);
        m_nCurPokerNum = (byte)m_HavePokerList.Count;

        int i = 0;
        foreach(Transform child in m_RecordPokerTfm)
        {
            if (i < m_HavePokerList.Count)
                PlayCard.SetCardSprite(child.gameObject, m_GameBase.CommonAssetBundle, m_HavePokerList[i], m_LordIcon.activeSelf && i  == 0, 0, "_small");
            else
                child.gameObject.SetActive(false);
            i++;
        }
    }
    public void OnDisOrReconnect(bool bDis)
    {
        if (m_DisconnectObj != null)
            m_DisconnectObj.SetActive(bDis);
    }
}