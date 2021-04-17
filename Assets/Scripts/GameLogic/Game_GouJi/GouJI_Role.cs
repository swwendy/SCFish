
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XLua;

/// <summary>
/// 够级玩家
/// </summary>
[Hotfix]
public class GouJi_Role
{
    /// <summary>
    /// 玩家ID
    /// </summary>
    public uint m_nUseId;

    /// <summary>
    /// 玩家图标ID
    /// </summary>
    public int m_nFaceId;

    /// <summary>
    /// 玩家图标网络资源定位符
    /// </summary>    public string m_sUrl;

    /// <summary>
    /// 玩家钻石
    /// </summary>    public long m_nTotalCoin;

    /// <summary>
    /// 玩家名称
    /// </summary>    public string m_sRoleName;

    /// <summary>
    /// 玩家服务端座位号
    /// </summary>    public byte m_nSSit;

    /// <summary>
    /// 玩家客户端座位号
    /// </summary>    public byte m_nCSit;

    /// <summary>
    /// 玩家性别
    /// </summary>    public byte m_nSex = 0;

    /// <summary>
    /// 玩家名次
    /// </summary>
    public sbyte m_nRank = -1;

    /// <summary>
    /// 大师分
    /// </summary>
    public float m_fMasterScore = 0.0f;

    /// <summary>
    /// 在线状态1:在线,0:离开
    /// </summary>
    public byte m_nDisconnectState = 0;

    /// <summary>
    /// 查看队友手牌座位号
    /// </summary>
    public bool m_bLookAtRoleState;

    /// <summary>
    /// 是否准备状态
    /// </summary>
    public byte m_nReady;

    /// <summary>
    /// 开点类型
    /// </summary>
    public KaiDianType_Enum m_eKaiDianType;

    /// <summary>
    /// 宣点动画播状态
    /// </summary>
    public bool m_bXuanDianAnimationState;

    /// <summary>
    /// 要头类型
    /// </summary>
    public YaoTouType_Enum m_eYaoTouType;

    /// <summary>
    /// 够级游戏对象
    /// </summary>
    public CGame_GouJi m_GameBase = null;

    /// <summary>
    /// 角色面板
    /// </summary>
    public UnityEngine.Transform m_RoleMainUITranform = null;

    /// <summary>
    /// 手牌挂节点对象
    /// </summary>
    public UnityEngine.Transform m_CardTransformOne, m_CardTransformTwo;

    /// <summary>
    /// 角色动画挂节点对象
    /// </summary>
    private Transform m_AnimationTransform;

    /// <summary>
    /// 角色交换牌位置对象
    /// </summary>
    public Transform m_PokerSwapTransform;

    /// <summary>
    /// 手牌列表
    /// </summary>
    public List<CardData> m_HavePokerList = new List<CardData>();

    /// <summary>
    /// 出牌选中列表
    /// </summary>
    public List<CardData> m_OutPokerList = new List<CardData>();

    /// <summary>
    /// 被憋三标记
    /// </summary>
    public bool m_bBeiThreeState;

    /// <summary>
    /// 烧牌成功标记
    /// </summary>
    public bool m_bShaoPaiSucceedState;

    /// <summary>
    /// 手牌剩余数量小于10标识
    /// </summary>
    bool m_bRemainPokerNumState;

    /// <summary>
    /// 动画开始播放状态
    /// </summary>
    bool m_bPlayerAnimationState;

    /// <summary>
    /// 动画列表
    /// </summary>
    List<string> m_RoleAnimationNameList = new List<string>();

    /// <summary>
    /// 烧牌动画
    /// </summary>
    GameObject m_ShaoPaiAnimationObject;

    /// <summary>
    /// 角色徽章
    /// </summary>
    Dictionary<RoleBadge_Enum, GameObject> m_RoleBadgeDictionary = new Dictionary<RoleBadge_Enum, GameObject>();

    /// <summary>
    /// 行牌提示协程对象
    /// </summary>
    IEnumerator m_pPromptTextEnumerator;

