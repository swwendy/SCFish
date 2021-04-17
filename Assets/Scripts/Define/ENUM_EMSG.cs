using XLua;
/// <summary>
/// 游戏类型枚举
/// </summary>
[LuaCallCSharp]
public enum GameKind_Enum{    GameKind_None = -1,    GameKind_CarPort,           //车行
    GameKind_DiuDiuLe,          //丢丢乐						
    GameKind_LaBa,              //拉霸
    GameKind_ForestDance,       //森林舞会
    GameKind_BlackJack,         //21点
    GameKind_FiveInRow,         //五子棋
    GameKind_TexasPoker,        //德州扑克
    GameKind_LandLords,         //斗地主
    GameKind_BullHundred,       //百人牛牛  两堆
    GameKind_BullAllKill,		//通杀牛牛  5堆
    GameKind_Fishing,           //捕鱼
    GameKind_BullHappy,         //抢庄牛牛
    GameKind_Mahjong,           //麻将
    GameKind_GuanDan,			//掼蛋
    GameKind_YcMahjong,         //盐城麻将
    GameKind_CzMahjong,			//常州麻将
    GameKind_LuckyTurntable,    //幸运盘
    GameKind_GouJi,             //够级
    GameKind_HongZhong,         //红中麻将
    GameKind_Answer,            //答题赛
    GameKind_Chess,             //象棋
    GameKind_Max}

[LuaCallCSharp]
public enum GameTye_Enum
{
    GameType_Normal,
    GameType_Contest,
    GameType_Appointment,
    GameType_Record,
}

//进roomser 的原因
enum GameReason_enum
{
    GameReason_Match = 0,       //匹配进的
    GameReason_Contest,     //比赛进的
    GameReason_Appoint,		//约局进的
};

public enum GameState_Enum
{
    GameState_Luancher = 0,  //资源检测更新状态
    GameState_Login,         //登陆状态
    GameState_Hall,          //大厅状态
    GameState_Game,          //游戏状态
    GameState_Contest,       //比赛状态
    GameState_Appointment,   //约局状态
}


//------------------------------------------------------------------------------
//消息类型定义，与服务器消息类型需保持一致
//------------------------------------------------------------------------------
namespace GameCity
{
    public enum EMSG_ENUM
    {
        CrazyCityMsg_BEGIN = 0,
        CrazyCityMsg_ROOMSERLOGIN,                      //游戏服务器登陆
        CrazyCityMsg_PLAYERLOGIN,                       //玩家登陆
        CrazyCityMsg_BACKPLAYERLOGIN,                   // 服务器回复玩家登陆
        CrazyCityMsg_PLAYERDATA,                        //平台检测通过后去游戏数据库
        CrazyCityMsg_PLAYERLOGINSUCCESS,                //玩家登陆成功
        CrazyCityMsg_CHANGENICKNAME,                    //玩家修改昵称
        CrazyCityMsg_BACKCHANGENAME,                    //回复修改昵称
        CrazyCityMsg_BACKROOMSER,                       //loginser 回复 roomser
        CrazyCityMsg_PLAYERAPPLYGAME,                   //玩家申请进入某个游戏
        CrazyCityMsg_BACKAPPLYGAME,                     //回复申请进入某游戏
        CrazyCityMsg_GETROLEDATA,                       // g->L 去login上请求玩家数据
        CrazyCityMsg_BACKROLEDATA,                      // 回复数据
        CrazyCityMsg_RESULTTOLOGIN,                     // G->L 一局游戏结束后保存玩家数据
        CrazyCityMsg_RESULTTODB,                        // L->D 一局游戏结束后保存玩家数据	
        CrazyCityMsg_SAVEROLEDATATODB,                  // 保存玩家数据到数据库
        CrazyCityMsg_LETROOMSERDOWNPLAYER,              //让房间服务器把某玩家断了
        CrazyCityMsg_SERVERCUTCONNECT,                  //服务器断开与你的连接（顶号）
        CrazyCityMsg_ROLELEAVEROOMSER,                  //玩家断开与房间服的连接
        CrazyCityMsg_RECHARGE,                          //充值
        CrazyCityMsg_BACKRECHARGE,                      //回复充值
        CrazyCityMsg_GETCLUBDATAFROMDB,                 //L->D 获取db的club 金钱排行榜 彩金池数据
        CrazyCityMsg_BACKCLUBDATATOLOGIN,               //D->L 回复club数据到客户端


