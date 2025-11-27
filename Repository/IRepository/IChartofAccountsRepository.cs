using EntityModels.Models;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IChartofAccountsRepository
    {
        List<StoreMaster> StoreMasterList();
        List<AccountDetailTypeMaster> AccountDetailTypeList();
        List<AccountTypeMaster> AccountTypeList();
        List<DepartmentMaster> DepartmentMasterList();
        List<DepartmentMaster> SelectByStoreID_Name(int StoreId, string DepartmentName);
        List<DepartmentMaster> SelectForUpdate(int? StoreId, string DepartmentName, int DepartmentId);
        DepartmentMaster SelectByStoreID_DepartmentId(int StoreId, int iDeptID);
        List<AccountTypeMaster> GetAccountType(string AccType);
        List<AccountDetailTypeMaster> GetQBDetailType(string DetailsType);
        DepartmentMaster SelectByStoreID_DepId(int? StoreId, int DepartmentId);
        List<AccountTypeMaster> GetAccountType1(string QBAccountType);
        DepartmentMaster SelectByStoreID_Name1(int StoreId, string DepartmentName);
        void SaveAccountTypeMasters(AccountTypeMaster objAcc);
        void SaveAccountDetailTypeMasters(AccountDetailTypeMaster objDetail);
        void UpdateDepartmentMaster(DepartmentMaster objDept);
        void SaveDepartmentMaster(DepartmentMaster objDept);
        List<DepartmentMaster> SelectByStoreID_ListID(int? StoreId, string ID);
        Task<DepartmentMaster> GetDepartmentMasterId(int? id);
        void DeleteConfirmed(DepartmentMaster departmentMaster);
        void ActiveStatus(int deptid);
        void InActiveStatus(int deptid);
    }
}