    public GouJi_Role(CGame_GouJi gameBase, byte cSit)    {        m_GameBase = gameBase;
        m_nUseId = 0;
        m_nFaceId = 0;
        m_sUrl = string.Empty;
        m_nTotalCoin = 0;
        m_sRoleName = string.Empty;
        m_nSSit = RoomInfo.NoSit;
        m_nCSit = RoomInfo.NoSit;
        m_fMasterScore = 0;
        m_nDisconnectState = 0;        m_nCSit = cSit;        m_nRank = -1;        m_nReady = 0;        m_bRemainPokerNumState = false;        m_pPromptTextEnumerator = null;        m_bPlayerAnimationState = true;        m_bBeiThreeState = false;        m_bLookAtRoleState = false;        m_eKaiDianType = KaiDianType_Enum.eKDT_None;        m_eYaoTouType = YaoTouType_Enum.eYTT_None;        m_bXuanDianAnimationState = false;        m_RoleAnimationNameList.Clear();        m_RoleBadgeDictionary.Clear();        m_ShaoPaiAnimationObject = null;        m_PokerSwapTransform = null;        m_bShaoPaiSucceedState = false;        m_RoleMainUITranform = m_GameBase.GetGameRoleMainUITransform(m_nCSit + 1);        m_AnimationTransform = m_GameBase.GetGameRoleAnimationTransform(m_nCSit + 1);        m_RoleMainUITranform.gameObject.SetActive(false);        CreateSwapPokerSocket();    }

    /// <summary>
    /// 重置角色数据
    /// </summary>
    /// <param name="bDestroryState">true:全部都删除false:删除一部分</param>
    public virtual void ResetRoleData(bool bDestroryState = true)
    {
        if(bDestroryState)
        {
            DestoryAllHavePokerObject();
        }
        DestoryAllOutPokerObject();
        DestoryAllLayPokerObject();
        ShowCountdownPanel(false);
        ShowGeMingYaotouInfoUI(false);
        RefreshRemainPokerPromptUI(false);
        SetTalkPanelActive(false);
        RefreshOutPokerPromptUI(0);
        RefreshRoleRankUI(-1);
        m_OutPokerList.Clear();
        m_HavePokerList.Clear();
        m_nRank = -1;
        m_bRemainPokerNumState = false;
        m_pPromptTextEnumerator = null;
        m_RoleAnimationNameList.Clear();
        m_bPlayerAnimationState = true;
        m_bBeiThreeState = false;
        m_bLookAtRoleState = false;
        m_bShaoPaiSucceedState = false;
        m_eKaiDianType = KaiDianType_Enum.eKDT_None;        m_eYaoTouType = YaoTouType_Enum.eYTT_None;        m_bXuanDianAnimationState = false;        for (RoleBadge_Enum badge = RoleBadge_Enum.eRB_XuanDain; badge < RoleBadge_Enum.eRB_Max; ++badge)
        {
            RefreshBadgeObject(badge, false);
        }
        PlayShaoPaiAnimation(false);
        UpdatePromptText(string.Empty);
    }

    /// <summary>
    /// 刷新角色信息面板
    /// </summary>
    public void RefreshInfoUI()
    {
        if (m_RoleMainUITranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }

        if (m_GameBase == null)
        {
            Debug.Log("游戏对象为空!<m_GameBase>");
            return;
        }

        Transform tfm = m_RoleMainUITranform.Find("Playerinfo/Head/HeadMask/ImageHead");        tfm.GetComponent<Image>().sprite = GameMain.hall_.GetIcon(m_sUrl, m_nUseId, m_nFaceId);        tfm = m_RoleMainUITranform.Find("Playerinfo/BG_nameJifen/Name_Text");        tfm.GetComponent<Text>().text = m_sRoleName;

        tfm = m_RoleMainUITranform.Find("Playerinfo/Text_dashifen");        tfm.GetComponent<Text>().text = CCsvDataManager.Instance.GameDataMgr.GetMasterLv(m_fMasterScore);

        if (m_GameBase.m_GouJiAssetBundle)
        {
            string imageName = m_nSSit % 2 > 0 ? "gj_frame_tx_lan" : "gj_frame_tx_huang";
            m_RoleMainUITranform.Find("Playerinfo/Head/HeadOutline (1)").GetComponent<Image>().sprite = m_GameBase.m_GouJiAssetBundle.LoadAsset<Sprite>(imageName);
        }

        RefreshRoleMoneyUI();
        RefreshRoleOfflineUI();

        m_RoleMainUITranform.gameObject.SetActive(true);
    }

    /// <summary>
    /// 刷新玩家货币(钻石或积分)
    /// </summary>
    public void RefreshRoleMoneyUI()
    {
        if (m_RoleMainUITranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }

        Transform transfrom = m_RoleMainUITranform.Find("Playerinfo/BG_nameJifen/Jifen_BG");        if (!m_GameBase.m_bIsFree)        {
            transfrom.gameObject.SetActive(true);
            transfrom.Find("Text_Jifen").GetComponent<Text>().text = m_nTotalCoin.ToString();
        }        else
        {
            transfrom.gameObject.SetActive(false);        }
    }

    /// <summary>
    /// 刷新角色离线标志
    /// </summary>
    public void RefreshRoleOfflineUI()
    {
        if (m_RoleMainUITranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }
        m_RoleMainUITranform.Find("Text_offline").gameObject.SetActive(m_nDisconnectState == 0);
    }

