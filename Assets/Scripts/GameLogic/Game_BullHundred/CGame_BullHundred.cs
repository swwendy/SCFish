﻿//#define HAVE_LOBBY
                go.transform.Find("head/tongxiang").GetComponent<Image>().sprite
                    = GameMain.hall_.GetIcon(url, playerid, (int)faceId);


    public override void ReconnectSuccess()
    {
        base.ReconnectSuccess();

        CCustomDialog.OpenCustomConfirmUI(1018, (p) => BackToChooseLevel());
    }