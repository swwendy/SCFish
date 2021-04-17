//
//  AppPayDelegate.m
//
//  Created by Batcel on 17-07-11

#import "AppPayDelegate.h"
#import <Foundation/Foundation.h>

@implementation AppPayDelegate

-(BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];
    [WXApi registerApp:@"wx043094aaa5449b38" enableMTA:false];
    return YES;
    
}

- (BOOL)application:(UIApplication *)application
            openURL:(NSURL *)url
  sourceApplication:(NSString *)sourceApplication
         annotation:(id)annotation
{

	//微信回调处理
    {
        
        [WXApi handleOpenURL:url delegate:self];
    }

    return YES;
}

// NOTE: 9.0以后使用新API接口
- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<NSString *,id> *)options
{   
	//微信回调处理
    {
        
         [WXApi handleOpenURL:url delegate:self];
    }
    return YES;
}

- (void) onReq:(BaseReq *)req
{
    
}

#pragma mark - WXApiDelegate
- (void)onResp:(BaseResp*)resp
{
    if ([resp isKindOfClass:[PayResp class]])
    {
        PayResp* response=(PayResp*)resp;
        NSString *errCodeStr = [NSString stringWithFormat:@"%d",resp.errCode];
        const char* result = [errCodeStr UTF8String];
        UnitySendMessage("gamemain", "WXPayCallback", result);
        switch(response.errCode)
        {
            case WXSuccess:
                //服务器端查询支付通知或查询API返回的结果再提示成功
                //NSlog(@"支付成功");
                break;
            default:
                //NSlog(@"支付失败，retcode=%d",resp.errCode);
                break;
        }
    }
    else if([resp isKindOfClass:[SendMessageToWXResp class]])
    {
        SendMessageToWXResp* response = (SendMessageToWXResp*)resp;
        NSString *errCodeStr = [NSString stringWithFormat:@"%d",resp.errCode];
        const char* result = [errCodeStr UTF8String];
        UnitySendMessage("gamemain", "WXShareCallback", result);
        switch(response.errCode)
        {
            case WXSuccess:
                //printf_console("分享成功");
                break;
            default:
                //printf_console("分享失败，retcode=%d",resp.errCode);
                break;
        }
    }
	else if([resp isKindOfClass:[SendAuthResp class]])
    {
        SendAuthResp* response = (SendAuthResp*)resp;
        switch(response.errCode)
        {
            case WXSuccess:
            {
                //printf_console("用户同意");
                const char* codestr = [response.code UTF8String];
                UnitySendMessage("gamemain", "WXAuthCallback", codestr);
            }
                break;
            default:
            {
                //printf_console("用户取消或拒绝，retcode=%d",resp.errCode);
                NSString *errCodeStr = [NSString stringWithFormat:@"%d",resp.errCode];
                const char* result = [errCodeStr UTF8String];
                UnitySendMessage("gamemain", "WXAuthCallback", result);
            }
                 break;
        }
    }
}

@end
