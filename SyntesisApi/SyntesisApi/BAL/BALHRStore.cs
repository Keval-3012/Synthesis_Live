using SyntesisApi.DAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace SyntesisApi.BAL
{
    public class BALHRStore
    {
        clsDAL db = new clsDAL();
        public DataTable GetStoreList()
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_HRApiStore";
                Cmd.Parameters.AddWithValue("@Mode", "StoreList");
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetStoreListData(int UserId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllStoreList");
                Cmd.Parameters.AddWithValue("@UserID", UserId);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
    }
}