using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EntityModels.Models
{
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Editable(false)]
        public int ProductId { get; set; }

        [Display(Name = "UPC Code")]
        [MaxLength(300)]
        public string UPCCode { get; set; }

        [Display(Name = "Item No")]
        [MaxLength(500)]
        public string ItemNo { get; set; }
                
        public string GenericID { get; set; }

        [Display(Name = "Synthesis Id")]
        [MaxLength(100)]
        public string SynthesisId { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(3000)]
        public string Vendor { get; set; }

        [MaxLength(1000)]
        public string Departments { get; set; }
        public ICollection<InvoiceProduct> invoiceProducts { get; set; }

        [MaxLength(500)]
        public string Brand { get; set; }

        public DateTime? DateCreated { get; set; }
        public string ProductImage { get; set; }

        [MaxLength(300)]
        public string Size { get; set; }

        public string KeyWord { get; set; }

        public bool Flag { get; set; }

        [NotMapped]
        public string FlagValue { get; set; }

        [NotMapped]
        public string[] MultiVendorId { get; set; }

        [NotMapped]
        public string[] MultiDepartmentId { get; set; }

        [NotMapped]
        public int ProductVendorId { get; set; }

        public int? CreatedBy { get; set; }

        public ICollection<ProductVendor> productVendors { get; set; }
        public int? DepartmentNumber { get; set; }
        public string Ecrs_Item_Id { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }

    public class ProductsList
    {

        public int ProductId { get; set; }


        public string UPCCode { get; set; }


        public string ItemNo { get; set; }
        public string GenericID { get; set; }

        public string SynthesisId { get; set; }

        public string Description { get; set; }

        public string Vendor { get; set; }

        public string Departments { get; set; }

        public string VendorsStr { get; set; }
        public string DepartmentsStr { get; set; }
        public string KeyWord { get; set; }
        public string Brand { get; set; }
        public string Size { get; set; }
        public int? InvoiceProductId { get; set; }
        public int? ProductVendorId { get; set; }
        public int? DepartmentNumber { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DateCreated { get; set; }
    }
    public class VendorNameList
    {
       public string VendorName { get; set; }
       public int StoreId { get; set; }
    }
    public class ProductPriceUpc
    {
        public string UPC { get; set; }
        public string Descritpion { get; set; }
    }
    public class ProductPriceModel
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public string UPCCode { get; set; }
        public string Date { get; set; }
        public string Qty { get; set; }
        public string FPStoreName { get; set; }
        public string QPStoreName { get; set; }
        public string ItemId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string Brand { get; set; }
        public string Department { get; set; }
        public string Notes { get; set; }
        public int ProductId { get; set; }
        public string DateCreated { get; set; }
        public string ProductImage { get; set; }
    }
    public class StoreCount {
        public string Name { get; set; }
        public int Count { get; set; }
    }
    public class QtyMax
    {
        public string StoreName { get; set; }
        public string Qty { get; set; }
    }
    public class LineItemsLookUp
    {
        public string StoreName { get; set; }
        public string ItemName { get; set; }
        public string Date { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string VendorName { get; set; }
        public string Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public int StoreId { get; set; }
        public int InvoiceId { get; set; }
        public int InvoiceProductId { get; set; }
    }

    public class LineItemInvoice
    {
        public string NickName { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string VendorName { get; set; }
        public int StoreId { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }

        public int ApproveStatus { get; set; }

      
    }

    public class LineItemInvoiceNew
    {
        public string NickName { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string VendorName { get; set; }
        public int StoreId { get; set; }
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }

        public int ApproveStatus { get; set; }

        public int ApproveShow { get; set; }
    }


    public class LineItemList
    {
        public string Description { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public int? InvoiceId { get; set; }
        public int? InvoiceProductId { get; set; }
        public int? ProductId { get; set; }
        public decimal? Description_Accuracy { get; set; }
        public decimal? Qty_Accuracy { get; set; }
        public decimal? UnitPrice_Accuracy { get; set; }
        public decimal? Total_Accuracy { get; set; }
        public bool Approved { get; set; }
        public string UPCCode { get; set; }
        public string ItemNo { get; set; }
        public string Approvevalue { get; set; }
        

    }
    public class VendoreLookUp
    {
        public string VendorName { get; set; }
    }

    public class InvoiceLookup
    {
        public string StoreName { get; set; }
        public string PaymentType { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }

    }
    public class StoreWiseLookup
    {
        public string StoreName { get; set; }
    }

    public partial class SPProductLineChart_Result
    {
        public int Id { get; set; }
        public Nullable<int> InvoiceID { get; set; }
        public string UPCCode { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string Qty { get; set; }
        public decimal Qtys { get; set; }
        public Nullable<decimal> Unitprice { get; set; }
        public string Total { get; set; }
        public string ScannedTotal { get; set; }
        public Nullable<System.DateTime> Invoice_Date { get; set; }
        public Nullable<int> Store_id { get; set; }
        public DateTime? InvoiceDate { get; set; }
    }
    public class VendorPiChartModel
    {
        public int VenQty { get; set; }
        public string VenName { get; set; }

    }

    public class StorePieChartModel
    {
        public int StoreQty { get; set; }
        public string StoreName { get; set; }
    }

    //public class ProductPriceData
    //{
    //    public int UPC { get; set; }
    //    public string Descritpion { get; set; }
    //    public string [74-84] { get; set; }
    //    public string [Maywood] { get; set; }
    //    public string 1407 { get; set; }
    //    public string 180 { get; set; }
    //    public string 2589 { get; set; }
    //    public string 2840 { get; set; }
    //    public string 170 { get; set; }

    //}
}