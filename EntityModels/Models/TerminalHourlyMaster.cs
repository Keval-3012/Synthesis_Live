using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("TerminalHourlyMaster")]
    public class TerminalHourlyMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int TerminalHourlyId { get; set; }

        [MaxLength(500)]
        public string TerminalName { get; set; }

        public virtual ICollection<StoreTerminalHourly> storeTerminalHourly { get; set; }
    }
}