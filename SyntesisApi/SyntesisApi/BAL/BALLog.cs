using SyntesisApi.DAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SyntesisApi.BAL
{
    public class BALLog : clsLogDAL
    {
        public int WriteLog(LogMaster obj)
        {
            int id = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Sp_LogMaster";
                cmd.Parameters.AddWithValue("@Mode", "Insert");
                cmd.Parameters.AddWithValue("@ModuleName", obj.ModuleName);
                cmd.Parameters.AddWithValue("@PageName", obj.PageName);
                cmd.Parameters.AddWithValue("@Message", obj.Message);
                cmd.Parameters.AddWithValue("@CreatedBy", obj.CreatedBy);
                cmd.Parameters.AddWithValue("@Action", obj.Action);
                Insert_Update_Delete(cmd);
            }
            catch (Exception ex)
            {
            }
            return id;
        }

    }
}