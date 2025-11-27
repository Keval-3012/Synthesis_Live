using SyntesisApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.BAL
{
    
    public class BALHRVaccine
    {
        clsDAL db = new clsDAL();
        #region Start Harsh
        public void saveVaccineDetails(VaccineCertificateInfo obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiVaccineDetails";
                cmd.Parameters.AddWithValue("@Mode", "SaveVaccine");
                cmd.Parameters.AddWithValue("@EmployeeId", obj.EmployeeID);
                cmd.Parameters.AddWithValue("@IsVaccine", obj.IsVaccine);
                cmd.Parameters.AddWithValue("@IsExemption", obj.IsExemption);
                cmd.Parameters.AddWithValue("@FileName", obj.FileName);
                cmd.Parameters.AddWithValue("@CreatedID", obj.CreatedID);
                cmd.Parameters.AddWithValue("@EmployeeChildID", obj.EmployeeChildID);
                db.Insert_Update_Delete(cmd);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
    }

}