using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("HomeExpenseWeeklySalesSetting")]
    public class HomeExpenseWeeklySalesSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int HomeExpenseWeeklySalesSettingId { get; set; }
        public int? DeliveryCostId { get; set; }
        [ForeignKey("DeliveryCostId")]
        public virtual DepartmentMaster DepartmentMasterDC { get; set; }
        public int? PaymentFeesId { get; set; }
        [ForeignKey("PaymentFeesId")]
        public virtual DepartmentMaster DepartmentMasterPF { get; set; }
        public int? TipsPaidId { get; set; }
        [ForeignKey("TipsPaidId")]
        public virtual DepartmentMaster DepartmentMasterTP { get; set; }
        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }
        public int? BankAccountId { get; set; }
        [ForeignKey("BankAccountId")]
        public virtual DepartmentMaster DepartmentMaster1 { get; set; }
        public int? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual VendorMaster VendorMaster { get; set; }
       
        [NotMapped]
        public string DeptName { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }
}