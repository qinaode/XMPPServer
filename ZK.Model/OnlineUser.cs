using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZK.Model
{
    public class OnlineUser : System.MarshalByRefObject
    {
        public static List<User> Users = new List<User>();
        public static string GetUsers()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Users)
            {
                sb.Append(item.Address + "\r\n");
            }
            return sb.ToString();
        }
        public List<User> GetUser()
        {
            return Users;
        }
    }
}
