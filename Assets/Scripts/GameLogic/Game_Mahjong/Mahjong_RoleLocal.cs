using DG.Tweening;using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class Mahjong_RoleLocal : Mahjong_Role{    Camera m_LocalCamera;    protected bool m_bCanHit = true;    Dictionary<byte, Dictionary<byte, ushort>> m_dictValueWinSet = null;    public Mahjong_RoleLocal(CGame_Mahjong game, byte index)        : base(game, index)    {        m_LocalCamera = GameObject.Find("Camera/Camera_shoupai").GetComponent<Camera>();
    }    public virtual void InitMsgHandle()
    {
    }    public override void Init()
    {
        base.Init();

        InitMsgHandle();
    }    public override void OnTick()    {
        if (GameBase.GameMode == GameTye_Enum.GameType_Record)
            return;

        if (GameBase.Bystander)
            return;

        if (m_bCanHit && m_bTileMoveEnd && Input.GetMouseButtonDown(0))
        {
            Ray ray = m_LocalCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit, 500f, CGame_Mahjong.LocalHandTileLayer))
            {
                Mahjong_Tile tile = rayhit.collider.gameObject.GetComponent<Mahjong_Tile>();
                OnHitTile(tile);
            }
            else if (!GameFunction.IsPointerOnUI())
                OnHitTile(null);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        m_bCanHit = true;
        m_dictValueWinSet = null;
    }

    public override bool SendTiles(List<byte> list, int showedCount)
    {
        if (!base.SendTiles(list, showedCount))
            return false;

        int i = m_HandTfm.childCount - showedCount - 1;
        if (i < 0)
            return false;

        Transform tfm;
        for (int j = 0; j < list.Count; j++, i--)
        {
            if (i < 0)
                break;

            tfm = m_HandTfm.GetChild(i);
            tfm.gameObject.SetActive(true);

            Mahjong_Tile tile = tfm.GetComponent<Mahjong_Tile>();
            tile.Value = list[j];

            GameBase.SetTileMat(tfm, list[j], GameBase.Bystander);

            tfm.Rotate(-90f, 0f, 0f);
            tfm.DOLocalRotate(Vector3.zero, 0.3f);
            GameBase.OnLeftTileChanged(list[j], 1);
        }        return true;    }    public override bool DealTile(byte tileValue, bool show, bool cancel)
    {
        if (!base.DealTile(tileValue, show, cancel))
            return false;

        GameBase.OnLeftTileChanged(tileValue, 1, !cancel);

        CheckTing();

        //DebugLog.Log("Local deal tile:" + tileValue);

        return true;
    }    public virtual void SortTiles(float rotTime)
    {
        m_HandTfm.DOLocalRotate(new Vector3(-90f, 0f, 0f), rotTime).OnComplete(() =>        {
            MahjongTileSort(ref m_HaveTiles);
            Transform tfm = null;
            Mahjong_Tile mTile = null;
            for (int i = 0, j = m_HandTfm.childCount - 1; i < m_HaveTiles.Count; i++, j--)
            {
                tfm = m_HandTfm.GetChild(j);
                GameBase.SetTileMat(tfm, m_HaveTiles[i], GameBase.Bystander);
                mTile = tfm.GetComponent<Mahjong_Tile>();
                mTile.Value = m_HaveTiles[i];
                UpdateAtomicCardTileColor(ref mTile);
            }

            m_HandTfm.DOLocalRotate(Vector3.zero, rotTime);

            m_HaveTiles.Reverse();        });
    }    protected virtual void OnHitTile(Mahjong_Tile tile)
    {
        if (tile == null)
        {
            DownAllTiles(false, true);
            return;
        }
        foreach (Transform tfm in m_HandTfm)
        {
            if (!tfm.gameObject.activeSelf)
                continue;

            Mahjong_Tile mt = tfm.GetComponent<Mahjong_Tile>();
            mt.OnSelect(mt == tile, true, TingState < MjTingState_Enum.MjTingState_Ting && OnTurn && GameBase.GameState > MahjongRoomState_Enum.MjRoomState_MakeLackCartoon);
        }        if (tile.m_bSelected)
        {
            if (m_dictValueWinSet != null && m_dictValueWinSet.ContainsKey(tile.Value))
                GameBase.ShowTingUI(true, m_dictValueWinSet[tile.Value]);
            else
                GameBase.ShowTingUI(false);

            GameBase.ShowDiscardedTile(tile.Value);
        }        else            GameBase.ShowDiscardedTile(RoomInfo.NoSit);    }

    protected void SendDiscardTileMsg(Mahjong_Tile tile, byte disValue, GameCity.EMSG_ENUM msgType)
    {
        int index = tile.transform.GetSiblingIndex();

        UMessage msg = new UMessage((uint)msgType);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(disValue);
        msg.Add((byte)index);
        HallMain.SendMsgToRoomSer(msg);

        SetCanHit(false);

        if (GameBase != null)
        {
            GameBase.SetTrustButtonActive(false);
        }
    }

    protected bool HandleBackDiscard(uint _msgType, UMessage _ms)
    {
        m_bCanHit = true;

        byte nState = _ms.ReadByte();
        byte nTile = _ms.ReadByte();
        byte index = _ms.ReadByte();

        //DebugLog.Log("local discard state:" + nState + " tile:" + nTile + " index:" + index);


        if(nState == 0)//0：成功 1：不在出牌状态 2：不是你出牌 3:花牌 11：没有这张牌 12 手上有定缺的牌 但是打出来的缺不是定缺的牌
        {
            GameBase.ShowGameButton(0);
            GameBase.Desk.StartCountdown(0);
            if (OnDiscardTile(nTile, index, 1f))
            {
                if (IsTing() && m_dictValueWinSet.ContainsKey(nTile))
                {                    Dictionary<byte, ushort> valueSet = new Dictionary<byte, ushort>(m_dictValueWinSet[nTile]);                    GameBase.ShowTingUI(false, valueSet);
                    GameBase.ShowTingBtn(true);
                    m_dictValueWinSet.Clear();
                    m_dictValueWinSet[0] = valueSet;
                }                else if(m_dictValueWinSet != null && TingState == MjTingState_Enum.MjTingState_Init)
                {                    GameBase.ShowTingUI(false);
                    GameBase.ShowTingBtn(false);
                    m_dictValueWinSet = null;
                }
                DownAllTiles(true);

                OnTurn = false;
            }
        }
        else
        {
            DownAllTiles(false);

            if (nState == 12)
                CRollTextUI.Instance.AddVerticalRollText(2602);            else if(nState == 3)
            {
                CRollTextUI.Instance.AddVerticalRollText(2603);
            }            else                CRollTextUI.Instance.AddVerticalRollText(2601);        }

        GameBase.ShowDiscardedTile(RoomInfo.NoSit);

        return true;
    }

    public override bool OnDiscardTile(byte value, byte index, float time)
    {
        if(!base.OnDiscardTile(value, index, time))
            return false;

        if(time < 0f)
        {
            CheckTing();
            return true;
        }

        if(GameBase.Bystander)
            index = (byte)Random.Range(0, m_HaveTiles.Count);
        else if (value != m_HaveTiles[index])//非常情况
            index = (byte)m_HaveTiles.FindIndex(s => s == value);

        if (index >= m_HandTfm.childCount)
        {            DebugLog.LogWarning("discard tile error index:" + index + " value:" + value);            return false;
        }
        Mahjong_Tile tile = m_HandTfm.GetChild(index).GetComponent<Mahjong_Tile>();
        if (tile == null)
            return false;

        tile.Reset();

        GameObject obj = GameBase.CreateTile(value, tile.transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity, m_nSit);
        GameBase.SetCurDiscardTile(obj, false);
        Transform target = m_DiscardTfm.GetChild(m_nDiscardIndex++);
        obj.transform.DOMove(target.position, 0.5f * time);
        obj.transform.DORotateQuaternion(target.rotation, 0.5f * time).OnComplete(() =>
        {
            GameBase.SetDiscardTip(obj);
            CustomAudioDataManager.GetInstance().PlayAudio(Sex * 1000 + int.Parse(value.ToString("X2")));
        });

        if (GameBase.GameMode == GameTye_Enum.GameType_Record)
        {
            m_LastFirstTiles.Add(m_HaveTiles[0]);

            m_HaveTiles.Remove(value);
            UpdateTiles(1);

            DownAllTiles(true);
            return true;
        }

        m_HaveTiles.RemoveAt(index);
        if (index == 0)
            return true;

        int i = 1;
        if (GameBase.Bystander)
            i = Random.Range(1, m_HaveTiles.Count);
        else
        {
            byte inValue = m_HaveTiles[0];
            int inSuit = JudgeTile.GetTileSuit(inValue);
            bool bInLack = (inSuit == m_nLackSuit);
            int suit;
            for (; i < m_HaveTiles.Count - GetAtomicTileCount(); i++)
            {
                suit = JudgeTile.GetTileSuit(m_HaveTiles[i]);
                if (suit != inSuit)
                {
                    if (bInLack)
                        break;

                    if (suit == m_nLackSuit)
                        continue;
                }

                if (m_HaveTiles[i] <= inValue)
                    break;
            }
            i = GetInsertAtomicTileIndex(index, i);
            m_HaveTiles.Insert(i, inValue);
            m_HaveTiles.RemoveAt(0);
        }
        InsertTile(index, i);

        return true;
    }

    /// <summary>
    /// 获取原子牌的数量
    /// </summary>
    protected virtual int GetAtomicTileCount()
    {
        return 0;
    }

    /// <summary>
    /// 获取抓取牌插入的索引位置
    /// </summary>
    /// <param name="dealIndex">出牌的索引位置</param>
    /// <param name="targetIndex">抓取牌插入的索引位置</param>
    /// <returns></returns>
    protected virtual int GetInsertAtomicTileIndex(int dealIndex, int targetIndex)
    {
        return targetIndex;
    }


    protected override void RemoveTileByValue(byte value, int num, bool midEnt, float time = 1f)
    {
        if(midEnt)
        {
            byte change = (byte)num;
            GameBase.OnTileNumChanged(change);
            GameBase.OnLeftTileChanged(value, change);
            return;
        }

        if(GameBase.Bystander)
        {
            m_HaveTiles.RemoveRange(0, num);
            return;
        }

        if(time < 0f)
        {
            for (int i = 0; i < num; i++)
                m_HaveTiles.Add(value);
            return;
        }

        m_LastFirstTiles.Add(m_HaveTiles[0]);

        List<byte> list = m_HaveTiles.FindAll(s => s == value);
        if (list.Count < num)
        {            DebugLog.LogError("!!!Wrong Pong:" + value + " localCount:" + list.Count);
            return;
        }

        for (int i = 0; i < num; i++)
        {
            m_HaveTiles.Remove(value);
        }

    }

    protected override void RemoveTileByValue(List<byte> valueList, bool midEnt, float time = 1f)
    {
        if (midEnt)
        {
            GameBase.OnTileNumChanged(valueList.Count);
            foreach(byte tile in valueList)
            {
                GameBase.OnLeftTileChanged(tile, 1);
            }
            return;
        }

        if (GameBase.Bystander)
        {
            m_HaveTiles.RemoveRange(0, valueList.Count);
            return;
        }

        if (time < 0f)
        {
            m_HaveTiles.AddRange(valueList);
            return;
        }

        m_LastFirstTiles.Add(m_HaveTiles[0]);

        foreach (byte tile in valueList)
        {
            m_HaveTiles.Remove(tile);
        }
    }

    public int GetRecommendSuit(int min, out List<byte> outlist)
    {
        List<List<byte>> listSuit = new List<List<byte>>();
        List<int> minIndexList = new List<int>();
        int minNum = 14;
        for (int i = 0; i < 3; i++)
        {
            List<byte> list = m_HaveTiles.FindAll(s => JudgeTile.GetTileSuit(s) == i);
            listSuit.Add(list);

            if (list.Count >= min)
            {                if(m_nLackSuit == i)
                {
                    minIndexList.Clear();
                    minIndexList.Add(i);
                    break;
                }                if (list.Count == minNum)
                {
                    minIndexList.Add(i);
                }                else if (list.Count < minNum)
                {                    minNum = list.Count;
                    minIndexList.Clear();
                    minIndexList.Add(i);
                }            }        }

        int resultIndex = 0;
        if (minIndexList.Count == 1)
            resultIndex = minIndexList[0];
        else
        {
            int maxSameNum = 0;
            foreach (byte index in minIndexList)
            {
                HashSet<byte> rset = new HashSet<byte>(listSuit[index]);
                if (rset.Count > maxSameNum)
                {
                    maxSameNum = rset.Count;
                    resultIndex = index;
                }
            }        }
        outlist = new List<byte>(listSuit[resultIndex]);

        return resultIndex;
    }

    public List<byte> GetRecommondTile(int num)
    {
        List<byte> readyList;
        int resultIndex = GetRecommendSuit(num, out readyList);

        List<byte> result = new List<byte>();
        if (readyList.Count == num)
        {
            result = new List<byte>(readyList);
        }
        else
        {
            readyList.Reverse();

            int nAdd = num;
            for (int i = 1; i <= 4; i++)
            {
                var group = readyList.GroupBy(p => p).Where(g => g.Count() == i);
                if (group.Count() <= nAdd)
                {
                    foreach (var g in group)
                    {                        result.Add(g.Key);
                        nAdd--;
                    }                }
                else
                {
                    foreach (var g in group)
                    {
                        byte value = g.Key;
                        if (readyList.Contains((byte)(value + 1)) && readyList.Contains((byte)(value + 2)))
                            continue;
                        if (readyList.Contains((byte)(value - 1)) && readyList.Contains((byte)(value + 1)))
                            continue;
                        if (readyList.Contains((byte)(value - 1)) && readyList.Contains((byte)(value - 2)))
                            continue;

                        result.Add(value);
                        nAdd--;

                        if (nAdd == 0)
                            break;
                    }
                }

                if (nAdd == 0)
                    break;            }

            if(nAdd > 0)
            {
                var group = readyList.GroupBy(p => p).Where(g => g.Count() > 1);
                foreach (var g in group)
                {
                    result.Add(g.Key);
                    nAdd--;

                    if (nAdd == 0)
                        break;                }            }

            if (nAdd > 0)
            {
                foreach (byte t in result)
                {
                    readyList.Remove(t);
                }

                readyList.RemoveRange(nAdd, readyList.Count - nAdd);
                result.AddRange(readyList);
            }
        }

        result.Sort();

        //string temp = "choose:";
        //foreach (byte tile in result)
        //{
        //    temp += tile.ToString("X2") + " ";
        //}
        //DebugLog.Log(temp);

        return result;
    }

    public override void GetChangedTiles(List<byte> tiles)
    {
        m_bCanHit = true;

        foreach (byte value in tiles)
            GameBase.OnLeftTileChanged(value, 1);
        m_HaveTiles.AddRange(tiles);
        UpdateTiles(-1, tiles);
    }

    public override void OnLackCofirm(byte suit, bool midEnt, float time)
    {
        base.OnLackCofirm(suit, midEnt, time);

        UpdateTiles();

        CheckTing();
        if (!midEnt)
            CustomAudioDataManager.GetInstance().PlayAudio(3005);
    }

    public virtual bool CheckTing(bool bForce = false,byte suitCount = 3,byte atomicCardValue = 0, bool check7pair = true)
    {
        if (GameBase.GameMode == GameTye_Enum.GameType_Record || GameBase.Bystander)
            return false;

        if (bForce ^ (TingState != MjTingState_Enum.MjTingState_Init))
            return false;

        Dictionary<byte, HashSet<byte>> tingDict;
        if (JudgeTile.CheckPrepareTing(m_HaveTiles, m_nLackSuit, out tingDict, check7pair, suitCount, atomicCardValue))
        {
            byte cur = RoomInfo.NoSit;
            ushort tempCurProValue = 0;
            ushort curPro = GameBase.m_ProInfo.m_iMaxPro;
            if(IsTing())
            {                Dictionary<byte, ushort> tingTiles = m_dictValueWinSet.FirstOrDefault().Value;
                //update cur pro
                if(TingState == MjTingState_Enum.MjTingState_Init)
                {
                    HashSet<byte> curTiles = new HashSet<byte>(tingTiles.Keys);
                    foreach (byte curTile in curTiles)
                    {
                        foreach (var tile in tingDict.Values)
                        {
                            if (tile.Contains(curTile))
                            {
                                List<byte> tiles = new List<byte>(m_HaveTiles);
                                tiles.RemoveAt(0);
                                tiles.Add(curTile);
                                tempCurProValue = GetTileWinPro(tiles);
                                if (tempCurProValue != 0)
                                {
                                    tingTiles[curTile] = tempCurProValue;
                                }
                                else
                                {
                                    tingTiles.Remove(curTile);
                                }
                                break;
                            }
                        }
                    }                }                cur = GameBase.GetTileRemainNum(tingTiles, out curPro);
            }

            m_dictValueWinSet = new Dictionary<byte, Dictionary<byte, ushort>>();
            foreach(var ting in tingDict)
            {
                Dictionary<byte, ushort> valueSet = new Dictionary<byte, ushort>();                foreach(byte tile in ting.Value)
                {
                    List<byte> tiles = new List<byte>(m_HaveTiles);
                    tiles.Remove(ting.Key);
                    tiles.Add(tile);
                    tempCurProValue = GetTileWinPro(tiles);
                    if(tempCurProValue != 0)
                    {
                        valueSet[tile] = tempCurProValue;
                    }
                }                if(valueSet.Count != 0)                    m_dictValueWinSet[ting.Key] = valueSet;
            }            if (TingState >= MjTingState_Enum.MjTingState_Ting)                return true;
            byte max = 0, tingNum = 0;
            ushort maxPro = 1;
            List<Mahjong_Tile> listPro = new List<Mahjong_Tile>();
            List<Mahjong_Tile> listMax = new List<Mahjong_Tile>();
            foreach (Transform tfm in m_HandTfm)
            {
                if (!tfm.gameObject.activeSelf)
                    continue;

                Mahjong_Tile mt = tfm.GetComponent<Mahjong_Tile>();
                if (mt == null)
                    continue;

                if(m_dictValueWinSet.ContainsKey(mt.Value))
                {
                    ushort pro;
                    byte total = GameBase.GetTileRemainNum(m_dictValueWinSet[mt.Value], out pro);
                    int tipType = 1;
                    if (pro > curPro)
                        tipType = 3;
                    else if (total > cur)
                        tipType = 2;
                    mt.ShowTip(true, tipType);                    if(pro > maxPro)
                    {
                        maxPro = pro;
                        listPro.Clear();
                        listPro.Add(mt);
                    }                    else if(pro == maxPro)
                        listPro.Add(mt);

                    if (total > max)
                    {                        max = total;
                        listMax.Clear();
                        listMax.Add(mt);
                    }                    else if (total == max)                        listMax.Add(mt);                    tingNum++;                }            }
            if (cur == RoomInfo.NoSit)
            {
                //去除listMax中与listPro重复的元素
                listMax.RemoveAll(s => listPro.Contains(s));

                if(listMax.Count != tingNum)
                    foreach (Mahjong_Tile mt in listMax)
                        mt.ShowTip(true, 2);
                if(listPro.Count != tingNum)
                    foreach (Mahjong_Tile mt in listPro)
                        mt.ShowTip(true, 3);
            }            return true;        }

        foreach (Transform tfm in m_HandTfm)
        {
            if (!tfm.gameObject.activeSelf)
                continue;

            Mahjong_Tile mt = tfm.GetComponent<Mahjong_Tile>();
            if (mt == null)
                continue;

            mt.ShowTip(false);
        }

        return false;
    }

    public override Camera GetCamera()
    {
        return m_LocalCamera;
    }

    public override PongKongType PongKongTile(byte value, byte kongType, bool midEnt, float time, MjTileDirection_Enum direction = MjTileDirection_Enum.TileDirection_Front)
    {
        PongKongType type = base.PongKongTile(value, kongType, midEnt, time,direction);

        CheckTing();

        return type;
    }


    public override PongKongType ChiTile(byte value, byte firstValue, bool midEnt = false, float time = 1f, MjTileDirection_Enum direction = MjTileDirection_Enum.TileDirection_Front)
    {
        PongKongType type = base.ChiTile(value, firstValue, midEnt, time, direction);

        CheckTing();

        return type;
    }

    public override void OnWin(byte tile, uint result, uint addCoin, bool end, float time)
    {
        base.OnWin(tile, result, addCoin, end, time);

        m_bCanHit = false;

        DownAllTiles(true);
    }

    public override void ShowTiles(List<byte> list)
    {
        base.ShowTiles(list);

        if (GameBase.Bystander)
        {
            if(IsWin())
            {
                list.RemoveAt(0);
            }
            m_HaveTiles = new List<byte>(list);
            m_HaveTiles.Sort();
            m_HaveTiles.Reverse();
            ReArrayTiles(1, null, true);
        }

        DownAllTiles(true);
        m_bCanHit = false;
        m_HandTfm.DOLocalRotate(new Vector3(75f, 0f, 0f), 0.3f);
    }

    public override void OnMidEnt(bool bLast, byte hu, List<byte> listHand, sbyte lack, List<byte> listPong = null,
                                List<byte> listKong = null, List<byte> listKongSelf = null, List<byte> listDiscard = null,
                                List<byte> listHua = null, List<byte> listChi = null, List<byte> listKong2 = null,
                                MjPlayerState_Enum ps = MjPlayerState_Enum.MjPlayerState_Init, MjTileMiddleEnterData_Enum middleEnterDataEnum = MjTileMiddleEnterData_Enum.MiddleEnterData_Normal)
    {
        base.OnMidEnt(bLast, hu, listHand, lack, listPong, listKong, listKongSelf, listDiscard, listHua, listChi, listKong2, ps, middleEnterDataEnum);

        foreach (byte tile in m_HaveTiles)
            GameBase.OnLeftTileChanged(tile, 1);

        if (OnTurn)
        {
            GameBase.ShowTingBtn(CheckTing());
        }
    }

    protected override void ArrayTileOnPongOrKong(byte value, PongKongType type, bool midEnt, float time = 1f, MjTileDirection_Enum direction = MjTileDirection_Enum.TileDirection_Front)
    {
        if(time > 0f)
        {
            if (type == PongKongType.ePKT_Kong_Concealed || type == PongKongType.ePKT_Pong2Kong)//有摸来的牌，重新排序
            {
                MahjongTileSort(ref m_HaveTiles);
                List<byte> list = m_HaveTiles.FindAll(s => JudgeTile.GetTileSuit(s) == m_nLackSuit);
                foreach (byte tile in list)
                    m_HaveTiles.Remove(tile);
                m_HaveTiles.AddRange(list);
                m_HaveTiles.Reverse();
            }        }
        base.ArrayTileOnPongOrKong(value, type, midEnt, time,direction);
    }

    protected void DownAllTiles(bool hideTip, bool showMove = false)
    {
        foreach (Transform tfm in m_HandTfm)
        {
            Mahjong_Tile tile = tfm.GetComponent<Mahjong_Tile>();
            if (tile == null)
                continue;

            tile.OnSelect(false, showMove);

            if (hideTip)
                tile.ShowTip(false);
        }

        GameBase.ShowDiscardedTile(RoomInfo.NoSit);    }

    public override void SetCanHit(bool canHit)
    {
        m_bCanHit = canHit;
    }
    public virtual ushort GetTileWinPro(List<byte> haveTiles)
    {
        return 0;
    }    public virtual bool IsTing()
    {
        return m_dictValueWinSet != null && m_dictValueWinSet.Count != 0;
    }

    public override bool OnAfterBuhua(List<byte> listHua, List<byte> addList, bool midEnt, float time)
    {
        if(!base.OnAfterBuhua(listHua, addList, midEnt, time))
            return false;

        if (addList != null)
        {            List<byte> haveHua = m_HaveTiles.FindAll(s => s > 0x30);            List<byte> change = new List<byte>(listHua);            foreach (byte tile in haveHua)
            {
                change.Remove(tile);
            }
            change.AddRange(addList);
            foreach (byte tile in change)
            {
                GameBase.OnLeftTileChanged(tile, 1, time > 0f);
            }
        }        return true;    }}