    /// <summary>
    /// 刷新角色名次图标
    /// </summary>
    /// <param name="rank">角色走完牌的名次</param>
    public void RefreshRoleRankUI(sbyte rank)
    {
        if (m_RoleMainUITranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }

        Transform RoleRankTransform = m_RoleMainUITranform.Find("Tip_BG/Image_ranking");
        rank += 1;
        if (rank > 0)
        {
            if (m_GameBase.m_GouJiAssetBundle)
            {
                if (m_nRank == -1 && rank < 3)
                {
                    m_GameBase.PlayerGameAudio(m_nCSit, 11 + rank);
                }
                m_nRank = rank;
                string imageName = "Gj_PaiMing_" + m_nRank;
                RoleRankTransform.GetComponent<Image>().sprite = m_GameBase.m_GouJiAssetBundle.LoadAsset<Sprite>(imageName);
            }

        }
        RoleRankTransform.gameObject.SetActive(rank > 0);
    }

    /// <summary>
    /// 刷新出牌数提示界面
    /// </summary>
    /// <param name="pokerNum">出牌数</param>
    public void RefreshOutPokerPromptUI(int pokerNum)
    {
        if (m_RoleMainUITranform == null)
        {
            return;
        }

        bool activeState = pokerNum >= 5;
        Transform chuPaiPromptTransform = m_RoleMainUITranform.Find("Tip_BG/Text_ChupaiNum");
        chuPaiPromptTransform.GetComponent<Text>().text = pokerNum + "张";
        chuPaiPromptTransform.gameObject.SetActive(activeState);
    }

    /// <summary>
    /// 刷新出牌界面
    /// </summary>
    public void RefreshOutPokerPanel()
    {
        if (m_RoleMainUITranform == null ||
            m_GameBase.m_GouJiAssetBundle == null)
        {
            return;
        }

        DestoryAllOutPokerObject();

        Object pokerObject = m_GameBase.m_GouJiAssetBundle.LoadAsset("Poker_GJ_Chupai");
        Transform outPokerTransform = m_RoleMainUITranform.Find("Poker_Chupai");
        m_OutPokerList.Sort(SortByCardValue);
        m_OutPokerList.Reverse();
        foreach (var outPoker in m_OutPokerList)
        {
            outPoker.m_CartTransform = ((GameObject)GameObject.Instantiate(pokerObject)).transform;
            outPoker.m_CartTransform.SetParent(outPokerTransform, false);
            UpdatePoker(outPoker);
        }
    }

    /// <summary>
    /// 刷新手牌剩余数量小于10张的提示图标
    /// </summary>
    /// <param name="remainPokerNumState">提示图标显示状态true:显示 false:不显示</param>
    public void RefreshRemainPokerPromptUI(bool remainPokerNumState)
    {
        if (m_bRemainPokerNumState == remainPokerNumState ||
           m_RoleMainUITranform == null)
        {
            return;
        }
        m_bRemainPokerNumState = remainPokerNumState;
        m_RoleMainUITranform.Find("Tip_BG/Image_10").gameObject.SetActive(remainPokerNumState);
    }

    /// <summary>
    /// 刷新手牌界面
    /// </summary>
    public virtual void RefreshHavePokerPanel()
    {
        if (m_RoleMainUITranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }

        if (m_OutPokerList.Count != 0)
        {
            DestoryHavePokerOjbect(m_OutPokerList);
            m_OutPokerList.Clear();
        }

        int index = 0, siblingIndex = 0;
        int maxCount = m_HavePokerList.Count > 18 ? (int)System.Math.Floor(m_HavePokerList.Count / 2.0f) : 0;
        m_CardTransformOne = m_RoleMainUITranform.Find("Poker_Shoupai_heng/Point_shoupai_1");  //第一层
        m_CardTransformTwo = m_RoleMainUITranform.Find("Poker_Shoupai_heng/Point_shoupai_2");  //第二层
        m_CardTransformOne.GetComponent<HorizontalLayoutGroup>().enabled = true;
        m_CardTransformTwo.GetComponent<HorizontalLayoutGroup>().enabled = true;
        for (index = 0; index < m_HavePokerList.Count; ++index, ++siblingIndex)
        {
            m_HavePokerList[index].m_CartTransform.name = "paker_" + index;
            m_HavePokerList[index].m_CartTransform.SetParent(index >= maxCount ? m_CardTransformOne : m_CardTransformTwo, false);
            if (index == maxCount)
            {
                siblingIndex = 0;
            }
            m_HavePokerList[index].m_CartTransform.SetSiblingIndex(siblingIndex);
        }

        UpdateCardNum();
    }

