﻿using DG.Tweening;

[LuaCallCSharp]
public enum PongKongType
{
    ePKT_None,
    ePKT_Pong,
    ePKT_Pong2Kong,
    ePKT_CancelKong,
    ePKT_Kong_Exposed,
    ePKT_Kong_Concealed,
    ePKT_Win,
    ePKT_Chi
}

[LuaCallCSharp]
public class PongKongData
{
    public byte value;
    public PongKongType pkt;
    public Transform tileTfm;
}

[Hotfix]

    {
        get
        {
            return m_nSex;
        }
        set
        {
            if (value != m_nSex)
            {
                if (m_RoleAnim != null)
                {
                    GameObject.Destroy(m_RoleAnim.gameObject);
                    m_RoleAnim = null;
                }
            }
            m_nSex = value;
        }
    }
    protected byte m_nDiscardIndex = 0;
    protected byte m_nWinIndex = 0;

    Animator m_RoleAnim = null;
    protected List<PongKongData> m_PKDList = new List<PongKongData>();

    public int m_faceid;
    public string m_url;

    //明牌在牌堆的位置
    Vector3 BrightCardPostion = Vector3.zero;
    Quaternion BrightCardQuaternion = Quaternion.identity;

    protected bool m_bTileMoveEnd = true;
        Transform tfm = GameBase.MainUITfm.Find("Middle/PlayerInfor/PlayerInfor_" + index);
            m_ChangeTfm = GameBase.ChangeTfm.Find("player_" + index + "/majiang_point");
        }

        m_LackStartPoint = GameBase.MainUITfm.Find("Pop-up/Animation/point_Player_" + index);
        {
            XPointEvent.AutoAddListener(tfm.gameObject, OnClickShowHua, null);
            tfm.Find("Text").GetComponent<Text>().text = "x0";
            m_HuaTfm = m_PopupUI.Find("hua_info");
        }

        m_EmotionTfm = m_PopupUI.Find("Chat_Text");
        {
            m_EmotiomText = m_EmotionTfm.Find("Text").GetComponent<Text>();

    void CreateRole()
    {
        if (m_RoleAnim != null)
            return;

        Transform tfm = m_WinTfm.parent.Find("Sit");
        if (tfm == null)
            return;

        string str = Sex == 2 ? "New3D_nv_1" : "New3D_nan_1";
        UnityEngine.Object obj = (GameObject)GameBase.CommonAssetBundle.LoadAsset(str);
            GameObject gameObj = (GameObject)GameMain.instantiate(obj);
            gameObj.transform.SetParent(tfm, false);
            m_RoleAnim = gameObj.GetComponent<Animator>();
            gameObj.SetActive(false);

    void PlayRoleAnim(string anim)
    {
        if (m_RoleAnim == null)
            return;

        m_RoleAnim.SetTrigger(anim);
    }

    {
        PlayRoleAnim(win ? "Trigger_win" : "Trigger_lsot");
    }

    public void SetupInfoUI(uint userId, long coin, string name, string url, int faceId, float masterScore, byte male)
        tfm.GetComponent<Text>().text = name;

        CreateRole();

        tfm = m_InfoUI.Find("BG_nameJifen/Jifen_BG");
        tfm.gameObject.SetActive(!GameBase.IsFree);


    public void UpdateInfoUI(long coin)


    public Sprite GetHeadImg()
    {
        Transform tfm = m_InfoUI.Find("Head/HeadMask/ImageHead");
        return tfm.GetComponent<Image>().sprite;
    }
    public string GetRoleName()
    {
        return m_InfoUI.Find("BG_nameJifen/Name_Text").GetComponent<Text>().text;
    }

    bool ShowPlayerInfo(bool show = true)

        if (m_RoleAnim != null)

    public virtual void Init()
    public virtual void OnEnd()
        m_nWinIndex = 0;
        m_LastFirstTiles.Clear();

            GameObject.DestroyImmediate(m_WallTfm.GetChild(i).gameObject);
        m_WallTfm.gameObject.SetActive(false);

        InitHandTile();
        m_HandTfm.localEulerAngles = Vector3.zero;

        if (m_RoleAnim != null)
            m_RoleAnim.Rebind();

        foreach (Transform tfm in m_WinTfm)
        {

            Transform t = tfm.Find("Effectpoint");
            foreach (Transform child in t)
                GameObject.Destroy(child.gameObject);
        }
        if(m_LackStartPoint != null)
            foreach (Transform tfm in m_LackStartPoint)
                GameObject.Destroy(tfm.gameObject);

        if(m_ChangeTfm != null)
            foreach (Transform tfm in m_ChangeTfm)
                GameObject.Destroy(tfm.gameObject);

        m_PopupUI.Find("Icon_Zhuang").gameObject.SetActive(false);

        if (m_HuaTfm != null)
        {
            m_PopupUI.Find("Text_ting").GetComponent<Text>().text = "";
            for (int i = m_HuaTfm.childCount - 1; i >= 0; i--)
                GameObject.DestroyImmediate(m_HuaTfm.GetChild(i).gameObject);
        }
        ShowAnswer();
        ShowChange(false);
    }
    {
        m_nSit = RoomInfo.NoSit;
        ShowAnswer();
        ShowChange(false);

        ShowPlayerInfo(false);
    }
    {
        for (int i = m_WallTfm.childCount - 1; i >= 0; i--)
            GameObject.DestroyImmediate(m_WallTfm.GetChild(i).gameObject);

        GameObject obj = (GameObject)GameBase.MahjongAssetBundle.LoadAsset("paiqiang_" + num);
        obj = GameMain.Instantiate(obj);
        obj.transform.SetParent(m_WallTfm, false);

        //create change tile
        if (m_ChangeTfm != null)
        {
            obj = (GameObject)GameBase.MahjongAssetBundle.LoadAsset("majiang_Change_" + GameBase.ChangeTileNum);
            obj = GameMain.Instantiate(obj);
            obj.transform.SetParent(m_ChangeTfm, false);
    {
        m_WallTfm.gameObject.SetActive(true);
    {
        foreach(Transform tfm in m_HandTfm)
        {
            Mahjong_Tile tile = tfm.GetComponent<Mahjong_Tile>();
            if(tile == null)
            {
                tile = tfm.gameObject.AddComponent<Mahjong_Tile>();
                tile.OwnerRole = this;
            }
        }
    }
    {
        if (m_WallTfm.childCount == 0)
            return 0;
        return m_WallTfm.GetChild(0).childCount;
    }


    public Vector3 GetDealWallTilePostion(int dealIndex)
    {
        int index = dealIndex % 100;
        if (m_WallTfm.childCount < index)
            return m_WallTfm.GetChild(0).position;
        return m_WallTfm.GetChild(0).GetChild(index).position;
    }

    public Transform GetDiscardTransform(bool reverse = false)
    {
        if(m_nDiscardIndex > m_DiscardTfm.childCount)
        {
            return null;
        }
        return m_DiscardTfm.GetChild(reverse ? m_nDiscardIndex-- :m_nDiscardIndex++);
    }

    //num < 0 表示放回来显示
    public int HideWallTile(int beginIndex, int num,int brightCardIndex )//返回有多少张不在这里
    {
        if (m_WallTfm.childCount == 0 || num == 0)
            return num;

        Transform parent = m_WallTfm.GetChild(0);
        int count = parent.childCount;
        int i = 0;
        int index;
        Transform child;

        if(num > 0)
        {
            for (; i < num; i++)
            {
                index = beginIndex + i;
                if (index >= count)
                    break;

                child = parent.GetChild(index);

                if (brightCardIndex != -1 && 
                    (brightCardIndex == index ||index == brightCardIndex + 1))
                {
                    if(index == brightCardIndex + 1)
                    {
                        BrightCardPostion = parent.GetChild(brightCardIndex).position;
                        parent.GetChild(brightCardIndex).position = child.position;
                    }else
                    {
                        break;
                    }
                }

                if (!child.gameObject.activeSelf)
                    break;

                child.gameObject.SetActive(false);
            }
        {
            num = -num;
            for (; i < num; i++)
            {
                index = beginIndex - i;
                if (index < 0)
                    break;

                if (brightCardIndex != -1&&
                    (brightCardIndex == index || index == brightCardIndex + 1))
                {
                    if (index == brightCardIndex + 1)
                    {
                        parent.GetChild(brightCardIndex).position = BrightCardPostion;
                    }
                    else
                    {
                        return 0;
                    }
                }

                child = parent.GetChild(index);
                if (child.gameObject.activeSelf)
                    break;

                child.gameObject.SetActive(true);
            }
        }
    {
        GameBase.OnTileNumChanged((byte)list.Count);
    {
        if (tileValue == byte.MaxValue)
            return false;

        Transform tfm = m_HandTfm.GetChild(0);
        if (tfm.gameObject.activeSelf)
        {
            {
                m_HaveTiles.Remove(tileValue);
                GameBase.OnTileNumChanged(-1);
                tfm.GetComponent<Mahjong_Tile>().Reset();
                return true;
            }

            return false;
        }
        tfm.gameObject.SetActive(true);
        m_HaveTiles.Insert(0, tileValue);
        Mahjong_Tile tile = tfm.GetComponent<Mahjong_Tile>();
        tile.Value = tileValue;

        if(show)
        {
            GameBase.SetTileMat(tfm, tileValue, GameBase.Bystander);
            tile.Lack = (!GameBase.Bystander && JudgeTile.GetTileSuit(tileValue) == m_nLackSuit);
        }

        UpdateAtomicCardTileColor(ref tile);

        CustomAudioDataManager.GetInstance().PlayAudio(3003);

        GameBase.OnTileNumChanged(1);
        return true;
    }
    {
        int atomicCardCount = 0;
        if (GameBase.m_AtomicCard != 0xff)
        {
            atomicCardCount = tileDataList.RemoveAll(value => value == GameBase.m_AtomicCard);
        }

        tileDataList.Sort();

        for (int index = 0; index < atomicCardCount; ++index)
        {
            DebugLog.Log("原子数据：" + GameBase.m_AtomicCard);
            tileDataList.Insert(0, GameBase.m_AtomicCard);
        }
    }
    {
        MahjongTileSort(ref m_HaveTiles);

        List<byte> list = m_HaveTiles.FindAll(s => JudgeTile.GetTileSuit(s) == m_nLackSuit);
        foreach (byte t in list)
            m_HaveTiles.Remove(t);
        m_HaveTiles.AddRange(list);

        if (useLastFirst && m_LastFirstTiles.Count > 0)
        {
            {
                m_LastFirstTiles.RemoveAt(index);
            }
        ReArrayTiles(hideNum, selList);
    }
    public virtual bool OnDiscardTile(byte value, byte index = 0, float time = 1f)
    {
        if (m_nDiscardIndex >= m_DiscardTfm.childCount)
            return false;

        if(time < 0f)//reverse
        {
            m_nDiscardIndex--;
            GameBase.SetCurDiscardTile(null);

            m_HaveTiles.Add(value);
            UpdateTiles(0, null, true);

            return true;
        }

        if (index >= m_HandTfm.childCount)
        {
            DebugLog.LogWarning("OnDiscardTile failed! index:" + index);
            return false;
        }
        CustomAudioDataManager.GetInstance().PlayAudio(3003);

        if (TingState > MjTingState_Enum.MjTingState_TingTypeNum
                    && TingState < MjTingState_Enum.MjTingState_Ting)
            OnTingEnd();

        return true;
    }

    public virtual void DiscardTile(Mahjong_Tile tile, byte value)
    {
    }

    protected virtual void RemoveTileByValue(byte value, int num, bool midEnt, float time = 1f)
    {
    }

    protected virtual void RemoveTileByValue(List<byte> valueList, bool midEnt, float time = 1f)
    {
    }

    public void OnOtherDoing(byte value, PongKongType type, float time = 1f)
    {
        if (m_nDiscardIndex == 0 && time > 0f)
            return;

        if (time > 0)
            PlayRoleAnim(false);

        if(type == PongKongType.ePKT_Pong || type == PongKongType.ePKT_Kong_Exposed || 
           type == PongKongType.ePKT_Win  || type == PongKongType.ePKT_Chi)
        {
            if(time > 0f)
            {
                m_nDiscardIndex--;
                GameBase.SetCurDiscardTile(null);
            {
                Transform target = m_DiscardTfm.GetChild(m_nDiscardIndex++);
                GameObject obj = GameBase.CreateTile(value, target.position, target.rotation, m_nSit);
                GameBase.SetCurDiscardTile(obj);
            }
            ArrayTileOnPongOrKong(value, PongKongType.ePKT_CancelKong, false, time);

    void ShowAnswer(int flag, float time, bool bFlag)
    {
        if (flag < 0 || (bFlag && flag == 0))
            return;

        string[] answerStr = new string[] { "碰", "房费", "杠", "杠", "杠", "胡", "自摸", "退税", "大叫", "花猪", "呼叫转移", "" };
        string str = "";
        bool hide = true;
        if(bFlag)
        {
            foreach (MjChangeCoinType_Enum item in System.Enum.GetValues(typeof(MjChangeCoinType_Enum)))
            {
                int j = (int)item;
                if (GameKind.HasFlag(j, flag))
                {
                    if (str.Length != 0)
                        str += "·";
                    str += answerStr[j];

                    if (j > 4 && j < 10)
                        hide = false;
                }
            }
        {
            str = answerStr[flag];
            if (flag > 4 && flag < 10)
                hide = false;
        }
        ShowAnswer(str, time, hide);
    }

    void ShowAnswer(MjChangeCoinType_Enum doing, float time)
    {
        ShowAnswer((int)doing, time, false);
    }

    void ShowAnswer(string str = "", float time = 1f, bool hide = true)
    {
        Transform tfm = m_PopupUI.Find("Image_zhuangtai");
        Text text = tfm.GetComponentInChildren<Text>();
        if (time < 0f || str.Length == 0)
        {
            tfm.localScale = Vector3.one;
            text.text = "";
            return;
        }
        text.text = str;
        tfm.DOScale(Vector3.one, 0.1f * time);

        if (hide)
            GameMain.WaitForCall(time, () =>
            {
                if(text != null)
                    text.text = "";
            });
    }

    //kongType(0:碰 1:明 2：暗 3:补) time<0(撤销)
    public virtual PongKongType PongKongTile(byte value, byte kongType, bool midEnt = false, float time = 1f, MjTileDirection_Enum direction = MjTileDirection_Enum.TileDirection_Front)
    {
        int removeNum = 2;
        PongKongType type = PongKongType.ePKT_Pong;
        if (kongType > 0)
        {
            removeNum = 3;
            type = PongKongType.ePKT_Kong_Exposed;
            if (kongType == 3)
            {
                type = PongKongType.ePKT_Pong2Kong;
                if(midEnt)
                {
                    removeNum += 3;
                }
            }
            {
                type = PongKongType.ePKT_Kong_Concealed;
            }

            if (!midEnt)
            {
                ShowAnswer(MjChangeCoinType_Enum.MjChangeCoinType_Gang_Ming, time);
                if(time > 0f)
                {
                    PlayRoleAnim(true);
                }
        else if (!midEnt)
        {
            if (time > 0f)
            {
                PlayRoleAnim(true);
            }

        RemoveTileByValue(value, removeNum, midEnt, time);
        ArrayTileOnPongOrKong(value, type, midEnt, time,direction);
    }

    //吃相关逻辑value(吃的牌)firstValue(首张牌)
    public virtual PongKongType ChiTile(byte value, byte firstValue, bool midEnt = false, float time = 1f, MjTileDirection_Enum direction = MjTileDirection_Enum.TileDirection_Front)
    {
        if (!midEnt)
        {
            ShowAnswer("吃", time);
            if (time > 0f)
            {
                CustomAudioDataManager.GetInstance().PlayAudio(Sex * 1000 + 501);
                PlayRoleAnim(true);
            }
        }

        byte tempValue = 0;
        List<byte> ChiTileList = new List<byte>();
        for (int index = 0; index < 3; ++index)
        {
            tempValue = (byte)(firstValue + index);
            if (!midEnt && tempValue == value)
            {
                continue;
            }
            ChiTileList.Add(tempValue);
        }

        RemoveTileByValue(ChiTileList, midEnt, time);

        Transform tfm = null;
        if (time > 0f)
        {
            if (m_PKDList.Count == m_PongTfm.childCount)
            {
                DebugLog.LogError("chi num is out range:" + m_PongTfm.childCount);
                return PongKongType.ePKT_None;
            }

            tfm = m_PongTfm.GetChild(m_PKDList.Count);

            PongKongData data = new PongKongData();
            data.pkt = PongKongType.ePKT_Chi;
            data.value = firstValue;
            data.tileTfm = GameBase.CreatePongOrKong(value, PongKongType.ePKT_Chi, tfm, direction, firstValue);
            m_PKDList.Add(data);
        }
        else
        {
            int index = m_PKDList.FindIndex(s => s.value == firstValue);
            if (index >= 0)
            {
                tfm = m_PKDList[index].tileTfm;
                GameObject.Destroy(tfm.gameObject);
                m_PKDList.RemoveAt(index);
            }
        }

        if (!midEnt)
        {
            else
                UpdateTiles(-1, null, true);
        }
        return PongKongType.ePKT_Chi;
    }

    public virtual void OnWin(byte tile, uint result, uint addCoin, bool end, float time = 1f)
    {
        if(addCoin != 0)
            ShowReward(addCoin);

        if(time > 0f)
        {
            PlayRoleAnim(true);

            Transform tfm = m_HandTfm.GetChild(0);
            if (tfm.gameObject.activeSelf)
            {
                m_HaveTiles.RemoveAt(0);
                tfm.gameObject.SetActive(false);
            }

            if (m_nWinIndex >= m_WinTfm.childCount)
                return;
            Transform target = m_WinTfm.GetChild(m_nWinIndex++);
            target.gameObject.SetActive(true);
            GameBase.SetTileMat(target, tile);
            GameBase.PlayEffect(false, "Effect_hu", time, 0, target.Find("Effectpoint"));
        }
        {
            if (OnTurn)
            {
                ReArrayTiles(0);
            }
            Transform target = m_WinTfm.GetChild(--m_nWinIndex);
            target.gameObject.SetActive(false);
            Transform tfm = target.Find("Effectpoint");
            foreach (Transform child in tfm)
                GameObject.Destroy(child.gameObject);
        }

    /// <summary>
    /// 更新原子牌的颜色
    /// </summary>
    protected virtual void UpdateAtomicCardTileColor(ref Mahjong_Tile tile)
    {
        foreach (Material mat in tile.transform.GetComponent<MeshRenderer>().materials)
        {
            if(tile.Value == GameBase.m_AtomicCard)
            {
                mat.color = Color.yellow;
            }
            else
            {
                if (mat.color == Color.yellow)
                {
                    mat.color = Color.white;
                }
            }
        }
    }

    public void ReArrayTiles(int hideNum = -1, List<byte> selList = null, bool forceShow = false)
    {
        if (m_HaveTiles.Count == 0)
            return;

        if (hideNum < 0)
            hideNum = OnTurn ? 0 : 1;

        Transform tfm;
        int i = 0;
        for(int j = 0; j < hideNum; j++)
        {
            tfm = m_HandTfm.GetChild(i++);
            tfm.GetComponent<Mahjong_Tile>().Reset();
        }

        for (int j = 0; j < m_HaveTiles.Count; j++)
        {
            if(i >= m_HandTfm.childCount)
            {
                DebugLog.LogWarning("rearray tiles failed on:" + i);
                break;
            }
            tfm = m_HandTfm.GetChild(i++);
            tfm.gameObject.SetActive(true);
            GameBase.SetTileMat(tfm, m_HaveTiles[j], forceShow ? false : GameBase.Bystander);
            Mahjong_Tile tile = tfm.GetComponent<Mahjong_Tile>();
            tile.Value = m_HaveTiles[j];
            tile.Lack = (!GameBase.Bystander && tile.Value > 0 && (JudgeTile.GetTileSuit(tile.Value) == m_nLackSuit));
            UpdateAtomicCardTileColor(ref tile);

            if (selList != null && selList.Contains(tile.Value))
            {
                tile.OnSelect(true, false);
                selList.Remove(tile.Value);
                GameMain.WaitForCall(1f, () =>
                {
                    tile.OnSelect(false);
                });
            }
            else
                tile.OnSelect(false, false);
        }

        for (; i < m_HandTfm.childCount; i++)
        {
            tfm = m_HandTfm.GetChild(i);
            tfm.GetComponent<Mahjong_Tile>().Reset();
        }
    }

    protected virtual void ArrayTileOnPongOrKong(byte value, PongKongType type, bool midEnt, float time = 1f, MjTileDirection_Enum direction = MjTileDirection_Enum.TileDirection_Front)
    {
        Transform tfm;
        if (((type != PongKongType.ePKT_Pong2Kong) || (midEnt && type == PongKongType.ePKT_Pong2Kong)) && type != PongKongType.ePKT_CancelKong)
        {
            if(time > 0f)
            {
                if (m_PKDList.Count == m_PongTfm.childCount)
                {
                    DebugLog.LogError("pong num is out range:" + m_PongTfm.childCount);
                    return;
                }

                tfm = m_PongTfm.GetChild(m_PKDList.Count);

                PongKongData data = new PongKongData();
                data.pkt = type;
                data.value = value;
                data.tileTfm = GameBase.CreatePongOrKong(value, type, tfm, direction);
                m_PKDList.Add(data);
            {
                int index = m_PKDList.FindIndex(s => s.value == value);
                if (index >= 0)
                {
                    tfm = m_PKDList[index].tileTfm;
                    GameObject.Destroy(tfm.gameObject);
                    m_PKDList.RemoveAt(index);
                }
            }
        {
            int index = m_PKDList.FindIndex(s => s.value == value);
            if(index >= 0)
            {
                tfm = m_PKDList[index].tileTfm;
                tfm = tfm.GetChild(3);
                bool cancel = (type == PongKongType.ePKT_CancelKong) ^ (time > 0f);
                if (cancel)
                {
                    m_PKDList[index].pkt = PongKongType.ePKT_Pong2Kong;
                    tfm.gameObject.SetActive(true);
                    GameBase.SetTileMat(tfm, value);
                }
                else
                {
                    m_PKDList[index].pkt = PongKongType.ePKT_Pong;
                    tfm.gameObject.SetActive(false);
                }
            }
            {
                if (time > 0f)
                    m_HaveTiles.AddRange(new byte[] { value, value, value });
                else
                {
                    return;
                }

        if (!midEnt)
        {
            else
                UpdateTiles(-1, null, true);
        }

    protected void InsertTile(int dealIndex, int targetIndex)
    {
        if (dealIndex == 0)
            return;

        Transform tfm0 = m_HandTfm.GetChild(0);
        if (tfm0 == null || !tfm0.gameObject.activeSelf)
            return;

        //DebugLog.Log("deal:" + dealIndex + " target:" + targetIndex);
        m_bTileMoveEnd = false;
        Transform tfmDeal = m_HandTfm.GetChild(dealIndex);
        Vector3 pos = m_HandTfm.GetChild(targetIndex).localPosition;

        Transform tfm, nextTfm;
        if(dealIndex > targetIndex)//left move
        {
            tfm = m_HandTfm.GetChild(targetIndex);
            for(int i = targetIndex; i < dealIndex; i++ )
            {
                nextTfm = m_HandTfm.GetChild(i + 1);
                tfm.DOLocalMoveX(nextTfm.localPosition.x, 0.1f);
                tfm = nextTfm;
            }
        }
        else if(dealIndex < targetIndex)
        {
            tfm = m_HandTfm.GetChild(targetIndex);
            for (int i = targetIndex; i > dealIndex; i--)
            {
                nextTfm = m_HandTfm.GetChild(i - 1);
                tfm.DOLocalMoveX(nextTfm.localPosition.x, 0.1f);
                tfm = nextTfm;
            }
        }

        Vector3 pos0 = tfm0.localPosition;
        if(targetIndex == 1)
        {
            tfm0.DOLocalMove(pos, 0.1f);
            m_bTileMoveEnd = true;
        }
        else
        {
            tfm0.DOLocalRotate(new Vector3(0f, 0f, 30f), 0.1f);
            tfm0.DOLocalMoveY(tfm0.localPosition.y + 1f, 0.1f).OnComplete(() =>
            {
                tfm0.DOLocalMoveX(pos.x, 0.05f * targetIndex).OnComplete(() =>
                {
                    tfm0.DOLocalRotate(Vector3.zero, 0.1f).OnComplete(() =>
                    {
                        tfm0.DOLocalMove(pos, 0.1f);
                        m_bTileMoveEnd = true;
                    });
                });
            });
        tfmDeal.SetSiblingIndex(0);
        tfm0.SetSiblingIndex(targetIndex);
    }

    public virtual void ShowChange(bool show, bool midEnt = false, List<byte> list = null)
    {
        if (m_ChangeTfm == null)
            return;

        m_ChangeTfm.gameObject.SetActive(show);

        if(!show)
            m_ChangeTfm.localEulerAngles = Vector3.zero;
        else if(list == null)
        {
            m_ChangeTfm.localEulerAngles = Vector3.zero;
            if (!midEnt)
            {
                Vector3 pos = m_ChangeTfm.localPosition;
                float oldZ = pos.z;
                pos.z -= 1f;
                m_ChangeTfm.localPosition = pos;
                m_ChangeTfm.DOLocalMoveZ(oldZ, 0.3f);
            }
        {
            m_ChangeTfm.localEulerAngles = new Vector3(0f, 180f, 180f);

            Transform child = m_ChangeTfm.GetChild(0);
            if(child.childCount >= list.Count)
            {
                list.Sort();
                list.Reverse();
                for(int i = 0; i < list.Count; i++)
                {
                    GameBase.SetTileMat(child.GetChild(i), list[i]);
                    m_HaveTiles.Remove(list[i]);
                }

    public virtual void GetChangedTiles(List<byte> tiles)
    {
    }

    public virtual void OnLackCofirm(byte suit, bool midEnt, float time = 3f)
    {
        if(suit > 2)
        {
            DebugLog.LogWarning("wrong lack suit:" + suit);
            return;
        }

        if(time < 0f)
        {
            UpdateTiles();
            return;
        }
        m_nLackSuit = suit;

        string[] suitStr = new string[] { "条", "万", "筒" };
        Transform tfm = m_PopupUI.Find("Icon_que");
        if (!midEnt)
        {
            GameObject obj = (GameObject)GameBase.MahjongAssetBundle.LoadAsset("Anime_Text_" + (suit + 1));
            obj = GameMain.Instantiate(obj);
            obj.transform.SetParent(m_LackStartPoint, false);
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(1f, 0.33f * time).OnComplete(() =>
            {
                obj.transform.DOMove(tfm.position, 0.67f*time).OnComplete(() =>
                {
                    tfm.gameObject.SetActive(true);
                    GameObject.Destroy(obj);
                    tfm.GetComponentInChildren<Text>().text = suitStr[suit];
                });
            });
        {
            tfm.gameObject.SetActive(true);
            tfm.GetComponentInChildren<Text>().text = suitStr[suit];

    public void ShowReward(long add, float time = 1f, int flag = -1)
    {
        if (add == 0)
            return;

        GameObject obj = (GameObject)GameBase.MahjongAssetBundle.LoadAsset("Text_Num_" + (add > 0 ? "Add" : "Subtract"));
        obj = GameMain.Instantiate(obj);
        obj.transform.SetParent(m_LackStartPoint, false);
        string text = add.ToString();
        if (add > 0)
            text = "+" + text;
        else
            PlayRoleAnim(false);
        obj.GetComponent<Text>().text = text;
        obj.transform.localPosition = new Vector3(10f, 0f, 0f);
        obj.transform.DOLocalMove(Vector3.zero, 0.2f * time);
        GameObject.Destroy(obj, time);

        //UpdateInfoUI(m_nTotalCoin + add);//update coin

        ShowAnswer(flag, time, true);
    }

    public virtual void ShowTiles(List<byte> list)
    {
    }

    public virtual Camera GetCamera()
    {
        return Camera.main;
    }

    public virtual void OnDisOrReconnect(bool bDis)
    {

    }

    public virtual bool Disconnected()
    {
        return true;
    }

    public virtual void OnMidEnt(bool bLast, byte hu, List<byte> listHand, sbyte lack, List<byte> listPong = null, 
                                List<byte> listKong = null, List<byte> listKongSelf = null, List<byte> listDiscard = null, 
                                List<byte> listHua = null, List<byte> listChi = null, List<byte> listKong2 = null, 
                                MjPlayerState_Enum ps = MjPlayerState_Enum.MjPlayerState_Init, MjTileMiddleEnterData_Enum middleEnterDataEnum = MjTileMiddleEnterData_Enum.MiddleEnterData_Normal)
    {
        if (hu != 0)//0:不胡 1：点炮胡 2：自摸
        {
            byte winTile = listHand[listHand.Count - 1];
            listHand.Remove(winTile);
            int result = 0;
            if (hu == 2)
                GameKind.AddFlag((int)MahjongHuType.MjHuType_ZiMo, ref result);
            OnWin(winTile, (uint)result, 0, true);
        }

        GameBase.OnTileNumChanged((byte)listHand.Count);

        if (middleEnterDataEnum == MjTileMiddleEnterData_Enum.MiddleEnterData_Normal)
        {
            //碰
            if (listPong != null)
            {
                foreach (byte value in listPong)
                    PongKongTile(value, 0, true);
            }
        }
        else
        {
            //碰
            if (listPong != null)
            {
                for (int index = 0; index < listPong.Count; index += 2)
                {
                    PongKongTile(listPong[index + 1], 0, true, 1.0f, GameBase.GetOtherTileDirection(m_nSit, listPong[index], 0));
                }
            }

            //吃
            if (listChi != null)
            {
                for (int index = 0; index < listChi.Count; index += 2)
                {
                    ChiTile(listChi[index], listChi[index + 1], true, 1.0f, MjTileDirection_Enum.TileDirection_Left);
                }
            }

            //补杠
            if (listKong2 != null)
            {
                for (int index = 0; index < listKong2.Count; index += 2)
                {
                    PongKongTile(listKong2[index + 1], 3, true, 1.0f, GameBase.GetOtherTileDirection(m_nSit, listKong2[index], 3));
                }
            }
        }

        //明杠
        if (listKong != null)
            foreach (byte value in listKong)
                PongKongTile(value, 1, true);
            foreach (byte value in listKongSelf)
                PongKongTile(value, 2, true, 1.0f,MjTileDirection_Enum.TileDirection_Count);

        if (listDiscard != null)
        {
            GameObject obj = null;
            foreach (byte value in listDiscard)
            {
                Transform target = m_DiscardTfm.GetChild(m_nDiscardIndex++);
                obj = GameBase.CreateTile(value, target.position, target.rotation, m_nSit);
                RemoveTileByValue(value, 1, true);
            }
            if (bLast)
                GameBase.SetCurDiscardTile(obj);
            OnAfterBuhua(listHua, new List<byte>(), true);

        if (ps == MjPlayerState_Enum.MjPlayerState_Ting)
        {
            TingState = MjTingState_Enum.MjTingState_WaitTing;
            OnTingEnd();
        }
        else if (ps == MjPlayerState_Enum.MjPlayerState_FeiTing)
        {
            TingState = MjTingState_Enum.MjTingState_WaitFeiTing;
            OnTingEnd();
        }

        if (lack >= 0)
            OnLackCofirm((byte)lack, true);

        UpdateTiles();
    }

    public void ShowZhuang()
    {
        m_PopupUI.Find("Icon_Zhuang").gameObject.SetActive(true);

    public bool IsWin()
    {
        return m_nWinIndex > 0;
    }

    public virtual bool OnAfterBuhua(List<byte> listHua, List<byte> addList = null, bool midEnt = false, float time = 1f)
    {
        if (listHua.Count == 0)
            return false;

        if (!midEnt)
            ShowAnswer("补花", time);

        int add = listHua.Count;
        if(time < 0f)
        {
            add = -add;
        }
            GameObject asset = (GameObject)GameBase.MahjongAssetBundle.LoadAsset("hua_majiang");
            GameObject obj;
            for (int i = 0; i < listHua.Count; i++)
            {
                obj = GameMain.Instantiate(asset);
                obj.transform.SetParent(m_HuaTfm, false);
                obj.transform.Find("ImageIcon").GetComponent<Image>().sprite
                    = GameBase.MahjongAssetBundle.LoadAsset<Sprite>("mahjong_" + listHua[i].ToString("X2"));
            }
        if (addList != null)
        else
        {
            m_HaveTiles.RemoveAt(0);
        }

    /// <summary>
    /// 补花以后手牌更新
    /// </summary>
    /// <param name="huaList">补花牌</param>
    /// <param name="pokerList">补花以后手牌</param>
    public virtual void UpdateMahjongHaveTiles(List<byte> huaList, List<byte> pokerList)
    {
        if (pokerList == null)
        {
            return;
        }
        foreach (byte huaTile in huaList)
        {
            m_HaveTiles.Remove(huaTile);
        }
        m_HaveTiles.AddRange(pokerList);
        UpdateTiles();
    }

    void OnClickShowHua(EventTriggerType eventtype, object message, PointerEventData eventData)
    {
        if (m_HuaTfm == null || m_HuaTfm.childCount == 0)
            return;

        if (eventtype == EventTriggerType.PointerEnter)
            m_HuaTfm.gameObject.SetActive(true);

        if (eventtype == EventTriggerType.PointerExit)
            m_HuaTfm.gameObject.SetActive(false);
    }

    public virtual void SetCanHit(bool canHit)
    {

    }

    public virtual void OnTing(MjOtherDoing_Enum doing)
    {
        TingState = MjTingState_Enum.MjTingState_WaitTing + (doing - MjOtherDoing_Enum.MjOtherDoing_Ting);
    }

    {
        if (bReverse)
        {
            m_PopupUI.Find("Text_ting").GetComponent<Text>().text = "";
        {
            TingState += (byte)MjTingState_Enum.MjTingState_TingTypeNum;

            m_PopupUI.Find("Text_ting").GetComponent<Text>().text = (TingState == MjTingState_Enum.MjTingState_Ting) ? "听" : "飞听";
            CustomAudioDataManager.GetInstance().PlayAudio(Sex * 1000 + 504);
        }
    /// <summary>
    /// 玩家喂牌达到条件所表现的效果(2次)
    /// </summary>
    /// <param name="_ms"> 正常打牌解析数据</param>
    /// <param name="actionList">录像解析数据</param>
    /// <param name="index">录像数据索引</param>
    public virtual void PlayWeiPaiShowExpression(UMessage _ms, List<int> actionList = null, int index = 0)
    {

    }
    /// <summary>
    /// 玩家喂牌达到条件所表现的效果(2次)
    /// </summary>
    /// <param name="expressionNum">玩家喂牌次数</param>
    public void PlayWeiPaiShowExpression(int expressionNum)
    {
        if (GameBase == null ||expressionNum != 2)
        {
            return;
        }

        DebugLog.Log("吃牌玩家: " + GetRoleName());
        GameBase.PlayEffect(true, "anime_chi2", 1.5f, Sex == 1 ? 1509 : 2509);
    }

    /// <summary>
    /// 更新明牌
    /// </summary>
    /// <param name="brightCardIndex">明牌在牌堆的索引</param>
    ///  <param name="brightCardValue">明牌数值</param>
    public void UpdateBrightCardTile(int brightCardIndex,byte brightCardValue)
    {
        if (brightCardIndex == -1 || m_WallTfm == null || brightCardValue == 0)
        {
            return;
        }

        Transform brightCardTransform = m_WallTfm.GetChild(0).GetChild(brightCardIndex);
        if (brightCardTransform)
        {
            if (BrightCardQuaternion == Quaternion.identity)
                BrightCardQuaternion = brightCardTransform.rotation;
            brightCardTransform.rotation = BrightCardQuaternion;
            brightCardTransform.Rotate(Vector3.right, 180);
            GameBase.SetTileMat(brightCardTransform,brightCardValue);
        }
    }

    /// <summary>
    /// 表情处理
    /// </summary>
    /// <param name="index">序号</param>
    public void OnEmotion(byte index)

        CustomAudioDataManager.GetInstance().PlayAudio(id);
        id = id %100 +24000;

        if (m_EmotiomText)
           m_EmotiomText.text = CCsvDataManager.Instance.TipsDataMgr.GetTipsText((uint)id);
        {
            m_EmotionTfm.gameObject.SetActive(true);
            GameMain.WaitForCall(3f, () => m_EmotionTfm.gameObject.SetActive(false));
        }