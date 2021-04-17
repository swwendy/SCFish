using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USocket.Messages;

/// <summary>
/// 兑换物品界面
/// </summary>
public class ExchangeItemPanel
{

    //兑换记录数据
    private class ExchangeRecordData    {
        public string orderId;     //兑换订单ID
        public uint itemId;        //兑换物品ID
        public byte orderState;    //订单处理状态(0未处理，1已处理)

        public ExchangeRecordData(string _orderId, uint _itemId, byte _orderState)        {            orderId = _orderId;            itemId = _itemId;            orderState = _orderState;        }    }

    //兑换物品界面对象
    GameObject ExchangeItemPanelGameObject = null;
    //当前兑换物品的名称
    string CurRecvName = string.Empty;
    //当前兑换人的手机号码
    string CurPhoneNum = string.Empty;
    //当前兑换人的地址
    string CurRecvAddress = string.Empty;
    //当前兑换物品ID
    BagItemData CurItemData = null;
    //兑换记录数据初始化标记
    private bool ExChangeRecordDataFlag = false;
    //兑换记录数据
    List<ExchangeRecordData> ExchangeRecordDataList = new List<ExchangeRecordData>();

    /// <summary>
    /// 加载兑换物品界面资源
    /// </summary>
    public void LoadExchangeItemPanelResource()
    {
        if (ExchangeItemPanelGameObject != null)
        {
            return;
        }
        AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (assetBundle == null)
        {
            return;
        }
        UnityEngine.Object obj0 = assetBundle.LoadAsset("Lobby_exchange");
        ExchangeItemPanelGameObject = (GameObject)GameMain.instantiate(obj0);
        ExchangeItemPanelGameObject.transform.SetParent(GameObject.Find("Canvas/Root").transform, false);

        //确定物品兑换按钮
        GameObject okbtn = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Gouwuka/ButtonOK").gameObject;
        XPointEvent.AutoAddListener(okbtn, OnClickOkExchangeEvent, null);

        //兑换物品界面关闭按钮
        GameObject closeExchangebtn = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Gouwuka/ButtonClose").gameObject;
        XPointEvent.AutoAddListener(closeExchangebtn, OnClickCloseExchangeEvent, null);

        //兑换记录界面关闭按钮
        GameObject closeRecordbtn = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Record/ButtonClose").gameObject;
        XPointEvent.AutoAddListener(closeRecordbtn, OnClickCloseExchangeEvent, null);

        GameObject confirmOkbtn = ExchangeItemPanelGameObject.transform.Find("pop_up/Tips_Confirm/ImageBG/ButtonOk").gameObject;
        GameObject confirmCancelbtn = ExchangeItemPanelGameObject.transform.Find("pop_up/Tips_Confirm/ImageBG/ButtonCancel").gameObject;
        XPointEvent.AutoAddListener(confirmOkbtn, OnClickConfirmEvent, true);
        XPointEvent.AutoAddListener(confirmCancelbtn, OnClickConfirmEvent, false);

        ExChangeRecordDataFlag = false;
        InitExchangeMsg();
    }

