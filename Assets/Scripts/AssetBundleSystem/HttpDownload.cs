using System;
using System.Collections.Generic;
using System.IO;using System.Net;using UnityEngine;




/// <summary>  
/// 请求状态  
/// </summary>  
public struct RequestState
{
    /// <summary>  
    /// 缓冲区大小  
    /// </summary>  
    public int BUFFER_SIZE { get; set; }

    /// <summary>  
    /// 缓冲区  
    /// </summary>  
    public byte[] BufferRead { get; set; }

    /// <summary>  
    /// 保存路径  
    /// </summary>  
    public string SavePath { get; set; }

    /// <summary>  
    /// 请求流  
    /// </summary>  
    public HttpWebRequest Request { get; set; }

    /// <summary>  
    /// 响应流  
    /// </summary>  
    public HttpWebResponse Response { get; set; }

    /// <summary>  
    /// 流对象  
    /// </summary>  
    public Stream ResponseStream { get; set; }

    /// <summary>  
    /// 文件流  
    /// </summary>  
    public FileStream outStream { get; set; }

    /// <summary>
    /// 当前读取的数据长度
    /// </summary>
    public long ReadFileLength{ get; set; }

    /// <summary>
    /// 是否断点续传
    /// </summary>
    public bool bFileRangeRequest { get; set; }
}


