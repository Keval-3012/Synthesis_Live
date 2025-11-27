using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("CreditcardDetailsHourly")]
    public class CreditcardDetailsHourly
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int CreditcardDetailsHourlyId { get; set; }

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

        public int? StoreTerminalHourlylId { get; set; }
        [ForeignKey("StoreTerminalHourlylId")]
        public virtual StoreTerminalHourly StoreTerminalHourlys { get; set; }

        public int? SalesActivitySummaryHourlyId { get; set; }
        [ForeignKey("SalesActivitySummaryHourlyId")]
        public virtual SalesActivitySummaryHourly salesActivitySummaryHourly { get; set; }

        public int HSequence { get; set; }
    }
}