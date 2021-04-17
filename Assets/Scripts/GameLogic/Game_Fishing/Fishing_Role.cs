using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USocket.Messages;
using XLua;

[LuaCallCSharp]
public enum LockFishState
{
    LFS_None,
    LFS_Begin,
    LFS_Locking,
    LFS_End
};


[Hotfix]
public class Fishing_Role
{
    byte m_nIndex;
    public byte m_nSrvSit = RoomInfo.NoSit;
    public long m_nTotalCoin = 0;
    int m_nDiamond;

    Transform m_InfoUI = null;
    Transform m_SitUI = null;
    Button m_LvAddBtn = null;
    Button m_LvSubBtn = null;
    Text m_SpecailCoinText = null;
    Fishing_Cannon m_Cannon = null;
    LockFishState m_eLockState = LockFishState.LFS_None;
    public Fishing_Fish LockFish { get; private set; }
    public Transform LockUI { get; private set; }

    public CGame_Fishing GameBase { get; private set; }

    public Fishing_Role(CGame_Fishing game, byte index)
    {
        GameBase = game;
        m_nIndex = index;

        Transform tfm = GameBase.MainUITfm.Find("Middle/playerPoint_" + (index + 1));
        m_SitUI = tfm.Find("Button_sit");
        m_InfoUI = tfm.Find("playerinfo");
        tfm = m_InfoUI.Find("Middle/ButtonBG_lv");

        m_SitUI.GetComponent<Button>().onClick.AddListener(OnClickSit);
        m_LvAddBtn = tfm.Find("Button_add").GetComponent<Button>();
        m_LvAddBtn.onClick.AddListener(()=>OnClickLevel(1));
        m_LvSubBtn = tfm.Find("Button_subtract").GetComponent<Button>();
        m_LvSubBtn.onClick.AddListener(() => OnClickLevel(0));

        GameObject obj = (GameObject)game.FishingAssetBundle.LoadAsset("Icon_LockTarget");
        obj = GameMain.Instantiate(obj);
        LockUI = obj.transform;
        LockUI.SetParent(game.GameCanvas.transform.Find("Root"), false);
        obj.SetActive(false); 
        
        obj = (GameObject)game.FishingAssetBundle.LoadAsset("Anime_7cai");
        obj = GameMain.Instantiate(obj);
        obj.transform.SetParent(m_InfoUI.parent.Find("AnimePoint_7cai"), false);
        obj.SetActive(false);
        m_SpecailCoinText = obj.GetComponentInChildren<Text>();
    }

    public virtual void OnTick()
    {
        UpdateLockState();
    }

    public bool IsLockFish()
    {
        return m_eLockState != LockFishState.LFS_None;
    }

    void UpdateLockState()
    {
        if (m_eLockState == LockFishState.LFS_Begin)
        {
            if(IsLocal())
            {
                if (LockFish == null)
                    ChangeLockFish(GameBase.FishMgr.LockNextFish(m_nIndex));
            }

            if (LockFish != null)
            {
                m_eLockState = LockFishState.LFS_Locking;
                LockUI.gameObject.SetActive(true);
            }
        }

        if (m_eLockState == LockFishState.LFS_End)
        {
            OnLockFishLost();
            LockUI.gameObject.SetActive(false);
            m_eLockState = LockFishState.LFS_None;
        }

        if(m_eLockState == LockFishState.LFS_Locking)
        {
            if(IsLocal())
            {
                if (LockFish == null || !LockFish.IsLockBySit(m_nIndex, -100f))
                {
                    //DebugLog.Log("locking->begin curfish:" + (LockFish == null ? 0 : LockFish.m_nOnlyId));

                    LockUI.gameObject.SetActive(false);
                    m_eLockState = LockFishState.LFS_Begin;
                    OnLockFishLost();
                }
            }

            if (LockFish != null)
            {
                Canvas cv = GameBase.GameCanvas;
                LockUI.localPosition = GameFunction.WorldToLocalPointInRectangle(LockFish.GetLockPoint().position, Camera.main, cv, cv.worldCamera);
            }
        }
    }

