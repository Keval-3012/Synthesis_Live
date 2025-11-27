using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("CustomersReceiveablesManagement")]

    public class CustomersReceiveablesManagement  
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int CompanyNameId { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        [MaxLength(100)]
        public string CompanyName{ get; set; }

        [MaxLength(50)]
        public string ContactPersonName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Invalid Email Address")]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [MaxLength(50)]        
        public string PhoneNumber { get; set; }

        [MaxLength(1000)]
        public string Address { get; set;}

        public bool IsDeleted { get; set; } = false;

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }
    }

    [Table("CustomersReceiveablesReceipts")]
    public class CustomersReceiveablesReceipts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int CustomersReceiveablesReceiptsId { get; set; }

        public int? CompanyNameId { get; set; }

        [MaxLength(250)]
        public string FileName { get; set; }

        public bool IsEmailTriggered { get; set; } = false;

        public DateTime? IsEmailSentDate { get; set; }

        public int StoreId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime DeletedDateTime { get; set; }
    }

    public class CustomersReceiveablesReceiptsListModel
    {
        public int CustomersReceiveablesReceiptsId { get; set; }
        public string CompanyName { get; set; }
        public string FileName { get; set; }
        public string StoreName { get; set; }
        public bool IsEmailTriggered { get; set; }
        public DateTime? IsEmailSentDate { get; set; }
    }
}
