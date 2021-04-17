//#define HAVE_LOBBY
#define MATCH_ROOM
using DG.Tweening;
using System.Collections;using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using USocket.Messages;
using UnityEngine.EventSystems;
using XLua;
using System;

[Hotfix]
public class CGame_XzMahjong : CGame_Mahjong
{
    Button[] m_LackButton;
    Button m_ChangeButton;
    public bool ChangeBtnInactive
    {
        get
        {
            if (m_ChangeButton == null)
                return false;
            return m_ChangeButton.interactable;
        }
        set
        {
            if (m_ChangeButton != null)
                m_ChangeButton.interactable = value;
        }
    }

    public CGame_XzMahjong(GameTye_Enum gameType, GameKind_Enum gameKind) : base(gameType, gameKind)
    {
    }

    public override void InitPlayers()
    {
        Mahjong_Role role;
        role = new XzMahjong_RoleLocal(this, 1);
        role.Init();        m_PlayerList.Add(role);

        for (byte i = 2; i < 5; i++)
        {
            role = new Mahjong_RoleOther(this, i);
            role.Init();            m_PlayerList.Add(role);
        }
    }

    public override void LoadMainUI(Transform root)
    {
        m_nTotalTilesNum = 108;
        m_nMaxTileValue = 0x30;

        GameObject obj;
        byte index = 0;
        Transform tfm;

        obj = (GameObject)MahjongAssetBundle.LoadAsset("Mahjong_Game");
        MainUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        MainUITfm.SetParent(root, false);
        MainUITfm.gameObject.SetActive(false);

        tfm = MainUITfm.Find("Pop-up");
        m_TrustButton = tfm.Find("button_tuoguan/Button_guanbi").GetComponent<Button>();
        m_TrustButton.onClick.AddListener(() => OnClickTrustBtn(GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_CANCLETRUSTSTATE));

        m_LackButton = tfm.Find("button_que").GetComponentsInChildren<Button>();
        index = 0;
        foreach (Button btn in m_LackButton)
        {
            byte temp = index;
            btn.onClick.AddListener(() => OnClickLackBtn(temp));
            index++;
        }

        ChooseKongTfm = tfm.Find("choose/choose_gang");
        Button[] buttons = ChooseKongTfm.GetComponentsInChildren<Button>();
        index = 0;
        foreach (Button btn in buttons)
        {
            byte temp = index;
            btn.onClick.AddListener(() => OnClickGameBtn(MjOtherDoing_Enum.MjOtherDoing_Gang, temp, GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_ANSWERDOING));
            index++;
        }

        tfm = tfm.Find("button_option");
        string[] btnName = new string[] { "Button_guo", "Button_peng", "Button_gang", "Button_he" };
        index = 0;
        foreach (MjOtherDoing_Enum item in System.Enum.GetValues(typeof(MjOtherDoing_Enum)))
        {
            if (index >= btnName.Length)
                break;

            Transform child = tfm.Find(btnName[index]);
            index++;

            if (child == null)
                continue;
            Button btn = child.GetComponent<Button>();
            if (btn == null)
                continue;

            m_GameButton[item] = btn;
            btn.onClick.AddListener(() => OnClickGameBtn(item, RoomInfo.NoSit, GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_ANSWERDOING));
        }

        tfm = MainUITfm.Find("Pop-up/change/ImageBG/Buttonok");
        m_ChangeButton = tfm.GetComponent<Button>();
        m_ChangeButton.onClick.AddListener(() => OnClickChange(true));

        ChangeTfm = GameObject.Find("Game_Model/zhuomian/Change").transform;
        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameType, "Mahjong_AudioCsv");
    }

    public override void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_CHOOSElEVEL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ENTERROOM, HandleEnterRoom);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ROOMSTATE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_OTHERENTER, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_DEALMJBEGIN, HandleSendTile);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_UPDATECHANGEPOKERS, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_UPDATEMAKELACK, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_FIRSTASKBANKERDEAL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_UPDATEDEALPOKER, HandleBackOtherDiscard);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_GETPOKERASKDEAL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ANSWERDOING, HandleAnswer);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_UPDATEDOING, HandleOtherAnswer);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_PLAYERHUPOKER, HandlePlayerWin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_PUBLISHRESULT, HandleResult);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_BACKLEAVE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ENTERTRUSTSTATE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_MIDDLEENTERROOM, HandleMiddleEnterRoom);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_AFTERQIANGGANGHUCUTCOIN, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_AFTERONLOOKERENTER, HandleBystanderEnter);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_LIUJUINFO, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_HUJIAOZHUANYI, HandleGameNetMsg);    }

    public override void CreateWallTiles()
    {
        foreach (Mahjong_Role role in m_PlayerList)
            role.CreateWallTiles((byte)((role.m_nSit % 2 == 0) ? 14 : 13));
    }

    public override string GetBeiShuStr()
    {
        return "倍";
    }

    protected override string GetMahJiongGameRule()
    {
        return "血战到底 ";
    }

    protected override IEnumerator ShowSendTile(int dealIndex, List<byte> list, byte dealerSit, List<byte> players)
    {
        yield return base.ShowSendTile(dealIndex, list, dealerSit, players);

        yield return new WaitUntil(() => GameState == MahjongRoomState_Enum.MjRoomState_WaitChangeMj);

        OnLocalChanged(false);
    }

    public override bool HandleGameNetMsg(uint _msgType, UMessage _ms)
    {
        if (base.HandleGameNetMsg(_msgType, _ms))
            return true;

        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;
        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_UPDATECHANGEPOKERS:
                {
                    byte nState = _ms.ReadByte();
                    byte nSit = _ms.ReadByte();
                    if (nState == 0)
                    {
                        byte cSit = m_dictSitPlayer[nSit];
                        m_PlayerList[cSit].ShowChange(true);
                    }
                    else
                    {
                        ChangeBtnInactive = true;
                        DebugLog.LogWarning("sit:" + nSit + " change error Code:" + nState);
                    }
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_UPDATEMAKELACK:
                {
                    byte state = _ms.ReadByte();
                    byte suit = _ms.ReadByte();

                    if (state == 0)
                        OnLocalMakeLack(suit);
                }
                break;

            default:
                return false;
        }
        return true;
    }

    public override void OnEnterState(MahjongRoomState_Enum state, UMessage _ms, byte mode, float timeLeft)
    {
        base.OnEnterState(state, _ms, mode, timeLeft);

        switch (state)
        {
            case MahjongRoomState_Enum.MjRoomState_MakeLackCartoon:
                {
                    ShowTips(false);

                    byte num = _ms.ReadByte();
                    for (int i = 0; i < num; i++)
                    {
                        byte sit = _ms.ReadByte();
                        byte suit = _ms.ReadByte();
                        m_PlayerList[m_dictSitPlayer[sit]].OnLackCofirm(suit, mode == 1);
                    }
                    OnLocalMakeLack();
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_WaitMakeLack:
                {
                    if (mode > 0)
                        ShowLackCofirm();
                    else
                        timeLeft = _ms.ReadSingle();

                    Desk.StartCountdown(timeLeft);
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_WaitChangeMj:
                {
                    if (mode > 0)
                    {
                        Desk.StartCountdown(timeLeft);

                        byte num = _ms.ReadByte();
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = _ms.ReadByte();
                            bool changed = (_ms.ReadByte() == 1);
                            byte cSit = m_dictSitPlayer[sit];
                            m_PlayerList[cSit].ShowChange(changed, true);
                        }
                    }
                    else
                        Desk.StartCountdown(_ms.ReadSingle());
                }
                break;

            case MahjongRoomState_Enum.MjRoomState_ChangeMjCartoon:
                {
                    if (mode == 0)
                    {
                        MjChangePokerDire_Enum dir = (MjChangePokerDire_Enum)_ms.ReadByte();
                        byte num = _ms.ReadByte();
                        List<byte> list = new List<byte>();
                        for (int i = 0; i < num; i++)
                        {
                            list.Add(_ms.ReadByte());
                        }
                        GameMain.SC(OnChangeTiles(dir, new List<byte>[] { list, list, list, list }));
                    }
                    Desk.StartCountdown(0);
                }
                break;

            default:
                break;
        }
    }

    IEnumerator OnChangeTiles(MjChangePokerDire_Enum dir, List<byte>[] list, float totalTime = 5f, bool wait = true)
    {
        string str;
        if (dir == MjChangePokerDire_Enum.MjChangePokerDire_Clockwise)
        {
            ChangeTfm.DOLocalRotate(new Vector3(0f, 90f, 0f), 0.2f * totalTime);
            str = "Anime_changeshun";
        }
        else if (dir == MjChangePokerDire_Enum.MjChangePokerDire_AntiClockwise)
        {
            ChangeTfm.DOLocalRotate(new Vector3(0f, -90f, 0f), 0.2f * totalTime);
            str = "Anime_changeni";
        }
        else
        {
            ChangeTfm.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.2f * totalTime);
            str = "Anime_changedui";
        }
        PlayEffect(true, str, 0.4f * totalTime);
        ShowTips(false);

        yield return new WaitForSecondsRealtime(0.4f * totalTime);

        foreach (byte value in m_dictSitPlayer.Values)
        {
            m_PlayerList[value].ShowChange(false);
            m_PlayerList[value].GetChangedTiles(new List<byte>(list[value]));
        }
        ChangeTfm.localEulerAngles = Vector3.zero;
        OnLocalChanged(false, false);

        if (!wait)
            yield break;

        yield return new WaitForSecondsRealtime(0.4f * totalTime);

        yield return new WaitUntil(() => GameState == MahjongRoomState_Enum.MjRoomState_WaitMakeLack);

        ShowLackCofirm();
    }

    void OnClickLackBtn(byte index)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(3006);

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_MAKELACK);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(index);
        HallMain.SendMsgToRoomSer(msg);
    }

    void OnClickChange(bool bClick)
    {
        if (bClick)
            CustomAudioDataManager.GetInstance().PlayAudio(3006);

        ((XzMahjong_RoleLocal)m_PlayerList[0]).OnRequestChange(bClick);
        ChangeBtnInactive = false;
    }

    void OnLocalMakeLack(byte index = 0)
    {
        foreach (Button btn in m_LackButton)
        {
            btn.transform.GetChild(0).gameObject.SetActive(false);
        }
        m_LackButton[index].transform.parent.gameObject.SetActive(false);
        Desk.StartCountdown(0);
    }

    void ShowLackCofirm()
    {
        ShowTips(true, "定缺中...");

        List<byte> list;
        int suit = ((Mahjong_RoleLocal)m_PlayerList[0]).GetRecommendSuit(0, out list);
        Button button = m_LackButton[suit];
        button.transform.parent.gameObject.SetActive(true);

        Transform tfm = button.transform.GetChild(0);
        tfm.gameObject.SetActive(true);
    }

    public void OnLocalChanged(bool changed, bool showBtn = true)
    {
        if (m_PlayerList.Count == 0 || m_ChangeButton == null)
            return;

        if (changed)
        {
            m_ChangeButton.transform.parent.parent.gameObject.SetActive(false);
            ShowTips(true, "等待换牌中...");
            Desk.StartCountdown(0);
        }
        else
        {
            Transform tfm = m_ChangeButton.transform.parent.parent;
            tfm.gameObject.SetActive(showBtn);
            ShowTips(false);
            if (showBtn && GameMode != GameTye_Enum.GameType_Record)
                ((XzMahjong_RoleLocal)m_PlayerList[0]).ChooseChangeTiles();
        }
    }

    public override void ResetGameUI()
    {
        base.ResetGameUI();

        ChangeBtnInactive = true;
        OnLocalChanged(false, false);
    }

    public override bool OnVideoStep(List<VideoAction> actionList, int curStep, bool reverse)
    {
        VideoAction action = actionList[curStep];
        List<int> list = action.list;
        int j = 0;
        bool res = true;
        if (!reverse)
        {
            switch (action.vai)
            {
                case VideoActionInfo_Enum.VideoActionInfo_203:
                    {
                        OnLocalChanged(false);
                        Desk.StartCountdown(5f, GameVideo.GetInstance().m_bPause);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_212:
                    {
                        OnLocalChanged(false, false);

                        int num = list[j++];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte cSit = m_dictSitPlayer[sit];

                            List<byte> tiles = new List<byte>();
                            byte tileNum = (byte)list[j++];
                            for (int k = 0; k < tileNum; k++)
                                tiles.Add((byte)list[j++]);
                            m_PlayerList[cSit].ShowChange(true, false, tiles);
                            m_PlayerList[cSit].UpdateTiles();
                        }
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_204:
                    {
                        Desk.StartCountdown(0f);

                        MjChangePokerDire_Enum dir = (MjChangePokerDire_Enum)list[j++];
                        int num = list[j++];
                        List<byte>[] tileList = new List<byte>[num];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte cSit = m_dictSitPlayer[sit];

                            List<byte> tiles = new List<byte>();
                            byte tileNum = (byte)list[j++];
                            for (int k = 0; k < tileNum; k++)
                                tiles.Add((byte)list[j++]);
                            tileList[cSit] = tiles;
                        }
                        GameMain.SC(OnChangeTiles(dir, tileList, GameVideo.GetInstance().GetStepTime(), false));
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_205:
                    {
                        ShowLackCofirm();
                        Desk.StartCountdown(5f, GameVideo.GetInstance().m_bPause);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_206:
                    {
                        ShowTips(false);
                        OnLocalMakeLack();
                        Desk.StartCountdown(0f);

                        byte num = (byte)list[j++];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte suit = (byte)list[j++];
                            m_PlayerList[m_dictSitPlayer[sit]].OnLackCofirm(suit, false, GameVideo.GetInstance().GetStepTime());
                        }
                    }
                    break;

                default:
                    res = false;
                    break;
            }
        }
        else
        {
            switch (action.vai)
            {
                case VideoActionInfo_Enum.VideoActionInfo_203:
                    {
                        OnLocalChanged(false, false);
                        Desk.StartCountdown(-1f);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_212:
                    {
                        int num = list[j++];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte cSit = m_dictSitPlayer[sit];
                            m_PlayerList[cSit].ShowChange(false);

                            byte tileNum = (byte)list[j++];
                            for (int k = 0; k < tileNum; k++)
                                m_PlayerList[cSit].m_HaveTiles.Add((byte)list[j++]);
                            m_PlayerList[cSit].UpdateTiles();
                        }

                        OnVideoStep(actionList, curStep - 1, false);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_204:
                    {
                        MjChangePokerDire_Enum dir = (MjChangePokerDire_Enum)list[j++];
                        int num = list[j++];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte cSit = m_dictSitPlayer[sit];

                            byte tileNum = (byte)list[j++];
                            for (int k = 0; k < tileNum; k++)
                                m_PlayerList[cSit].m_HaveTiles.Remove((byte)list[j++]);
                        }

                        OnVideoStep(actionList, curStep - 1, false);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_205:
                    {
                        ShowTips(false);
                        OnLocalMakeLack();
                        Desk.StartCountdown(0f);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_206:
                    {
                        foreach (Mahjong_Role role in m_PlayerList)
                            role.OnLackCofirm(0, false, -1f);
                        OnVideoStep(actionList, curStep - 1, false);
                    }
                    break;

                default:
                    res = false;
                    break;
            }
        }

         if (res)
        {
            DebugLog.Log("YcMahjong OnVideoStep:" + action.vai + " rev:" + reverse);
            return true;
        }

        return base.OnVideoStep(actionList, curStep, reverse);
    }

    /// <summary>
    /// 更新约据房间信息
    /// </summary>
    /// <param name="Data"></param>
    protected override void UpdateAppointmentData(AppointmentData Data, UMessage _ms)
    {
        MahjongAppointmentData mahjongData = (MahjongAppointmentData)Data;
        mahjongData.palyType = _ms.ReadShort();
    }

    protected override void OnLeaveLookRoom(GameCity.EMSG_ENUM GameCityMsg)
    {
        base.OnLeaveLookRoom(GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_LEAVEONLOOKERROOM);
    }
}