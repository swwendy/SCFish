using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USocket.Messages;
using XLua;

/// <summary>
/// 主控够级玩家
/// </summary>
[Hotfix]
public class GouJi_RoleLocal : GouJi_Role
{
    /// <summary>
    /// 选着出牌状态
    /// </summary>
    bool m_bMouseSelectCardState;

    /// <summary>
    /// 选中牌列表
    /// </summary>
    public HashSet<CardData> m_SelectPokerList = new HashSet<CardData>();

    /// <summary>
    /// 出牌-过按钮对象
    /// </summary>
    Transform m_GouTransform;

    /// <summary>
    /// 出牌-提示按钮对象
    /// </summary>
    Transform m_TiShiTransform;

    /// <summary>
    /// 出牌-出牌按钮对象
    /// </summary>
    Transform m_ChuPaiTransform;

    /// <summary>
    /// 出牌-让牌按钮对象
    /// </summary>
    Transform m_RangPaiTransform;

    /// <summary>
    /// 出牌-面板对象
    /// </summary>
    Transform m_ChuPaiPanelTransform;

    /// <summary>
    /// 提问面板对象
    /// </summary>
    Transform m_AskdelTransform;

    /// <summary>
    /// 出牌提示索引
    /// </summary>
    private int m_nPromptPokerDataIndex;

    /// <summary>
    /// 拆牌提示索引
    /// </summary>
    private int m_nDiscardPromptPokerDataIndex;

    /// <summary>
    /// 服务器提问标识
    /// </summary>
    uint m_nAskSign;

    /// <summary>
    /// 没有大于上家的牌提示
    /// </summary>
    bool m_bGuoPokerState;

    /// <summary>
    /// 烧牌第一次出牌标记
    /// </summary>
    bool m_bShaoPaiFirstPokerState = false;

    /// <summary>
    /// 倒计时提示音效时间
    /// </summary>
    float m_fSpeciaAlertTime;

    /// <summary>
    /// 出牌提示数据
    /// </summary>
    public Dictionary<int, List<CardData>> m_PromptPokerDataDictionary = new Dictionary<int, List<CardData>>();

    public GouJi_RoleLocal(CGame_GouJi gameBase, byte cSit) : base(gameBase, cSit)
    {
        m_nAskSign = 0;
        m_bGuoPokerState = false;
        m_nPromptPokerDataIndex = 0;
        m_bMouseSelectCardState = false;
        m_SelectPokerList.Clear();
        m_PromptPokerDataDictionary.Clear();
        m_nDiscardPromptPokerDataIndex = -1;

        m_AskdelTransform = null;
        m_ChuPaiPanelTransform = m_RoleMainUITranform.Find("ButtonBG/ButtonBG_xingpai");
        m_GouTransform = m_ChuPaiPanelTransform.Find("Button_buchu");        //过
        m_TiShiTransform = m_ChuPaiPanelTransform.Find("Button_tishi");      //提示
        m_ChuPaiTransform = m_ChuPaiPanelTransform.Find("Button_chupai");    //出牌
        m_RangPaiTransform = m_ChuPaiPanelTransform.Find("Button_rangpai");
        m_fSpeciaAlertTime = -1;
        m_CardTransformOne = m_RoleMainUITranform.Find("Poker_Shoupai_heng/Point_shoupai_1");  //第一层
        m_CardTransformTwo = m_RoleMainUITranform.Find("Poker_Shoupai_heng/Point_shoupai_2");  //第二层

        InitUIEvent();
    }

