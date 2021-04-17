﻿using System.Collections;

    /// <summary>
    /// 如果是游戏资源 ，则是正在下载资源的游戏Id
    /// </summary>
    /// 下载完成的文件列表
    /// </summary>
    /// 初始变量值
    /// </summary>
    {
        IsDownLoadOver = false;
        FileLength = 999999999;
        CurDownloadGameId = gameid;
    }
    /// 增加下载文件数
    /// </summary>
    /// <param name="count"></param>
    /// 添加下载完成的文件名
    /// </summary>
    /// <param name="filename"></param>
    {
        DownloadOverFileNameList.Add(filename);
        DownloadingFileCount -= 1;
        IsDownLoadOver = DownloadingFileCount == 0;
    }
    /// 获取当前下载文件百分比
    /// </summary>
    /// <returns></returns>
    {
        downPercent = (uint)(((float)DownLength / FileLength)*100);
       
        return downPercent;
    }

    /// <summary>
    /// 获取当前下载文件进度0-1
    /// </summary>
    /// <returns></returns>
    {
        float downprocess =(float)DownLength / FileLength;
        return downprocess;
    }

    /// <summary>
    /// 增加当前下载文件的下载长度
    /// </summary>
    /// <param name="readlen"></param>
    {
        //Debug.Log("AddDownloadFileLength：" + readlen);
        DownLength += readlen;
    }
    /// 设置下载文件总长度
    /// </summary>
    /// <param name="length"></param>
    {
        //Debug.Log("SetDownloadFileLength：" + length);
        FileLength = length;
    }

    /// <summary>
    /// 获取文件当前已下载大小（单位:KB）
    /// </summary>
    {
        long totallength = DownLength / 1024;
        return totallength;
    }

    /// <summary>
    /// 获取下载的总大小（单位:KB）
    /// </summary>
    {
        long totallength = FileLength / 1024;
        return totallength;
    }

    