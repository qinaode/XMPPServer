using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using agsXMPP.protocol;
using agsXMPP.protocol.iq;
using agsXMPP.protocol.iq.auth;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.client;
using agsXMPP.Xml;
using agsXMPP.Xml.Dom;
using agsXMPP;
using System.Net.Sockets;
using ZK.Model;

namespace XMPPServer
{
    public class XmppSeverConnection
    {
        #region 成员变量和属性
        private string m_SessionId = null;
       
        //流转换器,核心对象
        private StreamParser streamParser;
        private Socket m_Sock;
        private const int BUFFERSIZE = 1024;
        //缓存区
        private byte[] buffer = new byte[BUFFERSIZE];
        public string SessionId
        {
            get
            {
                return m_SessionId;
            }
            set
            {
                m_SessionId = value;
            }
        }
        #endregion
        #region 构造函数
        public XmppSeverConnection()
        {
            streamParser = new StreamParser();
            //流开始事件
            streamParser.OnStreamStart += new StreamHandler(streamParser_OnStreamStart);
            //流结束事件
            streamParser.OnStreamEnd += new StreamHandler(streamParser_OnStreamEnd);
            //节点处理事件
            streamParser.OnStreamElement += new StreamHandler(streamParser_OnStreamElement);

        }

        public XmppSeverConnection(Socket sock)
            : this()
        {
            m_Sock = sock;
            m_Sock.BeginReceive(buffer, 0, BUFFERSIZE, 0, new AsyncCallback(ReadCallback), null);
        }
        #endregion


        //处理客户端请求并接受客户端发送过来的数据。
        public void ReadCallback(IAsyncResult ar)
        {
            try
            {
                int bytesRead = m_Sock.EndReceive(ar);

                if (bytesRead > 0)
                {
                    streamParser.Push(buffer, 0, bytesRead);
                    // Not all data received. Get more.
                    m_Sock.BeginReceive(buffer, 0, BUFFERSIZE, 0, new AsyncCallback(ReadCallback), null);
                }
                else //客户端发送0字节的数据表示断开连接
                {
                    OnlineUser.Users.Remove(OnlineUser.Users.Where(m => m.Address == m_Sock.RemoteEndPoint.ToString()).FirstOrDefault());
                    m_Sock.Shutdown(SocketShutdown.Both);
                    m_Sock.Close();
                }
            }
            catch (Exception)
            {
                OnlineUser.Users.Remove(OnlineUser.Users.Where(m => m.Address == m_Sock.RemoteEndPoint.ToString()).FirstOrDefault());
                m_Sock.Shutdown(SocketShutdown.Both);
                m_Sock.Close();
            }
        }
        //采用异步方式给客户端发送数据
        private void Send(string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            m_Sock.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), null);
        }
        private void Send(Element el)
        {
            Send(el.ToString());
        }
        //数据发送完成回调函数(执行这个函数代表数据发送完毕，但不代表成功发送)
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Complete sending the data to the remote device.
                //这里代表发送成功
                int bytesSent = m_Sock.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void Stop()
        {
            Send("</stream:stream>");
            m_Sock.Shutdown(SocketShutdown.Both);
            m_Sock.Close();
        }


        //客户端发送连接请求时触发此事件
        private void streamParser_OnStreamStart(object sender, Node e)
        {
            SendOpenStream();
        }

        private void streamParser_OnStreamEnd(object sender, Node e)
        {

        }
        //节点处理
        private void streamParser_OnStreamElement(object sender, Node e)
        {
            if (e.GetType() == typeof(Presence))
            {
                // route presences here and handle all subscription stuff
            }
            else if (e.GetType() == typeof(Message))
            {
                // route the messages here

            }
            else if (e.GetType() == typeof(IQ))
            {
                ProcessIQ(e as IQ);
            }
        }

        private void ProcessIQ(IQ iq)
        {
            if (iq.Query.GetType() == typeof(Auth))
            {
                Auth auth = iq.Query as Auth;
                switch (iq.Type)
                {
                    //响应用户验证请求
                    case IqType.get:
                        iq.SwitchDirection();
                        iq.Type = IqType.result;
                        auth.AddChild(new Element("password"));
                        auth.AddChild(new Element("digest"));
                        Send(iq);
                        break;
                    //进行用户验证
                    case IqType.set:
                        //认证完成后,将建立的socket连接保存到集合
                        OnlineUser.Users.Add(new User
                        {
                            UserName = auth.Username,
                            UserId = 0,
                            ConnSocket = m_Sock,
                            Address = m_Sock.RemoteEndPoint.ToString()
                        });
                        iq.SwitchDirection();
                        iq.Type = IqType.result;
                        iq.Query = null;
                        Send(iq);
                        break;
                }


            }
            else if (iq.Query.GetType() == typeof(Roster))
            {
                ProcessRosterIQ(iq);

            }

        }

        private void ProcessRosterIQ(IQ iq)
        {
            if (iq.Type == IqType.get)
            {
                iq.SwitchDirection();
                iq.Type = IqType.result;
                for (int i = 1; i < 11; i++)
                {
                    RosterItem ri = new RosterItem();
                    ri.Name = "Item " + i.ToString();
                    ri.Subscription = SubscriptionType.both;
                    ri.Jid = new Jid("item" + i.ToString() + "@localhost");
                    ri.AddGroup("localhost");
                    iq.Query.AddChild(ri);
                }
                for (int i = 1; i < 11; i++)
                {
                    RosterItem ri = new RosterItem();
                    ri.Name = "Item JO " + i.ToString();
                    ri.Subscription = SubscriptionType.both;
                    ri.Jid = new Jid("item" + i.ToString() + "@jabber.org");
                    ri.AddGroup("JO");
                    iq.Query.AddChild(ri);
                }
                Send(iq);
            }
        }

        private void SendOpenStream()
        {
            string ServerDomain = "hoolian.com";

            this.SessionId = Guid.NewGuid().ToString();

            StringBuilder sb = new StringBuilder();

            sb.Append("<stream:stream from='");
            sb.Append(ServerDomain);

            sb.Append("' xmlns='");
            sb.Append(agsXMPP.Uri.CLIENT);

            sb.Append("' xmlns:stream='");
            sb.Append(agsXMPP.Uri.STREAM);

            sb.Append("' id='");
            sb.Append(this.SessionId);
            sb.Append("'>");

            Send(sb.ToString());
        }
    }
}
