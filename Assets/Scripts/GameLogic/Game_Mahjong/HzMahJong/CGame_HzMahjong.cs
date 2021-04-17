//#define HAVE_LOBBY
#define MATCH_ROOM
using DG.Tweening;
using System.Collections;using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using USocket.Messages;
using UnityEngine.EventSystems;
using XLua;
using System;

[Hotfix]
public class CGame_HzMahjong : CGame_Mahjong
{
    int m_nAddPrice = -1;

    public int AddPrice
    {
        get
        {
            return m_nAddPrice;
        }

        set
        {
            m_nAddPrice = value;
        }
    }
    
	byte m_nDianPaoState;

    Transform m_ChatTransform;

    IEnumerator m_ZhuaNiaoDEnumerator;

    Dictionary<Transform,Transform> m_ZhuaNiaoTransformDictionary= new Dictionary<Transform, Transform>();

    public CGame_HzMahjong(GameTye_Enum gameType, GameKind_Enum gameKind) : base(gameType, gameKind)
    {
        m_ChatTransform = null;
        m_nDianPaoState = 0;
        m_ZhuaNiaoDEnumerator = null;
        m_ZhuaNiaoTransformDictionary.Clear();
    }

    public override void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_CHOOSElEVEL, HandleChooseLevelMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_ENTERROOM, HandleEnterRoom);                    //进入房间成功
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_ROOMSTATE, HandRoomStateMsg);                   //同步房间状态到客户端
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_OTHERENTER, HandOtherEnterMsg);                 //告诉其他人 此人加入房间
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_GETPOKERASKDEAL, HandGetPokerAskdealMsg);       //发一张牌给该玩家并让他出牌
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_DEALMJBEGIN, HandleSendTile);                   //开始的时候每人13张牌
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_UPDATEDEALPOKER, HandleBackOtherDiscard);       //同步出牌出去
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_BACKLEAVE, HandleBackLevaeMsg);                 //回复给离开的人
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_ENTERTRUSTSTATE, HandleEnterTrustStateMsg);     //通知客户端进入托管状态
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_ANSWERDOING, HandleAnswer);                     //回复此人
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_UPDATEDOING, HandleOtherAnswer);                //同步出去
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_PLAYERHUPOKER, HandlePlayerWin);                //玩家胡牌
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_PUBLISHRESULT, HandleResult);                   //同步游戏结果
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_MIDDLEENTERROOM, HandleMiddleEnterRoom);        //玩家中途加入
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_AFTERONLOOKERENTER, HandleBystanderEnter);      //围观的玩家进入房间
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERLEAVEROOMSER, HandlePlayerLeaveRoomMsg);       //回复
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION, HandEmotionNetMsg);                            //发表情
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_SM_AFTERQIANGGANGHUCUTCOIN,HandleAfterQiangGangHuCutCoinMsg);//WaitQiangGangHu状态下 别的玩家不胡的 接下来扣钱的函数
    }

    public bool HandleChooseLevelMsg(uint _msgType, UMessage _ms)
    {
        base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_CHOOSElEVEL, _ms);
        return true;
    }

    public bool HandRoomStateMsg(uint _msgType, UMessage _ms)
    {
        base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ROOMSTATE, _ms);
        return true;
    }

    public bool HandOtherEnterMsg(uint _msgType, UMessage _ms)
    {
        base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_OTHERENTER, _ms);
        return true;
    }

    public bool HandleBackLevaeMsg(uint _msgType, UMessage _ms)
    {
        base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_BACKLEAVE, _ms);
        return true;
    }

    public bool HandleEnterTrustStateMsg(uint _msgType, UMessage _ms)
    {
        base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_ENTERTRUSTSTATE, _ms);
        return true;
    }

    public bool HandlePlayerLeaveRoomMsg(uint _msgType, UMessage _ms)
    {
        base.HandleGameNetMsg(_msgType, _ms);
        return true;
    }

    public bool HandleAfterQiangGangHuCutCoinMsg(uint _msgType, UMessage _ms)
    {
        base.HandleGameNetMsg((uint)GameCity.EMSG_ENUM.CCMsg_MAHJONG_SM_AFTERQIANGGANGHUCUTCOIN, _ms);
        return true;
    }

    public bool HandGetPokerAskdealMsg(uint _msgType, UMessage _ms)
    {
        byte sit = _ms.ReadByte();
        sbyte tile = _ms.ReadSByte();
        byte doing = _ms.ReadByte();
        byte num = _ms.ReadByte();

        List<byte> list = new List<byte>();
        for (byte i = 0; i < num; i++)
            list.Add(_ms.ReadByte());

        float time = _ms.ReadSingle();
        uint sign = _ms.ReadUInt();
        byte tileValue = (byte)tile;
        if (tile < 0)
        {
            tileValue = byte.MaxValue;
        }
        HandleAskDeal(sit, tileValue, doing, sign, time, list);
        return true;
    }

    //初始化胡牌倍数
    protected override void InitChangeCoinTypeData(UMessage _ms, bool midEnt = false)
    {
        m_ProInfo.m_iMaxPro = _ms.ReaduShort(); //最大倍数

        m_nDianPaoState = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_GangShangKaiHua] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_QiangGangHu] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_Bao3Jia] = _ms.ReadByte();
        m_ProInfo.m_CoinTypeProDict[MjChangeCoinType_Enum.MjChangeCoinType_Zimo] = _ms.ReadByte();

        if(midEnt)
        {
            UpdateHongZhongCard(_ms.ReadByte());
        }
    }

    protected override bool HandleSendTile(uint _msgType, UMessage _ms)
    {
        base.HandleSendTile(_msgType, _ms);

        //红中牌
        UpdateHongZhongCard(_ms.ReadByte());
        return true;
    }

    public override void InitPlayers()
    {
        Mahjong_Role role;
        role = new HzMahjong_RoleLocal(this, 1);
        role.Init();        m_PlayerList.Add(role);

        for (byte i = 2; i < 5; i++)
        {
            role = new Mahjong_RoleOther(this, i);
            role.Init();            m_PlayerList.Add(role);
        }
    }

    public override void ProcessTick()
    {
        base.ProcessTick();
        UpdateZhuaNiaoUILocalPosition();
    }

    public override void LoadMainUI(Transform root)
    {
        m_nTotalTilesNum = 112;
        m_nMaxTileValue = 0x37;
        m_nAddPrice = -1;

        GameObject obj = (GameObject)MahjongAssetBundle.LoadAsset("HZ_Mahjong_Game");
        MainUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        MainUITfm.SetParent(root, false);
        MainUITfm.gameObject.SetActive(false);

        Transform tfm = MainUITfm.Find("Pop-up");
        m_TrustButton = tfm.Find("button_tuoguan/Button_guanbi").GetComponent<Button>();
        m_TrustButton.onClick.AddListener(() => OnClickTrustBtn(GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_CM_CANCLETRUSTSTATE));

        ChooseKongTfm = MainUITfm.Find("Pop-up/choose/choose_gang");
        Button[] buttons = ChooseKongTfm.GetComponentsInChildren<Button>();
        byte index = 0;
        foreach (Button btn in buttons)
        {
            byte temp = index;
            btn.onClick.AddListener(() => OnClickGameBtn(MjOtherDoing_Enum.MjOtherDoing_Gang, temp, GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_CM_ANSWERDOING));
            index++;
        }

        tfm = tfm.Find("button_option");
        string[] btnName = new string[] { "Button_guo", "Button_peng", "Button_gang", "Button_he", "Button_chi", "Button_ting", "Button_tingfei" };
        index = 0;
        foreach (MjOtherDoing_Enum item in System.Enum.GetValues(typeof(MjOtherDoing_Enum)))
        {
            if (index >= btnName.Length)
                break;

            Transform child = tfm.Find(btnName[index]);
            index++;

            if (child == null)
                continue;
            Button btn = child.GetComponent<Button>();
            if (btn == null)
                continue;

            m_GameButton[item] = btn;
            btn.onClick.AddListener(() => OnClickGameBtn(item, RoomInfo.NoSit, GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_CM_ANSWERDOING));
        }

        m_ChatTransform = ((GameObject)GameMain.instantiate(MahjongAssetBundle.LoadAsset("majiang_Chat"))).transform;
        m_ChatTransform.SetParent(root, false);
        m_ChatTransform.gameObject.SetActive(false);

        index = 1;
        Button[] chatButtons = m_ChatTransform.Find("Chat_Viewport/Chat_Content").GetComponentsInChildren<Button>();
        foreach (var chatButton in chatButtons)
        {
            int tempIndex = index;
            chatButton.onClick.AddListener(() => OnClickChatEvent(tempIndex));
            index++;
        }

        MainUITfm.Find("Middle/Imagechat_BG/Button_chat").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (m_ChatTransform) { CustomAudioDataManager.GetInstance().PlayAudio(3006); m_ChatTransform.gameObject.SetActive(true); }
        });

        UpdateHongZhongCard(0);
        
        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameType, "Mahjong_AudioCsv");
    }

    public override void CreateWallTiles()
    {
        foreach (Mahjong_Role role in m_PlayerList)
            role.CreateWallTiles(18);
    }

    public override void OnEnterState(MahjongRoomState_Enum state, UMessage _ms, byte mode, float timeLeft)
    {
        base.OnEnterState(state, _ms, mode, timeLeft);

        switch (state)
        {
            case MahjongRoomState_Enum.MjRoomState_BuHua:
                {
                    byte birdSit = _ms.ReadByte();
                    byte birdNum = _ms.ReadByte();
                    Dictionary<int, byte> zhuanNiaoDictionary = new Dictionary<int, byte>();
                    for(int index = 0; index < birdNum;++index)
                    {
                        byte pokerValue = _ms.ReadByte();
                        byte pokerState = _ms.ReadByte();
                        zhuanNiaoDictionary.Add(index*100 +pokerValue, pokerState);
                    }
                    m_ZhuaNiaoDEnumerator = ShowZhuaNiaoDemonstrate(birdSit, zhuanNiaoDictionary, 5f);
                    GameMain.SC(m_ZhuaNiaoDEnumerator);
                }
                break;
        }
    }

    override protected void OnQuitState(MahjongRoomState_Enum state)
    {
        base.OnQuitState(state);
        if(state == MahjongRoomState_Enum.MjRoomState_BuHua)
        {
            DestoryZhuaNiaoGameObject();
        }
    }

    public override void OnEnd()
    {
        AddPrice = -1;
        m_nTotalTilesNum = 112;
        if(MainUITfm)
        {
            Transform zhuaNiaoTransform = MainUITfm.Find("Pop-up/zhuaNiao/Mask/ImageBG");
            for (int index = 0; index < zhuaNiaoTransform.childCount; ++index)
            {
                zhuaNiaoTransform.GetChild(index).gameObject.SetActive(false);
            }
            MainUITfm.Find("Pop-up/zhuaNiao").gameObject.SetActive(false);
        }

        DestoryZhuaNiaoGameObject();

        base.OnEnd();
    }

    public override string GetBeiShuStr()
    {
        return "倍";
    }

    protected override string GetMahJiongGameRule()
    {
        return "红中麻将 ";
    }

    protected override void OnLeaveLookRoom(GameCity.EMSG_ENUM GameCityMsg)
    {
        base.OnLeaveLookRoom(GameCity.EMSG_ENUM.CCMsg_HONGMAHJONG_CM_LEAVEONLOOKERROOM);
    }

    //更新红中牌
    void UpdateHongZhongCard(byte hongZhongCard)
    {
        m_AtomicCard = hongZhongCard;
        if (m_AtomicCard == 0)
        {
            m_AtomicCard = 0xff;
        }

        if (MainUITfm != null && MahjongAssetBundle != null)
        {
            Transform ImageTransform = MainUITfm.Find("Top/cai/majiang_1/ImageIcon");
            ImageTransform.GetComponent<Image>().sprite = MahjongAssetBundle.LoadAsset<Sprite>("mahjong_" + m_AtomicCard.ToString("X2"));
        }

        MainUITfm.Find("Top/cai").gameObject.SetActive(hongZhongCard != 0);
    }

    public override void ResetLeftTile()
    {
        m_LeftNumText.text = "";
        m_LeftNumText.transform.parent.gameObject.SetActive(false);

        m_dictTileValueNum.Clear();
        Transform tfm;
        foreach (byte value in JudgeTile.MahjongTiles)
        {
            if (value > m_nMaxTileValue)
            {
                break;
            }

            m_dictTileValueNum[value] = (byte)(value < 0x40 ? 4 : 1);
            tfm = LeftTileTfm.Find("Mask/" + JudgeTile.GetTileSuit(value));
            tfm.gameObject.SetActive(true);
            tfm = tfm.Find("majiang_" + JudgeTile.GetTileValue(value));
            tfm.Find("ImageIcon").GetComponent<Image>().color = Color.white;
            tfm.gameObject.SetActive(value > 0x30 && value == 0x35 || value <= 0x30);
            //tfm.FindChild("Text").GetComponent<Text>().text = m_dictTileValueNum[value].ToString();
        }
    }

    /// <summary>
    /// 是否开启点炮规则
    /// </summary>
    /// <returns>true(开启点炮规则)false(禁止点炮规则)</returns>
    public bool isDianPao()
    {
        return m_nDianPaoState == 1;
    }


    //获得查听胡牌类型显示界面路径
    protected override string GetTingUIPath()
    {
        return base.GetTingUIPath() + "/Viewport/Content";
    }

    /// <summary>
    /// 显示查听界面
    /// </summary>
    public override void ShowTingUI(bool bShow, Dictionary<byte, ushort> tileHu = null)
    {
        if (tileHu != null && TingTfm)
        {
            RectTransform ImageBgRectTransform = TingTfm.Find("Mask/ImageBG").GetComponent<RectTransform>();
            if (ImageBgRectTransform && tileHu != null)
            {
                ImageBgRectTransform.sizeDelta = new Vector2((tileHu.Count > 10 ? 10 : tileHu.Count) * 80 + 150, tileHu.Count > 10 ? 275 : 155);
            }
        }
        base.ShowTingUI(bShow, tileHu);
    }

    //断线重连同步数据
    protected override MjTileMiddleEnterData_Enum MiddleEnterRoomRoleLocal(UMessage _ms, ref List<byte> listPong, ref List<byte> listKong, ref List<byte> listKongSelf,
                                                                          ref List<byte> listDiscard, ref List<byte> listHua, ref List<byte> listChi, ref List<byte> listKong2)
    {
        //碰牌
        byte index = 0;
        listPong.Clear();
        byte listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listPong.Add(_ms.ReadByte());//喂吃碰的玩家座位号
            listPong.Add(_ms.ReadByte());//碰的牌
        }

        //明杠
        index = 0;
        listKong.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listKong.Add(_ms.ReadByte());
        }

        //暗杠
        index = 0;
        listKongSelf.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listKongSelf.Add(_ms.ReadByte());
        }

        //补杠
        index = 0;
        listKong2.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listKong2.Add(_ms.ReadByte());//喂吃碰的玩家座位号
            listKong2.Add(_ms.ReadByte());//杠的牌
        }

        //打出去的牌
        index = 0;
        listDiscard.Clear();
        listDataNum = _ms.ReadByte();
        for (; index < listDataNum; ++index)
        {
            listDiscard.Add(_ms.ReadByte());
        }
        return MjTileMiddleEnterData_Enum.MiddleEnterData_Hz;
    }

    /// <summary>
    /// 更新约据房间信息
    /// </summary>
    /// <param name="Data"></param>
    protected override void UpdateAppointmentData(AppointmentData Data, UMessage _ms)
    {
        HzMahjongAppointmentData mahjongData = (HzMahjongAppointmentData)Data;
        mahjongData.wanFa = (ushort)_ms.ReadShort();
        mahjongData.birdNum = _ms.ReadByte();
    }


    public override bool OnVideoStep(List<VideoAction> actionList, int curStep, bool reverse)
    {
        VideoAction action = actionList[curStep];
        List<int> list = action.list;
        int j = 0;
        bool res = true;
        if (!reverse)
        {
            switch (action.vai)
            {
                case VideoActionInfo_Enum.VideoActionInfo_207:
                    {
                        byte sit = (byte)list[j++];
                        sbyte tile = (sbyte)list[j++];
                        byte doing = (byte)list[j++];
                        byte lastDealSit = (byte)list[j++];

                        if(tile < 0)
                        {
                            return false;
                        }

                        HandleAskDeal(sit, (byte)tile, doing, 0, 15f, null, GameVideo.GetInstance().m_bPause);

                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_213:
                    {
                        byte birdSit = (byte)list[j++];
                        byte birdNum = (byte)list[j++];
                        Dictionary<int, byte> zhuanNiaoDictionary = new Dictionary<int, byte>();
                        for (byte pokerCount = 0; pokerCount < birdNum; ++pokerCount)
                        {
                            byte pokerValue = (byte)list[j++]; 
                            byte pokerState = (byte)list[j++];
                            zhuanNiaoDictionary.Add(pokerCount * 100 + pokerValue, pokerState);
                        }
                        GameMain.ST(m_ZhuaNiaoDEnumerator);
                        DestoryZhuaNiaoGameObject();
                        m_ZhuaNiaoDEnumerator = ShowZhuaNiaoDemonstrate(birdSit, zhuanNiaoDictionary, GameVideo.GetInstance().m_bPause ? 0.1f: GameVideo.GetInstance().GetStepTime());
                        GameMain.SC(m_ZhuaNiaoDEnumerator);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_202:
                    {
                        //原子
                        byte atomicCard = (byte)list[j++];
                        UpdateHongZhongCard(atomicCard);

                        byte dealSit = (byte)list[j++];
                        int num = list[j++];
                        for (int i = 0; i < num; i++)
                        {
                            byte sit = (byte)list[j++];
                            byte clientSit = m_dictSitPlayer[sit];
                            byte tileNum = (byte)list[j++];
                            List<byte> tileList = new List<byte>();
                            for (int k = 0; k < tileNum; k++)
                            {
                                byte tile = (byte)list[j++];
                                tileList.Add(tile);
                            }
                            m_PlayerList[clientSit].OnTurn = (sit == dealSit);
                            m_PlayerList[clientSit].OnMidEnt(false, 0, tileList, -1);
                        }

                        SyncWallTiles();

                    }
                    break;
                default:
                    res = false;
                    break;
            }
        }
        else
        {
            switch (action.vai)
            {
                case VideoActionInfo_Enum.VideoActionInfo_207:
                    {
                        byte sit = (byte)list[j++];
                        sbyte tile = (sbyte)list[j++];
                        byte doing = (byte)list[j++];
                        byte lastDealSit = (byte)list[j++];
        
                        if (tile < 0)
                        {
                            return false;
                        }
                        HandleAskDeal(sit, (byte)tile, 0, 0, -15f);

                        if (lastDealSit >= 0)
                            OnChangeTurn(m_dictSitPlayer[lastDealSit]);
                    }
                    break;

                case VideoActionInfo_Enum.VideoActionInfo_213:
                    {
                        byte birdSit = (byte)list[j++];
                        byte birdNum = (byte)list[j++];
                        GameObject DiscardTileGameObject = null;
                        for (byte pokerCount = 0; pokerCount < birdNum; ++pokerCount)
                        {
                            byte pokerValue = (byte)list[j++];
                            byte pokerState = (byte)list[j++];
                            DiscardTileGameObject = FindLastDiscardTile(birdSit, pokerValue);
                            if(DiscardTileGameObject)
                            {
                                GameObject.Destroy(DiscardTileGameObject);
                            }
                            m_PlayerList[m_dictSitPlayer[birdSit]].GetDiscardTransform(reverse);
                        }
                        OnTileNumChanged(-birdNum);
                        HideWallTile(birdNum, reverse);
                    }
                    break;
                case VideoActionInfo_Enum.VideoActionInfo_202:
                    {
                        OnVideoStep(actionList, curStep - 1, false);
                    }
                    break;
                default:
                    res = false;
                    break;
            }
        }

        if (res)
        {
            DebugLog.Log("HzMahjong OnVideoStep:" + action.vai + " rev:" + reverse);
            return true;
        }

        return base.OnVideoStep(actionList, curStep, reverse);
    }

    protected void OnClickChatEvent(int index)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(3006);
        if (m_ChatTransform)
        {
            m_ChatTransform.gameObject.SetActive(false);
        }

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_EMOTION);        msg.Add(GameMain.hall_.GetPlayerId());        msg.Add((byte)index);        HallMain.SendMsgToRoomSer(msg);    }

    /// <summary>
    /// 删除抓鸟对象
    /// </summary>
    void DestoryZhuaNiaoGameObject()
    {
        if (m_ZhuaNiaoTransformDictionary.Count > 0)
        {
            foreach (KeyValuePair<Transform, Transform> zhuaNiaoValuePair in m_ZhuaNiaoTransformDictionary)
            {
                zhuaNiaoValuePair.Value.gameObject.SetActive(false);
                GameObject.Destroy(zhuaNiaoValuePair.Value.gameObject);
            }
            m_ZhuaNiaoTransformDictionary.Clear();
        }
    }

    /// <summary>
    /// 更新抓鸟界面提示位置
    /// </summary>
    private void UpdateZhuaNiaoUILocalPosition()
    {
        if (m_ZhuaNiaoTransformDictionary.Count > 0 &&
            (GameMode == GameTye_Enum.GameType_Record || GameState == MahjongRoomState_Enum.MjRoomState_BuHua))
        {
            foreach (KeyValuePair<Transform, Transform> zhuaNiaoValuePair in m_ZhuaNiaoTransformDictionary)
            {
                if (!zhuaNiaoValuePair.Value.gameObject.activeSelf)
                {
                    continue;
                }
                Vector3 uiPos = GameFunction.WorldToLocalPointInRectangle(zhuaNiaoValuePair.Key.Find("Effectpoint").position,
                                                                          m_PlayerList[0].GetCamera(), GameCanvas, GameCanvas.worldCamera);
                zhuaNiaoValuePair.Value.localPosition = uiPos;
            }
        }
    }

    /// <summary>
    /// 抓鸟效果展示
    /// </summary>
    /// <param name="birdSit">抓鸟玩家座位号</param>
    /// <param name="zhuaNiaoDictionary">抓鸟数据</param>
    /// <param name="time">抓鸟展示时间</param>
    /// <returns></returns>
    IEnumerator ShowZhuaNiaoDemonstrate(byte birdSit,Dictionary<int,byte> zhuaNiaoDictionary,float time)
    {
        if( GameMode != GameTye_Enum.GameType_Record && GameState != MahjongRoomState_Enum.MjRoomState_BuHua)
        {
            yield break;
        }

        PlayEffect(true, "Anime_HZ_zhuaniao", time * 0.2f, m_PlayerList[m_dictSitPlayer[birdSit]].Sex * 1000 + 510);
        ((HzMahjong_RoleLocal)m_PlayerList[0]).LoadZhaNiaoPoker();
        yield return new WaitForSecondsRealtime(time * 0.2f);
        Vector3 Postion = m_PlayerList[m_nDealIndex / 100].GetDealWallTilePostion(m_nDealIndex);
        byte startIndex = 0;
        switch (zhuaNiaoDictionary.Count)
        {
            case 2:
                startIndex = 2;
                break;
            case 4:
                startIndex = 1;
                break;
        }

        Transform tipTransform = null;
        foreach (var zhuaNiaoData in zhuaNiaoDictionary)
        {
            byte pokerValue = (byte)(zhuaNiaoData.Key %100);
            byte pokerState = zhuaNiaoData.Value;
            OnLeftTileChanged(pokerValue, 1);

            byte index = startIndex++;
            GameObject birdGameObject = CreateTile(pokerValue, Postion, Quaternion.identity, birdSit);
            if(birdGameObject)
            {
                tipTransform = ((GameObject)GameObject .Instantiate(MahjongAssetBundle.LoadAsset("UItips_zhuaNiao"))).transform;
                tipTransform.SetParent(MainUITfm.parent, false);
                tipTransform.GetComponentInChildren<Image>().sprite = MahjongAssetBundle.LoadAsset<Sprite>("icon_zhuanao" + pokerState);
                tipTransform.gameObject.SetActive(false);
                m_ZhuaNiaoTransformDictionary.Add(birdGameObject.transform, tipTransform);
                birdGameObject.transform.DOMove(((HzMahjong_RoleLocal)m_PlayerList[0]).m_NiaoTransform.GetChild(index).position, time*0.06f).OnComplete(() =>
                {
                    birdGameObject.layer = ((HzMahjong_RoleLocal)m_PlayerList[0]).m_NiaoTransform.gameObject.layer;
                });
                birdGameObject.transform.rotation = ((HzMahjong_RoleLocal)m_PlayerList[0]).m_NiaoTransform.GetChild(index).rotation;
                birdGameObject.transform.Rotate(Vector3.up,180,Space.Self);
            }
        }
        HideWallTile((byte)zhuaNiaoDictionary.Count, false);
        OnTileNumChanged(zhuaNiaoDictionary.Count);
        yield return new WaitForSecondsRealtime(time * 0.1f);
        float waitTime = time * 0.7f;
        foreach (var zhuaNiaoData in m_ZhuaNiaoTransformDictionary.Reverse())
        {
            zhuaNiaoData.Key.DOLocalRotateQuaternion(Quaternion.identity, time * 0.1f).OnComplete(()=> 
            {
                m_ZhuaNiaoTransformDictionary[zhuaNiaoData.Key].gameObject.SetActive(true);
                UpdateZhuaNiaoUILocalPosition();
            });
            waitTime -= time * 0.1f;
            yield return new WaitForSecondsRealtime(time * 0.1f);
        }
        yield return new WaitForSecondsRealtime(waitTime);
        foreach (KeyValuePair<Transform, Transform> zhuaNiaoValuePair in m_ZhuaNiaoTransformDictionary.Reverse())
        {
            zhuaNiaoValuePair.Key.gameObject.layer = 0;
            Transform target = m_PlayerList[m_dictSitPlayer[birdSit]].GetDiscardTransform();
            zhuaNiaoValuePair.Key.DOMove(target.position, time * 0.2f);
            zhuaNiaoValuePair.Key.DORotateQuaternion(target.rotation, time * 0.2f);
            zhuaNiaoValuePair.Value.gameObject.SetActive(false);
        }
    }
}