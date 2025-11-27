using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_EmployeeVaccineCertificateInfo")]
    public class HREmployeeVaccineCertificateInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int VaccineCertificateInfoID { get; set; }

        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual HREmployeeMaster HREmployeeMaster { get; set; }

        public int? EmployeeChildId { get; set; }
        [ForeignKey("EmployeeChildId")]
        public virtual HREmployeeChild HREmployeeChild { get; set; }

        public bool IsVaccine { get; set; } = true;

        public bool IsExemption { get; set; } = false;

        [MaxLength]
        public string FileName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }
    }
}
