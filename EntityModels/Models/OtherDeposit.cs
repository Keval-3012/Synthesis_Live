using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("OtherDeposit")]
    public class OtherDeposit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int OtherDepositId { get; set; }

        [Required(ErrorMessage = " ")]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public int? PaymentMethodId { get; set; }
        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethodMaster PaymentMethodMasters { get; set; }

        public decimal Amount { get; set; }

        public int? OptionId { get; set; }
        [ForeignKey("OptionId")]
        public virtual OptionMaster OptionMasters { get; set; }

        public int? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual VendorMaster VendorMasters { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual DepartmentMaster DepartmentMaster { get; set; }

        public int? StoreTerminalId { get; set; }
        [ForeignKey("StoreTerminalId")]
        public virtual StoreTerminal StoreTerminals { get; set; }

        //public string Other { get; set; }

        public int? ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public virtual ShiftMaster ShiftMasters { get; set; }

        [MaxLength(200)]
        public string UploadDocument { get; set; }

        public int? SalesActivitySummaryId { get; set; }
        [ForeignKey("SalesActivitySummaryId")]
        public virtual SalesActivitySummary SalesActivitySummaries { get; set; }

        public DateTime? CreateDate { get; set; }

        public virtual ICollection<SalesOtherDeposite> SalesOtherDeposites { get; set; }
    }
}