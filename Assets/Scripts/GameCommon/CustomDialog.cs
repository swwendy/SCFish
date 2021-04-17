using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;
/// <summary>
/// ͨ����ʾ����
/// </summary>
[LuaCallCSharp]
public class CCustomDialog
{
    public delegate void CallBackFunc(object pragma);
    private static CallBackFunc DialogFuncCallBack;
    private static CallBackFunc ConfirmFuncCallBack;

    //ͨ�öԻ���
    private static GameObject CustomDialogObj = null;

    private static CTimerPersecondCall DialogTimer = null;

    private static string DialogTipsDataString = null;

    //ͨ��ȷ�Ͽ�
    private static GameObject CustomConfirmObj = null;

    //ͨ�õȴ���
    private static GameObject CustomWaitObj = null;

    /// <summary>
    /// ��ͨ�öԻ���
    /// </summary>
    /// <param name="titletxt">�����ı�</param>
    /// <param name="callbackfunc">��ť�����Ļص�����</param>
    /// <param name="contenttxt">��ʾ����</param>
    /// <returns></returns>
    public static bool OpenCustomDialogUI(string contenttxt, CallBackFunc callbackfunc, string titletxt = null)
    {
        if(CustomDialogObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomDialogObj = GameMain.instantiate( bd.LoadAsset("Tips_Dialog") as Object) as GameObject;
        }
        DialogFuncCallBack = callbackfunc;
        //����
        Text titleText = CustomDialogObj.transform.Find("ImageBG").Find("TextTop").gameObject.GetComponent<Text>();
        if (titleText == null)
            titleText.text = "��ʾ";
        else
            titleText.text = titletxt;

        //����
        Text contentText = CustomDialogObj.transform.Find("ImageBG").Find("ImageBG_01").
            Find("TextContent").gameObject.GetComponent<Text>();
        contentText.text = contenttxt;

        //��ť
        GameObject okBtn = CustomDialogObj.transform.Find("ImageBG").Find("ButtonOk").gameObject;
        XPointEvent.AutoAddListener(okBtn, OnDialogUIBtnClick, 1);

        GameObject cancelBtn = CustomDialogObj.transform.Find("ImageBG").Find("ButtonCancel").gameObject;
        XPointEvent.AutoAddListener(cancelBtn, OnDialogUIBtnClick, 0);

        Transform canvastf = GameObject.Find("Canvas_1/Root").transform;

        CustomDialogObj.transform.SetParent(canvastf, false);
        CustomDialogObj.transform.SetAsLastSibling();
        CustomDialogObj.SetActive(true);
        
        return true;
    }

    /// <summary>
    /// ��ͨ�öԻ���
    /// </summary>
    /// <param name="titletxt">�����ı�</param>
    /// <param name="callbackfunc">��ť�����Ļص�����</param>
    /// <param name="tipsId">tipsdataId</param>
    /// <returns></returns>
    public static bool OpenCustomDialogWithFormatParams(uint tipsId, CallBackFunc callbackfunc,params object[] args)
    {
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(tipsId);
        if (tdata == null)
            return false;

        if (CustomDialogObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomDialogObj = GameMain.instantiate(bd.LoadAsset("Tips_Dialog") as Object) as GameObject;
        }
        DialogFuncCallBack = callbackfunc;
        //����
        Text titleText = CustomDialogObj.transform.Find("ImageBG").Find("TextTop").gameObject.GetComponent<Text>();
            titleText.text=tdata.TipsTitle;
        //����
        Text contentText = CustomDialogObj.transform.Find("ImageBG").Find("ImageBG_01").
            Find("TextContent").gameObject.GetComponent<Text>();
        string formatstr = string.Format(tdata.TipsText, args);
        contentText.text = formatstr;

        //��ť
        GameObject okBtn = CustomDialogObj.transform.Find("ImageBG").Find("ButtonOk").gameObject;
        XPointEvent.AutoAddListener(okBtn, OnDialogUIBtnClick, 1);

        GameObject cancelBtn = CustomDialogObj.transform.Find("ImageBG").Find("ButtonCancel").gameObject;
        XPointEvent.AutoAddListener(cancelBtn, OnDialogUIBtnClick, 0);

        Transform canvastf = GameObject.Find("Canvas_1/Root").transform;

        CustomDialogObj.transform.SetParent(canvastf, false);
        CustomDialogObj.transform.SetAsLastSibling();
        CustomDialogObj.SetActive(true);

        return true;
    }

