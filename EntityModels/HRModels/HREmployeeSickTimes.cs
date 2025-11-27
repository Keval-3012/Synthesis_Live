using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_EmployeeSickTimes")]
    public class HREmployeeSickTimes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int EmployeeSickTimeId { get; set; }

        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual HREmployeeMaster HREmployeeMaster { get; set; }


        public int? EmployeeChildId { get; set; }
        [ForeignKey("EmployeeChildId")]
        public virtual HREmployeeChild HREmployeeChild { get; set; }

        public DateTime? EffectiveDate { get; set; }
        [NotMapped]
        public string sEffectiveDate { get; set; }
        public TimeType? TimeType { get; set; }
        [NotMapped]
        public string TimeTypeName
        {
            get
            {
                return Enum.GetName(typeof(TimeType), TimeType);
            }
        }

        [NotMapped]
        public int Used
        { 
            get 
            {
                if (Enum.GetName(typeof(TimeType), TimeType) == "Used")
                {
                    return Convert.ToInt32(Time);
                }
                else
                {
                    return 0;
                }
            } 
        }
        [NotMapped]
        public int Awarded {
            get
            {
                if (Enum.GetName(typeof(TimeType), TimeType) == "Awarded")
                {
                    return Convert.ToInt32(Time);
                }
                else
                {
                    return 0;
                }
            }
        }

        public int? Time { get; set; }

        [MaxLength]
        public string Comments { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }
    }
}
