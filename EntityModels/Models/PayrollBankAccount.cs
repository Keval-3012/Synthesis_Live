using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("PayrollBankAccount")]
    public class PayrollBankAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PayrollBankAccountId { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? BankAccountId { get; set; }
        [ForeignKey("BankAccountId")]
        public virtual DepartmentMaster DepartmentMasters { get; set; }

        public int? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual VendorMaster VendorMasters { get; set; }
    }
}