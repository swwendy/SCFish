using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using USocket.Messages;
using XLua;[Hotfix]public class Activity
{
    Transform SignInTfm, OldPlayerTfm;
    byte m_nSignState = 0;
    byte m_nWeekAddState = 0;
    Transform AwardTfm;
    GameObject m_OldPlayerBtn;

    public Activity()
    {
        RegisterMsgHangle();
    }

    public void InitUI(GameObject mainui)
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        if (bundle)
        {
            Transform tfm = mainui.transform.Find("Panelbottom/Left");            Button btn = tfm.Find("Button_checkin").GetComponent<Button>();            btn.onClick.RemoveAllListeners();            btn.onClick.AddListener(() => ShowSignIn(true));            m_OldPlayerBtn = tfm.Find("Button_oldplayer").gameObject;
            btn = m_OldPlayerBtn.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();            btn.onClick.AddListener(() => ShowOldPlayer(true));

            UnityEngine.Object obj;
            if(SignInTfm == null)
            {
                obj = (GameObject)bundle.LoadAsset("Activity_checkin");
                SignInTfm = ((GameObject)GameMain.instantiate(obj)).transform;
                SignInTfm.SetParent(mainui.transform.parent, false);

                SignInTfm.Find("ImageBG/Button_get").GetComponent<Button>().onClick.AddListener(OnClickSign);

                SetupWeekSign();

                ShowSignIn(m_nSignState > 0 || m_nWeekAddState > 0);
            }

            if(OldPlayerTfm == null)
            {
                obj = (GameObject)bundle.LoadAsset("Activity_oldplayer");
                OldPlayerTfm = ((GameObject)GameMain.instantiate(obj)).transform;
                OldPlayerTfm.SetParent(mainui.transform.parent, false);
                OldPlayerTfm.Find("ImageBG/cdkey/Button_Confirm").GetComponent<Button>().onClick.AddListener(OnClickCDKey);
                OldPlayerTfm.Find("ImageBG/gift_1/Button_group/Button_get").GetComponent<Button>().onClick.AddListener(OnClickGift);
                SetupOldPlayer();
                ShowOldPlayer(false);
            }

            if(AwardTfm == null)
            {
                obj = (GameObject)bundle.LoadAsset("Tips_Buy");
                AwardTfm = ((GameObject)GameMain.instantiate(obj)).transform;
                AwardTfm.SetParent(SignInTfm.parent, false);
                AwardTfm.Find("ImageBG/GroupText/TextTop").GetComponent<Text>().text = "领取成功";
                AwardTfm.gameObject.SetActive(false);
            }
        }
    }

    void RegisterMsgHangle()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
               (uint)GameCity.EMSG_ENUM.CrazyCityMsg_BACKWEEKOROLDREWORD, BackWeekOrOldReward); //
        CMsgDispatcher.GetInstance().RegMsgDictionary(
               (uint)GameCity.EMSG_ENUM.CrazyCityMsg_SM_PLAYERAPPLYBINDINVITE, HandlePlayerSetInviteCode);
    }

    bool BackWeekOrOldReward(uint _msgType, UMessage _ms)
    {
        byte sign = _ms.ReadByte();//1：签到或累计登陆 2:老用户
        long coin = _ms.ReadLong();
        GameMain.hall_.GetPlayerData().SetCoin(coin);
        long addCoin = _ms.ReadLong();

        byte state1 = _ms.ReadByte();//0:成功 1：已领取
        byte reward1 = _ms.ReadByte();

        if (sign == 1)
        {
            byte type = 3;
            if (state1 == 0)
                type = 1;
            byte state2 = _ms.ReadByte();//0:成功 1：已领取
            byte reward2 = _ms.ReadByte();
            if (state2 == 0)
            {
                if (type == 1)
                    type = 0;
                else
                    type = 2;
            }
            OnSignReward(type, reward1, reward2, addCoin);
        }
        else
        {
            if(state1 == 0)
                OnOldPlayerReward(reward1, addCoin);
        }
        return true;
    }

    void SetupWeekSign(int type = 0)//type 0:all 1:signIn 2:weekAdd
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        PlayerData playerdata = GameMain.hall_.GetPlayerData();
        Transform tfm;
        UnityEngine.Object effectObj;
        GameObject effect;
        Transform childTfm;
        bool received;

        if(type == 0 || type == 1)
        {
            m_nSignState = 0;

            tfm = SignInTfm.Find("ImageBG/checkin_everyday");
            int index = 0;
            DateTime currentTime = DateTime.Now;
            DayOfWeek weekday = currentTime.DayOfWeek;
            int day = (weekday == DayOfWeek.Sunday) ? 7 : (int)weekday;
            foreach (Transform child in tfm)
            {
                received = GameKind.HasFlag(++index, playerdata.weekSign);
                child.Find("IconBuy").gameObject.SetActive(received);
                childTfm = child.Find("Anime");
                foreach (Transform ch in childTfm)
                    GameObject.Destroy(ch.gameObject);

                if (!received && index == day)
                {
                    effectObj = (GameObject)bundle.LoadAsset("Anime_Actchoose");
                    effect = ((GameObject)GameMain.instantiate(effectObj));
                    effect.transform.SetParent(childTfm, false);
                    m_nSignState = 1;
                }
            }
        }

        if (type == 0 || type == 2)
        {
            m_nWeekAddState = 0;

            tfm = SignInTfm.Find("ImageBG/checkin_cumulative");
            tfm.Find("ImageBG/ImageStrip").GetComponent<Image>().fillAmount = playerdata.weekAdd / 7f;
            int[] weekAddWard = new int[] { 3, 5, 7 };
            for (int i = 0; i < 3; i++)
            {
                received = GameKind.HasFlag(i, playerdata.addUpWard);
                Transform child = tfm.Find("gift_" + (i + 1));
                child.Find("IconBuy").gameObject.SetActive(received);
                child.Find("Imageicon").gameObject.SetActive(false);
                childTfm = child.Find("Anime");
                foreach (Transform ch in childTfm)
                    GameObject.Destroy(ch.gameObject);
                if (!received)
                {
                    if(weekAddWard[i] <= playerdata.weekAdd)
                    {
                        effectObj = (GameObject)bundle.LoadAsset("Anime_checkin_box");
                        effect = ((GameObject)GameMain.instantiate(effectObj));
                        effect.transform.SetParent(childTfm, false);
                        m_nWeekAddState++;
                    }
                    else
                        child.Find("Imageicon").gameObject.SetActive(true);
                }
            }
        }

        SignInTfm.Find("ImageBG/Button_get").GetComponent<Button>().interactable = (m_nSignState > 0 || m_nWeekAddState > 0);
    }

    void SetupOldPlayer()
    {
        PlayerData playerdata = GameMain.hall_.GetPlayerData();
        bool isBind = playerdata.nIsBindInvite > 0;
        Transform tfm = OldPlayerTfm.Find("ImageBG/cdkey");
        tfm.gameObject.SetActive(!isBind);
        tfm = OldPlayerTfm.Find("ImageBG/gift_1");
        tfm.gameObject.SetActive(isBind);
        int index = playerdata.nIsBindInvite - 99;
        if (index > 7)
        {
            m_OldPlayerBtn.SetActive(false);
            index = 7;
            playerdata.nIsBindAward = 1;
        }

        tfm.Find("TextName").GetComponent<Text>().text = index.ToString() + "/7";
        if (playerdata.nIsBindAward > 0)
        {
            uint coin = index == 7 ? playerdata.nTodayAward : playerdata.nTomorrowAward;
            tfm.Find("Textinfo").GetComponent<Text>().text = coin.ToString() + "金币";
            tfm.Find("Button_group/Button_get").gameObject.SetActive(false);
            tfm = tfm.Find("Button_group/Text_tips");
            tfm.gameObject.SetActive(true);
            tfm.GetComponent<Text>().text = (index != 7) ? "明日登录可领取" : "已领取所有礼包";
        }
        else
        {
            uint coin = playerdata.nTodayAward;
            tfm.Find("Textinfo").GetComponent<Text>().text = coin.ToString() + "金币";
            tfm.Find("Button_group/Button_get").gameObject.SetActive(true);
            tfm.Find("Button_group/Text_tips").gameObject.SetActive(false);
        }
    }

    void ShowSignIn(bool show)
    {
        SignInTfm.gameObject.SetActive(show);
    }

    void ShowOldPlayer(bool show)
    {
        OldPlayerTfm.gameObject.SetActive(show);
    }

    void OnClickSign()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_GETWEEKOROLDREWORD);        msg.Add((byte)1);        msg.Add(GameMain.hall_.GetPlayerId());
        NetWorkClient.GetInstance().SendMsg(msg);
    }

    void OnClickCDKey()
    {
        Transform tfm = OldPlayerTfm.Find("ImageBG/cdkey/InputField");
        InputField field = tfm.GetComponent<InputField>();
        if(field.text.Length != 6)
        {
            CCustomDialog.OpenCustomConfirmUI(1402);
            return;
        }
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_CM_PLAYERAPPLYBINDINVITE);        msg.Add(GameMain.hall_.GetPlayerId());
        msg.Add(field.text);        NetWorkClient.GetInstance().SendMsg(msg);
    }

    void OnClickGift()
    {
        UMessage msg = new UMessage((uint)GameCity.EMSG_ENUM.CrazyCityMsg_GETWEEKOROLDREWORD);        msg.Add((byte)2);        msg.Add(GameMain.hall_.GetPlayerId());
        NetWorkClient.GetInstance().SendMsg(msg);
    }

    public void OnSignReward(byte type, byte signReward, byte addReward, long addCoin)//type 0:all 1:signIn 2:weekAdd >2:fail
    {
        if (type > 2)
            return;

        if (type == 0 || type == 1)
        {
            PlayerData playerdata = GameMain.hall_.GetPlayerData();
            playerdata.weekSign = signReward;
        }

        if(type == 0 || type == 2)
        {
            PlayerData playerdata = GameMain.hall_.GetPlayerData();
            playerdata.addUpWard = addReward;
        }

        SetupWeekSign(type);

        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.HallAssetbundleName);
        UnityEngine.Object obj = (GameObject)bundle.LoadAsset("Coin_Effect");
        Transform tfm = ((GameObject)GameMain.instantiate(obj)).transform;
        tfm.SetParent(SignInTfm.Find("ImageBG/Anime"), false);

        AudioManager.Instance.PlaySound(GameDefine.HallAssetbundleName, "lingjiang");
        GameMain.WaitForCall(1f, () =>
        {
            GameObject.Destroy(tfm.gameObject);
            ShowAward(addCoin);
        });
    }

    /// 处理绑定邀请码的回复
    private bool HandlePlayerSetInviteCode(uint _msgType, UMessage _ms)
    {
        byte isSuccess = _ms.ReadByte();
        if (isSuccess == 0)
        {
            PlayerData playerdata = GameMain.hall_.GetPlayerData();
            playerdata.nIsBindInvite = 100;
            uint userid = _ms.ReadUInt();
            playerdata.nTodayAward = _ms.ReadUInt();
            playerdata.nTomorrowAward = _ms.ReadUInt();
            SetupOldPlayer();
        }
        else//1:already 2:used 3:none
        {
            CCustomDialog.OpenCustomConfirmUI((uint)1399 + isSuccess);
        }
        return true;
    }

    void OnOldPlayerReward(byte reward, long addCoin)
    {
        PlayerData playerdata = GameMain.hall_.GetPlayerData();
        playerdata.nIsBindInvite = reward;
        playerdata.nIsBindAward = 1;
        SetupOldPlayer();
        ShowAward(addCoin);
    }

    void ShowAward(long addCoin)
    {
        AwardTfm.Find("ImageBG/TextInfo").GetComponent<Text>().text = addCoin.ToString() + "金币";
        AwardTfm.gameObject.SetActive(true);

        GameMain.hall_.RefreshPlayerCurrency();
    }
}
