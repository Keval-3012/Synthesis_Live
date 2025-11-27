using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace SyntesisApi.DAL
{
    public class clsLogDAL
    {
        private string GetConnectionString()
        {
            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnLog"].ConnectionString;
            return connstr;
        }

        internal void Insert_Update_Delete(SqlCommand cmd)
        {
            try
            {
                string strConn = GetConnectionString();
                SqlConnection Con = new SqlConnection(strConn);
                cmd.Connection = Con;
                cmd.CommandType = CommandType.StoredProcedure;
                Con.Open();
                cmd.ExecuteNonQuery();
                Con.Close();
            }
            catch (Exception ex) { ex.ToString(); }
        }

        internal DataTable Select(SqlCommand cmd)
        {
            DataTable dt = null;
            try
            {
                string strConn = GetConnectionString();
                SqlConnection conn = new SqlConnection(strConn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;

                dt = new DataTable();
                SqlDataReader dr;
                conn.Open();
                dr = cmd.ExecuteReader();
                dt.Load(dr);
                conn.Close();
            }
            catch (Exception ex) { ex.ToString(); }
            return dt;
        }

        internal DataSet SelectSet(SqlCommand cmd)
        {
            DataSet dt = null;
            try
            {
                string strConn = GetConnectionString();
                SqlConnection conn = new SqlConnection(strConn);
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;

                dt = new DataSet();
                conn.Open();
                var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
            }
            catch (Exception ex) { ex.ToString(); }
            return dt;
        }
    }
}