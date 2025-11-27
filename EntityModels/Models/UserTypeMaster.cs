using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("UserTypeMaster")]
    public class UserTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UserTypeId { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(100)]
        public string UserType { get; set; }

        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual GroupMaster GroupMasters { get; set; }

        public int? LevelsApproverId { get; set; }
        [ForeignKey("LevelsApproverId")]
        public virtual LevelsApprover LevelsApprovers { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? CreatedOn { get; set; }

        public bool IsViewInvoiceOnly { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string LevelName { get; set; }
        [NotMapped]
        public string LevelSortName { get; set; }
        public virtual  ICollection<UserMaster> UserMasters { get; set; }
        public virtual ICollection<UserRoles> userRoles { get; set; }
        public virtual ICollection<RightsStore> RightsStores { get; set; }

        public virtual ICollection<UserTypeModuleApprover> UserTypeModuleApprovers { get; set; }
    }
}