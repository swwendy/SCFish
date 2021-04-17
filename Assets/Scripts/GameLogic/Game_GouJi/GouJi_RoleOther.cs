using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XLua;

/// <summary>
/// 其他够级玩家
/// </summary>
[Hotfix]
public class GouJi_RoleOther : GouJi_Role
{
    private List<byte> m_recordHavePokerList = new List<byte>();
    public GouJi_RoleOther(CGame_GouJi gameBase, byte cSit) :base(gameBase, cSit)
    {

    }
    /// <summary>
    /// 重置角色数据
    /// </summary>
    /// <param name="bDestroryState">true:全部都删除false:删除一部分</param>
    public override void ResetRoleData(bool bDestroryState = true)
    {
        base.ResetRoleData(bDestroryState);
        if(m_GameBase == null)
        {
            return;
        }
        if(m_GameBase.GameMode == GameTye_Enum.GameType_Record &&
           m_GameBase.m_RecordTransform != null)
        {
            Transform havePokerTransform = m_GameBase.m_RecordTransform.Find("Point_shoupai_" + (m_nCSit + 1));
            for (int index = 0; index < havePokerTransform.childCount; ++index)
            {
                havePokerTransform.GetChild(index).gameObject.SetActive(false);
            }
        }
        m_recordHavePokerList.Clear();
    }

    /// <summary>
    /// 玩家游戏逻辑推进
    /// </summary>
    public override void OnTick()    {
        base.OnTick();
    }

    /// <summary>
    /// 服务器提问答复事件
    /// </summary>
    /// <param name="doing">类型</param>
    /// <param name="baseFunctionState">true(执行父类方法)false(不执行父类方法)</param>
    /// <param name="param">参数</param>
    /// <param name="time">文本提示时间</param>
    public override void AnswerDoingEvent(RoleDoing_Enum doing, bool baseFunctionState = true, byte param = 230,float time = 2.0f)
    {
        if(baseFunctionState)
        {
            base.AnswerDoingEvent(doing, baseFunctionState, param,time);
        }
        switch (doing)
        {
            case RoleDoing_Enum.RoleDoing_GeMing:
                break;
        }
    }

    /// <summary>
    /// 刷新手牌界面
    /// </summary>
    public override void RefreshHavePokerPanel()
    {
        if (m_OutPokerList.Count != 0)
        {
            DestoryHavePokerOjbect(m_OutPokerList);
            m_OutPokerList.Clear();
        }

        if (m_GameBase == null)
        {
            return;
        }


        if (m_GameBase.GameMode != GameTye_Enum.GameType_Record ||
            m_GameBase.m_RecordTransform == null)
        {
            return;
        }

        bool activeState = false;
        int tempPokerValue = 0;
        List<byte> curPokerList = null;
        Transform pokerTransform = null;
        Transform havePokerTransform = m_GameBase.m_RecordTransform.Find("Point_shoupai_" + (m_nCSit + 1));
        for (int pokerValue = 1; pokerValue < 16; ++pokerValue)
        {
            pokerTransform = havePokerTransform.Find("Poker_Choupai_Icon_ (" + pokerValue + ")");
            if (pokerTransform == null)
            {
                continue;
            }
            activeState = false;
            if (pokerValue == 14)
            {
                tempPokerValue = -1;
            }
            else if (pokerValue == 15)
            {
                tempPokerValue = 0;
            }
            else
            {
                tempPokerValue = pokerValue;
            }
            curPokerList = m_recordHavePokerList.FindAll(value => tempPokerValue == GameCommon.GetCardValue(value));
            if (curPokerList != null && curPokerList.Count > 0)
            {
                pokerTransform.Find("Text").GetComponent<Text>().text = curPokerList.Count.ToString();
                activeState = true;
            }
            pokerTransform.gameObject.SetActive(activeState);
        }
    }

    /// <summary>
    /// 添加手牌对象
    /// </summary>
    /// <param name="pokerDataList">牌值</param>
    /// <param name="isMask">是否打开遮罩 true（打开）false(关闭)</param>
    /// <param name="isGongMask">是否是贡牌 true（是）false(不是)</param>
    public override void AddHavePokerObject(List<byte> pokerDataList, bool isMask = false, bool isGongMask = false)
    {
        if(m_GameBase == null)
        {
            return;
        }
        if(m_GameBase.GameMode != GameTye_Enum.GameType_Record)
        {
            return;
        }
        m_recordHavePokerList.AddRange(pokerDataList);
        RefreshHavePokerPanel();
    }

    /// <summary>
    /// 删除手牌对象
    /// </summary>
    /// <param name="pokerDataList">牌值</param>
    public override void DestoryHavePokerOjbect(List<byte> pokerDataList)
    {
        if (pokerDataList.Count == 0 || m_GameBase == null)
        {
            return;
        }
        if (m_GameBase.GameMode != GameTye_Enum.GameType_Record)
        {
            return;
        }
        foreach (var value in pokerDataList)
        {
            m_recordHavePokerList.Remove(value);
        }
    }

    /// <summary>
    /// 删除手牌对象
    /// </summary>
    /// <param name="pokerDataList">牌值</param>
    public override void DestoryHavePokerOjbect(List<CardData> pokerDataList)
    {
        if (pokerDataList.Count == 0 || m_GameBase == null)
        {
            return;
        }

        if (m_GameBase.GameMode != GameTye_Enum.GameType_Record)
        {
            return;
        }

        foreach (var value in pokerDataList)
        {
            m_recordHavePokerList.Remove(value.m_nCardValue);
        }
    }

    /// <summary>
    /// 获得手牌数量
    /// </summary>
    /// <returns></returns>
    public override int GetHavePokerNum()
    {
        if(m_GameBase == null)
        {
            return -1;
        }
        return m_GameBase.GameMode == GameTye_Enum.GameType_Record ? m_recordHavePokerList.Count : m_HavePokerList.Count;
    }
}
