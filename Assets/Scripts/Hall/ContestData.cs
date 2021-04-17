using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//比赛状态
public enum CONTEST_STATE
{
    CONTEST_STATE_PREPARE = 0,     //比赛筹备期
    CONTEST_STATE_EXHIBITION = 1,    //比赛展示期
    CONTEST_STATE_ENROLL = 2,      //比赛报名期
    CONTEST_STATE_ADMISSION = 3,   //准许入场
    CONTEST_STATE_START = 4,       //比赛进行中
    CONTEST_STATE_PRELIMINARY = 5, //预赛
    CONTEST_STATE_FINALS = 6,      //决赛
    CONTEST_STATE_END = 7,         //比赛结束
    CONTEST_STATE_SETTLEMENT = 8,  //比赛结算计算名次
    CONTEST_STATE_REWARD = 9,      //比赛发放奖励
};

//赛事类型
public enum ContestType
{
    ContestType_None = 0,
    ContestType_Timing = 1,      //定时赛
    ContestType_AnyTime = 2,     //满足条件即开
};


//赛事分类
public enum ContestKind
{
    ContestKind_None = 0,
    ContestKind_NewBird = 1,    //新秀赛
    ContestKind_Elite = 2,      //精英赛
    ContestKind_Master = 3,     //大师赛
};


//举办方类型
public enum ContestOrganisersType
{
    ContestOrganisersType_None = 0,
    ContestOrganisersType_Official = 1, //官方
    ContestOrganisersType_Building = 2, //自建
    ContestOrganisersType_Club = 3,      //俱乐部内部赛
};

//赛事开启周期
public enum ContestOpenCycle
{
    enContestOpenCycle_None = 0,
    enContestOpenCycle_Week = 1,    //星期
    enContestOpenCycle_Month = 2,   //月
    enContestOpenCycle_Year = 3,    //年
};


public enum ContestRuleType
{
    SetScore = 0,
}

public class stConstestTime
{
    int ContestHour;
    int ContestMin;
};

public class stContestData
{
    public stContestData()
    {
        vecContestDate = new List<byte>();
        vecAdmissionHour = new List<stConstestTime>();
        vecContestHour = new List<stConstestTime>();
    }

    public uint nContestID;                         //赛事ID
    public uint nContestDataID;                    //赛事数据ID
    public string sContestName;                      //赛事名称
    public byte byGameID;                          //比赛玩的游戏ID
    public ushort nGamePlayerNum;                   //比赛游戏桌上人数
    public uint tStartTimeSeconds;                  //比赛开始时间
    public CONTEST_STATE contestState;                  //比赛状态
    public ContestType enContestType;                     //赛事类型（定时,即开）
    public ContestKind enContestKind;                     //赛事分类（新秀赛，精英赛，大师赛）
    public ContestOrganisersType enOrganisersType;          //举办方类型（官方，自建，俱乐部内部赛）
    public ContestOpenCycle enContestOpenCycle;             //开赛周期
    public List<byte> vecContestDate;      //开赛日期
    public List<stConstestTime> vecAdmissionHour;   //准允入场时间
    public List<stConstestTime> vecContestHour;     //开赛时间点
    public int nExhibitionTime;                     //展示时间(赛前多少分钟)
    public int nEnrollStartTime;                    //报名开始时间(比赛开始前多少分钟)
    public uint nMaxEnrollPlayerNum;                //报名最大人数
    public uint nMinEnrollPlayerNum;                //报名最小人数
    public uint nEnrollReputationMiniNum;           //报名信誉最低值
    public uint nEnrollMasterMiniNum;               //报名大师分最低值
    public uint nEnrollNamelistID;                  //报名名单表	
    public uint nEnrollItemID;                      //报名所需物品ID
    public uint nEnrollItemNum;                     //报名所需物品数量
    public uint nEnrollRechargeCoin;                //报名所需充值货币
    public uint nEnrollMoneyNum;                    //报名所需费用	
    public ushort shPreliminaryRuleID;              //预赛规则ID
    public ushort shFinalsRuleID;                   //决赛规则ID
    public uint nContestBeginBroadcastID;           //比赛开始广播文本ID
    public uint nChampionBroadcastID;               //冠军广播文本ID
    public uint nRewardDataID;                      //奖励ID
    public uint nContestQualificationBuyID;         //购买比赛下一轮资格ID
    public string iconname;                         //比赛图标
    public string nContestRuleDetail;               //比赛详细规则
    public string nContestRule;                     //比赛规则
    public uint bestsort;                           //历史最高排名
    public string playmode;                         //比赛玩法
    public string playmodeicon;                     //比赛玩法图标
    public string ticketIcon;                       //比赛门票图标
    public uint createid;                           //比赛创建者
    public string reward;                           //比赛奖励简述
    public string createName;                       //比赛创建者名称
    public string resetDetail;                      //重购描述
};

