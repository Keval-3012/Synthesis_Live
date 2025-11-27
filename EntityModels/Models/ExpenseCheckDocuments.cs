using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("ExpenseCheckDocuments")]
    public class ExpenseCheckDocuments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ExpenseCheckDocumentId { get; set; }

        public int ExpenseCheckId { get; set; }
        [ForeignKey("ExpenseCheckId")]
        public virtual ExpenseCheck ExpenseCheck { get; set; }

        [Required(ErrorMessage = "Required..")]
        [MaxLength(1000)]
        public string DocumentName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }
    }
   
}