using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("EmployeeMaster")]
    public class EmployeeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int EmployeeId { get; set; }


        [MaxLength(250)]
        public string FirstName { get; set; }

        [MaxLength(250)]
        public string LastName { get; set; }

        [MaxLength(750)]
        public string DisplayName { get; set; }

        [MaxLength(500)]
        public string Email { get; set; }
        [Required(ErrorMessage = " ")]

        public int StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        public int ListId { get; set; }

        public bool IsActive { get; set; }
    }
}