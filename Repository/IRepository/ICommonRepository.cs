using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ICommonRepository
    {
        int getUserId(string UserName);
        string getUserFirstName(string UserName);
        int getUserTypeId(string UserName);
        List<StoreMaster> GetStoreList_RoleWise(int ModuleNo, string Role, string UserName);
        List<StoreMaster> GetStoreList(int ModuleNo);
        List<StoreMaster> GetHeaderStoreList(int ModuleNo);
        string getUserFirstNameById(int UserId);

        string GetStoreNikeName(int StoreId);
        System.Data.DataTable Select(System.Data.SqlClient.SqlCommand cmd);
        int InsertProduct(Products obj);
        int InsertProductVendor(ProductVendor obj);
        string GetMonthName(int Month);
        string GetMessageValue(string strKey, string Defaultvalue = "");
        List<UserMaster> GetUserList();
        void LogEntries();

    }
}
