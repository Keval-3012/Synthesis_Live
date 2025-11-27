using EntityModels.Models;
using NLog;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ProductPriceRepository : IProductPriceRepository
    {
        private DBContext _context;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();

        public ProductPriceRepository(DBContext context)
        {
            _context = context;
            _CommonRepository = new CommonRepository(context);
        }

        public List<VendorNameList> GetsalePriceList()
        {
            List<VendorNameList> list = new List<VendorNameList>();
            try
            {
                list = _context.Database.SqlQuery<VendorNameList>("SP_ProductPrice @Spara = {0}", 5).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - GetsalePriceList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public DataTable getProductPriceList(string Upccode, string FromDate, string ToDate)
        {
            DataTable dt = new DataTable();
            try
            {
                string EndDate = "";
                string StartDate = "";
                if (String.IsNullOrEmpty(FromDate) && String.IsNullOrEmpty(ToDate))
                {
                    DateTime currentDate = DateTime.Now;
                    EndDate = currentDate.AddDays(15).ToString("yyyy-MM-dd"); // currentDate.ToString("yyyy-MM-dd");
                    currentDate = currentDate.AddDays(-15);
                    StartDate = "2022-01-01";
                }
                else
                {
                    StartDate = (Convert.ToDateTime(DateTime.ParseExact(FromDate, "MM/dd/yyyy", CultureInfo.InvariantCulture))).ToString("yyyy-MM-dd");
                    EndDate = (Convert.ToDateTime(DateTime.ParseExact(ToDate, "MM/dd/yyyy", CultureInfo.InvariantCulture))).ToString("yyyy-MM-dd");
                }

                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "Product_Price_Deviation_Proc";
                Cmd.Parameters.AddWithValue("@StartDate", StartDate);
                Cmd.Parameters.AddWithValue("@EndDate", EndDate);
                Cmd.Parameters.AddWithValue("@UPCSearch", Upccode);
                dt = _CommonRepository.Select(Cmd);
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - GetProductPriceList - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dt;
        }

        public List<ProductPriceModel> getlinechart(string Upccode, string startDate, string endDate, string storeId)
        {
            List<ProductPriceModel> list = new List<ProductPriceModel>();
            try
            {
                list = (from Result in _context.Database.SqlQuery<SPProductLineChart_Result>("SPProductLineChart @Spara = {0}, @UPCCode = {1}, @StartDate = {2}, @EndDate = {3}, @StoreId = {4}", "1", Upccode, Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), storeId)
                        select new ProductPriceModel
                        {
                            UnitPrice = Convert.ToDecimal(Result.Unitprice),
                            Qty = Result.Qty,
                            Date = Convert.ToDateTime(Result.Invoice_Date).ToString("yyyy-MM-dd")

                        }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - Getlinechart - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<VendorPiChartModel> VendorgetPiechart(string Upccode)
        {
            List<VendorPiChartModel> list = new List<VendorPiChartModel>();
            try
            {
                list = (from Result in _context.Database.SqlQuery<VendorPiChartModel>("SPVendorPiChart @Spara = {0},@UPCCode = {1}", "1", Upccode)
                        select new VendorPiChartModel
                        {
                            VenQty = Convert.ToInt32(Result.VenQty),
                            VenName = Result.VenName

                        }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - VendorgetPiechart - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<StorePieChartModel> StoregetPiechart(string Upccode)
        {
            List<StorePieChartModel> list = new List<StorePieChartModel>();
            try
            {
                list = (from Result in _context.Database.SqlQuery<StorePieChartModel>("SPStorePieChart @Spara = {0},@UPCCode = {1}", "1", Upccode)
                        select new StorePieChartModel
                        {
                            StoreQty = Convert.ToInt32(Result.StoreQty),
                            StoreName = Result.StoreName

                        }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - StoregetPiechart - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<LineItemsLookUp> LineItemsLookup(string Upccode, int StoreIds, string InvoiceNumber)
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

                            where (Ip.UPCCode.Trim().Equals(Upccode))
                            select new
                            {
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

                            where (Ip.UPCCode.Trim().Equals(Upccode))
                            select new
                            {
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

                            where (Ip.UPCCode.Trim().Equals(Upccode))
                            select new
                            {
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

                            where (Ip.UPCCode.Trim().Equals(Upccode))
                            select new
                            {
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
                logger.Error("ProductPriceRepository - LineItemsLookup - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<InvoiceLookup> InvoiceLookup(string Upccode)
        {
            List<InvoiceLookup> list = new List<InvoiceLookup>();
            try
            {
                list = _context.Database.SqlQuery<InvoiceLookup>("SP_ProductPrice @Spara = {0},@UPCCode = {1}", 3, Upccode).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - InvoiceLookup - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<StoreWiseLookup> StoreWiseLookup(string Upccode)
        {
            List<StoreWiseLookup> list = new List<StoreWiseLookup>();
            try
            {
                list = (from In in _context.Invoices
                        join Ip in _context.InvoiceProducts on In.InvoiceId equals Ip.InvoiceId
                        join sm in _context.StoreMasters on In.StoreId equals sm.StoreId
                        where Ip.UPCCode.Trim().Equals(Upccode)
                        select new StoreWiseLookup
                        {
                            StoreName = sm.Name
                        }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - StoreWiseLookup - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<VendorNameList> ViewBagVendor()
        {
            List<VendorNameList> list = new List<VendorNameList>();
            try
            {
                list = _context.Database.SqlQuery<VendorNameList>("SP_ProductPrice @Spara = {0}", 5).ToList();
                list.Insert(0, new VendorNameList { VendorName = "All Vendors" });
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - ViewBagVendor - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public List<DepartmentMaster> ViewBagDepartment()
        {
            List<DepartmentMaster> list = new List<DepartmentMaster>();
            try
            {
                var DepartmentMasters = _context.Database.SqlQuery<DepartmentMaster>("SP_DepartmentMaster @Mode = {0},@StoreId = {1}", "SelectExpense_Department", 1).ToList();
                list = (from I in DepartmentMasters
                                      select new DepartmentMaster
                                      {
                                          DepartmentName = I.DepartmentName
                                      }).Distinct().ToList();
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - ViewBagDepartment - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return list;
        }

        public DataTable ProductPriceGrid(string[] value, string Upccode, string vendorname, string ColorFilter, int Radio)
        {
            DataTable dt = new DataTable();
            try
            {
                if (vendorname == "All Vendors")
                {
                    vendorname = "All";
                }
                string EndDate = "";
                string StartDate = "";
                if (value.Count() == 0)
                {
                    DateTime currentDate = DateTime.Now;
                    EndDate = currentDate.AddDays(15).ToString("yyyy-MM-dd");
                    currentDate = currentDate.AddDays(-15);
                    StartDate = "2022-01-01";
                }
                else
                {
                    DateTime date = Convert.ToDateTime(value[0]);
                    DateTime date1 = Convert.ToDateTime(value[1]);
                    StartDate = date.ToString("yyyy-MM-dd");
                    EndDate = date1.ToString("yyyy-MM-dd");
                }
                if (ColorFilter == "")
                {
                    ColorFilter = null;
                }

                var color = ColorFilter == null ? null : (ColorFilter.Substring(0, ColorFilter.Length - 1));
                SqlCommand Cmd = new SqlCommand();
                Cmd.CommandText = "Product_Price_Deviation_Proc";
                Cmd.Parameters.AddWithValue("@StartDate", StartDate);
                Cmd.Parameters.AddWithValue("@EndDate", EndDate);
                Cmd.Parameters.AddWithValue("@UPCSearch", Upccode);
                Cmd.Parameters.AddWithValue("@vendorsearch", vendorname);
                Cmd.Parameters.AddWithValue("@ColorFilter", color);
                Cmd.Parameters.AddWithValue("@Radio", Radio);
                dt = _CommonRepository.Select(Cmd);
            }
            catch (Exception ex)
            {
                logger.Error("ProductPriceRepository - ProductPriceGrid - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dt;
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
    }
}
