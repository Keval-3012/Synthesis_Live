using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using SyntesisApi.DAL;
using SyntesisApi.Models;

namespace SyntesisApi.BAL
{
    public class BALAccount
    {
        clsDAL db = new clsDAL();
        clsLogDAL logDAL = new clsLogDAL();
        public DataTable GetData(string UserName)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_UserMaster";
                Cmd.Parameters.AddWithValue("@Mode", "SelectUserByName");
                Cmd.Parameters.AddWithValue("@UserName", UserName.Trim());
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetUserLogDetails(UserLogDetails UserLogDetails)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetUserLogdetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetUserLogdetails");
                Cmd.Parameters.AddWithValue("@UserID", UserLogDetails.UserID);
                Cmd.Parameters.AddWithValue("@StartDate", UserLogDetails.StartDate);
                Cmd.Parameters.AddWithValue("@EndDate", UserLogDetails.EndDate);
                Cmd.Parameters.AddWithValue("@ModuleName", UserLogDetails.ModuleName);
                Cmd.Parameters.AddWithValue("@ActionName", UserLogDetails.ActionName);
                dt = new DataTable();
               
                dt = logDAL.Select(Cmd);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable GetUser()
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetUserLogdetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetUser");
                dt = new DataTable();
                dt = logDAL.Select(Cmd);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable GetStore()
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetStoredetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetStore");
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable GetProductDetails(int PageNumber = 1)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetStoredetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetProductDetails");
                Cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable GetVendorProductDetails(string VendorName, int PageNumber = 1)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetStoredetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetProductVendorsDetails");
                Cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                Cmd.Parameters.AddWithValue("@VendorName", VendorName);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable GetVendorName(int StoreId =0)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetStoredetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetVendor");
                Cmd.Parameters.AddWithValue("@StoreID", StoreId);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable GetSales(int StoreId = 0, string StartDate = "", string EndDate = "", int PageNumber = 1)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetStoredetails";
                
                Cmd.Parameters.AddWithValue("@Mode", "GetSales");
                Cmd.Parameters.AddWithValue("@StoreID", StoreId);
                Cmd.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(StartDate));
                Cmd.Parameters.AddWithValue("@EndDate", Convert.ToDateTime(EndDate));
                Cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable GetPurchase(int StoreId = 0, int VendorID = 0, string StartDate = "", string EndDate = "", int PageNumber = 1)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetStoredetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetPurchase");
                Cmd.Parameters.AddWithValue("@StoreID", StoreId);
                Cmd.Parameters.AddWithValue("@VendorID", VendorID);
                Cmd.Parameters.AddWithValue("@StartDate", Convert.ToDateTime(StartDate));
                Cmd.Parameters.AddWithValue("@EndDate", Convert.ToDateTime(EndDate));
                Cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }
        public int GetStoreIDbyName(string StoreName = "")
        {
            DataTable dt = null;
            int ID = 0;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetStoredetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetStoreIDbyName");
                Cmd.Parameters.AddWithValue("@StoreName", (StoreName == "") ? null : StoreName);
                dt = new DataTable();
                dt = db.Select(Cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ID = Convert.ToInt32(dt.Rows[0]["StoreId"]);
                }
            }
            catch (Exception ex)
            {

            }
            return ID;
        }
        public int GetVendorIDbyName(string VendorName = "", int StoreID = 0)
        {
            DataTable dt = null;
            int ID = 0;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetStoredetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetVendorIDbyName");
                Cmd.Parameters.AddWithValue("@VendorName", (VendorName == "") ? null : VendorName);
                Cmd.Parameters.AddWithValue("@StoreID", (StoreID == 0) ? null : StoreID.ToString());
                dt = new DataTable();
                dt = db.Select(Cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ID = Convert.ToInt32(dt.Rows[0]["VendorId"]);
                }
            }
            catch (Exception ex)
            {

            }
            return ID;
        }

        public DataSet GetModuleandAction()
        {
            DataSet dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "GetUserLogdetails";
                Cmd.Parameters.AddWithValue("@Mode", "GetModuleandAction");
                dt = new DataSet();
                dt = logDAL.SelectSet(Cmd);
            }
            catch (Exception ex)
            {

            }
            return dt;
        }
        public DataTable GetuserDetailsNewChild(UserLogDetails UserLogDetails)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GETModuleWiseLog";
                Cmd.Parameters.AddWithValue("@Mode", "GetChildGrid");
                Cmd.Parameters.AddWithValue("@UserID", UserLogDetails.UserID);
                Cmd.Parameters.AddWithValue("@StartDate", UserLogDetails.StartDate);
                Cmd.Parameters.AddWithValue("@EndDate", UserLogDetails.EndDate);
                Cmd.Parameters.AddWithValue("@ModuleName", UserLogDetails.ModuleName);
                dt = new DataTable();
                dt = logDAL.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
        public DataTable GetuserDetailsNew(UserLogDetails UserLogDetails)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GETModuleWiseLog";
                Cmd.Parameters.AddWithValue("@Mode", "GetParentGrid");
                Cmd.Parameters.AddWithValue("@UserID", UserLogDetails.UserID);
                Cmd.Parameters.AddWithValue("@ActivityDate", UserLogDetails.StartDate);
                Cmd.Parameters.AddWithValue("@ModuleName", UserLogDetails.ModuleName);
                dt = new DataTable();
                dt = logDAL.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }
    }
}