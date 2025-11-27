using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("QBFinanicalClosingYear")]
    public class QBFinanicalClosingYear
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int QBFinanicalClosingYearId { get; set; }
        public string ClosingYear  { get; set; }
        public int? Createdby { get; set; }
        public DateTime? CreatedDate { get; set; }        
        public int? Updatedby { get; set; }
        public DateTime? UpdatedDate { get; set; }        
    }
}
