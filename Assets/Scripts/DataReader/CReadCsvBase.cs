﻿using System;

//读取 csv
using UnityEngine;
    /// 从文件读取表
    /// </summary>
    /// <param name="filepathname"></param>
    /// <param name="strlist"></param>
    {
        strlist = new List<string[]>();
        if (!File.Exists(filepathname))
            return; 
        StreamReader sr = new StreamReader(filepathname);
        string strline = "";
 
        while (strline != null)
        {
            strline = sr.ReadLine();
            if (strline != null && strline.Length > 0)
        }
    }

    /// <summary>
        {
            return;
        }