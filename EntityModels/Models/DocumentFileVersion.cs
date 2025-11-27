using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class DocumentFileVersion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DocumentFileVersionId { get; set; }

        public int DocumentId { get; set; }

        [MaxLength(100)]
        public string AttachFile { get; set; }

        public int Version { get; set; }

        public DateTime? CreatedOn { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; }
    }
}