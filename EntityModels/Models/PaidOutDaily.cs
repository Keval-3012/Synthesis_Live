using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("PaidOutDaily")]
    public class PaidOutDaily
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PaidOutDailyId { get; set; }

        public int? SalesActivitySummaryDailyId { get; set; }
        [ForeignKey("SalesActivitySummaryDailyId")]
        public virtual SalesActivitySummaryDaily salesActivitySummaryDaily { get; set; }

        [MaxLength(500)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Boolean IsActive { get; set; }
    }
}