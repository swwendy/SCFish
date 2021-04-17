﻿using System.Collections.Generic;


/// <summary>
public class TipsData
    public string TipsTitle;               //提示标题
    public string TipsText;                //提示内容

};


/// <summary>

    public CTipsDataMgr()
        TipsDataDictionary = new Dictionary<uint, TipsData>();

    /// <summary>

            tipsdata.TipsTitle = strList[i][1];
            tipsdata.TipsText = strList[i][2];


            TipsDataDictionary.Add(tipsdata.TipsID, tipsdata);

    }

    /// <summary>
    {
        if(TipsDataDictionary.ContainsKey(tipsId))
        {
            return string.Format(TipsDataDictionary[tipsId].TipsText, param);
        }
        return null;
    }