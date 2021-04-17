using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USocket.Messages;

//冠军排行数据
public class ChampionRankData
{
    public uint playerid;
    public long time;
    public string playerName;
    public string url;
    public uint contestid;
    public string contestName;
    public string contestReward;
    public int faceid;
}

//大师排行数据
public class MasterRankData
{
    public uint playerid;
    public string playerName;
    public int faceid;
    public string url;
    public float master;
}

public class MasterRank
{
    static MasterRank instance_;

    GameObject root_;

    //斗地主大师排行榜
    List<MasterRankData> llrMasterList;
    //掼蛋大师排行榜
    List<MasterRankData> terMasterList;
    //冠军排行榜
    List<ChampionRankData> championList;

    IEnumerator CurRanIEnumerator;
    //当前排行榜数据游戏ID
    byte currentgameid;
    bool isask4championdata;
    bool ismaster;
    bool rankDataPromptUpdateState;
    short currentChampionBeginIndex;

    MasterRank()
    {
        llrMasterList = new List<MasterRankData>();
        championList = new List<ChampionRankData>();
        terMasterList = new List<MasterRankData>();

        CurRanIEnumerator = null;
        ismaster = true;
        isask4championdata = false;
        currentChampionBeginIndex = 0;
        rankDataPromptUpdateState = false;
        currentgameid = (byte)GameKind_Enum.GameKind_LandLords;
        InitMsgEvents();
    }

    public static MasterRank GetMasterRankInstance()
    {
        if (instance_ == null)
            instance_ = new MasterRank();

        return instance_;
    }

