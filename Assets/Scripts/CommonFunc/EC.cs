using System;
using System.Collections.Generic;

/// <summary>
/// 0~999在场景切换时也不会清除
/// 1000后的留给消息之间派发
/// 注：非销毁类型在游戏周期内只会注册一次
/// </summary>
public enum enECType : uint
{
    DONT_CLEAR = 0,
    DONT_SCENE_CHANGE, //切换到相关场景
    DONT_HERO_LOGIN, //玩家登陆
    DONT_PLAYER_LOGIN,//玩家登陆
    DONT_WEALTH_FOOD_CHANGE,//体力数量发生变化
    DONT_WEALTH_POWER_CHANGE,//能量数量发生变化
    MSG_MODULE = 1000,

    MSG_LEVEL_DATA_INIT, //关卡数据准备完成
    MSG_LEVEL_PRELOAD_INIT, //关卡预加载单元完成
    MSG_LEVEL_UNIT_INIT, //关卡单位创建完成（英雄和其他单位）
    MSG_LEVEL_UI_INIT, //关卡UI准备完成
    MSG_LEVEL_INIT_OVER, //关卡所有初始化完成(loading背景还没加载完)
    MSG_LEVEL_LOADING_OVER, //加载界面完成（关掉loading界面）
    MSG_LEVEL_BATTLE_START, //开始战斗(loading背景已经撤掉，UI，场景都已加载完成)

    MSG_QUALITY_CHANGE,//画质改变了

    MSG_LEVEL_INIT, //关卡数据初始化完毕
    MSG_HERO_INIT,//英雄初始化完毕
    MSG_HERO_HP_MP_ADJUST, //血量，斗气变动
    MSG_HERO_REBORN,//英雄复活
    MSG_HERO_REBORN_TIME,//英雄复活倒计时
    MSG_CAMERA_TRANS_OFFSET,//摄像机位置增减
    MSG_EM_BATTLE_DEF_POINT_UPDATE,//防御值点数变动
    MSG_EM_BATTLE_DEF_AREA,//定制关卡当前操作区域
    MSG_SKILL_UNIQUE, //发动无双技能
    MSG_SKILL_UNIQUE_OVER, //结束无双技能
    MSG_SKILL_UNIQUE_EXIT, //无双技能退出
    MSG_BATTLE_OVER,//战斗结束
    MSG_BATTLE_OVER_CLEAR,//战斗结束资源清除
    MSG_BATTLE_AI_FOLLOWER_TYPE, //战斗伙伴AI模式
    MSG_BATTLE_UPDATE_TIME,
    MSG_BATTLE_PAUSE_TIME,
    MSG_BATTLE_RUN_TIME,
    MSG_BATTLE_ZONETRIGGER_MONSTER_SINGLE_OVER,//ZoneTrigger单个怪物触发点数据初始化完成

    MSG_UNIT_DIE,     //单位死亡,在派发此事件之前会将单位从战场列表移除
    MSG_UNIT_RECYCLE, //单位回收

