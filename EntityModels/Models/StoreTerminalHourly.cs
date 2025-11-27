using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("StoreTerminalHourly")]
    public class StoreTerminalHourly
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int StoreTerminalHourlylId { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? TerminalHourlyId { get; set; }
        [ForeignKey("TerminalHourlyId")]
        public virtual TerminalHourlyMaster TerminalHourlyMasters { get; set; }

        public Boolean IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int HSequence { get; set; }

        public virtual ICollection<SalesActivitySummaryHourly> salesActivitySummaryHourly { get; set; }
        public virtual ICollection<CreditcardDetailsHourly> creditcardDetailsHourly { get; set; }
        //public virtual ICollection<OtherDeposit> OtherDeposits { get; set; }
        //public virtual ICollection<CashPaidoutInvoice> CashPaidoutInvoices { get; set; }
    }
}