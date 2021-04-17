//
//  WECHATPAY_OC.m
//
//  Created by Batcel on 6/26/2017.

#import <Foundation/Foundation.h>  
#import <UIKit/UIKit.h>
#import "WXApi.h"
#import "UnityAppController.h"


NSString* const WXAPPID = @"wx043094aaa5449b38";
NSString* const WXMCH_ID = @"1515219211";
NSString* const WXAPIKEY = @"DjukmAw87yhgwi2SnagvHqsfzmstrc6x";

extern unsigned char *CC_MD5(const void *data, uint32_t len, unsigned char *md)
__OSX_AVAILABLE_STARTING(__MAC_10_4, __IPHONE_2_0);

@interface MD5Sign : NSObject
+(NSString *)createMD5SingForPay:(NSString *)prepayid_key noncestr:(NSString *)noncestr_key
                       timestamp:(NSString *)timestamp_key;

+(NSString *) md5:(NSString *)str;

@end


@implementation MD5Sign

//创建发起支付时的sige签名

+(NSString *)createMD5SingForPay:(NSString *)prepayid_key noncestr:(NSString *)noncestr_key
                       timestamp:(NSString *)timestamp_key
{
    NSMutableDictionary *dic = [NSMutableDictionary dictionary];
    [dic setObject:WXAPPID forKey:@"appid"];
    [dic setObject:noncestr_key forKey:@"noncestr"];
    [dic setObject:@"Sign=WXPay" forKey:@"package"];
    [dic setObject:WXMCH_ID forKey:@"partnerid"];
    [dic setObject:prepayid_key forKey:@"prepayid"];
    [dic setObject:timestamp_key forKey:@"timestamp"];
    
    NSMutableString *contentString  =[NSMutableString string];
    NSArray *keys = [dic allKeys];
    //按字母顺序排序
    NSArray *sortedArray = [keys sortedArrayUsingComparator:^NSComparisonResult(id obj1, id obj2)
    {
        return [obj1 compare:obj2 options:NSNumericSearch];
    }];
    //拼接字符串
    for (NSString *categoryId in sortedArray)
    {
        if (![[dic objectForKey:categoryId] isEqualToString:@""]
            && ![[dic objectForKey:categoryId] isEqualToString:@"sign"]
            && ![[dic objectForKey:categoryId] isEqualToString:@"key"]
            )
        {
            [contentString appendFormat:@"%@=%@&", categoryId, [dic objectForKey:categoryId]];
        }
    }
    //添加商户密钥key字段
#warning 注意此处一定要添加上商户密钥
    [contentString appendFormat:@"key=%@", WXAPIKEY];
    NSString *result = [MD5Sign md5:contentString];
    
    NSLog(@"result = %@",result);
    return result;
}


// MD5加密算法
+(NSString *) md5:(NSString *)str
{
    const char *cStr = [str UTF8String];
    //加密规则，因为逗比微信没有出微信支付demo，这里加密规则是参照安卓demo来得
    unsigned char result[16]= "0123456789abcdef";
    CC_MD5(cStr, (uint32_t)strlen(cStr), result);
    //这里的x是小写则产生的md5也是小写，x是大写则md5是大写，这里只能用大写，逗比微信的大小写验证很逗
    return [NSString stringWithFormat:
            @"%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X",
            result[0], result[1], result[2], result[3],
            result[4], result[5], result[6], result[7],
            result[8], result[9], result[10], result[11],
            result[12], result[13], result[14], result[15]
            ];
}
@end



//屏幕截图缩略图
@interface Screenshot : NSObject
+(UIImage *) GetImageWithScreenshot;
+(UIImage *) GetImagethumbScale:(UIImage*) scrImg;

@end

@implementation Screenshot

//获取图片缩略图
+(UIImage *) GetImagethumbScale:(UIImage*) scrImg
{
    CGSize size= {160,90};
    UIGraphicsBeginImageContext(size);
    [scrImg drawInRect:CGRectMake(0, 0, size.width, size.height)];
    UIImage* newimg = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newimg;
}

