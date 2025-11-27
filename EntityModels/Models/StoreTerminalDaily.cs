using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("StoreTerminalDaily")]
    public class StoreTerminalDaily
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int StoreTerminalDailylId { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? TerminalDailyId { get; set; }
        [ForeignKey("TerminalDailyId")]
        public virtual TerminalDailyMaster TerminalDailyMasters { get; set; }

        public Boolean IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public virtual ICollection<SalesActivitySummaryDaily> SalesActivitySummaryDailys { get; set; }
        public virtual ICollection<CreditcardDetailsDaily> creditcardDetailsDaily { get; set; }
        //public virtual ICollection<OtherDeposit> OtherDeposits { get; set; }
        //public virtual ICollection<CashPaidoutInvoice> CashPaidoutInvoices { get; set; }
    }
}