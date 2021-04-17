using UnityEngine;using USocket.Messages;using XLua;

//支付平台
public enum PayPlatform{    none = 0,      Apple,         //苹果    Wechat,        //微信    AliPay,        //支付宝 
    vvPay_Wechat,  //微贝h5-微信
    vvPay_Alipay,  //微贝h5-支付宝
}

//更新玩家钱币 位的枚举
public enum PlayerUpdateMoney_Enum
{
    PlayerUpdateMoney_Coin = 0,
    PlayerUpdateMoney_DiamondNum,
    PlayerUpdateMoney_Lottery,
    PlayerUpdateMoney_MasterScore,
    PlayerUpdateMoney_UnrecivedRedbag,
    PlayerUpdateMoney_RecivedRedbag,
    PlayerUpdateMoney_Vip,
    PlayerUpdateMoney_CreditScore,

    PlayerUpdateMoney_
};

/// <summary>/// 玩家对象/// </summary>[Hotfix]
public class Player{    //玩家数据集    private PlayerData playerData;        //正在购买的物品ID    private uint iBuyingItemId;    //是否处于购买流程    private bool bBuyingProcess;    //处于请求进入游戏    private bool bRequestingEnterGame;    //处于从游戏返回大厅状态    private bool bInGameBackToHallState;    public Player()    {        iBuyingItemId = 0;        bBuyingProcess = false;        bRequestingEnterGame = false;        bInGameBackToHallState = false;        playerData = new PlayerData();        RegitserMsgHandle();    }    private void RegitserMsgHandle()    {        //玩家购买结果        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYER_BUYRESULT, PlayerBuyResultHandle);        //更新玩家货币数量        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYER_UPDATEMONEY, PlayerUpdateCurrency);        //玩家支付订单信息        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_TRADEAPPPAYINFO, PlayerTradeAppPayInfo);        //更新玩家vip等级        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERVIPLEVELUP, PlayerVipLevelUp);    }    /// <summary>    /// 玩家购买结果消息处理    /// </summary>    /// <param name="msgType"></param>    /// <param name="umsg"></param>    /// <returns></returns>    private bool PlayerBuyResultHandle(uint msgType, UMessage umsg)    {        uint playerid = umsg.ReadUInt();        if (playerid == GetPlayerId())        {            bBuyingProcess = false;            uint itemid = umsg.ReadUInt();            byte buystate = umsg.ReadByte();
          
            ShopItemdata itemdata = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData(itemid);            if (itemdata == null)                return false;
            
