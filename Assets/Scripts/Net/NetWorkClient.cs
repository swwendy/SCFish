﻿using UnityEngine;
    {
        if (string.IsNullOrEmpty(ip) || port == 0)
            return false;
        if (strConnectIp.CompareTo(ip) == 0 && iConnectPort == port)
            return true;
        return false;
    }
    {
        if (uNetSocket == null)
            return false;
        return uNetSocket.CheckSocketConnected();
    }

    public static NetWorkClient GetInstance()
            return false;

        if (uNetSocket == null)

        if (IsConnectServer && IsSameIpAndPort(_serverIp,_serverPort))
            return true;

        IsConnectServer = uNetSocket.ConnectServer(_serverIp, _serverPort);
        if (IsConnectServer)
        {
            iConnectPort = _serverPort;
        }
        return IsConnectServer;
    {
        if (uNetSocket == null)
        {
            uNetSocket = new UnitySocket();         
        }
        IsConnectServer = uNetSocket.ConnectServer(strConnectIp, iConnectPort);      
        return IsConnectServer;
    }
        IsConnectServer = false;
    {
        if (uNetSocket == null || !IsSocketConnected)
        return uNetSocket.GetMessageCount();
    }