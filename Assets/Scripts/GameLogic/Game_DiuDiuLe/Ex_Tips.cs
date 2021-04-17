using UnityEngine;using UnityEngine.UI;using DG.Tweening;using UnityEngine.EventSystems;using XLua;[Hotfix]
public class Ex_Tips
{    GameObject tipsUI;    GameObject go_showGroup;    GameObject go_text;    GameObject go_btnGroup;    GameObject go_okBtn;    GameObject go_cancelBtn;
    private string str;
    public delegate void OkFunc();    public delegate void CancleFunc();    private OkFunc okFunc;    private CancleFunc cancleFunc;
    public Ex_Tips(GameObject _tipsUI, string _str, OkFunc _okFunc = null, CancleFunc _cancleFunc = null)
    {
        tipsUI = _tipsUI;        str = _str;        okFunc = _okFunc;        cancleFunc = _cancleFunc;        go_showGroup = tipsUI.transform.Find("ImageBG").gameObject;        go_text = go_showGroup.transform.Find("TextContent").gameObject;        go_btnGroup = go_showGroup.transform.Find("ButtonBG").gameObject;        go_okBtn = go_btnGroup.transform.Find("ButtonOk").gameObject;        go_cancelBtn = go_btnGroup.transform.Find("ButtonCancel").gameObject;        XPointEvent.AutoAddListener(go_okBtn, TapFunc, go_okBtn);
        XPointEvent.AutoAddListener(go_cancelBtn, TapFunc, go_cancelBtn);
        go_text.GetComponent<Text>().text = str;
        if (cancleFunc == null)
        {
            go_cancelBtn.SetActive(false);
        }
        go_showGroup.transform.localScale = new Vector3(0f, 0f, 1f);
        DOTween.To(() => go_showGroup.transform.localScale, r => go_showGroup.transform.localScale = r, new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.InOutBack);
    }
    void TapFunc(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            if (Ex_GameData.SOUNDCONFIG)
            {
                AudioManager.Instance.PlaySound("diu.resource", "ex_sBtn");
            }            if (button == (object)go_okBtn)            {                Tweener t = DOTween.To(() => go_showGroup.transform.localScale, r => go_showGroup.transform.localScale = r, new Vector3(0f, 0f, 1f), 0.3f).SetEase(Ease.InOutBack);                t.OnComplete(() =>                {
                    Object.Destroy(tipsUI);
                    if (okFunc != null)
                    {
                        okFunc();
                    }
                });            }            else if (button == (object)go_cancelBtn)            {
                Tweener t = DOTween.To(() => go_showGroup.transform.localScale, r => go_showGroup.transform.localScale = r, new Vector3(0f, 0f, 1f), 0.3f).SetEase(Ease.InOutBack);                t.OnComplete(() =>                {
                    Object.Destroy(tipsUI);
                    if (cancleFunc != null)
                    {
                        cancleFunc();
                    }
                });            }        }    }
}