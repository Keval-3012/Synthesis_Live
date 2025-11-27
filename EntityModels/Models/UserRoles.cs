using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    public class UserRoles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UserRoleId { get; set; }

        [Required(ErrorMessage = " ")]
        public int UserTypeId { get; set; }
        [ForeignKey("UserTypeId")]
        public virtual UserTypeMaster UserTypeMasters { get; set; }

        [Required(ErrorMessage = " ")]
        public string Role { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

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

    //public class RightsStore
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    [Editable(false)]
    //    public int RightsStoreId { get; set; }

    //    //public int RightsStoreId { get; set; }
    //    public int ModuleId { get; set; }
    //    [ForeignKey("ModuleId")]
    //    public virtual ModuleMaster ModuleMasters { get; set; }

    //    [Required(ErrorMessage = " ")]
    //    public int UserTypeId { get; set; }
    //    [ForeignKey("UserTypeId")]
    //    public virtual UserTypeMaster UserTypeMasters { get; set; }

    //    [NotMapped]
    //    public string ModuleName { get; set; }
    //    [NotMapped]
    //    public string ModuleDispName { get; set; }

    //    [NotMapped]
    //    public string[] StoreIds { get; set; }
       


    //    public int StoreId { get; set; }
    //    [ForeignKey("StoreId")]
    //    public virtual StoreMaster StoreMasters { get; set; }
    //}
    public class RightsStore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int RightsStoreId { get; set; }

        //public int RightsStoreId { get; set; }
        public int ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual ModuleMaster ModuleMasters { get; set; }

        [Required(ErrorMessage = " ")]
        public int UserTypeId { get; set; }
        [ForeignKey("UserTypeId")]
        public virtual UserTypeMaster UserTypeMasters { get; set; }

        [NotMapped]
        public string ModuleName { get; set; }
        [NotMapped]
        public string ModuleDispName { get; set; }

        [NotMapped]
        public string[] StoreIds { get; set; }

        [NotMapped]
        public string[] DepartmentIds { get; set; }

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }


        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMaster { get; set; }

        [NotMapped]
        public List<DepartmentList> DepartmentLists { get; set; }
    }
    public class DepartmentList
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int StoreId { get; set; }
    }
}