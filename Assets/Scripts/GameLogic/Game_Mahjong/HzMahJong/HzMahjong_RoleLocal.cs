using DG.Tweening;using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class HzMahjong_RoleLocal : Mahjong_RoleLocal{

    public Transform m_NiaoTransform = null;

    public HzMahjong_RoleLocal(CGame_Mahjong game, byte index)        : base(game, index)    {        m_NiaoTransform = null;    }

    public override void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_BACKDEALMJPOKER, HandleBackDiscard);    }

    public override void OnEnd()
    {
        base.OnEnd();
    }

    //查听
    public override bool CheckTing(bool bForce = false, byte suitCount = 3, byte atomicCardValue = 0, bool check7pair = true)
    {
        return base.CheckTing(bForce, 4 , ((CGame_HzMahjong)GameBase).m_AtomicCard);
    }

    public override void DiscardTile(Mahjong_Tile tile, byte disValue)
    {
        SendDiscardTileMsg(tile, disValue, GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_CM_PLAYERDEALMJPOKER);
    }

    public override ushort GetTileWinPro(List<byte> haveTiles)
    {
        byte lastValue = haveTiles.Last<byte>();
        if (haveTiles.Count % 3 != 2 || lastValue > 0x30 && lastValue != 0x35)
            return 0;

        ushort pro = 1;

        if(GameBase != null)
        {
            if(!((CGame_HzMahjong)GameBase).isDianPao())
            {
                pro = 2;
            }
        }
        if (pro > GameBase.m_ProInfo.m_iMaxPro)
            pro = GameBase.m_ProInfo.m_iMaxPro;

        return pro;
    }

    /// <summary>
    /// 获取原子牌的数量
    /// </summary>
    protected override int GetAtomicTileCount()
    {
        return m_HaveTiles.FindAll(value => value == ((CGame_HzMahjong)GameBase).m_AtomicCard).Count;
    }

    /// <summary>
    /// 获取抓取牌插入的索引位置
    /// </summary>
    /// <param name="dealIndex">出牌的索引位置</param>
    /// <param name="targetIndex">抓取牌插入的索引位置</param>
    /// <returns></returns>
    protected override int GetInsertAtomicTileIndex(int dealIndex, int targetIndex)
    {
        if (m_HaveTiles[0] == ((CGame_HzMahjong)GameBase).m_AtomicCard && dealIndex != 0)
        {
            return m_HaveTiles.Count;
        }
        return targetIndex;
    }    /// <summary>
    /// 加载抓鸟牌资源
    /// </summary>    public void LoadZhaNiaoPoker()
    {
        if(m_NiaoTransform == null)
        {
            m_NiaoTransform = ((GameObject)GameObject.Instantiate(GameBase.MahjongAssetBundle.LoadAsset("majiang_point_niao"))).transform;
            m_NiaoTransform.SetParent(m_HandTfm.parent,false);
        }
    }}