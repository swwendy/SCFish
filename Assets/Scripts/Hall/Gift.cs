using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DragonBones;
using XLua;

[Hotfix]
public class Gift
{
    GameObject discountobj;
    GameObject newcomerobj;
    GameObject mainui_;
    GameObject CanvasObj;

    uint currentDiscountItem;
    uint currentNewComerItem;

    Sprite buyIcon;
    GameObject discountAnimate;

    bool isShowDiscounts;
    bool isShowNewcomer;
    bool isStartShow;
    float leftseconds;

    public Gift( GameObject mainui )
    {
//         mainui_ = mainui;
//         CanvasObj = GameObject.Find("Canvas/Root");
//         GameObject discountsBtn = mainui_.transform.FindChild("Panelbottom").FindChild("Left").FindChild("Button_discounts").gameObject;
//         XPointEvent.AutoAddListener(discountsBtn, OnClickDiscount, null);
//         GameObject newcomerBtn = mainui_.transform.FindChild("Panelbottom").FindChild("Left").FindChild("Button_newcomer").gameObject;
//         XPointEvent.AutoAddListener(newcomerBtn, OnClickNewComer, null);
//         currentDiscountItem = 50000;
//         currentNewComerItem = 40000;
//         isStartShow = true;
//         isShowGiftButtons = !(GameMain.hall_.GetPlayerData().discountsGift == -1 || GameMain.hall_.GetPlayerData().newcomerGift == -1);
//         ShowGiftButtons(isShowGiftButtons);
//         leftseconds = GameMain.hall_.GetPlayerData().newcomertime;
//         LoadBuyIcon();
//         LoadDiscountSelectAnimation();
//         StartShowNewComerPanel();
    }


    public void ShowGiftUI(GameObject mainui)
    {
        mainui_ = mainui;
        CanvasObj = GameObject.Find("Canvas/Root");
        GameObject discountsBtn = mainui_.transform.Find("Panelbottom").Find("Left").Find("Button_discounts").gameObject;
        XPointEvent.AutoAddListener(discountsBtn, OnClickDiscount, null);
        GameObject newcomerBtn = mainui_.transform.Find("Panelbottom").Find("Left").Find("Button_newcomer").gameObject;
        XPointEvent.AutoAddListener(newcomerBtn, OnClickNewComer, null);
        currentDiscountItem = 50000;
        currentNewComerItem = 40000;
        isStartShow = true;
        isShowDiscounts = !(GameMain.hall_.GetPlayerData().discountsGift == -1);
        isShowNewcomer = !(GameMain.hall_.GetPlayerData().newcomerGift == -1);
        ShowGiftButtons(isShowDiscounts);
        leftseconds = GameMain.hall_.GetPlayerData().newcomertime;
        LoadBuyIcon();
        LoadDiscountSelectAnimation();
        StartShowNewComerPanel();
    }

    public void Update()
    {
        leftseconds -= Time.deltaTime;
        if (leftseconds < 0)
            leftseconds = 432000.0f;
        if (newcomerobj == null)
            return;

        Text timeTx = newcomerobj.transform.Find("ImageBG").Find("discounts_4").Find("ImageBG").
            Find("ImageText").Find("Text_Time").gameObject.GetComponent<Text>();
        int day = (int)leftseconds / 86400;
        int lefthour = (int)leftseconds % 86400;
        int hour = lefthour / 3600;
        int leftminute = lefthour % 3600;
        int minute = leftminute / 60;
        int seconds = leftminute % 60;

        string daystr = "";
        if(day > 0)
            daystr = day.ToString() + "天";
        string hourstr = "";
        if (day > 0 || hour > 0)
            hourstr = hour.ToString() + "小时";
        string minutestr = "";
        if (day > 0 || hour > 0 || minute > 0)
            minutestr = minute.ToString() + "分";
        string secondstr = seconds + "秒";
        timeTx.text = daystr + hourstr + minutestr + secondstr;
    }

    void InitDiscountCloseButton()
    {
        GameObject discountCloseBtn = discountobj.transform.Find("ImageBG").Find("Button_Close").gameObject;
        XPointEvent.AutoAddListener(discountCloseBtn, OnCloseDiscount, null);
    }

