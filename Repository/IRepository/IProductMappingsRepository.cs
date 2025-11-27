using EntityModels.Models;
using Syncfusion.EJ2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IProductMappingsRepository
    {
        Products GetProduct();
        Task<int> GetCountProduct(Products products);
        void ProductAdd(Products products);
        void ProductUpdate(Products products);
        void AddVendor(string vendor);
        List<Products> ProductList();
        List<Products> getProductListByID(int ID);
        List<Products> getSearchdata(string SearchVal);
        void UpdateProductInvoice(string selectedval);
        void ModifyProductMapping(string mappedProductId);
        List<Products> GetProductMasterData(int ID);
        void updateKeyWord(int ProductID, string KeyWord);
        List<ItemLibraryDepartment> GetItemLibraryDepartment();
        List<ProductsList> GetDataProductForUrl();
        void InsertProduct(CRUDModel<Products> products);
        void UpdateProduct(CRUDModel<Products> products);
        void RemoveProduct(CRUDModel<Products> products);
        Products getSynthesisId();
        List<StoreMaster> GetStoreId();
        ProductPriceModel ListProductPrice(string SynthesisId);
        StoreCount ListStoreCount(string SynthesisId);
        List<Invoice> GetInvoices();
        List<InvoiceProduct> GetInvoiceProduct();
        List<Products> GetProductList();
        void CheckProductUpdate();





    }
}
