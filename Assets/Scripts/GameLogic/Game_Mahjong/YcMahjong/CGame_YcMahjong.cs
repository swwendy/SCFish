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
public class CGame_YcMahjong : CGame_Mahjong
{
    Text m_AddPriceText;
    int m_nAddPrice = -1;

    public int AddPrice
    {
        get
        {
            return m_nAddPrice;
        }

        set
        {
            m_nAddPrice = value;
            if(m_AddPriceText != null)
            {
                m_AddPriceText.transform.parent.gameObject.SetActive(m_nAddPrice >= 0);
                m_AddPriceText.text = m_nAddPrice.ToString();
            }
        }
    }

    public CGame_YcMahjong(GameTye_Enum gameType, GameKind_Enum gameKind) : base(gameType, gameKind)
    {
    }

    public override void InitPlayers()
    {
        Mahjong_Role role;
        role = new YcMahjong_RoleLocal(this, 1);
        role.Init();        m_PlayerList.Add(role);

        for (byte i = 2; i < 5; i++)
        {
            role = new Mahjong_RoleOther(this, i);
            role.Init();            m_PlayerList.Add(role);
        }
    }

    public override void LoadMainUI(Transform root)
    {
        m_nTotalTilesNum = 144;
        m_nMaxTileValue = 0x50;

        GameObject obj;
        byte index = 0;
        Button[] buttons;
        Transform tfm;

        obj = (GameObject)MahjongAssetBundle.LoadAsset("YC_Mahjong_Game");
        MainUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        MainUITfm.SetParent(root, false);
        MainUITfm.gameObject.SetActive(false);

        m_AddPriceText = MainUITfm.Find("Top/jia/TextNum").GetComponent<Text>();
        AddPrice = -1;

        tfm = MainUITfm.Find("Pop-up");
        m_TrustButton = tfm.Find("button_tuoguan/Button_guanbi").GetComponent<Button>();
        m_TrustButton.onClick.AddListener(() => OnClickTrustBtn(GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_CM_CANCLETRUSTSTATE));

        ChooseKongTfm = tfm.Find("choose/choose_gang");
        buttons = ChooseKongTfm.GetComponentsInChildren<Button>();
        index = 0;
        foreach (Button btn in buttons)
        {
            byte temp = index;
            btn.onClick.AddListener(() => OnClickGameBtn(MjOtherDoing_Enum.MjOtherDoing_Gang, temp, GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_CM_ANSWERDOING));
            index++;
        }

        tfm = tfm.Find("button_option");
        string[] btnName = new string[] { "Button_guo", "Button_peng", "Button_gang", "Button_he", "Button_Chi", "Button_ting", "Button_tingfei"};
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
            btn.onClick.AddListener(() => OnClickGameBtn(item, RoomInfo.NoSit, GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_CM_ANSWERDOING));
        }
        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameType, "Mahjong_AudioCsv");
    }

    public override void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_CHOOSElEVEL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_ENTERROOM, HandleEnterRoom);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_ROOMSTATE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_OTHERENTER, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_DEALMJBEGIN, HandleSendTile);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_UPDATEPOKERSAFTERBUHUA, HandleAfterBuhua);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_FIRSTASKBANKERDEAL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_UPDATEDEALPOKER, HandleBackOtherDiscard);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_GETPOKERASKDEAL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_ANSWERDOING, HandleAnswer);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_UPDATEDOING, HandleOtherAnswer);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_PLAYERHUPOKER, HandlePlayerWin);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_PUBLISHRESULT, HandleResult);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_BACKLEAVE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_ENTERTRUSTSTATE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_MIDDLEENTERROOM, HandleMiddleEnterRoom);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER, HandleGameNetMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_AFTERONLOOKERENTER, HandleBystanderEnter);    }

    public override bool HandleGameNetMsg(uint _msgType, UMessage _ms)
    {
        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;
        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_CHOOSElEVEL:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_CHOOSElEVEL, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_ROOMSTATE:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ROOMSTATE, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_OTHERENTER:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_OTHERENTER, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_FIRSTASKBANKERDEAL:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_FIRSTASKBANKERDEAL, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_GETPOKERASKDEAL:
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
                    if(num > 0)
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

            case GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_BACKLEAVE:
                {
                    base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_BACKLEAVE, _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_ENTERTRUSTSTATE:
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

            if (cSit == 0 )
                ((Mahjong_RoleLocal)role).CheckTing(true);
        }

        role.SetCanHit(true);
    }

    public override void OnEnterState(MahjongRoomState_Enum state, UMessage _ms, byte mode, float timeLeft)
    {
        base.OnEnterState(state, _ms, mode, timeLeft);

        if(mode == 0)
        {
            if (state == MahjongRoomState_Enum.MjRoomState_ThrowDice)
            {
                AddPrice = _ms.ReadByte();
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        AddPrice = -1;
    }

    public override string GetBeiShuStr()
    {
        return "花";
    }

    protected override bool HandleMiddleEnterRoom(uint _msgType, UMessage _ms)
    {
        if(!base.HandleMiddleEnterRoom(_msgType, _ms))
        {
            return false;
        }
        AddPrice = _ms.ReadByte();
        return true;
    }

    protected override bool HandleBystanderEnter(uint _msgType, UMessage _ms)
    {
        base.HandleBystanderEnter(_msgType, _ms);

        AddPrice = _ms.ReadByte();

        return true;
    }

    protected override string GetMahJiongGameRule()
    {
        return "盐城麻将 ";
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
                        byte leftTiles = (byte)list[j++];
                        byte roleNum = (byte)list[j++];
                        byte huaNum = 0;
                        List<byte> huaList = new List<byte>();
                        List<byte> tiles = new List<byte>();
                        for (int i = 0; i < roleNum; i++)
                        {
                            tiles.Clear();
                            huaList.Clear();

                            byte sit = (byte)list[j++];
                            byte num = (byte)list[j++];
                            for (int k = 0; k < num; k++)
                                tiles.Add((byte)list[j++]);
                            num = (byte)list[j++];
                            for (int k = 0; k < num; k++)
                                huaList.Add((byte)list[j++]);

                            byte cSit = m_dictSitPlayer[sit];
                            List<byte> addList = new List<byte>(tiles);
                            foreach (byte tile in m_PlayerList[cSit].m_HaveTiles)
                                addList.Remove(tile);
                            m_PlayerList[cSit].OnAfterBuhua(huaList, addList);
                            m_PlayerList[cSit].m_HaveTiles = new List<byte>(tiles);
                            m_PlayerList[cSit].UpdateTiles();
                            huaNum += num;
                        }

                        HideWallTile(huaNum, false);
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
        YcMahjongAppointmentData mahjongData = (YcMahjongAppointmentData)Data;
        short palyType = _ms.ReadShort();
    }

    protected override void OnLeaveLookRoom(GameCity.EMSG_ENUM GameCityMsg)
    {
        base.OnLeaveLookRoom(GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_CM_LEAVEONLOOKERROOM);
    }
}