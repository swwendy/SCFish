﻿
        ITEMTYPE_NEWHANDGIFT    = 5,          //新手礼包
        ITEMTYPE_BENEFITGIFT    = 6,          //特惠礼包
        ITEMTYPE_GOODS          = 7,          //实物
    }
        CURRENCYTYPE_GOODS  = 4,            //兑换实物-物品ID
    }

        //GameObject tgExchangeBtn = ShopMainUI.transform.FindChild("Bottom").FindChild("ToggleGroup").FindChild("Toggle_Exchange").gameObject;
        //XPointEvent.AutoAddListener(tgExchangeBtn, ClickToggleExchangeBtn, null);

        GameObject recordbtn = ShopMainUI.transform.Find("Bottom").Find("ButtonRecord").gameObject;

        //IOS审核版本 或者VIP等级小于1 不显示
        //if(Luancher.IsReviewVersion)
        //   tgExchangeBtn.SetActive(false);
        //else
        //    tgExchangeBtn.SetActive(true);
    }
            ////姓名
            //InputField name = ShopMainUI.transform.FindChild("pop_up/Exchange_Gouwuka/InputName").gameObject.GetComponent<InputField>();
            //if (string.IsNullOrEmpty(name.text))
            //{
            //    CRollTextUI.Instance.AddVerticalRollText(1111);
            //    return;
            //}
            ////手机号码
            //InputField mobilenum = ShopMainUI.transform.FindChild("pop_up/Exchange_Gouwuka/InputPhoneNum").gameObject.GetComponent<InputField>();
            //if (string.IsNullOrEmpty(mobilenum.text) || !GameCommon.CheckPhoneIsAble(mobilenum.text))
            //{
            //    CRollTextUI.Instance.AddVerticalRollText(1112);
            //    return;
            //}

            ////收件地址
            //InputField address = ShopMainUI.transform.FindChild("pop_up/Exchange_Gouwuka/InputAdd").gameObject.GetComponent<InputField>();
            //if (string.IsNullOrEmpty(address.text))
            //{
            //    CRollTextUI.Instance.AddVerticalRollText(1113);
            //    return;
            //}
            //OrderConfirmCacheData.SetOrderCacheData(mobilenum.text, (uint)parma, name.text, address.text);
            ////CloseExchangeUI(EXCHANGEUITYPE.EXCHANGEUITYPE_JINGDONG);
            ////打开确认订单信息界面
            //OpenOrderConfirmUI();
            ////发送兑换消息
            ////GameMain.hall_.GetPlayer().RequestBuyItem((uint)parma, mobilenum.text, name.text,address.text);

        }
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            ////手机号码
            //InputField mobilenum = ShopMainUI.transform.FindChild("pop_up/Exchange_Huafei/InputPhoneNum").gameObject.GetComponent<InputField>();
            //if (string.IsNullOrEmpty(mobilenum.text) || !GameCommon.CheckPhoneIsAble(mobilenum.text))
            //{
            //    CRollTextUI.Instance.AddVerticalRollText(1112);
            //    return;
            //}

            //OrderConfirmCacheData.SetOrderCacheData(mobilenum.text, (uint)parma);

            ////打开确认订单信息界面
            //OpenOrderConfirmUI();
            ////发送兑换消息
            ////GameMain.hall_.GetPlayer().RequestBuyItem((uint)parma, mobilenum.text);

        }
            ////确认
            //if((int)parma == 1)
            //{
            //    if(string.IsNullOrEmpty(OrderConfirmCacheData.RecvName))
            //        CloseExchangeUI(EXCHANGEUITYPE.EXCHANGEUITYPE_MOBILE);
            //    else
            //        CloseExchangeUI(EXCHANGEUITYPE.EXCHANGEUITYPE_JINGDONG);

            //    GameMain.hall_.GetPlayer().RequestBuyItem(OrderConfirmCacheData.ItemId, OrderConfirmCacheData.PhoneNum
            //            ,OrderConfirmCacheData.RecvName,OrderConfirmCacheData.RecvAddress);

            //}
            ////取消
            //else
            //{

            //}
            //ShopMainUI.transform.FindChild("pop_up/Tips_Confirm").gameObject.SetActive(false);
        }
            //ShopItemdata itemdata = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData((uint)parma);
            //if (itemdata == null)
            //    return;
            GameMain.hall_.GetPlayer().RequestBuyItem((uint)parma);
            //GameMain.hall_.GetPlayer().RequestBuyItem((uint)parma);
            uint itemid = (uint)parma;