﻿
using UnityEngine;
    {
        LoginType_Default = 0,       
        LoginType_Guest,       //游客
        LoginType_Wechat,      //微信
        LoginTyee_WechatQRCode,//微信扫描二维码登录
        LoginType_Uniond,      //唯一编码登陆        
        LoginType_LastOne,     //系统上次登陆方式 
    }

    //登陆网络重连计时器
    private CTimerCirculateCall LoginNetReconnectTimer;

        CMsgDispatcher.GetInstance().RegMsgDictionary(
           (uint)GameCity.EMSG_ENUM.CCGateMsg_BackPlayerLoginGame, BackGateSerIpPort);
           (uint)GameCity.EMSG_ENUM.CCGateMsg_BackPlayerReqConnIdForWxQR, BackReqConnIdForWxQR);
           (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BackPlayerWxQRAuthCode, BackPlayerWxQRAuthCode);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKPLAYERLOGIN, LoginFailed);           //登陆返回消息

        CMsgDispatcher.GetInstance().RegMsgDictionary(

        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERLOGINFAILED, LoginFailed2);        //登录失败
        
#if !WINDOWS_GUEST


        //if (VisitorAccountId == BindMobileAccountId && VisitorAccountId !=0)
        //    VisitorAccountId = 0;
        //Debug.Log("AccountConfig VisitorId:" + VisitorAccountId + ",BindPlayerId:" + BindMobileAccountId);
#endif
        string accountInipath = GameDefine.AppPersistentDataPath + GameDefine.AccountInfoFile;
#if !WINDOWS_GUEST
#if !WINDOWS_GUEST
    }

    /// <summary>
#if UNITY_EDITOR 
        if (VisitorAccountId == 0 && BindMobileAccountId == 0)
#elif UNITY_STANDALONE_WIN
#if WINDOWS_GUEST
        uint accountId = 0;
        string []CommandLine = System.Environment.CommandLine.Split('-');
        if(CommandLine.Length >= 2)
        {
            string nameValue = "ID =";
            int separatorIndex = CommandLine[1].IndexOf("ID =") + nameValue.Length + 1;
            string AccountIdName = CommandLine[1].Substring(separatorIndex, CommandLine[1].Length - separatorIndex);
            if (separatorIndex != -1 && uint.TryParse(AccountIdName, out accountId))
            {
                CLoginUI.Instance.SetVisitorAccountId(accountId);
                CLoginUI.Instance.RequestLogin();
            }
            else
            {
                LoadChooseLoginType();
            }
        }
        else
        {
            LoadChooseLoginType();
        }
#else
        //如果已经微信授权用户数据直接登陆       
        if(!string.IsNullOrEmpty(CWechatUserAuth.GetInstance().GetUserUnionId()))
        {
           RequestLogin(LoginType.LoginType_Wechat);
        }
        else
        { 
           LoadChooseLoginType();           
        }
#endif
#else
        ////如果已经微信授权用户数据直接登陆       
        //if(!string.IsNullOrEmpty(CWechatUserAuth.GetInstance().GetUserUnionId()))
        //{
        //   RequestLogin(LoginType.LoginType_Wechat);
        //}
        //else
        //{ 
        //   LoadChooseLoginType();
        //   //RequestLogin();
        //}
        if (VisitorAccountId == 0 && BindMobileAccountId == 0)
#endif
    }

    /// <summary>
    /// 切换账号角色重置标记重新请求数据刷新界面
    /// </summary>
    {
        PlayerInfoUI.Instance.ChangeAccountResetDateFlag();
    }
    /// 初始登陆界面按钮监听事件
    /// </summary>
    {
        if (CanvasObj == null)
        GameObject agreementBtn = CanvasObj.transform.Find("Login").Find("Toggle_agreement")
           .Find("Label").gameObject;
        TouristLoginObject.SetActive(true);
        GameObject closeagreenBtn = CanvasObj.transform.Find("Login").Find("Image_protocol")
            .Find("Button_Back").gameObject;
        XPointEvent.AutoAddListener(closeagreenBtn, OnClickCloseAgreement, null);
    }

        //window平台下启用启用微信扫码登录
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR && !WINDOWS_GUEST
        RequestLogin(LoginType.LoginTyee_WechatQRCode);

