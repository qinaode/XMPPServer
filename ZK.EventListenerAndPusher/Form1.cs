using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZK.EventFrameWork;
using System.Data.SqlClient;
using ZK.Share;
namespace ZK.EventListenerAndPusher
{
    public partial class Form1 : Form
    {
        private static Commander remoting = (Commander)Activator.GetObject(typeof(Commander), "tcp://58.116.26.14:5223/Mobile");
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_prase_Click(object sender, EventArgs e)
        {
            EventFrameWork.EventFrameWork.Fire(new EventModel() {  EventContent="耍流氓", EventID=Guid.NewGuid(), EventType=EventType.Prase,CreateTime=DateTime.Now});
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EventFrameWork.EventFrameWork.Listen(ReciveEvent, new EventModel() { });
            
        }

        private void ReciveEvent(object sender, SqlNotificationEventArgs e)
        {
            EventFrameWork.EventFrameWork.Listen(ReciveEvent, new EventModel() { });
            if (e.Info==SqlNotificationInfo.Insert)
            {
                //执行相关推送业务
                //读出数据库事件
               // var reader = SqlHelper.ExecuteReader("select * from dbo.EventQueue");
                List<EventModel> eventmodellist = SqlHelper.GetModelList<EventModel>("select * from dbo.LE_EventQueue");
                foreach (var  item in eventmodellist)
                {
                    remoting.PushNotify(item.EventContent);
                    SqlHelper.ExecuteNonQuery("delete from dbo.LE_EventQueue where EventID='"+item.EventID+"'");
                }
               // remoting.PushNotify();
            }
        }
    }
}
