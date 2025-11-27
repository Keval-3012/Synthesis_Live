using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("PaymentMethodMaster")]
    public class PaymentMethodMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PaymentMethodId { get; set; }

        [MaxLength(200)]
        public string PaymentMethod { get; set; }

        public virtual ICollection<OtherDeposit> OtherDeposits { get; set; }
    }
}