    /// <summary>
    /// 刷新角色徽章
    /// </summary>
    /// <param name="badge">徽章类型</param>
    /// <param name="addOrRemove">ture: 添加 false: 删除</param>
    public void RefreshBadgeObject(RoleBadge_Enum badge, bool addOrRemove = true)
    {
        if (m_RoleMainUITranform == null || m_GameBase == null || badge == RoleBadge_Enum.eRB_Max)
        {
            return;
        }

        if (addOrRemove && !m_RoleBadgeDictionary.ContainsKey(badge) && m_GameBase.m_GouJiAssetBundle)
        {
            string[] iconName = { "Icon_xuan_1", "Icon_xuan_0", "Icon_dian_1", "Icon_dian_0",
                                  "Icon_shao_1", "Icon_shao_0", "Icon_men_1", "Icon_men_0" };
            Object huiZhangObject = m_GameBase.m_GouJiAssetBundle.LoadAsset(iconName[(int)badge]);
            if (huiZhangObject)
            {
                Transform huiZhangTransform = ((GameObject)GameObject.Instantiate(huiZhangObject)).transform;
                huiZhangTransform.SetParent(m_RoleMainUITranform.Find("Tip_BG/Icon_huizhang"), false);
                m_RoleBadgeDictionary.Add(badge, huiZhangTransform.gameObject);
            }
        }

        if (m_RoleBadgeDictionary.ContainsKey(badge))
        {
            m_RoleBadgeDictionary[badge].SetActive(addOrRemove);
        }
    }

    /// <summary>
    /// 游戏结束摊牌
    /// </summary>
    /// <param name="roleHavePokerList">角色手牌数据列表</param>
    public void RefreshLayPokerPanel(List<byte> roleHavePokerList)
    {
        if (m_RoleMainUITranform == null || m_GameBase == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }

        if (m_GameBase.m_GouJiAssetBundle == null)
        {
            Debug.Log("游戏资源对象为空!<m_GouJiAssetBundle>");
            return;
        }

        if (roleHavePokerList.Count == 0)
        {
            return;
        }

        roleHavePokerList.Sort(LL_PokerType.SortByIndex);
        roleHavePokerList.Reverse();

        Transform outPokerTransform = m_RoleMainUITranform.Find("Poker_liangpai");
        m_GameBase.DestoryAllChildObject(outPokerTransform);
        Transform ChildPokerTransform = null;
        Object pokerObject = m_GameBase.m_GouJiAssetBundle.LoadAsset("Poker_GJ_liangpai");
        foreach (var outPoker in roleHavePokerList)
        {
            ChildPokerTransform = ((GameObject)GameObject.Instantiate(pokerObject)).transform;
            ChildPokerTransform.SetParent(outPokerTransform, false);
            UpdatePoker(outPoker, ChildPokerTransform);
        }
    }

    /// <summary>
    /// 刷新聊天界面
    /// </summary>
    /// <param name="emotionIndex">聊天内容索引</param>
    public void RefreshEmotionPanel(byte emotionIndex)
    {
        if (m_GameBase != null)
        {
            m_GameBase.PlayerGameAudio(m_nCSit, emotionIndex + 100);
        }

        SetTalkPanelActive(true, emotionIndex + 25000);
    }

    /// <summary>
    /// 设置聊天提示界面激活
    /// </summary>
    /// <param name="activeState">true:界面激活false:界面关闭</param>
    /// <param name="promptTextId">提示文本ID</param>
    private void SetTalkPanelActive(bool activeState,int promptTextId = 0)
    {
        if (m_RoleMainUITranform == null)
        {
            return;
        }

        Transform talkTransfrom = m_RoleMainUITranform.Find("Tip_BG/Tip_talk");
        if (talkTransfrom)
        {
            if(activeState)
            {
                talkTransfrom.Find("Text").GetComponent<Text>().text = CCsvDataManager.Instance.TipsDataMgr.GetTipsText((uint)promptTextId);
                GameMain.WaitForCall(3f, () => talkTransfrom.gameObject.SetActive(false));
            }
            talkTransfrom.gameObject.SetActive(activeState);
        }
    }

    /// <summary>
    /// 创建交换牌挂节点对象
    /// </summary>
    public void CreateSwapPokerSocket()
    {
        if (m_RoleMainUITranform == null || m_GameBase == null)
        {
            return;
        }
        if (m_GameBase.m_GouJiAssetBundle == null)
        {
            return;
        }
        Transform outPokerTransform = m_RoleMainUITranform.Find("Poker_Chupai");
        m_PokerSwapTransform = ((GameObject)GameObject.Instantiate(m_GameBase.m_GouJiAssetBundle.LoadAsset("Poker_GJ_Chupai"))).transform;
        m_PokerSwapTransform.GetComponent<Image>().enabled = false;
    }

