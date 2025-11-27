using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("VendorMaster")]
    public class VendorMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int VendorId { get; set; }

        [Required(ErrorMessage =" ")]
        [Display(Name = "Vendor Name")]
        [MaxLength(300)]
        public string VendorName { get; set; }

        [Display(Name = "Company Name")]
        [MaxLength(300)]
        public string CompanyName { get; set; }

        [Display(Name = "Print On Check")]
        [MaxLength(200)]
        public string PrintOnCheck { get; set; }

        [MaxLength(1000)]
        public string Address { get; set; }

        [MaxLength(1000)]
        public string Address2 { get; set; }

        [MaxLength(50)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }
        [MaxLength(60)]
        public string AccountNumber { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public virtual StoreMaster StoreMasters { get; set; }

        [MaxLength(80)]
        public string EMail { get; set; }

        public string Instruction { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [MaxLength(50)]
        public string ListId { get; set; }

        [MaxLength(50)]
        public string DListId { get; set; }

        public Boolean IsActive { get; set; }

        public Boolean IsSync { get; set; }

        public DateTime? SyncDate { get; set; }

        //Added by Dani on 02-04-2025
        public DateTime? LastModifiedOn { get; set; }

        //Added by Dani on 13-11-2025
        public string RoutingNumber { get; set; }

        public string BankAccountNumber { get; set; }

        //Added by Dani on 21-08-2025
        public int? UniversalVendorMasterId { get; set; }
        [ForeignKey("UniversalVendorMasterId")]
        public virtual UniversalVendorMaster UniversalVendorMasters { get; set; }

        [NotMapped]
        public string[] MultiStoreId { get; set; }

        [NotMapped]
        public string[] MultiDepartmentId { get; set; }
        [NotMapped]
        public string Departments { get; set; }
        [NotMapped]
        public string Status { get; set; }
        [NotMapped]
        public string StoreName { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<OtherDeposit> OtherDeposits { get; set; }
        public virtual ICollection<GroupMaster> GroupMasters { get; set; }
        public virtual ICollection<PayrollBankAccount> PayrollBankAccounts { get; set; }
        public virtual ICollection<ConfigurationGroup> ConfigurationGroups { get; set; }

        public virtual ICollection<VendorDepartmentRelationMaster> VendorDepartmentRelationMasters { get; set; }
        //public virtual ICollection<HomeExpenseWeeklySalesSetting> HomeExpenseVendorMasters { get; set; }
    }

    public class VenodrMasterSelect
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public int? StoreId { get; set; }
        public string EMail { get; set; }
        public string Departments { get; set; }
        public string Status { get; set; }
        public string StoreName { get; set; }
        public string AccountNumber { get; set; }
    }
    public class VenodrMasterCopy
    {
        public string VendorName { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }

        public int VendorId { get; set; }
        public string Statuss { get; set; }
        public int status { get; set; }
    }

   
    public class TestVendorMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int VendorId { get; set; }

        [Required(ErrorMessage = " ")]
        [Display(Name = "Vendor Name")]
        [MaxLength(300)]
        public string VendorName { get; set; }

        [Display(Name = "Company Name")]
        [MaxLength(300)]
        public string CompanyName { get; set; }

        [Display(Name = "Print On Check")]
        [MaxLength(200)]
        public string PrintOnCheck { get; set; }

        [MaxLength(1000)]
        public string Address { get; set; }

        [MaxLength(1000)]
        public string Address2 { get; set; }

        [MaxLength(50)]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }
        [MaxLength(60)]
        public string AccountNumber { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }

        public int? StoreId { get; set; }
       

        [MaxLength(80)]
        public string EMail { get; set; }

        public string Instruction { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        [MaxLength(50)]
        public string ListId { get; set; }

        [MaxLength(50)]
        public string DListId { get; set; }

        public Boolean IsActive { get; set; }

        public Boolean IsSync { get; set; }

        public DateTime? SyncDate { get; set; }

        [NotMapped]
        public string[] MultiStoreId { get; set; }

        [NotMapped]
        public string[] MultiDepartmentId { get; set; }
        [NotMapped]
        public string Departments { get; set; }
        [NotMapped]
        public string Status { get; set; }
        [NotMapped]
        public string StoreName { get; set; }

    }

    public class AddMergeVendor
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }

        public string Status { get; set; }
    }

    public class GetMergeVendor
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        
    }

    public class MergeVendorInvoiceList
    {
        public int InvoiceId { get; set; }
        public int InvoiceTypeId { get; set; }

        public int VendorId { get; set; }

        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string UploadInvoice { get; set; }

        public int StoreId { get; set; }
    }
}