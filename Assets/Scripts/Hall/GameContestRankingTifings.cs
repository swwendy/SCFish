using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using USocket.Messages;
using System.Collections;
using XLua;

/// <summary>
/// 玩家排行榜信息
/// </summary>
[LuaCallCSharp]
public class CPlayerRankingInfo
{
    public string PlayerName ;
    public string PlayerIconUrl;
    public int PlayerFaceID ;
    public int PlayerScore;
    public int PlayerOppScore;
    public byte ShengJuValue;
    public byte ZhiShengValue;
    public byte HouShouValue;
}
/// <summary>
/// 游戏比赛排行榜
/// </summary>
[Hotfix]
public class CGameContestRankingTifings
{
    private Transform MainTransform;
    static CGameContestRankingTifings instance_;
    ScrollRect RightChessScrolllRect;

    float m_fRankingObjectHeight = 0;
    long m_ContestID = 0;
    int m_TotalPlayetNumber = 0;
    bool m_bScrollValueChange = true;
    float m_StartIndex, m_EndIndex;
    string Key = "Mqir*ekMxH&Ji3ecTLCNlIVR@J$r6F";

    IEnumerator CreateRankObjectEnumerator = null;
    List<Transform> RankPlayerList = new List<Transform>();
    SerializableDictionary<long, SerializableDictionary<int, CPlayerRankingInfo>> ContestRankingDataDictionary = new SerializableDictionary<long, SerializableDictionary<int, CPlayerRankingInfo>>();

    public static CGameContestRankingTifings GetChessRankingInstance(bool LoadResourceState = true)
    {
        if (instance_ == null)
            instance_ = new CGameContestRankingTifings();
        if(LoadResourceState)
        {
            instance_.LoadChessRankingResource();
        }
        return instance_;
    }

    public CGameContestRankingTifings()
    {
    }

    /// <summary>
    /// 获得排行界面对象
    /// </summary>
    /// <returns></returns>
    public Transform GetMainTransform()
    {
        return MainTransform;
    }

    /// <summary>
    /// 加载游戏排行榜数据
    /// </summary>
    public void LoadContestRankingData(List<long> ContestIDList)
    {
        if(ContestIDList.Count == 0)
        {
            return;
        }

        SerializableDictionary<long, SerializableDictionary<int, CPlayerRankingInfo>> ContestRankingDictionary =  StateStorage.LoadData<SerializableDictionary<long, SerializableDictionary<int, CPlayerRankingInfo>>>(Key);
        if(ContestRankingDictionary != null)
        {
            ContestRankingDataDictionary = ContestRankingDictionary;
        }
        List<long> RemoveIDList = new List<long>();
        foreach (var value in ContestRankingDataDictionary.Keys)
        {
            if(ContestIDList.FindIndex(id => { return value == id; }) == -1)
            {
                RemoveIDList.Add(value);
            }
        }
        foreach(var keyId in RemoveIDList)
        {
            ContestRankingDataDictionary.Remove(keyId);
        }
    }

    /// <summary>
    /// 清除游戏排行榜数据
    /// </summary>
    public void ClearContestRankingData()
    {
        StateStorage.ClearData(Key);
        ContestRankingDataDictionary.Clear();
    }


    /// <summary>
    /// 保存游戏排行榜数据
    /// </summary>
    public void SaveContestRankingData()
    {
        StateStorage.SaveData<SerializableDictionary<long, SerializableDictionary<int, CPlayerRankingInfo>>>(Key, ContestRankingDataDictionary);
    }

    /// <summary>
    /// 重置排行数据
    /// </summary>
    public void ResetChessRanking()
    {
        RightChessScrolllRect.verticalNormalizedPosition = 1;
        RefreshContestGameRankingPanel();
        RequestContestRankData((ushort)m_StartIndex, (ushort)m_EndIndex);
    }

