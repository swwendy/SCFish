﻿//create by Batcel 2017-8-1


using System.Collections.Generic;
using UnityEngine;
using XLua;
using UnityEngine.EventSystems;
using USocket.Messages;



/// <summary>
public class CRollTextUI

    private bool bRollTickPause;

    #region 水平滚动文本
    private List<string> HorizontalRollStringCacheList;

    #endregion

    #region 垂直滚动文本提示

    class VerticalRollObjData
    {
       public GameObject VerticalRollUIObj;
       public float DisappearTime;
    }

    private List<VerticalRollObjData> VerticalRollTextObjList;

    private Object VerticalRollUIPerfabObj;

    private bool bVerticalRollTextUIShow;

    #endregion



    CRollTextUI()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
                (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_SENDHORNTOALL, HandleTrumpetMsg);
    }
    {
        SystemHorizontalRollTextList.Clear();
        //加载系统默认滚动文本
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.CsvAssetbundleName);
        if (bundle == null)
            return false;
        TextAsset datatxt = bundle.LoadAsset("RollText", typeof(TextAsset)) as TextAsset;
        if (datatxt == null)
            return false;
        MemoryStream stream = new MemoryStream(datatxt.bytes);
        StreamReader filereader = new StreamReader(stream);

        string strline = "";
        while (strline != null)
        {
            strline = filereader.ReadLine();
            if (strline != null && strline.Length > 0)
            {
                SystemHorizontalRollTextList.Add(strline);
            }
        }
        filereader.Close();
        filereader.Dispose();
        return true;
    }
            LoadSystemRollText();

        }
    /// 处理服务器发送的喇叭消息
    /// </summary>
    /// <param name="_msgType"></param>
    /// <param name="_ms"></param>
    /// <returns></returns>
    {
        string name = _ms.ReadString();
        string content = _ms.ReadString();
        
        AddHorizontalRollText("<color=#FFFF00FF>" + name + "</color>" + ":" + content);
        return true;
    }

    /// <summary>

    /// <summary>
        if (tdata == null)
            return false;
        string FormatRollStr = string.Format(tdata.TipsText, money, playername);

    /// <summary>
    /// 创建垂直滚动条
    /// </summary>
    /// <param name="rollText"></param>
    private void CreateVerticalRollTextObj(string rollText)
    {
        if(VerticalRollUIPerfabObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            VerticalRollUIPerfabObj = bd.LoadAsset("Tips_Text") as Object;
        }

        GameObject obj = GameMain.instantiate(VerticalRollUIPerfabObj) as GameObject;
        obj.transform.SetParent(GameObject.Find("Canvas_1/Root").transform, false);
        obj.transform.Find("Text").gameObject.GetComponent<Text>().text = rollText;
        //调整一下已有文本的坐标
        int ObjListCount = VerticalRollTextObjList.Count;
        for (int i = 0; i< ObjListCount; i++)
        {
            Vector3 HorObjPos = VerticalRollTextObjList[i].VerticalRollUIObj.transform.localPosition;
            HorObjPos.y = (ObjListCount -i)*40;
            VerticalRollTextObjList[i].VerticalRollUIObj.transform.localPosition = HorObjPos;
        }

        VerticalRollObjData vrdata = new VerticalRollObjData();
        vrdata.VerticalRollUIObj = obj;
        vrdata.DisappearTime = 3.0f;
        VerticalRollTextObjList.Add(vrdata);
        bVerticalRollTextUIShow = true;
    }

    /// <summary>
    /// 添加垂直滚动文本
    /// </summary>
    /// <param name="RollTipsId"></param>
    /// <returns></returns>
    public bool AddVerticalRollText(uint RollTipsId)
    {
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(RollTipsId);
        if (tdata == null)
            return false;

        CreateVerticalRollTextObj(tdata.TipsText);

        return true;
    }


    /// <summary>
    /// 添加垂直滚动文本
    /// </summary>
    /// <param name="RollTipsId"></param>
    /// <returns></returns>
    public bool AddVerticalRollText(string VerticalString)
    {
        if (string.IsNullOrEmpty(VerticalString))
            return false;
        CreateVerticalRollTextObj(VerticalString);
        return true;
    }


    /// <summary>
    /// 设置rollText暂停开关
    /// </summary>
    /// <param name="pausetick"></param>
    {
        if (!pausetick)
        {
            RegitserRollUIToCanvas();
            if(HorizontalRollStringCacheList.Count >0)
                HorizontalRollUIObj.SetActive(true);
        }
        bRollTickPause = pausetick;

        if(bRollTickPause)
        {
            bVerticalRollTextUIShow = false;
            foreach (VerticalRollObjData obj in VerticalRollTextObjList)
                GameMain.safeDeleteObj(obj.VerticalRollUIObj);
            VerticalRollTextObjList.Clear();
        }

    }


    /// <summary>

        if (HorizontalRollUIObj == null)
    /// 注册一个计时器
    /// </summary>
    {      
        SystemHorizontalRollTextTimer = new SingleTimeBase(10f, SystemRollTimeCallBack, null);
        xTimeManger.Instance.RegisterTimer(SystemHorizontalRollTextTimer);
    }
    /// 计时器回调
    /// </summary>
    /// <param name="pramas"></param>
    {
        if (bHorizontalRollTextUIShow)
            return;
        bHorizontalRollTextUIShow = true;
        int randint = Random.Range(0, SystemHorizontalRollTextList.Count);
        RegitserRollUIToCanvas();
        SetTextToRoll(SystemHorizontalRollTextList[randint]);      
        HorizontalRollUIObj.SetActive(true);


        //AddVerticalRollText("这是一条测试文本");
        //AddVerticalRollText("易方达人可圣诞卡");
        //AddVerticalRollText("短发控龙湖镇中国最强音");
    }
    /// 推进水平滚动文本
    /// </summary>
    {
        if (HorizontalRollTextTransf == null)
            return;
        Vector3 textPos = HorizontalRollTextTransf.localPosition;

        //检测当前是否已经显示到文本结尾了
        float textshowposx = textPos.x + textRect.x + 230;
            if (HorizontalRollStringCacheList.Count > 0)
            {
                bHorizontalRollTextUIShow = false;
                HorizontalRollUIObj.SetActive(false);
                InitSystemRollTimer();
            }
        }
    }
    /// 推进垂直滚动文本
    /// </summary>
    {
        
        for ( int i = 0; i< VerticalRollTextObjList.Count;i++)
        {
            VerticalRollObjData VRdata = VerticalRollTextObjList[i];
            //Vector3 HorObjPos = VRdata.VerticalRollUIObj.transform.localPosition;
            //float posxmove = Time.deltaTime * 40;
            //HorObjPos.y += posxmove;
            //VRdata.VerticalRollUIObj.transform.localPosition = HorObjPos;
            VRdata.DisappearTime -= Time.deltaTime;
           
            if(VRdata.DisappearTime < 1.0f && VRdata.DisappearTime > 0.0f)
            {
                Color imgcol = VRdata.VerticalRollUIObj.GetComponent<Image>().color;
                imgcol.a = VRdata.DisappearTime ;
                VRdata.VerticalRollUIObj.GetComponent<Image>().color = imgcol;

                Color txtcol = VRdata.VerticalRollUIObj.transform.Find("Text").gameObject.GetComponent<Text>().color;
                txtcol.a =  VRdata.DisappearTime;
                VRdata.VerticalRollUIObj.transform.Find("Text").gameObject.GetComponent<Text>().color = txtcol;
            }

            if(VRdata.DisappearTime < 0f)
            {
                GameMain.safeDeleteObj(VRdata.VerticalRollUIObj);
                VerticalRollTextObjList[i] = null;
                VerticalRollTextObjList.RemoveAt(i);
            }
        }
        if (VerticalRollTextObjList.Count == 0)
            bVerticalRollTextUIShow = false;
    }