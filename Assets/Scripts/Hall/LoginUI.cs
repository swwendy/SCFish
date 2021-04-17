using System.Collections.Generic;
using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class CLoginUI{    public enum LoginType
    {
        LoginType_Default = 0,       
        LoginType_Guest,       //游客
        LoginType_Wechat,      //微信
        LoginTyee_WechatQRCode,//微信扫描二维码登录
        LoginType_Uniond,      //唯一编码登陆        
        LoginType_LastOne,     //系统上次登陆方式 
    }    private static CLoginUI instance = new CLoginUI();    private GameObject CanvasObj = null;    //游客状态的账号ID    private uint VisitorAccountId;    //绑定手机的账号ID    private uint BindMobileAccountId;    //验证码计时器    private CTimerPersecondCall MobileCodeTimer;

    //登陆网络重连计时器
    private CTimerCirculateCall LoginNetReconnectTimer;
    private LoginType enLoginType;    private bool bHavedGetGateServr;    public static CLoginUI Instance    {              get { return instance; }    }    public CLoginUI()    {        VisitorAccountId = 0;        BindMobileAccountId = 0;        MobileCodeTimer = null;        LoginNetReconnectTimer = null;        enLoginType = LoginType.LoginType_Guest;        bHavedGetGateServr = false;        RegitserMsgHandle();        InitLoginUIBtnEvent();        LoadAccountConfig();    }    private void RegitserMsgHandle()    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
           (uint)GameCity.EMSG_ENUM.CCGateMsg_BackPlayerLoginGame, BackGateSerIpPort);        CMsgDispatcher.GetInstance().RegMsgDictionary(
           (uint)GameCity.EMSG_ENUM.CCGateMsg_BackPlayerReqConnIdForWxQR, BackReqConnIdForWxQR);        CMsgDispatcher.GetInstance().RegMsgDictionary(
           (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BackPlayerWxQRAuthCode, BackPlayerWxQRAuthCode);        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERLOGINSUCCESS, LoginSuccess);        //登录成功
        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SENDGAMEMASTERSCORE, PlayerMasterScore);        //登录成功
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKPLAYERLOGIN, LoginFailed);           //登陆返回消息

        CMsgDispatcher.GetInstance().RegMsgDictionary(             (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SERVERCUTCONNECT, ServerCutConnect);      //顶号        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKCHECKINDENTIFYING, BackCheckIndentfying); //检测验证码的回复        CMsgDispatcher.GetInstance().RegMsgDictionary(            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKMOBILELOGIN, BackCheckMobileLogin); //检测验证码的回复

        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERLOGINFAILED, LoginFailed2);        //登录失败
            }    /// <summary>    /// 读取本地账号文件    /// </summary>    private void LoadAccountConfig()    {
#if !WINDOWS_GUEST        string accountInipath = GameDefine.AppPersistentDataPath + GameDefine.AccountInfoFile;        //检测文件是否存在        /*  if (!File.Exists(accountInipath))        {            Debug.Log("LoadAccountConfig no file ");            GetPlayerData().SetPlayerID(0);            return;        }        StreamReader sr = File.OpenText(path);        uint playerid;        uint.TryParse(sr.ReadLine(), out playerid);        GetPlayerData().SetPlayerID(playerid);        Debug.Log("LoadAccountConfig PlayerID:" + playerid);        sr.Close();        sr.Dispose();*/        INIParser IniFile = new INIParser();        IniFile.Open(accountInipath);        VisitorAccountId = IniFile.ReadUIntValue("Account", "VisitorId", 0);        BindMobileAccountId = IniFile.ReadUIntValue("Account", "BindPlayerId", 0);        IniFile.Close();
        CWechatUserAuth.GetInstance().ReadWeChatAuthInfo();
        //如果游客账号与绑定状态的账号ID一致说明上次登陆进行了绑定操作
        //if (VisitorAccountId == BindMobileAccountId && VisitorAccountId !=0)
        //    VisitorAccountId = 0;
        //Debug.Log("AccountConfig VisitorId:" + VisitorAccountId + ",BindPlayerId:" + BindMobileAccountId);
