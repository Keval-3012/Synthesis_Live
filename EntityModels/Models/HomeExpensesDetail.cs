using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("HomeSome_Expenses")]
    public class HomeExpensesDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ID { get; set; }
        public int? StoreID { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }
        public decimal? PaymentFees { get; set; }
        public decimal? DeliveryCost { get; set; }
        public decimal? TipsPaid { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? PaymentListID { get; set; }
        public int? DeliveryListID { get; set; }
        public int? TipListID { get; set; }
        public DateTime? ExpenseDate { get; set; }

        public int? Status { get; set; }

        [NotMapped]
        public string PaymentFeesStatus { get; set; }

        [NotMapped]
        public string DeliveryCostStatus { get; set; }

        [NotMapped]
        public string TipsPaidStatus { get; set; }
    }


    public class HomeExpensesDetailList
    {
        //public int ID { get; set; }
        //public int? StoreID { get; set; }
        public int? ID { get; set; }
        public decimal? PaymentFees { get; set; }
        public decimal? DeliveryCost { get; set; }
        public decimal? TipsPaid { get; set; }
        public int? PaymentListID { get; set; }
        public int? DeliveryListID { get; set; }
        public int? TipListID { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public string PaymentFeesStatus { get; set; }
        public string DeliveryCostStatus { get; set; }
        public string TipsPaidStatus { get; set; }

        public int? Status { get; set; }

        public string NickName { get; set; }
    }
}