using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("ModuleMaster")]
    public class ModuleMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ModuleId { get; set; }

        public int ModuleNo { get; set; }

        public string ModuleName { get; set; }

        public string DisplayName { get; set; }

        public virtual ICollection<RightsStore> RightsStores { get; set; }
        public virtual ICollection<UserRoles> userRoles { get; set; }
        public virtual ICollection<UserTypeModuleApprover> UserTypeModuleApprovers { get; set; }
    }
}