#elif UNITY_EDITOR || (UNITY_STANDALONE_WIN && WINDOWS_GUEST)
        RequestLogin();
#else
        //ios 审核版本下要显示游客登陆按钮
        if (Luancher.IsReviewVersion && Application.platform == RuntimePlatform.IPhonePlayer)
        {
            loginPanel.transform.Find("Button_TouristLogin").gameObject.SetActive(true);
            //审核版本下微信未安装直接不显示微信登陆按钮（以免审核人员以登陆不应依赖第三方应用拒绝）
#if UNITY_IOS && !UNITY_EDITOR
              loginPanel.transform.FindChild("Button_PhoneLogin").gameObject.SetActive(false);
            }
#endif
        }
        CanvasObj.transform.Find("Login/Panel_Login/Button_PhoneLogin").gameObject.SetActive(false);

        //切换账号把VisitorAccountId设置为0，避免切换账号登陆上次角色
#if !UNITY_EDITOR
        VisitorAccountId = 0;
#endif

    /// <summary>
    /// 点击关闭协议界面按钮
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="button"></param>
    /// <param name="eventData"></param>
    {
        if (eventtype == EventTriggerType.PointerClick)
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            GameObject protocolObj = CanvasObj.transform.Find("Login").Find("Image_protocol").gameObject;
        }
    }

#if UNITY_IOS && !UNITY_EDITOR
                CRollTextUI.Instance.AddVerticalRollText(1010);
#elif UNITY_ANDROID && !UNITY_EDITOR
            if (AlipayWeChatPay.IsWxAppInstalled())
                CRollTextUI.Instance.AddVerticalRollText(1010);
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
            //WeChatAuthLogin();
            RequestLogin();
