using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    public class LevelsApprover
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int LevelsApproverId { get; set; }

        [MaxLength(200)]
        [Required(ErrorMessage =" ")]
        public string LevelName { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = " ")]
        public string LevelSortName { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? CreatedOn { get; set; }
        public virtual ICollection<UserTypeMaster> UserTypeMasters { get; set; }
        public virtual ICollection<UserTypeModuleApprover> UserTypeModuleApprovers { get; set; }
    }
}