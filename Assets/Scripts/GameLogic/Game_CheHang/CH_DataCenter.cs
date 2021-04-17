﻿using System.Collections;

    public void LoadGameDataFile()
{
    public CH_BossCondition()
    {
        filename = "CarConfig";
        bossbases = new Dictionary<int, int>();
    }

    static public CH_BossCondition GetInstance()
    {
        if(instance_ == null)
        {
            instance_ = new CH_BossCondition();
            instance_.ReadIniConfig();
        }

        return instance_;
    }

    public void ReadIniConfig()
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.CsvAssetbundleName);
        if (bundle == null)
            return;

        TextAsset datatxt = bundle.LoadAsset(filename) as TextAsset;

        string content = filereader.ReadToEnd();

        INIParser ip = new INIParser();
        ip.OpenFromString(content);

        bossbases.Add(1, ip.ReadValue("level_1", "bossmoneybase", 0));
        bossbases.Add(2, ip.ReadValue("level_2", "bossmoneybase", 0));

        ip.Close();
    }

    public string filename;
    public Dictionary<int, int> bossbases;
    private static CH_BossCondition instance_;
}