using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("TenderInDrawerHourly")]
    public class TenderInDrawerHourly
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int TenderInDrawerHourlyId { get; set; }

        public int? SalesActivitySummaryHourlyId { get; set; }
        [ForeignKey("SalesActivitySummaryHourlyId")]
        public virtual SalesActivitySummaryHourly SalesActivitySummaryHourlys { get; set; }

        [MaxLength(500)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int IsManual { get; set; }
        public int HSequence { get; set; }
    }
}