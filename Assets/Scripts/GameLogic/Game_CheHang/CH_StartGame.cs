using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using USocket.Messages;
using DragonBones;
using XLua;[Hotfix]
public class CH_StartGame
{
    // Use this for initialization
    int currentPostion_;
    int targetPosition_;
    Vector4 turnPosition_;
    float round_ = 0.0f;

    float lowTime_;
    float highTime_;
    float currentTime_;
    float addTime_;
    float currentStepTime_;

    bool isStart_;
    bool isEnd_;
    bool isStop_;
    bool gameover_;
    bool goingtotarget_;

    bool isShowBigWin;
    float bigwinTime;

    //float duration_;
    List<GameObject> DisableCoronas_;

    List<GameObject> coronas_;
    List<GameObject> chooses_;
    float speed_ = 10.0f;

    GameObject gameUI_;
    GameObject overlist_;

    Dictionary<int, GameObject> resultShines_;
    //bool isShowResult_;
    //float resultTime_;

    public SortData sd;
    float timeCount_;

    public CH_StartGame(GameObject gameUI, GameObject overlist)
    {
        gameUI_ = gameUI;
        overlist_ = overlist;
    }

    public void Start () {
        targetPosition_ = (int)CH_DataCenter.Instance().result.carIndex;
        currentPostion_ = Random.Range(0,19);
        coronas_ = new List<GameObject>();
        chooses_ = new List<GameObject>();
        DisableCoronas_ = new List<GameObject>();
        resultShines_ = new Dictionary<int, GameObject>();
        sd = new SortData();
        turnPosition_ = new Vector4(1, 8, 11, 18);

        for (int i = 0; i < 20; i++)
        {
            string index;
            if (i < 9)
                index = "0" + (i + 1).ToString();
            else
                index = (i + 1).ToString();
            GameObject corona = gameUI_.transform.Find("ImageIconBG_" + index).Find("ImageShine").gameObject;
            corona.SetActive(false);
            coronas_.Add(corona);

            GameObject choose = gameUI_.transform.Find("ImageIconBG_" + index).Find("ImageChoose").gameObject;
            choose.SetActive(false);
            chooses_.Add(choose);
        }

        //panel_ = GameObject.Find("panel");

        lowTime_ = 0.4f;
        highTime_ = 0.03f;
        currentTime_ = 0.0f;
        addTime_ = 0.2f;
        currentStepTime_ = lowTime_;
        timeCount_ = 0.0f;

        resetgame();
    }

    // Update is called once per frame
    public void Update ()
    {
        DisableCoronas(Time.deltaTime * speed_);

        if (isStartedOver)
        {
            if (!isPlayed)
            {
                currentAnimTime += Time.deltaTime;
                if (currentAnimTime >= overAnimTime)
                {
                    isPlayed = true;
                    currentAnimTime = 0.0f;
                    initOverList();
                }
            }
        }

        if (isStop_)
        {
            if (!gameover_)
            {
                ShowResult();
                //ShowBigWinIcon();
                gameover_ = true;
            }

            //if(isShowResult_)
            //{
            //    resultTime_ += Time.deltaTime;
            //    if (resultTime_ >= 2.0f)
            //    {
            //        ShowOverList();
            //        isShowResult_ = false;
            //    }
            //}
            return; 
        }

        if(isShowBigWin)
        {
            bigwinTime += Time.deltaTime;
            if(bigwinTime > 2.0f)
            {
                CloseBigWinIcon();
                bigwinTime = 0.0f;
                isShowBigWin = false;
            }
        }

        GameLogic(Time.deltaTime);
    }

    void initIndex2ChePai(Dictionary<int, int> index2chepai)
    {
        index2chepai.Add(1, 6);
        index2chepai.Add(2, 2);
        index2chepai.Add(3, 1);
        index2chepai.Add(4, 3);
        index2chepai.Add(5, 4);
        index2chepai.Add(6, 2);
        index2chepai.Add(7, 1);
        index2chepai.Add(8, 5);
        index2chepai.Add(9, 3);
        index2chepai.Add(10, 4);
        index2chepai.Add(11, 7);
        index2chepai.Add(12, 2);
        index2chepai.Add(13, 1);
        index2chepai.Add(14, 3);
        index2chepai.Add(15, 4);
        index2chepai.Add(16, 2);
        index2chepai.Add(17, 1);
        index2chepai.Add(18, 8);
        index2chepai.Add(19, 3);
        index2chepai.Add(20, 4);
    }

