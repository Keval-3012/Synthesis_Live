using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("TerminalMaster")]
    public class TerminalMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int TerminalId { get; set; }

        [MaxLength(500)]
        public string TerminalName { get; set; }

        public virtual ICollection<StoreTerminal> StoreTerminals { get; set; }
    }
}