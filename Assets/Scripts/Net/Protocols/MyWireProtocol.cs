﻿using System;using System.Text;using USocket.Messages;using USocket.Protocols.BinarySerialization;using USocket.Public;using System.IO;namespace USocket.Protocols{    /// <summary>    /// This class is a sample custom wire protocol to use as wire protocol in SCS framework.    /// It extends BinarySerializationProtocol.    /// It is used just to send/receive ScsTextMessage messages.    ///     /// Since BinarySerializationProtocol automatically writes message length to the beggining    /// of the message, a message format of this class is:    ///     /// [Message length (4 bytes)][UTF-8 encoded text (N bytes)]    ///     /// So, total length of the message = (N + 4) bytes;    /// </summary>    public class MyWireProtocol : BinarySerializationProtocol    {        protected override byte[] SerializeMessage(IScsMessage message)        {            //MemoryStream stream = new MemoryStream();            //BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);                        //uint nType = ((ScsTextMessage)message).RepliedMessageId;            //string str = ((ScsTextMessage)message).Text;            //int length = sizeof(uint) + Encoding.UTF8.GetBytes(str).Length;            //byte[] retValue = new byte[length];            //int index = 0;            //Converter.IntToBytes(nType).CopyTo(retValue, index);            //index += 4;            //Encoding.UTF8.GetBytes(str).CopyTo(retValue, index);            //writer.Write(nType);            //writer.Write(str);            //byte[] ret = stream.ToArray();            //return retValue;            UMessage msg = ((UMessage)message);            return msg.ToBytes();        }        protected override IScsMessage DeserializeMessage(byte[] bytes)        {            //Decode UTF8 encoded text and create a ScsTextMessage object       //     uint type = Converter.BytesToUInt(bytes, 0);       //     string str = Encoding.UTF8.GetString(bytes, 4, bytes.Length-4);        //    return new ScsTextMessage(str, type);            return new UMessage(bytes);         //   return new ScsTextMessage(Encoding.UTF8.GetString(bytes,4,0));        }    }}