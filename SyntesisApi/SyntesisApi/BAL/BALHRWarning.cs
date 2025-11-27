using SyntesisApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.BAL
{
    public class BALHRWarning
    {
        clsDAL db = new clsDAL();
        public void SaveEmployeeWarning(EmployeeWarning obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiEmployeeWarning";
                cmd.Parameters.AddWithValue("@Mode", "SaveEmployeeWarning");
                cmd.Parameters.AddWithValue("@EmployeeId", obj.EmployeeID);
                cmd.Parameters.AddWithValue("@EmployeeChildId", obj.EmployeeChildID);
                cmd.Parameters.AddWithValue("@Warning", obj.Warning);
                cmd.Parameters.AddWithValue("@Remarks", obj.Remarks);
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