/// <summary>/// http下载类/// </summary>public class HttpDownloadMgr{
    private struct DownloadFileInfo
    {
        public string DownURL;
        public string SavePath;
        public string FileName;
        public uint FileVer;
        public byte BelongGameId;

        public DownloadFileInfo(string url,string path,string name,uint ver,byte gameId)
        {
            DownURL = url;
            SavePath = path;
            FileName = name;
            FileVer = ver;
            BelongGameId = gameId;
        }
    };

    public static readonly HttpDownloadMgr Instance = new HttpDownloadMgr();    private static  RequestState httpreqstate;

    private  bool isDownloading; 

    private  List<DownloadFileInfo>  DownloadFileList;


    public bool bDownloadError;

    private long CurDownloadFileLength;

    private HttpDownloadMgr()    {        DownloadFileList = new List<DownloadFileInfo>();        isDownloading = false;        httpreqstate = new RequestState();        httpreqstate.BUFFER_SIZE = 1024 * 15;        httpreqstate.BufferRead = new byte[httpreqstate.BUFFER_SIZE];        bDownloadError = false;        CurDownloadFileLength = 0;    }    /// <summary>
    /// 请求下载文件
    /// </summary>
    /// <param name="downUrl"></param>
    /// <param name="savePath"></param>
    /// <param name="fileName"></param>    public  void RequestDownload(string downUrl, string savePath, string fileName,uint ver,byte resbelonggameid)
    {
        DownloadFileInfo downfile = new DownloadFileInfo(downUrl,savePath,fileName,ver, resbelonggameid);
        DownloadFileList.Add(downfile);
        DownLoadProcessMgr.Instance.AddDownFileCount(1);
        if (!isDownloading)
        {
            DownLoadProcessMgr.Instance.InitDownloadFileLength(downfile.BelongGameId);
            DownFileAsync(downfile);            
            isDownloading = true;
        }
       
    }    /// <summary>    /// 下载文件    /// </summary>    /// <param name="downUrl">下载地址</param>    /// <param name="savePath">保存路径</param>    /// <param name="fileName">文件名</param>    public static void DownFile(string downUrl,string savePath,string fileName)    {                httpreqstate.Request = (HttpWebRequest)WebRequest.Create(downUrl + fileName);
  
        try
        {           
            httpreqstate.Response =(HttpWebResponse) httpreqstate.Request.GetResponse();
            Debug.Log("下载文件" + downUrl + fileName + httpreqstate.Response.ContentLength);
            httpreqstate.ResponseStream = httpreqstate.Response.GetResponseStream();
            
           int readCount = httpreqstate.ResponseStream.Read(httpreqstate.BufferRead, 0, httpreqstate.BUFFER_SIZE);
            //outStream = new StreamWriter(savePath + fileName);
            httpreqstate.outStream = File.Create(savePath + fileName);
            long Totallength = httpreqstate.Response.ContentLength;

            long readlength = 0;
            //float CurrentPercent = 0f;

            while (readCount > 0)
            {
                readlength += readCount;
                //CurrentPercent =  (float)readlength / Totallength;
                //Debug.Log("Download Process:" + (int)(CurrentPercent *100) + "%");
               
                httpreqstate.outStream.Write(httpreqstate.BufferRead, 0, readCount);
                readCount = httpreqstate.ResponseStream.Read(httpreqstate.BufferRead, 0, httpreqstate.BUFFER_SIZE);

            }
            httpreqstate.Response.Close();
            httpreqstate.ResponseStream.Close();
            httpreqstate.outStream.Close();
        }        catch(WebException)
        {
            Debug.Log("HttpDownFile connect failed:" + downUrl + fileName);
        }
    }    private  void DownFileAsync(DownloadFileInfo fileInfo)
    {
        //创建一个初始化请求对象  
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fileInfo.DownURL + fileInfo.FileName);
        RequestState requestState = new RequestState();
        //设置下载相关参数  
        try
        {
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version11;          
            requestState.BUFFER_SIZE = 1024*64;
            requestState.BufferRead = new byte[requestState.BUFFER_SIZE];
            requestState.Request = request;
            requestState.SavePath = fileInfo.SavePath;
            requestState.bFileRangeRequest = false;
            //requestState.FileStream = new FileStream(requestState.SavePath, FileMode.OpenOrCreate);
            requestState.outStream = File.Create(fileInfo.SavePath + fileInfo.FileName);

            //开始异步请求资源  
            request.BeginGetResponse(new AsyncCallback(ResponseCallback), requestState);
        }
        catch(WebException)
        {
            requestState.Request.Abort();
            requestState.outStream.Close();
            DownloadError();
        }
    }


    /// <summary>  
    /// 请求资源方法的回调函数  
    /// </summary>  
    /// <param name="asyncResult">用于在回调函数当中传递操作状态</param>  
    private void ResponseCallback(IAsyncResult asyncResult)
    {
        RequestState requestState = (RequestState)asyncResult.AsyncState;
        try
        {          
            requestState.Response = (HttpWebResponse)requestState.Request.EndGetResponse(asyncResult);
            requestState.ResponseStream = requestState.Response.GetResponseStream();
            requestState.ResponseStream.ReadTimeout = 10000;
            //不是断点续传设置当前下载文件的总大小
            if(!requestState.bFileRangeRequest)
            {
                CurDownloadFileLength = requestState.Response.ContentLength;
                DownLoadProcessMgr.Instance.SetDownloadFileLength(CurDownloadFileLength);
            }
            bDownloadError = false;
            //开始异步读取流  
            requestState.ResponseStream.BeginRead(requestState.BufferRead, 0, requestState.BUFFER_SIZE, ReadStreamCallback, requestState);
        }
        catch (WebException )
        {
            //Debug.Log("Response WebException:" + ex.Message + ex.Status.ToString());
            requestState.Request.Abort();
            //requestState.Response.Close();
            //requestState.ResponseStream.Close();
            requestState.outStream.Close();
            DownloadError();
        }
        catch(Exception )
        {
            //Debug.Log("Response Exception:" + e.Message);
            DownloadError();
        }
    }

    /// <summary>  
    /// 异步读取流的回调函数  
    /// </summary>  
    /// <param name="asyncResult">用于在回调函数当中传递操作状态</param>  
    private void ReadStreamCallback(IAsyncResult asyncResult)
    {
        RequestState requestState = (RequestState)asyncResult.AsyncState;
        //Debug.Log("WebException:ReadStreamCallback");
        try
        {
            //Debug.Log("WebException: ready to EndRead");
            int read = requestState.ResponseStream.EndRead(asyncResult);
            //Debug.Log("WebException: EndRead");
            if (read > 0)
            {
                DownLoadProcessMgr.Instance.AddDownloadFileLength(read);
                //将缓冲区的数据写入该文件流  
                requestState.outStream.Write(requestState.BufferRead, 0, read);
                requestState.outStream.Flush();
                requestState.ReadFileLength += read;
                //开始异步读取流  
                //Debug.Log("WebException: ready to BeginRead");
                requestState.ResponseStream.BeginRead(requestState.BufferRead, 0, requestState.BUFFER_SIZE, ReadStreamCallback, requestState);
                //Debug.Log("WebException: BeginRead over");
            }
            else
            {
                requestState.Request.Abort();
                requestState.Response.Close();
                requestState.ResponseStream.Close();
                requestState.outStream.Close();
                if (requestState.Response.ContentLength == requestState.ReadFileLength)
                    DownloadEnd();
                else
                {
                    //File.Delete(requestState.SavePath);
                    DownloadError();
                }
            }
        }
        catch (WebException)
        {
            //Debug.Log("WebException:" + ex.Message + ex.Status.ToString());
            requestState.Request.Abort();
            requestState.Response.Close();
            requestState.ResponseStream.Close();
            requestState.outStream.Close();
            DownloadError();
        }
        catch(Exception)
        {
            //Debug.Log("ReadStreamCallback Exception:" + e.Message);
            requestState.Request.Abort();
            requestState.Response.Close();
            requestState.ResponseStream.Close();
            requestState.outStream.Close();
            DownloadError();
        }      
    }

    /// <summary>
    /// 下载出错尝试重连下载(断点续传)
    /// </summary>
    private void DownloadError()
    {
        //Debug.Log("WebException: DownloadError");
        bDownloadError = true;
        DownloadFileInfo loadfile = DownloadFileList[0];
        //创建一个初始化请求对象          
        RequestState requestState = new RequestState();
        //设置下载相关参数  
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(loadfile.DownURL + loadfile.FileName);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version11;
            requestState.BUFFER_SIZE = 1024 * 64;
            requestState.BufferRead = new byte[requestState.BUFFER_SIZE];
            requestState.Request = request;
            requestState.SavePath = loadfile.SavePath+loadfile.FileName;
            requestState.bFileRangeRequest = true;

            //requestState.FileStream = new FileStream(requestState.SavePath, FileMode.OpenOrCreate);
            long lReadFilePos = 0;

            if (!File.Exists(loadfile.SavePath + loadfile.FileName))
            {
                requestState.outStream = File.Create(loadfile.SavePath + loadfile.FileName);
                DownLoadProcessMgr.Instance.InitDownloadFileLength(loadfile.BelongGameId);
            }
            else
            {
                requestState.outStream = File.OpenWrite(loadfile.SavePath + loadfile.FileName);
                lReadFilePos = requestState.outStream.Length;
                requestState.outStream.Seek(lReadFilePos, SeekOrigin.Current);
            }
            if (lReadFilePos >0)
                requestState.Request.AddRange((int)lReadFilePos, (int)CurDownloadFileLength);

            //开始异步请求资源  
            request.BeginGetResponse(new AsyncCallback(ResponseCallback), requestState);
        }
        catch (Exception)
        {
            requestState.outStream.Close();
            DownloadError();
        }
    }

    /// <summary>
    /// 文件下载完了
    /// </summary>
    private void DownloadEnd(bool bError = false)
    {
        DownloadFileInfo loadfile = DownloadFileList[0];
        if (bError)
        {
            Debug.LogWarning("下载文件出错 filename:" + loadfile.FileName + " 版本:"+ loadfile.FileVer + " Url:" + loadfile.DownURL);
            return;
        }

        CCsvDataManager.Instance.LocalABVerDataMgr.SetLocalABVersion(loadfile.FileName, loadfile.FileVer);
        DownloadFileList.RemoveAt(0);
        int remainCount = DownloadFileList.Count;
        if (loadfile.BelongGameId != 255)
            CGameResDownloadMgr.Instance.DownloadOverGameResCallBack(loadfile.BelongGameId, loadfile.FileName);
        DownLoadProcessMgr.Instance.AddDownloadOverFileName(loadfile.FileName);
        //DownLoadProcessMgr.Instance.ReduceDownFileCount(1);
        if (remainCount == 0)
        {
            isDownloading = false;
        }
        else
        {
            DownFileAsync(DownloadFileList[0]);
            DownLoadProcessMgr.Instance.InitDownloadFileLength(DownloadFileList[0].BelongGameId);
        }
    }
}