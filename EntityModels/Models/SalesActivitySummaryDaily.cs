using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("SalesActivitySummaryDaily")]
    public class SalesActivitySummaryDaily
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int SalesActivitySummaryDailyId { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? StoreTerminalDailylId { get; set; }
        [ForeignKey("StoreTerminalDailylId")]
        public virtual StoreTerminalDaily storeTerminalDaily { get; set; }

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

        [MaxLength(200)]
        public string CashierName { get; set; }

        public DateTime? StartDate { get; set; }

        public decimal CashierNegative { get; set; }

        public string Notes { get; set; }

        public decimal AllVoids { get; set; }
        public decimal ItemCorrects { get; set; }
        public decimal ItemReturns { get; set; }
        

        public virtual ICollection<TenderInDrawerDaily> tenderInDrawerDaily { get; set; }
        public virtual ICollection<DepartmentNetSalesDaily> departmentNetSalesDaily { get; set; }
        public virtual ICollection<CreditcardDetailsDaily> creditcardDetailsDaily { get; set; }
        public virtual ICollection<PaidOutDaily> paidOutDaily { get; set; }

        [NotMapped]
        public string StoreName { get; set; }

        [NotMapped]
        public string TerminalName { get; set; }

    }
}