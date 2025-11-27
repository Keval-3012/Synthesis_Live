using EntityModels.Models;
using NLog;
using Repository.IRepository;
using SynthesisViewModal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class LineItemRepository : ILineItemRepository
    {
        private DBContext _context;
        Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ICommonRepository _CommonRepository;

        public LineItemRepository(DBContext context)
        {
            _context = context;
            this._CommonRepository = new CommonRepository(new DBContext());
        }
        
        public void AddVendorLineItem(LineItemList value)
        {
            try
            {
                InvoiceProduct invoiceProduct = _context.InvoiceProducts.Where(w => w.InvoiceProductId == value.InvoiceProductId).FirstOrDefault();
                Invoice invoice = _context.Invoices.Where(w => w.InvoiceId == invoiceProduct.InvoiceId).FirstOrDefault();
                VendorMaster vendor = _context.VendorMasters.Where(w => w.VendorId == invoice.VendorId).FirstOrDefault();
                ProductVendor productVendor = new ProductVendor();
                productVendor.ItemNo = invoiceProduct.ItemNo;
                productVendor.UPCCode = invoiceProduct.UPCCode;
                productVendor.Description = invoiceProduct.Description;
                productVendor.Vendors = vendor.VendorName;
                productVendor.Price = invoiceProduct.UnitPrice.ToString();
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                productVendor.CreatedBy = _CommonRepository.getUserId(UserName);
                productVendor.DateCreated = DateTime.Now;
                productVendor.IsManually = 1;
                _context.productVendors.Add(productVendor);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - AddVendorLineItem - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public InvoiceProduct GetInvoiceProductById(int Id)
        {
            InvoiceProduct invoicePro = new InvoiceProduct();
            try
            {
                invoicePro = _context.InvoiceProducts.Find(Id);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetInvoiceProductById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoicePro;
        }

        public List<LineItemInvoiceNew> GetLineItemInvoiceDetails(int? StoreIds, string vendorName, DateTime? easternTime, string checkvalue)
        {
            List<LineItemInvoiceNew> lineitem = new List<LineItemInvoiceNew>();
            try
            {
                lineitem = _context.Database.SqlQuery<LineItemInvoiceNew>("SP_LineItemDetail @Mode = {0}, @StoreId = {1}, @VendorName = {2}, @Date = {3},@checkvalue ={4}", "GetLineItemInvoiceDetail", StoreIds, vendorName == null ? "" : vendorName, easternTime, checkvalue).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetLineItemInvoiceDetails - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return lineitem;
        }

        public ProductStoreAndVendorName GetPopupHeaderData(int? value)
        {
            ProductStoreAndVendorName PSVName = new ProductStoreAndVendorName();
            try
            {
                PSVName = _context.Database.SqlQuery<ProductStoreAndVendorName>("SP_MapLineItemList @Mode={0},@InvoiceId={1}", "GetPopupHeaderData", value).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetPopupHeaderData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return PSVName;
        }

        public List<ProductsList> GetRecommendedProductList(InvoiceProduct invoiceproduct, int Id)
        {
            List<ProductsList> productlist = new List<ProductsList>();
            try
            {
                productlist = _context.Database.SqlQuery<ProductsList>("SP_MapLineItemList @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@InvoiceProductId={4}", "GetRecommendedProductList", invoiceproduct.UPCCode != null ? invoiceproduct.UPCCode.Trim() : invoiceproduct.UPCCode, invoiceproduct.ItemNo != null ? invoiceproduct.ItemNo.Trim() : invoiceproduct.ItemNo, invoiceproduct.Description != null ? invoiceproduct.Description.Trim() : invoiceproduct.Description, Id).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetRecommendedProductList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return productlist;
        }

        public List<ProductsList> GetRecommendedProductListWithVendorName(InvoiceProduct invoiceproduct, int Id, string vendorName)
        {
            List<ProductsList> productlist = new List<ProductsList>();
            try
            {
                productlist = _context.Database.SqlQuery<ProductsList>("SP_MapLineItemList @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@InvoiceProductId={4}, @VendorName={5}", "GetRecommendedProductList", invoiceproduct.UPCCode != null ? invoiceproduct.UPCCode.Trim() : invoiceproduct.UPCCode, invoiceproduct.ItemNo != null ? invoiceproduct.ItemNo.Trim() : invoiceproduct.ItemNo, invoiceproduct.Description != null ? invoiceproduct.Description.Trim() : invoiceproduct.Description, Id, vendorName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetRecommendedProductListWithVendorName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return productlist;
        }

        public List<productVendorAllList> GetRecommendedProductVendorList(InvoiceProduct productVendorAllList, int Id, string vendorName)
        {
            List<productVendorAllList> productvendorlist = new List<productVendorAllList>();
            try
            {
                productvendorlist = _context.Database.SqlQuery<productVendorAllList>("SP_MapLineItemList @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@InvoiceProductId={4}, @VendorName={5}", "GetRecommendedProductVendorList", productVendorAllList.UPCCode != null ? productVendorAllList.UPCCode.Trim() : productVendorAllList.UPCCode, productVendorAllList.ItemNo != null ? productVendorAllList.ItemNo.Trim() : productVendorAllList.ItemNo, productVendorAllList.Description != null ? productVendorAllList.Description.Trim() : productVendorAllList.Description, Id, vendorName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetRecommendedProductVendorList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return productvendorlist;
        }

        public List<ProductsList> SelectByFilter(string search, string Id)
        {
            List<ProductsList> productlist = new List<ProductsList>();
            try
            {
                productlist = _context.Database.SqlQuery<ProductsList>("SP_LineItemDetail @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@InvoiceProductId={4},@Brand={5},@Size={6}", "SelectByFilter", search != null ? search : search, search != null ? search : search, search != null ? search : search, Id, search != null ? search : search, search != null ? search : search).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - SelectByFilter - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return productlist;
        }

        public List<ProductsList> SelectByFilterProduct(string search, int Id)
        {
            List<ProductsList> productlist = new List<ProductsList>();
            try
            {
                productlist = _context.Database.SqlQuery<ProductsList>("SP_MapLineItemList @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@InvoiceProductId={4},@Brand={5},@Size={6}", "SelectByFilterProduct", search != null ? search : search, search != null ? search : search, search != null ? search : search, Id, search != null ? search : search, search != null ? search : search, search != null ? search : search).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - SelectByFilterProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return productlist;
        }

        public List<productVendorAllList> SelectByFilterProductVendor(string search2, int Id)
        {
            List<productVendorAllList> productvendorlist = new List<productVendorAllList>();
            try
            {
                productvendorlist = _context.Database.SqlQuery<productVendorAllList>("SP_MapLineItemList @Mode={0},@UPC={1},@ItemNo={2},@Description={3},@InvoiceProductId={4},@Brand={5},@Size={6}", "SelectByFilterProductVendor", search2 != null ? search2 : search2, search2 != null ? search2 : search2, search2 != null ? search2 : search2, Id, search2 != null ? search2 : search2, search2 != null ? search2 : search2).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - SelectByFilterProductVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return productvendorlist;
        }

        public void Update_UPC(string UPCCode)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_Update_Mapping_Proc @UPC = {0}", UPCCode);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - Update_UPC - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<VendorNameList> VendorList()
        {
            List<VendorNameList> vendorlist = new List<VendorNameList>();
            try
            {
                vendorlist = (from I in _context.Invoices
                              join Ip in _context.InvoiceProducts on I.InvoiceId equals Ip.InvoiceId
                              join S in _context.StoreMasters on I.StoreId equals S.StoreId
                              join V in _context.VendorMasters on I.VendorId equals V.VendorId
                              select new VendorNameList
                              {
                                  VendorName = V.VendorName
                              }).Distinct().OrderBy(o => o.VendorName).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - VendorList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendorlist;
        }

        public List<LineItemList> GetLineItemInvoiceitemDetail(object Val)
        {
            List<LineItemList> LineItemList = new List<LineItemList>();
            try
            {
                LineItemList = _context.Database.SqlQuery<LineItemList>("SP_LineItemDetail @Mode = {0}, @InvoiceId = {1}", "GetLineItemInvoiceitemDetail", Val).ToList();

            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetLineItemInvoiceitemDetail - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return LineItemList;
        }

        public List<VendorNameList> VendorListByStoreId(int? StoreIds)
        {
            List<VendorNameList> vendorlist = new List<VendorNameList>();
            try
            {
                vendorlist = (from I in _context.Invoices
                              join Ip in _context.InvoiceProducts on I.InvoiceId equals Ip.InvoiceId
                              join S in _context.StoreMasters on I.StoreId equals S.StoreId
                              join V in _context.VendorMasters on I.VendorId equals V.VendorId
                              select new VendorNameList
                              {
                                  StoreId = I.StoreId,
                                  VendorName = V.VendorName
                              }).Distinct().Where(w => w.StoreId == StoreIds).OrderBy(o => o.VendorName).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - VendorListByStoreId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return vendorlist;
        }

        public void AssignProduct(ProductsList products)
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int UserId = _CommonRepository.getUserId(UserName);
                InvoiceProduct invoiceProduct = _context.InvoiceProducts.Find(products.InvoiceProductId);
                _context.Database.ExecuteSqlCommand("SP_UpdateLineItemDetail @Mode={0},@ProductId={1},@UPCCode={2},@ItemNo={3},@InvoiceProductId={4},@UserID={5},@ProductVendorId={6}", "GetLineItemInvoiceDetail", products.ProductId, invoiceProduct.UPCCode, invoiceProduct.ItemNo, products.InvoiceProductId, UserId, products.ProductVendorId);

            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - AssignProduct - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public Invoice GetInvoiceById(long Id)
        {
            Invoice invoice = new Invoice();
            try
            {
                invoice = _context.Invoices.Find(Id);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetInvoiceById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return invoice;
        }

        public List<ProductsList> GetProductMasterData(LineItemList value)
        {
            List<ProductsList> productlist = new List<ProductsList>();
            try
            {
                productlist = _context.Database.SqlQuery<ProductsList>("SP_LineItemDetail @Mode = {0}, @ProductId = {1}", "GetProductVendor", value.ProductId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetProductMasterData - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return productlist;
        }

        public void RemoveProductmapping(InvoiceProduct invoiceproduct)
        {
            try
            {
                var productId = invoiceproduct.ProductId;
                if (invoiceproduct != null)
                {
                    invoiceproduct.ProductId = null;
                    _context.Entry(invoiceproduct).State = EntityState.Modified;
                    _context.SaveChangesAsync();
                }
                _context.Database.ExecuteSqlCommand("SP_LineItemDetail @Mode={0},@ProductId={1},@Value={2}", "UpdateProductFlag", productId, 0);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - RemoveProductmapping - " + DateTime.Now + " - " + ex.Message.ToString());
            }
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
                logger.Error("LineItemRepository - GetStoreId - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storeMaster;
        }

        public int? InvoiceProducts(string UPC)
        {
            int? ID = 0;
            try
            {
                ID = _context.products.Where(x => x.UPCCode == UPC || x.ItemNo == UPC).Select(x => x.ProductId).FirstOrDefault();
                //ID = _context.InvoiceProducts.Where(x => x.UPCCode == UPC).Select(x => x.ProductId).FirstOrDefault();
                if (ID == null)
                {
                    ID = _context.products.Where(x => x.ItemNo == UPC || x.ItemNo == UPC).Select(x => x.ProductId).FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - InvoiceProducts - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ID;
        }

        public Products GetProductsById(int? Id)
        {
            Products products = new Products();
            try
            {
                products = _context.products.Find(Id);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetProductsById - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return products;
        }

        public void Unlink_ItemLines(LineItemList value)
        {
            try
            {
                var UserName = System.Web.HttpContext.Current.User.Identity.Name;
                int UserId = _CommonRepository.getUserId(UserName);
                _context.Database.SqlQuery<ProductsList>("SP_UpdateLineItemDetail @Mode = {0}, @UPCCode = {1}, @ItemNo = {2}, @UserID = {3}, @InvoiceProductId = {4}", "Unlink_ItemLines", value.UPCCode, value.ItemNo, UserId, value.InvoiceProductId).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - Unlink_ItemLines - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void Unlink_ItemLines_Lookup(string ItemName)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("SP_LineItemDetail @Mode = {0},@Description = {1}", "UpdateInvoiceProductColumn", ItemName);
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - Unlink_ItemLines_Lookup - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public ProductPriceModel GetMaxAndAveragePrice(int? ID)
        {
            
            ProductPriceModel productPrice = new ProductPriceModel();
            try
            {
                productPrice = _context.Database.SqlQuery<ProductPriceModel>("SP_LineItemDetail @Mode={0},@ProductId={1}", "GetMaxAndAveragePrice", ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetMaxAndAveragePrice - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return productPrice;
        }

        public StoreCount GetMAXStoreName(int? Id)
        {
            StoreCount storecount = new StoreCount();
            try
            {
                storecount = _context.Database.SqlQuery<StoreCount>("SP_LineItemDetail @Mode={0},@ProductId={1}", "GetMAXStoreName", Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetMAXStoreName - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return storecount;
        }

        public List<LineItemViewModel> GetInvoiceQty(int? id)
        {
            List<LineItemViewModel> model = new List<LineItemViewModel>();
            try
            {
                model = (from In in _context.Invoices
                         join Ip in _context.InvoiceProducts on In.InvoiceId equals Ip.InvoiceId
                         join sm in _context.StoreMasters on In.StoreId equals sm.StoreId
                         where Ip.ProductId == id
                         select new LineItemViewModel
                         {
                            Qty = Ip.Qty,
                            Name = sm.Name
                         }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetInvoiceQty - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return model;
        }

        public List<LineItemsLookUp> LineItemLookUp(int ProductId, int StoreIds, string InvoiceNumber)
        {
            List<LineItemsLookUp> list = new List<LineItemsLookUp>();
            try
            {
                if (StoreIds != 0 && InvoiceNumber != "")
                {
                    list = (from I in _context.Invoices
                            join Ip in _context.InvoiceProducts on I.InvoiceId equals Ip.InvoiceId
                            join S in _context.StoreMasters on I.StoreId equals S.StoreId
                            join V in _context.VendorMasters on I.VendorId equals V.VendorId
                            where (Ip.ProductId == ProductId)
                            select new
                            {
                                InvoiceProductId = Ip.InvoiceProductId,
                                StoreId = I.StoreId,
                                NickName = S.NickName,
                                InvoiceNumber = I.InvoiceNumber,
                                Description = Ip.Description,
                                InvoiceDate = I.InvoiceDate,
                                Qty = Ip.Qty,
                                UnitPrice = Ip.UnitPrice,
                                VendorName = V.VendorName,
                                InvoiceId = I.InvoiceId,

                            }).ToList().Select(x => new LineItemsLookUp
                            {
                                InvoiceProductId = x.InvoiceProductId,
                                StoreName = x.NickName,
                                InvoiceNumber = x.InvoiceNumber,
                                ItemName = x.Description,
                                InvoiceDate = Convert.ToDateTime(x.InvoiceDate).ToString("dd/MM/yyyy"),
                                Qty = x.Qty.ToString(),
                                UnitPrice = x.UnitPrice,
                                VendorName = x.VendorName,
                                StoreId = (int)x.StoreId,
                                InvoiceId = x.InvoiceId
                            }).Where(w => w.StoreId == StoreIds && w.InvoiceNumber.Contains(InvoiceNumber)).ToList();
                }
                else if (StoreIds != 0 && InvoiceNumber == "")
                {
                    list = (from I in _context.Invoices
                            join Ip in _context.InvoiceProducts on I.InvoiceId equals Ip.InvoiceId
                            join S in _context.StoreMasters on I.StoreId equals S.StoreId
                            join V in _context.VendorMasters on I.VendorId equals V.VendorId

                            where (Ip.ProductId == ProductId)
                            select new
                            {
                                InvoiceProductId = Ip.InvoiceProductId,
                                StoreId = I.StoreId,
                                NickName = S.NickName,
                                InvoiceNumber = I.InvoiceNumber,
                                Description = Ip.Description,
                                InvoiceDate = I.InvoiceDate,
                                Qty = Ip.Qty,
                                UnitPrice = Ip.UnitPrice,
                                VendorName = V.VendorName,
                                InvoiceId = I.InvoiceId,

                            }).ToList().Select(x => new LineItemsLookUp
                            {
                                InvoiceProductId = x.InvoiceProductId,
                                StoreName = x.NickName,
                                InvoiceNumber = x.InvoiceNumber,
                                ItemName = x.Description,
                                InvoiceDate = Convert.ToDateTime(x.InvoiceDate).ToString("dd/MM/yyyy"),
                                Qty = x.Qty.ToString(),
                                UnitPrice = x.UnitPrice,
                                VendorName = x.VendorName,
                                StoreId = (int)x.StoreId,
                                InvoiceId = x.InvoiceId
                            }).Where(w => w.StoreId == StoreIds).ToList();
                }
                else if (StoreIds == 0 && InvoiceNumber != "")
                {
                    list = (from I in _context.Invoices
                            join Ip in _context.InvoiceProducts on I.InvoiceId equals Ip.InvoiceId
                            join S in _context.StoreMasters on I.StoreId equals S.StoreId
                            join V in _context.VendorMasters on I.VendorId equals V.VendorId

                            where (Ip.ProductId == ProductId)
                            select new
                            {
                                InvoiceProductId = Ip.InvoiceProductId,
                                StoreId = I.StoreId,
                                NickName = S.NickName,
                                InvoiceNumber = I.InvoiceNumber,
                                Description = Ip.Description,
                                InvoiceDate = I.InvoiceDate,
                                Qty = Ip.Qty,
                                UnitPrice = Ip.UnitPrice,
                                VendorName = V.VendorName,
                                InvoiceId = I.InvoiceId,

                            }).ToList().Select(x => new LineItemsLookUp
                            {
                                InvoiceProductId = x.InvoiceProductId,
                                StoreName = x.NickName,
                                InvoiceNumber = x.InvoiceNumber,
                                ItemName = x.Description,
                                InvoiceDate = Convert.ToDateTime(x.InvoiceDate).ToString("dd/MM/yyyy"),
                                Qty = x.Qty.ToString(),
                                UnitPrice = x.UnitPrice,
                                VendorName = x.VendorName,
                                StoreId = (int)x.StoreId,
                                InvoiceId = x.InvoiceId
                            }).Where(w => w.InvoiceNumber.Contains(InvoiceNumber)).ToList();
                }
                else
                {
                    list = (from I in _context.Invoices
                            join Ip in _context.InvoiceProducts on I.InvoiceId equals Ip.InvoiceId
                            join S in _context.StoreMasters on I.StoreId equals S.StoreId
                            join V in _context.VendorMasters on I.VendorId equals V.VendorId

                            where (Ip.ProductId == ProductId)
                            select new
                            {
                                InvoiceProductId = Ip.InvoiceProductId,
                                StoreId = I.StoreId,
                                NickName = S.NickName,
                                InvoiceNumber = I.InvoiceNumber,
                                Description = Ip.Description,
                                InvoiceDate = I.InvoiceDate,
                                Qty = Ip.Qty,
                                UnitPrice = Ip.UnitPrice,
                                VendorName = V.VendorName,
                                InvoiceId = I.InvoiceId,

                            }).ToList().Select(x => new LineItemsLookUp
                            {
                                InvoiceProductId = x.InvoiceProductId,
                                StoreName = x.NickName,
                                InvoiceNumber = x.InvoiceNumber,
                                ItemName = x.Description,
                                InvoiceDate = Convert.ToDateTime(x.InvoiceDate).ToString("dd/MM/yyyy"),
                                Qty = x.Qty.ToString(),
                                UnitPrice = x.UnitPrice,
                                VendorName = x.VendorName,
                                StoreId = (int)x.StoreId,
                                InvoiceId = x.InvoiceId
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - LineItemLookUp - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<VendoreLookUp> VendorLookupList(int ProductId)
        {
            List<VendoreLookUp> list = new List<VendoreLookUp>();
            try
            {
                list = (from I in _context.Invoices
                        join Ip in _context.InvoiceProducts on I.InvoiceId equals Ip.InvoiceId

                        join V in _context.VendorMasters on I.VendorId equals V.VendorId
                        where (Ip.ProductId == ProductId)
                        select new
                        {
                            VendorName = V.VendorName,
                        }).ToList().Select(x => new VendoreLookUp
                        {
                            VendorName = x.VendorName
                        }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - VendorLookupList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<ProductPriceModel> Getlinechart(int ProductId, string startDate, string endDate, string storeId)
        {
            List<ProductPriceModel> list = new List<ProductPriceModel>();
            try
            {
                list = (from Result in _context.Database.SqlQuery<SPProductLineChart_Result>("SP_LineItemDetail @Mode = {0}, @ProductId = {1}, @StartDate = {2}, @EndDate = {3}, @StoreId = {4}", "getlinechart", ProductId, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), storeId)
                        select new ProductPriceModel
                        {

                            UnitPrice = Convert.ToDecimal(Result.Unitprice),
                            Qty = Result.Qtys.ToString(),
                            Date = Convert.ToDateTime(Result.InvoiceDate).ToString("yyyy-MM-dd")

                        }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - Getlinechart - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public void UploadImage(byte[] bytes, int ProductId)
        {
            try
            {
                Products products = _context.products.Find(ProductId);
                products.ProductImage = Convert.ToBase64String(bytes);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - UploadImage - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public void DeleteImage(int ProductId)
        {
            try
            {
                Products products = _context.products.Find(ProductId);
                products.ProductImage = null;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - DeleteImage - " + DateTime.Now + " - " + ex.Message.ToString());
            }
        }

        public List<StorePieChartModel> StoregetPiechart(int ProductId)
        {
            List<StorePieChartModel> list = new List<StorePieChartModel>();
            try
            {
                list = (from Result in _context.Database.SqlQuery<StorePieChartModel>("SP_LineItemDetail @Mode = {0},@ProductId = {1}", "StoregetPiechart", ProductId)
                        select new StorePieChartModel
                        {

                            StoreQty = Convert.ToInt32(Result.StoreQty),
                            StoreName = Result.StoreName

                        }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - StoregetPiechart - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public Products GetOrderByDescending()
        {
            Products product = new Products();
            try
            {
                product = _context.products.OrderByDescending(u => u.ProductId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - GetOrderByDescending - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return product;
        }

        public int LineItemInvoiceCount(string itemname)
        {
            int ivcount = 0;
            try
            {
                if(itemname != "")
                {
                    ivcount = _context.InvoiceProducts.Where(x => x.Description == itemname).Count();
                }
            }
            catch (Exception ex)
            {
                logger.Error("LineItemRepository - LineItemInvoiceCount - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return ivcount;
        }
    }
}
