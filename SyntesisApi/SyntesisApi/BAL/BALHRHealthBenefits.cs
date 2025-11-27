using SyntesisApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.BAL
{
    public class BALHRHealthBenefits
    {
        clsDAL db = new clsDAL();
        #region Start Harsh 
        public void saveHealthBenefits(EmployeeHealthBenefit obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiHealthBenefits";
                cmd.Parameters.AddWithValue("@Mode", "SaveHealthBenefits");
                cmd.Parameters.AddWithValue("@EmployeeId", Convert.ToInt32(obj.EmployeeID));
                cmd.Parameters.AddWithValue("@OtherCoverage", obj.OtherCoverage);
                cmd.Parameters.AddWithValue("@OtherCoverageDetail", obj.OtherCoverageDetail);
                cmd.Parameters.AddWithValue("@RefusedCoverage", obj.RefusedCoverage);
                cmd.Parameters.AddWithValue("@EmployeeChildID", obj.EmployeeChildID);
                cmd.Parameters.AddWithValue("@DocFileName", obj.DocFileName);
                cmd.Parameters.AddWithValue("@CreatedID", obj.CreatedID);
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