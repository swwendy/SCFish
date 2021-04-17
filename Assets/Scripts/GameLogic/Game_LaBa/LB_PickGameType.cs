﻿using System.Collections;
        //ShowLBTips("金币不足，将强制退出房间");

        LB_DataCenter.Instance().isKickOut = true;
    {
        UMessage leaveMsg = new UMessage((uint)GameCity.SlotSecondMsg.LabaMsg_CM_LEAVEROOM);
        leaveMsg.Add(GameMain.hall_.GetPlayerData().GetPlayerID());
        HallMain.SendMsgToRoomSer(leaveMsg);

        GameMain.hall_.SwitchToHallScene();
    }
    {
        GameData gamedata2 = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_LaBa);
        if (gamedata2 != null)
            AudioManager.Instance.PlaySound(gamedata2.ResourceABName, "Slots_button");
        //先进入该场条件是否达到
        List<int> coinlimitlist = LB_DataCenter.Instance().LevelJoinCoinLimit.ins;
        if (coinlimitlist.Count > 0 && GameMain.hall_.GetPlayerData().GetDiamond() < coinlimitlist[0])
        {
            CCustomDialog.OpenCustomConfirmUIWithFormatParamFunc(2003, OnExit, coinlimitlist[0]);
            
            return;
        }

        if (game_ == null)
        {
            game_ = new LB_StartGame();
            game_.Start(gameroot_, pickroot_, 1);
        }
        else
        {
            game_.level_ = 1;
            game_.reset();
        }
        isgame_ = true;
        game_.InitLevelInfo();

        PickGameMessage(1);
    }

    public override void ReconnectSuccess()
    {
        base.ReconnectSuccess();

        CCustomDialog.OpenCustomConfirmUI(1018, (p) => game_.Exit());
    }