        CrazyCityMsg_PLAYER_REQESTBUY,                  //玩家请求购买
        CrazyCityMsg_PLAYER_BUYVERIFY,                  //玩家购买完成请求服务器验证
        CrazyCityMsg_PLAYER_BUYRESULT,                  //玩家购买结果
        CrazyCityMsg_PLAYER_UPDATEMONEY,                //更新玩家的货币数值到客户端
        CrazyCityMsg_PAYSERVERLOGIN,                    //支付服务器登陆login
        CrazyCityMsg_SENDRECHARGETOPAY,                 //L->P login发送充值信息到pay验证
        CrazyCityMsg_BACKRECHARGEPAY,                   //P->L payserver验证完成后回复消息给login
        CrazyCityMsg_TRADEAPPPAYINFO,                   //P->L L->C 支付订单信息
        CrazyCityMsg_PLAYERVIPLEVELUP,                  //玩家vip等级升级了
        CrazyCityMsg_LOGINLOGSERVER,                    //其他服务器登陆log服务器
        CrazyCityMsg_GETROOMINFO,                       //L->R login获取各个房间服务器里的情况
        CrazyCityMsg_BACKROOMINFO,                      //回复
        CrazyCityMsg_GMTOOLACOUNTLOGIN,                 //GM工具账号登陆
        CrazyCityMsg_BACKCHECKGMACCOUNT,                //回复GM账号登陆
        CrazyCityMsg_GMTOOLADDITEMALL,                  //GM工具添加道具
        CrazyCityMsg_BACKGMTOOLADDITEM,                 //回复GM工具添加道具
        CrazyCityMsg_MOBILELOGIN,                       //手机号码登陆
        CrazyCityMsg_CHECKINDENTIFYING,                 //客户端验证码
        CrazyCityMsg_BACKCHECKINDENTIFYING,             //回复
        CrazyCityMsg_PLAYERBINGUSEID,                   //L->D 玩家绑定useid
        CrazyCityMsg_BACKMOBILELOGIN,                   //服务器回复玩家 获取验证码
        CrazyCityMsg_CHECKMOBILEISBIND,                 //L->D 去数据库中检测该手机号是否已经绑定
        CrazyCityMsg_BACKCHECKMOBILEISBIND,             //D->L 回复login该手机号是否绑定
        CrazyCityMsg_UPDATECOINRANKDATATOLOGIN,         //D->L 发送金钱排名到login
        CrazyCityMsg_UPDATECOINRANKDATATOCLINT,         //L->C 发送金钱排名到clint
        CrazyCityMsg_NEEDUPDATECOINRANK,                //需要更新金钱排行榜
        CrazyCityMsg_PLAYERGETCOINRANK,                 //玩家获取金钱排行榜
        CrazyCityMsg_CHANGERELIEF,                      //修改救济金次数
        CrazyCityMsg_BACKCHANGERELIEF,                  //通知客户端
        CrazyCityMsg_CHANGERELIEFTOROOMSER,             //去房间服务器修改玩家增加的救济金
        CrazyCityMsg_BEFOREGAMEOVER,                    //玩家上线后之前的游戏结束了
        CrazyCityMsg_SAVEJACKPOTHANDSELTODB,            //保存彩金奖池到数据库
        CrazyCityMsg_SENDJACKPOTHANDSELTOLOGIN,         //发送彩金奖池数据到login
        CrazyCityMsg_RUNHORSELIGHTDATA,                 //G->L L->C 走马灯数据到login
        CrazyCityMsg_SENDGOODSTATE,                     //L->P login处理完充值信息  是否发货成功
        CrazyCityMsg_NOTSENDGOODDATATOLOGIN,            //P->L payserver send data to login
        CrazyCityMsg_LETPAYSENDGOODDATA,                //L->P 玩家上线了 让pay告诉 loginserver 发货
        CrazyCityMsg_GMCLOSETALLSERVER,                 //GM服务器关闭所有服务器
        CrazyCityMsg_GETROLECLUBGIVEDATATOLOGIN,        //L->D 请求玩家工会赠送数据
        CrazyCityMsg_SENDROLECLUBGIVEDATATOLOGIN,       //D->L 发送玩家工会赠送数据到login
        CrazyCityMsg_SAVEROLECLUBGIVEDATATODB,          //L_>D 保存工会赠送数据到db
        CrazyCityMsg_GETROLECLUBGIVEDATA,               //C->L 客户端请求工会赠送数据
        CrazyCityMsg_SENDROLECLUBGIVEDATATOCLINT,       //L->C 服务器发送工会赠送数据到clint
        CrazyCityMsg_PLAYERREMOVECLUBGIVEDATA,          //C->L 玩家移除工会赠送数据
        CrazyCityMsg_CHANGECLUBACCEPTCOINTODB,          //L->D 通知数据库重置工会接受数据
        CCMsg_FIVEINROW_CM_INVATECLUBMEMBER,
        CCMsg_FIVEINROW_SM_BACKINVATECLUBMEMBER,        //邀请工会成员进入游戏
        CCMsg_FIVEINROW_CM_ISAGREEINVATE,
        CCMsg_FIVEINROW_SM_BACKISAGREEINVATE,       //是否同意邀请
        CrazyCityMsg_CM_PLAYERAPPLYBINDINVITE,              //玩家申请绑定邀请码
        CrazyCityMsg_SM_PLAYERAPPLYBINDINVITE,              //回复
        CrazyCityMsg_PLAYERBINDINVITETODB,              //去数据库保存
        CrazyCityMsg_CM_ROBOTAISERVERLOGIN,             //机器人服务器登陆
        CrazyCityMsg_SM_ROBOTAISERVERLOGIN,             //回复
        CrazyCityMsg_NEEDAIROBOTTOLOGIN,                //G->L 通知login 需要加 ai 机器人
        CrazyCityMsg_SM_ADDAIROBOTTOGAME,               //L->C login 发送消息到 robotser 添加机器人
        CrazyCityMsg_CM_BACKADDAIROBOTTOGAME,           //C->L 回复
        CrazyCityMsg_LETLOGINCUTAIROBOT,                //G->L 通知login 关闭某个 ai
        CrazyCityMsg_STOPPLAYERGAME,                    //玩家钱币的输赢已经达到上限了
        CrazyCityMsg_RESERTEVERYDAYDATE,                //充值每天的的一些数据
        CrazyCityMsg_UPDATERANDOMGAMEROLENUM,			//同步随机游戏上的人数给客户端
        CrazyCityMsg_SAVECLUBREBATETODB,                //会长不在线的时候会员充值返利给db
        CrazyCityMsg_GETWEEKOROLDREWORD,                //获取周奖励 或者老玩家
        CrazyCityMsg_BACKWEEKOROLDREWORD,               //回复给玩家
        CrazyCityMsg_CHECKINVITETODB,                   //L->D login去db上检验邀请码
        CrazyCityMsg_BACKCHECKINVITETOLOGIN,            //L->D 回复
        CrazyCityMsg_FIRSTADDCLUBREWORD,                //第一次加入俱乐部的奖励
        CrazyCityMsg_CM_SENDHORNTOALL,                  //客户端发送喇叭数据到服务器
        CrazyCityMsg_SM_SENDHORNTOALL,                  //发给所有人
        CrazyCityMsg_BACK_SENDHORNTOALL,                //回复给申请的客户端
        CrazyCityMsg_SENDADDLOTTERYTOCLINT,				//发送增加奖券数量的消息给客户端
        CrazyCityMsg_PLAYERGETTRADEINFO,                //申请获取奖券兑换信息
        CrazyCityMsg_BACKGETTRADEINFO,				    //回复奖券兑换记录
        CrazyCityMsg_REDBAGBEGIN,                       //活动开始
        CrazyCityMsg_REDBAGEND,                         //活动结束
        CrazyCityMsg_PLAYERGETREDBAG,                   //玩家申请获取红包
        CrazyCityMsg_BACKPLAYERREDBAG,					//回复
        CrazyCityMsg_PALYERGETACTIVITYINFO,             //玩家申请获得活动列表
        CrazyCityMsg_BACKGETACTIVITYINFO,				//huifu 
        CrazyCityMsg_GETTODAYREDBAGINFO,                //玩家获取当前红包数据
        CrazyCityMsg_BACKTODAYREDBAGINFO,				//回复
        CrazyCityMsg_PLAYERLOGINFAILED,					//玩家登陆失败 因为
        CrazyCityMsg_SETLETTRORYCOVERT,                 //设置实物兑换开关
        CrazyCityMsg_GETROLEFISHCANNONINFO,             //L->D获取捕鱼炮台数据
        CrazyCityMsg_BACKROLEFISHCANNONINFO,            //D-L返回
        CrazyCityMsg_AFTERPLAYERBUGCANNON,              //玩家购买完炮台后保存数据库
        CrazyCityMsg_AFTERPLAYERUSEFISHSKILL,           //玩家使用完技能之后保存
        CrazyCityMsg_REWORDFISHINGSKILL,                //奖励玩家捕鱼的技能或者使用次数
        CrazyCityMsg_REQUESTPLAYERPACKETINFO,           //申请玩家的背包数据
        CrazyCityMsg_SENDPLAYERPACKETINFO,              //发送玩家背包数据
        CrazyCityMsg_SAVEPLAYERPACKETINFO,              //保存玩家背包数据
        CrazyCityMsg_UPDATEPLAYERPACKETINFO,			//玩家背包数据更新的奥客户端
        CrazyCityMsg_APPLAYGETMAILREWORDDATA,           //玩家申请获取邮件
        CrazyCityMsg_BACKGETMAILREWORDDATA,             //通知玩家领取邮件的物品
        CrazyCityMsg_REQUESTPLAYERMAILINFO,             //申请玩家的邮件数据
        CrazyCityMsg_SENDPLAYERMAILINFO,                //发送玩家邮件数据
        CrazyCityMsg_SAVEPLAYERMAILINFO,                //保存玩家邮件数据
        CrazyCityMsg_UPDATEPLAYERGETMAIL,					//通知玩家收到一封邮件
        CrazyCityMsg_SAVEAPPOINTLOG,                //保存约局log到数据库
        CrazyCityMsg_SENDPLAYERAPPOINT,             //发送玩家约局记录
        CrazyCityMsg_GETROLELOGDATA,                //获取玩家数据型数据
        CrazyCityMsg_CM_PLAYERLEAVEROOMSER,         //玩家申请离开房间服务器
        CrazyCityMsg_SM_PLAYERLEAVEROOMSER,			//回复
        CrazyCityMsg_SM_PLAYERDISORRECONNECT,       //玩家掉线或者重新上线
        CrazyCityMsg_SM_MIDDLEENTERCONTEST,         //中途加入比赛
        CrazyCityMsg_PLAYER_QUERYSTATE,             //玩家请求查询当前所处的状态
        CrazyCityMsg_PLAYER_QUERYSTATE_REPLY,       //回复玩家请求查询当前所处的状态
        CrazyCityMsg_APPLYVIDEOSERIPPORT,           //申请获取videoser ip port
        CrazyCityMsg_BACKVIDEOSERIPPORT,			//回复
        CrazyCityMsg_APPLYTOBEONLOOKER,             //玩家申请围观
        CrazyCityMsg_BACKTOBEONLOOKER,              //回复
        CrazyCityMsg_GAMEINFOTOONLOOKERS,			//游戏中的情况发送给围观者
        CrazyCityMsg_CM_APPLYENTERROOMANDSIT,//玩家申请进入某房间某座位
        CrazyCityMsg_SM_APPLYENTERROOMANDSIT,//回复
        CrazyCityMsg_SM_UPDATEENTERROOMANDSIT,//同步给房间里的其他人
        CrazyCityMsg_SM_UPDATEENTERROOMANDSITTOREADYALL,//同步给准备大厅里的其他玩家
        CrazyCityMsg_CM_APPLYLEAVEROOMANDSIT,//玩家申请离开某房间某座位
        CrazyCityMsg_SM_APPLYLEAVEROOMANDSIT,//回复
        CrazyCityMsg_SM_UPDATELEAVEROOMANDSIT,//同步给房间里的其他人
        CrazyCityMsg_SM_UPDATELEAVEROOMANDSITTOREADYALL,//同步给准备大厅里的其他玩家
        CrazyCityMsg_SM_UPDATEBEFOREHANDROOMINFO,   //同步准备大厅房间的信息给
        CrazyCityMsg_CM_APPLYREADY,         //玩家申请准备
        CrazyCityMsg_SM_APPLYREADY,         //回复
        CrazyCityMsg_SM_UPDATEAPPLYREADY,       //同步出去
        CrazyCityMsg_CM_APPLYCANCLEREADY,   //玩家申请取消准备
        CrazyCityMsg_SM_APPLYCANCLEREADY,   //回复
        CrazyCityMsg_SM_UPDATEAPPLYCANCLEREADY,//同步出去
        CrazyCityMsg_CM_UPDATEBEFOREHANDROOMINFO,//玩家请求准备大厅房间数据
        CrazyCityMsg_CM_APPLYCHANGESIT,     //申请换座位
        CrazyCityMsg_SM_ASKTOCHANGESIT,     //询问对方是否同意换座位
        CrazyCityMsg_CM_ANSWERTOCHANGESIT,  //回复
        CrazyCityMsg_SM_APPLYCHANGESIT,     //别人是否同意换座位
        CrazyCityMsg_SM_UPDATECHANGESIT,    //换坐成功后
        CrazyCityMsg_CM_APPLYQUITSTARTGAME, //快速进入房间
        CrazyCityMsg_SM_BACKQUITSTARTGAME,	//回复
        CrazyCityMsg_SM_KICKOUTROOM,		//因为一些原因T出游戏房间
        CrazyCityMsg_SM_LOGINSERDISCONNECT,	//loginser未开启请稍后
        CrazyCityMsg_GETROLEOTHERDATE,      //获取玩家其他数据
        CrazyCityMsg_SENDGAMEMASTERSCORE,   //发送所有游戏的大师分
        CrazyCityMsg_SAVEGAMEMASTERSCORE,   //保存所有游戏的大师分
        CrazyCityMsg_UPDATERECHARGETOROOMSER,//充值成功后同步到房间服务器与房间里的其他人
        CrazyCityMsg_BACKMOMENTDATATOLOGIN,             //D->L 回复moment数据到客户端
        CrazyCityMsg_APPLYANNOUNCEMENTDATA,     //玩家申请公告
        CrazyCityMsg_BACKANNOUNCEMENTDATA,      //回复
        CrazyCityMsg_ANNOUNCEMENTNEEDUPDATE,	//服务器通知所有在线玩家需要更新公告了
        CrazyCityMsg_APPBACKSTATENOTIFYGAMESERVER,//APP切换至后台状态值通知游戏服务器(及时更新玩家离线状态)
        CrazyCityMsg_ADDROLECREDITSCOREEVERYDAY,    //L->D 每天定时增加玩家信誉分
        CrazyCityMsg_BACKADDROLECREDITSCOREEVERYDAY,//D->L db更新完之后 通知loginser
        CrazyCityMsg_SAVEPLAYERGAMESTATISTICSDATA,  //保存玩家游戏统计数据到数据库
        CrazyCityMsg_SENDPLAYERGAMESTATISTICSDATA,  //发送玩家游戏统计数据到
        CrazyCityMsg_APPLYPLAYERGAMESTATISTICSDATA, //玩家申请游戏统计数据
        CrazyCityMsg_UPDATEMASTERRANKTOLOGIN,       //D->L 发送大师分排名到login
        CrazyCityMsg_TELLMASTERRANKNEEDUPDATE,      //通知客户端大师分排行榜需要更新
        CrazyCityMsg_APPLYMASTERRANKTOLOGIN,		//玩家请求大师分排行榜
        CrazyCityMsg_SENDCONTESTCHAMPIONINFO,       //发送比赛冠军详情到客户端
        CrazyCityMsg_TELLCONTESTCHAMPIONNEEDUPDATE, //通知客户端比赛冠军列表需要更新
        CrazyCityMsg_APPLYCONTESTCHAMPIONINFO,      //玩家请求比赛冠军列表
        CrazyCityMsg_GETPLAYERPROXYDATATOGLOGIN,    //loginser获取玩家代理数据
        CrazyCityMsg_SENDPLAYERPROXYDATATOGLOGIN,   //玩家代理数据到loginser
        CrazyCityMsg_ADDNEWPLAYERPROXYDATA,         //客户端申请添加新的代理
        CrazyCityMsg_BACKADDNEWPLAYERPROXYDATA,     // 回复
        CrazyCityMsg_APPLYIMPROVEDINDENTITY,        //申请提升某人代理身份
        CrazyCityMsg_BACKIMPROVEDINDENTITY,         //回复
        CrazyCityMsg_ADDRECHARGEREBATE,             //增加玩家充值返利总额
        CrazyCityMsg_ADDWINREBATE,                  //增加玩家赢钱抽水总额
        CrazyCityMsg_GAMESERADDPLAYERBOTTOM,        //玩家在游戏中产生抽水
        CrazyCityMsg_EMOTION,						//发表情
        CrazyCityMsg_CashToDiamond,                 //红包兑换钻石
        CrazyCityMsg_SendContestData,               //发送玩家创建比赛数据
        CrazyCityMsg_UpdateContestData,
        CrazyCityMsg_ReqContestRankData,
        CrazyCityMsg_SendContestRankData,

        //微信公众号现金红包消息
        CrazyCityMsg_PL_QUERYWXCASHREDBAGAMOUNT = 491,  //支付平台服务器向login查询公众号用户微信现金红包金额
        CrazyCityMsg_LP_REPLYWXCASHREDBAGAMOUNT,        //login回复支付平台服务器公众号用户微信现金红包金额
        CrazyCityMsg_PL_RECEVIEDWXCASHREDBAGSUCESS,     //支付平台服务器通知login玩家领取公众号现金红包成功
        CrazyCityMsg_BackPlayerWxQRAuthCode,            //回复玩家微信扫描二维码登陆授权后code