    /// <summary>
    /// ��ͨ�öԻ���
    /// </summary>
    /// <param name="titletxt">�����ı�</param>
    /// <param name="callbackfunc">��ť�����Ļص�����</param>
    /// <param name="tipsId">tipsdataId</param>
    /// <returns></returns>
    public static bool OpenCustomDialogWithTimer(uint tipsId, uint timersecond,CallBackFunc callbackfunc)
    {
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(tipsId);
        if (tdata == null)
            return false;

        if (CustomDialogObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomDialogObj = GameMain.instantiate(bd.LoadAsset("Tips_Dialog") as Object) as GameObject;
        }
        DialogFuncCallBack = callbackfunc;
        //����
        Text titleText = CustomDialogObj.transform.Find("ImageBG").Find("TextTop").gameObject.GetComponent<Text>();
        titleText.text = tdata.TipsTitle;
        //����
        Text contentText = CustomDialogObj.transform.Find("ImageBG").Find("ImageBG_01").
            Find("TextContent").gameObject.GetComponent<Text>();
        DialogTipsDataString = tdata.TipsText;
        string formatstr = string.Format(tdata.TipsText, timersecond);
        contentText.text = formatstr;

        DialogTimer = new CTimerPersecondCall(timersecond, DialogTimerCallback);
        xTimeManger.Instance.RegisterTimer(DialogTimer);


        //��ť
        GameObject okBtn = CustomDialogObj.transform.Find("ImageBG").Find("ButtonOk").gameObject;
        XPointEvent.AutoAddListener(okBtn, OnDialogUIBtnClick, 1);

        GameObject cancelBtn = CustomDialogObj.transform.Find("ImageBG").Find("ButtonCancel").gameObject;
        XPointEvent.AutoAddListener(cancelBtn, OnDialogUIBtnClick, 0);

        Transform canvastf = GameObject.Find("Canvas_1/Root").transform;

        CustomDialogObj.transform.SetParent(canvastf, false);
        CustomDialogObj.transform.SetAsLastSibling();
        CustomDialogObj.SetActive(true);

        return true;
    }


    /// <summary>
    /// ȷ�Ͽ��ʱ���ص����� 
    /// </summary>
    /// <param name="args"></param>
    private static void DialogTimerCallback(object[] RemainTime)
    {
        float fRemaintime = (float)RemainTime[0];

        if(fRemaintime >= 0)
        {
            Text contentText = CustomDialogObj.transform.Find("ImageBG").Find("ImageBG_01").
                     Find("TextContent").gameObject.GetComponent<Text>();

            string formatstr = string.Format(DialogTipsDataString,Mathf.Ceil(fRemaintime));
            contentText.text = formatstr;
        }
        else
        {
            DialogFuncCallBack(0);
            CustomDialogObj.SetActive(false);
        }
    }


    /// <summary>
    /// ��ͨ�öԻ���
    /// </summary>
    /// <param name="titletxt">�����ı�</param>
    /// <param name="callbackfunc">��ť�����Ļص�����</param>
    /// <param name="tipsId">tipsdataId</param>
    /// <returns></returns>
    public static bool OpenCustomDialogWithTipsID(uint tipsId, CallBackFunc callbackfunc)
    {
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(tipsId);
        if (tdata == null)
            return false;

        if (CustomDialogObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomDialogObj = GameMain.instantiate(bd.LoadAsset("Tips_Dialog") as Object) as GameObject;
        }
        DialogFuncCallBack = callbackfunc;
        //����
        Text titleText = CustomDialogObj.transform.Find("ImageBG").Find("TextTop").gameObject.GetComponent<Text>();
        titleText.text = tdata.TipsTitle;
        //����
        Text contentText = CustomDialogObj.transform.Find("ImageBG").Find("ImageBG_01").
            Find("TextContent").gameObject.GetComponent<Text>();
        contentText.text = tdata.TipsText;

        //��ť
        GameObject okBtn = CustomDialogObj.transform.Find("ImageBG").Find("ButtonOk").gameObject;
        XPointEvent.AutoAddListener(okBtn, OnDialogUIBtnClick, 1);

        GameObject cancelBtn = CustomDialogObj.transform.Find("ImageBG").Find("ButtonCancel").gameObject;
        XPointEvent.AutoAddListener(cancelBtn, OnDialogUIBtnClick, 0);

        Transform canvastf = GameObject.Find("Canvas_1/Root").transform;

        CustomDialogObj.transform.SetParent(canvastf, false);
        CustomDialogObj.transform.SetAsLastSibling();
        CustomDialogObj.SetActive(true);

        return true;
    }

