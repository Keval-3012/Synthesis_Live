using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace SynthesisViewModal
{
    public class TerminalViewModel
    {
        public int StoreId { get; set; } = 0;
        public string StStoreId { get; set; } = "";
        public string TerminalId { get; set; } = "";
        public string StartDate { get; set; } = "";
        public string shiftid { get; set; } = "";
        public bool IsFirst { get; set; } = false;
        public string Terminalid_val { get; set; } = "";
        public string Shiftid_val { get; set; } = "";
        public DateTime SDate { get; set; }
        public DateTime? CDate { get; set; } = null;
        public int flag { get; set; } = 0;
        public string date { get; set; } = "";
        public string ErrorMessage { get; set; } = string.Empty;
        public string TerminalName { get; set; }
        public int terminal_Id { get; set; } = 0;
        public int shift_Id { get; set; } = 0;
        public int? shift_Id_N { get; set; } = null;
        public string[] Title { get; set; }
        public string[] Amount { get; set; }
        public int SalesActivitySummaryId { get; set; }
        public string ShiftName { get; set; }
        public string CashierName { get; set; }
        public string Notes { get; set; }
        public int sid { get; set; }
        public string payment { get; set; }
        public string amount { get; set; }
        public string options { get; set; }
        public string vendor { get; set; }
        public string Department { get; set; }
        public string Other { get; set; } = "";
        public string name { get; set; } = "";
        public int Terminal { get; set; } = 0;
        public int? Shift { get; set; } = null;
        public int ActivitySalesSummuryId { get; set; } = 0;
        public HttpPostedFileBase UploadFile { get; set; } = null;
        public decimal AmountValue { get; set; }
        public string Ext { get; set; }
        public string[] Extensions { get; set; }
        public string UploadFile_Title { get; set; }
        public int Id { get; set; }
        public int SalesActivityId { get; set; } = 0;
        public string date_val { get; set; } = "";
        public int IsSettlementDone { get; set; } = 0;
        public int UserID { get; set; } = 0;
        public string Store { get; set; }

        public int StoreFlag { get; set; }
        public byte[] fileBytes { get; set; }
        public string fullName { get; set; }
        public string filePath { get; set; }
        public string PathRel { get; set; }
        public string Acc { get; set; }

        public string StatusMessage { get; set; } = "";
        public string ExistCode { get; set; } = "";
        public bool IsArray { get; set; }

        public Invoice invoice { get; set; }
        public HttpPostedFileBase UploadInvoice { get; set; }
        public string[] ChildDepartmentId { get; set; }
        public string[] ChildAmount { get; set; }
        public string addnew { get; set; } = "";
        public string btnsubmit { get; set; } = "";
        public int ShiftID { get; set; } = 0;
        public int PaidOutID { get; set; } = 0;
        public string Sacn_Title { get; set; } = "";
        public string DSacn_Title { get; set; } = "";
        public int InvoiceId { get; set; } = 0;
        public string ActivityLogMessage { get; set; } = "";
    }

    public class TerminalSettlement
    {
        public string Title { get; set; }
        public int SettlementID { get; set; }
        public decimal Amount { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class CloseOut
    {
        public string PendingDayShift { get; set; }
        public decimal TotalSalesamount { get; set; }
        public int SalesGeneralEntries { get; set; }
        public int POSCount { get; set; }
        public decimal SalesTotal { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal PaidOutTotal { get; set; }
        public decimal OverShortVal { get; set; }

    }

    public class CreateInvoiceDetail
    {
        public List<StoreMaster> StoreMasters { get; set; }
        public List<VendorMaster> VendorMasters { get; set; }
        public List<DepartmentMaster> DepartmentMasters { get; set; }
        public List<DepartmentMaster> DepartmentMasters_store { get; set; }
        public List<DiscountTypeMaster> DiscountTypeMasters { get; set; }
        public List<InvoiceTypeMaster> InvoiceTypeMasters { get; set; }
        public List<PaymentTypeMaster> PaymentTypeMasters { get; set; }

    }
}
