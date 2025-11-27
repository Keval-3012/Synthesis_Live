using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    [Table("CustomerMaster")]
    public class CustomerMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int CustomerId { get; set; }

        [MaxLength(200)]
        public string DisplayName { get; set; }

        [MaxLength(200)]
        public string LastName { get; set; }

        [MaxLength(200)]
        public string FirstName { get; set; }

        [MaxLength(500)]
        public string PrimaryEmailAddr { get; set; }

        [MaxLength(50)]
        public string PrimaryPhone { get; set; }

        public bool Active { get; set; }

        [MaxLength(200)]
        public string PrintOnCheckName { get; set; }

        [MaxLength(200)]
        public string MiddleName { get; set; }

        [MaxLength(200)]
        public string CompanyName { get; set; }

        [MaxLength(200)]
        public string Notes { get; set; }

        [MaxLength(100)]
        public string Balance { get; set; }

        [MaxLength(200)]
        public string BAddress1 { get; set; }

        [MaxLength(200)]
        public string BAddress2 { get; set; }

        [MaxLength(50)]
        public string BCity { get; set; }

        [MaxLength(50)]
        public string BState { get; set; }

        [MaxLength(50)]
        public string BCountry { get; set; }

        [MaxLength(50)]
        public string BZipCode { get; set; }

        public int StoreId { get; set; }

        public DateTime? CreatedOn { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; }

        [MaxLength(50)]
        public string ListID { get; set; }

        public virtual ICollection<GroupMaster> GroupMasters { get; set; }
        public virtual ICollection<ConfigurationGroup> ConfigurationGroups { get; set; }
        public virtual ICollection<ExpenseCheckDetail> ExpenseCheckDetails { get; set; }
    }
}