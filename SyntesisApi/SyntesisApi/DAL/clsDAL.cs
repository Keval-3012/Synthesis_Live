using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace SyntesisApi.DAL
{
    public class clsDAL
    {
        private string GetConnectionString()
        {
            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString;
            return connstr;
        }

        private string GetConnectionStringNew()
        {
            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnNew"].ConnectionString;
            return connstr;
        }

        private string GetConnectionStringLog()
        {
            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnProductLog"].ConnectionString;
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

        internal int Insert_Update_Del(SqlCommand cmd)
        {
            int id = 0;
            try
            {
                string strConn = GetConnectionString();
                SqlConnection Con = new SqlConnection(strConn);
                cmd.Connection = Con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ids", 0).Direction = ParameterDirection.Output;
                Con.Open();
                cmd.ExecuteNonQuery();
                id = Convert.ToInt32(cmd.Parameters["@Ids"].Value.ToString());
                Con.Close();

            }
            catch (Exception ex) { ex.ToString(); }
            return id;
        }

        internal int Insert_Update_Del_Log(SqlCommand cmd)
        {
            int id = 0;
            try
            {
                string strConn = GetConnectionStringLog();
                SqlConnection Con = new SqlConnection(strConn);
                cmd.Connection = Con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ids", 0).Direction = ParameterDirection.Output;
                Con.Open();
                cmd.ExecuteNonQuery();
                id = Convert.ToInt32(cmd.Parameters["@Ids"].Value.ToString());
                Con.Close();

            }
            catch (Exception ex) { ex.ToString(); }
            return id;
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

        internal DataTable Select_New(SqlCommand cmd)
        {
            DataTable dt = null;
            try
            {
                string strConn = GetConnectionStringNew();
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

        internal DataSet SelectDataSet(SqlCommand cmd)
        {
            SqlConnection Con = new SqlConnection();
            DataSet dt = null;
            string strcon = "";
            strcon = GetConnectionStringNew();
            try
            {
                Con = new SqlConnection(strcon);
                cmd.Connection = new SqlConnection(strcon);
                cmd.CommandType = CommandType.StoredProcedure;
                dt = new DataSet();
                Con.Open();
                var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                Con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Con.Dispose();
                Con.Close();
            }
            return dt;
        }        

        internal DataTable SelectLog(SqlCommand cmd)
        {
            DataTable dt = null;
            try
            {
                string strConn = GetConnectionStringLog();
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

        internal int Insert_Update_Del_Log_Image(SqlCommand cmd)
        {
            int id = 0;
            try
            {
                string strConn = GetConnectionStringLog();
                using (SqlConnection Con = new SqlConnection(strConn))
                {
                    cmd.Connection = Con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter outputIdParam = new SqlParameter("@Ids", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output,
                        Value = 0
                    };
                    cmd.Parameters.Add(outputIdParam);
                    Con.Open();
                    cmd.ExecuteNonQuery();

                    object outputValue = cmd.Parameters["@Ids"].Value;
                    if (outputValue != null && outputValue != DBNull.Value)
                    {
                        id = Convert.ToInt32(outputValue);
                    }
                    Con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Insert_Update_Del_Log_Image: " + ex.Message);
            }
            return id;
        }
    }
}