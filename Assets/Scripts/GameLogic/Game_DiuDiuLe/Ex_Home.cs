using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DragonBones;
using System;
using USocket.Messages;
using UnityEngine.EventSystems;
using XLua;

[Hotfix]
public class Ex_Home : Ex_IBaseUI
{
    GameObject homeUI;                      //丢丢乐大厅界面
    GameObject go_bg;                       //背景
    GameObject go_entrance;                 //场次按钮
    GameObject modeUI;                      //模式选择
    GameObject putongUI;                    //普通模式
    GameObject dabiaoUI;                    //达标模式
    GameObject go_topBtn;                   //上部按钮区域
    GameObject go_playerInfo;               //玩家信息
    GameObject go_playerHead;               //玩家头像
    GameObject go_returnBtn;                //返回按钮
    GameObject go_settingBtn;               //设置按钮
    GameObject go_ruleBtn;                  //游戏规则按钮
    GameObject go_matchUI;                   //匹配界面
    GameObject go_mask;                     //蒙黑界面（设置、规则）
    GameObject go_ruleUI;                   //规则界面
    GameObject go_rule_image;               //规则界面底图
    GameObject go_rule_startBtn;            //规则界面开始游戏按钮
    GameObject go_settingUI;                //设置界面
    Toggle toggle_yinyue;                   //设置界面音乐组件
    Toggle toggle_yinxiao;                  //设置界面音效组件
    GameObject go_otherHead;                //匹配对手时的随机头像
    GameObject go_yujiText;                 //匹配时的预计时间
    GameObject go_dengdaiText;              //匹配时的等待时间
    GameObject go_cancleMatchBtn;           //取消匹配按钮
    bool isMatching = false;                //是否正在匹配
    GameObject go_textTime;                 //匹配时间区域
    GameObject go_vs;                       //匹配VS
    GameObject go_myHeadA;                  //匹配自己头像区域
    GameObject go_otherHeadA;               //匹配对手头像区域
    GameObject go_left;                     //匹配时左边背景
    GameObject go_right;                    //匹配时右边背景
    GameObject go_ani_vs;                   //vs动画
    GameObject go_playerName;               //玩家名字
    GameObject go_coin;                     //玩家钱
    GameObject go_jiangquan;                //玩家奖券
    GameObject go_suo_n1;                   //锁1 普通场
    GameObject go_suo_n2;                   //锁2 普通场
    GameObject go_suo_n3;                   //锁3 普通场
    GameObject go_suo_n4;                   //锁4 普通场
    GameObject go_suo_s1;                   //锁1 达标场
    GameObject go_suo_s2;                   //锁2 达标场
    GameObject go_suo_s3;                   //锁3 达标场
    GameObject go_suo_s4;                   //锁4 达标场
    UnityEngine.Transform trs_putong;
    UnityEngine.Transform trs_dabiao;
    UnityEngine.Transform trs_xinshou;
    UnityEngine.Transform trs_gaoshou;
    UnityEngine.Transform trs_zhuanjia;
    UnityEngine.Transform trs_dashi;
    UnityEngine.Transform trs_pingmin;
    UnityEngine.Transform trs_xiaokang;
    UnityEngine.Transform trs_tuhao;
    UnityEngine.Transform trs_shoufu;
    List<UnityEngine.Transform> trsArray_mode;
    List<UnityEngine.Transform> trsArray_putong;
    List<UnityEngine.Transform> trsArray_dabiao;
    List<GameObject> goArray_matchImg;
    float moveToTime = 0.25f;
    float waitTime = 0.1f;
    int stageType;                          //0:modeUI  1:putongUI  2:dabiaoUI
    int addNum = 10;
    Ex_Home ex_home;
    Sprite sprite_myHead;

