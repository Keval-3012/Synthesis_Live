using SyntesisApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.BAL
{
    public class BALHRRetirementInfo
    {
        clsDAL db = new clsDAL();
        #region Start Harsh
        public void saveRetirementInfo(EmpRetirementInfo obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiRetirementInfo";
                cmd.Parameters.AddWithValue("@Mode", "SaveRetirementInfo");
                cmd.Parameters.AddWithValue("@EmployeeId", Convert.ToInt32(obj.EmployeeID));
                cmd.Parameters.AddWithValue("@OptStatus", obj.OptStatus);
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