using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityModels.Models
{
    [Table("InvoiceUserProductMapLog")]
    public class InvoiceUserProductMapLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int InvoiceUserProductMapLogID { get; set; }

        public string ItemNo { get; set; }  

        public string UPCCode { get; set; } 

        public int? InvoiceID { get; set; }  

        public int? ProductID { get; set; }   

        public int? ProductVendorID { get; set; }

        public int? Operation { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set;}

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set;}
    }
    public class InvoiceUserProductMapLogList
    {
        public string ItemNoOrUPCCode { get; set; }

        public string CDateTime { get; set; }

        public string Name { get; set; }

        public string Operation { get; set; }

        public string Description { get; set; }

        public int? AffectedRows { get; set; }

        public string InvoiceNumber { get; set; }

        public int? InvoiceId { get; set; }
    }
    public class ItemProductScanLog
    {
        public string Name { get; set; }

        public string StoreName { get; set; }

        public string ScannedDate { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public string Description { get; set; }

        public int? AffectedRows { get; set; }

        public string InvoiceNumber { get; set; }

        public int? InvoiceId { get; set; }
        public string UserName { get; set; }
        public int ItemCodeLogId { get; set; }
        public string AppUserName { get; set; }
        public string RoleName { get; set; }
        public int? ItemsScannedToday { get; set; }
        public int? ItemsScannedYesterday { get; set; }
        public int? ItemsScannedThisWeek { get; set; }
        public int? ItemsScannedThisMonth { get; set; }
        public int? TotalItemsScanned { get; set; }
        public int? TotalUsers { get; set; }
        public int? ActivatedUsers { get; set; }
        public int? PartiallyUsedUsers { get; set; }
        public int? NotUsedUsers { get; set; }
        public string UserStatus { get; set; }
    }

    public class BarcodeLookupDetails
    {
        public int ProductId { get; set; }
        public string BarcodeNumber { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
    }

    public class CompetitorsUserLog
    {
        public int CompetitorsLogId { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string ItemNoItemName { get; set; }
        public string CompetitorsName { get; set; }
        public DateTime? LogDate { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public string OperationType { get; set; }
        public string SearchKeyword { get; set; }
        public string FilterKeyword { get; set; }
    }

}