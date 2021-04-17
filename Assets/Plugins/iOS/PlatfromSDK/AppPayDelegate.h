//
//  AppPayDelegate.h
//
//  Created by Batcel on 17-07-11


#import <UIKit/UIKit.h>
#import "WXApi.h"

#import "UnityAppController.h"

@interface AppPayDelegate: UnityAppController<WXApiDelegate>{}

@end
IMPL_APP_CONTROLLER_SUBCLASS(AppPayDelegate)
