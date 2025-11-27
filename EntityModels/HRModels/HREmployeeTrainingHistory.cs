using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_EmployeeTrainingHistory")]
    public class HREmployeeTrainingHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int EmployeeTrainingHistoryId { get; set; }

        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual HREmployeeMaster HREmployeeMaster { get; set; }

        [MaxLength]
        public string TraningFilePath { get; set; }

        public bool IsTrainingCompleted { get; set; } = false;

        [MaxLength] 
        public string TrainingContent { get; set; }

        public DateTime? TrainingCompletedDateTime { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }
    }
}