    /// <summary>
    /// 初始化兑换消息
    /// </summary>
    void InitExchangeMsg()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKGETTRADEINFO, HandleExchangeRecordDataMsg);

    }

    /// <summary>
    /// 显示兑换物品界面
    /// </summary>
    /// <param name="itemId">兑换物品Id</param>
    public void ShowExchangeItemPanel(ushort itemId)
    {
        CurItemData = BagDataManager.GetBagDataInstance().GetItemData(itemId);
        if(CurItemData == null)
        {
            CRollTextUI.Instance.AddVerticalRollText("兑换物品不存在！");
            return;
        }

        if (ExchangeItemPanelGameObject == null)
        {
            LoadExchangeItemPanelResource();
        }

        UnityEngine.Transform ItemPopTransform = ExchangeItemPanelGameObject.transform.Find("pop_up");
        UnityEngine.Transform ItemTransform = ItemPopTransform.Find("Exchange_Gouwuka");

        AssetBundle assetBundle = AssetBundleManager.GetAssetBundle(GameDefine.HallBagIconAssetBundleName);
        if (assetBundle)
        {
            Sprite goodsicon = assetBundle.LoadAsset<Sprite>(CurItemData.itemIcon);
            Image goods = ItemTransform.Find("Image").gameObject.GetComponent<Image>();
            goods.sprite = goodsicon;
        }

        ItemPopTransform.gameObject.SetActive(true);
        ItemTransform.gameObject.SetActive(true);
        ExchangeItemPanelGameObject.SetActive(true);
    }

    /// <summary>
    /// 显示兑换物品历史记录界面
    /// </summary>
    public void ShowExchangeItemRecordPanel()
    {
        if (ExchangeItemPanelGameObject == null)
        {
            LoadExchangeItemPanelResource();
        }
        CurItemData = null;
        UnityEngine.Transform ItemPopTransform = ExchangeItemPanelGameObject.transform.Find("pop_up");
        UnityEngine.Transform ItemRecordTransform = ItemPopTransform.Find("Exchange_Record");

        ItemRecordTransform.gameObject.SetActive(true);
        ItemPopTransform.gameObject.SetActive(true);
        ExchangeItemPanelGameObject.SetActive(true);

        if(!ExChangeRecordDataFlag)
        {
            ExChangeRecordDataFlag = true;
            //请求兑换记录数据
            UMessage recordMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_PLAYERGETTRADEINFO);
            recordMsg.Add(GameMain.hall_.GetPlayerId());
            NetWorkClient.GetInstance().SendMsg(recordMsg);

            CCustomDialog.OpenCustomWaitUI("正在进行兑换记录查询...");
        }
    }

    //显示兑换确认单
    void ShowExchangeItemOrderConfirmPanel()
    {
        if(CurItemData == null)
        {
            return;
        }

        if (ExchangeItemPanelGameObject == null)
        {
            LoadExchangeItemPanelResource();
        }
        UnityEngine.Transform ItemPopTransform = ExchangeItemPanelGameObject.transform.Find("pop_up");
        UnityEngine.Transform ItemConfirmTransform = ItemPopTransform.Find("Tips_Confirm");


        ShopItemdata itemdata = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData(CurItemData.itemScriptid);
        if (itemdata == null)
        {
            CRollTextUI.Instance.AddVerticalRollText("购买物品已经下架！");
            return;
        }

        ItemConfirmTransform.Find("ImageBG/Imageline/Image_goods/Textgoods").gameObject.GetComponent<Text>().text = itemdata.ItemName;

        string orderstring = "";
        if (string.IsNullOrEmpty(CurRecvName))
        {
            orderstring = "充值手机号码:<color=#FF8C00>" + CurPhoneNum + "</color>";
        }
        else
        {
            orderstring = "收件人:<color=#FF8C00>" + CurRecvName + "</color>\r\n手机号码:<color=#FF8C00>"
                + CurPhoneNum + "</color>\r\n收件地址:<color=#FF8C00>" + CurRecvAddress + "</color>";
        }
        ItemConfirmTransform.Find("ImageBG/Textaddress").gameObject.GetComponent<Text>().text = orderstring;

        ItemConfirmTransform.gameObject.SetActive(true);
        ItemPopTransform.gameObject.SetActive(true);
        ExchangeItemPanelGameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭兑换界面
    /// </summary>
    void CloseExchangeEvent()
    {
        if ( null == ExchangeItemPanelGameObject)
        {
            return;
        }
        CurItemData = null;
        UnityEngine.Transform ItemPopTransform = ExchangeItemPanelGameObject.transform.Find("pop_up");
        UnityEngine.Transform ItemRecordTransform = ItemPopTransform.Find("Exchange_Record");
        UnityEngine.Transform ItemTransform = ItemPopTransform.Find("Exchange_Gouwuka");
        UnityEngine.Transform ItemConfirmTransform = ItemPopTransform.Find("Tips_Confirm");
        ItemConfirmTransform.gameObject.SetActive(false);
        ItemRecordTransform.gameObject.SetActive(false);
        ItemTransform.gameObject.SetActive(false);
        ExchangeItemPanelGameObject.SetActive(false);
    }

    /// <summary>
    ///  更新兑换记录
    /// </summary>
    public void UpdateExchangeRecordData()    {
        if(ExchangeItemPanelGameObject == null)
        {
            return;
        }
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)
        {
            return;
        }
        ShopItemdata itemData = null;
        Transform recordTransform = null;
        Transform prtf = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Record/Viewport_Exchange_Record/Content_Exchange_Record");
        for(int index = 0; index < ExchangeRecordDataList.Count; ++index)
        {
            if(index >= prtf.childCount)
            {
                recordTransform = (GameMain.instantiate(bundle.LoadAsset<Object>("Shop_pop_up_Record")) as GameObject).transform;
                recordTransform.SetParent(prtf, false);
            }else
            {
                recordTransform = prtf.GetChild(index);
            }

            recordTransform.Find("TextRiqi").gameObject.GetComponent<Text>().text = ExchangeRecordDataList[index].orderId;
            itemData = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData(ExchangeRecordDataList[index].itemId);
            recordTransform.Find("TextJiangpin").gameObject.GetComponent<Text>().text = itemData != null ? itemData.ItemName : "未知物品";
            string statetxt = "<color=#FF8C00>等待处理</color>";
            if (ExchangeRecordDataList[index].orderState == 1)
                statetxt = "<color=#FF8C00>已处理</color>";
            recordTransform.Find("TextZhuangtai").gameObject.GetComponent<Text>().text = statetxt;
        }
    }

    /// <summary>
    /// 确认兑换按钮事件
    /// </summary>
    void OnClickOkExchangeEvent(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype != EventTriggerType.PointerClick  || ExchangeItemPanelGameObject == null)
        {
            return;
        }
        CustomAudio.GetInstance().PlayCustomAudio(1002);
        //姓名
        InputField name = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Gouwuka/InputName").gameObject.GetComponent<InputField>();
        if (string.IsNullOrEmpty(name.text))
        {
            CRollTextUI.Instance.AddVerticalRollText(1111);
            return;
        }
        //手机号码
        InputField mobilenum = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Gouwuka/InputPhoneNum").gameObject.GetComponent<InputField>();
        if (string.IsNullOrEmpty(mobilenum.text) || !GameCommon.CheckPhoneIsAble(mobilenum.text))
        {
            CRollTextUI.Instance.AddVerticalRollText(1112);
            return;
        }

        //收件地址
        InputField address = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Gouwuka/InputAdd").gameObject.GetComponent<InputField>();
        if (string.IsNullOrEmpty(address.text))
        {
            CRollTextUI.Instance.AddVerticalRollText(1111);
            return;
        }

        CurRecvName = name.text;
        CurPhoneNum = mobilenum.text;
        CurRecvAddress = address.text;
        //打开确认订单信息界面
        ShowExchangeItemOrderConfirmPanel();
    }

    /// <summary>
    /// 关闭兑换界面按钮事件
    /// </summary>
    void OnClickCloseExchangeEvent(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype != EventTriggerType.PointerClick)
        {
            return;
        }
        CustomAudio.GetInstance().PlayCustomAudio(1002);
        CloseExchangeEvent();
    }

    /// <summary>
    /// 订单确认界面按钮事件
    /// </summary>
    void OnClickConfirmEvent(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype != EventTriggerType.PointerClick)
        {
            return;
        }
        CustomAudio.GetInstance().PlayCustomAudio(1002);
        
        if((bool)button && CurItemData != null)
        {
            ShopItemdata itemdata = CCsvDataManager.Instance.ShopItemDataMgr.GetShopItemData(CurItemData.itemScriptid);
            if (itemdata == null)
            {
                CRollTextUI.Instance.AddVerticalRollText("购买物品已经下架!");
                return;
            }

            bool bCheckState = true;
            if(BagDataManager.GetBagDataInstance().currentItems_.ContainsKey((ushort)CurItemData.itemid))
            {
                if(BagDataManager.GetBagDataInstance().currentItems_[(ushort)CurItemData.itemid].itemNumber < itemdata.ItemPrice)
                {
                    bCheckState = false;
                }
            }

            if (itemdata.SpecilSign != CurItemData.itemid || !bCheckState)
            {
                CRollTextUI.Instance.AddVerticalRollText("兑换商品校验出错!");
                return;
            }

            //姓名
            InputField name = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Gouwuka/InputName").GetComponent<InputField>();
            //手机号码
            InputField mobilenum = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Gouwuka/InputPhoneNum").GetComponent<InputField>();
            //收件地址
            InputField address = ExchangeItemPanelGameObject.transform.Find("pop_up/Exchange_Gouwuka/InputAdd").GetComponent<InputField>();
            name.text = string.Empty;
            address.text = string.Empty;
            mobilenum.text = string.Empty;

            GameMain.hall_.GetPlayer().RequestBuyItem(CurItemData.itemScriptid, CurPhoneNum, CurRecvName, CurRecvAddress);
        }
        CloseExchangeEvent();
    }

    /// <summary>
    /// 处理兑换记录数据消息
    /// </summary>
    private bool HandleExchangeRecordDataMsg(uint msgType, UMessage umsg)    {        CCustomDialog.CloseCustomWaitUI();        uint playerid = umsg.ReadUInt();        int recordcount = umsg.ReadInt();        for (int i = 0; i < recordcount; i++)        {            string orderid = umsg.ReadString();            byte orderstate = umsg.ReadByte();            uint itemid = umsg.ReadUInt();

            AddExchangeRecordData(orderid, itemid, orderstate);
        }
        UpdateExchangeRecordData();
        return true;    }

    /// <summary>
    /// 添加兑换历史记录
    /// </summary>
    /// <param name="orderid">兑换订单ID</param>
    /// <param name="itemId">兑换物品ID</param>
    /// <param name="orderstate">订单处理状态(0未处理，1已处理)</param>
    public void AddExchangeRecordData(string orderid, uint itemId, byte orderstate)
    {
        ExchangeRecordData recordItemData = ExchangeRecordDataList.Find(recordData => recordData.orderId == orderid);
        if (recordItemData != null)
        {
            recordItemData.orderState = orderstate;
            recordItemData.itemId = itemId;
        }
        else
        {
            ExchangeRecordData redata = new ExchangeRecordData(orderid, itemId, orderstate);
            ExchangeRecordDataList.Insert(0,redata);
        }
        UpdateExchangeRecordData();
    }
}
