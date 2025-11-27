using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EntityModels.Models
{
    public class ProductVendor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ProductVendorId { get; set; }

        [Display(Name = "Item No")]
        [MaxLength(500)]
        public string ItemNo { get; set; }

        [Display(Name = "UPC Code")]
        [MaxLength(300)]
        public string UPCCode { get; set; }

     
        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(3000)]
        public string Vendors { get; set; }


        [MaxLength(500)]
        public string Brand { get; set; }

        [MaxLength(300)]
        public string Size { get; set; }

        public string Price { get; set; }
        [Display(Name = "Created By")]
        public int? CreatedBy { get; set; }
        [Display(Name = "Date Created")]
        public DateTime? DateCreated { get; set; }
        [NotMapped]
        public string CreatedBys { get; set; }
        [NotMapped]
        public string DateCreateds { get; set; }
        public bool? Flag { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products products { get; set; }

        public ICollection<InvoiceProduct> invoiceProducts { get; set; }

        public int? IsManually { get; set; }
    }
    public class productVendorAllList
    {
        
        public int ProductVendorId { get; set; }
        
        public string ItemNo { get; set; }
        public string UPCCode { get; set; }

      
        public string Description { get; set; }
        public string Vendors { get; set; }

        public string Brand { get; set; }
        public string Size { get; set; }
        public string Price { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBys { get; set; }
        public string DateCreateds { get; set; }
        public int? ProductId { get; set; }
    }
    public class ProductVendorList
    {

        public string ItemNo { get; set; }

        public string Brand { get; set; }

        public string Description { get; set; }

        public string Size { get; set; }

        public string UPCCode { get; set; }

        public decimal Price { get; set; }
        public string Vendors { get; set; }
        public int? ProductVendorId { get; set; }
        public int? InvoiceProductId { get; set; }
    }
    public class ProductStoreAndVendorName
    {
        public string NickName { get; set; }
        public string VendorName { get; set; }
    }
}