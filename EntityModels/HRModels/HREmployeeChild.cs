using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_EmployeeChild")]
    public class HREmployeeChild
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int EmployeeChildId { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual HREmployeeMaster HREmployeeMaster { get; set; }

        public int? SrNo { get; set; }

        [MaxLength(255)]
        public string OfficeEmployeeID { get; set; }

        public DateTime? HireDate { get; set; }
        [NotMapped]
        public string sHireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        [NotMapped]
        public string sTerminationDate { get; set; }
        public Status? Status { get; set; }
        [NotMapped]
        public string StatusName
        {
            get
            {
                if (Status.HasValue)
                {
                    return Enum.GetName(typeof(Status), Status.Value);
                }
                return null;
            }
        }
        public EmployeementTypeStatus? EmployeementTypeStatus { get; set; }

        [NotMapped]
        public string EmployeementTypeStatusName
        {
            get
            {
                if (EmployeementTypeStatus.HasValue)
                {
                    return Enum.GetName(typeof(EmployeementTypeStatus), EmployeementTypeStatus.Value);
                }
                return null;
            }
        }
        public int? StoreId { get; set; }
        [NotMapped]
        public string StoreName { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual HRDepartmentMaster HRDepartmentMaster { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        public virtual ICollection<HREmployeePayRate> HREmployeePayRates { get; set; }

        public virtual ICollection<HREmployeeSickTimes> HREmployeeSickTimes { get; set; }

        public virtual ICollection<HREmployeeVacationTime> HREmployeeVacationTimes { get; set; }

        public virtual ICollection<HREmployeeInsurance> HREmployeeInsurances { get; set; }

        public virtual ICollection<HREmployeeDocument> HREmployeeDocuments { get; set; }

        public virtual ICollection<HREmployeeNotes> HREmployeeNotes { get; set; }

        public virtual ICollection<HREmployeeRetirementInfo> HREmployeeRetirementInfos { get; set; }

        public virtual ICollection<HREmployeeHealthBenefitInfo> HREmployeeHealthBenefitInfos { get; set; }

        public virtual ICollection<HREmployeeVaccineCertificateInfo> HREmployeeVaccineCertificateInfos { get; set; }


    }
}