public class stContestRuleData
{
    public stContestRuleData()
    {
        vecPreliminaryRuleParma = new List<int>();
        vecScoreCarryParma = new List<float>();
    }

    public uint nContestRuleID;                    //比赛规则ID
    public ContestRuleType enContestRuleType;               //规则类型
    public byte byContestRoundCount;               //比赛轮数
    public uint nContestRoundTime;                 //每轮时长
    public uint nContestRoundGameNum;              //比赛每轮局数
    public List<int> vecPreliminaryRuleParma;        //规则参数
    public List<float> vecScoreCarryParma;           //积分带入参数
    public byte byDeskDivideRule;                  //比赛桌号分配方式
    public string infomation;                       //规则说明                         
};

public class stRankingRewardData
{
    public uint nRankingBegin;    //奖励起始名次
    public uint nRankingEnd;      //奖励结束名次
    public float fMasterReward;           //大师分奖励
    public uint nRechargeCoinReward;//充值币奖励
    public uint nRechargeCoinPercent;//充值币奖励比例（奖池中）
    public uint nMoneyCommonReward;//金币奖励
    public uint nChargeMoneyReward;
    public float fRedbegReward;           //微信红包现金奖励
    public uint nRewardItem1Id;   //道具1奖励ID
    public uint nRewardItem1Num;  //道具1数量
    public uint nRewardItem2Id;   //道具2奖励ID
    public uint nRewardItem2Num;  //道具2数量
    public uint nRewardItem3Id;   //道具3奖励ID
    public uint nRewardItem3Num;  //道具3数量
    public string nRewardContent; //奖励说明
};

public class CContestRewardData
{
    public CContestRewardData()
    {
        vecRankingReward = new List<stRankingRewardData>();
    }

    public stRankingRewardData GetRankingRewardByRank(uint rankVal)
    {
        for (int index = 0; index < vecRankingReward.Count; index++)
            if (rankVal >= vecRankingReward[index].nRankingBegin && rankVal <= vecRankingReward[index].nRankingEnd)
                return vecRankingReward[index];

        return null;
    }
    public uint nRewardSchemeID;      //奖励方案ID
    public List<stRankingRewardData> vecRankingReward;//具体的名次奖励	
};

public class ContestDataManager
{
    ContestDataManager()
    {
        contestData = new Dictionary<uint, stContestData>();
        ruleData_ = new Dictionary<uint, stContestRuleData>();
        rewardData_ = new Dictionary<uint, CContestRewardData>();
        contestdatas_ = new Dictionary<uint, stContestData>();
        selfcontestdatas_ = new Dictionary<uint, stContestData>();

        ReadConstDataConfig("Contest");
        LoadContestRuleData("ContestRule");
        LoadRewardSchemeData("ContestRankingReward");

        currentContestID = 0;
    }

