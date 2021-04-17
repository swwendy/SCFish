using XLua;
using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

/// <summary>
/// 房间状态
/// </summary>
[LuaCallCSharp]
public enum AnswerRoomState_Enum
{
    RoomState_Init = 0,

    RoomState_TotalBegin,       //总的开始

    RoomState_OnceBeginShow,    //每局开始前的显示

    RoomState_Ask,              //提问

    RoomState_OnceResult,       //一题结果
    RoomState_OnceEnd,          //

    RoomState_TotalResult,      //总的结果
    RoomState_TotalEnd,         //

    RoomState					//回收状态
};

/// <summary>
/// 玩家状态
/// </summary>
[LuaCallCSharp]
public enum AnwserPlayerState_Enum
{
    AnwserPlayerState_Init = 0,
    AnwserPlayerState_Match,            //1匹配中
    AnwserPlayerState_GameIn,           //2游戏中
    AnwserPlayerState_OnGameButIsOver,  //3游戏中但游戏已经结束
    AnwserPlayerState_ReadyHall,        //玩家在大厅
    AnwserPlayerState_OnDesk,           //玩家在桌子上

    AnwserPlayerState_Max
};

[Hotfix]
public class CGame_Answer : CGameBase
{
    /// <summary>
    /// 答题赛主界面对象
    /// </summary>
    GameObject m_AnswerObject = null;

    /// <summary>
    /// 问题回答选项界面对象
    /// </summary>
    Transform m_AnswerOptionTranstrom = null;

    /// <summary>
    /// 问题回答错误提示界面对象
    /// </summary>
    Transform m_AnswerErrorPromptTranstrom = null;

    /// <summary>
    /// 动画挂载对象
    /// </summary>
    Transform m_AnimationTfm = null;

    /// <summary>
    /// 游戏设置界面
    /// </summary>
    Transform m_SetMainTransform = null;

    /// <summary>
    /// 游戏AB资源包
    /// </summary>
    public AssetBundle m_AnswerAssetBundle = null;

    /// <summary>
    /// 问题剩余时间组件对象
    /// </summary>
    Text m_QuestionTimeText = null;

    /// <summary>
    /// 问题选项校验图标(正确或错误)
    /// </summary>
    Image m_ResultIcon = null;

    /// <summary>
    /// 断线重连标记
    /// </summary>
    bool m_bReconnected = false;

    /// <summary>
    /// 问题回答标记(-1:没有答题0:玩家主动答题 1:倒计时结束或答题状态结束)
    /// </summary>
    int m_nSendOptionState = -1;

    /// <summary>
    /// 答题倒计时3秒警告声音
    /// </summary>
    bool m_bWarningAudioState = false;

    /// <summary>
    /// 当前问题索引
    /// </summary>
    /// <param name="gameType"></param>
    byte m_nAnswerIndex = 1;

    /// <summary>
    /// 答题索引
    /// </summary>
    /// <param name="gameType"></param>
    uint m_nAnswerSign = 0;

    /// <summary>
    /// 问题ID
    /// </summary>
    /// <param name="gameType"></param>
    uint m_nCurQuestionID = 0;

    /// <summary>
    /// 当前问题答案序号
    /// </summary>
    List<byte> m_curOptionList = new List<byte>();

    /// <summary>
    /// 房间状态
    /// </summary>
    AnswerRoomState_Enum m_RoomState = AnswerRoomState_Enum.RoomState_Init;

    /// <summary>
    /// 问题剩余时间协程对象
    /// </summary>
    IEnumerator m_QuestionTimeEnumerator = null;

    public CGame_Answer(GameTye_Enum gameType) : base(GameKind_Enum.GameKind_Answer)
    {
        GameMode = gameType;
        InitLoadLuckyTurntableMsgHandle();
    }

