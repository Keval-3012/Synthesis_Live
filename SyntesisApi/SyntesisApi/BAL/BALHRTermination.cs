using SyntesisApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.BAL
{
    public class BALHRTermination
    {
        clsDAL db = new clsDAL();
        public void SaveEmployeeTermination(EmployeeTermination obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiEmployeeTermination";
                cmd.Parameters.AddWithValue("@Mode", "SaveEmployeeTermination");
                cmd.Parameters.AddWithValue("@EmployeeId", obj.EmployeeID);
                cmd.Parameters.AddWithValue("@EmployeeChildId", obj.EmployeeChildID);
                cmd.Parameters.AddWithValue("@DocFileName", obj.DocFileName);
                cmd.Parameters.AddWithValue("@IsActive", obj.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", obj.CreatedBy);
                db.Insert_Update_Delete(cmd);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}