    void ReadConstDataConfig(string name)
    {
        contestData.Clear();

        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, name, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            stContestData data = new stContestData();

            uint.TryParse(strList[i][0], out data.nContestDataID);
            data.sContestName = strList[i][1];
            byte.TryParse(strList[i][2], out data.byGameID);
            ushort.TryParse(strList[i][3], out data.nGamePlayerNum);
            byte contesttype;
            byte.TryParse(strList[i][4], out contesttype);
            data.enContestType = (ContestType)contesttype;
            byte contestkind;
            byte.TryParse(strList[i][5], out contestkind);
            data.enContestKind = (ContestKind)contestkind;
            byte organiserstype;
            byte.TryParse(strList[i][6], out organiserstype);
            data.enOrganisersType = (ContestOrganisersType)organiserstype;
            byte contestopencycle;
            byte.TryParse(strList[i][7], out contestopencycle);
            data.enContestOpenCycle = (ContestOpenCycle)contestopencycle;

            string[] strverc = strList[i][8].Split('|');
            for (int index = 0; index < strverc.Length; index++)
            {
                byte temp;
                byte.TryParse(strverc[index], out temp);
                data.vecContestDate.Add(temp);
            }
            //string[] Admissionverc = strList[i][9].Split('|');
            //for (int index = 0; i < Admissionverc.Length; index++)
            //{
            //    stConstestTime paddata;
            //    sscanf(Admissionverc[i].c_str(), "%d:%d", &paddata.ContestHour, &paddata.ContestMin);
            //    pData->vecAdmissionHour.push_back(paddata);
            //}

            //std::vector < std::string> startcontestverc = StdAndFile::Split(tab[i][10], "|");
            //for (int i = 0; i < startcontestverc.size(); i++)
            //{
            //    stConstestTime phodata;
            //    sscanf(startcontestverc[i].c_str(), "%d:%d", &phodata.ContestHour, &phodata.ContestMin);
            //    pData->vecContestHour.push_back(phodata);
            //}

            int.TryParse(strList[i][10], out data.nExhibitionTime);
            int.TryParse(strList[i][11], out data.nEnrollStartTime);
            uint.TryParse(strList[i][13], out data.nMaxEnrollPlayerNum);
            uint.TryParse(strList[i][14], out data.nMinEnrollPlayerNum);
            uint.TryParse(strList[i][15], out data.nEnrollReputationMiniNum);
            uint.TryParse(strList[i][16], out data.nEnrollMasterMiniNum);
            uint.TryParse(strList[i][17], out data.nEnrollNamelistID);
            uint.TryParse(strList[i][18], out data.nEnrollItemID);
            uint.TryParse(strList[i][19], out data.nEnrollItemNum);
            uint.TryParse(strList[i][20], out data.nEnrollRechargeCoin);
            //uint.TryParse(strList[i][21], out data.nEnrollMoneyNum);

            ushort.TryParse(strList[i][21], out data.shPreliminaryRuleID);
            ushort.TryParse(strList[i][22], out data.shFinalsRuleID);

            uint.TryParse(strList[i][23], out data.nContestBeginBroadcastID);
            uint.TryParse(strList[i][24], out data.nChampionBroadcastID);
            uint.TryParse(strList[i][25], out data.nRewardDataID);
            //uint.TryParse(strList[i][26], out data.nContestQualificationBuyID);            data.iconname = strList[i][26];            data.nContestRule = strList[i][27];            data.nContestRuleDetail = strList[i][28];            data.playmode = strList[i][29];            data.playmodeicon = strList[i][30];            data.resetDetail = strList[i][31];            data.ticketIcon = strList[i][32];            data.reward = strList[i][36];            contestData.Add(data.nContestDataID, data);        }
    }

    void LoadContestRuleData(string name /* = "ContestRuleData.csv" */)
    {

        ruleData_.Clear();

        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, name, out strList);        int columnCount = strList.Count;

