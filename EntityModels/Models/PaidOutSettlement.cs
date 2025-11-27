using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("PaidOutSettlement")]
    public class PaidOutSettlement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PaidOutSettlementId { get; set; }

        public int SalesActivitySummaryId { get; set; }
        [ForeignKey("SalesActivitySummaryId")]
        public virtual SalesActivitySummary SalesActivitySummaries { get; set; }

        [MaxLength(500)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Boolean IsActive { get; set; }
    }
}