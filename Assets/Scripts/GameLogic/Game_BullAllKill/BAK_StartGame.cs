﻿using System.Collections;
        {
            Vector3 deltaPosition = new Vector3(unitForward.x * speed * time, unitForward.y * speed * time, 0.0f);
            flyObj.transform.Translate(deltaPosition);
        }    
        //coin.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
        coin.text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();
            CustomAudio.GetInstance().PlayCustomAudio(1002);
                    //CCustomDialog.OpenCustomConfirmUI(2415);
                    CRollTextUI.Instance.AddVerticalRollText(2415);
                    //CCustomDialog.OpenCustomConfirmUI(2414);
                    CRollTextUI.Instance.AddVerticalRollText(2414);
        }
    {
        if (pickroot_ != null)
            pickroot_.SetActive(true);
        gameroot_.SetActive(false);
        isshowsetpanel = false;
        isupdatesetpanel = true;
        CloseResultPanel();
        ClearChipObjects();
        CCustomDialog.CloseCustomWaitUI();
        //RefreshPickPlayerInfo();

        GameMain.safeDeleteObj(FlyPokers.Instance().flyObj);
        isgaming = false;
        witchChip_ = 0;
        currentChip_ = BAK_DataCenter.Instance().chipsdatas[0];

        GameMain.hall_.SwitchToHallScene();

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullAllKill);
        if (gamedata == null)
            return;
        AudioManager.Instance.PlaySound(gamedata.ResourceABName, BAK_DataCenter.Instance().audiodatas[1002].audioName);
        //FlyPokers.instance_ = null;
    }
        //coin.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
        coin.text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();
            //coin.text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
        }
                coin.text = BAK_DataCenter.Instance().ber.vipSeatInfos[(byte)index].money.ToString();