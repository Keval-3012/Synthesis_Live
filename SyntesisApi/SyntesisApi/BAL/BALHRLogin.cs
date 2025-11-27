using SyntesisApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using SyntesisApi.Models;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.BAL
{
    public class BALHRLogin
    {
        clsDAL db = new clsDAL();
        public DataTable CheckUserLogin(CheckLogin objLogin)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_HRApiLogin";
                Cmd.Parameters.AddWithValue("@UserName", objLogin.UserName);
                Cmd.Parameters.AddWithValue("@Password", objLogin.Password);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public DataTable GetUserDetail(int UserId, string UserType)
        {
            DataTable dt = null;    
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_HRGetUserLoginDetail";
                Cmd.Parameters.AddWithValue("@UserID", UserId);
                Cmd.Parameters.AddWithValue("@UserType", UserType);
                dt = new DataTable();
                dt = db.Select(Cmd);
            }
            catch (Exception ex)
            {
            }
            return dt;
        }

        public UserDetail GetUserDetailProducts(int UserId, string UserType)
        {
            DataTable dt = null;
            UserDetail user = new UserDetail();
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_ProductGetUserLoginDetail";
                Cmd.Parameters.AddWithValue("@UserID", UserId);
                Cmd.Parameters.AddWithValue("@UserType", UserType);
                dt = new DataTable();
                dt = db.Select(Cmd);
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    user.Name = dt.Rows[0]["Name"].ToString();
                    user.UserName = dt.Rows[0]["UserName"].ToString();
                    //user.CompetitorsName1 = dt.Rows[0]["CompetitorsName1"].ToString();
                    //user.CompetitorsName2 = dt.Rows[0]["CompetitorsName2"].ToString();
                    //user.Zipcode = dt.Rows[0]["Zipcode"].ToString();
                    user.UserId = !String.IsNullOrEmpty(dt.Rows[0]["UserId"].ToString()) ? Convert.ToInt32(dt.Rows[0]["UserId"]) : 0;
                    user.UserTypeId = !String.IsNullOrEmpty(dt.Rows[0]["UserTypeId"].ToString()) ? Convert.ToInt32(dt.Rows[0]["UserTypeId"]) : 0;
                    user.UserRightsforStoreAccess = dt.Rows[0]["UserRightsforStoreAccess"].ToString();
                    user.ProductImageUpload = !String.IsNullOrEmpty(dt.Rows[0]["ProductImageUpload"].ToString()) ? Convert.ToInt32(dt.Rows[0]["ProductImageUpload"]) : 0;
                    user.UpdateProductDetails = !String.IsNullOrEmpty(dt.Rows[0]["UpdateProductDetails"].ToString()) ? Convert.ToInt32(dt.Rows[0]["UpdateProductDetails"]) : 0;
                    user.IsAbleExpiryChange = !String.IsNullOrEmpty(dt.Rows[0]["IsAbleExpiryChange"].ToString()) ? Convert.ToInt32(dt.Rows[0]["IsAbleExpiryChange"]) : 0;
                    user.DesignatedStore = !String.IsNullOrEmpty(dt.Rows[0]["DesignatedStore"].ToString()) ? Convert.ToInt32(dt.Rows[0]["DesignatedStore"]) : 0;
                    //user.LocationAccess = dt.Rows[0]["LocationAccess"].ToString();
                    //user.ViewStoreDataAccess = dt.Rows[0]["ViewStoreDataAccess"].ToString();
                    user.StoreDetails = new List<StoreDetail>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        StoreDetail store = new StoreDetail();
                        store.StoreId = !String.IsNullOrEmpty(dt.Rows[i]["StoreId"].ToString()) ? Convert.ToInt32(dt.Rows[i]["StoreId"]) : 0;
                        store.StoreName = dt.Rows[i]["StoreName"].ToString();
                        store.StoreNickName = dt.Rows[i]["StoreNickName"].ToString();
                        store.StoreAddress = dt.Rows[i]["StoreAddress"].ToString();
                        store.Latitude = double.TryParse(dt.Rows[i]["Latitude"].ToString(), out var lat) ? lat : 0.0;
                        store.Longitude = double.TryParse(dt.Rows[i]["Longitude"].ToString(), out var lon) ? lon : 0.0;
                        store.Radius = !String.IsNullOrEmpty(dt.Rows[i]["Radius"].ToString()) ? Convert.ToInt32(dt.Rows[i]["Radius"]) : 0;
                        store.LocationAccess = !String.IsNullOrEmpty(dt.Rows[i]["LocationAccess"].ToString()) ? Convert.ToInt32(dt.Rows[i]["LocationAccess"]) : 0;
                        store.ViewStoreDataAccess = !String.IsNullOrEmpty(dt.Rows[i]["ViewStoreDataAccess"].ToString()) ? Convert.ToInt32(dt.Rows[i]["ViewStoreDataAccess"]) : 0;
                        user.StoreDetails.Add(store);
                    }

                }
            }
            catch (Exception ex)
            {                
            }
            return user;
        }

        public UserDetail GetALLUserDetails(int UserId)
        {
            DataTable dt = null;
            UserDetail user = new UserDetail();
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "GetAllUserDetails");
                Cmd.Parameters.AddWithValue("@UserID", UserId);
                dt = new DataTable();
                dt = db.Select(Cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    user.Name = dt.Rows[0]["Name"].ToString();
                    //user.CompetitorsName1 = dt.Rows[0]["CompetitorsName1"].ToString();
                    //user.CompetitorsName2 = dt.Rows[0]["CompetitorsName2"].ToString();
                    //user.Zipcode = dt.Rows[0]["Zipcode"].ToString();
                    user.UserName = dt.Rows[0]["UserName"].ToString();
                    user.UserId = !String.IsNullOrEmpty(dt.Rows[0]["UserId"].ToString()) ? Convert.ToInt32(dt.Rows[0]["UserId"]) : 0;
                    user.UserTypeId = !String.IsNullOrEmpty(dt.Rows[0]["UserTypeId"].ToString()) ? Convert.ToInt32(dt.Rows[0]["UserTypeId"]) : 0;
                    user.ProductImageUpload = !String.IsNullOrEmpty(dt.Rows[0]["ProductImageUpload"].ToString()) ? Convert.ToInt32(dt.Rows[0]["ProductImageUpload"]) : 0;
                    user.UpdateProductDetails = !String.IsNullOrEmpty(dt.Rows[0]["UpdateProductDetails"].ToString()) ? Convert.ToInt32(dt.Rows[0]["UpdateProductDetails"]) : 0;
                    user.IsAbleExpiryChange = !String.IsNullOrEmpty(dt.Rows[0]["IsAbleExpiryChange"].ToString()) ? Convert.ToInt32(dt.Rows[0]["IsAbleExpiryChange"]) : 0;
                    user.DesignatedStore = !String.IsNullOrEmpty(dt.Rows[0]["DesignatedStore"].ToString()) ? Convert.ToInt32(dt.Rows[0]["DesignatedStore"]) : 0;
                    user.UserRightsforStoreAccess = dt.Rows[0]["UserRightsforStoreAccess"].ToString();                    
                    //user.LocationAccess = dt.Rows[0]["LocationAccess"].ToString();                    
                    //user.ViewStoreDataAccess = dt.Rows[0]["ViewStoreDataAccess"].ToString();
                    user.StoreDetails = new List<StoreDetail>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        StoreDetail store = new StoreDetail();
                        store.StoreId = !String.IsNullOrEmpty(dt.Rows[i]["StoreId"].ToString()) ? Convert.ToInt32(dt.Rows[i]["StoreId"]) : 0;
                        store.StoreName = dt.Rows[i]["StoreName"].ToString();
                        store.StoreNickName = dt.Rows[i]["StoreNickName"].ToString();
                        store.StoreAddress = dt.Rows[i]["StoreAddress"].ToString();
                        store.Latitude = double.TryParse(dt.Rows[i]["Latitude"].ToString(), out var lat) ? lat : 0.0;
                        store.Longitude = double.TryParse(dt.Rows[i]["Longitude"].ToString(), out var lon) ? lon : 0.0;
                        //store.Latitude = dt.Rows[i]["Latitude"].ToString();
                        //store.Longitude = dt.Rows[i]["Longitude"].ToString();
                        store.Radius = !String.IsNullOrEmpty(dt.Rows[i]["Radius"].ToString()) ? Convert.ToInt32(dt.Rows[i]["Radius"]) : 0;
                        store.LocationAccess = !String.IsNullOrEmpty(dt.Rows[i]["LocationAccess"].ToString()) ? Convert.ToInt32(dt.Rows[i]["LocationAccess"]) : 0;
                        store.ViewStoreDataAccess = !String.IsNullOrEmpty(dt.Rows[i]["ViewStoreDataAccess"].ToString()) ? Convert.ToInt32(dt.Rows[i]["ViewStoreDataAccess"]) : 0;
                        user.StoreDetails.Add(store);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return user;
        }

        public DataTable CheckProductUserLogin(CheckLogin objLogin)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "SP_GetItemListBasedonItemCode";
                Cmd.Parameters.AddWithValue("@Mode", "UserLoginCheck");
                Cmd.Parameters.AddWithValue("@UserName", objLogin.UserName);
                Cmd.Parameters.AddWithValue("@Password", objLogin.Password);
                Cmd.Parameters.AddWithValue("@FCMTokenApp", objLogin.FCMTokenApp);
                Cmd.Parameters.AddWithValue("@PlayerId", objLogin.PlayerId);
                Cmd.Parameters.AddWithValue("@DeviceType", objLogin.DeviceType);
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