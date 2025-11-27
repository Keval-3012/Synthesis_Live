using SyntesisApi.DAL;
using SyntesisApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SyntesisApi.BAL
{
    public class BALWebCam : clsDAL
    {
        public int GetStore(string Location)
        {
            int id = 0;
            DataTable dt = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "sp_WebCam_Proc";
                cmd.Parameters.AddWithValue("@Mode", "Get_StoreID");
                cmd.Parameters.AddWithValue("@Location", Location);
                dt = new DataTable();
                dt = Select(cmd);
                if (dt.Rows.Count != 0)
                {
                    id = Convert.ToInt32(dt.Rows[0]["StoreID"]);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return id;
        }

        public int InsertCamera(WebCamCamera obj)
        {
            int id = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "sp_WebCam_Proc";
                cmd.Parameters.AddWithValue("@Mode", "INSERT_CAMERA");
                cmd.Parameters.AddWithValue("@CameraName", obj.CameraName);
                cmd.Parameters.AddWithValue("@StoreID", obj.StoreId);
                id = Insert_Update_Del(cmd);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return id;
        }

        public int InsertHistory(WebCamCameraHistory obj)
        {
            int id = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "sp_WebCam_Proc";
                cmd.Parameters.AddWithValue("@Mode", "INSERT_HISTORY");
                cmd.Parameters.AddWithValue("@ListID", obj.WebCamCameraListID);
                cmd.Parameters.AddWithValue("@RecordingDate", obj.RecordingDate);
                cmd.Parameters.AddWithValue("@RecordingStartTime", obj.RecordingStartTime);
                cmd.Parameters.AddWithValue("@RecordingEndTime", obj.RecordingEndTime);
                cmd.Parameters.AddWithValue("@FileName", obj.FileName);
                cmd.Parameters.AddWithValue("@EndHour", obj.EndHour);
                cmd.Parameters.AddWithValue("@FolderName", obj.FolderName);
                id = Insert_Update_Del(cmd);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return id;
        }

        public int UpdateHistoryDownload(int Id)
        {
            int id = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "sp_WebCam_Proc";
                cmd.Parameters.AddWithValue("@Mode", "UPDATE_HISTORY_Download");
                cmd.Parameters.AddWithValue("@Id", Id);
                id = Insert_Update_Del(cmd);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return id;
        }

        public int UpdateHistoryUpload(int Id)
        {
            int id = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "sp_WebCam_Proc";
                cmd.Parameters.AddWithValue("@Mode", "UPDATE_HISTORY_Upload");
                cmd.Parameters.AddWithValue("@Id", Id);
                id = Insert_Update_Del(cmd);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return id;
        }


        public DataTable GetWebCamera(int StoreId)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "sp_WebCam_Proc";
                Cmd.Parameters.AddWithValue("@Mode", "Get_WebCamera");
                Cmd.Parameters.AddWithValue("@StoreID", StoreId);
                dt = new DataTable();
                dt = Select(Cmd);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return dt;
        }

        public DataTable GetWebCamHistory(WebCamCameraHistory Web)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "sp_WebCam_Proc";
                Cmd.Parameters.AddWithValue("@Mode", "Get_WebCamHistory");
                Cmd.Parameters.AddWithValue("@ListID", Web.WebCamCameraListID);
                Cmd.Parameters.AddWithValue("@RecordingDate", Web.RecordingDate);
                dt = new DataTable();
                dt = Select(Cmd);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return dt;
        }
        public DataTable GetWebCamHistoryList(WebCamCameraHistory Web)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "sp_WebCam_Proc";
                Cmd.Parameters.AddWithValue("@Mode", "Get_WebCamHistoryList");
                Cmd.Parameters.AddWithValue("@ListID", Web.WebCamCameraListID);                
                dt = new DataTable();
                dt = Select(Cmd);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return dt;
        }
        public DataTable GetWebCamHistoryList1(WebCamCameraHistory Web)
        {
            DataTable dt = null;
            try
            {
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "sp_WebCam_Proc";
                Cmd.Parameters.AddWithValue("@Mode", "Get_WebCamHistoryList1");
                Cmd.Parameters.AddWithValue("@ListID", Web.WebCamCameraListID);
                dt = new DataTable();
                dt = Select(Cmd);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return dt;
        }
    }
}