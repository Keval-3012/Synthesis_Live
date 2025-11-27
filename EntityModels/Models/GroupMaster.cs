using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("GroupMaster")]
    public class GroupMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int GroupId { get; set; }

        [Required(ErrorMessage = " ")]
        [RegularExpression(@"^([a-zA-Z ]{3,})*$", ErrorMessage = "Minimum 3 letters,No Number or special character")]
        [MaxLength(30,ErrorMessage = "Group name can be maximum 30 characters long.")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual CustomerMaster CustomerMaster { get; set; }

        public int? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual VendorMaster VendorMaster { get; set; }

        [NotMapped]
        public bool IsInvoice { get; set; }

        [NotMapped]
        public bool IsDocument { get; set; }

        [NotMapped]
        public bool IsTerminal { get; set; }

        [NotMapped]
        public bool IsReport { get; set; }
        public virtual ICollection<StoreMaster> StoreMasters { get; set; }
        public virtual ICollection<UserTypeMaster> UserTypeMasters { get; set; }
        public virtual ICollection<UserMaster> UserMasters { get; set; }
    }
}