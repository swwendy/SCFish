﻿using System;

    protected GameObject[] m_CountTip;
            GameObject.DestroyImmediate(parent.GetChild(i).gameObject);

                int index = i + (chips > grades[i] ? 2 : 1);
                UnityEngine.Object obj = (GameObject)m_BjGameBase.m_AssetBundle.LoadAsset("Model_chouma" + index);
                GameObject gameObj = GameMain.instantiate(obj) as GameObject;
                gameObj.transform.SetParent(parent, false);
                gameObj.GetComponentInChildren<TextMesh>().text = GameFunction.FormatCoinText(chips);

                break;

    public void ShowCountTip(bool bShow, byte pos = 0, Transform parent = null, byte count = 0, byte other = 0)
        if (m_CountTip == null || pos > m_CountTip.Length)
            return;

        int index = pos;
        if (pos > 0)
            index--;

        GameObject CountTip = m_CountTip[index];

        if (CountTip == null)
            return;

        CountTip.SetActive(false);
            {
                tm.text = "投降";
                {
                    if(pos == 0 && m_vecBlackPoker[pos].Count == 2)
                    {
                        tm.text = "黑杰克";
                        bg1.SetActive(false);
                        bg2.SetActive(true);
                    else if (m_vecBlackPoker[index].Count == 5)
                    {
                        tm.text = "五龙";
                        bg1.SetActive(false);
                        bg2.SetActive(true);
                    }
                    {
                        tm.text = "21";
                        bg1.SetActive(true);
                        bg2.SetActive(false);
                }
                {
                    tm.text = "五龙";
                    bg1.SetActive(false);
                    bg2.SetActive(true);
                {