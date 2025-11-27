using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("SalesActivitySummaryHourly")]
    public class SalesActivitySummaryHourly
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int SalesActivitySummaryHourlyId { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? StoreTerminalHourlylId { get; set; }
        [ForeignKey("StoreTerminalHourlylId")]
        public virtual StoreTerminalHourly storeTerminalHourly { get; set; }

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

       

        public int HSequence { get; set; }

        public decimal AllVoidsAmt { get; set; }
        public decimal ItemCorrectsAmt { get; set; }
        public decimal ReturnsAmt { get; set; }

        public int AllVoids { get; set; }
        public int ItemCorrects { get; set; }
        public int ItemReturns { get; set; }



        public virtual ICollection<TenderInDrawerHourly> tenderInDrawerHourly { get; set; }
        public virtual ICollection<DepartmentNetSalesHourly> departmentNetSalesHourly { get; set; }
        public virtual ICollection<CreditcardDetailsHourly> creditcardDetailsHourly { get; set; }
        public virtual ICollection<PaidOutHourly> paidOutHourly { get; set; }

        [NotMapped]
        public string StoreName { get; set; }

        [NotMapped]
        public string TerminalName { get; set; }

    }
}