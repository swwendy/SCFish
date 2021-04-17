using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using USocket.Messages;
using XLua;
[Hotfix]public class RoomDeskInfo
{
    public Transform deskTfm = null;
    public bool m_bInited = false;

    public void ShowPlayer(byte sit, bool show, string name = "", string url = "", uint playerid = 0, int faceId = 0)
    {
        if (!deskTfm)
            return;

        Transform tfm = deskTfm.Find("Image_player_" + (sit + 1));
        if (tfm == null)
            return;

        if(show)
        {
            tfm.Find("Button_seat").gameObject.SetActive(false);
            tfm = tfm.Find("Image_Head");
            tfm.gameObject.SetActive(true);
            Sprite sp = GameMain.hall_.GetIcon(url, playerid, faceId);
            tfm.Find("Image_HeadMask/Image_HeadImage").GetComponent<Image>().sprite = sp;
            tfm.Find("Image_NameBG/TextName").GetComponent<Text>().text = name;
        }
        else
        {
            tfm.Find("Button_seat").gameObject.SetActive(true);
            tfm.Find("Image_Head").gameObject.SetActive(false);
        }
    }

    public void Show(bool show)
    {
        if (deskTfm == null)
            return;
        deskTfm.gameObject.SetActive(show);
    }
}

[Hotfix]public class ChairInfo
{
    public Transform chairTfm;

    public void Show(bool show, string name = "", string url = "", uint playerid = 0, int faceId = 0)
    {
        if (chairTfm == null)
            return;

        if (show)
        {
            chairTfm.Find("Button_sit").gameObject.SetActive(false);
            Transform tfm = chairTfm.Find("Image_Head");
            tfm.gameObject.SetActive(true);
            Sprite sp = GameMain.hall_.GetIcon(url, playerid, faceId);
            tfm.Find("Image_HeadMask/Image_HeadImage").GetComponent<Image>().sprite = sp;
            tfm.Find("Image_NameBG/TextName").GetComponent<Text>().text = name;
        }
        else
        {
            chairTfm.Find("Button_sit").gameObject.SetActive(true);
            chairTfm.Find("Image_Head").gameObject.SetActive(false);
            SetReady(false);
            ShowOffline(false);
        }
    }

    public void SetReady(bool ready)
    {
        if(chairTfm != null)
            chairTfm.Find("already").gameObject.SetActive(ready);
    }

    public void ShowOffline(bool off)
    {
        if (chairTfm != null)
            chairTfm.Find("Text_offline").gameObject.SetActive(off);
    }
}

