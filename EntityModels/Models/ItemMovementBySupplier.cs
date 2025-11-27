using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("ItemMovementBySupplier")]
    public class ItemMovementBySupplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ItemMovementId { get; set; }
        [MaxLength(150)]
        public string SupplierName { get; set; }
        [MaxLength(150)]
        public string Department { get; set; }
        [MaxLength(150)]
        public string ItemCode { get; set; }
        [MaxLength(150)]
        public string ItemName { get; set; }
       
        public decimal QtySold { get; set; }
        
        public decimal LastCOst { get; set; }
       
        public decimal QtyOnHand { get; set; }
      
        public decimal BasePrice { get; set; }
        [MaxLength(150)]
        public string ProjMargin { get; set; }
        public int? ItemMovementHistoryID { get; set; }
    }

    public class ItemMovementBySupplierSelect
    {
        public int ItemMovementId { get; set; }
       
        public string SupplierName { get; set; }
      
        public string Department { get; set; }
       
        public string ItemCode { get; set; }
       
        public string ItemName { get; set; }

        public decimal QtySold { get; set; }

        public decimal LastCOst { get; set; }

        public decimal QtyOnHand { get; set; }

        public decimal BasePrice { get; set; }
       
        public string ProjMargin { get; set; }
    }
}