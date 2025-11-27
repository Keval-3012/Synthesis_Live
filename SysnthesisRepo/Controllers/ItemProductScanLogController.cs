using DocumentFormat.OpenXml.Bibliography;
using EntityModels.Models;
using NLog;
using Repository;
using Repository.IRepository;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.GridExport;
using Syncfusion.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SysnthesisRepo.Controllers
{
    public class ItemProductScanLogController : Controller
    {
        private readonly IProductMappingLogRepository _ProductMappingLogRepository;
        private readonly ICommonRepository _CommonRepository;
        Logger logger = LogManager.GetCurrentClassLogger();
        public ItemProductScanLogController()
        {
            this._ProductMappingLogRepository = new ProductMappingLogRepository(new DBContext());
            this._CommonRepository = new CommonRepository(new DBContext());
        }

        // GET: ItemProductScanLog
        protected static string StatusMessage = "";
        protected static Array Arr;
        protected static bool IsArray;
        protected static IEnumerable BindData;
        protected static bool IsEdit = false;
        protected static int TotalDataCount;
        protected static string success = null;
        protected static string Error = null;

        /// <summary>
        /// This method return Item Product Scan Log.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,ViewItemProductScanLog")]
        public ActionResult ItemProductScanLog()
        {
            ViewBag.Title = "Item Product Scan Log - Synthesis";
            return View();
        }

        public ActionResult Grid()
        {
            var userscounts = _ProductMappingLogRepository.GetItemScannedUsersCount();
            ViewBag.TotalUsersCnt = userscounts.TotalUsers;
            ViewBag.ActivelyUsersCnt = userscounts.ActivatedUsers;
            ViewBag.PartiallyUsedUserCnt = userscounts.PartiallyUsedUsers;
            ViewBag.NotUsedUserCnt = userscounts.NotUsedUsers;

            int storeid = Convert.ToInt32(Session["storeid"]);
            ViewBag.Storeidvalue = storeid;

            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            int UserTypesID = _CommonRepository.getUserTypeId(UserName);
            ViewBag.UserTypeId = UserTypesID;

            if (storeid == 0 && UserTypesID != 1)
            {
                ViewBag.TotalUsersCnt = 0;
                ViewBag.ActivelyUsersCnt = 0;
                ViewBag.PartiallyUsedUserCnt = 0;
                ViewBag.NotUsedUserCnt = 0;
            }

            return View();
        }

        /// <summary>
        /// This method is url Data Source.
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public ActionResult UrlDatasource(DataManagerRequest dm, string Startdate = null, string Enddate = null)
        {
            int StoreId = 0;
            StoreId = Convert.ToInt32(Session["storeid"]);
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            int UserTypesID = _CommonRepository.getUserTypeId(UserName);
            //Db class is get Product Mapping Log.
            List<ItemProductScanLog> lstInvoiceUserProductMapLog = new List<ItemProductScanLog>();
            lstInvoiceUserProductMapLog = _ProductMappingLogRepository.GetItemProductLog(StoreId, Startdate, Enddate);
            if (StoreId == 0 && UserTypesID != 1)
            {
                lstInvoiceUserProductMapLog = new List<ItemProductScanLog>();
            }
            IEnumerable DataSource = lstInvoiceUserProductMapLog;
            DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                    DataSource = lstInvoiceUserProductMapLog.ToList().Where(x => (x.AppUserName.ToLower().Contains(search)) || (x.RoleName.ToLower().Contains(search))).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<ItemProductScanLog>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }

            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - UrlDatasource - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet) : Json(DataSource, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ExcelExport(string gridModel, string startdate, string enddate)
        {
            List<ItemProductScanLog> lstProducts = new List<ItemProductScanLog>();
            GridExcelExport exp = new GridExcelExport();
            int StoreId = 0;
            StoreId = Convert.ToInt32(Session["storeid"]);
            try
            {
                lstProducts = _ProductMappingLogRepository.GetItemProductLog(StoreId, startdate, enddate);
                exp.FileName = "ItemProductScanLog.xlsx";
            }
            catch (Exception ex)
            {
                logger.Error("ItemProductScanLogController - ExcelExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstProducts, startdate, enddate);
            return exp.ExcelExport<ItemProductScanLog>(gridProperty, lstProducts);
        }

        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty, List<ItemProductScanLog> data, string startdate, string enddate)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));
            try
            {
                int storeid = Convert.ToInt32(Session["storeid"]);
                GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
                if (startdate == "" && enddate == "")
                {
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "AppUserName", HeaderText = "Mobile App User" });
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "RoleName", HeaderText = "User Type" });
                    if (storeid == 0)
                    {
                        cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "StoreName", HeaderText = "Store Name" });
                    }
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ItemsScannedToday", HeaderText = "ItemsScanned Today" });
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ItemsScannedYesterday", HeaderText = "ItemsScanned Yesterday" });
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ItemsScannedThisWeek", HeaderText = "ItemsScanned ThisWeek" });
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ItemsScannedThisMonth", HeaderText = "ItemsScanned ThisMonth" });
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "TotalItemsScanned", HeaderText = "Total No. Of Item Scanned" });
                }
                else
                {
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "AppUserName", HeaderText = "Mobile App User" });
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "RoleName", HeaderText = "User Type" });
                    if (storeid == 0)
                    {
                        cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "StoreName", HeaderText = "Store Name" });
                    }
                    cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "TotalItemsScanned", HeaderText = "Total No. Of Item Scanned" });
                }


                foreach (var item in cols.columns)
                {
                    item.AutoFit = true;
                    item.Width = "50%";
                }

                GridModel.Columns = cols.columns;
            }
            catch (Exception ex)
            {
                logger.Error("ItemProductScanLogController - ConvertGridObject - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return GridModel;
        }

        public ActionResult PdfExport(string gridModel, string startdate, string enddate)
        {
            List<ItemProductScanLog> lstProducts = new List<ItemProductScanLog>();
            PdfDocument doc = new PdfDocument();
            GridPdfExport exp = new GridPdfExport();
            int StoreId = 0;
            StoreId = Convert.ToInt32(Session["storeid"]);
            try
            {
                lstProducts = _ProductMappingLogRepository.GetItemProductLog(StoreId, startdate, enddate);

                doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
                doc.PageSettings.Size = PdfPageSize.A3;

                exp.Theme = "flat-saffron";
                exp.FileName = "ItemProductScanLog.pdf";
                exp.PdfDocument = doc;
            }
            catch (Exception ex)
            {
                logger.Error("ItemProductScanLogController - PdfExport - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel, lstProducts, startdate, enddate);
            return exp.PdfExport<ItemProductScanLog>(gridProperty, lstProducts);
        }

        //Updated Code by Dani on 12-03-2025
        [Authorize(Roles = "Administrator")]
        public ActionResult BarcodeLookupGrid()
        {
            ViewBag.Title = "Item Product Detail - Synthesis";
            ViewBag.ReportIssue = _ProductMappingLogRepository.GetReportIssueDropDown().Select(s => new DropdownViewModel { Text = s.Text, Value = s.Value }).OrderBy(o => o.Value).ToList();
            ViewBag.ProductCount = _ProductMappingLogRepository.GetProductCount().Count();
            return View();
        }

        public ActionResult UrlDatasourceBarcodeLookup(DataManagerRequest dm, string searchproduct)
        {
            List<BarcodeLookupDetails> itembarcodedata = new List<BarcodeLookupDetails>();

            int skip = dm.Skip;
            int take = dm.Take != 0 ? dm.Take : 48;
            itembarcodedata = _ProductMappingLogRepository.GetBarcodeLookupData(searchproduct, skip, take);
            IEnumerable DataSource = itembarcodedata;
            DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
                //if (!string.IsNullOrEmpty(searchproduct))
                //{
                //    searchproduct = searchproduct.Trim().ToLower();
                //    DataSource = itembarcodedata.Where(x => (x.BarcodeNumber.ToLower().Contains(searchproduct)) || (x.Title.ToLower().Contains(searchproduct))).ToList();
                //}
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string search = dm.Search[0].Key.ToString().Trim().ToLower();
                    DataSource = itembarcodedata.ToList().Where(x => (x.BarcodeNumber.ToLower().Contains(search)) || (x.Title.ToLower().Contains(search))).ToList();
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<BarcodeLookupDetails>().Count();

                
                //DataSource = operation.PerformSkip(DataSource, skip);
                //DataSource = operation.PerformTake(DataSource, take);

                //if (dm.Skip != 0)
                //{
                //    DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
                //}
                //if (dm.Take != 0)
                //{
                //    DataSource = operation.PerformTake(DataSource, dm.Take);
                //}

            }
            catch (Exception ex)
            {
                logger.Error("ProductMappingLogController - UrlDatasourceBarcodeLookup - " + DateTime.Now + " - " + ex.Message.ToString());
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet) : Json(DataSource, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath) || !System.IO.File.Exists(imagePath))
                {
                    return File(Server.MapPath("~/Images/default.png"), "image/jpeg");
                }

                return File(imagePath, "image/jpeg");
            }
            catch (Exception ex)
            {
                logger.Error("Error serving image: " + ex.Message);
                return File(Server.MapPath("~/Images/default.png"), "image/jpeg");
            }
        }

        public ActionResult AddReportAnIssueForm(int ProductId, int? selectedIssueId, string AdditionNotes)
        {
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            int UserId = _CommonRepository.getUserId(UserName);
            try
            {
                _ProductMappingLogRepository.InsertReportIssue(ProductId, selectedIssueId, AdditionNotes, UserId, UserName);
                StatusMessage = "Success";

            }
            catch (Exception ex)
            {
                StatusMessage = "Error";
                logger.Error("ItemProductScanLogController - AddReportAnIssueForm - " + DateTime.Now + " - " + ex.Message.ToString());
            }

            return Json(new { StatusMessage = StatusMessage, UserName = UserName });
        }
        //Himanshu's code end here

        #region Competitors Scan Log Code start from Here By Himanshu on 16-10-2025

        [Authorize(Roles = "Administrator")]
        public ActionResult CompetitorsScanLogGrid()
        {
            ViewBag.Title = "Competitors Scan Log - Synthesis";
            ViewBag.UsersList = _ProductMappingLogRepository.GetUserLogUserList();
            return View();
        }

        public ActionResult UrlDatasourceCompetitorsLog(DataManagerRequest dm,int? UserId,string searchdate)
        {
            List<CompetitorsUserLog> Cum = new List<CompetitorsUserLog>();
            int count = 0;
            try
            {
                if (string.IsNullOrEmpty(searchdate))
                {
                    DateTime now = DateTime.Now;
                    searchdate = now.ToString("MMM-yyyy");
                }
                DateTime consearchdate = Convert.ToDateTime(searchdate.Trim());
                Cum = _ProductMappingLogRepository.GetCompetitorLogList(UserId, consearchdate);
                IEnumerable DataSource = Cum;

                DataOperations operation = new DataOperations();
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                {
                    DataSource = operation.PerformSorting(DataSource, dm.Sorted);
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }
                count = DataSource.Cast<CompetitorsUserLog>().Count();
                if (dm.Skip != 0)
                {
                    DataSource = operation.PerformSkip(DataSource, dm.Skip);//Paging
                }
                if (dm.Take != 0)
                {
                    DataSource = operation.PerformTake(DataSource, dm.Take);
                }
                return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
            }
            catch (Exception ex)
            {
                logger.Error("ItemProductScanLogController - UrlDatasourceCompetitorsLog - " + DateTime.Now + " - " + ex.Message.ToString());
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult PdfExport(string gridModel, int? UserId, string searchdate)
        {
            List<CompetitorsUserLog> BindData = new List<CompetitorsUserLog>();
            //This is get all Check List data with filter.
            DateTime consearchdate = Convert.ToDateTime(searchdate.Trim());
            BindData = _ProductMappingLogRepository.GetCompetitorLogList(UserId, consearchdate);
            PdfDocument doc = new PdfDocument();
            doc.PageSettings.Orientation = PdfPageOrientation.Landscape;
            doc.PageSettings.Size = PdfPageSize.A3;
            GridPdfExport exp = new GridPdfExport();
            exp.Theme = "flat-saffron";
            exp.FileName = "CompetitorsScanLog.pdf";
            exp.PdfDocument = doc;
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.PdfExport<CompetitorsUserLog>(gridProperty, BindData);
        }

        private Syncfusion.EJ2.Grids.Grid ConvertGridObject(string gridProperty)
        {
            Syncfusion.EJ2.Grids.Grid GridModel = (Syncfusion.EJ2.Grids.Grid)Newtonsoft.Json.JsonConvert.DeserializeObject(gridProperty, typeof(Syncfusion.EJ2.Grids.Grid));

            GridColumnModel cols = (GridColumnModel)Newtonsoft.Json.JsonConvert.DeserializeObject("{\"allowGrouping\":false,\"allowPaging\":false,\"pageSettings\":{\"currentPage\":1,\"pageCount\":2,\"pageSize\":100},\"sortSettings\":{},\"allowPdfExport\":true,\"allowExcelExport\":true,\"aggregates\":[],\"filterSettings\":{\"immediateModeDelay\":1500,\"type\":\"CheckBox\"},\"groupSettings\":{\"columns\":[],\"enableLazyLoading\":false,\"disablePageWiseAggregates\":false},\"columns\":[],\"locale\":\"en-US\",\"searchSettings\":{\"key\":\"\",\"fields\":[]}}", typeof(GridColumnModel));
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "ItemNoItemName", HeaderText = "ItemNo & ItemName" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "CompetitorsName", HeaderText = "Competitors" });

            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "LogDate", HeaderText = "Date" });
            cols.columns.Add(new Syncfusion.EJ2.Grids.GridColumn { Field = "StoreName", HeaderText = "Store Name" });

            foreach (var item in cols.columns)
            {
                item.AutoFit = true;
            }
            GridModel.Columns = cols.columns;
            return GridModel;
        }

        public ActionResult ExcelExport(string gridModel, int? UserId, string searchdate)
        {
            List<CompetitorsUserLog> BindData = new List<CompetitorsUserLog>();
            //This is get all Check List data with filter.
            DateTime consearchdate = Convert.ToDateTime(searchdate.Trim());
            BindData = _ProductMappingLogRepository.GetCompetitorLogList(UserId, consearchdate);

            GridExcelExport exp = new GridExcelExport();
            exp.FileName = "CompetitorsScanLog.xlsx";
            Syncfusion.EJ2.Grids.Grid gridProperty = ConvertGridObject(gridModel);
            return exp.ExcelExport<CompetitorsUserLog>(gridProperty, BindData);
        }

        #endregion Competitors Scan Log code end here
    }
}