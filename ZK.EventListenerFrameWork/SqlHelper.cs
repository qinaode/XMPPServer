using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ZK.EventFrameWork
{
    public static class SqlHelper
    {
        private static readonly string connstr = ConfigurationManager.ConnectionStrings["EventConnStr"].ToString();
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.AddRange(parameters);
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
        public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connstr);
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        internal static void ListenCommand(string sql, OnChangeEventHandler Event)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText =sql;
                    SqlDependency.Start(connstr);
                    SqlDependency dependency = new SqlDependency(cmd);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    dependency.OnChange += new OnChangeEventHandler(Event);
                }
            }
        }

        internal static void Add<T>(T entity)
        {
            StringBuilder sqlsb = new StringBuilder();
            List<string> PropertyNames = new List<string>();
            List<SqlParameter> psList = new List<SqlParameter>();
            //拼接属性名
            foreach (var item in typeof(T).GetProperties())
            {
                IdentityAttribute identity = Attribute.GetCustomAttribute(item, typeof(IdentityAttribute)) as IdentityAttribute;
                if (identity == null)
                {
                    psList.Add(new SqlParameter(item.Name, item.GetValue(entity, null)));
                    PropertyNames.Add(item.Name);
                }
            }
            //拼接属性值
            //string sql = "insert into producte(a,b,c) values(a,b,c)";
            sqlsb.Append("insert into " + GetTableName(typeof(T)) + "(");
            sqlsb.Append(string.Join(",", PropertyNames.ToArray()));
            sqlsb.Append(") values(");
            sqlsb.Append("@");
            sqlsb.Append(string.Join(",@", PropertyNames.ToArray()));
            sqlsb.Append(");select @@identity");
            try
            {
                SqlHelper.ExecuteScalar(sqlsb.ToString(), psList.ToArray());
            }
            catch (Exception ex)
            {
            }
        }

        public static List<T> GetModelList<T>(string sql)
        {
            SqlDataReader reader = SqlHelper.ExecuteReader(sql);
            List<T> ModelList = new List<T>();
            using (reader)
            {
                while (reader.Read())
                {
                    //利用反射创建对象
                    T entity =(T)Activator.CreateInstance(typeof(T));
                    foreach (var item in typeof(T).GetProperties())
                    {
                        //给属性赋值
                        item.SetValue(entity, reader[item.Name], null);
                    }
                    ModelList.Add(entity);
                }
            }
            return ModelList;
        }

        internal static string GetTableName(Type type)
        {
            string TableName;
            TableAttribute AttrTableName = Attribute.GetCustomAttribute(type, typeof(TableAttribute)) as TableAttribute;
            TableName = AttrTableName == null ? type.Name : AttrTableName.TableName;
            return TableName;
        }
        internal static string GetIdentityName(Type type)
        {
            foreach (var item in type.GetProperties())
            {
                IdentityAttribute identity = Attribute.GetCustomAttribute(item, typeof(IdentityAttribute)) as IdentityAttribute;
                if (identity != null && identity.IsIdentity)
                {
                    return item.Name;
                }
            }
            return "";
        }
    }
}