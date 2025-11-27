using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    public class DocumentCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DocumentCategoryId { get; set; }

        [Required(ErrorMessage = " ")]
        [MaxLength(30)]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? CreatedOn { get; set; }
        [NotMapped]
        public int DocumentCount { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}