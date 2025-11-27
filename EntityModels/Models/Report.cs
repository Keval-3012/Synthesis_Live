using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    //----- OperatingRatioReport -------------//
    #region OperatingRatioReport 
    public class OperatingRatioReport
    {
        public OperatingRatioReport()
        {
            this.OperatingRatioLists = new List<OperatingRatioList>();
        }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public ShiftEnm? ShiftId { get; set; }
        public List<OperatingRatioList> OperatingRatioLists { get; set; }
        public List<ReportStoreList> ReportStoreLists { get; set; }
    }
    public class OperatingRatioList
    {
        public string Department { get; set; }
        public Nullable<decimal> Sales { get; set; }
        public Nullable<decimal> TotalSalPercentage { get; set; }
        public Nullable<decimal> COgs { get; set; }
        public Nullable<decimal> SalPercentage { get; set; }
        public Nullable<decimal> PDFAmount { get; set; }
        public Nullable<decimal> PDFPercentage { get; set; }
        public string PayrollDescription { get; set; }
        public int Status { get; set; }
        public string MasterDepartment { get; set; }
    }
    public class OperatingRatioListPDF
    {
        public string Department { get; set; }
        public string Sales { get; set; }
        public string TotalSalPercentage { get; set; }
        public string COgs { get; set; }
        public string SalPercentage { get; set; }
        public string PDFAmount { get; set; }
        public string PDFPercentage { get; set; }
        public string PayrollDescription { get; set; }
    }
    public class ReportStoreList
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        
    }
    #endregion
    #region SalesSummaryReport
    public class SalesSummaryReport
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class ShiftWiseTenderReport_Select
    {
        public List<ShiftWiseTotal> ShiftWiseTotal { get; set; }
        public List<TitleWiseTotal> TitleWiseTotal { get; set; }
        public List<ShiftList> shiftname { get; set; }
        public List<OtherDepositeList> OtherDepositeList { get; set; }
        public string Total { get; set; }
        public Nullable<decimal> CashTotal { get; set; }
        public Nullable<decimal> OtherTotal { get; set; }
        public int? PaidOutCount { get; set; }
        public List<string> PaidDistinctList { get; set; }
        public List<ShiftNewList> shiftnamelist { get; set; }
    }

    public class OtherDepositeList
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public string PaymentMethod { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string options { get; set; }
        public string Vendor { get; set; }
        public string Other { get; set; }
    }

    public class ShiftWiseTotal
    {
        public Nullable<decimal> TotleByShift { get; set; }
        public Nullable<decimal> TotalShiftWiseTax { get; set; }
    }

    public class TitleWiseTotal
    {
        public string Title { get; set; }
    }

    public class ShiftList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ShiftNewList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
    }
    public partial class GetShiftWiseTotal_Result
    {
        public Nullable<decimal> TotalCustomer { get; set; }
        public Nullable<decimal> TotleByShift { get; set; }
        public Nullable<decimal> TotalTax { get; set; }
        public Nullable<decimal> TotalReturn { get; set; }
        public Nullable<decimal> TotalAverage { get; set; }
    }

    public enum ShiftEnm
    {
        [Display(Name ="Shift 1")]
        Shift_1 = 1,
        [Display(Name = "Shift 2")]
        Shift_2 = 2,
        [Display(Name = "Shift 3")]
        Shift_3 = 3,
        SCO = 4,
        MOB = 5
    }
    #endregion

    #region DailyPosFeeds
    public class DailyPosFeeds
    {
        public int SalesActivitySummaryId { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public int? StoreTerminalId { get; set; }
        public int? ShiftId { get; set; }
        public string TerminalName { get; set; }
        public DateTime? TransactionStartTime { get; set; }
        public DateTime? TransactionEndTime { get; set; }
        public decimal CustomerCount { get; set; }
        public Nullable<decimal> NetSalesWithTax { get; set; }
        public Nullable<decimal> TotalTaxCollected { get; set; }
        public Nullable<decimal> AverageSale { get; set; }
        public string CeatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public string TransactionTime { get; set; }
        public string FileName { get; set; }
        public string ShiftName { get; set; }
    }

    public class DailyPosFeedsDetails
    {
        #region Basic Detail
        public int SalesActivitySummaryId { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public int? StoreTerminalId { get; set; }
        public string TerminalName { get; set; }
        public DateTime? TransactionStartTime { get; set; }
        public DateTime? TransactionEndTime { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public decimal CustomerCount { get; set; }
        public Nullable<decimal> NetSalesWithTax { get; set; }
        public Nullable<decimal> TotalTaxCollected { get; set; }
        public Nullable<decimal> AverageSale { get; set; }
        public string CeatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        #endregion

        #region Department
        public int DeptNetSalesId { get; set; }
        public Nullable<int> DeptActivitySalesSummuryId { get; set; }
        public string DeptNetSalesCeatedBy { get; set; }
        public Nullable<System.DateTime> DeptCreatedDate { get; set; }
        public string DeptNetSalesUpdatedBy { get; set; }
        public Nullable<System.DateTime> DeptNetSalesUpdatedDate { get; set; }
        public Nullable<bool> DeptNetSalesIdIsActive { get; set; }
        public List<Department> DepartmentList { get; set; }
        #endregion

        #region Tender
        public int TendersInDrawerId { get; set; }
        public Nullable<int> TendersActivitySalesSummuryId { get; set; }
        public string TendersCeatedBy { get; set; }
        public Nullable<System.DateTime> TendersCreatedDate { get; set; }
        public string TendersUpdatedBy { get; set; }
        public Nullable<System.DateTime> TendersUpdatedDate { get; set; }
        public Nullable<bool> TendersIsActive { get; set; }
        public List<Tender> TenderList { get; set; }
        #endregion

        #region PaidOut
        public int PaidOutOutId { get; set; }
        public Nullable<int> PaidOutActivitySalesSummuryId { get; set; }
        public List<PaidOuts> Paidoutlist { get; set; }
        public string PaidOutCeatedBy { get; set; }
        public Nullable<System.DateTime> PaidOutCreatedDate { get; set; }
        public string PaidOutUpdatedBy { get; set; }
        public Nullable<System.DateTime> PaidOutUpdatedDate { get; set; }
        public Nullable<bool> PaidOutIsActive { get; set; }
        #endregion
    }

    public class Tender
    {
        public string TendersTitle { get; set; }
        public decimal? TendersAmount { get; set; }
    }

    public class PaidOuts
    {
        public string PaidOutTitle { get; set; }
        public decimal? PaidOutAmount { get; set; }
    }

    public class Department
    {
        public string DeptNetSalesTitle { get; set; }
        public decimal? DeptAmount { get; set; }
    }
    #endregion

    #region PayrollExpense
    public class PayrollExpenseSelect
    {
        public int Payrollid { get; set; }
        public DateTime? EndCheckDate { get; set; }
        public string Storename { get; set; }
        public int StoreId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string TxnID { get; set; }
        public Nullable<int> IsSync { get; set; }
        public DateTime? ApproveDate { get; set; }
    }

    public class PayrollExpenseDetailss
    {
        public decimal? EditTotalAmount { get; set; }

        public List<PayrollExpenseDetail> PayrollExpenseDetail { get; set; }
    }

    public class PayrollExpenseDetail
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public int? QbAccountid { get; set; }
        public int Payrollid { get; set; }
        public int PayrollCashAnalysisId { get; set; }
        public int PayrollCashAnalysisDetailId { get; set; }
        public int IsSync { get; set; }
        public string NotConfigureAccount { get; set; }
        public decimal? OldTotalAmount { get; set; }
        public Nullable<int> ValueIn { get; set; }
        public string DepartmentListId { get; set; }
        public int? DepartmentId { get; set; }
        public int PayrollReportId { get; set; }
        public int? StoreID { get; set; }
        public bool QBStatusField { get; set; }
        public string TxnID { get; set; }

    }
    #endregion

    #region PayrollAnalysisReport
    public class PayrollAnalysisReport
    {
        public PayrollAnalysisReport()
        {
            this.PayrollAnalysisList = new List<PayrollAnalysisList>();
        }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<PayrollAnalysisList> PayrollAnalysisList { get; set; }
        public List<ReportStoreList> ReportStoreLists { get; set; }
    }
    public class PayrollAnalysisList
    {
        public string DepartmentName { get; set; }
        public Nullable<decimal> PayrollAmount { get; set; }
        public Nullable<decimal> PayrollPercentage { get; set; }
        public string PayrollHours { get; set; }
        public int? PayrollDepartmentId { get; set; }
    }

    public class PayrollAnalysisTotal
    {
        public string DepartmentName { get; set; }
        public Nullable<decimal> PayrollAmount { get; set; }
        public Nullable<decimal> PayrollPercentage { get; set; }
    }
    public class OperatingRationDailyAllTotal
    {
        public string StoreName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Percentage { get; set; }
    }
    #endregion
}