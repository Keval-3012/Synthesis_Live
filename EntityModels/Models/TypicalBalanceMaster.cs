using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("TypicalBalanceMaster")]
    public class TypicalBalanceMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int TypicalBalanceId { get; set; }

        [MaxLength(200)]
        public string TypicalBalanceName { get; set; }

        public virtual ICollection<ConfigurationGroup> ConfigurationGroups { get; set; }
        public virtual ICollection<Configuration> Configurations { get; set; }
        public virtual ICollection<Departmentconfiguration> Departmentconfigurations { get; set; }
    }
}