            //购买钻石时由支付平台返回值处理关闭等待框
            //if(itemdata.ItemType != Shop.ITEMTYPE.ITEMTYPE_DIAMOND)
            CCustomDialog.CloseCustomWaitUI();
#if UKGAME_SDK
            CCustomDialog.CloseCustomWaitUI();
#endif

#if UNITY_EDITOR || WINDOWS_GUEST
            //编辑器模式下购买钻石没有支付平台返回结果
            if (itemdata.ItemType == Shop.ITEMTYPE.ITEMTYPE_DIAMOND)
                CCustomDialog.CloseCustomWaitUI();
#endif
            if (buystate == 0)            {
                //京东卡和手机充值
                if (itemdata.ItemType == Shop.ITEMTYPE.ITEMTYPE_JINGDONG || 
                    itemdata.ItemType == Shop.ITEMTYPE.ITEMTYPE_MOBILE   || 
                    itemdata.ItemType == Shop.ITEMTYPE.ITEMTYPE_GOODS)
                {
                    string tradeNoId = umsg.ReadString();
                    Bag.GetBagInstance().AddExchangeRecordData(tradeNoId, itemid,0);
                    Bag.GetBagInstance().SetItemInfoPanelActive(false);
                    CCustomDialog.OpenCustomConfirmUIWithFormatParam(1113, itemdata.ItemName);
                }
                else
                {
                    //GameMain.hall_.shop.OpenBuyResultDialogUI(itemdata.ItemIcon, itemdata.ItemType, itemdata.ItemNum + itemdata.PresentItemNum, 1101);
                    //GameMain.hall_.gift.ShowBuyItemIcon((int)itemdata.ItemID);
                }                        }            else            {
                uint tipsID = 0;
                //0表示成功 ，1验证不通过，2购买所需的货币不足，3商品库存不足
                switch (buystate)
                {
                    case 1:
                        tipsID = 1104;
                        break;
                    case 2:
                        {
                            if (itemdata.CurrencyType == Shop.CURRENCYTYPE.CURRENCYTYPE_DIAMOND)
                                tipsID = 1102;
                            else if (itemdata.CurrencyType == Shop.CURRENCYTYPE.CURRENCYTYPE_LOTTERY)
                                tipsID = 1103;
                            else if(itemdata.CurrencyType == Shop.CURRENCYTYPE.CURRENCYTYPE_GOODS)
                                tipsID = 1114;
                        }
                        break;
                    case 3:
                        tipsID = 1105;
                        break;
                }                CCustomDialog.OpenCustomConfirmUI(tipsID);            }        }        return true;    }    //玩家货币数值更新处理    private bool PlayerUpdateCurrency(uint msgType, UMessage umsg)    {        uint nUpdateSign = umsg.ReadUInt();        long coin = 0;        uint diamond = 0;        long lottery = 0;        byte nGameKind = 0;        float masterScore = 0;        if (GameKind.HasFlag((int)PlayerUpdateMoney_Enum.PlayerUpdateMoney_Coin, nUpdateSign))
        {
            coin = umsg.ReadLong();
            GetPlayerData().SetCoin(coin);
        }        if (GameKind.HasFlag((int)PlayerUpdateMoney_Enum.PlayerUpdateMoney_DiamondNum, nUpdateSign))
        {
            diamond = umsg.ReadUInt();
            GetPlayerData().SetDiamond(diamond);
        }

        if (GameKind.HasFlag((int)PlayerUpdateMoney_Enum.PlayerUpdateMoney_Lottery, nUpdateSign))
        {
            lottery = umsg.ReadLong();
            GetPlayerData().SetLottery(lottery);
        }

        if (GameKind.HasFlag((int)PlayerUpdateMoney_Enum.PlayerUpdateMoney_MasterScore, nUpdateSign))
        {
            nGameKind = umsg.ReadByte();
            masterScore = umsg.ReadSingle();
            GameMain.hall_.GetPlayerData().MasterScoreKindArray[nGameKind] = masterScore;
        }

        if (GameKind.HasFlag((int)PlayerUpdateMoney_Enum.PlayerUpdateMoney_UnrecivedRedbag, nUpdateSign))
        {
            GameMain.hall_.GetPlayerData().UnreceivedRedBag = (float)System.Math.Round(umsg.ReadSingle(), 2) ;
        }

        if (GameKind.HasFlag((int)PlayerUpdateMoney_Enum.PlayerUpdateMoney_RecivedRedbag, nUpdateSign))
        {
            GameMain.hall_.GetPlayerData().ReceivedRedBag = umsg.ReadSingle();
        }        if (GameKind.HasFlag((int)PlayerUpdateMoney_Enum.PlayerUpdateMoney_Vip, nUpdateSign))
        {
            GameMain.hall_.GetPlayerData().PlayerLevel = umsg.ReadByte();
        }        if (GameKind.HasFlag((int)PlayerUpdateMoney_Enum.PlayerUpdateMoney_CreditScore, nUpdateSign))
        {
            GameMain.hall_.GetPlayerData().creditScore = umsg.ReadUInt();
        }        Debug.Log("====>>>>>> 更新金币 " + coin + " " + diamond + " " + lottery + " " + masterScore);        //即时更新界面上的货币显示        if (GameMain.hall_.shop != null)        {            GameMain.hall_.shop.RefreshPlayerCurrency();            //200奖券的时候刷新商城实物兑换            if (lottery >= 200)
                GameMain.hall_.shop.RefreshExchangeShopItem();        }        GameMain.hall_.RefreshPlayerCurrency();                   return true;    }    //更新玩家vip等级    private bool PlayerVipLevelUp(uint msgType, UMessage umsg)    {        uint playerid = umsg.ReadUInt();        if (playerid == GetPlayerId())        {            char vipLv = umsg.ReadChar();            long total = umsg.ReadLong();            GetPlayerData().SetVipLv ((byte)vipLv);            GetPlayerData().SetRechargeTotal((uint)total);            //即时更新界面上的vip显示            if (GameMain.hall_.shop != null)            {                GameMain.hall_.shop.RefreshShopPlayerVipText();                      }
            GameMain.hall_.RefreshPlayerVipText();        }        return true;    }    //玩家支付订单信息处理    private bool PlayerTradeAppPayInfo(uint msgType, UMessage umsg)    {        uint playerid = umsg.ReadUInt();        byte payplatform = umsg.ReadByte();        string paytradeInfo = umsg.ReadString();        Debug.Log("玩家："+ playerid + "订单:" + paytradeInfo);

        //订单信息过来后就可以关闭购买等待框了
        BuyEnd();        if (payplatform == (byte)PayPlatform.AliPay)        {#if UNITY_ANDROID            AlipayWeChatPay.RequestAliPay(paytradeInfo,false);#elif UNITY_IOS            WechatPlatfrom_IOS.MayunJieZhang_IOS(paytradeInfo);#endif        }        else if(payplatform == (byte)PayPlatform.Wechat)        {            string noncestr = umsg.ReadString();#if UNITY_ANDROID            if(AlipayWeChatPay.IsWxAppInstalled())            {               AlipayWeChatPay.ReqWxPay(paytradeInfo, noncestr);            }            else                {                //还末安装微信失败处理               // BuyEnd();                CCustomDialog.OpenCustomConfirmUI(1010);            }#elif UNITY_IOS            if (WechatPlatfrom_IOS.WeChat_IsWXAppInstalled())            {               WechatPlatfrom_IOS.WXLgoinJieZhang_IOS(paytradeInfo, noncestr);            }            else            {                //还末安装微信失败处理                //BuyEnd();
                CCustomDialog.OpenCustomConfirmUI(1010);            }#endif        }
        else if (payplatform == (byte)PayPlatform.vvPay_Wechat || payplatform == (byte)PayPlatform.vvPay_Alipay)
        {
            Application.OpenURL(paytradeInfo);
        }

        return true;    }    //获取玩家数据    public PlayerData GetPlayerData()    {        return playerData;    }    //获取玩家ID    public uint GetPlayerId()    {        return playerData.GetPlayerID();    }    //是否只允许玩家apple支付    public  bool IsOnlyApplePay()    {        uint payplatfromflag = playerData.nPayPlatform;        if (payplatfromflag == (0x1 << (int)(PayPlatform.Apple)))            return true;        return false;    }    //支付平台标flag    public  bool IsPayPlatfromHavaFlag(int mask)    {        uint payplatfromflag = playerData.nPayPlatform;        long ret = payplatfromflag & (0x1 << mask);        if (ret > 0 )            return true;        return false;    }    //是否处于购买流程中    public bool IsBuyingProcess()    {        return bBuyingProcess;    }    //是否处于请求进入游戏状态    public bool IsRequestEnterGameState()    {        return bRequestingEnterGame;    }    //更改请求进入游戏状态值    public void ChangeRequestEnterGameState(bool enterstate)    {        bRequestingEnterGame = enterstate;    }    //是否处于从游戏返回大厅中    public bool IsInGameBackToHall()    {        return bInGameBackToHallState;    }    //设置游戏返回大厅的状态    public void SetGameBackToHallState(bool bState)    {        bInGameBackToHallState = bState;    }    //设置正在购买的商品ID    public void SetPlayerBuyingItemId(uint itemid)    {        iBuyingItemId = itemid;    }    //获取正在购买的商品ID    public uint GetPlayerBuyingItemId()    {        return iBuyingItemId;    }    //填充玩家数据    public void InitPlayerData(UMessage msg)    {        if (playerData == null)            return;        float nLeftTime = 0.0f;        playerData.ReadPlayerData(msg, ref nLeftTime);    }    //请求购买物品    public bool RequestBuyItem(uint itemId, PayPlatform tradeplatform = PayPlatform.none)    {               ShopItemdata itemdata = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData(itemId);        if (itemdata == null)            return false;        if(bBuyingProcess)        {            Debug.LogWarning("当前正处于购买过程中。。。。");            return false;        }        iBuyingItemId = itemId;        bBuyingProcess = true;        CCustomDialog.OpenCustomWaitUI(1107);        switch (itemdata.ItemType)        {            case Shop.ITEMTYPE.ITEMTYPE_DIAMOND:                {                    //苹果平台                    if (tradeplatform  == PayPlatform.Apple)                    {#if UNITY_IOS                        ApplePay.Instance.BuyItem(itemId.ToString());#endif                    }                    else if(tradeplatform == PayPlatform.none)                    {                        //SendBuyReq(itemdata.ItemID, PayPlatform.Wechat);                        //发个假的票据                        Player.SendBuyReceiptToServer(tradeplatform, "this is fake receipt!");                    }                    else                    {                       SendBuyReq(itemdata.ItemID, tradeplatform);                    }                }                break;            case Shop.ITEMTYPE.ITEMTYPE_COIN:                {                    //判断钻石是否足够                    if(itemdata.CurrencyType == Shop.CURRENCYTYPE.CURRENCYTYPE_DIAMOND)                    {
                        if (playerData.GetDiamond() < itemdata.ItemPrice)
                        {
                            BuyEnd();
                            CCustomDialog.OpenCustomConfirmUI(1102);

                        }
                        else
                        {
                            SendBuyReq(itemdata.ItemID);
                        }
                    }                    else if(itemdata.CurrencyType == Shop.CURRENCYTYPE.CURRENCYTYPE_LOTTERY)
                    {
                        //判断奖券是否足够
                        if (playerData.GetLottery() < itemdata.ItemPrice)
                        {
                            BuyEnd();
                            CCustomDialog.OpenCustomConfirmUI(1103);
                        }
                        else
                        {
                            SendBuyReq(itemdata.ItemID);
                        }
                    }                                    }
                break;          
            default:
                //发个假的票据
                Player.SendBuyReceiptToServer(tradeplatform, "this is fake receipt!");                break;        }                return true;    }

    //请求购买物品(奖券兑换京东卡，手机充值卡)
    public bool RequestBuyItem(uint itemId, string consigneePhone, string consigneeName = null, string consigneeAddr = null)
    {
        ShopItemdata itemdata = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData(itemId);        if (itemdata == null)            return false;        if (bBuyingProcess)        {            Debug.LogWarning("当前正处于购买过程中。。。。");            return false;        }        iBuyingItemId = itemId;        bBuyingProcess = true;        CCustomDialog.OpenCustomWaitUI(1107);
        switch (itemdata.ItemType)        {
            case Shop.ITEMTYPE.ITEMTYPE_JINGDONG:
            case Shop.ITEMTYPE.ITEMTYPE_MOBILE:
                {
                    //判断奖券是否足够
                    if (playerData.GetLottery() < itemdata.ItemPrice)                    {                        BuyEnd();                        CCustomDialog.OpenCustomConfirmUI(1103);                    }                    else                    {                        SendBuyReq(itemdata.ItemID, consigneePhone,consigneeName,consigneeAddr);                    }
                }
                break;
            case Shop.ITEMTYPE.ITEMTYPE_GOODS:
                SendBuyReq(itemdata.ItemID, consigneePhone, consigneeName, consigneeAddr);
                break;
        }
        return true;
    }    //发送购买请求    private void SendBuyReq(uint itemid, PayPlatform payPlatform = PayPlatform.none)    {        UMessage buyMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYER_REQESTBUY);        buyMsg.Add(GetPlayerId());        buyMsg.Add(itemid);        buyMsg.Add((byte)payPlatform);        NetWorkClient.GetInstance().SendMsg(buyMsg);    }    //发送购买请求(奖券兑换京东卡，手机充值卡)    private void SendBuyReq(uint itemid,string consigneePhone, string consigneeName ,string consigneeAddr)
    {
        UMessage buyMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYER_REQESTBUY);        buyMsg.Add(GetPlayerId());        buyMsg.Add(itemid);        buyMsg.Add((byte)0);        buyMsg.Add(consigneePhone);        if(!string.IsNullOrEmpty(consigneeName))            buyMsg.Add(consigneeName);
        if (!string.IsNullOrEmpty(consigneeAddr))            buyMsg.Add(consigneeAddr);        NetWorkClient.GetInstance().SendMsg(buyMsg);
    }    //购买完成后发送凭据给服务器验证数据    public static bool SendBuyReceiptToServer(PayPlatform payplatform, string receipt)    {        //if (Application.platform == RuntimePlatform.IPhonePlayer)        {            UMessage buyMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYER_BUYVERIFY);            buyMsg.Add(GameMain.hall_.GetPlayerId());            buyMsg.Add(GameMain.hall_.GetPlayer().GetPlayerBuyingItemId());            buyMsg.Add((byte)payplatform);            buyMsg.Add(receipt);            NetWorkClient.GetInstance().SendMsg(buyMsg);        }        return true;    }

    /// <summary>
    /// 分享当前屏幕截图到微信
    /// </summary>
    /// <param name="bTimeline"></param>    public static void ShareImageToWechat(bool bTimeline = true)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;