        //比赛系统消息
        ContestMsg_Begin = 500,                           //比赛系统起始
        ContestMsg_ContestSvrRegitserToLogin,             //比赛服注册到login
        ContestMsg_GameServerRegitserToContest,           //游戏服注册到contest 服
        ContestMsg_ContestInfoList,                       //发送所有的比赛信息到login
        ContestMsg_RequestContestInfo,                    //客户端请求比赛信息
        ContestMsg_ContestInfoChange,                     //某个比赛信息或状态变后同步到login
        ContestMsg_PlayerEnroll,                          //玩家报名比赛
        ContestMsg_PlayerEnrollReply,                     //玩家报名比赛结果   
        ContestMsg_PlayerCancelEnroll,                    //玩家取消报名
        ContestMsg_PlayerCancelEnrollReply,               //玩家取消报名结果
        ContestMsg_NotifiyPlayerAdmission,                //通知玩家入场
        ContestMsg_PlayerRequestAdmission,                //玩家请求入场
        ContestMsg_PlayerAdmissionReply,                  //玩家入场回复
        ContestMsg_PlayerExitContest,                     //玩家退出比赛
        ContestMsg_PlayerExitContestReply,                //玩家退出比赛回复
        ContestMsg_NotifyStartContest,                    //通知login比赛开始
        ContestMsg_NotifyPlayerStartContest,              //通知玩家比赛开始
        ContestMsg_RoundEndDeskRoleRank,                  //比赛一轮结束桌上的玩家当前排名情况(发送给gamesvr)
        ContestMsg_ContestEnd,                            //比赛结束
        ContestMsg_ContestDeskRoleScore,                  //每轮游戏结束后玩家积分上报（桌为单位）
        ContestMsg_PlayerPromotionRank,                   //玩家晋级排名结果
        ContestMsg_ContestScoreRank,                      //比赛玩家名次排行
        ContestMsg_GiveOutReward,                         //发放名次奖励
        ContestMsg_ContestPlayerDeskDivide,               //比赛玩家桌号分配数据发送给GameServer
        ContestMsg_GameServerCreateDeskReply,             //游戏服务器创建比赛游戏桌子回复
        ContestMsg_NotifyGameServerStartGame,             //通知游戏服务器开始游戏（桌子为单位）
        ContestMsg_PlayerEnterGameServerContestDesk,      //玩家进入比赛游戏服比赛桌子
        ContestMsg_ContestPlayerByePromotion,             //比赛玩家轮空晋级
        ContestMsg_ContestDisband,                        //比赛开启某状态未达到条件解散比赛
        ContestMsg_PacketContestInfoToGameServer,         //开始比赛打包比赛数据给游戏服务器（比赛总轮数，每轮晋级数）
        ContestMsg_RefreshGameingDeskCount,               //刷新游戏状态中的桌子数
        ContestMsg_RequestDeskInfo,                       //玩家请求比赛桌子信息(游戏中，游戏结束)
        ContestMsg_PacketDeskInfo,                        //打包比赛桌子信息(游戏中，游戏结束)
        ContestMsg_PlayerRequestLookOnDesk,               //玩家请求旁观某比赛桌子
        ContestMsg_BackDeskInfoToPlayer,                  //回复玩家比赛桌子的信息
        ContestMsg_RequestGameingRank,                    //玩家请求游戏进行中的排行榜
        ContestMsg_BackGameingRankToPlayer,               //回复玩家游戏进行中的排行榜
        ContestMsg_UpdatePlayerRankAfterOneOver,          //一桌结束后 更新已经结束的玩家的排名
        ContestMsg_PacketContestRoleScoreRank,            //打包玩家比赛积分排行榜（给服务器）
        ContestMsg_RequestbuyEnterNextRound,              //请求购买进入下一轮比赛的资格
        ContestMsg_RequestbuyEnterNextRoundReply,         //购买进入下一轮比赛资格的请求回复
        ContestMsg_SeekSubstitutes,                       //通知gamesvr替补种子
        ContestMsg_RequestSubstitutes,                    //玩家请求成为替补
        ContestMsg_RequestSubstitutesReply,               //玩家替补请求回复
        ContestMsg_RoundTimeOverForceEndGameingDesk,      //一轮比赛计时用完强制结束游戏中的比赛桌(通知游戏服)
        ContestMsg_PlayerVideoRecordData,                 //玩家比赛记录视频数据
        ContestMsg_PlayerDisConnectAtConntestState,       //玩家在比赛状态下断开连接（login通知比赛服处理）
        ContestMsg_EnrollDisband_NotifyPlayer,            //比赛报名状态解散比赛通知玩家
        ContestMsg_AdmissionDisband_NotifyPlayer,         //比赛入场状态解散比赛通知玩家
        ContestMsg_CommandContestDisband,                 //命令解散比赛

        //自建比赛
        Contestmsg_PlayerCreateContestRequest = 580,       //玩家自建比赛请求
        Contestmsg_PlayerCreateContestReply,               //玩家自建比赛请求回复
        Contestmsg_PlayerDisbandCreateContestReq,          //玩家解散自建比赛请求
        Contestmsg_PlayerDisbandCreateContestReply,        //玩家解散自建比赛回复
        Contestmsg_AnyTimeContestCurPlayerCount,           //即开赛当前参数人数
        ContestMsg_ChineseChessRankingList,                //象棋比赛排名榜单，用于赛事组织方查看结果

        //房卡消息
        AppointmentInit = 600,
        Appointment_CM_Join_1,
        Appointment_SM_Join_1,
        Appointment_CM_Join_2,
        Appointment_SM_Join_2,
        Appointment_CM_Create,
        Appointment_SM_Create,
        Appointment_CM_Exit,
        Appointment_SM_Exit,
        Appointment_CM_Switch,
        Appointment_SM_Switch,
        Appointment_CM_AgreeSwitch,
        Appointment_SM_AgreeSwitch,
        Appointment_CM_Ready,
        Appointment_SM_Ready,
        Appointment_SM_StartGame,
        Appointment_SM_ConnectGameServer,
        Appointment_SM_GameBackLogin,
        Appointment_SM_GameResult,
        Appointment_CM_Record,
        Appointment_SM_Record,
        Appointment_SM_TellLoginGameIndex,              //通知login 第几局结束了
        Appointment_SM_RecycleRoom,						//回收房间
        Appointment_SM_ClearReady,
        Appointment_CM_PublicRooms,                     //公开房间信息
        Appointment_SM_PublicRooms,

        //朋友圈消息
        Friends_Moments_CM_Init = 700,
        Friends_Moments_CM_Info,
        Friends_Moments_SM_Info,
        Friends_Moments_CM_Create,
        Friends_Moments_SM_Create,
        Friends_Moments_CM_Break,
        Friends_Moments_SM_Break,
        Friends_Moments_CM_AgreeJoin,
        Friends_Moments_SM_AgreeJoin,
        Friends_Moments_CM_Exit,
        Friends_Moments_SM_Exit,
        Friends_Moments_CM_Join,
        Friends_Moments_SM_Join,
        Friends_Moments_CM_KickOut,
        Friends_Moments_SM_KickOut,
        Friends_Moments_CM_CreateTable,
        Friends_Moments_SM_CreateTable,
        Friends_Moments_CM_BuyFriendsDiamond,
        Friends_Moments_SM_BuyFriendsDiamond,
        Friends_Moments_CM_LoginOrExit,               //成员上下线
        Friends_Moments_SM_LoginOrExit,
        Friends_Moments_CM_ChangeContent,                   //修改公告
        Friends_Moments_SM_ChangeContent,
        Friends_Moments_CM_MemberChangeNameOrIcon,      //成员修改名称和头像	
        Friends_Moments_SM_MemberChangeNameOrIcon,
        Friends_Moments_CM_JoinFriendsTable,
        Friends_Moments_SM_JoinFriendsTable,
        Friends_Moments_CM_CloseTable,
        Friends_Moments_SM_CloseTable,
        Friends_Moments_CM_BRIEFINFO,
        Friends_Moments_SM_BRIEFINFO,
        Friends_Moments_CM_Record,
        Friends_Moments_SM_Record,                  //朋友圈游戏记录
        Friends_Moments_CM_AddRecord,
        Friends_Moments_SM_AddRecord,               //朋友圈添加记录
        Friends_Moments_CM_LeaveFriendsTable,
        Friends_Moments_SM_LeaveFriendsTable,       //离开朋友圈桌子

        //胜利ideo
        CCVideoMsg_Begin = 1000,
        CCVideoMsg_RegisterToLogin,             //
        CCVideoMsg_BeginSet,                    //初始的时候发送玩家数据到videoser
        CCVideoMsg_BackBeginSet,                //返回videoid
        CCVideoMsg_TellLoginVideoID,            //通知login 玩家videoid
        CCVideoMsg_TotalRoundScore,             //总的每轮玩家的积分
        CCVideoMsg_SendRoundScoreToClint,       //发送给玩家
        CCVideoMsg_SendStepInfoToSer,           //每步的数据给videoser
        CCVideoMsg_SendStepInfoToClint,         //每步的数据给玩家
        CCVideoMsg_ApplyGetRoundScore,          //玩家申请获取每轮的积分
        CCVideoMsg_ApplyGetStepInfo,            //玩家申请获取每局中详细的数据

        //gate
        CCGateMsg_Begin = 2000,
        CCGateMsg_GateLoginMaster,                              //gate去master上注册
        CCGateMsg_MasterBackGateLogin,                          //回复
        CCGateMsg_GateLoginLoginSer,                            //gate去login上注册
        CCGateMsg_LoginSerBackGateLogin,                        //回复
        CCGateMsg_RoomSerLoginGateSer,                          //房间服务器登陆gateser
        CCGateMsg_BackRoomSerLoginGateSer,                      //回复
        CCGateMsg_LetRoomSerConnectGateSer,                     //动态开启gateser后 让roomser去连接
        CCGateMsg_PlayerApplyLoginGame,                         //玩家申请网管服务器去登陆游戏
        CCGateMsg_BackPlayerLoginGame,                          //回复
        CCGateMsg_PlayerReqConnIdForWxQR,                       //玩家申请服务器connId用于微信扫描二维码登陆
        CCGateMsg_BackPlayerReqConnIdForWxQR,                   //回复玩家申请服务器connId用于微信扫描二维码登陆
        CCGateMsg_UpdateConnectNum,								//gateser 上 的连接数更新

        //车行消息的开始
        CrazyCityMsg_CARPORT_BEGIN = 100000,

        //丢丢了消息的开始
        CrazyCityMsg_DIUDIULE_BEGIN = 100100,

        //拉霸消息的开始
        CrazyCityMsg_LABA_BEGIN = 100200,

        //森林舞会消息的开始
        CrazyCityMsg_FOREST_BEGIN = 100300, 

        //21点消息的开始
        CCMsg_BLACKJACK_BEGIN = 100400,
        CCMsg_BLACKJACK_CM_LOGIN,
        CCMsg_BLACKJACK_SM_LOGIN,
        CCMsg_BLACKJACK_CM_CHOOSElEVEL,
        CCMsg_BLACKJACK_SM_CHOOSElEVEL,
        CCMsg_BLACKJACK_SM_ENTERROOM,       //选择等级之后 直接进入房间
        CCMsg_BLACKJACK_SM_BOSSCHANGE,      //换人做庄
        CCMsg_BLACKJACK_SM_BOSSDOWN,        //庄家下庄
        CCMsg_BLACKJACK_CM_APPLYSITDOWN,    //玩家申请坐下
        CCMsg_BLACKJACK_SM_APPLYSITDOWN,    //回复
        CCMsg_BLACKJACK_CM_APPLYSTADN,      //玩家申请站起来
        CCMsg_BLACKJACK_SM_APPLYSTADN,      //回复
        CCMsg_BLACKJACK_SM_DEALPOKERSTOPLAYER,//发牌
        CCMsg_BLACKJACK_SM_ENTERGAMESTATE,      //告诉客户端 进入状态
        CCMsg_BLACKJACK_SM_ASKBUYSAFECOIN,      //询问是否需要购买保险
        CCMsg_BLACKJACK_CM_ANSWERBUYSAFE,       //玩家回复 购买保险
        CCMsg_BLACKJACK_SM_AGREEBUYSAFE,        //该玩家同意买保险之后 回复
        CCMsg_BLACKJACK_SM_ASKDOUBLEORNEED,     //询问是要加倍还是要加牌
        CCMsg_BLACKJACK_CM_ANSWERDOUBLEORNEED,  //回复
        CCMsg_BLACKJACK_SM_ANSWERDOUBLEORNEED,  //同步被询问的人的消息给其他玩家
        CCMsg_BLACKJACK_CM_APPLYBEBOSS,         //申请做庄
        CCMsg_BLACKJACK_SM_APPLYBEBOSS,         //回复
        CCMsg_BLACKJACK_SM_CUTPOKERS,           //询问玩家是否要分牌
        CCMsg_BLACKJACK_CM_APPLYLEAVEROOM,      //玩家申请离开房间
        CCMsg_BLACKJACK_SM_APPLYLEAVEROOM,      //回复
        CCMsg_BLACKJACK_SM_SITPLAYERLEAVEROOM,  //坐下的玩家离开房间
        CCMsg_BLACKJACK_CM_CHIPIN,              //玩家下注
        CCMsg_BLACKJACK_SM_CHIPIN,              //回复
        CCMsg_BLACKJACK_CM_APPLYBOSSLIST,       //玩家申请坐庄列表
        CCMsg_BLACKJACK_SM_APPLYBOSSLIST,       //
        CCMsg_BLACKJACK_CM_AAPLYCHANGEROOM,     //申请换房间
        CCMsg_BLACKJACK_SM_OPENBOSSHOLDPOKE,    //庄家打开扣着的牌
        CCMsg_BLACKJACK_SM_BUYSAFECOINRESULT,	//购买保险结束后 的结果发给客户端
        CCMsg_BLACKJACK_SM_NICKOUTROOMFORCOIN,	//因为钱不够被T出房间


