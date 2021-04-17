﻿using System.Collections.Generic;using USocket.Messages;using XLua;public class BAK_ChipsData{    public BAK_ChipsData()    {        chipIndex = 0;        selfchip = 0;        totalchip = 0;    }    public byte chipIndex;    public long selfchip;    public long totalchip;}public class BAK_PokerType{    public int pokertypeNO;    public string pokertype;    public int rewardrate;}public class BAK_AudioData{    public int audioID;    public string audioName;}[Hotfix]public class BAK_DataCenter{    static BAK_DataCenter instance_;    public BAKLoginMsg blm;    public BAKEnterRoomMsg ber;    public BAKBossList bbl;    BAK_DataCenter()    {        InitBullAllKillChipsData();    }    void InitBullAllKillChipsData()    {        chipsdatas = new List<uint>();        pokertypes = new Dictionary<byte, BAK_PokerType>();        audiodatas = new Dictionary<int, BAK_AudioData>();        blm = new BAKLoginMsg();        ber = new BAKEnterRoomMsg();        bbl = new BAKBossList();        InitChipsData();        ReadPokerTypeConfig();        ReadBullAllKillAudioConfig();    }    void InitChipsData()    {        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.BullAllKillBetFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            uint chip1 = 0;            uint.TryParse(strList[i][0], out chip1);            //chipsdatas.Add(chip1);            uint chip2 = 0;            uint.TryParse(strList[i][1], out chip2);            //chipsdatas.Add(chip2);            uint chip3 = 0;            uint.TryParse(strList[i][2], out chip3);            chipsdatas.Add(chip3);            uint chip4 = 0;            uint.TryParse(strList[i][3], out chip4);            chipsdatas.Add(chip4);            uint chip5 = 0;            uint.TryParse(strList[i][4], out chip5);            chipsdatas.Add(chip5);            uint chip6 = 0;            uint.TryParse(strList[i][5], out chip6);            chipsdatas.Add(chip6);            uint chip7 = 0;            uint.TryParse(strList[i][6], out chip7);            chipsdatas.Add(chip7);            uint chip8 = 0;            uint.TryParse(strList[i][7], out chip8);            chipsdatas.Add(chip8);        }    }    void ReadPokerTypeConfig()    {        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.BullAllKillPokerTypeFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            BAK_PokerType pokertype = new BAK_PokerType();            byte key = 0;            byte.TryParse(strList[i][0], out key);            pokertype.pokertypeNO = key;            pokertype.pokertype = strList[i][1];            int.TryParse(strList[i][2],out pokertype.rewardrate);            pokertypes.Add(key, pokertype);        }    }    void ReadBullAllKillAudioConfig()    {        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.BullAllKillAudioFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            BAK_AudioData bad = new BAK_AudioData();            int.TryParse(strList[i][0], out bad.audioID);            bad.audioName = strList[i][1];            audiodatas.Add(bad.audioID, bad);        }            }    public static BAK_DataCenter Instance()    {        if (instance_ == null)            instance_ = new BAK_DataCenter();        return instance_;    }    public void ReadLevelConfigFromServer(UMessage msg)    {        blm.ReadConfig(msg);    }    public void ReadEnterRoomMsgFromServer(UMessage msg)    {        ber.ReadEnterRommMsg(msg);    }    public void ReadBossListInfoFromServer(UMessage msg)    {        bbl.ReadBossListData(msg);    }    public List<uint> chipsdatas;    public Dictionary<byte, BAK_PokerType> pokertypes;    public Dictionary<int, BAK_AudioData> audiodatas;}