#endif    }    /// <summary>    /// 账号信息写入本地文件    /// </summary>    /// <param name="isloginSuccess">是否登陆成功后写账号文件</param>    private void WriteAccountConfig(bool isloginSuccess = true)    {
        string accountInipath = GameDefine.AppPersistentDataPath + GameDefine.AccountInfoFile;        INIParser IniFile = new INIParser();        IniFile.Open(accountInipath);        if (isloginSuccess)        {            if (GameMain.hall_.GetPlayerData().GetBindPhoneNumber() == 0)                VisitorAccountId = GameMain.hall_.GetPlayerId();            else                BindMobileAccountId = GameMain.hall_.GetPlayerId();
#if !WINDOWS_GUEST            IniFile.WriteValue("Account", "VisitorId", VisitorAccountId.ToString());            IniFile.WriteValue("Account", "BindPlayerId", BindMobileAccountId.ToString());#endif        }        else        {
#if !WINDOWS_GUEST            IniFile.WriteValue("Account", "BindPlayerId", BindMobileAccountId.ToString());#endif        }        IniFile.Close();
    }

    /// <summary>    /// 登陆流程有账号直接登陆，没账号显示登陆界面    /// </summary>    public void IntoLoginProcess()    {
#if UNITY_EDITOR 
        if (VisitorAccountId == 0 && BindMobileAccountId == 0)        {            LoadChooseLoginType();        }        else        {            RequestLogin();        }
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
        if (VisitorAccountId == 0 && BindMobileAccountId == 0)        {            LoadChooseLoginType();        }        else        {            RequestLogin();        }
#endif
    }

    /// <summary>
    /// 切换账号角色重置标记重新请求数据刷新界面
    /// </summary>    private void ChangeAccountResetAllFlag()
    {
        PlayerInfoUI.Instance.ChangeAccountResetDateFlag();
    }    /// <summary>
    /// 初始登陆界面按钮监听事件
    /// </summary>    public void InitLoginUIBtnEvent()
    {
        if (CanvasObj == null)            CanvasObj = GameObject.Find("Canvas/Root");        GameObject loginPanel = CanvasObj.transform.Find("Login").Find("Panel_Login").gameObject;
        GameObject agreementBtn = CanvasObj.transform.Find("Login").Find("Toggle_agreement")
           .Find("Label").gameObject;        XPointEvent.AutoAddListener(agreementBtn, OnClickAgreementBtn, null);        GameObject PhoeLoginObject = loginPanel.transform.Find("Button_PhoneLogin").gameObject;        XPointEvent.AutoAddListener(PhoeLoginObject, OnBtn_PhoneLogin, null);        GameObject TouristLoginObject = loginPanel.transform.Find("Button_TouristLogin").gameObject;        XPointEvent.AutoAddListener(TouristLoginObject, OnBtn_TouristLogin, null);
        TouristLoginObject.SetActive(true);
        GameObject closeagreenBtn = CanvasObj.transform.Find("Login").Find("Image_protocol")
            .Find("Button_Back").gameObject;
        XPointEvent.AutoAddListener(closeagreenBtn, OnClickCloseAgreement, null);
    }    /// <summary>    /// 首次登陆 选择登陆方式    /// </summary>    public void LoadChooseLoginType()    {        if (CanvasObj == null)            CanvasObj = GameObject.Find("Canvas/Root");        InitLoginUIBtnEvent();        //CanvasObj.transform.FindChild("Main_Loading").FindChild("ImageStripBG").gameObject.SetActive(false);        CanvasObj.transform.Find("Login").gameObject.SetActive(true);        CanvasObj.transform.Find("Login/Panel_PhoneLogin").gameObject.SetActive(false);        GameObject loginPanel = CanvasObj.transform.Find("Login").Find("Panel_Login").gameObject;        loginPanel.SetActive(true);

        //window平台下启用启用微信扫码登录
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR && !WINDOWS_GUEST        loginPanel.SetActive(false);
        RequestLogin(LoginType.LoginTyee_WechatQRCode);

