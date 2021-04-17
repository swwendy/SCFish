//#define HAVE_LOBBY//#define SHOW_TIME
using DG.Tweening;
using SWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using USocket.Messages;
using XLua;

//捕鱼的游戏状态
[LuaCallCSharp]
public enum FishGameState_Enum
{
    FishGameState_Init,
    FishGameState_Begin,    //正处于刚开始填充的时候
    FishGameState_GameOn,   //游戏进行中
    FishGameState_Clear,    //清场状态
    FishGameState_Spacil,   //鱼群模式
    FishGameState_Forzen,   //冰冻状态

    FishGameState_End,      //游戏结束

    FishGameState
};


[LuaCallCSharp]
public class CannonSwitchData
{
    public bool m_bLock = false;
    public byte m_VipLimit;
    public int m_nDiamondLimit;
}

[LuaCallCSharp]
public class SkillData
{
    public byte m_nLeftNum;
}


//捕鱼技能的枚举
[LuaCallCSharp]
public enum FishingSkill_enum
{
    FISHINGSKILL_Init = 0,
    FISHINGSKILL_TRACE = 1,//捕鱼的 追踪技能
    FISHINGSKILL_FORZEN = 2,//捕鱼的 冰冻技能

    FISHINGSKILL
};

[LuaCallCSharp]
public enum FishType_Enum
{
    FishType_Init = 0,
    FishType_Normal,    //普通的鱼
    FishType_AllInOne,  //一网打尽
    FishType_IronChain, //铁索连环
    FishType_ScreenBomb,//全屏炸弹
    FishType_Lottery,   //出奖券的鱼
    FishType_Boss,      //boss鱼
    FishType_Cornucopia,//聚宝盆
    FishType_Seals,		//海豹

    FishType_Max
};



[Hotfix]
public class CGame_Fishing : CGameBase
{
    public Transform MainUITfm { get; private set; }
    public Transform LotteryUITfm { get; private set; }
#if HAVE_LOBBY    public Transform LobbyUITfm { get; private set; }#endif
    Transform FishUITfm { get; set; }
    Transform SwitchCannonUITfm { get; set; }

    Transform m_SkillUITfm;

    public AssetBundle FishingAssetBundle { get; private set; }

    public CustomCountdownImgMgr CCIMgr { get; private set; }
    public Fishing_FishMgr FishMgr { get; private set; }

    public Canvas GameCanvas { get; private set; }
    public Vector3 CameraSourcePos { get; private set; }
    MirrorFlipCamera m_MFCMgr;

    byte m_nLotteryValue = RoomInfo.NoSit;
    byte m_nCurrenLevel = RoomInfo.NoSit;
    public const byte PlayerNum = 4;
    public const int FishLayer = 1 << 11;
    FishGameState_Enum m_eGameState;

    Fishing_Role[] m_CPlayerList = new Fishing_Role[PlayerNum];
    Dictionary<byte, Fishing_Role> m_SPlayers = new Dictionary<byte, Fishing_Role>();
    byte m_nLocalSit = RoomInfo.NoSit;
    DragonBones.UnityArmatureComponent m_AnimBoss;
    DragonBones.UnityArmatureComponent m_AnimFishArray;

    public List<GameObject> m_AddItems = new List<GameObject>();

#if SHOW_TIME
    Transform m_TimeTfm;
#endif
    float m_fGameTime;

    Coroutine m_LotteryCoroutin = null;

    public CGame_Fishing() : base(GameKind_Enum.GameKind_Fishing)
    {
    }

    public override void Initialization()
    {
        base.Initialization();

        LoadResource();
        InitPlayers();
        InitMsgHandle();

        CustomAudioDataManager.GetInstance().ReadAudioCsvData((byte)GameKind_Enum.GameKind_Fishing, "Fishing_AudioCsv");
        CustomAudioDataManager.GetInstance().PlayAudio(1001, false);

        CCIMgr = new CustomCountdownImgMgr();
        FishMgr = new Fishing_FishMgr(this);

#if !HAVE_LOBBY        OnClickLevelBtn(GameMain.hall_.CurRoomIndex);
#endif
    }

    public override void ProcessTick()
    {
        base.ProcessTick();

        CCIMgr.UpdateTimeImage();

        if (!FishMgr.FishPause)
            m_fGameTime += Time.deltaTime;

#if SHOW_TIME
        if(m_TimeTfm.gameObject.activeSelf)
        {

            int time = (int)m_fGameTime;
            int hour =  time / 3600;
            time %= 3600;
            int min = time /60;
            time %= 60;

            m_TimeTfm.GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, min, time);
        }
#endif

