using EntityModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.IRepository
{
    public interface IInvoicePreviewRepository
    {
        List<ProductVendor> GetproductVendorsList();
        ProductVendor GetproductVendorsListByID(int ID);
        List<StoreMaster> GetStoreMasters();
        List<VendorMaster> GetPriceFinderVendor();
        List<Products> GetProductsList(ProductVendor ProductVendor); 
        List<ProductsList> GetProductsLists(ProductVendor InvoiceProduct);
        List<ProductVendor> VendorProductPreviewData(string vendorname);
        List<ProductsList> UrlDatasourceSearchListGet(string search);
        List<StoreNickName> SelectProductListbyvendorname(string vendorname);
        List<productVendorAllList> productVendorAllList(string vendorname);
        List<ProductVendor> SelectInvoiceMatchProducts(ProductVendor InvoiceProduct);
        void UpdateProductInvoice(Products products, int ProductVendorId);
        List<Products> GetproductsList(int? ProductId);
        void UpdateProductVendors(int ProductVendorId);
        string GetInvoice_StoreList(int UserID);
        List<Products> GetproductsList();
        void AddproductsList(Products products);
        void UpdateProductInvoice1(int ProductId, int ProductVendorId);
        void DeleteVendorItem(ProductVendor ProductVendor);
    }
}
