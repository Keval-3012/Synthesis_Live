using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("ExpenseCheck_Setting")]
    public class ExpenseCheck_Setting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ExpenseCheckId { get; set; }

        public int? AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public bool IsActive { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int Type { get; set; }

        public string ExcludeList { get; set; }

        [NotMapped]
        public List<SpExpenseCheck_Setting> ExpenseCheck_SettingList { get; set; }
    }

    public class SpExpenseCheck_Setting
    {   
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool? IsActive { get; set; }
        public int Type { get; set; }
        public int StoreId { get; set; }
        public List<DrpListStr> DeptExcludeList { get; set; }
        public string ExcludeList { get; set; }
    }

}