    void InitNewcomerCloseButton()
    {
        GameObject newcomerCloseBtn = newcomerobj.transform.Find("ImageBG").Find("Button_Close").gameObject;
        XPointEvent.AutoAddListener(newcomerCloseBtn, OnCloseNewComer, null);
    }

    void OnCloseDiscount(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            discountobj.SetActive(false);
        }
    }

    void OnCloseNewComer(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            newcomerobj.SetActive(false);

            //if(isStartShow)
            //{
            //    StartShowDiscountPanel();
            //}
        }
    }

    void ShowGiftButtons(bool isShowDiscounts)
    {
        bool isshow = IsShowNewComer();

        mainui_.transform.Find("Panelbottom").Find("Left").Find("Button_newcomer").gameObject.SetActive(isshow);

        if (!isshow)
            mainui_.transform.Find("Panelbottom").Find("Left").Find("Button_discounts").gameObject.SetActive(isShowDiscounts);
        else
            mainui_.transform.Find("Panelbottom").Find("Left").Find("Button_discounts").gameObject.SetActive(false);
    }

    bool IsShowNewComer()
    {
        bool isShow = false;
        bool isShowThree = false;
        for (GameCity.NewHand_Gift index = GameCity.NewHand_Gift.NewHand_6; index < GameCity.NewHand_Gift.NewHand_Max; index++)
        {
            bool isbuythree = GameKind.HasFlag((int)index, GameMain.hall_.GetPlayerData().newcomerGift);
            isShowThree = !isbuythree;
            if (isShowThree)
                break;
        }

        bool isShowOne = false;
        bool isbuyone = GameKind.HasFlag((int)GameCity.NewHand_Gift.NewHand_ThreeOne, GameMain.hall_.GetPlayerData().newcomerGift);
        isShowOne = !isbuyone;

        isShow = isShowThree && isShowOne;

        return isShow;
    }

    void StartShowNewComerPanel()
    {
        if (!isShowNewcomer)
            return;

        if (IsShowNewComer())
            ShowNewComer();
        else
            StartShowDiscountPanel();
    }

    void StartShowDiscountPanel()
    {
        if (!isShowDiscounts)
            return;

        bool isShow = false;
        for (GameCity.Benefit_Gift index = GameCity.Benefit_Gift.Benefit_68; index < GameCity.Benefit_Gift.Benefit_Max; index++)
        {
            bool isbuy = GameKind.HasFlag((int)index, GameMain.hall_.GetPlayerData().discountsGift);
            isShow = !isbuy;
        }

        if (isShow)
            ShowDiscount();
    }

    void LoadBuyIcon()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle)
            buyIcon = bundle.LoadAsset<Sprite>("lb_btn_lb1");
    }

    void LoadDiscountSelectAnimation()
    {
        if(discountAnimate == null)
        {
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bundle)
            {
                UnityEngine.Object obj0 = bundle.LoadAsset("Anime_Actchoose");
                discountAnimate = (GameObject)GameMain.instantiate(obj0);
            }
        }
    }

    void PlayAnimatorByIndex(int index)
    {
        GameObject animateObj = discountobj.transform.Find("ImageBG").
            Find("LeftButton").Find("ImageViewport").Find("Content").
            Find("Anime_" + index.ToString()).gameObject;

        UnityArmatureComponent animate = ((GameObject)discountAnimate).GetComponent<UnityArmatureComponent>();
        discountAnimate.transform.SetParent(animateObj.transform, false);
        animate.animation.Play("newAnimation");
    }

    void OnClickDiscount(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            ShowDiscount();
        }
    }

    void OnClickNewComer(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            ShowNewComer();
        }
    }

    void ShowDiscount()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (discountobj == null)
        {
            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Activity_discounts");
            discountobj = (GameObject)GameMain.instantiate(obj0);
            discountobj.transform.SetParent(CanvasObj.transform, false);

            InitDiscount();
            InitDiscountCloseButton();
            PlayAnimatorByIndex(1);
        }

        discountobj.SetActive(true);

        GameObject buttonBG = discountobj.transform.Find("ImageBG").Find("LeftButton").
            Find("ImageViewport").Find("Content").gameObject;
        for (GameCity.Benefit_Gift index = GameCity.Benefit_Gift.Benefit_68; index < GameCity.Benefit_Gift.Benefit_Max; index++)
        {
            bool isbuy = GameKind.HasFlag((int)index, GameMain.hall_.GetPlayerData().discountsGift);
            if(isbuy)
            {
                string name = "Button_" + ((int)index + 1).ToString();
                Image button = buttonBG.transform.Find(name).gameObject.GetComponent<Image>();
                button.sprite = buyIcon;
            }
        }

        isStartShow = false;
    }

    void InitDiscount()
    {
        GameObject buttonBG = discountobj.transform.Find("ImageBG").Find("LeftButton").
            Find("ImageViewport").Find("Content").gameObject;
        for(int index = 1; index <= 6; index++)
        {
            string name = "Button_" + index.ToString();

            GameObject button = buttonBG.transform.Find(name).gameObject;
            uint temp = (uint)index;
            XPointEvent.AutoAddListener(button, OnClickDiscountButton, temp - 1);
        }

        GameObject buttonBuy = discountobj.transform.Find("ImageBG").Find("goodsinfo").Find("Button_buy").gameObject;
        XPointEvent.AutoAddListener(buttonBuy, OnBuyDiscounts, null);

        ChangeGoodsInfo(50000);
    }

    void OnBuyDiscounts(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            GameObject buttonBuy = discountobj.transform.Find("ImageBG").Find("goodsinfo").Find("Button_buy").gameObject;
            Text buyTX = buttonBuy.transform.Find("Text").gameObject.GetComponent<Text>();
            buyTX.text = "购买";

            int discountInfo = GameMain.hall_.GetPlayerData().discountsGift;
            bool isbuy = GameKind.HasFlag((int)currentDiscountItem - 50000, discountInfo);
            if (isbuy)
            {
                CCustomDialog.OpenCustomConfirmUI(1110);
                return;
            }
            else
            {
                DoBuyByItemID(currentDiscountItem);
            }
        }
    }

    void OnClickDiscountButton(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            uint index = (uint)button + 50000;
            currentDiscountItem = index;
            ChangeGoodsInfo(index);

            PlayAnimatorByIndex((int)index + 1 - 50000);
        }
    }

    void ChangeGoodsInfo(uint index)
    {
        ShopItemdata sid = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData(index);
        if (sid == null)
            return;
        string goods = sid.ItemNum.ToString();
        string externgoods = sid.PresentItemNum.ToString();

        Text goodsTx = discountobj.transform.Find("ImageBG").Find("goodsinfo").
            Find("ImageBG").Find("coin_buy").Find("Text_Coin").gameObject.GetComponent<Text>();
        goodsTx.text = goods;

        Text externgoodsTx = discountobj.transform.Find("ImageBG").Find("goodsinfo").
            Find("ImageBG").Find("coin_give").Find("Text_Coin").gameObject.GetComponent<Text>();
        externgoodsTx.text = externgoods;
    }

    void ShowNewComer()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (newcomerobj == null)
        {
            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Activity_newcomer");
            newcomerobj = (GameObject)GameMain.instantiate(obj0);
            newcomerobj.transform.SetParent(CanvasObj.transform, false);

            InitNewComer();
            InitNewcomerCloseButton();

            GameObject newBG = newcomerobj.transform.Find("ImageBG").gameObject;
            for (GameCity.NewHand_Gift index = GameCity.NewHand_Gift.NewHand_ThreeOne; index < GameCity.NewHand_Gift.NewHand_Max; index++)
            {
                bool isbuy = GameKind.HasFlag((int)index, GameMain.hall_.GetPlayerData().newcomerGift);
                if (isbuy)
                {
                    int nameindex = (int)index + 1;
                    if (nameindex == 1)
                        nameindex = 4;
                    else
                        nameindex -= 1;

                    string name = "discounts_" + nameindex.ToString();
                    GameObject buybutton = newBG.transform.Find(name).Find("ImageBG").Find("Image_bought").gameObject;
                    buybutton.SetActive(true);
                }
            }
        }

        newcomerobj.SetActive(true);
    }

    void InitNewComer()
    {
        GameObject newBG = newcomerobj.transform.Find("ImageBG").gameObject;
        for(int index = 0; index < 4; index++)
        {
            uint nameindex = (uint)index + 1;
            if (nameindex == 1)
                nameindex = 4;
            else
                nameindex -= 1;

            string name = "discounts_" + nameindex.ToString();

            GameObject buybutton = newBG.transform.Find(name).Find("ButtonBuy").gameObject;
            XPointEvent.AutoAddListener(buybutton, OnBuyNewComer, nameindex);
        }
    }

    void OnBuyNewComer(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerUp)
        {
            uint ui2csvindex = (uint)button;
            if (ui2csvindex == 4)
                ui2csvindex = 0;
            currentNewComerItem = ui2csvindex + 40000;
            int newcomerInfo = GameMain.hall_.GetPlayerData().newcomerGift;
            bool isbuy = GameKind.HasFlag((int)currentNewComerItem - 40000, newcomerInfo);
            if(isbuy)
            {
                CCustomDialog.OpenCustomConfirmUI(1109);
                return;
            }
            else
            {
                if ((int)currentNewComerItem - 40000 == 0)
                {
                    bool buyother1 = GameKind.HasFlag(1, newcomerInfo);
                    bool buyother2 = GameKind.HasFlag(2, newcomerInfo);
                    bool buyother3 = GameKind.HasFlag(3, newcomerInfo);
                    if (buyother1 || buyother2 || buyother3)
                    {
                        CCustomDialog.OpenCustomConfirmUI(1109);
                        return;
                    }
                        
                }
                else
                {
                    bool buyother4 = GameKind.HasFlag(0, newcomerInfo);
                    if (buyother4)
                    {
                        CCustomDialog.OpenCustomConfirmUI(1109);
                        return;
                    }
                }

                DoBuyByItemID(currentNewComerItem);
            }
        }
    }

    public void ShowBuyItemIcon(int itemindex)
    {
        if(itemindex >= 40000 && itemindex < 50000)
        {
            GameKind.AddFlag((int)itemindex - 40000, ref GameMain.hall_.GetPlayerData().newcomerGift);

            int uiindex = itemindex - 39999;
            if (uiindex == 1)
                uiindex = 4;
            else
                uiindex -= 1;

            if (newcomerobj == null)
                return;

            GameObject newBG = newcomerobj.transform.Find("ImageBG").gameObject;
            string name = "discounts_" + uiindex.ToString();
            GameObject buybutton = newBG.transform.Find(name).Find("ImageBG").Find("Image_bought").gameObject;
            buybutton.SetActive(true);

            ShowGiftButtons(true);
        }
        if(itemindex >= 50000 && itemindex < 60000)
        {
            GameKind.AddFlag((int)itemindex - 50000, ref GameMain.hall_.GetPlayerData().discountsGift);

            if (discountobj == null)
                return;

            GameObject buttonBG = discountobj.transform.Find("ImageBG").Find("LeftButton").
                    Find("ImageViewport").Find("Content").gameObject;
            string name = "Button_" + ((int)currentDiscountItem - 49999).ToString();
            Image discountButton = buttonBG.transform.Find(name).gameObject.GetComponent<Image>();
            discountButton.sprite = buyIcon;

            ShowGiftButtons(false);
        }
    }

    void DoBuyByItemID(uint currentItem)
    {
#if UNITY_EDITOR || WINDOWS_GUEST
        GameMain.hall_.GetPlayer().RequestBuyItem((uint)currentItem);
#else
#if VVPAY_H5
             OpenPayPlatfromSelectUI((uint)parma);
#else
#if UKGAME_SDK
            ShopItemdata itemdata = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData((uint)currentItem);
            if (itemdata == null)
                return;
            string playeritemId = string.Format("{0}|{1}", GameMain.hall_.GetPlayerData().GetPlayerID(), itemdata.ItemID);
            CUKGameSDK.UKPlatform_Pay(itemdata.ItemPrice.ToString(), itemdata.ItemName, playeritemId, itemdata.ItemID.ToString());           
#else
            //如果只允许行apple支付
            if(GameMain.hall_.GetPlayer().IsOnlyApplePay())
                 GameMain.hall_.GetPlayer().RequestBuyItem((uint)currentItem,PayPlatform.Apple);
            else
            {
                GameMain.hall_.shop.OpenPayPlatfromSelectUI((uint)currentItem);
            }
#endif
#endif
#endif

    }
}
