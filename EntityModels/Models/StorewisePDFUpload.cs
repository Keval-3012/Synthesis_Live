using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("StorewisePDFUpload")]
    public class StorewisePDFUpload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int StorewisePDFUploadId { get; set; }

        [Required(ErrorMessage = " ")]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }


        public DateTime? CreatedOn { get; set; }
        public int IsSync { get; set; }

        public string FileName { get; set; }
        public bool IsActive { get; set; }

        [MaxLength(1)]
        public string FileType { get; set; }

        public int HSequence { get; set; }
    }
}