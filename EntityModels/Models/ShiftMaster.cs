using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("ShiftMaster")]
    public class ShiftMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ShiftId { get; set; }

        [Required(ErrorMessage ="Required...")]
        [MaxLength(100)]
        public string ShiftName { get; set; }

        public virtual ICollection<SalesActivitySummary> SalesActivitySummaries { get; set; }
        public virtual ICollection<CreditcardDetails> creditcardDetails { get; set; }
        public virtual ICollection<CashPaidoutInvoice> CashPaidoutInvoices { get; set; }
        public virtual ICollection<OtherDeposit> OtherDeposits { get; set; }
    }
}