using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("PaidOutHourly")]
    public class PaidOutHourly
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PaidOutHourlyId { get; set; }

        public int? SalesActivitySummaryHourlyId { get; set; }
        [ForeignKey("SalesActivitySummaryHourlyId")]
        public virtual SalesActivitySummaryHourly salesActivitySummaryHourly { get; set; }

        [MaxLength(500)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Boolean IsActive { get; set; }
        public int HSequence { get; set; }
    }
}