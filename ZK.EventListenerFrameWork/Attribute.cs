using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZK.EventFrameWork
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; set; }
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class IdentityAttribute : Attribute
    {
        public bool IsIdentity { get; set; }
    }
}
