﻿using System.Collections.Generic;
        CMsgDispatcher.GetInstance().RegMsgDictionary(
                (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SENDPLAYERGAMESTATISTICSDATA, BackPlayerTotalData);
    {
        byte length = msg.ReadByte();

        if(GameMain.hall_.GetPlayerData().totalinfo.Count == 0)
        {
            TotalInfo zeroti7 = new TotalInfo();
            GameMain.hall_.GetPlayerData().totalinfo.Add((byte)7, zeroti7);
            TotalInfo zeroti13 = new TotalInfo();
            GameMain.hall_.GetPlayerData().totalinfo.Add((byte)13, zeroti13);
        }

        for(int index = 0; index < length; index++)
        {
            byte gamekind = msg.ReadByte();
            if (GameMain.hall_.GetPlayerData().totalinfo.ContainsKey(gamekind))
            {
                GameMain.hall_.GetPlayerData().totalinfo[gamekind].matchtimes = msg.ReadUInt();
                GameMain.hall_.GetPlayerData().totalinfo[gamekind].totaltimes = msg.ReadUInt();
                GameMain.hall_.GetPlayerData().totalinfo[gamekind].finaltimes = msg.ReadUInt();
                GameMain.hall_.GetPlayerData().totalinfo[gamekind].bestsort = (ushort)msg.ReadShort();
                GameMain.hall_.GetPlayerData().totalinfo[gamekind].special1 = msg.ReadUInt();
                GameMain.hall_.GetPlayerData().totalinfo[gamekind].special2 = msg.ReadUInt();
                GameMain.hall_.GetPlayerData().totalinfo[gamekind].glory.Clear();
                for (int gloryindex = 0; gloryindex < 3; gloryindex++)
                    GameMain.hall_.GetPlayerData().totalinfo[gamekind].glory.Add(msg.ReadUInt());
            }
            else
            {
                TotalInfo ti = new TotalInfo();
                ti.matchtimes = msg.ReadUInt();
                ti.totaltimes = msg.ReadUInt();
                ti.finaltimes = msg.ReadUInt();
                ti.bestsort = (ushort)msg.ReadShort();
                ti.special1 = msg.ReadUInt();
                ti.special2 = msg.ReadUInt();

                ti.glory.Clear();
                for (int gloryindex = 0; gloryindex < 3; gloryindex++)
                    ti.glory.Add(msg.ReadUInt());

                GameMain.hall_.GetPlayerData().totalinfo.Add(gamekind, ti);
            }
        }

        InitPlayerInfoPanel();

        return true;
    }
    {
        if (isAskTotalData)
            return;

        UMessage totaldatamsg = new UMessage((byte)GameCity.EMSG_ENUM.CrazyCityMsg_APPLYPLAYERGAMESTATISTICSDATA);

        totaldatamsg.Add(GameMain.hall_.GetPlayerId());
        totaldatamsg.Add(kind);

        NetWorkClient.GetInstance().SendMsg(totaldatamsg);

        isAskTotalData = true;
    }

        //奖券
        //Text lotterytxt = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").FindChild("Image_TicketFrame").FindChild("Text_Ticket")
        //  .gameObject.GetComponent<Text>();
        //lotterytxt.text = playerdata.GetLottery().ToString();

        //vip
        //Image VipIamge = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").FindChild("Image_Vip").FindChild("Vip_Text")
        // .FindChild("Num").gameObject.GetComponent<Image>();
        //AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        //if (playerdata.GetVipLevel() == 0)
        //{
        //    Image VipHui = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").FindChild("Image_Vip").gameObject.GetComponent<Image>();
        //    VipHui.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_hui");
        //}

        //VipIamge.sprite = bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + playerdata.GetVipLevel().ToString());

        Text masterScoreTx = PlayerInfoMainUI.transform.Find("ImageBG").Find("Middle").

        //Text contestTimesTx = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").
        //    FindChild("Text_bisaichangci").FindChild("Text").gameObject.GetComponent<Text>();
        //contestTimesTx.text = "";

        //Text finalContestTimesTx = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").
        //    FindChild("Text_juesai").FindChild("Text").gameObject.GetComponent<Text>();
        //finalContestTimesTx.text = "";

        //Text bestSortTx = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").
        //    FindChild("Text_zuijia").FindChild("Text").gameObject.GetComponent<Text>();
        //bestSortTx.text = "";

        //Text sumTimesTx = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").
        //    FindChild("Text_zongshu").FindChild("Text").gameObject.GetComponent<Text>();
        //sumTimesTx.text = "";

        //Text winRateTx = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").
        //    FindChild("Text_shenglv").FindChild("Text").gameObject.GetComponent<Text>();
        //winRateTx.text = "";

        //Text creditScore = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").
        //    FindChild("Text_xinyu").FindChild("Text").gameObject.GetComponent<Text>();
        //creditScore.text = "";

        //俱乐部
        //Text clubname = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").FindChild("Image_Club")
        //               .FindChild("Text").gameObject.GetComponent<Text>();
        //if (playerdata.GetGuildID() == 0)
        //{
        //    clubname.text = "尚未加入俱乐部";
        //}
        //else
        //{
        //    if(!string.IsNullOrEmpty(GuildData.Instance().GuildName))
        //      clubname.text = GuildData.Instance().GuildName;
        //    else
        //    {
        //        clubname.text = playerdata.GetGuildName();
        //    }
        //}

        Text masterstep = PlayerInfoMainUI.transform.Find("ImageBG/Middle/Image_dashi/Text_num").gameObject.GetComponent<Text>();
        masterstep.text = CCsvDataManager.Instance.GameDataMgr.GetMasterLv(GameMain.hall_.GetPlayerData().MasterScoreKindArray[(int)GameKind_Enum.GameKind_LandLords]);
        Text masterscore = PlayerInfoMainUI.transform.Find("ImageBG/Middle/Image_dashi/Text_fen").gameObject.GetComponent<Text>();
        masterscore.text = GameMain.hall_.GetPlayerData().MasterScoreKindArray[(int)GameKind_Enum.GameKind_LandLords].ToString();

        ////已经修改过昵称
        //if (playerdata != null && playerdata.IsChangeName)
        //{
        //    changNameBtn.SetActive(false);         
        //}
        //else
        //{
        //    XPointEvent.AutoAddListener(changNameBtn, ClickChangeNameBtn, null);
        //}

        //修改ICON
        //GameObject changIconBtn = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").FindChild("Image_HeadFram")
        //    .FindChild("Button_Head").gameObject;
        //XPointEvent.AutoAddListener(changIconBtn, ClickChangeIconBtn, null);

        //切换账号
        //GameObject ChangeAccountBtn = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").FindChild("Image_ID")
        //    .FindChild("ButtonQiehuan").gameObject;
        //XPointEvent.AutoAddListener(ChangeAccountBtn, ClickChangeAccountBtn, null);

        //俱乐部
        //GameObject joinclubBtn = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Middle").FindChild("Image_Club")
        //       .FindChild("ButtonJoinClub").gameObject;
        // XPointEvent.AutoAddListener(joinclubBtn, ClickJoinClubBtn, null);
        // if (playerdata.GetGuildID() == 0)
        //     joinclubBtn.SetActive(true);
        // else
        //     joinclubBtn.SetActive(false);

        //绑定手机号
        //GameObject bindphoneBtn = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Bottom").FindChild("Button_Safe").gameObject;
        //XPointEvent.AutoAddListener(bindphoneBtn, ClickBindPhoneBtn, null);

        //赠送记录
        //GameObject GiveBtn = PlayerInfoMainUI.transform.FindChild("ImageBG").FindChild("Bottom").FindChild("Button_Zengsongjilu").gameObject;
        //XPointEvent.AutoAddListener(GiveBtn, ClickGiveRecordBtn, null);


        //邀请码
        /*GameObject InviteBtn = PlayerInfoMainUI.transform.FindChild("Bottom").FindChild("Button_invitecode").gameObject;

        Toggle guandanInfo = PlayerInfoMainUI.transform.Find("ImageBG/Left/Toggle (1)").gameObject.GetComponent<Toggle>();

        Toggle landlordInfo = PlayerInfoMainUI.transform.Find("ImageBG/Left/Toggle (2)").gameObject.GetComponent<Toggle>();

    //回到登录界面
    private void Exit2Login(EventTriggerType eventtype, object parma, PointerEventData eventData)
            CGameContestRankingTifings.GetChessRankingInstance(false).ClearContestRankingData();
    {
        GameObject panel = PlayerInfoMainUI.transform.Find("ImageBG/Middle/GameInfo_" + pick.ToString()).gameObject;
        panel.SetActive(ison);

        SetChangeInfoData(gameid, pick);
    }
    {
        Text masterstep = PlayerInfoMainUI.transform.Find("ImageBG/Middle/Image_dashi/Text_num").gameObject.GetComponent<Text>();
        masterstep.text = CCsvDataManager.Instance.GameDataMgr.GetMasterLv(GameMain.hall_.GetPlayerData().MasterScoreKindArray[gameid]);
        Text masterscore = PlayerInfoMainUI.transform.Find("ImageBG/Middle/Image_dashi/Text_fen").gameObject.GetComponent<Text>();
        masterscore.text = GameMain.hall_.GetPlayerData().MasterScoreKindArray[gameid].ToString();

        for (int gloryindex = 1; gloryindex < 4; gloryindex++)
        {
            GameObject glorybg = PlayerInfoMainUI.transform.Find("ImageBG/bottom/ImageIcon_" + gloryindex.ToString()).gameObject;

            Text tempglory = glorybg.transform.Find("Text").gameObject.GetComponent<Text>();
            tempglory.text = GameMain.hall_.GetPlayerData().totalinfo[(byte)gameid].glory[gloryindex - 1].ToString();
        }
    }
    {
        credit.text = GameMain.hall_.GetPlayerData().creditScore.ToString();

        for (int index = 1; index < 3; index++)
        {
            GameObject infobg = PlayerInfoMainUI.transform.Find("ImageBG/Middle/GameInfo_" + index.ToString()).gameObject;
            byte key = (index == 1) ? (byte)7 : (byte)13;
            if (!GameMain.hall_.GetPlayerData().totalinfo.ContainsKey(key))
                continue;
            Text matchtimes = infobg.transform.Find("Text_bisaichangci/Text").gameObject.GetComponent<Text>();
            matchtimes.text = GameMain.hall_.GetPlayerData().totalinfo[key].matchtimes.ToString();
            Text finaltimes = infobg.transform.Find("Text_juesai/Text").gameObject.GetComponent<Text>();
            finaltimes.text = GameMain.hall_.GetPlayerData().totalinfo[key].finaltimes.ToString();
            Text bestscore = infobg.transform.Find("Text_zuijia/Text").gameObject.GetComponent<Text>();
            bestscore.text = GameMain.hall_.GetPlayerData().totalinfo[key].bestsort.ToString();
            Text alltimes = infobg.transform.Find("Text_zongshu/Text").gameObject.GetComponent<Text>();
            alltimes.text = GameMain.hall_.GetPlayerData().totalinfo[key].totaltimes.ToString();
            Text info1 = infobg.transform.Find("Text_info_1/Text").gameObject.GetComponent<Text>();
            info1.text = GameMain.hall_.GetPlayerData().totalinfo[key].special1.ToString();
            Text info2 = infobg.transform.Find("Text_info_2/Text").gameObject.GetComponent<Text>();
            info2.text = GameMain.hall_.GetPlayerData().totalinfo[key].special2.ToString();
        }

        SetChangeInfoData((int)GameKind_Enum.GameKind_LandLords, 1);
    }
            // 回到登陆界面
            //PlayerData playerdata = GameMain.hall_.GetPlayerData();
            //if(playerdata.nBindMobileNum != 0)
            {
            if (PlayerIconToggleOnObj != null)
            CustomAudio.GetInstance().PlayCustomAudio(1002);