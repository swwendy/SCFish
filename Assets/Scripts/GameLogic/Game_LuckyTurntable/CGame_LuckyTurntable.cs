using XLua;
using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
/// <summary>
/// 幸运盘数据
/// </summary>
[LuaCallCSharp]
public class LuckyTurntableData
{
    public int Price = 0;                                           //价格
    public List<LuckyTurntableItemData> luckyItemDataList = null;   //物品数据
}

/// <summary>
/// 幸运盘物品数据
/// </summary>
[LuaCallCSharp]
public class LuckyTurntableItemData
{
    public byte ID = 0;                     //物品ID
    public byte Type = 0;                   //物品类型 0:谢谢参与，1：钻石，2：红包，3：实体奖品
    public string Name = string.Empty;      //物品名称
    public string ItemIcon = string.Empty;  //物品图片
    public Sprite IconSprite = null;        //物品图片对象
    public Image ItemBgImage = null;        //物品背景图片对象
}

/// <summary>
/// 中奖物品数据
/// </summary>
[LuaCallCSharp]
public class LotteryItemData
{
    public int PlayerID = 0;                               //玩家ID
    public string PlayerName = string.Empty;               //玩家名称
    public string ItemName = string.Empty;                 //物品名称
    public System.DateTime CurTime = System.DateTime.Now;  //当前时间

    public LotteryItemData()
    {

    }
    /// <summary>
    /// 中奖物品数据构造函数
    /// </summary>
    /// <param name="playerID">玩家ID</param>
    /// <param name="playerName">玩家名称</param>
    /// <param name="itemName">物品名称</param>
    /// <param name="curTime">当前时间</param>
    public LotteryItemData(int playerID,string playerName,string itemName, System.DateTime curTime)
    {
        InitLotteryItemData(playerID,playerName, itemName, curTime);
    }

    /// <summary>
    /// 初始化中奖信息
    /// </summary>
    /// <param name="playerID">玩家ID</param>
    /// <param name="playerName">玩家名称</param>
    /// <param name="itemName">物品名称</param>
    /// <param name="curTime">当前时间</param>
    public void InitLotteryItemData(int playerID, string playerName, string itemName, System.DateTime curTime)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        ItemName = itemName;
        CurTime = curTime;
    }
}

[Hotfix]
public class CGame_LuckyTurntable : CGameBase
{
    //幸运盘界面对象
    UnityEngine.GameObject LuckyTurntable = null;
    //退出幸运盘界面按钮对象
    Button ExitLuckyButton = null;
    //当前选中的幸运盘ID
    byte CurSelectLuckyTurntableID = 0;
    //当前抽奖以后玩家剩余钻石
    uint PlayerDiamond = 0;
    //最近历史中奖记录数
    int RecentlyLotteryRecord = 1;
    //抽奖按钮对象
    Button LuckyTurntableLottery = null;
    //钻石数量
    Text PlayerDiamondText = null;
    //抽奖动画
    IEnumerator LotteryIEnumerator = null;
    //服务端一直不响应消息
    IEnumerator RequestIEnumerator = null;
    //抽奖控件对象
    GameObject LotteryGameObject = null;
    //自己中奖历史记录界面对象
    GameObject SelfLotteryGameObject = null;
    //钻石商城按钮对象
    Button DiamonButton = null;
    //中奖的物品ID
    List<byte> LotteryItemIDList = null;
    //中奖界面资源
    UnityEngine.Object LuckyTurntableResultObject = null;
    //自己中奖记录信息(只保留当次抽奖的数据)
    List<LotteryItemData> SelfLotteryItemDataList = null;
    //自己中奖最近三十条记录信息
    List<LotteryItemData> SelfLotteryAllItemDataList = null;
    //幸运盘数据
    Dictionary<byte, LuckyTurntableData> LuckyTurntableDictionary = null;

    public CGame_LuckyTurntable() : base(GameKind_Enum.GameKind_LuckyTurntable)
    {
        LotteryItemIDList = new List<byte>();
        SelfLotteryItemDataList = new List<LotteryItemData>();
        SelfLotteryAllItemDataList = new List<LotteryItemData>();
        LuckyTurntableDictionary = new Dictionary<byte, LuckyTurntableData>();
        InitLoadLuckyTurntableMsgHandle();
    }

