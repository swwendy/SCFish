using UnityEngine;using USocket.Messages;using USocket.Net;using System;using System.Runtime.InteropServices;using System.Collections.Generic;using System.Text;using USocket.Cmd;using XLua;[LuaCallCSharp]public class NetWorkClient{	private UnitySocket uNetSocket;	private static NetWorkClient instance;    private bool IsConnectServer;    private string strConnectIp;    private int iConnectPort;        public bool IsSocketConnected    {        get        {            if (uNetSocket == null)                return false;            return uNetSocket.Connected();        }    }    public bool IsSameIpAndPort(string ip, int port)
    {
        if (string.IsNullOrEmpty(ip) || port == 0)
            return false;
        if (strConnectIp.CompareTo(ip) == 0 && iConnectPort == port)
            return true;
        return false;
    }    public bool CheckSocketConnected()
    {
        if (uNetSocket == null)
            return false;
        return uNetSocket.CheckSocketConnected();
    }

    public static NetWorkClient GetInstance()	{		if(instance == null)		{			instance = new NetWorkClient();		}		return instance;	}    public NetWorkClient()	{		uNetSocket = null;		IsConnectServer = false;        strConnectIp = null;        iConnectPort = 0;	}	public bool InitNetWork(string _serverIp,int _serverPort)	{        if (string.IsNullOrEmpty(_serverIp) || _serverPort == 0)
            return false;

        if (uNetSocket == null)		{		    uNetSocket = new UnitySocket();		   		}

        if (IsConnectServer && IsSameIpAndPort(_serverIp,_serverPort))
            return true;

        IsConnectServer = uNetSocket.ConnectServer(_serverIp, _serverPort);
        if (IsConnectServer)
        {            strConnectIp = _serverIp;
            iConnectPort = _serverPort;
        }
        return IsConnectServer;	}    public bool Reconnect()
    {
        if (uNetSocket == null)
        {
            uNetSocket = new UnitySocket();         
        }
        IsConnectServer = uNetSocket.ConnectServer(strConnectIp, iConnectPort);      
        return IsConnectServer;
    }	public bool SendMsg(UMessage _msg)	{		if(uNetSocket == null)			return false;		uNetSocket.Send(_msg);		return true;	}    public bool SendMsg(byte[] bytedata)    {        if (uNetSocket == null)            return false;        uNetSocket.Send(bytedata);        return true;    }	public void CloseNetwork()	{		if( uNetSocket != null)		{		    uNetSocket.close();					}        uNetSocket = null;
        IsConnectServer = false;        //strConnectIp = null;        //iConnectPort = 0;	}	public void Update()	{		ProcessPacket();	}    public int GetCacheMessageCount()
    {
        if (uNetSocket == null || !IsSocketConnected)            return 0;
        return uNetSocket.GetMessageCount();
    }	public void ProcessPacket()	{        if (uNetSocket== null || !IsSocketConnected)            return;        //先判断一下 防止浪费queue变量 拖慢gc        if (uNetSocket.GetMessageCount()>0)        {            Queue<IScsMessage> tmpQueue = uNetSocket.CopyMessage();            while (tmpQueue.Count > 0)            {                UMessage msg = (UMessage)tmpQueue.Dequeue();                if (msg is UMessage)                {                    CMsgDispatcher.GetInstance().MsgDispatch(msg.GetMsgType(), msg);                }                else                {                    Debug.LogError("message is Not a UMessage !");                }            }        }	}}