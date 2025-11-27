using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("StateMaster")]
    public class StateMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int StateId { get; set; }

        [Required(ErrorMessage = "Required")]
        [MaxLength(100)]
        public string StateName { get; set; }

        [MaxLength(100)]
        public string StateCode { get; set; }

        public virtual ICollection<StoreMaster> StoreMasters { get; set; }
    }
}