        //五子棋消息的开始
        CCMsg_FIVEINROW_BEGIN = 100500,
        CCMsg_FIVEINROW_CM_LOGIN,
        CCMsg_FIVEINROW_SM_LOGIN,
        CCMsg_FIVEINROW_CM_CHOOSElEVEL,
        CCMsg_FIVEINROW_SM_CHOOSElEVEL,
        CCMsg_FIVEINROW_CM_CREATEROOM,  //创建房间
        CCMsg_FIVEINROW_SM_CREATEROOM,  //回复
        CCMsg_FIVEINROW_SM_SENDTABLEINFO,//发送房间数据给客户端
        CCMsg_FIVEINROW_CM_ENTERROOM,   //加入房间
        CCMsg_FIVEINROW_SM_ENTERROOM_FAIL,  //回复
        CCMsg_FIVEINROW_SM_ENTERROOM_READYHALL,//加入房间成功后给准备大厅其他人
        CCMsg_FIVEINROW_CM_LEAVEROOM,   //离开房间
        CCMsg_FIVEINROW_SM_LEAVEROOM,   //回复
        CCMsg_FIVEINROW_SM_AFTERENTERROOM,  //有玩家加入房间成功
        CCMsg_FIVEINROW_CM_SETSTAKECOIN,    //房主设置赌金
        CCMsg_FIVEINROW_SM_SETSTAKECOIN,    //回复
        CCMsg_FIVEINROW_CM_PLAYERREADY,     //玩家点击准备按钮
        CCMsg_FIVEINROW_SM_PLAYERREADY,     //回复
        CCMsg_FIVEINROW_CM_PLAYERPRESS,     //玩家下棋
        CCMsg_FIVEINROW_SM_PLAYERPRESS,     //回复
        CCMsg_FIVEINROW_SM_GAMESTART,       //游戏开始
        CCMsg_FIVEINROW_SM_GAMEOVER,        //游戏结束
        CCMsg_FIVEINROW_SM_NOTENOUGHSTAKECOIN,// 都准备好了 发现有人身上钱不够
        CCMsg_FIVEINROW_CM_PLAYERSURRENDER, //玩家投降

        CCMsg_FIVEINROW_SM_WAITAPPLYPEACE,  //符合求和要求 正在征询对方同意
        CCMsg_FIVEINROW_SM_ASKAPPLYPEACE,   //服务器询问另一个玩家 求和
        CCMsg_FIVEINROW_CM_ANSWERAPPLYPEACE,    //另一个玩家回答 是否同意 求和
        CCMsg_FIVEINROW_SM_BACKAPPLYPEACE,  //回复给申请求和的玩家

        CCMsg_FIVEINROW_SM_WAITMOVEBACK,    //符合悔棋要求 正在征询对方同意
        CCMsg_FIVEINROW_SM_ASKBACKMOVE,     //询问另一个玩家能不能悔棋
        CCMsg_FIVEINROW_CM_ANSWERBACKMOVE,  //另一个玩家回复能不能悔棋
        CCMsg_FIVEINROW_SM_TELLANSWERBACK,  //告诉另一个人别人的回答
        CCMsg_FIVEINROW_CM_NICKOUTOTHER,    //房主申请T人
        CCMsg_FIVEINROW_SM_BACKNICKOUT,     //通知房间里的人

        //德州扑克消息的开始
        CCMsg_TEXASPOKER_BEGIN = 100600,
        CCMsg_TEXASPOKER_CM_LOGIN,
        CCMsg_TEXASPOKER_SM_LOGIN,
        CCMsg_TEXASPOKER_CM_CHOOSElEVEL,
        CCMsg_TEXASPOKER_SM_CHOOSElEVEL,
        CCMsg_TEXASPOKER_SM_ENTERROOM,          //成功加入房间后把房间的信息发给他
        CCMsg_TEXASPOKER_SM_ENTERGAMESTATE,     //服务器状态改变了 同步给客户端
        CCMsg_TEXASPOKER_SM_DEALPOKERSTOPUBLIC, //往公共的发一张牌
        CCMsg_TEXASPOKER_CM_APPLYSITORSTAND,    //玩家申请坐下或者站起
        CCMsg_TEXASPOKER_SM_APPLYSITORSTAND,    //回复
        CCMsg_TEXASPOKER_SM_ASKCALLORDOUBLE,    //询问玩家是跟注 还是 加注 还是放弃
        CCMsg_TEXASPOKER_CM_ANSWERCALLORDOUBLE, //玩家回复
        CCMsg_TEXASPOKER_SM_ANSWERCALLORDOUBLE, //同步该玩家的 回答给所有人
        CCMsg_TEXASPOKER_SM_PUBLISHALLPOKERTYPE,
        CCMsg_TEXASPOKER_SM_PUBLISHCOINPOND,
        CCMsg_TEXASPOKER_SM_SENDRESULTTOPLAYER,
        CCMsg_TEXASPOKER_SM_SITPLAYERLEAVEROOM, //玩家掉线
        CCMsg_TEXASPOKER_SM_FORCELETSTAND,      //制让他站起来

        //斗地主消息的开始
        CCMsg_LANDLORDS_BEGIN = 100700,
        CCMsg_LANDLORDS_CM_LOGIN,
        CCMsg_LANDLORDS_SM_LOGIN,
        CCMsg_LANDLORDS_CM_CHOOSElEVEL,
        CCMsg_LANDLORDS_SM_CHOOSElEVEL,
        CCMsg_LANDLORDS_SM_ENTERROOM,           //成功加入房间后把房间的信息发给他
        CCMsg_LANDLORDS_SM_ENTERGAMESTATE,      //发送游戏状态到客户端
        CCMsg_LANDLORDS_SM_BEGINDEALPOKER,      //游戏开始的时候发送17张牌给各个玩家
        CCMsg_LANDLORDS_SM_ASKBELORDS,          //服务器主动询问玩家是否要地主
        CCMsg_LANDLORDS_CM_ASKBELORDS,          //玩家回复
        CCMsg_LANDLORDS_SM_BELORDSFAILED,       //叫地主失败
        CCMsg_LANDLORDS_SM_PUBLISHBELORDS,      //同步该玩家的回答给所有人
        CCMsg_LANDLORDS_SM_PUBLISHLORDSPOKER,   //公布地主的三张牌
        CCMsg_LANDLORDS_SM_ASKDEALPOKER,        //询问出牌
        CCMsg_LANDLORDS_CM_ASKDEALPOKER,        //玩家回复出牌
        CCMsg_LANDLORDS_SM_DEALPOKERFAILED,     //玩家出牌失败
        CCMsg_LANDLORDS_SM_PUBLISHDEALPOKER,    //公布该玩家出牌的结果
        CCMsg_LANDLORDS_SM_PUBLISHRESULT,   //同步结果给客户端
        CCMsg_LANDLORDS_CM_ENTERMATCH,      //进入匹配
        CCMsg_LANDLORDS_SM_ENTERMATCH,      //返回
        CCMsg_LANDLORDS_CM_CANCLEMATCH,     //取消匹配
        CCMsg_LANDLORDS_SM_CANCLEMATCH,     //返回
        CCMsg_LANDLORDS_SM_RESTART,         //没人要地主重新开始
        CCMsg_LANDLORDS_CM_EMOTION,         //客户端发表情
        CCMsg_LANDLORDS_SM_EMOTION,         //返回
        CCMsg_LANDLORDS_CM_APPLYLEAVEROOM,      //玩家申请离开房间
        CCMsg_LANDLORDS_SM_APPLYLEAVEROOM,      //回复
        CCMsg_LANDLORDS_SM_MIDDLEENTERROOM,		//中途加入
        CCMsg_LANDLORDS_SM_PLAYERDISORRECONNECT,//玩家掉线或者重新上线
        CCMsg_LANDLORDS_SM_AFTERONLOOKERENTER,	//围观的玩家进入房间
        CCMsg_LANDLORDS_CM_LEAVEONLOOKERROOM,				//玩家离开围观的房间

        //百人牛牛消息的开始（2堆的）
        CCMsg_BULLHUNDRED_BEGIN = 100800,
        CCMsg_BULLHUNDRED_CM_LOGIN,
        CCMsg_BULLHUNDRED_SM_LOGIN,
        CCMsg_BULLHUNDRED_CM_CHOOSElEVEL,
        CCMsg_BULLHUNDRED_SM_CHOOSElEVEL,
        CCMsg_BULLHUNDRED_SM_ENTERROOM,     //进入房间
        CCMsg_BULLHUNDRED_SM_ROOMSTATE,     //同步房间状态到客户端
        CCMsg_BULLHUNDRED_SM_BOSSCHANGE,    //坐庄的额改变
        CCMsg_BULLHUNDRED_SM_BOSSDOWN,      //下庄
        CCMsg_BULLHUNDRED_CM_APPLYBEBOSS,
        CCMsg_BULLHUNDRED_SM_APPLYBEBOSS,
        CCMsg_BULLHUNDRED_CM_APPLYBOSSLIST,
        CCMsg_BULLHUNDRED_SM_APPLYBOSSLIST,
        CCMsg_BULLHUNDRED_CM_CHIPIN,
        CCMsg_BULLHUNDRED_SM_CHIPIN,
        CCMsg_BULLHUNDRED_SM_PUBLISHCHIPIN, //有玩家下注 同步消息给所有人
        CCMsg_BULLHUNDRED_SM_DEALPOKER,     //发牌
        CCMsg_BULLHUNDRED_SM_PUBLISHRESULT, //同步结果给客户端
        CCMsg_BULLHUNDRED_CM_APPLYSITVIP,
        CCMsg_BULLHUNDRED_SM_APPLYSITVIP,
        CCMsg_BULLHUNDRED_SM_FORCELETSTAND,
        CCMsg_BULLHUNDRED_CM_APPLYLEAVEROOM,        //玩家申请离开房间
        CCMsg_BULLHUNDRED_SM_APPLYLEAVEROOM,		//回复
        CCMsg_BULLHUNDRED_SM_PUBLISHROLENUM,		//同步随机的玩家人数


        //通杀牛牛消息的开始（5堆的）
        CCMsg_BULLKILL_BEGIN = 100900,
        CCMsg_BULLKILL_CM_LOGIN,
        CCMsg_BULLKILL_SM_LOGIN,
        CCMsg_BULLKILL_CM_CHOOSElEVEL,
        CCMsg_BULLKILL_SM_CHOOSElEVEL,
        CCMsg_BULLKILL_SM_ENTERROOM,
        CCMsg_BULLKILL_SM_ROOMSTATE,        //同步房间状态到客户端
        CCMsg_BULLKILL_SM_BOSSCHANGE,   //坐庄的额改变
        CCMsg_BULLKILL_SM_BOSSDOWN,     //下庄
        CCMsg_BULLKILL_SM_MAKEFIRSTPOKER,   //第一个发牌的人是谁
        CCMsg_BULLKILL_SM_PUBLISHPOKERS,    //打开所有牌
        CCMsg_BULLKILL_CM_APPLYBEBOSS,
        CCMsg_BULLKILL_SM_APPLYBEBOSS,
        CCMsg_BULLKILL_CM_APPLYBOSSLIST,
        CCMsg_BULLKILL_SM_APPLYBOSSLIST,
        CCMsg_BULLKILL_CM_CHIPIN,
        CCMsg_BULLKILL_SM_CHIPIN,
        CCMsg_BULLKILL_SM_PUBLISHCHIPIN,    //有玩家下注 同步消息给所有人
        CCMsg_BULLKILL_CM_APPLYSITVIP,
        CCMsg_BULLKILL_SM_APPLYSITVIP,
        CCMsg_BULLKILL_SM_FORCELETSTAND,
        CCMsg_BULLKILL_SM_PUBLISHRESULT,    //同步结果给客户端
        CCMsg_BULLKILL_CM_APPLYLEAVEROOM,       //玩家申请离开房间
        CCMsg_BULLKILL_SM_APPLYLEAVEROOM,		//回复
        CCMsg_BULLKILL_SM_PUBLISHROLENUM,       //同步玩家人数