#endif
            //检测是否同意协议
            //             Toggle agreementToggle = CanvasObj.transform.FindChild("Login").FindChild("Toggle_agreement")
            //                 .gameObject.GetComponent<Toggle>();
            //             if (!agreementToggle.isOn)
            //             {
            //                 CCustomDialog.OpenCustomConfirmUI(1007);
            //                 return;
            //             }
            //             CanvasObj.transform.FindChild("Login").gameObject.SetActive(false);
            RequestLogin();
        if (!NetWorkClient.GetInstance().IsSocketConnected)

        UMessage LoginMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_MOBILELOGIN);
        if (!NetWorkClient.GetInstance().IsSocketConnected)

    /// <summary>
    {
        UMessage vchatmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERLOGIN);

        Luancher.IsVChatLogin = true;
        ml_.nUseID = VisitorAccountId;
        vchatmsg.Add(ml_.nUseID);
        vchatmsg.Add(ml_.smachinecode);
        vchatmsg.Add(ml_.sVersion);
        vchatmsg.Add(ml_.nPlatform);
        vchatmsg.Add((byte)2);  //登陆方式标记 1 运营渠道 2 微信
        vchatmsg.Add(CWechatUserAuth.GetInstance().GetUserUnionId());
        vchatmsg.Add(CWechatUserAuth.GetInstance().GetUserHeadImgUrl());
        vchatmsg.Add(CWechatUserAuth.GetInstance().GetUserNickname());
        vchatmsg.Add(CWechatUserAuth.GetInstance().GetUserSex());
        NetWorkClient.GetInstance().SendMsg(vchatmsg);
    }
    /// unionid码登陆
    /// </summary>
    /// <param name="unionid"></param>
    {
        if (!NetWorkClient.GetInstance().IsSocketConnected)

        UMessage unionLoginMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERLOGIN);
        unionLoginMsg.Add((int)0);
        unionLoginMsg.Add(ml_.smachinecode);
        unionLoginMsg.Add(ml_.sVersion);
        unionLoginMsg.Add(ml_.nPlatform);
        unionLoginMsg.Add((byte)1);
        unionLoginMsg.Add(unionid);
        //unionLoginMsg.Add();
        //unionLoginMsg.Add(CWechatUserAuth.GetInstance().GetUserNickname());

        NetWorkClient.GetInstance().SendMsg(unionLoginMsg);
    }
    {
        UMessage LoginMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERLOGIN);
    }
        if (logintype != LoginType.LoginType_LastOne)
            enLoginType = logintype;

        CCustomDialog.OpenCustomWaitUI(1008);
        {
            if (!HallMain.ConnectLoginServer())
            {
                if (LoginNetReconnectTimer == null)
                    LoginNetReconnectTimer = new CTimerCirculateCall(3.0f,LoginNetReconnectCallBack);
                xTimeManger.Instance.RegisterTimer(LoginNetReconnectTimer);
                return;
            }
        }
        if(bHavedGetGateServr)
        {
            if (enLoginType == LoginType.LoginType_Guest)
                GuestLogin();
            else if (enLoginType == LoginType.LoginType_Wechat)
                WeChatAuthLogin();
            else if (enLoginType == LoginType.LoginTyee_WechatQRCode)
                WeChatAuthLogin();
        }

    private void LoginNetReconnectCallBack(object param)
    {
        if(HallMain.ConnectLoginServer())
        {
            LoginNetReconnectTimer.SetDeleteFlag(true);            
        }
    }

    bool BackCheckIndentfying(uint _msgType, UMessage _ms)
        CCustomDialog.OpenCustomConfirmUI(1015, (p)=> 
        {
            GameMain.hall_.AnyWhereBackToLoginUI();
        });

    bool BackGateSerIpPort(uint _msgType, UMessage _ms)
    {
        byte nState = _ms.ReadByte();
        if (nState > 0)
        {
            string sip = _ms.ReadString();
            int nPort = _ms.ReadInt();

            Debug.Log("Get gateSer Ip:" + sip + ",Port:" + nPort);

            NetWorkClient.GetInstance().CloseNetwork();

            if(NetWorkClient.GetInstance().InitNetWork(sip, nPort))
            {
                bHavedGetGateServr = true;
                if (enLoginType == LoginType.LoginType_Guest)
                    GuestLogin();
                else if (enLoginType == LoginType.LoginType_Wechat)
                    WeChatAuthLogin();
                else if (enLoginType == LoginType.LoginTyee_WechatQRCode)
                {
                    UMessage ReqConnIdMsg = new UMessage((uint)GameCity.EMSG_ENUM.CCGateMsg_PlayerReqConnIdForWxQR);
                    NetWorkClient.GetInstance().SendMsg(ReqConnIdMsg);
                }
            }
            else
                Debug.Log("Can not connect gateser!");
        }
        else
        {
            Debug.Log("Can not get gateser ip and port!");
        }

        return true;
    }

    bool BackReqConnIdForWxQR(uint _msgType, UMessage _ms)
    {
        uint connId = _ms.ReadUInt();
        uint GateLoginConnId = _ms.ReadUInt();
        CCustomDialog.CloseCustomWaitUI();
        //申请微信登录扫描登录二维码    
        //string QRCodeurl =  CWechatUserAuth.GetInstance().ReqBuildAuthQRcode(connId, GateLoginConnId);
        //if (CanvasObj == null)

        //GameObject QRCodePanel = CanvasObj.transform.FindChild("Login").FindChild("ImageQRcode").gameObject;
        //QRCodePanel.SetActive(true);
        //WebBrowser2D browser = QRCodePanel.transform.FindChild("ImageIcon").GetComponent<WebBrowser2D >();
        //browser.OnNavigate(QRCodeurl);
        return true;
    }


    bool BackPlayerWxQRAuthCode(uint _msgType, UMessage _ms)
    {       
        //微信扫描二维码登录授权后code
        string QRAuthCode = _ms.ReadString();
        CWechatUserAuth.GetInstance().GetAuthUserInfo(QRAuthCode);
        return true;
    }


    /// <summary>
        uint lastLoginPlayerId = playerdata.GetPlayerID();       
        //检测是否切换角色了
        if(lastLoginPlayerId !=0 && lastLoginPlayerId != playerdata.GetPlayerID())
        {

        }

        GameMain.hall_.AfterLogin();
        //GameMain.hall_.LoadHallResource();
        if (playerdata.nGameMode_Before >= 0)
        {
            Debug.Log("断线重连GameId:" + playerdata.nGameKind_Before + ",GameMode:" + playerdata.nGameMode_Before);
            string strSign = string.Empty;
            GameKind_Enum CurbeforeGameKind = (GameKind_Enum)playerdata.nGameKind_Before;
            if (playerdata.nGameMode_Before == 0)
            {
                CurbeforeGameKind = (GameKind_Enum)(Mathf.Log(playerdata.nGameKind_Before, 2));
                for(GameKind_Enum gameKind = GameKind_Enum.GameKind_CarPort; gameKind < GameKind_Enum.GameKind_Max; ++gameKind)
                {
                    if(gameKind != GameKind_Enum.GameKind_LandLords && gameKind != GameKind_Enum.GameKind_Mahjong &&
                       gameKind != GameKind_Enum.GameKind_GuanDan && gameKind != GameKind_Enum.GameKind_YcMahjong &&
                       gameKind != GameKind_Enum.GameKind_CzMahjong && gameKind != GameKind_Enum.GameKind_GouJi &&
                       gameKind != GameKind_Enum.GameKind_HongZhong && gameKind != GameKind_Enum.GameKind_Answer &&
                       gameKind != GameKind_Enum.GameKind_Chess)
                    {
                        if (GameKind.HasFlag((int)gameKind, (int)playerdata.nGameKind_Before))
                        {
                            GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)CurbeforeGameKind);
                            if (gamedata != null)
                            {
                                strSign += gamedata.GameName;
                            }else
                            {
                                strSign += "未知游戏";
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(strSign))
            {
                CCustomDialog.OpenCustomWaitUI(2014, strSign);
            }
            else
            {
                if(GameMain.hall_.contestui_)
                {
                    GameMain.hall_.contestui_.SetActive(false);
                }
                GameMain.hall_.ReconnectLoadGame(CurbeforeGameKind, (GameTye_Enum)playerdata.nGameMode_Before);
            }
        }
        else
        {
            if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                AlipayWeChatPay.GetAndroidActivity().Call("GetGameRoomID");               
#endif
            }
        }

        GameMain.hall_.ReconnectGameServer();

        UMessage activemsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_GETTODAYREDBAGINFO);

        PlayerInfoUI.Instance.Ask4PlayerTotalData();

        return true;
    {
        uint nUseid = _ms.ReadUInt();
        byte nNum = _ms.ReadByte();

        byte nKind = 0;
        float nScore = 0.0f;
        for(int i=0; i<nNum; i++)
        {
            nKind = _ms.ReadByte();
            nScore = _ms.ReadSingle();

            GameMain.hall_.GetPlayerData().MasterScoreKindArray[nKind] = nScore;

            Debug.Log("第一个登陆 获取玩家大师分 nKind:" + nKind + " nScore:" + nScore);
        }

        return true;
    }
    /// 登陆失败返回消息处理
    /// </summary>
    /// <param name="_msgType"></param>
    /// <param name="_ms"></param>
    /// <returns></returns>
    {
        CCustomDialog.CloseCustomWaitUI();
        byte errState = _ms.ReadByte();

        switch(errState)
        {
            //app版本不符
            case 1:
                {
                    CCustomDialog.OpenCustomConfirmUI(1019);
                }
                break;
        }
        return true;
    }

    private bool LoginFailed2(uint _msgType, UMessage _ms)
    {
        byte errState = _ms.ReadByte();

        switch (errState)
        {
            //被封号
            case 1:
                {
                    CCustomDialog.OpenCustomConfirmUI(1023);
                }
                break;
        }
        return true;
    }

    /// <summary>
    /// 设置游客状态的账号ID
    /// </summary>
    {
        VisitorAccountId = Id;
    }