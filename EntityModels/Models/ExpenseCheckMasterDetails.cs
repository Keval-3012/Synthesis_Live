using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    #region Model Created By Dani for Add Expense Check Module on 5th Dec 2024
    [Table("ExpenseCheckMasterDetails")]
    public class ExpenseCheckMasterDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ExpenseCheckMasterDetailId { get; set; }

        public int ExpenseCheckMasterId { get; set; }
        [ForeignKey("ExpenseCheckMasterId")]
        public virtual ExpenseCheckMaster ExpenseCheckMaster { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public decimal? Amount { get; set; }

        public string Description { get; set; }
    }
    #endregion 
}