    MSG_UI_HERO_LEFT_TO_RIGHT,//英雄出战界面，左边拖到右边
    MSG_UI_FUNCUNLOCKVIEW_END,//玩法解锁界面关闭
    MSG_UI_TIP_ANIMSG_CLOSE,//动画提示框关闭
    MSG_UI_GUARD_CLICK,//守卫界面点击卡牌
    MSG_UI_GUARD_UNLOCK,//守卫界面解锁卡牌
    MSG_UI_LEVEL_INTERFACESHOW,//战场回来后，真正看到关卡界面
    MSG_UI_LEVEL_CLOSE,//副本关卡消失
    MSG_UI_LEVEL_CLICK,//副本闯关界面点击关卡
    MSG_UI_LEVEL_CHANGECHAPTER,//副本闯关界面切换章节
    MGS_UI_LEVEL_CHANGECONDITION,//关卡状况发生变化
    MSG_UI_WEALTH_CHANGE,//拥有的资源，如金币、钻石，发生变化        
    MSG_UI_FX_IMMEDIATELYSHOW,//抽奖、升星、解锁等界面内容立即显示完
    MSG_UI_FIGHTTING_CHANGE, //对战匹配数据变动
    MSG_UI_INVENTORY_CHANGE, //背包数据变动
    MSG_UI_LOGINWINDOW_SHOW, //登陆界面出现
    MSG_UI_CLOSE_SERVE,//关闭服务器选择窗口
    MSG_UI_LOBBY_REFRESHPOINT,//刷新主界面小红点
    MSG_UI_LOBBY_CANFIGHT,//对战功能开放
    MSG_UI_SHOW_PLAY_NOTICE,//显示玩法预报
    MSG_STOP_CAMERASHAKE,//停止颤镜
    MSG_RESTORE_CAMERASHAKE,//恢复颤镜
    MSG_HIDE_EFFECT, //隐藏所有特效（用在结算时把所有不应该显示的特效隐藏：如技能冷却）
    MSG_LEVEL_PROGRESS, //设置当前场景进度
    MSG_LEVEL_BOSS_HP,   //boss血量设置
    MSG_LEVEL_AUTO_OFF,  //关闭自动模式
    MSG_COUNTDOWN_START, //开启倒计时
    MSG_DRAG_INTERRUPT_RESET, //摇杆拖拽中断恢复
    MSG_UI_CHAPTER_STARREWARD_REC,//章节星级奖励领取后
    MSG_UI_UNLOCK_DIAMOND,//章节钻石奖励
    MSG_UI_DIAMOND_UP,//钻石增加
    MSG_UI_INVENTORY_ITEM_CLICK,//背包物品点击
    MSG_UI_HERO_TIP_CLOSE,//关闭英雄信息提示框
    MSG_UI_HERO_TIP_OPEN,//打开英雄信息提示框
    MSG_UI_MISSION_FRESH,//任务界面刷新
    MSG_UI_MISSION_ACPT,//任务界面领取
    MSG_UI_PVEMATCH_SUCCEE,//对战匹配成功
    MSG_UI_FIGHTHERO_CHANGE,
    MSG_UI_SETHERO_FAST,//一键上阵
    MSG_UI_CLICK_TIPS,//点击了任务窗口的图标
    MSG_UI_RESIGN_IN,//点击补签
    MSG_UI_SIGN_IN,//点击签到
    MSG_UI_CLICK_MAIL,//点击邮件 
    MSG_UI_HERO_EAT_EXP,//英雄已经吃经验

    MSG_UI_THREEPVE_FRESH,//3v3金币刷新
    MSG_UI_THREEPVE_BUY,//3v3次数购买

    MSG_MISSION_WIN,//关卡胜利
    MSG_MISSION_LOSE,//关卡失败
    MSG_LOADING_COMPELETE,//load条结束
    MSG_CAN_USE_SKILL_UNIQUE,//可以使用无双技能
    MSG_HIT_TRIGGER_GUIDE,//碰到某场景triggerzone 引导专用
    MSG_CLOSE_NEWSTATE_FXWINDOW,//关闭升星特效窗口事件//引导用
    MSG_TRIGGER_INIT_FINISH,//trigger初始化完毕
    MSG_SERVER_SELECTED,//切换服务器

    MSG_SOS_WIN,//SOS胜利
    MSG_SOS_LOSE,//SOS失败
    MSG_OBTAIN_NEW_ITEM,//获得物品

    MSG_UI_HOME_FIGHTTING_CHANGE,//排行榜用户信息改变
    MSG_UI_HOME_LIST,//打开排行榜

    MSG_MATCHPVP_MATCH_ENTER,//PVP关卡进入匹配
    MSG_MATCHPVP_MATCH_CANCEL,//PVP关卡取消匹配
    MSG_MATCHPVP_MATCH_SUCCESS,//PVP关卡匹配成功
    MSG_MATCHPVP_FIGHT_PLAYER_EXIT, //玩家中途退出

    MSG_HANGUP_REFRESHCARD,//挂机模式刷新任务卡片
    MSG_HANGUP_REFRESHHERO,//挂机模式刷新出战英雄
    MSG_HANGUP_REFRESHLIGHT,//挂机模式刷新高亮边框
    MSG_HANGUP_BALANCE,//挂机模式领取战利品

