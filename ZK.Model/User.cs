using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace ZK.Model
{
    /// <summary>
    /// 当手机端登录与Xmpp服务器建立连接后，保存用户的Id和连接Socket
    /// </summary>
    public class User
    {
        public Socket ConnSocket { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
    }
}
