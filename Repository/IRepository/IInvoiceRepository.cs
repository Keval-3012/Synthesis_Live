using EntityModels.Models;
using Syncfusion.EJ2.Base;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IInvoiceRepository
    {
        List<InvoiceSelect> GetInvoiceSelects(string startdate, string enddate, string payment, int deptname, string strStore, string AmtMaximum, string AmtMinimum, int Page, int PageSize, string orderby, string AscDsc, string searchdashbord, int UserTypeId, string SearchFlg);
        List<DepartmentMaster> GetDepartments_WithDepartmentCond(int UserTypeId, int StoreId);
        string GetInvoice_StoreList(int StoreId,bool RoleFlg,int UserId);
        List<EntityModels.Models.DepartmentMaster> getDepartment_WithSP(int storeid);
        List<InvoiceSelect> InvoiceDataDepartmentWise_Get(string deptid, string storeid, int UserTypeId, DataManagerRequest dm);

        Invoice getInvoiceObj(Invoice invoice,string val);

        Invoice EditInvoiceObj(int id);

        Invoice deleteInvoice(int id,string From);

        Invoice invoiceDetail(int id);

        Invoice invoiceDetail(int Id, int UserId);

        void InvoiceDetailSave(Invoice PostedData, int ID, string FirstName, int UserId);
        void InvoiceReject(int Id, int UserId);
        void InvoiceApprove(int Id, int qbtransfer);
        void InvoiceOnHold(int Id);
        void Dispose(bool disposing);
        InvoicesViewModel CheckExistence(string vendorid, string invoiceno, string invoicedate, string type, string storeid, string invoiceid, decimal totalamtvalue);
        InvoiceProduct UpdateProductprice(InvoiceProduct invoiceProduct);
        void InsertProductprice(InvoiceProduct invoiceProduct);
        void DeleteProductprice(string InvoiceProductId);
        IEnumerable GetDatapagescroll(int PageSize, int currentPageIndex, string rdcash, string rdcheck, string datval, string datendval, int deptnm, string searchdashbord, string AmtMaximum, string AmtMinimum, string orderby, int IsAsc, string SearchFlg, string chk, string strStore, string ASC);
        string GetroleForApproval(string Role, int? StoreId, int ModuleID);
        CashPaidoutInvoice CashPaidoutInvoices(int InvoiceID);
        SalesActivitySummary SalesActivitySummaries(CashPaidoutInvoice CPInvoice);
        Invoice Invoices(int InvoiceID);
        List<int> getVendorDepartmentList(int VendorId);
        int GetModuleMastersId();
        List<DepartmentMaster> GetDepartmentMastersList(int Storeidval, int iID);
        void UpdateIsSyncStatus_reset(int InvoiceId);
        void InsertInvoices(Invoice Invoices);
        bool CheckUserTypeModuleApprovers(int UserTypeId, int ModuleId);
        List<InvoiceProductSelect> GetItemList(int InvoiceID);
        void InsertInvoiceDepartmentDetails(InvoiceDepartmentDetail InvoiceDepartmentDetail);
        void UpdateStatus(string id, string value);
        Invoice GetInvoiceDetail(int id);
        void ApproveAllStatus(string id);
        List<InvoiceDepartmentDetail> GetInvoiceDepartmentDetailsbyInvoiceID(int InvoiceId);
        List<DepartmentMaster> GetDepartmentMastersListbyDepartmentId(List<InvoiceDepartmentDetail> Departments);
        void ResetStatus(string id);
        List<VendorMaster> ViewInvoice();
        void ReadFiles(string vendor, string startdate, string enddate);
        UploadPdf CreateSplitInvoice(int val);
        void UploadPdfId(int? uploadpdfId);
        List<InvoiceSelect> GetExpenseCheckSelect(InvoiceReportViewModal invoiceReportViewModal);
        List<InvoiceSelect> GetInvoiceSelect(InvoiceReportViewModal invoiceReportViewModal);

        InvoiceFlgs CreateInvoicePost(Invoice invoice, bool RoleAdministrator, bool nnfApprovalInvoice, bool FLG, bool roleFlg, string Store, int StoreFlag, string[] ChildDepartmentId, string[] ChildAmount, int Storeidval, string UserName);

        void SaveInvoiceDiscount(int iInvoiceId, int? DiscountTypeId, decimal? DiscountAmount, decimal? DiscountPercent);

        int CreateInvoiceCreditmemoPost(Invoice invoice, bool ApproveInvoice, bool Administrator, string Store, int StoreFlag, string UserName, int iInvoiceId, string Credit_Invoiceno, string InvoiceNM, string iInvoiceStatus);

        int CheckInvoiceExist(Invoice invoice);

        CheckExistInvoice CheckInvoiceExistAmountDetails(Invoice invoice);
        Invoice ChildDepartmentSave(Invoice invoice, string[] ChildDepartmentId, string[] ChildAmount, string UserName);

        void SaveInvoiceDiscountEdit(Invoice invoice);

        string InvoiceQBSync(Invoice invoice,string[] ChildDepartmentId, string[] ChildAmount,string Store,int Flag);

        string InvoiceCreditmemoSaveEdit(Invoice invoice, bool QuickCRInvoice, bool Administrator, string UserName, string Credit_Invoiceno, string scandocument);
        List<InvoiceCount> InvoiceCounts(string Date, int InvoiceType);

        int SaveAutoMationFileChanges(UploadPDFAutomation uploadPDF);
        List<UploadPDFAutomationList> getuploadautomationlist(int store_idval,int UserId);
        UploadPDFAutomation GetUploadAutomation(int id);
        void DeleteInvoiceAutomation(int id);
        void UploadAutomationPdfId(int? UploadPdfAutomationId);
        InvoiceAutomation EditInvoiceAutomationObj(int id);
        InvoiceFlgs CreateInvoiceAutomationPost(InvoiceAutomation invoice, bool RoleAdministrator, bool nnfApprovalInvoice, bool FLG, bool roleFlg, string Store, int StoreFlag, string[] ChildDepartmentId, string[] ChildAmount, int Storeidval, string UserName);
        int CreateInvoiceAutomationCreditmemoPost(InvoiceAutomation invoice, bool ApproveInvoice, bool Administrator, string Store, int StoreFlag, string UserName, int iInvoiceId, string Credit_Invoiceno, string InvoiceNM, string iInvoiceStatus);
        void UpdateInvoiceReview(int invoiceid,string reviewnote);
        int UploadProcessStart();
        void UploadProcessFinish(int MultithreadingInvoiceLogId,int UploadPdfAutomationId);
        void UserlogDeleteInsert(string InvoiceNumber, int DeletedBy);
        void UpdateInvoiceTasks(int invoiceid, string invoicetask, string invoiceno, int createdby, int priorityid, int assignid, DateTime duedatetask);
        Tuple<List<InvoiceSelect>, int> InvoiceSyncfusionGrid_Data(string SearchTerm, string SortFields, string SortDirections, string Filters, int Skip, int Take, int UserTypeId, string Dept_id, string Store_id);
        List<InvoiceFilter> GetInvoiceFilteredRecords();
        List<InvoicePaymentStatusDetailsList> InvoicePaymentStatus(int InvoiceID);
    }
}