    public void SetupInfoUI(bool bLocal, long coin, string name = "", uint faceId = 0, string url = "")
    {
        Transform tfm;

        if(!string.IsNullOrEmpty(url) || faceId != 0)
        {
            tfm = m_InfoUI.Find("left/Head/HeadMask/ImageHead");
            tfm.GetComponent<Image>().sprite = GameMain.hall_.GetIcon(url, GameMain.hall_.GetPlayerId(), (int)faceId);
        }

        if (!string.IsNullOrEmpty(name))
        {
            tfm = m_InfoUI.Find("left/TextName");
            tfm.GetComponent<Text>().text = name;
        }

        m_nTotalCoin = coin;

        m_InfoUI.Find("left/ImageBG/ImageBG_own").gameObject.SetActive(bLocal);
        m_InfoUI.Find("left/ImageBG/ImageBG_other").gameObject.SetActive(!bLocal);
        Transform tfmOwnCoin = m_InfoUI.Find("right/Own/Image_coinframe");
        tfmOwnCoin.gameObject.SetActive(bLocal);
        Transform tfmOtherCoin = m_InfoUI.Find("left/Image_coinframe");
        tfmOtherCoin.gameObject.SetActive(!bLocal);
        if (bLocal)
        {
            tfm = m_InfoUI.Find("right/Own/Image_DiamondFrame/Text_Diamond");
            tfm.GetComponent<Text>().text = GameMain.hall_.GetPlayerData().GetDiamond().ToString();
            tfmOwnCoin.GetComponentInChildren<Text>().text = m_nTotalCoin.ToString();

            tfm = m_InfoUI.Find("Middle/Anime");
            GameObject obj = (GameObject)GameBase.FishingAssetBundle.LoadAsset("Anime_Sit");
            obj = GameMain.Instantiate(obj);
            obj.transform.SetParent(tfm, false);
            obj.GetComponent<DragonBones.UnityArmatureComponent>()
                .animation.Play(m_nIndex > 1 ? "weizhi_up" : "weizhi_do");
            GameMain.Destroy(obj, 5f);
        }
        else
        {
            tfmOtherCoin.GetComponentInChildren<Text>().text = m_nTotalCoin.ToString();
        }

        ShowPlayerInfo();
    }

    public void UpdateInfoUI(long coin, int diamond = -1)
    {
        if(coin >= 0)
        {
            m_nTotalCoin = coin;
            if(IsLocal())
            {
                Transform tfm = m_InfoUI.Find("right/Own/Image_coinframe/Text_Coin");
                tfm.GetComponent<Text>().text = m_nTotalCoin.ToString();
            }
            else
            {
                Transform tfm = m_InfoUI.Find("left/Image_coinframe/Text_Coin");
                tfm.GetComponent<Text>().text = m_nTotalCoin.ToString();
            }
        }

        if (diamond >= 0)
        {
            m_nDiamond = diamond;
            Transform tfm = m_InfoUI.Find("right/Own/Image_DiamondFrame/Text_Diamond");
            tfm.GetComponent<Text>().text = m_nDiamond.ToString();
        }
    }

    bool ShowPlayerInfo(bool show = true)
    {
        if ( m_InfoUI == null)
            return false;

        m_InfoUI.gameObject.SetActive(show);
        m_SitUI.gameObject.SetActive(!show);

        return true;
    }

    void OnClickSit()
    {
        //if (m_nSrvSit < CGame_Fishing.PlayerNum)
        //    return;

        //GameBase.LocalRoleSit(m_nIndex);
    }

    public void LeaveSit()
    {
        if (m_nSrvSit >= CGame_Fishing.PlayerNum)
            return;

        OnLockFishLost();
        LockUI.gameObject.SetActive(false);
        m_eLockState = LockFishState.LFS_None;

        Transform tfm = m_InfoUI.Find("Middle/Anime");
        foreach (Transform child in tfm)
            GameMain.Destroy(child.gameObject);

        GameObject.Destroy(m_Cannon.gameObject);
        ShowPlayerInfo(false);
        m_nSrvSit = RoomInfo.NoSit;
        m_nTotalCoin = 0;
    }

    public Vector3 GetCoinPos()
    {
        //Vector3 pos = m_InfoUI.FindChild("left/Image_coinframe/Image_Coin").position;
        Vector3 pos = m_InfoUI.position;
        Canvas cv = GameBase.GameCanvas;
        Vector3 uiPos = GameFunction.WorldToLocalPointInRectangle(pos, cv.worldCamera, cv, cv.worldCamera);
        return uiPos;
    }

    public bool IsLocal()
    {
        return GameBase.IsLocal(m_nSrvSit);
    }

