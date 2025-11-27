using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("DepartmentConfiguration")]
    public class Departmentconfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DepartmentConfigurationId { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMaster { get; set; }

        public int? ConfigurationGroupId { get; set; }
        [ForeignKey("ConfigurationGroupId")]
        public virtual ConfigurationGroup ConfigurationGroups { get; set; }

        [MaxLength(1000)]
        public string Title { get; set; }

        [MaxLength(2000)]
        public string Memo { get; set; }

        public int? TypicalBalanceId { get; set; }
        [ForeignKey("TypicalBalanceId")]
        public virtual TypicalBalanceMaster TypicalBalanceMasters { get; set; }

    }
}