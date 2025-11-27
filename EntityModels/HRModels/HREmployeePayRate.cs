using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_EmployeePayRate")]
    public class HREmployeePayRate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int EmployeePayRateId { get; set; }

        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual HREmployeeMaster HREmployeeMaster { get; set; }


        public int? EmployeeChildId { get; set; }
        [ForeignKey("EmployeeChildId")]
        public virtual HREmployeeChild HREmployeeChild { get; set; }


        public DateTime? PayRateDate { get; set; }
        [NotMapped]
        public string sPayRateDate { get; set; }

        public PayType? PayType { get; set; }

        [NotMapped]
        public string PayTypeName
        {
            get
            {
                return Enum.GetName(typeof(PayType), PayType);
            }
        }


        public decimal? PayRate { get; set; }

        [MaxLength]
        public string Comments { get; set; }

        public bool IsActive { get; set; }  = true;

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

    }
}