[Hotfix]public class MatchRoom : MonoBehaviour
{
    static MatchRoom m_Instance;

    Transform RoomTfm { get; set; }
    Transform TableTfm { get; set; }

    ScrollRect m_Scroll;

    Text m_TipText;
    float m_fTipTimer = 0f;

    bool m_bMoving = false;
    bool m_bUpdateDesk = false;
    bool m_bDeskComplete = false;
    byte m_nDeskNumPerRow = 0;
    float m_fDeskHRatio = 0f;
    float m_fScreenHRatio = 0f;
    float m_fOldScrollPos = 1f;
    ushort m_nDeskNum = 0;

    float minScrollSpeed = 500f;
    uint m_nBystanderRoom = 0;

    Dictionary<uint, RoomDeskInfo> m_dictIndexDesks = new Dictionary<uint, RoomDeskInfo>();
    List<ChairInfo> m_ChairList = new List<ChairInfo>();

    public static MatchRoom GetInstance()    {        if (m_Instance == null)            m_Instance = GameMain.Instance.gameObject.AddComponent<MatchRoom>();        m_Instance.LoadMatchingRoomResource();        return m_Instance;    }

    void Awake()    {        InitMsgHandle();    }    void InitMsgHandle()    {        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATEBEFOREHANDROOMINFO, HandleRoomInfo);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_APPLYENTERROOMANDSIT, HandleEnterDesk);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATEENTERROOMANDSIT, HandleDeskInfo);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATEENTERROOMANDSITTOREADYALL, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_APPLYLEAVEROOMANDSIT, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATELEAVEROOMANDSIT, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATELEAVEROOMANDSITTOREADYALL, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_APPLYREADY, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATEAPPLYREADY, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_BACKQUITSTARTGAME, HandleEnterDesk);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_KICKOUTROOM, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_UPDATERECHARGETOROOMSER, HandleGameNetMsg);    }
    void Start ()
    {
    }

    void InitDesks()
    {
        foreach(RoomDeskInfo di in m_dictIndexDesks.Values)
            di.m_bInited = false;
        m_bDeskComplete = false;
    }

    public void OnEnd(long diamond = 0)
    {
        if (RoomTfm == null)
            return;

        GameMain.hall_.GameBaseObj.OnDisconnect(false);

        ShowTable(false);
        m_bMoving = true;
        RoomTfm.gameObject.SetActive(true);
        if (diamond == 0)
            diamond = GameMain.hall_.GetPlayerData().GetDiamond();
        UpdateDiamond(diamond);

        CustomAudio.GetInstance().PlayCustomAudio(1001, true);

        CCustomDialog.CloseCustomWaitUI();
        m_nBystanderRoom = 0;
    }

    public void GameOver()
    {
        OnEnd();

        foreach (Transform child in m_Scroll.content)
            Destroy(child.gameObject);
        m_dictIndexDesks.Clear();
        m_ChairList.Clear();

        m_fTipTimer = 0f;
        m_bMoving = false;
        m_bUpdateDesk = false;
        m_nDeskNumPerRow = 0;
        m_fDeskHRatio = 0f;
        m_fScreenHRatio = 0f;
        m_fOldScrollPos = 1f;
        m_nDeskNum = 0;

        GameMain.hall_.SwitchToHallScene();
    }

    /// <summary>
    /// 加载匹配房间资源
    /// </summary>
    void LoadMatchingRoomResource()    {        if (GameMain.hall_.GameBaseObj == null || RoomTfm != null)
            return;

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        Transform canvasTfm = GameObject.Find("Canvas/Root").transform;
        if (canvasTfm == null)
            return;

        string PrefabName = CCsvDataManager.Instance.GameUIDataMgr.GetGameUIPrefabName("RoomTable", GameMain.hall_.GameBaseObj.GetGameType());
        GameObject obj = (GameObject)bundle.LoadAsset(PrefabName);
        obj = Instantiate(obj);
        obj.SetActive(false);
        RoomTfm = obj.transform;
        RoomTfm.SetParent(canvasTfm, false);

        m_Scroll = RoomTfm.Find("PanelGame_").GetComponent<ScrollRect>();
        m_Scroll.onValueChanged.AddListener(OnScrollValueChange);
        m_Scroll.verticalScrollbar.value = 1f;

        RoomTfm.Find("PanelHead_/Button_Return").GetComponent<Button>().
            onClick.AddListener(()=>OnClickReturn(3));
        RoomTfm.Find("PanelHead_/Button_matching").GetComponent<Button>().
            onClick.AddListener(OnClickQuickMatch);
        obj = RoomTfm.Find("PanelHead_/Image_DiamondFrame").gameObject;
        XPointEvent.AutoAddListener(obj, GameMain.hall_.Charge, Shop.SHOPTYPE.SHOPTYPE_DIAMOND);

        LoadMatchingTableResource(GameMain.hall_.GameBaseObj.GetGameType(), bundle);
    }

    /// <summary>
    /// 加载匹配桌子资源
    /// </summary>
    /// <param name="gameKind">游戏类型</param>    /// <param name="bundle">AB资源包</param>    public void LoadMatchingTableResource(GameKind_Enum gameKind, AssetBundle bundle)
    {
        if(bundle == null || TableTfm != null)
        {
            return;
        }

        Transform canvasTfm = GameObject.Find("Canvas/Root").transform;
        if (canvasTfm == null)
        {
            return;
        }

        byte playerNumPerDesk = 4;
        GameObject obj = null;
        string TableName  = CCsvDataManager.Instance.GameUIDataMgr.GetGameUIPrefabName("MatchReady", GameMain.hall_.GameBaseObj.GetGameType());
        switch (gameKind)
        {
            case GameKind_Enum.GameKind_GouJi:
                playerNumPerDesk = 6;
                TableName = "matching_table_Ready_6";
                break;
            case GameKind_Enum.GameKind_Chess:
                playerNumPerDesk = 2;
                break;
            case GameKind_Enum.GameKind_LandLords:
                playerNumPerDesk = 3;
                break;
        }

        obj = (GameObject)Instantiate(bundle.LoadAsset(TableName));
        obj.SetActive(false);
        TableTfm = obj.transform;
        TableTfm.SetParent(canvasTfm, false);

        Button btn = TableTfm.Find("bottom/Button_Invitation").GetComponent<Button>();
        btn.onClick.AddListener(OnClickDeskInvitate);
        btn = TableTfm.Find("bottom/Button_Ready").GetComponent<Button>();
        btn.onClick.AddListener(OnClickDeskReady);
        btn.gameObject.SetActive(false);
        btn = TableTfm.Find("Top/Button_Return").GetComponent<Button>();
        btn.onClick.AddListener(() => OnClickReturn(0));

        m_TipText = TableTfm.Find("Pop-up/ImagereadyTip/Text_time").GetComponent<Text>();

        UpdateMatchTablePlayerInfo(playerNumPerDesk);
    }    public void SetUIAsLast()
    {
        if(m_nBystanderRoom > 0)
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_APPLYTOBEONLOOKER);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add((uint)(GameKind_Enum.GameKind_CzMahjong));
            msg.Add(m_nBystanderRoom);
            HallMain.SendMsgToRoomSer(msg);
        }
        else
        {
            RoomTfm.SetAsLastSibling();
            if(TableTfm)
            {
                TableTfm.SetAsLastSibling();
                TableTfm.Find("bottom/Button_Ready").gameObject.SetActive(true);
            }
        }
    }
    void Update ()
    {
        if (RoomTfm == null)
            return;

        if(m_fTipTimer > 0f)
        {
            m_fTipTimer -= Time.deltaTime;
            if (m_fTipTimer > 0f)
                m_TipText.text = "(" + (int)m_fTipTimer + ")";
            else
                ShowKickTip(false);
        }

        if (RoomTfm.gameObject.activeSelf)
        {
            if (m_bUpdateDesk)
            {
                UpdateDeskData();
            }
            else if (m_bMoving)
            {
                if (m_Scroll.velocity.sqrMagnitude < minScrollSpeed)
                    OnScrollStop();
            }
        }
    }

    public void OnClickReturn(int index)//0:离开房间 1：继续 2:分享 3:退出
    {
        CustomAudio.GetInstance().PlayCustomAudio(1002);
        if (index == 0)
        {
            CCustomDialog.OpenCustomWaitUI("正在离开...");
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_APPLYLEAVEROOMANDSIT);
            msg.Add((byte)GameMain.hall_.GameBaseObj.GetGameType());
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);        }
        else if(index == 1)
        {
            GameMain.hall_.GameBaseObj.OnDisconnect(false);
            OnClickSit(0, -1);
        }
        else if(index == 3)
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_PLAYERLEAVEROOMSER);
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);        }
    }

    void OnClickQuickMatch()
    {
        CustomAudio.GetInstance().PlayCustomAudio(1002);
        CCustomDialog.OpenCustomWaitUI("正在进入...");
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_APPLYQUITSTARTGAME);
        msg.Add((byte)GameMain.hall_.GameBaseObj.GetGameType());
        msg.Add(GameMain.hall_.CurRoomIndex);
        msg.Add(GameMain.hall_.GetPlayerId());
        HallMain.SendMsgToRoomSer(msg);    }

    public void ShowTable(bool show, Dictionary<byte, AppointmentRecordPlayer> players = null, byte roomId = 0, uint deskId = 0)
    {
        if (TableTfm == null)
            return;

        InitDesks();
        Button btn = TableTfm.Find("bottom/Button_Ready").GetComponent<Button>();
        btn.interactable = true;
        TableTfm.gameObject.SetActive(show);
        foreach (ChairInfo ci in m_ChairList)
        {
            ci.Show(false);
        }

        if(show && players != null)
        {
            uint localUserId = GameMain.hall_.GetPlayerId();
            foreach(var v in players)
            {
                if (v.Key >= m_ChairList.Count)
                    continue;
                AppointmentRecordPlayer player = v.Value;
                m_ChairList[v.Key].Show(true, player.playerName, player.url, player.playerid, player.faceid);
                m_ChairList[v.Key].SetReady(player.ready == 1);
                if (player.playerid == localUserId)
                    btn.interactable = (player.ready == 0);
            }

            if(roomId != 0)
                TableTfm.Find("Top/ImageBG/Text_Room/Text_Num").GetComponent<Text>().text = roomId.ToString();
            if(deskId != 0)
                TableTfm.Find("Top/ImageBG/Text_table/Text_Num").GetComponent<Text>().text = deskId.ToString();

            CustomAudio.GetInstance().PlayCustomAudio(1001, true);
        }
    }

    bool HandleGameNetMsg(uint _msgType, UMessage _ms)
    {
        if (m_nDeskNum == 0)
            return false;
        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATEENTERROOMANDSITTOREADYALL:
                {
                    byte sit = _ms.ReadByte();
                    uint userId = _ms.ReadUInt();
                    int faceId = _ms.ReadInt();
                    string url = _ms.ReadString();
                    string name = _ms.ReadString();
                    ushort desk = _ms.ReaduShort();
                    m_dictIndexDesks[desk].ShowPlayer(sit, true, name, url, userId, faceId); 
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_APPLYLEAVEROOMANDSIT:
                {

                    sbyte state = _ms.ReadSByte();
                    if (state == 0)
                    {
                        long diamond = _ms.ReadLong();
                        OnEnd(diamond);
                    }
                    else
                    {
                        CCustomDialog.CloseCustomWaitUI();
                        CCustomDialog.OpenCustomConfirmUI(2206);
                    }

                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATELEAVEROOMANDSIT:
                {
                    byte sit = _ms.ReadByte();
                    uint userId = _ms.ReadUInt();
                    string name = _ms.ReadString();
                    if (sit < m_ChairList.Count)
                        m_ChairList[sit].Show(false);
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATELEAVEROOMANDSITTOREADYALL:
                {
                    byte sit = _ms.ReadByte();
                    uint deskId = _ms.ReadByte();
                    m_dictIndexDesks[deskId].ShowPlayer(sit, false);
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_APPLYREADY:
                {
                    byte state = _ms.ReadByte();
                    if(state == 0)
                    {
                        byte sit = _ms.ReadByte();
                        if (sit < m_ChairList.Count)
                            m_ChairList[sit].SetReady(true);
                        ShowKickTip(false);
                   }
                    else
                        TableTfm.Find("bottom/Button_Ready").GetComponent<Button>().interactable = true;
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_UPDATEAPPLYREADY:
                {
                    byte sit = _ms.ReadByte();
                    if (sit < m_ChairList.Count)
                        m_ChairList[sit].SetReady(true);
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_KICKOUTROOM:
                {
                    byte state = _ms.ReadByte();//1 长时间不点准备 2 不够入场钱  3 不够台费
                    uint tipsID = 2601;
                    switch (state)
                    {
                        case 1:
                            tipsID = 1647;
                            break;
                        case 2:
                            tipsID = 1652;
                            break;
                        case 3:
                            tipsID = 1664;
                            break;
                    }

                    CCustomDialog.CloseCustomWaitUI();
                    CCustomDialog.OpenCustomConfirmUI(tipsID, (obj) => { OnEnd(); });

                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_UPDATERECHARGETOROOMSER:
                {
                    uint userId = _ms.ReadUInt();
                    uint diamond = _ms.ReadUInt();
                    UpdateDiamond(diamond);
                }
                break;

            default:
                break;
        }

        return true;
    }

    bool HandleRoomInfo(uint _msgType, UMessage _ms)
    {
        if (m_nDeskNum == 0)
            return false;

        ushort size = _ms.ReaduShort();
        m_bDeskComplete = (_ms.ReadByte() == 1);
        HashSet<byte> playerSit = new HashSet<byte>();
        for(int i = 0; i < size; i++)
        {

            uint deskId = _ms.ReadUInt();
            RoomDeskInfo di = m_dictIndexDesks[deskId];
            di.m_bInited = true;

            playerSit.Clear();
            int playerNum = m_ChairList.Count;
            for (byte k = 0; k< playerNum; k++)
                playerSit.Add(k);

            byte num = _ms.ReadByte();
            for(int j = 0; j < num; j++)
            {
                byte sit = _ms.ReadByte();
                uint userId = _ms.ReadUInt();
                int faceId = _ms.ReadInt();
                string url = _ms.ReadString();
                string name = _ms.ReadString();
                di.ShowPlayer(sit, true, name, url, userId, faceId);
                playerSit.Remove(sit);            
            }

            foreach (byte sit in playerSit)
                di.ShowPlayer(sit, false);
        }

        return true;
    }

    void UpdateDiamond(long num)
    {
        RoomTfm.Find("PanelHead_/Image_DiamondFrame/Text_Diamond").GetComponent<Text>().text
            = num.ToString();
    }

    public void ShowRoom(byte gameId)
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)            return;

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(gameId);        if (gamedata == null)        {            Debug.Log("初始化游戏失败，id：" + gameId.ToString());            return;        }        RoomTfm.gameObject.SetActive(true);
        RoomTfm.Find("PanelHead_/Image_Icon").GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>(gamedata.GameTextIcon);
        UpdateDiamond(GameMain.hall_.GetPlayerData().GetDiamond());
    }

    /// <summary>
    /// 添加游戏桌子
    /// </summary>
    /// <param name="deskNum">最大桌子数量</param>
    /// <param name="playerNumPerDesk">当前游戏人数</param>
    /// <param name="deskMaxPlyerCount">桌子上可以承载的人数</param>
    public void AddDesk( ushort deskNum, byte playerNumPerDesk,byte deskMaxPlyerCount = 4)
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        string PrefabName = CCsvDataManager.Instance.GameUIDataMgr.GetGameUIPrefabName("MatchTable", GameMain.hall_.GameBaseObj.GetGameType());
        GameObject asset = (GameObject)bundle.LoadAsset<GameObject>(PrefabName + playerNumPerDesk);
        if (asset == null)
            return;

        foreach (Transform child in m_Scroll.content)
            Destroy(child.gameObject);
        m_dictIndexDesks.Clear();

        Transform tfm;
        RoomDeskInfo deskInfo;
        for(ushort i = 1; i <= deskNum; i++)
        {
            GameObject obj = Instantiate(asset);
            obj.transform.SetParent(m_Scroll.content, false);
            obj.name = i.ToString("d2");
            deskInfo = new RoomDeskInfo();
            deskInfo.deskTfm = obj.transform.Find("ImageBG");
            m_dictIndexDesks[i] = deskInfo;

            deskInfo.deskTfm.Find("Image_table/TextNum").GetComponent<Text>().text = obj.name;
            for (sbyte j = 0; j < playerNumPerDesk; j++)
            {
                tfm = deskInfo.deskTfm.Find("Image_player_" + (j + 1));
                ushort desk = i;
                sbyte sit = j;
                tfm.Find("Button_seat").GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickSit(desk, sit);
                });
            }
        }

        m_nDeskNum = deskNum;
        m_bUpdateDesk = true;
    }

    /// <summary>
    /// 更新游戏桌子玩家信息
    /// </summary>
    /// <param name="gamePlayerCount">当前游戏人数</param>
    /// <param name="deskMaxPlayerCount">桌子承载最大人数</param>
    void UpdateMatchTablePlayerInfo(byte gamePlayerCount,byte deskMaxPlayerCount = 4)
    {
        m_ChairList.Clear();
        ChairInfo ci;
        UnityEngine.Transform tfm = TableTfm.Find("table");
        for (byte i = 1; i <= deskMaxPlayerCount; i++)
        {
            Transform child = tfm.Find("Player_" + i);
            if (child == null)
                continue;

            if ((gamePlayerCount == 3 && i == 3)||
                (gamePlayerCount == 2 && (i == 2 || i == 4)))
            {
                child.gameObject.SetActive(false);
                continue;
            }

            child.gameObject.SetActive(true);

            ci = new ChairInfo();
            ci.chairTfm = child;
            m_ChairList.Add(ci);
        }
    }

    bool UpdateDeskData()
    {
        if (m_Scroll == null || m_Scroll.content.childCount == 0)
            return false;

        RectTransform deskTfm = m_Scroll.content.GetChild(0) as RectTransform;
        if (deskTfm.rect.width == 0)
            return false;

        m_nDeskNumPerRow = (byte)(m_Scroll.content.rect.width / deskTfm.rect.width);
        m_fDeskHRatio = deskTfm.rect.height / m_Scroll.content.rect.height;
        m_fScreenHRatio = m_Scroll.viewport.rect.height / m_Scroll.content.rect.height;
        DebugLog.Log("col:" + m_nDeskNumPerRow + "hr:" + m_fDeskHRatio);

        m_bMoving = true;
        m_bUpdateDesk = false;

        return true;
    }

    void OnScrollValueChange(Vector2 v)
    {
        CheckDeskInView(false);

        if (!m_bDeskComplete && m_fOldScrollPos != m_Scroll.verticalScrollbar.value && m_Scroll.velocity.sqrMagnitude > minScrollSpeed)
            m_bMoving = true;
        m_fOldScrollPos = m_Scroll.verticalScrollbar.value;
        //DebugLog.Log(m_fOldScrollPos+ "   " + m_Scroll.velocity.sqrMagnitude);
    }

    void OnScrollStop()
    {
        //DebugLog.Log("Stop!!!" + m_Scroll.verticalScrollbar.value + " " + m_Scroll.normalizedPosition.y);
        m_bMoving = false;

        CheckDeskInView(true);
    }

    void CheckDeskInView(bool send)
    {
        if (m_bUpdateDesk)
            return;

        float pos = (1f - m_Scroll.verticalScrollbar.value) * (1f - m_fScreenHRatio);
        float posEnd = pos + m_fScreenHRatio;
        uint begin = (uint)(pos / m_fDeskHRatio);
        begin = begin * m_nDeskNumPerRow + 1;
        uint end = (uint)(posEnd / m_fDeskHRatio);
        end = (end + 1) * m_nDeskNumPerRow;
        if (end > m_nDeskNum)
            end = m_nDeskNum;

        if (send)
        {
            DebugLog.LogWarning("Inview:" + begin + "-" + end);

            ushort start = 0;
            ushort stop = 0;
            for (uint i = begin; i <= end; i++)
            {
                RoomDeskInfo di = m_dictIndexDesks[i];
                if (!di.m_bInited)
                {
                    if (start == 0)
                        start = (ushort)i;
                }
                else if (start != 0)
                    stop = (ushort)(i - 1);
            }

            if (stop == 0)
                stop = (ushort)end;

            if (start != 0)
            {
                UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_UPDATEBEFOREHANDROOMINFO);
                msg.Add((byte)GameMain.hall_.GameBaseObj.GetGameType());
                msg.Add(GameMain.hall_.CurRoomIndex);
                msg.Add(start);
                msg.Add((ushort)(stop - start + 1));
                HallMain.SendMsgToRoomSer(msg);

                DebugLog.Log("Request desk info:" + start + "-" + stop + "level:" + GameMain.hall_.CurRoomIndex);
            }
        }
        else
        {
            foreach (var di in m_dictIndexDesks)
                di.Value.Show(di.Key >= begin && di.Key <= end);
        }
    }

    void OnClickSit(ushort desk, sbyte sit)
    {
        CustomAudio.GetInstance().PlayCustomAudio(1002);

        DebugLog.Log("Click desk:" + desk + " sit:" + sit);

        CCustomDialog.OpenCustomWaitUI("正在进入...");
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_APPLYENTERROOMANDSIT);
        msg.Add((byte)GameMain.hall_.GameBaseObj.GetGameType());
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(desk);
        msg.Add(sit);
        HallMain.SendMsgToRoomSer(msg);    }

    bool HandleEnterDesk(uint _msgType, UMessage _ms)
    {
        if (m_nDeskNum == 0)
            return false;

        CCustomDialog.CloseCustomWaitUI();

        byte state = _ms.ReadByte();
        if(state > 0)
        {
            if (state == 7)
            {
                //int coin = _ms.ReadInt();
                CCustomDialog.OpenCustomConfirmUI(1652);
            }else if(state ==8)//进入旁观
            {
                 m_nBystanderRoom = _ms.ReadUInt();
                 GameMain.hall_.GameBaseObj.StartLoad();
            }
            else
            {
                uint code = 2005;
                if (state == 6)
                    code = 1704;
                CCustomDialog.OpenCustomConfirmUI(code);
            }
            return false;
        }

        Dictionary<byte, AppointmentRecordPlayer> players = new Dictionary<byte, AppointmentRecordPlayer>();
        AppointmentRecordPlayer player;
        byte num = _ms.ReadByte();
        for(int i = 0; i < num; i++)
        {
            player = new AppointmentRecordPlayer();
            byte sit = _ms.ReadByte();
            player.playerid = _ms.ReadUInt();
            player.faceid = _ms.ReadInt();
            player.url = _ms.ReadString();
            player.coin = _ms.ReadLong();
            player.playerName = _ms.ReadString();
            player.master = _ms.ReadSingle();
            player.sex = _ms.ReadByte();
            player.ready = _ms.ReadByte();
            players[sit] = player;
        }
        byte roomId = _ms.ReadByte();
        uint deskId = _ms.ReadUInt();

        ShowTable(true, players, roomId, deskId);

        GameMain.hall_.GameBaseObj.StartLoad();

        return true;
    }

    bool HandleDeskInfo(uint _msgType, UMessage _ms)
    {
        if (m_nDeskNum == 0)
            return false;

        byte sit = _ms.ReadByte();
        uint userId = _ms.ReadUInt();
        int faceId = _ms.ReadInt();
        string url = _ms.ReadString();
        long coin = _ms.ReadLong();
        string name = _ms.ReadString();
        float master = _ms.ReadSingle();
        byte sex = _ms.ReadByte();
        byte ready = _ms.ReadByte();
        if (sit < m_ChairList.Count)
        {
            m_ChairList[sit].Show(true, name, url, userId, faceId);
            m_ChairList[sit].SetReady(ready == 1);
        }
        return true;
    }

    void OnClickDeskInvitate()
    {

    }

    void OnClickDeskReady()
    {
        CustomAudio.GetInstance().PlayCustomAudio(1002);

        TableTfm.Find("bottom/Button_Ready").GetComponent<Button>().interactable = false;

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_APPLYREADY);
        msg.Add((byte)GameMain.hall_.GameBaseObj.GetGameType());
        msg.Add(GameMain.hall_.GetPlayerId());
        HallMain.SendMsgToRoomSer(msg);    }

    public void StartGame()
    {
        ShowTable(false);
        RoomTfm.gameObject.SetActive(false);
    }

    public void ShowKickTip(bool show, float time = 0f)
    {
        if (show)
        {
            if (!TableTfm.Find("bottom/Button_Ready").GetComponent<Button>().interactable)
                return;
        }

        if(m_TipText != null)
        {
            m_TipText.transform.parent.gameObject.SetActive(show);
        }

        m_fTipTimer = time;
    }

    public void SetPlayerOffline(byte sit, bool offline)
    {
        if (sit < m_ChairList.Count)
            m_ChairList[sit].ShowOffline(offline);
    }
}
