using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("DepartmentNetSales")]
    public class DepartmentNetSales
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DepartmentNetSalesId { get; set; }

        public int? SalesActivitySummaryId { get; set; }
        [ForeignKey("SalesActivitySummaryId")]
        public virtual SalesActivitySummary SalesActivitySummary { get; set; }

        [MaxLength(500)]
        public string Title { get; set; }

        public decimal Amount { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Boolean IsActive { get; set; }
    }
}