        //捕魚消息的
        CCMsg_FISHING_BEING = 101000,
        CCMsg_FISHING_CM_LOGIN,
        CCMsg_FISHING_SM_LOGIN,
        CCMsg_FISHING_CM_CHOOSElEVEL,
        CCMsg_FISHING_SM_CHOOSElEVEL,
        CCMsg_FISHING_SM_ENTERROOM,             //进入房间成功
        CCMsg_FISHING_SM_FISHBORN,              //鱼出生了
        CCMsg_FISHING_SM_FISHDEAD,              //鱼死亡了
        CCMsg_FISHING_CM_APPLYFIRE,             //玩家申请开火
        CCMsg_FISHING_SM_BACKFIRE,              //
        CCMsg_FISHING_CM_FIRERESULT,            //客户端告诉服务器否是打中
        CCMsg_FISHING_SM_FIRETESULT,            //同步给所有人
        CCMsg_FISHING_CM_APPLYLEAVE,            //申请退出
        CCMsg_FISHING_SM_BACKLEAVE,             //回复给离开的人
        CCMsg_FISHING_CM_APPLYCHANGEROOM,		//申请换个房间
        CCMsg_FISHING_SM_OTHERPLAYERENTER,      //告诉已经进入房间的人
        CCMsg_FISHING_CM_CHANGECANNON,          //玩家改变炮台
        CCMsg_FISHING_SM_CHANGECANNON,          //同步给房间里所有人
        CCMsg_FISHING_CM_CHANGECANNONLEVEL,     //玩家要改变等级
        CCMsg_FISHING_SM_CHANGECANNONLEVEL,     //同步给房间里所有人
        CCMsg_FISHING_SM_UPDATEROOMSTATE,       //更新房间状态给客户端
        CCMsg_FISHING_CM_PLAYERBUGCANNON,       //玩家申请购物炮台	
        CCMsg_FISHING_SM_PLAYERBUGCANNON,       //回复
        CCMsg_FISHING_CM_USEFISHINGSKILL,       //玩家申请使用技能
        CCMsg_FISHING_SM_BACKUSEFISHINGSKILL,   //回复给这个玩家
        CCMsg_FISHING_SM_UPDATEUSEFISHINGSKILL, //同步给除此外加以外的其他人
        CCMsg_FISHING_CM_TRACECHANGETARGET,     //追踪的技能改变目标
        CCMsg_FISHING_SM_TRACECHANGETARGET,		//同步给其他人
        CCMsg_FISHING_SM_TRACESKILLTIMEOVER,    //追踪的技能时间结束
        CCMsg_FISHING_SM_KICKOUTROOM,			//一定时间不开火就剔除房间
        CCMsg_FISHING_SM_FIREFAILED,			//开火失败
        CCMsg_FISHING_CM_APPLYCORNUCOPIA,       //获取聚宝盆的奖励
        CCMsg_FISHING_SM_APPLYCORNUCOPIA,       //活肤
        CCMsg_FISHING_CM_APPLYSEALREWORD,       //获取海豹的奖励
        CCMsg_FISHING_SM_APPLYSEALREWORD,		//活肤

        //抢庄牛牛
        CCMsg_BULLHAPPY_BEING = 101100,
        CCMsg_BULLHAPPY_CM_LOGIN,
        CCMsg_BULLHAPPY_SM_LOGIN,
        CCMsg_BULLHAPPY_CM_CHOOSElEVEL,
        CCMsg_BULLHAPPY_SM_CHOOSElEVEL,
        CCMsg_BULLHAPPY_SM_OTHERENTERROOM,
        CCMsg_BULLHAPPY_SM_ENTERROOM,
        CCMsg_BULLHAPPY_SM_TIMECOUNT,
        CCMsg_BULLHAPPY_SM_PUBLISHPOKERS,
        CCMsg_BULLHAPPY_CM_DEALBOSS,
        CCMsg_BULLHAPPY_SM_DEALBOSS,
        CCMsg_BULLHAPPY_CM_CHIPIN,
        CCmsg_BULLHAPPY_SM_CHIPIN,
        CCmsg_BULLHAPPY_CM_CACULATE,
        CCmsg_BULLHAPPY_SM_CACULATE,
        CCmsg_BULLHAPPY_SM_OPENPOKERS,
        CCmsg_BULLHAPPY_SM_GAMERESULTS,
        CCMsg_BULLHAPPY_SM_RESULT,

        //麻将
        CCMsg_MAHJONG_BEING = 101200,
        CCMsg_MAHJONG_CM_LOGIN,
        CCMsg_MAHJONG_SM_LOGIN,
        CCMsg_MAHJONG_CM_CHOOSElEVEL,
        CCMsg_MAHJONG_SM_CHOOSElEVEL,
        CCMsg_MAHJONG_SM_ENTERROOM,             //进入房间成功
        CCMsg_MAHJONG_SM_ROOMSTATE,     //同步房间状态到客户端
        CCMsg_MAHJONG_SM_ASKREADY,      //服务器让玩家准备
        CCMsg_MAHJONG_CM_ANSWERREADY,   //玩家回复准备
        CCMsg_MAHJONG_SM_UPDATEREADY,   //该玩家准备好的消息通知其他人
        CCMsg_MAHJONG_SM_DEALMJBEGIN,   //开始的时候每人13张牌
        CCMsg_MAHJONG_CM_CHANGEPOKERS,  //玩家换张
        CCMsg_MAHJONG_SM_UPDATECHANGEPOKERS,    //告诉所有此人换完牌了
        CCMsg_MAHJONG_CM_MAKELACK,          //玩家定缺
        CCMsg_MAHJONG_SM_UPDATEMAKELACK,    //告诉所此人定缺
        CCMsg_MAHJONG_CM_PLAYERDEALMJPOKER, //玩家出牌
        CCMsg_MAHJONG_SM_BACKDEALMJPOKER,   //回复出牌玩家
        CCMsg_MAHJONG_SM_UPDATEDEALPOKER,   //同步出牌出去
        CCMsg_MAHJONG_SM_FIRSTASKBANKERDEAL,//定完缺之后 让坐庄的人首先出牌
        CCMsg_MAHJONG_CM_ANSWERDOING,       //玩家回复 碰 胡 杠
        CCMsg_MAHJONG_SM_ANSWERDOING,       //回复此人
        CCMsg_MAHJONG_SM_UPDATEDOING,       //同步出去
        CCMsg_MAHJONG_SM_GETPOKERASKDEAL,   //发一张牌给该玩家并让他出牌
        CCMsg_MAHJONG_SM_PLAYERHUPOKER,     //玩家胡牌
        CCMsg_MAHJONG_SM_PUBLISHRESULT,		//同步游戏结果
        CCMsg_MAHJONG_SM_OTHERENTER,        //告诉其他人 此人加入房间
        CCMsg_MAHJONG_CM_APPLYLEAVE,        //申请退出
        CCMsg_MAHJONG_SM_BACKLEAVE,			//回复给离开的人
        CCMsg_MAHJONG_SM_TRUSTCHANGEPOKERS,     //托管换牌
        CCMsg_MAHJONG_SM_ENTERTRUSTSTATE,       //通知客户端进入托管状态
        CCMsg_MAHJONG_CM_CANCLETRUSTSTATE,      //玩家取消托管状态
        CCMsg_MAHJONG_SM_MIDDLEENTERROOM,       //玩家中途加入
        CCMsg_MAHJONG_SM_AFTERONLOOKERENTER,    //围观的玩家进入房间
        CCMsg_MAHJONG_CM_LEAVEONLOOKERROOM,             //玩家离开围观的房间
        CCMsg_MAHJONG_SM_AFTERQIANGGANGHUCUTCOIN,//WaitQiangGangHu状态下 别的玩家不胡的 接下来扣钱的函数
        CCMsg_MAHJONG_SM_LIUJUINFO,         //退回刚的税 查大叫 查花猪
        CCMsg_MAHJONG_SM_HUJIAOZHUANYI,         //呼叫转移

        //盐城麻将
        CCMsg_YCMAHJONG_BEING = 101400,
        CCMsg_YCMAHJONG_CM_LOGIN,
        CCMsg_YCMAHJONG_SM_LOGIN,
        CCMsg_YCMAHJONG_CM_CHOOSElEVEL,
        CCMsg_YCMAHJONG_SM_CHOOSElEVEL,
        CCMsg_YCMAHJONG_SM_ENTERROOM,               //进入房间成功
        CCMsg_YCMAHJONG_SM_ROOMSTATE,       //同步房间状态到客户端
        CCMsg_YCMAHJONG_SM_ASKREADY,        //服务器让玩家准备
        CCMsg_YCMAHJONG_CM_ANSWERREADY, //玩家回复准备
        CCMsg_YCMAHJONG_SM_UPDATEREADY, //该玩家准备好的消息通知其他人
        CCMsg_YCMAHJONG_SM_DEALMJBEGIN, //开始的时候每人13张牌
        CCMsg_YCMAHJONG_CM_PLAYERDEALMJPOKER,   //玩家出牌
        CCMsg_YCMAHJONG_SM_BACKDEALMJPOKER, //回复出牌玩家
        CCMsg_YCMAHJONG_SM_UPDATEDEALPOKER, //同步出牌出去
        CCMsg_YCMAHJONG_SM_FIRSTASKBANKERDEAL,//定完缺之后 让坐庄的人首先出牌
        CCMsg_YCMAHJONG_CM_ANSWERDOING,     //玩家回复 碰 胡 杠
        CCMsg_YCMAHJONG_SM_ANSWERDOING,     //回复此人
        CCMsg_YCMAHJONG_SM_UPDATEDOING,     //同步出去
        CCMsg_YCMAHJONG_SM_GETPOKERASKDEAL, //发一张牌给该玩家并让他出牌
        CCMsg_YCMAHJONG_SM_PLAYERHUPOKER,       //玩家胡牌
        CCMsg_YCMAHJONG_SM_PUBLISHRESULT,       //同步游戏结果
        CCMsg_YCMAHJONG_SM_OTHERENTER,      //告诉其他人 此人加入房间
        CCMsg_YCMAHJONG_CM_APPLYLEAVE,          //申请退出
        CCMsg_YCMAHJONG_SM_BACKLEAVE,               //回复给离开的人
        CCMsg_YCMAHJONG_SM_TRUSTCHANGEPOKERS,       //托管换牌
        CCMsg_YCMAHJONG_SM_ENTERTRUSTSTATE,     //通知客户端进入托管状态
        CCMsg_YCMAHJONG_CM_CANCLETRUSTSTATE,        //玩家取消托管状态
        CCMsg_YCMAHJONG_SM_MIDDLEENTERROOM,     //玩家中途加入
        CCMsg_YCMAHJONG_SM_AFTERONLOOKERENTER,  //围观的玩家进入房间
        CCMsg_YCMAHJONG_CM_LEAVEONLOOKERROOM,               //玩家离开围观的房间
        CCMsg_YCMAHJONG_SM_UPDATEPOKERSAFTERBUHUA,	//补花结束后 再同步一次
        CCMsg_YCMAHJONG_CM_PLAYERAPPLYTINGORFEI,    //玩家申请听或者飞听
        CCMsg_YCMAHJONG_SM_BACKPLAYERTINGORFEI,     //回复
        CCMsg_YCMAHJONG_SM_UPDATEPLAYERTINGORFEI,	//同步出去

