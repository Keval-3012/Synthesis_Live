using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EntityModels.Models
{
    [Table("OptionMaster")]
    public class OptionMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int OptionId { get; set; }

        [MaxLength(200)]
        public string OptionName { get; set; }
        public virtual ICollection<OtherDeposit> OtherDeposits { get; set; }
    }
}