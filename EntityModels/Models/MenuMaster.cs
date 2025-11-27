using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityModels.HRModels;

namespace EntityModels.Models
{
    [Table("MenuMaster")]
    public class MenuMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int MenuId { get; set; }

        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(350)]
        public string MenuUrl { get; set; }

        public int? ParentMenuId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string MenuRoleList { get; set; }

        public virtual ICollection<UserMaster> UserMasters { get; set; }
    }

    public class MenuMasterDto
    {
        public int MenuId { get; set; }
        public string Title { get; set; }
        public string MenuUrl { get; set; }
        public int? ParentMenuId { get; set; }
    }
}