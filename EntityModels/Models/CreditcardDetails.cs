using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("CreditcardDetails")]
    public class CreditcardDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int CreditcardDetailId { get; set; }

        [Required(ErrorMessage = " ")]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public virtual ShiftMaster ShiftMasters { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount_AMEX { get; set; }

        public decimal Amount_Discover { get; set; }

        public decimal Amount_Master { get; set; }

        public decimal Amount_Visa { get; set; }

        public decimal Amount_CCOffline { get; set; }

        public int? StoreTerminalId { get; set; }
        [ForeignKey("StoreTerminalId")]
        public virtual StoreTerminal StoreTerminals { get; set; }

        public int? SalesActivitySummaryId { get; set; }
        [ForeignKey("SalesActivitySummaryId")]
        public virtual SalesActivitySummary SalesActivitySummaries { get; set; }
    }
}