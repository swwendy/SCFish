using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USocket.Messages;

//邮件奖励的来源
public enum MailFrom_Enum
{
    MailFrom_Init,
    MailFrom_Contest,   //比赛奖励
    MailFrom_MomentsKick,   //朋友圈踢人
    MailFrom_ContestCutCreditScore,//消极比赛扣除信誉分

    MailFrom_end
};

public enum MailType
{
    READONLY = 1,
    HASGOODS,
}

public class EmailData
{
    public EmailData()
    {
        emailitems = new List<Item>();
    }

    public uint emailid;
    public uint sendid;
    public string sendName;
    public MailFrom_Enum rewardSorce; //奖励来源 1 比赛 2 其他
    public uint sorceID;
    public ushort contestSort;
    public uint contentID; //描述id
    public string specialDiscript1;
    public string specialDiscript2;
    public byte gamekind;
    public float masterReward; //奖励大师分
    public uint diamondReward; //奖励钻石
    public uint coinReward;         //奖励钱
    public string emailicon;
    public byte hasread;
    public string emailname;
    public string emailtime;
    public MailType emailtype;
    public List<Item> emailitems;
    public string infomation;
    public uint titleid;
    public float redbag;
}

public class EmailDataManager
{
    public static EmailDataManager GetNewsInstance()
    {
        if (intance_ == null)
            intance_ = new EmailDataManager();

        return intance_;
    }

    EmailDataManager()
    {
        emilsdata_ = new Dictionary<uint, EmailData>();
        InitMailMsg();
    }

    void InitMailMsg()
    {
        CMsgDispatcher.GetInstance().RegMsgDictionary(
            (uint)GameCity.EMSG_ENUM.CrazyCityMsg_UPDATEPLAYERGETMAIL, BackMailUpdateData);                     //邮件数据
    }

    void AddEmail( ref UMessage msg )
    {

    }

    bool BackMailUpdateData(uint _msgType, UMessage msg)
    {
        EmailData data = new EmailData();

        data.emailid = msg.ReadUInt();
        data.hasread = msg.ReadByte();
        data.sendid = msg.ReadUInt();
        data.sendName = msg.ReadString();
        uint emailtime = msg.ReadUInt();
        data.rewardSorce = (MailFrom_Enum)msg.ReadByte();
        data.sorceID = msg.ReadUInt();
        data.contestSort = msg.ReaduShort();
        data.titleid = msg.ReadUInt();
        data.contentID = msg.ReadUInt();
        data.specialDiscript1 = msg.ReadString();
        data.specialDiscript2 = msg.ReadString();
        data.gamekind = msg.ReadByte();
        data.emailname = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(data.titleid).TipsText;
        TipsData tdata = CCsvDataManager.Instance.TipsDataMgr.GetTipsData(data.contentID);

        byte flag = msg.ReadByte();
        if (GameKind.HasFlag(0, flag))
            data.masterReward = msg.ReadSingle();
        if (GameKind.HasFlag(1, flag))
            data.diamondReward = msg.ReadUInt();
        if (GameKind.HasFlag(2, flag))
            data.coinReward = msg.ReadUInt();

        for (int itemindex = 3; itemindex < 6; itemindex++)
        {
            if (GameKind.HasFlag(itemindex, flag))
            {
                Item item = new Item();
                item.itemid = msg.ReaduShort();
                item.itemNumber = msg.ReadByte();
                data.emailitems.Add(item);
            }
        }

        if (GameKind.HasFlag(6, flag))
            data.redbag = msg.ReadSingle();

        if (data.rewardSorce == MailFrom_Enum.MailFrom_Contest)
        {
            int starttime = 0;
            int.TryParse(data.specialDiscript1, out starttime);

            System.DateTime stsdt = GameCommon.ConvertLongToDateTime(starttime);
            string contenttime = stsdt.ToString("yyyy年MM月dd日HH:mm");

            System.DateTime sdt = GameCommon.ConvertLongToDateTime(emailtime);
            data.emailtime = sdt.ToString("yyyy年MM月dd日HH:mm");

            string rewardcontent = "";
            if (GameKind.HasFlag(0, flag))
                rewardcontent += "大师分:" + data.masterReward.ToString();
            if (GameKind.HasFlag(6, flag))
                rewardcontent += " 现金红包:" + data.redbag.ToString() + "元";
            if (GameKind.HasFlag(1, flag) || GameKind.HasFlag(2, flag))
                rewardcontent += " 钻石:" + (data.diamondReward + data.coinReward).ToString();

            object[] args = { contenttime, "<color=#FF8C00>" + data.specialDiscript2 + "</color>", data.contestSort, rewardcontent };

            string formatcontent = string.Format(tdata.TipsText, args);
            data.infomation = formatcontent;
        }
        else if (data.rewardSorce == MailFrom_Enum.MailFrom_MomentsKick)
        {
            object[] args = { data.specialDiscript1 };

            string formatcontent = string.Format(tdata.TipsText, args);
            data.infomation = formatcontent;
        }
        else if (data.rewardSorce == MailFrom_Enum.MailFrom_ContestCutCreditScore)
        {
            int starttime = 0;
            int.TryParse(data.specialDiscript1, out starttime);

            System.DateTime stsdt = GameCommon.ConvertLongToDateTime(starttime);
            string contenttime = stsdt.ToString("yyyy年MM月dd日HH:mm");

            System.DateTime sdt = GameCommon.ConvertLongToDateTime(emailtime);
            data.emailtime = sdt.ToString("yyyy年MM月dd日HH:mm");

            object[] args = { contenttime, data.specialDiscript2 };

            string formatcontent = string.Format(tdata.TipsText, args);
            data.infomation = formatcontent;
        }

        //if (data.emailitems.Count == 0)
        if (GameKind.HasFlag(3, flag) || GameKind.HasFlag(4, flag) ||
            GameKind.HasFlag(5, flag))
            data.emailtype = MailType.HASGOODS;
        else
            data.emailtype = MailType.READONLY;

        emilsdata_.Add(data.emailid, data);

        Email.GetEmailInstance().InitNewsUIData();

        GameMain.hall_.m_Bulletin.OnEmailChange(EmailDataManager.GetNewsInstance().emilsdata_.Count <= 0);
        GameMain.hall_.GetPlayerData().mailNumber = (byte)EmailDataManager.GetNewsInstance().emilsdata_.Count;

        return true;
    }

    static EmailDataManager intance_;
    public Dictionary<uint, EmailData> emilsdata_;
}
