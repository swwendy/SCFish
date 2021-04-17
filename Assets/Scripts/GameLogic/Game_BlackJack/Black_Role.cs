using System;using System.Collections.Generic;using System.Linq;using System.Text;using UnityEngine;using UnityEngine.UI;using XLua;[Hotfix]public class Black_Role{    public uint m_nUserId;    public uint m_nFaceId;    public string m_nUrl;    public long m_nTotalCoin;    public string m_cRoleName;    public List<byte>[] m_vecBlackPoker;    public GameObject m_InfoUIObj = null;    protected CGame_BlackJack m_BjGameBase;

    protected GameObject[] m_CountTip;    public Black_Role(CGame_BlackJack bjGame, Transform uiParent)    {        m_BjGameBase = bjGame;        Init();    }    public virtual void Init()    {        OnEnd();        m_nUserId = 0;        m_nFaceId = 0;        m_nUrl = "";        m_nTotalCoin = 0;        m_cRoleName = "";        ShowPlayerInfo(false);    }    public virtual void OnTick()    {    }    public virtual void OnQuitTurn(byte state = 0)    {    }    public virtual void OnTurn(float fill = 1.0f, bool bFirst = false)    {    }    protected virtual bool ShowPlayerInfo(bool show = true, bool invite = false)    {        if (m_InfoUIObj == null)            return false;        m_InfoUIObj.SetActive(show);        return true;    }    public virtual void UpdateInfoUI()    {    }    public virtual void OnEnd()    {        if (m_vecBlackPoker != null)        {            foreach (List<byte> vec in m_vecBlackPoker)                vec.Clear();        }    }    public virtual void HandleAddPoker(byte pos, byte card, byte points, byte otherPoints)    {    }    public virtual void SetPointCount(byte count, byte other, byte pos = 0)    {    }    public virtual void SetSplit(byte split)    {    }    protected void AddPoker(byte card, Transform parent)    {        string mat = GameCommon.GetPokerMat(card);        //DebugLog.LogWarning("userid:" + m_nUserId + " card mat:" + mat);                //UnityEngine.Object obj = (GameObject)m_BjGameBase.m_AssetBundle.LoadAsset("Model_Pai");        //GameObject gameObj = GameMain.instantiate(obj) as GameObject;        //gameObj.transform.SetParent(parent, false);        //gameObj.transform.GetComponentInChildren<MeshRenderer>().material =        //    m_BjGameBase.m_AssetBundle.LoadAsset<Material>(mat);        parent.gameObject.SetActive(true);        parent.GetComponent<MeshRenderer>().material = m_BjGameBase.m_AssetBundle.LoadAsset<Material>(mat);        CustomAudioDataManager.GetInstance().PlayAudio(1005);    }    protected void AddChips(Transform parent, long chips)    {        for (int i = parent.childCount - 1; i >= 0; i--)
            GameObject.DestroyImmediate(parent.GetChild(i).gameObject);
        BetData bd = BlackJack_Data.GetInstance().m_BetData[m_BjGameBase.m_RoomInfo.m_nRoomLevel - 1];        int[] grades = bd.m_nGrades;        for (int i = grades.Length - 1; i >= 0; i--)        {            if(chips >= grades[i] || i == 0)            {
                int index = i + (chips > grades[i] ? 2 : 1);
                UnityEngine.Object obj = (GameObject)m_BjGameBase.m_AssetBundle.LoadAsset("Model_chouma" + index);
                GameObject gameObj = GameMain.instantiate(obj) as GameObject;
                gameObj.transform.SetParent(parent, false);
                gameObj.GetComponentInChildren<TextMesh>().text = GameFunction.FormatCoinText(chips);

                break;            }        }        CustomAudioDataManager.GetInstance().PlayAudio(1003);    }

    public void ShowCountTip(bool bShow, byte pos = 0, Transform parent = null, byte count = 0, byte other = 0)    {
        if (m_CountTip == null || pos > m_CountTip.Length)
            return;

        int index = pos;
        if (pos > 0)
            index--;

        GameObject CountTip = m_CountTip[index];

        if (CountTip == null)
            return;

        CountTip.SetActive(false);        if (bShow && count != 0)        {            CountTip.SetActive(true);            CountTip.transform.SetParent(parent, false);            TextMesh tm = CountTip.GetComponentInChildren<TextMesh>();            GameObject bg1 = CountTip.transform.Find("mesh_BG_1").gameObject;            GameObject bg2 = CountTip.transform.Find("mesh_BG_2").gameObject;            if(count == RoomInfo.NoSit)
            {
                tm.text = "投降";                tm.color = new Color(1.0f, 0.27f, 0.0f);                bg1.SetActive(false);                bg2.SetActive(true);            }            else if (count > 21)            {                tm.text = "爆牌";                tm.color = new Color(1.0f, 0.27f, 0.0f);                bg1.SetActive(false);                bg2.SetActive(true);            }            else            {                if(count == 21 || other == 21)
                {
                    if(pos == 0 && m_vecBlackPoker[pos].Count == 2)
                    {
                        tm.text = "黑杰克";
                        bg1.SetActive(false);
                        bg2.SetActive(true);                    }
                    else if (m_vecBlackPoker[index].Count == 5)
                    {
                        tm.text = "五龙";
                        bg1.SetActive(false);
                        bg2.SetActive(true);
                    }                    else
                    {
                        tm.text = "21";
                        bg1.SetActive(true);
                        bg2.SetActive(false);                    }
                }                else if(m_vecBlackPoker[index].Count == 5)
                {
                    tm.text = "五龙";
                    bg1.SetActive(false);
                    bg2.SetActive(true);                }                else if (count != other && other < 21)//have Ace
                {                    tm.text = count.ToString() + "/" + other.ToString();                    bg1.SetActive(false);                    bg2.SetActive(true);                }                else                {                    tm.text = count.ToString();                    bg1.SetActive(true);                    bg2.SetActive(false);                }                tm.color = Color.white;            }        }    }}