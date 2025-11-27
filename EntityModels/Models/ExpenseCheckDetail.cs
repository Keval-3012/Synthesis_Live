using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("ExpenseCheckDetail")]
    public class ExpenseCheckDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ExpenseCheckDetailId { get; set; }

        public int ExpenseCheckId { get; set; }
        [ForeignKey("ExpenseCheckId")]
        public virtual ExpenseCheck ExpenseCheck { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public decimal? Amount { get; set; }

        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual CustomerMaster CustomerMaster { get; set; }

        public string Description { get; set; }

        public bool Flag { get; set; }

        public bool IncludeBySetting { get; set; }
    }
}