    public void InitUI(GameObject _homeUI,Ex_Home _ex_home)
    {
        homeUI = _homeUI;
        ex_home = _ex_home;
        CGame_DiuDiuLe.SetCurUI(ex_home);
        stageType = 0;
        go_bg = homeUI.transform.Find("ImageLobbyBG").gameObject;
        go_entrance = homeUI.transform.Find("Entrance").gameObject;
        modeUI = go_entrance.transform.Find("ModeSelection").gameObject;
        trs_putong = modeUI.transform.Find("Button_PT");
        trs_dabiao = modeUI.transform.Find("Button_DB");
        putongUI = go_entrance.transform.Find("PoTong_").gameObject;
        trs_xinshou = putongUI.transform.Find("Button_grade1");
        trs_gaoshou = putongUI.transform.Find("Button_grade2");
        trs_zhuanjia = putongUI.transform.Find("Button_grade3");
        trs_dashi = putongUI.transform.Find("Button_grade4");
        dabiaoUI = go_entrance.transform.Find("DaBiao_").gameObject;
        trs_pingmin = dabiaoUI.transform.Find("Button_grade1");
        trs_xiaokang = dabiaoUI.transform.Find("Button_grade2");
        trs_tuhao = dabiaoUI.transform.Find("Button_grade3");
        trs_shoufu = dabiaoUI.transform.Find("Button_grade4");
        go_topBtn = homeUI.transform.Find("TopButton").gameObject;
        go_playerInfo = homeUI.transform.Find("PlayerInfo").gameObject;
        go_playerHead = go_playerInfo.transform.Find("Image_HeadFram").Find("Image_Mask").Find("Image_Head").gameObject;
        go_playerName = go_playerInfo.transform.Find("NameBG").Find("TextName").gameObject;
        go_coin = go_playerInfo.transform.Find("YinBi").Find("TextNum").gameObject;
        go_jiangquan = go_playerInfo.transform.Find("JiangQuan").Find("TextNum").gameObject;
        go_returnBtn = homeUI.transform.Find("TopButton").Find("ButtonReturn").gameObject;
        go_settingBtn = homeUI.transform.Find("TopButton").Find("ButtonSet").gameObject;
        go_ruleBtn = homeUI.transform.Find("TopButton").Find("ButtonTeach").gameObject;
        go_mask = homeUI.transform.Find("Pop-up").gameObject;
        go_ruleUI = go_mask.transform.Find("Rule").gameObject;
        go_rule_image = go_ruleUI.transform.Find("ImageRule").gameObject;
        go_rule_startBtn = go_ruleUI.transform.Find("ButtonStart").gameObject;
        go_settingUI = go_mask.transform.Find("Set").gameObject;
        toggle_yinyue = go_settingUI.transform.Find("ToggleYinyue").gameObject.GetComponent<Toggle>();
        toggle_yinxiao = go_settingUI.transform.Find("ToggleYinxiao").gameObject.GetComponent<Toggle>();
        go_matchUI = homeUI.transform.Find("Matching").gameObject;
        go_yujiText = go_matchUI.transform.Find("TextTimeBG").Find("TextTimeYuji").gameObject;
        go_dengdaiText = go_matchUI.transform.Find("TextTimeBG").Find("TextTimeShiji").gameObject;
        go_cancleMatchBtn = go_matchUI.transform.Find("ButtonCancel").gameObject;
        go_textTime = go_matchUI.transform.Find("TextTimeBG").gameObject;
        go_vs = go_matchUI.transform.Find("TextSvBG").gameObject;
        go_myHeadA = go_matchUI.transform.Find("Image_HeadFram_Oneself").gameObject;
        go_otherHeadA = go_matchUI.transform.Find("Image_HeadFram_Partner").gameObject;
        go_left = go_matchUI.transform.Find("ImageBG").Find("BG_Left").gameObject;
        go_right = go_matchUI.transform.Find("ImageBG").Find("BG_Right").gameObject;
        
        go_suo_n1 = trs_xinshou.Find("ImageLock").gameObject;
        go_suo_n2 = trs_gaoshou.Find("ImageLock").gameObject;
        go_suo_n3 = trs_zhuanjia.Find("ImageLock").gameObject;
        go_suo_n4 = trs_dashi.Find("ImageLock").gameObject;
        go_suo_s1 = trs_pingmin.Find("ImageLock").gameObject;
        go_suo_s2 = trs_xiaokang.Find("ImageLock").gameObject;
        go_suo_s3 = trs_tuhao.Find("ImageLock").gameObject;
        go_suo_s4 = trs_shoufu.Find("ImageLock").gameObject;
        go_matchUI.SetActive(false);

        if (GameMain.hall_.GetPlayerData() != null)
        {
            go_playerName.GetComponent<Text>().text = GameMain.hall_.GetPlayerData().GetPlayerName();
            go_coin.GetComponent<Text>().text = GameMain.hall_.GetPlayerData().GetCoin().ToString();
            go_jiangquan.GetComponent<Text>().text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();

            sprite_myHead = GameMain.hall_.GetIcon(GameMain.hall_.GetPlayerData().GetPlayerIconURL(),
                GameMain.hall_.GetPlayerId(), (int)GameMain.hall_.GetPlayerData().PlayerIconId);

            go_playerHead.GetComponent<Image>().sprite = sprite_myHead;
        }
        if (homeUI != null)
        {
            ModeUIIn();
        }
        //添加普通模式按钮监听
        XPointEvent.AutoAddListener(trs_putong.gameObject, ShowUI, trs_putong.gameObject);
        //添加达标模式按钮监听
        XPointEvent.AutoAddListener(trs_dabiao.gameObject, ShowUI, trs_dabiao.gameObject);
        XPointEvent.AutoAddListener(go_returnBtn, ShowUI, go_returnBtn);
        XPointEvent.AutoAddListener(go_settingBtn, ShowUI, go_settingBtn);
        XPointEvent.AutoAddListener(go_ruleBtn, ShowUI, go_ruleBtn);
        toggle_yinyue.onValueChanged.AddListener(delegate (bool music) {
            SwitchMusic(music);
        });
        toggle_yinxiao.onValueChanged.AddListener(delegate (bool sound)
        {
            SwitchSound(sound);
        });

        //发送CMGameLimit
        UMessage app = new UMessage((uint)GameCity.Diudiule_enum.DiudiuleMsg_CM_GAMELIMIT);
        Ex_CMGamelimit ex_CMGamelimit = new Ex_CMGamelimit();
        ex_CMGamelimit.userid = GameMain.hall_.GetPlayerId();
        ex_CMGamelimit.diuMsgType = (uint)GameCity.Diudiule_enum.DiudiuleMsg_CM_GAMELIMIT;
        ex_CMGamelimit.SetSendData(app);
        if (HallMain.gametcpclient.IsSocketConnected)
        {
            HallMain.SendMsgToRoomSer(app);
        }
        SoundFunc();

        InitDDLMsg();
    }

