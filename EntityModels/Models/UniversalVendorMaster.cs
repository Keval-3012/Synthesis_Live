using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("UniversalVendorMaster")]
    public class UniversalVendorMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int UniversalVendorMasterId { get; set; }

        [MaxLength(500)]
        public string VendorName { get; set; }

        [MaxLength(500)]
        public string DisplayName { get; set; }

        [MaxLength(500)]
        public string PrintCheckName { get; set; }

        [MaxLength(1000)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        public string VendorProfileImage { get; set; }

        public int? VendorProfileProgress { get; set; }

        [MaxLength(80)]
        public string Email { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }
        //Added by Dani on 11-11-2025
        public bool? NeedsManualReview { get; set; }

        public string ManualReviewReason { get; set; }

        public int? LastUpdatedBy { get; set; }

        public int? UpdateCount { get; set; }

        public DateTime? LastUpdatedOn { get; set; }

        //Added by Dani on 13-11-2025
        public string RoutingNumber { get; set; }

        public string BankAccountNumber { get; set; }

        [NotMapped]
        public string DepartmentNames { get; set; }

        public virtual ICollection<VendorMaster> VendorMasters { get; set; }
    }

    public class UniversalVendorMasterModel
    {
        public int UniversalVendorMasterId { get; set; }
        public string VendorName { get; set; }
        public string DisplayName { get; set; }
        public string PrintCheckName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string VendorProfileImage { get; set; }
        public int? VendorProfileProgress { get; set; }
        public string DepartmentNames { get; set; }
    }

    public class UniversalModelList
    {
        public int UniversalVendorMasterId { get; set; }
        public int VendorId { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string AccountNo { get; set; }
        public string Instruction { get; set; }
        public bool IsSync { get; set; }
    }

    public class VendorStoreList
    {
        public int StoreId { get; set; }
        public string NickName { get; set; }
    }

    public class VendorDepartmentList
    {
        public string DepartmentName { get; set; }
        public string Value { get; set; }
    }

    public class StoreDepartmentViewModel
    {
        public List<VendorStoreList> Stores { get; set; }
        public List<VendorDepartmentList> Departments { get; set; }
    }
}