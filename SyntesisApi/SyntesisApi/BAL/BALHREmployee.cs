using SyntesisApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.BAL
{
    public class BALHREmployee
    {
        clsDAL db = new clsDAL();
        #region Start Harsh
        public DataTable GetEmployeeList()
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_HRApiEmployee";
                Cmd.Parameters.AddWithValue("@Mode", "EmployeeList");
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetEmployeeByStoreId(int StoreID)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_HRApiEmployee";
                Cmd.Parameters.AddWithValue("@Mode", "EmployeeByStoreId");
                Cmd.Parameters.AddWithValue("@StoreID", StoreID);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetEmployeeById(int EmployeeId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_HRApiEmployee";
                Cmd.Parameters.AddWithValue("@Mode", "EmployeebyId");
                Cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetSignedUnsignedDoc(Store obj)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_HRApiSignedUnsignedDocument";
                Cmd.Parameters.AddWithValue("@Mode", "GetDoc");
                Cmd.Parameters.AddWithValue("@StoreID", obj.StoreID);
                Cmd.Parameters.AddWithValue("@FromDate", obj.FromDate);
                Cmd.Parameters.AddWithValue("@ToDate", obj.ToDate);
                Cmd.Parameters.AddWithValue("@Flag", obj.Sign_UnSigned);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
        #endregion
    }
}