    void SwitchMusic(bool music)
    {
        if (Ex_GameData.SOUNDCONFIG)
        {
            AudioManager.Instance.PlaySound("diu.resource","ex_sBtn");
        }
        Ex_GameData.BGSOUNDCONFIG = music == true ? false : true;
        SoundFunc();
    }

    void SwitchSound(bool sound)
    {
        if (Ex_GameData.SOUNDCONFIG)
        {
            AudioManager.Instance.PlaySound("diu.resource", "ex_sBtn");
        }
        Ex_GameData.SOUNDCONFIG = sound == true ? false : true;
    }

    void SoundFunc()
    {
        if (Ex_GameData.BGSOUNDCONFIG)
        {
            AudioManager.Instance.PlayBGMusic("diu.resource", "ex_sDabiaoBg");
        }
        else
        {
            AudioManager.Instance.StopMusic();
        }
    }

    // Update is called once per frame
    int matchNum = 0;
    float time_roll = 0;        //图片移动
    float time_dengdai = 0;     //时间统计（每秒）
    public void Update() {
        if (go_matchUI.activeSelf && isMatching)
        {
            float dt = Time.deltaTime;
            time_roll += dt;
            time_dengdai += dt;
            if (time_roll >= 0.06)
            {
                time_roll = 0;
                for (int i = 0;i < goArray_matchImg.Count;i++)
                {
                    if (goArray_matchImg[i].transform.localPosition.y <= -80)
                    {
                        goArray_matchImg[i].transform.localPosition = new Vector3(0, 160, 0);
                    }
                    goArray_matchImg[i].transform.localPosition = new Vector3(0, goArray_matchImg[i].transform.localPosition.y - addNum, 0);
                }
            }
            if (time_dengdai >= 1)
            {
                time_dengdai = 0;
                matchNum++;
                go_dengdaiText.GetComponent<Text>().text = "等待时间："+ matchNum + "秒";
                //if (matchNum == 5)
                //{
                //    PlayOver();
                //}
            }
        }
    }

    //void ShowAnimation_Mode(List<Transform> trsArray, bool comDisActive = false)
    //{

    //    StartCoroutine(Show(trsArray, comDisActive));
    //}

    //IEnumerator Show(List<Transform> trsArray, bool comDisActive)
    //{
    //    //items数组引用了图片中的那几个长条Transform
    //    foreach (var item in trsArray)
    //    {
    //        Tweener t = item.DOMoveX(item.position.x - 540, moveToTime, false).SetEase(Ease.OutBack);
    //        if (comDisActive)
    //        {
    //            t.OnComplete(() => OnComplete(item));
    //        }
    //        yield return new WaitForSeconds(waitTime);
    //    }
    //}

    void ShowAnimation_Mode(List<UnityEngine.Transform> trsArray, bool comDisActive = false)
    {
        for (var i = 0; i < trsArray.Count; i++)
        {
            Show(trsArray[i], i, comDisActive);
        }
    }

    void Show(UnityEngine.Transform item, int index, bool comDisActive)
    {
        Sequence s1 = DOTween.Sequence();
        int a = 0;
        Tweener t = DOTween.To(() => a, r => a = r, 0, index * waitTime);
        s1.Append(t);
        Tweener tween = DOTween.To(() => item.localPosition, r => item.localPosition = r, new Vector3(item.localPosition.x - 540, item.localPosition.y, 0), moveToTime).SetEase(Ease.OutBack);
        s1.Append(tween);
        if (comDisActive)
        {
            s1.AppendCallback(() => OnComplete(item));
        }
    }

    void OnComplete(UnityEngine.Transform item)
    {
        item.gameObject.SetActive(false);
    }

    void CompleteAni()
    {
        DOTween.CompleteAll();
        DOTween.Clear();

    }

