using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("AccountTypeMaster")]
    public class AccountTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int AccountTypeId { get; set; }

        [Required(ErrorMessage ="Required..")]
        [MaxLength(100)]
        public string AccountType { get; set; }

        [MaxLength(50)]
        public string Flag { get; set; }

        [MaxLength(100)]
        public string CommonType { get; set; }

        public virtual ICollection<AccountDetailTypeMaster> AccountDetailTypeMasters { get; set; }
        public virtual ICollection<DepartmentMaster> DepartmentMasters { get; set; }
    }

    public class DistinctAccountType
    {
        public int AccountTypeId { get; set; }
        public string CommonType { get; set; }
    }
}