using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_EmployeeHealthBenefitInfo")]
    public class HREmployeeHealthBenefitInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int EmployeeHealthBenefitInfoID { get; set; }

        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual HREmployeeMaster HREmployeeMaster { get; set; }

        public int? EmployeeChildId { get; set; }
        [ForeignKey("EmployeeChildId")]
        public virtual HREmployeeChild HREmployeeChild { get; set; }

        public bool OtherCoverage { get; set; } = true;

        [MaxLength(255)]
        public string OtherCoverageDetail { get; set; }

        public bool RefusedCoverage { get; set; } = true;

        [MaxLength]
        public string DocFileName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }
    }
}
