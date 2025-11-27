using EntityModels.Models;
using SynthesisQBOnline.BAL;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IQBRepository
    {
        string GetStoreOnlineDesktop(int StoreID);
        int GetStoreOnlineDesktopFlag(int StoreID);
        QBOnlineconfiguration GetConfigDetail(int StoreID);
        bool QBSyncDepartment(string Department, string AccountType, string DetailType, string Description, string AccountNumber, string IsSubAccount, int StoreId, ref QBResponse objRes);
        bool QBEditSyncDepartment(string Department, string AccountType, string DetailType, string Description, string AccountNumber, string IsSubAccount, string TxnId, int StoreId, ref SynthesisQBOnline.BAL.QBResponse objRes);
        bool QBEditSyncVendor(EntityModels.Models.VendorMaster objV, string TxnId, int StoreId, ref QBResponse objRes);
        bool QBSyncVendor(EntityModels.Models.VendorMaster objV, int StoreId, ref QBResponse objRes);
        List<EntityModels.Models.QBOnlineConfiguration1> GetQBOnlineConfiguration1();
        List<EntityModels.Models.QBDesktopConfiguration> GetQBDesktopConfiguration(QBConfigurationViewModal qBConfigurationViewModal);
        List<EntityModels.Models.QBDesktopConfiguration> UpdateBankDetails(UpdateBank updateBank);
        EntityModels.Models.QBDesktopConfiguration GenerateXML(GenerateXML generateXML, int StoreID);
        List<EntityModels.Models.QBOnlineConfiguration> SelectByStoreID(int StoreID);
        void SaveQBOnlineConfigurations(EntityModels.Models.QBOnlineConfiguration QBOnlines);
        EntityModels.Models.QBOnlineConfiguration GetQBOnlineId(int QBOnlineId);
        void UpdateQBOnlineConfigurations(EntityModels.Models.QBOnlineConfiguration obj2);
        List<EntityModels.Models.QBDesktopConfiguration> SelectByStoreIDForDesktop(int StoreID);
        void GetQBDesktopId(int QBDesktopId);
        List<EntityModels.Models.QBHistory> SelectByIdQBHistory(int StoreID);
        void SaveQBHistorys(EntityModels.Models.QBHistory QBHistorys);
        void UpdateQBHistory(int QBHistoryId);
        EntityModels.Models.VendorMaster SelectByStoreID_NameVendor(int StoreID, string DisplayName);
        List<EntityModels.Models.VendorMaster> VendorMasters();
        void UpdateVendorMaster(EntityModels.Models.VendorMaster objVendor);
        EntityModels.Models.VendorMaster SelectByStoreID_IDVendor(int StoreID, int ID);
        void SaveVendorMasters(EntityModels.Models.VendorMaster objVendor);
        List<EntityModels.Models.DepartmentMaster> SelectByStoreID_Department(int StoreID, string Department);
        List<EntityModels.Models.AccountTypeMaster> AccountTypeMastersList();
        EntityModels.Models.AccountTypeMaster GetAccountType(string AccountType, string Flag);
        void SaveAccountTypeMasters(EntityModels.Models.AccountTypeMaster objAcc);
        EntityModels.Models.AccountDetailTypeMaster GetQBDetailType(string DetailType);
        void SaveAccountDetailTypeMasters(EntityModels.Models.AccountDetailTypeMaster objDetail);
        void UpdateDepartmentMaster(EntityModels.Models.DepartmentMaster objDept);
        EntityModels.Models.DepartmentMaster SelectByStoreID_ListIDDepartment(int StoreID, string ID);
        List<EntityModels.Models.DepartmentMaster> DepartmentMastersList();
        void SaveDepartmentMasters(EntityModels.Models.DepartmentMaster objDept);
        List<EntityModels.Models.StoreMaster> StoreMastersList();
        List<EntityModels.Models.Invoice> InvoicesList();
        List<EntityModels.Models.StoreQBSyncModel> GetQBSyncData();
        List<EntityModels.Models.StoreQBTotalCount> GetTotalCount(int StoreId);
        List<EntityModels.Models.InvoiceSelect> GetUnsuccessfullInvoice(int StoreID);
        void SaveQBOnlineConfigurationsById(int StoreID, int Flag, string Type);
        List<EntityModels.Models.QBDesktopConfiguration> QBDesktopConfigurationsList();
        List<EntityModels.Models.QBOnlineConfiguration> QBOnlineConfigurationsList();
        EntityModels.Models.QBOnlineConfiguration1 GetQBConfigurationBystore(int Store);
        List<EntityModels.Models.DistinctAccountType> GetAccountTypeFlag(string QBFlg);
        List<EntityModels.Models.AccountDetailTypeMasterSelect> GetAccountDetailTypeFlag(string GetAccountType);
        List<EntityModels.Models.IsSubAccountsSelect> CommonType_Flag_StoreId(string GetAccountType, string flg, int StoreID);
        string GetSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes);
        void InvoicesUpdateUnsuccess(int[] Inv);
        bool QBEditBillData(int dbinvoiceId, string InvoiceID, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<BillDetail> objList, ref QBResponse objRes);
        void InvoicesIgnoreUnsuccess(int[] Inv);
        void UpdateQBFlagDataOnline();
        void UpdateQBFlagDataOnline1();
        string QBGetVendorStatus(string TxnId, int StoreId, ref QBResponse objRes);
        void CreateBill(int invoiceID, List<BillDetail> onjDepartment);
        void CreateVendorCredit(int invoiceID, List<VendorCredirDetail> onjVendorCredit, int IsDashboard);
        string GetVendorSyncToken(string Id, QBOnlineconfiguration objOnline, ref QBResponse objRes);
        bool QBEditVendorCreditData(int dbinvoiceId, string InvoiceID, string InvoiceNo, DateTime Invoice_Date, string Note, int StoreId, int VendorID, List<VendorCredirDetail> objList, ref QBResponse objRes);
        bool QBSyncExpense(EntityModels.Models.ExpenseCheck objV, ICollection<ExpenseCheckDetail> objDetail, int StoreId, ref QBResponse objRes);
        bool QBEditSyncExpense(EntityModels.Models.ExpenseCheck objV, ICollection<ExpenseCheckDetail> objDetail, int StoreId, ref QBResponse objRes);
    }
}
