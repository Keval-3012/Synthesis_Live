using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("InvoicePaymentStatusDetails")]
    public class InvoicePaymentStatusDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int InvoicePaymentId { get; set; }

        public int InvoiceId { get; set; }

        public int BillPaymentTxnId { get; set; }

        public string QbPaymentMethod { get; set; }

        public string QbAccountPaidName { get; set; }

        public decimal? QbCheckAmount { get; set; }

        public string QbCheckNo { get; set; }

        public DateTime? QbPaymentDate { get; set; }

        public decimal? QbTotalAmount { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class InvoicePaymentStatusDetailsList
    {
        public string QbPaymentMethod { get; set; }

        public string QbAccountPaidName { get; set; }

        public decimal? QbCheckAmount { get; set; }

        public string QbCheckNo { get; set; }

        public DateTime? QbPaymentDate { get; set; }

        public decimal? QbTotalAmount { get; set; }
        public decimal? InvoiceAmount { get; set; }
    }
}
