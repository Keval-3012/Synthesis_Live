using EntityModels.Models;
using SynthesisQBOnline.BAL;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.IRepository
{
    public interface IExpenseAccountsRepository
    {
        List<SpExpenseCheck_Setting> ExpenseCheck_SettingList(int storeid, int Type);
        void ExpenseCheck_SettingsRemoveRange(int StoreIDs, int TypeID);
        void AddExpenseCheck_Settings(ExpenseCheck_Setting obj);
        string GetExpenseCheck_StoreList(int StoreId, int UserID, bool IsAuth);
        string GetStoreName(int StoreId);
        List<ExpenseCheckSelect> GetExpenseCheck_BindData(ExpenseCheckSettingViewModal ExpenseCheckSettingViewModal, string strStore, int UserID, string AscDsc);
        List<int> GetExpenseCheckDetails();
        bool CheckUserTypeMasters(int UserTypeId);
        List<int> GetExpenseDeptIds(int UserTypeId, int StoreID);
        List<DepartmentMaster> GetDepartmentMasters(List<int> ExpenseDeptIds, int StoreID);
        List<ExpenseCheckSelect> ExpenseCheckPreviewData(ExpenseCheckSettingViewModal ExpenseCheckSettingViewModal, int UserTypeId, string strStore);
        string GetStore_ForExpenseCheck(int UserId);
        void AddexpenseCheckDocuments(ExpenseCheckDocuments expenseCheckDocuments);
        void UpdateExpenseCheckDetail(int ExpenseId, bool IncludeBySetting);
        void UpdateExpenseCheckDetail(bool flag, int ExpenseId);
        string DeleteexpenseCheckDocuments(int Id);
        List<ExpenseCheckExcludedList> ExpenseCheckPreviewData(int UserTypeId, string strStore);
        List<ExpenseCheckDocuments> GETExpenseCheckDocuments(int ExpenseCheckID);
        List<ExpenseCheckSelect> ExpenseCheckPreviewData_Beta(string payment, string Orderby, string AscDsc, string Status, string ForceStatus, int UserTypeId, string strStore, string Startdate = null, string Enddate = null);
        ExpenseCheck InsertExpenseData(ExpenseCheck expense);
        List<ExpensePaymentMethodMaster> PaymentMethodList();
        ExpenseCheckDetail InsertExpenseDetailsData(ExpenseCheckDetail expensedetails);
        List<ExpenseCheckList> GetExpenseCheckList(int storeid);
        ExpenseCheck GetExpenseCheckById(int ExpenseCheckId);
        List<ExpenseCheckDetail> GetExpenseCheckDetailsById(int ExpenseCheckId);
        void DeleteExpenseDetailsData(int ExpenseCheckDetailId);
        ExpenseCheckDocuments InsertExpenseFileData(ExpenseCheckDocuments expensefile);
        void DeleteExpenseCheckData(ExpenseCheck expensevalue);
        List<UploadedFiles> GetUploadedExpenseFiles(int ExpenseCheckId);
        List<DepartmentMaster> DepartmentAccountTypeList(int? StoreId);
        List<DepartmentMaster> CheckDepartmentAccountTypeList(int? StoreId);
        List<DepartmentMaster> DepartmentDetailList(int? StoreId);
        string GetPayeeMailingAddressById(int payeeid);
        void InsertExpenseSyncId(int ExpenseCheckId,QBResponse objRes);
        void UpdateExpenseSyncId(int ExpenseCheckId,QBResponse objRes);
        List<ExpenseCheckSelect> GetPrintCheckList(int StoreId);
        List<CheckPdfPrint> GetPrintCheckPayeeData(string expenseCheckIds);
        List<CheckPdfPrintDetails> GetPrintCheckDetails(int ExpenseCheckId);
        string GetQbErrorMessageById(int expensecheckid);
        void InsertExpenseErrorSyncId(int ExpenseCheckId, QBResponse objRes);

        List<ExpenseCheckSelectMain> BulkCheckPreviewData_Beta(string payment, string Orderby, string AscDsc, string Status, string ForceStatus, int UserTypeId, string strStore, string Startdate = null, string Enddate = null);
        List<ExpenseCheckSelectMain> BulkExpensePreviewData_Beta(string payment, string Orderby, string AscDsc, string Status, string ForceStatus, int UserTypeId, string strStore, string Startdate = null, string Enddate = null);
    }
}
