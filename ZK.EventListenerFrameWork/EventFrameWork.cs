using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace ZK.EventFrameWork
{
    public static class EventFrameWork
    {
        //public static void Fire(EventType eventtype, string content)
        //{
        //    #region 这里需要改进为事件对象(EventModel)
        //    string sql = "insert into dbo.a values(newid(),@Name,@Type)";
        //    List<SqlParameter> ps = new List<SqlParameter>();
        //    ps.Add(new SqlParameter("@Name", content));
        //    ps.Add(new SqlParameter("@Type", (int)eventtype));
        //    #endregion
        //    try
        //    {
        //        SqlHelper.ExecuteNonQuery(sql, ps.ToArray());
        //    }
        //    catch (Exception)
        //    {

        //    }
        //}
        public static void Fire<T>(T EventModel)
        {
            SqlHelper.Add(EventModel);
        }

        public static void Listen<T>(OnChangeEventHandler Event,T EventModel)
        {
            string sql = "select " + SqlHelper.GetIdentityName(EventModel.GetType()) + " From [dbo].[" + SqlHelper.GetTableName(EventModel.GetType())+"]";
            SqlHelper.ListenCommand(sql,Event);
        }
    }

    public enum EventType
    {
        [Description("点赞")]
        Prase = 0,
        [Description("评论")]
        Comment = 1,
        [Description("新动态")]
        NewDynamic = 2,
        [Description("提问")]
        Question = 3,
        [Description("回答问题")]
        Answer = 4
    }
}
