using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;
/// <summary>
/// 数字密码输入界面
/// </summary>
[Hotfix]
public class NumberPanel {

    /// <summary>
    /// 数字密码输入界面面板对象
    /// </summary>
    GameObject NumberPanelGameObjetct = null;

    /// <summary>
    /// 文本提示
    /// </summary>
    GameObject TextTipObject = null;

    /// <summary>
    /// 数字密码
    /// </summary>
    string InputNumberText = string.Empty;

    /// <summary>
    /// 数字密码输入界面实例对象
    /// </summary>
    static NumberPanel InstanceNumberPanel = null;

    /// <summary>
    /// 数字密码消息发送委托
    /// </summary>
    /// <param name="password">密码ID</param>
    public delegate void CallBackSendMsg(uint password);

    /// <summary>
    /// 数字密码消息发送委托对象
    /// </summary>
    private CallBackSendMsg CallBackSendMsgDelegate = null;
    /// <summary>
    /// 加载当前界面相关资源
    /// </summary>
    public void LoadNumberPanelResources()
    {
        if (NumberPanelGameObjetct != null)
        {
            return;
        }

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        UnityEngine.Object obj0 = bundle.LoadAsset("Import_Password");
        NumberPanelGameObjetct = (GameObject)GameMain.instantiate(obj0);

        GameObject CanvasObj = GameObject.Find("Canvas/Root");
        NumberPanelGameObjetct.transform.SetParent(CanvasObj.transform, false);
        NumberPanelGameObjetct.SetActive(false);
        TextTipObject = NumberPanelGameObjetct.transform.Find("ImageBG/InputField/TextTip").gameObject;

        InitNumberPanelEvent();
    }

    /// <summary>
    /// 初始化数字密码界面输入事件
    /// </summary>    private void InitNumberPanelEvent()    {        UnityEngine.Transform buttonsBGTransform = NumberPanelGameObjetct.transform.Find("ImageBG/ImageBG");        for (int index = 0; index < 10; index++)        {            int temp = index;            GameObject number = buttonsBGTransform.Find("Button_" + index.ToString()).gameObject;            XPointEvent.AutoAddListener(number, OnClickNumberEvent, temp);        }

        UnityEngine.Transform closeBtnTransform = NumberPanelGameObjetct.transform.Find("ImageBG/Button_close");        XPointEvent.AutoAddListener(closeBtnTransform.gameObject, OnCloseNumberPanelEvent, null);

        UnityEngine.Transform reinputTransform = buttonsBGTransform.Find("Button_delete");        XPointEvent.AutoAddListener(reinputTransform.gameObject, OnClickReinputEvent, null);        UnityEngine.Transform backspaceTransform = buttonsBGTransform.Find("Button_backspace");        XPointEvent.AutoAddListener(backspaceTransform.gameObject, OnClickBackSpaceEvent, null);    }

    /// <summary>
    /// 数字密码按钮点击事件
    /// </summary>    private void OnClickNumberEvent(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            if (InputNumberText.Length < 5)
            {
                int number = (int)button;                InputNumberText += number.ToString();
                UpdateInputText();
            }        }    }

    /// <summary>
    /// 密码重置事件
    /// </summary>    private void OnClickReinputEvent(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);
            ResetNumberInputTextData();        }    }

    /// <summary>
    /// 删除当前输入事件
    /// </summary>    private void OnClickBackSpaceEvent(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            if (!string.IsNullOrEmpty(InputNumberText))            {
                InputNumberText = InputNumberText.Remove(InputNumberText.Length - 1, 1);            }            UpdateInputText();        }    }

    /// <summary>
    /// 关闭界面事件
    /// </summary>
    private void OnCloseNumberPanelEvent(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);            SetNumberPanelActive(false);        }    }

    /// <summary>
    /// 更新数字密码
    /// </summary>    void UpdateInputText()    {        if (InputNumberText == null || NumberPanelGameObjetct == null)            return;        string oneName = "Text";        for (int index = 0; index < 5; index++)        {            if (index != 0)                oneName = "Text (" + index.ToString() + ")";            Text oneNumber = NumberPanelGameObjetct.transform.Find("ImageBG").Find("InputField").                Find("ImageTextBG").Find(oneName).gameObject.GetComponent<Text>();            if (index >= InputNumberText.Length)                oneNumber.text = string.Empty;            else                oneNumber.text = InputNumberText.Substring(index, 1);        }        if (InputNumberText.Length >= 5)        {            uint password = 0;            uint.TryParse(InputNumberText, out password);            CallBackSendMsgDelegate(password);            ResetNumberInputTextData();        }
        if(TextTipObject)
        {
            TextTipObject.SetActive(InputNumberText.Length == 0);
        }
    }

    /// <summary>
    /// 获得当前界面实例对象
    /// </summary>
    /// <returns></returns>
    public static NumberPanel GetInstance()
    {
        if(InstanceNumberPanel == null)
        {
            InstanceNumberPanel = new NumberPanel();
        }
        return InstanceNumberPanel;
    }

    /// <summary>
    /// 设置当前界面是否激活
    /// </summary>
    /// <param name="activeState">true:激活 false: 失效</param>
    /// <param name="_CallBackSendMsg">activeState 为true 的时候 此参数才有效 输入密码以后触发的委托</param>
    public void SetNumberPanelActive(bool activeState, CallBackSendMsg _CallBackSendMsg = null)
    {
        if(NumberPanelGameObjetct == null)
        {
            return;
        }
        NumberPanelGameObjetct.SetActive(activeState);
        if(activeState)
        {
            CallBackSendMsgDelegate = _CallBackSendMsg;
            ResetNumberInputTextData();
            NumberPanelGameObjetct.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// 重置密码输入数据
    /// </summary>
    public void ResetNumberInputTextData()
    {
        InputNumberText = string.Empty;
        UpdateInputText();
    }
}
