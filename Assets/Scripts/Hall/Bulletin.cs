using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using USocket.Messages;

public class Bulletin : MonoBehaviour
{
    enum PageType
    {
        ePT_Signin,
        ePT_Mail,
        ePT_Activity,
        ePT_Chess,
    }

    bool m_bNeedUpdate = true;
    bool NeedUpdate
    {
        get
        {
            return m_bNeedUpdate;
        }
        set
        {
            m_bNeedUpdate = value;
            NeedClick = NeedClick;
        }
    }

    byte m_nNeedClick = 0;
    byte NeedClick
    {
        get
        {
            return m_nNeedClick;
        }
        set
        {
            m_nNeedClick = value;

            GameObject obj = GameMain.hall_.contestui_;
            if (obj != null)
                obj.transform.Find("Panelbottom/Bottom/Button_News/ImageSpot").gameObject.SetActive(m_nNeedClick > 0 || NeedUpdate);

            Debug.Log("m_nNeedClick:" + m_nNeedClick);
        }
    }

    Toggle m_SignToggle;
    GameObject m_MailToggle,m_CurContestRankingToggle;
    Button m_SignBtn;

    Dictionary<GameObject, GameObject> m_Announcement = new Dictionary<GameObject, GameObject>();
    Dictionary<GameObject, string> ContestGameDataDictionary = new Dictionary<GameObject, string>();
    Dictionary<PageType, GameObject> PageLiftToggleGroupDictionary = new Dictionary<PageType, GameObject>();
    private void Awake()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
               (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKANNOUNCEMENTDATA, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
               (uint)GameCity.EMSG_ENUM.CrazyCityMsg_ANNOUNCEMENTNEEDUPDATE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
               (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKWEEKOROLDREWORD, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
              (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SendContestData, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_UpdateContestData, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SendContestRankData, HandleGameNetMsg);
        ContestGameDataDictionary.Clear();
        PageLiftToggleGroupDictionary.Clear();
        for (PageType type = PageType.ePT_Signin; type <= PageType.ePT_Chess; ++type)
        {
            PageLiftToggleGroupDictionary.Add(type, null);
        }
        AddFixedPage();
    }

    void Start()
    {
        transform.Find("PanelHead_/Button_Return").GetComponent<Button>()
            .onClick.AddListener(OnClickReturn);
    }

    private void OnEnable()
    {
        RequestAnnouncement();
    }

    void AddFixedPage()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);

        AddPage(bundle, "签到", PageType.ePT_Signin, null, true, (t, page) =>
        {
            m_SignToggle = t;

            if (page != null)
            {
                PlayerData pd = GameMain.hall_.GetPlayerData();
                m_SignBtn = page.transform.Find("ButtonLingqu").GetComponent<Button>();
                m_SignBtn.interactable = (pd.NeedSign != 0);
                m_SignBtn.onClick.AddListener(OnClickSign);

                page.transform.Find("TextNum").GetComponent<Text>().text = pd.SignAward.ToString();
            }
        });

