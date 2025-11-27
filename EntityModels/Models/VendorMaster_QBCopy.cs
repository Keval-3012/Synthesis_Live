using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EntityModels.Models
{
    public class VendorMaster_QBCopy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int VendorId { get; set; }

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

        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string City { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }

        public int? StoreId { get; set; }

        [MaxLength(50)]
        public string ListId { get; set; }

        public Boolean IsActive { get; set; }

        public Boolean IsNeedUpdate { get; set; }

        [MaxLength(80)]
        public string EMail { get; set; }

    }
}