    /// <summary>
    /// 播放出牌数提示相关动画
    /// </summary>
    /// <param name="bGoujiState">是否是够级牌: true 是 flse 不是</param>
    public void PlayerOutPokerPromptAnimation(bool bGoujiState)
    {
        string animationName = string.Empty;
        int index = m_OutPokerList.FindIndex(value => { return value.m_nCardValue == 0x4f; });
        if (index != -1)
        {
            animationName = "anime_GJ_hua2";
        }
        else
        {
            index = m_OutPokerList.FindIndex(value => { return value.m_nCardValue == 0x4e; });
            if (index != -1)
            {
                animationName = "anime_GJ_hua1";
            }
            else
            {
                if (bGoujiState)
                {
                    animationName = "anime_GJ_gouji";
                }
            }
        }

        if (!string.IsNullOrEmpty(animationName))
        {
            AddRoleAnimation(animationName);
        }
    }

    /// <summary>
    /// 播放烧牌动画
    /// </summary>
    /// <param name="isPalyer">true(播放)false(停止)</param>
    public void PlayShaoPaiAnimation(bool isPalyer)
    {
        if (isPalyer)
        {
            m_ShaoPaiAnimationObject = GameFunction.PlayUIAnim("anime_GJ_shao", 0f, GetRoleAnimationTransform(), m_GameBase.m_GouJiAssetBundle);
        }
        else
        {
            if (m_ShaoPaiAnimationObject)
            {
                GameObject.Destroy(m_ShaoPaiAnimationObject);
            }
        }
    }

    /// <summary>
    /// 添加行牌动画
    /// </summary>
    /// <param name="animationName">动画名次</param>
    public void AddRoleAnimation(string animationName)
    {
        if (m_RoleAnimationNameList.Count == 0)
        {
            m_bPlayerAnimationState = true;
        }
        m_RoleAnimationNameList.Add(animationName);
    }

    /// <summary>
    /// 更新播放行牌动画
    /// </summary>
    private void UpdateRolePlayAnimation()
    {
        if (m_RoleAnimationNameList.Count == 0 || !m_bPlayerAnimationState)
        {
            if (m_bPlayerAnimationState)
            {
                m_bPlayerAnimationState = false;
            }
            return;
        }

        if (!m_bPlayerAnimationState)
        {
            return;
        }

        m_bPlayerAnimationState = false;
        float time = 1.0f;
        if (m_GameBase != null)
        {
            if (m_GameBase.GameMode == GameTye_Enum.GameType_Record)
            {
                time = GameVideo.GetInstance().m_bPause ? 0.001f : GameVideo.GetInstance().GetStepTime() * 0.5f;
            }
        }
        GameFunction.PlayUIAnim(m_RoleAnimationNameList.First(), time, GetRoleAnimationTransform(), m_GameBase.m_GouJiAssetBundle);
        GameMain.WaitForCall(time, () => { m_RoleAnimationNameList.Remove(m_RoleAnimationNameList.First()); m_bPlayerAnimationState = true; });
    }

    /// <summary>
    /// 删除全部的出牌对象
    /// </summary>
    public void DestoryAllOutPokerObject()
    {
        if (m_RoleMainUITranform == null || m_GameBase == null)
        {
            return;
        }

        Transform outPokerTransform = m_RoleMainUITranform.Find("Poker_Chupai");

        m_GameBase.DestoryAllChildObject(outPokerTransform);
    }

    /// <summary>
    /// 删除全部的手牌对象
    /// </summary>
    public void DestoryAllHavePokerObject()
    {
        if (m_RoleMainUITranform == null || m_GameBase == null)
        {
            return;
        }

        m_HavePokerList.Clear();

        m_CardTransformOne = m_RoleMainUITranform.Find("Poker_Shoupai_heng/Point_shoupai_1");  //第一层
        m_CardTransformTwo = m_RoleMainUITranform.Find("Poker_Shoupai_heng/Point_shoupai_2");  //第二层

        m_GameBase.DestoryAllChildObject(m_CardTransformOne);
        m_GameBase.DestoryAllChildObject(m_CardTransformTwo);
    }

    /// <summary>
    /// 删除全部的摊牌对象
    /// </summary>
    public void DestoryAllLayPokerObject()
    {
        if (m_RoleMainUITranform == null || m_GameBase == null)
        {
            return;
        }
        m_GameBase.DestoryAllChildObject(m_RoleMainUITranform.Find("Poker_liangpai"));
    }