    void ShowResult()
    {
        GameObject resultTwo;
        GameObject result;

        Dictionary<int, int> index2chepai = new Dictionary<int, int>();
        initIndex2ChePai(index2chepai);
        result = resultShines_[index2chepai[targetPosition_]];
        if (CH_DataCenter.Instance().result.two == (char)1)
        {
            resultTwo = resultShines_[9];
        }
        else
        {
            resultTwo = resultShines_[10];
        }
        
        resultTwo.SetActive(true);
        result.SetActive(true);
        CH_DataCenter.Instance().state.roomLevel = 0;
        CH_DataCenter.Instance().histroy.icons.Add((char)index2chepai[targetPosition_]);
        HistroyTimes ht = new HistroyTimes();
        ht.iconID = (char)(index2chepai[targetPosition_] - 1);
        ht.times = CH_DataCenter.Instance().histroy.times[ht.iconID].times + 1;
        CH_DataCenter.Instance().histroy.times[index2chepai[targetPosition_] - 1] = ht;
        //isShowResult_ = true;
    }

    public void resetgame()
    {
        isStart_ = true;
        isEnd_ = false;
        isStop_ = false;
        goingtotarget_ = false;
        gameover_ = false;
        isShowBigWin = false;
        //isShowResult_ = false;
        round_ = 0.0f;
        //resultTime_ = 0.0f;
        timeCount_ = -0.0f;
        targetPosition_ = (int)CH_DataCenter.Instance().result.carIndex;
        //GameObject resultPanel = overlist_.transform.FindChild("ResultBG").gameObject;
        //GameObject resultPanel = overlist_;
        //GameObject BigWin = resultPanel.transform.FindChild("BigWin").gameObject;
        //BigWin.SetActive(false);
        foreach (GameObject choose in chooses_)
            choose.SetActive(false);

        isStartedOver = false;
        isPlayed = false;
    }

    void caculate(float deltatime)
    {
        currentTime_ += deltatime;

        timeCount_ += deltatime;
        if (round_ < 6 && timeCount_ >= 1.0f)
        {
            GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
            if (gamedata != null)
                AudioManager.Instance.PlaySound(gamedata.ResourceABName, "sgj_loop");
            else
                Debug.Log("游戏id:1不存在");
            timeCount_ = 0.0f;
        }

        if (round_ >= 6 && timeCount_ >= 1.0f)
        {
            GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
            if (gamedata != null)
                AudioManager.Instance.PlaySound(gamedata.ResourceABName, "sgj_end");
            else
                Debug.Log("游戏id:1不存在");
            
            timeCount_ = -100.0f;
        }

        if (currentTime_ >= currentStepTime_)
        {
            currentStepTime_ += addTime_;
            if (currentStepTime_ < highTime_ && addTime_ < 0)
            {
                currentStepTime_ = highTime_;
            }
            if (currentStepTime_ >= lowTime_ && addTime_ > 0)
                currentStepTime_ = lowTime_;

            DisableCoronas_.Add(coronas_[currentPostion_]);
            currentPostion_ += 1;
            //Debug.Log(currentPostion_);
            //AudioManager.Instance.PlaySound("car.resource", "chehang_overSound");
            round_ += 0.05f;

            if (currentPostion_ > 19)
                currentPostion_ = 0;

            if (round_ >= 6)
            {
                int step = targetPosition_ - currentPostion_;
                if (step < 0)
                    step += 20;
                if (step > 3 && step < 8)
                {
                    goingtotarget_ = true;

                    isEnd_ = true;
                    isStart_ = false;
                }
            }

            GotoTarget();
            coronas_[currentPostion_].SetActive(true);
            resetCorona();

            currentTime_ = 0;
        }
    }

    void DisableCoronas(float deltatime)
    {
        foreach (GameObject corona in DisableCoronas_)
        {
            UnityEngine.UI.Image image = corona.GetComponent<UnityEngine.UI.Image>();
            Color end = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
            image.color = Color.Lerp(image.color, end, deltatime);
        }
    }

    void resetCorona()
    {
        UnityEngine.UI.Image image = coronas_[currentPostion_].GetComponent<UnityEngine.UI.Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
        DisableCoronas_.Remove(coronas_[currentPostion_]);
    }

    void GotoTarget()
    {
        if (!goingtotarget_)
            return;

        if (currentPostion_ == targetPosition_ - 1)
        {
            isStop_ = true;
            chooses_[currentPostion_].SetActive(true);
        }
    }

    public void TargetPosition( int position )
    {
        targetPosition_ = position;
    }

    public int TargetPosition()
    {
        return targetPosition_;
    }

    void GameLogic( float deltatime )
    {
        if (isStart_)
        {
            addTime_ = -0.1f;
            caculate(deltatime);
        }

        if( isEnd_ )
        {
            addTime_ = 0.1f;
            caculate(deltatime);
        }
    }

