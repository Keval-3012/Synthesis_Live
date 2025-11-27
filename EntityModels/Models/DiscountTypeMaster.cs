using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("DiscountTypeMaster")]
    public class DiscountTypeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int DiscountTypeId { get; set; }

        [Required(ErrorMessage = "Required..")]
        [MaxLength(200)]
        public string DiscountType { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}