    /// <summary>
    /// 初始化UI界面事件
    /// </summary>
    void InitUIEvent()
    {
        m_GouTransform.GetComponent<Button>().onClick.AddListener(() =>
        {
            ResetOutPanelInteractable(false);
            ResetOutPokerPanel();
            SendRolePoker();
            if(m_GameBase != null)
            {
                m_GameBase.PlayerGameAudio(0, 6);
            }
        });

        m_ChuPaiTransform.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (m_OutPokerList.Count == 0)
            {
                CRollTextUI.Instance.AddVerticalRollText(2306);
                return;
            }
            ResetOutPanelInteractable(false);
            SendRolePoker();
            if (m_GameBase != null)
            {
                m_GameBase.PlayerGameAudio(0, 6);
            }
        });

        m_TiShiTransform.GetComponent<Button>().onClick.AddListener(() =>
        {
            ResetOutPokerPanel();

            foreach (var pokerList in m_PromptPokerDataDictionary[m_nPromptPokerDataIndex])
            {
                SetSelectPokerUIAnimation(pokerList);
            }

            UpdateOutPokerButtonInteractable();

            if (m_nDiscardPromptPokerDataIndex >= 0)
            {
                RefreshDiscardPokerPromptUI(m_nPromptPokerDataIndex >= m_nDiscardPromptPokerDataIndex);
            }

            m_nPromptPokerDataIndex++;
            if (m_nPromptPokerDataIndex >= m_PromptPokerDataDictionary.Count)
            {
                m_nPromptPokerDataIndex = 0;
            }
            if (m_GameBase != null)
            {
                m_GameBase.PlayerGameAudio(0, 6);
            }
        });

        if(m_GameBase != null)
        {
            Transform shangGongTransform = m_RoleMainUITranform.Find("ButtonBG/Button_shanggong");
            if (shangGongTransform)
            {
                XPointEvent.AutoAddListener(shangGongTransform.gameObject, m_GameBase.OnClickTribute, null);
            }

            Transform chatTransform = m_RoleMainUITranform.Find("ButtonBG/Button_chat");
            if (chatTransform)
            {
                XPointEvent.AutoAddListener(chatTransform.gameObject, m_GameBase.OnClickEmotion, null);
            }
        }
    }

    /// <summary>
    /// 刷新手牌没有大于上家的提示
    /// </summary>
    /// <param name="activeState">true:显示 false:不显示</param>
    public void RefreshGuoPokerPromptUI(bool activeState)
    {
        if (m_bGuoPokerState == activeState ||
           m_RoleMainUITranform == null)
        {
            return;
        }
        m_bGuoPokerState = activeState;
        m_RoleMainUITranform.Find("Tip_BG/Tip_yaobuqi").gameObject.SetActive(activeState);
        GameMain.WaitForCall(2,()=> { RefreshGuoPokerPromptUI(false); });
    }

    /// <summary>
    /// 刷新拆牌提示
    /// </summary>
    /// <param name="activeState">true:显示 false:不显示</param>
    public override void RefreshDiscardPokerPromptUI(bool activeState)
    {
        if (m_RoleMainUITranform == null)
        {
            return;
        }
        m_RoleMainUITranform.Find("Tip_BG/Tip_chaipai").gameObject.SetActive(activeState);
    }

    /// <summary>
    /// 刷新围观界面
    /// </summary>
    /// <param name="activeState">true:显示 false:不显示</param>
    /// <param name="upFriend">true:上联 false:下联</param>
    public void RefreshWatchingFriendUI(bool activeState,bool upFriend = false)
    {
        if (m_RoleMainUITranform == null)
        {
            return;
        }
        Transform wathchingTransform = m_RoleMainUITranform.Find("ButtonBG/ButtonBG_weiguan");
        wathchingTransform.gameObject.SetActive(activeState);

        if(!activeState)
        {
            return;
        }

        Transform upTransform = wathchingTransform.Find("Button_0");
        Button upButton = upTransform.GetComponent<Button>();
        upTransform.gameObject.SetActive(upFriend);
        XPointEvent.AutoAddListener(upTransform.gameObject, OnClickWathching, upButton);

        Transform downTransform = wathchingTransform.Find("Button_1");
        Button downButton = downTransform.GetComponent<Button>();
        downTransform.gameObject.SetActive(!upFriend);
        XPointEvent.AutoAddListener(downTransform.gameObject,OnClickWathching, downButton);

        Button selectButton = upFriend ? upButton : downButton;
        selectButton.interactable = false;
        GameMain.WaitForCall(15,()=> { selectButton.interactable = true; });
    }

    /// <summary>
    /// 刷新手牌对象
    /// </summary>
    /// <param name="CardTransform">手牌对象父节点</param>
    /// <param name="pokerIndex">手牌索引</param>
    /// <param name="isMask">手牌遮罩标识</param>
    void RefreshHavePokerObject(Transform CardTransform,ref int pokerIndex,bool isMask)
    {
        if(CardTransform == null)
        {
            return;
        }
        Transform haveTransform = null;
        for (int childIndex = 0; childIndex < CardTransform.childCount; ++childIndex)
        {
            haveTransform = CardTransform.GetChild(childIndex);
            if(!haveTransform.gameObject.activeSelf)
            {
                continue;
            }
            m_HavePokerList[pokerIndex].m_CartTransform = haveTransform;
            m_HavePokerList[pokerIndex].m_CartTransform.Find("Text_num").GetComponent<Text>().text = string.Empty;
            m_HavePokerList[pokerIndex].m_CartTransform.Find("image_Mask").gameObject.SetActive(isMask);
            m_HavePokerList[pokerIndex].m_CartTransform.Find("Text_gong").gameObject.SetActive(m_HavePokerList[pokerIndex].m_bGongMask);
            UpdatePoker(m_HavePokerList[pokerIndex], m_GameBase.Bystander ? "_onlooker" : "_Big");
            byte cardValue = m_HavePokerList[pokerIndex].m_nCardValue;
            if (!isMask)
            {
                XPointEvent.AutoAddListener(m_HavePokerList[pokerIndex].m_CartTransform.gameObject, OnClickCards, cardValue);
            }
            ++pokerIndex;
        }
    }

    /// <summary>
    /// 重置手牌对象结构
    /// </summary>
    /// <param name="bRarentTransform">手牌对象父节点</param>
    /// <param name="maxCount">单层手牌对象最大数量</param>
    /// <param name="havePokerIndex">现有手牌索引</param>
    void ResetHavePokerObject(Transform parentTransform, int maxCount,ref int havePokerIndex)
    {
        if(parentTransform == null)
        {
            return;
        }

        int totalCount = Mathf.Max(parentTransform.childCount, maxCount);
        Transform haveTransform = null;
        int transformChildCount = parentTransform.childCount;
        Object pokerObject = m_GameBase.m_GouJiAssetBundle.LoadAsset("Poker_GJ_shoupai_heng");
        for (int transformIndex = 0; transformIndex < totalCount; ++transformIndex)
        {
            if(havePokerIndex < m_HavePokerList.Count)
            {
                haveTransform = m_HavePokerList[havePokerIndex].m_CartTransform;
            }

            if (transformIndex < transformChildCount)
            {
                haveTransform = parentTransform.GetChild(transformIndex);
                if(transformIndex >= maxCount)
                {
                    haveTransform.gameObject.SetActive(false);
                    GameObject.Destroy(haveTransform.gameObject);
                    haveTransform = null;
                    continue;
                }
                if(!haveTransform.gameObject.activeSelf)
                {
                    haveTransform = null;
                }
            }

            if (haveTransform == null && transformIndex < maxCount)
            {
                haveTransform = ((GameObject)GameMain.instantiate(pokerObject)).transform;
            }

            if (haveTransform == null)
            {
                continue;
            }

            haveTransform.Find("image_Mask").gameObject.SetActive(false);

            haveTransform.SetParent(parentTransform, false);
            if (!haveTransform.gameObject.activeSelf)
            {
                haveTransform.gameObject.SetActive(true);
            }
            ++havePokerIndex;
        }
    }

    /// <summary>
    /// 重置角色数据
    /// </summary>
    /// <param name="bDestroryState">true:全部都删除false:删除一部分</param>
    public override void ResetRoleData(bool bDestroryState = true)
    {
        m_SelectPokerList.Clear();
        m_PromptPokerDataDictionary.Clear();
        m_bMouseSelectCardState = false;
        m_nPromptPokerDataIndex = 0;
        m_bGuoPokerState = false;
        m_nDiscardPromptPokerDataIndex = -1;
        RefreshWatchingFriendUI(false);
        m_fSpeciaAlertTime = -1;
        m_bShaoPaiFirstPokerState = false;
        m_AskdelTransform = null;
        base.ResetRoleData(bDestroryState);
    }

    /// <summary>
    /// 添加手牌对象
    /// </summary>
    /// <param name="pokerDataList">牌值</param>
    /// <param name="isMask">是否打开遮罩 true（打开）false(关闭)</param>
    /// <param name="isGongMask">是否是贡牌 true（是）false(不是)</param>
    public override void AddHavePokerObject(List<byte> pokerDataList, bool isMask = false,bool isGongMask = false)
    {
        if (m_RoleMainUITranform == null || m_GameBase.m_GouJiAssetBundle == null ||
            m_CardTransformOne == null || m_CardTransformTwo == null || pokerDataList.Count == 0)
        {
            return;
        }

        CardData cardData = null;
        for (int index = 0; index < pokerDataList.Count; ++index)
        {
            cardData = new CardData();
            cardData.m_bGongMask = isGongMask;
            cardData.m_nCardValue = m_GameBase.Bystander ? RoomInfo.NoSit : pokerDataList[index];
            m_HavePokerList.Add(cardData);
        }

        int totalPokerCount = m_HavePokerList.Count;
        int maxCount = totalPokerCount > 18 ? (int)System.Math.Floor(totalPokerCount / 2.0f) : 0;

        m_CardTransformOne.GetComponent<HorizontalLayoutGroup>().enabled = true;
        m_CardTransformTwo.GetComponent<HorizontalLayoutGroup>().enabled = true;

        int havePokerIndex = 0;
        ResetHavePokerObject(m_CardTransformTwo, maxCount, ref havePokerIndex);
        totalPokerCount -= maxCount;
        ResetHavePokerObject(m_CardTransformOne, totalPokerCount, ref havePokerIndex);

        m_HavePokerList.Sort(SortByCardValue);
        m_HavePokerList.Reverse();

        havePokerIndex = 0;
        RefreshHavePokerObject(m_CardTransformTwo,ref havePokerIndex, isMask);
        RefreshHavePokerObject(m_CardTransformOne, ref havePokerIndex, isMask);

        UpdateCardNum();
    }

    /// <summary>
    /// 删除手牌对象
    /// </summary>
    /// <param name="pokerDataList">牌值</param>
    public override void DestoryHavePokerOjbect(List<byte> pokerDataList)
    {
        if(m_GameBase == null)
        {
            return;
        }
        if(m_GameBase.Bystander)
        {
            int startIndex =0;
            int endIndex = startIndex + pokerDataList.Count;
            DebugLog.Log("删除:" + "startIndex " + startIndex + " endIndex " + endIndex +" _ " + pokerDataList.Count+" : " + m_HavePokerList.Count);
            while (startIndex < endIndex)
            {
                GameObject.Destroy(m_HavePokerList[startIndex].m_CartTransform.gameObject);
                ++startIndex;
            }
            startIndex = 0;
            m_HavePokerList.RemoveRange(startIndex, pokerDataList.Count);
        }
        else
        {
            base.DestoryHavePokerOjbect(pokerDataList);
        }
        RefreshHavePokerPanel();
    }

    /// <summary>
    /// 删除手牌对象
    /// </summary>
    /// <param name="pokerDataList">牌值</param>
    public override void DestoryHavePokerOjbect(List<CardData> pokerDataList)
    {
        if (m_GameBase == null)
        {
            return;
        }
        if (m_GameBase.Bystander)
        {
            int startIndex = 0;
            int endIndex = startIndex + pokerDataList.Count;
            DebugLog.Log("删除1:" + " startIndex " + startIndex + " endIndex " + endIndex + " _ " + pokerDataList.Count + " : " + m_HavePokerList.Count);
            while (startIndex < endIndex)
            {
                GameObject.Destroy(m_HavePokerList[startIndex].m_CartTransform.gameObject);
                ++startIndex;
            }
            startIndex = 0;
            m_HavePokerList.RemoveRange(startIndex, pokerDataList.Count);
        }
        else
        {
            base.DestoryHavePokerOjbect(pokerDataList);
        }
    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="time">发牌阶段时间</param>
    /// <returns></returns>
    public override IEnumerator DealPoker(float time)    {
        if (m_RoleMainUITranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            yield break;
        }

        if(m_GameBase.m_GouJiAssetBundle == null)
        {
            Debug.Log("游戏资源对象为空!<m_GouJiAssetBundle>");
            yield break;
        }

        List<UnityEngine.Transform> CardTransformList = new List<Transform>();

        int index = 0;
        int maxCount = (int)System.Math.Floor(m_HavePokerList.Count / 2.0f);
        UnityEngine.Transform shouPaiTransform = null;
        Object pokerObject = m_GameBase.m_GouJiAssetBundle.LoadAsset("Poker_GJ_shoupai_heng");
        m_CardTransformOne.GetComponent<HorizontalLayoutGroup>().enabled = true;
        m_CardTransformTwo.GetComponent<HorizontalLayoutGroup>().enabled = true;
        for (index = 0; index < m_HavePokerList.Count; ++index)
        {
            shouPaiTransform = ((GameObject)GameMain.instantiate(pokerObject)).transform;
            CardTransformList.Add(shouPaiTransform);
            m_HavePokerList[index].m_CartTransform = shouPaiTransform;
            shouPaiTransform.Find("image_Mask").gameObject.SetActive(false);
            shouPaiTransform.SetParent(index >= maxCount ? m_CardTransformOne : m_CardTransformTwo, false);
            UpdatePoker(m_HavePokerList[index], m_GameBase.Bystander ? "_onlooker" : "_Big");
            m_GameBase.PlayerGameAudio(0,5);
            yield return new WaitForSecondsRealtime(time);
        }
        m_CardTransformOne.GetComponent<HorizontalLayoutGroup>().enabled = false;
        m_CardTransformTwo.GetComponent<HorizontalLayoutGroup>().enabled = false;

        m_HavePokerList.Sort(SortByCardValue);
        m_HavePokerList.Reverse();

        index = 0;
        foreach (UnityEngine.Transform transform in CardTransformList)
        {
            m_HavePokerList[index].m_CartTransform = transform;
            UpdatePoker(m_HavePokerList[index], m_GameBase.Bystander ? "_onlooker" : "_Big");
            byte cardValue = m_HavePokerList[index].m_nCardValue;
            XPointEvent.AutoAddListener(transform.gameObject, OnClickCards, cardValue);
            index++;
        }
        UpdateCardNum();

        if (m_GameBase != null)
        {
            m_GameBase.RefreshCoroutinesDictionary(RoomState_Enum.RoomState_DealPoker);
        }
        yield break;
    }

    /// <summary>
    /// 玩家游戏逻辑推进
    /// </summary>
    public override void OnTick()    {
        base.OnTick();
        UpdateSpeciaAlarm();
    }

    /// <summary>
    /// 服务器提问事件
    /// </summary>
    /// <param name="doing">类型</param>
    /// <param name="askSign">ID</param>
    /// <param name="time">时间</param>
    public void AskdelEvent(RoleDoing_Enum doing, uint askSign, float time,bool guoActive = false)
    {
        ShowRoleDoingPanel(doing,true, askSign, time, guoActive);
    }

    /// <summary>
    /// 服务器提问答复事件
    /// </summary>
    /// <param name="doing">类型</param>
    /// <param name="baseFunctionState">true(执行父类方法)false(不执行父类方法)</param>
    /// <param name="param">参数</param>
    /// <param name="time">文本提示时间</param>
    public override void AnswerDoingEvent(RoleDoing_Enum doing, bool baseFunctionState = true, byte param = 230,float time = 2.0f)
    {
        if (baseFunctionState)
        {
            base.AnswerDoingEvent(doing,baseFunctionState, param, time);
        }
        switch (doing)
        {
            case RoleDoing_Enum.RoleDoing_Init:
            case RoleDoing_Enum.RoleDoing_XuanDian:
            case RoleDoing_Enum.DRoleDoing_YaoTou:
            case RoleDoing_Enum.RoleDoing_GeMing:
            case RoleDoing_Enum.RoleDoing_ShaoPai:
            case RoleDoing_Enum.RoleDoing_FanShao:
                SetAskdelPanelActive(false);
                break;
            case RoleDoing_Enum.RoleDoing_RangPai:
                ShowRoleDoingPanel(RoleDoing_Enum.RoleDoing_ChuPai,false);
                break;
        }
    }

    /// <summary>
    /// 显示玩家行为提示界面
    /// </summary>
    /// <param name="doing"></param> 
    /// <param name="activeState">界面显示状态</param>
    /// <param name="askSign">提问标识</param>
    /// <param name="time">倒计时-时间</param>
    public override void ShowRoleDoingPanel(RoleDoing_Enum doing, bool activeState, uint askSign = 0, float time = 0.0f, bool guoActive = false)
    {
        if (m_RoleMainUITranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }
        if (activeState)
        {
            m_nAskSign = askSign;
        }

        switch (doing)
        {
            case RoleDoing_Enum.RoleDoing_XuanDian:
            case RoleDoing_Enum.DRoleDoing_YaoTou:
            case RoleDoing_Enum.RoleDoing_GeMing:
            case RoleDoing_Enum.RoleDoing_ShaoPai:
            case RoleDoing_Enum.RoleDoing_FanShao:
                {
                    #region"宣点/革命/要头"
                    if (doing == RoleDoing_Enum.RoleDoing_GeMing)
                    {
                        m_AskdelTransform = m_RoleMainUITranform.Find("ButtonBG/ButtonBG_geming");
                    }
                    else if (doing == RoleDoing_Enum.DRoleDoing_YaoTou)
                    {
                        m_AskdelTransform = m_RoleMainUITranform.Find("ButtonBG/ButtonBG_yaotou");
                    }
                    else if (doing == RoleDoing_Enum.RoleDoing_XuanDian)
                    {
                        m_AskdelTransform = m_RoleMainUITranform.Find("ButtonBG/ButtonBG_xuan");
                    }
                    else if (doing == RoleDoing_Enum.RoleDoing_ShaoPai)
                    {
                        m_AskdelTransform = m_RoleMainUITranform.Find("ButtonBG/ButtonBG_shao");
                    }
                    else
                    {
                        m_AskdelTransform = m_RoleMainUITranform.Find("ButtonBG/ButtonBG_shaofan");
                    }

                    Transform GeMingTransform_No = m_AskdelTransform.Find("Button_0");
                    Transform GeMingTransform_Yes = m_AskdelTransform.Find("Button_1");

                    if (activeState)
                    {
                        ResetAskdelPanelInteractable(true);
                        XPointEvent.AutoAddListener(GeMingTransform_No.gameObject, (EventTriggerType eventtype, object message, PointerEventData eventData) =>
                         {
                             if (eventtype == EventTriggerType.PointerClick &&
                                GeMingTransform_No.GetComponent<Button>().interactable)
                             {
                                 ResetAskdelPanelInteractable(false);
                                 SendRoleDoing(RoleDoing_Enum.RoleDoing_Init);
                                 ShowCountdownPanel(false);
                                 if (m_GameBase != null)
                                 {
                                     m_GameBase.PlayerGameAudio(0, 6);
                                 }
                             }
                         }, null);

                        XPointEvent.AutoAddListener(GeMingTransform_Yes.gameObject, (EventTriggerType eventtype, object message, PointerEventData eventData) =>
                         {
                             if (eventtype == EventTriggerType.PointerClick &&
                                 GeMingTransform_Yes.GetComponent<Button>().interactable)
                             {
                                 RoleDoing_Enum roleDoing = (RoleDoing_Enum)message;
                                 ResetAskdelPanelInteractable(false);
                                 SendRoleDoing(roleDoing);
                                 ShowCountdownPanel(false);
                                 if (m_GameBase != null)
                                 {
                                     m_GameBase.PlayerGameAudio(0, 6);
                                 }
                             }
                         }, doing);

                        if (time > 0)
                        {
                            ShowCountdownPanel(true, time, (byte value, bool bClick, Image img, string userdata) =>
                            {
                                ShowCountdownPanel(false);
                                return true;
                            });
                        }
                    }

                    SetAskdelPanelActive(activeState);

                    foreach (Transform transform in m_AskdelTransform)
                    {
                        transform.gameObject.SetActive(activeState);
                    }
                    #endregion
                }
                break;

            case RoleDoing_Enum.RoleDoing_ChuPai:
            case RoleDoing_Enum.RoleDoing_ShaoChuPai:
                {
                    #region"出牌"
                    bool bPromptActive = false;
                    if (activeState)
                    {
                        if (doing == RoleDoing_Enum.RoleDoing_ShaoChuPai)
                        {
                            guoActive = true;
                            m_bShaoPaiFirstPokerState = true;
                        }

                        ResetOutPanelInteractable(true);
                        ResetOutPanelChuPaiInteractable(false);
                        bPromptActive = CollectOutPokerGroupData(guoActive);
                        ShowCountdownPanel(true, time, null);
                        m_fSpeciaAlertTime = time;
                        RefreshGuoPokerPromptUI(!bPromptActive);
                        UpdateOutPokerButtonInteractable();
                    }
                    else
                    {
                        m_fSpeciaAlertTime = -1;
                    }

                    m_GouTransform.gameObject.SetActive(!guoActive);
                    m_ChuPaiTransform.gameObject.SetActive(bPromptActive);
                    m_TiShiTransform.gameObject.SetActive(bPromptActive);
                    m_ChuPaiPanelTransform.gameObject.SetActive(activeState);
                    #endregion
                }
                break;
            case RoleDoing_Enum.RoleDoing_RangPai:
                {
                    m_RangPaiTransform.gameObject.SetActive(activeState);

                    XPointEvent.AutoAddListener(m_RangPaiTransform.gameObject, (EventTriggerType eventtype, object message, PointerEventData eventData) =>
                    {
                        if (eventtype == EventTriggerType.PointerClick &&
                            m_RangPaiTransform.GetComponent<Button>().interactable)
                        {                            ResetOutPanelInteractable(false);

                            SendRoleDoing((RoleDoing_Enum)message);
                            ShowCountdownPanel(false);
                            if (m_GameBase != null)
                            {
                                m_GameBase.PlayerGameAudio(0, 6);
                            }
                        }
                    }, doing);
                }
                break;
        }
    }

    /// <summary>
    /// 显示玩家行为提示界面(用于旁观和录像显示用)
    /// </summary>
    public override void ShowRoleDoingPanel(bool activeState)
    {
        if(m_ChuPaiTransform)
        {
            m_ChuPaiTransform.gameObject.SetActive(activeState);
        }
        if(m_TiShiTransform)
        {
            m_TiShiTransform.gameObject.SetActive(activeState);
        }
        if(m_ChuPaiPanelTransform)
        {
            m_ChuPaiPanelTransform.gameObject.SetActive(activeState);
        }
    }

    /// <summary>
    /// 播放出牌警报声
    /// </summary>
    void UpdateSpeciaAlarm()
    {
        if (m_fSpeciaAlertTime > 0f && m_GameBase != null)
        {
            m_fSpeciaAlertTime -= Time.deltaTime;
            if (m_fSpeciaAlertTime < 5f)
            {
                m_GameBase.PlayerGameAudio(0,7);
                m_fSpeciaAlertTime = -1f;
            }
        }
    }

    /// <summary>
    /// 刷新手牌界面
    /// </summary>
    public override void RefreshHavePokerPanel()
    {
        base.RefreshHavePokerPanel();
    }

    /// <summary>
    /// 出牌重置出牌按钮事件
    /// </summary>
    /// <param name="interactableState">按钮事件状态</param>
    public void ResetOutPanelInteractable(bool interactableState)
    {
        for(int index = 0; index < m_ChuPaiPanelTransform.childCount;++index)
        {
            m_ChuPaiPanelTransform.GetChild(index).GetComponent<Button>().interactable = interactableState;
        }
    }

    /// <summary>
    /// 提问重置出牌按钮事件
    /// </summary>
    /// <param name="interactableState">按钮事件状态</param>
    public void ResetAskdelPanelInteractable(bool interactableState)
    {
        if(m_AskdelTransform == null)
        {
            return;
        }

        for (int index = 0; index < m_AskdelTransform.childCount; ++index)
        {
            m_AskdelTransform.GetChild(index).GetComponent<Button>().interactable = interactableState;
        }
    }

    /// <summary>
    /// 设置提问面板显示状态
    /// </summary>
    /// <param name="active">true(显示)false(关闭)</param>
    public void SetAskdelPanelActive(bool active)
    {
        if (m_AskdelTransform == null)
        {
            return;
        }
        m_AskdelTransform.gameObject.SetActive(active);
    }

    /// <summary>
    /// 重置出牌按钮事件
    /// </summary>
    /// <param name="interactableState">按钮事件状态</param>
    public void ResetOutPanelChuPaiInteractable(bool interactableState)
    {
        if (m_ChuPaiTransform)
        {
            m_ChuPaiTransform.GetComponent<Button>().interactable = interactableState;
        }
    }

    /// <summary>
    /// 重置出牌状态
    /// </summary>
    /// <param name="state">true：全部重置,false :重置除大小王和2的牌</param>
    public override void ResetOutPokerPanel(bool state = true)
    {
        if (m_OutPokerList.Count == 0)
        {
            return;
        }

        m_CardTransformOne.GetComponent<HorizontalLayoutGroup>().enabled = false;
        m_CardTransformTwo.GetComponent<HorizontalLayoutGroup>().enabled = false;

        //重置上一次选择的出牌数据
        int curCardValue = 0;
        foreach (var data in m_OutPokerList)
        {
            if(!state)
            {
                curCardValue = GameCommon.GetCardValue(data.m_nCardValue);
                if(curCardValue <= 0  || curCardValue== 2)
                {
                    continue;
                }
            }
            data.m_bSelectState = !data.m_bSelectState;
            data.m_CartTransform.DOLocalMoveY(data.m_bSelectState ? data.m_fCardYPostion + 30 : data.m_fCardYPostion, 0.2f);
        }
        m_OutPokerList.RemoveAll(cardData =>
        {
            if(!state)
            {
               if(GameCommon.GetCardValue(cardData.m_nCardValue) <=0 || GameCommon.GetCardValue(cardData.m_nCardValue) == 2)
                {
                    return false;
                }
            }
            return true;
        });
    }

    /// <summary>
    ///获取手牌中补牌数
    /// </summary>
    /// <param name="pokerValue">当前牌值</param>
    /// <param name="blackJokerCount">小王数</param>
    /// <param name="redJokerCount">大王数</param>
    /// <param name="twoCardCount">2数</param>
    /// <returns>补牌数</returns>
    int GetHavePatchPokerValueCount(int pokerValue,int totalPokerCount, int blackJokerCount,int redJokerCount,int twoCardCount)
    {
        int totalNum = 0;
        switch (pokerValue)
        {
            case 16:    //小王
                totalNum = redJokerCount + twoCardCount;
                break;
            case 17:     //大王
                totalNum = blackJokerCount + twoCardCount;
                break;
            case 15:     //2
                totalNum = blackJokerCount + redJokerCount;
                break;
            default:
                totalNum = blackJokerCount + redJokerCount + twoCardCount;
                break;
        }
        return totalNum;
    }

    /// <summary>
    /// 解析牌的数据
    /// </summary>
    /// <param name="cardValue">需要解析牌的数据列表</param>
    /// <param name="cardValue">出去2,大小王的牌</param>
    /// <param name="rJCount">大王牌数</param>
    /// <param name="bJCount">小王牌数</param>
    /// <param name="tCount">2牌数</param>
    /// <returns>出去大小王和2以后牌种类数</returns>
    int GetAnalysisPokerDataList(List<CardData> pokerDataList, out byte cardValue, out int rJCount, out int bJCount, out int tCount)
    {
        cardValue = 0;
        rJCount = bJCount = tCount = 0;
        int curCardValue = -2;
        HashSet<int> pokerGroup = new HashSet<int>();
        foreach (var pokerData in pokerDataList)
        {
            if (pokerData.m_nCardValue == 0x4F)
            {
                rJCount++;
            }
            else if (pokerData.m_nCardValue == 0x4E)
            {
                bJCount++;
            }
            else
            {
                curCardValue = GameCommon.GetCardValue(pokerData.m_nCardValue);
                if (curCardValue == 2)
                {
                    tCount++;
                }
                else
                {
                    pokerGroup.Add(curCardValue);
                    cardValue = pokerData.m_nCardValue;
                }
            }
        }
        return pokerGroup.Count;
    }

    /// <summary>
    /// 解析最近出牌的数据
    /// </summary>
    /// <param name="cardValue">出去2,大小王的牌</param>
    /// <param name="rJCount">大王牌数</param>
    /// <param name="bJCount">小王牌数</param>
    /// <param name="tCount">2牌数</param>
    /// <returns>出去大小王和2以后牌种类数</returns>
    int GetAnalysisRecentlyPokerData(out byte cardValue,out int rJCount,out int bJCount,out int tCount)
    {
        cardValue = 0;
        rJCount = bJCount = tCount = 0;
        int curCardValue = -2;
        HashSet<int> pokerGroup = new HashSet<int>();
        foreach (var pokerValue in m_GameBase.m_RecentlyOutPokerList)
        {
            if (pokerValue == 0x4F)
            {
                rJCount++;
            }
            else if (pokerValue == 0x4E)
            {
                bJCount++;
            }
            else
            {
                curCardValue = GameCommon.GetCardValue(pokerValue);
                if(curCardValue == 2)
                {
                    tCount++;
                }else
                {
                    pokerGroup.Add(curCardValue);
                    cardValue = pokerValue;
                }
            }
        }

        #region "初始化默认值"
        if (cardValue == 0 && (rJCount + bJCount + tCount > 0))
        {
            if(tCount > 0)
            {
                cardValue = 2;
            }else if(bJCount > 0)
            {
                cardValue = 0x4e;
            }else
            {
                cardValue = 0x4f;
            }
        }
        #endregion

        return pokerGroup.Count;
    }

    /// <summary>
    /// 解析当前出牌的数据
    /// </summary>
    /// <param name="cardValue">出去2,大小王的牌</param>
    /// <param name="rJCount">大王牌数</param>
    /// <param name="bJCount">小王牌数</param>
    /// <param name="tCount">2牌数</param>
    /// <returns>出去大小王和2以后牌种类数</returns>
    int GetAnalysisCurrentPokerData(out byte cardValue, out int rJCount, out int bJCount, out int tCount)
    {
        int curCardValue = GetAnalysisPokerDataList(m_OutPokerList,out cardValue,out rJCount, out bJCount,out tCount);

        #region "初始化默认值"
        if (cardValue == 0 && (rJCount + bJCount + tCount > 0))
        {
            if (tCount > 0)
            {
                cardValue = 2;
            }
            else if (bJCount > 0)
            {
                cardValue = 0x4e;
            }
            else
            {
                cardValue = 0x4f;
            }
        }
        #endregion

        return curCardValue;
    }

    /// <summary>
    /// 获取所有牌的总值
    /// </summary>
    /// <param name="pokerDataList">所有牌</param>
    /// <returns>所有牌的总值</returns>
    int GetOutPokerValue(List<CardData> pokerDataList)
    {
        int totalPokerValue = 0;
        foreach(var cardData in pokerDataList)
        {
            totalPokerValue += LL_PokerType.GetPokerLogicValue(cardData.m_nCardValue);
        }
        return totalPokerValue;
    }

    /// <summary>
    /// 获取所有牌的总值
    /// </summary>
    /// <param name="pokerDataList">所有牌</param>
    /// <returns>所有牌的总值</returns>
    int GetOutPokerValue(List<byte> pokerDataList)
    {
        int totalPokerValue = 0;
        foreach (var cardData in pokerDataList)
        {
            totalPokerValue += LL_PokerType.GetPokerLogicValue(cardData);
        }
        return totalPokerValue;
    }

    /// <summary>
    /// 出牌提示数据
    /// </summary>
    /// <param name="pokerAuthority">牌权状态：true 有牌权，false 没有</param>
    public bool CollectOutPokerGroupData(bool pokerAuthority)
    {
        if(m_GameBase == null)
        {
            return false;
        }


        byte defaultCardValue = 0;
        m_nDiscardPromptPokerDataIndex = -1;
        int redJokerCount = 0, blackJokerCount = 0,twoCount = 0;
        GetAnalysisRecentlyPokerData(out defaultCardValue,out redJokerCount, out blackJokerCount, out twoCount);
        List<CardData> havePokerList = new List<CardData>(m_HavePokerList);
        int outPokerTotalCount = m_GameBase.m_RecentlyOutPokerList.Count;
        if (m_GameBase.IsGameRule(GouJiRule_Enum.eGJR_BeiShan))
        {
            if (havePokerList.Count != 1)
            {
                havePokerList.RemoveAll(cardData => 3 == GameCommon.GetCardValue(cardData.m_nCardValue));
            }
        }
        List<CardData> redJokerHavePokerList = havePokerList.FindAll(cardData => 0x4F == cardData.m_nCardValue);

        bool twoShaYiState = m_GameBase.IsGameRule(GouJiRule_Enum.eGJR_TwoShaYi) && redJokerCount > 0;
        if (!pokerAuthority)
        {
            if (twoShaYiState)
            {
                if(redJokerHavePokerList.Count < redJokerCount * 2)
                {
                    return false;
                }
            }else
            {
                if(redJokerCount > 0)
                {
                    return false;
                }
            }
        }

        if (blackJokerCount > 0 && redJokerHavePokerList.Count < blackJokerCount && !pokerAuthority)
        {
            return false;
        }

        int index = 0;
        m_nPromptPokerDataIndex = 0;
        m_PromptPokerDataDictionary.Clear();
        int mixedCount = redJokerCount + blackJokerCount;
        int pokerCount = Mathf.Max(outPokerTotalCount - mixedCount, 0);
        List<CardData> blackJokerHavePokerList = havePokerList.FindAll(cardData => 0x4E == cardData.m_nCardValue);
        List<CardData> TwoCardHavePokerList = havePokerList.FindAll(cardData => 2 == GameCommon.GetCardValue(cardData.m_nCardValue));
        IEnumerable<IGrouping<int, CardData>> CardDataGroup = havePokerList.GroupBy(value => GameCommon.GetCardValue(value.m_nCardValue));

        if (pokerAuthority || outPokerTotalCount == 0)
        {
            if(!m_bShaoPaiFirstPokerState || (m_bShaoPaiFirstPokerState && (!pokerAuthority || outPokerTotalCount == 0)))
            {
                foreach (var cardData in CardDataGroup.Reverse())
                {
                    m_PromptPokerDataDictionary.Add(index, cardData.ToList());
                    index++;
                }
                return m_PromptPokerDataDictionary.Count > 0;
            }
        }else
        {
            if (pokerCount == 0)
            {
                if (blackJokerCount > 0)
                {
                    pokerCount = blackJokerCount;
                }
                else
                {
                    pokerCount = redJokerCount;
                }
            }
        }

        IEnumerable<IGrouping<int, CardData>> OutPokerSameGroup = CardDataGroup.Where(data => data.ToList<CardData>().Count == pokerCount);  //相同
        IEnumerable<IGrouping<int, CardData>> OutPokerLessGroup = CardDataGroup.Where(data => data.ToList<CardData>().Count <  pokerCount);  //小于
        IEnumerable<IGrouping<int, CardData>> OutPokerMoreGroup = CardDataGroup.Where(data => data.ToList<CardData>().Count >  pokerCount);  //大于

        int moreGroupCount = -1,curGroupIndex = -1;
        List < IGrouping<int, CardData>> OutPokerGroupList = new List<IGrouping<int, CardData>>();
        OutPokerGroupList.AddRange(OutPokerSameGroup.Reverse());
        OutPokerGroupList.AddRange(OutPokerLessGroup.Reverse());
        moreGroupCount = OutPokerGroupList.Count;
        OutPokerGroupList.AddRange(OutPokerMoreGroup.Reverse());

        int curPokerValue = 0, curPokerCount = 0, totalPomptDataCount = 0,addPokerCount =0, redJokerUseCount = 0;
        foreach (var cardDataList in OutPokerGroupList)
        {
            redJokerUseCount = 0;
            ++curGroupIndex;
            curPokerCount = outPokerTotalCount + (twoShaYiState ? redJokerCount : 0);
            curPokerValue = LL_PokerType.GetPokerLogicValue(cardDataList.First().m_nCardValue);
            if (curPokerValue > LL_PokerType.GetPokerLogicValue(defaultCardValue) || (defaultCardValue == 0x4F && curPokerValue == 17))
            {
                
                if (cardDataList.Count() + GetHavePatchPokerValueCount(curPokerValue, outPokerTotalCount, blackJokerHavePokerList.Count(),
                                                                       redJokerHavePokerList.Count(), TwoCardHavePokerList.Count()) < outPokerTotalCount)
                {
                    continue;
                }

                if (blackJokerCount > 0 || redJokerCount > 0)
                {
                    if (redJokerHavePokerList.Count < blackJokerCount)
                    {
                        continue;
                    }
                    m_PromptPokerDataDictionary.Add(index,new List<CardData>());
                    addPokerCount = twoShaYiState ? redJokerCount * 2 + blackJokerCount : blackJokerCount;

                    curPokerCount -= addPokerCount;//大王数必须要减掉
                    if (addPokerCount <= redJokerHavePokerList.Count && addPokerCount > 0)
                    {
                        m_PromptPokerDataDictionary[index].AddRange(redJokerHavePokerList.GetRange(0, addPokerCount));
                        redJokerUseCount += addPokerCount;
                    }

                    if (curPokerValue != 17 && curPokerCount > 0)
                    {
                        addPokerCount = cardDataList.Count() >= curPokerCount ? curPokerCount : cardDataList.Count();
                        m_PromptPokerDataDictionary[index].AddRange(cardDataList.ToList().GetRange(0, addPokerCount));
                        curPokerCount -= addPokerCount;
                    }
                }
                else
                {
                    addPokerCount = cardDataList.Count() >= curPokerCount ? curPokerCount : cardDataList.Count();
                    m_PromptPokerDataDictionary.Add(index, cardDataList.ToList().GetRange(0, addPokerCount));
                    curPokerCount -= addPokerCount;
                }

                //补牌2
                if (curPokerCount > 0 && TwoCardHavePokerList.Count > 0 && curPokerValue != 15 &&
                    LL_PokerType.GetPokerLogicValue(2) > LL_PokerType.GetPokerLogicValue(defaultCardValue))
                {
                    addPokerCount = TwoCardHavePokerList.Count >= curPokerCount ? curPokerCount : TwoCardHavePokerList.Count;
                    m_PromptPokerDataDictionary[index].InsertRange(0, TwoCardHavePokerList.GetRange(0, addPokerCount));
                    curPokerCount -= addPokerCount;
                }

                //补小王
                if (curPokerCount > 0 && blackJokerHavePokerList.Count > 0 && curPokerValue != 16 &&
                    LL_PokerType.GetPokerLogicValue(0x4E) > LL_PokerType.GetPokerLogicValue(defaultCardValue))
                {
                    addPokerCount = blackJokerHavePokerList.Count >= curPokerCount ? curPokerCount : blackJokerHavePokerList.Count;
                    m_PromptPokerDataDictionary[index].InsertRange(0, blackJokerHavePokerList.GetRange(0, addPokerCount));
                    curPokerCount -= addPokerCount;
                }

                //补大王
                if (curPokerCount > 0 && (redJokerHavePokerList.Count - redJokerUseCount) >= curPokerCount && curPokerValue != 17)
                {
                    m_PromptPokerDataDictionary[index].InsertRange(0, redJokerHavePokerList.GetRange(redJokerUseCount, curPokerCount));
                    redJokerUseCount += curPokerCount;
                }

                byte cardValue = 0;
                int rJCount = 0, bJCount = 0, tCount = 0, groupCount = 0;
                groupCount = GetAnalysisPokerDataList(m_PromptPokerDataDictionary[index],out cardValue, out rJCount, out bJCount, out tCount);

                totalPomptDataCount = m_PromptPokerDataDictionary.Count(pokerDataList => 
                {
                    byte _cardValue = 0;
                    int _rJCount = 0, _bJCount = 0, _tCount = 0,_groupCount = 0;
                    _groupCount = GetAnalysisPokerDataList(pokerDataList.Value, out _cardValue, out _rJCount, out _bJCount, out _tCount);
                    return groupCount == _groupCount && _rJCount == rJCount && bJCount == _bJCount && tCount == _tCount && cardValue == _cardValue;
                 });

                if (outPokerTotalCount != 
                   (twoShaYiState ? m_PromptPokerDataDictionary[index].Count - redJokerCount : m_PromptPokerDataDictionary[index].Count) || 
                    totalPomptDataCount > 1)
                {
                    m_PromptPokerDataDictionary.Remove(m_PromptPokerDataDictionary.Last().Key);
              
                }else
                {
                    if(curGroupIndex >= moreGroupCount && m_nDiscardPromptPokerDataIndex < 0)
                    {
                        m_nDiscardPromptPokerDataIndex = index;
                    }
                    index++;
                }
            }
        }

        return m_PromptPokerDataDictionary.Count > 0;
    }

    /// <summary>
    /// 发送服务端提问
    /// </summary>
    /// <param name="doingState">提问行为状态</param>
    void SendRoleDoing(RoleDoing_Enum doingState)
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_CM_ANSWERDOING);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((byte)doingState);
        msg.Add(m_nAskSign);
        HallMain.SendMsgToRoomSer(msg);
        DebugLog.Log("发送服务端提问:" + doingState);
    }

    /// <summary>
    /// 发送出牌数据
    /// </summary>
    void SendRolePoker()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_CM_PLAYERDEALMJPOKER);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((byte)m_OutPokerList.Count);
        foreach (var pokerData in m_OutPokerList)
        {
            DebugLog.Log("****** 已经出的牌:" + GameCommon.GetCardValue(pokerData.m_nCardValue) +"牌数:" + m_OutPokerList.Count);
            msg.Add(pokerData.m_nCardValue);
        }
        HallMain.SendMsgToRoomSer(msg);
        m_bShaoPaiFirstPokerState = false;
        if(m_GameBase != null)
        {
            m_GameBase.SetTrustButtonActiveState(false);
        }
        DebugLog.Log("============出牌!!===============");
    }

    /// <summary>
    /// 请求队友手牌数据
    /// </summary>
    /// <param name="value">0(服务器选择)1(上联)2(下联)</param>
    public void RequestRoleWatchingFriendPoker(byte value)
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_GOUJI_CM_CHANGEFRIEND);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(value);
        HallMain.SendMsgToRoomSer(msg);
    }

    /// <summary>
    /// 鼠标选择出牌动画
    /// </summary>
    /// <param name="SelectCardData">鼠标当前选中的牌</param>
    public void SetSelectPokerUIAnimation(CardData SelectCardData)
    {
        m_CardTransformOne.GetComponent<HorizontalLayoutGroup>().enabled = false;
        m_CardTransformTwo.GetComponent<HorizontalLayoutGroup>().enabled = false;

        SelectCardData.m_bSelectState = !SelectCardData.m_bSelectState;
        if (!SelectCardData.m_bSelectState)
        {
            m_OutPokerList.Remove(SelectCardData);
        }
        else
        {
            m_OutPokerList.Add(SelectCardData);
        }

        if (float.IsNaN(SelectCardData.m_fCardYPostion))
        {
            SelectCardData.m_fCardYPostion = SelectCardData.m_CartTransform.localPosition.y;
        }
        SelectCardData.m_CartTransform.DOLocalMoveY(SelectCardData.m_bSelectState ? SelectCardData.m_fCardYPostion + 30 : SelectCardData.m_fCardYPostion, 0.2f);
    }

    /// <summary>
    /// 设置鼠标单选牌出牌动画
    /// </summary>
    /// <param name="cardValue">鼠标选中的牌值</param>
    /// <param name="eventData">鼠标点击事件数据</param>
    public void SetSinglePokerUIAnimation(byte cardValue,PointerEventData eventData)
    {
        CardData findCardData = m_HavePokerList.Find(cardData => GameCommon.GetCardValue(cardData.m_nCardValue) == GameCommon.GetCardValue(cardValue) &&
                                                                 cardData.m_CartTransform.name == eventData.pointerPressRaycast.gameObject.name);
        if (findCardData != null)
        {
            SetSelectPokerUIAnimation(findCardData);
        }
    }

    /// <summary>
    /// 更新出牌按钮鼠标事件
    /// </summary>
    public void UpdateOutPokerButtonInteractable()
    {
        bool bInteractable = false;
        byte pokerValue = 0,curPokerValue = 0;
        int totalPokerCount = m_GameBase.m_RecentlyOutPokerList.Count;
        bool beiSanState = m_GameBase.IsGameRule(GouJiRule_Enum.eGJR_BeiShan);
        int rJCount = 0, bJCount = 0, tCount = 0, curRJCount = 0, curBJCount = 0, curTCount = 0;
        int recentCount = GetAnalysisRecentlyPokerData(out pokerValue, out rJCount, out bJCount, out tCount);
        int currentCount = GetAnalysisCurrentPokerData(out curPokerValue, out curRJCount, out curBJCount, out curTCount);
        if ((recentCount == 1 || recentCount == 0) && (currentCount == 1 || currentCount == 0))
        {
            int recentRjCount = bJCount,recentPokerCount = totalPokerCount;
            bool pokerValueState = LL_PokerType.GetPokerLogicValue(curPokerValue) > LL_PokerType.GetPokerLogicValue(pokerValue); 
            if (m_GameBase.IsGameRule(GouJiRule_Enum.eGJR_TwoShaYi) && rJCount > 0)
            {
                recentPokerCount += rJCount;
                recentRjCount += rJCount*2;
                if(curPokerValue == pokerValue && curPokerValue == 0x4F)
                {
                    pokerValueState = true;
                }
            }

            if (curRJCount >= recentRjCount && pokerValueState &&
                ((totalPokerCount > 0 && m_OutPokerList.Count == recentPokerCount) || totalPokerCount == 0))
            {
                bInteractable = true;
            }
        }
        if (m_GameBase != null && bInteractable && m_GameBase.m_nRecentlyShaoPaiSit == m_nSSit && 
           (m_bShaoPaiFirstPokerState || m_bShaoPaiSucceedState))
        {
            if(m_OutPokerList.Find(value => value.m_nCardValue == 0x4f) == null &&
               m_OutPokerList.Find(value => value.m_nCardValue == 0x4e) == null && !m_bShaoPaiFirstPokerState)
            {
                bInteractable = false;
                if (beiSanState && m_HavePokerList.Count == 1)
                {
                    bInteractable = true;
                }
            }

            if(bInteractable)
            {
                List<CardData> remainHavePokerList = new List<CardData>(m_HavePokerList);
                if (beiSanState)
                {
                    remainHavePokerList.RemoveAll(value => GameCommon.GetCardValue(value.m_nCardValue) == 3);
                }
                foreach (var pokerData in m_OutPokerList)
                {
                    remainHavePokerList.Remove(pokerData);
                }

                byte defaultCardValue = 0;
                int rJ_Count = 0, bJ_Count = 0, t_Count = 0;
                int curCount = GetAnalysisPokerDataList(remainHavePokerList, out defaultCardValue, out rJ_Count, out bJ_Count, out t_Count);
                
                if(curCount == 0 && t_Count > 0)
                {
                    curCount++;
                }

                if (rJ_Count + bJ_Count < curCount)
                {
                    bInteractable = false;
                }
            }
        }

        if (bInteractable && m_GameBase.IsGameRule(GouJiRule_Enum.eGJR_BeiShan))
        {
            if(m_OutPokerList.Find(value => GameCommon.GetCardValue(value.m_nCardValue) == 3) != null)
            {
                if ((m_HavePokerList.Count - m_OutPokerList.Count) != 0 || m_OutPokerList.Count != 1)
                {
                    bInteractable = false;
                }
            }
        }

        ResetOutPanelChuPaiInteractable(bInteractable);
    }

    /// <summary>
    /// 围观点击事件
    /// </summary>
    void OnClickWathching(EventTriggerType eventtype, object message, PointerEventData eventData)    {
        Button watchingButton = (Button)message;
        if(watchingButton == null)
        {
            return;
        }
        if (eventtype == EventTriggerType.PointerClick && watchingButton.interactable)
        {
            watchingButton.interactable = false;
            RequestRoleWatchingFriendPoker((byte)(watchingButton.name == "Button_0" ? 1 : 2));
            if(m_GameBase != null)
            {
                m_GameBase.PlayerGameAudio(0, 6);
            }
        }
    }

    /// <summary>
    /// 单张牌点击事件
    /// </summary>
    void OnClickCards(EventTriggerType eventtype, object message, PointerEventData eventData)    {        switch (eventtype)
        {
            case EventTriggerType.PointerClick:
                {
                    if (m_GameBase != null)
                    {
                        m_GameBase.PlayerGameAudio(0, 6);
                    }
                    byte cardValue = (byte)message;
                    DebugLog.Log("选中的牌值: " + cardValue + "通用值: " + GameCommon.GetCardValue(cardValue));
                    if(m_OutPokerList.Count > 0)
                    {
                        bool findValueState = false,findNameState = false;
                        int curCardValue = GameCommon.GetCardValue(cardValue);
                        foreach (var data in m_OutPokerList)
                        {
                            if(GameCommon.GetCardValue(data.m_nCardValue) == curCardValue)
                            {
                                findValueState = true;
                                if (eventData.pointerPressRaycast.gameObject.name == data.m_CartTransform.name)
                                {
                                    findNameState = true;
                                    SetSelectPokerUIAnimation(data);
                                    break;
                                }
                            }
                        }
                        if(findValueState)
                        {
                            if(!findNameState)
                            {
                                SetSinglePokerUIAnimation(cardValue, eventData);
                            }
                            UpdateOutPokerButtonInteractable();
                            return;
                        }

                        //大王,小王，和2
                        int CardValue1 = GameCommon.GetCardValue(cardValue);
                        if (CardValue1 == 0 || CardValue1 == -1 || CardValue1 == 2)
                        {
                            SetSinglePokerUIAnimation(cardValue, eventData);
                            UpdateOutPokerButtonInteractable();
                            return;
                        }

                        //重置上一次选择的出牌数据
                        ResetOutPokerPanel(false);
                    }

                    int index = 0,totalPokerCount = m_GameBase.m_RecentlyOutPokerList.Count;
                    List<CardData> findAllPokerList = m_HavePokerList.FindAll(cardData => GameCommon.GetCardValue(cardData.m_nCardValue) == GameCommon.GetCardValue(cardValue));
                    foreach (CardData cardData in findAllPokerList)
                    {
                        if (totalPokerCount != 0 && index >= totalPokerCount)
                        {
                            break;
                        }
                        SetSelectPokerUIAnimation(cardData);
                        ++index;
                    }

                    UpdateOutPokerButtonInteractable();
                }
                break;
        }    }
}