#if UNITY_ANDROID        if (AlipayWeChatPay.IsWxAppInstalled())
        {
            AlipayWeChatPay.WeChat_ShareImage(bTimeline);
        }
        else
        {
            //还末安装微信失败处理
            CRollTextUI.Instance.AddVerticalRollText(1010);
        }
#endif

#if UNITY_IOS         if (WechatPlatfrom_IOS.WeChat_IsWXAppInstalled())         {            WechatPlatfrom_IOS.WeChat_ShareImage(bTimeline);         }         else         {            //还末安装微信失败处理               
            CRollTextUI.Instance.AddVerticalRollText(1010);         }
#endif
    }


    /// <summary>
    /// 分享当前屏幕截图区域到微信
    /// </summary>
    /// <param name="bTimeline"></param>    public static void ShareImageRectToWechat(Canvas cans,RectTransform rect,bool bTimeline = true)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;

#if UNITY_ANDROID        if (AlipayWeChatPay.IsWxAppInstalled())
        {
            AlipayWeChatPay.WeChat_ShareRectImage(cans,rect,bTimeline);
        }
        else
        {
            //还末安装微信失败处理
            CRollTextUI.Instance.AddVerticalRollText(1010);
        }
#endif

#if UNITY_IOS         if (WechatPlatfrom_IOS.WeChat_IsWXAppInstalled())         {            WechatPlatfrom_IOS.WeChat_ShareRectImage(cans,rect,bTimeline);         }         else         {            //还末安装微信失败处理               
            CRollTextUI.Instance.AddVerticalRollText(1010);         }
