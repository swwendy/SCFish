using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using XLua;using UnityEngine.UI;[Hotfix]public class Mahjong_Tile : MonoBehaviour
{
    public Mahjong_Role OwnerRole = null;
    public byte Value { get; set; }

    public bool m_bSelected = false;
    bool m_bMoving = false;
    bool m_bLack = false;
    float m_fSrcLocalPosY;

    GameObject m_TipObj = null;
    public bool Tiped()
    {
        return m_TipObj != null && m_TipObj.activeSelf;
    }

    // Use this for initialization
    void Awake ()
    {
        m_fSrcLocalPosY = transform.localPosition.y;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_TipObj != null && m_TipObj.activeSelf)
        {
            Vector3 uiPos = GameFunction.WorldToLocalPointInRectangle(transform.Find("Effectpoint").position
                , OwnerRole.GetCamera(), OwnerRole.GameBase.GameCanvas, OwnerRole.GameBase.GameCanvas.worldCamera);
            m_TipObj.transform.localPosition = uiPos;
        }
    }

    private void OnDestroy()
    {
        if (m_TipObj != null)
            Destroy(m_TipObj);
    }

    public bool Lack {
        get
        {
            return m_bLack;
        }
        set
        {
            m_bLack = value;
            Color color = m_bLack ? Color.gray : Color.white;
            OwnerRole.GameBase.ShowTileColor(transform, color);
        }
    }

    public bool Enable { get; set; }

    public void OnSelect(bool bSel, bool bShowMove = true, bool bDiscard = false)
    {
        if (!Enable)
            return;

        bool bMove = false;
        float offset = 0.3f;

        if (!bShowMove)
        {
            m_bSelected = bSel;
            Vector3 pos = transform.localPosition;
            pos.y = m_fSrcLocalPosY;
            if (m_bSelected)
                pos.y += offset;
            transform.localPosition = pos;
            m_bMoving = false;
        }
        else if (bSel)
        {
            if(bDiscard && m_bSelected)//要打出去了
            {
                OwnerRole.DiscardTile(this, Value);
                m_bSelected = false;
            }
            else if(!m_bMoving)
            {
                m_bSelected = !m_bSelected;
                bMove = true;
            }
        }
        else
        {
            bMove = (m_bSelected != bSel) && !m_bMoving;
            m_bSelected = false;
        }

        if(bMove)
        {
            float y = m_fSrcLocalPosY;
            if (m_bSelected)
            {
                y += offset;
            }

            m_bMoving = true;
            transform.DOLocalMoveY(y, 0.1f).OnComplete(() => m_bMoving = false);
        }
    }

    public void Reset()
    {
        Enable = true;
        Value = 0;
        m_bSelected = false;
        m_bMoving = false;
        transform.localPosition = new Vector3(transform.localPosition.x, m_fSrcLocalPosY, transform.localPosition.z);
        gameObject.SetActive(false);
        Lack = false;
        ShowTip(false);

        if (m_TipObj != null)
            m_TipObj.SetActive(false);
    }

    public void ShowTip(bool show, int type = 1)
    {
        if (!show && m_TipObj == null)
            return;

        if (m_TipObj == null)
        {
            m_TipObj = (GameObject)OwnerRole.GameBase.MahjongAssetBundle.LoadAsset("UItips_ting");
            m_TipObj = (GameObject)Instantiate(m_TipObj);
            Transform root = OwnerRole.GameBase.GameCanvas.transform.Find("Root");            m_TipObj.transform.SetParent(root, false);
        }

        if(show)
        {
            m_TipObj.transform.GetComponentInChildren<Image>().sprite =
                OwnerRole.GameBase.MahjongAssetBundle.LoadAsset<Sprite>("icon_ting" + type);
        }

        m_TipObj.SetActive(show);
    }
}
