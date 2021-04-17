using DG.Tweening;using System.Collections;using System.Collections.Generic;using System.Linq;
using UnityEngine;using UnityEngine.EventSystems;using UnityEngine.UI;using USocket.Messages;using XLua;[Hotfix]public class Mahjong_RoleOther : Mahjong_Role{    GameObject m_ShowTileObj;    Vector3 m_vShowTilePos;    GameObject m_DisconnectObj;
    public Mahjong_RoleOther(CGame_Mahjong game, byte index)        : base(game, index)    {        m_ShowTileObj = GameObject.Find("Game_Model/zhuomian/player_" + index + "/majiang_show");        m_DisconnectObj = m_InfoUI.Find("Text_offline").gameObject;

        Camera c = Camera.main.transform.parent.Find("Camera_shoupai").GetComponent<Camera>();
        Vector3 uiPos = RectTransformUtility.WorldToScreenPoint(c, m_ShowTileObj.transform.position);
        m_vShowTilePos = Camera.main.ScreenToWorldPoint(uiPos + new Vector3(0.0f, 0.0f, Camera.main.nearClipPlane + 1f));
    }

    public override void OnQuit()
    {
        base.OnQuit();

        if (m_ShowTileObj != null)            m_ShowTileObj.SetActive(false);    }

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
        }        return true;    }

    public override void ShowChange(bool show, bool midEnt, List<byte> list)
    {
        base.ShowChange(show, midEnt, list);

        if (show)
        {
            if (list == null)
            {                if (!midEnt)
                {
                    m_HaveTiles.RemoveRange(0, GameBase.ChangeTileNum);
                    ReArrayTiles(1 + GameBase.ChangeTileNum / 2);
                }
                else
                    GameBase.OnTileNumChanged(3);
            }        }
    }

    public override void GetChangedTiles(List<byte> tiles)
    {
        if(GameBase.GameMode == GameTye_Enum.GameType_Record)
        {            m_HaveTiles.AddRange(tiles);
            UpdateTiles(-1, tiles);
        }        else
        {
            m_HaveTiles.AddRange(new byte[tiles.Count]);            ReArrayTiles();
        }    }

    public override bool OnDiscardTile(byte value, byte index, float time)
    {
        if (!base.OnDiscardTile(value, index, time))
            return false;

        GameBase.OnLeftTileChanged(value, 1, time >= 0f);
        GameBase.Desk.StartCountdown(0);

        if (time < 0f)
            return true;
        GameBase.SetTileMat(m_ShowTileObj.transform, value);        m_ShowTileObj.SetActive(true);

        Transform target = m_DiscardTfm.GetChild(m_nDiscardIndex++);
        GameObject obj = GameBase.CreateTile(value, m_vShowTilePos, Quaternion.identity, m_nSit);
        GameBase.SetCurDiscardTile(obj, false);
        obj.SetActive(false);
        GameMain.WaitForCall(time * 0.5f, () =>
        {
            m_ShowTileObj.SetActive(false);

            if(obj != null)
            {
                obj.SetActive(true);
                obj.transform.DOMove(target.position, time * 0.5f);
                obj.transform.DORotateQuaternion(target.rotation, time * 0.5f).OnComplete(() =>
                {
                    GameBase.SetDiscardTip(obj);
                    CustomAudioDataManager.GetInstance().PlayAudio(Sex * 1000 + int.Parse(value.ToString("X2")));
                });            }        });

        if (GameBase.GameMode == GameTye_Enum.GameType_Record)
        {
            m_LastFirstTiles.Add(m_HaveTiles[0]);

            m_HaveTiles.Remove(value);
            UpdateTiles(1);
            return true;
        }

        int otherIndex = TingState < MjTingState_Enum.MjTingState_Ting ? Random.Range(0, m_HaveTiles.Count) : 0;
        Mahjong_Tile tile = m_HandTfm.GetChild(otherIndex).GetComponent<Mahjong_Tile>();
        if (tile == null)
            return false;

        tile.Reset();

        m_HaveTiles.RemoveAt(otherIndex);
        InsertTile(otherIndex, 1 + Random.Range(0, m_HaveTiles.Count));
        return true;
    }
    public override void ShowTiles(List<byte> list)
    {
        if (list == null || list.Count == 0)
            return;

        if(m_nWinIndex > 0)
        {
            byte winTile = list[list.Count - 1];
            list.Remove(winTile);        }
        MahjongTileSort(ref list);
        list.Reverse();

        Transform tfm = m_HandTfm.GetChild(0);
        tfm.gameObject.SetActive(false);
        Mahjong_Tile mTile = null;
        for (int i = 0; i < list.Count; i++)
        {
            tfm = m_HandTfm.GetChild(i + 1);
            tfm.gameObject.SetActive(true);
            GameBase.SetTileMat(tfm, list[i]);
            mTile = tfm.GetComponent<Mahjong_Tile>();
            mTile.Value = list[i];
            UpdateAtomicCardTileColor(ref mTile);
        }

        for(int i = list.Count + 1; i < m_HandTfm.childCount; i++)
        {
            tfm = m_HandTfm.GetChild(i);
            tfm.gameObject.SetActive(false);
        }

        m_HandTfm.DOLocalRotate(new Vector3(90f, 0f, 0f), 0.3f);

        base.ShowTiles(list);
    }

    protected override void RemoveTileByValue(byte value, int num, bool midEnt, float time = 1f)
    {
        if (!midEnt)
        {            if(GameBase.GameMode == GameTye_Enum.GameType_Record)
            {
                if(time > 0f)
                {
                    m_LastFirstTiles.Add(m_HaveTiles[0]);

                    for (int i = 0; i < num; i++)
                        m_HaveTiles.Remove(value);                }                else
                {
                    for (int i = 0; i < num; i++)
                        m_HaveTiles.Add(value);
                }            }            else                m_HaveTiles.RemoveRange(0, num);
        }        else
            GameBase.OnTileNumChanged((byte)num);


        GameBase.OnLeftTileChanged(value, (byte)num, time > 0f);
    }

    protected override void RemoveTileByValue(List<byte> valueList, bool midEnt, float time = 1f)
    {
        if (!midEnt)
        {            if (GameBase.GameMode == GameTye_Enum.GameType_Record)
            {
                if (time > 0f)
                {
                    m_LastFirstTiles.Add(m_HaveTiles[0]);

                    foreach (byte tileValue in valueList)
                        m_HaveTiles.Remove(tileValue);                }                else
                {
                    m_HaveTiles.AddRange(valueList);
                }            }            else                m_HaveTiles.RemoveRange(0, valueList.Count);
        }        else
            GameBase.OnTileNumChanged((byte)valueList.Count);

        foreach (byte tileValue in valueList)
        {
            GameBase.OnLeftTileChanged(tileValue, 1, time > 0f);
        }
    }    public override void OnWin(byte tile, uint result, uint addCoin, bool end, float time)
    {
        base.OnWin(tile, result, addCoin, end, time);

        if(GameKind.HasFlag((int)MahjongHuType.MjHuType_ZiMo, result))//自摸
            GameBase.OnLeftTileChanged(tile, 1, time > 0f);
    }

    public override void OnDisOrReconnect(bool bDis)
    {
        base.OnDisOrReconnect(bDis);

        if(m_DisconnectObj != null)
            m_DisconnectObj.SetActive(bDis);
    }

    public override bool Disconnected()
    {
        if (m_DisconnectObj == null)
            return true;

        return m_DisconnectObj.activeSelf;
    }

    public override void OnMidEnt(bool bLast, byte hu, List<byte> listHand, sbyte lack, List<byte> listPong = null,
                                List<byte> listKong = null, List<byte> listKongSelf = null, List<byte> listDiscard = null,
                                List<byte> listHua = null, List<byte> listChi = null, List<byte> listKong2 = null,
                                MjPlayerState_Enum ps = MjPlayerState_Enum.MjPlayerState_Init, MjTileMiddleEnterData_Enum middleEnterDataEnum = MjTileMiddleEnterData_Enum.MiddleEnterData_Normal)
    {
        base.OnMidEnt(bLast, hu, listHand, lack, listPong, listKong, listKongSelf, listDiscard, listHua, listChi, listKong2, ps, middleEnterDataEnum);

        if (GameBase.GameMode == GameTye_Enum.GameType_Record)
        {
            m_HandTfm.localEulerAngles = new Vector3(90f, 0f, 0f);
        }
    }

    public override void OnLackCofirm(byte suit, bool midEnt, float time)
    {
        base.OnLackCofirm(suit, midEnt, time);

        if (GameBase.GameMode == GameTye_Enum.GameType_Record)
            UpdateTiles();
    }    public override bool OnAfterBuhua(List<byte> listHua, List<byte> addList, bool midEnt, float time)
    {
        if (!base.OnAfterBuhua(listHua, addList, midEnt, time))
            return false;

        for (int i = 0; i < listHua.Count; i++)
        {
            GameBase.OnLeftTileChanged(listHua[i], 1, time > 0f);
        }
        return true;
    }}