        //掼蛋
        CCMsg_GUANDAN_BEING = 101300,
        CCMsg_GUANDAN_CM_LOGIN,
        CCMsg_GUANDAN_SM_LOGIN,
        CCMsg_GUANDAN_CM_CHOOSELEVEL,
        CCMsg_GUANDAN_SM_CHOOSELEVEL,
        CCMsg_GUANDAN_SM_ENTERROOM,
        CCMsg_GUANDAN_SM_ROOMSTATE,     //同步房间状态到客户端
        CCMsg_GUANDAN_SM_ASKREADY,      //询问是否准备
        CCMsg_GUANDAN_CM_ANSWERREADY,   //回复准备好
        CCMsg_GUANDAN_SM_DEALMJBEGIN,   //发牌
        CCMsg_GUANDAN_CM_SUBMITPOKER,   //玩家上贡
        CCMsg_GUANDAN_SM_SUBMITPOKER,   //回复
        CCMsg_GUANDAN_CM_RETURNPOKER,   //玩家还贡
        CCMsg_GUANDAN_SM_RETURNPOKER,   //回复
        CCMsg_GUANDAN_CM_OUTPOKERS,     //玩家出牌
        CCMsg_GUANDAN_SM_OUTPOKERS,     //失败了才恢复这个回复
        CCMsg_GUANDAN_SM_PUBLISHOUTPOKER,//玩家出牌同步出去
        CCMsg_GUANDAN_SM_PUBLISHRESULT,//同步 结果
        CCMsg_GUANDAN_SM_LETOUTPOKER,	//通知客户端让这个玩家出牌
        CCMsg_GUANDAN_SM_FRIENDPOKERS,  //一个玩家结束把队友当前的牌同步给他
        CCMsg_GUANDAN_CM_APPLYLEAVE,            //申请退出
        CCMsg_GUANDAN_SM_BACKLEAVE,             //回复给离开的人
        CCMsg_GUANDAN_SM_AFTERALLSUBMIT,    //大家都上贡完了之后
        CCMsg_GUANDAN_SM_AFTERALLRETURN,    //大家都还贡了
        CCMsg_GUANDAN_CM_EMOTION,           //客户端发表情
        CCMsg_GUANDAN_SM_EMOTION,           //返回
        CCMsg_GUANDAN_SM_MIDDLEENTERROOM,   //中途加入
        CCMsg_GUANDAN_CM_AGAINGAME,         //玩家申请再来一次（匹配场）
        CCMsg_GUANDAN_SM_AGAINGAME,         //回复
        CCMsg_GUANDAN_CM_ENTERMATCH,        //进入匹配
        CCMsg_GUANDAN_SM_ENTERMATCH,        //返回
        CCMsg_GUANDAN_CM_CANCLEMATCH,       //取消匹配
        CCMsg_GUANDAN_SM_CANCLEMATCH,		//返回
        CCMsg_GUANDAN_SM_AFTERONLOOKERENTER,	//围观的玩家进入房间
        CCMsg_GUANDAN_CM_LEAVEONLOOKERROOM,             //玩家离开围观的房间

        //常州麻将
        CCMsg_CZMAHJONG_BEING = 101500,
        CCMsg_CZMAHJONG_CM_LOGIN,
        CCMsg_CZMAHJONG_SM_LOGIN,
        CCMsg_CZMAHJONG_CM_CHOOSElEVEL,
        CCMsg_CZMAHJONG_SM_CHOOSElEVEL,
        CCMsg_CZMAHJONG_SM_ENTERROOM,               //进入房间成功
        CCMsg_CZMAHJONG_SM_ROOMSTATE,               //同步房间状态到客户端
        CCMsg_CZMAHJONG_SM_ASKREADY,                //服务器让玩家准备
        CCMsg_CZMAHJONG_CM_ANSWERREADY,             //玩家回复准备
        CCMsg_CZMAHJONG_SM_UPDATEREADY,             //该玩家准备好的消息通知其他人
        CCMsg_CZMAHJONG_SM_DEALMJBEGIN,             //开始的时候每人13张牌
        CCMsg_CZMAHJONG_CM_PLAYERDEALMJPOKER,       //玩家出牌
        CCMsg_CZMAHJONG_SM_BACKDEALMJPOKER,         //回复出牌玩家
        CCMsg_CZMAHJONG_SM_UPDATEDEALPOKER,         //同步出牌出去
        CCMsg_CZMAHJONG_SM_FIRSTASKBANKERDEAL,      //定完缺之后 让坐庄的人首先出牌
        CCMsg_CZMAHJONG_CM_ANSWERDOING,             //玩家回复 碰 胡 杠
        CCMsg_CZMAHJONG_SM_ANSWERDOING,             //回复此人
        CCMsg_CZMAHJONG_SM_UPDATEDOING,             //同步出去
        CCMsg_CZMAHJONG_SM_GETPOKERASKDEAL,         //发一张牌给该玩家并让他出牌
        CCMsg_CZMAHJONG_SM_PLAYERHUPOKER,           //玩家胡牌
        CCMsg_CZMAHJONG_SM_PUBLISHRESULT,           //同步游戏结果
        CCMsg_CZMAHJONG_SM_OTHERENTER,              //告诉其他人 此人加入房间
        CCMsg_CZMAHJONG_CM_APPLYLEAVE,              //申请退出
        CCMsg_CZMAHJONG_SM_BACKLEAVE,               //回复给离开的人
        CCMsg_CZMAHJONG_SM_TRUSTCHANGEPOKERS,       //托管换牌
        CCMsg_CZMAHJONG_SM_ENTERTRUSTSTATE,         //通知客户端进入托管状态
        CCMsg_CZMAHJONG_CM_CANCLETRUSTSTATE,        //玩家取消托管状态
        CCMsg_CZMAHJONG_SM_MIDDLEENTERROOM,         //玩家中途加入
        CCMsg_CZMAHJONG_SM_AFTERONLOOKERENTER,      //围观的玩家进入房间
        CCMsg_CZMAHJONG_CM_LEAVEONLOOKERROOM,       //玩家离开围观的房间
        CCMsg_CZMAHJONG_SM_UPDATEPOKERSAFTERBUHUA,  //补花结束后 再同步一次

        //幸运转盘
        CCMsg_TURNTABLE_BEING = 101600,
        CCMsg_TURNTABLE_CM_LOGIN,
        CCMsg_TURNTABLE_SM_LOGIN,
        CCMsg_TURNTABLE_CM_CHOOSElEVEL,
        CCMsg_TURNTABLE_SM_CHOOSElEVEL,
        CCMsg_TURNTABLE_SM_ENTERROOM,				//进入房间成功
        CCMsg_TURNTABLE_CM_APPLYLEAVE,              //申请退出
        CCMsg_TURNTABLE_SM_BACKLEAVE,               //回复给离开的人
        CCMsg_TURNTABLE_LotteryDraw,                //抽奖
        CCMsg_TURNTABLE_Bonus,                      //有人中奖

        //够级
        CCMsg_GOUJI_BEING = 101700,
        CCMsg_GOUJI_CM_LOGIN,
        CCMsg_GOUJI_SM_LOGIN,
        CCMsg_GOUJI_CM_CHOOSElEVEL,
        CCMsg_GOUJI_SM_CHOOSElEVEL,
        CCMsg_GOUJI_SM_ENTERROOM,               //进入房间成功
        CCMsg_GOUJI_SM_ROOMSTATE,               //同步房间状态到客户端
        CCMsg_GOUJI_SM_ASKREADY,                //服务器让玩家准备
        CCMsg_GOUJI_CM_ANSWERREADY,             //玩家回复准备
        CCMsg_GOUJI_SM_UPDATEREADY,             //该玩家准备好的消息通知其他人
        CCMsg_GOUJI_SM_DEALMJBEGIN,             //开始发牌
        CCMsg_GOUJI_CM_PLAYERDEALMJPOKER,       //玩家出牌
        CCMsg_GOUJI_SM_BACKDEALMJPOKER,         //回复出牌玩家
        CCMsg_GOUJI_SM_UPDATEDEALPOKER,         //同步出牌同步
        CCMsg_GOUJI_CM_ANSWERDOING,             //玩家回复
        CCMsg_GOUJI_SM_ANSWERDOING,             //回复此人
        CCMsg_GOUJI_SM_UPDATEDOING,             //同步出去
        CCMsg_GOUJI_SM_ASKDEAL,                 //提问
        CCMsg_GOUJI_SM_PUBLISHASKDEAL,          //同步提问
        CCMsg_GOUJI_SM_PUBLISHRESULT,           //同步游戏结果
        CCMsg_GOUJI_SM_OTHERENTER,              //告诉其他人 此人加入房间
        CCMsg_GOUJI_CM_APPLYLEAVE,              //申请退出
        CCMsg_GOUJI_SM_BACKLEAVE,               //回复给离开的人
        CCMsg_GOUJI_SM_TRUSTCHANGEPOKERS,       //托管换牌
        CCMsg_GOUJI_SM_ENTERTRUSTSTATE,         //通知客户端进入托管状态
        CCMsg_GOUJI_CM_CANCLETRUSTSTATE,        //玩家取消托管状态
        CCMsg_GOUJI_SM_MIDDLEENTERROOM,         //玩家中途加入
        CCMsg_GOUJI_SM_AFTERONLOOKERENTER,      //围观的玩家进入房间
        CCMsg_GOUJI_CM_LEAVEONLOOKERROOM,       //玩家离开围观的房间
        CCMsg_GOUJI_CM_CHANGEFRIEND,            //请求看另一个队友的牌
        CCMsg_GOUJI_SM_FRIENDPOKERS,            //同步队友的牌
        CCMsg_GOUJI_SM_YAOTOUINFO,              //通知要头相关
        CCMsg_GOUJI_SM_XUANDIANINFO,            //通知宣点相关
        CCMsg_GOUJI_SM_CLEARRANGPAI,            //通知取消让牌
        CCMsg_GOUJI_SM_KAIDIAN,					//同步开点情况
        CCMsg_GOUJI_SM_4HULUANCHAN,				//通知进入四户乱缠
        CCMsg_GOUJI_SM_BIE3,                    //通知憋3

        //红中麻将
        CCMsg_HONGMAHJONG_BEING = 101800,
        CCMsg_HONGMAHJONG_CM_LOGIN,
        CCMsg_HONGMAHJONG_SM_LOGIN,
        CCMsg_HONGMAHJONG_CM_CHOOSElEVEL,
        CCMsg_HONGMAHJONG_SM_CHOOSElEVEL,
        CCMsg_HONGMAHJONG_SM_ENTERROOM,             //进入房间成功
        CCMsg_HONGMAHJONG_SM_ROOMSTATE,             //同步房间状态到客户端
        CCMsg_HONGMAHJONG_SM_ASKREADY,              //服务器让玩家准备
        CCMsg_HONGMAHJONG_CM_ANSWERREADY,           //玩家回复准备
        CCMsg_HONGMAHJONG_SM_UPDATEREADY,           //该玩家准备好的消息通知其他人
        CCMsg_HONGMAHJONG_SM_DEALMJBEGIN,           //开始的时候每人13张牌
        CCMsg_HONGMAHJONG_CM_PLAYERDEALMJPOKER,     //玩家出牌
        CCMsg_HONGMAHJONG_SM_BACKDEALMJPOKER,       //回复出牌玩家
        CCMsg_HONGMAHJONG_SM_UPDATEDEALPOKER,       //同步出牌出去
        CCMsg_HONGMAHJONG_SM_FIRSTASKBANKERDEAL,    //定完缺之后 让坐庄的人首先出牌
        CCMsg_HONGMAHJONG_CM_ANSWERDOING,           //玩家回复 碰 胡 杠
        CCMsg_HONGMAHJONG_SM_ANSWERDOING,           //回复此人
        CCMsg_HONGMAHJONG_SM_UPDATEDOING,           //同步出去
        CCMsg_HONGMAHJONG_SM_GETPOKERASKDEAL,       //发一张牌给该玩家并让他出牌
        CCMsg_HONGMAHJONG_SM_PLAYERHUPOKER,         //玩家胡牌
        CCMsg_HONGMAHJONG_SM_PUBLISHRESULT,         //同步游戏结果
        CCMsg_HONGMAHJONG_SM_OTHERENTER,            //告诉其他人 此人加入房间
        CCMsg_HONGMAHJONG_CM_APPLYLEAVE,            //申请退出
        CCMsg_HONGMAHJONG_SM_BACKLEAVE,             //回复给离开的人
        CCMsg_HONGMAHJONG_SM_TRUSTCHANGEPOKERS,     //托管换牌
        CCMsg_HONGMAHJONG_SM_ENTERTRUSTSTATE,       //通知客户端进入托管状态
        CCMsg_HONGMAHJONG_CM_CANCLETRUSTSTATE,      //玩家取消托管状态
        CCMsg_HONGMAHJONG_SM_MIDDLEENTERROOM,       //玩家中途加入
        CCMsg_HONGMAHJONG_SM_AFTERONLOOKERENTER,    //围观的玩家进入房间
        CCMsg_HONGMAHJONG_CM_LEAVEONLOOKERROOM,		//玩家离开围观的房间
        CCMsg_HONGMAHJONG_SM_AFTERQIANGGANGHUCUTCOIN,//WaitQiangGangHu状态下 别的玩家不胡的 接下来扣钱的函数

