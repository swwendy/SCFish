﻿using System.Collections;

        Switch2GamePanel();

        UMessage chooselevel = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_BULLKILL_CM_CHOOSElEVEL);

        chooselevel.Add(GameMain.hall_.GetPlayerId());
        chooselevel.Add((byte)1);

        HallMain.SendMsgToRoomSer(chooselevel);

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_BullHappy);
        if (gamedata == null)
            return;

        AudioManager.Instance.PlaySound(gamedata.ResourceABName, BAK_DataCenter.Instance().audiodatas[1002].audioName);
        //Switch2GamePanel();
        game_.RefreshGamePanel();

    public override void ReconnectSuccess()
    {
        base.ReconnectSuccess();

        CCustomDialog.OpenCustomConfirmUI(1018, (p) => game_.Exit());
    }