    /// <summary>
    /// 添加交换牌对象
    /// </summary>
    /// <param name="pokerDataList">交换牌值</param>
    public virtual void AddSwapPokerObject(List<byte> pokerDataList)
    {
        if (m_RoleMainUITranform == null || pokerDataList.Count == 0 ||
            m_GameBase.m_GouJiAssetBundle == null)
        {
            return;
        }
        Object pokerObject = null;
        Transform swapPokerTransform = null;
        Transform outPokerTransform = m_RoleMainUITranform.Find("Poker_Chupai");
        foreach (var value in pokerDataList)
        {
            pokerObject = m_GameBase.m_GouJiAssetBundle.LoadAsset("Poker_GJ_Chupai");
            swapPokerTransform = ((GameObject)GameObject.Instantiate(pokerObject)).transform;
            swapPokerTransform.SetParent(outPokerTransform, false);
            UpdatePoker(value, swapPokerTransform);
        }
        DestoryHavePokerOjbect(pokerDataList);
    }

    /// <summary>
    /// 添加手牌对象
    /// </summary>
    /// <param name="pokerDataList">牌值</param>
    /// <param name="isMask">是否打开遮罩 true（打开）false(关闭)</param>
    /// <param name="isGongMask">是否是贡牌 true（是）false(不是)</param>
    public virtual void AddHavePokerObject(List<byte> pokerDataList, bool isMask = false, bool isGongMask = false)
    {

    }

    /// <summary>
    /// 删除手牌对象
    /// </summary>
    /// <param name="pokerDataList">牌值</param>
    public virtual void DestoryHavePokerOjbect(List<byte> pokerDataList)
    {
        if (pokerDataList.Count == 0)
        {
            return;
        }

        CardData CurCardData = null;
        foreach (var value in pokerDataList)
        {
            CurCardData = m_HavePokerList.Find(haveCardData => haveCardData.m_nCardValue == value);
            if (CurCardData != null)
            {
                CurCardData.m_CartTransform.gameObject.SetActive(false);
                GameObject.Destroy(CurCardData.m_CartTransform.gameObject);
                m_HavePokerList.Remove(CurCardData);
            }
        }
    }

