using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("PaymentTypeMaster")]
    public class PaymentTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PaymentTypeId { get; set; }

        [Required(ErrorMessage ="Required...")]
        [MaxLength(100)]
        public string PaymentType { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}