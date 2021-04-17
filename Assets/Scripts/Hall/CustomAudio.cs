using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData
{
    public uint id;
    public string name;
}

public class CustomAudio
{
    Dictionary<uint, AudioData> audioconfig;
    static CustomAudio instance;

    CustomAudio()
    {
        audioconfig = new Dictionary<uint, AudioData>();
        LoadMatchAudioCsv();
    }

    public static CustomAudio GetInstance()
    {
        if (instance == null)
            instance = new CustomAudio();

        return instance;
    }

    void LoadMatchAudioCsv()
    {
        audioconfig.Clear();

        List<string[]> strList;        CReadCsvBase.ReaderCsvDataFromAB(GameDefine.CsvAssetbundleName, "MatchAudioCsv", out strList);

        int columnCount = strList.Count;        for (int i = 2; i < columnCount; i++)
        {
            AudioData data = new AudioData();

            uint.TryParse(strList[i][0], out data.id);
            data.name = strList[i][1];

            audioconfig.Add(data.id, data);
        }
    }

    public void PlayCustomAudio(uint id, bool isBgm = false)
    {
        if (!audioconfig.ContainsKey(id))
            return;

        if (isBgm)
            AudioManager.Instance.PlayBGMusic(GameDefine.HallAssetbundleName, audioconfig[id].name);
        else
            AudioManager.Instance.PlaySound(GameDefine.HallAssetbundleName, audioconfig[id].name);
    }
}
