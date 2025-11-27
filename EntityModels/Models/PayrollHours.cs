using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("PayrollHours")]
    public class PayrollHours
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int PayrollHoursId { get; set; }

        [Required(ErrorMessage = "*")]
        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [Required(ErrorMessage = "Required..")]
        public string FileName { get; set; }
        
        public DateTime FileDate { get; set; }

        public virtual ICollection<PayrollHoursDetails> payrollHoursDetails { get; set; }
    }
}