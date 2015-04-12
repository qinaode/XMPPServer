using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZK.EventFrameWork
{
    [TableAttribute(TableName = "LE_EventList")]
    public class EventModel
    {
        [Identity(IsIdentity=true)]
        public Guid EventID { get; set; }
        public string EventContent { get; set; }
        public EventType EventType { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
