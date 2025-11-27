using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("UploadPdf")]
    public class UploadPdf
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UploadPdfId { get; set; }

        [MaxLength(150)]
        public string FileName { get; set; }

        public int IsProcess { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int PageCount { get; set; }

        public int ReadyForProcess { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }
        public int? Synthesis_Live_InvID { get; set; }
        public int? ReferenceId { get; set; }

        [Column(TypeName = "ntext")]
        [MaxLength]
        public string Errors { get; set; }

        [NotMapped]
        public string StoreName { get; set; }

        [NotMapped]
        public string CreatedName { get; set; }

        [NotMapped]
        public string CreatedDates { get; set; }
    }

    public class UploadFile {
        public int UploadPdfId { get; set; }
        public string FileName { get; set; }
        public int IsProcess { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int PageCount { get; set; }
        public int ReadyForProcess { get; set; }
        public int? StoreId { get; set; }
        public string Errors { get; set; }
        public string StoreName { get; set; }
        public string CreatedName { get; set; }
        public string CreatedDates { get; set; }
        public int? ReferenceId { get; set; }
    }
}