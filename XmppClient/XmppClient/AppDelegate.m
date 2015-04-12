//
//  AppDelegate.m
//  XmppClient
//
//  Created by qinao on 15/4/11.
//  Copyright (c) 2015年 qinao. All rights reserved.
//

#import "AppDelegate.h"
#import "MainController.h"
@interface AppDelegate ()
{
    XMPPStream *stream;

}
@end

@implementation AppDelegate


- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
    self.window= [[UIWindow alloc]initWithFrame:[UIScreen mainScreen].bounds];
    self.window.rootViewController=[[MainController alloc]init];
    [self.window makeKeyAndVisible];
    [self connXmppServer];// 连接XMPPServer
    return YES;
}

-(void)connXmppServer
{
   
    //读取用户信息
   // NSDictionary *info=[Tool getUserBox:@"info"];
    NSString *JID=[NSString stringWithFormat:@"%@@hoolian.com",@"aaaa"];
    
    stream = [[XMPPStream alloc]init];
    //注册断开自动重连模块
    XMPPReconnect *reconnect = [[XMPPReconnect alloc]init];
    [reconnect activate:stream];
    [stream addDelegate:self delegateQueue:dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0)];
    [stream setMyJID:[XMPPJID jidWithString:JID]];
    [stream setHostName:@"58.116.26.14"];
    [stream connectWithTimeout:XMPPStreamTimeoutNone error:nil];
}
-(void)xmppStreamDidConnect:(XMPPStream *)sender
{
    //这里密码任意，因为XMPPServer不需要在次验证
    [stream authenticateWithPassword:@"123456" error:nil];
}
-(void)xmppStreamDidAuthenticate:(XMPPStream *)sender
{
    NSLog(@"登录成功");
}
-(void)xmppStream:(XMPPStream *)sender didReceiveMessage:(XMPPMessage *)message
{
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alert = [[UIAlertView alloc]init];
        [alert addButtonWithTitle:message.body];
        [alert show];
    });
}

@end
