using System.Collections.Generic;using UnityEngine;using USocket.Messages;using System.IO;using XLua;[LuaCallCSharp]public class FishingCannonData{    public string m_szCannon;    public string m_szBullet;    public string m_szNet;    public float m_fBulletSpeed;    public float m_fBulletCD;    public float m_fBulletLifeTime;    public byte m_nBounceTimes;    //public float m_fExplosinRange;    //public int m_nBulletCost;    public string[] m_szBulletPoint;    public string m_szLvPoint;    //public string m_szIcon;    public int m_nAudio;    public string m_szName;    public string m_szDetail;}[LuaCallCSharp]public class FishingPathData{    public string m_szPath;    public float m_fSpeed;    public float m_fTime;    public bool m_bLoop;    public Dictionary<byte, float> m_ChangePoints = new Dictionary<byte, float>();    public List<Vector3> m_Offsets = new List<Vector3>();}[LuaCallCSharp]public enum FishType
{
    eFT_None,
    eFT_Normal,
    eFT_Special,

    eFT_Num
}[LuaCallCSharp]public class FishingFishData{    public string m_szFish;    public Dictionary<byte, float> m_GroupSpeed = new Dictionary<byte, float>();    //public float m_fHitRate;    public int m_nMultiple;    public int m_nHitAudio;    public int m_nDeadAudio;    public byte m_nTalkInterval = 0;    public byte m_nTalkShowTime = 0;    public byte m_nRotType = 0;    public FishType m_eFishType = FishType.eFT_None;    public string m_szTalk = "";    public string m_szName;    public string m_szIcon;}

[LuaCallCSharp]public class FishingRoomData{    public int m_nMinInCoin;    public int m_nMaxInCoin;}
[Hotfix]public class Fishing_Data{    Fishing_Data()    {        ReadFishingData();    }    public static Fishing_Data GetInstance()    {        if (instance == null)            instance = new Fishing_Data();        return instance;    }    static Fishing_Data instance;    public Dictionary<byte, FishingCannonData> m_CannonData = new Dictionary<byte, FishingCannonData>();    public Dictionary<ushort, FishingPathData> m_PathData = new Dictionary<ushort, FishingPathData>();    public Dictionary<byte, FishingFishData> m_FishData = new Dictionary<byte, FishingFishData>();    public List<FishingRoomData> m_RoomData = new List<FishingRoomData>();    public void ReadData(UMessage msg)    {
        m_RoomData.Clear();

        byte levelNum = msg.ReadByte();        for (int i = 0; i < levelNum; i++)        {            FishingRoomData data = new FishingRoomData();            data.m_nMinInCoin = msg.ReadInt();            data.m_nMaxInCoin = msg.ReadInt();            m_RoomData.Add(data);        }    }    void ReadFishingData()    {        List<string[]> strList;        int j;        byte id;        ushort pathId;        byte temp;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, "Fishing_Cannon.txt", out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            j = 0;            FishingCannonData gamedata = new FishingCannonData();            byte.TryParse(strList[i][j++], out id);            gamedata.m_szCannon = strList[i][j++];            gamedata.m_szBullet = strList[i][j++];            float.TryParse(strList[i][j++], out gamedata.m_fBulletSpeed);            float.TryParse(strList[i][j++], out gamedata.m_fBulletCD);            float.TryParse(strList[i][j++], out gamedata.m_fBulletLifeTime);            byte.TryParse(strList[i][j++], out gamedata.m_nBounceTimes);
            //float.TryParse(strList[i][j++], out gamedata.m_fExplosinRange);
            j++;            gamedata.m_szNet = strList[i][j++];
            //int.TryParse(strList[i][j++], out gamedata.m_nBulletCost);
            j++;            //level not read
            j++;            gamedata.m_szBulletPoint = strList[i][j++].Split('|');             gamedata.m_szLvPoint = strList[i][j++];
            //gamedata.m_szIcon = strList[i][j++];
            j++;            int.TryParse(strList[i][j++], out gamedata.m_nAudio);            j += 2;            gamedata.m_szName = strList[i][j++];            gamedata.m_szDetail = strList[i][j++];            m_CannonData[id] = gamedata;        }        strList.Clear();        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, "Fishing_Path.txt", out strList);        columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            j = 0;            FishingPathData gamedata = new FishingPathData();            ushort.TryParse(strList[i][j++], out pathId);            gamedata.m_szPath = strList[i][j++];            float.TryParse(strList[i][j++], out gamedata.m_fSpeed);            float.TryParse(strList[i][j++], out gamedata.m_fTime);            byte.TryParse(strList[i][j++], out temp);            gamedata.m_bLoop = temp == 0 ? false : true;            string str = strList[i][j++];            if (!string.IsNullOrEmpty(str))            {
                string[] offsets = str.Split('@');
                string[] point;
                foreach (string oft in offsets)
                {
                    point = oft.Split('|');
                    Debug.Assert(point.Length == 3, "offset point coordinate wrong(not 3)!!");
                    Vector3 pos = new Vector3();
                    float.TryParse(point[0], out pos.x);
                    float.TryParse(point[1], out pos.y);
                    float.TryParse(point[2], out pos.z);
                    gamedata.m_Offsets.Add(pos);
                }
            }
            str = strList[i][j++];
            if (!string.IsNullOrEmpty(str))            {
                string[] speedChange = str.Split('@');
                string[] info;
                foreach (string sc in speedChange)
                {
                    info = sc.Split('|');
                    Debug.Assert(info.Length == 3, "point change speed info is wrong(not 3)!!");
                    byte pointIndex, endIndex;
                    float speed, time;
                    byte.TryParse(info[0], out pointIndex);
                    float.TryParse(info[1], out speed);
                    if (speed == 0f)//pause
                    {                        float.TryParse(info[2], out time);
                        gamedata.m_ChangePoints[pointIndex] = -time;
                    }                    else//change speed                    {
                        byte.TryParse(info[2], out endIndex);                        gamedata.m_ChangePoints[pointIndex] = speed;
                        gamedata.m_ChangePoints[endIndex] = 1f;//change back
                    }                }
            }            m_PathData[pathId] = gamedata;        }        strList.Clear();        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, "Fishing_Fish.txt", out strList);        columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            j = 0;            FishingFishData gamedata = new FishingFishData();            byte.TryParse(strList[i][j++], out id);            gamedata.m_szFish = strList[i][j++];

            string[] groupspeed = strList[i][j++].Split('@');
            string[] speedStr;
            foreach (string speed in groupspeed)
            {
                speedStr = speed.Split('|');
                Debug.Assert(speedStr.Length == 2, "group speed data is wrong(not 2)!!");
                gamedata.m_GroupSpeed[byte.Parse(speedStr[0])] = float.Parse(speedStr[1]);
            }

            //float.TryParse(strList[i][j++], out gamedata.m_fHitRate);
            j++;            int.TryParse(strList[i][j++], out gamedata.m_nMultiple);            j += 5;            int.TryParse(strList[i][j++], out gamedata.m_nHitAudio);            int.TryParse(strList[i][j++], out gamedata.m_nDeadAudio);            string[] strs = strList[i][j++].Split('|');            if(strs.Length == 3)            {
                byte.TryParse(strs[0], out gamedata.m_nTalkInterval);                byte.TryParse(strs[1], out gamedata.m_nTalkShowTime);                gamedata.m_szTalk = strs[2];            }            j += 3;            byte.TryParse(strList[i][j++], out temp);            gamedata.m_eFishType = (FishType)temp;            gamedata.m_szName = strList[i][j++];            gamedata.m_szIcon = strList[i][j++];            byte.TryParse(strList[i][j++], out gamedata.m_nRotType);            m_FishData[id] = gamedata;        }
    }
}