using SyntesisApi.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static SyntesisApi.Models.ApiModel;

namespace SyntesisApi.BAL
{
    public class BALHRConsent
    {
        clsDAL db = new clsDAL();
        #region Start Harsh
        public void saveEmpDoc(EmployeeDocument obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SP_HRApiConsent";
                cmd.Parameters.AddWithValue("@Mode", "SaveConsentEmpDoc");
                cmd.Parameters.AddWithValue("@StoreID", obj.StoreId);
                cmd.Parameters.AddWithValue("@EmployeeId", obj.EmployeeID);
                cmd.Parameters.AddWithValue("@DocFileName", obj.DocFileName);
                cmd.Parameters.AddWithValue("@LocationFrom", obj.LocationFrom);
                cmd.Parameters.AddWithValue("@DocumentType", obj.DocumentType);
                cmd.Parameters.AddWithValue("@DocumentTypeId", obj.DocumentTypeId);
                cmd.Parameters.AddWithValue("@CreatedID", obj.CreatedID);
                cmd.Parameters.AddWithValue("@Status", obj.Status);
                cmd.Parameters.AddWithValue("@Comment", obj.Comment);
                cmd.Parameters.AddWithValue("@LanguageID", obj.LanguageID);
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