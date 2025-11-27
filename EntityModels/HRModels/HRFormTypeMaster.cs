using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.HRModels
{
    [Table("HR_FormTypeMaster")]
    public class HRFormTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int FormTypeId { get; set; }

        [Required(ErrorMessage = "Form Type Name is Required.")]
        [MaxLength(150)]
        public string FormTypeName { get; set; }

        public DateTime? CreatedOn { get; set; } = DateTime.Now;

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        public virtual ICollection<HRConsentMaster> HRConsentMasters { get; set; }

    }
}