    public void OnClickLevel(byte nAdd)
    {
        if (m_Cannon == null)
            return;

        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_CHANGECANNONLEVEL);
        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(nAdd);
        HallMain.SendMsgToRoomSer(msg);

        if(nAdd > 0)
            m_LvAddBtn.interactable = false;
        else
            m_LvSubBtn.interactable = false;
    }

    public void OnChangeCannonLv(bool bLocal, byte level, uint bulletCost)
    {
        if (m_Cannon == null)
            return;

        if(bLocal)
        {
            m_LvAddBtn.interactable = true;
            m_LvSubBtn.interactable = true;
        }

        Transform tfm = m_InfoUI.Find("Middle/ButtonBG_lv/ImageBG/TextNum");
        tfm.GetComponent<Text>().text = bulletCost.ToString();

        m_Cannon.ChangeLevel(bLocal, level, bulletCost);
    }


    public byte GetCannonTypeId()
    {
        if (m_Cannon == null)
            return 0;
        return m_Cannon.m_nTypeId;
    }

    public void OnChangeCannon(bool bLocal, byte typeId, byte level, uint bulletCost)
    {
        if(m_Cannon != null)
            GameObject.Destroy(m_Cannon.gameObject);

        Transform tfm = m_InfoUI.Find("Anime_paotai");
        m_Cannon = Fishing_Cannon.Create(GameBase.FishingAssetBundle, tfm, typeId, level, bLocal ? bulletCost : 0);
        m_Cannon.BelongRole = this;

        tfm = m_InfoUI.Find("right/Own");
        tfm.gameObject.SetActive(bLocal);

        tfm = m_InfoUI.Find("Middle/ButtonBG_lv/ImageBG/TextNum");
        tfm.GetComponent<Text>().text = bulletCost.ToString();

        m_LvAddBtn.transform.parent.gameObject.SetActive(bLocal);

        if (bLocal)
        {
            m_LvAddBtn.interactable = true;
            m_LvSubBtn.interactable = true;
            GameBase.SetAutoFire(false);
        }
    }

    public void BeginSitLock(bool bLock)
    {
        if (m_nSrvSit >= CGame_Fishing.PlayerNum)
            return;

        m_eLockState = bLock ? LockFishState.LFS_Begin : LockFishState.LFS_End;
    }

    public void OnLockFishLost()
    {
        LockFish = null;
    }

    public void ChangeLockFish(Fishing_Fish fish)
    {
        if (fish == null)
            return;

        if(LockFish != null)
            LockFish.Unlock(m_nIndex);

        LockFish = fish;

        LockFish.Lock(m_nIndex);

        if (IsLocal())
        {
            UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CCMsg_FISHING_CM_TRACECHANGETARGET);
            msg.Add(GameMain.hall_.GetPlayerId());
            msg.Add(LockFish.m_nOnlyId);
            HallMain.SendMsgToRoomSer(msg);
        }
    }

    public void OnAutoFireChange(bool on)
    {
        if (m_Cannon == null)
            return;

        m_Cannon.AutoFire = on;
    }

    public void LocalFireFail(long coin)
    {
        UpdateInfoUI(coin);
        m_Cannon.LocalFireFail();
    }

    public void OtherFire(float angle, byte num, long coin, uint lockId)
    {
        UpdateInfoUI(coin);

        Fishing_Fish fish = GameBase.FishMgr.GetFishById(lockId);
        ChangeLockFish(fish);

        if (m_nIndex > 1)
            angle = 180f - angle;
        m_Cannon.OtherFire(angle, num, fish == null ? null : fish.gameObject);
    }

    public void OnCatch(uint fishId, long reward, long leftCoin, FishType_Enum fishType = FishType_Enum.FishType_Init, byte getSkillId = 0, int specialId = 0)
    {
        UpdateInfoUI(leftCoin);
        Fishing_Fish fish = GameBase.FishMgr.GetFishById(fishId);
        if(fish != null)
        {
            fish.OnCatch(m_Cannon, IsLocal(), reward, fishType, getSkillId, specialId);
        }
    }

    public void OnSpecailReward(long reward, float showTime)
    {
        m_SpecailCoinText.transform.parent.gameObject.SetActive(true);
        m_SpecailCoinText.text = reward.ToString();
        GameMain.WaitForCall(showTime, () => m_SpecailCoinText.transform.parent.gameObject.SetActive(false));
    }
}
