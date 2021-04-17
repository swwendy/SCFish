using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using USocket.Messages;

public class Bag
{
    static Bag instance_;

    GameObject root_;
    GameObject CanvasObj;
    Dictionary<uint, GameObject> itemobjects_;
    GameObject switchgoodspanel_;

    //兑换物品界面
    ExchangeItemPanel exchangeItemPanel = null;
    public static Bag GetBagInstance()
    {
        if (instance_ == null)
        {
            instance_ = new Bag();
        }

        return instance_;
    }

	Bag()
    {
        itemobjects_ = new Dictionary<uint, GameObject>();
        //LoadBagResource();
    }

    /// <summary>
    ///初始化兑换界面数据
    /// </summary>
    void InitExchangeItemPanel()
    {
        if(exchangeItemPanel == null)
        {
            exchangeItemPanel = new ExchangeItemPanel();
            exchangeItemPanel.LoadExchangeItemPanelResource();
        }
    }

    void LoadBagResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if (root_ == null)
        {
            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Lobby_bag");
            root_ = (GameObject)GameMain.instantiate(obj0);

            CanvasObj = GameObject.Find("Canvas/Root");
            root_.transform.SetParent(CanvasObj.transform, false);
            root_.SetActive(false);

            InitBagEvents();
        }

        LoadBagItemResource();
    }

    void InitBagEvents()
    {
        GameObject btnClose = root_.transform.Find("ImageBG").Find("ButtonClose").gameObject;
        XPointEvent.AutoAddListener(btnClose, OnCloseBag, null);

        GameObject itemInfoClose = root_.transform.Find("Pop-up").Find("iteminfo").
            Find("ImageBG").Find("ButtonClose").gameObject;
        XPointEvent.AutoAddListener(itemInfoClose, OnCloseItemInfo, null);

        GameObject RecordGameObject =  root_.transform.Find("Button_exchangeRecord").gameObject;
        XPointEvent.AutoAddListener(RecordGameObject, OnExchangeRecordClickEvent, null);
    }

    private void OnCloseBag(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            CloseBag();
        }
    }

    private void OnCloseItemInfo(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            SetItemInfoPanelActive(false);
        }
    }

    private void OnExchangeRecordClickEvent(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            InitExchangeItemPanel();
            exchangeItemPanel.ShowExchangeItemRecordPanel();
        }
    }

    public void LoadBagItemResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        AssetBundle bagbundle = AssetBundleManager.GetAssetBundle(GameDefine.HallBagIconAssetBundleName);
        if (bagbundle == null)
            return;

        itemobjects_.Clear();

        if (root_ == null)
            return;

        GameObject bagbg = root_.transform.Find("ImageBG").Find("Viewport").Find("Content").gameObject;
        GameMain.hall_.ClearChilds(bagbg);

        int length = BagDataManager.GetBagDataInstance().currentItems_.Count;
        GameObject noitem = root_.transform.Find("ImageBG").Find("Viewport").Find("Imagekong").gameObject;
        noitem.SetActive(length == 0);

        foreach(ushort key in BagDataManager.GetBagDataInstance().currentItems_.Keys)
        {
            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("bag_icon");
            GameObject itemobj = (GameObject)GameMain.instantiate(obj0);

            itemobj.transform.SetParent(bagbg.transform, false);

            XPointEvent.AutoAddListener(itemobj, OnUseItem, BagDataManager.GetBagDataInstance().currentItems_[key].itemid);

            Image itemicon = itemobj.transform.Find("Imageicon").gameObject.GetComponent<Image>();
            itemicon.sprite = bagbundle.LoadAsset<Sprite>(BagDataManager.GetBagDataInstance().GetItemData(BagDataManager.GetBagDataInstance().currentItems_[key].itemid).itemIcon);
            Text itemNumber = itemobj.transform.Find("Textnum").gameObject.GetComponent<Text>();
            itemNumber.text = "x" + BagDataManager.GetBagDataInstance().currentItems_[key].itemNumber.ToString();
            itemobjects_.Add(BagDataManager.GetBagDataInstance().currentItems_[key].itemid, itemobj);
        }
    }

    private void OnUseItem(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            ushort itemid = (ushort)button;
            SetItemInfo(itemid);

            SetItemInfoPanelActive(true);
        }
    }

    void SetItemInfo( ushort itemid )
    {
        AssetBundle bagbundle = AssetBundleManager.GetAssetBundle(GameDefine.HallBagIconAssetBundleName);
        if (bagbundle == null)
        {
            return;
        }

        BagItemData bagItemData =  BagDataManager.GetBagDataInstance().GetItemData(itemid);
        if(bagItemData == null)
        {
            return;
        }

        GameObject iteminfo = root_.transform.Find("Pop-up").Find("iteminfo").Find("ImageBG").gameObject;

        Image infoicon = iteminfo.transform.Find("bag_icon").Find("Imageicon").gameObject.GetComponent<Image>();
        infoicon.sprite = bagbundle.LoadAsset<Sprite>(bagItemData.itemIcon);
        Text itemName = iteminfo.transform.Find("Textname").gameObject.GetComponent<Text>();
        itemName.text = bagItemData.itemName;
        Text itemNumber = iteminfo.transform.Find("Textnum").gameObject.GetComponent<Text>();
        itemNumber.text = BagDataManager.GetBagDataInstance().currentItems_[itemid].itemNumber.ToString();
        Text itemWorkTime = iteminfo.transform.Find("Texttime").gameObject.GetComponent<Text>();

        if(bagItemData.itemWorkTime > 0)
        {
            uint minutes = bagItemData.itemWorkTime / 60;
            uint hours = minutes / 60;
            minutes = minutes % 60;
            uint days = hours / 24;
            hours = hours % 24;
            string timeTx = "";
            if (days > 0)
                timeTx += days.ToString() + "天";
            if (hours > 0)
                timeTx += hours.ToString() + "小时";
            if (minutes > 0)
                timeTx += minutes.ToString() + "分钟";
            itemWorkTime.text = timeTx;
        }
        else
        {
            itemWorkTime.gameObject.SetActive(false);
        }

        Text itemcontent = iteminfo.transform.Find("Textinfo").gameObject.GetComponent<Text>();
        itemcontent.text = bagItemData.itemInfo;

        GameObject givetype = iteminfo.transform.Find("ButtonBG").Find("Button_Give").gameObject;
        givetype.SetActive(false);
        GameObject usetype = iteminfo.transform.Find("ButtonBG").Find("Button_shiyong").gameObject;
        usetype.SetActive(false);
        GameObject switchtype = iteminfo.transform.Find("ButtonBG").Find("Button_duihuan").gameObject;
        switchtype.SetActive(false);

        switch (bagItemData.usetype)
        {
            case ItemUseType.Type1:
                givetype.SetActive(true);
                XPointEvent.AutoAddListener(givetype, OnGiveItem, itemid);
                break;
            case ItemUseType.Type2:
                usetype.SetActive(true);
                XPointEvent.AutoAddListener(usetype, OnUseTicket, itemid);
                break;
            case ItemUseType.Type3:
                switchtype.SetActive(true);
                XPointEvent.AutoAddListener(switchtype, OnSwitchGoods, itemid);
                break;
            case ItemUseType.Type4:
                break;
            case ItemUseType.Type5:
                break;
        }
    }

    private void OnGiveItem(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {

        }
    }

    private void OnUseTicket(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            GameMain.hall_.Go2ContestHall();
            ushort itemid = (ushort)button;
            BagDataManager.GetBagDataInstance().isUseItem = true;
            BagDataManager.GetBagDataInstance().scriptid = BagDataManager.GetBagDataInstance().bagitemsdata_[itemid].itemScriptid;

            root_.SetActive(false);
            SetItemInfoPanelActive(false);
        }
    }

    private void OnSendSwitchGoodsMsg(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            uint goodsid = (uint)button;

            Text mobilenum = switchgoodspanel_.transform.Find("Exchange_Gouwuka")
                .Find("InputPhoneNum").gameObject.GetComponent<Text>();

            if (mobilenum.text.Length == 0)
                return;

            GameMain.hall_.GetPlayer().RequestBuyItem(goodsid, mobilenum.text);
        }
    }

    private void OnSwitchGoods(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            InitExchangeItemPanel();
            exchangeItemPanel.ShowExchangeItemPanel((ushort)button);
        }
    }

    public void ShowBag()
    {
        if (root_ == null)
            LoadBagResource();

        LoadBagItemResource();

        root_.SetActive(true);
    }

    public void CloseBag()
    {
        if (root_ == null)
            LoadBagResource();

        root_.SetActive(false);
    }

    /// <summary>
    /// 关闭物品信息界面
    /// </summary>
    /// <param name="activeState">激活状态 ture: 激活 false :失败</param>
    public void SetItemInfoPanelActive(bool activeState)
    {
        if (root_ == null)
        {
            return;
        }
        GameObject iteminfo = root_.transform.Find("Pop-up/iteminfo").gameObject;
        iteminfo.SetActive(activeState);
    }

    /// <summary>
    /// 添加兑换历史记录
    /// </summary>
    /// <param name="orderid">兑换订单ID</param>
    /// <param name="itemId">兑换物品ID</param>
    /// <param name="orderstate">订单处理状态(0未处理，1已处理)</param>
    public void AddExchangeRecordData(string orderid, uint itemId, byte orderstate)
    {
        if(exchangeItemPanel == null)
        {
            exchangeItemPanel = new ExchangeItemPanel();
        }
        exchangeItemPanel.AddExchangeRecordData(orderid, itemId, orderstate);
    }
}
