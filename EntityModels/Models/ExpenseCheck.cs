using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("ExpenseCheck")]
    public class ExpenseCheck
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ExpenseCheckId { get; set; }

        public int? BankAccountId { get; set; }
        [ForeignKey("BankAccountId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public PaymentType PaymentType { get; set; }

        public int? VendorId { get; set; }        

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public decimal? TotalAmt { get; set; }

        [MaxLength(50)]
        public string TXNId { get; set; }

        public string SyncToken { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? LastUpdatedTime { get; set; }

        public DateTime CreateOn { get; set; }

        public string DocNumber { get; set; }

        public DateTime? TxnDate { get; set; }
        
        public string Memo { get; set; }

        public string QBType { get; set; }

        public string RefType { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool? Isdeleted { get; set; } = false;

        public bool PrintLater { get; set; } = false;

        public string MailingAddress { get; set; }

        public bool IsSync { get; set; } = false;

        public int? PaymentMethodId { get; set; }        

        public DateTime? PaymentDate { get; set; }

        public string QBErrorMessage { get; set; }

        //Added by Dani on 02-04-2025
        public DateTime? LastModifiedOn { get; set; }

        [NotMapped]
        public string FileName { get; set; }
        [NotMapped]
        public string VendorIdstr { get; set; }

        [NotMapped]
        public string SavebtnName { get; set; }
        [NotMapped]
        public int? PaymentTypeId { get; set; }

        public virtual ICollection<ExpenseCheckDetail> ExpenseCheckDetails { get; set; }

        public virtual ICollection<ExpenseCheckDocuments> expenseCheckDocuments { get; set; }
    }

    public enum PaymentType
    {
        Check=1,
        Expense =2
    }

    public class ExpenseCheckSelect
    {
        public string VendorName { get; set; }
        public string Type { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime CreateOn { get; set; }
        public string StoreName { get; set; }
        public decimal Amount { get; set; }
        public int InvoiceId { get; set; }
        public string Department { get; set; }
        public string Memo { get; set; }
        public string DocNumber { get; set; }
        public int ExpenseCheckDetailId { get; set; }
        public int ExpenseCheckId { get; set; }
        public int ViewDocFlg { get; set; }
        public string QBType { get; set; }

        public string DisplayExpenseStatus { get; set; }
        public string ExpenseStatus { get; set; }
        [NotMapped]
        public string AmountString { get; set; }
        public bool PrintLater { get; set; }
        public bool IsSync { get; set; }
        public string QBErrorMessage { get; set; }
        public string PaymentAccountName { get; set; }
    }
    public class ExpenseCheckExcludedList
    {
        public string VendorName { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string StoreName { get; set; }
        public decimal Amount { get; set; }
        public string Department { get; set; }
        public string Memo { get; set; }
        public string DocNumber { get; set; }
        public int ExpenseCheckDetailId { get; set; }
    }
    public class ExpenseCheckExport
    {
        public string VendorName { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }

        public string Date { get; set; }
        
        public decimal Amount { get; set; }
        public string Department { get; set; }
        public string Memo { get; set; }

    }

    public class UploadedFiles
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
        public string FileUrl { get; set; } // URL for the file
    }

    public class CheckPdfPrint
    {
        public int ExpenseCheckId {  get; set; }
        public string Payee { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Address { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalAmountInWord { get; set; }
        public string AccountName { get; set; }
        public string Memo { get; set; }
        public List<CheckPdfPrintDetails> CheckPdfPrintDetails { get; set; }
    }

    public class CheckPdfPrintDetails
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    public class DropdownViewModelExpense
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class ExpenseCheckSelectMain
    {
        public string VendorName { get; set; }
        public string Type { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime CreateOn { get; set; }
        public string StoreName { get; set; }
        public decimal Amount { get; set; }
        public int InvoiceId { get; set; }
        public string Department { get; set; }
        public string Memo { get; set; }
        public string DocNumber { get; set; }
        public int ExpenseCheckDetailId { get; set; }
        public int ExpenseCheckId { get; set; }
        public int ViewDocFlg { get; set; }
        public string QBType { get; set; }

        public string DisplayExpenseStatus { get; set; }
        public string ExpenseStatus { get; set; }
        [NotMapped]
        public string AmountString { get; set; }
        public bool PrintLater { get; set; }
        public bool IsSync { get; set; }
        public string QBErrorMessage { get; set; }
        public string PaymentAccountName { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int? StoreId { get; set; }
        public DateTime? TxnDate { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? VendorId { get; set; }
        public string RefType { get; set; }
        public int? BankAccountId { get; set; }
        public decimal? TotalAmt { get; set; }
        public string PaymentMethodName { get; set; }
    }
}