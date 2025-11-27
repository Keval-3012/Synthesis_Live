using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("SalesActivitySummary")]
    public class SalesActivitySummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int SalesActivitySummaryId { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? StoreTerminalId { get; set; }
        [ForeignKey("StoreTerminalId")]
        public virtual StoreTerminal StoreTerminals { get; set; }

        public DateTime? TransactionStartTime { get; set; }

        public DateTime? TransactionEndTime { get; set; }

        public decimal CustomerCount { get; set; }

        public decimal NetSalesWithTax { get; set; }

        public decimal TotalTaxCollected { get; set; }

        public decimal AverageSale { get; set; }

        [MaxLength(500)]
        public string FileName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public Boolean IsActive { get; set; }

        public int? ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public virtual ShiftMaster ShiftMasters { get; set; }

        [MaxLength(200)]
        public string CashierName { get; set; }

        public DateTime? StartDate { get; set; }

        public decimal CashierNegative { get; set; }

        public string Notes { get; set; }

        public virtual ICollection<TenderInDrawer> TenderInDrawers { get; set; }
        public virtual ICollection<DepartmentNetSales> DepartmentNetSales { get; set; }
        public virtual ICollection<PaidOut> PaidOuts { get; set; }
        public virtual ICollection<PaidOutSettlement> PaidOutSettlements { get; set; }
        public virtual ICollection<CreditcardDetails> creditcardDetails { get; set; }
        public virtual ICollection<OtherDeposit> OtherDeposits { get; set; }
        public virtual ICollection<CashPaidoutInvoice> CashPaidoutInvoices { get; set; }
        public virtual ICollection<PaidOutDaily> paidOutDaily { get; set; }

        [NotMapped]
        public string StoreName { get; set; }

        [NotMapped]
        public string TerminalName { get; set; }

    }
}