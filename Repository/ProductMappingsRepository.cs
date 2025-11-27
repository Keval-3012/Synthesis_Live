using EntityModels.Models;
using NLog;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Repository
{
    public class ProductMappingsRepository : IProductMappingsRepository
    {
        private DBContext _context;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();

        public ProductMappingsRepository(DBContext context)
        {
            _context = context;
        }

        public void AddVendor(string vendor)
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                _context.Database.ExecuteSqlCommand("SP_VendorProduct_Transfer_Proc @Vendors = {0},@UserID = {1}", vendor, _CommonRepository.getUserId(UserName));
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - AddVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public async Task<int> GetCountProduct(Products products)
        {
            int ProductCount = 0;
            try
            {
                ProductCount = await _context.products.Where(s => (s.UPCCode == products.UPCCode || s.ItemNo == products.ItemNo)).CountAsync();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetCountProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ProductCount;
        }

        public Products GetProduct()
        {
            Products pro = new Products();
            try
            {
                pro = _context.products.OrderByDescending(u => u.ProductId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return pro;
        }

        public void ProductAdd(Products products)
        {
            try
            {
                _context.products.Add(products);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - ProductAdd - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void ProductUpdate(Products products)
        {
            try
            {
                Products existingProduct = _context.products.FirstOrDefault(s => s.UPCCode == products.UPCCode || s.ItemNo == products.ItemNo);

                if (existingProduct != null)
                {
                    existingProduct.UPCCode = products.UPCCode;
                    existingProduct.ItemNo = products.ItemNo;  
                    existingProduct.Description = products.Description;   
                    existingProduct.Brand = products.Brand;  
                    existingProduct.Size = products.Size;    
                    existingProduct.DepartmentNumber = products.DepartmentNumber;
                    // Mark the entity as modified
                    _context.Entry(existingProduct).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    logger.Error("Product not found.");
                }
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - ProductUpdate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<Products> ProductList()
        {
            List<Products> productlist = new List<Products>();
            try
            {
                productlist = _context.products.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - ProductList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return productlist;
        }

        public List<Products> getProductListByID(int ID)
        {
            List<Products> Product = new List<Products>();

            try
            {
                var InvoiceProduct = _context.InvoiceProducts.Find(ID);
                Product = _context.Database.SqlQuery<Products>("[SP_ProductMaster] @Mode={0},@UPC={1},@ItemNo={2},@Description={3}", "SelectByFilter", InvoiceProduct.UPCCode != null ? InvoiceProduct.UPCCode.Trim() : InvoiceProduct.UPCCode, InvoiceProduct.ItemNo != null ? InvoiceProduct.ItemNo.Trim() : InvoiceProduct.ItemNo, InvoiceProduct.Description != null ? InvoiceProduct.Description.Trim() : InvoiceProduct.Description).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetProductListByID - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Product;
        }

        public List<Products> getSearchdata(string SearchVal)
        {
            List<Products> pro = new List<Products>();
            try
            {
                pro = _context.Database.SqlQuery<Products>("[SP_ProductMaster] @Mode={0},@UPC={1},@ItemNo={2},@Description={3}", "SelectByFilter", SearchVal != null ? SearchVal.Trim() : SearchVal, SearchVal != null ? SearchVal : SearchVal, SearchVal != null ? SearchVal.Trim() : SearchVal).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetSearchdata - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return pro;
        }

        public void UpdateProductInvoice(string selectedval)
        {
            List<ProductVendor> lstInvoiceProduct = new List<ProductVendor>();

            try
            {
                var ArrayVal = selectedval.Split('_');
                int Id = Convert.ToInt32(ArrayVal[1]);
                var InvoiceProduct = _context.productVendors.Find(Convert.ToInt32(ArrayVal[0]));
                lstInvoiceProduct = _context.Database.SqlQuery<ProductVendor>("[SP_ProductMaster] @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@Vendors={4}", "SelectInvoiceMatchProducts", InvoiceProduct.UPCCode, InvoiceProduct.ItemNo, InvoiceProduct.Description, InvoiceProduct.Vendors).ToList();
                if (lstInvoiceProduct.Count() > 0)
                {
                    foreach (var item in lstInvoiceProduct)
                    {
                        _context.Database.ExecuteSqlCommand("[SP_ProductMaster] @Mode={0},@ProductVendorId={1},@ProductId={2}", "UpdateProductInvoice", item.ProductVendorId, Id); //.Replace("&amp;","&").Replace("&apos;","'")
                    }
                }
                else
                {
                    _context.Database.ExecuteSqlCommand("[SP_ProductMaster] @Mode={0},@ProductVendorId={1},@ProductId={2}", "UpdateProductInvoice", InvoiceProduct.ProductVendorId, Id); //.Replace("&amp;","&").Replace("&apos;","'")
                }

            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - UpdateProductInvoice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            
        }

        public List<ProductVendor> AssignProduct(ProductVendor InvoiceProduct)
        {
            List<ProductVendor> Provendor = new List<ProductVendor>();
            try
            {
                Provendor = _context.Database.SqlQuery<ProductVendor>("[SP_ProductMaster] @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@Vendors={4}", "SelectInvoiceMatchProducts", InvoiceProduct.UPCCode, InvoiceProduct.ItemNo, InvoiceProduct.Description, InvoiceProduct.Vendors).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - AssignProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return Provendor;
        }

        public void ModifyProductMapping(string mappedProductId)
        {
            try
            {
                var InvoiceProduct = _context.productVendors.Find(Convert.ToInt32(mappedProductId));
                if (InvoiceProduct != null)
                {
                    InvoiceProduct.ProductId = null;
                    _context.Entry(InvoiceProduct).State = EntityState.Modified;
                    _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - ModifyProductMapping - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<Products> GetProductMasterData(int ID)
        {
            List<Products> product = new List<Products>();
            try
            {
                product = _context.products.Where(s => s.ProductId == ID).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetProductMasterData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return product;
        }

        public void updateKeyWord(int ProductID, string KeyWord)
        {
            Products products = new Products();
            try
            {
                products = _context.products.Find(ProductID);
                products.KeyWord = KeyWord;
                _context.Entry(products).State = EntityState.Modified;
                _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - UpdateKeyWord - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<ItemLibraryDepartment> GetItemLibraryDepartment()
        {
            List<ItemLibraryDepartment> ItemLibraryDepartment = new List<ItemLibraryDepartment>();
            try
            {
                ItemLibraryDepartment = _context.ItemLibraryDepartment.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetItemLibraryDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ItemLibraryDepartment;
        }

        public List<ProductsList> GetDataProductForUrl()
        {
            List<ProductsList> lstProducts = new List<ProductsList>();
            try
            {
                lstProducts = _context.Database.SqlQuery<ProductsList>("SP_ProductMaster @Mode = {0}", "GetDataProduct").ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetDataProductForUrl - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lstProducts;
        }

        public void InsertProduct(CRUDModel<Products> products)
        {
            try
            {
                products.Value.DateCreated = DateTime.Now;
                _context.products.Add(products.Value);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - InsertProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void UpdateProduct(CRUDModel<Products> products)
        {
            try
            {
                _context.Entry(products.Value).State = EntityState.Modified;
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - UpdateProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void RemoveProduct(CRUDModel<Products> products)
        {
            Products products1 = new Products();
            try
            {
                if (products.Deleted == null)
                {
                    products1 = _context.products.Find(products.Key);
                    _context.products.Remove(products1);
                }
                else
                {
                    foreach (var item in products.Deleted)
                    {
                        products1 = _context.products.Find(item.ProductId);
                        _context.products.Remove(products1);
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - RemoveProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public Products getSynthesisId()
        {
            Products pro = new Products();
            try
            {
                pro = _context.products.OrderByDescending(u => u.ProductId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetSynthesisId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return pro;
        }

        public List<StoreMaster> GetStoreId()
        {
            List<StoreMaster> storeMaster = new List<StoreMaster>();
            try
            {
                storeMaster = _context.StoreMasters.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetStoreId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storeMaster;
        }

        public ProductPriceModel ListProductPrice(string SynthesisId)
        {
            ProductPriceModel listProductPrice = new ProductPriceModel();
            try
            {
                listProductPrice = _context.Database.SqlQuery<ProductPriceModel>("SP_ProductMappingDashboars @Spara = {0},@UPCCode = {1}", 1, SynthesisId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - ListProductPrice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return listProductPrice;
        }

        public StoreCount ListStoreCount(string SynthesisId)
        {
            StoreCount listStoreCount = new StoreCount();
            try
            {
                listStoreCount = _context.Database.SqlQuery<StoreCount>("SP_ProductMappingDashboars @Spara = {0},@UPCCode = {1}", 2, SynthesisId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - ListStoreCount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return listStoreCount;
        }

        public List<Invoice> GetInvoices()
        {
            List<Invoice> invoice = new List<Invoice>();
            try
            {
                invoice = _context.Invoices.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetInvoices - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }

        public List<InvoiceProduct> GetInvoiceProduct()
        {
            List<InvoiceProduct> invoiceproduct = new List<InvoiceProduct>();
            try
            {
                invoiceproduct = _context.InvoiceProducts.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetInvoiceProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoiceproduct;
        }

        public List<Products> GetProductList()
        {
            List<Products> product = new List<Products>();
            try
            {
                product = _context.products.ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - GetProductList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return product;
        }

        public void CheckProductUpdate()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_TempToProductSave");
            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingsRepository - CheckProductUpdate - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }
    }
}
