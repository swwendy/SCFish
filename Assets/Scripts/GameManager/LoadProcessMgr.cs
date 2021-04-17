using System.Collections;using System.Collections.Generic;using UnityEngine;using System;public class DownLoadProcessMgr{    public static readonly DownLoadProcessMgr Instance = new DownLoadProcessMgr();    /// <summary>    /// 是否已经下载完成    /// </summary>    public bool IsDownLoadOver;    /// <summary>    /// 文件总大小    /// </summary>    private long FileLength;    /// <summary>    /// 当前下载大小    /// </summary>    private long DownLength;    /// <summary>    /// 当前下载百分比    /// </summary>    private uint downPercent;    /// <summary>    /// 正在下载中的文件数量    /// </summary>    private int DownloadingFileCount;

    /// <summary>
    /// 如果是游戏资源 ，则是正在下载资源的游戏Id
    /// </summary>    public byte CurDownloadGameId;    /// <summary>
    /// 下载完成的文件列表
    /// </summary>    public List<string> DownloadOverFileNameList;    private DownLoadProcessMgr()    {        InitLoadProcess();    }	/// <summary>
    /// 初始变量值
    /// </summary>	public void InitLoadProcess()    {        IsDownLoadOver = false;        FileLength = 999999999;        DownLength = 0;        downPercent = 0;        DownloadingFileCount = 0;        DownloadOverFileNameList = new List<string>();        DownloadOverFileNameList.Clear();    }    public void InitDownloadFileLength(byte gameid)
    {
        IsDownLoadOver = false;
        FileLength = 999999999;        DownLength = 0;        downPercent = 0;
        CurDownloadGameId = gameid;
    }	    /// <summary>
    /// 增加下载文件数
    /// </summary>
    /// <param name="count"></param>    public  void AddDownFileCount(int count)    {        DownloadingFileCount += count;    }    /// <summary>
    /// 添加下载完成的文件名
    /// </summary>
    /// <param name="filename"></param>    public void AddDownloadOverFileName(string filename)
    {
        DownloadOverFileNameList.Add(filename);
        DownloadingFileCount -= 1;
        IsDownLoadOver = DownloadingFileCount == 0;
    }    /// <summary>
    /// 获取当前下载文件百分比
    /// </summary>
    /// <returns></returns>    public uint  GetDownloadPercent()
    {
        downPercent = (uint)(((float)DownLength / FileLength)*100);
       
        return downPercent;
    }

    /// <summary>
    /// 获取当前下载文件进度0-1
    /// </summary>
    /// <returns></returns>    public float GetDownloadProcess()
    {
        float downprocess =(float)DownLength / FileLength;
        return downprocess;
    }

    /// <summary>
    /// 增加当前下载文件的下载长度
    /// </summary>
    /// <param name="readlen"></param>    public void AddDownloadFileLength(int readlen)
    {
        //Debug.Log("AddDownloadFileLength：" + readlen);
        DownLength += readlen;
    }    /// <summary>
    /// 设置下载文件总长度
    /// </summary>
    /// <param name="length"></param>    public void SetDownloadFileLength(long length)
    {
        //Debug.Log("SetDownloadFileLength：" + length);
        FileLength = length;
    }

    /// <summary>
    /// 获取文件当前已下载大小（单位:KB）
    /// </summary>    public long GetDownloadFileCurLength()
    {
        long totallength = DownLength / 1024;
        return totallength;
    }

    /// <summary>
    /// 获取下载的总大小（单位:KB）
    /// </summary>    public long GetDownloadFileTotalLength()
    {
        long totallength = FileLength / 1024;
        return totallength;
    }

    }