using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("StoreTerminal")]
    public class StoreTerminal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int StoreTerminalId { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? TerminalId { get; set; }
        [ForeignKey("TerminalId")]
        public virtual TerminalMaster TerminalMasters { get; set; }

        public Boolean IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public virtual ICollection<SalesActivitySummary> SalesActivitySummaries { get; set; }
        public virtual ICollection<CreditcardDetails> creditcardDetails { get; set; }
        public virtual ICollection<OtherDeposit> OtherDeposits { get; set; }
        public virtual ICollection<CashPaidoutInvoice> CashPaidoutInvoices { get; set; }
        public virtual ICollection<SalesActivitySummaryDaily> SalesActivitySummaryDailys { get; set; }
    }
}