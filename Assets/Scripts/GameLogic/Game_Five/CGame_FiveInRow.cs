﻿
        CMsgDispatcher.GetInstance().RegMsgDictionary(
                (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER, BackLeaveRoomServer);
    {
        GameMain.hall_.SwitchToHallScene();

        return true;
    }
    {
        SwitchPickButton(false);

        UMessage levelMsg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FIVEINROW_CM_CHOOSElEVEL);

        levelMsg.Add(GameMain.hall_.GetPlayerId());
        currentLevel_ = 1;
        levelMsg.Add((byte)(currentLevel_));

        HallMain.SendMsgToRoomSer(levelMsg);
    }
            //if (roomroot_.activeSelf)
            //    SwitchPickButton(true);
            //else
            //    GameMain.hall_.SwitchToHallScene();
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_PLAYERLEAVEROOMSER);
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);