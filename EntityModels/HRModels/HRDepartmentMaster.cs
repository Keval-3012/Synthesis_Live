using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_DepartmentMaster")]
    public class HRDepartmentMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department Name is Required.")]
        [MaxLength(300)]
        public string DepartmentName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        [NotMapped]
        public string Status { get; set; }

        public virtual ICollection<HREmployeeChild> HREmployeeChild { get; set; }

    }
}
