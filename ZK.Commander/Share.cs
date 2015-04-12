using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using agsXMPP.protocol.component;
using System.Net.Sockets;
using agsXMPP.Xml.Dom;
using System.IO;
using System.Reflection;
using ZK.Model;
namespace ZK.Share
{

    /// <summary>
    /// 提供消息推送的接口类
    /// </summary>
    public class Commander : System.MarshalByRefObject
    {
        #region 对外接口
        /// <summary>
        /// 推送指定内容的通知
        /// </summary>
        /// <param name="body"></param>
        public void  PushNotify(string body)
        {
           
            Message msg = new Message();
            msg.Body = body;
            msg.From = new agsXMPP.Jid("server");
            foreach (var user in OnlineUser.Users)
            {
                Send(user.ConnSocket, msg);
            }
        }

        /// <summary>
        /// 推送消息给指定用户
        /// </summary>
        /// <param name="body"></param>
        /// <param name="ids"></param>
        public void PushNotify(string body, string ids)
        {

            var idarray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var id in idarray)
            {
                var user = OnlineUser.Users.Where(u => u.UserId == int.Parse(id)).FirstOrDefault();
                //如果用户不在线(目前唯一做的是调用APNS服务推送消息)
                if (user == null)
                {
                   // PushAPPNotify(id, body);
                }
                else
                {
                    Message msg = new Message();
                    msg.Body = body;
                    msg.From = new agsXMPP.Jid("server");
                    Send(user.ConnSocket, msg);
                }
            }
        }
        #endregion


        #region 辅助方法
        //采用异步方式给客户端发送数据
        private void Send(Socket socket, string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), null);
        }
        private void Send(Socket socket, Element el)
        {
            Send(socket, el.ToString());
        }

        //数据发送完成回调函数(执行这个函数代表数据发送完毕，但不代表成功发送)
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                //这里代表发送成功
            }
            catch (Exception e)
            {
               
            }
        }
        #endregion
    }
}
