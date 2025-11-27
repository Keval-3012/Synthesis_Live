using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("TerminalDailyMaster")]
    public class TerminalDailyMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int TerminalDailyId { get; set; }

        [MaxLength(500)]
        public string TerminalName { get; set; }

        public virtual ICollection<StoreTerminalDaily> storeTerminalDaily { get; set; }
    }
}