//获取屏幕截图
+(UIImage *) GetImageWithScreenshot
{
    CGSize imageSize = CGSizeZero;
    //UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
    //if (UIInterfaceOrientationIsPortrait(orientation))
        imageSize = [UIScreen mainScreen].bounds.size;
    //else
    //    imageSize = CGSizeMake([UIScreen mainScreen].bounds.size.height, [UIScreen mainScreen].bounds.size.width);
    
    UIGraphicsBeginImageContextWithOptions(imageSize, NO, 0);
    CGContextRef context = UIGraphicsGetCurrentContext();
    for (UIWindow *window in [[UIApplication sharedApplication] windows])
    {
        CGContextSaveGState(context);
        CGContextTranslateCTM(context, window.center.x, window.center.y);
        CGContextConcatCTM(context, window.transform);
        CGContextTranslateCTM(context, -window.bounds.size.width * window.layer.anchorPoint.x, -window.bounds.size.height * window.layer.anchorPoint.y);
        /*if (orientation == UIInterfaceOrientationLandscapeLeft)
        {
            CGContextRotateCTM(context, M_PI_2);
            CGContextTranslateCTM(context, 0, -imageSize.width);
        }
        else if (orientation == UIInterfaceOrientationLandscapeRight)
        {
            CGContextRotateCTM(context, -M_PI_2);
            CGContextTranslateCTM(context, -imageSize.width, -imageSize.height);
        } else if (orientation == UIInterfaceOrientationPortraitUpsideDown) {
            CGContextRotateCTM(context, M_PI);
            CGContextTranslateCTM(context, -imageSize.width, -imageSize.height);
        }*/
        if ([window respondsToSelector:@selector(drawViewHierarchyInRect:afterScreenUpdates:)])
        {
            [window drawViewHierarchyInRect:window.bounds afterScreenUpdates:YES];
        }
        else
        {
            [window.layer renderInContext:context];
        }
        CGContextRestoreGState(context);
    }
    
    UIImage *image = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    NSData *imageData = UIImageJPEGRepresentation(image,1.0);
    return [UIImage imageWithData:imageData];
}

@end



