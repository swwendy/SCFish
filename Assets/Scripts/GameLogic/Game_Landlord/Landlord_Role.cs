using DG.Tweening;using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class Landlord_Role{    public uint m_nFaceId;    public string m_nUrl;    public long m_nTotalCoin;    public string m_cRoleName;    public string m_cMaster;    public byte m_nSit = RoomInfo.NoSit;    byte m_nIndex;    private byte m_nSex = 0;    protected byte Sex
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
    }    public LandPokerType_Enum CurPokerType { get; set; }    protected Transform m_InfoUI = null;    Transform m_SitUI = null;    protected Transform m_HaveCards, m_PlayCards, m_AnimTfm, m_AlarmTfm;    Transform m_RecordPokerTfm;    GameObject m_CountdownObj, m_DisconnectObj;    protected GameObject m_LordIcon;    Text m_TipText, m_ChatText;    protected byte m_nCurPokerNum;    public List<byte> m_HavePokerList = new List<byte>();    public List<byte> m_PlayPokerList = new List<byte>();    protected CGame_LandLords m_GameBase;    protected const float PokerInterval = 0.2f;    DragonBones.UnityArmatureComponent m_RoleAnim = null;    string m_strNextAnim;    public Landlord_Role(CGame_LandLords game, byte index)    {        m_GameBase = game;        Transform tfm = m_GameBase.MainUITfm.Find("Middle/PlayerInfor/Icon_PlayerInfor_" + index);        m_InfoUI = tfm.Find("PlayerBG");        m_CountdownObj = m_InfoUI.Find("OutlineCountdown").gameObject;        m_SitUI = tfm.Find("Button_Sitdown");        m_HaveCards = tfm.Find("PopUp_BG/Poker_Shoupai");        m_HaveCards.gameObject.SetActive(false);        m_PlayCards = tfm.Find("PopUp_BG/Poker_Chupai");        m_InfoUI.gameObject.SetActive(false);        m_TipText = tfm.Find("PopUp_BG/Tip_BG/Image_zhuangtai/Text").GetComponent<Text>();        m_AlarmTfm = tfm.Find("PopUp_BG/Tip_BG/Image_alarm");        m_LordIcon = tfm.Find("PopUp_BG/Icon_Zhuang").gameObject;        m_ChatText = tfm.Find("PopUp_BG/Tip_BG/Tip_Text/Text").GetComponent<Text>();
        tfm = tfm.Find("Text_offline");
        if(tfm != null)
            m_DisconnectObj = tfm.gameObject;

        m_AnimTfm = m_GameBase.AnimationTfm.parent.Find("Playerpoint_" + index);        m_RecordPokerTfm = m_GameBase.RecordUITfm.Find("Point_shoupai_" + index);        if(m_RecordPokerTfm != null)            m_RecordPokerTfm.gameObject.SetActive(false);        m_nIndex = index;    }    void CreateRole()
    {
        if (m_RoleAnim != null)
            return;

        UnityEngine.Object obj = (GameObject)m_GameBase.CommonAssetBundle.LoadAsset("Anime_ren_" + Sex);        if(obj != null)        {
            GameObject gameObj = (GameObject)GameMain.instantiate(obj);
            gameObj.transform.SetParent(m_InfoUI.parent.Find("point_ren"), false);

            bool bFlip = m_nIndex == 2;
            m_RoleAnim = gameObj.GetComponent<DragonBones.UnityArmatureComponent>();
            m_RoleAnim.armature.flipX = bFlip;
            m_RoleAnim.flipX = bFlip;
            m_RoleAnim.AddEventListener(DragonBones.EventObject.COMPLETE, AnimationEventHandler);
            m_strNextAnim = "idle";

            gameObj.SetActive(false);        }    }

    void AnimationEventHandler(string _type, DragonBones.EventObject eventObject)
    {
        if (_type == DragonBones.EventObject.COMPLETE)
        {
            PlayRoleAnim(m_strNextAnim);
        }
    }
    public void PlayRoleAnim(string anim, string nextAnim = "")
    {
        if (m_RoleAnim == null)
            return;

        m_RoleAnim.animation.Play(anim);
        if (nextAnim.Length > 0)
            m_strNextAnim = nextAnim;
    }    public virtual void Init()    {        OnEnd();        m_nFaceId = 0;        m_nUrl = "";        m_nTotalCoin = 0;        m_cRoleName = "";        m_nSit = RoomInfo.NoSit;

        if (m_RoleAnim != null)            GameObject.Destroy(m_RoleAnim.gameObject);        ShowPlayerInfo(false);
        OnDisOrReconnect(false);    }    public virtual void OnTick()    {    }    void ClearAnim()
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
        m_strNextAnim = "idle";    }    public virtual void OnEnd()    {        m_nCurPokerNum = 0;        m_LordIcon.gameObject.SetActive(false);        ShowCountdown(false);        ShowLordIcon(false);        m_HavePokerList.Clear();        m_PlayPokerList.Clear();        m_HaveCards.gameObject.SetActive(false);
        ShowEndPoker(false);
        OnDealPoker();        ClearAnim();        m_ChatText.transform.parent.gameObject.SetActive(false);
        CurPokerType = LandPokerType_Enum.LandPokerType_Error;

        if(m_RecordPokerTfm)
            m_RecordPokerTfm.gameObject.SetActive(false);
    }    public void SetupInfoUI(bool bContest, uint userId, float masterScore, byte male)    {        Transform tfm = m_InfoUI.Find("Head/HeadMask/ImageHead");        tfm.GetComponent<Image>().sprite = GameMain.hall_.GetIcon(m_nUrl, userId, (int)m_nFaceId);        tfm = m_InfoUI.Find("BG_nameJifen/Name_Text");        tfm.GetComponent<Text>().text = m_cRoleName;        tfm = m_InfoUI.Find("BG_nameJifen/Jifen_BG");        if (!m_GameBase.IsFree)        {
            tfm.gameObject.SetActive(true);
            tfm.Find("Text_Jifen").GetComponent<Text>().text = m_nTotalCoin.ToString();
            tfm.Find("Image_Jinbi").gameObject.SetActive(!bContest);
            tfm.Find("Image_Jifen").gameObject.SetActive(bContest);
        }        else            tfm.gameObject.SetActive(false);        tfm = m_InfoUI.Find("Text_dashifen");        tfm.GetComponent<Text>().text = CCsvDataManager.Instance.GameDataMgr.GetMasterLv(masterScore);        Sex = male;        CreateRole();        ShowPlayerInfo();    }    public Sprite GetHeadImage()
    {
        Transform tfm = m_InfoUI.Find("Head/HeadMask/ImageHead");
        return tfm.GetComponent<Image>().sprite;
    }    public void UpdateInfoUI()    {        Transform tfm = m_InfoUI.Find("BG_nameJifen/Jifen_BG/Text_Jifen");        tfm.GetComponent<Text>().text = m_GameBase.IsFree ? "" : m_nTotalCoin.ToString();    }    bool ShowPlayerInfo(bool show = true)    {        if ( m_InfoUI == null)            return false;        m_InfoUI.gameObject.SetActive(show);        m_SitUI.gameObject.SetActive(!show);        if(m_RoleAnim != null)            m_RoleAnim.gameObject.SetActive(show);        PlayRoleAnim("idle");        return true;    }    public virtual IEnumerator BeginPoker()    {
        m_HaveCards.gameObject.SetActive(true);        Transform tfm = m_HaveCards.Find("ImageIcon");        tfm.gameObject.SetActive(true);        m_HaveCards.Find("Poker_Show").gameObject.SetActive(false);        m_nCurPokerNum = 17;        byte pokerNum = 1;        Text text = tfm.GetComponentInChildren<Text>();        while(pokerNum <= m_nCurPokerNum)        {            text.text = pokerNum.ToString();            yield return new WaitForSecondsRealtime(PokerInterval);            pokerNum++;        }    }    public virtual void OnAskBeLord(ushort curPro, float time)    {        ShowCountdown(true, time);    }    protected void ShowCountdown(bool show, float time = 0f, CustomCountdownImgMgr.CallBackFunc fun = null, bool pause = false)    {        Image img = m_CountdownObj.transform.Find("ImageCountdown").GetComponent<Image>();        if(show)        {            if(time > 0f || fun == null)            {
                m_CountdownObj.SetActive(true);
                Text text = m_CountdownObj.GetComponentInChildren<Text>();
                if (!pause)
                    m_GameBase.CCIMgr.AddTimeImage(img, time, 1f, fun, text, false);                else
                {                    text.text = time.ToString();                    img.fillAmount = 1f;
                }            }            else
            {
                m_CountdownObj.SetActive(false);

                fun(0, false, img, "");
            }        }        else
        {            m_GameBase.CCIMgr.RemoveTimeImage(img);            m_CountdownObj.SetActive(false);
        }    }    public virtual void BackAskBeLord(byte askPro)    {        ShowCountdown(false);        m_TipText.text = (askPro == 0) ? "不叫" : (askPro.ToString() + "分");        //GameMain.WaitForCall(2f, () => m_TipText.text = "");        CustomAudioDataManager.GetInstance().PlayAudio((Sex == 1 ? 1007 : 2007) + askPro);    }    public virtual void ShowLordIcon(bool bLord)    {        m_LordIcon.SetActive(bLord);    }    public virtual void OnPokerNumChanged(byte num)
    {
        m_nCurPokerNum = num;
        m_HaveCards.gameObject.SetActive(true);        Transform tfm = m_HaveCards.Find("ImageIcon");
        tfm.gameObject.SetActive(m_nCurPokerNum != 0);
        if (m_nCurPokerNum != 0)            tfm.GetComponentInChildren<Text>().text = m_nCurPokerNum.ToString();        OnDealPoker();
        ShowCountdown(false);    }    public virtual void AddCards(byte[] cards, bool show = true)    {        if(m_GameBase.GameMode == GameTye_Enum.GameType_Record)
        {
            m_HavePokerList.AddRange(cards);
            UpdateRecordCards();
        }        else        {
            m_nCurPokerNum += (byte)cards.Length;
            OnPokerNumChanged(m_nCurPokerNum);        }    }    public virtual void RemoveCards(byte[] cards)    {        if (m_GameBase.GameMode == GameTye_Enum.GameType_Record)
        {
            foreach(byte card in cards)
                m_HavePokerList.Remove(card);
            UpdateRecordCards();
        }        else        {
            m_nCurPokerNum -= (byte)cards.Length;
            OnPokerNumChanged(m_nCurPokerNum);
        }    }    public virtual void OnAskDealPoker(bool bHaveRight, List<byte> curPokerList, LandPokerType_Enum pokerType, float askTime, float bankTime, bool pause = false)    {
        PlayRoleAnim("sikao");        ShowCountdown(true, askTime, (byte value, bool bClick, Image img, string userdata) =>        {
            if (bankTime >= 0f)
            {                ShowCountdown(true, bankTime, null, pause);
                return false;
            }            return true;        }, pause);        OnDealPoker();    }    //haveRight -1:not server 0:no right 1:have right    public virtual void OnDealPoker(short haveRight = -1, byte[] cards = null, LandPokerType_Enum pokerType = LandPokerType_Enum.LandPokerType_Error)    {        int i = 0;
        int value = 0;        if (haveRight >= 0)
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
                    m_PlayPokerList = childList;                value = GameCommon.GetCardValue(cards[0]);

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
                GameMain.WaitForCall(0.3f, () => PlayPokerSound(haveRight, CurPokerType, value));        }        else
        {            m_TipText.text = "";

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
        }        for (; i < m_PlayCards.childCount; i++)        {            m_PlayCards.GetChild(i).gameObject.SetActive(false);        }
    }    IEnumerator PlayPokerEffect(LandPokerType_Enum pokerType)
    {
        if (pokerType == LandPokerType_Enum.LandPokerType_PlaneNoWith
        || pokerType == LandPokerType_Enum.LandPokerType_PlaneWithOherPair
        || pokerType == LandPokerType_Enum.LandPokerType_PlaneWithOherSingle)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_feiji", 2f, 1087);
        }        if(pokerType == LandPokerType_Enum.LandPokerType_Four)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_zhadan", 2f, 1084);
        }        if (pokerType == LandPokerType_Enum.LandPokerType_KingBlast)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_huojian_1", 1f, 1086, m_AnimTfm);

            yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_huojian_2", 1f, 1084);
        }        if (pokerType == LandPokerType_Enum.LandPokerType_SeriesPairs)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("Anime_liandui", 2f, 1082, m_AnimTfm);
        }        if (pokerType == LandPokerType_Enum.LandPokerType_Flush)
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
        }    }    protected virtual void OnPokerWarning()
    {
        if (m_AlarmTfm.childCount == 0)
        {
            m_GameBase.PlayUIAnim("Anime_baojing", -1f, 0, m_AlarmTfm);
            CustomAudioDataManager.GetInstance().PlayAudio(1083);        }    }

    void PlayPokerSound(short haveRight, LandPokerType_Enum type, int value)    {
        int begin = (Sex == 1) ? 1000 : 2000;
        if (type == LandPokerType_Enum.LandPokerType_Error)
            CustomAudioDataManager.GetInstance().PlayAudio(begin + 62 + Random.Range(0, 4));
        else if (type == LandPokerType_Enum.LandPokerType_KingBlast || type == LandPokerType_Enum.LandPokerType_Four)        {            CustomAudioDataManager.GetInstance().PlayAudio(begin + 4, false);            CustomAudioDataManager.GetInstance().PlayAudio(begin + (type == LandPokerType_Enum.LandPokerType_KingBlast ? 53 : 54));        }        else        {
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
        }    }    public virtual void ShowEndPoker(bool show, byte[] cards = null)    {        m_HaveCards.gameObject.SetActive(true);        m_HaveCards.Find("ImageIcon").gameObject.SetActive(false);        Transform tfm = m_HaveCards.Find("Poker_Show");        tfm.gameObject.SetActive(show);        if (!show || cards == null)            return;        List<byte> endPokerList = new List<byte>(cards);        endPokerList.Sort(LL_PokerType.SortByIndex);        int i = 0;        Transform tfm1;        for (; i < endPokerList.Count; i++)        {            tfm1 = tfm.Find("Poker_Show_Icon_" + i);            tfm1.gameObject.SetActive(true);            Sprite sp = m_GameBase.CommonAssetBundle.LoadAsset<Sprite>(GameCommon.GetPokerMat(endPokerList[i]));            tfm1.GetComponent<Image>().sprite = sp;        }        for (; i < tfm.childCount; i++)            tfm.Find("Poker_Show_Icon_" + i).gameObject.SetActive(false);        for (i = 0; i < m_PlayCards.childCount; i++)        {            m_PlayCards.GetChild(i).gameObject.SetActive(false);        }    }    public void OnChat(byte index)    {        int id = 23000 + index;        m_ChatText.text = CCsvDataManager.Instance.TipsDataMgr.GetTipsText((uint)id);        if (Sex != 1)            id += 1000;        CustomAudioDataManager.GetInstance().PlayAudio(id);        GameObject obj = m_ChatText.transform.parent.gameObject;        obj.SetActive(true);        GameMain.WaitForCall(3f, () => obj.SetActive(false));    }    public virtual void UpdateRecordCards()
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