﻿using System.Collections;

    Transform m_MailinfoTransform;

            data.gamekind = msg.ReadByte();

            byte flag = msg.ReadByte();
                if(GameKind.HasFlag(0, flag))
                    rewardcontent += "大师分:" + data.masterReward.ToString();
                    rewardcontent += " 现金红包:" + data.redbag.ToString() + "元";
                if (GameKind.HasFlag(1, flag) || GameKind.HasFlag(2, flag))
                    rewardcontent += " 钻石:" + (data.diamondReward + data.coinReward).ToString();

                object[] args = { contenttime, "<color=#FF8C00>" + data.specialDiscript2 + "</color>", data.contestSort, rewardcontent };
                System.DateTime sdt = GameCommon.ConvertLongToDateTime(emailtime);
                GameKind.HasFlag(5, flag))

    bool BackMailGetReward(uint _msgType, UMessage msg)
        {
            CCustomDialog.OpenCustomConfirmUI(1703);
            return false;
        }else if(state ==1)
        {
            CCustomDialog.OpenCustomConfirmUI(1702);
        }
        {
            m_MailinfoTransform.Find("ImageBG/Button_lingqu").GetComponent<Button>().interactable = true;
            m_MailinfoTransform.Find("ImageBG/Button_queding").GetComponent<Button>().interactable = true;
            m_MailinfoTransform.gameObject.SetActive(false);
        }
        //if(GameMain.hall_.contestui_ != null)
        //    GameMain.hall_.contestui_.transform.FindChild("Panelbottom/Bottom/Button_News/ImageSpot").gameObject.SetActive(EmailDataManager.GetNewsInstance().emilsdata_.Count > 0);
        GameMain.hall_.GetPlayerData().mailNumber -= 1;
        {
            Debug.Log("邮件对象资源找不到(错误编号242)!");
            return;
        }

    private void OnCloseMailPanel(EventTriggerType eventtype, object button, PointerEventData eventData)
            {
                m_MailinfoTransform.gameObject.SetActive(false);

    private void OnClickNews(EventTriggerType eventtype, object button, PointerEventData eventData)
            Transform ImageBGTransform = m_MailinfoTransform.Find("ImageBG");

    private void OnCloseMailPanelWithCloseMsg(EventTriggerType eventtype, object button, PointerEventData eventData)

            Button getbtn = m_MailinfoTransform.Find("ImageBG/Button_queding").GetComponent<Button>();
            {
                return;
            }
            getbtn.GetComponent<Button>().interactable = true;

    private void OnGetGoods(EventTriggerType eventtype, object button, PointerEventData eventData)
            {
                return;
            }