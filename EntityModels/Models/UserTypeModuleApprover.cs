using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace EntityModels.Models
{
    [Table("UserTypeModuleApprover")]
    public class UserTypeModuleApprover
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UserTypeModuleApproverId { get; set; }

        public int UserTypeId { get; set; }
        [ForeignKey("UserTypeId")]
        public virtual UserTypeMaster UserTypeMasters { get; set; }

        public int ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual ModuleMaster ModuleMasters { get; set; }

        public int LevelsApproverId { get; set; }
        [ForeignKey("LevelsApproverId")]
        public virtual LevelsApprover LevelsApprovers { get; set; }
    }
}