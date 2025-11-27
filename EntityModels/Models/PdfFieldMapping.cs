using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
   
    [Table("PdfFieldMapping")]
    public class PdfFieldMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PdfFieldMappingId { get; set; }
       
        public string PDFName { get; set; }
        public string CommonName { get; set; }
        public string VendorIds { get; set; }
    }
  
    
}