    void ShowUI(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerDown)
        {
            if (Ex_GameData.SOUNDCONFIG)
            {
                AudioManager.Instance.PlaySound("diu.resource", "ex_sBtn");
            }
            _ShowUI((GameObject)button);
        }
    }

    void _ShowUI(GameObject go)
    {
        CompleteAni();
        if (go == trs_putong.gameObject)
        {
            ShowAnimation_Mode(trsArray_mode, true);
            PutongUIIn();
            stageType = 1;
        }
        else if (go == trs_dabiao.gameObject)
        {
            ShowAnimation_Mode(trsArray_mode, true);
            DabiaoUIIn();
            stageType = 2;
        }
        else if (go == go_returnBtn)
        {
            if (stageType == 0)
            {
                CGame_DiuDiuLe.ExitToHall();
            }
            else if (stageType == 1)
            {
                ShowAnimation_Mode(trsArray_putong, true);
                ModeUIIn();
                stageType = 0;
            }
            else if (stageType == 2)
            {
                ShowAnimation_Mode(trsArray_dabiao, true);
                ModeUIIn();
                stageType = 0;
            }
        }
        else if (go == go_settingBtn)
        {
            if (!go_mask.activeSelf) go_mask.SetActive(true);
            if (!go_settingUI.activeSelf) go_settingUI.SetActive(true);
            go_settingUI.transform.localScale = new Vector3(0, 0, 1);
            Tweener t = go_settingUI.transform.DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.OutBack);
            t.OnComplete(() =>
            {
                XPointEvent.AutoAddListener(go_mask, CloseUI, go_mask);
            });
        }
        else if (go == go_ruleBtn)
        {
            if (!go_mask.activeSelf) go_mask.SetActive(true);
            if (!go_ruleUI.activeSelf) go_ruleUI.SetActive(true);
            if (go_rule_startBtn.activeSelf) go_rule_startBtn.SetActive(false);
            rule_index = 1;
            Sprite tempSprite = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("Guize_BG_" + rule_index);
            go_rule_image.GetComponent<Image>().sprite = tempSprite;
            XPointEvent.AutoAddListener(go_rule_image, Rule_Next, go_rule_image);
        }
    }

    int rule_index = 1;
    void Rule_Next(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerDown)
        {
            rule_index++;
            Sprite tempSprite = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("Guize_BG_" + rule_index);
            go_rule_image.GetComponent<Image>().sprite = tempSprite;
            if (rule_index == 3)
            {
                if (!go_rule_startBtn.activeSelf) go_rule_startBtn.SetActive(true);
                XPointEvent.AutoAddListener(go_rule_startBtn, CloseUI, go_rule_startBtn);
                XPointEvent.AutoAddListener(go_rule_image, null, null);
                rule_index = 1;
            }
            else
            {
                if (go_rule_startBtn.activeSelf) go_rule_startBtn.SetActive(false);
            }
        }
    }

    void CloseUI(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerDown)
        {
            if (button == (object)go_rule_startBtn)
            {
                XPointEvent.AutoAddListener(go_rule_startBtn, null, null);
                if (go_rule_startBtn.activeSelf) go_rule_startBtn.SetActive(false);
                if (go_mask.activeSelf) go_mask.SetActive(false);
                if (go_ruleUI.activeSelf) go_ruleUI.SetActive(false);
            }
            else if (button == (object)go_mask)
            {
                XPointEvent.AutoAddListener(go_mask, null, null);
                Tweener t = go_settingUI.transform.DOScale(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.InBack);
                t.OnComplete(() => {
                    if (go_settingUI.activeSelf) go_settingUI.SetActive(false);
                    if (go_mask.activeSelf) go_mask.SetActive(false);
                });
            }
        }
    }

    void ActiveBtn(List<UnityEngine.Transform> trsArr)
    {
        foreach (var trs in trsArr)
        {
            trs.gameObject.SetActive(true);
            trs.localPosition = new Vector3(540, trs.localPosition.y, trs.localPosition.z);
            //trs.localPosition = new Vector3(0, trs.localPosition.y, trs.localPosition.z);
        }
    }

    void ModeUIIn()
    {
        if (trsArray_mode == null)
        {
            trs_putong.localPosition = new Vector3(540, trs_putong.localPosition.y, trs_putong.localPosition.z);
            trs_dabiao.localPosition = new Vector3(540, trs_dabiao.localPosition.y, trs_dabiao.localPosition.z);
            trsArray_mode = new List<UnityEngine.Transform>();
            trsArray_mode.Add(trs_putong);
            trsArray_mode.Add(trs_dabiao);
        }
        else
        {
            ActiveBtn(trsArray_mode);
        }
        ShowAnimation_Mode(trsArray_mode);
    }

    void PutongUIIn()
    {
        if (trsArray_putong == null)
        {
            putongUI.SetActive(true);
            trs_xinshou.localPosition = new Vector3(540, trs_xinshou.localPosition.y, trs_xinshou.localPosition.z);
            trs_gaoshou.localPosition = new Vector3(540, trs_gaoshou.localPosition.y, trs_gaoshou.localPosition.z);
            trs_zhuanjia.localPosition = new Vector3(540, trs_zhuanjia.localPosition.y, trs_zhuanjia.localPosition.z);
            trs_dashi.localPosition = new Vector3(540, trs_dashi.localPosition.y, trs_dashi.localPosition.z);
            trsArray_putong = new List<UnityEngine.Transform>();
            trsArray_putong.Add(trs_xinshou);
            trsArray_putong.Add(trs_gaoshou);
            trsArray_putong.Add(trs_zhuanjia);
            trsArray_putong.Add(trs_dashi);
        }
        else
        {
            ActiveBtn(trsArray_putong);
        }
        ShowAnimation_Mode(trsArray_putong);
    }

    void DabiaoUIIn()
    {
        if (trsArray_dabiao == null)
        {
            dabiaoUI.SetActive(true);
            trs_pingmin.localPosition = new Vector3(540, trs_pingmin.localPosition.y, trs_pingmin.localPosition.z);
            trs_xiaokang.localPosition = new Vector3(540, trs_xiaokang.localPosition.y, trs_xiaokang.localPosition.z);
            trs_tuhao.localPosition = new Vector3(540, trs_tuhao.localPosition.y, trs_tuhao.localPosition.z);
            trs_shoufu.localPosition = new Vector3(540, trs_shoufu.localPosition.y, trs_shoufu.localPosition.z);
            trsArray_dabiao = new List<UnityEngine.Transform>();
            trsArray_dabiao.Add(trs_pingmin);
            trsArray_dabiao.Add(trs_xiaokang);
            trsArray_dabiao.Add(trs_tuhao);
            trsArray_dabiao.Add(trs_shoufu);
        }
        else
        {
            ActiveBtn(trsArray_dabiao);
        }
        ShowAnimation_Mode(trsArray_dabiao);
    }

    void ShowXinshou(GameObject go)
    {
        //GameObject xinshouUI = Ex_Main.InsPrefab("prefabs/ex_xinshouUI", homeUI);
        //Ex_Xinshou ex_xinshou = new Ex_Xinshou();
        //ex_xinshou.InitUI(xinshouUI);
    }

    void MatchFunc()
    {
        GameObject go_myHead = go_myHeadA.transform.Find("Image_Mask").Find("Image_Head").gameObject;
        go_myHead.GetComponent<Image>().sprite = sprite_myHead;
        XPointEvent.AutoAddListener(go_cancleMatchBtn, CancelMatchFunc, go_cancleMatchBtn);
        if (!go_matchUI.activeSelf) go_matchUI.SetActive(true);
        //vs动画
        if (go_ani_vs == null)
        {
            go_ani_vs = CGame_DiuDiuLe.InsPrefab("Anime_vscar", go_matchUI);
        }
        go_ani_vs.GetComponent<UnityArmatureComponent>().animation.Play("vs_0");
        if (goArray_matchImg == null)
        {
            goArray_matchImg = new List<GameObject>();
            UnityEngine.Transform trs_parent;
            trs_parent = go_matchUI.transform.Find("Image_HeadFram_Partner").Find("Image_Mask");
            go_otherHead = trs_parent.Find("Image_Head").gameObject;
            goArray_matchImg.Add(go_otherHead);
            for (int i = 2;i <= 3; i++)
            {
                GameObject go_temp = new GameObject();
                Image img_temp = go_temp.AddComponent<Image>();
                Sprite sprite_temp = CGame_DiuDiuLe.BundleIns().LoadAsset<Sprite>("Pipei_Icon_0"+i);
                img_temp.sprite = sprite_temp;
                go_temp.transform.SetParent(trs_parent);
                go_temp.transform.localPosition = new Vector3(0,(i-1)*80,0);
                go_temp.GetComponent<Image>().SetNativeSize();
                goArray_matchImg.Add(go_temp);
            }
        }
        matchNum = 0;
        float num = Mathf.Floor(UnityEngine.Random.value * 5) + 2;
        go_yujiText.GetComponent<Text>().text = "预计时间："+num+"秒";
        go_dengdaiText.GetComponent<Text>().text = "等待时间：0秒";
        isMatching = true;
    }

    void CancelMatchFunc(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerDown)
        {
            if (Ex_GameData.SOUNDCONFIG)
            {
                AudioManager.Instance.PlaySound("diu.resource", "ex_sBtn");
            }
            //发送取消匹配消息
            UMessage app = new UMessage((uint)GameCity.Diudiule_enum.DiudiuleMsg_CM_CANCLEREGION);
            Ex_CMGameExit ex_CMGameExit = new Ex_CMGameExit();
            ex_CMGameExit.diuMsgType = (uint)GameCity.Diudiule_enum.DiudiuleMsg_CM_CANCLEREGION;
            ex_CMGameExit.userid = GameMain.hall_.GetPlayerId();
            ex_CMGameExit.SetSendData(app);
            if (HallMain.gametcpclient.IsSocketConnected)
            {
                HallMain.SendMsgToRoomSer(app);
            }

            XPointEvent.AutoAddListener(go_cancleMatchBtn, null, go_cancleMatchBtn);
            go_matchUI.SetActive(false);
            isMatching = false;
            if (stageType == 1)
            {
                _ShowUI(trs_putong.gameObject);
            }
            else if (stageType == 2)
            {
                _ShowUI(trs_dabiao.gameObject);
            }
        }
    }

    void PlayOver(bool isReconect)
    {
        if (!go_matchUI.activeSelf) go_matchUI.SetActive(true);
        GameObject go_myHead = go_myHeadA.transform.Find("Image_Mask").Find("Image_Head").gameObject;
        go_myHead.GetComponent<Image>().sprite = sprite_myHead;
        isMatching = false;
        if (go_ani_vs != null)
        {
            go_ani_vs.GetComponent<UnityArmatureComponent>().animation.Stop();
            UnityEngine.Object.DestroyImmediate(go_ani_vs);
            go_ani_vs = null;
        }
        //UnityFactory.factory.Clear();
        go_cancleMatchBtn.SetActive(false);
        GameObject tableUI = CGame_DiuDiuLe.LoadPrefab("DiuDiuLe_Game");
        tableUI.transform.SetAsFirstSibling();
        UnityEngine.Transform trs_parent;
        trs_parent = go_matchUI.transform.Find("Image_HeadFram_Partner").Find("Image_Mask");
        GameObject go_temp = new GameObject();
        Image img_temp = go_temp.AddComponent<Image>();
        Sprite sprite_temp = null;

        sprite_temp = GameMain.hall_.GetIcon(Ex_GameData.otherUrl, Ex_GameData.otherUserId, (int)Ex_GameData.otherFaceId);

        img_temp.sprite = sprite_temp;
        go_temp.transform.SetParent(trs_parent);
        go_temp.transform.localPosition = new Vector3(0, 0, 0);
        go_temp.GetComponent<Image>().SetNativeSize();
        go_bg.SetActive(false);
        go_entrance.SetActive(false);
        go_topBtn.SetActive(false);
        go_playerInfo.SetActive(false);
        go_mask.SetActive(false);
        Sequence s1 = DOTween.Sequence();
        int a = 0;
        Tweener t = DOTween.To(() => a, r => a = r, 0, 1);
        s1.Append(t);
        s1.AppendCallback(() =>
        {
            go_textTime.SetActive(false);
            go_otherHeadA.SetActive(false);
            go_vs.SetActive(false);
            go_myHeadA.SetActive(false);
            float leftX = go_left.transform.localPosition.x;
            float rightX = go_right.transform.localPosition.x;
            DOTween.To(() => go_left.transform.localPosition, r => go_left.transform.localPosition = r, new Vector3(leftX - 270, 0, 0), 0.5f);
            DOTween.To(() => go_right.transform.localPosition, r => go_right.transform.localPosition = r, new Vector3(rightX + 270, 0, 0), 0.5f);
            Ex_Table ex_table = new Ex_Table();
            ex_table.InitUI(tableUI, ex_table, isReconect, Ex_GameData.areaId);
        });
        Tweener t1 = DOTween.To(() => a, r => a = r, 0, 0.5f);
        s1.Append(t1);
        s1.AppendCallback(() =>
        {
            UnityEngine.Object.Destroy(homeUI);
            ex_home = null;
        });
    }

    bool BackDiuDiuLeGameLimit(uint msgType, UMessage _ms)
    {
        //List<Dictionary<string,int>> data_normal = new List<Dictionary<string, int>>();
        //List<Dictionary<string, int>> data_standard = new List<Dictionary<string, int>>(); 
        Ex_GameData.SMGamelimit = new Ex_SMGamelimit();
        Ex_GameData.SMGamelimit.normallimit = new List<Ex_MFNormallimit>();
        Ex_GameData.SMGamelimit.standardlimit = new List<Ex_MFStandardlimit>();

        byte nNormalSize = _ms.ReadByte();          //普通场 有几个等级
        byte nStandardSize = _ms.ReadByte();        //达标场  有几个等级

        // normal
        for (int i = 0; i < nNormalSize; i++)
        {
            //Dictionary<string, int> data = new Dictionary<string, int>();
            Ex_MFNormallimit data = new Ex_MFNormallimit();
            int nPlayscore_pro = _ms.ReadInt();     //分数的比列
            int nStartcutcoin = _ms.ReadInt();      //刚开始 减少的钱币
            int nFakerreward = _ms.ReadInt();       //反杀增加的奖励
            int nMaxCarryIn = _ms.ReadInt();        //最大带入
            int nMinCarryIn = _ms.ReadInt();        //最小带入
            data.playscore_pro = nPlayscore_pro;
            data.startcutcoin = nStartcutcoin;
            data.fakerreward = nFakerreward;
            data.mincarryin = nMinCarryIn;
            data.maxcarryin = nMaxCarryIn;
            Ex_GameData.SMGamelimit.normallimit.Add(data);
            //data.Add("nPlayscore_pro", nPlayscore_pro);
            //data.Add("nStartcutcoin", nStartcutcoin);
            //data.Add("nFakerreward", nFakerreward);
            //data.Add("nMaxCarryIn", nMaxCarryIn);
            //data.Add("nMinCarryIn", nMinCarryIn);
            //data_normal.Add(data);
        }

        // standard
        for (int i = 0; i < nStandardSize; i++)
        {
            //Dictionary<string,int> data = new Dictionary<string, int>();
            Ex_MFStandardlimit data = new Ex_MFStandardlimit();
            int nReward = _ms.ReadInt();            //奖励
            int nRegistmoney = _ms.ReadInt();       //报名费
            int nMaxnum = _ms.ReadInt();            //最大场次
            int nStandardscore = _ms.ReadInt();     //达标分
            int nFakereward = _ms.ReadInt();        //反杀的奖励
            data.reward = nReward;
            data.registmoney = nRegistmoney;
            data.maxnum = nMaxnum;
            data.standardscore = nStandardscore;
            data.fakerreward = nFakereward;
            Ex_GameData.SMGamelimit.standardlimit.Add(data);
            //data.Add("nReward", nReward);
            //data.Add("nRegistmoney", nRegistmoney);
            //data.Add("nMaxnum", nMaxnum);
            //data.Add("nStandardscore", nStandardscore);
            //data.Add("nFakereward", nFakereward);
            //data_standard.Add(data);
        }
        //Ex_GameData.DataList.Clear();
        //Ex_GameData.DataList.Add("normallimit",data_normal);
        //Ex_GameData.DataList.Add("standardlimit", data_standard);
        SetGameLimitState();

        return true;
    }

    bool BackDiuDiuLeRegion(uint msgType, UMessage _ms)
    {
        byte nState = _ms.ReadByte();
        // 1:成功
        // 2：选择等级非法
        // 3：已经在匹配队列里了
        // 4：身上钱币低于最少带入
        if (nState == 1)
        {
            MatchFunc();
        }
        return true;
    }

    bool BackDiuDiuLeSitDown(uint msgType, UMessage _ms)
    {
        Ex_GameData.SMSitDownData = new Ex_SMSitDown();
        Ex_GameData.SMSitDownData.ps = new List<Ex_SitDownPlayer>();
        uint roomid = _ms.ReadUInt();
        byte nNum = _ms.ReadByte();

        for (int i = 0; i < nNum; i++)
        {
            Ex_SitDownPlayer p = new Ex_SitDownPlayer();
            uint userid = _ms.ReadUInt();
            string userName = _ms.ReadString();
            Int64 userCoin = _ms.ReadLong();
            uint faceId = _ms.ReadUInt();
            string url = _ms.ReadString();

            p.userid = userid;
            p.userName = userName;
            p.userCoin = userCoin;
            p.faceId = faceId;
            p.url = url;
            Ex_GameData.SMSitDownData.ps.Add(p);
            if (p.userid == GameMain.hall_.GetPlayerData().GetPlayerID())
            {
                Ex_GameData.myUserId = p.userid;
            }
            else
            {
                Ex_GameData.otherUserId = p.userid;
                Ex_GameData.otherFaceId = p.faceId;
                Ex_GameData.otherUrl = p.url;
            }
        }

        Ex_GameData.SMSitDownData.roomid = roomid;
        Ex_GameData.SMSitDownData.nNum = nNum;

        return true;
    }

    bool BackDiuDiuLeGameStart(uint msgType, UMessage _ms)
    {
        Ex_GameData.SMGameStartData = new Ex_SMGameStart();
        Ex_GameData.SMGameStartData.usercardList = new List<byte>();

        uint userid = _ms.ReadUInt();//第一个出牌的userid
        byte roomType = _ms.ReadByte();//1普通场 2达标场
        byte roomLevel = _ms.ReadByte();//4个场次
        uint scorePro = _ms.ReadUInt();//底分
        uint standardScore = _ms.ReadUInt();//达标分
        uint maxNum = _ms.ReadUInt();//最大场次
        uint countNum = _ms.ReadUInt();//当前场次
        uint rewardCoin = _ms.ReadUInt();//奖金分
                                         //5张手牌牌值
        for (int i = 0; i < 5; i++)
        {
            byte num = _ms.ReadByte();
            Ex_GameData.SMGameStartData.usercardList.Add(num);
        }
        Ex_GameData.SMGameStartData.userid = userid;
        Ex_GameData.SMGameStartData.roomType = roomType;
        Ex_GameData.SMGameStartData.roomLevel = roomLevel;
        Ex_GameData.SMGameStartData.scorePro = scorePro;
        Ex_GameData.SMGameStartData.standardScore = standardScore;
        Ex_GameData.SMGameStartData.maxNum = maxNum;
        Ex_GameData.SMGameStartData.countNum = countNum;
        Ex_GameData.SMGameStartData.rewardCoin = rewardCoin;

        PlayOver(false);

        return true;
    }

    public void InitDDLMsg()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Diudiule_enum.DiudiuleMsg_SM_GAMELIMIT, BackDiuDiuLeGameLimit);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Diudiule_enum.DiudiuleMsg_SM_REGION, BackDiuDiuLeRegion);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Diudiule_enum.DiudiuleMsg_SM_SITDOWN, BackDiuDiuLeSitDown);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.Diudiule_enum.DiudiuleMsg_SM_GAMESTART, BackDiuDiuLeGameStart);
    }

    void SetGameLimitState()
    {
        go_suo_n1.SetActive(false);
        go_suo_n2.SetActive(false);
        go_suo_n3.SetActive(false);
        go_suo_n4.SetActive(false);
        go_suo_s1.SetActive(false);
        go_suo_s2.SetActive(false);
        go_suo_s3.SetActive(false);
        go_suo_s4.SetActive(false);
        Ex_SMGamelimit dataList = Ex_GameData.SMGamelimit;
        //Dictionary<string, List<Dictionary<string, int>>> dataList = Ex_GameData.DataList;

        long playercoin = GameMain.hall_.GetPlayerData().GetCoin();
        if (dataList.normallimit[0].mincarryin > playercoin)
        {
            go_suo_n1.SetActive(true);
        }
        if (dataList.normallimit[1].mincarryin > playercoin)
        {
            go_suo_n2.SetActive(true);
        }
        if (dataList.normallimit[2].mincarryin > playercoin)
        {
            go_suo_n3.SetActive(true);
        }
        if (dataList.normallimit[3].mincarryin > playercoin)
        {
            go_suo_n4.SetActive(true);
        }
        if (dataList.standardlimit[0].registmoney > playercoin)
        {
            go_suo_s1.SetActive(true);
        }
        if (dataList.standardlimit[1].registmoney > playercoin)
        {
            go_suo_s2.SetActive(true);
        }
        if (dataList.standardlimit[2].registmoney > playercoin)
        {
            go_suo_s3.SetActive(true);
        }
        if (dataList.standardlimit[3].registmoney > playercoin)
        {
            go_suo_s4.SetActive(true);
        }

        XPointEvent.AutoAddListener(trs_xinshou.gameObject, Func, trs_xinshou.gameObject);
        XPointEvent.AutoAddListener(trs_gaoshou.gameObject, Func, trs_gaoshou.gameObject);
        XPointEvent.AutoAddListener(trs_zhuanjia.gameObject, Func, trs_zhuanjia.gameObject);
        XPointEvent.AutoAddListener(trs_dashi.gameObject, Func, trs_dashi.gameObject);
        XPointEvent.AutoAddListener(trs_pingmin.gameObject, Func, trs_pingmin.gameObject);
        XPointEvent.AutoAddListener(trs_xiaokang.gameObject, Func, trs_xiaokang.gameObject);
        XPointEvent.AutoAddListener(trs_tuhao.gameObject, Func, trs_tuhao.gameObject);
        XPointEvent.AutoAddListener(trs_shoufu.gameObject, Func, trs_shoufu.gameObject);
    }

    void Func(EventTriggerType eventtype, object button, PointerEventData eventData)
    {
        if (eventtype == EventTriggerType.PointerDown)
        {
            if (Ex_GameData.SOUNDCONFIG)
            {
                AudioManager.Instance.PlaySound("diu.resource", "ex_sBtn");
            }
            //新手
            if (button == (object)trs_xinshou.gameObject)
            {
                if (go_suo_n1.activeSelf)
                {
                    GameObject go_tips = CGame_DiuDiuLe.InsPrefab("DiuDiuLe_Tips", homeUI);
                    new Ex_Tips(go_tips, "该场需要 " + Ex_GameData.SMGamelimit.normallimit[0].mincarryin + " 才可进入（1元=1$）");
                }
                else
                {
                    RegionFunc(1, 1);
                }
            }
            //高手
            else if (button == (object)trs_gaoshou.gameObject)
            {
                if (go_suo_n2.activeSelf)
                {
                    GameObject go_tips = CGame_DiuDiuLe.InsPrefab("DiuDiuLe_Tips", homeUI);
                    new Ex_Tips(go_tips, "该场需要 " + Ex_GameData.SMGamelimit.normallimit[1].mincarryin + " 才可进入（1元=1$）");
                }
                else
                {
                    RegionFunc(1, 2);
                }
            }
            //专家
            else if (button == (object)trs_zhuanjia.gameObject)
            {
                if (go_suo_n3.activeSelf)
                {
                    GameObject go_tips = CGame_DiuDiuLe.InsPrefab("DiuDiuLe_Tips", homeUI);
                    new Ex_Tips(go_tips, "该场需要 " + Ex_GameData.SMGamelimit.normallimit[2].mincarryin + " 才可进入（1元=1$）");
                }
                else
                {
                    RegionFunc(1, 3);
                }
            }
            //大师
            else if (button == (object)trs_dashi.gameObject)
            {
                if (go_suo_n4.activeSelf)
                {
                    GameObject go_tips = CGame_DiuDiuLe.InsPrefab("DiuDiuLe_Tips", homeUI);
                    new Ex_Tips(go_tips, "该场需要 " + Ex_GameData.SMGamelimit.normallimit[3].mincarryin + " 才可进入（1元=1$）");
                }
                else
                {
                    RegionFunc(1, 4);
                }
            }//平民
            else if (button == (object)trs_pingmin.gameObject)
            {
                if (go_suo_s1.activeSelf)
                {
                    GameObject go_tips = CGame_DiuDiuLe.InsPrefab("DiuDiuLe_Tips", homeUI);
                    new Ex_Tips(go_tips, "该场需要报名费: " + Wan("" + Ex_GameData.SMGamelimit.standardlimit[0].registmoney) + " 才可进入");
                }
                else
                {
                    RegionFunc(2, 1);
                }
            }//小康
            else if (button == (object)trs_xiaokang.gameObject)
            {
                if (go_suo_s2.activeSelf)
                {
                    GameObject go_tips = CGame_DiuDiuLe.InsPrefab("DiuDiuLe_Tips", homeUI);
                    new Ex_Tips(go_tips, "该场需要报名费: " + Wan("" + Ex_GameData.SMGamelimit.standardlimit[1].registmoney) + " 才可进入");
                }
                else
                {
                    RegionFunc(2, 2);
                }
            }//土豪
            else if (button == (object)trs_tuhao.gameObject)
            {
                if (go_suo_s3.activeSelf)
                {
                    GameObject go_tips = CGame_DiuDiuLe.InsPrefab("DiuDiuLe_Tips", homeUI);
                    new Ex_Tips(go_tips, "该场需要报名费: " + Wan("" + Ex_GameData.SMGamelimit.standardlimit[2].registmoney) + " 才可进入");
                }
                else
                {
                    RegionFunc(2, 3);
                }
            }//首富
            else if (button == (object)trs_shoufu.gameObject)
            {
                if (go_suo_s4.activeSelf)
                {
                    GameObject go_tips = CGame_DiuDiuLe.InsPrefab("DiuDiuLe_Tips", homeUI);
                    new Ex_Tips(go_tips, "该场需要报名费: " + Wan("" + Ex_GameData.SMGamelimit.standardlimit[3].registmoney) + " 才可进入");
                }
                else
                {
                    RegionFunc(2, 4);
                }
            }
        }
    }

    string Wan(string str)
    {
        if (float.Parse(str) >= 10000)
        {
            return "" + float.Parse(str) / 10000 + "万";
        }
        return str;
    }

    public void RegionFunc(int _areaId, int _regionId)
    {
        Ex_GameData.areaId = _areaId;
        Ex_GameData.regionId = _regionId;
        Ex_GameData.isDengLu = true;

        //发送Region
        UMessage app = new UMessage((uint)GameCity.Diudiule_enum.DiudiuleMsg_CM_REGION);
        Ex_CMRegion ex_CMRegion = new Ex_CMRegion();
        ex_CMRegion.diuMsgType = (uint)GameCity.Diudiule_enum.DiudiuleMsg_CM_REGION;
        ex_CMRegion.userid = GameMain.hall_.GetPlayerId();
        ex_CMRegion.areaId = (byte)Ex_GameData.areaId;
        ex_CMRegion.regionId = (byte)Ex_GameData.regionId;
        ex_CMRegion.SetSendData(app);
        if (HallMain.gametcpclient.IsSocketConnected)
        {
            HallMain.SendMsgToRoomSer(app);
        }
    }
}