        //答题赛
        CCMsg_ANSWER_BEING = 101900,
        CCMsg_ANSWER_CM_LOGIN,
        CCMsg_ANSWER_SM_LOGIN,
        CCMsg_ANSWER_CM_CHOOSElEVEL,
        CCMsg_ANSWER_SM_CHOOSElEVEL,
        CCMsg_ANSWER_SM_ENTERROOM,              //进入房间成功
        CCMsg_ANSWER_SM_ROOMSTATE,              //同步房间状态到客户端
        CCMsg_ANSWER_SM_ASKREADY,               //服务器让玩家准备
        CCMsg_ANSWER_CM_ANSWERREADY,            //玩家回复准备
        CCMsg_ANSWER_SM_UPDATEREADY,            //该玩家准备好的消息通知其他人
        CCMsg_ANSWER_SM_DEALMJBEGIN,            //开始的时候每人13张牌
        CCMsg_ANSWER_CM_PLAYERDEALMJPOKER,      //玩家出牌
        CCMsg_ANSWER_SM_BACKDEALMJPOKER,        //回复出牌玩家
        CCMsg_ANSWER_SM_UPDATEDEALPOKER,        //同步出牌出去
        CCMsg_ANSWER_SM_FIRSTASKBANKERDEAL,     //定完缺之后 让坐庄的人首先出牌
        CCMsg_ANSWER_CM_ANSWERDOING,            //玩家回复 碰 胡 杠
        CCMsg_ANSWER_SM_ANSWERDOING,            //回复此人
        CCMsg_ANSWER_SM_UPDATEDOING,            //同步出去
        CCMsg_ANSWER_SM_GETPOKERASKDEAL,        //发一张牌给该玩家并让他出牌
        CCMsg_ANSWER_SM_PLAYERHUPOKER,          //玩家胡牌
        CCMsg_ANSWER_SM_PUBLISHRESULT,          //同步游戏结果
        CCMsg_ANSWER_SM_OTHERENTER,             //告诉其他人 此人加入房间
        CCMsg_ANSWER_CM_APPLYLEAVE,             //申请退出
        CCMsg_ANSWER_SM_BACKLEAVE,              //回复给离开的人
        CCMsg_ANSWER_SM_TRUSTCHANGEPOKERS,      //托管换牌
        CCMsg_ANSWER_SM_ENTERTRUSTSTATE,        //通知客户端进入托管状态
        CCMsg_ANSWER_CM_CANCLETRUSTSTATE,       //玩家取消托管状态
        CCMsg_ANSWER_SM_MIDDLEENTERROOM,        //玩家中途加入
        CCMsg_ANSWER_SM_AFTERONLOOKERENTER,     //围观的玩家进入房间
        CCMsg_ANSWER_CM_LEAVEONLOOKERROOM,      //玩家离开围观的房间

        ///象棋
		CCMsg_CChess_BEING = 102000,
        CCMsg_CChess_CM_LOGIN,
        CCMsg_CChess_SM_LOGIN,
        CCMsg_CChess_CM_CHOOSElEVEL,
        CCMsg_CChess_SM_CHOOSElEVEL,
        CCMsg_CChess_SM_ENTERROOM,              //进入房间成功
        CCMsg_CChess_SM_ROOMSTATE,              //同步房间状态到客户端
        CCMsg_CChess_SM_ASKREADY,               //服务器让玩家准备
        CCMsg_CChess_CM_ANSWERREADY,            //玩家回复准备
        CCMsg_CChess_SM_UPDATEREADY,            //该玩家准备好的消息通知其他人
        CCMsg_CChess_SM_DEALBEGIN,              //开始摆好棋
        CCMsg_CChess_CM_ANSWERDOING,            //玩家下棋
        CCMsg_CChess_SM_ANSWERDOING,            //回复
        CCMsg_CChess_SM_ASKDEAL,                //轮到玩家下
        CCMsg_CChess_WithDraw,                  //悔棋
        CCMsg_CChess_RequestDraw,               //求和
        CCMsg_CChess_RequestDrawResult,         //求和结果
        CCMsg_CChess_GiveUp,					//投降
        CCMsg_CChess_SM_PUBLISHRESULT,          //同步游戏结果
        CCMsg_CChess_SM_OTHERENTER,             //告诉其他人 此人加入房间
        CCMsg_CChess_CM_APPLYLEAVE,             //申请退出
        CCMsg_CChess_SM_BACKLEAVE,              //回复给离开的人
        CCMsg_CChess_SM_ENTERTRUSTSTATE,        //通知客户端进入托管状态
        CCMsg_CChess_CM_CANCLETRUSTSTATE,       //玩家取消托管状态
        CCMsg_CChess_SM_MIDDLEENTERROOM,        //玩家中途加入
        CCMsg_CChess_SM_AFTERONLOOKERENTER,     //围观的玩家进入房间
        CCMsg_CChess_CM_LEAVEONLOOKERROOM,		//玩家离开围观的房间

        //俱乐部消息的开始
        CrazyCityMsg_CLUB_BEGIN = 200000,
    }

    //车行游戏的子消息
    enum CarportMsg_enum
    {
        CarportMsg_Begin = EMSG_ENUM.CrazyCityMsg_CARPORT_BEGIN,
        CarportMsg_CM_LOGIN,                            //客户端登陆
        CarportMsg_SM_LOGIN,                            //服务器返回
        CarportMsg_CM_CHOOSELEVEL,                      //选择等级
        CarportMsg_SM_CHOOSELEVEL,                      //回复客户端
        CarportMsg_CM_CHIPCAR,                          //下注
        CarportMsg_SM_CHIPCAR,                          //回复下注
        CarportMsg_CM_APPLYBOSS,                        //玩家申请坐庄
        CarportMsg_SM_APPLYBOSS,                        //回复玩家申请坐庄
        CarportMsg_SM_BOSSCHANGE,                       //坐庄玩家改变
        CarportMsg_SM_GAMESULET,                        //结果
        CarportMsg_SM_SORTDATA,                         //排序结果
        CarportMsg_CM_CANCLEAPPLYBOSS,                  //取消申请坐庄
        CarportMsg_SM_CANCLEAPPLYBOSS,                  //回复
        CarportMsg_CM_APPLYDOWNBOSS,                    //坐庄的人申请下庄
        CarportMsg_SM_APPLYDOWNBOSS,                    //回复
        CarportMsg_CM_CANCLEAPPLYDOWNBOSS,              //玩家取消申请下庄
        CarportMsg_SM_CANCLEAPPLYDOWNBOSS,              //回复
        CarportMsg_CM_APPLYBOSSLIST,                    //玩家申请坐庄的列表
        CarportMsg_SM_APPLYBOSSLIST,                    //回复
        CarportMsg_SM_CURTABLEDATATONEWIN,              //发送当前牌桌的数据给新来的
        CarportMsg_SM_HISTROYCARDATA,                   //历史数据
        CarportMsg_SM_BEGINCHIP,                        //开始下注
        CarportMsg_SM_KNCKOUTROOM,                      //T出房间
        CarportMsg_CM_LEAVEROOM,                        //玩家离开房间
        CarportMsg_SM_LEAVEROOM,

        CarportMsg_Max

    }

    // 游戏状态的枚举
    public enum CarPortGameState_enum
    {
        CarPortState_Init = 0,

        CarPortState_WaitBoss = 1,      //等待别人坐庄
        CarPortState_ChipIn = 2,        //下注状态
        CarPortState_Roulette = 3,      //轮盘旋转状态
        CarPortState_OverWait = 4,      //一局结束需要等待一会儿

        CarPortState_Max
    }

    // 俱乐部2级消息
    public enum ClubSecondMsg
    {
        ClubSecondInit = EMSG_ENUM.CrazyCityMsg_CLUB_BEGIN,

        CM_ClubScondCreate,                 //创建俱乐部
        SM_ClubScondCreate,
        CM_ClubSecondExpel,                 //踢出
        SM_ClubSecondExpel,
        CM_ClubSecondJoin,                      //加入俱乐部
        SM_ClubSecondJoin,
        CM_ClubSecondLevelUp,                   //俱乐部升级
        SM_ClubSecondLevelUp,
        CM_ClubSecondGive,                      //赠送
        SM_ClubSecondGive,
        CM_ClubSecondBindingPhone,              //绑定手机
        SM_ClubSecondBindingPhone,
        CM_ClubSecondCheckPhone,                //验证手机
        SM_ClubSecondCheckPhone,
        CM_ClubSecondChangeJoinCondition,       //修改加入限制
        SM_ClubSecondChangeJoinCondition,
        CM_ClubSecondExpelOneKey,               //一键踢出
        SM_ClubSecondExpelOneKey,
        CM_ClubSecondJoinOneKey,                //一键加入
        SM_ClubSecondJoinOneKey,
        CM_ClubSecondInfo,                  //俱乐部基本信息
        SM_ClubSecondInfo,
        CM_ClubBreak,                           //解散俱乐部
        SM_ClubBreak,
        CM_ExitClub,                            //退出俱乐部
        SM_ExitClub,
        CM_ClubSearch,                      //搜索
        SM_ClubSearch,
        CM_ClubChangeContent,                   //修改工会宣言
        SM_ClubChangeContent,
        CM_ClubChangeActive,                    //修改工会活跃度
        SM_ClubChangeActive,
        CM_ClubChangeStep,                  //修改工会等级
        SM_ClubChangeStep,
        CM_ClubMemberLoginOrExit,               //成员上下线
        SM_ClubMemberLoginOrExit,
        CM_ClubMemberChangeNameOrIcon,      //成员修改名称和头像	
        SM_ClubMemberChangeNameOrIcon,
        CM_ClubCreateRefreshList,               //创建俱乐部通知其他玩家
        SM_ClubCreateRefreshList,
        CM_ClubBreakRefreshList,                //解散俱乐部通知其他玩家
        SM_ClubBreakRefreshList,

        ClubSecondMax
    }

