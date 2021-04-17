namespace USocket.Net
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
        }
                mSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSendCallBack), mSocket);
            }
                mSocket.Shutdown(SocketShutdown.Both);
                Debug.LogError(e.Message);
            } 
                mSocket.Close();