        foreach(Fishing_Role role in m_SPlayers.Values)
        {
            role.OnTick();
        }
    }

    void InitMsgHandle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_CHOOSElEVEL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_ENTERROOM, HandleEnterRoom);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_FISHBORN, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_FISHDEAD, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_BACKFIRE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_FIRETESULT, HandleFireResult);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_BACKLEAVE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_OTHERPLAYERENTER, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_CHANGECANNON, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_CHANGECANNONLEVEL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_UPDATEROOMSTATE, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_PLAYERBUGCANNON, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_BACKUSEFISHINGSKILL, HandleSkillMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_UPDATEUSEFISHINGSKILL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_TRACECHANGETARGET, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_TRACESKILLTIMEOVER, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_REWORDFISHINGSKILL, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_KICKOUTROOM, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_FIREFAILED, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_APPLYCORNUCOPIA, HandleGameNetMsg);
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CCMsg_FISHING_SM_APPLYSEALREWORD, HandleGameNetMsg);
    }

    public override bool HandleGameNetMsg(uint _msgType, UMessage _ms)
    {
        GameCity.EMSG_ENUM eMsg = (GameCity.EMSG_ENUM)_msgType;
        switch (eMsg)
        {
            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_CHOOSElEVEL:
                {
                    CCustomDialog.CloseCustomWaitUI();

                    byte errorCode = _ms.ReadByte();//1：没有空余的位置了 2：低于最小值 3：高于最大值
                    byte level = _ms.ReadByte();
                    if(errorCode != 1)
                    {
                        FishingRoomData frd = Fishing_Data.GetInstance().m_RoomData[level - 1];
                        if (errorCode == 2)
                            CCustomDialog.OpenCustomConfirmUI(2501, (p) => BackToChooseLevel(), new object[] { frd.m_nMinInCoin });
                        else
                            CCustomDialog.OpenCustomConfirmUI(2502, (p) => BackToChooseLevel(), new object[] { frd.m_nMaxInCoin });
                    }
                    else
                        CCustomDialog.OpenCustomConfirmUI(2500);

                    m_nCurrenLevel = RoomInfo.NoSit;
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_FISHBORN:
                {
                    uint onlyId = _ms.ReadUInt();
                    ushort pathId = _ms.ReaduShort();
                    byte typeId = _ms.ReadByte();
                    float moveTime = _ms.ReadSingle();
                    byte group = _ms.ReadByte();//鱼群ID
                    byte index = _ms.ReadByte();//出生的序号
                    FishType_Enum fishType = (FishType_Enum)_ms.ReadByte();
                    float serverGameTime = _ms.ReadSingle();//房间已开始多长时间
                    float interval = m_fGameTime - serverGameTime;
                    if (interval > 0f)
                        moveTime += interval;
                    FishMgr.CreatePathFish(pathId, onlyId, typeId, moveTime, group, index);

                    if (fishType == FishType_Enum.FishType_Boss)
                        GameMain.SC(OnSomethinComing(m_AnimBoss, 2f));
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_FISHDEAD:
                {
                    uint onlyId = _ms.ReadUInt();
                    FishMgr.RemoveFish(onlyId);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_FIREFAILED:
                {
                    long coin = _ms.ReadLong();
                    uint bulletId = _ms.ReadUInt();
                    m_SPlayers[m_nLocalSit].LocalFireFail(coin);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_BACKFIRE:
                {
                    byte sit = _ms.ReadByte();
                    float angle = _ms.ReadSingle();
                    byte num = _ms.ReadByte();
                    long leftCoin = _ms.ReadLong();
                    uint lockId = _ms.ReadUInt();
                    m_SPlayers[sit].OtherFire(angle, num, leftCoin, lockId);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_BACKLEAVE:
                {
                    byte sit = _ms.ReadByte();
                    string name = _ms.ReadString();
                    long coin = _ms.ReadLong();
                    m_SPlayers[sit].UpdateInfoUI(coin);

                    if (sit == m_nLocalSit)
                        BackToChooseLevel();
                    else
                    {
                        m_SPlayers[sit].LeaveSit();
                        m_SPlayers.Remove(sit);
                    }

                    DebugLog.Log(name + " leave sit:" + sit);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_OTHERPLAYERENTER:
                {
                    byte sit = _ms.ReadByte();
                    byte clientSit = GetClientSit(sit, m_nLocalSit);
                    m_SPlayers[sit] = m_CPlayerList[clientSit];

                    uint nUserId = _ms.ReadUInt();
                    int faceId = _ms.ReadInt();
                    long coin = _ms.ReadLong();
                    string name = _ms.ReadString();
                    byte curCannon = _ms.ReadByte();
                    byte curLv = _ms.ReadByte();
                    uint bulletCost = _ms.ReadUInt();

                    m_SPlayers[sit].m_nSrvSit = sit;
                    m_SPlayers[sit].SetupInfoUI(false, coin, name, (uint)faceId);
                    m_SPlayers[sit].OnChangeCannon(false, curCannon, curLv, bulletCost);

                    DebugLog.Log(name + " sit:" + sit + ", clientSit:" + clientSit);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_CHANGECANNON:
                {
                    byte sit = _ms.ReadByte();
                    byte cannonId = _ms.ReadByte();
                    uint bulletCost = _ms.ReadUInt();
                    byte curLv = _ms.ReadByte();
                    bool bLocal = (sit == m_nLocalSit);
                    if(bLocal)
                    {
                        byte curEquipId = m_SPlayers[sit].GetCannonTypeId();
                        RefreshSwitchCannonUI(curEquipId, cannonId, false);
                        RefreshSwitchCannonUI(cannonId, cannonId, false);
                    }
                    m_SPlayers[sit].OnChangeCannon(bLocal, cannonId, curLv, bulletCost);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_CHANGECANNONLEVEL:
                {
                    byte state = _ms.ReadByte();
                    if (state != 0)
                    {
                        byte sit = _ms.ReadByte();
                        byte curLv = _ms.ReadByte();
                        uint bulletCost = _ms.ReadUInt();
                        bool bLocal = (sit == m_nLocalSit);
                        m_SPlayers[sit].OnChangeCannonLv(bLocal, curLv, bulletCost);
                    }
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_UPDATEROOMSTATE:
                {
                    OnStateChange((FishGameState_Enum)_ms.ReadByte(), _ms);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_PLAYERBUGCANNON:
                {
                    byte state = _ms.ReadByte();
                    byte id = _ms.ReadByte();
                    uint leftDiamon = _ms.ReadUInt();
                    OnUnlockCannon(state, id, (int)leftDiamon);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_TRACECHANGETARGET:
                {
                    byte sit = _ms.ReadByte();
                    uint onlyId = _ms.ReadUInt();
                    m_SPlayers[sit].ChangeLockFish(FishMgr.GetFishById(onlyId));
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_TRACESKILLTIMEOVER:
                {
                    byte sit = _ms.ReadByte();
                    m_SPlayers[sit].BeginSitLock(false);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_UPDATEUSEFISHINGSKILL:
                {
                    byte sit = _ms.ReadByte();
                    byte id = _ms.ReadByte();
                    if (id == (byte)FishingSkill_enum.FISHINGSKILL_TRACE)
                        m_SPlayers[sit].BeginSitLock(true);
                }
                break;

            case GameCity.EMSG_ENUM.CrazyCityMsg_REWORDFISHINGSKILL:
                {
                    uint userId = _ms.ReadUInt();
                    byte id = _ms.ReadByte();
                    byte num = _ms.ReadByte();
                    RefreshSkill(id, num);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_KICKOUTROOM:
                {
                    byte sit = _ms.ReadByte();
                    string name = _ms.ReadString();
                    long coin = _ms.ReadLong();
                    if (sit == m_nLocalSit)
                        CCustomDialog.OpenCustomConfirmUI(2507, (p) => BackToChooseLevel());
                    else
                    {
                        m_SPlayers[sit].LeaveSit();
                        m_SPlayers.Remove(sit);
                    }

                    DebugLog.Log(name + " is kicked sit:" + sit);
                }
                break;

            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_APPLYCORNUCOPIA:
            case GameCity.EMSG_ENUM.CCMsg_FISHING_SM_APPLYSEALREWORD:
                {
                    byte res = _ms.ReadByte();
                    if(res == 0)
                    {
                        int reward = _ms.ReadInt();
                        byte sit = _ms.ReadByte();
                        if(sit == m_nLocalSit)
                            StartLottery(FishType_Enum.FishType_Init, reward);
                        long coin = _ms.ReadLong();
                        m_SPlayers[sit].UpdateInfoUI(coin);
                    }
                }
                break;

            default:
                break;
        }
        return true;
    }

    void BackToChooseLevel()
    {
        OnEnd();

        m_nCurrenLevel = RoomInfo.NoSit;
        MainUITfm.gameObject.SetActive(false);

#if HAVE_LOBBY        LobbyUITfm.gameObject.SetActive(true);
#else
        GameMain.hall_.SwitchToHallScene();
#endif
    }

    public override void ResetGameUI()
    {
        base.ResetGameUI();

#if HAVE_LOBBY        PlayerData pd = GameMain.hall_.GetPlayerData();
        string coinStr = pd.GetCoin().ToString();

        Transform tfm = LobbyUITfm.FindChild("Bottom/PlayerInfoBG");

        tfm = tfm.parent.FindChild("Image_coinframe");
        tfm.GetComponentInChildren<Text>().text = pd.GetCoin().ToString();

        tfm = tfm.parent.FindChild("Image_DiamondFrame");
        tfm.GetComponentInChildren<Text>().text = pd.GetDiamond().ToString();
#endif

        for (byte skillId = (byte)(FishingSkill_enum.FISHINGSKILL_Init + 1); skillId < (byte)FishingSkill_enum.FISHINGSKILL; skillId++)
        {
            RefreshSkill(skillId, 0);
        }

        LotteryUITfm.gameObject.SetActive(false);
    }


    public override void RefreshGamePlayerCoin(uint AddMoney)
    {
        base.RefreshGamePlayerCoin(AddMoney);

#if HAVE_LOBBY        Transform tfm = LobbyUITfm.FindChild("Bottom/PlayerInfoBG");
        tfm = tfm.parent.FindChild("Image_coinframe");
        long coin;
        long.TryParse(tfm.GetComponentInChildren<Text>().text, out coin);
        coin += AddMoney;
        tfm.GetComponentInChildren<Text>().text = coin.ToString();
#endif
    }

    void LoadResource()
    {
        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((int)GameKind_Enum.GameKind_Fishing);
        if (gamedata == null)
            return;

        FishingAssetBundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);
        if (FishingAssetBundle == null)
            return;

        GameCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        CameraSourcePos = Camera.main.transform.position;
        m_MFCMgr = Camera.main.gameObject.AddComponent<MirrorFlipCamera>();
        Transform root = GameCanvas.transform.Find("Root");

        UnityEngine.Object obj;
        Button butn;
        Transform tfm;
        Sprite sprite;
        byte index = 0;
        Button[] buttons;

        PlayerData pd = GameMain.hall_.GetPlayerData();
        sprite = GameMain.hall_.GetIcon(pd.GetPlayerIconURL(), pd.GetPlayerID(), (int)pd.PlayerIconId);

#if HAVE_LOBBY        //load lobby ui---------------------------------------------
        obj = (GameObject)FishingAssetBundle.LoadAsset("Fishing_Lobby");
        LobbyUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        LobbyUITfm.SetParent(root, false);

        butn = LobbyUITfm.FindChild("Top/ButtonReturn").GetComponent<Button>();
        butn.onClick.AddListener(() => OnClickReturn(true));

        tfm = LobbyUITfm.FindChild("Middle/Middle_Button");
        buttons = tfm.GetComponentsInChildren<Button>();
        index = 1;
        foreach (Button btn in buttons)
        {
            byte temp = index;
            btn.onClick.AddListener(() => OnClickLevelBtn(temp));
            index++;
        }

        tfm = LobbyUITfm.FindChild("Bottom/PlayerInfoBG");

        tfm.FindChild("TextName").GetComponent<Text>().text = pd.GetPlayerName();
        Image vipImg = tfm.FindChild("Image_Vip/Vip_Text/Num").gameObject.GetComponent<Image>();
        Image vipTypeImg = tfm.FindChild("Image_Vip").gameObject.GetComponent<Image>();
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle != null)
        {
            if (pd.GetVipLevel() == 0)
                vipTypeImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_hui");
            else
                vipTypeImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_vip_jin");
            vipImg.sprite = bundle.LoadAsset<Sprite>("zjm_word_sz_vip_" + pd.GetVipLevel().ToString());
        }

        Image icon = tfm.FindChild("Image_HeadFram/Image_Mask/Image_Head").GetComponent<Image>();
        icon.sprite = sprite;

        tfm = tfm.parent.FindChild("Image_coinframe");
        tfm.GetComponentInChildren<Text>().text = pd.GetCoin().ToString();

        tfm = tfm.parent.FindChild("Image_DiamondFrame");
        tfm.GetComponentInChildren<Text>().text = pd.GetDiamond().ToString();
        ////////////////////////////////////////////////////////////////////////
#endif

        //load game ui-----------------------------------------------
        obj = (GameObject)FishingAssetBundle.LoadAsset("Fishing_Game");
        MainUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        MainUITfm.SetParent(root, false);
        MainUITfm.gameObject.SetActive(false);


        butn = MainUITfm.Find("Top_1/ExpandBG/ImageBG/Left_Button/Button_Expand").GetComponent<Button>();
        butn.onClick.AddListener(() => OnClickMenu(0));

        butn = MainUITfm.Find("Top_1/ExpandBG/ImageBG/Left_Button/Button_Narrow").GetComponent<Button>();
        butn.onClick.AddListener(() => OnClickMenu(0));

        butn = MainUITfm.Find("Top_1/ButtonBG").GetComponent<Button>();
        butn.onClick.AddListener(() => OnClickMenu(0));

        buttons = MainUITfm.Find("Top_1/ExpandBG/ImageBG/Right_Button").GetComponentsInChildren<Button>(true);
        index = 1;
        foreach (Button btn in buttons)
        {
            byte temp = index;
            btn.onClick.AddListener(() => OnClickMenu(temp));
            index++;
        }

        m_SkillUITfm = MainUITfm.Find("Middle/ButtonSkillBG");

        Button btn1 = m_SkillUITfm.Find("Button_skill_1").GetComponent<Button>();
        btn1.onClick.AddListener(() => OnClickSkill(FishingSkill_enum.FISHINGSKILL_TRACE, btn1));

        Button btn2 = m_SkillUITfm.Find("Button_skill_2").GetComponent<Button>();
        btn2.onClick.AddListener(() => OnClickSkill(FishingSkill_enum.FISHINGSKILL_FORZEN, btn2));

        Button btnChange = m_SkillUITfm.Find("Button_change").GetComponent<Button>();
        btnChange.onClick.AddListener(() => SwitchCannonUITfm.gameObject.SetActive(true));

        Toggle AutoFire = m_SkillUITfm.Find("Toggle_zidong").GetComponent<Toggle>();
        AutoFire.isOn = false;
        AutoFire.onValueChanged.AddListener(OnAutoFireChange);

        m_SkillUITfm.gameObject.SetActive(false);

        tfm = MainUITfm.Find("Pop-up");
        Slider music = tfm.Find("Set/ImageBG/Slider_Music").gameObject.GetComponent<Slider>();
        Slider sound = tfm.Find("Set/ImageBG/Slider_Sound").gameObject.GetComponent<Slider>();
        music.value = AudioManager.Instance.MusicVolume;
        sound.value = AudioManager.Instance.SoundVolume;
        music.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.MusicVolume = value; });
        sound.onValueChanged.AddListener(delegate (float value) { AudioManager.Instance.SoundVolume = value; });

        SwitchCannonUITfm = tfm.Find("SwitchPaotai");
        SwitchCannonUITfm.gameObject.SetActive(false);

        FishUITfm = tfm.Find("Fish_Info");
        FishUITfm.gameObject.SetActive(false);
        SetFishUI();

        tfm = tfm.Find("Anime");

        obj = FishingAssetBundle.LoadAsset("Anime_boss");
        m_AnimBoss = ((GameObject)GameMain.instantiate(obj)).GetComponent<DragonBones.UnityArmatureComponent>();
        m_AnimBoss.transform.SetParent(tfm, false);
        m_AnimBoss.gameObject.SetActive(false);

        obj = FishingAssetBundle.LoadAsset("Anime_yuqun");
        m_AnimFishArray = ((GameObject)GameMain.instantiate(obj)).GetComponent<DragonBones.UnityArmatureComponent>();
        m_AnimFishArray.transform.SetParent(tfm, false);
        m_AnimFishArray.gameObject.SetActive(false);

        Fishing_Bullet.ParentTfm = MainUITfm.Find("Middle/ImageBG_Bullet");

#if SHOW_TIME
        obj = (GameObject)FishingAssetBundle.LoadAsset("Text_HurtNum");
        m_TimeTfm = ((GameObject)GameMain.instantiate(obj)).transform;
        m_TimeTfm.SetParent(MainUITfm, false);
        m_TimeTfm.localPosition = new Vector3(0, 200, 0);
        m_TimeTfm.gameObject.SetActive(false);
#endif

        obj = (GameObject)FishingAssetBundle.LoadAsset("Fishing_Game_lottery");
        LotteryUITfm = ((GameObject)GameMain.instantiate(obj)).transform;
        LotteryUITfm.SetParent(root, false);
        LotteryUITfm.gameObject.SetActive(false);

        tfm = LotteryUITfm.Find("Image_choose/Imagechoose");
        foreach(Transform child in tfm)
        {
            Button bt = child.GetComponent<Button>();
            bt.onClick.AddListener(() => OnClickLotteryBtn(bt.transform));
        }
    }

    IEnumerator PreloadResource(int levelIndex)
    {
        AssetBundle ab = null;

        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((int)GameKind_Enum.GameKind_Fishing);
        if (gamedata != null)
            ab = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);

        if (ab == null)
            yield break;

#if HAVE_LOBBY        CCustomDialog.OpenCustomWaitUI(1001);

        yield return null;
#endif

        //preload coin
        GameFunction.PreloadPrefab(ab, "FishCoin");
        GameFunction.PreloadPrefab(ab, "FishCoin_big");
        GameFunction.PreloadPrefab(ab, "FishHit");
        GameFunction.PreloadPrefab(ab, "Flshing_Bingdong");

        yield return null;

        //preload path
        foreach (FishingPathData pd in Fishing_Data.GetInstance().m_PathData.Values)
        {
            GameFunction.PreloadPrefab(ab, pd.m_szPath);
        }

        yield return null;

        //preload cannon
        foreach (FishingCannonData cd in Fishing_Data.GetInstance().m_CannonData.Values)
        {
            GameFunction.PreloadPrefab(ab, cd.m_szCannon);
            GameFunction.PreloadPrefab(ab, cd.m_szBullet);
            GameFunction.PreloadPrefab(ab, cd.m_szNet);
        }

        yield return null;

        //preload fish
        foreach (FishingFishData fd in Fishing_Data.GetInstance().m_FishData.Values)
        {
            GameFunction.PreloadPrefab(ab, fd.m_szFish);
        }

        yield return null;

        GameFunction.PreloadPrefab(ab, "ThunderLine");
        GameFunction.PreloadPrefab(ab, "background_001");
        GameFunction.PreloadPrefab(ab, "background_002");
        GameFunction.PreloadPrefab(ab, "background_003");

        yield return null;

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_CHOOSElEVEL);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(levelIndex);
        HallMain.SendMsgToRoomSer(msg);
    }

    void InitPlayers()
    {
        for(byte i = 0; i < PlayerNum; i++)
        {
            m_CPlayerList[i] = new Fishing_Role(this, i);
        }
    }

    void OnClickReturn(bool playsound)
    {
        if (playsound)
            CustomAudioDataManager.GetInstance().PlayAudio(1006);

#if HAVE_LOBBY        if (LobbyUITfm.gameObject.activeSelf)
            GameMain.hall_.SwitchToHallScene();
        else
#endif
        {
            //TryToLeaveRoom
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_APPLYLEAVE);
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);
        }
    }

    void OnClickLevelBtn(byte index)
    {
        if (m_nCurrenLevel == RoomInfo.NoSit)
        {
            m_nCurrenLevel = 255;

            CustomAudioDataManager.GetInstance().PlayAudio(1006);

            GameMain.SC(PreloadResource(index));
        }
    }

    void OnClickMenu(byte btn)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1006);

        RectTransform parent = MainUITfm.Find("Top_1/ExpandBG/ImageBG") as RectTransform;
        Button butn1 = parent.Find("Left_Button/Button_Expand").GetComponent<Button>();
        Button butn2 = parent.Find("Left_Button/Button_Narrow").GetComponent<Button>();
        if (butn1.enabled && butn2.enabled)
        {
            butn1.enabled = false;
            butn2.enabled = false;
            Transform tfm = MainUITfm.Find("Top_1/ButtonBG");
            bool bActive = tfm.gameObject.activeSelf;
            tfm.gameObject.SetActive(true);
            Vector3 vec = parent.localPosition;
            float offset = parent.rect.width * 0.7f;
            vec.x = bActive ? vec.x + offset : vec.x - offset;
            Tweener t = DOTween.To(() => parent.localPosition, r => parent.localPosition = r, vec, 0.5f);
            t.OnComplete(() =>
            {
                butn1.enabled = true;
                butn2.enabled = true;
                butn1.gameObject.SetActive(bActive);
                butn2.gameObject.SetActive(!bActive);
                tfm.gameObject.SetActive(!bActive);
            });
        }

        if (btn == 1)//设置
        {
            MainUITfm.Find("Pop-up/Set").gameObject.SetActive(true);
        }
        else if (btn == 2)//fish
        {
            FishUITfm.gameObject.SetActive(true);
        }
        else if (btn == 3)
        {
            OnClickReturn(false);
        }
    }

    void OnClickSkill(FishingSkill_enum id, Button btn)
    {
        if (btn == null)
            return;

        CustomAudioDataManager.GetInstance().PlayAudio(1006);

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_USEFISHINGSKILL);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add((byte)id);
        HallMain.SendMsgToRoomSer(msg);

        btn.interactable = false;
    }

    bool HandleSkillMsg(uint _msgType, UMessage _ms)
    {
        byte state = _ms.ReadByte();
        byte skillId = _ms.ReadByte();
        byte leftNum = _ms.ReadByte();
        float time = _ms.ReadSingle();

        Button btn = m_SkillUITfm.Find("Button_skill_" + skillId).GetComponent<Button>();
        if(state == 0)
        {
            btn.interactable = true;
            return false;
        }

        FishingSkill_enum skill = (FishingSkill_enum)skillId;
        switch (skill)
        {
            case FishingSkill_enum.FISHINGSKILL_TRACE:
                {
                    btn.transform.Find("Textnumber").GetComponent<Text>().text = leftNum.ToString();
                    Image img = btn.transform.Find("ImageMask").GetComponent<Image>();
                    CCIMgr.AddTimeImage(img, time, 1.0f, OnSkillEnd, img.GetComponentInChildren<Text>(), false, skillId.ToString());
                    btn.interactable = false;
                    m_SPlayers[m_nLocalSit].BeginSitLock(true);
                }
                break;

            case FishingSkill_enum.FISHINGSKILL_FORZEN:
                {
                    btn.transform.Find("Textnumber").GetComponent<Text>().text = leftNum.ToString();
                    Image img = btn.transform.Find("ImageMask").GetComponent<Image>();
                    CCIMgr.AddTimeImage(img, time, 1.0f, OnSkillEnd, img.GetComponentInChildren<Text>(), false, skillId.ToString());
                    btn.interactable = false;
                }
                break;

            default:
                break;
        }

        return true;
    }

    bool OnSkillEnd(byte res, bool clicked, Image img, string userdata)
    {
        img.GetComponentInChildren<Text>().text = "";

        Button btn = img.GetComponentInParent<Button>();
        if (btn == null)
            return true;

        byte leftNum = byte.Parse(btn.transform.Find("Textnumber").GetComponent<Text>().text);
        btn.interactable = (leftNum != 0);

        //FishingSkill_enum skill = (FishingSkill_enum)byte.Parse(userdata);
        return true;
    }

    bool HandleEnterRoom(uint _msgType, UMessage _ms)
    {
#if HAVE_LOBBY        CCustomDialog.CloseCustomWaitUI();
        LobbyUITfm.gameObject.SetActive(false);
#endif

        uint roomId = _ms.ReadUInt();
        m_nCurrenLevel = _ms.ReadByte();
        m_fGameTime = _ms.ReadSingle();

        if(m_nCurrenLevel > 0)
        {
            GameObject obj = (GameObject)FishingAssetBundle.LoadAsset("background_00" + m_nCurrenLevel);
            obj = (GameObject)GameMain.instantiate(obj);
            obj.transform.SetParent(GameObject.Find("Game_Model/BG").transform, false);
            m_AddItems.Add(obj);
        }

        byte num = _ms.ReadByte();
        Dictionary<byte, CannonSwitchData> cannonDict = new Dictionary<byte, CannonSwitchData>();
        for(byte i = 0; i < num; i++)
        {
            byte id = _ms.ReadByte();
            CannonSwitchData csd = new CannonSwitchData();
            cannonDict[id] = csd;
        }

        num = _ms.ReadByte();
        for (byte i = 0; i < num; i++)
        {
            byte id = _ms.ReadByte();
            CannonSwitchData csd = new CannonSwitchData();
            csd.m_bLock = true;
            csd.m_nDiamondLimit = _ms.ReadInt();
            csd.m_VipLimit = _ms.ReadByte();
            cannonDict[id] = csd;
        }

        num = _ms.ReadByte();
        Dictionary<byte, SkillData> skillDict = new Dictionary<byte, SkillData>();
        for (byte i = 0; i < num; i++)
        {
            byte id = _ms.ReadByte();
            SkillData sd = new SkillData();
            sd.m_nLeftNum = _ms.ReadByte();
            skillDict[id] = sd;
        }

        byte localSit = _ms.ReadByte();
        byte sitNum = _ms.ReadByte();
        for(byte i = 0; i < sitNum; i++)
        {
            byte sit = _ms.ReadByte();
            byte clientSit = GetClientSit(sit, localSit);
            m_SPlayers[sit] = m_CPlayerList[clientSit];

            uint nUserId = _ms.ReadUInt();
            int faceId = _ms.ReadInt();
            long coin = _ms.ReadLong();
            string name = _ms.ReadString();
            byte curCannon = _ms.ReadByte();
            byte curLv = _ms.ReadByte();
            uint skillOnlyId = _ms.ReadUInt();
            float leftTime = _ms.ReadSingle();
            uint bulletCost = _ms.ReadUInt();

            m_SPlayers[sit].m_nSrvSit = sit;
            if (sit == localSit)
            {
                m_SPlayers[sit].SetupInfoUI(true, coin, name, (uint)faceId);
                LocalRoleSit(sit, cannonDict, curCannon, curLv, skillDict, bulletCost);
            }
            else
            {
                m_SPlayers[sit].SetupInfoUI(false, coin, name, (uint)faceId);
                m_SPlayers[sit].OnChangeCannon(false, curCannon, curLv, bulletCost);
            }

            if(skillOnlyId > 0)
                m_SPlayers[sit].ChangeLockFish(FishMgr.GetFishById(skillOnlyId));
        }

        ushort fishNum = _ms.ReaduShort();
        for(ushort j = 0; j < fishNum; j++)
        {
            uint onlyId = _ms.ReadUInt();
            ushort pathId = _ms.ReaduShort();
            byte typeId = _ms.ReadByte();
            float pass = _ms.ReadSingle();
            byte group = _ms.ReadByte();
            byte index = _ms.ReadByte();
            FishMgr.CreatePathFish(pathId, onlyId, typeId, pass, group, index);
        }

        MainUITfm.gameObject.SetActive(true);

#if SHOW_TIME
        m_TimeTfm.gameObject.SetActive(true);
#endif

        return true;
    }

    void OnStateChange(FishGameState_Enum state, UMessage _ms)
    {
        if (m_eGameState == state)
            return;

        DebugLog.Log(string.Format("room state change: ({0}->{1})", m_eGameState, state));

        OnQuitState(m_eGameState);

        m_eGameState = state;

        OnEnterState(m_eGameState, _ms);

        m_fGameTime = _ms.ReadSingle();
    }

    void OnQuitState(FishGameState_Enum state)
    {
        switch (state)
        {
            case FishGameState_Enum.FishGameState_Forzen:
                FishMgr.PauseScene(false);
                break;

            default:
                break;
        }
    }

    void OnEnterState(FishGameState_Enum state, UMessage _ms)
    {
        switch (state)
        {
            case FishGameState_Enum.FishGameState_Clear:
                {
                    GameMain.SC(OnSomethinComing(m_AnimFishArray, 2f));
                    FishMgr.ClearScene();
                }
                break;

            case FishGameState_Enum.FishGameState_Forzen:
                FishMgr.PauseScene(true);
                break;

            default:
                break;
        }
    }

    void OnEnd()
    {
        if (GameMain.hall_.isGetRelief)
            GameMain.hall_.ShowRelief();

        GameMain.Instance.StopAllCoroutines();
        m_LotteryCoroutin = null;

        foreach (GameObject obj in m_AddItems)
        {
            GameObject.Destroy(obj);
        }
        m_AddItems.Clear();

        FishMgr.OnEnd();

        LocalRoleLeaveSit();

        foreach(Fishing_Role role in m_CPlayerList)
        {
            role.LeaveSit();
        }
        m_SPlayers.Clear();

        m_AnimBoss.gameObject.SetActive(false);
        m_AnimFishArray.gameObject.SetActive(false);

        m_nLotteryValue = RoomInfo.NoSit;

        ResetGameUI();
    }

    public override void OnDisconnect(bool over)
    {
        OnEnd();
    }

    void SetSwitchCannon(Dictionary<byte, CannonSwitchData> cannonDict, byte equipId)
    {
        Transform tfm = SwitchCannonUITfm.Find("ImageBG/Viewport/Content");
        for (int i = tfm.childCount - 1; i >= 0; i--)
            GameObject.DestroyImmediate(tfm.GetChild(i).gameObject);

        foreach (byte id in cannonDict.Keys)
        {
            FishingCannonData fcd = Fishing_Data.GetInstance().m_CannonData[id];
            CannonSwitchData csd = cannonDict[id];
            GameObject obj = (GameObject)FishingAssetBundle.LoadAsset("SwitchPaotai");
            obj = ((GameObject)GameMain.instantiate(obj));
            Transform cannonUI = obj.transform;
            cannonUI.name = id.ToString();
            cannonUI.SetParent(tfm, false);
            cannonUI.Find("Text_Name").GetComponent<Text>().text = fcd.m_szName;
            cannonUI.Find("Image_details/Text").GetComponent<Text>().text = fcd.m_szDetail;
            GameObject cannon = (GameObject)FishingAssetBundle.LoadAsset(fcd.m_szCannon);
            cannon = ((GameObject)GameMain.instantiate(cannon));
            cannon.transform.SetParent(cannonUI.Find("icon_Anime"), false);
            Button btnEquip = cannonUI.Find("Button_zhuangbei").GetComponent<Button>();
            btnEquip.onClick.AddListener(() => OnClickSwitchCannon(id));
            Button btnVip = cannonUI.Find("Button_jiesuo_1").GetComponent<Button>();
            btnVip.GetComponentInChildren<Text>().text = "Vip" + csd.m_VipLimit + "解锁";
            btnVip.onClick.AddListener(() => OnClickCannonUnlock(id, btnVip));
            Button btnDiamond = cannonUI.Find("Button_jiesuo_2").GetComponent<Button>();
            btnDiamond.GetComponentInChildren<Text>().text = csd.m_nDiamondLimit.ToString();
            btnDiamond.onClick.AddListener(() => OnClickCannonUnlock(id, btnDiamond));

            RefreshSwitchCannonUI(id, equipId, csd.m_bLock);
        }
    }

    void RefreshSwitchCannonUI(byte id, byte equipId, bool bLock)
    {
        Transform cannonUI = SwitchCannonUITfm.Find("ImageBG/Viewport/Content");
        cannonUI = cannonUI.Find(id.ToString());
        if (cannonUI == null)
            return;

        cannonUI.Find("Image_zhuangbei").gameObject.SetActive(id == equipId);
        Button btnEquip = cannonUI.Find("Button_zhuangbei").GetComponent<Button>();
        Button btnVip = cannonUI.Find("Button_jiesuo_1").GetComponent<Button>();
        Button btnDiamond = cannonUI.Find("Button_jiesuo_2").GetComponent<Button>();
        if (bLock)
        {
            PlayerData pd = GameMain.hall_.GetPlayerData();

            btnEquip.gameObject.SetActive(false);

            byte vipLimit;
            byte.TryParse(btnVip.GetComponentInChildren<Text>().text, out vipLimit);
            if(pd.GetVipLevel() >= vipLimit)
            {
                btnVip.gameObject.SetActive(true);
                btnDiamond.gameObject.SetActive(false);
            }
            else
            {
                btnVip.gameObject.SetActive(false);
                btnDiamond.gameObject.SetActive(true);

                int diamondLimit = int.Parse(btnDiamond.GetComponentInChildren<Text>().text);
                btnDiamond.interactable = (pd.GetDiamond() >= diamondLimit);
            }
        }
        else
        {
            btnEquip.gameObject.SetActive(true);
            btnEquip.interactable = (id != equipId);
            btnVip.gameObject.SetActive(false);
            btnDiamond.gameObject.SetActive(false);
        }
    }

    void OnClickSwitchCannon(byte id)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1006);
        if (id != m_SPlayers[m_nLocalSit].GetCannonTypeId())
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_CHANGECANNON);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(id);
            HallMain.SendMsgToRoomSer(msg);
        }
    }

    void OnClickCannonUnlock(byte id, Button btn)
    {
        CustomAudioDataManager.GetInstance().PlayAudio(1006);

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_PLAYERBUGCANNON);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(id);
        HallMain.SendMsgToRoomSer(msg);

        btn.interactable = false;
    }

    void OnUnlockCannon(byte state, byte id, int leftDiamond)
    {
        Transform tfm = SwitchCannonUITfm.Find("ImageBG/Viewport/Content");
        tfm = tfm.Find(id.ToString());
        if (tfm == null)
            return;

        byte curEquipId = m_SPlayers[m_nLocalSit].GetCannonTypeId();
        if (state == 0)
        {
            CCustomDialog.OpenCustomConfirmUI(2506);
            RefreshSwitchCannonUI(id, curEquipId, true);
            return;
        }

        RefreshSwitchCannonUI(id, curEquipId, false);
        m_SPlayers[m_nLocalSit].UpdateInfoUI(-1, leftDiamond);
    }

    public bool LocalRoleSit(byte sit, Dictionary<byte, CannonSwitchData> cannonDict, byte curCannon, byte curLv, Dictionary<byte, SkillData> skillDict, uint bulletCost)
    {
        if (m_nLocalSit == sit)
            return false;

        LocalRoleLeaveSit();

        m_nLocalSit = sit;

        m_MFCMgr.scale = new Vector3(1f, (sit > 1 ? -1f : 1f), 1f);
        GameObject.Find("light").transform.localEulerAngles = new Vector3(0, 0, sit > 1 ? 180f : 0f);

        m_SPlayers[m_nLocalSit].OnChangeCannon(true, curCannon, curLv, bulletCost);

        m_SkillUITfm.gameObject.SetActive(true);

        for(byte skillId = (byte)(FishingSkill_enum.FISHINGSKILL_Init + 1); skillId < (byte)FishingSkill_enum.FISHINGSKILL; skillId++)
        {
            RefreshSkill(skillId, skillDict.ContainsKey(skillId) ? skillDict[skillId].m_nLeftNum : (byte)0);
        }

        SetSwitchCannon(cannonDict, curCannon);

        //CustomAudioDataManager.GetInstance().PlayAudio(1006);

        return true;
    }

    void SetFishUI()
    {
        Transform tfm;
        byte num = (byte)FishType.eFT_Num;
        Transform[] tfmContents = new Transform[num];
        Toggle tg;
        for(int i = 0; i < num; i++)
        {
            tfmContents[i] = FishUITfm.Find("ImageBG/Viewport_" + i + "/Content");
            tfm = FishUITfm.Find("ImageBG/ImageToggleBG/Toggle_" + i);
            if(tfm != null && tfmContents[i] != null)
            {
                tg = tfm.GetComponent<Toggle>();
                GameObject temp = tfmContents[i].parent.gameObject;
                temp.SetActive(tg.isOn);
                tg.onValueChanged.AddListener((bool on) => temp.SetActive(on));
            }
        }

        byte type;
        GameObject obj;
        foreach (FishingFishData fd in Fishing_Data.GetInstance().m_FishData.Values)
        {
            type = (byte)fd.m_eFishType;
            tfm = tfmContents[type];
            if (tfm == null)
                continue;

            obj = (GameObject)FishingAssetBundle.LoadAsset("FishInfo_Content_" + type);
            obj = ((GameObject)GameMain.instantiate(obj));
            obj.transform.SetParent(tfm, false);
            obj.transform.Find("ImageIcon").GetComponent<Image>().sprite =
                FishingAssetBundle.LoadAsset<Sprite>(fd.m_szIcon);
            obj.transform.Find("TextName").GetComponent<Text>().text = fd.m_szName;
            obj.transform.Find("TextMultiple").GetComponent<Text>().text = fd.m_nMultiple + "倍";
        }
    }

    public void LocalRoleLeaveSit()
    {
        if (m_nLocalSit >= PlayerNum || !m_SPlayers.ContainsKey(m_nLocalSit))
            return;

        m_SPlayers[m_nLocalSit].LeaveSit();
        m_nLocalSit = RoomInfo.NoSit;
        m_SkillUITfm.gameObject.SetActive(false);
    }

    public bool IsLocal(byte index)
    {
        return m_nLocalSit == index;
    }

    public bool IsMirror()
    {
        return m_nLocalSit > 1 && m_nLocalSit < PlayerNum;
    }

    public void OnLockFishLost(List<byte> sitList)
    {
        foreach(byte sit in sitList)
        {            if(m_SPlayers.ContainsKey(sit))                m_SPlayers[sit].OnLockFishLost();
        }    }

    void OnAutoFireChange(bool on)
    {
        if (m_nLocalSit >= PlayerNum || !m_SPlayers.ContainsKey(m_nLocalSit))
            return;

        m_SPlayers[m_nLocalSit].OnAutoFireChange(on);
    }

    bool HandleFireResult(uint _msgType, UMessage _ms)
    {
        long leftCoin = _ms.ReadLong();
        byte sit = _ms.ReadByte();
        uint fishId = _ms.ReadUInt();
        long reward = _ms.ReadLong();
        byte skillID = _ms.ReadByte();
        byte skillNum = _ms.ReadByte();
        FishType_Enum fishType = (FishType_Enum)_ms.ReadByte();
        int specialId = _ms.ReadInt();
        m_SPlayers[sit].OnCatch(fishId, reward, leftCoin, fishType, skillID, specialId);
        if (skillID > 0 && sit == m_nLocalSit)
            RefreshSkill(skillID, skillNum);

        long total = reward;
        byte otherKillNum = _ms.ReadByte();
        List<uint> otherList = new List<uint>();
        uint otherFishId;
        for (byte i = 0; i < otherKillNum; i++)
        {
            otherFishId = _ms.ReadUInt();
            reward = _ms.ReadLong();
            m_SPlayers[sit].OnCatch(otherFishId, reward, -1);
            otherList.Add(otherFishId);
            total += reward;
        }

        if(otherKillNum > 0)
        {
            DebugLog.Log("Special fish:" + fishType + " otherKillNum:" + otherKillNum);

            if (fishType == FishType_Enum.FishType_AllInOne)
            {
                GameObject obj = (GameObject)FishingAssetBundle.LoadAsset("ThunderLine");
                obj = ((GameObject)GameMain.instantiate(obj));
                LineRenderer lr = obj.GetComponent<LineRenderer>();
                if (lr != null)
                {
                    UVChainLightning chain = obj.AddComponent<UVChainLightning>();
                    Fishing_Fish fish = FishMgr.GetFishById(fishId);
                    if(fish != null)
                        chain.posList.Add(fish.transform.position);
                    foreach (uint id in otherList)
                    {
                        fish = FishMgr.GetFishById(id);
                        if(fish != null)
                            chain.posList.Add(fish.transform.position);
                    }
                }

                m_AddItems.Add(obj);
                GameMain.WaitForCall(1f, () =>
                {
                    m_AddItems.Remove(obj);
                    GameMain.Destroy(obj);
                });
            }

            m_SPlayers[sit].OnSpecailReward(total, 3f);
        }
        return true;
    }

    public void SetAutoFire(bool on)
    {
        Toggle AutoFire = m_SkillUITfm.Find("Toggle_zidong").GetComponent<Toggle>();
        AutoFire.isOn = on;
    }

    void RefreshSkill(byte skillId, byte num)
    {
        Transform tfm = m_SkillUITfm.Find("Button_skill_" + skillId);
        if (tfm == null)
            return;

        Button btn = tfm.GetComponent<Button>();
        if (btn == null)
            return;

        btn.transform.Find("Textnumber").GetComponent<Text>().text = num.ToString();
        btn.interactable = (num != 0);

        Image img = btn.transform.Find("ImageMask").GetComponent<Image>();
        CCIMgr.RemoveTimeImage(img, true);
    }

    IEnumerator OnSomethinComing(DragonBones.UnityArmatureComponent anime, float loopTime)
    {
        if (anime.gameObject.activeSelf)
            yield break;

        anime.gameObject.SetActive(true);
        anime.animation.Play("chu_xian");

        yield return new WaitForSecondsRealtime(1f);

        anime.animation.Play("hun_huan");

        yield return new WaitForSecondsRealtime(loopTime);

        anime.animation.Play("xiao_shi");

        yield return new WaitForSecondsRealtime(1f);

        anime.gameObject.SetActive(false);
    }

    byte GetClientSit(byte sit, byte localSit)
    {
        if (localSit <= 1)
            return sit;
        return (byte)(3 - sit);
    }

    public override void ReconnectSuccess()
    {
        base.ReconnectSuccess();

        CCustomDialog.OpenCustomConfirmUI(1018, (p) => BackToChooseLevel());
    }

    IEnumerator OnLottery(FishType_Enum type, int ratio)
    {
        if (LotteryUITfm == null)
            yield break;

        LotteryUITfm.gameObject.SetActive(true);
        foreach (Transform child in LotteryUITfm)
            child.gameObject.SetActive(false);

        Transform tfm;
        int temp = ratio;
        byte num;
        float time;
        if (type == FishType_Enum.FishType_Cornucopia)
        {
            tfm = LotteryUITfm.Find("Image_choose");
            tfm.gameObject.SetActive(true);
            Transform btnTfm = tfm.Find("Imagechoose");
            foreach (Transform child in btnTfm)
                child.Find("Image").gameObject.SetActive(true);
            Transform boardTfm = tfm.Find("ImageBG");
            foreach (Transform child in boardTfm)
                child.GetComponent<Text>().text = ".";

            Transform popUp = LotteryUITfm.Find("Pop-up");
            tfm = popUp.Find("Image_lottery_bg");
            foreach (Transform child in tfm)
                GameObject.Destroy(child.gameObject);
            GameObject flyObj = (GameObject)FishingAssetBundle.LoadAsset("ImageNum_lottery");
            flyObj = (GameObject)GameMain.instantiate(flyObj);
            flyObj.transform.SetParent(tfm, false);

            for (int i = 0; i < 4; i++)
            {
                num = (byte)(temp % 10);
                m_nLotteryValue = num;
                temp /= 10;
                time = 5f;

                yield return new WaitForEndOfFrame();
                yield return new WaitUntil(() =>
                {
                    time -= Time.deltaTime;
                    if(time < 0f)
                        OnClickLotteryBtn(btnTfm.GetChild(Random.Range(0, btnTfm.childCount)));

                    return m_nLotteryValue > 9;
                });

                yield return new WaitForSecondsRealtime(0.3f);

                popUp.gameObject.SetActive(true);
                tfm = btnTfm.GetChild(m_nLotteryValue - 100);
                flyObj.transform.localPosition = tfm.localPosition;
                flyObj.GetComponentInChildren<Text>().text = num.ToString();

                tfm = boardTfm.GetChild(i);
                flyObj.transform.DOLocalMove(tfm.localPosition + boardTfm.localPosition, 0.5f);

                yield return new WaitForSecondsRealtime(0.5f);

                tfm.GetComponent<Text>().text = num.ToString();
                popUp.gameObject.SetActive(false);
            }

            yield return new WaitForSecondsRealtime(0.5f);

            m_nLotteryValue = RoomInfo.NoSit;
            GameObject.Destroy(flyObj);

            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_APPLYCORNUCOPIA);
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);
        }
        else if(type == FishType_Enum.FishType_Seals)
        {
            tfm = LotteryUITfm.Find("Image_crotate");
            tfm.gameObject.SetActive(true);

            tfm = tfm.Find("ImageBG");
            time = 3f;
            while(time > 0f)
            {
                time -= Time.deltaTime;
                foreach (Transform child in tfm)
                    child.GetComponent<Text>().text = Random.Range(0, 10).ToString();
                yield return new WaitForEndOfFrame();
            }

            for (int i = 1; i >= 0; i--)
            {
                num = (byte)(temp % 10);
                temp /= 10;
                tfm.GetChild(i).GetComponent<Text>().text = num.ToString();
            }

            yield return new WaitForSecondsRealtime(0.5f);

            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_APPLYSEALREWORD);
            msg.Add(GameMain.hall_.GetPlayerId());
            HallMain.SendMsgToRoomSer(msg);
        }
        else
        {
            tfm = LotteryUITfm.Find("Image_show");
            tfm.gameObject.SetActive(true);

            tfm.Find("ImageBG/Text").GetComponent<Text>().text = ratio.ToString();

            yield return new WaitForSecondsRealtime(1f);

            tfm.gameObject.SetActive(false);
        }

        LotteryUITfm.gameObject.SetActive(false);
    }

    void OnClickLotteryBtn(Transform btn)
    {
        if (m_nLotteryValue > 9)
            return;

        GameObject obj = btn.Find("Image").gameObject;
        if (!obj.activeSelf)
            return;

        obj.SetActive(false);
        btn.GetComponentInChildren<Text>().text = m_nLotteryValue.ToString();
        m_nLotteryValue = (byte)(100 + btn.transform.GetSiblingIndex());//for remember index
    }

    public void StartLottery(FishType_Enum type, int ratio)
    {
        if (m_LotteryCoroutin != null)
            GameMain.Instance.StopCoroutine(m_LotteryCoroutin);
        m_LotteryCoroutin = GameMain.SC(OnLottery(type, ratio));
    }
}







