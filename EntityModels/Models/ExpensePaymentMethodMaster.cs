using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("ExpensePaymentMethodMaster")]
    public class ExpensePaymentMethodMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PaymentMethodId { get; set; }

        [MaxLength(200)]
        public string PaymentMethod { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [MaxLength(50)]
        public string ListId { get; set; }

        public Boolean IsActive { get; set; }
    }
}
