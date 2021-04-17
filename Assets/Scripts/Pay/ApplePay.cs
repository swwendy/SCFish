#if UNITY_IOSusing UnityEngine.Purchasing;using UnityEngine;using System.Net;
using System.Text;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

public class ApplePay :  IStoreListener{
    public static ApplePay Instance = new ApplePay();

    //IAP组件相关的对象，m_Controller里存储着商品信息
    private IStoreController m_Controller;    private IAppleExtensions m_AppleExtensions;    /// <summary>
    /// 是否在购买过程中
    /// </summary>    private bool bPurchaseInProgress;

    private bool bInitUnityPurchaseSuceess;

    private string WaitingBuyProductId;

    private ApplePay()
    {
        WaitingBuyProductId = string.Empty;
        bPurchaseInProgress = false;
        bInitUnityPurchaseSuceess = false;
        //InitUnityPurchase();
    }

    //初始化支付环境
    public void InitUnityPurchase(List<uint> productlist)    {        if (bInitUnityPurchaseSuceess)            return;        bPurchaseInProgress = false;        var module = StandardPurchasingModule.Instance();        var builder = ConfigurationBuilder.Instance(module);

        //初始商店商品
        foreach(var productId in productlist)
        {
            builder.AddProduct(productId.ToString(), ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);    }


    //初始化成功回调
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)    {        m_Controller = controller;

        // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.  
        // On non-Apple platforms this will have no effect; OnDeferred will never be called.  
        IAppleExtensions m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

        //Product product = m_Controller.products.WithID("GameCityDiamondT1");

        Debug.Log("OnInitialized: Success");
        bInitUnityPurchaseSuceess = true;

        if(WaitingBuyProductId != string.Empty)
        {
            BuyItem(WaitingBuyProductId);
        }

        //测试购买一个东西
        //BuyItem("GameCityTestItem1");
    }

    //初始化失败回调
    public void OnInitializeFailed(InitializationFailureReason error)    {        Debug.Log("Billing failed to initialize!");        switch (error)        {            case InitializationFailureReason.AppNotKnown:                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");                break;            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");                break;            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");                break;        }    }

    /// <summary>
    /// 购买成功处理
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)    {

        Debug.Log("Purchase OK: " + args.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + args.purchasedProduct.receipt);

        
        var jsonobj = new JSONObject(args.purchasedProduct.receipt);
        Dictionary<string, string> result = jsonobj.ToDictionary();   
        string receiptPayload = result["Payload"];
        //Debug.Log("xxxxx jsonreceipt payload:" + receiptPayload);

        //以下为测试验证，正确流程是把json发送给服务器，服务器拿这个去apple itune验证
        string json = "{\"receipt-data\":\"" + receiptPayload + "\"}";
        //VerifyReceipt(json);

        Player.SendBuyReceiptToServer(PayPlatform.Apple,json);
        //购买流程已结束，等待服务器验证结果
        Player.BuyEnd();

        bPurchaseInProgress = false;
        return PurchaseProcessingResult.Complete;    }

   public void VerifyReceipt(string receiptstr)
    {
        try
        {
            byte[] postBytes = Encoding.UTF8.GetBytes(receiptstr);

            //正式地址
            //var req = HttpWebRequest.Create("https://buy.itunes.apple.com/verifyReceipt");
            //测试地址
            var req = HttpWebRequest.Create("https://sandbox.itunes.apple.com/verifyReceipt");
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = postBytes.Length;

            using (var stream = req.GetRequestStream())
            {
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Flush();
            }

            var sendResp = req.GetResponse();
            string sendRespText = "";
            using (var streamReader = new System.IO.StreamReader(sendResp.GetResponseStream()))
            {
                sendRespText = streamReader.ReadToEnd().Trim();
            }

            var jsonobj = new JSONObject(sendRespText);
            Dictionary<string, string> result = jsonobj.ToDictionary();

            Debug.Log("receipt return data:" + sendRespText);

            foreach (var a in result)
            {
                Debug.Log(a.Key + ":" + a.Value);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Exception: " + ex.Message.ToString());
        }
    }

    private void ResponseCallback(IAsyncResult asyncResult)
    {
        HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
        // 获取Response
        HttpWebResponse response = request.EndGetResponse(asyncResult) as HttpWebResponse;
        if (response.StatusCode == HttpStatusCode.OK)
        {

            // 读取请求内容
            Stream responseStream = response.GetResponseStream();
            byte[] buff = new byte[2048];
            MemoryStream ms = new MemoryStream();
            int len = -1;
            while ((len = responseStream.Read(buff, 0, buff.Length)) > 0)
            {
                ms.Write(buff, 0, len);
            }
            Debug.Log("receipt back mmmmmmmdata:" + Encoding.Default.GetString(ms.GetBuffer()));
            // 清理操作
            responseStream.Close();
            response.Close();
            request.Abort();
        }
    }

    //购买失败回调
    public void OnPurchaseFailed(Product item, PurchaseFailureReason r)    {        Debug.Log("Purchase failed: " + item.definition.id);        Debug.Log(r);        bPurchaseInProgress = false;        Player.BuyEnd();    }


    //购买延迟提示
    private void OnDeferred(Product item)    {        Debug.Log("Purchase deferred: " + item.definition.id);
    }


    //发起购买
    public void BuyItem(string productId)    {
        //商城还没有初始化成功，放入购物车
        if (bInitUnityPurchaseSuceess == false)
        {           
            WaitingBuyProductId = productId;
            return;
        }
        if(bPurchaseInProgress)
        {
            Debug.Log("上一次购买还未结束");
            return;
        }

        if (m_Controller != null)
        {
             var product = m_Controller.products.WithID(productId);             if (product != null && product.availableToPurchase)            {
                //调起支付  
                bPurchaseInProgress = true;
                m_Controller.InitiatePurchase(productId);
            }
        }
    }
}
#endif