﻿using System;using System.Collections.Generic;using System.Linq;using System.Text;using UnityEngine;using UnityEngine.UI;using XLua;[Hotfix]public class Black_Dealer : Black_Role{    Transform[] m_PokerPointArray;    Transform m_ChipPoint;    Transform m_CountPoint;    public byte m_nOpenPoke = 0;    public Black_Dealer(CGame_BlackJack bjGame, Transform uiParent)        : base(bjGame, uiParent)    {        if (m_BjGameBase != null)        {            string pointName = "Game_Model/Zhuozi/ZhuoMian/Points_New/Point_zhuang";            m_PokerPointArray = new Transform[5];            Transform parentTfm = GameObject.Find(pointName).transform;            Transform tfm = parentTfm.Find("Poker/Poker_1");            for (int j = 0; j < 5; j++)            {                m_PokerPointArray[j] = tfm.GetChild(j);            }            m_CountPoint = tfm.Find("point_tips_bg");            m_ChipPoint = parentTfm.Find("Chip").GetChild(0);            m_vecBlackPoker = new List<byte>[1];            m_vecBlackPoker[0] = new List<byte>();            m_InfoUIObj = uiParent.parent.Find("Top").Find("DelaerInfo").gameObject;            m_InfoUIObj.SetActive(false);

            m_CountTip = new GameObject[1];
            UnityEngine.Object obj = (GameObject)m_BjGameBase.m_AssetBundle.LoadAsset("Model_tipsBG_dealer");
            m_CountTip[0] = GameMain.instantiate(obj) as GameObject;
            m_CountTip[0].SetActive(false);
        }    }    public override void OnEnd()    {        m_nOpenPoke = 0;        if (m_PokerPointArray != null)        {            foreach (Transform tfm in m_PokerPointArray)            {                tfm.gameObject.SetActive(false);            }        }        if (m_ChipPoint != null)        {            foreach (Transform tfm in m_ChipPoint)            {                GameObject.Destroy(tfm.gameObject);            }        }        ShowCountTip(false);        base.OnEnd();    }    public void OnEnterRoom(bool bLocal)    {        if (m_nUserId == 0)            return;        ShowPlayerInfo(!bLocal);        for(int i = 0; i < m_vecBlackPoker[0].Count; i++)        {            AddPoker(m_vecBlackPoker[0][i], m_PokerPointArray[i]);        }        Update2ndPoker(m_nOpenPoke > 0);    }    public override void HandleAddPoker(byte pos, byte card, byte points, byte otherPoints)    {        int index = m_vecBlackPoker[pos].Count;        if (index >= 5)            return;        AddPoker(card, m_PokerPointArray[index]);        m_vecBlackPoker[pos].Add(card);        Update2ndPoker(index > 1, points, otherPoints);        base.HandleAddPoker(pos, card, points, otherPoints);    }    public void HidePoker(int index = 0)    {        if (index >= m_vecBlackPoker[0].Count)            return;        m_PokerPointArray[index].gameObject.SetActive(false);    }    public void Update2ndPoker(bool showCount = false, byte points = 0, byte otherPoints = 0)    {        if(m_vecBlackPoker[0].Count > 0)        {            m_PokerPointArray[0].gameObject.SetActive(true);            m_PokerPointArray[0].localRotation = Quaternion.AngleAxis(((m_vecBlackPoker[0].Count == 1) ? 182.0f : 2.0f), Vector3.forward);        }        if (m_vecBlackPoker[0].Count > 1)        {            m_PokerPointArray[1].gameObject.SetActive(true);            m_PokerPointArray[1].localRotation = Quaternion.AngleAxis((showCount ? 2.0f : 182.0f), Vector3.forward);        }        if (showCount)            ShowCountTip(true, 0, m_CountPoint, points, points >= 17 ? points : otherPoints);    }    protected override bool ShowPlayerInfo(bool show = true, bool invite = false)    {        if(!base.ShowPlayerInfo(show, invite))            return false;        if(show)        {            Text[] ts = m_InfoUIObj.GetComponentsInChildren<Text>();            Debug.Assert(ts.Length >= 3);            ts[1].text = m_cRoleName;            ts[2].text = m_nTotalCoin.ToString();        }        return true;    }    public override void UpdateInfoUI()    {        if (m_InfoUIObj == null || !m_InfoUIObj.activeSelf)            return;        Text[] ts = m_InfoUIObj.GetComponentsInChildren<Text>();        Debug.Assert(ts.Length >= 3);        ts[2].text = m_nTotalCoin.ToString();
    }

    public void OnResult(long addCoin)    {        if (m_InfoUIObj == null || !m_InfoUIObj.activeSelf)        {            if (m_nUserId == GameMain.hall_.GetPlayerId())            {                m_BjGameBase.OnGameLocalPlayerResult(m_nTotalCoin, addCoin);            }        }        UpdateInfoUI();    }}