    MSG_FRIEND_SEARCHREFRESH,//搜索到玩家后更新搜索列表
    MSG_FRIEND_LIST_REFRESH,//刷新当前的好友列表
    MSG_FRIEND_INFOMSG_REFRESH,//刷新请求信息列表
    MSG_FRIEND_INFO_UPDATE,//更新一条好友信息
    MSG_FRIEND_CHAT_ADD,//在当前列表的信息中添加

    MSG_CHAT_NEWMSG,//内容框中显示新信息

    MSG_EXPEDITION_GETREWARD,//远征领取奖励
}

public class EC
{

    private static Dictionary<uint, List<Action<object>>> mParamDict = new Dictionary<uint, List<Action<object>>>();
    private static Dictionary<uint, List<Action>> mDict = new Dictionary<uint, List<Action>>();

    static bool HasPDictDefined(uint _key)
    {
        return mParamDict.ContainsKey(_key);
    }

    static bool HasDictDefined(uint _key)
    {
        return mDict.ContainsKey(_key);
    }

    public static void AddListener(enECType _ecType, Action<object> _callBack)
    {
        uint key = (uint)_ecType;
        if (!HasPDictDefined(key)) mParamDict.Add(key, new List<Action<object>>() { _callBack });
        else if (!IsDontDestroy(_ecType)) mParamDict[key].Add(_callBack);
    }

    public static void AddListener(enECType _ecType, Action _callBack)
    {
        uint key = (uint)_ecType;
        if (!HasDictDefined(key)) mDict.Add(key, new List<Action>() { _callBack });
        else if (!IsDontDestroy(_ecType)) mDict[key].Add(_callBack);
    }

    public static void RemoveListener(enECType _ecType, Action<object> _callBack)
    {
        uint key = (uint)_ecType;
        if (!HasPDictDefined(key)) return;
        List<Action<object>> listHandlers = mParamDict[key];
        for (int i = 0; i < listHandlers.Count; i++)
        {
            if (listHandlers[i] == _callBack)
            {
                listHandlers.RemoveAt(i);
                break;
            }
        }
    }

    public static void RemoveListener(enECType _ecType, Action _callBack)
    {
        uint key = (uint)_ecType;
        if (!HasDictDefined(key)) return;
        List<Action> listHandlers = mDict[key];
        for (int i = 0; i < listHandlers.Count; i++)
        {
            if (listHandlers[i] == _callBack)
            {
                listHandlers.RemoveAt(i);
                break;
            }
        }
    }

    public static void RemoveAllListeners(enECType _ecType)
    {
        uint key = (uint)_ecType;
        if (HasPDictDefined(key)) mParamDict.Remove(key);
        if (HasDictDefined(key)) mDict.Remove(key);
    }

    public static void Dispatch(enECType _ecType, object param = null)
    {
        uint key = (uint)_ecType;
        if (param == null && HasDictDefined(key))
        {
            List<Action> listHandlers = mDict[key];
            for (int i = 0; i < listHandlers.Count; i++)
            {
                listHandlers[i]();
            }
        }
        else if (HasPDictDefined(key))
        {
            List<Action<object>> listHandlers = mParamDict[key];
            for (int i = 0; i < listHandlers.Count; i++)
            {
                listHandlers[i](param);
            }
        }
    }

    public static void Clear()
    {
        uint[] dictArr = new uint[mParamDict.Count];
        mParamDict.Keys.CopyTo(dictArr, 0);
        for (int i = 0; i < dictArr.Length; i++)
        {
            if (!IsDontDestroy((enECType)dictArr[i])) mParamDict.Remove(dictArr[i]);
        }

        dictArr = new uint[mDict.Count];
        mDict.Keys.CopyTo(dictArr, 0);
        for (int i = 0; i < dictArr.Length; i++)
        {
            if (!IsDontDestroy((enECType)dictArr[i])) mDict.Remove(dictArr[i]);
        }
    }

    static bool IsDontDestroy(enECType _type)
    {
        return _type < enECType.MSG_MODULE;
    }

}


