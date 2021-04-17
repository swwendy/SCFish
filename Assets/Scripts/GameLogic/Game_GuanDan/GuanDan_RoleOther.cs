﻿using DG.Tweening;
using UnityEngine;
    GameObject m_DisconnectObj;
    byte m_nCurPokerNum;
        m_CountdownObj = m_GameBase.MainUITfm.Find("Middle/OutlineCountdown/ImageBG/Player_" + index).gameObject; ;
        Transform tfm = m_GameBase.MainUITfm.Find("Middle/PlayerInfor/Icon_PlayerInfor_" + index);
        m_SitUI = tfm.Find("Button_Sitdown");
        m_DisconnectObj = tfm.Find("Text_offline").gameObject;

        m_nIndex = index;


        m_RecordPokerTfm = m_GameBase.RecordUITfm.Find("Point_shoupai_" + index);
        m_RecordPokerTfm.gameObject.SetActive(false);
        m_HaveCards.gameObject.SetActive(false);
    }
        m_nCurPokerNum = 0;
        {
                GameObject.Destroy(tfm.gameObject);
            m_HaveCards.gameObject.SetActive(false);


    public override void CreateRole()
    {
        CreateRole(m_nIndex == 2 || m_nIndex == 3, m_GameBase.MainUITfm.Find("Middle/PlayerInfor/Icon_PlayerInfor_"
            + m_nIndex).Find("point_ren"));
    }
    {
        m_HaveCards.gameObject.SetActive(true);

        if (showGradeIndex != null)
            showGradeIndex.Sort();


            if (showGradeIndex != null)
            {
                if (showIndex < showGradeIndex.Count)
                {
                    if (pokerNum - 1 == showGradeIndex[showIndex])
                    {
                        ShowTipPoker(showPoker, showIndex);
                        showIndex++;
                    }
                }
            }

    public override void RemoveCards(byte[] cards)
        if (m_GameBase.GameMode == GameTye_Enum.GameType_Record)
        {
            foreach (byte card in cards)
                m_HavePokerList.Remove(card);
            UpdateRecordCards();
        }
        {
            if (m_nCurPokerNum == 0)
                m_HaveCards.gameObject.SetActive(false);
            else
                m_HaveCards.GetComponentInChildren<Text>().text = m_nCurPokerNum <= 10 ? m_nCurPokerNum.ToString() : "";
        }

    public override void ShowEndPoker(byte[] cards)
        foreach (Transform tfm in m_ShowPokerTfm)
            GameObject.Destroy(tfm.gameObject);

        bool bShowMask;
        GameObject prefabObj = (GameObject)m_GameBase.GuanDanAssetBundle.LoadAsset("Poker_Show_1");
        GameObject obj;
        for (int i = 0; i < endPokerList.Count; i++)
            obj = GameMain.Instantiate(prefabObj);
            obj.transform.SetParent(m_ShowPokerTfm, false);

            bShowMask = LL_PokerType.GetPokerLogicValue(endPokerList[i], laizi) == 15;
            PlayCard.SetCardSprite(obj, m_GameBase.CommonAssetBundle, endPokerList[i], bShowMask, laizi);
        }

    public override void AddCards(byte[] cards)
    {
        if (m_GameBase.GameMode == GameTye_Enum.GameType_Record)
        {
            m_HavePokerList.AddRange(cards);
            UpdateRecordCards();
        }
        {
            m_HaveCards.GetComponentInChildren<Text>().text = m_nCurPokerNum <= 10 ? m_nCurPokerNum.ToString() : "";
        }

    public override void OnPokerNumChanged(byte num)
    {
        base.OnPokerNumChanged(num);

        m_nCurPokerNum = num;
        RemoveCards(new byte[] { });
    }

    public override void UpdateRecordCards(bool begin = false)
    {
        m_RecordPokerTfm.gameObject.SetActive(true);

        byte laizi = LL_PokerType.GetLaiziValue();

        Dictionary<int, List<byte>> valueList = null;
        LL_PokerType.GetValueList(m_HavePokerList, out valueList);

        Transform tfm;
        GameObject child;
        bool bShowMask;
        int index;

        index = m_RecordPokerTfm.childCount - 1;
        foreach (var pokerList in valueList.Values)
        {
            tfm = m_RecordPokerTfm.GetChild(index);
            tfm.gameObject.SetActive(true);

            int i = 0;
            for (; i < pokerList.Count; i++)
            {
                child = tfm.GetChild(i).gameObject;
                child.SetActive(true);
                bShowMask = LL_PokerType.GetPokerLogicValue(pokerList[i], laizi) == 15;
                PlayCard.SetCardSprite(child, m_GameBase.CommonAssetBundle, pokerList[i], bShowMask, laizi, "_small");
            }

            for (; i < tfm.childCount; i++)
                tfm.GetChild(i).gameObject.SetActive(false);

            index--;
        }

        for (; index >= 0; index--)
            m_RecordPokerTfm.GetChild(index).gameObject.SetActive(false);
    }

    public override void OnDisOrReconnect(bool bDis)
    {
        base.OnDisOrReconnect(bDis);

        m_DisconnectObj.SetActive(bDis);
    }

    public override bool Disconnected()
    {
        return m_DisconnectObj.activeSelf;
    }

    public override void ChangeSit(bool bFly, float time, Vector3 targetPos, GuanDan_Role targetRole)
    {
        base.ChangeSit(bFly, time, targetPos, targetRole);

        if (bFly)
        {
            Image headImg = GetHeadImg();
            GameObject flyObj = new GameObject();
            flyObj.AddComponent<Image>().sprite = headImg.sprite;
            flyObj.transform.SetParent(headImg.transform, false);
            flyObj.transform.SetParent(m_GameBase.GameCanvas.transform, true);

            headImg.gameObject.SetActive(false);
            Sprite tSprite = targetRole.GetHeadImg().sprite;
            Transform nameTfm = m_InfoUI.Find("BG_nameJifen/Name_Text");
            nameTfm.gameObject.SetActive(false);
            string tName = targetRole.GetName();
            Transform coinTfm = m_InfoUI.Find("BG_nameJifen/Jifen_BG/Text_Jifen");
            coinTfm.gameObject.SetActive(false);
            Transform masterTfm = m_InfoUI.Find("Text_dashifen");
            masterTfm.gameObject.SetActive(false);
            long tCoin = targetRole.m_nTotalCoin;
            string tMaster = targetRole.GetMasterLv();
            bool tDis = targetRole.Disconnected();
            byte tSex = targetRole.Sex;
            List<byte> tPokers = new List<byte>(targetRole.m_HavePokerList);

            if (m_GameBase.GameMode == GameTye_Enum.GameType_Record)
                m_RecordPokerTfm.gameObject.SetActive(false);

            flyObj.transform.DOMove(targetPos, time).OnComplete(() =>
            {
                if (headImg != null)
                {
                    headImg.gameObject.SetActive(true);
                    headImg.sprite = tSprite;
                    nameTfm.gameObject.SetActive(true);
                    nameTfm.GetComponent<Text>().text = tName;
                    coinTfm.gameObject.SetActive(true);
                    coinTfm.GetComponent<Text>().text = m_GameBase.IsFree ? "" : tCoin.ToString();
                {
                    masterTfm.gameObject.SetActive(true);
                    masterTfm.GetComponent<Text>().text = tMaster;
                GameObject.Destroy(flyObj);

                ShowRole();

                if (m_GameBase.GameMode == GameTye_Enum.GameType_Record)
                {
        {
            {
                ShowRole();
            });
        }
    }
}