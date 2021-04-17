using System.Collections.Generic;using UnityEngine;using USocket.Messages;using System.IO;using XLua;[LuaCallCSharp]public class LoginCondition{    public LoginCondition()    {        maxMoneys = new List<int>();        minMoneys = new List<int>();    }    public void ReadData(UMessage msg)    {        byte length = msg.ReadByte();        for (int index = 0; index < length; index++)        {            maxMoneys.Add(msg.ReadInt());            minMoneys.Add(msg.ReadInt());        }    }    public List<int> maxMoneys;    public List<int> minMoneys;}[LuaCallCSharp]public class ChipsData{    public ChipsData()    {        chipIndex = 0;        selfchip = 0;        totalchip = 0;    }    public int chipIndex;    public long selfchip;    public long totalchip;}public class ChooseLevel{    public byte levels;}[LuaCallCSharp]public class FD_DataCenter{    private FD_DataCenter()    {        condition = new LoginCondition();        levelInfo = new ChooseLevel();    }    public LoginCondition condition;    public ChooseLevel levelInfo;    public static FD_DataCenter GetInstance()    {        if (instance == null)            instance = new FD_DataCenter();        return instance;    }    static FD_DataCenter instance;}[Hotfix]public class FD_AudioDataManager{    private FD_AudioDataManager()    {        dataList = new Dictionary<int, FD_AudioData>();        chipList = new List<FD_ChipData>();        filename = "ForestConfig";        forestbases = new Dictionary<int, int>();    }    public void ReadAudioCsvData()    {        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.ForestDanceAudioFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            FD_AudioData data = new FD_AudioData();            int.TryParse(strList[i][0], out data.audioID);            data.audioName = strList[i][1];            dataList.Add(data.audioID, data);        }    }    public void ReadBetCsvData()
    {
        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, GameDefine.ForestDanceBetFileName, out strList);        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)        {            FD_ChipData data = new FD_ChipData();            uint.TryParse(strList[i][0], out data.minimum);            uint.TryParse(strList[i][1], out data.kickout);            uint.TryParse(strList[i][2], out data.gradeone);            uint.TryParse(strList[i][3], out data.gradetwo);            uint.TryParse(strList[i][4], out data.gradethree);            uint.TryParse(strList[i][5], out data.gradefour);            uint.TryParse(strList[i][6], out data.gradefive);            chipList.Add( data );        }
    }    public static FD_AudioDataManager GetInstance()    {        if (instance_ == null)        {            instance_ = new FD_AudioDataManager();            instance_.ReadAudioCsvData();            instance_.ReadBetCsvData();            instance_.ReadIniConfigData();        }        return instance_;    }    public void PlayAudio(int id, bool isSound = true)    {        GameData gamedata = CCsvDataManager.Instance.GameDataMgr.GetGameData((byte)GameKind_Enum.GameKind_ForestDance);        if (gamedata != null)        {            if (isSound)                AudioManager.Instance.PlaySound( gamedata.ResourceABName, dataList[id].audioName);            else                AudioManager.Instance.PlayBGMusic( gamedata.ResourceABName, dataList[id].audioName);        }        else            Debug.Log("音效资源" + dataList[id].audioName + "加载失败");    }    public void ReadIniConfigData()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.CsvAssetbundleName);
        if (bundle == null)
            return;

        TextAsset datatxt = bundle.LoadAsset(filename) as TextAsset;        if (datatxt == null)            return;        MemoryStream stream = new MemoryStream(datatxt.bytes);        StreamReader filereader = new StreamReader(stream);

        string content = filereader.ReadToEnd();

        INIParser ip = new INIParser();
        ip.OpenFromString(content);

        forestbases.Add(1, ip.ReadValue("level_1", "SingleMaxchipin", 0));
        forestbases.Add(2, ip.ReadValue("level_2", "SingleMaxchipin", 0));
        forestbases.Add(3, ip.ReadValue("level_3", "SingleMaxchipin", 0));

        ip.Close();
    }    private static FD_AudioDataManager instance_;    public Dictionary<int, FD_AudioData> dataList;    public List< FD_ChipData > chipList;    string filename;    public Dictionary<int, int> forestbases;}public class FD_AudioData{    public int audioID;    public string audioName;}public class  FD_ChipData
{
    public uint minimum;
    public uint kickout;
    public uint gradeone;
    public uint gradetwo;
    public uint gradethree;
    public uint gradefour;
    public uint gradefive;
}