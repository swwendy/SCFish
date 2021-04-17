using UnityEngine;

using ZXing;
using ZXing.QrCode;
using System.IO;
using XLua;

[LuaCallCSharp]public class QRCode
{
    static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    public static Texture2D CreateQR(string textForEncoding)
    {
        if (textForEncoding == null)
            return null;

        Texture2D encoded = new Texture2D(256, 256);
        var color32 = Encode(textForEncoding, encoded.width, encoded.height);
        if (color32 == null)
            return null;

        encoded.SetPixels32(color32);
        encoded.Apply();

        Texture2D encoded1 = new Texture2D(210, 210);
        encoded1.SetPixels(encoded.GetPixels(23, 23, 210, 210));
        encoded1.Apply();

        return encoded1;
    }

    public static void SaveQR(string path, string file, Texture2D tex, bool force = false)
    {
        string filePath = path + file;
        if (!force && File.Exists(filePath))
            return;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var bys = tex.EncodeToPNG();
        File.WriteAllBytes(filePath, bys);
    }

    public static Texture2D LoadQR(string path, string file)
    {
        string filePath = path + file;
        if (!File.Exists(filePath))
            return null;

        FileStream imgstream = File.OpenRead(filePath);
        byte[] imgbytes = new byte[(int)imgstream.Length];
        imgstream.Read(imgbytes, 0, imgbytes.Length);
        Texture2D tex = new Texture2D(256, 256);
        tex.LoadImage(imgbytes);

        return tex;
    }

    static string encryptKey = "Yuan"; //定义秘钥

    //public static string Encrypt(string str)  //加密
    //{
    //    DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();//实例化加密对象
    //    byte[] key = Encoding.Unicode.GetBytes(encryptKey);//定义字节数组，用来存储秘钥
    //    byte[] data = Encoding.Unicode.GetBytes(str);//定义字节数组，用来要存储加密的字符串
    //    MemoryStream mStream = new MemoryStream();//实例化内存流对象
    //                                              //使用内存流实例化加密对象
    //    CryptoStream cStream = new CryptoStream(mStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);
    //    cStream.Write(data, 0, data.Length);
    //    cStream.FlushFinalBlock();
    //    return Convert.ToBase64String(mStream.ToArray());
    //}

    //public static string DesCrypt(string str)
    //{
    //    DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();//定义加密类对象
    //    byte[] key = Encoding.Unicode.GetBytes(encryptKey);
    //    byte[] data = Convert.FromBase64String(str);
    //    MemoryStream mStram = new MemoryStream();//实例化内存流对象
    //    CryptoStream cStream = new CryptoStream(mStram, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);
    //    cStream.Write(data, 0, data.Length);
    //    cStream.FlushFinalBlock();
    //    return Encoding.Unicode.GetString(mStram.ToArray());
    //}

    public static string EncryptOrDescrypt(string str)
    {
        char[] data = str.ToCharArray();
        char[] key = encryptKey.ToCharArray();
        for (int i = 0; i < data.Length; i++)
        {
            data[i] ^= key[i % key.Length];
        }

        return new string(data);
    }
}
