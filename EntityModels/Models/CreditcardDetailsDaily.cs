using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("CreditcardDetailsDaily")]
    public class CreditcardDetailsDaily
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int CreditcardDetailsDailyId { get; set; }

        [Required(ErrorMessage = " ")]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount_AMEX { get; set; }

        public decimal Amount_Discover { get; set; }

        public decimal Amount_Master { get; set; }

        public decimal Amount_Visa { get; set; }

        public decimal Amount_CCOffline { get; set; }

        public int? StoreTerminalDailylId { get; set; }
        [ForeignKey("StoreTerminalDailylId")]
        public virtual StoreTerminalDaily StoreTerminalDailys { get; set; }

        public int? SalesActivitySummaryDailyId { get; set; }
        [ForeignKey("SalesActivitySummaryDailyId")]
        public virtual SalesActivitySummaryDaily salesActivitySummaryDaily { get; set; }
    }
}