    void InitMsgEvents()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_TELLMASTERRANKNEEDUPDATE, BackTellRankInfo);                      //通知申请排名信息

        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_TELLCONTESTCHAMPIONNEEDUPDATE, BackTellChampionInfo);             //通知申请冠军信息

        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_UPDATEMASTERRANKTOLOGIN, BackRankInfo);                           //排名数据

        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SENDCONTESTCHAMPIONINFO, BackChampionInfo);                       //冠军数据
    }

    bool BackTellChampionInfo(uint msgType, UMessage msg)
    {
        byte isadd = msg.ReadByte();

        if (isadd == 1)
            isask4championdata = true;
        else
            championList.Clear();

        return true;
    }

    bool BackTellRankInfo(uint msgType, UMessage msg)
    {
        GameKind_Enum gameid = (GameKind_Enum)msg.ReadByte();
        switch (gameid)
        {
            case GameKind_Enum.GameKind_LandLords:
                terMasterList.Clear();
                break;
            case GameKind_Enum.GameKind_GuanDan:
                llrMasterList.Clear();
                break;
        }
        return true;
    }

    bool BackChampionInfo(uint msgType, UMessage msg)
    {
        short iscontinue = msg.ReadShort();
        short length = msg.ReadShort();

        for (int index = 0; index < length; index++)
        {
            ChampionRankData crd = new ChampionRankData();

            crd.time = msg.ReadUInt();
            crd.contestName = msg.ReadString();
            crd.playerid = msg.ReadUInt();
            crd.faceid = msg.ReadInt();
            crd.playerName = msg.ReadString();
            crd.contestid = msg.ReadUInt();

            championList.Add(crd);
        }

        if (iscontinue >= 0)
        {
            currentChampionBeginIndex = (short)(iscontinue + length);
        }

        RankDataPrompt(0 == championList.Count);
        return true;
    }

    bool BackRankInfo(uint msgType, UMessage msg)
    {
        byte gameid = msg.ReadByte();

        short iscontinue = msg.ReadShort();
        short length = msg.ReadShort();
        short nextContinue = (short)(iscontinue + length);

        for (int index = 0; index < length; index++)
        {
            MasterRankData mr = new MasterRankData();
            mr.playerid = msg.ReadUInt();
            mr.faceid = msg.ReadInt();
            mr.url = msg.ReadString();
            mr.master = msg.ReadSingle();
            mr.playerName = msg.ReadString();

            if (gameid == (byte)GameKind_Enum.GameKind_LandLords)
            {
                llrMasterList.Add(mr);
            }
            else if(gameid == (byte)GameKind_Enum.GameKind_GuanDan)
            {
                terMasterList.Add(mr);
            }
        }

        if (iscontinue >= 0)
        {
            Ask4RankInfo(nextContinue);
        }

        if (gameid == (byte)GameKind_Enum.GameKind_LandLords)
        {
            RankDataPrompt(0 == llrMasterList.Count);
        }
        else if (gameid == (byte)GameKind_Enum.GameKind_GuanDan)
        {
            RankDataPrompt(0 == terMasterList.Count);
        }
       
        return true;
    }

    /// <summary>
    /// 清除当前排行榜所有子界面
    /// </summary>
    private void RemoveContentTournametChilds()
    {
        if (null == root_)
        {
            return;
        }

        GameObject crbg = root_.transform.Find("Right/Viewport_tournament/Content_tournament").gameObject;

        GameMain.hall_.ClearChilds(crbg);
    }

    /// <summary>
    /// 请求大师分排行榜数据消息
    /// </summary>
    /// <param name="beginindex">排行榜等级</param>
    public void Ask4RankInfo(short beginindex = 0)
    {
        rankDataPromptUpdateState = true;
        GameKind_Enum currentGameID = (GameKind_Enum)currentgameid;
        if (0 == beginindex)
        {
            if (currentGameID == GameKind_Enum.GameKind_LandLords)
            {
                if (llrMasterList.Count > 0)
                {
                    RankDataPrompt(false);
                    return;
                }
            }
            else if (currentGameID == GameKind_Enum.GameKind_GuanDan)
            {
                if (terMasterList.Count > 0)
                {
                    RankDataPrompt(false);
                    return;
                }
            }
        }

        UMessage ask4RankDataMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_APPLYMASTERRANKTOLOGIN);
        ask4RankDataMsg.Add(currentgameid);
        ask4RankDataMsg.Add(beginindex);
        NetWorkClient.GetInstance().SendMsg(ask4RankDataMsg);
    }

    /// <summary>
    /// 刷新自己排行数据
    /// </summary>
    /// <param name="RankIndex">排行榜等级</param>
    void RefreshSelfRank(int RankIndex = -1)
    {
        if (null == root_)
            return;

        GameObject selfrank = root_.transform.Find("Right/ranking_dashi_ziji").gameObject;
        if (selfrank.activeSelf)
        {
            return;
        }
        selfrank.SetActive(ismaster);

        Text rank = selfrank.transform.Find("Textpaiming").gameObject.GetComponent<Text>();
        if (-1 == RankIndex)
            rank.text = "榜外";
        else
            rank.text = RankIndex.ToString();

        Text playerName = selfrank.transform.Find("playerinfo/TextName").gameObject.GetComponent<Text>();
        playerName.text = GameMain.hall_.GetPlayerData().GetPlayerName();

        Image playericon = selfrank.transform.Find("playerinfo/Image_HeadBG/Image_HeadMask/Image_HeadImage").gameObject.GetComponent<Image>();
        playericon.sprite = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(), GameMain.hall_.GetPlayerId());

        Text masterscore = selfrank.transform.Find("Image_dashi/Text_fen").gameObject.GetComponent<Text>();
        masterscore.text = GameMain.hall_.GetPlayerData().MasterScoreKindArray[currentgameid].ToString();

        Text masterstep = selfrank.transform.Find("Image_dashi/Text_num").gameObject.GetComponent<Text>();
        masterstep.text = CCsvDataManager.Instance.GameDataMgr.GetMasterLv(GameMain.hall_.GetPlayerData().MasterScoreKindArray[currentgameid]);
    }

    /// <summary>
    /// 加载大师分排行主界面
    /// </summary>
    void LoadMasterRankResource()
    {
        if (root_ != null)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        currentgameid = (byte)GameKind_Enum.GameKind_LandLords;
        ismaster = true;
        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Lobby_ranking_new");
        root_ = (GameObject)GameMain.instantiate(obj0);
        root_.transform.SetParent(GameObject.Find("Canvas/Root").transform, false);

        root_.SetActive(false);

        GameObject returnbtn = root_.transform.Find("Top/ButtonReturn").gameObject;
        XPointEvent.AutoAddListener(returnbtn, OnCloseMasterRankPanel, null);

        GameObject championbtn = root_.transform.Find("Top/Change_BG/Button_guanjun").gameObject;
        XPointEvent.AutoAddListener(championbtn, OnClickChampionBtn, null);

        GameObject masterbtn = root_.transform.Find("Top/Change_BG/Button_dashi").gameObject;
        XPointEvent.AutoAddListener(masterbtn, OnClickMasterBtn, null);

        Dropdown switchGameID = root_.transform.Find("Top/ChooseGame_BG/Dropdown").gameObject.GetComponent<Dropdown>();
        switchGameID.onValueChanged.AddListener(delegate (int call)
                                                {
                                                    currentgameid = call == 0 ? (byte)7 : (byte)13;
                                                    Ask4RankInfo();
                                                    GameMain.ST(CurRanIEnumerator);
                                                    CurRanIEnumerator = RefreshMasterRankPanel();
                                                    GameMain.SC(CurRanIEnumerator);
                                                });

        ScrollRect rectbg = root_.transform.Find("Right/Viewport_tournament").gameObject.GetComponent<ScrollRect>();
        rectbg.onValueChanged.AddListener(delegate (Vector2 call)
                                         {
                                             if (ismaster)
                                                 return;
                                             if (call.y > 100)
                                                 Ask4ChampionData();
                                         });
    }

    /// <summary>
    /// 查看冠军排行榜按钮事件
    /// </summary>
    private void OnClickChampionBtn(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (root_ == null)
                return;

            ismaster = false;
            root_.transform.Find("Top/Change_BG/Button_guanjun").gameObject.SetActive(false);
            root_.transform.Find("Top/Change_BG/Button_dashi").gameObject.SetActive(true);
            root_.transform.Find("Top/ChooseGame_BG/Dropdown").gameObject.SetActive(false);

            Ask4ChampionData();
            GameMain.ST(CurRanIEnumerator);
            CurRanIEnumerator = RefreshMasterRankPanel();
            GameMain.SC(CurRanIEnumerator);
            RemoveContentTournametChilds();
            root_.transform.Find("Top/ImageIcon/Text").gameObject.GetComponent<Text>().text = "冠军排行";
        }
    }

    /// <summary>
    /// 查看大师排行榜按键事件
    /// </summary>
    private void OnClickMasterBtn(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if (root_ == null)
                return;

            ismaster = true;
            root_.transform.Find("Top/Change_BG/Button_guanjun").gameObject.SetActive(true);
            root_.transform.Find("Top/Change_BG/Button_dashi").gameObject.SetActive(false);
            root_.transform.Find("Top/ChooseGame_BG/Dropdown").gameObject.SetActive(true);

            Ask4RankInfo();

            GameMain.ST(CurRanIEnumerator);
            CurRanIEnumerator = RefreshMasterRankPanel();
            GameMain.SC(CurRanIEnumerator);
            RemoveContentTournametChilds();
            root_.transform.Find("Top/ImageIcon/Text").gameObject.GetComponent<Text>().text = "大师排行";
        }
    }

    /// <summary>
    /// 加载冠军排行榜界面
    /// </summary>
    /// <param name="championData">冠军排行榜数据</param>
    void LoadChampionItemResource(ChampionRankData championData)
    {
        if (root_ == null || null == championData)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        GameObject crbg = root_.transform.Find("Right/Viewport_tournament/Content_tournament").gameObject;

        UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("ranking_guanjun");
        GameObject championitem = (GameObject)GameMain.instantiate(obj0);

        Text rank = championitem.transform.Find("Texttime").gameObject.GetComponent<Text>();
        System.DateTime sdt = GameCommon.ConvertLongToDateTime(championData.time);
        rank.text = sdt.ToString("yyyy年MM月dd日HH:mm");

        Text contestName = championitem.transform.Find("Textname").gameObject.GetComponent<Text>();
        contestName.text = ContestDataManager.Instance().contestData[championData.contestid].sContestName;

        Text playerName = championitem.transform.Find("playerinfo/TextName").gameObject.GetComponent<Text>();
        playerName.text = championData.playerName;

        Image playericon = championitem.transform.Find("playerinfo/Image_HeadBG/Image_HeadMask/Image_HeadImage").gameObject.GetComponent<Image>();
        playericon.sprite = GameMain.hall_.GetIcon(championData.url, championData.playerid);

        Text reward = championitem.transform.Find("Textprize").gameObject.GetComponent<Text>();
        reward.text = ContestDataManager.Instance().contestData[championData.contestid].reward;

        championitem.transform.SetParent(crbg.transform, false);
    }

    /// <summary>
    /// 添加排行榜数据
    /// </summary>
    /// <param name="Index">排行榜等级</param>
    /// <param name="masterRankData">排行榜数据</param>
    /// <returns>false : 失败 true :成功</returns>
    bool LoadMasterItemResource(int Index, MasterRankData masterRankData)
    {
        if (root_ == null || null == masterRankData)
            return false;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return false;

        GameObject crbg = root_.transform.Find("Right/Viewport_tournament/Content_tournament").gameObject;

        Transform ChildeTransform = null;
        if (crbg.transform.childCount <= Index)
        {
            UnityEngine.Object obj0 = bundle.LoadAsset("ranking_dashi");
            GameObject masteritem = (GameObject)GameMain.instantiate(obj0);
            ChildeTransform = masteritem.transform;
            ChildeTransform.SetParent(crbg.transform, false);
        }
        else
        {
            ChildeTransform = crbg.transform.GetChild(Index);
            if (!ChildeTransform.gameObject.activeSelf)
            {
                ChildeTransform.gameObject.SetActive(true);
            }
        }

        if (!ChildeTransform)
        {
            return false;
        }
        
        if(masterRankData.playerid == GameMain.hall_.GetPlayerData().GetPlayerID())
        {
            RefreshSelfRank(Index +1);
        }

        Text rank = ChildeTransform.Find("Textpaiming").gameObject.GetComponent<Text>();
        rank.text = (Index + 1).ToString();

        Text playerName = ChildeTransform.Find("playerinfo/TextName").gameObject.GetComponent<Text>();
        playerName.text = masterRankData.playerName;

        Image playericon = ChildeTransform.Find("playerinfo/Image_HeadBG/Image_HeadMask/Image_HeadImage").gameObject.GetComponent<Image>();
        playericon.sprite = GameMain.hall_.GetIcon(masterRankData.url, masterRankData.playerid);

        Text masterscore = ChildeTransform.Find("Image_dashi/Text_fen").gameObject.GetComponent<Text>();
        masterscore.text = masterRankData.master.ToString();

        Text masterstep = ChildeTransform.Find("Image_dashi/Text_num").gameObject.GetComponent<Text>();
        masterstep.text = CCsvDataManager.Instance.GameDataMgr.GetMasterLv(masterRankData.master);
        return true;
    }

    /// <summary>
    /// 请求冠军排行榜数据
    /// </summary>
    void Ask4ChampionData()
    {
        rankDataPromptUpdateState = true;
        if(championList.Count > 0 && !isask4championdata)
        {
            RankDataPrompt(false);
            return;
        }

        isask4championdata = false;
        UMessage ask4ChampionDataMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_APPLYCONTESTCHAMPIONINFO);

        ask4ChampionDataMsg.Add(currentChampionBeginIndex);

        NetWorkClient.GetInstance().SendMsg(ask4ChampionDataMsg);
    }

    /// <summary>
    /// 排行榜数据为空显示界面
    /// </summary>
    /// <param name="activeState"> 提示界面是否打开</param>
    void RankDataPrompt(bool activeState)
    {
        if (!rankDataPromptUpdateState || root_ == null)
            return;

        //暂无数据
        GameObject imagekongObj = root_.transform.Find("Right/Viewport_tournament/Imagekong").gameObject;
        if (imagekongObj && imagekongObj.activeSelf != activeState)
        {
            imagekongObj.SetActive(activeState);
        }
        rankDataPromptUpdateState = false;
    }

    /// <summary>
    /// 关闭排行榜
    /// </summary>
    private void OnCloseMasterRankPanel(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            root_.SetActive(false);
        }
    }

    /// <summary>
    /// 打开排行榜
    /// </summary>
    public void ShowMasterRankPanel()
    {
        LoadMasterRankResource();
        if(ismaster)
        {
            Ask4RankInfo();
        }
        else
        {
            Ask4ChampionData();
        }
        GameMain.ST(CurRanIEnumerator);
        CurRanIEnumerator = RefreshMasterRankPanel();
        GameMain.SC(CurRanIEnumerator);
        root_.SetActive(true);
    }

    /// <summary>
    /// 重置排行榜界面
    /// </summary>
    private void ResetRankPanel()
    {
        if ( null == root_)
        {
            return;
        }

        GameObject crbg = root_.transform.Find("Right/Viewport_tournament/Content_tournament").gameObject;
        for (int childIndex = 0; childIndex < crbg.transform.childCount; ++childIndex)
        {
            crbg.transform.GetChild(childIndex).gameObject.SetActive(false);
        }

        GameObject selfrank = root_.transform.Find("Right/ranking_dashi_ziji").gameObject;
        if (selfrank.activeSelf)
        {
            selfrank.SetActive(false);
        }
    }

    /// <summary>
    /// 刷新排行榜数据
    /// </summary>
    /// <returns></returns>
    IEnumerator RefreshMasterRankPanel()
    {
        ResetRankPanel();
        if (ismaster)
        {
            GameKind_Enum CurGameKind = (GameKind_Enum)currentgameid;
            if (CurGameKind == GameKind_Enum.GameKind_LandLords)
            {
                yield return new WaitUntil(() => llrMasterList.Count > 0);
                for (int index = 0; index < llrMasterList.Count;)
                {
                    if (!ismaster)
                    {
                        yield break;
                    }
                    if (LoadMasterItemResource(index,llrMasterList[index]))
                    {
                        index++;
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                }
                RefreshSelfRank();
            }
            else if (CurGameKind == GameKind_Enum.GameKind_GuanDan)
            {
                yield return new WaitUntil(() => terMasterList.Count > 0);
                for (int index = 0; index < terMasterList.Count;)
                {
                    if (!ismaster)
                    {
                        yield break;
                    }
                    if (LoadMasterItemResource(index,terMasterList[index]))
                    {
                        index++;
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                }
                RefreshSelfRank();
            }
        }
        else
        {
            yield return new WaitUntil(() => championList.Count > 0);
            for(int index = 0; index < championList.Count; ++ index)
            {
                if (ismaster)
                {
                    yield break;
                }
                LoadChampionItemResource(championList[index]);
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
        yield break;
    }
}