#if defined (__cplusplus)
extern "C"  
{ 
#endif
    
    //微信平台登陆jiezhang
    void WeChatLgoinJieZhang_IOS(char* prepayIdstr,char* noncestr,char* timestr,char* signstr)
    {  
	    //项目属性中的URL Schemes为您的APPID
        //[WXApi registerApp:WXAPPID enableMTA:false];
	   
	   NSString *prePayString = [[NSString alloc] initWithUTF8String:prepayIdstr]; 
	   NSString *nonceString = [[NSString alloc] initWithUTF8String:noncestr]; 
	   NSString *timeStampString = [[NSString alloc] initWithUTF8String:timestr]; 
	   NSString *signString = [[NSString alloc] initWithUTF8String:signstr]; 

       //NSString* md5signstr = [MD5Sign createMD5SingForPay:prePayString noncestr:nonceString timestamp:timeStampString];
       
       PayReq *request = [[PayReq alloc] init];
       request.openID = WXAPPID;
       request.partnerId = WXMCH_ID;
       request.prepayId= prePayString;
       request.package = @"Sign=WXPay";
       request.nonceStr= nonceString;
       request.timeStamp= timeStampString.intValue;
       request.sign= signString;
       [WXApi sendReq:request];
	}

    //微信是否安装
	bool WeChat_IsWXAppInstalled()
	{
	  if([WXApi isWXAppInstalled])
	  {
	    if([WXApi isWXAppSupportApi])
		  return YES;
	  }
	  return NO;
	}

	//微信登陆授权
	void WeChat_AuthLogin()
	{
	  //构造SendAuthReq结构体 
      SendAuthReq* req =[[SendAuthReq alloc] init];
      req.scope = @"snsapi_userinfo";
      req.state = @"JoyBigGamer_WxAuth";
      //第三方向微信终端发送一个SendAuthReq消息结构
      [WXApi sendReq:req];
	}
    
    //微信分享图片
    void WeChat_ShareImage(bool isTimeline)
    {
        UIImage* shareImg = [Screenshot GetImageWithScreenshot];
        //2.缩略图
        WXMediaMessage * messageobj = [WXMediaMessage message];
        [messageobj setThumbImage:[Screenshot GetImagethumbScale:shareImg]];
        
        //3.高清大图
        WXImageObject * iamgeOb = [WXImageObject object];
        iamgeOb.imageData = UIImageJPEGRepresentation(shareImg,1.0);
        messageobj.mediaObject = iamgeOb;
        
        //4.发送请求
        SendMessageToWXReq * req = [[SendMessageToWXReq alloc]init];
        req.bText = NO;
        req.message = messageobj;
        req.scene = isTimeline? WXSceneTimeline : WXSceneSession;
        [WXApi sendReq:req];
    }

	//微信分享图片
    void WeChat_ShareImageByPath(char* imgpath,bool isTimeline)
    { 
	    NSString *ImgPathString = [[NSString alloc] initWithUTF8String:imgpath]; 

        UIImage* shareImg = [UIImage imageWithContentsOfFile:ImgPathString];

        //2.缩略图
        WXMediaMessage * messageobj = [WXMediaMessage message];
        [messageobj setThumbImage:[Screenshot GetImagethumbScale:shareImg]];
        
        //3.高清大图
        WXImageObject * iamgeOb = [WXImageObject object];
        iamgeOb.imageData = UIImageJPEGRepresentation(shareImg,1.0);
        messageobj.mediaObject = iamgeOb;
        
        //4.发送请求
        SendMessageToWXReq * req = [[SendMessageToWXReq alloc]init];
        req.bText = NO;
        req.message = messageobj;
        req.scene = isTimeline? WXSceneTimeline : WXSceneSession;
        [WXApi sendReq:req];      
    }

	//微信分享文本
    void WeChat_ShareText(char* sharetext,bool isTimeline)
    {

	    NSString *shareString = [[NSString alloc] initWithUTF8String:sharetext]; 
        SendMessageToWXReq * req = [[SendMessageToWXReq alloc]init];
        req.bText = YES;
		req.text = shareString;
        req.scene = isTimeline? WXSceneTimeline : WXSceneSession;
        [WXApi sendReq:req];
        
    }

	//微信分享网页
    void WeChat_ShareURL(char* shareUrl,char* descriptiontext, bool isTimeline)
    {
	    NSString *shareurlstr= [[NSString alloc] initWithUTF8String:shareUrl]; 
		NSString *sharedescstr= [[NSString alloc] initWithUTF8String:descriptiontext]; 

		WXMediaMessage *messageobj = [WXMediaMessage message];
	    messageobj.title = @"凤凰掼蛋";//分享标题
        messageobj.description = sharedescstr;//分享描述

		NSDictionary *infoPlist = [[NSBundle mainBundle] infoDictionary];
        NSString *icon = [[infoPlist valueForKeyPath:@"CFBundleIcons.CFBundlePrimaryIcon.CFBundleIconFiles"] lastObject];
        [messageobj setThumbImage:[UIImage imageNamed:icon]];

        //[messageobj setThumbImage:[UIImage imageNamed:@"icon.png"]];

		//创建多媒体对象
        WXWebpageObject *webObj = [WXWebpageObject object];
        webObj.webpageUrl = shareurlstr;//分享链接
	    messageobj.mediaObject = webObj;

        SendMessageToWXReq * req = [[SendMessageToWXReq alloc]init];
        req.bText = NO;
        req.message = messageobj;
        req.scene = isTimeline? WXSceneTimeline : WXSceneSession;
        [WXApi sendReq:req];        
    }

	//更新提示
    void ShowAppUpdateTips(char* urlstring)
    {
       //NSString *appurlstring = @"itms-apps://itunes.apple.com/cn/app/qiu-qiu-dou-di-zhu/id1319175293?mt=8";
	   NSString *appurlstring = [[NSString alloc] initWithUTF8String:urlstring];
        //初始化提示框；
       UIAlertController *alert = [UIAlertController alertControllerWithTitle:@"系统" message:@"发现新版本" preferredStyle:  UIAlertControllerStyleAlert];
        
       [alert addAction:[UIAlertAction actionWithTitle:@"更新" style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action)
	    {
          //点击按钮的响应事件；
          [[UIApplication sharedApplication] openURL:[NSURL URLWithString:appurlstring]];
        }]];
        
        //弹出提示框；
       UIViewController * vcCurViewController =[[UIApplication sharedApplication] keyWindow].rootViewController;
       [vcCurViewController presentViewController:alert animated:true completion:nil];
    }
    
	//获取手机电池电量
	float GetBatteryLevel()
    {
        UIDevice *myDevice = [UIDevice currentDevice];
        [myDevice setBatteryMonitoringEnabled:YES];
        float level = [myDevice batteryLevel];
        return level;
    }
	
#if defined (__cplusplus)
}
#endif








