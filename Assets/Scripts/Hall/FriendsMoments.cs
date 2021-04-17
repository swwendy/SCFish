﻿using System.Collections.Generic;

        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Friends_Moments_SM_LeaveFriendsTable, BackMomentsLeaveTable);

        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.Friends_Moments_SM_Break, BackMomentsBreak);
    {
        byte state = msg.ReadByte();
        if(state == 0)
        {
            CCustomDialog.OpenCustomConfirmUI(1650);
            return false;
        }

        GameMain.hall_.GetPlayerData().momentsid = 0;
        FriendsMomentsDataMamager.GetFriendsInstance().momentsID = 0;
        FriendsMomentsDataMamager.GetFriendsInstance().friendDiamond = 0;

        LoadFriendsResource();
        if(GameMain.hall_.contestui_ != null)
            GameMain.hall_.contestui_.SetActive(true);
        if(main_ != null)
            main_.SetActive(false);
        GameMain.safeDeleteObj(main_);

        FriendsMomentsDataMamager.GetFriendsInstance().memberList.Clear();
        FriendsMomentsDataMamager.GetFriendsInstance().tableList.Clear();
        FriendsMomentsDataMamager.GetFriendsInstance().joinList.Clear();
        recordlist_.Clear();

        CCustomDialog.OpenCustomConfirmUI(1212);

        return true;
    }
    {
        uint appointmentid = msg.ReadUInt();
        uint playerid = msg.ReadUInt();
        byte sitNo = 0;

        if(FriendsMomentsDataMamager.GetFriendsInstance().tableList.ContainsKey(appointmentid))
        {
            for(int index = 0; index < FriendsMomentsDataMamager.GetFriendsInstance().tableList[appointmentid].seats_.Count; index++)
            {
                if (FriendsMomentsDataMamager.GetFriendsInstance().tableList[appointmentid].seats_[index].playerid == playerid)
                {
                    sitNo = (byte)index;
                    break;
                }   
            }

            if(FriendsMomentsDataMamager.GetFriendsInstance().tableList[appointmentid].hostid == playerid)
                FriendsMomentsDataMamager.GetFriendsInstance().tableList.Remove(appointmentid);
            else
                FriendsMomentsDataMamager.GetFriendsInstance().tableList[appointmentid].seats_[sitNo].playerid = 0;

            if(tableobjects_.ContainsKey(appointmentid))
            {
                if (tableobjects_[appointmentid] != null)
                {
                    if(FriendsMomentsDataMamager.GetFriendsInstance().tableList.ContainsKey(appointmentid))
                    {
                        if (FriendsMomentsDataMamager.GetFriendsInstance().tableList[appointmentid].gameid == (byte)GameKind_Enum.GameKind_LandLords)
                            if (sitNo == 2)
                                sitNo += 1;

                        tableobjects_[appointmentid].transform.Find("tableBG/player_" + (sitNo + 1) + "/seat").gameObject.SetActive(true);
                        tableobjects_[appointmentid].transform.Find("tableBG/player_" + (sitNo + 1) + "/Image_HeadBG").gameObject.SetActive(false);
                    }
                }
                else
                    tableobjects_.Clear();
            }
        }

        return true;
    }
                    data.gameID == (byte)GameKind_Enum.GameKind_YcMahjong)
            {
                if (data.maxpower == 250 || data.maxpower == 0)
            }

            if (FriendsMomentsDataMamager.GetFriendsInstance().memberList.ContainsKey(playerdata.playerid))
            {
                playerdata.faceid = FriendsMomentsDataMamager.GetFriendsInstance().memberList[playerdata.playerid].faceid;
                playerdata.playerName = FriendsMomentsDataMamager.GetFriendsInstance().memberList[playerdata.playerid].memberName;
                playerdata.coin = msg.ReadLong();
                playerdata.url = FriendsMomentsDataMamager.GetFriendsInstance().memberList[playerdata.playerid].url;
            }
            {
                playerdata.faceid = 101;
                playerdata.playerName = "";
                playerdata.coin = msg.ReadLong();
                playerdata.url = "";
            }
        if (tableobjects_.ContainsKey(tableid))
        {
            FriendsMomentsDataMamager.GetFriendsInstance().tableList.Remove(tableid);
            InitTables();
        }
                    else
                        powerText = " 打" + data.gameTimes + "局 打" + data.maxpower.ToString();
            
            member.isonline = msg.ReadByte() == 1;
                    else
                        powerText = " 打" + mt.ter.times + "局 双下" + mt.ter.score.ToString() + "分 打" + (byte)mt.ter.cp;
                {
                    MomentsMember mm = new MomentsMember();

                    mm.faceid = msg.ReadInt();
                    mm.memberName = msg.ReadString();
                    mm.url = msg.ReadString();

                    if (!otherplayers.ContainsKey(mt.seats_[mtindex].playerid))
                        otherplayers.Add(mt.seats_[mtindex].playerid, mm);
                }
        {
            CCustomDialog.OpenCustomConfirmUI(2206);
            return false;
        }
        {
            FriendsMomentsDataMamager.GetFriendsInstance().tableList.Remove(tableid);
            InitTables();
        }
        {
            MomentsMember mm = new MomentsMember();

            mm.faceid = msg.ReadInt();
            mm.memberName = msg.ReadString();
            mm.url = msg.ReadString();

            if(!otherplayers.ContainsKey(playerid))
                otherplayers.Add(playerid, mm);
        }
        if (FriendsMomentsDataMamager.GetFriendsInstance().tableList.ContainsKey(tableid))
            table = FriendsMomentsDataMamager.GetFriendsInstance().tableList[tableid];
            tableobjects_.Clear();
            return false;
        }
        {
            icon.sprite = GameMain.hall_.GetIcon(FriendsMomentsDataMamager.GetFriendsInstance().memberList[table.seats_[sitNo].playerid].url,
                    FriendsMomentsDataMamager.GetFriendsInstance().memberList[table.seats_[sitNo].playerid].memberid,
                    FriendsMomentsDataMamager.GetFriendsInstance().memberList[table.seats_[sitNo].playerid].faceid);
        }
        else
        {
            icon.sprite = GameMain.hall_.GetIcon(otherplayers[table.seats_[sitNo].playerid].url,
                    otherplayers[table.seats_[sitNo].playerid].memberid,
                    otherplayers[table.seats_[sitNo].playerid].faceid);
        }
        uint playerid = msg.ReadUInt();
        long diamond = msg.ReadLong();

        if (main_ == null)
        {
            CCustomDialog.OpenCustomConfirmUI(1501);
            return false;
        }
                else
                    powerText = "打" + table.ter.times + "局 双下" + table.ter.score.ToString() + "分 打" + (byte)table.ter.cp;
        
        {
            CCustomDialog.OpenCustomConfirmUI(1649);
            return false;
        }
                {
                    FriendsMomentsDataMamager.GetFriendsInstance().joinList.Add(member.memberid, member);
                }
                    mm.isonline = msg.ReadByte() > 0;
                            else
                                powerText = " 打" + mt.ter.times + "局 双下" + mt.ter.score.ToString() + "分 打" + (byte)mt.ter.cp;
                mm.url = msg.ReadString();
                mm.isonline = msg.ReadByte() == 1;
                //mm.isonline = true;

        //被拒绝
        if (state == 1)
        {
            object[] param = { momentsName };
            CCustomDialog.OpenCustomConfirmUIWithFormatParam(1632, param);
        }

        //已有朋友圈
        if (state == 2)
            CCustomDialog.OpenCustomConfirmUI(1656);

        GameObject breakBtn = main_.transform.Find("Pop-up/set/ButtonGroupMask/ButtonGroup/Button_jiesan").gameObject;
        {
            if (data.result[GameMain.hall_.GetPlayerId()].coin >= 0)
            {
                selfcoinTx.text = "+" + data.result[GameMain.hall_.GetPlayerId()].coin.ToString();
                selfcoinTx.color = new Color(217.0f / 255.0f, 59.0f / 255.0f, 42.0f / 255.0f);
            }
            else
            {
                selfcoinTx.text = data.result[GameMain.hall_.GetPlayerId()].coin.ToString();
                selfcoinTx.color = new Color(89.0f / 255.0f, 130.0f / 255.0f, 188.0f / 255.0f);
            }
        }
        {
            selfcoinTx.text = "";
        }
            //捐钱
            buyfriendspanel_.SetActive(false);
            {
                CCustomDialog.OpenCustomConfirmUI(1501);
                return;
            }
                llr_.maxPower = LandLordRule.maxPowerList[temp]; });
                ter_.vectory = ThrowEggsRule.victoryList[temp]; });
                ter_.score = ThrowEggsRule.scoreList[temp]; });
                ter_.cp = (CurrentPoker)temp; });

        if (mjr_ == null)
            mjr_ = new MahjongRule();

        GameObject mjfirstGroup = right.transform.Find("rule_12/jushu/ImageBG").gameObject;
        {
            Toggle toggle = mjfirstGroup.transform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                mjr_.times = MahjongRule.timesList[temp];
            });
        }
        {
            Toggle toggle = mjsecondGroup.transform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                mjr_.maxPower = MahjongRule.maxPowerList[temp];
            });
        }
        {
            Toggle toggle = mjthirdGroup.transform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;
            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                if (temp == 0)
                    mjr_.isAddBet = value;
                if (temp == 1)
                    mjr_.isAddBet = !value;
            });
        }
        {
            Toggle toggle = mjforthGroup.transform.GetChild(index).gameObject.GetComponent<Toggle>();
            int temp = index;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                CustomAudio.GetInstance().PlayCustomAudio(1004);
                mjr_.isOtherFour = value;
            });
        }
        toggle0.onValueChanged.AddListener(delegate (bool value)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1004);
            mjr_.isOneNine = value;
        });

        Toggle toggle1 = mjfifthGroup.transform.GetChild(1).gameObject.GetComponent<Toggle>();
        toggle1.onValueChanged.AddListener(delegate (bool value)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1004);
            mjr_.isMiddle = value;
        });

        Toggle toggle2 = mjfifthGroup.transform.GetChild(2).gameObject.GetComponent<Toggle>();
        toggle2.onValueChanged.AddListener(delegate (bool value)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1004);
            mjr_.isSkyGroud = value;
        });
            createmsg.Add(playtimesindex_);

            switch ((GameKind_Enum)AppointmentDataManager.AppointmentDataInstance().gameid)
            {
                case GameKind_Enum.GameKind_LandLords:
                    createmsg.Add(llr_.playTimes);
                    createmsg.Add(llr_.maxPower);
                    break;
                case GameKind_Enum.GameKind_GuanDan:
                    createmsg.Add((byte)ter_.playType);
                    if (ter_.playType == TePlayType.Goon)
                    {
                        createmsg.Add(ter_.vectory);
                    }
                    if (ter_.playType == TePlayType.Times)
                    {
                        createmsg.Add(ter_.times);
                        createmsg.Add(ter_.score);
                        createmsg.Add((byte)ter_.cp);
                    }
                    break;
                case GameKind_Enum.GameKind_Mahjong:
                    createmsg.Add(mjr_.times);
                    createmsg.Add(mjr_.maxPower);
                    int playtype = 0;
                    if (mjr_.isAddBet)
                        GameKind.AddFlag(0, ref playtype);
                    if (mjr_.isOtherFour)
                        GameKind.AddFlag(1, ref playtype);
                    if (mjr_.isOneNine)
                        GameKind.AddFlag(2, ref playtype);
                    if (mjr_.isMiddle)
                        GameKind.AddFlag(3, ref playtype);
                    if (mjr_.isSkyGroud)
                        GameKind.AddFlag(4, ref playtype);
                    createmsg.Add((ushort)playtype);
                    break;
            }
   

        group.transform.Find("ImageBGcost/ImageIcon_jlb").gameObject.SetActive(true);
                case 2:
                    AppointmentDataManager.AppointmentDataInstance().gameid = (byte)GameKind_Enum.GameKind_Mahjong;
            CustomAudio.GetInstance().PlayCustomAudio(1002);
        else
            name.text = FriendsMomentsDataMamager.GetFriendsInstance().memberList[playerid].memberName;
        Text master = item.transform.Find("TextDashi").gameObject.GetComponent<Text>();
            {
                UMessage breakmsg = new UMessage((uint)GameCity.EMSG_ENUM.Friends_Moments_CM_Break);

                breakmsg.Add(GameMain.hall_.GetPlayerData().momentsid);

                NetWorkClient.GetInstance().SendMsg(breakmsg);
            }
            else
            {
                UMessage exitmsg = new UMessage((uint)GameCity.EMSG_ENUM.Friends_Moments_CM_Exit);

                exitmsg.Add(GameMain.hall_.GetPlayerId());
                exitmsg.Add(GameMain.hall_.GetPlayerData().momentsid);

                NetWorkClient.GetInstance().SendMsg(exitmsg);
            }
        {
            if (joinobjects_[key] != null)
            {
                joinobjects_[key].SetActive(false);
                GameMain.safeDeleteObj(joinobjects_[key]);
            }
        }

            DelayJoinPlayer(memberid);
        }
    {
        if (joinobjects_.ContainsKey(memberid))
        {
            if (joinobjects_[memberid] != null)
            {
                joinobjects_[memberid].SetActive(false);
                GameMain.safeDeleteObj(joinobjects_[memberid]);
            }

            joinobjects_.Remove(memberid);
        }

        if (FriendsMomentsDataMamager.GetFriendsInstance().joinList.ContainsKey(memberid))
            FriendsMomentsDataMamager.GetFriendsInstance().joinList.Remove(memberid);

        RefreshJoinSpot();
    }

    private void OnAgreeJoin(EventTriggerType eventtype, object button, PointerEventData eventData)
        //Text contentTx = main_.transform.FindChild("middle/ImageBG_notice/Text_notice").gameObject.GetComponent<Text>();
        //contentTx.text = FriendsMomentsDataMamager.GetFriendsInstance().content;

        main_.transform.Find("Pop-up/set/ButtonGroupMask/ButtonGroup/Button_tuichu").
        main_.transform.Find("Pop-up/set/ButtonGroupMask/ButtonGroup/Button_jiesan").
            gameObject.SetActive(FriendsMomentsDataMamager.GetFriendsInstance().hostid == GameMain.hall_.GetPlayerId());
    {
        if (main_ == null)
            return;

        GameObject spot = main_.transform.Find("Top/ButtonSet/ImageSpot").gameObject;
        spot.SetActive(FriendsMomentsDataMamager.GetFriendsInstance().joinList.Count != 0 &&
            FriendsMomentsDataMamager.GetFriendsInstance().hostid == GameMain.hall_.GetPlayerId());

        GameObject nextspot = main_.transform.Find("Pop-up/set/ButtonGroupMask/ButtonGroup/Button_shenqing/ImageSpot").gameObject;
        nextspot.SetActive(FriendsMomentsDataMamager.GetFriendsInstance().joinList.Count != 0 &&
            FriendsMomentsDataMamager.GetFriendsInstance().hostid == GameMain.hall_.GetPlayerId());
    }
        if (main_ == null)
            return;

        GameObject BgObj = main_.transform.Find("middle/friend_table_Viewport/Content").gameObject;
        {
            if (tableobjects_[key] != null)
            {
                tableobjects_[key].SetActive(false);
                GameMain.safeDeleteObj(tableobjects_[key]);
            }
        }

        if (table.gameid == (byte)GameKind_Enum.GameKind_LandLords)
            item.transform.Find("tableBG/player_3").gameObject.SetActive(false);

            if(table.gameid == (byte)GameKind_Enum.GameKind_LandLords)
            {
                if (index == 2)
            }

                {
                    icon.sprite = GameMain.hall_.GetIcon(FriendsMomentsDataMamager.GetFriendsInstance().memberList[table.seats_[index].playerid].url,
                        FriendsMomentsDataMamager.GetFriendsInstance().memberList[table.seats_[index].playerid].memberid,
                        FriendsMomentsDataMamager.GetFriendsInstance().memberList[table.seats_[index].playerid].faceid);
                }
                {
                    icon.sprite = GameMain.hall_.GetIcon(otherplayers[table.seats_[index].playerid].url,
                        otherplayers[table.seats_[index].playerid].memberid,
                        otherplayers[table.seats_[index].playerid].faceid);
                }
        //XPointEvent.AutoAddListener(sharebtn, OnShareTable, tempid);
        sharebtn.SetActive(false);
            if (!FriendsMomentsDataMamager.GetFriendsInstance().tableList.ContainsKey(tableid))
                return;

            //分享
            Player.ShareURLToWechat("http://gdcdn.qpdashi.com",
                CCsvDataManager.Instance.GameDataMgr.GetGameData(AppointmentDataManager.AppointmentDataInstance().gameid).GameName +
                " 房间密码:" + tableid.ToString() + " " + FriendsMomentsDataMamager.GetFriendsInstance().tableList[tableid].gamerule.Substring(3), false);
            tableobjects_ = new Dictionary<uint, GameObject>();
            delaybtn.SetActive(isshow);
            delaybtn.SetActive(false);
            {
                CCustomDialog.OpenCustomConfirmUI(1643);
            }