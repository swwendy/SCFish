﻿using System.Collections.Generic;

/// <summary>

    /// <summary>


    /// <summary>
        //CReadCsvBase.ReaderCsvData(GameDefine.CsvAssetbundleName, GameDefine.AssetbundleCsvFileName, out strList);

        string path = GameDefine.AppPersistentDataPath + GameDefine.ServerAssetbundleVersionDataFile;

        //检测文件是否存在
        if (!File.Exists(path))

        List<string[]> strList;
        CReadCsvBase.ReaderCsvDataFromFile(path, out strList);