    /// <summary>
    /// 设置比赛ID
    /// </summary>
    /// <param name="ContestID"></param>
    public void SetSelectContestID(long ContestID)
    {
        m_ContestID = ContestID;
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    private void LoadChessRankingResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);        if (bundle == null)            return;        if (MainTransform == null)        {            RankPlayerList.Clear();            MainTransform = ((GameObject)GameMain.instantiate(bundle.LoadAsset("Lobby_Matchranking_20"))).transform;            MainTransform.gameObject.SetActive(false);            if (RightChessScrolllRect == null)
            {
                RightChessScrolllRect = MainTransform.Find("Imagebg/Viewport_tournament").GetComponent<ScrollRect>();
            }
            CreateGameRankingPlayerUIOjbect(50,true);
            RightChessScrolllRect.onValueChanged.AddListener(OnScrollValueChange);        }
    }

    /// <summary>
    /// 游戏排行榜Tick函数
    /// </summary>
    public void Update()
    {
        if(RightChessScrolllRect == null || MainTransform == null)
        {
            return;
        }

        if(!MainTransform.gameObject.activeSelf)
        {
            return;
        }

        if(!m_bScrollValueChange && RightChessScrolllRect.velocity.sqrMagnitude < 100)
        {
            RequestContestRankData((ushort)m_StartIndex, (ushort)m_EndIndex);
            m_bScrollValueChange = true;
        }
    }

    /// <summary>
    /// 游戏排行榜玩家面板事件
    /// </summary>
    /// <param name="Postion"></param>
    void OnScrollValueChange(Vector2 Postion)
    {
        if(RightChessScrolllRect == null)
        {
            return;
        }
        RefreshContestGameRankingPanel();
        if (RightChessScrolllRect.velocity.sqrMagnitude > 100)
        {
            m_bScrollValueChange = false;
        }
    }

    /// <summary>
    /// 刷新游戏排行榜界面
    /// </summary>
    private void RefreshContestGameRankingPanel()
    {
        if (m_fRankingObjectHeight == 0)
        {
            RectTransform RectObject = RightChessScrolllRect.content.GetChild(0) as RectTransform;
            m_fRankingObjectHeight = RectObject.rect.height;
        }
        Rect rect = RightChessScrolllRect.content.rect;
        m_StartIndex = RightChessScrolllRect.content.anchoredPosition.y;
        RectTransform ChessViewRectTransform = RightChessScrolllRect.transform as RectTransform;
        m_StartIndex = Mathf.CeilToInt(m_StartIndex / m_fRankingObjectHeight) - 1;
        if(m_StartIndex < 0)
            m_StartIndex = 0;
       m_EndIndex = m_StartIndex + Mathf.FloorToInt(ChessViewRectTransform.rect.height / m_fRankingObjectHeight) + 1;
        if (m_EndIndex >= m_TotalPlayetNumber)
            m_EndIndex = m_TotalPlayetNumber - 1;
        //DebugLog.Log("StartY =  " + m_StartIndex + "EndY = " + m_EndIndex);
        Transform RankingTransform = null;
        for (int index = 0; index < RightChessScrolllRect.content.childCount; ++index)
        {
            bool ActiveState = index >= m_StartIndex && index <= m_EndIndex;
            RankingTransform = RightChessScrolllRect.content.GetChild(index);
            if (ActiveState && !RankingTransform.gameObject.activeSelf)
            {
                RankingTransform.gameObject.SetActive(ActiveState);
            }
            RankingTransform.GetChild(0).gameObject.SetActive(ActiveState);
        }
    }

    /// <summary>
    /// 创建游戏排行榜玩家对象
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <param name="ActiveState"></param>
    /// <returns></returns>
    public void CreateGameRankingPlayerUIOjbect(int playerNumber, bool ActiveState = false)
    {
        m_TotalPlayetNumber = playerNumber;
        GameMain.ST(CreateRankObjectEnumerator);
        CreateRankObjectEnumerator = CreateGameRankingPlayerOjbect(playerNumber, ActiveState);
        GameMain.SC(CreateRankObjectEnumerator);
    }

    /// <summary>
    /// 创建游戏排行榜玩家对象
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <param name="ActiveState"></param>
    /// <returns></returns>
    private IEnumerator CreateGameRankingPlayerOjbect(int playerNumber,bool ActiveState = false)
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (RightChessScrolllRect == null || bundle == null)
        {
            yield break;
        }
        Transform ParentTransform = RightChessScrolllRect.transform.Find("Content_tournament");
        if(ParentTransform.childCount >= playerNumber)
        {
            if(RankPlayerList.Count == playerNumber)
            {
                yield break;
            }

            if (RankPlayerList.Count > playerNumber)
            {
                for (int index = playerNumber; index < RankPlayerList.Count; ++index)
                {
                    RankPlayerList[index].gameObject.SetActive(false);
                }
                RankPlayerList.RemoveRange(playerNumber, RankPlayerList.Count - playerNumber); ;
            }
            else
            {
                Transform PlayerTransform = null;
                for (int index = RankPlayerList.Count; index < playerNumber; ++index)
                {
                    PlayerTransform = ParentTransform.GetChild(index);
                    PlayerTransform.gameObject.SetActive(true);
                    RankPlayerList.Add(PlayerTransform);
                }
            }
        }
        else
        {
            playerNumber -= ParentTransform.childCount;
            while (playerNumber > 0)
            {
                if (RightChessScrolllRect == null || bundle == null)
                {
                    break;
                }
                int TotalNumber = ActiveState ? playerNumber : 20;
                if(playerNumber < TotalNumber)
                {
                    TotalNumber = playerNumber;
                }
                playerNumber -= TotalNumber;
                for (int index = 0; index < TotalNumber; ++index)
                {
                    GameObject RankingObject = (GameObject)GameMain.instantiate(bundle.LoadAsset("Matchranking_20"));
                    if (RankingObject)
                    {
                        RankingObject.transform.SetParent(ParentTransform, false);
                        RankingObject.gameObject.SetActive(ActiveState);
                        if(ActiveState)
                        {
                            RankPlayerList.Add(RankingObject.transform);
                        }
                        RankingObject.transform.Find("BG/Textpaiming").GetComponent<Text>().text = ParentTransform.childCount.ToString();
                    }
                }
                yield return new WaitForSecondsRealtime(1);
            }
        }
        yield break;
    }
   
    /// <summary>
    /// 更新游戏排行榜数据
    /// </summary>
    /// <param name="_ms"></param>
    public bool UpdateGameRankingData(UMessage _ms)
    {
        CCustomDialog.CloseCustomWaitUI();
        RightChessScrolllRect.vertical = true;
        byte State = _ms.ReadByte();
        if(State != 0)
        {
            Debug.Log("比赛排行榜数据错误！错误码:"+ State);
            return false;
        }

        SerializableDictionary<int, CPlayerRankingInfo> GameRankingDataDictionary = null;
        long ContestID = _ms.ReadLong();
        if(!ContestRankingDataDictionary.ContainsKey(ContestID))
        {
            ContestRankingDataDictionary.Add(ContestID, new SerializableDictionary<int, CPlayerRankingInfo>());
        }
        GameRankingDataDictionary = ContestRankingDataDictionary[ContestID];
        ushort StartIndex = _ms.ReaduShort();
        byte DataNum = _ms.ReadByte();
        ushort EndIndex =(ushort)(StartIndex + DataNum -1);
        CPlayerRankingInfo PlayerRankingInfo = null;
        for (int index = 0; index < DataNum; ++index)
        {
            PlayerRankingInfo = new CPlayerRankingInfo();
            PlayerRankingInfo.PlayerName = _ms.ReadString();
            PlayerRankingInfo.PlayerIconUrl = _ms.ReadString();
            PlayerRankingInfo.PlayerFaceID = _ms.ReadInt();
            PlayerRankingInfo.PlayerScore = _ms.ReadInt();
            PlayerRankingInfo.PlayerOppScore = _ms.ReadInt();
            PlayerRankingInfo.ShengJuValue = _ms.ReadByte();
            PlayerRankingInfo.ZhiShengValue = _ms.ReadByte();
            PlayerRankingInfo.HouShouValue = _ms.ReadByte();
            int RankIndex = StartIndex + index;
            if(GameRankingDataDictionary.ContainsKey(RankIndex))
            {
                GameRankingDataDictionary[RankIndex] = PlayerRankingInfo;
            }
            else
            {
                GameRankingDataDictionary.Add(RankIndex, PlayerRankingInfo);
            }
        }

        if(m_ContestID == ContestID)
        {
            RefreshGameContestRankingPlayerPanel(StartIndex, EndIndex);
        }
        return true;
    }

    /// <summary>
    /// 请求比赛排行数据
    /// </summary>
    /// <param name="StartIndex"></param>
    /// <param name="EndIndex"></param>
    void RequestContestRankData(ushort StartIndex, ushort EndIndex)
    {
        ushort _StartIndex = StartIndex, _EndIndex = EndIndex;
        if(ContestRankingDataDictionary.ContainsKey(m_ContestID))
        {
            bool State = true;
            while(State && _StartIndex <= _EndIndex)
            {
                State = false;
                if(ContestRankingDataDictionary[m_ContestID].ContainsKey(_StartIndex))
                {
                    ++_StartIndex;
                    State = true;
                }
                if (ContestRankingDataDictionary[m_ContestID].ContainsKey(_EndIndex))
                {
                    --_EndIndex;
                    State = true;
                }
            }
        }
        if(_StartIndex <= _EndIndex)
        {
            RightChessScrolllRect.vertical = false;
            CCustomDialog.OpenCustomWaitUI("正在加载数据...");
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_ReqContestRankData);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(m_ContestID);
            msg.Add(_StartIndex);
            msg.Add(_EndIndex);
            NetWorkClient.GetInstance().SendMsg(msg);
        }
        else
        {
            RefreshGameContestRankingPlayerPanel(StartIndex, EndIndex);
        }
    }

    /// <summary>
    /// 刷新排行榜玩家数据
    /// </summary>
    /// <param name="StartIndex"></param>
    /// <param name="EndIndex"></param>
    private void RefreshGameContestRankingPlayerPanel(ushort StartIndex, ushort EndIndex)
    {
        Transform PlayerInfoTransform = null;
        CPlayerRankingInfo PlayerDataInfo = null;
        for (int index = StartIndex; index <= EndIndex; ++index)
        {
            PlayerDataInfo = ContestRankingDataDictionary[m_ContestID][index];
            PlayerInfoTransform = RightChessScrolllRect.content.GetChild(index).GetChild(0);
            Image HeadImage = PlayerInfoTransform.Find("playerinfo/Image_HeadBG/Image_HeadMask/Image_HeadImage").GetComponent<Image>();
            HeadImage.sprite = GameMain.hall_.GetIcon(PlayerDataInfo.PlayerIconUrl, (uint)(m_ContestID*1000 + index), PlayerDataInfo.PlayerFaceID);
            PlayerInfoTransform.Find("playerinfo/TextName").GetComponent<Text>().text = PlayerDataInfo.PlayerName;
            PlayerInfoTransform.Find("Textjifen").GetComponent<Text>().text = PlayerDataInfo.PlayerScore.ToString();
            PlayerInfoTransform.Find("Textxiaofen").GetComponent<Text>().text = PlayerDataInfo.PlayerOppScore.ToString();
            PlayerInfoTransform.Find("Textshengju").GetComponent<Text>().text = PlayerDataInfo.ShengJuValue.ToString();
            PlayerInfoTransform.Find("Textzhisheng").GetComponent<Text>().text = PlayerDataInfo.ZhiShengValue.ToString();
            PlayerInfoTransform.Find("Texthoushou").GetComponent<Text>().text = PlayerDataInfo.HouShouValue.ToString();
        }
    }
}