#endif
    }

    /// <summary>
    /// 分享文本到微信
    /// </summary>
    /// <param name="bTimeline"></param>    public static void ShareTextToWechat(string shareTxt, bool bTimeline = true)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;

#if UNITY_ANDROID        if (AlipayWeChatPay.IsWxAppInstalled())
        {
            AlipayWeChatPay.WeChat_ShareText(shareTxt, bTimeline);
        }
        else
        {
            //还末安装微信失败处理
            CRollTextUI.Instance.AddVerticalRollText(1010);
        }
#endif

#if UNITY_IOS         if (WechatPlatfrom_IOS.WeChat_IsWXAppInstalled())         {            WechatPlatfrom_IOS.WeChat_ShareText(shareTxt,bTimeline);         }         else         {            //还末安装微信失败处理               
            CRollTextUI.Instance.AddVerticalRollText(1010);         }
#endif
    }

    /// <summary>
    /// 分享URL到微信
    /// </summary>
    /// <param name="bTimeline"></param>    public static void ShareURLToWechat(string urlstr,string descpstr,bool bTimeline = true)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;

#if UNITY_ANDROID        if (AlipayWeChatPay.IsWxAppInstalled())
        {
            AlipayWeChatPay.WeChat_ShareURL(urlstr,descpstr,bTimeline);
        }
        else
        {
            //还末安装微信失败处理
            CRollTextUI.Instance.AddVerticalRollText(1010);
        }
#endif

#if UNITY_IOS         if (WechatPlatfrom_IOS.WeChat_IsWXAppInstalled())         {            WechatPlatfrom_IOS.WeChat_ShareURL(urlstr,descpstr,bTimeline);         }         else         {            //还末安装微信失败处理               
            CRollTextUI.Instance.AddVerticalRollText(1010);         }
#endif
    }

    //处理商店购买失败（非玩家原因）
    public static void BuyEnd()    {        GameMain.hall_.GetPlayer().bBuyingProcess = false;        CCustomDialog.CloseCustomWaitUI();    }    //获取vip下一等级所需充值金额    public static uint GetNextVipLvRechargeValue(byte curVipLv)    {        uint nextviprecharge = 0;        switch(curVipLv)        {            case 0:                nextviprecharge = 9;                break;            case 1:                nextviprecharge = 99;                break;            case 2:                nextviprecharge = 999;                break;            case 3:                nextviprecharge = 9999;                break;            default:                nextviprecharge = 999999999;                break;        }        return nextviprecharge;    } }