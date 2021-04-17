﻿using System.Collections.Generic;
    {
        List<string[]> strList;
    }
    {
        AssetBundle bundle = AssetBundleManager.GetAssetBundle(GameDefine.CsvAssetbundleName);
        if (bundle == null)
            return;

        TextAsset datatxt = bundle.LoadAsset(filename) as TextAsset;

        string content = filereader.ReadToEnd();

        INIParser ip = new INIParser();
        ip.OpenFromString(content);

        forestbases.Add(1, ip.ReadValue("level_1", "SingleMaxchipin", 0));
        forestbases.Add(2, ip.ReadValue("level_2", "SingleMaxchipin", 0));
        forestbases.Add(3, ip.ReadValue("level_3", "SingleMaxchipin", 0));

        ip.Close();
    }
{
    public uint minimum;
    public uint kickout;
    public uint gradeone;
    public uint gradetwo;
    public uint gradethree;
    public uint gradefour;
    public uint gradefive;
}