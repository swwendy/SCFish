﻿using DG.Tweening;

                m_CountTip = new GameObject[2];
                obj = (GameObject)m_BjGameBase.m_AssetBundle.LoadAsset("Model_tipsBG");
                m_CountTip[0] = GameMain.instantiate(obj) as GameObject;
                m_CountTip[0].SetActive(false);
                m_CountTip[1] = GameMain.instantiate(obj) as GameObject;
                m_CountTip[1].SetActive(false);
            }

        //if (m_BjGameBase.m_RoomInfo.m_iCurPokerPos > 1)
        //{
        //    ShowCurrentCountTip(1);
        //    ShowCurrentCountTip(2);
        //}
        //else
        //    ShowCurrentCountTip(m_BjGameBase.m_RoomInfo.m_iCurPokerPos);

                byte index = cardPos;
                if (index > 0)
                    index--;
                points = m_nPoints[index, 0];
                otherPoint = m_nPoints[index, 1];
        {
            {
                ShowCurrentCountTip(2);
            }
        }
            switch (state)
            {
                case 1://停叫
                case 5://投降
                    OnQuitTurn(state);
                    break;
                case 2://加倍
                    OnDouble(pos);
                    break;
                case 3://要牌
                    break;
                case 4://分牌
                    OnSplitBegin();
                    return;
                default://fail
                    OnTurn(-1.0f);
                    break;
            }
        {
            if(state == 4)
            {
                m_bCanSplit = false;
            }
    {
        m_BjGameBase.m_RoomInfo.m_DealerAnimCtrl.StopAsk();
    }
        {
            OnResult(m_nTotalCoin, m_nSafeChipInCoin);
            m_nSafeChipInCoin = 0;
        }
            m_InfoUIObj.GetComponent<BillBoard>().OnCameraChanged();
            RectTransform rtfm = uiTfm as RectTransform;
            m_InfoUIObj.GetComponent<BillBoard>().SetPos(rtfm.localPosition);
        }