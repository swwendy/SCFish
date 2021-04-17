using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USocket.Messages;

public class CrashRedBag
{
    public GameObject root_ = null;

    //兑换钻石界面对象
    private UnityEngine.Transform ExchangePanelTransform = null;
    //红包输入框对象
    private InputField RedBagInputField = null;
    //确定兑换按钮对象
    private Button DiamondOkButton = null;

    public CrashRedBag()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CashToDiamond, BackDiamondCashToDiamond);
    }

    void LoadCrashRedBagResource()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle == null)
            return;

        if(root_ == null)
        {
            UnityEngine.Object obj0 = (GameObject)bundle.LoadAsset("Lobby_RedEnvelope");
            root_ = (GameObject)GameMain.instantiate(obj0);

            GameObject CanvasObj = GameObject.Find("Canvas/Root");
            root_.transform.SetParent(CanvasObj.transform, false);

            GameObject sharebtn = root_.transform.Find("ImageBG/Image_xianjin/Button_share").gameObject;
            sharebtn.SetActive(false);
            //XPointEvent.AutoAddListener(sharebtn, OnShare, GameMain.hall_.GetPlayerData().UnreceivedRedBag);
            GameObject signbtn = root_.transform.Find("ImageBG/Image_xianjin/Button_lingqu").gameObject;
            XPointEvent.AutoAddListener(signbtn, OnSign, null);
            GameObject closebtn = root_.transform.Find("UiRootBG").gameObject;
            XPointEvent.AutoAddListener(closebtn, OnCloseCrashPanel, null);
            GameObject closesign = root_.transform.Find("Pop_up/jiaocheng/UiRootBG").gameObject;
            XPointEvent.AutoAddListener(closesign, OnCloseSign, null);
            GameObject closesignbtn = root_.transform.Find("Pop_up/jiaocheng/ImageBG/ButtonClose").gameObject;
            XPointEvent.AutoAddListener(closesignbtn, OnCloseSign, null);
            GameObject DiamondButton = root_.transform.Find("ImageBG/Image_xianjin/Button_exchange").gameObject;
            XPointEvent.AutoAddListener(DiamondButton, OnDiamondSign, true);
            ExchangePanelTransform = root_.transform.Find("Pop_up/exchange");
            GameObject DiamondCloseButton = ExchangePanelTransform.Find("ImageBG/Button_close").gameObject;
            XPointEvent.AutoAddListener(DiamondCloseButton, OnDiamondSign, false);
            GameObject MaxDiamondButton = ExchangePanelTransform.Find("ImageBG/ButtonMax").gameObject;
            XPointEvent.AutoAddListener(MaxDiamondButton, OnMaxDiamondSign, false);
            RedBagInputField = ExchangePanelTransform.Find("ImageBG/InputField").GetComponent<InputField>();
            RedBagInputField.onValueChanged.AddListener(InputFieldChanged);
            DiamondOkButton = ExchangePanelTransform.Find("ImageBG/ButtonOk").GetComponent<Button>();
            DiamondOkButton.onClick.AddListener(()=> { OnDiamondOKClickEvent(); });
        }

        RefreshMoney();
        root_.transform.Find("ImageBG/Image_xianjin/Textkeling_0").gameObject.SetActive(GameMain.hall_.GetPlayerData().UnreceivedRedBag == 0);
    }

    public void RefreshMoney()
    {
        if (root_ == null)
            return;

        Text Textkelingqu = root_.transform.Find("ImageBG/Image_xianjin/Textkeling/Textnum").gameObject.GetComponent<Text>();
        root_.transform.Find("ImageBG/Image_xianjin/Textkeling").gameObject.SetActive(GameMain.hall_.GetPlayerData().UnreceivedRedBag != 0);
        Textkelingqu.text = GameMain.hall_.GetPlayerData().UnreceivedRedBag.ToString();
        Text Textzongshu = root_.transform.Find("ImageBG/Image_xianjin/Textzongshu/Textnum").gameObject.GetComponent<Text>();
        Textzongshu.text = GameMain.hall_.GetPlayerData().ReceivedRedBag.ToString();
        if(ExchangePanelTransform)
        {
            ExchangePanelTransform.Find("ImageBG/TextNum_Red").GetComponent<Text>().text = "现有红包" + GameMain.hall_.GetPlayerData().UnreceivedRedBag + "元";
        }
    }

    public void ShowCrashPanel()
    {
        LoadCrashRedBagResource();
        root_.SetActive(true);
    }

    private void OnCloseCrashPanel(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);

            //关闭
            root_.SetActive(false);
        }    }

    private void OnShare(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);
            float unrecive = (float)button;
            //分享
            Player.ShareImageToWechat();
            GameObject shareImage = root_.transform.Find("Pop_up/ImageShare").gameObject;
            shareImage.transform.Find("Textzongshu/Textnum").gameObject.GetComponent<Text>().text = unrecive.ToString();
            shareImage.SetActive(true);

            Player.ShareImageRectToWechat(GameObject.Find("Canvas").gameObject.GetComponent<Canvas>(),
                 shareImage.gameObject.GetComponent<RectTransform>());
        }    }

    private void OnSign(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);

            //指引
            GameObject jiaocheng = root_.transform.Find("Pop_up/jiaocheng").gameObject;
            jiaocheng.SetActive(true);
        }    }

    private void OnCloseSign(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);

            //关闭指引
            GameObject jiaocheng = root_.transform.Find("Pop_up/jiaocheng").gameObject;
            jiaocheng.SetActive(false);
        }    }

    //红包兑换钻石事件
    private void OnDiamondSign(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if(ExchangePanelTransform)
            {
                ExchangePanelTransform.gameObject.SetActive((bool)button);
            }
        }    }

    //最大红包事件
    private void OnMaxDiamondSign(EventTriggerType eventtype, object button, PointerEventData eventData)    {        if (eventtype == EventTriggerType.PointerClick)        {            CustomAudio.GetInstance().PlayCustomAudio(1002);
            if(ExchangePanelTransform)
            {
                RedBagInputField.text = Mathf.Floor(GameMain.hall_.GetPlayerData().UnreceivedRedBag).ToString();
            }
        }    }

    //最大红包事件
    private void OnDiamondOKClickEvent()    {
        if(DiamondOkButton == null)
        {
            return;
        }
        DiamondOkButton.interactable = false;
        CustomAudio.GetInstance().PlayCustomAudio(1002);

        UMessage DiamondMsg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CashToDiamond);
        DiamondMsg.Add(GameMain.hall_.GetPlayerId());
        uint valueNum = 0;
        uint.TryParse(RedBagInputField.text, out valueNum);
        DiamondMsg.Add(valueNum);
        NetWorkClient.GetInstance().SendMsg(DiamondMsg);
    }

    /// <summary>
    /// 兑换钻石红包输入框
    /// </summary>
    /// <param name="valueChanged">红包数值</param>
    private void InputFieldChanged(string valueChanged)
    {
        int value = 0;
        int.TryParse(valueChanged, out value);
        if (value <= 0)
        {
            RedBagInputField.text = string.Empty;
            value = 0;
        }
        float curRedBag = Mathf.Floor(GameMain.hall_.GetPlayerData().UnreceivedRedBag);
        if (value > curRedBag)
        {
            RedBagInputField.text = curRedBag.ToString();
            int.TryParse(curRedBag.ToString(), out value);
        }
        if (ExchangePanelTransform)
        {
            value = value < 0 || value > curRedBag ? 0 : value;
            ExchangePanelTransform.Find("ImageBG/TextNum_Diamond").GetComponent<Text>().text = "兑换率110 % " + value + "红包 = " + value * 10 + "钻石 + " + value + "（额外赠送）钻石";
        }
        if(DiamondOkButton)
        {
            DiamondOkButton.interactable = value > 0;
        }
    }

    /// <summary>
    /// 兑换钻石消息
    /// </summary>
    bool BackDiamondCashToDiamond(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        if(state == 1)
        {
            if (DiamondOkButton)
            {
                DiamondOkButton.interactable = true;
            }
            CCustomDialog.OpenCustomConfirmUI(1508);
            return false;
        }

        if (RedBagInputField)
        {
            RedBagInputField.text = string.Empty;
        }
        if(DiamondOkButton)
        {
            DiamondOkButton.interactable = false;
        }
        CCustomDialog.OpenCustomConfirmUI(1507);
        return true;
    }
}