#elif UNITY_EDITOR || (UNITY_STANDALONE_WIN && WINDOWS_GUEST)
        RequestLogin();
#else
        //ios 审核版本下要显示游客登陆按钮
        if (Luancher.IsReviewVersion && Application.platform == RuntimePlatform.IPhonePlayer)
        {
            loginPanel.transform.Find("Button_TouristLogin").gameObject.SetActive(true);
            //审核版本下微信未安装直接不显示微信登陆按钮（以免审核人员以登陆不应依赖第三方应用拒绝）
#if UNITY_IOS && !UNITY_EDITOR            if (!WechatPlatfrom_IOS.WeChat_IsWXAppInstalled())            {
              loginPanel.transform.FindChild("Button_PhoneLogin").gameObject.SetActive(false);
            }
#endif
        }#endif        CanvasObj.transform.Find("Login/Toggle_agreement").gameObject.SetActive(true);
        CanvasObj.transform.Find("Login/Panel_Login/Button_PhoneLogin").gameObject.SetActive(false);

        //切换账号把VisitorAccountId设置为0，避免切换账号登陆上次角色
#if !UNITY_EDITOR
        VisitorAccountId = 0;
#endif    }

    /// <summary>    /// 点击查看协议按钮    /// </summary>    /// <param name="eventtype"></param>    /// <param name="button"></param>    /// <param name="eventData"></param>    private void OnClickAgreementBtn(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            GameObject protocolObj = CanvasObj.transform.Find("Login").Find("Image_protocol").gameObject;            protocolObj.SetActive(!protocolObj.activeSelf);        }    }    /// <summary>
    /// 点击关闭协议界面按钮
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="button"></param>
    /// <param name="eventData"></param>    private void OnClickCloseAgreement(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            GameObject protocolObj = CanvasObj.transform.Find("Login").Find("Image_protocol").gameObject;            protocolObj.SetActive(false);
        }
    }    /// <summary>    /// 手机登陆    /// </summary>    void OnBtn_PhoneLogin(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            if (CanvasObj == null)                CanvasObj = GameObject.Find("Canvas/Root");

#if UNITY_IOS && !UNITY_EDITOR             if (WechatPlatfrom_IOS.WeChat_IsWXAppInstalled())             {                WechatPlatfrom_IOS.WeChat_AuthLogin();             }             else             {                //还末安装微信失败处理               
                CRollTextUI.Instance.AddVerticalRollText(1010);             }
#elif UNITY_ANDROID && !UNITY_EDITOR
            if (AlipayWeChatPay.IsWxAppInstalled())             {                AlipayWeChatPay.ReqWxLoginAuth();             }             else             {                //还末安装微信失败处理               
                CRollTextUI.Instance.AddVerticalRollText(1010);             }
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
            //WeChatAuthLogin();
            RequestLogin();
