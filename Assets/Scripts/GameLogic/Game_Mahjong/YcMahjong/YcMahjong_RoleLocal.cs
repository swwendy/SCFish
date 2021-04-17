using DG.Tweening;using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class YcMahjong_RoleLocal : Mahjong_RoleLocal{
    List<byte> m_HuaList = new List<byte>();
    bool m_bHeizi = false;
    bool m_bQiShouTing = false;

    public YcMahjong_RoleLocal(CGame_Mahjong game, byte index)        : base(game, index)    {    }

    public override void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_SM_BACKDEALMJPOKER, HandleBackDiscard);    }

    public override void OnEnd()
    {
        base.OnEnd();

        m_HuaList.Clear();
        m_bHeizi = false;
        m_bQiShouTing = false; 
    }

    /*public override void OnTick()
    {
        base.OnTick();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameBase.m_ProInfo.m_iMaxPro = 2048;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuiDui] = 20;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QiDui] = 20;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QingYiSe] = 40;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_JinGouDiao] = 1;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_FourSame] = 1;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_Gang_Ming] = 1;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DaFeiJiPro] = 4;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_TingPaiDianPaoPro] = 2;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_FeiTingZiMoPro] = 3;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_TianHu] = 3;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_JiaJiaJu] = 2;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuanMen] = 10;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_YiTiaoLong] = 10;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DengZi] = 10;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_HeiZi] = 36;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QiShouJiaoTing] = 10;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_YaDang] = 10;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuiDao] = 10;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DaDiaoChe] = 10;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_JiangDui] = 1;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_HasYaoJiu] = 1;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_NoYaoJiu] = 1;
            GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_MenQing] = 10;

            m_PKDList.Clear();
            for (int i = 0; i < 1; i++)
            {                PongKongData pkd = new PongKongData();
                pkd.pkt = PongKongType.ePKT_Kong_Concealed;
                pkd.value = 0x26;
                m_PKDList.Add(pkd);
            }            m_HuaList.Clear();
            for (int i = 0; i < 9; i++)
                m_HuaList.Add(0);

            List<byte> HaveTiles = new List<byte> { 0x14, 0x14, 0x16, 0x17, 0x19, 0x19, 0x19, 0x24, 0x24, 0x24, 0x15 };
            GetTileWinPro(HaveTiles);
        }
    }*/

    public override bool OnAfterBuhua(List<byte> listHua, List<byte> addList, bool midEnt, float time)
    {
        if(!base.OnAfterBuhua(listHua, addList, midEnt, time))
            return false;

        if(time < 0f)
        {
            foreach (byte tile in listHua)
                m_HuaList.Remove(tile);
        }
        else
            m_HuaList.AddRange(listHua);
        return true;
    }
    public override void DiscardTile(Mahjong_Tile tile, byte disValue)
    {
        SendDiscardTileMsg(tile, disValue, GameCity.EMSG_ENUM.CCMsg_YCMAHJONG_CM_PLAYERDEALMJPOKER);
    }    public override bool CheckTing(bool bForce, byte suitCount = 3, byte atomicCardValue = 0,bool check7pair = true)
    {
        bool bTing = base.CheckTing(bForce, suitCount, atomicCardValue, check7pair);

        if (bTing && TingState == MjTingState_Enum.MjTingState_Init)
        {
            int selfDoing = 0;
            GameKind.AddFlag((int)MjOtherDoing_Enum.MjOtherDoing_Ting, ref selfDoing);
            GameKind.AddFlag((int)MjOtherDoing_Enum.MjOtherDoing_FeiTing, ref selfDoing);
            GameBase.ShowGameButton((byte)selfDoing);
        }
        return bTing;
    }    public override void OnTing(MjOtherDoing_Enum doing)
    {
        base.OnTing(doing);

        foreach (Transform tfm in m_HandTfm)
        {
            if (!tfm.gameObject.activeSelf)
                continue;

            Mahjong_Tile mt = tfm.GetComponent<Mahjong_Tile>();
            mt.Enable = mt.Tiped();
            mt.Lack = !mt.Enable;
        }

        m_bHeizi = (m_HuaList.Count == 0);
        m_bQiShouTing = (m_nDiscardIndex == 0);

        CheckTing(true);
    }    public override void OnTingEnd(bool bReverse)
    {
        base.OnTingEnd(bReverse);

        foreach (Transform tfm in m_HandTfm)
        {
            if (!tfm.gameObject.activeSelf)
                continue;

            Mahjong_Tile mt = tfm.GetComponent<Mahjong_Tile>();
            mt.Enable = true;
            mt.Lack = false;
        }
    }

    public override bool IsTing()
    {
        if (!base.IsTing())
            return false;

        return  TingState >= MjTingState_Enum.MjTingState_Ting;
    }
    public override ushort GetTileWinPro(List<byte> haveTiles)
    {
        if (haveTiles.Count % 3 != 2)
            return 0;

        HashSet<byte> tileHu;
        int hand4Count = JudgeTile.CheckSevenPair(haveTiles, false, out tileHu);
        //type:1：普通 2：七对 
        int type = (hand4Count < 0) ? 1 : 2;
        List<byte> suits = new List<byte> { 0, 1, 2 };

        ushort pro = (ushort)m_HuaList.Count;
        if (type == 1)
        {
            if (JudgeTile.CheckDuiDuiHu(haveTiles, m_PKDList))
                pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuiDui];

            if(CheckYiTiaoLong(haveTiles, suits))
                pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_YiTiaoLong];
        }
        else
        {
            pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QiDui];
            if (hand4Count > 0)
            {
                if (TingState == MjTingState_Enum.MjTingState_WaitFeiTing || TingState == MjTingState_Enum.MjTingState_FeiTing)
                {
                    byte huTile = haveTiles[haveTiles.Count - 1];
                    List<byte> list = haveTiles.FindAll(s => s == huTile);
                    if (list.Count == 4)                        pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_FourSame];
                }            }        }

        pro += GetYaoJiuHuaNum(haveTiles, type);

        pro += (ushort)(GetDengZiNum() * GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DengZi]);

        if (m_bHeizi)
            pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_HeiZi];

        if(m_bQiShouTing)
            pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QiShouJiaoTing];

        if (CheckMenQing())
            pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_MenQing];

        if (JudgeTile.CheckQingYiSe(haveTiles, m_PKDList))
            pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QingYiSe];
        else if (CheckDuanMen(haveTiles, m_PKDList, suits))
            pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuanMen];

        if (TingState == MjTingState_Enum.MjTingState_WaitFeiTing || TingState == MjTingState_Enum.MjTingState_FeiTing)
        {
            if (CheckDanDiao(haveTiles, suits))
                pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DaDiaoChe];
            else if(CheckKaZhang(haveTiles, suits))
                pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_YaDang];
            else if(CheckDuiDao(haveTiles, suits))
                pro += GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_DuiDao];
        }

        if(((CGame_YcMahjong)GameBase).AddPrice > 0)
            pro *= GameBase.m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_JiaJiaJu];

        if (pro > GameBase.m_ProInfo.m_iMaxPro)
            pro = GameBase.m_ProInfo.m_iMaxPro;

        return pro;
    }

    bool CheckMenQing()
    {
        return true;
    }

    //type 0:检测 1：普通 2：七对 
    ushort GetYaoJiuHuaNum(List<byte> haveTiles, int type)
    {
        ushort num = 0;
        List<byte> suits = new List<byte> { 0, 1, 2 };
        var group = haveTiles.GroupBy(p => p).Where(g => (JudgeTile.GetTileValue(g.Key) == 1 
                                                           || JudgeTile.GetTileValue(g.Key) == 9)
                                                           && g.Count() >= 2);

        if(type == 0)
        {
            HashSet<byte> tileHu;
            int hand4Count = JudgeTile.CheckSevenPair(haveTiles, false, out tileHu);
            type = (hand4Count < 0) ? 1 : 2;        }
        if (type == 1)
        {
            foreach (var g in group)
            {
                byte tile = g.Key;
                List<byte> testTiles;
                if (g.Count() > 2)//检测是不是刻子
                {
                    testTiles = new List<byte>(haveTiles);
                    for (int i = 0; i < 3; i++)
                        testTiles.Remove(tile);
                    if (JudgeTile.CheckNormalHu(testTiles, suits))
                    {
                        num += 3;
                        continue;
                    }
                }

                //检测是不是将
                testTiles = new List<byte>(haveTiles);
                for (int i = 0; i < 2; i++)
                {
                    testTiles.Remove(tile);
                }

                if (JudgeTile.CheckNormalHu(testTiles, suits, 0, false))
                    num += 2;
            }
        }
        else
        {
            foreach (var g in group)
            {
                num += (ushort)g.Count();
            }
        }

        return num;
    }    int GetDengZiNum()
    {
        int num = JudgeTile.GetGangNum(m_PKDList);

        //4张相同的字
        var group = m_HuaList.GroupBy(p => p).Where(g => g.Count() == 4);
        num += group.Count();

        //春夏秋冬
        List<byte> list = m_HuaList.FindAll(s => s >= 0x41 && s <= 0x44);
        if (list.Count == 4)
            num += 1;

        list = m_HuaList.FindAll(s => s >= 0x45 && s <= 0x48);
        if (list.Count == 4)
            num += 1;

        return num;
    }

     bool CheckYiTiaoLong(List<byte> handTiles, List<byte> suits)
    {
        foreach (byte i in suits)
        {
            List<byte> list = handTiles.FindAll(s => JudgeTile.GetTileSuit(s) == i);
            list = list.Distinct().ToList();//去重
            if (list.Count == 9)
            {                List<byte> testTiles = new List<byte>(handTiles);
                foreach (byte t in list)
                    testTiles.Remove(t);
                if (JudgeTile.CheckNormalHu(testTiles, suits))                    return true;
            }        }

        return false;
    }
    bool CheckDuanMen(List<byte> handTiles, List<PongKongData> pkd, List<byte> suits)
    {
        List<byte> tiles = new List<byte>(handTiles);
        if (pkd != null)
        {
            foreach (PongKongData d in pkd)
            {
                tiles.Add(d.value);
            }
        }

        foreach (byte suit in suits)
        {
            List<byte> list = tiles.FindAll(s => JudgeTile.GetTileSuit(s) == suit);
            if (list.Count == 0)
                return true;        }

        return false;
    }

    bool CheckDuiDao(List<byte> handTiles, List<byte> suits)
    {
        byte huTile = handTiles[handTiles.Count - 1];
        List<byte> list = handTiles.FindAll(s => s == huTile);
        if (list.Count < 3)
            return false;

        List<byte> testTiles = new List<byte>(handTiles);
        for (int i = 0; i < 3; i++)
            testTiles.Remove(huTile);
        if (JudgeTile.CheckNormalHu(testTiles, suits))
            return true;

        return false;
    }

    bool CheckDanDiao(List<byte> handTiles, List<byte> suits)
    {
        byte huTile = handTiles[handTiles.Count - 1];
        List<byte> list = handTiles.FindAll(s => s == huTile);
        if (list.Count < 2)
            return false;

        List<byte> testTiles = new List<byte>(handTiles);
        for (int i = 0; i < 2; i++)
        {            testTiles.Remove(huTile);
        }

        if (JudgeTile.CheckNormalHu(testTiles, suits, 0, false))
            return true;
        return false;
    }

    bool CheckKaZhang(List<byte> handTiles, List<byte> suits)
    {
        byte huTile = handTiles[handTiles.Count - 1];
        int value = JudgeTile.GetTileValue(huTile);

        if (value == 1 || value == 9)
            return false;

        List<byte> testTiles = new List<byte>(handTiles);
        if (testTiles.Remove((byte)(huTile - 1)) && testTiles.Remove((byte)(huTile + 1)))
        {
            testTiles.Remove(huTile);
            if (JudgeTile.CheckNormalHu(testTiles, suits))
                return true;
        }

        if (value == 3)//123
        {
            testTiles = new List<byte>(handTiles);
            if (testTiles.Remove((byte)(huTile - 1)) && testTiles.Remove((byte)(huTile - 2)))
            {
                testTiles.Remove(huTile);
                if (JudgeTile.CheckNormalHu(testTiles, suits))
                    return true;
            }
        }

        if (value == 7)//789
        {
            testTiles = new List<byte>(handTiles);
            if (testTiles.Remove((byte)(huTile + 1)) && testTiles.Remove((byte)(huTile + 2)))
            {
                testTiles.Remove(huTile);
                if (JudgeTile.CheckNormalHu(testTiles, suits))
                    return true;
            }
        }

        return false;
    }
}