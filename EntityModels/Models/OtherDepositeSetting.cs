using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("OtherDepositeSetting")]
    public class OtherDepositeSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int OtherDepositeSettingId { get; set; }

        [Required(ErrorMessage = " ")]
        [MaxLength(200)]
        public string Name { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int? BankAccountId { get; set; }
        [ForeignKey("BankAccountId")]
        public virtual DepartmentMaster DepartmentMaster { get; set; }

        [NotMapped]
        public string BankAccountName { get; set; }
    }
}