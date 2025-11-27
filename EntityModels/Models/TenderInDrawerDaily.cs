using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("TenderInDrawerDaily")]
    public class TenderInDrawerDaily
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int TenderInDrawerDailyId { get; set; }

        public int? SalesActivitySummaryDailyId { get; set; }
        [ForeignKey("SalesActivitySummaryDailyId")]
        public virtual SalesActivitySummaryDaily SalesActivitySummaryDailys { get; set; }

        [MaxLength(500)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int IsManual { get; set; }
    }
}