//create by Batcel 2017-12-29


using System.Collections.Generic;using System.IO;
using UnityEngine;using UnityEngine.UI;
using XLua;
using UnityEngine.EventSystems;
using USocket.Messages;public class TrumpetData
{
    public string SendPlayerName;
    public string SendContent;
}

/// <summary>/// ����/// </summary>
[Hotfix]
public class CTrumpetUI
{
    private static CTrumpetUI instance = new CTrumpetUI();
    private List<TrumpetData> TrumpetHistroyList;

    private Object TrumpetMessageSendUIPrefab;
    private GameObject TrumpetMessageSendUIObj;

    CTrumpetUI()
    {
        TrumpetMessageSendUIObj = null;
        TrumpetHistroyList = new List<TrumpetData>();

        RegitserMsgHandle();
    }

    public static CTrumpetUI Instance    {        get { return instance; }    }


    private void RegitserMsgHandle()
    {
        //CMsgDispatcher.GetInstance().RegMsgDictionary(
        //       (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_SENDHORNTOALL, BackTotall); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACK_SENDHORNTOALL, HandleTrumpetSendResult); //
    }

    //��ʼ���Ȱ�ť
    public void  InitTrumpetButtenEvent(string btnpath)
    {
        Transform tfm = GameObject.Find("Canvas/Root").transform.Find(btnpath);
        if(tfm != null)
            XPointEvent.AutoAddListener(tfm.gameObject, OnClickTrumpetBtn, 1);
    }

    //��ʼ�����ȹ��ܽ���
    private bool InitTrumpetUI()    {        AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bd == null)
            return false;        if (TrumpetMessageSendUIPrefab == null)            TrumpetMessageSendUIPrefab = bd.LoadAsset("Tips_Marquee_Popup") as Object;        if(TrumpetMessageSendUIObj == null)        {                       TrumpetMessageSendUIObj = GameMain.instantiate(TrumpetMessageSendUIPrefab) as GameObject;
            TrumpetMessageSendUIObj.SetActive(false);
            TrumpetMessageSendUIObj.transform.SetParent(GameObject.Find("Canvas/Root").transform, false);        
        }        InitTrumpetUIEvent();        return true;    }

    //��ʼ���Ƚ��水ť����¼�
    private void InitTrumpetUIEvent()
    {
        GameObject sendbtn = TrumpetMessageSendUIObj.transform.Find("ImageInputBG/Buttonsend").gameObject;
        XPointEvent.AutoAddListener(sendbtn, OnTrumpetSendBtn, null);      
        XPointEvent.AutoAddListener(TrumpetMessageSendUIObj, OnClickTrumpetBtn, 0);
    }


    //������Ȱ�ť
    private void OnClickTrumpetBtn(EventTriggerType eventtype, object param, PointerEventData eventData)
    {
        if(eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            if ((int)param == 1)
              OpenOrCloseTrumpetUI(true);
            else
              OpenOrCloseTrumpetUI(false);
        }
    }

    //�������������Ϣ��ť
    private void OnTrumpetSendBtn(EventTriggerType eventtype, object param, PointerEventData eventData)
    {
        if(eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            PlayerData playerdata = GameMain.hall_.GetPlayerData();
            if(playerdata.GetDiamond() <5)
            {
                CRollTextUI.Instance.AddVerticalRollText(1501);
                return;
            }

            InputField content = TrumpetMessageSendUIObj.transform.Find("ImageInputBG/Inputcontent").gameObject.GetComponent<InputField>();
            string sendtext = content.text;
            if (sendtext.Length == 0)
            {
                CRollTextUI.Instance.AddVerticalRollText(1506);
                return;
            }

            if (sendtext.Length >30)
            {
                CRollTextUI.Instance.AddVerticalRollText(1503);
                return;
            }

            UMessage trumpetMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_SENDHORNTOALL);
            trumpetMsg.Add(GameMain.hall_.GetPlayerId());
            GameCommon.CheckForbiddenWord(ref sendtext, true);
            trumpetMsg.Add(sendtext);
            NetWorkClient.GetInstance().SendMsg(trumpetMsg);

            OpenOrCloseTrumpetUI(false);
        }
    }

    //���������ʷ��¼
    public void AddTurmpetHistroy(string TrumpetStr)
    {
        if (TrumpetStr.Split(':').Length == 1)
            return;

        TrumpetData data = new TrumpetData();
        data.SendPlayerName = TrumpetStr.Split(':')[0];
        data.SendContent = TrumpetStr.Split(':')[1];
        if (TrumpetHistroyList.Count >= 10)
            TrumpetHistroyList.RemoveAt(9);
        TrumpetHistroyList.Add(data);
    }


    //��ʾ���������ȹ��ܽ���
    private void OpenOrCloseTrumpetUI(bool bShow)
    {
        if (TrumpetMessageSendUIObj == null)
            InitTrumpetUI();  
        if(bShow)
        {
            //�����ʷ��Ϣ������
            GameObject marqueelist = TrumpetMessageSendUIObj.transform.Find("ImageMask/MarqueeViewport/MarqueeContent").gameObject;
            GameMain.hall_.ClearChilds(marqueelist);

            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);            if (bd == null)                return;
            Object trumpettextprefab = bd.LoadAsset("Marquee_text") as Object;

            for (int index = TrumpetHistroyList.Count - 1; index >= 0; index--)
            {              
                GameObject trumpettextObj = GameMain.instantiate(trumpettextprefab) as GameObject;
                trumpettextObj.transform.SetParent(marqueelist.transform, false);
                Text text = trumpettextObj.transform.Find("Text").gameObject.GetComponent<Text>();
                text.text = TrumpetHistroyList[index].SendPlayerName + ":" + TrumpetHistroyList[index].SendContent;
            }
        }
        TrumpetMessageSendUIObj.SetActive(bShow);
    }


    //���ȷ����������
    private bool HandleTrumpetSendResult(uint _msgType, UMessage _ms)
    {
        byte issuccess = _ms.ReadByte();
        uint needdiamond = _ms.ReadUInt();
        uint diamond = _ms.ReadUInt();
        if (issuccess == 0)
        {
            GameMain.hall_.GetPlayerData().SetDiamond(diamond);
            GameMain.hall_.RefreshShopPlayerDiamondText();
            CRollTextUI.Instance.AddVerticalRollText(1502);
        }
        if (issuccess == 1)
        {
            CRollTextUI.Instance.AddVerticalRollText(1501);
        }
        return true;
    }

}