#endif             return;            CanvasObj.transform.Find("Login").Find("Panel_Login").gameObject.SetActive(false);            GameObject PhonePanel = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin").gameObject;            //返回按钮            GameObject btnreturn = PhonePanel.transform.Find("Button_Return").gameObject;            btnreturn.GetComponent<Image>().raycastTarget = true;            btnreturn.GetComponent<Button>().interactable = true;            //获取验证码按钮            GameObject codeBtn = PhonePanel.transform.Find("Button_GetCode").gameObject;            codeBtn.GetComponent<Image>().raycastTarget = true;            codeBtn.GetComponent<Button>().interactable = true;            Text strCode = codeBtn.transform.Find("Text_GetCode").gameObject.GetComponent<Text>();            strCode.text = "获取验证码";            //登陆按钮            GameObject loginBtn = PhonePanel.transform.Find("Button_FinishLogin").gameObject;            loginBtn.GetComponent<Image>().raycastTarget = true;            loginBtn.GetComponent<Button>().interactable = true;            PhonePanel.transform.Find("InputField_PhoneNum").gameObject.GetComponent<InputField>().text = "";            PhonePanel.transform.Find("InputField_Code").gameObject.GetComponent<InputField>().text = "";            PhonePanel.SetActive(true);            XPointEvent.AutoAddListener(codeBtn, OnBtn_GetCode, null);            XPointEvent.AutoAddListener(loginBtn, OnBtn_StartPhoneLogin, null);            XPointEvent.AutoAddListener(btnreturn, OnBtn_ReturnChooseType, null);                    }    }    /// <summary>    /// 游客登陆    /// </summary>    void OnBtn_TouristLogin(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);
            //检测是否同意协议
            //             Toggle agreementToggle = CanvasObj.transform.FindChild("Login").FindChild("Toggle_agreement")
            //                 .gameObject.GetComponent<Toggle>();
            //             if (!agreementToggle.isOn)
            //             {
            //                 CCustomDialog.OpenCustomConfirmUI(1007);
            //                 return;
            //             }
            //             CanvasObj.transform.FindChild("Login").gameObject.SetActive(false);
            RequestLogin();        }    }    /// <summary>    /// 获取验证码    /// </summary>    void OnBtn_GetCode(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            if (CanvasObj == null)                CanvasObj = GameObject.Find("Canvas/Root");            InputField PhonePanel = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin")                .Find("InputField_PhoneNum").gameObject.GetComponent<InputField>();            long phonenum = 0;            if (!long.TryParse(PhonePanel.text, out phonenum))            {                CCustomDialog.OpenCustomConfirmUI(1003);                return;            }            if (!GameCommon.CheckPhoneIsAble(PhonePanel.text))            {                CCustomDialog.OpenCustomConfirmUI(1003);                return;            }            if (phonenum > 0)            {                if (!NetWorkClient.GetInstance().IsSocketConnected)                    HallMain.ConnectLoginServer();                SendApplyGetCode(phonenum, 1);                //if(MobileCodeTimer == null)                MobileCodeTimer = new CTimerPersecondCall(60f, UpdataGetCodeBtnText);                xTimeManger.Instance.RegisterTimer(MobileCodeTimer);                GameObject PhoneLogin = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin").gameObject;                if (null != PhoneLogin)                {                    Button btn = PhoneLogin.transform.Find("Button_GetCode").gameObject.GetComponent<Button>();                    Image btnimg = PhoneLogin.transform.Find("Button_GetCode").gameObject.GetComponent<Image>();                    btn.interactable = false;                    btnimg.raycastTarget = false;                    Text strCode = PhoneLogin.transform.Find("Button_GetCode").Find("Text_GetCode").gameObject.GetComponent<Text>();                    strCode.text = "60S可重新获取";                }            }        }    }    /// <summary>    /// 登陆中的时候 更新 获取验证码按钮上的文字显示    /// </summary>    void UpdataGetCodeBtnText(object[] RemainTime)    {        if (CanvasObj == null)            return;        float fRemaintime = (float)RemainTime[0];        GameObject PhoneLogin = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin").gameObject;        if (null != PhoneLogin)        {            Text strCode = PhoneLogin.transform.Find("Button_GetCode").Find("Text_GetCode").gameObject.GetComponent<Text>();            if (fRemaintime > 1)                strCode.text = fRemaintime.ToString("f0") + "S可重新获取";            else            {                PhoneLogin.transform.Find("Button_GetCode").gameObject.GetComponent<Image>().raycastTarget = true;                PhoneLogin.transform.Find("Button_GetCode").gameObject.GetComponent<Button>().interactable = true;                strCode.text = "获取验证码";            }        }    }    /// <summary>    /// 请求发送验证码    /// </summary>    public void SendApplyGetCode(long nPhoneNum, byte nState)    {
        if (!NetWorkClient.GetInstance().IsSocketConnected)            HallMain.ConnectLoginServer();

        UMessage LoginMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_MOBILELOGIN);        MobileLogin_Msg msg_login = new MobileLogin_Msg();        msg_login.nState = nState;        msg_login.nMobileNum = nPhoneNum;        msg_login.SetSendData(LoginMsg);        NetWorkClient.GetInstance().SendMsg(LoginMsg);    }    /// <summary>    /// 开始手机登陆    /// </summary>    void OnBtn_StartPhoneLogin(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype != EventTriggerType.PointerClick)            return;        if (CanvasObj == null)            CanvasObj = GameObject.Find("Canvas/Root");        //检测是否同意协议        Toggle agreementToggle = CanvasObj.transform.Find("Login").Find("Toggle_agreement")            .gameObject.GetComponent<Toggle>();        if (!agreementToggle.isOn)        {            CCustomDialog.OpenCustomConfirmUI(1007);            return;        }        InputField CodePanel = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin").Find("InputField_Code").gameObject.GetComponent<InputField>();        uint nCode = 0;        if (!uint.TryParse(CodePanel.text, out nCode))        {            CCustomDialog.OpenCustomConfirmUI(1002);            return;        }        if (nCode > 0)        {            SendCodeApplyLogin(nCode, 1);            GameObject rtnBtn = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin").Find("Button_Return").gameObject;            rtnBtn.GetComponent<Image>().raycastTarget = false;            rtnBtn.GetComponent<Button>().interactable = false;            GameObject loginBtn = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin").Find("Button_FinishLogin").gameObject;            loginBtn.GetComponent<Image>().raycastTarget = false;            loginBtn.GetComponent<Button>().interactable = false;        }        CanvasObj.transform.Find("Login").gameObject.SetActive(false);    }    /// <summary>    /// 发送验证码请求登陆    /// nState = 1登陆 2绑定 3解绑    /// </summary>    public void SendCodeApplyLogin(uint ncode, byte nState)    {
        if (!NetWorkClient.GetInstance().IsSocketConnected)            HallMain.ConnectLoginServer();        UMessage LoginMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CHECKINDENTIFYING);        CheckCodeLogin_Msg msg_Check = new CheckCodeLogin_Msg();        if (Application.platform == RuntimePlatform.IPhonePlayer)            msg_Check.nPlatform = 1;        else if (Application.platform == RuntimePlatform.Android)            msg_Check.nPlatform = 2;        else            msg_Check.nPlatform = 0;        msg_Check.nBindOrLogin = nState;        msg_Check.nCode = ncode;        msg_Check.smachinecode = SystemInfo.deviceUniqueIdentifier;        msg_Check.SetSendData(LoginMsg);        NetWorkClient.GetInstance().SendMsg(LoginMsg);    }

    /// <summary>    /// 请求微信登录    /// </summary>    private void WeChatAuthLogin()
    {
        UMessage vchatmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERLOGIN);
        MessageLogin ml_ = new MessageLogin();
        Luancher.IsVChatLogin = true;
        ml_.nUseID = VisitorAccountId;        ml_.smachinecode = SystemInfo.deviceUniqueIdentifier;        ml_.sVersion = GameMain.Instance.GetAppVersion();        if (Application.platform == RuntimePlatform.IPhonePlayer)            ml_.nPlatform = 1;        else if (Application.platform == RuntimePlatform.Android)            ml_.nPlatform = 2;        else            ml_.nPlatform = 0;
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
    }    /// <summary>
    /// unionid码登陆
    /// </summary>
    /// <param name="unionid"></param>    private void UnionIdLogin(string unionid)
    {
        if (!NetWorkClient.GetInstance().IsSocketConnected)            HallMain.ConnectLoginServer();        CCustomDialog.OpenCustomWaitUI(1008);

        UMessage unionLoginMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERLOGIN);        MessageLogin ml_ = new MessageLogin();        ml_.smachinecode = SystemInfo.deviceUniqueIdentifier;        ml_.sVersion = GameMain.Instance.GetAppVersion();        if (Application.platform == RuntimePlatform.IPhonePlayer)            ml_.nPlatform = 1;        else if (Application.platform == RuntimePlatform.Android)            ml_.nPlatform = 2;        else            ml_.nPlatform = 0;
        unionLoginMsg.Add((int)0);
        unionLoginMsg.Add(ml_.smachinecode);
        unionLoginMsg.Add(ml_.sVersion);
        unionLoginMsg.Add(ml_.nPlatform);
        unionLoginMsg.Add((byte)1);
        unionLoginMsg.Add(unionid);
        //unionLoginMsg.Add();
        //unionLoginMsg.Add(CWechatUserAuth.GetInstance().GetUserNickname());

        NetWorkClient.GetInstance().SendMsg(unionLoginMsg);
    }    private void GuestLogin()
    {
        UMessage LoginMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERLOGIN);        MessageLogin ml_ = new MessageLogin();        Luancher.IsVChatLogin = false;        ml_.nUseID = VisitorAccountId;             ml_.smachinecode = SystemInfo.deviceUniqueIdentifier;        ml_.sVersion = GameMain.Instance.GetAppVersion();        if (Application.platform == RuntimePlatform.IPhonePlayer)            ml_.nPlatform = 1;        else if (Application.platform == RuntimePlatform.Android)            ml_.nPlatform = 2;        else            ml_.nPlatform = 0;        ml_.bLoadType = 0;        ml_.SetSendData(LoginMsg);        NetWorkClient.GetInstance().SendMsg(LoginMsg);
    }    /// <summary>    /// 返回选择登陆方式    /// </summary>    void OnBtn_ReturnChooseType(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            InputField CodePanel = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin").Find("InputField_Code").gameObject.GetComponent<InputField>();            CodePanel.text = "";            LoadChooseLoginType();        }    }    /// <summary>    /// 请求登陆    /// </summary>   public void RequestLogin(LoginType logintype = LoginType.LoginType_Guest)    {
        if (logintype != LoginType.LoginType_LastOne)
            enLoginType = logintype;

        CCustomDialog.OpenCustomWaitUI(1008);        if (!NetWorkClient.GetInstance().IsSocketConnected)
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
        }    }

    private void LoginNetReconnectCallBack(object param)
    {
        if(HallMain.ConnectLoginServer())
        {
            LoginNetReconnectTimer.SetDeleteFlag(true);            
        }
    }

    bool BackCheckIndentfying(uint _msgType, UMessage _ms)    {        byte nState = _ms.ReadByte();        if ((nState % 10) == 0)        {            //检测失败            if ((nState / 10) == 0)            {                CanvasObj.transform.Find("Login").gameObject.SetActive(true);                GameObject rtnBtn = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin").Find("Button_Return").gameObject;                rtnBtn.GetComponent<Image>().raycastTarget = true;                rtnBtn.GetComponent<Button>().interactable = true;                GameObject loginBtn = CanvasObj.transform.Find("Login").Find("Panel_PhoneLogin").Find("Button_FinishLogin").gameObject;                loginBtn.GetComponent<Image>().raycastTarget = true;                loginBtn.GetComponent<Button>().interactable = true;            }            CCustomDialog.OpenCustomConfirmUI(1002);        }        else        {            long nMobile = _ms.ReadLong();            GameMain.hall_.GetPlayerData().SetBindPhoneNumber(nMobile);            BindMobileAccountId = GameMain.hall_.GetPlayerId();            WriteAccountConfig(false);            //直接手机号登陆不提示绑定成功            if (nState == 11)            {                CCustomDialog.OpenCustomConfirmUI(1012);                PlayerInfoUI.Instance.HandleBindMobileResult(true);            }            else                MobileCodeTimer.SetDeleteFlag(true);            Debug.Log("验证码验证成功！登陆游戏！" + nMobile.ToString());        }        return true;    }    /// <summary>    /// 顶号处理    /// </summary>    /// <param name="_msgType"></param>    /// <param name="_ms"></param>    /// <returns></returns>    bool ServerCutConnect(uint _msgType, UMessage _ms)    {        //清除本地保存的绑定账号         BindMobileAccountId = 0;        WriteAccountConfig(false);        HallMain.bDisconnectReconnection = true;//切断重连机制
        CCustomDialog.OpenCustomConfirmUI(1015, (p)=> 
        {
            GameMain.hall_.AnyWhereBackToLoginUI();
        });        return false;    }

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
        //if (CanvasObj == null)        //    CanvasObj = GameObject.Find("Canvas/Root");

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


    /// <summary>    /// 登陆成功处理    /// </summary>    /// <param name="_msgType"></param>    /// <param name="_ms"></param>    /// <returns></returns>    bool LoginSuccess(uint _msgType, UMessage _ms)    {        PlayerData playerdata = GameMain.hall_.GetPlayerData();        
        uint lastLoginPlayerId = playerdata.GetPlayerID();               playerdata.ReadPlayerData(_ms, ref CRankUI.Instance.m_fLeftCoinRankTime);
        //检测是否切换角色了
        if(lastLoginPlayerId !=0 && lastLoginPlayerId != playerdata.GetPlayerID())
        {

        }
        GameMain.hall_.IntelentType = Application.internetReachability;        WriteAccountConfig();        //GameMain.hall_.SendGetCoinRankData();        //m_bIsSendMobileNum = false;        CCustomDialog.CloseCustomWaitUI();
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

        UMessage activemsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_GETTODAYREDBAGINFO);        activemsg.Add(GameMain.hall_.GetPlayerId());        NetWorkClient.GetInstance().SendMsg(activemsg);        BagDataManager.GetBagDataInstance();        if (GameMain.hall_.GetPlayerData().itemNumber > 0)        {            UMessage bagmsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_REQUESTPLAYERPACKETINFO);            bagmsg.Add(GameMain.hall_.GetPlayerId());            NetWorkClient.GetInstance().SendMsg(bagmsg);        }

        PlayerInfoUI.Instance.Ask4PlayerTotalData();

        return true;    }    private bool PlayerMasterScore(uint _msgType, UMessage _ms)
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
    }    /// <summary>
    /// 登陆失败返回消息处理
    /// </summary>
    /// <param name="_msgType"></param>
    /// <param name="_ms"></param>
    /// <returns></returns>    private bool LoginFailed(uint _msgType, UMessage _ms)
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
    }    bool BackCheckMobileLogin(uint _msgType, UMessage _ms)    {        //1获取成功        // 2改账号已经有绑定手机        // 3该手机号已经绑定账号        byte nState = _ms.ReadByte();        if (nState == 1)        {            Debug.Log("该手机号没有绑定任何号");        }        else if (nState == 2)        {            //Debug.Log("该账号已经有绑定手机");            CCustomDialog.OpenCustomConfirmUI(1009);        }        else if (nState == 3)        {            Debug.Log("该手机号已经绑定账号");            CCustomDialog.OpenCustomConfirmUI(1005);            PlayerInfoUI.Instance.HandleBindMobileResult(false);        }        return true;    }

    /// <summary>
    /// 设置游客状态的账号ID
    /// </summary>    public void SetVisitorAccountId(uint Id)
    {
        VisitorAccountId = Id;
    }}