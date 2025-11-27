using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("UploadPDFAutomation")]
    public class UploadPDFAutomation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UploadPdfAutomationId { get; set; }

        [MaxLength(150)]
        public string FileName { get; set; }

        public int IsProcess { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int PageCount { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }
        public int? Synthesis_Live_InvID { get; set; }

        public bool IsDeleted { get; set; }
        public bool Is_Processing_Enabled { get; set;}
        public string UploadedStatus { get; set;}
        public string QueueId { get; set;}   
    }

    public class UploadPDFAutomationList
    {
        public int UploadPdfAutomationId { get; set; }
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string StoreName { get; set; }
        public int IsProcess { get; set; }
        public int? Synthesis_Live_InvID { get; set; }
        public string UserName { get; set; }
        public bool Is_Processing_Enabled { get; set;}
        public string UploadedStatus { get; set; }
    }
}