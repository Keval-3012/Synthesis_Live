using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    public class UserWiseRights
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UserWiseRightsId { get; set; }

        [Required(ErrorMessage = " ")]
        public int UserTypeId { get; set; }
        [ForeignKey("UserTypeId")]
        public virtual UserTypeMaster UserTypeMasters { get; set; }

        [Required(ErrorMessage = " ")]
        public string Role { get; set; }

        public int? StoreId { get; set; }        
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserMaster UserMasters { get; set; }

        [NotMapped]
        public string UserType { get; set; }
        [NotMapped]
        public List<string> UserForms { get; set; }
        [NotMapped]
        public int? GroupId { get; set; }
        
        public int? ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual ModuleMaster ModuleMasters { get; set; }
        [NotMapped]     
        public List<RightsStore> RightsStoreLists { get; set; }
    }
}