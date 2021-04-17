﻿using System.Collections;using System.Collections.Generic;using UnityEngine;public class TP_Pokers{    //所有的牌    public static byte[] STD_CARDS = new byte[]{        (byte)0x01,(byte)0x02,(byte)0x03,(byte)0x04,(byte)0x05,(byte)0x06,(byte)0x07,(byte)0x08,(byte)0x09,(byte)0x0A,(byte)0x0B,(byte)0x0C,(byte)0x0D,  //#方块   1-K        (byte)0x11,(byte)0x12,(byte)0x13,(byte)0x14,(byte)0x15,(byte)0x16,(byte)0x17,(byte)0x18,(byte)0x19,(byte)0x1A,(byte)0x1B,(byte)0x1C,(byte)0x1D,  //#梅花   1-K        (byte)0x21,(byte)0x22,(byte)0x23,(byte)0x24,(byte)0x25,(byte)0x26,(byte)0x27,(byte)0x28,(byte)0x29,(byte)0x2A,(byte)0x2B,(byte)0x2C,(byte)0x2D,  //#红桃   1-K        (byte)0x31,(byte)0x32,(byte)0x33,(byte)0x34,(byte)0x35,(byte)0x36,(byte)0x37,(byte)0x38,(byte)0x39,(byte)0x3A,(byte)0x3B,(byte)0x3C,(byte)0x3D,  //#黑桃   1-K        (byte)0x41,(byte)0x42                                                                                                                            //小王 大王    };}public class TP_Poker{    public byte point;    public byte color;}public class ChipModel{    public GameObject BuleChip;    public GameObject OrangeChip;    public GameObject RedChip;    public GameObject ChipsOnPlayer;    public GameObject ChiipsOnDesk;    public ChipModel()    {    }    void LoadChipModels()    {        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_TexasPoker);        if (gamedata != null)        {            AssetBundle bundle = AssetBundleManager.GetAssetBundle(gamedata.ResourceABName);            if (bundle == null)                return;            UnityEngine.Object obj1 = bundle.LoadAsset("Model_Chouma_1");            BuleChip = (GameObject)GameMain.instantiate(obj1);            UnityEngine.Object obj2 = bundle.LoadAsset("Model_Chouma_2");            OrangeChip = (GameObject)GameMain.instantiate(obj2);            UnityEngine.Object obj3 = bundle.LoadAsset("Model_Chouma_3");            RedChip = (GameObject)GameMain.instantiate(obj3);            UnityEngine.Object obj4 = bundle.LoadAsset("Model_Chouma_4");            ChipsOnPlayer = (GameObject)GameMain.instantiate(obj4);            UnityEngine.Object obj5 = bundle.LoadAsset("Model_Chouma_5");            ChiipsOnDesk = (GameObject)GameMain.instantiate(obj5);        }    }}