using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("Invoice")]
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = " ")]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [Required(ErrorMessage = " ")]
        public int InvoiceTypeId { get; set; }
        [ForeignKey("InvoiceTypeId")]
        public virtual InvoiceTypeMaster InvoiceTypeMasters { get; set; }

        [Required(ErrorMessage = " ")]
        public int PaymentTypeId { get; set; }
        [ForeignKey("PaymentTypeId")]
        public virtual PaymentTypeMaster PaymentTypeMasters { get; set; }

        [Required(ErrorMessage = " ")]
        public int VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual VendorMaster VendorMasters { get; set; }

        public DateTime InvoiceDate { get; set; }

        //[Required(ErrorMessage = " ")]

        [NotMapped]
        [Required(ErrorMessage = " ")]
        public string strInvoiceDate { get; set; }

        [Required(ErrorMessage = " ")]
        [MaxLength(100)]
        public string InvoiceNumber { get; set; }

        [MaxLength(2000)]
        public string Note { get; set; }

        public string UploadInvoice { get; set; }

        public decimal TotalAmount { get; set; }

        public InvoiceStatusEnm? StatusValue { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public int? ApproveRejectBy { get; set; }

        public DateTime? ApproveRejectDate { get; set; }

        public Boolean IsActive { get; set; }

        [MaxLength(50)]
        public string TXNId { get; set; }

        public int? IsSync { get; set; } = 0;
        public DateTime? SyncDate { get; set; }

        public bool NotificationColor { get; set; }

        public bool QBTransfer { get; set; } = false;

        public int? DiscountTypeId { get; set; }
        [ForeignKey("DiscountTypeId")]
        public virtual DiscountTypeMaster DiscountTypeMasters { get; set; }

        public decimal? DiscountPercent { get; set; }

        public decimal? DiscountAmount { get; set; }
        [NotMapped]
        [Required(ErrorMessage = " ")]
        public int Disc_Dept_id { get; set; }

        public int? RefInvoiceId { get; set; }                    

        public bool? IsPaid { get; set; }

        //Added by Dani on 02-04-2025
        public DateTime? LastModifiedOn { get; set; }

        [NotMapped]
        public Nullable<decimal> RefDiscountAmount { get; set; }
        [NotMapped]
        public string RefInvoiceNumber { get; set; }

        [MaxLength(50)]
        public string Source { get; set; }

        [NotMapped]
        public int? ChildDepartmentId { get; set; }

        [NotMapped]
        public decimal? ChildAmount { get; set; }

        [NotMapped]
        public string VendorName { get; set; }
        [NotMapped]
        public string InvoiceType { get; set; }

        [NotMapped]
        public string StoreName { get; set; }

        [NotMapped]
        public string PaymentType { get; set; }

        [NotMapped]
        public decimal TotalAmtByDept { get; set; }
        [NotMapped]
        public string CreatedByUserName { get; set; }
        [NotMapped]
        public string ModifiedByUserName { get; set; }
        [NotMapped]
        public string VendorPhoneNumber { get; set; }
        [NotMapped]
        public string VendorAddress { get; set; }
        [NotMapped]
        public string ApproveRejectUserName { get; set; }

        [NotMapped]
        public string QuickInvoice { get; set; }

        [NotMapped]
        public string QuickCRInvoice { get; set; }

        public virtual List<InvoiceDepartmentDetail> InvoiceDepartmentDetails { get; set; }

        [NotMapped]
        public List<DepartmentMaster> DepartmentMasters { get; set; }

        [NotMapped]
        public string QBtransferss { get; set; }
        [NotMapped]
        public bool invoiceAproveFlg { get; set; }
        [NotMapped]
        public bool QBStatusField { get; set; }

        public int PDFProcessStatus { get; set; }
        public DateTime? PDFProcessDate { get; set; }

        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }
        public virtual ICollection<CashPaidoutInvoice> CashPaidoutInvoices { get; set; }

        public ICollection<InvoiceProduct> invoiceProducts { get; set; }

        [NotMapped]
        public string FromInvoicePage { get; set; }

        [NotMapped]
        public string Status { get; set; }

        [NotMapped]
        public int? UploadPdfId { get; set; }
        [NotMapped]
        public int? UploadPdfAutomationId { get; set; }

        public int? PDFPageCount { get; set; }
        [MaxLength]
        public string InvoiceReview { get; set; }

        [NotMapped]
        public virtual List<StoreWiseInvoiceDepartmentDetail> SplitInvoiceDepartmentDetails { get; set; }
        [NotMapped]
        public int CheckDup { get; set; }
        [NotMapped]
        public string btnName { get; set; }
        [NotMapped]
        public int UserDropData { get; set; }
        [NotMapped]
        public int PriorityDropData { get; set; }
    }
    [Table("InvoiceProduct")]
    public class InvoiceProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int InvoiceProductId { get; set; }

        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoices { get; set; }

        public string UPCCode { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public int? Department { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal ScannedTotal { get; set; }
        public bool Approved { get; set; }
        public decimal? UPCCode_Accuracy { get; set; }
        public decimal? ItemNo_Accuracy { get; set; }
        public decimal? Description_Accuracy { get; set; }
        public decimal? Qty_Accuracy { get; set; }
        public decimal? UnitPrice_Accuracy { get; set; }
        public decimal? Total_Accuracy { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products products { get; set; }
        public int? ProductVendorId { get; set; }
        [ForeignKey("ProductVendorId")]
        public virtual ProductVendor ProductVendors { get; set; }
        public int? StoreID { get; set; }
    }
    [Table("InvoiceDepartmentDetail")]
    public class InvoiceDepartmentDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int InvoiceDepartmentId { get; set; }

        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoices { get; set; }

        [Required(ErrorMessage = " ")]
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        //[Required(ErrorMessage = " ")]
        public decimal Amount { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }
        [NotMapped]
        public string DepartmentListId { get; set; }
    }
    public enum InvoiceStatusEnm
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        OnHold = 4
    }
    public class InvoiceSelect
    {
        public int InvoiceId { get; set; }
        public int StoreId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int PaymentTypeId { get; set; }
        public int VendorId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string strInvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Note { get; set; }
        public string UploadInvoice { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatusEnm StatusValue { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public int? ApproveRejectBy { get; set; }
        public DateTime? ApproveRejectDate { get; set; }
        public Boolean IsActive { get; set; }
        public string TXNId { get; set; }
        public int? IsSync { get; set; } = 0;
        public DateTime? SyncDate { get; set; }
        public bool NotificationColor { get; set; }
        public bool QBTransfer { get; set; }
        public int? DiscountTypeId { get; set; }
        //public virtual DiscountTypeMaster DiscountTypeMasters { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int? Disc_Dept_id { get; set; }
        public int? RefInvoiceId { get; set; }
        public string Source { get; set; }
        public int? ChildDepartmentId { get; set; }
        public decimal? ChildAmount { get; set; }
        public string VendorName { get; set; }
        public string InvoiceType { get; set; }
        public string StoreName { get; set; }
        public string PaymentType { get; set; }
        public decimal? TotalAmtByDept { get; set; }
        public string CreatedByUserName { get; set; }
        public string ModifiedByUserName { get; set; }
        public int? IsEdit { get; set; }
        public int? IsDelete { get; set; }
        public int? IsQbStatus { get; set; }
        public int? IsQbFStatus { get; set; }

        public int? departmentid { get; set; }

        public string InvoiceDepartmentDetail { get; set; }
        public bool? IsPaid { get; set; }
    }

    public class InvoiceSelect_BillDetail
    {
        public string TxnID { get; set; }
        public string ID { get; set; }
        public string LineNum { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Description { get; set; }
    }

    public class CheckExistInvoice
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public int? InvoiceId { get; set; }
    }

    public class Invoice_Select_ForPdf
    {
        public string VendorName { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceNumber { get; set; }
        public string CreatedOn { get; set; }
        public string InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentType { get; set; }

    }
    public class Invoice_Notification
    {
        public int InvoiceId { get; set; }
        public int StoreId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string StoreName { get; set; }
        public string UserName { get; set; }
        public Boolean NotificationColor { get; set; }
    }
    public class InvoiceProductSelect
    {
        public int InvoiceProductId { get; set; }
        public string VendorName { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string StoreName { get; set; }
        public int InvoiceId { get; set; }
        public string UPCCode { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public int? ProductId { get; set; }
        public decimal? UPCCode_Accuracy { get; set; }
        public decimal? ItemNo_Accuracy { get; set; }
        public decimal? Description_Accuracy { get; set; }
        public decimal? Qty_Accuracy { get; set; }
        public decimal? UnitPrice_Accuracy { get; set; }
        public decimal? Total_Accuracy { get; set; }
        public bool Approved { get; set; }

        public string Approvevalue { get; set; }
    }
    public class Invoice_Dashbord_Select
    {
        public int id { get; set; }
        public int Store_id { get; set; }
        public int Type_id { get; set; }
        public string Payment_type { get; set; }
        public int Vendor_id { get; set; }

        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Invoice_Date { get; set; }
        public string Invoice_Date_str { get; set; }
        public string Invoice_Number { get; set; }
        public string Note { get; set; }
        public string AttachNote { get; set; }
        public string UploadInvoice { get; set; }
        public string ScanInvoice { get; set; }
        public int IsStatus_id { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string Storename { get; set; }
        public List<ddllist> BindStoreList { get; set; }
        public string Type { get; set; }
        public List<ddllist> BindTypeList { get; set; }
        public string Vendorname { get; set; }
        public List<ddllist> BindVendorList { get; set; }
        public string Departmentname { get; set; }
        public List<ddllist> BindDepartmentList { get; set; }
        public Nullable<decimal> Amt { get; set; }
        public decimal TotalAmtByDept { get; set; }
        public string address { get; set; }
        public string ApproveRejectBy { get; set; }
        public DateTime ApproveRejectDate { get; set; }
        public int deptid { get; set; }
        public string StrInvoice_Date { get; set; }

        public string Tooltip { get; set; }
        public HttpPostedFileBase DocFiles { get; set; }

    }

    public class ViewInvoice
    {
        public int? InvoiceCount { get; set; }
        public string Name { get; set; }
    }





    public class SplitInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int InvoiceId { get; set; }

       

        [Required(ErrorMessage = " ")]
        public int InvoiceTypeId { get; set; }
     
        public virtual InvoiceTypeMaster InvoiceTypeMasters { get; set; }

        [Required(ErrorMessage = " ")]
        public int PaymentTypeId { get; set; }
     
        public virtual PaymentTypeMaster PaymentTypeMasters { get; set; }
             
        

        public DateTime InvoiceDate { get; set; }

        //[Required(ErrorMessage = " ")]

      
        [Required(ErrorMessage = " ")]
        public string strInvoiceDate { get; set; }

        [Required(ErrorMessage = " ")]
        [MaxLength(100)]
        public string InvoiceNumber { get; set; }

        [MaxLength(2000)]
        public string Note { get; set; }

        public string UploadInvoice { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal InvoiceAmount { get; set; }

        public InvoiceStatusEnm? StatusValue { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public int? ApproveRejectBy { get; set; }

        public DateTime? ApproveRejectDate { get; set; }

        public Boolean IsActive { get; set; }

        [MaxLength(50)]
        public string TXNId { get; set; }

        public int? IsSync { get; set; } = 0;
        public DateTime? SyncDate { get; set; }

        public bool NotificationColor { get; set; }

        public bool QBTransfer { get; set; } = false;


        public int? RefInvoiceId { get; set; }

        [NotMapped]
        public string RefInvoiceNumber { get; set; }

        [MaxLength(50)]
        public string Source { get; set; }

        [NotMapped]
        public int? ChildDepartmentId { get; set; }

        [NotMapped]
        public decimal? ChildAmount { get; set; }

        [NotMapped]
        public string VendorName { get; set; }
        [NotMapped]
        public string InvoiceType { get; set; }

        [NotMapped]
        public string StoreName { get; set; }

        [NotMapped]
        public string PaymentType { get; set; }

        [NotMapped]
        public decimal TotalAmtByDept { get; set; }
        [NotMapped]
        public string CreatedByUserName { get; set; }
        [NotMapped]
        public string ModifiedByUserName { get; set; }
        [NotMapped]
        public string VendorPhoneNumber { get; set; }
        [NotMapped]
        public string VendorAddress { get; set; }
        [NotMapped]
        public string ApproveRejectUserName { get; set; }

        [NotMapped]
        public string QuickInvoice { get; set; }      

        public virtual List<StoreWiseInvoiceDepartmentDetail> InvoiceDepartmentDetails { get; set; }

        [NotMapped]
        public List<DepartmentMaster> DepartmentMasters { get; set; }

        [NotMapped]
        public string QBtransferss { get; set; }
        [NotMapped]
        public bool invoiceAproveFlg { get; set; }
        [NotMapped]
        public bool QBStatusField { get; set; }

        public int PDFProcessStatus { get; set; }
        public DateTime? PDFProcessDate { get; set; }

        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }
        public virtual ICollection<CashPaidoutInvoice> CashPaidoutInvoices { get; set; }

        public ICollection<InvoiceProduct> invoiceProducts { get; set; }

        [NotMapped]
        public string FromInvoicePage { get; set; }

        [NotMapped]
        public string Status { get; set; }

        [NotMapped]
        public int? UploadPdfId { get; set; }
        [NotMapped]
        public int? UploadPdfAutomationId { get; set; }
        [NotMapped]
        public int CheckDup { get; set; }

        public int? PDFPageCount { get; set; }

        public SplitType SplitTypelist { get; set; }

        public List<Store> StoreMaster { get; set; }

        public HttpPostedFileBase UploadInvoiceTemp { get; set; }

    }

    public class StoreWiseInvoiceDepartmentDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int InvoiceDepartmentId { get; set; }

        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoices { get; set; }

        [Required(ErrorMessage = " ")]
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        [Required(ErrorMessage = " ")]
        public decimal Amount { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }
        [NotMapped]
        public string DepartmentListId { get; set; }
        public virtual StoreMaster StoreMasters { get; set; }
        public string StoreId { get; set; }

        public virtual VendorMaster VendorMasters { get; set; }
        public string VendorId { get; set; }
    }

    public enum SplitType
    {
        SplitEqually,
        Percentage,
        Custom
    }

    public class Store
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public bool Selected { get; set; }

        public int? Percentage { get; set; }
    }

    public class InvoiceFlgs 
    {
        public int iInvoiceId { get; set; }
        public string iInvoiceStatus { get; set; }
    }
    public class AttachmentNoteCls 
    {
        public string StatusMessage { get; set; }
        public string InvoiceNM { get; set; }
    }

    public class InvoiceCount
    {
        public int TotalCount { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }

    }

    public class InvoiceFilter
    {
        public string VendorName { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceNumber { get; set; }
        public string StoreName { get; set; }
        public string InvoiceDate { get; set; }
        public string TotalAmount { get; set; }
        public string PaymentType { get; set; }
    }
    public class Predicate
    {
        public string Field { get; set; }
        public bool IgnoreCase { get; set; }
        public bool IsComplex { get; set; }
        public string Operator { get; set; }
        public string Condition { get; set; }
        public object Value { get; set; }
        public List<Predicate> Predicates { get; set; }
    }
}