    //丢丢乐游戏的子消息
    public enum Diudiule_enum
    {
        DiudiuleMsg_Begin = EMSG_ENUM.CrazyCityMsg_DIUDIULE_BEGIN,
        DiudiuleMsg_CM_LOGIN,                           //客户端登陆
        DiudiuleMsg_SM_LOGIN,                           //服务器返回
        DiudiuleMsg_CM_GAMELIMIT,                       //请求
        DiudiuleMsg_SM_GAMELIMIT,                       //
        DiudiuleMsg_CM_REGION,                          //进入匹配
        DiudiuleMsg_SM_REGION,
        DiudiuleMsg_SM_SITDOWN,                         //匹配成功
        DiudiuleMsg_SM_GAMESTART,                       //游戏开始
        DiudiuleMsg_CM_CANCLEREGION,                    //取消匹配
        DiudiuleMsg_SM_CANCLEREGION,
        DiudiuleMsg_CM_ACTION,                          //玩家出牌
        DiudiuleMsg_SM_ACTION,
        DiudiuleMsg_CM_GAMEEND,                         //
        DiudiuleMsg_SM_GAMEEND,                         //游戏结束
        DiudiuleMsg_CM_GAMEAUTO,                        //玩家取消托管
        DiudiuleMsg_SM_GAMEAUTO,                        //是否托管
        DiudiuleMsg_SM_GAMESCENE,                       //
        DiudiuleMsg_SM_RECONNECT,                       //服务器问客户端是否重连
        DiudiuleMsg_CM_RECONNECT,                       //客户端回复服务器
        DiudiuleMsg_CM_EMOTION,                         //表情
        DiudiuleMsg_SM_EMOTION,
        DiudiuleMsg_CM_GAMESCENE,                       // 客户端请求重连
        DiudiuleMsg_SM_NOGAMESCENE,                     // 该玩家不在房间里 不需要同步

        DiudiuleMsg_Max

    }

    //拉霸2级消息
    public enum SlotSecondMsg
    {
        LabaMsg_Begin = EMSG_ENUM.CrazyCityMsg_LABA_BEGIN,
        LabaMsg_CM_LOGIN,                           //客户端登陆
        LabaMsg_SM_LOGIN,                           //服务器返回
        LabaMsg_CM_CHOOSElEVEL,                     //选择等级进入
        LabaMsg_SM_CHOOSElEVEL,
        LabaMsg_CM_GAMESTATE,                       //请求游戏开始
        LabaMsg_SM_GAMESTATE,
        LabaMsg_CM_DOUBLEREWORD,                    //请求双倍奖励
        LabaMsg_SM_DOUBLEREWORD,                    //回复
        LabaMsg_CM_DRAWREWORD,                      //请求抽牌
        LabaMsg_SM_DRAWREWORD,                      //回复
        LabaMsg_SM_KICKOUTROOM,                     //钱不够T出去
        LabaMsg_CM_LEAVEROOM,
        LabaMsg_SM_LEAVEROOM,

        Laba_enum_Max
    }

    //森林舞会的子消息
    enum Forest_enum
    {
        ForestMsg_Begin = EMSG_ENUM.CrazyCityMsg_FOREST_BEGIN,
        ForestMsg_CM_LOGIN,                         //客户端登陆
        ForestMsg_SM_LOGIN,                         //服务器返回
        ForestMsg_CM_CHOOSElEVEL,                   //选择等级进入
        ForestMsg_SM_CHOOSElEVEL,
        ForestMsg_SM_GAMEBEGIN,                     //服务器告诉客户端 随机ID是多少
        ForestMsg_SM_GAMERESULT,                    //服务器告诉客户端结果
        ForestMsg_CM_CHIPIN,                        //下注
        ForestMsg_SM_CHIPIN,
        ForestMsg_SM_GAMECOUNT,                     //服务器把玩家增减告诉客户端
        ForestMsg_SM_ROOMDATATONEWJOIN,             //发送房间数据给新加入的玩家
        ForestMsg_SM_ROOMHISTROYTOJOIN,             //发送房间历史数据给玩家
        ForestMsg_CM_LEAVEROOM,                     //玩家离开房间
        ForestMsg_SM_LEAVEROOM,                     //返回

        Forest_enum_Max
    };

    enum ForestMode_Enum
    {
        ForestMode_Init = 0,
        //正常 单颜色单动物方才获奖
        ForestMode_Normal,
        //大三元 该动物的所有颜色均可获奖
        ForestMode_Three,
        //大四喜 该颜色的所有动物均可获奖
        ForestMode_Four,
        //送枪 该模式下，系统会随机开奖多次，
        //每次开奖为单颜色单动物，也就是普通开奖多次
        ForestMode_GiveGun,
        // 该模式下，系统首先会随机一个倍率（2-4），
        //然后开单颜色和单动物，获奖的倍率在原有倍率的基础上*随机的倍率，对庄和闲无效
        ForestMode_Flash,
        //彩金模式 该模式下，系统会单开动物和颜色，
        //所有压中的玩家在获得基础的奖励之外，押注额最高的10个玩家
        //（如果有并列第十，则取所有的第十的玩家）还会均分额外的40%的彩金池奖励。
        //由于开奖效果上，有部分玩家会未获得奖励，二者的表现不同，
        //因此服务器开奖时需要告知客户端，哪些玩家获得了彩金，从而有不同的表现效果。
        ForestMode_Handsel,
        //系统赢钱模式
        ForestMode_SystemWin,

        ForestMode_Max,
    };

    //五子棋的时候 房间的状态
    enum FiveRoomState_Enum
    {
        FiveRoomState_Init = 0,
        FiveRoomState_WaitOther,    //房主创建完 等待其他人加入
        FiveRoomState_Ready,        //等待双方准备
        FiveRoomState_GameOn,       //游戏开始
        FiveRoomState_GameOver,     //游戏结束

        FiveRoomState_End
    };

    //玩家坐的方位
    enum PlayerSitSide_Enum
    {
        PlayerSitSide_Init = 0,
        PlayerSitSide_Left,
        PlayerSitSide_Right,
        PlayerSitSide_Middle,

        PlayerSitSide_End
    };

    //棋子的
    public enum ChessSign_Enum
    {
        ChessSign_Init = 0,
        ChessSign_White,
        ChessSign_Black,

        ChessSign_End
    };

    //德州牌型
    public enum TexasPokerType_Enum
    {
        TexasPokerType_Init = 0,
        TexasPokerType_High,                //5张单牌
        TexasPokerType_Two,                 //有一个对子外加三张单牌
        TexasPokerType_TwoPairs,            //有两个个对子
        TexasPokerType_Three,               //三条3张点数相同的牌+2张单牌
        TexasPokerType_Straight,            //花色不同的顺子
        TexasPokerType_SameColor,           //同花
        TexasPokerType_Gourd,               //葫芦：3张点数相同的牌+2张点数相同的牌
        TexasPokerType_Four,                //四条：4张点数相同的牌+1张单牌
        TexasPokerType_Flush,               //普通同花顺
        TexasPokerType_RoyalFlush,          //皇家同花顺

        TexasPokerType
    };

    //德州房间状态
    public enum TexasRoomState_Enum
    {
        TexasRoomState_Init = 0,

        TexasRoomState_WaitPlayerSit,       //等待玩家坐下
        TexasRoomState_CountDownBegin,      //游戏开始倒计时
        TexasRoomState_CutBlinds,           //扣掉大小盲注
        TexasRoomState_GiveTwoPoke,         //给每个玩家两张牌
        TexasRoomState_AskFirstChip,        //第一次询问每个人 下注
        TexasRoomState_DealPublic_First,    //发三张公共的牌 1
        TexasRoomState_AskSecondChip,       //第二次询问每个人 下注
        TexasRoomState_DealPublic_Second,   //发一张公共的牌 2
        TexasRoomState_AskThirdChip,        //第三次询问每个人 下注
        TexasRoomState_DealPublic_Third,    //发一张公共的牌 3
        TexasRoomState_AskFourthChip,       //第四次询问每个人 下注

        TexasRoomState_Resulet,             //游戏结果
        TexasRoomState_Over,                //最后结束等待

        TexasRoomState_Max
    };

    //德州房间等级
    public enum TexasRoomLevel_Enum
    {
        TexasRoomLevel_Init = 0,
        TexasRoomLevel_1,
        TexasRoomLevel_2,
        TexasRoomLevel_3,

        TexasRoomLevel_Max
    };

    //5堆牛牛房间状态
    public enum AllKillRoomState_Enum
    {
        AllKillRoomState_Init = 0,

        AllKillRoomState_WaitBoss,
        AllKillRoomState_DealPokers,        //发牌
        AllKillRoomState_ChipIn,            //下注
        AllKillRoomState_OpenPokers,        //开牌
        AllKillRoomState_GameWait,          //给玩家看

        AllKillRoomState_Result,
        AllKillRoomState_End,

        AllKillRoomState
    };

    //5堆牛牛排队序号
    enum AllKillDeskSign_Enum
    {
        AllKillDeskSign_Boss = 0,
        AllKillDeskSign_1,
        AllKillDeskSign_2,
        AllKillDeskSign_3,
        AllKillDeskSign_4,

        AllKillDeskSign
    };

    enum AllKillSign_Enum
    {
        AllKillSign_Zero = 0,       //没牛
        AllKillSign_One = 1,        //牛一
        AllKillSign_Two = 2,        //牛二
        AllKillSign_Three = 3,      //牛三
        AllKillSign_Four = 4,       //牛四
        AllKillSign_Five = 5,       //牛五
        AllKillSign_Six = 6,        //牛六
        AllKillSign_Seven = 7,      //牛七
        AllKillSign_Eight = 8,      //牛八
        AllKillSign_Nine = 9,       //牛九
        AllKillSign_Ten = 10,   //牛牛
        AllKillSign_RedWin = 11,    //红的胜
        AllKillSign_Tied = 12,  //平局
        AllKillSign_BlueWin = 13,   //蓝的胜
        AllKillSign_Double = 14,    //双牛
        AllKillSign_Max = 15,   //五小 五花 四炸 四花

        AllKillSign
    };

    enum AllKillMaxPokerType_Enum
    {
        AllKillMaxPokerType_None = 15,
        AllKillMaxPokerType_FourFlower, //四花
        AllKillMaxPokerType_FourBlust,  //四炸
        AllKillMaxPokerType_FiveFlower, //五花
        AllKillMaxPokerType_FiveSmall,  //五小

        AllKillMaxPokerType
    };

    //新手礼包的枚举
    enum NewHand_Gift
    {
        NewHand_ThreeOne,       //三合一
        NewHand_6,              //六元
        NewHand_18,             //十八元
        NewHand_30,             //三十元

        NewHand_Max
    };

    //特惠礼包的枚举
    enum Benefit_Gift
    {
        Benefit_68,             //68元礼包
        Benefit_128,
        Benefit_198,
        Benefit_328,
        Benefit_648,
        Benefit_1000,

        Benefit_Max
    };

    //商店物品类型
    enum newItemType
    {
        newItemType_None = 0,
        newItemType_Coin = 1,         //金币
        newItemType_Diamond = 2,      //钻石
        newItemType_JingDong = 3,     //京东购物卡
        newItemType_Mobile = 4,       //手机充值卡
        newItemType_NewHandGift = 5,  //新手礼包
        newItemType_BenefitGift = 6,  //特惠礼包
    };

    //活动处于什么状态
    enum ActivityState_Enum
    {
        ActivityState_Init,
        ActivityState_Wait,         //等待开始
        ActivityState_In,           //活动中
        ActivityState_Over,         //活动结束

        ActivityState
    };

    //活动类型的枚举
    enum ActivityType_Enum
    {
        ActivityType_Init,
        ActivityType_RedBag,            //发红包

        ActivityType
    };

    enum BullHappyGameState_Enum
    {
        BullHappyGameState_Init,

        BullHappyRoomState_WaitOtherPlayer, //等待其他玩家中
        BullHappyRoomState_Count,           //开始等待倒计时
        BullHappyRoomState_DealPokers,      //发牌
        BullHappyRoomState_DealBoss,        //抢庄
        BullHappyRoomState_ChipIn,          //下注
        BullHappyRoomState_Caculate,        //算牛
        BullHappyRoomState_OpenPokers,      //开牌
        BullHappyRoomState_GameWait,        //给玩家看

        BullHappyRoomState_Result,
        BullHappyRoomState_End,

        BullHappyGameState
    };
}