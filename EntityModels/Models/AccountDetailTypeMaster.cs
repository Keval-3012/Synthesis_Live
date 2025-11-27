using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("AccountDetailTypeMaster")]
    public class AccountDetailTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int AccountDetailTypeId { get; set; }

        public int AccountTypeId { get; set; }
        [ForeignKey("AccountTypeId")]
        public virtual AccountTypeMaster AccountTypeMasters { get; set; }

        [MaxLength(100)]
        public string DetailType { get; set; }

        [MaxLength(100)]
        public string QBDetailType { get; set; }

        public virtual ICollection<DepartmentMaster> DepartmentMasters { get; set; }
    }
    public class AccountDetailTypeMasterSelect
    {
        public int AccountDetailTypeId { get; set; }
        public string DetailType { get; set; }

    }
}