using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("CashPaidoutInvoice")]
    public class CashPaidoutInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int CashPaidoutInvoiceId { get; set; }

        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoices { get; set; }

        [Required(ErrorMessage = " ")]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? StoreTerminalId { get; set; }
        [ForeignKey("StoreTerminalId")]
        public virtual StoreTerminal StoreTerminals { get; set; }

        public int? SalesActivitySummaryId { get; set; }
        [ForeignKey("SalesActivitySummaryId")]
        public virtual SalesActivitySummary SalesActivitySummarys { get; set; }

        public int? ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public virtual ShiftMaster ShiftMasters { get; set; }

        public int? PaidOutId { get; set; }
        [ForeignKey("PaidOutId")]
        public virtual PaidOut PaidOuts { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
   
    public class Terminal_Select
    {
        public List<ddlList> TerminalList { get; set; }
        //public List<ddlShiftList> ShiftList { get; set; }
        public List<BindTerminaldata> TerminalData { get; set; }
        public List<BindShiftdata> ShiftData { get; set; }
        public List<Bindshiftwisetenderlist> ShiftWisetenderData { get; set; }

        public List<OtherDepositData> OtherDepositList { get; set; }
        public decimal? totalsalesamount { get; set; }
        public decimal? totalavgsales { get; set; }
        public decimal? totalcash { get; set; }
        public int? Coustomercount { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string terminal_id { get; set; }
        public string Shift_id { get; set; }
        public Nullable<decimal> AMEX { get; set; }
        public Nullable<decimal> Discover { get; set; }
        public Nullable<decimal> Master { get; set; }
        public Nullable<decimal> Visa { get; set; }
        public Nullable<decimal> CCOffline { get; set; }
        public string DepositeCount { get; set; }
        public int? Optionid { get; set; }
        public List<ddllist> SelectOptionList { get; set; }
        public int? Payid { get; set; }
        //public List<ddllist> payMehoed { get; set; }
        public int? vendorid { get; set; }
        public int? DepartmentId { get; set; }
        public List<ddllist> SelectVendorList { get; set; }
        public List<ddllist> SelectDepartmentList { get; set; }
        public List<ddllist> ShiftNameList { get; set; }
        public int? iTerminalId { get; set; }
        public int? ShiftId { get; set; }

        public Nullable<decimal> Bottle_Deposite { get; set; }
        public string DaycloseOutFile { get; set; }
        public string NotConfigureAccount { get; set; }
        public decimal? totalOverShort { get; set; }
    }

    public class ddllist
    {
        public Nullable<bool> IsActive { get; set; }
        public string Store { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public int selectedvalue { get; set; }
        public bool selected_value { get; set; }
        public string ListID { get; set; }
    }
    public class ddlList
    {
        public int TerminalId { get; set; }
        public string TerminalName { get; set; }
        public decimal? SalesAmount { get; set; }
        public decimal? TotalTaxAmount { get; set; }
        public int? CustomerCount { get; set; }
        public List<ddlShiftList> ShiftList { get; set; }

    }
    public class OtherDepositData
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string payment { get; set; }
        public decimal? amount { get; set; }
        public DateTime? date { get; set; }
        public int? storeid { get; set; }
        public string options { get; set; }
        public string Vendor { get; set; }
        public string Other { get; set; }
        public int? iTerminalId { get; set; }
        public int? ShiftId { get; set; }
        public string TerminalName { get; set; }
        public string ShiftName { get; set; }
        public int? Shift { get; set; }
        public string UploadDocument { get; set; }
        public int ActivitySalesSummuryId { get; set; }
        public string Department { get; set; }
    }
    public class BindTerminaldata
    {
        public int TerminalId { get; set; }
        public string TerminalName { get; set; }
    }
    public class Bindshiftwisetenderlist
    {
        public int Id { get; set; }
        public string Terminalname { get; set; }
        public string Terminalid { get; set; }
        public string ShiftName { get; set; }
        public decimal? SalesAmount { get; set; }
        public decimal? TotalTaxAmount { get; set; }
        public decimal? AvgSales { get; set; }
        public decimal? CustomerCount { get; set; }
        public string Notes { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<ShiftNameList> ShiftNameList { get; set; }
        public List<TenderList> TenderList { get; set; }
        public List<PaidoutList> paidoutLists { get; set; }
        public decimal? CashierNegative { get; set; }

        //[Required(ErrorMessage = "Cashier Name is Required ")]
        public string CashierName { get; set; }
        public decimal? Paidoutamount { get; set; }
        public string FileName { get; set; }

    }
    public class ShiftNameList
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public class TenderList
    {
        public string Name { get; set; }
        [Required(ErrorMessage = "Amount is Required ")]
        public decimal? Amount { get; set; }
        public int? Id { get; set; }
        public string ListName { get; set; }
        public bool? IsManually { get; set; }
    }
    public class PaidoutList
    {
        public string Title { get; set; }
        [Required(ErrorMessage = "Amount is Required ")]
        public decimal? Amount { get; set; }
        public int? Id { get; set; }
        public string ListName { get; set; }

        public List<BindCase_PaidOut_Invoice> BindCase_PaidOut_Invoice { get; set; }
        public List<PaidOut_Settlement> PaidOut_SettlementList { get; set; }
    }

    public class CreditcardDetailsData
    {
        public decimal Amount_AMEX { get; set; }
        public decimal Amount_Discover { get; set; }
        public decimal Amount_Visa { get; set; }
        public decimal Amount_Master { get; set; }
        public decimal Amount_CCOffline { get; set; }
    }
    public class BindShiftdata
    {
        public decimal? SalesAmount { get; set; }
        public int? terminalid { get; set; }
        public int? CustomerCount { get; set; }
        public List<ddlShiftList> ShiftdataList { get; set; }
    }

    public class ddlShiftList
    {
        public int Id { get; set; }
        public string ShiftName { get; set; }
        public decimal? SalesAmount { get; set; }
        public decimal? TotalTaxAmount { get; set; }
        public decimal? CustomerCount { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<ShiftNameList> ShiftNameList { get; set; }
        public List<TenderList> TenderList { get; set; }
        public string CashierName { get; set; }
        public int SalesActivitySummaryId { get; set; }
    }
    public class BindCase_PaidOut_Invoice
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int StoreId { get; set; }
        public int TerminalID { get; set; }
        public int? ShiftId { get; set; }
        public int PaidOutId { get; set; }
        public decimal Totalamount { get; set; }
        public string Invoiceno { get; set; }
        public string VendorNAme { get; set; }
    }
    public class PaidOut_Settlement
    {
        public int? Id { get; set; }
        public int ActivitySalesSummaryID { get; set; }
        public bool IsActive { get; set; }
        public string Title { get; set; }
        public decimal? Amount { get; set; }
    }

    public partial class tbl_TenderinDraw_ById_Result
    {
        public int Id { get; set; }
        public Nullable<int> SalesActivitySummaryId { get; set; }
        public string Title { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public int IsManual { get; set; }
    }


    public class TenderAccountsStoreWise
    {
        public string Title { get; set; }
        public int? GroupId { get; set; }
        public int? TypicalBalId { get; set; }
        public string TypicalBalName { get; set; }
        public int? DepartmentId { get; set; }
        public string DeptName { get; set; }
        public string Memo { get; set; }
        public int? Flag { get; set; }
    }

    public class DailyCloseOutSalesList
    {
        public string Title { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? Flag { get; set; }
        public int? RefId { get; set; }
        public string AccountType { get; set; }
        public int? ConfigurationGroupId { get; set; }
        public int? DepartmentId { get; set; }
        public string Memo { get; set; }
        public int? TypicalBalanceId { get; set; }
    }

    public class SalesActivitySummarySalesList
    {
        public Nullable<decimal> NetSalesWithTax { get; set; }
        public Nullable<decimal> TotalTaxCollected { get; set; }
        public string TerminalName { get; set; }
        public int TerminalID { get; set; }
    }
}