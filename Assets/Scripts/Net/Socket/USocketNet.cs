namespace USocket.Net{    using System;    using System.IO;    using System.Net.Sockets;    using System.Net;    using System.Collections;    using System.Collections.Generic;    using System.Text;    using System.Threading;    using USocket.Public;    using USocket.Cmd;    using USocket.Protocols;    using USocket.Messages;    using UnityEngine;    using System.Runtime.InteropServices;    /// <summary>    ///  Net for unity    /// </summary>    public class UnitySocket    {#if UNITY_IOS        [DllImport("__Internal")]        private static extern string Ipv4ToIpv6(string SvrIp);#endif        public Socket mSocket;        //     private int nTimeout = 100;     // micorsecond        /// <summary>        /// per receive buffer        /// </summary>        private const int PerRecLen = 1024*500;        private byte[] m_byRecvBuff = new byte[PerRecLen];        //拆包剩余的数据(一次收到的未完整数据包才出现这情况)        private byte[] m_byRemainBuff = new byte[PerRecLen];        private int m_iRemainBuffSize;        private IScsWireProtocol WireProtocol;        private IScsWireProtocolFactory WireProtocolFactory { get; set; }        private Queue<IScsMessage> msgQueue { get; set; }        private ManualResetEvent TimeoutObject;        private bool IsConnectSuccess;        public UnitySocket()        {            mSocket = null;            TimeoutObject = new ManualResetEvent(false);            IsConnectSuccess = false;            m_iRemainBuffSize = 0;        }        private void GetIpNetFamilyType(string SerIp, out string newSerIp, out AddressFamily NetFamilyType)        {            NetFamilyType = AddressFamily.InterNetwork;            newSerIp = SerIp;#if UNITY_IOS            if (Application.platform == RuntimePlatform.WindowsEditor)                return;            try            {                string IPv6str = Ipv4ToIpv6(SerIp);                if (!string.IsNullOrEmpty(IPv6str))                {                    string[] m_StrTemp = IPv6str.Split('|');                    if (m_StrTemp != null && m_StrTemp.Length >= 2)                    {                        if (m_StrTemp[1].CompareTo("Ipv6") == 0)                        {                            newSerIp = m_StrTemp[0];                            NetFamilyType = AddressFamily.InterNetworkV6;                        }                    }                }            }            catch (Exception e)            {                Console.WriteLine("GetIPv6 error:" + e.StackTrace.ToString());            }#endif        }        public bool ConnectServer(string SvrIp, int SvrPort)        {            String newServIp = "";            AddressFamily newAddressFamily = AddressFamily.InterNetwork;            GetIpNetFamilyType(SvrIp, out newServIp, out newAddressFamily);            mSocket = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);            //mSocket.ReceiveTimeout = 500;            mSocket.SendTimeout = 500;            //设置KeepAlive            mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);            mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);            TimeoutObject.Reset();            IPAddress SvrIpAddr = IPAddress.Parse(newServIp);            mSocket.BeginConnect(SvrIpAddr, SvrPort, new AsyncCallback(ConnectCallBack), mSocket);            if (!TimeoutObject.WaitOne(500, false))            {                               mSocket.Close();                Debug.Log("Connect Server TimeOut......");            }            if (IsConnectSuccess)            {                //mSocket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);                WireProtocolFactory = new MyWireProtocolFactory();                WireProtocol = WireProtocolFactory.CreateWireProtocol();                WireProtocol.Reset();                msgQueue = new Queue<IScsMessage>();                if (mSocket.Connected)                {                    BeginRecieveData();                }            }            return mSocket.Connected;        }        private void ConnectCallBack(IAsyncResult asyncresult)        {            try            {                mSocket = asyncresult.AsyncState as Socket;                if (mSocket != null)                {                    mSocket.EndConnect(asyncresult);                    IsConnectSuccess = true;                }            }            catch (Exception ex)            {                Console.WriteLine("Socket connectFailed,Ex:" + ex.StackTrace.ToString());                IsConnectSuccess = false;            }            finally            {                TimeoutObject.Set();            }        }        public bool SocketConnection(string LocalIP, int LocalPort)        {            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);            try            {                IPAddress ip = IPAddress.Parse(LocalIP);                IPEndPoint ipe = new IPEndPoint(ip, LocalPort);                mSocket.Connect(ipe);                WireProtocolFactory = new MyWireProtocolFactory();                WireProtocol = WireProtocolFactory.CreateWireProtocol();                WireProtocol.Reset();                msgQueue = new Queue<IScsMessage>();                if (mSocket.Connected)                {                    BeginRecieveData();                }                return true;            }            catch (Exception e)            {                //   Console.WriteLine("connect failed!" + e.Message);                Debug.Log("connect failed!" + e.Message);                return false;            }        }        public void close()        {            if (mSocket != null)            {                if(mSocket.Connected)                   mSocket.Shutdown(SocketShutdown.Both);                mSocket.Close();                mSocket = null;                //Debug.LogError("assssssssssssss");            }        }        public bool Connected()        {            return mSocket.Connected;        }        public bool CheckSocketConnected()
        {
            // This is how you can determine whether a socket is still connected.
            bool connectState = true;
            bool blockingState = mSocket.Blocking;
            try
            {
                byte[] tmp = new byte[1];

                mSocket.Blocking = false;
                mSocket.Send(tmp, 0, 0);
                //Console.WriteLine("Connected!");
                Debug.Log("phone over connected!");
                connectState = true; //若Send错误会跳去执行catch体，而不会执行其try体里其之后的代码
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035))
                {
                    //Console.WriteLine("Still Connected, but the Send would block");
                    connectState = true;
                    Debug.Log("phone over still connected!");
                }

                else
                {
                    //Console.WriteLine("Disconnected: error code {0}!", e.NativeErrorCode);
                    Debug.Log("phone over  disconnected!" + e.NativeErrorCode);
                    connectState = false;
                }
            }
            finally
            {
                mSocket.Blocking = blockingState;
            }

            //Console.WriteLine("Connected: {0}", client.Connected);
            return connectState;
        }        public void Send(float data)        {            byte[] longth = TypeConvert.getBytes(data, true);            mSocket.Send(longth);        }        public void Send(byte[] data)        {            if (!mSocket.Connected)            {                Console.WriteLine("Socket is disconnect");                return;            }            try            {
                mSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSendCallBack), mSocket);
            }            catch (Exception e)            {
                mSocket.Shutdown(SocketShutdown.Both);                mSocket.Close();
                Debug.LogError(e.Message);
            }         }        private void OnSendCallBack(IAsyncResult ar)        {            Socket sock = (Socket)ar.AsyncState;            if (!sock.Connected)                return;            try            {                int nSentBytes = sock.EndSend(ar);                if (nSentBytes <= 0)                {                    sock.Shutdown(SocketShutdown.Both);                    sock.Close();                    Debug.LogError(nSentBytes);                }            }            catch (Exception e)            {                sock.Shutdown(SocketShutdown.Both);                sock.Close();                //               Console.WriteLine("OnSendCallBack error:{0}", e.Message);                //               Debug.Log("OnSendCallBack error:" + e.Message);                Debug.LogError(e.Data);                Debug.LogError(e.InnerException);                Debug.LogError(e.StackTrace);                Debug.LogError(e.TargetSite);                Debug.LogError(e.Source);            }        }        public void Send(UMessage msg)        {            if (WireProtocol == null)                return;            var messageBytes = msg.GetBytes();            //string msg1 = "";            //foreach (byte b in msg.GetBytes())            //    msg1 += b;            //Debug.LogError(msg1);            Send(messageBytes);        }        public void BeginRecieveData()        {            try            {                AsyncCallback recieveData = new AsyncCallback(OnReceivedData);                mSocket.BeginReceive(m_byRecvBuff, 0, m_byRecvBuff.Length, SocketFlags.None, recieveData, mSocket);                // Debug.LogError(m_byBuff.Length);            }            catch (Exception e)            {                Console.WriteLine("Read data failed:{0}", e.Message);                mSocket.Shutdown(SocketShutdown.Both);
                mSocket.Close();            }        }        public void OnReceivedData(IAsyncResult ar)        {            Socket sock = (Socket)ar.AsyncState;            if (!sock.Connected)                return;            try            {                int nBytesRec = sock.EndReceive(ar);                if (nBytesRec > 0)                {                    // Wrote the data to the List                    var receivedBytes = new byte[nBytesRec];                    //     string sRecieved = Encoding.UTF8.GetString(m_byBuff, 0, nBytesRec);                    //     Console.WriteLine("receiv data:{0}", sRecieved);                    // If the connection is still usable restablish the callback                    Array.Copy(m_byRecvBuff, 0, receivedBytes, 0, nBytesRec);                    //SplitPackage(receivedBytes, nBytesRec);                    SplitPackageWithRemainBuff(receivedBytes,nBytesRec);                    BeginRecieveData();                }                else                {                    // If no data was recieved then the connection is probably dead                    Console.WriteLine("Client {0}, disconnected", sock.RemoteEndPoint);                    sock.Shutdown(SocketShutdown.Both);                    sock.Close();                    //Debug.LogError("dddddddddddddd");                }            }            catch (Exception e)            {                int nBytesRec = sock.EndReceive(ar);                Debug.LogError(nBytesRec);                sock.Shutdown(SocketShutdown.Both);                sock.Close();                Debug.LogError(e.Data);                Debug.LogError(e.InnerException);                Debug.LogError(e.StackTrace);                Debug.LogError(e.TargetSite);                Debug.LogError(e.Source);                Console.WriteLine("OnReceivedData error:{0}", e.Message);                Debug.Log("OnReceivedData error:" + e.Message);            }        }        private void SplitPackageWithRemainBuff(byte[] RecvMsgBytes,int iBuffSize)        {            int totalBuffSize = iBuffSize;            byte[] TotalBytesBuff;            //如果上次有剩余数据未处理            if (m_iRemainBuffSize > 0)            {                              totalBuffSize += m_iRemainBuffSize;                TotalBytesBuff = new byte[totalBuffSize];                Array.Copy(m_byRemainBuff, 0, TotalBytesBuff, 0, m_iRemainBuffSize);                            }            else            {                TotalBytesBuff = new byte[totalBuffSize];            }            Array.Copy(RecvMsgBytes, 0, TotalBytesBuff, m_iRemainBuffSize, iBuffSize);                       int buffheadIndex = 0;            while (true)            {                byte[] packageHead = new byte[4];                Array.Copy(TotalBytesBuff, buffheadIndex + 5, packageHead, 0, 4);                int packetlength = (BitConverter.ToInt32(packageHead, 0));                //Array.Copy(TotalBytesBuff, buffheadIndex + 1, packageHead, 0, 4);                //int msgtype = (BitConverter.ToInt32(packageHead, 0));                if (packetlength >= 9 && packetlength <= totalBuffSize)                {                    byte[] data = new byte[packetlength];                    Array.Copy(TotalBytesBuff, buffheadIndex, data, 0, packetlength);                    PushMessage(new UMessage(data));                    buffheadIndex += packetlength;                    totalBuffSize -= packetlength;                    //if (msgtype == 10)                    //    Debug.Log("MsgLength:" + packetlength + ",MsgType:" + msgtype);                    //数据全部处理完毕                    if (totalBuffSize == 0)                    {                        m_iRemainBuffSize = 0;                        //Debug.Log("��Ĵ�������" + buffheadIndex  + BuffSize);                        break;                    }                }                else                {                    m_iRemainBuffSize = totalBuffSize - buffheadIndex;                    Array.Copy(TotalBytesBuff, 0, m_byRemainBuff, buffheadIndex, m_iRemainBuffSize);                    break;                }            }        }        private void SplitPackage(byte[] bytes, int BuffSize)        {            int buffheadIndex = 0;            while (true)            {                byte[] packageHead = new byte[4];                Array.Copy(bytes, buffheadIndex + 5, packageHead, 0, 4);                int packetlength = (BitConverter.ToInt32(packageHead, 0));                Array.Copy(bytes, buffheadIndex + 1, packageHead, 0, 4);                int msgtype = (BitConverter.ToInt32(packageHead, 0));                if (packetlength >= 9)                {                    byte[] data = new byte[packetlength];                    Array.Copy(bytes, buffheadIndex, data, 0, packetlength);                    PushMessage(new UMessage(data));                    buffheadIndex += packetlength;                    if (msgtype == 10)                        Debug.Log("MsgLength:" + packetlength + ",MsgType:" + msgtype);                    if ((buffheadIndex) >= (BuffSize))                    {                        //Debug.Log("��Ĵ�������" + buffheadIndex  + BuffSize);                        break;                    }                }                else                {                    break;                }            }        }        private object BytesToStruct(byte[] bytes, Type type)        {            /*int size  =  Marshal.sizeof(type);			if(size > bytes.Length)				return null;			IntPtr buffer = Marshal.AllocHGlobal(size);*/            return null;        }        /// <summary>        /// push into deal message queue        /// </summary>        /// <param name="message"></param>        protected void PushMessage(IScsMessage message)        {            lock (msgQueue)            {                msgQueue.Enqueue(message);            }        }        /// <summary>        /// copy message queue        /// </summary>        /// <returns></returns>        public Queue<IScsMessage> CopyMessage()        {            lock (msgQueue)            {                IScsMessage[] array = new IScsMessage[msgQueue.Count];                msgQueue.CopyTo(array, 0);                msgQueue.Clear();                Queue<IScsMessage> retQueue = new Queue<IScsMessage>(array);                return retQueue;            }        }        /// <summary>        /// Get Message Count        /// </summary>        /// <returns></returns>        public int GetMessageCount()        {            lock (msgQueue)            {                return msgQueue.Count;            }        }    }}