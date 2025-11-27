using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("ConfigurationGroup")]
    public class ConfigurationGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ConfigurationGroupId { get; set; }

        [MaxLength(200)]
        public string GroupName { get; set; }

        public int? TypicalBalanceId { get; set; }
        [ForeignKey("TypicalBalanceId")]
        public virtual TypicalBalanceMaster TypicalBalanceMasters { get; set; }

        [MaxLength(2000)]
        public string Memo { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMaster { get; set; }

        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual CustomerMaster CustomerMasters { get; set; }

        public int? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual VendorMaster VendorMasters { get; set; }

        public virtual ICollection<Configuration> Configurations { get; set; }
        public virtual ICollection<Departmentconfiguration> Departmentconfigurations { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }

        [NotMapped]
        public int Exist { get; set; }

        [NotMapped]
        public string TypicalBalanceName { get; set; }

        [NotMapped]
        public string Entity { get; set; }
    }
}