    /// <summary>
    /// 游戏初始函数
    /// </summary>
    public override void Initialization()
    {
        base.Initialization();
        LoadAnwserResource();
        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameType, "GameAnswerAudioCsv");
    }

    /// <summary>
    /// 重置游戏UI
    /// </summary>
    public override void ResetGameUI()
    {
        base.ResetGameUI();
    }

    /// <summary>
    /// 游戏逻辑推进
    /// </summary>
    public override void ProcessTick()    {        base.ProcessTick();

        if (m_bReconnected)
        {
            GameMain.hall_.OnGameReconnect(GameType, GameMode);
            m_bReconnected = false;
        }    }

    /// <summary>
    /// 断线重连成功
    /// </summary>
    public override void ReconnectSuccess()
    {
        m_bReconnected = true;
    }

    /// <summary>
    /// 初始化答题赛消息回调
    /// </summary>
    void InitLoadLuckyTurntableMsgHandle()    {
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_ANSWER_SM_ENTERROOM, HandleEnterRoomMsg);        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_ANSWER_SM_MIDDLEENTERROOM, HandleMiddleEnterRoomMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_ANSWER_SM_GETPOKERASKDEAL, HandleSynchronizeAnswerDataMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_ANSWER_SM_ANSWERDOING, HandleSynchronizeQuestionOptionMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_ANSWER_SM_PUBLISHRESULT, HandleGameResultMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CCMsg_ANSWER_SM_ROOMSTATE, HandleRoomStateMsg);    }

    /// <summary>
    /// 进入答题房间成功
    /// </summary>
    bool HandleEnterRoomMsg(uint _msgType, UMessage msg)
    {
        if (GameMode != GameTye_Enum.GameType_Contest)
        {
            return false;
        }
        MatchInGame.GetInstance().ResetGui();
        SetMainPanelMashActive(false);
        CustomAudioDataManager.GetInstance().PlayAudio(1001, false);
        return true;
    }

    /// <summary>
    /// 断线重连
    /// </summary>
    bool HandleMiddleEnterRoomMsg(uint _msgType, UMessage msg)
    {
        SetMainPanelMashActive(false);
        sbyte enterState = msg.ReadSByte();
        if(enterState != 1)
        {
            Debug.Log("中途加入失败！");
            return false;
        }

        AnwserPlayerState_Enum playerState = (AnwserPlayerState_Enum)msg.ReadSByte();
        DebugLog.Log("Middle enter room playerState:" + playerState);

        if (playerState == AnwserPlayerState_Enum.AnwserPlayerState_OnGameButIsOver)
        {            CCustomDialog.OpenCustomConfirmUI(1621, (param) => GameMain.hall_.SwitchToHallScene());
            return false;
        }

        //if (playerState == AnwserPlayerState_Enum.AnwserPlayerState_ReadyHall)
        //{        //    MatchRoom.GetInstance().OnEnd();
        //    return false;
        //}

        msg.ReadUInt();//房间ID
        msg.ReadSByte();//房间等级

        msg.ReadLong();//金钱

        float time = Mathf.Ceil(msg.ReadSingle());//房间当前状态剩余时间

        m_nAnswerIndex = (byte)msg.ReadSByte();//第几局

        byte maxAnswerIndex = msg.ReadByte();//本轮有几局
        msg.ReadUInt();//比赛序号

        m_RoomState = (AnswerRoomState_Enum)msg.ReadSByte();//房间状态
        switch(m_RoomState)
        {
            case AnswerRoomState_Enum.RoomState_Ask:
            case AnswerRoomState_Enum.RoomState_OnceEnd:
            case AnswerRoomState_Enum.RoomState_OnceResult:
                {
                    m_nCurQuestionID = msg.ReadUInt();//问题ID
                    byte optionNum = msg.ReadByte();//问题答案数量
                    m_curOptionList.Clear();
                    for (int optionIndex = 0; optionIndex < optionNum; ++optionIndex)
                    {
                        byte ddd = msg.ReadByte();
                        m_curOptionList.Add(ddd);
                        DebugLog.Log("问题选项 " + ddd);
                    }
                    m_nAnswerSign = msg.ReadUInt(); //提问索引
                    RefreshQuestionPanel(time);
                    if(msg.ReadByte() == 1)
                    {
                        byte optionIndex = msg.ReadByte();//回答的选项
                        long value = msg.ReadLong();//本局钱币改变值
                    
                        m_nCurQuestionID = uint.MaxValue;

                        if (optionIndex < m_AnswerOptionTranstrom.childCount)
                        {
                            m_ResultIcon = m_AnswerOptionTranstrom.GetChild(optionIndex).Find("chooseIcon/resultIcon").GetComponent<Image>();
                            m_AnswerOptionTranstrom.GetChild(optionIndex).Find("chooseIcon/Icon").gameObject.SetActive(true);
                        }

                        if (m_ResultIcon && m_AnswerAssetBundle)
                        {
                            m_ResultIcon.sprite = m_AnswerAssetBundle.LoadAsset<Sprite>(value > 0 ? "dt_widget_result1" : "dt_widget_result2");
                            m_ResultIcon.gameObject.SetActive(true);
                        }

                        ProhibitQuestionOption();
                        m_nSendOptionState = 0;
                    }
                }
                break;
        }
        CustomAudioDataManager.GetInstance().PlayAudio(1001, false);
        return true;
    }

    /// <summary>
    /// 同步问题数据
    /// </summary>
    bool HandleSynchronizeAnswerDataMsg(uint _msgType, UMessage msg)
    {
        m_nCurQuestionID = msg.ReadUInt();//问题ID
        byte optionNum = msg.ReadByte();//问题答案数量
        m_curOptionList.Clear();
        for (int optionIndex = 0; optionIndex < optionNum; ++optionIndex)
        {
            byte ddd = msg.ReadByte();
            m_curOptionList.Add(ddd);
            DebugLog.Log("问题选项 " + ddd);
        }

        float time = msg.ReadSingle();//答题时间
        m_nAnswerSign = msg.ReadUInt(); //提问索引
        m_nSendOptionState = -1;
        RefreshQuestionPanel(time);
        return true;
    }

    /// <summary>
    /// 同步问题答案
    /// </summary>
    bool HandleSynchronizeQuestionOptionMsg(uint _msgType, UMessage msg)
    {
        sbyte answerState = msg.ReadSByte();
        if (answerState != 0)
        {
            DebugLog.Log("回答问题非法！" + answerState);
            return false;
        }
        msg.ReadByte();//选择的序号
        byte correctState = msg.ReadByte();

        RefreshQuestionOptionPrompt(correctState);

        return true;
    }

    /// <summary>
    /// 同步问题答案
    /// </summary>
    bool HandleGameResultMsg(uint _msgType, UMessage msg)
    {
        msg.ReadLong();//剩余钱数
        msg.ReadLong();//本人此局钱币改变值

        m_nAnswerIndex = (byte)msg.ReadSByte();//第几题
        m_nAnswerIndex++; //答题序号
        byte MaxAnswerIndex = msg.ReadByte();//最大问题数

        return true;
    }

    /// <summary>
    /// 房间状态
    /// </summary>
    bool HandleRoomStateMsg(uint _msgType, UMessage msg) 
    {
        AnswerRoomState_Enum state = (AnswerRoomState_Enum)msg.ReadByte();
        OnStateChange(state, msg);
        return true;
    }


    /// <summary>
    /// 加载答题赛资源
    /// </summary>
    bool LoadAnwserResource()
    {
        if (m_AnswerObject != null)
        {
            return true;
        }

        if (GameMode != GameTye_Enum.GameType_Contest)
        {
            return false;
        }

        GameObject gameRoot = GameObject.Find("Canvas/Root");
        if (gameRoot == null)
        {
            DebugLog.LogWarning("答题赛挂接父节点不存在: Canvas/Root");
            return false;
        }

        GameData Answerdata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_Answer);
        if (Answerdata == null)
        {
            DebugLog.LogWarning("答题赛数据资源不存在:" + GameKind_Enum.GameKind_Answer.ToString());
            return false;
        }

        m_AnswerAssetBundle = AssetBundleManager.GetAssetBundle(Answerdata.ResourceABName);
        if (m_AnswerAssetBundle == null)
        {
            DebugLog.LogWarning("答题赛ab资源不存在:" + Answerdata.ResourceABName);
            return false;
        }

        UnityEngine.Object AnswerGameObject = m_AnswerAssetBundle.LoadAsset("Answer_Game");
        m_AnswerObject = (GameObject)GameMain.instantiate(AnswerGameObject);
        m_AnswerObject.transform.SetParent(gameRoot.transform, false);
        m_AnswerOptionTranstrom = m_AnswerObject.transform.Find("Middle/AnswerBG");
        m_AnswerOptionTranstrom.parent.gameObject.SetActive(false);
        m_QuestionTimeText = m_AnswerObject.transform.Find("Middle/CountdownBG/Text_Num").GetComponent<Text>();
        m_AnswerErrorPromptTranstrom = m_AnswerObject.transform.Find("Pop-up/explain/ImageBG");
        m_AnimationTfm = m_AnswerObject.transform.Find("Pop-up/Animation");
        Transform ErrorPromptTransform = m_AnswerErrorPromptTranstrom.parent.Find("Button");
        XPointEvent.AutoAddListener(ErrorPromptTransform.gameObject,OnQuestionErrorPromptOptionClick, m_AnswerErrorPromptTranstrom.parent);

        //设置界面
        if (m_SetMainTransform == null)
        {
            m_SetMainTransform = m_AnswerObject.transform.Find("Pop-up/Set");

            Slider music = m_SetMainTransform.Find("ImageBG/Slider_Music").GetComponent<Slider>();
            Slider sound = m_SetMainTransform.Find("ImageBG/Slider_Sound").GetComponent<Slider>();
            music.value = AudioManager.Instance.MusicVolume;
            sound.value = AudioManager.Instance.SoundVolume;
            music.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.MusicVolume = value; });
            sound.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.SoundVolume = value; });
        }

        XPointEvent.AutoAddListener(m_SetMainTransform.Find("ImageBG/ButtonClose").gameObject, OnClickSetButtonEvent, false);
        m_SetMainTransform.gameObject.SetActive(false);

        //设置按钮
        Transform expandTransform = m_AnswerObject.transform.Find("Top/ButtonExpand");
        XPointEvent.AutoAddListener(expandTransform.gameObject, OnClickSetButtonEvent, true);

        if (!m_bReconnected)
        {
            SetMainPanelMashActive(true);
            MatchInGame.GetInstance().ShowWait();
        }
        return true;
    }

    /// <summary>
    /// 设置遮罩背景界面
    /// </summary>
    /// <param name="activeState"></param>
    private void SetMainPanelMashActive(bool activeState)
    {
        m_AnswerObject.transform.Find("Pop-up/UiRootBG_mask").gameObject.SetActive(activeState);
    }

    /// <summary>
    /// 刷新问题界面
    /// </summary>
    /// <param name="questionTime">回答问题时间</param>
    void RefreshQuestionPanel(float questionTime)
    {
        Answer_QuestionData questionData = null;
        if (m_AnswerObject == null ||
            !Answer_Data.GetInstance().GetAnswerData(m_nCurQuestionID, ref questionData))
        {
            return;
        }

        m_AnswerErrorPromptTranstrom.parent.gameObject.SetActive(false);

        if(m_ResultIcon)
        {
            m_ResultIcon.gameObject.SetActive(false);
        }
        m_AnswerObject.transform.Find("Middle/QuestionsBG/Text_Questions").GetComponent<Text>().text = questionData.m_sQuestion;

        Transform curTransform = null;
        for (int childIndex = 0; childIndex < m_AnswerOptionTranstrom.childCount; ++childIndex)
        {
            curTransform = m_AnswerOptionTranstrom.GetChild(childIndex);
            if (childIndex < m_curOptionList.Count)
            {
                int optionValue = m_curOptionList[childIndex];
                if (optionValue < questionData.m_sOption.Length)
                {
                    curTransform.Find("Text").GetComponent<Text>().text = questionData.m_sOption[optionValue];
                }
                optionValue = childIndex;
                XPointEvent.AutoAddListener(curTransform.gameObject, OnQuestionOptionClick, optionValue);
            }
            curTransform.Find("chooseIcon/Icon").gameObject.SetActive(false);
            curTransform.GetComponent<Button>().interactable = true;
            curTransform.gameObject.SetActive(childIndex < m_curOptionList.Count);
        }

        m_bWarningAudioState = false;

        GameMain.ST(m_QuestionTimeEnumerator);
        m_QuestionTimeEnumerator = UpdateQuestionTime(questionTime);
        GameMain.SC(m_QuestionTimeEnumerator);

        if(!m_AnswerOptionTranstrom.parent.gameObject.activeSelf)
        {
            m_AnswerOptionTranstrom.parent.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 刷新选中问题选项正确或错误
    /// </summary>
    /// <param name="correctState"></param>
    void RefreshQuestionOptionPrompt( byte correctState)
    {
        switch (correctState)
        {
            case 0:
                RefreshQuestionErrorPromptPanel(m_nCurQuestionID);
                break;
            case 1:
                GameFunction.PlayUIAnim("anime_answer_1", 2f, m_AnimationTfm, m_AnswerAssetBundle);
                break;
        }

        if (m_ResultIcon && m_AnswerAssetBundle)
        {
            m_ResultIcon.sprite = m_AnswerAssetBundle.LoadAsset<Sprite>(correctState == 1 ? "dt_widget_result1" : "dt_widget_result2");
            m_ResultIcon.gameObject.SetActive(true);
        }

        CustomAudioDataManager.GetInstance().PlayAudio(correctState == 1 ? 1002 : 1003);
        m_nCurQuestionID = uint.MaxValue;
    }
    /// <summary>
    /// 更新问题剩余时间
    /// </summary>
    /// <param name="questionTime">回答问题剩余时间</param>
    /// <returns></returns>
    IEnumerator UpdateQuestionTime(float questionTime)
    {
        while (true)
        {
            questionTime--;
            m_QuestionTimeText.text = questionTime.ToString() + "s";
            if (questionTime <= 0)
            {
                ProhibitQuestionOption();
                m_nSendOptionState = 1;
                break;
            }
            if (questionTime <= 3 && !m_bWarningAudioState)
            {
                m_bWarningAudioState = true;
                CustomAudioDataManager.GetInstance().PlayAudio(1005);
            }
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    /// <summary>
    /// 刷新回答问题错误提示界面
    /// </summary>
    /// <param name="id">问题ID</param>
    void RefreshQuestionErrorPromptPanel(uint id)
    {
        Answer_QuestionData questionData = null;
        if (m_AnswerErrorPromptTranstrom == null ||
            !Answer_Data.GetInstance().GetAnswerData(id, ref questionData))
        {
            return;
        }

        m_AnswerErrorPromptTranstrom.Find("Questions").GetComponent<Text>().text = questionData.m_sQuestion;
        m_AnswerErrorPromptTranstrom.Find("Answer").GetComponent<Text>().text = questionData.m_sOption[0] + "\n" + questionData.m_sAnswer;
        m_AnswerErrorPromptTranstrom.parent.gameObject.SetActive(true);
    }

    /// <summary>
    /// 回答问题事件
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="parma"></param>
    /// <param name="eventData"></param>
    private void OnQuestionOptionClick(EventTriggerType eventtype, object parma, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerClick && m_nSendOptionState == -1)
        {
            int optionValue = (int)(parma);

            if (optionValue < m_AnswerOptionTranstrom.childCount)
            {
                m_ResultIcon = m_AnswerOptionTranstrom.GetChild(optionValue).Find("chooseIcon/resultIcon").GetComponent<Image>();
                m_AnswerOptionTranstrom.GetChild(optionValue).Find("chooseIcon/Icon").gameObject.SetActive(true);
            }

            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_ANSWER_CM_ANSWERDOING);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add((byte)optionValue);
            msg.Add(m_nAnswerSign);
            HallMain.SendMsgToRoomSer(msg);
            DebugLog.Log("答题 -- 问题ID " + m_nCurQuestionID + "提问ID " + m_nAnswerSign + "选项 " + optionValue);
            ProhibitQuestionOption();
            m_nSendOptionState = 0;
            CustomAudioDataManager.GetInstance().PlayAudio(1004);
        }
    }

    /// <summary>
    /// 回答问题事件
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="parma"></param>
    /// <param name="eventData"></param>
    private void OnQuestionErrorPromptOptionClick(EventTriggerType eventtype, object parma, PointerEventData eventData)
    {
        if(eventtype == EventTriggerType.PointerClick)
        {
            Transform transform = (Transform)parma;
            if(transform)
            {
                transform.gameObject.SetActive(false);
                CustomAudioDataManager.GetInstance().PlayAudio(1004);
            }
        }
    }

    /// <summary>
    /// 游戏设置界面按钮事件
    /// </summary>
    private void OnClickSetButtonEvent(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {
            if (m_SetMainTransform == null)
            {
                return;
            }
            bool activeState = (bool)(button);
            if (m_SetMainTransform.gameObject.activeSelf != activeState)
            {
                m_SetMainTransform.gameObject.SetActive(activeState);
                CustomAudioDataManager.GetInstance().PlayAudio(1004);
            }        }    }

    /// <summary>
    /// 变更游戏房间状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="_ms"></param>
    void OnStateChange(AnswerRoomState_Enum state, UMessage _ms)    {        if (m_RoomState == state)            return;        DebugLog.Log(string.Format("room state change: ({0}->{1})", m_RoomState, state));        OnQuitState(m_RoomState);        m_RoomState = state;        OnEnterState(m_RoomState, _ms);    }

    /// <summary>
    /// 退出游戏房间状态
    /// </summary>
    /// <param name="state"></param>
    void OnQuitState(AnswerRoomState_Enum state)    {    }    /// <summary>
    /// 进入游戏房间状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="_ms"></param>    void OnEnterState(AnswerRoomState_Enum state, UMessage _ms)    {
        switch(state)
        {
            case AnswerRoomState_Enum.RoomState_OnceBeginShow:
                {
                    SetMainPanelMashActive(false);
                    MatchInGame.GetInstance().ShowBegin(Bystander,m_nAnswerIndex);
                }
                break;
            case AnswerRoomState_Enum.RoomState_OnceEnd:
                {
                    if(m_nSendOptionState == -1 || m_nCurQuestionID != uint.MaxValue)
                    {
                        if(m_nSendOptionState == -1)
                        {
                            ProhibitQuestionOption();
                            m_nSendOptionState = 1;
                        }
                        else
                        {
                            m_ResultIcon = null;
                        }
                        RefreshQuestionOptionPrompt(0);
                    }
                    
                    GameMain.ST(m_QuestionTimeEnumerator);
                }
                break;
            case AnswerRoomState_Enum.RoomState_TotalEnd:
                {
                    GameOver();
                }
                break;
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    void GameOver()
    {
        m_nAnswerIndex = 1;
        m_RoomState = AnswerRoomState_Enum.RoomState_Init;
        ProhibitQuestionOption();
        m_nSendOptionState = 1;
        if (m_AnswerOptionTranstrom)
        {
            m_AnswerOptionTranstrom.parent.gameObject.SetActive(false);
        }
        GameMain.Instance.StopAllCoroutines();
    }

    /// <summary>
    /// 禁止答题
    /// </summary>
    void ProhibitQuestionOption()
    {
        for (int childIndex = 0; childIndex < m_AnswerOptionTranstrom.childCount; ++childIndex)
        {
            m_AnswerOptionTranstrom.GetChild(childIndex).GetComponent<Button>().interactable = false;
        }
    }
}