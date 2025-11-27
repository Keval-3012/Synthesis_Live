using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("DepartmentNetSalesHourly")]
    public class DepartmentNetSalesHourly
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DepartmentNetSalesHourlyId { get; set; }

        public int? SalesActivitySummaryHourlyId { get; set; }
        [ForeignKey("SalesActivitySummaryHourlyId")]
        public virtual SalesActivitySummaryHourly SalesActivitySummaryHourlys { get; set; }

        [MaxLength(500)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Boolean IsActive { get; set; }

        public int HSequence { get; set; }
    }
}