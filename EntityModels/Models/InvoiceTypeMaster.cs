using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("InvoiceTypeMaster")]
    public class InvoiceTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int InvoiceTypeId { get; set; }

        [Required(ErrorMessage = "Required..")]
        [MaxLength(200)]
        public string InvoiceType { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }

    public class iVType
    {
        public int InvoiceTypeId { get; set; }
        public string InvoiceType { get; set; }
    }
}