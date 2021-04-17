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
public class CGame_CzMahjong : CGame_Mahjong
{
    Text m_AddPriceText;
    int m_nAddPrice = -1;

    Transform MahjongChatTransform;
    protected Transform ChooseChiTransform;
    public byte m_BrightCard = 0; //明牌
    public byte m_nDiHua = 0;     //底花
    public MinHuRule m_nMinHuRule = MinHuRule.eMHR_None; //起胡
    protected byte m_nZhuangJiaSit = 0;//庄家服务端座位号
    public int AddPrice
    {
        get
        {
            return m_nAddPrice;
        }

        set
        {
            m_nAddPrice = value;
            //if(m_AddPriceText != null)
            //{
            //    //m_AddPriceText.transform.parent.gameObject.SetActive(m_nAddPrice >= 0);
            //    //m_AddPriceText.text = m_nAddPrice.ToString();
            //}
        }
    }

    public CGame_CzMahjong(GameTye_Enum gameType, GameKind_Enum gameKind) : base(gameType, gameKind)
    {
    }

    public override void InitPlayers()
    {
        Mahjong_Role role;
        role = new CzMahjong_RoleLocal(this, 1);
        role.Init();        m_PlayerList.Add(role);

        for (byte i = 2; i < 5; i++)
        {
            role = new CzMahjong_RoleOther(this, i);
            role.Init();            m_PlayerList.Add(role);
        }
    }