    /// <summary>
    /// 游戏初始函数
    /// </summary>
    public override void Initialization()
    {
        base.Initialization();
        LoadLuckyTurntableResource();
        RequestLuckyTurntableData();
        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameType, "LuckyAudioCsv");
    }

    /// <summary>
    /// 重置游戏UI
    /// </summary>
    public override void ResetGameUI()
    {
        base.ResetGameUI();
    }

    /// <summary>
    /// 刷新游戏中玩家新获取的金币数量
    /// </summary>
    public override void RefreshGamePlayerCoin()    {        PlayerDiamond = GameMain.hall_.GetPlayerData().GetDiamond();        UpdatePlayerDiamond(PlayerDiamond);
    }

    /// <summary>
    /// 初始化幸运盘消息回调
    /// </summary>
    void InitLoadLuckyTurntableMsgHandle()    {        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_TURNTABLE_SM_BACKLEAVE, HandleExitLuckyTurntable);        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_TURNTABLE_SM_CHOOSElEVEL, HandleLuckyTurntableData);        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_TURNTABLE_LotteryDraw, HandleLuckyTurntableLottery);        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_TURNTABLE_Bonus, HandleLuckyTurntableLotteryBonus);
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_TURNTABLE_SM_ENTERROOM, HandleEnterRoomLuckyTurntableLottery);    }

    /// <summary>
    /// 加载幸运盘资源
    /// </summary>
    void LoadLuckyTurntableResource()
    {
        GameObject gameRoot = GameObject.Find("Canvas/Root");
        if (gameRoot == null)
        {
            DebugLog.LogWarning("幸运盘挂接父节点不存在: Canvas/Root");
            return;
        }

        GameData LuckyTurntabledata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_LuckyTurntable);
        if (LuckyTurntabledata == null)
        {
            DebugLog.LogWarning("幸运盘数据资源不存在:" + GameKind_Enum.GameKind_LuckyTurntable.ToString());
            return;
        }

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(LuckyTurntabledata.ResourceABName);
        if (bundle == null)
        {
            DebugLog.LogWarning("幸运盘ab资源不存在:" + LuckyTurntabledata.ResourceABName);
            return;
        }

        UnityEngine.Object LuckyGameObject = bundle.LoadAsset("Lobby_LuckyTurntable");
        LuckyTurntable = (GameObject)GameMain.instantiate(LuckyGameObject);
        LuckyTurntable.transform.SetParent(gameRoot.transform, false);

        LotteryGameObject = LuckyTurntable.transform.Find("Turntable_bg/Turntable").gameObject;
        ExitLuckyButton = LuckyTurntable.transform.Find("Top/ButtonReturn").GetComponent<Button>();
        ExitLuckyButton.onClick.AddListener(() => { RequestLuckyTurntable(); });

        GameObject DiamonGameObject = LuckyTurntable.transform.Find("Top/Image_DiamondFrame").gameObject;
        DiamonButton = DiamonGameObject.GetComponent<Button>();
        DiamonButton.onClick.AddListener(()=> { OnDiamondShopEvent(); });
        PlayerDiamondText = DiamonGameObject.transform.Find("Text_Diamond").GetComponent<Text>();
        PlayerDiamond = GameMain.hall_.GetPlayerData().GetDiamond();
        UpdatePlayerDiamond(PlayerDiamond);

        LuckyTurntableLottery = LuckyTurntable.transform.Find("Turntable_bg/Button_go").GetComponent<Button>();
        LuckyTurntableLottery.onClick.AddListener(() => { OnLuckyTruntableLottery(); });
        UnityEngine.Transform LuckyRotatorNumTransform =  LuckyTurntable.transform.Find("Toggle_x10");
        LuckyRotatorNumTransform.GetComponent<Toggle>().onValueChanged.AddListener((bool value)=> { CustomAudioDataManager.GetInstance().PlayAudio(1007); });

        LuckyTurntable.transform.Find("Left").gameObject.SetActive(false);
        LuckyTurntable.transform.Find("Turntable_bg").gameObject.SetActive(false);
        LuckyTurntable.transform.Find("Toggle_x10").gameObject.SetActive(false);
        LotteryGameObject.transform.rotation = Quaternion.Euler(0.0f,0,0);

        LuckyTurntableResultObject = bundle.LoadAsset("Lucky_Item");
        GameObject ResultOKGameObject =  LuckyTurntable.transform.Find("Pop-up/result/buttomBG/Button_ok").gameObject;
        XPointEvent.AutoAddListener(ResultOKGameObject,OnResualOKEvent,null);

        SelfLotteryGameObject = LuckyTurntable.transform.Find("Pop-up/record").gameObject;
        UnityEngine.Transform RecordTransform = SelfLotteryGameObject.transform.Find("UiRootBG/Viewport_record/Content_record");
        UnityEngine.Transform RecordChildTransform = null;
        for (int index = 0; index < 30; ++index)
        {
            RecordChildTransform = ((GameObject)GameMain.instantiate(bundle.LoadAsset("Lucky_record"))).transform;
            RecordChildTransform.SetParent(RecordTransform, false);
            RecordChildTransform.gameObject.SetActive(false);
            Color BgImageColor = RecordChildTransform.GetComponent<Image>().color;
            RecordChildTransform.GetComponent<Image>().color = new Color(BgImageColor.r, BgImageColor.g, BgImageColor.b, index % 2 == 0 ? 0.0f : 1.0f);
        }

        Button recordButton = LuckyTurntable.transform.Find("Top/Button_record").GetComponent<Button>();
        recordButton.onClick.AddListener(()=> { OnRecordEvent(true);});

        Button backRecord = SelfLotteryGameObject.transform.Find("UiRootBG/Top/ButtonReturn").GetComponent<Button>();
        backRecord.onClick.AddListener(()=> { OnRecordEvent(false); });
    }

    /// <summary>
    /// 更新中奖界面里面的奖品
    /// </summary>
    IEnumerator UpdateLuckyTurntableResultPanel()
    {
        if (null == LuckyTurntable || LotteryItemIDList.Count == 0)
        {
            yield break;
        }
        if(LuckyTurntableResultObject == null)
        {
            DebugLog.Log("中奖界面资源为空！！！");
            yield break;
        }

        UnityEngine.Transform ParentTransform = LuckyTurntable.transform.Find("Pop-up/result/ImageBG_ItemTips/ItemBG");

        int index = 0;
        bool ActiveState = false;
        LuckyTurntableItemData LuckyItemData = null;
        UnityEngine.Transform ResultChildTransform = null;
        int TotalChildNum = Mathf.Max(ParentTransform.childCount, LotteryItemIDList.Count);
        for (index = 0; index < TotalChildNum; ++index)
        {
            if (index >= ParentTransform.childCount)
            {
                ResultChildTransform = ((GameObject)GameMain.instantiate(LuckyTurntableResultObject)).transform;
                ResultChildTransform.SetParent(ParentTransform, false);
            }else
            {
                ResultChildTransform = ParentTransform.GetChild(index);
            }

            ActiveState = false;
            if (index < LotteryItemIDList.Count)
            {
                LuckyItemData = LuckyTurntableDictionary[CurSelectLuckyTurntableID].luckyItemDataList.Find(itemData => itemData.ID == LotteryItemIDList[index]);
                if(LuckyItemData != null)
                {
                    ResultChildTransform.Find("ItemIcon").GetComponent<Image>().sprite = LuckyItemData.IconSprite;
                    ResultChildTransform.Find("Textnum").GetComponent<Text>().text = LuckyItemData.Name;
                    ActiveState = true;
                }
            }
            ResultChildTransform.gameObject.SetActive(ActiveState);

            SetLuckyTurntableResultPanelActive(true, index == TotalChildNum - 1);
            if (LotteryItemIDList.Count != 1)
            {
                yield return new WaitForSecondsRealtime(1.0f);
            }
        }
        //奖品显示完以后再添加记录
        foreach (LotteryItemData recordItemData in SelfLotteryItemDataList)
        {
            AddSelfLotteryRecord(recordItemData);
            AddRecentlyLotteryRecord(recordItemData);
        }
        UpdatePlayerDiamond(PlayerDiamond);
        SelfLotteryItemDataList.Clear();
        yield break;
    }

    /// <summary>
    /// 设置中奖界面激活状态
    /// </summary>
    /// <param name="active">激活状态</param>
    /// <param name="completeActive">中奖界面确定按钮状态</param>
    void SetLuckyTurntableResultPanelActive(bool active,bool completeActive = false)
    {
        if(LuckyTurntable == null)
        {
            return;
        }

        UnityEngine.Transform ResultPanelTransform = LuckyTurntable.transform.Find("Pop-up/result");

        UnityEngine.Transform ResultPanelCompleteTransform = ResultPanelTransform.Find("buttomBG/Button_ok");
        if(ResultPanelCompleteTransform.gameObject.activeSelf != completeActive)
        {
            ResultPanelCompleteTransform.gameObject.SetActive(completeActive);
        }

        if (ResultPanelTransform.gameObject.activeSelf == active)
        {
            return;
        }

        ResultPanelTransform.gameObject.SetActive(active);

        if (active)
        {
            //先不暂停音效
            //CustomAudioDataManager.GetInstance().StopAudio(false);
            CustomAudioDataManager.GetInstance().PlayAudio(1004);
            DragonBones.UnityArmatureComponent luckyJiesuanComponent = ResultPanelTransform.Find("Animator/anime_luckyJiesuan").GetComponent<DragonBones.UnityArmatureComponent>();
            if(luckyJiesuanComponent)
            {
                luckyJiesuanComponent.animation.Play("chu_xian");
                luckyJiesuanComponent.AddEventListener(DragonBones.EventObject.COMPLETE, (type, eventObject) => { luckyJiesuanComponent.animation.Play("idle"); });
            }
        }
        else
        {
            UnityEngine.Transform LotteryTransform = ResultPanelTransform.Find("ImageBG_ItemTips/ItemBG");
            for (int index = 0; index < LotteryTransform.childCount; ++index)
            {
                LotteryTransform.GetChild(index).gameObject.SetActive(active);
            }
        }
    }

    /// <summary>
    /// 更新幸运盘等级界面
    /// </summary>
    void UpdateLuckyTurntablePanel()
    {
        if(null == LuckyTurntable)
        {
            return;
        }
        int index = 0;
        UnityEngine.Transform ChildItemTransform = null;
        UnityEngine.Transform LuckyTurntableTransform =  LuckyTurntable.transform.Find("Left");
        foreach (var luckyData in LuckyTurntableDictionary)
        {
            ChildItemTransform = LuckyTurntableTransform.GetChild(index);
            Toggle luckyTurntableToggle = ChildItemTransform.GetComponent<Toggle>();
            luckyTurntableToggle.isOn = index == 0 ? true : false;
            luckyTurntableToggle.onValueChanged.AddListener((isOn) => 
            {
                luckyTurntableToggle.interactable = isOn ? false : true; if (isOn) OnSwitchLuckyTruntable(luckyData.Key);
            });
            ChildItemTransform.Find("Label").GetComponent<Text>().text = luckyData.Value.Price + "钻/次";
            ++index;
        }
    }

    /// <summary>
    /// 更新当前幸运盘等级的物品界面
    /// </summary>
    /// <param name="ID">当前等级</param>
    void UpdateLuckyTurntableItemPanle(byte ID)
    {
        if (!LuckyTurntableDictionary.ContainsKey(ID) || null == LuckyTurntable)
        {
            return;
        }
        AssetBundle bagbundle = AssetBundleManager.GetAssetBundle(GameDefine.HallBagIconAssetBundleName);

        Image ItemIcon = null;
        CurSelectLuckyTurntableID = ID;
        UnityEngine.Transform ChildItemTransform = null;
        UnityEngine.Transform LuckyTurntableItemTransform = LuckyTurntable.transform.Find("Turntable_bg/Turntable");
        LuckyTurntableItemData luckyItemData = null;
        LuckyTurntableItemTransform.rotation = Quaternion.Euler(0, 0, 0);
        for (int index = 0; index < LuckyTurntableItemTransform.childCount; ++index)
        {
            if (LuckyTurntableDictionary[ID].luckyItemDataList.Count <= index)
            {
                break;
            }
            luckyItemData = LuckyTurntableDictionary[ID].luckyItemDataList[index];
            ChildItemTransform = LuckyTurntableItemTransform.GetChild(index);
            ChildItemTransform.Find("Text").GetComponent<Text>().text = luckyItemData.Name;
            ItemIcon = ChildItemTransform.Find("Icon").GetComponent<Image>();
            luckyItemData.ItemBgImage = ChildItemTransform.Find("Image").GetComponent<Image>();
            if (luckyItemData.IconSprite == null)
            {
                if (bagbundle)
                {
                    luckyItemData.IconSprite = bagbundle.LoadAsset<Sprite>(luckyItemData.ItemIcon);
                    ItemIcon.sprite = luckyItemData.IconSprite;
                }
            }
            else
            {
                ItemIcon.sprite = luckyItemData.IconSprite;
            }
           // ChildItemTransform.eulerAngles = new Vector3(0, 0,index * 45.0f);
            ChildItemTransform.rotation = Quaternion.Euler(0, 0, index * 45.0f);
        }

        LuckyTurntable.transform.Find("Left").gameObject.SetActive(true);
        LuckyTurntable.transform.Find("Turntable_bg").gameObject.SetActive(true);
        LuckyTurntable.transform.Find("Toggle_x10").gameObject.SetActive(true);
    }

    /// <summary>
    /// 更新玩家钻石
    /// </summary>
    void UpdatePlayerDiamond(uint diamond)
    {
        if(null == PlayerDiamondText)
        {
            return;
        }
        PlayerDiamondText.text = diamond.ToString();
    }

    /// <summary>
    /// 更新自己中奖记录
    /// </summary>
    /// <param name="recordTextInfo">中奖信息</param>
    void UpdateSelfLotteryRecory()
    {
        if (null == SelfLotteryGameObject)
        {
            return;
        }

        UnityEngine.Transform RecordChildTransform = null;
        UnityEngine.Transform RecordViewportTransform = SelfLotteryGameObject.transform.Find("UiRootBG/Viewport_record");
        UnityEngine.Transform RecordTransform = RecordViewportTransform.Find("Content_record");
        for (int index = 0; index < RecordTransform.childCount; ++index)
        {
            if(index >= SelfLotteryAllItemDataList.Count)
            {
                break;
            }

            RecordChildTransform = RecordTransform.GetChild(index);
            RecordChildTransform.Find("Text_time").GetComponent<Text>().text = SelfLotteryAllItemDataList[index].CurTime.ToString("yyyy-MM-dd HH:mm:ss");
            RecordChildTransform.Find("playerinfo/TextID").GetComponent<Text>().text = SelfLotteryAllItemDataList[index].PlayerID.ToString();
            RecordChildTransform.Find("playerinfo/TextName").GetComponent<Text>().text = SelfLotteryAllItemDataList[index].PlayerName;
            RecordChildTransform.Find("Text_Item").GetComponent<Text>().text = SelfLotteryAllItemDataList[index].ItemName;
            if(!RecordChildTransform.gameObject.activeSelf)
            {
                RecordChildTransform.gameObject.SetActive(true);
            }
        }

        RecordViewportTransform.Find("Imagekong").gameObject.SetActive(SelfLotteryAllItemDataList.Count == 0);
    }

    /// <summary>
    /// 禁用/开启幸运盘等级按钮
    /// </summary>
    void SetLuckyTurntablePanelInteractable(bool InteractableState)
    {
        UnityEngine.Transform ChildItemTransform = null;
        UnityEngine.Transform LuckyTurntableTransform = LuckyTurntable.transform.Find("Left");
        for (int index = 0; index < LuckyTurntableTransform.childCount; ++index)
        {
            ChildItemTransform = LuckyTurntableTransform.GetChild(index);
            if (ChildItemTransform)
            {
                ChildItemTransform.GetComponent<Toggle>().interactable = InteractableState;
            }
        }
    }

    /// <summary>
    /// 添加最近的中奖记录
    /// </summary>
    /// <param name="recordTextInfo">中奖信息</param>
    void AddRecentlyLotteryRecord(LotteryItemData recordTextInfo)
    {
        if (null == LuckyTurntable)
        {
            return;
        }

        Text recordText = LuckyTurntable.transform.Find("Right/Content_record/Text").GetComponent<Text>();

        if (RecentlyLotteryRecord >= 50)
        {
            recordText.text = recordText.text.Substring(0, recordText.text.LastIndexOf('\n'));
        }

        recordText.text = recordTextInfo.CurTime.ToString("yyyy-MM-dd HH:mm:ss ") + recordTextInfo.PlayerName + "获得了" + recordTextInfo.ItemName + "\n" + recordText.text;
        ++RecentlyLotteryRecord;
    }

    /// <summary>
    /// 添加自己的中奖记录
    /// </summary>
    /// <param name="recordTextInfo">中奖信息</param>
    void AddSelfLotteryRecord(LotteryItemData recordTextInfo)
    {
        SelfLotteryAllItemDataList.Insert(0, recordTextInfo);
        if (SelfLotteryAllItemDataList.Count > 30)
        {
            SelfLotteryAllItemDataList.RemoveAt(SelfLotteryAllItemDataList.Count - 1);
        }
    }

    /// <summary>
    /// 设置抽奖按钮鼠标事件激活/禁用状态
    /// </summary>
    /// <param name="activeState">true : 激活 false: 禁用</param>
    void SetLotteryButtonInteractable(bool activeState)
    {
        if (null == LuckyTurntableLottery)
        {
            DebugLog.Log("抽奖按钮对象不存在！！！");
            return;
        }
        LuckyTurntableLottery.interactable = activeState;
    }
    
    /// <summary>
    /// 切换不同等级的幸运盘事件
    /// </summary>
    /// <param name="ID">幸运盘ID</param>
    void OnSwitchLuckyTruntable(byte ID)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1006);
        UpdateLuckyTurntableItemPanle(ID);
    }

    /// <summary>
    /// 开始抽奖事件
    /// </summary>
    void OnLuckyTruntableLottery()
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1003);
        bool SelcetState = LuckyTurntable.transform.Find("Toggle_x10").GetComponent<Toggle>().isOn;
        byte LotteryNum = (byte)(SelcetState ? 10 : 1);

        if(!LuckyTurntableDictionary.ContainsKey(CurSelectLuckyTurntableID))
        {
            DebugLog.Log("当前抽奖类型错误: " + CurSelectLuckyTurntableID);
            return;
        }

        if(PlayerDiamond - LotteryNum * LuckyTurntableDictionary[CurSelectLuckyTurntableID].Price < 0)
        {
            CCustomDialog.OpenCustomConfirmUI(1501);
            return;
        }

        SetLotteryButtonInteractable(false);

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_TURNTABLE_LotteryDraw);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(CurSelectLuckyTurntableID);
        msg.Add(LotteryNum);
        HallMain.SendMsgToRoomSer(msg);
    }

    /// <summary>
    /// 中奖确定按钮
    /// </summary>
    void OnResualOKEvent(EventTriggerType eventtype, object parma, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            CustomAudioDataManager.GetInstance().PlayAudio(1005);
            SetLuckyTurntableResultPanelActive(false);        }    }    /// <summary>
    /// 自己中奖记录事件
    /// </summary>
    /// <param name="ActiveState">激活状态</param>    void OnRecordEvent(bool ActiveState)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1005);
        SelfLotteryGameObject.SetActive(ActiveState);
        if(ActiveState)
        {
            UpdateSelfLotteryRecory();
        }
    }    /// <summary>
    /// 钻石商城事件
    /// </summary>    void OnDiamondShopEvent()
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1005);
        GameMain.hall_.Charge(EventTriggerType.PointerClick, Shop.SHOPTYPE.SHOPTYPE_DIAMOND,null);
    }
    /// <summary>
    /// 抽奖动画
    /// </summary>
    /// <returns></returns>
    IEnumerator LuckyTurntableAnimation(float TargetAngle)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1002);
        SetLuckyTurntablePanelInteractable(false);
        if(DiamonButton)
        {
            DiamonButton.interactable = false;
        }
        if (LotteryGameObject)
        {
            LotteryGameObject.transform.DORotate(new Vector3(0, 0, TargetAngle),10, RotateMode.FastBeyond360).SetEase(Ease.OutCirc).OnComplete(()=>
            {
                if (LotteryItemIDList.Count == 1)
                {
                    LuckyTurntableItemData LotteryItemData = LuckyTurntableDictionary[CurSelectLuckyTurntableID].luckyItemDataList.Find(itemData => itemData.ID == LotteryItemIDList[0]);
                    LotteryItemData.ItemBgImage.DOColor(new Color(1,1,1,1), 0.5f).OnComplete(()=> 
                    {
                        LotteryItemData.ItemBgImage.DOColor(new Color(1, 1, 1, 0.4f), 0.15f).SetLoops(6, LoopType.Yoyo).OnComplete(()=> 
                        {
                            LotteryItemData.ItemBgImage.DOColor(new Color(1, 1, 1, 0), 0.5f);
                            SetLotteryButtonInteractable(true);
                        });
                    });
                    if(LotteryItemData != null && LotteryItemData.Type != 0)
                    {
                        GameMain.SC(UpdateLuckyTurntableResultPanel());
                    }
                }else
                {
                    GameMain.SC(UpdateLuckyTurntableResultPanel());
                    SetLotteryButtonInteractable(true);
                }
                if (DiamonButton)
                {
                    DiamonButton.interactable = true;
                }
                SetLuckyTurntablePanelInteractable(true);
                LotteryIEnumerator = null;
            });
        }
        yield break;
    }

    /// <summary>
    /// 服务端不响应客户端请求时处理
    /// </summary>
    /// <param name="time">时间单位秒</param>
    IEnumerator RequestServerResponding(float time)
    {
        yield return new WaitForSecondsRealtime(3);
        CCustomDialog.OpenCustomWaitUI("正在申请返回大厅...");
        yield return new WaitForSecondsRealtime(time - 3);
        CCustomDialog.CloseCustomWaitUI();
        CCustomDialog.OpenCustomConfirmUI(2007,(p)=> { LuckyTurntableToHallSceene(); });
        yield break;
    }

    /// <summary>
    /// 请求退出幸运盘场景
    /// </summary>
    void RequestLuckyTurntable()
    {
        if(ExitLuckyButton)
        {
            ExitLuckyButton.interactable = false;
        }
        CustomAudioDataManager.GetInstance().PlayAudio(1005);

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_TURNTABLE_CM_APPLYLEAVE);
        msg.Add(GameMain.hall_.GetPlayerId());
        HallMain.SendMsgToRoomSer(msg);

        if(RequestIEnumerator != null)
        {
            GameMain.ST(RequestIEnumerator);
        }
        RequestIEnumerator = RequestServerResponding(30);
        GameMain.SC(RequestIEnumerator);
    }

    /// <summary>
    /// 请求幸运盘数据
    /// </summary>
    void RequestLuckyTurntableData()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_TURNTABLE_CM_CHOOSElEVEL);
        msg.Add(GameMain.hall_.GetPlayerId());
        HallMain.SendMsgToRoomSer(msg);
    }

    /// <summary>
    /// 退出幸运盘场景回到大厅
    /// </summary>
    void LuckyTurntableToHallSceene()
    {
        if (LotteryIEnumerator != null)
        {
            GameMain.ST(LotteryIEnumerator);
        }

        GameMain.hall_.SwitchToHallScene();
    }

    /// <summary>
    /// 退出幸运盘场景返回消息
    /// </summary>
    bool HandleExitLuckyTurntable(uint _msgType, UMessage msg)
    {
        if (RequestIEnumerator != null)
        {
            GameMain.ST(RequestIEnumerator);
        }

        byte state = msg.ReadByte();
        if(state != 0)//失败
        {
            if (ExitLuckyButton)
            {
                ExitLuckyButton.interactable = true;
            }
            DebugLog.Log("退出幸运盘场景失败！！！:" + state);
            return false;
        }

        LuckyTurntableToHallSceene();
        return true;
    }

    /// <summary>
    /// 幸运盘数据返回消息
    /// </summary>
    bool HandleLuckyTurntableData(uint _msgType, UMessage msg)
    {
        LuckyTurntableDictionary.Clear();
        byte defaultLuckyID = 0;
        byte luckyID = 0;//幸运牌ID
        byte ItemCoumt = 0;//幸运盘上的物品数量
        byte luckyNum = msg.ReadByte();//幸运盘的个数
        BagItemData bagItemData = null;
        LuckyTurntableData luckyData = null;
        LuckyTurntableItemData luckyItemData = null;
        for (byte luckyIndex = 0; luckyIndex < luckyNum; ++luckyIndex)
        {
            luckyData = new LuckyTurntableData();
            luckyData.luckyItemDataList = new List<LuckyTurntableItemData>();
            luckyID = msg.ReadByte();
            luckyData.Price = msg.ReadInt();//一次价格
            ItemCoumt = msg.ReadByte();
            ushort ItemNum = 0;
            for (byte ItemIndex = 0; ItemIndex < ItemCoumt; ++ItemIndex)
            {
                luckyItemData = new LuckyTurntableItemData();
                luckyItemData.ID = ItemIndex;
                bagItemData = BagDataManager.GetBagDataInstance().GetItemData((uint)msg.ReaduShort());
                luckyItemData.Type = msg.ReadByte();
                ItemNum = msg.ReaduShort();
                if(luckyItemData.Type == 1 || luckyItemData.Type == 2)
                {
                    luckyItemData.Name = ItemNum.ToString();
                }
                if (bagItemData != null)
                {
                    luckyItemData.Name += bagItemData.itemName;
                    luckyItemData.ItemIcon = bagItemData.itemIcon;
                }
                luckyData.luckyItemDataList.Add(luckyItemData);
            }
            LuckyTurntableDictionary.Add(luckyID, luckyData);
            if(luckyIndex == 0)//默认选中第一个等级
            {
                defaultLuckyID = luckyID;
            }
        }

        UpdateLuckyTurntablePanel();
        UpdateLuckyTurntableItemPanle(defaultLuckyID);
        return true;
    }

    /// <summary>
    /// 抽奖返回消息
    /// </summary>
    bool HandleLuckyTurntableLottery(uint _msgType, UMessage msg)
    {
        byte state = msg.ReadByte();// 0 : 成功 1：数据错误 2：钻石不足
        if(state != 0)
        {
            CCustomDialog.OpenCustomConfirmUI((uint)(state == 2 ? 1501 : 2601));
            SetLotteryButtonInteractable(true);
            return false;
        }
        long LotteryDiamond = msg.ReadLong(); // 花费钻石

        UpdatePlayerDiamond((uint)(PlayerDiamond - LotteryDiamond));

        PlayerDiamond = (uint)msg.ReadLong(); // 当前钻石

        LotteryItemIDList.Clear();
        byte lotteryNum = msg.ReadByte();//获奖数
        for(byte index = 0; index < lotteryNum; ++index)
        {
            LotteryItemIDList.Add(msg.ReadByte());
        }

        if (LotteryItemIDList.Count == 0)
        {
            DebugLog.Log("当前抽奖错误,收到服务端数据为空!!!");
            SetLotteryButtonInteractable(true);
            return false;
        }

        if (!LuckyTurntableDictionary.ContainsKey(CurSelectLuckyTurntableID))
        {
            DebugLog.Log("当前抽奖类型错误: " + CurSelectLuckyTurntableID);
            SetLotteryButtonInteractable(true);
            return false;
        }

        int FindItemIndex = LuckyTurntableDictionary[CurSelectLuckyTurntableID].luckyItemDataList.FindIndex(value => value.ID == LotteryItemIDList[0]);
        if(FindItemIndex == -1)
        {
            DebugLog.Log("查找不到获奖物品 ID: " + LotteryItemIDList[0]);
            SetLotteryButtonInteractable(true);
            return false;
        }

        DebugLog.Log("获奖序号 : " + LotteryItemIDList[0]);
        GameMain.ST(LotteryIEnumerator);
        LotteryIEnumerator = LuckyTurntableAnimation(-(FindItemIndex * 45 + Random.Range(-20, 20) + 360*4));
        GameMain.SC(LotteryIEnumerator);
        return true;
    }

    /// <summary>
    /// 玩家中奖更新消息
    /// </summary>
    bool HandleLuckyTurntableLotteryBonus(uint _msgType, UMessage msg)
    {
        int PlayerID = msg.ReadInt();                       //玩家ID
        int CurTime = msg.ReadInt();                        //当前时间
        string PlayerName = msg.ReadString();               //玩家名称
        System.DateTime SystemDataTime = GameCommon.ConvertLongToDateTime(CurTime);

        bool SelfMark = GameMain.hall_.GetPlayerId().CompareTo((uint)PlayerID) == 0;
        LotteryItemData lotteryItemData = new LotteryItemData();
        byte lotteryNum = msg.ReadByte();//获得的奖品数量
        BagItemData bagItemData = null;
        ushort ItemNum = 0;//物品数量
        for (byte index = 0; index < lotteryNum;++index)
        {
            bagItemData = BagDataManager.GetBagDataInstance().GetItemData((uint)msg.ReaduShort());
            ItemNum = msg.ReaduShort();
            string ItemName = ItemNum  > 1 ? ItemNum.ToString(): "";
            if(SelfMark)
            {
                SelfLotteryItemDataList.Add(new LotteryItemData(PlayerID,PlayerName, ItemName + bagItemData.itemName, SystemDataTime));
                continue;
            }
            lotteryItemData.InitLotteryItemData(PlayerID, PlayerName, ItemName + bagItemData.itemName, SystemDataTime);
            AddRecentlyLotteryRecord(lotteryItemData);
        }
        return true;
    }

    /// <summary>
    /// 进入幸运盘房间成功
    /// </summary>
    bool HandleEnterRoomLuckyTurntableLottery(uint _msgType, UMessage msg)
    {
        //背景音乐
        CustomAudioDataManager.GetInstance().PlayAudio(1001,false);
        BagItemData bagItemData = null;
        uint playerID = msg.ReadUInt();
        byte recordNum = msg.ReadByte();
        ushort ItemNum = 0;
        for (int index =0; index < recordNum; ++index)
        {
            LotteryItemData lotteryItemData = new LotteryItemData();
            lotteryItemData.CurTime = GameCommon.ConvertLongToDateTime(msg.ReadUInt());
            bagItemData = BagDataManager.GetBagDataInstance().GetItemData((uint)msg.ReaduShort());
            ItemNum = msg.ReaduShort();
            lotteryItemData.ItemName = ItemNum > 1 ? ItemNum.ToString(): "";
            if(bagItemData != null)
            {
                lotteryItemData.ItemName += bagItemData.itemName;
            }
            lotteryItemData.PlayerID = (int)GameMain.hall_.GetPlayerId();
            lotteryItemData.PlayerName = GameMain.hall_.GetPlayerData().GetPlayerName();
            AddSelfLotteryRecord(lotteryItemData);
        }
        return true;
    }
}
