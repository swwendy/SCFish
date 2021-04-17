﻿using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
                if (m_TimeImageDict[img].hideObjOnRemove != null)
                    m_TimeImageDict[img].hideObjOnRemove.SetActive(false);

                m_TimeImageDict.Remove(img);

                if (info.text)
                    info.text.text = "0";

                bool bRemove = true;

                if (bRemove)
                    m_PreRemoveImgList.Add(img);
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(m_nGameId);
        {
        }
    {
        if (isBGM)
            AudioManager.Instance.StopMusic();
    {
        if (m_SoundList.ContainsKey(id))
            return m_SoundList[id];

        return "";
    }
{
    bool bSelected = false;
    bool bMasked = false;
    bool bMoving = false;
    public byte Value { get; private set; }
    float fSrcY;
    bool bChangeTo = false;

    private void Awake()
    {
        fSrcY = transform.localPosition.y;
    }

    public void Init()
    {
        Selected(false, true);
        Masked = false;
        bMoving = false;
        gameObject.SetActive(false);
    }

    public bool Selected()
    {
        return bChangeTo && gameObject.activeSelf;
    }

    public void Selected(bool value, bool bForce = false)
    {
        if (!gameObject.activeSelf)
            return;

        if (bForce)
        {
            RectTransform tfm = transform as RectTransform;
            Vector3 vec = tfm.localPosition;
            float offset = tfm.rect.height * 0.1f;
            vec.y = bSelected ? (fSrcY + offset) : fSrcY;
            transform.localPosition = vec;
            bMoving = false;
        }
            bChangeTo = value;
    }

    private void LateUpdate()
    {
        if (bChangeTo != bSelected)
            bMoving = true;
    }

    private void Update()
    {
        if (!bMoving)
            return;

        if (bSelected != bChangeTo)
        {
            RectTransform tfm = transform as RectTransform;
            float speed = tfm.rect.height * 0.5f;
            float y = tfm.localPosition.y;
            Vector3 vec = tfm.localPosition;
            if (bChangeTo)
            {
                float target = fSrcY + tfm.rect.height * 0.1f;
                y += speed * Time.deltaTime;
                if (y > target)
                {
                    bSelected = bChangeTo;
                    y = target;
                    bMoving = false;
                }
            }
            else
            {
                y -= speed * Time.deltaTime;
                if (y < fSrcY)
                {
                    bSelected = bChangeTo;
                    y = fSrcY;
                    bMoving = false;
                }
            }
            vec.y = y;
            tfm.localPosition = vec;
        }
        else
            Selected(bChangeTo, true);
    }

    public bool Masked
    {
        get
        {
            return bMasked;
        }

        set
        {
            bMasked = value;

            Transform tfm = transform.Find("image_Mask");
            if(tfm != null)
                tfm.gameObject.SetActive(bMasked);
        }
    }

    public static void SetCardSprite(GameObject go, AssetBundle ab, byte card, bool showMask, byte laizi = 0, string postfix = "")
    {
        Image img = go.transform.GetComponent<Image>();
        if (card == 0)
            img.enabled = false;
        else
        {
            Sprite sp;
            if (card == RoomInfo.NoSit)
                sp = ab.LoadAsset<Sprite>("puke_back" + postfix);
            else
                sp = ab.LoadAsset<Sprite>(GameCommon.GetPokerMat(card, laizi) + postfix);
            img.sprite = sp;
            img.enabled = true;
        }
        go.SetActive(true);

        Transform tfm = go.transform.Find("Image_zhuang");
        if (tfm != null)
            tfm.gameObject.SetActive(showMask);
        tfm = go.transform.Find("image_Mask");
        if (tfm != null)
            tfm.gameObject.SetActive(false);
    }

    public void SetCardSprite(AssetBundle ab, byte card, bool showMask = false, byte laizi = 0, string postfix = "_big")
    {
        SetCardSprite(gameObject, ab, card, showMask, laizi, postfix);
        Value = card;
    }
}