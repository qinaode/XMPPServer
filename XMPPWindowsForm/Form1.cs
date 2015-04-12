using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using agsXMPP.Xml.Dom;
using XMPPServer;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using ZK.Share;
namespace XMPPWindowsForm
{
    public partial class Form1 : Form
    {
        private static Socket listenSocket;//监听服务的Socket
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
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
        }
        private static void AcceptClientConnect()
        {
            while (true)
            {
                Socket proxSocket = listenSocket.Accept();
                XmppSeverConnection con = new XmppSeverConnection(proxSocket);
            }
        }
    }
}
