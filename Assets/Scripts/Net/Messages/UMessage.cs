using System;
using USocket.Public;
using System.Collections;
using System.Runtime.InteropServices;

/*
 * 
 *C++            C#
=====================================
WORD            ushort
DWORD            uint
UCHAR            int/byte  
UCHAR*            string/IntPtr
unsigned char*         [MarshalAs(UnmanagedType.LPArray)]byte[]
char*            string
LPCTSTR            string
LPTSTR            [MarshalAs(UnmanagedType.LPTStr)] string
long            int
ulong               uint
Handle            IntPtr
HWND            IntPtr
void*            IntPtr
int            int
int*            ref int
*int            IntPtr
unsigned int        uint
COLORREF                uint
*/

namespace USocket.Messages
{
    using System.IO;
    using System.Text;


    /// <summary>
    ///  base message for unity
    /// </summary>
    

	[Serializable]
    public class UMessage : IScsMessage
    {

        //public byte bEntry { get; set; }

		//public uint iEntryMsgSize { get; set; }
		public byte BaseMsgType{get; set;}
		
		public uint  iMsgType {get; set;} 
		public uint  iMsgSize {get; set;}
        /// <summary>
        ///  define message header
        /// </summary>
        
		/*[StructLayout(LayoutKind.Sequential,Pack =1)]
		public struct Header
        {
			public byte bEntry;             //消息是否加密
			public uint  iEntryMsgSize;  //加密消息结构体大小
            public byte BaseMsgType;        //基础消息类型  （与服务端一致固定为126） 
            public uint  iMsgType;       //消息类型
            public uint  iMsgSize;        //消息结构体大小
        }
        Header stHeader;*/

        /// <summary>
        ///  for send or receive data
        /// </summary>
        protected MemoryStream stream;
        protected BinaryWriter writer;
        protected BinaryReader reader;

        public UMessage()
        {
        }

        /// <summary>
        ///  construction 
        /// </summary>
        /// <param name="lType"></param>
        public UMessage(uint type)
        {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream, Encoding.UTF8);
			//bEntry = 0;
			//stHeader.iEntryMsgSize = 1;
			BaseMsgType = (byte)1;             //默认发个login
            //stHeader.lSize = sizeof(uint);  // 不包含自己
            iMsgType = type;
			//stHeader.iMsgSize = 1;            
            
        }

        public UMessage(byte[] data)
        {
            stream = new MemoryStream(data);
            reader = new BinaryReader(stream, Encoding.UTF8);
			//bEntry = ReadByte();
			//iEntryMsgSize = ReadUInt();
			BaseMsgType = ReadByte();
			iMsgType = ReadUInt();
			iMsgSize = ReadUInt();
        }

        public uint GetMsgType()
        {
            return iMsgType;
        }

        public void Add(ulong data)
        {
            writer.Write(data);
        }

        public ulong ReadULong()
        {
            return reader.ReadUInt64();
        }

        public void Add(uint data)
        {
            writer.Write(data);
        }

        public uint ReadUInt()
        {
            return reader.ReadUInt32();
        }

        //public void Add(bool data)
        //{
        //    writer.Write(data);
        //}

        public bool ReadBool()
        {
            return reader.ReadBoolean();
        }

        public void Add(int data)
        {
            writer.Write(data);
        }

        public int ReadInt()
        {
            return reader.ReadInt32();
        }

        public void Add(float data)
        {
            writer.Write(data);
        }

        public float ReadSingle()
        {
            return reader.ReadSingle();
        }

        public void Add(byte[] data)
        {
            writer.Write(data);
        }

        //dy 2016年1月28日14:21:56
        public byte[] ReadAllLeftBytes()
        {
            long leftlen = reader.BaseStream.Length - reader.BaseStream.Position;
            return reader.ReadBytes((int)leftlen);
        }

        public byte[] ReadBytes(int count)
        {
            return reader.ReadBytes(count);
        }

        public void Add(byte data)
        {
            writer.Write(data);
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public void Add(sbyte data)
        {
            writer.Write(data);
        }

        public sbyte ReadSByte()
        {
            return reader.ReadSByte();
        }

        public void Add(long data)
		{
			writer.Write(data);
		}
		
		public long ReadLong()
		{
			return reader.ReadInt64();
		}
	
		public void Add(short data)
		{
			writer.Write(data);
		}

        public void Add(bool data)
        {
            writer.Write(data);
        }
		
		public short ReadShort()
		{
			return reader.ReadInt16();
		}
		
		public void Add(ushort data)
		{
			writer.Write(data);
		}
		
		public ushort ReaduShort()
		{
			return reader.ReadUInt16();
		}
		
		public void Add(char data)
		{
			writer.Write(data);
		}
		
		public char ReadChar()
		{
			return reader.ReadChar();
		}

        /// <summary>
        /// 长度占用字符数有疑问，故按照byte[]写入
        /// </summary>
        /// <param name="data"></param>
        public void Add(string data)
        {
            byte[] bt = Encoding.UTF8.GetBytes(data);
            int length = bt.Length;
            Add(length);
            Add(bt);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        public string ReadString()   
        {
            int length = ReadInt();
			if(length==0)
			{
				return "";
			}
            byte[] bt = ReadBytes(length);
            return Encoding.UTF8.GetString(bt);
        }

        public byte[] ToBytes()
        {
           iMsgSize = (uint)stream.Length;
        //    byte[] st = Converter.StructToBytes(stHeader);
            MemoryStream msHeader = new MemoryStream();
            BinaryWriter bwHeader = new BinaryWriter(msHeader, Encoding.UTF8);
       //     bwHeader.Write(st);
			//bwHeader.Write(bEntry);
            //bwHeader.Write((uint)169);
			bwHeader.Write(BaseMsgType);
			bwHeader.Write(iMsgType);
			bwHeader.Write((uint)169);
            stream.WriteTo(msHeader);
            return msHeader.ToArray();
        }

		public virtual byte[] GetBytes()
		{
			uint streamsize =  (uint) (stream.Length +9);
			MemoryStream msHeader = new MemoryStream();
			BinaryWriter bwHeader = new BinaryWriter(msHeader, Encoding.UTF8);
			//bwHeader.Write(bEntry);
			//bwHeader.Write(streamsize);
			bwHeader.Write(BaseMsgType);
			bwHeader.Write(iMsgType);
			bwHeader.Write(streamsize);
			stream.WriteTo(msHeader);
			return msHeader.ToArray();
		}
		
		public void Add(Guid guid)
        {
            byte[] bt = guid.ToByteArray();
            Add((Byte)bt.Length);            
            Add(bt);
        }

        public Guid ReadGuid()
        {
            int length = (int)ReadByte();
			if(length==0)
			{
				return Guid.Empty;
			}
            byte[] bt = ReadBytes(length);
            return new Guid(bt);
        }
    }
}