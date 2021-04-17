using DG.Tweening;using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class GuanDan_Role{    public long m_nTotalCoin;    public byte m_nSrvSit = RoomInfo.NoSit;    public byte Rank { get; private set; }    private byte m_nSex = 0;    public byte Sex
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
    }    public List<byte> m_HavePokerList = new List<byte>();    public List<byte> m_PlayPokerList = new List<byte>();    public GuandanPokerType_Enum CurPokerType { get; set; }    protected const float PokerInterval = 0.15f;    protected Transform m_InfoUI = null;    protected Transform m_SitUI = null;    public Transform m_PlayCards;    protected Transform m_HaveCards, m_RankTfm, m_AnimTfm;    protected GameObject m_CountdownObj;    protected Text m_TipText, m_ChatText;    protected CGame_GuanDan m_GameBase;    protected DragonBones.UnityArmatureComponent m_RoleAnim = null;    string m_strNextAnim;    public int m_nFaceId;    public string m_szUrl;    public GuanDan_Role(CGame_GuanDan game, byte index)    {        m_GameBase = game;        m_AnimTfm = game.MainUITfm.Find("Pop-up/Animation/Playerpoint_" + index);    }    public virtual void CreateRole()
    {

    }    protected void CreateRole(bool bFlip, Transform parent)
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

            m_strNextAnim = "idle";        }    }

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
    }    public virtual void Init()    {        OnEnd();        m_nTotalCoin = 0;        m_nSrvSit = RoomInfo.NoSit;        ShowPlayerInfo(false);        OnDisOrReconnect(false);    }    public virtual void OnTick()    {        if(m_CountdownObj != null && m_CountdownObj.activeSelf)
        {
            Image img = m_CountdownObj.transform.Find("shine").GetComponent<Image>();

            float time = 0.5f;
            float lerp = Mathf.PingPong(Time.time, time)/time;

            Color c = img.color;
            c.a = lerp;
            img.color = c;
        }    }    public virtual void OnEnd()    {
        m_InfoUI.Find("Head").gameObject.SetActive(true);        m_ChatText.transform.parent.gameObject.SetActive(false);
        m_RankTfm.gameObject.SetActive(false);
        Rank = 0;
        m_HavePokerList.Clear();        m_PlayPokerList.Clear();
        CurPokerType = GuandanPokerType_Enum.GuanPokerType_Error;
        OnDealPoker();
        ShowRole(false);        ShowCountdown(false);

        foreach (Transform childTfm in m_PlayCards)            GameObject.Destroy(childTfm.gameObject);    }    protected void SetupInfoUI(Transform infoUI, bool bContest, long coin, uint userId, string name, string url, int faceId, float masterScore)
    {
        Transform tfm = infoUI.Find("Head/HeadMask/ImageHead");        tfm.GetComponent<Image>().sprite = GameMain.hall_.GetIcon(url, userId, faceId);        tfm = infoUI.Find("BG_nameJifen/Name_Text");        tfm.GetComponent<Text>().text = name;        tfm = infoUI.Find("BG_nameJifen/Jifen_BG");        if (!m_GameBase.IsFree)        {
            tfm.gameObject.SetActive(true);
            tfm.Find("Text_Jifen").GetComponent<Text>().text = m_nTotalCoin.ToString();
            tfm.Find("Image_Jinbi").gameObject.SetActive(!bContest);
            tfm.Find("Image_Jifen").gameObject.SetActive(bContest);
        }        else            tfm.gameObject.SetActive(false);
        tfm = infoUI.Find("Text_dashifen");        tfm.GetComponent<Text>().text = CCsvDataManager.Instance.GameDataMgr.GetMasterLv(masterScore);        ShowRole();    }    public virtual void SetupInfoUI(bool bContest, long coin, uint userId, string name, string url = "", int faceId = 0, float masterScore = 0f, byte male = 1)    {
        m_nTotalCoin = coin;        if(url.Length > 0)            m_szUrl = url;        if(faceId != 0)            m_nFaceId = faceId;        Sex = male;        SetupInfoUI(m_InfoUI, bContest, coin, userId, name, m_szUrl, m_nFaceId, masterScore);        ShowPlayerInfo();
    }

    public void UpdateInfoUI()    {        Transform tfm = m_InfoUI.Find("BG_nameJifen/Jifen_BG/Text_Jifen");        tfm.GetComponent<Text>().text = m_GameBase.IsFree ? "" : m_nTotalCoin.ToString();    }    protected bool ShowPlayerInfo(bool show = true)    {        if ( m_InfoUI == null)            return false;        m_InfoUI.gameObject.SetActive(show);        m_SitUI.gameObject.SetActive(!show);        return true;    }

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

    public virtual void RemoveCards(byte[] cards)    {    }

    public virtual void AddCards(byte[] cards)    {    }

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
        {            if (m_RankTfm.gameObject.activeSelf)            {                Rank = 0;                m_RankTfm.gameObject.SetActive(false);            }
            return;        }        if (m_RankTfm.gameObject.activeSelf)
            return;

        Rank = rank;
        m_RankTfm.gameObject.SetActive(true);
        m_RankTfm.Find("TextNum").GetComponent<Text>().text = rank.ToString();

        //string[] strs = new string[] { "", "头游", "二游", "三游", "末游" };
        //m_RankTfm.FindChild("TextName").GetComponent<Text>().text = strs[rank];

        m_RankTfm.GetComponent<Image>().sprite = m_GameBase.GuanDanAssetBundle.LoadAsset<Sprite>("gd_icon_" + rank);
    }


    //haveRight -1:not server 0:no right 1:have right
    public virtual void OnDealPoker(sbyte haveRight = -1, byte[] cards = null, GuandanPokerType_Enum pokerType = GuandanPokerType_Enum.GuanPokerType_Error, byte rank = 0)    {        foreach (Transform childTfm in m_PlayCards)            GameObject.Destroy(childTfm.gameObject);        if (haveRight >=0)
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
                        m_PlayPokerList = childList;                value = GameCommon.GetCardValue(m_PlayPokerList[0]);

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

            ShowRank(rank);        }        else
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
                    }                }                else
                {
                    m_PlayPokerList.Clear();
                    CurPokerType = GuandanPokerType_Enum.GuanPokerType_Error;
                    m_TipText.text = "不出";
                }            }
        }    }    public Image GetHeadImg()
    {
        Transform tfm = m_InfoUI.Find("Head/HeadMask/ImageHead");
        return tfm.GetComponent<Image>();
    }    public string GetName()
    {
        return m_InfoUI.Find("BG_nameJifen/Name_Text").GetComponent<Text>().text;
    }    public string GetMasterLv()
    {
        return m_InfoUI.Find("Text_dashifen").GetComponent<Text>().text;
    }    public virtual void ChangeSit(bool bFly, float time, Vector3 targetPos, GuanDan_Role targetRole)
    {
        OnDealPoker();
    }    public void ShowRole(bool show = true)
    {
        if(show)
        {            if (m_RoleAnim == null)
                CreateRole();
        }        else        {
            if (m_RoleAnim != null)
                GameObject.Destroy(m_RoleAnim.gameObject);            m_RoleAnim = null;        }
        if (m_RoleAnim != null)
        {
            m_RoleAnim.gameObject.SetActive(show);
            PlayRoleAnim("idle");        }        m_InfoUI.Find("Head").gameObject.SetActive(!show);    }

    public virtual void OnAskDealPoker(bool bHaveRight, List<byte> curPokerList, GuandanPokerType_Enum pokerType, float askTime, float bankTime, bool pause = false)    {
        PlayRoleAnim("sikao");        ShowCountdown(true, askTime, (byte value, bool bClick, Image img, string userdata) =>        {
            if (bankTime >= 0f)
            {                ShowCountdown(true, bankTime, null, pause);
                return false;
            }            return true;        }, pause);        OnDealPoker();    }

    protected void ShowCountdown(bool show, float time = 0f, CustomCountdownImgMgr.CallBackFunc fun = null, bool pause = false)    {
        Image img = m_CountdownObj.GetComponent<Image>();
        m_CountdownObj.SetActive(show && (time > 0f || fun == null));        if(time >= 0f)            m_GameBase.ShowCountdown(show, img, time, fun, pause);    }

    public virtual void ShowEndPoker(byte[] cards)    {    }

    //state: 0:显示按钮 1：显示贡牌 2:收贡 submitOrReturn:上贡还是还贡
    public virtual void OnGong(byte state, bool submitOrReturn, byte poker = 0, Transform srcTfm = null, bool bReverse = false)
    {
        if (state == 1)
        {
            if (!bReverse)
            {
                ShowTipPoker(submitOrReturn ? poker : RoomInfo.NoSit, 0);
                RemoveCards(new byte[] { poker });            }            else
            {
                OnDealPoker();
                AddCards(new byte[] { poker });
            }            ShowCountdown(false, -1f);        }        else if (state == 2)
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
            });        }        else            ShowCountdown(true, -1f);    }

    public void OnChat(byte index)    {        int id = 23000 + index;        m_ChatText.text = CCsvDataManager.Instance.TipsDataMgr.GetTipsText((uint)id);        if (Sex != 1)            id += 1000;        CustomAudioDataManager.GetInstance().PlayAudio(id);        GameObject obj = m_ChatText.transform.parent.gameObject;        obj.SetActive(true);        GameMain.WaitForCall(3f, () => obj.SetActive(false));    }

    void PlayPokerSound(sbyte haveRight, GuandanPokerType_Enum type, int value)    {
        int begin = (Sex == 1) ? 1000 : 2000;
        if (type == GuandanPokerType_Enum.GuanPokerType_Error)
            CustomAudioDataManager.GetInstance().PlayAudio(begin + 410 + Random.Range(0, 3));
        else if (type == GuandanPokerType_Enum.GuanPokerType_KingBlast || type == GuandanPokerType_Enum.GuanPokerType_Blast)        {            CustomAudioDataManager.GetInstance().PlayAudio(begin + (type == GuandanPokerType_Enum.GuanPokerType_KingBlast ? 405 : 406 + Random.Range(0, 2)));        }
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
        }        if (pokerType == GuandanPokerType_Enum.GuanPokerType_SameColorFlush)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_tonghuashun", 0.97f, 0, m_AnimTfm);
        }        if (pokerType == GuandanPokerType_Enum.GuanPokerType_Blast)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_GDzhadan", 2f, 1009, m_AnimTfm);
        }        if (pokerType == GuandanPokerType_Enum.GuanPokerType_KingBlast)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_GDwangzha1", 1f, 0, m_AnimTfm);

            yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_GDwangzha2", 1f, 0);
        }        if (pokerType == GuandanPokerType_Enum.GuanPokerType_SeriesPairs)
        {
            //yield return new WaitForSecondsRealtime(1f);

            m_GameBase.PlayUIAnim("anime_GDliandui", 2f, 0, m_AnimTfm);
        }        if (pokerType == GuandanPokerType_Enum.GuanPokerType_Flush)
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
    }}