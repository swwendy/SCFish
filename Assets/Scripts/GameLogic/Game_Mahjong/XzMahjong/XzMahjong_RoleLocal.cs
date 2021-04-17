using DG.Tweening;using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class XzMahjong_RoleLocal : Mahjong_RoleLocal{
    List<byte> m_PreChangeList = new List<byte>();    List<byte> m_SelChangeList = new List<byte>();
    public XzMahjong_RoleLocal(CGame_Mahjong game, byte index)        : base(game, index)    {    }

    public override void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_BACKDEALMJPOKER, HandleBackDiscard);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_TRUSTCHANGEPOKERS, HandleSystemChange);    }

    public override void OnEnd()
    {
        base.OnEnd();

        m_PreChangeList.Clear();
        m_SelChangeList.Clear();
    }

    protected override void OnHitTile(Mahjong_Tile tile)
    {
        if (tile == null || GameBase.GameState != MahjongRoomState_Enum.MjRoomState_WaitChangeMj)
        {            base.OnHitTile(tile);
            m_SelChangeList.Clear();
            return;
        }

        int suit = JudgeTile.GetTileSuit(tile.Value);
        List<byte> list = m_HaveTiles.FindAll(s => JudgeTile.GetTileSuit(s) == suit);
        if (list.Count < GameBase.ChangeTileNum)
            return;

        if (list.Count == GameBase.ChangeTileNum)
        {
            m_SelChangeList.Clear();
            Transform tfm;
            for (byte i = 0; i < m_HandTfm.childCount; i++)
            {
                tfm = m_HandTfm.GetChild(i);
                if (!tfm.gameObject.activeSelf)
                    continue;

                Mahjong_Tile mt = tfm.GetComponent<Mahjong_Tile>();
                bool bSel = (JudgeTile.GetTileSuit(mt.Value) == suit);
                mt.OnSelect(bSel);

                if (mt.m_bSelected)
                    m_SelChangeList.Add(mt.Value);
            }
        }
        else if (list.Count > GameBase.ChangeTileNum)
        {
            if (m_SelChangeList.Count > 0)
            {
                if (JudgeTile.GetTileSuit(m_SelChangeList[0]) != JudgeTile.GetTileSuit(tile.Value))
                {
                    DownAllTiles(false);
                    m_SelChangeList.Clear();
                }
            }
            tile.OnSelect(true);

            if (tile.m_bSelected)
                m_SelChangeList.Add(tile.Value);
            else
                m_SelChangeList.Remove(tile.Value);
        }

        ((CGame_XzMahjong)GameBase).ChangeBtnInactive = (m_SelChangeList.Count == GameBase.ChangeTileNum);
    }

    public void OnRequestChange(bool bPlayer)
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_CHANGEPOKERS);
        msg.Add(GameMain.hall_.GetPlayerId());
        List<byte> list;
        if (bPlayer)
            list = m_SelChangeList;
        else
        {            list = m_PreChangeList;
            m_SelChangeList = list;
        }        msg.Add((byte)list.Count);
        foreach (byte value in list)
        {
            msg.Add(value);
        }
        HallMain.SendMsgToRoomSer(msg);    }

    public override void DiscardTile(Mahjong_Tile tile, byte disValue)
    {
        SendDiscardTileMsg(tile, disValue, GameCity.EMSG_ENUM.CCMsg_MAHJONG_CM_PLAYERDEALMJPOKER);
    }

    public void ChooseChangeTiles()
    {
        List<byte> result = GetRecommondTile(GameBase.ChangeTileNum);
        GameObject tileObj;
        Mahjong_Tile tile;
        m_PreChangeList.Clear();
        for (int i = m_HandTfm.childCount - 1, j = 0; i >= 0; i--)
        {
            tileObj = m_HandTfm.GetChild(i).gameObject;
            if (!tileObj.activeSelf)
                continue;

            tile = tileObj.GetComponent<Mahjong_Tile>();
            tile.OnSelect(false, false);

            if (j < result.Count && tile.Value == result[j])
            {
                m_PreChangeList.Add(result[j]);
                tile.OnSelect(true);
                j++;
            }
        }
        m_SelChangeList = new List<byte>(m_PreChangeList);
    }

    public override void ShowChange(bool show, bool midEnt, List<byte> list = null)
    {
        base.ShowChange(show, midEnt, list);

        if (show)
        {
            if (list == null)
            {                if (!midEnt)
                {
                    foreach (byte value in m_SelChangeList)
                    {
                        m_HaveTiles.Remove(value);
                        GameBase.OnLeftTileChanged(value, 1, false);
                    }

                    ReArrayTiles(1 + m_SelChangeList.Count / 2);
                }
                else
                    GameBase.OnTileNumChanged(3);
            }
            m_PreChangeList.Clear();
            m_SelChangeList.Clear();
            m_bCanHit = false;
            ((CGame_XzMahjong)GameBase).OnLocalChanged(true);
        }
        else if (midEnt)
            ((CGame_XzMahjong)GameBase).OnLocalChanged(false);
    }

    bool HandleSystemChange(uint _msgType, UMessage _ms)
    {
        m_SelChangeList.Clear();
        byte num = _ms.ReadByte();
        for (byte i = 0; i < num; i++)
            m_SelChangeList.Add(_ms.ReadByte());
        ShowChange(true, false);
        ((CGame_XzMahjong)GameBase).OnLocalChanged(true);
        return true;
    }

    public override ushort GetTileWinPro(List<byte> haveTiles)
    {
        if (haveTiles.Count % 3 != 2)
            return 0;

        HashSet<byte> tileHu;
        int hand4Count = JudgeTile.CheckSevenPair(haveTiles, false, out tileHu);
        //type:1：普通 2：七对 
        int type = (hand4Count < 0) ? 1 : 2;

        ushort pro = 1;
        if (type == 1)
        {
            bool reslutState = JudgeTile.CheckDuiDuiHu(haveTiles, m_PKDList);
            if (reslutState)
                pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuiDui];
            if (reslutState && CheckJiangDui(haveTiles, m_PKDList))
                pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_JiangDui];
            else if (CheckNo19(haveTiles, m_PKDList))
                pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_NoYaoJiu];

            if (JudgeTile.CheckJinGouDiao(haveTiles, m_PKDList))
                pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_JinGouDiao];

            int res = JudgeTile.GetGangNum(m_PKDList);
            for (int i = 0; i < res; i++)
                pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_Gang_Ming];

            if (CheckMenQing())
                pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_MenQing];
        }
        else
        {
            pro = GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QiDui];
            if (hand4Count > 0)
                pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_FourSame];

            if (CheckNo19(haveTiles, m_PKDList))
                pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_NoYaoJiu];
        }

        if (JudgeTile.CheckQingYiSe(haveTiles, m_PKDList))
            pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QingYiSe];

        if (CheckHas19(haveTiles, m_PKDList, type))
            pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_HasYaoJiu];

        if (pro > GameBase.m_ProInfo.m_iMaxPro)
            pro = GameBase.m_ProInfo.m_iMaxPro;

        return pro;
    }

    bool CheckMenQing()
    {
        return m_PKDList == null || m_PKDList.Count == 0;
    }

    //type: 1：普通 2：七对 
    bool CheckHas19(List<byte> handTiles, List<PongKongData> pkd, int type)
    {
        int value;
        if (type == 1)
        {
            //首先传进来的牌肯定是可以胡的牌，如果不是123或789肯定不是
            List<byte> list = handTiles.FindAll(s => JudgeTile.GetTileValue(s) > 3 && JudgeTile.GetTileValue(s) < 7);
            if (list.Count > 0)
                return false;

            List<byte> list19 = handTiles.FindAll(s => JudgeTile.GetTileValue(s) == 1 || JudgeTile.GetTileValue(s) == 9);
            if (list19.Count < 2)
                return false;
            int noEyeCount = list19.Count - 2;//去除将牌2张
            int meldCount = handTiles.Count / 3;//多少坎
            if (noEyeCount < meldCount)
                return false;
            List<byte> list1 = new List<byte> { 0x02, 0x12, 0x22, 0x07, 0x17, 0x27 };
            List<byte> list2 = new List<byte> { 0x03, 0x13, 0x23, 0x08, 0x18, 0x28 };
            int no19Meld = 0, count;
            for (int i = 0; i < list1.Count; i++)
            {
                count = handTiles.FindAll(s => s == list1[i]).Count;
                if (count != handTiles.FindAll(s => s == list2[i]).Count)
                    return false;
                no19Meld += count;
            }
            noEyeCount -= no19Meld;
            if (noEyeCount % 3 != 0)
                return false;
            noEyeCount /= 3;
            if (no19Meld + noEyeCount != meldCount)
                return false;
        }
        else if (type == 2)
        {
            foreach (byte tile in handTiles)
            {
                value = JudgeTile.GetTileValue(tile);
                if (value != 1 && value != 9)
                    return false;
            }        }        else            return false;
        if (pkd == null)
            return true;

        foreach (PongKongData d in pkd)
        {
            value = JudgeTile.GetTileValue(d.value);
            if (value != 1 && value != 9)
                return false;
        }
        return true;
    }

    bool CheckNo19(List<byte> handTiles, List<PongKongData> pkd)
    {
        int value;
        foreach (byte tile in handTiles)
        {
            value = JudgeTile.GetTileValue(tile);
            if (value == 1 || value == 9)
                return false;
        }

        if (pkd == null)
            return true;

        foreach (PongKongData d in pkd)
        {
            value = JudgeTile.GetTileValue(d.value);
            if (value == 1 || value == 9)
                return false;
        }

        return true;
    }    //将对    bool CheckJiangDui(List<byte> handTiles, List<PongKongData> pkd)
    {
        int value;
        foreach (byte tile in handTiles)
        {
            value = JudgeTile.GetTileValue(tile);
            if (value != 2 && value != 5 && value != 8)
                return false;
        }

        foreach (PongKongData d in pkd)
        {
            value = JudgeTile.GetTileValue(d.value);
            if (value != 2 && value != 5 && value != 8)
                return false;
        }
        return true;
    }}