    /// <summary>
    /// 删除手牌对象
    /// </summary>
    /// <param name="pokerDataList">牌值</param>
    public virtual void DestoryHavePokerOjbect(List<CardData> pokerDataList)
    {
        CardData CurCardData = null;
        foreach (var cardData in pokerDataList)
        {
            CurCardData = m_HavePokerList.Find(haveCardData => haveCardData.m_nCardValue == cardData.m_nCardValue);
            if (CurCardData != null)
            {
                CurCardData.m_CartTransform.gameObject.SetActive(false);
                GameObject.Destroy(CurCardData.m_CartTransform.gameObject);
                m_HavePokerList.Remove(CurCardData);
            }
        }
    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="time">发牌阶段时间</param>
    /// <returns></returns>
    public virtual IEnumerator DealPoker(float time)    {
        yield return null;
    }

    /// <summary>
    /// 更新单个牌面
    /// </summary>
    /// <param name="cardData">牌对象数据</param>
    /// <param name="postfix">牌值资源名称</param>
    public void UpdatePoker(CardData cardData, string postfix = "_Big")
    {
        string cardName = cardData.m_nCardValue == RoomInfo.NoSit ? "puke_back" + postfix : GameCommon.GetPokerMat(cardData.m_nCardValue) + postfix;
        cardData.m_CartTransform.GetComponent<Image>().sprite = m_GameBase.m_GouJiCommonAssetBundle.LoadAsset<Sprite>(cardName);
    }

    /// <summary>
    /// 更新单个牌面
    /// </summary>
    /// <param name="cardValue">牌数据</param>
    /// <param name="pokerTransform">牌组件对象</param>
    /// <param name="postfix">牌值资源名称</param>
    public void UpdatePoker(byte cardValue, Transform pokerTransform, string postfix = "_Big")
    {
        string cardName = cardValue == RoomInfo.NoSit ? "puke_back" + postfix : GameCommon.GetPokerMat(cardValue) + postfix;
        if (pokerTransform)
        {
            pokerTransform.GetComponent<Image>().sprite = m_GameBase.m_GouJiCommonAssetBundle.LoadAsset<Sprite>(cardName);
        }
    }

    /// <summary>
    /// 玩家游戏逻辑推进
    /// </summary>
    public virtual void OnTick()    {
        UpdateRolePlayAnimation();
    }

    /// <summary>
    /// 牌值从大到小排序
    /// </summary>
    public int SortByCardValue(CardData cardDataOne, CardData cardDataTwo)
    {
        return LL_PokerType.SortByIndex(cardDataOne.m_nCardValue, cardDataTwo.m_nCardValue);
    }

    /// <summary>
    /// 牌值从小到大排序
    /// </summary>
    public int SortByCardValue(IGrouping<int, CardData> cardDataOne, IGrouping<int, CardData> cardDataTwo)
    {
        return LL_PokerType.SortByValue(cardDataOne.First().m_nCardValue, cardDataTwo.First().m_nCardValue);
    }

    /// <summary>
    /// 更新牌数
    /// </summary>
    public void UpdateCardNum()
    {
        if (m_GameBase == null)
        {
            return;
        }
        if (m_GameBase.Bystander)
        {
            return;
        }
        List<CardData> groupList = null;
        IEnumerable<IGrouping<int, CardData>> CardDataGroup = m_HavePokerList.GroupBy(value => GameCommon.GetCardValue(value.m_nCardValue));
        foreach (var dataGroup in CardDataGroup)
        {
            groupList = dataGroup.ToList<CardData>();
            groupList[0].m_CartTransform.Find("Text_num").GetComponent<Text>().text = groupList.Count.ToString();
        }
    }

    /// <summary>
    /// 打开倒计时界面
    /// </summary>
    /// <param name="show">显示状态</param>
    /// <param name="time">时间</param>
    /// <param name="fun">结束或关闭时回调函数</param>
    /// <param name="pause">倒计时暂停标记</param>
    public void ShowCountdownPanel(bool show, float time = 0f, CustomCountdownImgMgr.CallBackFunc fun = null, bool pause = false)
    {
        if (m_RoleMainUITranform == null)
        {
            Debug.Log("角色面板对象为空!<m_RoleMainUITranform>");
            return;
        }
        Transform CountDownTransform = m_RoleMainUITranform.Find("Tip_BG/Time_Countdown");
        Image image = CountDownTransform.GetComponent<Image>();        if (show)        {            if (time > 0f || fun == null)            {
                CountDownTransform.gameObject.SetActive(true);
                Text text = CountDownTransform.GetComponentInChildren<Text>();
                if (!pause)
                {
                    m_GameBase.CCIMgr.AddTimeImage(image, time, 1f, fun, text, false);                }
                else
                {                    text.text = time.ToString();                    image.fillAmount = 1f;
                }            }            else
            {
                CountDownTransform.gameObject.SetActive(false);
                fun(0, false, image, "");
            }        }        else
        {            m_GameBase.CCIMgr.RemoveTimeImage(image);            CountDownTransform.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 显示革命/要头图标
    /// </summary
    /// <param name="activeState">显示状态</param>
    /// <param name="yaoTouIconType">革命/要头图标/无头图标</param>
    public void ShowGeMingYaotouInfoUI(bool activeState, YaoTouType_Enum yaoTouIconType = YaoTouType_Enum.eYTT_GeMing)
    {
        if (m_RoleMainUITranform == null)
        {
            return;
        }
        Transform imageTransform = m_RoleMainUITranform.Find("Tip_BG/Image_geming");
        if (activeState && m_GameBase.m_GouJiAssetBundle)
        {
            string imageName = string.Empty;
            switch (yaoTouIconType)
            {
                case YaoTouType_Enum.eYTT_WuTou:
                    imageName = "GJ_icon_wutou";
                    break;
                case YaoTouType_Enum.eYTT_YaoTou:
                    imageName = "GJ_icon_yaotou";
                    break;
                case YaoTouType_Enum.eYTT_GeMing:
                    imageName = "GJ_icon_geming";
                    break;

            }
            m_eYaoTouType = yaoTouIconType;
            imageTransform.GetComponent<Image>().sprite = m_GameBase.m_GouJiAssetBundle.LoadAsset<Sprite>(imageName);
        }
        imageTransform.gameObject.SetActive(activeState);
    }

    /// <summary>
    /// 更新行牌提示信息
    /// </summary>
    /// <param name="promptTextData">提示内容</param>
    public void UpdatePromptText(string promptTextData)
    {
        if (m_RoleMainUITranform == null || (m_pPromptTextEnumerator != null && string.IsNullOrEmpty(promptTextData)))
        {
            return;
        }

        Text PromptText = m_RoleMainUITranform.Find("Tip_BG/Image_zhuangtai/Text").GetComponent<Text>();
        PromptText.text = promptTextData;
    }

    /// <summary>
    /// 更新行牌提示信息
    /// </summary>
    /// <param name="promptTextData">提示内容</param>
    /// <param name="time">延迟消失时间</param>
    /// <returns></returns>
    public IEnumerator UpdatePromptText(string promptTextData, float time)
    {
        UpdatePromptText(promptTextData);
        yield return new WaitForSecondsRealtime(time);
        m_pPromptTextEnumerator = null;
        UpdatePromptText("");
    }

    /// <summary>
    /// 服务器提问答复事件
    /// </summary>
    /// <param name="doing">类型</param>
    /// <param name="baseFunctionState">true(执行父类方法)false(不执行父类方法)</param>
    /// <param name="param">参数</param>
    /// <param name="time">文本提示时间</param>
    public virtual void AnswerDoingEvent(RoleDoing_Enum doing, bool baseFunctionState = true, byte param = 230, float time = 2.0f)
    {
        bool bPromptType = false;
        string textData = string.Empty;
        switch (doing)
        {
            case RoleDoing_Enum.RoleDoing_GeMing:
                textData = "革命";
                ShowGeMingYaotouInfoUI(true);
                break;
            case RoleDoing_Enum.DRoleDoing_YaoTou:
                textData = "要头";
                ShowGeMingYaotouInfoUI(true,YaoTouType_Enum.eYTT_YaoTou);
                break;
            case RoleDoing_Enum.RoleDoing_XuanDian:
                {
                    XuanDianType_Enum Type = param == 230 ? XuanDianType_Enum.eXDT_Yes : (XuanDianType_Enum)param;
                    switch (Type)
                    {
                        case XuanDianType_Enum.eXDT_No:
                            textData = "不宣点";
                            break;
                        case XuanDianType_Enum.eXDT_Yes:
                        case XuanDianType_Enum.eXDT_Natural_Yes:
                            textData = Type == XuanDianType_Enum.eXDT_Yes ? "宣点" : "自然点宣点";
                            RefreshBadgeObject(RoleBadge_Enum.eRB_XuanDain);
                            break;
                        case XuanDianType_Enum.eXDT_Natural_No:
                            textData = "自然点不宣点";
                            break;
                    }
                }
                break;
            case RoleDoing_Enum.RoleDoing_RangPai:
                {
                    textData = "让牌";
                    bPromptType = true;
                }
                break;
        }

        if (bPromptType)
        {
            UpdatePromptText(textData);
        }
        else
        {
            if (m_pPromptTextEnumerator != null)
            {
                GameMain.ST(m_pPromptTextEnumerator);
            }
            m_pPromptTextEnumerator = UpdatePromptText(textData, time);
            GameMain.SC(m_pPromptTextEnumerator);
        }
    }

    /// <summary>
    /// 重置出牌状态
    /// </summary>
    /// <param name="state">true：全部重置,false :重置除大小王和2的牌</param>
    public virtual void ResetOutPokerPanel(bool state = true)
    {

    }

    /// <summary>
    /// 获得动画挂节点对象
    /// </summary>
    /// <returns></returns>
    public Transform GetRoleAnimationTransform()
    {
        return m_AnimationTransform;
    }

    /// <summary>
    /// 获得头像图标
    /// </summary>
    /// <returns></returns>
    public Sprite GetHeadImg()
    {
        return GameMain.hall_.GetIcon(m_sUrl, m_nUseId, m_nFaceId);
    }
    /// <summary>
    /// 获得玩家名称
    /// </summary>
    /// <returns></returns>
    public string GetRoleName()
    {
        return m_sRoleName;
    }

    /// <summary>
    /// 获得手牌数量
    /// </summary>
    /// <returns></returns>
    public virtual int GetHavePokerNum()
    {
        return m_HavePokerList.Count;
    }

    /// <summary>
    /// 刷新拆牌提示
    /// </summary>
    /// <param name="activeState">true:显示 false:不显示</param>
    public virtual void RefreshDiscardPokerPromptUI(bool activeState)
    {

    }

    /// <summary>
    /// 显示玩家行为提示界面(用于旁观和录像显示用)
    /// </summary>
    public virtual void ShowRoleDoingPanel(bool activeState)
    {

    }

    /// <summary>
    /// 显示玩家行为提示界面
    /// </summary>
    /// <param name="doing"></param> 
    /// <param name="activeState">界面显示状态</param>
    /// <param name="askSign">提问标识</param>
    /// <param name="time">倒计时-时间</param>
    public virtual void  ShowRoleDoingPanel(RoleDoing_Enum doing, bool activeState, uint askSign = 0, float time = 0.0f, bool guoActive = false)
    {

    }

    /// <summary>
    /// 获得房间角色信息
    /// </summary>
    /// <param name="recordPalyerInfo">角色信息</param>
    public void GetRecordPlyerInfo(ref AppointmentRecordPlayer  recordPalyerInfo)
    {
        recordPalyerInfo.playerid = m_nUseId;
        recordPalyerInfo.faceid = m_nFaceId;
        recordPalyerInfo.url = m_sUrl;
        recordPalyerInfo.coin = m_nTotalCoin;
        recordPalyerInfo.playerName = GetRoleName();
        recordPalyerInfo.master = m_fMasterScore;
        recordPalyerInfo.sex = m_nSex;
        recordPalyerInfo.ready = m_nReady;
    }
}