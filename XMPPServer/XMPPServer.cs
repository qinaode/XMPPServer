using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using ZK.Share;

namespace XMPPServer
{
    public partial class XMPPServer : ServiceBase
    {
        private static Socket listenSocket;//监听服务的Socket
        protected override void OnStart(string[] args)
        {
            System.IO.File.AppendAllText("log.txt", "server start->" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\r\n"));
            //创建一个Socket
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 5222);
            listenSocket.Bind(endPoint);
            //开始监听
            listenSocket.Listen(10);
            Thread acceptThread = new Thread(AcceptClientConnect);
            acceptThread.Start();
            #region 注册.net remotin用于通讯
            var channel = new TcpServerChannel(5223);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Commander), "Mobile", WellKnownObjectMode.SingleCall);
            #endregion
            // Console.WriteLine("server is running");
        }
        private static void AcceptClientConnect()
        {
            while (true)
            {
                try
                {
                    Socket proxSocket = listenSocket.Accept();
                    XmppSeverConnection con = new XmppSeverConnection(proxSocket);
                }
                catch (Exception ex)
                {
                    //记录日志
                    System.IO.File.AppendAllText("log.txt",ex.Message+"->"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\r\n"));
                }
            }
        }

        protected override void OnStop()
        {
            System.IO.File.AppendAllText("log.txt", "server stop->"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\r\n"));
        }
    }
}
