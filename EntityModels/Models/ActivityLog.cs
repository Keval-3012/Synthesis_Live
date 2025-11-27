using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("ActivityLog")]
    public class ActivityLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ActivityLogId { get; set; }

        public int UserId { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }

        public int Action { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }
    }
}