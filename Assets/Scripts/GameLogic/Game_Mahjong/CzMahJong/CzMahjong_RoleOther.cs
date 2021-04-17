using DG.Tweening;using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class CzMahjong_RoleOther : Mahjong_RoleOther{    public CzMahjong_RoleOther(CGame_Mahjong game, byte index)        : base(game, index)    {

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

    /// <summary>
    /// 补花以后手牌更新
    /// </summary>
    /// <param name="huaList">补花牌</param>
    /// <param name="pokerList">补花以后手牌</param>
    public override void UpdateMahjongHaveTiles(List<byte> huaList, List<byte> pokerList)
    {
       if(GameBase.GameMode == GameTye_Enum.GameType_Record)
        {
            base.UpdateMahjongHaveTiles(huaList,pokerList);
        }
    }}