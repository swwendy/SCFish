using DG.Tweening;using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class CzMahjong_RoleLocal : Mahjong_RoleLocal{
    List<byte> m_HuaList = new List<byte>();
    bool m_bHeizi = false;
    bool m_bQiShouTing = false;
    public CzMahjong_RoleLocal(CGame_Mahjong game, byte index)        : base(game, index)    {
    }

    public override void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_SM_BACKDEALMJPOKER, HandleBackDiscard);    }

    public override void OnEnd()
    {
        base.OnEnd();

        m_HuaList.Clear();
        m_bHeizi = false;
        m_bQiShouTing = false;
    }

    public override bool OnAfterBuhua(List<byte> listHua, List<byte> addList, bool midEnt, float time)
    {
        if (!base.OnAfterBuhua(listHua, addList, midEnt, time))
            return false;

        if (time < 0f)
        {
            foreach (byte tile in listHua)
                m_HuaList.Remove(tile);
        }
        else
            m_HuaList.AddRange(listHua);        return true;
    }
    public override void DiscardTile(Mahjong_Tile tile, byte disValue)
    {
        SendDiscardTileMsg(tile, disValue, GameCity.EMSG_ENUM.CCMsg_CZMAHJONG_CM_PLAYERDEALMJPOKER);
    }

    //JudgeTile.CheckNormalHu 函数参数 hunNum 不为空的时候一定要把tiles中原子删除掉
    public override ushort GetTileWinPro(List<byte> haveTiles)
    {
        if (haveTiles.Count % 3 != 2)
            return 0;

        //花数 ,番数,硬一番,总倍数
        ushort huaNum = (ushort)m_HuaList.Count, fanNum = 0, yingYiFanNum = 0,totalNum = 1;

        int tileSuit = -1;//麻将类型
        bool bHuaState = false;
        foreach (PongKongData data in m_PKDList)
        {
            tileSuit = JudgeTile.GetTileSuit(data.value);
            bHuaState = false;
            if (data.pkt == PongKongType.ePKT_Kong_Exposed || data.pkt == PongKongType.ePKT_Pong2Kong) // 明杠或补杠
            {
                huaNum++;
                bHuaState = true;
            }
            else if(data.pkt == PongKongType.ePKT_Kong_Concealed) //暗杠
            {
                huaNum += 2;
                bHuaState = true;
            }
            else if (data.pkt == PongKongType.ePKT_Pong)  //碰
            {
                bHuaState = true;
                if (data.value == ((CGame_CzMahjong)GameBase).m_BrightCard)//名牌
                {
                    huaNum++;
                }
            }

            if(bHuaState && tileSuit == 3)
            {
                huaNum++;
            }
        }

        //原子牌
        ushort atomicCardCount = (ushort)haveTiles.FindAll(tile => tile == ((CGame_CzMahjong)GameBase).m_AtomicCard).Count;
        huaNum += atomicCardCount;

        //明牌:刻(无原子和有原子)和风牌(刻子)
        bool bIsFeng = false;
        List<byte> checkTileData = null;
        ushort useAtomicCardCount = 0,atomicCardNum = atomicCardCount;
        byte brightCardTile = ((CGame_CzMahjong)GameBase).m_BrightCard;
        List<byte> suits = new List<byte> { 0, 1, 2, 3 };
        List<byte> tiles = new List<byte>(haveTiles);
        List<byte> checkTileList = new List<byte>(JudgeTile.MahjongTiles);
        checkTileList = checkTileList.FindAll(tile => JudgeTile.GetTileSuit(tile) == 3);
        int findBrightCardIndex = checkTileList.FindIndex(tile => tile == brightCardTile);
        if (findBrightCardIndex != -1)
        {
            bIsFeng = true;
            checkTileList.RemoveAt(findBrightCardIndex);
        }
        checkTileList.Insert(0, brightCardTile);
        tiles.RemoveAll(value => value == ((CGame_CzMahjong)GameBase).m_AtomicCard);
        foreach (var tile in checkTileList)
        {
            useAtomicCardCount = atomicCardNum;
            checkTileData = tiles.FindAll(value => tile == value);
            if(checkTileData.Count == 0 || tile == ((CGame_CzMahjong)GameBase).m_AtomicCard || checkTileData.Count + atomicCardNum < 3)
            {
                continue;
            }

            checkTileData.Clear();
            checkTileData.AddRange(tiles);
            for (int index = 0; index < 3; ++index)
            {
                if (!checkTileData.Remove(tile))
                {
                    useAtomicCardCount--;
                }
            }

            if (JudgeTile.CheckNormalHu(checkTileData, suits, useAtomicCardCount))
            {
                huaNum++; //刻（有原子）

                if(tile == brightCardTile) //明牌
                {
                    if (useAtomicCardCount == 0)//刻（无原子）再计一花
                    {
                        huaNum++;
                    }
                    if (bIsFeng)//明牌是凤字牌(刻是风字牌)
                    {
                        huaNum++;
                    }
                }
                tiles = checkTileData;
                atomicCardNum = useAtomicCardCount;
                //DebugLog.Log("**** 明牌刻子: " + tile + " 使用原子数: " + useAtomicCardCount + " ****");
            }
        }

        //四原子
        if (CheckShiYuanZhi(haveTiles))
        {
            StatisticsProValue("四原子", MjChangeCoinType_Enum.MjChangeCoinType_4YuanZiPro,ref fanNum,ref yingYiFanNum,ref totalNum);
        }

        //套花齐
        int taoHuaQiNum = CheckTaoHuaQi();
        if (taoHuaQiNum > 0)
        {
            StatisticsProValue("套花齐", MjChangeCoinType_Enum.MjChangeCoinType_TaoHuaQiPro, ref fanNum, ref yingYiFanNum, ref totalNum, taoHuaQiNum);
        }

        //擦背
        if (CheckCaBei())
        {
            StatisticsProValue("擦背", MjChangeCoinType_Enum.MjChangeCoinType_CaBeiPro, ref fanNum, ref yingYiFanNum, ref totalNum);
        }

        //大吊车
        if (CheckDaDiaoChe(haveTiles, suits))
        {
            StatisticsProValue("大吊车", MjChangeCoinType_Enum.MjChangeCoinType_DaDiaoChe, ref fanNum, ref yingYiFanNum, ref totalNum);
        }

        //全大元
        if (CheckQuanDaYuan(haveTiles, m_PKDList))
        {
            //大大元
            if (JudgeTile.GetTileSuit(((CGame_CzMahjong)GameBase).m_AtomicCard) == 3)
            {
                StatisticsProValue("大大元", MjChangeCoinType_Enum.MjChangeCoinType_DaDaYuanPro, ref fanNum, ref yingYiFanNum, ref totalNum);
            }
            else
            {
                StatisticsProValue("全大元", MjChangeCoinType_Enum.MjChangeCoinType_QuanDaYuanPro, ref fanNum, ref yingYiFanNum, ref totalNum);
            }

        }
        else
        {
            //对对胡
            if (JudgeTile.CheckDuiDuiHu(haveTiles, m_PKDList, ((CGame_CzMahjong)GameBase).m_AtomicCard))
            {
                StatisticsProValue("对对胡", MjChangeCoinType_Enum.MjChangeCoinType_DuiDui, ref fanNum, ref yingYiFanNum, ref totalNum);
            }

            //混一色和清一色
            byte tileTempSuit = 10;
            byte retReslut = CheckHunYiSeOrQingYiSe(haveTiles, ref tileTempSuit);
            if (retReslut > 0)
            {
                if (retReslut == 1)
                {
                    StatisticsProValue("混一色", MjChangeCoinType_Enum.MjChangeCoinType_HunYiSePro, ref fanNum, ref yingYiFanNum, ref totalNum);
                }
                else
                {
                    StatisticsProValue("清一色", MjChangeCoinType_Enum.MjChangeCoinType_QingYiSe, ref fanNum, ref yingYiFanNum, ref totalNum);
                }

                //冷门
                if (tileTempSuit == JudgeTile.GetTileSuit(((CGame_CzMahjong)GameBase).m_AtomicCard))
                {
                    StatisticsProValue("冷门", MjChangeCoinType_Enum.MjChangeCoinType_LenMenPro, ref fanNum, ref yingYiFanNum, ref totalNum);
                }
            }
        }

        if (totalNum > GameBase.m_ProInfo.m_iMaxPro)
            totalNum = GameBase.m_ProInfo.m_iMaxPro;

        totalNum *= (ushort)(huaNum + ((CGame_CzMahjong)GameBase).m_nDiHua);

        if (!CheckMahjongHu(huaNum, fanNum, yingYiFanNum))
        {
            totalNum = 0;
        }

        //DebugLog.Log("*********** 当前牌: " + haveTiles.Last<byte>().ToString("X") + " 倍数: " + totalNum + " 花数: " + huaNum   + 
        //             " 底花: " + ((CGame_CzMahjong)GameBase).m_nDiHua + " 番数: " + fanNum + " 硬一番: " + yingYiFanNum + " ***********");
        return totalNum;
    }

    //胡牌条件限制
    bool CheckMahjongHu(ushort huaNum, ushort fanNum, ushort yingYiFanNum)
    {
        switch (((CGame_CzMahjong)GameBase).m_nMinHuRule)
        {
            case MinHuRule.eMHR_1Fan:
                if (fanNum == 0)
                {
                    return false;
                }
                break;
            case MinHuRule.eMHR_Ying1Fan:
                if (yingYiFanNum == 0)
                {
                    return false;
                }
                break;
            case MinHuRule.eMHR_3Hua:
                if (huaNum < 3 && fanNum == 0)
                {
                    return false;
                }
                break;
            case MinHuRule.eMHR_4Hua:
                if (huaNum < 4 && fanNum == 0)
                {
                    return false;
                }
                break;
        }
        return true;
    }

    //计算倍率
    void  StatisticsProValue(string name, MjChangeCoinType_Enum CoinType, ref ushort fanNum, ref ushort yingYiFanNum, ref ushort totalNum,int taoHuaQiNum = 1)
    {
        byte value = GameBase.m_ProInfo.m_CoinTypeProDict[CoinType];
        totalNum *= (ushort)Mathf.Pow(Mathf.Max(value, 1), taoHuaQiNum);
        fanNum += value;
        if (CoinType == MjChangeCoinType_Enum.MjChangeCoinType_QuanDaYuanPro || CoinType == MjChangeCoinType_Enum.MjChangeCoinType_DaDiaoChe ||
            CoinType == MjChangeCoinType_Enum.MjChangeCoinType_DaDaYuanPro || CoinType == MjChangeCoinType_Enum.MjChangeCoinType_DuiDui ||
            CoinType == MjChangeCoinType_Enum.MjChangeCoinType_HunYiSePro || CoinType == MjChangeCoinType_Enum.MjChangeCoinType_QingYiSe ||
            CoinType == MjChangeCoinType_Enum.MjChangeCoinType_LenMenPro)
        {
            yingYiFanNum++;
        }
        //DebugLog.Log(name +":" + value);
    }

    //查听
    public override bool CheckTing(bool bForce = false, byte suitCount = 3, byte atomicCardValue = 0,bool check7pair = true)
    {
        return base.CheckTing(bForce, 4, ((CGame_CzMahjong)GameBase).m_AtomicCard,false);
    }

    //四原子
    bool CheckShiYuanZhi(List<byte> handTiles)
    {
        int atomicCardCount = handTiles.FindAll(tile => tile == ((CGame_CzMahjong)GameBase).m_AtomicCard).Count();
        return atomicCardCount == 4;
    }

    //套花齐
    int CheckTaoHuaQi()
    {
        int taoHuaQiNum = 0;
        //春夏秋冬
        List<byte> list = m_HuaList.FindAll(s => s >= 0x41 && s <= 0x44);
        if (list.Count == 4)
        {
            taoHuaQiNum++; 
        }
        //梅兰竹菊
        list = m_HuaList.FindAll(s => s >= 0x45 && s <= 0x48);
        if (list.Count == 4)
        {
            taoHuaQiNum++;
        }
        return taoHuaQiNum;
    }

    //擦背
    bool CheckCaBei()
    {
        return ((CGame_CzMahjong)GameBase).AddPrice > 0;
    }

    //杠开胡
    bool CheckGangKaiHu()
    {
        return false;
    }

    //大吊车
    bool CheckDaDiaoChe(List<byte> handTiles, List<byte> suits)
    {
        List<byte> tiles = new List<byte>(handTiles);
        if (tiles.Count > 2)
            return false;

        ushort atomicCardCount = (ushort)tiles.RemoveAll(tileValue => tileValue == ((CGame_CzMahjong)GameBase).m_AtomicCard);
        if (JudgeTile.CheckNormalHu(tiles, suits, atomicCardCount))
            return true;
        return false;
    }

    //全大元(手上的牌是否全部是风字牌)
    bool CheckQuanDaYuan(List<byte> handTiles, List<PongKongData> pkdList)
    {
        if (handTiles.Count == 0)
            return false;

        List<byte> tiles = new List<byte>(handTiles);
        tiles.RemoveAll(tile => tile == ((CGame_CzMahjong)GameBase).m_AtomicCard);
        int findTileValue = tiles.FindIndex(tile => JudgeTile.GetTileSuit(tile) == 0 ||
                                                    JudgeTile.GetTileSuit(tile) == 1 || 
                                                    JudgeTile.GetTileSuit(tile) == 2);
        if (findTileValue != -1)
        {
            return false;
        }

        findTileValue = pkdList.FindIndex(pkdData => JudgeTile.GetTileSuit(pkdData.value) == 0 ||
                                                     JudgeTile.GetTileSuit(pkdData.value) == 1 || 
                                                     JudgeTile.GetTileSuit(pkdData.value) == 2);
        if (findTileValue != -1)
        {
            return false;
        }

        return true;
    }

    //判断是否是含有某种类型的牌
    bool CheckTileSuit(List<byte> handTiles, byte tileSuit)
    {
        int findTileValue = handTiles.FindIndex(tile => JudgeTile.GetTileSuit(tile) == tileSuit);
        if (findTileValue == -1)
        {
            findTileValue = m_PKDList.FindIndex(pkdData => JudgeTile.GetTileSuit(pkdData.value) == tileSuit);
            if (findTileValue == -1)
            {
                return false;
            }
        }
        return true;
    }


    //混一色和清一色判断 0:判断失败1: 混一色 2：清一色
    byte CheckHunYiSeOrQingYiSe(List<byte> handTiles, ref byte outSuit)
    {
        if (handTiles.Count == 0)
            return 0;

        List<byte> tiles = new List<byte>(handTiles);
        tiles.RemoveAll(tile => tile == ((CGame_CzMahjong)GameBase).m_AtomicCard);

        int suitCount = 0;
        List<byte> suits = new List<byte> { 0, 1, 2 };
        foreach (var suit in suits)
        {
            if (suitCount > 1)
            {
                return 0;
            }

            if (CheckTileSuit(tiles, suit))
            {
                outSuit = suit;
                suitCount++;
            }
        }

        if (suitCount == 1)
        {
            if (outSuit != 3 && CheckTileSuit(tiles, 3))
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        return 0;
    }

    //天胡和杠上胡
    bool CheckTianHuiOrGangShangHu(List<byte> handTiles, byte discardIndex = 0)
    {
        if (m_nDiscardIndex == discardIndex)
        {
            List<byte> tiles = new List<byte>(handTiles);
            List<byte> suits = new List<byte> { 0, 1, 2, 3 };
            ushort atomicCardCount = (ushort)tiles.RemoveAll(tileValue => tileValue == ((CGame_CzMahjong)GameBase).m_AtomicCard);
            if (JudgeTile.CheckNormalHu(tiles, suits, atomicCardCount))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取原子牌的数量
    /// </summary>
    protected override int GetAtomicTileCount()
    {
        return m_HaveTiles.FindAll(value => value == ((CGame_CzMahjong)GameBase).m_AtomicCard).Count;
    }

    /// <summary>
    /// 获取抓取牌插入的索引位置
    /// </summary>
    /// <param name="dealIndex">出牌的索引位置</param>
    /// <param name="targetIndex">抓取牌插入的索引位置</param>
    /// <returns></returns>
    protected override int GetInsertAtomicTileIndex(int dealIndex, int targetIndex)
    {
        if (m_HaveTiles[0] == ((CGame_CzMahjong)GameBase).m_AtomicCard && dealIndex != 0)
        {
            return m_HaveTiles.Count;
        }
        return targetIndex;
    }

    /// <summary>
    /// 玩家喂牌达到条件所表现的效果(2次)
    /// </summary>
    /// <param name="_ms"> 正常打牌解析数据</param>
    /// <param name="actionList">录像解析数据</param>
    /// <param name="index">录像数据索引</param>
    public override void PlayWeiPaiShowExpression(UMessage _ms, List<int> actionList = null, int index = 0)
    {
        byte expressionNum = 0;
        if (actionList == null)
        {
            expressionNum = _ms.ReadByte();
        }
        else
        {
            expressionNum = (byte)actionList[index];
        }

        PlayWeiPaiShowExpression(expressionNum);
    }
}