    Text GetTextByTag(string name)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(name);
        Text text = obj.GetComponent<Text>();
        return text;
    }

    public void showYazhuInfo( List<YazhuButtonStruct> buttons )
    {
        GameObject bet = gameUI_.transform.Find("BetNum").gameObject;
        resultShines_.Clear();
        Dictionary<int, string> two1 = new Dictionary<int, string>();
        two1.Add(5, "05");
        two1.Add(6, "06");
        two1.Add(7, "07");
        two1.Add(8, "08");
        two1.Add(9, "09");
        foreach (KeyValuePair<int, string> kvp in two1)
        {
            Text temp = bet.transform.Find("BetNum1").
                Find("ImageBG_YaZhu_" + kvp.Value).
                Find("TextNum").gameObject.GetComponent<Text>();
            if( (kvp.Key - 1)< buttons.Count )
            {
                if (GameMain.hall_.GetPlayerId() == CH_DataCenter.Instance().state.currentBossID)
                    temp.text = buttons[kvp.Key - 1].totalMoney.ToString();
                else
                    temp.text = buttons[kvp.Key - 1].selfMoney.ToString();
            }
                

            GameObject shine = bet.transform.Find("BetNum1").
                Find("ImageBG_YaZhu_" + kvp.Value).
                Find("ImageShine").gameObject;
            shine.SetActive(false);
            resultShines_.Add(kvp.Key,shine);
        }

        Dictionary<int, string> two2 = new Dictionary<int, string>();
        two2.Add(1, "01");
        two2.Add(2, "02");
        two2.Add(3, "03");
        two2.Add(4, "04");
        two2.Add(10, "10");
        foreach (KeyValuePair<int, string> kvp in two2)
        {
            Text temp = bet.transform.Find("BetNum2").
                Find("ImageBG_YaZhu_" + kvp.Value).
                Find("TextNum").gameObject.GetComponent<Text>();
            if ((kvp.Key - 1) >= buttons.Count)
            {
                Debug.Log("out buttons array index:" + (kvp.Key - 1) + "buttons count:" + buttons.Count);
            }
            if ((kvp.Key - 1) < buttons.Count)
            {
                if (GameMain.hall_.GetPlayerId() == CH_DataCenter.Instance().state.currentBossID)
                    temp.text = buttons[kvp.Key - 1].totalMoney.ToString();
                else
                    temp.text = buttons[kvp.Key - 1].selfMoney.ToString();
            }
                

            GameObject shine = bet.transform.Find("BetNum2").
                Find("ImageBG_YaZhu_" + kvp.Value).
                Find("ImageShine").gameObject;

            shine.SetActive(false);

            resultShines_.Add(kvp.Key,shine);
        }
    }

    public void ShowOverList()
    {
        //initOverList();
        PlayOverAnimator();
        overlist_.SetActive(true);
        //Button tempbtn = overlist_.GetComponent<Button>();
        //tempbtn.onClick.AddListener(
        //delegate ()
        //{
        //    CloseOverlist();
        //});
    }

    public void CloseOverlist()
    {
        overlist_.SetActive(false);
        if (GameMain.hall_.isGetRelief)
        {
            GameMain.hall_.ShowRelief();
        }
    }

    public bool AccessResultData(UMessage msg)
    {
        sd.ReadData(msg);
        ShowOverList();
        return true;
    }

    void CloseBigWinIcon()
    {
        GameObject resultPanel = overlist_.transform.Find("ResultBG").gameObject;
        GameObject BigWin = resultPanel.transform.Find("BigWin").gameObject;
        BigWin.SetActive(false);
    }

    void ShowBigWinIcon()
    {
        GameObject resultPanel = overlist_.transform.Find("ResultBG").gameObject;
        if (targetPosition_ == (int)turnPosition_.x || targetPosition_ == (int)turnPosition_.y ||
    targetPosition_ == (int)turnPosition_.z || targetPosition_ == (int)turnPosition_.w)
        {
            GameObject BigWin = resultPanel.transform.Find("BigWin").gameObject;
            Animator wa = BigWin.GetComponent<Animator>();
            if (wa != null)
            {
                wa.Play(0);
            }
            Image icon = BigWin.transform.Find("ImageIcon").gameObject.GetComponent<Image>();

            BigWin.SetActive(true);
            string index;
            if (targetPosition_ < 10)
                index = "0" + targetPosition_.ToString();
            else
                index = targetPosition_.ToString();

            Image targetImg = gameUI_.transform.Find("ImageIconBG_" + index).Find("ImageIcon").gameObject.GetComponent<Image>();
            icon.sprite = targetImg.sprite;

            isShowBigWin = true;
        }
    }

    bool isPlayed = false;
    float overAnimTime = 1.5f;
    float currentAnimTime = 0.0f;
    bool isStartedOver = false;
    UnityArmatureComponent overAnimate;

    void PlayOverAnimator()
    {
        isStartedOver = true;
        CH_DataCenter.Instance().state.infos.Clear();
        for (int dindex = 0; dindex < (int)CH_DataCenter.Instance().state.carIcon; dindex++)
        {
            CarInfo info = new CarInfo();
            info.carID = (char)dindex;
            info.totalMoney = 0;
            info.selfMoney = 0;
            CH_DataCenter.Instance().state.infos.Add(info);
        }
        Image resultIcon = overlist_.transform.Find("ImageIconBG").Find("ImageIcon").gameObject.GetComponent<Image>();
        string index;
        if (targetPosition_ < 10)
            index = "0" + targetPosition_.ToString();
        else
            index = targetPosition_.ToString();

        Image targetImg = gameUI_.transform.Find("ImageIconBG_" + index).Find("ImageIcon").gameObject.GetComponent<Image>();
        resultIcon.sprite = targetImg.sprite;

        GameObject resultList = overlist_.transform.Find("ImageRanking").gameObject;
        overlist_.transform.Find("ImageEarningBG").gameObject.SetActive(false);
        resultList.SetActive(false);
        if (overAnimate == null)
        {
            GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_CarPort);
            if (gamedata == null)
            {
                Debug.Log("游戏id:1不存在");
                return;
            }
            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
            if (bundle == null)
                return;

            //GameObject overAnimObj = overlist_.transform.FindChild("Anime_Car_Result").gameObject;
            //UnityArmatureComponent overAnimate = overAnimObj.GetComponent<UnityArmatureComponent>();
            Object overAnimateObj = (GameObject)bundle.LoadAsset("Anime_Car_Result");
            GameObject over = (GameObject)GameMain.instantiate(overAnimateObj);
            over.transform.SetParent(overlist_.transform.Find("ImageAnimation"), false);
            //over.transform.localPosition = Vector3.zero;
            //over.transform.localRotation = Quaternion.Euler(Vector3.zero);
            //over.transform.localScale = Vector3.one;
            overAnimate = over.GetComponent<UnityArmatureComponent>();
        }
        overAnimate.animation.Play("newAnimation");
    }

    void initOverList()
    {
        //GameObject resultPanel = overlist_.transform.FindChild("ResultBG").gameObject;
        GameObject resultList = overlist_.transform.Find("ImageRanking").gameObject;
        
        Text playermoney = overlist_.transform.Find("ImageEarningBG").Find("Text_Earning").
            Find("TextNum").gameObject.GetComponent<Text>();
        if (GameMain.hall_.GetPlayerId() == CH_DataCenter.Instance().state.currentBossID)
            playermoney.text = sd.god.nBossAdd.ToString();
        else
            playermoney.text = sd.god.nSelfAdd.ToString();
        //Text sort = overlist_.transform.FindChild("Player").FindChild("paiming").gameObject.GetComponent<Text>();
        //sort.text = sd.god.nSelfRank.ToString();

        Image bossicon = resultList.transform.Find("Dealer").Find("Image_HeadFram").Find("Image_Mask").Find("Image_Head").gameObject.GetComponent<Image>();

        bossicon.sprite = GameMain.hall_.GetIcon(CH_DataCenter.Instance().state.url, CH_DataCenter.Instance().state.currentBossID, (int)CH_DataCenter.Instance().state.faceID);

        Text bossname = resultList.transform.Find("Dealer").Find("TextName").gameObject.GetComponent<Text>();
        bossname.text = CH_DataCenter.Instance().state.bossName;
        Text bossmoney = resultList.transform.Find("Dealer").Find("Image_coinframe").
            Find("Text_Coin").gameObject.GetComponent<Text>();
        bossmoney.text = sd.god.nBossAdd.ToString();

        string resultName;
        for(int index = 0; index < 3; index++)
        {
            resultName = "ResultInfo" + (index + 1).ToString();

            if (index >= (int)sd.number)
            {
                resultList.transform.Find(resultName).gameObject.SetActive(false);
                continue;
            }
            Image icon = resultList.transform.Find(resultName).Find("Image_HeadFram").Find("Image_Mask").Find("Image_Head").gameObject.GetComponent<Image>();
            icon.sprite = GameMain.hall_.GetIcon(sd.si[index].url, sd.si[index].playerid, (int)sd.si[index].faceid);
            resultList.transform.Find(resultName).gameObject.SetActive(true);
            Text name = resultList.transform.Find(resultName).Find("TextName").gameObject.GetComponent<Text>();
            name.text = sd.si[index].name;
            Text money = resultList.transform.Find(resultName).Find("Image_coinframe").
                Find("Text_Coin").gameObject.GetComponent<Text>();
            money.text = sd.si[index].money.ToString();
        }
        resultList.SetActive(true);
        overlist_.transform.Find("ImageEarningBG").gameObject.SetActive(true);
    }
}
