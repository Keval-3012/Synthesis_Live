using EntityModels.Models;
using SynthesisViewModal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface ILineItemRepository
    {
        List<VendorNameList> VendorListByStoreId(int? StoreIds);
        List<VendorNameList> VendorList();
        List<LineItemInvoiceNew> GetLineItemInvoiceDetails(int? StoreIds, string vendorName, DateTime? easternTime, string checkvalue);
        void Update_UPC(string UPCCode);
        List<LineItemList> GetLineItemInvoiceitemDetail(object Val);
        InvoiceProduct GetInvoiceProductById(int Id);
        List<ProductsList> GetRecommendedProductList(InvoiceProduct invoiceproduct, int Id);
        List<productVendorAllList> GetRecommendedProductVendorList(InvoiceProduct productVendorAllList, int Id, string vendorName);
        ProductStoreAndVendorName GetPopupHeaderData(int? value);
        List<ProductsList> GetRecommendedProductListWithVendorName(InvoiceProduct invoiceproduct, int Id, string vendorName);
        List<ProductsList> SelectByFilterProduct(string search, int Id);
        List<productVendorAllList> SelectByFilterProductVendor(string search2, int Id);
        List<ProductsList> SelectByFilter(string search, string Id);
        void AddVendorLineItem(LineItemList value);
        void AssignProduct(ProductsList products);
        Invoice GetInvoiceById(long Id);
        List<ProductsList> GetProductMasterData(LineItemList value);
        void RemoveProductmapping(InvoiceProduct invoiceproduct);
        List<StoreMaster> GetStoreId();
        int? InvoiceProducts(string UPC);
        Products GetProductsById(int? Id);
        void Unlink_ItemLines(LineItemList value);
        void Unlink_ItemLines_Lookup(string ItemName);
        ProductPriceModel GetMaxAndAveragePrice(int? Id);
        StoreCount GetMAXStoreName(int? Id);
        List<LineItemViewModel> GetInvoiceQty(int? id);
        List<LineItemsLookUp> LineItemLookUp(int ProductId, int StoreIds, string InvoiceNumber);
        List<VendoreLookUp> VendorLookupList(int ProductId);
        List<ProductPriceModel> Getlinechart(int ProductId, string startDate, string endDate, string storeId);
        void UploadImage(byte[] bytes, int ProductId);
        void DeleteImage(int ProductId);
        List<StorePieChartModel> StoregetPiechart(int ProductId);
        Products GetOrderByDescending();
        int LineItemInvoiceCount(string itemname);
    }
}
