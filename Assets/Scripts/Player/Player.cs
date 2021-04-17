﻿

//支付平台
public enum PayPlatform
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

/// <summary>
public class Player
          
            ShopItemdata itemdata = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData(itemid);
            
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
            if (buystate == 0)
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
                }            
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
                }
        {
            coin = umsg.ReadLong();
            GetPlayerData().SetCoin(coin);
        }
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
        }
        {
            GameMain.hall_.GetPlayerData().PlayerLevel = umsg.ReadByte();
        }
        {
            GameMain.hall_.GetPlayerData().creditScore = umsg.ReadUInt();
        }
                GameMain.hall_.shop.RefreshExchangeShopItem();
            GameMain.hall_.RefreshPlayerVipText();

        //订单信息过来后就可以关闭购买等待框了
        BuyEnd();
                CCustomDialog.OpenCustomConfirmUI(1010);
        else if (payplatform == (byte)PayPlatform.vvPay_Wechat || payplatform == (byte)PayPlatform.vvPay_Alipay)
        {
            Application.OpenURL(paytradeInfo);
        }

        return true;
                        if (playerData.GetDiamond() < itemdata.ItemPrice)
                        {
                            BuyEnd();
                            CCustomDialog.OpenCustomConfirmUI(1102);

                        }
                        else
                        {
                            SendBuyReq(itemdata.ItemID);
                        }
                    }
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
                    }
                break;          
            default:
                //发个假的票据
                Player.SendBuyReceiptToServer(tradeplatform, "this is fake receipt!");

    //请求购买物品(奖券兑换京东卡，手机充值卡)
    public bool RequestBuyItem(uint itemId, string consigneePhone, string consigneeName = null, string consigneeAddr = null)
    {
        ShopItemdata itemdata = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData(itemId);
        switch (itemdata.ItemType)
            case Shop.ITEMTYPE.ITEMTYPE_JINGDONG:
            case Shop.ITEMTYPE.ITEMTYPE_MOBILE:
                {
                    //判断奖券是否足够
                    if (playerData.GetLottery() < itemdata.ItemPrice)
                }
                break;
            case Shop.ITEMTYPE.ITEMTYPE_GOODS:
                SendBuyReq(itemdata.ItemID, consigneePhone, consigneeName, consigneeAddr);
                break;
        }
        return true;
    }
    {
        UMessage buyMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYER_REQESTBUY);
        if (!string.IsNullOrEmpty(consigneeAddr))
    }

    /// <summary>
    /// 分享当前屏幕截图到微信
    /// </summary>
    /// <param name="bTimeline"></param>
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;
#if UNITY_ANDROID
        {
            AlipayWeChatPay.WeChat_ShareImage(bTimeline);
        }
        else
        {
            //还末安装微信失败处理
            CRollTextUI.Instance.AddVerticalRollText(1010);
        }
#endif

#if UNITY_IOS
            CRollTextUI.Instance.AddVerticalRollText(1010);
#endif
    }


    /// <summary>
    /// 分享当前屏幕截图区域到微信
    /// </summary>
    /// <param name="bTimeline"></param>
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;

#if UNITY_ANDROID
        {
            AlipayWeChatPay.WeChat_ShareRectImage(cans,rect,bTimeline);
        }
        else
        {
            //还末安装微信失败处理
            CRollTextUI.Instance.AddVerticalRollText(1010);
        }
#endif

#if UNITY_IOS
            CRollTextUI.Instance.AddVerticalRollText(1010);
#endif
    }

    /// <summary>
    /// 分享文本到微信
    /// </summary>
    /// <param name="bTimeline"></param>
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;

#if UNITY_ANDROID
        {
            AlipayWeChatPay.WeChat_ShareText(shareTxt, bTimeline);
        }
        else
        {
            //还末安装微信失败处理
            CRollTextUI.Instance.AddVerticalRollText(1010);
        }
#endif

#if UNITY_IOS
            CRollTextUI.Instance.AddVerticalRollText(1010);
#endif
    }

    /// <summary>
    /// 分享URL到微信
    /// </summary>
    /// <param name="bTimeline"></param>
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            return;

#if UNITY_ANDROID
        {
            AlipayWeChatPay.WeChat_ShareURL(urlstr,descpstr,bTimeline);
        }
        else
        {
            //还末安装微信失败处理
            CRollTextUI.Instance.AddVerticalRollText(1010);
        }
#endif

#if UNITY_IOS
            CRollTextUI.Instance.AddVerticalRollText(1010);
#endif
    }

    //处理商店购买失败（非玩家原因）
    public static void BuyEnd()