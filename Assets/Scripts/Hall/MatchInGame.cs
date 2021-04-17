using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using USocket.Messages;
using XLua;using DG.Tweening;
using UnityEngine.EventSystems;

[LuaCallCSharp]
public class RoundResultData
{
    public Sprite headImg;
    public string name;
    public long addCoin;
    public long coin;
    public uint playerid;
}[LuaCallCSharp]
public enum ContestPlayerState_enum
{
    ContestPlayerState_Gameing,     //游戏中
    ContestPlayerState_UpGrade,     //玩家可以晋级
    ContestPlayerState_Out,         //玩家被淘汰
    ContestPlayerState_WaitResult,  //玩家在等待结果
    ContestPlayerState_Bye,         //玩家处于轮空状态

    ContestPlayerState_end
};

[Hotfix]public class MatchInGame : MonoBehaviour
{
    static MatchInGame m_Instance;
    Transform ProcessTfm { get; set; }
    Transform ResultTfm { get; set; }
    GameObject RoundResultObj { get; set; }
    Coroutine m_CoutdownCoroutin = null;
    Transform TablesViewTfm { get; set; }
    Transform RebuyTfm { get; set; }
    GameObject RankEffectObj { get; set; }
    GameObject ByStanderObj { get; set; }

    public byte m_nCurTurn = 1;
    byte m_nMaxTurn = 2;
    byte m_nCurRound = 1;
    byte m_nRoundPerTurn = 3;

    bool m_bNeedUpdateRank = true;
    bool m_bNeedUpdateDesk = true;

    public float contestTimeLeft;
    int m_nRoleNum = 1;

    void Awake()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_AdmissionDisband_NotifyPlayer, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_ContestPlayerByePromotion, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_RoundEndDeskRoleRank, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_PlayerPromotionRank, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_ContestScoreRank, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_PacketContestInfoToGameServer, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_BackDeskInfoToPlayer, HandleDeskInfo);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_BackGameingRankToPlayer, HandleRankInfo);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_UpdatePlayerRankAfterOneOver, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_RequestbuyEnterNextRoundReply, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_SeekSubstitutes, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_RequestSubstitutesReply, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_RoundTimeOverForceEndGameingDesk, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_MIDDLEENTERCONTEST, HandleMiddleEnterContest);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKTOBEONLOOKER, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Contestmsg_AnyTimeContestCurPlayerCount, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.ContestMsg_CommandContestDisband, HandleGameNetMsg);    }

    private void Update()
    {
        if (contestTimeLeft > 0)            contestTimeLeft -= Time.deltaTime;    }

    void Init()
    {
        if (GameMain.hall_.GameBaseObj == null || ProcessTfm != null)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        Transform canvasTfm = GameObject.Find("Canvas/Root").transform;
        if (canvasTfm == null)
            return;

        string PrefabName = CCsvDataManager.Instance.GameUIDataMgr.GetGameUIPrefabName("MatchUI", GameMain.hall_.GameBaseObj.GetGameType());
        GameObject obj = (GameObject)bundle.LoadAsset(PrefabName);
        obj = Instantiate(obj);
        obj.SetActive(false);
        ProcessTfm = obj.transform;
        ProcessTfm.SetParent(canvasTfm, false);

        ProcessTfm.Find("Waiting_Startgame/Button_close").GetComponent<Button>().
            onClick.AddListener(OnClickReturn);
        ProcessTfm.Find("Waiting_fixedNum/Button_close").GetComponent<Button>().
            onClick.AddListener(OnClickReturn);
        ProcessTfm.Find("Waiting_Nextgame_new/ButtonChakan").GetComponent<Button>().
            onClick.AddListener(OnClickTablesView);
        ProcessTfm.Find("ImageLeftBG").GetComponent<Button>().
            onClick.AddListener(OnClickRankView);
        ProcessTfm.Find("ImageLeftBG").gameObject.SetActive(false);
        ProcessTfm.Find("ImageLeftBG/Text_mingci/Text_num").GetComponent<Text>().text = "";

        TablesViewTfm = ProcessTfm.Find("Pop-up/Match_jindu");
        TablesViewTfm.gameObject.SetActive(false);

        obj = (GameObject)bundle.LoadAsset("Match_result");
        obj = Instantiate(obj);
        obj.SetActive(false);
        ResultTfm = obj.transform;
        ResultTfm.SetParent(canvasTfm, false);

        ResultTfm.Find("Button_return").GetComponent<Button>().
            onClick.AddListener(OnClickReturn);
        ResultTfm.Find("buttomBG/Button_ok").GetComponent<Button>().
            onClick.AddListener(OnClickResultOK);
        //ResultTfm.FindChild("buttomBG/Button_share").GetComponent<Button>().
        //    onClick.AddListener(OnClickResultShare);
        ResultTfm.Find("buttomBG/Button_share").gameObject.SetActive(false);

        RoundResultObj = null;

        obj = (GameObject)bundle.LoadAsset("Match_result_buytickets");
        obj = Instantiate(obj);
        obj.SetActive(false);
        RebuyTfm = obj.transform;
        RebuyTfm.SetParent(canvasTfm, false);
        RebuyTfm.Find("Buttonbuy").GetComponent<Button>().onClick.AddListener(OnClickRebuy);

        obj = (GameObject)bundle.LoadAsset("Mask_Onlooker");
        ByStanderObj = Instantiate(obj);
        ByStanderObj.SetActive(false);
        ByStanderObj.transform.SetParent(canvasTfm, false);
        ByStanderObj.transform.Find("Button_ExitOnlooker").GetComponent<Button>().
            onClick.AddListener(OnClickExitBystander);

        StartCoroutine(PreloadResource());
    }

    IEnumerator PreloadResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            yield break;

        yield return null;

        GameFunction.PreloadPrefab(bundle, "Contest_Result_Q");
        yield return null;

        GameFunction.PreloadPrefab(bundle, "Contest_Result_sh");
        yield return null;

        GameFunction.PreloadPrefab(bundle, "Anime_match_win");
        yield return null;

        GameFunction.PreloadPrefab(bundle, "Anime_match_lost");
        yield return null;
    }

    public static MatchInGame GetInstance()    {        if (m_Instance == null)            m_Instance = GameMain.Instance.gameObject.AddComponent<MatchInGame>();        m_Instance.Init();        return m_Instance;    }

    void OnTotalEnd()
    {
        m_nCurTurn = 1;
        m_nCurRound = 1;
        m_nRoleNum = 1;

        m_bNeedUpdateRank = true;
        m_bNeedUpdateDesk = true;

        Transform tfm = ResultTfm.Find("ImageBG_ItemTips/ItemBG");
        foreach (Transform child in tfm)
            GameObject.Destroy(child.gameObject);

        if (RoundResultObj != null)
            GameObject.Destroy(RoundResultObj);

        ProcessTfm.gameObject.SetActive(false);
        ResultTfm.gameObject.SetActive(false);
        TablesViewTfm.gameObject.SetActive(false);
        ProcessTfm.Find("ImageLeftBG").gameObject.SetActive(false);
        ProcessTfm.Find("ImageLeftBG/Text_mingci/Text_num").GetComponent<Text>().text = "";
        if (RankEffectObj != null)
            GameObject.Destroy(RankEffectObj);

        StopAllCoroutines();
        m_CoutdownCoroutin = null;
        CancelInvoke("GetDeskInfoFail");
        CancelInvoke("GetRankInfoFail");

        GameMain.hall_.SwitchToHallScene(true, 3);
    }

    public void OnClickReturn()
    {
        if(ResultTfm.gameObject.activeSelf)
        {
            OnTotalEnd();
        }
        else            CCustomDialog.OpenCustomDialogWithTipsID(1603, Giveup);    }

    void Giveup(object param)
    {
        if (param != null && (int)param == 0)            return;        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_PlayerExitContest);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(ContestDataManager.Instance().currentContestID);
        NetWorkClient.GetInstance().SendMsg(msg);

        OnTotalEnd();
    }

    public void ShowWait(int timeLeft = 0)
    {
        ProcessTfm.gameObject.SetActive(true);

        Transform tfm;
        stContestData cd = ContestDataManager.Instance().GetCurrentContestData();
        if(cd != null)
        {
            if (cd.enContestType == ContestType.ContestType_Timing)
            {
                tfm = ProcessTfm.Find("Waiting_Startgame");
                if (!tfm.gameObject.activeSelf)
                {
                    tfm.gameObject.SetActive(true);

                    tfm = tfm.Find("CountdownBG");
                    StartCountDown((uint)(timeLeft > 0 ? timeLeft : contestTimeLeft), tfm.Find("Text_1").GetComponent<Text>(),
                        tfm.Find("Text_3").GetComponent<Text>(), tfm.Find("ImageCountdown").GetComponent<Image>(), "", null,
                        tfm.Find("Text_2").GetComponent<Text>(), tfm.Find("Text_4").GetComponent<Text>());
                }
            }
            else
            {
                tfm = ProcessTfm.Find("Waiting_fixedNum");
                tfm.gameObject.SetActive(true);

                tfm = tfm.Find("TextNum/num");
                tfm.GetComponent<Text>().text = m_nRoleNum + "/" + cd.nMaxEnrollPlayerNum;
            }
        }
        contestTimeLeft = 0f;

        AudioManager.Instance.PlayBGMusic(GameDefine.HallAssetbundleName, "hall");
    }

    void UpdateWaitRole(ushort cur)
    {
        m_nRoleNum = cur;

        if(ProcessTfm != null)
        {
            stContestData cd = ContestDataManager.Instance().GetCurrentContestData();
            Transform tfm = ProcessTfm.Find("Waiting_fixedNum/TextNum/num");
            tfm.GetComponent<Text>().text = cur + "/" + cd.nMaxEnrollPlayerNum;
        }
    }

    public void SetContestData(ushort roundPerTurn, int roleNum)
    {
        DebugLog.Log("SetContestData:" + roundPerTurn + "," + roleNum);

        if (ProcessTfm == null || roleNum == 0)
            return;

        m_nRoundPerTurn = (byte)roundPerTurn;
        m_nRoleNum = roleNum;

        Transform tfm = ProcessTfm.Find("ImageLeftBG");
        Text t = tfm.Find("Text_mingci/Text_num").GetComponent<Text>();
        if (t.text.Length == 0)
            t.text = "无/"+roleNum;
        else
        {
            string str = t.text;
            int index = str.IndexOf('/');
            str = str.Substring(0, index + 1);
            t.text = str + roleNum;
        }

        m_bNeedUpdateDesk = true;
        m_bNeedUpdateRank = true;
    }

    public void ResetGui()
    {
        HideRoundResult(false);
        ProcessTfm.gameObject.SetActive(true);
        ProcessTfm.Find("Waiting_Startgame").gameObject.SetActive(false);
        ProcessTfm.Find("Waiting_Nextgame_new").gameObject.SetActive(false);
        ProcessTfm.Find("Waiting_fixedNum").gameObject.SetActive(false);
        TablesViewTfm.gameObject.SetActive(false);
        CloseRebuyUI(true);
    }

    public void ShowBegin(bool bystander = false, byte curRound = 0, byte maxRound = 0)
    {
        DebugLog.Log("showBegin:" + bystander + "," + curRound + "," + maxRound);

        //if(playAnim)
        //{
        //    AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        //    GameObject obj = (GameObject)bundle.LoadAsset("Anime_startgame");
        //    obj = Instantiate(obj);
        //    obj.transform.SetParent(ProcessTfm.FindChild("Pop-up/Animation/point_startgame"), false);
        //    GameObject.Destroy(obj, 1f);
        //}

        ResetGui();

        Transform tfm = ProcessTfm.Find("ImageLeftBG");
        Text t = tfm.Find("Text_lunshu/Text_num").GetComponent<Text>();
        t.text = m_nCurTurn + "/" + m_nMaxTurn;

        t = tfm.Find("Text_jushu/Text_num").GetComponent<Text>();
        if (curRound != 0)
            m_nCurRound = curRound;
        if (maxRound != 0)
            m_nRoundPerTurn = maxRound;
        t.text = m_nCurRound + "/" + m_nRoundPerTurn;

        tfm.gameObject.SetActive(true);
        ByStanderObj.SetActive(bystander);
    }

    void StartCountDown(uint secondLeft, Text min, Text sec = null, Image timeImg = null
        , string addStrFormat = "", UnityAction action = null, Text min2 = null, Text sec2 = null)
    {
        if (m_CoutdownCoroutin != null)
            StopCoroutine(m_CoutdownCoroutin);
        m_CoutdownCoroutin = StartCoroutine(ShowCountDown(secondLeft, min, sec, timeImg, addStrFormat, action, min2, sec2));
    }

    IEnumerator ShowCountDown(uint secondLeft, Text min, Text sec, Image timeImg
        , string addStrFormat, UnityAction action, Text min2, Text sec2)
    {
        int nTime = (int)secondLeft;

        while ( nTime >= 0)
        {
            if (timeImg != null)
            {
                timeImg.fillAmount = (float)nTime / secondLeft;
            }

            if (min != null)
            {
                int nMin = nTime / 60;
                int nSec = nTime % 60;

                if (sec != null)
                {
                    if(min2 != null && sec2 != null)
                    {
                        string str = nMin.ToString("D2");
                        min.text = str[0].ToString();
                        min2.text = str[1].ToString();

                        str = nSec.ToString("D2");
                        sec.text = str[0].ToString();
                        sec2.text = str[1].ToString();
                    }
                    else
                    {
                        min.text = nMin.ToString("D2");
                        sec.text = nSec.ToString("D2");
                    }
                }
                else
                    min.text = nMin.ToString("D2") + ":" + nSec.ToString("D2");
            }
            else if(sec != null)
            {
                if (addStrFormat.Length > 0)
                    sec.text = string.Format(addStrFormat, nTime);
                else
                    sec.text = nTime.ToString();
            }

            yield return new WaitForSecondsRealtime(1f);

            nTime -= 1;
        }

        yield return null;

        if (action != null)
            action();
    }

    void HideRoundResult(bool forceClear)
    {
        if (RoundResultObj != null && RoundResultObj.activeSelf)
        {
            Transform tfm = RoundResultObj.transform.Find("Button_Confirm");
            Button btn = tfm.GetComponent<Button>();
            btn.onClick.Invoke();
        }
        else if(forceClear)
        {
            if (GameMain.hall_.GameBaseObj != null)
                GameMain.hall_.GameBaseObj.OnDisconnect();
        }

        if (ByStanderObj != null)
            ByStanderObj.SetActive(false);

        if (!ProcessTfm.gameObject.activeSelf)
            CustomAudio.GetInstance().PlayCustomAudio(1001, true);
    }

    void InitNextGame(List<short> promotionList)
    {
        if (RankEffectObj != null)
            return;

        Transform tfm = ProcessTfm.Find("Waiting_Nextgame_new/Viewport_round");

        Image img = tfm.Find("rankingBg/Image_ranking/Image_HeadBG/Image_HeadMask/Image_HeadImage").GetComponent<Image>();
        img.sprite = GameMain.hall_.GetPlayerIcon();

        img = tfm.Find("ImagelineBG/Imageline").GetComponent<Image>();        img.fillAmount = 0f;        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        GameObject obj = (GameObject)bundle.LoadAsset("Contest_Result_Q");
        RankEffectObj = Instantiate(obj);
        RankEffectObj.name = "effect";

        tfm = tfm.Find("ImageBg");
        for(int i = tfm.childCount - 1; i >=0; i-- )//reset
        {
            Transform turnTfm = tfm.GetChild(i);
            if (turnTfm.name == "Matchprocess_start")
            {
                HighlightTurn(turnTfm, true);
                RankEffectObj.transform.SetParent(turnTfm, false);
            }
            else if (turnTfm.name == "Matchprocess_end")
            {
                HighlightTurn(turnTfm, false);
            }
            else
                GameObject.DestroyImmediate(turnTfm.gameObject);
        }

        int add = promotionList.Count - 2;
        if(add > 0)
        {
            float offset = img.rectTransform.rect.width;
            offset /= (add + 1);
            GameObject assetObj = (GameObject)bundle.LoadAsset("Matchprocess_Next_new");
            RectTransform rtf;
            Vector2 pos = Vector2.zero;
            for (int i = 0; i < add; i++)
            {
                pos.x += offset;
                obj = Instantiate(assetObj);
                obj.transform.SetParent(tfm, false);
                obj.transform.SetSiblingIndex(i + 1);
                rtf = obj.transform as RectTransform;
                rtf.anchoredPosition = pos;
            }
        }

        Transform child;
        string str;
        for(int i = 0; i < promotionList.Count; i++)
        {
            child = tfm.GetChild(i);

            str = "第" + (i + 1) + "轮";
            child.Find("Image_1/Text_lun").GetComponent<Text>().text = str;
            child.Find("Image_2/Text_lun").GetComponent<Text>().text = str;

            if(promotionList[i] > 0)
            {
                str = "前"+promotionList[i] + "人";
                child.Find("Image_1/Text_jinji").GetComponent<Text>().text = str;
                child.Find("Image_2/Text_jinji").GetComponent<Text>().text = str;
            }
        }

        m_nCurTurn = 1;
        m_nCurRound = 1;
        m_nMaxTurn = (byte)promotionList.Count;
    }

    void HighlightTurn(Transform tfm, bool light)
    {
        tfm.Find("Image_1").gameObject.SetActive(!light);
        tfm.Find("Image_2").gameObject.SetActive(light);
    }

    void ShowPromotion(int rank = 0,bool MiddleEnterState = false,uint time = 30)
    {
        HideRoundResult(true);

        ProcessTfm.gameObject.SetActive(true);
        Transform tfm;
        tfm = ProcessTfm.Find("Waiting_Nextgame_new");
        tfm.gameObject.SetActive(true);
        TablesViewTfm.gameObject.SetActive(false);

        if (rank >= 0)
        {
            tfm = ProcessTfm.Find("Waiting_Nextgame_new/Imagebottom/Text_jizhuo");
            tfm.gameObject.SetActive(false);
            tfm = ProcessTfm.Find("Waiting_Nextgame_new/Imagebottom/Text_xiayiju");
            tfm.gameObject.SetActive(true);
        }

        StartCountDown(time, ProcessTfm.Find("Waiting_Nextgame_new/Imagebottom/TextTime").GetComponent<Text>());

        UpdateNextGame(rank, true);

        if (m_nCurTurn < m_nMaxTurn)
        {
            tfm = ProcessTfm.Find("Waiting_Nextgame_new/Viewport_round");
            RectTransform rtfm = tfm.Find("rankingBg/Image_ranking") as RectTransform;
            Image img = tfm.Find("ImagelineBG/Imageline").GetComponent<Image>();

            RankEffectObj.SetActive(false);

            RectTransform targetTfm = tfm.Find("ImageBg").GetChild(m_nCurTurn) as RectTransform;
            DOTween.To(() => rtfm.anchoredPosition,
                r =>
                {
                    rtfm.anchoredPosition = r;
                    img.fillAmount = r.x / img.rectTransform.rect.width;
                },
                targetTfm.anchoredPosition, 0.5f).OnComplete(() =>
                {
                    HighlightTurn(targetTfm, true);

                    img.fillAmount = targetTfm.anchoredPosition.x / img.rectTransform.rect.width;

                    RankEffectObj.SetActive(true);
                    RankEffectObj.transform.SetParent(targetTfm, false);

                });

            if(!MiddleEnterState)
            {
                m_nCurTurn++;
            }
        }

        m_nCurRound = 1;
    }

    void ShowNextGame(int rank, uint secondLeft, ushort playingDesk, ushort deskNum, bool bPromotion = false)
    {
        HideRoundResult(true);

        ProcessTfm.gameObject.SetActive(true);
        TablesViewTfm.gameObject.SetActive(false);

        Transform tfm;
        tfm = ProcessTfm.Find("Waiting_Nextgame_new");
        tfm.gameObject.SetActive(true);

        if(rank != 0)
        {
            tfm = ProcessTfm.Find("Waiting_Nextgame_new/Imagebottom/Text_jizhuo");
            tfm.gameObject.SetActive(rank > 0);
            tfm = ProcessTfm.Find("Waiting_Nextgame_new/Imagebottom/Text_xiayiju");
            tfm.gameObject.SetActive(rank < 0);
        }

        UpdateNextGame(rank, bPromotion, playingDesk, deskNum);
        StartCountDown(secondLeft,
            ProcessTfm.Find("Waiting_Nextgame_new/Imagebottom/TextTime").GetComponent<Text>());

        AudioManager.Instance.PlayBGMusic(GameDefine.HallAssetbundleName, "hall");
    }

    //rank =0:不更新 <0:轮空 >0:名次
    void UpdateNextGame(int ranking, bool bPromotion, uint playingDeskNum = 0, uint totalDeskNum = 0)
    {
        Transform tfm;
        if (!bPromotion)
        {
            tfm = ProcessTfm.Find("Waiting_Nextgame_new/Imagebottom/Text_jizhuo");
            Text t = tfm.Find("TextNum").GetComponent<Text>();
            string str = t.text;
            if (totalDeskNum > 0)
                str = playingDeskNum + "/" + totalDeskNum;
            else
            {
                int index = str.IndexOf('/');
                str = str.Substring(index);
                str = playingDeskNum + str;
            }
            t.text = str;
        }

        tfm = ProcessTfm.Find("Waiting_Nextgame_new/Viewport_round/rankingBg/Image_ranking");
        Transform cornTfm = ProcessTfm.Find("ImageLeftBG");
        if (ranking == 0)
        {
            cornTfm.gameObject.SetActive(tfm.Find("Text_ranking").gameObject.activeSelf);
        }
        else if (ranking < 0)
        {
            tfm.Find("Text_ranking_lunkong").gameObject.SetActive(true);
            tfm.Find("Text_ranking").gameObject.SetActive(false);
            cornTfm.gameObject.SetActive(false);
        }
        else
        {
            tfm.Find("Text_ranking_lunkong").gameObject.SetActive(false);
            tfm.Find("Text_ranking").gameObject.SetActive(true);

            tfm = tfm.Find("Text_ranking/TextNum");
            tfm.GetComponent<Text>().text = ranking.ToString();

            tfm = cornTfm.Find("Text_mingci/Text_num");
            Text t = tfm.GetComponent<Text>();
            t.text = ranking + "/" + m_nRoleNum;
            cornTfm.gameObject.SetActive(true);
        }

        m_bNeedUpdateRank = true;
        m_bNeedUpdateDesk = true;
    }

    void UpdateCurTurnUI()
    {
        Transform parent = ProcessTfm.Find("Waiting_Nextgame_new/Viewport_round");
        Transform tfm = parent.Find("ImageBg");
        for (byte i = 1; i < m_nMaxTurn; i++)
            HighlightTurn(tfm.GetChild(i), i < m_nCurTurn);
        RectTransform targetTfm = tfm.GetChild(m_nCurTurn - 1) as RectTransform;
        RankEffectObj.transform.SetParent(targetTfm, false);
        RectTransform rtfm = parent.Find("rankingBg/Image_ranking") as RectTransform;
        rtfm.anchoredPosition = targetTfm.anchoredPosition;
        Image img = parent.Find("ImagelineBG/Imageline").GetComponent<Image>();
        img.fillAmount = targetTfm.anchoredPosition.x / img.rectTransform.rect.width;
    }

    void OnClickRebuy()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_RequestbuyEnterNextRound);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(ContestDataManager.Instance().currentContestID);
        NetWorkClient.GetInstance().SendMsg(msg);

        RebuyTfm.Find("Buttonbuy").GetComponent<Button>().interactable = false;
    }

    void CloseRebuyUI(bool bRebuy)
    {
        RebuyTfm.gameObject.SetActive(false);

        if (m_CoutdownCoroutin != null)
            StopCoroutine(m_CoutdownCoroutin);

        if (!bRebuy)
            StartCoroutine(ShowResult(0, true));
    }

    public void ShowBuyTicket(int cost, uint secondLeft)
    {
        RebuyTfm.gameObject.SetActive(true);

        //RebuyTfm.FindChild("Textnumber/Text").GetComponent<Text>().text = times + "/" + maxTimes;
        RebuyTfm.Find("Textprice/Text").GetComponent<Text>().text = cost.ToString();
        RebuyTfm.Find("Buttonbuy").GetComponent<Button>().interactable = true;

        StartCountDown(secondLeft, null, RebuyTfm.Find("Buttonbuy/Text").GetComponent<Text>(), null, "({0}s)", () => CloseRebuyUI(false));
    }

    void OnClickResultOK()
    {
        OnClickReturn();
    }

    void OnClickResultShare()
    {
        if (ResultTfm == null)
            return;

        GameObject sharepanel = ResultTfm.Find("Pop-up/image_share").gameObject;
        sharepanel.SetActive(true);

        GameObject closeshare = ResultTfm.Find("Pop-up/image_share/UiRootBG").gameObject;
        XPointEvent.AutoAddListener(closeshare, OnCloseShare, null);

        GameObject closesharBtn = ResultTfm.Find("Pop-up/image_share/ImageBG/ButtonClose").gameObject;
        XPointEvent.AutoAddListener(closesharBtn, OnCloseShare, null);

        GameObject share1btn = ResultTfm.Find("Pop-up/image_share/ImageBG/Buttonshare_1").gameObject;
        //XPointEvent.AutoAddListener(share1btn, OnShare2Moments, sharepanel.transform.FindChild("ImageBG"));
        share1btn.SetActive(false);

        GameObject share2btn = ResultTfm.Find("Pop-up/image_share/ImageBG/Buttonshare_2").gameObject;
        //XPointEvent.AutoAddListener(share2btn, OnShare2Friends, sharepanel.transform.FindChild("ImageBG"));
        share2btn.SetActive(false);

        Text timeTx = ResultTfm.Find("Pop-up/image_share/ImageBG/Text_riqi").gameObject.GetComponent<Text>();
        timeTx.text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");
        Image head = ResultTfm.Find("Pop-up/image_share/ImageBG/Image_Head/Image_HeadMask/Image_HeadImage").gameObject.GetComponent<Image>();
        head.sprite = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(), GameMain.hall_.GetPlayerId(), (int)GameMain.hall_.GetPlayerData().PlayerIconId);
        Text name = ResultTfm.Find("Pop-up/image_share/ImageBG/Image_Head/Image_NameBG/TextName").gameObject.GetComponent<Text>();
        name.text = GameMain.hall_.GetPlayerData().GetPlayerName();

    }

    private void OnShare2Moments(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            Transform shareimg = (Transform)button;
            //Player.ShareImageToWechat();
            Player.ShareImageRectToWechat(GameObject.Find("Canvas").gameObject.GetComponent<Canvas>(),
                 shareimg.gameObject.GetComponent<RectTransform>());

            //GameObject sharepanel = ByStanderObj.transform.FindChild("Pop-up/image_share").gameObject;
            //sharepanel.SetActive(false);
        }
    }

    private void OnShare2Friends(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            Transform shareimg = (Transform)button;
            Player.ShareImageRectToWechat(GameObject.Find("Canvas").gameObject.GetComponent<Canvas>(),
                 shareimg.gameObject.GetComponent<RectTransform>(), false);

            //GameObject sharepanel = ByStanderObj.transform.FindChild("Pop-up/image_share").gameObject;
            //sharepanel.SetActive(false);
        }
    }

    private void OnCloseShare(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            GameObject sharepanel = ResultTfm.Find("Pop-up/image_share").gameObject;
            sharepanel.SetActive(false);
        }
    }

    IEnumerator ShowResult(ushort ranking, bool bOut)
    {
        Transform tfm;
        if (ranking == 0)
        {
            tfm = ProcessTfm.Find("Waiting_Nextgame_new/Viewport_round/rankingBg/Image_ranking/Text_ranking/TextNum");
            ranking = ushort.Parse(tfm.GetComponent<Text>().text);
        }

        if (!ProcessTfm.gameObject.activeSelf)
            CustomAudio.GetInstance().PlayCustomAudio(1001, true);

        ResetGui();

        ProcessTfm.gameObject.SetActive(false);
        ResultTfm.gameObject.SetActive(true);

        ResultTfm.Find("ImageBG_ItemTips/Text_tips").gameObject.SetActive(bOut);
        tfm = ResultTfm.Find("Text_ranking");
        tfm.gameObject.SetActive(!bOut);
        tfm.GetComponent<Text>().text = ranking.ToString();
        tfm = ResultTfm.Find("Text_ranking_lost");
        tfm.gameObject.SetActive(bOut);
        tfm.GetComponent<Text>().text = ranking.ToString();

        Text contestName = ResultTfm.Find("Pop-up/image_share/ImageBG/Text_bisaiming").gameObject.GetComponent<Text>();
        contestName.text = ContestDataManager.Instance().GetCurrentContestData().sContestName;
        Text sortTx = ResultTfm.Find("Pop-up/image_share/ImageBG/Text_paiming").gameObject.GetComponent<Text>();
        sortTx.text = ranking.ToString();
        Text rewardTx = ResultTfm.Find("Pop-up/image_share/ImageBG/Text_jiangpin").gameObject.GetComponent<Text>();
        rewardTx.text = ContestDataManager.Instance().GetCurrentContestData().reward;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        GameObject assetObj, obj;
        tfm = ResultTfm.Find("Animator");
        if (!bOut)//win
        {
            assetObj = (GameObject)bundle.LoadAsset("Contest_Result_sh");
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);

            assetObj = (GameObject)bundle.LoadAsset("Anime_match_win");
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
        }
        else
        {
            assetObj = (GameObject)bundle.LoadAsset("Anime_match_lost");
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
        }

        DragonBones.UnityArmatureComponent animate = obj.GetComponent<DragonBones.UnityArmatureComponent>(); ;
        animate.animation.Play("zhankai");        yield return new WaitUntil(() => animate.animation.isCompleted);        animate.animation.Play("idle");        yield return null;        tfm = ResultTfm.Find("ImageBG_ItemTips/ItemBG");
        foreach (Transform child in tfm)
            Destroy(child.gameObject);
        tfm.gameObject.SetActive(false);
        assetObj = (GameObject)bundle.LoadAsset("MatchResult_Item");
        stRankingRewardData rrd = ContestDataManager.Instance().GetCurrentRewardData(ranking);
        if (rrd == null)
            yield break;

        bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallBagIconAssetBundleName);

        bool bReward = false;        if(rrd.fMasterReward > 0)//大师分
        {
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
            obj.transform.Find("ItemIcon").GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>("bs_icon_07");
            obj.transform.Find("Textnum").GetComponent<Text>().text = rrd.fMasterReward.ToString();
            bReward = true;
        }

        if (rrd.fRedbegReward > 0)//红包
        {
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
            obj.transform.Find("ItemIcon").GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>("bs_icon_03");
            obj.transform.Find("Textnum").GetComponent<Text>().text = rrd.fRedbegReward.ToString();
            bReward = true;
        }

        if (rrd.nRechargeCoinReward > 0)//钻石
        {
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
            obj.transform.Find("ItemIcon").GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>("bs_icon_02");
            obj.transform.Find("Textnum").GetComponent<Text>().text = rrd.nRechargeCoinReward.ToString();
            bReward = true;
        }

        if (rrd.nMoneyCommonReward > 0)//金币
        {
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
            obj.transform.Find("ItemIcon").GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>("bs_icon_01");
            obj.transform.Find("Textnum").GetComponent<Text>().text = rrd.nMoneyCommonReward.ToString();
            bReward = true;
        }

        if (rrd.nRewardItem1Id > 0)
        {
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
            string icon = BagDataManager.GetBagDataInstance().bagitemsdata_[rrd.nRewardItem1Id].itemIcon;
            obj.transform.Find("ItemIcon").GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>(icon);
            obj.transform.Find("Textnum").GetComponent<Text>().text = rrd.nRewardItem1Num.ToString();
            bReward = true;
        }

        if (rrd.nRewardItem2Id > 0)
        {
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
            string icon = BagDataManager.GetBagDataInstance().bagitemsdata_[rrd.nRewardItem2Id].itemIcon;
            obj.transform.Find("ItemIcon").GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>(icon);
            obj.transform.Find("Textnum").GetComponent<Text>().text = rrd.nRewardItem2Num.ToString();
            bReward = true;
        }

        if (rrd.nRewardItem3Id > 0)
        {
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
            string icon = BagDataManager.GetBagDataInstance().bagitemsdata_[rrd.nRewardItem3Id].itemIcon;
            obj.transform.Find("ItemIcon").GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>(icon);
            obj.transform.Find("Textnum").GetComponent<Text>().text = rrd.nRewardItem3Num.ToString();
            bReward = true;
        }

        tfm.gameObject.SetActive(bReward);
    }

    public bool HandleGameNetMsg(uint _msgType, UMessage _ms)
    {
        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.ContestMsg_PacketContestInfoToGameServer:
                {
                    DebugLog.Log("GameServer:promotion info begin");
                    uint contestId = _ms.ReadUInt();
                    ushort count = _ms.ReaduShort();
                    List<short> list = new List<short>();
                    for(int i = 0; i < count; i++)
                    {
                        short t = _ms.ReadShort();
                        list.Add(t);
                        DebugLog.Log(t);
                    }
                    InitNextGame(list);
                    DebugLog.Log("promotion info end-----------");
                }
                break;
            case GameCity.EMSG_ENUM.ContestMsg_CommandContestDisband:
            case GameCity.EMSG_ENUM.ContestMsg_AdmissionDisband_NotifyPlayer:
                {
                    uint contestId = _ms.ReadUInt();
                    DissolutionGameContest(contestId, eMsg == GameCity.EMSG_ENUM.ContestMsg_CommandContestDisband);
                }
                break;

            case GameCity.EMSG_ENUM.ContestMsg_ContestPlayerByePromotion://轮空晋级
                {
                    DebugLog.Log("LoginServer:promotion info begin");
                    uint contestId = _ms.ReadUInt();
                    ushort deskNum = _ms.ReaduShort();
                    uint leftSec = _ms.ReadUInt();
                    int inNum = _ms.ReadInt();
                    ushort count = _ms.ReaduShort();
                    List<short> list = new List<short>();
                    for (int i = 0; i < count; i++)
                    {
                        short t = _ms.ReadShort();
                        list.Add(t);
                        DebugLog.Log(t);
                    }
                    byte nSerIndex = _ms.ReadByte();
                    HallMain.SetRoomSerIndex(nSerIndex);
                    InitNextGame(list);
                    DebugLog.Log("promotion info end----------");
                    ShowNextGame(-1, leftSec, deskNum, deskNum);

                    GameMain.hall_.Connect2ContestCommonServer(ContestDataManager.Instance().GetCurrentContestData().byGameID,
                        0, contestId, 0, inNum, 1);
                }
                break;

            case GameCity.EMSG_ENUM.ContestMsg_RoundEndDeskRoleRank:
                {
                    uint contestId = _ms.ReadUInt();
                    ushort rank = _ms.ReaduShort();
                    uint leftSec = _ms.ReadUInt();
                    ushort playingDesk = _ms.ReaduShort();
                    ushort deskNum = _ms.ReaduShort();

                    ShowNextGame(rank, leftSec, playingDesk, deskNum);
                }
                break;

            case GameCity.EMSG_ENUM.ContestMsg_PlayerPromotionRank:
                {
                    uint contestId = _ms.ReadUInt();
                    uint PromotionCount = _ms.ReadUInt();
                    ushort rank = _ms.ReaduShort();
                    byte state = _ms.ReadByte();//1:晋级 2：淘汰
                    short rebuy = _ms.ReadShort();
                    float time = _ms.ReadSingle();
                    if (state == 1)
                        ShowPromotion(rank,false,(uint)time);
                    else if (rebuy > 0)
                    {
                        UpdateNextGame(rank, false, 0);
                        ShowBuyTicket(rebuy, 10);
                    }
                    else
                        StartCoroutine(ShowResult(rank, true));
                }
                break;

            case GameCity.EMSG_ENUM.ContestMsg_ContestScoreRank:
                {
                    uint contestId = _ms.ReadUInt();
                    ushort rank = _ms.ReaduShort();
                    StartCoroutine(ShowResult(rank, false));
                }
                break;

            case GameCity.EMSG_ENUM.ContestMsg_UpdatePlayerRankAfterOneOver:
                {
                    uint deskNum = _ms.ReadUInt();
                    ushort rank = _ms.ReaduShort();
                    UpdateNextGame(rank, false, deskNum);
                }
                break;

            case GameCity.EMSG_ENUM.ContestMsg_RequestbuyEnterNextRoundReply:
                {
                    //重购状态 1 成功 2 比赛不存在，3 重置时间已过
                    byte state = _ms.ReadByte();

                    if (state == 1)
                    {
                        CloseRebuyUI(true);
                        ShowPromotion();
                    }
                    else
                    {
                        DebugLog.Log("rebuy fail, errCode:" + state);
                        CCustomDialog.OpenCustomConfirmUI(1612);
                    }
                    RebuyTfm.Find("Buttonbuy").GetComponent<Button>().interactable = true;
                }
                break;

            case GameCity.EMSG_ENUM.ContestMsg_SeekSubstitutes:
                {
                    CloseRebuyUI(true);

                    uint contestId = _ms.ReadUInt();
                    CCustomDialog.OpenCustomDialogWithTimer(1026, 10, (param) =>
                    {
                        int isagree = (int)param;                        if(isagree > 0)
                        {
                            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_RequestSubstitutes);
                            msg.Add(GameMain.hall_.GetPlayerId());
                            msg.Add(contestId);
                            NetWorkClient.GetInstance().SendMsg(msg);
                        }
                    });
                }
                break;

            case GameCity.EMSG_ENUM.ContestMsg_RequestSubstitutesReply:
                {
//                     uint userId = _ms.ReadUInt();
//                     uint contestId = _ms.ReadUInt();
                    //替补状态 1 成功 2 比赛不存在，3名额已满 4 替补时间已过
                    byte state = _ms.ReadByte();
                    if (state != 1)
                    {
                        uint id;
                        if (state == 3)
                            id = 1614;
                        else if (state == 4)
                            id = 1615;
                        else
                            id = 1616;
                        CCustomDialog.OpenCustomConfirmUI(id, (param) =>
                        {
                            StartCoroutine(ShowResult(0, true));
                        });
                    }
                    else
                        ShowPromotion();
                }
                break;

            case GameCity.EMSG_ENUM.ContestMsg_RoundTimeOverForceEndGameingDesk:
                {
                    CCustomDialog.OpenCustomConfirmUI(1600);

                    if (GameMain.hall_.GameBaseObj != null)
                        GameMain.hall_.GameBaseObj.OnDisconnect();
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_BACKTOBEONLOOKER:
                {
                    byte state = _ms.ReadByte();//1:游戏未开始 2:围观人数已满 3已在其他房间不能旁观
                    CCustomDialog.OpenCustomConfirmUI(1641);
                }
                break;
            case GameCity.EMSG_ENUM.Contestmsg_AnyTimeContestCurPlayerCount:
                {
                    uint contestId = _ms.ReadUInt();
                    ushort curNum = _ms.ReaduShort();
                    UpdateWaitRole(curNum);
                }
                break;

            default:
                break;
        }
        return true;
    }

    /// <summary>
    /// 解散游戏比赛
    /// </summary>
    /// <param name="contestID"></param>
    /// <param name="commandDissolutionState"></param>
    private void DissolutionGameContest(uint contestID,bool commandDissolutionState = false)
    {
        stContestData contestData = null;
        if (ContestDataManager.Instance().contestdatas_.ContainsKey(contestID))
        {
            contestData = ContestDataManager.Instance().contestdatas_[contestID];
        }
        else if (ContestDataManager.Instance().selfcontestdatas_.ContainsKey(contestID))
        {
            contestData = ContestDataManager.Instance().selfcontestdatas_[contestID];
        }

        if (contestData != null)
        {
            if (contestData.contestState >= CONTEST_STATE.CONTEST_STATE_ADMISSION)
            {
                if(commandDissolutionState)
                {
                    OnAlreadyEnd(1667);
                }
                else
                {
                    CCustomDialog.OpenCustomConfirmUI(1605, Giveup);
                }
            }
        }

        if (GameMain.hall_.GetPlayerData().signedContests.Contains(contestID))
            GameMain.hall_.GetPlayerData().signedContests.Remove(contestID);
    }

    public IEnumerator ShowRoundResult(int num, List<RoundResultData> resultDataList, UnityAction action, bool bOver, bool bBystander, string detail = "")
    {
        Transform tfm;
        if(RoundResultObj == null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            GameObject obj = (GameObject)bundle.LoadAsset("Match_resultRound_" + num);
            RoundResultObj = Instantiate(obj);
            RoundResultObj.transform.SetParent(ProcessTfm, false);
        }

        RoundResultObj.SetActive(true);
        tfm = RoundResultObj.transform.Find("Button_Confirm");
        Button btn = tfm.GetComponent<Button>();
        btn.gameObject.SetActive(bOver);
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            RoundResultObj.SetActive(false);

            if (bBystander && bOver)
                OnClickExitBystander();
            else if (action != null)
                action.Invoke();
        });

        string str;
        for (int i = 0; i < num; i++)
        {
            tfm = RoundResultObj.transform.Find("player_" + (i + 1));
            if (tfm == null || resultDataList[i] == null)
                continue;

            tfm.Find("Image_HeadBG/Image_HeadMask/Image_HeadImage").
                GetComponent<Image>().sprite = resultDataList[i].headImg;
            tfm.Find("Image_add/TextName").GetComponent<Text>().text = resultDataList[i].name;
            tfm.Find("Image_add/Text_zongfen/TextNum").GetComponent<Text>().text = resultDataList[i].coin.ToString();
            str = resultDataList[i].addCoin.ToString();
            if (resultDataList[i].addCoin > 0)
            {
                str = "+" + str;
                tfm.Find("Image_add/Text_jifen").gameObject.SetActive(true);
                tfm.Find("Image_add/Text_jifen/TextNum").GetComponent<Text>().text = str;
                if(i == 0)
                    tfm.Find("Image_add/Image_bg").gameObject.SetActive(true);

                tfm.Find("Image_subtract").gameObject.SetActive(false);

            }
            else
            {
                tfm.Find("Image_add/Text_jifen").gameObject.SetActive(false);

                tfm.Find("Image_subtract").gameObject.SetActive(true);
                tfm.Find("Image_subtract/Text_jifen/TextNum").GetComponent<Text>().text = str;

                if(i == 0)
                {
                    tfm.Find("Image_subtract/TextName").GetComponent<Text>().text = resultDataList[i].name;
                    tfm.Find("Image_subtract/Text_zongfen/TextNum").GetComponent<Text>().text = resultDataList[i].coin.ToString();

                    tfm.Find("Image_add/Image_bg").gameObject.SetActive(false);
                }
            }
        }

        tfm = RoundResultObj.transform.Find("player_1/Text_mjfen");
        tfm.GetComponent<Text>().text = detail;

        m_nCurRound++;

        if (bBystander)
        {
            yield return new WaitForSecondsRealtime(1f);

            btn.onClick.Invoke();
        }
    }

    void OnClickTablesView()
    {
        if(m_bNeedUpdateDesk)
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_RequestDeskInfo);
            msg.Add(ContestDataManager.Instance().currentContestID);
            HallMain.SendMsgToRoomSer(msg);

            ProcessTfm.Find("Waiting_Nextgame_new/ButtonChakan").GetComponent<Button>().interactable = false;
            Invoke("GetDeskInfoFail", 5f);

            m_bNeedUpdateDesk = false;
        }
        else
        {
            TablesViewTfm.gameObject.SetActive(true);
        }
    }

    void GetDeskInfoFail()
    {
        ProcessTfm.Find("Waiting_Nextgame_new/ButtonChakan").GetComponent<Button>().interactable = true;
        m_bNeedUpdateDesk = true;
    }

    bool HandleDeskInfo(uint _msgType, UMessage _ms)
    {
        CancelInvoke("GetDeskInfoFail");
        ProcessTfm.Find("Waiting_Nextgame_new/ButtonChakan").GetComponent<Button>().interactable = true;

        TablesViewTfm.gameObject.SetActive(true);

        Transform tfm = TablesViewTfm.Find("ImageBG/Viewport/Content");
        foreach (Transform child in tfm)
            GameObject.Destroy(child.gameObject);

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        GameObject assetObj;
        GameObject obj;

        uint contestId = _ms.ReadUInt();
        ushort deskNum = _ms.ReaduShort();
        for(ushort i = 0; i < deskNum; i++)
        {
            uint desk = _ms.ReadUInt();
            byte state = _ms.ReadByte();//1:playing 2:over

            assetObj = (GameObject)bundle.LoadAsset("Matchprocess_jindu_" + state);
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);

            string deskStr = desk.ToString();
            deskStr = deskStr.Substring(5) + "号桌";
            obj.transform.Find("TextZhuohao").GetComponent<Text>().text = deskStr;
            obj.GetComponent<Button>().onClick.AddListener(() => OnClickDesk(desk, state));
        }

        return true;
    }

    void OnClickRankView()
    {
        if (m_bNeedUpdateRank)
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.ContestMsg_RequestGameingRank);
            msg.Add(ContestDataManager.Instance().currentContestID);
            HallMain.SendMsgToRoomSer(msg);

            ProcessTfm.Find("ImageLeftBG").GetComponent<Button>().interactable = false;
            Invoke("GetRankInfoFail", 5f);

            m_bNeedUpdateRank = false;
        }
        else
        {
            GameMain.hall_.contest.LoadContestInfoResource(ContestDataManager.Instance().GetCurrentContestData());
        }
    }

    void GetRankInfoFail()
    {
        ProcessTfm.Find("ImageLeftBG").GetComponent<Button>().interactable = true;
        m_bNeedUpdateRank = true;
    }

    bool HandleRankInfo(uint _msgType, UMessage _ms)
    {
        CancelInvoke("GetRankInfoFail");
        ProcessTfm.Find("ImageLeftBG").GetComponent<Button>().interactable = true;

        GameMain.hall_.contest.LoadContestInfoResource(ContestDataManager.Instance().GetCurrentContestData());

        Transform parent = GameMain.hall_.contest.matchinfo_.transform;
        if (parent == null)
            return false;

        Transform tfm = parent.Find("ImageBG/InfoBG/Image_saikuang/Viewport/Content");
        foreach(Transform child in tfm)
        {
            if (child.name == "none")
                continue;

            GameObject.Destroy(child.gameObject);
        }


        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        GameObject assetObj = (GameObject)bundle.LoadAsset("MatchInfo_saikuang");
        GameObject obj;

        bool isAnswerType = false;
        if(GameMain.hall_.GameBaseObj != null)
        {
            isAnswerType = GameMain.hall_.GameBaseObj.GetGameType() == GameKind_Enum.GameKind_Answer;
        }
        Text GameTimeText = null;
        DateTime GameTime = DateTime.Now;
        uint contestId = _ms.ReadUInt();
        int selfScore = _ms.ReadInt();
        byte num = _ms.ReadByte();
        float answerTime = 0.0f;
        for (byte i = 0; i < num; i++)
        {
            byte rank = _ms.ReadByte();
            uint userid = _ms.ReadUInt();
            int score = _ms.ReadInt();

            string name = _ms.ReadString();
            string url = _ms.ReadString();

            answerTime = 0.0f;
            
            obj = Instantiate(assetObj);
            obj.transform.SetParent(tfm, false);
            GameTimeText = obj.transform.Find("Texttime").GetComponent<Text>();
            if (isAnswerType)
            {
                answerTime = _ms.ReadSingle();
                GameTime = GameCommon.ConvertLongToDateTime(answerTime);
                GameTimeText.text = GameTime.Minute + ":" + GameTime.Second + "."+ GameTime.Millisecond;
            }
            GameTimeText.gameObject.SetActive(isAnswerType);
            obj.transform.Find("TextRanking").GetComponent<Text>().text = rank.ToString();
            obj.transform.Find("TextName").GetComponent<Text>().text = name;
            obj.transform.Find("TextGrade").GetComponent<Text>().text = score.ToString();

            Sprite sp = GameMain.hall_.GetIcon(url, userid);
            obj.transform.Find("Image_HeadBG/Image_HeadMask/Image_HeadImage").GetComponent<Image>().sprite = sp;
        }

        PlayerData pd = GameMain.hall_.GetPlayerData();
        tfm = parent.Find("ImageBG/InfoBG/Image_saikuang/MatchInfo_saikuang_ziji");
        string str = ProcessTfm.Find("ImageLeftBG/Text_mingci/Text_num").GetComponent<Text>().text;
        int index = str.IndexOf('/');
        str = str.Substring(0, index);
        tfm.Find("TextRanking").GetComponent<Text>().text = str;
        tfm.Find("TextName").GetComponent<Text>().text = pd.GetPlayerName();
        tfm.Find("TextGrade").GetComponent<Text>().text = selfScore.ToString();
        Image img = tfm.Find("Image_HeadBG/Image_HeadMask/Image_HeadImage").GetComponent<Image>();        img.sprite = GameMain.hall_.GetIcon(pd.GetPlayerIconURL(), GameMain.hall_.GetPlayerId(), (int)pd.PlayerIconId);
        return true;
    }

    public void OnAlreadyEnd(uint tipsID = 1627)
    {
        CCustomDialog.OpenCustomConfirmUI(tipsID, (param) =>
        {
            OnTotalEnd();        });
    }

    bool HandleMiddleEnterContest(uint _msgType, UMessage _ms)
    {
        ContestPlayerState_enum state = (ContestPlayerState_enum)_ms.ReadByte();
        DebugLog.Log("Middle enter contest--State:" + state);
        if (state == ContestPlayerState_enum.ContestPlayerState_end)
        {
            OnAlreadyEnd();
            return true;
        }
        float leftTime = _ms.ReadSingle();
        ushort rank = _ms.ReaduShort();
        byte CurTurn = _ms.ReadByte();
        byte MaxTurn = _ms.ReadByte();
        byte CurRound = _ms.ReadByte();
        byte RoundPerTurn = _ms.ReadByte();
        ushort deskNum = _ms.ReaduShort();
        ushort playingDesk = _ms.ReaduShort();
        uint promotion = _ms.ReadUInt();
        ushort roleNum = _ms.ReaduShort();

        DebugLog.Log("Middle enter contest turn:"+ m_nCurTurn + " round:"+m_nCurRound + " rank:" + rank);

        ushort count = _ms.ReaduShort();
        List<short> list = new List<short>();
        for (int i = 0; i < count; i++)
        {
            short t = _ms.ReadShort();
            list.Add(t);
        }

        SetContestData(RoundPerTurn, roleNum);
        InitNextGame(list);
        m_nCurTurn = CurTurn;
        m_nMaxTurn = MaxTurn;
        m_nCurRound = CurRound;
        m_nRoundPerTurn = RoundPerTurn;
        UpdateCurTurnUI();
        ShowBegin();

        switch (state)
        {
            case ContestPlayerState_enum.ContestPlayerState_Gameing:
                break;
            case ContestPlayerState_enum.ContestPlayerState_UpGrade:
                ShowPromotion(rank,true);
                break;
            case ContestPlayerState_enum.ContestPlayerState_Out:
                StartCoroutine(ShowResult(rank, true));
                break;
            case ContestPlayerState_enum.ContestPlayerState_WaitResult:
                ShowNextGame(rank, (uint)leftTime, playingDesk, deskNum);
                break;
            case ContestPlayerState_enum.ContestPlayerState_Bye:
                ShowNextGame(-1, (uint)leftTime, deskNum, deskNum);
                break;

            default:
                break;
        }

        return true;
    }

    void OnClickDesk(uint desk, byte state)
    {
        if(state == 1)
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_APPLYTOBEONLOOKER);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(ContestDataManager.Instance().currentContestID);
            msg.Add(desk);
            HallMain.SendMsgToRoomSer(msg);
        }
    }

    void OnClickExitBystander()
    {
        if (GameMain.hall_.GameBaseObj != null)
            GameMain.hall_.GameBaseObj.OnDisconnect();

        string time = ProcessTfm.Find("Waiting_Nextgame_new/Imagebottom/TextTime").GetComponent<Text>().text;
        int index = time.IndexOf(':');
        string min = time.Substring(0, index);
        string sec = time.Remove(0, index + 1);
        uint left = uint.Parse(min) * 60 + uint.Parse(sec);

        ShowNextGame(0, left, 0, 0, true);
    }
}
