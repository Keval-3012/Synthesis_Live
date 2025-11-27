using SyntesisApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.BAL
{
    public class BALHRTraining
    {
        clsDAL db = new clsDAL();
        #region Kirtan 12-03-2024
        public int UpdateLastSlide(LastSlide obj)
        {
            int id= 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiTraining";
                cmd.Parameters.AddWithValue("@Mode", "UpdateLastSlide");
                cmd.Parameters.AddWithValue("@EmployeeId", obj.EmployeeId);
                cmd.Parameters.AddWithValue("@LastSlideName", obj.LastSlideName);
                id = db.Insert_Update_Del(cmd);
            }
            catch (Exception ex)
            {
            }
            return id;
        }

        public int ResetTraining(EmployeeList obj)
        {
            int id = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiTraining";
                cmd.Parameters.AddWithValue("@Mode", "ResetTraining");
                cmd.Parameters.AddWithValue("@EmployeeId", obj.EmployeeId);
                id = db.Insert_Update_Del(cmd);
            }
            catch (Exception ex)
            {
            }
            return id;
        }

        public int UpdateLanguage(LanguageUpdate obj)
        {
            int id = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiTraining";
                cmd.Parameters.AddWithValue("@Mode", "UpdateLanguage");
                cmd.Parameters.AddWithValue("@EmployeeId", obj.EmployeeId);
                cmd.Parameters.AddWithValue("@LanguageId", obj.LanguageId);
                id = db.Insert_Update_Del(cmd);
            }
            catch (Exception ex)
            {
            }
            return id;
        }
        public int UpdateEmployeeTraining(CompleteyTrainingPost obj)
        {
            int id = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiTraining";
                cmd.Parameters.AddWithValue("@Mode", "UpdateEmployeeTraining");
                cmd.Parameters.AddWithValue("@EmployeeId", obj.EmployeeId);
                cmd.Parameters.AddWithValue("@CertificatesFileName", obj.TraningContent);
                cmd.Parameters.AddWithValue("@TrainingDate", obj.TrainingCompletedDateTime);
                cmd.Parameters.AddWithValue("@TrainingTime", obj.TrainingCompletedTime);
                cmd.Parameters.AddWithValue("@CertificatesFilePath", obj.TraningFilePath);
                id = db.Insert_Update_Del(cmd);
            }
            catch (Exception ex)
            {
            }
            return id;
        }

        public int EmployeeTrainingDownload(CompleteyTrainingPost obj)
        {
            int id = 0;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiTraining";
                cmd.Parameters.AddWithValue("@Mode", "EmployeeTrainingDownload");
                cmd.Parameters.AddWithValue("@EmployeeId", obj.EmployeeId);
                cmd.Parameters.AddWithValue("@CertificatesFileName", obj.TraningContent);
                cmd.Parameters.AddWithValue("@CertificatesFilePath", obj.TraningFilePath);
                id = db.Insert_Update_Del(cmd);
            }
            catch (Exception ex)
            {
            }
            return id;
        }
        #endregion
    }
}