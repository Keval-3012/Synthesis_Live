using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("OperatingRatioExpenseDepartment")]
    public class OperatingRatioExpenseDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int OperatingRatioExpenseDepartmentId { get; set; }
        public int AccountNumber { get; set; }
        [MaxLength(200)]
        public string DepartmentName { get; set; }
    }
}