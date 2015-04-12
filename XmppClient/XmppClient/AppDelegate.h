//
//  AppDelegate.h
//  XmppClient
//
//  Created by qinao on 15/4/11.
//  Copyright (c) 2015年 qinao. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "XMPP.h"
#import "XMPPReconnect.h"
@interface AppDelegate : UIResponder <UIApplicationDelegate,XMPPStreamDelegate>

@property (strong, nonatomic) UIWindow *window;


@end

