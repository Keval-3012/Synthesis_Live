using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    public class UserWiseStickyNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UserWiseStickyNoteId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserMaster usermasters { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}