    private static void OnDialogUIBtnClick(EventTriggerType eventtype, object parma, PointerEventData eventData)
    {
        if(eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);

            DialogFuncCallBack(parma);
            CustomDialogObj.SetActive(false);
            if(DialogTimer != null)
            {
                DialogTimer.SetDeleteFlag(true);
            }
        }
    }

    /// <summary>
    /// ��ͨ��ȷ�Ͽ�
    /// </summary>
    /// <param name="tipsId">��ʾdataId</param>
    /// <returns></returns>
    public static bool OpenCustomConfirmUI(uint tipsId,CallBackFunc func = null, object[] values = null)
    {
        if (CustomConfirmObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomConfirmObj = GameMain.instantiate(bd.LoadAsset("Tips_Confirm") as Object) as GameObject;
        }
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(tipsId);
        if (tdata == null)
            return false;
        ConfirmFuncCallBack = func;
        //����
        Text titleText = CustomConfirmObj.transform.Find("ImageBG").Find("TextTop").gameObject.GetComponent<Text>();
        titleText.text = tdata.TipsTitle;

        //����
        Text contentText = CustomConfirmObj.transform.Find("ImageBG").Find("ImageBG_01").
            Find("TextContent").gameObject.GetComponent<Text>();
        if(values != null)
            contentText.text = string.Format(tdata.TipsText, values);
        else
            contentText.text = tdata.TipsText;

        //��ť
        GameObject okBtn = CustomConfirmObj.transform.Find("ImageBG").Find("ButtonOk").gameObject;
        XPointEvent.AutoAddListener(okBtn, CloseCustomConfirmUI,null);

        Transform canvastf = GameObject.Find("Canvas_1/Root").transform;

        CustomConfirmObj.transform.SetParent(canvastf,false);
        CustomConfirmObj.transform.SetAsLastSibling();
        CustomConfirmObj.SetActive(true);
        return true;
    }

    /// <summary>
    /// ��ͨ��ȷ�Ͽ�
    /// </summary>
    /// <param name="tipsId">��ʾdataId</param>
    /// <param name="parmas">�贫�����</param>
    /// <returns></returns>
    public static bool OpenCustomConfirmUIWithFormatParam(uint tipsId, params object[] values)
    {
        if (CustomConfirmObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomConfirmObj = GameMain.instantiate(bd.LoadAsset("Tips_Confirm") as Object) as GameObject;
        }
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(tipsId);
        if (tdata == null)
            return false;
        ConfirmFuncCallBack = null;
        //����
        Text titleText = CustomConfirmObj.transform.Find("ImageBG").Find("TextTop").gameObject.GetComponent<Text>();
        titleText.text = tdata.TipsTitle;

        //����
        Text contentText = CustomConfirmObj.transform.Find("ImageBG").Find("ImageBG_01").
            Find("TextContent").gameObject.GetComponent<Text>();
        string formatstr = string.Format(tdata.TipsText, values);
        contentText.text = formatstr;

        //��ť
        GameObject okBtn = CustomConfirmObj.transform.Find("ImageBG").Find("ButtonOk").gameObject;
        XPointEvent.AutoAddListener(okBtn, CloseCustomConfirmUI, null);

        Transform canvastf = GameObject.Find("Canvas_1/Root").transform;

        CustomConfirmObj.transform.SetParent(canvastf, false);
        CustomConfirmObj.transform.SetAsLastSibling();
        CustomConfirmObj.SetActive(true);
        return true;
    }

    public static bool OpenCustomConfirmUIWithFormatParamFunc(uint tipsId, CallBackFunc func, params object[] values)
    {
        if (CustomConfirmObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomConfirmObj = GameMain.instantiate(bd.LoadAsset("Tips_Confirm") as Object) as GameObject;
        }
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(tipsId);
        if (tdata == null)
            return false;
        ConfirmFuncCallBack = func;
        //����
        Text titleText = CustomConfirmObj.transform.Find("ImageBG").Find("TextTop").gameObject.GetComponent<Text>();
        titleText.text = tdata.TipsTitle;

        //����
        Text contentText = CustomConfirmObj.transform.Find("ImageBG").Find("ImageBG_01").
            Find("TextContent").gameObject.GetComponent<Text>();
        string formatstr = string.Format(tdata.TipsText, values);
        contentText.text = formatstr;

        //��ť
        GameObject okBtn = CustomConfirmObj.transform.Find("ImageBG").Find("ButtonOk").gameObject;
        XPointEvent.AutoAddListener(okBtn, CloseCustomConfirmUI, null);

        Transform canvastf = GameObject.Find("Canvas_1/Root").transform;

        CustomConfirmObj.transform.SetParent(canvastf, false);
        CustomConfirmObj.transform.SetAsLastSibling();
        CustomConfirmObj.SetActive(true);
        return true;
    }

    /// <summary>
    /// �ر�ͨ��ȷ�Ͽ�
    /// </summary>
    /// <param name="eventtype"></param>
    /// <param name="parma"></param>
    /// <param name="eventData"></param>
    private static void CloseCustomConfirmUI(EventTriggerType eventtype, object parma, PointerEventData eventData)
    {
        if(eventtype == EventTriggerType.PointerClick)
        {
            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if (CustomConfirmObj != null)
            {
                CustomConfirmObj.SetActive(false);
                if (ConfirmFuncCallBack != null)
                    ConfirmFuncCallBack(parma);
            }
        }
    }

    /// <summary>
    /// ��ͨ�õȴ�����
    /// </summary>
    /// <param name="tipstext"></param>
    /// <returns></returns>
    public static bool OpenCustomWaitUI(string tipstext, bool block = true)
    {
        if (CustomWaitObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomWaitObj = GameMain.instantiate(bd.LoadAsset("Tips_Waiting") as Object) as GameObject;
        }

        //����
        Text contentText = CustomWaitObj.transform.Find("Text").gameObject.GetComponent<Text>();
        contentText.text = tipstext;

        Transform canvastf = GameObject.Find("Canvas/Root").transform;

        CustomWaitObj.transform.SetParent(canvastf, false);
        CustomWaitObj.transform.SetAsLastSibling();
        CustomWaitObj.SetActive(true);
        CustomWaitObj.GetComponent<Image>().raycastTarget = block;
        return true;
    }

    /// <summary>
    /// ��ͨ�õȴ�����
    /// </summary>
    /// <param name="tipstext"></param>
    /// <returns></returns>
    public static bool OpenCustomWaitUI(uint tipsID, bool block = true)
    {
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(tipsID);
        if (tdata == null)
            return false;

        if (CustomWaitObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomWaitObj = GameMain.instantiate(bd.LoadAsset("Tips_Waiting") as Object) as GameObject;
           
        }
       
        //����
        Text contentText = CustomWaitObj.transform.Find("Text").gameObject.GetComponent<Text>();
        contentText.text = tdata.TipsText;

        Transform canvastf = GameObject.Find("Canvas/Root").transform;
        CustomWaitObj.transform.SetParent(canvastf, false);
        CustomWaitObj.transform.SetAsLastSibling();
        CustomWaitObj.SetActive(true);
        CustomWaitObj.GetComponent<Image>().raycastTarget = block;
        
        return true;
    }


    /// <summary>
    /// ��ͨ�õȴ�����
    /// </summary>
    /// <param name="tipstext"></param>
    /// <returns></returns>
    public static bool OpenCustomWaitUI(uint tipsID,string param, bool block = true)
    {
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(tipsID);
        if (tdata == null)
            return false;

        if (CustomWaitObj == null)
        {
            AssetBundle bd = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
            if (bd == null)
                return false;
            CustomWaitObj = GameMain.instantiate(bd.LoadAsset("Tips_Waiting") as Object) as GameObject;
        }

        //����
        Text contentText = CustomWaitObj.transform.Find("Text").gameObject.GetComponent<Text>();
        string formatstr = string.Format(tdata.TipsText, param);
        contentText.text = formatstr;

        Transform canvastf = GameObject.Find("Canvas/Root").transform;

        CustomWaitObj.transform.SetParent(canvastf, false);
        CustomWaitObj.transform.SetAsLastSibling();
        CustomWaitObj.SetActive(true);
        CustomWaitObj.GetComponent<Image>().raycastTarget = block;
        return true;
    }


    /// <summary>
    /// �ر�ͨ�õȴ�����
    /// </summary>
    public static void CloseCustomWaitUI()
    {
        if (CustomWaitObj != null)
        {
            CustomWaitObj.SetActive(false);
            //GameObject.Destroy(CustomWaitObj);
            //CustomWaitObj = null;
        }
    }


    /// <summary>
    /// �ر�ͨ�öԻ�����
    /// </summary>
    public static void CloseCustomDialogUI()
    {
        if (CustomDialogObj != null)
        {
            CustomDialogObj.SetActive(false);
        }
    }
}