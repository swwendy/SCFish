﻿using System.Collections;
            {
                currentChipIndex -= 1;
                unitChip = unitChips[currentChipIndex];
                return;
            }
        SendLeaveRoomMsg();
        GameMain.hall_.SwitchToHallScene();
    {
        UMessage leaveMsg = new UMessage((uint)GameCity.SlotSecondMsg.LabaMsg_CM_LEAVEROOM);
        leaveMsg.Add(GameMain.hall_.GetPlayerData().GetPlayerID());
        HallMain.SendMsgToRoomSer(leaveMsg);
    }