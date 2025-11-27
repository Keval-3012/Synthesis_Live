using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Repository
{
    public class InvoicePreviewRepository : IInvoicePreviewRepository
    {
        private DBContext db;
        Logger logger = LogManager.GetCurrentClassLogger();

        public InvoicePreviewRepository(DBContext context) { db = context; }

        public List<ProductVendor> GetproductVendorsList()
        {
            List<ProductVendor> products = new List<ProductVendor>();
            try
            {
                products = db.productVendors.OrderBy(b => b.Vendors).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - GetproductVendorsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return products;
        }
        public ProductVendor GetproductVendorsListByID(int ID)
        {
            ProductVendor products = new ProductVendor();
            try
            {
                products = db.productVendors.Find(Convert.ToInt32(ID));
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - GetproductVendorsListByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return products;
        }
        public List<StoreMaster> GetStoreMasters()
        {
            List<StoreMaster> storemaster = new List<StoreMaster>();
            try
            {
                storemaster = db.StoreMasters.Where(s => s.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - GetStoreMasters - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storemaster;
        }
        public List<VendorMaster> GetPriceFinderVendor()
        {
            List<VendorMaster> vendormaster = new List<VendorMaster>();
            try
            {
                vendormaster = db.Database.SqlQuery<VendorMaster>("SP_VendorMaster @Mode={0}", "GetPriceFinderVendor").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - GetPriceFinderVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendormaster;
        }
        public List<Products> GetProductsList(ProductVendor InvoiceProduct)
        {
            List<Products> Products = new List<Products>();
            try
            {
                Products = db.Database.SqlQuery<Products>("[SP_ProductMaster] @Mode={0},@UPC={1},@ItemNo={2},@Description={3}", "SelectByFilter", InvoiceProduct.UPCCode != null ? InvoiceProduct.UPCCode.Trim() : InvoiceProduct.UPCCode, InvoiceProduct.ItemNo != null ? InvoiceProduct.ItemNo.Trim() : InvoiceProduct.ItemNo, InvoiceProduct.Description != null ? InvoiceProduct.Description.Trim() : InvoiceProduct.Description).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - GetProductsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Products;
        }
        public List<ProductsList> GetProductsLists(ProductVendor InvoiceProduct)
        {
            List<ProductsList> Products = new List<ProductsList>();
            try
            {
                Products = db.Database.SqlQuery<ProductsList>("[SP_ProductMaster] @Mode={0},@UPC={1},@ItemNo={2},@Description={3}", "SelectByFilter", InvoiceProduct.UPCCode != null ? InvoiceProduct.UPCCode.Trim() : InvoiceProduct.UPCCode, InvoiceProduct.ItemNo != null ? InvoiceProduct.ItemNo.Trim() : InvoiceProduct.ItemNo, InvoiceProduct.Description != null ? InvoiceProduct.Description.Trim() : InvoiceProduct.Description).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - GetProductsLists - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Products;
        }
        public List<ProductVendor> VendorProductPreviewData(string vendorname)
        {
            List<ProductVendor> Products = new List<ProductVendor>();
            try
            {
                Products = db.Database.SqlQuery<ProductVendor>("[SP_VendorProductPreviewData] @searchbox={0},@SearchType={1},@StoreID={2},@VendorID={3},@Vendor={4}", "", "", null, null, vendorname.Trim()).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - VendorProductPreviewData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Products;
        }
        public List<productVendorAllList> productVendorAllList(string vendorname)
        {
            List<productVendorAllList> Products = new List<productVendorAllList>();
            try
            {
                Products = db.Database.SqlQuery<productVendorAllList>("[SP_VendorProductPreviewData] @searchbox={0},@SearchType={1},@StoreID={2},@VendorID={3},@Vendor={4}", "", "", null, null, vendorname.Trim()).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - productVendorAllList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Products;
        }
        public List<ProductVendor> SelectInvoiceMatchProducts(ProductVendor InvoiceProduct)
        {
            List<ProductVendor> Products = new List<ProductVendor>();
            try
            {
                Products = db.Database.SqlQuery<ProductVendor>("[SP_ProductMaster] @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@Vendors={4}", "SelectInvoiceMatchProducts", InvoiceProduct.UPCCode, InvoiceProduct.ItemNo, InvoiceProduct.Description, InvoiceProduct.Vendors).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - SelectInvoiceMatchProducts - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Products;
        }
        public List<ProductsList> UrlDatasourceSearchListGet(string search)
        {
            List<ProductsList> Products = new List<ProductsList>();
            try
            {
                Products = db.Database.SqlQuery<ProductsList>("[SP_ProductMaster] @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@Brand={4},@Size={5}", "SelectByFilter", search != null ? search : search, search != null ? search : search, search != null ? search : search, search != null ? search : search, search != null ? search : search).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - UrlDatasourceSearchListGet - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Products;
        }
        public List<StoreNickName> SelectProductListbyvendorname(string vendorname)
        {
            List<StoreNickName> StoreNickName = new List<StoreNickName>();
            try
            {
                StoreNickName = db.Database.SqlQuery<StoreNickName>("SP_VendorMaster @Mode={0},@VendorName={1}", "SelectProductList", vendorname).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - SelectProductListbyvendorname - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return StoreNickName;
        }
        public void UpdateProductInvoice(Products products, int ProductVendorId)
        {
            try
            {
                db.Database.ExecuteSqlCommand("[SP_ProductMaster] @Mode={0},@ProductVendorId={1},@ProductId={2},@UPC={3},@ItemNo={4}", "UpdateProductInvoice", ProductVendorId, products.ProductId, products.UPCCode, products.ItemNo);
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - UpdateProductInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateProductInvoice1(int ProductId, int ProductVendorId)
        {
            try
            {
                db.Database.ExecuteSqlCommand("[SP_ProductMaster] @Mode={0},@ProductVendorId={1},@ProductId={2}", "UpdateProductInvoice", ProductVendorId, ProductId);
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - UpdateProductInvoice1 - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void DeleteVendorItem(ProductVendor pv)
        {
            try
            {
                db.productVendors.Remove(pv);
                db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - DeleteVendorItem - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public void UpdateProductVendors(int ProductVendorId)
        {
            try
            {
                var InvoiceProduct = db.productVendors.Find(Convert.ToInt32(ProductVendorId));
                if (InvoiceProduct != null)
                {
                    InvoiceProduct.ProductId = null;
                    db.Entry(InvoiceProduct).State = EntityState.Modified;
                    db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - UpdateProductVendors - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
        public string GetInvoice_StoreList(int UserID)
        {
            string strList = "";
            try
            {
                var list = db.Database.SqlQuery<int>("SP_GetStore_ForDashboard @Mode={0},@ModuleId={1},@UserTypeId={2}", "GetStore_ForDashboard", 1, UserID);
                if (list != null) { strList = String.Join(",", list); }
                list = null;
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - GetInvoice_StoreList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return strList;
        }
        public List<Products> GetproductsList(int? ProductId)
        {
            List<Products> products = new List<Products>();
            try
            {
                products = db.products.Where(s => s.ProductId == ProductId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - GetproductsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return products;
        }
        public List<Products> GetproductsList()
        {
            List<Products> products = new List<Products>();
            try
            {
                products = db.products.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - GetproductsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return products;
        }
        public void AddproductsList(Products products)
        {
            try
            {
                db.products.Add(products);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("InvoicePreviewRepository - AddproductsList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