        AddPage(bundle, "邮件", PageType.ePT_Mail);
    }

    void RequestAnnouncement()
    {
        if (!NeedUpdate)
            return;

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_APPLYANNOUNCEMENTDATA);
        NetWorkClient.GetInstance().SendMsg(msg);
    }

    void ResetAnnouncement()
    {
        StopAllCoroutines();

        foreach (var v in m_Announcement)
        {
            if (v.Key.transform.Find("ImageSpot").gameObject.activeSelf)
                NeedClick--;

            Destroy(v.Key);
            Destroy(v.Value);
        }

        m_Announcement.Clear();
        m_SignToggle.isOn = true;
    }

    bool HandleGameNetMsg(uint _msgType, UMessage _ms)
    {
        if (GameMain.hall_.m_Bulletin == null)
            return false;

        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;
        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CrazyCityMsg_BACKANNOUNCEMENTDATA:
                {
                    ResetAnnouncement();
                    NeedUpdate = false;

                    AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
                    if (bundle == null)
                        return false;

                    byte num = _ms.ReadByte();
                    for (int i = 0; i < num; i++)
                    {
                        string name = _ms.ReadString();
                        string url = _ms.ReadString();
                        AddPage(bundle, name, PageType.ePT_Activity, url);
                    }
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_ANNOUNCEMENTNEEDUPDATE:
                {
                    NeedUpdate = true;
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_BACKWEEKOROLDREWORD:
                {
                    byte sign = _ms.ReadByte();//1：签到或累计登陆 2:老用户
                    long coin = _ms.ReadLong();
                    long addCoin = _ms.ReadLong();

                    byte state1 = _ms.ReadByte();//0:成功 1：已领取
                    byte reward1 = _ms.ReadByte();

                    if (sign == 1)
                    {
                        m_SignBtn.interactable = (state1 != 0);
                        m_SignToggle.transform.Find("ImageSpot").
                            gameObject.SetActive(m_SignBtn.interactable);
                        GameMain.hall_.GetPlayerData().NeedSign = state1;
                        if (state1 == 0)
                            NeedClick--;
                    }
                }
                break;
            case GameCity.EMSG_ENUM.CrazyCityMsg_SendContestData://发送玩家创建比赛数据
            case GameCity.EMSG_ENUM.CrazyCityMsg_UpdateContestData:
                {
                    byte ContestNumber = 1;//比赛数量
                    List<long> ContestIDList = null;
                    if (eMsg == GameCity.EMSG_ENUM.CrazyCityMsg_SendContestData)
                    {
                        ContestNumber = _ms.ReadByte();
                        ContestIDList = new List<long>();
                    }
                   
                    for(byte index = 0; index < ContestNumber;++index)
                    {
                        long ConstestID = _ms.ReadLong();//排行榜ID
                        uint ContestPlayerNum = _ms.ReadUInt();//比赛人数
                        byte GameId = _ms.ReadByte();//游戏ID
                        byte ReadState = _ms.ReadByte();//当前记录是否读取
                        if(ContestIDList != null)
                        {
                            ContestIDList.Add(ConstestID);
                        }
                        AddGameRankingPage(ConstestID, GameId, ReadState == 1,ContestPlayerNum);
                    }
                    if(ContestIDList != null)
                    {
                        CGameContestRankingTifings.GetChessRankingInstance().LoadContestRankingData(ContestIDList);
                    }
                }
                break;
            case GameCity.EMSG_ENUM.CrazyCityMsg_SendContestRankData:
                {
                   bool retValue =  CGameContestRankingTifings.GetChessRankingInstance().UpdateGameRankingData(_ms);
                    if(retValue && m_CurContestRankingToggle != null)
                    {
                        GameObject SpotObject = m_CurContestRankingToggle.transform.Find("ImageSpot").gameObject;
                        if (SpotObject.activeSelf)
                        {
                            NeedClick--;
                            SpotObject.SetActive(false);
                        }
                    }
                }
                break;
            default:
                break;
        }

        return true;
    }

    void Update()
    {
        CGameContestRankingTifings.GetChessRankingInstance().Update();
    }

    void OnClickReturn()
    {
        CustomAudio.GetInstance().PlayCustomAudio(1002);
        gameObject.SetActive(false);
        CGameContestRankingTifings.GetChessRankingInstance(false).SaveContestRankingData();
    }

    void AddPage(AssetBundle bundle, string name, PageType pt, object param = null, 
                 bool pageOn = false, UnityAction<Toggle, GameObject> fun = null,bool bReadState = true)
    {
        if (bundle == null)
            return;

        GameObject obj = (GameObject)bundle.LoadAsset("Lobby_News_Toggle");
        obj = (GameObject)GameMain.instantiate(obj);
        ToggleGroup tg = transform.Find("PanelDown/Lobby_News_Left/Content").GetComponent<ToggleGroup>();
        obj.transform.SetParent(tg.transform, false);
        obj.transform.Find("Label").GetComponent<Text>().text = name;

        if(PageLiftToggleGroupDictionary.ContainsKey(pt))
        {
            if (PageLiftToggleGroupDictionary[pt] != null)
            {
                for(int index = 0; index < tg.transform.childCount; ++index)
                {
                    if(PageLiftToggleGroupDictionary[pt] == tg.transform.GetChild(index).gameObject)
                    {
                        if(pt == PageType.ePT_Chess)
                        {
                            long oldContestID = 0 , newContestID = (long)param;
                            long.TryParse(PageLiftToggleGroupDictionary[pt].name,out oldContestID);
                            if(oldContestID > newContestID)
                            {
                                obj.transform.SetSiblingIndex(index+1);
                                break;
                            }
                        }
                        obj.transform.SetSiblingIndex(index);
                        break;
                    }
                }
            }
            PageLiftToggleGroupDictionary[pt] = obj;
        }
        GameObject assetObj = null;
        GameObject objR = null;
        bool clicked = bReadState;
        switch (pt)
        {
            case PageType.ePT_Signin:
                {
                    assetObj = (GameObject)bundle.LoadAsset("Lobby_News_signin");
                    clicked = (GameMain.hall_.GetPlayerData().NeedSign == 0);
                }
                break;
            case PageType.ePT_Mail:
                {
                    if (Email.GetEmailInstance().root_ == null)
                        Email.GetEmailInstance().LoadNewsResource();
                    objR = Email.GetEmailInstance().root_;
                    clicked = (GameMain.hall_.GetPlayerData().mailNumber <= 0);
                    m_MailToggle = obj;
                }
                break;
            case PageType.ePT_Chess:
                {
                    objR = CGameContestRankingTifings.GetChessRankingInstance().GetMainTransform().gameObject;
                }
                break;
            default:
                {
                    assetObj = (GameObject)bundle.LoadAsset("Lobby_News_Activity");
                    clicked = StateStorage.LoadData<bool>(pt + ":" + name);
                }
                break;
        }

        if (assetObj != null)
            objR = (GameObject)GameMain.instantiate(assetObj);

        if(objR != null)
        { 
            objR.transform.SetParent(transform.Find("PanelDown/Lobby_News_Right"), false);

            bool ActiveState = pageOn;
            if (pt == PageType.ePT_Activity)
            {
                GameMain.SC(LoadActivityImage(objR, (string)param));
                m_Announcement[obj] = objR;
            }else if(pt == PageType.ePT_Chess)
            {
                ActiveState = objR.activeSelf;
            }

            objR.SetActive(ActiveState);
        }

        Toggle t = obj.GetComponent<Toggle>();
        t.group = tg;
        t.isOn = pageOn;
        t.onValueChanged.AddListener((isOn) => OnToggleChanged(t, name, pt, isOn, objR));

        obj.transform.Find("ImageSpot").gameObject.SetActive(!clicked);

        if (!clicked)
            NeedClick++;

        if (fun != null)
            fun.Invoke(t, objR);
    }

    void OnToggleChanged(Toggle t, string name, PageType pt, bool isOn, GameObject child)
    {
        if(isOn)
        {
            m_CurContestRankingToggle = null;
            CustomAudio.GetInstance().PlayCustomAudio(1003);
            if(pt == PageType.ePT_Chess)
            {
                m_CurContestRankingToggle = t.gameObject;
                if (ContestGameDataDictionary.ContainsKey(t.gameObject))
                {
                   string [] ValueString = ContestGameDataDictionary[t.gameObject].Split('|');
                    if(ValueString.Length == 2)
                    {
                        long ContestID = 0;
                        long.TryParse(ValueString[0],out ContestID);
                        int PlayetNumber = 0;
                        int.TryParse(ValueString[1], out PlayetNumber);
                        CGameContestRankingTifings.GetChessRankingInstance().SetSelectContestID(ContestID);
                        CGameContestRankingTifings.GetChessRankingInstance().CreateGameRankingPlayerUIOjbect(PlayetNumber,true);
                    }
                }
                CGameContestRankingTifings.GetChessRankingInstance().ResetChessRanking();
            }
            else if (pt == PageType.ePT_Activity)
            {
                GameObject obj = t.transform.Find("ImageSpot").gameObject;
                if (obj.activeSelf)
                {
                    NeedClick--;
                    obj.SetActive(false);
                    StateStorage.SaveData(pt + ":" + name, true);
                }
            }
        }

        if (child != null)
        {
            child.SetActive(isOn);
        }
    }

    void OnClickSign()
    {
        CustomAudio.GetInstance().PlayCustomAudio(1002);

        m_SignBtn.interactable = false;

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_GETWEEKOROLDREWORD);
        msg.Add((byte)1);
        msg.Add(GameMain.hall_.GetPlayerId());
        NetWorkClient.GetInstance().SendMsg(msg);
    }

    IEnumerator LoadActivityImage(GameObject acObj, string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (acObj == null)
            yield break;

        Transform tfm = acObj.transform.Find("Content/ImageNews");
        if (tfm == null)
            yield break;

        Image img = tfm.GetComponent<Image>();
        if (img == null)
            yield break;
        RectTransform RightRectTransform = acObj.transform.GetComponent<RectTransform>();
        float MaxAspectRation = RightRectTransform.rect.width / RightRectTransform.rect.height;

        AspectRatioFitter aspectRationFitter = tfm.GetComponent<AspectRatioFitter>();
        aspectRationFitter.aspectRatio = Mathf.Clamp(aspectRationFitter.aspectRatio, 0, MaxAspectRation);
        img.preserveAspect = aspectRationFitter.aspectRatio < MaxAspectRation;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            yield break;
        }

        Texture2D tex = www.texture;
        img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        //活动图片等比例缩放
        aspectRationFitter.aspectRatio = Mathf.Clamp(img.sprite.rect.width / img.sprite.rect.height,0, MaxAspectRation);
        img.preserveAspect = aspectRationFitter.aspectRatio < MaxAspectRation;
    }

    void ShowMailPage()
    {
        Email.GetEmailInstance().ShowNewsPanel();
    }

    public void OnEmailChange(bool isempty)
    {
        if (isempty)
        {
            NeedClick--;
            m_MailToggle.transform.Find("ImageSpot").gameObject.SetActive(false);
            return;
        }
            
        if(m_MailToggle != null)
        {
            if (!m_MailToggle.transform.Find("ImageSpot").gameObject.activeSelf)
            {
                NeedClick++;
                m_MailToggle.transform.Find("ImageSpot").gameObject.SetActive(true);
            }
        }
    }

    void AddGameRankingPage(long ContestID,byte GameKind,bool ReadState,uint PlayerNumber,bool PageOnState = false)
    {
        string GameContestValue = string.Format("{0}|{1}", ContestID, PlayerNumber);
        if (ContestGameDataDictionary.ContainsValue(GameContestValue))
        {
            return;
        }
        string ContestStrData = ContestID.ToString();
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(GameKind);
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        AddPage(bundle, gamedata.GameName, PageType.ePT_Chess, ContestID, PageOnState, (t, page) => {
            t.name = ContestStrData;
            ContestGameDataDictionary.Add(t.gameObject,GameContestValue);
            t.transform.Find("text_time").GetComponent<Text>().text = string.Format("{0}-{1}-{2} {3}:{4}",
                ContestStrData.Substring(0,4), ContestStrData.Substring(4,2), ContestStrData.Substring(6, 2),
                ContestStrData.Substring(8,2), ContestStrData.Substring(10, 2));
        }, ReadState);
    }
}
