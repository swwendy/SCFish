﻿using System;

            m_CountTip = new GameObject[1];
            UnityEngine.Object obj = (GameObject)m_BjGameBase.m_AssetBundle.LoadAsset("Model_tipsBG_dealer");
            m_CountTip[0] = GameMain.instantiate(obj) as GameObject;
            m_CountTip[0].SetActive(false);
        }
    }

    public void OnResult(long addCoin)