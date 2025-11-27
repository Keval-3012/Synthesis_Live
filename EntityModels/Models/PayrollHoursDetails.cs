using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("PayrollHoursDetails")]
    public class PayrollHoursDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PayrollHoursDetailId { get; set; }

        [Required(ErrorMessage = "*")]
        public int PayrollHoursId { get; set; }
        [ForeignKey("PayrollHoursId")]
        public virtual PayrollHours payrollHours { get; set; }


        [Required(ErrorMessage = "*")]
        public int PayrollDepartmentId { get; set; }
        [ForeignKey("PayrollDepartmentId")]
        public virtual PayrollDepartment payrollDepartment { get; set; }

        [Required(ErrorMessage = "Required..")]
        [MaxLength(15)]
        public string RegHours { get; set; }

        [Required(ErrorMessage = "Required..")]
        [MaxLength(15)]
        public string Overtime { get; set; }

        [MaxLength(50)]
        public string SalaryTime { get; set; }
    }
}