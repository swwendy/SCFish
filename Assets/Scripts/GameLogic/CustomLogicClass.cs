using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;[Hotfix]public class CustomCountdownImgMgr{    public delegate bool CallBackFunc(byte value, bool bClick, Image img, string userdata);    public struct CountdownImgInfo    {        public float time;        public Text text;        public GameObject hideObjOnRemove;        public string userdata;        public CallBackFunc callFuc;    }    Dictionary<Image, CountdownImgInfo> m_TimeImageDict = new Dictionary<Image, CountdownImgInfo>();    HashSet<Image> m_PreRemoveImgList = new HashSet<Image>();    public CustomCountdownImgMgr()    {        m_TimeImageDict.Clear();        m_PreRemoveImgList.Clear();    }    public void AddTimeImage(Image img, float time, float fill, CallBackFunc fun, Text t = null, bool bHaveBtn = false, string userdata = "", GameObject hideObjOnRemove = null)    {        img.fillAmount = fill;        m_PreRemoveImgList.Remove(img);        if (!m_TimeImageDict.ContainsKey(img))        {            CountdownImgInfo info = new CountdownImgInfo();            info.time = time;            info.callFuc = fun;            info.text = t;            info.hideObjOnRemove = hideObjOnRemove;            info.userdata = userdata;            if(bHaveBtn)            {                Button[] btns = img.transform.parent.GetComponentsInChildren<Button>();                if (btns.Length > 0)                {                    btns[0].onClick.RemoveAllListeners();                    btns[0].onClick.AddListener(() =>                    {                        fun(1, true, img, info.userdata);                        m_PreRemoveImgList.Add(img);                    });                }                if (btns.Length > 1)                {                    btns[1].onClick.RemoveAllListeners();                    btns[1].onClick.AddListener(() =>                    {                        fun(0, true, img, info.userdata);                        m_PreRemoveImgList.Add(img);                    });                }            }            m_TimeImageDict.Add(img, info);        }        else        {            CountdownImgInfo info = m_TimeImageDict[img];            info.time = time;            info.callFuc = fun;            info.text = t;            info.hideObjOnRemove = hideObjOnRemove;            info.userdata = userdata;            m_TimeImageDict[img] = info;        }    }    public void RemoveTimeImage(Image img, bool bCallFun = false)    {        if (m_TimeImageDict.ContainsKey(img))        {            CountdownImgInfo info = m_TimeImageDict[img];            img.fillAmount = 0;            bool bRemove = true;            if(bCallFun && info.callFuc != null)                bRemove = info.callFuc(0, false, img, info.userdata);            if(bRemove)                m_PreRemoveImgList.Add(img);        }    }    public void UpdateTimeImage()    {        foreach (Image img in m_PreRemoveImgList)        {            if(m_TimeImageDict.ContainsKey(img))            {
                if (m_TimeImageDict[img].hideObjOnRemove != null)
                    m_TimeImageDict[img].hideObjOnRemove.SetActive(false);

                m_TimeImageDict.Remove(img);            }        }        m_PreRemoveImgList.Clear();        List<Image> list = new List<Image>(m_TimeImageDict.Keys);        foreach (Image img in list)        {            CountdownImgInfo info = m_TimeImageDict[img];            if(info.text)            {                int sec = (int)System.Math.Ceiling(img.fillAmount * info.time);                info.text.text = sec.ToString();            }            img.fillAmount -= Time.unscaledDeltaTime / info.time;            if (img.fillAmount <= 0.0f)            {                img.fillAmount = 0.0f;

                if (info.text)
                    info.text.text = "0";

                bool bRemove = true;                if (info.callFuc != null)                    bRemove = info.callFuc(0, false, img, info.userdata);

                if (bRemove)
                    m_PreRemoveImgList.Add(img);            }        }    }}[Hotfix]public class CustomAudioDataManager{    byte m_nGameId = 0;    public void ReadAudioCsvData(byte gameId, string configFile)    {        m_nGameId = gameId;        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, configFile, out strList);        m_SoundList.Clear();        int columnCount = strList.Count;        int id = 0;        for (int i = 2; i < columnCount; i++)        {            int.TryParse(strList[i][0], out id);            m_SoundList.Add(id, strList[i][1]);        }    }    public static CustomAudioDataManager GetInstance()    {        if (instance_ == null)        {            instance_ = new CustomAudioDataManager();        }        return instance_;    }    public void PlayAudio(string soundName, bool isSound = true, bool forceStopPlaying = false)
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData(m_nGameId);        if (gamedata != null)
        {            if (isSound)                AudioManager.Instance.PlaySound(gamedata.ResourceABName, soundName);            else                AudioManager.Instance.PlayBGMusic(gamedata.ResourceABName, soundName, true, forceStopPlaying);
        }    }    public void PlayAudio(int id, bool isSound = true, bool forceStopPlaying = false)    {        if (m_SoundList.ContainsKey(id))        {            PlayAudio(m_SoundList[id], isSound, forceStopPlaying);        }        else            DebugLog.Log("音效资源:" + id + " 加载失败");    }    public void StopAudio(bool isBGM = true)
    {
        if (isBGM)
            AudioManager.Instance.StopMusic();        else            AudioManager.Instance.StopSound();    }    public string GetSoundName(int id)
    {
        if (m_SoundList.ContainsKey(id))
            return m_SoundList[id];

        return "";
    }    private static CustomAudioDataManager instance_;    Dictionary<int, string> m_SoundList = new Dictionary<int, string>();}[Hotfix]public class PlayCard : MonoBehaviour
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
        {            bChangeTo = bSelected = value;
            RectTransform tfm = transform as RectTransform;
            Vector3 vec = tfm.localPosition;
            float offset = tfm.rect.height * 0.1f;
            vec.y = bSelected ? (fSrcY + offset) : fSrcY;
            transform.localPosition = vec;
            bMoving = false;
        }        else if(!bMoving)
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