using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models 
{
    #region Model Created By Dani for Add Expense Check Module on 5th Dec 2024
    [Table("ExpenseCheckMaster")]
    public class ExpenseCheckMaster 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ExpenseCheckMasterId { get; set; }

        public int? BankAccountId { get; set; }
        [ForeignKey("BankAccountId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public int? PaymentMethodId { get; set; }  

        public int? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual VendorMaster VendorMaster { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public decimal? TotalAmt { get; set; }                           

        public string DocNumber { get; set; }

        [MaxLength(700)]
        public string Memo { get; set; }

        public string RefType { get; set; }

        public string FileName { get; set; }

        public string SyncStatus { get; set; }

        [MaxLength(50)]
        public string TXNId { get; set; }

        public DateTime? TxnDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<ExpenseCheckMasterDetails> ExpenseCheckMasterDetails { get; set; }
    }

    public class ExpenseCheckList
    {
        public int ExpenseCheckMasterId { get; set; }
        public string VendorName { get; set; }
        public string AccountType { get; set; }
        public string PaymentMethod { get; set; }
        public decimal? TotalAmt { get; set; }
        public string DocNumber { get; set; }
        public string FileName { get; set; }
        public string ActualFileName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class DropdownViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
    #endregion
}