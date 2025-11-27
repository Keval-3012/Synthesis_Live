using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModels.Models
{
    [Table("InvoiceAutomation")]

    public class InvoiceAutomation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int InvoiceAutomationId { get; set; }

        public int StoreId { get; set; }

        public int InvoiceTypeId { get; set; }

        public int VendorId { get; set; }

        public DateTime InvoiceDate { get; set; }

        [MaxLength(100)]
        public string InvoiceNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public string UploadInvoice { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? PDFPageCount { get; set; }

        public string VendorName { get; set; }

        public bool FlagDeleted { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public DateTime? InvoiceApprovedDate {  get; set; } 
        [NotMapped]
        [Required(ErrorMessage = " ")]
        public string strInvoiceDate { get; set; }
        [NotMapped]
        public string CreatedByUserName { get; set; }
        [NotMapped]
        public string StoreName { get; set; }
        [NotMapped]
        [Required(ErrorMessage = " ")]
        public int PaymentTypeId { get; set; }
        [NotMapped]
        public string PhoneNumber { get; set; }
        [NotMapped]
        public string Address { get; set; }
        [NotMapped]
        public string Address2 { get; set; }
        [NotMapped]
        public string City { get; set; }
        [NotMapped]
        public string State { get; set; }
        [NotMapped]
        public string Country { get; set; }
        [NotMapped]
        public string PostalCode { get; set; }
        [NotMapped]
        public List<DepartmentMaster> DepartmentMasters { get; set; }
        [NotMapped]
        public int? ChildDepartmentId { get; set; }
        [NotMapped]
        public decimal? ChildAmount { get; set; }
        [NotMapped]
        [Required(ErrorMessage = " ")]
        public int Disc_Dept_id { get; set; }
        [NotMapped]
        [MaxLength(2000)]
        public string Note { get; set; }
        [NotMapped]
        public int DepartmentId { get; set; }
        [NotMapped]
        public decimal? DiscountPercent { get; set; }
        [NotMapped]
        public int CheckDup { get; set; }
        [NotMapped]
        public string btnName { get; set; }
        [NotMapped]
        public decimal? DiscountAmount { get; set; }
        [NotMapped]
        public bool QBTransfer { get; set; } = false;
        [NotMapped]
        public string QBtransferss { get; set; }
        [NotMapped]
        public int? DiscountTypeId { get; set; }
        [NotMapped]
        public string QuickInvoice { get; set; }
        [NotMapped]
        public int? UploadPdfId { get; set; }
        [NotMapped]
        public int? UploadPdfAutomationId { get; set; }
        [NotMapped]
        public InvoiceStatusEnm? StatusValue { get; set; }
        [NotMapped]
        public int? ApproveRejectBy { get; set; }
        [NotMapped]
        public DateTime? ApproveRejectDate { get; set; }
        [NotMapped]
        public int? IsSync { get; set; } = 0;
        [NotMapped]
        public Boolean IsActive { get; set; }
        [NotMapped]
        public string Source { get; set; }
        [NotMapped]
        public string InvoiceReview { get; set; }
    }
}