        for (int i = 2; i < strList.Count; i++)
        {
            stContestRuleData data = new stContestRuleData();
            uint.TryParse(strList[i][0], out data.nContestRuleID);
            byte contestruletype;
            byte.TryParse(strList[i][1], out contestruletype);
            data.enContestRuleType = (ContestRuleType)contestruletype;
            byte.TryParse(strList[i][2], out data.byContestRoundCount);
            uint.TryParse(strList[i][3], out data.nContestRoundTime);
            uint.TryParse(strList[i][4], out data.nContestRoundGameNum);

            string[] strverc = strList[i][5].Split('|');
            for (int index = 0; index < strverc.Length; index++)
            {
                int temp;
                int.TryParse(strverc[index], out temp);
                data.vecPreliminaryRuleParma.Add(temp);
            }

            string[] scoreverc = strList[i][7].Split('|');
            for (int index = 0; index < scoreverc.Length; index++)
            {
                float temp;
                float.TryParse(scoreverc[index], out temp);
                data.vecScoreCarryParma.Add(temp);
            }

            byte.TryParse(strList[i][8], out data.byDeskDivideRule);

            data.infomation = strList[i][9];

            ruleData_.Add(data.nContestRuleID, data);
        }
    }

    void LoadRewardSchemeData(string name /* = "RewardSchemeData.csv" */)
    {
        rewardData_.Clear();
        List<string[]> strList;
        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, name, out strList);
        int columnCount = strList.Count;

        for (int i = 2; i < strList.Count; i++)
        {
            uint RewardDataId;
            uint.TryParse(strList[i][0], out RewardDataId);
            CContestRewardData pReward = GetRewardDataById(RewardDataId);
            stRankingRewardData pRinkData = new stRankingRewardData();
            uint.TryParse(strList[i][1], out pRinkData.nRankingBegin);
            uint.TryParse(strList[i][2], out pRinkData.nRankingEnd);
            float.TryParse(strList[i][3], out pRinkData.fMasterReward);
            uint.TryParse(strList[i][4], out pRinkData.nRechargeCoinReward);
            uint.TryParse(strList[i][5], out pRinkData.nRechargeCoinPercent);
            uint.TryParse(strList[i][6], out pRinkData.nMoneyCommonReward);
            uint.TryParse(strList[i][7], out pRinkData.nChargeMoneyReward);
            float.TryParse(strList[i][8], out pRinkData.fRedbegReward);
            uint.TryParse(strList[i][9], out pRinkData.nRewardItem1Id);
            uint.TryParse(strList[i][10], out pRinkData.nRewardItem1Num);
            uint.TryParse(strList[i][11], out pRinkData.nRewardItem2Id);
            uint.TryParse(strList[i][12], out pRinkData.nRewardItem2Num);
            uint.TryParse(strList[i][13], out pRinkData.nRewardItem3Id);
            uint.TryParse(strList[i][14], out pRinkData.nRewardItem3Num);
            pRinkData.nRewardContent = strList[i][15];

            if (pReward == null)
            {
                pReward = new CContestRewardData();
                pReward.nRewardSchemeID = RewardDataId;
                rewardData_.Add(RewardDataId, pReward);
            }
            pReward.vecRankingReward.Add(pRinkData);
        }
    }

    CContestRewardData GetRewardDataById(uint RewardId)
    {
        if (rewardData_.ContainsKey(RewardId))
            if (rewardData_[RewardId] != null)
                return rewardData_[RewardId];

        return null;
    }

    public static ContestDataManager Instance()
    {
        if (instance_ == null)
            instance_ = new ContestDataManager();

        return instance_;
    }

    public stContestData GetCurrentContestData()
    {
        if (contestdatas_.ContainsKey(currentContestID))
             return contestdatas_[currentContestID];

        if(selfcontestdatas_.ContainsKey(currentContestID))
            return selfcontestdatas_[currentContestID];

        return null;
    }

    public stContestData GetContestDataByDataId(uint dataId)
    {
        if (!contestData.ContainsKey(dataId))
            return null;

        return contestData[dataId];
    }


    public stRankingRewardData GetCurrentRewardData(uint rankValue, uint contestid = 0)
    {
        if(contestid == 0)
            contestid = currentContestID;

        uint rewardkey = 0; 
        if (contestdatas_.ContainsKey(contestid))
        {
            rewardkey = contestdatas_[contestid].nRewardDataID;
        }

        if (selfcontestdatas_.ContainsKey(contestid))
        {
            rewardkey = selfcontestdatas_[contestid].nRewardDataID;
        }

        if (rewardData_.ContainsKey(rewardkey))
            return rewardData_[rewardkey].GetRankingRewardByRank(rankValue);

        Debug.Log("rewardkey : " + rewardkey.ToString() + " rankvalue:" + rankValue.ToString() + " error contestid:" + contestid);

        return null;
    }

    public stRankingRewardData GetRewardDataByDataId(uint dataId, uint rankValue)
    {
        if (contestData.ContainsKey(dataId))
            return null;

        uint rewardkey = contestData[dataId].nRewardDataID;
        if (!rewardData_.ContainsKey(rewardkey))
        {
            Debug.Log("rewardkey : " + rewardkey.ToString() + "rankvalue:" + rankValue.ToString());

            return null;
        }

        return rewardData_[rewardkey].GetRankingRewardByRank(rankValue);
    }

    public Dictionary<uint, stContestData> contestData;
    public Dictionary<uint, stContestRuleData> ruleData_;
    public Dictionary<uint, CContestRewardData> rewardData_;
    static ContestDataManager instance_;
    public uint currentContestID;                   //进入场景后，当前比赛ID

    public Dictionary<uint, stContestData> contestdatas_;
    public Dictionary<uint, stContestData> selfcontestdatas_;
}
