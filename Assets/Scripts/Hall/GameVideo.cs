﻿using System;
                               
    //麻将
    VideoActionInfo_Mahjong = 200,
    VideoActionInfo_201,//掷骰子
    VideoActionInfo_202,//系统发牌
    VideoActionInfo_203,//系统通知换牌
    VideoActionInfo_204,//系统通知换牌完毕
    VideoActionInfo_205,//系统通知定缺
    VideoActionInfo_206,//系统通知定缺完毕
    VideoActionInfo_207,//系统发一张牌给玩家
    VideoActionInfo_208,//玩家出牌
    VideoActionInfo_209,//玩家碰杠
    VideoActionInfo_210,//别人不抢杠胡就下雨
    VideoActionInfo_211,//玩家胡
    VideoActionInfo_212,//玩家换牌
    VideoActionInfo_213,//补花

    //够级
    VideoActionInfo_Gouji = 300,
    VideoActionInfo_301,//发牌
    VideoActionInfo_302,//抗贡
    VideoActionInfo_303,//买3
    VideoActionInfo_304,//买4
    VideoActionInfo_305,//圈贡
    VideoActionInfo_306,//点贡
    VideoActionInfo_307,//烧贡
    VideoActionInfo_308,//闷贡
    VideoActionInfo_309,//落贡
    VideoActionInfo_310,//革命
    VideoActionInfo_311,//要头
    VideoActionInfo_312,//宣点
    VideoActionInfo_313,//系统通知出牌(烧牌结束)
    VideoActionInfo_314,//开始烧(反烧)牌
    VideoActionInfo_315,//转换出牌人
    VideoActionInfo_316,//玩家出牌
    VideoActionInfo_317,//让牌
    VideoActionInfo_318,//开点（宣点成功或失败）

    //象棋
    VideoActionInfo_CChess = 400,
    VideoActionInfo_401,//发棋
    VideoActionInfo_402,//走棋

    VideoActionInfo_end
    {
        OnEnd();

        m_CurData = null;

    {
        foreach(Transform child in PlayUITfm)
        {
            child.gameObject.SetActive(forceShow ? true : !child.gameObject.activeSelf);
        }
    }
        if (index == 0)
        if (index == 0)//pause
        m_fSpeed = speed;
                byte sit = _ms.ReadByte();
                {
                    break;
                }
            //tfm.FindChild("Button_share").GetComponent<Button>().onClick.AddListener(() => OnClickShare(videoId, temp));
            tfm.Find("Button_share").gameObject.SetActive(false);
        string key = videoId + ":" + index;
    {
        if (LobbyUITfm == null)
            return false;

        LobbyUITfm.gameObject.SetActive(show);
        return true;
    }