    public override void LoadMainUI(Transform root)
    {
        m_nTotalTilesNum = 144;
        m_nMaxTileValue = 0x50;

        GameObject obj;
        byte index = 0;
        Transform tfm;

        obj = (GameObject)MahjongAssetBundle.LoadAsset("CZ_Mahjong_Game");
        MainUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        MainUITfm.SetParent(root, false);
        MainUITfm.gameObject.SetActive(false);

        m_AddPriceText = MainUITfm.Find("Top/jia/TextNum").GetComponent<Text>();
        AddPrice = -1;

        MainUITfm.Find("Top/cai").gameObject.SetActive(false);

        tfm = MainUITfm.Find("Pop-up");
        m_TrustButton = tfm.Find("button_tuoguan/Button_guanbi").GetComponent<Button>();
        m_TrustButton.onClick.AddListener(() => OnClickTrustBtn(GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_CM_CANCLETRUSTSTATE));

        ChooseKongTfm = InitChooseKongClickEvent("Pop-up/choose/choose_gang", MjOtherDoing_Enum.MjOtherDoing_Gang);

        ChooseChiTransform = InitChooseKongClickEvent("Pop-up/choose/choose_chi", MjOtherDoing_Enum.MjOtherDoing_Chi);

        MainUITfm.Find("Middle/Imagechat_BG/Button_chat").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (MahjongChatTransform) { CustomAudioDataManager.GetInstance().PlayAudio(3006); MahjongChatTransform.gameObject.SetActive(true); }
        });

        tfm = tfm.Find("button_option");
        string[] btnName = new string[] { "Button_guo", "Button_peng", "Button_gang", "Button_he", "Button_chi", "Button_ting", "Button_tingfei" };
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
            btn.onClick.AddListener(() => OnClickGameBtn(item, RoomInfo.NoSit, GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_CM_ANSWERDOING));
        }

        m_GameButton[MjOtherDoing_Enum.MjOtherDoing_Chi].gameObject.AddComponent<CommonUserData>();
        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameType, "Mahjong_AudioCsv_CZ");
        LoadMahjongChatUI(root);
    }
    /// <summary>
    /// 加载俏皮话界面
    /// </summary>
    /// <param name="parentTransform">俏皮话界面父节点</param>
    void LoadMahjongChatUI(Transform parentTransform)
    {
        UnityEngine.Object obj = MahjongAssetBundle.LoadAsset("majiang_Chat");
        MahjongChatTransform = ((GameObject)GameMain.instantiate(obj)).transform;
        MahjongChatTransform.SetParent(parentTransform, false);
        MahjongChatTransform.gameObject.SetActive(false);

        int index = 1;
        Button[] chatButtons = MahjongChatTransform.Find("Chat_Viewport/Chat_Content").GetComponentsInChildren<Button>();
        foreach (var chatButton in chatButtons)
        {
            int tempIndex = index;
            chatButton.onClick.AddListener(() => OnClickMahjongChat(tempIndex));
            index++;
        }
    }

    protected void OnClickMahjongChat(int index)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(3006);
        if (MahjongChatTransform)
        {
            MahjongChatTransform.gameObject.SetActive(false);
        }

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION);        msg.Add(GameMain.hall_.GetPlayerId());        msg.Add((byte)index);        HallMain.SendMsgToRoomSer(msg);    }

    Transform InitChooseKongClickEvent(string name, MjOtherDoing_Enum doingEnum)
    {
        if (MainUITfm == null || string.IsNullOrEmpty(name) ||
            (doingEnum != MjOtherDoing_Enum.MjOtherDoing_Chi && doingEnum != MjOtherDoing_Enum.MjOtherDoing_Gang))
            return null;
        Transform transform = MainUITfm.Find(name);
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        byte index = 0;
        foreach (Button btn in buttons)
        {
            byte temp = index;
            btn.onClick.AddListener(() => OnClickGameBtn(doingEnum, temp, GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_CM_ANSWERDOING));
            index++;
        }
        return transform;
    }

    public override void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_CHOOSElEVEL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_ENTERROOM, HandleEnterRoom);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_ROOMSTATE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_OTHERENTER, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_DEALMJBEGIN, HandleSendTile);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_UPDATEPOKERSAFTERBUHUA, HandleAfterBuhua);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_FIRSTASKBANKERDEAL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_UPDATEDEALPOKER, HandleBackOtherDiscard);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_GETPOKERASKDEAL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_ANSWERDOING, HandleAnswer);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_UPDATEDOING, HandleOtherAnswer);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_PLAYERHUPOKER, HandlePlayerWin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_PUBLISHRESULT, HandleResult);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_BACKLEAVE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_ENTERTRUSTSTATE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_MIDDLEENTERROOM, HandleMiddleEnterRoom);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_AFTERONLOOKERENTER, HandleBystanderEnter);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION, HandEmotionNetMsg);    }

    public override bool HandleGameNetMsg(uint _msgType, UMessage _ms)
    {
        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;
        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_CHOOSElEVEL:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_CHOOSElEVEL, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_ROOMSTATE:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ROOMSTATE, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_OTHERENTER:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_OTHERENTER, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_FIRSTASKBANKERDEAL:
                {
                    //base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_FIRSTASKBANKERDEAL, _ms);
                    byte sit = _ms.ReadByte(); //玩家桌子序号
                    byte doing = _ms.ReadByte();

                    //补花以后的牌(花牌个数一样)
                    byte pokerNum = _ms.ReadByte();
                    List<byte> pokerList = new List<byte>();
                    for (byte i = 0; i < pokerNum; i++)
                    {
                        byte vaule = _ms.ReadByte();
                        pokerList.Add(vaule);
                    }

                    //杠牌
                    byte num = _ms.ReadByte();
                    List<byte> list = new List<byte>();
                    for (byte i = 0; i < num; i++)
                        list.Add(_ms.ReadByte());

                    float time = _ms.ReadSingle();
                    uint sign = _ms.ReadUInt();
                    byte cSit = m_dictSitPlayer[sit];


                    //花牌
                    byte huaNum = _ms.ReadByte();
                    List<byte> huaList = new List<byte>();
                    for (byte i = 0; i < huaNum; i++)
                    {
                        byte huaValue = _ms.ReadByte();
                        huaList.Add(huaValue);
                    }

                    FirstRoundBuHuaLogic(sit, time, sign, doing, huaList, pokerList, list);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_GETPOKERASKDEAL:
                {
                    byte sit = _ms.ReadByte();
                    sbyte tile = _ms.ReadSByte();
                    byte doing = _ms.ReadByte();
                    byte num = _ms.ReadByte();

                    List<byte> list = new List<byte>();
                    for (byte i = 0; i < num; i++)
                        list.Add(_ms.ReadByte());

                    float time = _ms.ReadSingle();
                    uint sign = _ms.ReadUInt();

                    num = _ms.ReadByte();
                    if (num > 0)
                    {
                        List<byte> listHua = new List<byte>();
                        for (byte i = 0; i < num; i++)
                            listHua.Add(_ms.ReadByte());

                        GameMain.SC(OnDealHua(sit, tile, doing, sign, time, list, listHua));
                    }
                    else
                        HandleAskDeal(sit, (byte)tile, doing, sign, time, list);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_BACKLEAVE:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_BACKLEAVE, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_ENTERTRUSTSTATE:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ENTERTRUSTSTATE, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER:                {
                    base.HandleGameNetMsg(_msgType, _ms);
                }                break;

            default:
                return false;
        }
        return true;
    }

    //首轮补花相关逻辑
    void FirstRoundBuHuaLogic(byte sit, float time, uint sign, byte doing, List<byte> huaList, List<byte> pokerList, List<byte> list)
    {
        //确保补花零位置的牌显示正确（刚摸得牌）
        m_PlayerList[m_dictSitPlayer[sit]].OnTurn = (m_nZhuangJiaSit == sit);

        byte huaNum = (byte)huaList.Count;
        if (m_nZhuangJiaSit != sit)
        {
            byte poker = RoomInfo.NoSit;
            if (pokerList.Count > 0)
            {
                poker = pokerList.Last();
                pokerList.Remove(poker);
            }

            m_PlayerList[m_dictSitPlayer[sit]].OnAfterBuhua(huaList, pokerList);
            m_PlayerList[m_dictSitPlayer[sit]].UpdateMahjongHaveTiles(huaList, pokerList);
            if (poker != RoomInfo.NoSit)
                m_PlayerList[m_dictSitPlayer[sit]].DealTile(poker, m_dictSitPlayer[sit] == 0, false);

            huaNum += 1;
        }
        else
        {
            m_PlayerList[m_dictSitPlayer[sit]].OnAfterBuhua(huaList, pokerList);
            m_PlayerList[m_dictSitPlayer[sit]].UpdateMahjongHaveTiles(huaList, pokerList);
        }

        OnChangeTurn(m_dictSitPlayer[sit], doing, sign, list);
        Desk.StartCountdown(time, false, m_dictSitPlayer[sit] == 0);

        //刷
        HideWallTile(huaNum, false);

        if (m_dictSitPlayer[sit] == 0)
            ((Mahjong_RoleLocal)m_PlayerList[0]).CheckTing();
    }

    bool HandleAfterBuhua(uint _msgType, UMessage _ms)
    {
        byte leftTiles = _ms.ReadByte();
        List<byte> tiles = new List<byte>();
        byte num = _ms.ReadByte();
        for (int i = 0; i < num; i++)
        {
            tiles.Add(_ms.ReadByte());
        }

        List<byte> addList = new List<byte>(tiles);
        foreach (byte tile in m_PlayerList[0].m_HaveTiles)
            addList.Remove(tile);

        DebugLog.Log("HandleAfterBuhua!!! leftTile:" + leftTiles);
        byte roleNum = _ms.ReadByte();
        byte huaNum = 0;
        List<byte> huaList = new List<byte>();
        for (int i = 0; i < roleNum; i++)
        {
            byte sit = _ms.ReadByte();
            huaList.Clear();
            num = _ms.ReadByte();
            for (int j = 0; j < num; j++)
                huaList.Add(_ms.ReadByte());
            m_PlayerList[m_dictSitPlayer[sit]].OnAfterBuhua(huaList, addList);
            DebugLog.Log("Buhua sit:" + sit + " num:" + num);
            huaNum += num;
        }

        m_PlayerList[0].m_HaveTiles = new List<byte>(tiles);
        m_PlayerList[0].UpdateTiles();

        HideWallTile(huaNum, false);

        return true;
    }

    public override void CreateWallTiles()
    {
        foreach (Mahjong_Role role in m_PlayerList)
            role.CreateWallTiles(18);
    }

    IEnumerator OnDealHua(byte sit, sbyte tile, byte doing, uint sign, float time, List<byte> listKong, List<byte> listHua)
    {
        byte hua;
        byte cSit = m_dictSitPlayer[sit];
        Mahjong_Role role = m_PlayerList[cSit];
        role.SetCanHit(false);
        DebugLog.Log("补花 name:" + role.GetRoleName() + " num:" + listHua.Count);

        int count = listHua.Count;
        if (tile < 0)
            count -= 1;
        for (byte i = 0; i < count; i++)
        {
            hua = listHua[i];

            HandleAskDeal(sit, hua, 0, 0, time);

            yield return new WaitForSecondsRealtime(0.5f);

            role.OnAfterBuhua(new List<byte> { hua });

            yield return new WaitForSecondsRealtime(0.2f);
        }

        if (tile < 0)
            HandleAskDeal(sit, listHua[count], doing, sign, time, listKong);
        else
        {
            HandleAskDeal(sit, (byte)tile, doing, sign, time, listKong);

            if (cSit == 0)
                ((Mahjong_RoleLocal)role).CheckTing(true);
        }

        role.SetCanHit(true);
    }

    public override void OnEnterState(MahjongRoomState_Enum state, UMessage _ms, byte mode, float timeLeft)
    {
        base.OnEnterState(state, _ms, mode, timeLeft);

        if (mode == 0)
        {
            if (state == MahjongRoomState_Enum.MjRoomState_ThrowDice)
            {
                AddPrice = _ms.ReadByte();
            }
        }
    }

    public override void OnEnd()
    {
        AddPrice = -1;
        m_nTotalTilesNum = 144;
        m_BrightCard = 0;                   //明牌
        m_nZhuangJiaSit = 0;                //庄家服务端座位号
        if (MainUITfm)
        {
            MainUITfm.Find("Top/cai").gameObject.SetActive(false);
        }
        base.OnEnd();
    }

    public override string GetBeiShuStr()
    {
        return "倍";
    }

    protected override bool HandleMiddleEnterRoom(uint _msgType, UMessage _ms)
    {
        if(!base.HandleMiddleEnterRoom(_msgType, _ms))
        {
            return false;
        }

        return true;
    }

    protected override bool HandleBystanderEnter(uint _msgType, UMessage _ms)
    {
        base.HandleBystanderEnter(_msgType, _ms);
        return true;
    }

    protected override string GetMahJiongGameRule()
    {
        return "常州麻将 ";
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
                case VideoActionInfo_Enum.VideoActionInfo_207:
                    {
                        byte sit = (byte)list[j++];
                        sbyte tile = (sbyte)list[j++];
                        byte doing = (byte)list[j++];
                        byte lastDealSit = (byte)list[j++];
                        byte huaNum = (byte)list[j++];
                        List<byte> huaList = new List<byte>();
                        for (int i = 0; i < huaNum; i++)
                        {
                            huaList.Add((byte)list[j++]);
                        }

                        byte hideNum = huaNum;
                        byte lastTile = (byte)tile;
                        if (tile < 0)
                        {
                            int endIndex = huaList.Count - 1;
                            lastTile = huaList[endIndex];
                            huaList.RemoveAt(endIndex);
                            hideNum--;
                        }
                        m_PlayerList[m_dictSitPlayer[sit]].OnAfterBuhua(huaList, new List<byte>(), false, GameVideo.GetInstance().GetStepTime());
                        HideWallTile(hideNum, false);
                        HandleAskDeal(sit, lastTile, doing, 0, 15f, null, GameVideo.GetInstance().m_bPause);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_213:
                    {
                        //玩家座位号
                        byte sit = (byte)list[j++];
                        //获取上一次出牌成功人的座位号
                        byte lastSit = (byte)list[j++];
                        byte doing = (byte)list[j++];
                        byte num = (byte)list[j++];
                        List<byte> addList = new List<byte>();
                        for (int k = 0; k < num; k++)
                            addList.Add((byte)list[j++]);
                        num = (byte)list[j++];
                        List<byte> huaList = new List<byte>();
                        for (int k = 0; k < num; k++)
                            huaList.Add((byte)list[j++]);

                        FirstRoundBuHuaLogic(sit, 1.0f, 0, doing, huaList, addList, null);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_202:
                    {
                        //明牌
                        byte brightCard = (byte)list[j++];
                        //原子
                        byte atomicCard = (byte)list[j++];

                        UpdateBrightCardOrAtomicCard(brightCard, atomicCard);

                        if (m_nBrightCardSit != -1 && m_PlayerList.Count > m_nBrightCardSit)
                        {
                            m_PlayerList[m_nBrightCardSit].UpdateBrightCardTile(GetBrightCardIndex(m_nBrightCardSit), m_BrightCard);
                        }
                        byte dealSit = (byte)list[j++];
                        int num = list[j++];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte clientSit = m_dictSitPlayer[sit];
                            byte tileNum = (byte)list[j++];
                            List<byte> tileList = new List<byte>();
                            for (int k = 0; k < tileNum; k++)
                            {
                                byte tile = (byte)list[j++];
                                tileList.Add(tile);
                            }
                            m_PlayerList[clientSit].OnTurn = (sit == dealSit);
                            m_PlayerList[clientSit].OnMidEnt(false, 0, tileList, -1);
                        }

                        SyncWallTiles();

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
                case VideoActionInfo_Enum.VideoActionInfo_207:
                    {
                        byte sit = (byte)list[j++];
                        sbyte tile = (sbyte)list[j++];
                        byte doing = (byte)list[j++];
                        byte lastDealSit = (byte)list[j++];
                        byte huaNum = (byte)list[j++];
                        List<byte> huaList = new List<byte>();
                        for (int i = 0; i < huaNum; i++)
                        {
                            huaList.Add((byte)list[j++]);
                        }

                        byte hideNum = huaNum;
                        byte lastTile = (byte)tile;
                        if (tile < 0)
                        {
                            int endIndex = huaList.Count - 1;
                            lastTile = huaList[endIndex];
                            huaList.RemoveAt(endIndex);
                            hideNum--;
                        }
                        m_PlayerList[m_dictSitPlayer[sit]].OnAfterBuhua(huaList, new List<byte>(), false, -GameVideo.GetInstance().GetStepTime());
                        HideWallTile(hideNum, true);
                        HandleAskDeal(sit, lastTile, 0, 0, -15f);

                        if (lastDealSit >= 0)
                            OnChangeTurn(m_dictSitPlayer[lastDealSit]);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_213:
                    {
                        OnVideoStep(actionList, curStep - 2, false);
                        OnVideoStep(actionList, curStep - 1, false);
                    }
                    break;
                case VideoActionInfo_Enum.VideoActionInfo_202:
                    {
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
            DebugLog.Log("CzMahjong OnVideoStep:" + action.vai + " rev:" + reverse);
            return true;
        }

        return base.OnVideoStep(actionList, curStep, reverse);
    }

    protected override bool HandleSendTile(uint _msgType, UMessage _ms)
    {
        base.HandleSendTile(_msgType, _ms);

        //明牌
        byte brightCard = _ms.ReadByte();
        //原子
        byte atomicCard = _ms.ReadByte();

        UpdateBrightCardOrAtomicCard(brightCard, atomicCard);

        if (m_nBrightCardSit != -1 && m_PlayerList.Count > m_nBrightCardSit)
        {
            m_PlayerList[m_nBrightCardSit].UpdateBrightCardTile(GetBrightCardIndex(m_nBrightCardSit), m_BrightCard);
        }
        return true;
    }

    //更新明牌的位置
    void UpdateBarightCardTile()
    {
        m_nBrightCardSit = m_nDealIndex / 100;
        m_nBrightCardIndex = m_nDealIndex % 100;
        m_nBrightCardSit += 1;
        if (m_nBrightCardSit > 3)
        {
            m_nBrightCardSit = 0;
        }
        DebugLog.Log("翻牌的方位 : " + m_nBrightCardSit + "翻牌的位置:" + m_nBrightCardIndex);
    }

    protected override void OnStartDie(byte dealerSit, byte point1, byte point2, bool midEnt = false)
    {
        if (point1 == 0 || point2 == 0)
            return;
        int beginSit = (m_dictSitPlayer[dealerSit] + point1 + point2 + 3) % 4;
        int index = (point1 + point2) * 2;
        m_nDealIndex = beginSit * 100 + index;
        Desk.OnPlayDie(new byte[] { point1, point2 }, midEnt);
        ThrowDice(dealerSit, midEnt);
        m_nZhuangJiaSit = dealerSit;
        UpdateBarightCardTile();
        if (midEnt)
        {
            if (m_nBrightCardSit != -1 && m_PlayerList.Count > m_nBrightCardSit)
            {
                m_PlayerList[m_nBrightCardSit].UpdateBrightCardTile(GetBrightCardIndex(m_nBrightCardSit), m_BrightCard);
            }
            m_nTotalTilesNum -= 1;//牌张总数扣除明牌数。
        }
    }

    protected override bool HandleBackOtherDiscard(uint _msgType, UMessage _ms)
    {
        byte sit = _ms.ReadByte();
        byte tile = _ms.ReadByte();
        byte doing = _ms.ReadByte();
        uint sign = _ms.ReadUInt();

        if (!m_dictSitPlayer.ContainsKey(sit))
            return false;

        //吃
        List<byte> valueList = new List<byte>();
        byte num = _ms.ReadByte();//几种吃法
        //DebugLog.Log("吃的数量:" + num);
        for (int index = 0; index < num; ++index)
        {
            valueList.Add(_ms.ReadByte());
            //DebugLog.Log("吃的牌:" + valueList[index]);
        }

        if (GameKind.HasFlag((int)MjOtherDoing_Enum.MjOtherDoing_Chi, doing))//吃
        {
            CommonUserData cud = m_GameButton[MjOtherDoing_Enum.MjOtherDoing_Chi].gameObject.GetComponent<CommonUserData>();
            cud.userData = valueList;
        }

        if (_msgType == 0)
            m_PlayerList[m_dictSitPlayer[sit]].UpdateTiles(1);
        else
            m_PlayerList[m_dictSitPlayer[sit]].OnDiscardTile(tile);

        ShowGameButton(doing, sign, new List<byte> { tile });

        return true;
    }

    public override void ResetGameUI()
    {
        base.ResetGameUI();
        ChooseChiTransform.gameObject.SetActive(false);
    }

    //获得杠和吃类型
    protected override bool GetOtherDoingValue(ref byte dongValue, MjOtherDoing_Enum doing, byte chooseIndex)
    {
        if (doing == MjOtherDoing_Enum.MjOtherDoing_Chi)
        {
            CommonUserData cud = m_GameButton[doing].gameObject.GetComponent<CommonUserData>();
            if (cud == null || cud.userData == null)
                return false;

            List<byte> userDataList = (List<byte>)cud.userData;
            if (userDataList.Count == 0)
                return false;

            if (chooseIndex != RoomInfo.NoSit)
            {
                dongValue = userDataList[chooseIndex];
                ChooseChiTransform.gameObject.SetActive(false);
            }
            else if (userDataList.Count == 1)
                dongValue = userDataList[0];
            else
            {
                if (MahjongAssetBundle == null)
                    return false;

                ChooseChiTransform.gameObject.SetActive(true);
                Transform tfm;
                for (int index = 0; index < ChooseChiTransform.childCount; index++)
                {
                    if (index >= userDataList.Count)
                    {
                        ChooseChiTransform.GetChild(index).gameObject.SetActive(false);
                        continue;
                    }
                    tfm = ChooseChiTransform.GetChild(index);
                    tfm.gameObject.SetActive(true);
                    for (int childIndex = 0; childIndex < tfm.childCount; ++childIndex)
                    {
                        tfm.GetChild(childIndex).GetComponent<Image>().sprite =
                            MahjongAssetBundle.LoadAsset<Sprite>("mahjong_" + (userDataList[index] + childIndex).ToString("X2"));
                    }
                }
                return false;
            }
        }
        else if (doing == MjOtherDoing_Enum.MjOtherDoing_Gang)
        {
            return base.GetOtherDoingValue(ref dongValue, doing, chooseIndex);
        }
        else if (doing == MjOtherDoing_Enum.MjOtherDoing_Init)
        {
            if (ChooseChiTransform.gameObject.activeSelf)
            {
                ChooseChiTransform.gameObject.SetActive(false);
            }
        }
        return true;
    }

    //创建碰.吃.杠牌型的挂接对象
    private Transform CreateTileGameObject(string name)
    {
        Transform parent = m_DiscardTfm.Find(name);
        if (parent == null)
        {
            parent = new GameObject(name).transform;
            parent.SetParent(m_DiscardTfm, false);
        }
        return parent;
    }

    //创建碰.吃.杠牌型
    public override Transform CreatePongOrKong(byte value, PongKongType type, Transform target, MjTileDirection_Enum direction = MjTileDirection_Enum.TileDirection_Front, byte firstValue = 0)
    {
        string pathName = "majiang_4ming";
        switch (direction)
        {
            case MjTileDirection_Enum.TileDirection_Right: //右边
                pathName = "majiang_4ming_2";
                break;
            case MjTileDirection_Enum.TileDirection_Left: //左边
                pathName = "majiang_4ming_1";
                break;
            case MjTileDirection_Enum.TileDirection_Front://对面
                pathName = "majiang_4ming";
                break;
            default:
                pathName = "majiang_4an";
                break;
        }

        string name = value.ToString("X2");
        if (type == PongKongType.ePKT_Chi)
        {
            name = "chi";
        }

        Transform parent = CreateTileGameObject(name);

        GameObject obj = (GameObject)MahjongAssetBundle.LoadAsset(pathName);
        obj = GameMain.Instantiate(obj, target.position, target.rotation);
        Transform tfm = obj.transform;
        tfm.SetParent(parent, true);
        tfm.name = type == PongKongType.ePKT_Chi ? "chi" + parent.childCount.ToString() : "pong";

        int num = 4;
        if (type == PongKongType.ePKT_Pong || type == PongKongType.ePKT_Chi)
        {
            num = 3;
            tfm.GetChild(num).gameObject.SetActive(false);
        }

        num -= 1;
        Transform chiTileTransform = null;
        List<byte> titleValueList = null;
        if (type == PongKongType.ePKT_Chi)
        {
            chiTileTransform = tfm.GetChild(num);
            chiTileTransform.name = value.ToString("X2");
            SetTileMat(chiTileTransform, value);
            byte tempValue = firstValue;
            titleValueList = new List<byte>();
            for (; tempValue <= firstValue + num; ++tempValue)
            {
                if (value != tempValue)
                {
                    titleValueList.Add(tempValue);
                }
            }
            num -= 1;
        }

        for (int index = num, indexValue = 0; index >= 0; --index, ++indexValue)
        {
            chiTileTransform = tfm.GetChild(index);
            if (type == PongKongType.ePKT_Chi)
            {
                value = titleValueList[indexValue];
                chiTileTransform.name = value.ToString("X2");
            }
            SetTileMat(chiTileTransform, value);
        }

        return tfm;
    }

    //初始化胡牌倍数
    protected override void InitChangeCoinTypeData(UMessage _ms, bool midEnt = false)
    {
        m_ProInfo.m_iMaxPro = _ms.ReaduShort(); //最大倍数
        m_nMinHuRule = (MinHuRule)(_ms.ReadByte());//起胡
        m_nDiHua = _ms.ReadByte();   //底花
        AddPrice = _ms.ReadByte();   //流局

        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DaDiaoChe] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_4YuanZiPro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_TaoHuaQiPro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QuanDaYuanPro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DaDaYuanPro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuiDui] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QingYiSe] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_HunYiSePro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_LenMenPro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_GangShangKaiHua] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_TianHu] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_CaBeiPro] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_Bao3Kou] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_Bao4Kou] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_HuaKai] = _ms.ReadByte();
        if (midEnt)
        {
            //明牌
            byte brightCard = _ms.ReadByte();
            //原子
            byte atomicCard = _ms.ReadByte();
            UpdateBrightCardOrAtomicCard(brightCard, atomicCard);
        }
    }

    //获得查听胡牌类型显示界面路径
    protected override string GetTingUIPath()
    {
        return base.GetTingUIPath() + "/Viewport/Content";
    }

    //更新明牌和原子牌
    void UpdateBrightCardOrAtomicCard(byte brightCard, byte atomicCard)
    {
        m_BrightCard = brightCard; //明牌
        m_AtomicCard = atomicCard; //原子
        if(m_AtomicCard == 0)
        {
            m_AtomicCard = 0xff;
        }

        DebugLog.Log("明牌 :" + m_BrightCard + " 原子:" + m_AtomicCard);
        if (MainUITfm != null && MahjongAssetBundle != null)
        {
            Transform ImageTransform = MainUITfm.Find("Top/cai/majiang_1/ImageIcon");
            ImageTransform.GetComponent<Image>().sprite = MahjongAssetBundle.LoadAsset<Sprite>("mahjong_" + m_BrightCard.ToString("X2"));
            ImageTransform = MainUITfm.Find("Top/cai/majiang_2/ImageIcon");
            ImageTransform.GetComponent<Image>().sprite = MahjongAssetBundle.LoadAsset<Sprite>("mahjong_" + m_AtomicCard.ToString("X2"));
            MainUITfm.Find("Top/cai").gameObject.SetActive(true);
        }

        OnTileNumChanged(1);
        OnLeftTileChanged(m_BrightCard, 1);
    }

    //断线重连同步数据
    protected override MjTileMiddleEnterData_Enum MiddleEnterRoomRoleLocal(UMessage _ms, ref List<byte> listPong, ref List<byte> listKong, ref List<byte> listKongSelf,
                                                                           ref List<byte> listDiscard, ref List<byte> listHua, ref List<byte> listChi, ref List<byte> listKong2)
    {
        //吃牌
        byte index = 0;
        byte listDataNum = _ms.ReadByte();
        listChi.Clear();
        for (index = 0; index < listDataNum; ++index)
        {
            listChi.Add(_ms.ReadByte()); //吃的牌 
            listChi.Add(_ms.ReadByte()); //吃的首牌
        }
        //碰牌
        index = 0;
        listPong.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listPong.Add(_ms.ReadByte());//喂吃碰的玩家座位号
            listPong.Add(_ms.ReadByte());//碰的牌
        }

        //明杠
        index = 0;
        listKong.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listKong.Add(_ms.ReadByte());
        }

        //暗杠
        index = 0;
        listKongSelf.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listKongSelf.Add(_ms.ReadByte());
        }

        //补杠
        index = 0;
        listKong2.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listKong2.Add(_ms.ReadByte());//喂吃碰的玩家座位号
            listKong2.Add(_ms.ReadByte());//杠的牌
        }

        //打出去的牌
        index = 0;
        listDiscard.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listDiscard.Add(_ms.ReadByte());
        }

        //花
        index = 0;
        listHua.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listHua.Add(_ms.ReadByte());
        }

        return MjTileMiddleEnterData_Enum.MiddleEnterData_Cz;
    }

    /// <summary>
    /// 更新约据房间信息
    /// </summary>
    /// <param name="Data"></param>
    protected override void UpdateAppointmentData(AppointmentData Data, UMessage _ms)
    {
        CzMahjongAppointmentData mahjongData = (CzMahjongAppointmentData)Data;
        mahjongData.wanFa = _ms.ReadShort();
        mahjongData.diHua = m_nDiHua;
        mahjongData.qiHu = (byte)m_nMinHuRule;
    }

    protected override void OnLeaveLookRoom(GameCity.EMSG_ENUM GameCityMsg)
    {
        base.OnLeaveLookRoom(GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_CM_LEAVEONLOOKERROOM);
    }

    /// <summary>
    /// 显示查听界面
    /// </summary>
    public override void ShowTingUI(bool bShow, Dictionary<byte, ushort> tileHu = null)
    {
        if (tileHu != null && TingTfm)
        {
            RectTransform ImageBgRectTransform = TingTfm.Find("Mask/ImageBG").GetComponent<RectTransform>();
            if (ImageBgRectTransform && tileHu != null)
            {
                ImageBgRectTransform.sizeDelta = new Vector2((tileHu.Count > 10 ? 10 : tileHu.Count) * 80 + 150, tileHu.Count > 10 ? 275 : 155);
            }
        }
        base.ShowTingUI(bShow, tileHu);
    }
}